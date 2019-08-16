using Oracle.ManagedDataAccess.Client;
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
    public partial class StormTITANExtractForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接
        Thread[] threads;//线程
        OracleConnection[] orcConns;//数据库连接，对应线程
        string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID

        public StormTITANExtractForm(OracleConnection _conn)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            conn = _conn;
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

        public StormTITANExtractForm(string connString)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            conn = new OracleConnection(connString);
            conn.Open();
            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                eventStateRelationComboBox.Items.Add(tableName[0].ToString());
                eventComboBox.Items.Add(tableName[0].ToString());
            }
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

        StormShp[] stormShps; //记录暴雨图层
        List<StormPolygonList>[] stormPolyListsTime;//用来储存所有时间暴雨链
        List<Storm> stormList = new List<Storm>();//记录暴雨最开始节点
        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            stormShps = new StormShp[mFileNum];//清空
            stormPolyListsTime = new List<StormPolygonList>[mFileNum];
            stormList.Clear();

            for (int fileId = 0; fileId < mFileNum; fileId++)
            {//每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(ReadShp));
                threads[tId].IsBackground = true;
                threads[tId].Start(fileId);
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) isEnd = false;
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }

            for (int index = 0; index < stormShps.Length - 1; index++)
            {//每个图层
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (index * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(PolyConnect));
                threads[tId].IsBackground = true;
                threads[tId].Start(index);
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) isEnd = false;
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }

            //生成暴雨链
            for (int index = 0; index < stormShps.Length; index++)
            {//遍历每个图层
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (index * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(PolyToList));
                threads[tId].IsBackground = true;
                threads[tId].Start(index);
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) isEnd = false;
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }

            //暴雨链连接
            for (int fileId = 0; fileId < stormPolyListsTime.Length; fileId++)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(PolyListConnect));
                threads[tId].IsBackground = true;
                threads[tId].Start(fileId);


            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) isEnd = false;
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }

            //遍历编号
            int stormId = 1;//暴雨id从1开始
            for (int index = 0; index < stormShps.Length; index++)
            {//每个图层
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (index * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                StormShp stormShp = stormShps[index];//shp
                List<StormPolygon> stormPolyList = stormShp.stormPolyList;//所有多边形
                foreach (StormPolygon stormPoly in stormPolyList)
                {//每个暴雨多边形
                    if (stormPoly.isOrder) continue;//已经编过号了
                    Storm storm = new Storm();
                    storm.startTime = stormPoly.startTime;//记录开始时间
                    storm.endTime = stormPoly.endTime;//结束时间
                    storm.id = stormId;
                    storm.maxStateId = 0;
                    storm.headPoly = stormPoly;
                    OrderStorm(stormPoly, ref storm);//遍历所有多边形并计算属性
                    stormList.Add(storm);//记录新暴雨
                    stormId++;
                }
            }

            //数据库存储
            eventOid = 1;
            eventStateOid = 1;//事件状态表oid
            eventStateRelOid = 1;//关系表oid
            for (int index = 0; index < stormList.Count - 1; index++)
            {//每个图层
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (index * 100) / stormList.Count;//进度
                worker.ReportProgress(progress);//记录进度

                Storm storm = stormList[index];//取出暴雨
                storm.duration = storm.endTime - storm.startTime;
                if (storm.powerSumArea[0] > (storm.powerSumArea[1] + storm.powerSumArea[2])) storm.power = 1;//暴雨
                else if (storm.powerSumArea[1] > storm.powerSumArea[2]) storm.power = 2;//大暴雨
                else storm.power = 3;//特大暴雨
                storm.avgRain = storm.volume / (storm.sumArea * timeCell) * 0.001;//累计降雨量为立方米，累计面积为平方千米，平均降雨量为毫米
                storm.maxAvgRain = storm.maxAvgRain / timeCell;//换算为每小时
                storm.minAvgRain = storm.minAvgRain / timeCell;
                //if (storm.duration.TotalHours <= timeCell) continue;//单独暴雨点剔除
                if (storm.maxAvgRain < oneHourRainFloor && (storm.maxAvgRain * storm.duration.TotalHours) < oneDayRainFloor)
                {//一小时最大降雨量不满足且24小时不满足
                    continue;
                }

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(StormToDatabase));
                threads[tId].IsBackground = true;
                storm.threadId = tId;
                storm.oid = index + 1;
                threads[tId].Start(storm);
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) isEnd = false;
                    break;
                }
                if (isEnd)
                {
                    break;
                }
            }
        }

        private void StormToDatabase(object obj)
        {
            Storm storm = (Storm)obj;//取出暴雨
            StormPolygon stormHead = storm.headPoly;//一个暴雨头
            SaveToDataBase(stormHead, storm.threadId);//保存到数据库
            SaveToDataBase(storm);
        }

        struct Param1
        {
            public int index;
            public int tId;
        }

        int eventOid;//关系表oid
        private static object objlock6 = new object();
        //暴雨保存到数据库
        private void SaveToDataBase(Storm storm)
        {
            //Storm storm = (Storm)o;
            int oid = 0;
            lock (objlock6)
            {
                oid = eventOid++;
            }
            //Console.WriteLine("开始处理oid" + oid);
            

            string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY; begin select space into v_geo from " + eventStateTBName + " where state_id='{1}_1'; insert into " + eventTBName + " values ('{0}','{1}',TO_TIMESTAMP('{2}','YYYYMMDD_HH24MISS'),TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS '),'{4}',(v_geo),'{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}'); end;", oid, storm.id, storm.startTime.ToString("yyyyMMdd_HHmmss"), storm.endTime.ToString("yyyyMMdd_HHmmss"), storm.duration.TotalHours.ToString("f2"), "", storm.minLog, storm.minLat, storm.maxLog, storm.maxLat, storm.avgRain, storm.volume, storm.maxAvgRain, storm.minAvgRain, storm.power);

            OracleCommand inserCmd = new OracleCommand(sql, orcConns[storm.threadId]);
            inserCmd.ExecuteNonQuery();//执行数据库操作

            //合并后面的状态
            for (int i = 1; i < storm.stateIdList.Count; i++)
            {
                sql = string.Format("declare v_geo1 MDSYS.SDO_GEOMETRY;v_geo2 MDSYS.SDO_GEOMETRY; begin select space into v_geo1 from {0} where id={1}; select space into v_geo2 from {2} where state_id='{1}_{3}'; update " + eventTBName + " set space=SDO_GEOM.SDO_UNION(v_geo1,v_geo2,{4}) where id={1}; end;", eventTBName, storm.id, eventStateTBName, i, 0.001);

                inserCmd = new OracleCommand(sql, orcConns[storm.threadId]);
                inserCmd.ExecuteNonQuery();//执行数据库操作
            }
            //Console.WriteLine("结束处理oid" + oid);
        }

        //暴雨状态保存到数据库
        private void SaveToDataBase(StormPolygon stormPoly, int tId)
        {
            if (stormPoly.isSaveToDB) return;
            stormPoly.isSaveToDB = true;
            SaveToStateTB(stormPoly, tId);//保存到状态数据库
            if (stormPoly.stormId == 2)
            {
                int a = 0;
            }
            //遍历子节点
            foreach (StormPolygon stormPolyNext in stormPoly.childList)
            {
                SaveToDataBase(stormPolyNext, tId);//遍历所有多边形并编号
                SaveToRelatTB(stormPoly, stormPolyNext, tId);//保存到关系表
            }
            //遍历父节点
            foreach (StormPolygon stormPolyPrev in stormPoly.parentList)
            {
                SaveToDataBase(stormPolyPrev, tId);//遍历所有多边形并编号
                //SaveToRelatTB(stormPolyPrev, stormPoly, tId, ref eventStateRelOid);//保存到关系表
            }
        }

        //保存关系
        int eventStateRelOid;//关系表oid
        private static object objlock1 = new object();
        private void SaveToRelatTB(StormPolygon stormPolyPrior, StormPolygon stormPolyNext, int tId)
        {
            int oid = 0;
            lock (objlock1)
            {
                oid = eventStateRelOid++;
            }

            bool isSplit = false;//是否分裂
            if (stormPolyPrior.childList.Count > 1)
            {//分裂
                isSplit = true;
            }
            bool isMerge = false;//是否合并
            if (stormPolyNext.parentList.Count > 1)
            {//合并
                isMerge = true;
            }
            string stateAction = "develop";//0：发展 1：分裂 2：合并 3：分裂合并
            if (isSplit && isMerge) stateAction = "split_then_merge";//分裂合并
            else if (isSplit) stateAction = "split";//分裂
            else if (isMerge) stateAction = "merge";//合并
            string sql = string.Format("insert into " + eventStateRelTBName + " values ('{0}','{1}','{2}','{3}','{4}')", oid, stormPolyPrior.stormId, stormPolyPrior.stormId.ToString() + "_" + stormPolyPrior.stateId.ToString(), stormPolyNext.stormId.ToString() + "_" + stormPolyNext.stateId.ToString(), stateAction);
            OracleCommand inserCmd = new OracleCommand(sql, orcConns[tId]);
            inserCmd.ExecuteNonQuery();
        }

        //保存到状态数据库
        int eventStateOid;//事件状态表oid
        private static object objlock2 = new object();
        private void SaveToStateTB(StormPolygon stormPoly, int tId)
        {
            int oid = 0;
            lock (objlock2)
            {
                oid = eventStateOid++;
            }

            //构建oracle spatial类型
            string sdoGtype = string.Empty;
            string sdoElemInfoArray = string.Empty;
            string sdoOrdinateArray = string.Empty;
            stormPoly.GetSdo(out sdoGtype, out sdoElemInfoArray, out sdoOrdinateArray);//获取相关字符串
            string sdoGeometry = string.Format("MDSYS.SDO_GEOMETRY({0},{1},{2},MDSYS.SDO_ELEM_INFO_ARRAY({3}),MDSYS.SDO_ORDINATE_ARRAY({4}))", sdoGtype, sdoSrid, "NULL", sdoElemInfoArray, sdoOrdinateArray);

            string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY:= {4}; begin insert into " + eventStateTBName + " values ('{0}','{1}','{2}',TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS'),v_geo,'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}'); end;", oid, stormPoly.stormId, stormPoly.stormId.ToString() + "_" + stormPoly.stateId.ToString(), stormPoly.startTime.ToString("yyyyMMdd_HHmmss"), sdoGeometry, stormPoly.minLog, stormPoly.minLat, stormPoly.maxLog, stormPoly.maxLat, stormPoly.area, stormPoly.avgRain, stormPoly.volume, stormPoly.maxRain, stormPoly.minRain, stormPoly.power, stormPoly.coreLog, stormPoly.coreLat);
            //string sql = string.Format("insert into " + eventStateTBName + " values ('{0}','{1}','{2}',TO_TIMESTAMP_TZ('{3}','YYYYMMDD_HH24MISS TZH:TZM '),{4},'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", ++eventStateOid, stormId, stormId.ToString() + "_" + stateID.ToString(), startTime + " " + timeZone, sdoGeometry, minLog, minLat, maxLog, maxLat, stormArea, avgRain, volume, maxRain, minRain, stormPower);
            OracleCommand inserCmd = new OracleCommand(sql, orcConns[tId]);
            inserCmd.ExecuteNonQuery();//执行数据库操作
        }

        //遍历一个暴雨所有多边形并编号
        private void OrderStorm(StormPolygon stormPoly, ref Storm storm)
        {
            if (stormPoly.isOrder) return;//已经编号
            stormPoly.isOrder = true;
            stormPoly.stormId = storm.id;
            stormPoly.stateId = storm.maxStateId + 1;
            storm.stateIdList.Add(stormPoly.stateId);//添加状态id
            storm.maxStateId++;
            storm.powerSumArea[stormPoly.power - 1] += stormPoly.area;//统计不同暴雨级别面积
            storm.sumArea += stormPoly.area;//累计面积
            storm.volume += stormPoly.volume;//降雨量体积
            if (stormPoly.avgRain > storm.maxAvgRain)
            {
                storm.maxAvgRain = stormPoly.avgRain;//最大平均降雨量
            }
            if (stormPoly.avgRain < storm.minAvgRain)
            {
                storm.minAvgRain = stormPoly.avgRain;//最小平均降雨量
            }
            if (stormPoly.time > storm.endTime) storm.endTime = stormPoly.time;//结束时间

            if (stormPoly.minLog < storm.minLog) storm.minLog = stormPoly.minLog;//最小经度
            if (stormPoly.minLat < storm.minLat) storm.minLat = stormPoly.minLat;//最小纬度
            if (stormPoly.maxLog > storm.maxLog) storm.maxLog = stormPoly.maxLog;//最大经度
            if (stormPoly.maxLat > storm.maxLat) storm.maxLat = stormPoly.maxLat;//最大纬度
            if (stormPoly.startTime < storm.startTime) storm.startTime = stormPoly.startTime;
            if (stormPoly.endTime > storm.endTime) storm.endTime = stormPoly.endTime;

            //遍历子节点
            foreach (StormPolygon stormPolyNext in stormPoly.childList)
            {
                OrderStorm(stormPolyNext, ref storm);//遍历所有多边形并编号
            }
            //遍历父节点
            foreach (StormPolygon stormPolyPrev in stormPoly.parentList)
            {
                OrderStorm(stormPolyPrev, ref storm);//遍历所有多边形并编号
            }
        }

        private void PolyListConnect(object obj)
        {
            int fileId = (int)obj;
            if (fileId == 6)
            {
                int a = 0;
            }
            List<StormPolygonList> stormPolyLists = stormPolyListsTime[fileId];//取出以当前时间为起始的暴雨链
            foreach (StormPolygonList stormPolyList in stormPolyLists)
            {//当前时间每个暴雨链
                double maxMoveLenTime = maxMoveLen * timeCell;//最大移动距离
                if (fileId > 0)
                {//不是第一个
                    double[] prePos = stormPolyList.prePos;//前时刻预测位置
                    List<StormPolygon> stormPolygonsLast = stormShps[fileId - 1].stormPolyList;//取出上时刻所有暴雨多边形
                    foreach (StormPolygon stormPolygonLast in stormPolygonsLast)
                    {//上一时刻每个多边形
                     //double disKm = TowPosDisKm(stormPolyList.head.coreLog, stormPolyList.head.coreLat, stormPolygonLast.coreLog, stormPolygonLast.coreLat);
                     //if (disKm > maxMoveLenTime) continue; //距离超限
                     //Circle c = new Circle(prePos[0], prePos[1], maxMoveLen * timeCell);
                     //if (!stormPolygonLast.Overlap(c)) continue;
                        if (!stormPolyList.head.Overlap(stormPolygonLast)) continue;
                        double SOD = stormPolyList.head.SOD(stormPolygonLast); //计算重叠度
                        if (SOD >= sodMinValue)
                        {//添加链接关系
                            bool isConnected = false;//默认还没有进行连接
                            foreach (StormPolygon parent in stormPolyList.head.parentList)
                            {
                                if (parent.id == stormPolygonLast.id)
                                {//已经链接
                                    isConnected = true;
                                    break;
                                }
                            }
                            if (!isConnected)
                            {//没有链接
                                stormPolyList.head.parentList.Add(stormPolygonLast);
                                stormPolygonLast.childList.Add(stormPolyList.head);
                            }
                        }
                    }
                }
                if (fileId < (mFileNum - 1) && (stormPolyList.tail.fileId < mFileNum - 1))
                {//不是最后一个且尾节点不是最后一个
                    double[] nextPos = stormPolyList.nextPos;//后时刻预测位置
                    List<StormPolygon> stormPolysNext = stormShps[stormPolyList.tail.fileId + 1].stormPolyList;//取出尾节点后时刻所有暴雨多边形
                    foreach (StormPolygon stormPolyNext in stormPolysNext)
                    {
                        //double disKm = TowPosDisKm(stormPolyList.tail.coreLog, stormPolyList.tail.coreLat, stormPolyNext.coreLog, stormPolyNext.coreLat);
                        //if (disKm > maxMoveLenTime) continue; //距离超限
                        //Circle c = new Circle(nextPos[0], nextPos[1], maxMoveLen * timeCell);
                        //if (!stormPolyNext.Overlap(c)) continue;
                        if (!stormPolyList.tail.Overlap(stormPolyNext)) continue;
                        double SOD = stormPolyList.tail.SOD(stormPolyNext); //计算重叠度
                        if (SOD >= sodMinValue)
                        {//添加链接关系
                            bool isConnected = false;//默认还没有进行连接
                            foreach (StormPolygon child in stormPolyList.tail.childList)
                            {
                                if (child.id == stormPolyNext.id)
                                {
                                    isConnected = true;
                                    break;
                                }
                            }
                            if (!isConnected)
                            {//没有链接
                                stormPolyList.tail.childList.Add(stormPolyNext);
                                stormPolyNext.parentList.Add(stormPolyList.tail);
                            }
                        }
                    }
                }
            }
        }

        private void PolyToList(object obj)
        {
            int index = (int)obj;

            StormShp stormShp = stormShps[index];//当前shp
            List<StormPolygon> stormPolyList = stormShp.stormPolyList;//当前shp所有多边形
            List<StormPolygonList> stormPolygonLists = new List<StormPolygonList>();//保存以当前时间为起始时间的暴雨链
            foreach (StormPolygon stormPolygon in stormPolyList)
            {
                if (stormPolygon.parentList.Count() == 0)
                {//没有父，即为头
                    StormPolygonList stormPolygonList = new StormPolygonList(stormPolygon);
                    stormPolygonList.GetList();//获取头的子，包括子的子
                    //stormPolygonList.UnityDirect();//角度统一
                    stormPolygonList.CalculatePrePos(timeCell);//计算头节点上一点预测状态
                    stormPolygonList.CalculateNextPos(timeCell);//计算尾节点下一点预测状态
                    stormPolygonList.CalculatPreRec(timeCell);//预测外包矩形
                    stormPolygonList.CalculatNextRec(timeCell);//预测外包矩形
                    stormPolygonLists.Add(stormPolygonList);//记录
                }
            }
            stormPolyListsTime[index] = stormPolygonLists;//保存当前时间的所有暴雨链 
        }

        //多边形链接
        private void PolyConnect(object obj)
        {
            int index = (int)obj;
            if (index == 162)
            {
                int a = 0;
            }
            //匈牙利算法
            int maxValue = 99999;//匈牙利算法中的无穷大
            StormShp stormShpPre = stormShps[index];//前shp
            StormShp stormShpNext = stormShps[index + 1];//后shp
            List<StormPolygon> stormPolyListPre = stormShpPre.stormPolyList;//前shp所有多边形
            List<StormPolygon> stormPolyListNext = stormShpNext.stormPolyList;//后shp所有多边形

            //矩阵行列数（相等）
            int maxLength = stormPolyListPre.Count;
            if (maxLength < stormPolyListNext.Count) maxLength = stormPolyListNext.Count;

            YMatrix ym = new YMatrix(maxLength, maxLength);
            ym.SetAllValue(maxValue);//设置初始值
            for (int _r = 0; _r < stormPolyListPre.Count; _r++)
            {//每个前多边形
                for (int _c = 0; _c < stormPolyListNext.Count; _c++)
                {//每个后多边形
                    int distance = GetDistance(stormPolyListPre[_r], stormPolyListNext[_c]);//计算两个多边形的属性距离
                    ym.SetValue(_r, _c, distance);//修改矩阵的值
                }
            }
            ym.Calculation();
            List<Point> resultPoint = ym.GetResult();//结果 对应关系
            for (int i = 0; i < resultPoint.Count; i++)
            {//遍历每一个对应关系
                int prePos = resultPoint[i].X;//前对象id
                int nextpos = resultPoint[i].Y;//后对象id
                if (nextpos < stormPolyListNext.Count && prePos < stormPolyListPre.Count && ym.GetOriValue(prePos, nextpos) < maxValue)
                {//不是添加的行列
                    stormPolyListPre[prePos].childList.Add(stormPolyListNext[nextpos]);
                    stormPolyListNext[nextpos].parentList.Add(stormPolyListPre[prePos]);
                    stormPolyListNext[nextpos].CalculateState(timeCell);//重新计算速度和方向
                }
            }
        }

        //读取shp
        private void ReadShp(object obj)
        {
            int fileId = (int)obj;
            string filePath = shpFileNames[fileId].ToString();
            string startTime = Path.GetFileName(filePath).Substring(0, 8) + "_" + Path.GetFileName(filePath).Substring(10, 6);
            string endTime = Path.GetFileName(filePath).Substring(0, 8) + "_" + Path.GetFileName(filePath).Substring(18, 6);
            //Shp shp = new Shp(filePath);//读取shp
            StormShp stormShp = new StormShp(filePath);//读取shp
            stormShp.fileId = fileId;//记录文件id，方便进行时间上的查询
                                     //字符串转时间
            stormShp.startTime = DateTime.ParseExact(startTime, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.CurrentCulture);//起始时间
            stormShp.endTime = DateTime.ParseExact(endTime, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.CurrentCulture);//结束时间
            foreach (Feature feature in stormShp.featureList)
            {
                int featureId = feature.GetFieldAsInteger("ID");
                //int stormId = feature.GetFieldAsInteger("StormID");
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
                double length = feature.GetFieldAsDouble("Length");
                double coreLog = feature.GetFieldAsDouble("CoreLog");//质心经度
                double coreLat = feature.GetFieldAsDouble("CoreLat");//质心纬度
                double shapeIndex = feature.GetFieldAsDouble("SI");//形状系数
                double lengthMax = feature.GetFieldAsDouble("LMax");//最大长度
                double widthMax = feature.GetFieldAsDouble("WMax");//最大宽度
                double eRatio = feature.GetFieldAsDouble("ERatio");//偏心率 最大宽度/最大长度
                double recDeg = feature.GetFieldAsDouble("RecDeg");//矩形度
                double sphDeg = feature.GetFieldAsDouble("SphDeg");//圆形度
                double recP1X = feature.GetFieldAsDouble("RecP1X");//外包矩形点1
                double recP1Y = feature.GetFieldAsDouble("RecP1Y");//外包矩形点1
                double recP2X = feature.GetFieldAsDouble("RecP2X");//外包矩形点2
                double recP2Y = feature.GetFieldAsDouble("RecP2Y");//外包矩形点2
                double recP3X = feature.GetFieldAsDouble("RecP3X");//外包矩形点3
                double recP3Y = feature.GetFieldAsDouble("RecP3Y");//外包矩形点3
                double recP4X = feature.GetFieldAsDouble("RecP4X");//外包矩形点1
                double recP4Y = feature.GetFieldAsDouble("RecP4Y");//外包矩形点1


                //构建暴雨多边形
                StormPolygon stormPoly = new StormPolygon(polygonWkt);
                stormPoly.id = featureId;//多边形唯一id
                stormPoly.fileId = fileId;//所属图层的fileId 方便进行时间维度的查询
                stormPoly.startTime = stormShp.startTime;//记录时间
                stormPoly.endTime = stormShp.endTime;//记录时间
                stormShp.oLayer.SetFeature(feature);
                stormPoly.power = stormPower;
                stormPoly.area = stormArea;
                stormPoly.volume = volume;//立方米
                stormPoly.avgRain = avgRain;
                stormPoly.maxRain = maxRain;
                stormPoly.minRain = minRain;
                stormPoly.minLog = minLog;
                stormPoly.minLat = minLat;
                stormPoly.maxLog = maxLog;
                stormPoly.maxLat = maxLat;
                stormPoly.area = stormArea;
                stormPoly.length = length;
                stormPoly.coreLog = coreLog;
                stormPoly.coreLat = coreLat;
                stormPoly.shapeIndex = shapeIndex;
                stormPoly.lengthMax = lengthMax;
                stormPoly.widthMax = widthMax;
                stormPoly.eRatio = eRatio;
                stormPoly.recDeg = recDeg;
                stormPoly.sphDeg = sphDeg;
                //stormPoly.minAreaRecP1[0] = Convert.ToDouble(MinRecP1.Split(' ')[0]);
                //stormPoly.minAreaRecP1[1] = Convert.ToDouble(MinRecP1.Split(' ')[1]);
                //stormPoly.minAreaRecP2[0] = Convert.ToDouble(MinRecP2.Split(' ')[0]);
                //stormPoly.minAreaRecP2[1] = Convert.ToDouble(MinRecP2.Split(' ')[1]);
                //stormPoly.minAreaRecP3[0] = Convert.ToDouble(MinRecP3.Split(' ')[0]);
                //stormPoly.minAreaRecP3[1] = Convert.ToDouble(MinRecP3.Split(' ')[1]);
                //stormPoly.minAreaRecP4[0] = Convert.ToDouble(MinRecP4.Split(' ')[0]);
                //stormPoly.minAreaRecP4[1] = Convert.ToDouble(MinRecP4.Split(' ')[1]);
                stormPoly.minAreaRec = new double[5, 2] { { recP1X, recP1Y }, { recP2X, recP2Y }, { recP3X, recP3Y }, { recP4X, recP4Y }, { recP1X, recP1Y } };
                stormShp.stormPolyList.Add(stormPoly);//添加暴雨簇
            }
            stormShps[fileId] = stormShp;//记录图层
        }

        //计算地球两点距离
        double TowPosDisKm(double p1Log, double p1Lat, double p2Log, double p2Lat)
        {
            double logDis = p1Log - p2Log;//经度差距
            double latDis = p1Lat - p2Lat;//纬度差距
            double latMean = 0.5 * (p1Lat + p2Lat);//纬度平均值
            double logDisKm = logDis * Earth.OneLogLen(latMean) / 1000.0;
            double latDisKM = latDis * Earth.OneLatLen() / 1000.0;
            double disKm = Math.Sqrt(Math.Pow(logDisKm, 2) + Math.Pow(latDisKM, 2));
            return disKm;
        }

        /// <summary>
        /// 计算两个暴雨多边形的差距
        /// </summary>
        /// <param name="stormPolygon1"></param>
        /// <param name="stormPolygon2"></param>
        /// <returns></returns>
        private int GetDistance(StormPolygon stormPolygon1, StormPolygon stormPolygon2)
        {
            double[] prePos = stormPolygon1.GetPrePos(timeCell);
            double logDis = stormPolygon1.coreLog - stormPolygon2.coreLog;//经度差
            double latDis = stormPolygon1.coreLat - stormPolygon2.coreLat;//纬度差
            double logDisKm = 111.319 * logDis * Math.Cos(((stormPolygon1.coreLat + stormPolygon2.coreLat) / 2.0) * Math.PI / 180.0);//计算水平距离
            double latDisKm = 111.319 * latDis;//计算竖直距离
            double len = Math.Sqrt(logDisKm * logDisKm + latDisKm * latDisKm);//两点距离，单位km
            if (len > maxMoveLength)
            {
                return maxMatrixValue;
            }
            //Circle c = new Circle(prePos[0], prePos[1], maxMoveLen * timeCell);
            else //if (stormPolygon2.Overlap(c))
            {
                double volumeDis =Math.Abs(stormPolygon1.volume - stormPolygon2.volume);
                double distabce = 0.5 * len + 0.5 * (Math.Pow(volumeDis, 1.0 / 3.0)/1000.0);
                int distabceInt = (int)(distabce * 100.0);//乘100取整
                return distabceInt;
            }
            //else
            //{
            //    return maxMatrixValue;
            //}
        }

        string test;
        int count = 0;
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
                //string sql = "delete from titan_event_state_relation";
                //OracleCommand inserCmd = new OracleCommand(sql, conn);
                //inserCmd.ExecuteNonQuery();//执行数据库操作
                //sql = "delete from titan_event_state";
                //inserCmd = new OracleCommand(sql, conn);
                //inserCmd.ExecuteNonQuery();//执行数据库操作
                //sql = "delete from titan_event";
                //inserCmd = new OracleCommand(sql, conn);
                //inserCmd.ExecuteNonQuery();//执行数据库操作

                progressBar1.Value = 100;
                TimeSpan costTime = DateTime.Now - pStartTime;//用时
                //if (count < 101)
                //{
                //    count++;
                //    worker.RunWorkerAsync();
                //    test += count.ToString() + ":" + (DateTime.Now - pStartTime).TotalSeconds.ToString() + "\n";
                //}
                //else
                //{
                //    count = 0;
                    MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}

            }

            tipLabel.Text = "";
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

        ListBox.ObjectCollection shpFileNames;//所有文件名
        int mFileNum = 0;
        //string eventStateTBName = string.Empty;//事件状态表
        //string eventStateRelTBName = string.Empty;//事件状态关系表
        //string eventTBName = string.Empty;//事件表
        //string timeZone = "";
        double oneHourRainFloor = 16.0;//一小时降雨量下限
        double oneDayRainFloor = 50.0;//一天降雨量下限
        double timeCell = 0.5;//时间间隔
        DateTime pStartTime;//处理开始时间
        //string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID
        int threadCount = 1;
        double maxMoveLength = 30;
        int maxMatrixValue = 99999;
        double sodMinValue = 0.6;//重叠度最小值
        double maxMoveLen = 60.0;//最大移动距离
        string eventStateTBName = string.Empty;//事件状态表
        string eventStateRelTBName = string.Empty;//事件状态关系表
        string eventTBName = string.Empty;//事件表
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
            //eventStateTBName = tableNameComboBox.Text.Trim();
            //eventStateRelTBName = eventStateRelationComboBox.Text.Trim();
            //eventTBName = eventComboBox.Text.Trim();
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

            eventStateTBName = tableNameComboBox.Text.Trim();
            eventStateRelTBName = eventStateRelationComboBox.Text.Trim();
            eventTBName = eventComboBox.Text.Trim();

            pStartTime = DateTime.Now;//记录处理开始时间
            progressBar1.Show();
            worker.RunWorkerAsync();
        }
    }
}
