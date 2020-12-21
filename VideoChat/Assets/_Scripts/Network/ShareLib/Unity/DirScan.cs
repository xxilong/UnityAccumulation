using System.Collections.Generic;
using System.IO;
using ShareLib.Log;

namespace ShareLib.Unity
{
    class DirScan
    {
        public static Dictionary<string, string> ScanFiles(string dirPath, string fileFilter)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                foreach (var f in new DirectoryInfo(dirPath).GetFiles(fileFilter, SearchOption.TopDirectoryOnly))
                {
                    result[Path.GetFileNameWithoutExtension(f.Name)] = f.FullName;
                }
            }
            catch(System.Exception e)
            {
                Logger.Warning(string.Format("Exception on scan dir {0}: {1}", dirPath, e.Message));
            }

            return result;
        }
    }
}
