namespace Forms.GUI
{
    partial class FormRoSTCM
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
            this.comboBoxNeighborhood = new System.Windows.Forms.ComboBox();
            this.groupBoxFileList = new System.Windows.Forms.GroupBox();
            this.listBoxFileList = new System.Windows.Forms.ListBox();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.comboBoxDataset = new System.Windows.Forms.ComboBox();
            this.textBoxNum = new System.Windows.Forms.TextBox();
            this.labelNum = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxCore = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxMax = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMin = new System.Windows.Forms.TextBox();
            this.labelRange = new System.Windows.Forms.Label();
            this.labelNeighborhood = new System.Windows.Forms.Label();
            this.labelDataset = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxPropertyNear = new System.Windows.Forms.TextBox();
            this.labelPropertyNear = new System.Windows.Forms.Label();
            this.groupBoxThreshold = new System.Windows.Forms.GroupBox();
            this.buttonGet = new System.Windows.Forms.Button();
            this.textBoxTime = new System.Windows.Forms.TextBox();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelCore = new System.Windows.Forms.Label();
            this.groupBoxFileList.SuspendLayout();
            this.groupBoxThreshold.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxNeighborhood
            // 
            this.comboBoxNeighborhood.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxNeighborhood.FormattingEnabled = true;
            this.comboBoxNeighborhood.Location = new System.Drawing.Point(68, 65);
            this.comboBoxNeighborhood.Name = "comboBoxNeighborhood";
            this.comboBoxNeighborhood.Size = new System.Drawing.Size(43, 20);
            this.comboBoxNeighborhood.TabIndex = 48;
            // 
            // groupBoxFileList
            // 
            this.groupBoxFileList.Controls.Add(this.listBoxFileList);
            this.groupBoxFileList.Location = new System.Drawing.Point(14, 11);
            this.groupBoxFileList.Name = "groupBoxFileList";
            this.groupBoxFileList.Size = new System.Drawing.Size(348, 187);
            this.groupBoxFileList.TabIndex = 48;
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
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(111, 328);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(341, 21);
            this.textBoxFilePath.TabIndex = 57;
            // 
            // comboBoxDataset
            // 
            this.comboBoxDataset.FormattingEnabled = true;
            this.comboBoxDataset.Location = new System.Drawing.Point(377, 175);
            this.comboBoxDataset.Name = "comboBoxDataset";
            this.comboBoxDataset.Size = new System.Drawing.Size(75, 20);
            this.comboBoxDataset.TabIndex = 54;
            // 
            // textBoxNum
            // 
            this.textBoxNum.Location = new System.Drawing.Point(377, 119);
            this.textBoxNum.Name = "textBoxNum";
            this.textBoxNum.Size = new System.Drawing.Size(75, 21);
            this.textBoxNum.TabIndex = 52;
            // 
            // labelNum
            // 
            this.labelNum.AutoSize = true;
            this.labelNum.Location = new System.Drawing.Point(398, 101);
            this.labelNum.Name = "labelNum";
            this.labelNum.Size = new System.Drawing.Size(29, 12);
            this.labelNum.TabIndex = 51;
            this.labelNum.Text = "个数";
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(377, 64);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(75, 23);
            this.buttonDelete.TabIndex = 50;
            this.buttonDelete.Text = "删除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(377, 28);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 49;
            this.buttonAdd.Text = "添加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(287, 355);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 58;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxCore
            // 
            this.textBoxCore.Location = new System.Drawing.Point(360, 68);
            this.textBoxCore.Name = "textBoxCore";
            this.textBoxCore.Size = new System.Drawing.Size(64, 21);
            this.textBoxCore.TabIndex = 9;
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(17, 327);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(87, 23);
            this.buttonSave.TabIndex = 56;
            this.buttonSave.Text = "选择保存路径";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxMax
            // 
            this.textBoxMax.Location = new System.Drawing.Point(188, 24);
            this.textBoxMax.Name = "textBoxMax";
            this.textBoxMax.Size = new System.Drawing.Size(35, 21);
            this.textBoxMax.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(175, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "-";
            // 
            // textBoxMin
            // 
            this.textBoxMin.Location = new System.Drawing.Point(135, 24);
            this.textBoxMin.Name = "textBoxMin";
            this.textBoxMin.Size = new System.Drawing.Size(36, 21);
            this.textBoxMin.TabIndex = 3;
            // 
            // labelRange
            // 
            this.labelRange.AutoSize = true;
            this.labelRange.Location = new System.Drawing.Point(6, 28);
            this.labelRange.Name = "labelRange";
            this.labelRange.Size = new System.Drawing.Size(131, 12);
            this.labelRange.TabIndex = 2;
            this.labelRange.Text = "忽略属性范围（选填）:";
            // 
            // labelNeighborhood
            // 
            this.labelNeighborhood.AutoSize = true;
            this.labelNeighborhood.Location = new System.Drawing.Point(6, 68);
            this.labelNeighborhood.Name = "labelNeighborhood";
            this.labelNeighborhood.Size = new System.Drawing.Size(59, 12);
            this.labelNeighborhood.TabIndex = 0;
            this.labelNeighborhood.Text = "空间邻域:";
            // 
            // labelDataset
            // 
            this.labelDataset.AutoSize = true;
            this.labelDataset.Location = new System.Drawing.Point(395, 153);
            this.labelDataset.Name = "labelDataset";
            this.labelDataset.Size = new System.Drawing.Size(41, 12);
            this.labelDataset.TabIndex = 53;
            this.labelDataset.Text = "数据集";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(377, 355);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 59;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textBoxPropertyNear
            // 
            this.textBoxPropertyNear.Location = new System.Drawing.Point(381, 24);
            this.textBoxPropertyNear.Name = "textBoxPropertyNear";
            this.textBoxPropertyNear.Size = new System.Drawing.Size(43, 21);
            this.textBoxPropertyNear.TabIndex = 7;
            // 
            // labelPropertyNear
            // 
            this.labelPropertyNear.AutoSize = true;
            this.labelPropertyNear.Location = new System.Drawing.Point(281, 28);
            this.labelPropertyNear.Name = "labelPropertyNear";
            this.labelPropertyNear.Size = new System.Drawing.Size(83, 12);
            this.labelPropertyNear.TabIndex = 6;
            this.labelPropertyNear.Text = "属性临近阈值:";
            // 
            // groupBoxThreshold
            // 
            this.groupBoxThreshold.Controls.Add(this.buttonGet);
            this.groupBoxThreshold.Controls.Add(this.textBoxTime);
            this.groupBoxThreshold.Controls.Add(this.comboBoxNeighborhood);
            this.groupBoxThreshold.Controls.Add(this.labelTime);
            this.groupBoxThreshold.Controls.Add(this.textBoxCore);
            this.groupBoxThreshold.Controls.Add(this.labelCore);
            this.groupBoxThreshold.Controls.Add(this.textBoxPropertyNear);
            this.groupBoxThreshold.Controls.Add(this.labelPropertyNear);
            this.groupBoxThreshold.Controls.Add(this.textBoxMax);
            this.groupBoxThreshold.Controls.Add(this.label1);
            this.groupBoxThreshold.Controls.Add(this.textBoxMin);
            this.groupBoxThreshold.Controls.Add(this.labelRange);
            this.groupBoxThreshold.Controls.Add(this.labelNeighborhood);
            this.groupBoxThreshold.Location = new System.Drawing.Point(17, 218);
            this.groupBoxThreshold.Name = "groupBoxThreshold";
            this.groupBoxThreshold.Size = new System.Drawing.Size(435, 103);
            this.groupBoxThreshold.TabIndex = 55;
            this.groupBoxThreshold.TabStop = false;
            this.groupBoxThreshold.Text = "阈值设定";
            // 
            // buttonGet
            // 
            this.buttonGet.Enabled = false;
            this.buttonGet.Location = new System.Drawing.Point(231, 38);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(47, 42);
            this.buttonGet.TabIndex = 62;
            this.buttonGet.Text = "计算阈值";
            this.buttonGet.UseVisualStyleBackColor = true;
            // 
            // textBoxTime
            // 
            this.textBoxTime.Location = new System.Drawing.Point(176, 65);
            this.textBoxTime.Name = "textBoxTime";
            this.textBoxTime.Size = new System.Drawing.Size(36, 21);
            this.textBoxTime.TabIndex = 61;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(115, 70);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(59, 12);
            this.labelTime.TabIndex = 60;
            this.labelTime.Text = "时间窗口:";
            // 
            // labelCore
            // 
            this.labelCore.AutoSize = true;
            this.labelCore.Location = new System.Drawing.Point(281, 74);
            this.labelCore.Name = "labelCore";
            this.labelCore.Size = new System.Drawing.Size(71, 12);
            this.labelCore.TabIndex = 8;
            this.labelCore.Text = "核心点阈值:";
            // 
            // FormRoSTCM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(467, 389);
            this.Controls.Add(this.groupBoxFileList);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.comboBoxDataset);
            this.Controls.Add(this.textBoxNum);
            this.Controls.Add(this.labelNum);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.labelDataset);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxThreshold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormRoSTCM";
            this.Text = "时空聚类-DcSTCA";
            this.groupBoxFileList.ResumeLayout(false);
            this.groupBoxThreshold.ResumeLayout(false);
            this.groupBoxThreshold.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxNeighborhood;
        private System.Windows.Forms.GroupBox groupBoxFileList;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.ComboBox comboBoxDataset;
        private System.Windows.Forms.TextBox textBoxNum;
        private System.Windows.Forms.Label labelNum;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxCore;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxMax;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMin;
        private System.Windows.Forms.Label labelRange;
        private System.Windows.Forms.Label labelNeighborhood;
        private System.Windows.Forms.Label labelDataset;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxPropertyNear;
        private System.Windows.Forms.Label labelPropertyNear;
        private System.Windows.Forms.GroupBox groupBoxThreshold;
        private System.Windows.Forms.Button buttonGet;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelCore;
        private System.Windows.Forms.ListBox listBoxFileList;
    }
}