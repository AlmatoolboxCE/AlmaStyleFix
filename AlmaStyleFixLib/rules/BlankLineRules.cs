//----------------------------------------------------------------------------------------------
// <copyright file="BlankLineRules.cs" company="Almaviva TSF">
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
    /// Risolve i warning relativi alle righe vuote.
    /// </summary>
    public class BlankLineRules : StyleCopRules
    {
        internal void SA1500_IfTheStatementSpansMultipleLinesTheClosingBracketMustBePlacedOnItsOwnLine(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1500"))
                {
                    var lines = workingLines[i].Line.Split(new char[] { '}' }, StringSplitOptions.None);
                    if (lines == null || lines.Length == 0)
                        break;
                    workingLines[i].Line = lines[0];
                   
                    for (int j = 1; j < lines.Length; j++)
                    {
                        var s = new SFWorkingLine(i, "}", false, new List<SAObject>());
                        workingLines.Insert(i +(j-1)*2+1, s);
                        s = new SFWorkingLine(i, lines[j], false, new List<SAObject>());
                        workingLines.Insert(i+(j-1)*2+2, s);                       
                    }
                    i += (lines.Length-1) * 2;

                }
            }
        }

        /// <summary>
        /// Wrappa gli if con una sola istruzione aggiungendo le graffe.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1503_TheBodyOfTheStatementIfMustBeWrappedInOpeningAndClosingBrackets(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1503"))
                {
                    var m = Regex.Match(workingLines[i].Line, @" *if *(.*) +", RegexOptions.None);
                    if (m.Success)
                    {
                        var start = m.Index;
                        while (workingLines[i].Line[start] == ' ')
                        {
                            start++;
                        }

                        if (workingLines[i].Line[start] != 'i')
                        {
                            continue;   // errore
                        }
                        else
                        {
                            start++;
                        }

                        if (workingLines[i].Line[start] != 'f')
                        {
                            continue;   // errore
                        }
                        else
                        {
                            start++;
                        }

                        while (workingLines[i].Line[start] == ' ')
                        {
                            start++;
                        }

                        if (workingLines[i].Line[start] != '(')
                        {
                            continue;   // errore
                        }
                        else
                        {
                            start++;
                        }

                        int opened = 1;
                        while (opened > 0)
                        {
                            if (workingLines[i].Line[start] == '(')
                            {
                                opened++;
                            }
                            else if (workingLines[i].Line[start] == ')')
                            {
                                opened--;
                            }

                            start++;
                        }

                        workingLines.Insert(i + 1, new SFWorkingLine(i + 1, workingLines[i].Line.Substring(start), false, new List<SAObject>()));
                        workingLines.Insert(i + 2, new SFWorkingLine(i + 2, "}", false, new List<SAObject>()));
                        workingLines[i].Line = workingLines[i].Line.Substring(0, start) + " {";
                        i += 2;
                    }
                    else
                    {
                        workingLines[i].Line = "{" + workingLines[i].Line;
                        workingLines.Insert(i + 1, new SFWorkingLine(i + 1, "}", false, new List<SAObject>()));
                        i++;
                    }

                    // workingLine.Line = Regex.Replace(
                    //                       workingLine.Line,
                    //                       string.Format(@"(\s|\()({0})(\W)", arrayWord[3]),
                    //                       "$1this.$2$3",
                    //                       RegexOptions.None);
                    Console.WriteLine("SA1503 intercettata " + i.ToString());
                }
            }
        }

        /// <summary>
        /// Rimuove la riga bianca dopo la graffa aperta.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1505_AnOpeningCurlyBracketMustNotBeFolloewdByBlankLine(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1505"))
                {
                    // elimino la riga sucessiva
                    workingLines[i + 1].IsRemove = true;
                }
            }
        }

        /// <summary>
        /// Rimuove la riga bianca prima della graffa chiusa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1508_AClosingCurlyBracketMustNotBePrecededByBlankLine(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1508"))
                {
                    // elimino la riga precedente
                    workingLines[i - 1].IsRemove = true;
                    Console.WriteLine("SA1508 rimossa riga " + i.ToString());
                }
            }
        }

        /// <summary>
        /// Rimuove la riga bianca dopo il commento.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1512_ASingleLineCommentMustNotBeFollowedByBlankLine(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1512"))
                {
                    workingLines[++i].IsRemove = true;
                }
            }
        }

        /// <summary>
        /// Aggiunge una riga bianca dopo la graffa chiusa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1513_StatementsOrElementesWrappedInCurlyBracetsMustBeFollowedByBlankLine(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1513"))
                {
                    workingLines.Insert(i + 1, new SFWorkingLine(i, "\n", false, new List<SAObject>()));
                    i++;
                }
            }
        }

        /// <summary>
        /// Aggiunge una riga bianca prima del commento.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1514andSA1515_ASingleLineCommentMustBePrecededByBlankLineOrAnotherSingleLineComment(ref List<SFWorkingLine> workingLines)
        {
            for (int i = 0; i < workingLines.Count; i++)
            {
                if (IsLineViolated(workingLines[i], "SA1514") || IsLineViolated(workingLines[i], "SA1515"))
                {
                    workingLines.Insert(i, new SFWorkingLine(i, "\n", false, new List<SAObject>()));
                    i++;
                }
            }
        }
    }
}