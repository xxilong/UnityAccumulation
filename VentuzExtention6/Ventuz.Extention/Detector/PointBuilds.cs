using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Detector
{
    public class PointBuilds
    {
        public double p3_left_12 = 0;
        public double p3_top_12 = 0;

        public double p1_left_23 = 0;
        public double p1_top_23 = 0;

        public double p2_left_31 = 0;
        public double p2_top_31 = 0;

        static public PointBuilds MakeFromDistance(double dis12, double dis23, double dis13, bool dir)
        {
            PointBuilds pb = new PointBuilds();

            double d12 = Math.Sqrt(dis12);
            double d23 = Math.Sqrt(dis23);
            double d13 = Math.Sqrt(dis13);

            double angle1 = Math.Acos((dis12 + dis13 - dis23) / (2 * d12 * d13));
            double angle2 = Math.Acos((dis23 + dis12 - dis13) / (2 * d23 * d12));
            double angle3 = Math.PI - angle1 - angle2;

            pb.p3_left_12 = d13 * Math.Cos(angle1);
            pb.p3_top_12 = d13 * Math.Sin(angle1);

            pb.p1_left_23 = d12 * Math.Cos(angle2);
            pb.p1_top_23 = d12 * Math.Sin(angle2);

            pb.p2_left_31 = d23 * Math.Cos(angle3);
            pb.p2_top_31 = d23 * Math.Sin(angle3);

            if(!dir)
            {
                pb.p3_top_12 = -pb.p3_top_12;
                pb.p1_top_23 = -pb.p1_top_23;
                pb.p2_top_31 = -pb.p2_top_31;
            }

            return pb;
        }
    }
}
