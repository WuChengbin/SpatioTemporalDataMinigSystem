using System;
using System.Linq;
using System.Windows;

namespace MarineSTMiningSystem
{
    public static class MCC
    {
        private const double eps = 1e-7; //极小
        private const int LSFail = 1;
        private const int OverIteration = 2;
        private const int Maxiteration = 100;
        private static int iteration = 0;

        public static int MCCircleFit(double[] x, double[] y, ref double xc, ref double yc, ref double r)
        {
            Point centre;
            iteration = 0;
            if (x == null || x.Length < 1)
                return -3;
            if (y == null || y.Length < 1)
                return -3;
            if (x.Length != y.Length)
                return -3;

            //0:坐标转换
            //double[] x = new double[angle.Length];
            //double[] y = new double[angle.Length];
            double[] d = new double[x.Length];
            int[] maxindex = new int[3]; //记录最大的三个数
            /*
            for (i = 0; i < angle.Length; i++)
            {
                x[i] = radius[i] * Math.Cos(angle[i] * Math.PI / 180);
                y[i] = radius[i] * Math.Sin(angle[i] * Math.PI / 180);
            }
            */
            //1:用最小二乘圆心做为起始圆心
            if (LS(x, y, out centre) != 0)
                return LSFail;

            //2:找两点半径一致
            while (iteration < Maxiteration)
            {
                CalDistance(x, y, centre, d, out maxindex[0], out maxindex[1], out maxindex[2]); //计算所有点到圆心的值
                if (Math.Abs(d[maxindex[0]] - d[maxindex[1]]) < eps) //认为相等,找到L2
                {
                    break;
                }
                centre = Phase2FindNextCentre(x, y, d, maxindex[0], centre); //找到下一步的圆心
                iteration++;
            }

            //2.x 判断是否吻合标准二
            Point ChordCentrePoint = new Point((x[maxindex[1]] + x[maxindex[0]]) / 2, (y[maxindex[1]] + y[maxindex[0]]) / 2);
            double O_OCDistance = Math.Sqrt((ChordCentrePoint.X - centre.X) * (ChordCentrePoint.X - centre.X) + (ChordCentrePoint.Y - centre.Y) * (ChordCentrePoint.Y - centre.Y));
            if (O_OCDistance < eps) //同心判断标准
            {
                xc = centre.X;
                yc = centre.Y;
                r = d[maxindex[0]]; //半径
                return 0;
            }
            /*
            Point vectorAB = new Point(x[maxindex[1]] - x[maxindex[0]], y[maxindex[1]] - y[maxindex[0]]);
            Point vectorOB = new Point(x[maxindex[1]] - centre.X, y[maxindex[1]] - centre.Y);
            double k1=0;
            if (Math.Abs(vectorAB.X -vectorOB.X) < eps)
                k1 =1;
            else
            {
                if (vectorOB.X ==0)
                    vectorOB.X =eps;
                k1 = vectorAB.X / vectorOB.X;
            }
            double k2=0; 
            if (Math.Abs(vectorAB.Y -vectorOB.Y) <eps)
                k2 =1;
            else
            {
                if (vectorOB.Y == 0)
                    vectorOB.Y = eps;
                k2 = vectorAB.Y / vectorOB.Y;
            }
            if (Math.Abs(k1 - k2) < eps) //同心判断标准
            {
                xc = centre.X;
                yc = centre.Y;
                r = d[maxindex[0]]; //半径
                return 0;
            }
             */

            //3:寻找第三个半径一致
            while (iteration < Maxiteration)
            {
                if (Math.Abs(d[maxindex[0]] - d[maxindex[1]]) < eps && Math.Abs(d[maxindex[1]] - d[maxindex[2]]) < eps) //三点等距找到
                {
                    //判断是否符合标准三
                    //A为0
                    Point AB = new Point(x[maxindex[1]] - x[maxindex[0]], y[maxindex[1]] - y[maxindex[0]]); //向量AB
                    Point AC = new Point(x[maxindex[2]] - x[maxindex[0]], y[maxindex[2]] - y[maxindex[0]]); //向量AC
                    //B为1
                    Point BA = new Point(x[maxindex[0]] - x[maxindex[1]], y[maxindex[0]] - y[maxindex[1]]); //向量BA
                    Point BC = new Point(x[maxindex[2]] - x[maxindex[1]], y[maxindex[2]] - y[maxindex[1]]); //向量BC
                    //C为2
                    Point CA = new Point(x[maxindex[0]] - x[maxindex[2]], y[maxindex[0]] - y[maxindex[2]]); //向量CA
                    Point CB = new Point(x[maxindex[1]] - x[maxindex[2]], y[maxindex[1]] - y[maxindex[2]]); //向量CB

                    double moAB = Math.Sqrt(AB.X * AB.X + AB.Y * AB.Y);
                    double moAC = Math.Sqrt(AC.X * AC.X + AC.Y * AC.Y);
                    double moBC = Math.Sqrt(BC.X * BC.X + BC.Y * BC.Y);

                    if ((AB.X * AC.X + AB.Y * AC.Y) > 0 && (BA.X * BC.X + BA.Y * BC.Y) > 0 && (CA.X * CB.X + CA.Y * CB.Y) > 0) //全锐角,符合三点标准
                    {
                        break;
                    }
                    else //三点等距，不满足三点准则
                    {
                        bool isTwoPointCondition = false;
                        if (moAB >= moAC && moAB >= moBC) //使用AB两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, maxindex[0], maxindex[1], maxindex[2], centre, out isTwoPointCondition);
                            iteration++;
                        }
                        else if (moAC >= moAB && moAC >= moBC) //使用AC两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, maxindex[0], maxindex[2], maxindex[1], centre, out isTwoPointCondition);
                            iteration++;
                        }
                        else if (moBC >= moAB && moBC >= moAC) //使用BC两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, maxindex[1], maxindex[2], maxindex[0], centre, out isTwoPointCondition);
                            iteration++;
                        }
                        if (isTwoPointCondition == true) //转回两点模式
                        {
                            CalDistance(x, y, centre, d, out maxindex[0], out maxindex[1], out maxindex[2]); //计算所有点到圆心的值
                            break;
                        }
                    }
                }
                else //未找到三点等距，用等距的两点来继续寻找
                {
                    centre = Phase3FindNextCentre(x, y, d, maxindex[0], maxindex[1], centre); //找到下一步的圆心
                    iteration++;
                }
                CalDistance(x, y, centre, d, out maxindex[0], out maxindex[1], out maxindex[2]); //计算所有点到圆心的值
            }

            //找到对应的拟合圆
            xc = centre.X;
            yc = centre.Y;
            r = d[maxindex[0]];

            if (iteration >= Maxiteration)
                return OverIteration;
            return 0;
        }
        private static void CalDistance(double[] x, double[] y, Point centre, double[] d, out int maxindex1, out int maxindex2, out int maxindex3)
        {
            int i;
            maxindex1 = -1;
            maxindex2 = -1;
            maxindex3 = -1;

            for (i = 0; i < d.Length; i++)
            {
                d[i] = Math.Sqrt((x[i] - centre.X) * (x[i] - centre.X) + (y[i] - centre.Y) * (y[i] - centre.Y));
                //寻找前三甲
                if (maxindex1 < 0 || d[i] > d[maxindex1])
                {
                    maxindex1 = i;
                }
                else if (maxindex2 < 0 || d[i] > d[maxindex2])
                {
                    maxindex2 = i;
                }
                else if (maxindex3 < 0 || d[i] > d[maxindex3])
                {
                    maxindex3 = i;
                }
            }
        }
        private static Point Phase2FindNextCentre(double[] x, double[] y, double[] d, int maxindex, Point centre)
        {
            int i;
            //int maxindex = 0;
            //double []d= new double[x.Length];
            double[] t = new double[x.Length - 1]; //步长阵列
            //计算圆心移动方向
            Point L1vector = new Point(x[maxindex] - centre.X, y[maxindex] - centre.Y); //计算圆心到最大距离点的方向向量
            double mo = Math.Sqrt(L1vector.X * L1vector.X + L1vector.Y * L1vector.Y); //模
            Point Tkvector = new Point(L1vector.X / mo, L1vector.Y / mo); //前进单位方向向量
            //单位方向向量
            //L1vector.X /= mo;
            //L1vector.Y /= mo;
            double[] CosAlpha = new double[x.Length]; //cosα阵列
            for (i = 0; i < CosAlpha.Length; i++)
            {
                Point ivector = new Point(x[i] - centre.X, y[i] - centre.Y); //i向量
                CosAlpha[i] = (ivector.X * Tkvector.X + ivector.Y * Tkvector.Y) / Math.Sqrt((ivector.X * ivector.X + ivector.Y * ivector.Y)); //计算cosα
            }
            int j;
            for (j = 0, i = 0; i < x.Length; i++)
            {
                if (i == maxindex) //跳过不处理
                    continue;
                t[j] = (d[maxindex] * d[maxindex] - d[i] * d[i]) / 2 / (d[maxindex] * CosAlpha[maxindex] - d[i] * CosAlpha[i]);
                j++;
            }

            double tk = -1; // = t.Min(); //步进长度
            for (i = 0; i < t.Length; i++)
            {
                if (t[i] >= 0 && (t[i] < tk || tk < 0)) //寻找正的最小值
                {
                    tk = t[i];
                }
            }

            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }
        //没找到三点等距的情况下
        private static Point Phase3FindNextCentre(double[] x, double[] y, double[] d, int maxindex1, int maxindex2, Point centre)
        {
            int i;
            double[] t = new double[x.Length - 2];
            Point ChordCenterPoint = new Point((x[maxindex1] + x[maxindex2]) / 2, (y[maxindex1] + y[maxindex2]) / 2); //两点的中点
            Point Tkvector = new Point(ChordCenterPoint.X - centre.X, ChordCenterPoint.Y - centre.Y); //前进单位方向向量
            double motk = Math.Sqrt(Tkvector.X * Tkvector.X + Tkvector.Y * Tkvector.Y); //求圆心到弦中点的模
            //模单位化
            Tkvector.X /= motk;
            Tkvector.Y /= motk;

            double[] CosAlpha = new double[x.Length]; //cosα阵列
            for (i = 0; i < CosAlpha.Length; i++)
            {
                Point ivector = new Point(x[i] - centre.X, y[i] - centre.Y); //i向量
                CosAlpha[i] = (ivector.X * Tkvector.X + ivector.Y * Tkvector.Y) / Math.Sqrt((ivector.X * ivector.X + ivector.Y * ivector.Y)); //计算cosα
            }

            int j;
            for (j = 0, i = 0; i < x.Length; i++)
            {
                if (i == maxindex1 || i == maxindex2) //跳过不处理
                    continue;
                t[j] = (d[maxindex1] * d[maxindex1] - d[i] * d[i]) / 2 / (d[maxindex1] * CosAlpha[maxindex1] - d[i] * CosAlpha[i]);
                j++;
            }
            //t[t.Length - 1] = motk;

            double tk = -1; // = t.Min(); //步进长度
            for (i = 0; i < t.Length; i++)
            {
                if (t[i] >= 0 && (t[i] < tk || tk < 0)) //寻找正的最小值
                {
                    tk = t[i];
                }
            }
            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }
        private static Point Phase3FindNextCentre(double[] x, double[] y, double[] d, int maxindex1, int maxindex2, int exceptIndex, Point centre, out bool IsTwoPointCondition)
        {
            int i;
            double[] t = new double[x.Length - 3];
            Point ChordCenterPoint = new Point((x[maxindex1] + x[maxindex2]) / 2, (y[maxindex1] + y[maxindex2]) / 2); //两点的中点
            Point Tkvector = new Point(ChordCenterPoint.X - centre.X, ChordCenterPoint.Y - centre.Y); //前进单位方向向量
            double motk = Math.Sqrt(Tkvector.X * Tkvector.X + Tkvector.Y * Tkvector.Y); //求圆心到弦中点的模
            //模单位化
            Tkvector.X /= motk;
            Tkvector.Y /= motk;

            double[] CosAlpha = new double[x.Length]; //cosα阵列
            for (i = 0; i < CosAlpha.Length; i++)
            {
                Point ivector = new Point(x[i] - centre.X, y[i] - centre.Y); //i向量
                CosAlpha[i] = (ivector.X * Tkvector.X + ivector.Y * Tkvector.Y) / Math.Sqrt((ivector.X * ivector.X + ivector.Y * ivector.Y)); //计算cosα
            }

            int j;
            for (j = 0, i = 0; i < x.Length; i++)
            {
                if (i == maxindex1 || i == maxindex2 || i == exceptIndex) //跳过不处理
                    continue;
                t[j] = (d[maxindex1] * d[maxindex1] - d[i] * d[i]) / 2 / (d[maxindex1] * CosAlpha[maxindex1] - d[i] * CosAlpha[i]);
                j++;
            }

            double tk = -1; // = t.Min(); //步进长度
            for (i = 0; i < t.Length; i++)
            {
                if (t[i] >= 0 && (t[i] < tk || tk < 0)) //寻找正的最小值
                {
                    tk = t[i];
                }
            }

            if (tk > motk)
            {
                tk = motk;
                IsTwoPointCondition = true;
            }
            else
                IsTwoPointCondition = false;

            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }
        private static int LS(double[] x, double[] y, out Point centre)
        {
            try
            {
                double x0 = 0;
                double y0 = 0;
                //Point centre =new Point();
                x0 = x.Sum();
                y0 = y.Sum();
                x0 = x0 * 2.0 / x.Length;
                y0 = y0 * 2.0 / x.Length;
                centre = new Point(x0, y0);
                return 0;
            }
            catch
            {
                centre = new Point(0, 0);
                return -1;
            }
        }
    }
}
