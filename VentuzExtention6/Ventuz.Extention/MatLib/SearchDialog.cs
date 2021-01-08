using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventuz.Extention.Compatible;

namespace Ventuz.Extention.MatLib
{
    public partial class SearchDialog : Form
    {
        public SearchDialog()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var res = NodeSearcher.Instance.SearchNodes(searchText.Text, 
                nodeName.Checked, nodeValue.Checked, propertyName.Checked, propertyValue.Checked);

            listView.Items.Clear();
            foreach(var item in res)
            {
                ListViewItem listItem = listView.Items.Add(item.itemDesc);
                listItem.Tag = item.itemGuid;
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            if(listView.SelectedItems.Count != 1)
            {
                return;
            }

            ListViewItem item = listView.SelectedItems[0];
            if(item.Tag == null || (Guid)item.Tag == Guid.Empty)
            {
                return;
            }

            Guid sItem = (Guid)item.Tag;

            VentuzWare.Instance.GetDocumentMager()?.Navigate(sItem);
        }
    }
}
