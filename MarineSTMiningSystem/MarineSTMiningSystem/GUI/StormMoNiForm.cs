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

namespace MarineSTMiningSystem
{
    public partial class StormMoNiForm : Form
    {
        private BackgroundWorker worker = new BackgroundWorker();//后台线程
        public StormMoNiForm()
        {
            InitializeComponent();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;//支持取消
            worker.DoWork += new DoWorkEventHandler(worke);//正式做事情的地方
            worker.ProgressChanged += new ProgressChangedEventHandler(ProgessChanged);//任务进行时，报告进度
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CompltetWork);//任务完成时要做的
            
        }

        private void worke(object sender, DoWorkEventArgs e)
        {//后台工作方法
            for(int fileId=0;fileId<fileCount;fileId++)
            {//每个文件
                if (worker.CancellationPending)
                {//取消
                    e.Cancel = true;
                    return;
                }
                int progress = (fileId * 100) / fileCount;
                worker.ReportProgress(progress);//记录进度

                string shpPath = shpFileNames[fileId].ToString();
                Shp shp = new Shp(shpPath);
                if (!shp.ExistField("Id")) shp.AddIntField("Id");
                if (!shp.ExistField("Power")) shp.AddIntField("Power");
                if (!shp.ExistField("Area")) shp.AddRealField("Area", 16, 8);
                if (!shp.ExistField("Volume")) shp.AddRealField("Volume",16,8);
                if (!shp.ExistField("AvgRain")) shp.AddRealField("AvgRain",16,8);
                if (!shp.ExistField("MaxRain")) shp.AddRealField("MaxRain",16,8);
                if (!shp.ExistField("MinRain")) shp.AddRealField("MinRain",16,8);
                if (!shp.ExistField("MinLog")) shp.AddRealField("MinLog",16,8);
                if (!shp.ExistField("MinLat")) shp.AddRealField("MinLat",16,8);
                if (!shp.ExistField("MaxLog")) shp.AddRealField("MaxLog",16,8);
                if (!shp.ExistField("MaxLat")) shp.AddRealField("MaxLat",16,8);
                if (!shp.ExistField("Length")) shp.AddRealField("Length",16,8);
                if (!shp.ExistField("CoreLog")) shp.AddRealField("CoreLog",16,8);
                if (!shp.ExistField("CoreLat")) shp.AddRealField("CoreLat",16,8);
                if (!shp.ExistField("SI")) shp.AddRealField("SI",16,8);
                if (!shp.ExistField("LMax")) shp.AddRealField("LMax",16,8);
                if (!shp.ExistField("WMax")) shp.AddRealField("WMax",16,8);
                if (!shp.ExistField("ERatio")) shp.AddRealField("ERatio",16,8);
                if (!shp.ExistField("RecDeg")) shp.AddRealField("RecDeg",16,8);
                if (!shp.ExistField("SphDeg")) shp.AddRealField("SphDeg",16,8);
                if (!shp.ExistField("RecP1X")) shp.AddRealField("RecP1X", 16, 8);
                if (!shp.ExistField("RecP1Y")) shp.AddRealField("RecP1Y", 16, 8);
                if (!shp.ExistField("RecP2X")) shp.AddRealField("RecP2X", 16, 8);
                if (!shp.ExistField("RecP2Y")) shp.AddRealField("RecP2Y", 16, 8);
                if (!shp.ExistField("RecP3X")) shp.AddRealField("RecP3X", 16, 8);
                if (!shp.ExistField("RecP3Y")) shp.AddRealField("RecP3Y", 16, 8);
                if (!shp.ExistField("RecP4X")) shp.AddRealField("RecP4X", 16, 8);
                if (!shp.ExistField("RecP4Y")) shp.AddRealField("RecP4Y", 16, 8);
                if (!shp.ExistField("IsNoise")) shp.AddIntField("IsNoise");
                shp.Dispose();
                shp = new Shp(shpPath);
                int id = 0;
                foreach (Feature feature in shp.featureList)
                {
                    string polygonWkt = string.Empty;//wkt坐标
                    feature.GetGeometryRef().ExportToIsoWkt(out polygonWkt);

                    //构建暴雨多边形
                    StormPolygon stormPoly = new StormPolygon(polygonWkt);
                    stormPoly.CalculateRec();//计算平行于坐标轴的外包矩形
                    stormPoly.CalculateArea();//计算面积
                    double area = stormPoly.area;//获取面积
                    stormPoly.CalculateLength();//计算周长
                    double length = stormPoly.length;
                    stormPoly.CalculateCorePos();
                    Rectangle minAreaRec = stormPoly.GetMinAreaRec();//最小面积外包矩形
                    Circle minOutCir = stormPoly.GetMinOutCir();//最小面积外接圆
                    Circle maxInCir = stormPoly.GetMaxInCir();//最大面积内切圆
                    //形状系数（SI）：面积（A）/周长（P）
                    double si = (4 * Math.Sqrt(area)) / length;
                    double eRatio = minAreaRec.width / minAreaRec.length;
                    double recDeg = area / (minAreaRec.length * minAreaRec.width * Earth.OneLatLen()*0.001*Earth.OneLogLen((stormPoly.maxLat + stormPoly.minLat) / 2)*0.001);//最小外包矩形面积为近似计算
                    double sphDeg = maxInCir.r / minOutCir.r;

                    int power = 1;
                    RandomDistribution rd = new RandomDistribution();
                    double avgRain = rd.GetRandomValue(30, 10, 16, 100);
                    double minRain = rd.GetRandomValue(10, 10, 1, avgRain);
                    double maxRain= rd.GetRandomValue((avgRain+101)/2.0, 10, avgRain, 101);
                    double volume = avgRain * area*1000;//立方米
                    feature.SetField("Id", id++);
                    feature.SetField("Power", power);
                    feature.SetField("Area", area);
                    feature.SetField("Volume", volume);
                    feature.SetField("AvgRain", avgRain);
                    feature.SetField("MaxRain", maxRain);
                    feature.SetField("MinRain", minRain);
                    feature.SetField("MinLog", stormPoly.minLog);
                    feature.SetField("MinLat", stormPoly.minLat);
                    feature.SetField("MaxLog", stormPoly.maxLog);
                    feature.SetField("MaxLat", stormPoly.maxLat);
                    feature.SetField("Length", length);
                    feature.SetField("MaxLog", stormPoly.maxLog);
                    feature.SetField("CoreLog", stormPoly.coreLog);
                    feature.SetField("CoreLat", stormPoly.coreLat);
                    feature.SetField("SI", si);
                    feature.SetField("LMax", minAreaRec.length);
                    feature.SetField("WMax", minAreaRec.width);
                    feature.SetField("ERatio", eRatio);
                    feature.SetField("RecDeg", recDeg);
                    feature.SetField("SphDeg", sphDeg);
                    feature.SetField("RecP1X", minAreaRec.p1[0]);
                    feature.SetField("RecP1Y", minAreaRec.p1[1]);
                    feature.SetField("RecP2X", minAreaRec.p2[0]);
                    feature.SetField("RecP2Y", minAreaRec.p2[1]);
                    feature.SetField("RecP3X", minAreaRec.p3[0]);
                    feature.SetField("RecP3Y", minAreaRec.p3[1]);
                    feature.SetField("RecP4X", minAreaRec.p4[0]);
                    feature.SetField("RecP4Y", minAreaRec.p4[1]);
                    //feature.SetField("IsNoise", 0);

                    shp.oLayer.SetFeature(feature);
                }
                shp.Dispose();
            }
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

        string savePath = string.Empty;
        ListBox.ObjectCollection shpFileNames;//原始图像路径，含文件名
        int fileCount;//图像文件个数
        private void okBtn_Click(object sender, EventArgs e)
        {
            if (worker.IsBusy)
            {
                MessageBox.Show("正在进行处理！");
                return;
            }

            if (listBox1.Items.Count == 0)
            {
                MessageBox.Show("请添加处理文件！");
                return;
            }

            shpFileNames = listBox1.Items;
            fileCount = shpFileNames.Count;
            progressBar1.Show();
            worker.RunWorkerAsync();
        }
    }
}
