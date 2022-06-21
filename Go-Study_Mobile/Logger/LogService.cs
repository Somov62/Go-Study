using Newtonsoft.Json;
using System;
using System.IO;

namespace Logger
{
    public partial class LogService
    {
        private readonly string _filename;
        private readonly IDeviceLogService _deviceService;
        public LogService(IDeviceLogService deviceService)
        {
            _deviceService = deviceService;
            ConfigurateLogsFolder();
            _filename = string.Format("Log {0}.txt", DateTime.Now.ToString());
            _service = this;
        }

        private string LocalPath => @"GoStudy/App";
        private string PathToFile => Path.Combine(PathToFolder, _filename);
        private string PathToFolder => Path.Combine(_deviceService.PathToLogFoldFolder, LocalPath, "Logs");

        public void WriteToLog(string action, bool success, Exception exception)
        {
            LogItem item = new LogItem()
            {
                Time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(),
                Action = action,
                Success = success,
            };

            if (exception != null)
            {
                item.Exception = new ExceptionModel()
                {
                    Message = exception.Message,
                    InnerException = exception.InnerException?.Message,
                    Source = exception.Source,
                    StackTrace = exception.StackTrace,
                    TargetSite = exception.TargetSite.Name
                };
            }

            string json = JsonConvert.SerializeObject(item) + ",";

            using (StreamWriter writer = new StreamWriter(PathToFile))
            {
                writer.WriteLine(json);
            }
        }

        private void ConfigurateLogsFolder()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(PathToFolder);
            if (!directoryInfo.Exists)
            {
                Directory.CreateDirectory(PathToFolder);
            }
        }
    }
}
