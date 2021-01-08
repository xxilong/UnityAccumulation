using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using ShareLib.Conf;
using ShareLib.Unity;
using System.Threading;
using Newtonsoft.Json;
using ShareLib.Log;

namespace MoviePlayer
{
    public class MpvRpc
    {
        public void Start()
        {
            string mpvPath = GlobalConf.getconf_aspath("mpv", "path");
            mpvPath = Path.Combine(PathHelp.appDir, mpvPath);
            string mpvOpt = GlobalConf.getconf("mpv", "opts");
            mpvOpt += " --input-ipc-server=\\\\.\\pipe\\" + rpcPipeName;

            _rpcWriter = new NamedPipeClientStream(".", rpcPipeName, PipeDirection.InOut);
            try
            {
                _rpcWriter.Connect(500);
            }
            catch (TimeoutException)
            {
            }

            if (!_rpcWriter.IsConnected)
            {
                RunCommand.Run(mpvPath, mpvOpt);
                _rpcWriter.Connect();
            }

            _rpcReader = new StreamReader(_rpcWriter, Encoding.UTF8);

            NamedPipeClientStream readerPipe = new NamedPipeClientStream(".", rpcPipeName, PipeDirection.In);
            readerPipe.Connect();
            _eventReader = new StreamReader(readerPipe, Encoding.UTF8);
            new Thread(MpvEventReciver).Start();
        }

        public object Request(string reqline)
        {
            if (string.IsNullOrEmpty(reqline))
            {
                return null;
            }

            string jsreq = CommandLineToMpvCommand(reqline);

            if (string.IsNullOrEmpty(jsreq))
            {
                return null;
            }

            byte[] reqdata = Encoding.UTF8.GetBytes(jsreq + "\n");

            lock (reqlocker)
            {
                _rpcWriter.Write(reqdata, 0, reqdata.Length);
                _rpcWriter.Flush();

                while (true)
                {
                    string res = _rpcReader.ReadLine();
                    var jsobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(res);
                    if (jsobj.ContainsKey("event"))
                    {
                        continue;
                    }

                    if (jsobj.ContainsKey("error"))
                    {
                         if (jsobj.ContainsKey("data"))
                        {
                            return jsobj["data"];
                        }

                        return "";
                    }
                }
            }
        }

        public void SetEventListener(Action<Dictionary<string, object>> act) => _EventReciver = act;

        public string QueryStrProperty(string name)
        {
            object r = Request("get_property " + name);
            return r == null ? "" : r.ToString();
        }

        public bool QueryBoolProperty(string name)
        {
            object r = Request("get_property " + name);
            return r == null ? false : (bool)r;
        }

        public double QueryFloatProperty(string name)
        {
            object r = Request("get_property " + name);
            if (r == null)
            {
                return 0;
            }

            if (double.TryParse(r.ToString(), out double val))
            {
                return val;
            }

            return 0;
        }

        private void MpvEventReciver()
        {
            while (!_eventReader.EndOfStream)
            {
                string evline = _eventReader.ReadLine();
                var jsobj = JsonConvert.DeserializeObject<Dictionary<string, object>>(evline);
                if (!jsobj.ContainsKey("event"))
                {
                    continue;
                }

                Console.WriteLine("\r--- " + evline);
                Console.Write(">>> ");

                _EventReciver?.Invoke(jsobj);                
            }

            Logger.Info("MPV event read thread quited.");
        }

        private string CommandLineToMpvCommand(string cmdline)
        {
            string[] cmds = cmdline.Trim().Split(' ');
            if (cmds.Length < 0)
            {
                return null;
            }

            string json = "{\"command\":[";
            bool hasCmd = false;
            foreach (string v in cmds)
            {
                string val = v.Trim();
                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                if (!hasCmd)
                {
                    hasCmd = true;
                    json += "\"" + val + "\"";
                    continue;
                }

                json += ",";
                if (IsNativeValue(val))
                {
                    json += val;
                }
                else
                {
                    json += "\"" + val + "\"";
                }
            }

            if (!hasCmd)
            {
                return null;
            }

            return json + "]}";
        }

        private bool IsNativeValue(string val)
        {
            if (val == "true" || val == "false")
            {
                return true;
            }

            if (double.TryParse(val, out double v))
            {
                return true;
            }

            return false;
        }

        private readonly string rpcPipeName = "mpv-control-pipe";
        private NamedPipeClientStream _rpcWriter;
        private StreamReader _rpcReader;
        private StreamReader _eventReader;

        private object reqlocker = new object();
        private Action<Dictionary<string, object>> _EventReciver = null;
    }
}
