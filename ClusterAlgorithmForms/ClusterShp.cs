using MarineSTMiningSystem;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    /// <summary>
    /// 时空簇shp类,导出矢量多边形，并添加属性
    /// </summary>
    class ClusterShp : StormShp
    {
        public List<ClusterPolygon> clusterPolyList = new List<ClusterPolygon>();//簇多边形链表
        public List<int> clusterPIdList = new List<int>();//包含的暴雨事件id
        private string spaRefWkt = string.Empty;//坐标参考

        public ClusterShp(string path)
        {
            url = path;
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            Ogr.RegisterAll();// 注册所有的驱动
            ds = Ogr.Open(path, 1);//0表示只读，1表示可修改  

            if (ds == null)
            {//数据为空
                ds.Dispose();//关闭数据集
                return;
            }
            int iLayerCount = ds.GetLayerCount();//图层个数
            oLayer = ds.GetLayerByIndex(0);// 获取第一个图层
            string argout = string.Empty;//用来存储空间参考
            var spaRef = oLayer.GetSpatialRef();
            spaRef.ExportToWkt(out argout);//获取空间参考
            spaRefWkt = argout;//记录
            string coordSys;
            spaRef.ExportToMICoordSys(out coordSys);
            string pic, units;
            spaRef.ExportToPCI(out pic, out units);
            string proj4;
            spaRef.ExportToProj4(out proj4);
            int usgs, zone, datum;
            spaRef.ExportToUSGS(out usgs, out zone, out datum);
            if (oLayer == null)
            {//图层为空
                oLayer.Dispose();
                return;
            }
            //List<string> fieldList = GetFieldList(oLayer);//获取图层属性表字段列表
            long featureCount = oLayer.GetFeatureCount(0);

            //输出属性表字段的详细信息，数据类型、宽度、精度等
            FeatureDefn oDefn = oLayer.GetLayerDefn();
            int fieldCount = oDefn.GetFieldCount();
            //string[,] fieldInfo = new string[fieldCount, 4];
            fieldInfo = new DataTable("属性表字段信息");//储存属性表字段的详细信息，数据类型、宽度、精度等
            fieldInfo.Columns.Add("名称"); fieldInfo.Columns.Add("类型"); fieldInfo.Columns.Add("宽度"); fieldInfo.Columns.Add("精度");
            //string fieldInfo = string.Empty;//属性表元数据信息
            {
                for (int i = 0; i < fieldCount; i++)
                {//每个属性
                    FieldDefn oField = oDefn.GetFieldDefn(i);
                    object[] row = { oField.GetNameRef(), oField.GetFieldType(), oField.GetWidth(), oField.GetPrecision() };
                    fieldInfo.Rows.Add(row);
                    //fieldInfo += String.Format("{0}:{1} {2} {3}", oField.GetNameRef(), oField.GetFieldTypeName(oField.GetFieldType()), oField.GetWidth(), oField.GetPrecision());
                    //fieldInfo += Environment.NewLine;
                }
            }

            Feature oFeature = null;
            while ((oFeature = oLayer.GetNextFeature()) != null)
            {
                featureList.Add(oFeature);
                //ShpFeature shpFeature;//shp要素
                //shpFeature.feature= oFeature
                //for (int i=0;i< fieldCount;i++)
                //{//每个属性
                //    Filed filed;//属性
                //    filed.name = fieldInfo.Rows[i][0].ToString();
                //    filed.type = fieldInfo.Rows[i][1].ToString();
                //    filed.value= oFeature.GetFieldAsString(i);
                //    //switch (filed.type.ToString())
                //    //{
                //    //    case "OFTInteger":
                //    //        filed.value
                //    //        break;
                //    //    case "OFTString":
                //    //        break;
                //    //    case "OFTReal":
                //    //        break;
                //    //    default:
                //    //        break;
                //    //}
                //}
                //string name = oFeature.GetFieldAsString(0);
                //double x = oFeature.GetFieldAsDouble(1);
                //double y = oFeature.GetFieldAsDouble(2);
                //double z = oFeature.GetFieldAsDouble(3);
            }
        }
    }
}
