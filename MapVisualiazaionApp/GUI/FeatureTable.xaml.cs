using Esri.ArcGISRuntime.Geometry;
using MarineSTMiningSystem;
using MarineSTMiningSystem.DataOP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Panuon.UI;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MapVisualizationApp.DataOP;

namespace MapVisualizationApp.GUI
{
    /// <summary>
    /// FeatureTable.xaml 的交互逻辑
    /// </summary>
    public partial class FeatureTable 
    {
        
        private QueryDlg QDlg;
        private List<Dictionary<string, string>> SqNode;
        private List<Dictionary<string, string>> StNode;
        private List<String> SqFields;
        private List<String> StFields;
        private string QueryCQL = string.Empty;
        private string OrderCQLTemplate = string.Empty;
        private string NonOrderCQLTemplate = string.Empty;
        private int EventCount = 0;
        private string OrderType = "ASC";
        
        public FeatureTable(QueryDlg QDlg, DataTable dt, string text,string QueryCQL,int EventCount=0)
        {
            InitializeComponent();
            if (dt != null)
            {
                dataGridEvent.ItemsSource = dt.DefaultView;
            }         
            this.QDlg = QDlg;
            this.Title = text;
            this.QueryCQL = QueryCQL;

            //获取CQL模板
            if (QueryCQL.Contains("ORDER BY"))
            {
                OrderCQLTemplate = QueryCQL.Replace("ORDER BY NODE." + SortComboBox.SelectedValue.ToString(), "ORDER BY NODE.{0}");
                OrderCQLTemplate = OrderCQLTemplate.Replace(QueryCQL.Contains("ASC") == true ? "ASC" : "DESC", " {1}");
            }
            else//未进行排序的情况
            {
                NonOrderCQLTemplate = QueryCQL.Substring(0, QueryCQL.LastIndexOf("SKIP")) + "SKIP {0} LIMIT {1}";
            }
            

            this.EventCount = EventCount;
            EventPageNav.TotalPage = (int)Math.Ceiling((double)(EventCount * 1.0 / Const.PERPAGECOUNT));
            (this.tabControl.Items[1] as TabItem).Visibility = Visibility.Hidden;
            (this.tabControl.Items[2] as TabItem).Visibility = Visibility.Hidden;
            for(int i = 1; i < QDlg.SortComboBox.Items.Count; i++)//不要第一项
            {
                PUComboBoxItem tempItem = new PUComboBoxItem();
                tempItem.Content = (QDlg.SortComboBox.Items[i] as PUComboBoxItem).Content;
                SortComboBox.Items.Add(tempItem);
            }
            SortComboBox.SelectedIndex = QDlg.SortComboBox.SelectedIndex;
            Thread thread = new Thread(new ParameterizedThreadStart(LoadFields));
            thread.Start(text);
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridEvent.SelectedItems.Count != 1)
            {
                PUMessageBox.ShowDialog("选择一行查看详细信息", "选择行数过多:"+dataGridEvent.SelectedItems.Count, Buttons.Yes);
                return;
            }
            QDlg.mainForm.SetProgessVisible(Visibility.Visible);
            string PRID = string.Empty;
            int Index_PRID = -1;
            for(int i=0;i< dataGridEvent.Columns.Count;i++)
            {
                if (dataGridEvent.Columns[i].Header.ToString().Equals("过程编号"))
                {
                    Index_PRID = i;
                    break;
                }
            }
            try
            {             
                if (Index_PRID >= 0)
                {
                    PRID = (dataGridEvent.SelectedItem as DataRowView).Row[Index_PRID].ToString();                   
                    string SQCQL = "MATCH(NODE:" + this.Title + "{PRID:" + PRID + "})-[:Belong]->(SQNODE) RETURN SQNODE";
                    string STCQL = "MATCH(NODE:" + this.Title + "{PRID:" + PRID + "})-[:Belong]->()-[:Belong]->(STNODE) RETURN STNODE";
                    try
                    {
                        SqNode = Neo4j64.QueryNodeDataTable(SQCQL);
                        StNode = Neo4j64.QueryNodeDataTable(STCQL);
                    }
                    catch (Exception ex)
                    {
                        QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                        PUMessageBox.ShowDialog("查询错误: " + ex.Message, "错误", Buttons.Yes);
                        
                        //System.Windows.MessageBox.Show("查询错误: " + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                }
                else
                {
                    QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                    PUMessageBox.ShowDialog("未找到编号列表(数据库中PRID未定义)!", "错误", Buttons.Yes);
                    //System.Windows.MessageBox.Show("未找到编号列表(数据库中PRID未定义)!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Thread thread = new Thread(UpdateTables);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            catch
            {
               // PUMessageBox.ShowDialog("未找到编号列表(数据库中PRID未定义)!", "错误", Buttons.Yes);
                return;
            }
            

        }

        private void UpdateTables()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            while (SqFields == null || StFields == null)
            {
                //等待字段数据
                if (stopwatch.Elapsed.TotalSeconds >= 15)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                        PUMessageBox.ShowDialog("未找到序列和状态节点（查询超时）", "错误");
                    }));        
                    return;
                }
            }
            Dispatcher.Invoke(new Action(delegate
            {
                dataGridSequence.ItemsSource = Convertor.MapNode2DataTable(SqNode, QDlg.mainForm.FieldsMap, SqFields).DefaultView;
                dataGridState.ItemsSource = Convertor.MapNode2DataTable(StNode, QDlg.mainForm.FieldsMap, StFields).DefaultView;
                QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                (this.tabControl.Items[1] as TabItem).Visibility = Visibility.Visible;
                (this.tabControl.Items[2] as TabItem).Visibility = Visibility.Visible;
            }));
            
                       
        }
        private void LoadFields(object NodeType)
        {
            try
            {
                SqFields = Neo4j64.QueryFeatureFields(Neo4j64.QueryEventSeqLabel(NodeType.ToString()));
                StFields = Neo4j64.QueryFeatureFields(Neo4j64.QueryEventStateLabel(NodeType.ToString()));
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    QDlg.mainForm.SetStatuLabel(ex.Message);
                }));
            }
        }

        private void MenuItem2_Click(object sender, RoutedEventArgs e)
        {
            #region 获取节点
            string PRID = string.Empty;
            int Index_PRID = -1;
            for (int i = 0; i < dataGridEvent.Columns.Count; i++)
            {
                if (dataGridEvent.Columns[i].Header.ToString().Equals("过程编号"))
                {
                    Index_PRID = i;
                    break;
                }
            }
            try
            {
                if (Index_PRID > -1)
                {
                    PRID = (dataGridEvent.SelectedItem as DataRowView).Row[Index_PRID].ToString();
                }
                else
                {
                    PUMessageBox.ShowDialog("未找到编号列表(数据库中PRID未定义)!", "错误", Buttons.Yes);
                    return;
                }
            }
            catch
            {
                return;
            }
            

            string SQCQL = "MATCH(NODE:" + this.Title + "{PRID:" + PRID + "})-[:Belong]->(SQNODE) RETURN SQNODE order by datetime(replace(SQNODE.Time,' ','T'))";
            string STCQL = "MATCH(NODE:" + this.Title + "{PRID:" + PRID + "})-[:Belong]->()-[:Belong]->(STNODE) RETURN STNODE order by datetime(replace(STNODE.Time,' ','T'))";
            string STREL = "MATCH(NODE: " + this.Title + "{ PRID: " + PRID + "})-[:Belong]->() -[:Belong]->(STNODE1)-[R]->(STNODE2) RETURN STNODE1.STID,R.StateAction,STNODE2.STID limit 100";
            
            try
            {
                SqNode = Neo4j64.QueryNodeDataTable(SQCQL);
                StNode = Neo4j64.QueryNodeDataTable(STCQL);
            }
            catch (Exception ex)
            {
                PUMessageBox.ShowDialog("查询错误: " + ex.Message, "错误", Buttons.OK);
                return;
            }
            #endregion
            if (SqNode==null || StNode == null)
            {
                PUMessageBox.ShowDialog("未找到该事件的序列或状态，请检查图结构是否完整！");
                return;
            }

            #region 排列多边形
            //按时间使用交错数组存储事件信息
            //每个内存单元存储WKT格式的多边形信息
            //同一列表示同一时刻出现的多边形
            //ListOfProcess中存储的数据用于展示
            //---------------------------------------------------------
            //|  t1  |  t2  |  t3  |  t4  |  t5  |  t6  |  t7  |  t8  |
            //---------------------------------------------------------
            //|  t1  |  t2  |  t3  |  t4  |  t5  |  t6  |      |  t8  |
            //-------------------------------------------      --------
            //       |  t2  |      |  t4  |
            //       --------      --------
            //
            //ListOfTime（Hashset)用于处理存储唯一值
            //---------------------------------------------------------
            //|  t1  |  t2  |  t3  |  t4  |  t5  |  t6  |  t7  |  t8  |
            //---------------------------------------------------------
            //
            HashSet<String> ListOfTime = new HashSet<String>();
            List<String[]> ListOfProcess = new List<String[]>();
            if (ListOfProcess.Count > 0) ListOfProcess.Clear();
            if (ListOfTime.Count > 0) ListOfTime.Clear();


            List<String> tempList = new List<String>();
            tempList.Add(StNode[0]["geometry"]/* + "-" + StNode[0]["Time"]*/);
            for (int i = 1; i < StNode.Count; i++)
            {

                if (StNode[i]["Time"] == StNode[i - 1]["Time"])
                {
                    tempList.Add(StNode[i]["geometry"]/*+"-"+StNode[i]["Time"]*/);
                    ListOfTime.Add(StNode[i]["Time"]);
                }
                else
                {
                    ListOfTime.Add(StNode[i - 1]["Time"]);
                    ListOfTime.Add(StNode[i]["Time"]);

                    String[] tempString = new String[tempList.Count];
                    for (int m = 0; m < tempList.Count; m++)
                    {
                        tempString[m] = tempList[m];
                    }
                    ListOfProcess.Add(tempString);
                    tempList.Clear();
                    tempList.Add(StNode[i]["geometry"]/* + "-" + StNode[i]["Time"]*/);
                }
                if (i == StNode.Count - 1)
                {
                    String[] tempString = new String[tempList.Count];
                    for (int m = 0; m < tempList.Count; m++)
                    {
                        tempString[m] = tempList[m];
                    }
                    ListOfProcess.Add(tempString);
                    ListOfTime.Add(StNode[i]["Time"]);
                }
            }
            #endregion
            Thread WebThread = new Thread(new ParameterizedThreadStart(UpdateWebRelation));
            WebThread.SetApartmentState(ApartmentState.STA);
            WebThread.Start(STREL);
            string WKT = string.Empty;
            int Index_WKT = -1;
            for (int i = 0; i < dataGridEvent.Columns.Count; i++)
            {
                if (dataGridEvent.Columns[i].Header.ToString().Equals("WKT"))
                {
                    Index_WKT = i;
                    break;
                }
            }
            try
            {
                if (Index_WKT >= 0)
                {
                    WKT = (dataGridEvent.SelectedItem as DataRowView).Row[Index_WKT].ToString();
                    List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = Convertor.Wkt2Geometry(WKT, 4326, 4326);
                    Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(GeoList);
                    QDlg.mainForm.MyMapView.SetViewpointGeometryAsync(UnionGeo, 200);
                }
            }
            catch
            {

            }

            QDlg.mainForm.ListOfTime = ListOfTime;
            QDlg.mainForm.ListOfProcess = ListOfProcess;
            QDlg.mainForm.SetPlayBarVisible(true);
            QDlg.mainForm.button3.IsEnabled = true;
            QDlg.mainForm.button4.IsEnabled = true;
            QDlg.mainForm.slider.Value = 0;
            //QDlg.mainForm.Topmost = true;
            QDlg.mainForm.Focus();
            
        }

        private void MenuItem3_Click(object sender, RoutedEventArgs e)
        {

        }
        private void MenuItem4_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridEvent.SelectedItems.Count > 50)
            {
                PUMessageBox.ShowDialog("为保证显示效果，最多不超过50个多边形", "显示数量过多:"+dataGridEvent.SelectedItems.Count);
                return;
            }
            QDlg.mainForm.ExtentGeo.Clear();
            string WKT = string.Empty;
            int Index_WKT = -1;
            for (int i = 0; i < dataGridEvent.Columns.Count; i++)
            {
                if (dataGridEvent.Columns[i].Header.ToString().Contains("WKT"))
                {
                    Index_WKT = i;
                    break;
                }
            }
            try
            {
                if (Index_WKT >= 0)
                {                                       
                       Dispatcher.Invoke(new Action(delegate
                        {
                            for (int i = 0; i < dataGridEvent.SelectedItems.Count; i++)
                            {
                                WKT = (dataGridEvent.SelectedItems[i] as DataRowView).Row[Index_WKT].ToString();
                                Dictionary<string, string> rowInfo = new Dictionary<string, string>();
                                for (int j = 0; j < (dataGridEvent.SelectedItems[i] as DataRowView).Row.ItemArray.Length; j++)
                                {
                                    rowInfo.Add(dataGridEvent.Columns[j].Header.ToString(), (dataGridEvent.SelectedItems[i] as DataRowView).Row[j].ToString());
                                }   
                                List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = Convertor.Wkt2Geometry(WKT, 4326, 4326);
                                Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(GeoList);
                                QDlg.mainForm.ExtentGeo.Add(UnionGeo, rowInfo);
                                QDlg.mainForm.ShowExtentGeometry(QDlg.mainForm.ExtentGeo);
                                QDlg.mainForm.Focus();
                                QDlg.mainForm.button3.IsEnabled = true;
                            }
                        }));
                    }
                        
                  
                else
                {
                    PUMessageBox.ShowDialog("未找到WKT数据(数据库中无多边形数据)!", "错误", Buttons.OK);
                    return;
                }
            }
            catch
            {
                return;
            }
            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {          
        }

        private void dataGridEvent_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

        }

        private void DataGridEvent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string WKT = string.Empty;
            int Index_WKT = -1;
            for (int i = 0; i < dataGridEvent.Columns.Count; i++)
            {
                if (dataGridEvent.Columns[i].Header.ToString().Equals("WKT"))
                {
                    Index_WKT = i;
                    break;
                }
            }
            try
            {
                if (Index_WKT >= 0)
                {
                    WKT = (dataGridEvent.SelectedItem as DataRowView).Row[Index_WKT].ToString();
                    Dispatcher.Invoke(new Action(delegate
                    {
                        List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = Convertor.Wkt2Geometry(WKT, 4326, 4326);
                        QDlg.mainForm.FlashShape(GeoList);
                    }));

                }
                else
                {
                    PUMessageBox.ShowDialog("未找到WKT数据(数据库中无多边形数据)!", "错误");
                    return;
                }
            }
            catch
            {
                return;
            }
            
        }

        private void DataGridState_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string WKT = string.Empty;
            int Index_WKT = -1;
            for (int i = 0; i < dataGridState.Columns.Count; i++)
            {
                if (dataGridState.Columns[i].Header.ToString().Equals("WKT"))
                {
                    Index_WKT = i;
                    break;
                }
            }
            try
            {
                if (Index_WKT >= 0)
                {
                    WKT = (dataGridState.SelectedItem as DataRowView).Row[Index_WKT].ToString();
                    Dispatcher.Invoke(new Action(delegate
                    {
                        List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = Convertor.Wkt2Geometry(WKT, 4326, 4326);
                        QDlg.mainForm.FlashShape(GeoList);
                    }));

                }
                else
                {
                    PUMessageBox.ShowDialog("未找到WKT数据(数据库中无多边形数据)!", "错误");
                    return;
                }
            }
            catch
            {
                return;
            }
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv|Excel (*.xls)|*.xls";
            saveFileDialog.FileName = this.Title;
            switch (tabControl.SelectedIndex)
            {
                case 0: { saveFileDialog.FileName += "_Event"; break; }
                case 1: { saveFileDialog.FileName += "_Sequence"; break; }
                case 2: { saveFileDialog.FileName += "_State"; break; }
            }
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog.FileName.Length > 0)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    #region CSV
                    case 1:
                        {
                            switch (tabControl.SelectedIndex)
                            {
                                case 0:
                                    {
                                        if (dataGridEvent.ItemsSource!=null)
                                        {
                                            if (ExportCSV((dataGridEvent.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        if (dataGridEvent.ItemsSource != null)
                                        {
                                            if (ExportCSV((dataGridSequence.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (dataGridState.ItemsSource != null)
                                        {
                                            if (ExportCSV((dataGridState.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                            }

                            break;
                        }
                    #endregion
                    #region EXCEL
                    case 2:
                        {
                            switch (tabControl.SelectedIndex)
                            {
                                case 0:
                                    {
                                        if (dataGridEvent.ItemsSource != null)
                                        {
                                            if (ExportExcel((dataGridEvent.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                                case 1:
                                    {
                                        if (dataGridSequence.ItemsSource != null)
                                        {
                                            if (ExportExcel((dataGridSequence.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                                case 2:
                                    {
                                        if (dataGridState.ItemsSource != null)
                                        {
                                            if (ExportExcel((dataGridState.ItemsSource as DataView).ToTable(), saveFileDialog.FileName.ToString()))
                                            {
                                                PUMessageBox.ShowDialog("数据已导出到:" + saveFileDialog.FileName.ToString(), "完成");
                                            }
                                            else
                                            {
                                                PUMessageBox.ShowDialog("数据导出失败:" + saveFileDialog.FileName.ToString(), "失败");
                                            }
                                        }
                                        else
                                        {
                                            PUMessageBox.ShowDialog("当前表无数据，已取消", "完成");
                                        }
                                        break;
                                    }
                            }

                            break;

                        }
                        #endregion
                }
            }
        }

        public static Boolean ExportCSV(DataTable dt, string fullFileName)
        {
            try
            {
                FileStream fs = new FileStream(fullFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                string data = "";

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += dt.Columns[i].ColumnName.ToString();
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        data += dt.Rows[i][j].ToString();
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }
                    sw.WriteLine(data);
                }

                sw.Close();
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Boolean ExportExcel(DataTable m_DataTable, string s_FileName)
        {
            try
            {
                if (System.IO.File.Exists(s_FileName))                                //存在则删除
                {
                    System.IO.File.Delete(s_FileName);
                }
                System.IO.FileStream objFileStream;
                System.IO.StreamWriter objStreamWriter;
                string strLine = "";
                objFileStream = new System.IO.FileStream(s_FileName, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                objStreamWriter = new System.IO.StreamWriter(objFileStream, Encoding.Unicode);
                for (int i = 0; i < m_DataTable.Columns.Count; i++)
                {
                    strLine = strLine + m_DataTable.Columns[i].Caption.ToString() + Convert.ToChar(9);      //写列标题
                }
                objStreamWriter.WriteLine(strLine);
                strLine = "";
                for (int i = 0; i < m_DataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < m_DataTable.Columns.Count; j++)
                    {
                        if (m_DataTable.Rows[i].ItemArray[j] == null)
                            strLine = strLine + " " + Convert.ToChar(9);                                    //写内容
                        else
                        {
                            string rowstr = "";
                            rowstr = m_DataTable.Rows[i].ItemArray[j].ToString();
                            if (rowstr.IndexOf("\r\n") > 0)
                                rowstr = rowstr.Replace("\r\n", " ");
                            if (rowstr.IndexOf("\t") > 0)
                                rowstr = rowstr.Replace("\t", " ");
                            strLine = strLine + rowstr + Convert.ToChar(9);
                        }
                    }
                    objStreamWriter.WriteLine(strLine);
                    strLine = "";
                }
                objStreamWriter.Close();
                objFileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                string strex = ex.Message;
                return false;
            }

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                try
                {
                    StatusTableInfo.Content = "事件总量统计：" + EventCount;
                }
                catch
                {

                }
                
            }
            else if (tabControl.SelectedIndex == 1)
            {
                try
                {
                    StatusTableInfo.Content = "所选事件序列统计：" + (dataGridSequence.ItemsSource as DataView).ToTable().Rows.Count;
                }
                catch
                {

                }
            }
            else if (tabControl.SelectedIndex ==2)
            {
                try
                {
                    StatusTableInfo.Content = "所选事件状态统计：" + (dataGridState.ItemsSource as DataView).ToTable().Rows.Count;
                }
                catch
                {

                }
               
            }
            else
            {

            }
        }

        private void UpdateWebRelation(object cql)
        {
            string CQL = (string)cql;
            List<List<string>> Res = new List<List<string>>();
            try
            {
                Res = Neo4j64.QueryNonNodeDataTable(CQL);
            }
            catch(Exception ex)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    PUMessageBox.ShowDialog( ex.Message);
                }));
            }
            
            if (Res.Count!=0)
            {
                string JSON = "{\"links\":[";
                string strTemplate = "\"source\":{0},\"target\":{1},\"type\":\"resolved\",\"rela\":{2}";
                string Content = string.Empty;
                for (int i = 0; i < Res.Count; i++)
                {
                    Content += "{" + string.Format(strTemplate, "\"" + Res[i][0] + "\"", "\"" + Res[i][2] + "\"", "\"" + Res[i][1] + "\"") + "},";
                }
                Content = Content.Substring(0, Content.Length - 1);
                JSON += Content + "]}";
                FileStream fs = new FileStream(@"miserables.json", FileMode.Create, FileAccess.Write, FileShare.Read);
                // UTF-8 为默认编码 
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(JSON);
                sw.Close();
                string currentPath = System.Windows.Forms.Application.ExecutablePath;
                currentPath = currentPath.Replace("/", "\\");
                currentPath = currentPath.Substring(0, currentPath.LastIndexOf("\\"));
                string url = Convertor.ChineseUrlEncode(currentPath + "\\index.html");
                QDlg.mainForm.LoadWeb(url);
            }
        }

        private void PUWindow_Closed(object sender, EventArgs e)
        {
            QDlg.FeatureTableList.Remove(this);
            if (QDlg.FeatureTableList.Count == 0) QDlg.mainForm.button3.IsEnabled = false;
        }

        private void PUContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridState.SelectedItems.Count > 50)
            {
                PUMessageBox.ShowDialog("为保证显示效果，最多不超过50个多边形", "显示数量过多:" + dataGridEvent.SelectedItems.Count);
                return;
            }
            QDlg.mainForm.StateGeo.Clear();
            string WKT = string.Empty;
            int Index_WKT = -1;
            for (int i = 0; i < dataGridState.Columns.Count; i++)
            {
                if (dataGridState.Columns[i].Header.ToString().Contains("WKT"))
                {
                    Index_WKT = i;
                    break;
                }
            }
            try
            {
                if (Index_WKT >= 0)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        for (int i = 0; i < dataGridState.SelectedItems.Count; i++)
                        {
                            WKT = (dataGridState.SelectedItems[i] as DataRowView).Row[Index_WKT].ToString();
                            Dictionary<string, string> rowInfo = new Dictionary<string, string>();
                            for (int j = 0; j < (dataGridState.SelectedItems[i] as DataRowView).Row.ItemArray.Length; j++)
                            {
                                rowInfo.Add(dataGridState.Columns[j].Header.ToString(), (dataGridState.SelectedItems[i] as DataRowView).Row[j].ToString());
                            }
                            List<Esri.ArcGISRuntime.Geometry.Geometry> GeoList = Convertor.Wkt2Geometry(WKT, 4326, 4326);
                            Esri.ArcGISRuntime.Geometry.Geometry UnionGeo = GeometryEngine.Union(GeoList);
                            QDlg.mainForm.StateGeo.Add(UnionGeo, rowInfo);
                            QDlg.mainForm.ShowStatetGeometry(QDlg.mainForm.StateGeo);
                            QDlg.mainForm.Focus();
                            QDlg.mainForm.button3.IsEnabled = true;
                        }
                    }));
                }


                else
                {
                    PUMessageBox.ShowDialog("未找到WKT数据(数据库中无多边形数据)!", "错误", Buttons.OK);
                    return;
                }
            }
            catch
            {
                return;
            }
        }

        private void EventPageNav_CurrentPageChanged(object sender, RoutedPropertyChangedEventArgs<int> e)
        {
            this.IsAwaitShow = true;
            //string tempCQL = QueryCQL.Substring(0, QueryCQL.LastIndexOf("SKIP"));
            //string tempStr = QueryCQL.Substring(QueryCQL.LastIndexOf("LIMIT"));
            //string tempSkip = (Convert.ToInt16(tempStr.Substring(6))*(EventPageNav.CurrentPage-1)).ToString();
            //string CQL = tempCQL + "SKIP " + tempSkip + " "+tempStr;//重新组织下一页表的查询语句
            string CQL = string.Empty;
            if (SortComboBox.SelectedIndex > 0)
            {
                if (OrderCQLTemplate == string.Empty)
                {
                    OrderCQLTemplate = NonOrderCQLTemplate.Replace("SKIP {0} LIMIT {1}", "ORDER BY NODE.{0} {1} SKIP {2} LIMIT {3}");
                }
                CQL = string.Format(OrderCQLTemplate, 
                    SortComboBox.SelectedValue.ToString(), 
                    OrderType, (Const.PERPAGECOUNT*(EventPageNav.CurrentPage - 1)).ToString(), 
                    Const.PERPAGECOUNT.ToString());           
            }
            else
            {
                CQL = string.Format(NonOrderCQLTemplate,
                    (Const.PERPAGECOUNT * (EventPageNav.CurrentPage - 1)).ToString(),
                    Const.PERPAGECOUNT.ToString());
            }
            Thread thread = new Thread(() =>
             {
                 List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
                 //转换为DataTable
                 DataTable tempTable = new DataTable("Dataset");
                 try
                 {
                     Nodes = Neo4j64.QueryNodeDataTable((string)CQL);
                 }
                 catch
                 {
                     Dispatcher.Invoke(new Action(delegate
                     {
                         this.IsAwaitShow = false;
                     }));                      
                     return;
                 }
                 List<String> Keys = new List<String>();
                 Dispatcher.Invoke(new Action(delegate
                 {
                     {
                         for (int i = 0; i < QDlg.listBox1.Items.Count; i++)
                         {
                             Keys.Add((QDlg.listBox1.Items[i] as PUListBoxItem).Content.ToString());
                         }
                     }
                 }));
                 if (Nodes != null)
                 {
                     Dispatcher.Invoke(new Action(delegate
                     {
                         tempTable = Convertor.MapNode2DataTable(Nodes, QDlg.mainForm.FieldsMap, Keys);
                         if (tempTable != null)
                         {
                             (dataGridEvent.ItemsSource as DataView).Table.Rows.Clear();
                             for (int i = 0; i < tempTable.Rows.Count; i++)
                             {
                                 (dataGridEvent.ItemsSource as DataView).Table.ImportRow(tempTable.Rows[i]);
                             }                            
                         }
                         QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                         this.IsAwaitShow = false;
                     }));
                 }
                 else
                 {
                     Dispatcher.Invoke(new Action(delegate
                     {
                         QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                         PUMessageBox.ShowDialog("未查询到数据！", "提示", Buttons.OK);
                         this.IsAwaitShow = false;
                     }));
                 }
             });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void PUButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderType == "ASC")
            {
                ImageBrush EnableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/DESC.PNG", UriKind.Relative)));
                EnableBr.Stretch = Stretch.Uniform;
                OrderBtn.Background = EnableBr;
                OrderType = "DESC";
                OrderBtn.ToolTip = "降序排列";
            }
            else if (OrderType == "DESC")
            {
                ImageBrush DisableBr = new ImageBrush(new BitmapImage(new Uri("../../../ICONS/ASC.png", UriKind.Relative)));
                DisableBr.Stretch = Stretch.Uniform;
                OrderBtn.Background = DisableBr;
                OrderType = "ASC";
                OrderBtn.ToolTip = "升序排列";
            }
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string CQL = string.Empty;
            if (SortComboBox.SelectedIndex > 0)
            {
                if (OrderCQLTemplate == string.Empty)
                {
                    OrderCQLTemplate = NonOrderCQLTemplate.Replace("SKIP {0} LIMIT {1}", "ORDER BY NODE.{0} {1} SKIP {2} LIMIT {3}");
                }
                CQL = string.Format(OrderCQLTemplate,
                    SortComboBox.SelectedValue.ToString(),
                    OrderType, (Const.PERPAGECOUNT * (EventPageNav.CurrentPage - 1)).ToString(),
                    Const.PERPAGECOUNT.ToString());
                Thread thread = new Thread(() =>
                {
                    List<Dictionary<string, string>> Nodes = new List<Dictionary<string, string>>();
                    //转换为DataTable
                    DataTable tempTable = new DataTable("Dataset");
                    try
                    {
                        Nodes = Neo4j64.QueryNodeDataTable((string)CQL);
                    }
                    catch
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            this.IsAwaitShow = false;
                        }));
                        return;
                    }
                    List<String> Keys = new List<String>();
                    Dispatcher.Invoke(new Action(delegate
                    {
                        {
                            for (int i = 0; i < QDlg.listBox1.Items.Count; i++)
                            {
                                Keys.Add((QDlg.listBox1.Items[i] as PUListBoxItem).Content.ToString());
                            }
                        }
                    }));
                    if (Nodes != null)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            tempTable = Convertor.MapNode2DataTable(Nodes, QDlg.mainForm.FieldsMap, Keys);
                            if (tempTable != null)
                            {
                                (dataGridEvent.ItemsSource as DataView).Table.Rows.Clear();
                                for (int i = 0; i < tempTable.Rows.Count; i++)
                                {
                                    (dataGridEvent.ItemsSource as DataView).Table.ImportRow(tempTable.Rows[i]);
                                }
                            }
                            QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                            this.IsAwaitShow = false;
                        }));
                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            QDlg.mainForm.SetProgessVisible(Visibility.Hidden);
                            PUMessageBox.ShowDialog("未查询到数据！", "提示", Buttons.OK);
                            this.IsAwaitShow = false;
                        }));
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }
    }
}
