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

namespace MarineSTMiningSystem
{
    public partial class GetProjectForm : Form
    {
        public GetProjectForm()
        {
            InitializeComponent();
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            ofd.Filter = "图像文件|*.hdf;*.tif;*.tiff|hdf文件|*.hdf|tif文件|*.tif;*tiff|所有文件|*.*";
            //openFileDialog.RestoreDirectory = true;
            //openFileDialog.FilterIndex = 1;
            //ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = ofd.FileName;
            }

            //string inTifPath = @"E:\rain\coor.tiff";
            //string outTifPath = @"E:\rain\14_tif_coor.tif";
            Gdal.AllRegister();//注册所有的格式驱动
            Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "YES");//支持中文路径和名称

            //打开文件
            Dataset ds = Gdal.Open(ofd.FileName, Access.GA_ReadOnly);
            //int col = ds.RasterXSize;//列数
            //int row = ds.RasterYSize;//行数

            //Band demband1 = ds.GetRasterBand(1);//读取波段

            double[] argout = new double[6]; //如果图像不含地理坐标信息，默认返回值是：(0,1,0,0,0,1)
            ds.GetGeoTransform(argout);//读取地理坐标信息
            string projection = ds.GetProjection();

            string[] metadatas = ds.GetMetadata("");//获取元数据

            richTextBox1.Clear();
            richTextBox1.AppendText("GeoTransform:\n{");
            foreach(double v in argout) richTextBox1.AppendText(v.ToString()+",");
            richTextBox1.Text = richTextBox1.Text.Remove(richTextBox1.Text.Length - 1);
            richTextBox1.AppendText("}\n");
            richTextBox1.AppendText("\nProject:\n");
            richTextBox1.AppendText(projection.ToString()+"\n");
            richTextBox1.AppendText("\nMetadata:\n");
            foreach (string v in metadatas) richTextBox1.AppendText(v + "\n");

            ds.Dispose();
        }
    }
}
