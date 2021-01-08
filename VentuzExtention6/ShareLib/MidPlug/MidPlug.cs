using ShareLib.Log;
using ShareLib.Conf;
using System.IO;
using System;
using System.Runtime.InteropServices;
using ShareLib.Unity;

namespace MidCtrl
{
    public class MidPlug : CPPPlug<MidPlug>
    {
        public virtual string ConfigFile => "config.ini";

        public virtual void OnInit() { }
        public virtual void OnStart() { }
        public virtual void OnStop() { }

        public virtual void OnRecvCommand(string cmd) { }

        protected void SendCommandToControl(string cmd) => Service_Send?.Invoke(Service_Object, cmd);
        protected void SendCommandToService(string cmd)
        {
            Delay.Run(10, () => Control_Send?.Invoke(Control_Object, cmd));
        }

        static public int CPPInitalize(string objaddr)
        {
            GlobalConf.SetGlobalFileGetter(() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Instance.ConfigFile));
            if(!InitObjectInstance(objaddr))
            {
                return -1;
            }
            Instance.OnInit();
            return 1;
        }

        protected override bool init_functions() 
        {
            GetFuncPtr = GetCppVirtualFunc<GetFuncPtr_t>(4);
            SetFuncPtr = GetCppVirtualFunc<SetFuncPtr_t>(3);

            Service_Object = GetFuncPtr(cobject, "service_send_object");
            Service_Function = GetFuncPtr(cobject, "service_send_func");
            Control_Object = GetFuncPtr(cobject, "control_send_object");
            Control_Function = GetFuncPtr(cobject, "control_send_func");

            Control_Send = Marshal.GetDelegateForFunctionPointer<SendFunc_t>(Control_Function);
            Service_Send = Marshal.GetDelegateForFunctionPointer<SendFunc_t>(Service_Function);

            SetFuncPtr(cobject, "recv_func", Marshal.GetFunctionPointerForDelegate(new SendCommand_t(OnRecvCommand)));
            return true;
        }

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SetFuncPtr_t(IntPtr obj, string name, IntPtr f);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate IntPtr GetFuncPtr_t(IntPtr obj, string name);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SendCommand_t(string cmd);
        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void SendFunc_t(IntPtr obj, string s);

        GetFuncPtr_t GetFuncPtr = null;
        SetFuncPtr_t SetFuncPtr = null;
        private IntPtr Service_Object, Service_Function, Control_Object, Control_Function;
        private SendFunc_t Service_Send = null;
        private SendFunc_t Control_Send = null;
    }
}
