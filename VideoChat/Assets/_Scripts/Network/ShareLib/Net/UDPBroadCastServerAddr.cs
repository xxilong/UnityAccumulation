using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ShareLib.Net
{
    public class UDPBroadCastServerAddr
    {
        public void Start(int port)
        {
            _udpPort = port;
            _workthread = new Thread(NetworkThread);
            _workthread.Start();
        }

        public void Stop()
        {
            _isexit = true;
            if(_workthread != null)
            {
                _workthread.Join();
            }
        }

        private void NetworkThread()
        {
            using (UdpClient udp = new UdpClient())
            {
                string ipaddr = LocalIPAddress();
                Logger.Info("Get First Local Address: "+ipaddr);
                byte[] localIP = Encoding.Unicode.GetBytes(ipaddr.ToString());

                IPEndPoint broadAddr = new IPEndPoint(IPAddress.Broadcast, _udpPort);

                while (!_isexit)
                {
                    udp.Send(localIP, localIP.Length, broadAddr);
                    Thread.Sleep(1000);
                }
            }
        }

        private string LocalIPAddress()
        {
            IPHostEntry host;
            string localIP = "0.0.0.0";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private volatile bool _isexit = false;
        private Thread _workthread = null;
        private int _udpPort = 1000;
    }
}
