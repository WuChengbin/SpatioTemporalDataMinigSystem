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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClusterAlgorithmForms.GUI
{
    public partial class FormClusterShpAddAttribute : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public FormClusterShpAddAttribute()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法，后台执行避免主界面卡死
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

                string oriPath = oriFileNames[fileId].ToString();//原始图像路径
                string shpPath = shpFileNames[fileId].ToString();//shp图层路径
                string pidPath = pidFileNames[fileId].ToString();//聚类结果路径


                #region 读取原始tiff图像,读取聚类结果tiff图像
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用，因此，所有路径都为英文

                //打开tiff文件
                Dataset oriDs = Gdal.Open(oriPath, Access.GA_ReadOnly);
                Dataset pidDs = Gdal.Open(pidPath, Access.GA_ReadOnly);
                int col = oriDs.RasterXSize;//列数
                int row = oriDs.RasterYSize;//行数
                Band oriBand1 = oriDs.GetRasterBand(1);//读取波段
                Band pidBand1 = pidDs.GetRasterBand(1);

                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                oriDs.GetGeoTransform(argout);//读取地理坐标信息

                string[] metadatas = oriDs.GetMetadata("");//获取元数据

                //基本属性初始化，后边会获取,不同数据不一样
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

                double[] oriData1 = new double[row * col];//原始图像数据
                double[] pidData = new double[row * col];//原始图像数据
                oriBand1.ReadRaster(0, 0, col, row, oriData1, col, row, 0, 0);//读取数据
                pidBand1.ReadRaster(0, 0, col, row, pidData, col, row, 0, 0);//读取数据
                oriDs.Dispose();
                pidDs.Dispose();

                double[] oriData = new double[row * col];//需要乘以scale

                for (int i = 0; i < row * col; i++)
                {
                    oriData[i] = oriData1[i] * mScale;
                }

                List<ClusterRaster> CrasterL = new List<ClusterRaster>();
                for (int j = 0; j < row * col; j++)
                {
                    ClusterRaster Craster = new ClusterRaster();
                    Craster.Clusterid = (int)pidData[j];
                    Craster.isVisited = false;
                    double SCol = startLat + (row - j / col - 1) * resolution;//纬度
                    double ECol = SCol + resolution;
                    double SRow = startLog + (j % col) * resolution;//经度
                    double ERow = SRow + resolution;
                    Craster.col = (SCol + ECol) / 2;
                    Craster.row = (SRow + ERow) / 2;//栅格中心的经纬度
                    CrasterL.Add(Craster);

                }


                #endregion



                #region shp添加属性
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
                Gdal.SetConfigOption("SHAPE_ENCODING", "");
                Ogr.RegisterAll();// 注册所有的驱动
                DataSource ds = Ogr.Open(shpPath, 1);//0表示只读，1表示可修改  


                if (ds == null)
                {//数据为空
                    ds.Dispose();//关闭数据集
                    return;
                }
                int iLayerCount = ds.GetLayerCount();//图层个数
                Layer oLayer = ds.GetLayerByIndex(0);// 获取第一个图层


                if (oLayer == null)
                {//图层为空
                    oLayer.Dispose();
                    return;
                }
                long featureCount = oLayer.GetFeatureCount(0);

                //输出属性表字段的详细信息，数据类型、宽度、精度等
                FeatureDefn oDefn = oLayer.GetLayerDefn();
                int fieldCount = oDefn.GetFieldCount();

                // 创建过程ID整型属性
                FieldDefn oFieldProcessID = new FieldDefn("ProcessID", FieldType.OFTInteger);
                oFieldProcessID.SetWidth(20);
                oLayer.CreateField(oFieldProcessID, 1);

                //创建状态ID属性,唯一标识每个多边形
                FieldDefn oFieldStateID = new FieldDefn("StateID", FieldType.OFTString);
                oFieldStateID.SetWidth(30);
                oLayer.CreateField(oFieldStateID, 1);

                //创建面积属性
                FieldDefn oFieldArea = new FieldDefn("Area", FieldType.OFTReal);
                oFieldArea.SetWidth(15);
                oFieldArea.SetPrecision(8);
                oLayer.CreateField(oFieldArea, 1);

                //创建要素属性最大值
                FieldDefn oFieldMaxValue = new FieldDefn("MaxValue", FieldType.OFTReal);
                oFieldMaxValue.SetWidth(15);
                oFieldMaxValue.SetPrecision(8);
                oLayer.CreateField(oFieldMaxValue, 1);

                //创建要素属性最小值
                FieldDefn oFieldMinValue = new FieldDefn("MinValue", FieldType.OFTReal);
                oFieldMinValue.SetWidth(20);
                oFieldMinValue.SetPrecision(8);
                oLayer.CreateField(oFieldMinValue, 1);

                //创建要素属性均值
                FieldDefn oFieldAvgValue = new FieldDefn("AvgValue", FieldType.OFTReal);
                oFieldAvgValue.SetWidth(20);
                oFieldAvgValue.SetPrecision(8);
                oLayer.CreateField(oFieldAvgValue, 1);

                //创建强度属性
                FieldDefn oFieldPowerValue = new FieldDefn("Power", FieldType.OFTReal);
                oFieldPowerValue.SetWidth(20);
                oFieldPowerValue.SetPrecision(8);
                oLayer.CreateField(oFieldPowerValue, 1);

                //创建质心字段
                FieldDefn oFieldLogCore = new FieldDefn("CoreLog", FieldType.OFTReal);
                oFieldLogCore.SetWidth(10);
                oFieldLogCore.SetPrecision(8);
                oLayer.CreateField(oFieldLogCore, 1);

                //创建质心字段
                FieldDefn oFieldLatCore = new FieldDefn("CoreLat", FieldType.OFTReal);
                oFieldLatCore.SetWidth(10);
                oFieldLatCore.SetPrecision(8);
                oLayer.CreateField(oFieldLatCore, 1);

                int idnumber = 0;//每个shp图层中的多边形编号

                Feature oFeature = null;//用来遍历每个多边形
                while ((oFeature = oLayer.GetNextFeature()) != null)
                {//每个多边形

                    double processID = oFeature.GetFieldAsDouble("GRIDCODE");
                    string time = oFeature.GetFieldAsString("Time");

                    string polygonWkt = string.Empty;//wkt坐标
                    oFeature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);//获取多边形的WKT坐标

                    //解析WKT坐标，并将多边形顶点存入nodelist中
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
                            PolygonNode.Clusterid = (int)processID;
                            PolygonNode.isVisited = false;
                            PolygonNode.col = lat;
                            PolygonNode.row = log;
                            NodeList.Add(PolygonNode);
                        }
                        break;//为了避免出现内部有镂空的现象，这样会造成结果比实际面积和强度都大
                    }



                    int pID = (int)processID;//获取时空簇ID，即过程ID
                    string pids = pID.ToString();
                    string numbers = idnumber.ToString();

                    string stateID = pids + '_' + time + '_' + numbers;//格式“3423_1952-02_12”

                    double Aera = 0.0;//面积
                    double Power = 0.0;//强度
                    double volum = 0.0;//要素属性体积，即属性乘以每个栅格的面积
                    double sum = 0.0;//要素属性和，简单加和
                    int num = 0;//栅格个数
                    double avgValue = 0.0;//要素属性均值
                    double maxValue = double.MinValue;//要素属性最大值
                    double minValue = double.MaxValue;//要素属性最小值
                    double _rowCore = 0.0;//重心行号中间量
                    double _colCore = 0.0;//重心列号中间量



                    for (int i = 0; i < row * col; i++)
                    {
                        ClusterRaster raster = CrasterL[i];

                        if (IsInPolygonNew(CrasterL[i], NodeList))
                        {
                            //raster.isVisited = true;
                            double rasterStartLat = startLat + (row - i / col - 1) * resolution;//栅格下边缘纬度，i/列数为行，i%列数
                            double rasterEndLat = rasterStartLat + resolution;//栅格上边缘纬度
                            double rasterArea = GetRasterArea(rasterStartLat, rasterEndLat, resolution);//计算一个网格面积
                            Aera += rasterArea;
                            num++;

                            double value = oriData[i];
                            if (value > maxValue) maxValue = value;
                            if (value < minValue) minValue = value;
                            double _volum = rasterArea * value;
                            volum += _volum;
                            sum += value;

                            //加权
                            _rowCore += _volum * (i / col);
                            _colCore += _volum * (i % col);

                        }
                    }

                    _rowCore = _rowCore / volum;
                    _colCore = _colCore / volum;


                    //不同的海洋要素，power的计算方法不同

                    string[] DatasetNameSpilt = DatasetName.Split('_');
                    DatasetName = DatasetNameSpilt[0];
                    switch (DatasetName)
                    {
                        case "NPP":
                            Power = volum * 30;
                            avgValue = volum / Aera;
                            break;
                        case "SST":
                            Power = sum;
                            avgValue = sum / num;
                            break;
                        case "ANOM":
                            Power = sum;
                            avgValue = sum / num;
                            break;

                    }


                    double coreLog = startLog + (_colCore + 0.5) * resolution;//质心经度
                    double coreLat = startLat + (row - _rowCore - 0.5) * resolution;//质心纬度




                    oFeature.SetField(4, pID);//过程id赋值
                    oFeature.SetField(5, stateID);//状态id赋值
                    oFeature.SetField(6, Aera);//面积赋值
                    oFeature.SetField(7, maxValue);//最大值赋值
                    oFeature.SetField(8, minValue);//最小值赋值
                    oFeature.SetField(9, avgValue);//均值赋值
                    oFeature.SetField(10, Power);//强度赋值
                    oFeature.SetField(11, coreLog);//质心经度赋值
                    oFeature.SetField(12, coreLat);//质心纬度赋值


                    oLayer.SetFeature(oFeature);//保存多边形

                    idnumber++;
                }
                oLayer.Dispose();
                ds.Dispose();
                #endregion
            }
        }
        struct ClusterRaster
        {
            public int Clusterid;//聚簇结果id
            public bool isVisited;//是否被访问
            public double col;//纬度
            public double row;//经度

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
                if (n1.col > (checkNode.col - 0.5))
                {//右侧
                    if (n1.row >= (checkNode.row - 0.5) && n2.row < (checkNode.row - 0.5))
                    {
                        inside = !inside;
                    }
                    else if (n1.row < (checkNode.row - 0.5) && n2.row >= (checkNode.row - 0.5))
                    {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }

        /// <summary>
        /// 计算一个经纬网格面积
        /// </summary>
        /// <param name="rasterStartLat">网格起始纬度,角度值</param>
        /// <param name="rasterEndLat">网格结束纬度,角度值</param>
        /// <param name="rasterLog">网格跨越经度范围</param>
        /// <returns>经纬网格实际面积，单位平方千米</returns>
        public static double GetRasterArea(double rasterStartLat, double rasterEndLat, double rasterLog)
        {
            double earthRadius = 6371.393;//单位千米
            double cutCount = 360.0 / rasterLog;//一个圆环被切割的份数

            //将地球视为球体的经纬1°网格计算公式为2πr²(sin(α+1)-sin(α))/360，半径为r,在纬度为α，https://www.zybang.com/question/deba669df1201d9c8d5c95e003716524.html
            double rasterArea = 2 * Math.PI * earthRadius * earthRadius * (Math.Sin(Math.PI * rasterEndLat / 180) - Math.Sin(Math.PI * rasterStartLat / 180)) / cutCount;//积分计算面积

            return rasterArea;
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (!e.Cancelled)
            {
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //foreach (Thread t in threads)
                //{
                //    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                //}
                //toolStripStatusLabel1.Text = "处理取消";
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            progressBar1.Hide();
            //okBtn.Enabled = true;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            progressBar1.Value = e.ProgressPercentage;

        }

        #region 无关紧要的按钮事件
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

        private void oriAddFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox3.Items.Add(fileName);
                }
                oriCountTextBox.Text = listBox3.Items.Count.ToString();
            }
        }

        private void pidAddFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox2.Items.Add(fileName);
                }
                pidCountTextBox.Text = listBox2.Items.Count.ToString();
            }

        }


        private void oriDeleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox3.SelectedIndex;
            while (index > -1)
            {
                listBox3.Items.RemoveAt(index);
                index = listBox3.SelectedIndex;
            }
            oriCountTextBox.Text = listBox3.Items.Count.ToString();
        }

        private void pidDeleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox2.SelectedIndex;
            while (index > -1)
            {
                listBox2.Items.RemoveAt(index);
                index = listBox2.SelectedIndex;
            }
            oriCountTextBox.Text = listBox2.Items.Count.ToString();
        }

        private void oriMoveUpBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox3.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox3.Items.Insert(endIndex + 1, listBox3.Items[startIndex - 1].ToString());
                    listBox3.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }
        private void pidMoveDownBtn_Click(object sender, EventArgs e)
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

        private void oriMoveDownBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox3.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox3.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox3.Items.Insert(startIndex, listBox3.Items[endIndex + 1].ToString());
                    listBox3.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox3.SelectedIndices;
                }
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
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.Description = "选择输出文件夹";
            //if (fbd.ShowDialog() == DialogResult.OK)
            //{//确定
            //    textBox1.Text = fbd.SelectedPath;
            //}
        }
        #endregion

        ListBox.ObjectCollection oriFileNames;//原始图像路径，含文件名
        ListBox.ObjectCollection shpFileNames;//shp图层路径，含文件名
        ListBox.ObjectCollection pidFileNames;//聚类结果id，即过程id，含文件名
        int fileCount;//图像文件个数
        DateTime pStartTime;//程序执行开始时间
        //int threadCount = 1;//线程个数
        private void okBtn_Click(object sender, EventArgs e)
        {//确认按钮
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
            if (!(listBox3.Items.Count == listBox1.Items.Count && listBox1.Items.Count == listBox2.Items.Count))
            {//数目不同
                MessageBox.Show("图像数目不完全相同！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            oriFileNames = listBox3.Items;
            shpFileNames = listBox1.Items;
            pidFileNames = listBox2.Items;

            fileCount = oriFileNames.Count;
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
    }
}
