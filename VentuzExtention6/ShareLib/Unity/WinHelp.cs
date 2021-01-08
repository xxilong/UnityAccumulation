using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ShareLib.Unity
{
    public class WinHelp
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int IsWindow(IntPtr hWnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int IsWindowVisible(IntPtr hWnd);

        [DllImport("user32", CharSet=CharSet.Auto)]
        static public extern bool ShowWindow(IntPtr hWnd, short State);
    }
}
