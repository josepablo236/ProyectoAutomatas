using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizadorDeLexico.ExpressionsTree
{
    public class Node
    {
        public string Value { get; set; }
        public string Number { get; set; }
        public bool IsNull { get; set; }
        public List<Node> First { get; set; }
        public List<Node> Last { get; set; }
        public List<string> Follow { get; set; }
        public List<string> State { get; set; }
        public bool EsHoja { get; set; }
        public Node RightChild { get; set; }
        public Node LeftChild { get; set; }
        public Node UniqueChild { get; set; }
    }
}