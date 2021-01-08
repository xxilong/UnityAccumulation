using System;
using System.Diagnostics;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    public class MarkerGeometry
    {
        public void LocMarkerBy3Pts(MarkerItem marker, LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            MarkerConfig config = marker.config;
            Debug.Assert(config.actdis12 > 0);

            marker.SetLocPoint(p1, p2, p3);

            LogicTouchPos center = MathTools.rightup(p1, p2, config.centerx, config.centery, config.actdis12);
            marker.SetPosition(center.x, center.y);

            LogicTouchPos v = p1 - center;
            double angle = Math.Atan2(v.y, v.x);
            angle = angle * 180 / Math.PI;
            marker.SetAngle(angle);
        }

        public void LocMarkerBy1Pts(MarkerItem marker, LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            LogicTouchPos offset = null;

            if(p1.isvalid())
            {
                offset = p1 - marker.p1pos;
            }

            if(p2.isvalid())
            {
                offset = p2 - marker.p2pos;
            }

            if(p3.isvalid())
            {
                offset = p3 - marker.p3pos;
            }

            marker.MoveOff(offset);
        }
    }
}
