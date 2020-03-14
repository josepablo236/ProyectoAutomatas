using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnalizadorDeLexico.ExpressionsTree
{
    public class ExpressionTree
    {
        List<string> specialChars = new List<string>() { ".", "+", "*", "?", "|", "(", ")" };
        List<string> operators = new List<string>() { ".", "+", "*", "?", "|" };
        List<string> precedP = new List<string>() { "+", "*", "?" };
        List<string> precedS = new List<string>() { "." };
        List<string> precedT = new List<string>() { "|" };
        bool mistake = false;
        List<Node> nodesList = new List<Node>();

        public List<string> Calculator(string expression, Dictionary<string, List<string>> setsDictionary)
        {
            List<string> listOutput = new List<string>();
            Stack<string> stackSimbols = new Stack<string>();
            bool mayoroigual = true;
            bool end = false;
            string cadena = "";
            while (end == false && mistake == false)
            {
                for (int i = 0; i < expression.Length; i++)
                {
                    var E = expression[i].ToString();
                    if (!specialChars.Contains(E))
                    {
                        if (E == "'" && expression[i+2].ToString() == "'")
                        {
                            listOutput.Add("'" + expression[i + 1].ToString() + "'");
                            i = i + 2;
                        }
                        else if(E=="#")
                        {
                            end = true;
                        }
                        else
                        {
                            cadena += E;
                            if (setsDictionary.ContainsKey(cadena))
                            {
                                listOutput.Add(cadena);
                                cadena = "";
                            }
                        }
                    }
                    if(E == "(")
                    {
                        stackSimbols.Push(E);
                    }
                    if (E == ")")
                    {
                        while(stackSimbols.Count > 0 && stackSimbols.Peek() != "(")
                        {
                            var cima = stackSimbols.Pop();
                            listOutput.Add(cima);
                        }
                        if(stackSimbols.Peek() == "(")
                        {
                            stackSimbols.Pop();
                        }
                        else
                        {
                            mistake = true;
                        }
                    }
                    if(operators.Contains(E))
                    {
                        while(stackSimbols.Count > 0 && mayoroigual==true)
                        {
                            if(precedP.Contains(E) && precedP.Contains(stackSimbols.Peek().ToString())
                                || precedS.Contains(E) && precedS.Contains(stackSimbols.Peek().ToString())
                                || precedT.Contains(E) && precedT.Contains(stackSimbols.Peek().ToString())

                                || precedS.Contains(E) && precedP.Contains(stackSimbols.Peek().ToString())
                                || precedT.Contains(E) && precedP.Contains(stackSimbols.Peek().ToString())
                                || precedT.Contains(E) && precedS.Contains(stackSimbols.Peek().ToString()))
                            {
                                mayoroigual = true;
                                var cima = stackSimbols.Pop();
                                listOutput.Add(cima);
                            }
                            else
                            {
                                mayoroigual = false;
                            }
                        }
                        stackSimbols.Push(E);
                        mayoroigual = true;
                    }
                }
            }
            while(stackSimbols.Count > 0)
            {
                var cima = stackSimbols.Pop();
                listOutput.Add(cima);
            }
            listOutput.Add("#");
            listOutput.Add(".");
            stackSimbols.Clear();
            return listOutput;
        }
        public List<Node> Tree(List<string> listExpression)
        {
            List<Node> Arbol = new List<Node>();
            Stack<Node> stackNodes = new Stack<Node>();
            var cont = 0;
            var totalNodes = listExpression.Count;
            while(totalNodes > 0 && mistake == false)
            {
                foreach (var item in listExpression)
                {
                    if (!specialChars.Contains(item))
                    {
                        cont++;
                        Node hoja = new Node();
                        hoja.EsHoja = true;
                        hoja.IsNull = false;
                        hoja.Number = cont.ToString();
                        hoja.Value = item;
                        hoja.First = new List<Node>();
                        hoja.First.Add(hoja);
                        hoja.Last = new List<Node>();
                        hoja.Last.Add(hoja);
                        stackNodes.Push(hoja);
                        nodesList.Add(hoja);
                    }
                    if(item == "(")
                    {
                        mistake = true;
                    }
                    if (operators.Contains(item))
                    {
                        if(stackNodes.Count < 2)
                        {
                            mistake = true;
                        }
                        else
                        {
                            //Validate its * + ? because they have one child
                            if (precedP.Contains(item))
                            {
                                var A = stackNodes.Pop();
                                Node raiz = new Node();
                                raiz.UniqueChild = A;
                                raiz.First = raiz.UniqueChild.First;
                                raiz.Last = raiz.UniqueChild.Last;
                                raiz.EsHoja = false;
                                raiz.Value = item;
                                stackNodes.Push(raiz);
                                if(item == "*" || item == "?")
                                {
                                    raiz.IsNull = true;
                                }
                                else
                                {
                                    raiz.IsNull = false;
                                }
                                //Follows
                                if (raiz.Value == "*" || raiz.Value == "+")
                                {
                                    foreach (var number in raiz.UniqueChild.Last)
                                    {
                                        var listFollow = new List<string>();
                                        foreach (var i in raiz.UniqueChild.First)
                                        {
                                            listFollow.Add(i.Number);
                                        }
                                        if (number.Follow == null)
                                        {
                                            number.Follow = listFollow;
                                        }
                                        else
                                        {
                                            foreach (var follow in listFollow)
                                            {
                                                number.Follow.Add(follow);
                                            }
                                        }
                                    }
                                }
                                nodesList.Add(raiz);
                            }
                            else
                            {
                                var A2 = stackNodes.Pop();
                                var A1 = stackNodes.Pop();
                                Node raiz = new Node();
                                raiz.RightChild = A2;
                                raiz.LeftChild = A1;
                                raiz.EsHoja = false;
                                raiz.Value = item;
                                stackNodes.Push(raiz);
                                if(raiz.Value == "|")
                                {
                                    if(raiz.LeftChild.IsNull == true || raiz.RightChild.IsNull == true)
                                    {
                                        raiz.IsNull = true;
                                    }
                                    else
                                    {
                                        raiz.IsNull = false;
                                    }
                                    var listTemporalF = new List<Node>();
                                    foreach (var i in raiz.LeftChild.First)
                                    {
                                        listTemporalF.Add(i);
                                    }
                                    foreach (var i in raiz.RightChild.First)
                                    {
                                        listTemporalF.Add(i);
                                    }
                                    raiz.First = listTemporalF;
                                    var listTemporalL = new List<Node>();
                                    foreach (var i in raiz.LeftChild.Last)
                                    {
                                        listTemporalL.Add(i);
                                    }
                                    foreach (var i in raiz.RightChild.Last)
                                    {
                                        listTemporalL.Add(i);
                                    }
                                    raiz.Last = listTemporalL;
                                }
                                if (raiz.Value == ".")
                                {
                                    if(raiz.LeftChild.IsNull == true)
                                    {
                                        var listTemporalF = new List<Node>();
                                        foreach (var i in raiz.LeftChild.First)
                                        {
                                            listTemporalF.Add(i);
                                        }
                                        foreach (var i in raiz.RightChild.First)
                                        {
                                            listTemporalF.Add(i);
                                        }
                                        raiz.First = listTemporalF;
                                    }
                                    else
                                    {
                                        raiz.First = raiz.LeftChild.First;
                                    }
                                    if (raiz.RightChild.IsNull == true)
                                    {
                                        var listTemporalL = new List<Node>();
                                        foreach (var i in raiz.LeftChild.Last)
                                        {
                                            listTemporalL.Add(i);
                                        }
                                        foreach (var i in raiz.RightChild.Last)
                                        {
                                            listTemporalL.Add(i);
                                        }
                                        raiz.Last = listTemporalL;
                                    }
                                    else
                                    {
                                        raiz.Last = raiz.RightChild.Last;
                                    }
                                    //Follows
                                    foreach (var number in raiz.LeftChild.Last)
                                    {
                                        var listFollow = new List<string>();
                                        foreach (var i in raiz.RightChild.First)
                                        {
                                            listFollow.Add(i.Number);
                                        }
                                        if(number.Follow == null)
                                        {
                                            number.Follow = listFollow;
                                        }
                                        else
                                        {
                                            foreach (var follow in listFollow)
                                            {
                                                number.Follow.Add(follow);
                                            }
                                        }
                                    }
                                }
                                nodesList.Add(raiz);
                            }
                        }
                    }
                    totalNodes--;
                }
            }
            if(stackNodes.Count == 0 || stackNodes.Count > 1)
            {
                mistake = true;
            }
            else
            {
                var E = stackNodes.Pop();
            }
            return nodesList;
        }
        public List<Node> GetList()
        {
            return nodesList;
        }
        public List<Node> GetNoTerminalList()
        {
            var listNoTerminals = new List<Node>();
            var listTemporal = new List<string>();
            foreach (var item in nodesList)
            {
                if(item.EsHoja == true && item.Value != "#")
                {
                    if (!listTemporal.Contains(item.Value))
                    {
                        listTemporal.Add(item.Value);
                        listNoTerminals.Add(item);
                    }
                }
            }
            return listNoTerminals;
        }
    }
}