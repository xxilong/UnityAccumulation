using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GACEnum
{
    public struct GacAssembly
    {
        // Token: 0x060062E3 RID: 25315 RVA: 0x00040376 File Offset: 0x0003E576
        public GacAssembly(string name, string fullName, Version version)
        {
            this.Name = name;
            this.FullName = fullName;
            this.Version = version;
        }

        // Token: 0x060062E4 RID: 25316 RVA: 0x0004038D File Offset: 0x0003E58D
        public override string ToString()
        {
            return string.Format("{0}, {1} [{2}]", this.Name, this.Version, this.FullName);
        }

        // Token: 0x0400398B RID: 14731
        public string Name;

        // Token: 0x0400398C RID: 14732
        public string FullName;

        // Token: 0x0400398D RID: 14733
        public Version Version;
    }

    class GAC
    {
        static void Main(string[] args)
        {
            GacAssembly[] ass = GetAssemblies();
            foreach(var s in ass)
            {
                Console.WriteLine(s.FullName);
            }
        }

        [Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        private interface IAssemblyName
        {
            // Token: 0x06004D1C RID: 19740
            [PreserveSig]
            int SetProperty(int PropertyId, IntPtr pvProperty, uint cbProperty);

            // Token: 0x06004D1D RID: 19741
            [PreserveSig]
            int GetProperty(int PropertyId, IntPtr pvProperty, ref uint pcbProperty);

            // Token: 0x06004D1E RID: 19742
            [PreserveSig]
            int Finalize();

            // Token: 0x06004D1F RID: 19743
            [PreserveSig]
            int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder szDisplayName, ref uint pccDisplayName, int dwDisplayFlags);

            // Token: 0x06004D20 RID: 19744
            [PreserveSig]
            int BindToObject(ref Guid refIID, [MarshalAs(UnmanagedType.IUnknown)] object pUnkSink, [MarshalAs(UnmanagedType.IUnknown)] object pUnkContext, [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, long llFlags, IntPtr pvReserved, uint cbReserved, out IntPtr ppv);

            // Token: 0x06004D21 RID: 19745
            [PreserveSig]
            int GetName(ref uint lpcwBuffer, [MarshalAs(UnmanagedType.LPWStr)] [Out] StringBuilder pwzName);

            // Token: 0x06004D22 RID: 19746
            [PreserveSig]
            int GetVersion(out uint pdwVersionHi, out uint pdwVersionLow);

            // Token: 0x06004D23 RID: 19747
            [PreserveSig]
            int IsEqual(GAC.IAssemblyName pName, int dwCmpFlags);

            // Token: 0x06004D24 RID: 19748
            [PreserveSig]
            int Clone(out GAC.IAssemblyName pName);
        }

        [Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [ComImport]
        private interface IAssemblyEnum
        {
            // Token: 0x06004D25 RID: 19749
            [PreserveSig]
            int GetNextAssembly(IntPtr pvReserved, out IAssemblyName ppName, uint dwFlags);

            // Token: 0x06004D26 RID: 19750
            [PreserveSig]
            int Reset();

            // Token: 0x06004D27 RID: 19751
            [PreserveSig]
            int Clone(out GAC.IAssemblyEnum ppEnum);
        }

        [DllImport("fusion.dll", PreserveSig = false, SetLastError = true)]
        private static extern void CreateAssemblyEnum(out IAssemblyEnum pEnum, IntPtr pUnkReserved, GAC.IAssemblyName pName, int dwFlags, IntPtr pvReserved);

        public static GacAssembly[] GetAssemblies()
        {
            Type typeFromHandle = typeof(GAC);
            GacAssembly[] result;
            lock (typeFromHandle)
            {
                if (GAC.gacAssemblies == null)
                {
                    List<GacAssembly> list = new List<GacAssembly>();
                    bool processIs = false;
                    GAC.IAssemblyEnum assemblyEnum;
                    GAC.CreateAssemblyEnum(out assemblyEnum, IntPtr.Zero, null, 2, IntPtr.Zero);
                    IntPtr intPtr = Marshal.AllocCoTaskMem(512);
                    try
                    {
                        GAC.IAssemblyName assemblyName;
                        while (assemblyEnum.GetNextAssembly((IntPtr)0, out assemblyName, 0u) == 0)
                        {
                            StringBuilder stringBuilder = new StringBuilder(512);
                            uint num = (uint)stringBuilder.Capacity;
                            if (assemblyName.GetDisplayName(stringBuilder, ref num, 167) == 0)
                            {
                                string fullName = stringBuilder.ToString();
                                num = (uint)stringBuilder.Capacity;
                                if (assemblyName.GetName(ref num, stringBuilder) == 0)
                                {
                                    string name = stringBuilder.ToString();
                                    ushort[] array = new ushort[4];
                                    for (int i = 0; i < 4; i++)
                                    {
                                        num = 8u;
                                        if (assemblyName.GetProperty(4 + i, intPtr, ref num) == 0 && num == 2u)
                                        {
                                            array[i] = (ushort)(Marshal.ReadInt32(intPtr) & 65535);
                                        }
                                    }
                                    Version version = new Version((int)array[0], (int)array[1], (int)array[2], (int)array[3]);
                                    PortableExecutableKinds portableExecutableKinds = PortableExecutableKinds.NotAPortableExecutableImage;
                                    bool flag2 = false;
                                    num = 512u;
                                    if (assemblyName.GetProperty(27, intPtr, ref num) == 0 && num == 4u)
                                    {
                                        portableExecutableKinds = (PortableExecutableKinds)Marshal.ReadInt32(intPtr);
                                    }
                                    switch (portableExecutableKinds)
                                    {
                                        case PortableExecutableKinds.ILOnly:
                                            flag2 = true;
                                            break;
                                        case PortableExecutableKinds.Required32Bit:
                                            if (processIs)
                                            {
                                                flag2 = true;
                                            }
                                            break;
                                        case PortableExecutableKinds.PE32Plus:
                                            if (!processIs)
                                            {
                                                flag2 = true;
                                            }
                                            break;
                                    }
                                    if (flag2)
                                    {
                                        list.Add(new GacAssembly(name, fullName, version));
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(intPtr);
                    }
                    list.Sort(new Comparison<GacAssembly>(GAC.Compare));
                    GAC.gacAssemblies = list.ToArray();
                }
                result = GAC.gacAssemblies;
            }
            return result;
        }

        private static int Compare(GacAssembly a, GacAssembly b)
        {
            return string.Compare(a.Name, b.Name, true);
        }

        // Token: 0x06004D1A RID: 19738 RVA: 0x000332BE File Offset: 0x000314BE
        private static int CompareVersion(GacAssembly a, GacAssembly b)
        {
            return string.Compare(a.Version.ToString(), b.Version.ToString(), true);
        }

        private static GacAssembly[] gacAssemblies;
    }
}
