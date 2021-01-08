using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace HttpServer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string str = "HTTP/1.1 200 OK\r\nX-Powered-By: Express\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: 234\r\nETag: W/\"ea-ziz374hPenzsNoiC23B8zw\"\r\nset-cookie: connect.sid=s%3At-qlFqhXQi_zbVoLWfyDdZwFZy6_FwIR.TY%2FdYx5NIVqyMeE%2FWJChDSo9kvl%2FZdkzBptWB2luZfg; Path=/; Expires=Tue, 23 Jul 2019 07:18:17 GMT; HttpOnly\r\nDate: Tue, 23 Jul 2019 07:08:17 GMT\r\nConnection: close\r\n\r\n";
            str += "{\"err\":null,\"success\":true,\"data\":{\"hash\":\"WXmkK5bbQIezU6or+LgwQ7U0s3nBVQOsSeX7J0iOTdMWVrm2cRxZCmrHhWZln41p2nhzYC2VG/041Hz32tu3f8QV2ARza/FizWDMKuVBAt+cuo8GPHQ5zQbMAtEpTQdLqVfn1NXHXzAn51AXZFZh4Y9eKQJFN5Co6evPFsqJ078=\",\"version\":\"1.0\"}}";
                       
            int len;
            int port = 80;//端口  
            byte[] buf = new byte[1024];

            //IP是本地127.0.0.1  
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("服务运行在[{0}]端口", port);

            while (true)
            {
                Socket clent = server.AcceptSocket();
                len = clent.Receive(buf);
                Console.WriteLine("收到 [{0}] 数据", len);
                Console.WriteLine(Encoding.ASCII.GetString(buf));
                len = clent.Send(Encoding.ASCII.GetBytes(str));
                Console.WriteLine("发送 [{0}] 数据", len);
                clent.Close();
                clent = null;
            }

        }
    }
}
