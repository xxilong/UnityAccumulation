using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ShareLib.Unity
{
    public class Coroutine
    {
        static public Coroutine StartCoroutine(IEnumerator enumobj)
        {
            return new Coroutine(enumobj);
        }

        static public void StopCoroutine(Coroutine cor)
        {
            if(cor != null)
            {
                cor.Stop();
            }
        }

        public Coroutine(IEnumerator enumobj)
        {
            _enumobj = enumobj;

            int wait = 10;

            if (_enumobj.Current is int)
            {
                wait = (int)_enumobj.Current;
            }

            _runTimer = Delay.Run(wait, RunFrame);
        }

        public void Stop()
        {
            if(_runTimer != null)
            {
                _runTimer.Stop();
                _runTimer.Close();
                _runTimer = null;
            }

            if(_enumobj != null)
            {            
                _enumobj = null;
            }
        }

        public void Continue()
        {
            if(_runTimer == null)
            {
                RunFrame();
            }
        }

        private void RunFrame()
        {
            if(_enumobj == null)
            {
                return;
            }

            if(_waitContion != null)
            {
                if(_waitContion())
                {
                    _waitContion = null;
                }
                else
                {
                    _runTimer = Delay.Run(100, RunFrame);
                    return;
                }
            }

            if(!_enumobj.MoveNext())
            {
                Stop();
                return;
            }

            int wait = 1;

            if(_enumobj.Current is int)
            {
                wait = (int)_enumobj.Current;                
            }
            else if(_enumobj.Current is float)
            {
                wait = (int)(float)_enumobj.Current;
            }
            else if (_enumobj.Current is double)
            {
                wait = (int)(double)_enumobj.Current;
            }
            else if(_enumobj.Current is Func<bool>)
            {
                _waitContion = _enumobj.Current as Func<bool>;
                if(!_waitContion())
                {
                    //不满足条件, 继续等待
                    wait = 100;
                }
                else
                {
                    wait = 1;
                    _waitContion = null;
                }
            }
            else if(!int.TryParse(_enumobj.Current.ToString(), out wait))
            {
                wait = 1;
            }

            if(wait > 0)
            {
                _runTimer = Delay.Run(wait, RunFrame);
            }
            else
            {
                _runTimer.Stop();
                _runTimer.Close();
                _runTimer = null;
            }
        }

        private IEnumerator _enumobj = null;
        private Timer _runTimer = null;
        private Func<bool> _waitContion = null;
    }
}
