using ShareLib.Log;
using System;
using System.Collections.Generic;
using Ventuz.Kernel;
using Ventuz.Kernel.Animation;
using Ventuz.Kernel.Script;

namespace Ventuz.Extention.VZHelp
{
    public class AniHelp
    {
        public AniHelp(IScriptedAnimation animal)
        {
            IAniCtrl = animal;
            if(IAniCtrl != null)
            {
                AniCtrl = IAniCtrl as ScriptedAnimationControl;
            }

            if(AniCtrl != null)
            {
                IAni = AniCtrl.ControlledAnimation;
            }

            if(IAni != null)
            {
                Ani = IAni as Animation;
                if(Ani != null)
                {
                    Ani.StateMachineStatusChanged += (object sender, EventArgs e) =>
                    {
                        bool isPlaying = IAni != null && IAni.Playing;
                        if(isPlaying != _lastIsPlaying)
                        {
                            _lastIsPlaying = isPlaying;
                            _PlayStatusChanged?.Invoke(_lastIsPlaying);
                        }
                    };
                }
            }

            InitConnectMap();
        }

        // 从动画的当前状态前往指定的状态
        // 若搜索 maxstep 之后仍然无法到达该状态, 则不播动画, 直接跳转到此状态.
        public void GotoStatus(int st, int maxstep = 1)
        {
            if(!_StateConnections.ContainsKey(st))
            {
                Logger.Error($"未找到动画状态 {st}!");
                return;
            }

            Stop();
            IAniCtrl.GetStatus(out int c, out int s);

            Logger.Debug($"AniStatus Go {s} -> {st}");
            _curRunPath = SearchAniConnect(s, st, maxstep);

            if(_curRunPath == null)
            {
                Logger.Warning($"NoPath Found {s} => {st}");
                IAniCtrl.Jump(st);
                return;
            }

            _curRunPath.Run(IAniCtrl);
        }

        public AniPath SearchAniConnect(int from, int to, int maxstep)
        {
            _searched.Clear();
            return SearchPath(from, to, maxstep);
        }

        public void GotoStatus(string st, int maxstep = 1) => GotoStatus(FindStateByName(st), maxstep);

        public void RunPath(string pathdesc)
        {
            Stop();
            _curRunPath = new AniPath(pathdesc);
            _curRunPath.Run(IAniCtrl);
        }

        public int FindStateByName(string name)
        {
            if (!Valid)
            {
                return -1;
            }

            foreach (AnimationState st in IAniCtrl.States)
            {
                if (st.Name == name)
                {
                    return st.ID;
                }
            }

            Logger.Error($"未找到动画状态 {name}!");
            return -1;
        }

        public bool IsPlaying
        {
            get
            {
                if(_curRunPath != null && _curRunPath.IsPlaying)
                {
                    return true;
                }

                if(IAni != null && IAni.Playing)
                {
                    return true;
                }

                return false;
            }
        }

        public void Stop()
        {
            if(_curRunPath != null)
            {
                _curRunPath.Stop();
                _curRunPath = null;
            }
        }

        public void SetPlayStatusListener(Action<bool> act) => _PlayStatusChanged = act;

        private bool Valid
        {
            get
            {
                if(IAniCtrl == null)
                {
                    Logger.Warning($"空指针, 未正确创建动画对象");
                    return false;
                }

                if(!IAniCtrl.IsConnected)
                {
                    Logger.Warning($"动画未连接");
                    return false;
                }

                return true;
            }
        }

        private void InitConnectMap()
        {
            foreach(var s in IAniCtrl.States)
            {
                _StateConnections[s.ID] = new Dictionary<int, int>();
            }

            foreach(var c in IAniCtrl.Connections)
            {
                _StateConnections[c.FromStateID][c.ToStateID] = c.ID;
                if(!_StateConnections[c.ToStateID].ContainsKey(c.FromStateID))
                {
                    _StateConnections[c.ToStateID][c.FromStateID] = -c.ID;
                }
            }
        }

        // 广度遍历搜索
        private AniPath SearchPath(int from, int to, int maxstep)
        {
            if(from == to)
            {
                return new AniPath();
            }

            if(maxstep <= 0)
            {
                return null;
            }

            if(from == 0)
            {
                return null;
            }

            _searched.Add(from);

            Dictionary<int, int> cons = _StateConnections[from];
            int minstep = maxstep + 2;
            AniPath minPath = null;

            foreach(var item in cons)
            {
                if(item.Key == to)
                {
                    AniPath path = new AniPath();
                    path.PushPlay(item.Value);
                    return path;
                }

                if(_searched.Contains(item.Key))
                {
                    continue;
                }

                AniPath spath = SearchPath(item.Key, to, maxstep - 1);
                if(spath == null)
                {
                    continue;
                }

                if(spath.StepCount < minstep)
                {
                    minstep = spath.StepCount;
                    minPath = spath;
                    minPath.PushFrontPlay(item.Value);
                }
            }

            return minPath;
        }

        private IScriptedAnimation IAniCtrl = null;
        private ScriptedAnimationControl AniCtrl = null;
        private IAnimation IAni = null;
        private Animation Ani = null;
        private Dictionary<int, Dictionary<int, int>> _StateConnections = new Dictionary<int, Dictionary<int, int>>();
        private AniPath _curRunPath = null;
        private HashSet<int> _searched = new HashSet<int>();
        private Action<bool> _PlayStatusChanged = null;
        private bool _lastIsPlaying = false;
    }
}
