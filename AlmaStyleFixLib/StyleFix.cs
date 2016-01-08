//-------------------------------------------------------------------------------------------------
// <copyright file="StyleFix.cs" company="OfficeClip LLC">
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
    using System.Text.RegularExpressions;

    using StyleCop;

    /// <summary>
    /// Definisce il formato delle violazioni.
    /// </summary>
    public struct Violation
    {
        /// <summary>
        /// Contiene il messaggio di errore.
        /// </summary>
        public string ErrorMessage;

        /// <summary>
        /// Contiene il numero di riga.
        /// </summary>
        public int LineNumber;
    }

    /// <summary>
    /// A class to fix the styles.
    /// </summary>
    public class StyleFix
    {
        /// <summary>
        /// Il percorso del documento da correggere.
        /// </summary>
        private static string filePath;

        /// <summary>
        /// La struttura delle righe.
        /// </summary>
        private static List<SFWorkingLine> sfWorkingLines = new List<SFWorkingLine>();

        /// <summary>
        /// Usato per aggiungere le violazioni da styleCop.
        /// </summary>
        private static StringBuilder violationContent;

        /// <summary>
        /// Dizionario che contiene la lista delle violazioni per ogni file del progetto.
        /// </summary>
        private static Dictionary<string, List<Violation>> violations = new Dictionary<string, List<Violation>>();

        /// <summary>
        /// Autore del file da inserire nello header.
        /// </summary>
        private string author;

        /// <summary>
        /// Societ? da inserire nello header.
        /// </summary>
        private string company;

        /// <summary>
        /// 
        /// </summary>
        private bool forceFix;

        /// <summary>
        /// Il percorso del progetto.
        /// </summary>
        private string projectPath;

        /// <summary>
        /// Inizializza una nuova istanza della classe StyleFix.
        /// </summary>
        /// <param name="theProjectPath">
        /// Il percorso del progetto.
        /// </param>
        /// <param name="theFilePath">
        /// Il percorso del file.
        /// </param>
        /// <param name="company">
        /// Il nome della Societ? da inserire nello header.
        /// </param>
        /// <param name="author">
        /// Il nome dell'autore del file.
        /// </param>
        /// <param name="forceFix">
        /// Se true esegue il filx anche in presenza di errori di debug.
        /// </param>
        public StyleFix(string theProjectPath, string theFilePath, string company, string author, bool forceFix)
        {
            this.projectPath = theProjectPath;
            filePath = theFilePath;
            this.company = company;
            this.author = author;
            this.forceFix = forceFix;
        }

        /// <summary>
        /// Recupera o imposta un dizionario che contiene, per ogni file, la lista degli errori.
        /// </summary>
        public static Dictionary<string, List<Violation>> Violations
        {
            get { return StyleFix.violations; }
            set { StyleFix.violations = value; }
        }

        /// <summary>
        /// Calcola le violazioni corrette.
        /// </summary>
        /// <param name="total">
        /// Il numero di violazioni totali.
        /// </param>
        /// <param name="corrected">
        /// Il numero di violazioni corrette.
        /// </param>
        public void CalculateViolations(out int total, out int corrected)
        {
            total = 0;
            corrected = 0;
            foreach (SFWorkingLine workingLine in sfWorkingLines)
            {
                total += workingLine.TotalViolationsCount;
                corrected += workingLine.TotalFixedViolationsCount;
            }
        }

        /// <summary>
        /// Restituisce il nuovo testo corretto.
        /// </summary>
        /// <returns>
        /// Il testo corretto.
        /// </returns>
        public string GetNewText()
        {
            decimal lineNumber = 0;
            SFWorkingLine sfWorkingLine;
            string input;
            if (File.Exists(filePath))
            {
                StreamReader sr = new StreamReader(File.OpenRead(filePath));
                sfWorkingLines = new List<SFWorkingLine>();       // stronzi le liste si inizializzano!!!!
                while ((input = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    sfWorkingLine = new SFWorkingLine(lineNumber, input, false, new List<SAObject>());
                    sfWorkingLines.Add(sfWorkingLine);
                }

                sr.Close();
                if (this.projectPath != null)
                {
                    this.RunStyleCop(this.projectPath, filePath);
                    this.FixStyleCop();
                }

                // Write the output file
                return this.OutputFileContent();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Outputs the content of the file.
        /// </summary>
        /// <returns>
        /// The output content.
        /// </returns>
        public string OutputFileContent()
        {
            StringBuilder sb = new StringBuilder();
            foreach (SFWorkingLine workingLine in sfWorkingLines)
            {
                if (!workingLine.IsRemove)
                {
                    sb.Append(workingLine.Line);
                    sb.Append(System.Environment.NewLine);
                }
            }

            string outputLines = sb.ToString();
            (new Drivers.DocumentationRules() { Company = this.company, Author = this.author }).SA1633_FileMustHaveHeader(Path.GetFileName(filePath), ref outputLines);
            return outputLines;
        }

        /// <summary>
        /// Lancia syileCop su un progetto. le violazioni verranno corrette in maniera mirata.
        /// </summary>
        /// <param name="projectPath">
        /// Il percorso del progetto.
        /// </param>
        /// <param name="filePath">
        /// Il percorso del documento da correggere.
        /// </param>
        public void RunStyleCop(string projectPath, string filePath)
        {
            // Debug.Assert(sfWorkingLines.Count > 0);
            violationContent = new StringBuilder();
            if (!Violations.ContainsKey(filePath))
            {
                violations.Add(filePath, new List<Violation>());
            }

            violations[filePath] = new List<Violation>();       // reset violation list
            StyleCopConsole console = new StyleCopConsole(null, false, null, null, true);
            CodeProject project = new CodeProject(0, projectPath, new Configuration(null));
            console.Core.Environment.AddSourceCode(project, filePath, null);
            console.ViolationEncountered += OnViolationEncountered;
            console.Start(new[] { project }, true);
            console.ViolationEncountered -= OnViolationEncountered;

            // console.Dispose();
        }

        /// <summary>
        /// Handler dell'evento violazione.
        /// </summary>
        /// <param name="sender">
        /// Chi invoca l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private static void OnViolationEncountered(object sender, ViolationEventArgs e)
        {
            Violations[filePath].Add(new Violation() { LineNumber = e.LineNumber - 1, ErrorMessage = e.Violation.Rule.CheckId + ": " + e.Message });
            sfWorkingLines[e.LineNumber - 1].AddViolation(new SAObject(e.Violation.Rule.CheckId, e.LineNumber, e.Message, false));

            violationContent.AppendFormat(
                "{0}:{1}:{2} - {3}{4}",
                e.Violation.Rule.CheckId,
                e.Violation.SourceCode.Name,
                e.LineNumber,
                e.Message,
                System.Environment.NewLine);
        }

        /// <summary>
        /// Lancia i correttori delle regole SA15* (righe vuote).
        /// </summary>
        private void FixBlankLineRules()
        {
            Drivers.BlankLineRules blankLineRules = new Drivers.BlankLineRules();
            blankLineRules.SA1500_IfTheStatementSpansMultipleLinesTheClosingBracketMustBePlacedOnItsOwnLine(ref sfWorkingLines);
            blankLineRules.SA1503_TheBodyOfTheStatementIfMustBeWrappedInOpeningAndClosingBrackets(ref sfWorkingLines);
            blankLineRules.SA1505_AnOpeningCurlyBracketMustNotBeFolloewdByBlankLine(ref sfWorkingLines);
            blankLineRules.SA1508_AClosingCurlyBracketMustNotBePrecededByBlankLine(ref sfWorkingLines);
            blankLineRules.SA1512_ASingleLineCommentMustNotBeFollowedByBlankLine(ref sfWorkingLines);
            blankLineRules.SA1513_StatementsOrElementesWrappedInCurlyBracetsMustBeFollowedByBlankLine(ref sfWorkingLines);
            blankLineRules.SA1514andSA1515_ASingleLineCommentMustBePrecededByBlankLineOrAnotherSingleLineComment(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia i correttori delle regole CR00* (regole custom in italiano).
        /// </summary>
        private void FixCustomRules()
        {
            Drivers.AlmaCustomRules almaCustomRules = new Drivers.AlmaCustomRules();
            almaCustomRules.CR0001(ref sfWorkingLines);
            almaCustomRules.CR0003(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia i correttori delle regole SA16* (summary).
        /// </summary>
        private void FixDocumentation()
        {
            Drivers.DocumentationRules documentationRules = new Drivers.DocumentationRules();
            documentationRules.SA1600_SA1601TheMethodOrClassMustHaveAnHeader(ref sfWorkingLines);
            documentationRules.SA1611andSA1612and1618and1620_ParamTagsMustMatchTheParameterList(ref sfWorkingLines);
            documentationRules.SA1628_TheDocumentationTextWithinTheSummaryMustBegineWithCapitalLetter(ref sfWorkingLines);
            documentationRules.SA1629_TheDocumentationTextWithinTheSummaryMustEndWithPeriod(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia il correttore delle regole che gestiscono i "modifiers".
        /// </summary>
        private void FixModifiers()
        {
            Drivers.ModifierRules modRules = new Drivers.ModifierRules();
            modRules.SA1400_TheMethodMustHaveAnAccessModifier(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia i correttori delle regole SA11* (leggibilita').
        /// </summary>
        private void FixReadabilityRules()
        {
            Drivers.ReadabilityRules readibilityRules = new Drivers.ReadabilityRules();
            readibilityRules.SA1101_PrefixLocalCallsWithThis(ref sfWorkingLines);
            readibilityRules.SA1120_TheCommentIsEmptyAddTextToTheCommentOrRemoveIt(ref sfWorkingLines);
            readibilityRules.SA1121_UseBuiltInTypeAlias(ref sfWorkingLines);
            readibilityRules.SA1126_PrefixLocalCallsWithThis(ref sfWorkingLines);
            readibilityRules.SA1122_UseStringEmpty(ref sfWorkingLines);
            readibilityRules.Sa1117_ParametersMustBeOnTheSameLineOrSeparatedLines(ref sfWorkingLines);
            readibilityRules.Sa1116_FirstParameterMustBeginInLineBeheart(ref sfWorkingLines);
            readibilityRules.SA1115_TheParameterMustBeginOnLineAfterPreviusParameter(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia i correttori delle regole SA13* (renaming).
        /// </summary>
        private void FixRenaming()
        {
            Drivers.RenamingRules renamingRules = new Drivers.RenamingRules();
            renamingRules.SA1300_MethodNamesBeginWithAnUpperCaseLetter(ref sfWorkingLines);
            renamingRules.SA1303_ConstatsMustStartWithAnUpperCaseLetter(ref sfWorkingLines);
            renamingRules.SA1306_VariableNamesBeginWithAnLowCaseLetter(ref sfWorkingLines);
            renamingRules.SA1307_PublicAndInternalFieldsMustStartWithAnUpperCaseLetter(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia i correttori delle regole SA10* (regole di spaziatura).
        /// </summary>
        private void FixSpacingRules()
        {
            Drivers.SpacingRules spacingRules = new Drivers.SpacingRules();

            // spacingRules.SA1000_KeywordsMustBeSpacedCorrectly(ref sfWorkingLines);
            // spacingRules.SA1001_CommasMustBeSpaceCorrectly(ref sfWorkingLines);
            // spacingRules.SA1002_SemicolonsMustBeSpaceCorrectly(ref sfWorkingLines);
            // spacingRules.SA1009_ClosingParenthesisMustBeSpacedCorrectly(ref sfWorkingLines);
            // spacingRules.SA1010_OpeningSquareBracketsMustBeSpacedCorrectly(ref sfWorkingLines);
            spacingRules.SA1005_SingleLineCommentsMustBeginWithSingleSpace(ref sfWorkingLines);
        }

        /// <summary>
        /// Lancia tutti i correttori.
        /// </summary>
        private void FixStyleCop()
        {
            Drivers.CheckErrors checkForErrors = new Drivers.CheckErrors();
            var errorLine = checkForErrors.SA0102_ErrorInCompiling(ref sfWorkingLines);
            if (errorLine >= 0)
            {
                if (!this.forceFix)
                {
                    throw new Exception("Errore in compilazione al rigo " + errorLine);
                }
            }

            this.FixRenaming();
            this.FixSpacingRules();
            this.FixReadabilityRules();
            this.FixBlankLineRules();
            this.FixDocumentation();
            this.FixCustomRules();
            this.FixUsing();
            this.FixModifiers();
        }

        /// <summary>
        /// Risolve i problemi legati all'ordine degli USING. Viene sostituita da Narrange.
        /// </summary>
        private void FixUsing()
        {
            // Drivers.AlmaUsing usingRules = new Drivers.AlmaUsing();
            // usingRules.sa1200(ref sfWorkingLines);
        }
    }
}