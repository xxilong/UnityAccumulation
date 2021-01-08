using Ventuz.Extention.Marker;

namespace Ventuz.Extention.Detector
{
    public class MarkerChecker
    {
        public bool IsMatch(MarkerConfig config, LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            double dis12 = p1.square_dis_to(p2);
            double dis13 = p1.square_dis_to(p3);
            double dis23 = p2.square_dis_to(p3);

            if(!MathTools.equal_deviation(dis12, config.dis12, config.erdis12))
            {
                return false;
            }

            if(!MathTools.equal_deviation(dis13, config.dis13, config.erdis13))
            {
                return false;
            }

            if(!MathTools.equal_deviation(dis23, config.dis23, config.erdis23))
            {
                return false;
            }

            if((MathTools.side_of_pt3(p1, p2, p3) > 0) != config.direction)
            {
                return false;
            }

            return true;
        }
    }
}
