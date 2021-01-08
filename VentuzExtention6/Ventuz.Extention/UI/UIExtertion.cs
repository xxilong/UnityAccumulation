using System.Windows.Forms;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.UI
{
    public class UIExtention
    {
        public static UIExtention Instance = new UIExtention();

        private UIExtention()
        {
        }

        public ToolStripMenuItem[] GetRightMenus() => rightMenus.GetTopMenus();
        public ToolStripMenuItem[] GetLeftMenus() => leftMenus.GetTopMenus();
        public ToolStripItem GetComponentMenu() => rightMenus.GetComponentButton();
        
        public void OnHotKey(int unkKeyCode)
        {
            //Console.WriteLine("{0}", unkKeyCode);

            if (unkKeyCode == 108)
            {
                TouchPlayer.player.PlayStep();
            }
        }

        private LeftMenus leftMenus = new LeftMenus();
        private RightMenus rightMenus = new RightMenus();
    }
}
