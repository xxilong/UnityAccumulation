using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using ShareLib.Ayz;

namespace MoviePlayer
{
    class Program
    {
        static void Main(string[] args) => new Program().Main();
        
        private void Main()
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger("\r"),
                new FileLogger(),
            });

            _mpv = new MpvRpc();
            _mpv.SetEventListener(OnMPVEvent);
            _mpv.Start();

            _srv.SetReciverListener(OnReciveCommand);
            _srv.SetPageSendListener(OnPageSended);
            _srv.Start(GlobalConf.getconf<int>("control", "port", 3131),
                Path.Combine(PathHelp.appDir, "pages"),
                GlobalConf.getconf("control", "page", "single"));

            //Delay.Every(2000, OnUpdatePosition);

            while (true)
            {
                Console.Write(">>> ");
                string req = Console.ReadLine();
                object res = _mpv.Request(req.Trim());
                if(res != null)
                {
                    Console.WriteLine($"--- {res}");
                }
            }            
        }

        private void OnReciveCommand(string cmdline)
        {
            Logger.Info($"recived command: {cmdline}");
            CmdLine cmd = new CmdLine(cmdline);
            if(cmd.cmd == "play")
            {
                if(!cmd.getarg<string>(0, out string name))
                {
                    Logger.Error("缺少文件名参数.");
                    return;           
                }

                string file = Path.Combine(PathHelp.appDir, "movies", name);
                file = "file://" + file.Replace("\\", "/");
                _mpv.Request($"loadfile {file}");
                return;
            }
            else
            {
                _mpv.Request(cmdline);
            }
        }
 
        private void OnPageSended()
        {
            //bool pause = _mpv.QueryBoolProperty("pause");            
            //_srv.SetCheck("pause", pause);
            //UpdatePlaylist();
        }

        private void UpdatePlaylist()
        {
            object res = _mpv.Request("get_property playlist");
            JArray lists = res as JArray;
            if(lists == null)
            {
                Logger.Error($"Fetch playlist failed: {res}");
                return;
            }

            string filelist = "";
            
            foreach(JObject obj in lists)
            {
                string path = obj["filename"].ToString();
                string filename = Path.GetFileNameWithoutExtension(path);
                if(filelist != "")
                {
                    filelist += ";";
                }

                filelist += filename.Replace(' ', '_');
            }

            _srv.SendCommand($"+text playlist {filelist}");
        }

        private void OnUpdatePosition()
        {
            double dur = _mpv.QueryFloatProperty("duration");
            double pos = _mpv.QueryFloatProperty("time-pos");
            float percent = 0;

            if(dur != 0)
            {
                if(pos >= dur)
                {
                    percent = 100f;
                }
                else
                {
                    percent = (float)(pos * 100.0f / dur);
                }
            }

            _srv.SendCommand($"+text times {FormatTimeSeconds((int)pos)}/{FormatTimeSeconds((int)dur)}");
            _srv.SendCommand($"+percent process {percent}");
        }

        private void OnMPVEvent(Dictionary<string, object> jsobj)
        {
            switch (jsobj["event"])
            {
                case "pause":
                    //_srv.SendCommand("+check pause");
                    break;

                case "unpause":
                    //_srv.SendCommand("+uncheck pause");
                    break;

                case "start-file":
                    //_srv.SendCommand("+text filename " + _mpv.QueryStrProperty("filename/no-ext").Replace(' ', '_'));
                    break;

                default:
                    break;
            }
        }

        private string FormatTimeSeconds(int sec)
        {
            int s = sec % 60;
            int min = sec / 60;
            int m = min % 60;
            int h = min / 60;

            return $"{h.ToString().PadLeft(2, '0')}:{m.ToString().PadLeft(2, '0')}:{s.ToString().PadLeft(2, '0')}";
        }               

        private PageControlServer _srv = new PageControlServer();
        private MpvRpc _mpv = new MpvRpc();
    }
}
