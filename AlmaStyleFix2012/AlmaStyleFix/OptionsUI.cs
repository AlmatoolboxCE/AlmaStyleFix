//----------------------------------------------------------------------------------------------
// <copyright file="OptionsUI.cs" company="Almaviva TSF">
// Copyright (c) Almaviva TSF.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace TSF.AlmaStyleFix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Windows.Media;

    /// <summary>
    /// Struttura della form da usare nelle option page.
    /// </summary>
    public class ToolOptionsUI : UserControl
    {
        /// <summary>
        /// Checkbox collegata alla prima opzione.
        /// </summary>
        private CheckBox checkBox1;

        /// <summary>
        /// Checkbox collegata alla seconda opzione.
        /// </summary>
        private CheckBox checkBox2;

        /// <summary>
        /// Checkbox collegata alla terza opzione.
        /// </summary>
        private CheckBox checkBox3;

        /// <summary>
        /// Checkbox collegata all'uso di NArrange
        /// </summary>
        private CheckBox checkBox5;

        /// <summary>
        /// Grupbox che contiene la gestione del colore.
        /// </summary>
        private GroupBox groupBox1;

        /// <summary>
        /// Etichetta del primo campo testo.
        /// </summary>
        private Label label1;

        /// <summary>
        /// Etichetta del secondo campo testo.
        /// </summary>
        private Label label2;

        /// <summary>
        /// Etichetta dei colori.
        /// </summary>
        private Label label3;

        /// <summary>
        /// Etichetta dei colori.
        /// </summary>
        private Label label4;

        /// <summary>
        /// Etichetta dei colori.
        /// </summary>
        private Label label5;

        /// <summary>
        /// Etichetta dei colori.
        /// </summary>
        private Label label6;

        /// <summary>
        /// Anteprima del colore.
        /// </summary>
        private PictureBox pictureBox1;

        /// <summary>
        /// Contiene il nome della società.
        /// </summary>
        private TextBox textBox1;

        /// <summary>
        /// Contiene il nome dell'autore.
        /// </summary>
        private TextBox textBox2;

        /// <summary>
        /// Variabile che contiene le opzioni.
        /// </summary>
        private ToolsOptions theOptions;

        /// <summary>
        /// Quantità di rosso.
        /// </summary>
        private TrackBar trackBar1;

        /// <summary>
        /// Quantità di verde.
        /// </summary>
        private TrackBar trackBar2;

        /// <summary>
        /// Quantità di blu.
        /// </summary>
        private TrackBar trackBar3;

        /// <summary>
        /// Livello di trasparenza.
        /// </summary>
        private TrackBar trackBar4;

        /// <summary>
        /// Inizializza una nuova istanza della classe ToolOptionsUI.
        /// </summary>
        public ToolOptionsUI()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Recupera o imposta il valore della classe di opzioni collegata.
        /// </summary>
        internal ToolsOptions TheOptions
        {
            get { return this.theOptions; }
            set { this.theOptions = value; }
        }

        /// <summary>
        /// Inizializza il contenuto delle checkbox in base ai valori della classe collegata.
        /// </summary>
        public void Initialize()
        {
            this.checkBox1.Checked = this.TheOptions.UseVersioning;
            this.checkBox2.Checked = this.TheOptions.OnFixSave;
            this.checkBox3.Checked = this.TheOptions.UseAdornment;
            this.checkBox5.Checked = this.theOptions.UseNarrange;

            // this.checkBox4.Checked = this.theOptions.ForceFixingOnCompileError;

            // this.checkBox4.Checked = this.theOptions.CheckForUpdates;
            this.textBox1.Text = this.theOptions.Company;
            this.textBox2.Text = this.theOptions.Author;
            this.trackBar1.Value = this.theOptions.R;
            this.trackBar2.Value = this.theOptions.G;
            this.trackBar3.Value = this.theOptions.B;
            this.trackBar4.Value = this.theOptions.A;
            this.GeneratePicture(this.trackBar1.Value, this.trackBar2.Value, this.trackBar3.Value, this.trackBar4.Value);
        }

        /// <summary>
        /// Handler lanciato se il contenuto di checkbox1 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.TheOptions.UseVersioning = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Handler lanciato se il contenuto di checkbox2 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.TheOptions.OnFixSave = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Handler lanciato se il contenuto di checkbox3 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.TheOptions.UseAdornment = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Handler lanciato se il contenuto di checkbox2 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            this.theOptions.ForceFixingOnCompileError = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Handler lanciato se il contenuto di checkbox5 cambia.
        /// </summary>
        /// <param name="sender">
        ///  Chi esegue la chiamata.
        /// </param>
        /// <param name="e">
        ///  Parametri della chiamata.
        /// </param>
        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            this.theOptions.UseNarrange = ((CheckBox)sender).Checked;
        }

        /// <summary>
        /// Genera l'anteprima di colore.
        /// </summary>
        /// <param name="r">
        /// Quantità di rosso.
        /// </param>
        /// <param name="g">
        /// Quantità di verde.
        /// </param>
        /// <param name="b">
        /// Quantità di blu.
        /// </param>
        /// <param name="a">
        /// Livello di trasparenza.
        /// </param>
        private void GeneratePicture(int r, int g, int b, int a)
        {
            // var c = Color.FromArgb(Convert.ToByte(t), Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
            var bmp = new System.Drawing.Bitmap(10, 10);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    bmp.SetPixel(i, j, System.Drawing.Color.FromArgb(a, r, g, b));
                }
            }

            this.pictureBox1.Image = bmp;
        }

        /// <summary>
        /// Inizializza le componenti grafiche della form.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolOptionsUI));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();

            // checkBox1
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);

            // checkBox2
            resources.ApplyResources(this.checkBox2, "checkBox2");
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);

            // checkBox3
            resources.ApplyResources(this.checkBox3, "checkBox3");
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);

            // label1
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";

            // textBox1
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.TextBox1_TextChanged);

            // textBox2
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.Name = "textBox2";
            this.textBox2.TextChanged += new System.EventHandler(this.TextBox2_TextChanged);

            // label2
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";

            // groupBox1
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Controls.Add(this.trackBar4);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.trackBar3);
            this.groupBox1.Controls.Add(this.trackBar2);
            this.groupBox1.Controls.Add(this.trackBar1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;

            // pictureBox1
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;

            // trackBar4
            resources.ApplyResources(this.trackBar4, "trackBar4");
            this.trackBar4.LargeChange = 50;
            this.trackBar4.Maximum = 255;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar4.Value = 137;
            this.trackBar4.Scroll += new System.EventHandler(this.TrackBar_Scroll);

            // label6
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";

            // trackBar3
            resources.ApplyResources(this.trackBar3, "trackBar3");
            this.trackBar3.LargeChange = 50;
            this.trackBar3.Maximum = 255;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.Value = 137;
            this.trackBar3.Scroll += new System.EventHandler(this.TrackBar_Scroll);

            // trackBar2
            resources.ApplyResources(this.trackBar2, "trackBar2");
            this.trackBar2.LargeChange = 50;
            this.trackBar2.Maximum = 255;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Value = 137;
            this.trackBar2.Scroll += new System.EventHandler(this.TrackBar_Scroll);

            // trackBar1
            this.trackBar1.LargeChange = 50;
            resources.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Value = 137;
            this.trackBar1.Scroll += new System.EventHandler(this.TrackBar_Scroll);

            // label5
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";

            // label4
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";

            // label3
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";

            // checkBox5
            resources.ApplyResources(this.checkBox5, "checkBox5");
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.UseVisualStyleBackColor = true;
            this.checkBox5.CheckedChanged += new System.EventHandler(this.CheckBox5_CheckedChanged);

            // ToolOptionsUI
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Name = "ToolOptionsUI";
            resources.ApplyResources(this, "$this");
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Handler lanciato se il contenuto di textbox1 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            this.theOptions.Company = this.textBox1.Text;
        }

        /// <summary>
        /// Handler lanciato se il contenuto di textbox2 cambia.
        /// </summary>
        /// <param name="sender">
        /// Chi lancia l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            this.theOptions.Author = this.textBox2.Text;
        }

        /// <summary>
        /// Handler dell'evento di modifica del valore sella scrollBar.
        /// </summary>
        /// <param name="sender">
        /// Chi genera l'evento.
        /// </param>
        /// <param name="e">
        /// Parametri dell'evento.
        /// </param>
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            this.theOptions.R = this.trackBar1.Value;
            this.theOptions.G = this.trackBar2.Value;
            this.theOptions.B = this.trackBar3.Value;
            this.theOptions.A = this.trackBar4.Maximum - this.trackBar4.Value;
            this.GeneratePicture(this.trackBar1.Value, this.trackBar2.Value, this.trackBar3.Value, this.trackBar4.Maximum - this.trackBar4.Value);
        }
    }
}