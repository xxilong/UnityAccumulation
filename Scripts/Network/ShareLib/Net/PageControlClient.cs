using ShareLib.Protocl;
using System;
using ShareLib.Log;
using ShareLib.Ayz;

namespace ShareLib.Net
{
    public class PageControlClient : AsyncTcpClient
    {
        public PageControlClient()
        {
            _pro.SetReciver(OnRecvCommand);
        }
        
        public void Connect(string ip, int port)
        {
            if(string.IsNullOrEmpty(ip))
            {
                return;
            }

            Connect(ip, port, _pro);
        }

        public void SendCommand(string str)
        {
            Logger.Info(string.Format("SendCommand: {0}", str));
            RawSend(_pro.PackString(str));
        }

        public void OnRecvCommand(string str)
        {
            if(str.Length < 1)
            {
                return;
            }

            if(str[0] == '@')
            {
                str = str.Substring(1);
                Logger.Info("收到页面更新命令.");
                Logger.Debug(str);
                if(OnUpdatePage!=null)
                    OnUpdatePage.Invoke(str);
                return;
            }

            if(str[0] == '+')
            {
                str = str.Substring(1);
                Logger.Info(string.Format("收到页面修改命令: {0}", str));
                CmdLine modcmd = new CmdLine(str);
                if(OnModifyPage!=null)
                    OnModifyPage.Invoke(modcmd);
                return;
            }

            Logger.Info(string.Format("收到主机命令: {0}", str));
            CmdLine cmd = new CmdLine(str);
            if (OnAppCommand!=null)
                OnAppCommand.Invoke(cmd);            
        }

        protected override void OnConnectResult(bool success)
        {
            if (ConnectStatus!=null)
                ConnectStatus.Invoke(success);
        }

        private StringProtocl _pro = new StringProtocl();
        public Action<bool> ConnectStatus;
        public Action<CmdLine> OnAppCommand;
        public Action<CmdLine> OnModifyPage;
        public Action<string> OnUpdatePage;
    }
}
