using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Vals
{
    public class GlobalEvent
    {
        private class GlobalEventItem
        {
            public event EventHandler<string> EventItem;
            public void fireEvent(string args)
            {
                EventItem?.Invoke(this, args);
            }
        }

        public void FireEvent(string name, string args)
        {
            if(name == null)
            {
                return;
            }

            if(!_values.ContainsKey(name))
            {
                return;
            }

            _values[name].fireEvent(args);
        }

        public EventHandler<string> AddListener(string name, Action<string> act)
        {
            if(name == null)
            {
                return null;
            }

            if(act == null)
            {
                return null;
            }

            if(!_values.ContainsKey(name))
            {
                _values[name] = new GlobalEventItem();
            }

            EventHandler<string> hander = (object sender, string args) => act(args);
            _values[name].EventItem += hander;
            return hander;
        }

        public void RemoveListener(string name, EventHandler<string> hander)
        {
            if(name == null)
            {
                return;
            }

            if(hander == null)
            {
                return;
            }

            if(!_values.ContainsKey(name))
            {
                return;
            }

            _values[name].EventItem -= hander;           
        }

        private Dictionary<string, GlobalEventItem> _values = new Dictionary<string, GlobalEventItem>();
    }
}
