using System.Collections.Generic;

namespace Ventuz.Extention.Marker
{
    public class CurrentTouchID
    {
        public bool HasTouch(string touchid)
        {
            return _touchlist.ContainsKey(touchid);
        }

        public void AddTouch(string touchid)
        {
            _touchlist[touchid] = true;
        }

        public void DelTouch(string touchid)
        {
            _touchlist.Remove(touchid);
        }

        private Dictionary<string, bool> _touchlist = new Dictionary<string, bool>();
    }
}
