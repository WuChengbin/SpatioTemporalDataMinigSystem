using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms.ProcessHDFByGdal
{
    // 定义HDF4 SD接口函数库的文件位置
    
    class HDF4Operator
    {
        public const string MFHDF4_DLL = "mfhdf.dll";
        // 引入SDstart方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDstart", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDstart(string filename, int access_mode);

        // 引入SDfindattr方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDfindattr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDfindattr(int obj_id, string attr_name);

        // 引入SDreadattr方法（字符串类型属性）
        [DllImport(MFHDF4_DLL, EntryPoint = "SDreadattr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDreadattr(int obj_id, int attr_index, StringBuilder attr_buffer);

        // 引入SDreadattr方法（单精度浮点类型属性）
        [DllImport(MFHDF4_DLL, EntryPoint = "SDreadattr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDreadattr(int obj_id, int attr_index, float[] attr_buffer);

        // 引入SDnametoindex方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDnametoindex", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDnametoindex(int sd_id, string sds_name);

        // 引入SDselect方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDselect", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDselect(int sd_id, int sds_index);

        // 引入SDgetinfo方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDgetinfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDgetinfo(int sds_id, StringBuilder sds_name, int[] rank, int[] dimsizes, int[] ntype, int[] num_attrs);

        // 引入SDreaddata方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDreaddata", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDreaddata(int sds_id, int[] start, int[] stride, int[] edge, short[,] buffer);

        // 引入SDendaccess方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDendaccess", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDendaccess(int sds_id);

        // 引入SDend方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDend", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDend(int sd_id);

        // 引入SDcreate方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDcreate", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDcreate(int fid, string name, int nt, int rank, int []dimsizes);

        // 引入SDwritedata方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDwritedata", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDwritedata(int sdsid, int [] start, int[] stride, int[] end, int[] data);

        // 引入SDsetattr方法
        [DllImport(MFHDF4_DLL, EntryPoint = "SDsetattr", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDsetattr(int id, string name, int nt, int count, string data);
        
        /// <summary>
        /// 获取行数
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static int GetDatasetsRow(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            return ds.RasterYSize;
        }

        /// <summary>
        /// 获取列数
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static int GetDatasetsCol(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            return ds.RasterXSize;
        }

        /// <summary>
        /// 获取数据集名称//没写出来。。。
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static string GetDatasetsName(string hdfFileName)
        {
            int fid = SDstart(hdfFileName, 1);//4:DFACC_CREATE
            int sid = SDselect(fid, 0);
            // 数据集名称
            StringBuilder sds_name = new StringBuilder();
            // 秩数
            int[] rank = new int[1];
            // 行列数
            int[] dimsizes = new int[2];
            // 数据类型
            int[] ntype = new int[1];
            // 属性数目
            int[] num_attrs = new int[1];
            int status = SDgetinfo(sid, sds_name, rank, dimsizes, ntype, num_attrs);
            

            return sds_name.ToString();
        }

        /// <summary>
        /// 获取驱动
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static Driver GetDatasetsDriver(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            Driver driver = ds.GetDriver();
            return driver;
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static DataType GetDatasetsFileDataType(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            Band band = ds.GetRasterBand(1);
            DataType datatype = band.DataType;
            return datatype;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double[] GetDatasetsData(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            
            int col = ds.RasterXSize;
            int row = ds.RasterYSize;
            double[] data = new double[row * col];
            Band band = ds.GetRasterBand(1);
            band.ReadRaster(0, 0, col, row, data, col, row, 0, 0);//读取数据
            ds.Dispose();
            return data;
        }

        /// <summary>
        /// 获取开始经度
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsStartLog(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLog = 0.0;//起始经度
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }
            return startLog;
        }

        /// <summary>
        /// 获取数据时间
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static string GetDatasetsImageDate(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            string ImageData = "";//数据时间
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "ImageDate":
                        ImageData = mdArr[1];//数据时间
                        break;
                    default:
                        break;
                }
            }
            return ImageData;
        }

        /// <summary>
        /// 获取结束经度
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsEndLog(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double endLog = 0.0;//结束经度
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }
            return endLog;
        }

        /// <summary>
        /// 获取开始纬度
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsStartLat(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLat = 0.0;//起始维度
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }
            return startLat;
        }

        /// <summary>
        /// 获取结束纬度
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsEndLat(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double endLat = 0.0;//结束维度
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }
            return endLat;
        }

        /// <summary>
        /// 获取截距
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsOffsets(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double Offsets = 0.0;//结束维度
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Offsets":
                        Offsets = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    default:
                        break;
                }
            }
            return Offsets;
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static string GetDatasetsDataType(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            string dataType = "";//数据类型
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "DataType":
                        dataType = Convert.ToString(mdArr[1]);//数据类型
                        break;
                    default:
                        break;
                }
            }
            return dataType;
        }

        /// <summary>
        /// 获取比例系数
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsScale(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double scale = 0.0;//比例
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "Scale":
                        scale = Convert.ToDouble(mdArr[1]);//数据类型
                        break;
                    default:
                        break;
                }
            }
            return scale;
        }

        /// <summary>
        /// 获取缺省值
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsFillValue(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double FillValue = 0.0;//比例
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "FillValue":
                        FillValue = Convert.ToDouble(mdArr[1]);//数据类型
                        break;
                    default:
                        break;
                }
            }
            return FillValue;
        }

        /// <summary>
        /// 获取分辨率
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <returns></returns>
        public static double GetDatasetsResolution(string hdfFileName)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double Resolution = 0.0;//比例
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "DSResolution":
                        Resolution = Convert.ToDouble(mdArr[1]);//数据类型
                        break;
                    default:
                        break;
                }
            }
            return Resolution;
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="hdfFileName">文件路径</param>
        /// <param name="driver">驱动</param>
        /// <param name="row">行</param>
        /// <param name="col">列</param>
        /// <param name="data">数据数组</param>
        /// <param name="ImageData">数据时间</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="Scale">比例系数</param>
        /// <param name="Offsets">偏移量</param>
        /// <param name="StartLog">起始经度</param>
        /// <param name="EndLog">结束经度</param>
        /// <param name="StartLat">起始纬度</param>
        /// <param name="EndLat">结束纬度</param>
        /// <param name="FillValue">缺省值</param>
        /// <param name="MaxValue">最大值</param>
        /// <param name="MinValue">最小值</param>
        /// <param name="DSResolution">分辨率</param>
        public static void write(string hdfFileName, Driver driver, int row, int col, int[] data,string ImageData,string dataType,double Scale,double Offsets,double StartLog,double EndLog,double StartLat,double EndLat,double FillValue, double MaxValue, double MinValue,double DSResolution)
        {
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //Driver driver = Gdal.GetDriverByName("GTiff");
            int[] bandArray = new int[1];
            bandArray[0] = 1;

            Dataset ds = driver.Create(hdfFileName, col, row, 1, DataType.GDT_Int32, null);
            ds.WriteRaster(0, 0, col, row, data, col, row, 1, bandArray, 0, 0, 0);//写入数据
            Band band1 = ds.GetRasterBand(1);
  
            ds.SetMetadataItem("ImageData", ImageData, "");
            ds.SetMetadataItem("DataType", dataType, "");
            ds.SetMetadataItem("Scale", Scale.ToString(), "");
            ds.SetMetadataItem("Offsets", Offsets.ToString(), "");
            ds.SetMetadataItem("StartLog", StartLog.ToString(), "");
            ds.SetMetadataItem("EndLog", EndLog.ToString(), "");
            ds.SetMetadataItem("StartLat", StartLat.ToString(), "");
            ds.SetMetadataItem("EndLat", EndLat.ToString(), "");
            ds.SetMetadataItem("Rows", row.ToString(), "");
            ds.SetMetadataItem("Cols", col.ToString(), "");
            ds.SetMetadataItem("FillValue", FillValue.ToString(), "");
            ds.SetMetadataItem("MaxValue", MaxValue.ToString(), "");
            ds.SetMetadataItem("MinValue", MinValue.ToString(), "");
            ds.SetMetadataItem("DSResolution", DSResolution.ToString(), "");

            MessageBox.Show("写入成功！");
        }

        public static void WriteCustomHDF2DFile(string Filename, string m_ImageDate,
                                            string m_ProductType, string m_DataType,
                                            string m_DsName, int [] pBuffer, double m_Scale,
                                            double m_Offset, double m_StartLog, double m_EndLog,
                                            double m_StartLat, double m_EndLat, int m_Rows,
                                            int m_Cols, double m_MaxValue, double m_MinValue,
                                            double m_MeanValue, double m_StdValue, int m_FillValue,
                                            double m_Resolution, string m_Dimension)
        {
            //成像时间     DFNT_CHAR8
            //产品类型     DFNT_CHAR8
            //数据类型     DFNT_CHAR8 
            //纬度	       DFNT_CHAR8 
            //数据集名     DFNT_INT32
            //比例系数     DFNT_FLOAT64
            //比例截距     DFNT_FLOAT64
            //起始经度     DFNT_FLOAT64
            //终止经度     DFNT_FLOAT64
            //起始纬度     DFNT_FLOAT64 
            //终止纬度     DFNT_FLOAT64
            //行数	       DFNT_UINT16,
            //列数	       DFNT_UINT16
            //最大值       DFNT_FLOAT64
            //最小值       DFNT_FLOAT64
            //均值         DFNT_FLOAT64 
            //方差         DFNT_FLOAT64
            //空值         DFNT_UINT16 
            //空间分辨率   DFNT_ FLOAT64

            int fid = SDstart(Filename, 4);//4:DFACC_CREATE
            if (fid == -1) return;

            int[] startWrite = new int[2];
            int[] dimesizeWrite = new int [2];

            //创建数据集id
            dimesizeWrite[0] = m_Rows;
            dimesizeWrite[1] = m_Cols;
            int status;
            int sid = SDcreate(fid, m_DsName, 24, 2, dimesizeWrite);//24指INT32
            if (sid == -1)
            {
                //关闭打开hdf文件			
                status = SDend(fid);
                return;
            }

            //写入数据集
            startWrite[0] = 0;
            startWrite[1] = 0;

            status = SDwritedata(sid, startWrite, null, dimesizeWrite, pBuffer);
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //写入属性信息
            //数据集日期
            status = SDsetattr(fid, "ImageDate", 4, 50, m_ImageDate);//4:DFNT_CHAR8
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //数据类型信息
            status = SDsetattr(fid, "DataType", 4, 50, m_DataType);//4:DFNT_CHAR8
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //产品类型信息
            string tagChar = "Product";
            status = SDsetattr(fid, "ProductType", 4, 50, tagChar);
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //维度信息
            status = SDsetattr(fid, "Dimension", 4, 50, m_Dimension);
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //写入数据集的属性信息
            //转换比例和截距
            string Scale = "0.01";
            status = SDsetattr(sid, "Scale", 4, 50, Scale);//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            double mOffsets = 0.00;
            status = SDsetattr(sid, "Offsets", 4, 50, mOffsets.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //起始经纬度
            status = SDsetattr(sid, "StartLog", 4, 50, m_StartLog.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "EndLog", 4, 50, m_EndLog.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "StartLat", 4, 50, m_StartLat.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "EndLat", 4, 50, m_EndLat.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //数据集的行列数
            status = SDsetattr(sid, "Rows", 4, 50, m_Rows.ToString());//23:DFNT_UINT16
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "Cols", 4, 50, m_Cols.ToString());//23:DFNT_UINT16
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //填充值
            status = SDsetattr(sid, "FillValue", 4, 50, m_FillValue.ToString());//24:DFNT_INT32
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //数据集的最大值和最小值
            //double minValue, maxValue;
            //double  *tempValue = new double[2];
            //tempValue = CClsGeneralOperator::GetMin_Max(pBuffer, m_Rows, m_Cols);
            //minValue = tempValue[0]; m_MaxValue = tempValue[1];

            status = SDsetattr(sid, "MaxValue", 4, 50, m_MaxValue.ToString());//6:DFNT_FLOAT64
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "MinValue", 4, 50, m_MinValue.ToString());
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }
            //数据集的平均值和标准差
            status = SDsetattr(sid, "MeanVaue", 4, 50, m_MeanValue.ToString());
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            status = SDsetattr(sid, "StdValue", 4, 50, m_StdValue.ToString());
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }
            //写入数据集分辨率
            status = SDsetattr(sid, "DSResolution", 4, 50, m_Resolution.ToString());
            if (status == -1)
            {
                //关闭打开hdf文件
                status = SDendaccess(sid);
                status = SDend(fid);
                return;
            }

            //关闭打开hdf文件
            status = SDendaccess(sid);
            status = SDend(fid);

            return;
        }

    }

}
