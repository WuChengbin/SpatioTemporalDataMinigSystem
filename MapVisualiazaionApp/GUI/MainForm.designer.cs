namespace MarineSTMiningSystem.GUI
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.连接数据库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.连接数据库ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.关闭连接ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.时空数据查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于我们ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelState = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabelLat = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLon = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.移除图层ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.ExitPlay = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.PauseBtn = new System.Windows.Forms.Button();
            this.StopBtn = new System.Windows.Forms.Button();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.LastBtn = new System.Windows.Forms.Button();
            this.StartLabel = new System.Windows.Forms.Label();
            this.EndLabel = new System.Windows.Forms.Label();
            this.VisualizationMapControl = new ESRI.ArcGIS.Controls.AxMapControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.axToolbarControl1 = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VisualizationMapControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.连接数据库ToolStripMenuItem,
            this.时空数据查询ToolStripMenuItem,
            this.关于我们ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1020, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 连接数据库ToolStripMenuItem
            // 
            this.连接数据库ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.连接数据库ToolStripMenuItem1,
            this.toolStripSeparator1,
            this.关闭连接ToolStripMenuItem});
            this.连接数据库ToolStripMenuItem.Name = "连接数据库ToolStripMenuItem";
            this.连接数据库ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.连接数据库ToolStripMenuItem.Text = "数据库管理";
            this.连接数据库ToolStripMenuItem.Click += new System.EventHandler(this.连接数据库ToolStripMenuItem_Click);
            // 
            // 连接数据库ToolStripMenuItem1
            // 
            this.连接数据库ToolStripMenuItem1.Name = "连接数据库ToolStripMenuItem1";
            this.连接数据库ToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.连接数据库ToolStripMenuItem1.Text = "连接数据库";
            this.连接数据库ToolStripMenuItem1.Click += new System.EventHandler(this.连接数据库ToolStripMenuItem1_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // 关闭连接ToolStripMenuItem
            // 
            this.关闭连接ToolStripMenuItem.Name = "关闭连接ToolStripMenuItem";
            this.关闭连接ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.关闭连接ToolStripMenuItem.Text = "关闭连接";
            this.关闭连接ToolStripMenuItem.Click += new System.EventHandler(this.关闭连接ToolStripMenuItem_Click);
            // 
            // 时空数据查询ToolStripMenuItem
            // 
            this.时空数据查询ToolStripMenuItem.Name = "时空数据查询ToolStripMenuItem";
            this.时空数据查询ToolStripMenuItem.Size = new System.Drawing.Size(116, 21);
            this.时空数据查询ToolStripMenuItem.Text = "时空数据统计分析";
            this.时空数据查询ToolStripMenuItem.Click += new System.EventHandler(this.时空数据查询ToolStripMenuItem_Click);
            // 
            // 关于我们ToolStripMenuItem
            // 
            this.关于我们ToolStripMenuItem.Name = "关于我们ToolStripMenuItem";
            this.关于我们ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.关于我们ToolStripMenuItem.Text = "关于我们";
            this.关于我们ToolStripMenuItem.Click += new System.EventHandler(this.关于我们ToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelState,
            this.toolStripProgressBar1,
            this.toolStripStatusLabelLat,
            this.toolStripStatusLabelLon});
            this.statusStrip1.Location = new System.Drawing.Point(0, 555);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1020, 25);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelState
            // 
            this.toolStripStatusLabelState.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.toolStripStatusLabelState.Name = "toolStripStatusLabelState";
            this.toolStripStatusLabelState.Size = new System.Drawing.Size(230, 20);
            this.toolStripStatusLabelState.Text = "海洋数据挖掘与分析系统（V1.2.0）";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(200, 19);
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabelLat
            // 
            this.toolStripStatusLabelLat.Name = "toolStripStatusLabelLat";
            this.toolStripStatusLabelLat.Size = new System.Drawing.Size(25, 20);
            this.toolStripStatusLabelLat.Text = "Lat";
            this.toolStripStatusLabelLat.Visible = false;
            // 
            // toolStripStatusLabelLon
            // 
            this.toolStripStatusLabelLon.Name = "toolStripStatusLabelLon";
            this.toolStripStatusLabelLon.Size = new System.Drawing.Size(29, 20);
            this.toolStripStatusLabelLon.Text = "Lon";
            this.toolStripStatusLabelLon.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.移除图层ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 26);
            // 
            // 移除图层ToolStripMenuItem
            // 
            this.移除图层ToolStripMenuItem.Name = "移除图层ToolStripMenuItem";
            this.移除图层ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.移除图层ToolStripMenuItem.Text = "移除图层";
            this.移除图层ToolStripMenuItem.Click += new System.EventHandler(this.移除图层ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.ExitPlay});
            this.toolStrip1.Location = new System.Drawing.Point(0, 53);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(1020, 25);
            this.toolStrip1.TabIndex = 18;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.Visible = false;
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "清除要素";
            this.toolStripButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // ExitPlay
            // 
            this.ExitPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExitPlay.Image = ((System.Drawing.Image)(resources.GetObject("ExitPlay.Image")));
            this.ExitPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExitPlay.Name = "ExitPlay";
            this.ExitPlay.Size = new System.Drawing.Size(23, 22);
            this.ExitPlay.Text = "退出播放";
            this.ExitPlay.Click += new System.EventHandler(this.ExitPlay_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 53);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl2);
            this.splitContainer1.Size = new System.Drawing.Size(1020, 502);
            this.splitContainer1.SplitterDistance = 135;
            this.splitContainer1.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(135, 502);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.axLicenseControl1);
            this.tabPage2.Controls.Add(this.axTOCControl1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(127, 476);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据框";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(6, 438);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 1;
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl1.Location = new System.Drawing.Point(3, 3);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(121, 470);
            this.axTOCControl1.TabIndex = 0;
            this.axTOCControl1.OnMouseDown += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseDownEventHandler(this.axTOCControl1_OnMouseDown);
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(881, 502);
            this.tabControl2.TabIndex = 40;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.PauseBtn);
            this.tabPage1.Controls.Add(this.StopBtn);
            this.tabPage1.Controls.Add(this.PlayBtn);
            this.tabPage1.Controls.Add(this.NextBtn);
            this.tabPage1.Controls.Add(this.trackBar1);
            this.tabPage1.Controls.Add(this.LastBtn);
            this.tabPage1.Controls.Add(this.StartLabel);
            this.tabPage1.Controls.Add(this.EndLabel);
            this.tabPage1.Controls.Add(this.VisualizationMapControl);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(873, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "数据可视化";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // PauseBtn
            // 
            this.PauseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PauseBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PauseBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PauseBtn.BackgroundImage")));
            this.PauseBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PauseBtn.FlatAppearance.BorderSize = 0;
            this.PauseBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PauseBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PauseBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PauseBtn.ForeColor = System.Drawing.Color.Transparent;
            this.PauseBtn.Location = new System.Drawing.Point(737, 441);
            this.PauseBtn.Name = "PauseBtn";
            this.PauseBtn.Size = new System.Drawing.Size(42, 32);
            this.PauseBtn.TabIndex = 48;
            this.PauseBtn.UseVisualStyleBackColor = false;
            this.PauseBtn.Visible = false;
            this.PauseBtn.Click += new System.EventHandler(this.button3_Click);
            // 
            // StopBtn
            // 
            this.StopBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StopBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.StopBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("StopBtn.BackgroundImage")));
            this.StopBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.StopBtn.FlatAppearance.BorderSize = 0;
            this.StopBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.StopBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.StopBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.StopBtn.ForeColor = System.Drawing.Color.Transparent;
            this.StopBtn.Location = new System.Drawing.Point(780, 441);
            this.StopBtn.Name = "StopBtn";
            this.StopBtn.Size = new System.Drawing.Size(42, 32);
            this.StopBtn.TabIndex = 47;
            this.StopBtn.UseVisualStyleBackColor = false;
            this.StopBtn.Visible = false;
            this.StopBtn.Click += new System.EventHandler(this.button2_Click);
            // 
            // PlayBtn
            // 
            this.PlayBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.PlayBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PlayBtn.BackgroundImage")));
            this.PlayBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PlayBtn.FlatAppearance.BorderSize = 0;
            this.PlayBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.PlayBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.PlayBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PlayBtn.ForeColor = System.Drawing.Color.Transparent;
            this.PlayBtn.Location = new System.Drawing.Point(737, 441);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(42, 32);
            this.PlayBtn.TabIndex = 46;
            this.PlayBtn.UseVisualStyleBackColor = false;
            this.PlayBtn.Visible = false;
            this.PlayBtn.Click += new System.EventHandler(this.button1_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NextBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NextBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("NextBtn.BackgroundImage")));
            this.NextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.NextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.NextBtn.Location = new System.Drawing.Point(823, 441);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(42, 32);
            this.NextBtn.TabIndex = 45;
            this.NextBtn.UseVisualStyleBackColor = false;
            this.NextBtn.Visible = false;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar1.AutoSize = false;
            this.trackBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackBar1.Location = new System.Drawing.Point(5, 441);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(688, 32);
            this.trackBar1.TabIndex = 41;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar1.Visible = false;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // LastBtn
            // 
            this.LastBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LastBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LastBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("LastBtn.BackgroundImage")));
            this.LastBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.LastBtn.FlatAppearance.BorderSize = 0;
            this.LastBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LastBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LastBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LastBtn.ForeColor = System.Drawing.Color.Transparent;
            this.LastBtn.Location = new System.Drawing.Point(694, 441);
            this.LastBtn.Name = "LastBtn";
            this.LastBtn.Size = new System.Drawing.Size(42, 32);
            this.LastBtn.TabIndex = 44;
            this.LastBtn.UseVisualStyleBackColor = false;
            this.LastBtn.Visible = false;
            this.LastBtn.Click += new System.EventHandler(this.LastBtn_Click);
            // 
            // StartLabel
            // 
            this.StartLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartLabel.AutoSize = true;
            this.StartLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.StartLabel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartLabel.ForeColor = System.Drawing.Color.White;
            this.StartLabel.Location = new System.Drawing.Point(5, 426);
            this.StartLabel.Name = "StartLabel";
            this.StartLabel.Size = new System.Drawing.Size(87, 14);
            this.StartLabel.TabIndex = 42;
            this.StartLabel.Text = "StartLabel";
            this.StartLabel.Visible = false;
            // 
            // EndLabel
            // 
            this.EndLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EndLabel.AutoSize = true;
            this.EndLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.EndLabel.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.EndLabel.ForeColor = System.Drawing.Color.White;
            this.EndLabel.Location = new System.Drawing.Point(534, 426);
            this.EndLabel.Name = "EndLabel";
            this.EndLabel.Size = new System.Drawing.Size(71, 14);
            this.EndLabel.TabIndex = 43;
            this.EndLabel.Text = "EndLabel";
            this.EndLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.EndLabel.Visible = false;
            // 
            // VisualizationMapControl
            // 
            this.VisualizationMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisualizationMapControl.Location = new System.Drawing.Point(3, 3);
            this.VisualizationMapControl.Name = "VisualizationMapControl";
            this.VisualizationMapControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("VisualizationMapControl.OcxState")));
            this.VisualizationMapControl.Size = new System.Drawing.Size(867, 470);
            this.VisualizationMapControl.TabIndex = 40;
            this.VisualizationMapControl.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.VisualizationMapControl_OnMouseDown);
            this.VisualizationMapControl.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.VisualizationMapControl_OnMouseMove);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(873, 476);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "关系可视化";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // axToolbarControl1
            // 
            this.axToolbarControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl1.Location = new System.Drawing.Point(0, 25);
            this.axToolbarControl1.Name = "axToolbarControl1";
            this.axToolbarControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl1.OcxState")));
            this.axToolbarControl1.Size = new System.Drawing.Size(1020, 28);
            this.axToolbarControl1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 580);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.axToolbarControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "海洋数据挖掘与分析系统（V1.2.0）";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VisualizationMapControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 时空数据查询ToolStripMenuItem;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelState;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLat;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLon;
        private System.Windows.Forms.ToolStripMenuItem 关于我们ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 连接数据库ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 移除图层ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 连接数据库ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 关闭连接ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton ExitPlay;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button NextBtn;
        public System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button LastBtn;
        private System.Windows.Forms.Label StartLabel;
        private System.Windows.Forms.Label EndLabel;
        public ESRI.ArcGIS.Controls.AxMapControl VisualizationMapControl;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button PauseBtn;
        private System.Windows.Forms.Button StopBtn;
        private System.Windows.Forms.Button PlayBtn;
    }
}