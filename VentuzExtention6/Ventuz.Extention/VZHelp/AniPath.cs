using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Ventuz.Kernel;
using Ventuz.Kernel.Script;

namespace Ventuz.Extention.VZHelp
{
    public class AniPath
    {
        public enum StepType
        {
            PLAY_CONNECT,
            JUMP_STATUE,
            DELAY_SECONDS,
            GOTO_INDEX,
            EXEC_ACT,
            GOSTATUS_SEARCH,
        }

        public struct AniPathStep
        {
            public StepType type;
            public int cors;
            public float seconds;
            public Action<AniPath> action;
        }

        public AniPath(string pathdesc)
        {
            InitPathByStringDesc(pathdesc);
        }

        public AniPath()
        {
        }

        public void Run(IScriptedAnimation ani)
        {
            _curstep = 0;
            _ani = ani;
            _runcor = Coroutine.StartCoroutine(AniRunner());
        }

        public void Stop()
        {
            Coroutine.StopCoroutine(_runcor);
            _runcor = null;
        }

        public int StepCount => _pathlist.Count;
        public bool IsPlaying => _runcor != null;
        public bool IsAniPlaying {
            get
            {
                ScriptedAnimationControl ani = _ani as ScriptedAnimationControl;
                return ani.ControlledAnimation.Playing;
            }
        }
        public void Dump()
        {
            foreach(var sp in _pathlist)
            {
                switch(sp.type)
                {
                    case StepType.JUMP_STATUE:
                        Logger.Debug($"Jump to {sp.cors}");
                        break;
                    case StepType.PLAY_CONNECT:
                        Logger.Debug($"Play {sp.cors}");
                        break;
                    case StepType.GOTO_INDEX:
                        Logger.Debug($"Goto {sp.cors}");
                        break;
                    case StepType.DELAY_SECONDS:
                        Logger.Debug($"Delay {sp.seconds}s");
                        break;
                    case StepType.EXEC_ACT:
                        Logger.Debug($"Exec {sp.action}");
                        break;
                    case StepType.GOSTATUS_SEARCH:
                        Logger.Debug($"Search to {sp.cors}");
                        break;
                    default:
                        break;
                }
            }
        }

        private IEnumerator AniRunner()
        {
            while(_curstep < _pathlist.Count)
            {
                AniPathStep step = _pathlist.ElementAt(_curstep);
                ++_curstep;
                switch(step.type)
                {
                    case StepType.JUMP_STATUE:
                        _ani.Jump(step.cors);
                        Logger.Debug($"动画: Jump {step.cors}");
                        yield return 200;
                        break;

                    case StepType.PLAY_CONNECT:
                        _ani.Play(step.cors);
                        Logger.Debug($"动画: Play {step.cors}");
                        yield return 200;
                        yield return WaitAniFinished();
                        break;

                    case StepType.GOTO_INDEX:
                        _curstep = step.cors;
                        break;

                    case StepType.DELAY_SECONDS:
                        Logger.Debug($"动画: Delay {step.seconds}s");
                        yield return step.seconds * 1000;
                        break;

                    case StepType.EXEC_ACT:
                        step.action(this);
                        break;

                    case StepType.GOSTATUS_SEARCH:
                        yield return WaitGotoStatusWithSerach(step.cors);
                        break;

                    default:
                        break;
                }
            }

            Stop();
        }

        private Func<bool> WaitAniFinished()
        {
            ScriptedAnimationControl ani = _ani as ScriptedAnimationControl;            
            return () =>
            {
                if(ani == null)
                {
                    return true;
                }
                
                return !ani.ControlledAnimation.Playing;
            };
        }
        private Func<bool> WaitGotoStatusWithSerach(int st)
        {
            AniHelp anh = new AniHelp(_ani);
            anh.GotoStatus(st, 5);
            return () => !anh.IsPlaying;
        }
        
        #region 路径构造函数
        public void PushPlay(int c)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.PLAY_CONNECT;
            step.cors = c;
            _pathlist.Add(step);
        }

        public void PushFrontPlay(int c)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.PLAY_CONNECT;
            step.cors = c;
            _pathlist.Insert(0, step);
        }

        public void PushJump(int s)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.JUMP_STATUE;
            step.cors = s;
            _pathlist.Add(step);
        }

        public void PushDelay(float s)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.DELAY_SECONDS;
            step.seconds = s;
            _pathlist.Add(step);
        }

        public void PushRestart() => PushGoto(0);

        public void PushGoto(int index)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.GOTO_INDEX;
            step.cors = index;
            _pathlist.Add(step);
        }

        public void PushAction(Action<AniPath> act)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.EXEC_ACT;
            step.action = act;
            _pathlist.Add(step);
        }

        public void PushSearchToStatus(int st)
        {
            AniPathStep step = new AniPathStep();
            step.type = StepType.GOSTATUS_SEARCH;
            step.cors = st;
            _pathlist.Add(step);
        }

        #endregion

        /***************************************************
         * 用 - 分隔的路径描述
         *  <num>       前往指定状态
         *  !<num>      跳转到指定状态
         *  +/-<num>    播放指定的连接
         *  <           跳转到起点循环播放
         *  @<num>      延时指定的秒数
         * ************************************************/
        private void InitPathByStringDesc(string desc)
        {
            string[] paths = desc.Split('-');
            foreach(string des in paths)
            {
                string path = des.Trim();
                if(string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                int s = 0;
                float t = 0;

                switch(path[0])
                {
                    case '!':
                        if(int.TryParse(path.Substring(1), out s))
                        {
                            PushJump(s);
                        }
                        break;

                    case '+':
                    case '-':
                        if(int.TryParse(path, out s))
                        {
                            PushPlay(s);
                        }
                        break;

                    case '<':
                        PushRestart();
                        break;

                    case '@':
                        if(float.TryParse(path.Substring(1), out t))
                        {
                            PushDelay(t);
                        }
                        break;

                    default:
                        if(int.TryParse(path, out s))
                        {
                            PushSearchToStatus(s);
                        }
                        break;
                }
            }
        }

        private List<AniPathStep> _pathlist = new List<AniPathStep>();
        private int _curstep = 0;
        private Coroutine _runcor = null;
        private IScriptedAnimation _ani = null;
    }
}
