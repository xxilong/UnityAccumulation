using ShareLib.Net;
using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjMedical
{
    class LLClientManager
    {
        public void StartConnect(string name, string ip, int port)
        {
            PageControlClient client = new PageControlClient();
            client.ConnectStatus = (bool s) => {
                if(s)
                {
                    _clients[name] = client;
                    Logger.Info($"Connect {name} successes.");
                }
                else
                {
                    _clients.Remove(name);
                    Logger.Warning($"Connect {name} failed.");
                    Delay.Run(1000 * 10, () => {
                        client.ReConnect();
                    });
                }
            };

            client.Connect(ip, port);
        }

        public PageControlClient FindClient(string name)
        {
            if(_clients.ContainsKey(name))
            {
                return _clients[name];
            }

            return null;
        }

        public void RunShutdown()
        {
            foreach(var item in _clients)
            {
                item.Value.SendCommand("#shutdown");
            }
        }

        public bool IsEmpty()
        {
            return _clients.Count == 0;
        }

        public void DumpList()
        {
            Console.WriteLine($"{_clients.Count} Connect PageControl Servers:");
            foreach(var item in _clients)
            {
                Console.WriteLine($"{item.Key}");
            }
        }

        internal Dictionary<string, PageControlClient> _clients = new Dictionary<string, PageControlClient>();
    }
}
