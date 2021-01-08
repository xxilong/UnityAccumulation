using ShareLib.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventuz.Designer;
using Ventuz.Extention.VentuzHook;

namespace Ventuz.Extention.MatLib
{
    class ComponentManager
    {
        public static ComponentManager Instance = new ComponentManager();
        private ComponentManager(){}
        
        public void Init()
        {
            if(win != null)
            {
                return;
            }

            LibaryPath = ModConfig.getconf<string>("ComponentManager", "libPath", null);
            win = new ComponentLibary();
            win.Dock = DockStyle.Fill;
            ((Mainform)HookEntry.vzg._29).WindowLayoutManager.RegisterDockableWindow(Designer.DesignerWindow.ComponentLibary, win);
        }

        public void ToggleWindowVisibility(object sender, EventArgs args)
        {
            ((Mainform)HookEntry.vzg._29).ToggleWindowVisibility(Designer.DesignerWindow.ComponentLibary);
        }

        public void Exported()
        {
            win.ReloadPackerItems();
        }

        public string LibaryPath {
            get
            {
                return _libPath;
            }
            set
            {
                _libPath = value;
                ModConfig.setconf("ComponentManager", "libPath", _libPath);
            }
        }

        private string _libPath = null;
        private ComponentLibary win = null;
    }
}
