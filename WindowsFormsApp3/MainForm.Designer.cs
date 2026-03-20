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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.glControl = new OpenTK.GLControl();
            this.checkBoxModeEdit = new System.Windows.Forms.CheckBox();
            this.groupBoxTools = new System.Windows.Forms.GroupBox();
            this.checkBoxShowEdges = new System.Windows.Forms.CheckBox();
            this.checkBoxShowVertices = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxAdd = new System.Windows.Forms.CheckBox();
            this.checkBoxRemove = new System.Windows.Forms.CheckBox();
            this.checkBoxShowGrid = new System.Windows.Forms.CheckBox();
            this.groupBoxTools.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(12, 408);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(100, 30);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Загрузить OBJ";
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(118, 408);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 30);
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
            this.glControl.Location = new System.Drawing.Point(228, 28);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(560, 410);
            this.glControl.TabIndex = 5;
            this.glControl.VSync = false;
            // 
            // checkBoxModeEdit
            // 
            this.checkBoxModeEdit.AutoSize = true;
            this.checkBoxModeEdit.Location = new System.Drawing.Point(6, 19);
            this.checkBoxModeEdit.Name = "checkBoxModeEdit";
            this.checkBoxModeEdit.Size = new System.Drawing.Size(147, 17);
            this.checkBoxModeEdit.TabIndex = 0;
            this.checkBoxModeEdit.Text = "Режим редактирования";
            this.checkBoxModeEdit.UseVisualStyleBackColor = true;
            // 
            // groupBoxTools
            // 
            this.groupBoxTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxTools.Controls.Add(this.checkBoxShowGrid);
            this.groupBoxTools.Controls.Add(this.checkBoxRemove);
            this.groupBoxTools.Controls.Add(this.checkBoxAdd);
            this.groupBoxTools.Controls.Add(this.checkBoxShowVertices);
            this.groupBoxTools.Controls.Add(this.checkBoxModeEdit);
            this.groupBoxTools.Controls.Add(this.checkBoxShowEdges);
            this.groupBoxTools.Location = new System.Drawing.Point(12, 28);
            this.groupBoxTools.Name = "groupBoxTools";
            this.groupBoxTools.Size = new System.Drawing.Size(200, 371);
            this.groupBoxTools.TabIndex = 2;
            this.groupBoxTools.TabStop = false;
            this.groupBoxTools.Text = "Инструменты редактирования";
            // 
            // checkBoxShowEdges
            // 
            this.checkBoxShowEdges.AutoSize = true;
            this.checkBoxShowEdges.Location = new System.Drawing.Point(6, 109);
            this.checkBoxShowEdges.Name = "checkBoxShowEdges";
            this.checkBoxShowEdges.Size = new System.Drawing.Size(120, 17);
            this.checkBoxShowEdges.TabIndex = 6;
            this.checkBoxShowEdges.Text = "Отображать грани";
            this.checkBoxShowEdges.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowVertices
            // 
            this.checkBoxShowVertices.AutoSize = true;
            this.checkBoxShowVertices.Location = new System.Drawing.Point(6, 132);
            this.checkBoxShowVertices.Name = "checkBoxShowVertices";
            this.checkBoxShowVertices.Size = new System.Drawing.Size(137, 17);
            this.checkBoxShowVertices.TabIndex = 7;
            this.checkBoxShowVertices.Text = "Отображать вершины";
            this.checkBoxShowVertices.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Enabled = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButtonView,
            this.toolStripButton3,
            this.toolStripSeparator3,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(57, 22);
            this.toolStripButton4.Text = "Справка";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton4_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemImport,
            this.toolStripMenuItemExport,
            this.toolStripMenuItemSettings,
            this.toolStripMenuItemExit});
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(49, 22);
            this.toolStripButton1.Text = "Файл";
            this.toolStripButton1.ToolTipText = "Файл";
            // 
            // toolStripMenuItemExport
            // 
            this.toolStripMenuItemExport.Name = "toolStripMenuItemExport";
            this.toolStripMenuItemExport.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemExport.Text = "Экспорт";
            // 
            // toolStripMenuItemSettings
            // 
            this.toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            this.toolStripMenuItemSettings.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemSettings.Text = "Настройки";
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemExit.Text = "Выход";
            // 
            // toolStripButtonView
            // 
            this.toolStripButtonView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2});
            this.toolStripButtonView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonView.Image")));
            this.toolStripButtonView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonView.Name = "toolStripButtonView";
            this.toolStripButtonView.Size = new System.Drawing.Size(40, 22);
            this.toolStripButtonView.Text = "Вид";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Checked = true;
            this.toolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(293, 22);
            this.toolStripMenuItem1.Text = "Отображать кнопки работы с файлом";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Checked = true;
            this.toolStripMenuItem2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(293, 22);
            this.toolStripMenuItem2.Text = "Отображать чек боксы редактирования";
            // 
            // toolStripMenuItemImport
            // 
            this.toolStripMenuItemImport.Name = "toolStripMenuItemImport";
            this.toolStripMenuItemImport.Size = new System.Drawing.Size(180, 22);
            this.toolStripMenuItemImport.Text = "Импорт";
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(96, 22);
            this.toolStripButton3.Text = "Инструменты";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(204, 22);
            this.toolStripMenuItem3.Text = "Режим редактирования";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(204, 22);
            this.toolStripMenuItem4.Text = "Отображать грани";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(204, 22);
            this.toolStripMenuItem5.Text = "Отображать вершины";
            // 
            // checkBoxAdd
            // 
            this.checkBoxAdd.AutoSize = true;
            this.checkBoxAdd.Location = new System.Drawing.Point(6, 42);
            this.checkBoxAdd.Name = "checkBoxAdd";
            this.checkBoxAdd.Size = new System.Drawing.Size(124, 17);
            this.checkBoxAdd.TabIndex = 8;
            this.checkBoxAdd.Text = "Режим добавления";
            this.checkBoxAdd.UseVisualStyleBackColor = true;
            // 
            // checkBoxRemove
            // 
            this.checkBoxRemove.AutoSize = true;
            this.checkBoxRemove.Location = new System.Drawing.Point(6, 65);
            this.checkBoxRemove.Name = "checkBoxRemove";
            this.checkBoxRemove.Size = new System.Drawing.Size(111, 17);
            this.checkBoxRemove.TabIndex = 9;
            this.checkBoxRemove.Text = "Режим удаления";
            this.checkBoxRemove.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowGrid
            // 
            this.checkBoxShowGrid.AutoSize = true;
            this.checkBoxShowGrid.Location = new System.Drawing.Point(6, 348);
            this.checkBoxShowGrid.Name = "checkBoxShowGrid";
            this.checkBoxShowGrid.Size = new System.Drawing.Size(159, 17);
            this.checkBoxShowGrid.TabIndex = 10;
            this.checkBoxShowGrid.Text = "Отображать осевую сетку";
            this.checkBoxShowGrid.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.groupBoxTools);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "3D Модель редактор";
            this.groupBoxTools.ResumeLayout(false);
            this.groupBoxTools.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
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
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExport;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSettings;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButtonView;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImport;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.CheckBox checkBoxRemove;
        private System.Windows.Forms.CheckBox checkBoxAdd;
        private System.Windows.Forms.CheckBox checkBoxShowGrid;
    }
}