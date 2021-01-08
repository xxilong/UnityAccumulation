using System.IO;
using ShareLib.Log;
using ShareLib.Packer;
using Ventuz.Extention.DRZ;

namespace Ventuz.Extention.Conf
{
    class ExtractArchiveMachineConfig
    {
        public static void TryExtract()
        {
            string drzPath = Path.Combine(FilePaths.ventuzDir, "_mvshow.dzg");
            if(!File.Exists(drzPath))
            {
                return;
            }

            string extPath = Path.Combine(FilePaths.ventuzDir, "machine", "public", "Configuration");
            FileArchiver ar = new FileArchiver(new DrzStream(drzPath));
            int fileCount = ar.ExtractDiskFiles(extPath, true);
            ar.Close();

            Logger.Debug($"Import {fileCount} Archived Config Files.");
        }
    }
}
