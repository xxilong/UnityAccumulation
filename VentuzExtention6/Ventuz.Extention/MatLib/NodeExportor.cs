using ShareLib.Packer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ventuz.Basics;
using Ventuz.Designer;
using Ventuz.Extention.Compatible;
using Ventuz.Extention.VentuzHook;
using Ventuz.Kernel;
using Ventuz.Kernel.CModel;

namespace Ventuz.Extention.MatLib
{
    class NodeExportor
    {
        public void ExportLayers(object sender, EventArgs e)
        {
            LayerEditor layer = (LayerEditor)HookEntry.vzg._50;
            if(layer == null)
            {
                MessageBox.Show(HookEntry.vzg._29, "还未设置图层编辑器实例!", "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var sel = layer.Selection;
            if(sel.IsEmpty)
            {
                MessageBox.Show(HookEntry.vzg._29, "请先选择要导出的图层!", "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Ventuz 图层存档|*.layers.vpk";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".layers.vpk";
            dlg.InitialDirectory = ComponentManager.Instance.LibaryPath != null ? ComponentManager.Instance.LibaryPath : "";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            int extPos = dlg.FileName.IndexOf(".vpk");
            if (extPos > 0)
            {
                dlg.FileName = dlg.FileName.Substring(0, extPos + 4);
            }

            using (Stream fStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                FileArchiver ar = new FileArchiver(fStream);
                byte[] VentuzComponentsBin = HierarchyEditor.CopyRaw(sel);
                ar.PushFile("@layerComponents", VentuzComponentsBin);
                PushResources(ar, sel);
            }

            Success("导出图层完成.");
        }

        public void ExportHierarchyNodes(object sender, EventArgs e)
        {
            HierarchyEditor hier = GetHierarchy();
            if (hier == null)
            {
                return;
            }

            var sel = hier.Selection;
            if (sel.IsEmpty)
            {
                MessageBox.Show(HookEntry.vzg._29, "请先选择要导出的层次节点!", "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Ventuz 层次节点存档|*.hierarchy.vpk";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".hierarchy.vpk";
            dlg.InitialDirectory = ComponentManager.Instance.LibaryPath != null ? ComponentManager.Instance.LibaryPath : "";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            int extPos = dlg.FileName.IndexOf(".vpk");
            if (extPos > 0)
            {
                dlg.FileName = dlg.FileName.Substring(0, extPos + 4);
            }

            using (Stream fStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                FileArchiver ar = new FileArchiver(fStream);
                byte[] VentuzComponentsBin = HierarchyEditor.CopyRaw(sel);
                ar.PushFile("@hierarchyComponents", VentuzComponentsBin);
                PushResources(ar, sel);
            }

            Success("导出层次节点完成.");
        }

        public void ExportHierarchyNodesBase64(object sender, EventArgs e)
        {
            HierarchyEditor hier = GetHierarchy();
            if (hier == null)
            {
                return;
            }

            var sel = hier.Selection;
            if (sel.IsEmpty)
            {
                Failed("请先选择要导出的层次节点!");
                return;
            }

            if(sel.DirectSelectionCount != 1)
            {
                Failed("只能选择一个节点!");
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Ventuz 层次节点存档|*.hvpk.txt";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".hvpk.txt";
            dlg.InitialDirectory = VentuzWare.Instance.GetProjectPath() + "Data";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            int extPos = dlg.FileName.IndexOf(".txt");
            if (extPos > 0)
            {
                dlg.FileName = dlg.FileName.Substring(0, extPos + 4);
            }

            using (Stream fStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                StreamWriter ar = new StreamWriter(fStream);
                byte[] VentuzComponentsBin = HierarchyEditor.CopyRaw(sel);
                string data = Convert.ToBase64String(VentuzComponentsBin);
                ar.Write(data);
                ar.Close();
            }

            Success("导出成功.");
        }

        #region ExportContent
        public void ExportContentNodesDefault(object sender, EventArgs e) =>
            ExportContentNodeWithSelectType(
                SelectionType.WithDirectBindings | SelectionType.WithoutLayers | SelectionType.WithSources);

        public void ExportContentNodeOnlySelect(object sender, EventArgs e) =>
            ExportContentNodeWithSelectType(
                SelectionType.None | SelectionType.WithoutLayers | SelectionType.WithoutLinkableSources);

        public void ExportContentNodeWithInput(object sender, EventArgs e) =>
            ExportContentNodeWithSelectType(
                SelectionType.WithSources | SelectionType.WithoutLayers | SelectionType.WithoutLinkableSources);

        private void ExportContentNodeWithSelectType(SelectionType seltype)
        {
            ContentEditor content = (ContentEditor)HookEntry.vzg._110;
            if (content == null)
            {
                Failed("还未设置内容编辑器实例!");
                return;
            }

            object[] sel = content.Selection;
            int selcount = sel.Length;
            if (selcount == 0)
            {
                Failed("请先选择要导出的内容节点!");
                return;
            }

            VAdvancedSelection vs = new VAdvancedSelection(seltype, uint.MaxValue);

            vs.BeginUpdate();
            for (int i = 0; i < selcount; ++i)
            {
                if (!(sel[i] is VLinkable))
                {
                    vs.Add((VComponent)sel[i]);
                }
            }
            vs.EndUpdate();

            selcount = vs.DirectSelectionCount;
            if (selcount == 0)
            {
                Failed("请先选择要导出的内容节点!");
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Ventuz 内容节点存档|*.content.vpk";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".content.vpk";
            dlg.InitialDirectory = ComponentManager.Instance.LibaryPath != null ? ComponentManager.Instance.LibaryPath : "";

            if (selcount == 1)
            {
                var selcomp = vs.FirstDirectSelectionOrNull;
                if (selcomp == null)
                {
                    Failed("请先选择要导出的内容节点!");
                    return;
                }

                if (selcomp is ScriptCSharp)
                {
                    ScriptCSharp cs = (ScriptCSharp)selcomp;
                    dlg.Filter = "Ventuz 脚本|*.script.vpk|Ventuz 内容节点存档|*.content.vpk";
                    dlg.DefaultExt = ".script.vpk";
                }

                if (selcomp is VComponent)
                {
                    VComponent cp = (VComponent)selcomp;
                    dlg.FileName = cp.Name;
                    dlg.AddExtension = false;
                }
            }
            
            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }                      

            int extPos = dlg.FileName.IndexOf(".vpk");
            if (extPos > 0)
            {
                dlg.FileName = dlg.FileName.Substring(0, extPos + 4);
            }

            using (Stream fStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                FileArchiver ar = new FileArchiver(fStream);
                ar.PushFile("@contentComponents", vs.Serialize());
                PushResources(ar, vs);
            }

            Success("导出内容节点完成.");
        }

        #endregion

        private void PushResources(FileArchiver ar, VAdvancedSelection sel)
        {
            VLog.Debug("Archiver", "*** Archive Items ***");
            VLog.Debug("Archiver", $"Direct_Component: {sel.DirectSelectionCount}, Indirect_Component: {sel.IndirectSelection.Length}");
            VLog.Debug("Archiver", $"Direct_Link: {sel.DirectLinksCount}, Indirect_Link: {sel.IndirectLinksCount}");
            VLog.Debug("Archiver", $"All_Builds: {sel.AllBindings.Length}");

            var compes = sel.IndirectSelection;

            HashSet<string> resFiles = new HashSet<string>();

            foreach(var comp in compes)
            {
                var props = comp.Properties;
                VLog.Debug("Archiver", $"Component: [{comp.Name}]({ comp.ID })");
   
                for(int i = 0; i < props.Count; ++i)
                {
                    var prop = props[i];

                    if(typeof(Uri) == prop.PropertyType || typeof(string) == prop.PropertyType)
                    {
                        object propval = prop.GetValue(comp);
                        if(propval == null)
                        {
                            continue;
                        }

                        string url = propval.ToString();
                        VLog.Debug("Archiver", $"  Prop {prop.Name}({prop.PropertyType}): {url}");

                        if(url.StartsWith("ventuz://"))
                        {
                            url = url.Substring(9);
                            url = url.Replace('/', '\\');
                            resFiles.Add(url);
                        }
                    }
                }
            }

            PushResFiles(ar, resFiles);
            VLog.Debug("Archiver", "*** End Archive Items ***");
        }

        private void PushResFiles(FileArchiver ar, HashSet<string> files)
        {
            string projectPath = VentuzWare.Instance.GetProjectPath();

            foreach(var file in files)
            {
                string resFile = Path.Combine(projectPath, file);
                if(!ar.PushDiskFile(file, resFile))
                {
                    VLog.Error("Archiver", $"导出文件 {file} 失败, 可能是文件太大了.", VPopup.DesignOnly);
                }
            }
        }

        private void Failed(string msg) => MessageBox.Show(HookEntry.vzg._29, msg, "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void Success(string msg)
        {
            MessageBox.Show(HookEntry.vzg._29, msg, "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ComponentManager.Instance.Exported();
        }

        private HierarchyEditor GetHierarchy()
        {
            HierarchyEditor hier = (HierarchyEditor)HookEntry.vzg._77;
            if (hier == null)
            {
                Failed("还未设置层次编辑器实例!");
                return null;
            }

            return hier;
        }

        public static NodeExportor Instance = new NodeExportor();
    }
}
