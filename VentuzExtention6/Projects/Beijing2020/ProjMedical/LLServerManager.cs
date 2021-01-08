using ShareLib.Net;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ProjMedical
{
    struct ClientItem : IComparable<ClientItem>
    {
        public int order;
        public int srvPort;
        public string peerName;
        public string showName;
        public string srvIP;

        int IComparable<ClientItem>.CompareTo(ClientItem other)
        {
            return order.CompareTo(other.order);
        }
    }

    /* LL = Lower Level
     * 
     */
    class LLServerManager
    {
        //_subItems["12323"] = new ClientItem { srvPort = 3000, peerName = "123", showName = "456", srvIP = "192.168.1.22" };
        public void Init(PageControlServer srv, DynamicServer dyn)
        {
            _srv = srv;
            _dyn = dyn;
        }

        public void AddSubServer(string peer, string name, int port, int order)
        {
            lock(locker)
            {
                string ip = peer.Substring(0, peer.IndexOf(':'));
                _subItems[peer] = new ClientItem { order = order, srvPort = port, peerName = peer, showName = name, srvIP = ip };
            }
            UpdateDynPage();
        }

        public void RemoveSubServer(string peer)
        {
            lock(locker)
            {
                if (_subItems.ContainsKey(peer))
                {
                    _subItems.Remove(peer);
                    UpdateDynPage();
                }
            }
        }

        public void RunShutdown()
        {
            _srv.SendCommand("#shutdown");            
        }
        
        public ClientItem[] SubItems
        {
            get
            {
                ClientItem[] ret = null;

                lock (locker)
                {
                    ret = new ClientItem[_subItems.Count];
                    _subItems.Values.CopyTo(ret, 0);
                }

                Array.Sort<ClientItem>(ret);
                return ret;
            }
        }

        public bool IsEmpty()
        {
            return _subItems.Count == 0;
        }

        public ClientItem? FindClient(string name)
        {
            foreach(var item in _subItems)
            {
                if(item.Value.showName == name)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public void DumpList()
        {
            Console.WriteLine($"{_subItems.Count} Connected Clients:");
            foreach(var item in _subItems)
            {
                Console.WriteLine($"{item.Key}: {item.Value.peerName}");
            }
        }

        private void UpdateDynPage()
        {
            if(_dyn == null)
            {
                return;
            }

            Delay.Run(50, () => { _dyn.SendDynPage(); });
        }

        private Dictionary<string, ClientItem> _subItems = new Dictionary<string, ClientItem>();
        private PageControlServer _srv = null;
        private DynamicServer _dyn = null;
        private object locker = new object();
    }
}
