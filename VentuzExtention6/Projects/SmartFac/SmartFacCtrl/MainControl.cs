using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports;
using ShareLib.Ayz;
using ShareLib.Unity;
using System;
using System.IO;
using SmartFacCtrl.Serial;

namespace SmartFacCtrl
{
    public class MainControl
    {
        public static MainControl Instance = new MainControl();

        public void StartUp()   // 程序启动
        {
            GlobalConf.Set(Path.ChangeExtension(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".ini"));

            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
            });

            // 打开串口
            SerialPortMgr.Instance.Open("COM");
            SerialPortMgr.Instance._procol.OnArrivedArea(DoArrivedArea);
            SerialPortMgr.Instance._procol.OnSetBattery(DoRecivedBattery);

            // 启动服务器
            _server.SetReciverListener(OnServerCmd);
            _server.SetCtrlChangedListener(OnControlClientChange);
            _server.Start(5871);

            property.SetVideoURL();
            property.FireChanged();
        }

        public void CloseUp()
        {
            SerialPortMgr.Instance.Close();
            _server.Stop();
        }

        public void OnServerCmd(string cmd, string peername)
        {
            Logger.Info($"收到 Pad 命令: {cmd}");

            CmdLine command = new CmdLine(cmd);
            switch(command.cmd)
            {
                case "set_robot":
                    if(command.getarg<int>(0, out int robotIndex))
                    {
                        Logger.Info($"设置机器人编号: {robotIndex}");
                        SerialPortMgr.Instance.SetCurRobot(robotIndex);
                        property.SetRobot(robotIndex);
                    }
                    break;

                case "set_auto":
                    if (command.getarg<int>(0, out int autoMode))
                    {
                        Logger.Info($"设置自动模式: {autoMode}");
                        property.SetAutoMode(autoMode != 0);
                    }
                    break;

                case "go_home":
                    Logger.Info("返回首页");
                    property.GotoHome();
                    break;

                case "go_area":
                    if(command.getarg<int>(0, out int areaIndex))
                    {
                        Logger.Info($"巡检区域: {areaIndex}");
                        SerialPortMgr.Instance.SendGoto(areaIndex);
                        property.GotoArea(areaIndex);
                    }
                    break;

                case "handle_area":
                    if (command.getarg<int>(0, out int areaIndex2))
                    {
                        Logger.Info($"已处理区域: {areaIndex2}");
                        property.HandledArea(areaIndex2);
                    }
                    break;

                default:
                    Logger.Error($"未知命令: { command.cmd }");
                    break;
            }
        }

        public void DoArrivedArea(int area)
        {
            Logger.Info($"到达区域: {area}");
            _server.SendCommand($"arrive {area}");
            property.ArrviedArea(area);
        }

        public void DoRecivedBattery(int battery)
        {
            Logger.Info($"电池电量: {battery}%");
            property.SetBattery(battery);
        }

        public void OnControlClientChange(bool hasCtrl)
        {
            Logger.Info($"控制端连接: {hasCtrl}");
            property.SetControlStatus(hasCtrl);
        }

        private CmdTcpServer _server = new CmdTcpServer();
        public string localIP = NetUnity.GetLocalIPs();
        public VentuzProperty property = new VentuzProperty();
    }
}
