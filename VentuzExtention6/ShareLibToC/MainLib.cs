using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Conf;

using System.IO;

namespace ShareLibToC
{
    public class MainLib
    {
        static public int Initalize(string arg)
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
                new UDPBroadCastLog(),
            });

            GlobalConf.SetGlobalFileGetter(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
            return 0;
        }        
    }
}
