//----------------------------------------------------------------------------------------------
// <copyright file="ReadabilityRules.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Classe che esegue il Fix delle regole di leggibilita'.
    /// </summary>
    public class ReadabilityRules : StyleCopRules
    {
        /// <summary>
        /// Aggiunge "this." alle chiamate a metodi locali.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1101_PrefixLocalCallsWithThis(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (this.IsLineViolated(workingLine, "SA1101"))
                {
                    // Find the violation
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1101");

                    string[] arrayWord = violation.Description.Split(" ".ToCharArray());

                    // The fourth letter happens to be the filename
                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           string.Format(@"(\s|\(|\))({0})(\W)", arrayWord[3]),
                                           "$1this.$2$3",
                                           RegexOptions.None);

                    // Sometimes the same error is shown twice and the this. is repeated... so we need to take one out
                    // return workingLine.Line.Replace("this.this.", "this.");
                }
            }
        }

        /// <summary>
        /// Se i parametri non sono sulla stessa riga vengono disposti su rughe differenti
        /// </summary>
        /// <param name="workingLines">
        /// la struttura delle righe
        /// </param>
        internal void SA1115_TheParameterMustBeginOnLineAfterPreviusParameter(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1115"))
                {
                    var parameters = workingLines[i].Line.Split(new char[] { ',' });
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        while (parameters[j].StartsWith(" "))
                        {
                            parameters[j] = parameters[j].Substring(1);
                        }
                    }

                    workingLines[i].Line = parameters[0];
                    if (parameters.Length > 1)
                    {
                        workingLines[i].Line += ',';
                    }

                    for (int j = 1; j < parameters.Length; j++)
                    {
                        if (string.IsNullOrEmpty(parameters[j]))
                        {
                            continue;
                        }

                        workingLines.Insert(++i, new SFWorkingLine(i, parameters[j], false, new List<SAObject>()));
                        if (j < parameters.Length - 1)
                        {
                            workingLines[i].Line += ',';
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sopsta il primo parametro di un costruttore i cui parametri sono dichiarati su rughe differenti nella riga successiva
        /// </summary>
        /// <param name="workingLines">
        /// la srtuttura delle righe
        /// </param>
        internal void Sa1116_FirstParameterMustBeginInLineBeheart(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1116"))
                {
                    bool closedBrace = false;
                    for (int j = workingLines[i].Line.Length - 1; j > 0; j--)
                    {
                        if (workingLines[i].Line[j] == ')')
                        {
                            closedBrace = true;
                            continue;
                        }

                        if (workingLines[i].Line[j] == '(')
                        {
                            if (closedBrace)
                            {
                                closedBrace = false;
                                continue;
                            }
                            else
                            {
                                var originaString = workingLines[i].Line;
                                workingLines[i].Line = originaString.Substring(0, j + 1);
                                var nuova = new SFWorkingLine(i, originaString.Substring(j + 1, originaString.Length - j - 1), false, workingLines[i].Violations);
                                workingLines.Insert(++i, nuova);
                                j = 0;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sposta i parametri di un costruttore che non sono sulla stessa riga ciascuno su una riga diversa
        /// </summary>
        /// <param name="workingLines">
        /// la struttura delle righe
        /// </param>
        internal void Sa1117_ParametersMustBeOnTheSameLineOrSeparatedLines(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (this.IsLineViolated(workingLines[i], "SA1117"))
                {
                    int start = i;
                    int count = this.CountParantesis(workingLines[i].Line);
                    int stop = start + 1;
                    while (count > 0)
                    {
                        count += this.CountParantesis(workingLines[stop].Line);
                        stop++;
                    }

                    for (int j = i + 1; j < stop; j++)
                    {
                        workingLines[i].Line += workingLines[i + 1].Line;
                        workingLines.RemoveAt(i + 1);
                    }

                    workingLines[i].Violations = new List<SAObject>();

                    // var parameters = workingLines[i].Line.Split(new char[] { ',' });
                    // for (int j = 0; j < parameters.Length; j++)
                    // {
                    //    while (parameters[j].StartsWith(" "))
                    //    {
                    //        parameters[j] = parameters[j].Substring(1);
                    //    }
                    // }

                    // if (parameters.Length > 1)
                    // {
                    //    workingLines[i].Line = parameters[0] + ',';
                    // }

                    // for (int j = 1; j < parameters.Length; j++)
                    // {
                    //    if (string.IsNullOrEmpty(parameters[j]))
                    //    {
                    //        continue;
                    //    }

                    // workingLines.Insert(++i, new SFWorkingLine(i, parameters[j], false, new List<SAObject>()));
                    //    if (j < parameters.Length - 1)
                    //    {
                    //        workingLines[i].Line += ',';
                    //    }
                    // }
                }
            }
        }

        /// <summary>
        /// Elimina i commenti vuoti.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1120_TheCommentIsEmptyAddTextToTheCommentOrRemoveIt(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (this.IsLineViolated(workingLine, "SA1120"))
                {
                    var ind = workingLine.Line.IndexOf("//");
                    if (ind < 0)
                    {
                        continue;
                    }

                    workingLine.Line = workingLine.Line.Substring(ind + 2);
                }
            }
        }

        /// <summary>
        /// Usa gli alias di tipo built-in.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1121_UseBuiltInTypeAlias(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (this.IsLineViolated(workingLine, "SA1121"))
                {
                    SAObject saob = workingLine.Violations.Find(sao => sao.ErrorId == "SA1121");

                    if (saob.Description.IndexOf("'array'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Array", "array").Replace("Array", "array");
                    }

                    if (saob.Description.IndexOf("'bool'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Boolean", "bool").Replace("Boolean", "bool");
                    }

                    if (saob.Description.IndexOf("'byte'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Byte", "byte").Replace("Byte", "byte");
                    }

                    if (saob.Description.IndexOf("'char'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Char", "char").Replace("Char", "char");
                    }

                    if (saob.Description.IndexOf("'decimal'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Decimal", "decimal").Replace("Decimal", "decimal");
                    }

                    if (saob.Description.IndexOf("'double'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Double", "double").Replace("Double", "double");
                    }

                    if (saob.Description.IndexOf("'short'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Int16", "short").Replace("Int16", "short");
                    }

                    if (saob.Description.IndexOf("'int'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Int32", "int").Replace("Int32", "int");
                    }

                    if (saob.Description.IndexOf("'long'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Int64", "long").Replace("Int64", "long");
                    }

                    if (saob.Description.IndexOf("'object'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Object", "object").Replace("Object", "object");
                    }

                    if (saob.Description.IndexOf("'sbyte'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Sbyte", "sbyte").Replace("Sbyte", "sbyte");
                    }

                    if (saob.Description.IndexOf("'single'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Single", "single").Replace("Single", "single");
                    }

                    if (saob.Description.IndexOf("'string'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.String", "string").Replace("String", "string");
                    }

                    if (saob.Description.IndexOf("'ushort'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Ushort", "ushort").Replace("Ushort", "ushort");
                    }

                    if (saob.Description.IndexOf("'uint'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Uint", "uint").Replace("Uint", "uint");
                    }

                    if (saob.Description.IndexOf("'ulong'") > -1)
                    {
                        workingLine.Line = workingLine.Line.Replace("System.Ulong", "ulong").Replace("Ulong", "ulong");
                    }
                }
            }
        }

        /// <summary>
        /// Sostituisce "" con String.Empty.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1122_UseStringEmpty(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (this.IsLineViolated(workingLine, "SA1122"))
                {
                    workingLine.Line = workingLine.Line.Replace("\"\"", "String.Empty");
                }
            }
        }

        /// <summary>
        /// Questa regola è stata introdotta nella versione 4.7 di stylecop e sostituisce la vecchia regola del this
        /// </summary>
        /// <param name="workingLines">
        /// la collezione di righe di codice da controllare
        /// </param>
        internal void SA1126_PrefixLocalCallsWithThis(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (this.IsLineViolated(workingLine, "SA1126"))
                {
                    // Find the violation
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1126");

                    string[] arrayWord = violation.Description.Split(" ".ToCharArray());

                    // The fourth letter happens to be the filename
                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           string.Format(@"(\s|\(|\)|\[|\]|!)({0})(\W)", arrayWord[3]),
                                           "$1this.$2$3",
                                           RegexOptions.None);

                    // Sometimes the same error is shown twice and the this. is repeated... so we need to take one out
                    // return workingLine.Line.Replace("this.this.", "this.");
                }
            }
        }

        /// <summary>
        /// Serve per rilevare l'apertura/chiusura delle parentesi tonde
        /// </summary>
        /// <param name="theLine">
        /// la linea di codice
        /// </param>
        /// <returns>
        /// maggiore 0 se ci sono  pi? parentesi aperte che chiuse o minore di 0 in caso contrario
        /// </returns>
        private int CountParantesis(string theLine)
        {
            int count = 0;
            var ca = theLine.ToCharArray();
            for (int i = 0; i < ca.Length; i++)
            {
                if (ca[i] == '(')
                {
                    count++;
                }
                else if (ca[i] == ')')
                {
                    count--;
                }
            }

            return count;
        }
    }
}