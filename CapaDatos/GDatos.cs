using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CapaDatos
{
    public abstract class GDatos
    {

        #region "Declaración de Variables"
        //Servidor de base de datos
        protected string _Servidor { get; set; }
        //Nombre de base de datos
        protected string _BaseDatos { get; set; }
        //Usuario que se usará para la autenticacion al motor de base de datos
        protected string _Usuario { get; set; }
        //Contraseña con la que accedera el usuario a la base de datos
        protected string _Password { get; set; }
        //Se utilizara para guardar la Cadena de conexión completa de la conexion a la BD, solo podrá se accedida 
        //a las clases que heredan de la clase GDatos
        protected string _CadenaConexion = "";
        //Conexion que usara para acceder a la base de datos, solo puede ser accedida de la misma clase
        private IDbConnection _Conexion;
        #endregion

        #region "Establecer y Obtener"
        //Funcion Abstracta que servira para dar acceso de modificar la variable _CadenaConexion de la Clase GDatos,
        //la funcion get y set se establecerá en la clase SqlServer y se sella con la palabra reservada sealed, esto
        //para que una vez definida no se pueda heredar en las clases devibadas, esto para asegurar que no se modifique la conexion en la instancia en uso
        public abstract string CadenaConexion
        {
            get;
            set;
        }

        //Funcion que establece la cadena de conexion (_Conexion) para luego retornarla
        protected IDbConnection Conexion
        {
            get
            {
                //si la conexión no existe la crea
                if (_Conexion == null)
                    _Conexion = CrearConexion(CadenaConexion);

                //si al crear la conexion su estado es Closed, la abre
                if (_Conexion.State != ConnectionState.Open)
                    _Conexion.Open();
                
                //se retorna la conexion con estado Open
                return _Conexion;

            }
        }

        #endregion

        #region "Acciones"

        //Funcion que crea y retorna una nueva instancia a partir de la candena de conexion que recibe como parametro
        protected abstract IDbConnection CrearConexion(string CadenaConexion);

        //Inicio de funciones de lectura a la BD
        public abstract int EjecutarSql(string ConsultaSql);
        public abstract int Ejecutar(string ProcedimientoAlmacenado);
        public abstract int Ejecutar(string ProcedimientoAlmacenado, params object[] arg);
        public abstract object ObtenerValorScalarSql(string ConsultaSql);
        public abstract object ObtenerValorScalar(string ProcedimientoAlmacenado);
        public abstract object ObtenerValorScalar(string ProcedimientoAlmacenado, params object[] arg);
        public abstract DataSet ObtenerDataSetSql(string ConsultaSql);
        public abstract DataSet ObtenerDataSet(string ProcedimientoAlmacenado);
        public abstract DataSet ObtenerDataSet(string ProcedimientoAlmacenado, params object[] arg);
        public abstract DataTable ObtenerDataTableSql(string ConsultaSql);
        public abstract DataTable ObtenerDataTable(string ProcedimientoAlmacenado);
        public abstract DataTable ObtenerDataTable(string ProcedimientoAlmacenado, params object[] arg);

        //Fin de funciones de lectura a la BD

        #endregion

        #region "Atenticacion y Desconexion"

        //Funcion que se utiliza para establecer la conexion por primera vez si se usa el contructor sobrecargado (public SqlServer(string Servidor, string BaseDatos, string Usuario, string Password))
        protected void Autenticar()
        {
            if (Conexion.State != ConnectionState.Open)
                Conexion.Open();
        }// end Autenticar


        // funcion que permite cerrar conexion
        public void CerrarConexion()
        {
            if (Conexion.State != ConnectionState.Closed)
                _Conexion.Close();
        }

        #endregion

        #region "Transacciones"

        protected IDbTransaction Transaccion;
        protected bool EnTransaccion;

        //Comienza una Transacción en la base en uso. 
        public void IniciarTransaccion()
        {
            try
            {
                Transaccion = Conexion.BeginTransaction();
                EnTransaccion = true;
            }// end try
            finally
            { EnTransaccion = false; }
        }// end IniciarTransaccion


        //Confirma la transacción activa. 
        public void TerminarTransaccion()
        {
            try
            { Transaccion.Commit(); }
            finally
            {
                Transaccion = null;
                EnTransaccion = false;
            }// end finally
        }// end TerminarTransaccion


        //Cancela la transacción activa.
        public void AbortarTransaccion()
        {
            try
            { Transaccion.Rollback(); }
            finally
            {
                Transaccion = null;
                EnTransaccion = false;
            }// end finally
        }// end AbortarTransaccion

        #endregion

    }
}
