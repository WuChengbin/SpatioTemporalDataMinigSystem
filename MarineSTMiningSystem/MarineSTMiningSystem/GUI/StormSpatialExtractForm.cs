using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSGeo.GDAL;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace MarineSTMiningSystem
{
    public partial class StormSpatialExtractForm : Form
    {
        int threadCount = 1;//线程数
        Thread[] threads;//线程
        public StormSpatialExtractForm()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private BackgroundWorker worker = new BackgroundWorker();//后台线程


        int col;//列数
        int row;//行数
        double[] argout; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
        string projection;//坐标系
        double startLog = 0.0;//起始经度
        double startLat = 0.0;//起始维度
        double endLog = 0.0;//结束经度
        double endLat = 0.0;//结束维度
        double mScale = 0.0;//比例
        string dataType = "";
        double resolution = 0.0;//分辨率
        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            //string[] filesName = Directory.GetFiles(inFolderPath);//所有文件名
            int mFileNum = inFileNames.Count;//文件个数
            if (mFileNum == 0)
            {
                MessageBox.Show("目录下不存在文件！");
                return;
            }
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
            string fileName = inFileNames[0].ToString();
            //打开hdf文件
            Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
            col = ds.RasterXSize;//列数
            row = ds.RasterYSize;//行数
            Band demband1 = ds.GetRasterBand(1);//读取波段

            argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息
            
            projection = ds.GetProjection();//坐标系

            string[] metadatas = ds.GetMetadata("");//获取元数据
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
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }
            ds.Dispose();

            for (int i = 0; i < mFileNum; i++)
            {//处理每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (i * 100) / mFileNum;
                worker.ReportProgress(progress);//记录进度

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(SpaceWorke));
                threads[tId].IsBackground = true;
                threads[tId].Start(i);

                //progressBar.Value = (i * 100) / mFileNum;
                
            }

            while (true)
            {//判断线程是否执行结束
                bool isEnd = true;
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped)
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

        private void SpaceWorke(object obj)
        {
            int i = (int)obj;
            int[] file = new int[row * col];//用来存储一个图像
            int[] fileResult = new int[row * col];//用来存储结果 
            string fileName = inFileNames[i].ToString();
            //打开hdf文件
            Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
            Band demband = ds.GetRasterBand(1);//读取波段
            demband.ReadRaster(0, 0, col, row, file, col, row, 0, 0);//读取数据
            List<int> visitedPosList = new List<int>();//记录已经访问的位置
            ds.Dispose();
            for (int j = 0; j < col * row; j++)
            {//处理每个栅格
                int rowNow = j / col;//当前行号
                int colNow = j % col;//当前列号
                if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) continue;//边界点，不搜索
                if (type == 1)
                {//
                    if (file[j] > 0 && file[j] <= 3)
                    {//暴雨
                        if (!visitedPosList.Contains(j))
                        {//没有被访问
                            List<int> storm = new List<int>();//一个暴雨
                            int[] stormLevel = new int[3];//记录暴雨级别个数
                            getStorm(file, j, row, col, ref visitedPosList, ref storm, ref stormLevel);
                            int stormMode = 1;//暴雨众数
                            if ((stormLevel[1] + stormLevel[2]) >= stormLevel[0])
                            {
                                if (stormLevel[1] > stormLevel[2])
                                {//大暴雨
                                    stormMode = 2;
                                }
                                else
                                {//特大暴雨
                                    stormMode = 3;
                                }
                            }
                            for (int k = 0; k < storm.Count; k++)
                            {//给暴雨赋值
                                fileResult[storm[k]] = stormMode;
                            }
                        }
                    }
                    else if (file[j] == 255 || file[j] == -9999)
                    {
                        fileResult[j] = -9999;
                    }
                }
                else if (type == 2)
                {//降雨
                    if (file[j] > 0 && file[j] <= 4)
                    {//暴雨
                        if (!visitedPosList.Contains(j))
                        {//没有被访问
                            List<int> rain = new List<int>();//一个暴雨
                            int[] rainLevel = new int[4];//记录暴雨级别个数
                            List<int> rainPos = new List<int>();//记录邻域内未访问暴雨点
                            rainPos.Add(j);
                            visitedPosList.Add(j);//记录被访问
                            do
                            {
                                List<int> moreRainPos = getRain(file, rainPos[0], row, col, ref visitedPosList, ref rain, ref rainLevel);//获得更多暴雨点并进行相应处理
                                rainPos.RemoveAt(0);//移除第一个已经处理的
                                rainPos.AddRange(moreRainPos);
                            } while (rainPos.Count > 0);
                            int stormMode = 1;//暴雨众数
                            if (rainLevel[1] + rainLevel[2] + rainLevel[3] >= rainLevel[0])
                            {//中雨及以上
                                if (rainLevel[2] + rainLevel[3] >= rainLevel[1])
                                {//大雨及以上
                                    if (rainLevel[3] > rainLevel[2]) stormMode = 4;//暴雨
                                    else stormMode = 3;//大雨
                                }
                                else stormMode = 2;//中雨
                            }
                            for (int k = 0; k < rain.Count; k++)
                            {//给暴雨赋值
                                fileResult[rain[k]] = stormMode;
                            }
                        }
                    }
                    else if (file[j] == 255 || file[j] == -9999)
                    {
                        fileResult[j] = -9999;
                    }
                }
            }

            string fileNameWithPath = Path.GetFileName(fileName);//获取文件名
            int pointIndex = fileNameWithPath.LastIndexOf('.');//最后一个.的位置
            string outfileName = fileNameWithPath.Substring(0, pointIndex) + "_Spatial";

            //保存为tif格式
            string outPath = outFolderPath + "\\" + outfileName + ".tif";
            Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
            Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
            gTiffDataset.SetGeoTransform(argout);//地理坐标信息
            gTiffDataset.SetProjection(projection);//设置坐标系
            gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
            gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
            gTiffDataset.SetMetadataItem("startLat", startLat.ToString(), null);
            gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
            gTiffDataset.SetMetadataItem("Scale", "1", null);
            gTiffDataset.SetMetadataItem("FillValue", "-9999", null);
            gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
            gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
            gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
            gTiffDataset.SetMetadataItem("Offsets", "0", null);
            gTiffDataset.SetMetadataItem("MaxValue", "3", null);
            gTiffDataset.SetMetadataItem("MinValue", "0", null);           
            gTiffDataset.WriteRaster(0, 0, col, row, fileResult, col, row, 1, null, 0, 0, 0);
            gTiffDataset.Dispose();
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (e.Cancelled)
            {//用户取消
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                }
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {//异常结束
                foreach (Thread t in threads)
                {
                    if (t.ThreadState != ThreadState.Stopped) t.Abort();
                }
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

        ListBox.ObjectCollection inFileNames;
        string outFolderPath;
        int type;//处理类型
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

            if (stormBtm.Checked == true) type = 1;//暴雨处理
            else if (rainBtn.Checked == true) type = 2;//降雨处理

            inFileNames = listBox1.Items;
            outFolderPath = textBox1.Text.Trim();
            threads = new Thread[threadCount];//线程数
            progressBar1.Show();
            worker.RunWorkerAsync();

            //progressBar.Hide();//进度条显示
            //MessageBox.Show("处理完成!");
        }

        private void getStorm(int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> storm, ref int[] stormLevel)
        {
            visitedPosList.Add(gridId);//记录被访问
            storm.Add(gridId);//添加到暴雨
            if (file[gridId] == 1)
            {//暴雨
                stormLevel[0]++;
            }
            else if (file[gridId] == 2)
            {//大暴雨
                stormLevel[1]++;
            }
            else if (file[gridId] == 3)
            {//特大暴雨
                stormLevel[2]++;
            }
            int rowNow = gridId / col;//当前行号
            int colNow = gridId % col;//当前列号
            if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) return;//边界点，不搜索邻域
            int[] nearPos = { gridId - col - 1, gridId - col, gridId - col + 1, gridId - 1, gridId + 1, gridId + col - 1, gridId + col, gridId + col + 1 };//8邻域位置
            //int[,] nearPos = { { rowNow - 1, colNow - 1 }, { rowNow - 1, colNow }, { rowNow - 1, colNow + 1 }, { rowNow, colNow - 1 }, { rowNow, col + 1 }, { rowNow + 1, col - 1 }, { rowNow + 1, colNow }, { rowNow + 1, colNow + 1 } };//8邻域位置
            for (int i = 0; i < nearPos.Length; i++)
            {
                if (nearPos[i] >= 0 && nearPos[i] < file.Length && file[nearPos[i]] > 0 && file[nearPos[i]] <=3)
                {//在图像范围内,且是暴雨
                    bool isVisited = visitedPosList.Contains(nearPos[i]);//该位置是否被访问
                    if (!isVisited)
                    {//没有被访问
                        getStorm(file, nearPos[i], row, col, ref visitedPosList, ref storm, ref stormLevel);
                    }
                }
            }
        }

        private List<int>  getRain(int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> rain, ref int[] rainLevel)
        {
            
            int rowNow = gridId / col;//当前行号
            int colNow = gridId % col;//当前列号
            List<int> nearPosList = new List<int>();//没有被访问的降雨栅格
            if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) return nearPosList;//边界点，不搜索邻域
            int[] nearPos = { gridId - col - 1, gridId - col, gridId - col + 1, gridId - 1, gridId + 1, gridId + col - 1, gridId + col, gridId + col + 1 };//8邻域位置
            //int[,] nearPos = { { rowNow - 1, colNow - 1 }, { rowNow - 1, colNow }, { rowNow - 1, colNow + 1 }, { rowNow, colNow - 1 }, { rowNow, col + 1 }, { rowNow + 1, col - 1 }, { rowNow + 1, colNow }, { rowNow + 1, colNow + 1 } };//8邻域位置
            for (int i = 0; i < nearPos.Length; i++)
            {
                if (nearPos[i] >= 0 && nearPos[i] < file.Length && file[nearPos[i]] > 0 && file[nearPos[i]] <=4)
                {//在图像范围内,且是暴雨
                    bool isVisited = visitedPosList.Contains(nearPos[i]);//该位置是否被访问
                    if (!isVisited)
                    {//没有被访问
                        visitedPosList.Add(nearPos[i]);
                        nearPosList.Add(nearPos[i]);
                        //getRain(file, nearPos[i], row, col, ref visitedPosList, ref rain, ref rainLevel);visitedPosList.Add(gridId);//记录被访问
                        rain.Add(nearPos[i]);//添加到暴雨
                        if (file[nearPos[i]] == 1)
                        {//暴雨
                            rainLevel[0]++;
                        }
                        else if (file[nearPos[i]] == 2)
                        {//大暴雨
                            rainLevel[1]++;
                        }
                        else if (file[nearPos[i]] == 3)
                        {//特大暴雨
                            rainLevel[2]++;
                        }
                        else if (file[nearPos[i]] == 4)
                        {//特大暴雨
                            rainLevel[3]++;
                        }
                    }
                }
            }
            return nearPosList;
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
