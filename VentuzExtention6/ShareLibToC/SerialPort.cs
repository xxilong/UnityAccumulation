using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;
using ShareLib.Ports.QXSandTable;

namespace ShareLibToC
{
    public class SerialPort
    {
        static public int Open(string args)
        {
            try
            {
                QXSTSerialPort.Instance.Open();
                return 0;
            }
            catch (Exception e)
            {
                Logger.Error($"{e}");
                return -2;
            }
        }

        static public int SendQXHex(string data)
        {
            try
            {
                QXSTSerialPort.Instance.SendRawData(data);
                return 0;
            }
            catch(Exception e)
            {
                Logger.Error($"{e}");
                return -2;
            }
        }

        static public int SendRawHex(string data)
        {
            try
            {
                QXSTSerialPort.Instance.Write(QXSTSerialProtocol.StrToToHexByte(data));
                return 0;
            }
            catch(Exception e)
            {
                Logger.Error($"{e}");
                return -2;
            }
        }
    }
}
