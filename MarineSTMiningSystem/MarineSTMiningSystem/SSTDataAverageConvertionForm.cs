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
    public partial class SSTDataAverageConvertionForm : Form
    {
        ListBox.ObjectCollection inFileNames;
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public SSTDataAverageConvertionForm()
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
            DateTime time2018 = new DateTime(2018, 1, 1);
            int mFileNum = inFileNames.Count;//文件个数
            for (int fileTd = 0; fileTd < mFileNum; fileTd++)
            {//处理每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileTd * 100) / mFileNum;
                worker.ReportProgress(progress);//记录进度

                string hdfFileName = inFileNames[fileTd].ToString();
                Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
                int col = ds.RasterXSize;//列数
                int row = ds.RasterYSize;//行数
                Band demband1 = ds.GetRasterBand(1);//读取波段
                //string[] metadatas = ds.GetMetadata("");//获取元数据
                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ds.GetGeoTransform(argout);//读取地理坐标信息
                double startLog = 0.0;//起始经度
                double startLat = -90.0;//起始维度
                double endLog = 360.0;//结束经度
                double endLat = 90.0;//结束维度
                double mScale = 0.01;//比例
                double resolution = 0.25;//每像素宽度
                argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };
                int[] databuf = new int[col * row];//存储
                demband1.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据
                demband1.Dispose();
                ds.Dispose();

                double[] databufCovert = new double[col * row];
                for (int i = 0; i < databufCovert.Length; i++)
                {
                    int _row = i / col;//当前行号
                    int _col = i % col;//当前列号
                    _row = row - _row - 1;//行号转换
                    databufCovert[i] = databuf[_row * col + _col];//重新赋值
                }

                DateTime time = time2018.AddDays(fileTd);
                //string fileName = Path.GetFileNameWithoutExtension(hdfFileName);
                //string timeStr = fileName.Substring(14, 8);//截取日期字符串
                string outPath = outFolderPath + "\\" + time.ToString("MMdd") + ".tif";

                OSGeo.GDAL.Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int16, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                string projection = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartYear", "1971", null);
                gTiffDataset.SetMetadataItem("EndYear", "2000", null);
                gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
                gTiffDataset.SetMetadataItem("StartLat", startLat.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
                gTiffDataset.SetMetadataItem("Scale", mScale.ToString(), null);
                gTiffDataset.SetMetadataItem("FillValue", "-999", null);
                gTiffDataset.SetMetadataItem("Resolution", resolution.ToString(), null);
                gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
                gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
                gTiffDataset.SetMetadataItem("Offsets", "0", null);
                gTiffDataset.SetMetadataItem("Units", "degrees C", null);
                gTiffDataset.SetMetadataItem("MinValue", "-300", null);
                gTiffDataset.SetMetadataItem("MaxValue", "4500", null);
                gTiffDataset.WriteRaster(0, 0, col, row, databufCovert, col, row, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
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
            outFolderPath.TrimEnd('/').TrimEnd('\\');
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
