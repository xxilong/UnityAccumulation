using Newtonsoft.Json;
using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.IO;

namespace ProjMedical
{
    class DynamicServer
    {
        private PageControlServer _server = new PageControlServer();
        private LLServerManager _lowersrv = new LLServerManager();

        public void Main()
        {
            _lowersrv.Init(_server, this);

            string mainPage = GlobalConf.getconf("dynamic", "mainmode", "index");
            int port = GlobalConf.getconf<int>("dynamic", "port", 3131);

            _clientConfig = "";
            foreach(string key in GlobalConf.DomainKeys("client"))
            {
                string val = GlobalConf.getconf<string>("client", key);
                if(string.IsNullOrEmpty(key) || string.IsNullOrEmpty(val))
                {
                    continue;
                }
                _clientConfig += $"{key}={val}";
            }

           
           

            _server.SetSrvCtrlListener(HandleServerCtrlCommand);
            _server.SetClientCloseListener(HandleClientClose);
            _server.SetPageCtrlListener(HandlePageCtrlCommand);
            _server.Start(port, "", null);

            SendDynPage();
            while (true)
            {
                string cmdline = Console.ReadLine();
                SendCommand(cmdline);
            }
        }

        private void SendCommand(string cmd) => _server.SendCommand(cmd);
        public void SetReciverListener(Action<string> reciver) => _server.SetReciverListener(reciver);

        private bool HandlePageCtrlCommand(CmdLine cmd)
        {
            if(cmd.cmd == "$GP" && cmd.getarg<string>(0, out string name))
            {
                if(name == "dymain")
                {
                    SendDynPage();
                    return true;
                }
            }

            if(cmd.cmd == "$GCF")
            {
                SendConfig();
                return true;
            }

            return false;
        }

        private void HandleServerCtrlCommand(string cmd, string clientname)
        {
            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#shutdown":
                    _lowersrv.RunShutdown();
                    break;

                case "#reg":
                    if(command.getarg<string>(0, out string name) && command.getarg<int>(1, out int port))
                    {
                        int order = 100;
                        command.getarg<int>(2, out order);
                        _lowersrv.AddSubServer(clientname, name, port, order);
                    }
                    break;

                default:
                    Logger.Error($"未知服务控制命令: { command.cmd }");
                    break;
            }
        }
        private void HandleClientClose(string clientname)
        {
            _lowersrv.RemoveSubServer(clientname);
        }
        public void SendDynPage()
        {
            try
            {
                PageDefine page = JsonConvert.DeserializeObject<PageDefine>(_modeContent);

                ClientItem[] subItems = _lowersrv.SubItems;
                int count = subItems.Length;
                if(count > 27)
                {
                    count = 27;
                    Logger.Warning($"The page design max 27 items, but here is {subItems.Length}, rest is ignored.");
                }

                PageButton[] buildbtns = page.buttons;
                int bincount = 0;
                if(buildbtns != null)
                {
                    bincount = buildbtns.Length;
                }
                page.buttons = new PageButton[count + bincount];

                for(int i = 0; i < bincount; ++i)
                {
                    page.buttons[i] = buildbtns[i];
                    page.buttons[i].rect = new int[] { 300, 0, 500, 80 };
                }

                for(int i = 0; i < count; ++i)
                {
                    page.buttons[i + bincount] = new PageButton();
                    page.buttons[i + bincount].name = subItems[i].showName;
                    page.buttons[i + bincount].style = "normal";
                    page.buttons[i + bincount].command = $".sub {subItems[i].srvIP} {subItems[i].srvPort}";
                    page.buttons[i + bincount].rect = new int[] { 300, 0, 500, 80 };
                }

                count += bincount;

                int colcount = count / 9 + (count % 9 == 0 ? 0 : 1);
                int colspace = (1800 - colcount * 500) / (colcount + 1);
                int xpos = 80 + colspace;
                int index = 0;

                for (int col = 0; col < colcount; ++col)
                {
                    int rowcount = (col >= count / 9) ? count % 9 : 9;
                    int rowspace = (900 - rowcount * 80) / (rowcount + 1);
                    int ypos = 200 + rowspace;

                    for (int i = 0; i < rowcount; ++i)
                    {
                        page.buttons[index].rect[0] = xpos;
                        page.buttons[index].rect[1] = ypos;
                        ypos += rowspace + 80;
                        ++index;
                    }

                    xpos += colspace + 500;
                }                           

                string ctx = JsonConvert.SerializeObject(page);
                _server.SwitchPageContent(ctx);
            }
            catch(Exception e)
            {
                Logger.Error($"Exception on send dyn page: {e.Message}");
            }
        }

        public void SendConfig()
        {
            if(string.IsNullOrEmpty(_clientConfig))
            {
                return;
            }

            _server.SendCommand("*" + _clientConfig);
        }

        private string _modeContent;
        private string _clientConfig;
    }
}
