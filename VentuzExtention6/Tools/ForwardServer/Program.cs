using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Net;
using ShareLib.Log;

namespace ForwardServer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RealMain();
        }

        private void RealMain()
        {
            _server.SetReciverListener((string cmd, string peername) =>
            {
                Logger.Info($"收到客户端 {peername} 命令: {cmd}");
                _server.SendCommandToOther(cmd, peername);
            });

            _server.Start(1012);

            while(Console.ReadLine() != "exit")
            {

            }
        }

        private CmdTcpServer _server = new CmdTcpServer();
    }
}
