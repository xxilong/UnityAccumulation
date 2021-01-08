using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OceanStor
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            OceanStor.Main.instance.StartUp();
            while (true)
            {
                string str = Console.ReadLine();
                if (str == "exit")
                {
                    break;
                }
                else if(!String.IsNullOrWhiteSpace(str))
                {
                    OceanStor.Main.instance._server.SendCommand(str);
                }
            }
        }
    }
}
