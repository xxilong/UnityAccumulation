using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRcodeUnity
{
    class Program
    {
        static void Main(string[] args)
        {
            //string strCode = "http://192.168.0.45/1.mp4";
            string strCode = Console.ReadLine();

            QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);

            // qrcode.GetGraphic 方法可参考最下发“补充说明”
            Bitmap qrCodeImage = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
            qrCodeImage.Save("D:\\1.jpg", ImageFormat.Jpeg);

        }
    }
}
