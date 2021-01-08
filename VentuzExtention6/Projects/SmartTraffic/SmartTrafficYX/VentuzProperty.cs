using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Conf;

namespace SmartTrafficYX
{
    public class VentuzProperty
    {
        internal VentuzProperty()
        {
            isAtHomePage = true;
            isServerConnected = false;
            isShowWarning = false;
            isShowHandleRes = false;
            isUseTruck = false;
            isBlocked = false;
            isWaitBlock = false;
            isRedLight = false;
            isXPathFront = false;
            curArea = 0;
            curCar = 0;
            curTruck = 0;
            carBattle = 100;
            truckBattle = 100;
        }

        public void GotoHome()
        {
            if(isAtHomePage)
            {
                return;
            }

            isAtHomePage = true;
            isShowWarning = false;
            isShowHandleRes = false;
            curArea = 0;
            FireChanged();
        }

        public void SetControlStatus(bool hasControl)
        {
            if(isServerConnected == hasControl)
            {
                return;
            }

            isServerConnected = hasControl;
            FireChanged();
        }

        public void SetCarIndex(int robot)
        {
            if(curCar == robot)
            {
                return;
            }

            curCar = robot;
            carBattle = 100;

            SetVideoURL();
        }

        public void SetTruckIndex(int robot)
        {
            if(curTruck == robot)
            {
                return;
            }

            curTruck = robot;
            truckBattle = 100;
        }

        public void GotoArea(int area)
        {
            isAtHomePage = false;
            isShowWarning = false;
            isShowHandleRes = false;
            curArea = area;

            FireChanged();
        }

        public void ArrviedArea(int area)
        {
            isAtHomePage = false;
            isShowWarning = true;
            isShowHandleRes = false;
            curArea = area;

            FireChanged();
        }

        public void SetBattery(int quanity)
        {
            if(isUseTruck)
            {
                truckBattle = quanity;
            }
            else
            {
                carBattle = quanity;
            }

            FireChanged();
        }

        public void SetBlock()
        {
            isBlocked = true;
            isWaitBlock = false;

            FireChanged();
        }

        public void WaitBlock()
        {
            isBlocked = false;
            isWaitBlock = true;

            FireChanged();
        }

        public void ClearBlock()
        {
            isBlocked = false;
            isWaitBlock = false;

            FireChanged();
        }

        public void SetSpeed(bool isOn)
        {
            isSpeedOn = isOn;

            FireChanged();
        }

        public void SetUseTruck(bool useTruck)
        {
            isUseTruck = useTruck;

            FireChanged();
        }

        public void HandledArea(int area)
        {
            isAtHomePage = false;
            isShowWarning = false;
            isShowHandleRes = true;
            curArea = area;

            FireChanged();
        }

        public void SetPage(PageList page)
        {
            curPage = page;
            isAtHomePage = curPage == PageList.HOME;
            isPlayMovie = curPage == PageList.MOVIE;

            FireChanged();
        }

        public void SetRedLight(bool isRed)
        {
            isRedLight = isRed;

            FireChanged();
        }

        public void SetXPath(bool isAtPath)
        {
            isXPathFront = isAtPath;

            FireChanged();
        }

        internal void SetVideoURL()
        {
            videoURL = GlobalConf.getconf($"car{curCar}", "video_url");

            FireChanged();
        }

        internal void FireChanged()
        {
            OnPropertyChanged?.Invoke(this, 0);
        }

        public PageList curPage;        // 当前页面
        public bool isAtHomePage;       // 是否在首页
        public bool isServerConnected;  // 服务器是否连接
        public bool isShowWarning;      // 是否显示处理的提示框
        public bool isShowHandleRes;    // 是否显示处理结果
        public bool isUseTruck;         // 是否在使用货车
        public bool isBlocked;          // 遇到障碍中
        public bool isWaitBlock;        // 障碍已清除, 等待继续
        public bool isRedLight;         // 遇到红灯
        public bool isXPathFront;       // 红灯路口前
        public bool isSpeedOn;          // 速度盘是否显示数字
        public bool isPlayMovie;        // 是否显示视频播放页面
        public int curArea;             // 当前巡检区域
        public int curCar;              // 当前使用的轿车
        public int curTruck;            // 当前使用的货车
        public int carBattle;           // 轿车电量
        public int truckBattle;         // 货车电量
        public string videoURL;         // 视频 URL
        public EventHandler<int> OnPropertyChanged;
    }
}
