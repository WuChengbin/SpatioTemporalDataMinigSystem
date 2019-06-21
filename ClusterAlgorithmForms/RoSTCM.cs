using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithm
{
    class RoSTCM
    {
        public int rsID; //栅格像元ID
        public int rsclusterId; //所属聚类ID
        public bool isKeyrs; //是否为核心对象
        public bool Visited; //是否已访问 
        public double Attribute;//专题属性，即栅格值
        public int t;//时间属性
        public int x;
        public int y;//空间属性
        public int a;//转换属性（高值区为1，低值区为-1，其余为0）
        public List<int> neighborgrids = new List<int>(); //时空领域数据点id列表

        public bool IsKey()
        {
            return this.isKeyrs;
        }

        //设置核心对象标志
        public void SetKey(bool isKeyrs)
        {
            this.isKeyrs = isKeyrs;
        }

        //获取DpId方法
        public int GetrsID()
        {
            return this.rsID;
        }

        //设置DpId方法
        public void SetrsId(int rsID)
        {
            this.rsID = rsID;
        }

        //GetIsVisited方法
        public bool isVisited()
        {
            return this.Visited;
        }

        //SetIsVisited方法
        public void SetVisited(bool Visited)
        {
            this.Visited = Visited;
        }

        //GetClusterId方法
        public long GetrsClusterId()
        {
            return this.rsclusterId;
        }

        //GetClusterId方法
        public void SetrsClusterId(int rsclusterId)
        {
            this.rsclusterId = rsclusterId;
        }

        //GetArrivalPoints方法
        public List<int> Getneighborgrids()
        {
            return neighborgrids;
        }
    }
}
