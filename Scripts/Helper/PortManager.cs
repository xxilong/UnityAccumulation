using ShareLib.Conf;
using ShareLib.Ports.QXSandTable;
using ShareLib.Protocl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using UnityEngine;

public class PortManager : MonoBehaviour
{
    public Action<int> OnArrivedPosResult;
    public Action<int, int> OnArrivedPosWithMask;
    public Action<int, int> OnArrivedPosWithBoardID;
    public Action<int> OnArrivedAreaResult;
    public static PortManager instance;
    public string portname = "2";
    private int baudRate = 115200;

    SerialPort _port = null;
    private IPackProtocl _reader;
    public string msg;

    private void Awake()
    {
        instance = this;
    }

    public void Open()
    {

        foreach (var item in SerialPort.GetPortNames())
        {
            Debug.Log(item);
        }

        OpenPort();
        InvokeRepeating("GetNumOfData", 0.1f, 0.1f);
    }


    private void OpenPort(string cfgdomain = "COM")
    {
        if (_port != null && _port.IsOpen)
        {
            return;
        }

        _port = new System.IO.Ports.SerialPort(GlobalConf.getconf(cfgdomain, "name","COM2"),
            GlobalConf.getconf<int>(cfgdomain, "baudrate", 9600),
            GlobalConf.getenum<System.IO.Ports.Parity>(cfgdomain, "parity", System.IO.Ports.Parity.None),
            GlobalConf.getconf<int>(cfgdomain, "databits", 8),
            GlobalConf.getenum<System.IO.Ports.StopBits>(cfgdomain, "stopbits", System.IO.Ports.StopBits.None));

        _port.DtrEnable = GlobalConf.getconf<bool>(cfgdomain, "enable_dtr", false);
        _port.RtsEnable = GlobalConf.getconf<bool>(cfgdomain, "enable_rts", false);
        //_port = new SerialPort(@"\\.\COM" + portname, baudRate);
        _port.ReadTimeout = 100;
        try
        {
            _port.Open();
            Debug.Log($"打开串口{_port.PortName}成功");
        }
        catch (Exception ex)
        {
            Debug.Log($"打开串口时出现异常: {ex}");
        }        
    }


    private void ClosePort()
    {
        try
        {            
            _port.Close();
            Debug.Log($"关闭串口{_port.PortName}");
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }


    private void GetNumOfData()
    {
        if (_port != null && _port.IsOpen)
        {
            if (_port.BytesToRead > 0)
            {
                byte[] buff = new byte[_port.BytesToRead];
                Debug.Log($"收到数据长度: {buff.Length}");
                int length = buff.Length;
                _port.Read(buff, 0, length);                
                MemoryStream stream = new MemoryStream(buff);
                msg = BitConverter.ToString(buff);
                Debug.Log($"收到数据: {msg}");
                if (length > 4)
                {
                    byte[] data = new byte[length - 4];
                    Array.Copy(buff, 3, data, 0, data.Length);
                    OnRecvData(data);
                }
            }
            //try
            //{
            //    if (_port.BytesToRead>0)
            //    {
            //        byte[] buff = new byte[_port.BytesToRead];
            //        Debug.Log($"收到数据长度: {buff.Length}");
            //        int length = buff.Length;
            //        _port.Read(buff, 0, length);
            //        msg = BitConverter.ToString(buff);

            //        Debug.Log($"收到数据: {msg}");
            //        if (length>4)
            //        {
            //            byte[] data = new byte[length - 4];
            //            buff.CopyTo(data, 2);
            //            OnRecvData(data);
            //        }                    
            //    }
                
            //    //int length= sp.BytesToRead;
            //    //print(sp.ReadByte());
            //}
            //catch (System.Exception ex)
            //{

            //    Debug.LogError(ex.Message);
            //}
        }
    }

    public void SendRawData( string str)
    {
        try
        {
            byte[] data = QXSTSerialProtocol.StrToToHexByte(str);            
            //sp.Open();
            _port.Write(data, 0, data.Length);
            Debug.Log($"发送数据{str}");
        }
        catch (Exception eloop)
        {
            Debug.LogError($"发送数据异常后重试打开串口发送异常: {eloop}");
        }
    }
       
    public void OnRecvData(byte[] data)
    {
        string sdata = BitConverter.ToString(data);
        sdata = sdata.Replace("-", "");
        MemoryStream stream = new MemoryStream(data);
        int maincmd = stream.ReadByte();

        if (maincmd == 0xF8)
        {
            Debug.Log($"收到程序更新指令, 忽略");
            return;
        }

        if (maincmd != 0x10)
        {
            Debug.LogError($"收到未知的指令 0x{maincmd.ToString("x")}");
            return;
        }

        int robot = 0;
        int subcmd = stream.ReadByte();

        switch (subcmd)
        {
            case 0x88:
                int bordid = stream.ReadByte();
                Debug.Log($"bordid, {bordid}");

                int mask = stream.ReadByte();              
                Debug.Log($"mask, {mask}");
                OnArrivedPosResult?.Invoke(mask);
                OnArrivedPosWithMask += (b, m) =>
                {
                    if (b==0xFF)
                    {
                        bool open = m == 0x01;
                    }
                };
                OnArrivedPosWithMask?.Invoke(bordid, mask);
                return;
        }
    }

    protected void OnRecvPackage(MyMemoryStream stream)
    {
        string sdata = BitConverter.ToString(stream.ToArray());
        sdata = sdata.Replace("-", "");

        int maincmd = stream.ReadByte();
         
        if (maincmd == 0xF8)
        {
            Debug.Log($"收到程序更新指令, 忽略");
            return;
        }

        if (maincmd != 0x10)
        {
            Debug.LogError($"收到未知的指令 0x{maincmd.ToString("x")}");
            return;
        }

        int robot = 0;
        int subcmd = stream.ReadByte();

        switch (subcmd)
        {
            case 0x88:
                int bordid = stream.ReadByte();
                int mask = stream.ReadHostOrderInt();
                int status = stream.ReadHostOrderInt();

                QXSTSerialPort.Instance.ResInput(bordid);
                Debug.Log($"收到输入信息, 已回应");

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

                Debug.Log($"输入状态有效值: 0x{ status.ToString("x") }, 对应区域为: {area}");
                OnArrivedAreaResult?.Invoke(area);
                return;

            case 0x8D:
                stream.Skip(1);
                int success = stream.ReadByte();
                Debug.Log($"设置灯光结果 {success}");
                return;

            case 0x8E:
                Debug.Log($"收到读取灯光回应指令, 已忽略");
                return;

            case 0x8F:
                robot = stream.ReadByte();
                QXSTSerialPort.Instance.ResAvoid(robot);
                Debug.Log($"收到避障信息, 已回应");
                return;

            case 0x90:
                Debug.Log($"收到前往区域指令回应");
                return;

            case 0x91:
                robot = stream.ReadByte();
                int leftBattery = stream.ReadByte();
                int batteryVoltage = stream.ReadHostOrderInt();
                QXSTSerialPort.Instance.ResBatteryInfo(robot);
                Debug.Log($"收到电量信息, 剩余 {leftBattery}%, 电压 {batteryVoltage}V");
                return;

            case 0x85:
                int boardid = stream.ReadByte();
                int jdstatus = stream.ReadByte();
                Debug.Log($"收到继电器状态信息: {jdstatus}");
                return;
            case 0x84:
                int powerBoardId = stream.ReadByte();
                int powerid = stream.ReadHostOrderInt();
                Debug.Log($"收到电源状态信息: {powerid}");
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
                Debug.Log($"收到设置回应指令 0x{subcmd.ToString("x")}, 已忽略");
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
                Debug.Log($"收到Marker状态信息: id={markerids} side={markersides}");
                return;

            case 0xA1:
                int mkid = stream.ReadByte();
                int mkstatus = stream.ReadByte();
                Debug.Log($"收到设置 Marker {mkid} 结果 {mkstatus}");
                return;

            default:
                Debug.Log($"收到未知指令 0x{subcmd.ToString("x")}, 已忽略");
                return;
        }
    }


    private void OnDisable()
    {        
        ClosePort();
    }
}
