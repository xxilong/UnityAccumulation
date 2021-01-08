using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Conf
{
    /* 只在 Ventuz.Extention.dll 中使用, 用于保存 Designer 相关的界面操作配置
     * 配置文件为 Ventuz 目录下的 Ventuz.Extention.ini
     * 不建议在项目中使用此配置文件, 项目中应该统一使用 GlobalConf 来作为项目的配置
     */
    public class ModConfig : INIConfig
    {
        private ModConfig() :
            base(Path.ChangeExtension(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".ini"))
        {
        }
        public static new void setconf(string domain, string key, string value)
        {
            INIWriter wr = new INIWriter(_instance.FilePath);
            wr.SetConf(domain, key, value);
            wr.Save();
            _instance.setconf(domain, key, value);
        }

        public static new bool hasconf(string domain, string key)
        {
            return _instance.hasconf(domain, key);
        }

        public static new string getconf(string domain, string key)
        {
            return _instance.getconf(domain, key);
        }

        public static string getconf_aspath(string domain, string key)
        {
            string path = _instance.getconf(domain, key);
            if(path.Contains(":"))
            {
                return path;
            }
            else
            {
                return Path.Combine(Path.GetDirectoryName(_instance.FilePath), path);
            }
        }

        public static new T getconf<T>(string domain, string key)
        {
            return _instance.getconf<T>(domain, key);
        }

        public static new T getconf<T>(string domain, string key, T defval)
        {
            return _instance.getconf<T>(domain, key, defval);
        }

        public static new T getenum<T>(string domain, string key)
        {
            return _instance.getenum<T>(domain, key);
        }

        public static new T getenum<T>(string domain, string key, T defval)
        {
            return _instance.getenum<T>(domain, key, defval);
        }

        public static T trygets<T>(string domain, List<string> keys, T defval)
        {
            foreach(string key in keys)
            {
                if(hasconf(domain, key))
                {
                    return getconf<T>(domain, key);
                }
            }

            return defval;
        }

        public static new void Dump()
        {
            _instance.Dump();
        }

        public static INIConfig _instance = new ModConfig();
    }
}
