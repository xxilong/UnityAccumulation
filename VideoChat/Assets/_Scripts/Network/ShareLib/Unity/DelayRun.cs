using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ShareLib.Unity
{
    public class Delay
    {
        public static Timer Run(int ms, Action act)
        {
            Timer runTimer = new Timer(ms);
            runTimer.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                act();
            };

            runTimer.AutoReset = false;
            runTimer.Enabled = true;
            runTimer.Start();

            return runTimer;
        }

        public static Timer Every(int ms, Action act)
        {
            Timer runTimer = new Timer(ms);
            runTimer.Elapsed += delegate (object sender, ElapsedEventArgs e)
            {
                act();
            };

            runTimer.AutoReset = true;
            runTimer.Enabled = true;
            runTimer.Start();

            return runTimer;
        }
    }
}
