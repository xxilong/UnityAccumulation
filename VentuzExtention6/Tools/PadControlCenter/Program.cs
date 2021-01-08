using Microsoft.Win32;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;

namespace PadControlCenter
{
    class Program
    {
        static void Main(string[] args)
        {           
            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
                new UDPBroadCastLog(),
            });

            string workmode = GlobalConf.getconf("base", "workmode");
            bool enableCom = GlobalConf.getconf<int>("base", "enablecom", 0) != 0;
            bool enableCom2 = GlobalConf.getconf<int>("base", "enablecom2", 0) != 0;

            if(enableCom)
            {
                QXSTSerialPort.Instance.Open();
                QXSTSerialPort.Instance.SwitchCar(CarType.Car);
            }

            if(enableCom2)
            {
                QXSTSerialPort.Instance2.Open("COM2");
                QXSTSerialPort.Instance.SwitchCar(CarType.Car);
            }

            if(workmode == "static")
            {
                StaticPageServer srv = new StaticPageServer();
                srv.SetReciverListener(OnRecvCommand);
                srv.Main();
            }
            else if(workmode == "dynamic")
            {
                DynamicServer srv = new DynamicServer();
                srv.SetReciverListener(OnRecvCommand);
                srv.Main();
            }
            else if(workmode == "forward")
            {
                try
                {
                    ForwardServer_ChengYan srv = new ForwardServer_ChengYan();
                    srv.SetReciverListener(OnRecvCommand);
                    srv.Main();
                }
                catch(Exception e)
                {
                    Logger.Error($"!!Exception: {e}");            
                }
            }
            else if(workmode == "poweronly")
            {
                PowerCtrlServer srv = new PowerCtrlServer();
                srv.Main();
            }
            else if(workmode == "fwd")
            {
                try
                {
                    ForwardServer srv = new ForwardServer();
                    srv.SetReciverListener(OnRecvCommand);
                    srv.Main();
                }
                catch (Exception e)
                {
                    Logger.Error($"!!Exception On Start ForwardServer: {e}");
                }
            }
        }

        static private void OnRecvCommand(string cmd)
        {
            string[] args = cmd.Split(_sepchars, 2);
            if(args[0] == "comsend")
            {
                QXSTSerialPort.Instance.Write(StrToToHexByte(args[1]));
            }            
        }

        static private byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace("0x", "");
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private static readonly char[] _sepchars = { ' ', '\t', '\r', '\n' };
    }
}
