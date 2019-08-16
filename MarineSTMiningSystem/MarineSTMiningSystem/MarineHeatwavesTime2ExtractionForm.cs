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
    public partial class MarineHeatwavesTime2ExtractionForm : Form
    {
        ListBox.ObjectCollection inFileNames;
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        int timeCell = 1;//时间间隔（天）
        int maxTimeLength = 5;//最长时间间隔（天）
        public MarineHeatwavesTime2ExtractionForm()
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
            int fileCount = inFileNames.Count;//文件个数
            string fileName = inFileNames[0].ToString();
            Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
            int colCount = ds.RasterXSize;//列数
            int rowCount = ds.RasterYSize;//行数
            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息
            string projection = ds.GetProjection();//坐标系
            string[] metadatas = ds.GetMetadata("");//获取元数据
            ds.Dispose();

            List<short[]> imgs = new List<short[]>();
            for (int fileId = 0; fileId < fileCount; fileId++)
            {//读取每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / fileCount;
                worker.ReportProgress(progress);//记录进度

                ds = Gdal.Open(inFileNames[fileId].ToString(), Access.GA_ReadOnly);
                Band band = ds.GetRasterBand(1);
                short[] img = new short[colCount*rowCount];
                band.ReadRaster(0, 0, colCount, rowCount, img, colCount, rowCount, 0, 0);
                band.Dispose();
                ds.Dispose();

                //保存
                imgs.Add(img);
            }

            for(int id=0;id<(rowCount*colCount);id++)
            {//每个栅格
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (id * 100) / (rowCount * colCount);
                worker.ReportProgress(progress);//记录进度

                int startId = 0;//记录开始时刻
                int endId = 0;//记录结束时刻
                for (int fileId = 1; fileId < fileCount; fileId++)
                {//每个对应时刻
                    if (imgs[fileId-1][id] == 0 && imgs[fileId][id] > 0)
                    {//开始时刻
                        startId = fileId;//记录开始时刻
                    }
                    if (imgs[fileId - 1][id] > 0 && imgs[fileId][id] == 0)
                    {//结束时刻
                        endId = fileId;//记录结束时刻
                        if ((endId - startId) * timeCell < maxTimeLength)
                        {
                            for(int i=startId;i<endId;i++)
                            {//清楚持续时间过短的
                                imgs[i][id] = 0;
                            }
                        }
                    }
                }
                if(imgs[fileCount - 1][id]>0)
                {//最后时刻结束
                    if ((fileCount - startId) * timeCell < maxTimeLength)
                    {
                        for (int i = startId; i < fileCount; i++)
                        {//清楚持续时间过短的
                            imgs[i][id] = 0;
                        }
                    }
                }
            }

            //保存结果
            for (int fileId = 0; fileId < fileCount; fileId++)
            {//每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / (fileCount);
                worker.ReportProgress(progress);//记录进度
                
                string outPath = outFolderPath + "\\" + Path.GetFileNameWithoutExtension(inFileNames[fileId].ToString())+ ".tif";
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, colCount, rowCount, 1, DataType.GDT_Int16, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                gTiffDataset.SetProjection(projection);//设置坐标系
                for (int i = 0; i < metadatas.Length; i++)
                {
                    string[] metadata = metadatas[i].Split('=');
                    gTiffDataset.SetMetadataItem(metadata[0], metadata[1], null);
                }
                gTiffDataset.WriteRaster(0, 0, colCount, rowCount, imgs[fileId], colCount, rowCount, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            GC.Collect();//强制垃圾回收
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
                if (endIndex < (listBox1.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(startIndex, listBox1.Items[endIndex + 1].ToString());
                    listBox1.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox1.SelectedIndices;
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

            inFileNames = listBox1.Items;
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
    }
}
