using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Packer;
using ShareLib.Unity;
using ShareLib.PadUI;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareLib.Net
{
    public class PageControlServer
    {
        public void Start(int port, string pagePath, string defautPage = "index")
        {
            Start(port, new DiskFileReader(pagePath), defautPage);
        }

        public void Start(int port)
        {
            Start(port, new NoneFileReader(), "");
        }

        public void Start(int port, IFileReader reader, string defautPage = "index")
        {
            _lastRecvTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            _freader = reader;
            _freader.OnWatchFileChanged += OnFileChanged;
            _server.SetReciverListener(OnReciveClientCommand);
            _server.Start(port);

            if (!string.IsNullOrEmpty(defautPage))
            {
                SwitchPage(defautPage);
            }

            if (GlobalConf.getconf<bool>("upserv", "enable", false))
            {
                _reporter = new PageReportClient();

                if (GlobalConf.getconf<bool>("upserv", "recv_cmd", false))
                {
                    _reporter.SetRecvCommandListener(OnReciveClientCommand);
                }
                else
                {
                    _reporter.SetRecvCommandListener(HandleServerCtrlCommand);
                }
                _reporter.Start(port);
            }
        }

        // 普通命令
        public void SetReciverListener(Action<string> reciver) => OnReciveCommandAdapter += reciver;

        // 页面控制命令
        public void SetPageCtrlListener(Func<CmdLine, bool> act) => OnRecivePageCtrlCommand = act;

        // 页面控制命令
        public void SetAfterPageCtrlListener(Action<string> act) => OnReciveAfterPageCtrlCommand = act;
        // 服务器控制命令
        public void SetSrvCtrlListener(Action<string, string> act) => OnReciveServerCtrlCommand = act;

        // 向客户端发送完页面回调
        public void SetPageSendListener(Action reciver) => OnPageSended = reciver;

        // 客户端连接状态发生变化
        public void SetCtrlChangedListener(Action<bool> act) => _server.SetCtrlChangedListener(act);

        // 客户端修改语言
        public void SetLanguageChangedListener(Action<string> act) => OnLanguageChanged = act;

        public UIStateManager Status { get => _status; }

        // 有客户端断开了连接
        public void SetClientCloseListener(Action<string> act) => _server.SetClientCloseListener(act);
        public void SendCommand(string cmd)
        {
            _server.SendCommand(cmd);
            if (_server.ClientCount > 0 && cmd.Length > 0 && cmd[0] == '@')
            {
                _status.UpdateStatus((string arg) => _server.SendCommand(arg));

                if (OnPageSended != null)
                {
                    Delay.Run(1000, () =>
                    {
                        OnPageSended?.Invoke();
                    });
                }
            }
        }

        public void SendCommandTo(string cmd, string peer) => _server.SendCommandTo(cmd, peer);

        public void SendCommandToOther(string cmd, string peer)=> _server.SendCommandToOther(cmd, peer);

        public void SetCheck(string id, bool check)
        {
            if (check)
            {
                SendCommand($"+check {id}");
            }
            else
            {
                SendCommand($"+uncheck {id}");
            }
        }
        public void SwitchPage(string name)
        {
            try
            {
                if (_inMemoryPages.ContainsKey(name))
                {
                    string content = "@" + _inMemoryPages[name];
                     _curPageName = $"#{name}#";
                    _curPageConent = content;
                    PreProcessPage();
                    _freader.UnWatch();
                }
                else
                {
                    string pageName = name + ".uipage";
                    string content = "@" + ReadPageText(pageName);
                    _curPageName = pageName;
                    _curPageConent = content;
                    PreProcessPage();
                    _freader.WatchFile(_curPageName);
                }

                SendCurPage();
            }
            catch (Exception e)
            {
                Logger.Error($"Exception on switch page to {name}: {e.Message}");
            }
        }

        public void SwitchPageContent(string content)
        {
            try
            {
                content = "@" + content;
                _curPageConent = content;
                PreProcessPage();
                SendCurPage();
                _freader.UnWatch();
            }
            catch (Exception e)
            {
                Logger.Error($"Exception on set page content: {e.Message}");
            }
        }

        public void SetPageContent(string name, string content)
        {
            _inMemoryPages[name] = content;
        }

        public long LastRecvTime { get { return _lastRecvTime; } }

        private void OnFileChanged(object source, int e)
        {
            Logger.Info("PageFile Changed.");
            _curPageConent = "@" + ReadPageText(_curPageName);
            PreProcessPage();
            SendCurPage();
        }

        private string ReadPageText(string name)
        {
            string orgtext = _freader.ReadAllText(name);
            int index = orgtext.IndexOf("@inc(");
            while(index >= 0)
            {
                int endpos = orgtext.IndexOf(')', index + 5);
                string incname = orgtext.Substring(index + 5, endpos - index - 5);
                orgtext = orgtext.Replace("@inc(" + incname + ")", _freader.ReadAllText(incname + ".uipage"));
                index = orgtext.IndexOf("@inc(");
            }

            return orgtext;
        }

        private void OnReciveClientCommand(string cmd, string peername)
        {
            Logger.Info($"收到客户端 {peername} 命令: {cmd}");

            _lastRecvTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            _status.OnRecvCommand(cmd, (string arg) => _server.SendCommand(arg));

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

            OnReciveCommandAdapter?.Invoke(cmd);
        }

        private void HandlePageCtrlCommand(string cmd, string peername)
        {
            CmdLine command = new CmdLine(cmd);
            bool? handled = OnRecivePageCtrlCommand?.Invoke(command);

            if(handled != null && handled.Value)
            {
                return;
            }

            switch (command.cmd)
            {
                case "$GCP":
                    SendCurPage();
                    break;

                case "$GP":
                    if(command.getarg<string>(0, out string name))
                    {
                        SwitchPage(name);
                    }
                    break;

                case "$GSP":
                    if(command.getarg<string>(0, out string sname))
                    {
                        SendSubPage(sname);
                    }
                    break;

                case "$RES":
                    if(command.getarg<string>(0, out string filename))
                    {
                        HandleFetchResourceCommand(filename, peername);
                    }
                    break;

                case "$GCF":
                    SendPadConfig(); 
                    break;

                case "$LANG":
                    if(command.getarg<string>(0, out string lang))
                    {
                        if (string.IsNullOrWhiteSpace(lang))
                            break;

                        lang = lang.Trim();
                        if(_curlanguage != lang)
                        {
                            _curlanguage = lang;
                        }
                        OnLanguageChanged?.Invoke(_curlanguage);
                    }
                    break;

                default:
                    Logger.Error($"未知页面控制命令: { command.cmd }");
                    break;
            }

            OnReciveAfterPageCtrlCommand?.Invoke(command.cmd);
        }

        private void PreProcessPage()
        {
        }

        private void SendCurPage()
        {
            SendCommand(_curPageConent.Replace("<$LANG$>", _curlanguage));
        }

        private void SendPadConfig()
        {
            try
            {
                string cfgstr = _freader.ReadAllText("pad.ini");
                _server.SendCommand("*" + cfgstr);
            }
            catch(Exception e)
            {
                _server.SendCommand("*");
                Logger.Warning($"{e.Message}");
            }
        }

        private void SendSubPage(string name)
        {
            try
            {
                string text = ReadPageText(name);
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                _server.SendCommand("?" + name + "@" + text);
            }
            catch(Exception e)
            {
                Logger.Error($"{e.Message}");
            }
        }

        protected virtual void HandleServerCtrlCommand(string cmd, string clientname)
        {
            if(OnReciveServerCtrlCommand != null)
            {
                OnReciveServerCtrlCommand?.Invoke(cmd, clientname);
                return;
            }

            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "#shutdown":
                    Machine.shutdown();
                    break;

                default:
                    Logger.Error($"未知服务控制命令: { command.cmd }");
                    break;
            }
        }

        protected virtual void HandleFetchResourceCommand(string filename, string clientname) => 
            _server.SendDataTo(filename, _freader.ReadAllBytes(filename), clientname);


        private Action<string> OnReciveCommandAdapter = null;
        private Action<string> OnLanguageChanged = null;
        private Action<string, string> OnReciveServerCtrlCommand = null;
        private Func<CmdLine, bool> OnRecivePageCtrlCommand = null;
        private Action<string> OnReciveAfterPageCtrlCommand = null;

        private Action OnPageSended = null;
        private CmdTcpServer _server = new CmdTcpServer();

        private string _curPageName = "";
        private string _curPageConent = "";

        private Dictionary<string, string> _inMemoryPages = new Dictionary<string, string>();

        private IFileReader _freader;
        private PageReportClient _reporter;
        private UIStateManager _status = new UIStateManager();
        private long _lastRecvTime = 0;
        private string _curlanguage = "cn";
    }
}
