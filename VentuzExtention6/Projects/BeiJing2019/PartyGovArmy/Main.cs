using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Page;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;
using Ventuz.Extention.Control;

namespace PartyGovArmy
{
    public class Main
    {
        static public Main instance = new Main();

        public void StartUp(string pageTree)
        {
            ControlServer.Instance.Start(3131);
            QXSTSerialPort.Instance.Open();

            QXSTSerialPort.Instance._procol.OnArrivedArea((int pos) => 
            {  
                switch(pos)
                {
                    case 0: // 无人机到达 A 点
                        break;

                    case 1: // 无人机到达 B 点
                        break;

                    case 3: // 机器人达到逃犯位置
                        break;

                    case 4: // 机器人到达初始位置
                        break;

                    default:
                        break;
                }
            });

            Page.Control.InitPages(pageTree);
            Page.Control.GotoPage("S0");

            Page.Control.MProperty("fac", "true", () =>{
                Logger.Debug("工厂亮灯");
                QXSTSerialPort.Instance.SetSandLightMask(16, 16);
                Logger.Debug("工厂烟雾开");
                QXSTSerialPort.Instance.SetSandLightMask(262144, 262144);
            }, ()=> {
                Logger.Debug("工厂灭灯");
                QXSTSerialPort.Instance.SetSandLightMask(0, 16);
                Logger.Debug("工厂烟雾灭");
                QXSTSerialPort.Instance.SetSandLightMask(0, 262144);
            });

            Page.Control.MProperty("water", "true", () =>
            {
                Logger.Debug("水面亮灯");
                QXSTSerialPort.Instance.SetSandLightMask(1048576, 1572864);
            }, () =>
            {
                Logger.Debug("水面灭灯");
                QXSTSerialPort.Instance.SetSandLightMask(524288, 1572864);
            });

            Page.Control.MProperty("station", "true", () =>
            {
                Logger.Debug("基站亮灯");
                QXSTSerialPort.Instance.SendOn(CarType.UAV, p => p.SendGoto(8));
            }, () =>
            {
                Logger.Debug("基站灭灯");
                QXSTSerialPort.Instance.SendOn(CarType.UAV, p => p.SendGoto(9));
            });

            Page.Control.MProperty("gym", "true", () =>
            {
                Logger.Debug("体育场灯亮");
                QXSTSerialPort.Instance.SetSandLightMask(64, 64);
            }, () =>
            {
                Logger.Debug("体育场灯灭");
                QXSTSerialPort.Instance.SetSandLightMask(0, 64);
            });

            Page.Control.MProperty("uav", "0", () =>
            {
                Logger.Debug("无人机去 A");
                QXSTSerialPort.Instance.SendOn(CarType.UAV, p => p.SendGoto(0));
            }, () =>
            {
                Logger.Debug("无人机去 B");
                QXSTSerialPort.Instance.SendOn(CarType.UAV, p => p.SendGoto(1));
            });

            Page.Control.MProperty("car10", "run", () =>
            {
                Logger.Debug("10号车开始巡检");
                QXSTSerialPort.Instance.SendOn(CarType.Patrol, p => p.SendGo());
            }, () =>
            {
                Logger.Debug("10号车停止巡检");
                QXSTSerialPort.Instance.SendOn(CarType.Patrol, p => p.SendStop());
            });

            Page.Control.MProperty("car11", "run", () =>
            {
                Logger.Debug("11号车前往逃犯处");
                QXSTSerialPort.Instance.SendOn(CarType.Robot, p => p.SendGoto(3));
            }, () =>
            {
                Logger.Debug("11号车回初始位置");
                QXSTSerialPort.Instance.SendOn(CarType.Robot, p => p.SendGoto(4));
            });

            Page.Control.MProperty("car12", "run", () =>
            {
                Logger.Debug("12号车开始运行一周");
                QXSTSerialPort.Instance.SendOn(CarType.PoliceCar, p => p.SendGoto(5));
            });

            int delaytime = GlobalConf.getconf<int>("Control", "TimeOfS6", 5000);
            Page.Control.MPage("S5", () => {
                Logger.Debug($"进入场景5, {delaytime}毫秒后自动前往场景6");
                Delay.Run(delaytime, () =>
                {
                    Logger.Debug("前往场景6");
                    Page.Control.GotoPage("S6");
                });
            });
        }
    }
}
