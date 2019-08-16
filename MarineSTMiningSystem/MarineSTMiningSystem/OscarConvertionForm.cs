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
    public partial class OscarConvertionForm : Form
    {
        ListBox.ObjectCollection inFileNames;
        string outFolderPath = string.Empty;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        List<string> outFileNames=new List<string>();
        public OscarConvertionForm()
        {
            InitializeComponent();

            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            StreamReader sr = new StreamReader(@"E:\OSCAR\fileNames.txt");
            string line = sr.ReadLine();
            while(line!=null)
            {
                outFileNames.Add(line);
                line = sr.ReadLine();
            }
            sr.Close();
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
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

                string fileName = inFileNames[fileTd].ToString();
                Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                int colCount = ds.RasterXSize;//列数
                int rowCount = ds.RasterYSize;//行数
                Band demband1 = ds.GetRasterBand(1);//读取波段
                int[] databuf = new int[colCount * rowCount];//存储
                demband1.ReadRaster(0, 0, colCount, rowCount, databuf, colCount, rowCount, 0, 0);//读取数据
                demband1.Dispose();
                ds.Dispose();

                int fillValue = -9999;
                int colCountResult = 1440;
                int rowCountResult = 720;
                double startLat = -80.1667;
                double endLat = 80.1667;
                int[] dataResult = new int[colCountResult * rowCountResult];//存储结果
                for (int rowId=0;rowId<rowCountResult;rowId++)
                {
                    double coreLat = (90 - 0.125) - rowId * 0.25;//栅格中心维度
                    for(int colId=0;colId<colCountResult;colId++)
                    {
                        int dataId = rowId * colCountResult + colId;
                        double coreLog = 0.125 + colId * 0.25;
                        if(coreLat< startLat|| coreLat> endLat)
                        {//没有值的区域
                            dataResult[dataId] = fillValue;
                        }
                        else
                        {
                            if(coreLog < 19.8333)
                            {
                                coreLog += 360;
                            }
                            int rowId2 = Convert.ToInt32(Math.Floor((80.1667 - coreLat) / (1.0 / 3.0)));
                            int colId2 = Convert.ToInt32(Math.Floor((coreLog - 19.8333) / (1.0 / 3.0)));
                            int dataId2 = rowId2 * colCount + colId2;
                            dataResult[dataId] = databuf[dataId2];
                        }
                    }
                }

                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ////ds.GetGeoTransform(argout);//读取地理坐标信息
                //double startLog = 0.0;//起始经度
                //double startLat = -90.0;//起始维度
                //double endLog = 360.0;//结束经度
                //double endLat = 90.0;//结束维度
                //double mScale = 0.01;//比例
                //double resolution = 0.25;//每像素宽度
                //argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };
                argout = new double[] { 0.0, 0.25, 0, 90.0, 0, -0.25 };
                //DateTime date1 = new DateTime(2017, 1, 1);
                //int dateInterval = fileTd * 5;
                //DateTime dateNow = date1.AddDays(dateInterval);
                //string timeStr = dateNow.ToString("yyyyMMdd");//截取日期字符串
                string timeStr = outFileNames[fileTd];
                string outPath = outFolderPath + "\\" + timeStr + ".tif";

                OSGeo.GDAL.Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, colCountResult, rowCountResult, 1, DataType.GDT_Int16, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                string projection = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartLog", "0", null);
                gTiffDataset.SetMetadataItem("EndLog", "360", null);
                gTiffDataset.SetMetadataItem("StartLat", "-90", null);
                gTiffDataset.SetMetadataItem("EndLat", "90", null);
                gTiffDataset.SetMetadataItem("Scale", "0.001", null);
                gTiffDataset.SetMetadataItem("FillValue", "-9999", null);
                gTiffDataset.SetMetadataItem("Resolution", "0.25", null);
                gTiffDataset.SetMetadataItem("Rows", rowCountResult.ToString(), null);
                gTiffDataset.SetMetadataItem("Cols", colCountResult.ToString(), null);
                gTiffDataset.SetMetadataItem("Offsets", "0", null);
                gTiffDataset.SetMetadataItem("Units", "m/s", null);
                gTiffDataset.WriteRaster(0, 0, colCountResult, rowCountResult, dataResult, colCountResult, rowCountResult, 1, null, 0, 0, 0);
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
    }
}
