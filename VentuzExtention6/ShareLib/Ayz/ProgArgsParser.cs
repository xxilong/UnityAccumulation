using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Ayz
{
    public class ProgArgsParser
    {
        public ProgArgsParser(string[] args)
        {
            _args = args;
        }

        public ProgArgsParser()
        {
            _args = Environment.GetCommandLineArgs();
        }

        private bool HasOpt(string key)
        {
            string stdopt = "-" + key;
            string longopt = "--" + key;
            string winopt = "/" + key;

            foreach(string arg in _args)
            {
                if(arg == stdopt || arg == longopt || arg == winopt)
                {
                    return true;
                }             

                if(arg.StartsWith(winopt) && arg[winopt.Length] == ':')
                {
                    return true;
                }
            }

            return false;
        }

        private string GetOpt(string key)
        {
            string stdopt = "-" + key;
            string longopt = "--" + key;
            string winopt = "/" + key;

            for(int i = 0; i < _args.Length; ++i)
            {
                if(_args[i] == stdopt || _args[i] == longopt)
                {
                    ++i;
                    if(i < _args.Length)
                    {
                        string val = _args[i];
                        if(val.Length <= 0)
                        {
                            return string.Empty;
                        }

                        if(val[0] == '-' || val[0] == '/')
                        {
                            return string.Empty;
                        }

                        return val;
                    }

                    return string.Empty;
                }

                if(_args[i].StartsWith(winopt))
                {
                    string val = _args[i];
                    int optlen = winopt.Length;
                    if(val.Length < optlen + 1)
                    {
                        return string.Empty;
                    }

                    if(val[optlen] != ':')
                    {
                        continue;
                    }

                    return val.Substring(optlen + 1);
                }
            }

            return null;
        }

        private string[] _args = null;
    }
}
