using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ventuz.Extention.Conf
{
    public class FilePaths
    {
        public static string ventuzDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string baseDir = AppDomain.CurrentDomain.BaseDirectory + "machine\\Extention\\";
        public static string pakCfgDir = AppDomain.CurrentDomain.BaseDirectory + "machine\\public\\Configuration\\";
        public static string resDir = baseDir + "Resource\\";
        public static string ScreenConfigPath = baseDir + "screen.data";
        public static string TouchRecordePath = baseDir + "touch_event.txt";
        public static string MarkerConfigPath = baseDir + "markers.data";
        public static string TranslateFilePath = baseDir + "zh_cn.txt";
    }
}
