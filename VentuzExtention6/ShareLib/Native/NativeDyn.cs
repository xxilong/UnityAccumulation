using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Native
{
    public class NativeDynGet : DynamicObject
    {
        public NativeDynGet(IntPtr obj)
        {
            _obj = obj;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[8];
            }

            f = Marshal.GetDelegateForFunctionPointer<NativeWapper.AnyFunc2S>(f)(_obj, binder.Name);
            if(f == IntPtr.Zero)
            {
                result = null;
                return true;
            }

            GCHandle obj = GCHandle.FromIntPtr(f);
            result = obj.Target;
            return true;
        }

        /*
        public T As<T>(string name) where T : class
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[8];
            }

            if(f == IntPtr.Zero)
            {
                return null;
            }

            f = Marshal.GetDelegateForFunctionPointer<NativeWapper.AnyFunc2S>(f)(_obj, "_" + name);
            if (f == IntPtr.Zero)
            {
                return null;
            }

            GCHandle obj = GCHandle.FromIntPtr(f);
            return obj.Target as T;
        }

        public T As<T>(int name) where T : class
        {
            return As<T>(name.ToString());
        }
        */

        private IntPtr _obj;
    }

    public class NativeDynSet : DynamicObject
    {
        public NativeDynSet(IntPtr obj)
        {
            _obj = obj;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)_obj)[7];
            }
            
            Marshal.GetDelegateForFunctionPointer<NativeWapper.AnyFunc3SP>(f)(_obj, binder.Name, GCHandle.ToIntPtr(GCHandle.Alloc(value)));
            return true;
        }

        private IntPtr _obj;
    }
}
