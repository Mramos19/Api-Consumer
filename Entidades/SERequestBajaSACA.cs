using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entidades
{
    public class SERequestBajaSACA
    {
        public string Celular { get; set; }
        public int IdSuscripcion { get; set; }
        public int IdSuscripcionSACA { get; set; }
        private int _IdUsuario;

        public int IdUsuario
        {
            get { return _IdUsuario; }
            set { _IdUsuario = 247; }
        }

        private Int16 _IdComando;

        public Int16 IdComando
        {
            get { return _IdComando; }
            set { _IdComando = 72; }
        }

    }
}
