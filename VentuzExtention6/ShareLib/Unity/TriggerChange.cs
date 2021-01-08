using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLib.Unity
{
    public class TriggerChange
    {
        public TriggerChange(Action on, Action off, bool init = false)
        {
            triggerOn = on;
            triggerOff = off;
            curStatus = init;
        }

        public void SetStatus(bool ison)
        {
            if(ison == curStatus)
            {
                return;
            }

            curStatus = ison;
            if(curStatus)
            {
                triggerOn();
            }
            else
            {
                triggerOff();
            }
        }

        private Action triggerOn;
        private Action triggerOff;
        private bool curStatus = false;
    }
}
