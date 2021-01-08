using ShareLib.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Conf
{
    public class GlobalConfWatcher
    {
        public GlobalConfWatcher(string domain, string key, Action<string> notify)
        {
            _domain = domain;
            _key = key;
            _notify = notify;

            _value = GlobalConf.getconf(_domain, _key);

            GlobalConf.OnConfFileChange += OnConfChangeNotify;
        }

        public void Close()
        {
            GlobalConf.OnConfFileChange -= OnConfChangeNotify;
        }

        public string Value { get => _value; }
        public int AsInt
        {
            get
            { 
                if(int.TryParse(_value, out int val))
                {
                    return val;
                }
                return 0;
            }
        }

        public double AsFloat
        {
            get
            {
                if(double.TryParse(_value, out double val))
                {
                    return val;
                }

                return 0.0;
            }
        }

        public bool AsBool
        {
            get => _value == "true" || _value == "True" || AsInt != 0;
        }

        private void OnConfChangeNotify(object sender, int way)
        {
            string value = GlobalConf.getconf(_domain, _key);

            if (value == _value)
            {
                return;
            }

            _value = value;
            if (_notify == null)
            {
                return;
            }

            _notify(_value);
        }
               
        private string _domain;
        private string _key;
        private string _value;
        private Action<string> _notify;
    }
}
