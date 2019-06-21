namespace ClusterAlgorithm.GUI
{
    partial class FormDDBSC
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
            this.labelCore = new System.Windows.Forms.Label();
            this.textBoxPropertySimilarity = new System.Windows.Forms.TextBox();
            this.labelPropertySimilarity = new System.Windows.Forms.Label();
            this.labelDataset = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.labelRange = new System.Windows.Forms.Label();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxThreshold = new System.Windows.Forms.GroupBox();
            this.comboBoxNeighborhood = new System.Windows.Forms.ComboBox();
            this.textBoxCore = new System.Windows.Forms.TextBox();
            this.labelNeighborhood = new System.Windows.Forms.Label();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.comboBoxDataset = new System.Windows.Forms.ComboBox();
            this.textBoxNum = new System.Windows.Forms.TextBox();
            this.labelNum = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.groupBoxFileList = new System.Windows.Forms.GroupBox();
            this.listBoxFileList = new System.Windows.Forms.ListBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxThreshold.SuspendLayout();
            this.groupBoxFileList.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelCore
            // 
            this.labelCore.AutoSize = true;
            this.labelCore.Location = new System.Drawing.Point(223, 68);
            this.labelCore.Name = "labelCore";
            this.labelCore.Size = new System.Drawing.Size(77, 12);
            this.labelCore.TabIndex = 8;
            this.labelCore.Text = "核心点阈值：";
            // 
            // textBoxPropertySimilarity
            // 
            this.textBoxPropertySimilarity.Location = new System.Drawing.Point(110, 65);
            this.textBoxPropertySimilarity.Name = "textBoxPropertySimilarity";
            this.textBoxPropertySimilarity.Size = new System.Drawing.Size(43, 21);
            this.textBoxPropertySimilarity.TabIndex = 7;
            // 
            // labelPropertySimilarity
            // 
            this.labelPropertySimilarity.AutoSize = true;
            this.labelPropertySimilarity.Location = new System.Drawing.Point(6, 68);
            this.labelPropertySimilarity.Name = "labelPropertySimilarity";
            this.labelPropertySimilarity.Size = new System.Drawing.Size(101, 12);
            this.labelPropertySimilarity.TabIndex = 6;
            this.labelPropertySimilarity.Text = "属性相似性阈值：";
            // 
            // labelDataset
            // 
            this.labelDataset.AutoSize = true;
            this.labelDataset.Location = new System.Drawing.Point(395, 153);
            this.labelDataset.Name = "labelDataset";
            this.labelDataset.Size = new System.Drawing.Size(41, 12);
            this.labelDataset.TabIndex = 41;
            this.labelDataset.Text = "数据集";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(377, 355);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 47;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(376, 27);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(50, 21);
            this.textBoxMax.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(359, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "-";
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(302, 26);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(50, 21);
            this.textBoxMin.TabIndex = 3;
            // 
            // labelRange
            // 
            this.labelRange.AutoSize = true;
            this.labelRange.Location = new System.Drawing.Point(163, 30);
            this.labelRange.Name = "labelRange";
            this.labelRange.Size = new System.Drawing.Size(137, 12);
            this.labelRange.TabIndex = 2;
            this.labelRange.Text = "忽略属性范围（选填）：";
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(17, 327);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(87, 23);
            this.buttonSave.TabIndex = 44;
            this.buttonSave.Text = "选择保存路径";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // groupBoxThreshold
            // 
            this.groupBoxThreshold.Controls.Add(this.comboBoxNeighborhood);
            this.groupBoxThreshold.Controls.Add(this.textBoxCore);
            this.groupBoxThreshold.Controls.Add(this.labelCore);
            this.groupBoxThreshold.Controls.Add(this.textBoxPropertySimilarity);
            this.groupBoxThreshold.Controls.Add(this.labelPropertySimilarity);
            this.groupBoxThreshold.Controls.Add(this.textBoxMax);
            this.groupBoxThreshold.Controls.Add(this.label1);
            this.groupBoxThreshold.Controls.Add(this.textBoxMin);
            this.groupBoxThreshold.Controls.Add(this.labelRange);
            this.groupBoxThreshold.Controls.Add(this.labelNeighborhood);
            this.groupBoxThreshold.Location = new System.Drawing.Point(17, 218);
            this.groupBoxThreshold.Name = "groupBoxThreshold";
            this.groupBoxThreshold.Size = new System.Drawing.Size(435, 103);
            this.groupBoxThreshold.TabIndex = 43;
            this.groupBoxThreshold.TabStop = false;
            this.groupBoxThreshold.Text = "阈值设定";
            // 
            // comboBoxNeighborhood
            // 
            this.comboBoxNeighborhood.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNeighborhood.FormattingEnabled = true;
            this.comboBoxNeighborhood.Location = new System.Drawing.Point(110, 26);
            this.comboBoxNeighborhood.Name = "comboBoxNeighborhood";
            this.comboBoxNeighborhood.Size = new System.Drawing.Size(43, 20);
            this.comboBoxNeighborhood.TabIndex = 48;
            // 
            // textBoxCore
            // 
            this.textBoxCore.Location = new System.Drawing.Point(303, 65);
            this.textBoxCore.Name = "textBoxCore";
            this.textBoxCore.Size = new System.Drawing.Size(49, 21);
            this.textBoxCore.TabIndex = 9;
            // 
            // labelNeighborhood
            // 
            this.labelNeighborhood.AutoSize = true;
            this.labelNeighborhood.Location = new System.Drawing.Point(18, 30);
            this.labelNeighborhood.Name = "labelNeighborhood";
            this.labelNeighborhood.Size = new System.Drawing.Size(89, 12);
            this.labelNeighborhood.TabIndex = 0;
            this.labelNeighborhood.Text = "空间邻域阈值：";
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(111, 328);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(341, 21);
            this.textBoxFilePath.TabIndex = 45;
            // 
            // comboBoxDataset
            // 
            this.comboBoxDataset.FormattingEnabled = true;
            this.comboBoxDataset.Location = new System.Drawing.Point(377, 175);
            this.comboBoxDataset.Name = "comboBoxDataset";
            this.comboBoxDataset.Size = new System.Drawing.Size(75, 20);
            this.comboBoxDataset.TabIndex = 42;
            // 
            // textBoxNum
            // 
            this.textBoxNum.Location = new System.Drawing.Point(377, 119);
            this.textBoxNum.Name = "textBoxNum";
            this.textBoxNum.Size = new System.Drawing.Size(75, 21);
            this.textBoxNum.TabIndex = 40;
            // 
            // labelNum
            // 
            this.labelNum.AutoSize = true;
            this.labelNum.Location = new System.Drawing.Point(398, 101);
            this.labelNum.Name = "labelNum";
            this.labelNum.Size = new System.Drawing.Size(29, 12);
            this.labelNum.TabIndex = 39;
            this.labelNum.Text = "个数";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(377, 64);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 38;
            this.buttonDelete.Text = "删除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(377, 28);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 37;
            this.buttonAdd.Text = "添加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // groupBoxFileList
            // 
            this.groupBoxFileList.Controls.Add(this.listBoxFileList);
            this.groupBoxFileList.Location = new System.Drawing.Point(14, 11);
            this.groupBoxFileList.Name = "groupBoxFileList";
            this.groupBoxFileList.Size = new System.Drawing.Size(348, 187);
            this.groupBoxFileList.TabIndex = 36;
            this.groupBoxFileList.TabStop = false;
            this.groupBoxFileList.Text = "文件列表或文件路径";
            // 
            // listBoxFileList
            // 
            this.listBoxFileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxFileList.FormattingEnabled = true;
            this.listBoxFileList.HorizontalScrollbar = true;
            this.listBoxFileList.ItemHeight = 12;
            this.listBoxFileList.Location = new System.Drawing.Point(3, 17);
            this.listBoxFileList.Name = "listBoxFileList";
            this.listBoxFileList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFileList.Size = new System.Drawing.Size(342, 167);
            this.listBoxFileList.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(283, 355);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 46;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // FormDDBSC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 389);
            this.Controls.Add(this.labelDataset);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBoxThreshold);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.comboBoxDataset);
            this.Controls.Add(this.textBoxNum);
            this.Controls.Add(this.labelNum);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.groupBoxFileList);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormDDBSC";
            this.Text = "双重约束聚类-DDBSC";
            this.groupBoxThreshold.ResumeLayout(false);
            this.groupBoxThreshold.PerformLayout();
            this.groupBoxFileList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCore;
        private System.Windows.Forms.TextBox textBoxPropertySimilarity;
        private System.Windows.Forms.Label labelPropertySimilarity;
        private System.Windows.Forms.Label labelDataset;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMin;
        private System.Windows.Forms.Label labelRange;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.GroupBox groupBoxThreshold;
        private System.Windows.Forms.ComboBox comboBoxNeighborhood;
        private System.Windows.Forms.TextBox textBoxCore;
        private System.Windows.Forms.Label labelNeighborhood;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.ComboBox comboBoxDataset;
        private System.Windows.Forms.TextBox textBoxNum;
        private System.Windows.Forms.Label labelNum;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.GroupBox groupBoxFileList;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBoxFileList;
    }
}