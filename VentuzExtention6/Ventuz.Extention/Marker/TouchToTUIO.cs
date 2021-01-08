using System.Collections.Generic;
using TUIO;
using Ventuz.Kernel.Input;

namespace Ventuz.Extention.Marker
{
    class TouchToTUIO : TouchRecorder
    {
        public TouchToTUIO()
        {

        }

        public void FrameBegin(double timeCode, List<EventAtom> events)
        {
            srv.initFrame(TuioTime.SessionTime);
        }

        public void FrameEnd(double timeCode, List<EventAtom> events)
        {
            // commit 
            srv.stopUntouchedMovingCursors();
            srv.removeUntouchedStoppedCursors();
            srv.commitFrame();
        }

        public void FrameTouchEvent(EventAtomTouch touchEvent)
        {
            if(touchEvent.TouchType != Touch.TouchType.Cursor)
            {
                return;
            }

            if(touchEvent.ID == "TouchAdd")
            {
                curs[touchEvent.TouchID] = srv.addTuioCursor(touchEvent.X, touchEvent.Y);
            }
            else if(touchEvent.ID == "TouchMove")
            {
                srv.updateTuioCursor(curs[touchEvent.TouchID], touchEvent.X, touchEvent.Y);
            }
            else if(touchEvent.ID == "TouchRemove")
            {
                srv.removeTuioCursor(curs[touchEvent.TouchID]);
            }
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        private TuioServer srv = new TuioServer("127.0.0.1", 3332);
        private Dictionary<string, TuioCursor> curs = new Dictionary<string, TuioCursor>();
    }
}
