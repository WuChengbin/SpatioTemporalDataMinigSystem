namespace MarineSTMiningSystem
{
    partial class MarineHeatwavesTrackingForm
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
            this.threadCountTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.eventStateRelationComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableNameComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.eventComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.selectButton2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.selectButton1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.selectButton3 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // threadCountTextBox
            // 
            this.threadCountTextBox.Location = new System.Drawing.Point(417, 110);
            this.threadCountTextBox.Name = "threadCountTextBox";
            this.threadCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.threadCountTextBox.TabIndex = 138;
            this.threadCountTextBox.Text = "1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 135;
            this.label3.Text = "事件表：";
            // 
            // eventStateRelationComboBox
            // 
            this.eventStateRelationComboBox.FormattingEnabled = true;
            this.eventStateRelationComboBox.Location = new System.Drawing.Point(118, 155);
            this.eventStateRelationComboBox.Name = "eventStateRelationComboBox";
            this.eventStateRelationComboBox.Size = new System.Drawing.Size(293, 25);
            this.eventStateRelationComboBox.TabIndex = 134;
            this.eventStateRelationComboBox.Text = "EVENT_STATE_RELATION";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 17);
            this.label2.TabIndex = 133;
            this.label2.Text = "事件状态关联表：";
            // 
            // tableNameComboBox
            // 
            this.tableNameComboBox.FormattingEnabled = true;
            this.tableNameComboBox.Location = new System.Drawing.Point(118, 124);
            this.tableNameComboBox.Name = "tableNameComboBox";
            this.tableNameComboBox.Size = new System.Drawing.Size(293, 25);
            this.tableNameComboBox.TabIndex = 132;
            this.tableNameComboBox.Text = "EVENT_STATE";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(32, 127);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 17);
            this.label13.TabIndex = 131;
            this.label13.Text = "事件状态表：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(11, 186);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(344, 23);
            this.progressBar1.TabIndex = 130;
            this.progressBar1.Visible = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(417, 186);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 129;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(361, 186);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 128;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // eventComboBox
            // 
            this.eventComboBox.FormattingEnabled = true;
            this.eventComboBox.Location = new System.Drawing.Point(118, 93);
            this.eventComboBox.Name = "eventComboBox";
            this.eventComboBox.Size = new System.Drawing.Size(293, 25);
            this.eventComboBox.TabIndex = 136;
            this.eventComboBox.Text = "EVENT";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(414, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 137;
            this.label4.Text = "线程数：";
            // 
            // selectButton2
            // 
            this.selectButton2.Location = new System.Drawing.Point(417, 35);
            this.selectButton2.Name = "selectButton2";
            this.selectButton2.Size = new System.Drawing.Size(50, 23);
            this.selectButton2.TabIndex = 144;
            this.selectButton2.Text = "选择";
            this.selectButton2.UseVisualStyleBackColor = true;
            this.selectButton2.Click += new System.EventHandler(this.selectButton2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 143;
            this.label1.Text = "u文件夹：";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(98, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(313, 23);
            this.textBox2.TabIndex = 142;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(313, 23);
            this.textBox1.TabIndex = 141;
            // 
            // selectButton1
            // 
            this.selectButton1.Location = new System.Drawing.Point(417, 6);
            this.selectButton1.Name = "selectButton1";
            this.selectButton1.Size = new System.Drawing.Size(50, 23);
            this.selectButton1.TabIndex = 140;
            this.selectButton1.Text = "选择";
            this.selectButton1.UseVisualStyleBackColor = true;
            this.selectButton1.Click += new System.EventHandler(this.selectButton1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 17);
            this.label5.TabIndex = 139;
            this.label5.Text = "矢量文件夹：";
            // 
            // selectButton3
            // 
            this.selectButton3.Location = new System.Drawing.Point(417, 64);
            this.selectButton3.Name = "selectButton3";
            this.selectButton3.Size = new System.Drawing.Size(50, 23);
            this.selectButton3.TabIndex = 147;
            this.selectButton3.Text = "选择";
            this.selectButton3.UseVisualStyleBackColor = true;
            this.selectButton3.Click += new System.EventHandler(this.selectButton3_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 67);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 17);
            this.label6.TabIndex = 146;
            this.label6.Text = "v文件夹：";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(98, 64);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(313, 23);
            this.textBox3.TabIndex = 145;
            // 
            // MarineHeatwavesTrackingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 220);
            this.Controls.Add(this.selectButton3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.selectButton2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.selectButton1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.threadCountTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.eventComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.eventStateRelationComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableNameComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MarineHeatwavesTrackingForm";
            this.Text = "MarineHeatwavesTrackingForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox threadCountTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox eventStateRelationComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tableNameComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.ComboBox eventComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button selectButton2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button selectButton1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button selectButton3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox3;
    }
}