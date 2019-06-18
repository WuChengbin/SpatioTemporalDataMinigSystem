using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo4j.Driver.V1;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Data;
using Panuon.UI;
using System.Net.NetworkInformation;

//此类调用了PanuonUI WPF对话框
//WinForm或原生WPF注释PUMessageBox即可
namespace STDMS.DataOP
{
    class Neo4j64
    {
        public static IDriver neoDirver;
        public static bool isConnected = false;
        public static bool isAdminRole = false;

        /// <summary>
        /// 驱动并连接数据库
        /// 在创建节点和关系前调用
        /// </summary>
        /// <param name="url">数据库连接</param>
        /// <param name="user">数据库用户名</param>
        /// <param name="pwd">数据库密码</param>
        public static void Neo4jConnect(string ip,string portNum, string user, string pwd)
        {
            bool isIP = true;
            IPAddress ipAddress=null;
            try
            {
                ipAddress = IPAddress.Parse(ip);
                
            }
            catch
            {
                isIP = false;
                Ping p = new Ping();
                PingOptions options = new PingOptions();
                options.DontFragment = true;
                string data = "Test Data";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                PingReply reply = p.Send(ip, 1000, buffer, options);
                if (reply.Status != IPStatus.Success)
                {
                    throw new Exception("无法与：" + ip + "建立连接，请检查远程主机是否可用");
                }
            }
            if (isIP)
            {
                IPEndPoint point = new IPEndPoint(ipAddress, int.Parse(portNum));
                using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    sock.Connect(point);
                    sock.Close();
                }
            }           
            string url = "bolt://" + ip + ":" + portNum;
            neoDirver = GraphDatabase.Driver(url, AuthTokens.Basic(user, pwd), new Config { ConnectionTimeout = TimeSpan.FromSeconds(5), ConnectionAcquisitionTimeout = TimeSpan.FromSeconds(10) });
            var session = neoDirver.Session(AccessMode.Read);
            session.ReadTransaction(rx => rx.Run("return 0"));//测试数据库是否连接成功
            isConnected = true;
            isAdminRole = VerifyjAdminRtole(user);
        }
        public static void Neo4jClose()
        {
            if (neoDirver != null)
            {
                neoDirver.Close();
                isConnected = false;
            }          
        }

        public static List<Dictionary<string,string>> QueryNodeDataTable(string CQL)
        {
            DataTable dataTable = new DataTable("Dataset");
            if (isConnected)
            {
                List<Dictionary<string, string>> dicList = new List<Dictionary<string, string>>();
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results.Count != 0)
                    {
                        string key = results[0].Keys[0];
                        foreach (var row in results)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            var cols = row.Values;
                            foreach (var col in cols)
                            {
                                if (col.Key == key)
                                {
                                    var node = col.Value as INode;
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
                                            string bbox = "[";
                                            
                                            bbox += ObjectList[0].ToString() + " "+ObjectList[1].ToString() +","+ ObjectList[2].ToString()+" "+ObjectList[3].ToString()+"]";                                                                                    
                                            dic.Add(property.Key, bbox);
                                        }
                                        else
                                        {
                                            dic.Add(property.Key, property.Value.ToString());
                                        }
                                    }
                                }
                            }
                            dicList.Add(dic);
                        }
                        return dicList;
                    }
                    else
                    {
                        return null;
                    }

                }
            }
               
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static List<List<string>>QueryNonNodeDataTable(string CQL)
        {
            List<List<string>> ResList = new List<List<string>>();
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            List<string> temp = new List<string>();
                            for(int j=0;j < results[i].Values.ToArray().Length; j++)
                            {
                                if (results[i].Values.ToArray()[j].Value != null)
                                {
                                    Type t = results[i].Values.ToArray()[j].Value.GetType();
                                    if (results[i].Values.ToArray()[j].Value.GetType().Name.ToUpper().Contains("STRING"))
                                    {
                                        temp.Add(results[i].Values.ToArray()[j].Value.ToString());
                                    }
                                    else if (results[i].Values.ToArray()[j].Value.GetType().Name.ToUpper().Contains("LIST"))
                                    {
                                        string tempList = string.Empty;
                                        for (int kk = 0; kk < ((List<object>)results[i].Values.ToArray()[j].Value).Count; kk++)
                                        {
                                            tempList += ((List<object>)results[i].Values.ToArray()[j].Value)[kk] + " ";
                                        }
                                        temp.Add(tempList);
                                    }
                                    else
                                    {
                                        temp.Add(results[i].Values.ToArray()[j].Value.ToString());
                                    }
                                }                                                             
                            }
                            ResList.Add(temp);
                        }
                    }
                    else
                        return null;
                }
                return ResList;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        /// <summary>
        /// 查询数据库中所有节点标签
        /// </summary>
        /// <returns></returns>
        public static List<String> QueryFeatures()
        {
            List<String> ResList = new List<String>();
            if (isConnected)
            {
                String CQL = "CALL db.labels()";
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            ResList.Add(results[i].Values["label"].ToString());
                        }
                    }
                    else
                        return null;
                    
                }
                return ResList;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
            
        }
        /// <summary>
        /// 查询指定节点的所有字段
        /// </summary>
        /// <param name="NodeType"></param>
        /// <param name="Simple">快速查询，如果节点字段不一致，可能会导致字段缺失</param>
        /// <returns>字段列表</returns>
        public static List<String> QueryFeatureFields(string NodeType,bool Simple=true)
        {
            if(!Simple)
            {
                string CQL = "MATCH(N: " + NodeType + ") WITH keys(N) AS KEYS ORDER BY length(keys(N)) DESC LIMIT 1 UNWIND KEYS AS RES RETURN RES";
                List<String> ResList = new List<String>();
                if (isConnected)
                {
                    using (var session = neoDirver.Session(AccessMode.Read))
                    {
                        var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                        if (results != null)
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                ResList.Add(results[i].Values["RES"].ToString());
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return ResList;
                }
                else
                {
                    throw new Exception("未连接数据库!");
                }
            }
            else
            {
                string CQL = "MATCH(N: " + NodeType + ") WITH keys(N) AS KEYS  LIMIT 1 UNWIND KEYS AS RES RETURN RES";
                List<String> ResList = new List<String>();
                if (isConnected)
                {
                    using (var session = neoDirver.Session(AccessMode.Read))
                    {
                        var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                        if (results != null)
                        {
                            for (int i = 0; i < results.Count; i++)
                            {
                                ResList.Add(results[i].Values["RES"].ToString());
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return ResList;
                }
                else
                {
                    throw new Exception("未连接数据库!");
                }
            }
            
        }

        public static List<String>QueryDistinctVale(string NodeType,string Key)
        {
            string CQL = "MATCH(N: " + NodeType + ") RETURN DISTINCT(N."+Key+") AS VALUE";
            List<String> ResList = new List<String>();
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            try
                            {
                                if (Key == "geometry")
                                {

                                    ResList.Add(Convertor.ConvertWKBToWKT(results[i].Values["VALUE"] as byte[]));
                                }
                                else
                                {
                                    ResList.Add(results[i].Values["VALUE"].ToString());
                                }

                            }
                            catch
                            {
                                ResList.Add("null");
                            }
                        }
                    }
                    else
                        return null;
                    
                }
                return ResList;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static List<String> QueryLayers()
        {
            string CQL = "MATCH (N:ReferenceNode)-[]->(M) return M.layer as RES";
            List<String> ResList = new List<String>();
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            ResList.Add(results[i].Values["RES"].ToString());
                        }
                    }
                    else
                    {
                        return null;
                    }
                    
                }
                return ResList;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static String QueryEventSeqLabel(string EventNode)
        {
            string strTemp = string.Empty;
            if (EventNode.Contains("Event") || EventNode.Contains("EVENT")) { strTemp = EventNode.Substring(0, EventNode.Length - 5); }
            else strTemp = EventNode;
            string CQL = "MATCH(NODE:" + EventNode + ")-[:Belong]->(SQNODE) with labels(SQNODE) as RES UNWIND RES AS KEYS return distinct(KEYS)";
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].Values["KEYS"].ToString().Contains(strTemp))
                            {
                                return results[i].Values["KEYS"].ToString();
                            }
                        }
                        return null;
                    }
                    else
                        return null;
                }
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static String QueryEventStateLabel(string EventNode)
        {
            string strTemp = string.Empty;
            if (EventNode.Contains("Event") || EventNode.Contains("EVENT")) { strTemp = EventNode.Substring(0, EventNode.Length - 5); }
            else strTemp = EventNode;
            string CQL = "MATCH(NODE:" + EventNode + ")-[:Belong]->()-[:Belong]->(STNODE) with labels(STNODE) as RES UNWIND RES AS KEYS return distinct(KEYS)";
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            if (results[i].Values["KEYS"].ToString().Contains(strTemp))
                            {
                                return results[i].Values["KEYS"].ToString();
                            }
                        }
                        return null;
                    }
                    else
                        return null;
                }
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static String QueryAreaWKT(string NAME)
        {
            string CQL = "match(n) where n.NAME=~'.*" + NAME + ".*' return n.geometry order by n.AREA desc";
            string CityGeometry = "";
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        foreach (var row in results)//对链表第一行数据进行操作
                        {
                            byte[] wkb = row.Values.ToArray()[0].Value as byte[];
                            CityGeometry = Convertor.ConvertWKBToWKT(wkb);
                            break;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return CityGeometry;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
        }

        public static List<String> QueryIndexes()
        {
            List<String> ResList = new List<String>();
            if (isConnected)
            {
                String CQL = "CALL db.indexes()";
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
                    if (results != null)
                    {
                        for (int i = 0; i < results.Count; i++)
                        {
                            ResList.Add(results[i].Values["description"].ToString().Split(':')[1]);
                        }
                    }
                    else
                        return null;

                }
                return ResList;
            }
            else
            {
                throw new Exception("未连接数据库!");
            }

        }

        public static List<Dictionary<string,string>> ExcuteCQL(string CQL)
        {
            List<Dictionary<string, string>> tempList = new List<Dictionary<string, string>>();
            if (isConnected)
            {

                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var Result=session.WriteTransaction(rx => rx.Run(CQL)).ToList();
                    for(int i = 0; i < Result.Count; i++)
                    {
                        Dictionary<string, string> tempDic = new Dictionary<string, string>();
                        for(int j = 0; j < Result[i].Keys.Count; j++)
                        {
                            tempDic.Add(Result[i].Keys[j], Result[i].Values[Result[i].Keys[j].ToString()].ToString());
                        }
                        tempList.Add(tempDic);
                    }
                    
                }
            }
            else
            {
                throw new Exception("未连接数据库!");
            }
            return tempList;
        }

        public static bool HasRelation(string Node1,string Node2)
        {
            string CQL = "match(n:"+Node1+") -[r]->(m:"+Node2+ ") return r as RES limit 1";
            if (isConnected)
            {
                using (var session = neoDirver.Session(AccessMode.Read))
                {
                    var results = session.ReadTransaction(rx => rx.Run(CQL)).ToArray();
                    if (results!=null)
                    {
                        if (results.Length > 0)
                            return true;
                        else
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        //public static bool VerifyUserInfo(string user,string pwd)
        //{
        //    string CQL = "MATCH(n:user) where n.name=\"" + user + "\" return n.pwd as PWD";
        //    if (isConnected)
        //    {             
        //        using (var session = neoDirver.Session(AccessMode.Read))
        //        {
        //            var results = session.ReadTransaction(rx => rx.Run(CQL)).ToList();
        //            if(results.Count>0)
        //            {
        //                string userPwd = results[0].Values["PWD"].ToString();
        //                if (userPwd.ToLower() == Convertor.MD5Encrypt32(pwd).ToLower())
        //                {
        //                    return true;
        //                }
        //                else
        //                {
        //                    return false;
        //                }
        //            }
        //            else
        //            {
        //                return false;
        //            }                    
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("未连接数据库!");
        //    }
        //}

        private static bool VerifyjAdminRtole(string user)
        {
            List <List<string>>res = Neo4j64.QueryNonNodeDataTable("call dbms.security.listRolesForUser(\'" + user + "\')");
            if (res.Count > 0)
            {
                if (res[0].Contains("admin"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }  
}
