using ShareLib.Protocl;
using System;
using UnityEngine;

namespace ShareLib.Net
{
    class CmdControlClient : AsyncTcpClient
    {
        public CmdControlClient()
        {
        }

        public void SetReciver(Action<string> r)
        {
            _pro.SetReciver(r);
        }
        
        public void Connect(string ip, int port)
        {
            Debug.LogFormat("Conect{0}:{1}", ip, port);
            if(string.IsNullOrEmpty(ip))
            {
                return;
            }

            Connect(ip, port, _pro);
        }

        public void SendCommand(string str)
        {
            Debug.Log(string.Format("SendCommand: {0}", str));
            RawSend(_pro.PackString(str));
        }

        protected override void OnConnectResult(bool success)
        {
            ConnectStatus(success);
        }

        public void SetConnectStatusListener(Action<bool> act)
        {
            ConnectStatus = act;
        }

        private StringProtocl _pro = new StringProtocl();
        private Action<bool> ConnectStatus;
    }
}
