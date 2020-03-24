using System;
using System.IO;

namespace ShareLib.Conf
{
    public class PathHelp
    {
        public static string appDir = AppDomain.CurrentDomain.BaseDirectory;
        public static string appIni = Path.ChangeExtension(System.Reflection.Assembly.GetEntryAssembly().Location, ".ini");

        public static string dllDir {
            get
            {
                return Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            }
        }

        public static string dllIni
        {
            get
            {
                return Path.ChangeExtension(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath, ".ini");
            }
        }
    }
}