using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OSGeo.GDAL;
using System.IO;

namespace MarineSTMiningSystem
{
    class ClsStormEvent
    {
        /// <summary>
        /// 暴雨时间维度提取
        /// </summary>
        /// <param name="progressBar">进度条</param>
        public void stormTimeExtraction(ProgressBar progressBar)
        {
            string inFolderPath = "";//输入路径
            string outFolderPath = "";//输出路径
            //选择输入目录
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"E:\";
            fbd.Description = "选择原始数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                //this.textbox1.Text = fbd.SelectedPath;
                inFolderPath = fbd.SelectedPath;
                fbd.Description = "选择输出结果文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    outFolderPath = fbd.SelectedPath;
                }
            }
            if (inFolderPath != "" && outFolderPath != "")
            {//存在输入输出路径
                progressBar.Show();//进度条显示
                string[] files = Directory.GetFiles(inFolderPath);//所有文件名
                int mFileNum = files.Length;//文件个数
                if (mFileNum == 0)
                {
                    MessageBox.Show("目录下不存在文件！");
                    return;
                }
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
                string hdfFileName = files[0];
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

                //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
                string[] metadatas = ds.GetMetadata("");//获取元数据
                double startLog = 0.0;//起始经度
                double startLat = 0.0;//起始维度
                double endLog = 0.0;//结束经度
                double endLat = 0.0;//结束维度
                double mScale = 0.0;//比例
                string dataType = "";
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
                        default:
                            break;
                    }
                }
                //double geoTransform1 = (endLat - startLat) / col;
                //double geoTransform5 = (endLog - startLog) / row;
                //double[] geoTransform = { startLog, col, 0, endLat, 0, row };
                byte[] pJuJiBuffer = new byte[mFileNum * row * col];//存储结果的时空立方体

                //源数据切100份
                int rowCut = row / 10;//行数切割
                int colCut = col / 10;//列数切割


                for (int mr = 0; mr < 10; mr++)
                {
                    for (int mc = 0; mc < 10; mc++)
                    {//分100份
                        int progress = mr * 10 + mc;
                        progressBar.Value = progress;
                        int[] pBuffer = new int[mFileNum * rowCut * colCut];//存储切割部分的时空立方体
                                                                            //当前切割图像的起始行列号
                        long startRow = mr * rowCut;
                        long startCol = mc * colCut;

                        //读取所有文件切割部分
                        for (int i = 0; i < mFileNum; i++)
                        {
                            hdfFileName = files[i];
                            //打开hdf文件
                            ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
                            Band demband = ds.GetRasterBand(1);//读取波段
                            int[] databuf = new int[rowCut * colCut];//存储该切割部分的数组
                            demband.ReadRaster(mc * colCut, mr * rowCut, colCut, rowCut, databuf, colCut, rowCut, 0, 0);//读取数据
                            for (int j = 0; j < rowCut * colCut; j++)
                            {//进行存储
                                pBuffer[i * rowCut * colCut + j] = databuf[j];
                            }
                        }

                        for (int rowNow = 0; rowNow < rowCut; rowNow++)
                        {//按行循环
                            for (int colNow = 0; colNow < colCut; colNow++)
                            {//按列循环
                                double[] pTBuffer = new double[mFileNum];
                                for (int t = 0; t < mFileNum; t++)
                                {//t为时间
                                 //int	k=row*c1+col;
                                    pTBuffer[t] = ((double)(pBuffer[t * rowCut * colCut + rowNow * colCut + colNow]) * mScale) / 2.0;
                                    if (pBuffer[t * rowCut * colCut + rowNow * colCut + colNow] != -9999)
                                    {
                                        pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 0;
                                    }
                                    else
                                    {
                                        pJuJiBuffer[t * row * col + (startRow + rowNow) * col + startCol + colNow] = 255;
                                    }
                                }

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
                                        for (int j = 1; j <= 6; j++)
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
                                        for (int j = 1; j <= 6; j++)
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
                                        for (int j = startTime; j <= endTime; j++)
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
                                            for (int j = startTime; j <= endTime; j++)
                                            {
                                                pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = baoYu;//
                                            }
                                        }
                                        else
                                        {//不是24小时连续
                                            for (int j = startTime; j <= endTime-1; j++)
                                            {
                                                if (pTBuffer[j] + pTBuffer[j + 1] > 16.0&& pTBuffer[j]>0.0)
                                                {//暴雨
                                                    pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                    if(pTBuffer[j+1] > 0.0)
                                                    {
                                                        pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                    }
                                                }
                                                //if (pTBuffer[j]*2 > 16.0)
                                                //{//暴雨
                                                //    pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                //    pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                                //}
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < mFileNum; i++)
                {//保存
                    int progress = (i * 100) / mFileNum;
                    progressBar.Value = progress;
                    byte[] pResultBuffer = new byte[row * col];//需要保存的图像
                    for (int j = 0; j < row * col; j++)
                    {
                        pResultBuffer[j] = pJuJiBuffer[j + (row * col * i)];
                    }
                    string fileName = Path.GetFileName(files[i]);//获取文件名
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
                    gTiffDataset.SetMetadata("StartLog", startLog.ToString());
                    gTiffDataset.SetMetadata("EndLog", endLog.ToString());
                    gTiffDataset.SetMetadata("startLat", startLat.ToString());
                    gTiffDataset.SetMetadata("EndLat", endLat.ToString());
                    gTiffDataset.SetMetadata("Scale", "1");
                    gTiffDataset.SetMetadata("FillValue", "255");
                    string imgDate = outfileName.Substring(0, 24);
                    gTiffDataset.SetMetadata("ImageDate", imgDate);
                    gTiffDataset.SetMetadata("DSResoution", "0.1");
                    gTiffDataset.SetMetadata("DataType", dataType);
                    gTiffDataset.WriteRaster(0, 0, col, row, pResultBuffer, col, row, 1, null, 0, 0, 0);
                    gTiffDataset.Dispose();
                }
                progressBar.Hide();
                MessageBox.Show("处理完成！");
            }
        }

        /// <summary>
        /// 暴雨空间维度提取
        /// </summary>
        /// <param name="progressBar">进度条</param>
        public void stormSpatialExtraction(ProgressBar progressBar)
        {
            string inFolderPath = "";//输入路径
            string outFolderPath = "";//输出路径
            //选择输入目录
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"E:\";
            fbd.Description = "选择时间数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                //this.textbox1.Text = fbd.SelectedPath;
                inFolderPath = fbd.SelectedPath;
                fbd.Description = "选择输出结果文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    outFolderPath = fbd.SelectedPath;
                }
            }
            if (inFolderPath != "" && outFolderPath != "")
            {//存在输入输出路径
                progressBar.Show();//进度条显示
                string[] filesName = Directory.GetFiles(inFolderPath);//所有文件名
                int mFileNum = filesName.Length;//文件个数
                if (mFileNum == 0)
                {
                    MessageBox.Show("目录下不存在文件！");
                    return;
                }
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
                string fileName = filesName[0];
                //打开hdf文件
                Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                int col = ds.RasterXSize;//列数
                int row = ds.RasterYSize;//行数
                Band demband1 = ds.GetRasterBand(1);//读取波段

                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ds.GetGeoTransform(argout);//读取地理坐标信息

                string[] metadatas = ds.GetMetadata("");//获取元数据
                double startLog = 0.0;//起始经度
                double startLat = 0.0;//起始维度
                double endLog = 0.0;//结束经度
                double endLat = 0.0;//结束维度
                double mScale = 0.0;//比例
                string dataType = "";
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
                        default:
                            break;
                    }
                }
                for (int i = 0; i < mFileNum; i++)
                {//处理每个文件
                    progressBar.Value = (i * 100) / mFileNum;
                    int[] file = new int[row * col];//用来存储一个图像
                    int[] fileResult = new int[row * col];//用来存储结果 
                    fileName = filesName[i];
                    //打开hdf文件
                    ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                    Band demband = ds.GetRasterBand(1);//读取波段
                    demband.ReadRaster(0, 0, col, row, file, col, row, 0, 0);//读取数据
                    List<int> visitedPosList = new List<int>();//记录已经访问的位置
                    for (int j = 0; j < col * row; j++)
                    {//处理每个栅格
                        int rowNow = j / col;//当前行号
                        int colNow = j % col;//当前列号
                        if (rowNow == 0 || colNow == 0 || rowNow == row - 1 || colNow == col - 1) continue;//边界点，不搜索
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

                    string fileNameWithPath = Path.GetFileName(fileName);//获取文件名
                    int pointIndex = fileNameWithPath.LastIndexOf('.');//最后一个.的位置
                    string outfileName = fileNameWithPath.Substring(0, pointIndex) + "_Spatial";

                    //保存为tif格式
                    string outPath = outFolderPath + "\\" + outfileName + ".tif";
                    Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                    Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
                    gTiffDataset.SetMetadata("StartLog", startLog.ToString());
                    gTiffDataset.SetMetadata("EndLog", endLog.ToString());
                    gTiffDataset.SetMetadata("startLat", startLat.ToString());
                    gTiffDataset.SetMetadata("EndLat", endLat.ToString());
                    gTiffDataset.SetMetadata("Scale", "1");
                    gTiffDataset.SetMetadata("FillValue", "255");
                    string imgDate = outfileName.Substring(0, 24);
                    gTiffDataset.SetMetadata("ImageDate", imgDate);
                    gTiffDataset.SetMetadata("DSResoution", "0.1");
                    gTiffDataset.SetMetadata("DataType", dataType);
                    gTiffDataset.WriteRaster(0, 0, col, row, fileResult, col, row, 1, null, 0, 0, 0);
                    gTiffDataset.Dispose();
                }

                progressBar.Hide();//进度条显示
                MessageBox.Show("处理完成!");
            }
        }

        /// <summary>
        /// 暴雨空间维度提取中一个暴雨簇的提取
        /// </summary>
        /// <param name="file"></param>
        /// <param name="gridId"></param>
        /// <param name="col"></param>
        /// <param name="visitedPosList"></param>
        /// <param name="storm"></param>
        /// <param name="stormLevel"></param>
        private void getStorm(int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> storm, ref int[] stormLevel)
        {
            visitedPosList.Add(gridId);//记录被访问
            storm.Add(gridId);//添加到暴雨
            if(file[gridId]==1)
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
                if (nearPos[i] >= 0 && nearPos[i] < file.Length && file[nearPos[i]] > 0)
                {//在图像范围内,且是暴雨
                    bool isVisited = visitedPosList.Contains(nearPos[i]);//该位置是否被访问
                    if (!isVisited)
                    {//没有被访问
                        getStorm(file, nearPos[i], row, col, ref visitedPosList, ref storm, ref stormLevel);
                    }
                }
            }
        }

        /// <summary>
        /// 暴雨编号提取
        /// </summary>
        /// <param name="progressBar">进度条</param>
        public void stormNumberExtraction(ProgressBar progressBar)
        {
            string inFolderPath = "";//输入路径
            string outFolderPath = "";//输出路径
            //选择输入目录
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"E:\";
            fbd.Description = "选择空间数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                //this.textbox1.Text = fbd.SelectedPath;
                inFolderPath = fbd.SelectedPath;
                fbd.Description = "选择输出结果文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    outFolderPath = fbd.SelectedPath;
                }
            }
            if (inFolderPath != "" && outFolderPath != "")
            {//存在输入输出路径
                //string inFolderPath = @"E:\baoYu";
                //string outFolderPath = @"E:\BaoYuTime";
                progressBar.Show();//进度条显示
                //toolStripStatusLabel1.Text = "正在处理...";
                string[] filesName = Directory.GetFiles(inFolderPath);//所有文件名
                int mFileNum = filesName.Length;//文件个数
                if (mFileNum == 0)
                {
                    MessageBox.Show("目录下不存在文件！");
                    return;
                }
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
                string hdfFileName = filesName[0];
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

                //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
                string[] metadatas = ds.GetMetadata("");//获取元数据
                double startLog = 0.0;//起始经度
                double startLat = 0.0;//起始维度
                double endLog = 0.0;//结束经度
                double endLat = 0.0;//结束维度
                double mScale = 0.0;//比例
                string dataType = "";
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
                        default:
                            break;
                    }
                }

                int stormId = 1;//暴雨编号
                List<int[]> filesList = new List<int[]>();//记录所有输入图像
                List<int[]> filesResultList = new List<int[]>();//记录所有结果图像
                List<List<int>> fileIdList = new List<List<int>>();//记录所有图像的id
                int startId = 0;//记录保存图像的第一个id
                for (int i = 0; i < mFileNum; i++)
                {//处理每一个图像
                    progressBar.Value = (i * 100) / mFileNum;
                    //toolStripStatusLabel1.Text = "正在处理 " + i.ToString() + "/" + mFileNum.ToString();
                    int[] file = new int[row * col];//用来存储一个图像
                    hdfFileName = filesName[i];
                    //打开hdf文件
                    ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
                    Band demband = ds.GetRasterBand(1);//读取波段
                    demband.ReadRaster(0, 0, col, row, file, col, row, 0, 0);//读取数据
                    filesList.Add(file);//记录下来
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
                                List<int> usedId = new List<int>();//记录该暴雨在之前时刻的id
                                bool isVisited = visitedPosList.Contains(j);//该位置是否被访问
                                if (!isVisited)
                                {//没有被访问
                                    List<int> storm = new List<int>();//一个暴雨
                                    getStorm(ref filesResultList, startId, i, ref file, j, row, col, ref visitedPosList, ref storm, ref usedId);
                                    if (usedId.Count > 0)
                                    {
                                        for (int k = 0; k < storm.Count; k++)
                                        {//给暴雨赋id
                                            fileResult[storm[k]] = usedId[0];
                                        }
                                        if (usedId.Count > 1)
                                        {//存在多个id
                                            for (int k = 0; k < filesResultList.Count; k++)
                                            {//遍历之前的结果，赋第一个id
                                                for (int k1 = 0; k1 < filesResultList.Count; k1++)
                                                {//每个栅格
                                                    if (usedId.Contains(filesResultList[k][k1]))
                                                    {//被使用的id赋为相同的id
                                                        filesResultList[k][k1] = usedId[0];
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
                                            fileIds.Add(stormId);
                                            stormId++;
                                        }
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
                    for (int j = filesResultList.Count - 2; j >= 0; j--)
                    {//寻找已经不被使用的图像
                        bool isUsed = false;//默认没有被使用
                        foreach (int idValue in fileIdList[j])
                        {
                            if (fileIdList[fileIdList.Count - 1].Contains(idValue))
                            {//还在被使用
                                isUsed = true;
                                break;
                            }
                        }
                        if (!isUsed)
                        {//该图像没有被使用
                            for (int k = j; k >= 0; k--)
                            {//输出本图像及之前的图像
                                string fileName = Path.GetFileName(filesName[startId + k]);//获取文件名
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
                                gTiffDataset.SetMetadata("StartLog", startLog.ToString());
                                gTiffDataset.SetMetadata("EndLog", endLog.ToString());
                                gTiffDataset.SetMetadata("startLat", startLat.ToString());
                                gTiffDataset.SetMetadata("EndLat", endLat.ToString());
                                gTiffDataset.SetMetadata("Scale", "1");
                                gTiffDataset.SetMetadata("FillValue", "255");
                                string imgDate = outfileName.Substring(0, 24);
                                gTiffDataset.SetMetadata("ImageDate", imgDate);
                                gTiffDataset.SetMetadata("DSResoution", "0.1");
                                gTiffDataset.SetMetadata("DataType", dataType);
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
                    string fileName = Path.GetFileName(filesName[startId + i]);//获取文件名
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
                    gTiffDataset.SetMetadata("StartLog", startLog.ToString());
                    gTiffDataset.SetMetadata("EndLog", endLog.ToString());
                    gTiffDataset.SetMetadata("startLat", startLat.ToString());
                    gTiffDataset.SetMetadata("EndLat", endLat.ToString());
                    gTiffDataset.SetMetadata("Scale", "1");
                    gTiffDataset.SetMetadata("FillValue", "255");
                    string imgDate = outfileName.Substring(0, 24);
                    gTiffDataset.SetMetadata("ImageDate", imgDate);
                    gTiffDataset.SetMetadata("DSResoution", "0.1");
                    gTiffDataset.SetMetadata("DataType", dataType);
                    gTiffDataset.WriteRaster(0, 0, col, row, filesResultList[i], col, row, 1, null, 0, 0, 0);
                    gTiffDataset.Dispose();
                }
                progressBar.Hide();
                //toolStripStatusLabel1.Text = "就绪";
                MessageBox.Show("处理完成！");
            }
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
        private void getStorm(ref List<int[]> filesResultList, int startId, int fileIdNow, ref int[] file, int gridId, int row, int col, ref List<int> visitedPosList, ref List<int> storm, ref List<int> usedId)
        {
            visitedPosList.Add(gridId);//记录被访问
            storm.Add(gridId);//添加到暴雨

            for (int i = 1; i <= 6; i++)
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
                        getStorm(ref filesResultList, startId, fileIdNow, ref file, nearPos[i], row, col, ref visitedPosList, ref storm, ref usedId);
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

        //异步委托
        public delegate int[,,] GetFileMC(string[] filesName, int mFileNum, int rowNow, int col, int partRow);//定一个代理
        public int[,,] GetFile(string[] filesName, int mFileNum, int rowNow, int col,int partRow)
        {
            int[,,] files = new int[partRow, col, mFileNum];
            for (int time = 0; time < mFileNum; time++)
            {//读取该行所有数据
                string fileName = filesName[time];
                int[] file = new int[col * partRow];
                Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                Band demband = ds.GetRasterBand(1);//读取波段
                demband.ReadRaster(0, rowNow, col, partRow, file, col, partRow, 0, 0);//读取数据
                ds.Dispose();
                for (int i = 0; i < partRow; i++)
                {
                    for (int j = 0; j < col; j++)
                    {//保存
                        files[i, j, time] = file[i * col + j];
                    }
                }
            }
            return files;
        }

        /// <summary>
        /// 栅格暴雨提取
        /// </summary>
        /// <param name="progressBar1"></param>
        public void stormRasterExtraction(ProgressBar progressBar1)
        {
            string oriFolderPath = "";//原始数据文件夹
            string spFolderPath = "";//暴雨空间文件夹
            string idFolderPath = "";//暴雨编号文件夹
            string outFolderPath = "";//输出路径
            //选择输入目录
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = @"E:\";
            fbd.Description = "选择原始数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                //this.textbox1.Text = fbd.SelectedPath;
                oriFolderPath = fbd.SelectedPath;
                fbd.Description = "选择空间数据文件夹";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    spFolderPath = fbd.SelectedPath;
                    fbd.Description = "选择编号数据文件夹";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        idFolderPath = fbd.SelectedPath;
                        fbd.Description = "选择输出结果文件夹";
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            outFolderPath = fbd.SelectedPath;
                        }
                    }
                }
            }
            if (outFolderPath != "")
            {//存在输出路径
                progressBar1.Show();//进度条显示
                //toolStripStatusLabel1.Text = "正在处理...";
                string[] oriFilesName = Directory.GetFiles(oriFolderPath);//所有文件名
                string[] spFilesName = Directory.GetFiles(spFolderPath);//所有文件名
                string[] idFilesName = Directory.GetFiles(idFolderPath);//所有文件名
                int mFileNum = oriFilesName.Length;//文件个数
                if (mFileNum == 0)
                {
                    MessageBox.Show("目录下不存在文件！");
                    return;
                }
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
                string hdfFileName = oriFilesName[0];
                //string hdfFileName = @"E:\BaoYuTime\XM_2014_CJ_tif";
                //打开hdf文件
                Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
                int col = ds.RasterXSize;//列数
                int row = ds.RasterYSize;//行数
                Band demband1 = ds.GetRasterBand(1);//读取波段

                double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
                ds.GetGeoTransform(argout);//读取地理坐标信息
                //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
                string[] metadatas = ds.GetMetadata("");//获取元数据
                double startLog = 0.0;//起始经度
                double startLat = 0.0;//起始维度
                double endLog = 0.0;//结束经度
                double endLat = 0.0;//结束维度
                double mScale = 0.0;//比例
                double resolution = 0.0;//分辨率
                string dataType = "";
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

                string outPath = outFolderPath + "\\baoYuRaster.txt";
                FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine("OID 经度 纬度 事件ID 事件名称 类型 起始时间 终止时间 持续时间 平均降雨量 累计降雨量 瞬时最大降雨强度 降雨强度");
                int oid = 0;
                //DateTime startT = DateTime.Now;
                TimeSpan t1 = new TimeSpan();
                TimeSpan t2 = new TimeSpan();
                int rowPart = 50;//每一部分行数
                for (int rowBig = 0; rowBig < row; rowBig += rowPart)
                {//逐行
                    progressBar1.Value = (rowBig * 100) / row;
                    //DateTime startT1 = DateTime.Now;
                    GetFileMC gf1 = new GetFileMC(GetFile);
                    IAsyncResult result1 = gf1.BeginInvoke(oriFilesName, mFileNum, rowBig, col, rowPart, null, null);

                    GetFileMC gf2 = new GetFileMC(GetFile);
                    IAsyncResult result2 = gf2.BeginInvoke(spFilesName, mFileNum, rowBig, col, rowPart, null, null);

                    GetFileMC gf3 = new GetFileMC(GetFile);
                    IAsyncResult result3 = gf3.BeginInvoke(idFilesName, mFileNum, rowBig, col, rowPart, null, null);


                    int[,,] oriFiles = gf1.EndInvoke(result1);//用于接收返回值 
                    int[,,] spFiles = gf2.EndInvoke(result2);//用于接收返回值 
                    int[,,] idFiles = gf3.EndInvoke(result3);//用于接收返回值 
                    //t1 =t1.Add(DateTime.Now - startT1);

                    /*
                    int[,,] oriFiles = new int[10, col, mFileNum];
                    int[,,] spFiles = new int[10, col, mFileNum];
                    int[,,] idFiles = new int[10, col, mFileNum];
                    for (int time = 0; time < mFileNum; time++)
                    {//读取该行所有数据
                        string oriFileName = oriFilesName[time];
                        string spFileName = spFilesName[time];
                        string idFileName = idFilesName[time];

                        int[] oriFile = new int[col*10];
                        int[] spFile = new int[col*10];
                        int[] idFile = new int[col*10];

                        ds = Gdal.Open(oriFileName, Access.GA_ReadOnly);
                        Band demband = ds.GetRasterBand(1);//读取波段
                        demband.ReadRaster(0, rowBig, col, 10, oriFile, col, 10, 0, 0);//读取数据
                        ds.Dispose();

                        ds = Gdal.Open(spFileName, Access.GA_ReadOnly);
                        demband = ds.GetRasterBand(1);//读取波段
                        demband.ReadRaster(0, rowBig, col, 10, spFile, col, 10, 0, 0);//读取数据
                        ds.Dispose();

                        ds = Gdal.Open(idFileName, Access.GA_ReadOnly);
                        demband = ds.GetRasterBand(1);//读取波段
                        demband.ReadRaster(0, rowBig, col, 10, idFile, col, 10, 0, 0);//读取数据
                        ds.Dispose();
                        for(int i=0;i<10;i++)
                        {
                            for (int j = 0; j < col; j++)
                            {//保存
                                oriFiles[i, j, time] = oriFile[i * col + j];
                                spFiles[i, j, time] = spFile[i * col + j];
                                idFiles[i, j, time] = idFile[i * col + j];
                            }
                        }
                    }
                    */
                    //DateTime startT2 = DateTime.Now;
                    for (int rowNow = 0; rowNow < rowPart; rowNow++)
                    {
                        for (int colNow = 0; colNow < col; colNow++)
                        {//逐列
                            if ((rowNow + rowBig) == 0 || colNow == 0 || (rowNow + rowBig) >= row - 1 || colNow >= col - 1) continue;//边界点，不搜索
                            bool isStormNow = false;//记录当前暴雨状态
                            int startTime = 0;//暴雨开始时间
                            int endTime = 0;//暴雨结束时间
                            double jiangYuLiang = 0.0;//总降雨量
                            double maxJiangYuLiang = 0.0;//最大降雨量
                            int baoYuCount1 = 0;//暴雨个数
                            int baoYuCount2 = 0;//大暴雨个数
                            int baoYuCount3 = 0;//特大暴雨个数
                            for (int time = 0; time < mFileNum; time++)
                            {//按时间顺序处理
                                if (!isStormNow)
                                {//当前不是暴雨，寻找暴雨开始点
                                    if (spFiles[rowNow, colNow, time] > 0)
                                    {//暴雨点
                                        startTime = time;
                                        isStormNow = true;
                                        jiangYuLiang = oriFiles[rowNow, colNow, time];
                                        maxJiangYuLiang = oriFiles[rowNow, colNow, time];
                                        baoYuCount1 = 0;//暴雨个数
                                        baoYuCount2 = 0;//大暴雨个数
                                        baoYuCount3 = 0;//特大暴雨个数
                                        if (spFiles[rowNow, colNow, time] == 1)
                                        {
                                            baoYuCount1++;
                                        }
                                        else if (spFiles[rowNow, colNow, time] == 2)
                                        {
                                            baoYuCount2++;
                                        }
                                        else if (spFiles[rowNow, colNow, time] == 3)
                                        {
                                            baoYuCount3++;
                                        }
                                    }
                                }
                                else
                                {//当前是暴雨
                                    if (spFiles[rowNow, colNow, time] <= 0 || time == (mFileNum - 1))
                                    {//非暴雨点
                                        isStormNow = false;
                                        if (spFiles[rowNow, colNow, time] <= 0)
                                        {
                                            endTime = time - 1;
                                        }
                                        else
                                        {
                                            endTime = time;
                                        }
                                        double durTime = (endTime + 1 - startTime) * 0.5;//持续时间
                                        double log = startLog + (colNow + 0.5) * resolution;//经度
                                        double lat = endLat - (rowBig + rowNow + 0.5) * resolution;//维度
                                        int stormId = idFiles[rowNow, colNow, endTime];//暴雨id
                                        jiangYuLiang = jiangYuLiang * mScale / 2;//除以2
                                        double avgJiangYuLiang = Math.Round(jiangYuLiang * 2 / (endTime + 1 - startTime), 4);//平均降雨量，注意原始数据是半小时的
                                        maxJiangYuLiang = maxJiangYuLiang * mScale;
                                        int stormMode = 1;
                                        string stormModeString = "暴雨";
                                        if ((baoYuCount2 + baoYuCount3) >= baoYuCount1)
                                        {
                                            if (baoYuCount3 >= baoYuCount2)
                                            {//特大暴雨
                                                stormMode = 3;
                                                stormModeString = "特大暴雨";
                                            }
                                            else
                                            {//大暴雨
                                                stormMode = 2;
                                                stormModeString = "大暴雨";
                                            }
                                        }
                                        string startTimeFormat = Path.GetFileName(oriFilesName[startTime]).Substring(0, 8) + "_" + Path.GetFileName(oriFilesName[startTime]).Substring(10, 6);
                                        string endTimeFormat = Path.GetFileName(oriFilesName[endTime]).Substring(0, 8) + "_" + Path.GetFileName(oriFilesName[endTime]).Substring(18, 6);
                                        sw.WriteLine(oid.ToString() + " " + log.ToString() + " " + lat.ToString() + " " + stormId.ToString() + " 暴雨事件 " + stormModeString + " " + startTimeFormat + " " + endTimeFormat + " " + durTime.ToString() + " " + avgJiangYuLiang.ToString() + " " + jiangYuLiang.ToString() + " " + maxJiangYuLiang.ToString() + " " + stormMode.ToString());
                                        oid++;
                                    }
                                    else
                                    {//暴雨点
                                        jiangYuLiang += oriFiles[rowNow, colNow, time];//累积降雨量
                                        if (maxJiangYuLiang < oriFiles[rowNow, colNow, time])
                                        {//判断是否降雨量更大
                                            maxJiangYuLiang = oriFiles[rowNow, colNow, time];
                                        }
                                        if (spFiles[rowNow, colNow, time] == 1)
                                        {
                                            baoYuCount1++;
                                        }
                                        else if (spFiles[rowNow, colNow, time] == 2)
                                        {
                                            baoYuCount2++;
                                        }
                                        else if (spFiles[rowNow, colNow, time] == 3)
                                        {
                                            baoYuCount3++;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    //t2=t2.Add(DateTime.Now - startT2);
                }
                //DateTime endT = DateTime.Now;
                //TimeSpan et = endT - startT;
                //MessageBox.Show(et.TotalSeconds.ToString()+"\nt1:"+t1.TotalSeconds.ToString() + "\nt2:" + t2.TotalSeconds.ToString());
                sw.Close();
                fs.Close();
                progressBar1.Hide();
                MessageBox.Show("处理完成！");
            }
        }

        public void stormDatabase()
        {
            StormDatabaseForm sdf = new StormDatabaseForm();
            sdf.Show();
        }
    }
}
