using System;
using System.Windows.Forms;

namespace WindowsFormsApp3.Forms
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private CheckBox chkShowGrid;
        private CheckBox chkShowFaces;
        private CheckBox chkShowVertices;
        private CheckBox chkShowBasis;
        private CheckBox chkUseGizmo;
        private TrackBar trackSensitivity;
        private NumericUpDown numLightingLevel;
        private Label lblSensitivity;
        private Label lblLightingLevel;
        private Button btnSave;
        private Button btnCancel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkShowGrid = new System.Windows.Forms.CheckBox();
            this.chkShowFaces = new System.Windows.Forms.CheckBox();
            this.chkShowVertices = new System.Windows.Forms.CheckBox();
            this.chkShowBasis = new System.Windows.Forms.CheckBox();
            this.chkUseGizmo = new System.Windows.Forms.CheckBox();
            this.trackSensitivity = new System.Windows.Forms.TrackBar();
            this.numLightingLevel = new System.Windows.Forms.NumericUpDown();
            this.lblSensitivity = new System.Windows.Forms.Label();
            this.lblLightingLevel = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackSensitivity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLightingLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // chkShowGrid
            // 
            this.chkShowGrid.AutoSize = true;
            this.chkShowGrid.Location = new System.Drawing.Point(20, 20);
            this.chkShowGrid.Name = "chkShowGrid";
            this.chkShowGrid.Size = new System.Drawing.Size(211, 17);
            this.chkShowGrid.TabIndex = 0;
            this.chkShowGrid.Text = "Отображать ориентировочную сетку";
            this.chkShowGrid.CheckedChanged += new System.EventHandler(this.chkShowGrid_CheckedChanged);
            // 
            // chkShowFaces
            // 
            this.chkShowFaces.AutoSize = true;
            this.chkShowFaces.Location = new System.Drawing.Point(20, 50);
            this.chkShowFaces.Name = "chkShowFaces";
            this.chkShowFaces.Size = new System.Drawing.Size(120, 17);
            this.chkShowFaces.TabIndex = 1;
            this.chkShowFaces.Text = "Отображать грани";
            this.chkShowFaces.CheckedChanged += new System.EventHandler(this.chkShowFaces_CheckedChanged);
            // 
            // chkShowVertices
            // 
            this.chkShowVertices.AutoSize = true;
            this.chkShowVertices.Location = new System.Drawing.Point(20, 80);
            this.chkShowVertices.Name = "chkShowVertices";
            this.chkShowVertices.Size = new System.Drawing.Size(137, 17);
            this.chkShowVertices.TabIndex = 2;
            this.chkShowVertices.Text = "Отображать вершины";
            this.chkShowVertices.CheckedChanged += new System.EventHandler(this.chkShowVertices_CheckedChanged);
            // 
            // chkShowBasis
            // 
            this.chkShowBasis.AutoSize = true;
            this.chkShowBasis.Location = new System.Drawing.Point(20, 110);
            this.chkShowBasis.Name = "chkShowBasis";
            this.chkShowBasis.Size = new System.Drawing.Size(121, 17);
            this.chkShowBasis.TabIndex = 3;
            this.chkShowBasis.Text = "Отображать базис";
            this.chkShowBasis.CheckedChanged += new System.EventHandler(this.chkShowBasis_CheckedChanged);
            // 
            // chkUseGizmo
            // 
            this.chkUseGizmo.AutoSize = true;
            this.chkUseGizmo.Location = new System.Drawing.Point(20, 140);
            this.chkUseGizmo.Name = "chkUseGizmo";
            this.chkUseGizmo.Size = new System.Drawing.Size(281, 17);
            this.chkUseGizmo.TabIndex = 4;
            this.chkUseGizmo.Text = "Использовать гизмо при редактировании вершин";
            this.chkUseGizmo.CheckedChanged += new System.EventHandler(this.chkUseGizmo_CheckedChanged);
            // 
            // trackSensitivity
            // 
            this.trackSensitivity.Location = new System.Drawing.Point(180, 170);
            this.trackSensitivity.Maximum = 100;
            this.trackSensitivity.Minimum = 1;
            this.trackSensitivity.Name = "trackSensitivity";
            this.trackSensitivity.Size = new System.Drawing.Size(150, 45);
            this.trackSensitivity.TabIndex = 6;
            this.trackSensitivity.TickFrequency = 10;
            this.trackSensitivity.Value = 50;
            this.trackSensitivity.Scroll += new System.EventHandler(this.trackSensitivity_Scroll);
            // 
            // numLightingLevel
            // 
            this.numLightingLevel.Location = new System.Drawing.Point(180, 215);
            this.numLightingLevel.Name = "numLightingLevel";
            this.numLightingLevel.Size = new System.Drawing.Size(60, 20);
            this.numLightingLevel.TabIndex = 8;
            this.numLightingLevel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numLightingLevel.ValueChanged += new System.EventHandler(this.numLightingLevel_ValueChanged);
            // 
            // lblSensitivity
            // 
            this.lblSensitivity.AutoSize = true;
            this.lblSensitivity.Location = new System.Drawing.Point(20, 180);
            this.lblSensitivity.Name = "lblSensitivity";
            this.lblSensitivity.Size = new System.Drawing.Size(134, 13);
            this.lblSensitivity.TabIndex = 5;
            this.lblSensitivity.Text = "Чувствительность мыши";
            // 
            // lblLightingLevel
            // 
            this.lblLightingLevel.AutoSize = true;
            this.lblLightingLevel.Location = new System.Drawing.Point(20, 220);
            this.lblLightingLevel.Name = "lblLightingLevel";
            this.lblLightingLevel.Size = new System.Drawing.Size(111, 13);
            this.lblLightingLevel.TabIndex = 7;
            this.lblLightingLevel.Text = "Уровень освещения";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(220, 280);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "Сохранить";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(93, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.chkShowGrid);
            this.Controls.Add(this.chkShowFaces);
            this.Controls.Add(this.chkShowVertices);
            this.Controls.Add(this.chkShowBasis);
            this.Controls.Add(this.chkUseGizmo);
            this.Controls.Add(this.lblSensitivity);
            this.Controls.Add(this.trackSensitivity);
            this.Controls.Add(this.lblLightingLevel);
            this.Controls.Add(this.numLightingLevel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Name = "SettingsForm";
            this.Text = "Настройки";
            ((System.ComponentModel.ISupportInitialize)(this.trackSensitivity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLightingLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
