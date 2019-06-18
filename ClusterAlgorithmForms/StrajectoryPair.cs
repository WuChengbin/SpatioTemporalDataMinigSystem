using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    class StrajectoryPair
    {
        public string SNowid;//均为状态id
        public string ENowid;
        public string SMatchid;
        public string EMatchid;
        public DateTime SNowtime;
        public DateTime ENowtime;
        public DateTime SMatchtime;
        public DateTime EMatchtime;
        public bool isvisited=false;//默认未被访问

        public StrajectoryPair()
        {
            //构造函数
        }
    }
}
