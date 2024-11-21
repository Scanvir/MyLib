using System.Diagnostics;
using System.IO;

namespace MyLib
{
    public class Copy
    {
        public static string CopyFiles(string from, string to, string logfile)
        {
            try
            {
                Process p = new Process();

                p.StartInfo.Arguments = string.Format(@"/C ROBOCOPY /MIR {0} {1} /log:{2}", from, to, logfile);
                p.StartInfo.FileName = "CMD.EXE";
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                p.Start();
                p.WaitForExit();
                if (p.HasExited)
                {
                    return "Ok";
                }
                return "Some problem";
            }
            catch
            {
                return "Ошибка при выполнении синхронизации";
            }
        }
        public bool CopyFile(string from, string to)
        {
            try
            {
                if (File.Exists(from))
                {
                    File.Copy(from, to, true);
                    return true;
                } else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
