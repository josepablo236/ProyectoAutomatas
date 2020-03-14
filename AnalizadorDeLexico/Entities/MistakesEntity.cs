using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizadorDeLexico.Entities
{
    public class MistakesEntity
    {
        public int Line { get; set; }
        public string Mistake { get; set; }
    }
}