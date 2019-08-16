using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem.Class
{
    public partial class Sequence
    {
        public enum SequenceType { SequenceNode, LinkedNode };
        public SequenceType SeqType;
        public MarineHeatwavePolygon Head;
        public MarineHeatwavePolygon Tail;
        public List<MarineHeatwavePolygon> SQList;
        public string SeqID;
        public List<string> ParentsSeqID;
        public List<string> ChildrenSeqID;
        public string wkt;
        public double MaxValue = double.MinValue;
        public double MinValue = double.MaxValue;
        public double AvgValue = 0;
        public double Spped;
        public double Theta;//方向 tan值

        public Sequence()
        {
            SQList = new List<MarineHeatwavePolygon>();
            ParentsSeqID = new List<string>();
            ChildrenSeqID = new List<string>();
            MarineHeatwavePolygon Head = new MarineHeatwavePolygon();
            MarineHeatwavePolygon Tail = new MarineHeatwavePolygon();
        }

        public void ComputeAttributes()
        {
            //计算方向,指北向为0度方向，顺时针旋转
            double dLon = Math.Abs(SQList[0].coreLog - SQList[SQList.Count - 1].coreLog);
            double dLat = Math.Abs(SQList[0].coreLat - SQList[SQList.Count - 1].coreLat);
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog > 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat > 0)//第一象限
            {
                Theta = 90 - Math.Atan(dLat / dLon);
            }
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog < 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat > 0)//第二象限
            {
                Theta = Math.Atan(dLat / dLon) + 270;
            }
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog < 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat < 0)//第三象限
            {
                Theta = 270 - Math.Atan(dLat / dLon);
            }
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog > 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat < 0)//第四象限
            {
                Theta = Math.Atan(dLat / dLon) + 90;
            }
            ////
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog > 0 && dLat == 0)//水平向右
            {
                Theta = 90;
            }
            if (SQList[SQList.Count - 1].coreLog - SQList[0].coreLog < 0 && dLat == 0)//水平向左
            {
                Theta = 270;
            }
            if (dLon == 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat < 0)//竖直向下
            {
                Theta = 180;
            }
            if (dLon == 0 && SQList[SQList.Count - 1].coreLat - SQList[0].coreLat > 0)//竖直向上
            {
                Theta = 0;
            }
            //计算速度


            //计算最大、最小、均值
            double areaSum = 0;
            for (int i=0;i<SQList.Count;i++)
            {
                if (SQList[i].iMax>MaxValue)
                {
                    MaxValue = SQList[i].iMax;
                }
                if (SQList[i].iMin<MinValue)
                {
                    MinValue = SQList[i].iMin;
                }
                AvgValue += SQList[i].area * SQList[i].iMean;
                areaSum += SQList[i].area;
            }
            AvgValue = AvgValue / areaSum;
        }
    }
}
