using ShareLib.Log;
using ShareLib.Ports.QXSandTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COMSendHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Set(new ConsoleLogger());

            QXSTSerialPort.Instance.Open();
            Logger.Info("SerialPort opened.");

            while (true)
            {
                Console.Write(">>> ");
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "exit" || line == "quit")
                {
                    break;
                }

                if(line[0] == 's')
                {
                    int i = line.IndexOf(' ');
                    string number = line.Substring(1, i);
                    string data = line.Substring(i);
                    if(!int.TryParse(number, out int index))
                    {
                        Logger.Error("bad number.");
                        continue;
                    }

                    _senddata[index] = data;
                    Logger.Info($"data {index} set to: {data}");
                }
                else
                {
                    if(!int.TryParse(line, out int index))
                    {
                        Logger.Error("bad number.");
                        continue;
                    }

                    if(!_senddata.ContainsKey(index))
                    {
                        Logger.Error($"{index} has not set.");
                        continue;
                    }

                    QXSTSerialPort.Instance.SendRawData(_senddata[index]);
                }
            }
        }

        static private Dictionary<int, string> _senddata = new Dictionary<int, string>();
    }
}
