using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareLib.Log
{
    public class LockedLoggerGroup : LoggerGroup
    {
        public override void LogMessage(LogLevel level, string msg)
        {
            lock(_lockobj)
            {
                base.LogMessage(level, msg);
            }
        }

        private object _lockobj = new object();
    }
}
