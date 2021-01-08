using System;
using ShareLib.Log;
using System.Runtime.InteropServices;

namespace MidCtrl
{
    public class WOPlug : CPPPlug<WOPlug>
    {
        // 需要实现的接口
        public virtual bool init() { return true; }
        public virtual bool show() { return true; }
        public virtual bool hide() { return true; }
        public virtual void destory() { }
        public virtual void update()
        {
            destory();
            init();
        }
        public virtual void OnRecvCommand(string cmd) { }


        // 可供使用的功能
        public void SendCommand(string cmd) => SendCommandFunc?.Invoke(cobject, cmd);
        public string GetArgument(int n) => GetArgumentFunc?.Invoke(cobject, n);


        static public int CPPInitalize(string objaddr)
        {
            try
            {
                if (!InitObjectInstance(objaddr))
                {
                    return -1;
                }
            }
            catch(Exception e)
            {
                Logger.Error($"初始化异常: {e.Message}");
                return -1;
            }
            return 1;
        }

        // 初始化
        protected override bool init_functions()
        {
            SetFuncPtr = GetCppVirtualFunc<SetFuncPtr_t>(1);
            SendCommandFunc = GetCppVirtualFunc<SendCommand_t>(2);
            GetArgumentFunc = GetCppVirtualFunc<GetArgument_t>(3);

            SetFuncPtr(cobject, "init", ToCPPFunc(new sbv(init)));
            SetFuncPtr(cobject, "show", ToCPPFunc(new sbv(show)));
            SetFuncPtr(cobject, "hide", ToCPPFunc(new sbv(hide)));
            SetFuncPtr(cobject, "destory", ToCPPFunc(new svv(destory)));
            SetFuncPtr(cobject, "update", ToCPPFunc(new svv(update)));
            SetFuncPtr(cobject, "recvcmd", ToCPPFunc(new svs(OnRecvCommand)));
            return true;
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SetFuncPtr_t(IntPtr obj, string name, IntPtr f);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SendCommand_t(IntPtr obj, string cmd);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate string GetArgument_t(IntPtr obj, int n);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate bool sbv();
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void svv();
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void svs(string s);

        SetFuncPtr_t SetFuncPtr = null;
        SendCommand_t SendCommandFunc = null;
        GetArgument_t GetArgumentFunc = null;
    }
}
