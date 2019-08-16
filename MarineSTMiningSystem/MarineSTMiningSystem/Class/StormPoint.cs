using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    //暴雨多边形结点
    public class StormPoint
    {
        public StormPoint()
        {

        }
        public StormPoint(double _log,double _lat)
        {
            log = _log;lat = _lat;
        }
        public int row;//x坐标
        public int col;//y坐标
        public double log;//经度
        public double lat;//纬度
    }
}
