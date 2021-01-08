using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Vals
{
    public class GlobalFloat
    {
        public float GetVal(string name)
        {
            if(_values.ContainsKey(name))
            {
                return _values[name];
            }

            return 0f;
        }

        public void SetVal(string name, float val)
        {
            _values[name] = val;
        }

        private Dictionary<string, float> _values = new Dictionary<string, float>();
    }
}
