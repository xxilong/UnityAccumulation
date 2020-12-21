using System;
using System.Collections;
using System.Collections.Generic;

namespace ShareLib.Log
{
    public class LoggerGroup : BaseLogger, IEnumerable<BaseLogger>
    {
        public LoggerGroup Add(BaseLogger logger)
        {
            _loggerList.Add(logger);
            return this;
        }

        public override void LogMessage(LogLevel level, string msg)
        {
            foreach(BaseLogger logger in _loggerList)
            {
                logger.LogMessage(level, msg);
            }
        }

        public IEnumerator<BaseLogger> GetEnumerator()
        {
            foreach (BaseLogger logger in _loggerList)
            {
                yield return logger;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (BaseLogger logger in _loggerList)
            {
                yield return logger;
            }
        }

        private List<BaseLogger> _loggerList = new List<BaseLogger>();
    }
}
