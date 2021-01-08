using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using ShareLib.Log;

namespace ShareLib.Net
{
    public class SimpleUDPClient
    {
        public SimpleUDPClient(string ip, int port)
        {
            _client = new UdpClient(ip, port);
        }

        public void SendCommand(string cmd)
        {
            if(string.IsNullOrWhiteSpace(cmd))
            {
                return;
            }

            Logger.Info($"发送 UDP 命令: {cmd}");
            byte[] data = Encoding.UTF8.GetBytes(cmd);
            _client.Send(data, data.Length);
        }

        private UdpClient _client = null;
    }
}
