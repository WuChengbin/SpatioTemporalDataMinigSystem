using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    class Trajectory
    {
        public List<STrajectory> STrajectoryList = new List<STrajectory>();//每个事件包含的序列轨迹列表
        //List<string> TrajectoryIdList = new List<string>();//所包含的所有事件ID
        public int ID;//事件ID       
        public DateTime duration;//事件持续时间                                           

        public Trajectory()
        {
            //构造函数
        }
    }
}
