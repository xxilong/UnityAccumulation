using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Ayz;

namespace ShareLib.PadUI
{
    public class ToggerCheck : IUIStatus
    {
        public ToggerCheck(string header, string item,
            Action<bool, Action<string>> changed = null,
            bool initstatus = false, string id = null,
            Func<bool, Action<string>, bool> prechange = null)
        {
            _cmdheader = header;
            _cmditem = item;
            _ctrlid = (id == null ? item : id);
            _active = initstatus;
            _onchanged = changed;
            _prechange = prechange;
        }

        public virtual void OnRecvCommand(CmdLine cmd, Action<string> sender)
        {
            if(cmd.cmd != _cmdheader)
            {
                return;
            }

            if(cmd.argcount < 1)
            {
                return;
            }

            cmd.getarg<string>(0, out string item);
            if(item != _cmditem)
            {
                return;
            }

            if (_prechange != null && !_prechange(!_active, sender))
            {
                return;
            }

            _active = !_active;

            if(_active)
            {
                sender($"+check {_ctrlid}");
            }
            else
            {
                sender($"+uncheck {_ctrlid}");
            }

            _onchanged?.Invoke(_active, sender);
        }

        public virtual void UpdateStatus(Action<string> sender)
        {
            if (_active)
            {
                sender($"+check {_ctrlid}");
            }
        }

        public void SetCheck(bool active)
        {
            if (active != _active)
            {
                _active = active;
            }
        }

        public void SetCheck(bool active, Action<string> sender)
        {
            if(_prechange != null && !_prechange(active, sender))
            {
                return;
            }

            if(active != _active)
            {
                _active = active;
                if (_active)
                {
                    sender($"+check {_ctrlid}");
                }
                else
                {
                    sender($"+uncheck {_ctrlid}");
                }
                _onchanged?.Invoke(_active, sender);
            }
        }

        public bool GetCheck()
        {
            return _active;
        }

        private string _cmdheader;
        private string _cmditem;
        private string _ctrlid;
        private bool _active = false;

        
        protected Action<bool, Action<string>> _onchanged = null;
        protected Func<bool, Action<string>, bool> _prechange = null;
    }
}
