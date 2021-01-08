using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Ayz;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidCtrl
{
    public class WOPlugIndepend<T> where T: WOPlugIndepend<T>, new()
    {
        public virtual bool init() { return true; }
        public virtual bool show() { return true; }
        public virtual bool hide() { return true; }
        public virtual void destory() { }
        public virtual void OnRecvCommand(string cmd) { }
        public virtual void reset()
        {
            destory();
            init();
        }

        // 可供使用的功能
        public void SendCommand(string cmd)
        {
            _srv.SendCommand(_id + " " + cmd);
        }

        public string GetArgument(int n)
        {
            if(n + 1 < _args.Count)
            {
                string arg = _args[n + 1];
                arg = arg.Replace("$(space)", " ");
                return arg;
            }

            return "";
        }

        // Call From Main
        static public void CSharpInitalize(string[] args)
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new UDPBroadCastLog(),
            });

            if (args.Length < 1)
            {
                Logger.Error("[PPTPlayer] argument not enough");
                return;
            }

            int.TryParse(args[0], out int port);
            _srv.SetReciverListener(OnRecvAdminCommand);
            _srv.Start(port);
        }

        static public void OnRecvAdminCommand(string cmdline, string peer)
        {
            Logger.Debug($"[WOPlug] Recv {cmdline}");
            CmdLine cmd = new CmdLine(cmdline);
            if(!cmd.getarg(0, out string id))
            {
                Logger.Error($"Admin Command Error: {cmdline}");
                return;
            }

            if(cmd.cmd != "new" && !_instances.ContainsKey(id))
            {
                _instances[id] = new T();
                _instances[id]._id = id;
                Logger.Warning($"Admin Command Error, id not found, renew");
            }

            switch(cmd.cmd)
            {
                case "new":
                    _instances[id] = new T();
                    _instances[id]._id = id;
                    break;

                case "init":
                    _instances[id]._args = new List<string>(cmd.args);
                    _instances[id].init();
                    break;

                case "show":
                    _instances[id].show();
                    break;

                case "hide":
                    _instances[id].hide();
                    break;

                case "destory":
                    _instances[id].destory();
                    break;

                case "update":
                    _instances[id].destory();
                    _instances[id].init();
                    break;

                case "reset":
                    _instances[id]._args = new List<string>(cmd.args);
                    _instances[id].reset();
                    break;

                case "send":
                    _instances[id].OnRecvCommand(cmd.rightline(1));
                    break;

                case "delete":
                    _instances.Remove(id);
                    break;

                default:
                    break;
            }
        }

        static CmdTcpServer _srv = new CmdTcpServer();
        static Dictionary<string, T> _instances = new Dictionary<string, T>();
        private List<string> _args = new List<string>();
        private string _id = "";
    }
}
