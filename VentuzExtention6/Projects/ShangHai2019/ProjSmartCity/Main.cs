using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Page;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Control;

namespace ProjSmartCity
{
    public class Main
    {
        public static Main instance = new Main();

        public void StartUp()
        {
            QXSTSerialPort.Instance = new QXSTSerialPort(new QXSTSerialProtocol2());
            QXSTSerialPort.Instance.Open();
            QXSTSerialPort.Instance._procol.MontherBoardId = 0x0b;

            ControlServer.Instance.OnReciveCommand = OnServerCmd;
            ControlServer.Instance.Start(3131);

            Page.Control.SetProperty("MainAni", "1");
            Page.Control.SetProperty("UseBigTitle", "true");
            Page.Control.SetProperty("BigTitle", "0");
            Page.Control.SetProperty("SmallTitle", "0");
            Page.Control.SetProperty("pptPage", "-1");
        }

        public void OnServerCmd(string cmd)
        {
            CmdLine command = new CmdLine(cmd);

            // 控制 Pad 按钮
            switch(command.cmd)
            {
                case "zycl":
                case "csgl":
                case "zhaf":
                case "hjjc":
                case "af_js":
                case "hj_js":
                case "af_sc":
                case "home":
                    selBtn.SetCurSel(command.cmd);
                    break;
            }

            //
            switch(command.cmd)
            {
                case "home":
                    Page.Control.SetProperty("MainAni", "1");
                    Page.Control.SetProperty("UseBigTitle", "true");
                    Page.Control.SetProperty("BigTitle", "0");
                    break;

                case "zycl":
                    Page.Control.SetProperty("MainAni", "2");
                    Page.Control.SetProperty("UseBigTitle", "false");
                    cn_page = 0;
                    break;

                case "csgl":
                    Page.Control.SetProperty("MainAni", "3");
                    Page.Control.SetProperty("UseBigTitle", "true");
                    Page.Control.SetProperty("BigTitle", "1");
                    break;

                case "zhaf":
                    Page.Control.SetProperty("MainAni", "4");
                    Page.Control.SetProperty("UseBigTitle", "true");
                    Page.Control.SetProperty("BigTitle", "2");
                    af_page = 0;
                    break;

                case "hjjc":
                    Page.Control.SetProperty("MainAni", "7");
                    Page.Control.SetProperty("UseBigTitle", "true");
                    Page.Control.SetProperty("BigTitle", "3");
                    hj_page = 0;
                    break;

                case "af_sc":
                    Page.Control.SetProperty("MainAni", "8");
                    Page.Control.SetProperty("UseBigTitle", "true");
                    Page.Control.SetProperty("BigTitle", "2");
                    break;
            }

            // 翻页状态
            if(selBtn.CurSel != "zycl")
            {
                cn_page = -1;
            }
            else
            {
                if(command.cmd == "cn_next")
                {
                    if(cn_page < 3)
                    {
                        ++cn_page;
                    }
                }
                else if(command.cmd == "cn_prev")
                {
                    if(cn_page > 0)
                    {
                        --cn_page;
                    }
                }
                else
                {
                    cn_page = 0;
                }
            }

            if(selBtn.CurSel != "zhaf")
            {
                af_page = -1;
            }
            else
            {
                if(command.cmd == "af_next" || command.cmd == "af_prev" || command.cmd == "zhaf")
                {
                    if(command.cmd == "af_next")
                    {
                        if(af_page < 3)
                        {
                            af_page++;
                        }
                    }
                    else if(command.cmd == "af_prev")
                    {
                        if(af_page > 0)
                        {
                            af_page--;
                        }
                    }
                
                    switch(af_page)
                    {
                        case 0:
                            Page.Control.SetProperty("MainAni", "4");
                            break;

                        case 1:
                            Page.Control.SetProperty("MainAni", "4");
                            break;

                        case 2:
                            Page.Control.SetProperty("MainAni", "5");
                            break;

                        case 3:
                            Page.Control.SetProperty("MainAni", "9");
                            break;
                    }
                }
            }

            Page.Control.SetProperty("LossChild", af_page == 1 ? "1" : "0");

            if(selBtn.CurSel != "hjjc")
            {
                hj_page = -1;
            }
            else
            {
                if(command.cmd == "hj_next" || command.cmd == "hj_prev" || command.cmd == "hjjc")
                {
                    if (command.cmd == "hj_next")
                    {
                        if(hj_page < 5)
                        {
                            hj_page++;
                        }
                    }
                    else if(command.cmd == "hj_prev")
                    {
                        if(hj_page > 0)
                        {
                            hj_page--;
                        }
                    }

                    switch(hj_page)
                    {
                        case 0:
                            Page.Control.SetProperty("MainAni", "7");
                            break;

                        case 1:
                            Page.Control.SetProperty("MainAni", "10");
                            break;

                        case 4:
                            Page.Control.SetProperty("MainAni", "11");
                            break;

                        case 5:
                            Page.Control.SetProperty("MainAni", "12");
                            break;
                    }
                }
            }

            if (command.cmd == "af_js")
            {
                Page.Control.SetProperty("overlay", "1");
                Page.Control.SetProperty("camera", "1");

            }
            else if(command.cmd == "hj_js")
            {
                Page.Control.SetProperty("overlay", "2");
                Page.Control.SetProperty("camera", "2");
            }
            else if(hj_page == 2)
            {
                Page.Control.SetProperty("overlay", "3");
                Page.Control.SetProperty("camera", "3");
            }
            else if(hj_page == 3)
            {
                Page.Control.SetProperty("overlay", "4");
                Page.Control.SetProperty("camera", "4");
            }
            else
            {
                Page.Control.SetProperty("overlay", "0");
                Page.Control.SetProperty("camera", "0");
            }            

            // 控制沙盘灯光
            lightStatus.SetStatus(hj_page >= 1 && hj_page <= 3);

            Page.Control.SetProperty("pptPage", $"{cn_page}");
            if(selBtn.CurSel == "zhaf" || selBtn.CurSel == "af_js" || selBtn.CurSel == "af_sc")
            {
                Page.Control.SetProperty("IsAtAf", "true");
            }
            else
            {
                Page.Control.SetProperty("IsAtAf", "false");
            }

            if(selBtn.CurSel == "hjjc" || selBtn.CurSel == "hj_js")
            {
                Page.Control.SetProperty("IsAtHj", "true");
            }
            else
            {
                Page.Control.SetProperty("IsAtHj", "false");
            }
        }

        private SingleSel selBtn = new SingleSel();
        private TriggerChange lightStatus = new TriggerChange(() => {
            QXSTSerialPort.Instance.SendOn(0x0a, p => p.SetSandLightMask(0x020000, 0x030000));
        }, () => {
            QXSTSerialPort.Instance.SendOn(0x0a, p => p.SetSandLightMask(0x010000, 0x030000));
        });

        private int hj_page = -1;
        private int cn_page = -1;
        private int af_page = -1;
    }
}
