using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Ventuz.Extention.Compatible;
using Ventuz.Extention.VentuzHook;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Layers;
using Ventuz.Kernel.Sys;

namespace Ventuz.Extention.MatLib
{
    class NodeSearcher
    {
        public static NodeSearcher Instance = new NodeSearcher();
        public class SearchResultItem
        {
            public SearchResultItem(string item)
            {
                itemGuid = Guid.Empty;
                itemDesc = item;
            }

            public SearchResultItem(Guid guid, string item)
            {
                itemGuid = guid;
                itemDesc = item;
            }

            public Guid itemGuid;
            public string itemDesc;
        }

        public void OpenSearchDialog(object sender, EventArgs earg)
        {
            SearchDialog dlg = new SearchDialog();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.Show();
        }

        public List<SearchResultItem> SearchNodes(string text, bool name, bool type, bool pname, bool pvalue)
        {
            List<SearchResultItem> result = new List<SearchResultItem>();

            text = text.Trim();
            if(string.IsNullOrEmpty(text))
            {
                result.Add(new SearchResultItem("请输入要搜索的内容"));
                return result;
            }

            if(!(name || type || pname || pvalue))
            {
                result.Add(new SearchResultItem("请选择要搜索的项"));
                return result;
            }

            VScene s = VentuzWare.Instance.GetProjectScene();
            if (s == null)
            {
                result.Add(new SearchResultItem("还未打开场景!"));
                return result;
            }
            
            VComponentList comps = s.Components;
            for (int i = 0; i < comps.Count; ++i)
            {
                VComponent comp = comps[i];

                if(comp.ToolboxName == null)
                {
                    continue;
                }

                if(name && comp.Name.Contains(text))
                {
                    result.Add(new SearchResultItem(comp.ID, ShowNodeAsResult(comp)));
                    continue;
                }

                if (type && comp.ToolboxName.Contains(text))
                {
                    result.Add(new SearchResultItem(comp.ID, ShowNodeAsResult(comp)));
                    continue;
                }

                if(pname || pvalue)
                {
                    PropertyDescriptorCollection props = comp.Properties;
                    for (int j = 0; j < props.Count; ++j)
                    {
                        PropertyDescriptor prop = props[j];
                        if(pname && prop.Name.Contains(text))
                        {
                            result.Add(new SearchResultItem(comp.ID, ShowNodeAsResult(comp)));
                            break;
                        }

                        if(pvalue)                            
                        {
                            object value = prop.GetValue(comp);
                            if(value != null && value.ToString().Contains(text))
                            {
                                result.Add(new SearchResultItem(comp.ID, ShowNodeAsResult(comp)));
                                break;
                            }
                        }                            
                    }
                }
            }

            return result;
        }

        private string ShowNodeAsResult(VComponent comp)
        {
            string desc = $"{comp}";
            VLinkable link = comp as VLinkable;
            if(link == null)
            {
                link = FindContentNodeLink(comp, ref desc);
            }

            return ShowLinkableAsResult(link, desc) + "\r\n";
        }

        struct SearchItem
        {
            public string desc;
            public IVComponent node;
        }

        private VLinkable FindContentNodeLink(VComponent comp, ref string desc)
        {
            Queue<SearchItem> items = new Queue<SearchItem>();
            items.Enqueue(new SearchItem { desc = desc, node = comp});

            while(items.Count != 0)
            {
                SearchItem item = items.Dequeue();
                VBindingCollection binds = item.node.Targets;

                for (int i = 0; i < binds.Count; ++i)
                {
                    VBinding bind = binds[i];
                    IVComponent target = bind.TargetC;
                    string dsc = $"{item.desc} - {target}";

                    if(target is VLinkable)
                    {
                        desc = dsc;
                        return target as VLinkable;
                    }

                    items.Enqueue(new SearchItem { desc = dsc, node = target });
                }
            } 
                                 
            return GetHerialLinkRoot(comp.Parent.Components);
        }

        private string ShowLinkableAsResult(VLinkable link, string desc)
        {
            if(link == null)
            {
                return desc;
            }

            if(link.Links.Count == 0)
            {
                return desc;
            }

            int len = desc.Length;
            string result = "";

            for(int i = 0; i < link.Links.Count; ++i)
            {
                VLinkable parent = link.Links[i].Source;
                desc += $" > {parent}";
                if(!string.IsNullOrEmpty(result))
                {
                    result += "\r\n";
                }
                result += ShowLinkableAsResult(parent, desc);
                desc = "".PadLeft(len);
            }

            return result;
        }

        public void ExportNodeTree(object sender, EventArgs earg)
        {
            VScene s = VentuzWare.Instance.GetProjectScene();
            if(s == null)
            {
                MessageBox.Show(HookEntry.vzg._29, "还未打开任何场景!", "导出失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "文本文档|*.txt";
            dlg.AddExtension = true;
            dlg.DefaultExt = ".txt";

            if (dlg.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using (Stream fStream = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter wr = new StreamWriter(fStream))
                {
                    VComponent node = GetLayerRoot(s);
                    PrintNode(node, wr, "");
                }
            }
        }

        private void PrintNode(VComponent node, StreamWriter wr, string prefex)
        {
            wr.WriteLine($"{prefex}{node.Name}({node.ToolboxName})");
            VLinkable link = node as VLinkable;
            if(link == null)
            {
                return;
            }

            for(int i = 0; i < link.LinkLists.Count; ++i)
            {
                ILinkList links = link.LinkLists[i];
                for(int j = 0; j < links.Count; ++j)
                {
                    VLink childLink = links[j];
                    PrintNode(childLink.Target, wr, prefex + "--");
                }
            }

            Layer3D layer = node as Layer3D;
            if(layer == null)
            {
                return;
            }

            PrintNode(layer.RootNode, wr, prefex + "**");
        }

        private VComponent GetLayerRoot(VScene s)
        {
            VComponentList comps = s.MainContainer.Components;
            for (int i = 0; i < comps.Count; ++i)
            {
                VComponent comp = comps[i];
                if(comp.Name == "__layerroot__")
                {
                    return comp;
                }               
            }

            return null;
        }

        private VLinkable GetHerialLinkRoot(VComponentList list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                VComponent comp = list[i];
                if (comp.Name == "3D Layer Root")
                {
                    return comp as VLinkable;
                }
            }

            return null;
        }
    }
}
