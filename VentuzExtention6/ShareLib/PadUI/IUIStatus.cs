using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Ayz;

namespace ShareLib.PadUI
{
    public interface IUIStatus
    {
        void UpdateStatus(Action<string> sender);
        void OnRecvCommand(CmdLine cmd, Action<string> sender);
    }
}
