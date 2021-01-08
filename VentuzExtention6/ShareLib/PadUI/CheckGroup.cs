using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Ayz;

namespace ShareLib.PadUI
{
    public class CheckGroup : IUIStatus
    {
        public CheckGroup(string cmdhdr, string[] items, string[] ids = null, Action<string, Action<string>> onchanged = null)
        {
            _cmdhdrs = new string[] { cmdhdr };
            _groupitems = items;
            _ids = ids == null ? items : ids;
            _cursel = "";
            _onchanged = onchanged;
        }

        public CheckGroup(string[] hdrs, string[] items, string defsel)
        {
            _cmdhdrs = hdrs;
            _groupitems = items;
            _ids = items;
            _cursel = defsel;
        }

        public CheckGroup(string[] hdr1, string[] hdr2, string[] items, string defsel)
        {
            _cmdhdrs = hdr1;
            _cmdhdr2 = hdr2;
            _groupitems = items;
            _ids = items;
            _cursel = defsel;
        }

        public void OnRecvCommand(CmdLine cmd, Action<string> sender)
        {
            int argIndex = IsMatchHeader(cmd, _cmdhdrs);

            if(argIndex == -1 && _cmdhdr2 != null)
            {
                argIndex = IsMatchHeader(cmd, _cmdhdr2);
            }
            if(argIndex == -1)
            {
                return;
            }
            
            cmd.getarg<string>(argIndex, out string item);
            int sel = Array.IndexOf(_groupitems, item);
            if(sel < 0)
            {
                return;
            }

            string cursel = _ids[sel];
            if(cursel != _cursel)
            {
                if(_cursel == "")
                {
                    sender($"+check {cursel}");
                }
                else
                {
                    if(cursel == "")
                    {
                        sender($"+uncheck {_cursel}");
                    }
                    else
                    {
                        sender($"+swcheck {_cursel} {cursel}");
                    }
                }

                _cursel = cursel;
                _onchanged?.Invoke(_cursel, sender);
            }
        }

        private int IsMatchHeader(CmdLine cmd, string[] hdr)
        {
            int argIndex = 0;
            if (cmd.cmd != hdr[0])
            {
                return -1;
            }

            for (; argIndex < hdr.Length - 1; ++argIndex)
            {
                cmd.getarg<string>(argIndex, out string x);
                if (x != hdr[argIndex + 1])
                {
                    return -1;
                }
            }

            return argIndex;
        }

        public void UpdateStatus(Action<string> sender)
        {
            if(_cursel != "")
            {
                sender($"+check {_cursel}");
            }
        }

        private string[] _cmdhdrs;
        private string[] _cmdhdr2 = null;
        private string[] _groupitems;
        private string[] _ids;
        private string _cursel;
        protected Action<string, Action<string>> _onchanged = null;
    }
}
