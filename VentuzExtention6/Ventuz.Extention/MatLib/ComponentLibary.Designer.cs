namespace Ventuz.Extention.MatLib
{
    partial class ComponentLibary
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SelLibPath = new System.Windows.Forms.Button();
            this.libPath = new System.Windows.Forms.Label();
            this.panelTab = new System.Windows.Forms.TabControl();
            this.layerPanel = new System.Windows.Forms.TabPage();
            this.layerList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hierPanel = new System.Windows.Forms.TabPage();
            this.hierList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ctxPanel = new System.Windows.Forms.TabPage();
            this.ctxList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.scriptPanel = new System.Windows.Forms.TabPage();
            this.scriptList = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panelTab.SuspendLayout();
            this.layerPanel.SuspendLayout();
            this.hierPanel.SuspendLayout();
            this.ctxPanel.SuspendLayout();
            this.scriptPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelLibPath
            // 
            this.SelLibPath.BackColor = System.Drawing.Color.DimGray;
            this.SelLibPath.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SelLibPath.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.SelLibPath.FlatAppearance.BorderSize = 0;
            this.SelLibPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SelLibPath.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.SelLibPath.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.SelLibPath.Location = new System.Drawing.Point(4, 4);
            this.SelLibPath.Margin = new System.Windows.Forms.Padding(0);
            this.SelLibPath.Name = "SelLibPath";
            this.SelLibPath.Size = new System.Drawing.Size(49, 20);
            this.SelLibPath.TabIndex = 0;
            this.SelLibPath.Text = "库路径";
            this.SelLibPath.UseVisualStyleBackColor = false;
            this.SelLibPath.Click += new System.EventHandler(this.SelLibPath_Click);
            // 
            // libPath
            // 
            this.libPath.AutoSize = true;
            this.libPath.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.libPath.Location = new System.Drawing.Point(56, 8);
            this.libPath.Name = "libPath";
            this.libPath.Size = new System.Drawing.Size(29, 12);
            this.libPath.TabIndex = 1;
            this.libPath.Text = "NULL";
            // 
            // panelTab
            // 
            this.panelTab.Controls.Add(this.layerPanel);
            this.panelTab.Controls.Add(this.hierPanel);
            this.panelTab.Controls.Add(this.ctxPanel);
            this.panelTab.Controls.Add(this.scriptPanel);
            this.panelTab.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.panelTab.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panelTab.Location = new System.Drawing.Point(0, 32);
            this.panelTab.Margin = new System.Windows.Forms.Padding(0);
            this.panelTab.Name = "panelTab";
            this.panelTab.Padding = new System.Drawing.Point(0, 0);
            this.panelTab.SelectedIndex = 0;
            this.panelTab.Size = new System.Drawing.Size(497, 456);
            this.panelTab.TabIndex = 1;
            this.panelTab.TabStop = false;
            this.panelTab.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.panelTab_DrawItem);
            // 
            // layerPanel
            // 
            this.layerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.layerPanel.Controls.Add(this.layerList);
            this.layerPanel.Location = new System.Drawing.Point(4, 22);
            this.layerPanel.Name = "layerPanel";
            this.layerPanel.Padding = new System.Windows.Forms.Padding(3);
            this.layerPanel.Size = new System.Drawing.Size(489, 430);
            this.layerPanel.TabIndex = 0;
            this.layerPanel.Text = "图层";
            // 
            // layerList
            // 
            this.layerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.layerList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.layerList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.layerList.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.layerList.FullRowSelect = true;
            this.layerList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.layerList.Location = new System.Drawing.Point(0, 0);
            this.layerList.MultiSelect = false;
            this.layerList.Name = "layerList";
            this.layerList.Size = new System.Drawing.Size(438, 195);
            this.layerList.TabIndex = 1;
            this.layerList.UseCompatibleStateImageBehavior = false;
            this.layerList.View = System.Windows.Forms.View.Details;
            this.layerList.DoubleClick += new System.EventHandler(this.layerList_DoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Width = 400;
            // 
            // hierPanel
            // 
            this.hierPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.hierPanel.Controls.Add(this.hierList);
            this.hierPanel.Location = new System.Drawing.Point(4, 22);
            this.hierPanel.Name = "hierPanel";
            this.hierPanel.Padding = new System.Windows.Forms.Padding(3);
            this.hierPanel.Size = new System.Drawing.Size(489, 430);
            this.hierPanel.TabIndex = 1;
            this.hierPanel.Text = "层次";
            // 
            // hierList
            // 
            this.hierList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.hierList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hierList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.hierList.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.hierList.FullRowSelect = true;
            this.hierList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.hierList.Location = new System.Drawing.Point(0, 0);
            this.hierList.MultiSelect = false;
            this.hierList.Name = "hierList";
            this.hierList.Scrollable = false;
            this.hierList.Size = new System.Drawing.Size(476, 152);
            this.hierList.TabIndex = 0;
            this.hierList.UseCompatibleStateImageBehavior = false;
            this.hierList.View = System.Windows.Forms.View.Details;
            this.hierList.DoubleClick += new System.EventHandler(this.hierList_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 400;
            // 
            // ctxPanel
            // 
            this.ctxPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.ctxPanel.Controls.Add(this.ctxList);
            this.ctxPanel.Location = new System.Drawing.Point(4, 22);
            this.ctxPanel.Name = "ctxPanel";
            this.ctxPanel.Size = new System.Drawing.Size(489, 430);
            this.ctxPanel.TabIndex = 2;
            this.ctxPanel.Text = "内容";
            // 
            // ctxList
            // 
            this.ctxList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.ctxList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ctxList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
            this.ctxList.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.ctxList.FullRowSelect = true;
            this.ctxList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.ctxList.Location = new System.Drawing.Point(0, 0);
            this.ctxList.MultiSelect = false;
            this.ctxList.Name = "ctxList";
            this.ctxList.Scrollable = false;
            this.ctxList.Size = new System.Drawing.Size(487, 152);
            this.ctxList.TabIndex = 1;
            this.ctxList.UseCompatibleStateImageBehavior = false;
            this.ctxList.View = System.Windows.Forms.View.Details;
            this.ctxList.DoubleClick += new System.EventHandler(this.ctxList_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Width = 400;
            // 
            // scriptPanel
            // 
            this.scriptPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.scriptPanel.Controls.Add(this.scriptList);
            this.scriptPanel.Location = new System.Drawing.Point(4, 22);
            this.scriptPanel.Name = "scriptPanel";
            this.scriptPanel.Size = new System.Drawing.Size(489, 430);
            this.scriptPanel.TabIndex = 3;
            this.scriptPanel.Text = "脚本";
            // 
            // scriptList
            // 
            this.scriptList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.scriptList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.scriptList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.scriptList.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.scriptList.FullRowSelect = true;
            this.scriptList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.scriptList.Location = new System.Drawing.Point(0, 0);
            this.scriptList.MultiSelect = false;
            this.scriptList.Name = "scriptList";
            this.scriptList.Scrollable = false;
            this.scriptList.Size = new System.Drawing.Size(377, 268);
            this.scriptList.TabIndex = 1;
            this.scriptList.UseCompatibleStateImageBehavior = false;
            this.scriptList.View = System.Windows.Forms.View.Details;
            this.scriptList.DoubleClick += new System.EventHandler(this.scriptList_DoubleClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Width = 400;
            // 
            // ComponentLibary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.Controls.Add(this.panelTab);
            this.Controls.Add(this.libPath);
            this.Controls.Add(this.SelLibPath);
            this.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.Name = "ComponentLibary";
            this.Size = new System.Drawing.Size(497, 488);
            this.Resize += new System.EventHandler(this.ComponentLibary_Resize);
            this.panelTab.ResumeLayout(false);
            this.layerPanel.ResumeLayout(false);
            this.hierPanel.ResumeLayout(false);
            this.ctxPanel.ResumeLayout(false);
            this.scriptPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelLibPath;
        private System.Windows.Forms.Label libPath;
        private System.Windows.Forms.TabControl panelTab;
        private System.Windows.Forms.TabPage layerPanel;
        private System.Windows.Forms.TabPage hierPanel;
        private System.Windows.Forms.TabPage ctxPanel;
        private System.Windows.Forms.TabPage scriptPanel;
        private System.Windows.Forms.ListView hierList;
        internal System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ListView layerList;
        internal System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ListView ctxList;
        internal System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ListView scriptList;
        internal System.Windows.Forms.ColumnHeader columnHeader4;
    }
}
