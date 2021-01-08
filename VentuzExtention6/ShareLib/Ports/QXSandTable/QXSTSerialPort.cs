using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Conf;

namespace ShareLib.Ports.QXSandTable
{
    public enum CarType
    {
        Car,            // 私家车
        Truck,          // 货车
        Ambulance,      // 救护车
        Patrol,         // 巡检车
        BridgeTesting,  // 桥梁测试车
        Elevator,       // 电梯
        ComSupport,     // 通信保障车
        UAV,            // 无人机
        Robot,          // 机器人
        PoliceCar,      // 警车
    }

    public class QXSTSerialPort : SerialPort
    {
        public QXSTSerialPort() :
           base(new QXSTSerialProtocol2())
        {
            _procol = (QXSTSerialProtocol)base.Protocol;
        }

        public QXSTSerialPort(QXSTSerialProtocol prol) :
            base(prol)
        {
            _procol = prol;
        }

        public new void Open(string cfgdomain = "COM")
        {
            _procol.MontherBoardId = GlobalConf.getconf<int>(cfgdomain, "DefaultMotherBoardId", 0);
            base.Open(cfgdomain);
        }

        #region 通用发包
        public void SendOn(CarType t, Action<QXSTSerialPort> act)
        {
            if(t == _CurSelectCarType)
            {
                act(this);
                return;
            }

            CarType oldType = _CurSelectCarType;
            SwitchCar(t);
            act(this);
            SwitchCar(oldType);
        }

        public void SendOn(int motherBoard, Action<QXSTSerialPort> act)
        {
            if(motherBoard == _procol.MontherBoardId)
            {
                act(this);
                return;
            }

            int oldBoard = _procol.MontherBoardId;
            _procol.MontherBoardId = motherBoard;
            act(this);
            _procol.MontherBoardId = oldBoard;
            return;
        }

        public void SendOn(int motherBoard, CarType t, Action<QXSTSerialPort> act)
            => SendOn(motherBoard, p => p.SendOn(t, act));

        // 调用方法: port.SendWith(p=>p.SetCarLight(10));      
        public void SendWith(Func<QXSTSerialProtocol, byte[]> act)
        {
            Write(act(_procol));
        }
        #endregion

        #region 常用发包
        public void SendGoto(int area)
        {
            Logger.Info($"发送去区域 {area} 的命令");
            Write(_procol.GotoArea(area));
            Logger.Info($"发送完成");
        }

        public void SendGoPos(int pos)
        {
            Logger.Info($"发送去地点 {pos} 的命令");
            Write(_procol.GotoPlace(pos));
            Logger.Info($"发送完成");
        }

        public void SendGo()
        {
            Logger.Info($"发送前进的命令");
            Write(_procol.Go());
            Logger.Info($"发送完成");
        }

        public void SendStop()
        {
            Logger.Info($"发送停止命令");
            Write(_procol.Stop());
            Logger.Info($"发送完成");
        }

        public void TruckStart() => SendOn(CarType.Truck, p => p.SendGo());
        public void TruckStop() => SendOn(CarType.Truck, p => p.SendStop());
        
        public void SetRobotLight(int light)
        {
            Logger.Info($"发送设置车辆灯光 {light} 的命令");
            Write(_procol.SetCarLight(light));
            Logger.Info($"设置完成");
        }

        public void SetSandLight(int light)
        {
            Logger.Info($"发送设置沙盘灯光 {light} 的命令");
            Write(_procol.SetSandboxLight(light));
            Logger.Info($"设置完成");
        }

        public void SetSandLightMask(int lightmask, int controlmask = -1)
        {
            Logger.Info($"发送设置沙盘灯光 {lightmask} 的命令");
            Write(_procol.SetSandboxLightMask(lightmask, controlmask));
            Logger.Info($"设置完成");
        }

        public void SetOutSignal(int single, int mask)
        {
            Logger.Info($"发送 8 路继电器输出 {single}/{mask} 命令");
            Write(_procol.SetOutSignal((byte)mask, (byte)single));
            Logger.Info($"设置完成");
        }

        public void SetSandLightOff()
        {
            Logger.Info($"发送关闭沙盘灯光的命令");
            Write(_procol.SetSandboxLightOff());
            Logger.Info($"发送完成");
        }

        public void SendRawData(string data)
        {
            Logger.Info($"发送原始数据");
            Write(_procol.MakeRawData(data));
            Logger.Info($"发送完成");
        }

        #endregion

        #region 收包回应
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
        #endregion

        #region 车 ID 管理
        public void SetCurCar(int index) => SelectCar(CarType.Car, index);
        public void SetCurTruck(int index) => SelectCar(CarType.Truck, index);


        public void SelectCar(CarType t, int index) => _SelectCarIndex[t] = index;
        public void SwitchCar(CarType t)
        {
            int index = 0;
            if(_SelectCarIndex.ContainsKey(t))
            {
                index = _SelectCarIndex[t];
            }

            string domain = $"{t}{index}";
            int carid = GlobalConf.getconf<int>(domain, "id", 0);
            Logger.Info($"读取配置当前车ID: {carid} ({domain})");
            _procol.SetCurCar(carid);
            _CurSelectCarType = t;
        }
        #endregion

        private Dictionary<CarType, int> _SelectCarIndex = new Dictionary<CarType, int>();
        private CarType _CurSelectCarType = CarType.Car;

        public QXSTSerialProtocol _procol;
        public static QXSTSerialPort Instance = new QXSTSerialPort();
        public static QXSTSerialPort Instance2 = new QXSTSerialPort();
    }
}
