using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text.RegularExpressions;
using AnalizadorDeLexico.Models;
using AnalizadorDeLexico.Entities;
using AnalizadorDeLexico.ExpressionsTree;
using System.Web.Routing;

namespace AnalizadorDeLexico.Controllers
{
    public class ReadTextController : Controller
    {
        public string FilePath = "";
        public static string Message;
        public static ExpressionTree tree = new ExpressionTree();
        // GET: ReadText
        public string regularExpression = "";
        public List<Node> nList = new List<Node>();
        public ActionResult Read(string filename)
        {
            regularExpression = "";
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            FilePath = Server.MapPath("~/Archivo");
            var numberLine = 0;
            var specialCharsSets = new List<string>() { "=", "'", ".", "+", "(", ")" };
            var specialCharsTokens = new List<string>() { "*", "|", "+", "?", "'", "\"", "(", ")" };
            var specialCharsActions = new List<string>() { "{", "RESERVADAS", "()", "'" };
            var dictionaryMistakes = new Dictionary<int, string>();
            var rangeList = new List<string>();
            var tokensDictionary = new Dictionary<int, string>();
            var setsDictionary = new Dictionary<string, List<string>>();
            var lettersM = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
            var lettersm = "abcdefghijklmnñopqrstuvwxyz";
            var space = "_";
            var digits = "0123456789";
            var nextChar = "=";
            var setName = "";
            var rangeL = new List<string>();
            var rangel = new List<string>();
            var rangeD = new List<string>();
            var range_ = new List<string>();
            var rangeCharset = new List<string>();
            var listCharacters = new List<string>();
            var character = "";
            var noToken = 0;
            var error = false;
            var cadena = "";
            var token = "";
            var tokenValue = "";
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var line = reader.ReadLine();
                    numberLine++;
                    line = Regex.Replace(line, @"\s+", "");
                    //SETS
                    if (line.Substring(0, 4).ToUpper() == "SETS")
                    {
                        line = reader.ReadLine();
                        numberLine++;
                        while (line.Substring(0, line.Length).ToUpper() != "TOKENS")
                        {
                            line = Regex.Replace(line, @"\s+", "");
                            //ValidateSets(line, specialCharsSets);
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (error == true)
                                {
                                    nextChar = "=";
                                }
                                else
                                {
                                    switch (nextChar)
                                    {
                                        case "=":
                                            if (line.Contains("="))
                                            {
                                                if (line[i].ToString() == "=")
                                                {
                                                    if (setName != string.Empty)
                                                    {
                                                        setsDictionary.Add(setName, rangeList);
                                                        setName = string.Empty;
                                                        var newRangeList = new List<string>();
                                                        rangeList = newRangeList;
                                                    }
                                                    setName = line.Substring(0, i);
                                                    nextChar = "'";
                                                }
                                            }
                                            else if (!specialCharsSets.Contains(line[i].ToString()))
                                            {
                                                setName += line[i];
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "'":
                                            if (line[i].ToString() == "'")
                                            {
                                                nextChar = "caracter";
                                            }
                                            else if (line[i].ToString() == "C")
                                            {
                                                nextChar = "H";
                                            }
                                            else
                                            {
                                                if (line != "")
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "caracter":
                                            if ((i + 1) >= line.Length)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (!specialCharsSets.Contains(line[i].ToString()) && specialCharsSets.Contains(line[i + 1].ToString()))
                                                {
                                                    //if(LM Lm E D o C) Contains character
                                                    if (lettersM.Contains(line[i].ToString()))
                                                    {
                                                        if (rangeL.Count == 0)
                                                        {
                                                            rangeL.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangeL.Add(line[i].ToString());
                                                            if (rangeL[0].CompareTo(rangeL[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = lettersM.IndexOf(lettersM.FirstOrDefault(x => x.ToString() == rangeL[0]));
                                                                var indexLast = lettersM.IndexOf(lettersM.FirstOrDefault(x => x.ToString() == rangeL[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(lettersM[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (lettersm.Contains(line[i].ToString()))
                                                    {
                                                        if (rangel.Count == 0)
                                                        {
                                                            rangel.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangel.Add(line[i].ToString());
                                                            if (rangel[0].CompareTo(rangel[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = lettersm.IndexOf(lettersm.FirstOrDefault(x => x.ToString() == rangel[0]));
                                                                var indexLast = lettersm.IndexOf(lettersm.FirstOrDefault(x => x.ToString() == rangel[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(lettersm[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (space.Contains(line[i].ToString()))
                                                    {
                                                        rangeList.Add(line[i].ToString());
                                                    }
                                                    if (digits.Contains(line[i].ToString()))
                                                    {
                                                        if (rangeD.Count == 0)
                                                        {
                                                            rangeD.Add(line[i].ToString());
                                                        }
                                                        else
                                                        {
                                                            rangeD.Add(line[i].ToString());
                                                            if (rangeD[0].CompareTo(rangeD[1]) != -1)
                                                            {
                                                                dictionaryMistakes.Add(numberLine, line);
                                                                error = true;
                                                            }
                                                            else
                                                            {
                                                                var indexFirts = digits.IndexOf(digits.FirstOrDefault(x => x.ToString() == rangeD[0]));
                                                                var indexLast = digits.IndexOf(digits.FirstOrDefault(x => x.ToString() == rangeD[1]));
                                                                for (int k = indexFirts; k <= indexLast; k++)
                                                                {
                                                                    rangeList.Add(digits[k].ToString());
                                                                }
                                                            }
                                                        }
                                                    }
                                                    nextChar = "'";
                                                }
                                                else if (line[i].ToString() == ".")
                                                {
                                                    nextChar = ".";
                                                }
                                                else if (line[i].ToString() == "+")
                                                {
                                                    nextChar = "'";
                                                }
                                                else if (specialCharsSets.Contains(line[i].ToString()) && specialCharsSets.Contains(line[i + 1].ToString()))
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                                else
                                                {
                                                    nextChar = "=";
                                                }
                                            }
                                            break;
                                        case ".":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "'";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "C":
                                            if (line[i].ToString().ToUpper() != "C")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "H";
                                            break;
                                        case "H":
                                            if (line[i].ToString().ToUpper() != "H")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "R";
                                            break;
                                        case "R":
                                            if (line[i].ToString().ToUpper() != "R")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            nextChar = "(";
                                            break;
                                        case "(":
                                            if (line[i].ToString() == "(")
                                            {
                                                nextChar = "charset";
                                            }
                                            else
                                            {
                                                if (line != "")
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "charset":
                                            if (!specialCharsSets.Contains(line[i].ToString()))
                                            {
                                                character += line[i].ToString();
                                            }
                                            else if (line[i].ToString() == ")")
                                            {
                                                rangeCharset.Add(character);
                                                character = "";
                                                nextChar = "punto";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "punto":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "punto2";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "punto2":
                                            if (line[i].ToString() == ".")
                                            {
                                                nextChar = "C";
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            line = reader.ReadLine();
                            numberLine++;
                            error = false;
                        }
                        setsDictionary.Add(setName, rangeList);
                    }
                    //TOKENS
                    if (line.Substring(0, 6).ToUpper() == "TOKENS")
                    {
                        line = reader.ReadLine();
                        numberLine++;
                        nextChar = "=";
                        while (line.Substring(0, line.Length).ToUpper() != "ACTIONS")
                        {
                            line = line.ToUpper();
                            line = Regex.Replace(line, @"\s+", "");
                            for (int i = 0; i < line.Length; i++)
                            {
                                if (error == true)
                                {
                                    nextChar = "=";
                                }
                                else
                                {
                                    switch (nextChar)
                                    {
                                        case "=":
                                            if (line.Contains("="))
                                            {
                                                if (line[i].ToString() == "=")
                                                {
                                                    if (tokenValue != string.Empty)
                                                    {
                                                        tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                        if (!tokensDictionary.Keys.Contains(noToken))
                                                        {
                                                            tokensDictionary.Add(noToken, tokenValue);
                                                            tokenValue = string.Empty;
                                                        }
                                                        else
                                                        {
                                                            dictionaryMistakes.Add(numberLine, line);
                                                            error = true;
                                                        }
                                                    }
                                                    if (line.Substring(0, i - 1) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 1));
                                                    }
                                                    else if (line.Substring(0, i - 2) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 2));
                                                    }
                                                    else if (token.Substring(0, 5) == "TOKEN")
                                                    {
                                                        noToken = Convert.ToInt32(line.Substring(5, 1));
                                                    }
                                                    else
                                                    {
                                                        dictionaryMistakes.Add(numberLine, line);
                                                        error = true;
                                                    }
                                                    nextChar = "set";
                                                }
                                            }
                                            else if (!specialCharsTokens.Contains(line[i].ToString()))
                                            {
                                                token += line[i];
                                            }
                                            else
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            break;
                                        case "set":
                                            if ((i + 1) >= line.Length)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                if (!specialCharsTokens.Contains(line[i].ToString()))
                                                {
                                                    cadena += line[i].ToString();
                                                    if (cadena == "TOKEN")
                                                    {
                                                        nextChar = "=";
                                                        cadena = "";
                                                        break;
                                                    }
                                                    if (setsDictionary.ContainsKey(cadena))
                                                    {
                                                        listCharacters.Add(cadena);
                                                        cadena = "";
                                                    }
                                                    if (specialCharsTokens.Contains(line[i + 1].ToString()))
                                                    {
                                                        foreach (var item in listCharacters)
                                                        {
                                                            tokenValue = tokenValue + item + ".";
                                                        }
                                                        listCharacters = new List<string>();
                                                        if (line[i + 1].ToString() == "'")
                                                        {
                                                            nextChar = "set";
                                                        }
                                                        else
                                                        {
                                                            nextChar = "simbol";
                                                        }
                                                    }
                                                }
                                                else if (line[i].ToString() == "'")
                                                {
                                                    nextChar = "character";
                                                }
                                                else if (line[i].ToString() == "(")
                                                {
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == "|")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    error = true;
                                                }
                                            }
                                            break;
                                        case "simbol":
                                            if (cadena != string.Empty)
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            else
                                            {
                                                if (line[i].ToString() == "|")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == "(")
                                                {
                                                    tokenValue += line[i].ToString();
                                                    nextChar = "set";
                                                }
                                                else if (line[i].ToString() == ")")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString() + ".";
                                                    nextChar = "pow";
                                                }
                                                else if (line[i].ToString() == "*" || line[i].ToString() == "+" || line[i].ToString() == "?")
                                                {
                                                    tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                    tokenValue += line[i].ToString() + ".";
                                                    nextChar = "set";
                                                }
                                            }
                                            break;
                                        case "character":
                                            if (line[i].ToString() == "'")
                                            {
                                                if (cadena != string.Empty)
                                                {
                                                    listCharacters.Add(cadena);
                                                    cadena = "";
                                                }
                                                else
                                                {
                                                    listCharacters.Add(line[i].ToString());
                                                }
                                                nextChar = "'";
                                            }
                                            else if (line[i].ToString() == "\"")
                                            {
                                                listCharacters.Add(line[i].ToString());
                                                nextChar = "'";
                                            }
                                            else
                                            {
                                                cadena += line[i].ToString();
                                                if (setsDictionary.ContainsKey(cadena))
                                                {
                                                    dictionaryMistakes.Add(numberLine, line);
                                                    cadena = "";
                                                }
                                                if (specialCharsTokens.Contains(line[i + 1].ToString()))
                                                {
                                                    listCharacters.Add(cadena);
                                                    cadena = "";
                                                    nextChar = "'";
                                                }
                                            }
                                            break;
                                        case "'":
                                            if (line[i].ToString() != "'")
                                            {
                                                dictionaryMistakes.Add(numberLine, line);
                                                error = true;
                                            }
                                            else
                                            {
                                                foreach (var item in listCharacters)
                                                {
                                                    tokenValue = tokenValue + "'" + item + "'" + ".";
                                                }
                                                listCharacters = new List<string>();
                                                nextChar = "set";
                                            }
                                            break;
                                        case "pow":
                                            if (line[i].ToString() == "*" || line[i].ToString() == "+" || line[i].ToString() == "?")
                                            {
                                                tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                                                tokenValue += line[i].ToString() + ".";
                                            }
                                            nextChar = "set";
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                            line = reader.ReadLine();
                            numberLine++;
                        }
                        tokenValue = tokenValue.Substring(0, tokenValue.Length - 1);
                        if (error == false)
                        {
                            if (!tokensDictionary.Keys.Contains(noToken))
                            {
                                tokensDictionary.Add(noToken, tokenValue);
                                tokenValue = string.Empty;
                            }
                            else
                            {
                                dictionaryMistakes.Add(numberLine, line);
                                error = true;
                            }
                            foreach (var item in tokensDictionary)
                            {
                                regularExpression += "(" + item.Value + ")" + "|";
                            }
                            regularExpression = regularExpression.Substring(0, regularExpression.Length - 1) + "#";
                        }
                    }
                    else
                    {
                        dictionaryMistakes.Add(numberLine, line);
                    }
                }
            }
            if(dictionaryMistakes.Count > 0)
            {
                MistakesListViewModel modelList = new MistakesListViewModel();
                modelList.ListMistakes = new List<MistakesEntity>();
                foreach (var item in dictionaryMistakes)
                {
                    MistakesEntity model = new MistakesEntity();
                    model.Line = item.Key;
                    model.Mistake = item.Value;
                    modelList.ListMistakes.Add(model);
                }
                TempData["Mistakes"] = modelList;
                return RedirectToAction("Mistakes");
            }
            else
            {
                var postList = new List<string>();
                var nodesList = new List<Node>();
                postList = tree.Calculator(regularExpression, setsDictionary);
                nodesList = tree.Tree(postList);
                nList = nodesList;
                ArbolViewModel arbol = new ArbolViewModel();
                arbol.ListNodes = new List<NodeEntity>();
                foreach (var item in nodesList)
                {
                    NodeEntity model = new NodeEntity();
                    model.EsHoja = item.EsHoja;
                    model.IsNull = item.IsNull;
                    model.Value = item.Value;
                    if (item.Number != null)
                    {
                        model.Number = item.Number;
                    }
                    foreach (var number in item.First)
                    {
                        model.First = model.First + number.Number + ",";
                    }
                    foreach (var number1 in item.Last)
                    {
                        model.Last = model.Last + number1.Number + ",";
                    }
                    if (item.Follow != null)
                    {
                        foreach (var number2 in item.Follow)
                        {
                            model.Follow = model.Follow + number2 + ",";
                        }
                    }
                    arbol.ListNodes.Add(model);
                }
                TempData["Tree"] = arbol;
                return RedirectToAction("TableOfTree");
            }
        }
        public ActionResult Mistakes()
        {
            var modelList = TempData["Mistakes"] as MistakesListViewModel;
            return View(modelList);
        }
        public ActionResult TableOfTree()
        {
            var arbol = TempData["Tree"] as ArbolViewModel;
            return View(arbol);
        }
        public ActionResult Automata()
        {
            var statesTable = new Dictionary<string, List<string>>();
            var nodesList = new List<Node>();
            nodesList = tree.GetList();
            var listNoTerminals = new List<Node>();
            listNoTerminals = tree.GetNoTerminalList();
            var firstRaiz = new List<string>();
            foreach (var item in nodesList[nodesList.Count - 1].First)
            {
                firstRaiz.Add(item.Number);
            }
            statesTable.Add("Estado1", firstRaiz);
            var conjunto = firstRaiz;
            var position = 0;
            var listConjuntos = new List<List<string>>();
            listConjuntos.Add(conjunto);
            var Conjuntos = CreateTransitions(listNoTerminals, nodesList, listConjuntos, position);
            return View();
        }

        public List<List<string>>  CreateTransitions(List<Node> listNoTerminals, List<Node> nodesList, List<List<string>> listConjuntos, int pos)
        {
            var cont = 1;
            var differentState = false;
            var listNoT = new List<string>();
            foreach (var item in listNoTerminals)
            {
                listNoT.Add(item.Number);
            }
            foreach (var item in listConjuntos[pos])
            {
                if(listNoT.Contains(item))
                {
                    var position = listNoTerminals.IndexOf(listNoTerminals.FirstOrDefault(x => x.Number == item));
                    var valor = listNoTerminals[position].Value;
                    listNoTerminals[position].State = new List<string>();
                    foreach (var node in nodesList)
                    {
                        if (node.Value == valor)
                        {
                            foreach (var i in node.Follow)
                            {
                                if (!listNoTerminals[position].State.Contains(i))
                                {
                                    listNoTerminals[position].State.Add(i);
                                }
                            }
                        }
                    }
                }
            }
            foreach (var item in listNoTerminals)
            {
                if (!listConjuntos.Contains(item.State))
                {
                    listConjuntos.Add(item.State);
                    CreateTransitions(listNoTerminals, nodesList, listConjuntos, pos++);
                }
            }
            return listConjuntos;
        }

        private List<string> FilesUploaded()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames1 = dir.GetFiles("*.txt");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file1 in fileNames1)
            {
                filesupld.Add(file1.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }
    }
}