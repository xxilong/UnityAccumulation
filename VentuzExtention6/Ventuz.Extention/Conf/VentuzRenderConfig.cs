using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Resource;

namespace Ventuz.Extention.Conf
{
    public class VentuzRenderConfig
    {
        public VentuzRenderConfig(string name)
        {
            vsrpath = Path.Combine(FilePaths.pakCfgDir, "RenderSetup", name + ".vrs");
            if(!File.Exists(vsrpath))
            {
                SetResolution(1920, 1080);
            }
        }

        public void SetResolution(int w, int h)
        {
            string str = ExtentionResource.GetTextContent("InitRenderSetup");
            str = str.Replace("5678", w.ToString()).Replace("8765", h.ToString());
            File.WriteAllText(vsrpath, str);
        }

        private string vsrpath;
    }
}
