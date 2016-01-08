//----------------------------------------------------------------------------------------------
// <copyright file="StyleCopRules.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib.Drivers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Gestisce le regole di fix di styleCop.
    /// </summary>
    public class StyleCopRules
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe StyleCopRules.
        /// </summary>        
        public StyleCopRules()
        {
            this.ForceAllow = false;
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se forzare le ammissioni degli errori.
        /// </summary>
        public bool ForceAllow
        {
            get;
            set;
        }

        /// <summary>
        /// Specifica se la linea viola questa particolare regola.
        /// </summary>
        /// <param name="workingLine">
        /// La righa di codice.
        /// </param>
        /// <param name="errorId">
        /// L'ID dell'errore.
        /// </param>
        /// <returns>
        /// Il vaolre che indica se la linea ? violata.
        /// </returns>
        internal bool IsLineViolated(SFWorkingLine workingLine, string errorId)
        {
            if (!this.IsAllowed(errorId))
            {
                return false;
            }

            List<SAObject> violations = workingLine.Violations.FindAll(sao => sao.ErrorId == errorId);
            bool returnValue = (violations != null) && (violations.Count >= 1);
            if (returnValue)
            {
                foreach (SAObject violation in violations)
                {
                    violation.IsFixed = true;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Verifica se l'ID dell'errore ? presente nella lista di quelli da correggere.
        /// </summary>
        /// <param name="errorId">
        /// L'ID dell'errore.
        /// </param>
        /// <returns>
        /// True se l'errore deve esere corretto.
        /// </returns>
        protected bool IsAllowed(string errorId)
        {
            return true;
        }
    }
}