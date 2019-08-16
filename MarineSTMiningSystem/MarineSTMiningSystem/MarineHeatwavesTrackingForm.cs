using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class MarineHeatwavesTrackingForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接
        //Thread[] threads;//线程
        //OracleConnection[] orcConns;//数据库连接，对应线程
        string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID

        public MarineHeatwavesTrackingForm(OracleConnection _conn)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            conn = _conn;
            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                eventStateRelationComboBox.Items.Add(tableName[0].ToString());
                eventComboBox.Items.Add(tableName[0].ToString());
            }
        }

        public MarineHeatwavesTrackingForm(string connString)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            conn = new OracleConnection(connString);
            conn.Open();
            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                tableNameComboBox.Items.Add(tableName[0].ToString());
                eventStateRelationComboBox.Items.Add(tableName[0].ToString());
                eventComboBox.Items.Add(tableName[0].ToString());
            }
        }
        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="sql">SQL查询语句</param>
        /// <returns>结果Table</returns>
        public DataTable QueryResultTable(string sql)
        {
            DataTable ResultDT = new DataTable();
            try
            {
                OracleDataAdapter objAdaper;
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                objAdaper = new OracleDataAdapter(cmd);
                objAdaper.Fill(ResultDT);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return ResultDT;
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法

        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (e.Cancelled)
            {//用户取消
                MessageBox.Show("处理取消！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (e.Error != null)
            {//异常结束
                MessageBox.Show(e.Error.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            progressBar1.Value = e.ProgressPercentage;
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            if (!System.IO.Directory.Exists(textBox1.Text.Trim()))
            {
                MessageBox.Show("时间数据文件夹路径有误！");
                return;
            }
            if (!System.IO.Directory.Exists(textBox2.Text.Trim()))
            {
                MessageBox.Show("流场数据文件夹路径有误！");
                return;
            }
            //timeFolderPath = textBox2.Text.Trim();
            //timeFolderPath = timeFolderPath.TrimEnd('/').TrimEnd('\\');

            //oscarFolderPath = textBox4.Text.Trim();
            //oscarFolderPath = outFolderPath.TrimEnd('/').TrimEnd('\\');
            ////eventStateTBName = tableNameComboBox.Text.Trim();
            ////eventStateRelTBName = eventStateRelationComboBox.Text.Trim();
            ////eventTBName = eventComboBox.Text.Trim();
            ////timeZone = timeZoneCB.Text.Trim();
            //threadCount = Convert.ToInt32(threadCountTextBox.Text.Trim());


            //threads = new Thread[threadCount];//线程数
            //orcConns = new OracleConnection[threadCount];//数据库连接数

            ////数据库连接
            ////for (int i = 0; i < orcConns.Length; i++)
            ////{
            ////    orcConns[i] = DatabaseConnection.GetNewOracleConnection();//获取连接
            ////    orcConns[i].Open();//打开连接
            ////}
            ////数据库连接
            //for (int i = 0; i < orcConns.Length; i++)
            //{
            //    orcConns[i] = new OracleConnection(conn.ConnectionString);//获取连接
            //    orcConns[i].Open();//打开连接
            //}

            //eventStateTBName = tableNameComboBox.Text.Trim();
            //eventStateRelTBName = eventStateRelationComboBox.Text.Trim();
            //eventTBName = eventComboBox.Text.Trim();

            //pStartTime = DateTime.Now;//记录处理开始时间
            //progressBar1.Show();
            //worker.RunWorkerAsync();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                worker.CancelAsync();//取消后台
            }
            else
            {
                //toolStripStatusLabel1.Text = "未进行处理";
                MessageBox.Show("未进行处理！");
            }
        }

        private void selectButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择时间数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox1.Text = fbd.SelectedPath;
            }
        }

        private void selectButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择流场纬向(u)数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox2.Text = fbd.SelectedPath;
            }
        }

        private void selectButton3_Click(object sender, EventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            //fbd.SelectedPath = @"E:\";
            fbd.Description = "选择流场经向(v)数据文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {//确定
                textBox3.Text = fbd.SelectedPath;
            }
        }
    }
}
