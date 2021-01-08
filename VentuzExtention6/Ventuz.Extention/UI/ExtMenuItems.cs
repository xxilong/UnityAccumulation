using System;
using System.Windows.Forms;
using Ventuz.Extention.Detector;
using Ventuz.Extention.Marker;
using Ventuz.Extention.MatLib;
using Ventuz.Extention.Conf;

namespace Ventuz.Extention.UI
{
    class ExtMenuItems
    {
        public static ExtMenuItems Instance = new ExtMenuItems();

        #region 触摸事件录制菜单
        private ExtMenuItems()
        {
            pauseContinue.Text = "暂停/继续";
            pauseContinue.Enabled = false;
            pauseContinue.Click += (object sender, EventArgs e) => TouchPlayer.player.PausePlay();

            singleStep.Text = "单步(PageDown)";
            singleStep.Enabled = false;
            singleStep.Click += (object sender, EventArgs e) => TouchPlayer.player.PlayStep();

            InitMarkerMenus();
        }

        public ToolStripItem[] GetRecorderMenus()
        {
            return new ToolStripItem[]
            {
                new ToolStripMenuItem("开始录制触摸事件", null, (object sender, EventArgs e) => TouchToFile.toFile.Start()),
                new ToolStripMenuItem("停止录制", null, (object sender, EventArgs e) => TouchToFile.toFile.Stop()),
                new ToolStripMenuItem("播放录制的触摸事件", null, (object sender, EventArgs e) => TouchPlayer.player.PlayFile()),
                pauseContinue,
                singleStep,
            };
        }
        #endregion

        #region Marker 菜单
        public ToolStripItem[] GetMarkerMenus()
        {
            return new ToolStripItem[]{ setupScreen, setupMarker, modifyMarker };
        }
        
        private void InitMarkerMenus()
        {
            // Marker 配置菜单         
            setupScreen.Text = "触摸屏配置";
            setupScreen.Click += delegate (object sender, EventArgs e)
            {
                var scn = new ScreenSetting();
                scn.TopMost = true;
                scn.Show();
            };

            setupMarker.Text = "添加 Marker";
            setupMarker.Click += delegate (object sender, EventArgs e)
            {
                var marker = new AddMarker(MarkerConfigManager.instance.NewConfig());
                marker.TopMost = true;
                marker.StartPosition = FormStartPosition.CenterScreen;
                marker.Show();
                marker.FormClosed += delegate (object s, FormClosedEventArgs fe)
                {
                    ResetMarkerMenu();
                };
            };

            modifyMarker.Text = "修改 Marker";
            ResetMarkerMenu();
        }

        private void ResetMarkerMenu()
        {
            modifyMarker.DropDownItems.Clear();

            foreach (var cfg in MarkerConfigManager.instance.GetAllConfigs())
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = cfg.name;
                item.Click += delegate (object sender, EventArgs e)
                {
                    var marker = new AddMarker(cfg);
                    marker.TopMost = true;
                    marker.StartPosition = FormStartPosition.CenterScreen;
                    marker.Show();

                    marker.FormClosed += delegate (object s, FormClosedEventArgs fe)
                    {
                        ResetMarkerMenu();
                    };
                };

                modifyMarker.DropDownItems.Add(item);
            }
        }

        #endregion

        #region 导入导出菜单
        public ToolStripItem[] GetArchiveMenu()
        {
            // 导出, 导入
            ToolStripMenuItem exportMenu = new ToolStripMenuItem("导出");

            exportMenu.DropDownItems.AddRange(new ToolStripItem[]
            {
                new ToolStripMenuItem("选中的 Layers", null,  NodeExportor.Instance.ExportLayers),
                new ToolStripSeparator(),
                new ToolStripMenuItem("选中的 Hierarchy Nodes", null,  NodeExportor.Instance.ExportHierarchyNodes),
                new ToolStripMenuItem("单个 Hierarchy Node(Base64)", null, NodeExportor.Instance.ExportHierarchyNodesBase64),
                new ToolStripSeparator(),

                new ToolStripMenuItem("选中的内容节点(仅选中)", null,  NodeExportor.Instance.ExportContentNodeOnlySelect),
                new ToolStripMenuItem("选中的内容节点(含输入依赖节点)", null,  NodeExportor.Instance.ExportContentNodeWithInput),
                new ToolStripMenuItem("选中的内容节点(默认选择规则)", null,  NodeExportor.Instance.ExportContentNodesDefault),
            });

            return new ToolStripItem[] {
                exportMenu,
                new ToolStripMenuItem("导入...", null, NodeImportor.Instance.ImportArchive),
                new ToolStripMenuItem("搜索...", null, NodeSearcher.Instance.OpenSearchDialog),
                new ToolStripMenuItem("导出场景节点树", null, NodeSearcher.Instance.ExportNodeTree),
            };
        }
        #endregion

        #region 杂项菜单
        public ToolStripItem[] GetMiscMenu()
        {
            ToolStripMenuItem mipFilt = new ToolStripMenuItem("强制采样模式", null, (object o, EventArgs e) =>
            {
                ToolStripMenuItem menu = o as ToolStripMenuItem;
                ExtentionConf.Instance.ForceModifySimple = !ExtentionConf.Instance.ForceModifySimple;
                menu.Checked = ExtentionConf.Instance.ForceModifySimple;
            });

            mipFilt.Checked = ExtentionConf.Instance.ForceModifySimple;

            return new ToolStripItem[]
            {
                mipFilt,
            };
        }

        #endregion

        public ToolStripMenuItem pauseContinue = new ToolStripMenuItem();
        public ToolStripMenuItem singleStep = new ToolStripMenuItem();

        private ToolStripMenuItem setupScreen = new ToolStripMenuItem();
        private ToolStripMenuItem setupMarker = new ToolStripMenuItem();
        private ToolStripMenuItem modifyMarker = new ToolStripMenuItem();
    }
}
