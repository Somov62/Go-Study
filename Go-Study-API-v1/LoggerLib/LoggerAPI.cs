using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LoggerLib
{
    public partial class LoggerDirectory
    {
        public string GetLog(string logname)
        {
            string pathToFile = Path.Combine(_pathToFolder, logname);
            if (!File.Exists(pathToFile)) return null;
            return Encoding.UTF8.GetString(File.ReadAllBytes(pathToFile));
        }

        public List<string> GetLogsDateList()
        {
            DirectoryInfo directory = new DirectoryInfo(_pathToFolder);
            directory.GetFiles().Select(p => p.Name).ToList();

            return directory.GetFiles().Select(p => p.Name).ToList();
        }

        public List<(string FileName, string Content)> GetAllLogs()
        {
            List<(string, string)> logsList = new List<(string, string)>();
            DirectoryInfo directory = new DirectoryInfo(_pathToFolder);
            foreach (var item in directory.GetFiles())
            {
                logsList.Add((item.Name, GetLog(item.Name)));
            }
            return logsList;
        }

        public void ClearAll()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(_pathToFolder);
            foreach (var item in directoryInfo.GetFiles())
            {
                item.Delete();
            }
        }
    }
}
