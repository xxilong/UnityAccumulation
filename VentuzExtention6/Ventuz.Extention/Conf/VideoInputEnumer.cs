using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Config;
using Ventuz.Video;

namespace Ventuz.Extention.Conf
{
    public sealed class VideoInputEnumer
    {
        //static public IEnumerable<string> GetUsbVideoInputs()
        //{
        //    MachineHardware hardware = MachineHardware.GetLocalHardware(true);
        //    VideoPipe vpipe = new VideoPipe(hardware);
        //    int streamConfigCount = 10;
        //    for(int i = 0; i < streamConfigCount; ++i)
        //    {
        //        //VideoPipe.StreamInfo info = new VideoPipe.StreamInfo(vpipe, i);
        //        yield return info.Master.KeyCode;
        //    }
        //}


        //[DllImport("Ventuz.Pure.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int GetStreamConfigCount();
    }
}
