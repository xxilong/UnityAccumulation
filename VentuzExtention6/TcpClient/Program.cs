using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace TcpClient
{
    /// <summary>Socket StateObject </summary>
    class SocketInfo
    {
        /// <summary>ClientSocket </summary>
        public Socket _socket = null;
        /// <summary>Size of receive buffer.</summary>
        public const int BufferSize = 1024 * 1024*2;
        /// <summary>Receive buffer.</summary>
        public byte[] _buffer = new byte[BufferSize];
        /// <summary>Received data string.</summary>
        public StringBuilder _sb = new StringBuilder();
    }

    static class Program
    {
        private static byte[] data = new byte[1024];
        static IPEndPoint endPoint;
        internal static SocketInfo info = new SocketInfo();
        static string fileName;
        static string filePath = @"C:\Users\Shawn\Desktop\";
        public static int Main(string[] args)
        {
            ConnectToServer();
            while (true)
            {
                string str = Console.ReadLine();
                if (info._socket.Connected)
                {
                    if (!string.IsNullOrWhiteSpace(str))
                    {
                        //client.Send(Encoding.ASCII.GetBytes(str));
                        byte[] strData = Encoding.ASCII.GetBytes(str);
                        List<byte> dataList = new List<byte>();
                        dataList.Add(0);
                        dataList.AddRange(strData);
                        info._socket.BeginSend(dataList.ToArray(),0,dataList.Count, SocketFlags.None, SendCallBack, info._socket);
                    }
                }
                else
                {
                    Console.WriteLine("Disconnected");
                    info._socket.BeginConnect(endPoint, OnConnectCallBack, info._socket);
                }
            }
        }

        private static void ConnectToServer()
        {
            //新建客户端Socket
            info._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //连接服务端IP和端口
            IPAddress ip = IPAddress.Parse("192.168.0.132");
            Console.WriteLine($"开始连接服务器{ip}");
            endPoint = new IPEndPoint(ip, 3131);
            info._socket.BeginConnect(endPoint, OnConnectCallBack, info._socket);
        }
        static void OnConnectCallBack(IAsyncResult ar)
        {
            try
            {
                Socket socket = ar.AsyncState as Socket;
                socket.EndConnect(ar);
                Console.WriteLine("Connected sucess");
                socket.BeginReceive(info._buffer, 0, SocketInfo.BufferSize, 0, ReceiveCallBack, socket);                
            }
            catch (Exception e)
            {
                Console.Write(e);
                Console.WriteLine();
                info._socket.BeginConnect(endPoint, OnConnectCallBack, info._socket);
            }
        }

        static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int count = info._socket.EndReceive(ar);
                if (count == 0)
                {
                    if (info._socket.Connected)
                    {
                        info._socket.BeginConnect(endPoint, OnConnectCallBack, info._socket);
                    }
                    info._socket.Close();
                    Console.WriteLine("客户端关闭");
                    //return;
                }
                else if (info._buffer[0] == 0)
                {
                    info._sb.Append(Encoding.UTF8.GetString(info._buffer, 1, count - 1));
                    string str = info._sb.ToString();
                    Console.WriteLine(string.Format("收到客户端{0}消息：{1}", info._socket.RemoteEndPoint, str));
                    //"sendfile <path>"
                    if (str.StartsWith("sendfile"))
                    {
                        fileName = str.Split(' ')[1];
                    }
                    else
                    {
                        fileName = null;
                    }
                }
                else if (info._buffer[0] == 1)
                {                    
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        string name = Path.GetFileName(fileName);
                        name = Path.Combine(filePath, name);
                        
                        using (FileStream writer = new FileStream(name, FileMode.Append, FileAccess.Write))
                        {                            
                            //writer.Write(info._buffer,1, count - 1);
                            writer.Write(info._buffer, 1, count-1);
                            Console.WriteLine("保存文件：" + name);
                            //File.WriteAllBytes(path, data);
                        }
                    }
                }
                info._sb.Clear();
                info._socket.BeginReceive(info._buffer,
                    0, SocketInfo.BufferSize, 0, ReceiveCallBack, info);
            }
            catch (SocketException e)
            {
                Console.WriteLine("接收数据失败：" + e);
            }
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteFile(string path, byte[] data)
        {

            using (FileStream writer = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                writer.Write(data, 0, data.Length);
                Console.WriteLine("保存文件：" + path);
                //File.WriteAllBytes(path, data);
            }
        }
    }    
}
