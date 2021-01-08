using MidCtrl;
using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WO
{
    public class MPVPlayer : WOPlugIndepend<MPVPlayer>
    {
        public override bool init()
        {
            string file = GetArgument(0);          
            if (string.IsNullOrWhiteSpace(file))
            {
                Logger.Warning($"[MPVPlayer] no file to open.");
                return false;
            }

            notify_pause = GetArgument(1);
            notify_unpause = GetArgument(2);
            notify_postion = GetArgument(3);

            if (!file.StartsWith("http"))
            {
                file = Path.Combine(PathHelp.appDir, file);
            }            

            Logger.Debug($"[MPVPlayer] try open: {file}");
            rpc.SetEventListener(OnMPVEvent);
            rpc.Start(file);
            Delay.Every(500, OnUpdatePosition);
            return true;
        }

        public override bool show()
        {
            Logger.Debug($"[MPVPlayer] show {rpc.Window}");
            restart();
            WinHelp.ShowWindow(rpc.Window, 1);            
            return true;
        }

        public override bool hide()
        {
            Logger.Debug($"[MPVPlayer] hide {rpc.Window}");
            WinHelp.ShowWindow(rpc.Window, 0);
            pause();
            return true;
        }

        public override void destory()
        {
            rpc.Close();
        }

        private void OnMPVEvent(Dictionary<string, object> jsobj)
        {
            switch (jsobj["event"])
            {
                case "pause":
                    ispause = true;
                    if(!string.IsNullOrEmpty(notify_pause) && notify_pause != "-")
                    {
                        SendCommand(notify_pause);
                    }
                    break;

                case "unpause":
                    ispause = false;
                    if(!string.IsNullOrEmpty(notify_unpause) && notify_unpause != "-")
                    {
                        SendCommand(notify_unpause);
                    }
                    break;

                case "start-file":
                    break;

                default:
                    break;
            }
        }

        private void OnUpdatePosition()
        {
            if(string.IsNullOrEmpty(notify_postion))
            {
                return;
            }

            if(notify_postion == "-")
            {
                return;
            }

            if(ispause)
            {
                return;
            }

            if(Environment.TickCount - lastSeekTime < 2000)
            {
                return;
            }

            double dur = rpc.QueryFloatProperty("duration");
            double pos = rpc.QueryFloatProperty("time-pos");
            float percent = 0;

            if (dur != 0)
            {
                if (pos >= dur)
                {
                    percent = 100f;
                }
                else
                {
                    percent = (float)(pos * 100.0f / dur);
                }
            }

            SendCommand(string.Format(notify_postion, percent));
        }

        public override void OnRecvCommand(string cmd)
        {
            CmdLine c = new CmdLine(cmd);

            switch(c.cmd)
            {
                case "restart":
                    restart();
                    break;

                case "pause":
                    pause();
                    break;

                case "start":
                    play();
                    break;

                case "play":
                    if(c.getarg<string>(0, out string file))
                    {
                        if (!file.StartsWith("http"))
                        {
                            file = Path.Combine(PathHelp.appDir, file);
                        }

                        Logger.Debug($"[MPVPlayer] try open: {file}");
                        rpc.Play(file);
                    }
                    break;

                case "seek":
                    if(c.getarg<double>(0, out double seekp))
                    {
                        rpc.Request($"seek {seekp} absolute-percent");
                        lastSeekTime = Environment.TickCount;
                    }
                    break;

                default:
                    rpc.Request(cmd);
                    break;
            }
        }

        public void pause()
        {
            rpc.Request("set_property pause true");
        }

        public void play()
        {
            rpc.Request("set_property pause false");
        }

        public void restart()
        {
            rpc.Request("seek 0 absolute");
            rpc.Request("set_property pause false");
        }

        private MpvRpc rpc = new MpvRpc();
        private string notify_postion = "";
        private string notify_pause = "";
        private string notify_unpause = "";
        private int lastSeekTime = 0;
        private bool ispause = false;

        static void Main(string[] args)
        {
            CSharpInitalize(args);
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
