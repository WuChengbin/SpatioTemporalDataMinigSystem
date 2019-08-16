﻿namespace MarineSTMiningSystem
{
    partial class StormProcessExtractForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.tipLabel = new System.Windows.Forms.Label();
            this.eventComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.eventStateRelationComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tableNameComboBox = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // threadCountTextBox
            // 
            this.threadCountTextBox.Location = new System.Drawing.Point(418, 226);
            this.threadCountTextBox.Name = "threadCountTextBox";
            this.threadCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.threadCountTextBox.TabIndex = 120;
            this.threadCountTextBox.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(415, 206);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 119;
            this.label4.Text = "线程数：";
            // 
            // tipLabel
            // 
            this.tipLabel.AutoSize = true;
            this.tipLabel.Location = new System.Drawing.Point(12, 334);
            this.tipLabel.Name = "tipLabel";
            this.tipLabel.Size = new System.Drawing.Size(0, 17);
            this.tipLabel.TabIndex = 118;
            // 
            // eventComboBox
            // 
            this.eventComboBox.FormattingEnabled = true;
            this.eventComboBox.Location = new System.Drawing.Point(119, 209);
            this.eventComboBox.Name = "eventComboBox";
            this.eventComboBox.Size = new System.Drawing.Size(293, 25);
            this.eventComboBox.TabIndex = 117;
            this.eventComboBox.Text = "EVENT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 116;
            this.label3.Text = "事件表：";
            // 
            // eventStateRelationComboBox
            // 
            this.eventStateRelationComboBox.FormattingEnabled = true;
            this.eventStateRelationComboBox.Location = new System.Drawing.Point(119, 271);
            this.eventStateRelationComboBox.Name = "eventStateRelationComboBox";
            this.eventStateRelationComboBox.Size = new System.Drawing.Size(293, 25);
            this.eventStateRelationComboBox.TabIndex = 113;
            this.eventStateRelationComboBox.Text = "EVENT_STATE_RELATION";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 274);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 17);
            this.label2.TabIndex = 112;
            this.label2.Text = "事件状态关联表：";
            // 
            // tableNameComboBox
            // 
            this.tableNameComboBox.FormattingEnabled = true;
            this.tableNameComboBox.Location = new System.Drawing.Point(119, 240);
            this.tableNameComboBox.Name = "tableNameComboBox";
            this.tableNameComboBox.Size = new System.Drawing.Size(293, 25);
            this.tableNameComboBox.TabIndex = 111;
            this.tableNameComboBox.Text = "EVENT_STATE";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(33, 243);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(80, 17);
            this.label13.TabIndex = 110;
            this.label13.Text = "事件状态表：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 302);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(344, 23);
            this.progressBar1.TabIndex = 109;
            this.progressBar1.Visible = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(418, 302);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 108;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(362, 302);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 107;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(418, 11);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(50, 23);
            this.addFileBtn.TabIndex = 100;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Location = new System.Drawing.Point(418, 40);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.deleteFileBtn.TabIndex = 101;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 17;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(400, 191);
            this.listBox1.TabIndex = 102;
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(418, 69);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.moveUpBtn.TabIndex = 103;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(418, 98);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.moveDownBtn.TabIndex = 104;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Location = new System.Drawing.Point(418, 180);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(50, 23);
            this.countTextBox.TabIndex = 105;
            this.countTextBox.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(418, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 106;
            this.label1.Text = "个数";
            // 
            // StormProcessExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 331);
            this.Controls.Add(this.threadCountTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tipLabel);
            this.Controls.Add(this.eventComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.eventStateRelationComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableNameComboBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.countTextBox);
            this.Controls.Add(this.moveDownBtn);
            this.Controls.Add(this.moveUpBtn);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.deleteFileBtn);
            this.Controls.Add(this.addFileBtn);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "StormProcessExtractForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "基于过程的暴雨提取";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox threadCountTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label tipLabel;
        private System.Windows.Forms.ComboBox eventComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox eventStateRelationComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tableNameComboBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Label label1;
    }
}