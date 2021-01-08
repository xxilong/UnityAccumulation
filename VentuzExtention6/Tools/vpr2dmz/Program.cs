using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.DRZ;

namespace vpr2drz
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "default.vpr";
            string output = "_mvshow.drz";

            FileStream strin = File.OpenRead(input);
            DrzStream strout = new DrzStream(output, true);

            byte[] buff = new byte[4096];
            int read = 0;

            while(read < strin.Length)
            {
                int len = strin.Read(buff, 0, buff.Length);
                strout.Write(buff, 0, len);
                read += len;
                Console.Write($"\r{read}/{strin.Length}");
            }

            strin.Close();
            strout.Close();
        }
    }
}
