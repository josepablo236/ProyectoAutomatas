using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizadorDeLexico.Entities
{
    public class NodeEntity
    {
        public string Value { get; set; }
        public string Number { get; set; }
        public bool IsNull { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Follow { get; set; }
        public bool EsHoja { get; set; }
    }
}