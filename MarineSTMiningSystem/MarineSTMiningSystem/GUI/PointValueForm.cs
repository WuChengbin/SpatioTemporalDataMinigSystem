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
    public partial class PointValueForm : Form
    {
        string outFilePath = "";
        private BackgroundWorker worker = new BackgroundWorker();//后台线程

        public PointValueForm()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            //string[] files = Directory.GetFiles(inFolderPath);//所有文件名
            int mFileNum = inFileNames.Count;//文件个数
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            string hdfFileName = inFileNames[0].ToString();
            //string hdfFileName = @"E:\BaoYuTime\XM_2014_CJ_tif";
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            //string projection = ds.GetProjection();
            //double[] geoTransformO = new double[6];
            //ds.GetGeoTransform(geoTransformO);
            int col = ds.RasterXSize;//列数
            int row = ds.RasterYSize;//行数
            Band demband1 = ds.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息

            //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLog = 70.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 60.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            double resolution = 0.1;//每像素宽度
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
                        mScale = Convert.ToDouble(mdArr[1]);//值比例
                        break;
                    case "DataType":
                        dataType = mdArr[1];//
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);//值比例
                        break;
                    default:
                        break;
                }
            }
            ds.Dispose();

            int pRow = (int)Math.Ceiling(DoubleRvide((endLat - lat) , resolution)) - 1;//点所在行号
            int pCol = (int)Math.Floor(DoubleRvide((log - startLog) , resolution));//点所在列号
            StreamWriter sw = new StreamWriter(outFilePath);
            //double[] valueList = new double[mFileNum];//用来存储结果
            //处理所有文件切割部分
            for (int i = 0; i < mFileNum; i++)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (i*100)/ mFileNum;//进度

                worker.ReportProgress(progress);//记录进度
                //hdfFileName = oriFilesName[i];
                //打开文件
                ds = Gdal.Open(inFileNames[i].ToString(), Access.GA_ReadOnly);
                Band demband = ds.GetRasterBand(1);//读取波段
                int[] databuf = new int[1 * 1];//存储该切割部分的数组
                demband.ReadRaster(pCol, pRow, 1, 1, databuf, 1, 1, 0, 0);//读取数据
                //valueList[i] = Convert.ToDouble(databuf[0]) * mScale;
                sw.WriteLine((Convert.ToDouble(databuf[0]) * mScale).ToString());
                ds.Dispose();
            }
            sw.Close();
        }

        /// <summary>
        /// 解决除不尽的情况
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>结果</returns>
        public double DoubleRvide(double a, double b)
        {
            double temp = System.Math.IEEERemainder(a, b);
            if (Math.Abs(temp) < 2E-10)
            {
                return Math.Round(a / b);
            }
            else
            {
                return (a / b);
            }
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (!e.Cancelled)
            {
                //toolStripStatusLabel1.Text = "处理完成";
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！");
            }
            else
            {
                //toolStripStatusLabel1.Text = "处理取消";
                MessageBox.Show("处理取消！");
            }
            progressBar1.Hide();
            //okBtn.Enabled = true;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            progressBar1.Value = e.ProgressPercentage;
        }

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
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

        private void openBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            sfd.Filter = "文本文件|*.txt;*.asc";
            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            //ofd.Multiselect = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = sfd.FileName; //获得文件路径
            }
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


        ListBox.ObjectCollection inFileNames;
        double log;
        double lat;
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            inFileNames = listBox1.Items;
            outFilePath = textBox1.Text;
            if (inFileNames.Count == 0)
            {
                MessageBox.Show("请添加处理文件！");
                return;
            }

            log = Convert.ToDouble(logTextBox.Text.Trim());//值的比例系数
            lat = Convert.ToDouble(latTextBox.Text.Trim());//每条记录时间尺度，小时单位

            //okBtn.Enabled = false;
            progressBar1.Show();
            worker.RunWorkerAsync();
        }
    }
}
