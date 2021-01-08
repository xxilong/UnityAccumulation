using ShareLib.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Designer.Shared;
using Ventuz.Extention.Compatible;
using Ventuz.Kernel.CModel;
using Ventuz.Kernel.Sys;

namespace Ventuz.Extention.Nodes.Manop
{
    public class ContentNode
    {
        public static VProperty FindProperty(IVComponent comp, string name)
        {
            foreach(VPropertyDescriptor p in comp.Properties)
            {
                if(p.Name == name)
                {
                    return p.VProperty;
                }
            }

            return null;
        }

        public static VEvent FindEvent(IVComponent comp, string name)
        {
            foreach(VEvent e in comp.Events)
            {
                if(e.Name == name)
                {
                    return e;
                }
            }

            return null;
        }

        public static VMethod FindMethod(IVComponent comp, string name)
        {
            foreach(VMethod m in comp.Methods)
            {
                if(m.Name == name)
                {
                    return m;
                }
            }

            return null;
        }

        static public VComponent FindContentNode(string name)
        {
            VScene sc = VentuzWare.Instance.GetProjectScene();
            if (sc == null)
            {
                return null;
            }

            VComponentList vcl = sc.Components;
            foreach (VComponent c in vcl)
            {
                if (c.Name == name)
                {
                    return c;
                }
            }

            return null;
        }

        public static bool BindProperty(VProperty src, VProperty dst)
        {
            try
            {
                dst.BindTo(src);
                if (!(dst.Owner is VLinkable))
                {
                    src.Owner.FollowTargets = true;
                }

                VDesigner.NotifyEditors(src.Owner, EditorServiceType.Refresh, src.Owner.Site);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Binding failed: {ex}");
                return false;
            }
        }

        public static bool BindEvent(VEvent src, VMethod dst)
        {
            try
            {
                VEventBinding veventBinding = dst.BindTo(src);
                VEventBinding[] bindings = dst.Bindings;

                foreach(VEventBinding veventBinding2 in bindings)
                {
                    if (veventBinding != veventBinding2 && veventBinding.Event == veventBinding2.Event)
                    {  
                        veventBinding.Dispose();
                        return false;
                    }
                }

                if (!(dst.Owner is VLinkable))
                {
                    src.Owner.FollowTargets = true;
                }

                VDesigner.NotifyEditors(src.Owner, EditorServiceType.Refresh, src.Owner.Site);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Binding failed: {ex}");
                return false;
            }
        }

        public static bool BindProperty(string comp_src, string perty_src, string comp_dst, string perty_dst)
        {
            VComponent src = FindContentNode(comp_src);
            if(src == null)
            {
                Logger.Error($"Source Component {comp_src} not found.");
                return false;
            }

            VComponent dst = FindContentNode(comp_dst);
            if(dst == null)
            {
                Logger.Error($"Dest Component {comp_dst} not found.");
                return false;
            }

            VProperty sp = FindProperty(src, perty_src);
            if(sp == null)
            {
                Logger.Error($"Property {perty_src} not found in {comp_src}");
                return false;
            }

            VProperty dp = FindProperty(dst, perty_dst);
            if(dp == null)
            {
                Logger.Error($"Property {perty_dst} not found in {comp_dst}");
                return false;
            }

            return BindProperty(sp, dp);
        }
        
        public static bool BindEvent(string comp_src, string perty_src, string comp_dst, string perty_dst)
        {
            VComponent src = FindContentNode(comp_src);
            if (src == null)
            {
                Logger.Error($"Source Component {comp_src} not found.");
                return false;
            }

            VComponent dst = FindContentNode(comp_dst);
            if (dst == null)
            {
                Logger.Error($"Dest Component {comp_dst} not found.");
                return false;
            }

            VEvent sp = FindEvent(src, perty_src);
            if (sp == null)
            {
                Logger.Error($"Event {perty_src} not found in {comp_src}");
                return false;
            }

            VMethod dp = FindMethod(dst, perty_dst);
            if (dp == null)
            {
                Logger.Error($"Method {perty_dst} not found in {comp_dst}");
                return false;
            }

            return BindEvent(sp, dp);
        }

        public static void SetProperty(IVComponent comp, string perty, object value)
        {
            if(comp == null)
            {
                return;
            }

            VProperty p = FindProperty(comp, perty);
            if(p == null)
            {
                return;
            }

            p.Value = value;
        }

        public static void SetProperty(string comp, string perty, object value)
            => SetProperty(FindContentNode(comp), perty, value);
    }
}
