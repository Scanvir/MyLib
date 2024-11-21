using System;
using System.IO;

namespace MyLib
{
    public class MyFileSystem
    {
        public static void ClearOldFile(string folder, string mask = "*.*", int dayCount = 0, LogFile log = null)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder, mask);
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo info = new FileInfo(files[i]);
                        if (info.CreationTime < DateTime.Now.AddDays(-dayCount))
                            DeleteFile(files[i], log);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void DeleteFolder(string folder, LogFile log = null)
        {
            try
            {
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder);
                    foreach (string file in files)
                    {
                        DeleteFile(file);
                    }

                    string[] subDirectories = Directory.GetDirectories(folder);
                    foreach (string subDirectory in subDirectories)
                    {
                        DeleteFolder(subDirectory);
                    }

                    Directory.Delete(folder);
                    log?.ToLog($"Видалено каталог: {folder}");
                }
            }
            catch (Exception ex)
            {
                log?.ToLog($"Помилка при видаленні каталогу {folder}: {ex.Message}");
            }
        }
        public static void DeleteFile(string file, LogFile log = null)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                    log?.ToLog($"Видалено файл: {file}");
                }
            }
            catch (Exception ex)
            {
                log?.ToLog($"Помилка при видаленні файлу {file}: {ex.Message}");
            }
        }
    }
}
