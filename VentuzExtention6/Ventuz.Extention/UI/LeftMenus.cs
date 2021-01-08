using System.Windows.Forms;

namespace Ventuz.Extention.UI
{
    class LeftMenus
    {
        public ToolStripMenuItem[] GetTopMenus()
        {
            ToolStripMenuItem marker = new ToolStripMenuItem();
            marker.Text = "Marker";
            marker.DropDownItems.AddRange(ExtMenuItems.Instance.GetRecorderMenus());
            marker.DropDownItems.Add(new ToolStripSeparator());
            marker.DropDownItems.AddRange(ExtMenuItems.Instance.GetMarkerMenus());

            ToolStripMenuItem archive = new ToolStripMenuItem();
            archive.Text = "Archive";
            archive.DropDownItems.AddRange(ExtMenuItems.Instance.GetArchiveMenu());

            ToolStripMenuItem misc = new ToolStripMenuItem();
            misc.Text = "Misc";
            misc.DropDownItems.AddRange(ExtMenuItems.Instance.GetMiscMenu());
            return new ToolStripMenuItem[] { marker, archive, misc };
        }
    }
}
