using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Unity;

namespace DebugClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
            });

            Console.Write("Server IP(Default 127.0.0.1): ");
            string ip = Console.ReadLine();
            Console.Write("Server Port(Default 3131): ");
            string sPort = Console.ReadLine();

            if(string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }

            int port = 3131;
            if(!int.TryParse(sPort, out port))
            {
                port = 3131;
            }

            client.ConnectStatus = ConnectResult;
            client.Connect(ip, port);

            string cmd = "";
            while(cmd != "exit")
            {
                if(!string.IsNullOrEmpty(cmd))
                {
                    client.SendCommand(cmd);
                }

                Console.Write(">>> ");
                cmd = Console.ReadLine();
            }
        }

        static void ConnectResult(bool success)
        {
            if(success)
            {
                Logger.Info("Server Connected.");
            }
            else
            {
                Logger.Error("Server Connect Failed.");
                Delay.Run(3000, client.ReConnect);
            }
        }

        static private PageControlClient client = new PageControlClient();
    }
}
