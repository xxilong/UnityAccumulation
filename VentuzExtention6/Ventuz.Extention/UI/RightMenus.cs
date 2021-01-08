using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventuz.Designer.Shared;
using Ventuz.Extention.Conf;
using Ventuz.Extention.MatLib;

namespace Ventuz.Extention.UI
{
    class RightMenus
    {
        public ToolStripMenuItem[] GetTopMenus()
        {
            return new ToolStripMenuItem[] {  };
        }

        public ToolStripItem GetComponentButton()
        {
            ToolStripButtonBackground btn = new ToolStripButtonBackground();

            btn.BackColorSignaled = Color.Empty;
            btn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            btn.ImageTransparentColor = Color.Black;
            btn.Name = "ComponentLibary";
            btn.Size = new Size(23, 22);
            btn.Image = new Bitmap(FilePaths.resDir + "qxlogo23x22.bmp");
            btn.Text = "Componet Libary: 打开/关闭组件管理窗口";
            btn.Click += ComponentManager.Instance.ToggleWindowVisibility;

            return btn;
        }
    }
}
