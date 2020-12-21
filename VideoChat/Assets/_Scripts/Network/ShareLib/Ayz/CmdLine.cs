using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ShareLib.Log;

namespace ShareLib.Ayz
{
    public class CmdLine
    {
        public CmdLine(string cmdline)
        {
            msg = cmdline;
            string[] cmds = cmdline.Split(_sepchars);

            if (cmds.Length == 0)
            {
                return;
            }

            List<string> args = new List<string>();
            foreach(var cmd in cmds)
            {
                if (string.IsNullOrEmpty(cmd))
                    continue;

                args.Add(cmd);
            }

            if(args.Count == 0)
            {
                return;
            }

            _cmd = args[0];
            _args = new string[args.Count - 1];
            for(int i = 1; i < args.Count; ++i)
            {
                _args[i - 1] = args[i];
            }
        }

        public string cmd { get { return _cmd; } }
        public int argcount { get { return _args.Length; } }
        public bool getarg<T>(int i, out T r)
        {
            r = default(T);

            if (i >= _args.Length)
            {
                Logger.Error(string.Format("{0} 命令参数不足, 至少需要 {1} 个参数.", _cmd, i + 1) );
                return false;
            }

            try
            {
                r = (T)Convert.ChangeType(_args[i], typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                Logger.Error(string.Format("{0} 命令的第 {1} 个参数需要 {2}, 传递的是 {3}", _cmd, i + 1, typeof(T).Name, _args[i]));
                return false;
            }
        }

        public string msg { get; set; }
        
        private string _cmd;
        private string[] _args;
        private static readonly char[] _sepchars = { ' ', '\t', '\r', '\n'};
    }
}
