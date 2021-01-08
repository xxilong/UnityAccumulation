using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using ShareLib.Conf;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using ShareLib.Log;

namespace PhotoPrinter
{
    class PrintImgURL
    {
        public async Task PrintImage(string url, Action<string> PrtResult)
        {
            Stream imageStream = await client.OpenReadTaskAsync(url);
            PrintDocument pd = new PrintDocument();
            Image image = Image.FromStream(imageStream);
            
            pd.PrinterSettings.PrinterName = _PrinterName;
            pd.PrintController = _PrintController;
            bool rpterror = false;

            pd.PrintPage += (object sender, PrintPageEventArgs e) =>
            {
                try
                {
                    Rectangle printRect = e.PageBounds;
                    Rectangle imageRect = new Rectangle(0, 0, image.Width, image.Height);

                    Logger.Info($"打印纸像素大小为: {printRect.Width}X{printRect.Height}");
                    int prtwidth = printRect.Width;
                    int prtheight = printRect.Height;

                    if ((printRect.Height > printRect.Width) != (imageRect.Height > imageRect.Width))
                    {
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        imageRect = new Rectangle(0, 0, image.Width, image.Height);
                    }

                    if (printRect.Height * imageRect.Width > printRect.Width * imageRect.Height)
                    {
                        int height = printRect.Width * imageRect.Height / imageRect.Width;
                        int border = (printRect.Height - height) / 2;
                        printRect = new Rectangle(0, border, printRect.Width, printRect.Height - 2 * border);
                    }
                    else
                    {
                        int width = printRect.Height * imageRect.Width / imageRect.Height;
                        int border = (printRect.Width - width);
                        printRect = new Rectangle(border, 0, printRect.Width - border, printRect.Height);
                    }

                    e.Graphics.DrawImage(image, printRect, imageRect, GraphicsUnit.Pixel);
                    e.Graphics.TranslateTransform(prtwidth / 2, prtheight / 2);
                    e.Graphics.RotateTransform(90);
                    e.Graphics.DrawString(_PrintText, _TextFont, _TextBrush, _TextX, _TextY);
                    e.Graphics.DrawString(DateTime.Now.ToString("yyyy-MM-dd"), _TextFont, _TextBrush, _DateX, _DateY);

                    Rectangle logo1Rect = new Rectangle(-280, 140, 50, 50);
                    Rectangle logo2Rect = new Rectangle(-220, 140, 50, 50);
                    //e.Graphics.DrawImage(_logo1, logo1Rect);
                    e.Graphics.DrawImage(_logo2, logo1Rect);
                }
                catch(Exception ex)
                {
                    rpterror = true;
                    PrtResult($"打印异常: {ex}");
                }
            };

            pd.EndPrint += (object sender, PrintEventArgs ev) =>
            {
                if(!rpterror)
                {
                    rpterror = true;
                    PrtResult("ok");
                }
            };

            pd.Print();
        }

        private WebClient client = new WebClient();
        private PrintController _PrintController = new StandardPrintController();
        private SolidBrush _TextBrush = new SolidBrush(System.Drawing.Color.Black);
        private Font _TextFont = new Font("黑体", 10);
        private int _TextX = GlobalConf.getconf<int>("printer", "text_x");
        private int _TextY = GlobalConf.getconf<int>("printer", "text_y");
        private int _DateX = GlobalConf.getconf<int>("printer", "date_x");
        private int _DateY = GlobalConf.getconf<int>("printer", "date_y");
        private string _PrinterName = GlobalConf.getconf("printer", "name");
        private string _PrintText = GlobalConf.getconf("printer", "text");

        private Image _logo1 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo1.png"));
        private Image _logo2 = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo2.png"));
    }
}
