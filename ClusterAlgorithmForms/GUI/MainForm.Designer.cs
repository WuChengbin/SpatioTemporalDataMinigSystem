namespace Forms
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.聚类分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.空间聚类分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时间聚类分析ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.双重约束聚类RoCMSACToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.聚类分析DBSCANToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时空聚类RoSTCMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时空聚类STSNNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时空聚类STDBSCANToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.截取当前图像ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.聚类分析ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(305, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 聚类分析ToolStripMenuItem
            // 
            this.聚类分析ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.空间聚类分析ToolStripMenuItem,
            this.时间聚类分析ToolStripMenuItem,
            this.双重约束聚类RoCMSACToolStripMenuItem,
            this.聚类分析DBSCANToolStripMenuItem,
            this.时空聚类RoSTCMToolStripMenuItem,
            this.时空聚类STSNNToolStripMenuItem,
            this.时空聚类STDBSCANToolStripMenuItem,
            this.截图ToolStripMenuItem,
            this.截取当前图像ToolStripMenuItem});
            this.聚类分析ToolStripMenuItem.Name = "聚类分析ToolStripMenuItem";
            this.聚类分析ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.聚类分析ToolStripMenuItem.Text = "聚类分析";
            // 
            // 空间聚类分析ToolStripMenuItem
            // 
            this.空间聚类分析ToolStripMenuItem.Name = "空间聚类分析ToolStripMenuItem";
            this.空间聚类分析ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.空间聚类分析ToolStripMenuItem.Text = "空间聚类分析";
            this.空间聚类分析ToolStripMenuItem.Click += new System.EventHandler(this.空间聚类分析ToolStripMenuItem_Click);
            // 
            // 时间聚类分析ToolStripMenuItem
            // 
            this.时间聚类分析ToolStripMenuItem.Name = "时间聚类分析ToolStripMenuItem";
            this.时间聚类分析ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.时间聚类分析ToolStripMenuItem.Text = "时间聚类分析";
            // 
            // 双重约束聚类RoCMSACToolStripMenuItem
            // 
            this.双重约束聚类RoCMSACToolStripMenuItem.Name = "双重约束聚类RoCMSACToolStripMenuItem";
            this.双重约束聚类RoCMSACToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.双重约束聚类RoCMSACToolStripMenuItem.Text = "双重约束聚类RoCMSAC";
            this.双重约束聚类RoCMSACToolStripMenuItem.Click += new System.EventHandler(this.双重约束聚类RoCMSACToolStripMenuItem_Click);
            // 
            // 聚类分析DBSCANToolStripMenuItem
            // 
            this.聚类分析DBSCANToolStripMenuItem.Name = "聚类分析DBSCANToolStripMenuItem";
            this.聚类分析DBSCANToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.聚类分析DBSCANToolStripMenuItem.Text = "聚类分析DBSCAN";
            this.聚类分析DBSCANToolStripMenuItem.Click += new System.EventHandler(this.聚类分析DBSCANToolStripMenuItem_Click);
            // 
            // 时空聚类RoSTCMToolStripMenuItem
            // 
            this.时空聚类RoSTCMToolStripMenuItem.Name = "时空聚类RoSTCMToolStripMenuItem";
            this.时空聚类RoSTCMToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.时空聚类RoSTCMToolStripMenuItem.Text = "时空聚类DcSTCA";
            this.时空聚类RoSTCMToolStripMenuItem.Click += new System.EventHandler(this.时空聚类RoSTCMToolStripMenuItem_Click);
            // 
            // 时空聚类STSNNToolStripMenuItem
            // 
            this.时空聚类STSNNToolStripMenuItem.Name = "时空聚类STSNNToolStripMenuItem";
            this.时空聚类STSNNToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.时空聚类STSNNToolStripMenuItem.Text = "时空聚类STSNN";
            // 
            // 时空聚类STDBSCANToolStripMenuItem
            // 
            this.时空聚类STDBSCANToolStripMenuItem.Name = "时空聚类STDBSCANToolStripMenuItem";
            this.时空聚类STDBSCANToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.时空聚类STDBSCANToolStripMenuItem.Text = "时空聚类STDBSCAN";
            // 
            // 截图ToolStripMenuItem
            // 
            this.截图ToolStripMenuItem.Name = "截图ToolStripMenuItem";
            this.截图ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.截图ToolStripMenuItem.Text = "截图";
            this.截图ToolStripMenuItem.Click += new System.EventHandler(this.截图ToolStripMenuItem_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(76, 98);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 1;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(76, 140);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 21);
            this.textBox2.TabIndex = 2;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(76, 178);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 21);
            this.textBox3.TabIndex = 3;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(76, 220);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 21);
            this.textBox4.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "LeftX+";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "LeftY+";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 186);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "HUAN";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "GAO";
            // 
            // 截取当前图像ToolStripMenuItem
            // 
            this.截取当前图像ToolStripMenuItem.Name = "截取当前图像ToolStripMenuItem";
            this.截取当前图像ToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.截取当前图像ToolStripMenuItem.Text = "截取当前图像";
            this.截取当前图像ToolStripMenuItem.Click += new System.EventHandler(this.截取当前图像ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 292);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "聚类分析";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 聚类分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 空间聚类分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时间聚类分析ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 双重约束聚类RoCMSACToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 聚类分析DBSCANToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时空聚类RoSTCMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时空聚类STSNNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 时空聚类STDBSCANToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 截图ToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem 截取当前图像ToolStripMenuItem;
    }
}

