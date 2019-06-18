using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    /// <summary>
    /// 轨迹类,构成事件的sub轨迹
    /// </summary>
    class STrajectory
    {
        public double StartLog;//开始经度
        public double StartLat;//开始纬度
        public double EndLog;//结束经度
        public double EndLat;//结束纬度
        public DateTime StartTime;//开始时间
        public DateTime EndTime;//结束时间
        public int ID;//所属事件id
        //public int ClusterID;//所属聚类id
        public string Stype;//起始节点的类型
        public string Sstateid;//
        public string Etype;//终止节点的类型
        public string Estateid;//

        public STrajectory()
        {
            //构造函数
        }
    }
}
