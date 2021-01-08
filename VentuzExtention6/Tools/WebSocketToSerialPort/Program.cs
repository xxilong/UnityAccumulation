using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Conf;
using ShareLib.Log;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareLib.Ports.QXSandTable;

namespace WebSocketToSerialPort
{
    class Program
    {
        static void Main(string[] args)
        {
            _instance.RealMain();
        }

        private void RealMain()
        {
            ShareLib.Log.Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
            });

            string url = GlobalConf.getconf<string>("server", "url");
            ShareLib.Log.Logger.Info($"Server: {url}");
            LoadForwardConfigs();

            QXSTSerialPort.Instance = new QXSTSerialPort(new NackedPackProtocol());
            QXSTSerialPort.Instance.Open();

            ws = new WebSocket(url);
            ws.OnMessage += OnMessage;
            ws.EmitOnPing = true;
            ws.Connect();
            
            while(true)
            {
                Console.ReadLine();
            }
        }

        private void OnMessage(object s, MessageEventArgs e)
        {
            JObject json = JsonConvert.DeserializeObject<JObject>(e.Data);

            string cmd = json["cmd"].Value<string>();
            string sender = json["sender"].Value<string>();
            JObject data = json["data"].Value<JObject>();

            foreach(var item in _forwardcmds)
            {
                if(item.Key.cmd != cmd)
                {
                    continue;
                }

                if(item.Key.sender != "*" &&
                    item.Key.sender != sender)
                {
                    continue;
                }

                QXSTSerialPort.Instance.SendRawData(item.Value);
                return;
            }

            ShareLib.Log.Logger.Debug($"Unhandled command {sender}:{cmd}");
        }

        private void LoadForwardConfigs()
        {
            foreach (string key in GlobalConf.DomainKeys("cmds"))
            {
                string data = GlobalConf.getconf<string>("cmds", key);
                string[] cmds = key.Split(':');
                if (cmds.Length != 2)
                {
                    ShareLib.Log.Logger.Error($"配置错误 [cmds]->{key} 格式应该为 sender:cmd=data 或 *:cmd=data");
                    continue;
                }

                CmdIdentiry cmd;
                cmd.sender = cmds[0];
                cmd.cmd = cmds[1];
                _forwardcmds[cmd] = data;                
            }

            ShareLib.Log.Logger.Info($"Loaded {_forwardcmds.Count } commands.");
        }

        struct CmdIdentiry
        {
            public string sender;
            public string cmd;
        }

        private WebSocket ws = null;
        private Dictionary<CmdIdentiry, string> _forwardcmds = new Dictionary<CmdIdentiry, string>();

        static Program _instance = new Program();
    }
}
