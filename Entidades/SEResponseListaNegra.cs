using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapaNegocio
{
   public class SEResponseListaNegra
    {
        public byte CodeError { get; set; }
        public string Message { get; set; }
        public DateTime FechaResponse { get; set; }

    }
}
