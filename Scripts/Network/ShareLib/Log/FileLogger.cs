using System;
using System.IO;

namespace ShareLib.Log
{
    public class FileLogger : BaseLogger
    {
        public FileLogger()
        {
            _logFileName = null;
        }

        public FileLogger(string filename)
        {
            _logFileName = filename;
        }

        public override void LogMessage(LogLevel level, string msg)
        {
            string logFileName = _logFileName ?? AppDomain.CurrentDomain.BaseDirectory + "Log\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

            try
            {
                using (StreamWriter writer = File.AppendText(logFileName))
                {
                    writer.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " " + LevelToString(level) + " " + msg);
                }
            }
            catch (Exception e)
            {
                string excepInfo = e.Message;
            }
        }

        protected string _logFileName;
    }
}
