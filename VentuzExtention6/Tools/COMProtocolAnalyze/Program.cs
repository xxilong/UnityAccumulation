using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Log;

namespace COMProtocolAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer analyzer = new Analyzer();
            Logger.Set(new ConsoleLogger());

            analyzer.Test();

            while(true)
            {
                Console.Write("-$ ");          
                string line = Console.ReadLine();

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line == "exit" || line == "quit")
                {
                    break;
                }

                analyzer.DumpPacketInfo(line);
            }            
        }
    }
}
