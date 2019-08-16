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
    public partial class TifToShpBasedSpaceForm : Form
    {
        string outFolderPath = "";
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        Thread[] threads;//处理线程
        public TifToShpBasedSpaceForm()
        {
            InitializeComponent(); worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            for (int i = 0; i < spFileNames.Count; i++)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = i * 100 / spFileNames.Count;//进度
                worker.ReportProgress(progress);//记录进度
                string oriPath = oriFileNames[i].ToString();//原始图像路径
                string spPath = spFileNames[i].ToString();//空间图像路径
                string fileName = Path.GetFileNameWithoutExtension(spPath);//文件名没有后缀
                string outPath = outFolderPath + fileName + ".shp";

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(TifToShp));
                threads[tId].IsBackground = true;
                Parameter p = new Parameter(oriPath, valueScale, timeCell, spPath, outPath);
                threads[tId].Start(p);

                //ResterToVector.TifToShp(oriPath, valueScale,timeCell, spPath, idPath, outPath);
                //ResterToVector.TifToShp(spPath, outPath);
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

        struct Parameter
        {//参数结构体
            public string _oriPath;
            public double _valueScale;
            public double _timeCell;
            public string _spPath;
            public string _outPath;

            public Parameter(string _oriPath, double _valueScale, double _timeCell, string _spPath, string _outPath) : this()
            {
                this._oriPath = _oriPath;
                this._valueScale = _valueScale;
                this._timeCell = _timeCell;
                this._spPath = _spPath;
                this._outPath = _outPath;
            }
        }

        private void TifToShp(object obj)
        {
            Parameter p = (Parameter)obj;
            ResterToVector.TifToShp(p._oriPath, p._valueScale, p._timeCell, p._spPath, p._outPath);
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
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                }
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
            TimeSpan costTime = DateTime.Now - pStartTime;//已经花费时间

        }

        private void oriAddFileBtn_Click(object sender, EventArgs e)
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
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }


        ListBox.ObjectCollection oriFileNames;
        ListBox.ObjectCollection spFileNames;
        double valueScale;
        double timeCell;
        //double maxTimeInv;
        DateTime pStartTime;//程序执行开始时间
        int threadCount = 1;//线程个数
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
            if (listBox1.Items.Count != listBox3.Items.Count)
            {//数目不同
                MessageBox.Show("图像数目不完全相同！");
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

            outFolderPath = textBox1.Text.Trim();
            if (!outFolderPath.EndsWith("\\")) outFolderPath += "\\";
            oriFileNames = listBox3.Items;
            spFileNames = listBox1.Items;
            valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            //timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            //maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离
            threadCount = Convert.ToInt32(threadTextBox.Text.Trim());
            threads = new Thread[threadCount];
            //okBtn.Enabled = false;
            pStartTime = DateTime.Now;//记录程序开始时间
            progressBar1.Show();
            worker.RunWorkerAsync();
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
