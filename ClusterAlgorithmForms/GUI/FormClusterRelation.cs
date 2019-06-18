using System;
using System.Collections.Generic;
using System.ComponentModel;
using OSGeo.GDAL;
using OSGeo.OGR;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClusterAlgorithmForms.GUI
{
    public partial class FormClusterRelation : Form
    {
        string outFolderPath = "";
        string[] AfilesNames;//距平数据列表
        bool isOpened = false;//是否打开距平数据
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public FormClusterRelation()
        {
            InitializeComponent();


            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void worke(object sender, DoWorkEventArgs e)
        {

            List<ST_Cluster> clusterList = new List<ST_Cluster>();//所有聚簇
            List<int> clusterPIdList = new List<int>();//每个聚簇对应过程id，所有图层的
            List<ClusterShp> clusterShpList = new List<ClusterShp>(); //记录之前的聚簇图层，最大长度为6
            List<String> stringlist = new List<String>();//簇间关系结果列表
            List<String> Clusterstringlist = new List<String>();//时空簇属性记录列表


            // 后台工作方法，后台执行避免主界面卡死

            for (int fileId = 0; fileId < fileCount; fileId++)
            {//遍历每个文件
                if (worker.CancellationPending)
                {//判断点击取消按钮，终端执行
                    e.Cancel = true;
                    return;
                }

                //更新进度条
                int progress = fileId * 100 / fileCount;//进度
                worker.ReportProgress(progress);//记录进度

                string shpPath = shpFileNames[fileId].ToString();//shp图层路径

                ClusterShp clustershp = new ClusterShp(shpPath);//读取shp

                //以下三个list是为了将maxpower和maxarea按照每个时刻一个总值计算
                List<int> Pidlist = new List<int>();//存储每个图层中包含的PID
                List<double> Powerlist = new List<double>();//存储每个pid多边形的power
                List<double> Arealist = new List<double>();//存储每个pid多边形的area

                foreach (Feature feature in clustershp.featureList)
                {
                    int featureId = feature.GetFieldAsInteger("ID");//之前转成shp的时候就自带
                    double Pid = feature.GetFieldAsDouble("ProcessID");
                    int pid = (int)Pid;
                    double aera = feature.GetFieldAsDouble("Area");
                    double power = feature.GetFieldAsDouble("Power");
                    string time = feature.GetFieldAsString("Time");
                    string stateid = feature.GetFieldAsString("StateID");
                    string polygonWkt = string.Empty;//wkt坐标
                    feature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);

                    Pidlist.Add(pid);
                    Powerlist.Add(power);
                    Arealist.Add(aera);

                    //构建聚簇多边形
                    ClusterPolygon Clusterpoly = new ClusterPolygon(polygonWkt);
                    Clusterpoly.id = featureId;
                    Clusterpoly.pid = pid;
                    Clusterpoly.aera = aera;
                    Clusterpoly.power = power;
                    Clusterpoly.stateid = stateid;
                    Clusterpoly.time = time;
                    clustershp.clusterPolyList.Add(Clusterpoly);
                    if (!clustershp.clusterPIdList.Contains(Clusterpoly.pid))
                    {//新的时空簇，记录簇过程id
                        clustershp.clusterPIdList.Add(Clusterpoly.pid);
                    }

                    //时空簇处理
                    if (clusterPIdList.Contains(pid))//之前存在此过程标号的时空簇
                    {
                        int pos = clusterPIdList.IndexOf(pid);
                        ST_Cluster cluster = clusterList[pos];

                        cluster.pid = pid;
                        cluster.sumAera += aera;
                        cluster.sumpower += power;
                        cluster.endTime = time;
                        if (aera > cluster.maxAera)
                            cluster.maxAera = aera;
                        if (Math.Abs(power) > Math.Abs(cluster.maxpower))
                            cluster.maxpower = power;
                        if (Math.Abs(power) < Math.Abs(cluster.minpower))
                            cluster.minpower = power;
                        if (isOpened && AfilesNames.Length > 0)
                        {
                            string AnomalyPath = AfilesNames[fileId];//距平数据路径

                            Gdal.AllRegister();//注册所有的格式驱动
                            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");

                            //打开tiff文件
                            Dataset anomalDs = Gdal.Open(AnomalyPath, Access.GA_ReadOnly);
                            int col = anomalDs.RasterXSize;//列数
                            int row = anomalDs.RasterYSize;//行数
                            Band anomalBand1 = anomalDs.GetRasterBand(1);//读取波段

                            double[] argout = new double[6];
                            anomalDs.GetGeoTransform(argout);//读取地理坐标信息

                            string[] metadatas = anomalDs.GetMetadata("");//获取元数据

                            double startLog = 100;//起始经度
                            double startLat = -50;//起始维度
                            double endLog = 300;//结束经度
                            double endLat = 50;//结束维度
                            double mScale = 0.01;//比例
                            string dataType = "";
                            string DatasetName = "";

                            string fillValue = "";
                            double resolution = 2;//分辨率

                            foreach (string md in metadatas)
                            {//获取信息
                                string[] mdArr = md.Split('=');
                                switch (mdArr[0])
                                {
                                    case "StartLog":
                                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                                        break;
                                    case "StartLat":
                                        startLat = Convert.ToDouble(mdArr[1]);//起始纬度
                                        break;
                                    case "EndLog":
                                        endLog = Convert.ToDouble(mdArr[1]);//终止经度
                                        break;
                                    case "EndLat":
                                        endLat = Convert.ToDouble(mdArr[1]);//终止纬度
                                        break;
                                    case "Scale":
                                        mScale = Convert.ToDouble(mdArr[1]);
                                        break;
                                    case "DataType":
                                        dataType = mdArr[1];
                                        break;
                                    case "DatasetName":
                                        DatasetName = mdArr[1];
                                        break;
                                    case "FillValue":
                                        fillValue = mdArr[1];
                                        break;
                                    case "DSResolution":
                                        resolution = Convert.ToDouble(mdArr[1]);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            double[] AnomaData1 = new double[row * col];//距平图像数据，未乘以尺度系数
                            anomalBand1.ReadRaster(0, 0, col, row, AnomaData1, col, row, 0, 0);//读取数据
                            anomalDs.Dispose();
                            double[] AnomaData = new double[row * col];//距平图像数据，乘以尺度系数

                            for (int i = 0; i < row * col; i++)
                            {
                                AnomaData[i] = AnomaData1[i] * mScale;
                            }

                            List<ClusterRaster> CrasterL = new List<ClusterRaster>();
                            for (int j = 0; j < row * col; j++)
                            {
                                ClusterRaster Craster = new ClusterRaster();
                                Craster.value = AnomaData[j];
                                Craster.isVisited = false;
                                double SCol = startLat + (row - j / col - 1) * resolution;//纬度
                                double ECol = SCol + resolution;
                                double SRow = startLog + (j % col) * resolution;//经度
                                double ERow = SRow + resolution;
                                Craster.col = (SCol + ECol) / 2;
                                Craster.row = (SRow + ERow) / 2;//栅格中心的经纬度
                                CrasterL.Add(Craster);

                            }

                            List<ClusterRaster> NodeList = new List<ClusterRaster>();
                            int subPos = polygonWkt.IndexOf('(');
                            polygonWkt = polygonWkt.Substring(subPos);
                            polygonWkt = polygonWkt.TrimStart('(');
                            polygonWkt = polygonWkt.TrimEnd(')');

                            string[] lineWktArr = polygonWkt.Split(new string[] { "),(" }, StringSplitOptions.None);
                            foreach (string lineWkt in lineWktArr)
                            {
                                string[] pointWktArr = lineWkt.TrimStart('(').TrimEnd(')').Split(',');
                                foreach (string pointWkt in pointWktArr)
                                {
                                    string[] coor = pointWkt.Split(' ');
                                    double log = Convert.ToDouble(coor[0]);//经度
                                    double lat = Convert.ToDouble(coor[1]);//纬度
                                    ClusterRaster PolygonNode = new ClusterRaster();
                                    // PolygonNode.Clusterid = (int)processID;
                                    PolygonNode.isVisited = false;
                                    PolygonNode.col = lat;
                                    PolygonNode.row = log;
                                    NodeList.Add(PolygonNode);
                                }
                            }

                            for (int i = 0; i < row * col; i++)
                            {
                                ClusterRaster raster = CrasterL[i];

                                if (IsInPolygonNew(CrasterL[i], NodeList))
                                {
                                    if (CrasterL[i].value > 0)
                                    {
                                        cluster.valuetype = "high";
                                        break;
                                    }
                                    if (CrasterL[i].value < 0)
                                    {
                                        cluster.valuetype = "low";
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (power > 0)
                                cluster.valuetype = "high";
                            if (power < 0)
                                cluster.valuetype = "low";
                        }


                        clusterList[pos] = cluster;//修改簇


                    }
                    else
                    {
                        //不存在该暴雨
                        ST_Cluster cluster = new ST_Cluster();
                        cluster.pid = pid;
                        cluster.sumAera = aera;
                        cluster.maxAera = aera;
                        cluster.sumpower = power;
                        cluster.startTime = time;
                        cluster.maxpower = power;
                        cluster.minpower = power;
                        if (isOpened && AfilesNames.Length > 0)
                        {
                            string AnomalyPath = AfilesNames[fileId];//距平数据路径

                            Gdal.AllRegister();//注册所有的格式驱动
                            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");

                            //打开tiff文件
                            Dataset anomalDs = Gdal.Open(AnomalyPath, Access.GA_ReadOnly);
                            int col = anomalDs.RasterXSize;//列数
                            int row = anomalDs.RasterYSize;//行数
                            Band anomalBand1 = anomalDs.GetRasterBand(1);//读取波段

                            double[] argout = new double[6];
                            anomalDs.GetGeoTransform(argout);//读取地理坐标信息

                            string[] metadatas = anomalDs.GetMetadata("");//获取元数据

                            double startLog = 100;//起始经度
                            double startLat = -50;//起始维度
                            double endLog = 300;//结束经度
                            double endLat = 50;//结束维度
                            double mScale = 0.01;//比例
                            string dataType = "";
                            string DatasetName = "";

                            string fillValue = "";
                            double resolution = 2;//分辨率

                            foreach (string md in metadatas)
                            {//获取信息
                                string[] mdArr = md.Split('=');
                                switch (mdArr[0])
                                {
                                    case "StartLog":
                                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                                        break;
                                    case "StartLat":
                                        startLat = Convert.ToDouble(mdArr[1]);//起始纬度
                                        break;
                                    case "EndLog":
                                        endLog = Convert.ToDouble(mdArr[1]);//终止经度
                                        break;
                                    case "EndLat":
                                        endLat = Convert.ToDouble(mdArr[1]);//终止纬度
                                        break;
                                    case "Scale":
                                        mScale = Convert.ToDouble(mdArr[1]);
                                        break;
                                    case "DataType":
                                        dataType = mdArr[1];
                                        break;
                                    case "DatasetName":
                                        DatasetName = mdArr[1];
                                        break;
                                    case "FillValue":
                                        fillValue = mdArr[1];
                                        break;
                                    case "DSResolution":
                                        resolution = Convert.ToDouble(mdArr[1]);
                                        break;
                                    default:
                                        break;
                                }
                            }

                            double[] AnomaData1 = new double[row * col];//距平图像数据，未乘以尺度系数
                            anomalBand1.ReadRaster(0, 0, col, row, AnomaData1, col, row, 0, 0);//读取数据
                            anomalDs.Dispose();
                            double[] AnomaData = new double[row * col];//距平图像数据，乘以尺度系数

                            for (int i = 0; i < row * col; i++)
                            {
                                AnomaData[i] = AnomaData1[i] * mScale;
                            }

                            List<ClusterRaster> CrasterL = new List<ClusterRaster>();
                            for (int j = 0; j < row * col; j++)
                            {
                                ClusterRaster Craster = new ClusterRaster();
                                Craster.value = AnomaData[j];
                                Craster.isVisited = false;
                                double SCol = startLat + (row - j / col - 1) * resolution;//纬度
                                double ECol = SCol + resolution;
                                double SRow = startLog + (j % col) * resolution;//经度
                                double ERow = SRow + resolution;
                                Craster.col = (SCol + ECol) / 2;
                                Craster.row = (SRow + ERow) / 2;//栅格中心的经纬度
                                CrasterL.Add(Craster);

                            }

                            List<ClusterRaster> NodeList = new List<ClusterRaster>();
                            int subPos = polygonWkt.IndexOf('(');
                            polygonWkt = polygonWkt.Substring(subPos);
                            polygonWkt = polygonWkt.TrimStart('(');
                            polygonWkt = polygonWkt.TrimEnd(')');

                            string[] lineWktArr = polygonWkt.Split(new string[] { "),(" }, StringSplitOptions.None);
                            foreach (string lineWkt in lineWktArr)
                            {
                                string[] pointWktArr = lineWkt.TrimStart('(').TrimEnd(')').Split(',');
                                foreach (string pointWkt in pointWktArr)
                                {
                                    string[] coor = pointWkt.Split(' ');
                                    double log = Convert.ToDouble(coor[0]);//经度
                                    double lat = Convert.ToDouble(coor[1]);//纬度
                                    ClusterRaster PolygonNode = new ClusterRaster();
                                    // PolygonNode.Clusterid = (int)processID;
                                    PolygonNode.isVisited = false;
                                    PolygonNode.col = lat;
                                    PolygonNode.row = log;
                                    NodeList.Add(PolygonNode);
                                }
                            }

                            for (int i = 0; i < row * col; i++)
                            {
                                ClusterRaster raster = CrasterL[i];

                                if (IsInPolygonNew(CrasterL[i], NodeList))
                                {
                                    if (CrasterL[i].value > 0)
                                    {
                                        cluster.valuetype = "high";
                                        break;
                                    }
                                    if (CrasterL[i].value < 0)
                                    {
                                        cluster.valuetype = "low";
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (power > 0)
                                cluster.valuetype = "high";
                            if (power < 0)
                                cluster.valuetype = "low";
                        }

                        clusterList.Add(cluster);//记录时空簇
                        clusterPIdList.Add(cluster.pid);//记录时空簇过程id
                    }
                }

                //重新计算最大power和最大area
                //首先，先将同一图层的同一pid的多边形的power和area相加

                List<int> isread = new List<int>();
                List<int> sameindex = new List<int>();
                for (int i = 0; i < Pidlist.Count; i++)
                {
                    sameindex.Clear();
                    if (isread.Contains(i))
                        continue;
                    for (int j = 0; j < Pidlist.Count; j++)
                    {
                        if (j != i)
                        {
                            if (Pidlist[j] == Pidlist[i])
                            {
                                Powerlist[i] += Powerlist[j];
                                Arealist[i] += Arealist[j];
                                isread.Add(j);
                                sameindex.Add(j);
                            }
                        }
                    }
                    for (int k = 0; k < sameindex.Count; k++)
                    {
                        Powerlist[sameindex[k]] = Powerlist[i];
                        Arealist[sameindex[k]] = Arealist[i];
                    }
                }

                //比较相加后的属性是否比之前赋值的max值大，如果是则替换
                for (int i = 0; i < Pidlist.Count; i++)
                {
                    int pos = clusterPIdList.IndexOf(Pidlist[i]);
                    ST_Cluster cluster = clusterList[pos];
                    if (cluster.maxAera < Arealist[i])
                    {
                        cluster.maxAera = Arealist[i];
                    }
                    if (Math.Abs(cluster.maxpower) < Math.Abs(Powerlist[i]))
                    {
                        cluster.maxpower = Powerlist[i];
                    }
                    clusterList[pos] = cluster;//修改簇
                }

                if (fileId > 0)
                {
                    for (int i = 0; i < clusterShpList.Last().clusterPolyList.Count; i++)
                    {
                        ClusterPolygon clusterPolyPrior = clusterShpList.Last().clusterPolyList[i];
                        for (int j = 0; j < clustershp.clusterPolyList.Count; j++)
                        {
                            ClusterPolygon clusterPolyNext = clustershp.clusterPolyList[j];
                            if (clusterPolyPrior.pid == clusterPolyNext.pid)
                                if (clusterPolyPrior.Overlap(clusterPolyNext))
                                {
                                    clusterPolyNext.parentPosList.Add(i);//记录父多边形位置
                                    clusterPolyPrior.childPosList.Add(j);//记录子多边形位置
                                }
                        }

                    }

                    //时空簇内多边形的关系
                    foreach (ClusterPolygon clusterPolyPrior in clusterShpList.Last().clusterPolyList)
                    {
                        bool isSplit = false;//是否分裂
                        if (clusterPolyPrior.childPosList.Count > 1)
                        {
                            isSplit = true;
                        }
                        foreach (int childStateId in clusterPolyPrior.childPosList)
                        {
                            bool isMerge = false;//是否合并
                            if (clustershp.clusterPolyList[childStateId].parentPosList.Count > 1)
                            {
                                isMerge = true;
                            }
                            string stateAction = "develop";//0：发展 1：分裂 2：合并 3：分裂合并
                            if (isSplit && isMerge) stateAction = "split_then_merge";//分裂合并
                            else if (isSplit) stateAction = "split";//分裂
                            else if (isMerge) stateAction = "merge";//合并

                            string relationstr = clusterPolyPrior.pid.ToString() + "," + clusterPolyPrior.stateid + "," + clustershp.clusterPolyList[childStateId].stateid + "," + stateAction;

                            stringlist.Add(relationstr);

                        }
                    }

                    //时空簇过程属性处理

                    foreach (int PId in clusterShpList[0].clusterPIdList)
                    {//第一个图层的每个过程id
                        bool clusterEnd = true;//默认时空簇结束

                        for (int i = 1; i < clusterShpList.Count; i++)
                        {//后面的每个图层
                            if (clusterShpList[i].clusterPIdList.Contains(PId))
                            {
                                clusterEnd = false;
                                break;
                            }
                        }
                        if (clusterEnd)
                        {//比较最新图层
                            if (clustershp.clusterPIdList.Contains(PId))
                                clusterEnd = false;
                        }
                        if (clusterEnd)
                        {//该时空簇结束
                            int pos = clusterPIdList.IndexOf(PId);//查找位置
                            ST_Cluster cluster = clusterList[pos];

                            string clusterstr = GetClusterProcessString(cluster);

                            Clusterstringlist.Add(clusterstr);

                            //移除时空簇
                            clusterPIdList.RemoveAt(pos);
                            clusterList.RemoveAt(pos);

                        }
                    }

                    if (fileId >= fileCount - 1)
                    {//最后一个文件
                        foreach(var cluster in clusterList)//保存在最后一个图层结束的时空簇过程信息
                        {
                            string clusterstr = GetClusterProcessString(cluster);

                            Clusterstringlist.Add(clusterstr);
                        }

                        clusterPIdList.Clear();
                        clusterList.Clear();
                        foreach (ClusterShp _clustershp in clusterShpList)
                        {
                            _clustershp.Dispose();//释放及保存

                        }
                        clusterShpList.Clear();

                    }
                }
                clusterShpList.Add(clustershp);//记录图层

                if (clusterShpList.Count > 6)
                {
                    clusterShpList[0].Dispose();//释放及保存
                    clusterShpList.RemoveAt(0);//移除第一个图层
                }



            }

            string outPath = outFolderPath + "ClusterRelation" + ".txt";//写出簇内部关系
            string outPath2 = outFolderPath + "ClusterProcess" + ".txt";//写出簇过程属性

            FileStream fs = null;
            FileStream cs = null;

            if (!File.Exists(outPath))//如果不存在文件创建
            {
                fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            }
            else//如果存在就打开，因此处理新的数据时，需要在不同路径，或者在相同路径下保证这个txt是空的
            {
                fs = new FileStream(outPath, FileMode.Append, FileAccess.Write);
            }

            if (!File.Exists(outPath2))
                cs = new FileStream(outPath2, FileMode.Create, FileAccess.Write);
            else
                cs = new FileStream(outPath2, FileMode.Append, FileAccess.Write);


            StreamWriter sw = new StreamWriter(fs);
            StreamWriter cw = new StreamWriter(cs);

            sw.WriteLine("PID,FromStateID,ToStateID,StateAction");//表头

            cw.WriteLine("PID,StartTime,EndTime,Duration,SumArea,MaxArea,SumPower,MaxPower,MinPower,ValueType");//表头

            for (int i = 0; i < stringlist.Count; i++)
            {
                sw.WriteLine(stringlist[i]);//writeline函数写出一行，默认换行

            }
            sw.Flush();
            fs.Close();

            for (int j = 0; j < Clusterstringlist.Count; j++)
            {
                cw.WriteLine(Clusterstringlist[j]);
            }
            cw.Flush();
            cs.Close();


        }

        public static int MonthDifference(DateTime Stime, DateTime Etime)
        {
            return (Math.Abs((Stime.Month - Etime.Month) + 12 * (Stime.Year - Etime.Year)) + 1);
        }

        struct ClusterRaster
        {
            public int Clusterid;//聚簇结果id
            public bool isVisited;//是否被访问
            public double col;//纬度
            public double row;//经度
            public double value;//值

        }


        //时空簇过程属性记录
        private string GetClusterProcessString(ST_Cluster cluster)
        {
            //字符串转时间
            DateTime startTime = DateTime.ParseExact(cluster.startTime, "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);//起始时间
            DateTime endTime = DateTime.ParseExact(cluster.endTime, "yyyy-MM", System.Globalization.CultureInfo.CurrentCulture);//终止时间

            string duration = MonthDifference(startTime, endTime).ToString();

            string clusterstr = cluster.pid.ToString() + "," + cluster.startTime + "," + cluster.endTime + "," + duration + "," + cluster.sumAera.ToString() + "," + cluster.maxAera.ToString() + "," + cluster.sumpower.ToString() + "," + cluster.maxpower.ToString() + "," + cluster.minpower.ToString() + "," + cluster.valuetype;

            return clusterstr;
        }

        //判断栅格点是否在多边形内部
        private static bool IsInPolygonNew(ClusterRaster checkNode, List<ClusterRaster> nodes)
        {
            bool inside = false;
            ClusterRaster n1, n2;
            for (int i = 0; i < nodes.Count - 1; i++)
            {
                n1 = nodes[i];
                n2 = nodes[i + 1];
                if (n1.col > checkNode.col)
                {//右侧
                    if (n1.row >= checkNode.row && n2.row < checkNode.row)
                    {
                        inside = !inside;
                    }
                    else if (n1.row < checkNode.row && n2.row >= checkNode.row)
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        //按钮操作
        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (!e.Cancelled)
            {

                //toolStripStatusLabel1.Text = "处理完成";
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //foreach (Thread t in threads)
                //{
                //if (t.ThreadState != ThreadState.Stopped) t.Abort();
                //}
                //toolStripStatusLabel1.Text = "处理取消";
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            progressBar1.Hide();
            //okBtn.Enabled = true;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            progressBar1.Value = e.ProgressPercentage;
            //TimeSpan costTime = DateTime.Now - pStartTime;//已经花费时间

        }

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "shp文件|*.shp|所有文件|*.*";
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
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(endIndex + 1, listBox1.Items[startIndex - 1].ToString());
                    listBox1.Items.RemoveAt(startIndex - 1);
                }
            }
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox1.Text = fbd.SelectedPath;
            }
        }

        ListBox.ObjectCollection shpFileNames;//shp图层路径，含文件名

        int fileCount;//图像文件个数
        DateTime pStartTime;//程序执行开始时间

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("请添加处理文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(textBox1.Text.Trim()); //新建文件夹   
                }
                catch
                {
                    MessageBox.Show("输出文件夹路径有误！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            outFolderPath = textBox1.Text.Trim();
            if (!outFolderPath.EndsWith("\\")) outFolderPath += "\\";

            shpFileNames = listBox1.Items;


            fileCount = shpFileNames.Count;
            pStartTime = DateTime.Now;//记录程序开始时间
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
                MessageBox.Show("未进行处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AnomalyAdd_Click(object sender, EventArgs e)
        {
            isOpened = true;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图像文件|*.tiff;*.tif|tiff文件|*.tiff|tif文件|*.tif|所有文件|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AfilesNames = ofd.FileNames;

            }
        }
    }
}
