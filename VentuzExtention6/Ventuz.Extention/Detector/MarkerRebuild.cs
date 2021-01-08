using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    /// <summary>
    /// 丢失了一个 Marker 点后, 尝试通过现有的两个点, 计算出第三个点的位置.
    /// </summary>
    public class MarkerRebuild
    {
        public void RebuildMarkerPoint(MarkerItem marker, LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            if(!p1.isvalid())
            {
                p1 = RebuildP1(marker, p2, p3);
            }
            else if(!p2.isvalid())
            {
                p2 = RebuildP2(marker, p1, p3);
            }
            else if(!p3.isvalid())
            {
                p3 = RebuildP3(marker, p1, p2);
            }

            markerGeometry.LocMarkerBy3Pts(marker, p1, p2, p3);
        }

        private LogicTouchPos RebuildP1(MarkerItem marker, LogicTouchPos p2, LogicTouchPos p3)
        {
            var pb = marker.config.pb;
            return MathTools.rightup(p2, p3, pb.p1_left_23, pb.p1_top_23, marker.config.actdis23);
        }

        private LogicTouchPos RebuildP2(MarkerItem marker, LogicTouchPos p1, LogicTouchPos p3)
        {
            var pb = marker.config.pb;
            return MathTools.rightup(p3, p1, pb.p2_left_31, pb.p2_top_31, marker.config.actdis13);
        }

        private LogicTouchPos RebuildP3(MarkerItem marker, LogicTouchPos p1, LogicTouchPos p2)
        {
            var pb = marker.config.pb;
            return MathTools.rightup(p1, p2, pb.p3_left_12, pb.p3_top_12, marker.config.actdis12);
        }

        private MarkerGeometry markerGeometry = new MarkerGeometry();
    }
}
