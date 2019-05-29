using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
using MapVisualizationApp.Class;
using MarineSTMiningSystem.DataOP;
using Panuon.UI;
namespace MapVisualizationApp.GUI
{
    /// <summary>
    /// DBManagementDlg.xaml 的交互逻辑
    /// </summary>
    public partial class DBManagementDlg
    {
        Process proc = new Process();
        Process ConsoleProc = new Process();
        List<string> DeleteIndexes = new List<string>();
        bool isCreating = false;
        bool isEdit = false;

        DataTable dataTableUser = new DataTable();
        Dictionary<string, Dictionary<string, string>> UserInfo = new Dictionary<string, Dictionary<string, string>>();

        DataTable dataTableLayer = new DataTable();
        public DBManagementDlg()
        {
            InitializeComponent();
            InitializeGUISettings();
        }

        private void InitializeGUISettings()
        {
            LayerComboBox.SelectedIndex = 0;
            EventLabelComboBox.SelectedIndex = 0;
            EventIDComboBox.SelectedIndex = 0;
            SeqLabelComboBox.SelectedIndex = 0;
            SeqIDComboBox.SelectedIndex = 0;
            StLabelComboBox.SelectedIndex = 0;
            StIDComboBox.SelectedIndex = 0;
            LabelRelComboBox.SelectedIndex = 0;
            UserComboBox.SelectedIndex = 0;

            dataTableUser.Columns.Add("用户名");
            dataTableUser.Columns.Add("角色组");
            dataTableUser.Columns.Add("状态");
            UserInfoTable.ItemsSource = dataTableUser.DefaultView;

            dataTableLayer.Columns.Add("图层名");
            dataTableLayer.Columns.Add("创建时间");
            dataTableLayer.Columns.Add("图层类型");
            dataTableLayer.Columns.Add("编码类型");
            dataTableLayer.Columns.Add("索引类型");
            dataTableLayer.Columns.Add("多边形数量", Type.GetType("System.Int32"));
            LayerInfoTable.ItemsSource = dataTableLayer.DefaultView;
        }
        private void PUButton_Click(object sender, RoutedEventArgs e)
        {

            if (DBFileTextBox.Text == "")
            {
                PUMessageBox.ShowDialog("数据库文件不能为空!");
                return;
            }
            if (ShpFileTextBox.Text == "")
            {
                PUMessageBox.ShowDialog("Shp文件不能为空!");
                return;
            }
            if (LayerTextBox.Text == "")
            {
                PUMessageBox.ShowDialog("图层名不能为空!");
                return;
            }
            try
            {
                proc = new Process();
                proc.StartInfo.FileName = "ShapefileImporter.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.OutputDataReceived += proc_OutputDataReceived;
                proc.ErrorDataReceived += Console_OutputErrorReceived;
                proc.StartInfo.Arguments = DBFileTextBox.Text + "  " + ShpFileTextBox.Text + "  " + LayerTextBox.Text;
                proc.Start();
                this.IsAwaitShow = true;
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                ConsoleTextBox.Text = ex.Message + "\r\n";
            }
        }

        void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                IsAwaitShow = false;
                ConsoleTextBox.Text += e.Data + "\r\n";
                ConsoleTextBox.ScrollToEnd();
                //Application.DoEvents();
                if (ConsoleTextBox.LineCount > 300) ConsoleTextBox.Clear();
            }));
        }

        private void PUButton_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择.db数据库文件夹路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    //System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    PUMessageBox.ShowDialog("文件夹路径不能为空", "提示");
                    return;
                }
                if (dialog.SelectedPath.Contains("graph.db"))
                {
                    DBFileTextBox.Text = dialog.SelectedPath;
                }
                else
                {
                    if (Directory.Exists(dialog.SelectedPath + "\\graph.db"))
                    {
                        DBFileTextBox.Text = dialog.SelectedPath + "\\graph.db";
                    }
                    else
                    {
                        PUMessageBox.ShowDialog("所选路径未包含graph.db");
                        DBFileTextBox.Text = dialog.SelectedPath;
                    }
                }

            }
        }

        private void PUButton_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择.shp数据文件夹路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    //System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    PUMessageBox.ShowDialog("文件夹路径不能为空", "提示");
                    return;
                }
                ShpFileTextBox.Text = dialog.SelectedPath;
            }
        }

        private void PUButton_Click_3(object sender, RoutedEventArgs e)
        {
            ConsoleTextBox.Clear();
        }

        private void PUButton_Click_4(object sender, RoutedEventArgs e)
        {
            if (proc != null)
            {
                try
                {
                    proc.Kill();
                }
                catch (Exception ex)
                {
                    ConsoleTextBox.Text = ex.Message + "\r\n";
                }
            }
        }

        private void LabelsCombBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.IsAwaitShow = true;
            AllFieldsList.Items.Clear();
            if (LabelsCombBox.SelectedIndex != -1)
            {
                Thread updateThread = new Thread(new ParameterizedThreadStart(UpdateFileds));
                updateThread.Start(LabelsCombBox.SelectedValue.ToString());
            }
        }

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
                        item.Content = FeaturesFileds[i];
                        AllFieldsList.Items.Add(item);
                    }
                    this.IsAwaitShow = false;
                }));
            }
        }

        private void PUButton_Click_5(object sender, RoutedEventArgs e)
        {
            if (AllFieldsList.SelectedItems.Count > 0) isEdit = true;
            HashSet<string> HashIndexes = new HashSet<string>();
            for (int i = 0; i < IndexFieldsList.Items.Count; i++)
            {
                HashIndexes.Add((IndexFieldsList.Items[i] as PUListBoxItem).Content.ToString());
            }
            for (int i = 0; i < AllFieldsList.SelectedItems.Count; i++)
            {
                PUListBoxItem tempItem = new PUListBoxItem();
                tempItem.Content = LabelsCombBox.SelectedValue.ToString() + "(" + (AllFieldsList.SelectedItems[i] as PUListBoxItem).Content + ")";
                if (!HashIndexes.Contains(tempItem.Content.ToString()))
                {
                    IndexFieldsList.Items.Add(tempItem);
                }
            }
            AllFieldsList.SelectedItems.Clear();
        }

        private void PUButton_Click_6(object sender, RoutedEventArgs e)
        {
            if (AllFieldsList.Items.Count > 0) isEdit = true;
            HashSet<string> HashIndexes = new HashSet<string>();
            for (int i = 0; i < IndexFieldsList.Items.Count; i++)
            {
                HashIndexes.Add((IndexFieldsList.Items[i] as PUListBoxItem).Content.ToString());
            }
            for (int i = 0; i < AllFieldsList.Items.Count; i++)
            {
                PUListBoxItem tempItem = new PUListBoxItem();
                tempItem.Content = LabelsCombBox.SelectedValue.ToString() + "(" + (AllFieldsList.Items[i] as PUListBoxItem).Content + ")";
                if (!HashIndexes.Contains(tempItem.Content.ToString()))
                {
                    IndexFieldsList.Items.Add(tempItem);
                }
            }
            AllFieldsList.SelectedItems.Clear();
        }

        private void PUButton_Click_7(object sender, RoutedEventArgs e)
        {

            if (IndexFieldsList.SelectedItems.Count > 0) isEdit = true;
            List<object> tempItems = new List<object>();
            for (int i = 0; i < IndexFieldsList.SelectedItems.Count; i++)
            {
                tempItems.Add(IndexFieldsList.SelectedItems[i]);
            }
            for (int i = 0; i < tempItems.Count; i++)
            {
                IndexFieldsList.Items.Remove(tempItems[i]);
                DeleteIndexes.Add((tempItems[i] as PUListBoxItem).Content.ToString());
            }
        }

        private void PUButton_Click_8(object sender, RoutedEventArgs e)
        {
            if (IndexFieldsList.Items.Count > 0) isEdit = true;
            List<object> tempItems = new List<object>();
            for (int i = 0; i < IndexFieldsList.Items.Count; i++)
            {
                tempItems.Add(IndexFieldsList.Items[i]);
            }
            for (int i = 0; i < tempItems.Count; i++)
            {
                IndexFieldsList.Items.Remove(tempItems[i]);
                DeleteIndexes.Add((tempItems[i] as PUListBoxItem).Content.ToString());
            }
        }

        private void PUButton_Click_9(object sender, RoutedEventArgs e)
        {
            isEdit = false;
            Thread thread = new Thread(UpdateIndex);
            thread.Start();

        }

        private void UpdateIndex()
        {
            if (!Neo4j64.isAdminRole)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    PUMessageBox.ShowDialog("当前用户无权限！");
                    return;
                }));
            }
            else
            {
                List<string> CreateIndexes = new List<string>();
                Dispatcher.Invoke(new Action(delegate
                {
                    this.IsAwaitShow = true;
                }));
                Dispatcher.Invoke(new Action(delegate
                {
                    for (int i = 0; i < IndexFieldsList.Items.Count; i++)
                    {
                        //string CQL = "CREATE INDEX ON:" + (IndexFieldsList.Items[i] as PUListBoxItem).Content.ToString();
                        CreateIndexes.Add((IndexFieldsList.Items[i] as PUListBoxItem).Content.ToString());
                        //Neo4j64.ExcuteCQL(CQL);
                    }
                }));
                if (CreateIndexes.Count > 0 || DeleteIndexes.Count > 0)
                {
                    for (int i = 0; i < CreateIndexes.Count; i++)
                    {
                        string CQL = "CREATE INDEX ON:" + CreateIndexes[i];
                        try
                        {
                            Neo4j64.ExcuteCQL(CQL);
                        }
                        catch (Exception ex)
                        {
                            PUMessageBox.ShowDialog("创建失败:" + ex.Message);
                        }

                    }
                    for (int i = 0; i < DeleteIndexes.Count; i++)
                    {
                        string CQL = "DROP INDEX ON:" + DeleteIndexes[i];
                        try
                        {
                            Neo4j64.ExcuteCQL(CQL);
                        }
                        catch (Exception ex)
                        {
                            PUMessageBox.ShowDialog("创建失败:" + ex.Message);
                        }
                    }
                }
                else
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        this.IsAwaitShow = false;
                    }));
                    return;
                }
                Dispatcher.Invoke(new Action(delegate
                {
                    DeleteIndexes.Clear();
                    Dispatcher.Invoke(new Action(delegate
                    {
                        this.IsAwaitShow = false;
                    }));
                }));
            }



        }

        private void UpdateGUI()
        {
            int IndexCount = 0;
            int LabelsCount = 0;
            Dispatcher.Invoke(new Action(delegate
            {
                LabelsCount = LabelsCombBox.Items.Count;
                IndexCount = IndexFieldsList.Items.Count;

            }));
            if (Neo4j64.isConnected)
            {
                if (LabelsCount == 0&&IndexCount==0)
                {
                    List<string> NodeLabels = new List<string>();
                    try
                    {
                        NodeLabels = Neo4j64.QueryFeatures();
                    }
                    catch
                    {

                    }

                    NodeLabels.Sort();
                    for (int i = 0; i < NodeLabels.Count; i++)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUComboBoxItem tempItem = new PUComboBoxItem();
                            tempItem.Height = 41;
                            tempItem.Content = NodeLabels[i];
                            LabelsCombBox.Items.Add(tempItem);
                        }));
                    }
                    Dispatcher.Invoke(new Action(delegate
                    {
                        if (NodeLabels.Count > 0)
                            LabelsCombBox.SelectedIndex = 0;
                    }));
                    List<string> Indexes = new List<string>();
                    Indexes = Neo4j64.QueryIndexes();
                    Indexes.Sort();
                    
                    for (int i = 0; i < Indexes.Count; i++)
                    {                       
                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUListBoxItem tempItem = new PUListBoxItem();
                            tempItem.Content = Indexes[i];
                            IndexFieldsList.Items.Add(tempItem);
                            label.Content = "已创建索引：";
                        }));
                    }

                }
            }
        }

        private void PUWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isEdit)
            {
                if (PUMessageBox.ShowConfirm("是否保存更改?") == true)
                {
                    Thread thread = new Thread(UpdateIndex);
                    thread.Start();
                }
            }
        }

        private void PUTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NeoConsoleText.Clear();
            if (OptionTab.SelectedValue.ToString() == "数据更新")
            {

            }
            if (OptionTab.SelectedValue.ToString() == "索引更新")
            {
                Thread thread = new Thread(UpdateGUI);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            if (OptionTab.SelectedValue.ToString() == "用户更新")
            {
                Thread t = new Thread(() =>
                {
                    if (dataTableUser.Rows.Count == 0)
                    {
                        UpdateUserTable();
                    }
                    UpdateUserInfo();
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }
            if (OptionTab.SelectedValue.ToString() == "Neo4j控制台")
            {
                try
                {
                    ConsoleProc = new Process();
                    ConsoleProc.StartInfo.FileName = "cmd.exe";
                    ConsoleProc.StartInfo.UseShellExecute = false;
                    ConsoleProc.StartInfo.RedirectStandardInput = true;
                    ConsoleProc.StartInfo.RedirectStandardOutput = true;
                    ConsoleProc.StartInfo.RedirectStandardError = true;
                    ConsoleProc.StartInfo.CreateNoWindow = true;
                    ConsoleProc.OutputDataReceived += Console_OutputDataReceived;
                    ConsoleProc.ErrorDataReceived += Neo4jConsole_OutputErrorReceived;
                    ConsoleProc.Start();
                    ConsoleProc.BeginOutputReadLine();
                    ConsoleProc.BeginErrorReadLine();
                }
                catch
                {

                }
            }
        }
        void Console_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                if (e.Data.Contains(">"))
                {
                    //NeoConsoleText.Text += "\r";
                }
                else
                {
                    NeoConsoleText.Text += e.Data + "\r";
                    Console.WriteLine(e.Data);
                }
                NeoConsoleText.SelectionStart = NeoConsoleText.Text.Length;
                NeoConsoleText.ScrollToEnd();
                if (NeoConsoleText.LineCount > 300) ConsoleTextBox.Clear();
            }));
        }

        void Neo4jConsole_OutputErrorReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                //PUMessageBox.ShowDialog(e.Data.Length.ToString(), "错误");
                NeoConsoleText.Text += e.Data + "\r\n";
            }));

        }

        void Console_OutputErrorReceived(object sender, DataReceivedEventArgs e)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                //PUMessageBox.ShowDialog(e.Data.Length.ToString(), "错误");
                ConsoleTextBox.Text += e.Data + "\r\n";
            }));

        }

        private void NeoConsoleText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //string LastInput= NeoConsoleText.GetLineText(NeoConsoleText.LineCount-1);              
                string LastInput = NeoConsoleText.Text.Substring(NeoConsoleText.Text.LastIndexOf('\r') + 1);
                if (!LastInput.Equals(""))
                {
                    NeoConsoleText.Text = NeoConsoleText.Text.Insert(NeoConsoleText.Text.Length - LastInput.Length, "Neo4j> ") + "\r\n";
                    ConsoleProc.StandardInput.WriteLine(LastInput);
                }

            }
        }

        private void PUButton_Click_10(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(NeoConsoleText.Text);
            NeoConsoleText.Clear();
        }

        private void PUButton_Click_11(object sender, RoutedEventArgs e)
        {
            ConsoleProc.StandardInput.WriteLine("Neo4j Status");
        }

        private void PUButton_Click_12(object sender, RoutedEventArgs e)
        {
            ConsoleProc.StandardInput.WriteLine("Neo4j Stop");
        }

        private void PUButton_Click_13(object sender, RoutedEventArgs e)
        {
            ConsoleProc.StandardInput.WriteLine("Neo4j Start");
        }

        private void PUButton_Click_14(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                int index = -1;
                Dispatcher.Invoke(new Action(delegate
                {
                    index = LayerComboBox.SelectedIndex;
                }));
                try
                {
                    string CQL = string.Empty;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        CQL = "match (n)-[:RTREE_ROOT]-()-[:RTREE_CHILD*]->()-[:RTREE_REFERENCE*]->(m) where n.layer=\"" + LayerComboBox.SelectedValue + "\" set m:" + NodeLabel.Text + " remove m.ID";
                    }));
                    try
                    {

                        if (!Neo4j64.isAdminRole)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                PUMessageBox.ShowDialog("当前用户无权限！");
                                return;
                            }));
                        }
                        else if (index > 0)
                        {
                            Neo4j64.ExcuteCQL(CQL);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                PUMessageBox.ShowDialog("标签已更新!");
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("标签更新失败:" + ex.Message);
                            return;
                        }));

                    }


                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        PUMessageBox.ShowDialog(ex.Message);
                    }));
                }
            });
            t.Start();

        }

        private void PUButton_Click_15(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                int index = -1;
                Dispatcher.Invoke(new Action(delegate
                {
                    index = LayerComboBox.SelectedIndex;
                }));
                try
                {
                    string CQL = string.Empty;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        CQL = "match (n)-[:RTREE_ROOT]-()-[:RTREE_CHILD*]->()-[:RTREE_REFERENCE*]->(m) where n.layer=\"" + LayerComboBox.SelectedValue + "\" remove m:" + NodeLabel.Text;
                    }));
                    try
                    {
                        if (!Neo4j64.isAdminRole)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                PUMessageBox.ShowDialog("当前用户无权限！");
                                return;
                            }));
                        }
                        else if (index > 0)
                        {
                            Neo4j64.ExcuteCQL(CQL);
                            Dispatcher.Invoke(new Action(delegate
                            {
                                NodeLabel.Text = "";
                                PUMessageBox.ShowDialog("标签已删除!");
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                            return;
                        }));
                    }

                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        PUMessageBox.ShowDialog(ex.Message);
                    }));
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

        }

        private void PUButton_Click_16(object sender, RoutedEventArgs e)
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

        private void EventBtn_Click(object sender, RoutedEventArgs e)
        {
            if (EventLabelComboBox.SelectedIndex == -1) EventLabelComboBox.SelectedIndex = 0;
            if (EventIDComboBox.SelectedIndex == -1) EventIDComboBox.SelectedIndex = 0;
            if (Neo4j64.isConnected && EventLabelComboBox.Items.Count == 1)
            {
                List<string> Event = new List<string>();
                try
                {
                    Event = Neo4j64.QueryFeatures();
                }
                catch
                {

                }
                for (int i = 0; i < Event.Count; i++)
                {
                    if (Event[i].Contains("Event") || Event[i].Contains("Process"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = Event[i];
                        EventLabelComboBox.Items.Add(tempItem);
                    }
                }
            }
            if (EventBubble.Visibility == Visibility.Hidden)
            {
                EventBubble.Visibility = Visibility.Visible;
            }
            else if (EventBubble.Visibility == Visibility.Visible)
            {
                EventBubble.Visibility = Visibility.Hidden;
            }
        }

        private void SeqtBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SeqLabelComboBox.SelectedIndex == -1) SeqLabelComboBox.SelectedIndex = 0;
            if (SeqIDComboBox.SelectedIndex == -1) SeqIDComboBox.SelectedIndex = 0;
            if (Neo4j64.isConnected && SeqLabelComboBox.Items.Count == 1)
            {
                List<string> Seq = new List<string>();
                try
                {
                    Seq = Neo4j64.QueryFeatures();
                }
                catch
                {

                }
                for (int i = 0; i < Seq.Count; i++)
                {
                    if (Seq[i].Contains("Sequence"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = Seq[i];
                        SeqLabelComboBox.Items.Add(tempItem);
                    }
                }
            }
            if (SeqtBubble.Visibility == Visibility.Hidden)
            {
                SeqtBubble.Visibility = Visibility.Visible;
            }
            else if (SeqtBubble.Visibility == Visibility.Visible)
            {
                SeqtBubble.Visibility = Visibility.Hidden;
            }
        }

        private void StateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StLabelComboBox.SelectedIndex == -1) StLabelComboBox.SelectedIndex = 0;
            if (StIDComboBox.SelectedIndex == -1) StIDComboBox.SelectedIndex = 0;
            if (Neo4j64.isConnected && StLabelComboBox.Items.Count == 1)
            {
                List<string> State = new List<string>();
                try
                {
                    State = Neo4j64.QueryFeatures();
                }
                catch
                {

                }

                for (int i = 0; i < State.Count; i++)
                {
                    if (State[i].Contains("State"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = State[i];
                        StLabelComboBox.Items.Add(tempItem);
                    }
                }
            }
            if (StateBubble.Visibility == Visibility.Hidden)
            {
                StateBubble.Visibility = Visibility.Visible;
            }
            else if (StateBubble.Visibility == Visibility.Visible)
            {
                StateBubble.Visibility = Visibility.Hidden;
            }
        }

        private void PUButton_Click_17(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "RelationFile|*.csv";
            dialog.Title = "打开序列-序列关系文件";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = dialog.FileNames;
                for(int i = 0; i < files.Length; i++)
                {
                    SeqSeqFileTextBox.Text += files[i] + ";";
                }
            }
        }

        private void PUButton_Click_20(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "RelationFile|*.csv";
            dialog.Title = "打开状态-状态关系文件";         
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string[] files = dialog.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    StStFileTextBox.Text += files[i] + ";";
                }
            }
        }

        private void ProcessTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProcessTab.SelectedIndex == 1)
            {
                if (LayerComboBox.Items.Count == 1 && Neo4j64.isConnected)
                {
                    List<string> Layers = Neo4j64.QueryLayers();
                    for (int i = 0; i < Layers.Count; i++)
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = Layers[i];
                        LayerComboBox.Items.Add(tempItem);
                    }
                }
            }
            if (ProcessTab.SelectedIndex == 2)
            {
                //
            }
            if (ProcessTab.SelectedIndex == 3)
            {
                if (Neo4j64.isConnected && LabelRelComboBox.Items.Count == 1)
                {
                    Thread t = new Thread(() =>
                    {
                        List<string> tempNodeLabels = Neo4j64.QueryFeatures();
                        if (tempNodeLabels != null)
                        {
                            for (int i = 0; i < tempNodeLabels.Count; i++)
                            {
                                if (Neo4j64.HasRelation(tempNodeLabels[i], tempNodeLabels[i]))
                                {
                                    Dispatcher.Invoke(new Action(delegate
                                    {
                                        PUComboBoxItem tempItem = new PUComboBoxItem();
                                        tempItem.Content = tempNodeLabels[i];
                                        LabelRelComboBox.Items.Add(tempItem);
                                    }));
                                }
                            }

                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            }
        }

        private void EventLabelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = EventIDComboBox.Items.Count - 1; i > 0; i--)
            {
                EventIDComboBox.Items.RemoveAt(i);
            }
            EventIDComboBox.SelectedIndex = 0;
            if (EventLabelComboBox.SelectedIndex > 0)
            {
                if (SeqLabelComboBox.SelectedIndex > 0 && Neo4j64.HasRelation(EventLabelComboBox.SelectedValue.ToString(), SeqLabelComboBox.SelectedValue.ToString()))
                {
                    EtSeqDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    EtSeqDelBtn.Visibility = Visibility.Hidden;
                }
                List<string> ID = Neo4j64.QueryFeatureFields(EventLabelComboBox.SelectedValue.ToString());
                for (int i = 0; i < ID.Count; i++)
                {
                    if (ID[i].Contains("PRID"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        EventIDComboBox.Items.Add(tempItem);
                    }
                }
                if (EventIDComboBox.Items.Count == 1)//没找到PRID项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        if (ID[i].Contains("ID"))
                        {
                            PUComboBoxItem tempItem = new PUComboBoxItem();
                            tempItem.Content = ID[i];
                            EventIDComboBox.Items.Add(tempItem);
                        }
                    }
                }
                if (EventIDComboBox.Items.Count == 1)//没找到包含ID的项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        EventIDComboBox.Items.Add(tempItem);
                    }
                }
            }
            else
            {
                EtSeqDelBtn.Visibility = Visibility.Hidden;
            }
        }

        private void SeqLabelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = SeqIDComboBox.Items.Count - 1; i > 0; i--)
            {
                SeqIDComboBox.Items.RemoveAt(i);
            }
            SeqIDComboBox.SelectedIndex = 0;
            if (SeqLabelComboBox.SelectedIndex > 0)
            {
                if (Neo4j64.HasRelation(SeqLabelComboBox.SelectedValue.ToString(), SeqLabelComboBox.SelectedValue.ToString()))
                {
                    SeqSeqDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    SeqSeqDelBtn.Visibility = Visibility.Hidden;
                }
                if (EventLabelComboBox.SelectedIndex > 0 && Neo4j64.HasRelation(EventLabelComboBox.SelectedValue.ToString(), SeqLabelComboBox.SelectedValue.ToString()))
                {
                    EtSeqDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    EtSeqDelBtn.Visibility = Visibility.Hidden;
                }
                if (StLabelComboBox.SelectedIndex > 0 && Neo4j64.HasRelation(SeqLabelComboBox.SelectedValue.ToString(), StLabelComboBox.SelectedValue.ToString()))
                {
                    SeqStDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    SeqStDelBtn.Visibility = Visibility.Hidden;
                }
                List<string> ID = Neo4j64.QueryFeatureFields(SeqLabelComboBox.SelectedValue.ToString());
                for (int i = 0; i < ID.Count; i++)
                {
                    if (ID[i].Contains("SQID"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        SeqIDComboBox.Items.Add(tempItem);
                    }
                }
                if (SeqIDComboBox.Items.Count == 1)//没找到PRID项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        if (ID[i].Contains("ID"))
                        {
                            PUComboBoxItem tempItem = new PUComboBoxItem();
                            tempItem.Content = ID[i];
                            SeqIDComboBox.Items.Add(tempItem);
                        }
                    }
                }
                if (SeqIDComboBox.Items.Count == 1)//没找到包含ID的项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        SeqIDComboBox.Items.Add(tempItem);
                    }
                }
            }
            else
            {
                EtSeqDelBtn.Visibility = Visibility.Hidden;
                SeqSeqDelBtn.Visibility = Visibility.Hidden;
                SeqStDelBtn.Visibility = Visibility.Hidden;
            }
        }

        private void StLabelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = StIDComboBox.Items.Count - 1; i > 0; i--)
            {
                StIDComboBox.Items.RemoveAt(i);
            }
            StIDComboBox.SelectedIndex = 0;
            if (StLabelComboBox.SelectedIndex > 0)
            {
                if (Neo4j64.HasRelation(StLabelComboBox.SelectedValue.ToString(), StLabelComboBox.SelectedValue.ToString()))
                {
                    StStDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    StStDelBtn.Visibility = Visibility.Hidden;
                }
                if (SeqLabelComboBox.SelectedIndex > 0 && Neo4j64.HasRelation(SeqLabelComboBox.SelectedValue.ToString(), StLabelComboBox.SelectedValue.ToString()))
                {
                    SeqStDelBtn.Visibility = Visibility.Visible;
                }
                else
                {
                    SeqStDelBtn.Visibility = Visibility.Hidden;
                }
                List<string> ID = Neo4j64.QueryFeatureFields(StLabelComboBox.SelectedValue.ToString());
                for (int i = 0; i < ID.Count; i++)
                {
                    if (ID[i].Contains("STID"))
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        StIDComboBox.Items.Add(tempItem);
                    }
                }
                if (StIDComboBox.Items.Count == 1)//没找到PRID项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        if (ID[i].Contains("ID"))
                        {
                            PUComboBoxItem tempItem = new PUComboBoxItem();
                            tempItem.Content = ID[i];
                            StIDComboBox.Items.Add(tempItem);
                        }
                    }
                }
                if (StIDComboBox.Items.Count == 1)//没找到包含ID的项
                {
                    for (int i = 0; i < ID.Count; i++)
                    {
                        PUComboBoxItem tempItem = new PUComboBoxItem();
                        tempItem.Content = ID[i];
                        StIDComboBox.Items.Add(tempItem);
                    }
                }
            }
            else
            {
                SeqStDelBtn.Visibility = Visibility.Hidden;
                StStDelBtn.Visibility = Visibility.Hidden;
            }
        }

        private void PUButton_Click_21(object sender, RoutedEventArgs e)
        {
            if (isCreating) return;
            Thread t = new Thread(IncreamentCreateRel);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void IncreamentCreateRel()
        {
            if (!Neo4j64.isAdminRole)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    PUMessageBox.ShowDialog("当前用户无权限！");
                    return;
                }));
            }
            else
            {
                int TotalProcess = 0;
                string EtSeqCQL = string.Empty;     //创建事件-序列关系
                string SeqStCQL = string.Empty;     //创建序列-状态关系
                const string BelongTemplate = "MATCH(N:{0}),(M:{1}) WHERE N.{2}=M.{3} MERGE(N)-[:{4}]->(M)";
                const string RelTemplate = "MATCH(N:{0}),(M:{1}) WHERE N.{2}='{3}' AND M.{4}='{5}' MERGE(N)-[r:SRelationship{{StateAction:'{6}'}}]->(M)";
                List<Dictionary<string, string>> SeqSeqRel = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> StStRel = new List<Dictionary<string, string>>();
                #region 获取控件值以及读取数据
                int EventLabelIndex = -1;
                int EventIDIndex = -1;
                int SeqLabelIndex = -1;
                int SeqIDIndex = -1;
                int StLabelIndex = -1;
                int StIDIndex = -1;

                string EventLabelValue = string.Empty;
                string EventIDValue = string.Empty;
                string SeqLabelValue = string.Empty;
                string SeqIDValue = string.Empty;
                string StLabelValue = string.Empty;
                string StIDValue = string.Empty;

                string SeqSeqRelFile = string.Empty;
                string StStRelFile = string.Empty;

                Counter ProgressCounter = new Counter();
                //获取控件值
                Dispatcher.Invoke(new Action(delegate
                {
                    EventLabelIndex = EventLabelComboBox.SelectedIndex;
                    EventIDIndex = EventIDComboBox.SelectedIndex;
                    SeqLabelIndex = SeqLabelComboBox.SelectedIndex;
                    SeqIDIndex = SeqIDComboBox.SelectedIndex;
                    StLabelIndex = StLabelComboBox.SelectedIndex;
                    StIDIndex = StIDComboBox.SelectedIndex;

                    EventLabelValue = EventLabelComboBox.SelectedValue.ToString();
                    EventIDValue = EventIDComboBox.SelectedValue.ToString();
                    SeqLabelValue = SeqLabelComboBox.SelectedValue.ToString();
                    SeqIDValue = SeqIDComboBox.SelectedValue.ToString();
                    StLabelValue = StLabelComboBox.SelectedValue.ToString();
                    StIDValue = StIDComboBox.SelectedValue.ToString();

                    SeqSeqRelFile = SeqSeqFileTextBox.Text;
                    StStRelFile = StStFileTextBox.Text;
                }));

                if (EventLabelIndex > 0 && EventIDIndex > 0 && SeqLabelIndex > 0)
                {
                    TotalProcess++;
                    EtSeqCQL = string.Format(BelongTemplate, EventLabelValue, SeqLabelValue, EventIDValue, EventIDValue, "Belong");
                }
                if (SeqLabelIndex > 0 && SeqIDIndex > 0 && StLabelIndex > 0)
                {
                    TotalProcess++;
                    SeqStCQL = string.Format(BelongTemplate, SeqLabelValue, StLabelValue, SeqIDValue, SeqIDValue, "Belong");
                }
                if (SeqLabelIndex > 0 && SeqIDIndex > 0 && !SeqSeqRelFile.Equals(string.Empty))
                {
                    TotalProcess++;
                    string[] files = SeqSeqRelFile.Split(';');
                    for(int i = 0; i < files.Length; i++)
                    {
                        if (files[i] !=string.Empty)
                        {
                            StreamReader sr = new StreamReader(files[i]);
                            string line;
                            sr.ReadLine();//跳过第一行
                            while ((line = sr.ReadLine()) != null)
                            {
                                string[] temp = line.Split(',');
                                Dictionary<string, string> lineDic = new Dictionary<string, string>();
                                if (temp.Length == 3)
                                {
                                    lineDic.Add("FromID", temp[0]);
                                    lineDic.Add("ToID", temp[1]);
                                    lineDic.Add("Property", temp[2]);
                                }
                                if (temp.Length == 2)
                                {
                                    lineDic.Add("FromID", temp[0]);
                                    lineDic.Add("ToID", temp[1]);
                                    lineDic.Add("Property", "Belong");
                                }
                                SeqSeqRel.Add(lineDic);
                            }
                        }
                    }
                    
                }
                if (StLabelIndex > 0 && StIDIndex > 0 && !StStRelFile.Equals(string.Empty))
                {
                    TotalProcess++;
                    string[] files = StStRelFile.Split(';');
                    for(int i = 0; i < files.Length; i++)
                    {
                        if (files[i] != string.Empty)
                        {
                            StreamReader sr = new StreamReader(files[i]);
                            string line;
                            sr.ReadLine();//跳过第一行
                            while ((line = sr.ReadLine()) != null)
                            {
                                string[] temp = line.Split(',');
                                Dictionary<string, string> lineDic = new Dictionary<string, string>();
                                if (temp.Length == 3)
                                {
                                    lineDic.Add("FromID", temp[0]);
                                    lineDic.Add("ToID", temp[1]);
                                    lineDic.Add("Property", temp[2]);
                                }
                                if (temp.Length == 2)
                                {
                                    lineDic.Add("FromID", temp[0]);
                                    lineDic.Add("ToID", temp[1]);
                                    lineDic.Add("Property", "Belong");
                                }
                                StStRel.Add(lineDic);
                            }
                        }
                    }                   
                }
                #endregion
                int Step = 0;
                if (!EtSeqCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建事件-序列关系...";
                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(EtSeqCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        EtSeqDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (!SeqStCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-状态关系...";

                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(SeqStCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        SeqStDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (SeqSeqRel.Count != 0)
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-序列关系...";
                    }));
                    Counter seqCounter = new Counter();
                    Parallel.For(0, SeqSeqRel.Count, (i) =>
                    {
                        seqCounter.Increment();
                        ProgressCounter.Increment();
                        try
                        {
                            Neo4j64.ExcuteCQL(string.Format(RelTemplate, SeqLabelValue, SeqLabelValue, SeqIDValue, SeqSeqRel[i]["FromID"], SeqIDValue, SeqSeqRel[i]["ToID"], SeqSeqRel[i]["Property"]));
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                    return;
                            }));
                        }
                        Dispatcher.Invoke(new Action(delegate
                        {
                            labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-序列关系..." + seqCounter.Count + "/" + SeqSeqRel.Count;
                            RelProgressBar.Percent = Math.Round((ProgressCounter.Count + 1) * 1.0 / (StStRel.Count + SeqSeqRel.Count), 3);
                        }));
                    });
                    Dispatcher.Invoke(new Action(delegate
                    {
                        SeqSeqDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (StStRel.Count != 0)
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建状态-状态关系...";
                    }));
                    Counter stCounter = new Counter();
                    Parallel.For(0, StStRel.Count, (i) =>
                    {
                        stCounter.Increment();
                        ProgressCounter.Increment();
                        try
                        {
                            Neo4j64.ExcuteCQL(string.Format(RelTemplate, StLabelValue, StLabelValue, StIDValue, StStRel[i]["FromID"], StIDValue, StStRel[i]["ToID"], StStRel[i]["Property"]));
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                    return;
                            }));
                        }
                        Dispatcher.Invoke(new Action(delegate
                        {
                            labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-序列关系..." + stCounter.Count + "/" + StStRel.Count;
                            RelProgressBar.Percent = Math.Round((ProgressCounter.Count + 1) * 1.0 / (StStRel.Count + SeqSeqRel.Count), 3);
                        }));
                    });
                    Dispatcher.Invoke(new Action(delegate
                    {
                        StStDelBtn.Visibility = Visibility.Visible;
                    }));
                }
                Dispatcher.Invoke(new Action(delegate
                {
                    isCreating = false;
                    if (TotalProcess > 0)
                    {
                        PUMessageBox.ShowDialog("完成!");
                        labelProgress.Content = "";
                        RelProgressBar.Percent = 0;
                        RelProgressBar.Visibility = Visibility.Hidden;
                    }
                }));
            }

        }

        private void BatchCreateRel()
        {
            if (!Neo4j64.isAdminRole)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    PUMessageBox.ShowDialog("当前用户无权限！");
                    return;
                }));
            }
            else
            {
                int TotalProcess = 0;
                string EtSeqCQL = string.Empty;     //创建事件-序列关系
                string SeqStCQL = string.Empty;     //创建序列-状态关系
                string SeqSeqCQL = string.Empty;    //创建序列-序列关系
                string StStCQL = string.Empty;      //创建状态-状态关系
                const string BelongTemplate = "MATCH(N:{0}),(M:{1}) WHERE N.{2}=M.{3} MERGE(N)-[:{4}]->(M)";
                const string RelTemplate = "USING PERIODIC COMMIT 1000 LOAD CSV WITH HEADERS FROM \"file:///{0}\" as line match(from: {1}{{{2}: line.FromID}}),match(to: {1}{{{2}: line.TOID}}) merge(from) -[r: SRelationship{{ StateAction: line.property}}]->(to)";


                #region 获取控件值以及读取数据
                int EventLabelIndex = -1;
                int EventIDIndex = -1;
                int SeqLabelIndex = -1;
                int SeqIDIndex = -1;
                int StLabelIndex = -1;
                int StIDIndex = -1;

                string EventLabelValue = string.Empty;
                string EventIDValue = string.Empty;
                string SeqLabelValue = string.Empty;
                string SeqIDValue = string.Empty;
                string StLabelValue = string.Empty;
                string StIDValue = string.Empty;

                string SeqSeqRelFile = string.Empty;
                string StStRelFile = string.Empty;

                Counter ProgressCounter = new Counter();
                //获取控件值
                Dispatcher.Invoke(new Action(delegate
                {
                    EventLabelIndex = EventLabelComboBox.SelectedIndex;
                    EventIDIndex = EventIDComboBox.SelectedIndex;
                    SeqLabelIndex = SeqLabelComboBox.SelectedIndex;
                    SeqIDIndex = SeqIDComboBox.SelectedIndex;
                    StLabelIndex = StLabelComboBox.SelectedIndex;
                    StIDIndex = StIDComboBox.SelectedIndex;

                    EventLabelValue = EventLabelComboBox.SelectedValue.ToString();
                    EventIDValue = EventIDComboBox.SelectedValue.ToString();
                    SeqLabelValue = SeqLabelComboBox.SelectedValue.ToString();
                    SeqIDValue = SeqIDComboBox.SelectedValue.ToString();
                    StLabelValue = StLabelComboBox.SelectedValue.ToString();
                    StIDValue = StIDComboBox.SelectedValue.ToString();

                    SeqSeqRelFile = SeqSeqFileTextBox.Text;
                    StStRelFile = StStFileTextBox.Text;
                }));

                if (EventLabelIndex > 0 && EventIDIndex > 0 && SeqLabelIndex > 0)
                {
                    TotalProcess++;
                    EtSeqCQL = string.Format(BelongTemplate, EventLabelValue, SeqLabelValue, EventIDValue, EventIDValue, "Belong");
                }
                if (SeqLabelIndex > 0 && SeqIDIndex > 0 && StLabelIndex > 0)
                {
                    TotalProcess++;
                    SeqStCQL = string.Format(BelongTemplate, SeqLabelValue, StLabelValue, SeqIDValue, SeqIDValue, "Belong");
                }
                if (SeqLabelIndex > 0 && SeqIDIndex > 0 && !SeqSeqRelFile.Equals(string.Empty))
                {
                    TotalProcess++;
                    SeqSeqCQL = string.Format(RelTemplate, SeqSeqRelFile, SeqLabelValue, SeqIDValue);
                }
                if (StLabelIndex > 0 && StIDIndex > 0 && !StStRelFile.Equals(string.Empty))
                {
                    TotalProcess++;
                    StStCQL = string.Format(RelTemplate, StStRelFile, StLabelValue, StIDValue);
                }
                #endregion
                int Step = 0;
                if (!EtSeqCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建事件-序列关系...";
                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(EtSeqCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        EtSeqDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (!SeqStCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-状态关系...";

                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(SeqStCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        SeqStDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (!SeqSeqCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建序列-序列关系...";
                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(SeqSeqCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        SeqSeqDelBtn.Visibility = Visibility.Visible;
                    }));

                }
                if (!StStCQL.Equals(string.Empty))
                {
                    Step++;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        //RelProgressBar.Visibility = Visibility.Visible;
                        labelProgress.Content = "当前进度（" + Step + "/" + TotalProcess + "）:创建状态-状态关系...";
                    }));
                    try
                    {
                        Neo4j64.ExcuteCQL(StStCQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            if (PUMessageBox.ShowConfirm("创建失败:" + ex.Message + "\r\n是否继续?") == false)
                                return;
                        }));
                    }


                    Dispatcher.Invoke(new Action(delegate
                    {
                        StStDelBtn.Visibility = Visibility.Visible;
                    }));
                }
                Dispatcher.Invoke(new Action(delegate
                {
                    isCreating = false;
                    if (TotalProcess > 0)
                    {
                        PUMessageBox.ShowDialog("完成!");
                        labelProgress.Content = "";
                        RelProgressBar.Percent = 0;
                        RelProgressBar.Visibility = Visibility.Hidden;
                    }
                }));
            }
        }

        private void EtSeqDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PUMessageBox.ShowConfirm("关系删除后无法恢复，是否继续？") == true)
            {
                string CQL = "MATCH(N:" + EventLabelComboBox.SelectedValue + ")-[r]->(M:" + SeqLabelComboBox.SelectedValue + ") DELETE r";
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {
                        PUMessageBox.ShowDialog("当前用户无权限！");
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(CQL);
                        EtSeqDelBtn.Visibility = Visibility.Hidden;
                        PUMessageBox.ShowDialog("已删除!");
                    }
                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                }
            }
        }

        private void SeqSeqDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PUMessageBox.ShowConfirm("关系删除后无法恢复，是否继续？") == true)
            {
                string CQL = "MATCH(N:" + SeqLabelComboBox.SelectedValue + ")-[r]->(M:" + SeqLabelComboBox.SelectedValue + ") DELETE r";
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        PUMessageBox.ShowDialog("当前用户无权限！");
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(CQL);
                        SeqSeqDelBtn.Visibility = Visibility.Hidden;
                        PUMessageBox.ShowDialog("已删除!");
                    }

                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                }
            }
        }

        private void SeqStDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PUMessageBox.ShowConfirm("关系删除后无法恢复，是否继续？") == true)
            {
                string CQL = "MATCH(N:" + SeqLabelComboBox.SelectedValue + ")-[r]->(M:" + StLabelComboBox.SelectedValue + ") DELETE r";
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        PUMessageBox.ShowDialog("当前用户无权限！");
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(CQL);
                        SeqStDelBtn.Visibility = Visibility.Hidden;
                        PUMessageBox.ShowDialog("已删除!");
                    }

                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                }
            }
        }

        private void StStDelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PUMessageBox.ShowConfirm("关系删除后无法恢复，是否继续？") == true)
            {
                string CQL = "MATCH(N:" + StLabelComboBox.SelectedValue + ")-[r]->(M:" + StLabelComboBox.SelectedValue + ") DELETE r";
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        PUMessageBox.ShowDialog("当前用户无权限！");
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(CQL);
                        StStDelBtn.Visibility = Visibility.Hidden;
                        PUMessageBox.ShowDialog("已删除!");
                    }

                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                }
            }
        }

        private void LayerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                int index = -1;
                Dispatcher.Invoke(new Action(delegate
                {
                    index = LayerComboBox.SelectedIndex;
                }));
                if (Neo4j64.isConnected && index > 0)
                {
                    string CQL = string.Empty;
                    Dispatcher.Invoke(new Action(delegate
                    {
                        CQL = "match (n)-[:RTREE_ROOT]-()-[:RTREE_CHILD*]->()-[:RTREE_REFERENCE*]->(m) where n.layer=\"" + LayerComboBox.SelectedValue + "\" WITH m limit 1 UNWIND labels(m) as LABELS return LABELS";
                    }));
                    List<List<string>> Res = new List<List<string>>();
                    try
                    {
                        Res = Neo4j64.QueryNonNodeDataTable(CQL);
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog(ex.Message);
                        }));
                    }
                    if (Res.Count > 0)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            NodeLabel.Text = Res[0][0];
                            LabelBubble.Visibility = Visibility.Visible;
                        }));

                    }
                    else
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            NodeLabel.Text = "";
                        }));
                    }
                }
                else
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        NodeLabel.Text = "";
                        LabelBubble.Visibility = Visibility.Hidden;
                    }));
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void PUButton_Click_18(object sender, RoutedEventArgs e)
        {
            if (isCreating) return;
            Thread t = new Thread(BatchCreateRel);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void PUButton_Click_19(object sender, RoutedEventArgs e)
        {
            if (LayerBubble.Visibility == Visibility.Hidden)
            {
                LayerBubble.Visibility = Visibility.Visible;
                if (LayerComboBox.SelectedIndex > 0)
                {
                    LabelBubble.Visibility = Visibility.Visible;
                }
            }
            else if (LayerBubble.Visibility == Visibility.Visible)
            {
                LayerBubble.Visibility = Visibility.Hidden;
                LabelBubble.Visibility = Visibility.Hidden;
            }
        }

        private void PUButton_Click_22(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                string NodeLabel = string.Empty;
                Dispatcher.Invoke(new Action(delegate
                {
                    NodeLabel = LabelRelComboBox.SelectedValue.ToString();
                }));
                string CQL1 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree=0 AND outDegree=1 set p.StateType = 0";
                string CQL2 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree=1 AND outDegree=0 set p.StateType = 1";
                string CQL3 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree=1 AND outDegree>1 or inDegree=0 AND outDegree>1  set p.StateType = 2";
                string CQL4 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree>1 AND outDegree=1 or inDegree>1 AND outDegree=0  set p.StateType = 3";
                string CQL5 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree>1 AND outDegree>1 set p.StateType = 4";
                string CQL6 = "MATCH (p:{0}) WITH p, size ((p) -[:SRelationship]-> ()) AS outDegree, size ((p) < -[:SRelationship]-()) AS inDegree WHERE inDegree=1 AND outDegree=1 set p.StateType = 5";

                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("当前用户无权限！");
                        }));
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(string.Format(CQL1, NodeLabel));
                        Neo4j64.ExcuteCQL(string.Format(CQL2, NodeLabel));
                        Neo4j64.ExcuteCQL(string.Format(CQL3, NodeLabel));
                        Neo4j64.ExcuteCQL(string.Format(CQL4, NodeLabel));
                        Neo4j64.ExcuteCQL(string.Format(CQL5, NodeLabel));
                        Neo4j64.ExcuteCQL(string.Format(CQL6, NodeLabel));

                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("计算状态完成!");
                        }));
                    }

                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        PUMessageBox.ShowDialog("计算失败:" + ex.Message);
                    }));
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void PUButton_Click_23(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(() =>
            {
                string NodeLabel = string.Empty;
                Dispatcher.Invoke(new Action(delegate
                {
                    NodeLabel = LabelRelComboBox.SelectedValue.ToString();
                }));
                string CQL = "MATCH (p:{0}) REMOVE p.StateType";
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("当前用户无权限！");
                        }));
                        return;
                    }
                    else
                    {
                        Neo4j64.ExcuteCQL(string.Format(CQL, NodeLabel));

                        Dispatcher.Invoke(new Action(delegate
                        {
                            PUMessageBox.ShowDialog("删除成功!");
                        }));
                    }

                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new Action(delegate
                    {
                        PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                    }));
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void PUButton_Click_24(object sender, RoutedEventArgs e)
        {
           
        }

        private void PUTabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            
        }
        public void UpdateUserTable()
        {
            Dispatcher.Invoke(new Action(delegate
            {
                UserInfo.Clear();
                dataTableUser.Rows.Clear();
            }));
            List<List<string>> res = Neo4j64.QueryNonNodeDataTable("call dbms.security.listUsers");
            for (int i = 0; i < res.Count; i++)
            {
                Dictionary<string, string> user = new Dictionary<string, string>();
                DataRow newRow = null;
                Dispatcher.Invoke(new Action(delegate
                {

                    newRow = dataTableUser.NewRow();
                }));
                newRow["用户名"] = res[i][0];
                newRow["角色组"] = res[i][1];
                string info = string.Empty;
                if (res[i][2] == string.Empty)
                {
                    newRow["状态"] = "可用";
                    info = "可用";
                }
                else if (res[i][2].Contains("is_suspended"))
                {
                    newRow["状态"] = "暂停使用";
                    info = "暂停使用";
                }
                else if (res[i][2].Contains("password_change_required"))
                {
                    newRow["状态"] = "需重设密码";
                    info = "需重设密码";
                }

                Dispatcher.Invoke(new Action(delegate
                {
                    dataTableUser.Rows.Add(newRow);
                }));


                user.Add("username", res[i][0]);
                user.Add("roles", res[i][1]);
                user.Add("flags", info);
                try
                {
                    UserInfo.Add(res[i][0], user);
                }
                catch
                {

                }

            }
        }

        public void UpdateUserInfo()
        {
            try
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    if (UserComboBox.Items.Count == 1)
                    {
                        for (int i = 0; i < UserInfo.Count; i++)
                        {
                            PUComboBoxItem tempItem = new PUComboBoxItem();
                            tempItem.Content = UserInfo.Keys.ToList()[i];
                            UserComboBox.Items.Add(tempItem);
                        }
                    }
                    UserInfoTable.ItemsSource = dataTableUser.DefaultView;
                    if (UserComboBox.SelectedIndex > 0)
                    {
                        UserTextBox.Text = UserComboBox.SelectedValue.ToString();
                        if (UserInfo[UserComboBox.SelectedValue.ToString()]["roles"].ToString().ToUpper().Contains("ADMIN"))
                        {
                            AdminCheckBox.IsChecked = true;
                        }
                        if (UserInfo[UserComboBox.SelectedValue.ToString()]["roles"].ToString().ToUpper().Contains("READER"))
                        {
                            ReaderCheckBox.IsChecked = true;
                        }
                        if (UserInfo[UserComboBox.SelectedValue.ToString()]["roles"].ToString().ToUpper().Contains("PUBLISHER"))
                        {
                            PublisherCheckBox.IsChecked = true;
                        }
                        if (UserInfo[UserComboBox.SelectedValue.ToString()]["roles"].ToString().ToUpper().Contains("ARCHITECT"))
                        {
                            ArchitectCheckBox.IsChecked = true;
                        }
                        UserStateLabel.Content = "用户状态：" + UserInfo[UserComboBox.SelectedValue.ToString()]["flags"];
                    }
                }));



            }
            catch (Exception ex)
            {
                PUMessageBox.ShowDialog(ex.Message);
            }
        }

        private void PUButton_Click_25(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PUButton_Click_26(object sender, RoutedEventArgs e)
        {
            
           

        }

        private void UserComboBOx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdminCheckBox.IsChecked = false;
            ReaderCheckBox.IsChecked = false;
            PublisherCheckBox.IsChecked = false;
            ArchitectCheckBox.IsChecked = false;
            UserTextBox.Text = "";
            PwdTextBox.Password = "";
            RePwdTextBox.Password = "";
            if (UserComboBox.SelectedIndex == 0)
            {
                CreateBtn.Content = "创建用户";
                CreateBtn.IsEnabled = true;
                UserStateLabel.Content = "用户状态：待创建";
                UserInfoTable.SelectedItems.Clear();
                SuspendBtn.Visibility = Visibility.Hidden;
            }
            else
            {
                CreateBtn.Content = "删除用户";
                SuspendBtn.Visibility = Visibility.Visible;
                if (UserInfo[UserComboBox.SelectedValue.ToString()]["flags"] == "暂停使用")
                {
                    SuspendBtn.Content = "激活";
                }
                else
                {
                    SuspendBtn.Content = "停用";
                }
                UpdateUserInfo();
            }
        }

        private void PUButton_Click_27(object sender, RoutedEventArgs e)
        {
            if (CreateBtn.Content.ToString() == "创建用户")
            {
                if (PwdTextBox.Password == string.Empty)
                {
                    PUMessageBox.ShowDialog("密码不能为空！");
                    return;
                }
                if (PwdTextBox.Password == RePwdTextBox.Password)
                {
                    try
                    {
                        if (!Neo4j64.isAdminRole)
                        {

                            PUMessageBox.ShowDialog("当前用户无权限！");
                            return;
                        }
                        else
                        {
                            string CQL = "call dbms.security.createUser('{0}','{1}',false)";
                            string GrantCQL = "call dbms.security.addRoleToUser('{0}','{1}')";
                            Neo4j64.ExcuteCQL(string.Format(CQL, UserTextBox.Text, PwdTextBox.Password));
                            if (AdminCheckBox.IsChecked == true)
                            {
                                Neo4j64.ExcuteCQL(string.Format(GrantCQL, "admin", UserTextBox.Text));
                            }
                            if (ReaderCheckBox.IsChecked == true)
                            {
                                Neo4j64.ExcuteCQL(string.Format(GrantCQL, "reader", UserTextBox.Text));
                            }
                            if (PublisherCheckBox.IsChecked == true)
                            {
                                Neo4j64.ExcuteCQL(string.Format(GrantCQL, "publisher", UserTextBox.Text));
                            }
                            if (ArchitectCheckBox.IsChecked == true)
                            {
                                Neo4j64.ExcuteCQL(string.Format(GrantCQL, "architect", UserTextBox.Text));
                            }
                            UpdateUserTable();
                            for (int i = UserComboBox.Items.Count - 1; i > 0; i--)
                            {
                                UserComboBox.Items.RemoveAt(i);
                            }
                            UpdateUserInfo();
                            for (int i = 0; i < UserComboBox.Items.Count; i++)
                            {
                                if ((UserComboBox.Items[i] as PUComboBoxItem).Content.ToString() == UserTextBox.Text)
                                {
                                    UserComboBox.SelectedIndex = i;
                                    break;
                                }
                            }
                            PUMessageBox.ShowDialog("已创建用户!");
                        }

                    }
                    catch (Exception ex)
                    {
                        PUMessageBox.ShowDialog("创建失败:" + ex.Message);
                    }

                }
                else
                {
                    PUMessageBox.ShowDialog("密码不一致！");
                    return;
                }
            }
            else if (CreateBtn.Content.ToString() == "删除用户")
            {
                try
                {
                    if (!Neo4j64.isAdminRole)
                    {

                        PUMessageBox.ShowDialog("当前用户无权限！");
                        return;
                    }
                    else
                    {
                        string CQL = "call dbms.security.deleteUser('{0}')";
                        Neo4j64.ExcuteCQL(string.Format(CQL, UserTextBox.Text));
                        UpdateUserTable();
                        for (int i = UserComboBox.Items.Count - 1; i > 0; i--)
                        {
                            UserComboBox.Items.RemoveAt(i);
                        }
                        UserComboBox.SelectedIndex = 0;
                        UpdateUserInfo();
                        PUMessageBox.ShowDialog("已删除用户!");
                    }
                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog("删除失败:" + ex.Message);
                }
            }

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UserComboBox.SelectedIndex == 0) return;
            if (!Neo4j64.isAdminRole)
            {
                PUMessageBox.ShowDialog("当前用户无权限！");
                return;
            }
            else
            {
                try
                {
                    string[] temp = UserInfo[UserComboBox.SelectedValue.ToString()]["roles"].Split(' ');
                    string CQL = "call dbms.security.removeRoleFromUser('{0}','{1}')";
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i] != string.Empty)
                        {
                            try
                            {
                                Neo4j64.ExcuteCQL(string.Format(CQL, temp[i], UserComboBox.SelectedValue.ToString()));

                            }
                            catch (Exception ex)
                            {
                                PUMessageBox.ShowDialog(ex.Message);
                            }
                        }
                    }
                }
                catch
                {

                }
               
                try
                {
                    if (RePwdTextBox.Password != PwdTextBox.Password)
                    {
                        PUMessageBox.ShowDialog("密码不一致!");
                        return;
                    }
                    if (PwdTextBox.Password != string.Empty && RePwdTextBox.Password == PwdTextBox.Password)
                    {
                        string PwdCQL = "call dbms.security.changeUserPassword('{0}','{1}',false)";
                        Neo4j64.ExcuteCQL(string.Format(PwdCQL, UserComboBox.SelectedValue, PwdTextBox.Password));
                    }
                    string GrantCQL = "call dbms.security.addRoleToUser('{0}','{1}')";
                    if (AdminCheckBox.IsChecked == true)
                    {
                        Neo4j64.ExcuteCQL(string.Format(GrantCQL, "admin", UserTextBox.Text));
                    }
                    if (ReaderCheckBox.IsChecked == true)
                    {
                        Neo4j64.ExcuteCQL(string.Format(GrantCQL, "reader", UserTextBox.Text));
                    }
                    if (PublisherCheckBox.IsChecked == true)
                    {
                        Neo4j64.ExcuteCQL(string.Format(GrantCQL, "publisher", UserTextBox.Text));
                    }
                    if (ArchitectCheckBox.IsChecked == true)
                    {
                        Neo4j64.ExcuteCQL(string.Format(GrantCQL, "architect", UserTextBox.Text));
                    }
                    UpdateUserTable();
                    PUMessageBox.ShowDialog("保存成功!");
                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog(ex.Message);
                    return;
                }               
            }

        }

        private void SuspendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Neo4j64.isAdminRole)
            {

                PUMessageBox.ShowDialog("当前用户无权限！");
                return;
            }
            else
            {
                try
                {
                    if (SuspendBtn.Content.ToString() == "激活")
                    {
                        string CQL = "call dbms.security.activateUser('{0}',false)";
                        Neo4j64.ExcuteCQL(string.Format(CQL, UserComboBox.SelectedValue));
                        UpdateUserTable();
                        SuspendBtn.Content = "停用";
                        UserStateLabel.Content = "用户状态：" + UserInfo[UserComboBox.SelectedValue.ToString()]["flags"];
                        //PUMessageBox.ShowDialog("用户已经激活！");
                    }
                    else
                    {
                        string CQL = "call dbms.security.suspendUser('{0}')";
                        Neo4j64.ExcuteCQL(string.Format(CQL, UserComboBox.SelectedValue));
                        UpdateUserTable();
                        SuspendBtn.Content = "激活";
                        UserStateLabel.Content = "用户状态：" + UserInfo[UserComboBox.SelectedValue.ToString()]["flags"];
                        //PUMessageBox.ShowDialog("用户已经停用！");
                    }
                }
                catch (Exception ex)
                {
                    PUMessageBox.ShowDialog(ex.Message);
                }

            }

        }

        private void PUButton_Click_28(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择.shp数据文件夹路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    //System.Windows.MessageBox.Show(this, "文件夹路径不能为空", "提示");
                    PUMessageBox.ShowDialog("文件夹路径不能为空", "提示");
                    return;
                }
                ShpFileIncreamentTextBox.Text = dialog.SelectedPath;
            }
        }

        private void PUButton_Click_29(object sender, RoutedEventArgs e)
        {
            if (LayerInfoTable.SelectedItems.Count < 1) return;
            if (PUMessageBox.ShowConfirm("图层删除后无法恢复，是否继续？") == true)
            {
                this.IsAwaitShow = true;
                string CQL = "call spatial.removeLayer('{0}')";
                if (!Neo4j64.isAdminRole)
                {
                    PUMessageBox.ShowDialog("当前用户无权限！");
                    return;
                }
                else
                {
                    Thread t = new Thread(() =>
                    {
                        List<string> Layers = new List<string>();
                        Dispatcher.Invoke(new Action(delegate
                        {
                            for(int i = 0; i < LayerInfoTable.SelectedItems.Count; i++)
                            {
                                Layers.Add((LayerInfoTable.SelectedItems[i] as DataRowView).Row["图层名"].ToString());
                            }
                        }));
                        try
                        {
                            int CountLayers = 0;
                            Dispatcher.Invoke(new Action(delegate
                            {
                                CountLayers = LayerInfoTable.SelectedItems.Count;
                            }));                           
                            for(int i = 0; i < CountLayers; i++)
                            {
                                Neo4j64.ExcuteCQL(string.Format(CQL, Layers[i]));
                            }                          
                            Dispatcher.Invoke(new Action(delegate
                            {
                                this.IsAwaitShow = false;
                                if (dataTableLayer.Rows.Count != 0)
                                {
                                    dataTableLayer.Rows.Clear();
                                    UpdateLayerInfo();
                                }
                                PUMessageBox.ShowDialog("删除成功!");
                            }));
                        }
                        catch (Exception ex)
                        {
                            Dispatcher.Invoke(new Action(delegate
                            {
                                this.IsAwaitShow = false;
                                PUMessageBox.ShowDialog(ex.Message);
                            }));

                        }
                    });
                    t.SetApartmentState(ApartmentState.STA);
                    t.Start();
                }
            }
        }

        private void PUTabControl_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {
            if (ImportTabl.SelectedValue.ToString() == "增量导入")
            {
                Thread t = new Thread(() =>
                {
                    if (dataTableUser.Rows.Count == 0)
                    {
                        UpdateLayerInfo();
                    }
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }

        }
        private void UpdateLayerInfo()
        {
            try
            {
                int RowCount = 0;
                Dispatcher.Invoke(new Action(delegate
                {
                    RowCount = dataTableLayer.Rows.Count;
                }));
                Dispatcher.Invoke(new Action(delegate
                {
                    //dataTableLayer.Rows.Clear();
                }));
                if (RowCount == 0)
                {
                    List<string> Layers = Neo4j64.QueryLayers();
                    for (int i = 0; i < Layers.Count; i++)
                    {
                        List<Dictionary<string, string>> tempNode = Neo4j64.QueryNodeDataTable(string.Format("MATCH(N:ReferenceNode)-[:LAYER]->(M) WHERE M.layer='{0}' RETURN M", Layers[i]));
                        if (tempNode.Count > 0)
                        {
                            DataRow newRow = dataTableLayer.NewRow();
                            for (int j = 0; j < tempNode[0].Count; j++)
                            {
                                //dataTableLayer.Columns.Add("图层名");
                                //dataTableLayer.Columns.Add("创建时间");
                                //dataTableLayer.Columns.Add("编码类型");
                                //dataTableLayer.Columns.Add("索引类型");
                                //dataTableLayer.Columns.Add("多边形数量");                          
                                newRow["图层名"] = tempNode[0]["layer"];
                                newRow["图层类型"] = tempNode[0]["layer_class"].Substring(tempNode[0]["layer_class"].LastIndexOf('.') + 1);
                                newRow["编码类型"] = tempNode[0]["geomencoder"].Substring(tempNode[0]["geomencoder"].LastIndexOf('.') + 1);
                                newRow["索引类型"] = tempNode[0]["index_class"].Substring(tempNode[0]["index_class"].LastIndexOf('.') + 1);

                                string CQL = "MATCH(N:ReferenceNode)-[:LAYER]->(M)-[:RTREE_METADATA]->(P) WHERE M.layer='{0}' return P.totalGeometryCount";
                                List<List<string>> RES = Neo4j64.QueryNonNodeDataTable(string.Format(CQL, Layers[i]));
                                newRow["多边形数量"] = RES[0].Count > 0 ? RES[0][0] : "0";


                                DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                newRow["创建时间"] = dt.AddMilliseconds(Convert.ToDouble(tempNode[0]["ctime"])).ToLocalTime().ToString("yyyy'/'MM'/'dd HH:mm:ss");
                            }
                            Dispatcher.Invoke(new Action(delegate
                            {
                                dataTableLayer.Rows.Add(newRow);
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(new Action(delegate
                {
                    PUMessageBox.ShowDialog(ex.Message);
                }));              
            }
        }

        private void ImpIncreamentBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Neo4j64.isAdminRole)
            {

                PUMessageBox.ShowDialog("当前用户无权限！");
                return;
            }
            else
            {
                if (LayerIncreamentTextBox.Text == string.Empty || ShpFileIncreamentTextBox.Text == string.Empty)
                {
                    PUMessageBox.ShowDialog("图层名或文件路径不能为空！");
                    return;
                }
                List<string> ShpFiles = new List<string>();
                string FilePath = ShpFileIncreamentTextBox.Text;
                DirectoryInfo root = new DirectoryInfo(FilePath);
                foreach (FileInfo file in root.GetFiles())
                {
                    if (file.FullName.EndsWith(".shp"))
                    {
                        ShpFiles.Add(file.FullName.Replace("\\", "/"));
                    }
                }
                IncreamentProgressBar.Visibility = Visibility.Visible;
                IncreamentProgressBar.Percent = 0;
                string LayerName = LayerIncreamentTextBox.Text;
                Thread t = new Thread(() =>
                {
                    try
                    {
                        List<string> CurrentExistLayer = Neo4j64.QueryLayers();
                        if (!CurrentExistLayer.Contains(LayerName))
                            Neo4j64.ExcuteCQL(string.Format("call spatial.addLayer('{0}','wkb','')", LayerName));
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(new Action(delegate
                        {
                            IncreamentProgressBar.Visibility = Visibility.Hidden;
                            PUMessageBox.ShowDialog(ex.Message);
                            return;
                        }));
                    }                 
                    if (ShpFiles.Count > 20) //数量多的时候使用并行方式
                    {
                        Counter Counter = new Counter();
                        Parallel.For(0, ShpFiles.Count, (i) =>
                        {
                            try
                            {
                                Neo4j64.ExcuteCQL(string.Format("call spatial.importShapefileToLayer('{0}','{1}')", LayerName, ShpFiles[i]));
                                Counter.Increment();
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    IncreamentProgressBar.Percent = Math.Round((Counter.Count) * 1.0 / ShpFiles.Count, 3);
                                    ShpProgressLabel.Content = "已导入:" + Counter.Count + "/" + ShpFiles.Count;
                                }));

                            }
                            catch (Exception ex)
                            {
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    IncreamentProgressBar.Visibility = Visibility.Hidden;
                                    PUMessageBox.ShowDialog(ex.Message);
                                    return;
                                }));

                            }
                        });                       
                    }
                    else
                    {
                        Counter Counter = new Counter();
                        for (int i = 0; i < ShpFiles.Count; i++)
                        {
                            try
                            {
                                Neo4j64.ExcuteCQL(string.Format("call spatial.importShapefileToLayer('{0}','{1}')", LayerName, ShpFiles[i]));
                                Counter.Increment();
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    IncreamentProgressBar.Percent = Math.Round((Counter.Count) * 1.0 / ShpFiles.Count, 3);
                                    ShpProgressLabel.Content = "已导入:" + Counter.Count + "/" + ShpFiles.Count;
                                }));

                            }
                            catch (Exception ex)
                            {
                                Dispatcher.Invoke(new Action(delegate
                                {
                                    IncreamentProgressBar.Visibility = Visibility.Hidden;
                                    PUMessageBox.ShowDialog(ex.Message);
                                    return;
                                }));

                            }
                        }
                    }

                    Dispatcher.Invoke(new Action(delegate
                    {
                        ShpProgressLabel.Content = "";
                        IncreamentProgressBar.Visibility = Visibility.Hidden;
                        dataTableLayer.Rows.Clear();
                        UpdateLayerInfo();
                        PUMessageBox.ShowDialog("导入完成！");                        
                    }));
                });
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }           
        }

        private void UserInfoTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UserInfoTable.SelectedItems.Count>0)
            {
                string User = (string)(UserInfoTable.SelectedItem as DataRowView)["用户名"];
                UserComboBox.SelectedValue = User;
            }
        }
    }
}
