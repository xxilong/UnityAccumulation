using ShareLib.Log;
using ShareLib.Protocl;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ports.QXSandTable
{
    public class QXSTSerialProtocol : DeviceProtocl
    {
        #region 发包
        public virtual byte[] GotoArea(int area)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x10);
            pack.WriteByte((byte)_curCarId);
            pack.WriteHostOrderInt(1 << area);
            return ToArray(pack);
        }

        public virtual byte[] GotoPlace(int pos)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x10);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteHostOrderInt(pos);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }

        public virtual byte[] Go()
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0B);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }

        public virtual byte[] Stop()
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0C);
            pack.WriteByte((byte)_curCarId);
            return ToArray(pack);
        }

        public virtual byte[] SetCarLight(int light)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x0D);
            pack.WriteByte((byte)_curCarId);
            pack.WriteByte(0xFF);
            pack.WriteByte((byte)light);
            return ToArray(pack);
        }

        public byte[] SetSandboxLight(int light) => SetSandboxLightMask(1 << light, -1);
        public byte[] SetSandboxLightOff() => SetSandboxLightMask(0, -1);

        public byte[] SetSandboxLightMask(int lightmask, int controlmask)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x03);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteHostOrderInt(controlmask);
            pack.WriteHostOrderInt(lightmask);
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

        public byte[] SetOutSignal(byte mask, byte status)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x05);
            pack.WriteByte((byte)MontherBoardId);
            pack.WriteByte((byte)mask);
            pack.WriteByte((byte)status);
            return ToArray(pack);
        }

        public virtual byte[] SetMarkerLight(int markerid, int mode, Color color)
        {
            MyMemoryStream pack = MakePackage(0x10, 0x21);
            pack.WriteByte((byte)markerid);
            pack.WriteByte((byte)mode);
            pack.WriteByte(color.R);
            pack.WriteByte(color.G);
            pack.WriteByte(color.B);
            return ToArray(pack);
        }

        public virtual byte[] MakeRawData(string hexstr)
        {
            var stream = new MyMemoryStream();
            stream.WriteByteArray(StrToToHexByte(hexstr));
            return ToArray(stream);
        }

        static public byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace("\t", "");
            hexString = hexString.Replace("-", "");
            hexString = hexString.Replace("0x", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        protected MyMemoryStream MakePackage(byte mainCmd, byte subCmd)
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
            string sdata = BitConverter.ToString(stream.ToArray());
            sdata = sdata.Replace("-", "");
            if(_rawDataEvent.ContainsKey(sdata))
            {
                _rawDataEvent[sdata]?.Invoke();
            }

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

            if(OnRecvSubCmd(subcmd, stream))
            {
                return;
            }

            switch (subcmd)
            {
                case 0x88:
                    int bordid = stream.ReadByte();
                    int mask = stream.ReadHostOrderInt();
                    int status = stream.ReadHostOrderInt();

                    QXSTSerialPort.Instance.ResInput(bordid);
                    Logger.Info($"收到输入信息, 已回应");

                    OnArrivedPosResult?.Invoke(status);
                    OnArrivedPosWithMask?.Invoke(status, mask);
                    OnArrivedPosWithBoardID?.Invoke(status, bordid);

                    status = mask & status;

                    int area = 0;
                    for (int i = 0; i < 32; ++i)
                    {
                        if ((status & 0x1) != 0)
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
                    QXSTSerialPort.Instance.ResAvoid(robot);
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
                    QXSTSerialPort.Instance.ResBatteryInfo(robot);
                    Logger.Info($"收到电量信息, 剩余 {leftBattery}%, 电压 {batteryVoltage}V");
                    OnSetBatteryQuantity?.Invoke(leftBattery);
                    return;

                case 0x85:
                    int boardid = stream.ReadByte();
                    int jdstatus = stream.ReadByte();
                    Logger.Info($"收到继电器状态信息: {jdstatus}");
                    OnNotifyRelayStatus?.Invoke(jdstatus);
                    return;
                case 0x84:
                    int powerBoardId = stream.ReadByte();
                    int powerid = stream.ReadHostOrderInt();
                    Logger.Info($"收到电源状态信息: {powerid}");
                    OnNotifyPowerStatus?.Invoke(powerid);
                    return;

                case 0x81:
                case 0x82:
                case 0x83:
                case 0x86:
                case 0x87:
                case 0x89:
                case 0x8A:
                case 0x8B:
                case 0x8C:
                    Logger.Info($"收到设置回应指令 0x{subcmd.ToString("x")}, 已忽略");
                    return;

                case 0x94:
                    int[] markerids = { 0, 0, 0, 0, 0, 0 };
                    int[] markersides = { 0, 0, 0, 0, 0, 0 };
                    boardid = stream.ReadByte();
                    for (int i = 0; i < 6; ++i)
                    {
                        status = stream.ReadByte();
                        status = stream.ReadByte();
                        int id = stream.ReadByte();
                        int side = stream.ReadByte();
                        markerids[i] = id;
                        markersides[i] = side;
                    }
                    Logger.Info($"收到Marker状态信息: id={string.Join(",", markerids)} side={string.Join(",", markersides)}");
                    OnMarkersStatus?.Invoke(markerids, markersides);
                    return;

                case 0xA1:
                    int mkid = stream.ReadByte();
                    int mkstatus = stream.ReadByte();
                    Logger.Info($"收到设置 Marker {mkid} 结果 {mkstatus}");
                    return;

                default:
                    Logger.Info($"收到未知指令 0x{subcmd.ToString("x")}, 已忽略");
                    return;
            }
        }

        public virtual bool OnRecvSubCmd(int subcmd, MyMemoryStream stream) => false;

        public void OnSetLight(Action<bool> act) => OnSetLightResult = act;
        public void OnGotoAreaRecived(Action<bool> act) => OnGotoAreaResult = act;
        public void OnArrivedArea(Action<int> area) => OnArrivedAreaResult = area;
        public void OnSetBattery(Action<int> quantity) => OnSetBatteryQuantity = quantity;
        public void SetOnMeetBlock(Action act) => OnMeetBlock = act;
        public void SetRelayStatusListener(Action<int> act) => OnNotifyRelayStatus = act;
        public void SetPowerStatusListener(Action<int> act) => OnNotifyPowerStatus = act;
        public void SetArrivedPosListener(Action<int> act) => OnArrivedPosResult = act;
        public void SetArrivedPosWithMaskListenner(Action<int, int> act) => OnArrivedPosWithMask = act;
        public void SetArrivedPosWithIDListenner(Action<int, int> act) => OnArrivedPosWithBoardID = act;
        public void SetMarkersStatusListenner(Action<int[], int[]> act) => OnMarkersStatus = act;
        public void SetRawDataListenner(string data, Action act)
        {
            string key = data;
            key = key.Replace("-", "");
            key = key.Replace(" ", "");
            if(act == null)
            {
                _rawDataEvent.Remove(key);
            }
            else
            {
                _rawDataEvent[key] = act;
            }
        }

        private Action<bool> OnSetLightResult;
        private Action<bool> OnGotoAreaResult;
        private Action<int> OnArrivedAreaResult;
        private Action<int> OnArrivedPosResult;
        private Action<int, int> OnArrivedPosWithMask;
        private Action<int, int> OnArrivedPosWithBoardID;
        private Action<int> OnSetBatteryQuantity;
        private Action<int> OnNotifyRelayStatus;
        private Action<int> OnNotifyPowerStatus;
        private Action<int[], int[]> OnMarkersStatus;
        private Action OnMeetBlock;
        private Dictionary<string, Action> _rawDataEvent = new Dictionary<string, Action>();

        #endregion

        #region 状态设置

        public void SetCurCar(int car)
        {
            _curCarId = car;
        }

        public int MontherBoardId { get; set; } = 0xff;

        #endregion
        protected int _curCarId = 0;
    }
}
