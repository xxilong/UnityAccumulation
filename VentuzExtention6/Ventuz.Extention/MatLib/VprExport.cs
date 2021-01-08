using ShareLib.Conf;
using ShareLib.Log;
using ShareLib.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Extention.Compatible;
using Ventuz.Kernel.IO;

namespace Ventuz.Extention.MatLib
{
    class VprExport
    {
        public void AddProjectFiles(UriCollection col)
        {
            if (!ModConfig.getconf<bool>("VprExport", "addnoref", false))
                return;
            
            string projPath = VentuzWare.Instance.GetProjectPath();
            Logger.Debug("Export vpr with no reference resource add.");

            foreach(string restype in scanDirs)
            {
                AddDirFiles(projPath + restype, $"ventuz://{restype}/", col);
            }            
        }

        private void AddDirFiles(string dirPath, string urlPath, UriCollection col)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            foreach(var f in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
            {
                Uri fileURI = new Uri(urlPath + f.Name);
                col.Add(fileURI);
            }

            foreach(var d in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                AddDirFiles(dirPath + "\\" + d.Name, urlPath + d.Name + "/", col);
            }
        }

        private string[] scanDirs = { "Assets", "Audio", "AutoImport", "Data", "Design", "Documentation", "Fonts",
            "Geometries", "Images", "Misc", "Modules", "Movies", "Pages", "Sounds", "Textures", "XMS", "Scripts"  };
    }
}
