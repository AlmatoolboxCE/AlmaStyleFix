//----------------------------------------------------------------------------------------------
// <copyright file="AlmaCustomRules.cs" company="Almaviva TSF">
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
    /// Risolve i warning sulle regole custom.
    /// </summary>
    public class AlmaCustomRules : StyleCopRules
    {
        /// <summary>
        /// Autocommenta i costruttori.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void CR0001(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "CR0001"))
                {
                    SAObject violation = workingLines[i].Violations.Find(sao => sao.ErrorId == "CR0001");
                    string[] text = violation.Description.Split(':');

                    // cerco all'indietro
                    for (int j = -1; i + j > 0; j--)
                    {
                        if (workingLines[i + j].Line.Contains("<summary>"))
                        {
                            j++;
                            var match = Regex.Match(workingLines[i + j].Line, "/// ");
                            if (match.Success)
                            {
                                var ind = match.Index + match.Length;
                                if (workingLines[i + j].Line.Substring(ind).Length > 2)
                                {
                                    workingLines[i + j].Line = workingLines[i + j].Line.Insert(workingLines[i + j].Line.IndexOf("/// ") + "/// ".Length, text[1].Substring(1, text[1].Length - 2) + " ");
                                }
                                else
                                {
                                    workingLines[i + j].Line = workingLines[i + j].Line.Insert(workingLines[i + j].Line.IndexOf("/// ") + "/// ".Length, text[1].Substring(1, text[1].Length - 2));
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Autocommenta le proprieta' e i costruttori.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void CR0003(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "CR0003"))
                {
                    SAObject violation = workingLines[i].Violations.Find(sao => sao.ErrorId == "CR0003");
                    string[] text = violation.Description.Split(':');

                    // cerco all'indietro
                    for (int j = -1; i + j > 0; j--)
                    {
                        if (workingLines[i + j].Line.Contains("<summary>"))
                        {
                            j++;
                            var match = Regex.Match(workingLines[i + j].Line, "/// ");
                            if (match.Success)
                            {
                                var ind = match.Index + match.Length;
                                if (workingLines[i + j].Line.Substring(ind).Length > 2)
                                {
                                    workingLines[i + j].Line = workingLines[i + j].Line.Insert(workingLines[i + j].Line.IndexOf("/// ") + "/// ".Length, text[1].Substring(1, text[1].Length - 2) + " ");
                                }
                                else
                                {
                                    workingLines[i + j].Line = workingLines[i + j].Line.Insert(workingLines[i + j].Line.IndexOf("/// ") + "/// ".Length, text[1].Substring(1, text[1].Length - 2));
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}