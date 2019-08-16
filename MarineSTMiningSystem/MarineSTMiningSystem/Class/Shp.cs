using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Data;

namespace MarineSTMiningSystem
{
    public class Shp
    {
        public int fileId;//记录文件id 文件顺序 与时间顺序相同
        public string url;//物理地址
        public DataTable fieldInfo; //属性元数据信息
        public DateTime startTime;//开始事件
        public DateTime endTime;//结束时间
        public List<Feature> featureList=new List<Feature>();//要素
        public DataSource ds;
        public Layer oLayer;
        //struct ShpFeature
        //{
        //    public Feature feature;//要素
        //    public List<Filed> fileds;//属性
        //}
        //struct Filed
        //{
        //    public string name;
        //    public string value;
        //    public string type;
        //}
        public Shp()
        {//构造函数

        }
        public Shp(string path)
        {//构造函数
            url = path;
            Gdal.AllRegister();//注册所有的格式驱动
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
            fieldInfo = new DataTable("fieldInfo");//储存属性表字段的详细信息，数据类型、宽度、精度等
            fieldInfo.Columns.Add("name"); fieldInfo.Columns.Add("type"); fieldInfo.Columns.Add("width"); fieldInfo.Columns.Add("precision");
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

        private List<string> GetFieldList(Layer mLayer)
        {
            List<string> newFieldList = new List<string>();
            FeatureDefn oDefn = mLayer.GetLayerDefn();
            int FieldCount = oDefn.GetFieldCount();
            for (int i = 0; i < FieldCount; i++)
            {
                FieldDefn oField = oDefn.GetFieldDefn(i);
                string fieldName = oField.GetNameRef();
                newFieldList.Add(fieldName);
            }
            oDefn.Dispose();
            return newFieldList;
        }

        public bool ExistField(string fieldName)
        {
            DataRow[] row = fieldInfo.Select("name = '" + fieldName + "'");
            if (row.Length == 0) return false;
            else return true;
        }

        public void AddIntField(string fieldName)
        {
            FieldDefn fieldInt = new FieldDefn(fieldName, FieldType.OFTInteger);
            oLayer.CreateField(fieldInt, 1);
            fieldInt.Dispose();
        }

        public void AddStringField(string fieldName,int width)
        {
            FieldDefn fieldString = new FieldDefn(fieldName, FieldType.OFTString);
            fieldString.SetWidth(width);
            oLayer.CreateField(fieldString, 1);
            fieldString.Dispose();
        }
        public void AddRealField(string fieldName,int width,int precission)
        {
            FieldDefn fieldReal = new FieldDefn(fieldName, FieldType.OFTReal);
            fieldReal.SetWidth(width);
            fieldReal.SetPrecision(precission);
            oLayer.CreateField(fieldReal, 1);
            fieldReal.Dispose();
        }

        public void ReSave()
        {
            foreach (Feature feature in featureList)
            {
                oLayer.SetFeature(feature);
            }
        }

        public void Dispose()
        {
            oLayer.Dispose();
            ds.Dispose();//关闭数据集
        }
    }
}
