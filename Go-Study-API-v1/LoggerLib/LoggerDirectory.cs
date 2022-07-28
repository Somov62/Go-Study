using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLib
{
    public partial class LoggerDirectory
    {
        private readonly string _pathToFolder;

        public LoggerDirectory(string pathToProjectFolder)
        {
            _pathToFolder = Path.Combine(pathToProjectFolder, "LogsStorage");

            DirectoryInfo directoryInfo = new DirectoryInfo(_pathToFolder);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(_pathToFolder);
            }
        }

        internal void AddLog(string text)
        {
            string pathToFile = Path.Combine(_pathToFolder, DateTime.Now.ToString("d") + ".txt");

            Task.Run(() => WriteTextAsync(pathToFile, text));
        }

        internal bool TodayLogExists()
        {
            string pathToFile = Path.Combine(_pathToFolder, DateTime.Now.ToString("d") + ".txt");
            return File.Exists(pathToFile);
        }

        private async Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.UTF8.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
    }
}
