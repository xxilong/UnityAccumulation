using Ventuz.Kernel.Input;
using Ventuz.Extention.Marker;
using ShareLib.Conf;

namespace Ventuz.Extention.Detector
{
    public class MarkerItem
    {
        public MarkerItem(MarkerConfig config, string pt1, string pt2, string pt3)
        {
            _config = config;
            _pos = new LogicTouchPos();
            _angle = 0;
            _place_angle = 0;
            _range = 0;

            _pt1 = pt1;
            _pt2 = pt2;
            _pt3 = pt3;

            _isnew = true;
            _moved = false;
            _removed = false;
        }

        public string pt1 { get { return _pt1; } }
        public string pt2 { get { return _pt2; } }
        public string pt3 { get { return _pt3; } }
        public bool remove { get { return _removed; } }

        public void SetPosition(double x, double y)
        {
            if(_pos.x != x || _pos.y != y)
            {
                _pos.x = x;
                _pos.y = y;
                _moved = true;
            }

            int range = (int)(config.Radius * 1920 / TouchScreen.Width);
            if(range != _range)
            {
                _range = range;
                _moved = true;
            }
        }

        public void MoveOff(LogicTouchPos offset)
        {
            if(offset.x != 0 || offset.y != 0)
            {
                _pos.moveoffset(offset);
                p1pos.moveoffset(offset);
                p2pos.moveoffset(offset);
                p3pos.moveoffset(offset);
                _moved = true;
            }
        }

        public void SetAngle(double angle)
        {
            if(_angle != angle)
            {
                _angle = angle;
                _moved = true;
            }

            if(_isnew)
            {
                if(!ModConfig.getconf<bool>("marker", "fixedangle", false))
                {
                    _place_angle = angle;
                }
            }
        }

        public void Remove()
        {
            _removed = true;
        }

        public bool IsAround(LogicTouchPos pos)
        {
            if(_removed)
            {
                return false;
            }

            return _pos.real_dis_to(pos) < _config.Radius;
        }

        public EventAtom[] GetEvent(string markerid)
        {
            RawTouchPos _rpos = _pos.ToRawPos();

            if(_removed)
            {
                return new EventAtom[]{
                    new EventAtomTouch(DeviceTypes.MultiTouch, 1, "TouchRemove", 
                    1, Touch.TouchType.Marker, markerid, (float)_rpos.rx, (float)_rpos.ry, 0, _range, (float)(_angle - _place_angle))
                };
            }
            else if(_isnew)
            {
                _isnew = false;
                _moved = false;
                return new EventAtom[]{
                    new EventAtomTouch(DeviceTypes.MultiTouch, 1, "TouchAdd", 1,
                         Touch.TouchType.Marker, markerid, 0, 0, 0, 0, 0),
                    new EventAtomTouch(DeviceTypes.MultiTouch, 1, "TouchMove", 1,
                         Touch.TouchType.Marker, markerid, (float)_rpos.rx, (float)_rpos.ry, 0, _range, (float)(_angle - _place_angle))
                };
            }
            else if(_moved)
            {
                _moved = false;
                return new EventAtom[]{
                    new EventAtomTouch(DeviceTypes.MultiTouch, 1, "TouchMove", 1,
                    Touch.TouchType.Marker, markerid, (float)_rpos.rx, (float)_rpos.ry, 0, _range, (float)(_angle - _place_angle))
                };
            }
            
            return new EventAtom[0];
        }

        public void SetLocPoint(LogicTouchPos p1, LogicTouchPos p2, LogicTouchPos p3)
        {
            p1pos.copyfrom(p1);
            p2pos.copyfrom(p2);
            p3pos.copyfrom(p3);
        }

        public MarkerConfig config { get { return _config; } }

        private MarkerConfig _config;
        private LogicTouchPos _pos;
        private double _place_angle;  // 放入时的角度
        private double _angle;
        private int _range;

        private string _pt1;
        private string _pt2;
        private string _pt3;

        private bool _isnew;      // 是否是新加入
        private bool _moved;      // 是否移动过
        private bool _removed;    // 是否已经删除

        // 用于跟踪点丢失后的移动, 需要记录每个点的位置
        public LogicTouchPos p1pos = new LogicTouchPos();
        public LogicTouchPos p2pos = new LogicTouchPos();
        public LogicTouchPos p3pos = new LogicTouchPos();
    }
}
