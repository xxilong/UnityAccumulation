using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ShareLib.Log;
using System.Net;
using System.Net.Sockets;
using Microsoft.Win32;

namespace DebugLog
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient udp = new UdpClient(3199);
            ConsoleLogger console = new ConsoleLogger();

            
            
            while(true)
            {
                IPEndPoint addr = null;

                byte[] data = udp.Receive(ref addr);
                if(data.Length < 4)
                {
                    continue;
                }

                if(data[0] == 'D' && data[1] == 'L' && data[2] == 'G')
                {
                    LogLevel level = (LogLevel)data[3];
                    string msg = Encoding.UTF8.GetString(data, 4, data.Length - 4);
                    Logger.Instance.LogMessage(level, msg);
                }
                else if(data[0] == 'D' && data[1] == 'L' && data[2] == 'C')
                {
                    LogLevel level = (LogLevel)data[3];
                    string msg = Encoding.UTF8.GetString(data, 4, data.Length - 4);
                    console.LogWithLevelColor(level, msg);
                }
            }
        }

        
    }
}
