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
        public string argsline => string.Join(" ", _args).TrimStart(' ');
        public string[] args => _args;
        public string rightline(int n)
        {
            return string.Join(" ", _args, n, _args.Length - n);
        }

        // 参数从 0 开始编号
        public bool getarg<T>(int i, out T r)
        {
            r = default(T);

            if (i >= _args.Length)
            {
                Logger.Error($"{_cmd} 命令参数不足, 至少需要 {i + 1} 个参数.");
                return false;
            }

            try
            {
                r = (T)Convert.ChangeType(_args[i], typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                Logger.Error($"{_cmd} 命令的第 {i + 1} 个参数需要 {typeof(T).Name}, 传递的是 { _args[i] }");
                return false;
            }
        }

        private string _cmd;
        private string[] _args;
        private static readonly char[] _sepchars = { ' ', '\t', '\r', '\n'};
    }
}
