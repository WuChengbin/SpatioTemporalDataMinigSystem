using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class StormPolygon
    {
        public List<StormLine> lineList = new List<StormLine>();
        public double minLog;
        public double minLat;
        public double maxLog;
        public double maxLat;
        public double avgRain;//平均降雨量
        public double volume;//累计降雨量
        public double maxRain;//最大降雨量
        public double minRain;//最小降雨量
        public double area;//面积
        public int power;//降雨强度
        public int id;//多边形id 在一幅图像中是唯一值
        public int stormId;//暴雨id
        public int stateId;//状态id
        public string wkt;//wkt格式数据

        public double length;//周长
        public double coreLog;//质心经度
        public double coreLat;//质心纬度
        public double shapeIndex;//形状系数
        public double lengthMax;//最大长度
        public double widthMax;//最大宽度
        public double eRatio;//偏心率 最大宽度/最大长度
        public double recDeg;//矩形度
        public double sphDeg;//圆形度
        public double speed = 0;//移动速度
        public double direct = 0;//移动方向
        public bool isAcnode = false;//孤立点
        public bool isSequenceOrder = false;

        //public double[] minAreaRecP1 = new double[2];//最小面积外接矩形点1
        //public double[] minAreaRecP2 = new double[2];//最小面积外接矩形点2
        //public double[] minAreaRecP3 = new double[2];//最小面积外接矩形点3
        //public double[] minAreaRecP4 = new double[2];//最小面积外接矩形点4
        public double[,] minAreaRec = new double[5, 2];//最小面积外包矩形

        public List<StormPolygon> parentList = new List<StormPolygon>();//记录父对象
        public List<StormPolygon> childList = new List<StormPolygon>();//记录子对象

        public DateTime time;//当前时刻

        public DateTime startTime;//起始时刻
        public DateTime endTime;//结束时刻
        public int fileId;

        public List<int> parentPosList = new List<int>();//记录父多边形在链表中的位置
        public List<int> childPosList = new List<int>();//记录子多边形在链表中的位置
        internal bool isOrder = false; //是否编过号
        internal bool isSaveToDB = false; //是否保存到数据库了

        public StormPolygon()
        {
        }
        public StormPolygon(string _wkt)
        {
            wkt = _wkt;
            CreatFromWkt(_wkt);
        }

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

        internal void CalculateRec()
        {
            double _minLog = double.MaxValue;
            double _minLat = double.MaxValue;
            double _maxLog = double.MinValue;
            double _maxLat = double.MinValue;
            List<StormPoint> ps = lineList[0].pointList;
            foreach(StormPoint p in ps)
            {
                if (p.log < _minLog) _minLog = p.log;
                if (p.log > _maxLog) _maxLog = p.log;
                if (p.lat < _minLat) _minLat = p.lat;
                if (p.lat > _maxLat) _maxLat = p.lat;
            }
            minLog = _minLog;
            minLat = _minLat;
            maxLog = _maxLog;
            maxLat = _maxLat;
        }

        internal void CalculateCorePos()
        {
            List<StormPoint> ps = lineList[0].pointList;
            double logSum = 0.0;
            double latSum = 0.0;
            for(int i=0;i<ps.Count-1;i++)
            {
                logSum += ps[i].log;
                latSum += ps[i].lat;
            }
            coreLog = logSum / (ps.Count - 1);
            coreLat = latSum / (ps.Count - 1);
        }

        internal void CalculateLength()
        {
            double _length = 0.0;
            List<StormPoint> points = lineList[0].pointList;
            int preId = 0;
            int nextId = 1;
            while(nextId<points.Count)
            {
                StormPoint prePoint = points[preId];
                StormPoint nextPoint = points[nextId];
                double distance = Earth.TowPosDisM(prePoint.log, prePoint.lat, nextPoint.log, nextPoint.lat)*0.001;
                _length += distance;
                preId++;
                nextId++;
            }
            length = _length;
        }

        internal void CalculateArea()
        {
            CalculateArea(0.05);
        }

        /// <summary>
        /// 散点法计算不规则多边形面积
        /// </summary>
        /// <param name="stepLen"></param>
        internal void CalculateArea(double stepLen)
        {
            double _areaSum = 0.0;
            for(double _log=minLog;_log<=maxLog;_log+=stepLen)
            {
                for(double _lat=minLat;_lat<=maxLat;_lat+=stepLen)
                {
                    if(IsInPolygonVectorNoBorder(_log,_lat))
                    {
                        double _area = Earth.OneLatLen() * 0.001 * stepLen * Earth.OneLogLen(_lat) * 0.001 * stepLen;
                        _areaSum += _area;
                    }
                }
            }
            area = _areaSum;
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

                StormLine line = lineList[i];//取出线
                List<StormPoint> pointList = line.pointList;//取出点集合
                for (int j = pointList.Count - 1; j >= 0; j--)
                {//每个点，注意wkt外环逆时针内环顺时针，oracle spatial相反
                    StormPoint point = pointList[j];//取出点
                    sdoOrdinateArray += point.log + "," + point.lat + ",";//构建字符串
                }
            }
            sdoElemInfoArray = sdoElemInfoArray.TrimEnd(',');//去掉最后一个逗号
            sdoOrdinateArray = sdoOrdinateArray.TrimEnd(',');//去掉最后一个逗号
            return true;
        }

        public bool Overlap(StormPolygon _stormPolygon)
        {
            bool isOverlap = false;
            foreach (StormPoint checkPoint in _stormPolygon.lineList[0].pointList)
            {
                if (IsInPolygonVectorInBorder(checkPoint, lineList[0].pointList))
                {//存在包换或者邻接关系
                    isOverlap = true;
                    break;
                }
            }
            if (!isOverlap)
            {//可能是被包含状态
                foreach (StormPoint checkPoint in lineList[0].pointList)
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

        internal bool Overlap(Circle c)
        {
            double cLog = c.x;//圆心经度
            double cLat = c.y;//圆心维度
            double cRad = c.r;//园半径 单位km
            StormPoint checkPoint = new StormPoint(cLog, cLat);
            if (IsInPolygonVectorInBorder(checkPoint, lineList[0].pointList))
            {//判断圆心是否包含在多边形中
                return true;
            }
            else
            {
                bool isOverlap = false;//默认不相交
                foreach (StormPoint _checkPoint in lineList[0].pointList)
                {
                    double len = Earth.TowPosDisM(cLog, cLat, _checkPoint.log, _checkPoint.lat) / 1000.0;//转换为千米
                    if (len <= cRad)
                    {//存在包换或者邻接关系
                        isOverlap = true;
                        break;
                    }
                }
                return isOverlap;
            }
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
            for (int i=0;i< points.GetLength(0)-1;i++)
            {
                if (IsInPolygonVectorInBorder(new double[] { points[i,0], points[i, 1] }, points2))
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

        /// <summary>
        /// 判断点是否在多边形内,由yang编写
        /// </summary>
        /// <param name="checkPoint">要判断的点</param>
        /// <param name="pointList">多边形的顶点，首尾同点</param>
        /// <returns></returns>
        private bool IsInPolygonNew(StormPoint checkPoint, List<StormPoint> pointList)
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

        /// <summary>
        /// 判断点是否在多边形内,矢量不规则多边形，由yang编写
        /// </summary>
        /// <param name="checkPoint"></param>
        /// <param name="pointList"></param>
        /// <returns></returns>
        private bool IsInPolygonVectorInBorder(StormPoint checkPoint, List<StormPoint> pointList)
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

        private bool IsInPolygonVectorNoBorder(double log, double lat)
        {
            double[] ckPoint = { log, lat };
            List<StormPoint> pointList = lineList[0].pointList;
            double[,] points = new double[pointList.Count, 2];
            for (int i = 0; i < pointList.Count; i++)
            {
                points[i, 0] = pointList[i].log;
                points[i, 1] = pointList[i].lat;
            }
            return IsInPolygonVectorNoBorder(ckPoint, points);
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
        /// 移除前缀字符串
        /// </summary>
        /// <param name="val">原字符串</param>
        /// <param name="str">前缀字符串</param>
        /// <returns>移除前缀的字符串</returns>
        private string GetRemovePrefixString(string val, string str)
        {
            string strRegex = @"^(" + str + ")";
            return Regex.Replace(val, strRegex, "");
        }

        /// <summary>
        /// 计算速度和方向
        /// </summary>
        internal void CalculateState(double timeCell)
        {
            StormPolygon parent = parentList[0];//父节点
            double logDis = coreLog - parent.coreLog;//经度差
            double latDis = coreLat - parent.coreLat;//纬度差
            double logDisKm = 111.319 * logDis * Math.Cos(((parent.coreLat + coreLat) / 2.0) * Math.PI / 180.0);//计算水平距离
            double latDisKm = 111.319 * latDis;//计算竖直距离
            double len = Math.Sqrt(logDisKm * logDisKm + latDisKm * latDisKm);//距离
            speed = len / timeCell;//计算速度
            if (latDisKm == 0.0)
            {//维度差0
                if (logDisKm == 0)
                {//经度差为0，没有移动
                    direct = parent.direct;//继承父方向
                }
                else if (logDisKm > 0.0)
                {//东
                    direct = 90;
                }
                else if (logDisKm < 0.0)
                {//西
                    direct = 270;
                }
            }
            else
            {//维度差不为0
                double direct = GetDir(logDisKm, latDisKm);//计算方向角
            }
        }

        private double GetDir(double _x, double _y)
        {
            double directR = Math.Atan(_x / _y);//方向角弧度制
            direct = directR * 180.0 / Math.PI;
            if (_y == 0)
            {
                if (_x == 0)
                {//没有移动
                    direct = 0.0;
                }
                else if (_x > 0.0)
                {//东
                    direct = 90;
                }
                else if (_x < 0.0)
                {//西
                    direct = 270;
                }
            }
            else if (_y > 0)
            {
                if (_x < 0)
                {//
                    direct += 360;
                }
            }
            else if (_y < 0)
            {
                direct += 180;
            }
            return direct;
        }

        /// <summary>
        /// 移除后缀字符串
        /// </summary>
        /// <param name="val">原字符串</param>
        /// <param name="str">后缀字符串</param>
        /// <returns>移除后缀的字符串</returns>
        private string GetRemoveSuffixString(string val, string str)
        {
            string strRegex = @"(" + str + ")" + "$";
            return Regex.Replace(val, strRegex, "");
        }

        /// <summary>
        /// 预测位置
        /// </summary>
        /// <param name="timeCell">时间间隔</param>
        /// <returns></returns>
        internal double[] GetPrePos(double timeCell)
        {
            double length = speed * timeCell;
            double directR = direct * Math.PI / 180;//弧度制
            double logAddKm = length * Math.Sin(directR);//经度增量
            double latAddKm = length * Math.Cos(directR);//纬度增量
            double logDis = logAddKm / (Earth.OneLogLen(coreLat) / 1000.0);//移动经度
            double latDis = latAddKm / (Earth.OneLatLen() / 1000.0);//移动纬度
            double logPer = coreLog + logDis;//经度预测
            double latPer = coreLat + latDis;//纬度预测
            double[] posPer = { logPer, latPer };
            return posPer;
        }

        internal Rectangle GetMinAreaRec()
        {
            Rectangle minRec = new Rectangle(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue, double.MaxValue);

            StormLine line1 = lineList[0];
            List<StormPoint> pointList = line1.pointList;

            double[,] points = getPoints(pointList);//row=x col=y

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
            Rectangle minRecR = GetRecRotate(minRec, -minAreaAngle);//获取旋转后的矩形

            return minRecR;
        }

        /// <summary>
        /// 计算重叠度
        /// </summary>
        /// <param name="stormPolygon"></param>
        /// <returns></returns>
        internal double SOD(StormPolygon stormPolygon)
        {
            double sodArea = 0.0;//重叠面积
            double minRecArea1 = lengthMax * widthMax;//最小外包矩形面积
            double minRecArea2 = stormPolygon.lengthMax * stormPolygon.widthMax;//最小外包矩形面积
            List<double[]> borderNodes = new List<double[]>();
            for (int i = 0; i < (minAreaRec.GetLength(0) - 1); i++)
            {//判断每个点是否在矩形中
                if (IsInPolygonVectorNoBorder(new double[] { minAreaRec[i, 0], minAreaRec[i, 1] }, stormPolygon.minAreaRec))
                {//在里面
                    borderNodes.Add(new double[] { minAreaRec[i, 0], minAreaRec[i, 1] });
                }
            }
            for (int i = 0; i < (stormPolygon.minAreaRec.GetLength(0) - 1); i++)
            {
                if (IsInPolygonVectorNoBorder(new double[] { stormPolygon.minAreaRec[i, 0], stormPolygon.minAreaRec[i, 1] }, minAreaRec))
                {
                    borderNodes.Add(new double[] { stormPolygon.minAreaRec[i, 0], stormPolygon.minAreaRec[i, 1] });
                }
            }

            //寻找交点
            for (int i = 0; i < (minAreaRec.GetLength(0) - 1); i++)
            {
                for (int j = 0; j < (stormPolygon.minAreaRec.GetLength(0) - 1); j++)
                {
                    double i_x = 0.0; double i_y = 0.0;
                    int isX = get_line_intersection(minAreaRec[i, 0], minAreaRec[i, 1], minAreaRec[i + 1, 0], minAreaRec[i + 1, 1], stormPolygon.minAreaRec[j, 0], stormPolygon.minAreaRec[j, 1], stormPolygon.minAreaRec[j + 1, 0], stormPolygon.minAreaRec[j + 1, 1], ref i_x, ref i_y);
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

        // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
        // intersect the intersection point may be stored in the doubles i_x and i_y.
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

        #region 最大面积内切圆

        internal Circle GetMaxInCir()
        {
            return GetMaxInCir(0.05);
        }
        /// <summary>
        /// 求最大面积内切圆（遗传算法）
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private Circle GetMaxInCir(double step)
        {
            double minX = minLog;
            double minY = minLat;
            double maxX = maxLog;
            double maxY = maxLat;

            //double[,] ccs = new double[Convert.ToInt32(((maxX - minX) / step - 1) * ((maxY - minY) / step - 1)), 2];//初始远心点
            List<Circle> cirs = new List<Circle>();//最大半径的圆
            cirs.Add(new Circle());
            //double radiusMax = 0.0;
            //double xm = 0.0;
            //double ym = 0.0;
            for (double _x = minX + step; _x < maxX; _x += step)
            {
                for (double _y = minY + step; _y < maxY; _y += step)
                {
                    if (IsInPolygonVectorNoBorder(_x, _y))
                    {//点位于多边形内
                        double radius = GetRadiusPoint( _x, _y);//获取该点为圆心的内切圆半径

                        if (radius > cirs[0].r)
                        {
                            cirs.Clear();
                            cirs.Add(new Circle(_x, _y, radius));
                        }
                        else if (radius == cirs[0].r)
                        {
                            cirs.Add(new Circle(_x, _y, radius));
                        }

                        //bool isInsert = false;
                        //for (int i = 0; i < cirs.Count; i++)
                        //{
                        //    if (cirs[i].r < radius)
                        //    {
                        //        cirs.Insert(i, new Circle(_x, _y, radius));
                        //        if (cirs.Count > 3) cirs.RemoveAt(cirs.Count - 1);
                        //        isInsert = true;
                        //        break;
                        //    }
                        //}
                        //if(!isInsert&&cirs.Count<3) cirs.Add(new Circle(_x, _y, radius));

                        //if(radius> radiusMax)
                        //{//更大的半径
                        //    radiusMax = radius;
                        //    xm = _x;
                        //    ym = _y;
                        //}
                    }
                }
            }
            Circle maxInCir = new Circle();
            foreach (Circle cir in cirs)
            {
                Circle _maxInCir = GetMaxInCir(step, cir.x, cir.y);//计算调整后的圆
                if (_maxInCir.r > maxInCir.r) maxInCir = _maxInCir;//寻找最大半径圆
            }
            return maxInCir;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <returns></returns>
        private double GetRadiusPoint(double x, double y)
        {
            List<StormPoint> nodeList = lineList[0].pointList;
            double[,] points = new double[nodeList.Count, 2];
            for (int i = 0; i < nodeList.Count; i++)
            {//取出xy
                points[i, 0] = nodeList[i].log;//x
                points[i, 1] = nodeList[i].lat;//y
            }

            double distances = double.MaxValue;//点与线段的距离
            for (int i = 0; i < nodeList.Count - 1; i++)
            {//计算距离
                double _distances = GetPointLineDistance(x, y, points[i, 0], points[i, 1], points[i + 1, 0], points[i + 1, 1]);//计算距离
                if (_distances < distances) distances = _distances;//记录更小的
            }

            return distances;
        }

        /// <summary>
        /// 寻找面积最大的内切圆
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="step"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Circle GetMaxInCir(double step, double x, double y)
        {
            double xOir = x;
            double yOir = y;
            double stepOir = step;//记录原始值
            step = step * 0.5;
            StormLine line = lineList[0];
            List<StormPoint> nodeList = line.pointList;
            double[,] points = new double[nodeList.Count, 2];
            for (int i = 0; i < nodeList.Count; i++)
            {//取出xy
                points[i, 0] = nodeList[i].log;//x
                points[i, 1] = nodeList[i].lat;//y
            }

            double lastStandDev = double.MaxValue;//记录上一个标准差
            double standDev = double.MaxValue;//当前标准差
            double r = 0.0;//圆的半径
            while (true)
            {//迭代求最大面积内切圆
                double[] distances = new double[nodeList.Count - 1];//点与线段的距离
                double[,] disPoints = new double[nodeList.Count - 1, 2];//计算线段距离的另一个端点

                List<int> minDisPosList = new List<int>();//记录距离最小点位置
                for (int i = 0; i < distances.Length; i++)
                {//计算距离
                    double[] disPoint = new double[2];//计算距离的另一个点
                    distances[i] = Math.Round(GetPointLineDistance(x, y, points[i, 0], points[i, 1], points[i + 1, 0], points[i + 1, 1], ref disPoint), 4);//计算距离
                    disPoints[i, 0] = disPoint[0];//记录计算距离端点位置
                    disPoints[i, 1] = disPoint[1];
                    bool isInsert = false;//记录是否插入
                    for (int j = 0; j < minDisPosList.Count; j++)
                    {
                        if (distances[i] < distances[minDisPosList[j]])
                        {//更小的距离
                            minDisPosList.Insert(j, i);//插入该位置
                            if (minDisPosList.Count > 3) minDisPosList.RemoveAt(minDisPosList.Count - 1);//移除后面的
                            isInsert = true;
                            break;
                        }
                    }
                    if (!isInsert && minDisPosList.Count < 3)
                    {//没有插入
                        minDisPosList.Add(i);//在最后记录下该位置
                    }
                    //if (minDisPosList.Count > 3) minDisPosList.RemoveRange(3, minDisPosList.Count() - 3);//移除后面的
                }
                r = distances.Min();//选取最小距离最为圆的半径
                //if(r>1000)
                //{
                //    r = 0;
                //}
                standDev = GetStandDev(distances[minDisPosList[0]], distances[minDisPosList[1]], distances[minDisPosList[2]]);//计算标准

                if (standDev < (stepOir * 0.01))
                {//标准差很小
                    break;//结束迭代运算
                }
                else
                {
                    int minDisPos = minDisPosList[0];//最短距离位置
                    double[] pointMove = GetMovePoint(disPoints[minDisPos, 0], disPoints[minDisPos, 1], x, y, step);//获取移动后的点
                    x = pointMove[0];
                    y = pointMove[1];
                    double[] pointMove2 = GetMovePoint(disPoints[minDisPos, 0], disPoints[minDisPos, 1], 561, 639.5, step);//获取移动后的点
                }

                if (standDev >= lastStandDev)
                {//标准差变大了

                    step = step * 0.5;//调整距离变为减半
                }
                lastStandDev = standDev;//记录标准差

                if (step < (stepOir * (Math.Pow(0.5, 100))))
                {//调整很小
                    break;//结束迭代运算
                }
            }
            Circle cir = new Circle(x, y, r);
            return cir;
        }

        private double[] GetMovePoint(double x1, double y1, double x, double y, double step)
        {
            double length = Math.Sqrt(Math.Pow((x1 - x), 2) + Math.Pow((y1 - y), 2));//线段长度
            double xm = x + ((x - x1) / length) * step;//计算移动后的x坐标
            double ym = y + ((y - y1) / length) * step;//计算移动后的y坐标
            double[] mp = { xm, ym };
            return mp;
        }

        /// <summary>
        /// 计算标准差
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        private double GetStandDev(double v1, double v2, double v3)
        {
            double avg = (v1 + v2 + v3) / 3.0;//平均值
            double variance = (Math.Pow(v1 - avg, 2) + Math.Pow(v2 - avg, 2) + Math.Pow(v3 - avg, 2)) / 3;//方差
            double dev = Math.Sqrt(variance);//标准差
            return dev;
        }

        private double GetPointLineDistance(double x, double y, double px, double py, double nx, double ny, ref double[] disPoint)
        {
            double distance = 0.0;
            if (px == nx)
            {//竖线
                if (y > Max(py, ny) || y < Min(py, ny))
                {//垂足不在线段上
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        disPoint[0] = px;
                        disPoint[1] = py;
                        distance = d1;
                    }
                    else
                    {
                        disPoint[0] = nx;
                        disPoint[1] = ny;
                        distance = d2;
                    }
                }
                else
                {
                    disPoint[0] = px;
                    disPoint[1] = y;
                    distance = Math.Abs(x - px);
                }
            }
            else if (py == ny)
            {//横线
                if (x > Max(px, nx) || x < Min(px, nx))
                {
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        disPoint[0] = px;
                        disPoint[1] = py;
                        distance = d1;
                    }
                    else
                    {
                        disPoint[0] = nx;
                        disPoint[1] = ny;
                        distance = d2;
                    }
                }
                else
                {
                    disPoint[0] = x;
                    disPoint[1] = py;
                    distance = Math.Abs(y - py);
                }
            }
            else
            {
                double A = ny - py;
                double B = px - nx;
                double C = nx * py - px * ny;
                double d =Math.Abs((A*x + B*y + C) / Math.Sqrt(A * A + B * B));
                double footX = (B * B * x - A * B * y - A * C) / (A * A + B * B);
                double footY = (-A * B * x + A * A * y - B * C) / (A * A + B * B);
                double pd = GetPointDistance(x, y, px, py);//计算两点间的位置
                double nd = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                if ((footX<px&& footX < nx)||(footX > px && footX > nx))
                {
                    if(pd<nd)
                    {
                        disPoint[0] = px;
                        disPoint[1] = py;
                        distance = pd;
                    }
                    else
                    {
                        disPoint[0] = nx;
                        disPoint[1] = ny;
                        distance = nd;
                    }
                }
                else
                {
                    disPoint[0] = footX;
                    disPoint[1] = footY;
                    distance = d;
                }
            }
            return distance;
        }

        private double GetPointLineDistance(double x, double y, double px, double py, double nx, double ny)
        {
            double distance = 0.0;
            if (px == nx)
            {//竖线
                if (y > Max(py, ny) || y < Min(py, ny))
                {//垂足不在线段上
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        distance = d1;
                    }
                    else
                    {
                        distance = d2;
                    }
                }
                else
                {
                    distance = Math.Abs(x - px);
                }
            }
            else if (py == ny)
            {//横线
                if (x > Max(px, nx) || x < Min(px, nx))
                {
                    //distance = double.MaxValue;
                    double d1 = GetPointDistance(x, y, px, py);//计算两点间的位置
                    double d2 = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                    if (d1 < d2)
                    {
                        distance = d1;
                    }
                    else
                    {
                        distance = d2;
                    }
                }
                else
                {
                    distance = Math.Abs(y - py);
                }
            }
            else
            {
                double A = ny - py;
                double B = px - nx;
                double C = nx * py - px * ny;
                double d = Math.Abs((A * x + B * y + C) / Math.Sqrt(A * A + B * B));
                double footX = (B * B * x - A * B * y - A * C) / (A * A + B * B);
                double footY = (-A * B * x + A * A * y - B * C) / (A * A + B * B);
                double pd = GetPointDistance(x, y, px, py);//计算两点间的位置
                double nd = GetPointDistance(x, y, nx, ny);//计算两点间的位置
                if ((footX < px && footX < nx) || (footX > px && footX > nx))
                {
                    if (pd < nd)
                    {
                        distance = pd;
                    }
                    else
                    {
                        distance = nd;
                    }
                }
                else
                {
                    distance = d;
                }
            }
            return distance;
        }

        /// <summary>
        /// 计算两点间的距离
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double GetPointDistance(double x1, double y1, double x2, double y2)
        {
            double dis = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            return dis;
        }

        /// <summary>
        /// 返回两者较小值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private double Min(double v1, double v2)
        {
            if (v1 < v2) return v1;
            else return v2;
        }

        /// <summary>
        /// 返回两者较大值
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private double Max(double v1, double v2)
        {
            if (v1 > v2) return v1;
            else return v2;
        }
        #endregion

        #region 最小面积外接圆
        internal Circle GetMinOutCir()
        {
            Circle minCircle = new Circle();

            List<StormPoint> pointList = lineList[0].pointList;

            double[,] points = new double[pointList.Count, 2];

            for (int i = 0; i < pointList.Count; i++)
            {
                points[i, 0] = pointList[i].log;
                points[i, 1] = pointList[i].lat;
            }

            int minAngle = 0;//符合条件时旋转角度

            for (int angle = 0; angle < 360; angle++)
            {//旋转每个角度
                double[,] pointsR = getPointsRotate(points, angle);//获取旋转后的点
                Circle cir = getVerCir(pointsR);//计算以垂直方向最大最小点确定的面积最小的点
                if (cir.r > minCircle.r)
                {//更大半径的外接圆
                    minCircle = cir;
                    minAngle = angle;
                }
            }

            Circle minRecR = getCircleRotate(minCircle, -minAngle);//获取旋转后的矩形

            return minRecR;
        }

        private Circle getCircleRotate(Circle minCircle, int angle)
        {
            double[] pointR = GetPointRotate(minCircle.x, minCircle.y, angle);
            Circle minCirR = new Circle(pointR[0], pointR[1], minCircle.r);
            return minCirR;
        }

        private Circle getVerCir(double[,] pointsR)
        {
            double[] maxYPoint = { 0, double.MinValue };
            double[] minYPoint = { 0, double.MaxValue };
            for (int i = 0; i < pointsR.GetLength(0); i++)
            {
                double[] point = { pointsR[i, 0], pointsR[i, 1] };//取出该点
                if (point[1] > maxYPoint[1]) maxYPoint = point;//记录y值更大的点
                if (point[1] < minYPoint[1]) minYPoint = point;//记录y值更小的点
            }
            double x = (maxYPoint[0] + minYPoint[0]) / 2;//圆心横坐标
            double y = (maxYPoint[1] + minYPoint[1]) / 2;//圆心纵坐标
            double r = (maxYPoint[1] - minYPoint[1]) / 2;//圆心半径
            Circle cir = new Circle(x, y, r);
            return cir;
        }

        #endregion
    }
}
