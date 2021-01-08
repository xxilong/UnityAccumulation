using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ShareLib.Log;

namespace Ventuz.Extention.MatLib
{
    public partial class ComponentLibary : UserControl
    {
        private Brush tab_backgroudBrush = new SolidBrush(Color.FromArgb(37, 37, 38));
        private Brush tab_labelBG = new SolidBrush(Color.FromArgb(37, 37, 38));
        private Brush tab_labelFT = new SolidBrush(Color.FromArgb(200, 200, 200));
        private StringFormat tab_strFormat = new StringFormat();
        private Font tab_labelFont = new Font("宋体", 9f);

        public ComponentLibary()
        {
            tab_strFormat.LineAlignment = StringAlignment.Center;
            tab_strFormat.Alignment = StringAlignment.Center;

            InitializeComponent();
            ReloadPackerItems();
        }

        public void ReloadPackerItems()
        {
            string libPath = ComponentManager.Instance.LibaryPath;

            layerList.Items.Clear();
            hierList.Items.Clear();
            ctxList.Items.Clear();
            scriptList.Items.Clear();

            if(libPath == null)
            {
                this.libPath.Text = "NULL";
                return;
            }

            this.libPath.Text = libPath;

            try
            {
                foreach (var f in new DirectoryInfo(libPath).GetFiles("*.vpk", SearchOption.TopDirectoryOnly))
                {
                    string name = f.Name;

                    if (name.EndsWith(".layers.vpk"))
                    {
                        layerList.Items.Add(name.Substring(0, name.Length - 11));
                    }
                    else if (name.EndsWith(".hierarchy.vpk"))
                    {
                        hierList.Items.Add(name.Substring(0, name.Length - 14));
                    }
                    else if (name.EndsWith(".content.vpk"))
                    {
                        ctxList.Items.Add(name.Substring(0, name.Length - 12));
                    }
                    else if (name.EndsWith(".script.vpk"))
                    {
                        scriptList.Items.Add(name.Substring(0, name.Length - 11));
                    }
                }
            }
            catch(Exception e)
            {
                Logger.Error($"Exception on load ventuz lib: {e}");
            }
        }

        private void ComponentLibary_Resize(object sender, EventArgs e)
        {
            Size size = this.Size;
            size.Height -= panelTab.Top;
            size.Height -= 2;
            panelTab.Size = size;

            layerList.Columns[0].Width = size.Width;
            hierList.Columns[0].Width = size.Width;
            ctxList.Columns[0].Width = size.Width;
            scriptList.Columns[0].Width = size.Width;

            size = panelTab.ClientSize;
            size.Width += 10;
            layerList.Size = size;
            hierList.Size = size;
            ctxList.Size = size;
            scriptList.Size = size;
        }

        private void panelTab_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(tab_backgroudBrush, panelTab.ClientRectangle);

            for (int i = 0; i < panelTab.TabPages.Count; i++)
            {
                Rectangle recChild = panelTab.GetTabRect(i);
                e.Graphics.FillRectangle(tab_labelBG, recChild);

                if(panelTab.SelectedIndex == i)
                {
                    recChild.Inflate(4, 4);
                }

                e.Graphics.FillRectangle(tab_labelBG, recChild);
                e.Graphics.DrawString(panelTab.TabPages[i].Text, tab_labelFont, tab_labelFT, recChild, tab_strFormat);
            }
        }

        private void SelLibPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            if(path.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ComponentManager.Instance.LibaryPath = path.SelectedPath;
            ReloadPackerItems();
        }

        private void ImportByName(string name)
        {
            NodeImportor.Instance.ImportArchiveFile(Path.Combine(ComponentManager.Instance.LibaryPath, name));
        }

        private void layerList_DoubleClick(object sender, EventArgs e)
        {
            ImportByName(layerList.SelectedItems[0].Text + ".layers.vpk");
        }

        private void hierList_DoubleClick(object sender, EventArgs e)
        {
            ImportByName(hierList.SelectedItems[0].Text + ".hierarchy.vpk");
        }

        private void ctxList_DoubleClick(object sender, EventArgs e)
        {
            ImportByName(ctxList.SelectedItems[0].Text + ".content.vpk");
        }

        private void scriptList_DoubleClick(object sender, EventArgs e)
        {
            ImportByName(scriptList.SelectedItems[0].Text + ".script.vpk");
        }
    }
}
