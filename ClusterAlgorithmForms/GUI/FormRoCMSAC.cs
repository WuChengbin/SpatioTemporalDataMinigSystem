using ClusterAlgorithm;
using OSGeo.GDAL;
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

namespace ClusterAlgorithm.GUI
{
    public partial class FormRoCMSAC : Form
    {
        const int maxIteration = 200;//最大迭代次数
        List<Dual> duSets = new List<Dual>();
        string DataSetName = "";
        double threshold1 = 0.5;//属性距离阈值
        double threshold2 = 15.0;//空间距离阈值，行列号距离
        const int threshold3 = 5;//栅格阈值数，组成簇的栅格一旦小于此值不予考虑
        public FormRoCMSAC()
        {
            
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            textBoxPropertySimilarity.Text = "0.5";
            textBoxSpatialSimilarity.Text = "15.0";
            textBoxK.Text = "3";

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
            if (textBoxK.Text == "")
            {
                MessageBox.Show("请输入K值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                double.Parse(textBoxPropertySimilarity.Text);
                double.Parse(textBoxSpatialSimilarity.Text);
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
            DataSetName = comboBoxDataset.SelectedItem.ToString() + "_cluster3";
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
            threshold1 = double.Parse(textBoxPropertySimilarity.Text.ToString());//属性阈值
            threshold2 = double.Parse(textBoxSpatialSimilarity.Text.ToString());//空间阈值
            int[] resultIndex = new int[row * col];  //聚类结果
            double[] center = new double[K];     //聚类中心
            double[] centerCopy = new double[K]; //将旧的中心点保留以作比较 
            int[] centerIndex = new int[K];    //聚类中心索引 K * 1
            List<double> dataBuffer = new List<double>();
            duSets = new List<Dual>();
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
                        if (data[i] != fillValue * scale&& data[i] != 0)
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
                        else if(data[i] == 0)
                        {
                            resultIndex[i] = 0;
                        }
                        else
                        {
                            distance[i] = new double[K];
                            for (int j = 0; j < K; j++)
                            {
                                distance[i][j] = 0.0;
                                distance[i][j] = Math.Abs(data[i] - center[j]);
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
                        if (Math.Abs(center[i] - centerCopy[i]) > 0.0001)
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


                //属性与空间双重约束聚类
                for (int j = 0; j < row * col; j++)//I*mRows*mCols
                {
                    Dual du = new Dual();
                    du.SetclusID(resultIndex[j]);
                    du.k = resultIndex[j];
                    du.SetVisited(false);
                    du.x = j / col;
                    du.y = j % col;
                    duSets.Add(du);
                    //if (duSets[j].clusID == -1)
                    //    MessageBox.Show(j.ToString());
                }

                //开始遍历,提取空间簇
                int clusID = K + 1;
                for (int i = 1; i < K + 1; i++)
                {
                    for (int j = 0; j < row * col; j++)
                    {
                        //去除边界栅格
                        if (duSets[j].x == 0 || duSets[j].x == row - 1 || duSets[j].y == 0 || duSets[j].y == col - 1)
                            continue;
                        else if (duSets[j].k == i && !duSets[j].isVisited())
                        {
                            //遍历8-邻域,查找所有连通区域，构成新的空间簇
                            duSets[j].SetVisited(true);
                            duSets[j].SetclusID(clusID);
                            ergodic(i, j, clusID, row, col);//进行递归查询全部同属性连通区域
                            clusID++;
                        }
                    }
                }
                //处理未被搜索的边界栅格
                for (int i = 1; i < K + 1; i++)
                {
                    for (int j = 0; j < row * col; j++)
                    {
                        if (duSets[j].x == 0 || duSets[j].x == row - 1 || duSets[j].y == 0 || duSets[j].y == col - 1)
                            if (!duSets[j].isVisited() && duSets[j].k == i)
                            {
                                duSets[j].SetVisited(true);
                                duSets[j].SetclusID(0);

                            }
                    }
                }

                //记录组成空间簇的空间位置（方法：统计所有x和y的累加和除以总栅格数）
                //将每个簇抽象为三个属性，即簇号，属性中心，空间中心，为合并做准备    
                List<duall> reSets = new List<duall>();
                //初始化

                for (int j = K + 1; j < clusID; j++)//I*clusID
                {
                    duall re = new duall();
                    re.clusid = j;
                    re.num = 0;
                    re.mean = 0.0;
                    re.stdep = 0.0;
                    re.positionx = 0.0;
                    re.positiony = 0.0;
                    for (int i = 0; i < row * col; i++)
                        if (duSets[i].clusID == j)
                        {
                            re.centers = center[duSets[i].k - 1];
                            break;
                        }
                    reSets.Add(re);

                }
                //开始遍历每个空间簇，求空间中心，均值和标准差
                for (int j = 0; j < reSets.Count(); j++)
                {
                    for (int i = 0; i < row * col; i++)
                    {
                        if (duSets[i].clusID == reSets[j].clusid)
                        {
                            reSets[j].num++;
                            reSets[j].positionx += duSets[i].x;
                            reSets[j].positiony += duSets[i].y;
                            reSets[j].mean += (double)data[i];
                            //reSets[j].stdep += pow(((double)pBuffer[j]*mScale),2.0);
                        }

                    }
                    reSets[j].mean = reSets[j].mean / reSets[j].num;
                    //reSets[j].stdep =(reSets[j].stdep/reSets[j].num)-pow(reSets[j].mean,2.0); 


                    if (reSets[j].num <= threshold3)//将小簇簇号直接设为0
                    {
                        for (int i = 0; i < row * col; i++)
                        {
                            if (duSets[i].clusID == reSets[j].clusid)
                                duSets[i].SetclusID(0);
                        }
                    }
                    reSets[j].positionx = reSets[j].positionx / reSets[j].num;
                    reSets[j].positiony = reSets[j].positiony / reSets[j].num;

                }
                //进行簇的双重约束合并
                double atrribute = 0.0;
                double distance1 = 0.0;
                for (int j = 0; j < reSets.Count(); j++)
                {
                    for (int i = 0; i < reSets.Count(); i++)
                    {
                        if (reSets[j].clusid != reSets[i].clusid)
                        {
                            atrribute = Math.Abs(reSets[j].centers - reSets[i].centers);
                            distance1 = (reSets[j].positionx - reSets[i].positionx) * (reSets[j].positionx - reSets[i].positionx) + (reSets[j].positiony - reSets[i].positiony) * (reSets[j].positiony - reSets[i].positiony);
                            distance1 = Math.Pow(distance1, 0.5);
                            if (atrribute < threshold1 && distance1 < threshold2 && reSets[j].num > threshold3 && reSets[i].num > threshold3)//reSets[j].num >threshold3 && reSets[i].num >threshold3
                            {
                                //reSets[i].clusid = reSets[j].clusid ;
                                reSets[j].positionx = (reSets[j].positionx + reSets[i].positionx) / 2.0;
                                reSets[j].positiony = (reSets[j].positiony + reSets[i].positiony) / 2.0;
                                reSets[j].centers = (reSets[j].centers + reSets[i].centers) / 2.0;
                                //reSets[j].num += reSets[i].num ;
                                //reSets[j].stdep =(reSets[j].stdep + pow(reSets[j].mean,2.0))*reSets[j].num + (reSets[i].stdep + pow(reSets[i].mean,2.0))*reSets[i].num;
                                reSets[j].mean = (reSets[j].mean * reSets[j].num + reSets[i].mean * reSets[i].num);
                                reSets[j].num += reSets[i].num;
                                reSets[j].mean = reSets[j].mean / reSets[j].num;
                                //reSets[j].stdep = (reSets[j].stdep/reSets[j].num)-pow(reSets[j].mean,2.0);
                                for (int m = 0; m < row * col; m++)
                                {
                                    if (duSets[m].clusID == reSets[i].clusid)
                                        duSets[m].clusID = reSets[j].clusid;
                                }
                                reSets[i].clusid = reSets[j].clusid;
                                reSets[i].centers = reSets[j].centers;
                                //reSets[i].num = reSets[j].num ;
                                reSets[i].positionx = reSets[j].positionx;
                                reSets[i].positiony = reSets[j].positiony;

                            }
                        }
                    }
                }
                for (int j = 0; j < reSets.Count(); j++)
                {
                    for (int i = 0; i < row * col; i++)
                    {
                        if (duSets[i].clusID == reSets[j].clusid)
                        {
                            reSets[j].stdep += Math.Pow(((double)data[i] - reSets[j].mean), 2.0);
                        }

                    }
                    reSets[j].stdep = reSets[j].stdep / reSets[j].num;
                    reSets[j].stdep = Math.Sqrt(reSets[j].stdep);
                }
                //得到结果
                for (int i = 0; i < row * col; i++)
                    resultIndex[i] = duSets[i].clusID;

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

                //将每一簇的均值和标准差输出到.txt中
                FileStream fs = new FileStream(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf[n] +"_result.txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入标题
                string stringID = "ID";
                stringID = stringID.PadRight(10);
                sw.Write(stringID);
                string stringMean = "MEAN";
                stringMean = stringMean.PadRight(20);
                sw.Write(stringMean);
                string stringStdep = "STDEP";
                stringStdep = stringStdep.PadRight(20);
                stringStdep += "\r\n";
                sw.Write(stringStdep);
                //开始写入内容
                for (int j = 0; j < reSets.Count(); j++)
                {
                    //将每一簇的均值和标准差输出到.txt中

                    sw.Write(reSets[j].clusid.ToString().PadRight(10) + reSets[j].mean.ToString().PadRight(20,' ') + reSets[j].stdep.ToString().PadRight(20,' ') + "\r\n");
                }
                //清空缓冲区、关闭流
                sw.Flush();
                sw.Close();

                //写hdf文件
                HDF4Operator.WriteCustomHDF2DFile(textBoxFilePath.Text + "\\" + hdfFileNameNoPathNohdf[n] + "_cluster3.hdf", ImageDate, dataType, "0", DataSetName, resultIndex, scale, Offsets, startLog, endLog, startLat, endLat, row, col, MaxValue, MinValue, MeanValue, StdValue, (int)fillValue, DSResolution, "2");
            }
            MessageBox.Show("完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        void ergodic(int kID, int m, int ID, int rows, int cols)
        {
            int[] w = { m - cols - 1, m - cols, m - cols + 1, m - 1, m + 1, m + cols - 1, m + cols, m + cols + 1 };
            for (int i = 0; i < 8; i++)
            {
                if (duSets[w[i]].k == kID && !duSets[w[i]].isVisited())
                {
                    duSets[w[i]].SetclusID(ID);
                    duSets[w[i]].SetVisited(true);
                    if (duSets[w[i]].x != 0 && duSets[w[i]].x != rows - 1 && duSets[w[i]].y != 0 && duSets[w[i]].y != cols - 1)
                        ergodic(kID, w[i], ID, rows, cols);

                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxPropertySimilarity_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxSpatialSimilarity_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxK_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxMin_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBoxMax_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
    }
}
