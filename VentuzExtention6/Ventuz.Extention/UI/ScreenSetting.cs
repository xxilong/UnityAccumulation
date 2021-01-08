using System.Windows.Forms;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.UI
{
    public partial class ScreenSetting : Form
    {
        public ScreenSetting()
        {
            InitializeComponent();

            ScreenWidth.Text = TouchScreen.Width.ToString("G");
            ScreenHeight.Text = TouchScreen.Height.ToString("G");
        }

        private void ScreenSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            TouchScreen.Width = double.Parse(ScreenWidth.Text);
            TouchScreen.Height = double.Parse(ScreenHeight.Text);
            TouchScreen.SaveSetting();
        }
    }
}
