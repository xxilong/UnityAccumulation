using System;
using System.Runtime.InteropServices;
using Ventuz.Kernel.Engine;
using Ventuz.Kernel.Gfx;

namespace Ventuz.Extention.Nodes.Helper
{
    class TextureFromNative
    {
        public VTexture CreateTextureFromNative(string id)
        {
            IntPtr data = GetNativeTexture(id, out int width, out int height);
            if(data == null || data == IntPtr.Zero)
            {
                return null;
            }
            
            CreateTextureExPara createTextureExPara2 = default(CreateTextureExPara);
            createTextureExPara2.Flags = CreateTextureExFlags.None;
            createTextureExPara2.SizeX = (uint)width;
            createTextureExPara2.SizeY = (uint)height;
            createTextureExPara2.SourcePitch = (uint)(width * 4);
            createTextureExPara2.DestFormat = PixelFormat.BGRA_UN8;
            createTextureExPara2.DestMipmaps = 0;
            createTextureExPara2.HeightmapAmount = 0;

            GammaAndPremulHelper.SetFlags(ref createTextureExPara2, GammaAndPremul.GammaNotAColor, GammaEnum.sRGB, true);

            TextureHandle handle = EngineApi.CreateTextureEx(ref createTextureExPara2, data, width * height * 4L);
            if(handle.IsNull)
            {
                return null;
            }

            return new VTexture(handle);
        }

        [DllImport("Ventuz.Extention.IPCameraProc.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern IntPtr GetNativeTexture(string videoId, out int width, out int height);
    }
}
