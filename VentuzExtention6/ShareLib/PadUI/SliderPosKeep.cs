using System;
using System.Collections.Generic;
using System.Text;
using ShareLib.Ayz;

namespace ShareLib.PadUI
{
    public class SliderPosKeep : IUIStatus
    {
        public SliderPosKeep(string[] cmdseq, string id)
        {
            _cmdseq = cmdseq;
            _ctrlid = id;
        }

        public void OnRecvCommand(CmdLine cmd, Action<string> sender)
        {
            if(cmd.cmd != _cmdseq[0])
            {
                return;
            }

            int argIndex = 0;
            for(; argIndex < _cmdseq.Length - 1; ++argIndex)
            {
                if(!cmd.getarg<string>(argIndex, out string item))
                {
                    return;
                }

                if(item != _cmdseq[argIndex + 1])
                {
                    return;
                }
            }

            cmd.getarg<string>(argIndex, out string valitem);
            float.TryParse(valitem, out _value);
        }

        public void UpdateStatus(Action<string> sender)
        {
            sender($"+percent {_ctrlid} {_value}");
        }

        private string[] _cmdseq;
        private string _ctrlid;
        private float _value = 0.0f;
    }
}
