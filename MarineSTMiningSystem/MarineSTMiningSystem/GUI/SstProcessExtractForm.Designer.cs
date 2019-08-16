namespace MarineSTMiningSystem.GUI
{
    partial class SstProcessExtractForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.threadCountTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SavePathComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.SelectSavepathBtn = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.countTextBox2 = new System.Windows.Forms.TextBox();
            this.moveDownBtn2 = new System.Windows.Forms.Button();
            this.moveUpBtn2 = new System.Windows.Forms.Button();
            this.deleteFileBtn2 = new System.Windows.Forms.Button();
            this.addFileBtn2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.timecellBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.minDurBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(496, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 163;
            this.label1.Text = "个数";
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Location = new System.Drawing.Point(496, 147);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(79, 21);
            this.countTextBox.TabIndex = 162;
            this.countTextBox.Text = "0";
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(496, 90);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(79, 23);
            this.moveDownBtn.TabIndex = 161;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(496, 61);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(79, 23);
            this.moveUpBtn.TabIndex = 160;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(7, 17);
            this.listBox1.Name = "listBox1";
            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(471, 196);
            this.listBox1.TabIndex = 159;
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Location = new System.Drawing.Point(496, 32);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(79, 23);
            this.deleteFileBtn.TabIndex = 158;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(496, 3);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(79, 23);
            this.addFileBtn.TabIndex = 157;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // threadCountTextBox
            // 
            this.threadCountTextBox.Location = new System.Drawing.Point(498, 191);
            this.threadCountTextBox.Name = "threadCountTextBox";
            this.threadCountTextBox.Size = new System.Drawing.Size(77, 21);
            this.threadCountTextBox.TabIndex = 156;
            this.threadCountTextBox.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(495, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 155;
            this.label4.Text = "线程数：";
            // 
            // SavePathComboBox
            // 
            this.SavePathComboBox.FormattingEnabled = true;
            this.SavePathComboBox.Location = new System.Drawing.Point(65, 242);
            this.SavePathComboBox.Name = "SavePathComboBox";
            this.SavePathComboBox.Size = new System.Drawing.Size(370, 20);
            this.SavePathComboBox.TabIndex = 154;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 242);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 153;
            this.label3.Text = "保存路径：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 268);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(430, 23);
            this.progressBar1.TabIndex = 148;
            this.progressBar1.Visible = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(498, 269);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(77, 23);
            this.cancelBtn.TabIndex = 147;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(441, 269);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 146;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // SelectSavepathBtn
            // 
            this.SelectSavepathBtn.Location = new System.Drawing.Point(442, 243);
            this.SelectSavepathBtn.Name = "SelectSavepathBtn";
            this.SelectSavepathBtn.Size = new System.Drawing.Size(49, 23);
            this.SelectSavepathBtn.TabIndex = 164;
            this.SelectSavepathBtn.Text = "选择";
            this.SelectSavepathBtn.UseVisualStyleBackColor = true;
            this.SelectSavepathBtn.Click += new System.EventHandler(this.SelectSavepathBtn_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(6, 20);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(472, 208);
            this.listBox2.TabIndex = 167;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox2);
            this.groupBox1.Location = new System.Drawing.Point(5, 297);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(484, 239);
            this.groupBox1.TabIndex = 168;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "负异常";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Location = new System.Drawing.Point(5, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(484, 231);
            this.groupBox2.TabIndex = 169;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "正异常";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(496, 493);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 171;
            this.label5.Text = "个数";
            // 
            // countTextBox2
            // 
            this.countTextBox2.BackColor = System.Drawing.Color.White;
            this.countTextBox2.Location = new System.Drawing.Point(496, 513);
            this.countTextBox2.Name = "countTextBox2";
            this.countTextBox2.ReadOnly = true;
            this.countTextBox2.Size = new System.Drawing.Size(79, 21);
            this.countTextBox2.TabIndex = 170;
            this.countTextBox2.Text = "0";
            // 
            // moveDownBtn2
            // 
            this.moveDownBtn2.Location = new System.Drawing.Point(496, 404);
            this.moveDownBtn2.Name = "moveDownBtn2";
            this.moveDownBtn2.Size = new System.Drawing.Size(79, 23);
            this.moveDownBtn2.TabIndex = 175;
            this.moveDownBtn2.Text = "下移";
            this.moveDownBtn2.UseVisualStyleBackColor = true;
            this.moveDownBtn2.Click += new System.EventHandler(this.moveDownBtn2_Click);
            // 
            // moveUpBtn2
            // 
            this.moveUpBtn2.Location = new System.Drawing.Point(496, 375);
            this.moveUpBtn2.Name = "moveUpBtn2";
            this.moveUpBtn2.Size = new System.Drawing.Size(79, 23);
            this.moveUpBtn2.TabIndex = 174;
            this.moveUpBtn2.Text = "上移";
            this.moveUpBtn2.UseVisualStyleBackColor = true;
            this.moveUpBtn2.Click += new System.EventHandler(this.moveUpBtn2_Click);
            // 
            // deleteFileBtn2
            // 
            this.deleteFileBtn2.Location = new System.Drawing.Point(496, 346);
            this.deleteFileBtn2.Name = "deleteFileBtn2";
            this.deleteFileBtn2.Size = new System.Drawing.Size(79, 23);
            this.deleteFileBtn2.TabIndex = 173;
            this.deleteFileBtn2.Text = "删除";
            this.deleteFileBtn2.UseVisualStyleBackColor = true;
            this.deleteFileBtn2.Click += new System.EventHandler(this.deleteFileBtn2_Click);
            // 
            // addFileBtn2
            // 
            this.addFileBtn2.Location = new System.Drawing.Point(496, 317);
            this.addFileBtn2.Name = "addFileBtn2";
            this.addFileBtn2.Size = new System.Drawing.Size(79, 23);
            this.addFileBtn2.TabIndex = 172;
            this.addFileBtn2.Text = "添加";
            this.addFileBtn2.UseVisualStyleBackColor = true;
            this.addFileBtn2.Click += new System.EventHandler(this.addFileBtn2_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(498, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 176;
            this.label2.Text = "时间分辨率：";
            // 
            // timecellBox
            // 
            this.timecellBox.Location = new System.Drawing.Point(498, 240);
            this.timecellBox.Name = "timecellBox";
            this.timecellBox.Size = new System.Drawing.Size(40, 21);
            this.timecellBox.TabIndex = 177;
            this.timecellBox.Text = "30";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(544, 245);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 178;
            this.label6.Text = "天";
            // 
            // minDurBox
            // 
            this.minDurBox.Location = new System.Drawing.Point(496, 451);
            this.minDurBox.Name = "minDurBox";
            this.minDurBox.Size = new System.Drawing.Size(79, 21);
            this.minDurBox.TabIndex = 180;
            this.minDurBox.Text = "5";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(496, 430);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 179;
            this.label7.Text = "最短持续时间：";
            // 
            // SstProcessExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 548);
            this.Controls.Add(this.minDurBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.timecellBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.moveDownBtn2);
            this.Controls.Add(this.moveUpBtn2);
            this.Controls.Add(this.deleteFileBtn2);
            this.Controls.Add(this.addFileBtn2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.countTextBox2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SelectSavepathBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.countTextBox);
            this.Controls.Add(this.moveDownBtn);
            this.Controls.Add(this.moveUpBtn);
            this.Controls.Add(this.deleteFileBtn);
            this.Controls.Add(this.addFileBtn);
            this.Controls.Add(this.threadCountTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SavePathComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Name = "SstProcessExtractForm";
            this.Text = "SstProcessExtractForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.TextBox threadCountTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox SavePathComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button SelectSavepathBtn;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox countTextBox2;
        private System.Windows.Forms.Button moveDownBtn2;
        private System.Windows.Forms.Button moveUpBtn2;
        private System.Windows.Forms.Button deleteFileBtn2;
        private System.Windows.Forms.Button addFileBtn2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox timecellBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox minDurBox;
        private System.Windows.Forms.Label label7;
    }
}