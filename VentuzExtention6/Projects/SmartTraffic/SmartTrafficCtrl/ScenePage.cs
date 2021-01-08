using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartTrafficCtrl
{
    public enum PageList
    {
        HOME,
        TRUCK,  // 卡车
        MOVIE,  // 视频模式

        CAR_SELFCHECKREADY,
        CAR_PATHTOOILREADY,
        CAR_MVTO_OILFRONT,
        CAR_REPATHTOOIL,

        CAR_MVTO_OIL,
        CAR_ADD_OIL,
        CAR_TOBUSIESSREADY,

        CAR_MVTO_BUSIESS,
        CAR_ARRIVED_BUSIESS,

        CAR_MVTO_PARK,
        CAR_PARK_READY,
        CAR_PARK_DONE,
        CAR_PATHTOHOMEREADY,

        CAR_MVTO_HOME,
        CAR_ARRIVED_HOME,
    }

    public enum PageMode
    {
        HOME,       // 主页模式
        MOVIE,      // 视频播放模式
        ANIMAL,     // 播放动画中
        WAITCLICK,  // 等待用户点击
        RUNNING,    // 车辆移动中
    }

    public class PageAttr
    {
        public PageAttr(PageMode mode, int index = 0)
        {
            areaIndex = index;
            this.mode = mode;
        }

        public PageMode mode;
        public int areaIndex;
    }

    public class Page
    {
        static Dictionary<PageList, PageAttr> attrlist = new Dictionary<PageList, PageAttr>() {
            { PageList.HOME, new PageAttr(PageMode.HOME) },
            { PageList.TRUCK, new PageAttr(PageMode.RUNNING) },
            { PageList.MOVIE, new PageAttr(PageMode.MOVIE) },
            { PageList.CAR_SELFCHECKREADY, new PageAttr(PageMode.WAITCLICK) },
            { PageList.CAR_PATHTOOILREADY, new PageAttr(PageMode.WAITCLICK) },
            { PageList.CAR_MVTO_OILFRONT, new PageAttr(PageMode.RUNNING, 1) },
            { PageList.CAR_REPATHTOOIL, new PageAttr(PageMode.WAITCLICK, 1) },
            { PageList.CAR_MVTO_OIL, new PageAttr(PageMode.RUNNING, 2) },
            { PageList.CAR_ADD_OIL, new PageAttr(PageMode.WAITCLICK, 2) },
            { PageList.CAR_TOBUSIESSREADY, new PageAttr(PageMode.WAITCLICK, 2) },
            { PageList.CAR_MVTO_BUSIESS, new PageAttr(PageMode.RUNNING, 3) },
            { PageList.CAR_ARRIVED_BUSIESS, new PageAttr(PageMode.WAITCLICK, 3) },
            { PageList.CAR_MVTO_PARK, new PageAttr(PageMode.RUNNING, 4) },
            { PageList.CAR_PARK_READY, new PageAttr(PageMode.WAITCLICK, 4) },
            { PageList.CAR_PARK_DONE, new PageAttr(PageMode.WAITCLICK, 4) },
            { PageList.CAR_PATHTOHOMEREADY, new PageAttr(PageMode.WAITCLICK, 4) },
            { PageList.CAR_MVTO_HOME, new PageAttr(PageMode.WAITCLICK, 0) },
            { PageList.CAR_ARRIVED_HOME, new PageAttr(PageMode.WAITCLICK, 0) },
        };

        static public PageAttr Attr(PageList page)
        {
            return attrlist[page];
        }
    }
}
