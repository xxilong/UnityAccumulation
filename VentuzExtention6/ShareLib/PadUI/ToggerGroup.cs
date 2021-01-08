using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Ayz;

namespace ShareLib.PadUI
{
    public class ToggerGroup : ToggerCheck
    {
        public ToggerGroup(string header, string item, ToggerCheck[] childs)
            : base(header, item)
        {
            _onchanged = OnChanged;
            _childs = childs;
        }

        public void OnChanged(bool active, Action<string> sender)
        {
            foreach(var c in _childs)
            {
                c.SetCheck(active, sender);
            }
        }

        public override void OnRecvCommand(CmdLine cmd, Action<string> sender)
        {
            base.OnRecvCommand(cmd, sender);
            foreach (var c in _childs)
            {
                c.OnRecvCommand(cmd, sender);
            }
        }

        public override void UpdateStatus(Action<string> sender)
        {
            base.UpdateStatus(sender);
            foreach (var c in _childs)
            {
                c.UpdateStatus(sender);
            }
        }

        private ToggerCheck[] _childs;
    }
}
