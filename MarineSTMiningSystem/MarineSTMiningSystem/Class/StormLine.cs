using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    public class StormLine
    {
        public StormLine()
        {
        }
        public StormLine(List<StormPoint> _pointList)
        {
            pointList = _pointList;
        }
        public List<StormPoint> pointList=new List<StormPoint>();
        public double minLog;
        public double minLat;
        public double maxLog;
        public double maxLat;
    }
}
