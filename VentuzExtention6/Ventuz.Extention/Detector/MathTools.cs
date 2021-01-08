using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    public class MathTools
    {
        public static bool equal_deviation(double val1, double val2, double deviation)
        {
            return (val1 >= val2 - deviation) && (val1 <= val2 + deviation);
        }

        /// <summary>
        /// 将 p3 带入 p1, p2 组成的直线方程 Ax + By + C = 0 的结果
        /// </summary>
        public static double side_of_pt3(LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            return (p2.y - p1.y) * p3.x + (p1.x - p2.x) * p3.y + p2.x * p1.y - p2.y * p1.x;
        }

        /// <summary>
        /// 两点的距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double distance(LogicTouchPos p1, LogicTouchPos p2)
        {
            return Math.Sqrt(p1.square_dis_to(p2));
        }

        /// <summary>
        /// 把一个点沿着 p1->p2 直线移动 right 的距离, 然后再向上移动 up 的距离
        /// 返回这个点移动后的坐标
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="right"></param>
        /// <param name="up"></param>
        /// <param name="dis12"></param>
        /// <returns></returns>
        public static LogicTouchPos rightup(LogicTouchPos p1, LogicTouchPos p2, double right, double up, double dis12 = - 1)
        {
            if(dis12 < 0)
            {
                dis12 = distance(p1, p2);
            }

            // 移动到 L12 的垂点坐标
            double xc = right * (p2.x - p1.x) / dis12 + p1.x;
            double yc = right * (p2.y - p1.y) / dis12 + p1.y;

            // 从垂点往上移动
            double cx = up / dis12 * (p2.y - p1.y) + xc;
            double cy = up / dis12 * (p1.x - p2.x) + yc;

            return new LogicTouchPos(cx, cy);
        }
    }
}
