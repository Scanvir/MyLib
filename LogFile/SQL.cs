using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace MyLib
{
    public class SQL : IDisposable
    {
        public string ConnectionString;
        public string ApplicationName;

        private SqlConnection connection;
        public bool isConnected = false;
        public SQL(string connectionString, string applicationName = "MyLib.dll")
        {
            ConnectionString = connectionString;
            ApplicationName = string.Format(";Application Name=\"{0}\"", applicationName);
        }

        public void OpenConnection()
        {
            try
            {
                connection = new SqlConnection(ConnectionString + ApplicationName);
                connection.StateChange += Connection_StateChange;

                connection.Open();
            }
            catch { }
        }
        public void CloseConnection()
        {
            try
            {
                connection.Close();
            }
            catch { }
        }
        private void Connection_StateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Open)
                isConnected = true;
            else
                isConnected = false;
        }
        public DataTable SelectQuery(string query, LogFile log, string DBName, int timeout = 30)
        {
            DataTable tbl = new DataTable();

            try
            {
                if (isConnected)
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.CommandTimeout = timeout;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(tbl);
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString + "Initial Catalog=" + DBName + ApplicationName))
                    {
                        conn.Open();

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandTimeout = timeout;

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(tbl);
                            }
                        }

                        conn.Close();
                    }
                }
            }
            catch (SqlException ex)
            {
                log.ToLog($"Query:\t{query}");
                log.ToLog($"Помилка 01 Select SQL:\t{ex.Message}");
            }
            catch (Exception ex)
            {
                log.ToLog($"Query:\t{query}");
                log.ToLog($"Помилка 02 Select SQL:\t{ex.Message}");
            }

            return tbl;
        }
        public bool Execute(string query, LogFile log, int timeout = 30, bool inTransaction = true)
        {
            try
            {
                if (isConnected)
                {
                    if (inTransaction)
                        using (SqlTransaction transaction = connection.BeginTransaction())
                        {
                            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                            {
                                cmd.CommandTimeout = timeout;
                                cmd.ExecuteNonQuery();

                                transaction.Commit();
                            }
                        }
                    else
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            cmd.CommandTimeout = timeout;
                            cmd.ExecuteNonQuery();
                        }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString + ApplicationName))
                    {
                        conn.Open();
                        if (inTransaction)
                            using (SqlTransaction transaction = conn.BeginTransaction())
                            {
                                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                                {
                                    cmd.CommandTimeout = timeout;
                                    cmd.ExecuteNonQuery();

                                    transaction.Commit();
                                }
                            }
                        else
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.CommandTimeout = timeout;
                                cmd.ExecuteNonQuery();
                            }
                    }
                }

                return true;
            }
            catch (SqlException ex)
            {
                log.ToLog($"Query:\t{query}");
                log.ToLog($"Помилка 01 Execute SQL:\t{ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                log.ToLog($"Query:\t{query}");
                log.ToLog($"Помилка 02 Execute SQL:\t{ex.Message}");
                return false;
            }
        }
        public double DoubleFromSQL(string param)
        {
            double result = 0;

            if (string.IsNullOrWhiteSpace(param))
            {
                return result;
            }

            if (double.TryParse(param.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Не можливо перетворити параметер у double: " + param);
            }
        }
        public void Dispose()
        {
            ConnectionString = null;
            ApplicationName = null;
            return;
        }
    }
}
