using System;
using System.Collections.Generic;
using Ventuz.Kernel.Input;
using Ventuz.Extention.Detector;

namespace Ventuz.Extention.Marker
{
    public class TouchStatus : TouchWatcher
    {
        public static TouchStatus touchs = new TouchStatus();

        // 事件过滤接口
        public void FrameBegin(double time, List<EventAtom> events)
        {
            frameTouchEventCount = 0;
        }
        public void FrameEnd(double time, List<EventAtom> events)
        {
            if (frameTouchEventCount == 0 && !MarkerConfigManager.instance.IsModifyed())
            {
                return;
            }
            
            InitMarkerTouchInfo();
            CheckCurrentMarkers();
            CheckNewMarkers();
            FireMarkerEvent(events);
            FilterTouchEvents(events);
        }
        public void FrameTouchEvent(EventAtomTouch ev)
        {
            if(ev.ID == "TouchAdd")
            {
                curTouchs[ev.TouchID] = new TouchInfo(ev);
            }
            else if(ev.ID == "TouchMove")
            {
                curTouchs[ev.TouchID] = new TouchInfo(ev);
            }
            else if(ev.ID == "TouchRemove")
            {
                curTouchs.Remove(ev.TouchID);
            }

            ++frameTouchEventCount;
        }

        // Marker 检测接口, 获取最近的 3 个触摸点的距离
        public struct DisWithPt : IComparable<DisWithPt>
        {
            public double dis;
            public LogicTouchPos pt1, pt2;

            public int CompareTo(DisWithPt other)
            {
                return dis.CompareTo(other.dis);
            }
        }

        public DisWithPt[] GetNearest3PtDistance()
        {
            if (curTouchs.Count < 3)
            {
                return new DisWithPt[0];
            }
            
            List<string> keys = new List<string>();
            keys.AddRange(curTouchs.Keys);

            int Size = curTouchs.Count;
            List<DisWithPt> dis = new List<DisWithPt>();
            Console.WriteLine("检测Marker: 当前的触摸点 {0} 个", Size);

            for (int i = 0; i < Size; ++i)
            {
                for(int j = i + 1; j < Size; ++j)
                {
                    var pos1 = curTouchs[keys[i]];
                    var pos2 = curTouchs[keys[j]];

                    DisWithPt obj = new DisWithPt();
                    obj.dis = pos1.square_dis_to(pos2);
                    obj.pt1 = pos1;
                    obj.pt2 = pos2;

                    dis.Add(obj);
                }
            }

            dis.Sort();
            return new DisWithPt[] { dis[0], dis[1], dis[2] };
        }

        //检测 Marker 
        private void InitMarkerTouchInfo()
        {
            markerTouchs.Clear();

            foreach (var item in curTouchs)
            {
                markerTouchs[item.Key] = new TouchMarkerInfo(item.Key, item.Value);
            }
        }
        private void CheckCurrentMarkers()
        {
            foreach(var marker in curMarkers)
            {
                CheckMarkerStatus(marker.Value);
            }
        }

        private void CheckMarkerStatus(MarkerItem marker)
        {
            string pt1 = marker.pt1;
            string pt2 = marker.pt2;
            string pt3 = marker.pt3;

            LogicTouchPos p1 = new LogicTouchPos();
            LogicTouchPos p2 = new LogicTouchPos();
            LogicTouchPos p3 = new LogicTouchPos();

            int alivePointCount = 0;

            if(markerTouchs.ContainsKey(pt1))
            {
                var touch = markerTouchs[pt1];
                touch.ismarker = true;
                p1 = touch.clone();
                ++alivePointCount;
            }

            if(markerTouchs.ContainsKey(pt2))
            {
                var touch = markerTouchs[pt2];
                touch.ismarker = true;
                p2 = touch.clone();
                ++alivePointCount;
            }

            if (markerTouchs.ContainsKey(pt3))
            {
                var touch = markerTouchs[pt3];
                touch.ismarker = true;
                p3 = touch.clone();
                ++alivePointCount;
            }         

            if(alivePointCount == 3)
            {
                markerGeometry.LocMarkerBy3Pts(marker, p1, p2, p3);
                return;
            }

            if (alivePointCount == 0)
            {
                marker.Remove();
                return;
            }

            // 捕捉到了新的点
            if(markerCapture.TryCaptureLostPoint(marker, markerTouchs, p1, p2, p3))
            {
                return;
            }

            if(alivePointCount == 1)
            {
                markerGeometry.LocMarkerBy1Pts(marker, p1, p2, p3);
            }
            else
            {
                markerRebuilder.RebuildMarkerPoint(marker, p1, p2, p3);
            }
        }
        private void CheckNewMarkers()
        {
            double maxdis = MarkerConfigManager.instance.GetMaxDistance();
            if(maxdis <= 0)
            {
                return;
            }

            List <TouchMarkerInfo> touchlist = new List<TouchMarkerInfo>();
            foreach(var item in markerTouchs)
            {
                if(item.Value.ismarker)
                {
                    continue;
                }

                touchlist.Add(item.Value);
            }
            
            if(touchlist.Count < 3)
            {
                return;
            }

            int ptCount = touchlist.Count;

            Dictionary<string, double> disbuff = new Dictionary<string, double>();
            // 计算所有点两两之间的距离
            for (int i = 0; i < ptCount; ++i)
            {
                for(int j = i + 1; j < ptCount; ++j)
                {
                    var pt1 = touchlist[i];
                    var pt2 = touchlist[j];

                    double dis = pt1.square_dis_to(pt2);
                    if(dis <= maxdis)
                    {
                        pt1.usefull = true;
                        pt2.usefull = true;

                        disbuff[pt1.id + "-" + pt2.id] = dis;
                        disbuff[pt2.id + "-" + pt1.id] = dis;
                    }                  
                }
            }

            for(int i = ptCount - 1; i >= 0; --i)
            {
                if(!touchlist[i].usefull)
                {
                    touchlist.Remove(touchlist[i]);
                }
            }
            
            if(touchlist.Count < 3)
            {
                return;
            }
            
            Console.WriteLine("过滤出 {0} 个点, {1} 个距离值", touchlist.Count, disbuff.Count);
            var allCfg = MarkerConfigManager.instance.GetValidConfigs();
            foreach(MarkerConfig cfg in allCfg)
            {
                foreach(var pt1 in touchlist)
                {
                    if (pt1.ismarker)
                        continue;

                    foreach (var pt2 in touchlist)
                    {
                        if (pt1 == pt2)
                            continue;

                        if (pt2.ismarker)
                            continue;

                        string diskey = pt1.id + "-" + pt2.id;
                        if (!disbuff.ContainsKey(diskey))
                            continue;

                        double dis12 = disbuff[pt1.id + "-" + pt2.id];
                        if (dis12 < cfg.dis12 - cfg.erdis12 || dis12 > cfg.dis12 + cfg.erdis12)
                            continue;                                               

                        foreach (var pt3 in touchlist)
                        {
                            if (pt3.ismarker)
                                continue;

                            if (pt3 == pt1)
                                continue;

                            if (pt3 == pt2)
                                continue;

                            string dis13key = pt1.id + "-" + pt3.id;
                            string dis23key = pt2.id + "-" + pt3.id;

                            if (!disbuff.ContainsKey(dis13key))
                                continue;

                            if (!disbuff.ContainsKey(dis23key))
                                continue;

                            double dis13 = disbuff[dis13key];
                            double dis23 = disbuff[dis23key];

                            if (dis13 < cfg.dis13 - cfg.erdis13 || dis13 > cfg.dis13 + cfg.erdis13)
                                continue;

                            if (dis23 < cfg.dis23 - cfg.erdis23 || dis23 > cfg.dis23 + cfg.erdis23)
                                continue;

                            if (dis13 > dis23)
                                continue;

                            if (dis23 > dis12)
                                continue;

                            if ((MathTools.side_of_pt3(pt1, pt2, pt3) > 0) != cfg.direction)
                                continue;

                            pt1.ismarker = true;
                            pt2.ismarker = true;
                            pt3.ismarker = true;

                            string name = UniqueMarkerName(cfg.name);
                            Console.WriteLine("检测到 Marker: {0} as {1}", cfg.name, name);
                            var marker = new MarkerItem(cfg, pt1.id, pt2.id, pt3.id);
                            markerGeometry.LocMarkerBy3Pts(marker,pt1, pt2, pt3);
                            curMarkers[name] = marker;
                        }
                    }
                }
            }
        }

        private string UniqueMarkerName(string cfgname)
        {
            if (!curMarkers.ContainsKey(cfgname))
                return cfgname;

            int index = 1;
            while(true)
            {
                string name = cfgname + index.ToString();
                if (!curMarkers.ContainsKey(name))
                    return name;
            }
        }

        private void FireMarkerEvent(List<EventAtom> events)
        {
            List<string> toRemove = new List<string>();
            foreach(var marker in curMarkers)
            {
                var evs = marker.Value.GetEvent(marker.Key);
                events.AddRange(evs);

                //foreach(var ev in evs)
                //{
                //    Console.WriteLine("发送 Marker 事件: {0}", ev.ToString());
                //}
    
                if (marker.Value.remove)
                {
                    toRemove.Add(marker.Key);
                }
            }

            foreach(var k in toRemove)
            {
                curMarkers.Remove(k);
            }
        }

        private void FilterTouchEvents(List<EventAtom> events)
        {
            EventOperator evop = new EventOperator(events);

            foreach(var item in curTouchs)
            {
                var touch = item.Value;
                bool inAnyMarker = false;

                foreach(var marker in curMarkers)
                {
                    if(marker.Value.IsAround(touch))
                    {
                        if(marker.Value.config.rmtouchs)
                        {
                            HandleHideTouch(touch, evop);
                        }
                        else
                        {
                            HandleShowTouch(touch, evop);
                        }

                        inAnyMarker = true;
                        break;
                    }
                }

                if(!inAnyMarker)
                {
                    HandleShowTouch(touch, evop);
                }
            }

            foreach(var ev in events)
            {
                if (ev.DeviceType != DeviceTypes.MultiTouch)
                    continue;
                
                EventAtomTouch touchEvent = (EventAtomTouch)ev;
                if (touchEvent.TouchType != Touch.TouchType.Cursor)
                    continue;

                if (touchEvent.ID == "TouchAdd" || touchEvent.ID == "TouchMove")
                {
                    lastTouchs.AddTouch(touchEvent.TouchID);
                }
                else if (touchEvent.ID == "TouchRemove")
                {
                    lastTouchs.DelTouch(touchEvent.TouchID);
                }
            }
        }

        private void HandleShowTouch(TouchInfo touch, EventOperator events)
        {
            if (lastTouchs.HasTouch(touch.id))
                return;

            if (events.HaveAdd(touch.id))
                return;

            if(events.HaveMove(touch.id))
            {
                events.fireAdd(touch);
            }
            else
            {
                events.fireAddMove(touch);
            }
        }

        private void HandleHideTouch(TouchInfo touch, EventOperator events)
        {
            if(!lastTouchs.HasTouch(touch.id))
            {
                events.delTouchs(touch);
                return;
            }

            if(!events.HaveDel(touch.id))
            {
                events.delTouchs(touch);
                events.fireDel(touch);
            }
        }

        private Dictionary<string, TouchInfo> curTouchs = new Dictionary<string, TouchInfo>();
        private CurrentTouchID lastTouchs = new CurrentTouchID();
        private Dictionary<string, TouchMarkerInfo> markerTouchs = new Dictionary<string, TouchMarkerInfo>();
        private Dictionary<string, MarkerItem> curMarkers = new Dictionary<string, MarkerItem>();

        private int frameTouchEventCount = 0;

        private MarkerGeometry markerGeometry = new MarkerGeometry();
        private MarkerCapture markerCapture = new MarkerCapture();
        private MarkerRebuild markerRebuilder = new MarkerRebuild();
    }
}
