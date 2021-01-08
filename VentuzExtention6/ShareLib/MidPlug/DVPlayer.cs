using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MidCtrl
{
    public class DVPlayer : CPPPlug<DVPlayer>
    {
        public virtual void OnInit() {}

        // call from cpp
        static public int CPPInitalize(string objaddr)
        {
            if (!InitObjectInstance(objaddr))
            {
                return -1;
            }

            Instance.OnInit();
            return 1;
        }

        // functions
        public void GotoPage(string name) => GotoPagePtr?.Invoke(cobject, name);

        // inner
        protected override bool init_functions()
        {
            SetFuncPtr = GetCppVirtualFunc<SetFuncPtr_t>(1);
            GotoPagePtr = GetCppVirtualFunc<GotoPagePtr_t>(2);

            return true;
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SetFuncPtr_t(IntPtr obj, string name, IntPtr f);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void GotoPagePtr_t(IntPtr obj, string cmd);

        SetFuncPtr_t SetFuncPtr = null;
        GotoPagePtr_t GotoPagePtr = null;
    }
}
