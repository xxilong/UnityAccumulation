using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Ports;
using ShareLib.Log;
using ShareLib.Conf;

namespace SmartFacCtrl.Serial
{
    public class SerialPortMgr : SerialPort
    {
        SerialPortMgr() :
            base(new RobotProtocl())
        {
            _procol = (RobotProtocl)base.Protocol;
        }

        public void SendGoto(int area)
        {
            Logger.Info($"发送去区域 {area} 的命令");
            Write(_procol.GotoArea(_curRobot, area));
            Logger.Info($"发送完成");
        }

        public void SetLight(int light)
        {
            Logger.Info($"发送设置灯光 {light} 的命令");
            Write(_procol.SetLight(_curRobot, light));
            Logger.Info($"设置完成");
        }

        public void ResAvoid(int robot)
        {
            Write(_procol.ResAvoidInfo(robot));
        }

        public void ResInput(int board)
        {
            Write(_procol.ResInputInfo(board));
        }

        public void ResBatteryInfo(int robot)
        {
            Write(_procol.ResBatteryInfo(robot));
        }

        public void SetCurRobot(int robot)
        {
            string domain = "robot" + robot.ToString();
            _curRobot = GlobalConf.getconf<int>(domain, "id");
        }

        private int _curRobot = 0;
        public RobotProtocl _procol;

        public static SerialPortMgr Instance = new SerialPortMgr();
    }
}
