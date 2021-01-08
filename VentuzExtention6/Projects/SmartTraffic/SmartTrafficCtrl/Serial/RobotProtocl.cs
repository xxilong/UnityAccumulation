using ShareLib.Log;
using ShareLib.Protocl;
using System;

namespace SmartTrafficCtrl.Serial
{
    public class RobotProtocl : DeviceProtocl
    {
        #region 发包
        /// <summary>
        /// 0 社区起点
        /// 1 加油站前
        /// 2 加油站
        /// 3 商业中心
        /// 4 停车场
        /// 0 社区起点
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public byte[] GotoArea(int robot, int area)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x10);
            pack.WriteByte((byte)robot);
            pack.WriteHostOrderInt(1 << area);
            return ToArray(pack);
        }

        public byte[] Go(int robot)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0B);
            pack.WriteByte((byte)robot);
            return ToArray(pack);
        }

        public byte[] Stop(int robot)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0C);
            pack.WriteByte((byte)robot);
            return ToArray(pack);
        }

        public byte[] SetLight(int robot, int light)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0D);
            pack.WriteByte((byte)robot);
            pack.WriteByte(0xFF);
            pack.WriteByte((byte)light);
            return ToArray(pack);
        }

        public byte[] ResAvoidInfo(int robot)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0F);
            pack.WriteByte((byte)robot);
            return ToArray(pack);
        }

        public byte[] ResInputInfo(int board)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x08);
            pack.WriteByte((byte)board);
            return ToArray(pack);
        }

        public byte[] ResBatteryInfo(int robot)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x11);
            pack.WriteByte((byte)robot);
            return ToArray(pack);
        }

        private MyMemoryStream MakePackage(byte mainCmd, byte subCmd)
        {
            var stream = new MyMemoryStream();
            stream.WriteByte(mainCmd);
            stream.WriteByte(subCmd);

            return stream;
        }

        #endregion

        #region 收包处理
        protected override void OnRecvPackage(MyMemoryStream stream)
        {
            int maincmd = stream.ReadByte();
            if (maincmd == 0xF8)
            {
                Logger.Info($"收到程序更新指令, 忽略");
                return;
            }

            if (maincmd != 0x10)
            {
                Logger.Error($"收到未知的指令 0x{maincmd.ToString("x")}");
                return;
            }

            int robot = 0;
            int subcmd = stream.ReadByte();

            switch(subcmd)
            {
                case 0x88:
                    int bordid = stream.ReadByte();
                    int mask = stream.ReadHostOrderInt();
                    int status = stream.ReadHostOrderInt();

                    SerialPortMgr.Instance.ResInput(bordid);
                    Logger.Info($"收到输入信息, 已回应");

                    status = mask & status;

                    int area = 0;
                    for(int i = 0; i < 32; ++i)
                    {
                        if((status & 0x1) != 0)
                        {
                            break;
                        }

                        area += 1;
                        status >>= 1;
                    }

                    Logger.Info($"输入状态有效值: 0x{ status.ToString("x") }, 对应区域为: {area}");
                    OnArrivedAreaResult?.Invoke(area);
                    return;

                case 0x8D:
                    stream.Skip(1);
                    int success = stream.ReadByte();
                    Logger.Info($"设置灯光结果 {success}");
                    OnSetLightResult?.Invoke(success == 0);
                    return;

                case 0x8E:
                    Logger.Info($"收到读取灯光回应指令, 已忽略");
                    return;

                case 0x8F:
                    robot = stream.ReadByte();
                    SerialPortMgr.Instance.ResAvoid(robot);
                    OnMeetBlock?.Invoke();
                    Logger.Info($"收到避障信息, 已回应");
                    return;

                case 0x90:
                    Logger.Info($"收到前往区域指令回应");
                    OnGotoAreaResult?.Invoke(true);
                    return;

                case 0x91:
                    robot = stream.ReadByte();
                    int leftBattery = stream.ReadByte();
                    int batteryVoltage = stream.ReadHostOrderInt();
                    SerialPortMgr.Instance.ResBatteryInfo(robot);
                    Logger.Info($"收到电量信息, 剩余 {leftBattery}%, 电压 {batteryVoltage}V");
                    OnSetBatteryQuantity?.Invoke(leftBattery);
                    return;

                case 0x81:
                case 0x82:
                case 0x83:
                case 0x84:
                case 0x85:
                case 0x86:
                case 0x87:
                case 0x89:
                case 0x8A:
                case 0x8B:
                case 0x8C:
                    Logger.Info($"收到设置回应指令 0x{subcmd.ToString("x")}, 已忽略");
                    return;

                default:
                    Logger.Info($"收到未知指令 0x{subcmd.ToString("x")}, 已忽略");
                    return;
            }
        }

        public void OnSetLight(Action<bool> act)
        {
            OnSetLightResult = act;
        }

        public void OnGotoAreaRecived(Action<bool> act)
        {
            OnGotoAreaResult = act;
        }

        public void OnArrivedArea(Action<int> area)
        {
            OnArrivedAreaResult = area;
        }

        public void OnSetBattery(Action<int> quantity)
        {
            OnSetBatteryQuantity = quantity;
        }

        public void SetOnMeetBlock(Action act)
        {
            OnMeetBlock = act;
        }

        private Action<bool> OnSetLightResult;
        private Action<bool> OnGotoAreaResult;
        private Action<int> OnArrivedAreaResult;
        private Action<int> OnSetBatteryQuantity;
        private Action OnMeetBlock;

        #endregion
    }
}
