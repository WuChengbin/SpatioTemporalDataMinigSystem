using MarineSTMiningSystem.Class;
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
    public partial class SstTimeExtractForm : Form
    {
        int threadCount = 1;//线程数
        Thread[] threads;//线程
        string outFolderPath = "";
        int mFileNum = 0;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程

        public SstTimeExtractForm()
        {
            InitializeComponent();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消

            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        struct Param
        {
            public int startRow;
            public int endRow;
            public int startCol;
            public int endCol;
        }

        int col = 0;//列数
        int row = 0;//行数
        string[] metadatas;//获取元数据
        double startLog;//起始经度
        double startLat;//起始维度
        double endLog;//结束经度
        double endLat;//结束维度
        double mScale;//比例
        string dataType = "";
        double resolution = 0.0;//分辨率
        double[] argout;
        string projection;
        List<int[]> byteArrList_positive = new List<int[]>();//存储结果的时空立方体
        List<int[]> byteArrList_negative = new List<int[]>();//存储结果的时空立方体

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            //string[] files = Directory.GetFiles(inFolderPath);//所有文件名
            mFileNum = inFileNames.Count;//文件个数
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            string hdfFileName = inFileNames[0].ToString();
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);

            col = ds.RasterXSize;//列数
            row = ds.RasterYSize;//行数
            Band demband1 = ds.GetRasterBand(1);//读取波段

            argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息
            //赋值地理坐标信息
            projection = ds.GetProjection();//坐标系
            //赋值坐标系
            projection = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";
            
            //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
            metadatas = ds.GetMetadata("");//获取元数据
            startLog = 0.0;//起始经度
            startLat = 0.0;//起始维度
            endLog = 0.0;//结束经度
            endLat = 0.0;//结束维度
            mScale = 0.0;//比例
            dataType = "";
            resolution = 0.0;//分辨率
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);
                        break;
                    case "DataType":
                        dataType = mdArr[1];
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }
            argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };

            ds.Dispose();
            for (int i = 0; i < mFileNum; i++)
            {
                byteArrList_positive.Add(new int[row * col]);
                byteArrList_negative.Add(new int[row * col]);
            }
            int startRow = 0;//当前起始行号
            int endRow = startRow + rowCut;//结束行号,不包括该行
            int startCol = 0;//起始列号
            int endCol = col;//结束列号,不包括该行
            int colCut = endCol - startCol;
            while (startRow < row)
            {//起始行号未超限
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (startRow * 100) / row;
                worker.ReportProgress(progress);//记录进度

                endRow = startRow + rowCut;//结束行号,不包括该行
                if (endRow > row)
                {
                    endRow = row;//结束行号超限
                    rowCut = endRow - startRow;
                }

                int tId = 0;//线程id
                while (threads[tId] != null && threads[tId].IsAlive == true)
                {//线程在执行
                    tId++;
                    if (tId >= threads.Length) tId = 0;
                }
                threads[tId] = new Thread(new ParameterizedThreadStart(TimeWorke));
                threads[tId].IsBackground = true;
                Param p = new Param();
                p.startRow = startRow; p.endRow = endRow;
                p.startCol = startCol; p.endCol = endCol;
                threads[tId].Start(p);

                startRow += rowCut;
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

            for (int i = 0; i < mFileNum; i++)
            {//保存
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
                threads[tId] = new Thread(new ParameterizedThreadStart(Store));
                threads[tId].IsBackground = true;
                threads[tId].Start(i);
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

        private static object objlock1 = new object();

        private int[] ReadAllRaster(ListBox.ObjectCollection FileNames,int startRow, int endRow, int startCol, int endCol)
        {
            int rowCut = endRow - startRow;
            int colCut = endCol - startCol;
            int[] pBuffer = new int[mFileNum * rowCut * colCut];//存储切割部分的时空立方体
            //读取所有文件切割部分
            for (int i = 0; i < mFileNum; i++)
            {
                //hdfFileName = oriFilesName[i];
                //打开文件
                Dataset ds = Gdal.Open(FileNames[i].ToString(), Access.GA_ReadOnly);
                Band demband = ds.GetRasterBand(1);//读取波段
                int[] databuf = new int[rowCut * colCut];//存储该切割部分的数组
                demband.ReadRaster(startCol, startRow, colCut, rowCut, databuf, colCut, rowCut, 0, 0);//读取数据
                for (int j = 0; j < rowCut * colCut; j++)
                {//进行存储
                    pBuffer[i * rowCut * colCut + j] = databuf[j];
                }
                ds.Dispose();
            }
            return pBuffer;
        }
        private void TimeWorke(object obj)
        {
            Param p = (Param)obj;
            int startRow = p.startRow;
            int endRow = p.endRow;
            int startCol = p.startCol;
            int endCol = p.endCol;
            int rowCut = endRow - startRow;
            int colCut = endCol - startCol;
            int[] pJuJiBufferCut_positive = new int[mFileNum * rowCut * colCut];//存储结果的时空立方体,正异常
            int[] pJuJiBufferCut_negative = new int[mFileNum * rowCut * colCut];//存储结果的时空立方体,负异常
            int[] pBuffer = ReadAllRaster(inFileNames,startRow, endRow, startCol, endCol);//读取所有时刻切割图像，距平数据
            //int[] pOriBuffer = ReadAllRaster(oriFileNames, startRow, endRow, startCol, endCol);//读取所有时刻切割图像,原始数据
            for (int rowNow = 0; rowNow < rowCut; rowNow++)
            {//按行循环
                for (int colNow = 0; colNow < colCut; colNow++)
                {//按列循环
                    double[] pTBuffer = new double[mFileNum];//一个栅格所有时间的降雨量，一维
                    for (int t = 0; t < mFileNum; t++)
                    {//t为时间
                     //int	k=row*c1+col;
                        if (pBuffer[t * rowCut * colCut + rowNow * colCut + colNow] != -9999)
                        {//非空值
                            pTBuffer[t] = ((double)(pBuffer[t * rowCut * colCut + rowNow * colCut + colNow]) * mScale) * valueScale;
                            //pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 0;//结果时空立方体切割前
                            pJuJiBufferCut_positive[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                            pJuJiBufferCut_negative[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                        }
                        else
                        {//空值
                            pTBuffer[t] = -9999;
                            //pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 255;
                            pJuJiBufferCut_positive[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                            pJuJiBufferCut_negative[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                        }
                    }
                    #region SST

                    double T_AvgValue = GeneralFun.GetMeanValue(pTBuffer);//默认带有-9999填充值
                    double T_StdValue = GeneralFun.GetStdValue(pTBuffer);//默认带有-9999填充值

                    double T_PlusThresholdValue = 0.5;//+
                    double T_MinusThresholdValue = -0.5;//-
                    #region 阈值
                    switch (thresholdIndex)
                    {
                        case 0:
                            {
                                T_PlusThresholdValue = T_AvgValue + 0.5 * T_StdValue;//+
                                T_MinusThresholdValue = T_AvgValue - 0.5 * T_StdValue;//-
                                break;
                            }
                        case 1:
                            {
                                T_PlusThresholdValue = T_AvgValue + T_StdValue;//+
                                T_MinusThresholdValue = T_AvgValue - T_StdValue;//-
                                break;
                            }
                        case 2:
                            {
                                T_PlusThresholdValue = T_AvgValue + 1.5 * T_StdValue;//+
                                T_MinusThresholdValue = T_AvgValue - 1.5 * T_StdValue;//-
                                break;
                            }
                        case 3:
                            {
                                T_PlusThresholdValue = T_AvgValue + 2.0 * T_StdValue;//+
                                T_MinusThresholdValue = T_AvgValue - 2.0 * T_StdValue;//-
                                break;
                            }
                        case 4://k倍
                            {
                                T_PlusThresholdValue = T_AvgValue + threshold * T_StdValue;//+
                                T_MinusThresholdValue = T_AvgValue - threshold * T_StdValue;//-
                                break;
                            }
                        case 5://固定阈值
                            {
                                T_PlusThresholdValue = threshold;//+
                                T_MinusThresholdValue = (-1)*threshold;//-
                                break;
                            }
                        default:
                            T_PlusThresholdValue = 0.5;//+
                            T_MinusThresholdValue = -0.5;//-
                            break;
                    }
                    #endregion

                    int startTime = 0;//初始起始点                               
                    int endTime = 0; if (endTime >= mFileNum) endTime = mFileNum - 1;//初始结束点
                    #region 正异常 
                    for (int t = 0; t < mFileNum; t++)
                    {//正异常                              
                        startTime = t;//初始起始点                               
                        endTime = startTime + 1; if (endTime >= mFileNum) endTime = mFileNum - 1;//初始结束点
                        if (pTBuffer[startTime] != -9999.0 && pTBuffer[startTime] > T_PlusThresholdValue)//找到正异常点
                        {   //连续寻找异常
                            while (pTBuffer[endTime] != -9999.0 && pTBuffer[endTime] > T_PlusThresholdValue)
                            {
                                endTime++;
                                if (endTime >= mFileNum) { endTime = mFileNum; break; }
                                //endTime++;
                            }
                            if (endTime - startTime >= minDurTime)//持续5个月
                            {
                                //t = endTime;
                                for (int j = startTime; j < endTime; j++)
                                {
                                    //pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                    //保存正异常值
                                    pJuJiBufferCut_positive[j * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[j * rowCut * colCut + rowNow * colCut + colNow];//结果时空立方体切割后
                                }
                                #region 连续正异常之间的0赋值为1,往前推
                                if (zeroValue)//0赋值，选中状态
                                {
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往前推
                                    {
                                        int SIndex = startTime - kk; if (SIndex < 0) break;//时间范围内的异常点 
                                        if (pTBuffer[SIndex] > T_PlusThresholdValue && pTBuffer[SIndex] != -9999)
                                        {
                                            //int STemp = SIndex;
                                            if (SIndex > 1)//至少是第二个
                                            {
                                                int STemp = SIndex;
                                                while (pTBuffer[STemp] > T_PlusThresholdValue && pTBuffer[STemp] != -9999)
                                                {
                                                    //pJuJiBufferCut[STemp * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                    pJuJiBufferCut_positive[STemp * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[STemp * rowCut * colCut + rowNow * colCut + colNow];
                                                    STemp--; if (STemp < 0) break;
                                                }
                                                if (STemp != SIndex)
                                                {
                                                    for (int n = SIndex + 1; n < startTime; n++)
                                                    {
                                                        //pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                        pJuJiBufferCut_positive[n * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[n * rowCut * colCut + rowNow * colCut + colNow];
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往后推
                                    {
                                        int EIndex = endTime + kk; if (EIndex >= mFileNum) break;//时间范围内的异常点   
                                        if (pTBuffer[EIndex] > T_PlusThresholdValue && pTBuffer[EIndex] != -9999)
                                        {
                                            if (EIndex < mFileNum - 1)//至多是倒数第二个
                                            {
                                                int ETemp = EIndex;
                                                while (pTBuffer[ETemp] > T_PlusThresholdValue && pTBuffer[ETemp] != -9999)
                                                {
                                                    //pJuJiBufferCut[ETemp * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                    pJuJiBufferCut_positive[ETemp * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[ETemp * rowCut * colCut + rowNow * colCut + colNow];
                                                    ETemp++; if (ETemp > mFileNum - 1) break;
                                                }
                                                if (ETemp != EIndex)
                                                {
                                                    for (int n = endTime; n < EIndex; n++)
                                                    {
                                                        //pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                        pJuJiBufferCut_positive[n * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[n * rowCut * colCut + rowNow * colCut + colNow];
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                #endregion
                            }
                            t = endTime;
                        }
                    }
                    #endregion

                    #region 负异常 
                    for (int t = 0; t < mFileNum; t++)
                    {//负异常                              
                        startTime = t;//初始起始点                               
                        endTime = t + 1; if (endTime >= mFileNum) endTime = mFileNum - 1;//初始结束点
                        if (pTBuffer[startTime] != -9999 && pTBuffer[startTime] < T_MinusThresholdValue)//找到正异常点
                        {   //连续寻找异常
                            while (pTBuffer[endTime] != -9999 && pTBuffer[endTime] < T_MinusThresholdValue)
                            {
                                endTime++;
                                if (endTime >= mFileNum) { endTime = mFileNum; break; }
                            }
                            if (endTime - startTime >= minDurTime)
                            {
                                for (int j = startTime; j < endTime; j++)
                                {
                                    //pJuJiBufferCut_negative[j * rowCut * colCut + rowNow * colCut + colNow] = 2;//结果时空立方体切割后
                                    //保存负异常值
                                    pJuJiBufferCut_negative[j * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[j * rowCut * colCut + rowNow * colCut + colNow];//结果时空立方体切割后
                                }
                                #region 连续负异常之间的0赋值为2,往前推
                                if (zeroValue)//0赋值，选中状态
                                {
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往前推
                                    {
                                        int SIndex = startTime - kk; if (SIndex < 0) break;//时间范围内的异常点 
                                        if (pTBuffer[SIndex] < T_MinusThresholdValue && pTBuffer[SIndex] != -9999)
                                        {
                                            if (SIndex > 1)//至少是第二个
                                            {
                                                int STemp = SIndex;
                                                while (pTBuffer[STemp] < T_MinusThresholdValue && pTBuffer[STemp] != -9999)
                                                {
                                                    //pJuJiBufferCut_negative[STemp * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                    pJuJiBufferCut_negative[STemp * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[STemp * rowCut * colCut + rowNow * colCut + colNow];
                                                    STemp--; if (STemp < 0) break;
                                                }
                                                if (STemp != SIndex)
                                                {
                                                    for (int n = SIndex + 1; n < startTime; n++)
                                                    {
                                                        //pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                        pJuJiBufferCut_negative[n * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[n * rowCut * colCut + rowNow * colCut + colNow];
                                                    }
                                                }
                                            }
                                            break;
                                        }

                                    }
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往后推
                                    {
                                        int EIndex = endTime + kk; if (EIndex >= mFileNum) break;//时间范围内的异常点   
                                        if (pTBuffer[EIndex] < T_MinusThresholdValue && pTBuffer[EIndex] != -9999)
                                        {
                                            if (EIndex < mFileNum - 1)//至多是倒数第二个
                                            {
                                                int ETemp = EIndex;
                                                while (pTBuffer[ETemp] < T_MinusThresholdValue && pTBuffer[ETemp] != -9999)
                                                {
                                                    //pJuJiBufferCut[ETemp * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                    pJuJiBufferCut_negative[ETemp * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[ETemp * rowCut * colCut + rowNow * colCut + colNow];
                                                    ETemp++; if (ETemp > mFileNum - 1) break;
                                                }
                                                if (ETemp != EIndex)
                                                {
                                                    for (int n = endTime; n < EIndex; n++)
                                                    {
                                                        //pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                        pJuJiBufferCut_negative[n * rowCut * colCut + rowNow * colCut + colNow] = pBuffer[n * rowCut * colCut + rowNow * colCut + colNow];
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                #endregion
                            }
                            t = endTime;
                        }
                    }
                    #endregion
                    #endregion
                }
                #region 保存
                for (int i = 0; i < pJuJiBufferCut_negative.Length; i++)
                {
                    int fileId = i / (rowCut * colCut);//文件编号
                    int filePos = i % (rowCut * colCut);//文件中编号
                    int startPos = startRow * col;//在该文件的起始位置
                    lock (objlock1)
                    {
                        //存储正异常图像
                        byteArrList_positive[fileId][startPos + filePos] = pJuJiBufferCut_positive[i];
                        //if (pJuJiBufferCut[i] == 2)
                        //{
                        //    byteArrList_positive[fileId][startPos + filePos] = 0;
                        //}
                        //else
                        //{
                        //    byteArrList_positive[fileId][startPos + filePos] = pJuJiBufferCut[i];
                        //}
                        //存储负异常图像
                        byteArrList_negative[fileId][startPos + filePos] = pJuJiBufferCut_negative[i];
                        //if (pJuJiBufferCut[i] == 1)
                        //{
                        //    byteArrList_negative[fileId][startPos + filePos] = 0;
                        //}
                        //else
                        //{
                        //    byteArrList_negative[fileId][startPos + filePos] = pJuJiBufferCut[i];
                        //}
                    }
                }
                #endregion
            }
        }

        private void Store(object obj)
        {
            int i = (int)obj;
            for (int j = 1; j <= 2; j++)
            {
                int[] pResultBuffer = new int[row * col];
                string fileName = Path.GetFileName(inFileNames[i].ToString());//获取文件名
                int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
                string outfileName;
                if (j == 1)
                {
                    pResultBuffer = byteArrList_positive[i];//需要保存的图像,正异常
                    outfileName = fileName.Substring(0, pointIndex) + "_positive_Time";
                }
                else
                {
                    pResultBuffer = byteArrList_negative[i];//需要保存的图像,负异常
                    outfileName = fileName.Substring(0, pointIndex) + "_negative_Time";
                }
                #region 删除独立点，填充噪声,八邻域,3*3,是否考虑在时间维处理时进行
                for (int m = 1; m < row - 1; m++)
                {
                    for (int n = 1; n < col - 1; n++)
                    {
                        //删除独立点
                        int count_delete = 0;
                        for (int mm = m - 1; mm < m + 2; mm++)
                        {
                            for (int nn = n - 1; nn < n + 2; nn++)
                            {
                                if (mm == m && nn == n)
                                {
                                    continue;
                                }
                                else
                                {
                                    if ((pResultBuffer[mm * col + nn] == 0 || pResultBuffer[mm * col + nn] == -9999) && (pResultBuffer[m * col + n] != 0 && pResultBuffer[m * col + n] != -9999))
                                    {
                                        count_delete++;
                                    }
                                }
                            }
                        }
                        if (count_delete > 6)
                        {
                            pResultBuffer[m * col + n] = 0;
                        }
                        //填充噪声,中值滤波，取众数
                        int count_fill = 0;
                        double fill_value = 0;
                        if (pResultBuffer[m * col + n] == 0 || pResultBuffer[m * col + n] == -9999)
                        {
                            for (int mm = m - 1; mm < m + 2; mm++)
                            {
                                for (int nn = n - 1; nn < n + 2; nn++)
                                {
                                    if (mm == m && nn == n)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        if (pResultBuffer[mm * col + nn] != 0 && pResultBuffer[mm * col + nn] != -9999)
                                        {
                                            count_fill++;
                                            fill_value += pResultBuffer[mm * col + nn];
                                        }
                                    }
                                }
                            }
                            if (count_fill >= 6)
                            {
                                pResultBuffer[m * col + n] = (int)(fill_value / count_fill);
                            }
                        }
                    }
                }
                #endregion

                string outPath = outFolderPath + "\\" + outfileName + ".tif";
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
                gTiffDataset.SetMetadataItem("StartLat", startLat.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
                gTiffDataset.SetMetadataItem("Scale", mScale.ToString(), null);
                gTiffDataset.SetMetadataItem("FillValue", "-9999", null);
                gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
                gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
                gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
                gTiffDataset.SetMetadataItem("Offsets", "0", null);
                gTiffDataset.SetMetadataItem("MaxValue", "3", null);
                gTiffDataset.SetMetadataItem("MinValue", "0", null);
                //gTiffDataset.GetRasterBand(1).SetNoDataValue(-9999);//设置空值 
                gTiffDataset.WriteRaster(0, 0, col, row, pResultBuffer, col, row, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
            #region 正负异常一起保存
            //int[] pResultBuffer = byteArrList_positive[i];//需要保存的图像
            //string fileName = Path.GetFileName(inFileNames[i].ToString());//获取文件名
            //int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
            //string outfileName = fileName.Substring(0, pointIndex) + "_Time";
            ////demband1.WriteRaster(0, 0, c, r, pResultBuffer, c, r, 0, 0);
            ////OSGeo.GDAL.SWIGTYPE_p_p_GDALRasterBandShadow ss=new SWIGTYPE_p_p_GDALRasterBandShadow()
            ////Gdal.CreatePansharpenedVRT(outPath, demband1, 1, demband1);

            //#region bmp格式保存
            ////Bitmap bmp = new Bitmap(c, r, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            ////for (int j = 0; j < r * c; j++)
            ////{
            ////    int x = j % c;
            ////    int y = j / c;
            ////    //pResultBuffer[j] = pJuJiBuffer[j + (r * c * i)];
            ////    Color color = Color.FromArgb(pResultBuffer[j] * 80, pResultBuffer[j] * 80, pResultBuffer[j] * 80);
            ////    bmp.SetPixel(x, y, color);
            ////}
            ////outfileName += ".bmp";
            ////string outPath = @"E:\BaoYuTime\" + outfileName;
            ////bmp.Save(outPath);
            //#endregion

            ////保存为tif格式
            //string outPath = outFolderPath + "\\" + outfileName + ".tif";
            //Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
            //Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
            ////gTiffDataset.SetProjection("PROJCS[\"WGS_1984_World_Mercator\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]]]");
            ////厦门坐标系统：PROJCS["WGS 84 / UTM zone 50N",GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0],UNIT["degree",0.0174532925199433],AUTHORITY["EPSG","4326"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",117],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","32650"]]
            ////gTiffDataset.SetGeoTransform(geoTransform);

            //gTiffDataset.SetGeoTransform(argout);//地理坐标信息
            //gTiffDataset.SetProjection(projection);//设置坐标系
            //gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
            //gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
            //gTiffDataset.SetMetadataItem("startLat", startLat.ToString(), null);
            //gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
            //gTiffDataset.SetMetadataItem("Scale", "1", null);
            //gTiffDataset.SetMetadataItem("FillValue", "255", null);
            //gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
            //gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
            //gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
            //gTiffDataset.SetMetadataItem("Offsets", "0", null);
            //gTiffDataset.SetMetadataItem("MaxValue", "3", null);
            //gTiffDataset.SetMetadataItem("MinValue", "0", null);
            ////gTiffDataset.GetRasterBand(1).SetNoDataValue(-9999);//设置空值 
            //gTiffDataset.WriteRaster(0, 0, col, row, pResultBuffer, col, row, 1, null, 0, 0, 0);
            //gTiffDataset.Dispose();
            #endregion
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            progressBar1.Value = e.ProgressPercentage;
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
                    if (t != null && t.ThreadState != ThreadState.Stopped) t.Abort();
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

        #region 按钮Button
        private void addFilesbtn_Click(object sender, EventArgs e)
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
                    FileListbox.Items.Add(fileName);
                }
                FileNumbox.Text = FileListbox.Items.Count.ToString();
            }
        }

        private void deleteFilesbtn_Click(object sender, EventArgs e)
        {
            int index = FileListbox.SelectedIndex;
            while (index > -1)
            {
                FileListbox.Items.RemoveAt(index);
                index = FileListbox.SelectedIndex;
            }
            FileNumbox.Text = FileListbox.Items.Count.ToString();
        }

        private void upMovebtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = FileListbox.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    FileListbox.Items.Insert(endIndex + 1, FileListbox.Items[startIndex - 1].ToString());
                    FileListbox.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void downMovebtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = FileListbox.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (FileListbox.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    FileListbox.Items.Insert(startIndex, FileListbox.Items[endIndex + 1].ToString());
                    FileListbox.Items.RemoveAt(endIndex + 2);
                    selectedIndices = FileListbox.SelectedIndices;
                }
            }
        }

        private void addFilesbtn2_Click(object sender, EventArgs e)
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
                    FileListbox2.Items.Add(fileName);
                }
                FileNumbox2.Text = FileListbox2.Items.Count.ToString();
            }
        }

        private void deleteFilesbtn2_Click(object sender, EventArgs e)
        {
            int index = FileListbox2.SelectedIndex;
            while (index > -1)
            {
                FileListbox2.Items.RemoveAt(index);
                index = FileListbox2.SelectedIndex;
            }
            FileNumbox2.Text = FileListbox2.Items.Count.ToString();
        }

        private void upMovebtn2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = FileListbox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    FileListbox2.Items.Insert(endIndex + 1, FileListbox2.Items[startIndex - 1].ToString());
                    FileListbox2.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void downMovebtn2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = FileListbox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (FileListbox2.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    FileListbox2.Items.Insert(startIndex, FileListbox2.Items[endIndex + 1].ToString());
                    FileListbox2.Items.RemoveAt(endIndex + 2);
                    selectedIndices = FileListbox2.SelectedIndices;
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
        //ListBox.ObjectCollection oriFileNames;
        double valueScale;
        double timeCell;
        double maxTimeInv;
        int rowCut = 10;//每次处理栅格长度
        bool zeroValue = false;//零值是否赋值
        int thresholdIndex;
        double threshold;
        int minDurTime = 0;

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (FileListbox.Items.Count == 0)
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

            inFileNames = FileListbox.Items;
            //oriFileNames = FileListbox2.Items;
            outFolderPath = textBox1.Text.Trim();
            threads = new Thread[threadCount];//线程数
            valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离
            rowCut = Convert.ToInt32(rowTextBox.Text.Trim());
            minDurTime = Convert.ToInt32(MinDurTimeBox.Text.Trim());
            minDurTime = Math.Abs(minDurTime);
            thresholdIndex = thresholdCBox.SelectedIndex;//异常阈值
            if (thresholdIndex == 4)
            {
                threshold = Convert.ToDouble(thresholdBox.Text.Trim());
                threshold = Math.Abs(threshold);
            }
            zeroValue = zeroValueCheckBox.Checked;
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
                MessageBox.Show("未进行处理！");
            }
        }
        #endregion

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

        private void thresholdCBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = thresholdCBox.SelectedIndex;
            if (index == 4 || index == 5)
            {
                thresholdBox.Visible = true;
            }
            else
            {
                thresholdBox.Visible = false;
            }
        }

    }
}
