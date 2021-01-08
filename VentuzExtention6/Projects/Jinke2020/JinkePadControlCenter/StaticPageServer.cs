using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.IO;

namespace PadControlCenter
{
    class StaticPageServer
    {        
        private PageControlServer _server = new PageControlServer();

        public void Main()
        {
            string pageDir = GlobalConf.getconf_aspath("static", "pagedir");
            string mainPage = GlobalConf.getconf("static", "main", "index");
            int port = GlobalConf.getconf<int>("static", "port", 3131);

            _server.Start(port, pageDir, mainPage);

            while (true)
            {
                string cmdline = Console.ReadLine();
                SendCommand(cmdline);
            }
        }

        private void SendCommand(string cmd) => _server.SendCommand(cmd);
        public void SetReciverListener(Action<string> reciver) => _server.SetReciverListener(reciver);
    }
}
