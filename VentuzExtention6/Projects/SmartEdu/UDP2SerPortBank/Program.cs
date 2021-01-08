using ShareLib.Ayz;
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

        static string lastMsg = "";

        static string HandleUDPMessage(string msg)
        {
            if(lastMsg == msg)
            {
                Logger.Debug("Skip Same Command.");
                return null;
            }

            lastMsg = msg;

            Logger.Debug($"Receved UDP Command: {msg}");
            CmdLine cmd = new CmdLine(msg);

            string arg;
            int mask = 0;
            switch(cmd.cmd)
            {
                case "led":
                    cmd.getarg<string>(0, out arg);
                    mask = Convert.ToInt32(arg, 16);
                    Logger.Debug($"Send Set Light On: {mask}");
                    QXSTSerialPort.Instance.SetSandLightMask(mask, 0xfffffff);
                    break;

                case "door":
                    cmd.getarg<string>(0, out arg);
                    mask = Convert.ToInt32(arg, 16);
                    Logger.Debug($"Send Set Door On: {mask}");
                    QXSTSerialPort.Instance.SetSandLightMask(mask != 0 ? 0x40 : 0, 0x40);
                    break;
                    
            }

           
            //if (msg.StartsWith("led") && msg.EndsWith("on"))
            //{
            //    string id = msg.Substring(3, 1);
            //    int maskid = Convert.ToInt32(id, 10);

            //    Logger.Debug($"Send Set Light On: {maskid}");
            //    QXSTSerialPort.Instance.SetSandLightMask(maskid, 0xfffffff);
            //}
            //else if (msg.StartsWith("led") && msg.EndsWith("off"))
            //{
            //    string id = msg.Substring(3, 1);
            //    int maskid = Convert.ToInt32(id, 10);

            //    switch (maskid)
            //    {
                   
            //    }
            //    Logger.Debug($"Send Set Light Of: {maskid}");
            //    // QXSTSerialPort.Instance.SetSandLight(maskid);
            //}
            return null;
        }
    }
}
