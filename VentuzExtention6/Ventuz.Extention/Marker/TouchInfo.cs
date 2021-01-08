namespace Ventuz.Extention.Marker
{
    public class TouchMarkerInfo : LogicTouchPos
    {
        public TouchMarkerInfo(string name, LogicTouchPos pos)
            : base(pos)
        {
            id = name;

            ismarker = false;
            usefull = false;
        }

        public string id;
        public bool ismarker;   // 该触摸已被识别为某个 Marker 的触摸点
        public bool usefull;    // 临时使用, 是否有可能属于 Marker 的触摸
    }
}
