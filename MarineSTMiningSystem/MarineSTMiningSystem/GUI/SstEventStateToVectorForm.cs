using Oracle.ManagedDataAccess.Client;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem.GUI
{
    public partial class SstEventStateToVectorForm : Form
    {
        private OracleConnection conn;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        Thread[] threads;//线程
        OracleConnection[] orcConns;//数据库连接，对应线程
        bool outParentsChildren = false;//是否导出父子节点

        public SstEventStateToVectorForm(OracleConnection _conn)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            // 为了使属性表字段支持中文，请添加下面这句
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            Ogr.RegisterAll();

            this.conn = _conn;

            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables", conn);
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                tableNameComboBox2.Items.Add(tableName[0].ToString());
            }
        }

        struct Parameter
        {//参数结构体
            //public int _oid;
            public int _threadId;
            public DateTime _startTime;

            public Parameter(int _threadId, DateTime _startTime) : this()
            {
                this._threadId = _threadId;
                this._startTime = _startTime;
            }
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            DataTable startTimes = QueryResultTable("select distinct Time from " + stateTableName + " order by Time", conn);
            if (startTimes.Rows[0][0] == DBNull.Value)
            {//非空
                return;
            }

            for (int timeId = 0; timeId < startTimes.Rows.Count; timeId++)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = timeId * 100 / startTimes.Rows.Count;//进度
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(SstToShpTime));
                threads[tId].IsBackground = true;
                Parameter p = new Parameter(tId, (DateTime)startTimes.Rows[timeId][0]);
                threads[tId].Start(p);
            }
        }

        private void SstToShpTime(object obj)
        {
            Parameter p = (Parameter)obj;//保存shp
            string strVectorFile1 = outFolderPath;
            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            DataSource ds1 = oDriver.CreateDataSource(strVectorFile1, null);
            if (ds1 == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }

            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            Layer olayer1 = ds1.CreateLayer(p._startTime.ToString("yyyyMMdd_HHmmss"), sr, wkbGeometryType.wkbPolygon, null);

            #region 接下来创建属性表字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("OID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("PRID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormStateID = new FieldDefn("STID", FieldType.OFTString);
            oFieldStormStateID.SetWidth(20);
            olayer1.CreateField(oFieldStormStateID, 1);

            if (outParentsChildren)
            {
                // 先创建一个叫FieldID的整型属性
                FieldDefn oFieldParentsID = new FieldDefn("ParentsID", FieldType.OFTString);
                oFieldParentsID.SetWidth(50);
                olayer1.CreateField(oFieldParentsID, 1);

                // 先创建一个叫FieldID的整型属性
                FieldDefn oFieldChildrenID = new FieldDefn("ChildrenID", FieldType.OFTString);
                oFieldChildrenID.SetWidth(50);
                olayer1.CreateField(oFieldChildrenID, 1);
            }

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldStartTime.SetWidth(20);
            olayer1.CreateField(oFieldStartTime, 1);

            //// 再创建一个叫FeatureName的字符型属性，字符长度为50
            //FieldDefn oFieldEndTime = new FieldDefn("EndTime", FieldType.OFTString);
            //oFieldEndTime.SetWidth(20);
            //olayer1.CreateField(oFieldEndTime, 1);

            ////创建x坐标字段
            //FieldDefn oFieldDuration = new FieldDefn("Duration", FieldType.OFTReal);
            //oFieldDuration.SetWidth(10);
            //oFieldDuration.SetPrecision(8);
            //olayer1.CreateField(oFieldDuration, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog.SetWidth(20);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建平均降雨量字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            olayer1.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            //创建体积字段
            FieldDefn oFieldVolume = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldVolume.SetWidth(20);
            oFieldVolume.SetPrecision(8);
            olayer1.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            //FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            //oFieldLongTime.SetWidth(20);
            //olayer1.CreateField(oFieldLongTime, 1);

            //创建
            FieldDefn oFieldCoreLog = new FieldDefn("CoreLon", FieldType.OFTReal);
            oFieldCoreLog.SetWidth(20);
            oFieldCoreLog.SetPrecision(8);
            olayer1.CreateField(oFieldCoreLog, 1);

            //创建
            FieldDefn oFieldCoreLat = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldCoreLat.SetWidth(20);
            oFieldCoreLat.SetPrecision(8);
            olayer1.CreateField(oFieldCoreLat, 1);

            //创建
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower.SetWidth(20);
            oFieldPower.SetPrecision(8);
            olayer1.CreateField(oFieldPower, 1);
            #endregion

            //string sql = "select oid,event_id,state_id,time,sdo_geometry.get_wkt(space),min_log,min_lat,max_log,max_lat,area,i_mean,i_max,i_min,core_log,core_lat from " 
            //    + stateTableName + " where time= to_date('" + p._startTime.ToString("yyyyMMdd HHmmss") + "','yyyymmdd hh24miss')";

            string sql = "select OID,EVENT_ID,STATE_ID,Time,MinLon,MinLat,MaxLon,MaxLat,Area,AvgValue,MaxValue,MinValue,Power,CoreLon,CoreLat,sdo_geometry.get_wkt(space) from "
                + stateTableName + " where Time = to_date('" + p._startTime.ToString("yyyyMMdd_HHmmss") + "','yyyymmdd hh24miss')";
            DataTable dtResult = QueryResultTable(sql, orcConns[p._threadId]);

            for (int id = 0; id < dtResult.Rows.Count; id++)
            {
                //写入数据
                FeatureDefn oDefn = olayer1.GetLayerDefn();
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField("OID", Convert.ToInt32(dtResult.Rows[id][0]));
                oFeature.SetField("PRID", Convert.ToInt32(dtResult.Rows[id][1]));
                oFeature.SetField("STID", (dtResult.Rows[id][2]).ToString());
                if (outParentsChildren)
                {//导出父子节点关系
                    string sql2 = "select PRIOR_STATE_ID from " + relTableName + " where NEXT_STATE_ID ='" + dtResult.Rows[id][2].ToString() + "'";
                    DataTable dtResult2 = QueryResultTable(sql2, orcConns[p._threadId]);
                    string parentsIdStr = string.Empty;
                    foreach (DataRow row in dtResult2.Rows)
                    {
                        parentsIdStr += row[0].ToString() + " ";
                    }
                    parentsIdStr.TrimEnd(' ');
                    oFeature.SetField("ParentsID", parentsIdStr);
                    
                    string sql3 = "select NEXT_STATE_ID from " + relTableName + " where PRIOR_STATE_ID ='" + dtResult.Rows[id][2].ToString() + "'";
                    DataTable dtResult3 = QueryResultTable(sql3, orcConns[p._threadId]);
                    string childrenIdStr = string.Empty;
                    foreach (DataRow row in dtResult3.Rows)
                    {
                        childrenIdStr += row[0].ToString() + " ";
                    }
                    childrenIdStr.TrimEnd(' ');
                    oFeature.SetField("ChildrenID", childrenIdStr);
                }
                oFeature.SetField("Time", ((DateTime)dtResult.Rows[id][3]).ToString("yyyy-MM-dd HH:mm:ss"));

                oFeature.SetField("MinLon", Convert.ToDouble(dtResult.Rows[id][4]));
                oFeature.SetField("MinLat", Convert.ToDouble(dtResult.Rows[id][5]));
                oFeature.SetField("MaxLon", Convert.ToDouble(dtResult.Rows[id][6]));
                oFeature.SetField("MaxLat", Convert.ToDouble(dtResult.Rows[id][7]));
                oFeature.SetField("Area", Convert.ToDouble(dtResult.Rows[id][8]));

                oFeature.SetField("AvgValue", Convert.ToDouble(dtResult.Rows[id][9]));
                oFeature.SetField("MaxValue", Convert.ToDouble(dtResult.Rows[id][10]));
                oFeature.SetField("MinValue", Convert.ToDouble(dtResult.Rows[id][11]));
                //DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
                //long timeStamp = (long)(((DateTime)dtResult.Rows[id][3]) - time1970).TotalSeconds; // 相差秒数
                //oFeature.SetField("LongTime", timeStamp.ToString());
                oFeature.SetField("Power", Convert.ToDouble(dtResult.Rows[id][12]));
                oFeature.SetField("CoreLon", Convert.ToDouble(dtResult.Rows[id][13]));
                oFeature.SetField("CoreLat", Convert.ToDouble(dtResult.Rows[id][14]));
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = dtResult.Rows[id][15].ToString();//MDSYS.SDO_GEOMETRY
                //string polygonStr = sdo_geometry.get_wkt("MDSYS.SDO_GEOMETRY");
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);
                //释放资源
                oDefn.Dispose();
                geoPolygon.Dispose();
                oFeature.Dispose();
            }
            olayer1.Dispose();
            ds1.Dispose();
        }

        public DataTable QueryResultTable(string sql, OracleConnection _conn)
        {
            DataTable ResultDT = new DataTable();
            try
            {
                OracleDataAdapter objAdaper;
                OracleCommand cmd = _conn.CreateCommand();
                cmd.CommandText = sql;
                objAdaper = new OracleDataAdapter(cmd);
                objAdaper.Fill(ResultDT);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return ResultDT;
        }
        
        string stateTableName = string.Empty;
        string relTableName = string.Empty;
        DateTime pStartTime;//处理开始时间
        int threadCount = 1;
        string outFolderPath = string.Empty;
        private void okButton_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }

            outParentsChildren = checkBox1.Checked;

            stateTableName = tableNameComboBox.Text.Trim();
            relTableName = tableNameComboBox2.Text.Trim();

            outFolderPath = textBox1.Text.Trim();
            if (!outFolderPath.EndsWith("\\")) outFolderPath += "\\";

            threadCount = Convert.ToInt32(threadCountTextBox.Text.Trim());
            threads = new Thread[threadCount];

            orcConns = new OracleConnection[threadCount];//数据库连接数
            
            //数据库连接
            for (int i = 0; i < orcConns.Length; i++)
            {
                orcConns[i] = new OracleConnection(conn.ConnectionString);//获取连接
                orcConns[i].Open();//打开连接
            }

            pStartTime = DateTime.Now;//记录处理开始时间
            progressBar1.Show();
            worker.RunWorkerAsync();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();//取消后台
            }
            else
            {
                //toolStripStatusLabel1.Text = "未进行处理";
                MessageBox.Show("未进行处理！");
            }
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (e.Cancelled)
            {//用户取消
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                }
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {//异常结束
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                }
                MessageBox.Show(e.Error.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                progressBar1.Value = 100;
                TimeSpan costTime = DateTime.Now - pStartTime;//用时
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            toolStripStatusLabel1.Text = "";
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            if (e.ProgressPercentage == 0)
            {
                pStartTime = DateTime.Now;
            }
            progressBar1.Value = e.ProgressPercentage;
            TimeSpan costTime = DateTime.Now - pStartTime;//已经花费时间
            double costSeconds = costTime.TotalSeconds;//花费秒数
            if (e.ProgressPercentage > 0)
            {
                double needSeconds = costSeconds * (100 - e.ProgressPercentage) / e.ProgressPercentage;//还需要的秒数
                TimeSpan needTime = new TimeSpan(0, 0, (int)(needSeconds));
                toolStripStatusLabel1.Text = "已完成：" + e.ProgressPercentage + "%，用时：" + costTime.ToString(@"d\.hh\:mm\:ss") + "，剩余时间：" + needTime.ToString(@"d\.hh\:mm\:ss");
            }
        }
    }
}
