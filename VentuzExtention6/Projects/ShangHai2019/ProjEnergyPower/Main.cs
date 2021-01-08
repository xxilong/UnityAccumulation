using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Page;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Control;

namespace ProjEnergyPower
{
    public class Main
    {
        static public Main instance = new Main();

        public void StartUp()
        {
            ControlServer.Instance.OnReciveCommand = OnServerCmd;
            ControlServer.Instance.Start(3131);
            Page.Control.SetProperty("Page", "home");
            Page.Control.SetProperty("SDVideo", "false");
            Page.Control.SetProperty("BDVideo", "false");
            Page.Control.SetProperty("PDStatus", "0");
            Page.Control.SetProperty("BDVideoURL", "0");
            Page.Control.OnProperty("Page").Change += OnPageChange;

            QXSTSerialPort.Instance.Open();
            QXSTSerialPort.Instance._procol.SetRelayStatusListener(OnNotifyRelayStatus);
            QXSTSerialPort.Instance._procol.SetArrivedPosListener(OnNotifyArrivedPos);
            QXSTSerialPort.Instance.SwitchCar(CarType.Car);
            QXSTSerialPort.Instance.SelectCar(CarType.Car, 0);
        }

        private void SD_TowerErrorOff() => QXSTSerialPort.Instance.SendGoPos(1);
        private void SD_TowerErrorOn() => QXSTSerialPort.Instance.SendGoPos(2);
        private void SD_UAVBackHome() => QXSTSerialPort.Instance.SendGoPos(3);
        private void SD_UAVToErrorPt() => QXSTSerialPort.Instance.SendGoPos(4);
        private void BD_DeviceOK() => QXSTSerialPort.Instance.SendGoPos(6);
        private void BD_DeviceError() => QXSTSerialPort.Instance.SendGoPos(7);
        private void BD_RobotToErrorPt() => QXSTSerialPort.Instance.SendGoPos(8);
        private void BD_RobotBackHome() => QXSTSerialPort.Instance.SendGoPos(9);
        private void PD_DefaultMode() => QXSTSerialPort.Instance.SendGoPos(0x10);
        private void PD_4GLossPower() => QXSTSerialPort.Instance.SendGoPos(0x12);
        private void PD_4GRepaired() => QXSTSerialPort.Instance.SendGoPos(0x14);
        private void PD_5GAutoRepair() => QXSTSerialPort.Instance.SendGoPos(0x16);
        private void LightDefaultMode() => QXSTSerialPort.Instance.SendGoPos(0x40);
        

        private void OnNotifyRelayStatus(int st)
        {
            if(st == 0x01)
            {
                Page.Control.SetProperty("PDStatus", "2");
            }
        }

        private void OnNotifyArrivedPos(int pos)
        {
            switch(pos)
            {
                case 0x04:
                    uac_arrvied = true;
                    Delay.Run(100, () =>
                    {
                        SD_TowerErrorOn();
                    });                    
                    break;

                case 0x08:
                    rbt_arrvied = true;
                    Delay.Run(100, () =>
                    {
                        BD_DeviceError();
                    });                    
                    break;
            }
        }

        private void OnPageChange(object sender, PropertyChangeArg arg)
        {
            if(arg.fromval == "shudian" && arg.toval != "shudian")
            {
                if(sd_actived)
                {
                    SD_TowerErrorOff();
                    sd_actived = false;
                    uac_arrvied = false;
                    Delay.Run(100, () =>
                    {
                        SD_UAVBackHome();
                    });
                }

            }

            if(arg.fromval == "biandian" && arg.toval != "biandian")
            {
                if(bd_actived)
                {
                    BD_DeviceOK();
                    bd_actived = false;
                    rbt_arrvied = false;
                    Delay.Run(100, () =>
                    {
                        BD_RobotBackHome();
                    });
                }

            }

            if(arg.fromval == "peidian" && arg.toval != "peidian")
            {
                Page.Control.SetProperty("PDStatus", "0");
            }
        }

        public void OnServerCmd(string cmd)
        {
            CmdLine command = new CmdLine(cmd);

            if(command.cmd == "show_sd" && uac_arrvied)
            {
                Page.Control.SetProperty("SDVideo", "true");
            }
            else
            {
                Page.Control.SetProperty("SDVideo", "false");
            }

            if (command.cmd == "show_bd" && rbt_arrvied)
            {
                Page.Control.SetProperty("BDVideo", "true");
            }
            else
            {
                Page.Control.SetProperty("BDVideo", "false");
            }

            if(command.cmd != "loop" && loopCor != null)
            {
                loopCor.Stop();
                loopCor = null;
                ControlServer.Instance.SendCommand("+uncheck loop");
            }

            switch (command.cmd)
            {
                case "loop":     
                    if(loopCor != null)
                    {
                        loopCor.Stop();
                        loopCor = null;
                        Page.Control.SetProperty("Page", "home");
                        SetCurSel("home");
                        ControlServer.Instance.SendCommand("+uncheck loop");
                    }
                    else
                    {
                        loopCor = Coroutine.StartCoroutine(LoopPlay());
                        ControlServer.Instance.SendCommand("+check loop");
                    }
                    break;

                case "home":
                    Page.Control.SetProperty("Page", "home");
                    SetCurSel("home");
                    LightDefaultMode();
                    break;

                case "safeapp":
                    Page.Control.SetProperty("Page", "safeapp");
                    if(command.getarg<string>(0, out string index))
                    {
                        Page.Control.SetProperty("VideoIndex", index);
                    }
                    break;

                case "main_fd":
                    Page.Control.SetProperty("Page", "fadian");
                    SetCurSel("main_fd");
                    break;

                case "main_sd":
                    sd_actived = true;
                    Page.Control.SetProperty("Page", "shudian");
                    SetCurSel("main_sd");
                    SD_UAVToErrorPt();
                    break;

                case "main_bd":
                    bd_actived = true;
                    Page.Control.SetProperty("Page", "biandian");
                    SetCurSel("main_bd");
                    BD_RobotToErrorPt();
                    break;

                case "main_pd":                   
                    Page.Control.SetProperty("Page", "peidian");
                    Page.Control.SetProperty("PDStatus", "0");
                    SetCurSel("main_pd");
                    PD_DefaultMode();
                    break;

                case "main_yd":
                    Page.Control.SetProperty("Page", "yongdian");
                    SetCurSel("main_yd");
                    break;

                case "show_sd":
                    if(uac_arrvied)
                    {                        
                        Page.Control.SetProperty("Page", "shudian");
                        SetCurSel("show_sd");
                    }
                    break;

                case "show_bd":
                    if(rbt_arrvied)
                    {                        
                        Page.Control.SetProperty("Page", "biandian");
                        SetCurSel("show_bd");
                    }
                    break;

                case "show_4g":
                    Page.Control.SetProperty("Page", "peidian");
                    Page.Control.SetProperty("PDStatus", "1");
                    SetCurSel("show_4g");
                    PD_4GLossPower();
                    break;

                case "show_5g":
                    Page.Control.SetProperty("Page", "peidian");
                    Page.Control.SetProperty("PDStatus", "3");
                    SetCurSel("show_5g");
                    PD_5GAutoRepair();
                    break;

                case "show_rep":
                    Page.Control.SetProperty("Page", "peidian");
                    Page.Control.SetProperty("PDStatus", "2");
                    SetCurSel("show_rep");
                    PD_4GRepaired();
                    break;

                case "switch_bdurl":
                    string curval = Page.Control.GetProperty("BDVideoURL");
                    curval = (curval == "0") ? "1" : "0";
                    Page.Control.SetProperty("BDVideoURL", curval);
                    if(curval == "1")
                    {
                        ControlServer.Instance.SendCommand("+check switch_bdurl");
                    }
                    else
                    {
                        ControlServer.Instance.SendCommand("+uncheck switch_bdurl");
                    }
                    break;

                default:
                    Logger.Error($"未知命令: { command.cmd }");
                    break;
            }
        }

        private string curSel = "";
        private bool sd_actived = false;
        private bool bd_actived = false;
        private bool uac_arrvied = false;
        private bool rbt_arrvied = false;
        private Coroutine loopCor = null;

        private void SetCurSel(string btnid)
        {
            if(btnid == curSel)
            {
                return;
            }

            if(curSel != "")
            {
                ControlServer.Instance.SendCommand($"+swcheck {curSel} {btnid}");
            }
            else
            {
                ControlServer.Instance.SendCommand($"+check {btnid}");
            }

            curSel = btnid;
        }

        private IEnumerator LoopPlay()
        {
            while(true)
            {
                Page.Control.SetProperty("Page", "fadian");
                SetCurSel("main_fd");
                yield return 10 * 1000;

                Page.Control.SetProperty("Page", "shudian");
                SetCurSel("main_sd");
                yield return 10 * 1000;

                Page.Control.SetProperty("Page", "biandian");
                SetCurSel("main_bd");
                yield return 10 * 1000;

                Page.Control.SetProperty("Page", "peidian");   
                SetCurSel("main_pd");
                yield return 10 * 1000;

                Page.Control.SetProperty("Page", "yongdian");
                SetCurSel("main_yd");
                yield return 10 * 1000;
            }
        }
    }
}
