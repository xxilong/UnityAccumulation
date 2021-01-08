using ShareLib.Ayz;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RealMain();
        }

        private void RealMain()
        {
            //QXSTSerialPort.Instance.Open();

            _server.SetReciverListener((string line, string peername) =>
            {
                Logger.Info($"收到客户端 {peername} 命令: {line}");
                CmdLine cmd = new CmdLine(line);
                string arg;

                if(cmd.cmd == "engine")
                {
                    if(!cmd.getarg<string>(0, out arg))
                    {
                        return;
                    }

                    if(arg != "status")
                    {
                        return;
                    }

                    if(_IsEngineError)
                    {
                        _server.SendCommand("engine bad");
                    }
                    else
                    {
                        _server.SendCommand("engine good");
                    }
                }
            });

            _server.Start(3131);

            Console.Write(">>> ");
            string cmdline = Console.ReadLine();
            while (cmdline != "exit")
            {
                if(cmdline == "good")
                {
                    _IsEngineError = false;
                    _server.SendCommand("engine good");
                }
                else if(cmdline == "bad")
                {
                    _IsEngineError = true;
                    _server.SendCommand("engine bad");
                }
                else if(cmdline != "")
                {
                    Console.WriteLine("unknown command.");
                }

                Console.Write(">>> ");
                cmdline = Console.ReadLine();
            }
        }

        private CmdTcpServer _server = new CmdTcpServer();
        private bool _IsEngineError = true;
    }
}
