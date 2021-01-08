using System;
using ShareLib.Log;
using ShareLib.Net;
using System.Timers;

namespace SmartAgricul
{
    public class MainControl
    {
        public static MainControl Instance = new MainControl();

        public void StartUp()   // 程序启动
        { 
            // 启动服务器
            _server.SetReciver(OnServerCmd);
            _server.SetCtrlChanged(OnControlClientChange);
            _server.Start(25380);
            _board.Start(12380);
        }

        public void CloseUp()
        {
            blockCheckTimer.Stop();
            _board.Stop();
            _server.Stop();
        }

        public void OnServerCmd(string cmd)
        {
            Logger.Info($"收到 Pad 命令: {cmd}");

            string[] getData = cmd.Split('-');

            if (getData.Length < 2)
            {
                return;
            }

            if (getData[0] != "1")
            {
                SetVideoIndex(getData[1]);
            }
        }

        public void OnControlClientChange(bool hasCtrl)
        {
            Logger.Info($"控制端连接: {hasCtrl}");
        }

        private void SetVideoIndex(string video)
        {
            Logger.Info($"SetVideoIndex {video}");

            VideoURL = "ventuz://Movies/" + video + ".mp4";
            VideoChanged?.Invoke();
        }

        private CmdTcpServerShortLen _server = new CmdTcpServerShortLen();
        private UDPBroadCastServerAddr _board = new UDPBroadCastServerAddr();        
        private Timer blockCheckTimer = new Timer(400);

        public string VideoURL = "ventuz://Movies/1.mp4";
        public Action VideoChanged;
   }
}
