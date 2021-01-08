using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PadControlCenter;
using Microsoft.Win32;

namespace JinkePadControlCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            //自动获取串口列表并填写配置文件
            List<string> comList = GetComlist(false);
            foreach (var item in comList)
            {
                Console.WriteLine(item);
            }
            if (comList.Count >= 3)
            {
                GlobalConf.setconf("COM2", "name", comList[2]);
            }

            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
                new UDPBroadCastLog(),
            });

            string workmode = GlobalConf.getconf("base", "workmode");
            bool enableCom = GlobalConf.getconf<int>("base", "enablecom", 0) != 0;
            bool enableCom2 = GlobalConf.getconf<int>("base", "enablecom2", 0) != 0;

            if (enableCom)
            {
                QXSTSerialPort.Instance.Open();
                QXSTSerialPort.Instance.SwitchCar(CarType.Car);
            }

            if (enableCom2)
            {
                QXSTSerialPort.Instance2.Open("COM2");
                QXSTSerialPort.Instance.SwitchCar(CarType.Car);
            }

            if (workmode == "static")
            {
                StaticPageServer srv = new StaticPageServer();
                srv.SetReciverListener(OnRecvCommand);
                srv.Main();
            }
            else if (workmode == "dynamic")
            {
                DynamicServer srv = new DynamicServer();
                srv.SetReciverListener(OnRecvCommand);
                srv.Main();
            }
            else if (workmode == "forward")
            {
                try
                {
                    ForwardServer srv = new ForwardServer();
                    srv.SetReciverListener(OnRecvCommand);
                    srv.Main();
                }
                catch (Exception e)
                {
                    Logger.Error($"!!Exception: {e}");
                }
            }
            else if (workmode == "poweronly")
            {
                PowerCtrlServer srv = new PowerCtrlServer();
                srv.Main();
            }
        }

        private static List<string> GetComlist(bool isUseReg)
        {
            Console.WriteLine("GetComlist");
            List<string> list = new List<string>();
            try
            {
                if (isUseReg)
                {
                    RegistryKey RootKey = Registry.LocalMachine;
                    RegistryKey Comkey = RootKey.OpenSubKey(@"HARDWARE\DEVICEMAP\SERIALCOMM");

                    String[] ComNames = Comkey.GetValueNames();

                    foreach (String ComNamekey in ComNames)
                    {
                        string TemS = Comkey.GetValue(ComNamekey).ToString();
                        list.Add(TemS);
                    }
                }
                else
                {
                    foreach (string com in System.IO.Ports.SerialPort.GetPortNames())  //自动获取串行口名称
                        list.Add(com);
                }
            }
            catch
            {
            }
            list.Sort((com1, com2) =>
            {
                int a = int.Parse(com1[com1.Length - 1].ToString());
                int b = int.Parse(com2[com2.Length - 1].ToString());
                return a.CompareTo(b);
            });
            return list;
        }

        static private void OnRecvCommand(string cmd)
        {
            string[] args = cmd.Split(_sepchars, 2);
            if (args[0] == "comsend")
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
