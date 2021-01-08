using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Resource
{
    public class ExtentionResource
    {
        static public string GetTextContent(string name)
        {
            Assembly myAssem = Assembly.GetExecutingAssembly();
            ResourceManager rm = new ResourceManager("Ventuz.Extention.Properties.Resources", myAssem);
            return rm.GetString(name);
        }

        static public byte[] GetBinaryContent(string name)
        {
            Assembly myAssem = Assembly.GetExecutingAssembly();
            ResourceManager rm = new ResourceManager("Ventuz.Extention.Properties.Resources", myAssem);
            return rm.GetObject(name) as byte[];
        }
    }
}
