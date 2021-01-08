using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.PadUI;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;

namespace PadControlCenter
{
    class ForwardServer
    {
        private PageControlServer _server = new PageControlServer();
        private LLServerManager _lowersrv = new LLServerManager();
        private LLClientManager _lowercli = new LLClientManager();

        public void Main()
        {
            _lowersrv.Init(_server, null);

            string pageDir = GlobalConf.getconf_aspath("fwd", "pagedir");
            string mainPage = GlobalConf.getconf("fwd", "mainmode", "index");
            int port = GlobalConf.getconf<int>("fwd", "port", 3131);

            foreach (string key in GlobalConf.DomainKeys("servers"))
            {
                string line = GlobalConf.getconf<string>("servers", key);
                string[] ipport = line.Split(':');
                if (ipport.Length != 2)
                {
                    Logger.Error($"配置错误 [servers]->{key} 格式应该为 ip:port");
                    continue;
                }

                string ip = ipport[0];
                int srport = 3131;
                int.TryParse(ipport[1], out srport);
                _lowercli.StartConnect(key, ip, srport);
            }

            _server.SetSrvCtrlListener(OnForwardsCommand);
            _server.SetReciverListener(OnReciveCommand);
            _server.SetClientCloseListener(HandleClientClose);
            _server.Start(port, pageDir, mainPage);

            while (true)
            {
                string cmdline = Console.ReadLine();
                if (string.IsNullOrEmpty(cmdline))
                {
                    continue;
                }

                if (cmdline[0] == '.')
                {
                    cmdline = cmdline.Substring(1);
                    HanderInnerCommand(cmdline);
                }
                else
                {
                    _server.SendCommand(cmdline);
                }
            }
        }

        public void SetReciverListener(Action<string> reciver) => _server.SetReciverListener(reciver);

        private void OnForwardsCommand(string cmd, string clientname)
        {
            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#reg":
                    if (command.getarg<string>(0, out string name) && command.getarg<int>(1, out int port))
                    {
                        int order = 100;
                        command.getarg<int>(2, out order);
                        _lowersrv.AddSubServer(clientname, name, port, order);
                    }
                    break;
                default:
                    if (!SendForwardCommand(command))
                    {
                        Logger.Error($"未知服务控制命令: { command.cmd }");
                    }
                    break;
            }
        }

        private void SendCommandToAll(string command)
        {
            _server.SendCommand(command);
            _lowercli.SendCommandToAll(command);
        }

        private bool SendForwardCommand(CmdLine command)
        {
            string client = command.cmd.Substring(1);
            if(client == "*")
            {
                Logger.Debug($"转发所有客户端: {command.argsline}");
                SendCommandToAll(command.argsline);
                return true;
            }

            ClientItem? item = _lowersrv.FindClient(client);
            if (item != null)
            {
                Logger.Debug($"转发 {item.Value.peerName}: {command.argsline}");
                _server.SendCommandTo(command.argsline, item.Value.peerName);
                return true;
            }

            PageControlClient llc = _lowercli.FindClient(client);
            if (llc != null)
            {
                Logger.Debug($"转发 {client}: {command.argsline}");
                llc.SendCommand(command.argsline);
                return true;
            }

            return false;
        }

        private void OnReciveCommand(string cmd)
        {
            CmdLine command = new CmdLine(cmd);
            if (command.cmd[0] == '@')
            {
                if (SendForwardCommand(command))
                {
                    return;
                }
            }
            else
            {
                Logger.Debug($"转发所有客户端: {cmd}");
                SendCommandToAll(cmd);
                return;
            }
        }

        private void HandleClientClose(string clientname)
        {
            _lowersrv.RemoveSubServer(clientname);
        }

        private void HanderInnerCommand(string cmd)
        {
            switch (cmd)
            {
                case "online":
                    _lowersrv.DumpList();
                    _lowercli.DumpList();
                    break;

                default:
                    break;
            }
        }
    }
}
