using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class MarineHeatwavePoint
    {
        public MarineHeatwavePoint()
        {

        }
        public MarineHeatwavePoint(double _log, double _lat)
        {
            log = _log; lat = _lat;
        }
        public double log;//经度
        public double lat;//纬度
    }
}
