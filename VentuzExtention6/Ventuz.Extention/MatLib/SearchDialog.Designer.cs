namespace Ventuz.Extention.MatLib
{
    partial class SearchDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.searchText = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.nodeName = new System.Windows.Forms.CheckBox();
            this.nodeValue = new System.Windows.Forms.CheckBox();
            this.propertyName = new System.Windows.Forms.CheckBox();
            this.propertyValue = new System.Windows.Forms.CheckBox();
            this.listView = new System.Windows.Forms.ListView();
            this.Header = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // searchText
            // 
            this.searchText.Location = new System.Drawing.Point(12, 9);
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(307, 21);
            this.searchText.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.DimGray;
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.DimGray;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearch.Location = new System.Drawing.Point(325, 7);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(50, 25);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // nodeName
            // 
            this.nodeName.AutoSize = true;
            this.nodeName.Location = new System.Drawing.Point(396, 13);
            this.nodeName.Name = "nodeName";
            this.nodeName.Size = new System.Drawing.Size(72, 16);
            this.nodeName.TabIndex = 3;
            this.nodeName.Text = "节点名称";
            this.nodeName.UseVisualStyleBackColor = true;
            // 
            // nodeValue
            // 
            this.nodeValue.AutoSize = true;
            this.nodeValue.Location = new System.Drawing.Point(491, 13);
            this.nodeValue.Name = "nodeValue";
            this.nodeValue.Size = new System.Drawing.Size(72, 16);
            this.nodeValue.TabIndex = 3;
            this.nodeValue.Text = "节点类型";
            this.nodeValue.UseVisualStyleBackColor = true;
            // 
            // propertyName
            // 
            this.propertyName.AutoSize = true;
            this.propertyName.Location = new System.Drawing.Point(583, 13);
            this.propertyName.Name = "propertyName";
            this.propertyName.Size = new System.Drawing.Size(72, 16);
            this.propertyName.TabIndex = 3;
            this.propertyName.Text = "属性名称";
            this.propertyName.UseVisualStyleBackColor = true;
            // 
            // propertyValue
            // 
            this.propertyValue.AutoSize = true;
            this.propertyValue.Location = new System.Drawing.Point(674, 13);
            this.propertyValue.Name = "propertyValue";
            this.propertyValue.Size = new System.Drawing.Size(72, 16);
            this.propertyValue.TabIndex = 3;
            this.propertyValue.Text = "属性内容";
            this.propertyValue.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Header});
            this.listView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 40);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(921, 609);
            this.listView.TabIndex = 4;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // Header
            // 
            this.Header.Text = "搜索结果";
            this.Header.Width = 917;
            // 
            // SearchDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(921, 649);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.propertyValue);
            this.Controls.Add(this.propertyName);
            this.Controls.Add(this.nodeValue);
            this.Controls.Add(this.nodeName);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.searchText);
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "SearchDialog";
            this.Text = "场景节点搜索";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.CheckBox nodeName;
        private System.Windows.Forms.CheckBox nodeValue;
        private System.Windows.Forms.CheckBox propertyName;
        private System.Windows.Forms.CheckBox propertyValue;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader Header;
    }
}