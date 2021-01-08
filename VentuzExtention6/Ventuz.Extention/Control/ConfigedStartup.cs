using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Conf;
using ShareLib.Ports.QXSandTable;
using ShareLib.Unity;
using ShareLib.Page;
using ShareLib.Packer;
using Ventuz.Extention.Compatible;

namespace Ventuz.Extention.Control
{
    public class ConfigedStartup
    {
        public static ConfigedStartup Instance = new ConfigedStartup();

        public void  StartUp()
        {
            if(_scripts_reader == null)
            {
                _scripts_reader = VentuzWare.Instance.GetReader("scripts");
            }

            if(GlobalConf.getconf<bool>("startup", "ctrlserv", false))
            {
                ControlServer.Instance.Start(3131);
            }

            if (GlobalConf.getconf<bool>("startup", "ctrlclient", false))
            {
                ClientConnectError = true;
                ControlClient.Instance.ConnectStatus = ConnectResult;
                ControlClient.Instance.Connect(
                    GlobalConf.getconf("ctrlclient", "ip", "127.0.0.1"),
                    GlobalConf.getconf("ctrlclient", "port", 3131));
            }

            if (GlobalConf.getconf<bool>("startup", "serialport", false))
            {
                QXSTSerialPort.Instance.Open();
            }

            if(GlobalConf.getconf<bool>("startup", "pages", false))
            {
                Page.Control.InitPages(_scripts_reader.ReadAllText("pages.txt"));
                Page.Control.GotoPage(0);
            }

            if(GlobalConf.getconf<bool>("startup", "idle", false))
            {
                _idle_time = GlobalConf.getconf<int>("idle", "time", 60 * 5);
                _idle_page = GlobalConf.getconf<string>("idle", "page", "");
                Delay.Run(_idle_time * 1000, OnIdleTimeOut);
            }
        }

        public void ReadloadPages()
        {
            Page.Control.InitPages(_scripts_reader.ReadAllText("pages.txt"));
            Page.Control.GotoPage(0);
        }

        private void ConnectResult(bool success)
        {
            if (success)
            {
                ClientConnectError = false;
            }
            else
            {
                ClientConnectError = true;
                Delay.Run(3000, () =>
                {
                    ControlClient.Instance.ReConnect();
                });
            }

            _StateChanged?.Invoke();
        }

        private void OnIdleTimeOut()
        {
            long now = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            long idletime = now - ControlServer.Instance.Server.LastRecvTime;
            if(idletime >= _idle_time)
            {
                Page.Control.GotoPage(_idle_page);
                Delay.Run(_idle_time * 1000, OnIdleTimeOut);
            }
            else
            {
                Delay.Run((int)(_idle_time - idletime) * 1000, OnIdleTimeOut);
            }
        }

        public bool ClientConnectError = false;
        public bool ServerNoClient = false;

        public void SetStateChangeListener(Action act) => _StateChanged = act;
        private Action _StateChanged;
        private IFileReader _scripts_reader = null;

        private int _idle_time;
        private string _idle_page; 
    }
}
