using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class MarineHeatwavePolygon
    {
        public List<MarineHeatwaveLine> lineList = new List<MarineHeatwaveLine>();
        public double minLog;
        public double minLat;
        public double maxLog;
        public double maxLat;
        public double iMean;//平均强度
        public double iMax;
        public double iMin;
        public double area;
        public double id;//多边形id 一副图像中的唯一值
        public int eventId;//事件id
        public int stateId;
        public string wkt;
        public string SQID;

        public double length;
        public double coreLog;
        public double coreLat;
        public double shapeIndex;//形状系数
        public double lengthMax;//最大长度
        public double widthMax;//最大宽度
        public double eRatio;//偏心率 最大宽度/最大长度
        public double recDeg;//矩形度
        public double sphDeg;//圆形度
        public int power;
        //public double speed;
        public double uSpeed;
        public double vSpeed;
        public bool isAcnode = false;//孤立点
        public double[,] minAreaRec = new double[5, 2];//最小面积外包矩形
        public List<MarineHeatwavePolygon> parentList = new List<MarineHeatwavePolygon>();
        public List<MarineHeatwavePolygon> childList = new List<MarineHeatwavePolygon>();
        public DateTime time;//当前时刻
        public DateTime startTime;//起始时刻
        public DateTime endTime;//结束时刻
        public int fileId;
        public List<int> parentPosList = new List<int>();
        public List<int> childPosList = new List<int>();
        internal bool isOrder = false; //是否编过号
        internal bool isSaveToDB = false; //是否保存到数据库了
        public bool isSequenceOrder = false;

        public MarineHeatwavePolygon()
        {
        }
        public MarineHeatwavePolygon(string _wkt)
        {
            wkt = _wkt;
            CreatFromWkt(_wkt);
        }

        private void CreatFromWkt(string _wkt)
        {
            lineList = new List<MarineHeatwaveLine>();
            int subPos = _wkt.IndexOf('(');
            _wkt = _wkt.Substring(subPos);//截取
            _wkt = _wkt.TrimStart('(');
            _wkt = _wkt.TrimEnd(')');
            string[] lineWktArr = _wkt.Split(new string[] { "),(" }, StringSplitOptions.None);//线wkt，前面没有LINESTRING标识
            foreach (string lineWkt in lineWktArr)
            {//每条线
                string[] pointWktArr = lineWkt.TrimStart('(').TrimEnd(')').Split(',');
                MarineHeatwaveLine stormLine = new MarineHeatwaveLine();
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
                    MarineHeatwavePoint stormPoint = new MarineHeatwavePoint(log, lat);//点
                    stormLine.pointList.Add(stormPoint);//保存点
                }
                lineList.Add(stormLine);
            }
        }

        public bool GetSdo(out string sdoGtype, out string sdoElemInfoArray, out string sdoOrdinateArray)
        {
            sdoGtype = string.Empty;//描述类型
            sdoElemInfoArray = string.Empty;//oracle spatil需要，用来描述坐标数组
            sdoOrdinateArray = string.Empty;//oracle spatil需要,坐标数组字符串
            if (lineList.Count == 1) sdoGtype = "2003";//Polygon类型 地理对象包含一个普通的多边形，但不包含空岛
            else if (lineList.Count > 1) sdoGtype = "2007";//多多边形MutliPolygon类型 一个地理对象包含岛（多岛）的多边形和N个多边形
            for (int i = 0; i < lineList.Count; i++)
            {//每条线
                int startingOffset = 1;//起始点位置，从1开始
                for (int j = 0; j < i; j++)
                {
                    startingOffset += (lineList[j].pointList.Count) * 2;//计算起始点位置
                }
                string etype = "1003";//多边形外环
                if (i > 0) etype = "2003";//多边形内环
                sdoElemInfoArray += startingOffset.ToString() + "," + etype + ",1,";

                MarineHeatwaveLine line = lineList[i];//取出线
                List<MarineHeatwavePoint> pointList = line.pointList;//取出点集合
                for (int j = pointList.Count - 1; j >= 0; j--)
                {//每个点，注意wkt外环逆时针内环顺时针，oracle spatial相反
                    MarineHeatwavePoint point = pointList[j];//取出点
                    sdoOrdinateArray += point.log + "," + point.lat + ",";//构建字符串
                }
            }
            sdoElemInfoArray = sdoElemInfoArray.TrimEnd(',');//去掉最后一个逗号
            sdoOrdinateArray = sdoOrdinateArray.TrimEnd(',');//去掉最后一个逗号
            return true;
        }

        public bool Overlap(MarineHeatwavePolygon _stormPolygon)
        {
            bool isOverlap = false;
            foreach (MarineHeatwavePoint checkPoint in _stormPolygon.lineList[0].pointList)
            {
                if (IsInPolygonVectorInBorder(checkPoint, lineList[0].pointList))
                {//存在包换或者邻接关系
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap)
            {//可能是被包含状态
                foreach (MarineHeatwavePoint checkPoint in lineList[0].pointList)
                {
                    if (IsInPolygonVectorInBorder(checkPoint, _stormPolygon.lineList[0].pointList))
                    {//存在包换或者邻接关系
                        isOverlap = true;
                        break;
                    }
                }
            }
            return isOverlap;
        }

        internal bool Overlap(double[,] points)
        {
            bool isOverlap = false;
            double[,] points2 = new double[lineList[0].pointList.Count, 2];
            for (int j = 0; j < lineList[0].pointList.Count; j++)
            {
                points2[j, 0] = lineList[0].pointList[j].log;
                points2[j, 1] = lineList[0].pointList[j].lat;
            }
            for (int i = 0; i < points.GetLength(0) - 1; i++)
            {
                if (IsInPolygonVectorInBorder(new double[] { points[i, 0], points[i, 1] }, points2))
                {//存在包换或者邻接关系
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap)
            {//可能是被包含状态
                for (int i = 0; i < points2.GetLength(0) - 1; i++)
                {
                    if (IsInPolygonVectorInBorder(new double[] { points2[i, 0], points2[i, 1] }, points))
                    {//存在包换或者邻接关系
                        isOverlap = true;
                        break;
                    }
                }
            }
            return isOverlap;
        }

        internal double SOD(double[,] rec, double recArea)
        {
            double sodArea = 0.0;//重叠面积
            double minRecArea1 = lengthMax * widthMax;//最小外包矩形面积
            double minRecArea2 = recArea;//最小外包矩形面积
            List<double[]> borderNodes = new List<double[]>();
            for (int i = 0; i < (minAreaRec.GetLength(0) - 1); i++)
            {//判断每个点是否在矩形中
                if (IsInPolygonVectorNoBorder(new double[] { minAreaRec[i, 0], minAreaRec[i, 1] }, rec))
                {//在里面
                    borderNodes.Add(new double[] { minAreaRec[i, 0], minAreaRec[i, 1] });
                }
            }
            for (int i = 0; i < (rec.GetLength(0) - 1); i++)
            {
                if (IsInPolygonVectorNoBorder(new double[] { rec[i, 0], rec[i, 1] }, minAreaRec))
                {
                    borderNodes.Add(new double[] { rec[i, 0], rec[i, 1] });
                }
            }

            //寻找交点
            for (int i = 0; i < (minAreaRec.GetLength(0) - 1); i++)
            {
                for (int j = 0; j < (rec.GetLength(0) - 1); j++)
                {
                    double i_x = 0.0; double i_y = 0.0;
                    int isX = get_line_intersection(minAreaRec[i, 0], minAreaRec[i, 1], minAreaRec[i + 1, 0], minAreaRec[i + 1, 1], rec[j, 0], rec[j, 1], rec[j + 1, 0], rec[j + 1, 1], ref i_x, ref i_y);
                    if (isX == 1)
                    { borderNodes.Add(new double[] { i_x, i_y }); }
                }
            }

            if (borderNodes.Count > 2)
            {//计算最小外包矩形
                Rectangle SODMinRec = GetMinAreaRec(borderNodes);//数值会×100
                sodArea = (SODMinRec.length / 100.0) * (SODMinRec.width / 100.0);
                return sodArea / minRecArea1 + sodArea / minRecArea2;
            }
            else
            {
                return 0.0;
            }
        }

        int get_line_intersection(double p0_x, double p0_y, double p1_x, double p1_y,
    double p2_x, double p2_y, double p3_x, double p3_y, ref double i_x, ref double i_y)
        {
            double s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, s_numer, t_numer, denom, t;
            s10_x = p1_x - p0_x;
            s10_y = p1_y - p0_y;
            s32_x = p3_x - p2_x;
            s32_y = p3_y - p2_y;

            denom = s10_x * s32_y - s32_x * s10_y;
            if (denom == 0)//平行或共线
                return 0; // Collinear
            bool denomPositive = denom > 0;

            s02_x = p0_x - p2_x;
            s02_y = p0_y - p2_y;
            s_numer = s10_x * s02_y - s10_y * s02_x;
            if ((s_numer < 0) == denomPositive)//参数是大于等于0且小于等于1的，分子分母必须同号且分子小于等于分母
                return 0; // No collision

            t_numer = s32_x * s02_y - s32_y * s02_x;
            if ((t_numer < 0) == denomPositive)
                return 0; // No collision

            if (((s_numer > denom) == denomPositive) || ((t_numer > denom) == denomPositive))
                return 0; // No collision
                          // Collision detected
            t = t_numer / denom;

            i_x = p0_x + (t * s10_x);

            i_y = p0_y + (t * s10_y);

            return 1;
        }

        /// <summary>
        /// 判断点是否在多边形内,矢量不规则多边形，由yang编写
        /// </summary>
        /// <param name="ckPoint">要判断的点</param>
        /// <param name="points">多边形的顶点，首尾同点</param>
        /// <returns></returns>
        private bool IsInPolygonVectorNoBorder(double[] ckPoint, double[,] points)
        {
            bool inside = false;
            for (int i = 0; i < points.GetLength(0) - 1; i++)
            {
                double[] p1 = { points[i, 0], points[i, 1] };
                double[] p2 = { points[i + 1, 0], points[i + 1, 1] };
                if (p1[1] > p2[1] && p1[1] >= ckPoint[1] && p2[1] < ckPoint[1])
                {//纬度跨越
                    if (p1[0] > ckPoint[0] && p2[0] > ckPoint[0])
                    {//一定在右侧
                        inside = !inside;
                    }
                    else if ((p1[0] >= ckPoint[0] && p2[0] < ckPoint[0]) || (p1[0] <= ckPoint[0] && p2[0] > ckPoint[0]))
                    {
                        double xLog = (p1[0] - p2[0]) * (ckPoint[1] - p2[1]) / (p1[1] - p2[1]) + p2[0];//交点经度
                        if (Math.Round(xLog, 4) == Math.Round(ckPoint[0], 4)) return false;//线上
                        else if (Math.Round(xLog, 4) > Math.Round(ckPoint[0], 4))
                            inside = !inside;//记录穿过
                    }
                }
                else if (p2[1] > p1[1] && p2[1] >= ckPoint[1] && p1[1] < ckPoint[1])
                {
                    if (p1[0] > ckPoint[0] && p2[0] > ckPoint[0]) inside = !inside;//一定在右侧
                    else if ((p1[0] > ckPoint[0] && p2[0] <= ckPoint[0]) || (p1[0] < ckPoint[0] && p2[0] >= ckPoint[0]))
                    {
                        double xLog = (p1[0] - p2[0]) * (ckPoint[1] - p2[1]) / (p1[1] - p2[1]) + p2[0];//交点经度
                        if (Math.Round(xLog, 4) == Math.Round(ckPoint[0], 4)) return false;//线上
                        else if (Math.Round(xLog, 4) > Math.Round(ckPoint[0], 4))
                            inside = !inside;//记录穿过
                    }
                }

            }
            return inside;
        }

        /// <summary>
        /// 判断点是否在多边形内,矢量不规则多边形，由yang编写
        /// </summary>
        /// <param name="checkPoint"></param>
        /// <param name="pointList"></param>
        /// <returns></returns>
        private bool IsInPolygonVectorInBorder(MarineHeatwavePoint checkPoint, List<MarineHeatwavePoint> pointList)
        {
            double[] ckPoint = { checkPoint.log, checkPoint.lat };
            double[,] points = new double[pointList.Count, 2];
            for (int i = 0; i < pointList.Count; i++)
            {
                points[i, 0] = pointList[i].log;
                points[i, 1] = pointList[i].lat;
            }
            return IsInPolygonVectorInBorder(ckPoint, points);
        }

        /// <summary>
        /// 判断点是否在多边形内,矢量不规则多边形，由yang编写
        /// </summary>
        /// <param name="ckPoint">要判断的点</param>
        /// <param name="points">多边形的顶点，首尾同点</param>
        /// <returns></returns>
        private bool IsInPolygonVectorInBorder(double[] ckPoint, double[,] points)
        {
            bool inside = false;
            for (int i = 0; i < points.GetLength(0) - 1; i++)
            {
                double[] p1 = { points[i, 0], points[i, 1] };
                double[] p2 = { points[i + 1, 0], points[i + 1, 1] };
                if (p1[1] > p2[1] && p1[1] >= ckPoint[1] && p2[1] < ckPoint[1])
                {//纬度跨越
                    if (p1[0] > ckPoint[0] && p2[0] > ckPoint[0])
                    {//一定在右侧
                        inside = !inside;
                    }
                    else if ((p1[0] >= ckPoint[0] && p2[0] < ckPoint[0]) || (p1[0] <= ckPoint[0] && p2[0] > ckPoint[0]))
                    {
                        double xLog = (p1[0] - p2[0]) * (ckPoint[1] - p2[1]) / (p1[1] - p2[1]) + p2[0];//交点经度
                        if (Math.Round(xLog, 4) == Math.Round(ckPoint[0], 4)) return true;//线上
                        else if (Math.Round(xLog, 4) > Math.Round(ckPoint[0], 4))
                            inside = !inside;//记录穿过
                    }
                }
                else if (p2[1] > p1[1] && p2[1] >= ckPoint[1] && p1[1] < ckPoint[1])
                {
                    if (p1[0] > ckPoint[0] && p2[0] > ckPoint[0]) inside = !inside;//一定在右侧
                    else if ((p1[0] > ckPoint[0] && p2[0] <= ckPoint[0]) || (p1[0] < ckPoint[0] && p2[0] >= ckPoint[0]))
                    {
                        double xLog = (p1[0] - p2[0]) * (ckPoint[1] - p2[1]) / (p1[1] - p2[1]) + p2[0];//交点经度
                        if (Math.Round(xLog, 4) == Math.Round(ckPoint[0], 4)) return true;//线上
                        else if (Math.Round(xLog, 4) > Math.Round(ckPoint[0], 4))
                            inside = !inside;//记录穿过
                    }
                }

            }
            return inside;
        }


        /// <summary>
        /// 预测位置
        /// </summary>
        /// <param name="timeCell">时间间隔</param>
        /// <returns></returns>
        internal double[] GetPrePos(double timeCell)
        {
            //double length = speed * timeCell;
            //double directR = direct * Math.PI / 180;//弧度制
            double logAddKm = uSpeed * 60 * 60 * 24 * timeCell / 1000.0;//经度增量
            double latAddKm = vSpeed * 60 * 60 * 24 * timeCell / 1000.0;//纬度增量
            double logDis = logAddKm / (Earth.OneLogLen(coreLat) / 1000.0);//移动经度
            double latDis = latAddKm / (Earth.OneLatLen() / 1000.0);//移动纬度
            double logPer = coreLog + logDis;//经度预测
            double latPer = coreLat + latDis;//纬度预测
            double[] posPer = { logPer, latPer };
            return posPer;
        }

        #region 计算最小面积外接矩形
        private Rectangle GetMinAreaRec(List<double[]> pointsRL)
        {
            int[,] pointsR = new int[pointsRL.Count, 2];
            for (int i = 0; i < pointsRL.Count; i++)
            {
                pointsR[i, 0] = Convert.ToInt32(pointsRL[i][0] * 100);
                pointsR[i, 1] = Convert.ToInt32(pointsRL[i][1] * 100);
            }
            return GetMinAreaRec(pointsR);
        }

        private Rectangle GetMinAreaRec(int[,] points)
        {
            Rectangle minRec = new Rectangle(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, double.MaxValue);

            int minAreaAngle = 0;//最小外包矩形旋转角度

            for (int angle = 0; angle < 360; angle++)
            {//旋转每个角度
                double[,] pointsR = getPointsRotate(points, angle);//获取旋转后的点
                Rectangle rec = getRec(pointsR);//计算平行于坐标轴的最小外包矩形
                if (rec.area < minRec.area)
                {//更小的外包矩形
                    minRec = rec;
                    minAreaAngle = angle;
                }
            }
            minRec.area = (minRec.maxX - minRec.minX) * (minRec.maxY - minRec.minY);
            Rectangle minRecR = getRecRotate(minRec, -minAreaAngle);//获取旋转后的矩形

            return minRecR;
        }

        /// <summary>
        /// //查找平行于坐标轴的最小外包矩形
        /// </summary>
        /// <param name="pointsR"></param>
        /// <returns></returns>
        private Rectangle getRec(double[,] pointsR)
        {
            Rectangle rec = new Rectangle();
            rec.minX = pointsR[0, 0];
            rec.maxX = pointsR[0, 0];
            rec.minY = pointsR[0, 1];
            rec.maxY = pointsR[0, 1];
            for (int i = 0; i < pointsR.GetLength(0); i++)
            {//每个点
                double _x = pointsR[i, 0];
                double _y = pointsR[i, 1];
                if (_x < rec.minX) rec.minX = _x; else if (_x > rec.maxX) rec.maxX = _x;
                if (_y < rec.minY) rec.minY = _y; else if (_y > rec.maxY) rec.maxY = _y;
            }
            rec.area = (rec.maxX - rec.minX) * (rec.maxY - rec.minY);
            return rec;
        }

        private Rectangle getRecRotate(Rectangle minRec, int angle)
        {
            double[] p1 = { minRec.minX, minRec.minY };
            double[] p2 = { minRec.maxX, minRec.minY };
            double[] p3 = { minRec.maxX, minRec.maxY };
            double[] p4 = { minRec.minX, minRec.maxY };
            double[] p1R = GetPointRotate(p1[0], p1[1], angle);
            double[] p2R = GetPointRotate(p2[0], p2[1], angle);
            double[] p3R = GetPointRotate(p3[0], p3[1], angle);
            double[] p4R = GetPointRotate(p4[0], p4[1], angle);

            double length = minRec.maxX - minRec.minX;
            double width = minRec.maxY - minRec.minY;
            if (width > length)
            {//交换长宽
                double temp = length;
                length = width;
                width = temp;
            }

            Rectangle RecR = new Rectangle(p1R, p2R, p3R, p4R, length, width, minRec.area);
            return RecR;
        }

        private double[,] getPointsRotate(int[,] points, int angle)
        {
            double[,] pointsR = new double[points.GetLength(0), points.GetLength(1)];//用来存储旋转后的点
            for (int i = 0; i < points.GetLength(0); i++)
            {
                int[] point = { points[i, 0], points[i, 1] };//取出一个点
                double[] pointR = GetPointRotate(point[0], point[1], angle);//获取改点绕原点旋转坐标
                pointsR[i, 0] = pointR[0];
                pointsR[i, 1] = pointR[1];
            }
            return pointsR;
        }

        private double[,] getPointsRotate(double[,] points, int angle)
        {
            double[,] pointsR = new double[points.GetLength(0), points.GetLength(1)];//用来存储旋转后的点
            for (int i = 0; i < points.GetLength(0); i++)
            {
                double[] point = { points[i, 0], points[i, 1] };//取出一个点
                double[] pointR = GetPointRotate(point[0], point[1], angle);//获取改点绕原点旋转坐标
                pointsR[i, 0] = pointR[0];
                pointsR[i, 1] = pointR[1];
            }
            return pointsR;
        }

        private double[] GetPointRotate(double x, double y, int angle)
        {
            double angleR = angle * Math.PI / 180;//计算弧度制角度
            double xr = x * Math.Cos(angleR) + y * Math.Sin(angleR);
            double yr = -x * Math.Sin(angleR) + y * Math.Cos(angleR);
            double[] pr = { xr, yr };
            return pr;
        }

        private Rectangle GetRecRotate(Rectangle minRec, int angle)
        {
            double[] p1 = { minRec.minX, minRec.minY };
            double[] p2 = { minRec.maxX, minRec.minY };
            double[] p3 = { minRec.maxX, minRec.maxY };
            double[] p4 = { minRec.minX, minRec.maxY };
            double[] p1R = GetPointRotate(p1[0], p1[1], angle);
            double[] p2R = GetPointRotate(p2[0], p2[1], angle);
            double[] p3R = GetPointRotate(p3[0], p3[1], angle);
            double[] p4R = GetPointRotate(p4[0], p4[1], angle);

            double length = minRec.maxX - minRec.minX;
            double width = minRec.maxY - minRec.minY;
            if (width > length)
            {//交换长宽
                double temp = length;
                length = width;
                width = temp;
            }

            Rectangle RecR = new Rectangle(p1R, p2R, p3R, p4R, length, width, minRec.area);
            return RecR;
        }

        /// <summary>
        /// 返回暴雨点序列的坐标
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        private double[,] getPoints(List<StormPoint> pointList)
        {
            double[,] points = new double[pointList.Count, 2];

            for (int i = 0; i < pointList.Count; i++)
            {
                points[i, 0] = pointList[i].log;
                points[i, 1] = pointList[i].lat;
            }

            return points;
        }
        #endregion
    }
}
