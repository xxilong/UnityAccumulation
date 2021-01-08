using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjShMWCFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            ProjShMWCFactory.Main.instance.StartUp();
            while (true)
            {
                if (Console.ReadLine()== "exit")
                {
                    break;
                }
            }
        }
             
    }
}
