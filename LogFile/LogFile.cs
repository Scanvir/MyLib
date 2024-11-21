using System;
using System.IO;
using System.Text;

namespace MyLib
{
    public class LogFile
    {
        private readonly string logDir;
        private readonly string logFileName;
        private readonly string extension;

        private bool rotate;
        private string logFile;
        private DateTime currentDate;
        private int dayRotate;

        public LogFile(string logFile, bool rotate = false, int dayRotate = 5)
        {
            this.rotate = rotate;
            this.dayRotate = dayRotate;

            logDir = Path.GetDirectoryName(logFile);
            logFileName = Path.GetFileNameWithoutExtension(logFile);
            extension = Path.GetExtension(logFile);

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            currentDate = DateTime.Now;

            UpdateLogFileName();
        }
        private void UpdateLogFileName()
        {
            if (rotate)
            {
                string newFileName = $"{logFileName}.{currentDate:yyyyMMdd}{extension}";
                logFile = Path.Combine(logDir, newFileName);
            } else
            {
                string newFileName = $"{logFileName}{extension}";
                logFile = Path.Combine(logDir, newFileName);
            }
        }
        public void ToLog(string str)
        {
            if (currentDate.Date != DateTime.Now.Date)
            {
                currentDate = DateTime.Now;
                UpdateLogFileName();
            }

            try
            {
                File.AppendAllText(logFile, DateTime.Now.ToString("dd.MM.yy;HH:mm:ss;") + Environment.MachineName + ";" + str + ";" + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }
        public bool ClearLog()
        {
            try
            {
                File.WriteAllText(logFile, "");
                return true;
            }
            catch { return false; }
        }
        public bool TruncateLog(int dayCount)
        {
            try
            {
                if (File.Exists(logFile))
                {
                    string[] Log = File.ReadAllLines(logFile, Encoding.Default);
                    File.Delete(logFile);
                    for (int iii = 0; iii < Log.Length; iii++)
                    {
                        string[] row = Log[iii].Split(';');
                        if (DateTime.Parse(row[0]) < DateTime.Now.AddDays(-dayCount))
                            continue;

                        File.AppendAllText(logFile, Log[iii] + Environment.NewLine, Encoding.GetEncoding("UTF-8"));
                    }
                }
                return true;
            }
            catch { return false;  }
        }
        public void DeleteOldFiles()
        {
            string searchPattern = "*.log";
            var files = Directory.GetFiles(logDir, searchPattern);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.LastWriteTime < DateTime.Now.AddDays(-dayRotate))
                {
                    try
                    {
                        fileInfo.Delete();
                    }
                    catch { }
                }
            }
        }
    }
}
