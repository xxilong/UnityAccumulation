using System;
using System.Collections.Generic;
using Ventuz.Kernel.Input;

namespace Ventuz.Extention.Marker
{
    public class EventOperator
    {
        public EventOperator(List<EventAtom> events)
        {
            this.events = events;
        }

        public bool HaveAdd(string id)
        {
            foreach(var ev in events)
            {
                if (ev.DeviceType != DeviceTypes.MultiTouch)
                    continue;

                EventAtomTouch touchEvent = (EventAtomTouch)ev;
                if (touchEvent.TouchType != Touch.TouchType.Cursor)
                    continue;

                if (touchEvent.TouchID != id)
                    continue;

                if (touchEvent.ID == "TouchAdd")
                {
                    return true;
                }
            }

            return false;
        }

        public bool HaveMove(string id)
        {
            foreach (var ev in events)
            {
                if (ev.DeviceType != DeviceTypes.MultiTouch)
                    continue;

                EventAtomTouch touchEvent = (EventAtomTouch)ev;
                if (touchEvent.TouchType != Touch.TouchType.Cursor)
                    continue;

                if (touchEvent.TouchID != id)
                    continue;

                if (touchEvent.ID == "TouchMove")
                {
                    return true;
                }
            }

            return false;
        }

        public bool HaveDel(string id)
        {
            foreach (var ev in events)
            {
                if (ev.DeviceType != DeviceTypes.MultiTouch)
                    continue;

                EventAtomTouch touchEvent = (EventAtomTouch)ev;
                if (touchEvent.TouchType != Touch.TouchType.Cursor)
                    continue;

                if (touchEvent.TouchID != id)
                    continue;

                if (touchEvent.ID == "TouchRemove")
                {
                    return true;
                }
            }

            return false;
        }

        public void fireAdd(TouchInfo touch)
        {
            var ev = new EventAtomTouch(DeviceTypes.MultiTouch, 
                touch.deviceID, "TouchAdd", touch.groupID, Touch.TouchType.Cursor, touch.id, 0, 0, 0, 1, 0);
            events.Insert(0, ev);
        }

        public void fireMove(TouchInfo touch)
        {
            RawTouchPos pos = touch.ToRawPos();

            var ev = new EventAtomTouch(DeviceTypes.MultiTouch,
                touch.deviceID, "TouchMove", touch.groupID, Touch.TouchType.Cursor, touch.id, (float)pos.rx, (float)pos.ry, 0, 1, 0);
            events.Add(ev);
        }

        public void fireAddMove(TouchInfo touch)
        {
            RawTouchPos pos = touch.ToRawPos();

            var ev1 = new EventAtomTouch(DeviceTypes.MultiTouch,
                touch.deviceID, "TouchAdd", touch.groupID, Touch.TouchType.Cursor, touch.id, 0, 0, 0, 1, 0);
            var ev2 = new EventAtomTouch(DeviceTypes.MultiTouch,
                touch.deviceID, "TouchMove", touch.groupID, Touch.TouchType.Cursor, touch.id, (float)pos.rx, (float)pos.ry, 0, 1, 0);

            events.Add(ev1);
            events.Add(ev2);
        }

        public void fireDel(TouchInfo touch)
        {
            var ev = new EventAtomTouch(DeviceTypes.MultiTouch,
                touch.deviceID, "TouchRemove", touch.groupID, Touch.TouchType.Cursor, touch.id, 0, 0, 0, 1, 0);

            events.Add(ev);
        }

        public void delTouchs(TouchInfo touch)
        {
            int nCount = events.Count;

            for(int i = nCount - 1; i >= 0; --i)
            {
                var ev = events[i];
                if (ev.DeviceType != DeviceTypes.MultiTouch)
                    continue;

                EventAtomTouch touchEvent = (EventAtomTouch)ev;
                if (touchEvent.TouchType != Touch.TouchType.Cursor)
                    continue;

                if (touchEvent.ID == "TouchRemove")
                    continue;

                if (touchEvent.TouchID == touch.id)
                {
                    events.RemoveAt(i);
                }
            }
        }

        private List<EventAtom> events;
    }
}
