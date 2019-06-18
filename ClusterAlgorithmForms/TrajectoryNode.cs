using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    /// <summary>
    /// 轨迹节点类
    /// </summary>
    class TrajectoryNode
    {
        //属性
        public double Log;//经度
        public double Lat;//维度
        public DateTime time;//所在时刻
        public int ID;//所属事件id
        public string stateid;//状态id
        public bool isvisited;//是否可被忽略
        public string type;//节点类型，开始0，结束1，分裂2，合并3，合并分裂4，发展5

        public TrajectoryNode()
        {
            //构造函数
        }

    }
}
