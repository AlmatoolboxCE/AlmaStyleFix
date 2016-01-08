//----------------------------------------------------------------------------------------------
// <copyright file="SpacingRules.cs" company="Almaviva TSF">
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
    /// Risolve i warning relativi alle regole di spaziatura.
    /// </summary>
    public class SpacingRules : StyleCopRules
    {
        /// <summary>
        /// Aggiusta gli spazi sulle keyword.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1000_KeywordsMustBeSpacedCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1000"))
                {
                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           @"(new|switch|foreach|for|catch|fixed|from|group|if|in|into|join|let|lock|orderby|return|select|stackalloc|throw|using|where|while|yield)(\S)",
                                           @"$1 $2");
                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           @"(checked|default|sizeof|typeof|unchecked)\s+(\S)",
                                           @"$1$2");

                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           @"new\s\[",
                                           @"new[");
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi intorno alle virgole.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1001_CommasMustBeSpaceCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1001"))
                {
                    workingLine.Line = Regex.Replace(workingLine.Line, @"(\S)\s+,\s*", @"$1, ");
                    workingLine.Line = Regex.Replace(workingLine.Line, @",(\S)", @", $1");
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi intorno ai punto e virgola.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1002_SemicolonsMustBeSpaceCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1002"))
                {
                    workingLine.Line = Regex.Replace(workingLine.Line, @"(\S)\s+;\s*", @"$1; ");
                    workingLine.Line = Regex.Replace(workingLine.Line, @";(\S)", @"; $1");
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi intorno ai simboli.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1003_SymbolsMustBeSpaceCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1003"))
                {
                    // operator must be surrounded by white space on both sides
                    workingLine.Line = Regex.Replace(
                                           workingLine.Line,
                                           @"(\w)(=|<|>|\+|\*|-|%|<<|>>|<=|>=|==|!=|&|&&|\||\|\||\?:|\+=|-=|\*=|/=|%=|\|=|^=|<<=|>>=|\?\?)\s*",
                                           @"$1 $2 ");

                    // unary operator should not be followed by a space but must be preceded by a space
                    // TODO
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi dei commenti.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1005_SingleLineCommentsMustBeginWithSingleSpace(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1005"))
                {
                    workingLine.Line = Regex.Replace(workingLine.Line, @"//\s*(\S)", @"// $1", RegexOptions.None);
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi intorno alle parentesi chiuse.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1009_ClosingParenthesisMustBeSpacedCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1009"))
                {
                    // Case 1: The closing parenthesis should not be preceded by a whitespace and the
                    // closing parenthesis is not by itself on the line
                    workingLine.Line = Regex.Replace(workingLine.Line, @"(\S)\s+\)", @"$1)", RegexOptions.None);
                    ////workingLine.Line.Replace(" )", ")");

                    // Case 2: The closing parenthesis should not be followed by a whitespace if there is a ; ( ) [ ] ; ,
                    workingLine.Line = Regex.Replace(workingLine.Line, @"\)\s+(\(|\)|\[|\]|;|,)", @")$1", RegexOptions.None);

                    // Case 3: The closing parenthesis must not be followed by a whitespace after a cast
                    workingLine.Line = Regex.Replace(workingLine.Line, @"(\(\w+\))\s+(\S)", @"$1$2", RegexOptions.None);
                }
            }
        }

        /// <summary>
        /// Aggiusta gli spazi intorno alle parentesi quadre.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1010_OpeningSquareBracketsMustBeSpacedCorrectly(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1010"))
                {
                    // preceded by whitespace
                    workingLine.Line = Regex.Replace(workingLine.Line, @"(\S)\s+\[", @"$1[");

                    // followed by whitespace
                    workingLine.Line = Regex.Replace(workingLine.Line, @"\[\s+(\S)", @"[$1");
                }
            }
        }

        /// <summary>
        /// Elimina il carattere di tabulazione.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        internal void SA1027_TabsMustNotBeUsed(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1027"))
                {
                    workingLine.Line = workingLine.Line.Replace("\t", "    ");
                }
            }
        }
    }
}