//----------------------------------------------------------------------------------------------
// <copyright file="AlmaStyleFixLib.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using NArrange.Core;

    /// <summary>
    /// Questa classe corregge il codice secondo le regole definite da StyleCop.
    /// Elimina alcuni Warning, ma non tutti.
    /// </summary>
    public class AlmaStyleFix
    {
        /// <summary>
        /// Puntatore ad una istanza della classe styleFix.
        /// </summary>
        private StyleFix stf = null;

        /// <summary>
        /// File temporaneo usato per salvare i risultati parziali prima di invocare NArrange.
        /// </summary>
        private string tempFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "/tmp.cs";

        /// <summary>
        /// Ottiene il dizionario con la lista delle violazioni.
        /// </summary>
        /// <returns>
        /// La lista delle violazioni.
        /// </returns>
        public Dictionary<string, List<Violation>> GetViolations()
        {
            return StyleFix.Violations;
        }

        /// <summary>
        /// Lancia NArrange su una stringa.
        /// </summary>
        /// <param name="oldText">
        /// La stringa di partenza.
        /// </param>
        /// <returns>
        /// Il nuovo testo corretto.
        /// </returns>
        public string GoNArrange(string oldText)
        {
            var myTempFileWrite = new StreamWriter(this.tempFilePath, false);

            myTempFileWrite.Write(oldText);
            myTempFileWrite.Close();

            FileArranger fileArranger = new FileArranger(string.Format(@"{0}\NArrangeConfig.xml", (new Utility()).GetSetupDir()), null);

            // FileArranger fileArranger = new FileArranger(string.Format(@"{0}\DefaultConfig.xml", (new Utility()).GetSetupDir()), null);
            bool success = fileArranger.Arrange(this.tempFilePath, this.tempFilePath, false);

            var myTempFileRead = new StreamReader(this.tempFilePath);
            var ret = myTempFileRead.ReadToEnd();
            myTempFileRead.Close();
            File.Delete(this.tempFilePath);
            return ret;

            // return oldText;
        }

        /// <summary>
        /// Lancia stylefix su questo documento.
        /// </summary>
        /// <param name="projectPath">
        /// Il percorso del file di progetto.
        /// </param>
        /// <param name="filePath">
        /// Il percorso del documento corrente.
        /// </param>
        /// <param name="company">
        /// Societ? da inserire nello header.
        /// </param>
        /// <param name="author">
        /// Autore da inserire nello header.
        /// </param>
        /// <returns>
        /// Il testo corretto.
        /// </returns>
        public string GoStyleFix(string projectPath, string filePath, string company, string author, bool forceFix)
        {
            this.stf = new StyleFix(projectPath, filePath, company, author, forceFix);
            var ret = this.stf.GetNewText();
            return ret;
        }

        /// <summary>
        /// Lancia styleCop modificando la struttura dati dell'evidenziatore.
        /// </summary>
        /// <param name="projectPath">
        /// Il path del progetto.
        /// </param>
        /// <param name="filePath">
        /// Il path del file.
        /// </param>
        public void RunStyleCop(string projectPath, string filePath)
        {
            if (this.stf != null)
            {
                this.stf.RunStyleCop(projectPath, filePath);
            }
        }
    }
}