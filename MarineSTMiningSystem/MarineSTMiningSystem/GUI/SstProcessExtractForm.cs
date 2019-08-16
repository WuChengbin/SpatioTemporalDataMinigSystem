//using ESRI.ArcGIS.Geometry;
using Oracle.ManagedDataAccess.Client;
using OSGeo.OGR;
using OSGeo.GDAL;
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
using MarineSTMiningSystem.Class;

//using GisSharpBlog.NetTopologySuite.IO;

namespace MarineSTMiningSystem.GUI
{
    public partial class SstProcessExtractForm : Form
    {
        string[] shpFileNames;
        string[] shpFileNames_positive;
        string[] shpFileNames_negative;
        //string[] TimeList;//存储所有文件时间
        Layer EventStateLayer;//输出状态图层
        Layer EventLayer;//输出事件图层
        Layer EventSequenceLayer;//输出事件-序列图层
        int threadCount = 1;
        int mFileNum = 0;
        string SavePath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        Thread[] threads;//线程
        string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID
        int timeCell = 30;//1月
        int minDurTime = 5;
        int maxMoveLen = 40;//最大移动距离，每天,km
        //int maxMoveLen = 80;
        double sodMinValue = 0.6;
        int maxMatrixValue = 99999;
        int eventid = 1;
        //int LastPosiID = 0;
        bool checkPosi = false;
        string isAbnormal;//区别正异常

        public SstProcessExtractForm()
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
            
        }

        MarineHeatwaveShp[] mhShps; //记录暴雨图层
        List<MarineHeatwavePolygonList>[] stormPolyListsTime;//用来储存所有时间暴雨链
        List<MarineHeatwave> stormList = new List<MarineHeatwave>();//记录暴雨最开始节点
        //int eventOid;//关系表oid
        //保存关系
        int eventStateRelOid;//关系表oid
        //保存到状态数据库
        //int eventStateOid;//事件状态表oid
        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            for (int ip = 0; ip < 2; ip++)
            {
                if (ip == 0)//正异常
                {
                    shpFileNames = shpFileNames_positive;
                    isAbnormal = "positive";
                }
                else//负异常
                {
                    shpFileNames = shpFileNames_negative;
                    isAbnormal = "negative";
                    eventid++;
                    statecount = 0;

                    EventLayer.Dispose();
                    ds_Event.Dispose();
                    string tempName = EventLayerName + EventLayerNum.ToString();
                    CreateEventLayer(tempName);
                    EventLayerNum++;

                    EventStateLayer.Dispose();
                    ds_EventState.Dispose();
                    string temp = EventStateLayerName + EventStateLayerNum.ToString();
                    CreateEventStateLayer(temp);
                    EventStateLayerNum++;

                    //EventSequenceLayer.Dispose();
                    //ds_EventSequence.Dispose();
                    string temp1 = EventSequenceLayerName + EventSequenceLayerNum.ToString();
                    CreateEventSequenceLayer(temp1);
                    EventSequenceLayerNum++;
                    checkPosi = true;
                }
                mhShps = new MarineHeatwaveShp[mFileNum];//清空
                stormPolyListsTime = new List<MarineHeatwavePolygonList>[mFileNum];
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
                        if (tId >= 1) tId = 0;
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
                GC.Collect();
                for (int index = 0; index < mhShps.Length - 1; index++)
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
                        if (tId >= 1) tId = 0;
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
                GC.Collect();
                //生成暴雨链
                for (int index = 0; index < mhShps.Length; index++)
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
                        if (tId >= 1) tId = 0;
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
                GC.Collect();
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
                        if (tId >= 1) tId = 0;
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
                GC.Collect();
                //遍历编号
                int stormId = eventid;//暴雨id从1开始
                for (int index = 0; index < mhShps.Length; index++)
                {//每个图层
                    if (worker.CancellationPending)
                    {//取消
                        e.Cancel = true;
                        return;
                    }
                    int progress = (index * 100) / mFileNum;//进度
                    worker.ReportProgress(progress);//记录进度
                    MarineHeatwaveShp stormShp = mhShps[index];//shp
                    List<MarineHeatwavePolygon> stormPolyList = stormShp.stormPolyList;//所有多边形
                    foreach (MarineHeatwavePolygon stormPoly in stormPolyList)
                    {//每个暴雨多边形
                        if (stormPoly.isOrder) continue;//已经编过号了
                        MarineHeatwave storm = new MarineHeatwave();
                        storm.startTime = stormPoly.startTime;//记录开始时间
                        storm.endTime = stormPoly.endTime;//结束时间
                        storm.id = stormId;
                        if (ip == 0)
                        {
                            eventid = stormId;
                        }
                        storm.maxStateId = 0;
                        storm.headPoly = stormPoly;
                        OrderStorm(stormPoly, ref storm);//遍历所有多边形并计算属性
                        stormList.Add(storm);//记录新暴雨
                        stormId++;
                    }
                }
                GC.Collect();
                //for (int i=0;i< stormPolyListsTime.Length;i++)
                //{
                //    for (int j=0;j< stormPolyListsTime[i].Count;j++)
                //    {
                //        string str = string.Empty;
                //        str = stormPolyListsTime[i][j].head.eventId.ToString();
                //        Console.WriteLine(str);
                //    }
                //}
                List<List<Sequence>> SequenceList = new List<List<Sequence>>();
                SequenceList = OrgnizeSequence(stormPolyListsTime);
                SaveSequence(SequenceList);
                ds_EventSequence.Dispose();
                EventSequenceLayer.Dispose();
                //数据库存储
                //eventOid = 785;
                //eventStateOid = 1;//事件状态表oid
                eventStateRelOid = 1;//关系表oid
                //int eventIndex = 1;
                for (int index = 0; index < stormList.Count - 1; index++)
                {//每个图层
                    if (worker.CancellationPending)
                    {//取消
                        e.Cancel = true;
                        return;
                    }
                    int progress = (index * 100) / stormList.Count;//进度
                    worker.ReportProgress(progress);//记录进度
                    MarineHeatwave storm = stormList[index];//取出暴雨
                    storm.duration = storm.endTime - storm.startTime;
                    storm.IMean = storm.volume / storm.sumArea;
                    //storm.maxIMean = storm.maxIMean;
                    //storm.minIMean = storm.minIMean;
                    int tempDaysNum = (int)storm.duration.TotalDays;
                    int MonthNum = tempDaysNum / 366 * 12 + tempDaysNum % 366 / 31 + 1;
                    if (MonthNum < minDurTime)
                    {
                        continue;
                    }
                    //if (storm.duration.TotalDays <= 32) continue;//单独暴雨点剔除
                    //if (storm.maxAvgRain < oneHourRainFloor && (storm.maxAvgRain * storm.duration.TotalHours) < oneDayRainFloor)
                    //{//一小时最大降雨量不满足且24小时不满足
                    //    continue;
                    //}
                    int tId = 0;//线程id
                    while (threads[tId] != null && threads[tId].IsAlive == true)
                    {//线程在执行
                        tId++;
                        if (tId >= threads.Length) tId = 0;
                    }
                    threads[tId] = new Thread(new ParameterizedThreadStart(StormToShpAndCSV));
                    threads[tId].IsBackground = true;
                    storm.threadId = tId;
                    storm.oid = index + 1;
                    //storm.oid = eventIndex;
                    //eventIndex++;
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
                    GC.Collect();
                }
            }
        }

        private static object objlock3 = new object();
        //读取shp
        private void ReadShp(object obj)
        {
            int fileId = (int)obj;
            string filePath = shpFileNames[fileId].ToString();
            //string endTime = Path.GetFileName(filePath).Substring(0, 8) + "_" + Path.GetFileName(filePath).Substring(18, 6);
            //Shp shp = new Shp(filePath);//读取shp
            MarineHeatwaveShp mhShp = new MarineHeatwaveShp(filePath);//读取shp
            mhShp.fileId = fileId;//记录文件id，方便进行时间上的查询
            //字符串转时间
            string startTime = System.IO.Path.GetFileName(filePath).Substring(0, 6);
            mhShp.startTime = DateTime.ParseExact(startTime, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);//起始时间
            //TimeList[fileId] = mhShp.startTime.ToString();

            //mhShp.endTime = mhShp.startTime.AddDays(1);//结束时间
            mhShp.endTime = mhShp.startTime.AddMonths(1);//结束时间
            foreach (Feature feature in mhShp.featureList)
            {
                int featureId = feature.GetFieldAsInteger("ID");

                string polygonWkt = string.Empty;//wkt坐标
                feature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);
                double minLog = feature.GetFieldAsDouble("MinLog");
                double minLat = feature.GetFieldAsDouble("MinLat");
                double maxLog = feature.GetFieldAsDouble("MaxLog");
                double maxLat = feature.GetFieldAsDouble("MaxLat");
                double area = feature.GetFieldAsDouble("Area");
                double iMean = feature.GetFieldAsDouble("IMean");
                double iMax = feature.GetFieldAsDouble("IMax");
                double iMin = feature.GetFieldAsDouble("IMin");
                double uSpeed = feature.GetFieldAsDouble("USpeed");
                double vSpeed = feature.GetFieldAsDouble("VSpeed");
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
                int power = feature.GetFieldAsInteger("Power");

                //构建暴雨多边形
                MarineHeatwavePolygon mhPoly = new MarineHeatwavePolygon(polygonWkt);
                mhPoly.id = featureId;//多边形唯一id
                mhPoly.fileId = fileId;//所属图层的fileId 方便进行时间维度的查询
                mhPoly.startTime = mhShp.startTime;//记录时间
                mhPoly.endTime = mhShp.endTime;//记录时间
                mhShp.oLayer.SetFeature(feature);
                mhPoly.area = area;
                mhPoly.iMean = iMean;
                mhPoly.iMax = iMax;
                mhPoly.iMin = iMin;
                mhPoly.uSpeed = uSpeed;
                mhPoly.vSpeed = vSpeed;
                mhPoly.minLog = minLog;
                mhPoly.minLat = minLat;
                mhPoly.maxLog = maxLog;
                mhPoly.maxLat = maxLat;
                mhPoly.length = length;
                mhPoly.coreLog = coreLog;
                mhPoly.coreLat = coreLat;
                mhPoly.shapeIndex = shapeIndex;
                mhPoly.lengthMax = lengthMax;
                mhPoly.widthMax = widthMax;
                mhPoly.eRatio = eRatio;
                mhPoly.recDeg = recDeg;
                mhPoly.sphDeg = sphDeg;
                mhPoly.power = power;
                mhPoly.minAreaRec = new double[5, 2] { { recP1X, recP1Y }, { recP2X, recP2Y }, { recP3X, recP3Y }, { recP4X, recP4Y }, { recP1X, recP1Y } };
                mhShp.stormPolyList.Add(mhPoly);//添加暴雨簇
            }
            lock (objlock3)
            {
                mhShps[fileId] = mhShp;//记录图层
            }
        }

        private static object objlock4 = new object();
        //多边形链接
        private void PolyConnect(object obj)
        {
            int index = (int)obj;
            if (index == 3476)
            {
                int a = 0;
            }
            //匈牙利算法
            int maxValue = 99999;//匈牙利算法中的无穷大
            MarineHeatwaveShp stormShpPre = mhShps[index];//前shp
            MarineHeatwaveShp stormShpNext = mhShps[index + 1];//后shp
            List<MarineHeatwavePolygon> stormPolyListPre = stormShpPre.stormPolyList;//前shp所有多边形
            List<MarineHeatwavePolygon> stormPolyListNext = stormShpNext.stormPolyList;//后shp所有多边形

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
            List<System.Drawing.Point> resultPoint = ym.GetResult();//结果 对应关系
            for (int i = 0; i < resultPoint.Count; i++)
            {//遍历每一个对应关系
                int prePos = resultPoint[i].X;//前对象id
                int nextpos = resultPoint[i].Y;//后对象id
                if (nextpos < stormPolyListNext.Count && prePos < stormPolyListPre.Count && ym.GetOriValue(prePos, nextpos) < maxValue)
                {//不是添加的行列
                    lock (objlock4)
                    {
                        stormPolyListPre[prePos].childList.Add(stormPolyListNext[nextpos]);
                        stormPolyListNext[nextpos].parentList.Add(stormPolyListPre[prePos]);
                        //stormPolyListNext[nextpos].CalculateState(timeCell);//重新计算速度和方向
                    }
                }
            }
            ////
        }

        /// <summary>
        /// 计算两个暴雨多边形的差距
        /// </summary>
        /// <param name="stormPolygon1"></param>
        /// <param name="stormPolygon2"></param>
        /// <returns></returns>
        private int GetDistance(MarineHeatwavePolygon stormPolygon1, MarineHeatwavePolygon stormPolygon2)
        {
            double[] prePos = stormPolygon1.GetPrePos(timeCell);
            double logDis = prePos[0] - stormPolygon2.coreLog;//经度差
            double latDis = prePos[1] - stormPolygon2.coreLat;//纬度差
            double logDisKm = 111.319 * logDis * Math.Cos(((prePos[1] + stormPolygon2.coreLat) / 2.0) * Math.PI / 180.0);//计算水平距离
            double latDisKm = 111.319 * latDis;//计算竖直距离
            double len = Math.Sqrt(logDisKm * logDisKm + latDisKm * latDisKm);//两点距离，单位km
            if (len > maxMoveLen * timeCell)
            {
                return maxMatrixValue;
            }
            //Circle c = new Circle(prePos[0], prePos[1], maxMoveLen * timeCell);
            else //(stormPolygon2.Overlap(c))
            {
                double eRatioDis = Math.Abs(stormPolygon1.eRatio - stormPolygon2.eRatio);//偏心率差
                double recDegDis = Math.Abs(stormPolygon1.recDeg - stormPolygon2.recDeg);//矩形度差
                double sphDegDis = Math.Abs(stormPolygon1.sphDeg - stormPolygon2.sphDeg);//圆形度差
                double shapeIndexDis = Math.Abs(stormPolygon1.shapeIndex - stormPolygon2.shapeIndex);//形状系数差
                double distabce = 0.25 * eRatioDis + 0.25 * recDegDis + 0.25 * sphDegDis + 0.25 * shapeIndexDis;
                int distabceInt = (int)(distabce * 100.0);//乘100取整
                return distabceInt;
            }
            //else
            //{
            //    return maxMatrixValue;
            //}
        }

        private static object objlock5 = new object();

        private void PolyToList(object obj)
        {
            int index = (int)obj;
            MarineHeatwaveShp stormShp = mhShps[index];//当前shp
            List<MarineHeatwavePolygon> stormPolyList = stormShp.stormPolyList;//当前shp所有多边形
            List<MarineHeatwavePolygonList> stormPolygonLists = new List<MarineHeatwavePolygonList>();//保存以当前时间为起始时间的暴雨链
            foreach (MarineHeatwavePolygon stormPolygon in stormPolyList)
            {
                if (stormPolygon.parentList.Count() == 0)
                {//没有父，即为头
                    MarineHeatwavePolygonList stormPolygonList = new MarineHeatwavePolygonList(stormPolygon);
                    stormPolygonList.GetList();//获取头的子，包括子的子
                    if (stormPolygonList.mhPolygons.Count == 1)
                    {//只有一个点
                        stormPolygonList.mhPolygons[0].isAcnode = true;
                    }
                    stormPolygonList.CalculatePrePos(timeCell);//计算头节点上一点预测状态
                    stormPolygonList.CalculateNextPos(timeCell);//计算尾节点下一点预测状态
                    stormPolygonList.CalculatPreRec(timeCell);//预测外包矩形
                    stormPolygonList.CalculatNextRec(timeCell);//预测外包矩形
                    stormPolygonLists.Add(stormPolygonList);//记录
                }
            }
            lock (objlock5)
            {
                stormPolyListsTime[index] = stormPolygonLists;//保存当前时间的所有暴雨链 
            }
        }

        private static object objlock7 = new object();
        private void PolyListConnect(object obj)
        {
            int fileId = (int)obj;
            List<MarineHeatwavePolygonList> stormPolyLists = stormPolyListsTime[fileId];//取出以当前时间为起始的暴雨链
            foreach (MarineHeatwavePolygonList stormPolyList in stormPolyLists)
            {//当前时间每个暴雨链
                double maxMoveLenTime = maxMoveLen * timeCell;//最大移动距离
                if (fileId > 0)
                {//不是第一个
                    //double[] prePos = stormPolyList.prePos;//前时刻预测位置
                    List<MarineHeatwavePolygon> stormPolygonsLast = mhShps[fileId - 1].stormPolyList;//取出上时刻所有暴雨多边形
                    foreach (MarineHeatwavePolygon stormPolygonLast in stormPolygonsLast)
                    {//上一时刻每个多边形
                        //double disKm = TowPosDisKm(prePos[0], prePos[1], stormPolygonLast.coreLog, stormPolygonLast.coreLat);
                        //if (disKm > maxMoveLenTime) continue; //距离超限
                        //if (!stormPolyList.head.Overlap(stormPolygonLast)) continue;
                        //if (Math.Abs(stormPolygonLast.avgRain - stormPolyList.head.avgRain) > 20.0) continue;//降雨量差距过大
                        if (!stormPolygonLast.Overlap(stormPolyList.preMinAreaRec)) continue;
                        //double SOD = stormPolyList.head.SOD(stormPolygonLast); //计算重叠度
                        double preRecArea = stormPolyList.preRecArea;
                        double SOD = stormPolygonLast.SOD(stormPolyList.preMinAreaRec, preRecArea); //计算重叠度
                        if (SOD >= sodMinValue)
                        {//添加链接关系
                            bool isConnected = false;//默认还没有进行连接
                            foreach (MarineHeatwavePolygon parent in stormPolyList.head.parentList)
                            {
                                if (parent.id == stormPolygonLast.id)
                                {//已经链接
                                    isConnected = true;
                                    break;
                                }
                            }
                            if (!isConnected)
                            {//没有链接
                                lock (objlock7)
                                {
                                    stormPolyList.head.parentList.Add(stormPolygonLast);
                                    stormPolygonLast.childList.Add(stormPolyList.head);
                                }
                            }
                        }
                    }
                }
                if (fileId < (mFileNum - 1) && (stormPolyList.tail.fileId < mFileNum - 1))
                {//不是最后一个且尾节点不是最后一个
                    //double[] nextPos = stormPolyList.nextPos;//后时刻预测位置
                    List<MarineHeatwavePolygon> stormPolysNext = mhShps[stormPolyList.tail.fileId + 1].stormPolyList;//取出尾节点后时刻所有暴雨多边形
                    foreach (MarineHeatwavePolygon stormPolyNext in stormPolysNext)
                    {
                        //double disKm = TowPosDisKm(nextPos[0], nextPos[1], stormPolyNext.coreLog, stormPolyNext.coreLat);
                        //if (disKm > maxMoveLenTime) continue; //距离超限
                        //Circle c = new Circle(nextPos[0], nextPos[1], maxMoveLen * timeCell);
                        //if (!stormPolyNext.Overlap(c)) continue;
                        //if (Math.Abs(stormPolyNext.avgRain - stormPolyList.tail.avgRain) > 100.0) continue;//降雨量差别过大
                        if (!stormPolyNext.Overlap(stormPolyList.nextMinAreaRec)) continue;
                        //if (!stormPolyList.tail.Overlap(stormPolyNext)) continue;
                        //double SOD = stormPolyList.tail.SOD(stormPolyNext); //计算重叠度
                        double nextRecArea = stormPolyList.nextRecArea;
                        double SOD = stormPolyNext.SOD(stormPolyList.nextMinAreaRec, nextRecArea);
                        if (SOD >= sodMinValue)
                        {//添加链接关系
                            bool isConnected = false;//默认还没有进行连接
                            foreach (MarineHeatwavePolygon child in stormPolyList.tail.childList)
                            {
                                if (child.id == stormPolyNext.id)
                                {
                                    isConnected = true;
                                    break;
                                }
                            }
                            if (!isConnected)
                            {//没有链接
                                lock (objlock7)
                                {
                                    stormPolyList.tail.childList.Add(stormPolyNext);
                                    stormPolyNext.parentList.Add(stormPolyList.tail);
                                }
                            }
                        }
                    }
                }
            }
        }

        //遍历一个暴雨所有多边形并编号
        private void OrderStorm(MarineHeatwavePolygon stormPoly, ref MarineHeatwave storm)
        {
            if (stormPoly.isOrder) return;//已经编号
            stormPoly.isOrder = true;
            stormPoly.eventId = storm.id;
            stormPoly.stateId = storm.maxStateId + 1;
            storm.stateIdList.Add(stormPoly.stateId);//添加状态id
            storm.maxStateId++;
            //storm.powerSumArea[stormPoly.power - 1] += stormPoly.area;//统计不同暴雨级别面积
            storm.sumArea += stormPoly.area;//累计面积
            if (stormPoly.area >= storm.NodeMaxArea)
            {
                storm.NodeMaxArea = stormPoly.area;
            }
            storm.volume += stormPoly.iMean * stormPoly.area;//降雨量体积
            if (stormPoly.iMean > storm.maxIMean)
            {
                storm.maxIMean = stormPoly.iMean;//最大平均降雨量
            }
            if (stormPoly.iMean < storm.minIMean)
            {
                storm.minIMean = stormPoly.iMean;//最小平均降雨量
            }
            if (stormPoly.time > storm.endTime) storm.endTime = stormPoly.time;//结束时间

            if (stormPoly.minLog < storm.minLog) storm.minLog = stormPoly.minLog;//最小经度
            if (stormPoly.minLat < storm.minLat) storm.minLat = stormPoly.minLat;//最小纬度
            if (stormPoly.maxLog > storm.maxLog) storm.maxLog = stormPoly.maxLog;//最大经度
            if (stormPoly.maxLat > storm.maxLat) storm.maxLat = stormPoly.maxLat;//最大纬度
            if (stormPoly.startTime < storm.startTime) storm.startTime = stormPoly.startTime;
            if (stormPoly.endTime > storm.endTime) storm.endTime = stormPoly.endTime;

            //遍历子节点
            foreach (MarineHeatwavePolygon stormPolyNext in stormPoly.childList)
            {
                OrderStorm(stormPolyNext, ref storm);//遍历所有多边形并编号
            }
            //遍历父节点
            foreach (MarineHeatwavePolygon stormPolyPrev in stormPoly.parentList)
            {
                OrderStorm(stormPolyPrev, ref storm);//遍历所有多边形并编号
            }
        }

        private void StormToShpAndCSV(object obj)
        {
            MarineHeatwave storm = (MarineHeatwave)obj;//取出暴雨
            MarineHeatwavePolygon stormHead = storm.headPoly;//一个暴雨头
            SaveToShpAndCSV(stormHead, storm.threadId);//保存到数据库
            SaveToEventShp(storm);
        }

        private static object objlock6 = new object();
        int EventLayerNum = 1;
        //暴雨保存到数据库
        private void SaveToEventShp(MarineHeatwave storm)
        {
            //Storm storm = (Storm)o;
            //int oid = 0;
            //lock (objlock6)
            //{
            //    oid = eventOid++;
            //}
            //string sql0 = string.Format("select sdo_geometry.get_wkt(space) from " + eventStateTBName + " where event_id =" + storm.id);
            //DataTable dtResult = QueryResultTable(sql0);
            //Geometry UnionGeo = new Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
            //for (int i=0;i< dtResult.Rows.Count;i++)
            //{
            //    string polygonStr = dtResult.Rows[i][0].ToString();//wkt
            //    //Geometry geoPolygon = Geometry.CreateFromWkt(polygonStr);
            //    UnionGeo = UnionGeo.Union(Ogr.CreateGeometryFromWkt(ref polygonStr, null));
            //}
            string UnionGeometryWkt;//事件多边形
            UnionGeo.ExportToWkt(out UnionGeometryWkt);

            //Console.WriteLine("开始处理oid" + oid);
            //string sql = string.Format("declare v_geo sdo_geometry('" + UnionGeometryWkt + "','" + 2007 + "');begin insert into " + eventTBName + " values ('{0}','{1}',TO_TIMESTAMP('{2}','YYYYMMDD_HH24MISS'),TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS '),'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',(v_geo)); end; ",
            //           oid, storm.id, storm.startTime.ToString("yyyyMMdd_HHmmss"), storm.endTime.ToString("yyyyMMdd_HHmmss"), storm.duration.TotalDays.ToString(),
            //             storm.minLog, storm.minLat, storm.maxLog, storm.maxLat, storm.IMean, 0, storm.maxIMean, storm.minIMean, 0, 0, "");

            // string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY; begin select space into v_geo from " + eventStateTBName + " where event_id='{1}'; insert into " + eventTBName +
            //" values ('{0}','{1}',TO_TIMESTAMP('{2}','YYYYMMDD_HH24MISS'),TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS '),'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',(v_geo)); end;",
            // oid, storm.id, storm.startTime.ToString("yyyyMMdd_HHmmss"), storm.endTime.ToString("yyyyMMdd_HHmmss"), storm.duration.TotalDays.ToString(),
            // storm.minLog, storm.minLat, storm.maxLog, storm.maxLat, storm.IMean, 0, storm.maxIMean, storm.minIMean, 0, 0,"");
            //OracleCommand inserCmd = new OracleCommand(sql, orcConns[storm.threadId]);
            //inserCmd.ExecuteNonQuery();//执行数据库操作
            //Console.WriteLine("结束处理oid" + oid);

            //输出shp
            //if (checkPosi)
            //{
            //    storm.id += eventid;
            //}
            //if (storm.id == 39)
            //{
            //    int a = 0;
            //}
            if (storm.id / 1000 >= EventLayerNum)
            {
                EventLayer.Dispose();
                ds_Event.Dispose();
                string tempName = EventLayerName + EventLayerNum.ToString();
                CreateEventLayer(tempName);
                EventLayerNum++;
            }
            #region 写入数据
            FeatureDefn oDefn = EventLayer.GetLayerDefn();
            Feature oFeature = new Feature(oDefn);
            //oFeature.SetField(0, oid);
            oFeature.SetField(0, storm.id);
            oFeature.SetField(1, storm.startTime.ToString("yyyy-MM-dd HH:mm:ss"));
            oFeature.SetField(2, storm.endTime.ToString("yyyy-MM-dd HH:mm:ss"));
            //将天数转为月数
            int tempDaysNum = (int)storm.duration.TotalDays;
            int MonthNum = tempDaysNum / 366 * 12 + tempDaysNum % 366 / 31 + 1;

            oFeature.SetField(3, MonthNum);

            oFeature.SetField(4, storm.IMean);
            oFeature.SetField(5, storm.minIMean);
            oFeature.SetField(6, storm.maxIMean);
            double minlon = storm.minLog;
            if (storm.minLog > 180.0)
            {
                minlon = storm.minLog - 360;
            }
            oFeature.SetField(7, minlon);
            oFeature.SetField(8, storm.minLat);
            double maxlon = storm.maxLog;
            if (storm.maxLog > 180.0)
            {
                maxlon = storm.maxLog - 360;
            }
            oFeature.SetField(9, maxlon);
            oFeature.SetField(10, storm.maxLat);

            oFeature.SetField(11, 0);
            oFeature.SetField(12, 0);
            oFeature.SetField(13, isAbnormal);
            //oFeature.SetField(14, storm.NodeMaxArea);
            //oFeature.SetField(13, 0);
            //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
            //string polygonStr = result[15].ToString();
            Geometry geoPolygon = Geometry.CreateFromWkt(UnionGeometryWkt);
            oFeature.SetGeometry(geoPolygon);
            EventLayer.CreateFeature(oFeature);
            geoPolygon.Dispose();
            #endregion
            //释放资源
            oDefn.Dispose();
            oFeature.Dispose();
            UnionGeo.Empty();
        }

        //暴雨状态保存到数据库
        private void SaveToShpAndCSV(MarineHeatwavePolygon stormPoly, int tId)
        {
            if (stormPoly.isSaveToDB) return;
            stormPoly.isSaveToDB = true;
            //if (!(stormPoly.isAcnode && (stormPoly.parentList.Count + stormPoly.childList.Count == 1)))
            //{//不只有一个关系个孤立点
            SaveToEventStateShp(stormPoly, tId);//保存到状态数据库

            //}
            //遍历子节点
            foreach (MarineHeatwavePolygon stormPolyNext in stormPoly.childList)
            {
                SaveToShpAndCSV(stormPolyNext, tId);//遍历所有多边形并编号
                //if ((!(stormPoly.isAcnode && (stormPoly.parentList.Count + stormPoly.childList.Count == 1))) && (!(stormPolyNext.isAcnode && (stormPolyNext.parentList.Count + stormPolyNext.childList.Count == 1))))
                //{//不只有一个关系个孤立点
                SaveToRelationCSV(stormPoly, stormPolyNext, tId);//保存到关系表
                //}
            }
            //遍历父节点
            foreach (MarineHeatwavePolygon stormPolyPrev in stormPoly.parentList)
            {
                SaveToShpAndCSV(stormPolyPrev, tId);//遍历所有多边形并编号
                //SaveToRelatTB(stormPolyPrev, stormPoly, tId, ref eventStateRelOid);//保存到关系表
            }
        }

        private static object objlock1 = new object();
        //string strEventStateRelationFile = "C:\\Users\\Lenovo\\Desktop\\event_state_relation_test\\EVENT_STATE_RELATION.csv";
        string FileRelationName = "EVENT_STATE_RELATION";
        int outFileRelationNum = 1;
        private void SaveToRelationCSV(MarineHeatwavePolygon stormPolyPrior, MarineHeatwavePolygon stormPolyNext, int tId)
        {
            int oid = 0;
            lock (objlock1)
            {
                oid = eventStateRelOid++;
            }
            //if (checkPosi)
            //{
            //    stormPolyPrior.eventId += eventid;
            //    stormPolyNext.eventId += eventid;
            //}
            bool isSplit = false;//是否分裂
            int childCount = stormPolyPrior.childList.Count;
            foreach (MarineHeatwavePolygon poly in stormPolyPrior.childList)
            {//噪声
                if ((poly.isAcnode && (poly.parentList.Count + poly.childList.Count == 1))) childCount--;
            }
            if (childCount > 1)
            {//分裂
                isSplit = true;
            }

            bool isMerge = false;//是否合并
            int parentCount = stormPolyNext.parentList.Count;
            foreach (MarineHeatwavePolygon poly in stormPolyNext.parentList)
            {//噪声
                if ((poly.isAcnode && (poly.parentList.Count + poly.childList.Count == 1))) parentCount--;
            }
            if (parentCount > 1)
            {//合并
                isMerge = true;
            }
            string stateAction = "develop";//0：发展 1：分裂 2：合并 3：分裂合并
            if (isSplit && isMerge) stateAction = "split_then_merge";//分裂合并
            else if (isSplit) stateAction = "split";//分裂
            else if (isMerge) stateAction = "merge";//合并
            //string sql = string.Format("insert into " + eventStateRelTBName + " values ('{0}','{1}','{2}','{3}','{4}')", oid, stormPolyPrior.eventId, stormPolyPrior.eventId.ToString() + "_" + stormPolyPrior.stateId.ToString(), stormPolyNext.eventId.ToString() + "_" + stormPolyNext.stateId.ToString(), stateAction);
            //OracleCommand inserCmd = new OracleCommand(sql, orcConns[tId]);
            //inserCmd.ExecuteNonQuery();
            if (oid / 3000 >= outFileRelationNum)//每3000行一个文件
            {
                outFileRelationNum++;
                //strEventStateRelationFile = SavePath + FileRelationName + outFileRelationNum.ToString() + ".csv";
                FileRelationName += outFileRelationNum.ToString();
            }
            string strEventStateRelationFile = SavePath + FileRelationName + ".csv";
            
            WriteCSV_Relation(strEventStateRelationFile,stormPolyPrior.eventId.ToString() + "_" + stormPolyPrior.stateId.ToString(), stormPolyNext.eventId.ToString() + "_" + stormPolyNext.stateId.ToString(), stateAction);
        }

        private static object objlock2 = new object();
        Geometry UnionGeo = new Geometry(OSGeo.OGR.wkbGeometryType.wkbPolygon);
        int EventStateLayerNum = 1;
        int statecount = 0;
        //bool checkdown = false;
        private void SaveToEventStateShp(MarineHeatwavePolygon stormPoly, int tId)
        {
            //int oid = 0;
            //lock (objlock2)
            //{
            //    oid = eventStateOid++;
            //}
            //构建oracle spatial类型
            string sdoGtype = string.Empty;
            string sdoElemInfoArray = string.Empty;
            string sdoOrdinateArray = string.Empty;
            stormPoly.GetSdo(out sdoGtype, out sdoElemInfoArray, out sdoOrdinateArray);//获取相关字符串
            string sdoGeometry = string.Format("MDSYS.SDO_GEOMETRY({0},{1},{2},MDSYS.SDO_ELEM_INFO_ARRAY({3}),MDSYS.SDO_ORDINATE_ARRAY({4}))", sdoGtype, sdoSrid, "NULL", sdoElemInfoArray, sdoOrdinateArray);

            //string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY:= {4}; begin insert into " + eventStateTBName
            //    + " values ('{0}','{1}','{2}',TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS'),v_geo,'{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}'); end;",
            //    oid, stormPoly.eventId, stormPoly.eventId.ToString() + "_" + stormPoly.stateId.ToString(), stormPoly.startTime.ToString("yyyyMMdd_HHmmss"),
            //    sdoGeometry, stormPoly.minLog, stormPoly.minLat, stormPoly.maxLog, stormPoly.maxLat, stormPoly.area, stormPoly.iMean, 0, stormPoly.iMax, stormPoly.iMin, stormPoly.power,
            //    stormPoly.coreLog, stormPoly.coreLat);
            //string sql = string.Format("declare v_geo MDSYS.SDO_GEOMETRY:= {15};begin insert into " + eventStateTBName
            //                  + " values ('{0}','{1}','{2}',TO_TIMESTAMP('{3}','YYYYMMDD_HH24MISS'),'{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}',v_geo); end;",
            //                 oid, stormPoly.eventId, stormPoly.eventId.ToString() + "_" + stormPoly.stateId.ToString(), stormPoly.startTime.ToString("yyyyMMdd_HHmmss"),
            //                 stormPoly.minLog, stormPoly.minLat, stormPoly.maxLog, stormPoly.maxLat, stormPoly.area, stormPoly.iMean, stormPoly.iMax, stormPoly.iMin, stormPoly.power,
            //                 stormPoly.coreLog, stormPoly.coreLat, sdoGeometry);
            //OracleCommand inserCmd = new OracleCommand(sql, orcConns[tId]);
            //inserCmd.ExecuteNonQuery();//执行数据库操作
            statecount++;
            //输出shp
            if (statecount / 1000 >= EventStateLayerNum)
            {
                EventStateLayer.Dispose();
                ds_EventState.Dispose();
                string temp = EventStateLayerName + EventStateLayerNum.ToString();
                CreateEventStateLayer(temp);
                EventStateLayerNum++;
            }
            #region 写入数据
            //if (checkPosi)
            //{
            //    stormPoly.eventId += eventid;
            //}
            FeatureDefn oDefn = EventStateLayer.GetLayerDefn();
            Feature oFeature = new Feature(oDefn);
            //oFeature.SetField("OID", oid);
            oFeature.SetField("PRID", stormPoly.eventId);
            oFeature.SetField("STID", stormPoly.eventId.ToString() + "_" + stormPoly.stateId.ToString());
            oFeature.SetField("SQID", stormPoly.SQID);
            /*
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
            */
            oFeature.SetField("Time", stormPoly.startTime.ToString("yyyy-MM-dd HH:mm:ss"));
            double minlon = stormPoly.minLog;
            if (stormPoly.minLog>180.0)
            {
                minlon = stormPoly.minLog - 360;
            }
            oFeature.SetField("MinLon", minlon);
            oFeature.SetField("MinLat", stormPoly.minLat);
            double maxlon = stormPoly.maxLog;
            if (stormPoly.maxLog > 180.0)
            {
                maxlon = stormPoly.maxLog - 360;
            }
            oFeature.SetField("MaxLon", maxlon);
            oFeature.SetField("MaxLat", stormPoly.maxLat);
            oFeature.SetField("Area", stormPoly.area);
            oFeature.SetField("AvgValue", stormPoly.iMean);
            oFeature.SetField("MaxValue", stormPoly.iMax);
            oFeature.SetField("MinValue", stormPoly.iMin);
            //DateTime time1970 = new DateTime(1970, 1, 1); // 当地时区
            //long timeStamp = (long)(((DateTime)dtResult.Rows[id][3]) - time1970).TotalSeconds; // 相差秒数
            //oFeature.SetField("LongTime", timeStamp.ToString());
            oFeature.SetField("Power", stormPoly.power);
            oFeature.SetField("CoreLon", stormPoly.coreLog);
            oFeature.SetField("CoreLat", stormPoly.coreLat);
            oFeature.SetField("Abnormal", isAbnormal);
            //"POLYGON ((0 0,20 0,10 15,0 0),(5 2,10 10,15 2,5 2))"
            //string polygonStr = dtResult.Rows[id][15].ToString();//MDSYS.SDO_GEOMETRY

            //string polygonStr = sdoGeometry;//MDSYS.SDO_GEOMETRY
            //string polygonStr = sdo_geometry.get_wkt("MDSYS.SDO_GEOMETRY");
            string strwkt = stormPoly.wkt;
            Geometry geoPolygon = Geometry.CreateFromWkt(stormPoly.wkt);

            oFeature.SetGeometry(geoPolygon);
            EventStateLayer.CreateFeature(oFeature);
            
            UnionGeo = UnionGeo.Union(geoPolygon);
            //释放资源
            oDefn.Dispose();
            geoPolygon.Dispose();
            oFeature.Dispose();
            //ds1.Dispose();
            #endregion
        }
        
        #region 按钮Button
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

        private void addFileBtn2_Click(object sender, EventArgs e)
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
                    listBox2.Items.Add(fileName);
                }
                countTextBox2.Text = listBox2.Items.Count.ToString();
            }
        }

        private void deleteFileBtn2_Click(object sender, EventArgs e)
        {
            int index = listBox2.SelectedIndex;
            while (index > -1)
            {
                listBox2.Items.RemoveAt(index);
                index = listBox2.SelectedIndex;
            }
            countTextBox2.Text = listBox2.Items.Count.ToString();
        }

        private void moveUpBtn2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox2.Items.Insert(endIndex + 1, listBox2.Items[startIndex - 1].ToString());
                    listBox2.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void moveDownBtn2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox2.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox2.Items.Insert(startIndex, listBox2.Items[endIndex + 1].ToString());
                    listBox2.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox2.SelectedIndices;
                }
            }
        }

        private void SelectSavepathBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                SavePathComboBox.Text = fbd.SelectedPath;
            }
        }

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

            shpFileNames_positive = listBox1.Items.Cast<string>().ToArray();
            mFileNum = shpFileNames_positive.Length;//文件个数
            shpFileNames_negative = listBox2.Items.Cast<string>().ToArray();
            threadCount = Convert.ToInt32(threadCountTextBox.Text.Trim());
            threads = new Thread[threadCount];//线程数
            timeCell = Convert.ToInt32(timecellBox.Text.Trim());
            minDurTime = Convert.ToInt32(minDurBox.Text.Trim());
            //eventid = Math.Abs(Convert.ToInt32(EventFirstIndexBox.Text.Trim()));
            SavePath = SavePathComboBox.Text.Trim() + "\\";
            //CreateOutLayer();//创建输出图层
            CreateEventLayer(EventLayerName);
            CreateEventStateLayer(EventStateLayerName);
            CreateEventSequenceLayer(EventSequenceLayerName);
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
        #endregion

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (e.Cancelled)
            {//用户取消
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {//异常结束
                MessageBox.Show(e.Error.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //释放资源
                EventLayer.Dispose();
                EventStateLayer.Dispose();
                
                ds_Event.Dispose();
                ds_EventState.Dispose();

                //EventSequenceLayer.Dispose();
                //ds_EventSequence.Dispose();

                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            progressBar1.Value = e.ProgressPercentage;
        }

        #region CreatLayer
        DataSource ds_EventState;
        DataSource ds_Event;
        DataSource ds_EventSequence;
        string EventStateLayerName = "Event_State";
        string EventLayerName = "Event";
        string EventSequenceLayerName = "Event_Sequence";
        private void CreateEventLayer(string LayerName)
        {
            #region 创建输出图层
            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                return;
            }
            ds_Event = oDriver.CreateDataSource(SavePath, null);
            if (ds_Event == null)
            {
                return;
            }
            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            EventLayer = ds_Event.CreateLayer(LayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
            #region 创建属性字段 EventLayer
            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldID1 = new FieldDefn("OID", FieldType.OFTInteger);
            //EventLayer.CreateField(oFieldID1, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID1 = new FieldDefn("PRID", FieldType.OFTInteger);
            EventLayer.CreateField(oFieldStormID1, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime1 = new FieldDefn("STime", FieldType.OFTString);
            oFieldStartTime1.SetWidth(20);
            EventLayer.CreateField(oFieldStartTime1, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldEndTime1 = new FieldDefn("ETime", FieldType.OFTString);
            oFieldEndTime1.SetWidth(20);
            EventLayer.CreateField(oFieldEndTime1, 1);

            //创建x坐标字段
            FieldDefn oFieldDuration = new FieldDefn("DurTime", FieldType.OFTReal);
            oFieldDuration.SetWidth(10);
            oFieldDuration.SetPrecision(8);
            EventLayer.CreateField(oFieldDuration, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall1 = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall1.SetWidth(20);
            oFieldAvgRainFall1.SetPrecision(8);
            EventLayer.CreateField(oFieldAvgRainFall1, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            EventLayer.CreateField(oFieldMinRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall1 = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldMaxRainFall1.SetWidth(20);
            oFieldMaxRainFall1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxRainFall1, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog1 = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog1.SetWidth(10);
            oFieldMinLog1.SetPrecision(8);
            EventLayer.CreateField(oFieldMinLog1, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat1 = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat1.SetWidth(10);
            oFieldMinLat1.SetPrecision(8);
            EventLayer.CreateField(oFieldMinLat1, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog1 = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog1.SetWidth(10);
            oFieldMaxLog1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxLog1, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat1 = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat1.SetWidth(10);
            oFieldMaxLat1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxLat1, 1);

            //创建
            FieldDefn oFieldPower1 = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower1.SetWidth(20);
            oFieldPower1.SetPrecision(8);
            EventLayer.CreateField(oFieldPower1, 1);

            //创建
            //FieldDefn oFieldTotalVolume = new FieldDefn("TotalVolume", FieldType.OFTReal);
            //oFieldTotalVolume.SetWidth(24);
            //oFieldTotalVolume.SetPrecision(8);
            //EventLayer.CreateField(oFieldTotalVolume, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldTrajectoryID = new FieldDefn("TrajectoryID", FieldType.OFTInteger);
            oFieldTrajectoryID.SetWidth(28);
            EventLayer.CreateField(oFieldTrajectoryID, 1);

            FieldDefn oFieldSSTisAbnormal = new FieldDefn("Abnormal", FieldType.OFTString);
            oFieldSSTisAbnormal.SetWidth(20);
            EventLayer.CreateField(oFieldSSTisAbnormal, 1);

            //创建z坐标字段
            //FieldDefn oFieldMaxArea = new FieldDefn("NodeMaxArea", FieldType.OFTReal);
            //oFieldMaxArea.SetWidth(10);
            //oFieldMaxArea.SetPrecision(8);
            //EventLayer.CreateField(oFieldMaxArea, 1);
            #endregion
            #endregion
        }
        private void CreateEventStateLayer(string LayerName)
        {
            #region 创建输出图层
            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                return;
            }
            ds_EventState = oDriver.CreateDataSource(SavePath, null);
            if (ds_EventState == null)
            {
                return;
            }
            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            EventStateLayer = ds_EventState.CreateLayer(LayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
            #region 接下来创建属性表字段 EventStateLayer
            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldID = new FieldDefn("OID", FieldType.OFTInteger);
            //EventStateLayer.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("PRID", FieldType.OFTInteger);
            EventStateLayer.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormStateID = new FieldDefn("STID", FieldType.OFTString);
            oFieldStormStateID.SetWidth(20);
            EventStateLayer.CreateField(oFieldStormStateID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormSQID = new FieldDefn("SQID", FieldType.OFTString);
            oFieldStormSQID.SetWidth(20);
            EventStateLayer.CreateField(oFieldStormSQID, 1);
            //if (outParentsChildren)
            //{
            //    // 先创建一个叫FieldID的整型属性
            //    FieldDefn oFieldParentsID = new FieldDefn("ParentsID", FieldType.OFTString);
            //    oFieldParentsID.SetWidth(50);
            //    olayer1.CreateField(oFieldParentsID, 1);

            //    // 先创建一个叫FieldID的整型属性
            //    FieldDefn oFieldChildrenID = new FieldDefn("ChildrenID", FieldType.OFTString);
            //    oFieldChildrenID.SetWidth(50);
            //    olayer1.CreateField(oFieldChildrenID, 1);
            //}

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldStartTime.SetWidth(20);
            EventStateLayer.CreateField(oFieldStartTime, 1);

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
            EventStateLayer.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxLat, 1);

            //创建平均降雨量字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            EventStateLayer.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            EventStateLayer.CreateField(oFieldAvgRainFall, 1);

            //创建体积字段
            FieldDefn oFieldVolume = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldVolume.SetWidth(20);
            oFieldVolume.SetPrecision(8);
            EventStateLayer.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxRainFall, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            //FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            //oFieldLongTime.SetWidth(20);
            //olayer1.CreateField(oFieldLongTime, 1);

            //创建
            FieldDefn oFieldCoreLog = new FieldDefn("CoreLon", FieldType.OFTReal);
            oFieldCoreLog.SetWidth(20);
            oFieldCoreLog.SetPrecision(8);
            EventStateLayer.CreateField(oFieldCoreLog, 1);

            //创建
            FieldDefn oFieldCoreLat = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldCoreLat.SetWidth(20);
            oFieldCoreLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldCoreLat, 1);

            //创建
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower.SetWidth(20);
            oFieldPower.SetPrecision(8);
            EventStateLayer.CreateField(oFieldPower, 1);

            FieldDefn oFieldSSTisAbnormal = new FieldDefn("Abnormal", FieldType.OFTString);
            oFieldSSTisAbnormal.SetWidth(20);
            EventStateLayer.CreateField(oFieldSSTisAbnormal, 1);
            #endregion
            #endregion
        }
        private void CreateEventSequenceLayer(string LayerName)
        {
            #region 创建输出图层
            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                return;
            }
            ds_EventSequence = oDriver.CreateDataSource(SavePath, null);
            if (ds_EventSequence == null)
            {
                return;
            }
            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            EventSequenceLayer = ds_EventSequence.CreateLayer(LayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
            #region 创建属性字段 EventSequenceLayer
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID2 = new FieldDefn("PRID", FieldType.OFTInteger);
            EventSequenceLayer.CreateField(oFieldStormID2, 1);

            //先创建一个叫FieldID的整型属性
            FieldDefn oFieldID2 = new FieldDefn("SQID", FieldType.OFTString);
            oFieldID2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldID2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime2 = new FieldDefn("STime", FieldType.OFTString);
            oFieldStartTime2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldStartTime2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldEndTime2 = new FieldDefn("ETime", FieldType.OFTString);
            oFieldEndTime2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldEndTime2, 1);

            //创建x坐标字段
            FieldDefn oFieldDuration2 = new FieldDefn("DurTime", FieldType.OFTReal);
            oFieldDuration2.SetWidth(10);
            oFieldDuration2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldDuration2, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall2 = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall2.SetWidth(20);
            oFieldAvgRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldAvgRainFall2, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall2 = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMinRainFall2.SetWidth(20);
            oFieldMinRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinRainFall2, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall2 = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldMaxRainFall2.SetWidth(20);
            oFieldMaxRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxRainFall2, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog2 = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog2.SetWidth(10);
            oFieldMinLog2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinLog2, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat2 = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat2.SetWidth(10);
            oFieldMinLat2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinLat2, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog2 = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog2.SetWidth(10);
            oFieldMaxLog2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxLog2, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat2 = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat2.SetWidth(10);
            oFieldMaxLat2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxLat2, 1);

            //创建
            FieldDefn oFieldPower2 = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower2.SetWidth(20);
            oFieldPower2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldPower2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldSeqType = new FieldDefn("SeqType", FieldType.OFTString);
            oFieldSeqType.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldSeqType, 1);

            //创建z坐标字段
            FieldDefn oFieldTheta = new FieldDefn("Theta", FieldType.OFTReal);
            oFieldTheta.SetWidth(20);
            oFieldTheta.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldTheta, 1);

            FieldDefn oFieldSSTisAbnormal = new FieldDefn("Abnormal", FieldType.OFTString);
            oFieldSSTisAbnormal.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldSSTisAbnormal, 1);
            //创建
            //FieldDefn oFieldTotalVolume = new FieldDefn("TotalVolume", FieldType.OFTReal);
            //oFieldTotalVolume.SetWidth(24);
            //oFieldTotalVolume.SetPrecision(8);
            //EventLayer.CreateField(oFieldTotalVolume, 1);

            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldTrajectoryID1 = new FieldDefn("TrajectoryID", FieldType.OFTInteger);
            //oFieldTrajectoryID1.SetWidth(28);
            //EventSequenceLayer.CreateField(oFieldTrajectoryID1, 1);
            #endregion
            #endregion
        }
        private void CreateOutLayer()
        {
            #region 创建输出图层
            string strDriver = "ESRI Shapefile";
            OSGeo.OGR.Driver oDriver = Ogr.GetDriverByName(strDriver);
            if (oDriver == null)
            {
                //MessageBox.Show(" 驱动不可用！\n", strVectorFile1);
                return;
            }
            //string temp = SavePath;
            ds_EventState = oDriver.CreateDataSource(SavePath, null);
            ds_Event = oDriver.CreateDataSource(SavePath, null);
            ds_EventSequence = oDriver.CreateDataSource(SavePath, null);
            if (ds_EventState == null || ds_Event == null || ds_EventSequence == null)
            {
                //MessageBox.Show("创建矢量文件【%s】失败！\n", strVectorFile1);
                return;
            }
            string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
            OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
            //string EventStateLayerName = "Event_State";
            //string EventLayerName = "Event";
            //string EventSequenceLayerName = "Event_Sequence";

            //if (File.Exists(SavePath + EventStateLayerName))
            //{
            //    File.Delete(SavePath + EventStateLayerName);
            //    File.Delete(SavePath + EventLayerName);
            //    File.Delete(SavePath + EventSequenceLayerName);
            //}
            EventStateLayer = ds_EventState.CreateLayer(EventStateLayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
            EventLayer = ds_Event.CreateLayer(EventLayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);
            EventSequenceLayer = ds_EventSequence.CreateLayer(EventSequenceLayerName, sr, OSGeo.OGR.wkbGeometryType.wkbPolygon, null);

            #region 接下来创建属性表字段 EventStateLayer
            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldID = new FieldDefn("OID", FieldType.OFTInteger);
            //EventStateLayer.CreateField(oFieldID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID = new FieldDefn("PRID", FieldType.OFTInteger);
            EventStateLayer.CreateField(oFieldStormID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormStateID = new FieldDefn("STID", FieldType.OFTString);
            oFieldStormStateID.SetWidth(20);
            EventStateLayer.CreateField(oFieldStormStateID, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormSQID = new FieldDefn("SQID", FieldType.OFTString);
            oFieldStormSQID.SetWidth(20);
            EventStateLayer.CreateField(oFieldStormSQID, 1);
            //if (outParentsChildren)
            //{
            //    // 先创建一个叫FieldID的整型属性
            //    FieldDefn oFieldParentsID = new FieldDefn("ParentsID", FieldType.OFTString);
            //    oFieldParentsID.SetWidth(50);
            //    olayer1.CreateField(oFieldParentsID, 1);

            //    // 先创建一个叫FieldID的整型属性
            //    FieldDefn oFieldChildrenID = new FieldDefn("ChildrenID", FieldType.OFTString);
            //    oFieldChildrenID.SetWidth(50);
            //    olayer1.CreateField(oFieldChildrenID, 1);
            //}

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime = new FieldDefn("Time", FieldType.OFTString);
            oFieldStartTime.SetWidth(20);
            EventStateLayer.CreateField(oFieldStartTime, 1);

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
            EventStateLayer.CreateField(oFieldMinLog, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat.SetWidth(20);
            oFieldMinLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMinLat, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog.SetWidth(20);
            oFieldMaxLog.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxLog, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat.SetWidth(20);
            oFieldMaxLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxLat, 1);

            //创建平均降雨量字段
            FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
            oFieldArea.SetWidth(20);
            oFieldArea.SetPrecision(8);
            EventStateLayer.CreateField(oFieldArea, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall.SetWidth(20);
            oFieldAvgRainFall.SetPrecision(8);
            EventStateLayer.CreateField(oFieldAvgRainFall, 1);

            //创建体积字段
            FieldDefn oFieldVolume = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldVolume.SetWidth(20);
            oFieldVolume.SetPrecision(8);
            EventStateLayer.CreateField(oFieldVolume, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMaxRainFall.SetWidth(20);
            oFieldMaxRainFall.SetPrecision(8);
            EventStateLayer.CreateField(oFieldMaxRainFall, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            //FieldDefn oFieldLongTime = new FieldDefn("LongTime", FieldType.OFTString);
            //oFieldLongTime.SetWidth(20);
            //olayer1.CreateField(oFieldLongTime, 1);

            //创建
            FieldDefn oFieldCoreLog = new FieldDefn("CoreLon", FieldType.OFTReal);
            oFieldCoreLog.SetWidth(20);
            oFieldCoreLog.SetPrecision(8);
            EventStateLayer.CreateField(oFieldCoreLog, 1);

            //创建
            FieldDefn oFieldCoreLat = new FieldDefn("CoreLat", FieldType.OFTReal);
            oFieldCoreLat.SetWidth(20);
            oFieldCoreLat.SetPrecision(8);
            EventStateLayer.CreateField(oFieldCoreLat, 1);

            //创建
            FieldDefn oFieldPower = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower.SetWidth(20);
            oFieldPower.SetPrecision(8);
            EventStateLayer.CreateField(oFieldPower, 1);
            #endregion

            #region 创建属性字段 EventLayer
            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldID1 = new FieldDefn("OID", FieldType.OFTInteger);
            //EventLayer.CreateField(oFieldID1, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID1 = new FieldDefn("PRID", FieldType.OFTInteger);
            EventLayer.CreateField(oFieldStormID1, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime1 = new FieldDefn("STime", FieldType.OFTString);
            oFieldStartTime1.SetWidth(20);
            EventLayer.CreateField(oFieldStartTime1, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldEndTime1 = new FieldDefn("ETime", FieldType.OFTString);
            oFieldEndTime1.SetWidth(20);
            EventLayer.CreateField(oFieldEndTime1, 1);

            //创建x坐标字段
            FieldDefn oFieldDuration = new FieldDefn("DurTime", FieldType.OFTReal);
            oFieldDuration.SetWidth(10);
            oFieldDuration.SetPrecision(8);
            EventLayer.CreateField(oFieldDuration, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall1 = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall1.SetWidth(20);
            oFieldAvgRainFall1.SetPrecision(8);
            EventLayer.CreateField(oFieldAvgRainFall1, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMinRainFall.SetWidth(20);
            oFieldMinRainFall.SetPrecision(8);
            EventLayer.CreateField(oFieldMinRainFall, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall1 = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldMaxRainFall1.SetWidth(20);
            oFieldMaxRainFall1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxRainFall1, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog1 = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog1.SetWidth(10);
            oFieldMinLog1.SetPrecision(8);
            EventLayer.CreateField(oFieldMinLog1, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat1 = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat1.SetWidth(10);
            oFieldMinLat1.SetPrecision(8);
            EventLayer.CreateField(oFieldMinLat1, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog1 = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog1.SetWidth(10);
            oFieldMaxLog1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxLog1, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat1 = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat1.SetWidth(10);
            oFieldMaxLat1.SetPrecision(8);
            EventLayer.CreateField(oFieldMaxLat1, 1);

            //创建
            FieldDefn oFieldPower1 = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower1.SetWidth(20);
            oFieldPower1.SetPrecision(8);
            EventLayer.CreateField(oFieldPower1, 1);

            //创建
            //FieldDefn oFieldTotalVolume = new FieldDefn("TotalVolume", FieldType.OFTReal);
            //oFieldTotalVolume.SetWidth(24);
            //oFieldTotalVolume.SetPrecision(8);
            //EventLayer.CreateField(oFieldTotalVolume, 1);

            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldTrajectoryID = new FieldDefn("TrajectoryID", FieldType.OFTInteger);
            oFieldTrajectoryID.SetWidth(28);
            EventLayer.CreateField(oFieldTrajectoryID, 1);
            #endregion

            #region 创建属性字段 EventSequenceLayer
            // 先创建一个叫FieldID的整型属性
            FieldDefn oFieldStormID2 = new FieldDefn("PRID", FieldType.OFTInteger);
            EventSequenceLayer.CreateField(oFieldStormID2, 1);

            //先创建一个叫FieldID的整型属性
            FieldDefn oFieldID2 = new FieldDefn("SQID", FieldType.OFTString);
            oFieldID2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldID2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldStartTime2 = new FieldDefn("STime", FieldType.OFTString);
            oFieldStartTime2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldStartTime2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldEndTime2 = new FieldDefn("ETime", FieldType.OFTString);
            oFieldEndTime2.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldEndTime2, 1);

            //创建x坐标字段
            FieldDefn oFieldDuration2 = new FieldDefn("DurTime", FieldType.OFTReal);
            oFieldDuration2.SetWidth(10);
            oFieldDuration2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldDuration2, 1);

            //创建平均降雨量字段
            FieldDefn oFieldAvgRainFall2 = new FieldDefn("AvgValue", FieldType.OFTReal);
            oFieldAvgRainFall2.SetWidth(20);
            oFieldAvgRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldAvgRainFall2, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMinRainFall2 = new FieldDefn("MinValue", FieldType.OFTReal);
            oFieldMinRainFall2.SetWidth(20);
            oFieldMinRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinRainFall2, 1);

            //创建最大降雨量字段
            FieldDefn oFieldMaxRainFall2 = new FieldDefn("MaxValue", FieldType.OFTReal);
            oFieldMaxRainFall2.SetWidth(20);
            oFieldMaxRainFall2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxRainFall2, 1);

            //创建x坐标字段
            FieldDefn oFieldMinLog2 = new FieldDefn("MinLon", FieldType.OFTReal);
            oFieldMinLog2.SetWidth(10);
            oFieldMinLog2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinLog2, 1);
            //创建y坐标字段
            FieldDefn oFieldMinLat2 = new FieldDefn("MinLat", FieldType.OFTReal);
            oFieldMinLat2.SetWidth(10);
            oFieldMinLat2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMinLat2, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLog2 = new FieldDefn("MaxLon", FieldType.OFTReal);
            oFieldMaxLog2.SetWidth(10);
            oFieldMaxLog2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxLog2, 1);
            //创建z坐标字段
            FieldDefn oFieldMaxLat2 = new FieldDefn("MaxLat", FieldType.OFTReal);
            oFieldMaxLat2.SetWidth(10);
            oFieldMaxLat2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldMaxLat2, 1);

            //创建
            FieldDefn oFieldPower2 = new FieldDefn("Power", FieldType.OFTReal);
            oFieldPower2.SetWidth(20);
            oFieldPower2.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldPower2, 1);

            // 再创建一个叫FeatureName的字符型属性，字符长度为50
            FieldDefn oFieldSeqType = new FieldDefn("SeqType", FieldType.OFTString);
            oFieldSeqType.SetWidth(20);
            EventSequenceLayer.CreateField(oFieldSeqType, 1);

            //创建z坐标字段
            FieldDefn oFieldTheta = new FieldDefn("Theta", FieldType.OFTReal);
            oFieldTheta.SetWidth(20);
            oFieldTheta.SetPrecision(8);
            EventSequenceLayer.CreateField(oFieldTheta, 1);
            //创建
            //FieldDefn oFieldTotalVolume = new FieldDefn("TotalVolume", FieldType.OFTReal);
            //oFieldTotalVolume.SetWidth(24);
            //oFieldTotalVolume.SetPrecision(8);
            //EventLayer.CreateField(oFieldTotalVolume, 1);

            // 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldTrajectoryID1 = new FieldDefn("TrajectoryID", FieldType.OFTInteger);
            //oFieldTrajectoryID1.SetWidth(28);
            //EventSequenceLayer.CreateField(oFieldTrajectoryID1, 1);
            #endregion

            #endregion
        }
        #endregion

        public void WriteCSV_Relation(string fileName, string prior_state_id,string next_state_id,string state_action)
        {
            //if (File.Exists(fileName))
            //{
            //    File.Delete(fileName);
            //}
            if (!File.Exists(fileName))
            {
                //创建文件流(创建文件)                
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                //创建流写入对象，并绑定文件流                
                StreamWriter sw = new StreamWriter(fs);
                //实例化字符串流                
                StringBuilder sb = new StringBuilder();
                //将数据添加进字符串流中（如果数据标题有变更，修改此处）                
                //sb.Append("OID").Append(",").Append("EVENT_ID").Append(",").Append("PRIOR_STATE_ID").Append(",").Append("NEXT_STATE_ID").Append(",").Append("STATE_ACTION");
                sb.Append("PRIOR_STATE_ID").Append(",").Append("NEXT_STATE_ID").Append(",").Append("STATE_ACTION");
                //将字符串流数据写入文件                
                sw.WriteLine(sb);
                //刷新文件流                
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            //将数据写入文件             
            //实例化文件写入对象            
            StreamWriter swd = new StreamWriter(fileName, true, Encoding.Default);
            StringBuilder sbd = new StringBuilder();            
            //将需要保存的数据添加到字符串流中            
            sbd.Append(prior_state_id).Append(",").Append(next_state_id).Append(",").Append(state_action);
            swd.WriteLine(sbd);
            swd.Flush();
            swd.Close();
        }

        public void WriteCSV_EVENT_Sequence_State(string fileName,string colName1,string colName2,string id1,string id2)
        {
            if (!File.Exists(fileName)) //当文件不存在时创建文件            
            {
                //创建文件流(创建文件)                
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                //创建流写入对象，并绑定文件流                
                StreamWriter sw = new StreamWriter(fs);
                //实例化字符串流                
                StringBuilder sb = new StringBuilder();
                //将数据添加进字符串流中（如果数据标题有变更，修改此处）                
                sb.Append(colName1).Append(",").Append(colName2);
                //将字符串流数据写入文件                
                sw.WriteLine(sb);
                //刷新文件流                
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            //将数据写入文件             
            //实例化文件写入对象            
            StreamWriter swd = new StreamWriter(fileName, true, Encoding.Default);
            StringBuilder sbd = new StringBuilder();
            //将需要保存的数据添加到字符串流中            
            sbd.Append(id1).Append(",").Append(id2);
            swd.WriteLine(sbd);
            swd.Flush();
            swd.Close();
        }

        public void WriteCSV_Sequence_Sequence(string fileName, string colName1, string colName2, string colName3,string id1, string id2,string id3)
        {
            if (!File.Exists(fileName)) //当文件不存在时创建文件            
            {
                //创建文件流(创建文件)                
                FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                //创建流写入对象，并绑定文件流                
                StreamWriter sw = new StreamWriter(fs);
                //实例化字符串流                
                StringBuilder sb = new StringBuilder();
                //将数据添加进字符串流中（如果数据标题有变更，修改此处）                
                sb.Append(colName1).Append(",").Append(colName2).Append(",").Append(colName3);
                //将字符串流数据写入文件                
                sw.WriteLine(sb);
                //刷新文件流                
                sw.Flush();
                sw.Close();
                fs.Close();
            }
            //将数据写入文件             
            //实例化文件写入对象            
            StreamWriter swd = new StreamWriter(fileName, true, Encoding.Default);
            StringBuilder sbd = new StringBuilder();
            //将需要保存的数据添加到字符串流中            
            sbd.Append(id1).Append(",").Append(id2).Append(",").Append(id3);
            swd.WriteLine(sbd);
            swd.Flush();
            swd.Close();
        }

        private List<List<Sequence>> OrgnizeSequence(List<MarineHeatwavePolygonList>[] stormPolyListsTime)
        {
            Ogr.RegisterAll();
            Gdal.AllRegister();
            //序列分组
            List<List<string>> SeqList = new List<List<string>>();
            List<List<Sequence>> newSeqList = new List<List<Sequence>>();
            List<List<MarineHeatwavePolygon>> ReOrderPolygon = ReOrder(stormPolyListsTime);
            SeqList = new List<List<string>>();
            //int[] aEventID = new int[10000];
            //for (int nn = 0; nn < ReOrderPolygon.Count; nn++)
            //{
            //    for (int mm = 0; mm < ReOrderPolygon[nn].Count; mm++)
            //    {
            //        aEventID[ReOrderPolygon[nn][mm].eventId] = 0;
            //    }
            //}
            for (int nn = 0; nn < ReOrderPolygon.Count; nn++)
            {
                List<Sequence> tempList = new List<Sequence>();
                int iCount = 0;
                for (int mm = 0; mm < ReOrderPolygon[nn].Count; mm++)
                {
                    if (ReOrderPolygon[nn][mm].isSequenceOrder == false && !isAcNode(ReOrderPolygon[nn][mm]))
                    {
                        iCount++;
                        //aEventID[ReOrderPolygon[nn][mm].eventId]++;
                        MarineHeatwavePolygon polygon = ReOrderPolygon[nn][mm];
                        Sequence seq = new Sequence();
                        //if (polygon.eventId == 133 && iCount == 1)
                        //{
                        //    int a = 0;
                        //}
                        seq = ExtractSequence(ref seq, ref polygon, iCount);
                        if (seq.SQList.Count == 0)
                        {
                            iCount--;
                        }
                        else
                        {
                            seq.Head = seq.SQList[0];
                            seq.Tail = seq.SQList[seq.SQList.Count - 1];
                            tempList.Add(seq);
                        }
                    }
                    if (mm == ReOrderPolygon[nn].Count - 1 && tempList.Count != 0)
                    {
                        newSeqList.Add(tempList);
                    }
                }
            }
            #region 按事件组织序列
            //List<List<Sequence>> newSeqList2 = new List<List<Sequence>>();
            //int MaxID = 0;int MinID = 9999999;
            //for (int i=0;i< newSeqList.Count(); i++)//事件
            //{
            //    for (int j=0;j< newSeqList[i].Count;j++)//序列
            //    {
            //        //int num = bEventID[newSeqList[i][j].SQList[0].eventId];
            //        //newSeqList2[newSeqList[i][j].SQList[0].eventId][num] = newSeqList[i][j];
            //        //bEventID[newSeqList[i][j].SQList[0].eventId]++;
            //        if (newSeqList[i][j].SQList[0].eventId > MaxID)
            //        {
            //            MaxID = newSeqList[i][j].SQList[0].eventId;
            //        }
            //        if (newSeqList[i][j].SQList[0].eventId<MinID)
            //        {
            //            MinID = newSeqList[i][j].SQList[0].eventId;
            //        }
            //    }
            //}
            //for (int id = MinID; id <= MaxID; id++)//事件
            //{
            //    List<Sequence> tempList = new List<Sequence>();
            //    for (int i = 0; i < newSeqList.Count(); i++)//事件
            //    {
            //        for (int j = 0; j < newSeqList[i].Count; j++)//序列
            //        {
            //            if (newSeqList[i][j].SQList[0].eventId == id)
            //            {
            //                tempList.Add(newSeqList[i][j]);
            //            }
            //        }
            //    }
            //    newSeqList2.Add(tempList);
            //}
            //newSeqList.Clear();
            //newSeqList = newSeqList2;
            #endregion
            for (int i = 0; i < newSeqList.Count; i++)
            {
                for (int j = 0; j < newSeqList[i].Count; j++)
                {
                    //newSeqList[i][j].SeqID = newSeqList[i][j].SQList[0].eventId + "_" + (j + 1).ToString();
                    Geometry UnionGeo_SeqList = new Geometry(wkbGeometryType.wkbPolygon);
                    for (int m = 0; m < newSeqList[i][j].SQList.Count; m++)
                    {
                        string wkt = new string(newSeqList[i][j].SQList[m].wkt.ToCharArray());
                        UnionGeo_SeqList = UnionGeo_SeqList.Union(Ogr.CreateGeometryFromWkt(ref wkt, null));
                    }
                    UnionGeo_SeqList.ExportToWkt(out newSeqList[i][j].wkt);
                    //序列属性的计算函数
                    newSeqList[i][j].ComputeAttributes();
                }
            }
            //同一过程的序列和连接点两两比较后进行连接，可优化遍历方法
            for (int i = 0; i < newSeqList.Count; i++)
            {
                for (int j = 0; j < newSeqList[i].Count; j++)
                {
                    for (int m = 0; m < newSeqList[i].Count; m++)
                    {
                        if (isChild(newSeqList[i][j].Tail, newSeqList[i][m].Head))
                        {
                            newSeqList[i][j].ChildrenSeqID.Add(newSeqList[i][m].SeqID);
                        }
                        if (isChild(newSeqList[i][m].Tail, newSeqList[i][j].Head))
                        {
                            newSeqList[i][j].ParentsSeqID.Add(newSeqList[i][m].SeqID);
                        }
                    }
                }
            }
            return newSeqList;
        }

        private Sequence ExtractSequence(ref Sequence seq, ref MarineHeatwavePolygon Polygons, int SequenceID)//递归寻找非发展节点间的序列状态编号
        {
            //if (checkPosi)
            //{
            //    Polygons.eventId += eventid;
            //}
            seq.SeqID = Polygons.eventId.ToString() + "_" + SequenceID;
            //if (Polygons.eventId == 133)
            //{
            //    int a = 0;
            //}
            //当前节点 * 为非连接节点 （父节点个数<=1 && 子节点个数<=1）
            if (isNonLinkedNode(Polygons))
            {
                //非连接点：
                // ...- *  子节点为0   尾递归，仅用于结束递归函数
                seq.SeqType = Sequence.SequenceType.SequenceNode;
                if (Polygons.childList.Count == 0)
                {
                    Polygons.isSequenceOrder = true;
                    seq.SQList.Add(Polygons);
                    return seq;
                }
                //子节点为非连接节点
                //  ...- * -0-...   子节点为1 父节点为1
                //       * -0-...   子节点为1 父节点为0
                else if (isNonLinkedNode(Polygons.childList[0]))
                {
                    seq.SQList.Add(Polygons);
                    Polygons.isSequenceOrder = true;
                    MarineHeatwavePolygon polygon = Polygons.childList[0];
                    return ExtractSequence(ref seq, ref polygon, SequenceID);
                }
                //子节点为连接节点
                //  ...- * -0<...   子节点为1 父节点为1 子节点为连接点
                //  ...- * >0-...   
                //  ...- * >0<...
                //  ...- * >0
                //  ...- * -0<
                //       * -0<...    
                //       * >0-...
                //       * >0<...
                //       * >0
                //       * -0<
                else
                {
                    //子节点为连接节点，父节点为非连接节点，该点为序列节点
                    //  ...-0- * -0<...                  
                    //  ...-0- * >0-...
                    //  ...-0- * >0
                    //  ...-0- * -0<
                    if (Polygons.parentList.Count == 1 && isNonLinkedNode(Polygons.parentList[0]))
                    {
                        Polygons.isSequenceOrder = true;
                        seq.SQList.Add(Polygons);
                        return seq;
                    }
                    //子节点为连接节点，无父节点，该点为连接节点
                    //         * -0<...                  
                    //         * >0-...
                    //         * >0
                    //         * -0<
                    else if (Polygons.parentList.Count == 0)
                    {
                        seq.SeqType = Sequence.SequenceType.LinkedNode;
                        Polygons.isSequenceOrder = true;
                        seq.SQList.Add(Polygons);
                        return seq;
                    }
                    //子节点为连接节点，父节点为连接节点，该点为连接节点
                    else
                    {
                        seq.SeqType = Sequence.SequenceType.LinkedNode;
                        Polygons.isSequenceOrder = true;
                        seq.SQList.Add(Polygons);
                        return seq;
                    }

                }
            }
            //当前节点为连接连接节点
            //连接节点：
            //  ...-0<...
            //  ...>0<...
            //  ...>0
            //  ...>0-...
            //      0<...
            else
            {
                seq.SeqType = Sequence.SequenceType.LinkedNode;
                seq.SQList.Add(Polygons);
                Polygons.isOrder = true;
                return seq;
            }
        }

        private List<List<MarineHeatwavePolygon>> ReOrder(List<MarineHeatwavePolygonList>[] stormPolyListsTime)
        {
            List<List<MarineHeatwavePolygon>> ReOrderPolygon = new List<List<MarineHeatwavePolygon>>();
            for (int i = 0; i < stormPolyListsTime.Count(); i++)
            {
                for (int j = 0; j < stormPolyListsTime[i].Count; j++)
                {
                    int tempID = stormPolyListsTime[i][j].head.eventId;
                    if (checkPosi)
                    {
                        tempID = tempID - eventid + 1;
                    }
                    if (tempID - 1 < ReOrderPolygon.Count)
                    {
                        for (int k = 0; k < stormPolyListsTime[i][j].mhPolygons.Count; k++)
                        {
                            if (!(stormPolyListsTime[i][j].mhPolygons[k].isAcnode && (stormPolyListsTime[i][j].mhPolygons[k].parentList.Count + stormPolyListsTime[i][j].mhPolygons[k].childList.Count == 1)))
                            {
                                ReOrderPolygon[tempID - 1].Add(stormPolyListsTime[i][j].mhPolygons[k]);
                            }
                            else
                            {
                                //删去父节点中子节点的连接
                                for (int kk = 0; kk < stormPolyListsTime[i][j].mhPolygons[k].parentList.Count; kk++)
                                {
                                    stormPolyListsTime[i][j].mhPolygons[k].parentList[kk].childList.Remove(stormPolyListsTime[i][j].mhPolygons[k]);
                                }
                                //删去子节点中子父节点的连接
                                for (int kk = 0; kk < stormPolyListsTime[i][j].mhPolygons[k].childList.Count; kk++)
                                {
                                    stormPolyListsTime[i][j].mhPolygons[k].childList[kk].parentList.Remove(stormPolyListsTime[i][j].mhPolygons[k]);
                                }
                            }

                        }
                    }
                    else
                    {
                        List<MarineHeatwavePolygon> tempList = new List<MarineHeatwavePolygon>();
                        for (int k = 0; k < stormPolyListsTime[i][j].mhPolygons.Count; k++)
                        {
                            if (!(stormPolyListsTime[i][j].mhPolygons[k].isAcnode && (stormPolyListsTime[i][j].mhPolygons[k].parentList.Count + stormPolyListsTime[i][j].mhPolygons[k].childList.Count == 1)))
                            {
                                tempList.Add(stormPolyListsTime[i][j].mhPolygons[k]);
                            }
                            else
                            {
                                //删去父节点中子节点的连接
                                for (int kk = 0; kk < stormPolyListsTime[i][j].mhPolygons[k].parentList.Count; kk++)
                                {
                                    stormPolyListsTime[i][j].mhPolygons[k].parentList[kk].childList.Remove(stormPolyListsTime[i][j].mhPolygons[k]);
                                }
                                //删去子节点中子父节点的连接
                                for (int kk = 0; kk < stormPolyListsTime[i][j].mhPolygons[k].childList.Count; kk++)
                                {
                                    stormPolyListsTime[i][j].mhPolygons[k].childList[kk].parentList.Remove(stormPolyListsTime[i][j].mhPolygons[k]);
                                }
                            }
                        }
                        ReOrderPolygon.Add(tempList);
                    }
                }
            }
            //for (int i=0;i<ReOrderPolygon.Count;i++)
            //{
            //    string str = string.Empty;
            //    for (int j=0;j<ReOrderPolygon[i].Count;j++)
            //    {
            //        str += ReOrderPolygon[i][j].eventId + "_" + ReOrderPolygon[i][j].stateId + " ";
            //    }
            //    Console.WriteLine((i+1)+": "+str);
            //}
            return ReOrderPolygon;
        }

        private bool isNonLinkedNode(MarineHeatwavePolygon polygon)
        {
            return !(polygon.childList.Count > 1 || polygon.parentList.Count > 1);
        }

        private bool isAcNode(MarineHeatwavePolygon polygon)
        {
            return polygon.parentList.Count == 0 && polygon.childList.Count == 0;
        }

        private bool isChild(MarineHeatwavePolygon polygon1, MarineHeatwavePolygon polygon2)
        {
            if (polygon1.childList.Contains(polygon2)) return true;
            return false;
        }

        int EventSequenceLayerNum = 1;
        public void SaveSequence(List<List<Sequence>> SequenceList)
        {
            //string FileEventSequenceName = "Event_Sequence";
            string FileSequenceSequenceName = "Sequence_Sequence";
            //int outFileEventSequenceNum = 1;
            int outFileSequenceSequenceNum = 1;
            long count1 = 0;
            long count2 = 0;
            for (int i=0;i< SequenceList.Count;i++)//事件数
            {
                for (int j=0;j< SequenceList[i].Count;j++)//每个事件所包含的序列数
                {
                    double minlon = double.MaxValue;
                    double minlat = double.MaxValue;
                    double maxlon = double.MinValue;
                    double maxlat = double.MinValue;
                    for (int k=0;k< SequenceList[i][j].SQList.Count;k++)//每个序列包含的节点数
                    {
                        if (minlon> SequenceList[i][j].SQList[k].minLog)
                        {
                            minlon = SequenceList[i][j].SQList[k].minLog;
                        }
                        if (minlat > SequenceList[i][j].SQList[k].minLat)
                        {
                            minlat = SequenceList[i][j].SQList[k].minLat;
                        }
                        if (maxlon < SequenceList[i][j].SQList[k].maxLog)
                        {
                            maxlon = SequenceList[i][j].SQList[k].maxLog;
                        }
                        if (maxlat < SequenceList[i][j].SQList[k].maxLat)
                        {
                            maxlat = SequenceList[i][j].SQList[k].maxLat;
                        }
                        //string SQID = SequenceList[i][j].SQList[0].eventId.ToString() + "_" + (j + 1).ToString();//序列ID
                        SequenceList[i][j].SQList[k].SQID = SequenceList[i][j].SeqID;
                        //string SQID_StateID = SequenceList[i][j].SQList[0].eventId.ToString() + "_" + SequenceList[i][j].SQList[k].stateId.ToString();//序列节点状态ID
                        //WriteCSV_EVENT_Sequence_State(Sequence_StateFilepath, "SQID","SQID_StateID",SQID, SQID_StateID);//序列状态表
                    }
                    count1++;
                    if (count1 / 1000 >= EventSequenceLayerNum)
                    {
                        EventSequenceLayer.Dispose();
                        ds_EventSequence.Dispose();
                        string temp = EventSequenceLayerName + EventSequenceLayerNum.ToString();
                        CreateEventSequenceLayer(temp);
                        EventSequenceLayerNum++;
                    }
                    #region 写入数据
                    FeatureDefn oDefn = EventSequenceLayer.GetLayerDefn();
                    Feature oFeature = new Feature(oDefn);
                    //oFeature.SetField("OID", oid);
                    oFeature.SetField(0, SequenceList[i][j].SQList[0].eventId);
                    //int SequenceID = j + 1;
                    //oFeature.SetField(1, SequenceList[i][j].SQList[0].eventId.ToString() + "_" + SequenceID.ToString());//SQID
                    oFeature.SetField(1, SequenceList[i][j].SeqID);//SQID
                    oFeature.SetField(2, SequenceList[i][j].SQList[0].startTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    //SequenceList[i][j].SQList[0].time
                    int lastIndex = SequenceList[i][j].SQList.Count - 1;
                    oFeature.SetField(3, SequenceList[i][j].SQList[lastIndex].endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    TimeSpan duration = SequenceList[i][j].SQList[lastIndex].endTime - SequenceList[i][j].SQList[0].startTime;
                    int tempDaysNum = (int)duration.TotalDays;
                    int MonthNum = tempDaysNum / 366 * 12 + tempDaysNum % 366 / 31 + 1;
                    oFeature.SetField(4, MonthNum);
                    oFeature.SetField(5, SequenceList[i][j].AvgValue);
                    oFeature.SetField(6, SequenceList[i][j].MinValue);
                    oFeature.SetField(7, SequenceList[i][j].MaxValue);
                    if (minlon>180.0)
                    {
                        minlon = minlon - 360;
                    }
                    oFeature.SetField(8, minlon);
                    oFeature.SetField(9, minlat);
                    if (maxlon > 180.0)
                    {
                        maxlon = maxlon - 360;
                    }
                    oFeature.SetField(10, maxlon);
                    oFeature.SetField(11, maxlat);
                    oFeature.SetField(12, 0);
                    string strType;
                    if (SequenceList[i][j].SeqType == Sequence.SequenceType.LinkedNode)
                    {
                        strType = "LinkedNode";
                    }
                    else
                    {
                        strType = "SequenceNode";
                    }
                    oFeature.SetField(13, strType);
                    oFeature.SetField(14, SequenceList[i][j].Theta);
                    oFeature.SetField(15, isAbnormal);
                    Geometry geoPolygon = Geometry.CreateFromWkt(SequenceList[i][j].wkt);
                    oFeature.SetGeometry(geoPolygon);
                    EventSequenceLayer.CreateFeature(oFeature);
                    //释放资源
                    oDefn.Dispose();
                    geoPolygon.Dispose();
                    oFeature.Dispose();
                    #endregion
                    //事件序列表
                    //if (count1/3000 >= outFileEventSequenceNum)
                    //{
                    //    outFileEventSequenceNum++;
                    //    //Event_SequenceFilepath = SavePath + FileEventSequenceName + outFileEventSequenceNum.ToString() + ".csv";
                    //    FileEventSequenceName += outFileEventSequenceNum.ToString();
                    //}
                    //string Event_SequenceFilepath = SavePath + FileEventSequenceName + ".csv";
                    //WriteCSV_EVENT_Sequence_State(Event_SequenceFilepath, "PRID","SQID",SequenceList[i][j].SQList[0].eventId.ToString(), SequenceList[i][j].SeqID);
                    //序列-序列表
                    if (count2 / 3000 >= outFileSequenceSequenceNum)
                    {
                        outFileSequenceSequenceNum++;
                        //Sequence_SequenceFilepath = SavePath + FileEventSequenceName + outFileEventSequenceNum.ToString() + ".csv";
                        FileSequenceSequenceName += outFileSequenceSequenceNum.ToString();
                    }
                    string Sequence_SequenceFilepath = SavePath + FileSequenceSequenceName + ".csv";
                    if (SequenceList[i][j].ChildrenSeqID.Count == 1)//子节点==1
                    {
                        int index = SequenceList[i][j].ChildrenSeqID[0].IndexOf("_");
                        string strID = SequenceList[i][j].ChildrenSeqID[0].Substring(index + 1);
                        int ChildrenSeqID = int.Parse(strID);
                        //子节点数为1，子节点的父节点数大于1，合并
                        if (SequenceList[i][ChildrenSeqID - 1].ParentsSeqID.Count > 1)
                        {
                            count2++;
                            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[0], "merge");
                        }
                        else//子节点的父节点数==1,发展
                        {
                            count2++;
                            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[0], "develop");
                        }
                    }
                    if (SequenceList[i][j].ChildrenSeqID.Count > 1)//子节点>1
                    {
                        for (int k = 0; k < SequenceList[i][j].ChildrenSeqID.Count; k++)
                        {
                            int index = SequenceList[i][j].ChildrenSeqID[0].IndexOf("_");
                            string strID = SequenceList[i][j].ChildrenSeqID[0].Substring(index + 1);
                            int ChildrenSeqID = int.Parse(strID);

                            if (SequenceList[i][ChildrenSeqID - 1].ParentsSeqID.Count > 1)//子节点的父节点数大于1，分裂合并
                            {
                                count2++;
                                WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[k], "split_then_merge");
                            }
                            else//子节点的父节点数==1,分裂
                            {
                                count2++;
                                WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[k], "split");
                            }
                        }
                    }
                    #region error
                    //if (SequenceList[i][j].ParentsSeqID.Count > 1 && SequenceList[i][j].ChildrenSeqID.Count <= 1)//合并
                    //{
                    //    for (int k=0;k < SequenceList[i][j].ParentsSeqID.Count;k++)
                    //    {
                    //        int index = SequenceList[i][j].ParentsSeqID[k].IndexOf("_");
                    //        string strID = SequenceList[i][j].ParentsSeqID[k].Substring(index + 1);
                    //        int ParentsSeqID = int.Parse(strID);
                    //        if (SequenceList[i][ParentsSeqID-1].ChildrenSeqID.Count>1)//父节点的子节点数>1
                    //        {
                    //            //序列-序列表
                    //            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ParentsSeqID[k], "split_then_merge");
                    //        }
                    //        else//父节点的子节点数=1
                    //        {
                    //            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ParentsSeqID[k], "merge");
                    //        }
                    //    }
                    //}
                    //if (SequenceList[i][j].ParentsSeqID.Count <= 1 && SequenceList[i][j].ChildrenSeqID.Count > 1)//分裂
                    //{
                    //    for (int k = 0; k < SequenceList[i][j].ChildrenSeqID.Count; k++)
                    //    {
                    //        int index = SequenceList[i][j].ChildrenSeqID[k].IndexOf("_");
                    //        string strID = SequenceList[i][j].ChildrenSeqID[k].Substring(index + 1);
                    //        int ChildrenSeqID = int.Parse(strID);
                    //        if (SequenceList[i][ChildrenSeqID - 1].ParentsSeqID.Count > 1)//子节点的父节点数>1
                    //        {
                    //            //序列-序列表
                    //            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[k], "split_then_merge");
                    //        }
                    //        else//子节点的父节点数=1
                    //        {
                    //            WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[k], "split");
                    //        }
                    //    }
                    //}
                    //if (SequenceList[i][j].ParentsSeqID.Count <= 1 && SequenceList[i][j].ChildrenSeqID.Count <= 1)//发展
                    //{
                    //    //序列 - 序列表
                    //    if (SequenceList[i][j].ParentsSeqID.Count > 0)
                    //    {
                    //        WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ParentsSeqID[0], "develop");
                    //    }
                    //    if (SequenceList[i][j].ChildrenSeqID.Count > 0)
                    //    {
                    //        WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[0], "develop");
                    //    }
                    //}
                    //if (SequenceList[i][j].ParentsSeqID.Count > 1 && SequenceList[i][j].ChildrenSeqID.Count > 1)//父子节点都大于1
                    //{
                    //    //for (int k=0;k< SequenceList[i][j].ParentsSeqID.Count;k++)
                    //    //{
                    //    //    WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ParentsSeqID[k], "merge");
                    //    //}
                    //    for (int k = 0; k < SequenceList[i][j].ChildrenSeqID.Count; k++)
                    //    {
                    //        WriteCSV_Sequence_Sequence(Sequence_SequenceFilepath, "SQID_1", "SQID_2", "Relation", SequenceList[i][j].SeqID, SequenceList[i][j].ChildrenSeqID[k], "split");
                    //    }
                    //}
                    #endregion
                }
            }
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
