using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.Input;

namespace Ventuz.Extention.Marker
{
    public class TouchEventGroup
    {
        public double happentime = 0;
        public List<EventAtom> events = new List<EventAtom>();
    }
}
