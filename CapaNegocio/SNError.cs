using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapaDatos;

namespace CapaNegocio
{
    public class SNError
    {
        public static int IngresaError(Int16 IdPais,  int IdOperadora , string Aplicacion, string Procedimiento, string Mensaje)
        {
            int resultado = 0;
            string sp = "[dbo].[spInsertError]";

            try
            {
                Conexion.TRX();

               resultado = Conexion.GDatos.Ejecutar(sp, IdOperadora, Aplicacion, Procedimiento, Mensaje);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Conexion.GDatos.CerrarConexion();
            }

            return resultado;
        }
    }
}
