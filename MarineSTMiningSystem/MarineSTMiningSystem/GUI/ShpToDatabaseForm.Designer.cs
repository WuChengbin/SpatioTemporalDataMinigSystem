﻿namespace MarineSTMiningSystem
{
    partial class ShpToDatabaseForm
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
            this.tableComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.okBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.geoTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tableComboBox
            // 
            this.tableComboBox.FormattingEnabled = true;
            this.tableComboBox.Location = new System.Drawing.Point(50, 209);
            this.tableComboBox.Name = "tableComboBox";
            this.tableComboBox.Size = new System.Drawing.Size(218, 25);
            this.tableComboBox.TabIndex = 153;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 17);
            this.label3.TabIndex = 152;
            this.label3.Text = "表名：";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 240);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(344, 23);
            this.progressBar1.TabIndex = 147;
            this.progressBar1.Visible = false;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(418, 240);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 146;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(362, 240);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 145;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(418, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 144;
            this.label1.Text = "个数";
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Location = new System.Drawing.Point(418, 180);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(50, 23);
            this.countTextBox.TabIndex = 143;
            this.countTextBox.Text = "0";
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(418, 98);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.moveDownBtn.TabIndex = 142;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(418, 69);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.moveUpBtn.TabIndex = 141;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
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
            this.listBox1.TabIndex = 140;
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Location = new System.Drawing.Point(418, 40);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.deleteFileBtn.TabIndex = 139;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(418, 11);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(50, 23);
            this.addFileBtn.TabIndex = 138;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(330, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 17);
            this.label2.TabIndex = 154;
            this.label2.Text = "空间字段名称：";
            // 
            // geoTextBox
            // 
            this.geoTextBox.BackColor = System.Drawing.Color.White;
            this.geoTextBox.Location = new System.Drawing.Point(418, 209);
            this.geoTextBox.Name = "geoTextBox";
            this.geoTextBox.ReadOnly = true;
            this.geoTextBox.Size = new System.Drawing.Size(50, 23);
            this.geoTextBox.TabIndex = 155;
            this.geoTextBox.Text = "space";
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(274, 209);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(50, 23);
            this.clearButton.TabIndex = 156;
            this.clearButton.Text = "清空";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // ShpToDatabaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 269);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.geoTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tableComboBox);
            this.Controls.Add(this.label3);
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
            this.Name = "ShpToDatabaseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shp导入数据库";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox tableComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox geoTextBox;
        private System.Windows.Forms.Button clearButton;
    }
}