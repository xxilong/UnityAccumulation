using System;
using System.Runtime.InteropServices;

namespace Native
{
    public class NativeWapper
    {
        static NativeWapper()
        {
            DllRegisterServer();
        }

        static public void DoNothing()
        {

        }

        public NativeWapper(byte[] project)
        {
            IntPtr obj = DllGetClassObject(project, Marshal.GetFunctionPointerForDelegate(new AnyFunc1(LogMessage)));
            if (obj != IntPtr.Zero)
            {
                _obj = obj;
            }

            _sxbinder = new NativeDynSet(_obj);
            _gxbinder = new NativeDynGet(_obj);
        }

        public void Post(int ms, Action act)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[2];
            }

            Marshal.GetDelegateForFunctionPointer<AnyFunc3>(f)(_obj, ms, Marshal.GetFunctionPointerForDelegate(new AnyAction(act)));
        }

        public void Commit(Action act)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[3];
            }

            Marshal.GetDelegateForFunctionPointer<AnyFunc2>(f)(_obj, Marshal.GetFunctionPointerForDelegate(new AnyAction(act)));
        }

        public void Solo(Action act)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[4];
            }

            Marshal.GetDelegateForFunctionPointer<AnyFunc2>(f)(_obj, Marshal.GetFunctionPointerForDelegate(new AnyAction(act)));
        }

        public void Marsh<T>(T obj)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[5];
            }

            Marshal.GetDelegateForFunctionPointer<AnyFunc3SP>(f)(_obj, obj.GetType().ToString(), GCHandle.ToIntPtr(GCHandle.Alloc(obj)));
        }

        public T Fetch<T>()
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[6];
            }

            GCHandle obj = GCHandle.FromIntPtr(Marshal.GetDelegateForFunctionPointer<AnyFunc2S>(f)(_obj, typeof(T).ToString()));
            return (T)obj.Target;
        }

        public IntPtr LogMessage(IntPtr log)
        {
            if (_obj != IntPtr.Zero)
            {
                return IntPtr.Zero;
            }
            
            _obj = log;
            return IntPtr.Zero;
        }

        public dynamic Sx { get => _sxbinder; }
        public dynamic Gx { get => _gxbinder; }

        private dynamic _sxbinder = null;
        private dynamic _gxbinder = null;
        private IntPtr _obj = IntPtr.Zero;

        [DllImport("Ventuz.Extention.Navtive.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr DllGetClassObject(byte[] data, IntPtr count);

        [DllImport("Ventuz.Extention.Navtive.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern void DllRegisterServer();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate void AnyAction();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc0();

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc1(IntPtr a1);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc2(IntPtr a1, IntPtr a2);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc2S(IntPtr a1, string s);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc3(IntPtr a1, int a2, IntPtr a3);

        [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public delegate IntPtr AnyFunc3SP(IntPtr a1, string s, IntPtr a3);
    }
}
