using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class MarineHeatwaveLine
    {
        public MarineHeatwaveLine()
        {
        }
        public MarineHeatwaveLine(List<MarineHeatwavePoint> _pointList)
        {
            pointList = _pointList;
        }
        public List<MarineHeatwavePoint> pointList = new List<MarineHeatwavePoint>();
        public double minLog;
        public double minLat;
        public double maxLog;
        public double maxLat;
    }
}
