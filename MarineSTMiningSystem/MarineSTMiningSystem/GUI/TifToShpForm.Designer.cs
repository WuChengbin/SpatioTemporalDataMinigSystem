namespace MarineSTMiningSystem
{
    partial class TifToShpForm
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
            this.cancelBtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.openBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.okBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.idCountTextBox = new System.Windows.Forms.TextBox();
            this.idMoveDownBtn = new System.Windows.Forms.Button();
            this.idMoveUpBtn = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.idDeleteFileBtn = new System.Windows.Forms.Button();
            this.idAddFileBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.oriCountTextBox = new System.Windows.Forms.TextBox();
            this.oriMoveDownBtn = new System.Windows.Forms.Button();
            this.oriMoveUpBtn = new System.Windows.Forms.Button();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.oriDeleteFileBtn = new System.Windows.Forms.Button();
            this.oriAddFileBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.timeCellTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.valueScaleTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.threadTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(909, 371);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 62;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(500, 371);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(347, 23);
            this.progressBar1.TabIndex = 61;
            this.progressBar1.Visible = false;
            // 
            // openBtn
            // 
            this.openBtn.Location = new System.Drawing.Point(909, 342);
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
            this.label2.Location = new System.Drawing.Point(500, 345);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 17);
            this.label2.TabIndex = 59;
            this.label2.Text = "输出文件夹：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(586, 342);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(317, 23);
            this.textBox1.TabIndex = 58;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(853, 371);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 57;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(412, 133);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 56;
            this.label1.Text = "个数";
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Location = new System.Drawing.Point(412, 153);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(50, 23);
            this.countTextBox.TabIndex = 55;
            this.countTextBox.Text = "0";
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(412, 107);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.moveDownBtn.TabIndex = 54;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(412, 78);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.moveUpBtn.TabIndex = 53;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(6, 21);
            this.listBox1.Name = "listBox1";
            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(400, 157);
            this.listBox1.TabIndex = 52;
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Location = new System.Drawing.Point(412, 49);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.deleteFileBtn.TabIndex = 51;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(412, 20);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(50, 23);
            this.addFileBtn.TabIndex = 50;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(412, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 17);
            this.label3.TabIndex = 77;
            this.label3.Text = "个数";
            // 
            // idCountTextBox
            // 
            this.idCountTextBox.BackColor = System.Drawing.Color.White;
            this.idCountTextBox.Location = new System.Drawing.Point(412, 154);
            this.idCountTextBox.Name = "idCountTextBox";
            this.idCountTextBox.ReadOnly = true;
            this.idCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.idCountTextBox.TabIndex = 76;
            this.idCountTextBox.Text = "0";
            // 
            // idMoveDownBtn
            // 
            this.idMoveDownBtn.Location = new System.Drawing.Point(412, 108);
            this.idMoveDownBtn.Name = "idMoveDownBtn";
            this.idMoveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.idMoveDownBtn.TabIndex = 75;
            this.idMoveDownBtn.Text = "下移";
            this.idMoveDownBtn.UseVisualStyleBackColor = true;
            this.idMoveDownBtn.Click += new System.EventHandler(this.idMoveDownBtn_Click);
            // 
            // idMoveUpBtn
            // 
            this.idMoveUpBtn.Location = new System.Drawing.Point(412, 79);
            this.idMoveUpBtn.Name = "idMoveUpBtn";
            this.idMoveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.idMoveUpBtn.TabIndex = 74;
            this.idMoveUpBtn.Text = "上移";
            this.idMoveUpBtn.UseVisualStyleBackColor = true;
            this.idMoveUpBtn.Click += new System.EventHandler(this.idMoveUpBtn_Click);
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.HorizontalScrollbar = true;
            this.listBox2.ItemHeight = 17;
            this.listBox2.Location = new System.Drawing.Point(6, 22);
            this.listBox2.Name = "listBox2";
            this.listBox2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox2.Size = new System.Drawing.Size(400, 157);
            this.listBox2.TabIndex = 73;
            // 
            // idDeleteFileBtn
            // 
            this.idDeleteFileBtn.Location = new System.Drawing.Point(412, 50);
            this.idDeleteFileBtn.Name = "idDeleteFileBtn";
            this.idDeleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.idDeleteFileBtn.TabIndex = 72;
            this.idDeleteFileBtn.Text = "删除";
            this.idDeleteFileBtn.UseVisualStyleBackColor = true;
            this.idDeleteFileBtn.Click += new System.EventHandler(this.idDeleteFileBtn_Click);
            // 
            // idAddFileBtn
            // 
            this.idAddFileBtn.Location = new System.Drawing.Point(412, 21);
            this.idAddFileBtn.Name = "idAddFileBtn";
            this.idAddFileBtn.Size = new System.Drawing.Size(50, 23);
            this.idAddFileBtn.TabIndex = 71;
            this.idAddFileBtn.Text = "添加";
            this.idAddFileBtn.UseVisualStyleBackColor = true;
            this.idAddFileBtn.Click += new System.EventHandler(this.idAddFileBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(412, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 17);
            this.label4.TabIndex = 84;
            this.label4.Text = "个数";
            // 
            // oriCountTextBox
            // 
            this.oriCountTextBox.BackColor = System.Drawing.Color.White;
            this.oriCountTextBox.Location = new System.Drawing.Point(412, 154);
            this.oriCountTextBox.Name = "oriCountTextBox";
            this.oriCountTextBox.ReadOnly = true;
            this.oriCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.oriCountTextBox.TabIndex = 83;
            this.oriCountTextBox.Text = "0";
            // 
            // oriMoveDownBtn
            // 
            this.oriMoveDownBtn.Location = new System.Drawing.Point(412, 108);
            this.oriMoveDownBtn.Name = "oriMoveDownBtn";
            this.oriMoveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.oriMoveDownBtn.TabIndex = 82;
            this.oriMoveDownBtn.Text = "下移";
            this.oriMoveDownBtn.UseVisualStyleBackColor = true;
            this.oriMoveDownBtn.Click += new System.EventHandler(this.oriMoveDownBtn_Click);
            // 
            // oriMoveUpBtn
            // 
            this.oriMoveUpBtn.Location = new System.Drawing.Point(412, 79);
            this.oriMoveUpBtn.Name = "oriMoveUpBtn";
            this.oriMoveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.oriMoveUpBtn.TabIndex = 81;
            this.oriMoveUpBtn.Text = "上移";
            this.oriMoveUpBtn.UseVisualStyleBackColor = true;
            this.oriMoveUpBtn.Click += new System.EventHandler(this.oriMoveUpBtn_Click);
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.HorizontalScrollbar = true;
            this.listBox3.ItemHeight = 17;
            this.listBox3.Location = new System.Drawing.Point(6, 22);
            this.listBox3.Name = "listBox3";
            this.listBox3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox3.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox3.Size = new System.Drawing.Size(400, 157);
            this.listBox3.TabIndex = 80;
            // 
            // oriDeleteFileBtn
            // 
            this.oriDeleteFileBtn.Location = new System.Drawing.Point(412, 50);
            this.oriDeleteFileBtn.Name = "oriDeleteFileBtn";
            this.oriDeleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.oriDeleteFileBtn.TabIndex = 79;
            this.oriDeleteFileBtn.Text = "删除";
            this.oriDeleteFileBtn.UseVisualStyleBackColor = true;
            this.oriDeleteFileBtn.Click += new System.EventHandler(this.oriDeleteFileBtn_Click);
            // 
            // oriAddFileBtn
            // 
            this.oriAddFileBtn.Location = new System.Drawing.Point(412, 21);
            this.oriAddFileBtn.Name = "oriAddFileBtn";
            this.oriAddFileBtn.Size = new System.Drawing.Size(50, 23);
            this.oriAddFileBtn.TabIndex = 78;
            this.oriAddFileBtn.Text = "添加";
            this.oriAddFileBtn.UseVisualStyleBackColor = true;
            this.oriAddFileBtn.Click += new System.EventHandler(this.oriAddFileBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listBox3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.oriAddFileBtn);
            this.groupBox1.Controls.Add(this.oriCountTextBox);
            this.groupBox1.Controls.Add(this.oriDeleteFileBtn);
            this.groupBox1.Controls.Add(this.oriMoveDownBtn);
            this.groupBox1.Controls.Add(this.oriMoveUpBtn);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(478, 200);
            this.groupBox1.TabIndex = 85;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "原始图像";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox2);
            this.groupBox2.Controls.Add(this.idAddFileBtn);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.idDeleteFileBtn);
            this.groupBox2.Controls.Add(this.idCountTextBox);
            this.groupBox2.Controls.Add(this.idMoveUpBtn);
            this.groupBox2.Controls.Add(this.idMoveDownBtn);
            this.groupBox2.Location = new System.Drawing.Point(12, 217);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(478, 200);
            this.groupBox2.TabIndex = 86;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "编号图像";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBox1);
            this.groupBox3.Controls.Add(this.addFileBtn);
            this.groupBox3.Controls.Add(this.deleteFileBtn);
            this.groupBox3.Controls.Add(this.moveUpBtn);
            this.groupBox3.Controls.Add(this.moveDownBtn);
            this.groupBox3.Controls.Add(this.countTextBox);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Location = new System.Drawing.Point(497, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(478, 200);
            this.groupBox3.TabIndex = 87;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "空间图像";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(748, 316);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 17);
            this.label5.TabIndex = 92;
            this.label5.Text = "小时";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(618, 316);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 17);
            this.label8.TabIndex = 91;
            this.label8.Text = "时间尺度：";
            // 
            // timeCellTextBox
            // 
            this.timeCellTextBox.BackColor = System.Drawing.Color.White;
            this.timeCellTextBox.Location = new System.Drawing.Point(692, 313);
            this.timeCellTextBox.Name = "timeCellTextBox";
            this.timeCellTextBox.Size = new System.Drawing.Size(50, 23);
            this.timeCellTextBox.TabIndex = 90;
            this.timeCellTextBox.Text = "0.5";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(500, 316);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 17);
            this.label9.TabIndex = 89;
            this.label9.Text = "值系数：";
            // 
            // valueScaleTextBox
            // 
            this.valueScaleTextBox.BackColor = System.Drawing.Color.White;
            this.valueScaleTextBox.Location = new System.Drawing.Point(562, 313);
            this.valueScaleTextBox.Name = "valueScaleTextBox";
            this.valueScaleTextBox.Size = new System.Drawing.Size(50, 23);
            this.valueScaleTextBox.TabIndex = 88;
            this.valueScaleTextBox.Text = "0.5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(791, 316);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 17);
            this.label6.TabIndex = 93;
            this.label6.Text = "线程数：";
            // 
            // threadTextBox
            // 
            this.threadTextBox.BackColor = System.Drawing.Color.White;
            this.threadTextBox.Location = new System.Drawing.Point(853, 313);
            this.threadTextBox.Name = "threadTextBox";
            this.threadTextBox.Size = new System.Drawing.Size(50, 23);
            this.threadTextBox.TabIndex = 94;
            this.threadTextBox.Text = "4";
            // 
            // TifToShpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 426);
            this.Controls.Add(this.threadTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.timeCellTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.valueScaleTextBox);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.openBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.okBtn);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "TifToShpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tif转Shp";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button openBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox idCountTextBox;
        private System.Windows.Forms.Button idMoveDownBtn;
        private System.Windows.Forms.Button idMoveUpBtn;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button idDeleteFileBtn;
        private System.Windows.Forms.Button idAddFileBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox oriCountTextBox;
        private System.Windows.Forms.Button oriMoveDownBtn;
        private System.Windows.Forms.Button oriMoveUpBtn;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Button oriDeleteFileBtn;
        private System.Windows.Forms.Button oriAddFileBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox timeCellTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox valueScaleTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox threadTextBox;
    }
}