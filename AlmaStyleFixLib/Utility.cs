//-------------------------------------------------------------------------------------------------
// <copyright file="Utility.cs" company="OfficeClip LLC">
// Copyright (c) OfficeClip LLC.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace AlmaStyleFixLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Fornisce alcune utility.
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Ritorna la directory di esecuzione dell'assembly corrente.
        /// </summary>
        /// <returns>
        /// La directory dell'asembly.
        /// </returns>
        public string GetSetupDir()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            return Path.GetDirectoryName(asm.Location);
        }

        /// <summary>
        /// Esegue un processo esterno.
        /// </summary>
        /// <param name="dir">
        /// La directory del file EXE.
        /// </param>
        /// <param name="programPath">
        /// Il nome del file EXE.
        /// </param>
        /// <param name="processArgument">
        /// Parametri del processo.
        /// </param>
        /// <param name="filePath">
        /// Path del file target.
        /// </param>
        /// <returns>
        /// Il valore di ritorno del processo in formato stringa.
        /// </returns>
        public string Run(string dir, string programPath, string processArgument, string filePath)
        {
            string retVal = string.Empty;
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo(
                    string.Format(@"{0}\{1}", dir, programPath));

                processStartInfo.Arguments = string.Format(
                                                 @"{0} ""{1}""", processArgument, filePath);

                processStartInfo.UseShellExecute = false;
                processStartInfo.ErrorDialog = false;

                processStartInfo.RedirectStandardError = true;
                processStartInfo.RedirectStandardOutput = false;
                processStartInfo.RedirectStandardInput = false;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                bool started = proc.Start();
                if (started)
                {
                    retVal = proc.StandardError.ReadToEnd();
                    if (retVal != string.Empty)
                    {
                        retVal = string.Format("{1}AStyle: {0}{1}", retVal, System.Environment.NewLine);
                    }

                    proc.WaitForExit(7000);
                }
            }
            catch (Exception e)
            {
                retVal = string.Format("{1}AStyle: {0}{1}", e.Message, System.Environment.NewLine);
            }

            return retVal;
        }
    }
}