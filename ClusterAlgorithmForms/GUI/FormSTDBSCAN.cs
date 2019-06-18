using Forms.ProcessHDFByGdal;
using OSGeo.GDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forms.GUI
{
    public partial class FormSTDASCAN : Form
    {
        string DataSetName = "";
        List<RoSTCM> Rasterpixels = new List<RoSTCM>();
        List<Dual> duSets = new List<Dual>();
        public FormSTDASCAN()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBoxCore.Text = "5";
            textBoxPropertyNear.Text = "0.3";
            textBoxTime.Text = "2";
            textBoxNeighborhood.Text = "2.5";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListBox.ObjectCollection inFileNames;
            inFileNames = listBoxFileList.Items;
            int FileNum = inFileNames.Count;
            if (inFileNames.Count == 0)
            {
                MessageBox.Show("请添加处理文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int.Parse(textBoxCore.Text);
                double.Parse(textBoxPropertyNear.Text);
                int.Parse(textBoxTime.Text);
                double.Parse(textBoxNeighborhood.Text);
                if (textBoxMax.Text != "" || textBoxMin.Text != "")
                {
                    double.Parse(textBoxMax.Text);
                    double.Parse(textBoxMin.Text);
                }
            }
            catch
            {
                MessageBox.Show("数字格式错误，请检查！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxFilePath.Text == "")
            {
                MessageBox.Show("输出文件不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] hdfFileName = new string[inFileNames.Count];
            string[] hdfFileNameNoPath = new string[inFileNames.Count];
            string[] hdfFileNameNoPathNohdf = new string[inFileNames.Count];
            for (int i = 0; i < inFileNames.Count; i++)
            {
                hdfFileName[i] = inFileNames[i].ToString();//文件路径
                hdfFileNameNoPath[i] = hdfFileName[i].Substring(hdfFileName[i].LastIndexOf("\\") + 1);//获取文件名，不带路径
                hdfFileNameNoPathNohdf[i] = hdfFileNameNoPath[i].Substring(0, hdfFileNameNoPath[i].LastIndexOf("."));//获取文件名，不带路径
            }

            double MaxIgnoredValue = 0;//最大属性忽略值
            double MinIgnoredValue = 0;//最小属性忽略值
            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")
            {
                MaxIgnoredValue = double.Parse(textBoxMax.Text.ToString());
                MinIgnoredValue = double.Parse(textBoxMin.Text.ToString());
            }
            int T = 2;//原则上，也应当设置时间窗口阈值，但由于海洋异常变化的特殊性
                      //一般连续变化5个月以上为一次异常事件（先验），因此时间间隔选为2
            int CP = 5;//核心点阈值
            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")
            {
                MaxIgnoredValue = double.Parse(textBoxMax.Text.ToString());
                MinIgnoredValue = double.Parse(textBoxMin.Text.ToString());
            }
            DataSetName = comboBoxDataset.SelectedItem.ToString() + "_cluster";
            string ImageDate = HDF4Operator.GetDatasetsImageDate(hdfFileName[0]);//数据时间
            int row = HDF4Operator.GetDatasetsRow(hdfFileName[0]);//行数
            int col = HDF4Operator.GetDatasetsCol(hdfFileName[0]);//列数
            double startLog = HDF4Operator.GetDatasetsStartLog(hdfFileName[0]);//起始经度
            double endLog = HDF4Operator.GetDatasetsEndLog(hdfFileName[0]);//结束经度
            double startLat = HDF4Operator.GetDatasetsStartLat(hdfFileName[0]);//起始纬度
            double endLat = HDF4Operator.GetDatasetsEndLat(hdfFileName[0]);//结束纬度
            double scale = HDF4Operator.GetDatasetsScale(hdfFileName[0]);//比例系数
            double fillValue = HDF4Operator.GetDatasetsFillValue(hdfFileName[0]);//缺省值
            double Offsets = HDF4Operator.GetDatasetsOffsets(hdfFileName[0]);//截距
            double DSResolution = HDF4Operator.GetDatasetsResolution(hdfFileName[0]);//分辨率
            string dataType = HDF4Operator.GetDatasetsDataType(hdfFileName[0]);//数据类型
            Driver driver = HDF4Operator.GetDatasetsDriver(hdfFileName[0]);//文件Driver
            double[] pBuffer = new double[row * col];//文件数据
            double[] psBuffer = new double[FileNum * row * col];//文件数据
            List<double> dataBuffer = new List<double>();
            double NNeighbor = 0.3;//属性阈值
            double eps2 = 0.3;//属性相似性阈值
            double eps1 = 0.5;//空间临近阈值
            eps2 = double.Parse(textBoxPropertyNear.Text);
            eps1 = double.Parse(textBoxNeighborhood.Text);
            T = int.Parse(textBoxTime.Text.ToString());
            CP = int.Parse(textBoxCore.Text.ToString());
            if (textBoxPropertyNear.Text.ToString() != "")
                NNeighbor = double.Parse(textBoxPropertyNear.Text.ToString());
            double MaxValue = 0;//最大值
            double MinValue = 100;//最小值
            double MeanValue = 0;//平均值
            double StdValue = 0;//方差
            Rasterpixels = new List<RoSTCM>();


            //读取数据
            for (int i = 0; i < FileNum; i++)
            {
                //读取一个月的数据
                pBuffer = new double[row * col];//文件数据
                pBuffer = HDF4Operator.GetDatasetsData(hdfFileName[i]);

                //初始化
                for (int j = 0; j < row * col; j++)
                {
                    psBuffer[j] = (double)pBuffer[j] * scale;
                    RoSTCM rsets = new RoSTCM();

                    rsets.Attribute = (float)psBuffer[j];
                    rsets.x = j / col;
                    rsets.y = j % col;
                    rsets.t = i;
                    rsets.SetrsId(j + i * row * col);
                    rsets.SetVisited(false);
                    rsets.SetKey(false);
                    rsets.SetrsClusterId(-1);
                    Rasterpixels.Add(rsets);

                }
            }
            for (int j = 0; j < FileNum * row * col; j++)//寻找核心点
            {
                if (Rasterpixels[j].Attribute == fillValue * scale)
                {
                    Rasterpixels[j].SetVisited(true);
                    Rasterpixels[j].SetKey(false);
                    Rasterpixels[j].SetrsClusterId(0);
                }
                else if (Rasterpixels[j].Attribute >= MinIgnoredValue && Rasterpixels[j].Attribute <= MaxIgnoredValue)
                {
                    Rasterpixels[j].SetVisited(true);
                    Rasterpixels[j].SetKey(false);
                    Rasterpixels[j].SetrsClusterId(1);
                }
                else if (Rasterpixels[j].x == 0 || Rasterpixels[j].x == row - 1 || Rasterpixels[j].y == 0 || Rasterpixels[j].y == col - 1)
                    Rasterpixels[j].SetKey(false);
                else if (Rasterpixels[j].t >= T && Rasterpixels[j].t <= FileNum - T - 1)
                {
                    for (int i = 0; i < FileNum * row * col; i++)
                    {
                        if (Rasterpixels[i].t == Rasterpixels[j].t - T || Rasterpixels[i].t == Rasterpixels[j].t + T || Rasterpixels[i].t == Rasterpixels[j].t)//满足时间阈值
                        {
                            if (Rasterpixels[i].Attribute == fillValue * scale || (Rasterpixels[i].Attribute <= MaxIgnoredValue && Rasterpixels[i].Attribute >= MinIgnoredValue))
                                continue;
                            double distance = (Rasterpixels[i].x - Rasterpixels[j].x) * (Rasterpixels[i].x - Rasterpixels[j].x) + (Rasterpixels[i].y - Rasterpixels[j].y) * (Rasterpixels[i].y - Rasterpixels[j].y);//计算空间距离
                            distance = Math.Sqrt(distance);
                            if (distance <= eps1 && Math.Abs(Rasterpixels[i].Attribute - Rasterpixels[j].Attribute) <= eps2)
                                Rasterpixels[j].Getneighborgrids().Add(Rasterpixels[i].rsID);
                        }
                        else
                            continue;
                    }

                    RoSTCM srcDr = Rasterpixels[j]; //获取数据点对象                                                                                                 																																													
                    List<int> arrvalrasters = srcDr.Getneighborgrids(); //获取对象领域内点ID列表 

                    //int c=0;int d=0;
                    for (int r = 0; r < arrvalrasters.Count(); r++)
                    {
                        RoSTCM desDr = Rasterpixels[arrvalrasters[r]]; //获取领域内点数据点    
                        List<int> arrval = desDr.Getneighborgrids(); //获取对象领域内点ID列表
                                                                     //if (desDr.rsID==Rasterpixels[j].rsID-mRows*mCols)//正上方和正下方的邻域也是其最近邻
                                                                     //{
                                                                     //	c=1;
                                                                     //}
                                                                     //if (desDr.rsID==Rasterpixels[j].rsID+mRows*mCols)
                                                                     //{
                                                                     //	d=1;
                                                                     //}
                    }

                    if (Rasterpixels[j].Getneighborgrids().Count() >= CP)
                        Rasterpixels[j].SetKey(true);
                    else
                        Rasterpixels[j].SetKey(false);

                }

            }


            //开始聚类
            int clusID = 2;
            //int m=1;
            for (int j = 0; j < FileNum * row * col; j++)
            {

                if (!Rasterpixels[j].isVisited() && Rasterpixels[j].IsKey())
                {
                    Rasterpixels[j].SetVisited(true);
                    Rasterpixels[j].SetrsClusterId(clusID);
                    ergodic2(j, clusID);
                    clusID++;
                }

            }


            int[] outt = new int[row * col];
            for (int i = 0; i < FileNum; i++)
            {
                for (int j = i * row * col; j < (i + 1) * row * col; j++)
                {
                    outt[j - i * row * col] = Rasterpixels[j].rsclusterId;//从大数组中逐个输出文件
                }
                double sum = 0;
                //获取最大值、最小值
                for (int j = 0; j < row * col; j++)
                {
                    if (outt[j] > MaxValue)
                        MaxValue = outt[j];
                    if (outt[j] < MinValue)
                        MinValue = outt[i];
                    sum += outt[j];
                }
                //获取平均值
                MeanValue = sum / (row * col);
                sum = 0;
                for (int j = 0; j < row * col; j++)
                {
                    sum += ((double)outt[j] - MeanValue) * ((double)outt[j] - MeanValue);
                }
                //获取方差
                StdValue = sum / (row * col);
                //写hdf文件
                HDF4Operator.WriteCustomHDF2DFile(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf[i] + "_Tcluster.hdf", ImageDate, dataType, "0", DataSetName, outt, scale, Offsets, startLog, endLog, startLat, endLat, row, col, MaxValue, MinValue, MeanValue, StdValue, (int)fillValue, DSResolution, "2");

            }
            MessageBox.Show("完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "d:\\";
            openFileDialog.Filter = "hdf文件|*.hdf|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;
            string[] filesNames;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filesNames = openFileDialog.FileNames;
                foreach (string fileName in filesNames)
                {
                    listBoxFileList.Items.Add(fileName);
                }
                textBoxNum.Text = listBoxFileList.Items.Count.ToString();
            }
            //获取数据集名称
            ListBox.ObjectCollection inFileNames;
            inFileNames = listBoxFileList.Items;
            comboBoxDataset.Items.Clear();
            for (int i = 0; i < inFileNames.Count; i++)
            {
                string hdfFileName = inFileNames[i].ToString();//文件路径
                string hdfFileNameNoPath = hdfFileName.Substring(hdfFileName.LastIndexOf("\\") + 1);//获取文件名，不带路径
                string hdfFileNameNoPathNohdf = hdfFileNameNoPath.Substring(0, hdfFileNameNoPath.LastIndexOf("."));//获取文件名，不带路径
                DataSetName = HDF4Operator.GetDatasetsName(hdfFileName);
                comboBoxDataset.Items.Add(DataSetName);
                comboBoxDataset.SelectedItem = DataSetName;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            int index = listBoxFileList.SelectedIndex;
            while (index > -1)
            {
                listBoxFileList.Items.RemoveAt(index);
                index = listBoxFileList.SelectedIndex;
            }
            textBoxNum.Text = listBoxFileList.Items.Count.ToString();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //openFileDialog.InitialDirectory = "d:\\";
            saveFileDialog.Filter = "hdf文件|*.hdf|所有文件|*.*";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string SaveFileName = saveFileDialog.FileName;
                textBoxFilePath.Text = SaveFileName;
                textBoxFilePath.Text = SaveFileName.Substring(SaveFileName.LastIndexOf("\\") + 1);//获取文件名，不带路径
                textBoxFilePath.Text = SaveFileName.Substring(0, SaveFileName.LastIndexOf("\\"));//获取路径，不带文件名

            }
        }


        void ergodic2(int drID, int rsclusterId)
        {
            RoSTCM scr = Rasterpixels[drID];
            if (!scr.IsKey()) return;
            scr.SetrsClusterId(rsclusterId);
            scr.SetVisited(true);
            List<int> arrvals = scr.Getneighborgrids();//获取最近邻
            for (int i = 0; i < arrvals.Count(); i++)
            {
                RoSTCM des = Rasterpixels[arrvals[i]];
                des.SetrsClusterId(rsclusterId);
                des.SetVisited(true);
                if (des.isVisited() && des.IsKey() && des.a != 1)
                {
                    des.a = 1;//防止堆栈溢出
                    ergodic2(arrvals[i], rsclusterId);
                }

            }

        }
    }
}
