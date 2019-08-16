using MarineSTMiningSystem.GUI;
using Oracle.ManagedDataAccess.Client;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class MainForm : Form
    {
        OracleConnection conn = new OracleConnection();//数据库连接,数据库相关操作请使用此连接

        public MainForm()
        {
            InitializeComponent();
            //DateTime t1 = DateTime.Now;
            //DateTime t2 = DateTime.Now;
            //TimeSpan t = t2.Subtract(t1);
        }

        [DllImport("ExportDllTest.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "Add")]
        extern static int Add(int a, int b);

        private void aboutUs_Click(object sender, EventArgs e)
        {//关于我们
            Gdal.AllRegister();
            Dataset GeoTiffDataset = Gdal.Open(@"C:\Users\Administrator\Desktop\Space\20160719-S143000-E145959Precipitation_Resample_Time_Spatial.tif", Access.GA_ReadOnly);
            //Dataset GeoTiffDataset = Gdal.Open(@"C:\Users\Administrator\Desktop\test.tiff", Access.GA_ReadOnly);
            Band tiffBand = GeoTiffDataset.GetRasterBand(1);
            double[] argout = new double[6];
            GeoTiffDataset.GetGeoTransform(argout);
            int col = GeoTiffDataset.RasterXSize;
            int row = GeoTiffDataset.RasterYSize;
            int[] pBuffer = new int[GeoTiffDataset.RasterXSize * GeoTiffDataset.RasterYSize];
            tiffBand.ReadRaster(0, 0, col, row, pBuffer, col, row, 0, 0);//读取数据

            OSGeo.GDAL.Driver shpDriver = Gdal.GetDriverByName("ESRI Shapefile");
            OSGeo.OGR.Driver shpDriver1= Ogr.GetDriverByName("ESRI Shapefile");
            if (shpDriver != null)
            {
                Dataset outShp = shpDriver.Create(@"C:\Users\Administrator\Desktop\test.shp", 0, 0,0, DataType.GDT_Unknown, null);
                DataSource srShp = shpDriver1.CreateDataSource(@"C:\Users\Administrator\Desktop\test1.shp", null);
                if (srShp != null)
                {
                    OSGeo.OSR.SpatialReference shpSR = new OSGeo.OSR.SpatialReference(GeoTiffDataset.GetProjectionRef());
                    Layer shpLayer = srShp.CreateLayer("TestLayer", shpSR, wkbGeometryType.wkbPolygon, null);
                    FieldDefn oField = new FieldDefn("value", FieldType.OFTInteger);
                    shpLayer.CreateField(oField, 0);
                    Gdal.Polygonize(tiffBand, tiffBand, shpLayer, 0, null, null, null);
                }
                GeoTiffDataset.Dispose();
                srShp.Dispose();
            }

            //OSGeo.GDAL.Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
            //Dataset gTiffDataset = gTiffRriver.Create(@"C:\Users\Administrator\Desktop\test.tiff", col, row, 1, DataType.GDT_Int32, null);
            //gTiffDataset.SetGeoTransform(argout);
            //gTiffDataset.SetProjection(GeoTiffDataset.GetProjection());
            //gTiffDataset.GetRasterBand(1).SetNoDataValue(0);
            //gTiffDataset.WriteRaster(0, 0, GeoTiffDataset.RasterXSize, GeoTiffDataset.RasterYSize, pBuffer, GeoTiffDataset.RasterXSize, GeoTiffDataset.RasterYSize, 1, null, 0, 0, 0);
            //gTiffDataset.Dispose();

            ////hdf转tif 添加坐标系
            //   Gdal.AllRegister();//注册所有的格式驱动
            //   Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //                                                       //int mFileNum = files.Length;//文件个数
            //   string[] files = Directory.GetFiles(@"E:\Storm\20160530-1001-China-product", "*.hdf");
            //   //string hdfFileName = @"E:\Storm\20160530-1001-China-product\20160625-S000000-E002959Precipitation_Resample.hdf";
            //   string outFolder = @"E:\Storm\20160530-1001-China-product_tif\";
            //   for (int i = 0; i < files.Length; i++)
            //   {
            //       string hdfFileName = files[i];
            //       Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            //       int col = ds.RasterXSize;//列数
            //       int row = ds.RasterYSize;//行数
            //       Band demband1 = ds.GetRasterBand(1);//读取波段
            //       double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            //       ds.GetGeoTransform(argout);//读取地理坐标信息
            //       string[] metadatas = ds.GetMetadata("");//获取元数据
            //       double startLog = 70.0;//起始经度
            //       double startLat = 0.0;//起始维度
            //       double endLog = 140.0;//结束经度
            //       double endLat = 60.0;//结束维度
            //       double mScale = 1.0;//比例
            //       string dataType = "";
            //       double resolution = 0.1;//每像素宽度
            //       foreach (string md in metadatas)
            //       {//获取信息
            //           string[] mdArr = md.Split('=');
            //           switch (mdArr[0])
            //           {
            //               case "StartLog":
            //                   startLog = Convert.ToDouble(mdArr[1]);//起始经度
            //                   break;
            //               case "StartLat":
            //                   startLat = Convert.ToDouble(mdArr[1]);//起始经度
            //                   break;
            //               case "EndLog":
            //                   endLog = Convert.ToDouble(mdArr[1]);//起始经度
            //                   break;
            //               case "EndLat":
            //                   endLat = Convert.ToDouble(mdArr[1]);//起始经度
            //                   break;
            //               case "Scale":
            //                   mScale = Convert.ToDouble(mdArr[1]);//值比例
            //                   break;
            //               case "DataType":
            //                   dataType = mdArr[1];//
            //                   break;
            //               case "DSResolution":
            //                   resolution = Convert.ToDouble(mdArr[1]);//值比例
            //                   break;
            //               default:
            //                   break;
            //           }
            //       }
            //       argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };
            //       int[] databuf = new int[col * row];//存储该切割部分的数组
            //       demband1.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据

            //       demband1.Dispose();
            //       ds.Dispose();

            //       //保存为tif格式
            //       //string outPath = @"E:\Storm\20160530-1001-China-product_tif\20160625-S000000-E002959.tif";
            //       string fileName = Path.GetFileNameWithoutExtension(hdfFileName);
            //       string outPath = outFolder + fileName + ".tif";
            //       OSGeo.GDAL.Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
            //       Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
            //       //gTiffDataset.SetGeoTransform(argout);//地理坐标信息
            //       string projection = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";

            //       gTiffDataset.SetGeoTransform(argout);//地理坐标信息
            //       gTiffDataset.SetProjection(projection);//设置坐标系
            //       gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
            //       gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
            //       gTiffDataset.SetMetadataItem("startLat", startLat.ToString(), null);
            //       gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
            //       gTiffDataset.SetMetadataItem("Scale", mScale.ToString(), null);
            //       gTiffDataset.SetMetadataItem("FillValue", "0", null);
            //       gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
            //       gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
            //       gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
            //       gTiffDataset.SetMetadataItem("Offsets", "0", null);
            //       gTiffDataset.SetMetadataItem("MinValue", "0", null);
            //       gTiffDataset.WriteRaster(0, 0, col, row, databuf, col, row, 1, null, 0, 0, 0);
            //       gTiffDataset.Dispose();
            //       Console.WriteLine(i + "/" + files.Length);
            //   }

            //   MessageBox.Show("成功");

            //int[] a1 = new int[7];
            //int[] b1 = new int[7];
            //int[] c1 = new int[7];
            //int[] a2 = new int[7];
            //int[] b2 = new int[7];
            //int[] c2 = new int[7];
            //int[] a1 = new int[4];
            //int[] b1 = new int[4];
            //int[] c1 = new int[4];
            //int a1Sum = 0;
            //int b1Sum = 0;
            //int c1Sum = 0;

            //int[] a2 = new int[4];
            //int[] b2 = new int[4];
            //int[] c2 = new int[4];
            //int stationCount = 0;
            //List<int> stationIds = new List<int>();

            //string oriPath = @"E:\Storm\20160531-1001-China-day";
            //string[] files = Directory.GetFiles(oriPath, "*.tif");
            //Gdal.AllRegister();//注册所有的格式驱动
            //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            //int mFileNum = files.Length;//文件个数
            //string hdfFileName = files[0];
            //Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            //int col = ds.RasterXSize;//列数
            //int row = ds.RasterYSize;//行数
            //Band demband1 = ds.GetRasterBand(1);//读取波段
            //double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            //ds.GetGeoTransform(argout);//读取地理坐标信息
            //string[] metadatas = ds.GetMetadata("");//获取元数据
            //double startLog = 70.0;//起始经度
            //double startLat = 0.0;//起始维度
            //double endLog = 140.0;//结束经度
            //double endLat = 60.0;//结束维度
            //double mScale = 1.0;//比例
            //string dataType = "";
            //double resolution = 0.1;//每像素宽度
            //foreach (string md in metadatas)
            //{//获取信息
            //    string[] mdArr = md.Split('=');
            //    switch (mdArr[0])
            //    {
            //        case "StartLog":
            //            startLog = Convert.ToDouble(mdArr[1]);//起始经度
            //            break;
            //        case "StartLat":
            //            startLat = Convert.ToDouble(mdArr[1]);//起始经度
            //            break;
            //        case "EndLog":
            //            endLog = Convert.ToDouble(mdArr[1]);//起始经度
            //            break;
            //        case "EndLat":
            //            endLat = Convert.ToDouble(mdArr[1]);//起始经度
            //            break;
            //        case "Scale":
            //            mScale = Convert.ToDouble(mdArr[1]);//值比例
            //            break;
            //        case "DataType":
            //            dataType = mdArr[1];//
            //            break;
            //        case "DSResolution":
            //            resolution = Convert.ToDouble(mdArr[1]);//值比例
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };
            //demband1.Dispose();
            //ds.Dispose();

            ////查出所有站点
            //string sql = "select * from station_use";
            //DataTable stationTable = QueryResultTable(sql);

            //for (int i = 0; i < files.Length; i++)
            //{
            //    string path = files[i];
            //    string fileName = Path.GetFileName(path);
            //    int year = Convert.ToInt32(fileName.Substring(0, 4));
            //    int month = Convert.ToInt32(fileName.Substring(4, 2).TrimStart('0'));
            //    //if (month != 8) continue;
            //    int day = Convert.ToInt32(fileName.Substring(6, 2).TrimStart('0'));
            //    ds = Gdal.Open(path, Access.GA_ReadOnly);
            //    Band demband = ds.GetRasterBand(1);//读取波段
            //    int[] databuf = new int[col * row];//存储该切割部分的数组
            //    demband.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据
            //    demband.Dispose();
            //    ds.Dispose();

            //    for (int oid = 0; oid < stationTable.Rows.Count; oid++)
            //    {//每个站点
            //        int stationId = Convert.ToInt32(stationTable.Rows[oid][0]);
            //        string province = stationTable.Rows[oid][1].ToString();
            //        string name = stationTable.Rows[oid][2].ToString();
            //        double log = Convert.ToDouble(stationTable.Rows[oid][3]);
            //        double lat = Convert.ToDouble(stationTable.Rows[oid][4]);
            //        string region = stationTable.Rows[oid][5].ToString();
            //        int regionId = -1;
            //        switch (region)
            //        {
            //            case "东北":
            //                regionId = 0;
            //                break;
            //            case "华北":
            //                regionId = 1;
            //                break;
            //            case "西北":
            //                regionId = 2;
            //                break;
            //            case "华东":
            //                regionId = 3;
            //                break;
            //            case "华中":
            //                regionId = 4;
            //                break;
            //            case "西南":
            //                regionId = 5;
            //                break;
            //            case "华南":
            //                regionId = 6;
            //                break;
            //        }
            //        if (regionId == 0 || regionId == 2 || lat > 40.0) continue;
            //        if (!stationIds.Contains(stationId)) stationIds.Add(stationId);
            //        sql = "select * from rainfall where station_id =" + stationId + " and year=" + year + " and month=" + month + " and day=" + day;
            //        DataTable rainfallTable = QueryResultTable(sql);
            //        if (rainfallTable.Rows.Count == 0) continue;
            //        double rainfallStation = Convert.ToInt32(rainfallTable.Rows[0][9]);

            //        int pRow = (int)Math.Ceiling(DoubleRvide((endLat - lat), resolution)) - 1;//点所在行号
            //        int pCol = (int)Math.Floor(DoubleRvide((log - startLog), resolution));//点所在列号
            //        int pId = pRow * col + pCol;//在图像数组中id
            //        double rainfallImg = Convert.ToDouble(databuf[pId]) * mScale;
            //        if (rainfallImg > 50.0 && rainfallStation > 50.0)
            //        {
            //            //a1[month - 6] += 1;
            //            a1[month - 6] += 1;
            //            a1Sum++;
            //        }
            //        else if (rainfallImg <= 50.0 && rainfallStation > 50.0)
            //        {
            //            b1[month - 6] += 1;
            //            b1Sum++;
            //        }
            //        else if (rainfallImg > 50.0 && rainfallStation <= 50.0)
            //        {
            //            c1[month - 6] += 1;
            //            c1Sum++;
            //        }

            //        DateTime endTime = new DateTime(year, month, day, 20, 0, 0);
            //        DateTime startTime = endTime.AddDays(-1);
            //        sql = "SELECT count(*) FROM (select * from event_state where time >=TO_TIMESTAMP_TZ('" + startTime.ToString("yyyyMMdd_HHmmss") + " +08:00','YYYYMMDD_HH24MISS TZH:TZM ') and time <TO_TIMESTAMP_TZ('" + endTime.ToString("yyyyMMdd_HHmmss") + " +08:00','YYYYMMDD_HH24MISS TZH:TZM ')) e WHERE SDO_RELATE(e.space, sdo_geometry(2001,4326,null,sdo_elem_info_array(1,1,1),sdo_ordinate_array(" + log + "," + lat + ")),'MASK=CONTAINS') = 'TRUE'";
            //        DataTable countTable = QueryResultTable(sql);
            //        int v = Convert.ToInt32(countTable.Rows[0][0]);
            //        if (rainfallStation > 50.0 && v > 0)
            //        {
            //            a2[month - 6] += 1;
            //        }
            //        else if (rainfallStation > 50.0 && v == 0)
            //        {
            //            b2[month - 6] += 1;
            //        }
            //        else if (rainfallStation == 0.0 && v > 0)
            //        {
            //            c2[month - 6] += 1;
            //        }
            //        if (a1Sum > 0)
            //        {
            //            int POD = a1Sum * 100 / (a1Sum + b1Sum);
            //            int FAR = c1Sum * 100 / (a1Sum + c1Sum);
            //            Console.WriteLine(year + " " + month + " " + day + " " + oid + "/" + stationTable.Rows.Count + " a:" + a1Sum + " b:" + b1Sum + " c:" + c1Sum + " POD:" + POD + "% FAR:" + FAR + "%");
            //        }
            //    }
            //    Console.WriteLine(year + " " + month + " " + day);
            //}
            //MessageBox.Show("成功");

            #region 天尺度累加
            /* //天尺度累加
            string oriPath = @"E:\Storm\20160530-1001-China-product";
            string[] files = Directory.GetFiles(oriPath, "*.hdf");
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称
            int mFileNum = files.Length;//文件个数
            string hdfFileName = files[0];
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
            double startLog = 70.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 140.0;//结束经度
            double endLat = 60.0;//结束维度
            double mScale = 1.0;//比例
            string dataType = "";
            double resolution = 0.1;//每像素宽度
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
                        mScale = Convert.ToDouble(mdArr[1]);//值比例
                        break;
                    case "DataType":
                        dataType = mdArr[1];//
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);//值比例
                        break;
                    default:
                        break;
                }
            }
            argout = new double[] { startLog, resolution, 0, endLat, 0, -resolution };
            demband1.Dispose();
            ds.Dispose();

            for (int i = 0; i < mFileNum; i+=48)
            {
                int[] file = new int[col * row];
                for(int j=0;j<48;j++)
                {
                    int fileId = i + j;
                    ds = Gdal.Open(files[fileId], Access.GA_ReadOnly);
                    Band demband = ds.GetRasterBand(1);//读取波段
                    int[] databuf = new int[col * row];//存储该切割部分的数组
                    demband.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据
                    demband.Dispose();
                    ds.Dispose();
                    for(int k=0;k<col*row;k++)
                    {
                        if(databuf[k]>0)
                        {
                            file[k] += databuf[k] / 2;
                        }
                    }
                }

                string fileNameWithPath = Path.GetFileName(files[i+47]);//获取文件名
                string outfileName = fileNameWithPath.Substring(0, 8);

                //保存为tif格式
                string outPath = @"E:\Storm\20160531-1001-China-day\" + outfileName + ".tif";
                OSGeo.GDAL.Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                string projection = "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0],UNIT[\"degree\",0.0174532925199433],AUTHORITY[\"EPSG\",\"4326\"]]";

                gTiffDataset.SetGeoTransform(argout);//地理坐标信息
                gTiffDataset.SetProjection(projection);//设置坐标系
                gTiffDataset.SetMetadataItem("StartLog", startLog.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLog", endLog.ToString(), null);
                gTiffDataset.SetMetadataItem("startLat", startLat.ToString(), null);
                gTiffDataset.SetMetadataItem("EndLat", endLat.ToString(), null);
                gTiffDataset.SetMetadataItem("Scale", mScale.ToString(), null);
                gTiffDataset.SetMetadataItem("FillValue", "0", null);
                gTiffDataset.SetMetadataItem("DSResolution", resolution.ToString(), null);
                gTiffDataset.SetMetadataItem("Rows", row.ToString(), null);
                gTiffDataset.SetMetadataItem("Cols", col.ToString(), null);
                gTiffDataset.SetMetadataItem("Offsets", "0", null);
                gTiffDataset.SetMetadataItem("MinValue", "0", null);
                gTiffDataset.WriteRaster(0, 0, col, row, file, col, row, 1, null, 0, 0, 0);
                gTiffDataset.Dispose();
                Console.WriteLine(outfileName);
            }
            MessageBox.Show("成功");
            */
            #endregion

            /* //结果与站点比较
            int[] a = new int[4];
            int[] b = new int[4];
            int[] c = new int[4];
            string sql = "select * from station_use";
            DataTable stationTable = QueryResultTable(sql);
            for (int oid = 0; oid < stationTable.Rows.Count; oid++)
            {//每个站点
                int stationId = Convert.ToInt32(stationTable.Rows[oid][0]);
                string province = stationTable.Rows[oid][1].ToString();
                string name = stationTable.Rows[oid][2].ToString();
                double log = Convert.ToDouble(stationTable.Rows[oid][3]);
                double lat = Convert.ToDouble(stationTable.Rows[oid][4]);
                sql = "select * from rainfall where station_id =" + stationId;
                DataTable rainfallTable = QueryResultTable(sql);
                for (int i = 0; i < rainfallTable.Rows.Count; i++)
                {//站点每条数据
                    int year = Convert.ToInt32(rainfallTable.Rows[i][4]);
                    int month = Convert.ToInt32(rainfallTable.Rows[i][5]);
                    int day = Convert.ToInt32(rainfallTable.Rows[i][6]);
                    if (month == 6 && day < 5) continue;
                    if (month == 6 && day > 15) continue;
                    double rainfall = Convert.ToInt32(rainfallTable.Rows[i][9]);
                    //DateTime startTime = new DateTime(year, month, day, 20, 0, 0);
                    //startTime=startTime.AddDays(-1);//前一天
                    DateTime time = new DateTime(year, month, day, 8, 0, 0);
                    //DateTime time = endTime.AddHours(-12);
                    //sql = "SELECT count(*) FROM (select * from event_state where time >=TO_TIMESTAMP_TZ('" + startTime.ToString("yyyyMMdd_HHmmss") + "','YYYYMMDD_HH24MISS TZH:TZM ') and time<TO_TIMESTAMP_TZ('" + endTime.ToString("yyyyMMdd_HHmmss") + "','YYYYMMDD_HH24MISS TZH:TZM ')) e WHERE SDO_FILTER(e.space, SDO_GEOMETRY(2001, 4326, sdo_point_type(" + log + "," + lat + ",null), null,null)) = 'TRUE'";
                    sql = "SELECT count(*) FROM (select * from event_state where time =TO_TIMESTAMP_TZ('" + time.ToString("yyyyMMdd_HHmmss") + " +08:00','YYYYMMDD_HH24MISS TZH:TZM ')) e WHERE SDO_RELATE(e.space, SDO_GEOMETRY(2001, 4326, sdo_point_type(" + log + "," + lat + ",null), null,null),'mask=contins') = 'TRUE'";
                    DataTable countTable = QueryResultTable(sql);
                    int v = Convert.ToInt32(countTable.Rows[0][0]);
                    if (rainfall > 50.0)
                    {
                        a[month - 6] += v;
                        b[month - 6] += (1 - v);
                    }
                    else if (rainfall == 0.0)
                    {
                        c[month - 6] += v;
                    }
                    Console.WriteLine(oid + "/" + stationTable.Rows.Count + " " + i + "/" + rainfallTable.Rows.Count);
                    //if (a[month - 6] > 0)
                    //{
                    //    Console.WriteLine(oid + "/" + stationTable.Rows.Count + " a:" + a + " b:" + b + " c:" + c + " POD:" + a * 100 / (a + b) + "% FAR:" + c * 100 / (a + c) + "% CSI:" + a * 100 / (a + b + c) + "% " + province + " " + name+" "+time.ToString("yyyyMMdd"));
                    //}
                    //else
                    //{
                    //    Console.WriteLine(oid + "/" + stationTable.Rows.Count + " a:" + a + " b:" + b + " c:" + c + " " + province + " " + name+" " + time.ToString("yyyyMMdd"));
                    //}
                }
            }
            //Console.WriteLine("a:"+a+";b:"+b+";c:"+c + ";POD:" + a * 100 / (a + b) + "%;FAR:" + c * 100 / (a + c) + "%;CSI:" + a * 100 / (a + b + c) + "%;");
            //MessageBox.Show("a:" + a + ";b:" + b + ";c:" + c + ";POD:" + a * 100 / (a + b) + "%;FAR:" + c * 100 / (a + c) + "%;CSI:" + a * 100 / (a + b + c) + "%;");
            MessageBox.Show("成功");
            /*

            /*
            int a = 0;
            int b = 0;
            int c = 0;
            string sql = "select max(oid) from rainfall";
            DataTable maxOidTable = QueryResultTable(sql);
            int maxOid= Convert.ToInt32(maxOidTable.Rows[0][0]);
            for(int oid = 2;oid<=maxOid;oid++)
            {//每条站点数据
                sql = "select * from rainfall where oid =" + oid;
                DataTable rainfallTable = QueryResultTable(sql);
                for (int i = 0; i < rainfallTable.Rows.Count; i++)
                {//每条站点数据
                    //double log = Convert.ToDouble(dt.Rows[i][2]);
                    //double lat = Convert.ToDouble(dt.Rows[i][3]);
                    int stationId= Convert.ToInt32(rainfallTable.Rows[i][1]);
                    sql = "select * from station where id =" + stationId;
                    DataTable stationTable = QueryResultTable(sql);
                    if(stationTable.Rows.Count==0)
                    {
                        Console.WriteLine("无站点" + stationId);
                        continue;
                    }
                    string province = stationTable.Rows[0][1].ToString();
                    string name = stationTable.Rows[0][2].ToString();
                    double log = Convert.ToDouble(stationTable.Rows[0][3]);
                    double lat = Convert.ToDouble(stationTable.Rows[0][4]);
                    int year = Convert.ToInt32(rainfallTable.Rows[i][4]);
                    int month = Convert.ToInt32(rainfallTable.Rows[i][5]);
                    int day = Convert.ToInt32(rainfallTable.Rows[i][6]);
                    double rainfall= Convert.ToInt32(rainfallTable.Rows[i][9]);
                    //DateTime startTime = new DateTime(year, month, day, 20, 0, 0);
                    //startTime=startTime.AddDays(-1);//前一天
                    DateTime time = new DateTime(year, month, day, 8, 0, 0);
                    //DateTime time = endTime.AddHours(-12);
                    //sql = "SELECT count(*) FROM (select * from event_state where time >=TO_TIMESTAMP_TZ('" + startTime.ToString("yyyyMMdd_HHmmss") + "','YYYYMMDD_HH24MISS TZH:TZM ') and time<TO_TIMESTAMP_TZ('" + endTime.ToString("yyyyMMdd_HHmmss") + "','YYYYMMDD_HH24MISS TZH:TZM ')) e WHERE SDO_FILTER(e.space, SDO_GEOMETRY(2001, 4326, sdo_point_type(" + log + "," + lat + ",null), null,null)) = 'TRUE'";
                    sql = "SELECT count(*) FROM (select * from event_state where time =TO_TIMESTAMP_TZ('" + time.ToString("yyyyMMdd_HHmmss") + " +08:00','YYYYMMDD_HH24MISS TZH:TZM ')) e WHERE SDO_FILTER(e.space, SDO_GEOMETRY(2001, 4326, sdo_point_type(" + log + "," + lat + ",null), null,null)) = 'TRUE'";
                    DataTable countTable = QueryResultTable(sql);
                    int v = Convert.ToInt32(countTable.Rows[0][0]);
                    if (rainfall > 30.0)
                    {
                        a += v;
                        b += (1 - v);
                    }
                    else if(rainfall == 0.0)
                    {
                        c += v;
                    }
                    if (a > 0)
                    {
                        Console.WriteLine(oid + "/" + maxOid + " a:" + a + ";b:" + b + ";c:" + c + ";POD:" + a * 100 / (a + b) + "%;FAR:" + c * 100 / (a + c) + "%;CSI:" + a * 100 / (a + b + c) + "%;" + province + " " + name);
                    }
                    else
                    {
                        Console.WriteLine(oid + "/" + maxOid + " a:" + a + ";b:" + b + ";c:" + c + ";" + province + " " + name);
                    }
                }
            }
            Console.WriteLine("a:"+a+";b:"+b+";c:"+c + ";POD:" + a * 100 / (a + b) + "%;FAR:" + c * 100 / (a + c) + "%;CSI:" + a * 100 / (a + b + c) + "%;");
            MessageBox.Show("a:" + a + ";b:" + b + ";c:" + c + ";POD:" + a * 100 / (a + b) + "%;FAR:" + c * 100 / (a + c) + "%;CSI:" + a * 100 / (a + b + c) + "%;");
            */

            ////站点数据入站
            //StreamReader sr = new StreamReader(@"E:\station\2016-6月.txt");
            //string line = sr.ReadLine(); line = sr.ReadLine();
            //int oid = 1;
            //while (line != null)
            //{
            //    string[] values = line.Split();
            //    double log = Convert.ToDouble(values[2]) * 0.01;
            //    double lat = Convert.ToDouble(values[1]) * 0.01;
            //    double t20_8 = Convert.ToDouble(values[7]) * 0.1;
            //    if (t20_8 > 999) t20_8 = 0.0;
            //    double t8_20 = Convert.ToDouble(values[8]) * 0.1;
            //    if (t8_20 > 999) t8_20 = 0.0;
            //    double t20_20 = Convert.ToDouble(values[9]) * 0.1;
            //    if (t20_20 > 999) t20_20 = 0.0;
            //    string sql = "insert into rainfall values(" + oid + "," + values[0] + "," + log + "," + lat + "," + values[4] + "," + values[5] + "," + values[6] + "," + t20_8 + "，" + t8_20 + "," + t20_20 + ")";
            //    OracleCommand inserCmd = new OracleCommand(sql, conn);
            //    inserCmd.ExecuteNonQuery();//执行数据库操作
            //    line = sr.ReadLine();
            //    oid++;
            //}
            //sr = new StreamReader(@"E:\station\2016-7月.txt");
            //line = sr.ReadLine(); line = sr.ReadLine();
            //while (line != null)
            //{
            //    string[] values = line.Split();
            //    double log = Convert.ToDouble(values[2]) * 0.01;
            //    double lat = Convert.ToDouble(values[1]) * 0.01;
            //    double t20_8 = Convert.ToDouble(values[7]) * 0.1;
            //    if (t20_8 > 999) t20_8 = 0.0;
            //    double t8_20 = Convert.ToDouble(values[8]) * 0.1;
            //    if (t8_20 > 999) t8_20 = 0.0;
            //    double t20_20 = Convert.ToDouble(values[9]) * 0.1;
            //    if (t20_20 > 999) t20_20 = 0.0;
            //    string sql = "insert into rainfall values(" + oid + "," + values[0] + "," + log + "," + lat + "," + values[4] + "," + values[5] + "," + values[6] + "," + t20_8 + "，" + t8_20 + "," + t20_20 + ")";
            //    OracleCommand inserCmd = new OracleCommand(sql, conn);
            //    inserCmd.ExecuteNonQuery();//执行数据库操作
            //    line = sr.ReadLine();
            //    oid++;
            //}
            //sr = new StreamReader(@"E:\station\2016-8月.txt");
            //line = sr.ReadLine(); line = sr.ReadLine();
            //while (line != null)
            //{
            //    string[] values = line.Split();
            //    double log = Convert.ToDouble(values[2]) * 0.01;
            //    double lat = Convert.ToDouble(values[1]) * 0.01;
            //    double t20_8 = Convert.ToDouble(values[7]) * 0.1;
            //    if (t20_8 > 999) t20_8 = 0.0;
            //    double t8_20 = Convert.ToDouble(values[8]) * 0.1;
            //    if (t8_20 > 999) t8_20 = 0.0;
            //    double t20_20 = Convert.ToDouble(values[9]) * 0.1;
            //    if (t20_20 > 999) t20_20 = 0.0;
            //    string sql = "insert into rainfall values(" + oid + "," + values[0] + "," + log + "," + lat + "," + values[4] + "," + values[5] + "," + values[6] + "," + t20_8 + "，" + t8_20 + "," + t20_20 + ")";
            //    OracleCommand inserCmd = new OracleCommand(sql, conn);
            //    inserCmd.ExecuteNonQuery();//执行数据库操作
            //    line = sr.ReadLine();
            //    oid++;
            //}
            //sr = new StreamReader(@"E:\station\2016-9月.txt");
            //line = sr.ReadLine(); line = sr.ReadLine();
            //while (line != null)
            //{
            //    string[] values = line.Split();
            //    double log = Convert.ToDouble(values[2]) * 0.01;
            //    double lat = Convert.ToDouble(values[1]) * 0.01;
            //    double t20_8 = Convert.ToDouble(values[7]) * 0.1;
            //    if (t20_8 > 999) t20_8 = 0.0;
            //    double t8_20 = Convert.ToDouble(values[8]) * 0.1;
            //    if (t8_20 > 999) t8_20 = 0.0;
            //    double t20_20 = Convert.ToDouble(values[9]) * 0.1;
            //    if (t20_20 > 999) t20_20 = 0.0;
            //    string sql = "insert into rainfall values(" + oid + "," + values[0] + "," + log + "," + lat + "," + values[4] + "," + values[5] + "," + values[6] + "," + t20_8 + "，" + t8_20 + "," + t20_20 + ")";
            //    OracleCommand inserCmd = new OracleCommand(sql, conn);
            //    inserCmd.ExecuteNonQuery();//执行数据库操作
            //    line = sr.ReadLine();
            //    oid++;
            //}
            //MessageBox.Show("成功");

            ////站点数据入站
            //StreamReader sr = new StreamReader(@"E:\station\station3.txt");
            //string line = sr.ReadLine();
            //while (line != null)
            //{
            //    string[] values = line.Split();
            //    string sql = "insert into station_use values(" + values[0] + ",'" + values[1] + "','" + values[2] + "'," + values[3] + "," + values[4] + ")";
            //    OracleCommand inserCmd = new OracleCommand(sql, conn);
            //    inserCmd.ExecuteNonQuery();//执行数据库操作
            //    line = sr.ReadLine();
            //}
            //MessageBox.Show("成功");


            ////统计头节点个数
            //DataTable relTable = QueryResultTable("select * from event_state_relation");
            //List<String> heads = new List<string>();
            //for (int i = 0; i < relTable.Rows.Count; i++)
            //{
            //    string priorStateId = relTable.Rows[i][2].ToString();
            //    bool isHead = true;
            //    for (int j = 0; j < relTable.Rows.Count; j++)
            //    {
            //        if (relTable.Rows[j][3].ToString() == priorStateId)
            //        {
            //            isHead = false;
            //            break;
            //        }
            //    }
            //    if (isHead == true)
            //    {
            //        if (!heads.Contains(priorStateId)) heads.Add(priorStateId);
            //    }
            //}
            //MessageBox.Show(heads.Count.ToString());



            ////统计尾节点个数
            //DataTable relTable = QueryResultTable("select * from event_state_relation");
            //List<String> tails = new List<string>();
            //for (int i = 0; i < relTable.Rows.Count; i++)
            //{
            //    string nextStateId = relTable.Rows[i][3].ToString();
            //    bool isTail = true;
            //    for (int j = 0; j < relTable.Rows.Count; j++)
            //    {
            //        if (relTable.Rows[j][2].ToString() == nextStateId)
            //        {
            //            isTail = false;
            //            break;
            //        }
            //    }
            //    if (isTail == true)
            //    {
            //        if (!tails.Contains(nextStateId))
            //        {
            //            tails.Add(nextStateId);
            //            Console.WriteLine(nextStateId);
            //        }
            //    }
            //}
            //MessageBox.Show(tails.Count.ToString());


            //ClusterForm cf = new ClusterForm();
            //cf.Show();

            //AboutUsForm auf = new AboutUsForm();
            //auf.Show();

            //ResterToVector.TifToShp(@"E:\storm\tif\big2.tif", @"");
            //HdfToTif.Transform(@"E:\rain\20170601-0610_ori\20170601-S013000-E015959Precipitation_Resample.hdf", @"");
            //AboutUsForm auf = new AboutUsForm();
            //auf.Show();
            //string inpath = @"E:\strom\China201706\20170601-S000000-E002959Precipitation_Resample.hdf";//输入路径
            //string outPath = @"E:\strom\tif\4.tif";//输出路径
            //HdfToTif htf = new HdfToTif();
            //htf.Transform(inpath, outPath);
            //byte[] a = new byte[(long)(17520) * 500 * 700];
            //ResterToVector.TifToShp(@"E:\strom\space\20170602-S080000-E082959Precipitation_Resample_Time_Spatial.tif", @"E:\strom\shp\131.shp");

            //取出一点所有时间序列
            //PointValueForm pvf = new PointValueForm();
            //pvf.Show();

            //double startLat = 30.0;
            //double endLat = 30.1;
            //double log = 0.1;
            //double area = ResterToVector.GetRasterArea(startLat, endLat, log);

            //int countAll = 0;
            //int erroeCount = 0;
            //while (true)
            //{
            //    DateTime startTime = DateTime.Now;
            //    YMatrix m = new YMatrix(5, 5);
            //    //m.SetGiveValue();
            //    m.SetRandomValue(1, 10);
            //    bool r = m.Calculation();
            //    if (r == false)
            //    {
            //        erroeCount++;
            //    }
            //    countAll++;
            //    TimeSpan time = DateTime.Now - startTime;
            //}

            //MessageBox.Show(time.ToString());//@"d\.hh\:mm\:ss"

            //string path= @"E:\Storm\3\20170601-S000000-E002959Precipitation_Resample_Time_Spatial.shp";
            //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            //Gdal.SetConfigOption("SHAPE_ENCODING", "");
            //Ogr.RegisterAll();// 注册所有的驱动
            //DataSource ds = Ogr.Open(path, 1);//0表示只读，1表示可修改  

            //if (ds == null)
            //{//数据为空
            //    ds.Dispose();//关闭数据集
            //    return;
            //}
            //int iLayerCount = ds.GetLayerCount();//图层个数
            //Layer oLayer = ds.GetLayerByIndex(0);// 获取第一个图层

            //string argout = string.Empty;//用来存储空间参考
            //var spaRef = oLayer.GetSpatialRef();
            //spaRef.ExportToWkt(out argout);//获取空间参考
            //string spaRefWkt = argout;//记录
            //string coordSys;
            //spaRef.ExportToMICoordSys(out coordSys);
            //string pic, units;
            //spaRef.ExportToPCI(out pic, out units);
            //string proj4;
            //spaRef.ExportToProj4(out proj4);
            //int usgs, zone, datum;
            //spaRef.ExportToUSGS(out usgs, out zone, out datum);
            //if (oLayer == null)
            //{//图层为空
            //    oLayer.Dispose();
            //    return;
            //}
            ////List<string> fieldList = GetFieldList(oLayer);//获取图层属性表字段列表
            //long featureCount = oLayer.GetFeatureCount(0);

            ////输出属性表字段的详细信息，数据类型、宽度、精度等
            //FeatureDefn oDefn = oLayer.GetLayerDefn();
            //int fieldCount = oDefn.GetFieldCount();


            //// 先创建一个叫FieldID的整型属性
            //FieldDefn oFieldStateID = new FieldDefn("CeShi", FieldType.OFTString);
            //oFieldStateID.SetWidth(20);
            //oLayer.CreateField(oFieldStateID, 1);
            //Feature oFeature = null;
            //while ((oFeature = oLayer.GetNextFeature()) != null)
            //{
            //    oFeature.SetField(25,"abe");
            //    oLayer.SetFeature(oFeature);
            //}
            //oLayer.Dispose();
            //ds.Dispose();
        }
        #region 暴雨事件相关
        //时间维度暴雨提取
        private void stormTimeExtraction_Click(object sender, EventArgs e)
        {
            //ClsStormEvent cse = new ClsStormEvent();
            //cse.stormTimeExtraction(progressBar1);

            StormTimeExtractForm stef = new StormTimeExtractForm();
            stef.Show();
        }

        //暴雨空间维度提取
        private void stormSpatialExtraction_Click(object sender, EventArgs e)
        {
            //ClsStormEvent cse = new ClsStormEvent();
            //cse.stormSpatialExtraction(progressBar1);

            StormSpatialExtractForm ssef = new StormSpatialExtractForm();
            ssef.Show();
        }

        //暴雨编号提取
        private void stormNumberExtraction_Click(object sender, EventArgs e)
        {
            //ClsStormEvent cse = new ClsStormEvent();
            //cse.stormNumberExtraction(progressBar1);

            StormNumberExtractForm snef = new StormNumberExtractForm();
            snef.Show();
        }
        
        //暴雨栅格提取
        private void stormRasterExtraction_Click(object sender, EventArgs e)
        {
            if(conn.State == ConnectionState.Open)
            {//数据库已经打开
                StormRasterExtractForm sref = new StormRasterExtractForm(conn);
                sref.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //暴雨数据库存储
        private void stormDatabase_Click(object sender, EventArgs e)
        {
            ClsStormEvent cse = new ClsStormEvent();
            cse.stormDatabase();
        }

        //栅格转矢量
        private void stormResterToVector_Click(object sender, EventArgs e)
        {
            TifToShpForm ttf = new TifToShpForm();
            ttf.Show();
        }
        #endregion

        //降雨事件
        private void rainTimeExtraction_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果Table</returns>
        public DataTable QueryResultTable(string sql)
        {
            DataTable ResultDT = new DataTable();
            try
            {
                OracleDataAdapter objAdaper;
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                objAdaper = new OracleDataAdapter(cmd);
                objAdaper.Fill(ResultDT);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return ResultDT;
        }

        private void stromOneDimensionTimeSequence_Click(object sender, EventArgs e)
        {
            string inPath = @"E:\rain\70.05_58.95.txt";
            string outPath = @"E:\rain\70.05_58.95_time2.txt";
            double maxTimeInv = 3.0;
            double timeCell = 0.5;
            List<double> pTBuffer = new List<double>();//降雨量链表
            StreamReader sr = new StreamReader(inPath);
            string line = sr.ReadLine();
            while(line!=null)
            {
                pTBuffer.Add(Convert.ToDouble(line));
                line = sr.ReadLine();
            }
            sr.Close();
            int mFileNum = pTBuffer.Count;
            double[] timeResult = new double[mFileNum];//时间维度结果
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
                            //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = baoYu;//
                            timeResult[j] = baoYu;//结果时空立方体切割后
                        }
                    }
                    else
                    {//不是24小时连续
                        for (int j = startTime; j <= endTime; j++)
                        {
                            //if (pTBuffer[j] + pTBuffer[j + 1] > 16.0 && pTBuffer[j] > 0.0)
                            //{//暴雨
                            //    pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                            //    if (pTBuffer[j + 1] > 0.0)
                            //    {
                            //        pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                            //    }
                            //}

                            if (pTBuffer[j] / timeCell > 16.0)
                            {//暴雨
                             //pJuJiBuffer[j * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                                timeResult[j] = 1;//结果时空立方体切割后
                                                                                                   // pJuJiBuffer[(j + 1) * row * col + (startRow + rowNow) * col + startCol + colNow] = 1;
                            }
                        }
                    }
                }
            }

            StreamWriter sw = new StreamWriter(outPath);
            foreach(int value in timeResult)
            {
                sw.WriteLine(value);
            }
            sw.Close();
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

        private void readTifCoor_Click(object sender, EventArgs e)
        {
            GetProjectForm gpf = new GetProjectForm();
            gpf.Show();
        }

        private void linkServerDatabase_Click(object sender, EventArgs e)
        {
            LinkServerDatabaseForm lsdf = new LinkServerDatabaseForm(saveOracleConnection);
            lsdf.Show();
        }

        void saveOracleConnection(OracleConnection _conn)
        {
            conn = _conn;
            toolStripStatusLabel1.Text = "数据库已连接";
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            conn.Close();//关闭数据库连接
            toolStripStatusLabel1.Text = "数据库已关闭";
        }

        private void databaseClose_Click(object sender, EventArgs e)
        {
            conn.Close();//关闭数据库连接
            toolStripStatusLabel1.Text = "数据库已关闭";
        }

        private void linkLocalDatabase_Click(object sender, EventArgs e)
        {
            LinkServerDatabaseForm lsdf = new LinkServerDatabaseForm(saveOracleConnection,"localhost");
            lsdf.Text = "连接本地服务器";
            lsdf.Show();
        }

        private void writeTifCoor_Click(object sender, EventArgs e)
        {

        }

        private void 取出一点数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //取出一点所有时间序列
            PointValueForm pvf = new PointValueForm();
            pvf.Show();
        }

        private void EventExtraction_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                StromEventExtractionForm seef = new StromEventExtractionForm(conn);
                seef.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stormResterToVectorBasedSpace_Click(object sender, EventArgs e)
        {
            TifToShpBasedSpaceForm tf = new TifToShpBasedSpaceForm();
            tf.Show();
        }

        private void stormProcessExtract_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                StormProcessExtractForm spef = new StormProcessExtractForm(conn);
                spef.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataConversion_Click(object sender, EventArgs e)
        {

        }

        private void stormEventToVector_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                StormEventToVectorForm sevf = new StormEventToVectorForm(conn);
                sevf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stormEventStateToVector_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                StormEventStateToVectorForm sestvf = new StormEventStateToVectorForm(conn);
                sestvf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stormTITANExtract_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                StormTITANExtractForm sestvf = new StormTITANExtractForm(conn);
                sestvf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void nppToDatabase_Click(object sender, EventArgs e)
        {

        }

        private void shpToDatabase_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                ShpToDatabaseForm sdf = new ShpToDatabaseForm(conn);
                sdf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eventToDatabase_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                EventToDatabaseForm edf = new EventToDatabaseForm(conn);
                edf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void eventRelToDatabase_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                EventRelToDatabaseForm erdf = new EventRelToDatabaseForm(conn);
                erdf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 导出为文本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                OutToTxtForm otf = new OutToTxtForm(conn);
                otf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void 暴雨shp属性重新生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StormMoNiForm smnf = new StormMoNiForm();
            smnf.Show();
        }

        private void sST转换处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSTDataConvertion sdc = new SSTDataConvertion();
            sdc.Show();
        }

        private void marineHeatwavesTimeAnomaly_Click(object sender, EventArgs e)
        {
            MarineHeatwavesTimeAnomalyForm mhtaf = new MarineHeatwavesTimeAnomalyForm();
            mhtaf.Show();
        }

        private void marineHeatwavesMedian_Click(object sender, EventArgs e)
        {
            MarineHeatwavesMedianForm mhmf = new MarineHeatwavesMedianForm();
            mhmf.Show();
        }

        private void sST平均图像转换处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SSTDataAverageConvertionForm sdacf = new SSTDataAverageConvertionForm();
            sdacf.Show();
        }

        private void marineHeatwavesAverage_Click(object sender, EventArgs e)
        {
            MarineHeatwavesAverageForm mhaf = new MarineHeatwavesAverageForm();
            mhaf.Show();
        }

        private void marineHeatwaves90Percent_Click(object sender, EventArgs e)
        {
            MarineHeatwaves90PercentForm mhpf = new MarineHeatwaves90PercentForm();
            mhpf.Show();
        }

        private void marineHeatwavesTimeExtraction_Click(object sender, EventArgs e)
        {
            MarineHeatwavesTimeExtractionForm mhtef = new MarineHeatwavesTimeExtractionForm();
            mhtef.Show();
        }

        private void marineHeatwavesTimeExtraction2_Click(object sender, EventArgs e)
        {
            MarineHeatwavesTime2ExtractionForm mhtef2 = new MarineHeatwavesTime2ExtractionForm();
            mhtef2.Show();
        }

        private void marineHeatwavesVectorExtraction_Click(object sender, EventArgs e)
        {
            MarineHeatwavesSpaceExtractionForm mhsef = new MarineHeatwavesSpaceExtractionForm();
            mhsef.Show();
        }

        private void 流程数据转换处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OscarConvertionForm ocf = new OscarConvertionForm();
            ocf.Show();
        }

        private void 获取文件夹下所有文件名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetFileNamesForm gfnf = new GetFileNamesForm();
            gfnf.Show();
        }

        private void sstTimeExtraction_Click(object sender, EventArgs e)
        {
            SstTimeExtractForm temp = new SstTimeExtractForm();
            temp.Show();
        }

        private void sstSpaceExtraction_Click(object sender, EventArgs e)
        {
            SstSpatialExtractForm temp = new SstSpatialExtractForm();
            temp.Show();
        }

        private void sstResterToVectorBasedSpace_Click(object sender, EventArgs e)
        {
            sstTifToShpBasedSpaceForm temp = new sstTifToShpBasedSpaceForm();
            temp.Show();
        }

        private void sstRestertoVectorExtractInfo_Click(object sender, EventArgs e)
        {
            SstRestertoVectorExtractInfo temp = new SstRestertoVectorExtractInfo();
            temp.Show();
        }

        private void sstProcessExtract_Click(object sender, EventArgs e)
        {
            SstProcessExtractForm mhtf = new SstProcessExtractForm();
            mhtf.Show();
            //if (conn.State == ConnectionState.Open)
            //{//数据库已经打开

            //}
            //else
            //{
            //    MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void SSTEventStateToVector_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                SstEventStateToVectorForm sestvf = new SstEventStateToVectorForm(conn);
                sestvf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SstEventToVector_Click(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {//数据库已经打开
                SstEventToVectorForm sestvf = new SstEventToVectorForm(conn);
                sestvf.Show();
            }
            else
            {
                MessageBox.Show("数据库未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
