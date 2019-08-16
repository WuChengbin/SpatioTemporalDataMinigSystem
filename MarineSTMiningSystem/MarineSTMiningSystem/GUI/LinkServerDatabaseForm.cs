using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows.Forms;

namespace MarineSTMiningSystem
{
    public partial class LinkServerDatabaseForm : Form
    {
        public delegate void saveOracleConnection(OracleConnection _conn);//传参委托
        OracleConnection conn = new OracleConnection();
        saveOracleConnection soc;
        public LinkServerDatabaseForm(saveOracleConnection _soc)
        {
            InitializeComponent();
            soc = _soc;
        }

        public LinkServerDatabaseForm(saveOracleConnection _soc,string _dataBaseAddress)
        {
            InitializeComponent();
            soc = _soc;
            addressTextBox.Text = _dataBaseAddress;
        }

        private void linkButton_Click(object sender, EventArgs e)
        {
            string dataBaseAddress = addressTextBox.Text.Replace(" ","");//去掉空格
            string sid = sidTextBox.Text.Replace(" ", "");
            string userName = nameTextBox.Text.Replace(" ", "");
            string password = passwordTextBox.Text.Replace(" ", "");

            string connString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dataBaseAddress + ")(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=" + sid + ")));Persist Security Info=True;User ID=" + userName + ";Password=" + password + ";";//连接字符串

            conn = new OracleConnection(connString);
            try
            {
                conn.Open();//打开连接
                soc(conn);//保存连接

                //保存到类中
                DatabaseConnection.dataBaseAddress = dataBaseAddress;
                DatabaseConnection.sid = sid;
                DatabaseConnection.userName = userName;
                DatabaseConnection.password = password;
                DatabaseConnection.connString = connString;
                DatabaseConnection.orcConn = conn;

                MessageBox.Show("数据库连接成功","提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //this.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show("数据库连接失败："+err.Message,"错误", MessageBoxButtons.OK ,MessageBoxIcon.Error);
            }
        }
    }
}
