using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarineSTMiningSystem
{
    /// <summary>
    /// 地球相关参数，基于WGS84
    /// </summary>
    public static class Earth
    {
        /// <summary>
        /// 赤道半径，单位：米
        /// </summary>
        public const int ER = 6378137; //赤道半径 长半径

        /// <summary>
        /// 极半径，单位：米
        /// </summary>
        public const int PR = 6356752; //极半径 短半径

        /// <summary>
        /// 平均半径，单位：米
        /// </summary>
        public const int MR = 6371009; //平均半径=(赤道半径+赤道半径+极半径)/3

        /// <summary>
        /// 扁率f
        /// </summary>
        public const double f = 1 / 298.256223563; //扁率f

        /// <summary>
        /// 赤道周长，单位：米
        /// </summary>
        public const int EC = 40075016;

        /// <summary>
        /// 子午线（经线）长度，单位：米
        /// </summary>
        public const int ML = 20003964; //粗略计算

        /// <summary>
        /// 1纬度长度，单位：米
        /// </summary>
        /// <returns></returns>
        public static double OneLatLen()
        {
            return ML / 180.0;
        }

        /// <summary>
        /// 1经度长度，单位：米
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public static double OneLogLen(double lat)
        {
            return EC*Math.Cos(lat*Math.PI/180) / 360.0;
        }

        ///计算地球两点距离,单位：米
        public static double TowPosDisM(double p1Log, double p1Lat, double p2Log, double p2Lat)
        {
            double logDis = p1Log - p2Log;//经度差距
            double latDis = p1Lat - p2Lat;//纬度差距
            double latMean = 0.5 * (p1Lat + p2Lat);//纬度平均值
            double logDisM = logDis * OneLogLen(latMean);
            double latDisM = latDis * OneLatLen();
            double disM = Math.Sqrt(Math.Pow(logDisM, 2) + Math.Pow(latDisM, 2));
            return disM;
        }
    }
}
