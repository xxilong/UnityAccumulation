using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.Designer;
using Ventuz.Extention.VentuzHook;
using Ventuz.Kernel.CModel;

namespace Ventuz.Extention.Nodes.Manop
{
    public class Designer
    {
        static public void DeleteChild(VLinkable item)
        {
            if(editor == null)
            {
                return;
            }
            
            editor.ClearSelection();
            editor.ToggleInSelection(item);
            editor.Delete();
        }

        static private HierarchyEditor editor {
            get => HookEntry.vzg._77 as HierarchyEditor;
        }
    }
}
