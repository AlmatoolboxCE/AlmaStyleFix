//----------------------------------------------------------------------------------------------
// <copyright file="AlmaStyleFixPackage.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------
namespace TSF.AlmaStyleFix
{
    using System;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    using EnvDTE;

    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.    
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    //// This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    //// a package.
    //// This attribute is used to register the informations needed to show the this package
    //// in the Help/About dialog of Visual Studio.
    //// This attribute is needed to let the shell know that this package exposes some menus.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.GuidAlmaStyleFixPkgString)]
    [ProvideOptionPage(typeof(ToolsOptions), "AlmaStyleFix", "General", 1001, 1002, true)]
    public sealed class AlmaStyleFixPackage : Package
    {
        /// <summary>
        /// Riferimento statico all'environement di Visual studio.
        /// </summary>
        private static DTE dte = null;

        /// <summary>
        /// Riferimento statico alle opzioni.
        /// </summary>
        private static ToolsOptions page = null;

        /// <summary>
        /// Resource manager per realizzare il multilingua.
        /// </summary>
        private ResourceManager locRM = new ResourceManager("TSF.AlmaStyleFix.Resources.MultiLang", System.Reflection.Assembly.GetExecutingAssembly()) { IgnoreCase = true };

        /// <summary>
        /// Ottimizzo salvando la versione corrente.
        /// </summary>
        private Version myVersion = null;

        /// <summary>
        /// Inizializza una nuova istanza della classe AlmaStyleFixPackage.
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public AlmaStyleFixPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// Recupera o imposta il riferimento statico alla variabile di ambiente di Visual Studio.
        /// </summary>
        public static DTE Dte
        {
            get { return AlmaStyleFixPackage.dte; }
            set { AlmaStyleFixPackage.dte = value; }
        }

        /// <summary>
        /// Recupera le opzioni.
        /// </summary>
        public static ToolsOptions Page
        {
            get { return page; }
            private set { page = value; }
        }

        /// <summary>
        /// Get the active project.
        /// </summary>
        /// <param name="dte">
        /// The Visual Studio interface parameter.
        /// </param>
        /// <returns>
        /// The active project.
        /// </returns>
        internal static Project GetActiveProject(DTE dte)
        {
            Project activeProject = null;

            Array activeSolutionProjects = dte.ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null && activeSolutionProjects.Length > 0)
            {
                activeProject = activeSolutionProjects.GetValue(0) as Project;
            }

            return activeProject;
        }

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.GuidAlmaStyleFixCmdSet, (int)PkgCmdIDList.IdAlmaStyleFix);
                MenuCommand menuItem = new MenuCommand(this.AlmaStyleFixCallback, menuCommandID);
                mcs.AddCommand(menuItem);
                menuCommandID = new CommandID(GuidList.GuidAlmaStyleFixCmdSet, (int)PkgCmdIDList.IdAlmaStyleFixSettings);
                menuItem = new MenuCommand(this.AlmaStyleFixSettingsCallback, menuCommandID);
                mcs.AddCommand(menuItem);
                menuCommandID = new CommandID(GuidList.GuidAlmaStyleFixCmdSet, (int)PkgCmdIDList.IdAlmaStyleFixUpdate);
                menuItem = new MenuCommand(this.AlmaStyleFixUpdateCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>        
        /// <param name="sender">
        /// The caller obj.
        /// </param>
        /// <param name="e">
        /// The call's parameters.
        /// </param>
        private void AlmaStyleFixCallback(object sender, EventArgs e)
        {
            Dte = null;

            try
            {
                Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;

                if (Dte == null)
                {
                    throw new Exception(this.locRM.GetString("NoDTE"));
                }
            }
            catch (Exception ex)
            {
                this.Show(this.locRM.GetString("ErrorDTE") + ex.Message);
                return;
            }

            // aggiorno leggo le opzioni
            page = (ToolsOptions)GetDialogPage(typeof(ToolsOptions));

            Project prj = null;
            Document theDoc = null;
            try
            {
                theDoc = Dte.ActiveDocument;
                if (theDoc == null)
                {
                    throw new Exception();
                }

                if (theDoc.ProjectItem != null)
                {
                    prj = theDoc.ProjectItem.ContainingProject;
                }

                if (prj == null)
                {
                    this.Show(this.locRM.GetString("NoPRJ"));
                    return;
                }

                if (!theDoc.FullName.EndsWith(".cs"))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Show(this.locRM.GetString("ErrorActive") + ex.Message);
                return;
            }

            try
            {
                if (theDoc.Saved == false)
                {
                    if (page.OnFixSave == true)
                    {
                        theDoc.Save();
                    }
                    else if (this.Question(this.locRM.GetString("QuestionSave")))
                    {
                        theDoc.Save();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Show(this.locRM.GetString("ErrorSave") + ex.Message);
                return;
            }

            try
            {
                ////AppDomain currentDomain = AppDomain.CurrentDomain;
                ////currentDomain.AssemblyResolve += new ResolveEventHandler(MyResolveEventHandler);
                TextDocument td = (TextDocument)theDoc.Object(string.Empty);
                var startDoc = td.CreateEditPoint();
                startDoc.StartOfDocument();
                var endDoc = td.CreateEditPoint();
                endDoc.EndOfDocument();

                // launch fixstyle here
                string prjPath = null;
                if (prj != null)
                {
                    prjPath = prj.FullName;
                }

                var styleFix = new AlmaStyleFixLib.AlmaStyleFix();
                var theNewText = styleFix.GoStyleFix(prjPath, theDoc.FullName, page.Company, page.Author, page.ForceFixingOnCompileError);

                if (page.UseNarrange)
                {
                    try
                    {
                        var appo = styleFix.GoNArrange(theNewText);
                        theNewText = appo;
                    }
                    catch (Exception ex)
                    {
                        this.Show(this.locRM.GetString("ErrorNarrange") + ex.Message);
                    }
                }

                try
                {
                    // replace document content
                    startDoc.ReplaceText(endDoc, theNewText, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
                }
                catch (Exception ex)
                {
                    this.Show(this.locRM.GetString("ErrorReplace") + " : " + ex.Message);
                    return;
                }

                try
                {
                    // rename using refactoring
                    foreach (string oldName in AlmaStyleFixLib.Drivers.RenamingRules.ToChange.Keys)
                    {
                        var newName = AlmaStyleFixLib.Drivers.RenamingRules.ToChange[oldName];

                        for (int i = 0; i < Dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElements.Count; i++)
                        {
                            var itm = Dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElements.Item(i + 1);
                            var target = this.RecursiveSearchByName(itm, oldName);
                            if (target != null)
                            {
                                ((EnvDTE80.CodeElement2)target).RenameSymbol(newName);
                                break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // ...  //non rinomino!
                }

                // reindent using edit.formatDocument
                Dte.ExecuteCommand("Edit.FormatDocument", string.Empty);

                if (page.UseAdornment)
                {
                    bool noSave = false;
                    if (page.OnFixSave == true)
                    {
                        theDoc.Save();
                    }
                    else if (this.Question(this.locRM.GetString("QuestionSave")))
                    {
                        theDoc.Save();
                    }
                    else
                    {
                        noSave = true;
                    }

                    if (!noSave)
                    {
                        // relaunch stylecop and set text adornment
                        styleFix.RunStyleCop(prjPath, theDoc.FullName);
                        HighLight.HighLighter.Violations = styleFix.GetViolations();

                        // ViewportAdornment1.ViewportAdornment1.ForceGo();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Show(this.locRM.GetString("ErrorStileFix") + ex.Message);
                return;
            }

            if (page.UseVersioning)
            {
                // versioning dell'assembly
                if (!this.Versioning(prj))
                {
                    // if (this.Question(this.locRM.GetString("QuestionVersioning")))
                    // {
                    //    page.UseVersioning = false;
                    // }
                }
            }

            if (page.CheckForUpdates)
            {
                this.AlmaStyleFixUpdateCallback(this, new EventArgs());
            }
        }

        /// <summary>
        /// Apre le impostazioni di AlmaStyleFix.
        /// </summary>
        /// <param name="sender">
        /// Chi scatena l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void AlmaStyleFixSettingsCallback(object sender, EventArgs e)
        {
            // SettingsManager settingsManager = new ShellSettingsManager(this);
            // SettingsStore configurationSettingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.Configuration);
            Dte = null;
            try
            {
                Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
                if (Dte == null)
                {
                    throw new Exception(this.locRM.GetString("NoDTE"));
                }
            }
            catch (Exception ex)
            {
                this.Show(this.locRM.GetString("ErrorDTE") + ex.Message);
                return;
            }

            // dte.ExecuteCommand("Tools.Options", "AlmaStyleFix"); //it doesn't work
            // use this. It's open AlmaStyleFix directly
            this.ShowOptionPage(typeof(ToolsOptions));
        }

        /// <summary>
        /// Handler dell'evento di pressione del bottone UPDATE.
        /// </summary>
        /// <param name="sender">
        /// Chi scatena l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void AlmaStyleFixUpdateCallback(object sender, EventArgs e)
        {
            var ts = new System.Threading.ThreadStart(this.CheckForUpdates);
            var t = new System.Threading.Thread(ts);
            t.Priority = System.Threading.ThreadPriority.Lowest;
            t.Start();
        }

        /// <summary>
        /// Esegue il controllo sulle versioni e lancia il programma di aggiornamento.
        /// </summary>
        private void CheckForUpdates()
        {
            string theVersion = null;
            if (this.myVersion == null)
            {
                var myDir = System.Reflection.Assembly.GetExecutingAssembly().Location;
                myDir = System.IO.Path.GetDirectoryName(myDir);
                var myFiles = System.IO.Directory.GetFiles(myDir);
                string myManifest = null;
                foreach (var f in myFiles)
                {
                    if (f.ToLower().EndsWith(".vsixmanifest"))
                    {
                        myManifest = f;
                        break;
                    }
                }

                if (myManifest == null)
                {
                    return;
                }

                using (var theFile = System.IO.File.OpenRead(myManifest))
                {
                    var xml = System.Xml.XmlReader.Create(theFile);
                    xml.ReadToFollowing("Version");
                    theVersion = xml.ReadString();
                    xml.Close();
                }

                if (theVersion == null)
                {
                    return;
                }

                this.myVersion = new Version(theVersion);
                if (!this.myVersion.IsValid())
                {
                    this.myVersion = null;
                    return;
                }
            }

            if (System.IO.Directory.Exists(@"S:\AlmaStyleFix\Updates"))
            {
                var files = new System.Collections.Generic.List<string>();
                files.AddRange(System.IO.Directory.GetFiles(@"S:\AlmaStyleFix\Updates"));
                for (int i = 0; i < files.Count; i++)
                {
                    if (!files[i].EndsWith(".vsix"))
                    {
                        files.RemoveAt(i--);
                    }
                }

                if (files.Count > 0)
                {
                    string fileToUpdate = null;
                    foreach (var f in files)
                    {
                        var theFileVersion = new Version(f);
                        if (!theFileVersion.IsValid())
                        {
                            continue;
                        }

                        if (theFileVersion > this.myVersion)
                        {
                            fileToUpdate = f;
                            break;
                        }
                    }

                    if (fileToUpdate != null)
                    {
                        if (this.Question(this.locRM.GetString("Update")))
                        {
                            var commonFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
                            commonFolder += "\\microsoft shared\\MSEnv\\";
                            var launcher = commonFolder + "VSLauncher.exe";

                            var pi = new System.Diagnostics.ProcessStartInfo();
                            pi.FileName = launcher;
                            pi.Arguments = fileToUpdate;

                            var p = new System.Diagnostics.Process();

                            p.StartInfo = pi;
                            p.Start();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ritorna la versione del branch contenuta nel path della relativa cartella.
        /// </summary>
        /// <param name="folder">
        /// Il nome della cartella.
        /// </param>
        /// <returns>
        /// Una stringa che contiene la versione corrente (xx.yy.ww.zz).
        /// </returns>
        private string GetVersionFromFolder(string folder)
        {
            var match = Regex.Match(folder, @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+");
            if (match.Success == true)
            {
                return match.Value;
            }

            return null;
        }

        /// <summary>
        /// Stampa a video un popUp del tipo OK/CANCEL.
        /// </summary>
        /// <param name="text">
        /// Testo del popup.
        /// </param>
        /// <returns>
        /// Se OK true, altrimenti false.
        /// </returns>
        private bool Question(string text)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            int result;
            Guid clsid = Guid.Empty;
            uiShell.ShowMessageBox(
                0,
                ref clsid,
                "AlmaStyleFix",
                text,
                string.Empty,
                0,
                OLEMSGBUTTON.OLEMSGBUTTON_OKCANCEL,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                OLEMSGICON.OLEMSGICON_INFO,
                0,        // false
                out result);
            if (result == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ricerca ricorsivamente il nome specificato nel DOM del documento.
        /// </summary>
        /// <param name="father">
        /// Puntatore all'elemento padre.
        /// </param>
        /// <param name="name">
        /// Nome del nodo da ricercare.
        /// </param>
        /// <returns>
        /// Il puntatore al nodo trovato, o null se non trovato.
        /// </returns>
        private CodeElement RecursiveSearchByName(CodeElement father, string name)
        {
            if (father == null)
            {
                return null;
            }

            try
            {
                if (father.Name == name)
                {
                    return father;
                }
            }
            catch (Exception)
            {
                return null;
            }

            for (int i = 0; i < father.Children.Count; i++)
            {
                var itm = father.Children.Item(i + 1);
                try
                {
                    if (itm.Name == name)
                    {
                        return itm;
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                var ret = this.RecursiveSearchByName(itm, name);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        /// <summary>
        /// Salva tutti i documenti aperti.
        /// </summary>
        private void SaveAll()
        {
            Dte = Package.GetGlobalService(typeof(SDTE)) as DTE;
            for (int i = 1; i <= Dte.Documents.Count; i++)
            {
                var theDoc = Dte.Documents.Item(i);
                if (theDoc.FullName.EndsWith(".cs"))
                {
                    theDoc.Save();
                }
            }
        }

        /// <summary>
        /// Stampa a video un popUp.
        /// </summary>
        /// <param name="text">
        /// Tesro del PopUp.
        /// </param>        
        private void Show(string text)
        {
            IVsUIShell uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            int result;
            Guid clsid = Guid.Empty;
            uiShell.ShowMessageBox(
                       0,
                          ref clsid,
                          "AlmaStyleFix",
                          text,
                          string.Empty,
                          0,
                          OLEMSGBUTTON.OLEMSGBUTTON_OK,
                          OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                          OLEMSGICON.OLEMSGICON_INFO,
                          0,        // false
                          out result);
            return;
        }

        /// <summary>
        /// Verifica la versione del progetto e la confronta con quella del branch.
        /// </summary>
        /// <param name="prj">
        /// Il progetto corrente.
        /// </param>
        /// <returns>
        /// True se la versione è corretta, altrimenti false.
        /// </returns>
        private bool Versioning(Project prj)
        {
            if (prj == null)
            {
                return false;
            }

            var branchName = prj.FullName;

            branchName.Replace("/", "\\");

            var branchVersion = branchName;

            while (true)
            {
                int finInd = branchVersion.LastIndexOf("\\");

                if (finInd < 0)
                {
                    return false;
                }

                branchVersion = branchVersion.Substring(0, finInd);

                int iniInd = branchVersion.LastIndexOf("\\");
                if (iniInd < 0)
                {
                    return false;
                }

                var branchVersionCurrent = branchVersion.Substring(iniInd + 1);
                branchVersionCurrent = this.GetVersionFromFolder(branchVersionCurrent);
                if (branchVersionCurrent != null)
                {
                    branchVersion = branchVersionCurrent;
                    break;
                }
            }

            Properties properties = prj.Properties;
            var thisVersion = properties.Item("AssemblyVersion").Value.ToString();

            // get version from path
            if (branchVersion.CompareTo(thisVersion) != 0)
            {
                if (this.Question(string.Format(this.locRM.GetString("QuestionChangeVersion"), thisVersion, branchVersion)))
                {
                    properties.Item("AssemblyVersion").Value = branchVersion;
                    properties.Item("AssemblyFileVersion").Value = branchVersion;
                }
            }

            return true;
        }

        /// <summary>
        /// Contiene un dato di tipo VERSIONE.
        /// </summary>
        private class Version
        {
            /// <summary>
            /// Inizializza una nuova istanza della classe Version.
            /// </summary>
            /// <param name="theVersion">
            /// La stringa che contiene la versione.
            /// </param>
            public Version(string theVersion)
            {
                var match = System.Text.RegularExpressions.Regex.Match(theVersion, @"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+");
                if (!match.Success)
                {
                    return;
                }

                var toStringArray = match.Value.Split(new char[] { '.' });
                if (toStringArray.Length != 4)
                {
                    return;
                }

                try
                {
                    var toIntArray = new int[4];
                    for (int i = 0; i < 4; i++)
                    {
                        toIntArray[i] = int.Parse(toStringArray[i]);
                    }

                    this.SetValuesByIntArray(toIntArray);
                }
                catch
                {
                    return;
                }
            }

            /// <summary>
            /// Inizializza una nuova istanza della classe Version.
            /// </summary>
            /// <param name="theVersion">
            /// Array di interi che contiene la versione.
            /// </param>
            public Version(int[] theVersion)
            {
                this.SetValuesByIntArray(theVersion);
            }

            /// <summary>
            /// Recupera o imposta il numero major.
            /// </summary>
            public int Major
            {
                get;
                set;
            }

            /// <summary>
            /// Recupera o imposta il numero minor.
            /// </summary>
            public int Minor
            {
                get;
                set;
            }

            /// <summary>
            /// Recupera o imposta il numero di patch.
            /// </summary>
            public int Patch
            {
                get;
                set;
            }

            /// <summary>
            /// Recupera o imposta il numero di revisione.
            /// </summary>
            public int Revision
            {
                get;
                set;
            }

            /// <summary>
            /// Compara due versioni.
            /// </summary>
            /// <param name="a">
            /// Primo operando.
            /// </param>
            /// <param name="b">
            /// Secondo operando.
            /// </param>
            /// <returns>
            /// True se a minore di b.
            /// </returns>
            public static bool operator <(Version a, Version b)
            {
                return IsAbove(b, a);
            }

            /// <summary>
            /// Compara due versioni.
            /// </summary>
            /// <param name="a">
            /// Primo operando.
            /// </param>
            /// <param name="b">
            /// Secondo operando.
            /// </param>
            /// <returns>
            /// True se a>b.
            /// </returns>
            public static bool operator >(Version a, Version b)
            {
                return IsAbove(a, b);
            }

            /// <summary>
            /// Indica se la versione è valida.
            /// </summary>
            /// <returns>
            /// true se la versione è valida.
            /// </returns>
            public bool IsValid()
            {
                if (this.Major > 0)
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// Compara due versioni e restituisce true se la prima è più grande.
            /// </summary>
            /// <param name="a">
            /// La prima versione da comparare.
            /// </param>
            /// <param name="b">
            /// La seconda versione da comparare.
            /// </param>
            /// <returns>
            /// True se a>b.
            /// </returns>
            private static bool IsAbove(Version a, Version b)
            {
                if (a.Major > b.Major)
                {
                    return true;
                }

                if (a.Major == b.Major)
                {
                    if (a.Minor > b.Minor)
                    {
                        return true;
                    }

                    if (a.Minor == b.Minor)
                    {
                        if (a.Revision > b.Revision)
                        {
                            return true;
                        }

                        if (a.Revision == b.Revision)
                        {
                            if (a.Patch > b.Patch)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Imposta i dati interni da un array di interi di lunghezza 4.
            /// </summary>
            /// <param name="theVersion">
            /// L'array di interi che rappresenta la versione.
            /// </param>
            private void SetValuesByIntArray(int[] theVersion)
            {
                if (theVersion.Length != 4)
                {
                    return;
                }

                this.Major = theVersion[0];
                this.Minor = theVersion[1];
                this.Revision = theVersion[2];
                this.Patch = theVersion[3];
            }
        }
    }
}