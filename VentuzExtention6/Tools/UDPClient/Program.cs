using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Unity;

namespace UDPClientTool
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

            if (string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }

            int port = 3131;
            if (!int.TryParse(sPort, out port))
            {
                port = 3131;
            }

            UdpClient client = new UdpClient(ip, port);
            string cmd = "";
            while (cmd != "exit")
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    byte[] data = Encoding.UTF8.GetBytes(cmd);
                    client.Send(data, data.Length);
                }

                Console.Write(">>> ");
                cmd = Console.ReadLine();
            }
        }
    }
}
