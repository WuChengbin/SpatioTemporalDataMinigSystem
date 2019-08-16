using Oracle.ManagedDataAccess.Client;
using OSGeo.GDAL;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class StormRasterExtractForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        OracleConnection conn = new OracleConnection();

        public StormRasterExtractForm(OracleConnection _conn)
        {
            InitializeComponent();

            conn = _conn;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach(DataRow tableName in tableNames.Rows)
            {
                tableNameComboBox.Items.Add(tableName[0].ToString());
            }
            tableNameComboBox.SelectedIndex = 0;
            timeZoneCB.SelectedIndex = 0;
        }
        public StormRasterExtractForm(string connString)
        {
            InitializeComponent();

            conn = new OracleConnection(connString);
            conn.Open();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的

            DataTable tableNames = QueryResultTable("SELECT table_name FROM User_tables");
            //if (tableNames.Rows.Count == 0) MessageBox.Show("数据库中不存在用户表，请确认数据库是否连接成功", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            foreach (DataRow tableName in tableNames.Rows)
            {
                tableNameComboBox.Items.Add(tableName[0].ToString());
            }
            tableNameComboBox.SelectedIndex = 0;
            timeZoneCB.SelectedIndex = 0;
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
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称
            string hdfFileName = oriFileNames[0].ToString();
            //string hdfFileName = @"E:\BaoYuTime\XM_2014_CJ_tif";
            //打开hdf文件
            Dataset ds = Gdal.Open(hdfFileName, Access.GA_ReadOnly);
            int col = ds.RasterXSize;//列数
            int row = ds.RasterYSize;//行数
            Band demband1 = ds.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息

            string projection = ds.GetProjection();//坐标系
                                                   //string[] mdl = ds.GetMetadataDomainList();//获取元数据的域
            string[] metadatas = ds.GetMetadata("");//获取元数据
            double startLog = 0.0;//起始经度
            double startLat = 0.0;//起始维度
            double endLog = 0.0;//结束经度
            double endLat = 0.0;//结束维度
            double mScale = 0.0;//比例
            double resolution = 0.0;//分辨率
            string dataType = "";
            foreach (string md in metadatas)
            {//获取信息
                string[] mdArr = md.Split('=');
                switch (mdArr[0])
                {
                    case "StartLog":
                        startLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "StartLat":
                        startLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLog":
                        endLog = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "EndLat":
                        endLat = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "Scale":
                        mScale = Convert.ToDouble(mdArr[1]);//起始经度
                        break;
                    case "DataType":
                        dataType = mdArr[1];//起始经度
                        break;
                    case "DSResolution":
                        resolution = Convert.ToDouble(mdArr[1]);
                        break;
                    default:
                        break;
                }
            }
            ds.Dispose();

            //string outPath = outFolderPath + "\\baoYuRaster.txt";
            //FileStream fs = new FileStream(outPath, FileMode.Create, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.WriteLine("OID 经度 纬度 事件ID 事件名称 类型 起始时间 终止时间 持续时间 平均降雨量 累计降雨量 瞬时最大降雨强度 降雨强度");
            int oid = 0;
            DataTable maxOid = QueryResultTable("SELECT max(oid) FROM " + tableName);
            if (maxOid.Rows[0][0] != DBNull.Value)
            {//非空
                oid = Convert.ToInt32(maxOid.Rows[0][0]);
            }
            //DateTime startT = DateTime.Now;
            //TimeSpan t1 = new TimeSpan();
            //TimeSpan t2 = new TimeSpan();
            int rowPart = 10;//每一部分行数
            for (int rowBig = 470; rowBig < row; rowBig += rowPart)
            {//逐行
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (rowBig * 100) / row;//进度
                worker.ReportProgress(progress);//记录进度
                //progressBar1.Value = (rowBig * 100) / row;
                //DateTime startT1 = DateTime.Now;
                GetFileMC gf1 = new GetFileMC(GetFile);
                IAsyncResult result1 = gf1.BeginInvoke(oriFileNames, mFileNum, rowBig, col, rowPart, null, null);

                GetFileMC gf2 = new GetFileMC(GetFile);
                IAsyncResult result2 = gf2.BeginInvoke(spFileNames, mFileNum, rowBig, col, rowPart, null, null);

                GetFileMC gf3 = new GetFileMC(GetFile);
                IAsyncResult result3 = gf3.BeginInvoke(idFileNames, mFileNum, rowBig, col, rowPart, null, null);


                int[,,] oriFiles = gf1.EndInvoke(result1);//用于接收返回值 
                int[,,] spFiles = gf2.EndInvoke(result2);//用于接收返回值 
                int[,,] idFiles = gf3.EndInvoke(result3);//用于接收返回值 
                                                         //t1 =t1.Add(DateTime.Now - startT1);

                /*
                int[,,] oriFiles = new int[10, col, mFileNum];
                int[,,] spFiles = new int[10, col, mFileNum];
                int[,,] idFiles = new int[10, col, mFileNum];
                for (int time = 0; time < mFileNum; time++)
                {//读取该行所有数据
                    string oriFileName = oriFilesName[time];
                    string spFileName = spFilesName[time];
                    string idFileName = idFilesName[time];

                    int[] oriFile = new int[col*10];
                    int[] spFile = new int[col*10];
                    int[] idFile = new int[col*10];

                    ds = Gdal.Open(oriFileName, Access.GA_ReadOnly);
                    Band demband = ds.GetRasterBand(1);//读取波段
                    demband.ReadRaster(0, rowBig, col, 10, oriFile, col, 10, 0, 0);//读取数据
                    ds.Dispose();

                    ds = Gdal.Open(spFileName, Access.GA_ReadOnly);
                    demband = ds.GetRasterBand(1);//读取波段
                    demband.ReadRaster(0, rowBig, col, 10, spFile, col, 10, 0, 0);//读取数据
                    ds.Dispose();

                    ds = Gdal.Open(idFileName, Access.GA_ReadOnly);
                    demband = ds.GetRasterBand(1);//读取波段
                    demband.ReadRaster(0, rowBig, col, 10, idFile, col, 10, 0, 0);//读取数据
                    ds.Dispose();
                    for(int i=0;i<10;i++)
                    {
                        for (int j = 0; j < col; j++)
                        {//保存
                            oriFiles[i, j, time] = oriFile[i * col + j];
                            spFiles[i, j, time] = spFile[i * col + j];
                            idFiles[i, j, time] = idFile[i * col + j];
                        }
                    }
                }
                */
                //DateTime startT2 = DateTime.Now;
                for (int rowNow = 0; rowNow < rowPart; rowNow++)
                {
                    for (int colNow = 0; colNow < col; colNow++)
                    {//逐列
                        double log = startLog + (colNow + 0.5) * resolution;//经度
                        double lat = endLat - (rowBig + rowNow + 0.5) * resolution;//纬度
                        string spaceName = getName(lat, log);//行政区划
                        if ((rowNow + rowBig) == 0 || colNow == 0 || (rowNow + rowBig) >= row - 1 || colNow >= col - 1) continue;//边界点，不搜索
                        bool isStormNow = false;//记录当前暴雨状态
                        int startTime = 0;//暴雨开始时间
                        int endTime = 0;//暴雨结束时间
                        double jiangYuLiang = 0.0;//总降雨量
                        double maxJiangYuLiang = 0.0;//最大降雨量
                        int baoYuCount1 = 0;//暴雨个数
                        int baoYuCount2 = 0;//大暴雨个数
                        int baoYuCount3 = 0;//特大暴雨个数
                        for (int time = 0; time < mFileNum; time++)
                        {//按时间顺序处理
                            if (!isStormNow)
                            {//当前不是暴雨，寻找暴雨开始点
                                if (spFiles[rowNow, colNow, time] > 0)
                                {//暴雨点
                                    startTime = time;
                                    isStormNow = true;
                                    jiangYuLiang = oriFiles[rowNow, colNow, time];
                                    maxJiangYuLiang = oriFiles[rowNow, colNow, time];
                                    baoYuCount1 = 0;//暴雨个数
                                    baoYuCount2 = 0;//大暴雨个数
                                    baoYuCount3 = 0;//特大暴雨个数
                                    if (spFiles[rowNow, colNow, time] == 1)
                                    {
                                        baoYuCount1++;
                                    }
                                    else if (spFiles[rowNow, colNow, time] == 2)
                                    {
                                        baoYuCount2++;
                                    }
                                    else if (spFiles[rowNow, colNow, time] == 3)
                                    {
                                        baoYuCount3++;
                                    }
                                }
                            }
                            else
                            {//当前是暴雨
                                if (spFiles[rowNow, colNow, time] <= 0 || time == (mFileNum - 1))
                                {//非暴雨点
                                    isStormNow = false;
                                    if (spFiles[rowNow, colNow, time] <= 0)
                                    {
                                        endTime = time - 1;
                                    }
                                    else
                                    {
                                        endTime = time;
                                    }
                                    double durTime = (endTime + 1 - startTime) * timeCell;//持续时间
                                    int stormId = idFiles[rowNow, colNow, endTime];//暴雨id
                                    jiangYuLiang = jiangYuLiang * mScale * valueScale;//乘以0.5
                                    double avgJiangYuLiang = Math.Round(jiangYuLiang / ((endTime + 1 - startTime) * timeCell), 4);//平均降雨量，注意原始数据是半小时的
                                    maxJiangYuLiang = maxJiangYuLiang * mScale* valueScale / timeCell;
                                    int stormMode = 1;
                                    string stormModeString = "暴雨";
                                    if ((baoYuCount2 + baoYuCount3) >= baoYuCount1)
                                    {
                                        if (baoYuCount3 >= baoYuCount2)
                                        {//特大暴雨
                                            stormMode = 3;
                                            stormModeString = "特大暴雨";
                                        }
                                        else
                                        {//大暴雨
                                            stormMode = 2;
                                            stormModeString = "大暴雨";
                                        }
                                    }
                                    string startTimeFormat = Path.GetFileName(oriFileNames[startTime].ToString()).Substring(0, 8) + "_" + Path.GetFileName(oriFileNames[startTime].ToString()).Substring(10, 6);
                                    string endTimeFormat = Path.GetFileName(oriFileNames[endTime].ToString()).Substring(0, 8) + "_" + Path.GetFileName(oriFileNames[endTime].ToString()).Substring(18, 6);
                                    //存储
                                    //conn.Open();
                                    //OID 经度 纬度 事件ID 事件名称 类型 起始时间 终止时间 持续时间 平均降雨量 累计降雨量 瞬时最大降雨强度 降雨强度
                                    oid++;
                                    string sql = string.Format("insert into "+ tableName + " values ('{0}','{1}','{2}','{3}','{4}','{5}',TO_TIMESTAMP_TZ('{6}','YYYYMMDD_HH24MISS TZH:TZM '),TO_TIMESTAMP_TZ('{7}','YYYYMMDD_HH24MISS TZH:TZM'),'{8}','{9}','{10}','{11}','{12}','{13}')", oid.ToString(), log.ToString(), lat.ToString(), stormId.ToString(), "暴雨事件", stormModeString, startTimeFormat+" "+timeZone, endTimeFormat + " " + timeZone, durTime.ToString(), avgJiangYuLiang.ToString(), jiangYuLiang.ToString(), maxJiangYuLiang.ToString(), stormMode.ToString(), spaceName);
                                    OracleCommand inserCmd = new OracleCommand(sql, conn);
                                    inserCmd.ExecuteNonQuery();
                                    //conn.Close();


                                    //sw.WriteLine(oid.ToString() + " " + log.ToString() + " " + lat.ToString() + " " + stormId.ToString() + " 暴雨事件 " + stormModeString + " " + startTimeFormat + " " + endTimeFormat + " " + durTime.ToString() + " " + avgJiangYuLiang.ToString() + " " + jiangYuLiang.ToString() + " " + maxJiangYuLiang.ToString() + " " + stormMode.ToString());
                                    
                                }
                                else
                                {//暴雨点
                                    jiangYuLiang += oriFiles[rowNow, colNow, time];//累积降雨量
                                    if (maxJiangYuLiang < oriFiles[rowNow, colNow, time])
                                    {//判断是否降雨量更大
                                        maxJiangYuLiang = oriFiles[rowNow, colNow, time];
                                    }
                                    if (spFiles[rowNow, colNow, time] == 1)
                                    {
                                        baoYuCount1++;
                                    }
                                    else if (spFiles[rowNow, colNow, time] == 2)
                                    {
                                        baoYuCount2++;
                                    }
                                    else if (spFiles[rowNow, colNow, time] == 3)
                                    {
                                        baoYuCount3++;
                                    }
                                }
                            }
                        }

                    }
                }
                //t2=t2.Add(DateTime.Now - startT2);
            }
            //DateTime endT = DateTime.Now;
            //TimeSpan et = endT - startT;
            //MessageBox.Show(et.TotalSeconds.ToString()+"\nt1:"+t1.TotalSeconds.ToString() + "\nt2:" + t2.TotalSeconds.ToString());
            //sw.Close();
            //fs.Close();
            //progressBar1.Hide();
            //MessageBox.Show("处理完成！");
        }

        static string getName(double Lat, double Lon)
        {
            string serviceAddress = "http://gc.ditu.aliyun.com/regeocoding?l=" + Lat.ToString() + "," + Lon.ToString() + "&type=010";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            //解析josn
            Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(retString);
            string Result = jobject["addrList"][0]["admName"].ToString();
            if (Result != "")
            {
                return (Result.Substring(0, Result.Length - 1));
            }
            else
            {
                return ("不属于中国范围");
            }


        }

        private void CompltetWork(object sender, RunWorkerCompletedEventArgs e)
        {//工作完成方法
            if (e.Cancelled)
            {//用户取消
                MessageBox.Show("处理取消！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else if(e.Error!=null)
            {//异常结束
                MessageBox.Show(e.Error.Message,"错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            else
            {
                progressBar1.Value = 100;
                MessageBox.Show("处理完成！","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            progressBar1.Hide();
            progressBar1.Value = 0;
        }

        private void ProgessChanged(object sender, ProgressChangedEventArgs e)
        {//进度改变方法
            //toolStripStatusLabel1.Text = "正在处理，已完成" + e.ProgressPercentage.ToString() + '%';
            progressBar1.Value = e.ProgressPercentage;
        }

        //异步委托
        public delegate int[,,] GetFileMC(ListBox.ObjectCollection filesName, int mFileNum, int rowNow, int col, int partRow);//定一个代理
        public int[,,] GetFile(ListBox.ObjectCollection filesName, int mFileNum, int rowNow, int col, int partRow)
        {
            int[,,] files = new int[partRow, col, mFileNum];
            for (int time = 0; time < mFileNum; time++)
            {//读取该行所有数据
                string fileName = filesName[time].ToString();
                int[] file = new int[col * partRow];
                Dataset ds = Gdal.Open(fileName, Access.GA_ReadOnly);
                Band demband = ds.GetRasterBand(1);//读取波段
                demband.ReadRaster(0, rowNow, col, partRow, file, col, partRow, 0, 0);//读取数据
                ds.Dispose();
                for (int i = 0; i < partRow; i++)
                {
                    for (int j = 0; j < col; j++)
                    {//保存
                        files[i, j, time] = file[i * col + j];
                    }
                }
            }
            return files;
        }


        ListBox.ObjectCollection oriFileNames;//所有文件名
        ListBox.ObjectCollection spFileNames;//所有文件名
        ListBox.ObjectCollection idFileNames;//所有文件名
        int mFileNum = 0;
        double valueScale;//值的比例系数
        double timeCell;//每条记录时间尺度，小时单位
        double maxTimeInv;//最大连续时间距离
        string tableName = "";
        string timeZone = "";
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }

            if (!(listBox3.Items.Count == listBox1.Items.Count && listBox1.Items.Count == listBox2.Items.Count))
            {//三个个数不完全相等
                MessageBox.Show("文件个数不相等！");
                return;
            }
            mFileNum = listBox3.Items.Count;//文件个数
            if (mFileNum == 0)
            {
                MessageBox.Show("请添加处理文件！");
                return;
            }

            oriFileNames = listBox3.Items;
            spFileNames = listBox1.Items;
            idFileNames = listBox2.Items;

            timeZone = timeZoneCB.Text.Trim();

            valueScale = Convert.ToDouble(valueScaleTextBox.Text.Trim());//值的比例系数
            timeCell = Convert.ToDouble(timeCellTextBox.Text.Trim());//每条记录时间尺度，小时单位
            maxTimeInv = Convert.ToDouble(maxTimeIntervalTextBox.Text.Trim());//最大连续时间距离
            tableName = tableNameComboBox.Text.Trim();

            progressBar1.Show();
            worker.RunWorkerAsync();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            //this.Close();
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

        /// <summary>
        /// 解决除不尽的情况
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        /// <returns>结果</returns>
        public static double DoubleRvide(double a, double b)
        {
            double temp = System.Math.IEEERemainder(a, b);
            if (Math.Abs(temp) < 2E-10)
            {
                return Math.Round(a / b);
            }
            else
            {
                return (a / b);
            }
        }

        private void oriAddFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox3.Items.Add(fileName);
                }
                oriCountTextBox.Text = listBox3.Items.Count.ToString();
            }
        }

        private void oriDeleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox3.SelectedIndex;
            while (index > -1)
            {
                listBox3.Items.RemoveAt(index);
                index = listBox3.SelectedIndex;
            }
            oriCountTextBox.Text = listBox3.Items.Count.ToString();
        }

        private void oriMoveUpBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox3.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (startIndex > 0 && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox3.Items.Insert(endIndex + 1, listBox3.Items[startIndex - 1].ToString());
                    listBox3.Items.RemoveAt(startIndex - 1);
                    //selectedIndices = listBox1.SelectedIndices;
                }
            }
        }

        private void oriMoveDownBtn_Click(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection selectedIndices = listBox3.SelectedIndices;
            if (selectedIndices.Count > 0)
            {
                int startIndex = selectedIndices[0];
                int endIndex = selectedIndices[selectedIndices.Count - 1];
                if (endIndex < (listBox3.Items.Count - 1) && endIndex - startIndex + 1 == selectedIndices.Count)
                {
                    listBox3.Items.Insert(startIndex, listBox3.Items[endIndex + 1].ToString());
                    listBox3.Items.RemoveAt(endIndex + 2);
                    selectedIndices = listBox3.SelectedIndices;
                }
            }
        }

        private void addFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
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

        private void idAddFileBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "图像文件|*.hdf;*.tif|hdf文件|*.hdf|tif文件|*.tif|所有文件|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            ofd.Multiselect = true;
            string[] filesNames;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filesNames = ofd.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBox2.Items.Add(fileName);
                }
                idCountTextBox.Text = listBox2.Items.Count.ToString();
            }
        }

        private void idDeleteFileBtn_Click(object sender, EventArgs e)
        {
            int index = listBox2.SelectedIndex;
            while (index > -1)
            {
                listBox2.Items.RemoveAt(index);
                index = listBox2.SelectedIndex;
            }
            idCountTextBox.Text = listBox2.Items.Count.ToString();
        }

        private void idMoveUpBtn_Click(object sender, EventArgs e)
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

        private void idMoveDownBtn_Click(object sender, EventArgs e)
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
    }
}
