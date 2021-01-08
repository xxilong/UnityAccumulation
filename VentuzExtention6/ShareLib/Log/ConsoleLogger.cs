using System;

namespace ShareLib.Log
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(string prfex = "")
        {
            _prfex = prfex;
        }

        public override void LogMessage(LogLevel level, string msg)
        {           
            LogWithLevelColor(level, _prfex + DateTime.Now.ToString("HH:mm:ss.fff") + " " + LevelToString(level) + " " + msg);
        }

        public void LogWithLevelColor(LogLevel level, string msg)
        {
            switch (level)
            {
                case LogLevel.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.INFO:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                case LogLevel.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case LogLevel.WARNING:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private string _prfex = "";
    }
}
