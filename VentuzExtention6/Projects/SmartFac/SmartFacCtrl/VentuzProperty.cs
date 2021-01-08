using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Conf;

namespace SmartFacCtrl
{
    public class VentuzProperty
    {
        internal VentuzProperty()
        {
            isAtHomePage = true;
            isAutoMode = false;
            isServerConnected = false;
            isShowWarning = false;
            isShowHandleRes = false;
            curArea = 0;
            curRobot = 0;
            batQuantity = 100;
        }

        public void GotoHome()
        {
            if(isAtHomePage)
            {
                return;
            }

            isAtHomePage = true;
            isAutoMode = false;
            isShowWarning = false;
            isShowHandleRes = false;
            curArea = 0;
            FireChanged();
        }

        public void SetAutoMode(bool isAuto)
        {
            if(isAutoMode == isAuto)
            {
                return;
            }

            isAutoMode = isAuto;
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

        public void SetRobot(int robot)
        {
            if(curRobot == robot)
            {
                return;
            }

            curRobot = robot;
            batQuantity = 100;
            SetVideoURL();

            FireChanged();
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
            batQuantity = quanity;

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

        internal void SetVideoURL()
        {
            videoURL = GlobalConf.getconf($"robot{curRobot}", "video_url");
        }

        internal void FireChanged()
        {
            OnPropertyChanged?.Invoke(this, 0);
        }
        
        public bool isAtHomePage;   // 是否在首页
        public bool isAutoMode;     // 是否是自动模式
        public bool isServerConnected;  // 服务器是否连接
        public bool isShowWarning;      // 是否显示处理的提示框
        public bool isShowHandleRes;    // 是否显示处理结果
        public int curArea;             // 当前巡检区域
        public int curRobot;            // 当期使用的机器人
        public int batQuantity;         // 电池电量
        public string videoURL;         // 视频 URL
        public EventHandler<int> OnPropertyChanged;
    }
}
