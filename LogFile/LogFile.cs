using System;
using System.IO;
using System.Text;

namespace MyLib
{
    public class LogFile
    {
        public readonly string LOGFILE;

        public LogFile(string logFile)
        {
            LOGFILE = logFile;
        }

        public void ToLog(string str)
        {
            try
            {
                File.AppendAllText(LOGFILE, DateTime.Now.ToString("dd.MM.yy;HH:mm:ss;") + Environment.MachineName + ";" + str + ";" + Environment.NewLine, Encoding.GetEncoding("UTF-8"));
            }
            catch { }
        }
        public bool ClearLog()
        {
            try
            {
                File.WriteAllText(LOGFILE, "");
                return true;
            }
            catch { return false; }
        }
        public bool TruncateLog(int dayCount)
        {
            try
            {
                if (File.Exists(LOGFILE))
                {
                    string[] Log = File.ReadAllLines(LOGFILE, Encoding.Default);
                    File.Delete(LOGFILE);
                    for (int iii = 0; iii < Log.Length; iii++)
                    {
                        string[] row = Log[iii].Split(';');
                        if (DateTime.Parse(row[0]) < DateTime.Now.AddDays(-dayCount))
                            continue;

                        File.AppendAllText(LOGFILE, Log[iii] + Environment.NewLine, Encoding.GetEncoding("UTF-8"));
                    }
                }
                return true;
            }
            catch { return false; }
        }
    }
}
