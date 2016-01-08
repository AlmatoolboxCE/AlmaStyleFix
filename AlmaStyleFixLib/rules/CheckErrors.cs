//----------------------------------------------------------------------------------------------
// <copyright file="CheckErrors.cs" company="Almaviva TSF">
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
    /// Verifica la presenza di errori di compilazione.
    /// </summary>
    public class CheckErrors : StyleCopRules
    {
        /// <summary>
        /// Controlla gli errori di compilazione.
        /// </summary>
        /// <param name="workingLines">
        /// La struttura delle righe.
        /// </param>
        /// <returns>
        /// Ritorna un valore che indica se la compilazione ha terminato correttamente.
        /// </returns>
        internal int SA0102_ErrorInCompiling(ref List<SFWorkingLine> workingLines)
        {
            foreach (SFWorkingLine ws in workingLines)
            {
                if (IsLineViolated(ws, "SA0102"))
                {
                    return Convert.ToInt32(ws.LineNo);
                }
            }

            return -1;
        }
    }
}