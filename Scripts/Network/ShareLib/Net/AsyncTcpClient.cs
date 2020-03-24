using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ShareLib.Protocl;
using ShareLib.Log;

namespace ShareLib.Net
{
    public class AsyncTcpClient 
    {
        public void Connect(string ip, int port, IPackProtocl pr)
        {
            Logger.Info(string.Format("Conect{0}:{1}", ip, port));
            _ip = ip;
            _port = port;
            _recver = pr;
            _client.BeginConnect(ip, port, DoConnected, null);
        }

        public void ReConnect()
        {
            Close();
            Connect(_ip, _port, _recver);
        }

        public bool IsConnected()
        {
            return _client.Connected;
        }

        public void RawSend(byte[] data)
        {
            if(!_client.Connected)
            {
                return;
            }

            try
            {
                _client.Send(data);
            }
            catch(Exception e)
            {
                Logger.Error(string.Format("Client Send Except: {0}", e));

                Close();
                OnConnectResult(false);
                return;
            }
        }

        private void DoConnected(IAsyncResult ar)
        {
            try
            {
                _client.EndConnect(ar);
            }
            catch(Exception)
            {
                OnConnectResult(false);
                return;
            }

            OnConnectResult(true);
            _client.BeginReceive(_buff, 0, _buff.Length, SocketFlags.None, OnClientRecv, null);
        }

        private void OnClientRecv(IAsyncResult ar)
        {
            try
            {
                int count = _client.EndReceive(ar);
                if (count <= 0)
                {
                    Close();                    
                    Logger.Info(string.Format("连接断开"));

                    OnConnectResult(false);
                    return;
                }

                _recver.OnRead(_buff, count);
                _client.BeginReceive(_buff, 0, _buff.Length, SocketFlags.None, OnClientRecv, null);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Client Recevie Except: {0}", e));

                Close();
                OnConnectResult(false);
                return;
            }
        }

        public void Close()
        {
            _client.Close();
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        protected virtual void OnConnectResult(bool success)
        {

        }

        private Socket _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private string _ip;
        private int _port;
        private byte[] _buff = new byte[1024];
        private IPackProtocl _recver;
    }
}
