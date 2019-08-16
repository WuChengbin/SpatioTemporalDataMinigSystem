using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MarineSTMiningSystem
{
    public partial class MarineHeatwavesTimeAnomalyForm : Form
    {
        string inFolderPath = string.Empty;//实际温度
        string normalFolderPath = string.Empty;//正常温度
        string outFolderPath = string.Empty;//输出
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public MarineHeatwavesTimeAnomalyForm()
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
            int startYear = 1981;
            int endYear = 2018;
            string[] normalFiles = Directory.GetFiles(normalFolderPath, "*.tif");
            for(int fileId=0;fileId<normalFiles.Length;fileId++)
            {//每个距平图像
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / normalFiles.Length;
                worker.ReportProgress(progress);//记录进度

                //读取距平数据图像
                Dataset ds = Gdal.Open(normalFiles[fileId], Access.GA_ReadOnly);
                int colCount = ds.RasterXSize;//列数
                int rowCount = ds.RasterYSize;//行数
                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ds.GetGeoTransform(argout);//读取地理坐标信息
                string projection = ds.GetProjection();//坐标系
                string[] metadatas = ds.GetMetadata("");//获取元数据
                //int inFillValue = -9999;
                //int outFillValue = -9999;
                Band demband = ds.GetRasterBand(1);//读取波段
                int[] normalData = new int[colCount*rowCount];//距平数据
                demband.ReadRaster(0, 0, colCount, rowCount, normalData, colCount, rowCount, 0, 0);//读取数据
                demband.Dispose();
                ds.Dispose();

                string timeStr = Path.GetFileNameWithoutExtension(normalFiles[fileId]);//无后缀文件名
                for(int year=startYear;year<=endYear;year++)
                {//距平每一年图像
                    string inFilePath = inFolderPath + "\\" + year.ToString() + timeStr + ".tif";
                    if(File.Exists(inFilePath))
                    {//存在该文件
                        ds = Gdal.Open(inFilePath, Access.GA_ReadOnly);
                        demband = ds.GetRasterBand(1);
                        int[] inData = new int[colCount * rowCount];//输入数据
                        demband.ReadRaster(0, 0, colCount, rowCount, inData, colCount, rowCount, 0, 0);//读取数据
                        for(int i=0;i<inData.Length;i++)
                        {//每个栅格
                            //if(inData[i]==inFillValue)
                            //{//无值
                            //    inData[i] = outFillValue;
                            //}
                            //else
                            //{//有值
                                inData[i] -= normalData[i];
                           // }
                        }

                        //保存距平图像
                        string outPath = outFolderPath + "\\" + year.ToString() + timeStr + ".tif";
                        Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                        Dataset gTiffDataset = gTiffRriver.Create(outPath, colCount, rowCount, 1, DataType.GDT_Int16, null);
                        gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                        gTiffDataset.SetProjection(projection);//设置坐标系
                        gTiffDataset.SetMetadataItem("StartLog", "0", null);
                        gTiffDataset.SetMetadataItem("EndLog", "360", null);
                        gTiffDataset.SetMetadataItem("StartLat", "-90", null);
                        gTiffDataset.SetMetadataItem("EndLat", "90", null);
                        gTiffDataset.SetMetadataItem("Scale", "0.01", null);
                        gTiffDataset.SetMetadataItem("FillValue", "-9999", null);
                        gTiffDataset.SetMetadataItem("Resolution", "0.25", null);
                        gTiffDataset.SetMetadataItem("Rows", rowCount.ToString(), null);
                        gTiffDataset.SetMetadataItem("Cols", colCount.ToString(), null);
                        gTiffDataset.SetMetadataItem("Offsets", "0", null);
                        gTiffDataset.SetMetadataItem("Units", "degrees C", null);
                        gTiffDataset.SetMetadataItem("MinValue", "-5000", null);
                        gTiffDataset.SetMetadataItem("MaxValue", "5000", null);
                        gTiffDataset.WriteRaster(0, 0, colCount, rowCount, inData, colCount, rowCount, 1, null, 0, 0, 0);
                        gTiffDataset.Dispose();
                    }
                }
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

        private void selectButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输入文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                inPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void selectButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择距平文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                normalTextBox.Text = fbd.SelectedPath;
            }
        }

        private void selectButton3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                outPathTextBox.Text = fbd.SelectedPath;
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (!System.IO.Directory.Exists(inPathTextBox.Text.Trim()))
            {
                MessageBox.Show("输入文件夹路径有误！");
                return;
            }
            if (!System.IO.Directory.Exists(normalTextBox.Text.Trim()))
            {
                MessageBox.Show("输入文件夹路径有误！");
                return;
            }
            if (!System.IO.Directory.Exists(outPathTextBox.Text.Trim()))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(outPathTextBox.Text.Trim()); //新建文件夹   
                }
                catch
                {
                    MessageBox.Show("输出文件夹路径有误！");
                    return;
                }
            }

            inFolderPath = inPathTextBox.Text.Trim();
            inFolderPath =inFolderPath.TrimEnd('/').TrimEnd('\\');
            normalFolderPath = normalTextBox.Text.Trim();
            normalFolderPath = normalFolderPath.TrimEnd('/').TrimEnd('\\');
            outFolderPath = outPathTextBox.Text.Trim();
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
