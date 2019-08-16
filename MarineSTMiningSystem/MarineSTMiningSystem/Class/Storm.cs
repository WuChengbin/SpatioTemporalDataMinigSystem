using Oracle.ManagedDataAccess.Client;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    /// <summary>
    /// 暴雨类
    /// </summary>
    public class Storm
    {
        //属性
        public int threadId;//线程id
        public int oid;//数据库oid
        public int id;//暴雨id
        public string wkt = "";
        public DateTime startTime;//开始时间
        public DateTime endTime;//结束时间
        public TimeSpan duration;//持续时间
        public short power;//降雨强度
        public double sumArea;//面积和
        public double[] powerSumArea = new double[3];//暴雨面积和
        public double avgRain;//平均降雨量
        public double volume;//累计降雨量
        public double maxRain=double.MinValue;//最大降雨量
        public double minRain=double.MaxValue;//最小降雨量
        public double maxAvgRain=double.MinValue;//最大平均降雨量，一个时刻的
        public double minAvgRain=double.MaxValue;//最小平均降雨量，一个时刻的
        public double minLog = double.MaxValue;//最小经度
        public double minLat = double.MaxValue;//最小纬度
        public double maxLog=double.MinValue;//最大经度
        public double maxLat=double.MinValue;//最大纬度
        public StormPolygon headPoly = new StormPolygon();//头多边形
        public List<Feature> featureList = new List<Feature>();//要素列表
        public List<StormPolygon> polygonList = new List<StormPolygon>();//暴雨多边形列表
        public int maxStateId;//最大状态id
        public List<int> stateIdList = new List<int>();//记录所有状态id
        //public int startEventStateOid;//起始状态oid
        //public int startEventStateRelOid;//起始关系oid

        public Storm()
        {//构造函数

        }

        public string GetGeoSpl(string tableName)
        {
            string idColName = "STATE_ID"; //状态id列名
            string geoColName = "SPACE";//Geometry列名
            string geometrySql = string.Empty;//建立Geometry的sql语句
            double tol = 0.001;//Tolerance value
            if (stateIdList.Count == 1)
            {//一个状态id
                geometrySql = string.Format("select {2} from {0} where {1}='{4}_{3}'", tableName, idColName, geoColName, stateIdList[0], id);
            }
            else if (stateIdList.Count == 2)
            {//两个状态id
                geometrySql = string.Format("select SDO_GEOM.SDO_UNION(g{3}.{2},g{4}.{2},{5}) from {0} g{3}, {0} g{4} where g{3}.{1}='{6}_{3}' and g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, stateIdList[0], stateIdList[1], tol, id);
            }
            else if (stateIdList.Count > 2)
            {//多个状态id
                GetGeoMulitSql(tableName, idColName, geoColName, geometrySql);
                //geometrySql = string.Format("select SDO_GEOM.SDO_UNION(g{3}.{2},g{4}.{2},{5}) from {0} g{3}, {0} g{4} where g{3}.{1}='{6}_{3}' and g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, stateIdList[0], stateIdList[1], tol, id);
                //for (int i = 2; i < stateIdList.Count; i++)
                //{//从第三个状态id开始 循环每个状态id
                //    geometrySql = string.Format("select SDO_GEOM.SDO_UNION(({3}),g{4}.{2},{5})from {0} g{4} where g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, geometrySql, stateIdList[i], tol, id);
                //}
            }
            return geometrySql;
        }

        private void GetGeoMulitSql(string tableName, string idColName, string geoColName, string geometrySql)
        {
            throw new NotImplementedException();
        }

        public string GetGeoSpl(string tableName,string vName)
        {
            string idColName = "STATE_ID"; //状态id列名
            string geoColName = "SPACE";//Geometry列名
            string geometrySql = string.Empty;//建立Geometry的sql语句
            double tol = 0.001;//Tolerance value
            if (stateIdList.Count==1)
            {//一个状态id
                geometrySql = string.Format("select {2} from {0} where {1}='{4}_{3}'", tableName, idColName, geoColName, stateIdList[0],id);
            }
            else if (stateIdList.Count == 2)
            {
                geometrySql = string.Format("select SDO_GEOM.SDO_UNION(g{3}.{2},g{4}.{2},{5}) into {7} from {0} g{3}, {0} g{4} where g{3}.{1}='{6}_{3}' and g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, stateIdList[0], stateIdList[1], tol, id,vName);
            }
            else if(stateIdList.Count>2)
            {//多个状态id
                geometrySql = string.Format("select SDO_GEOM.SDO_UNION(g{3}.{2},g{4}.{2},{5}) from {0} g{3}, {0} g{4} where g{3}.{1}='{6}_{3}' and g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, stateIdList[0], stateIdList[1], tol,id);
                for (int i=2;i<stateIdList.Count;i++)
                {//从第三个状态id开始 循环每个状态id
                    if(i< stateIdList.Count-1)
                    {//不是最后一个
                        geometrySql = string.Format("select SDO_GEOM.SDO_UNION(({3}),g{4}.{2},{5})from {0} g{4} where g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, geometrySql, stateIdList[i], tol, id);
                    }
                    else
                    {//最后一个
                        geometrySql = string.Format("select SDO_GEOM.SDO_UNION(({3}),g{4}.{2},{5})into {7} from {0} g{4} where g{4}.{1}='{6}_{4}'", tableName, idColName, geoColName, geometrySql, stateIdList[i], tol, id, vName);
                    }
                }
            }
            return geometrySql;
        }
    }
}
