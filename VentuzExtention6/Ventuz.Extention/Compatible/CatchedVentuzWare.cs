using ShareLib.Conf;
using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Designer.Shared;
using Ventuz.Kernel.Sys;

namespace Ventuz.Extention.Compatible
{
    class CatchedVentuzWare : VentuzWare
    {
        public override string GetProjectPath()
        {
            try
            {
                return base.GetProjectPath();
            }
            catch(Exception e)
            {
                Logger.Warning($"Catched Exception: {e.Message}");
                return PathHelp.dllDir + Path.DirectorySeparatorChar;
            }
        }

        public override VScene GetProjectScene()
        {
            try
            {
                return base.GetProjectScene();
            }
            catch(Exception e)
            {
                Logger.Warning($"Catched Exception: {e.Message}");
                return null;
            }
        }

        public override IDocumentManager GetDocumentMager()
        {
            try
            {
                return base.GetDocumentMager();
            }
            catch(Exception e)
            {
                Logger.Warning($"Catched Exception: {e.Message}");
                return null;
            }             
        }
    }
}
