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
    public partial class OutToTxtForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接

        public OutToTxtForm(OracleConnection _conn)
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
            }
            tableNameComboBox.SelectedIndex = 0;

            ResetColNames();
        }

        public OutToTxtForm(string connString)
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
                tableNameComboBox.Items.Add(tableName[0].ToString());
            }

            ResetColNames();
        }

        void ResetColNames()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            string _tableName = tableNameComboBox.Text;
            if (IsExistTable(_tableName))
            {
                DataTable fileds = GetTableFields(_tableName);
                if (fileds.Rows.Count > 0)
                {
                    foreach (DataRow row in fileds.Rows)
                    {
                        listBox1.Items.Add(row[0]);
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
            StreamWriter sw = new StreamWriter(savePath);
            string sql = "select";
            string line = string.Empty;
            foreach(string colName in colNames)
            {
                sql += " " + colName + ",";
                line += colName + ",";
            }
            sql = sql.TrimEnd(',') + " from " + tableName;
            line = line.TrimEnd(',');
            DataTable results = QueryResultTable(sql);
            sw.WriteLine(line);

            for (int rowId=0;rowId<results.Rows.Count;rowId++)
            {//每条数据
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (rowId * 100) / results.Rows.Count;//进度
                worker.ReportProgress(progress);//记录进度

                line = string.Empty;
                for(int colId=0;colId< results.Columns.Count;colId++)
                {//每列
                    if(results.Rows[rowId][colId] is DateTime)
                    {
                        line += ((DateTime)results.Rows[rowId][colId]).ToString("yyyy-MM-dd HH:mm:ss") + ",";
                    }
                    else
                    {
                        line += results.Rows[rowId][colId].ToString() + ",";
                    }
                }
                line = line.TrimEnd(',');
                sw.WriteLine(line);
            }
            sw.Close();
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
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            progressBar1.Value = e.ProgressPercentage;
        }

        string[] colNames;
        string savePath = string.Empty;
        string tableName = string.Empty;
        private void okButton_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }
            bool exist = IsExistTable(tableNameComboBox.Text.Trim()); //查询数据库是否存在表
            if (!exist)
            {
                MessageBox.Show("表不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(listBox2.Items.Count==0)
            {
                MessageBox.Show("请选择输出字段", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt|csv文件|*.csv|所有文件|*.*";
            if (sfd.ShowDialog()==DialogResult.OK)
            {
                string fileName = sfd.FileName;
                //if (File.Exists(fileName))
                //{
                //    if(MessageBox.Show("文件已存在，是否覆盖？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)==DialogResult.Cancel)
                //    {
                //        return;
                //    }
                //}
                savePath = fileName;
                tableName = tableNameComboBox.Text.Trim();
                colNames = new string[listBox2.Items.Count];
                for(int i=0;i< listBox2.Items.Count;i++)
                {
                    colNames[i] = listBox2.Items[i].ToString();
                }
                progressBar1.Show();
                worker.RunWorkerAsync();
            }
        }

        private void tableNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetColNames();
        }

        private void addAllbutton_Click(object sender, EventArgs e)
        {
            var items = listBox1.Items;
            listBox2.Items.AddRange(items);
            listBox1.Items.Clear();
        }

        private void deleteAllButton_Click(object sender, EventArgs e)
        {
            var items = listBox2.Items;
            listBox1.Items.AddRange(items);
            listBox2.Items.Clear();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection items = listBox1.SelectedIndices;
            for(int i= 0;i< items.Count; i++)
            {
                listBox2.Items.Add(listBox1.Items[items[i]]);
            }
            for (int i = (items.Count-1); i >=0; i--)
            {
                listBox1.Items.RemoveAt(items[i]);
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox2.Items.Insert(endIndex + 1, listBox2.Items[startIndex - 1].ToString());
                    listBox2.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox2.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox2.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox2.Items.Insert(startIndex, listBox2.Items[endIndex + 1].ToString());
                    listBox2.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox2.SelectedIndices;
                }
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection items = listBox2.SelectedIndices;
            for (int i = 0; i < items.Count; i++)
            {
                listBox1.Items.Add(listBox2.Items[items[i]]);
            }
            for (int i = (items.Count - 1); i >= 0; i--)
            {
                listBox2.Items.RemoveAt(items[i]);
            }
        }
    }
}
