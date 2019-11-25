using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entidades
{
    public class SERequestListaNegra
    {
        public int RequestBlacklistDetailId { get; set; }
        public string Msisdn { get; set; }
        public byte RequestType { get; set; }
        public DateTime FechaRequest { get; set; }

    }
}
