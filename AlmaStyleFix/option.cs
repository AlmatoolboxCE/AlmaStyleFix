//----------------------------------------------------------------------------------------------
// <copyright file="option.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------
namespace TSF.AlmaStyleFix
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// Classe che gestisce le opzioni di Visual Studio.
    /// </summary>   
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false),
    ComVisible(true)]
    public class ToolsOptions : DialogPage
    {
        /// <summary>
        /// Parola che rappresenta il colore da utilizzare nell'evidenziatore.
        /// </summary>
        private int a = 127, r = 250, g = 10, b = 10;

        /// <summary>
        /// Contiene il nome dell'autore.
        /// </summary>
        private string author = "Almaviva TSF Developer";

        /// <summary>
        /// Campo privato che indica se eseguire il controllo degli aggiornamenti.
        /// </summary>
        private bool checkForUpdates = false;

        /// <summary>
        /// Contiene il nome della società.
        /// </summary>
        private string company = "Almaviva TSF";

        /// <summary>
        /// Recupera o imposta il comportamento in caso di errori di compilazione
        /// </summary>
        private bool forceFixingOnCompileError = false;

        /// <summary>
        /// Campo privato che indica se salvare automaticamente.
        /// </summary>
        private bool onFixSave = true;

        /// <summary>
        /// Campo privato che contiene il valore che indica se usare il versioning.
        /// </summary>
        private bool useVersioning = true;

        /// <summary>
        /// Recupera o imposta il livello di trasparenza.
        /// </summary>
        public int A
        {
            get { return this.a; }
            set { this.a = value; }
        }

        /// <summary>
        /// Recupera o imposta il nome dell'autore.
        /// </summary>
        public string Author
        {
            get { return this.author; }
            set { this.author = value; }
        }

        /// <summary>
        /// Recupera o imposta il livello di blu.
        /// </summary>
        public int B
        {
            get { return this.b; }
            set { this.b = value; }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se eseguire il controllo degli aggiornamenti.
        /// </summary>
        public bool CheckForUpdates
        {
            get { return this.checkForUpdates; }
            set { this.checkForUpdates = value; }
        }

        /// <summary>
        /// Recupera o imposta il nome della società.
        /// </summary>
        public string Company
        {
            get { return this.company; }
            set { this.company = value; }
        }

        /// <summary>
        /// Recupera o imposta il comportamento in caso di errori di compilazione
        /// </summary>
        public bool ForceFixingOnCompileError
        {
            get { return this.forceFixingOnCompileError; }
            set { this.forceFixingOnCompileError = value; }
        }

        /// <summary>
        /// Recupera o imposta il livello di verde.
        /// </summary>
        public int G
        {
            get { return this.g; }
            set { this.g = value; }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se salvare automaticamente.
        /// </summary>
        public bool OnFixSave
        {
            get { return this.onFixSave; }
            set { this.onFixSave = value; }
        }

        /// <summary>
        /// Recupera o imposta il livello di rosso.
        /// </summary>
        public int R
        {
            get { return this.r; }
            set { this.r = value; }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se usare l'evidenziatore.
        /// </summary>
        public bool UseAdornment
        {
            get { return HighLight.HighLighter.Enable; }
            set { HighLight.HighLighter.Enable = value; }
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se usare il versioning.
        /// </summary>        
        public bool UseVersioning
        {
            get { return this.useVersioning; }
            set { this.useVersioning = value; }
        }

        /// <summary>
        /// Recupera il controllo da utilizzare nella finestra delle opzioni.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                ToolOptionsUI page = new ToolOptionsUI();
                page.TheOptions = this;
                page.Initialize();
                return page;
            }
        }
    }
}