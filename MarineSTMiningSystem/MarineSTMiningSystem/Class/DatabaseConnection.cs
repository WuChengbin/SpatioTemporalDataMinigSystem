using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MarineSTMiningSystem
{
    //数据库连接类
    static class DatabaseConnection
    {
        public static string dataBaseAddress = string.Empty;
        public static string sid = string.Empty;
        public static string userName = string.Empty;
        public static string password = string.Empty;
        public static string connString = string.Empty;//连接字符串
        public static OracleConnection orcConn;

        public static OracleConnection GetNewOracleConnection()
        {
            OracleConnection _conn = new OracleConnection(connString);
            return _conn;
        }
    }
}