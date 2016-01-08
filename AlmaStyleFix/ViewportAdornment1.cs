//----------------------------------------------------------------------------------------------
// <copyright file="ViewportAdornment1.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------------------------------
namespace TSF.ViewportAdornment1
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Microsoft.VisualStudio.Text.Editor;

    using TSF.AlmaStyleFix;
    using TSF.HighLight;

    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport.
    /// </summary>
    public class ViewportAdornment1
    {
        /// <summary>
        /// Layer dove inserire le immagini.
        /// </summary>
        private static IAdornmentLayer adornmentLayer;

        /// <summary>
        /// Sorgente dell'immagine.
        /// </summary>
        private static ImageSource image;

        /// <summary>
        /// Puntatore alla vista.
        /// </summary>
        private static IWpfTextView view;

        /// <summary>
        /// Inizializza una nuova istanza della classe ViewportAdornment1 Creates a square image and attaches an event handler to the layout changed event that.
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer.
        /// </summary>
        /// <param name="theView">
        /// Puntatore alla vista di Visual Studio da Factory.
        /// </param>
        public ViewportAdornment1(IWpfTextView theView)
        {
            view = theView;

            // Brush brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            // brush.Freeze();

            // Brush penBrush = new SolidColorBrush(Colors.Red);
            // penBrush.Freeze();
            // Pen pen = new Pen(penBrush, 0.5);
            // pen.Freeze();

            //// draw a square with the created brush and pen
            // System.Windows.Rect r = new System.Windows.Rect(0, 0, 5, 5);
            // Geometry g = new RectangleGeometry(r);
            // GeometryDrawing drawing = new GeometryDrawing(brush, pen, g);
            // drawing.Freeze();

            // image = new DrawingImage(drawing);
            // image.Freeze();
            SetColor();

            // Grab a reference to the adornment layer that this adornment should be added to
            adornmentLayer = view.GetAdornmentLayer("ViewportAdornment1");

            view.ViewportHeightChanged += delegate { OnSizeChange(); };
            view.ViewportWidthChanged += delegate { OnSizeChange(); };
        }

        /// <summary>
        /// Forza il ridisegno delle immagini.
        /// </summary>
        public static void ForceGo()
        {
            OnSizeChange();
        }

        /// <summary>
        /// Handler dell'evento di ridimensionamento .
        /// </summary>
        /// <param name="theNumOfLines">
        /// Il numero di righe totali del documento (opzionale).
        /// </param>
        public static void OnSizeChange(int theNumOfLines = 0)
        {
            // check for enable
            // ottimizzazione! controllo i prerequisiti
            if (!HighLighter.Enable || HighLighter.Violations == null || HighLighter.Violations.Count == 0 || view == null)
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
            if (!HighLighter.Violations.ContainsKey(filePath))
            {
                return;         // no violations
            }

            SetColor();

            var numOfLines = view.TextSnapshot.LineCount;
            if (theNumOfLines > 0)
            {
                numOfLines = theNumOfLines;
            }

            // clear the adornment layer of previous adornments
            adornmentLayer.RemoveAllAdornments();

            var viewPortRatio = Convert.ToDouble((view.ViewportBottom - 20 - (view.ViewportTop + 35)) / numOfLines);

            for (int i = 0; i < HighLighter.Violations[filePath].Count; i++)
            {
                var img = new Image();
                img.Source = image;

                var violationIndex = HighLighter.Violations[filePath][i].LineNumber;

                // Place the image in the top right hand corner of the Viewport
                Canvas.SetLeft(img, view.ViewportRight - 10);
                var position = view.ViewportTop + 35 + (viewPortRatio * violationIndex);
                Canvas.SetTop(img, position);

                // add the image to the adornment layer and make it relative to the viewport
                adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, img, null);
            }
        }

        /// <summary>
        /// Setta il colore usato dall'evidenziatore.
        /// </summary>
        private static void SetColor()
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

            // draw a square with the created brush and pen
            System.Windows.Rect r = new System.Windows.Rect(0, 0, 5, 5);
            Geometry g = new RectangleGeometry(r);
            GeometryDrawing drawing = new GeometryDrawing(myBrush, myPen, g);
            drawing.Freeze();

            image = new DrawingImage(drawing);
            image.Freeze();
        }
    }
}