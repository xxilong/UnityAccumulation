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
            Logger.Info(string.Format("SendCommand:{0}", str));
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
                OnUpdatePage?.Invoke(str);
                return;
            }

            if(str[0] == '+')
            {
                str = str.Substring(1);
                Logger.Info(string.Format("收到页面修改命令: {0}", str));
                CmdLine modcmd = new CmdLine(str);
                OnModifyPage?.Invoke(modcmd);
                return;
            }

            if(str[0] == '!')
            {
                Logger.Info("收到图像数据");
                return;
            }

            Logger.Info(string.Format("收到主机命令: {0}", str));
            CmdLine cmd = new CmdLine(str);
            OnAppCommand?.Invoke(cmd);            
        }

        protected override void OnConnectResult(bool success)
        {
            ConnectStatus?.Invoke(success);
        }

        private StringProtocl _pro = new StringProtocl();
        public Action<bool> ConnectStatus;
        public Action<CmdLine> OnAppCommand;
        public Action<CmdLine> OnModifyPage;
        public Action<string> OnUpdatePage;
    }
}
