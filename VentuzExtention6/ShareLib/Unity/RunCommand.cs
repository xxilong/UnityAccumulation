using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ShareLib.Unity
{
    public class RunCommand
    {
        public static Process Run(string file, string argument)
        {
            Process p = new Process();
            p.StartInfo.Arguments = argument;
            p.StartInfo.FileName = file;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            Logger.Debug($"RunCommand: {file} {argument}");
            return p;
        }
    }
}
