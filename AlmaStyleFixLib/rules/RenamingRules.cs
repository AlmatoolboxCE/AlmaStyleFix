//----------------------------------------------------------------------------------------------
// <copyright file="RenamingRules.cs" company="Almaviva TSF">
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

    /// <summary>
    /// Questa classe risolve i warning relativi al renaming.
    /// </summary>
    public class RenamingRules : StyleCopRules
    {
        /// <summary>
        /// Elenco dei nomi degli oggetti da rinominare.
        /// </summary>
        private static Dictionary<string, string> toChange;

        /// <summary>
        /// Inizializza una nuova istanza della classe RenamingRules.
        /// </summary>     
        public RenamingRules()
        {
            toChange = new Dictionary<string, string>();
        }

        /// <summary>
        /// Recupera o imposta il dizionario degli elementi da rinominare.
        /// </summary>
        public static Dictionary<string, string> ToChange
        {
            get { return RenamingRules.toChange; }
            set { RenamingRules.toChange = value; }
        }

        /// <summary>
        /// Cambia la prima lettera del nome di un metodo da minuscola a maiuscola o viceversa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        public void SA1300_MethodNamesBeginWithAnUpperCaseLetter(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1300"))
                {
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1300");
                    string[] arrayWord = violation.Description.Split(" ".ToCharArray());
                    var theItemName = arrayWord[arrayWord.Length - 1];
                    if (theItemName.EndsWith("."))
                    {
                        theItemName = theItemName.Substring(0, theItemName.Length - 1);
                    }

                    var upper = false;
                    for (int i = 0; i < arrayWord.Length; i++)
                    {
                        if (arrayWord[i].Contains("case"))
                        {
                            if (arrayWord[i].Contains("upper"))
                            {
                                upper = true;
                            }

                            break;
                        }
                    }

                    var initialChar = theItemName[0];
                    string initialString = String.Empty;
                    if (upper)
                    {
                        initialString = initialChar.ToString().ToUpper();
                    }
                    else
                    {
                        initialString = initialChar.ToString().ToLower();
                    }

                    if (!toChange.ContainsKey(theItemName))
                    {
                        toChange.Add(theItemName, initialString + theItemName.Substring(1));
                    }
                }
            }
        }

        /// <summary>
        /// Cambia la prima lettera del nome di una costante da minuscola a maiuscola o viceversa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        public void SA1303_ConstatsMustStartWithAnUpperCaseLetter(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1303"))
                {
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1303");
                    string[] arrayWord = violation.Description.Replace("-", String.Empty).Split(" ".ToCharArray());
                    var theItemName = arrayWord[arrayWord.Length - 1];
                    if (theItemName.EndsWith("."))
                    {
                        theItemName = theItemName.Substring(0, theItemName.Length - 1);
                    }

                    var upper = false;
                    for (int i = 0; i < arrayWord.Length; i++)
                    {
                        if (arrayWord[i].Contains("case"))
                        {
                            if (arrayWord[i].Contains("upper"))
                            {
                                upper = true;
                            }

                            break;
                        }
                    }

                    var initialChar = theItemName[0];
                    string initialString = String.Empty;
                    if (upper)
                    {
                        initialString = initialChar.ToString().ToUpper();
                    }
                    else
                    {
                        initialString = initialChar.ToString().ToLower();
                    }

                    if (!toChange.ContainsKey(theItemName))
                    {
                        toChange.Add(theItemName, initialString + theItemName.Substring(1));
                    }
                }
            }
        }

        /// <summary>
        /// Cambia la prima lettera del nome di una variabile da minuscola a maiuscola o viceversa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        public void SA1306_VariableNamesBeginWithAnLowCaseLetter(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1306"))
                {
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1306");
                    string[] arrayWord = violation.Description.Replace("-", " ").Split(" ".ToCharArray());
                    var theItemName = arrayWord[arrayWord.Length - 1];
                    if (theItemName.EndsWith("."))
                    {
                        theItemName = theItemName.Substring(0, theItemName.Length - 1);
                    }

                    var upper = false;
                    for (int i = 0; i < arrayWord.Length; i++)
                    {
                        if (arrayWord[i].Contains("case"))
                        {
                            if (arrayWord[i].Contains("upper"))
                            {
                                upper = true;
                            }

                            break;
                        }
                    }

                    var initialChar = theItemName[0];
                    string initialString = String.Empty;
                    if (upper)
                    {
                        initialString = initialChar.ToString().ToUpper();
                    }
                    else
                    {
                        initialString = initialChar.ToString().ToLower();
                    }

                    if (!toChange.ContainsKey(theItemName))
                    {
                        toChange.Add(theItemName, initialString + theItemName.Substring(1));
                    }
                }
            }
        }

        /// <summary>
        /// Cambia la prima lettera del nome di un metodo da minuscola a maiuscola o viceversa.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        public void SA1307_PublicAndInternalFieldsMustStartWithAnUpperCaseLetter(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine workingLine in workingLines)
            {
                if (IsLineViolated(workingLine, "SA1307"))
                {
                    SAObject violation = workingLine.Violations.Find(sao => sao.ErrorId == "SA1307");
                    string[] arrayWord = violation.Description.Replace("-", String.Empty).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var theItemName = arrayWord[arrayWord.Length - 1];
                    if (theItemName.EndsWith("."))
                    {
                        theItemName = theItemName.Substring(0, theItemName.Length - 1);
                    }

                    var upper = false;
                    for (int i = 0; i < arrayWord.Length; i++)
                    {
                        if (arrayWord[i].Contains("case"))
                        {
                            if (arrayWord[i].Contains("upper"))
                            {
                                upper = true;
                            }

                            break;
                        }
                    }

                    var initialChar = theItemName[0];
                    string initialString = String.Empty;
                    if (upper)
                    {
                        initialString = initialChar.ToString().ToUpper();
                    }
                    else
                    {
                        initialString = initialChar.ToString().ToLower();
                    }

                    if (!toChange.ContainsKey(theItemName))
                    {
                        toChange.Add(theItemName, initialString + theItemName.Substring(1));
                    }
                }
            }
        }
    }
}