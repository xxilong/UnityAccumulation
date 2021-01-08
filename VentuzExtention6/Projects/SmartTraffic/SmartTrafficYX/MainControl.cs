using System;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports;
using ShareLib.Ayz;
using ShareLib.Unity;
using System.IO;
using System.Timers;
using ShareLib.Ports.QXSandTable;

namespace SmartTrafficYX
{
    public class MainControl
    {
        public static MainControl Instance = new MainControl();

        public void StartUp()   // 程序启动
        {
            string dllini = Path.ChangeExtension(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".ini");
            GlobalConf.Set(dllini);

            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
            });

            // 打开串口
            QXSTSerialPort.Instance.Open();
            QXSTSerialPort.Instance._procol.OnArrivedArea(DoArrivedArea);
            QXSTSerialPort.Instance._procol.OnSetBattery(DoRecivedBattery);
            QXSTSerialPort.Instance._procol.SetOnMeetBlock(DoMeetBlock);

            // 启动服务器
            _server.SetReciverListener(OnServerCmd);
            _server.SetCtrlChangedListener(OnControlClientChange);
            _server.Start(5872);

            blockCheckTimer.Elapsed += BlockCheckTimer;
            blockCheckTimer.AutoReset = true;
            blockCheckTimer.Enabled = true;
            blockCheckTimer.Start();

            property.SetVideoURL();
            property.FireChanged();
        }

        public void CloseUp()
        {
            blockCheckTimer.Stop();
            QXSTSerialPort.Instance.Close();
            _server.Stop();
        }

        public void OnServerCmd(string cmd, string peername)
        {
            Logger.Info($"收到 Pad 命令: {cmd}");

            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "set_car":
                    if (command.getarg<int>(0, out int robotIndex))
                    {
                        Logger.Info($"设置使用的轿车编号: {robotIndex}");
                        QXSTSerialPort.Instance.SetCurCar(robotIndex);
                        property.SetCarIndex(robotIndex);
                    }
                    break;

                case "set_truck":
                    if (command.getarg<int>(0, out int truckIndex))
                    {
                        Logger.Info($"设置使用的货车编号: {truckIndex}");
                        QXSTSerialPort.Instance.SetCurTruck(truckIndex);
                        property.SetTruckIndex(truckIndex);
                    }
                    break;

                case "set_auto":
                    if (command.getarg<int>(0, out int autoMode))
                    {
                        Logger.Info($"设置自动模式: {autoMode}");
                        //property.SetAutoMode(autoMode != 0);
                    }
                    break;

                case "go_home":
                    Logger.Info("返回首页");
                    property.GotoHome();
                    break;

                case "go_area":
                    if (command.getarg<int>(0, out int areaIndex))
                    {
                        Logger.Info($"巡检区域: {areaIndex}");
                        QXSTSerialPort.Instance.SendGoto(areaIndex);
                        property.GotoArea(areaIndex);
                    }
                    break;

                case "set_page":
                    if (command.getarg<int>(0, out int pageIndex))
                    {
                        Logger.Info($"切换页面 {pageIndex}");
                        PageList page = (PageList)pageIndex;
                        Logger.Info($"页面名称: {page}");
                        property.SetPage(page);
                    }
                    break;

                case "go_continue":
                    Logger.Info($"继续行驶");
                    QXSTSerialPort.Instance.SendGo();
                    property.ClearBlock();
                    break;

                case "start_truck":
                    Logger.Info($"货车启动");
                    QXSTSerialPort.Instance.TruckStart();
                    property.SetUseTruck(true);
                    break;

                case "stop_truck":
                    Logger.Info($"货车停止");
                    QXSTSerialPort.Instance.TruckStop();
                    property.SetUseTruck(false);
                    break;

                case "speed_on":
                    Logger.Info("速度盘启动");
                    property.SetSpeed(true);
                    break;

                case "speed_off":
                    Logger.Info("速度盘归零");
                    property.SetSpeed(false);
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
            if (area == 9)
            {
                Logger.Info("红灯停车");
                _server.SendCommand("trigger 2");
                property.SetRedLight(true);
                return;
            }

            if (area == 10)
            {
                Logger.Info("绿灯停车");
                _server.SendCommand("trigger 3");
                property.SetRedLight(false);
                return;
            }

            if (area == 11)
            {
                Logger.Info("十字路口前减速");

                _server.SendCommand("trigger 0");
                property.SetXPath(true);

                lightFrontTime = Environment.TickCount;
                isLightFront = true;
                return;
            }

            Logger.Info($"到达区域: {area}");
            _server.SendCommand($"arrive {area}");
            property.ArrviedArea(area);
        }

        public void DoRecivedBattery(int battery)
        {
            Logger.Info($"电池电量: {battery}%");
            property.SetBattery(battery);
            _server.SendCommand($"set_battery {battery}");
        }

        public void DoMeetBlock()
        {
            blockTime = Environment.TickCount;

            if (!isBlocking)
            {
                isBlocking = true;

                Logger.Info($"遇到障碍");
                property.SetBlock();
                _server.SendCommand("set_block");
            }
        }

        private void BlockCheckTimer(object sender, ElapsedEventArgs e)
        {
            if (isBlocking)
            {
                if (Environment.TickCount - blockTime > 1200)
                {
                    isBlocking = false;

                    Logger.Info($"障碍消失");
                    property.WaitBlock();
                    _server.SendCommand("clear_block");
                }
            }

            if (isLightFront)
            {
                if (Environment.TickCount - lightFrontTime > 2000)
                {
                    isLightFront = false;

                    Logger.Info($"离开十字路口");
                    _server.SendCommand("trigger 1");
                    property.SetXPath(false);
                }
            }
        }

        public void OnControlClientChange(bool hasCtrl)
        {
            Logger.Info($"控制端连接: {hasCtrl}");
            property.SetControlStatus(hasCtrl);
        }

        private CmdTcpServer _server = new CmdTcpServer();
        public string localIP = NetUnity.GetLocalIPs();
        public VentuzProperty property = new VentuzProperty();

        private Timer blockCheckTimer = new Timer(400);
        private volatile bool isBlocking = false;
        private int blockTime = 0;

        private volatile bool isLightFront = false;
        private int lightFrontTime = 0;
    }
}
