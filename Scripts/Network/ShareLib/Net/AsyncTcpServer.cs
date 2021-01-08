﻿using ShareLib.Log;
using ShareLib.Protocl;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace ShareLib.Net
{
    public class AsyncTcpServer<T>
        where T: IPackProtocl, new()
    {
        public bool Start(int port)
        {
            if(_tcpListener != null)
            {
                return true;
            }

            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptSocket (DoAcceptTcpClientCallback, null);  
            Logger.Info(string.Format("开始侦听{0}端口", port));
            return true;
        }

        public void Stop()
        {
            _tcpListener.Stop();

            lock(_listlocker)
            {
                foreach(var info in _tcpClients)
                {
                    info._sock.Close();
                }
            }

            _tcpListener = null;
        }

        public void SendRawData(byte[] bytes)
        {
            lock(_listlocker)
            {
                if(_tcpClients.Count == 0)
                {
                    return;
                }

                foreach(var info in _tcpClients)
                {
                    info._sock.Send(bytes);
                }
            }
        }

        public void SendRawDataTo(byte[] bytes, string clientName)
        {
            lock(_listlocker)
            {
                foreach(var info in _tcpClients)
                {
                    if(info._recver.PeerName == clientName)
                    {
                        info._sock.Send(bytes);
                    }
                }
            }
        }

        public void SendRawDataToOther(byte[] bytes, string clientName)
        {
            Logger.Debug("forward :"+_tcpClients.Count);
            lock (_listlocker)
            {
                foreach(var info in _tcpClients)
                {
                    if(info._recver.PeerName != clientName)
                    {
                        Logger.Debug(string.Format("send to {0}/{1} : + {2}", info._recver.PeerName, _tcpClients.Count, bytes));
                        info._sock.Send(bytes);
                    }
                }
            }
        }

        public int ClientCount
        {
            get
            {
                return _tcpClients.Count;
            }
        }

        protected virtual void OnNewConnect(T prot)
        {
        }

        protected virtual void OnClosedConnect(IPackProtocl prot)
        {
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            Socket client = null;
            try
            {
                client = _tcpListener.EndAcceptSocket(ar);
            }
            catch(Exception e)
            {
                Logger.Error("Exception On Accept Client: "+e);
                client = null;
            }

            try
            {
                _tcpListener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, null);
            }
            catch(Exception e)
            {
                // 监听端口异常直接退出

                Logger.Error("Exception On Lissen Socket: "+e);
                Stop();
                return;
            }

            if(client == null)
            {
                return;
            }

            try
            {
                string clientName = client.RemoteEndPoint.ToString();
                T r = new T();
                r.PeerName = clientName;
                OnNewConnect(r);
                ClientInfo info = new ClientInfo();
                info._sock = client;
                info._recver = r; 

                lock (_listlocker)
                {
                    _tcpClients.AddLast(info);
                }
                Logger.Info(string.Format("客户端 {0} 连接",clientName));

                info._sock.BeginReceive(info._buff, 0, info._buff.Length, SocketFlags.None, OnClientRecv, info);
            }
            catch(Exception e)
            {
                Logger.Error("Exception "+e);
            }
        }

        private void OnClientRecv(IAsyncResult ar)
        {
            ClientInfo info = (ClientInfo)ar.AsyncState;

            try
            {
                int count = info._sock.EndReceive(ar);
                if (count <= 0)
                {
                    Logger.Info(string.Format("客户端 {0} 断开", info._sock.RemoteEndPoint));
                    CloseClient(info);
                    return;
                }

                info._recver.OnRead(info._buff, count);
                info._sock.BeginReceive(info._buff, 0, info._buff.Length, SocketFlags.None, OnClientRecv, info);
            }
            catch (Exception e)
            {
                Logger.Error(string.Format("Client {0}: {1}", info._sock.RemoteEndPoint, e.Message));
                CloseClient(info);
            }
        }

        private void CloseClient(ClientInfo info)
        {
            info._sock.Close();

            lock (_listlocker)
            {
                _tcpClients.Remove(info);
            }

            OnClosedConnect(info._recver);
        }

        private class ClientInfo
        {
            public Socket _sock;
            public byte[] _buff = new byte[1024];
            public IPackProtocl _recver;
        }

        private TcpListener _tcpListener;
        private LinkedList<ClientInfo> _tcpClients = new LinkedList<ClientInfo>();
        private object _listlocker = new object();
    }
}