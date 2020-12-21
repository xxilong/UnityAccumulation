using System;
using System.Collections.Generic;

namespace ShareLib.Unity
{
    public class ThreadPool
    {
        private float lastuptime = 0;
        private object locker = new object();
        private Queue<Action> _runners = new Queue<Action>();
        private static ThreadPool _instance = new ThreadPool();

        public static void RunInUI(Action act)
        {
            _instance._RunInUI(act);
        }

        public static void CheckInUpdate(float curtime)
        {
            _instance._CheckInUpdate(curtime);
        }

        public static void CheckInUpdate()
        {
            _instance._CheckInUpdate(Environment.TickCount / 1000.0f);
        }

        public static void NoCheckUpdate()
        {
            _instance._NoCheckInUpdate();
        }

        private void _RunInUI(Action act)
        {
            lock (locker)
            {
                _runners.Enqueue(act);
            }
        }

        private void _CheckInUpdate(float curtime)
        {
            if (curtime - lastuptime < 0.1f)
            {
                return;
            }

            lastuptime = curtime;
            lock (locker)
            {
                while (_runners.Count > 0)
                {
                    Action act = _runners.Dequeue();
                    act();
                }
            }
        }

        private void _NoCheckInUpdate()
        {
            lock (locker)
            {
                while (_runners.Count > 0)
                {
                    Action act = _runners.Dequeue();
                    act();
                }
            }
        }
    }
}
