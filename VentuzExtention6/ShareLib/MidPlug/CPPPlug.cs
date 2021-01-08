using System;
using ShareLib.Log;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MidCtrl
{
    public abstract class CPPPlug<T> where T : CPPPlug<T>
    {
        public static bool InitObjectInstance(string addr)
        {
            Logger.Set(new LockedLoggerGroup {
                new ConsoleLogger(),
                new UDPBroadCastLog(),
            });


            CreateInstance();

            if (Instance == null)
            {
                Logger.Error("No Type implement IMidPlug at this assembly.");
                return false;
            }

            if (!Instance.SetupFunctions((IntPtr)Convert.ToInt64(addr, 16)))
            {
                Logger.Error("function table init failed.");
                return false;
            }

            return true;
        }

        static private void CreateInstance()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Type[] types = asm.GetTypes();
            foreach (var t in types)
            {
                if (t.BaseType == typeof(T))
                {
                    Instance = asm.CreateInstance(t.FullName) as T;
                    if (Instance != null)
                    {
                        Logger.Info($"Create Plug From Type: {t.FullName}");
                        return;
                    }
                }
            }
        }

        protected abstract bool init_functions();

        private bool SetupFunctions(IntPtr plug)
        {
            if (plug == IntPtr.Zero)
            {
                return false;
            }

            cobject = plug;
            return init_functions();
        }

        protected FT GetCppVirtualFunc<FT>(int n)
        {
            IntPtr f = IntPtr.Zero;
            unsafe
            {
                f = (*(IntPtr**)cobject)[n];
            }
            return Marshal.GetDelegateForFunctionPointer<FT>(f);
        }

        protected IntPtr ToCPPFunc<FT>(FT f)
        {
            return Marshal.GetFunctionPointerForDelegate(f);
        }

        protected static T Instance = null;
        protected IntPtr cobject = IntPtr.Zero;
    }
}
