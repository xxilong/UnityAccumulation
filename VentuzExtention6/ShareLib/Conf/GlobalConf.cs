using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Conf
{
    public class GlobalConf
    {
        static GlobalConf()
        {
            _watcher.Changed += OnFileChanged;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;
        }

        [Obsolete]
        public static EventHandler<int> OnReset;
        public static EventHandler<int> OnConfFileChange;

        [Obsolete("在6.3之后的版本中不应该再主动设置配置文件名,默认配置文件为项目的 Scripts/config.ini,打包后的路径为播放器目录下的 _mvshow.ini")]
        public static void Set(string path)
        {
            SetWatchFile(path);
            INIConfig conf = new INIConfig(path);
            AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", conf);
            OnReset?.Invoke(null, 0);
            OnConfFileChange?.Invoke(conf, 0);
        }

        // 不建议在项目中改变此设置, 应该有 Ventuz.Extention.dll 进行设置
        public static void SetGlobalFileGetter(Func<string> getter)
        {
            _cfgPathGetter = getter;
            AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", null);
        }

        public static void SetGlobalStreamGetter(Func<Stream> getter)
        {
            _cfgStreamGetter = getter;
            AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", null);
        }

        private static INIConfig _instance
        {
            get
            {
                INIConfig conf = AppDomain.CurrentDomain.GetData("INI_APP_CONFIG_FILENAME_K20181229") as INIConfig;
                if(conf == null)
                {
                    if(_cfgStreamGetter != null)
                    {
                        conf = new INIConfig(_cfgStreamGetter());
                        UnWatchFile();
                    }
                    else
                    {
                        string path = _cfgPathGetter();
                        SetWatchFile(path);
                        conf = new INIConfig(path);
                    }
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
                    Logger.Warning($"@( 没有匹配的 ), 在 {val} 中.");
                    break;
                }

                string args = val.Substring(p + 2, p2 - p - 2);
                string repm = val.Substring(p, p2 - p + 1);
                string[] pair = args.Split('/');

                if (pair.Length != 2)
                {
                    Logger.Warning($"@() 内缺少 /, 在 {val} 中.");
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

        private static Func<string> _cfgPathGetter = () => Path.ChangeExtension(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName, ".ini");
        private static Func<Stream> _cfgStreamGetter = null;
        private static FileSystemWatcher _watcher = new FileSystemWatcher();
        private static int lastChangeTime = 0;

        private static void SetWatchFile(string filepath)
        {          
            _watcher.Path = Path.GetDirectoryName(filepath);
            _watcher.Filter = Path.GetFileName(filepath);
            _watcher.EnableRaisingEvents = true;
        }

        private static void UnWatchFile()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            int now = Environment.TickCount;
            if (now - lastChangeTime < 500)
            {
                return;
            }

            lastChangeTime = now;

            Delay.Run(100, () =>
            {
                INIConfig conf = AppDomain.CurrentDomain.GetData("INI_APP_CONFIG_FILENAME_K20181229") as INIConfig;
                if (conf != null)
                {
                    Logger.Info("Global ConfigFile Modified.");
                    string path = conf.FilePath;
                    conf = new INIConfig(path);
                    AppDomain.CurrentDomain.SetData("INI_APP_CONFIG_FILENAME_K20181229", conf);
                    OnConfFileChange?.Invoke(conf, 1);
                }
            });
        }
    }
}
