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
using System.Diagnostics;

namespace WO
{
    public class MpvRpc
    {
        public void Start(string playfile)
        {
            _playfile = playfile;
            _isquited = false;

            INIConfig mpvcfg = new INIConfig(Path.Combine(PathHelp.dllDir, "mpv.ini"));
            string mpvpath = mpvcfg.getconf("mpv", "path");
            string mpvopt = mpvcfg.getconf("mpv", "opts");
            float subzoom = mpvcfg.getconf<float>("mpv", "subfac", 1.0f);
            int subpos = mpvcfg.getconf<int>("mpv", "subpos", 15);           
            mpvopt += $" --sub-file=s.srt --sub-pos={subpos} --sub-scale={subzoom} --fullscreen --keep-open-pause=no --keep-open=yes  --input-ipc-server=\\\\.\\pipe\\" + rpcPipeName;

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
                _mpvProcess = RunCommand.Run(mpvpath, mpvopt + " \"" + playfile + "\"");
                Logger.Debug($"[MPVPlayer] CreatedProcess null? {_mpvProcess == null}");
                _rpcWriter.Connect();
            }

            _rpcReader = new StreamReader(_rpcWriter, Encoding.UTF8);

            NamedPipeClientStream readerPipe = new NamedPipeClientStream(".", rpcPipeName, PipeDirection.In);
            readerPipe.Connect();
            _eventReader = new StreamReader(readerPipe, Encoding.UTF8);
            _recvThread = new Thread(MpvEventReciver);
            _recvThread.Start();
        }

        public void Play(string file)
        {
            if(_isquited)
            {
                Start(file);
            }
            else
            {
                _playfile = file;
                Logger.Debug($"play file {file}");
                Request($"loadfile {file}");
            }
        }

        public void Close()
        {
            if(_recvThread != null)
            {
                _recvThread.Abort();
                _recvThread = null;
            }

            if(_mpvProcess != null)
            {
                try
                {
                    _mpvProcess.CloseMainWindow();
                    _mpvProcess.Close();
                    _mpvProcess.Dispose();
                }
                catch(Exception e)
                {
                    Logger.Error($"[MPVPlayer] close process error: {e.Message}");
                }

                _mpvProcess = null;
                Thread.Sleep(5000);
            }

            if(_eventReader != null)
            {
                _eventReader.Close();
                _eventReader.Dispose();
                _eventReader = null;
            }

            if(_rpcReader != null)
            {
                _rpcReader.Close();
                _rpcReader.Dispose();
                _rpcReader = null;
            }

            if(_rpcWriter != null)
            {
                _rpcWriter.Close();
                _rpcWriter.Dispose();
                _rpcWriter = null;
            }
        }

        public IntPtr Window
        {
            get
            {
                Logger.Debug($"[MPVPlayer] Process {_mpvProcess}");
                Logger.Debug($"[MPVPlayer] Process {_mpvProcess == null}");
                //Logger.Debug($"[MPVPlayer] MainWindow {_mpvProcess.MainWindowHandle}");
                return _mpvProcess != null ? _mpvProcess.MainWindowHandle : IntPtr.Zero;
            }
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

        public bool HasEnd()
        {
            double dur = QueryFloatProperty("duration");
            double pos = QueryFloatProperty("time-pos");

            return (dur - pos) < 0.5f;
        }

        public bool IsLiveFailed()
        {
            return _isquited;
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

                string ev = jsobj["event"].ToString();

                Console.WriteLine($"[MPVPlayer][{ev}] -- " + evline);
                _EventReciver?.Invoke(jsobj);                
            }

            Logger.Info("[MPVPlayer] event read thread quited.");
            _isquited = true;
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
        private Process _mpvProcess;
        private Thread _recvThread;
        private string _playfile;
        private bool _isquited = false;

        private object reqlocker = new object();
        private Action<Dictionary<string, object>> _EventReciver = null;
    }
}
