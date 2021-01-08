using System;
using System.Drawing;
using System.IO;
using Ventuz.Kernel.Engine;
using Ventuz.Kernel.Gfx;
using Ventuz.Kernel.Tools;
using Atalasoft.Imaging;
using Atalasoft.Imaging.ImageProcessing;
using Atalasoft.Imaging.Memory;

namespace Ventuz.Extention.Nodes.Helper
{
    public class TextureCreator
    {
        public TextureCreator()
        {
        }

        public AtalaImage LoadImageFile(string filePath)
        {
            using (Stream readStream5 = new FileStream(filePath, FileMode.Open))
            {
                try
                {
                    return new AtalaImage(readStream5, null);
                }
                catch (Exception)
                {
                    Bitmap bitmap = (Bitmap)Image.FromStream(readStream5);
                    if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format64bppArgb || bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format64bppPArgb || bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format48bppRgb)
                    {
                        bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    }
                    else if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format48bppRgb)
                    {
                        bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    }
                    AtalaImage image = AtalaImage.FromBitmap(bitmap, false);
                    bitmap.Dispose();
                    return image;
                }
            }
        }

        public VTexture CreateTextureFromImage(AtalaImage image)
        {
            if (image.PixelFormat != Atalasoft.Imaging.PixelFormat.Pixel32bppBgra)
            {
                ChangePixelFormatCommand changePixelFormatCommand = new ChangePixelFormatCommand(Atalasoft.Imaging.PixelFormat.Pixel32bppBgra);
                changePixelFormatCommand.Progress = null;
                image = AtalaImageProcessing.ApplyImageCommand(image, changePixelFormatCommand, false);
            }

            Size imageSize = image.Size;
            Size size = VDeviceCaps.MaxTextureSize();

            CreateTextureExPara createTextureExPara2 = default(CreateTextureExPara);
            createTextureExPara2.Flags = CreateTextureExFlags.None;
            createTextureExPara2.SizeX = (uint)imageSize.Width;
            createTextureExPara2.SizeY = (uint)imageSize.Height;
            createTextureExPara2.SourcePitch = (uint)(imageSize.Width * 4);
            createTextureExPara2.DestFormat = Kernel.Engine.PixelFormat.BGRA_UN8;
            createTextureExPara2.DestMipmaps = (uint)0;
            createTextureExPara2.HeightmapAmount = 0;

            GammaAndPremulHelper.SetFlags(ref createTextureExPara2, GammaAndPremul.GammaNotAColor, GammaEnum.sRGB, true);

            return new VTexture(EngineApi.CreateTextureEx(ref createTextureExPara2,
                PixelMemory.PixelDataFromPixelMemory(image),
                (long)imageSize.Width * (long)imageSize.Height * 4L));
        }

        public VTexture CreateTextureFromFile(string filePath)
        {
            using (AtalaImage atalaImage = LoadImageFile(filePath))
            {
                return CreateTextureFromImage(atalaImage);
            }
        }

        public VTexture CreateTexutreFromBitmap(Bitmap bitmap)
        {
            AtalaImage image = AtalaImage.FromBitmap(bitmap, false);
            return CreateTextureFromImage(image);
        }
    }
}
