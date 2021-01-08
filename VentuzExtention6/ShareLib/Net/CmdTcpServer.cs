using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Log;
using ShareLib.Protocl;

namespace ShareLib.Net
{
    public class CmdTcpServer : AsyncTcpServer<StringProtocl>
    {
        public void SendCommand(string cmd)
        {
            Logger.Info($"发送客户端命令: {cmd}");
            SendRawData(_procol.PackString(cmd));
        }

        public void SendCommandTo(string cmd, string client)
        {
            Logger.Info($"SendCommand {cmd} To: {client}");
            SendRawDataTo(_procol.PackString(cmd), client);
        }

        public void SendDataTo(string resname, byte[] data, string client)
        {
            SendRawDataTo(_datapro.PackResData(resname, data), client);
        }

        public void SendCommandToOther(string cmd, string client)
        {
            Logger.Info(string.Format("SendCommand {0} ToOther: {1}",cmd,client));
            SendRawDataToOther(_procol.PackString(cmd), client);
        }

        protected override void OnNewConnect(StringProtocl protocl)
        {
            protocl.SetReciver((string message)=> {
                OnRecive(protocl, message);
            });

            if(ClientCount == 0)
            {
                OnClientChange?.Invoke(true);
            }
            OnNewConnected?.Invoke(protocl.PeerName);
        }

        protected override void OnClosedConnect(IPackProtocl prot)
        {
            OnClientClose?.Invoke(prot.PeerName);
            if(ClientCount == 0)
            {
                OnClientChange?.Invoke(false);
            }
        }

        protected virtual void OnRecive(IPackProtocl prot, string str)
        {
            OnRecvString?.Invoke(str, prot.PeerName);
        }

        public void SetOnNewConnected(Action<string> act) => OnNewConnected = act;
        public void SetReciverListener(Action<string, string> act) => OnRecvString = act;
        public void SetCtrlChangedListener(Action<bool> act) => OnClientChange = act;
        public void SetClientCloseListener(Action<string> act) => OnClientClose = act;

        
        private Action<bool> OnClientChange = null;
        private Action<string> OnClientClose = null;
        private Action<string> OnNewConnected = null;
        private Action<string, string> OnRecvString = null;
        private StringProtocl _procol = new StringProtocl();
        private ResDataProtocl _datapro = new ResDataProtocl();
    }
}
