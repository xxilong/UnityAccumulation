using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Protocl;

namespace ShareLib.Net
{
    public class CmdTcpServer : AsyncTcpServer<StringProtocl>
    {
        public void SendCommand(string cmd)
        {
            SendRawData(_procol.PackString(cmd));
        }

        public void SendCommandTo(string cmd, string client)
        {
            SendRawDataTo(_procol.PackString(cmd), client);
        }

        public void SendDataTo(string resname, byte[] data, string client)
        {
            SendRawDataTo(_datapro.PackResData(resname, data), client);
        }

        public void SendCommandToOther(string cmd, string client)
        {
            SendRawDataToOther(_procol.PackString(cmd), client);
        }

        protected override void OnNewConnect(StringProtocl protocl)
        {
            protocl.SetReciver((string message)=> {
                OnRecive(protocl, message);
            });

            if(ClientCount == 0&& OnClientChange!=null)
            {
                OnClientChange.Invoke(true);
            }
        }

        protected override void OnClosedConnect(IPackProtocl prot)
        {
            if(OnClientClose!=null) OnClientClose.Invoke(prot.PeerName);

            if (ClientCount == 0 && OnClientChange != null)
            {
                OnClientChange.Invoke(false);
            }
        }

        protected virtual void OnRecive(IPackProtocl prot, string str)
        {
            if(OnRecvString!=null)
                OnRecvString.Invoke(str, prot.PeerName);
        }

        public void SetReciverListener(Action<string, string> act) { OnRecvString = act; }
        public void SetCtrlChangedListener(Action<bool> act) { OnClientChange = act; }
        public void SetClientCloseListener(Action<string> act) { OnClientClose = act; }

        private Action<bool> OnClientChange = null;
        private Action<string> OnClientClose = null;
        private Action<string, string> OnRecvString = null;
        private StringProtocl _procol = new StringProtocl();
        private ResDataProtocl _datapro = new ResDataProtocl();
    }
}
