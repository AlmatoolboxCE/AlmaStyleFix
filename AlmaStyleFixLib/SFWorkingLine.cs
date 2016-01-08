//----------------------------------------------------------------------------------------------
// <copyright file="SFWorkingLine.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Stylefix working object.
    /// </summary>
    public class SFWorkingLine
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe SFWorkingLine.
        /// </summary>
        /// <param name="lineNo">
        /// Il numero della riga.
        /// </param>
        /// <param name="line">
        /// Il contenuto della riga.
        /// </param>
        /// <param name="isRemove">
        /// Specifica se la riga deve essere rimossa.
        /// </param>
        /// <param name="violations">
        /// Lista delle violazioni rilevate da style cop.
        /// </param>
        public SFWorkingLine(decimal lineNo, string line, bool isRemove, List<SAObject> violations)
        {
            this.LineNo = lineNo;
            this.Line = line;
            this.IsRemove = isRemove;
            this.Violations = violations;
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se la stringa deve essere rimossa.
        /// </summary>
        public bool IsRemove
        {
            get;
            set;
        }

        /// <summary>
        /// Recupera o imposta il contenuto della stringa.
        /// </summary>
        public string Line
        {
            get;
            set;
        }

        /// <summary>
        /// Recupera o imposta il numero della stringa.
        /// </summary>
        public decimal LineNo
        {
            get;
            set;
        }

        /// <summary>
        /// Recupera il numero di violazioni corrette per questa riga.
        /// </summary>
        public int TotalFixedViolationsCount
        {
            get
            {
                int count = 0;
                foreach (SAObject saObject in this.Violations)
                {
                    if (saObject.IsFixed)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Recupera il numero di violazioni rilevate per questa riga.
        /// </summary>
        public int TotalViolationsCount
        {
            get
            {
                return this.Violations.Count;
            }
        }

        /// <summary>
        /// Recupera o imposta le violazioni rilevate da stylecop.
        /// </summary>
        public List<SAObject> Violations
        {
            get;
            set;
        }

        /// <summary>
        /// Aggiunge una violazione alla riga.
        /// </summary>
        /// <param name="styleAccessObject">
        /// La violazione da aggiungere.
        /// </param>
        public void AddViolation(SAObject styleAccessObject)
        {
            this.Violations.Add(styleAccessObject);
        }
    }
}