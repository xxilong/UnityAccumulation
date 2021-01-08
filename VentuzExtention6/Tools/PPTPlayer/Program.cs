using System;
using ShareLib.Conf;
using ShareLib.Net;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.IO;
using ShareLib.Ayz;
using ShareLib.Log;
using System.Threading;

namespace PPTPlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RealMain();
        }

        private void RealMain()
        {
            _app = new PowerPoint.Application();

            try
            {
                _present = _app.Presentations.Open(
                    GlobalConf.getconf_aspath("play", "ppt"), MsoTriState.msoCTrue);
                slideCount = _present.Slides.Count;
                _app.SlideShowNextSlide += OnSlideShowChanged;
                Logger.Info($"PPT文件已打开, 页面数量: {slideCount}");

                //_present.SlideShowSettings.LoopUntilStopped = MsoTriState.msoCTrue;
                _present.SlideShowSettings.AdvanceMode = PowerPoint.PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                _present.SlideShowSettings.Run();
            }
            catch(Exception e)
            {
                Logger.Error($"PPT文件打开失败: {e.Message}");
            }
            
            _srv.SetReciverListener(OnReciveCommand);
            _srv.Start(
                GlobalConf.getconf<int>("play", "port", 3131),
                Path.Combine(PathHelp.appDir, "pages"),
                GlobalConf.getconf<string>("pages", "index", "index"));

            while (true)
            {
                Console.ReadKey();
            }
        }

        private void OnSlideShowChanged(PowerPoint.SlideShowWindow Win)
        {
            Logger.Debug($"当前页面: {Win.View.CurrentShowPosition}/{slideCount}");
            _srv.SendCommand($"+text pageinfo 当前播放:_{Win.View.CurrentShowPosition}/{slideCount}");
        }

        private void OnReciveCommand(string cmdline)
        {
            Logger.Info($"recived command:{cmdline}");
            CmdLine cmd = new CmdLine(cmdline);
            switch(cmd.cmd)
            {
                case "next":
                    if(_present.SlideShowWindow.View.CurrentShowPosition >= slideCount)
                    {
                        break;
                    }
                    _present.SlideShowWindow.View.Next();
                    break;

                case "prev":
                    if (_present.SlideShowWindow.View.CurrentShowPosition <= 1)
                    {
                        break;
                    }
                    _present.SlideShowWindow.View.Previous();
                    break;

                case "home":
                    _present.SlideShowWindow.View.First();
                    break;

                case "goto":
                    if(cmd.getarg<int>(0, out int pageIndex))
                    {
                        _present.SlideShowWindow.View.GotoSlide(pageIndex);
                    }
                    break;

                case "auto":
                    autoMode = !autoMode;
                    try
                    {
                        autoSlidePos = _present.SlideShowWindow.View.CurrentShowPosition;
                        _present.SlideShowWindow.View.Exit();
                    }
                    catch(Exception e)
                    {
                        Logger.Error($"{e.Message}");
                    }

                    _present.SlideShowSettings.AdvanceMode = autoMode ?
                        PowerPoint.PpSlideShowAdvanceMode.ppSlideShowUseSlideTimings :
                        PowerPoint.PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                    //_present.SlideShowSettings.StartingSlide = autoSlidePos;
                    _present.SlideShowSettings.Run();
                    
                    try
                    {
                        if(autoSlidePos > 0 && autoSlidePos <= slideCount)
                        {
                            _present.SlideShowWindow.View.GotoSlide(autoSlidePos);
                            Logger.Info($"Restart at {autoSlidePos} with auto mode {autoMode}");
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Error($"{e.Message}");
                    }
                    
                    _srv.SetCheck("auto", autoMode);
                    break;

                default:
                    Logger.Error($"unknown command {cmd.cmd}");
                    break;
            }
        }

        private PageControlServer _srv = new PageControlServer();
        private PowerPoint.Application _app;
        private PowerPoint.Presentation _present;
        private bool autoMode = false;
        private int autoSlidePos = 1;
        private int slideCount = 0;
    }
}
