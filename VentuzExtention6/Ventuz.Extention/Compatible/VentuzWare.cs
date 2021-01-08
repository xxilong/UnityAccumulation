using ShareLib.Log;
using ShareLib.Packer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventuz.Designer;
using Ventuz.Designer.Shared;
using Ventuz.Extention.Control;
using Ventuz.Extention.VentuzHook;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.IO;
using Ventuz.Kernel.Sys;

namespace Ventuz.Extention.Compatible
{
    class VentuzWare
    {
        internal static VentuzWare Instance = null;
        static VentuzWare()
        {
            if(CheckMachineCode()) // Check Listence
            {
                Instance = new CatchedVentuzWare();
            }
            else
            {
                Instance = new VentuzWare();
            }
        }

        static bool CheckMachineCode()
        {
            return true;
        }

        public virtual string GetProjectPath()
        {
            if(projectPath != null)
            {
                return projectPath;
            }

            if(HookEntry.vzg._149 != null)
            {
                Logger.Debug($"ProjectPath: {GetProjectPathFromProjectManager()}");
                return Path.GetDirectoryName(GetProjectPathFromProjectManager()) + Path.DirectorySeparatorChar;
            }

            if(!string.IsNullOrEmpty(UserStartupOptions.StartFile))
            {
                if(!UserStartupOptions.VPRModeRuntime)
                {
                    return Path.GetDirectoryName(UserStartupOptions.StartFile) + Path.DirectorySeparatorChar;
                }
            }

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public void SetProjectPath(string path)
        {
            projectPath = path;
        }

        public virtual VScene GetProjectScene()
        {
            if(HookEntry.vzg._149 != null)
            {
                return ((ProjectManager)HookEntry.vzg._149).DocumentManager2.ActiveScene;
            }

            return VCommonRemoting2.IRemoteScene.GetScene(null, 0) as VScene;
        }

        private string GetProjectPathFromProjectManager()
        {
            return ((ProjectManager)HookEntry.vzg._149).CurrentProjectPath;
        }

        public virtual IDocumentManager GetDocumentMager()
        {
            return ((ProjectManager)HookEntry.vzg._149).DocumentManager;
        }

        public virtual UriManager GetUriManager()
        {
            ProjectFile projFile = HookEntry.runproj as ProjectFile;
            if(projFile == null)
            {
                projFile = HookEntry.vzg._194 as ProjectFile;
            }

            if(projFile == null)
            {
                ProjectManager projmgr = HookEntry.vzg._149 as ProjectManager;
                if (projmgr != null)
                {
                    projFile = projmgr.CurrentProjectFile;
                }
            }

            return projFile.UriManager;
        }

        public virtual IFileReader GetReader(string dir)
        {
            if (IsVprMode)
            {
                return new VentuzSchemeReader("ventuz://" + dir + "/");
            }
            else
            {
                return new DiskFileReader(VentuzWare.Instance.GetProjectPath() + dir);
            }
        }

        public bool IsVprMode { get; set; } = false;
        private string projectPath = null;
    }
}
