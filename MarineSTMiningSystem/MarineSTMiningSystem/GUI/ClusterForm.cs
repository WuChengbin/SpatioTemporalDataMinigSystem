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

namespace MarineSTMiningSystem
{
    public partial class ClusterForm : Form
    {
        //string outFolderPath = "";//文件输出路径
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public ClusterForm()
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
            for(int fileId=0;fileId<fileCount;fileId++)
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

                #region 读取原始图像
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

                //打开hdf文件
                Dataset oriDs = Gdal.Open(oriPath, Access.GA_ReadOnly);
                int col = oriDs.RasterXSize;//列数
                int row = oriDs.RasterYSize;//行数
                Band oriBand1 = oriDs.GetRasterBand(1);//读取波段

                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                oriDs.GetGeoTransform(argout);//读取地理坐标信息

                string[] metadatas = oriDs.GetMetadata("");//获取元数据
                double startLog = 70.0;//起始经度
                double startLat = 3.0;//起始维度
                double endLog = 140.0;//结束经度
                double endLat = 53.0;//结束维度
                double mScale = 1.0;//比例
                string dataType = "";
                string imgDate = "";
                string fillValue = "";
                double resolution = 0.1;//分辨率

                string fileName = Path.GetFileName(oriPath);
                DateTime startTime = DateTime.ParseExact(fileName.Substring(0, 16), "yyyyMMdd-SHHmmss", System.Globalization.CultureInfo.CurrentCulture);//起始时间 

                foreach (string md in metadatas)
                {//获取信息
                    string[] mdArr = md.Split('=');
                    switch (mdArr[0])
                    {
                        case "StartLog":
                            startLog = Convert.ToDouble(mdArr[1]);//起始经度
                            break;
                        case "StartLat":
                            startLat = Convert.ToDouble(mdArr[1]);//起始经度
                            break;
                        case "EndLog":
                            endLog = Convert.ToDouble(mdArr[1]);//起始经度
                            break;
                        case "EndLat":
                            endLat = Convert.ToDouble(mdArr[1]);//起始经度
                            break;
                        case "Scale":
                            mScale = Convert.ToDouble(mdArr[1]);//起始经度
                            break;
                        case "DataType":
                            dataType = mdArr[1];//起始经度
                            break;
                        case "ImageDate":
                            imgDate = mdArr[1];
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

                double[] oriData = new double[row * col];//原始图像数据
                oriBand1.ReadRaster(0, 0, col, row, oriData, col, row, 0, 0);//读取数据
                oriDs.Dispose();
                #endregion

                #region 处理shp
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

                //string wkt = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";//自定义投影坐标系的WKT
                //OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(wkt);
                //oLayer.GetSpatialRef(sr);

                if (oLayer == null)
                {//图层为空
                    oLayer.Dispose();
                    return;
                }
                long featureCount = oLayer.GetFeatureCount(0);

                //输出属性表字段的详细信息，数据类型、宽度、精度等
                FeatureDefn oDefn = oLayer.GetLayerDefn();
                int fieldCount = oDefn.GetFieldCount();

                // 先创建一个叫FieldID的整型属性
                FieldDefn oFieldStateID = new FieldDefn("CeShi", FieldType.OFTString);
                oFieldStateID.SetWidth(20);
                oLayer.CreateField(oFieldStateID, 1);

                Feature oFeature = null;//用来遍历每个多边形
                while ((oFeature = oLayer.GetNextFeature()) != null)
                {//每个多边形
                    double stormArea = oFeature.GetFieldAsDouble("area");//获取area属性值
                    oFeature.SetField(25, "abe");//设置多边形属性
                    oLayer.SetFeature(oFeature);//保存多边形
                }
                oLayer.Dispose();
                ds.Dispose();
                #endregion
            }
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (!e.Cancelled)
            {
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！");
            }
            else
            {
                //foreach (Thread t in threads)
                //{
                //    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                //}
                //toolStripStatusLabel1.Text = "处理取消";
                MessageBox.Show("处理取消！");
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
        int fileCount;//图像文件个数
        double valueScale;
        double timeCell;
        //double maxTimeInv;
        DateTime pStartTime;//程序执行开始时间
        //int threadCount = 1;//线程个数
        private void okBtn_Click(object sender, EventArgs e)
        {//确认按钮
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
            if (listBox1.Items.Count != listBox3.Items.Count)
            {//数目不同
                MessageBox.Show("图像数目不完全相同！");
                return;
            }

            //if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            //{
            //    try
            //    {
            //        System.IO.Directory.CreateDirectory(textBox1.Text.Trim()); //新建文件夹   
            //    }
            //    catch
            //    {
            //        MessageBox.Show("输出文件夹路径有误！");
            //        return;
            //    }
            //}

            //outFolderPath = textBox1.Text.Trim();
            //if (!outFolderPath.EndsWith("\\")) outFolderPath += "\\";
            oriFileNames = listBox3.Items;
            shpFileNames = listBox1.Items;
            //valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            //timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            //timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            //maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离
            //threadCount = Convert.ToInt32(threadTextBox.Text.Trim());
            //threads = new Thread[threadCount];
            //okBtn.Enabled = false;
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
                MessageBox.Show("未进行处理！");
            }
        }
    }
}
