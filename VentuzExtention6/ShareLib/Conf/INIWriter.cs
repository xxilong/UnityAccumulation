using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Conf
{
    public class INIWriter
    {
        public INIWriter(string filePath)
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

                _FilePath = filePath;
            }
            catch (Exception e)
            {
                Console.WriteLine($"LoadConfig Exception: { e.Message }");
            }
        }

        public void Save()
        {
            if(string.IsNullOrEmpty(_FilePath))
            {
                return;
            }

            using (StreamWriter s = new StreamWriter(_FilePath))
            {
                foreach(var item in _ConfFile)
                {
                    s.WriteLine(item.lineText);
                }
            }
        }
        
        public void SetConf(string domain, string key, string value)
        {
            for(int i = 0; i < _ConfFile.Count; ++i)
            {
                if(_ConfFile[i].inDomain == domain &&
                    _ConfFile[i].keyName == key)
                {
                    _ConfFile[i].lineText = $"{key}={value}";
                    return;
                }
            }

            for(int i = 0; i < _ConfFile.Count; ++i)
            {
                if(_ConfFile[i].inDomain == domain)
                {
                    ConfLine cl = new ConfLine();
                    cl.lineText = $"{key}={value}";
                    cl.keyName = key;
                    cl.inDomain = domain;
                    _ConfFile.Insert(i + 1, cl);
                    return;
                }
            }

            ConfLine c1 = new ConfLine();
            ConfLine c2 = new ConfLine();
            c1.lineText = $"[{domain}]";
            c1.inDomain = domain;
            c2.inDomain = domain;
            c2.keyName = key;
            c2.lineText = $"{key}={value}";
            _ConfFile.Add(c1);
            _ConfFile.Add(c2);
        }

        private void AnalyzeLine(string line)
        {
            ConfLine cl = new ConfLine();
            cl.lineText = line;
            cl.inDomain = _CurDomain;
            _ConfFile.Add(cl);

            if (line.Length < 2)
            {
                return;
            }

            if (line[0] == '#')
            {
                return;
            }

            if (line[0] == '/' && line[1] == '/')
            {
                return;
            }

            line = line.Trim();
            if (line.Length < 2)
            {
                return;
            }

            if (line[0] == '[')
            {
                int pos = line.IndexOf(']');
                if (pos > 0)
                {
                    _CurDomain = line.Substring(1, pos - 1);
                    cl.inDomain = _CurDomain;
                }

                return;
            }

            int eqpos = line.IndexOf('=');
            string name = line.Substring(0, eqpos);
            cl.keyName = name;            
        }

        private class ConfLine
        {
            public string lineText = "";
            public string inDomain = "";
            public string keyName = "";
        }

        private string _CurDomain = "";
        private string _FilePath;
        private List<ConfLine> _ConfFile = new List<ConfLine>();
    }
}
