using ShareLib.Ayz;
using ShareLib.Log;
using ShareLib.Page;
using Ventuz.Extention.Control;

namespace P2P5G
{
    public class MainControl
    {
        public static MainControl Instance = new MainControl();

        public void StartUp(string pagetree)
        {
            // 初始化日志功能
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new FileLogger(),
            });

            Page.Control.InitPages(typeof(PageList), pagetree);
            Page.Control.OnPageChanged += OnPageChanged;

            // 启动服务器
            ControlServer.Instance.OnReciveCommand = OnPadCommand;
            ControlServer.Instance.OnNewClientConnected = () => { OnPageChanged(this, Page.Control.CurPage); };
            ControlServer.Instance.Start(5875);
        }

        public void ResetPageTree(string treedef)
        {
            Page.Control.InitPages(typeof(PageList), treedef);
        }

        public void CloseUp()
        {
        }

        private void OnPadCommand(string cmd)
        {
            Logger.Info($"收到 Pad 命令: {cmd}");

            CmdLine command = new CmdLine(cmd);
            switch (command.cmd)
            {
                case "set_car":
                    if (command.getarg<int>(0, out int robotIndex))
                    {
                        Logger.Info($"设置使用的轿车编号: {robotIndex}");
                    }
                    break;

                case "set_page":
                    if (command.getarg<int>(0, out int pageIndex))
                    {
                        Logger.Info($"切换页面 {pageIndex}");
                        PageList page = (PageList)pageIndex;
                        Logger.Info($"页面名称: {page}");
                        Page.Control.GotoPage(pageIndex);
                    }
                    break;

                case "continue":
                    Page.Control.SetProperty("pause", "false");
                    break;

                case "pause":
                    Page.Control.SetProperty("pause", "true");
                    break;

                case "next":
                    Page.Control.GoNextPageByTree();
                    break;

                case "prev":
                    Page.Control.GoPrevPageByTree();
                    break;

                default:
                    Logger.Error($"未知命令: { command.cmd }");
                    break;
            }
        }

        private void OnPageChanged(object sender, int page)
        {
            ControlServer.Instance.SendCommand($"enter_page {page}");
            Logger.Debug($"向 Pad 发送命令: enter_page {page}");
        }
    }
}
