namespace LoggerLib.LogModels.Base
{
    public class BaseLogModel
    {
        public string DateTime { get; set; }

        public string ModelName { get; set; }

        public int Id { get; set; }

        public string Message { get; set; }

        public LogType Type { get; set; }
        public string CurrentMethod { get; set; }
    }
}
