using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text.RegularExpressions;
using Esri.ArcGISRuntime.Geometry;
using NetTopologySuite.IO;
using System.Security.Cryptography;
using System.Text;

namespace STDMS.DataOP
{
    class Convertor
    {

        public static DataTable MapNode2DataTable(List<Dictionary<String,String>>Nodes,Dictionary<String,String> FieldsMap, List<String>Keys)
        {
            List<String> tempKeys = new List<string>(Keys);
            
            //转换为DataTable
            DataTable dataTable = new DataTable("Dataset");

            #region 字段排序、中文映射
            for (int i = 0; i < FieldsMap.Count; i++)
            {
                if (Keys.Contains(FieldsMap.Keys.ToList()[i]))//可以通过XML文件映射
                {
                    //按照配置文件顺序添加字段
                    #region 获取数据类型、映射中文
                    string DataType = string.Empty;
                    if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT" ||
                        FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT16" ||
                        FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT32")
                    {
                        DataType = "System.Int32";
                    }
                    else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DOUBLE" ||
                        FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "FLOAT")
                    {
                        DataType = "System.Double";
                    }
                    else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "STRING")
                    {
                        DataType = "System.String";
                    }
                    //else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DATETIME" ||
                    //    FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DATE")
                    //{
                    //    DataType = "System.DateTime";
                    //}
                    else
                    {
                        DataType = "System.String";
                    }
                    #endregion
                    dataTable.Columns.Add(FieldsMap.Values.ToList()[i].Split(',')[1], Type.GetType(DataType));
                    tempKeys.Remove(FieldsMap.Keys.ToList()[i]);
                }
            }
            for (int i = 0; i < tempKeys.Count; i++)
            {
                if(tempKeys[i]!="geometry")dataTable.Columns.Add(tempKeys[i]);
            }
            for (int i = 0; i < Nodes.Count; i++)
            {
                DataRow newRow = dataTable.NewRow();
                for (int j = 0; j < Nodes[i].Count; j++)
                {
                    #region 映射数据
                    if (FieldsMap.ContainsKey(Nodes[i].Keys.ToArray()[j]))
                    {
                        try
                        {
                            newRow[FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                        }
                        catch
                        {
                            string DataType = string.Empty;
                            DataType = FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0];
                            if (FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "INT" ||
                                    FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "INT16" ||
                                    FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "INT32")
                            {
                                DataType = "System.Int32";
                            }
                            else if (FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "DOUBLE" ||
                                FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "FLOAT")
                            {
                                DataType = "System.Double";
                            }
                            else if (FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "STRING")
                            {
                                DataType = "System.String";
                            }
                            //else if (FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "DATETIME" ||
                            //    FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[0].ToUpper() == "DATE")
                            //{
                            //    DataType = "System.DateTime";
                            //}
                            else
                            {
                                DataType = "System.String";
                            }
                            if (Nodes[i].Keys.ToArray()[j] != "geometry")
                            {
                                dataTable.Columns.Add(FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1], Type.GetType(DataType));
                                newRow[FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                            }
                            //dataTable.Columns.Add(FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1], Type.GetType(DataType));
                            //newRow[FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];

                        }

                    }
                    else if(Nodes[i].Keys.ToArray()[j]!="geometry")//不映射geometry在表中
                    {
                        try
                        {
                            newRow[Nodes[i].Keys.ToArray()[j]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                        }
                        catch
                        {
                            if (FieldsMap.Keys.Contains(Nodes[i].Keys.ToArray()[j]))
                            {
                                string DataType = string.Empty;
                                if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT" ||
                                    FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT16" ||
                                    FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "INT32")
                                {
                                    DataType = "System.Int32";
                                }
                                else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DOUBLE" ||
                                    FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "FLOAT")
                                {
                                    DataType = "System.Double";
                                }
                                else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "STRING")
                                {
                                    DataType = "System.String";
                                }
                                //else if (FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DATETIME" ||
                                //    FieldsMap.Values.ToList()[i].Split(',')[0].ToUpper() == "DATE")
                                //{
                                //    DataType = "System.DateTime";
                                //}
                                else
                                {
                                    DataType = "System.String";
                                }
                                if (Nodes[i].Keys.ToArray()[j] != "geometry")
                                {
                                    dataTable.Columns.Add(FieldsMap[Nodes[i][Nodes[i].Keys.ToArray()[j]]].Split(',')[1], Type.GetType(DataType));
                                    newRow[FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                                }
                                //dataTable.Columns.Add(FieldsMap[Nodes[i][Nodes[i].Keys.ToArray()[j]]].Split(',')[1], Type.GetType(DataType));
                                //newRow[FieldsMap[Nodes[i].Keys.ToArray()[j]].Split(',')[1]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];

                            }
                            else
                            {
                                //if (Nodes[i].Keys.ToArray()[j] != "geometry")
                                //{
                                //    dataTable.Columns.Add(Nodes[i].Keys.ToArray()[j]);
                                //    newRow[Nodes[i].Keys.ToArray()[j]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                                //}
                                dataTable.Columns.Add(Nodes[i].Keys.ToArray()[j]);
                                newRow[Nodes[i].Keys.ToArray()[j]] = Nodes[i][Nodes[i].Keys.ToArray()[j]];
                            }
                        }
                    }
                    #endregion
                }
                dataTable.Rows.Add(newRow);
            }
            #endregion
            return dataTable;
        }
        public static string ChineseUrlEncode(string url)
        {
            url = url.Replace("/", "\\");
            string[] strs = url.Split('\\');
            for(int i = 0; i < strs.Length; i++)
            {
                if (HasChinese(strs[i]))
                {
                    strs[i] = System.Web.HttpUtility.UrlEncode(strs[i]);
                }
            }
            string EncodeUrl = string.Empty;
            for(int i = 0; i < strs.Length-1; i++)
            {
                EncodeUrl += strs[i]+"\\";
            }
            EncodeUrl += strs[strs.Length-1];
            return EncodeUrl;
        }
        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }
        public static string ConvertWKBToWKT(byte[] wkb)
        {
            WKTWriter writer = new WKTWriter();
            WKBReader reader = new WKBReader();
            return writer.Write(reader.Read(wkb));
        }
        public static List<Geometry> Wkt2Geometry(string WKT,int From_WKID,int To_WKID)
        {
            List<Geometry> GeoList = new List<Geometry>();
            if (WKT.Contains("MULTIPOLYGON"))
            {
                WKT = WKT.Replace("MULTIPOLYGON", "");
                string[] temp = WKT.Split(')');
                List<string> PointList = new List<string>();
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = temp[i].Replace(",(", "");
                    temp[i] = temp[i].Replace("(", "");
                    if (temp[i] != "")
                    {
                        if (temp[i][0] == ',')
                        {
                            temp[i] = temp[i].Substring(1, temp[i].Length - 1);
                        }
                        PointList.Add(temp[i]);
                    }
                }
                for (int kk = 0; kk < PointList.Count; kk++)
                {
                    string[] Points = PointList[kk].Split(',');
                    for (int i = 0; i < Points.Count(); i++)
                    {
                        Points[i] = Points[i].Trim();
                    }
                    string PointTemplate = "[{0}]";
                    string PointString = string.Empty;
                    for (int i = 0; i < Points.Length; i++)
                    {
                        PointString += string.Format(PointTemplate, Points[i].Replace(' ', ',')) + ",";
                    }
                    PointString = PointString.Substring(0, PointString.Length - 1);
                    string ArcJson = "{\"rings\":[[" + PointString + "]],\"spatialReference\":{\"wkid\":" + From_WKID.ToString() + "}}";
                    Esri.ArcGISRuntime.Geometry.Geometry Geo = Esri.ArcGISRuntime.Geometry.Geometry.FromJson(ArcJson);
                    SpatialReference Sf = new SpatialReference(To_WKID);
                    Esri.ArcGISRuntime.Geometry.Geometry TransGeo = GeometryEngine.Project(Geo, Sf) as Esri.ArcGISRuntime.Geometry.Geometry;
                    GeoList.Add(TransGeo);
                }
            }
            else if (WKT.Contains("POLYGON"))
            {
                WKT = WKT.Replace("POLYGON", "");
                WKT = WKT.Replace("(", "");
                WKT = WKT.Replace(")", "");
                string[] Points = WKT.Split(',');
                for (int i = 0; i < Points.Count(); i++)
                {
                    Points[i] = Points[i].Trim();
                }
                string PointTemplate = "[{0}]";
                string PointString = string.Empty;
                for (int i = 0; i < Points.Length; i++)
                {
                    PointString += string.Format(PointTemplate, Points[i].Replace(' ', ',')) + ",";
                }
                PointString = PointString.Substring(0, PointString.Length - 1);
                string ArcJson = "{\"rings\":[[" + PointString + "]],\"spatialReference\":{\"wkid\":" + From_WKID.ToString() + "}}";
                Esri.ArcGISRuntime.Geometry.Geometry Geo = Esri.ArcGISRuntime.Geometry.Geometry.FromJson(ArcJson);
                SpatialReference Sf = new SpatialReference(To_WKID);
                Esri.ArcGISRuntime.Geometry.Geometry TransGeo = GeometryEngine.Project(Geo, Sf) as Esri.ArcGISRuntime.Geometry.Geometry;
                GeoList.Add(TransGeo);
            }
            else return null;
            return GeoList;

        }
        public static string Geometry2WktWgs84(Geometry Geo)
        {
            Esri.ArcGISRuntime.Geometry.Geometry TransGeo = GeometryEngine.Project(Geo, SpatialReferences.Wgs84) as Esri.ArcGISRuntime.Geometry.Geometry;
            string ArcJson = TransGeo.ToJson();
            Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(ArcJson);
            string Points = string.Empty;
            try
            {
                if (jobject["rings"].Count() > 1)
                {
                    
                    for(int i = 0; i < jobject["rings"].Count(); i++)
                    {
                        Points += "((";
                        for (int j=0;j<jobject["rings"][i].Count();j++)
                        {
                            Points += jobject["rings"][i][j].ToArray()[0].ToString() + " " + jobject["rings"][i][j].ToArray()[1].ToString() + ",";
                        }
                        Points = Points.Substring(0, Points.Length - 1);
                        Points += ")),";
                    }
                    Points = Points.Substring(0, Points.Length - 1);
                    string WktTemplate = "MULTIPOLYGON({0})";
                    WktTemplate = string.Format(WktTemplate, Points);
                    return WktTemplate;
                }
                else
                {
                    for (int i = 0; i < jobject["rings"][0].Count(); i++)
                    {
                        Points += jobject["rings"][0][i].ToArray()[0].ToString() + " " + jobject["rings"][0][i].ToArray()[1].ToString() + ",";
                    }
                    Points = Points.Substring(0, Points.Length - 1);
                    string WktTemplate = "POLYGON(({0}))";
                    WktTemplate = string.Format(WktTemplate, Points);
                    return WktTemplate;
                }
               
            }
            catch
            {
                string WktTemplate = "POINT({0})";
                WktTemplate = string.Format(WktTemplate, jobject["x"].ToString() + " " + jobject["y"].ToString());
                return WktTemplate;
            }
            
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string password)
        {
            string cl = password;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
                                    // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X2）则格式后的字符是大写字符 
                //X2 保证两位，否则通过网页计算的MD5值和程序计算值不一致
                pwd = pwd + s[i].ToString("X2");      
            }
            return pwd.ToLower();
        }
    }
}
