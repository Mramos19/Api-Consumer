using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapaDatos;
using System.Data;
using Entidades;
using System.Configuration;

namespace CapaNegocio
{
    public class SNListaNegra
    {

        #region "Constructor"
        public SNListaNegra()
        {
            _IdOperadora = Convert.ToInt32(ConfigurationManager.AppSettings["IdOperadora"]);
            _Log = Convert.ToBoolean(ConfigurationManager.AppSettings["Log"]);
        }
        #endregion

        #region "Propiedades Privadas"

        private int _IdOperadora = 0;
        private enum CODE_ERROR : byte { SUCCESS = 0, SOLICITUD_PENDIENTE = 1, SOLICITUD_REPETIDA = 2, ERROR_INESPERADO = 98, ERROR_INTERNO = 99 };
        public bool _Log = false;

        #endregion

        #region "Metodos Privados"
        private SERequestBajaSACA GetdataRow(DataRow drow)
        {
            SERequestBajaSACA objLista = new SERequestBajaSACA();

            objLista.Celular = drow["Celular"].ToString();
            objLista.IdSuscripcion = Convert.ToInt32(drow["IdSuscripcion"].ToString());
            objLista.IdSuscripcionSACA = Convert.ToInt32(drow["IdSuscripcionSACA"].ToString());
            objLista.IdUsuario = objLista.IdUsuario;
            objLista.IdComando = objLista.IdComando;

            return objLista;
        }

        /*
       * Metodo que retorna las suscripciones SACA
       */
        private List<SERequestBajaSACA> ObtenerSuscripcionesSACA(int IdOperadora, string Celular)
        {
            List<SERequestBajaSACA> listaSuscripciones = new List<SERequestBajaSACA>();
            string sp = "SC.spObtenerSuscripcionYProductID";

            try
            {
                Conexion.SMSNI();



                listaSuscripciones = new List<SERequestBajaSACA>(
                    (from drow in Conexion.GDatos.ObtenerDataTable(sp, IdOperadora, Celular).AsEnumerable() select (GetdataRow(drow)))
                    );
            }
            catch (Exception ex)
            {
                SNError.IngresaError(1, 1, "WSIntegratorService", sp, ex.Message);
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return listaSuscripciones;
        }

        public bool ValidarListaNegra(int IdOperadora, string Celular)
        {

            string sp = string.Format("SELECT SMS.fnValidaListaNegra({0}, '{1}') AS Existe", IdOperadora, Celular);
            bool resultado = false;

            try
            {
                Conexion.SMS();

                resultado = Convert.ToBoolean(Conexion.GDatos.ObtenerValorScalarSql(sp));

            }
            catch (Exception ex)
            {
                SNError.IngresaError(1, 1, "WSIntegratorService", sp, ex.Message);
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return resultado;
        }

        private int IncluirEnListaNegra(int IdOperadora, string Celular, string Observacion, DateTime FechaSolicitud)
        {
            string sp = "SMS.spBajaMasivaXListaNegraInsertar";
            int resultado = 0;

            try
            {
                Conexion.SMS();
                Conexion.GDatos.IniciarTransaccion();
                resultado = Conexion.GDatos.Ejecutar(sp, IdOperadora, Celular, Observacion, FechaSolicitud);
                Conexion.GDatos.TerminarTransaccion();
            }
            catch (Exception ex)
            {
                Conexion.GDatos.AbortarTransaccion();
                SNError.IngresaError(1, 1, "WSIntegratorService", sp, ex.Message);
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return resultado;
        }

        private int ExcluirDeListaNegra(int IdOperadora, string Celular, string Observacion, DateTime FechaSolicitud)
        {
            string sp = "SMS.spLNGralActualizar";
            int resultado = 0;

            if (Celular.Trim().Length == 11)
            {
                Celular = Celular.Substring(3, 8);
            }

            try
            {
                Conexion.SMS();

                resultado = Conexion.GDatos.Ejecutar(sp, Celular, Observacion, FechaSolicitud, IdOperadora);

            }
            catch (Exception ex)
            {
                SNError.IngresaError(1, 1, "WSIntegratorService", sp, ex.Message);
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return resultado;
        }

        #endregion

        #region "Metodos Públicos"

        /*
         * Metodo para incluir en lista negra a un cliente por numero celular
         * se debe considerar lo siguiente:
         * -Validar que el cliente no exista en lista negra
         * -Verificar si esta suscrito tanto en MT y MO, de ser así se procede a desuscribir
         * -ingresar en lista negra
         */
        public SEResponseListaNegra ProcesarInclusion(SERequestListaNegra objRequest)
        {
            SEResponseListaNegra objResponse = new SEResponseListaNegra();

            try
            {

                //se valida que no esta en lista negra
                if (!ValidarListaNegra(_IdOperadora, objRequest.Msisdn))
                {

                    //lista del resultado al consumir el api de solicitud de baja mediante web api puente -> http://10.31.16.21/Desuscripciones/api/ProcesarBajaMasiva
                    List<SEResultadoProcesoSACA<SEResponseBajaSACA>> resultadoSACA = new List<SEResultadoProcesoSACA<SEResponseBajaSACA>>();

                    //Lista de suscripciones del lado del saca
                    List<SERequestBajaSACA> SuscripcionesSACA = new List<SERequestBajaSACA>();
                    SuscripcionesSACA = ObtenerSuscripcionesSACA(_IdOperadora, objRequest.Msisdn).ToList();

                    //si tiene suscripciones en el SACA se solicita la baja mediante consumo del api
                    if (SuscripcionesSACA.Count > 0)
                    {
                        ApiConsumerManager obj = new ApiConsumerManager();
                        string url = "http://10.31.16.21/Desuscripciones/api";
                        resultadoSACA = obj.EnviarSolicitud <SEResultadoProcesoSACA<SEResponseBajaSACA>>(url, "ProcesarBajaMasiva", SuscripcionesSACA, "POST", "application/json");
                    }

                    //si se realizó la baja del lado del saca se procede a ingresar a lista negra
                    if (resultadoSACA.Where(x => x.Exitoso.Equals(true)).ToList().Count() == SuscripcionesSACA.Count)
                    {
                        // si no existe se ingresa a lista negra y se da de baja de los servicios suscritos
                        if (IncluirEnListaNegra(_IdOperadora, objRequest.Msisdn, "Inclusion desde WSIntegratorService", objRequest.FechaRequest) > 0)
                        {
                            objResponse.CodeError = (byte)CODE_ERROR.SUCCESS;
                            objResponse.Message = "Sucess";
                        }
                        else
                        {
                            objResponse.CodeError = (byte)CODE_ERROR.ERROR_INESPERADO;
                            objResponse.Message = "Error Inesperado";
                        }
                    }
                    else
                    {
                        objResponse.CodeError = (byte)CODE_ERROR.ERROR_INESPERADO;
                        objResponse.Message = "Error Inesperado";
                    }

                }
                else
                {
                    objResponse.CodeError = (byte)CODE_ERROR.SUCCESS;
                    objResponse.Message = "Sucess";
                }


            }
            catch (Exception)
            {

                objResponse.CodeError = (byte)CODE_ERROR.ERROR_INTERNO;
                objResponse.Message = "Error Interno de aplicación";
            }

            objResponse.FechaResponse = DateTime.Now;

            return objResponse;
        }

        /*
         * Metodo para Excluir de lista negra a un cliente por numero celular
         * se debe considerar lo siguiente:
         * -Validar que el cliente exista en lista negra
         * -Sacar al cliente en lista negra, en caso que exista
         */
        public SEResponseListaNegra ProcesarExclusion(SERequestListaNegra objRequest)
        {
            SEResponseListaNegra objResponse = new SEResponseListaNegra();

            try
            {

                //se valida que el cliente este lista negra
                if (ValidarListaNegra(_IdOperadora, objRequest.Msisdn))
                {
                    // si existe se excluye de lista negra
                    if (ExcluirDeListaNegra(_IdOperadora, objRequest.Msisdn, "Exclusion desde WSIntegratorService", objRequest.FechaRequest) > 0)
                    {
                        objResponse.CodeError = (byte)CODE_ERROR.SUCCESS;
                        objResponse.Message = "Sucess";
                    }
                    else
                    {
                        objResponse.CodeError = (byte)CODE_ERROR.ERROR_INESPERADO;
                        objResponse.Message = "Error Inesperado";
                    }

                }
                else
                {
                    objResponse.CodeError = (byte)CODE_ERROR.SUCCESS;
                    objResponse.Message = "Sucess";
                }
            }
            catch (Exception)
            {

                objResponse.CodeError = (byte)CODE_ERROR.ERROR_INTERNO;
                objResponse.Message = "Error Interno de aplicación";
            }

            objResponse.FechaResponse = DateTime.Now;
            return objResponse;

        }


        public int RegistraLogSolicitudes(SERequestListaNegra ObjRequest, SEResponseListaNegra ObjResponse)
        {
            string sp = "SMS.spRegistrarResponseRequesSolicitudesBaja";
            int resultado = 0;

            try
            {
                Conexion.TRX();
                Conexion.GDatos.IniciarTransaccion();
                resultado = Conexion.GDatos.Ejecutar(sp, _IdOperadora, ObjRequest.RequestBlacklistDetailId, ObjRequest.Msisdn, ObjRequest.RequestType, ObjResponse.CodeError, ObjRequest.FechaRequest, ObjResponse.FechaResponse);
                Conexion.GDatos.TerminarTransaccion();
            }
            catch (Exception ex)
            {
                Conexion.GDatos.AbortarTransaccion();
                SNError.IngresaError(1, 1, "WSIntegratorService", sp, ex.Message);
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return resultado;
        }

        #endregion

    }
}
