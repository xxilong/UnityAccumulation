using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShareLib.Unity
{
    public class FileSystem
    {
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination, bool copySubDirs)
        {
            if (!destination.Exists)
            {
                destination.Create(); //目标目录若不存在就创建
            }
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName, file.Name), true); //复制目录中所有文件
            }
            if (copySubDirs)
            {
                DirectoryInfo[] dirs = source.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    string destinationDir = Path.Combine(destination.FullName, dir.Name);
                    CopyDirectory(dir, new DirectoryInfo(destinationDir), copySubDirs); //复制子目录
                }
            }
        }

        public static void DeleteDir(string file)
        {

            try
            {
                DirectoryInfo fileInfo = new DirectoryInfo(file);
                fileInfo.Attributes = FileAttributes.Normal & FileAttributes.Directory;
                File.SetAttributes(file, System.IO.FileAttributes.Normal);

                if (Directory.Exists(file))
                {
                    foreach (string f in Directory.GetFileSystemEntries(file))
                    {
                        if (File.Exists(f))
                        {
                            File.Delete(f);
                        }
                        else
                        {
                            DeleteDir(f);
                        }

                    }
                    Directory.Delete(file);
                }

            }
            catch (Exception)
            {
            }
        }

        public static IEnumerable<string> ScanAllFiles(DirectoryInfo source)
        {
            List<string> allfiles = new List<string>();
            ScanFileToList(allfiles, source, "");
            return allfiles;
        }

        private static void ScanFileToList(List<string> list, DirectoryInfo source, string prefix)
        {
            foreach (FileInfo file in source.GetFiles())
            {
                list.Add(prefix + file.Name);
            }

            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                string destinationDir = Path.Combine(source.FullName, dir.Name);
                ScanFileToList(list, new DirectoryInfo(destinationDir), prefix + dir.Name + "\\");
            }
        }
    }
}
