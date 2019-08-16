using Oracle.ManagedDataAccess.Client;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class StormDatabaseForm : Form
    {
        string connString = "";//数据库连接语句
        OracleConnection conn = new OracleConnection();
        private BackgroundWorker worker = new BackgroundWorker();//后台线程

        public StormDatabaseForm()
        {
            InitializeComponent();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择需要处理的文件";
            ofd.Filter = "所有文件(*.*)|*.*";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathTextBox.Text = ofd.FileName;
            }
        }

        private void connectTextButton_Click(object sender, EventArgs e)
        {
            connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + addressTextBox.Text + ")(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=" + sidTextBox.Text + ")));Persist Security Info=True;User ID=" + nameTextBox.Text + ";Password=" + passwordTextBox.Text + ";";
            conn = new OracleConnection(connString);
            try
            {
                conn.Open();
                toolStripStatusLabel1.Text = "数据库连接成功";
                conn.Close();
                startButton.Enabled = true;
                cancelButton.Enabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if(File.Exists(pathTextBox.Text))
            {
                worker.RunWorkerAsync();
            }
            else
            {
                toolStripStatusLabel1.Text = "文件数据路径不正确";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if(worker.IsBusy)
            {
                worker.CancelAsync();//取消后台
            }
            else
            {
                toolStripStatusLabel1.Text = "未进行处理";
            }
        }

        private void worke(object sender, DoWorkEventArgs e)
        {
            StreamReader sr = new StreamReader(pathTextBox.Text);
            long fileSize = new FileInfo(pathTextBox.Text).Length;
            if (sr.ReadLine() == null) return;
            conn.Open();
            string line = sr.ReadLine();
            long lineCount = 2;
            while (line != null)
            {
                string[] record = line.Split(' ');
                string sql = string.Format("insert into storm_raster values ('{0}','{1}','{2}','{3}','{4}','{5}',to_date('{6}','YYYYMMDD_HH24MISS'),to_date('{7}','YYYYMMDD_HH24MISS'),'{8}','{9}','{10}','{11}','{12}')", record[0], record[1], record[2], record[3], record[4], record[5], record[6], record[7], record[8], record[9], record[10], record[11], record[12]);
                OracleCommand inserCmd = new OracleCommand(sql, conn);
                inserCmd.ExecuteNonQuery();
                if (lineCount % 100 == 0)
                {
                    worker.ReportProgress((int)(lineCount * 106 * 100 / fileSize));//记录进度
                    if (worker.CancellationPending)
                    {//取消
                        e.Cancel = true;
                        sr.Close();
                        conn.Close();
                        return;
                    }
                }
                line = sr.ReadLine();
                lineCount++;
            }
            sr.Close();
            conn.Close();
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel1.Text ="正在处理，已完成"+ e.ProgressPercentage.ToString() + '%';
        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                toolStripStatusLabel1.Text = "处理完成";
            }
            else
            {
                toolStripStatusLabel1.Text = "处理取消";
            }
        }
    }
}
