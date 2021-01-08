using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Control
{
    public class SingleSel
    {
        public void SetCurSel(string btnid)
        {
            if (btnid == curSel)
            {
                return;
            }

            if (curSel != "")
            {
                ControlServer.Instance.SendCommand($"+swcheck {curSel} {btnid}");
            }
            else
            {
                ControlServer.Instance.SendCommand($"+check {btnid}");
            }

            curSel = btnid;
        }

        public string CurSel { get { return curSel; } }

        private string curSel = "";
    }
}
