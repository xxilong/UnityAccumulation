using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Unity
{
    class Machine
    {
        public static void shutdown()
        {
            System.Diagnostics.Process.Start("cmd.exe", "/c shutdown -s -t 10");
        }
    }
}
