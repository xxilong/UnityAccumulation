using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Basics;
using Ventuz.Designer;
using Ventuz.Designer.Shared;
using Ventuz.Extention.Compatible;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Sys;

namespace Ventuz.Extention.Nodes.Manop
{
    public class HierarchyNode
    {
        static public VLinkable LoadBase64ToNode(string base64, string name)
        {
            VLinkable parent = FindHierarchyNode(name);
            if(parent == null)
            {
                return null;
            }

            return LoadFromBase64Archive(base64, parent);
        }

        static public VLinkable LoadFromBase64Archive(string base64, VLinkable parent)
        {
            ILinkListCollection linkCol = parent.LinkLists;
            ILinkList link = null;
            if (linkCol != null && !linkCol.IsEmpty)
            {
                link = linkCol[0];
            }

            byte[] bin = Convert.FromBase64String(base64);
            IVComponent[] array = VAdvancedSelection.Deserialize(bin, link.Owner.Parent);

            if(array == null || array.Length == 0)
            {
                return null;
            }
            
            foreach(VLinkable item in array)
            {
                SetLink(parent, item);
            }

            return array[0] as VLinkable;
        }
        
        static public VLinkable CopyNodes(VLinkable src, VLinkable parent)
        {
            VAdvancedSelection sel = new VAdvancedSelection(SelectionType.WithSources|SelectionType.WithChildren, uint.MaxValue);
            sel.BeginUpdate();
            sel.Add(src);
            sel.EndUpdate();

            byte[] bindata = sel.Serialize();
            string data = Convert.ToBase64String(bindata);
            return LoadFromBase64Archive(data, parent);
        }

        static public Axis CreateAxis(VLinkable parent, string name = "")
        {
            Axis node = new Axis(parent.Site, name);
            parent.Parent.Components.Add(node);
            SetLink(parent, node);
            return node;
        }

        static public IEnumerable<VLinkable> EnumChildrenHierarchyNode(VLinkable root)
        {
            yield return root;

            for(int i = 0; i < root.LinkLists.Count; ++i)
            {
                ILinkList links = root.LinkLists[i];
                for(int j = 0; j < links.Count; ++j)
                {
                    VLink link = links[j];
                    yield return link.Target;

                    foreach(VLinkable item in EnumChildrenHierarchyNode(link.Target))
                    {
                        yield return item;
                    }
                }
            }
        }

        static public IEnumerable<VComponent> EnumChildrenContentNode(VLinkable root)
        {
            HashSet<VComponent> enumed = new HashSet<VComponent>();

            foreach(VLinkable item in EnumChildrenHierarchyNode(root))
            {
                enumed.Add(item);
                foreach (VComponent cmp in EnumContentNodeInernal(enumed, item))
                {
                    yield return cmp;
                }
            }
        }

        static public IEnumerable<VComponent> EnumChildrenAllNode(VLinkable root)
        {
            HashSet<VComponent> enumed = new HashSet<VComponent>();

            foreach (VLinkable item in EnumChildrenHierarchyNode(root))
            {
                enumed.Add(item);
                yield return item;

                foreach (VComponent cmp in EnumContentNodeInernal(enumed, item))
                {
                    yield return cmp;
                }
            }
        }

        static private IEnumerable<VComponent> EnumContentNodeInernal(HashSet<VComponent> enumed, VComponent node)
        {
            VBindingCollection source = node.Sources;
            VBindingCollection target = node.Targets;

            if(source != null)
            {
                foreach(VBinding bind in source)
                {
                    VComponent s = bind.SourceC as VComponent;
                    if(s != null && !enumed.Contains(s))
                    {
                        enumed.Add(s);
                        yield return s;

                        foreach(VComponent child in EnumContentNodeInernal(enumed, s))
                        {
                            yield return child;
                        }
                    }

                    s = bind.TargetC as VComponent;
                    if (s != null && !enumed.Contains(s))
                    {
                        enumed.Add(s);
                        yield return s;

                        foreach (VComponent child in EnumContentNodeInernal(enumed, s))
                        {
                            yield return child;
                        }
                    }
                }
            }

            if(target != null)
            {
                foreach(VBinding bind in target)
                {
                    VComponent s = bind.SourceC as VComponent;
                    if (s != null && !enumed.Contains(s))
                    {
                        enumed.Add(s);
                        yield return s;

                        foreach (VComponent child in EnumContentNodeInernal(enumed, s))
                        {
                            yield return child;
                        }
                    }

                    s = bind.TargetC as VComponent;
                    if (s != null && !enumed.Contains(s))
                    {
                        enumed.Add(s);
                        yield return s;

                        foreach (VComponent child in EnumContentNodeInernal(enumed, s))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        static public VLinkable FindHierarchyNode(string name)
        {
            VScene sc = VentuzWare.Instance.GetProjectScene();
            if (sc == null)
            {
                return null;
            }

            VComponentList vcl = sc.Components;
            foreach (VComponent c in vcl)
            {
                if (c is VLinkable && c.Name == name)
                {
                    return c as VLinkable;
                }
            }

            return null;
        }

        static public VLinkable FindHierarchyNode(VLinkable root, string name)
        {
            foreach(VLinkable item in EnumChildrenHierarchyNode(root))
            {
                if(item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        static public IVComponent FindContentNode(VLinkable root, string name)
        {
            foreach(IVComponent comp in EnumChildrenContentNode(root))
            {
                if(comp.Name == name)
                {
                    return comp;
                }
            }

            return null;
        }

        static public IVComponent FindAllNode(VLinkable root, string name)
        {
            foreach(IVComponent comp in EnumChildrenAllNode(root))
            {
                if(comp.Name == name)
                {
                    return comp;
                }
            }

            return null;
        }

        static public void SetLink(VLinkable parent, VLinkable child)
        {
            if(parent == null || child == null || parent.LinkLists == null || parent.LinkLists.IsEmpty)
            {
                return;
            }

            parent.LinkLists[0].LinkTo(child);
        }

        static public void DeleteHierarchyNode(VLinkable root)
        {
            if(root == null)
            {
                return;
            }

            VSite site = root.Site;
            DeleteHierarchyWithoutRefresh(root);
            VDesigner.NotifyEditors(site, EditorServiceType.Refresh, site);
            root.Dispose();
        }

        static public void DeleteHierarchyNode(string name) => DeleteHierarchyNode(FindHierarchyNode(name));

        static public void DeleteChilds(VLinkable comp)
        {
            if (comp == null)
            {
                return;
            }

            VSite site = comp.Site;

            var childs = comp.LinkLists;
            for (int i = childs.Count - 1; i >= 0; --i)
            {
                var links = childs[i];
                for (int j = links.Count - 1; j >= 0; --j)
                {
                    var link = links[j];
                    DeleteHierarchyWithoutRefresh(link.Target);
                    link.Dispose();
                }
            }

            VDesigner.NotifyEditors(site, EditorServiceType.Refresh, site);
        }

        static public void DeleteChilds(string root) => DeleteChilds(FindHierarchyNode(root));
        static public void DeleteChildsWithContent(string root) => DeleteChildsWithContent(FindHierarchyNode(root));

        static public void DeleteChildsWithContent(VLinkable comp)
        {
            VAdvancedSelection sel = new VAdvancedSelection(SelectionType.WithSources | SelectionType.WithChildren, uint.MaxValue);
            sel.BeginUpdate();
            sel.Add(comp);
            sel.EndUpdate();

            foreach(var p in sel.IndirectSelection)
            {
                if(p != comp)
                {
                    p.Dispose();
                }
            }

            VDesigner.NotifyEditors(comp.Site, EditorServiceType.Refresh, comp.Site);
        }

        static private void DeleteHierarchyWithoutRefresh(VLinkable comp)
        {
            var childs = comp.LinkLists;
            for (int i = childs.Count - 1; i >= 0; --i)
            {
                var links = childs[i];
                for (int j = links.Count - 1; j >= 0; --j)
                {
                    var link = links[j];
                    DeleteHierarchyWithoutRefresh(link.Target);
                    link.Dispose();
                }
            }
            comp.Dispose();
        }
    }
}
