using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Ventuz.Extention.Conf;

namespace Ventuz.Extention.Marker
{
    public class TouchScreen
    {
        public static double Width = 1920;
        public static double Height = 1080;

        [Serializable]
        private struct ScreenSettingSerialize
        {
            public double w, h;
        }

        static TouchScreen()
        {
            BinaryFormatter binFormat = new BinaryFormatter();
            try
            {
                using (Stream fStream = new FileStream(FilePaths.ScreenConfigPath, FileMode.Open, FileAccess.Read))
                {
                    ScreenSettingSerialize sss = (ScreenSettingSerialize)binFormat.Deserialize(fStream);
                    Width = sss.w;
                    Height = sss.h;
                    Console.WriteLine("读入屏幕长宽配置: {0}X{1}", Width, Height);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("未能读入屏幕长宽配置, 使用默认值: {0}X{1}", Width, Height);
            }
        }

        static public void SaveSetting()
        {
            BinaryFormatter binFormat = new BinaryFormatter();

            using (Stream fStream = new FileStream(FilePaths.ScreenConfigPath, FileMode.Create, FileAccess.Write))
            {
                ScreenSettingSerialize sss = new ScreenSettingSerialize();
                sss.w = Width;
                sss.h = Height;
                binFormat.Serialize(fStream, sss);
            }

            Console.WriteLine("屏幕配置已保存");
        }
    }
}
