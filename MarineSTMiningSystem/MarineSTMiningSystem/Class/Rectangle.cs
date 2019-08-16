using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    class Rectangle
    {
        public double minX;
        public double maxX;
        public double minY;
        public double maxY;
        public double area;

        public double[] p1;
        public double[] p2;
        public double[] p3;
        public double[] p4;

        public double length;//矩形长度
        public double width;//矩形宽度

        public Rectangle(double _minX, double _maxX, double _minY, double _maxY, double _area)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
            area = _area;

            length = _maxX - _minX;
            width = _maxY - _minY;
            if(width>length)
            {//长宽交换
                double temp = length;
                length = width;
                width = temp;
            }

            p1 = new double[] { minX, minY };
            p2 = new double[] { maxX, minY };
            p3 = new double[] { minX, maxY };
            p4 = new double[] { maxX, maxY };
        }

        public Rectangle()
        {
            minX = double.MinValue;
            maxX = double.MaxValue;
            minY = double.MinValue;
            maxY = double.MaxValue;
            area = double.MaxValue;

            length = double.MaxValue;
            width = double.MinValue;

            p1 = new double[] { minX, minY };
            p2 = new double[] { maxX, minY };
            p3 = new double[] { minX, maxY };
            p4 = new double[] { maxX, maxY };
        }

        public Rectangle(double[] _p1, double[] _p2, double[] _p3, double[] _p4,double _length,double _width, double _area)
        {
            area = _area;

            p1 = _p1;
            p2 = _p2;
            p3 = _p3;
            p4 = _p4;

            length = _length;
            width = _width;

            double[] _xa = { _p1[0], _p2[0], _p3[0], p4[0] };
            double[] _ya = { _p1[1], _p2[1], _p3[1], p4[1] };

            minX = _xa.Min();
            maxX = _xa.Max();
            minY = _ya.Min();
            maxY = _ya.Max();

        }
    }
}
