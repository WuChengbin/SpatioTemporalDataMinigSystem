﻿using Oracle.ManagedDataAccess.Client;
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
    public partial class SstEventToVectorForm : Form
    {
        private OracleConnection conn;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        Thread[] threads;//线程
        OracleConnection[] orcConns;//数据库连接，对应线程
        public SstEventToVectorForm(OracleConnection _conn)
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
            }
            tableNameComboBox.SelectedIndex = 0;
            //eventStateRelationComboBox.SelectedIndex = 0;
            //eventComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果Table</returns>
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

        struct Parameter
        {//参数结构体
            public int _oid;
            public int _threadId;
            public int _startOid;
            public int _endOid;

            public Parameter(int _oid, int _threadId) : this()
            {
                this._oid = _oid;
                this._threadId = _threadId;
            }

            public Parameter(int _startOid, int _endOid, int _threadId) : this()
            {
                this._startOid = _startOid;
                this._endOid = _endOid;
                this._threadId = _threadId;
            }
        }
        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            int minOid = 0;//oid
            int maxOid = -1;//
            DataTable minOidR = QueryResultTable("SELECT min(oid) FROM " + tableName, conn);
            if (minOidR.Rows[0][0] != DBNull.Value)
            {//非空
                minOid = Convert.ToInt32(minOidR.Rows[0][0]);
            }

            DataTable maxOidR = QueryResultTable("SELECT max(oid) FROM " + tableName, conn);
            if (maxOidR.Rows[0][0] != DBNull.Value)
            {//非空
                maxOid = Convert.ToInt32(maxOidR.Rows[0][0]);
            }

            if (maxOid - minOid <= 0) return;
            
            for (int startOid = minOid; startOid <= maxOid; startOid += 1000)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = startOid * 100 / (maxOid - minOid + 1);//进度
                worker.ReportProgress(progress);//记录进度

                int endOid = startOid + 999;
                if (endOid > maxOid) endOid = maxOid;

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(SstToShpRange));
                threads[tId].IsBackground = true;
                Parameter p = new Parameter(startOid, endOid, tId);
                threads[tId].Start(p);
            }
        }

        private void SstToShpRange(object obj)
        {
            Parameter p = (Parameter)obj;//保存shp
            string strVectorFile1 = outFolderPath; string strDriver = "ESRI Shapefile";
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
            Layer olayer1 = ds1.CreateLayer("Sst_Oid" + p._startOid + "-" + p._endOid, sr, wkbGeometryType.wkbPolygon, null);//接下来创建属性表字段
            #region 创建属性字段
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldID = new FieldDefn("OID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("PRID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldStormID, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime = new FieldDefn("STime", FieldType.OFTString);
            oFieldStartTime.SetWidth(20);
            olayer1.CreateField(oFieldStartTime, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldEndTime = new FieldDefn("ETime", FieldType.OFTString);
            oFieldEndTime.SetWidth(20);
            olayer1.CreateField(oFieldEndTime, 1);

            //创建x坐标字段
            FieldDefn oFieldDuration = new FieldDefn("DurTime", FieldType.OFTReal);
            oFieldDuration.SetWidth(10);
            oFieldDuration.SetPrecision(8);
            olayer1.CreateField(oFieldDuration, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldAvgRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMinRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            olayer1.CreateField(oFieldMaxRainFall, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog.SetWidth(10);
            oFieldMinLog.SetPrecision(8);
            olayer1.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(10);
            oFieldMinLat.SetPrecision(8);
            olayer1.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(10);
            oFieldMaxLog.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(10);
            oFieldMaxLat.SetPrecision(8);
            olayer1.CreateField(oFieldMaxLat, 1);

            //创建
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower.SetWidth(20);
            oFieldPower.SetPrecision(8);
            olayer1.CreateField(oFieldPower, 1);

            //创建
            FieldDefn oFieldTotalVolume = new FieldDefn("TotalVolume", FieldType.OFTReal);
            oFieldTotalVolume.SetWidth(20);
            oFieldTotalVolume.SetPrecision(8);
            olayer1.CreateField(oFieldTotalVolume, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldTrajectoryID = new FieldDefn("TrajectoryID", FieldType.OFTInteger);
            olayer1.CreateField(oFieldTrajectoryID, 1);
            #endregion

            for (int oid = p._startOid; oid <= p._endOid; oid++)
            {
                //string sql = "select oid,id,start_time,end_time,duration,sdo_geometry.get_wkt(space),min_log,min_lat,max_log,max_lat,i_mean,i_max,i_min from " + tableName + " where oid=" + Convert.ToInt32(oid);

                //string sql = "select oid,id,starttime,endtime,durtime,avgvalue,minvalue,maxvalue,minlon,minlat,maxlon,maxlat,power,totalvolume,trajectoryid,sdo_geometry.get_wkt(space) from " 
                //    + tableName + " where oid = TO_NUMBER('" + oid.ToString() + "','9G999D99')";
                string sql = "select OID,ID,StartTime,EndTime,DurTime,AvgValue,MinValue,MaxValue,MinLon,MinLat,MaxLon,MaxLat,Power,TotalVolume,Trajectory_ID,sdo_geometry.get_wkt(space) from "
                       + tableName + " where OID = '" + Convert.ToInt32(oid) + "'";
                DataTable dtResult = QueryResultTable(sql, orcConns[p._threadId]);
                if (dtResult.Rows.Count <= 0) continue;
                DataRow result = dtResult.Rows[0];
                #region 写入数据
                FeatureDefn oDefn = olayer1.GetLayerDefn();
                Feature oFeature = new Feature(oDefn);
                oFeature.SetField(0, Convert.ToInt32(result[0]));
                oFeature.SetField(1, Convert.ToInt32(result[1]));
                oFeature.SetField(2, ((DateTime)result[2]).ToString("yyyy-MM-dd HH:mm:ss"));
                oFeature.SetField(3, ((DateTime)result[3]).ToString("yyyy-MM-dd HH:mm:ss"));

                oFeature.SetField(4, Convert.ToDouble(result[4]));

                oFeature.SetField(5, Convert.ToDouble(result[9]));
                oFeature.SetField(6, Convert.ToDouble(result[12]));
                oFeature.SetField(7, Convert.ToDouble(result[11]));
                oFeature.SetField(8, Convert.ToDouble(result[5]));

                oFeature.SetField(9, Convert.ToDouble(result[6]));
                oFeature.SetField(10, Convert.ToDouble(result[7]));
                oFeature.SetField(11, Convert.ToDouble(result[8]));

                oFeature.SetField(12, Convert.ToDouble(result[13]));
                oFeature.SetField(13, Convert.ToDouble(result[10]));
                oFeature.SetField(14, Convert.ToDouble(result[14]));
                //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
                string polygonStr = result[15].ToString();
                Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
                oFeature.SetGeometry(geoPolygon);
                olayer1.CreateFeature(oFeature);
                geoPolygon.Dispose();
                #endregion
                //释放资源
                oDefn.Dispose();
                oFeature.Dispose();
            }
            olayer1.Dispose();
            ds1.Dispose();
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

        string tableName = string.Empty;
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

            tableName = tableNameComboBox.Text.Trim();

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
