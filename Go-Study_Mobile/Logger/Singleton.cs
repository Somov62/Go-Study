using System;

namespace Logger
{
    public partial class LogService
    {
        private static LogService _service;

        public static LogService GetService()
        {
            //if (_service == null)
            //{
            //    _service = new LogService();
            //}
            return _service;
        }
    }
}
