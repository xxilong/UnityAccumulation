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

            QXSTSerialPort.Instance._procol.SetPowerStatusListener(CheckPowerStatus);

            string pageDir = GlobalConf.getconf_aspath("forward", "pagedir");
            string mainPage = GlobalConf.getconf("forward", "mainmode", "index");
            int port = GlobalConf.getconf<int>("forward", "port", 3131);

            foreach(string key in GlobalConf.DomainKeys("servers"))
            {
                string line = GlobalConf.getconf<string>("servers", key);
                string[] ipport = line.Split(':');
                if(ipport.Length != 2)
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
            powerCheck = new ToggerCheck("togger", "power", null, false, null, OnPowerStatusChange);
            _server.Status.Reg(powerCheck);
            lightCheck = new ToggerCheck("togger", "light", OnLightStatus);
            _server.Status.Reg(lightCheck);

            //"sjdp"-数据大屏,"dpjr"-大屏金融, "dpzf"-大屏支付, "dpds"-大屏电商, "dpjcnl"-大屏基础能力,"qyxc"-大屏企业宣传,
            //"jscx"-技术创新,"jr"-金融,"zf"-支付,"ds"-电商,
            _server.Status.Reg(new CheckGroup("$GP", 
                new string[] { "sjdp","dpjr", "dpzf", "dpds", "dpjcnl","qyxc","jscx","jr","zf","ds", "index" },
                new string[] { "sjdp", "dpjr", "dpzf", "dpds", "dpjcnl", "qyxc", "jscx", "jr", "zf", "ds", "" }));

            _server.Status.Reg(new CheckGroup(new string[] { "@dp", "go" }, new string[] { "dp_jr", "dp_zf","dp_ds", "dp_jcnl"},
                new string[] { "dp_xyg", "dp_hjb", "dp_hmj", "dp_xyk",
                "dp_yhyh","dp_xssh","dp_xxsh","dp_fqdsh","dp_md","dp_jhzf","dp_hfcz","dp_cszf","dpcs_qy","dpcs_sh",

                "dp_jhzf_hebei","dp_jhzf_shanxi","dp_jhzf_ln","dp_jhzf_jl","dp_jhzf_hlj","dp_jhzf_js","dp_jhzf_zj","dp_jhzf_ah",
                "dp_jhzf_fj","dp_jhzf_jx","dp_jhzf_sd","dp_jhzf_henan","dp_jhzf_hubei","dp_jhzf_hunan","dp_jhzf_gd","dp_jhzf_hainan",
                "dp_jhzf_sc","dp_jhzf_gz","dp_jhzf_yn","dp_jhzf_sx","dp_jhzf_gs","dp_jhzf_qh","dp_jhzf_tw","dp_jhzf_nmg","dp_jhzf_gx",
                "dp_jhzf_xz","dp_jhzf_nx","dp_jhzf_xj","dp_jhzf_bj","dp_jhzf_tj","dp_jhzf_sh","dp_jhzf_cq","dp_jhzf_xg","dp_jhzf_am",

                "dp_hfcz_hebei","dp_hfcz_shanxi","dp_hfcz_ln","dp_hfcz_jl","dp_hfcz_hlj","dp_hfcz_js","dp_hfcz_zj","dp_hfcz_ah",
                "dp_hfcz_fj","dp_hfcz_jx","dp_hfcz_sd","dp_hfcz_henan","dp_hfcz_hubei","dp_hfcz_hunan","dp_hfcz_gd","dp_hfcz_hainan",
                "dp_hfcz_sc","dp_hfcz_gz","dp_hfcz_yn","dp_hfcz_sx","dp_hfcz_gs","dp_hfcz_qh","dp_hfcz_tw","dp_hfcz_nmg","dp_hfcz_gx",
                "dp_hfcz_xz","dp_hfcz_nx","dp_hfcz_xj","dp_hfcz_bj","dp_hfcz_tj","dp_hfcz_sh","dp_hfcz_cq","dp_hfcz_xg","dp_hfcz_am" }, "da_m1"));

            _server.Status.Reg(new CheckGroup(new string[] { "@js", "go" }, new string[] { "ppt", "video" }, "ppt"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jr", "go" }, new string[] { "ppt", "video" }, "ppt"));
            _server.Status.Reg(new CheckGroup(new string[] { "@zf", "go" }, new string[] { "ppt", "video" }, "ppt"));
            _server.Status.Reg(new CheckGroup(new string[] { "@ds", "go" }, new string[] { "ppt", "video" }, "ppt"));

            //_server.Status.Reg(new SliderPosKeep(new string[] { "@cxvol", "vol" }, "cxvol"));
            //_server.Status.Reg(new SliderPosKeep(new string[] { "@jrvol", "vol" }, "jrvol"));
            //_server.Status.Reg(new SliderPosKeep(new string[] { "@zfvol", "vol" }, "zfvol"));
            //_server.Status.Reg(new SliderPosKeep(new string[] { "@dsvol", "vol" }, "dsvol"));

            //-----

            _server.SetSrvCtrlListener(OnForwardsCommand);
            _server.SetReciverListener(OnReciveCommand);
            _server.SetClientCloseListener(HandleClientClose);

            _server.Start(port, pageDir, mainPage);
            

            CheckPower();

            while (true)
            {
                string cmdline = Console.ReadLine();
                if(string.IsNullOrEmpty(cmdline))
                {
                    continue;
                }

                if(cmdline[0] == '.')
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

        /// <summary>
        /// 发送查询电源状态命令
        /// </summary>
        private void CheckPower()
        {
            Logger.Info($"查询电源状态");
            QXSTSerialPort.Instance.SendRawData("10 04 FF");
        }

        /// <summary>
        /// 获取电源状态并记录到内存
        /// </summary>
        /// <param name="check"></param>
        private void CheckPowerStatus(int check)
        {
            Logger.Info(check.ToString("X"));
            bool noclient = _lowersrv.IsEmpty() && _lowercli.IsEmpty();                
            powerCheck.SetCheck(check > 0&&!noclient);

            lightCheck.SetCheck((check & 0X2000) > 0);
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
                    Logger.Warning($"!开始开机");
                    BeginPowerOn();
                    break;
                case "#checkpower":
                    CheckPower();
                    break;
                case "#reg":
                    if (command.getarg<string>(0, out string name) && command.getarg<int>(1, out int port))
                    {
                        int order = 100;
                        command.getarg<int>(2, out order);
                        _lowersrv.AddSubServer(clientname, name, port, order);
                    }
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

            if(!noclient)
            {
                Logger.Warning($"等待展项服务器关机...");
                Delay.Run(2000, BeginShutdown);
            }
            else
            {
                Logger.Warning($"关机完毕, 2分钟后断电...");
                Delay.Run(1000 * 60 * 2, BeginCutOffPower);
            }
        }

        

        private void BeginCutOffPower()
        {
            Logger.Info($"发送切断电源指令");
            QXSTSerialPort.Instance.SendRawData("10 03 FF 3E 00 00 00 00 00 00 00");
        }

        public void BeginPowerOn()
        {
            lastChangePowerStatus = Environment.TickCount;
            Logger.Info($"发送打开电源指令");
            QXSTSerialPort.Instance.SendRawData("10 03 FF 3F 00 00 00 3F 00 00 00");

            Delay.Run(1000 * 10, () =>
            {
                Logger.Info($"大屏主机开机指令");
                QXSTSerialPort.Instance.SendRawData("10 03 FF 40 00 00 00 40 00 00 00");
            });

            //if (GlobalConf.getconf<int>("base", "enablecom2", 0) != 0)
            //{
            //    Delay.Run(1000 * 10, () =>
            //    {
            //        Logger.Info($"发送主机开机指令");
            //        QXSTSerialPort.Instance2.SendRawData("10 03 12 FC FF FF FF FF FF FF FF");
            //    });
            //}
        }

        private bool OnPowerStatusChange(bool st, Action<string> sender)
        {
            int nowtime = Environment.TickCount;
            if (Math.Abs(nowtime - lastChangePowerStatus) < 1000 * 60*6)
            //    if (Math.Abs(nowtime - lastChangePowerStatus) < 1000 * 60 * 10)
            {
                _server.SwitchPage("warning");
                Logger.Warning($"开关机切换过于频繁, 已忽略");
                return false;
            }
            //lastChangePowerStatus = nowtime;

            if(st)
            {
                Logger.Warning($"!开始开机");
                BeginPowerOn();
                _server.SwitchPage("kj");
                
            }
            else
            {
                BeginShutdown();
                ShutDownConfirm();                               
            }
            return true;
        }
                

        private void ShutDownConfirm()
        {
            _server.SwitchPage("gj");
            //BeginShutdown();
        }

        private void OnLightStatus(bool st, Action<string> sender)
        {
            Logger.Info($"影院灯 {st}");
            if (st)
            {
                Logger.Info($"发送灯光开指令");
                QXSTSerialPort.Instance.SendRawData("10 03 FF 01 00 00 00 01 00 00 00");                
            }
            else
            {
                Logger.Info($"发送灯光关指令");
                QXSTSerialPort.Instance.SendRawData("10 03 FF 01 00 00 00 00 00 00 00");
            }           
        }

        private bool SendForwardCommand(CmdLine command)
        {
            string client = command.cmd.Substring(1);
            ClientItem? item = _lowersrv.FindClient(client);
            if (item != null)
            {
                Logger.Debug($"转发 {item.Value.peerName}:{command.argsline}");
                _server.SendCommandTo(command.argsline, item.Value.peerName);
                return true;
            }
            
            PageControlClient llc = _lowercli.FindClient(client);
            if (llc != null)
            {
                Logger.Debug($"转发 {client}:{command.argsline}");
                llc.SendCommand(command.argsline);
                return true;
            }            

            return false;
        }

        private void SendCommand(string cmd)
        {
            foreach (var c in _lowercli._clients)
            {
                c.Value.SendCommand(cmd);
            }
        }

        private void OnReciveCommand(string cmd)
        {
            CmdLine command = new CmdLine(cmd);


            if (command.cmd[0] == '@')
            {
                if(SendForwardCommand(command))
                {
                    return;
                }
            }

            SendForwardCommand(command);
        }

        private void HandleClientClose(string clientname)
        {
            _lowersrv.RemoveSubServer(clientname);
        }

        private void HanderInnerCommand(string cmd)
        {
            switch(cmd)
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
        private ToggerCheck powerCheck,lightCheck= null;
    }
}
