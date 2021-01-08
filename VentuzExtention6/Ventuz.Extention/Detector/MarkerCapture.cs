using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    /// <summary>
    /// 当 Marker 的触摸点离开后, 尝试从新的触摸点中找到属于此 Marker 的触摸点
    /// </summary>
    public class MarkerCapture
    {
        public bool TryCaptureLostPoint(MarkerItem marker, 
            Dictionary<string, TouchMarkerInfo> markerTouchs,
            LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            if(p1.isvalid())
            {
                return TryCaptureLostPointP1Ready(marker, markerTouchs, p1, p2, p3);
            }

            foreach(var item in markerTouchs)
            {
                TouchMarkerInfo p = item.Value;
                if(p.ismarker)
                {
                    continue;
                }

                p.ismarker = true;
                if(!TryCaptureLostPointP1Ready(marker, markerTouchs, p, p2, p3))
                {
                    p.ismarker = false;
                }
                else
                {
                    return true;
                }
            }

            //Console.WriteLine("Point Losted: {0}, {1}, {2}", p1.isvalid(), p2.isvalid(), p3.isvalid());
            return false;
        }

        private bool TryCaptureLostPointP1Ready(MarkerItem marker,
            Dictionary<string, TouchMarkerInfo> markerTouchs,
            LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            if(p2.isvalid())
            {
                return TryCaptureLostPointP12Ready(marker, markerTouchs, p1, p2, p3);
            }

            foreach (var item in markerTouchs)
            {
                TouchMarkerInfo p = item.Value;
                if (p.ismarker)
                {
                    continue;
                }

                p.ismarker = true;
                if (!TryCaptureLostPointP1Ready(marker, markerTouchs, p1, p, p3))
                {
                    p.ismarker = false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryCaptureLostPointP12Ready(MarkerItem marker,
            Dictionary<string, TouchMarkerInfo> markerTouchs,
            LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            if(p3.isvalid())
            {
                if(markerChecker.IsMatch(marker.config, p1, p2, p3))
                {
                    markerGeometry.LocMarkerBy3Pts(marker, p1, p2, p3);
                    return true;
                }

                return false;
            }

            foreach (var item in markerTouchs)
            {
                TouchMarkerInfo p = item.Value;
                if (p.ismarker)
                {
                    continue;
                }

                p.ismarker = true;
                if (!markerChecker.IsMatch(marker.config, p1, p2, p))
                {
                    p.ismarker = false;
                }
                else
                {
                    markerGeometry.LocMarkerBy3Pts(marker, p1, p2, p);
                    return true;
                }
            }

            return false;
        }
        
        private MarkerGeometry markerGeometry = new MarkerGeometry();
        private MarkerChecker markerChecker = new MarkerChecker();
    }
}
