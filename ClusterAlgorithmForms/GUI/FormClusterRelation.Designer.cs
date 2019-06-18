namespace ClusterAlgorithmForms.GUI
{
    partial class FormClusterRelation
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
            this.AnomalyAdd = new System.Windows.Forms.Button();
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
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // AnomalyAdd
            // 
            this.AnomalyAdd.Location = new System.Drawing.Point(221, 251);
            this.AnomalyAdd.Name = "AnomalyAdd";
            this.AnomalyAdd.Size = new System.Drawing.Size(87, 23);
            this.AnomalyAdd.TabIndex = 83;
            this.AnomalyAdd.Text = "打开距平数据";
            this.AnomalyAdd.UseVisualStyleBackColor = true;
            this.AnomalyAdd.Click += new System.EventHandler(this.AnomalyAdd_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(314, 251);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(45, 23);
            this.cancelBtn.TabIndex = 82;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 251);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(201, 23);
            this.progressBar1.TabIndex = 81;
            this.progressBar1.Visible = false;
            // 
            // openBtn
            // 
            this.openBtn.Location = new System.Drawing.Point(373, 214);
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(53, 23);
            this.openBtn.TabIndex = 80;
            this.openBtn.Text = "选择";
            this.openBtn.UseVisualStyleBackColor = true;
            this.openBtn.Click += new System.EventHandler(this.openBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 219);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 79;
            this.label2.Text = "输出文件夹：";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(98, 216);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(261, 21);
            this.textBox1.TabIndex = 78;
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(373, 251);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(53, 23);
            this.okBtn.TabIndex = 77;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(376, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 76;
            this.label1.Text = "个数";
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Location = new System.Drawing.Point(376, 161);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(50, 21);
            this.countTextBox.TabIndex = 75;
            this.countTextBox.Text = "0";
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Location = new System.Drawing.Point(376, 102);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.moveDownBtn.TabIndex = 74;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Location = new System.Drawing.Point(376, 73);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.moveUpBtn.TabIndex = 73;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Location = new System.Drawing.Point(376, 44);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.deleteFileBtn.TabIndex = 72;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // addFileBtn
            // 
            this.addFileBtn.Location = new System.Drawing.Point(376, 15);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(50, 23);
            this.addFileBtn.TabIndex = 71;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(349, 172);
            this.listBox1.TabIndex = 70;
            // 
            // FormClusterRelation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 296);
            this.Controls.Add(this.AnomalyAdd);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.openBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.countTextBox);
            this.Controls.Add(this.moveDownBtn);
            this.Controls.Add(this.moveUpBtn);
            this.Controls.Add(this.deleteFileBtn);
            this.Controls.Add(this.addFileBtn);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormClusterRelation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "聚簇过程组织";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AnomalyAdd;
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
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.ListBox listBox1;
    }
}