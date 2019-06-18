using Oracle.ManagedDataAccess.Client;
using OSGeo.GDAL;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClusterAlgorithmForms.GUI
{
    public partial class ShpToDatabaseForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();//oracle数据库连接
        string sdoSrid = "4326";//WGS 84 坐标系在oracle spatial中的SRID

        public ShpToDatabaseForm(OracleConnection _conn)
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
                tableComboBox.Items.Add(tableName[0].ToString());
            }

            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            Ogr.RegisterAll();// 注册所有的驱动
        }

        public ShpToDatabaseForm(string connString)
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
                tableComboBox.Items.Add(tableName[0].ToString());
            }

            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            Gdal.SetConfigOption("SHAPE_ENCODING", "");
            Ogr.RegisterAll();// 注册所有的驱动
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            string path = shpFileNames[0].ToString();
            DataSource ds = Ogr.Open(path, 0);//0表示只读，1表示可修改 
            if (ds == null)
            {//数据为空
                MessageBox.Show("数据不存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ds.Dispose();//关闭数据集
                return;
            }
            int iLayerCount = ds.GetLayerCount();//图层个数
            if (iLayerCount < 1)
            {
                MessageBox.Show("图层个数为零!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ds.Dispose();//关闭数据集
                return;
            }
            Layer oLayer = ds.GetLayerByIndex(0);// 获取第一个图层
            if (oLayer == null)
            {//图层为空
                MessageBox.Show("图层为空!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                oLayer.Dispose();
                ds.Dispose();//关闭数据集
                return;
            }
            FeatureDefn oDefn = oLayer.GetLayerDefn();
            int fieldCount = oDefn.GetFieldCount();

            bool exist = IsExistTable(tableName); //查询数据库是否存在表
            if (!exist)
            {//不存在表，创建
                string creatSql = "CREATE TABLE " + tableName + " ( OID NUMBER(*, 0) NOT NULL,";
                //输出属性表字段的详细信息，数据类型、宽度、精度等
                for (int i = 0; i < fieldCount; i++)
                {//每个属性
                    FieldDefn oField = oDefn.GetFieldDefn(i);
                    string name = oField.GetNameRef();
                    FieldType type = oField.GetFieldType();
                    int width = oField.GetWidth();
                    int Precision = oField.GetPrecision();
                    creatSql += " " + name;
                    switch (type)
                    {
                        case FieldType.OFTString:
                            creatSql += " VARCHAR2(" + width + " BYTE)";
                            break;
                        case FieldType.OFTInteger:
                        case FieldType.OFTInteger64:
                            creatSql += " NUMBER(*, 0)";
                            break;
                        case FieldType.OFTReal:
                            creatSql += " NUMBER";
                            break;
                        case FieldType.OFTDate:
                        case FieldType.OFTDateTime:
                            creatSql += " TIMESTAMP(6)";
                            break;
                        default:
                            MessageBox.Show("未处理的类型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            oField.Dispose();
                            oDefn.Dispose();
                            oLayer.Dispose();
                            ds.Dispose();//关闭数据集
                            return;
                            //break;
                    }
                    creatSql += ",";
                    oField.Dispose();

                }
                //creatSql = creatSql.TrimEnd(',');
                creatSql += ""+geoTagName+ " MDSYS.SDO_GEOMETRY , CONSTRAINT "+tableName+"_PK PRIMARY KEY ( OID ))";
                OracleCommand inserCmd = new OracleCommand(creatSql, conn);
                inserCmd.ExecuteNonQuery();//执行数据库操作
            }
            
            FieldType[] types = new FieldType[fieldCount];
            for (int i = 0; i < fieldCount; i++)
            {//每个属性
                FieldDefn oField = oDefn.GetFieldDefn(i);
                types[i] = oField.GetFieldType();
            }

            oDefn.Dispose();
            oLayer.Dispose();
            ds.Dispose();//关闭数据集

            //储存
            int oid = 1;
            for (int fileId=0;fileId< mFileNum;fileId++)
            {//每个图层
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / mFileNum;//进度
                worker.ReportProgress(progress);//记录进度

                path = shpFileNames[fileId].ToString();
                ds = Ogr.Open(path, 0);//0表示只读，1表示可修改 
                oLayer = ds.GetLayerByIndex(0);// 获取第一个图层
                Feature oFeature = null;
                while ((oFeature = oLayer.GetNextFeature()) != null)
                {//每个多边形
                    string polygonWkt = string.Empty;//wkt坐标
                    oFeature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);
                    string insertSql = "declare v_geo MDSYS.SDO_GEOMETRY:= sdo_geometry('" + polygonWkt + "'," + sdoSrid + "); begin insert into " + tableName + " values ( '" + oid + "',";
                    oid++;
                    for (int i = 0; i < fieldCount; i++)
                    {//每个属性
                        switch(types[i])
                        {
                            case FieldType.OFTString:
                                insertSql += " '" + oFeature.GetFieldAsString(i)+"',";
                            break;
                            case FieldType.OFTInteger:
                            case FieldType.OFTInteger64:
                                insertSql += " '" + oFeature.GetFieldAsInteger(i) + "',";
                                break;
                            case FieldType.OFTReal:
                                insertSql += " '" + oFeature.GetFieldAsDouble(i) + "',";
                                break;
                            case FieldType.OFTDate:
                            case FieldType.OFTDateTime:
                            default:
                                MessageBox.Show("未处理的类型！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                                oLayer.Dispose();
                                ds.Dispose();//关闭数据集
                            return;
                        }
                    }
                    //insertSql = insertSql.TrimEnd(',');
                    insertSql += "v_geo); end;";
                    OracleCommand inserCmd = new OracleCommand(insertSql, conn);
                    inserCmd.ExecuteNonQuery();//执行数据库操作
                    
                }
                oLayer.Dispose();
                ds.Dispose();//关闭数据集
            }
        }

        private bool IsExistTable(string tableName)
        {
            bool exist = false;
            string sql= "select count(*) from user_tables where table_name=upper('"+tableName+"')";
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

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "shp文件|*.shp|所有文件|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox1.Items.Add(fileName);
                }
                countTextBox.Text = listBox1.Items.Count.ToString();
            }
        }

        private void deleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            while (index > -1)
            {
                listBox1.Items.RemoveAt(index);
                index = listBox1.SelectedIndex;
            }
            countTextBox.Text = listBox1.Items.Count.ToString();
        }

        private void moveUpBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(endIndex + 1, listBox1.Items[startIndex - 1].ToString());
                    listBox1.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void moveDownBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox1.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox1.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox1.Items.Insert(startIndex, listBox1.Items[endIndex + 1].ToString());
                    listBox1.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox1.SelectedIndices;
                }
            }
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

        ListBox.ObjectCollection shpFileNames;//所有文件名
        int mFileNum = 0;
        string tableName = string.Empty;//事件表
        string geoTagName = "space";
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("请添加处理文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            geoTagName = geoTextBox.Text.Trim();
            shpFileNames = listBox1.Items;
            mFileNum = shpFileNames.Count;//文件个数
            tableName = tableComboBox.Text.Trim();
            progressBar1.Show();
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

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult dr = MessageBox.Show("确定清空表中所有数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                try
                {
                    string sql = "delete from " + tableComboBox.Text.Trim();
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
}
