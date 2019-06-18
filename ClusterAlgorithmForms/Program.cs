using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Forms.GUI;
using ClusterAlgorithmForms.GUI;
using Oracle.ManagedDataAccess.Client;

namespace ClusterAlgorithmForms
{
    static class Program
    {
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string []args)
        {

            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

            //.Run(new FormTrajectoryClustering("neo4j","Snow800505","10.126.11.67","7687"));

            Neo4j64.isForwardConnected("10.126.11.67", "7687", "neo4j", "Snow800505", "260_1", "260_4");

            Neo4j64.updateTrajectoryID("123", "456", "SstProcessNode");
            //TrajectoryNode node1 = new TrajectoryNode();
            //node1.stateid = "863_62";

            //TrajectoryNode node2 = new TrajectoryNode();
            //node2.stateid = "863_61";

            //TrajectoryNode node3 = new TrajectoryNode();
            //node3.stateid = "863_65";

            //TrajectoryNode node4 = new TrajectoryNode();
            //node4.stateid = "863_64";

            //TrajectoryNode node5 = new TrajectoryNode();
            //node5.stateid = "863_66";


            //List<TrajectoryNode> TestList = new List<TrajectoryNode>();

            //TestList.Add(node1);
            //TestList.Add(node2);
            //TestList.Add(node3);
            //TestList.Add(node4);
            //TestList.Add(node5);

            //Neo4j64.queryNonDevelopNode("10.126.11.67", "7687", "neo4j", "Snow800505", "863_62", TestList);






            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //if (args.Length < 1)
            //{
            //    bool in64bit = (IntPtr.Size == 8);
            //    //if(in64bit)
            //    //{
            //    //    MessageBox.Show("未知的参数类型! x64");
            //    //}
            //    //else
            //    //{
            //    //    MessageBox.Show("未知的参数类型! x86");
            //    //}
            //    MessageBox.Show("未获取窗体参数", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //}
            //else if (args[0] == "DDBSC")
            //{
            //    Application.Run(new FormDDBSC());
            //}
            //else if (args[0] == "RoCMSAC")
            //{
            //    Application.Run(new FormRoCMSAC());
            //}
            //else if (args[0] == "RoSTCM")
            //{
            //    Application.Run(new FormRoSTCM());
            //}
            //else if (args[0] == "SpatialKmean")
            //{
            //    Application.Run(new FormSpatialKmean());
            //}
            //else if (args[0] == "STDBSCAN")
            //{
            //    Application.Run(new FormSTDASCAN());
            //}
            //else if (args[0] == "STSNN")
            //{
            //    Application.Run(new FormSTSNN());
            //}
            //else if (args[0] == "TimeKmean")
            //{
            //    Application.Run(new FormTimeKmean());
            //}
            //else if (args[0] == "WeightKmean")
            //{
            //    Application.Run(new FormWeightKmean());
            //}
            //else if (args[0] == "ClusterRelation")
            //{
            //    Application.Run(new FormClusterRelation());
            //}
            //else if (args[0] == "ClusterShpAddAttributes")
            //{
            //    Application.Run(new FormClusterShpAddAttribute());
            //}
            //else if (args[0] == "EventRelToDatabase")
            //{
            //    string connString = "";
            //    for (int i = 1; i < args.Length; i++)
            //    {
            //        connString += args[i] + " ";
            //    }
            //    connString.TrimEnd(' ');
            //    OracleConnection conn = new OracleConnection(connString);
            //    //conn.Open();
            //    Application.Run(new EventRelToDatabaseForm(conn));
            //}
            //else if (args[0] == "EventToDatabase")
            //{
            //    string connString = "";
            //    for (int i = 1; i < args.Length; i++)
            //    {
            //        connString += args[i] + " ";
            //    }
            //    connString.TrimEnd(' ');
            //    OracleConnection conn = new OracleConnection(connString);
            //    //conn.Open();
            //    Application.Run(new EventToDatabaseForm(conn));
            //}
            //else if (args[0] == "ShpToDatabase")
            //{
            //    string connString = "";
            //    for (int i = 1; i < args.Length; i++)
            //    {
            //        connString += args[i] + " ";
            //    }
            //    connString.TrimEnd(' ');
            //    OracleConnection conn = new OracleConnection(connString);
            //    //conn.Open();
            //    Application.Run(new ShpToDatabaseForm(conn));
            //}
            //else
            //{
            //    MessageBox.Show("未知的参数类型!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
        }
    }
}
