using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Kernel.CModel;

namespace Ventuz.Extention.Nodes.Manop
{
    public class VentuzNode
    {
        static public void SetSubNodeProperty(VLinkable root, string subcomp, string property, object value)
        {
            IVComponent comp = HierarchyNode.FindAllNode(root, subcomp);
            ContentNode.SetProperty(comp, property, value);
        }

        static public void NegSubNodeProperty(VLinkable root, string subcomp, string property)
        {
            IVComponent comp = HierarchyNode.FindAllNode(root, subcomp);

            if(comp == null)
            {
                return;
            }

            VProperty p = ContentNode.FindProperty(comp, property);
            if (p == null)
            {
                return;
            }

            if(p.Value is int)
            {
                p.Value = -(int)p.Value;
            }
            else if(p.Value is float)
            {
                p.Value = -(float)p.Value;
            }
            else if(p.Value is double)
            {
                p.Value = -(double)p.Value;
            }
        }
    }
}
