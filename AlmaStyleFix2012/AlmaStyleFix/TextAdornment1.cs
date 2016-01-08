//----------------------------------------------------------------------------------------------
// <copyright file="TextAdornment1.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace TSF.HighLight
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using Microsoft.VisualStudio.Utilities;

    using TSF.AlmaStyleFix;

    /// <summary>
    /// Evidenzia le righe che contengono errori di stile.
    /// </summary>
    public class HighLighter : IDisposable
    {
        /// <summary>
        /// Campo privato che contiene informazioni sulle violazioni di stile.
        /// </summary>
        private static Dictionary<string, List<AlmaStyleFixLib.Violation>> violations;

        /// <summary>
        /// Colore dell'evidenziatore.
        /// </summary>
        private Brush brush;

        /// <summary>
        /// Layer su cui aggiungere l'evidenziazione.
        /// </summary>
        private IAdornmentLayer layer;

        /// <summary>
        /// Disegnatore dell'evidenziatore.
        /// </summary>
        private Pen pen;

        /// <summary>
        /// Variabile in cui salvare il parametro passato da factory.
        /// </summary>
        private IWpfTextView view;

        /// <summary>
        /// Inizializza una nuova istanza della classe HighLighter Inizializza una nuova istanza della classe TextAdornment1.
        /// </summary>
        /// <param name="view">
        /// Parametro passato da factory.
        /// </param>
        public HighLighter(IWpfTextView view)
        {
            this.view = view;
            this.layer = view.GetAdornmentLayer("TextAdornment1");

            // Listen to any event that changes the layout (text changes, scrolling, etc)
            this.view.LayoutChanged += this.OnLayoutChanged;
            this.view.TextBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(this.TextBuffer_Changed);

            this.SetColor();
        }

        /// <summary>
        /// Recupera o imposta un valore che indica se usare l'evidenziatore.
        /// </summary>
        public static bool Enable
        {
            get;
            set;
        }

        /// <summary>
        /// Recupera o imposta la lista delle violazioni.
        /// </summary>
        public static Dictionary<string, List<AlmaStyleFixLib.Violation>> Violations
        {
            get
            {
                return violations;
            }

            set
            {
                violations = value;
            }
        }

        /// <summary>
        /// Prepara la classe all'eliminazione.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Viene lanciato prima di cancellare la classe. Mi serve per interrompere l'ascolto dell'evento.
        /// </summary>
        /// <param name="disposing">
        /// Parametro richiesto dall'interfaccia.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            this.view.LayoutChanged -= this.OnLayoutChanged;
        }

        /// <summary>
        /// Evidenzia le righe con gli errori.
        /// </summary>
        /// <param name="line">
        /// Puntatore al formato grafico della linea da modificare.
        /// </param>
        /// <param name="filePath">
        /// FullName del documento attivo.
        /// </param>
        private void CreateVisuals(ITextViewLine line, string filePath)
        {
            // controllo l'indice della riga corrente                      
            int ind = 0;
            //var appo = this.view.TextSnapshot.Lines.Select((val, i) => new { val, i }).FirstOrDefault(itm => itm.val.Start == line.Start);
            //if (appo != null) ind = appo.i;
            bool found = false;
            foreach (ITextSnapshotLine l in this.view.TextSnapshot.Lines)
            {
                if (l.Start == line.Start)
                {
                    found = true;
                    break;
                }

                ind++;
            }

            // controllo l'indice delle violazioni
            int violationIndex = -1;
            if (found)
            {
                found = false;

                for (int i = 0; i < Violations[filePath].Count; i++)
                {
                    if (Violations[filePath][i].LineNumber == ind)
                    {
                        if (string.IsNullOrEmpty(violations[filePath][i].ErrorMessage))
                        {
                            violations[filePath].RemoveAt(i--);
                        }
                        else
                        {
                            violationIndex = i;
                            found = true;
                            break;
                        }
                    }
                }
            }

            if (!found)
            {
                return;
            }

            // grab a reference to the lines in the current TextView
            IWpfTextViewLineCollection textViewLines = this.view.TextViewLines;
            int start = line.Start;
            int end = line.End;

            // evidenzio a partire dal primo carattere non vuoto
            bool isFirstBlank = true;
            for (int i = start; i < end; ++i)
            {
                if (isFirstBlank && this.view.TextSnapshot[i] == ' ')
                {
                    start++;
                }
                else
                {
                    isFirstBlank = false;
                }
            }

            // eseguo la colorazione del rigo
            SnapshotSpan span = new SnapshotSpan(this.view.TextSnapshot, Span.FromBounds(start, end));
            Geometry g = textViewLines.GetMarkerGeometry(span);

            if (g != null)
            {
                GeometryDrawing drawing = new GeometryDrawing(this.brush, this.pen, g);
                drawing.Freeze();

                DrawingImage drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                Image image = new Image();
                image.ToolTip = Violations[filePath][violationIndex].ErrorMessage;
                image.Source = drawingImage;

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, g.Bounds.Left);
                Canvas.SetTop(image, g.Bounds.Top);

                this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            }
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines.
        /// </summary>
        /// <param name="sender">
        /// Oggetto che lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            // ottimizzazione! controllo i prerequisiti
            if (!Enable || Violations == null || Violations.Count == 0)
            {
                return;
            }

            // check for active document
            if (AlmaStyleFixPackage.Dte == null)
            {
                return;        // noDTE
            }

            var theDoc = AlmaStyleFixPackage.Dte.ActiveDocument;
            if (theDoc == null)
            {
                return;         // no Document
            }

            var filePath = theDoc.FullName;
            if (!Violations.ContainsKey(filePath))
            {
                return;         // no violations
            }

            this.SetColor();     // set highLighter con ultime impostazioni
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                this.CreateVisuals(line, filePath);
            }
        }

        /// <summary>
        /// Setta il colore dell'evidenziatore.
        /// </summary>
        private void SetColor()
        {
            // Create the pen and brush to color the box behind the a's
            Brush myBrush = null;
            if (AlmaStyleFixPackage.Page == null)
            {
                myBrush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            }
            else
            {
                myBrush = new SolidColorBrush(Color.FromArgb(
                    Convert.ToByte(AlmaStyleFixPackage.Page.A),
                    Convert.ToByte(AlmaStyleFixPackage.Page.R),
                    Convert.ToByte(AlmaStyleFixPackage.Page.G),
                    Convert.ToByte(AlmaStyleFixPackage.Page.B)));
            }

            // myBrush.Freeze();
            Brush penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            Pen myPen = new Pen(penBrush, 0.5);
            myPen.Freeze();

            this.brush = myBrush;
            this.pen = myPen;
        }

        /// <summary>
        /// Handler dell'evento di modifica del testo.
        /// </summary>
        /// <param name="sender">
        /// Chi ha scatenato l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (violations == null)
            {
                return;
            }

            if (AlmaStyleFixPackage.Dte == null)
            {
                return;        // noDTE
            }

            var theDoc = AlmaStyleFixPackage.Dte.ActiveDocument;
            if (theDoc == null)
            {
                return;         // no Document
            }

            var filePath = theDoc.FullName;

            if (!Violations.ContainsKey(filePath))
            {
                return;         // no violations
            }

            bool isModified = false;
            foreach (var ch in e.Changes)
            {
                if (ch.LineCountDelta == 0)
                {
                    continue;
                }

                int ind = 0;
                bool found = false;
                foreach (ITextSnapshotLine l in this.view.TextSnapshot.Lines)
                {
                    if (l.Start >= e.Changes[0].OldSpan.Start)
                    {
                        found = true;
                        break;
                    }

                    ind++;
                }

                if (found)
                {
                    for (int i = 0; i < violations[filePath].Count; i++)
                    {
                        if (violations[filePath][i].LineNumber >= ind)
                        {
                            var appo = new AlmaStyleFixLib.Violation() { LineNumber = violations[filePath][i].LineNumber + ch.LineCountDelta, ErrorMessage = violations[filePath][i].ErrorMessage };
                            violations[filePath].RemoveAt(i);
                            violations[filePath].Insert(i, appo);
                            isModified = true;
                        }
                    }
                }
            }

            if (isModified)
            {
                int numOfLines = 0;
                {
                    foreach (ITextSnapshotLine l in this.view.TextSnapshot.Lines)
                    {
                        numOfLines++;
                    }
                }

                // ViewportAdornment1.ViewportAdornment1.OnSizeChange(numOfLines);
            }
        }
    }
}