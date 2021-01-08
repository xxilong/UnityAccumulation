using ShareLib.Packer;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Ventuz.Designer;
using Ventuz.Extention.Compatible;
using Ventuz.Extention.Nodes.Manop;
using Ventuz.Extention.VentuzHook;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Layers;

namespace Ventuz.Extention.MatLib
{
    class NodeImportor
    {
        public void ImportArchive(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Ventuz 存档|*.vpk|Ventuz 图层存档|*.layers.vpk|Ventuz 层次节点存档" +
                "|*.hierarchy.vpk|Ventuz 内容节点存档|*.content.vpk|Ventuz 脚本|*.script.vpk" +
                "|层次节点(Base64编码)|*.hvpk.txt";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            if(dlg.FileName.EndsWith(".hvpk.txt"))
            {
                ImportBase64Archive(dlg.FileName);
            }
            else
            {
                ImportArchiveFile(dlg.FileName);
            }
        }

        public void ImportBase64Archive(string filepath)
        {
            HierarchyEditor hier = (HierarchyEditor)HookEntry.vzg._77;
            if(hier == null)
            {
                return;
            }

            VLink vlink = (hier.Selection.DirectLinksCount == 1) ? hier.Selection.DirectLinks[0] : null;
            ILinkList linkList = (vlink != null) ? vlink.LinkList : null;
            int index = (vlink != null) ? vlink.Index : -1;
            if (linkList == null && hier.Container.Root.Out != null)
            {
                linkList = hier.Container.Root.Out;
            }

            string data = File.ReadAllText(filepath);
            HierarchyNode.LoadFromBase64Archive(data, linkList.Owner);
        }

        public void ImportArchiveFile(string filepath)
        {
            LayerEditor layer = (LayerEditor)HookEntry.vzg._50;
            ContentEditor content = (ContentEditor)HookEntry.vzg._110;
            HierarchyEditor hier = (HierarchyEditor)HookEntry.vzg._77;

            if (layer == null || hier == null || content == null)
            {
                MessageBox.Show(HookEntry.vzg._29, "未找到编辑器实例!", "导入失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int importedLayer = 0;
            int importedHierarchy = 0;
            int importedContent = 0;
            int importedResource = 0;
            
            using (Stream fStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                FileArchiver ar = new FileArchiver(fStream);

                byte[] VentuzComponentsBin = ar.GetFile("@layerComponents");
                if(VentuzComponentsBin != null)
                {
                    importedLayer += ImportLayers(layer, VentuzComponentsBin);
                }

                VentuzComponentsBin = ar.GetFile("@hierarchyComponents");
                if(VentuzComponentsBin != null)
                {
                    importedHierarchy += ImportHierarchyNodes(hier, VentuzComponentsBin);
                }

                VentuzComponentsBin = ar.GetFile("@contentComponents");
                if(VentuzComponentsBin != null)
                {
                    importedContent += ImportContentNodes(content, VentuzComponentsBin);
                }

                string projectPath = VentuzWare.Instance.GetProjectPath();
                importedResource = ar.ExtractDiskFiles(projectPath);
            }
            
            MessageBox.Show(HookEntry.vzg._29,
                $"导入完成, 导入了 {importedLayer} 个图层, {importedHierarchy} 个层次节点, {importedContent} 个内容节点, {importedResource} 个资源文件",
                "导入成功",  MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private int ImportLayers(LayerEditor layer, byte[] data)
        {
            int count = 0;

            foreach (IVComponent ivcomponent in VAdvancedSelection.Deserialize(data, layer.MMC))
            {
                ++count;
                LayerManager.AddLayer(layer.MMC, ivcomponent as BaseLayer, layer.Selection.DirectSelection.FirstOrDefault<IVComponent>() as BaseLayer, null, false);
            }

            layer.UpdateAll();
            return count;
        }

        public int ImportHierarchyNodes(HierarchyEditor hier, byte[] data)
        {
            VLink vlink = (hier.Selection.DirectLinksCount == 1) ? hier.Selection.DirectLinks[0] : null;
            ILinkList linkList = (vlink != null) ? vlink.LinkList : null;
            int index = (vlink != null) ? vlink.Index : -1;
            if (linkList == null && hier.Container.Root.Out != null)
            {
                linkList = hier.Container.Root.Out;
            }
            hier.PasteRaw(data, linkList, index, false);
            return 1;
        }

        public int ImportContentNodes(ContentEditor content, byte[] data)
        {
            VAdvancedSelection.Deserialize(data, content.Container);
            ContentFamilies.Cleanup(content.Container);
            content.SourceSelection.Update();
            return 1;
        }

        public static NodeImportor Instance = new NodeImportor();
    }
}
