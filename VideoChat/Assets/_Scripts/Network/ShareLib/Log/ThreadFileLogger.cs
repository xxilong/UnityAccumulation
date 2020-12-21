using System;
using System.Diagnostics;
using System.Threading;

namespace ShareLib.Log
{
    public class ThreadFileLogger : FileLogger
    {
        public ThreadFileLogger()
        {
        }

        public override void LogMessage(LogLevel level, string msg)
        {
            _logFileName = GetLogFileName();
            base.LogMessage(level, msg);
        }

        static private string GetLogFileName()
        {
            return AppDomain.CurrentDomain.BaseDirectory+"Log\\"
                +DateTime.Now.ToString("yyyy-MM-dd")+
                "."+Process.GetCurrentProcess().Id +
                "."+Thread.CurrentThread.ManagedThreadId +".log";
        }
    }
}
