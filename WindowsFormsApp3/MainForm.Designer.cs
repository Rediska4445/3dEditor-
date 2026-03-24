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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericVertexZ = new System.Windows.Forms.NumericUpDown();
            this.numericVertexY = new System.Windows.Forms.NumericUpDown();
            this.numericVertexX = new System.Windows.Forms.NumericUpDown();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxRemove = new System.Windows.Forms.CheckBox();
            this.checkBoxAdd = new System.Windows.Forms.CheckBox();
            this.checkBoxShowGrid = new System.Windows.Forms.CheckBox();
            this.checkBoxShowVertices = new System.Windows.Forms.CheckBox();
            this.checkBoxShowEdges = new System.Windows.Forms.CheckBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemImport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonView = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.groupBoxView = new System.Windows.Forms.GroupBox();
            this.groupBoxWorld = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelLoadSaveButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.отключитьГлубинуДляВершинToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexX)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBoxView.SuspendLayout();
            this.groupBoxWorld.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanelLoadSaveButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoad.Location = new System.Drawing.Point(3, 3);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(198, 30);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Загрузить OBJ";
            this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(3, 39);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(198, 30);
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
            this.glControl.Size = new System.Drawing.Size(580, 467);
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
            this.groupBoxTools.Controls.Add(this.label3);
            this.groupBoxTools.Controls.Add(this.label2);
            this.groupBoxTools.Controls.Add(this.label1);
            this.groupBoxTools.Controls.Add(this.numericVertexZ);
            this.groupBoxTools.Controls.Add(this.numericVertexY);
            this.groupBoxTools.Controls.Add(this.numericVertexX);
            this.groupBoxTools.Controls.Add(this.listBox1);
            this.groupBoxTools.Controls.Add(this.button1);
            this.groupBoxTools.Controls.Add(this.checkBoxRemove);
            this.groupBoxTools.Controls.Add(this.checkBoxAdd);
            this.groupBoxTools.Controls.Add(this.checkBoxModeEdit);
            this.groupBoxTools.Location = new System.Drawing.Point(3, 3);
            this.groupBoxTools.Name = "groupBoxTools";
            this.groupBoxTools.Size = new System.Drawing.Size(198, 249);
            this.groupBoxTools.TabIndex = 2;
            this.groupBoxTools.TabStop = false;
            this.groupBoxTools.Text = "Инструменты редактирования";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 220);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Z";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 194);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Y";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 168);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "X";
            // 
            // numericVertexZ
            // 
            this.numericVertexZ.DecimalPlaces = 3;
            this.numericVertexZ.Location = new System.Drawing.Point(23, 218);
            this.numericVertexZ.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericVertexZ.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericVertexZ.Name = "numericVertexZ";
            this.numericVertexZ.Size = new System.Drawing.Size(169, 20);
            this.numericVertexZ.TabIndex = 15;
            this.numericVertexZ.ValueChanged += new System.EventHandler(this.numericVertexZ_ValueChanged);
            // 
            // numericVertexY
            // 
            this.numericVertexY.DecimalPlaces = 3;
            this.numericVertexY.Location = new System.Drawing.Point(23, 192);
            this.numericVertexY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericVertexY.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericVertexY.Name = "numericVertexY";
            this.numericVertexY.Size = new System.Drawing.Size(169, 20);
            this.numericVertexY.TabIndex = 14;
            this.numericVertexY.ValueChanged += new System.EventHandler(this.numericVertexY_ValueChanged);
            // 
            // numericVertexX
            // 
            this.numericVertexX.DecimalPlaces = 3;
            this.numericVertexX.Location = new System.Drawing.Point(23, 166);
            this.numericVertexX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericVertexX.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numericVertexX.Name = "numericVertexX";
            this.numericVertexX.Size = new System.Drawing.Size(169, 20);
            this.numericVertexX.TabIndex = 13;
            this.numericVertexX.ValueChanged += new System.EventHandler(this.numericVertexX_ValueChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(6, 117);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(186, 43);
            this.listBox1.TabIndex = 12;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 88);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(186, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Цвет объекта";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            // checkBoxShowGrid
            // 
            this.checkBoxShowGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxShowGrid.AutoSize = true;
            this.checkBoxShowGrid.Location = new System.Drawing.Point(6, 24);
            this.checkBoxShowGrid.Name = "checkBoxShowGrid";
            this.checkBoxShowGrid.Size = new System.Drawing.Size(159, 17);
            this.checkBoxShowGrid.TabIndex = 10;
            this.checkBoxShowGrid.Text = "Отображать осевую сетку";
            this.checkBoxShowGrid.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowVertices
            // 
            this.checkBoxShowVertices.AutoSize = true;
            this.checkBoxShowVertices.Location = new System.Drawing.Point(6, 42);
            this.checkBoxShowVertices.Name = "checkBoxShowVertices";
            this.checkBoxShowVertices.Size = new System.Drawing.Size(137, 17);
            this.checkBoxShowVertices.TabIndex = 7;
            this.checkBoxShowVertices.Text = "Отображать вершины";
            this.checkBoxShowVertices.UseVisualStyleBackColor = true;
            this.checkBoxShowVertices.CheckedChanged += new System.EventHandler(this.checkBoxShowVertices_CheckedChanged);
            // 
            // checkBoxShowEdges
            // 
            this.checkBoxShowEdges.AutoSize = true;
            this.checkBoxShowEdges.Location = new System.Drawing.Point(6, 19);
            this.checkBoxShowEdges.Name = "checkBoxShowEdges";
            this.checkBoxShowEdges.Size = new System.Drawing.Size(120, 17);
            this.checkBoxShowEdges.TabIndex = 6;
            this.checkBoxShowEdges.Text = "Отображать грани";
            this.checkBoxShowEdges.UseVisualStyleBackColor = true;
            this.checkBoxShowEdges.CheckedChanged += new System.EventHandler(this.checkBoxShowEdges_CheckedChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButtonView,
            this.toolStripButton3,
            this.toolStripSeparator3,
            this.toolStripButton4});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(820, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
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
            // toolStripMenuItemImport
            // 
            this.toolStripMenuItemImport.Name = "toolStripMenuItemImport";
            this.toolStripMenuItemImport.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItemImport.Text = "Импорт";
            this.toolStripMenuItemImport.Click += new System.EventHandler(this.toolStripMenuItemImport_Click);
            // 
            // toolStripMenuItemExport
            // 
            this.toolStripMenuItemExport.Name = "toolStripMenuItemExport";
            this.toolStripMenuItemExport.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItemExport.Text = "Экспорт";
            this.toolStripMenuItemExport.Click += new System.EventHandler(this.toolStripMenuItemExport_Click);
            // 
            // toolStripMenuItemSettings
            // 
            this.toolStripMenuItemSettings.Enabled = false;
            this.toolStripMenuItemSettings.Name = "toolStripMenuItemSettings";
            this.toolStripMenuItemSettings.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItemSettings.Text = "Настройки";
            this.toolStripMenuItemSettings.Visible = false;
            this.toolStripMenuItemSettings.Click += new System.EventHandler(this.toolStripMenuItemSettings_Click);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(134, 22);
            this.toolStripMenuItemExit.Text = "Выход";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            // 
            // toolStripButtonView
            // 
            this.toolStripButtonView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.отключитьГлубинуДляВершинToolStripMenuItem});
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
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Checked = true;
            this.toolStripMenuItem2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(293, 22);
            this.toolStripMenuItem2.Text = "Отображать чек боксы редактирования";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
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
            // groupBoxView
            // 
            this.groupBoxView.Controls.Add(this.checkBoxShowEdges);
            this.groupBoxView.Controls.Add(this.checkBoxShowVertices);
            this.groupBoxView.Location = new System.Drawing.Point(3, 258);
            this.groupBoxView.Name = "groupBoxView";
            this.groupBoxView.Size = new System.Drawing.Size(198, 69);
            this.groupBoxView.TabIndex = 10;
            this.groupBoxView.TabStop = false;
            this.groupBoxView.Text = "Просмотр";
            // 
            // groupBoxWorld
            // 
            this.groupBoxWorld.Controls.Add(this.checkBoxShowGrid);
            this.groupBoxWorld.Location = new System.Drawing.Point(3, 333);
            this.groupBoxWorld.Name = "groupBoxWorld";
            this.groupBoxWorld.Size = new System.Drawing.Size(198, 47);
            this.groupBoxWorld.TabIndex = 11;
            this.groupBoxWorld.TabStop = false;
            this.groupBoxWorld.Text = "Мир";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.groupBoxTools);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxView);
            this.flowLayoutPanel1.Controls.Add(this.groupBoxWorld);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 28);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(210, 384);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // flowLayoutPanelLoadSaveButtons
            // 
            this.flowLayoutPanelLoadSaveButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanelLoadSaveButtons.Controls.Add(this.btnLoad);
            this.flowLayoutPanelLoadSaveButtons.Controls.Add(this.btnSave);
            this.flowLayoutPanelLoadSaveButtons.Location = new System.Drawing.Point(12, 418);
            this.flowLayoutPanelLoadSaveButtons.Name = "flowLayoutPanelLoadSaveButtons";
            this.flowLayoutPanelLoadSaveButtons.Size = new System.Drawing.Size(210, 77);
            this.flowLayoutPanelLoadSaveButtons.TabIndex = 13;
            // 
            // отключитьГлубинуДляВершинToolStripMenuItem
            // 
            this.отключитьГлубинуДляВершинToolStripMenuItem.Name = "отключитьГлубинуДляВершинToolStripMenuItem";
            this.отключитьГлубинуДляВершинToolStripMenuItem.Size = new System.Drawing.Size(293, 22);
            this.отключитьГлубинуДляВершинToolStripMenuItem.Text = "Отключить глубину для вершин";
            this.отключитьГлубинуДляВершинToolStripMenuItem.Click += new System.EventHandler(this.отключитьГлубинуДляВершинToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(820, 507);
            this.Controls.Add(this.flowLayoutPanelLoadSaveButtons);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.glControl);
            this.Name = "MainForm";
            this.Text = "3D Модель редактор";
            this.groupBoxTools.ResumeLayout(false);
            this.groupBoxTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericVertexX)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBoxView.ResumeLayout(false);
            this.groupBoxView.PerformLayout();
            this.groupBoxWorld.ResumeLayout(false);
            this.groupBoxWorld.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanelLoadSaveButtons.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBoxView;
        private System.Windows.Forms.GroupBox groupBoxWorld;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelLoadSaveButtons;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.NumericUpDown numericVertexZ;
        private System.Windows.Forms.NumericUpDown numericVertexY;
        private System.Windows.Forms.NumericUpDown numericVertexX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem отключитьГлубинуДляВершинToolStripMenuItem;
    }
}