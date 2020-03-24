using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ShareLib.Conf
{
    public class INIConfig
    {
        public INIConfig(string filePath)
        {
            try
            {
                using (StreamReader reader = File.OpenText(filePath))
                {
                    string str = reader.ReadLine();
                    while (str != null)
                    {
                        AnalyzeLine(str);
                        str = reader.ReadLine();
                    }
                }

                _filePath = filePath;
            }
            catch(Exception e)
            {
                Console.WriteLine("LoadConfig Exception:"+ e.Message);
            }
        }

        public string getconf(string domain, string key)
        {
            if(!_confData.ContainsKey(domain))
            {
                return "";
            }

            if(!_confData[domain].ContainsKey(key))
            {
                return "";
            }

            return _confData[domain][key];
        }

        public void setconf(string domain, string key, string value)
        {
            _confData[domain][key] = value;
        }

        public bool hasconf(string domain, string key)
        {
            return _confData.ContainsKey(domain) && _confData[domain].ContainsKey(key);
        }

        public T getconf<T>(string domain, string key)
        {
            string str = getconf(domain, key);

            try
            {
                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return default(T);
            }
        }

        public T getconf<T>(string domain, string key, T defval)
        {
            string str = getconf(domain, key);
            if(string.IsNullOrEmpty(str))
            {
                return defval;
            }

            try
            {
                if(typeof(T) == typeof(int))
                {
                    if(str.Length > 2 && str[0] == '0' && (str[1] == 'x' || str[1] == 'X'))
                    {
                        return (T)(object)Convert.ToInt32(str, 16);
                    }
                }

                return (T)Convert.ChangeType(str, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return defval;
            }
        }

        public T getenum<T>(string domain, string key)
        {
            string str = getconf(domain, key);
            return (T)Enum.Parse(typeof(T), str, true);
        }

        public T getenum<T>(string domain, string key, T def)
        {
            string str = getconf(domain, key);
            if(string.IsNullOrEmpty(str))
            {
                return def;
            }

            return (T)Enum.Parse(typeof(T), str, true);
        }

        public IEnumerable<string> DomainKeys(string domain)
        {
            if(_confData.ContainsKey(domain))
            {
                return _confData[domain].Keys;
            }

            return new string[0];
        }

        /// <summary>
        /// 测试使用
        /// </summary>
        public void Dump()
        {
            foreach(var dm in _confData)
            {
                System.Console.WriteLine(string.Format("[{0}]",dm.Key));
                foreach(var cf in dm.Value)
                {
                    System.Console.WriteLine(string.Format("{0}={1}", cf.Key, cf.Value));
                }
            }
        }

        public string FilePath { get { return _filePath; } }

        private void AnalyzeLine(string line)
        {
            if(line.Length < 2)
            {
                return;
            }

            if(line[0] == '#')
            {
                return;
            }

            if(line[0] == '/' && line[1] == '/')
            {
                return;
            }

            line = line.Trim();
            if(line.Length < 2)
            {
                return;
            }

            if(line[0] == '[')
            {
                int pos = line.IndexOf(']');
                if(pos > 0)
                {
                    _curDomain = line.Substring(1, pos - 1);
                }

                return;
            }

            int eqpos = line.IndexOf('=');
            string name = line.Substring(0, eqpos);
            string value = line.Substring(eqpos + 1);

            if(!_confData.ContainsKey(_curDomain))
            {
                _confData.Add(_curDomain, new Dictionary<string, string>());
            }

            _confData[_curDomain].Add(name, value);
        }

        private string _curDomain = "";
        private string _filePath;
        private Dictionary<string, Dictionary<string, string>> _confData = new Dictionary<string, Dictionary<string, string>>();
    }
}
