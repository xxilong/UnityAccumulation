using System.Collections.Generic;

public enum PageList
{
    HOME,
    MOVIE,  // 视频模式

    CAR_CHECK_READY,
    CAR_DIRVER_READY,
    CAR_TO_GASSTATION,
    CAR_ARRIVED_GASSTATION,
    CAR_TO_WORK,
    CAR_ARRIVED_WORK,
    CAR_TO_MARKET,
    CAR_ARRIVED_MARKET,
    CAR_TO_HOME,
    CAR_ARRIVED_HOME,

    XJ_READY,
    XJ_ARRIVED_OLD,
    XJ_TO_NEW,
    XJ_ARRIVED_NEW,
    XJ_TO_HOME,
    XJ_DONE,
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
        { PageList.MOVIE, new PageAttr(PageMode.MOVIE) },
        { PageList.CAR_CHECK_READY, new PageAttr(PageMode.WAITCLICK) },
        { PageList.CAR_DIRVER_READY, new PageAttr(PageMode.WAITCLICK) },
        { PageList.CAR_TO_GASSTATION, new PageAttr(PageMode.RUNNING, 1) },
        { PageList.CAR_ARRIVED_GASSTATION, new PageAttr(PageMode.WAITCLICK, 1) },
        { PageList.CAR_TO_WORK, new PageAttr(PageMode.RUNNING, 2) },
        { PageList.CAR_ARRIVED_WORK, new PageAttr(PageMode.WAITCLICK, 2) },
        { PageList.CAR_TO_MARKET, new PageAttr(PageMode.RUNNING, 3) },
        { PageList.CAR_ARRIVED_MARKET, new PageAttr(PageMode.WAITCLICK, 3) },
        { PageList.CAR_TO_HOME, new PageAttr(PageMode.RUNNING, 4) },
        { PageList.CAR_ARRIVED_HOME, new PageAttr(PageMode.WAITCLICK, 4) },
        { PageList.XJ_READY, new PageAttr(PageMode.WAITCLICK) },
        { PageList.XJ_ARRIVED_OLD, new PageAttr(PageMode.WAITCLICK) },
        { PageList.XJ_TO_NEW, new PageAttr(PageMode.RUNNING, 5) },
        { PageList.XJ_ARRIVED_NEW, new PageAttr(PageMode.WAITCLICK, 5) },
        { PageList.XJ_TO_HOME, new PageAttr(PageMode.RUNNING, 6) },
        { PageList.XJ_DONE, new PageAttr(PageMode.WAITCLICK, 6) },
    };

    static public PageAttr Attr(PageList page)
    {
        return attrlist[page];
    }
}