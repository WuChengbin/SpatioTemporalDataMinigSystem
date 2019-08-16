using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class StormPolygonList
    {
        public List<StormPolygon> stormPolygons = new List<StormPolygon>();//记录包含的所有暴雨多边形
        public StormPolygon head = new StormPolygon();//头
        public StormPolygon tail =new StormPolygon();//尾
        public double[] prePos = new double[2];//头节点前一点预测位置
        public double[] nextPos = new double[2];//尾节点后一点预测位置
        double dirLast = 0.0;
        double speedLast = 0.0;
        double dirNext = 0.0;
        double speedNext = 0.0;
        public double[,] preMinAreaRec = new double[5, 2];
        public double preRecArea = 0.0;
        public double[,] nextMinAreaRec = new double[5, 2];
        public double nextRecArea = 0.0;
        public StormPolygonList()
        {

        }
        public StormPolygonList(StormPolygon _head)
        {
            head = _head;
        }

        /// <summary>
        /// 以头节点为起始，取出所有子节点及子节点的子节点
        /// </summary>
        internal void GetList()
        {
            stormPolygons.Clear();//首先清空
            tail = head;//记录当前节点
            stormPolygons.Add(tail);//添加头节点
            while(tail.childList.Count>0)
            {//存在子节点
                tail = tail.childList[0];//取子节点
                stormPolygons.Add(tail);//保存子节点
            }
        }

        /// <summary>
        /// 计算前预测矩形
        /// </summary>
        /// <param name="timeCell"></param>
        internal void CalculatPreRec(double timeCell)
        {
            double[,] _minAreaRec = head.minAreaRec;
            preRecArea = head.lengthMax * head.widthMax;
            for (int i=0;i<5;i++)
            {
                double log = _minAreaRec[i, 0];
                double lat = _minAreaRec[i, 1];
                double[] prePos= GetForePos(log, lat, speedLast, dirLast, timeCell);
                preMinAreaRec[i, 0] = prePos[0];
                preMinAreaRec[i, 1] = prePos[1];
            }
        }

        internal void CalculatNextRec(double timeCell)
        {
            double[,] _minAreaRec = tail.minAreaRec;
            nextRecArea = tail.lengthMax * tail.widthMax;
            for (int i = 0;i< 5;i++)
            {
                double log = _minAreaRec[i, 0];
                double lat = _minAreaRec[i, 1];
                double[] nextPos = GetForePos(log, lat, speedNext, dirNext, timeCell);
                nextMinAreaRec[i, 0] = nextPos[0];
                nextMinAreaRec[i, 1] = nextPos[1];
            }
        }

        internal void UnityDirect()
        {
            if(stormPolygons.Count>2)
            {
                StormPolygon prePoly = head.childList[0];
                StormPolygon nextPoly;
                double dir1 = 0.0;
                double dir2 = 0.0;
                while(true)
                {
                    nextPoly = prePoly.childList[0];
                    dir1 = prePoly.direct;
                    dir2 = nextPoly.direct;
                    if (Math.Abs(dir1 - dir2) > 180)
                    {
                        if (dir1 > dir2)
                        {
                            prePoly.direct -= 360;
                        }
                        else
                        {
                            nextPoly.direct -= 360;
                        }
                    }
                    if(nextPoly.childList.Count>0)
                    {
                        prePoly = nextPoly;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
        }

        /// <summary>
        /// 计算头节点上一点质心预测位置
        /// </summary>
        internal void CalculatePrePos(double timeCell)
        {
            StormPolygon poly = head;
            int timeId = 1;//用来判断当前多边形时刻
            if (poly.childList.Count > 0)
            {
                poly.speed = poly.childList[0].speed;//头节点方向更换子节点速度
                poly.direct = poly.childList[0].direct;//头节点方向更换子节点方向
            }
            double areaWeighSum = poly.area / timeId;//面积加权和
            double dirWeighSum = areaWeighSum * poly.direct;//方向加权
            double speedWeighSum = areaWeighSum * poly.speed;//速度加权
            while (poly.childList.Count > 0)
            {//子节点不为空
                poly = poly.childList[0];//替换为子节点
                timeId++;//时刻加1
                areaWeighSum += poly.area / timeId;//面积加权和
                dirWeighSum += poly.direct * poly.area / timeId;//方向加权
                speedWeighSum += poly.speed * poly.area / timeId;//方向加权
            }
            double _dirLast = dirWeighSum / areaWeighSum;//最后一点方向
            dirLast = _dirLast - 180;//取反方向
            speedLast = speedWeighSum / areaWeighSum;//最后一点速度
            prePos = GetForePos(head.coreLog, head.coreLat, speedLast, dirLast, timeCell);//获取预测位置
        }

        /// <summary>
        /// 计算尾节点下一点质心预测位置
        /// </summary>
        internal void CalculateNextPos(double timeCell)
        {
            StormPolygon poly = head;
            int timeId = 1;//用来判断当前多边形时刻
            if (poly.childList.Count > 0)
            {
                poly.speed= poly.childList[0].speed;//头节点方向更换子节点速度
                poly.direct = poly.childList[0].direct;//头节点方向更换子节点方向
            }
            double areaWeighSum = poly.area / (stormPolygons.Count() - timeId + 1);//面积加权和
            double dirWeighSum = areaWeighSum * poly.direct;//方向加权
            double speedWeighSum = areaWeighSum * poly.speed;//速度加权
            while (poly.childList.Count>0)
            {//子节点不为空
                poly = poly.childList[0];//替换为子节点
                timeId++;//时刻加1
                areaWeighSum+= poly.area / (stormPolygons.Count() - timeId + 1);//面积加权和
                dirWeighSum += poly.direct * poly.area / (stormPolygons.Count() - timeId + 1);//方向加权
                speedWeighSum += poly.speed * poly.area / (stormPolygons.Count() - timeId + 1);//方向加权
            }
            dirNext = dirWeighSum / areaWeighSum;//最后一点方向
            speedNext = speedWeighSum / areaWeighSum;//最后一点速度
            nextPos = GetForePos(poly.coreLog, poly.coreLat, speedNext, dirNext, timeCell);//获取预测位置
        }

        /// <summary>
        /// 获取预测位置
        /// </summary>
        /// <param name="coreLog"></param>
        /// <param name="coreLat"></param>
        /// <param name="speedLast"></param>
        /// <param name="dirLast"></param>
        /// <param name="timeCell"></param>
        /// <returns></returns>
        private double[] GetForePos(double coreLog, double coreLat, double speedLast, double dirLast,double timeCell)
        {
            double length = speedLast * timeCell;//移动距离
            double logDisKm = length * Math.Sin(dirLast * Math.PI / 180);//经度方向移动距离
            double latDisKm = length * Math.Cos(dirLast * Math.PI / 180);//维度方向移动距离
            double logDis = logDisKm / (Earth.OneLogLen(coreLat) / 1000.0);//移动经度
            double latDis = latDisKm / (Earth.OneLatLen() / 1000.0);//移动纬度
            double coreLogFore = coreLog + logDis;//预测经度
            double coreLatFore = coreLat + latDis;//预测纬度
            double[] forePos = { coreLogFore, coreLatFore };
            return forePos;
        }
    }
}
