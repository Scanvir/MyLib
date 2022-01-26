using System;
using System.Data;
using System.Data.OleDb;
using System.Globalization;

namespace MyLib
{
    public class SQL
    {
        public string CONNECTIONSTRING;

        public SQL(string connectionString)
        {
            CONNECTIONSTRING = connectionString;
        }

        public DataTable SelectQuery(string query, LogFile log, string DBName, int timeout = 30)
        {
            OleDbConnection OleDbConn = new OleDbConnection(String.Format(@"{0}Initial Catalog={1}", CONNECTIONSTRING, DBName));
            OleDbCommand cmd = new OleDbCommand
            {
                CommandText = query,
                Connection = OleDbConn,
                CommandTimeout = timeout
            };
            
            OleDbDataAdapter da = new OleDbDataAdapter
            {
                SelectCommand = cmd
            };

            DataTable tbl = new DataTable();
            
            da.Fill(tbl);
            da.Dispose();

            OleDbConn.Close();
            OleDbConn.Dispose();

            return tbl;
        }
        public bool Execute(string query, LogFile log, int timeout = 30)
        {
            try
            {
                OleDbConnection OleDbConn = new OleDbConnection(CONNECTIONSTRING);
                OleDbCommand cmd = new OleDbCommand
                {
                    CommandText = query,
                    Connection = OleDbConn,
                    CommandTimeout = timeout
                };
                OleDbConn.Open();

                int count = cmd.ExecuteNonQuery();

                OleDbConn.Close();
                OleDbConn.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                log.ToLog("Ошибка БД: " + ex.Message);
                return false;
            }
        }
        public double DoubleFromSQL(string param)
        {
            if (param == null)
                return 0;

            if (param.Replace(" ", "") == "")
                return 0;

            return double.Parse(param.Replace(',', '.'), CultureInfo.InvariantCulture);
        }
    }
}
