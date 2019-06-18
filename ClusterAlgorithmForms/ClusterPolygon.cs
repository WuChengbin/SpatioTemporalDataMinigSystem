using MarineSTMiningSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    /// <summary>
    /// 时空簇多边形类,导出矢量多边形，并添加属性
    /// </summary>
    class ClusterPolygon
    {
        public List<StormLine> lineList = new List<StormLine>();

        public int id; //多边形唯一id
        public int pid;//所属时空簇过程id
        public double power;//强度
        public double aera;//面积
        public string time;//所在时间
        public string stateid;//状态id 
        string wkt;

        public List<ClusterPolygon> parentList = new List<ClusterPolygon>();//记录父对象
        public List<ClusterPolygon> childList = new List<ClusterPolygon>();//记录子对象

        public int fileId;

        public List<int> parentPosList = new List<int>();//记录父多边形在链表中的位置
        public List<int> childPosList = new List<int>();//记录子多边形在链表中的位置

        public ClusterPolygon()
        {
        }
        public ClusterPolygon(string _wkt)
        {
            wkt = _wkt;
            CreatFromWkt(_wkt);
        }

        //解析WKT坐标
        private void CreatFromWkt(string _wkt)
        {
            lineList = new List<StormLine>();
            int subPos = _wkt.IndexOf('(');
            _wkt = _wkt.Substring(subPos);//截取
            _wkt = _wkt.TrimStart('(');
            _wkt = _wkt.TrimEnd(')');
            string[] lineWktArr = _wkt.Split(new string[] { "),(" }, StringSplitOptions.None);//线wkt，前面没有LINESTRING标识
            foreach (string lineWkt in lineWktArr)
            {//每条线
                string[] pointWktArr = lineWkt.TrimStart('(').TrimEnd(')').Split(',');
                StormLine stormLine = new StormLine();
                string[] coor1 = pointWktArr[0].Split(' ');//第一个点的坐标
                double log1 = Convert.ToDouble(coor1[0]);//经度
                double lat1 = Convert.ToDouble(coor1[1]);//纬度
                stormLine.minLog = log1; stormLine.minLat = lat1;//最小经纬度
                stormLine.maxLog = log1; stormLine.maxLat = lat1;//最大经纬度
                foreach (string pointWkt in pointWktArr)
                {//每个点
                    string[] coor = pointWkt.Split(' ');
                    double log = Convert.ToDouble(coor[0]);//经度
                    double lat = Convert.ToDouble(coor[1]);//纬度
                    if (log < stormLine.minLog) stormLine.minLog = log;//更新最小经度
                    else if (log > stormLine.maxLog) stormLine.maxLog = log;//更新最小纬度
                    if (lat < stormLine.minLat) stormLine.minLat = lat;//更新最大经度
                    else if (lat > stormLine.maxLat) stormLine.maxLat = lat;//更新最大纬度
                    StormPoint stormPoint = new StormPoint(log, lat);//点
                    stormLine.pointList.Add(stormPoint);//保存点
                }
                lineList.Add(stormLine);
            }
        }


        //多边形拓扑判断
        public bool Overlap(ClusterPolygon _clusterPolygon)
        {
            bool isOverlap = false;
            foreach (StormPoint checkPoint in _clusterPolygon.lineList[0].pointList)
            {
                if (IsInPolygonNew(checkPoint, lineList[0].pointList))
                {//存在包换或者邻接关系
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap)
            {//可能是被包含状态
                foreach (StormPoint checkPoint in lineList[0].pointList)
                {
                    if (IsInPolygonNew(checkPoint, _clusterPolygon.lineList[0].pointList))
                    {//存在包换或者邻接关系
                        isOverlap = true;
                        break;
                    }
                }
            }
            return isOverlap;
        }

        //判断点是否在多边形内部
        private static bool IsInPolygonNew(StormPoint checkPoint, List<StormPoint> pointList)
        {
            bool inside = false;
            for (int i = 0; i < pointList.Count - 1; i++)
            {
                StormPoint p1 = pointList[i];
                StormPoint p2 = pointList[i + 1];
                if (p1.log >= checkPoint.log)
                {//右侧
                    if (p1.lat >= checkPoint.lat && p2.lat < checkPoint.lat)
                    {
                        inside = !inside;
                    }
                    else if (p1.lat < checkPoint.lat && p2.lat >= checkPoint.lat)
                    {
                        inside = !inside;
                    }
                }
            }

            return inside;
        }

    }
}
