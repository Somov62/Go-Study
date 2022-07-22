using LoggerLib.LogModels.Base;
using System;

namespace LoggerLib
{
    public class Logger
    {
        private static Logger _context;
        private int _idCounter = 1;
        private readonly LoggerDirectory _directory;

        public Logger(string pathToProjectFolder) 
        {
            _directory = new LoggerDirectory(pathToProjectFolder);
            _context = this;
        }
        
        public LoggerDirectory Directory => _directory;

        public static Logger GetContext()
        {
            return _context;
        }

        public void CreateLog(BaseLogModel logModel)
        {
            if (_directory.TodayLogExists()) _idCounter = 1;
            logModel.DateTime = DateTime.Now.ToString("F");
            if (logModel.Type == default) logModel.Type = LogType.Trace;
            logModel.ModelName = logModel.GetType().Name;
            logModel.Id = _idCounter;
            _idCounter++;
            SaveLog(logModel);
        }

        private void SaveLog(BaseLogModel logModel)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(logModel) + "|";
            Directory.AddLog(json);
        }
    }

    public enum LogType
    {
        Trace,
        Info,
        Warning,
        Error
    }
}
