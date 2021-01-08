using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Ventuz.Extention.Detector;
using Ventuz.Extention.Marker;

namespace Ventuz.Extention.UI
{
    public partial class AddMarker : Form
    {
        private const double MaxDistance = 99999999999999999999.99;
        public AddMarker(MarkerConfig cfg)
        {
            config = cfg;

            if(config.dis12 > 0)
            {
                mindis12 = config.dis12 - config.erdis12;
                maxdis12 = config.dis12 + config.erdis12;
            }

            if(config.dis13 > 0)
            {
                mindis13 = config.dis13 - config.erdis13;
                maxdis13 = config.dis13 + config.erdis13;
            }

            if(config.dis23 > 0)
            {
                mindis23 = config.dis23 - config.erdis23;
                maxdis23 = config.dis23 + config.erdis23;
            }

            InitializeComponent();
            UpdateConfigToUI();
        }

        private MarkerConfig config;
        private double mindis12 = MaxDistance;
        private double maxdis12 = 0.0;
        private double mindis23 = MaxDistance;
        private double maxdis23 = 0.0;
        private double mindis13 = MaxDistance;
        private double maxdis13 = 0.0;

        private void checkMarker_Click(object sender, EventArgs e)
        {
            var res = TouchStatus.touchs.GetNearest3PtDistance();
            if(res.Length != 3)
            {
                errNode.Text = "未检测到 Marker, 可能是当前触摸点不足 3 个";
                return;
            }

            double dis12 = res[2].dis;
            double dis23 = res[1].dis;
            double dis13 = res[0].dis;

            if(dis12 < mindis12)
            {
                mindis12 = dis12;
            }

            if(dis12 > maxdis12)
            {
                maxdis12 = dis12;
            }

            if(dis13 < mindis13)
            {
                mindis13 = dis13;
            }

            if(dis13 > maxdis13)
            {
                maxdis13 = dis13;
            }

            if(dis23 < mindis23)
            {
                mindis23 = dis23;
            }

            if(dis23 > maxdis23)
            {
                maxdis23 = dis23;
            }

            config.dis12 = (maxdis12 + mindis12) / 2.0;
            config.erdis12 = (maxdis12 - mindis12) / 2.0;

            config.dis13 = (maxdis13 + mindis13) / 2.0;
            config.erdis13 = (maxdis13 - mindis13) / 2.0;

            config.dis23 = (maxdis23 + mindis23) / 2.0;
            config.erdis23 = (maxdis23 - mindis23) / 2.0;

            LogicTouchPos p1, p2, p3;
            p1 = res[2].pt1;
            p2 = res[2].pt2;

            if(p1 != res[0].pt1 && p1 != res[0].pt2)
            {
                p1 = res[2].pt2;
                p2 = res[2].pt1;
            }

            if(p1 == res[0].pt1)
            {
                p3 = res[0].pt2;
            }
            else
            {
                p3 = res[0].pt1;
            }

            Debug.Assert((p2 == res[1].pt1 && p3 == res[1].pt2) || (p2 == res[1].pt2 && p3 == res[1].pt1));

            config.direction = MathTools.side_of_pt3(p1, p2, p3) > 0;
            config.pbuilder = null;

            UpdateConfigToUI();            
            errNode.Text = "";

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void UpdateConfigToUI()
        {
            Dis12.Text = config.dis12.ToString();
            Dis13.Text = config.dis13.ToString();
            Dis23.Text = config.dis23.ToString();

            Err12.Text = config.erdis12.ToString("G");
            Err13.Text = config.erdis13.ToString("G");
            Err23.Text = config.erdis23.ToString("G");

            CenterOffX.Text = config.centerx.ToString();
            CenterOffY.Text = config.centery.ToString();

            if(config.dis12 <= 0)
            {
                CenterXTracker.Value = 0;
                CenterYTracker.Value = 0;
            }
            else
            {
                CenterXTracker.Value = config.cxRate;
                CenterYTracker.Value = config.cyRate;
            }

            RadiusText.Text = config.radius.ToString("G");
            RadiusTracker.Value = config.radiusRate;

            markerName.Text = config.name;
            removeTouchs.Checked = config.rmtouchs;
            direction.Text = config.direction ? "顺时针" : "逆时针";

            int st = config.valid(out string msg);
            switch(st)
            {
                case 0:
                    validMessage.Text = "无效: " + msg;
                    validMessage.ForeColor = Color.Red;
                    break;

                case 1:
                    validMessage.Text = "有效";
                    validMessage.ForeColor = Color.Green;
                    break;

                case -1:
                    validMessage.Text = "警告: " + msg;
                    validMessage.ForeColor = Color.Pink;
                    break;
            }

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void CenterXTracker_Scroll(object sender, EventArgs e)
        {
            config.cxRate = CenterXTracker.Value;
            CenterOffX.Text = config.centerx.ToString();

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void CenterYTracker_Scroll(object sender, EventArgs e)
        {
            config.cyRate = CenterYTracker.Value;
            CenterOffY.Text = config.centery.ToString();

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void markerName_TextChanged(object sender, EventArgs e)
        {
            config.name = markerName.Text;

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void removeTouchs_CheckedChanged(object sender, EventArgs e)
        {
            config.rmtouchs = removeTouchs.Checked;

            MarkerConfigManager.instance.ModifyedConfig();
        }

        private void AddMarker_FormClosed(object sender, FormClosedEventArgs e)
        {
            MarkerConfigManager.instance.SaveConfigs();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            config.Reset();

            mindis12 = MaxDistance;
            maxdis12 = 0.0;
            mindis23 = MaxDistance;
            maxdis23 = 0.0;
            mindis13 = MaxDistance;
            maxdis13 = 0.0;

            UpdateConfigToUI();
        }

        private void RadiusTracker_Scroll(object sender, EventArgs e)
        {
            config.radiusRate = RadiusTracker.Value;
            RadiusText.Text = config.radius.ToString();

            MarkerConfigManager.instance.ModifyedConfig();
        }
    }
}
