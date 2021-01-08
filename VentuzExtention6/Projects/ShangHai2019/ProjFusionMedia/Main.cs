using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Page;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Control;

namespace ProjFusionMedia
{
    public class Main
    {
        static public Main instance = new Main();

        public void StartUp()
        {
            ControlServer.Instance.OnReciveCommand = OnServerCmd;
            ControlServer.Instance.Start(3131);
            QXSTSerialPort.Instance.Open();

            Page.Control.SetProperty("EditVideoItem", $"0");
            Page.Control.SetProperty("EditLogoItem", $"0");
            Page.Control.SetProperty("EditWapItem", $"0");
            Page.Control.SetProperty("PlayVideoItem", $"0");
            Page.Control.SetProperty("PlayLogoItem", $"0");
            Page.Control.SetProperty("PlayWapItem", $"0");
        }

        public void OnServerCmd(string cmd)
        {
            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "reset":
                    SetSelPos(0);
                    SetSelLogo(0);
                    SetSelWapper(0);
                    Page.Control.SetProperty("PlayVideoItem", "0");
                    Page.Control.SetProperty("PlayLogoItem", "0");
                    Page.Control.SetProperty("PlayWapItem", "0");
                    break;

                case "play":
                    if (isready)
                    {
                        Page.Control.SetProperty("PlayVideoItem", $"{curSelPos}");
                        Page.Control.SetProperty("PlayLogoItem", $"{curSelLogo}");
                        Page.Control.SetProperty("PlayWapItem", $"{curSelWaper}");
                    }
                    break;

                case "pos":
                    if(command.getarg<int>(0, out int pos))
                    {
                        SetSelPos(pos);
                    }
                    break;

                case "logo":
                    if(command.getarg<int>(0, out int logo))
                    {
                        SetSelLogo(logo);
                    }
                    break;

                case "wap":
                    if(command.getarg<int>(0, out int wap))
                    {
                        SetSelWapper(wap);
                    }
                    break;

                default:
                    Logger.Error($"未知命令: { command.cmd }");
                    break;
            }
        }

        private void SetSelPos(int pos)
        {
            if(curSelPos == pos)
            {
                return;
            }

            if(curSelPos != 0)
            {
                ControlServer.Instance.SendCommand($"+uncheck p{curSelPos}");
            }

            if(curSelPos == 6)
            {
                QXSTSerialPort.Instance.SetOutSignal(0, 1);
            }

            if(pos != 0)
            {
                ControlServer.Instance.SendCommand($"+check p{pos}");
            }

            if(pos == 6)
            {
                QXSTSerialPort.Instance.SetOutSignal(1, 1);
            }

            curSelPos = pos;
            Page.Control.SetProperty("EditVideoItem", $"{curSelPos}");
            CheckAllReady();
        }

        private void SetSelLogo(int logo)
        {
            if (curSelLogo == logo)
            {
                return;
            }

            if (curSelLogo != 0)
            {
                ControlServer.Instance.SendCommand($"+uncheck l{curSelLogo}");
            }

            if (logo != 0)
            {
                ControlServer.Instance.SendCommand($"+check l{logo}");
            }

            curSelLogo = logo;
            Page.Control.SetProperty("EditLogoItem", $"{curSelLogo}");
            CheckAllReady();
        }

        private void SetSelWapper(int wap)
        {
            if (curSelWaper == wap)
            {
                return;
            }

            if (curSelWaper != 0)
            {
                ControlServer.Instance.SendCommand($"+uncheck w{curSelWaper}");
            }

            if (wap != 0)
            {
                ControlServer.Instance.SendCommand($"+check w{wap}");
            }

            curSelWaper = wap;
            Page.Control.SetProperty("EditWapItem", $"{curSelWaper}");
            CheckAllReady();
        }

        private void CheckAllReady()
        {
            bool ready = curSelLogo != 0 && curSelPos != 0 && curSelWaper != 0;
            if(isready == ready)
            {
                return;
            }

            isready = ready;
            if(isready)
            {
                ControlServer.Instance.SendCommand($"+check play");
            }
            else
            {
                ControlServer.Instance.SendCommand($"+uncheck play");
            }
        }

        private int curSelPos = 0;
        private int curSelLogo = 0;
        private int curSelWaper = 0;
        private bool isready = false;
    }
}
