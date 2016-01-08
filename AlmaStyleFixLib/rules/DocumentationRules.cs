//----------------------------------------------------------------------------------------------
// <copyright file="DocumentationRules.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Risolve le regole di creazione dei summary.
    /// </summary>
    public class DocumentationRules : StyleCopRules
    {
        /// <summary>
        /// Il nome dell'autore dei documenti da inserire nello header.
        /// </summary>
        private string author;

        /// <summary>
        /// Nome della compagnia da inserire nello header del copyRight.
        /// </summary>
        private string company;

        /// <summary>
        /// 
        /// </summary>
        public string Author
        {
            get { return this.author; }
            set { this.author = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Company
        {
            get { return this.company; }
            set { this.company = value; }
        }

        /// <summary>
        /// Controlla se il file contiene un header di copyRight.
        /// </summary>
        /// <param name="fileName">
        /// Il nome del file.
        /// </param>
        /// <param name="outputLines">
        /// Struttura delle righe.
        /// </param>
        public void SA1633_FileMustHaveHeader(string fileName, ref string outputLines)
        {
            if (!IsAllowed("SA1633"))
            {
                return;
            }

            if (!ForceAllow && (this.company == string.Empty))
            {
                return;
            }

            string copyrightMessage =
            @"//----------------------------------------------------------------------------------------------
            // <copyright file=""{0}"" company=""{1}"" author=""{2}"">
            // Copyright (c) {1}.  All rights reserved.
            // </copyright>
            //----------------------------------------------------------------------------------------------
            ";
            copyrightMessage = string.Format(copyrightMessage, fileName, this.company, this.author);

            // add the file header in the begining of the file
            if (outputLines.IndexOf("<copyright file=") < 0)
            {
                outputLines = outputLines.Insert(0, copyrightMessage);
            }
        }

        /// <summary>
        /// Controlla se metodi, proprieta' e campi hanno un commento, oppure lo inserisce automaticamente.
        /// </summary>
        /// <param name="workingLines">
        /// Struttura delle righe.
        /// </param>
        internal void SA1600_SA1601TheMethodOrClassMustHaveAnHeader(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1600") || this.IsLineViolated(workingLines[i], "SA1601"))
                {
                    var theLine = workingLines[i].Line;

                    // var parameters = new List<string>();
                    var addReturn = true;

                    // se funzione void oppure variabile
                    if (theLine.Contains(" void ") || theLine.IndexOf("(") < 0)
                    {
                        // non e' presente il tag returns
                        addReturn = false;
                    }

                    // metodo o propriet?
                    // no parametri, no ritorno
                    if (!theLine.Contains("(") || (theLine.Contains("=") && theLine.Contains("(") && theLine.IndexOf("=") < theLine.IndexOf("(")))
                    {
                        addReturn = false;
                    }

                    var parameters = this.GetParameters(theLine);
                    if (parameters == null)
                    {
                        parameters = new List<string>();
                    }

                    bool isCallBack = false;
                    if (parameters.Contains("sender") && parameters.Contains("e"))
                    {
                        isCallBack = true;
                    }

                    // devo mettere il summary prima degli attributi
                    int k = i - 1;

                    while (workingLines[k].Line.Replace(" ", string.Empty).Replace("\t", string.Empty).StartsWith("["))
                    {
                        k--;
                    }

                    k++;
                    var total = (addReturn ? 3 : 0) + (parameters.Count * 3 + 3);

                    // Aggiungo uno header
                    if (addReturn)
                    {
                        workingLines.Insert(k, new SFWorkingLine(k - total > 0 ? k - total : 0, "/// </returns>", false, new List<SAObject>()));
                        workingLines.Insert(k, new SFWorkingLine(k - total - 1 > 0 ? k - total - 1 : 0, "/// ", false, new List<SAObject>()));
                        workingLines.Insert(k, new SFWorkingLine(k - total - 2 > 0 ? k - total - 2 : 0, "/// <returns>", false, new List<SAObject>()));
                    }

                    for (int j = parameters.Count - 1; j >= 0; j--)
                    {
                        workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 > 0 ? k - 2 - j * 3 : 0, "/// </param>", false, new List<SAObject>()));
                        if (isCallBack)
                        {
                            if (parameters[j] == "sender")
                            {
                                workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 + 1 > 0 ? k - 2 - j * 3 - 1 : 0, "///  Chi esegue la chiamata.", false, new List<SAObject>()));
                            }
                            else if (parameters[j] == "e")
                            {
                                workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 + 1 > 0 ? k - 2 - j * 3 - 1 : 0, "///  Parametri della chiamata.", false, new List<SAObject>()));
                            }
                            else
                            {
                                workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 + 1 > 0 ? k - 2 - j * 3 - 1 : 0, "/// ", false, new List<SAObject>()));
                            }

                        }
                        else
                        {
                            workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 + 1 > 0 ? k - 2 - j * 3 - 1 : 0, "/// ", false, new List<SAObject>()));
                        }
                        workingLines.Insert(k, new SFWorkingLine(k - 2 - j * 3 + 2 > 0 ? k - 2 - j * 3 - 2 : 0, "/// <param name=\"" + parameters[j] + "\">", false, new List<SAObject>()));
                    }

                    workingLines.Insert(k, new SFWorkingLine(k - 1 > 0 ? k - 1 : 0, "/// </summary>", false, new List<SAObject>()));
                    workingLines.Insert(k, new SFWorkingLine(k - 1 - 1 > 0 ? k - 1 - 1 : 0, "/// ", false, new List<SAObject>()));
                    workingLines.Insert(k, new SFWorkingLine(k - 1 - 2 > 0 ? k - 1 - 2 : 0, "/// <summary>", false, new List<SAObject>()));
                    i += total;
                }
            }
        }

        /// <summary>
        /// Controlla se la documentazione del metodo è well-formed oppure la corregge.
        /// </summary>
        /// <param name="workingLines">
        /// Struttura delle righe.
        /// </param>
        internal void SA1611andSA1612and1618and1620_ParamTagsMustMatchTheParameterList(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1611") || this.IsLineViolated(workingLines[i], "SA1612") || this.IsLineViolated(workingLines[i], "SA1618") || this.IsLineViolated(workingLines[i], "SA1620"))
                {
                    var types = this.GetTemplateTypes(workingLines[i].Line);

                    var theLine = workingLines[i].Line;

                    // var parameters = new List<string>();
                    var addReturn = true;

                    // se funzione void oppure variabile
                    if (theLine.Contains(" void ") || theLine.IndexOf("(") < 0)
                    {
                        // non e' presente il tag returns
                        addReturn = false;
                    }

                    // metodo o propriet?
                    // no parametri, no ritorno
                    if (!theLine.Contains("(") || (theLine.Contains("=") && theLine.Contains("(") && theLine.IndexOf("=") < theLine.IndexOf("(")))
                    {
                        addReturn = false;
                    }

                    var parameters = this.GetParameters(theLine);

                    // devo mettere il summary prima degli attributi
                    int k = i - 1;
                    while (workingLines[k].Line.Replace(" ", string.Empty).Replace("\t", string.Empty).StartsWith("["))
                    {
                        k--;
                    }

                    // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    // ora rimuovo  tutte le vecchie righe e ce le rimetto dentro daccapo
                    var commentsList = new List<SFWorkingLine>();

                    // mi posiziono sulla prima riga del commento
                    while (workingLines[k].Line.Replace(" ", string.Empty).Replace("\t", string.Empty).StartsWith("///"))
                    {
                        k--;
                    }

                    k++;

                    // copio le righe
                    while (workingLines[k].Line.Replace(" ", string.Empty).Replace("\t", string.Empty).StartsWith("///"))
                    {
                        commentsList.Add(workingLines[k]);
                        workingLines.RemoveAt(k);
                        i--;        // aggiusto l'indice i
                    }

                    var myPars = new List<MyPars>();

                    // prima i tipi di template
                    if (types != null)
                    {
                        foreach (string s in types)
                        {
                            var appo = this.GetIndexComplexTag("typeparam", ref commentsList, "name", s);
                            if (appo != null)
                            {
                                myPars.Add(new MyPars() { Name = s, Start = appo[0], Stop = appo[1] });
                            }
                            else
                            {
                                commentsList.Add(new SFWorkingLine(k, "/// <typeparam name=\"" + s + "\">", false, new List<SAObject>()));
                                commentsList.Add(new SFWorkingLine(k, "/// ", false, new List<SAObject>()));
                                commentsList.Add(new SFWorkingLine(k, "/// </typeparam>", false, new List<SAObject>()));
                                appo = this.GetIndexComplexTag("typeparam", ref commentsList, "name", s);
                                myPars.Add(new MyPars() { Name = s, Start = appo[0], Stop = appo[1] });
                            }
                        }
                    }

                    // poi i dati
                    if (parameters != null)
                    {
                        foreach (string s in parameters)
                        {
                            var appo = this.GetIndexComplexTag("param", ref commentsList, "name", s);
                            if (appo != null)
                            {
                                myPars.Add(new MyPars() { Name = s, Start = appo[0], Stop = appo[1] });
                            }
                            else
                            {
                                commentsList.Add(new SFWorkingLine(k, "/// <param name=\"" + s + "\">", false, new List<SAObject>()));
                                commentsList.Add(new SFWorkingLine(k, "/// ", false, new List<SAObject>()));
                                commentsList.Add(new SFWorkingLine(k, "/// </param>", false, new List<SAObject>()));
                                appo = this.GetIndexComplexTag("param", ref commentsList, "name", s);
                                myPars.Add(new MyPars() { Name = s, Start = appo[0], Stop = appo[1] });
                            }
                        }
                    }

                    var summ = this.GetIndexSimpleTag("summary", ref commentsList);
                    var ret = this.GetIndexSimpleTag("returns", ref commentsList);

                    if (summ != null)
                    {
                        for (int n = summ[0]; n <= summ[1]; n++)
                        {
                            workingLines.Insert(i++, commentsList[n]);
                        }
                    }
                    else
                    {
                        workingLines.Insert(i, new SFWorkingLine(i - 1, "/// <summary>", false, new List<SAObject>()));
                        i++;
                        workingLines.Insert(i, new SFWorkingLine(i - 1, "/// ", false, new List<SAObject>()));
                        i++;
                        workingLines.Insert(i, new SFWorkingLine(i - 1, "/// </summary>", false, new List<SAObject>()));
                        i++;
                    }

                    foreach (MyPars par in myPars)
                    {
                        for (int n = par.Start; n <= par.Stop; n++)
                        {
                            workingLines.Insert(i++, commentsList[n]);
                        }
                    }

                    if (ret != null)
                    {
                        for (int n = ret[0]; n <= ret[1]; n++)
                        {
                            workingLines.Insert(i++, commentsList[n]);
                        }
                    }
                    else
                    {
                        if (addReturn)
                        {
                            workingLines.Insert(i, new SFWorkingLine(i - 1, "/// <returns>", false, new List<SAObject>()));
                            i++;
                            workingLines.Insert(i, new SFWorkingLine(i - 1, "/// ", false, new List<SAObject>()));
                            i++;
                            workingLines.Insert(i, new SFWorkingLine(i - 1, "/// </returns>", false, new List<SAObject>()));
                            i++;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Controlla se i commenti iniziano con la lettera maiuscola.
        /// </summary>
        /// <param name="workingLines">
        /// Struttura delle righe.
        /// </param>
        internal void SA1628_TheDocumentationTextWithinTheSummaryMustBegineWithCapitalLetter(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1628"))
                {
                    // la riga che viola lo stile ? quella della dichiarazione di funzione
                    // cerco i commenti all'indietro
                    for (int j = -1; i + j > 0; j--)
                    {
                        // cerco i commenti
                        var match = Regex.Match(workingLines[i + j].Line, "/// [a-zA-Z0-9 ]+");
                        if (match.Success)
                        {
                            // Cerco la prima lettera
                            var capital = workingLines[i + j].Line[match.Index + 4].ToString().ToUpper();
                            workingLines[i + j].Line = workingLines[i + j].Line.Remove(match.Index + 4, 1);
                            workingLines[i + j].Line = workingLines[i + j].Line.Insert(match.Index + 4, capital);
                        }
                        else if (workingLines[i + j].Line.Contains("<summary>"))
                        {
                            // fine del commento
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Controlla se i commenti finiscono con un punto.
        /// </summary>
        /// <param name="workingLines">
        /// Struttura delle righe.
        /// </param>
        internal void SA1629_TheDocumentationTextWithinTheSummaryMustEndWithPeriod(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1629"))
                {
                    for (int j = -1; i + j > 0; j--)
                    {
                        var match = Regex.Match(workingLines[i + j].Line, "/// [a-zA-Z0-9]+");
                        if (match.Success)
                        {
                            if (!workingLines[i + j].Line.EndsWith(">"))
                            {
                                if (!workingLines[i + j].Line.EndsWith("."))
                                {
                                    workingLines[i + j].Line = workingLines[i + j].Line + ".";
                                }
                            }
                            else
                            {
                                // var appo = workingLines[i + j].Line.Substring(0, workingLines[i + j].Line.LastIndexOf("<"));
                                // TODO : perfezionare tutti i casi
                            }
                        }
                        else if (workingLines[i + j].Line.Contains("<summary>"))
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ritorna gli indici di prima e ultima riga di un commento con tag con attributo.
        /// </summary>
        /// <param name="tag">
        /// Nome del tag.
        /// </param>
        /// <param name="lines">
        /// Struttura delle righe.
        /// </param>
        /// <param name="attributeName">
        /// Nome dell'attributo.
        /// </param>
        /// <param name="attributeValue">
        /// Valore dell'attributo.
        /// </param>
        /// <returns>
        /// Int[2] con [0]=prima riga e [1] = ultima riga.
        /// </returns>
        private int[] GetIndexComplexTag(string tag, ref List<SFWorkingLine> lines, string attributeName, string attributeValue)
        {
            string begin = "<" + tag;
            string end = "</" + tag + ">";
            string attr = attributeName + "=\"" + attributeValue + "\"";
            var start = -1;
            var stop = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (start < 0)
                {
                    if (lines[i].Line.Contains(begin))
                    {
                        if (lines[i].Line.Contains(attr))
                        {
                            start = i;
                        }
                    }
                }

                if (start >= 0 && stop < start)
                {
                    if (lines[i].Line.Contains(end))
                    {
                        stop = i;
                        break;
                    }
                }
            }

            if (start >= 0 && stop >= start)
            {
                return new int[2] { start, stop };
            }

            return null;
        }

        /// <summary>
        /// Ritorna gli indici di prima e ultima riga di un commento con tag semplice.
        /// </summary>
        /// <param name="tag">
        /// Nome del tag.
        /// </param>
        /// <param name="lines">
        /// Struttura delle righe.
        /// </param>
        /// <returns>
        /// Int[2] con [0]=prima riga e [1] = ultima riga.
        /// </returns>
        private int[] GetIndexSimpleTag(string tag, ref List<SFWorkingLine> lines)
        {
            string begin = "<" + tag + ">";
            string end = "</" + tag + ">";
            var start = -1;
            var stop = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (start < 0)
                {
                    if (lines[i].Line.Contains(begin))
                    {
                        start = i;
                    }
                }

                if (start >= 0 && stop < start)
                {
                    if (lines[i].Line.Contains(end))
                    {
                        stop = i;
                        break;
                    }
                }
            }

            if (start >= 0 && stop >= start)
            {
                return new int[2] { start, stop };
            }

            return null;
        }

        /// <summary>
        /// Cerca i nomi dei parametri in una dichiarazione di funzione.
        /// </summary>
        /// <param name="theline">
        /// La stringa che contiene la dichiarazione.
        /// </param>
        /// <returns>
        /// La lista dei nomi dei parametri.
        /// </returns>
        private List<string> GetParameters(string theline)
        {
            var res = new List<string>();
            var m = Regex.Match(theline, @"\(.*\)", RegexOptions.None);
            if (m.Success)
            {
                var appo = m.Value.Replace("(", string.Empty).Replace(")", string.Empty);
                appo = this.RemoveAngulars(appo);

                var elements = appo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Contains("="))
                    {
                        elements[i] = elements[i].Substring(0, elements[i].IndexOf("="));
                    }

                    var par = elements[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (par.Length > 1)
                    {
                        res.Add(par[par.Length - 1]);
                    }
                }

                return res;
            }

            return null;
        }

        /// <summary>
        /// Utility che cerca una sottostringa.
        /// </summary>
        /// <param name="theMainString">
        /// Stringa di partenza.
        /// </param>
        /// <param name="initial">
        /// Marcatore di inizio.
        /// </param>
        /// <param name="final">
        /// Marcatore di fine.
        /// </param>
        /// <returns>
        /// Sottostringa compresa nei marcatori.
        /// </returns>
        private string GetSubString(string theMainString, string initial, string final)
        {
            var begin = theMainString.IndexOf(initial);
            if (begin < 0)
            {
                return null;
            }

            begin += initial.Length;
            var end = theMainString.IndexOf(final, begin);
            if (end < begin)
            {
                return null;
            }

            return theMainString.Substring(begin, end - begin);
        }

        /// <summary>
        /// Cerca in nomi dei tipi nelle funzion Template.
        /// </summary>
        /// <param name="theLine">
        /// La dichiarazione della funzione.
        /// </param>
        /// <returns>
        /// La lista dei tipi template.
        /// </returns>
        private List<string> GetTemplateTypes(string theLine)
        {
            var m = Regex.Match(theLine, @"[A-Za-z0-9]+[ ]*<[A-Za-z0-9 ,]+>[ ]*\(");
            if (m.Success)
            {
                var appo = m.Value;

                m = Regex.Match(appo, "<[A-Za-z0-9 ,]+>");
                if (m.Success)
                {
                    appo = m.Value.Replace(" ", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty);
                    var types = appo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    return types.ToList<string>();
                }
            }

            return null;
        }

        /// <summary>
        /// Rimuove da una sctinga il contenuto delle parentesi angolari.
        /// </summary>
        /// <param name="original">
        /// La stringa originale.
        /// </param>
        /// <returns>
        /// La nuova stringa senza il contenuto delle parentesi angolari.
        /// </returns>
        private string RemoveAngulars(string original)
        {
            var sb = new StringBuilder();
            var parentesi = 0;
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] == '<')
                {
                    parentesi++;
                }
                else if (original[i] == '>')
                {
                    parentesi--;
                }
                else if (parentesi == 0)
                {
                    sb.Append(original[i]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Struttura di appoggio. serve per contenere la lista dei parametri.
        /// </summary>
        private struct MyPars
        {
            /// <summary>
            /// Nome del parametro.
            /// </summary>
            public string Name;

            /// <summary>
            /// Prima riga del commento.
            /// </summary>
            public int Start;

            /// <summary>
            /// Ultima riga del commento.
            /// </summary>
            public int Stop;
        }
    }
}