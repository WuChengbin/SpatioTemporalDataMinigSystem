using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithm
{
    class DataRaster
    {
        public int drID; //数据点ID
        public int clusterId; //所属聚类ID
        public bool isKey; //是否为核心对象
        public bool visited; //是否已访问 
        public double attribute;
        public int x;
        public int y;
        public List<int> arrivalgrids = new List<int>(); //领域数据点id列表
        public bool IsKey()
        {
            return this.isKey;
        }
        //设置核心对象标志
        public void SetKey(bool isKey)
        {
            this.isKey = isKey;
        }

        //获取DpId方法
        public int GetDrId()
        {
            return this.drID;
        }

        //设置DpId方法
        public void SetDrId(int drID)
        {
            this.drID = drID;
        }

        //GetIsVisited方法
        public bool isVisited()
        {
            return this.visited;
        }


        //SetIsVisited方法
        public void SetVisited(bool visited)
        {
            this.visited = visited;
        }

        //GetClusterId方法
        public int GetClusterId()
        {
            return this.clusterId;
        }

        //GetClusterId方法
        public void SetClusterId(int clusterId)
        {
            this.clusterId = clusterId;
        }

        //GetArrivalPoints方法
        public List<int> GetArrivalgrids()
        {
            return arrivalgrids;
        }
    }
}
