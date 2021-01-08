using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    /// <summary>
    /// Marker 的方向定义为 中心点->点1 的方向
    /// 实际使用是可在这个方向上定义一个偏移,
    /// 比如每次都让 Marker 放进来时的方向为 0 度方向
    /// </summary>
    /// 
    [Serializable]
    public class MarkerConfig
    {
        public string name;         // Marker 的名字
        public double centerx;      // 中心点在线段 1->2 上离点 1 的距离, 线段 12 长度的百分比
        public double centery;      // 中心点离直线 1-2 的距离
        public double dis12;        // 点 1 到 点 2 之间的距离
        public double dis23;        // 点 2 到 点 3 之间的距离
        public double dis13;        // 点 1 到 点 3 之间的距离
        public double erdis12;      // 误差范围
        public double erdis23;
        public double erdis13;
        public double radius;       // Marker 的半径大小, 在此之类的触摸点被删除
        public bool rmtouchs;       // 删除原始触摸事件
        public bool direction;      // 方向, true 为顺时针, false 为逆时针
        [NonSerialized]
        public PointBuilds pbuilder = null;

        public MarkerConfig()
        {
            name = "Marker1";

            centerx = 0;
            centery = 0;

            dis12 = 0;
            dis23 = 0;
            dis13 = 0;

            erdis12 = 0;
            erdis23 = 0;
            erdis13 = 0;

            radius = 0;

            direction = true;
            rmtouchs = false;
        }

        public void Reset()
        {
            centerx = 0;
            centery = 0;

            dis12 = 0;
            dis23 = 0;
            dis13 = 0;

            erdis12 = 0;
            erdis23 = 0;
            erdis13 = 0;

            radius = 0;

            direction = true;
            rmtouchs = false;
        }

        public void SetByPts(LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            dis12 = p1.square_dis_to(p2);
            erdis12 = 0;
            dis13 = p1.square_dis_to(p3);
            erdis13 = 0;
            dis23 = p2.square_dis_to(p3);
            erdis23 = 0;

            radius = actdis12 * 1.5;
            direction = MathTools.side_of_pt3(p1, p2, p3) > 0;
            pbuilder = null;
        }

        public PointBuilds pb
        {
            get
            {
                if(pbuilder == null)
                {
                    pbuilder = PointBuilds.MakeFromDistance(dis12, dis23, dis13, direction);
                }

                return pbuilder;
            }
        }

        public double actdis12
        {
            get
            {
                return Math.Sqrt(dis12);
            }
        }

        public double actdis23
        {
            get
            {
                return Math.Sqrt(dis23);
            }
        }

        public double actdis13
        {
            get
            {
                return Math.Sqrt(dis13);
            }
        }

        public int cxRate
        {
            get
            {
                return (int)(centerx / actdis12 * 100.0 + 200.0); ;
            }
            set
            {
                centerx = (value - 200) / 100.0 * actdis12;
            }
        }

        public int cyRate
        {
            get
            {
                return (int)(centery / actdis12 * 100.0 + 200.0); ;
            }

            set
            {
                centery = (value - 200) / 100.0 * actdis12;
            }
        }

        public int radiusRate
        {
            get
            {
                if(actdis12 == 0)
                {
                    return 0;
                }

                return (int)(radius / actdis12 * 100.0);
            }

            set
            {
                radius = (value) / 100.0 * actdis12;
            }
        }

        public double Radius
        {
            get
            {
                if(radius <= 0)
                {
                    return actdis12 * 1.5;
                }

                return radius;
            }
        }

        public bool valid()
        {
            if (dis12 <= 0 || dis23 <= 0 || dis13 <= 0)
                return false;

            return true;
        }

        public int valid(out string msg)    // 0 无效  -1 警告  1 有效
        {
            if(dis12 <= 0)
            {
                msg = "距离 Pt1-Pt2 未定义";
                return 0;
            }

            if(dis23 <= 0)
            {
                msg = "距离 Pt2-Pt3 未定义";
                return 0;
            }

            if (dis13 <= 0)
            {
                msg = "距离 Pt1-Pt3 未定义";
                return 0;
            }

            if(dis12 - erdis12 <= dis23 + erdis23)
            {
                msg = "距离 12 和 距离 23 之间的误差过大, 可能造成混淆";
                return -1;
            }

            if(dis23 - erdis23 <= dis13 + erdis13)
            {
                msg = "距离 23 和 距离 13 之间的误差过大, 可能造成混淆";
                return -1;
            }

            msg = "";
            return 1;
        }
    }
}
