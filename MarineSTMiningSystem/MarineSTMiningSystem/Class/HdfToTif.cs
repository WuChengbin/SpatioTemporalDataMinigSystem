using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    static class HdfToTif
    {
        /// <summary>
        /// Hdf转tif
        /// </summary>
        /// <param name="inPath">输入单个文件完整路径</param>
        /// <param name="outPath">输出单个文件完整路径</param>
        public static void Transform(string inPath,string outPath)
        {
            try
            {
                Gdal.AllRegister();//注册所有的格式驱动
                Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");//支持中文路径和名称,然而没有用

                //打开hdf文件
                Dataset ds = Gdal.Open(inPath, Access.GA_ReadOnly);
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
                string imgDate = "";
                string fillValue = "";
                double resolution = 0.0;
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
                        case "ImageDate":
                            imgDate = mdArr[1];
                            break;
                        case "FillValue":
                            fillValue = mdArr[1];
                            break;
                        case "DSResolution":
                            resolution = Convert.ToDouble(mdArr[1]);
                            break;
                        default:
                            break;
                    }
                }

                int[] databuf = new int[row * col];//存储
                demband1.ReadRaster(0, 0, col, row, databuf, col, row, 0, 0);//读取数据

                //保存为tif格式
                Driver gTiffRriver = Gdal.GetDriverByName("GTiff");
                Dataset gTiffDataset = gTiffRriver.Create(outPath, col, row, 1, DataType.GDT_Int32, null);
                gTiffDataset.SetMetadata("StartLog", startLog.ToString());
                gTiffDataset.SetMetadata("EndLog", endLog.ToString());
                gTiffDataset.SetMetadata("startLat", startLat.ToString());
                gTiffDataset.SetMetadata("EndLat", endLat.ToString());
                gTiffDataset.SetMetadata("Scale", mScale.ToString());
                gTiffDataset.SetMetadata("FillValue", fillValue);
                //string imgDate = outfileName.Substring(0, 24);
                gTiffDataset.SetMetadata("ImageDate", imgDate);
                gTiffDataset.SetMetadata("DSResoution", resolution.ToString());
                gTiffDataset.SetMetadata("DataType", dataType);
                gTiffDataset.WriteRaster(0, 0, col, row, databuf, col, row, 1, null, 0, 0, 0);

                gTiffDataset.Dispose();
                ds.Dispose();
                demband1.Dispose();
                gTiffRriver.Dispose();
                gTiffDataset.Dispose();
            }
            catch(Exception err)
            {
                throw err;
            }
        }
    }
}
