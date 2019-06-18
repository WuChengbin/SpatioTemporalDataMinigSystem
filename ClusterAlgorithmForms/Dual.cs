using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forms
{
    class Dual
    {
        public int clusID; //空间簇编号，即只要不连通就独立成一簇
        public int k;//所属属性簇
        public bool visited; //是否已访问
        public int x;//记录栅格x值
        public int y;//记录栅格y值
        public void SetclusID(int clusID)
        {
            this.clusID = clusID;
        }

        public bool isVisited()
        {
            return this.visited;
        }

        public void SetVisited(bool visited)
        {
            this.visited = visited;
        }
    }
}
