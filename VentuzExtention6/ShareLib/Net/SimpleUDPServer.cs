using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ShareLib.Log;

namespace ShareLib.Net
{
    public class SimpleUDPServer
    {
        public void StartLoop(int port, Encoding msgEncode = null)
        {
            IPEndPoint serverIP = new IPEndPoint(0, port);

            if(msgEncode == null)
            {
                msgEncode = Encoding.Default;
            }

            _encode = msgEncode;
            _udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpServer.Bind(serverIP);
            _encode = msgEncode;
            Logger.Info("UDP服务器开始监听" + serverIP.Port + "端口");

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)ipep;

            byte[] data = new byte[40960];
            int length = 0;

            while (true)
            {
                try
                {
                    length = _udpServer.ReceiveFrom(data, ref Remote);//接受来自服务器的数据
                }
                catch (Exception ex)
                {
                    Logger.Error($"出现异常：{ ex.Message }, 1 分钟后重启");
                    _udpServer.Close();
                    Thread.Sleep(1000 * 60);
                    _udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    _udpServer.Bind(serverIP);
                    Logger.Info("UDP服务器开始监听" + serverIP.Port + "端口");
                    continue;
                }

                string message = msgEncode.GetString(data, 0, length);
                //string ipport = (Remote as IPEndPoint).Address.ToString() + ":" + (Remote as IPEndPoint).Port.ToString();
                //Logger.Info($"来自 { ipport } 的消息: { message }");

                try
                {
                    OnRecvMessage(ipep, message);   
                }
                catch(Exception ex)
                {
                    Logger.Error($"处理消息时出现异常: { ex.Message }");
                }               
            }

            //udpServer.Close();
        }

        protected virtual void OnRecvMessage(IPEndPoint addr, string message)
        {
            string ret = MessageHander?.Invoke(message);
            if(!string.IsNullOrEmpty(ret))
            {
                _udpServer.SendTo(_encode.GetBytes(ret), addr);
            }
        }

        public Func<string, string> MessageHander;
        private Socket _udpServer = null;
        private Encoding _encode = null;
    }
}
