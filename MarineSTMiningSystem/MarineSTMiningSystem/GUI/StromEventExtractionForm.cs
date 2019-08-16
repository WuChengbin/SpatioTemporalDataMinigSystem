using Oracle.ManagedDataAccess.Client;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class StromEventExtractionForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接
        Thread[] threads;//线程
        OracleConnection[] orcConns;//数据库连接，对应线程
        public StromEventExtractionForm(OracleConnection _conn)
        {
            InitializeComponent();

            conn = _conn;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                eventStateRelationComboBox.Items.Add(tableName[0].ToString());
                eventComboBox.Items.Add(tableName[0].ToString());
            }
            //tableNameComboBox.SelectedIndex = 0;
            //eventStateRelationComboBox.SelectedIndex = 0;
            //eventComboBox.SelectedIndex = 0;
            timeZoneCB.SelectedIndex = 0;

        }
        public StromEventExtractionForm(string connString)
        {
            InitializeComponent();

            conn = new OracleConnection(connString);
            conn.Open();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                eventStateRelationComboBox.Items.Add(tableName[0].ToString());
                eventComboBox.Items.Add(tableName[0].ToString());
            }
            //tableNameComboBox.SelectedIndex = 0;
            //eventStateRelationComboBox.SelectedIndex = 0;
            //eventComboBox.SelectedIndex = 0;
            //timeZoneCB.SelectedIndex = 0;

        }
        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果Table</returns>
        public DataTable QueryResultTable(string sql)
        {
            DataTable ResultDT = new DataTable();
            try
            {
                OracleDataAdapter objAdaper;
                OracleCommand cmd = conn.CreateCommand();
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

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            int eventStateOid = 0;//暴雨状态oid
            DataTable maxEventStateOid = QueryResultTable("SELECT max(oid) FROM " + eventStateTBName);
            if (maxEventStateOid.Rows[0][0] != DBNull.Value)
            {//非空
                eventStateOid = Convert.ToInt32(maxEventStateOid.Rows[0][0]);
            }

            int eventStateRelOid = 0;//暴雨状态关系表oid
            DataTable maxEventStateRelOid = QueryResultTable("SELECT max(oid) FROM " + eventStateRelTBName);
            if (maxEventStateRelOid.Rows[0][0] != DBNull.Value)
            {//非空
                eventStateRelOid = Convert.ToInt32(maxEventStateRelOid.Rows[0][0]);
            }

            int eventOid = 0;//暴雨状态oid
            DataTable maxEventOid = QueryResultTable("SELECT max(oid) FROM " + eventTBName);
            if (maxEventOid.Rows[0][0] != DBNull.Value)
            {//非空
                eventOid = Convert.ToInt32(maxEventOid.Rows[0][0]);
            }

            //List<Shp> shpList = new List<Shp>();//所有图层
            //List<StormShp> stormShpList = new List<StormShp>();//所有暴雨图层
            List<Storm> stormList = new List<Storm>();//所有暴雨
            List<int> stormIdList = new List<int>();//每个暴雨事件对应暴雨id，所有图层的
            //StormShp lastStormShp = new StormShp();//记录上一个暴雨图层
            List<StormShp> stormShpList = new List<StormShp>(); //记录之前的暴雨图层，最大长度为6
            //List<List<int>> stormShpIdListList = new List<List<int>>();//存储所有图层id
            List<int> lastShpStormIdList = new List<int>();//上一个图层所有id
            for (int fileId = 0; fileId < mFileNum; fileId++)
            {//每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                //List<int> shpStormIdList = new List<int>();//当前图层所有id
                string filePath = shpFileNames[fileId].ToString();
                string startTime = Path.GetFileName(filePath).Substring(0, 8) + "_" + Path.GetFileName(filePath).Substring(10, 6);
                string endTime = Path.GetFileName(filePath).Substring(0, 8) + "_" + Path.GetFileName(filePath).Substring(18, 6);
                //Shp shp = new Shp(filePath);//读取shp
                StormShp stormShp = new StormShp(filePath);//读取shp
                //字符串转时间
                stormShp.startTime = DateTime.ParseExact(startTime, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.CurrentCulture);//起始时间
                stormShp.endTime = DateTime.ParseExact(endTime, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.CurrentCulture);//结束时间
                //保存事件状态表
                foreach (Feature feature in stormShp.featureList)
                {
                    int featureId = feature.GetFieldAsInteger("ID");
                    int stormId = feature.GetFieldAsInteger("stormID");
                    int stormPower = feature.GetFieldAsInteger("Power");
                    double stormArea = feature.GetFieldAsDouble("area");
                    double volume = feature.GetFieldAsDouble("Volume");
                    double avgRain = feature.GetFieldAsDouble("AvgRain");
                    double maxRain = feature.GetFieldAsDouble("MaxRain");
                    double minRain = feature.GetFieldAsDouble("MinRain");
                    string polygonWkt = string.Empty;//wkt坐标
                    feature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);
                    double minLog = feature.GetFieldAsDouble("MinLog");
                    double minLat = feature.GetFieldAsDouble("MinLat");
                    double maxLog = feature.GetFieldAsDouble("MaxLog");
                    double maxLat = feature.GetFieldAsDouble("MaxLat");
                    //double area = feature.GetGeometryRef().GetArea();



                    int stateId = 0;//暴雨状态oid
                    //DataTable _stateID = QueryResultTable("SELECT max(state_id) FROM " + eventStateTBName + " where EVENT_ID =" + stormId);
                    //if (_stateID.Rows[0][0] != DBNull.Value)
                    //{//非空
                    //    stateID = Convert.ToInt32(_stateID.Rows[0][0]);
                    //}

                    //存入数据库,解决大字符串问题 https://blog.csdn.net/xx1710/article/details/72667338
                    if (stormIdList.Contains(stormId))
                    {//存在该暴雨
                        int pos = stormIdList.IndexOf(stormId);
                        stateId = stormList[pos].maxStateId;
                    }
                    ++stateId;

                    //构建暴雨多边形
                    StormPolygon stormPoly = new StormPolygon(polygonWkt);
                    stormPoly.id = featureId;//多边形唯一id
                    stormPoly.stormId = stormId;//暴雨id
                    stormPoly.stateId = stateId;//状态id
                    feature.SetField(2, stormId.ToString()+"_"+ stateId.ToString());
                    stormShp.oLayer.SetFeature(feature);
                    stormPoly.power = stormPower;
                    stormPoly.area = stormArea;
                    stormPoly.volume = volume;
                    stormPoly.avgRain = avgRain;
                    stormPoly.maxRain = maxRain;
                    stormPoly.minLog = minLog;
                    stormPoly.minLat = minLat;
                    stormPoly.maxLog = maxLog;
                    stormPoly.maxLat = maxLat;
                    stormPoly.area = stormArea;
                    //stormShp.stormPolyList.Add(stormPoly);//添加暴雨簇
                    //if (!stormShp.stormIdList.Contains(stormPoly.stormId))
                    //{//新的暴雨，记录暴雨id
                    //    stormShp.stormIdList.Add(stormPoly.stormId);
                    //    //shpStormIdList.Add(stormPoly.stormId);
                    //}

                    //构建oracle spatial类型
                    string sdoGtype = string.Empty;
                    string sdoElemInfoArray = string.Empty;
                    string sdoOrdinateArray = string.Empty;
                    stormPoly.GetSdo(out sdoGtype,out sdoElemInfoArray,out sdoOrdinateArray);//获取相关字符串
                    string sdoGeometry = string.Format("MDSYS.SDO_GEOMETRY({0},{1},{2},MDSYS.SDO_ELEM_INFO_ARRAY({3}),MDSYS.SDO_ORDINATE_ARRAY({4}))", sdoGtype, sdoSrid, "NULL", sdoElemInfoArray, sdoOrdinateArray);

                    string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY:= {4}; begin insert into " + eventStateTBName + " values ('{0}','{1}','{2}',TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS'),v_geo,'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}'); end;", ++eventStateOid, stormId, stormId.ToString() + "_" + stateId.ToString(), startTime, sdoGeometry, minLog, minLat, maxLog, maxLat, stormArea, avgRain, volume, maxRain, minRain, stormPower);
                    //string sql = string.Format("insert into " + eventStateTBName + " values ('{0}','{1}','{2}',TO_TIMESTAMP_TZ('{3}','YYYYMMDD_HH24MISS TZH:TZM '),{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", ++eventStateOid, stormId, stormId.ToString() + "_" + stateID.ToString(), startTime + " " + timeZone, sdoGeometry, minLog, minLat, maxLog, maxLat, stormArea, avgRain, volume, maxRain, minRain, stormPower);
                    OracleCommand inserCmd = new OracleCommand(sql, conn);
                    //inserCmd.ExecuteNonQuery();//执行数据库操作

                    //暴雨事件处理
                    if (stormIdList.Contains(stormId))
                    {//存在该暴雨事件
                        int pos = stormIdList.IndexOf(stormId);//查找位置
                        Storm storm = stormList[pos];//取出该暴雨
                        storm.powerSumArea[stormPower - 1] += stormArea;//统计不同暴雨级别面积
                        storm.sumArea += stormArea;//累计面积
                        storm.volume += volume;//累计降雨量
                        storm.maxStateId = stateId;//记录最大状态id
                        if (maxRain > storm.maxRain)
                        {
                            storm.maxRain = maxRain;//最大降雨量
                        }
                        if (minRain < storm.minRain)
                        {
                            storm.minRain = minRain;//最小降雨量
                        }
                        if (avgRain > storm.maxAvgRain)
                        {
                            storm.maxAvgRain = avgRain;//最大平均降雨量
                        }
                        else if (avgRain < storm.minAvgRain)
                        {
                            storm.minAvgRain = avgRain;//最小平均降雨量
                        }
                        storm.endTime = stormShp.endTime;//结束时间
                        if (minLog < storm.minLog) storm.minLog = minLog;//最小经度
                        if (minLat < storm.minLat) storm.minLat = minLat;//最小纬度
                        if (maxLog > storm.maxLog) storm.maxLog = maxLog;//最大经度
                        if (maxLat > storm.maxLat) storm.maxLat = maxLat;//最大纬度
                        storm.stateIdList.Add(stateId);//添加状态id
                        stormList[pos] = storm;//修改暴雨
                    }
                    else
                    {//不存在该暴雨事件
                        Storm storm = new Storm();
                        //storm.polygonList.Add(stormPoly);//添加暴雨多边形
                        storm.id = stormId;//暴雨id
                        storm.powerSumArea[stormPower - 1] = stormArea;//统计不同暴雨级别面积
                        storm.sumArea = stormArea;//累计面积
                        storm.volume = volume;//累计降雨量
                        storm.maxRain = maxRain;//最大降雨量
                        storm.minRain = minRain;//最小降雨量
                        storm.maxAvgRain = avgRain;//最大平均降雨量
                        storm.minAvgRain = avgRain;//最小平均降雨量
                        storm.startTime = stormShp.startTime;//起始时间
                        storm.endTime = stormShp.endTime;//结束时间
                        storm.minLog = minLog;//最小经度
                        storm.minLat = minLat;//最小纬度
                        storm.maxLog = maxLog;//最大经度
                        storm.maxLat = maxLat;//最大纬度
                        storm.maxStateId = stateId;//记录最大状态id
                        storm.stateIdList.Add(stateId);//添加状态id
                        stormList.Add(storm);//记录该暴雨
                        stormIdList.Add(storm.id);//记录暴雨id
                    }
                }
                stormShpList.Add(stormShp);//保存
                if (fileId > 0)
                {//处理到第二个及以后shp文件
                    //StormShp lastStormShp = stormShpList[fileId-1];//上一个暴雨shp
                    for (int i = 0; i < stormShpList.Last().stormPolyList.Count; i++)
                    {//上一个shp每一个要素
                        StormPolygon stormPolyPrior = stormShpList.Last().stormPolyList[i];
                        for (int j = 0; j < stormShp.stormPolyList.Count; j++)
                        {//当前shp每一个要素
                            StormPolygon stormPolyNext = stormShp.stormPolyList[j];
                            if (stormPolyPrior.stormId == stormPolyNext.stormId)
                            {//同一个暴雨
                                if (stormPolyNext.minLog > stormPolyPrior.maxLog || stormPolyNext.minLat > stormPolyPrior.maxLat || stormPolyNext.maxLog < stormPolyPrior.minLog || stormPolyNext.maxLat < stormPolyPrior.minLat) continue;//邻接矩形不重叠，退出本次循环
                                if (stormPolyPrior.Overlap(stormPolyNext))
                                {//重叠
                                    stormPolyNext.parentPosList.Add(i);//记录父多边形位置
                                    stormPolyPrior.childPosList.Add(j);//记录子多边形位置
                                }
                            }
                        }
                    }
                    foreach (StormPolygon stormPolyPrior in stormShpList.Last().stormPolyList)
                    {//上一个shp每一个暴雨多边形
                        bool isSplit = false;//是否分裂
                        if (stormPolyPrior.childPosList.Count > 1)
                        {//分裂
                            isSplit = true;
                        }
                        foreach (int childStateId in stormPolyPrior.childPosList)
                        {//每一个子状态位置
                            bool isMerge = false;//是否合并
                            if (stormShp.stormPolyList[childStateId].parentPosList.Count > 1)
                            {//合并
                                isMerge = true;
                            }
                            string stateAction = "develop";//0：发展 1：分裂 2：合并 3：分裂合并
                            if (isSplit && isMerge) stateAction = "split_then_merge";//分裂合并
                            else if (isSplit) stateAction = "split";//分裂
                            else if (isMerge) stateAction = "merge";//合并
                            string sql = string.Format("insert into " + eventStateRelTBName + " values ('{0}','{1}','{2}','{3}','{4}')", ++eventStateRelOid, stormPolyPrior.stormId, stormPolyPrior.stormId.ToString() + "_" + stormPolyPrior.stateId.ToString(), stormPolyPrior.stormId.ToString() + "_" + stormShp.stormPolyList[childStateId].stateId.ToString(), stateAction);
                            OracleCommand inserCmd = new OracleCommand(sql, conn);
                            //inserCmd.ExecuteNonQuery();
                        }
                    }

                    //暴雨事件处理及保存
                    if (fileId >= 6)
                    {
                        foreach (int stormId in stormShpList[0].stormIdList)
                        {//第一个图层的每个事件id
                            bool stormEnd = true;//默认暴雨结束
                            for (int i = 1; i < stormShpList.Count; i++)
                            {//后面的每个图层
                                if (stormShpList[i].stormIdList.Contains(stormId))
                                {//没有结束
                                    stormEnd = false;
                                    break;
                                }
                            }
                            if (stormEnd)
                            {//比较最新的图层
                                if (stormShp.stormIdList.Contains(stormId))
                                {//没有结束
                                    stormEnd = false;
                                }
                            }
                            if (stormEnd)
                            {//该暴雨结束
                                int pos = stormIdList.IndexOf(stormId);//查找位置
                                Storm storm = stormList[pos];//取出该暴雨
                                ++eventOid;
                                storm.oid = eventOid;
                                //SaveStormToDataBase(storm);//暴雨存到数据库

                                int tId = 0;//线程id
                                while (threads[tId] != null && threads[tId].IsAlive == true)
                                {//线程在执行
                                    tId++;
                                    if (tId >= threads.Length) tId = 0;
                                }
                                storm.threadId = tId;//记录线程id
                                threads[tId] = new Thread(new ParameterizedThreadStart(SaveStormToDataBase));
                                threads[tId].IsBackground = true;
                                threads[tId].Start(storm);

                                //移除该暴雨
                                stormIdList.RemoveAt(pos);
                                stormList.RemoveAt(pos);
                            }
                        }
                    }
                    if (fileId >= mFileNum - 1)
                    {//最后一个文件
                        for (int i = 0; i < stormList.Count; i++)
                        {
                            //重新计算进度
                            if (worker.CancellationPending)
                            {//取消
                                e.Cancel = true;
                                return;
                            }
                            progress = (i * 100) / stormList.Count;//进度
                            worker.ReportProgress(progress);//记录进度

                            Storm storm = stormList[i];
                            ++eventOid;
                            storm.oid = eventOid;
                            int tId = 0;//线程id
                            while (threads[tId] != null && threads[tId].ThreadState != ThreadState.Stopped)
                            {//线程在执行
                                tId++;
                                if (tId >= threads.Length) tId = 0;
                            }
                            storm.threadId = tId;//记录线程id
                            threads[tId] = new Thread(new ParameterizedThreadStart(SaveStormToDataBase));
                            threads[tId].IsBackground = true;
                            threads[tId].Start(storm);
                            //SaveStormToDataBase(storm, eventOid);//暴雨存到数据库
                        }
                        //foreach(Storm storm in stormList)
                        //{
                        //    ++eventOid;
                        //    SaveStormToDataBase(storm, eventOid);//暴雨存到数据库
                        //}
                        stormIdList.Clear();
                        stormList.Clear();

                        foreach (StormShp _stormShp in stormShpList)
                        {
                            //_stormShp.ReSave();
                            _stormShp.Dispose();//释放及保存
                        }
                        stormShpList.Clear();
                    }
                }
                //stormShp.ReSave();
                stormShpList.Add(stormShp);//记录图层
                if (stormShpList.Count > 6)
                {
                    //stormShpList[0].ReSave();
                    stormShpList[0].Dispose();//释放及保存
                    stormShpList.RemoveAt(0);//移除第一个图层
                }
                lastShpStormIdList = stormShp.stormIdList;//记录图层暴雨id
            }
            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped)
                    {
                        isEnd = false;
                        break;
                    }
                }
                if (isEnd)
                {
                    break;
                }
            }
        }
        
        void SaveStormToDataBase(object o)
        {//保存暴雨到数据库
            Storm storm = (Storm)o;
            int oid = storm.oid;
            Console.WriteLine("开始处理oid" + oid);
            if (storm.powerSumArea[0] > (storm.powerSumArea[1] + storm.powerSumArea[2])) storm.power = 1;//暴雨
            else if (storm.powerSumArea[1] > storm.powerSumArea[2]) storm.power = 2;//大暴雨
            else storm.power = 3;//特大暴雨
            storm.duration = storm.endTime - storm.startTime;
            storm.avgRain = storm.volume / (storm.sumArea* timeCell) * 0.001;//累计降雨量为立方米，累计面积为平方千米，平均降雨量为毫米
            storm.maxAvgRain = storm.maxAvgRain / timeCell;//换算为每小时
            storm.minAvgRain = storm.minAvgRain / timeCell;
            //string geoSpl = storm.GetGeoSpl(eventStateTBName);//获取Geometry创建语句
            //string qSql = string.Format("SELECT to_char(space) FROM {0} where state_id='{1}_1'", eventStateTBName, storm.id);//查询语句
            //DataTable olcGeoDt = QueryResultTable(qSql);//执行查询
            //string olcGeo = olcGeoDt.Rows[0][0].ToString();//暴雨多边形
            string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY; begin select space into v_geo from "+eventStateTBName+" where state_id='{1}_1'; insert into " + eventTBName + " values ('{0}','{1}',TO_TIMESTAMP('{2}','YYYYMMDD_HH24MISS'),TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS'),'{4}',(v_geo),'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}'); end;", oid, storm.id, storm.startTime.ToString("yyyyMMdd_HHmmss"), storm.endTime.ToString("yyyyMMdd_HHmmss"), storm.duration.TotalHours.ToString("f2"), "", storm.minLog, storm.minLat, storm.maxLog, storm.maxLat, storm.avgRain, storm.volume, storm.maxAvgRain, storm.minAvgRain, storm.power);
            //string sql = string.Format("insert into " + eventTBName + " values ('{0}','{1}',TO_TIMESTAMP_TZ('{2}','YYYYMMDD_HH24MISS TZH:TZM '),TO_TIMESTAMP_TZ('{3}','YYYYMMDD_HH24MISS TZH:TZM '),'{4}',({5}),'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", oid, storm.id, storm.startTime.ToString("yyyyMMdd_HHmmss") + " " + timeZone, storm.endTime.ToString("yyyyMMdd_HHmmss") + " " + timeZone, storm.duration.TotalHours.ToString("f2"), geoSpl, storm.minLog, storm.minLat, storm.maxLog, storm.maxLat, storm.avgRain, storm.volume, storm.maxAvgRain, storm.minAvgRain, storm.power);
            OracleCommand inserCmd = new OracleCommand(sql, orcConns[storm.threadId]);
            //inserCmd.ExecuteNonQuery();//执行数据库操作

            //合并后面的状态
            for(int i=1;i<storm.stateIdList.Count;i++)
            {
                sql = string.Format("declare v_geo1 MDSYS.SDO_GEOMETRY;v_geo2 MDSYS.SDO_GEOMETRY; begin select space into v_geo1 from {0} where id={1}; select space into v_geo2 from {2} where state_id='{1}_{3}'; update " + eventTBName + " set space=SDO_GEOM.SDO_UNION(v_geo1,v_geo2,{4}) where id={1}; end;", eventTBName, storm.id, eventStateTBName,i,0.001);

                inserCmd = new OracleCommand(sql, orcConns[storm.threadId]);
                //inserCmd.ExecuteNonQuery();//执行数据库操作
            }
            Console.WriteLine("结束处理oid" + oid);
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

            //停止数据库连接
            foreach (OracleConnection orcConn in orcConns)
            {
                orcConn.Close();
            }
            tipLabel.Text = "";
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            if(e.ProgressPercentage==0)
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
                tipLabel.Text = "已完成：" + e.ProgressPercentage + "%，用时：" + costTime.ToString(@"d\.hh\:mm\:ss") + "，剩余时间：" + needTime.ToString(@"d\.hh\:mm\:ss");
            }
        }

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "shp文件|*.shp|所有文件|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox1.Items.Add(fileName);
                }
                countTextBox.Text = listBox1.Items.Count.ToString();
            }
        }

        private void deleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            while (index > -1)
            {
                listBox1.Items.RemoveAt(index);
                index = listBox1.SelectedIndex;
            }
            countTextBox.Text = listBox1.Items.Count.ToString();
        }

        private void moveUpBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(endIndex + 1, listBox1.Items[startIndex - 1].ToString());
                    listBox1.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void moveDownBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox1.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(startIndex, listBox1.Items[endIndex + 1].ToString());
                    listBox1.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        ListBox.ObjectCollection shpFileNames;//所有文件名
        int mFileNum = 0;
        string eventStateTBName = string.Empty;//事件状态表
        string eventStateRelTBName = string.Empty;//事件状态关系表
        string eventTBName = string.Empty;//事件表
        //string timeZone = "";
        double timeCell = 0.5;//时间间隔
        DateTime pStartTime;//处理开始时间
        string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID
        int threadCount = 1;
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("请添加处理文件！");
                return;
            }
            shpFileNames = listBox1.Items;
            mFileNum = shpFileNames.Count;//文件个数
            eventStateTBName = tableNameComboBox.Text.Trim();
            eventStateRelTBName = eventStateRelationComboBox.Text.Trim();
            eventTBName = eventComboBox.Text.Trim();
            //timeZone = timeZoneCB.Text.Trim();
            threadCount = Convert.ToInt32(threadCountTextBox.Text.Trim());


            threads = new Thread[threadCount];//线程数
            orcConns = new OracleConnection[threadCount];//数据库连接数

            //数据库连接
            //for (int i = 0; i < orcConns.Length; i++)
            //{
            //    orcConns[i] = DatabaseConnection.GetNewOracleConnection();//获取连接
            //    orcConns[i].Open();//打开连接
            //}
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

        private void cancelBtn_Click(object sender, EventArgs e)
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

        private void StromEventExtractionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
