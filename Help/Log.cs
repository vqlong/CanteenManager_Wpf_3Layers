using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Help
{
    public static class Log
    {
        private static readonly ILog _log  = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static void Info(string format, params object[] args) => _log.InfoFormat(format, args);
        public static void Error(string format, params object[] args) => _log.ErrorFormat(format, args);
    }
}
