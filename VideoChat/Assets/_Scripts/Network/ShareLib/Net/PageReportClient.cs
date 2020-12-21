using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Protocl;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ShareLib.Net
{
    public class PageReportClient : AsyncTcpClient
    {
        public void SetRecvCommandListener(Action<string, string> act)
        {
            _pro.SetReciver((string cmd) =>
            {
                if (cmd.Length > 0 && cmd[0] == '#')
                {
                    act(cmd, _pro.PeerName);
                }
            });
        }

        public void Start(int srvPort)
        {
            string ip = GlobalConf.getconf<string>("upserv", "ip");
            int port = GlobalConf.getconf<int>("upserv", "port", 3131);
            _myPort = srvPort;
            _myName = GlobalConf.getconf<string>("upserv", "regname", "");
            _myName = _myName.Replace(' ', '_');
            _myOrder = GlobalConf.getconf<int>("upserv", "order", 100);

            Connect(ip, port, _pro);
        }

        protected override void OnConnectResult(bool success)
        {
            if(success)
            {
                Delay.Run(200, () =>
                {
                    RawSend(_pro.PackString(String.Format("#reg {0} {1} {2}", _myName, _myPort, _myOrder)));
                });
            }
            else
            {
                Logger.Debug("Connect Upper Server Failed, Try again in 5 seconds...");
                Delay.Run(5000, () =>
                {
                    string ip = GlobalConf.getconf<string>("upserv", "ip");
                    int port = GlobalConf.getconf<int>("upserv", "port", 3131);
                    Connect(ip, port, _pro);
                });
            }
        }

        public void Stop()
        {
            Close();
            if(_delayAct != null)
            {
                _delayAct.Stop();
                _delayAct = null;
            }
        }

        private StringProtocl _pro = new StringProtocl();
        private Timer _delayAct = null;
        private string _myName = "";
        private int _myPort = 0;
        private int _myOrder = 100;
    }
}
