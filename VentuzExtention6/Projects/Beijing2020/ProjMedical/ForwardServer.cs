using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.everythinghouse.movingtv;
using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.PadUI;
using ShareLib.Unity;

namespace ProjMedical
{
    class ForwardServer
    {
        private PageControlServer _server = new PageControlServer();
        private LLServerManager _lowersrv = new LLServerManager();
        private LLClientManager _lowercli = new LLClientManager();

        public void Main()
        {
            _lowersrv.Init(_server, null);            

            string pageDir = GlobalConf.getconf_aspath("forward", "pagedir");
            string mainPage = GlobalConf.getconf("forward", "mainmode", "index");
            int port = GlobalConf.getconf<int>("forward", "port", 3131);

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

            //业务相关功能, 有时间后改成配置文件

            _server.Status.Reg(new CheckGroup(new string[] { "@left", "go" }, 
                new string[] { "home", "net", "jj", "hz","hz_cs","hz_yx","hz_hz","ss","ss_sj","ss_yc","icu","icu_hz","icu_jqr","icu_vr","icu_yh" }, "home"));

            _server.Status.Reg(new CheckGroup(new string[] { "@right", "go" },
                new string[] { "home", "net", "jj", "hz", "hz_cs", "hz_yx", "hz_hz", "ss", "ss_sj", "ss_yc", "icu", "icu_hz", "icu_jqr", "icu_vr", "icu_yh" }, "home"));

            //-----
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
                    SendCommand(cmdline);
                }
            }
        }


        public void SetReciverListener(Action<string> reciver) => _server.SetReciverListener(reciver);

        private void OnForwardsCommand(string cmd, string clientname)
        {
            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#shutdown":
                    Logger.Warning($"!开始关机");
                    BeginShutdown();
                    break;
                case "#poweron":
                    
                    break;
                case "#checkpower":
                    
                    break;
                case "#reg":

                    break;
                default:
                    SendCommand(cmd);
                    //if (!SendForwardCommand(command))
                    //{
                    //    Logger.Error($"未知服务控制命令: { command.cmd }");
                    //}
                    break;
            }
        }

        public void BeginShutdown()
        {
            lastChangePowerStatus = Environment.TickCount;
            Logger.Warning($"!开始关机");
            bool noclient = true;
            try
            {
                if (!_lowersrv.IsEmpty())
                {
                    _lowersrv.RunShutdown();
                    noclient = false;
                }

                if (!_lowercli.IsEmpty())
                {
                    _lowercli.RunShutdown();
                    noclient = false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }

            if (!noclient)
            {
                Logger.Warning($"等待展项服务器关机...");
                Delay.Run(2000, BeginShutdown);
            }
            else
            {
                Logger.Warning($"关机完毕, 2分钟后断电...");
            }
        }


        private void ShutDownConfirm()
        {
            _server.SwitchPage("gj");
            //BeginShutdown();
        }

        private bool SendForwardCommand(CmdLine command)
        {
            string client = command.cmd.Substring(1);
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
                SendCommand(cmd);
            }
        }

        private void SendCommand(string cmd)
        {
            foreach (var c in _lowercli._clients)
            {
                c.Value.SendCommand(cmd);
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

        private int lastChangePowerStatus = 0;
  
    }
}
