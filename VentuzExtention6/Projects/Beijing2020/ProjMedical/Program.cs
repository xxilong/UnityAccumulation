using com.everythinghouse.movingtv;
using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjMedical
{
    class Program
    {
        static void Main(string[] args)
        {

            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
                new UDPBroadCastLog(),
            });

            string workmode = GlobalConf.getconf("base", "workmode");
             if (workmode == "forward")
            {
                try
                {
                    ForwardServer srv = new ForwardServer();
                    srv.Main();
                }
                catch (Exception e)
                {
                    Logger.Error($"!!Exception: {e}");
                }
            }
        }
        
    }
}
