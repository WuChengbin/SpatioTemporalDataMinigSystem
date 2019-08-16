using OSGeo.GDAL;
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
    public partial class MarineHeatwavesMedianForm : Form
    {
        string inFolderPath = string.Empty;
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public MarineHeatwavesMedianForm()
        {
            InitializeComponent();

            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            for (DateTime time2018 = new DateTime(2018, 1, 1); time2018.Year < 2019; time2018=time2018.AddDays(1))
            {//一年中每一天距平
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (time2018.DayOfYear * 100) / 365;
                worker.ReportProgress(progress);//记录进度

                int startYear = 1981;
                int endYear = 2018;
                string month = time2018.ToString("MM");
                string day = time2018.ToString("dd");
                List<string> files = new List<string>();//存储路径
                List<int> years = new List<int>();//储存存在的年份
                for (int year = startYear; year <= endYear; year++)
                {//每一年该天
                    string filePath = inFolderPath + "\\" + year + month + day + ".tif";
                    if (File.Exists(filePath))
                    {//存在该路径
                        files.Add(filePath);
                        years.Add(year);
                    }
                }
                int mFileNum = files.Count;//文件个数
                string fileName = files[0].ToString();
                Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                int colCount = ds.RasterXSize;//列数
                int rowCount = ds.RasterYSize;//行数
                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ds.GetGeoTransform(argout);//读取地理坐标信息
                string projection = ds.GetProjection();//坐标系
                string[] metadatas = ds.GetMetadata("");//获取元数据
                ds.Dispose();

                int[] medianValues = new int[colCount * rowCount];//中位数图像
                                                                        //每行处理
                for (int rowId = 0; rowId < rowCount; rowId++)
                {
                    int[,] dataCols = new int[mFileNum, colCount];
                    for (int fileId = 0; fileId < mFileNum; fileId++)
                    {//处理每个文件
                        ds = Gdal.Open(files[fileId].ToString(), Access.GA_ReadOnly);
                        Band demband1 = ds.GetRasterBand(1);//读取波段
                        int[] databuf = new int[colCount];//存储
                        demband1.ReadRaster(0, rowId, colCount, 1, databuf, colCount, 1, 0, 0);//读取数据
                        //double[] databuf2 = new double[colCount * rowCount];//存储
                        //demband1.ReadRaster(0, 0, colCount, rowCount, databuf2, colCount, rowCount, 0, 0);//读取数据
                        demband1.Dispose();
                        ds.Dispose();

                        //保存
                        for (int colId = 0; colId < databuf.Length; colId++)
                        {
                            dataCols[fileId, colId] = databuf[colId];
                        }
                    }

                    //排序
                    for (int colId = 0; colId < colCount; colId++)
                    {
                        int lo = 0;//起始位置
                        int hi = mFileNum;//结束位置
                        while (!bubble(ref dataCols, colId, lo, hi--)) ;//冒泡排序
                    }

                    int medianId = mFileNum / 2;//中位数位置
                    for (int colId = 0; colId < colCount; colId++)
                    {
                        if(mFileNum%2==0)
                        {//总数为偶数
                            medianValues[rowId * colCount + colId] =Convert.ToInt32((dataCols[medianId, colId]+dataCols[medianId-1, colId])/2.0);
                        }
                        else
                        {//总数为奇数
                            medianValues[rowId * colCount + colId] = dataCols[medianId, colId];
                        }
                    }
                }

                //保存
                string outPath = outFolderPath + "\\" + month + day +".tif";
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, colCount, rowCount, 1, DataType.GDT_Int16, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartYear", years[0].ToString(), null);
                gTiffDataset.SetMetadataItem("EndYear", years[years.Count-1].ToString(), null);
                for (int i = 0; i < metadatas.Length; i++)
                {
                    string[] metadata = metadatas[i].Split('=');
                    gTiffDataset.SetMetadataItem(metadata[0], metadata[1], null);
                }
                gTiffDataset.WriteRaster(0, 0, colCount, rowCount, medianValues, colCount, rowCount, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
        }

        private bool bubble(ref int[,] dataCols, int colId, int lo, int hi)
        {
            bool sorted = true;//整体有序标志
            while(++lo<hi)//自左向右，逐一检查各对相邻元素
            {
                if(dataCols[lo - 1,colId]> dataCols[lo,colId])
                {//若逆序，则
                    sorted = false;
                    //交换
                    dataCols[lo - 1, colId] += dataCols[lo, colId];
                    dataCols[lo, colId] = dataCols[lo - 1, colId] - dataCols[lo, colId];
                    dataCols[lo - 1, colId] -= dataCols[lo, colId];
                }
            }
            return sorted;
        }

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
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            progressBar1.Value = e.ProgressPercentage;
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

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (!System.IO.Directory.Exists(textBox2.Text.Trim()))
            {
                MessageBox.Show("输入文件夹路径有误！");
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
                    MessageBox.Show("输出文件夹路径有误！");
                    return;
                }
            }

            inFolderPath = textBox2.Text.Trim();
            inFolderPath = inFolderPath.TrimEnd('/').TrimEnd('\\');
            outFolderPath = textBox1.Text.Trim();
            outFolderPath = outFolderPath.TrimEnd('/').TrimEnd('\\');
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

        private void selectButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输入文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox2.Text = fbd.SelectedPath;
            }
        }
    }
}
