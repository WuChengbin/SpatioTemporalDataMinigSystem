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
    public partial class FormTimeKmean : Form
    {
        int maxIteration = 200;//最大迭代次数
        
        string DataSetName = "";
        public FormTimeKmean()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
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
                string DataSetName = HDF4Operator.GetDatasetsName(hdfFileName);
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

        private void buttonOK_Click(object sender, EventArgs e)
        {
            ListBox.ObjectCollection inFileNames;
            inFileNames = listBoxFileList.Items;
            int filenumber = inFileNames.Count;
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
            DataSetName = comboBoxDataset.SelectedItem.ToString() + "_timecluster";
            string ImageDateS = HDF4Operator.GetDatasetsImageDate(hdfFileName[0]);//开始数据时间
            string ImageDateE = HDF4Operator.GetDatasetsImageDate(hdfFileName[filenumber-1]);//结束数据时间
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

            double[] data = new double[filenumber * row * col];//多个文件读取结果存储
            int K = int.Parse(textBoxK.Text.ToString());//聚类数
            int[] resultIndex = new int[row * col];  //聚类结果

            double[][] center = new double[K][];//聚类中心-时间序列
            for(int i=0;i<K;i++)
            {
                center[i] = new double[filenumber];
            }
            
            double[][] centerCopy = new double[K][]; //将旧的中心点保留以作比较
            for (int i = 0; i < K; i++)
            {
                centerCopy[i] = new double[filenumber];
            }
           
            int[] centerIndex = new int[K];    //聚类中心索引 K * 1
            double[] pMeanBuffer = new double[row * col];//每个时间序列的均值
            int[] pValueNumber = new int[row * col];//每个时间序列有效值的个数
            double[][] pS = new double[row * col][];//每个时间序列
            for(int i=0;i<row*col;i++)
            {
                pS[i] = new double[filenumber];
            }
            
            List<double> dataBuffer = new List<double>();//每读取一次有效值所需空间

            double[] DataBuffer = new double[row * col];//随机取聚类中心，为保证是栅格任意位置，只读取第一个数据
            DataBuffer= HDF4Operator.GetDatasetsData(hdfFileName[0]);//压入全部数值，包括-9999
            for (int i = 0; i < row * col; i++)
            {
                DataBuffer[i] = DataBuffer[i] * scale;
                if (DataSetName == "Precipitation_timecluster")
                {
                    if ( DataBuffer[i] != 0)
                    {
                        if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                        {
                            if (DataBuffer[i] < MinIgnoredValue || DataBuffer[i] > maxIteration)
                                dataBuffer.Add(DataBuffer[i]);
                        }
                        else
                            dataBuffer.Add(DataBuffer[i]);
                    }
                }
                else
                {
                   if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                   {
                        if (DataBuffer[i] < MinIgnoredValue || DataBuffer[i] > maxIteration)
                            dataBuffer.Add(DataBuffer[i]);
                    }
                   else
                        dataBuffer.Add(DataBuffer[i]);
                    
                }
            }

            for (int i=0;i< row * col; i++)//初始化每个序列有效值个数
            {
                pValueNumber[i] = 0;
            }
            for(int i=0;i<row*col;i++)//初始化聚类结果
            {
                resultIndex[i] = -1;
            }

            double MaxValue = 0;//最大值
            double MinValue = 100;//最小值
            double MeanValue = 0;//平均值
            double StdValue = 0;//方差

            for (int n = 0; n < inFileNames.Count; n++)
            {
                double[] Data = new double[row * col];
                Data = HDF4Operator.GetDatasetsData(hdfFileName[n]);
                for (int i = 0; i < row * col; i++)
                {
                    Data[i] = Data[i] * scale;
                    if (DataSetName == "Precipitation_timecluster")
                    {
                        if (Data[i] != fillValue * scale && Data[i] != 0)
                        {
                            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                            {
                                if (Data[i] < MinIgnoredValue || Data[i] > maxIteration)
                                {
                                    data[i + n * row * col] = Data[i];
                                    pMeanBuffer[i] += Data[i];
                                    pValueNumber[i]++;
                                }
                                   
                            }
                            else
                            {
                                data[i + n * row * col] = Data[i];
                                pMeanBuffer[i] += Data[i];
                                pValueNumber[i]++;
                            }
                               
                        }
                    }
                    else
                    {
                        if (Data[i] != fillValue * scale)
                        {
                            if (textBoxMax.Text.ToString() != "" && textBoxMin.Text.ToString() != "")//如果属性忽略值不为空
                            {
                                if (Data[i] < MinIgnoredValue || Data[i] > maxIteration)
                                {
                                    data[i + n * row * col] = Data[i];
                                    pMeanBuffer[i] += Data[i];
                                    pValueNumber[i]++;
                                }
                                  
                            }
                            else
                            {
                                data[i + n * row * col] = Data[i];
                                pMeanBuffer[i] += Data[i];
                                pValueNumber[i] ++;
                            }
                               
                        }
                    }
                }
            }

            //计算序列均值
            for(int i=0;i<row*col;i++)
            {
                if(pValueNumber[i]!=0)
                {
                    pMeanBuffer[i] = pMeanBuffer[i] / pValueNumber[i];
                    for(int j=0;j<filenumber;j++)
                    {
                        pS[i][j] = data[i + j * row * col];//pValueNumber[i]==0的地方pS的值都为0
                    }
                }
            }

            //随机生成聚类中心
            for (int i = 0; i < K; i++)
            {
                int j = 0;
                Random random = new Random();
                int a = random.Next(0, dataBuffer.Count - 1);
                if(dataBuffer[a]!=fillValue*scale)
                {
                    centerIndex[i] = a;
                }
                for (j = 0; j < i; j++)
                {
                    if (dataBuffer[a] == dataBuffer[centerIndex[j]] || dataBuffer[a]==fillValue*scale)
                    {
                        i--;
                        break;
                    }

                }
            }


            //选取聚类中心
            for (int i=0;i<K;i++)
            {
                for(int j=0;j<filenumber;j++)
                {
                    center[i][j] = pS[centerIndex[i]][j];
                    centerCopy[i][j]= pS[centerIndex[i]][j];
                }
            }

            int curIteration = 0;
            do
            {
                double[][] distance = new double[row * col][];
                double[][]dis1=new double[row * col][];
                double[][]dis2=new double[row * col][];

                for(int i=0;i<row*col;i++)
                {
                    if (pValueNumber[i] == 0)
                        resultIndex[i] = -1;
                   
                    else
                    {
                        distance[i] = new double[K];
                        dis1[i]= new double[K];
                        dis2[i]= new double[K];
                        for(int m=0;m<K;m++)
                        {
                            distance[i][m] = 0.0;
                            dis1[i][m] = 0.0;
                            dis2[i][m] = 0.0;
                            for(int j=0;j<filenumber;j++)
                            {
                                distance[i][m] += (pS[i][j] - pMeanBuffer[i]) * (center[m][j] - pMeanBuffer[centerIndex[m]]);
                                dis1[i][m] += (pS[i][j] - pMeanBuffer[i]) * (pS[i][j] - pMeanBuffer[i]);
                                dis2[i][m] += (center[m][j] - pMeanBuffer[centerIndex[m]]) * (center[m][j] - pMeanBuffer[centerIndex[m]]);
                            }
                            distance[i][m] = distance[i][m] / Math.Sqrt(dis1[i][m] * dis2[i][m]);
                        }
                        double maxTmp = Math.Abs(distance[i][0]);//相关系数的绝对值越大越相似
                        resultIndex[i] = 1;
                        for(int j=0;j<K;j++)
                        {
                            if(Math.Abs(distance[i][j])>maxTmp)
                            {
                                maxTmp = Math.Abs(distance[i][j]);
                                resultIndex[i] = j+1;
                            }
                        }
                    }
                }

                //迭代更新聚类中心
                for(int i=0;i<K;i++)
                {
                    for(int j=0;j<filenumber;j++)
                    {
                        int num = 0;
                        center[i][j] = 0.0;
                        for(int n=0;n<row*col;n++)
                        {
                            if(resultIndex[n]==i+1)
                            {
                                num++;
                                center[i][j] += pS[n][j];
                            }
                        }
                        center[i][j] = center[i][j] / num;
                    }
                }

                //判断聚类中心是否变化
                bool IsEqual = true;
                for (int i = 0; i < K; i++)
                {
                    for (int j = 0; j < filenumber; j++)
                    {
                        if (Math.Abs(center[i][j] - centerCopy[i][j]) > 0.001)
                        {
                            IsEqual = false;
                            break;
                        }
                    }
                }
             
               if (IsEqual == false)   //两次聚类中心有变化
                {
                    for (int i =0; i < K; i++)
                    {
                        for(int j=0;j<filenumber;j++)
                        {
                            centerCopy[i][j] = center[i][j];
                        }
                    }
                }
                else  //如果聚类中心不变，结束迭代
                    break;

                curIteration++;


            } while (curIteration < maxIteration);

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

            string ImageDate = ImageDateS + "-" + ImageDateE;
            string mOutFileName = ImageDate + DataSetName + ".hdf";
            //输出结果HDF
            HDF4Operator.WriteCustomHDF2DFile(textBoxFilePath.Text + "\\" + mOutFileName, ImageDate, dataType, "0", DataSetName, resultIndex, scale, Offsets, startLog, endLog, startLat, endLat, row, col, MaxValue, MinValue, MeanValue, StdValue, (int)fillValue, DSResolution, "2");
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
    }
}
