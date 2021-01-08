using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Ayz;
using ShareLib.Page;
using ShareLib.Unity;
using System.Collections;

namespace ProjShMWCFactory
{
    public class Main
    {
        public static Main instance = new Main();
        
        public void StartUp()
        {
            //_server.SetOnNewConnected((string peerName) =>
            //{
            //    _server.SendCommandToOther("11 click",peerName);
            //});

            _server.SetReciverListener((string line, string peername) =>
            {
                Logger.Info($"收到客户端 {peername} 命令: {line}");
                CmdLine cmd = new CmdLine(line);
                string arg;
                
                if (cmd.cmd == "engine")
                {
                    if (!cmd.getarg<string>(0, out arg))
                    {
                        return;
                    }

                    if (arg != "status")
                    {
                        return;
                    }

                    if (_IsEngineError)
                    {
                        _server.SendCommand("engine bad");
                    }
                    else
                    {
                        _server.SendCommand("engine good");
                    }
                }
                else
                {
                    _server.SendCommandToOther(line, peername);
                }
                 
            });

            _server.Start(3131);

            QXSTSerialPort.Instance._procol.SetArrivedPosListener(OnArrivedPos);
            QXSTSerialPort.Instance.Open();
            Page.Control.SetProperty("scene", "bg");
        }

        private void OnArrivedPos(int pos)
        {
            Logger.Info($"收到按键状态 {pos}");

            switch (pos)
            {
                case 0x01:
                    avgCoroute = Coroutine.StartCoroutine(AVGMode());
                    break;

                case 0x02:
                    StopAVG();
                    Page.Control.SetProperty("scene", "hj");
                    break;

                case 0x04:
                    StopAVG();
                    Page.Control.SetProperty("scene", "bg");
                    break;

                case 0x08:
                    StopAVG();
                    Page.Control.SetProperty("scene", "csd");
                    break;

                case 0x10:
                    StopAVG();
                    Page.Control.SetProperty("scene", "aj");
                    break;

                case 0x20:
                    _IsEngineError = false;
                    _server.SendCommand("engine good");
                    Logger.Info($"engine status change to good.");
                    break;

                case 0x40:
                    StopAVG();
                    Page.Control.SetProperty("scene", "agv");
                    break;

                case 0x0:
                    _IsEngineError = true;
                    _server.SendCommand("engine bad");
                    Logger.Info($"engine status change to bad.");
                    break;

                default:
                    Logger.Debug($"unknown status {pos}");
                    break;
            }
        }

        private void StopAVG()
        {
            Coroutine.StopCoroutine(avgCoroute);
            avgCoroute = null;
        }

        private IEnumerator AVGMode()
        {
            Logger.Info("Begin Coroute");

            //if(Page.Control.GetProperty("scene") != "bg")
            //{
            //    Logger.Info("Back to home");
            //    Page.Control.SetProperty("scene", "bg");
            //    yield return -1;    // 等待回到主页
            //}

            Logger.Info("Wait 2 Seconds");
            yield return 2000;
            Logger.Info("Goto Camera");
            Page.Control.SetProperty("scene", "cam");
            Logger.Info("Wait 8 Seconds");
            yield return 8000;
            //Page.Control.SetProperty("scene", "agv");
            //yield return -1;
            //yield return 18000;
            Logger.Info("Back to home");
            Page.Control.SetProperty("scene", "bg");
        }

        public void OnEnterSceneLoop()
        {
            if(avgCoroute != null)
            {
                avgCoroute.Continue();
            }
        }

        private CmdTcpServer _server = new CmdTcpServer();
        private bool _IsEngineError = true;
        private Coroutine avgCoroute = null;
    }
}
