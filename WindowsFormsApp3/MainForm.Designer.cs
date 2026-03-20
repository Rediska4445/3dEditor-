using OpenTK;

namespace WindowsFormsApp3
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.glControl = new OpenTK.GLControl();
            this.checkBoxModeEdit = new System.Windows.Forms.CheckBox();
            this.groupBoxTools = new System.Windows.Forms.GroupBox();
            this.checkBoxShowEdges = new System.Windows.Forms.CheckBox();
            this.checkBoxShowVertices = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 12);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(100, 30);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Загрузить OBJ";
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(120, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Сохранить OBJ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // glControl
            // 
            this.glControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Location = new System.Drawing.Point(228, 12);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(560, 426);
            this.glControl.TabIndex = 5;
            this.glControl.VSync = false;
            // 
            // checkBoxModeEdit
            // 
            this.checkBoxModeEdit.AutoSize = true;
            this.checkBoxModeEdit.Location = new System.Drawing.Point(12, 156);
            this.checkBoxModeEdit.Name = "checkBoxModeEdit";
            this.checkBoxModeEdit.Size = new System.Drawing.Size(147, 17);
            this.checkBoxModeEdit.TabIndex = 0;
            this.checkBoxModeEdit.Text = "Режим редактирования";
            this.checkBoxModeEdit.UseVisualStyleBackColor = true;
            // 
            // groupBoxTools
            // 
            this.groupBoxTools.Location = new System.Drawing.Point(12, 50);
            this.groupBoxTools.Name = "groupBoxTools";
            this.groupBoxTools.Size = new System.Drawing.Size(200, 100);
            this.groupBoxTools.TabIndex = 2;
            this.groupBoxTools.TabStop = false;
            this.groupBoxTools.Text = "Инструменты редактирования";
            // 
            // checkBoxShowEdges
            // 
            this.checkBoxShowEdges.AutoSize = true;
            this.checkBoxShowEdges.Location = new System.Drawing.Point(12, 179);
            this.checkBoxShowEdges.Name = "checkBoxShowEdges";
            this.checkBoxShowEdges.Size = new System.Drawing.Size(120, 17);
            this.checkBoxShowEdges.TabIndex = 6;
            this.checkBoxShowEdges.Text = "Отображать грани";
            this.checkBoxShowEdges.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowVertices
            // 
            this.checkBoxShowVertices.AutoSize = true;
            this.checkBoxShowVertices.Location = new System.Drawing.Point(12, 202);
            this.checkBoxShowVertices.Name = "checkBoxShowVertices";
            this.checkBoxShowVertices.Size = new System.Drawing.Size(137, 17);
            this.checkBoxShowVertices.TabIndex = 7;
            this.checkBoxShowVertices.Text = "Отображать вершины";
            this.checkBoxShowVertices.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBoxShowVertices);
            this.Controls.Add(this.checkBoxShowEdges);
            this.Controls.Add(this.checkBoxModeEdit);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxTools);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "3D Модель редактор";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private GLControl glControl;

        #endregion

        private System.Windows.Forms.CheckBox checkBoxModeEdit;
        private System.Windows.Forms.GroupBox groupBoxTools;
        private System.Windows.Forms.CheckBox checkBoxShowEdges;
        private System.Windows.Forms.CheckBox checkBoxShowVertices;
    }
}