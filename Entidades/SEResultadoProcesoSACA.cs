using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entidades
{
   public class SEResultadoProcesoSACA<T>
    {
        public bool Exitoso { get; set; }
        public bool Ejecutado { get; set; }
        public bool ErrorEjecucionLocal { get; set; }
        public bool ErrorEnvio { get; set; }
        public string ErrorMessage { get; set; }
        public bool ErrorLog { get; set; }
        public string ErrorLogMessage { get; set; }
        public bool ErrorGeneral { get; set; }
        public string ErrorGeneralMessage { get; set; }
        public T Data { get; set; }
    }
   
}
