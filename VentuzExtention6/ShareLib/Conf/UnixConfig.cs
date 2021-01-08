using System;
using System.Collections.Generic;
using System.IO;
using ShareLib.Log;

namespace Tideinfo.ShareLib.Conf
{
    public class UnixConfig
    {
        public class ConfigItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Section
        {
            public string Name { get; set; }
            public string Args { get; set; }
            public List<ConfigItem> Configs { get; } = new List<ConfigItem>();
        }

        public UnixConfig(string filePath)
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
            }
            catch (Exception e)
            {
                Logger.Error($"LoadUnixConfig Exception: { e.Message }");
            }
        }

        public Section FindSection(string name, string cfkey, string cfvalue)
        {
            foreach(Section sec in Sections)
            {
                if(sec.Name != name)
                {
                    continue;
                }

                foreach(ConfigItem cf in sec.Configs)
                {
                    if(cf.Key == cfkey && cf.Value == cfvalue)
                    {
                        return sec;
                    }
                }
            }

            return null;
        }

        public IEnumerable<Section> GetSections(string name)
        {
            foreach(Section sec in Sections)
            {
                if(sec.Name == name)
                {
                    yield return sec;
                }
            }
        }

        private void AnalyzeLine(string line)
        {
            if(string.IsNullOrEmpty(line))
            {
                return;
            }

            if(line[0] == '#')
            {
                return;
            }

            line = line.Trim();
            if (line.Length < 2)
            {
                return;
            }

            if (line[0] == '<')
            {
                int pos = line.IndexOf('>');
                if (pos > 0)
                {
                    if(line[1] == '/')
                    {
                        string typeName = line.Substring(2, line.Length - 3);
                        if(_curSection != null && _curSection.Name == typeName)
                        {
                            Sections.Add(_curSection);
                            _curSection = null;
                            return;
                        }

                        throw new SystemException($"unexcept line { line }");
                    }

                    _curSection = new Section();
                    line = line.Substring(1, line.Length - 2);
                    string [] sepResult = line.Split(_spaceSplitor, 2);
                    _curSection.Name = sepResult[0];

                    if(sepResult.Length == 2)
                    {
                        _curSection.Args = sepResult[1];
                    }

                    return;
                }

                throw new SystemException($"no > match with < on line: { line }");
            }

            string[] sepCfg = line.Split(_spaceSplitor, 2);
            ConfigItem cfItem = new ConfigItem();
            cfItem.Key = sepCfg[0].Trim();
            if(sepCfg.Length > 1)
            {
                cfItem.Value = sepCfg[1].Trim();
            }

            if(_curSection != null)
            {
                _curSection.Configs.Add(cfItem);
            }
            else
            {
                GlobalSets.Add(cfItem);
            }
        }

        public List<Section> Sections { get; } = new List<Section>();
        public List<ConfigItem> GlobalSets { get; } = new List<ConfigItem>();
        private Section _curSection = null;
        private char[] _spaceSplitor = { ' ', '\t' };
    }
}
