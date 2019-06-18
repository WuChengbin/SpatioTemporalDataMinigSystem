using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClusterAlgorithmForms;
namespace ClusterAlgorithmForms.GUI
{
    public partial class FormTrajectoryClustering : Form
    {
        string usr;
        string pwd;
        string IP;
        string PortNum;
        public FormTrajectoryClustering(string user,string password,string ip,string Port)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBox4.Text = "1";//以月为最小分析尺度，前后1个月为时间窗口，时间窗口为2
            textBox2.Text = "30";//最大移动距离30km
            textBox3.Text = "5";//共享最近邻个数
            usr = user;
            pwd = password;
            IP = ip;
            PortNum = Port;          
        }
        private void buttonOK_Click(object sender, EventArgs e)
        {
            List<string> temp= Neo4j64.queryNodeIDList(IP, PortNum, usr, pwd, "RainStormEventNode");//获取所有ID列表
            List<Trajectory> TrajectoryList = new List<Trajectory>();//所有事件轨迹列表
            for(int i=0;i<temp.Count;i++)
            {
                //获取每个ID节点列表
                List<Dictionary<string, string>> Dic = Neo4j64.queryEventState(IP, PortNum, usr, pwd, null,temp[i]);
                //获取每个ID的事件信息
                Dictionary<string, string> Dic2 = Neo4j64.queryEventInfo(IP, PortNum, usr, pwd, temp[i]);
                string eventduration = Dic2["Duration"];
                DateTime EventDuration= DateTime.ParseExact(eventduration, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                
                List<TrajectoryNode> NodeList = new List<TrajectoryNode>();//存储每个事件的节点列表
                List<STrajectory> STrajectoryList = new List<STrajectory>();//每个事件的序列轨迹列表
                             
                //解析Dictionary
                for (int j = 0; j < Dic.Count; j++)
                {
                    //解析出每个事件中的所有节点
                    TrajectoryNode node = new TrajectoryNode();
                    string latstring = Dic[j]["CoreLat"];
                    string logstring = Dic[j]["CoreLog"];
                    string idstring = Dic[j]["StormID"];
                    string timestring = Dic[j]["Time"];
                    string stateidstring = Dic[j]["StateID"];
                    node.Lat = double.Parse(latstring);
                    node.Log = double.Parse(logstring);
                    node.ID = int.Parse(idstring);
                    node.time = Convert.ToDateTime(timestring);
                    node.type = Dic[j]["Type"];
                    node.stateid = stateidstring;
                    node.isvisited = false;
                    NodeList.Add(node);
                }

                int number = 0;
                for(int n=0;n<NodeList.Count;n++)
                {
                    if(int.Parse(NodeList[n].type)!=5 || int.Parse(NodeList[n].type) != 1)//非发展和结束节点个数统计
                    {
                        number++;
                    }
                }

                TrajectoryNode[][] SNode = new TrajectoryNode[number][];//number表示最大行数，交错数组

                int M = 0;

                /*构造序列轨迹*/
                for (int k=0;k<NodeList.Count;k++)
                {
                    if(NodeList[k].isvisited == true || int.Parse(NodeList[k].type)== 5)
                            continue;

                    List<int> childidlist = new List<int>();//存储与当前节点相连且为非发展节点的id

                    //数据库查询语言，找出与当前节点相连、在此节点之后的"非发展"节点，将节点在nodelist中的id放到childidlist中，如果childidlist为空，则continue，否则执行下面的语句

                    NodeList[k].isvisited = true;
                    STrajectory strajectory = new STrajectory();
                    SNode[M] = new TrajectoryNode[childidlist.Count];
                    for(int m=0;m<childidlist.Count;m++)
                    {
                        SNode[M][m] = NodeList[childidlist[m]];
                        if (SNode[M][m].isvisited == true)
                            continue;
                       
                        int compNum = DateTime.Compare(SNode[M][m].time, NodeList[k].time);
                        if(compNum>0)
                        {
                            strajectory.ID = NodeList[k].ID;
                            strajectory.StartLat = NodeList[k].Lat;
                            strajectory.StartLog = NodeList[k].Log;
                            strajectory.StartTime = NodeList[k].time;
                            strajectory.Stype = NodeList[k].type;
                            strajectory.EndLat = SNode[M][m].Lat;
                            strajectory.EndLog = SNode[M][m].Log;
                            strajectory.EndTime = SNode[M][m].time;
                            strajectory.Etype = SNode[M][m].type;

                            STrajectoryList.Add(strajectory);
                        }
                        if(compNum<0)
                        {
                            strajectory.ID = NodeList[k].ID;
                            strajectory.EndLat = NodeList[k].Lat;
                            strajectory.EndLog = NodeList[k].Log;
                            strajectory.EndTime = NodeList[k].time;
                            strajectory.Etype = NodeList[k].type;
                            strajectory.StartLat = SNode[M][m].Lat;
                            strajectory.StartLog = SNode[M][m].Log;
                            strajectory.StartTime = SNode[M][m].time;
                            strajectory.Stype = SNode[M][m].type;

                            STrajectoryList.Add(strajectory);
                        }
                    }
                    M++;
                    if (M > number)
                        break;
               
                }

                //将事件轨迹压入轨迹集合中
                Trajectory trajectory = new Trajectory();
                trajectory.duration = EventDuration;
                trajectory.ID = NodeList[0].ID;
                trajectory.STrajectoryList = STrajectoryList;
                TrajectoryList.Add(trajectory);

            }//完成全部事件轨迹的表达


            /*层次轨迹相似性度量方法*/

            double Sthreshold = 0.0;//空间距离阈值
            Sthreshold = double.Parse(textBox2.Text.ToString());
            double Tthreshold = 0.0;//时间距离阈值
            Tthreshold = double.Parse(textBox4.Text.ToString());

            double[,] similarmatrix = new double[TrajectoryList.Count, TrajectoryList.Count];//邻近度矩阵
            for(int j=0;j<TrajectoryList.Count;j++)
            {
                for(int i=0;i<TrajectoryList.Count;i++)
                {
                    similarmatrix[j,i] =1;
                }
                
            }
                

            for (int j=0;j<TrajectoryList.Count;j++)
            {
               
                for (int i=0;i<j;i++)//保证只有对角线一侧的进行计算，减少复杂度
                {
                    List<StrajectoryPair> strpairlist = new List<StrajectoryPair>();//与每个事件轨迹都有一个相似轨迹对儿集合
                   
                    if (TrajectoryList[i].ID != TrajectoryList[j].ID)
                    {
                        //序列相似性度量
                        for (int k=0;k<TrajectoryList[j].STrajectoryList.Count;k++)
                        {
                            for(int n=0;n<TrajectoryList[i].STrajectoryList.Count;n++)
                            {
                                bool SimilarNumberStr = false;//结构相似性
                                bool SimilarNumberS = false;//空间邻近性
                                bool SimilarNumberT = false;//时间临近性

                                //结构相似性
                                if (TrajectoryList[i].STrajectoryList[n].Stype== TrajectoryList[j].STrajectoryList[k].Stype && TrajectoryList[i].STrajectoryList[n].Etype== TrajectoryList[j].STrajectoryList[k].Etype)
                                {
                                    SimilarNumberStr=true;
                                }

                                //空间邻近性（刚开始想利用共享最近邻来做，但是意义不大，先利用起点和终点的直接欧式距离进行运算；后再改进，这部分不是重点，可采用其他Flow聚类中的相似性衡量方法）
                                double Sdistance = 0.0;
                                double Edistance = 0.0;
                                
                                Sdistance = Math.Pow((TrajectoryList[i].STrajectoryList[n].StartLat - TrajectoryList[j].STrajectoryList[k].StartLat), 2)+Math.Pow((TrajectoryList[i].STrajectoryList[n].StartLog - TrajectoryList[j].STrajectoryList[k].StartLog),2);
                                Sdistance = Math.Sqrt(Sdistance);
                                Edistance= Math.Pow((TrajectoryList[i].STrajectoryList[n].EndLat - TrajectoryList[j].STrajectoryList[k].EndLat), 2) + Math.Pow((TrajectoryList[i].STrajectoryList[n].EndLog - TrajectoryList[j].STrajectoryList[k].EndLog), 2);
                                Edistance = Math.Sqrt(Edistance);

                                if(Sdistance<=Sthreshold && Edistance<=Sthreshold)
                                {
                                    SimilarNumberS = true;
                                }

                                //时间临近性，考虑周期和分析尺度，以下先以月尺度，不考虑周期性为例
                                string timescale = comboBox1.Text;
                                switch(timescale)
                                {
                                    case "年":

                                        break;
                                    case "季":
                                        break;
                                    case "月":
                                        break;
                                    case "天":
                                        break;
                                    case "时刻":
                                        break;
                                    default:
                                        break;
                                }
                                double SDmonth = 0.0;
                                double EDmonth = 0.0;

                                string Sm = TrajectoryList[i].STrajectoryList[n].StartTime.Month.ToString();
                                string Em = TrajectoryList[i].STrajectoryList[n].EndTime.Month.ToString();
                                string Smm = TrajectoryList[j].STrajectoryList[k].StartTime.Month.ToString();
                                string Emm = TrajectoryList[j].STrajectoryList[k].EndTime.Month.ToString();

                                SDmonth = Math.Abs(double.Parse(Sm) - double.Parse(Smm));
                                EDmonth = Math.Abs(double.Parse(Em)-double.Parse(Emm));

                                if(SDmonth<=Tthreshold && EDmonth<=Tthreshold)
                                {
                                    SimilarNumberT = true;
                                }

                                //序列相似性衡量，根据用户输入，case语句,以下以空间、时间、结构均相似为例
                                if(SimilarNumberS==true && SimilarNumberStr==true && SimilarNumberT==true)
                                {
                                    StrajectoryPair spair = new StrajectoryPair();
                                    spair.SNowid = TrajectoryList[j].STrajectoryList[k].Sstateid;
                                    spair.ENowid = TrajectoryList[j].STrajectoryList[k].Estateid;
                                    spair.SNowtime = TrajectoryList[j].STrajectoryList[k].StartTime;
                                    spair.ENowtime = TrajectoryList[j].STrajectoryList[k].EndTime;
                                    spair.SMatchid = TrajectoryList[i].STrajectoryList[n].Sstateid;
                                    spair.EMatchid = TrajectoryList[i].STrajectoryList[n].Estateid;
                                    spair.SMatchtime = TrajectoryList[i].STrajectoryList[n].StartTime;
                                    spair.EMatchtime = TrajectoryList[i].STrajectoryList[n].EndTime;

                                    strpairlist.Add(spair);
                                }

                            }
                        }


                        //事件轨迹相似性度量

                        List<int> Maxnumberlist = new List<int>();//记录每个结构的最大相似序列轨迹数
                        List<TimeSpan> MaxTimelist = new List<TimeSpan>();//记录每个结构的最大持续时间

                        for(int m=0;m< strpairlist.Count;m++)
                        {
                            List<StrajectoryPair> Nowlist = new List<StrajectoryPair>();
                            List<StrajectoryPair> Nextlist = new List<StrajectoryPair>();
                            int Snumber = 1;
                            TimeSpan Duration;
                            DateTime min = strpairlist[m].SNowtime;
                            DateTime max = strpairlist[m].ENowtime;
                                                        
                            if (strpairlist[m].isvisited == true)
                                continue;
                            strpairlist[m].isvisited = true;
                            Nowlist.Add(strpairlist[m]);

                            while(true)
                            {
                                for (int k = 0; k < Nowlist.Count; k++)
                                {
                                    Nextlist.Clear();
                                    for (int n = 0; n < strpairlist.Count; n++)
                                    {
                                        if (strpairlist[n].isvisited==false)
                                        {
                                            if ((strpairlist[n].SNowid == strpairlist[m].SNowid && strpairlist[n].SMatchid == strpairlist[m].SMatchid) || (strpairlist[n].ENowid == strpairlist[m].SNowid && strpairlist[n].EMatchid == strpairlist[m].SMatchid))
                                            {
                                                if (strpairlist[n].ENowid == strpairlist[m].ENowid)//避免出现相同轨迹
                                                    continue;

                                                Nextlist.Add(strpairlist[n]);
                                                strpairlist[n].isvisited = true;
                                                Snumber++;

                                                if (strpairlist[n].SNowtime < min)
                                                    min = strpairlist[n].SNowtime;
                                                if (strpairlist[n].ENowtime > max)
                                                    max = strpairlist[n].ENowtime;
                                                //Duration = max - min;
                                            }
                                            if((strpairlist[n].ENowid==strpairlist[m].ENowid && strpairlist[n].EMatchid==strpairlist[m].EMatchid)||(strpairlist[n].SNowid==strpairlist[m].ENowid && strpairlist[n].SMatchid==strpairlist[m].EMatchid))
                                            {
                                                if (strpairlist[n].SNowid == strpairlist[m].SNowid)
                                                    continue;

                                                Nextlist.Add(strpairlist[n]);
                                                strpairlist[n].isvisited = true;
                                                Snumber++;

                                                if (strpairlist[n].SNowtime < min)
                                                    min = strpairlist[n].SNowtime;
                                                if (strpairlist[n].ENowtime > max)
                                                    max = strpairlist[n].ENowtime;
                                            }
                                           // Duration = max - min;
                                        }
                                    }
                                }
                                if (Nextlist.Count != 0)
                                    Nowlist = new List<StrajectoryPair>(Nextlist);
                                else
                                    break;
                            }

                            Duration = max - min;
                            Maxnumberlist.Add(Snumber);
                            MaxTimelist.Add(Duration);
                    
                        }
                        //寻找最大相似性结构，即包含的最多相连的序列轨迹数目
                        //寻找最长的相似性持续时间
                        int maxnumber = 0;
                        int maxduration=0;
                        for(int k=0;k<Maxnumberlist.Count;k++)
                        {
                            if (Maxnumberlist[k] > maxnumber)
                                maxnumber = Maxnumberlist[k];
                        }
                        for(int n=0;n<MaxTimelist.Count;n++)
                        {
                            if(MaxTimelist[n].Hours>maxduration)//此处取“小时”，根据数据不同，这里需要修改
                            {
                                maxduration = MaxTimelist[n].Hours;
                            }

                        }

                        //计算两个事件轨迹的相似性,放入邻近度矩阵中
                        similarmatrix[j,i] = 0.5 * maxnumber / (TrajectoryList[j].STrajectoryList.Count + TrajectoryList[i].STrajectoryList.Count - maxnumber)+0.5*maxduration/(TrajectoryList[j].duration.Hour+ TrajectoryList[i].duration.Hour-maxduration);
                    }

                }
            }
            for(int j=0;j<TrajectoryList.Count;j++)
            {
                for (int i = 0; i < j; i++)
                {
                    similarmatrix[i, j] = similarmatrix[j, i];//给另外一侧对角线赋值
                }
            }

            /* 构造完成邻近度矩阵*/

        }
        private void buttonSave_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "选择输出文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBoxFilePath.Text = fbd.SelectedPath;
            }
        }


    }
}
