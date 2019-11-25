using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CapaDatos
{
    public class SqlServer : GDatos
    {
        //Contructor de la clase SqlServer
        public SqlServer()
        {
            _Servidor = "";
            _BaseDatos = "";
            _Usuario = "";
            _Password = "";
        }

        //Contructor sobrecargado que recibe la conexión para luego establecerla en el metodo set de la variable _CadenaConexion(public override sealed string CadenaConexion)
        public SqlServer(string cadenaConexion)
        {
            CadenaConexion = cadenaConexion;
        }

        //Contructor sobrecargado que recibe la conexión para luego establecerla en el metodo get de la variable _CadenaConexion (public override sealed string CadenaConexion)
        public SqlServer(string Servidor, string BaseDatos, string Usuario, string Password)
        {
            _Servidor = Servidor;
            _BaseDatos = BaseDatos;
            _Usuario = Usuario;
            _Password = Password;

            Autenticar();
        }

        //Variable para establecer y obtener el valor de la variable _CadenaConexion
        public override sealed string CadenaConexion
        {
            get
            {
                //si el length es cero es porque se usa el contructor (public SqlServer(string Servidor, string BaseDatos, string Usuario, string Password))
                if (_CadenaConexion.Length == 0)
                {
                    //Estableciendo la cadena conexión
                    if (_Servidor.Length != 0 && _BaseDatos.Length != 0 && _Usuario.Length != 0 && _Password.Length != 0)
                    {
                        var sCadena = new System.Text.StringBuilder("");
                        sCadena.Append("data source=<SERVIDOR>;");
                        sCadena.Append("initial catalog=<BASE>;");
                        sCadena.Append("user id=<USER>;");
                        sCadena.Append("password=<PASSWORD>;");
                        sCadena.Append("persist security info=True;");
                        sCadena.Replace("<SERVIDOR>", _Servidor);
                        sCadena.Replace("<BASE>", _BaseDatos);
                        sCadena.Replace("<USER>", _Usuario);
                        sCadena.Replace("<PASSWORD>", _Password);

                        return sCadena.ToString();
                    }
                    throw new Exception("No se puede establecer la cadena de conexión en la clase DatosSQLServer");
                }
                else
                {   //si la cadena ya se habia establecido solo se retorna
                    return _CadenaConexion;
                }

            }
            set
            {
                //se establece el valor si usamos el contructor sobrecargado que para por parametro la cadena de conexion (public SqlServer(string cadenaConexion))
                _CadenaConexion = value;
            }
        }


        protected override IDbConnection CrearConexion(string CadenaConexion)
        {
            return new System.Data.SqlClient.SqlConnection(CadenaConexion);
        }

        public override int EjecutarSql(string ConsultaSql)
        {
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);

            return cmd.ExecuteNonQuery();
        }

        public override int Ejecutar(string ProcedimientoAlmacenado)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            return cmd.ExecuteNonQuery();
        }

        public override int Ejecutar(string ProcedimientoAlmacenado, params object[] arg)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            return cmd.ExecuteNonQuery();
        }

        public override object ObtenerValorScalarSql(string ConsultaSql)
        {
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);

            return cmd.ExecuteScalar();
        }

        public override object ObtenerValorScalar(string ProcedimientoAlmacenado)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            return cmd.ExecuteScalar();
        }

        public override object ObtenerValorScalar(string ProcedimientoAlmacenado, params object[] arg)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            return cmd.ExecuteScalar();

        }

        public override DataSet ObtenerDataSetSql(string ConsultaSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataSet ObtenerDataSet(string ProcedimientoAlmacenado)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataSet ObtenerDataSet(string ProcedimientoAlmacenado, params object[] arg)
        {

            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds;
        }

        public override DataTable ObtenerDataTableSql(string ConsultaSql)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand(ConsultaSql, (SqlConnection)Conexion, (SqlTransaction)Transaccion);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }

        public override DataTable ObtenerDataTable(string ProcedimientoAlmacenado)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }

        public override DataTable ObtenerDataTable(string ProcedimientoAlmacenado, params object[] arg)
        {
            DataSet ds = new DataSet();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = ProcedimientoAlmacenado;
            cmd.Connection = (SqlConnection)Conexion;
            cmd.Transaction = (SqlTransaction)Transaccion;
            SqlCommandBuilder.DeriveParameters(cmd);

            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
            {
                cmd.Parameters[i + 1].Value = arg[i];
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            return ds.Tables[0].Copy();
        }


    }
}
