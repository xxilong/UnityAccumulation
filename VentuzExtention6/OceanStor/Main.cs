/*
设置全部点亮蓝灯/电机复位
上位机发给主板：  51 58 0B 10 03 03 DF 03 00 00 9A 02 00 00 Xor
主板回复给上位机：51 58 04 10 83 03 00 Xor
上位机发给主板：  51 58 0B 10 03 05 DF 03 00 00 9A 02 00 00 Xor
主板回复给上位机：51 58 04 10 83 05 00 Xor
设置四机柜故障(左机柜ID = 3 右机柜ID = 5)
上位机发给主板：  51 58 0B 10 03 ID 0C 03 00 00 86 01 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
恢复四机柜正常（左机柜ID = 3 右机柜ID = 5）
上位机发给主板：  51 58 0B 10 03 ID 0C 03 00 00 88 02 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
设置三机柜故障(左机柜ID = 3 右机柜ID = 5)
上位机发给主板：  51 58 0B 10 03 ID C3 00 00 00 49 02 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
恢复三机柜正常（左机柜ID = 3 右机柜ID = 5）
上位机发给主板：  51 58 0B 10 03 ID C3 00 00 00 82 00 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
设置七机柜故障(左机柜ID = 3 右机柜ID = 5)
上位机发给主板：  51 58 0B 10 03 ID CF 03 00 00 45 01 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
设置所有灯光熄灭(左机柜ID = 3 右机柜ID = 5)
上位机发给主板：  51 58 0B 10 03 ID DF 03 00 00 80 02 00 00 Xor
主板回复给上位机：51 58 04 10 83 ID 00 Xor
推杆输入上传指令(左机柜ID = 3 右机柜ID = 5)
主板发给上位机：  51 58 0B 10 88 ID 01 00 00 00 01 00 00 00 Xor （推杆上）
主板发给上位机：  51 58 0B 10 88 ID 01 00 00 00 00 00 00 00 Xor （推杆下）
*/

using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Ayz;
using ShareLib.Page;
using ShareLib.Unity;
using System.Collections;

namespace OceanStor
{
    public class Main
    {
        private bool left, right = false;

        private bool isDouble = false;
        public bool IsDouble
        {
            get { return isDouble; }
            set
            {
                if (value!=isDouble)
                {
                    if (value)
                    {
                        //进入双活模块
                        QXSTSerialPort.Instance.SendRawData("10 10 03 01 00 00 00");
                        ResetHardware();
                    }
                    else
                    {
                        //离开双活模块
                        QXSTSerialPort.Instance.SendRawData("10 10 03 00 00 00 00");
                    }
                    isDouble = value;
                }
            }
        }

        private bool isOnline = false;
        public bool IsOnline
        {
            get { return isOnline; }
            set
            {
                if (value != isOnline)
                {
                    if (value)
                    {
                        ResetHardware();
                        left = true;
                        right = false;
                    }
                    isOnline = value;
                }
            }
        }

        public static Main instance = new Main();
        
        public void StartUp()
        {
            _server.SetReciverListener(OnRecievedCmd);
            _server.Start(3131);

            //QXSTSerialPort.Instance._procol.SetArrivedPosListener(OnArrivedPos);
            QXSTSerialPort.Instance._procol.SetArrivedPosWithIDListenner(OnArrivedPosWithID);

            QXSTSerialPort.Instance.Open();
            ResetHardware();
        }

        private static void ResetHardware()
        {
            QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 9A 02 00 00");
            QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 9A 02 00 00");
        }



        //转发所有收到的命令给所有客户端（或除发送端之外的客户端）；
        //收到“left on”指令：控制左侧机柜模组弹出；
        //收到“left off”指令：控制左侧机柜模组收回；
        //收到“right on”指令：控制右侧机柜模组弹出；
        //收到“right off”指令：控制右侧机柜模组收回；
        //收到“engine1 on”指令或初始状态：控制左侧机柜对应的4个模组亮蓝灯并收回。
        //收到“engine1 off”指令：控制左侧机柜对应的4组亮红灯并弹出。
        //收到“engine2 on”指令或初始状态：控制右侧机柜对应的4个模组亮蓝灯并收回。
        //收到“engine2 off”指令：控制右侧机柜对应的3组亮红灯并弹出。
        private void OnRecievedCmd(string line,string peername)
        {
            Logger.Info($"收到客户端 {peername} 命令: {line}");
            CmdLine cmd = new CmdLine(line);
            _server.SendCommandToOther(line, peername);

            string arg;            

            if (!cmd.getarg<string>(0, out arg))
            {
                return;
            }
            if (cmd.cmd=="go")
            {
                if (arg=="home")
                {
                    ResetHardware();
                }
                if (arg=="double")
                {
                    IsDouble = true;
                }
                else
                {
                    IsDouble = false;
                    
                    if (arg == "online")
                    {
                        IsOnline = true;
                    }
                    else
                    {
                        IsOnline = false;
                    }
                }
            }
            
            if (cmd.cmd == "left")
            {
                if (arg == "on")
                {
                    //左侧机柜全部模块复位
                    QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 9A 02 00 00");
                    if (right)
                    {
                        //右侧机柜全部模块复位
                        QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 9A 02 00 00");
                        right = false;
                    }
                    left = true;
                }
                else if (arg == "off")
                {
                    left = false;
                    //左侧机柜全部模块复位
                    QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 9A 02 00 00");
                }
            }
            if (cmd.cmd == "right")
            {
                if (arg == "on")
                {
                    //右侧机柜全部模块复位
                    QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 9A 02 00 00");
                    right = true;
                    if (left)
                    {
                        //左侧机柜全部模块复位
                        QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 9A 02 00 00");
                        left = false;
                    }
                }
                else if (arg == "off")
                {
                    right = false;
                    //右侧机柜全部模块复位
                    QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 9A 02 00 00");
                }
            }
            if (cmd.cmd == "engine1")
            {
                if (arg == "on")
                {
                    if (left)
                    {
                        //左侧机柜对应的4个模组亮蓝灯并收回
                        QXSTSerialPort.Instance.SendRawData("10 03 03 0C 03 00 00 88 02 00 00");
                    }
                    else if (right)
                    {
                        //右侧机柜对应的4个模组亮蓝灯并收回
                        QXSTSerialPort.Instance.SendRawData("10 03 05 0C 03 00 00 88 02 00 00");
                    }
                    
                }
                else if (arg == "off")
                {
                    if (left)
                    {
                        //左侧机柜对应的4个模组亮红灯并弹出
                        QXSTSerialPort.Instance.SendRawData("10 03 03 0C 03 00 00 86 01 00 00");
                    }
                    else if (right)
                    {
                        //侧机柜右对应的4个模组亮红灯并弹出
                        QXSTSerialPort.Instance.SendRawData("10 03 05 0C 03 00 00 86 01 00 00");
                    }
                }
            }
            if (cmd.cmd == "engine2")
            {
                if (arg == "on")
                {
                    if (left)
                    {
                        //左侧机柜对应的3个模组亮蓝灯并收回
                        QXSTSerialPort.Instance.SendRawData("10 03 03 C3 00 00 00 82 00 00 00");
                    }
                    else if (right)
                    {
                        //右侧机柜对应的3个模组亮蓝灯并收回
                        QXSTSerialPort.Instance.SendRawData("10 03 05 C3 00 00 00 82 00 00 00");
                    }
                }
                else if (arg == "off")
                {
                    if (left)
                    {
                        //左侧机柜对应的3个模组亮红灯并弹出
                        QXSTSerialPort.Instance.SendRawData("10 03 03 C3 00 00 00 49 02 00 00");
                    }
                    else if (right)
                    {
                        //右侧机柜对应的3个模组亮红灯并弹出
                        QXSTSerialPort.Instance.SendRawData("10 03 05 C3 00 00 00 49 02 00 00");
                    }
                }
            }
            
        }


        private void OnArrivedPosWithID(int status, int id)
        {
            Logger.Info($"收到主板{id}按键状态 {status}");
            if (!isDouble)
            {
                return;
            }
            if (id==3)
            {
                if (status==0x01)
                {
                    _server.SendCommand("switch1 on");
                    //左侧机柜全部亮灯
                    //QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 9A 02 00 00");
                }
                else if (status==0x0)
                {
                    //左侧机柜熄灯
                    //QXSTSerialPort.Instance.SendRawData("10 03 03 DF 03 00 00 80 02 00 00");
                    _server.SendCommand("switch1 off");
                }
            }
            else if (id==5)
            {
                if (status == 0x01)
                {
                    _server.SendCommand("switch2 on");
                    //右侧机柜全部亮灯
                    //QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 9A 02 00 00");
                }
                else if (status == 0x0)
                {
                    _server.SendCommand("switch2 off");
                    //右侧机柜熄灯
                    //QXSTSerialPort.Instance.SendRawData("10 03 05 DF 03 00 00 80 02 00 00");
                }
            }
        }

        /*
         收到开关1打开指令：发送“switch1 on”指令给所有客户端，控制左侧机柜所有模组亮灯。
         收到开关1关闭指令：发送“switch1 off”指令给所有客户端，控制左侧机柜所有模组灭灯。
         收到开关2打开指令：发送“switch2 on”指令给所有客户端，控制右侧机柜所有模组亮灯。
         收到开关2关闭指令：发送“switch2 off”指令给所有客户端，控制右侧机柜所有模组灭灯。
         */
        private void OnArrivedPos(int pos)
        {
            Logger.Info($"收到按键状态 {pos}");

            switch (pos)
            {
                case 0x01:            
                    break;

                case 0x02:
                    Page.Control.SetProperty("scene", "hj");
                    break;

                case 0x04:
                    Page.Control.SetProperty("scene", "bg");
                    break;

                case 0x08:
                    Page.Control.SetProperty("scene", "csd");
                    break;

                case 0x10:
                    Page.Control.SetProperty("scene", "aj");
                    break;

                case 0x20:
                    _isSwitch1Off = false;
                    _server.SendCommand("engine good");
                    Logger.Info($"engine status change to good.");
                    break;

                case 0x40:
                    Page.Control.SetProperty("scene", "agv");
                    break;

                case 0x0:
                    _isSwitch1Off = true;
                    _server.SendCommand("engine bad");
                    Logger.Info($"engine status change to bad.");
                    break;

                default:
                    Logger.Debug($"unknown status {pos}");
                    break;
            }
        }
        public CmdTcpServer _server = new CmdTcpServer();
        private bool _isSwitch1Off, _isSwitch2Off = false;
    }
}
