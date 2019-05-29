using Esri.ArcGISRuntime.UI;
using MarineSTMiningSystem;
using MarineSTMiningSystem.DataOP;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Panuon.UI;
using MapVisualizationApp.DataOP;

namespace MapVisualizationApp.GUI
{
    /// <summary>
    /// QueryDlg.xaml 的交互逻辑
    /// </summary>
    public partial class QueryDlg 
    {
        public MainWindow mainForm = null;
        private bool isInsert = false;
        private int indexInsert = 0;
        //private List<Dictionary<string, string>> Nodes;
        private string TimeCondition = string.Empty;
        private string SpaceCondition = string.Empty;
        private string WKTPolygon = string.Empty;
        private string WKTPoint = string.Empty;
        private string Layer = string.Empty;
        private string OrderType = "ASC";
        public List<FeatureTable> FeatureTableList = new List<FeatureTable>();
        private enum QueryType
        {
            /***********************************************/
            /*属性查询*/
            FeatureAttributes,                     //要素属性
            /***********************************************/
            /*时间相关*/
            QUARTER,                               //季度
            YEAR,                                  //年份
            MONTH,                                 //月份
            DAY,                                   //日分
            TIME,                                  //时刻
            DURATION,                              //时间段
            ENSO,                                  //ENSO  
            /***********************************************/
            /*空间相关*/
            Rectangle,                             //矩形
            Circle,                                //圆形
            POINT,                                 //点
            POLYGON,                               //多边形
            //AdDivision,                          //行政区划
            //RIVER,                               //河流
            /***********************************************/
            NONE
        };
        QueryType TIMETYPE = QueryType.NONE;
        QueryType SPACETYPE = QueryType.NONE;
        private string strStartDateTime = "{0}-{1}-{2}T{3}";
        private string strEndDateTime = "{0}-{1}-{2}T{3}";
        private bool loadCQL = false;
        public QueryDlg(MainWindow mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            SortComboBox.SelectedIndex = 0;
            SketchEditConfiguration config = mainForm.MyMapView.SketchEditor.EditConfiguration;
            
            config.AllowVertexEditing = true;
            config.ResizeMode = SketchResizeMode.Uniform;
            config.AllowMove = true;
            DataContext = mainForm.MyMapView.SketchEditor;
            
        }

        //自定义函数
        private void UpdateFileds(object NodeType)
        {
            List<String> FeaturesFileds = new List<String>();
            FeaturesFileds = Neo4j64.QueryFeatureFields(NodeType.ToString());
            FeaturesFileds.Sort();
            if (FeaturesFileds != null)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    for (int i = 0; i < FeaturesFileds.Count; i++)
                    {
                        PUListBoxItem item = new PUListBoxItem();
                        PUComboBoxItem citem = new PUComboBoxItem();
                        item.Content = FeaturesFileds[i];
                        citem.Content = FeaturesFileds[i];
                        listBox1.Items.Add(item);
                        SortComboBox.Items.Add(citem);
                    }
                }));
            }
            List<String> tempLayers = Neo4j64.QueryLayers();
            Layer = string.Empty;
            string strTemp = NodeType.ToString();
            if (strTemp.Contains("Node") || strTemp.Contains("NODE"))
            {
                strTemp = strTemp.Substring(0, strTemp.Length - 4);
            }
            for (int i = 0; i < tempLayers.Count; i++)
            {
                if (tempLayers[i].Contains(strTemp))
                {
                    Layer = tempLayers[i];
                }
            }
            Dispatcher.Invoke(new Action(delegate
            {
                if (AdditionalCheckBox.IsChecked == true&&TimeCheckBox.IsChecked == true)
                {
                    StartFieldComboBox.Items.Clear();
                    EndFieldComboBox.Items.Clear();
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        if ((listBox1.Items[i] as PUListBoxItem).Content.ToString().Contains("ETime") || (listBox1.Items[i] as PUListBoxItem).Content.ToString().Contains("STime"))
                        {
                            PUComboBoxItem tempItem1 = new PUComboBoxItem();
                            PUComboBoxItem tempItem2 = new PUComboBoxItem();
                            tempItem1.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                            tempItem2.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                            StartFieldComboBox.Items.Add(tempItem1);
                            EndFieldComboBox.Items.Add(tempItem2);
                        }
                    }
                    if (StartFieldComboBox.Items.Count >= 2)
                    {
                        StartFieldComboBox.SelectedItem = StartFieldComboBox.Items[1];
                        EndFieldComboBox.SelectedItem = EndFieldComboBox.Items[0];
                    }
                    if (StartFieldComboBox.Items.Count == 0)
                    {
                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            PUComboBoxItem tempItem1 = new PUComboBoxItem();
                            PUComboBoxItem tempItem2 = new PUComboBoxItem();
                            tempItem1.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                            tempItem2.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                            StartFieldComboBox.Items.Add(tempItem1);
                            EndFieldComboBox.Items.Add(tempItem2);
                        }
                    }
                }
            }));
        }

        private void UpdateValuesThread()
        {
            string NodeType = string.Empty;
            string Key = string.Empty;
            int cbIndex = -1;
            int lsIndex = -1;
            Dispatcher.Invoke(new Action(delegate
            {
                NodeType = comboBox1.SelectedValue.ToString();
                Key = listBox1.SelectedValue.ToString();
                cbIndex = comboBox1.SelectedIndex;
                lsIndex = listBox1.SelectedIndex;
                listBox2.Items.Clear();
            }));



            if (cbIndex != -1 && lsIndex != -1)
            {
                List<String> Values = new List<String>();
                Values = Neo4j64.QueryDistinctVale(NodeType, Key);
                Dispatcher.Invoke(new Action(delegate
                {
                    string dataType = string.Empty;
                    if (mainForm.FieldsMap.Keys.Contains(listBox1.SelectedValue.ToString()))
                    {
                        dataType = mainForm.FieldsMap[listBox1.SelectedValue.ToString()].Split(',')[0];
                    }
                    if (dataType.ToUpper() == "STRING" || dataType.ToUpper() == "DATETIME")
                    {
                        List<string> tempList = new List<string>();
                        for (int i = 0; i < Values.Count; i++)
                        {
                            tempList.Add("\"" + Values[i] + "\"");
                        }
                        tempList.Sort();
                        for (int i = 0; i < Values.Count; i++)
                        {
                            listBox2.Items.Add(tempList[i]);
                        }

                    }
                    else
                    {
                        List<double> tempList = new List<double>();
                        for (int i = 0; i < Values.Count; i++)
                        {
                            try
                            {
                                tempList.Add(Convert.ToDouble(Values[i]));
                            }
                            catch
                            {
                                listBox2.Items.Add(Values[i]);
                            }

                        }
                        tempList.Sort();
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            listBox2.Items.Add(tempList[i]);
                        }
                    }

                }));
            }
  
        }

        private int WhereUpdate(string buttonText, int indexInsert)
        {
            try
            {
                string strTemp = WhereTextBox.Text;
                strTemp = strTemp.Insert(indexInsert, buttonText + " ");
                WhereTextBox.Text = strTemp;
                if (!isInsert) return WhereTextBox.Text.Length;
                else return indexInsert + buttonText.Length + 1;
            }
            catch
            {
                return indexInsert;
            }

        }

        private void UpdateGUI()
        {
            switch (TIMETYPE)
            {
                case QueryType.DURATION:
                    {
                        int sy, sm, sd, st, ey, em, ed, et;
                        sy = comboBoxStartYear.SelectedIndex;
                        sm = comboBoxStartMonth.SelectedIndex;
                        sd = comboBoxStartDay.SelectedIndex;
                        st = comboBoxStartTime.SelectedIndex;

                        ey = comboBoxEndYear.SelectedIndex;
                        em = comboBoxEndMonth.SelectedIndex;
                        ed = comboBoxEndDay.SelectedIndex;
                        et = comboBoxEndTime.SelectedIndex;

                        comboBoxQuarter.SelectedIndex = -1;
                        comboBoxMonth.SelectedIndex = -1;
                        comboBoxDay.SelectedIndex = -1;
                        comboBoxTime.SelectedIndex = -1;

                        comboBoxQuarter.Text = string.Empty;
                        comboBoxMonth.Text = string.Empty;
                        comboBoxDay.Text = string.Empty;
                        comboBoxTime.Text = string.Empty;


                        comboBoxStartYear.SelectedIndex = sy;
                        comboBoxStartMonth.SelectedIndex = sm;
                        comboBoxStartDay.SelectedIndex = sd;
                        comboBoxStartTime.SelectedIndex = st;

                        comboBoxEndYear.SelectedIndex = ey;
                        comboBoxEndMonth.SelectedIndex = em;
                        comboBoxEndDay.SelectedIndex = ed;
                        comboBoxEndTime.SelectedIndex = et;

                        strStartDateTime = string.Format("{0}-{1}-{2}T{3}",
                                comboBoxStartYear.SelectedIndex == -1 ? (comboBoxStartYear.Items[0] as PUComboBoxItem).Content : (comboBoxStartYear.SelectedItem as PUComboBoxItem).Content,
                                comboBoxStartMonth.SelectedIndex == -1 ? (comboBoxStartMonth.Items[0] as PUComboBoxItem).Content : (comboBoxStartMonth.SelectedItem as PUComboBoxItem).Content,
                                comboBoxStartDay.SelectedIndex == -1 ? (comboBoxStartDay.Items[0] as PUComboBoxItem).Content : (comboBoxStartDay.SelectedItem as PUComboBoxItem).Content,
                                comboBoxStartTime.SelectedIndex == -1 ? (comboBoxStartTime.Items[0] as PUComboBoxItem).Content : (comboBoxStartTime.SelectedItem as PUComboBoxItem).Content);

                        strEndDateTime = string.Format("{0}-{1}-{2}T{3}",
                              comboBoxEndYear.SelectedIndex == -1 ? (comboBoxEndYear.Items[0] as PUComboBoxItem).Content : (comboBoxEndYear.SelectedItem as PUComboBoxItem).Content,
                              comboBoxEndMonth.SelectedIndex == -1 ? (comboBoxEndMonth.Items[0] as PUComboBoxItem).Content : (comboBoxEndMonth.SelectedItem as PUComboBoxItem).Content,
                              comboBoxEndDay.SelectedIndex == -1 ? (comboBoxEndDay.Items[0] as PUComboBoxItem).Content : (comboBoxEndDay.SelectedItem as PUComboBoxItem).Content,
                              comboBoxEndTime.SelectedIndex == -1 ? (comboBoxEndTime.Items[0] as PUComboBoxItem).Content : (comboBoxEndTime.SelectedItem as PUComboBoxItem).Content);
                        break;
                    }
                case QueryType.QUARTER:
                    {
                        int QuaterIndex = comboBoxQuarter.SelectedIndex;
                        comboBoxStartYear.Text = string.Empty;
                        comboBoxStartMonth.Text = string.Empty;
                        comboBoxStartDay.Text = string.Empty;
                        comboBoxStartTime.Text = string.Empty;

                        comboBoxEndYear.Text = string.Empty;
                        comboBoxEndMonth.Text = string.Empty;
                        comboBoxEndDay.Text = string.Empty;
                        comboBoxEndTime.Text = string.Empty;

                        comboBoxMonth.Text = string.Empty;
                        comboBoxDay.Text = string.Empty;
                        comboBoxTime.Text = string.Empty;


                        comboBoxStartYear.SelectedIndex = -1;
                        comboBoxStartMonth.SelectedIndex = -1;
                        comboBoxStartDay.SelectedIndex = -1;
                        comboBoxStartTime.SelectedIndex = -1;

                        comboBoxEndYear.SelectedIndex = -1;
                        comboBoxEndMonth.SelectedIndex = -1;
                        comboBoxEndDay.SelectedIndex = -1;
                        comboBoxEndTime.SelectedIndex = -1;

                        comboBoxMonth.SelectedIndex = -1;
                        comboBoxDay.SelectedIndex = -1;
                        comboBoxTime.SelectedIndex = -1;

                        comboBoxQuarter.SelectedIndex = QuaterIndex;

                        break;
                    }
                case QueryType.MONTH:
                    {
                        int MonthIndex = comboBoxMonth.SelectedIndex;
                        comboBoxStartYear.SelectedIndex = -1;
                        comboBoxStartMonth.SelectedIndex = -1;
                        comboBoxStartDay.SelectedIndex = -1;
                        comboBoxStartTime.SelectedIndex = -1;

                        comboBoxEndYear.SelectedIndex = -1;
                        comboBoxEndMonth.SelectedIndex = -1;
                        comboBoxEndDay.SelectedIndex = -1;
                        comboBoxEndTime.SelectedIndex = -1;

                        comboBoxQuarter.SelectedIndex = -1;
                        comboBoxDay.SelectedIndex = -1;
                        comboBoxTime.SelectedIndex = -1;


                        comboBoxStartYear.Text = string.Empty;
                        comboBoxStartMonth.Text = string.Empty;
                        comboBoxStartDay.Text = string.Empty;
                        comboBoxStartTime.Text = string.Empty;

                        comboBoxEndYear.Text = string.Empty;
                        comboBoxEndMonth.Text = string.Empty;
                        comboBoxEndDay.Text = string.Empty;
                        comboBoxEndTime.Text = string.Empty;

                        comboBoxQuarter.Text = string.Empty;
                        comboBoxDay.Text = string.Empty;
                        comboBoxTime.Text = string.Empty;
                        comboBoxMonth.SelectedIndex = MonthIndex;
                        break;
                    }
                case QueryType.DAY:
                    {
                        int DayIndex = comboBoxDay.SelectedIndex;
                        comboBoxStartYear.SelectedIndex = -1;
                        comboBoxStartMonth.SelectedIndex = -1;
                        comboBoxStartDay.SelectedIndex = -1;
                        comboBoxStartTime.SelectedIndex = -1;

                        comboBoxEndYear.SelectedIndex = -1;
                        comboBoxEndMonth.SelectedIndex = -1;
                        comboBoxEndDay.SelectedIndex = -1;
                        comboBoxEndTime.SelectedIndex = -1;

                        comboBoxQuarter.SelectedIndex = -1;
                        comboBoxMonth.SelectedIndex = -1;
                        comboBoxTime.SelectedIndex = -1;

                        comboBoxStartYear.Text = string.Empty;
                        comboBoxStartMonth.Text = string.Empty;
                        comboBoxStartDay.Text = string.Empty;
                        comboBoxStartTime.Text = string.Empty;

                        comboBoxEndYear.Text = string.Empty;
                        comboBoxEndMonth.Text = string.Empty;
                        comboBoxEndDay.Text = string.Empty;
                        comboBoxEndTime.Text = string.Empty;

                        comboBoxQuarter.Text = string.Empty;
                        comboBoxMonth.Text = string.Empty;
                        comboBoxTime.Text = string.Empty;

                        comboBoxDay.SelectedIndex = DayIndex;
                        break;
                    }
                case QueryType.TIME:
                    {
                        int TimeIndex = comboBoxTime.SelectedIndex;
                        comboBoxStartYear.SelectedIndex = -1;
                        comboBoxStartMonth.SelectedIndex = -1;
                        comboBoxStartDay.SelectedIndex = -1;
                        comboBoxStartTime.SelectedIndex = -1;

                        comboBoxEndYear.SelectedIndex = -1;
                        comboBoxEndMonth.SelectedIndex = -1;
                        comboBoxEndDay.SelectedIndex = -1;
                        comboBoxEndTime.SelectedIndex = -1;

                        comboBoxQuarter.SelectedIndex = -1;
                        comboBoxMonth.SelectedIndex = -1;
                        comboBoxDay.SelectedIndex = -1;

                        comboBoxStartYear.Text = string.Empty;
                        comboBoxStartMonth.Text = string.Empty;
                        comboBoxStartDay.Text = string.Empty;
                        comboBoxStartTime.Text = string.Empty;

                        comboBoxEndYear.Text = string.Empty;
                        comboBoxEndMonth.Text = string.Empty;
                        comboBoxEndDay.Text = string.Empty;
                        comboBoxEndTime.Text = string.Empty;

                        comboBoxQuarter.Text = string.Empty;
                        comboBoxMonth.Text = string.Empty;
                        comboBoxDay.Text = string.Empty;

                        comboBoxTime.SelectedIndex = TimeIndex;
                        break;
                    }

            }
        }

        private void UpdateNodes(object CQLS)
        {
  
            string CQL = ((string[])CQLS)[0];
            string CountCQL = ((string[])CQLS)[1]; 
            List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
            Dictionary<string, string> EventGeometries = new Dictionary<string, string>();
            //转换为DataTable
            DataTable dataTable = new DataTable("Dataset");
            try
            {
                Nodes = Neo4j64.QueryNodeDataTable((string)CQL);
                for(int i = 0; i < Nodes.Count; i++)
                {
                    EventGeometries.Add(Nodes[i]["PRID"], Nodes[i]["geometry"]);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    mainForm.SetProgessVisible(Visibility.Hidden);
                    PUMessageBox.ShowDialog("查询错误: " + ex.Message, "错误", Buttons.OK);
                    return;
                }));
            }
            List<String> Keys = new List<String>();
            Dispatcher.Invoke(new Action(delegate
            {              
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        Keys.Add((listBox1.Items[i] as PUListBoxItem).Content.ToString());
                    }
                }
            }));
            List<List<string>> CountNodes=Neo4j64.QueryNonNodeDataTable(CountCQL);
            if (Nodes != null)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    dataTable = Convertor.MapNode2DataTable(Nodes, mainForm.FieldsMap, Keys);
                    FeatureTable tableForm = new FeatureTable(this, dataTable, comboBox1.SelectedValue.ToString(),CQL.ToString(),Convert.ToInt32(CountNodes[0][0]),EventGeometries);
                    FeatureTableList.Add(tableForm);
                    mainForm.SetProgessVisible(Visibility.Hidden);
                    tableForm.Show();
                }));
            }
            else
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    mainForm.SetProgessVisible(Visibility.Hidden);
                    PUMessageBox.ShowDialog("未查询到数据！", "提示", Buttons.OK);
                }));
            }
        }

        //事件响应
          
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button1.Content.ToString(), indexInsert);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button2.Content.ToString(), indexInsert);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button3.Content.ToString(), indexInsert);
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button4.Content.ToString(), indexInsert);
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button5.Content.ToString(), indexInsert);
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button6.Content.ToString(), indexInsert);
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button7.Content.ToString(), indexInsert);
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button8.Content.ToString(), indexInsert);
        }

        private void button9_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button9.Content.ToString(), indexInsert);
        }

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button10.Content.ToString(), indexInsert);
        }

        private void button11_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button11.Content.ToString(), indexInsert);
        }

        private void button12_Click(object sender, RoutedEventArgs e)
        {
            indexInsert = WhereUpdate(button12.Content.ToString(), indexInsert);
        }

        private void button13_Click(object sender, RoutedEventArgs e)
        {
            Thread updateUniqueValueThread = new Thread(UpdateValuesThread);
            updateUniqueValueThread.Start();
        }
        
        private void button14_Click(object sender, RoutedEventArgs e)
        {
            string CQL = matchLabel.Content.ToString().Substring(0, matchLabel.Content.ToString().LastIndexOf('.') - 8) + " ";
            if (WhereTextBox.Text.Length != 0)
            {
                CQL += "WHERE ";
                CQL += WhereTextBox.Text;
                CQL += " RETURN NODE limit 1";
                try
                {
                    Neo4j64.QueryNodeDataTable(CQL);
                    PUMessageBox.ShowDialog("已成功验证表达式", "表达式正确", Buttons.OK);
                   // MessageBox.Show("已成功验证表达式", "表达式正确", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch(Exception ex)
                {
                    PUMessageBox.ShowDialog("表达式或数据类型错误:" + ex.Message, "请检查表达式",Buttons.Cancel);
                    //MessageBox.Show("表达式或数据类型错误", "请检查表达式", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
  
        private void button15_Click(object sender, RoutedEventArgs e)
        {
            if (loadCQL)
            {
                loadCQL = false;
                matchLabel.Visibility=Visibility.Visible;
            }

            WhereTextBox.Text = String.Empty;
            indexInsert = 0;
        }

        private void button16_Click(object sender, RoutedEventArgs e)
        {
            //string url = @"https://neo4j.com/docs/cypher-manual/current";
            //PUMessageBox.ShowDialog("请参考Cypher教程: " + url, "帮助", Buttons.OK);
            //MessageBox.Show("请参考Cypher教程: " + url, "帮助", MessageBoxButton.OK, MessageBoxImage.Information);

            UserGuideDlg userGuideDlg = new UserGuideDlg();
            string path = AppDomain.CurrentDomain.BaseDirectory.ToString();
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            path = path.Substring(0, path.LastIndexOf("\\"));
            //获取运行目录的文档
            userGuideDlg.browser.Address = path + "\\Document\\neo4j-cypher-manual-3.5.pdf";
            userGuideDlg.Show();
        }

        private void button17_Click(object sender, RoutedEventArgs e)
        {
            loadCQL = true;
            matchLabel.Visibility = Visibility.Hidden;
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            openFileDlg.Filter = "CQL(*.txt)|*.txt";
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDlg.FileName;
                StreamReader sr = new StreamReader(file, Encoding.Default);
                string content = string.Empty;
                while ((content = sr.ReadLine()) != null)
                {
                    WhereTextBox.Text += content;
                }
            }
        }

        private void button18_Click(object sender, RoutedEventArgs e)
        {
            List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
            string CQL = string.Empty;
            if (loadCQL)
            {
                CQL = WhereTextBox.Text;
            }
            else
            {
                if (comboBox1.SelectedIndex < 0) return;
                TimeCondition = string.Empty;
                Nodes = new List<Dictionary<string, string>>();
                if (SPACETYPE == QueryType.POLYGON ||
                    SPACETYPE == QueryType.Rectangle ||
                    SPACETYPE == QueryType.POINT)
                {
                    if (Layer != string.Empty)
                    {
                        switch (SPACETYPE)
                        {
                            case QueryType.POINT:
                                {
                                    CQL += "call spatial.intersects('" + Layer + "',\"" + textBoxWKT.Text + "\") YIELD node as NODE ";
                                    break;
                                }
                            case QueryType.NONE:
                                {
                                    break;
                                }
                            default:
                                {
                                    CQL += "WITH \"" + textBoxWKT.Text + "\" as polygon CALL spatial.intersects('" + Layer + "', polygon) YIELD node as NODE ";
                                    break;
                                }
                        }

                    }
                }

                //组织CQL语句
                CQL += matchLabel.Content.ToString().Substring(0, matchLabel.Content.ToString().LastIndexOf('.') - 8) + " ";
                if (WhereTextBox.Text.Length != 0)
                {
                    CQL += "WHERE ";
                }
                if (AdditionalCheckBox.IsChecked == true && TIMETYPE != QueryType.NONE)
                {
                    if (WhereTextBox.Text.Length == 0)
                    {
                        CQL += "WHERE ";
                    }
                    if (WhereTextBox.Text.Length != 0)
                    {
                        TimeCondition += "AND ";
                    }
                    switch (TIMETYPE)
                    {
                        case QueryType.DURATION:
                            {
                                TimeCondition += "datetime(replace(NODE." + StartFieldComboBox.SelectedValue + ",' ','T')) >=" + "datetime('" + strStartDateTime + "') ";//起始时间
                                TimeCondition += "AND datetime(replace(NODE." + EndFieldComboBox.SelectedValue + ",' ','T')) <=" + "datetime('" + strEndDateTime + "') ";//终止时间
                                break;
                            }
                        case QueryType.QUARTER:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).quarter<={2} AND datetime(replace(NODE.{1},' ','T')).quarter>={2} AND datetime(replace(NODE.{0},' ','T')).year=datetime(replace(NODE.{1},' ','T')).year) OR ((datetime(replace(NODE.{0},' ','T')).quarter<={2} OR (datetime(replace(NODE.{1},' ','T')).quarter>={2})) AND datetime(replace(NODE.{0},' ','T')).year<>datetime(replace(NODE.{1},' ','T')).year)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxQuarter.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.MONTH:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).month<={2} AND datetime(replace(NODE.{1},' ','T')).month>={2} AND datetime(replace(NODE.{0},' ','T')).year=datetime(replace(NODE.{1},' ','T')).year) OR ((datetime(replace(NODE.{0},' ','T')).month<={2} OR (datetime(replace(NODE.{1},' ','T')).month>={2})) AND datetime(replace(NODE.{0},' ','T')).year<>datetime(replace(NODE.{1},' ','T')).year)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxMonth.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.DAY:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).day<={2} AND datetime(replace(NODE.{1},' ','T')).day>={2} AND datetime(replace(NODE.{0},' ','T')).month=datetime(replace(NODE.{1},' ','T')).month) OR ((datetime(replace(NODE.{0},' ','T')).day<={2} OR (datetime(replace(NODE.{1},' ','T')).day>={2})) AND datetime(replace(NODE.{0},' ','T')).month<>datetime(replace(NODE.{1},' ','T')).month)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxDay.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.TIME:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).hour<={2} AND datetime(replace(NODE.{1},' ','T')).hour>={2} AND datetime(replace(NODE.{0},' ','T')).ordinalDay=datetime(replace(NODE.{1},' ','T')).ordinalDay) OR ((datetime(replace(NODE.{0},' ','T')).hour<={2} OR (datetime(replace(NODE.{1},' ','T')).hour>={2})) AND datetime(replace(NODE.{0},' ','T')).ordinalDay<>datetime(replace(NODE.{1},' ','T')).ordinalDay)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedItem, EndFieldComboBox.SelectedValue, (comboBoxTime.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.ENSO:
                            {
                                string strCQL = "datetime(replace(NODE.{0},' ','T'))>=datetime(replace('{1}',' ','T')) AND datetime(replace(NODE.{0},' ','T'))<=datetime(replace('{2}',' ','T'))";
                                string strTemp = "";
                                if (treeENSO.SelectedItem != null)
                                {
                                    if (treeENSO.SelectedValue.ToString().Contains("ENSO"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            for (int j = 0; j < ((treeENSO.SelectedItem as TreeViewItem).Items[i] as TreeViewItem).Items.Count; j++)
                                            {
                                                string strTime = ((treeENSO.SelectedItem as TreeViewItem).Items[i] as TreeViewItem).Items[j].ToString().Split('/')[0];
                                                DateTime Now = new DateTime();
                                                Now = Convert.ToDateTime(strTime);
                                                DateTime Before = new DateTime();
                                                Before = Convert.ToDateTime(strTime);
                                                Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedItem as PUComboBoxItem).Content.ToString()));
                                                strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                            }
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else if (treeENSO.SelectedValue.ToString().Contains("ELNINO"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            string strTime = (treeENSO.SelectedItem as TreeViewItem).Items[i].ToString().Split('/')[0];
                                            DateTime Now = new DateTime();
                                            Now = Convert.ToDateTime(strTime);
                                            DateTime Before = new DateTime();
                                            Before = Convert.ToDateTime(strTime);
                                            Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedItem as PUComboBoxItem).Content.ToString()));
                                            strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else if (treeENSO.SelectedValue.ToString().Contains("LANINA"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            string strTime = (treeENSO.SelectedItem as TreeViewItem).Items[i].ToString().Split('/')[0];
                                            DateTime Now = new DateTime();
                                            Now = Convert.ToDateTime(strTime);
                                            DateTime Before = new DateTime();
                                            Before = Convert.ToDateTime(strTime);
                                            Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedItem as PUComboBoxItem).Content.ToString()));
                                            strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else
                                    {
                                        string strTime = labelENSO.Content.ToString().Split('/')[0];
                                        DateTime Now = new DateTime();
                                        Now = Convert.ToDateTime(strTime);
                                        DateTime Before = new DateTime();
                                        Before = Convert.ToDateTime(strTime);
                                        Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedValue as PUComboBoxItem).Content.ToString()));
                                        strTemp = string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString());
                                    }
                                }
                                else { CQL = CQL.Substring(0, CQL.Length - 6); }

                                TimeCondition += strTemp;
                                break;
                            }

                    }

                }
                CQL += SpaceCondition + WhereTextBox.Text + TimeCondition;
                CQL += " RETURN NODE";
            }
            System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "CQL(*.txt)|*.txt";
            saveDialog.FileName = "CQL";
            if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveDialog.FileName);
                sw.Write(CQL);
                sw.Close();
                PUMessageBox.ShowDialog("CQL文件已写入:" + saveDialog.FileName, "完成");
            }
        }

        private void button19_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button20_Click(object sender, RoutedEventArgs e)
        {
            List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
            if (comboBox1.SelectedIndex == -1) return;
            mainForm.SetProgessVisible(Visibility.Visible);
            string CQL = string.Empty;
            if (loadCQL)
            {
                CQL = WhereTextBox.Text;
            }
            else
            {
                if (comboBox1.SelectedIndex < 0) return;
                TimeCondition = string.Empty;
                Nodes = new List<Dictionary<string, string>>();
                if (SPACETYPE == QueryType.POLYGON ||
                    SPACETYPE == QueryType.Rectangle ||
                    SPACETYPE == QueryType.POINT)
                {
                    if (Layer != string.Empty)
                    {
                        switch (SPACETYPE)
                        {
                            case QueryType.POINT:
                                {
                                    CQL += "call spatial.intersects('" + Layer + "',\"" + textBoxWKT.Text + "\") YIELD node as NODE ";
                                    break;
                                }
                            case QueryType.NONE:
                                {
                                    break;
                                }
                            default:
                                {
                                    CQL += "WITH \"" + textBoxWKT.Text + "\" as polygon CALL spatial.intersects('" + Layer + "', polygon) YIELD node as NODE ";
                                    break;
                                }
                        }

                    }
                    else
                    {
                        mainForm.SetProgessVisible(Visibility.Hidden);
                        PUMessageBox.ShowDialog("该类型事件无多边形图层", "请检查图层", Buttons.OK);
                        return;
                    }
                }

                //组织CQL语句
                CQL += matchLabel.Content.ToString().Substring(0, matchLabel.Content.ToString().LastIndexOf('.') - 8) + " ";
                if (WhereTextBox.Text.Length != 0)
                {
                    CQL += "WHERE ";
                }
                if (AdditionalCheckBox.IsChecked == true && TIMETYPE != QueryType.NONE)
                {
                    if (WhereTextBox.Text.Length == 0)
                    {
                        CQL += "WHERE ";
                    }
                    if (WhereTextBox.Text.Length != 0)
                    {
                        TimeCondition += "AND ";
                    }
                    switch (TIMETYPE)
                    {
                        case QueryType.DURATION:
                            {
                                TimeCondition += "datetime(replace(NODE." + StartFieldComboBox.SelectedValue + ",' ','T')) >=" + "datetime('" + strStartDateTime + "') ";//起始时间
                               TimeCondition += "AND datetime(replace(NODE." + EndFieldComboBox.SelectedValue + ",' ','T')) <=" + "datetime('" + strEndDateTime + "') ";//终止时间
                                break;
                            }
                        case QueryType.QUARTER:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).quarter<={2} AND datetime(replace(NODE.{1},' ','T')).quarter>={2} AND datetime(replace(NODE.{0},' ','T')).year=datetime(replace(NODE.{1},' ','T')).year) OR ((datetime(replace(NODE.{0},' ','T')).quarter<={2} OR (datetime(replace(NODE.{1},' ','T')).quarter>={2})) AND datetime(replace(NODE.{0},' ','T')).year<>datetime(replace(NODE.{1},' ','T')).year)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxQuarter.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.MONTH:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).month<={2} AND datetime(replace(NODE.{1},' ','T')).month>={2} AND datetime(replace(NODE.{0},' ','T')).year=datetime(replace(NODE.{1},' ','T')).year) OR ((datetime(replace(NODE.{0},' ','T')).month<={2} OR (datetime(replace(NODE.{1},' ','T')).month>={2})) AND datetime(replace(NODE.{0},' ','T')).year<>datetime(replace(NODE.{1},' ','T')).year)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxMonth.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.DAY:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).day<={2} AND datetime(replace(NODE.{1},' ','T')).day>={2} AND datetime(replace(NODE.{0},' ','T')).month=datetime(replace(NODE.{1},' ','T')).month) OR ((datetime(replace(NODE.{0},' ','T')).day<={2} OR (datetime(replace(NODE.{1},' ','T')).day>={2})) AND datetime(replace(NODE.{0},' ','T')).month<>datetime(replace(NODE.{1},' ','T')).month)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxDay.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.TIME:
                            {
                                string strTemp = "(datetime(replace(NODE.{0},' ','T')).hour<={2} AND datetime(replace(NODE.{1},' ','T')).hour>={2} AND datetime(replace(NODE.{0},' ','T')).ordinalDay=datetime(replace(NODE.{1},' ','T')).ordinalDay) OR ((datetime(replace(NODE.{0},' ','T')).hour<={2} OR (datetime(replace(NODE.{1},' ','T')).hour>={2})) AND datetime(replace(NODE.{0},' ','T')).ordinalDay<>datetime(replace(NODE.{1},' ','T')).ordinalDay)";
                                TimeCondition += string.Format(strTemp, StartFieldComboBox.SelectedValue, EndFieldComboBox.SelectedValue, (comboBoxTime.SelectedItem as PUComboBoxItem).Content);
                                break;
                            }
                        case QueryType.ENSO:
                            {
                                string strCQL = "datetime(replace(NODE.{0},' ','T'))>=datetime(replace('{1}',' ','T')) AND datetime(replace(NODE.{0},' ','T'))<=datetime(replace('{2}',' ','T'))";
                                string strTemp = "";
                                if (treeENSO.SelectedItem != null)
                                {
                                    if (treeENSO.SelectedValue.ToString().Contains("ENSO"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            for (int j = 0; j < ((treeENSO.SelectedItem as TreeViewItem).Items[i] as TreeViewItem).Items.Count; j++)
                                            {
                                                string strTime = ((treeENSO.SelectedItem as TreeViewItem).Items[i] as TreeViewItem).Items[j].ToString().Split('/')[0];
                                                DateTime Now = new DateTime();
                                                Now = Convert.ToDateTime(strTime);
                                                DateTime Before = new DateTime();
                                                Before = Convert.ToDateTime(strTime);
                                                Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedValue as PUComboBoxItem).Content.ToString()));
                                                strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                            }
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else if (treeENSO.SelectedValue.ToString().Contains("ELNINO"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            string strTime = (treeENSO.SelectedItem as TreeViewItem).Items[i].ToString().Split('/')[0];
                                            DateTime Now = new DateTime();
                                            Now = Convert.ToDateTime(strTime);
                                            DateTime Before = new DateTime();
                                            Before = Convert.ToDateTime(strTime);
                                            Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedItem as PUComboBoxItem).Content.ToString()));
                                            strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else if (treeENSO.SelectedValue.ToString().Contains("LANINA"))
                                    {
                                        for (int i = 0; i < (treeENSO.SelectedItem as TreeViewItem).Items.Count; i++)
                                        {
                                            string strTime = (treeENSO.SelectedItem as TreeViewItem).Items[i].ToString().Split('/')[0];
                                            DateTime Now = new DateTime();
                                            Now = Convert.ToDateTime(strTime);
                                            DateTime Before = new DateTime();
                                            Before = Convert.ToDateTime(strTime);
                                            Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedItem as PUComboBoxItem).Content.ToString()));
                                           strTemp += string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString()) + " OR ";
                                        }
                                        strTemp = strTemp.Substring(0, strTemp.Length - 4);
                                    }
                                    else
                                    {
                                        string strTime = labelENSO.Content.ToString().Split('/')[0];
                                        DateTime Now = new DateTime();
                                        Now = Convert.ToDateTime(strTime);
                                        DateTime Before = new DateTime();
                                        Before = Convert.ToDateTime(strTime);
                                        Before = Before.AddMonths(-1 * Convert.ToInt16((comboBox2.SelectedValue as PUComboBoxItem).Content.ToString()));
                                        strTemp = string.Format(strCQL, StartFieldComboBox.SelectedValue, Before.ToString(), Now.ToString());
                                    }
                                }
                                else { CQL = CQL.Substring(0, CQL.Length - 6); }

                                TimeCondition += strTemp;
                                break;
                            }
                    }
                }
                CQL += SpaceCondition + WhereTextBox.Text+" "/*多加一个空格防止手动输入时导致格式错误*/ + TimeCondition;
                string CountCQL = CQL + " RETURN COUNT(NODE)";
                CQL += " RETURN NODE";              
                if (SortComboBox.SelectedIndex > 0)
                {
                    if(SortComboBox.SelectedValue.ToString().ToUpper().Contains("TIME")&& SortComboBox.SelectedValue.ToString().ToUpper() != "DURTIME")
                    {
                        CQL += " ORDER BY datetime(replace(NODE." + SortComboBox.SelectedValue.ToString() + ",\" \",\"T\")) " + OrderType;
                    }
                    else
                    {
                        CQL += " ORDER BY NODE." + SortComboBox.SelectedValue.ToString() + " " + OrderType;
                    }
                    
                }
                CQL += " SKIP 0 LIMIT "+Const.PERPAGECOUNT.ToString();
                string[] CQLS = { CQL, CountCQL };
                Thread UpdateTable = new Thread(new ParameterizedThreadStart(UpdateNodes));
                UpdateTable.SetApartmentState(ApartmentState.STA);
                UpdateTable.Start(CQLS);
            }
        }

        private async void button21_Click(object sender, RoutedEventArgs e)
        {
            if (mainForm.WindowState == WindowState.Minimized)
            {
                mainForm.WindowState = WindowState.Normal;
            }
            //mainForm.Focus();
            this.WindowState = WindowState.Minimized;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Rectangle;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mainForm.MyMapView.SketchEditor.StartAsync(creationMode, true);
                textBoxWKT.Text = Convertor.Geometry2WktWgs84(geometry);
                SPACETYPE = QueryType.Rectangle;
                this.WindowState = WindowState.Normal;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                PUMessageBox.ShowDialog("Error drawing graphic shape: " + ex.Message, "提示");
            }
        }

        private async void button22_Click(object sender, RoutedEventArgs e)
        {
            if (mainForm.WindowState == WindowState.Minimized)
            {
                mainForm.WindowState = WindowState.Normal;
            }
            //mainForm.Focus();
            this.WindowState = WindowState.Minimized;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Polygon;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mainForm.MyMapView.SketchEditor.StartAsync(creationMode, true);
                textBoxWKT.Text = Convertor.Geometry2WktWgs84(geometry);
                SPACETYPE = QueryType.POLYGON;
                this.WindowState = WindowState.Normal;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                PUMessageBox.ShowDialog("Error drawing graphic shape: " + ex.Message);
            }
        }

        private async void button23_Click(object sender, RoutedEventArgs e)
        {
            if (mainForm.WindowState == WindowState.Minimized)
            {
                mainForm.WindowState = WindowState.Normal;
            }
            //mainForm.Focus();
            this.WindowState = WindowState.Minimized;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.FreehandPolygon;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mainForm.MyMapView.SketchEditor.StartAsync(creationMode, true);
                textBoxWKT.Text = Convertor.Geometry2WktWgs84(geometry);
                this.WindowState = WindowState.Normal;
                SPACETYPE = QueryType.POLYGON;
                this.WindowState = WindowState.Normal;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                PUMessageBox.ShowDialog("Error drawing graphic shape: " + ex.Message);
            }
        }

        private async void button24_Click(object sender, RoutedEventArgs e)
        {
            if (mainForm.WindowState == WindowState.Minimized)
            {
                mainForm.WindowState = WindowState.Normal;
            }
            //mainForm.Focus();
            this.WindowState = WindowState.Minimized;
            try
            {
                // Let the user draw on the map view using the chosen sketch mode
                SketchCreationMode creationMode = SketchCreationMode.Point;
                Esri.ArcGISRuntime.Geometry.Geometry geometry = await mainForm.MyMapView.SketchEditor.StartAsync(creationMode, true);
                textBoxWKT.Text = Convertor.Geometry2WktWgs84(geometry);
                this.WindowState = WindowState.Normal;
                SPACETYPE = QueryType.POINT;
                this.WindowState = WindowState.Normal;
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel drawing
            }
            catch (Exception ex)
            {
                // Report exceptions
                PUMessageBox.ShowDialog("Error drawing graphic shape: " + ex.Message);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            String strTemp = "NODE.";
            strTemp += listBox1.SelectedValue.ToString();
            indexInsert = WhereUpdate(strTemp, indexInsert);
        }

        private void listBox2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            String strTemp = "";
            strTemp += listBox2.SelectedValue.ToString();
            indexInsert = WhereUpdate(strTemp, indexInsert);
        }

        private void AdditionalCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                (OptionTab.Items[1] as PUTabItem).IsEnabled = true;
                (OptionTab.Items[2] as PUTabItem).IsEnabled = true;
                OptionTab.Foreground= new SolidColorBrush(Color.FromRgb( 202, 81, 0));
                OptionTab.SelectedIndex = 1;
                TimeCheckBox.IsChecked = true;
                checkBoxCustom.IsChecked = true;
            }
            else
            {
                AdditionalCheckBox.IsChecked = false;
            }
        }

        private void AdditionalCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            (OptionTab.Items[1] as PUTabItem).IsEnabled = false;
            (OptionTab.Items[2] as PUTabItem).IsEnabled = false;
            OptionTab.Foreground = new SolidColorBrush(Color.FromRgb(172, 172, 172));
            OptionTab.SelectedIndex = 0;
            SpaceCondition = string.Empty;
            TimeCondition = string.Empty;
            SPACETYPE = QueryType.NONE;
            TIMETYPE = QueryType.NONE;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainForm.blOpenQueryDlg = false;
            //for (int i = 0; i < FeatureTableList.Count; i++)
            //{
            //    FeatureTableList[i].Close();
            //}
        }

        private void WhereTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            isInsert = true;
            indexInsert = WhereTextBox.SelectionStart;
            if (WhereTextBox.SelectionStart == WhereTextBox.Text.Length)
            {
                isInsert = false;
            }
        }

        private void TimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            comboBoxStartYear.SelectedIndex = 0;
            comboBoxStartMonth.SelectedIndex = 0;
            comboBoxStartDay.SelectedIndex = 0;
            comboBoxStartTime.SelectedIndex = 0;

            comboBoxEndYear.SelectedIndex = 0;
            comboBoxEndMonth.SelectedIndex = 0;
            comboBoxEndDay.SelectedIndex = 0;
            comboBoxEndTime.SelectedIndex = 0;

            TimeGroupBox.IsEnabled = true;
            ENSOCheckBox.IsChecked = false;
            //ENSOGroupBox.IsEnabled = false;

            TimeGroupBox.Background= new SolidColorBrush(Color.FromArgb(80, 42, 42, 42));
            ENSOGroupBox.Background= new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));

            if (StartFieldComboBox.Items.Count == 0)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    if ((listBox1.Items[i] as PUListBoxItem).Content.ToString().Contains("ETime") || (listBox1.Items[i] as PUListBoxItem).Content.ToString().Contains("STime"))
                    {
                        PUComboBoxItem item1 = new PUComboBoxItem();
                        PUComboBoxItem item2 = new PUComboBoxItem();
                        item1.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                        item2.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                        StartFieldComboBox.Items.Add(item1);
                        EndFieldComboBox.Items.Add(item2);
                    }
                    if (StartFieldComboBox.Items.Count >= 2)
                    {
                        StartFieldComboBox.SelectedIndex = 1;
                        EndFieldComboBox.SelectedIndex = 0;
                    }
                }
                if (StartFieldComboBox.Items.Count == 0)
                {
                    for (int i = 0; i < listBox1.Items.Count; i++)
                    {
                        PUComboBoxItem item1 = new PUComboBoxItem();
                        PUComboBoxItem item2= new PUComboBoxItem();
                        item1.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                        item2.Content = (listBox1.Items[i] as PUListBoxItem).Content;
                        StartFieldComboBox.Items.Add(item1);
                        EndFieldComboBox.Items.Add(item2);
                    }
                }
               }
            }

        private void TimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TimeCondition = string.Empty;
            TimeGroupBox.IsEnabled = false;
            TIMETYPE = QueryType.NONE;
            TimeGroupBox.Background = new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));
        }

        private void ENSOCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            comboBox2.SelectedIndex = 0;
            TIMETYPE = QueryType.ENSO;
            ENSOGroupBox.IsEnabled = true;
            comboBox3.IsEnabled = true;
            comboBox2.IsEnabled = true;
            TimeCheckBox.IsChecked = false;
            TimeGroupBox.IsEnabled = false;
            ENSOGroupBox.Background = new SolidColorBrush(Color.FromArgb(80, 42, 42, 42));
            List<String> Features =  new List<string>();
            try
            {
                Features = Neo4j64.QueryFeatures();
            }
            catch
            {

            }
            
            if (comboBox3.Items.Count == 0)
            {
                for (int i = 0; i < Features.Count; i++)
                {
                    if (Features[i].Contains("ENSO") || Features[i].Contains("enso"))
                    {
                        PUComboBoxItem combItem = new PUComboBoxItem();
                        combItem.Content = (Features[i]);
                        comboBox3.Items.Add(combItem);
                    }
                }
            }
            if (comboBox3.Items.Count > 0)
            {
                comboBox3.SelectedIndex = 0;
                List<Dictionary<string, string>> tempNodes = null; ;
                string CQL = "MATCH(NODE:" + comboBox3.SelectedValue + ") RETURN NODE";
                try
                {
                    tempNodes = Neo4j64.QueryNodeDataTable(CQL);
                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("查询错误: " + ex.Message, "错误");
                }

                if (tempNodes != null)
                {
                    if (treeENSO.Items.Count == 0)
                    {
                        TreeViewItem ParentItem = new TreeViewItem();
                        ParentItem.Header = "ENSO";
                        ParentItem.Foreground = Brushes.White;
                        TreeViewItem ELNINOItem = new TreeViewItem();
                        ELNINOItem.Header = "ELNINO";
                        ELNINOItem.Foreground = Brushes.White;
                        TreeViewItem LANINAItem = new TreeViewItem();
                        LANINAItem.Header = "LANINA";
                        LANINAItem.Foreground = Brushes.White;
                        for (int i = 0; i < tempNodes.Count; i++)
                        {
                            if (tempNodes[i]["Type"] == "ELNINO")
                            {
                                TreeViewItem item = new TreeViewItem();
                                item.Header = tempNodes[i]["StartTime"] + "/" + tempNodes[i]["EndTime"];
                                item.Foreground = Brushes.White;
                                ELNINOItem.Items.Add(item);
                            }
                            else if (tempNodes[i]["Type"] == "LANINA")
                            {
                                TreeViewItem item = new TreeViewItem();
                                item.Header = tempNodes[i]["StartTime"] + "/" + tempNodes[i]["EndTime"];
                                item.Foreground = Brushes.White;
                                LANINAItem.Items.Add(item);
                            }
                        }
                        ELNINOItem.IsExpanded = true;
                        LANINAItem.IsExpanded = true;
                        ParentItem.Items.Add(ELNINOItem);
                        ParentItem.Items.Add(LANINAItem);
                        treeENSO.Items.Add(ParentItem);
                        (treeENSO.Items[0] as TreeViewItem).IsExpanded = true;        
                    }
                }
            }
            else
            {
                for (int i = 0; i < Features.Count; i++)
                {
                    comboBox3.Items.Add(Features[i]);
                }
            }
        }

        private void ENSOCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //ENSOGroupBox.IsEnabled = false;
            ENSOGroupBox.Background = new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));
            treeENSO.Items.Clear();
            comboBox3.IsEnabled = false;
            comboBox2.IsEnabled = false;
            TIMETYPE = QueryType.NONE;
        }

        private void comboBoxStartYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
        }

        private void comboBoxStartMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();          
        }

        private void comboBoxStartDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
        
        }

        private void comboBoxStartTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
           
        }

        private void comboBoxEndYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
            
        }

        private void comboBoxEndMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
           
        }

        private void comboBoxEndDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
           
        }

        private void comboBoxEndTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DURATION;
            UpdateGUI();
           
        }

        private void comboBoxQuarter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.QUARTER;
            UpdateGUI();
        }

        private void comboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.MONTH;
            UpdateGUI();
        }

        private void comboBoxDay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.DAY;
            UpdateGUI();
        }

        private void comboBoxTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TIMETYPE = QueryType.TIME;
            UpdateGUI();
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                AdditionalCheckBox.IsEnabled = true;

            }
            if (comboBox1.SelectedIndex == -1)
            {
                AdditionalCheckBox.IsEnabled = false;

            }
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            for(int i = SortComboBox.Items.Count - 1; i > 0; i--)
            {
                SortComboBox.Items.RemoveAt(i);
            }
            SortComboBox.SelectedIndex = 0;
            matchLabel.Content = "MATCH(NODE:" + comboBox1.SelectedValue + ") WHERE ... RETURN NODE";
            if (comboBox1.SelectedIndex != -1)
            {
                Thread updateThread = new Thread(new ParameterizedThreadStart(UpdateFileds));
                updateThread.Start(comboBox1.SelectedValue.ToString());
            }
        }

        private void comboBox1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                List<String> Features = new List<String>();
                try
                {
                    Features = Neo4j64.QueryFeatures();
                }
                catch
                {

                }                
                if (Features.Count>0)
                {
                    for (int i = 0; i < Features.Count; i++)
                    {
                        if (Features[i].Contains("Event") || Features[i].Contains("Process"))
                        {
                            //comboBox1.Items.Add(Features[i]);
                            PUComboBoxItem item = new PUComboBoxItem();
                            item.Height = 41;
                            item.Content = Features[i];
                            comboBox1.Items.Add(item);
                        }

                    }
                }
            }
        }

        private void comboBox3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Dictionary<string, string>> tempNodes = null; ;
            string CQL = "MATCH(NODE:" + comboBox3.SelectedValue.ToString() + ") RETURN NODE";
            try
            {
                tempNodes = Neo4j64.QueryNodeDataTable(CQL);
            }
            catch (Exception ex)
            {
                PUMessageBox.ShowDialog("查询错误: " + ex.Message, "错误");
            }

            if (tempNodes != null)
            {
                if (treeENSO.Items.Count == 0)
                {
                    TreeViewItem ParentItem = new TreeViewItem();
                    ParentItem.Header = "ENSO";
                    ParentItem.Foreground = Brushes.White;
                    TreeViewItem ELNINOItem = new TreeViewItem();
                    ELNINOItem.Header = "ELNINO";
                    ELNINOItem.Foreground = Brushes.White;
                    TreeViewItem LANINAItem = new TreeViewItem();
                    LANINAItem.Header = "LANINA";
                    LANINAItem.Foreground = Brushes.White;
                    for (int i = 0; i < tempNodes.Count; i++)
                    {
                        if (tempNodes[i]["Type"] == "ELNINO")
                        {
                            TreeViewItem item = new TreeViewItem();
                            item.Header = tempNodes[i]["StartTime"] + "/" + tempNodes[i]["EndTime"];
                            item.Foreground = Brushes.White;
                            ELNINOItem.Items.Add(item);
                        }
                        else if (tempNodes[i]["Type"] == "LANINA")
                        {
                            TreeViewItem item = new TreeViewItem();
                            item.Header = tempNodes[i]["StartTime"] + "/" + tempNodes[i]["EndTime"];
                            item.Foreground = Brushes.White;
                            LANINAItem.Items.Add(item);
                        }
                    }
                    ELNINOItem.IsExpanded = true;
                    LANINAItem.IsExpanded = true;
                    ParentItem.Items.Add(ELNINOItem);
                    ParentItem.Items.Add(LANINAItem);
                    treeENSO.Items.Add(ParentItem);
                    (treeENSO.Items[0] as TreeViewItem).IsExpanded = true;
                }
            }
        }

        private void checkBoxCustom_Checked(object sender, RoutedEventArgs e)
        {
            groupBoxCustom.Background = new SolidColorBrush(Color.FromArgb(80, 42, 42, 42));
            //groupBoxArea.Background = new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));
            checkBoxArea.IsChecked = false;
            //groupBoxArea.IsEnabled = false;
            //groupBoxCustom.IsEnabled = true;
        }

        private void checkBoxCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            groupBoxCustom.Background = new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));
            SpaceCondition = string.Empty;
            //groupBoxCustom.IsEnabled = false;
            SPACETYPE = QueryType.NONE;
        }

        private void checkBoxArea_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxCustom.IsChecked = false;
            groupBoxArea.Background = new SolidColorBrush(Color.FromArgb(80, 42, 42, 42));
            //groupBoxCustom.IsEnabled = false;
            //groupBoxArea.IsEnabled = true;
            if (treeArea.Items.Count == 0)
            {
                StreamReader reader = new StreamReader("../../../Data/Administrative_division.txt", Encoding.UTF8);
                string retString = reader.ReadToEnd();
                reader.Close();
                //解析josn
                Newtonsoft.Json.Linq.JArray jobject = (Newtonsoft.Json.Linq.JArray)Newtonsoft.Json.JsonConvert.DeserializeObject(retString);
                for (int i = 0; i < jobject.Count; i++)
                {
                    string str = jobject[i]["name"].ToString();
                }
                TreeViewItem AreaItem = new TreeViewItem();
                AreaItem.Header = "行政区划";
                AreaItem.Foreground = Brushes.White;
                TreeViewItem RiverItem = new TreeViewItem();
                RiverItem.Header = "河流";
                RiverItem.Foreground = Brushes.White;
                TreeViewItem RoadItem = new TreeViewItem();
                RoadItem.Header = "道路";
                RoadItem.Foreground = Brushes.White;
                for (int i = 0; i < jobject.Count; i++)
                {
                    TreeViewItem ProvinceItem = new TreeViewItem();
                    ProvinceItem.Header = jobject[i]["name"].ToString();
                    var jsonLength = 0;
                    foreach (var k in jobject[i]["city"])
                    {
                        jsonLength++;
                    }
                    for (int j = 0; j < jsonLength; j++)
                    {
                        TreeViewItem cityItem = new TreeViewItem();
                        cityItem.Header = jobject[i]["city"][j]["name"].ToString();
                        cityItem.Foreground = Brushes.White;
                        var jsonLength1 = 0;
                        foreach (var k in jobject[i]["city"][j]["area"])
                        {
                            jsonLength1++;
                        }
                        for (int k = 0; k < jsonLength1; k++)
                        {
                            TreeViewItem tempItem = new TreeViewItem();
                            tempItem.Header = jobject[i]["city"][j]["area"][k].ToString();
                            tempItem.Foreground = Brushes.White;
                            cityItem.Items.Add(tempItem);
                        }
                        ProvinceItem.Items.Add(cityItem);
                        ProvinceItem.Foreground = Brushes.White;
                    }
                    AreaItem.Items.Add(ProvinceItem);
                }
                AreaItem.IsExpanded = true;
                treeArea.Items.Add(AreaItem);
                treeArea.Items.Add(RiverItem);
                treeArea.Items.Add(RoadItem);
            }         
        }

        private void checkBoxArea_Unchecked(object sender, RoutedEventArgs e)
        {
            groupBoxArea.Background = new SolidColorBrush(Color.FromArgb(0, 42, 42, 42));
            SpaceCondition = string.Empty;
            treeArea.Items.Clear();
            //groupBoxArea.IsEnabled = false;
            SPACETYPE = QueryType.NONE;
        }

        private void treeENSO_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if ((treeENSO.Items[0] as TreeViewItem).IsExpanded == false)
                {
                    (treeENSO.Items[0] as TreeViewItem).IsExpanded = true;
                    for (int i = 0; i < (treeENSO.Items[0] as TreeViewItem).Items.Count; i++)
                    {
                        (((treeENSO.Items[0] as TreeViewItem).Items[i]) as TreeViewItem).IsExpanded = true;
                    }
                }
                for (int i = 0; i < (treeENSO.Items[0] as TreeViewItem).Items.Count; i++)
                {
                    if ((((treeENSO.Items[0] as TreeViewItem).Items[i]) as TreeViewItem).IsExpanded == false)
                    {
                        (((treeENSO.Items[0] as TreeViewItem).Items[i]) as TreeViewItem).IsExpanded = true;
                    }
                }
                if (treeENSO.SelectedValue.ToString().Contains("ENSO"))
                {
                    labelENSO.Content = "全部ENSO事件";
                }
                else if (treeENSO.SelectedValue.ToString().Contains("ELNINO"))
                {
                    labelENSO.Content = "全部ELNINO事件";
                }
                else if (treeENSO.SelectedValue.ToString().Contains("LANINA"))
                {
                    labelENSO.Content = "全部LANINA事件";
                }
                else
                {
                    labelENSO.Content = (treeENSO.SelectedItem as TreeViewItem).Header.ToString();
                }
                TIMETYPE = QueryType.ENSO;
            }
            catch
            {

            }
        }

        private void treeArea_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (treeArea.SelectedValue != null)
            {
                SPACETYPE = QueryType.POLYGON;
                try
                {
                    string WKT = string.Empty;
                    if (treeArea.SelectedValue.GetType().ToString() == "System.String")
                    {
                        WKT = Neo4j64.QueryAreaWKT(treeArea.SelectedValue.ToString());
                    }
                    else
                    {
                        bool preIsExpande = (treeArea.SelectedValue as TreeViewItem).IsExpanded;
                        (treeArea.SelectedValue as TreeViewItem).IsExpanded = !preIsExpande;
                        WKT = Neo4j64.QueryAreaWKT((treeArea.SelectedValue as TreeViewItem).Header.ToString());

                        //if ((treeArea.SelectedValue as TreeViewItem).IsExpanded == false)
                        //{
                        //    (treeArea.SelectedValue as TreeViewItem).IsExpanded = true;
                        //}
                    }
                    if (WKT == string.Empty)
                    {
                        textBoxWKT.Text = "该区域暂无数据";
                        SPACETYPE = QueryType.NONE;
                    }
                    else
                    {
                        textBoxWKT.Text = WKT;

                    }
                }
                catch (Exception ex)
                {
                    textBoxWKT.Text = ex.Message.ToString();
                    SPACETYPE = QueryType.NONE;
                }
            }
        }

        private void WhereTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //WhereTextBox.SelectionStart = WhereTextBox.Text.Length;  
            if (!isInsert)
            {
                indexInsert = WhereTextBox.Text.Length;
            }
            if (WhereTextBox.Text.Length == 0)
            {
                button14.IsEnabled = false;
                button15.IsEnabled = false;
            }
            else
            {
                button14.IsEnabled = true;
                button15.IsEnabled = true;
            }
        }

        private void PUWindow_StateChanged(object sender, EventArgs e)
        {
        }

        private void PUButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderType=="ASC")
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/DESC.PNG", UriKind.Relative)));
                EnableBr.Stretch = Stretch.Uniform;
                OrderBtn.Background = EnableBr;
                OrderType = "DESC";
                OrderBtn.ToolTip = "降序排列";
            }
            else if(OrderType=="DESC")
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/ASC.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.Uniform;
                OrderBtn.Background = DisableBr;
                OrderType = "ASC";
                OrderBtn.ToolTip = "升序排列";
            }
        }
    }
}
