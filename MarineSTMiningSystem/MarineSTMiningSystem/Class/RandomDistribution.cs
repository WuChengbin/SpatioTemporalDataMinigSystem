using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    //正态分布
    class RandomDistribution
    {


        static Random ran = new Random();
        private double AverageRandom(double min, double max)//产生(min,max)之间均匀分布的随机数
        {
            int MinInt = (int)(min * 10000);
            int MaxInt = (int)(max * 10000);
            int resultInteger = ran.Next(MinInt, MaxInt);
            return resultInteger / 10000.0;
        }
        private double Normal(double x, double miu, double sigma) //正态分布概率密度函数
        {
            //return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
            return 1.0 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (x - miu) * (x - miu) / (2 * sigma * sigma));
        }
        public double GetRandomValue(double miu, double sigma, double min, double max)//产生正态分布随机数
        {
            double x;
            double dScope;
            double y;
            do
            {
                x = AverageRandom(min, max);//最大最小间的均匀随机数
                y = Normal(x, miu, sigma);//x的密度
                dScope = AverageRandom(0, Normal(miu, miu, sigma));//0到平均值密度的均匀随机数
            } while (dScope > y);
            return x;
        }
    }
}
