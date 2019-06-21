using ClusterAlgorithm;
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

namespace ClusterAlgorithm.GUI
{
    public partial class FormSTSNN : Form
    {
        string DataSetName = "";
        List<RoSTCM> Rasterpixels = new List<RoSTCM>();
        public FormSTSNN()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBoxShareNearest.Text = "4";
            textBoxCore.Text = "5";
            textBoxPropertyNear.Text = "0.3";
            textBoxTime.Text = "2";
            comboBoxNeighborhood.Items.Add("4");
            comboBoxNeighborhood.Items.Add("8");
            comboBoxNeighborhood.Items.Add("24");
            comboBoxNeighborhood.SelectedItem = "8";
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
                if (comboBoxDataset.Items.Count == 0)
                {
                    comboBoxDataset.Items.Add(DataSetName);
                    comboBoxDataset.SelectedItem = DataSetName;
                }
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
                double.Parse(textBoxShareNearest.Text);
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
                MaxIgnoredValue = int.Parse(textBoxMax.Text.ToString());
                MinIgnoredValue = int.Parse(textBoxMin.Text.ToString());
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
            int Neighborhood = 8;
            int SNNeighbor = 4;//共享最邻近阈值
            T = int.Parse(textBoxTime.Text.ToString());
            CP = int.Parse(textBoxCore.Text.ToString());
            NNeighbor = double.Parse(textBoxPropertyNear.Text.ToString());
            Neighborhood = int.Parse(comboBoxNeighborhood.Text);
            SNNeighbor = int.Parse(textBoxShareNearest.Text);
            double MaxValue = 0;//最大值
            double MinValue = 100;//最小值
            double MeanValue = 0;//平均值
            double StdValue = 0;//方差
            Rasterpixels = new List<RoSTCM>();


            for (int i = 0; i < FileNum; i++)
            {
                //读取一个月的数据
                pBuffer = new double[row * col];//文件数据
                pBuffer = HDF4Operator.GetDatasetsData(hdfFileName[i]);
                for (int j = 0; j < row * col; j++)
                {
                    psBuffer[j + i * row * col] = pBuffer[j] * scale;
                    RoSTCM rsets = new RoSTCM();
                    rsets.Attribute = (float)psBuffer[j + i * row * col];
                    rsets.x = j / col;
                    rsets.y = j % col;
                    rsets.t = i;
                    rsets.a = 0;
                    rsets.SetrsId(j + i * row * col);
                    rsets.SetVisited(false);
                    rsets.SetKey(false);
                    rsets.SetrsClusterId(-1);
                    Rasterpixels.Add(rsets);
                }
            }

            //时空邻域查询与时空核标记

            double[] TT = new double[2 * T];//时间邻居，即时间序列属性存放数组
            int[] TID = new int[2 * T];//时间邻居，即时间序列栅格ID存放数组
            double[] NN = new double[(2 * T + 1) * (Neighborhood + 1) - 1];//时空邻居属性存放数组
            int[] ID = new int[(2 * T + 1) * (Neighborhood + 1) - 1];//时空邻居ID存放数组

            for (int j = 0; j < FileNum * row * col; j++)
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

                else if (Rasterpixels[j].t >= T && Rasterpixels[j].t <= FileNum - T-1)//else if(Rasterpixels[j].t >=2 && Rasterpixels[j].t <=mFileNum-3 && Rasterpixels[j].t%2 ==0)
                {
                    ////时间邻域，时间窗口为2
                    for (int n = 0; n < T; n++)
                    {
                        if (Neighborhood == 24)
                        {
                            if (Rasterpixels[j].x == 1 || Rasterpixels[j].y == 1 || Rasterpixels[j].x == row - 2 || Rasterpixels[j].y == col - 2)
                                Rasterpixels[j].SetKey(false);
                            else
                            {
                                TID[n * 2] = Rasterpixels[j - (n + 1) * row * col].rsID;
                                TT[n * 2] = Rasterpixels[j - (n + 1) * row * col].Attribute;
                                TID[n * 2 + 1] = Rasterpixels[j + (n + 1) * row * col].rsID;
                                TT[n * 2 + 1] = Rasterpixels[j + (n + 1) * row * col].Attribute;
                            }
                        }
                        else
                        {
                            TID[n * 2] = Rasterpixels[j - (n + 1) * row * col].rsID;
                            TT[n * 2] = Rasterpixels[j - (n + 1) * row * col].Attribute;
                            TID[n * 2 + 1] = Rasterpixels[j + (n + 1) * row * col].rsID;
                            TT[n * 2 + 1] = Rasterpixels[j + (n + 1) * row * col].Attribute;
                        }

                    }
                    //时空邻域共有26个
                    for (int tt = 0; tt < T; tt++)
                    {
                        ID[tt] = TID[tt];
                        NN[tt] = TT[tt];
                    }
                    //空间邻域
                    if (Neighborhood == 4)//4邻域
                    {
                        ID[T] = Rasterpixels[j - 1].rsID;
                        ID[T + 1] = Rasterpixels[j + 1].rsID;
                        ID[T + 2] = Rasterpixels[j - col].rsID;
                        ID[T + 3] = Rasterpixels[j + col].rsID;
                        NN[T] = Rasterpixels[j - 1].Attribute;
                        NN[T + 1] = Rasterpixels[j + 1].Attribute;
                        NN[T + 2] = Rasterpixels[j - col].Attribute;
                        NN[T + 3] = Rasterpixels[j + col].Attribute;
                    }
                    else if (Neighborhood == 8)//8邻域
                    {
                        ID[T] = Rasterpixels[j - 1].rsID;
                        ID[T + 1] = Rasterpixels[j + 1].rsID;
                        ID[T + 2] = Rasterpixels[j - col].rsID;
                        ID[T + 3] = Rasterpixels[j - col - 1].rsID;
                        ID[T + 4] = Rasterpixels[j - col + 1].rsID;
                        ID[T + 5] = Rasterpixels[j + col].rsID;
                        ID[T + 6] = Rasterpixels[j + col - 1].rsID;
                        ID[T + 7] = Rasterpixels[j + col + 1].rsID;
                        NN[T] = Rasterpixels[j - 1].Attribute;
                        NN[T + 1] = Rasterpixels[j + 1].Attribute;
                        NN[T + 2] = Rasterpixels[j - col].Attribute;
                        NN[T + 3] = Rasterpixels[j - col - 1].Attribute;
                        NN[T + 4] = Rasterpixels[j - col + 1].Attribute;
                        NN[T + 5] = Rasterpixels[j + col].Attribute;
                        NN[T + 6] = Rasterpixels[j + col - 1].Attribute;
                        NN[T + 7] = Rasterpixels[j + col + 1].Attribute;
                    }
                    else//24邻域
                    {
                        if (Rasterpixels[j].x == 1 || Rasterpixels[j].y == 1 || Rasterpixels[j].x == row - 2 || Rasterpixels[j].y == col - 2)
                            Rasterpixels[j].SetKey(false);
                        else
                        {
                            ID[T] = Rasterpixels[j - 1].rsID;
                            ID[T + 1] = Rasterpixels[j + 1].rsID;
                            ID[T + 2] = Rasterpixels[j - col].rsID;
                            ID[T + 3] = Rasterpixels[j - col - 1].rsID;
                            ID[T + 4] = Rasterpixels[j - col + 1].rsID;
                            ID[T + 5] = Rasterpixels[j + col].rsID;
                            ID[T + 6] = Rasterpixels[j + col - 1].rsID;
                            ID[T + 7] = Rasterpixels[j + col + 1].rsID;
                            ID[T + 8] = Rasterpixels[j - 2].rsID;
                            ID[T + 9] = Rasterpixels[j + 2].rsID;
                            ID[T + 10] = Rasterpixels[j - 2 * col].rsID;
                            ID[T + 11] = Rasterpixels[j - 2 * col - 1].rsID;
                            ID[T + 12] = Rasterpixels[j - 2 * col + 1].rsID;
                            ID[T + 13] = Rasterpixels[j + 2 * col].rsID;
                            ID[T + 14] = Rasterpixels[j + 2 * col - 1].rsID;
                            ID[T + 15] = Rasterpixels[j + 2 * col + 1].rsID;
                            ID[T + 16] = Rasterpixels[j - col - 2].rsID;
                            ID[T + 17] = Rasterpixels[j + col - 2].rsID;
                            ID[T + 18] = Rasterpixels[j - col + 2].rsID;
                            ID[T + 19] = Rasterpixels[j - col + 2].rsID;
                            ID[T + 20] = Rasterpixels[j - 2 * col + 2].rsID;
                            ID[T + 21] = Rasterpixels[j - 2 * col - 2].rsID;
                            ID[T + 22] = Rasterpixels[j + 2 * col - 2].rsID;
                            ID[T + 23] = Rasterpixels[j + 2 * col + 2].rsID;


                            NN[T] = Rasterpixels[j - 1].Attribute;
                            NN[T + 1] = Rasterpixels[j + 1].Attribute;
                            NN[T + 2] = Rasterpixels[j - col].Attribute;
                            NN[T + 3] = Rasterpixels[j - col - 1].Attribute;
                            NN[T + 4] = Rasterpixels[j - col + 1].Attribute;
                            NN[T + 5] = Rasterpixels[j + col].Attribute;
                            NN[T + 6] = Rasterpixels[j + col - 1].Attribute;
                            NN[T + 7] = Rasterpixels[j + col + 1].Attribute;
                            NN[T + 8] = Rasterpixels[j - 2].Attribute;
                            NN[T + 9] = Rasterpixels[j + 2].Attribute;
                            NN[T + 10] = Rasterpixels[j - 2 * col].Attribute;
                            NN[T + 11] = Rasterpixels[j - 2 * col - 1].Attribute;
                            NN[T + 12] = Rasterpixels[j - 2 * col + 1].Attribute;
                            NN[T + 13] = Rasterpixels[j + 2 * col].Attribute;
                            NN[T + 14] = Rasterpixels[j + 2 * col - 1].Attribute;
                            NN[T + 15] = Rasterpixels[j + 2 * col + 1].Attribute;
                            NN[T + 16] = Rasterpixels[j - col - 2].Attribute;
                            NN[T + 17] = Rasterpixels[j + col - 2].Attribute;
                            NN[T + 18] = Rasterpixels[j - col + 2].Attribute;
                            NN[T + 19] = Rasterpixels[j - col + 2].Attribute;
                            NN[T + 20] = Rasterpixels[j - 2 * col + 2].Attribute;
                            NN[T + 21] = Rasterpixels[j - 2 * col - 2].Attribute;
                            NN[T + 22] = Rasterpixels[j + 2 * col - 2].Attribute;
                            NN[T + 23] = Rasterpixels[j + 2 * col + 2].Attribute;
                        }

                    }
                    //其他时空邻域
                    if (Neighborhood == 4)//四邻域
                    {
                        for (int tt = 0; tt < T * 2; tt++)
                        {
                            ID[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].rsID;
                            ID[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].rsID;
                            ID[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].rsID;
                            ID[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].rsID;

                            NN[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].Attribute;
                            NN[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].Attribute;
                            NN[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].Attribute;
                            NN[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].Attribute;
                        }
                        for (int tt = 0; tt < T; tt++)
                        {
                            ID[Neighborhood * (T * 2 + 1) + T + tt] = TID[T + tt];
                            NN[Neighborhood * (T * 2 + 1) + T + tt] = TT[T + tt];
                        }
                    }
                    else if (Neighborhood == 8)//8邻域
                    {
                        for (int tt = 0; tt < T * 2; tt++)
                        {
                            ID[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].rsID;
                            ID[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].rsID;
                            ID[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].rsID;
                            ID[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 1].rsID;
                            ID[T + 4 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 1].rsID;
                            ID[T + 5 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].rsID;
                            ID[T + 6 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 1].rsID;
                            ID[T + 7 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col + 1].rsID;

                            NN[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].Attribute;
                            NN[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].Attribute;
                            NN[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].Attribute;
                            NN[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 1].Attribute;
                            NN[T + 4 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 1].Attribute;
                            NN[T + 5 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].Attribute;
                            NN[T + 6 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 1].Attribute;
                            NN[T + 7 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col + 1].Attribute;
                        }
                        for (int tt = 0; tt < T; tt++)
                        {
                            ID[Neighborhood * (T * 2 + 1) + T + tt] = TID[T + tt];
                            NN[Neighborhood * (T * 2 + 1) + T + tt] = TT[T + tt];
                        }
                    }
                    else//24邻域
                    {
                        if (Rasterpixels[j].x == 1 || Rasterpixels[j].y == 1 || Rasterpixels[j].x == row - 2 || Rasterpixels[j].y == col - 2)
                            Rasterpixels[j].SetKey(false);
                        else
                        {
                            for (int tt = 0; tt < T * 2; tt++)
                            {
                                ID[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].rsID;
                                ID[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].rsID;
                                ID[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].rsID;
                                ID[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 1].rsID;
                                ID[T + 4 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 1].rsID;
                                ID[T + 5 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].rsID;
                                ID[T + 6 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 1].rsID;
                                ID[T + 7 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col + 1].rsID;
                                ID[T + 8 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2].rsID;
                                ID[T + 9 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2].rsID;
                                ID[T + 10 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col].rsID;
                                ID[T + 11 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col - 1].rsID;
                                ID[T + 12 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col + 1].rsID;
                                ID[T + 13 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col].rsID;
                                ID[T + 14 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col - 1].rsID;
                                ID[T + 15 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col + 1].rsID;
                                ID[T + 16 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 2].rsID;
                                ID[T + 17 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 2].rsID;
                                ID[T + 18 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 2].rsID;
                                ID[T + 19 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 2].rsID;
                                ID[T + 20 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col + 2].rsID;
                                ID[T + 21 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col - 2].rsID;
                                ID[T + 22 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col - 2].rsID;
                                ID[T + 23 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col + 2].rsID;


                                NN[T + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 1].Attribute;
                                NN[T + 1 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 1].Attribute;
                                NN[T + 2 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col].Attribute;
                                NN[T + 3 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 1].Attribute;
                                NN[T + 4 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 1].Attribute;
                                NN[T + 5 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col].Attribute;
                                NN[T + 6 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 1].Attribute;
                                NN[T + 7 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col + 1].Attribute;
                                NN[T + 8 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2].Attribute;
                                NN[T + 9 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2].Attribute;
                                NN[T + 10 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col].Attribute;
                                NN[T + 11 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col - 1].Attribute;
                                NN[T + 12 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col + 1].Attribute;
                                NN[T + 13 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col].Attribute;
                                NN[T + 14 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col - 1].Attribute;
                                NN[T + 15 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col + 1].Attribute;
                                NN[T + 16 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col - 2].Attribute;
                                NN[T + 17 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + col - 2].Attribute;
                                NN[T + 18 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 2].Attribute;
                                NN[T + 19 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - col + 2].Attribute;
                                NN[T + 20 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col + 2].Attribute;
                                NN[T + 21 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] - 2 * col - 2].Attribute;
                                NN[T + 22 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col - 2].Attribute;
                                NN[T + 23 + Neighborhood * (tt + 1)] = Rasterpixels[TID[tt] + 2 * col + 2].Attribute;

                            }
                            for (int tt = 0; tt < T; tt++)
                            {
                                ID[Neighborhood * (T * 2 + 1) + T + tt] = TID[T + tt];
                                NN[Neighborhood * (T * 2 + 1) + T + tt] = TT[T + tt];
                            }
                        }
                    }
                    for (int k = 0; k < (2 * T + 1) * (Neighborhood + 1) - 1; k++)
                    {

                        if (TT[0] == fillValue * scale || TT[1] == fillValue * scale || TID[0] == 1 || TID[1] == 1)
                            break;

                        else if ((NN[k] == fillValue * scale) || (NN[k] >= MinIgnoredValue && NN[k] <= MaxIgnoredValue))//NN[k]>=-0.9 && NN[k]<=0.8)
                            continue;
                        else
                        {
                            if (Math.Abs(NN[k] - Rasterpixels[j].Attribute) <= NNeighbor)
                                Rasterpixels[j].Getneighborgrids().Add(ID[k]);
                        }
                    }

                    /*if(Rasterpixels[j].Getneighborgrids().size()>= minms)
                    Rasterpixels[j].SetKey (true);
                    else 
                    Rasterpixels[j].SetKey (false);*/

                }

            }
            for (int o = 0; o < FileNum * row * col; o++)
            {
                RoSTCM srcDr = Rasterpixels[o]; //获取数据点对象                                                                                                 																																													
                List<int> arrvalrasters = srcDr.Getneighborgrids(); //获取对象领域内点ID列表 
                int b = 0;//与该点共享邻个数超过MinSimilar的点的个数
                          //
                int c = 0; int d = 0;
                for (int r = 0; r < arrvalrasters.Count(); r++)
                {

                    RoSTCM desDr = Rasterpixels[arrvalrasters[r]]; //获取领域内点数据点    
                    int a = 0;//与该点共享邻的个数
                    List<int> arrval = desDr.Getneighborgrids(); //获取对象领域内点ID列表
                    for (int j = 0; j < arrvalrasters.Count(); j++)
                    {
                        for (int l = 0; l < arrval.Count(); l++)
                        {
                            RoSTCM aaDr = Rasterpixels[arrval[l]]; //获取领域内点数据点
                            RoSTCM bbDr = Rasterpixels[arrvalrasters[j]];
                            if (aaDr.rsID == bbDr.rsID)
                            {
                                a = a + 1;
                            }
                        }
                    }
                    if (a >= SNNeighbor)//3.15为4
                    {
                        b = b + 1;
                    }
                    if (desDr.rsID == Rasterpixels[o].rsID - T * row * col)//判断上下邻域的点是否是该点的最近邻
                    {
                        c = 1;
                    }
                    if (desDr.rsID == Rasterpixels[o].rsID + T * row * col)
                    {
                        d = 1;
                    }
                }
                //改 30 原值为8  3.15为6
                if (b >= CP && c == 1 && d == 1)//判断核心点
                {
                    Rasterpixels[o].SetKey(true);
                }
                /*if (b >=6&&fabs(Rasterpixels[(Rasterpixels[o].rsID-T*row*col)].Attribute-Rasterpixels[o].Attribute)<=0.9&&fabs(Rasterpixels[(Rasterpixels[o].rsID+T*row*col)].Attribute-Rasterpixels[o].Attribute)<=0.9)
                {
                    Rasterpixels[o].SetKey(true);
                }*/
                else
                {
                    Rasterpixels[o].SetKey(false);
                }
            }



            //开始聚类

            int rsclusterId = 2;//簇编号

            //double clusteravg[100000];//各个簇的均值
            //int clusternum=0;//各个簇内元素的个数


            for (int j = 0; j < FileNum * row * col; j++)
            {

                if (!Rasterpixels[j].isVisited() && Rasterpixels[j].IsKey())
                {
                    Rasterpixels[j].SetrsClusterId(rsclusterId);
                    Rasterpixels[j].SetVisited(true);
                    Rasterpixels[j].a = 1;
                    ExpandCluster1(j, rsclusterId, row, col, Rasterpixels[j].Attribute, 1);
                    rsclusterId++;
                }
            }
            //删除小簇
            int minnum = 8 * FileNum * ((row * col) / 20000);//猜想：改为（8*((row*col)/20000)*mFileNum）更好！
            int rsclusterIdnum = rsclusterId - 2;
            for (int l = 2; l < rsclusterId; l++)
            {
                int idnum = 0;
                for (int m = 0; m < FileNum * row * col; m++)
                {
                    if (Rasterpixels[m].rsclusterId == l)
                    {
                        idnum = idnum + 1;
                    }
                }
                if (idnum <= minnum)
                {
                    for (int m = 0; m < FileNum * row * col; m++)
                    {
                        if (Rasterpixels[m].rsclusterId == l)
                        {
                            Rasterpixels[m].SetrsClusterId(1);
                        }
                    }
                    rsclusterIdnum = rsclusterIdnum - 1;
                }
            }



            /* 保存聚类结果，即输出的结果每个栅格值表示的是此栅格所属的聚类簇ID */

            //输出结果的形式固定，即保存为.hdf文件，现有方法是一个文件一个文件的输出


            //long out[5000];
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

        void ExpandCluster1(int drID, int rsclusterId, int row, int col, double avg, int num)
        {
            RoSTCM scr = Rasterpixels[drID];
            if (!scr.IsKey()) return;
            List<int> arrvals = scr.Getneighborgrids();//获取最近邻
            double r = 0;//新加入栅格数据属性值与原有簇属性均值的差
            for (int i = 0; i < arrvals.Count(); i++)
            {
                //判断时空相连要素是否加入簇中
                RoSTCM des = Rasterpixels[arrvals[i]];//获取第i个最近邻
                r = Math.Abs(des.Attribute - avg);
                if (!des.isVisited() && r <= 5.0)
                {
                    //开始分类
                    des.SetrsClusterId(rsclusterId);
                    des.SetVisited(true);
                    num = num + 1;
                    avg = (avg + des.Attribute) / num;//重新计算簇均值
                }
            }
            for (int i = 0; i < arrvals.Count(); i++)
            {
                RoSTCM des = Rasterpixels[arrvals[i]];
                //double s =0;
                if (des.isVisited() && des.IsKey() && des.a != 1 && Math.Abs((Rasterpixels[des.rsID + 2 * col * row].Attribute + Rasterpixels[des.rsID - 2 * col * row].Attribute) / 2 - (avg * num - des.Attribute) / (num - 1)) <= 2)
                {
                    des.a = 1;//a=1代表该点已经被聚类，防止出现死循环，堆栈溢出
                    ExpandCluster1(arrvals[i], rsclusterId, row, col, avg, num);//扩展该簇
                }
            }

        }

        private void textBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxTime_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxNeighborhood_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxPropertyNear_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxShareNearest_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxCore_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
