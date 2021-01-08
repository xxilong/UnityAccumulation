using System;
using Ventuz.Kernel.Input;

namespace Ventuz.Extention.Marker
{
    /// <summary>
    /// 原始触摸点坐标, 未经过屏幕长宽处理
    /// </summary>
    public class RawTouchPos
    {
        public RawTouchPos(double x, double y)
        {
            rx = x;
            ry = y;
        }

        public double rx = -1;
        public double ry = -1;
    }

    /// <summary>
    /// 供逻辑使用的转换过长宽比的坐标
    /// </summary>
    public class LogicTouchPos
    {
        public LogicTouchPos(RawTouchPos rpt)
        {
            if (rpt.rx > 0 && rpt.ry > 0)
            {
                x = rpt.rx * TouchScreen.Width;
                y = rpt.ry * TouchScreen.Height;
            }
        }

        public RawTouchPos ToRawPos()
        {
            return new RawTouchPos(x / TouchScreen.Width, y / TouchScreen.Height);
        }

        public LogicTouchPos()
        {
            x = y = -1;
        }

        public LogicTouchPos(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public LogicTouchPos(LogicTouchPos other)
        {
            x = other.x;
            y = other.y;
        }

        public LogicTouchPos(EventAtomTouch ev)
        {
            x = ev.X * TouchScreen.Width;
            y = ev.Y * TouchScreen.Height;
        }

        public static LogicTouchPos operator-(LogicTouchPos lhs, LogicTouchPos rhs)
        {
            return new LogicTouchPos(lhs.x - rhs.x, lhs.y - rhs.y);
        }      

        public LogicTouchPos clone()
        {
            return new LogicTouchPos(x, y);
        }

        public void copyfrom(LogicTouchPos other)
        {
            x = other.x;
            y = other.y;
        }

        public void moveoffset(LogicTouchPos other)
        {
            x += other.x;
            y += other.y;
        }

        public bool EqualTo(LogicTouchPos other)
        {
            return Math.Sqrt(square_dis_to(other)) < 1E-5;
        }

        public bool isvalid()
        {
            return x > 0 && y > 0;
        }

        public double square_dis_to(LogicTouchPos other)
        {
            double dltx = x - other.x;
            double dlty = y - other.y;
            return dltx * dltx + dlty * dlty;
        }

        public double real_dis_to(LogicTouchPos other)
        {
            return Math.Sqrt(square_dis_to(other));
        }
        
        public double x = -1;
        public double y = -1;
    }

    public class TouchInfo : LogicTouchPos
    {
        public TouchInfo(EventAtomTouch ev)
            : base(ev)
        {
            _deviceID = ev.DeviceID;
            _groupID = ev.TouchGroups;
            _id = ev.TouchID;
        }

        public int deviceID
        {
            get
            {
                return _deviceID;
            }
        }

        public int groupID
        {
            get
            {
                return _groupID;
            }
        }

        public string id
        {
            get
            {
                return _id;
            }
        }

        private int _deviceID;
        private int _groupID;
        private string _id;
    }
}
