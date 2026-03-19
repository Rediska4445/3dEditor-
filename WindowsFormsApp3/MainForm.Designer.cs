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
            this.groupBoxTools = new System.Windows.Forms.GroupBox();
            this.radioMoveVertex = new System.Windows.Forms.RadioButton();
            this.radioAddVertex = new System.Windows.Forms.RadioButton();
            this.radioDeleteVertex = new System.Windows.Forms.RadioButton();
            this.panelControls = new System.Windows.Forms.Panel();
            this.labelMode = new System.Windows.Forms.Label();
            this.glControl = new OpenTK.GLControl();
            this.groupBoxTools.SuspendLayout();
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
            // 
            // groupBoxTools
            // 
            this.groupBoxTools.Controls.Add(this.radioMoveVertex);
            this.groupBoxTools.Controls.Add(this.radioAddVertex);
            this.groupBoxTools.Controls.Add(this.radioDeleteVertex);
            this.groupBoxTools.Location = new System.Drawing.Point(12, 50);
            this.groupBoxTools.Name = "groupBoxTools";
            this.groupBoxTools.Size = new System.Drawing.Size(200, 100);
            this.groupBoxTools.TabIndex = 2;
            this.groupBoxTools.TabStop = false;
            this.groupBoxTools.Text = "Инструменты редактирования";
            // 
            // radioMoveVertex
            // 
            this.radioMoveVertex.Location = new System.Drawing.Point(10, 20);
            this.radioMoveVertex.Name = "radioMoveVertex";
            this.radioMoveVertex.Size = new System.Drawing.Size(104, 24);
            this.radioMoveVertex.TabIndex = 0;
            this.radioMoveVertex.Text = "Перемещение вершин";
            // 
            // radioAddVertex
            // 
            this.radioAddVertex.Location = new System.Drawing.Point(10, 45);
            this.radioAddVertex.Name = "radioAddVertex";
            this.radioAddVertex.Size = new System.Drawing.Size(104, 24);
            this.radioAddVertex.TabIndex = 1;
            this.radioAddVertex.Text = "Добавить вершину";
            // 
            // radioDeleteVertex
            // 
            this.radioDeleteVertex.Location = new System.Drawing.Point(10, 70);
            this.radioDeleteVertex.Name = "radioDeleteVertex";
            this.radioDeleteVertex.Size = new System.Drawing.Size(104, 24);
            this.radioDeleteVertex.TabIndex = 2;
            this.radioDeleteVertex.Text = "Удалить вершину";
            // 
            // panelControls
            // 
            this.panelControls.Location = new System.Drawing.Point(12, 160);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(200, 200);
            this.panelControls.TabIndex = 3;
            // 
            // labelMode
            // 
            this.labelMode.Location = new System.Drawing.Point(12, 370);
            this.labelMode.Name = "labelMode";
            this.labelMode.Size = new System.Drawing.Size(200, 20);
            this.labelMode.TabIndex = 4;
            this.labelMode.Text = "Режим: перемещение вершин";
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
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxTools);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.labelMode);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "3D Модель редактор";
            this.groupBoxTools.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBoxTools;
        private System.Windows.Forms.RadioButton radioMoveVertex;
        private System.Windows.Forms.RadioButton radioAddVertex;
        private System.Windows.Forms.RadioButton radioDeleteVertex;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Label labelMode;
        private GLControl glControl;

        #endregion
    }
}