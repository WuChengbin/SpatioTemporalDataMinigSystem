using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem.Class
{
    class GeneralFun
    {
        public static double GetMeanValue(double []pBuffer,double FillValue=-9999)
        {
            double dSum = 0;
            long lCount = 0;
            for(int i=0;i<pBuffer.Length;i++)
            {
                if(pBuffer[i]!=FillValue)
                {
                    dSum += pBuffer[i];
                    lCount++;
                }                
            }
            if(lCount==0)
            {
                return 0.0;
            }
            else
            {
                return dSum / lCount;
            }
            
        }

        public static double GetStdValue(double []pBuffer, double FillValue = -9999)
        {
            double dAvgValue = GetMeanValue(pBuffer,FillValue);
            double dSum = 0;
            long lCount = 0;
            for(int i=0;i<pBuffer.Length;i++)
            {
                if(pBuffer[i]!=FillValue)
                {
                    dSum += (pBuffer[i] - dAvgValue) * (pBuffer[i] - dAvgValue);
                    lCount++;
                }              
            }
            if (lCount == 0)
            {
                return 0.0;
            }
            return Math.Sqrt(dSum / lCount);
        }
    }
}
