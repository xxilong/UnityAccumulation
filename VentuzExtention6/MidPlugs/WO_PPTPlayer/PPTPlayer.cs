using System;
using System.Threading;
using ShareLib.Ayz;
using MidCtrl;
using ShareLib.Log;
using Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using ShareLib.Conf;
using System.IO;
using ShareLib.Unity;
using System.Diagnostics;

namespace WO
{
    public class PPTPlayer : WOPlugIndepend<PPTPlayer>
    {
        public override bool init()
        {
            string file = GetArgument(0);
            if(string.IsNullOrWhiteSpace(file))
            {
                Logger.Warning($"[PPTPlayer] no file to open.");
                return false;
            }

            if(!file.StartsWith("http"))
            {
                file = Path.Combine(PathHelp.dllDir, file);
            }

            Logger.Debug($"[PPTPlayer] try open: {file}");
            if(_app != null)
            {
                destory();
            }

            TerminatePowerPoint();
            _app = new PowerPoint.Application();
            try
            {
                _present = _app.Presentations.Open(file, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                slideCount = _present.Slides.Count;
                _app.SlideShowNextSlide += OnSlideShowChanged;
                Logger.Info($"PPT文件已打开, 页面数量: {slideCount}");
                //_present.SlideShowSettings.AdvanceMode = PowerPoint.PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                _present.SlideShowSettings.Run();
            }
            catch (Exception e)
            {
                destory();
                Logger.Error($"PPT文件打开失败: {e.Message}");
            }
            Logger.Debug($"[PPTPlayer] init powerpoint done.");
            return true;
        }

        public override bool show()
        {
            if(_app == null)
            {
                reopen();
            }

            PowerPoint.SlideShowWindow slider = null;
            try
            {
                slider = _present.SlideShowWindow;
            }
            catch
            {
                Logger.Warning($"[PPTPlayer] present window not found, reopen.");
                _present.SlideShowSettings.Run();
                return false;
            }

            IntPtr hWnd = (IntPtr)_present.SlideShowWindow.HWND;
            WinHelp.ShowWindow(hWnd, 1);
            Logger.Debug($"[PPTPlayer] show ppt window.");
            return true;
        }

        public override bool hide()
        {
            PowerPoint.SlideShowWindow slider = null;
            try
            {
                slider = _present.SlideShowWindow;
            }
            catch
            {
                Logger.Warning($"[PPTPlayer] present window not found, hide ingored.");
                return false;
            }

            IntPtr hWnd = (IntPtr)_present.SlideShowWindow.HWND;
            WinHelp.ShowWindow(hWnd, 0);
            Logger.Debug($"[PPTPlayer] hide ppt window.");
            return true;
        }

        public override void destory()
        {
            Logger.Debug($"[PPTPlayer] destory.");

            if (_present != null)
            {
                _present.Close();
                _present = null;
            }

            if(_app != null)
            {
                //_app.Quit();
                _app = null;
            }
        }

        public void close()
        {
            Logger.Info("try close powerpoint.");
            if(_present != null)
            {
                _present.Close();
                _present = null;
            }

            if(_app != null)
            {
                //_app.Quit();
                _app = null;
            }

            TerminatePowerPoint();
        }

        private void TerminatePowerPoint()
        {
            var processs = Process.GetProcessesByName("POWERPNT");
            foreach(var p in processs)
            {
                Logger.Debug($"Terminate Process {p.Id}");
                p.Kill();
            }
        }

        public void reopen()
        {
            Logger.Info("reopen powerpoint.");
            init();
        }

        public override void OnRecvCommand(string cmdline)
        {
            Logger.Debug($"[PPTPlayer] recived command {cmdline}");


            CmdLine cmd = new CmdLine(cmdline);
            if(cmd.cmd == "close")
            {
                close();
                return;
            }

            if (cmd.cmd == "reopen")
            {
                reopen();
                return;
            }            

            PowerPoint.SlideShowWindow slider = null;
            try
            {
                slider = _present.SlideShowWindow;
            }
            catch
            {
                Logger.Error($"[PPTPlayer] present window not found any more.");
                return;
            }

            switch (cmd.cmd)
            {
                case "next":
                    if (slider.View.CurrentShowPosition >= slideCount)
                    {
                        break;
                    }
                    slider.View.Next();
                    break;

                case "prev":
                    if (slider.View.CurrentShowPosition <= 1)
                    {
                        break;
                    }
                    slider.View.Previous();
                    break;

                case "home":
                    slider.View.First();
                    break;

                case "goto":
                    if (cmd.getarg<int>(0, out int pageIndex))
                    {
                        slider.View.GotoSlide(pageIndex);
                    }
                    break;

                case "auto":
                    autoMode = !autoMode;
                    try
                    {
                        autoSlidePos = _present.SlideShowWindow.View.CurrentShowPosition;
                        slider.View.Exit();
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"{e.Message}");
                    }

                    _present.SlideShowSettings.AdvanceMode = autoMode ?
                        PowerPoint.PpSlideShowAdvanceMode.ppSlideShowUseSlideTimings :
                        PowerPoint.PpSlideShowAdvanceMode.ppSlideShowManualAdvance;
                    _present.SlideShowSettings.Run();

                    try
                    {
                        if (autoSlidePos > 0 && autoSlidePos <= slideCount)
                        {
                            slider.View.GotoSlide(autoSlidePos);
                            Logger.Info($"Restart at {autoSlidePos} with auto mode {autoMode}");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"{e.Message}");
                    }
                    break;

                default:
                    Logger.Error($"unknown command {cmd.cmd}");
                    break;
            }
        }

        private void OnSlideShowChanged(PowerPoint.SlideShowWindow Win)
        {
            Logger.Debug($"当前页面: {Win.View.CurrentShowPosition}/{slideCount}");
        }

        private PowerPoint.Application _app = null;
        private PowerPoint.Presentation _present = null;
        private bool autoMode = false;
        private int autoSlidePos = 1;
        private int slideCount = 0;

        static void Main(string[] args)
        {
            CSharpInitalize(args);
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
