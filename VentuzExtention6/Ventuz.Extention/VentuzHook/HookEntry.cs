using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using Ventuz.Designer.Shared;
using Ventuz.Extention.Conf;
using Ventuz.Extention.Marker;
using Ventuz.Extention.MatLib;
using Ventuz.Extention.Translate;
using Ventuz.Extention.UI;
using Ventuz.Extention.Render;
using Ventuz.Kernel.Input;
using Ventuz.Kernel.IO;
using Ventuz.Kernel.Sys;
using Ventuz.Extention.DRZ;
using System.IO;
using ShareLib.Conf;
using Ventuz.Extention.Compatible;
using Ventuz.Extention.Resource;
using Native;
using System.Diagnostics;
using Ventuz.Kernel.Gfx;
using Ventuz.Kernel;

namespace Ventuz.Extention.VentuzHook
{
    /// <summary>
    /// 将 Ventuz 中插入的调用统一放到这里管理
    /// </summary>
    public class HookEntry
    {
        #region 公共启动
        public static void ProgramBegin()
        {
        }

        #endregion

        #region 启动数据

        // --------------------------- Designer -----------------------------------------------------------
        // Ventuz.exe Launcher.MainProgram.Main 
        public static void DesignerBegin()
        {
            ProgramBegin();

            VentuzWare.Instance.IsVprMode = false;
            byte[] data = ExtentionResource.GetBinaryContent("Ventuz6_Designer");
            _nv = new NativeWapper(data);
            vzg = _nv.Gx;
        }

        // 在 Ventuz.Designer.Mainform Launch 函数中调用
        public static void SetMainForm(Form form)
        {
            _nv.Sx._31 = form;            
            ComponentManager.Instance.Init();
            GlobalConf.SetGlobalFileGetter(() => Path.Combine(VentuzWare.Instance.GetProjectPath(), "Scripts", "config.ini"));
        }

        // 在 Ventuz.Designer.Mainform Launch 函数中调用
        public static void SetEditorWindow(UserControl layer, object hierarchy,
            object content, object project)
        {
            _nv.Sx._49 = layer;         // LayerEditor
            _nv.Sx._71 = hierarchy;     // HierarchyEditor
            _nv.Sx._97 = content;       // ContentEditor
            _nv.Sx._127 = project;      // ProjectManager
        }

        // ---------------------- Presenter -----------------------------------------------------

        public static void OriginPresenterBegin()
        {
            string projectfile = "";
            string [] args = Environment.GetCommandLineArgs();
            bool isvzp = false;

            Console.WriteLine($"=== Begin!");

            foreach(string arg in args)
            {                
                if(Path.GetExtension(arg) == ".vzp")
                {
                    projectfile = arg;
                    isvzp = true;
                }
            }

            if(isvzp)
            {
                VentuzWare.Instance.IsVprMode = false;
                byte[] data = ExtentionResource.GetBinaryContent("Ventuz6_Designer");
                _nv = new NativeWapper(data);
                vzg = _nv.Gx;

                VentuzWare.Instance.SetProjectPath(Path.GetDirectoryName(projectfile) + "\\");
                GlobalConf.SetGlobalFileGetter(() => Path.Combine(VentuzWare.Instance.GetProjectPath(), "Scripts", "config.ini"));
            }
            else
            {
                MessageBox.Show("只支持播放工程文件, 打包项目请使用 DevotedRender.");
                Process.GetCurrentProcess().Kill();
            }

            ProgramBegin();
        }

        // ---------------------- Script -----------------------
        public static void InitFromScript()
        {
            Console.WriteLine($"=== Init Ventuz.Extention From Script ===!");

            string projectfile = "";
            string[] args = Environment.GetCommandLineArgs();

            bool isvzp = false;
            bool isvpr = false;            

            foreach (string arg in args)
            {
                if (Path.GetExtension(arg) == ".vzp")
                {
                    projectfile = arg;
                    isvzp = true;
                }

                if(Path.GetExtension(arg) == ".vpr")
                {
                    projectfile = arg;
                    isvpr = true;
                }
            }

            byte[] data = ExtentionResource.GetBinaryContent("Ventuz6_Designer");
            _nv = new NativeWapper(data);
            vzg = _nv.Gx;

            if (isvzp)
            {
                VentuzWare.Instance.IsVprMode = false;
                VentuzWare.Instance.SetProjectPath(Path.GetDirectoryName(projectfile) + "\\");
                GlobalConf.SetGlobalFileGetter(() => Path.Combine(VentuzWare.Instance.GetProjectPath(), "Scripts", "config.ini"));
            }
            else if(isvpr)
            {
                VentuzWare.Instance.IsVprMode = true;
            }
            else
            {
                VentuzWare.Instance.IsVprMode = false;

                Assembly designer = null;
                foreach(Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if(ass.GetName().Name == "Ventuz.Designer")
                    {
                        designer = ass;
                        break;
                    }
                }

                if(designer == null)
                {
                    VLog.Error("Extention", "Ventuz.Extention 初始化失败: 未找到 Designer 模块, 当前不是在 Desinger 中启动的?", VPopup.Always);
                    return;
                }

                Type g = designer.GetType("Ventuz.Designer.Global");
                FieldInfo mgr = g.GetField("ProjectManager", BindingFlags.Static | BindingFlags.NonPublic);
                object projmgr = mgr.GetValue(null);

                Type m = designer.GetType("Ventuz.Designer.ProjectManager");
                PropertyInfo projfile = m.GetProperty("CurrentProjectPath", BindingFlags.Instance | BindingFlags.NonPublic);
                string projPath = projfile.GetValue(projmgr) as string;

                VentuzWare.Instance.SetProjectPath(Path.GetDirectoryName(projPath) + "\\");
                GlobalConf.SetGlobalFileGetter(() => Path.Combine(VentuzWare.Instance.GetProjectPath(), "Scripts", "config.ini"));
            }
        }


        // ---------------------- DevtoedPlayer ------------------------------------------------- 

        // Ventuz.Presenter.StartUp.Main
        public static void PresenterBegin()
        {
            NativeWapper.DoNothing();
            VentuzWare.Instance.IsVprMode = true;
            Logger.Debug($"Begin Start Devoted Presenter Mode");

            string vprini = Path.Combine(FilePaths.ventuzDir, "_mvshow.ini");

            if (File.Exists(vprini))
            {
                GlobalConf.SetGlobalFileGetter(() => vprini);
            }
            else
            {
                GlobalConf.SetGlobalStreamGetter(() => VentuzWare.Instance.GetUriManager().GetReadStream(new Uri("ventuz://scripts/config.ini")));
            }

            ExtractArchiveMachineConfig.TryExtract();
            OverideSystemConf.instance.TryReplaceConf();

            ProgramBegin();
        }

        // 在 DevotedRender.exe  Ventuz.Presenter.Runner.ctor()  中调用
        public static void SetRunnerWindow(object _projectfile)
        {
            Logger.Debug($"-> SetRunnerWindow: {_projectfile}");
            Stream stx = null;
            try
            {
                runproj = _projectfile;
                stx = VentuzWare.Instance.GetUriManager().GetReadStream(new Uri("ventuz://scripts/project.dkey"));
            }
            catch(IOException)
            {
                stx = null;
                //MessageBox.Show("此项目还未创建授权文件, 请创建授权后重新打包!");
                //Process.GetCurrentProcess().Kill();
            }

            if (stx == null)
            {
                Logger.Warning("project.dkey not found!");
                byte[] data = ExtentionResource.GetBinaryContent("Ventuz6_Designer");
                _nv = new NativeWapper(data);

                //MessageBox.Show("此项目还未创建授权文件, 请创建授权后重新打包!");
                //Process.GetCurrentProcess().Kill();
            }
            else
            {
                BinaryReader reader = new BinaryReader(stx);
                _nv = new NativeWapper(reader.ReadBytes(1540));
                reader.Close();
                stx.Close();
            }

            vzg = _nv.Gx;
            _nv.Sx._161 = _projectfile;
        }
        #endregion

        #region 菜单扩展

        // 在 Ventuz.Designer.Shared.UIStyle  INIT 函数中加入调用
        public static void OnInitIcons(IconManager iconManager)
        {
            iconManager.AddBitmap("TransIcons", "QuanXin.Logo", new Bitmap(FilePaths.resDir + "qxlogo64x64.bmp"));
            iconManager.AddBitmap("TransIcons", "QuanXin.MiniLogo", new Bitmap(FilePaths.resDir + "qxlogo16x16.bmp"));
            iconManager.AddBitmap("TransIcons", "QuanXin.MiniLogo!", new Bitmap(FilePaths.resDir + "qxlogo16x16!.bmp"));
            iconManager.AddBitmap("TransIcons", "QuanXin.IPCam", new Bitmap(FilePaths.resDir + "ipcam.bmp"));
        }

        // 在 Ventuz.Designer.Mainform InitalizeComponent 函数中调用
        public static void ModifyRightMenus(ToolStrip mainRight)
        {
            var rightExtMenus = UIExtention.Instance.GetRightMenus();
            foreach(var f in  rightExtMenus)
            {
                mainRight.Items.Insert(0, f);
            }

            mainRight.Items.Insert(2 + rightExtMenus.Length, UIExtention.Instance.GetComponentMenu());
        }

        // 在 Ventuz.Debugger.SceneTabs InitalizeComponent 函数中调用
        public static ToolStripMenuItem[] ExternLeftMenus() => UIExtention.Instance.GetLeftMenus();

        #endregion

        #region 实现功能
        // 在 Ventuz.Kernel.Input.InputServer SendInputData 函数中调用
        public static void FilterEventAtomAtRender(double time, List<EventAtom> events) => EventFilter.Instance.FilterEventAtom(time, events);

        // 在 Ventuz.Designer.PropertyEditor GetMemberDisplayName 函数中调用
        public static string doTranslate(string srctext) => Trans.file.tryTranslate(srctext);

        // 在 Ventuz.Kernel.Gfx.VRender.Initialize 中调用
        public static void UpdateSceneLinks()
        {
            VScene sc = Compatible.VentuzWare.Instance.GetProjectScene();
            sc?.AbsTreeManager?.NotifyModuleChanged();
        }

        // Ventuz.Kernel.dll Ventuz.Kernel.IO.SingleScenePresentationManifest.Write
        public static void ExtendSceneUriCollection(UriCollection col)
        {
            VprExport extion = new VprExport();
            extion.AddProjectFiles(col);
        }

        // Ventuz.Kernel.Gfx VRenderSetup.PaintProgress 
        public static void DrawLoadingProgress(object vtexture, float progress) => RenderLoading.Instance.DoRender(vtexture, progress);

        // Ventuz.Presenter.Runner Runner File.Open
        public static Stream OpenDMZFile(string path) => new DrzStream(path);

        // Ventuz.Kernel.dll Ventuz.Kernel.IO.FileSchemeManager GetWriteStreamCore
        public static Stream WriteDMZFile(string path) => new DrzStream(path, true);

        // Ventuz.Kernel.dll Ventuz.Kernel.Gfx VRSBase Setup 
        public static object OnRsBaseSetup(object obj, object status)
        {
            if(!ExtentionConf.Instance.ForceModifySimple)
            {
                return status;
            }

            if(status is VRSTexture.VRSTextureStage.State)
            {
                VRSTexture.VRSTextureStage.State s = (VRSTexture.VRSTextureStage.State)status;
                s.SamplerState.MipFilter = VTextureFilterMip.None;
                //s.SamplerState.MagFilter = VTextureFilterMag.Anisotropic;
                //s.SamplerState.MinFilter = VTextureFilterMin.Anisotropic;
                return s;
            }

            return status;
        }

        // Ventuz.Kernel.IO.UriManager  GetSchemeManagerAndFixUri
        public static SchemeManager UriFilter(ref Uri uri) => ProjectResource.instance.UriFilter(ref uri);

        #endregion

        public static Random ran = new Random();
        private static NativeWapper _nv = null;
        public static dynamic vzg = null;
        public static object runproj = null;
    }
}
