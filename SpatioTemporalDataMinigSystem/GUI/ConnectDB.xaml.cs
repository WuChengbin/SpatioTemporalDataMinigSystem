using Panuon.UI;
using STDMS.DataOP;
using System;
using System.Threading;
using System.Windows;

namespace STDMS.GUI
{
    /// <summary>
    /// ConnectDB.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectDB
    {
        private MainWindow mainWindow = null;
        Thread ConnectThread = null;
        public ConnectDB(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            mainWindow.IsCoverMaskShow = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsAwaitShow = true;
            mainWindow.SetNeo4jConnection(textBoxAddress.Text, textBoxPort.Text, textBoxUser.Text, textBoxPwd.Password);
            ConnectThread = new Thread(new ParameterizedThreadStart(connectDB));
            ConnectThread.SetApartmentState(ApartmentState.STA);
            ConnectThread.Start(this);                           
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            mainWindow.IsCoverMaskShow = false;
        }

        private void connectDB(object Parent)
        {
            string Address = string.Empty;
            string Port = string.Empty;
            string User = string.Empty;
            string Pwd = string.Empty;
            string Type = string.Empty;
            Dispatcher.Invoke(new Action(delegate
            {
                Address = textBoxAddress.Text;
                Port = textBoxPort.Text;
                User = textBoxUser.Text;
                Pwd = textBoxPwd.Password;
                Type = ((Parent as ConnectDB).tabControl.SelectedItem as PUTabItem).Header.ToString();
            }));
            if (Type.ToUpper().Equals("NEO4J"))
            {
                try
                {
                    Neo4j64.Neo4jConnect(Address,Port, User, Pwd);
                    Dispatcher.Invoke(new Action(delegate
                    {
                        (Parent as ConnectDB).IsAwaitShow = false;
                        (Parent as ConnectDB).mainWindow.SetStatuLabel("Neo4j数据库: " + textBoxAddress.Text+ "["+User+"]" + " 连接成功");
                        PUMessageBox.ShowDialog("连接成功!", "Neo4j连接成功", Buttons.Yes, false);
                        (Parent as ConnectDB).Close();
                        mainWindow.IsCoverMaskShow = false;
                    }));
                }
                catch(Exception ex)
                {                  
                    Dispatcher.Invoke(new Action(delegate
                    {
                        (Parent as ConnectDB).IsAwaitShow = false;
                        PUMessageBox.ShowDialog( ex.Message, "Neo4j连接失败", Buttons.Yes, false);                       
                        (Parent as ConnectDB).mainWindow.SetStatuLabel("Neo4j数据库: " + textBoxAddress.Text + " 连接失败");
                    }));
                }
            }
        }
    }
}
