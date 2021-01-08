using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Translate
{
    class TransFile
    {
        public TransFile(string filename)
        {
            this.fileName = filename;
        }

        public string tryTranslate(string str)
        {
            checkTransFileChanged();

            //using (StreamWriter fs = File.AppendText("re.txt"))
            //{
            //    fs.WriteLine(str);
            //}

            if (dict.ContainsKey(str))
            {
                return dict[str];
            }

            return "";
        }

        void checkTransFileChanged()
        {
            DateTime writeTime = (new FileInfo(fileName)).LastWriteTime;
            if(writeTime > lastLoadTime)
            {
                lastLoadTime = writeTime;
                dict.Clear();
                LoadTranslate();
            }
        }

        void LoadTranslate()
        {
            using (StreamReader fs = File.OpenText(fileName))
            {
                string key = null;

                while(true)
                {
                    string line = fs.ReadLine();
                    if(line == null)
                    {
                        break;
                    }

                    if(line.Length < 3)
                    {
                        continue;
                    }

                    if(line[0] == '>' && line[1] == '>')
                    {
                        key = line.Substring(2);
                    }
                    else if(line[0] == '-' && line[1] == '>')
                    {
                        dict.Add(key, line.Substring(2));
                        key = null;
                    }
                }
            }
        }

        private Dictionary<string, string> dict = new Dictionary<string, string>();
        private string fileName;
        private DateTime lastLoadTime = new DateTime(1900, 1, 1, 0, 0, 0);
    }
}
