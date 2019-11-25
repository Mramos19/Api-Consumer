using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CapaDatos
{
    public class Conexion
    {
        public static GDatos GDatos;

        public static void IniciarSesion(string Servidor, string BaseDatos, string Usuario, string Password)
        {
            GDatos = new SqlServer(Servidor, BaseDatos, Usuario, Password);
        }

        public static void SMSNI()
        {
            string conexionSMS = ConfigurationManager.AppSettings["SMSNI"].ToString();

            GDatos = new SqlServer(conexionSMS);
        }

        public static void SMS()
        {
            string conexionSMS = ConfigurationManager.AppSettings["SMS"].ToString();

            GDatos = new SqlServer(conexionSMS);
        }

        public static void TRX()
        {
            GDatos = new SqlServer(ConfigurationManager.AppSettings["TRX"].ToString());
        }

    }
}
