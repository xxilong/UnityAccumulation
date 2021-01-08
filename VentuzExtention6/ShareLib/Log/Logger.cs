using System;

namespace ShareLib.Log
{
    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
    }

    public class BaseLogger
    {
        public void Debug(string msg)
        {
            if (LogLevel.DEBUG >= Level)
            {
                LogMessage(LogLevel.DEBUG, msg);
            }
        }

        public void Info(string msg)
        {
            if (LogLevel.INFO >= Level)
            {
                LogMessage(LogLevel.INFO, msg);
            }
        }

        public void Warning(string msg)
        {
            if (LogLevel.WARNING >= Level)
            {
                LogMessage(LogLevel.WARNING, msg);
            }
        }

        public void Error(string msg)
        {
            if (LogLevel.ERROR >= Level)
            {
                LogMessage(LogLevel.ERROR, msg);
            }
        }

        public virtual void LogMessage(LogLevel level, string msg)
        {
        }

        protected string LevelToString(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.DEBUG:
                    return "[调试]";

                case LogLevel.INFO:
                    return "[消息]";

                case LogLevel.WARNING:
                    return "[警告]";

                case LogLevel.ERROR:
                    return "[错误]";

                default:
                    return "[未知]";
            }
        }

        public LogLevel Level { get; set; } = LogLevel.DEBUG;
    }

    public class Logger
    {
        public static void Set(BaseLogger logger)
        {
            _instance = logger;
        }

        public static void Debug(string msg)
        {
            _instance.Debug(msg);
        }

        public static void Info(string msg)
        {
            _instance.Info(msg);
        }

        public static void Warning(string msg)
        {
            _instance.Warning(msg);
        }

        public static void Error(string msg)
        {
            _instance.Error(msg);
        }

        public static BaseLogger Instance
        {
            get
            {
                return _instance;
            }
        }

        private static BaseLogger _instance = new LockedLoggerGroup { new ConsoleLogger(), new FileLogger() };
    }
}
