using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Protocl;

namespace ShareLib.Net
{
    public class CmdTcpServerShortLen : AsyncTcpServer<StringProtoclShortLen>
    {
        public void SendCommand(string cmd)
        {
            SendRawData(_procol.PackString(cmd));
        }

        protected override void OnNewConnect(StringProtoclShortLen protocl)
        {
            protocl.SetReciver(OnRecive);
            if (ClientCount == 0 && OnClientChange != null)
            {
                OnClientChange.Invoke(true);
            }
        }

        protected override void OnClosedConnect(IPackProtocl prot)
        {
            if (ClientCount == 0 && OnClientChange != null)
            {
                OnClientChange.Invoke(false);
            }
        }

        protected virtual void OnRecive(string str)
        {
            if (OnRecvString != null)
                OnRecvString.Invoke(str);
        }

        public void SetReciver(Action<string> act)
        {
            OnRecvString = act;
        }

        public void SetCtrlChanged(Action<bool> act)
        {
            OnClientChange = act;
        }

        private Action<bool> OnClientChange;
        private Action<string> OnRecvString;
        private StringProtoclShortLen _procol = new StringProtoclShortLen();
    }
}
