using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClusterAlgorithmForms
{
    /// <summary>
    /// 时空簇类,导出簇间关系与过程信息
    /// </summary>
    class ST_Cluster
    {
        //属性
        public int pid;//时空簇过程id

        public string wkt = "";
        public string startTime;//开始时间
        public string endTime;//结束时间
        public double sumAera;//总面积
        public double maxAera;//最大面积
        public double sumpower;//总强度
        public double maxpower;//最大强度
        public double minpower;//最小强度
        public string valuetype;//簇的属性，即是高值簇还是低值簇

        public List<Feature> featureList = new List<Feature>();//要素列表
        public List<ClusterPolygon> polygonList = new List<ClusterPolygon>();//簇多边形列表
        public string maxStateId;//最大状态id
        public List<string> stateIdList = new List<string>();//记录所有状态id

        public ST_Cluster()
        {//构造函数

        }
    }
}
