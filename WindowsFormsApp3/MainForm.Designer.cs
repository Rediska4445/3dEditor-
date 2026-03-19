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
            this.glControl = new OpenTK.WinForms.GLControl();

            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(12, 12);
            this.btnLoad.Size = new System.Drawing.Size(100, 30);
            this.btnLoad.Text = "Загрузить OBJ";
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);

            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(120, 12);
            this.btnSave.Size = new System.Drawing.Size(100, 30);
            this.btnSave.Text = "Сохранить OBJ";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // 
            // groupBoxTools
            // 
            this.groupBoxTools.Location = new System.Drawing.Point(12, 50);
            this.groupBoxTools.Size = new System.Drawing.Size(200, 100);
            this.groupBoxTools.Text = "Инструменты редактирования";

            // 
            // radioMoveVertex
            // 
            this.radioMoveVertex.Location = new System.Drawing.Point(10, 20);
            this.radioMoveVertex.Text = "Перемещение вершин";
            this.radioMoveVertex.CheckedChanged += new System.EventHandler(this.RadioMoveVertex_CheckedChanged);
            this.groupBoxTools.Controls.Add(this.radioMoveVertex);

            // 
            // radioAddVertex
            // 
            this.radioAddVertex.Location = new System.Drawing.Point(10, 45);
            this.radioAddVertex.Text = "Добавить вершину";
            this.groupBoxTools.Controls.Add(this.radioAddVertex);

            // 
            // radioDeleteVertex
            // 
            this.radioDeleteVertex.Location = new System.Drawing.Point(10, 70);
            this.radioDeleteVertex.Text = "Удалить вершину";
            this.groupBoxTools.Controls.Add(this.radioDeleteVertex);

            // 
            // panelControls
            // 
            this.panelControls.Location = new System.Drawing.Point(12, 160);
            this.panelControls.Size = new System.Drawing.Size(200, 200);

            // 
            // labelMode
            // 
            this.labelMode.Location = new System.Drawing.Point(12, 370);
            this.labelMode.Size = new System.Drawing.Size(300, 20);
            this.labelMode.Text = "Режим: перемещение вершин";

            // 
            // glControl
            // 
            this.glControl.Location = new System.Drawing.Point(230, 12);
            this.glControl.Size = new System.Drawing.Size(560, 400);
            this.glControl.BackColor = System.Drawing.Color.Black;
            // Инициализация происходит в коде или через дизайнер

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
            this.Text = "3D Модель редактор";

        }

        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox groupBoxTools;
        private System.Windows.Forms.RadioButton radioMoveVertex;
        private System.Windows.Forms.RadioButton radioAddVertex;
        private System.Windows.Forms.RadioButton radioDeleteVertex;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Label labelMode;
        private OpenTK.WinForms.GLControl glControl;

        #endregion
    }
}