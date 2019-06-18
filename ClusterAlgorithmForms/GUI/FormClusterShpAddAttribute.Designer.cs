namespace ClusterAlgorithmForms.GUI
{
    partial class FormClusterShpAddAttribute
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
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.oriAddFileBtn = new System.Windows.Forms.Button();
            this.oriCountTextBox = new System.Windows.Forms.TextBox();
            this.oriDeleteFileBtn = new System.Windows.Forms.Button();
            this.oriMoveDownBtn = new System.Windows.Forms.Button();
            this.oriMoveUpBtn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pidAddFileBtn = new System.Windows.Forms.Button();
            this.pidCountTextBox = new System.Windows.Forms.TextBox();
            this.pidDeleteFileBtn = new System.Windows.Forms.Button();
            this.pidMoveDownBtn = new System.Windows.Forms.Button();
            this.pidMoveUpBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.addFileBtn = new System.Windows.Forms.Button();
            this.deleteFileBtn = new System.Windows.Forms.Button();
            this.moveUpBtn = new System.Windows.Forms.Button();
            this.moveDownBtn = new System.Windows.Forms.Button();
            this.countTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.okBtn = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
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
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(466, 197);
            this.groupBox1.TabIndex = 118;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "原始图像tiff";
            // 
            // listBox3
            // 
            this.listBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox3.FormattingEnabled = true;
            this.listBox3.HorizontalScrollbar = true;
            this.listBox3.ItemHeight = 17;
            this.listBox3.Location = new System.Drawing.Point(6, 21);
            this.listBox3.Name = "listBox3";
            this.listBox3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox3.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox3.Size = new System.Drawing.Size(400, 157);
            this.listBox3.TabIndex = 80;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(413, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 17);
            this.label4.TabIndex = 84;
            this.label4.Text = "个数";
            // 
            // oriAddFileBtn
            // 
            this.oriAddFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.oriAddFileBtn.Location = new System.Drawing.Point(413, 21);
            this.oriAddFileBtn.Name = "oriAddFileBtn";
            this.oriAddFileBtn.Size = new System.Drawing.Size(50, 23);
            this.oriAddFileBtn.TabIndex = 78;
            this.oriAddFileBtn.Text = "添加";
            this.oriAddFileBtn.UseVisualStyleBackColor = true;
            this.oriAddFileBtn.Click += new System.EventHandler(this.oriAddFileBtn_Click);
            // 
            // oriCountTextBox
            // 
            this.oriCountTextBox.BackColor = System.Drawing.Color.White;
            this.oriCountTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.oriCountTextBox.Location = new System.Drawing.Point(413, 154);
            this.oriCountTextBox.Name = "oriCountTextBox";
            this.oriCountTextBox.ReadOnly = true;
            this.oriCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.oriCountTextBox.TabIndex = 83;
            this.oriCountTextBox.Text = "0";
            // 
            // oriDeleteFileBtn
            // 
            this.oriDeleteFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.oriDeleteFileBtn.Location = new System.Drawing.Point(413, 50);
            this.oriDeleteFileBtn.Name = "oriDeleteFileBtn";
            this.oriDeleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.oriDeleteFileBtn.TabIndex = 79;
            this.oriDeleteFileBtn.Text = "删除";
            this.oriDeleteFileBtn.UseVisualStyleBackColor = true;
            this.oriDeleteFileBtn.Click += new System.EventHandler(this.oriDeleteFileBtn_Click);
            // 
            // oriMoveDownBtn
            // 
            this.oriMoveDownBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.oriMoveDownBtn.Location = new System.Drawing.Point(413, 108);
            this.oriMoveDownBtn.Name = "oriMoveDownBtn";
            this.oriMoveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.oriMoveDownBtn.TabIndex = 82;
            this.oriMoveDownBtn.Text = "下移";
            this.oriMoveDownBtn.UseVisualStyleBackColor = true;
            this.oriMoveDownBtn.Click += new System.EventHandler(this.oriMoveDownBtn_Click);
            // 
            // oriMoveUpBtn
            // 
            this.oriMoveUpBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.oriMoveUpBtn.Location = new System.Drawing.Point(413, 79);
            this.oriMoveUpBtn.Name = "oriMoveUpBtn";
            this.oriMoveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.oriMoveUpBtn.TabIndex = 81;
            this.oriMoveUpBtn.Text = "上移";
            this.oriMoveUpBtn.UseVisualStyleBackColor = true;
            this.oriMoveUpBtn.Click += new System.EventHandler(this.oriMoveUpBtn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox2);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pidAddFileBtn);
            this.groupBox2.Controls.Add(this.pidCountTextBox);
            this.groupBox2.Controls.Add(this.pidDeleteFileBtn);
            this.groupBox2.Controls.Add(this.pidMoveDownBtn);
            this.groupBox2.Controls.Add(this.pidMoveUpBtn);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(466, 235);
            this.groupBox2.TabIndex = 120;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "聚簇结果tiff";
            // 
            // listBox2
            // 
            this.listBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.HorizontalScrollbar = true;
            this.listBox2.ItemHeight = 17;
            this.listBox2.Location = new System.Drawing.Point(6, 21);
            this.listBox2.Name = "listBox2";
            this.listBox2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox2.Size = new System.Drawing.Size(400, 157);
            this.listBox2.TabIndex = 80;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(413, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 17);
            this.label2.TabIndex = 84;
            this.label2.Text = "个数";
            // 
            // pidAddFileBtn
            // 
            this.pidAddFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidAddFileBtn.Location = new System.Drawing.Point(413, 21);
            this.pidAddFileBtn.Name = "pidAddFileBtn";
            this.pidAddFileBtn.Size = new System.Drawing.Size(50, 23);
            this.pidAddFileBtn.TabIndex = 78;
            this.pidAddFileBtn.Text = "添加";
            this.pidAddFileBtn.UseVisualStyleBackColor = true;
            this.pidAddFileBtn.Click += new System.EventHandler(this.pidAddFileBtn_Click);
            // 
            // pidCountTextBox
            // 
            this.pidCountTextBox.BackColor = System.Drawing.Color.White;
            this.pidCountTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidCountTextBox.Location = new System.Drawing.Point(413, 154);
            this.pidCountTextBox.Name = "pidCountTextBox";
            this.pidCountTextBox.ReadOnly = true;
            this.pidCountTextBox.Size = new System.Drawing.Size(50, 23);
            this.pidCountTextBox.TabIndex = 83;
            this.pidCountTextBox.Text = "0";
            // 
            // pidDeleteFileBtn
            // 
            this.pidDeleteFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidDeleteFileBtn.Location = new System.Drawing.Point(413, 50);
            this.pidDeleteFileBtn.Name = "pidDeleteFileBtn";
            this.pidDeleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.pidDeleteFileBtn.TabIndex = 79;
            this.pidDeleteFileBtn.Text = "删除";
            this.pidDeleteFileBtn.UseVisualStyleBackColor = true;
            this.pidDeleteFileBtn.Click += new System.EventHandler(this.pidDeleteFileBtn_Click);
            // 
            // pidMoveDownBtn
            // 
            this.pidMoveDownBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidMoveDownBtn.Location = new System.Drawing.Point(413, 108);
            this.pidMoveDownBtn.Name = "pidMoveDownBtn";
            this.pidMoveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.pidMoveDownBtn.TabIndex = 82;
            this.pidMoveDownBtn.Text = "下移";
            this.pidMoveDownBtn.UseVisualStyleBackColor = true;
            this.pidMoveDownBtn.Click += new System.EventHandler(this.pidMoveDownBtn_Click);
            // 
            // pidMoveUpBtn
            // 
            this.pidMoveUpBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidMoveUpBtn.Location = new System.Drawing.Point(413, 79);
            this.pidMoveUpBtn.Name = "pidMoveUpBtn";
            this.pidMoveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.pidMoveUpBtn.TabIndex = 81;
            this.pidMoveUpBtn.Text = "上移";
            this.pidMoveUpBtn.UseVisualStyleBackColor = true;
            this.pidMoveUpBtn.Click += new System.EventHandler(this.pidMoveDownBtn_Click);
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
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(466, 235);
            this.groupBox3.TabIndex = 121;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "shp图层";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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
            // addFileBtn
            // 
            this.addFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.addFileBtn.Location = new System.Drawing.Point(413, 21);
            this.addFileBtn.Name = "addFileBtn";
            this.addFileBtn.Size = new System.Drawing.Size(50, 23);
            this.addFileBtn.TabIndex = 50;
            this.addFileBtn.Text = "添加";
            this.addFileBtn.UseVisualStyleBackColor = true;
            this.addFileBtn.Click += new System.EventHandler(this.addFileBtn_Click);
            // 
            // deleteFileBtn
            // 
            this.deleteFileBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteFileBtn.Location = new System.Drawing.Point(413, 50);
            this.deleteFileBtn.Name = "deleteFileBtn";
            this.deleteFileBtn.Size = new System.Drawing.Size(50, 23);
            this.deleteFileBtn.TabIndex = 51;
            this.deleteFileBtn.Text = "删除";
            this.deleteFileBtn.UseVisualStyleBackColor = true;
            this.deleteFileBtn.Click += new System.EventHandler(this.deleteFileBtn_Click);
            // 
            // moveUpBtn
            // 
            this.moveUpBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.moveUpBtn.Location = new System.Drawing.Point(413, 79);
            this.moveUpBtn.Name = "moveUpBtn";
            this.moveUpBtn.Size = new System.Drawing.Size(50, 23);
            this.moveUpBtn.TabIndex = 53;
            this.moveUpBtn.Text = "上移";
            this.moveUpBtn.UseVisualStyleBackColor = true;
            this.moveUpBtn.Click += new System.EventHandler(this.moveUpBtn_Click);
            // 
            // moveDownBtn
            // 
            this.moveDownBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.moveDownBtn.Location = new System.Drawing.Point(413, 108);
            this.moveDownBtn.Name = "moveDownBtn";
            this.moveDownBtn.Size = new System.Drawing.Size(50, 23);
            this.moveDownBtn.TabIndex = 54;
            this.moveDownBtn.Text = "下移";
            this.moveDownBtn.UseVisualStyleBackColor = true;
            this.moveDownBtn.Click += new System.EventHandler(this.moveDownBtn_Click);
            // 
            // countTextBox
            // 
            this.countTextBox.BackColor = System.Drawing.Color.White;
            this.countTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.countTextBox.Location = new System.Drawing.Point(413, 154);
            this.countTextBox.Name = "countTextBox";
            this.countTextBox.ReadOnly = true;
            this.countTextBox.Size = new System.Drawing.Size(50, 23);
            this.countTextBox.TabIndex = 55;
            this.countTextBox.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(413, 134);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 17);
            this.label1.TabIndex = 56;
            this.label1.Text = "个数";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cancelBtn.Location = new System.Drawing.Point(431, 236);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(50, 23);
            this.cancelBtn.TabIndex = 124;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 236);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(340, 23);
            this.progressBar1.TabIndex = 123;
            this.progressBar1.Visible = false;
            // 
            // okBtn
            // 
            this.okBtn.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.okBtn.Location = new System.Drawing.Point(375, 236);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(50, 23);
            this.okBtn.TabIndex = 122;
            this.okBtn.Text = "确定";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(1, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(480, 229);
            this.tabControl1.TabIndex = 125;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(472, 203);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "原始图像";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(472, 241);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "聚簇结果";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(472, 241);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "图层";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // FormClusterShpAddAttribute
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 267);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.okBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormClusterShpAddAttribute";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "聚簇多边形添加属性";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button oriAddFileBtn;
        private System.Windows.Forms.TextBox oriCountTextBox;
        private System.Windows.Forms.Button oriDeleteFileBtn;
        private System.Windows.Forms.Button oriMoveDownBtn;
        private System.Windows.Forms.Button oriMoveUpBtn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button pidAddFileBtn;
        private System.Windows.Forms.TextBox pidCountTextBox;
        private System.Windows.Forms.Button pidDeleteFileBtn;
        private System.Windows.Forms.Button pidMoveDownBtn;
        private System.Windows.Forms.Button pidMoveUpBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button addFileBtn;
        private System.Windows.Forms.Button deleteFileBtn;
        private System.Windows.Forms.Button moveUpBtn;
        private System.Windows.Forms.Button moveDownBtn;
        private System.Windows.Forms.TextBox countTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
    }
}