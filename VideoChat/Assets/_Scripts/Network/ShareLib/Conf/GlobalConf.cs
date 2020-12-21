using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Conf
{
    public class GlobalConf
    {
        public static Action<int> OnReset;

        public static void Set(string path)
        {
            INIConfig conf = new INIConfig(path);
            AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", conf);
            if(OnReset!=null) OnReset.Invoke(0); 
        }

        private static INIConfig _instance
        {
            get
            {
                INIConfig conf = AppDomain.CurrentDomain.GetData("INI_APP_CONFIG_FILENAME_K20181229") as INIConfig;
                if(conf == null)
                {
                    conf = new INIConfig(Path.ChangeExtension(
                        System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, ".ini"));
                    AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", conf);
                }
                return conf;
            }
        }

        public static void setconf(string domain, string key, string value)
        {
            INIWriter wr = new INIWriter(_instance.FilePath);
            wr.SetConf(domain, key, value);
            wr.Save();
            _instance.setconf(domain, key, value);
        }

        public static bool hasconf(string domain, string key)
        {
            return _instance.hasconf(domain, key);
        }

        public static string getconf(string domain, string key)
        {
            return _instance.getconf(domain, key);
        }

        public static string getconf_aspath(string domain, string key)
        {
            string path = _instance.getconf(domain, key);
            if (path.Contains(":"))
            {
                return path;
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(_instance.FilePath), path);
            }
        }

        public static T getconf<T>(string domain, string key)
        {
            return _instance.getconf<T>(domain, key);
        }

        public static T getconf<T>(string domain, string key, T defval)
        {
            return _instance.getconf<T>(domain, key, defval);
        }

        public static T getenum<T>(string domain, string key)
        {
            return _instance.getenum<T>(domain, key);
        }

        public static T getenum<T>(string domain, string key, T defval)
        {
            return _instance.getenum<T>(domain, key, defval);
        }

        public static T trygets<T>(string domain, List<string> keys, T defval)
        {
            foreach (string key in keys)
            {
                if (hasconf(domain, key))
                {
                    return getconf<T>(domain, key);
                }
            }

            return defval;
        }

        public static IEnumerable<string> DomainKeys(string domain)
        {
            return _instance.DomainKeys(domain);
        }

        public static string ReplaceAtGramma(string val)
        {
            int p = val.IndexOf("@(");
            while (p != -1)
            {
                int p2 = val.IndexOf(')', p);
                if (p2 == -1)
                {
                    Logger.Warning(string .Format("@( 没有匹配的 ), 在 {0} 中.", val));
                    break;
                }

                string args = val.Substring(p + 2, p2 - p - 2);
                string repm = val.Substring(p, p2 - p + 1);
                string[] pair = args.Split('/');

                if (pair.Length != 2)
                {
                    Logger.Warning(string.Format("@() 内缺少 /, 在 {0} 中.", val));
                    break;
                }

                var domain = pair[0].Trim();
                var key = pair[1].Trim();

                string cfgval = getconf<string>(domain, key);
                val = val.Replace(repm, cfgval);

                p = val.IndexOf("@(");
            }

            return val;
        }

        public static void Dump()
        {
            _instance.Dump();
        }
    }
}
