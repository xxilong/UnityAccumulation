using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareLib.Unity;
using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Packer;
using Ventuz.Extention.Conf;
using Ventuz.Extention.DRZ;

namespace packventuzcfg
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RealMain(args);
        }

        private void RealMain(string[] args)
        {
            Logger.Debug("正在获取配置文件路径...");
            string cfgPath = @"C:\Users\Public\Documents\Ventuz6\Configuration";
            if(!Directory.Exists(cfgPath))
            {
                Logger.Error("未找到配置文件路径, 注意此程序使用未修改的原始安装路径.");
                Console.ReadLine();
                return;
            }

            string curPath = FilePaths.ventuzDir;
            string targetPath = Path.Combine(curPath, "Configuration");
            FileSystem.CopyDirectory(new DirectoryInfo(cfgPath), new DirectoryInfo(targetPath), true);
            Console.Write($"配置文件已拷贝到 {targetPath}, 现在可以手动编辑文件修改, 完成后按回车继续 ");
            Console.ReadLine();

            FileArchiver ar = new FileArchiver(new DrzStream(Path.Combine(curPath, "_mvshow.dzg"), true));

            var files = FileSystem.ScanAllFiles(new DirectoryInfo(targetPath));
            foreach(var file in files)
            {
                Logger.Debug($"Pack file {file}...");
                ar.PushDiskFile(file, Path.Combine(targetPath, file));
            }

            ar.Close();

            Console.WriteLine("Done.");
            Console.ReadLine();
            FileSystem.DeleteDir(targetPath);
        }
    }
}
