using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Net;
using ShareLib.Ayz;
using ShareLib.Log;

namespace VolumeControlServ
{
    class Program
    {
        static void Main(string[] args)
        {
            _server.SetReciverListener(OnRecvCommand);
            _server.Start(3166);  
            while(true)
            {
                Console.ReadLine();
            }
        }

        static void OnRecvCommand(string cmd, string peer)
        {
            CmdLine command = new CmdLine(cmd);
            if(command.cmd != "vol")
            {
                Logger.Warning($"unknown command {command.cmd}");
                return;
            }

            double val = 10.0;
            if(!command.getarg<double>(0, out val))
            {
                Logger.Warning($"vol argument should be double with 0-100");
                return;
            }

            VolumeControl.Instance.MasterVolume = val;
            Logger.Info($"volume has been set to {VolumeControl.Instance.MasterVolume}/{val}");
        }

        static private CmdTcpServer _server = new CmdTcpServer();
    }
}
