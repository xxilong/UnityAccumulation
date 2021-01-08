using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ShareLib.Log;

namespace ShareLib.Net
{
    /// <summary>
    /// 每个客户端连接创建一个线程进行处理的服务器模型
    /// </summary>
    public abstract class ThreadModeTCPServer
    {
        public bool Start(int port, string allowIPS)
        {
            if(!string.IsNullOrEmpty(allowIPS))
            {
                _allowIPS = allowIPS.Split(',');
            }
            
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, port);
                _tcpListener.Start();
            }
            catch (Exception e)
            {
                Logger.Error($"启动服务器失败: { e.Message }");
                return false;
            }

            new Thread(() =>
            {
                while (true)
                {
                    TcpClient client = null;

                    try
                    {
                        client = _tcpListener.AcceptTcpClient();
                    }
                    catch(Exception e)
                    {
                        Logger.Error($"接收连接异常 { e.Message }");
                        break;
                    }
                        
                    string clientName = client.Client.RemoteEndPoint.ToString();                        
                    if (!IsSafeIP(clientName))
                    {
                        Logger.Warning($"未知客户端 { clientName } 尝试连接");
                        client.Close();
                        continue;
                    }

                    Logger.Info($"客户端 { clientName } 连接");

                    new Thread(() =>
                    {
                        var stream = client.GetStream();
                        try
                        {
                            HandleClient(stream);
                        }
                        catch (Exception e)
                        {
                            Logger.Error($"处理客户端请求异常 { e.Message }");
                        }
                        finally
                        {
                            stream.Close();
                            client.Close();
                        }

                        Logger.Info($"客户端 { clientName } 退出");

                    }).Start();
                }
            }).Start();

            Logger.Info($"开始侦听 { port } 端口");
            return true;
        }
        public void Stop() => _tcpListener.Stop();
        protected abstract void HandleClient(NetworkStream stream);

        private bool IsSafeIP(string clientName)
        {
            if(_allowIPS == null)
            {
                return true;
            }

            string clientIP = clientName.Split(':')[0];

            foreach (string aip in _allowIPS)
            {
                if (clientIP == aip)
                {
                    return true;
                }
            }

            return false;
        }

        private TcpListener _tcpListener;
        private string[] _allowIPS;
    }
}
