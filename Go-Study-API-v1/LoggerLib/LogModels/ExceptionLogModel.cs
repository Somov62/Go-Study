using LoggerLib.LogModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLib.LogModels
{
    public class ExceptionLogModel : BaseLogModel
    {
        public Exception Exception { get; set; }
    }
}
