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
    public partial class FormDDBSC : Form
    {
        string DataSetName = "";
        double radius = 0.3;//属性相似性阈值
        double minNms = 12;//核心点阈值
        List<DataRaster> RasterSets = new List<DataRaster>();
        public FormDDBSC()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.comboBoxNeighborhood.Items.Add(4);
            this.comboBoxNeighborhood.Items.Add(8);
            this.comboBoxNeighborhood.Items.Add(24);
            this.comboBoxNeighborhood.SelectedItem = 24;
            textBoxPropertySimilarity.Text = "0.3";
            textBoxCore.Text = "12";
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

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListBox.ObjectCollection inFileNames;
            inFileNames = listBoxFileList.Items;
            if (inFileNames.Count == 0)
            {
                MessageBox.Show("请添加处理文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                double.Parse(textBoxPropertySimilarity.Text);
                double.Parse(textBoxCore.Text);
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
            if(double.Parse(comboBoxNeighborhood.Text)<double.Parse(textBoxCore.Text))
            {
                MessageBox.Show("核心点阈值不能大于邻域阈值，请检查！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(textBoxFilePath.Text =="")
            {
                MessageBox.Show("输出文件不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] hdfFileName = new string[inFileNames.Count];//文件路径
            for (int i = 0; i < inFileNames.Count; i++)
            {
                hdfFileName[i] = inFileNames[i].ToString();//文件路径
            }
            string[] hdfFileNameNoPath = new string[inFileNames.Count];//获取文件名，不带路径
            for (int i = 0; i < inFileNames.Count; i++)
            {
                hdfFileNameNoPath[i] = hdfFileName[i].Substring(hdfFileName[i].LastIndexOf("\\") + 1);//获取文件名，不带路径
            }
            string[] hdfFileNameNoPathNohdf = new string[inFileNames.Count];//获取文件名，不带路径，不带后缀
            for (int i = 0; i < inFileNames.Count; i++)
            {
                hdfFileNameNoPathNohdf[i] = hdfFileNameNoPath[i].Substring(0, hdfFileNameNoPath[i].LastIndexOf("."));//获取文件名，不带路径，不带后缀
            }
            double MaxIgnoredValue = 0;//最大属性忽略值
            double MinIgnoredValue = 1;//最小属性忽略值
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
            double[] psBuffer = new double[row * col];//文件数据
            List<double> dataBuffer = new List<double>();
            double MaxValue = 0;//最大值
            double MinValue = 100;//最小值
            double MeanValue = 0;//平均值
            double StdValue = 0;//方差
            if (textBoxPropertySimilarity.Text.ToString() != "")//属性相似性阈值
                radius = double.Parse(textBoxPropertySimilarity.Text.ToString());
            if (textBoxCore.Text.ToString() != "")//核心点阈值
                minNms = double.Parse(textBoxCore.Text.ToString());
            int Neighborhood = int.Parse(comboBoxNeighborhood.SelectedItem.ToString());
            for (int nn = 0; nn < inFileNames.Count; nn++)
            {
                pBuffer = new double[row * col];
                RasterSets = new List<DataRaster>();
                pBuffer = HDF4Operator.GetDatasetsData(hdfFileName[nn]);
                //将读取的数据写入缓存，全部都写入，包括无效值
                for (int j = 0; j < row * col; j++)
                {
                    DataRaster pbu = new DataRaster();
                    psBuffer[j] = pBuffer[j] * scale;
                    //初始化栅格
                    pbu.attribute = psBuffer[j];
                    pbu.x = j / col;
                    pbu.y = j % col;
                    pbu.SetDrId(j);
                    pbu.SetVisited(false);
                    pbu.SetClusterId(-1);
                    RasterSets.Add(pbu);
                }
                double distance = 0.0;
                double[] w = new double[Neighborhood];//存储8-邻域的属性值,24邻域
                int[] n = new int[Neighborhood];//存储8-邻域的ID号，24邻域，将“空间半径”增大

                //求每个栅格的8-邻域内属性邻域栅格数
                for (int j = 0; j < row * col; j++)
                {
                    if ((RasterSets[j].attribute == fillValue * scale) || (RasterSets[j].attribute >= MinIgnoredValue && RasterSets[j].attribute <= MaxIgnoredValue))
                    {
                        RasterSets[j].SetVisited(true);
                        RasterSets[j].SetKey(false);
                        RasterSets[j].SetClusterId(0);
                    }
                    else if (RasterSets[j].x == 0 || RasterSets[j].x == 1 || RasterSets[j].x == row - 1 || RasterSets[j].x == row - 2 || RasterSets[j].y == 0 || RasterSets[j].y == 1 || RasterSets[j].y == col - 2 || RasterSets[j].y == col - 1)
                    {
                        RasterSets[j].SetKey(false);
                    }

                    else
                    {
                        if (Neighborhood == 4)
                        {
                            w[0] = RasterSets[j - col].attribute; n[0] = RasterSets[j - col].drID;
                            w[1] = RasterSets[j + 1].attribute; n[1] = RasterSets[j + 1].drID;
                            w[2] = RasterSets[j + col].attribute; n[2] = RasterSets[j + col].drID;
                            w[3] = RasterSets[j - 1].attribute; n[3] = RasterSets[j - 1].drID;
                        }
                        else if (Neighborhood == 8)
                        {
                            w[0] = RasterSets[j - col].attribute; n[0] = RasterSets[j - col].drID;
                            w[1] = RasterSets[j - col - 1].attribute; n[1] = RasterSets[j - col - 1].drID;
                            w[2] = RasterSets[j + 1].attribute; n[2] = RasterSets[j + 1].drID;
                            w[3] = RasterSets[j - col + 1].attribute; n[3] = RasterSets[j - col + 1].drID;
                            w[4] = RasterSets[j + col].attribute; n[4] = RasterSets[j + col].drID;
                            w[5] = RasterSets[j + col - 1].attribute; n[5] = RasterSets[j + col - 1].drID;
                            w[6] = RasterSets[j - 1].attribute; n[6] = RasterSets[j - 1].drID;
                            w[7] = RasterSets[j + col + 1].attribute; n[7] = RasterSets[j + col + 1].drID;
                        }
                        else
                        {
                            w[0] = RasterSets[j - col].attribute; n[0] = RasterSets[j - col].drID;
                            w[1] = RasterSets[j - col - 1].attribute; n[1] = RasterSets[j - col - 1].drID;
                            w[2] = RasterSets[j + 1].attribute; n[2] = RasterSets[j + 1].drID;
                            w[3] = RasterSets[j - col + 1].attribute; n[3] = RasterSets[j - col + 1].drID;
                            w[4] = RasterSets[j + col].attribute; n[4] = RasterSets[j + col].drID;
                            w[5] = RasterSets[j + col - 1].attribute; n[5] = RasterSets[j + col - 1].drID;
                            w[6] = RasterSets[j - 1].attribute; n[6] = RasterSets[j - 1].drID;
                            w[7] = RasterSets[j + col + 1].attribute; n[7] = RasterSets[j + col + 1].drID;
                            w[8] = RasterSets[j - 2 * col].attribute; n[8] = RasterSets[j - 2 * col].drID;
                            w[9] = RasterSets[j - 2 * col - 1].attribute; n[9] = RasterSets[j - 2 * col - 1].drID;
                            w[10] = RasterSets[j - 2 * col - 2].attribute; n[10] = RasterSets[j - 2 * col - 2].drID;
                            w[11] = RasterSets[j - 2 * col + 1].attribute; n[11] = RasterSets[j - 2 * col + 1].drID;
                            w[12] = RasterSets[j - 2 * col + 2].attribute; n[12] = RasterSets[j - 2 * col + 2].drID;
                            w[13] = RasterSets[j - col - 2].attribute; n[13] = RasterSets[j - col - 2].drID;
                            w[14] = RasterSets[j - col + 2].attribute; n[14] = RasterSets[j - col + 2].drID;
                            w[15] = RasterSets[j - 2].attribute; n[15] = RasterSets[j - 2].drID;
                            w[16] = RasterSets[j + 2].attribute; n[16] = RasterSets[j + 2].drID;
                            w[17] = RasterSets[j + col - 2].attribute; n[17] = RasterSets[j + col - 2].drID;
                            w[18] = RasterSets[j + col + 2].attribute; n[18] = RasterSets[j + col + 2].drID;
                            w[19] = RasterSets[j + 2 * col - 2].attribute; n[19] = RasterSets[j + 2 * col - 2].drID;
                            w[20] = RasterSets[j + 2 * col - 1].attribute; n[20] = RasterSets[j + 2 * col - 1].drID;
                            w[21] = RasterSets[j + 2 * col].attribute; n[21] = RasterSets[j + 2 * col].drID;
                            w[22] = RasterSets[j + 2 * col + 1].attribute; n[22] = RasterSets[j + 2 * col + 1].drID;
                            w[23] = RasterSets[j + 2 * col + 2].attribute; n[23] = RasterSets[j + 2 * col + 2].drID;
                        }
                        for (int k = 0; k < Neighborhood; k++)
                        {
                            if (w[k] == fillValue * scale || (w[k] >= MinIgnoredValue && w[k] <= MaxIgnoredValue))
                                continue;
                            else
                            {
                                distance = Math.Abs(w[k] - RasterSets[j].attribute);

                                if (distance <= radius)
                                {
                                    RasterSets[j].GetArrivalgrids().Add(n[k]);
                                }
                            }
                        }

                        if (RasterSets[j].GetArrivalgrids().Count() >= minNms)
                        {
                            RasterSets[j].SetKey(true);
                        }
                        else
                        {
                            RasterSets[j].SetKey(false);
                        }

                    }
                }
                /* 对栅格影像进行聚类操作 */
                int clusterId = 1;
                //long m[20000];//不能在方括号中用变量来表示元素的个数
                int[] m = new int[row * col];
                for (int j = 0; j < row * col; j++)
                {
                    //m[j]=0.0;
                    if (!RasterSets[j].isVisited() && RasterSets[j].IsKey())
                    {
                        RasterSets[j].SetClusterId(clusterId);
                        RasterSets[j].SetVisited(true);
                        KeyRasterCluster(j, clusterId); //对该对象领域内点进行聚类
                        clusterId++;
                    }
                }
                for (int j = 0; j < row * col; j++)
                {
                    //if(RasterSets[j].attribute >=-0.7 && RasterSets[j].attribute <=0.5)
                    //RasterSets[j].clusterId =-1;
                    m[j] = RasterSets[j].clusterId;

                }

                double sum = 0;
                //获取最大值、最小值
                for (int i = 0; i < row * col; i++)
                {
                    if (m[i] > MaxValue)
                        MaxValue = m[i];
                    if (m[i] < MinValue)
                        MinValue = m[i];
                    sum += m[i];
                }
                //获取平均值
                MeanValue = sum / (row * col);
                sum = 0;
                for (int i = 0; i < row * col; i++)
                {
                    sum += ((double)m[i] - MeanValue) * ((double)m[i] - MeanValue);
                }
                //获取方差
                StdValue = sum / (row * col);
                HDF4Operator.WriteCustomHDF2DFile(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf[nn] + "_cluster.hdf", ImageDate, dataType, "0", DataSetName, m, scale, Offsets, startLog, endLog, startLat, endLat, row, col, MaxValue, MinValue, MeanValue, StdValue, (int)fillValue, DSResolution, "2");
            }
            MessageBox.Show("完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        void KeyRasterCluster(int drID, int clusterId)
        {
            DataRaster srcDr = RasterSets[drID]; //获取数据点对象
            if (!srcDr.IsKey()) return;
            List <int> arrvalrasters = srcDr.GetArrivalgrids(); //获取对象领域内点ID列表
            for (int i = 0; i < arrvalrasters.Count(); i++)
            {

                DataRaster desDr = RasterSets[arrvalrasters[i]]; //获取领域内点数据点

                //DataRaster *desDr =new DataRaster;
                //*desDr=RasterSets[arrvalrasters[i]];
                if (!desDr.isVisited()) //若该对象没有被访问过执行
                {

                    desDr.SetClusterId(clusterId); //设置该对象所属簇的ID为clusterId，即将该对象吸入簇中
                    desDr.SetVisited(true); //设置该对象已被访问
                    if (desDr.IsKey()) //若该对象是核心对象
                    {
                        KeyRasterCluster(desDr.GetDrId(), clusterId); //递归地对该领域点数据的领域内的点执行聚类操作，采用深度优先方法
                    }

                }
                //delete desDr;
            }

        }

        private void textBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxPropertySimilarity_KeyPress(object sender, KeyPressEventArgs e)
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
