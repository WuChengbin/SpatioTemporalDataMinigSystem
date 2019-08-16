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

namespace MarineSTMiningSystem
{
    public partial class MarineHeatwavesSpaceExtractionForm : Form
    {
        int timeCell = 1;//时间间隔（天）
        //string oriFolderPath = string.Empty;
        string timeFolderPath = string.Empty;
        string uFolderPath = string.Empty;//流场纬线方向速度
        string vFolderPath = string.Empty;//流场经线方向速度
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        int maxThreadCount = 1;
        Thread[] threads;
        public MarineHeatwavesSpaceExtractionForm()
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
            string[] timeFilePaths = Directory.GetFiles(timeFolderPath, "*.tif");
            //string[] uFilePaths = Directory.GetFiles(uFolderPath, "*.tif");
            //string[] vFilePaths = Directory.GetFiles(vFolderPath, "*.tif");
            int fileCount = timeFilePaths.Length;
            //int activeThreads = 0;
            for (int fileId=0;fileId<fileCount;fileId++)
            {//每一天文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / fileCount;
                worker.ReportProgress(progress);//记录进度

                string timeFileName = Path.GetFileNameWithoutExtension(timeFilePaths[fileId]);
                //string oriFilePath = oriFolderPath + "\\" + timeFileName;//原始图像路径
                string outFilePath = outFolderPath + "\\" + timeFileName+".shp";//输出图像路径

                //查找时间最近的uv数据
                DateTime dt= DateTime.ParseExact(timeFileName, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                string uFilePath = uFolderPath + "\\" + timeFileName + ".tif";
                string vFilePath = vFolderPath + "\\" + timeFileName + ".tif";
                int addDay = 1;
                while(!File.Exists(uFilePath))
                {
                    uFilePath = uFolderPath + "\\" + dt.AddDays(addDay).ToString("yyyyMMdd") + ".tif";
                    addDay = -addDay;
                    if(addDay>0) addDay++;
                }
                addDay = 1;
                while (!File.Exists(vFilePath))
                {
                    vFilePath = vFolderPath + "\\" + dt.AddDays(addDay).ToString("yyyyMMdd") + ".tif";
                    addDay = -addDay;
                    if (addDay > 0) addDay++;
                }

                ThreadInfo threadInfo = new ThreadInfo();
                threadInfo.timeFilePath = timeFilePaths[fileId];
                threadInfo.uFilePath = uFilePath;
                threadInfo.vFilePath = vFilePath;
                threadInfo.outFilePath = outFilePath;
                threadInfo.timeCell = timeCell;

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(ThreadWork));
                threads[tId].IsBackground = true;
                threads[tId].Start(threadInfo);
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.IsAlive)
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

        private void ThreadWork(object obj)
        {
            ThreadInfo threadInfo = (ThreadInfo)obj;
            ResterToVector.MarineHeatwavesTifToShp(threadInfo.timeFilePath, threadInfo.uFilePath, threadInfo.vFilePath, threadInfo.outFilePath, threadInfo.timeCell);
        }

        struct ThreadInfo
        {
            public string timeFilePath;
            public string outFilePath;
            public string uFilePath;
            public string vFilePath;
            public int timeCell;
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            foreach (Thread t in threads)
            {
                t.Abort();
            }
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

        private void selectButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择u数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox2.Text = fbd.SelectedPath;
            }
        }

        private void selectButton4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择数据输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox4.Text = fbd.SelectedPath;
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

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            //if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            //{
            //    MessageBox.Show("原始数据文件夹路径有误！");
            //    return;
            //}
            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                MessageBox.Show("时间数据文件夹路径有误！");
                return;
            }
            if (!System.IO.Directory.Exists(textBox4.Text.Trim()))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(textBox4.Text.Trim()); //新建文件夹   
                }
                catch
                {
                    MessageBox.Show("输出文件夹路径有误！");
                    return;
                }
            }

            //oriFolderPath = textBox1.Text.Trim();
            //oriFolderPath = oriFolderPath.TrimEnd('/').TrimEnd('\\');
            timeFolderPath = textBox1.Text.Trim();
            timeFolderPath = timeFolderPath.TrimEnd('/').TrimEnd('\\');

            uFolderPath= textBox2.Text.Trim();
            uFolderPath= uFolderPath.TrimEnd('/').TrimEnd('\\');

            vFolderPath = textBox3.Text.Trim();
            vFolderPath = vFolderPath.TrimEnd('/').TrimEnd('\\');

            outFolderPath = textBox4.Text.Trim();
            outFolderPath = outFolderPath.TrimEnd('/').TrimEnd('\\');
            //maxThreadCount = Convert.ToInt32(textBox5.Text);
            //ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
            int threadCount= Convert.ToInt32(textBox5.Text);
            threads = new Thread[threadCount];
            progressBar1.Show();
            worker.RunWorkerAsync();
        }

        private void selectButton1_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择时间数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void selectButton3_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择v数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox3.Text = fbd.SelectedPath;
            }
        }
    }
}
