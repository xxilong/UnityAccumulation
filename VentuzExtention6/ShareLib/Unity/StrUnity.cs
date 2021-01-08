using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareLib.Unity
{
    public static class StrUnity
    {
        public static string[] SpritString(string val)
        {
            List<string> ret = new List<string>();
            string[] parts = Regex.Split(val, "\"([^\"]*?)\"|(\\S+)");

            foreach (string v in parts)
            {
                string y = v.Trim();
                if (!string.IsNullOrEmpty(y))
                {
                    ret.Add(y);
                }
            }

            return ret.ToArray();
        }
    }
}
