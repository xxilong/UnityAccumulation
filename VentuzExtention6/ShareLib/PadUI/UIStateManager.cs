using ShareLib.Ayz;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.PadUI
{
    public class UIStateManager
    {
        public UIStateManager Reg(IUIStatus st)
        {
            _uiStatus.Add(st);
            return this;
        }

        public void OnRecvCommand(string cmdline, Action<string> sendcmd)
        {
            CmdLine cmd = new CmdLine(cmdline);
            foreach(var st in _uiStatus)
            {
                st.OnRecvCommand(cmd, sendcmd);
            }
        }

        public void UpdateStatus(Action<string> sendcmd)
        {
            foreach(var st in _uiStatus)
            {
                st.UpdateStatus(sendcmd);
            }
        }

        private List<IUIStatus> _uiStatus = new List<IUIStatus>();
    }
}
