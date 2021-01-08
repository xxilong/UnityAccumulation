using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Conf
{
    class OverideSystemConf
    {
        internal static OverideSystemConf instance = new OverideSystemConf();

        public OverideSystemConf()
        {
            InitHanders();
        }

        public void TryReplaceConf()
        {
            string sysini = Path.Combine(FilePaths.ventuzDir, "_system.ini");
            if (!File.Exists(sysini))
            {
                return;
            }

            INIConfig inifile = new INIConfig(sysini);
            foreach(var h in _handers)
            {
                if(!inifile.hasconf(h.Key.Item1, h.Key.Item2))
                {
                    continue;
                }

                string value = inifile.getconf(h.Key.Item1, h.Key.Item2);
                if(string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                h.Value(value);
            }

            int index = 0;
            while(inifile.hasconf("video", $"v{index}"))
            {
                string camid = inifile.getconf("video", $"v{index}");
                VentuzConfig.instance.AVConfig.VideoIn.AddInputVideo(camid);
                ++index;
            }
        }

        private Dictionary<Tuple<string, string>, Action<string>> _handers = new Dictionary<Tuple<string, string>, Action<string>>();
        private void Reg(string domain, string key, Action<string> hander) => _handers[new Tuple<string, string>(domain ,key)] = hander;
        private void InitHanders()
        {
            Reg("view", "cursor", (string value) => { VentuzConfig.instance.AVConfig.RenderOut.ShowCursor(value == "true" || value == "1"); });
            Reg("view", "resolving", SetResolving);
            Reg("view", "screen", (string value) => { VentuzConfig.instance.AVConfig.RenderOut.SetScreen(int.Parse(value)); });
        }

        private void SetResolving(string value)
        {
            if(value == "auto")
            {
                Logger.Debug($"OCFG: Render Setup Set To None.");
                VentuzConfig.instance.RenderSetupName = string.Empty;
            }
            else
            {
                string[] vl = value.Split('x');
                if(vl.Length != 2)
                {
                    Logger.Warning($"OCFG: resolving should be split by single x");
                    return;
                }

                int w = 0, h = 0;
                int.TryParse(vl[0], out w);
                int.TryParse(vl[1], out h);
                if(w == 0 || h == 0)
                {
                    Logger.Warning($"OCFG: resolving value error.");
                    return;
                }

                Logger.Debug($"OCFG: set resolving to {w}x{h}");
                VentuzConfig.instance.RenderConfig.SetResolution(w, h);
            }
        }
    }
}
