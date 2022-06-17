using Newtonsoft.Json;
using System;
using System.IO;

namespace Logger
{
    public partial class LogService
    {
        private readonly string _filename;
        public LogService()
        {
            ConfigurateLogsFolder();
            _filename = string.Format("Log {0}.txt", DateTime.Now.ToString());
        }
        private string PathToFile => Path.Combine(Environment.CurrentDirectory, "Logs", _filename);

        public void WriteToLog(string action, bool success, Exception exception)
        {
            LogItem item = new LogItem()
            {
                Time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                Action = action,
                Success = success,
                Exception = new ExceptionModel()
                {
                    Message = exception?.Message,
                    InnerException = exception?.InnerException?.Message,
                    Source = exception?.Source,
                    StackTrace = exception?.StackTrace,
                    TargetSite = exception?.TargetSite.Name
                }
            };
            string json = JsonConvert.SerializeObject(item) + ",";

            using (StreamWriter writer = new StreamWriter(PathToFile))
            {
                writer.WriteLine(json);
            }
        }

        private void ConfigurateLogsFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Logs"));
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Logs"));
            }
        }
    }
}
