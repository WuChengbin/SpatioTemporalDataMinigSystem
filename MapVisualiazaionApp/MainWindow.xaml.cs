using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Esri.ArcGISRuntime.Data;
using MarineSTMiningSystem;
using MarineSTMiningSystem.DataOP;
using System.Xml;
using MapVisualizationApp.GUI;
using System.Timers;
using Panuon.UI;
using Esri.ArcGISRuntime;
using CefSharp.Wpf;
using CefSharp;
using Esri.ArcGISRuntime.UI.Controls;

namespace MapVisualizationApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        /**************程序加载及界面初始化****************/


        /**************************************************/

        /**********************界面***********************/
        public bool blOpenQueryDlg = false;
        private QueryDlg TemporalSpatioDlg;
        /**************************************************/


        /******************Neo4j数据库**********************/
        private string Neo4jIP;                 //Neo4j地址
        private string Neo4jPortNum;            //端口号
        private string Neo4jUser;               //用户名
        private string Neo4jPwd;                //密码

        //WKT多边形类型
        public enum WKTType { NULL, RECTANGLE, POLYGON, POINT };
        public WKTType TYPE = WKTType.NULL;

        public Dictionary<string, string> FieldsMap;
        public List<String[]> ListOfProcess;
        public HashSet<String> ListOfTime;

        public List<Esri.ArcGISRuntime.Geometry.Geometry> geometries;//不带属性的多边形，仅用于闪烁
        public Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>> ExtentGeo = new Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>>();//选择用于范围展示的多边形，带属性
        public Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>> StateGeo = new Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>>(); //选择标记状态的多边形，带属性
        private GraphicsOverlay AnimateGraphicsOverlay = new GraphicsOverlay(); //动画层
        private GraphicsOverlay StaticGraphicsOverlay = new GraphicsOverlay();  //静态属性层

        private int FlashTime = 0;
        private int FlashIndex = 0;
        private bool isFlash = false;
        System.Timers.Timer FlashTimer = new System.Timers.Timer();
        System.Timers.Timer AnimateTimer = new System.Timers.Timer();
        System.Timers.Timer LoadTimer = new System.Timers.Timer();


        public MainWindow()
        {
            LoadTimer.Enabled = true;
            /***************必须在创建web控件前初始化***************/
            CefSettings settings = new CefSettings();
            settings.CefCommandLineArgs.Add("disable-web-security", "1");
           
            Cef.Initialize(settings);
            /*******************************************************/          
            //IsAwaitShow = true;           
            InitializeComponent();                      
            InitalizeMap();
            Initialize();
            loadConfiguration();
        }


        //初始化
        public void InitalizeMap()
        {
            // Create new Map with basemap
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateImageryWithLabelsVector();
           // Assign the map to the MapView
            MyMapView.Map = myMap;

            MyMapView.Map.LoadStatusChanged += OnMapsLoadStatusChanged;
            MyMapView.GeoViewTapped += MapViewTapped;
            MyMapView.SelectionProperties.Color = System.Drawing.Color.Cyan;
            MyMapView.GraphicsOverlays.Add(StaticGraphicsOverlay);
            MyMapView.GraphicsOverlays.Add(AnimateGraphicsOverlay);

            SketchEditConfiguration config = MyMapView.SketchEditor.EditConfiguration;
            config.AllowVertexEditing = true;
            config.ResizeMode = SketchResizeMode.Uniform;
            config.AllowMove = true;
            DataContext = MyMapView.SketchEditor;          

        }

        private void Initialize()
        {
            slider.Minimuim = 0;
            AnimateTimer.Interval = 300;
            AnimateTimer.Elapsed += new System.Timers.ElapsedEventHandler(AnimateTimer_Tick);       
            
            //this.Topmost = true;
            button3.IsEnabled = false;
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
                   PUMessageBox.ShowDialog("配置文件加载出错:" + ex.Message, "错误");
                }

            }
        }


        //自定义函数
        public void SetStatuLabel(string info)
        {
            StatusInformationLabel.Content = info;
        }
        internal void SetProgessVisible(Visibility bl)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (bl == Visibility.Visible)
                {
                    //StatusProgressBar.Value = 0;
                    StatusProgressBar.IsRunning = true;
                }
                StatusProgressBar.Visibility = bl;

            }));
        }

        public void SetPlayBarVisible(bool bl)
        {
            if (bl)
            {
                buttonLeft.Visibility = Visibility.Visible;
                buttonRight.Visibility = Visibility.Visible;
                buttonPlay.Visibility = Visibility.Visible;
                buttonStop.Visibility = Visibility.Visible;                
                slider.Visibility = Visibility.Visible;
                labelStart.Visibility = Visibility.Visible;
                labelEnd.Visibility = Visibility.Visible;
                labelStart.Content = ListOfTime.ElementAt(0);
                labelSep.Visibility = Visibility.Visible;
                labelEnd.Content = ListOfTime.ElementAt(ListOfTime.Count - 1);
            }
            else
            {
                buttonLeft.Visibility = Visibility.Hidden;
                buttonRight.Visibility = Visibility.Hidden;
                buttonPlay.Visibility = Visibility.Hidden;
                buttonStop.Visibility = Visibility.Hidden;
                buttonPause.Visibility = Visibility.Hidden;
                slider.Visibility = Visibility.Hidden;
                labelStart.Visibility = Visibility.Hidden;
                labelSep.Visibility = Visibility.Hidden;
                labelEnd.Visibility = Visibility.Hidden;
            }
        }

        public void SetNeo4jConnection(string ip, string port, string user, string pwd)
        {
            Neo4jIP = ip;
            Neo4jPortNum = port;
            Neo4jUser = user;
            Neo4jPwd = pwd;
        }

        public void FlashShape(List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList)
        {
            if (!isFlash)
            {
                FlashTimer.Interval = 250; //执行间隔时间,单位为毫秒
                FlashTimer.Elapsed += new System.Timers.ElapsedEventHandler(FlashTimer_Tick);
                FlashTime = 0;
                FlashIndex = this.MyMapView.GraphicsOverlays.Count;
                geometries = GeoList;
                FlashTimer.Start();
                isFlash = true;
            }          
        }

        public void ShowExtentGeometry(Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>> ExtentGeo)
        {
            List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = new List<Esri.ArcGISRuntime.Geometry.Geometry>();//展示带属性信息的范围多边形
            for (int i = 0; i < ExtentGeo.Count; i++)
            {
                GeoList.Add(ExtentGeo.Keys.ToArray()[i]);
            }
            StaticGraphicsOverlay.Graphics.Clear();
            System.Drawing.Color color = System.Drawing.Color.FromArgb(35, 0, 0, 255);
            SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dash, System.Drawing.Color.FromArgb(255, 0, 0), 1.2);
            SimpleFillSymbol theSimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, simpleLineSymbol);
            //GraphicsOverlay theGraphicsOverlays = new GraphicsOverlay();
            // Get the graphic collection from the graphics overlay.
            GraphicCollection theGraphicCollection = StaticGraphicsOverlay.Graphics;
            for(int i = 0; i < GeoList.Count; i++)
            {
                theGraphicCollection.Add(new Graphic(GeoList[i], theSimpleFillSymbol));
            }
            // Add a graphic to the graphic collection - polygon with a simple fill symbol.
            Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(GeoList);
            //this.MyMapView.GraphicsOverlays.Add(theGraphicsOverlays);
            this.MyMapView.SetViewpointGeometryAsync(UnionGeo, 200);          
        } 

        public void ShowStatetGeometry(Dictionary<Esri.ArcGISRuntime.Geometry.Geometry, Dictionary<string, string>> StatetGeo)    //展示带属性信息的状态多边形
        {
            List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = new List<Esri.ArcGISRuntime.Geometry.Geometry>();
            for (int i = 0; i < StatetGeo.Count; i++)
            {
                GeoList.Add(StatetGeo.Keys.ToArray()[i]);
            }
            StaticGraphicsOverlay.Graphics.Clear();
            System.Drawing.Color color = System.Drawing.Color.FromArgb(35, 255, 0, 0);
            SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 0, 0), 1.2);
            SimpleFillSymbol theSimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, simpleLineSymbol);
            // Get the graphic collection from the graphics overlay.
            GraphicCollection theGraphicCollection = StaticGraphicsOverlay.Graphics;
            for (int i = 0; i < GeoList.Count; i++)
            {
                theGraphicCollection.Add(new Graphic(GeoList[i], theSimpleFillSymbol));
            }
            // Add a graphic to the graphic collection - polygon with a simple fill symbol.
            Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(GeoList);
            this.MyMapView.SetViewpointGeometryAsync(UnionGeo, 200);
        }

        public void ShowGeometrys(List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList)
        {
            AnimateGraphicsOverlay.Graphics.Clear();
            System.Drawing.Color color = System.Drawing.Color.FromArgb(80, 255, 0, 0);
            SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Dot, System.Drawing.Color.FromArgb(255, 0, 0), 1);
            SimpleFillSymbol theSimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, simpleLineSymbol);
            GraphicCollection theGraphicCollection = AnimateGraphicsOverlay.Graphics;
            for (int i = 0; i < GeoList.Count; i++)
            {
                theGraphicCollection.Add(new Graphic(GeoList[i], theSimpleFillSymbol));
            }            
        }

        public void LoadWeb(string url)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                browser.Address = url;
            }));
        }

        //事件响应

        private void OnMapsLoadStatusChanged(object sender, LoadStatusEventArgs e)
        {

            Dispatcher.Invoke(new Action(delegate
            {

                if (e.Status.ToString() == "Loaded")
                {
                    this.IsAwaitShow = false;
                } 
                else if (e.Status.ToString()=="FailedToLoad")
                {                    
                    this.IsAwaitShow = false;
                    string currentPath = System.Windows.Forms.Application.ExecutablePath;
                    currentPath = currentPath.Replace("/", "\\");
                    currentPath = currentPath.Substring(0, currentPath.LastIndexOf("bin"));
                    Uri serviceOfflineUri = new Uri(currentPath + "Data\\SimpleOffline.vtpk");
                    ArcGISVectorTiledLayer arcGISVectorTiledLayer = new ArcGISVectorTiledLayer(serviceOfflineUri);
                    // Create new tiled layer from the url
                    MyMapView.Map = new Map();
                    MyMapView.Map.Basemap.BaseLayers.Add(arcGISVectorTiledLayer);
                    MyMapView.Map.InitialViewpoint = new Viewpoint(arcGISVectorTiledLayer.FullExtent.Extent);
                    PUMessageBox.ShowDialog("地图服务加载失败，已加载离线地图");
                }
                else
                {
                    this.IsAwaitShow = true;
                }
            }));
        }

        private async void MapViewTapped(object sender, GeoViewInputEventArgs geoViewInputEventArgs)
        {
            Point pt = geoViewInputEventArgs.Position;
            
            if (MyMapView.GraphicsOverlays.Count > 0)
            {
                StaticGraphicsOverlay.ClearSelection();
                IdentifyGraphicsOverlayResult result = await MyMapView.IdentifyGraphicsOverlayAsync(StaticGraphicsOverlay, geoViewInputEventArgs.Position, 1, false);
                // Return if there are no results
                if (result.Graphics.Count < 1)
                {
                    puBubble.Visibility = Visibility.Hidden;
                    MainWindowContextMenu.Visibility = Visibility.Hidden;
                    MainWindowContextMenu.IsOpen = false;
                    return;
                }
                MainWindowContextMenu.Visibility = Visibility.Visible;
                MainWindowContextMenu.IsOpen = false;
                puBubble.Visibility = Visibility.Visible;
                Graphic identifiedGraphic = result.Graphics.First();
                // Clear any existing selection, then select the tapped graphic         
                identifiedGraphic.IsSelected = true;
                //puBubble.Margin = new Thickness(pt.X - 0.5 * puBubble.ActualWidth, pt.Y - puBubble.ActualHeight, 0, 0);
                if (ExtentGeo.Keys.Contains(identifiedGraphic.Geometry))
                {
                    Dictionary<string, string> info = ExtentGeo[identifiedGraphic.Geometry];
                    textBox.Text = string.Empty;
                    textBox.Text += "事件信息:\r\n";
                    for (int i = 0; i < info.Count; i++)
                    {
                        if (info.Keys.ToArray()[i].Contains("WKT") || info.Keys.ToArray()[i].Contains("覆盖范围") || info.Keys.ToArray()[i].Contains("几何类型") || info.Keys.ToArray()[i].Contains("外接")) continue;
                        textBox.Text += "   " + info.Keys.ToArray()[i] + ":" + info.Values.ToArray()[i] + "\r\n";
                    }
                }
                else if(StateGeo.Keys.Contains(identifiedGraphic.Geometry))
                {
                    Dictionary<string, string> info = StateGeo[identifiedGraphic.Geometry];
                    textBox.Text = string.Empty;
                    textBox.Text += "状态信息:\r\n";
                    for (int i = 0; i < info.Count; i++)
                    {
                        if (info.Keys.ToArray()[i].Contains("WKT") || info.Keys.ToArray()[i].Contains("覆盖范围") || info.Keys.ToArray()[i].Contains("几何类型") || info.Keys.ToArray()[i].Contains("外接")) continue;
                        textBox.Text += "   "+info.Keys.ToArray()[i] + ":" + info.Values.ToArray()[i] + "\r\n";
                    }
                }
                
            }
        }

        public void FlashTimer_Tick(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {

                if (MyMapView.GraphicsOverlays.Count > FlashIndex)
                {
                    this.MyMapView.GraphicsOverlays[FlashIndex] = new GraphicsOverlay();
                }
                else
                {
                    this.MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());
                }



                System.Drawing.Color color = System.Drawing.Color.FromArgb(60, 0, 255, 255);
                //SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, System.Drawing.Color.FromArgb(255, 255, 255), 1.5);
                SimpleFillSymbol theSimpleFillSymbol = new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, color, null);
                GraphicsOverlay theGraphicsOverlays = new GraphicsOverlay();
                // Get the graphic collection from the graphics overlay.
                GraphicCollection theGraphicCollection = theGraphicsOverlays.Graphics;

                // Add a graphic to the graphic collection - polygon with a simple fill symbol.
                Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(geometries);
                theGraphicCollection.Add(new Graphic(UnionGeo, theSimpleFillSymbol));
                //this.MyMapView.GraphicsOverlays.Add(theGraphicsOverlays);
                try
                {
                    this.MyMapView.GraphicsOverlays[FlashIndex] = theGraphicsOverlays;
                }
                catch
                {

                }
                

                FlashTime++;
                if (FlashTime == 6)
                {
                    FlashTimer.Stop();
                    FlashTimer = new System.Timers.Timer();
                    //Dispatcher.Invoke(new Action(delegate
                    //{
                    isFlash = false;
                    this.MyMapView.GraphicsOverlays.RemoveAt(FlashIndex);
                    //}));
                }

            }));
        }

        public void AnimateTimer_Tick(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (slider.Value < slider.Maximuim)
                    slider.Value += 1;
                else
                {
                    buttonPause.Visibility = Visibility.Hidden;
                    buttonPlay.Visibility = Visibility.Visible;
                    AnimateTimer.Enabled = false;
                    //slider.Value = slider.Minimuim;
                    //AnimateGraphicsOverlay.Graphics.Clear();
                }
            })); 
               

        }

        private void MyMapView_MouseMove(object sender, MouseEventArgs e)
        {
            String LocationTemplate = "经度:{0} 纬度:{1}";
            var _currentLatLng = MyMapView.ScreenToLocation(e.GetPosition(MyMapView));
            if (_currentLatLng != null)
            {
                StatusLocationLabel.Visibility = Visibility.Visible;
                var normalizedPoint = GeometryEngine.NormalizeCentralMeridian(_currentLatLng);
                var projectedCenter = GeometryEngine.Project(normalizedPoint, SpatialReferences.Wgs84) as MapPoint;
                StatusLocationLabel.Content = String.Format(LocationTemplate, projectedCenter.X.ToString(), projectedCenter.Y.ToString());
            }

        }

        private void button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (button.IsEnabled == true)
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/FINISH_ENABLE_12px.png", UriKind.Relative)));
                EnableBr.Stretch = Stretch.None;
                button.Background = EnableBr;
            }
            else
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/FINISH_DISABLE_12px.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.None;
                button.Background = DisableBr;
            }
        }

        private void button1_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (button1.IsEnabled == true)
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/UNDO_ENABLE_12px.png", UriKind.Relative)));
                EnableBr.Stretch = Stretch.None;
                button1.Background = EnableBr;
            }
            else
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/UNDO_DISABLE_12px.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.None;
                button1.Background = DisableBr;
            }
        }

        private void button2_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (button2.IsEnabled == true)
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/REDO_ENABLE_12px.png", UriKind.Relative)));
                EnableBr.Stretch = Stretch.None;
                button2.Background = EnableBr;
            }
            else
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/REDO_DISABLE_12px.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.None;
                button2.Background = DisableBr;
            }
        }

        private void Button3_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (button3.IsEnabled == true)
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/ERASER_ENABLE_12px.png", UriKind.Relative)));
                EnableBr.Stretch = Stretch.None;
                button3.Background = EnableBr;
            }
            else
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/ERASER_DISABLE_12px.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.None;
                button3.Background = DisableBr;
            }
        }


        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (slider.Value == slider.Minimuim) slider.Value = slider.Minimuim;
            else slider.Value -= 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            slider.Maximuim = ListOfTime.Count - 1;
            if (slider.Value < 0) slider.Value = slider.Maximuim;
            if (slider.Value > slider.Maximuim) slider.Value = slider.Maximuim;
            if (slider.Value >= slider.Minimuim && slider.Value <= slider.Maximuim)
            {
                List<Esri.ArcGISRuntime.Geometry.Geometry> geo = new List<Esri.ArcGISRuntime.Geometry.Geometry>();


                for (int i = 0; i < ListOfProcess[(int)slider.Value].Length; i++)
                {
                    geo.Add(GeometryEngine.Union(Convertor.Wkt2Geometry(ListOfProcess[(int)slider.Value][i], 4326, 4326)));
                }
                ShowGeometrys(geo);
                labelStart.Content = ListOfTime.ElementAt((int)slider.Value);
            }

        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            if (slider.Value == slider.Maximuim) slider.Value = slider.Minimuim;
            else slider.Value += 1;
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            //pause
            //AnimateTimer.Enabled = false;
            AnimateTimer.Stop();
            buttonPause.Visibility = Visibility.Hidden;
            buttonPlay.Visibility = Visibility.Visible;
            //buttonLeft.IsEnabled = true;
            //buttonRight.IsEnabled = true;
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            //play
            buttonPlay.Visibility = Visibility.Hidden;
            buttonPause.Visibility = Visibility.Visible;
            //buttonLeft.IsEnabled = false;
            //buttonRight.IsEnabled = false;
            //AnimateTimer.Enabled = true;          
            AnimateTimer.Start();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            //stop
            //AnimateTimer.Enabled = false;
            //AnimateTimer.Stop();
            //slider.Value = slider.Minimuim;
            //buttonPlay.Visibility = Visibility.Visible;
            //buttonPause.Visibility = Visibility.Hidden;
            //labelStart.Content = ListOfTime.ElementAt(0);
            //buttonLeft.IsEnabled = true;
            //buttonRight.IsEnabled = true;
            AnimateTimer.Stop();
            SetPlayBarVisible(false);
            AnimateTimer.Enabled = false;
            button3.IsEnabled = false;
            browser.Address = "about:blank";
            puBubble.Visibility = Visibility.Hidden;
            this.AnimateGraphicsOverlay.Graphics.Clear();
            AnimateGraphicsOverlay.Graphics.Clear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (TemporalSpatioDlg != null) TemporalSpatioDlg.Close();
            System.Environment.Exit(0);
        }

        private void PUTreeViewItem_Connect(object sender, MouseButtonEventArgs e)
        {
            ConnectDB connectDB = new ConnectDB(this);
            connectDB.ShowDialog();
        }

        private void PUTreeViewItem_DisConnect(object sender, MouseButtonEventArgs e)
        {
            if (Neo4j64.isConnected)
            {
                Neo4j64.Neo4jClose();
                StatusInformationLabel.Content = "数据库已关闭";
            }
        }
        private void PUTreeViewItem_Query(object sender, MouseButtonEventArgs e)
        {
            if (Neo4j64.isConnected && !blOpenQueryDlg)
            {
                TemporalSpatioDlg = new QueryDlg(this);
                TemporalSpatioDlg.Show();
                blOpenQueryDlg = true;
            }
            if (!Neo4j64.isConnected)
            {
                PUMessageBox.ShowDialog("未连接Neo4j数据库!", "错误", Buttons.Yes, false);
                return;
            }
            if (TemporalSpatioDlg != null)
            {
                //TemporalSpatioDlg.Topmost = true;
                //this.Topmost = false;
                TemporalSpatioDlg.WindowState = WindowState.Normal;
                TemporalSpatioDlg.Focus();
            }
        }


        private void PUTreeViewItem_PreviewMouseLeftButton_1(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateStreets();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_2(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateStreetsVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_3(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateStreetsNightVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_4(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateImagery();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_5(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateImageryWithLabels();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_6(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateImageryWithLabelsVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_7(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateDarkGrayCanvasVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_8(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateLightGrayCanvas();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_9(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateLightGrayCanvasVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_10(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateNavigationVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_11(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateOpenStreetMap();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_12(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateOceans();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_13(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateTopographic();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_14(object sender, MouseButtonEventArgs e)
        {
            Map myMap = new Map();
            myMap.Basemap = Basemap.CreateTopographicVector();
            // Assign the map to the MapView
            MyMapView.Map = myMap;
        }

        private void PUTreeViewItem_PreviewMouseLeftButton_15(object sender, MouseButtonEventArgs e)
        {
            string currentPath = System.Windows.Forms.Application.ExecutablePath;
            currentPath = currentPath.Replace("/", "\\");
            currentPath = currentPath.Substring(0, currentPath.LastIndexOf("bin"));
            Uri serviceOfflineUri = new Uri(currentPath + "Data\\SimpleOffline.vtpk");
            ArcGISVectorTiledLayer arcGISVectorTiledLayer = new ArcGISVectorTiledLayer(serviceOfflineUri);
            // Create new tiled layer from the url
            MyMapView.Map = new Map();
            MyMapView.Map.Basemap.BaseLayers.Add(arcGISVectorTiledLayer);
            MyMapView.Map.InitialViewpoint = new Viewpoint(arcGISVectorTiledLayer.FullExtent.Extent);
        }

        private void PUWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            string url = browser.Address;
            browser.Address = "about:blank";
            browser.Address = url;

        }

        private void PUTreeViewItem_About(object sender, MouseButtonEventArgs e)
        {
            AboutUs about = new AboutUs();
            about.ShowDialog();          
        }
            
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            SetPlayBarVisible(false);
            AnimateTimer.Enabled = false;
            button3.IsEnabled = false;
            browser.Address = "about:blank";
            puBubble.Visibility = Visibility.Hidden;
            this.AnimateGraphicsOverlay.Graphics.Clear();
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            StaticGraphicsOverlay.Graphics.Clear();
            puBubble.Visibility = Visibility.Hidden;
        }

        private void PUContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (StaticGraphicsOverlay.SelectedGraphics.Count() > 0)
            {
                StaticGraphicsOverlay.Graphics.Remove(StaticGraphicsOverlay.SelectedGraphics.ElementAt(0));
                puBubble.Visibility = Visibility.Hidden;
            }
            MainWindowContextMenu.IsOpen = false;
            MainWindowContextMenu.Visibility = Visibility.Hidden;
        }

        private void PUTreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserGuideDlg userGuideDlg = new UserGuideDlg();
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            //获取运行目录的文档
            userGuideDlg.browser.Address = path+"\\Document\\neo4j-cypher-manual-3.5.pdf";
            userGuideDlg.Show();
        }

        private void PUTreeViewItem_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            UserGuideDlg userGuideDlg = new UserGuideDlg();
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            userGuideDlg.browser.Address = path + "\\Document\\用户指南.pdf";
            userGuideDlg.Show();
        }

        private void PUTreeViewItem_PreviewMouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            if (!Neo4j64.isConnected) {
                if (PUMessageBox.ShowConfirm("未连接数据库仅部分功能可用,是否连接数据库？")==true){
                    ConnectDB connectDB = new ConnectDB(this);
                    connectDB.ShowDialog();
                    if (Neo4j64.isConnected)
                    {
                        if (Neo4j64.isAdminRole)
                        {
                            DBManagementDlg dBManagementDlg = new DBManagementDlg();
                            dBManagementDlg.Show();
                        }
                        else
                        {
                            PUMessageBox.ShowDialog("当前用户无权限！");
                            return;
                        }                      
                    }                                  
                }
                else
                {
                    DBManagementDlg dBManagementDlg = new DBManagementDlg();
                    PUTabItem rmTab1 = dBManagementDlg.OptionTab.Items[1] as PUTabItem;
                    PUTabItem rmTab2= dBManagementDlg.OptionTab.Items[2] as PUTabItem;

                    PUTabItem rmTab3 = dBManagementDlg.ProcessTab.Items[1] as PUTabItem;
                    PUTabItem rmTab4 = dBManagementDlg.ProcessTab.Items[2] as PUTabItem;
                    PUTabItem rmTab5 = dBManagementDlg.ProcessTab.Items[3] as PUTabItem;
                    PUTabItem rmTab6 = dBManagementDlg.ImportTabl.Items[0] as PUTabItem;
                    dBManagementDlg.OptionTab.Items.Remove(rmTab1);
                    dBManagementDlg.OptionTab.Items.Remove(rmTab2);
                    dBManagementDlg.ProcessTab.Items.Remove(rmTab3);
                    dBManagementDlg.ProcessTab.Items.Remove(rmTab4);
                    dBManagementDlg.ProcessTab.Items.Remove(rmTab5);
                    dBManagementDlg.ImportTabl.Items.Remove(rmTab6);
                    dBManagementDlg.Show();

                }
            }
            else
            {
                if (Neo4j64.isAdminRole)
                {
                    DBManagementDlg dBManagementDlg = new DBManagementDlg();
                    dBManagementDlg.Show();
                }
                else
                {
                    PUMessageBox.ShowDialog("当前用户无权限！");
                    return;
                }
            }
            
        }

    }
}
