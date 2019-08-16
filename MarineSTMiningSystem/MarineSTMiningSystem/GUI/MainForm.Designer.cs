namespace MarineSTMiningSystem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode1});
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点3");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点4");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点2", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4});
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.databaseManagement = new System.Windows.Forms.ToolStripMenuItem();
            this.linkServerDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.linkLocalDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.objectUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.WPWPObjectUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.EPWPObjectUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEventObjectUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOObjectUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceRainfallAnomalyObject = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceAltitudeAnomalyObject = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanColorAnomalyObject = new System.Windows.Forms.ToolStripMenuItem();
            this.SSTAnomalyObject = new System.Windows.Forms.ToolStripMenuItem();
            this.attributeUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEventDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.clusteringObjectDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.STAssociationRulesDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.SSTAnomalyDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanColorAnomalyDatabaseTabelPropertyUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseClose = new System.Windows.Forms.ToolStripMenuItem();
            this.STInfoExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOEvent = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOEventPartition = new System.Windows.Forms.ToolStripMenuItem();
            this.droughtIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.droughOneDimensionTimeSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.droughTwoDimensionRasterSequence = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEvent = new System.Windows.Forms.ToolStripMenuItem();
            this.基于栅格的暴雨提取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stormTimeExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.stormSpatialExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.stormNumberExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.stormRasterExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.stormResterToVector = new System.Windows.Forms.ToolStripMenuItem();
            this.EventExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.基于矢量的暴雨提取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stormTimeExtraction2 = new System.Windows.Forms.ToolStripMenuItem();
            this.stormSpatialExtraction2 = new System.Windows.Forms.ToolStripMenuItem();
            this.stormResterToVectorBasedSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.stormProcessExtract = new System.Windows.Forms.ToolStripMenuItem();
            this.stormTITAN = new System.Windows.Forms.ToolStripMenuItem();
            this.stormTimeExtraction3 = new System.Windows.Forms.ToolStripMenuItem();
            this.stormSpatialExtraction3 = new System.Windows.Forms.ToolStripMenuItem();
            this.stormResterToVectorBasedSpace2 = new System.Windows.Forms.ToolStripMenuItem();
            this.stormTITANExtract = new System.Windows.Forms.ToolStripMenuItem();
            this.OutToTxt = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEventToVector = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEventStateToVector = new System.Windows.Forms.ToolStripMenuItem();
            this.导出为文本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanAnomalySensitiveObject = new System.Windows.Forms.ToolStripMenuItem();
            this.SSTExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceRainfallExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceAltitudeAnomalyExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanPrimaryProductivityExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanSurfaceChlorophylAConcentrationExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.WPWPSTInfoExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.EPWPSTInfoExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.海洋热浪ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesAverage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.marineHeatwavesMedian = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwaves90Percent = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesTimeAnomaly = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesTimeExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesTimeExtraction2 = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesVectorExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesSpeed = new System.Windows.Forms.ToolStripMenuItem();
            this.marineHeatwavesTracking = new System.Windows.Forms.ToolStripMenuItem();
            this.海温事件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.基于矢量的SST提取ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sstTimeExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.sstSpaceExtraction = new System.Windows.Forms.ToolStripMenuItem();
            this.sstResterToVectorBasedSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.sstRestertoVectorExtractInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.sstProcessExtract = new System.Windows.Forms.ToolStripMenuItem();
            this.数据库导出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SstEventToVector = new System.Windows.Forms.ToolStripMenuItem();
            this.SSTEventStateToVector = new System.Windows.Forms.ToolStripMenuItem();
            this.STAnomalyDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterSpatialAnomalyDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialStatisticModel = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialClusteringMining = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialTrendSurfaceSimulation = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterTimeAnomalyDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.timeStatisticModel = new System.Windows.Forms.ToolStripMenuItem();
            this.timeClusteringMining = new System.Windows.Forms.ToolStripMenuItem();
            this.timeAnalogueFunction = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterSTAnomalyDetection = new System.Windows.Forms.ToolStripMenuItem();
            this.STStatisticModel = new System.Windows.Forms.ToolStripMenuItem();
            this.STClusteringMining = new System.Windows.Forms.ToolStripMenuItem();
            this.grid = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterSpatialClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.objectSpatialClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterTimeClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.objectTimeClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.spatialAttributeDoubleConstraintsClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterSTClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterSTProcessClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.processTrackClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.stormEventCenterMovingTrackClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.WPWPCenterrMovingTrackClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.easternPacificColdTongueCenterrMovingTrackClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.oceanAnomalyObjectCenterrMovingTrackClustering = new System.Windows.Forms.ToolStripMenuItem();
            this.STCorrelationPattern = new System.Windows.Forms.ToolStripMenuItem();
            this.STAssociationMiningTransactionTableBuilt = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterTransactionTable = new System.Windows.Forms.ToolStripMenuItem();
            this.objectTransactionTable = new System.Windows.Forms.ToolStripMenuItem();
            this.eventTransactionTable = new System.Windows.Forms.ToolStripMenuItem();
            this.rasterAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.objectAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.processAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSORasterAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOObjectAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOEventAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.ENSOMovingTrackAssociationModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.internetCascadeModeMining = new System.Windows.Forms.ToolStripMenuItem();
            this.visualization = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutUs = new System.Windows.Forms.ToolStripMenuItem();
            this.dataConversion = new System.Windows.Forms.ToolStripMenuItem();
            this.readTifCoor = new System.Windows.Forms.ToolStripMenuItem();
            this.writeTifCoor = new System.Windows.Forms.ToolStripMenuItem();
            this.取出一点数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shpToDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.eventToDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.eventRelToDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.暴雨shp属性重新生成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sST转换处理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sST平均图像转换处理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.流程数据转换处理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.获取文件夹下所有文件名ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.新建NToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.打开OToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.保存SToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.打印PToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.剪切UToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.复制CToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.粘贴PToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.帮助LToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseManagement,
            this.STInfoExtraction,
            this.STAnomalyDetection,
            this.grid,
            this.STCorrelationPattern,
            this.visualization,
            this.aboutUs,
            this.dataConversion});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1037, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // databaseManagement
            // 
            this.databaseManagement.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linkServerDatabase,
            this.linkLocalDatabase,
            this.objectUpdate,
            this.attributeUpdate,
            this.databaseClose});
            this.databaseManagement.Name = "databaseManagement";
            this.databaseManagement.Size = new System.Drawing.Size(80, 21);
            this.databaseManagement.Text = "数据库管理";
            // 
            // linkServerDatabase
            // 
            this.linkServerDatabase.Name = "linkServerDatabase";
            this.linkServerDatabase.Size = new System.Drawing.Size(172, 22);
            this.linkServerDatabase.Text = "连接服务器数据库";
            this.linkServerDatabase.Click += new System.EventHandler(this.linkServerDatabase_Click);
            // 
            // linkLocalDatabase
            // 
            this.linkLocalDatabase.Name = "linkLocalDatabase";
            this.linkLocalDatabase.Size = new System.Drawing.Size(172, 22);
            this.linkLocalDatabase.Text = "连接本地数据库";
            this.linkLocalDatabase.Click += new System.EventHandler(this.linkLocalDatabase_Click);
            // 
            // objectUpdate
            // 
            this.objectUpdate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.WPWPObjectUpdate,
            this.EPWPObjectUpdate,
            this.stormEventObjectUpdate,
            this.ENSOObjectUpdate,
            this.oceanSurfaceRainfallAnomalyObject,
            this.oceanSurfaceAltitudeAnomalyObject,
            this.oceanColorAnomalyObject,
            this.SSTAnomalyObject});
            this.objectUpdate.Name = "objectUpdate";
            this.objectUpdate.Size = new System.Drawing.Size(172, 22);
            this.objectUpdate.Text = "对象更新";
            // 
            // WPWPObjectUpdate
            // 
            this.WPWPObjectUpdate.Name = "WPWPObjectUpdate";
            this.WPWPObjectUpdate.Size = new System.Drawing.Size(196, 22);
            this.WPWPObjectUpdate.Text = "西太平洋暖池";
            // 
            // EPWPObjectUpdate
            // 
            this.EPWPObjectUpdate.Name = "EPWPObjectUpdate";
            this.EPWPObjectUpdate.Size = new System.Drawing.Size(196, 22);
            this.EPWPObjectUpdate.Text = "东太平洋暖池";
            // 
            // stormEventObjectUpdate
            // 
            this.stormEventObjectUpdate.Name = "stormEventObjectUpdate";
            this.stormEventObjectUpdate.Size = new System.Drawing.Size(196, 22);
            this.stormEventObjectUpdate.Text = "暴雨事件";
            // 
            // ENSOObjectUpdate
            // 
            this.ENSOObjectUpdate.Name = "ENSOObjectUpdate";
            this.ENSOObjectUpdate.Size = new System.Drawing.Size(196, 22);
            this.ENSOObjectUpdate.Text = "ENSO事件";
            // 
            // oceanSurfaceRainfallAnomalyObject
            // 
            this.oceanSurfaceRainfallAnomalyObject.Name = "oceanSurfaceRainfallAnomalyObject";
            this.oceanSurfaceRainfallAnomalyObject.Size = new System.Drawing.Size(196, 22);
            this.oceanSurfaceRainfallAnomalyObject.Text = "海洋表面降雨异常对象";
            // 
            // oceanSurfaceAltitudeAnomalyObject
            // 
            this.oceanSurfaceAltitudeAnomalyObject.Name = "oceanSurfaceAltitudeAnomalyObject";
            this.oceanSurfaceAltitudeAnomalyObject.Size = new System.Drawing.Size(196, 22);
            this.oceanSurfaceAltitudeAnomalyObject.Text = "海洋表面高度异常对象";
            // 
            // oceanColorAnomalyObject
            // 
            this.oceanColorAnomalyObject.Name = "oceanColorAnomalyObject";
            this.oceanColorAnomalyObject.Size = new System.Drawing.Size(196, 22);
            this.oceanColorAnomalyObject.Text = "海洋水色异常对象";
            // 
            // SSTAnomalyObject
            // 
            this.SSTAnomalyObject.Name = "SSTAnomalyObject";
            this.SSTAnomalyObject.Size = new System.Drawing.Size(196, 22);
            this.SSTAnomalyObject.Text = "海洋表面温度异常对象";
            // 
            // attributeUpdate
            // 
            this.attributeUpdate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormEventDatabaseTabelPropertyUpdate,
            this.clusteringObjectDatabaseTabelPropertyUpdate,
            this.STAssociationRulesDatabaseTabelPropertyUpdate,
            this.SSTAnomalyDatabaseTabelPropertyUpdate,
            this.oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate,
            this.oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate,
            this.oceanColorAnomalyDatabaseTabelPropertyUpdate});
            this.attributeUpdate.Name = "attributeUpdate";
            this.attributeUpdate.Size = new System.Drawing.Size(172, 22);
            this.attributeUpdate.Text = "属性更新";
            // 
            // stormEventDatabaseTabelPropertyUpdate
            // 
            this.stormEventDatabaseTabelPropertyUpdate.Name = "stormEventDatabaseTabelPropertyUpdate";
            this.stormEventDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.stormEventDatabaseTabelPropertyUpdate.Text = "暴雨事件表";
            // 
            // clusteringObjectDatabaseTabelPropertyUpdate
            // 
            this.clusteringObjectDatabaseTabelPropertyUpdate.Name = "clusteringObjectDatabaseTabelPropertyUpdate";
            this.clusteringObjectDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.clusteringObjectDatabaseTabelPropertyUpdate.Text = "聚簇对象表";
            // 
            // STAssociationRulesDatabaseTabelPropertyUpdate
            // 
            this.STAssociationRulesDatabaseTabelPropertyUpdate.Name = "STAssociationRulesDatabaseTabelPropertyUpdate";
            this.STAssociationRulesDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.STAssociationRulesDatabaseTabelPropertyUpdate.Text = "时空关联规则表";
            // 
            // SSTAnomalyDatabaseTabelPropertyUpdate
            // 
            this.SSTAnomalyDatabaseTabelPropertyUpdate.Name = "SSTAnomalyDatabaseTabelPropertyUpdate";
            this.SSTAnomalyDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.SSTAnomalyDatabaseTabelPropertyUpdate.Text = "海洋表面温度异常对象表";
            // 
            // oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate
            // 
            this.oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate.Name = "oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate";
            this.oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate.Text = "海洋表面降雨异常对象表";
            // 
            // oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate
            // 
            this.oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate.Name = "oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate";
            this.oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate.Text = "海洋表面高度异常对象表";
            // 
            // oceanColorAnomalyDatabaseTabelPropertyUpdate
            // 
            this.oceanColorAnomalyDatabaseTabelPropertyUpdate.Name = "oceanColorAnomalyDatabaseTabelPropertyUpdate";
            this.oceanColorAnomalyDatabaseTabelPropertyUpdate.Size = new System.Drawing.Size(208, 22);
            this.oceanColorAnomalyDatabaseTabelPropertyUpdate.Text = "海洋水色异常对象表";
            // 
            // databaseClose
            // 
            this.databaseClose.Name = "databaseClose";
            this.databaseClose.Size = new System.Drawing.Size(172, 22);
            this.databaseClose.Text = "数据库关闭";
            this.databaseClose.Click += new System.EventHandler(this.databaseClose_Click);
            // 
            // STInfoExtraction
            // 
            this.STInfoExtraction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ENSOEvent,
            this.droughtIndex,
            this.stormEvent,
            this.oceanAnomalySensitiveObject,
            this.WPWPSTInfoExtraction,
            this.EPWPSTInfoExtraction,
            this.海洋热浪ToolStripMenuItem,
            this.海温事件ToolStripMenuItem});
            this.STInfoExtraction.Name = "STInfoExtraction";
            this.STInfoExtraction.Size = new System.Drawing.Size(92, 21);
            this.STInfoExtraction.Text = "时空信息提取";
            // 
            // ENSOEvent
            // 
            this.ENSOEvent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ENSOIndex,
            this.ENSOEventPartition});
            this.ENSOEvent.Name = "ENSOEvent";
            this.ENSOEvent.Size = new System.Drawing.Size(196, 22);
            this.ENSOEvent.Text = "ENSO事件";
            // 
            // ENSOIndex
            // 
            this.ENSOIndex.Name = "ENSOIndex";
            this.ENSOIndex.Size = new System.Drawing.Size(158, 22);
            this.ENSOIndex.Text = "ENSO指数";
            // 
            // ENSOEventPartition
            // 
            this.ENSOEventPartition.Name = "ENSOEventPartition";
            this.ENSOEventPartition.Size = new System.Drawing.Size(158, 22);
            this.ENSOEventPartition.Text = "ENSO事件划分";
            // 
            // droughtIndex
            // 
            this.droughtIndex.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.droughOneDimensionTimeSequence,
            this.droughTwoDimensionRasterSequence});
            this.droughtIndex.Name = "droughtIndex";
            this.droughtIndex.Size = new System.Drawing.Size(196, 22);
            this.droughtIndex.Text = "干旱指数";
            // 
            // droughOneDimensionTimeSequence
            // 
            this.droughOneDimensionTimeSequence.Name = "droughOneDimensionTimeSequence";
            this.droughOneDimensionTimeSequence.Size = new System.Drawing.Size(148, 22);
            this.droughOneDimensionTimeSequence.Text = "一维时间序列";
            // 
            // droughTwoDimensionRasterSequence
            // 
            this.droughTwoDimensionRasterSequence.Name = "droughTwoDimensionRasterSequence";
            this.droughTwoDimensionRasterSequence.Size = new System.Drawing.Size(148, 22);
            this.droughTwoDimensionRasterSequence.Text = "二维栅格序列";
            // 
            // stormEvent
            // 
            this.stormEvent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.基于栅格的暴雨提取ToolStripMenuItem,
            this.基于矢量的暴雨提取ToolStripMenuItem,
            this.stormTITAN,
            this.OutToTxt});
            this.stormEvent.Name = "stormEvent";
            this.stormEvent.Size = new System.Drawing.Size(196, 22);
            this.stormEvent.Text = "暴雨事件";
            // 
            // 基于栅格的暴雨提取ToolStripMenuItem
            // 
            this.基于栅格的暴雨提取ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormTimeExtraction,
            this.stormSpatialExtraction,
            this.stormNumberExtraction,
            this.stormRasterExtraction,
            this.stormResterToVector,
            this.EventExtraction});
            this.基于栅格的暴雨提取ToolStripMenuItem.Name = "基于栅格的暴雨提取ToolStripMenuItem";
            this.基于栅格的暴雨提取ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.基于栅格的暴雨提取ToolStripMenuItem.Text = "基于栅格的暴雨提取";
            // 
            // stormTimeExtraction
            // 
            this.stormTimeExtraction.Name = "stormTimeExtraction";
            this.stormTimeExtraction.Size = new System.Drawing.Size(148, 22);
            this.stormTimeExtraction.Text = "时间维度提取";
            this.stormTimeExtraction.Click += new System.EventHandler(this.stormTimeExtraction_Click);
            // 
            // stormSpatialExtraction
            // 
            this.stormSpatialExtraction.Name = "stormSpatialExtraction";
            this.stormSpatialExtraction.Size = new System.Drawing.Size(148, 22);
            this.stormSpatialExtraction.Text = "空间维度提取";
            this.stormSpatialExtraction.Click += new System.EventHandler(this.stormSpatialExtraction_Click);
            // 
            // stormNumberExtraction
            // 
            this.stormNumberExtraction.Name = "stormNumberExtraction";
            this.stormNumberExtraction.Size = new System.Drawing.Size(148, 22);
            this.stormNumberExtraction.Text = "编号提取";
            this.stormNumberExtraction.Click += new System.EventHandler(this.stormNumberExtraction_Click);
            // 
            // stormRasterExtraction
            // 
            this.stormRasterExtraction.Name = "stormRasterExtraction";
            this.stormRasterExtraction.Size = new System.Drawing.Size(148, 22);
            this.stormRasterExtraction.Text = "栅格暴雨提取";
            this.stormRasterExtraction.Click += new System.EventHandler(this.stormRasterExtraction_Click);
            // 
            // stormResterToVector
            // 
            this.stormResterToVector.Name = "stormResterToVector";
            this.stormResterToVector.Size = new System.Drawing.Size(148, 22);
            this.stormResterToVector.Text = "栅格转矢量";
            this.stormResterToVector.Click += new System.EventHandler(this.stormResterToVector_Click);
            // 
            // EventExtraction
            // 
            this.EventExtraction.Name = "EventExtraction";
            this.EventExtraction.Size = new System.Drawing.Size(148, 22);
            this.EventExtraction.Text = "事件提取入库";
            this.EventExtraction.Click += new System.EventHandler(this.EventExtraction_Click);
            // 
            // 基于矢量的暴雨提取ToolStripMenuItem
            // 
            this.基于矢量的暴雨提取ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormTimeExtraction2,
            this.stormSpatialExtraction2,
            this.stormResterToVectorBasedSpace,
            this.stormProcessExtract});
            this.基于矢量的暴雨提取ToolStripMenuItem.Name = "基于矢量的暴雨提取ToolStripMenuItem";
            this.基于矢量的暴雨提取ToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.基于矢量的暴雨提取ToolStripMenuItem.Text = "基于矢量的暴雨提取";
            // 
            // stormTimeExtraction2
            // 
            this.stormTimeExtraction2.Name = "stormTimeExtraction2";
            this.stormTimeExtraction2.Size = new System.Drawing.Size(184, 22);
            this.stormTimeExtraction2.Text = "时间维度提取";
            this.stormTimeExtraction2.Click += new System.EventHandler(this.stormTimeExtraction_Click);
            // 
            // stormSpatialExtraction2
            // 
            this.stormSpatialExtraction2.Name = "stormSpatialExtraction2";
            this.stormSpatialExtraction2.Size = new System.Drawing.Size(184, 22);
            this.stormSpatialExtraction2.Text = "空间维度提取";
            this.stormSpatialExtraction2.Click += new System.EventHandler(this.stormSpatialExtraction_Click);
            // 
            // stormResterToVectorBasedSpace
            // 
            this.stormResterToVectorBasedSpace.Name = "stormResterToVectorBasedSpace";
            this.stormResterToVectorBasedSpace.Size = new System.Drawing.Size(184, 22);
            this.stormResterToVectorBasedSpace.Text = "基于空间的矢量提取";
            this.stormResterToVectorBasedSpace.Click += new System.EventHandler(this.stormResterToVectorBasedSpace_Click);
            // 
            // stormProcessExtract
            // 
            this.stormProcessExtract.Name = "stormProcessExtract";
            this.stormProcessExtract.Size = new System.Drawing.Size(184, 22);
            this.stormProcessExtract.Text = "面向过程的提取";
            this.stormProcessExtract.Click += new System.EventHandler(this.stormProcessExtract_Click);
            // 
            // stormTITAN
            // 
            this.stormTITAN.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormTimeExtraction3,
            this.stormSpatialExtraction3,
            this.stormResterToVectorBasedSpace2,
            this.stormTITANExtract});
            this.stormTITAN.Name = "stormTITAN";
            this.stormTITAN.Size = new System.Drawing.Size(184, 22);
            this.stormTITAN.Text = "TITAN算法";
            // 
            // stormTimeExtraction3
            // 
            this.stormTimeExtraction3.Name = "stormTimeExtraction3";
            this.stormTimeExtraction3.Size = new System.Drawing.Size(184, 22);
            this.stormTimeExtraction3.Text = "时间维度提取";
            this.stormTimeExtraction3.Click += new System.EventHandler(this.stormTimeExtraction_Click);
            // 
            // stormSpatialExtraction3
            // 
            this.stormSpatialExtraction3.Name = "stormSpatialExtraction3";
            this.stormSpatialExtraction3.Size = new System.Drawing.Size(184, 22);
            this.stormSpatialExtraction3.Text = "空间维度提取";
            this.stormSpatialExtraction3.Click += new System.EventHandler(this.stormSpatialExtraction_Click);
            // 
            // stormResterToVectorBasedSpace2
            // 
            this.stormResterToVectorBasedSpace2.Name = "stormResterToVectorBasedSpace2";
            this.stormResterToVectorBasedSpace2.Size = new System.Drawing.Size(184, 22);
            this.stormResterToVectorBasedSpace2.Text = "基于空间的矢量提取";
            this.stormResterToVectorBasedSpace2.Click += new System.EventHandler(this.stormResterToVectorBasedSpace_Click);
            // 
            // stormTITANExtract
            // 
            this.stormTITANExtract.Name = "stormTITANExtract";
            this.stormTITANExtract.Size = new System.Drawing.Size(184, 22);
            this.stormTITANExtract.Text = "事件提取入库";
            this.stormTITANExtract.Click += new System.EventHandler(this.stormTITANExtract_Click);
            // 
            // OutToTxt
            // 
            this.OutToTxt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormEventToVector,
            this.stormEventStateToVector,
            this.导出为文本ToolStripMenuItem});
            this.OutToTxt.Name = "OutToTxt";
            this.OutToTxt.Size = new System.Drawing.Size(184, 22);
            this.OutToTxt.Text = "数据库导出";
            // 
            // stormEventToVector
            // 
            this.stormEventToVector.Name = "stormEventToVector";
            this.stormEventToVector.Size = new System.Drawing.Size(160, 22);
            this.stormEventToVector.Text = "事件转矢量";
            this.stormEventToVector.Click += new System.EventHandler(this.stormEventToVector_Click);
            // 
            // stormEventStateToVector
            // 
            this.stormEventStateToVector.Name = "stormEventStateToVector";
            this.stormEventStateToVector.Size = new System.Drawing.Size(160, 22);
            this.stormEventStateToVector.Text = "事件状态转矢量";
            this.stormEventStateToVector.Click += new System.EventHandler(this.stormEventStateToVector_Click);
            // 
            // 导出为文本ToolStripMenuItem
            // 
            this.导出为文本ToolStripMenuItem.Name = "导出为文本ToolStripMenuItem";
            this.导出为文本ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.导出为文本ToolStripMenuItem.Text = "导出为文本";
            this.导出为文本ToolStripMenuItem.Click += new System.EventHandler(this.导出为文本ToolStripMenuItem_Click);
            // 
            // oceanAnomalySensitiveObject
            // 
            this.oceanAnomalySensitiveObject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SSTExtraction,
            this.oceanSurfaceRainfallExtraction,
            this.oceanSurfaceAltitudeAnomalyExtraction,
            this.oceanPrimaryProductivityExtraction,
            this.oceanSurfaceChlorophylAConcentrationExtraction});
            this.oceanAnomalySensitiveObject.Name = "oceanAnomalySensitiveObject";
            this.oceanAnomalySensitiveObject.Size = new System.Drawing.Size(196, 22);
            this.oceanAnomalySensitiveObject.Text = "海洋异常变化敏感对象";
            // 
            // SSTExtraction
            // 
            this.SSTExtraction.Name = "SSTExtraction";
            this.SSTExtraction.Size = new System.Drawing.Size(167, 22);
            this.SSTExtraction.Text = "海洋表面温度";
            // 
            // oceanSurfaceRainfallExtraction
            // 
            this.oceanSurfaceRainfallExtraction.Name = "oceanSurfaceRainfallExtraction";
            this.oceanSurfaceRainfallExtraction.Size = new System.Drawing.Size(167, 22);
            this.oceanSurfaceRainfallExtraction.Text = "海洋表面降雨";
            // 
            // oceanSurfaceAltitudeAnomalyExtraction
            // 
            this.oceanSurfaceAltitudeAnomalyExtraction.Name = "oceanSurfaceAltitudeAnomalyExtraction";
            this.oceanSurfaceAltitudeAnomalyExtraction.Size = new System.Drawing.Size(167, 22);
            this.oceanSurfaceAltitudeAnomalyExtraction.Text = "海面高度异常";
            // 
            // oceanPrimaryProductivityExtraction
            // 
            this.oceanPrimaryProductivityExtraction.Name = "oceanPrimaryProductivityExtraction";
            this.oceanPrimaryProductivityExtraction.Size = new System.Drawing.Size(167, 22);
            this.oceanPrimaryProductivityExtraction.Text = "海洋初级生产力";
            // 
            // oceanSurfaceChlorophylAConcentrationExtraction
            // 
            this.oceanSurfaceChlorophylAConcentrationExtraction.Name = "oceanSurfaceChlorophylAConcentrationExtraction";
            this.oceanSurfaceChlorophylAConcentrationExtraction.Size = new System.Drawing.Size(167, 22);
            this.oceanSurfaceChlorophylAConcentrationExtraction.Text = "海表叶绿素a浓度";
            // 
            // WPWPSTInfoExtraction
            // 
            this.WPWPSTInfoExtraction.Name = "WPWPSTInfoExtraction";
            this.WPWPSTInfoExtraction.Size = new System.Drawing.Size(196, 22);
            this.WPWPSTInfoExtraction.Text = "西太平洋暖池";
            // 
            // EPWPSTInfoExtraction
            // 
            this.EPWPSTInfoExtraction.Name = "EPWPSTInfoExtraction";
            this.EPWPSTInfoExtraction.Size = new System.Drawing.Size(196, 22);
            this.EPWPSTInfoExtraction.Text = "东太平洋暖池";
            // 
            // 海洋热浪ToolStripMenuItem
            // 
            this.海洋热浪ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.marineHeatwavesAverage,
            this.toolStripSeparator2,
            this.marineHeatwavesMedian,
            this.marineHeatwaves90Percent,
            this.marineHeatwavesTimeAnomaly,
            this.marineHeatwavesTimeExtraction,
            this.marineHeatwavesTimeExtraction2,
            this.marineHeatwavesVectorExtraction,
            this.marineHeatwavesSpeed,
            this.marineHeatwavesTracking});
            this.海洋热浪ToolStripMenuItem.Name = "海洋热浪ToolStripMenuItem";
            this.海洋热浪ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.海洋热浪ToolStripMenuItem.Text = "海洋热浪";
            // 
            // marineHeatwavesAverage
            // 
            this.marineHeatwavesAverage.Name = "marineHeatwavesAverage";
            this.marineHeatwavesAverage.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesAverage.Text = "均值图像";
            this.marineHeatwavesAverage.Click += new System.EventHandler(this.marineHeatwavesAverage_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(169, 6);
            // 
            // marineHeatwavesMedian
            // 
            this.marineHeatwavesMedian.Name = "marineHeatwavesMedian";
            this.marineHeatwavesMedian.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesMedian.Text = "中位数图像";
            this.marineHeatwavesMedian.Click += new System.EventHandler(this.marineHeatwavesMedian_Click);
            // 
            // marineHeatwaves90Percent
            // 
            this.marineHeatwaves90Percent.Name = "marineHeatwaves90Percent";
            this.marineHeatwaves90Percent.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwaves90Percent.Text = "90百分位图像";
            this.marineHeatwaves90Percent.Click += new System.EventHandler(this.marineHeatwaves90Percent_Click);
            // 
            // marineHeatwavesTimeAnomaly
            // 
            this.marineHeatwavesTimeAnomaly.Name = "marineHeatwavesTimeAnomaly";
            this.marineHeatwavesTimeAnomaly.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesTimeAnomaly.Text = "时间距平";
            this.marineHeatwavesTimeAnomaly.Click += new System.EventHandler(this.marineHeatwavesTimeAnomaly_Click);
            // 
            // marineHeatwavesTimeExtraction
            // 
            this.marineHeatwavesTimeExtraction.Name = "marineHeatwavesTimeExtraction";
            this.marineHeatwavesTimeExtraction.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesTimeExtraction.Text = "时间维度提取";
            this.marineHeatwavesTimeExtraction.Click += new System.EventHandler(this.marineHeatwavesTimeExtraction_Click);
            // 
            // marineHeatwavesTimeExtraction2
            // 
            this.marineHeatwavesTimeExtraction2.Name = "marineHeatwavesTimeExtraction2";
            this.marineHeatwavesTimeExtraction2.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesTimeExtraction2.Text = "时间维度间隔提取";
            this.marineHeatwavesTimeExtraction2.Click += new System.EventHandler(this.marineHeatwavesTimeExtraction2_Click);
            // 
            // marineHeatwavesVectorExtraction
            // 
            this.marineHeatwavesVectorExtraction.Name = "marineHeatwavesVectorExtraction";
            this.marineHeatwavesVectorExtraction.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesVectorExtraction.Text = "矢量化及信息提取";
            this.marineHeatwavesVectorExtraction.Click += new System.EventHandler(this.marineHeatwavesVectorExtraction_Click);
            // 
            // marineHeatwavesSpeed
            // 
            this.marineHeatwavesSpeed.Name = "marineHeatwavesSpeed";
            this.marineHeatwavesSpeed.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesSpeed.Text = "移动速度计算";
            // 
            // marineHeatwavesTracking
            // 
            this.marineHeatwavesTracking.Name = "marineHeatwavesTracking";
            this.marineHeatwavesTracking.Size = new System.Drawing.Size(172, 22);
            this.marineHeatwavesTracking.Text = "事件追踪提取";
            // 
            // 海温事件ToolStripMenuItem
            // 
            this.海温事件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.基于矢量的SST提取ToolStripMenuItem});
            this.海温事件ToolStripMenuItem.Name = "海温事件ToolStripMenuItem";
            this.海温事件ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.海温事件ToolStripMenuItem.Text = "海温事件";
            // 
            // 基于矢量的SST提取ToolStripMenuItem
            // 
            this.基于矢量的SST提取ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sstTimeExtraction,
            this.sstSpaceExtraction,
            this.sstResterToVectorBasedSpace,
            this.sstRestertoVectorExtractInfo,
            this.sstProcessExtract,
            this.数据库导出ToolStripMenuItem});
            this.基于矢量的SST提取ToolStripMenuItem.Name = "基于矢量的SST提取ToolStripMenuItem";
            this.基于矢量的SST提取ToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.基于矢量的SST提取ToolStripMenuItem.Text = "基于矢量的SST提取";
            // 
            // sstTimeExtraction
            // 
            this.sstTimeExtraction.Name = "sstTimeExtraction";
            this.sstTimeExtraction.Size = new System.Drawing.Size(238, 22);
            this.sstTimeExtraction.Text = "时间维度提取";
            this.sstTimeExtraction.Click += new System.EventHandler(this.sstTimeExtraction_Click);
            // 
            // sstSpaceExtraction
            // 
            this.sstSpaceExtraction.Name = "sstSpaceExtraction";
            this.sstSpaceExtraction.Size = new System.Drawing.Size(238, 22);
            this.sstSpaceExtraction.Text = "空间维度提取（error）";
            this.sstSpaceExtraction.Click += new System.EventHandler(this.sstSpaceExtraction_Click);
            // 
            // sstResterToVectorBasedSpace
            // 
            this.sstResterToVectorBasedSpace.Name = "sstResterToVectorBasedSpace";
            this.sstResterToVectorBasedSpace.Size = new System.Drawing.Size(238, 22);
            this.sstResterToVectorBasedSpace.Text = "基于空间的矢量提取（error）";
            this.sstResterToVectorBasedSpace.Click += new System.EventHandler(this.sstResterToVectorBasedSpace_Click);
            // 
            // sstRestertoVectorExtractInfo
            // 
            this.sstRestertoVectorExtractInfo.Name = "sstRestertoVectorExtractInfo";
            this.sstRestertoVectorExtractInfo.Size = new System.Drawing.Size(238, 22);
            this.sstRestertoVectorExtractInfo.Text = "矢量化及信息提取";
            this.sstRestertoVectorExtractInfo.Click += new System.EventHandler(this.sstRestertoVectorExtractInfo_Click);
            // 
            // sstProcessExtract
            // 
            this.sstProcessExtract.Name = "sstProcessExtract";
            this.sstProcessExtract.Size = new System.Drawing.Size(238, 22);
            this.sstProcessExtract.Text = "面向过程提取";
            this.sstProcessExtract.Click += new System.EventHandler(this.sstProcessExtract_Click);
            // 
            // 数据库导出ToolStripMenuItem
            // 
            this.数据库导出ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SstEventToVector,
            this.SSTEventStateToVector});
            this.数据库导出ToolStripMenuItem.Name = "数据库导出ToolStripMenuItem";
            this.数据库导出ToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.数据库导出ToolStripMenuItem.Text = "数据库导出";
            // 
            // SstEventToVector
            // 
            this.SstEventToVector.Name = "SstEventToVector";
            this.SstEventToVector.Size = new System.Drawing.Size(160, 22);
            this.SstEventToVector.Text = "事件转矢量";
            this.SstEventToVector.Click += new System.EventHandler(this.SstEventToVector_Click);
            // 
            // SSTEventStateToVector
            // 
            this.SSTEventStateToVector.Name = "SSTEventStateToVector";
            this.SSTEventStateToVector.Size = new System.Drawing.Size(160, 22);
            this.SSTEventStateToVector.Text = "事件状态转矢量";
            this.SSTEventStateToVector.Click += new System.EventHandler(this.SSTEventStateToVector_Click);
            // 
            // STAnomalyDetection
            // 
            this.STAnomalyDetection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rasterSpatialAnomalyDetection,
            this.rasterTimeAnomalyDetection,
            this.rasterSTAnomalyDetection});
            this.STAnomalyDetection.Name = "STAnomalyDetection";
            this.STAnomalyDetection.Size = new System.Drawing.Size(92, 21);
            this.STAnomalyDetection.Text = "时空异常探测";
            // 
            // rasterSpatialAnomalyDetection
            // 
            this.rasterSpatialAnomalyDetection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spatialStatisticModel,
            this.spatialClusteringMining,
            this.spatialTrendSurfaceSimulation});
            this.rasterSpatialAnomalyDetection.Name = "rasterSpatialAnomalyDetection";
            this.rasterSpatialAnomalyDetection.Size = new System.Drawing.Size(208, 22);
            this.rasterSpatialAnomalyDetection.Text = "面向栅格的空间异常探测";
            // 
            // spatialStatisticModel
            // 
            this.spatialStatisticModel.Name = "spatialStatisticModel";
            this.spatialStatisticModel.Size = new System.Drawing.Size(160, 22);
            this.spatialStatisticModel.Text = "基于统计模型";
            // 
            // spatialClusteringMining
            // 
            this.spatialClusteringMining.Name = "spatialClusteringMining";
            this.spatialClusteringMining.Size = new System.Drawing.Size(160, 22);
            this.spatialClusteringMining.Text = "基于聚类挖掘";
            // 
            // spatialTrendSurfaceSimulation
            // 
            this.spatialTrendSurfaceSimulation.Name = "spatialTrendSurfaceSimulation";
            this.spatialTrendSurfaceSimulation.Size = new System.Drawing.Size(160, 22);
            this.spatialTrendSurfaceSimulation.Text = "基于趋势面模拟";
            // 
            // rasterTimeAnomalyDetection
            // 
            this.rasterTimeAnomalyDetection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeStatisticModel,
            this.timeClusteringMining,
            this.timeAnalogueFunction});
            this.rasterTimeAnomalyDetection.Name = "rasterTimeAnomalyDetection";
            this.rasterTimeAnomalyDetection.Size = new System.Drawing.Size(208, 22);
            this.rasterTimeAnomalyDetection.Text = "面向栅格的时序异常探测";
            // 
            // timeStatisticModel
            // 
            this.timeStatisticModel.Name = "timeStatisticModel";
            this.timeStatisticModel.Size = new System.Drawing.Size(148, 22);
            this.timeStatisticModel.Text = "基于统计模型";
            // 
            // timeClusteringMining
            // 
            this.timeClusteringMining.Name = "timeClusteringMining";
            this.timeClusteringMining.Size = new System.Drawing.Size(148, 22);
            this.timeClusteringMining.Text = "基于聚类挖掘";
            // 
            // timeAnalogueFunction
            // 
            this.timeAnalogueFunction.Name = "timeAnalogueFunction";
            this.timeAnalogueFunction.Size = new System.Drawing.Size(148, 22);
            this.timeAnalogueFunction.Text = "基于模拟函数";
            // 
            // rasterSTAnomalyDetection
            // 
            this.rasterSTAnomalyDetection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.STStatisticModel,
            this.STClusteringMining});
            this.rasterSTAnomalyDetection.Name = "rasterSTAnomalyDetection";
            this.rasterSTAnomalyDetection.Size = new System.Drawing.Size(208, 22);
            this.rasterSTAnomalyDetection.Text = "面向栅格的时空异常探测";
            // 
            // STStatisticModel
            // 
            this.STStatisticModel.Name = "STStatisticModel";
            this.STStatisticModel.Size = new System.Drawing.Size(148, 22);
            this.STStatisticModel.Text = "基于统计模型";
            // 
            // STClusteringMining
            // 
            this.STClusteringMining.Name = "STClusteringMining";
            this.STClusteringMining.Size = new System.Drawing.Size(148, 22);
            this.STClusteringMining.Text = "基于聚类挖掘";
            // 
            // grid
            // 
            this.grid.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rasterSpatialClustering,
            this.objectSpatialClustering,
            this.rasterTimeClustering,
            this.objectTimeClustering,
            this.spatialAttributeDoubleConstraintsClustering,
            this.rasterSTClustering,
            this.rasterSTProcessClustering,
            this.processTrackClustering});
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(92, 21);
            this.grid.Text = "时空聚簇挖掘";
            // 
            // rasterSpatialClustering
            // 
            this.rasterSpatialClustering.Name = "rasterSpatialClustering";
            this.rasterSpatialClustering.Size = new System.Drawing.Size(208, 22);
            this.rasterSpatialClustering.Text = "面向栅格的空间聚类";
            // 
            // objectSpatialClustering
            // 
            this.objectSpatialClustering.Name = "objectSpatialClustering";
            this.objectSpatialClustering.Size = new System.Drawing.Size(208, 22);
            this.objectSpatialClustering.Text = "面向对象的空间聚类";
            // 
            // rasterTimeClustering
            // 
            this.rasterTimeClustering.Name = "rasterTimeClustering";
            this.rasterTimeClustering.Size = new System.Drawing.Size(208, 22);
            this.rasterTimeClustering.Text = "面向栅格的时间序列聚类";
            // 
            // objectTimeClustering
            // 
            this.objectTimeClustering.Name = "objectTimeClustering";
            this.objectTimeClustering.Size = new System.Drawing.Size(208, 22);
            this.objectTimeClustering.Text = "面向对象的时间序列聚类";
            // 
            // spatialAttributeDoubleConstraintsClustering
            // 
            this.spatialAttributeDoubleConstraintsClustering.Name = "spatialAttributeDoubleConstraintsClustering";
            this.spatialAttributeDoubleConstraintsClustering.Size = new System.Drawing.Size(208, 22);
            this.spatialAttributeDoubleConstraintsClustering.Text = "空间属性双约束聚类";
            // 
            // rasterSTClustering
            // 
            this.rasterSTClustering.Name = "rasterSTClustering";
            this.rasterSTClustering.Size = new System.Drawing.Size(208, 22);
            this.rasterSTClustering.Text = "面向栅格的时空聚类";
            // 
            // rasterSTProcessClustering
            // 
            this.rasterSTProcessClustering.Name = "rasterSTProcessClustering";
            this.rasterSTProcessClustering.Size = new System.Drawing.Size(208, 22);
            this.rasterSTProcessClustering.Text = "面向栅格的时空过程聚类";
            // 
            // processTrackClustering
            // 
            this.processTrackClustering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stormEventCenterMovingTrackClustering,
            this.WPWPCenterrMovingTrackClustering,
            this.easternPacificColdTongueCenterrMovingTrackClustering,
            this.oceanAnomalyObjectCenterrMovingTrackClustering});
            this.processTrackClustering.Name = "processTrackClustering";
            this.processTrackClustering.Size = new System.Drawing.Size(208, 22);
            this.processTrackClustering.Text = "面向过程的轨迹聚类";
            // 
            // stormEventCenterMovingTrackClustering
            // 
            this.stormEventCenterMovingTrackClustering.Name = "stormEventCenterMovingTrackClustering";
            this.stormEventCenterMovingTrackClustering.Size = new System.Drawing.Size(268, 22);
            this.stormEventCenterMovingTrackClustering.Text = "暴雨事件中心移动轨迹聚类";
            // 
            // WPWPCenterrMovingTrackClustering
            // 
            this.WPWPCenterrMovingTrackClustering.Name = "WPWPCenterrMovingTrackClustering";
            this.WPWPCenterrMovingTrackClustering.Size = new System.Drawing.Size(268, 22);
            this.WPWPCenterrMovingTrackClustering.Text = "西太平洋暖池中心移动轨迹聚类";
            // 
            // easternPacificColdTongueCenterrMovingTrackClustering
            // 
            this.easternPacificColdTongueCenterrMovingTrackClustering.Name = "easternPacificColdTongueCenterrMovingTrackClustering";
            this.easternPacificColdTongueCenterrMovingTrackClustering.Size = new System.Drawing.Size(268, 22);
            this.easternPacificColdTongueCenterrMovingTrackClustering.Text = "东太平洋冷舌中心移动轨迹聚类";
            // 
            // oceanAnomalyObjectCenterrMovingTrackClustering
            // 
            this.oceanAnomalyObjectCenterrMovingTrackClustering.Name = "oceanAnomalyObjectCenterrMovingTrackClustering";
            this.oceanAnomalyObjectCenterrMovingTrackClustering.Size = new System.Drawing.Size(268, 22);
            this.oceanAnomalyObjectCenterrMovingTrackClustering.Text = "海洋异常变化对象中心移动轨迹聚类";
            // 
            // STCorrelationPattern
            // 
            this.STCorrelationPattern.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.STAssociationMiningTransactionTableBuilt,
            this.rasterAssociationModeMining,
            this.objectAssociationModeMining,
            this.processAssociationModeMining,
            this.ENSORasterAssociationModeMining,
            this.ENSOObjectAssociationModeMining,
            this.ENSOEventAssociationModeMining,
            this.ENSOMovingTrackAssociationModeMining,
            this.internetCascadeModeMining});
            this.STCorrelationPattern.Name = "STCorrelationPattern";
            this.STCorrelationPattern.Size = new System.Drawing.Size(92, 21);
            this.STCorrelationPattern.Text = "时空关联模式";
            // 
            // STAssociationMiningTransactionTableBuilt
            // 
            this.STAssociationMiningTransactionTableBuilt.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rasterTransactionTable,
            this.objectTransactionTable,
            this.eventTransactionTable});
            this.STAssociationMiningTransactionTableBuilt.Name = "STAssociationMiningTransactionTableBuilt";
            this.STAssociationMiningTransactionTableBuilt.Size = new System.Drawing.Size(235, 22);
            this.STAssociationMiningTransactionTableBuilt.Text = "时空关联挖掘事务表构建";
            // 
            // rasterTransactionTable
            // 
            this.rasterTransactionTable.Name = "rasterTransactionTable";
            this.rasterTransactionTable.Size = new System.Drawing.Size(136, 22);
            this.rasterTransactionTable.Text = "栅格事务表";
            // 
            // objectTransactionTable
            // 
            this.objectTransactionTable.Name = "objectTransactionTable";
            this.objectTransactionTable.Size = new System.Drawing.Size(136, 22);
            this.objectTransactionTable.Text = "对象事务表";
            // 
            // eventTransactionTable
            // 
            this.eventTransactionTable.Name = "eventTransactionTable";
            this.eventTransactionTable.Size = new System.Drawing.Size(136, 22);
            this.eventTransactionTable.Text = "事件事务表";
            // 
            // rasterAssociationModeMining
            // 
            this.rasterAssociationModeMining.Name = "rasterAssociationModeMining";
            this.rasterAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.rasterAssociationModeMining.Text = "面向栅格的关联模式挖掘";
            // 
            // objectAssociationModeMining
            // 
            this.objectAssociationModeMining.Name = "objectAssociationModeMining";
            this.objectAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.objectAssociationModeMining.Text = "面向对象的关联模式挖掘";
            // 
            // processAssociationModeMining
            // 
            this.processAssociationModeMining.Name = "processAssociationModeMining";
            this.processAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.processAssociationModeMining.Text = "面向过程的关联模式挖掘";
            // 
            // ENSORasterAssociationModeMining
            // 
            this.ENSORasterAssociationModeMining.Name = "ENSORasterAssociationModeMining";
            this.ENSORasterAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.ENSORasterAssociationModeMining.Text = "ENSO-栅格关联模式挖掘";
            // 
            // ENSOObjectAssociationModeMining
            // 
            this.ENSOObjectAssociationModeMining.Name = "ENSOObjectAssociationModeMining";
            this.ENSOObjectAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.ENSOObjectAssociationModeMining.Text = "ENSO-对象关联模式挖掘";
            // 
            // ENSOEventAssociationModeMining
            // 
            this.ENSOEventAssociationModeMining.Name = "ENSOEventAssociationModeMining";
            this.ENSOEventAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.ENSOEventAssociationModeMining.Text = "ENSO-事件关联模式挖掘";
            // 
            // ENSOMovingTrackAssociationModeMining
            // 
            this.ENSOMovingTrackAssociationModeMining.Name = "ENSOMovingTrackAssociationModeMining";
            this.ENSOMovingTrackAssociationModeMining.Size = new System.Drawing.Size(235, 22);
            this.ENSOMovingTrackAssociationModeMining.Text = "ENSO-移动轨迹关联模式挖掘";
            // 
            // internetCascadeModeMining
            // 
            this.internetCascadeModeMining.Name = "internetCascadeModeMining";
            this.internetCascadeModeMining.Size = new System.Drawing.Size(235, 22);
            this.internetCascadeModeMining.Text = "基于互信息的级联模式挖掘";
            // 
            // visualization
            // 
            this.visualization.Name = "visualization";
            this.visualization.Size = new System.Drawing.Size(56, 21);
            this.visualization.Text = "可视化";
            // 
            // aboutUs
            // 
            this.aboutUs.Name = "aboutUs";
            this.aboutUs.Size = new System.Drawing.Size(68, 21);
            this.aboutUs.Text = "关于我们";
            this.aboutUs.Click += new System.EventHandler(this.aboutUs_Click);
            // 
            // dataConversion
            // 
            this.dataConversion.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readTifCoor,
            this.writeTifCoor,
            this.取出一点数据ToolStripMenuItem,
            this.shpToDatabase,
            this.eventToDatabase,
            this.eventRelToDatabase,
            this.暴雨shp属性重新生成ToolStripMenuItem,
            this.sST转换处理ToolStripMenuItem,
            this.sST平均图像转换处理ToolStripMenuItem,
            this.流程数据转换处理ToolStripMenuItem,
            this.获取文件夹下所有文件名ToolStripMenuItem});
            this.dataConversion.Name = "dataConversion";
            this.dataConversion.Size = new System.Drawing.Size(44, 21);
            this.dataConversion.Text = "其他";
            this.dataConversion.Click += new System.EventHandler(this.dataConversion_Click);
            // 
            // readTifCoor
            // 
            this.readTifCoor.Name = "readTifCoor";
            this.readTifCoor.Size = new System.Drawing.Size(208, 22);
            this.readTifCoor.Text = "Tif坐标信息读取";
            this.readTifCoor.Click += new System.EventHandler(this.readTifCoor_Click);
            // 
            // writeTifCoor
            // 
            this.writeTifCoor.Name = "writeTifCoor";
            this.writeTifCoor.Size = new System.Drawing.Size(208, 22);
            this.writeTifCoor.Text = "Tif坐标信息写入";
            this.writeTifCoor.Click += new System.EventHandler(this.writeTifCoor_Click);
            // 
            // 取出一点数据ToolStripMenuItem
            // 
            this.取出一点数据ToolStripMenuItem.Name = "取出一点数据ToolStripMenuItem";
            this.取出一点数据ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.取出一点数据ToolStripMenuItem.Text = "取出一点数据";
            this.取出一点数据ToolStripMenuItem.Click += new System.EventHandler(this.取出一点数据ToolStripMenuItem_Click);
            // 
            // shpToDatabase
            // 
            this.shpToDatabase.Name = "shpToDatabase";
            this.shpToDatabase.Size = new System.Drawing.Size(208, 22);
            this.shpToDatabase.Text = "shp图层入库";
            this.shpToDatabase.Click += new System.EventHandler(this.shpToDatabase_Click);
            // 
            // eventToDatabase
            // 
            this.eventToDatabase.Name = "eventToDatabase";
            this.eventToDatabase.Size = new System.Drawing.Size(208, 22);
            this.eventToDatabase.Text = "事件入库";
            this.eventToDatabase.Click += new System.EventHandler(this.eventToDatabase_Click);
            // 
            // eventRelToDatabase
            // 
            this.eventRelToDatabase.Name = "eventRelToDatabase";
            this.eventRelToDatabase.Size = new System.Drawing.Size(208, 22);
            this.eventRelToDatabase.Text = "事件关系入库";
            this.eventRelToDatabase.Click += new System.EventHandler(this.eventRelToDatabase_Click);
            // 
            // 暴雨shp属性重新生成ToolStripMenuItem
            // 
            this.暴雨shp属性重新生成ToolStripMenuItem.Name = "暴雨shp属性重新生成ToolStripMenuItem";
            this.暴雨shp属性重新生成ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.暴雨shp属性重新生成ToolStripMenuItem.Text = "暴雨shp属性重新生成";
            this.暴雨shp属性重新生成ToolStripMenuItem.Click += new System.EventHandler(this.暴雨shp属性重新生成ToolStripMenuItem_Click);
            // 
            // sST转换处理ToolStripMenuItem
            // 
            this.sST转换处理ToolStripMenuItem.Name = "sST转换处理ToolStripMenuItem";
            this.sST转换处理ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sST转换处理ToolStripMenuItem.Text = "SST转换处理";
            this.sST转换处理ToolStripMenuItem.Click += new System.EventHandler(this.sST转换处理ToolStripMenuItem_Click);
            // 
            // sST平均图像转换处理ToolStripMenuItem
            // 
            this.sST平均图像转换处理ToolStripMenuItem.Name = "sST平均图像转换处理ToolStripMenuItem";
            this.sST平均图像转换处理ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.sST平均图像转换处理ToolStripMenuItem.Text = "SST平均图像转换处理";
            this.sST平均图像转换处理ToolStripMenuItem.Click += new System.EventHandler(this.sST平均图像转换处理ToolStripMenuItem_Click);
            // 
            // 流程数据转换处理ToolStripMenuItem
            // 
            this.流程数据转换处理ToolStripMenuItem.Name = "流程数据转换处理ToolStripMenuItem";
            this.流程数据转换处理ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.流程数据转换处理ToolStripMenuItem.Text = "流程数据转换处理";
            this.流程数据转换处理ToolStripMenuItem.Click += new System.EventHandler(this.流程数据转换处理ToolStripMenuItem_Click);
            // 
            // 获取文件夹下所有文件名ToolStripMenuItem
            // 
            this.获取文件夹下所有文件名ToolStripMenuItem.Name = "获取文件夹下所有文件名ToolStripMenuItem";
            this.获取文件夹下所有文件名ToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.获取文件夹下所有文件名ToolStripMenuItem.Text = "获取文件夹下所有文件名";
            this.获取文件夹下所有文件名ToolStripMenuItem.Click += new System.EventHandler(this.获取文件夹下所有文件名ToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建NToolStripButton,
            this.打开OToolStripButton,
            this.保存SToolStripButton,
            this.打印PToolStripButton,
            this.toolStripSeparator,
            this.剪切UToolStripButton,
            this.复制CToolStripButton,
            this.粘贴PToolStripButton,
            this.toolStripSeparator1,
            this.帮助LToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1037, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // 新建NToolStripButton
            // 
            this.新建NToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.新建NToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("新建NToolStripButton.Image")));
            this.新建NToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.新建NToolStripButton.Name = "新建NToolStripButton";
            this.新建NToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.新建NToolStripButton.Text = "新建(&N)";
            // 
            // 打开OToolStripButton
            // 
            this.打开OToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打开OToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打开OToolStripButton.Image")));
            this.打开OToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打开OToolStripButton.Name = "打开OToolStripButton";
            this.打开OToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.打开OToolStripButton.Text = "打开(&O)";
            // 
            // 保存SToolStripButton
            // 
            this.保存SToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.保存SToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("保存SToolStripButton.Image")));
            this.保存SToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.保存SToolStripButton.Name = "保存SToolStripButton";
            this.保存SToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.保存SToolStripButton.Text = "保存(&S)";
            // 
            // 打印PToolStripButton
            // 
            this.打印PToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.打印PToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("打印PToolStripButton.Image")));
            this.打印PToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.打印PToolStripButton.Name = "打印PToolStripButton";
            this.打印PToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.打印PToolStripButton.Text = "打印(&P)";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // 剪切UToolStripButton
            // 
            this.剪切UToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.剪切UToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("剪切UToolStripButton.Image")));
            this.剪切UToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.剪切UToolStripButton.Name = "剪切UToolStripButton";
            this.剪切UToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.剪切UToolStripButton.Text = "剪切(&U)";
            // 
            // 复制CToolStripButton
            // 
            this.复制CToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.复制CToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("复制CToolStripButton.Image")));
            this.复制CToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.复制CToolStripButton.Name = "复制CToolStripButton";
            this.复制CToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.复制CToolStripButton.Text = "复制(&C)";
            // 
            // 粘贴PToolStripButton
            // 
            this.粘贴PToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.粘贴PToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("粘贴PToolStripButton.Image")));
            this.粘贴PToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.粘贴PToolStripButton.Name = "粘贴PToolStripButton";
            this.粘贴PToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.粘贴PToolStripButton.Text = "粘贴(&P)";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // 帮助LToolStripButton
            // 
            this.帮助LToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.帮助LToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("帮助LToolStripButton.Image")));
            this.帮助LToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.帮助LToolStripButton.Name = "帮助LToolStripButton";
            this.帮助LToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.帮助LToolStripButton.Text = "帮助(&L)";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 610);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1037, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel1.Text = "就绪";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.treeView1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 52);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 558);
            this.panel1.TabIndex = 3;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "节点1";
            treeNode1.Text = "节点1";
            treeNode2.Name = "节点0";
            treeNode2.Text = "节点0";
            treeNode3.Name = "节点3";
            treeNode3.Text = "节点3";
            treeNode4.Name = "节点4";
            treeNode4.Text = "节点4";
            treeNode5.Name = "节点2";
            treeNode5.Text = "节点2";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode5});
            this.treeView1.Size = new System.Drawing.Size(250, 558);
            this.treeView1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(250, 52);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 558);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(253, 52);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(784, 558);
            this.panel2.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(784, 558);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(787, 610);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 22);
            this.progressBar1.TabIndex = 6;
            this.progressBar1.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1037, 632);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "海洋时空挖掘与分析系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem databaseManagement;
        private System.Windows.Forms.ToolStripMenuItem linkServerDatabase;
        private System.Windows.Forms.ToolStripMenuItem linkLocalDatabase;
        private System.Windows.Forms.ToolStripMenuItem objectUpdate;
        private System.Windows.Forms.ToolStripMenuItem WPWPObjectUpdate;
        private System.Windows.Forms.ToolStripMenuItem EPWPObjectUpdate;
        private System.Windows.Forms.ToolStripMenuItem stormEventObjectUpdate;
        private System.Windows.Forms.ToolStripMenuItem ENSOObjectUpdate;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceRainfallAnomalyObject;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceAltitudeAnomalyObject;
        private System.Windows.Forms.ToolStripMenuItem oceanColorAnomalyObject;
        private System.Windows.Forms.ToolStripMenuItem SSTAnomalyObject;
        private System.Windows.Forms.ToolStripMenuItem attributeUpdate;
        private System.Windows.Forms.ToolStripMenuItem stormEventDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem clusteringObjectDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem STAssociationRulesDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem SSTAnomalyDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceRainfallAnomalyDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceAltitudeAnomalyDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem oceanColorAnomalyDatabaseTabelPropertyUpdate;
        private System.Windows.Forms.ToolStripMenuItem databaseClose;
        private System.Windows.Forms.ToolStripMenuItem STInfoExtraction;
        private System.Windows.Forms.ToolStripMenuItem ENSOEvent;
        private System.Windows.Forms.ToolStripMenuItem ENSOIndex;
        private System.Windows.Forms.ToolStripMenuItem ENSOEventPartition;
        private System.Windows.Forms.ToolStripMenuItem droughtIndex;
        private System.Windows.Forms.ToolStripMenuItem droughOneDimensionTimeSequence;
        private System.Windows.Forms.ToolStripMenuItem droughTwoDimensionRasterSequence;
        private System.Windows.Forms.ToolStripMenuItem stormEvent;
        private System.Windows.Forms.ToolStripMenuItem oceanAnomalySensitiveObject;
        private System.Windows.Forms.ToolStripMenuItem SSTExtraction;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceRainfallExtraction;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceAltitudeAnomalyExtraction;
        private System.Windows.Forms.ToolStripMenuItem oceanPrimaryProductivityExtraction;
        private System.Windows.Forms.ToolStripMenuItem oceanSurfaceChlorophylAConcentrationExtraction;
        private System.Windows.Forms.ToolStripMenuItem WPWPSTInfoExtraction;
        private System.Windows.Forms.ToolStripMenuItem EPWPSTInfoExtraction;
        private System.Windows.Forms.ToolStripMenuItem STAnomalyDetection;
        private System.Windows.Forms.ToolStripMenuItem rasterSpatialAnomalyDetection;
        private System.Windows.Forms.ToolStripMenuItem spatialStatisticModel;
        private System.Windows.Forms.ToolStripMenuItem spatialClusteringMining;
        private System.Windows.Forms.ToolStripMenuItem spatialTrendSurfaceSimulation;
        private System.Windows.Forms.ToolStripMenuItem rasterTimeAnomalyDetection;
        private System.Windows.Forms.ToolStripMenuItem timeStatisticModel;
        private System.Windows.Forms.ToolStripMenuItem timeClusteringMining;
        private System.Windows.Forms.ToolStripMenuItem timeAnalogueFunction;
        private System.Windows.Forms.ToolStripMenuItem rasterSTAnomalyDetection;
        private System.Windows.Forms.ToolStripMenuItem STStatisticModel;
        private System.Windows.Forms.ToolStripMenuItem STClusteringMining;
        private System.Windows.Forms.ToolStripMenuItem grid;
        private System.Windows.Forms.ToolStripMenuItem rasterSpatialClustering;
        private System.Windows.Forms.ToolStripMenuItem objectSpatialClustering;
        private System.Windows.Forms.ToolStripMenuItem rasterTimeClustering;
        private System.Windows.Forms.ToolStripMenuItem objectTimeClustering;
        private System.Windows.Forms.ToolStripMenuItem spatialAttributeDoubleConstraintsClustering;
        private System.Windows.Forms.ToolStripMenuItem rasterSTClustering;
        private System.Windows.Forms.ToolStripMenuItem rasterSTProcessClustering;
        private System.Windows.Forms.ToolStripMenuItem processTrackClustering;
        private System.Windows.Forms.ToolStripMenuItem stormEventCenterMovingTrackClustering;
        private System.Windows.Forms.ToolStripMenuItem WPWPCenterrMovingTrackClustering;
        private System.Windows.Forms.ToolStripMenuItem easternPacificColdTongueCenterrMovingTrackClustering;
        private System.Windows.Forms.ToolStripMenuItem oceanAnomalyObjectCenterrMovingTrackClustering;
        private System.Windows.Forms.ToolStripMenuItem STCorrelationPattern;
        private System.Windows.Forms.ToolStripMenuItem STAssociationMiningTransactionTableBuilt;
        private System.Windows.Forms.ToolStripMenuItem rasterTransactionTable;
        private System.Windows.Forms.ToolStripMenuItem objectTransactionTable;
        private System.Windows.Forms.ToolStripMenuItem eventTransactionTable;
        private System.Windows.Forms.ToolStripMenuItem rasterAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem objectAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem processAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem ENSORasterAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem ENSOObjectAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem ENSOEventAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem ENSOMovingTrackAssociationModeMining;
        private System.Windows.Forms.ToolStripMenuItem internetCascadeModeMining;
        private System.Windows.Forms.ToolStripMenuItem aboutUs;
        private System.Windows.Forms.ToolStripButton 新建NToolStripButton;
        private System.Windows.Forms.ToolStripButton 打开OToolStripButton;
        private System.Windows.Forms.ToolStripButton 保存SToolStripButton;
        private System.Windows.Forms.ToolStripButton 打印PToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripButton 剪切UToolStripButton;
        private System.Windows.Forms.ToolStripButton 复制CToolStripButton;
        private System.Windows.Forms.ToolStripButton 粘贴PToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton 帮助LToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem visualization;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ToolStripMenuItem dataConversion;
        private System.Windows.Forms.ToolStripMenuItem readTifCoor;
        private System.Windows.Forms.ToolStripMenuItem writeTifCoor;
        private System.Windows.Forms.ToolStripMenuItem 取出一点数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 基于栅格的暴雨提取ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stormTimeExtraction;
        private System.Windows.Forms.ToolStripMenuItem stormSpatialExtraction;
        private System.Windows.Forms.ToolStripMenuItem stormNumberExtraction;
        private System.Windows.Forms.ToolStripMenuItem stormRasterExtraction;
        private System.Windows.Forms.ToolStripMenuItem stormResterToVector;
        private System.Windows.Forms.ToolStripMenuItem EventExtraction;
        private System.Windows.Forms.ToolStripMenuItem 基于矢量的暴雨提取ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stormTimeExtraction2;
        private System.Windows.Forms.ToolStripMenuItem stormSpatialExtraction2;
        private System.Windows.Forms.ToolStripMenuItem stormResterToVectorBasedSpace;
        private System.Windows.Forms.ToolStripMenuItem stormProcessExtract;
        private System.Windows.Forms.ToolStripMenuItem OutToTxt;
        private System.Windows.Forms.ToolStripMenuItem stormEventToVector;
        private System.Windows.Forms.ToolStripMenuItem stormEventStateToVector;
        private System.Windows.Forms.ToolStripMenuItem stormTITAN;
        private System.Windows.Forms.ToolStripMenuItem stormTimeExtraction3;
        private System.Windows.Forms.ToolStripMenuItem stormSpatialExtraction3;
        private System.Windows.Forms.ToolStripMenuItem stormResterToVectorBasedSpace2;
        private System.Windows.Forms.ToolStripMenuItem stormTITANExtract;
        private System.Windows.Forms.ToolStripMenuItem shpToDatabase;
        private System.Windows.Forms.ToolStripMenuItem eventToDatabase;
        private System.Windows.Forms.ToolStripMenuItem eventRelToDatabase;
        private System.Windows.Forms.ToolStripMenuItem 导出为文本ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 暴雨shp属性重新生成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sST转换处理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 海洋热浪ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesTimeAnomaly;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesMedian;
        private System.Windows.Forms.ToolStripMenuItem sST平均图像转换处理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesAverage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwaves90Percent;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesTimeExtraction;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesTimeExtraction2;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesVectorExtraction;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesTracking;
        private System.Windows.Forms.ToolStripMenuItem marineHeatwavesSpeed;
        private System.Windows.Forms.ToolStripMenuItem 流程数据转换处理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 获取文件夹下所有文件名ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 海温事件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 基于矢量的SST提取ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sstTimeExtraction;
        private System.Windows.Forms.ToolStripMenuItem sstSpaceExtraction;
        private System.Windows.Forms.ToolStripMenuItem sstResterToVectorBasedSpace;
        private System.Windows.Forms.ToolStripMenuItem sstProcessExtract;
        private System.Windows.Forms.ToolStripMenuItem sstRestertoVectorExtractInfo;
        private System.Windows.Forms.ToolStripMenuItem 数据库导出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SstEventToVector;
        private System.Windows.Forms.ToolStripMenuItem SSTEventStateToVector;
    }
}

