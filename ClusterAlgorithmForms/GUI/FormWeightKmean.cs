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

namespace ClusterAlgorithmForms.GUI
{
    public partial class FormWeightKmean : Form
    {
        int maxIteration = 200;//最大迭代次数
        string DataSetName = "";
        public FormWeightKmean()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBoxK.Text = "2";
            textBoxW1.Text = "0.5";
            textBoxW2.Text = "0.5";
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

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListBox.ObjectCollection inFileNames;
            inFileNames = listBoxFileList.Items;
            if (inFileNames.Count == 0)
            {
                MessageBox.Show("请添加处理文件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBoxK.Text == "")
            {
                MessageBox.Show("请输入K值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                double.Parse(textBoxW1.Text);
                double.Parse(textBoxW2.Text);
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
            double MinIgnoredValue = 0;//最小属性忽略值
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
            double[] data = new double[row * col];//文件数据
            int K = int.Parse(textBoxK.Text.ToString());//聚类数
            double w1 = double.Parse(textBoxW1.Text);//空间权重
            double w2 = double.Parse(textBoxW2.Text);//属性权重
            int[] resultIndex = new int[row * col];  //聚类结果
            double[] center = new double[K];     //聚类中心
            double[] centerCopy = new double[K]; //将旧的中心点保留以作比较 
            int[] centerIndex = new int[K];    //聚类中心索引 K * 1
            List<double> dataBuffer = new List<double>();
            double MaxValue = 0;//最大值
            double MinValue = 100;//最小值
            double MeanValue = 0;//平均值
            double StdValue = 0;//方差
            for (int n = 0; n < inFileNames.Count; n++)
            {
                data = new double[row * col];
                dataBuffer = new List<double>();
                data = HDF4Operator.GetDatasetsData(hdfFileName[n]);
                for (int i = 0; i < row * col; i++)
                {
                    data[i] = data[i] * scale;
                    if (DataSetName == "Precipitation_cluster")
                    {
                        if (data[i] != fillValue * scale && data[i] != 0)
                        {
                            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                            {
                                if (data[i] < MinIgnoredValue || data[i] > maxIteration)
                                    dataBuffer.Add(data[i]);
                            }
                            else
                                dataBuffer.Add(data[i]);
                        }
                    }
                    else
                    {
                        if (data[i] != fillValue * scale)
                        {
                            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                            {
                                if (data[i] < MinIgnoredValue || data[i] > maxIteration)
                                    dataBuffer.Add(data[i]);
                            }
                            else
                                dataBuffer.Add(data[i]);
                        }
                    }
                }
                //随机生成聚类中心
                for (int i = 0; i < K; i++)
                {
                    int j = 0;
                    Random random = new Random();
                    int a = random.Next(0, dataBuffer.Count - 1);
                    centerIndex[i] = a;
                    for (j = 0; j < i; j++)
                    {
                        if (dataBuffer[a] == dataBuffer[centerIndex[j]])
                        {
                            i--;
                            break;
                        }

                    }
                }
                //选取聚类中心
                for (int i = 0; i < K; i++)
                {
                    center[i] = dataBuffer[centerIndex[i]];
                    centerCopy[i] = dataBuffer[centerIndex[i]];

                }
                //k-mean聚类算法实现
                int curIteration = 0;    //当前迭代次数
                do
                {
                    //迭代更新聚类
                    double[][] distance = new double[row * col][];
                    for (int i = 0; i < row * col; i++)
                    {
                        if (data[i] == fillValue * scale)//如果为缺省值，赋值-1
                        {
                            resultIndex[i] = -1;
                        }
                        else if (data[i] == 0)
                        {
                            resultIndex[i] = 0;
                        }
                        else
                        {
                            distance[i] = new double[K];
                            for (int j = 0; j < K; j++)
                            {
                                distance[i][j] = 0.0;
                                distance[i][j] = w1 * Math.Abs(data[i] - center[j]) + w2 * Math.Sqrt((i % col - centerIndex[j] % col) * (i % col - centerIndex[j] % col) + (i / col - centerIndex[j] / col) * (i / col - centerIndex[j] / col));

                            }
                            double minTmp = distance[i][0];//设到第1个初始中心的距离最小
                            resultIndex[i] = 1;//则聚类的结果为属于第1类
                            for (int j = 0; j < K; j++)
                            {
                                if (distance[i][j] < minTmp) //判断到第j个聚类中心距离的大小
                                {
                                    minTmp = distance[i][j];
                                    resultIndex[i] = j + 1;
                                }
                            }
                        }
                        if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                        {
                            if (data[i] >= MinIgnoredValue && data[i] <= maxIteration)
                                resultIndex[i] = 0;
                        }

                    }

                    //迭代更新聚类中心
                    for (int i = 0; i < K; i++)
                    {
                        int num = 0;  //统计类中元素个数
                        center[i] = 0.0;
                        for (int j = 0; j < row * col; j++)
                        {
                            if (resultIndex[j] == i + 1)
                            {
                                num++;
                                center[i] += data[j];
                            }
                        }
                        center[i] = center[i] / num;
                    }
                    //判断聚类中心是否变化
                    bool IsEqual = true;
                    for (int i = 0; i < K; i++)
                    {
                        if (Math.Abs(center[i] - centerCopy[i]) > 0.001)
                        {
                            IsEqual = false;
                            break;
                        }
                    }
                    if (IsEqual == false)   //两次聚类中心有变化
                    {
                        for (int i = 0; i < K; i++)
                            centerCopy[i] = center[i];
                    }
                    else  //如果聚类中心不变，结束迭代
                        break;
                    curIteration++;
                } while (curIteration < maxIteration);//k-mean聚类完成

                double sum = 0;
                //获取最大值、最小值
                for (int i = 0; i < row * col; i++)
                {
                    if (resultIndex[i] > MaxValue)
                        MaxValue = resultIndex[i];
                    if (resultIndex[i] < MinValue)
                        MinValue = resultIndex[i];
                    sum += resultIndex[i];
                }
                //获取平均值
                MeanValue = sum / (row * col);
                sum = 0;
                for (int i = 0; i < row * col; i++)
                {
                    sum += ((double)resultIndex[i] - MeanValue) * ((double)resultIndex[i] - MeanValue);
                }
                //获取方差
                StdValue = sum / (row * col);

                //HDF4Operator.write(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf + "_Kmean.hdf", driver, row, col, resultIndex, ImageDate, dataType, scale, Offsets, startLog, endLog, startLat, endLat, fillValue, MaxValue, MinValue, DSResolution);

                HDF4Operator.WriteCustomHDF2DFile(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf[n] + "_cluster.hdf", ImageDate, dataType, "0", DataSetName, resultIndex, scale, Offsets, startLog, endLog, startLat, endLat, row, col, MaxValue, MinValue, MeanValue, StdValue, (int)fillValue, DSResolution, "2");
            }
            MessageBox.Show("完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxK_KeyPress(object sender, KeyPressEventArgs e)
        {
            //允许输入数字、删除键 
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
            {
                MessageBox.Show("请输入正确的类型数字", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.textBoxK.Text = "";
                e.Handled = true;
            }
        }

        private void textBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxW1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxW2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
