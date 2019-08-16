using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class EventToDatabaseForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接
        //string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID

        public EventToDatabaseForm(OracleConnection _conn)
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
                aimTableComboBox.Items.Add(tableName[0].ToString());
                corrTableComboBox.Items.Add(tableName[0].ToString());
            }
        }

        public EventToDatabaseForm(string connString)
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            conn = new OracleConnection(connString);
            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {//下拉列表显示表名
                aimTableComboBox.Items.Add(tableName[0].ToString());
                corrTableComboBox.Items.Add(tableName[0].ToString());
            }
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法

            StreamReader sr = new StreamReader(filePath);
            string line = sr.ReadLine();
            string[] fields = line.Split(',');
            int corrFieldId = Array.IndexOf(fields, fileFieldName);

            bool exist = IsExistTable(aimTableName); //查询数据库是否存在表
            if(!exist)
            {//不存在目标表
                string creatSql = "CREATE TABLE " + aimTableName + "( OID NUMBER(*, 0) NOT NULL,";
                //输出属性表字段的详细信息，数据类型、宽度、精度等
                for (int i = 0; i < fields.Length; i++)
                {//每个属性
                    creatSql += fields[i] + " VARCHAR2(" + dbCharWidth + " BYTE),";

                }
                //creatSql = creatSql.TrimEnd(',');
                creatSql += geoTagName + " MDSYS.SDO_GEOMETRY , CONSTRAINT " + aimTableName + "_PK PRIMARY KEY ( OID ))";
                OracleCommand inserCmd = new OracleCommand(creatSql, conn);
                inserCmd.ExecuteNonQuery();//执行数据库操作
            }

            int oid = 0;
            line = sr.ReadLine();
            while(line!=null&&line.Trim().Length>0)
            {
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                worker.ReportProgress(oid);//记录进度

                fields = line.Split(',');
                string corrField = fields[corrFieldId];
                string sql = "select oid from " + corrTableName + " where " + tableFieldName + "='" + corrField + "'";
                DataTable result = QueryResultTable(sql);
                if(result.Rows.Count>0)
                {//存在状态
                    sql = "declare v_geo MDSYS.SDO_GEOMETRY; begin select " + geoTagName + " into v_geo from " + corrTableName + " where oid='" + result.Rows[0][0].ToString() + "'; insert into " + aimTableName + " values ('" + (++oid) + "',";
                    foreach(string field in fields)
                    {
                        sql += "'" + field + "',";
                    }
                    sql+= "v_geo); end;";

                    OracleCommand inserCmd = new OracleCommand(sql, conn);
                    inserCmd.ExecuteNonQuery();//执行数据库操作

                    //合并后面的状态
                    for (int i = 1; i < result.Rows.Count; i++)
                    {
                        sql = "declare v_geo1 MDSYS.SDO_GEOMETRY;v_geo2 MDSYS.SDO_GEOMETRY; begin select " + geoTagName + " into v_geo1 from "+ aimTableName + " where oid='"+oid+"'; select space into v_geo2 from "+ corrTableName + " where oid='"+ result.Rows[i][0].ToString() + "'; update " + aimTableName + " set " + geoTagName + "=SDO_GEOM.SDO_UNION(v_geo1,v_geo2,0.001) where oid='" + oid + "'; end;";

                        inserCmd = new OracleCommand(sql, conn);
                        inserCmd.ExecuteNonQuery();//执行数据库操作
                    }
                }
                line = sr.ReadLine();
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

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "文本文件|*.txt|所有文件|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = ofd.FileName;

                StreamReader sr = new StreamReader(ofd.FileName);
                string line = sr.ReadLine();
                sr.Close();
                string[] fields = line.Split(',');
                fileFieldComboBox.Items.Clear();
                foreach (string field in fields)
                {
                    fileFieldComboBox.Items.Add(field);
                }
            }
        }

        int dbCharWidth = 50;//数据库varchar宽度
        string geoTagName = "space";
        string filePath = "";
        string aimTableName = "";
        string corrTableName = "";
        string fileFieldName = "";
        string tableFieldName = "";
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！","提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!File.Exists(pathTextBox.Text.Trim()))
            {
                MessageBox.Show("文件不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (geoTextBox.Text.Trim() == ""|| aimTableComboBox.Text.Trim()==""|| corrTableComboBox.Text.Trim() == ""|| fileFieldComboBox.Text.Trim()==""|| tableFieldComboBox.Text.Trim()=="")
            {
                MessageBox.Show("请完整填写参数！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            filePath = pathTextBox.Text.Trim();
            geoTagName = geoTextBox.Text.Trim();
            aimTableName = aimTableComboBox.Text.Trim();
            corrTableName = corrTableComboBox.Text.Trim();
            fileFieldName = fileFieldComboBox.Text.Trim();
            tableFieldName = tableFieldComboBox.Text.Trim();

            //progressBar1.Show();
            label7.Text = "0";
            label7.Show();
            worker.RunWorkerAsync();
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
                //progressBar1.Value = 100;
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            label7.Hide();
            //progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            label7.Text = e.ProgressPercentage.ToString();
        }

        private void corrTableComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            tableFieldComboBox.Items.Clear();
            if (corrTableComboBox.Text.Trim().Length>0)
            {
                if(IsExistTable(corrTableComboBox.Text.Trim()))
                {
                    DataTable fileds = GetTableFields(corrTableComboBox.Text);
                    if(fileds.Rows.Count>0)
                    {
                        foreach(DataRow row in fileds.Rows)
                        {
                            tableFieldComboBox.Items.Add(row[0]);
                        }
                    }
                }
            }
        }

        private DataTable GetTableFields(string tableName)
        {
            string sql = "select column_name from user_tab_columns where TABLE_NAME = upper('" + tableName + "')";
            DataTable tableNames = QueryResultTable(sql);
            return tableNames;
        }

        private bool IsExistTable(string tableName)
        {
            bool exist = false;
            string sql = "select count(*) from user_tables where table_name=upper('" + tableName + "')";
            DataTable tableNames = QueryResultTable(sql);
            if (tableNames.Rows.Count > 0 && Convert.ToInt32(tableNames.Rows[0][0]) > 0) exist = true;
            return exist;
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

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                string sql = "delete from " + aimTableComboBox.Text.Trim();
                OracleCommand inserCmd = new OracleCommand(sql, conn);
                inserCmd.ExecuteNonQuery();//执行数据库操作
                MessageBox.Show("表已清空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
