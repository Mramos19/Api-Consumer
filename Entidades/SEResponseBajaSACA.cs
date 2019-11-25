using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Entidades
{
    [XmlType("response_data")]
    public class SEResponseBajaSACA
    {
        public string result_code { get; set; }
        public string result_description { get; set; }
    }
}
