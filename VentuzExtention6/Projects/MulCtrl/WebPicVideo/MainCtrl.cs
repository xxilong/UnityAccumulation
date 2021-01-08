using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Page;
using System;
using System.IO;
using Ventuz.Extention.Control;

namespace WebPicVideo
{
    public class MainCtrl
    {
        public static MainCtrl Instance = new MainCtrl();

        public void StartUp()   // 程序启动
        {
            string dllini = Path.Combine(RootDir, "wpv.ini");
            GlobalConf.Set(dllini);

            int port = GlobalConf.getconf<int>("serv", "port", 3131);
            string begcmd = GlobalConf.getconf<string>("serv", "start", "");

            // 启动服务器
            ControlServer.Instance.OnReciveCommand = OnReciveCommand;
            ControlServer.Instance.Start(port);

            if(!string.IsNullOrEmpty(begcmd))
            {
                OnReciveCommand(begcmd);
            }
        }

        private void OnReciveCommand(string line)
        {
            string[] words = line.Split(_sep, 2);
            if(words.Length != 2)
            {
                Logger.Warning($"错误命令: {line}");
                return;
            }

            string media = PauseURL(words[1]);

            switch(words[0].Trim())
            {
                case "play":
                    Page.Control.SetProperty("video", "true");
                    Page.Control.SetProperty("web", "false");
                    Page.Control.SetProperty("image", "false");
                    Page.Control.SetProperty("videofile", media);
                    break;

                case "view":
                    Page.Control.SetProperty("video", "false");
                    Page.Control.SetProperty("web", "false");
                    Page.Control.SetProperty("image", "true");
                    Page.Control.SetProperty("picfile", media);
                    break;

                case "open":
                    Page.Control.SetProperty("video", "false");
                    Page.Control.SetProperty("web", "true");
                    Page.Control.SetProperty("image", "false");
                    Page.Control.SetProperty("webfile", media);
                    break;
            }
        }

        private string PauseURL(string path)
        {
            path = path.Trim();
            if(path.IndexOf(':') != -1)
            {
                return path;
            }

            return Path.Combine(RootDir, path);
        }

        private string RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "WPV");
        private readonly char[] _sep = new char[] { ' ', '\t' };
    }
}
