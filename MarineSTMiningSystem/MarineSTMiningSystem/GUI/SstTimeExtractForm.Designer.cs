namespace MarineSTMiningSystem.GUI
{
    partial class SstTimeExtractForm
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
            this.FileListbox = new System.Windows.Forms.ListBox();
            this.addFilesbtn = new System.Windows.Forms.Button();
            this.deleteFilesbtn = new System.Windows.Forms.Button();
            this.upMovebtn = new System.Windows.Forms.Button();
            this.downMovebtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.FileNumbox = new System.Windows.Forms.TextBox();
            this.rowTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.maxTimeIntervalTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.timeCellTextBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.valueScaleTextBox = new System.Windows.Forms.TextBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.zeroValueCheckBox = new System.Windows.Forms.CheckBox();
            this.thresholdCBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.thresholdBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.MinDurTimeBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.FileListbox2 = new System.Windows.Forms.ListBox();
            this.FileNumbox2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.downMovebtn2 = new System.Windows.Forms.Button();
            this.upMovebtn2 = new System.Windows.Forms.Button();
            this.deleteFilesbtn2 = new System.Windows.Forms.Button();
            this.addFilesbtn2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // FileListbox
            // 
            this.FileListbox.BackColor = System.Drawing.Color.White;
            this.FileListbox.FormattingEnabled = true;
            this.FileListbox.ItemHeight = 12;
            this.FileListbox.Location = new System.Drawing.Point(6, 20);
            this.FileListbox.Name = "FileListbox";
            this.FileListbox.Size = new System.Drawing.Size(374, 196);
            this.FileListbox.TabIndex = 0;
            // 
            // addFilesbtn
            // 
            this.addFilesbtn.Location = new System.Drawing.Point(398, 12);
            this.addFilesbtn.Name = "addFilesbtn";
            this.addFilesbtn.Size = new System.Drawing.Size(75, 23);
            this.addFilesbtn.TabIndex = 1;
            this.addFilesbtn.Text = "添加";
            this.addFilesbtn.UseVisualStyleBackColor = true;
            this.addFilesbtn.Click += new System.EventHandler(this.addFilesbtn_Click);
            // 
            // deleteFilesbtn
            // 
            this.deleteFilesbtn.Location = new System.Drawing.Point(398, 41);
            this.deleteFilesbtn.Name = "deleteFilesbtn";
            this.deleteFilesbtn.Size = new System.Drawing.Size(75, 23);
            this.deleteFilesbtn.TabIndex = 2;
            this.deleteFilesbtn.Text = "删除";
            this.deleteFilesbtn.UseVisualStyleBackColor = true;
            this.deleteFilesbtn.Click += new System.EventHandler(this.deleteFilesbtn_Click);
            // 
            // upMovebtn
            // 
            this.upMovebtn.Location = new System.Drawing.Point(398, 70);
            this.upMovebtn.Name = "upMovebtn";
            this.upMovebtn.Size = new System.Drawing.Size(75, 23);
            this.upMovebtn.TabIndex = 3;
            this.upMovebtn.Text = "上移";
            this.upMovebtn.UseVisualStyleBackColor = true;
            this.upMovebtn.Click += new System.EventHandler(this.upMovebtn_Click);
            // 
            // downMovebtn
            // 
            this.downMovebtn.Location = new System.Drawing.Point(398, 99);
            this.downMovebtn.Name = "downMovebtn";
            this.downMovebtn.Size = new System.Drawing.Size(75, 23);
            this.downMovebtn.TabIndex = 4;
            this.downMovebtn.Text = "下移";
            this.downMovebtn.UseVisualStyleBackColor = true;
            this.downMovebtn.Click += new System.EventHandler(this.downMovebtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(399, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "文件数量：";
            // 
            // FileNumbox
            // 
            this.FileNumbox.Location = new System.Drawing.Point(401, 146);
            this.FileNumbox.Name = "FileNumbox";
            this.FileNumbox.Size = new System.Drawing.Size(72, 21);
            this.FileNumbox.TabIndex = 6;
            this.FileNumbox.Text = "0";
            // 
            // rowTextBox
            // 
            this.rowTextBox.BackColor = System.Drawing.Color.White;
            this.rowTextBox.Location = new System.Drawing.Point(66, 268);
            this.rowTextBox.Name = "rowTextBox";
            this.rowTextBox.Size = new System.Drawing.Size(50, 21);
            this.rowTextBox.TabIndex = 74;
            this.rowTextBox.Text = "10";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 271);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 73;
            this.label4.Text = "处理行数：";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(184, 268);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(50, 21);
            this.textBox2.TabIndex = 72;
            this.textBox2.Text = "2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 271);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 71;
            this.label3.Text = "线程数：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(385, 243);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 12);
            this.label7.TabIndex = 70;
            this.label7.Text = "月";
            // 
            // maxTimeIntervalTextBox
            // 
            this.maxTimeIntervalTextBox.BackColor = System.Drawing.Color.White;
            this.maxTimeIntervalTextBox.Location = new System.Drawing.Point(325, 240);
            this.maxTimeIntervalTextBox.Name = "maxTimeIntervalTextBox";
            this.maxTimeIntervalTextBox.Size = new System.Drawing.Size(50, 21);
            this.maxTimeIntervalTextBox.TabIndex = 69;
            this.maxTimeIntervalTextBox.Text = "5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(263, 243);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 68;
            this.label8.Text = "最大间隔：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(240, 243);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 12);
            this.label9.TabIndex = 67;
            this.label9.Text = "月";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(122, 243);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 12);
            this.label10.TabIndex = 66;
            this.label10.Text = "时间尺度：";
            // 
            // timeCellTextBox
            // 
            this.timeCellTextBox.BackColor = System.Drawing.Color.White;
            this.timeCellTextBox.Location = new System.Drawing.Point(184, 240);
            this.timeCellTextBox.Name = "timeCellTextBox";
            this.timeCellTextBox.Size = new System.Drawing.Size(50, 21);
            this.timeCellTextBox.TabIndex = 65;
            this.timeCellTextBox.Text = "1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 243);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 64;
            this.label11.Text = "值系数：";
            // 
            // valueScaleTextBox
            // 
            this.valueScaleTextBox.BackColor = System.Drawing.Color.White;
            this.valueScaleTextBox.Location = new System.Drawing.Point(66, 240);
            this.valueScaleTextBox.Name = "valueScaleTextBox";
            this.valueScaleTextBox.Size = new System.Drawing.Size(50, 21);
            this.valueScaleTextBox.TabIndex = 63;
            this.valueScaleTextBox.Text = "1";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(413, 335);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 62;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(4, 335);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(347, 23);
            this.progressBar1.TabIndex = 61;
            this.progressBar1.Visible = false;
            // 
            // openBtn
            // 
            this.openBtn.Location = new System.Drawing.Point(413, 306);
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(50, 23);
            this.openBtn.TabIndex = 60;
            this.openBtn.Text = "选择";
            this.openBtn.UseVisualStyleBackColor = true;
            this.openBtn.Click += new System.EventHandler(this.openBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 309);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 59;
            this.label2.Text = "输出文件夹：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(78, 306);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(329, 21);
            this.textBox1.TabIndex = 58;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(357, 335);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 57;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // zeroValueCheckBox
            // 
            this.zeroValueCheckBox.AutoSize = true;
            this.zeroValueCheckBox.Location = new System.Drawing.Point(413, 243);
            this.zeroValueCheckBox.Name = "zeroValueCheckBox";
            this.zeroValueCheckBox.Size = new System.Drawing.Size(54, 16);
            this.zeroValueCheckBox.TabIndex = 75;
            this.zeroValueCheckBox.Text = "0赋值";
            this.zeroValueCheckBox.UseVisualStyleBackColor = true;
            // 
            // thresholdCBox
            // 
            this.thresholdCBox.FormattingEnabled = true;
            this.thresholdCBox.Items.AddRange(new object[] {
            "0.5 std",
            "1 std",
            "1.5 std",
            "2 std",
            "k std",
            "固定阈值"});
            this.thresholdCBox.Location = new System.Drawing.Point(302, 268);
            this.thresholdCBox.Name = "thresholdCBox";
            this.thresholdCBox.Size = new System.Drawing.Size(73, 20);
            this.thresholdCBox.TabIndex = 76;
            this.thresholdCBox.SelectedIndexChanged += new System.EventHandler(this.thresholdCBox_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(240, 271);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 77;
            this.label5.Text = "异常阈值：";
            // 
            // thresholdBox
            // 
            this.thresholdBox.Location = new System.Drawing.Point(382, 267);
            this.thresholdBox.Name = "thresholdBox";
            this.thresholdBox.Size = new System.Drawing.Size(91, 21);
            this.thresholdBox.TabIndex = 78;
            this.thresholdBox.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(399, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 79;
            this.label6.Text = "最短持续时间：";
            // 
            // MinDurTimeBox
            // 
            this.MinDurTimeBox.Location = new System.Drawing.Point(401, 206);
            this.MinDurTimeBox.Name = "MinDurTimeBox";
            this.MinDurTimeBox.Size = new System.Drawing.Size(72, 21);
            this.MinDurTimeBox.TabIndex = 80;
            this.MinDurTimeBox.Text = "5";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FileListbox);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 221);
            this.groupBox1.TabIndex = 81;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "距平数据";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.FileListbox2);
            this.groupBox2.Location = new System.Drawing.Point(6, 364);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(386, 253);
            this.groupBox2.TabIndex = 82;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "原始数据";
            // 
            // FileListbox2
            // 
            this.FileListbox2.FormattingEnabled = true;
            this.FileListbox2.ItemHeight = 12;
            this.FileListbox2.Location = new System.Drawing.Point(6, 20);
            this.FileListbox2.Name = "FileListbox2";
            this.FileListbox2.Size = new System.Drawing.Size(374, 220);
            this.FileListbox2.TabIndex = 0;
            // 
            // FileNumbox2
            // 
            this.FileNumbox2.Location = new System.Drawing.Point(398, 583);
            this.FileNumbox2.Name = "FileNumbox2";
            this.FileNumbox2.Size = new System.Drawing.Size(72, 21);
            this.FileNumbox2.TabIndex = 88;
            this.FileNumbox2.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(396, 568);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 87;
            this.label12.Text = "文件数量：";
            // 
            // downMovebtn2
            // 
            this.downMovebtn2.Location = new System.Drawing.Point(398, 471);
            this.downMovebtn2.Name = "downMovebtn2";
            this.downMovebtn2.Size = new System.Drawing.Size(75, 23);
            this.downMovebtn2.TabIndex = 86;
            this.downMovebtn2.Text = "下移";
            this.downMovebtn2.UseVisualStyleBackColor = true;
            this.downMovebtn2.Click += new System.EventHandler(this.downMovebtn2_Click);
            // 
            // upMovebtn2
            // 
            this.upMovebtn2.Location = new System.Drawing.Point(398, 442);
            this.upMovebtn2.Name = "upMovebtn2";
            this.upMovebtn2.Size = new System.Drawing.Size(75, 23);
            this.upMovebtn2.TabIndex = 85;
            this.upMovebtn2.Text = "上移";
            this.upMovebtn2.UseVisualStyleBackColor = true;
            this.upMovebtn2.Click += new System.EventHandler(this.upMovebtn2_Click);
            // 
            // deleteFilesbtn2
            // 
            this.deleteFilesbtn2.Location = new System.Drawing.Point(398, 413);
            this.deleteFilesbtn2.Name = "deleteFilesbtn2";
            this.deleteFilesbtn2.Size = new System.Drawing.Size(75, 23);
            this.deleteFilesbtn2.TabIndex = 84;
            this.deleteFilesbtn2.Text = "删除";
            this.deleteFilesbtn2.UseVisualStyleBackColor = true;
            this.deleteFilesbtn2.Click += new System.EventHandler(this.deleteFilesbtn2_Click);
            // 
            // addFilesbtn2
            // 
            this.addFilesbtn2.Location = new System.Drawing.Point(398, 384);
            this.addFilesbtn2.Name = "addFilesbtn2";
            this.addFilesbtn2.Size = new System.Drawing.Size(75, 23);
            this.addFilesbtn2.TabIndex = 83;
            this.addFilesbtn2.Text = "添加";
            this.addFilesbtn2.UseVisualStyleBackColor = true;
            this.addFilesbtn2.Click += new System.EventHandler(this.addFilesbtn2_Click);
            // 
            // SstTimeExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(480, 629);
            this.Controls.Add(this.FileNumbox2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.downMovebtn2);
            this.Controls.Add(this.upMovebtn2);
            this.Controls.Add(this.deleteFilesbtn2);
            this.Controls.Add(this.addFilesbtn2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.MinDurTimeBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.thresholdBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.thresholdCBox);
            this.Controls.Add(this.zeroValueCheckBox);
            this.Controls.Add(this.rowTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.maxTimeIntervalTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.timeCellTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.valueScaleTextBox);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.openBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.FileNumbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.downMovebtn);
            this.Controls.Add(this.upMovebtn);
            this.Controls.Add(this.deleteFilesbtn);
            this.Controls.Add(this.addFilesbtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "SstTimeExtractForm";
            this.Text = "海温时间维提取";
            this.TransparencyKey = System.Drawing.Color.LightGray;
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox FileListbox;
        private System.Windows.Forms.Button addFilesbtn;
        private System.Windows.Forms.Button deleteFilesbtn;
        private System.Windows.Forms.Button upMovebtn;
        private System.Windows.Forms.Button downMovebtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox FileNumbox;
        private System.Windows.Forms.TextBox rowTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox maxTimeIntervalTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox timeCellTextBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox valueScaleTextBox;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button openBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.CheckBox zeroValueCheckBox;
        private System.Windows.Forms.ComboBox thresholdCBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox thresholdBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox MinDurTimeBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox FileListbox2;
        private System.Windows.Forms.TextBox FileNumbox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button downMovebtn2;
        private System.Windows.Forms.Button upMovebtn2;
        private System.Windows.Forms.Button deleteFilesbtn2;
        private System.Windows.Forms.Button addFilesbtn2;
    }
}