using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem.GUI
{
    internal class MarineHeatwavePolygonList
    {
        public List<MarineHeatwavePolygon> mhPolygons = new List<MarineHeatwavePolygon>();//记录包含的所有暴雨多边形
        public MarineHeatwavePolygon head = new MarineHeatwavePolygon();//头
        public MarineHeatwavePolygon tail = new MarineHeatwavePolygon();//尾
        public double[] prePos = new double[2];//头节点前一点预测位置
        public double[] nextPos = new double[2];//尾节点后一点预测位置
        //double dirLast = 0.0;
        //double speedLast = 0.0;
        double uSpeedLast = 0.0;
        double vSpeedLast = 0.0;

        //double dirNext = 0.0;
        //double speedNext = 0.0;
        double uSpeedNext = 0.0;
        double vSpeedNext = 0.0;

        public double[,] preMinAreaRec = new double[5, 2];
        public double preRecArea = 0.0;
        public double[,] nextMinAreaRec = new double[5, 2];
        public double nextRecArea = 0.0;

        public MarineHeatwavePolygonList()
        {

        }
        public MarineHeatwavePolygonList(MarineHeatwavePolygon _head)
        {
            head = _head;
        }

        /// <summary>
        /// 以头节点为起始，取出所有子节点及子节点的子节点
        /// </summary>
        internal void GetList()
        {
            mhPolygons.Clear();//首先清空
            tail = head;//记录当前节点
            mhPolygons.Add(tail);//添加头节点
            while (tail.childList.Count > 0)
            {//存在子节点
                tail = tail.childList[0];//取子节点
                mhPolygons.Add(tail);//保存子节点
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
            for (int i = 0; i < 5; i++)
            {
                double log = _minAreaRec[i, 0];
                double lat = _minAreaRec[i, 1];
                double[] prePos = GetForePos(log, lat, uSpeedLast, vSpeedLast, timeCell);
                preMinAreaRec[i, 0] = prePos[0];
                preMinAreaRec[i, 1] = prePos[1];
            }
        }

        internal void CalculatNextRec(double timeCell)
        {
            double[,] _minAreaRec = tail.minAreaRec;
            nextRecArea = tail.lengthMax * tail.widthMax;
            for (int i = 0; i < 5; i++)
            {
                double log = _minAreaRec[i, 0];
                double lat = _minAreaRec[i, 1];
                double[] nextPos = GetForePos(log, lat, uSpeedNext, vSpeedNext, timeCell);
                nextMinAreaRec[i, 0] = nextPos[0];
                nextMinAreaRec[i, 1] = nextPos[1];
            }
        }

        /// <summary>
        /// 计算头节点上一点质心预测位置
        /// </summary>
        internal void CalculatePrePos(double timeCell)
        {
            MarineHeatwavePolygon poly = head;
            int timeId = 1;//用来判断当前多边形时刻

            double areaWeighSum = poly.area / timeId;//面积加权和
            double uSpeedWeighSum = areaWeighSum * poly.uSpeed;//方向加权
            double vSpeedWeighSum = areaWeighSum * poly.vSpeed;//速度加权
            while (poly.childList.Count > 0)
            {//子节点不为空
                poly = poly.childList[0];//替换为子节点
                timeId++;//时刻加1
                areaWeighSum += poly.area / timeId;//面积加权和
                uSpeedWeighSum += poly.uSpeed * poly.area / timeId;//方向加权
                vSpeedWeighSum += poly.vSpeed * poly.area / timeId;//方向加权
            }
            uSpeedLast = -uSpeedWeighSum / areaWeighSum;//最后一点方向 取反方向
            vSpeedLast = -vSpeedWeighSum / areaWeighSum;//最后一点速度
            prePos = GetForePos(head.coreLog, head.coreLat, uSpeedLast, vSpeedLast, timeCell);//获取预测位置
        }

        /// <summary>
        /// 计算尾节点下一点质心预测位置
        /// </summary>
        internal void CalculateNextPos(double timeCell)
        {
            MarineHeatwavePolygon poly = head;
            int timeId = 1;//用来判断当前多边形时刻

            double areaWeighSum = poly.area / (mhPolygons.Count() - timeId + 1);//面积加权和
            double uSpeedWeighSum = areaWeighSum * poly.uSpeed;//方向加权
            double vSpeedWeighSum = areaWeighSum * poly.vSpeed;//速度加权
            while (poly.childList.Count > 0)
            {//子节点不为空
                poly = poly.childList[0];//替换为子节点
                timeId++;//时刻加1
                areaWeighSum += poly.area / (mhPolygons.Count() - timeId + 1);//面积加权和
                uSpeedWeighSum += poly.uSpeed * poly.area / (mhPolygons.Count() - timeId + 1);//方向加权
                vSpeedWeighSum += poly.vSpeed * poly.area / (mhPolygons.Count() - timeId + 1);//方向加权
            }
            uSpeedNext = uSpeedWeighSum / areaWeighSum;//最后一点方向
            vSpeedNext = vSpeedWeighSum / areaWeighSum;//最后一点速度
            nextPos = GetForePos(poly.coreLog, poly.coreLat, uSpeedNext, vSpeedNext, timeCell);//获取预测位置
        }

        /// <summary>
        /// 获取预测位置
        /// </summary>
        /// <param name="coreLog"></param>
        /// <param name="coreLat"></param>
        /// <param name="uSpeedLast"></param>
        /// <param name="vSpeedLast"></param>
        /// <param name="timeCell"></param>
        /// <returns></returns>
        private double[] GetForePos(double coreLog, double coreLat, double uSpeedLast, double vSpeedLast, double timeCell)
        {
            //double length = uSpeedLast * timeCell;//移动距离
            double logDisKm = uSpeedLast * 60 * 60 * 24 * timeCell / 1000.0;//经度方向移动距离
            double latDisKm = vSpeedLast * 60 * 60 * 24 * timeCell / 1000.0;//维度方向移动距离
            double logDis = logDisKm / (Earth.OneLogLen(coreLat) / 1000.0);//移动经度
            double latDis = latDisKm / (Earth.OneLatLen() / 1000.0);//移动纬度
            double coreLogFore = coreLog + logDis;//预测经度
            double coreLatFore = coreLat + latDis;//预测纬度
            double[] forePos = { coreLogFore, coreLatFore };
            return forePos;
        }
    }
}