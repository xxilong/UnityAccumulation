using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.Input;

namespace Ventuz.Extention.Marker
{
    public interface TouchWatcher
    {
        void FrameBegin(double timeCode, List<EventAtom> events);

        void FrameEnd(double timeCode, List<EventAtom> events);

        void FrameTouchEvent(EventAtomTouch touchEvent);
    }

    public interface TouchRecorder : TouchWatcher
    {
        void Start();
        void Stop();
    }
}
