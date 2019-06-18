namespace Forms.GUI
{
    partial class FormTimeKmean
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxThreshold = new System.Windows.Forms.GroupBox();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.labelRange = new System.Windows.Forms.Label();
            this.textBoxK = new System.Windows.Forms.TextBox();
            this.labelK = new System.Windows.Forms.Label();
            this.comboBoxDataset = new System.Windows.Forms.ComboBox();
            this.textBoxNum = new System.Windows.Forms.TextBox();
            this.labelNum = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.labelDataset = new System.Windows.Forms.Label();
            this.groupBoxFileList = new System.Windows.Forms.GroupBox();
            this.listBoxFileList = new System.Windows.Forms.ListBox();
            this.groupBoxThreshold.SuspendLayout();
            this.groupBoxFileList.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(377, 321);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 23;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(283, 321);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 22;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(111, 294);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(341, 21);
            this.textBoxFilePath.TabIndex = 21;
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(17, 293);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(87, 23);
            this.buttonSave.TabIndex = 20;
            this.buttonSave.Text = "选择保存路径";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // groupBoxThreshold
            // 
            this.groupBoxThreshold.Controls.Add(this.textBoxMax);
            this.groupBoxThreshold.Controls.Add(this.label1);
            this.groupBoxThreshold.Controls.Add(this.textBoxMin);
            this.groupBoxThreshold.Controls.Add(this.labelRange);
            this.groupBoxThreshold.Controls.Add(this.textBoxK);
            this.groupBoxThreshold.Controls.Add(this.labelK);
            this.groupBoxThreshold.Location = new System.Drawing.Point(17, 222);
            this.groupBoxThreshold.Name = "groupBoxThreshold";
            this.groupBoxThreshold.Size = new System.Drawing.Size(435, 64);
            this.groupBoxThreshold.TabIndex = 19;
            this.groupBoxThreshold.TabStop = false;
            this.groupBoxThreshold.Text = "阈值设定";
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(367, 27);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(50, 21);
            this.textBoxMax.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(349, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "-";
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(292, 26);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(50, 21);
            this.textBoxMin.TabIndex = 3;
            // 
            // labelRange
            // 
            this.labelRange.AutoSize = true;
            this.labelRange.Location = new System.Drawing.Point(153, 30);
            this.labelRange.Name = "labelRange";
            this.labelRange.Size = new System.Drawing.Size(137, 12);
            this.labelRange.TabIndex = 2;
            this.labelRange.Text = "忽略属性范围（选填）：";
            // 
            // textBoxK
            // 
            this.textBoxK.Location = new System.Drawing.Point(91, 26);
            this.textBoxK.Name = "textBoxK";
            this.textBoxK.Size = new System.Drawing.Size(43, 21);
            this.textBoxK.TabIndex = 1;
            // 
            // labelK
            // 
            this.labelK.AutoSize = true;
            this.labelK.Location = new System.Drawing.Point(17, 30);
            this.labelK.Name = "labelK";
            this.labelK.Size = new System.Drawing.Size(71, 12);
            this.labelK.TabIndex = 0;
            this.labelK.Text = "聚类个数K：";
            // 
            // comboBoxDataset
            // 
            this.comboBoxDataset.FormattingEnabled = true;
            this.comboBoxDataset.Location = new System.Drawing.Point(377, 179);
            this.comboBoxDataset.Name = "comboBoxDataset";
            this.comboBoxDataset.Size = new System.Drawing.Size(75, 20);
            this.comboBoxDataset.TabIndex = 18;
            // 
            // textBoxNum
            // 
            this.textBoxNum.Location = new System.Drawing.Point(377, 123);
            this.textBoxNum.Name = "textBoxNum";
            this.textBoxNum.Size = new System.Drawing.Size(75, 21);
            this.textBoxNum.TabIndex = 16;
            // 
            // labelNum
            // 
            this.labelNum.AutoSize = true;
            this.labelNum.Location = new System.Drawing.Point(398, 105);
            this.labelNum.Name = "labelNum";
            this.labelNum.Size = new System.Drawing.Size(29, 12);
            this.labelNum.TabIndex = 15;
            this.labelNum.Text = "个数";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(377, 68);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 14;
            this.buttonDelete.Text = "删除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(377, 32);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 13;
            this.buttonAdd.Text = "添加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // labelDataset
            // 
            this.labelDataset.AutoSize = true;
            this.labelDataset.Location = new System.Drawing.Point(395, 157);
            this.labelDataset.Name = "labelDataset";
            this.labelDataset.Size = new System.Drawing.Size(41, 12);
            this.labelDataset.TabIndex = 17;
            this.labelDataset.Text = "数据集";
            // 
            // groupBoxFileList
            // 
            this.groupBoxFileList.Controls.Add(this.listBoxFileList);
            this.groupBoxFileList.Location = new System.Drawing.Point(14, 15);
            this.groupBoxFileList.Name = "groupBoxFileList";
            this.groupBoxFileList.Size = new System.Drawing.Size(348, 187);
            this.groupBoxFileList.TabIndex = 12;
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
            // FormTimeKmean
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 358);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.groupBoxThreshold);
            this.Controls.Add(this.comboBoxDataset);
            this.Controls.Add(this.textBoxNum);
            this.Controls.Add(this.labelNum);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.labelDataset);
            this.Controls.Add(this.groupBoxFileList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormTimeKmean";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "时间序列聚类分析-K-MEAN";
            this.groupBoxThreshold.ResumeLayout(false);
            this.groupBoxThreshold.PerformLayout();
            this.groupBoxFileList.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.GroupBox groupBoxThreshold;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMin;
        private System.Windows.Forms.Label labelRange;
        private System.Windows.Forms.TextBox textBoxK;
        private System.Windows.Forms.Label labelK;
        private System.Windows.Forms.ComboBox comboBoxDataset;
        private System.Windows.Forms.TextBox textBoxNum;
        private System.Windows.Forms.Label labelNum;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label labelDataset;
        private System.Windows.Forms.GroupBox groupBoxFileList;
        private System.Windows.Forms.ListBox listBoxFileList;
    }
}