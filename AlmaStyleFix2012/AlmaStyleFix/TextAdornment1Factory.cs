//----------------------------------------------------------------------------------------------
// <copyright file="TextAdornment1Factory.cs" company="Almaviva TSF" author="Andrea De Lucia">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace TSF.AdornementFactory
{
    using System.ComponentModel.Composition;

    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Utilities;

    using TSF.HighLight;
    using TSF.ViewportAdornment1;

    /// <summary>
    /// Establishes an <see cref="IAdornmentLayer"/> to place the adornment on and exports the <see cref="IWpfTextViewCreationListener"/>
    /// that instantiates the adornment on the event of a <see cref="IWpfTextView"/>'s creation.
    /// </summary>
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class TextAdornment1Factory : IWpfTextViewCreationListener
    {
        /// <summary>
        /// Defines the adornment layer for the adornment. This layer is ordered .
        /// after the selection layer in the Z-order.
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("TextAdornment1")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        private AdornmentLayerDefinition editorAdornmentLayer = null;

        /// <summary>
        /// Defines the adornment layer for the scarlet adornment. This layer is ordered .
        /// after the selection layer in the Z-order.
        /// </summary>
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("ViewportAdornment1")]
        [Order(After = PredefinedAdornmentLayers.Caret)]
        [TextViewRole(PredefinedTextViewRoles.Document)]
        private AdornmentLayerDefinition editorAdornmentLayer2 = null;

        /// <summary>
        /// Recupera o imposta il layer di editor.
        /// </summary>
        public AdornmentLayerDefinition EditorAdornmentLayer
        {
            get { return this.editorAdornmentLayer; }
            set { this.editorAdornmentLayer = value; }
        }

        /// <summary>
        /// Recupera o imposta il layer di viewPort.
        /// </summary>
        public AdornmentLayerDefinition EditorAdornmentLayer2
        {
            get { return this.editorAdornmentLayer2; }
            set { this.editorAdornmentLayer2 = value; }
        }

        /// <summary>
        /// Instantiates a TextAdornment1 manager when a textView is created.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment should be placed.</param>
        public void TextViewCreated(IWpfTextView textView)
        {
            new HighLighter(textView);

            // new ViewportAdornment1(textView);
        }
    }
}