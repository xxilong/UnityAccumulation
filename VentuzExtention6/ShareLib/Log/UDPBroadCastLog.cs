using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ShareLib.Log
{
    public class UDPBroadCastLog : BaseLogger
    {
        private UdpClient client = new UdpClient();
        private IPEndPoint targetAddr = new IPEndPoint(IPAddress.Broadcast, 3199);

        public override void LogMessage(LogLevel level, string msg)
        {
            MemoryStream stream = new MemoryStream();
            byte[] msgbytes = Encoding.UTF8.GetBytes(msg);
            stream.WriteByte((byte)'D');
            stream.WriteByte((byte)'L');
            stream.WriteByte((byte)'G');
            stream.WriteByte((byte)level);
            stream.Write(msgbytes, 0, msgbytes.Length);
            msgbytes = stream.ToArray();
            client.Send(msgbytes, msgbytes.Length, targetAddr);
        }
    }
}
