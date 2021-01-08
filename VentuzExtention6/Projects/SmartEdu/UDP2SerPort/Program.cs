using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports.QXSandTable;
using System;
using System.IO;

namespace UDP2SerPort
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllini = Path.ChangeExtension(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".ini");
            GlobalConf.Set(dllini);

            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
            });

            QXSTSerialPort.Instance.Open();
            SimpleUDPServer udp = new SimpleUDPServer();
            udp.MessageHander = HandleUDPMessage;

            udp.StartLoop(8080);
        }

        // 智慧学习笔
        static private void PenLightOn() => QXSTSerialPort.Instance.SetSandLightMask(1, 0xFF);
        // 棒棒答
        static private void BangBangLightOn() => QXSTSerialPort.Instance.SetSandLightMask(2, 0xFF);
        // 错题本
        static private void ErrorBookLightOn() => QXSTSerialPort.Instance.SetSandLightMask(4, 0xFF);
        // 魔镜系统
        static private void MirrorLightOn() => QXSTSerialPort.Instance.SetSandLightMask(8, 0xFF);
        // 双师课堂
        static private void ClassRoomLightOn() => QXSTSerialPort.Instance.SetSandLightMask(0x30, 0xFF);

        static string HandleUDPMessage(string msg)
        {
            Logger.Debug($"Receved UDP Command: {msg}");
            if(msg.StartsWith("led") && msg.EndsWith("on"))
            {
                string id = msg.Substring(3, 1);
                int maskid = Convert.ToInt32(id, 10);

                switch(maskid)
                {
                    case 2:
                        ClassRoomLightOn();
                        break;

                    case 3:
                        MirrorLightOn();
                        break;

                    case 5:
                        PenLightOn();
                        break;

                    case 6:
                        BangBangLightOn();
                        break;

                    case 7:
                        ErrorBookLightOn();
                        break;

                    default:
                        break;
                }

                Logger.Debug($"Send Set Light Of: {maskid}");
               // QXSTSerialPort.Instance.SetSandLight(maskid);
            }
            return null;
        }
    }
}
