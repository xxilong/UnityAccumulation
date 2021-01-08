using ShareLib.Ayz;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidCtrl
{
    class KunPeng : MidPlug
    {
        public override string ConfigFile => "hardware.ini";
        private bool ltAll,ltTon1, ltTon2, ltTon3, ltTon4, ltDX, ltGQ, ltFGZ,ltDD,pwrAll,pwrA1,pwrA2,pwrA3,parA4;
        public bool LtAll
        {
            get { return ltAll; }
            set { ltAll = value;
                SetToggleFromID(ltAll, "lt-all");
            }
        }
        public bool LtTon1
        {
            get { return ltTon1; }
            set
            {
                ltTon1 = value;
                SetToggleFromID(ltTon1, "lt-ton1");
            }
        }
        public bool LtTon2
        {
            get { return ltTon2; }
            set
            {
                ltTon2 = value;
                SetToggleFromID(ltTon2, "lt-ton2");
            }
        }
        public bool LtTon3
        {
            get { return ltTon3; }
            set
            {
                ltTon3 = value;
                SetToggleFromID(ltTon3, "lt-ton3");
            }
        }
        public bool LtTon4
        {
            get { return ltTon4; }
            set
            {
                ltTon4 = value;
                SetToggleFromID(ltTon4, "lt-ton4");
            }
        }
        public bool LtDX
        {
            get { return ltDX; }
            set
            {
                ltDX = value;
                SetToggleFromID(ltDX, "dengxiang");
            }
        }
        public bool LtGQ
        {
            get { return ltGQ; }
            set
            {
                ltGQ = value;
                SetToggleFromID(ltGQ, "guangqian");
            }
        }
        public bool LtFGZ
        {
            get { return ltFGZ; }
            set
            {
                ltFGZ = value;
                SetToggleFromID(ltFGZ, "faguangzi");
            }
        }
        public bool LtDD
        {
            get { return ltDD; }
            set
            {
                ltDD = value;
                SetToggleFromID(ltDD, "dengdai");
            }
        }
        public bool PwrAll
        {
            get { return pwrAll; }
            set
            {
                pwrAll = value;
                SetToggleFromID(pwrAll, "paon");
                SetToggleFromID(!pwrAll, "paoff");
            }
        }
        public bool PwrA1
        {
            get { return pwrA1; }
            set
            {
                pwrA1 = value;
                SetToggleFromID(pwrA1, "a1on");
                SetToggleFromID(!pwrA1, "a1off");
            }
        }
        public bool PwrA2
        {
            get { return pwrA2; }
            set
            {
                pwrA2 = value;
                SetToggleFromID(pwrA2, "a2on");
                SetToggleFromID(!pwrA2, "a2off");
            }
        }
        public bool PwrA3
        {
            get { return pwrA3; }
            set
            {
                pwrA3 = value;
                SetToggleFromID(pwrA3, "a3on");
                SetToggleFromID(!pwrA3, "a3off");
            }
        }
        public override void OnInit()
        {
            base.OnInit();
            try
            {
                QXSTSerialPort.Instance.Open();
            }
            catch (Exception e)
            {

                Logger.Error(e.ToString());
            }
            
            //QXSTSerialPort.Instance.SwitchCar(CarType.Car);
        }

        public override void OnRecvCommand(string cmd)
        {
            Logger.Info($"Recive: {cmd}");
            CmdLine cmdline = new CmdLine(cmd);
            try
            {
                if (cmdline.cmd == "light")
                {
                    switch (cmdline.args[0])
                    {
                        case "all":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("7F F0 00 00", ltAll ? "00 00 00 00" : "7F F0 00 00"));
                            LtTon1 = LtTon2 =  LtTon3 =  LtTon4 = 
                            LtDX = LtFGZ =  LtGQ = LtDD = !ltAll;
                            break;
                        case "ton1":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("02 00 00 00", ltTon1 ? "00 00 00 00" : "02 00 00 00"));
                            LtTon1 = !ltTon1;
                            break;
                        case "ton2":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("04 00 00 00", ltTon2 ? "00 00 00 00" : "04 00 00 00"));
                            LtTon2 = !ltTon2;
                            break;
                        case "ton3":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("08 00 00 00", ltTon3 ? "00 00 00 00" : "08 00 00 00"));
                            LtTon3 = !ltTon3;
                            break;
                        case "ton4":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("10 00 00 00", ltTon4 ? "00 00 00 00" : "10 00 00 00"));
                            LtTon4 = !ltTon4;
                            break;
                        case "dengxiang":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("20 00 00 00", ltDX ? "00 00 00 00" : "20 00 00 00"));
                            LtDX = !ltDX;
                            break;
                        case "dengdai":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 10 00 00", ltDD ? "00 00 00 00" : "00 10 00 00"));
                            LtDD = !ltDD;
                            break;
                        case "faguangzi":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 40 00 00", ltFGZ ? "00 00 00 00" : "00 40 00 00"));
                            LtFGZ = !ltFGZ;
                            break;
                        case "guangqian":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 80 00 00", ltGQ ? "00 00 00 00" : "00 80 00 00"));
                            LtGQ = !ltGQ;
                            break;
                        default:
                            break;                         
                    }
                    if (ltTon1&& ltTon2&& ltTon3&&ltTon4&& ltDX&& ltDD&& ltFGZ&& ltGQ)
                    {
                        LtAll = true;
                    }
                    else if (!ltTon1 && !ltTon2 && !ltTon3 && !ltTon4 && !ltDX && !ltDD && !ltFGZ && !ltGQ)
                    {
                        LtAll = false;
                    }
                }
                if (cmdline.cmd == "power_off")
                {
                    switch (cmdline.args[0])
                    {
                        case "all":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("80 0F 00 00", "00 00 00 00"));
                            PwrA1 = PwrA2 = PwrA3 = PwrAll = false;
                            break;
                        case "A":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 0C 00 00", "00 00 00 00"));
                            PwrA1 = false;
                            break;
                        case "B":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 02 00 00", "00 00 00 00"));
                            PwrA2 = false;
                            break;
                        case "C":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("80 01 00 00", "00 00 00 00"));
                            PwrA3 = false;
                            break;                        
                        default:
                            break;
                    }
                    if (!PwrA1 && !PwrA2 && !PwrA3)
                    {
                        PwrAll = false;
                    }
                }
                if (cmdline.cmd == "power_on")
                {
                    switch (cmdline.args[0])
                    {
                        case "all":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("80 0F 00 00", "80 0F 00 00"));
                            PwrA1 = PwrA2 = PwrA3 = PwrAll = true;
                            break;
                        case "A":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 0C 00 00", "00 0C 00 00"));
                            PwrA1 = true;
                            break;
                        case "B":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("00 02 00 00", "00 02 00 00"));
                            PwrA2 = true;
                            break;
                        case "C":
                            QXSTSerialPort.Instance.SendRawData(GetRawData("80 01 00 00", "80 01 00 00"));
                            PwrA3 = true;
                            break;                        
                        default:
                            break;
                    }
                    if (PwrA1&& PwrA2&& PwrA3)
                    {
                        PwrAll= true;
                    }
                }
                if (cmdline.cmd == "query")
                {
                    LtTon1 = ltTon1; LtTon2 = ltTon2; LtTon3 = ltTon3; LtTon4 = ltTon4;
                    LtDX = ltDX; LtFGZ = ltFGZ; LtGQ = ltGQ; LtDD = ltDD;
                    LtAll = ltAll;

                    PwrA1 = pwrA1;
                    PwrA2 = pwrA2;
                    PwrA3 = pwrA3;
                    PwrAll = pwrAll;
                }
                SendCommandToControl("Get it!");
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
            
        }

        private void SetToggleFromID(bool check,string id)
        {
            SendCommandToControl("+" + (check ? "check" : "uncheck") +" "+ id);
        }

        static StringBuilder sb=new StringBuilder();
        static string GetRawData(string io,string cmd="00 00 00 00")
        {
            sb.Clear();
            sb.Append("10 03 FF ");
            sb.Append(io+" ");
            sb.Append(cmd);
            return sb.ToString();
        }


        static private byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace("0x", "");
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}
