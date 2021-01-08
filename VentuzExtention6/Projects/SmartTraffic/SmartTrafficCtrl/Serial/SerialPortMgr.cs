using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Ports;
using ShareLib.Log;
using ShareLib.Conf;

namespace SmartTrafficCtrl.Serial
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
            Write(_procol.GotoArea(_curCarId, area));
            Logger.Info($"发送完成");
        }

        public void SendGo()
        {
            Logger.Info($"发送继续前进的命令");
            Write(_procol.Go(_curCarId));
            Logger.Info($"发送完成");
        }

        public void TruckStart()
        {
            Logger.Info($"发送货车启动命令");
            Write(_procol.Go(_curTruckId));
            Logger.Info($"发送完成");
        }

        public void TruckStop()
        {
            Logger.Info($"发送货车停止命令");
            Write(_procol.Stop(_curTruckId));
            Logger.Info($"发送完成");
        }

        public void SetLight(int light)
        {
            Logger.Info($"发送设置灯光 {light} 的命令");
            Write(_procol.SetLight(_curCarId, light));
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

        public void SetCurCar(int index)
        {
            string domain = "car" + index.ToString();
            _curCarId = GlobalConf.getconf<int>(domain, "id");
        }

        public void SetCurTruck(int index)
        {
            string domain = "truck" + index.ToString();
            _curTruckId = GlobalConf.getconf<int>(domain, "id");
        }

        private int _curCarId = 0;
        private int _curTruckId = 0;
        public RobotProtocl _procol;

        public static SerialPortMgr Instance = new SerialPortMgr();
    }
}
