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
    public partial class StormNumberExtractForm : Form
    {
        string outFolderPath = "";
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public StormNumberExtractForm()
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
            //string[] filesName = Directory.GetFiles(inFolderPath);//所有文件名
            int mFileNum = inFileNames.Count;//文件个数
            if (mFileNum == 0)
            {
                MessageBox.Show("目录下不存在文件！");
                return;
            }
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
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

            string projection = ds.GetProjection();//坐标系

            //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLog = 0.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 0.0;//结束经度
            double endLat = 0.0;//结束维度
            double mScale = 0.0;//比例
            string dataType = "";
            double resolution = 0.0;//分辨率
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

            int stormId = 1;//暴雨编号
            List<int[]> filesList = new List<int[]>();//记录所有输入图像
            List<int[]> filesResultList = new List<int[]>();//记录所有结果图像
            List<List<int>> fileIdList = new List<List<int>>();//记录所有图像的id
            int startId = 0;//记录保存图像的第一个id
            for (int i = 0; i < mFileNum; i++)
            {//处理每一个图像
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (i * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度
                //progressBar.Value = (i * 100) / mFileNum;
                //toolStripStatusLabel1.Text = "正在处理 " + i.ToString() + "/" + mFileNum.ToString();
                int[] file = new int[row * col];//用来存储一个图像
                hdfFileName = inFileNames[i].ToString();
                //打开hdf文件
                ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
                Band demband = ds.GetRasterBand(1);//读取波段
                demband.ReadRaster(0, 0, col, row, file, col, row, 0, 0);//读取数据
                filesList.Add(file);//记录下来
                ds.Dispose();
                if (i == 0)
                {//第一个图像
                    List<int> visitedPosList = new List<int>();//记录已经访问的位置
                    List<List<int>> stormList = new List<List<int>>();//记录一个图像所有暴雨
                    int[] fileResult = new int[row * col];
                    List<int> fileIds = new List<int>();//一个图像所有的id
                    for (int j = 0; j < file.Length; j++)
                    {//处理每个栅格
                        int rowNow = j / col;//当前行号
                        int colNow = j % col;//当前列号
                        if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) continue;//边界点，不搜索
                        if (file[j] > 0)
                        {//暴雨栅格
                            bool isVisited = visitedPosList.Contains(j);//该位置是否被访问
                            if (!isVisited)
                            {//没有被访问
                                List<int> storm = new List<int>();//一个暴雨
                                getStorm(file, j, row, col, ref visitedPosList, ref storm);
                                //stormList.Add(storm);//把暴雨记录下来
                                for (int k = 0; k < storm.Count; k++)
                                {//暴雨中的每个栅格
                                    fileResult[storm[k]] = stormId;
                                }
                                fileIds.Add(stormId);//记录图像中的id
                                stormId++;//暴雨id加一
                            }
                        }
                        else if (file[j] < 0)
                        {//应该只有缺失值
                            fileResult[j] = file[j];
                        }
                    }
                    fileIdList.Add(fileIds);//将一副图像的id保存起来
                    filesResultList.Add(fileResult);//将结果保存
                }
                else
                {//不是第一个图像
                    List<int> visitedPosList = new List<int>();//记录已经访问的位置
                    int[] fileResult = new int[row * col];
                    List<int> fileIds = new List<int>();//一个图像所有的id
                    for (int j = 0; j < file.Length; j++)
                    {//处理每个栅格
                        int rowNow = j / col;//当前行号
                        int colNow = j % col;//当前列号
                        if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) continue;//边界点，不搜索
                        if (file[j] > 0)
                        {//暴雨栅格
                            bool isVisited = visitedPosList.Contains(j);//该位置是否被访问
                            if (!isVisited)
                            {//没有被访问
                                List<int> usedId = new List<int>();//记录该暴雨在之前时刻的id
                                List<int> storm = new List<int>();//一个暴雨

                                getStorm(filesResultList, startId, i, file, j, row, col, ref visitedPosList, ref storm, ref usedId, timeCell, maxTimeInv);
                                if (usedId.Count > 0)
                                {
                                    for (int k = 0; k < storm.Count; k++)
                                    {//给暴雨赋id
                                        fileResult[storm[k]] = usedId[0];
                                        //if (usedId.Count > 1)
                                        //{//存在多个id
                                        //    for (int k1 = 0; k1 < filesResultList.Count; k1++)
                                        //    {//每个栅格
                                        //        if (usedId.Contains(filesResultList[k][k1]))
                                        //        {//被使用的id赋为相同的id
                                        //            filesResultList[k][k1] = usedId[0];
                                        //        }
                                        //    }
                                        //}
                                    }
                                    if (usedId.Count > 1)
                                    {//存在多个id
                                        List<int> reNameId = new List<int>(usedId);//重新命名的id
                                        reNameId.RemoveAt(0);//第一个不需要重新命名
                                        for (int k = 0; k < filesResultList.Count; k++)
                                        {//遍历之前的结果，赋第一个id
                                            bool needReName = false;//是否需要重新赋值
                                            for(int idPos=0;idPos< fileIdList[k].Count;idPos++)
                                            {//每个id
                                                if (reNameId.Contains(fileIdList[k][idPos]))
                                                {//被使用的id赋为相同的id
                                                    fileIdList[k][idPos] = usedId[0];
                                                    needReName = true;
                                                }
                                            }
                                            //fileIdList[k] = fileIdList[k].Distinct().ToList();//去重
                                            if(needReName)
                                            {//需要重新赋值
                                                for (int k1 = 0; k1 < filesResultList[k].Length; k1++)
                                                {//每个栅格
                                                    if (reNameId.Contains(filesResultList[k][k1]))
                                                    {//被使用的id赋为相同的id
                                                        filesResultList[k][k1] = usedId[0];
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    fileIds.Add(usedId[0]);
                                }
                                else
                                {//新的暴雨
                                    for (int k = 0; k < storm.Count; k++)
                                    {//给暴雨赋id
                                        fileResult[storm[k]] = stormId;
                                    }
                                    fileIds.Add(stormId);
                                    stormId++;
                                }
                            }
                        }
                        else if (file[j] < 0)
                        {//应该只有缺失值
                            fileResult[j] = file[j];
                        }
                    }
                    fileIdList.Add(fileIds);//将一副图像的id保存起来
                    filesResultList.Add(fileResult);//将结果保存
                }
                for (int j = filesResultList.Count - 2-Convert.ToInt32(maxTimeInv/timeCell); j >= 0; j--)
                {//寻找已经不被使用的图像
                    bool isUsed = false;//默认没有被使用
                    foreach (int idValue in fileIdList[j])
                    {
                        for(int k= fileIdList.Count - 1;k> filesResultList.Count - 2 - Convert.ToInt32(maxTimeInv / timeCell);k--)
                        {//保留的每个图像
                            if (fileIdList[k].Contains(idValue))
                            {//还在被使用
                                isUsed = true;
                                break;
                            }
                        }
                        if (isUsed) break;//还在被使用
                    }
                    if (!isUsed)
                    {//该图像没有被使用
                        //MessageBox.Show(isUsed.ToString());
                        for (int k = j; k >= 0; k--)
                        {//输出本图像及之前的图像
                            string fileName = Path.GetFileName(inFileNames[startId + k].ToString());//获取文件名
                            int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
                            string outfileName = fileName.Substring(0, pointIndex) + "_ID";

                            //保存为Asc格式
                            //string outPath = outFolderPath + "\\" + outfileName + ".txt";
                            //FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                            //StreamWriter sw = new StreamWriter(fs);
                            //sw.WriteLine("ncols " + col);
                            //sw.WriteLine("nrows " + row);
                            //sw.WriteLine("xllcorner " + startLog);
                            //sw.WriteLine("yllcorner " + startLat);
                            //sw.WriteLine("cellsize " + "0.1");
                            //sw.WriteLine("NODATA_VALUE " + "-9999");
                            //sw.WriteLine("DataType " + "8");
                            //for (int k1 = 0; k1 < row; k1++)
                            //{
                            //    for (int k2 = 0; k2 < col; k2++)
                            //    {
                            //        sw.Write(filesResultList[k][k1 * col + k2].ToString() + " ");
                            //    }
                            //    sw.Write("\n");
                            //}
                            //sw.Close();
                            //fs.Close();

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
                            gTiffDataset.SetMetadataItem("MinValue", "0", null);
                            gTiffDataset.WriteRaster(0, 0, col, row, filesResultList[k], col, row, 1, null, 0, 0, 0);
                            gTiffDataset.Dispose();
                        }
                        filesList.RemoveRange(0, j + 1);
                        filesResultList.RemoveRange(0, j + 1);
                        fileIdList.RemoveRange(0, j + 1);
                        startId += j + 1;
                        break;
                    }
                }
            }

            for (int i = 0; i < filesResultList.Count; i++)
            {//输出本图像及之前的图像
                string fileName = Path.GetFileName(inFileNames[startId + i].ToString());//获取文件名
                int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
                string outfileName = fileName.Substring(0, pointIndex) + "_ID";

                //保存为Asc格式
                //string outPath = outFolderPath + "\\" + outfileName + ".txt";
                //FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                //StreamWriter sw = new StreamWriter(fs);
                //sw.WriteLine("ncols " + col);
                //sw.WriteLine("nrows " + row);
                //sw.WriteLine("xllcorner " + startLog);
                //sw.WriteLine("yllcorner " + startLat);
                //sw.WriteLine("cellsize " + "0.1");
                //sw.WriteLine("NODATA_VALUE " + "-9999");
                //sw.WriteLine("DataType " + "8");
                //for (int j = 0; j < row; j++)
                //{
                //    for (int k = 0; k < col; k++)
                //    {
                //        sw.Write(filesResultList[i][j * col + k].ToString() + " ");
                //    }
                //    sw.Write("\n");
                //}
                //sw.Close();
                //fs.Close();

                //保存为tif格式
                string outPath2 = outFolderPath + "\\" + outfileName + ".tif";
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath2, col, row, 1, DataType.GDT_Int32, null);
                //Dataset gTiffDataset = gTiffRriver.CreateCopy(outPath, ds, 0, null, null, null);
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
                gTiffDataset.SetMetadataItem("MaxValue", stormId.ToString(), null);
                gTiffDataset.SetMetadataItem("MinValue", "0", null);
                gTiffDataset.WriteRaster(0, 0, col, row, filesResultList[i], col, row, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
            //progressBar.Hide();
            //toolStripStatusLabel1.Text = "就绪";
            //MessageBox.Show("www");
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
        //double valueScale;
        double timeCell;
        double maxTimeInv;
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            inFileNames = listBox1.Items;
            outFolderPath = textBox1.Text;
            if (inFileNames.Count == 0)
            {
                MessageBox.Show("请添加处理文件！");
                return;
            }
            if (!System.IO.Directory.Exists(outFolderPath))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(outFolderPath); //新建文件夹   
                }
                catch
                {
                    MessageBox.Show("输出文件夹路径有误！");
                    return;
                }
            }

            //double valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离

            progressBar1.Show();
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// 编号提取过程中暴雨提取
        /// </summary>
        /// <param name="filesResultList"></param>
        /// <param name="startId"></param>
        /// <param name="fileIdNow"></param>
        /// <param name="file"></param>
        /// <param name="gridId"></param>
        /// <param name="col"></param>
        /// <param name="visitedPosList"></param>
        /// <param name="storm"></param>
        /// <param name="usedId"></param>
        private void getStorm(List<int[]> filesResultList, int startId, int fileIdNow, int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> storm, ref List<int> usedId,double timeCell,double maxTimeInv)
        {
            visitedPosList.Add(gridId);//记录被访问
            storm.Add(gridId);//添加到暴雨

            for (int i = 1; i <= DoubleRvide(maxTimeInv,timeCell); i++)
            {//向前搜索3小时
                if (fileIdNow - startId - i >= 0)
                {//范围内
                    int idValue = filesResultList[fileIdNow - startId - i][gridId];//在编号结果图像中的值
                    if (idValue > 0)
                    {//是暴雨
                        if (!usedId.Contains(idValue))
                        {//没有被记录的话记录下来
                            usedId.Add(idValue);
                        }
                    }
                }
                else { break; }//超出范围，终止
            }
            int rowNow = gridId / col;//当前行号
            int colNow = gridId % col;//当前列号
            if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) return;//边界点，不搜索邻域
            int[] nearPos = { gridId - col - 1, gridId - col, gridId - col + 1, gridId - 1, gridId + 1, gridId + col - 1, gridId + col, gridId + col + 1 };//8邻域位置
            for (int i = 0; i < nearPos.Length; i++)
            {
                if (nearPos[i] >= 0 && nearPos[i] < file.Length && file[nearPos[i]] > 0)
                {//在图像范围内,且是暴雨
                    bool isVisited = visitedPosList.Contains(nearPos[i]);//该位置是否被访问
                    if (!isVisited)
                    {//没有被访问
                        //getStorm(file, nearPos[i], col, ref visitedPosList, ref storm);
                        getStorm(filesResultList, startId, fileIdNow, file, nearPos[i], row, col, ref visitedPosList, ref storm, ref usedId,timeCell,maxTimeInv);
                    }
                }
            }
        }

        //寻找属于一个暴雨的所有点
        private void getStorm(int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> storm)
        {
            visitedPosList.Add(gridId);//记录被访问
            storm.Add(gridId);//添加到暴雨
            int rowNow = gridId / col;//当前行号
            int colNow = gridId % col;//当前列号
            if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) return;//边界点，不搜索邻域
            int[] nearPos = { gridId - col - 1, gridId - col, gridId - col + 1, gridId - 1, gridId + 1, gridId + col - 1, gridId + col, gridId + col + 1 };//8邻域位置
            for (int i = 0; i < nearPos.Length; i++)
            {
                if (nearPos[i] >= 0 && nearPos[i] < file.Length && file[nearPos[i]] > 0)
                {//在图像范围内,且是暴雨
                    bool isVisited = visitedPosList.Contains(nearPos[i]);//该位置是否被访问
                    if (!isVisited)
                    {//没有被访问
                        getStorm(file, nearPos[i], row, col, ref visitedPosList, ref storm);
                    }
                }
            }
        }



        private void cancelBtn_Click(object sender, EventArgs e)
        {
            //this.Close();
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

        /// <summary>
        /// 解决除不尽的情况
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>结果</returns>
        public static double DoubleRvide(double a, double b)
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
    }
}
