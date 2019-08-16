using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    class Circle
    {
        public double x;//圆心x坐标
        public double y;//圆心y坐标
        public double r;//圆半径

        public Circle(double _x,double _y,double _r)
        {
            x = _x;
            y = _y;
            r = _r;
        }

        public Circle()
        {
            x = 0.0;
            y = 0.0;
            r = 0.0;
        }
    }
}
