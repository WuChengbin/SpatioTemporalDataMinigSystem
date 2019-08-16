using System;
using System.Linq;
using System.Windows;

namespace MarineSTMiningSystem
{
    public class MIC
    {
        private const double eps = 1e-7; //极小
        private const int LSFail = 1;
        private const int OverIteration = 2;
        private const int Maxiteration = 100;
        private static int iteration = 0;
        public static int MICircleFit(double[] x, double[] y, ref double xc, ref double yc, ref double r)
        {
            //int i;
            Point centre;
            iteration = 0; //迭代清零
            if (x == null || x.Length < 1)
                return -3;
            if (y == null || y.Length < 1)
                return -3;
            if (x.Length != y.Length)
                return -3;

            double[] d = new double[x.Length];
            int[] minindex = new int[3]; //记录最小的三个数的索引

            //1:用最小二乘圆心做为起始圆心
            if (LS(x, y, out centre) != 0)
                return LSFail;

            //2:找两点半径一致
            while (iteration < Maxiteration)
            {
                CalDistance(x, y, centre, d, out minindex[0], out minindex[1], out minindex[2]); //计算所有点到圆心的值
                if (Math.Abs(d[minindex[0]] - d[minindex[1]]) < eps) //认为相等,找到S2
                {
                    break;
                }
                centre = Phase2FindNextCentre(x, y, d, minindex[0], centre); //找到下一步的圆心
                iteration++;
            }

            //2.x 判断是否吻合标准二
            /*
            Point ChordCentrePoint = new Point((x[minindex[1]] + x[minindex[0]])/2, (y[minindex[1]] + y[minindex[0]])/2);
            double O_OCDistance = Math.Sqrt((ChordCentrePoint.X - centre.X) * (ChordCentrePoint.X - centre.X) + (ChordCentrePoint.Y - centre.Y) * (ChordCentrePoint.Y - centre.Y));
            if (O_OCDistance < eps) //同心判断标准
            {
                xc = centre.X;
                yc = centre.Y;
                r = d[minindex[0]]; //半径
                return 0;
            }
            */

            //3:寻找第三个半径一致
            while (iteration < Maxiteration)
            {
                if (Math.Abs(d[minindex[0]] - d[minindex[1]]) < eps && Math.Abs(d[minindex[1]] - d[minindex[2]]) < eps) //三点等距找到
                {
                    //判断是否符合标准三
                    //A为0
                    Point AB = new Point(x[minindex[1]] - x[minindex[0]], y[minindex[1]] - y[minindex[0]]); //向量AB
                    Point AC = new Point(x[minindex[2]] - x[minindex[0]], y[minindex[2]] - y[minindex[0]]); //向量AC
                    //B为1
                    Point BA = new Point(x[minindex[0]] - x[minindex[1]], y[minindex[0]] - y[minindex[1]]); //向量BA
                    Point BC = new Point(x[minindex[2]] - x[minindex[1]], y[minindex[2]] - y[minindex[1]]); //向量BC
                    //C为2
                    Point CA = new Point(x[minindex[0]] - x[minindex[2]], y[minindex[0]] - y[minindex[2]]); //向量CA
                    Point CB = new Point(x[minindex[1]] - x[minindex[2]], y[minindex[1]] - y[minindex[2]]); //向量CB

                    double moAB = Math.Sqrt(AB.X * AB.X + AB.Y * AB.Y);
                    double moAC = Math.Sqrt(AC.X * AC.X + AC.Y * AC.Y);
                    double moBC = Math.Sqrt(BC.X * BC.X + BC.Y * BC.Y);

                    if ((AB.X * AC.X + AB.Y * AC.Y) > 0 && (BA.X * BC.X + BA.Y * BC.Y) > 0 && (CA.X * CB.X + CA.Y * CB.Y) > 0) //全锐角,符合三点标准
                    {
                        break;
                    }
                    else //三点等距，不满足三点准则
                    {
                        bool IsTwoPointCondition = false;
                        if (moAB >= moAC && moAB >= moBC) //使用AB两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, minindex[0], minindex[1], minindex[2], centre, out IsTwoPointCondition);
                            iteration++;
                        }
                        else if (moAC >= moAB && moAC >= moBC) //使用AC两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, minindex[0], minindex[2], minindex[1], centre, out IsTwoPointCondition);
                            iteration++;
                        }
                        else if (moBC >= moAB && moBC >= moAC) //使用BC两点继续寻找
                        {
                            centre = Phase3FindNextCentre(x, y, d, minindex[1], minindex[2], minindex[0], centre, out IsTwoPointCondition);
                            iteration++;
                        }
                        if (IsTwoPointCondition == true)
                        {
                            break;
                        }
                    }
                }
                else //未找到三点等距，用等距的两点来继续寻找
                {
                    bool isTwoPointCondition = false;
                    centre = Phase3FindNextCentre(x, y, d, minindex[0], minindex[1], centre, out isTwoPointCondition); //找到下一步的圆心
                    iteration++;
                    if (isTwoPointCondition == true) //该情况下centre没变，所以不需要重新计算d
                    {
                        break;
                    }
                }
                CalDistance(x, y, centre, d, out minindex[0], out minindex[1], out minindex[2]); //计算所有点到圆心的值
            }

            //找到对应的拟合圆
            xc = centre.X;
            yc = centre.Y;
            r = d[minindex[0]];

            if (iteration >= Maxiteration)
                return OverIteration;
            return 0;

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
        private static void CalDistance(double[] x, double[] y, Point centre, double[] d, out int minindex1, out int minindex2, out int minindex3)
        {
            int i;
            minindex1 = -1;
            minindex2 = -1;
            minindex3 = -1;

            for (i = 0; i < d.Length; i++)
            {
                d[i] = Math.Sqrt((x[i] - centre.X) * (x[i] - centre.X) + (y[i] - centre.Y) * (y[i] - centre.Y));
                //寻找最小三甲
                if (minindex1 < 0 || d[i] < d[minindex1])
                {
                    minindex1 = i;
                }
                else if (minindex2 < 0 || d[i] < d[minindex2])
                {
                    minindex2 = i;
                }
                else if (minindex3 < 0 || d[i] < d[minindex3])
                {
                    minindex3 = i;
                }
            }
        }
        private static Point Phase2FindNextCentre(double[] x, double[] y, double[] d, int minindex, Point centre)
        {
            //int i;
            //int maxindex = 0;
            //double []d= new double[x.Length];
            double[] t = new double[x.Length - 1]; //步长阵列
            //计算圆心移动方向
            Point S1vector = new Point(centre.X - x[minindex], centre.Y - y[minindex]); //计算最小距离点到圆心的方向向量
            double mo = Math.Sqrt(S1vector.X * S1vector.X + S1vector.Y * S1vector.Y); //模
            Point Tkvector = new Point(S1vector.X / mo, S1vector.Y / mo); //前进单位方向向量
            //单位方向向量


            double[] CosAlpha = CalCosAlpha(x, y, centre, Tkvector);
            double tk = FindTk(d, CosAlpha, 1, minindex, -1, -1);

            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }

        //没找到三点等距的情况下
        private static Point Phase3FindNextCentre(double[] x, double[] y, double[] d, int minindex1, int minindex2, Point centre, out bool IsTwoPointCondition)
        {
            //int i;
            Point ChordCenterPoint = new Point((x[minindex1] + x[minindex2]) / 2, (y[minindex1] + y[minindex2]) / 2); //两点的中点
            Point Tkvector = new Point(centre.X - ChordCenterPoint.X, centre.Y - ChordCenterPoint.Y);
            double tk = -1;
            double motk = Math.Sqrt(Tkvector.X * Tkvector.X + Tkvector.Y * Tkvector.Y); //求圆心到弦中点的模
            IsTwoPointCondition = false; //一般都需要三点准则

            if (motk < eps) //同心的情况下,计算两个方向
            {
                Point OAVector = new Point(x[minindex1] - centre.X, y[minindex1] - centre.Y);
                Point Tkvector1;
                Point Tkvector2;
                //两个方向寻找合适tk
                if (Math.Abs(OAVector.Y) < eps) //y =0
                {
                    Tkvector1 = new Point(0, 1); //已经是单位化
                    Tkvector2 = new Point(0, -1);
                }
                else
                {
                    Tkvector1 = new Point(1, -OAVector.X / OAVector.Y);
                    //单位化
                    motk = Math.Sqrt(Tkvector1.X * Tkvector1.X + Tkvector1.Y * Tkvector1.Y); //求圆心到弦中点的模
                    Tkvector1.X /= motk;
                    Tkvector1.Y /= motk;
                    Tkvector2 = new Point(-Tkvector1.X, -Tkvector1.Y); //反向向量
                }
                double[] CosAlpha1 = CalCosAlpha(x, y, centre, Tkvector1);
                double tk1 = FindTk(d, CosAlpha1, 2, minindex1, minindex2, -1);
                double[] CosAlpha2 = CalCosAlpha(x, y, centre, Tkvector2);
                double tk2 = FindTk(d, CosAlpha2, 2, minindex1, minindex2, -1);
                if (Math.Abs(tk1) < eps && Math.Abs(tk2) < eps) //两点判断内切圆最大
                {
                    IsTwoPointCondition = true;
                    return centre; //圆心已经找到，下一步圆心跟当前圆心一致
                }
                else if (tk1 > tk2) //tk1 方向为标准方向
                {
                    Tkvector = Tkvector1;
                    tk = tk1;
                }
                else //tk2 方向为标准方向
                {
                    Tkvector = Tkvector2;
                    tk = tk2;
                }
            }
            else //普通情况
            {
                //模单位化
                Tkvector.X /= motk;
                Tkvector.Y /= motk;
                double[] CosAlpha = CalCosAlpha(x, y, centre, Tkvector);
                tk = FindTk(d, CosAlpha, 2, minindex1, minindex2, -1);
            }

            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }
        private static double[] CalCosAlpha(double[] x, double[] y, Point centre, Point Tkvector)
        {
            int i;
            double[] CosAlpha = new double[x.Length]; //cosα阵列
            for (i = 0; i < CosAlpha.Length; i++)
            {
                Point ivector = new Point(x[i] - centre.X, y[i] - centre.Y); //i向量
                CosAlpha[i] = (ivector.X * Tkvector.X + ivector.Y * Tkvector.Y) / Math.Sqrt((ivector.X * ivector.X + ivector.Y * ivector.Y)); //计算cosα
            }
            return CosAlpha;
        }
        private static double FindTk(double[] d, double[] CosAlpha, int UsingParam, int minindex1, int minindex2, int exceptIndex)
        {
            int i;
            int j;
            double[] t = new double[d.Length - UsingParam];
            for (j = 0, i = 0; i < d.Length; i++)
            {
                if (i == minindex1 || i == minindex2 || i == exceptIndex) //跳过不处理
                    continue;
                t[j] = (d[minindex1] * d[minindex1] - d[i] * d[i]) / 2 / (d[minindex1] * CosAlpha[minindex1] - d[i] * CosAlpha[i]);
                j++;
            }

            double tk = -1;  //步进长度
            for (i = 0; i < t.Length; i++)
            {
                if (t[i] >= 0 && (t[i] < tk || tk < 0)) //寻找正的最小值
                {
                    tk = t[i];
                }
            }
            return tk;
        }
        //找到三点等距的情况下
        private static Point Phase3FindNextCentre(double[] x, double[] y, double[] d, int minindex1, int minindex2, int exceptIndex, Point centre, out bool IsTwoPointCondition)
        {
            //int i;
            //double[] t = new double[x.Length - 3];
            Point ChordCenterPoint = new Point((x[minindex1] + x[minindex2]) / 2, (y[minindex1] + y[minindex2]) / 2); //两点的中点
            Point Tkvector = new Point(ChordCenterPoint.X - centre.X, ChordCenterPoint.Y - centre.Y); //前进单位方向向量
            double motk = Math.Sqrt(Tkvector.X * Tkvector.X + Tkvector.Y * Tkvector.Y); //求圆心到弦中点的模
            double tk = -1;

            IsTwoPointCondition = false; //一般都需要三点准则

            if (motk < eps) //同心的情况下,计算两个方向
            {
                Point OAVector = new Point(x[minindex1] - centre.X, y[minindex1] - centre.Y);
                Point Tkvector1;
                Point Tkvector2;
                //两个方向寻找合适tk
                if (Math.Abs(OAVector.Y) < eps) //y =0
                {
                    Tkvector1 = new Point(0, 1); //已经是单位化
                    Tkvector2 = new Point(0, -1);
                }
                else
                {
                    Tkvector1 = new Point(1, -OAVector.X / OAVector.Y);
                    //单位化
                    motk = Math.Sqrt(Tkvector1.X * Tkvector1.X + Tkvector1.Y * Tkvector1.Y); //求圆心到弦中点的模
                    Tkvector1.X /= motk;
                    Tkvector1.Y /= motk;
                    Tkvector2 = new Point(-Tkvector1.X, -Tkvector1.Y); //反向向量
                }
                double[] CosAlpha1 = CalCosAlpha(x, y, centre, Tkvector1);
                double tk1 = FindTk(d, CosAlpha1, 3, minindex1, minindex2, exceptIndex);
                double[] CosAlpha2 = CalCosAlpha(x, y, centre, Tkvector2);
                double tk2 = FindTk(d, CosAlpha2, 3, minindex1, minindex2, exceptIndex);
                if (Math.Abs(tk1) < eps && Math.Abs(tk2) < eps) //两点判断内切圆最大
                {
                    IsTwoPointCondition = true;
                    return centre; //圆心已经找到，下一步圆心跟当前圆心一致
                }
                else if (tk1 > tk2) //tk1 方向为标准方向
                {
                    Tkvector = Tkvector1;
                    tk = tk1;
                }
                else //tk2 方向为标准方向
                {
                    Tkvector = Tkvector2;
                    tk = tk2;
                }
            }
            else
            {
                //模单位化
                Tkvector.X /= motk;
                Tkvector.Y /= motk;

                double[] CosAlpha = CalCosAlpha(x, y, centre, Tkvector);
                tk = FindTk(d, CosAlpha, 3, minindex1, minindex2, exceptIndex);
            }
            Point nextCentre = new Point(centre.X + Tkvector.X * tk, centre.Y + Tkvector.Y * tk);
            return nextCentre;
        }
    }
}
