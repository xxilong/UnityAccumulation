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
    class ForwardServer_ChengYan
    {        
        private PageControlServer _server = new PageControlServer();
        private LLServerManager _lowersrv = new LLServerManager();
        private LLClientManager _lowercli = new LLClientManager();

        private SimpleUDPClient _sliderCtrl,_yyCtrl= null;

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

            // 滑轨屏! 又是业务相关的功能!
            _sliderCtrl = new SimpleUDPClient(GlobalConf.getconf("slider_screen", "ip", "192.168.1.30"), GlobalConf.getconf<int>("slider_screen", "port", 56789));
            _yyCtrl = new SimpleUDPClient(GlobalConf.getconf("yy", "ip", "192.168.1.9"), GlobalConf.getconf<int>("yy", "port", 5050));

            //业务相关功能, 有时间后改成配置文件
            powerCheck = new ToggerCheck("togger", "power", null, false, null, OnPowerStatusChange);
            _server.Status.Reg(powerCheck);
            ltyyCheck = new ToggerCheck("togger", "ltyy", OnLightStatusYY);
            ltjyCheck = new ToggerCheck("togger", "ltjy", OnLightStatusJY);
            ltwrjCheck = new ToggerCheck("togger", "ltwrj", OnLightStatusWRJ);
            _server.Status.Reg(new ToggerGroup("togger", "light", new ToggerCheck[]{
                ltyyCheck,
                ltjyCheck,
                ltwrjCheck
            }));
            _server.Status.Reg(new CheckGroup("$GP", 
                new string[] {"xt", "lc", "hx", "yy", "jy", "jj", "yl", "hs", "wrj", "ny", "index" },
                new string[] { "m0xt", "m1lc", "m2hx", "m3yy", "m4jy", "m5jj", "m6yl", "m7hs", "m8wrj", "m9ny", "" }));
            _server.Status.Reg(new CheckGroup(new string[] { "@xt", "go" }, new string[] { "xt_home","xt_year", "xt_intr", "xt_oppt" }, "xt_home"));

            _server.Status.Reg(new CheckGroup(new string[] { "@jytm", "go" }, new string[] { "ls", "bs", "js" }, "ls"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jysj", "go" }, new string[] { "jysj_home", "jysj_web"}, "jysj_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jyzb", "go" }, new string[] { "jyzb_home", "jyzb_video" }, "jyzb_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jyxy", "go" }, new string[] { "jyxy_home", "jyxy_video" }, "jyxy_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jytb", "go" }, new string[] { "jytb_home", "jytb_video" }, "jytb_home"));

            _server.Status.Reg(new CheckGroup(new string[] { "@hxsm", "go" }, new string[] { "hxsm_home", "hxsm_demo", "hxsm_lc" }, "hxsm_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@hxxz", "go" }, new string[] { "hxzw_home", "hxzw_demo", "hxzw_xz", "hxzw_fl", "hxzw_cj", "hxzw_cs" }, "hxzw_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@hxwg", "go" }, new string[] { "hxwg_home", "hxwg_jj", "hxwg_pz", "hxwg_gj", "hxwg_xn" }, "hxwg_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@hx5g", "go" }, new string[] { "@hx5g", "pop" },
                new string[] { "hx5g_home", "hx5g_nr", "hx5g_jy", "hx5g_yl", "hx5g_aq" }, "hx5g_home"));

            _server.Status.Reg(new CheckGroup(new string[] { "@yy", "go" }, new string[] { "yy_m1","yy_zb", "yy_m2", "yy_m3", "yy_m4", "yy_m5" }, "yy_m1"));
            _server.Status.Reg(new CheckGroup(new string[] { "@jj", "go" }, new string[] { "jj_intr", "jj_demo", "jj_1min", "jj_home" }, "jj_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@yl", "go" }, new string[] { "yl_demo", "yl_home" }, "yl_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@hs", "go" }, new string[] { "hs_demo", "hs_pd", "hs_home", "hs_pa", "hs_sz", "hs_ts" }, "hs_home"));

            _server.Status.Reg(new CheckGroup(new string[] { "@nyqx", "go" }, new string[] { "nyqx_home", "nyqx_null" }, "nyqx_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@nyyz", "go" }, new string[] { "nyyz_home", "nyyz_plat" }, "nyyz_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@nysz", "go" }, new string[] { "nysz_home", "nysz_plat" }, "nysz_home"));
            _server.Status.Reg(new CheckGroup(new string[] { "@nyjz", "go" }, new string[] { "nyjz_home", "nyjz_plat", "nyjz_jk" }, "nyjz_home"));

            _server.Status.Reg(new SliderPosKeep(new string[] { "@yyvol", "vol" }, "yyvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@xtvol", "vol" }, "xtvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@lcvol", "vol" }, "lcvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@jjvol", "vol" }, "jjvol"));

            _server.Status.Reg(new SliderPosKeep(new string[] { "@wrjs1vol", "vol" }, "wrjs1vol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@wrjs2vol", "vol" }, "wrjs1vol"));

            _server.Status.Reg(new SliderPosKeep(new string[] { "@jyzbvol", "vol" }, "jyzbvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@jytbvol", "vol" }, "jytbvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@jyxyvol", "vol" }, "jyxyvol"));
            _server.Status.Reg(new SliderPosKeep(new string[] { "@jysjvol", "vol" }, "jysjvol"));
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
                    _server.SendCommand(cmdline);
                }
            }
        }

        /// <summary>
        /// 发送查询电源状态命令
        /// </summary>
        private void CheckPower()
        {
            Logger.Info($"查询电源状态");
            QXSTSerialPort.Instance.SendRawData("10 04 11");
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

            ltwrjCheck.SetCheck((check & 0X800) > 0);
            ltjyCheck.SetCheck((check & 0X1000) > 0);
            ltyyCheck.SetCheck((check & 0X2000) > 0);
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
                    if(!SendForwardCommand(command))
                    {
                        Logger.Error($"未知服务控制命令: { command.cmd }");
                    }
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
            QXSTSerialPort.Instance.SendRawData("10 03 11 FF C7 FF FF 00 00 00 00");
        }

        public void BeginPowerOn()
        {
            lastChangePowerStatus = Environment.TickCount;
            Logger.Info($"发送打开电源指令");
            QXSTSerialPort.Instance.SendRawData("10 03 11 FF C7 FF FF FF C7 FF FF");

            if(GlobalConf.getconf<int>("base", "enablecom2", 0) != 0)
            {
                Delay.Run(1000 * 10, () =>
                {
                    Logger.Info($"发送主机开机指令");
                    QXSTSerialPort.Instance2.SendRawData("10 03 12 FC FF FF FF FF FF FF FF");
                });
            }
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

        private void OnLightStatusYY(bool st, Action<string> sender)
        {
            Logger.Info($"影院灯 {st}");
            if (st)
            {
                Logger.Info($"发送灯光三开指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 20 00 00 00 20 00 00");                
            }
            else
            {
                Logger.Info($"发送灯光三关指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 20 00 00 00 00 00 00");
            }           
        }

        private void OnLightStatusJY(bool st, Action<string> sender)
        {
            Logger.Info($"教育灯 {st}");

            if (st)
            {
                Logger.Info($"发送灯光二开指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 10 00 00 00 10 00 00");
            }
            else
            {
                Logger.Info($"发送灯光二关指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 10 00 00 00 00 00 00");
            }
        }

        private void OnLightStatusWRJ(bool st, Action<string> sender)
        {
            Logger.Info($"无人机灯 {st}");

            if (st)
            {
                Logger.Info($"发送灯光一开指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 08 00 00 00 08 00 00");
            }
            else
            {
                Logger.Info($"发送灯光一关指令");
                QXSTSerialPort.Instance.SendRawData("10 03 11 00 08 00 00 00 00 00 00");
            }
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

            if (command.cmd == "@lc")
            {
                if(!command.getarg<string>(0, out string pageid))
                {
                    Logger.Error($"发展历程 参数错误");
                    return;
                }
            
                switch(pageid)
                {
                    case "left":
                        _sliderCtrl.SendCommand("ccc left");
                        _server.SendCommand("ccc left");
                        break;

                    case "right":
                        _sliderCtrl.SendCommand("ccc right");
                        _server.SendCommand("ccc right");
                        break;

                    case "stop":
                        _sliderCtrl.SendCommand("ccc stop");
                        break;

                    default:
                        break;
                }

                return;
            }

            if (command.cmd == "@yy")
            {
                if (!command.getarg<string>(0, out string pageid))
                {
                    Logger.Error($"影院文旅 参数错误");
                    return;
                }

                _yyCtrl.SendCommand(pageid);
                //return;
            }


            if (command.cmd[0] == '@')
            {
                if(SendForwardCommand(command))
                {
                    return;
                }
            }
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
        private ToggerCheck powerCheck,ltyyCheck,ltjyCheck,ltwrjCheck= null;
    }
}
