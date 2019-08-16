using System;
using System.Windows.Forms;
using OSGeo.GDAL;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading;
using MarineSTMiningSystem.Class;

namespace MarineSTMiningSystem
{
    public partial class StormTimeExtractForm : Form
    {
        int threadCount = 1;//线程数
        Thread[] threads;//线程
        string outFolderPath = "";
        int mFileNum = 0;
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public StormTimeExtractForm()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }
        //ListBox.ObjectCollection inFileNames;//输入文件名

        struct Param
        {
            public int startRow;
            public int endRow;
            public int startCol;
            public int endCol;
        }

        struct Param2
        {
            public int i;
            public int j;
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
        //List<byte[]> byteArrList = new List<byte[]>();//存储结果的时空立方体
        //byte[] byteArrList;//存储结果的时空立方体
        List<int[]> byteArrList_positive = new List<int[]>();//存储结果的时空立方体
        List<int[]> byteArrList_negative = new List<int[]>();//存储结果的时空立方体
        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            //string[] files = Directory.GetFiles(inFolderPath);//所有文件名
            mFileNum = inFileNames.Count;//文件个数
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            string hdfFileName = inFileNames[0].ToString();
            //string hdfFileName = @"E:\BaoYuTime\XM_2014_CJ_tif";
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            //string projection = ds.GetProjection();
            //double[] geoTransformO = new double[6];
            //ds.GetGeoTransform(geoTransformO);
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
            //double geoTransform1 = (endLat - startLat) / col;
            //double geoTransform5 = (endLog - startLog) / row;
            //double[] geoTransform = { startLog, col, 0, endLat, 0, row };
            //byte[] pJuJiBuffer = new byte[mFileNum * row * col];//存储结果的时空立方体
            //string[] oriFilesName = new string[inFileNames.Count];
            //for(int i=0;i< inFileNames.Count;i++)
            //{
            //    oriFilesName[i] = inFileNames[i].ToString();
            //}

            //byteArrList = new byte[mFileNum * row * col];
            for(int i=0;i<mFileNum;i++)
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
                p.startRow = startRow;p.endRow = endRow;
                p.startCol = startCol;p.endCol = endCol;
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

            #region 以前版本
            /*
            //源数据切100份
            int rowCut = row / 10;//行数切割
            int colCut = col / 10;//列数切割


            for (int mr = 0; mr < 10; mr++)
            {
                for (int mc = 0; mc < 10; mc++)
                {//分100份
                    if (worker.CancellationPending)
                    {//取消
                        e.Cancel = true;
                        return;
                    }
                    byte[] pJuJiBufferCut = new byte[mFileNum * rowCut * colCut];//存储结果的时空立方体

                    int progress = mr * 10 + mc;//进度
                    worker.ReportProgress(progress);//记录进度

                    int[] pBuffer = new int[mFileNum * rowCut * colCut];//存储切割部分的时空立方体
                    //当前切割图像的起始行列号
                    long startRow = mr * rowCut;
                    long startCol = mc * colCut;

                    //读取所有文件切割部分
                    for (int i = 0; i < mFileNum; i++)
                    {
                        //hdfFileName = oriFilesName[i];
                        //打开文件
                        ds = Gdal.Open(inFileNames[i].ToString(), Access.GA_ReadOnly);
                        Band demband = ds.GetRasterBand(1);//读取波段
                        int[] databuf = new int[rowCut * colCut];//存储该切割部分的数组
                        demband.ReadRaster(mc * colCut, mr * rowCut, colCut, rowCut, databuf, colCut, rowCut, 0, 0);//读取数据
                        for (int j = 0; j < rowCut * colCut; j++)
                        {//进行存储
                            pBuffer[i * rowCut * colCut + j] = databuf[j];
                        }
                        ds.Dispose();
                    }

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
                                    pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                                }
                                else
                                {//空值
                                    pTBuffer[t] = 0;
                                    //pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 255;
                                    pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 255;//结果时空立方体切割后
                                }
                            }

                            if (type == 1)
                            {//暴雨处理
                                #region 暴雨处理
                                for (int t = 1; t < mFileNum - 1; t++)
                                {//对每个降雨量进行处理
                                    if (pTBuffer[t] >= 2.0 && pTBuffer[t] >= pTBuffer[t - 1] && pTBuffer[t] >= pTBuffer[t + 1])
                                    {//极值点，且大于2.0
                                        int startTime = t - 1;//初始起始点
                                        if (startTime < 0) startTime = 0;
                                        int endTime = t + 1;//初始结束点
                                        if (endTime > mFileNum - 1) endTime = mFileNum - 1;
                                        Jump1:
                                        while (pTBuffer[startTime] > 0.0)
                                        {//向前扩
                                         //if((t-startTime)<24)
                                         //{//起始点位于12个小时以内
                                         //	
                                         //}
                                         //else
                                         //{
                                         //	break;
                                         //}
                                            startTime--;//判断下一个
                                            if (startTime <= 0) break;
                                        }
                                        //for (int j = 1; j <= 6; j++)
                                        for (int j = 1; j <= DoubleRvide(maxTimeInv, timeCell); j++)
                                        {
                                            //if((i-startTime)<(24-j)&&pTBuffer[startTime-j]>0.0)
                                            if (startTime - j >= 0 && pTBuffer[startTime - j] > 0.0)
                                            {//3小时内存在降雨
                                                if ((startTime - j) < 0) break;
                                                startTime -= j;
                                                goto Jump1;
                                            }
                                        }
                                        startTime++;

                                        Jump2:
                                        while (pTBuffer[endTime] > 0.0)
                                        {//向后扩
                                         //if((endTime-t)<24)
                                         //{//
                                         //	
                                         //}
                                         //else
                                         //{
                                         //	break;
                                         //}
                                            endTime++;//判断下一个
                                            if (endTime >= (mFileNum - 1)) break;
                                        }
                                        //for (int j = 1; j <= 6; j++)
                                        for (int j = 1; j <= DoubleRvide(maxTimeInv, timeCell); j++)
                                        {
                                            //if((endTime-1)<(24-j)&&pTBuffer[endTime+j]>0.0)
                                            if (endTime + j < mFileNum && pTBuffer[endTime + j] > 0.0)
                                            {//没有向前12个小时,3小时内存在降雨
                                                if (endTime + j > (mFileNum - 1)) break;
                                                endTime = endTime + j;
                                                goto Jump2;
                                            }
                                        }
                                        endTime--;

                                        double jylCount = 0.0;//降雨量之和
                                        byte baoYu = 0;//记录暴雨

                                        int st24 = startTime;//保证在24小时内的范围
                                        int et24 = endTime;//同理
                                        if (st24 < (t - 12 * timeCell - 1)) st24 = t - Convert.ToInt32(12 * timeCell) - 1;
                                        if (et24 > (t + 12 * timeCell - 1)) et24 = t + Convert.ToInt32(12 * timeCell) - 1;
                                        for (int j = st24; j <= et24; j++)
                                        {
                                            jylCount += pTBuffer[j];
                                        }
                                        if (jylCount >= 250.0)
                                        {//特大暴雨
                                            baoYu = 3;
                                        }
                                        else if (jylCount >= 100.0)
                                        {//大暴雨
                                            baoYu = 2;
                                        }
                                        else if (jylCount >= 50.0)
                                        {//暴雨
                                            baoYu = 1;
                                        }

                                        if (baoYu > 0)
                                        {//暴雨
                                            if(baoYu > pJuJiBufferCut[startTime * rowCut * colCut + rowNow * colCut + colNow])
                                            {//大于已经赋值的
                                                for (int j = startTime; j <= endTime; j++)
                                                {
                                                    //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = baoYu;//
                                                    if(pTBuffer[j]>0.0)//只对非0值赋值
                                                    pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = baoYu;//结果时空立方体切割后
                                                }
                                            }
                                            
                                        }
                                        else
                                        {//不是24小时连续
                                            if (pTBuffer[startTime] > 16.0)
                                            {//开始点暴雨
                                             //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                pJuJiBufferCut[startTime * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                                                                                         // pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                            }

                                            for (int j = startTime+1; j <= endTime-1; j++)
                                            {
                                                if (pTBuffer[j] + pTBuffer[j + 1] > 16.0 && pTBuffer[j] > 0.0)
                                                {//暴雨
                                                    pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                    if (pTBuffer[j + 1] > 0.0)
                                                    {
                                                        pJuJiBufferCut[(j + 1) * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                    }
                                                }
                                                else if (pTBuffer[j] + pTBuffer[j - 1] > 16.0 && pTBuffer[j] > 0.0)
                                                {//暴雨
                                                    pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                    if (pTBuffer[j - 1] > 0.0)
                                                    {
                                                        pJuJiBufferCut[(j - 1) * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                    }
                                                }

                                                //if (pTBuffer[j] > 16.0)
                                                //{//暴雨
                                                // //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                //    pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                //                                                                       // pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                //}
                                            }

                                            if (pTBuffer[endTime] > 16.0)
                                            {//结束点暴雨
                                             //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                pJuJiBufferCut[endTime * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                                                                                                         // pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (type == 2)
                            {//降雨处理
                                #region 降雨处理
                                for (int t = 0; t < mFileNum; t++)
                                {//对每个降雨量进行处理
                                    if (pTBuffer[t] <= 0.0) continue;//小于等于0的不处理
                                    int startTime = t - (Convert.ToInt32(6 / timeCell)-1);//开始时刻
                                    int endTime = t + Convert.ToInt32(6 / timeCell);//结束时刻
                                    if(startTime<0)
                                    {
                                        endTime += 0 - startTime;
                                        startTime = 0;
                                    }
                                    else if(endTime>=mFileNum)
                                    {
                                        startTime -= endTime - (mFileNum - 1);
                                        endTime = mFileNum - 1;
                                    }
                                    //统计降雨量
                                    double jyl = 0.0;//降雨量
                                    for(int k=startTime;k<=endTime;k++)
                                    {
                                        if(pTBuffer[k]>0.0)
                                        {
                                            jyl += pTBuffer[k];
                                        }
                                    }
                                    if(jyl>0.0&&jyl<5.0) pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 1;//小雨
                                    else if(jyl>=5.0&&jyl<15.0) pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 2;//中雨
                                    else if(jyl>=15.0&&jyl<30.0) pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 3;//大雨
                                    else if(jyl>30.0) pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 4;//暴雨
                                }
                                #endregion
                            }

                        }
                    }
                    byteArrList.Add(pJuJiBufferCut);
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

                byte[] pResultBuffer = new byte[row * col];//需要保存的图像
                for (int j = 0; j < row * col; j++)
                {
                    int rowNow = j / col;//当前行号
                    int colNow = j % col;//当前列号
                    int rowBig = rowNow / rowCut;//切割行号
                    int colBig = colNow / colCut;//切割列号
                    int listId = rowBig * 10 + colBig;//切割编号
                    int rowSmallNow = rowNow % rowCut;//切割体中的行号
                    int colSmallNow = colNow % colCut;//切割体中的列号
                    pResultBuffer[j] = byteArrList[listId][rowSmallNow * colCut + colSmallNow + (rowCut * colCut * i)];

                    //pResultBuffer[j] = pJuJiBuffer[j + (row * col * i)];
                }
                string fileName = Path.GetFileName(inFileNames[i].ToString());//获取文件名
                int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
                string outfileName = fileName.Substring(0, pointIndex) + "_Time";
                //demband1.WriteRaster(0, 0, c, r, pResultBuffer, c, r, 0, 0);
                //OSGeo.GDAL.SWIGTYPE_p_p_GDALRasterBandShadow ss=new SWIGTYPE_p_p_GDALRasterBandShadow()
                //Gdal.CreatePansharpenedVRT(outPath, demband1, 1, demband1);

                #region bmp格式保存
                //Bitmap bmp = new Bitmap(c, r, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                //for (int j = 0; j < r * c; j++)
                //{
                //    int x = j % c;
                //    int y = j / c;
                //    //pResultBuffer[j] = pJuJiBuffer[j + (r * c * i)];
                //    Color color = Color.FromArgb(pResultBuffer[j] * 80, pResultBuffer[j] * 80, pResultBuffer[j] * 80);
                //    bmp.SetPixel(x, y, color);
                //}
                //outfileName += ".bmp";
                //string outPath = @"E:\BaoYuTime\" + outfileName;
                //bmp.Save(outPath);
                #endregion

                //保存为tif格式
                string outPath = outFolderPath + "\\" + outfileName + ".tif";
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Byte, null);
                //gTiffDataset.SetProjection("PROJCS[\"WGS_1984_World_Mercator\",GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]],PROJECTION[\"Mercator_1SP\"],PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]]]");
                //厦门坐标系统：PROJCS["WGS 84 / UTM zone 50N",GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0],UNIT["degree",0.0174532925199433],AUTHORITY["EPSG","4326"]],PROJECTION["Transverse_Mercator"],PARAMETER["latitude_of_origin",0],PARAMETER["central_meridian",117],PARAMETER["scale_factor",0.9996],PARAMETER["false_easting",500000],PARAMETER["false_northing",0],UNIT["metre",1,AUTHORITY["EPSG","9001"]],AUTHORITY["EPSG","32650"]]
                //gTiffDataset.SetGeoTransform(geoTransform);
                
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
                gTiffDataset.SetMetadataItem("startLat", startLat.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
                gTiffDataset.SetMetadataItem("Scale", "1", null);
                gTiffDataset.SetMetadataItem("FillValue", "255", null);
                gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
                gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
                gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
                gTiffDataset.SetMetadataItem("Offsets", "0", null);
                gTiffDataset.SetMetadataItem("MaxValue", "3", null);
                gTiffDataset.SetMetadataItem("MinValue", "0", null);
                gTiffDataset.WriteRaster(0, 0, col, row, pResultBuffer, col, row, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
            }
            */
            #endregion
        }

        private void Store(object obj)
        {
            int i = (int)obj;
            //byte[] pResultBuffer = new byte[row * col];//需要保存的图像
            //for (int j = 0; j < row * col; j++)
            //{
            //    pResultBuffer[j] = byteArrList[i * row * col + j];
            //}
            for (int j=1;j<=2;j++)
            {
                int[] pResultBuffer = new int[row * col];
                string fileName = Path.GetFileName(inFileNames[i].ToString());//获取文件名
                int pointIndex = fileName.LastIndexOf('.');//最后一个.的位置
                string outfileName;
                if (j==1)
                {
                    pResultBuffer = byteArrList_positive[i];//需要保存的图像,正异常
                    outfileName = fileName.Substring(0, pointIndex) + "positive_Time";
                }
                else
                {
                    pResultBuffer = byteArrList_negative[i];//需要保存的图像,负异常
                    outfileName = fileName.Substring(0, pointIndex) + "negative_Time";
                }
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
                gTiffDataset.SetMetadataItem("FillValue", "255", null);
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

        private void TimeWorke(object obj)
        {
            Param p = (Param)obj;
            int startRow = p.startRow;
            int endRow = p.endRow;
            int startCol = p.startCol;
            int endCol = p.endCol;
            int rowCut = endRow - startRow;
            int colCut = endCol - startCol;
            int[] pJuJiBufferCut = new int[mFileNum * rowCut * colCut];//存储结果的时空立方体
            int[] pBuffer = ReadAllRaster(startRow, endRow, startCol, endCol);//读取所有时刻切割图像
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
                            pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = 0;//结果时空立方体切割后
                        }
                        else
                        {//空值
                            pTBuffer[t] = -9999;
                            //pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 255;
                            pJuJiBufferCut[t * rowCut * colCut + rowNow * colCut + colNow] = -9999;//结果时空立方体切割后
                        }
                    }
                    #region SST
                    if (type == 1)
                    {//SST处理
                        double T_AvgValue = GeneralFun.GetMeanValue(pTBuffer);//默认带有-9999填充值
                        double T_StdValue = GeneralFun.GetStdValue(pTBuffer);//默认带有-9999填充值
                        //double T_PlusThresholdValue = T_AvgValue + 1.5 * T_StdValue;//+
                        //double T_MinusThresholdValue = T_AvgValue - 1.5 * T_StdValue;//-

                        double T_PlusThresholdValue = 0.5;//+
                        double T_MinusThresholdValue = -0.5;//-

                        //double T_PlusThresholdValue = 0.5;//+
                        //double T_MinusThresholdValue = -0.5;//-
                        int startTime = 0;//初始起始点                               
                        int endTime = 0; if (endTime >= mFileNum) endTime = mFileNum - 1;//初始结束点
                        #region 正异常 
                        for (int t = 0; t < mFileNum; t++)
                        {//正异常                              
                            startTime = t;//初始起始点                               
                            endTime = startTime + 1; if (endTime >= mFileNum) endTime = mFileNum - 1;//初始结束点
                            if (pTBuffer[startTime] != -9999 && pTBuffer[startTime] > T_PlusThresholdValue)//找到正异常点
                            {   //连续寻找异常
                                while (pTBuffer[endTime] != -9999 && pTBuffer[endTime] > T_PlusThresholdValue)
                                {
                                    endTime++;
                                    if (endTime >= mFileNum) { endTime = mFileNum - 1; break; }
                                }
                                if (endTime - startTime >= 5)//持续5个月
                                {
                                    //t = endTime;
                                    for (int j = startTime; j < endTime; j++)
                                    {
                                        pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 1;//结果时空立方体切割后
                                    }
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往前推
                                    {
                                        int SIndex = startTime - kk; if (SIndex < 0) break;//时间范围内的异常点 
                                        if(pTBuffer[SIndex] > T_PlusThresholdValue && pTBuffer[SIndex] != -9999)
                                        {
                                            //int STemp = SIndex;
                                            if (SIndex > 1)//至少是第二个
                                            {
                                                int STemp = SIndex;
                                                while (pTBuffer[STemp] > T_PlusThresholdValue && pTBuffer[STemp] != -9999)
                                                {
                                                    pJuJiBufferCut[STemp * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                    STemp--; if (STemp < 0) break;
                                                }
                                                #region 连续正异常之间的0赋值为1,往前推
                                                if (zeroValue)//0赋值，选中状态
                                                {
                                                    if (STemp != SIndex)
                                                    {
                                                        for (int n = SIndex + 1; n < startTime; n++)
                                                        {
                                                            pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                        }
                                                    }
                                                }
                                                #endregion
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
                                                    pJuJiBufferCut[ETemp * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                    ETemp++; if (ETemp > mFileNum - 1) break;
                                                }
                                                #region 连续正异常之间的0赋值为1,往后推
                                                if (zeroValue)//0赋值，选中状态
                                                {
                                                    if (ETemp != EIndex)
                                                    {
                                                        for (int n = endTime; n < EIndex; n++)
                                                        {
                                                            pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 1;
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            break;
                                        }
                                    }

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
                                    if (endTime >= mFileNum) { endTime = mFileNum - 1; break; }
                                }
                                if (endTime - startTime >= 5)
                                {                                  
                                    for (int j = startTime; j < endTime; j++)
                                    {
                                        pJuJiBufferCut[j * rowCut * colCut + rowNow * colCut + colNow] = 2;//结果时空立方体切割后
                                    }
                                    for (int kk = 1; kk < DoubleRvide(maxTimeInv, timeCell); kk++)//往前推
                                    {
                                        int SIndex = startTime - kk; if (SIndex < 0) break;//时间范围内的异常点 
                                        if (pTBuffer[SIndex]< T_MinusThresholdValue && pTBuffer[SIndex] != -9999)
                                        {
                                            if (SIndex > 1)//至少是第二个
                                            {
                                                int STemp = SIndex;
                                                while (pTBuffer[STemp] < T_MinusThresholdValue && pTBuffer[STemp] != -9999)
                                                {
                                                    pJuJiBufferCut[STemp * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                    STemp--; if (STemp < 0) break;
                                                }
                                                #region 连续负异常之间的0赋值为2,往前推
                                                if (zeroValue)//0赋值，选中状态
                                                {
                                                    if (STemp != SIndex)
                                                    {
                                                        for (int n = SIndex + 1; n < startTime; n++)
                                                        {
                                                            pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                        }
                                                    }
                                                }
                                                #endregion
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
                                                    pJuJiBufferCut[ETemp * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                    ETemp++; if (ETemp > mFileNum - 1) break;
                                                }
                                                #region 连续负异常之间的0赋值为2,往后推
                                                if (zeroValue)//0赋值，选中状态
                                                {
                                                    if (ETemp != EIndex)
                                                    {
                                                        for (int n = endTime; n < EIndex; n++)
                                                        {
                                                            pJuJiBufferCut[n * rowCut * colCut + rowNow * colCut + colNow] = 2;
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            break;
                                        }

                                    }
                                }                               
                                t = endTime;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                //byteArrList.Add(pJuJiBufferCut);
                //保存
                for (int i = 0; i < pJuJiBufferCut.Length; i++)
                {
                    int fileId = i / (rowCut * colCut);//文件编号
                    int filePos = i % (rowCut * colCut);//文件中编号
                    int startPos = startRow * col;//在该文件的起始位置
                    lock (objlock1)
                    {
                        //存储正异常图像
                        if (pJuJiBufferCut[i]==2)
                        {
                            byteArrList_positive[fileId][startPos + filePos] = 0;
                        }
                        else
                        {
                            byteArrList_positive[fileId][startPos + filePos] = pJuJiBufferCut[i];
                        }
                        //存储负异常图像
                        if (pJuJiBufferCut[i] == 1)
                        {
                            byteArrList_negative[fileId][startPos + filePos] = 0;
                        }
                        else
                        {
                            byteArrList_negative[fileId][startPos + filePos] = pJuJiBufferCut[i];
                        }
                    }
                }
            }
        }

        private byte GetPower(double rainSum)
        {
            byte power = 0;
            if (rainSum >= 250.0)
            {//特大暴雨
                power = 3;
            }
            else if (rainSum >= 100.0)
            {//大暴雨
                power = 2;
            }
            else if (rainSum >= 50.0)
            {//暴雨
                power = 1;
            }
            return power;
        }

        private static object objlock1 = new object();

        private int[] ReadAllRaster(int startRow, int endRow, int startCol, int endCol)
        {
            int rowCut = endRow - startRow;
            int colCut = endCol - startCol;
            int[] pBuffer = new int[mFileNum * rowCut * colCut];//存储切割部分的时空立方体
            

            //读取所有文件切割部分
            for (int i = 0; i < mFileNum; i++)
            {
                //hdfFileName = oriFilesName[i];
                //打开文件
                Dataset ds = Gdal.Open(inFileNames[i].ToString(), Access.GA_ReadOnly);
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
                    if (t!=null&&t.ThreadState != ThreadState.Stopped) t.Abort();
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
                foreach(string fileName in filesNames)
                {
                    listBox1.Items.Add(fileName);
                }
                countTextBox.Text = listBox1.Items.Count.ToString();
            }
        }

        private void deleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            while (index>-1)
            {
                listBox1.Items.RemoveAt(index);
                index = listBox1.SelectedIndex;
            }
            countTextBox.Text = listBox1.Items.Count.ToString();
        }

        private void moveUpBtn_Click(object sender, EventArgs e)
        {//多选不连续时，存在bug
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0&& endIndex- startIndex+1== selectedIndices.Count)
                {
                    listBox1.Items.Insert(endIndex + 1, listBox1.Items[startIndex-1].ToString());
                    listBox1.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void moveDownBtn_Click(object sender, EventArgs e)
        {//多选不连续时，存在bug
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox1.Items.Count-1) && endIndex - startIndex + 1 == selectedIndices.Count)
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

        ListBox.ObjectCollection inFileNames;
        double valueScale;
        double timeCell;
        double maxTimeInv;
        int type;
        int rowCut = 10;//每次处理栅格长度
        bool zeroValue = false;//零值是否赋值
        private void okBtn_Click(object sender, EventArgs e)
        {
            if(worker.IsBusy)
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
            threads = new Thread[threadCount];//线程数
            valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            timeCell= Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离
            rowCut =Convert.ToInt32(rowTextBox.Text.Trim());
            if (stormBtm.Checked == true) type = 1;//暴雨处理
            else if (rainBtn.Checked == true) type = 2;//降雨处理
            //type = 3;
            //okBtn.Enabled = false;
            zeroValue = zeroValueCheckBox.Checked;
            progressBar1.Show();
            worker.RunWorkerAsync();
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
