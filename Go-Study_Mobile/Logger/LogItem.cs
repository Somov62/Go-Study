using System;

namespace Logger
{
    internal struct LogItem
    {
        public string Action{ get; set; }
        public bool Success { get; set; }
        public long Time { get; set; } //Unix dateTime format
        public ExceptionModel Exception { get; set; }

    }

    internal struct ExceptionModel
    {
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string Source { get; set; }
        public string TargetSite { get; set; }
        public string StackTrace { get; set; }
    }

}
