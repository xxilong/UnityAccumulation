using System;
using System.Collections.Generic;
using Ventuz.Kernel.Input;
using Ventuz.Extention.UI;
using ShareLib.Log;
using ShareLib.Conf;

namespace Ventuz.Extention.Marker
{
    public class EventFilter
    {
        public static EventFilter Instance = new EventFilter();
        
        public EventFilter()
        {
            AddWatcher(TouchToFile.toFile);
            AddWatcher(TouchStatus.touchs);

            Logger.Info($"TouchPosRevise.Enable: {enableModifyCoord}");
        }
                
        public void FilterEventAtom(double time, List<EventAtom> events)
        {
            // 快捷键消息处理
            foreach (EventAtom ev in events)
            {
                if (ev.DeviceType == DeviceTypes.RawKeyboard && ev.ID == "KeyUp")
                {
                    var kev = (EventAtomInt)ev;
                    UIExtention.Instance.OnHotKey(kev.Value);
                }
            }

            // 导入播放的事件
            TouchPlayer.player.AddPlayEvents(time, events);

            // 回调事件处理器
            foreach(var w in watchs)
            {
                w.FrameBegin(time, events);
            }

            foreach(EventAtom ev in events)
            {
                if(ev.DeviceType == DeviceTypes.MultiTouch)
                {
                    EventAtomTouch touchEvent = (EventAtomTouch)ev;

                    if (touchEvent.TouchType == Touch.TouchType.Cursor)
                    {                      
                        foreach(var w in watchs)
                        {
                            w.FrameTouchEvent(touchEvent);
                        }
                    }
                }
            }

            foreach(var w in watchs)
            {
                w.FrameEnd(time, events);
            }

            if (enableModifyCoord)
            {
                foreach (EventAtom ev in events)
                {
                    if (ev.DeviceType == DeviceTypes.MultiTouch)
                    {
                        EventAtomTouch touchEvent = (EventAtomTouch)ev;

                        if (touchEvent.TouchType == Touch.TouchType.Cursor)
                        {
                            Logger.Debug(ev.ToString());
                            Logger.Debug($"Touch Event x={touchEvent.X} y={touchEvent.Y}");
                            touchEvent.Y = touchEvent.Y * modifyYMultiple + modifyYOffset;
                            touchEvent.X = touchEvent.X * modifyXMultiple + modifyXOffset;
                            Logger.Debug($"Modifyed Touch Event x={touchEvent.X} y={touchEvent.Y}");
                        }
                    }
                }
            }

            // 打印最终发送给 Ventuz 的消息
            //foreach (EventAtom ev in events)
            //{
            //    if (ev.DeviceType == DeviceTypes.MultiTouch)
            //    {
            //        Console.WriteLine(ev.ToString());
            //    }
            //}
        }
        public void AddWatcher(TouchWatcher watch)
        {
            watchs.Add(watch);
        }
        public void DelWatcher(TouchWatcher watch)
        {
            watchs.Remove(watch);
        }

        private List<TouchWatcher> watchs = new List<TouchWatcher>();
        private bool enableModifyCoord = ModConfig.getconf<bool>("TouchPosRevise", "enable", false);
        private float modifyXOffset = ModConfig.getconf<float>("TouchPosRevise", "offsetx", 0.0f);
        private float modifyXMultiple = ModConfig.getconf<float>("TouchPosRevise", "mulx", 1.0f);
        private float modifyYOffset = ModConfig.getconf<float>("TouchPosRevise", "offsety", 0.0f);
        private float modifyYMultiple = ModConfig.getconf<float>("TouchPosRevise", "muly", 1.0f);
    }
}
