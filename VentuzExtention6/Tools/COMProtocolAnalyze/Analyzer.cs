using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Protocl;
using ShareLib.Ports.QXSandTable;
using System.Drawing;

namespace COMProtocolAnalyze
{
    public class Analyzer
    {
        public void Test()
        {
            //QXSTSerialPort.Instance.Open();
            //QXSTSerialPort.Instance.SendWith(p => p.SetMarkerLight(10, 0, Color.FromArgb(0x123456)));
        }

        public void DumpPacketInfo(string hexdata)
        {
            byte[] data;

            try
            {
                data = StrToToHexByte(hexdata);
            }
            catch(Exception)
            {
                Logger.Error("包含非 16 进制字符");
                return;
            }

            MyMemoryStream stream = new MyMemoryStream(data);
            DumpStreamInfo(stream);
        }

        public void DumpStreamInfo(MyMemoryStream stream)
        {
            int fac1 = stream.ReadByte();
            int fac2 = stream.ReadByte();

            if(fac1 != 0x51 || fac2 != 0x58)
            {
                Logger.Error("数据头错误, 未包含厂商控制字 51 58");
                return;
            }

            Logger.Info("厂商控制字: 51 58");
            int len = stream.ReadByte();
            if(len != stream.Length - 4)
            {
                Logger.Error($"数据长度错误, 实际长度: {stream.Length}, 字段标识长度: {len} + 4(HEAD & TAIL) = {len + 4}");
                return;
            }
            Logger.Info($"数据长度: {len} (0X{len.ToString("X2")})");

            int mainCmd = stream.ReadByte();
            int subCmd = stream.ReadByte();
            Logger.Info($"命令: {mainCmd.ToString("X2")} {subCmd.ToString("X2")}");
            if(!DumpCommand(mainCmd, subCmd, stream, len))
            {
                return;
            }

            int xorCheck = stream.ReadByte();
            stream.SeekToBegin();
            byte x = 0;
            for(int i = 0; i < stream.Length - 1; ++i)
            {
                x ^= (byte)stream.ReadByte();
            }

            if(xorCheck != 0 && x != xorCheck)
            {
                Logger.Warning($"校验码不匹配! 计算值: 0X{x.ToString("X2")}, 使用值: 0X{xorCheck.ToString("X2")}");
            }
            else
            {
                if(xorCheck == 0)
                {
                    Logger.Info($"校验码未设置(0X00), 应为: 0X{x.ToString("X2")}");
                }
                else
                {
                    Logger.Info($"校验码: 0X{xorCheck.ToString("X2")}");
                }
            }
        }

        public bool DumpCommand(int main, int sub, MyMemoryStream stream, int len)
        {
            switch(main)
            {
                case 0x10:
                    if(DumpCommand10(sub, stream))
                    {
                        return true;
                    }
                    break;

                default:
                    Logger.Error($"未识别的主命令 0X{main.ToString("X2")}");
                    break;
            }

            byte[] data = new byte[len - 2];
            stream.ReadByteArray(data);
            Logger.Info($"RawData {main.ToString("X2")} {sub.ToString("X2")} {BitConverter.ToString(data)}");
            return true;
        }

        public bool DumpCommand10(int sub, MyMemoryStream stream)
        {
            int boardid = 0;
            int carid = 0;
            int status = 0;
            int mask = 0;
            int area = 0;
            int markerid = 0;
            int lightmode = 0;
            int lightR = 0;
            int lightG = 0;
            int lightB = 0;
            string format;

            switch(sub)
            {
                case 0x03:
                    boardid = stream.ReadByte();
                    mask = stream.ReadHostOrderInt();
                    status = stream.ReadHostOrderInt();
                    Logger.Info("指令 0x10 0x03 设置输出信号");
                    Logger.Info($"    主板 ID: {boardid}(0X{boardid.ToString("X2")})");
                    Logger.Info($"    输出掩码: {mask} (0X{mask.ToString("X8")})");
                    Logger.Info($"    输出状态: {status} (0X{status.ToString("X8")})");
                    Logger.Info($"发送参考代码: QXSTSerialPort.Instance.SetSandLightMask({status}, {mask});");
                    return true;

                case 0x0B:
                    boardid = stream.ReadByte();
                    status = stream.ReadHostOrderInt();
                    carid = stream.ReadByte();
                    Logger.Info("指令 0x10 0x0B 智能车发车");
                    Logger.Info($"    主板 ID: {boardid}(0X{boardid.ToString("X2")})");
                    Logger.Info($"    小车 ID: {carid}(0X{carid.ToString("X2")})");
                    Logger.Info($"    目标(未使用): {status} (0X{status.ToString("X8")})");
                    Logger.Info($"发送参考代码: QXSTSerialPort.Instance.SendGo();");
                    Logger.Info($"              QXSTSerialPort.Instance.SendOn(CarType.Car, p => p.SendGo())");
                    return true;

                case 0x0C:
                    boardid = stream.ReadByte();
                    status = stream.ReadHostOrderInt();
                    carid = stream.ReadByte();
                    Logger.Info("指令 0x10 0x0C 智能车停车");
                    Logger.Info($"    主板 ID: {boardid}(0X{boardid.ToString("X2")})");
                    Logger.Info($"    小车 ID: {carid}(0X{carid.ToString("X2")})");
                    Logger.Info($"    目标(未使用): {status} (0X{status.ToString("X8")})");
                    Logger.Info($"发送参考代码: QXSTSerialPort.Instance.SendStop();");
                    Logger.Info($"              QXSTSerialPort.Instance.SendOn(CarType.Car, p => p.SendStop())");
                    return true;

                case 0x10:
                    boardid = stream.ReadByte();
                    status = stream.ReadHostOrderInt();
                    carid = stream.ReadByte();
                    area = GetArea(status);
                    Logger.Info($"指令 0x10 0x10 智能车运行到指定位置后停车");
                    Logger.Info($"    主板 ID: {boardid}(0X{boardid.ToString("X2")})");
                    Logger.Info($"    小车 ID: {carid}(0X{carid.ToString("X2")})");
                    Logger.Info($"    位置: {area} (0X{status.ToString("X8")})");
                    if(area == -1)
                    {
                        Logger.Info($"发送参考代码: QXSTSerialPort.Instance.SendGoPos({status});");
                        Logger.Info($"              QXSTSerialPort.Instance.SendOn(CarType.Car, p => p.SendGoPos({status}))");
                    }
                    else
                    {
                        Logger.Info($"发送参考代码: QXSTSerialPort.Instance.SendGoto({area});");
                        Logger.Info($"              QXSTSerialPort.Instance.SendOn(CarType.Car, p => p.SendGoto({area}))");
                    }
                    return true;

                case 0x21:
                    markerid = stream.ReadByte();
                    lightmode = stream.ReadByte();
                    lightR = stream.ReadByte();
                    lightG = stream.ReadByte();
                    lightB = stream.ReadByte();
                    Logger.Info($"指令 0x10 0x21 设置Marker颜色");
                    Logger.Info($"    Marker ID: {markerid}(0X{markerid.ToString("X2")})");
                    Logger.Info($"    变色模式: {lightmode}(0X{lightmode.ToString("X2")}) {(lightmode == 0 ? "不变色" : "")}");
                    Logger.Info($"    颜色: #{lightR.ToString("X2")}{lightG.ToString("X2")}{lightB.ToString("X2")})");
                    Logger.Info($"发送代码: QXSTSerialPort.Instance.SendWith(p=>p.SetMarkerLight({markerid}, {lightmode}, Color.FromArgb(0x{lightR.ToString("X2")}{lightG.ToString("X2")}{lightB.ToString("X2")})));");
                    return true;

                case 0x88:
                    boardid = stream.ReadByte();
                    mask = stream.ReadHostOrderInt();
                    status = stream.ReadHostOrderInt();
                    area = GetArea(mask & status);
                    Logger.Info($"指令 0x10 0x88 主板上传输入状态");
                    Logger.Info($"    掩码: {mask} (0X{mask.ToString("X8")})");
                    Logger.Info($"    状态: {status} (0X{status.ToString("X8")})");
                    if(area != -1)
                    {
                        Logger.Info($"处理代码: QXSTSerialPort.Instance._procol.OnArrivedArea((int area@{area}) => {{ }});");
                    }
                    Logger.Info($"处理代码:  ..._procol.SetArrivedPosWithMaskListenner((int status@{status}, int mask@{mask}) => {{}});");
                    Logger.Info($"处理代码:  QXSTSerialPort.Instance._procol.SetArrivedPosListener((int status@{status}) => {{}});");
                    return true;

                case 0x94:
                    Logger.Info($"指令 0x10 0x94 主板上传Marker信息");
                    boardid = stream.ReadByte();
                    Logger.Info($"    设备 ID: {boardid}(0X{boardid.ToString("X2")})");

                    for(int i = 0; i < 6; ++i)
                    {
                        Logger.Info($"       位置{i + 1}:");
                        status = stream.ReadByte();
                        Logger.Info($"           读卡器是否存在: {status} {(status == 0 ? "不存在" : "存在")}");
                        status = stream.ReadByte();
                        format = "";
                        if (status == 0)
                        {
                            format = "不存在";
                        }
                        else if (status == 1)
                        {
                            format = "IC卡插入";
                        }
                        else if (status == 2)
                        {
                            format = "IC卡存在";
                        }
                        else if (status == 3)
                        {
                            format = "IC卡拔出";
                        }
                        Logger.Info($"           IC卡是否存在: {status} {format}");
                        int id = stream.ReadByte();
                        int side = stream.ReadByte();
                        Logger.Info($"           MarkerID: {id}");
                        Logger.Info($"           面 {side}");
                    }
                    return true;

                default:
                    Logger.Error($"未识别的命令 0X10 0X{sub.ToString("X2")}");
                    return false;
            }
        }

        static private int GetArea(int place)
        {
            for(int i = 0; i < 32; ++i)
            {
                if(1 << i == place)
                {
                    return i;
                }
            }

            return -1;
        }

        static private byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace("\t", "");
            hexString = hexString.Replace("-", "");
            hexString = hexString.Replace("XOR", "00");
            hexString = hexString.Replace("Xor", "00");
            hexString = hexString.Replace("0x", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
