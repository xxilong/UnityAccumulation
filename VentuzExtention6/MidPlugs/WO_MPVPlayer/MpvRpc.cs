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
            _curplay = playfile;
            INIConfig mpvcfg = new INIConfig(Path.Combine(PathHelp.dllDir, "mpv.ini"));
            string mpvpath = mpvcfg.getconf("mpv", "path");
            string mpvopt = mpvcfg.getconf("mpv", "opts");
            mpvopt += " --input-ipc-server=\\\\.\\pipe\\" + rpcPipeName;

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

        public void Play(string playfile)
        {
            _curplay = playfile;

            if(_rpcWriter == null)
            {
                Start(playfile);
                return;
            }

            string file = playfile;
            file = "file://" + file.Replace("\\", "/");
            Request($"loadfile {file}");
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
                try
                {
                    _rpcWriter.Write(reqdata, 0, reqdata.Length);
                    _rpcWriter.Flush();
                }
                catch(Exception e)
                {
                    Logger.Error($"播放器通信异常: {e.Message}");
                }
                
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

                Console.WriteLine("[MPVPlayer] -- " + evline);
                _EventReciver?.Invoke(jsobj);                
            }

            Logger.Info("[MPVPlayer] event read thread quited.");
        }

        private string CommandLineToMpvCommand(string cmdline)
        {
            string[] cmds = StrUnity.SpritString(cmdline);
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
        private string _curplay;

        private object reqlocker = new object();
        private Action<Dictionary<string, object>> _EventReciver = null;
    }
}
