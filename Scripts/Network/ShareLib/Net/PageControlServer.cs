using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareLib.Net
{
    public class PageControlServer
    {
        public void Start(int port, string pagePath, string defautPage = "index")
        {
            _server.SetReciverListener(OnReciveClientCommand);
            _server.Start(port);

            try
            {
                _pageDir = pagePath;
                _watcher.Changed += OnFileChanged;
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Path = pagePath;
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrEmpty(defautPage))
            {
                SwitchPage(defautPage);
            }

            if (GlobalConf.getconf<bool>("upserv", "enable", false))
            {
                _reporter = new PageReportClient();
                _reporter.SetRecvCommandListener(HandleServerCtrlCommand);
                _reporter.Start(port);
            }
        }

        // 普通命令
        public void SetReciverListener(Action<string> reciver) { OnReciveCommandAdapter = reciver; }

        // 页面控制命令
        public void SetPageCtrlListener(Func<CmdLine, bool> act) {OnRecivePageCtrlCommand = act;}

        // 服务器控制命令
        public void SetSrvCtrlListener(Action<string, string> act) { OnReciveServerCtrlCommand = act; }

        // 向客户端发送完页面回调
        public void SetPageSendListener(Action reciver) { OnPageSended = reciver; }

        // 客户端连接状态发生变化
        public void SetCtrlChangedListener(Action<bool> act) { _server.SetCtrlChangedListener(act); }

        // 有客户端断开了连接
        public void SetClientCloseListener(Action<string> act) { _server.SetClientCloseListener(act); }
        public void SendCommand(string cmd)
        {
            _server.SendCommand(cmd);
            if(OnPageSended != null && _server.ClientCount > 0 && cmd.Length > 0 && cmd[0] == '@')
            {                
                Delay.Run(2000, ()=>{
                    OnPageSended.Invoke();
                });
            }
        }

        public void SetCheck(string id, bool check)
        {
            if(check)
            {
                SendCommand("+check "+id);
            }
            else
            {
                SendCommand("+uncheck "+id);
            }
        }
        public void SwitchPage(string name)
        {
            try
            {
                if(_inMemoryPages.ContainsKey(name))
                {
                    string content = "@" + _inMemoryPages[name];
                    SendCommand(content);
                    _curPagePath = string.Format("#{0}#", name);
                    _curPageConent = content;
                    _watcher.Filter = name + ".uipage";
                    _watcher.EnableRaisingEvents = false;
                }
                else
                {
                    string pagePath = Path.Combine(_pageDir, name + ".uipage");
                    string content = "@" + File.ReadAllText(pagePath);
                    SendCommand(content);
                    _curPagePath = pagePath;
                    _curPageConent = content;
                    _watcher.Filter = name + ".uipage";
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch(Exception e)
            {
                Logger.Error(string.Format("Exception on switch page to {0}: {1}", name, e.Message));
            }            
        }

        public void SwitchPageContent(string content)
        {
            try
            {
                content = "@" + content;
                SendCommand(content);
                _curPageConent = content;
                _watcher.EnableRaisingEvents = false;
            }
            catch (Exception e)
            {
                Logger.Error("Exception on set page content: "+ e.Message);
            }
        }

        public void SetPageContent(string name, string content)
        {
            _inMemoryPages[name] = content;
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            int now = Environment.TickCount;
            if (now - lastChangeTime < 500)
            {
                return;
            }

            lastChangeTime = now;

            Delay.Run(100, () =>
            {
                Logger.Info("PageFile Changed.");
                _curPageConent = "@" + File.ReadAllText(_curPagePath);
                SendCommand(_curPageConent);
            });
        }

        private void OnReciveClientCommand(string cmd, string peername)
        {
            Logger.Info(string.Format("收到客户端 {0} 命令: {1}", peername, cmd));

            if(cmd.Length > 0 && cmd[0] == '$')
            {
                HandlePageCtrlCommand(cmd, peername);
                return;
            }

            if(cmd.Length > 0 && cmd[0] == '#')
            {
                HandleServerCtrlCommand(cmd, peername);
                return;
            }

            if(OnReciveCommandAdapter!=null)
                OnReciveCommandAdapter.Invoke(cmd);
        }

        private void HandlePageCtrlCommand(string cmd, string peername)
        {
            CmdLine command = new CmdLine(cmd);

            if (OnRecivePageCtrlCommand != null&& OnRecivePageCtrlCommand.Invoke(command))
            {
                return;
            }

            switch (command.cmd)
            {
                case "$GCP":
                    SendCommand(_curPageConent);
                    break;

                case "$GP":
                    string name;
                    if(command.getarg<string>(0, out name))
                    {
                        SwitchPage(name);
                    }
                    break;

                case "$RES":
                    string filename;
                    if (command.getarg<string>(0, out filename))
                    {
                        HandleFetchResourceCommand(filename, peername);
                    }
                    break;

                default:
                    Logger.Error("未知页面控制命令: "+command.cmd );
                    break;
            }
        }

        protected virtual void HandleServerCtrlCommand(string cmd, string clientname)
        {
            if(OnReciveServerCtrlCommand != null)
            {
                OnReciveServerCtrlCommand.Invoke(cmd, clientname);
                return;
            }

            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#shutdown":
                    Machine.shutdown();
                    break;

                default:
                    Logger.Error("未知服务控制命令: "+ command.cmd);
                    break;
            }
        }

        protected virtual void HandleFetchResourceCommand(string filename, string clientname)
        {
            string resPath = Path.Combine(_pageDir, filename);
            if(!File.Exists(resPath))
            {
                _server.SendDataTo(filename, new byte[0], clientname);
                return;
            }

            _server.SendDataTo(filename, File.ReadAllBytes(resPath), clientname);
        }

        private Action<string> OnReciveCommandAdapter = null;
        private Action<string, string> OnReciveServerCtrlCommand = null;
        private Func<CmdLine, bool> OnRecivePageCtrlCommand = null;

        private Action OnPageSended = null;
        private CmdTcpServer _server = new CmdTcpServer();

        private string _pageDir;
        private string _curPagePath = "";
        private string _curPageConent = "";
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private Dictionary<string, string> _inMemoryPages = new Dictionary<string, string>();
        private int lastChangeTime = 0;

        private PageReportClient _reporter;
    }
}
