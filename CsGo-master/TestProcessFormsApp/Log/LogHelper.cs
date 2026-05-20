using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JLRScan.Log
{
    internal class LogHelper
    {
        static readonly ILog _loggerInfo = LogManager.GetLogger("LogTraceInfo");
        public static void Info(string message)
        {
            _loggerInfo.Info(message);                               //打印事件
        }

        public static void Debug(string message)
        {
            _loggerInfo.Debug(message);                             //调试
        }

        public static void Warn(string message)
        {
            _loggerInfo.Warn(message);                              //警告
        }

        public static void Error(string message)
        {
            _loggerInfo.Error(message);                             //错误
        }
    }
}
