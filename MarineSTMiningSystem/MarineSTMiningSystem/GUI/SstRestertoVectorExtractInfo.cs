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

namespace MarineSTMiningSystem.GUI
{
    public partial class SstRestertoVectorExtractInfo : Form
    {
        int timeCell = 1;//时间间隔（天）
        string[] timeFilePaths;
        string uFolderPath = string.Empty;//流场纬线方向速度
        string vFolderPath = string.Empty;//流场经线方向速度
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        int maxThreadCount = 1;
        Thread[] threads;

        public SstRestertoVectorExtractInfo()
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

        struct ThreadInfo
        {
            public string timeFilePath;
            public string outFilePath;
            public string uFilePath;
            public string vFilePath;
            public int timeCell;
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            //string[] timeFilePaths = Directory.GetFiles(timeFolderPath, "*.tif");
            //string[] uFilePaths = Directory.GetFiles(uFolderPath, "*.tif");
            //string[] vFilePaths = Directory.GetFiles(vFolderPath, "*.tif");
            int fileCount = timeFilePaths.Length;
            //int activeThreads = 0;
            for (int fileId = 0; fileId < fileCount; fileId++)
            {//每一天文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / fileCount;
                worker.ReportProgress(progress);//记录进度

                string timeFileName = Path.GetFileNameWithoutExtension(timeFilePaths[fileId]).Substring(0, 6);
                //string oriFilePath = oriFolderPath + "\\" + timeFileName;//原始图像路径
                string outFilePath = outFolderPath + "\\" + timeFileName + ".shp";//输出图像路径

                //查找时间最近的uv数据
                //DateTime dt = DateTime.ParseExact(timeFileName, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                DateTime dt = DateTime.ParseExact(timeFileName, "yyyyMM", System.Globalization.CultureInfo.CurrentCulture);
                string uFilePath = uFolderPath + "\\" + timeFileName + ".tif";
                string vFilePath = vFolderPath + "\\" + timeFileName + ".tif";
                //int addDay = 1;
                int addmonth = 1;
                while (!File.Exists(uFilePath))
                {
                    //uFilePath = uFolderPath + "\\" + dt.AddDays(addDay).ToString("yyyyMMdd") + ".tif";
                    //addDay = -addDay;
                    //if (addDay > 0) addDay++;
                    uFilePath = uFolderPath + "\\" + dt.AddMonths(addmonth).ToString("yyyyMM") + ".tif";
                    addmonth = -addmonth;
                    if (addmonth > 0) addmonth++;
                }
                //addDay = 1;
                addmonth = 1;
                while (!File.Exists(vFilePath))
                {
                    //vFilePath = vFolderPath + "\\" + dt.AddDays(addDay).ToString("yyyyMMdd") + ".tif";
                    vFilePath = vFilePath + "\\" + dt.AddMonths(addmonth).ToString("yyyyMM") + ".tif";
                    //addDay = -addDay;
                    //if (addDay > 0) addDay++;
                    addmonth = -addmonth;
                    if (addmonth > 0) addmonth++;
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
            ResterToVector.SST_TifToShp(threadInfo.timeFilePath, threadInfo.uFilePath, threadInfo.vFilePath, threadInfo.outFilePath, threadInfo.timeCell);
        }

        #region 按钮Button
        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "tif文件|*.tif|所有文件|*.*";
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

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (listBox1.Items.Count <= 0)
            {
                MessageBox.Show("请添加时间数据！");
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
            //timeFolderPath = textBox1.Text.Trim();
            //timeFolderPath = timeFolderPath.TrimEnd('/').TrimEnd('\\');
            timeFilePaths = listBox1.Items.Cast<string>().ToArray();

            uFolderPath = textBox2.Text.Trim();
            uFolderPath = uFolderPath.TrimEnd('/').TrimEnd('\\');

            vFolderPath = textBox3.Text.Trim();
            vFolderPath = vFolderPath.TrimEnd('/').TrimEnd('\\');

            outFolderPath = textBox4.Text.Trim();
            outFolderPath = outFolderPath.TrimEnd('/').TrimEnd('\\');
            //maxThreadCount = Convert.ToInt32(textBox5.Text);
            //ThreadPool.SetMaxThreads(maxThreadCount, maxThreadCount);
            int threadCount = Convert.ToInt32(textBox5.Text);
            threads = new Thread[threadCount];
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
    }
}
