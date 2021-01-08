using ShareLib.Ayz;
using ShareLib.Log;
using ShareLib.Net;
using ShareLib.Packer;
using ShareLib.Page;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using Ventuz.Extention.Compatible;

namespace Ventuz.Extention.Control
{
    public class ControlServer
    {
        static public ControlServer Instance = new ControlServer();

        public void Start(int port)
        {
            if (hasStart)
            {
                return;
            }

            IFileReader reader = null;
            if(VentuzWare.Instance.IsVprMode)
            {
                reader = new VentuzSchemeReader("ventuz://pages/");
            }
            else
            {
                reader = new DiskFileReader(VentuzWare.Instance.GetProjectPath() + "Pages");
            }

            _server.SetReciverListener(OnReciveClientCommand);
            _server.SetCtrlChangedListener(OnControlClientChange);
            _server.Start(port, reader);

            hasStart = true;
            OnStateChanged?.Invoke();
        }

        public void SendCommand(string cmd)
        {
            if (!hasStart)
            {
                return;
            }

            Logger.Info($"发送命令: {cmd}");
            _server.SendCommand(cmd);
        }

        public void SwitchControlPage(string name) => _server.SwitchPage(name);
        public void SetPageContent(string name, string content) => _server.SetPageContent(name, content);
        public PageControlServer Server
        {
            get => _server;
        }

        private void OnControlClientChange(bool hasCtrl)
        {
            Logger.Info($"控制端连接: {hasCtrl}");
            if(hasCtrl)
            {
                Delay.Run(200, () => {
                    OnNewClientConnected?.Invoke();
                });
            }
            this.hasCtrl = hasCtrl;
            OnStateChanged?.Invoke();
        }

        private void OnReciveClientCommand(string cmd)
        {
            if(OnReciveCommand != null)
            {
                OnReciveCommand?.Invoke(cmd);
                return;
            }

            Logger.Info($"收到 Pad 命令: {cmd}");

            CmdLine command = new CmdLine(cmd);

            if(RunRegCommand(command.cmd, command))
            {
                return;
            }

            switch (command.cmd)
            {
                case "go":
                    if (command.getarg<string>(0, out string page))
                    {
                        Logger.Info($"切换页面 {page}");
                        Page.Control.GotoPage(page);
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

        private bool RunRegCommand(string cmd, CmdLine line)
        {
            if(_regHander.ContainsKey(cmd))
            {
                _regHander[cmd]?.Invoke(line);
                return true;
            }

            return false;
        }

        public void RegLanguageHander(Action<string> act) => _server.SetLanguageChangedListener(act);

        public void UnRegCmdHander(string cmd)
        {
            _regHander.Remove(cmd);
        }

        public void RegCmdHander(string cmd, Action<CmdLine> act)
        {
            _regHander[cmd] = act;
        }

        #region 重载不同参数的命令注册函数, 最多支持 6 个参数
        public void RegCmdHander(string cmd, Action act) =>
            _regHander[cmd] = (CmdLine line) => { act(); };
        public void RegCmdHander<T>(string cmd, Action<T> act) =>
            _regHander[cmd] = (CmdLine line) =>
            {
                if(line.getarg<T>(0, out T r))
                {
                    act(r);
                }
            };
        public void RegCmdHander<T1, T2>(string cmd, Action<T1, T2> act) =>
            _regHander[cmd] = (CmdLine line) =>
            {
                if(line.getarg<T1>(0, out T1 r1) && line.getarg<T2>(1, out T2 r2))
                {
                    act(r1, r2);
                }
            };
        public void RegCmdHander<T1, T2, T3>(string cmd, Action<T1, T2, T3> act) =>
            _regHander[cmd] = (CmdLine line) =>
            {
                if (
                line.getarg<T1>(0, out T1 r1) &&
                line.getarg<T2>(1, out T2 r2) &&
                line.getarg<T3>(2, out T3 r3))
                {
                    act(r1, r2, r3);
                }
            };
        public void RegCmdHander<T1, T2, T3, T4>(string cmd, Action<T1, T2, T3, T4> act) =>
            _regHander[cmd] = (CmdLine line) =>
            {
                if (
                line.getarg<T1>(0, out T1 r1) &&
                line.getarg<T2>(1, out T2 r2) &&
                line.getarg<T3>(2, out T3 r3) &&
                line.getarg<T4>(3, out T4 r4))
                {
                    act(r1, r2, r3, r4);
                }
            };
        public void RegCmdHander<T1, T2, T3, T4, T5>(string cmd, Action<T1, T2, T3, T4, T5> act) =>
            _regHander[cmd] = (CmdLine line) =>
            {
                if (
                line.getarg<T1>(0, out T1 r1) &&
                line.getarg<T2>(1, out T2 r2) &&
                line.getarg<T3>(2, out T3 r3) &&
                line.getarg<T4>(3, out T4 r4) &&
                line.getarg<T5>(4, out T5 r5))
                {
                    act(r1, r2, r3, r4, r5);
                }
            };

        public void RegCmdHander<T1, T2, T3, T4, T5, T6>(string cmd, Action<T1, T2, T3, T4, T5, T6> act) =>
           _regHander[cmd] = (CmdLine line) =>
           {
               if (
               line.getarg<T1>(0, out T1 r1) &&
               line.getarg<T2>(1, out T2 r2) &&
               line.getarg<T3>(2, out T3 r3) &&
               line.getarg<T4>(3, out T4 r4) &&
               line.getarg<T5>(4, out T5 r5) &&
               line.getarg<T6>(5, out T6 r6))
               {
                   act(r1, r2, r3, r4, r5, r6);
               }
           };
        #endregion


        //
        public Action<string> OnReciveCommand;
        public Action OnNewClientConnected;

        // 
        public bool hasCtrl = false;
        public bool hasStart = false;
        public string localIP = NetUnity.GetLocalIPs();
        public Action OnStateChanged;

        private PageControlServer _server = new PageControlServer();
        private Dictionary<string, Action<CmdLine>> _regHander = new Dictionary<string, Action<CmdLine>>();
    }
}
