using CefSharp;
using CefSharp.WinForms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using MarineSTMiningSystem.DataOP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MarineSTMiningSystem.GUI
{
    public partial class MainForm : Form
    { 
        /**************程序加载及界面初始化****************/
        private static StartupForm startFrm = new StartupForm();

        /**************************************************/

        /**********************界面***********************/
        public bool blOpenQueryDlg = false;


        /**************************************************/


        /******************Neo4j数据库**********************/
        private string Neo4jIP;                 //Neo4j地址
        private string Neo4jPortNum;            //端口号
        private string Neo4jUser;               //用户名
        private string Neo4jPwd;                //密码
        private QueryDlg TemporalSpatioDlg;
        //WKT多边形类型
        public enum WKTType { NULL, RECTANGLE, POLYGON, POINT };
        public WKTType TYPE = WKTType.NULL;

        public Dictionary<string, string> FieldsMap;
        public List<String[]> ListOfProcess;
        public HashSet<String> ListOfTime;

        public CefSharp.WinForms.ChromiumWebBrowser browser = null;

        public void SetNeo4jConnection(string ip,string port,string user,string pwd)
        {
            Neo4jIP= ip;
            Neo4jPortNum=port;
            Neo4jUser=user;
            Neo4jPwd=pwd;
        }

        public void SetStatuLabel(string info)
        {
            toolStripStatusLabelState.Text = info;
        }

        /**************************************************/

        /******************ESRI相关************************/
        ILayer SelectedLayer_TOC = null;

        /***************************************************/
        public MainForm()
        {
            ESRI.ArcGIS.RuntimeManager.BindLicense(ESRI.ArcGIS.ProductCode.EngineOrDesktop, ESRI.ArcGIS.LicenseLevel.GeodatabaseUpdate);
            Thread thread = new Thread(WaitForLoad);
            thread.Start();
            startFrm.setProcess("初始化控件...");
            InitializeComponent();//控件初始化
            MainControlsInitialization();//主界面设置
            startFrm.setProcess("加载配置文件...");
            loadConfiguration();//加载配置文件
            startFrm.setProcess("加载底图...");
            EsriControlsInitialization();
            //关闭启动窗体
            startFrm.Close();
        }
        public void EsriControlsInitialization()
        {
            this.DoubleBuffered = true;
            if (VisualizationMapControl.Map.LayerCount > 0)
            {
                int layerCount = VisualizationMapControl.Map.LayerCount;
                for (int i = 0; i < layerCount; i++)
                {
                    VisualizationMapControl.DeleteLayer(0);
                }
                VisualizationMapControl.SpatialReference = null;
            }
            VisualizationMapControl.Map.Name = "图层";
            IWorkspaceFactory pWSF;
            IWorkspace pWS;
            IRasterWorkspace pRWS;
            IRasterDataset pRasterDataset;
            string filePath = "../../../Data/Layer/sea.tif";
            string pathName = System.IO.Path.GetDirectoryName(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);
            pWSF = new RasterWorkspaceFactoryClass();
            pWS = pWSF.OpenFromFile(pathName, 0);
            pRWS = pWS as IRasterWorkspace;
            pRasterDataset = pRWS.OpenRasterDataset(fileName);
            //影像金字塔的判断与创建
            IRasterPyramid pRasPyrmid;
            pRasPyrmid = pRasterDataset as IRasterPyramid;
            if (pRasPyrmid != null)
            {
                if (!(pRasPyrmid.Present))
                {
                    pRasPyrmid.Create();
                }
            }
            IRaster pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            VisualizationMapControl.AddLayer(pRasterLayer as ILayer, 0);
            VisualizationMapControl.get_Layer(0).Visible = false;
            //wgs84世界地图服务
            IMapServerRESTLayer mapServerRESTLayer = new MapServerRESTLayerClass();
            mapServerRESTLayer.Connect("http://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer");
            VisualizationMapControl.AddLayer(mapServerRESTLayer as ILayer,1);

            //用于动态转换坐标 使得太平在中间
            IEnvelope Area = new EnvelopeClass();
            Area.XMin = 21; Area.XMax = 236; Area.YMin = -43; Area.YMax = 78;

            VisualizationMapControl.Extent = Area;
            VisualizationMapControl.Refresh();
            VisualizationMapControl.DeleteLayer(0);
            VisualizationMapControl.Visible = true;
        }
        public void MainControlsInitialization()
        {
            statusStrip1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStripStatusLabelLat.Alignment = ToolStripItemAlignment.Right;
            toolStripStatusLabelLon.Alignment = ToolStripItemAlignment.Right;
            trackBar1.Maximum = 0;

            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-web-security", "1");
            Cef.Initialize(settings);

            browser = new CefSharp.WinForms.ChromiumWebBrowser("about:blank");
            browser.Dock = DockStyle.Fill;
            tabControl2.TabPages[1].Controls.Add(browser);

            CefSharpSettings.LegacyJavascriptBindingEnabled = true;//新cefsharp绑定需要优先申明
        }
        public void WaitForLoad()
        {
            startFrm = new StartupForm();
            startFrm.ShowDialog();
        }
        private void loadConfiguration()
        {
            FieldsMap = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            string file = "../../../Data/Neo4jData.xml";
            doc.Load(file);//Load方法必须使用相对路径 LoadXml要求文件为XML字符串，否则会出错
            XmlNode xn = doc.SelectSingleNode("configuration");
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                string FieldName = xe.GetAttribute("NAME");
                string DataType = xe.GetAttribute("DataType");
                string ChineseName = xe.GetAttribute("Chinese");
                try
                {
                    FieldsMap.Add(FieldName, DataType + ',' + ChineseName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("配置文件加载出错:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
        public void setProgessVisible(bool bl)
        {
            toolStripProgressBar1.Visible = bl;
        }

        public void setPlayBarVisible(bool bl)
        {
            if (bl)
            {
                toolStrip1.Visible = true;
                LastBtn.Visible = true;
                NextBtn.Visible = true;
                trackBar1.Visible = true;
                StartLabel.Visible = true;
                EndLabel.Visible = true;
                PlayBtn.Visible = true;
                StopBtn.Visible = true;
                StartLabel.Text = ListOfTime.ElementAt(0);
                EndLabel.Text = ListOfTime.ElementAt(ListOfTime.Count - 1);
            }
            else
            {
                toolStrip1.Visible = false;
                LastBtn.Visible = false;
                NextBtn.Visible = false;
                PlayBtn.Visible = false;
                StopBtn.Visible = false;
                trackBar1.Visible = false;
                StartLabel.Visible = false;
                EndLabel.Visible = false;
            }
        }
        public void setToolBarVisible(bool bl) { toolStrip1.Visible = bl; }
        private void VisualizationMapControl_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                VisualizationMapControl.CurrentTool = null;
                TYPE = WKTType.NULL;

            }
            else if (e.button == 4)//滚轮按下
            {
                VisualizationMapControl.MousePointer = esriControlsMousePointer.esriPointerPan;
                VisualizationMapControl.Pan();
                VisualizationMapControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
            }
            switch (TYPE)
            {
                case WKTType.NULL:
                    {
                        break;
                    }
                case WKTType.POLYGON:
                    {
                        IGeometry TrackGeometry = VisualizationMapControl.TrackPolygon();
                        string WKT=string.Empty;
                        try
                        {
                            WKT= Convertor.ConvertGeometryToWKT(TrackGeometry);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show("多边形转换为WKT出错:"+ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                         
                        TemporalSpatioDlg.textBoxWKT.Text = WKT;
                        TemporalSpatioDlg.Focus();
                        break;
                    }
                case WKTType.RECTANGLE:
                    {
                        IGeometry TrackGeometry = VisualizationMapControl.TrackRectangle();
                        string WKT = string.Empty;
                        try
                        {
                            WKT = Convertor.ConvertGeometryToWKT(TrackGeometry,false);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("多边形转换为WKT出错:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        TemporalSpatioDlg.textBoxWKT.Text = WKT;
                        TemporalSpatioDlg.Focus();
                        break;
                    }
                case WKTType.POINT:
                    {
                        string WKT = "POINT(" + e.mapX + " " + e.mapY + ")";
                        TemporalSpatioDlg.textBoxWKT.Text = WKT;
                        TemporalSpatioDlg.Focus();
                        break;
                    }
            }
            
        }

        private void VisualizationMapControl_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            toolStripStatusLabelLat.Visible = true;
            toolStripStatusLabelLon.Visible = true;
            toolStripStatusLabelLat.Text = "纬度：" + e.mapY.ToString();
            toolStripStatusLabelLon.Text = "经度：" + e.mapX.ToString();
        }

        private void 时空数据查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
                   
            if (Neo4j32.isConnected&&!blOpenQueryDlg)
            {
                TemporalSpatioDlg = new QueryDlg(this);
                TemporalSpatioDlg.Show();
                blOpenQueryDlg = true;
            }
            if(!Neo4j32.isConnected)
            {
                MessageBox.Show("未连接Neo4j数据库!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (TemporalSpatioDlg != null)
            {
                TemporalSpatioDlg.Focus();
            }
        }


        private void 关于我们ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutUsForm about = new AboutUsForm();
            about.ShowDialog();
        }
        

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar1.Value < 0) trackBar1.Value = 0;
            if (trackBar1.Value > trackBar1.Maximum) trackBar1.Value = trackBar1.Maximum;
            if (trackBar1.Value > trackBar1.Minimum && trackBar1.Value < trackBar1.Maximum)
            {
                List<IGeometry> geo = new List<IGeometry>();
                IMap pMap = VisualizationMapControl.Map;
                IGraphicsContainer pContainer = pMap as IGraphicsContainer;
                IElementCollection cEle = new ElementCollectionClass();
                pContainer.DeleteAllElements();

                for (int i = 0; i < ListOfProcess[trackBar1.Value].Length; i++)
                {
                    geo.Add(Convertor.ConvertWKTToGeometry(ListOfProcess[trackBar1.Value][i]));

                }

                for (int i = 0; i < geo.Count; i++)
                {
                    IFillShapeElement pPoloygonElement = new PolygonElementClass();

                    ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                    IRgbColor pColor = new RgbColorClass();
                    pColor.Red = 255;
                    pColor.Green = 0;
                    pColor.Blue = 0;
                    pSimpleFillSymbol.Color = pColor;
                    pSimpleFillSymbol.Outline.Color = pColor;
                    pPoloygonElement.Symbol = pSimpleFillSymbol;
                    IElement pEle = pPoloygonElement as IElement;
                    pEle.Geometry = geo[i];
                    cEle.Add(pEle);

                }
                pContainer.AddElements(cEle, 0);
                this.VisualizationMapControl.Refresh();
                this.Update();
                StartLabel.Text = ListOfTime.ElementAt(trackBar1.Value);
            }          
        }

        private void LastBtn_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value == trackBar1.Minimum) trackBar1.Value = trackBar1.Minimum;
            else trackBar1.Value -= 1;
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            if (trackBar1.Value == trackBar1.Maximum) trackBar1.Value = trackBar1.Maximum;
            else trackBar1.Value += 1;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TemporalSpatioDlg != null) TemporalSpatioDlg.Close();
        }

        private void 连接数据库ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void 移除图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedLayer_TOC != null)
            {
                if (MessageBox.Show("是否移除图层？", "移除图层", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)

                {
                    VisualizationMapControl.Map.DeleteLayer(SelectedLayer_TOC);
                    VisualizationMapControl.SpatialReference = null;
                }
            }
        }

        private void contextMenuStrip2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                IBasicMap map = null;
                ILayer layer = null;
                System.Object other = null;
                System.Object index = null;
                esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
                axTOCControl1.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
                if (item == esriTOCControlItem.esriTOCControlItemLayer)
                {
                    SelectedLayer_TOC = layer;
                    contextMenuStrip1.Show(axTOCControl1, e.x, e.y);
                }

            }
        }

        private void 连接数据库ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ConnectDB connDB = new ConnectDB(this);
            connDB.ShowDialog();
        }

        private void 关闭连接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Neo4j32.Neo4jClose();
            toolStripStatusLabelState.Text = "数据库连接已关闭";
        }

        private void Play_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < trackBar1.Maximum; i++)
            //{
            //    trackBar1.Value += 1;
            //}
            timer1.Enabled = true;
        }

        private void ExitPlay_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 0;
            IMap pMap = VisualizationMapControl.Map;
            IGraphicsContainer pContainer = pMap as IGraphicsContainer;
            pContainer.DeleteAllElements();
            VisualizationMapControl.Refresh();
            setPlayBarVisible(false);
            browser.Load("about:blank");
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            IMap pMap = VisualizationMapControl.Map;
            IGraphicsContainer pContainer = pMap as IGraphicsContainer;
            pContainer.DeleteAllElements();
            VisualizationMapControl.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (trackBar1.Value < trackBar1.Maximum)
                trackBar1.Value += 1;
            else
            {
                PauseBtn.Visible = false;
                PlayBtn.Visible = true;
                timer1.Enabled = false;
                trackBar1.Value = trackBar1.Minimum;
                IMap pMap = VisualizationMapControl.Map;
                IGraphicsContainer pContainer = pMap as IGraphicsContainer;
                pContainer.DeleteAllElements();
                VisualizationMapControl.Refresh();
            }
                
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            PlayBtn.Visible = false;
            PauseBtn.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            PauseBtn.Visible = false;
            PlayBtn.Visible = true;
            VisualizationMapControl.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            trackBar1.Value = trackBar1.Minimum;
            IMap pMap = VisualizationMapControl.Map;
            IGraphicsContainer pContainer = pMap as IGraphicsContainer;
            pContainer.DeleteAllElements();
            VisualizationMapControl.Refresh();
            PlayBtn.Visible = true;
            PauseBtn.Visible = false;
        }
    }
}
