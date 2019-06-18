namespace ClusterAlgorithmForms.GUI
{
    partial class FormTrajectoryClustering
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Location = new System.Drawing.Point(13, 61);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(418, 71);
            this.groupBox1.TabIndex = 71;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "阈值设定";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(270, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 12);
            this.label4.TabIndex = 73;
            this.label4.Text = "共享最近邻个数:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 12);
            this.label3.TabIndex = 72;
            this.label3.Text = "时间窗口:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(79, 30);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(37, 21);
            this.textBox4.TabIndex = 71;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(368, 30);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(37, 21);
            this.textBox3.TabIndex = 70;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(132, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 67;
            this.label1.Text = "空间邻近阈值:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(216, 30);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(37, 21);
            this.textBox2.TabIndex = 68;
            // 
            // buttonSave
            // 
            this.buttonSave.AutoSize = true;
            this.buttonSave.Location = new System.Drawing.Point(17, 141);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(87, 23);
            this.buttonSave.TabIndex = 82;
            this.buttonSave.Text = "选择保存路径";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Location = new System.Drawing.Point(111, 142);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(311, 21);
            this.textBoxFilePath.TabIndex = 83;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(262, 190);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 84;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(356, 190);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 85;
            this.buttonOK.Text = "确定";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "年",
            "季",
            "月",
            "天",
            "时刻"});
            this.comboBox1.Location = new System.Drawing.Point(147, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 86;
            // 
            // FormTrajectoryClustering
            // 
            this.ClientSize = new System.Drawing.Size(443, 236);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormTrajectoryClustering";
            this.Text = "时空轨迹聚类";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.GroupBox groupBoxThreshold;
        private System.Windows.Forms.TextBox textBoxShareNearest;
        private System.Windows.Forms.Label labelShareNearest;
        private System.Windows.Forms.TextBox textBoxTime;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.TextBox textBoxCore;
        private System.Windows.Forms.Label labelCore;
        private System.Windows.Forms.TextBox textBoxPropertyNear;
        private System.Windows.Forms.Label labelNeighborhood;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxFilePath;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}