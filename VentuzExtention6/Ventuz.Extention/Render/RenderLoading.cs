using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.Engine;
using Ventuz.Kernel.Gfx;
using Ventuz.Extention.Nodes.Helper;
using Ventuz.Extention.Conf;
using System.IO;

namespace Ventuz.Extention.Render
{
    class RenderLoading
    {
        public static RenderLoading Instance = new RenderLoading();

        private RenderLoading()
        {
            //logoTexture = creator.CreateTextureFromFile(Path.Combine(FilePaths.resDir, "qxvlogo.png"));

            progressBitmap = new Bitmap(600, 200, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            progressGraphics = Graphics.FromImage(progressBitmap);
            progRect = Rect.FromSize(0, 0, 600, 200);

            proFormat.Alignment = StringAlignment.Center;
        }

        public void DoRender(object tex, float progress)
        {
            VTexture vtexture = tex as VTexture;
            if (vtexture == null || vtexture.Disposed)
            {
                return;
            }            
            
            int sizeX = vtexture.SizeX;
            int sizeY = vtexture.SizeY;

            Rect srcRect = Rect.FromSize(0, 0, 100, 100); // logoTexture.SizeX, logoTexture.SizeY);
            Rect fullRect = Rect.FromSize(0, 0, sizeX, sizeY);
            int freeYSize = sizeY - 201;
            Rect dstRect = Rect.FromSize((sizeX - 584) / 2, freeYSize / 4, 584, 201);
            int logoBottom = freeYSize / 3 + 201;
                
            EngineApi.BeginColorFill(vtexture.Handle);
            EngineApi.DoColorFill(fullRect, (uint)bgColor.ToArgb());
                
            int yBegin = sizeY - (logoBottom + freeYSize / 5);

            EngineApi.DoColorFill(Rect.FromSize(5, yBegin, sizeX - 10, 20), (uint)borderColor.ToArgb());
            int xSize = (int)((float)(sizeX - 14) * progress);

            if (xSize > 0)
            {
                EngineApi.DoColorFill(Rect.FromSize(7, yBegin + 2, xSize, 16), (uint)fillColor.ToArgb());
            }   
                
            EngineApi.EndColorFill();
            //EngineApi.StretchRect(logoTexture.Handle, srcRect, vtexture.Handle, dstRect, 0);

            dstRect = Rect.FromSize((sizeX - 600) / 2, (logoBottom + freeYSize / 5) + 60, 600, 200);

            progressGraphics.Clear(bgColor);
            progressGraphics.DrawString($"正在加载 {progress * 100}%", font, brush, proRectangle, proFormat);
            //progressGraphics.DrawString("精彩内容, 即将为你呈现!", tlfont, tlbrush, tlRectangle, proFormat);

            if(progressTexture != null)
            {
                progressTexture.Dispose();
            }
            progressTexture = creator.CreateTexutreFromBitmap(progressBitmap);
            EngineApi.StretchRect(progressTexture.Handle, progRect, vtexture.Handle, dstRect, 0);
        }

        private TextureCreator creator = new TextureCreator();
        private Color fillColor = Color.Yellow;
        private Color borderColor = Color.Blue;
        private Color bgColor = Color.LightBlue;
        //private VTexture logoTexture;
        private Bitmap progressBitmap;
        private Graphics progressGraphics;
        private VTexture progressTexture = null;
        private Rect progRect;
        private Font font = new Font("宋体", 20);
        private Brush brush = new SolidBrush(Color.Blue);
        private Rectangle proRectangle = new Rectangle(0, 0, 600, 80);
        private StringFormat proFormat = new StringFormat();

        private Font tlfont = new Font("宋体", 35, FontStyle.Bold);
        private Brush tlbrush = new SolidBrush(Color.Red);
        private Rectangle tlRectangle = new Rectangle(0, 100, 600, 120);
    }
}
