using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using System.Windows.Forms;

namespace ClusterAlgorithmForms
{
    class Neo4j64
    {
        public static IDriver neoDirver;

        /// <summary>
        /// 驱动并连接数据库
        /// 在创建节点和关系前调用
        /// </summary>
        /// <param name="url">数据库连接</param>
        /// <param name="user">数据库用户名</param>
        /// <param name="pwd">数据库密码</param>
        public static bool Neo4jDrive(string url, string user, string pwd)
        {
            try
            {
                neoDirver = GraphDatabase.Driver(url, AuthTokens.Basic(user, pwd), new Config { ConnectionTimeout = TimeSpan.FromSeconds(5) });
                var session = neoDirver.Session(AccessMode.Read);
                session.ReadTransaction(rx => rx.Run("return 0"));//测试数据库是否连接成功
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show("数据库连接失败：" + err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }


        /// <summary>
        /// 查询数据库中所有的暴雨事件ID值，返回ID列表
        /// </summary>
        /// <param NodeType="RainStormEventNode"></param>
        /// <returns></returns>
        public static List<String> queryNodeIDList(string Neo4jAddress, string Neo4jPortNum, string Neo4jUserName, string Neo4jKeys, string NodeType)
        {
            string cql = "Match(n:" + NodeType + ") return n.ID"; //RainStorm
            //string cql= "Match(n:" + NodeType + ") return n.PID" //SST & NPP
            List<String> EventIDList = new List<String>();
            if (Neo4jDrive("bolt://" + Neo4jAddress + ":" + Neo4jPortNum, Neo4jUserName, Neo4jKeys))
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var result = session.ReadTransaction(rx => rx.Run(cql)).ToList();
                    foreach (var row in result)
                    {
                        EventIDList.Add(row.Values.ToArray()[0].Value.ToString());
                    }
                }
                neoDirver.Close();
            }
            return EventIDList;
        }


        /// <summary>
        /// RainStormStateNode查询暴雨事件信息，返回暴雨节点每个属性
        /// </summary>
        /// <param name="EventID"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> queryEventState(string Neo4jAddress, string Neo4jPortNum, string Neo4jUserName, string Neo4jKeys, string QueryClass, string EventID)
        {
            string cql = "MATCH(N:RainStormStateNode) where N.StormID=" + EventID + " RETURN N order by ToInt(N.LongTime) asc";
            if (QueryClass == "SST")
                cql = "MATCH(N:SstClusterNode) where N.GRIDCODE=" + EventID + " RETURN N order by ToInt(N.LongTime) asc";
            else if (QueryClass == "NPP")
                cql = "MATCH(N:NppClusterNode) where N.GRIDCODE=" + EventID + " RETURN N order by ToInt(N.LongTime) asc";
            List<Dictionary<string, string>> dicList = new List<Dictionary<string, string>>();
            if (Neo4jDrive("bolt://" + Neo4jAddress + ":" + Neo4jPortNum, Neo4jUserName, Neo4jKeys))
            {
                List<String> EventIDList = new List<String>();
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var result = session.ReadTransaction(rx => rx.Run(cql)).ToList();
                    foreach (var row in result)
                    {
                        var cols = row.Values;

                        foreach (var col in cols)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            if (col.Key == "N")
                            {
                                var node = col.Value as INode;

                                long NodeID = node.Id;
                                string NodeLables = string.Join(",", node.Labels.ToArray());
                                foreach (var property in node.Properties)
                                {
                                    if (property.Key.ToString() == "geometry")
                                    {
                                        byte[] wkb = property.Value as byte[];
                                        string wkt = Convertor.ConvertWKBToWKT(wkb);
                                        dic.Add(property.Key, wkt);
                                    }
                                    else if (property.Key.ToString() == "bbox")
                                    {
                                        List<object> ObjectList = (List<object>)property.Value;
                                        string bbox = null;
                                        for (int i = 0; i < ObjectList.Count - 1; i++)
                                        {
                                            bbox += ObjectList[i].ToString() + ",";
                                        }
                                        bbox += ObjectList[ObjectList.Count - 1].ToString();
                                        dic.Add(property.Key, bbox);
                                    }
                                    else
                                        dic.Add(property.Key, property.Value.ToString());
                                }
                                dicList.Add(dic);
                            }
                        }
                    }
                }
                neoDirver.Close();
            }

            return dicList;
        }

        /// <summary>
        /// 通过ID查询暴雨事件信息
        /// </summary>
        /// <param name="Neo4jAddress">数据库地址</param>
        /// <param name="Neo4jPortNum">端口号</param>
        /// <param name="Neo4jUserName">用户名</param>
        /// <param name="Neo4jKeys">密码</param>
        /// <param name="EventID">事件ID</param>
        /// <returns></returns>
        public static Dictionary<string, string> queryEventInfo(string Neo4jAddress, string Neo4jPortNum, string Neo4jUserName, string Neo4jKeys, string EventID)
        {
            string cql = "MATCH(N:EventNode) where N.StormID='" + EventID + "' RETURN N";
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (Neo4jDrive("bolt://" + Neo4jAddress + ":" + Neo4jPortNum, Neo4jUserName, Neo4jKeys))
            {
                List<String> EventIDList = new List<String>();
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var result = session.ReadTransaction(rx => rx.Run(cql)).ToList();
                    foreach (var row in result)
                    {
                        var cols = row.Values;

                        foreach (var col in cols)
                        {
                            if (col.Key == "N")
                            {
                                var node = col.Value as INode;

                                long NodeID = node.Id;
                                string NodeLables = string.Join(",", node.Labels.ToArray());
                                foreach (var property in node.Properties)
                                {
                                    dic.Add(property.Key, property.Value.ToString());
                                }
                            }
                        }
                    }
                }
                neoDirver.Close();
            }

            return dic;
        }

        public static List<int> queryNonDevelopNode(string Neo4jAddress, string Neo4jPortNum, string Neo4jUserName, string Neo4jKeys,string StateID, List<TrajectoryNode> NodeList, string NodeType = "RainStormStateNode")//NodeType参数如果不填，则默认为暴雨节点
        {
            Dictionary<string, int> NodeMap = new Dictionary<string, int>();
            for(int i=0;i<NodeList.Count;i++)
            {
                NodeMap.Add(NodeList[i].stateid, i);
            }
            List<string> ResultIDList = new List<string>();
            List<string> TempIDList = new List<string>();
            //ResultIDList.Add(StateID);//起始节点也需要存
            //string CQL = "match(n:"+NodeType+"{StateID:'"+StateID+"'})-[:SRelationship*..30]->(m:"+ NodeType + "{StormID:"+StateID.Split('_')[0]+ "}) where m.Type<>5 return distinct(m.StateID)"; //RainStorm
            string CQL = "match shortestPath((n:" + NodeType + "{StateID:'" + StateID + "'})-[:SRelationship*]->(m:" + NodeType + "{StormID:" + StateID.Split('_')[0] + "})) where m.Type<>5 and m.StateID<>'"+ StateID + "'return m.StateID order by m.LongTime asc";
            if (Neo4jDrive("bolt://" + Neo4jAddress + ":" + Neo4jPortNum, Neo4jUserName, Neo4jKeys))
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var result = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    foreach (var row in result)
                    {
                        ResultIDList.Add(row.Values.ToArray()[0].Value.ToString());
                    }
                }
                neoDirver.Close();
            }
            //for(int i=0;i<TempIDList.Count;i++)
            //{
            //    if(isForwardConnected(Neo4jAddress, Neo4jPortNum,Neo4jUserName,Neo4jKeys,StateID, TempIDList[i]))
            //    {
            //        ResultIDList.Add(TempIDList[i]);
            //    }
            //}

            List<int> IndexList = new List<int>();
            for (int i=0;i<ResultIDList.Count;i++)
            {
                IndexList.Add(NodeMap[ResultIDList[i]]);
            }           
            return IndexList;
        }

        public static bool isForwardConnected(string Neo4jAddress, string Neo4jPortNum, string Neo4jUserName, string Neo4jKeys, string StateID1, string StateID2, string NodeType = "RainStormStateNode")
        {
            //string CQL = "match(n:" + NodeType + "{StateID:'" + StateID1 + "'})-[r:SRelationship*..30]->(m:"+NodeType+"{StateID:'"+StateID2+"'}) return r"; //RainStorm
            string CQL= "match p=shortestPath((n:" + NodeType + "{StateID:'" + StateID1 + "'})-[:SRelationship*]->(m:" + NodeType + "{StateID:'" + StateID2 + "'})) return p ";
            if (Neo4jDrive("bolt://" + Neo4jAddress + ":" + Neo4jPortNum, Neo4jUserName, Neo4jKeys))
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var result = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    neoDirver.Close();
                    if (result.Count == 0)
                        return false;
                    else
                        return true;
                }
                
            }
            return false;
        }

        public static bool updateTrajectoryID(string EventID,string ClassID,string NodeType)
        {
            string cql = "Match(N:" + NodeType + ") where N.PID='" + EventID + "' set N.TrajectoryID=" + ClassID;
            try
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    session.Run(cql).ToString();
                    //session.ReadTransaction(rx => rx.Run(CQL));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
