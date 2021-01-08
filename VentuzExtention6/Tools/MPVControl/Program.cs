using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShareLib.Conf;
using ShareLib.Unity;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using WO;
using ShareLib.Log;

namespace MPVControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger("ctrl.log")
            });

            GlobalConf.SetGlobalFileGetter(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));
            string livePath = GlobalConf.getconf<string>("video", "live");
            int livetime = GlobalConf.getconf<int>("live", "time", 60);

            rpc.Start(livePath);
            int tryed = 0;
            for(int i = 0; i < livetime * 2; ++i)
            {
                Thread.Sleep(500);
                if(rpc.IsLiveFailed())
                {
                    ++tryed;
                    if(tryed > 5)
                    {
                        Logger.Warning($"open live failed {tryed}, give up");
                        break;
                    }

                    Logger.Warning($"open live failed {tryed}");
                    rpc.Start(livePath);
                    i = 0;
                }
            }


            //while (true)
            //{
            //    string line = Console.ReadLine();
            //    rpc.Request(line);
            //}            

            while (true)
            {
                int i = 1;
                while (true)
                {
                    string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "videos", $"{i}.mp4");
                    if (!File.Exists(file))
                    {
                        break;
                    }

                    file = file.Replace("\\", "/");
                    rpc.Play("file://" + file);
                    rpc.Request("set_property sub-visibility false");
                    ++i;

                    Thread.Sleep(2000);

                    while (!rpc.HasEnd())
                    {
                        Thread.Sleep(50);
                    }

                    rpc.Request("set_property sub-visibility true");
                    rpc.Play(livePath);
                    for (int j = 0; j < livetime * 2; ++j)
                    {
                        Thread.Sleep(500);
                        if (rpc.IsLiveFailed())
                        {
                            Logger.Error($"open live failed, skip.");
                            break;
                        }
                    }
                }
            }
        }

        private static MpvRpc rpc = new MpvRpc();
    }
}
