namespace Ventuz.Extention.UI
{
    partial class AddMarker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddMarker));
            this.checkMarker = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Dis12 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Dis23 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Dis13 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.CenterOffX = new System.Windows.Forms.TextBox();
            this.CenterXTracker = new System.Windows.Forms.TrackBar();
            this.label6 = new System.Windows.Forms.Label();
            this.CenterOffY = new System.Windows.Forms.TextBox();
            this.CenterYTracker = new System.Windows.Forms.TrackBar();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Err12 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.markerName = new System.Windows.Forms.TextBox();
            this.errNode = new System.Windows.Forms.Label();
            this.removeTouchs = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.Err23 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.Err13 = new System.Windows.Forms.TextBox();
            this.validMessage = new System.Windows.Forms.Label();
            this.ResetButton = new System.Windows.Forms.Button();
            this.label_direction = new System.Windows.Forms.Label();
            this.direction = new System.Windows.Forms.TextBox();
            this.RadiusLable = new System.Windows.Forms.Label();
            this.RadiusText = new System.Windows.Forms.TextBox();
            this.RadiusTracker = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.CenterXTracker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CenterYTracker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadiusTracker)).BeginInit();
            this.SuspendLayout();
            // 
            // checkMarker
            // 
            this.checkMarker.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.checkMarker.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkMarker.Location = new System.Drawing.Point(342, 21);
            this.checkMarker.Name = "checkMarker";
            this.checkMarker.Size = new System.Drawing.Size(75, 30);
            this.checkMarker.TabIndex = 0;
            this.checkMarker.Text = "检测";
            this.checkMarker.UseVisualStyleBackColor = false;
            this.checkMarker.Click += new System.EventHandler(this.checkMarker_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(54, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(269, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "请将 Marker 放置到触摸屏上, 然后点击检测按钮";
            // 
            // Dis12
            // 
            this.Dis12.BackColor = System.Drawing.Color.Gainsboro;
            this.Dis12.Enabled = false;
            this.Dis12.Location = new System.Drawing.Point(143, 66);
            this.Dis12.Name = "Dis12";
            this.Dis12.Size = new System.Drawing.Size(171, 21);
            this.Dis12.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label2.Location = new System.Drawing.Point(32, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "长边距离(P1-P2):";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label3.Location = new System.Drawing.Point(20, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "次长边距离(P2-P3):";
            // 
            // Dis23
            // 
            this.Dis23.BackColor = System.Drawing.Color.Gainsboro;
            this.Dis23.Enabled = false;
            this.Dis23.Location = new System.Drawing.Point(143, 98);
            this.Dis23.Name = "Dis23";
            this.Dis23.Size = new System.Drawing.Size(171, 21);
            this.Dis23.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label4.Location = new System.Drawing.Point(32, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "短边距离(P1-P3):";
            // 
            // Dis13
            // 
            this.Dis13.BackColor = System.Drawing.Color.Gainsboro;
            this.Dis13.Enabled = false;
            this.Dis13.Location = new System.Drawing.Point(143, 130);
            this.Dis13.Name = "Dis13";
            this.Dis13.Size = new System.Drawing.Size(171, 21);
            this.Dis13.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label5.Location = new System.Drawing.Point(36, 241);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "中心点偏移X:";
            // 
            // CenterOffX
            // 
            this.CenterOffX.BackColor = System.Drawing.Color.Gainsboro;
            this.CenterOffX.Enabled = false;
            this.CenterOffX.Location = new System.Drawing.Point(139, 235);
            this.CenterOffX.Name = "CenterOffX";
            this.CenterOffX.Size = new System.Drawing.Size(100, 21);
            this.CenterOffX.TabIndex = 2;
            // 
            // CenterXTracker
            // 
            this.CenterXTracker.Location = new System.Drawing.Point(245, 233);
            this.CenterXTracker.Maximum = 400;
            this.CenterXTracker.Name = "CenterXTracker";
            this.CenterXTracker.Size = new System.Drawing.Size(248, 45);
            this.CenterXTracker.TabIndex = 3;
            this.CenterXTracker.Scroll += new System.EventHandler(this.CenterXTracker_Scroll);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label6.Location = new System.Drawing.Point(36, 291);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "中心点偏移Y:";
            // 
            // CenterOffY
            // 
            this.CenterOffY.BackColor = System.Drawing.Color.Gainsboro;
            this.CenterOffY.Enabled = false;
            this.CenterOffY.Location = new System.Drawing.Point(139, 285);
            this.CenterOffY.Name = "CenterOffY";
            this.CenterOffY.Size = new System.Drawing.Size(100, 21);
            this.CenterOffY.TabIndex = 2;
            // 
            // CenterYTracker
            // 
            this.CenterYTracker.Location = new System.Drawing.Point(245, 283);
            this.CenterYTracker.Maximum = 400;
            this.CenterYTracker.Name = "CenterYTracker";
            this.CenterYTracker.Size = new System.Drawing.Size(248, 45);
            this.CenterYTracker.TabIndex = 3;
            this.CenterYTracker.Scroll += new System.EventHandler(this.CenterYTracker_Scroll);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label7.Location = new System.Drawing.Point(78, 39);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(245, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "多次移动 Marker 并点击检测来确定误差范围";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label8.Location = new System.Drawing.Point(324, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "误差:";
            // 
            // Err12
            // 
            this.Err12.BackColor = System.Drawing.Color.Gainsboro;
            this.Err12.Enabled = false;
            this.Err12.Location = new System.Drawing.Point(367, 65);
            this.Err12.Name = "Err12";
            this.Err12.Size = new System.Drawing.Size(126, 21);
            this.Err12.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label9.Location = new System.Drawing.Point(38, 395);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 12);
            this.label9.TabIndex = 1;
            this.label9.Text = "Marker 名称:";
            // 
            // markerName
            // 
            this.markerName.BackColor = System.Drawing.Color.Gainsboro;
            this.markerName.Location = new System.Drawing.Point(121, 390);
            this.markerName.Name = "markerName";
            this.markerName.Size = new System.Drawing.Size(171, 21);
            this.markerName.TabIndex = 2;
            this.markerName.TextChanged += new System.EventHandler(this.markerName_TextChanged);
            // 
            // errNode
            // 
            this.errNode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errNode.ForeColor = System.Drawing.Color.Red;
            this.errNode.Location = new System.Drawing.Point(12, 191);
            this.errNode.Name = "errNode";
            this.errNode.Size = new System.Drawing.Size(494, 76);
            this.errNode.TabIndex = 1;
            this.errNode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // removeTouchs
            // 
            this.removeTouchs.AutoSize = true;
            this.removeTouchs.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.removeTouchs.Location = new System.Drawing.Point(311, 393);
            this.removeTouchs.Name = "removeTouchs";
            this.removeTouchs.Size = new System.Drawing.Size(168, 16);
            this.removeTouchs.TabIndex = 4;
            this.removeTouchs.Text = "删除 Marker 周围的触摸点";
            this.removeTouchs.UseVisualStyleBackColor = true;
            this.removeTouchs.CheckedChanged += new System.EventHandler(this.removeTouchs_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label10.Location = new System.Drawing.Point(324, 103);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 1;
            this.label10.Text = "误差:";
            // 
            // Err23
            // 
            this.Err23.BackColor = System.Drawing.Color.Gainsboro;
            this.Err23.Enabled = false;
            this.Err23.Location = new System.Drawing.Point(367, 98);
            this.Err23.Name = "Err23";
            this.Err23.Size = new System.Drawing.Size(126, 21);
            this.Err23.TabIndex = 2;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label11.Location = new System.Drawing.Point(324, 135);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 1;
            this.label11.Text = "误差:";
            // 
            // Err13
            // 
            this.Err13.BackColor = System.Drawing.Color.Gainsboro;
            this.Err13.Enabled = false;
            this.Err13.Location = new System.Drawing.Point(367, 130);
            this.Err13.Name = "Err13";
            this.Err13.Size = new System.Drawing.Size(126, 21);
            this.Err13.TabIndex = 2;
            // 
            // validMessage
            // 
            this.validMessage.AutoSize = true;
            this.validMessage.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.validMessage.ForeColor = System.Drawing.Color.Red;
            this.validMessage.Location = new System.Drawing.Point(51, 429);
            this.validMessage.Name = "validMessage";
            this.validMessage.Size = new System.Drawing.Size(35, 14);
            this.validMessage.TabIndex = 5;
            this.validMessage.Text = "无效";
            // 
            // ResetButton
            // 
            this.ResetButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ResetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ResetButton.Location = new System.Drawing.Point(432, 21);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(61, 30);
            this.ResetButton.TabIndex = 0;
            this.ResetButton.Text = "重置";
            this.ResetButton.UseVisualStyleBackColor = false;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // label_direction
            // 
            this.label_direction.AutoSize = true;
            this.label_direction.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label_direction.Location = new System.Drawing.Point(98, 163);
            this.label_direction.Name = "label_direction";
            this.label_direction.Size = new System.Drawing.Size(35, 12);
            this.label_direction.TabIndex = 1;
            this.label_direction.Text = "方向:";
            // 
            // direction
            // 
            this.direction.BackColor = System.Drawing.Color.Gainsboro;
            this.direction.Enabled = false;
            this.direction.Location = new System.Drawing.Point(142, 159);
            this.direction.Name = "direction";
            this.direction.Size = new System.Drawing.Size(171, 21);
            this.direction.TabIndex = 2;
            // 
            // RadiusLable
            // 
            this.RadiusLable.AutoSize = true;
            this.RadiusLable.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.RadiusLable.Location = new System.Drawing.Point(52, 342);
            this.RadiusLable.Name = "RadiusLable";
            this.RadiusLable.Size = new System.Drawing.Size(59, 12);
            this.RadiusLable.TabIndex = 1;
            this.RadiusLable.Text = "半径大小:";
            // 
            // RadiusText
            // 
            this.RadiusText.BackColor = System.Drawing.Color.Gainsboro;
            this.RadiusText.Enabled = false;
            this.RadiusText.Location = new System.Drawing.Point(138, 336);
            this.RadiusText.Name = "RadiusText";
            this.RadiusText.Size = new System.Drawing.Size(100, 21);
            this.RadiusText.TabIndex = 2;
            // 
            // RadiusTracker
            // 
            this.RadiusTracker.Location = new System.Drawing.Point(245, 334);
            this.RadiusTracker.Maximum = 400;
            this.RadiusTracker.Name = "RadiusTracker";
            this.RadiusTracker.Size = new System.Drawing.Size(248, 45);
            this.RadiusTracker.TabIndex = 3;
            this.RadiusTracker.Scroll += new System.EventHandler(this.RadiusTracker_Scroll);
            // 
            // AddMarker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(518, 463);
            this.Controls.Add(this.validMessage);
            this.Controls.Add(this.removeTouchs);
            this.Controls.Add(this.RadiusTracker);
            this.Controls.Add(this.CenterYTracker);
            this.Controls.Add(this.RadiusText);
            this.Controls.Add(this.CenterXTracker);
            this.Controls.Add(this.RadiusLable);
            this.Controls.Add(this.CenterOffY);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.CenterOffX);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.markerName);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Err13);
            this.Controls.Add(this.Err23);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.Err12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.errNode);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.direction);
            this.Controls.Add(this.label_direction);
            this.Controls.Add(this.Dis13);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Dis23);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Dis12);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.checkMarker);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AddMarker";
            this.Text = "添加/修改 Marker";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddMarker_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.CenterXTracker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CenterYTracker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RadiusTracker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button checkMarker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Dis12;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Dis23;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Dis13;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox CenterOffX;
        private System.Windows.Forms.TrackBar CenterXTracker;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox CenterOffY;
        private System.Windows.Forms.TrackBar CenterYTracker;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Err12;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox markerName;
        private System.Windows.Forms.Label errNode;
        private System.Windows.Forms.CheckBox removeTouchs;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox Err23;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox Err13;
        private System.Windows.Forms.Label validMessage;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label label_direction;
        private System.Windows.Forms.TextBox direction;
        private System.Windows.Forms.Label RadiusLable;
        private System.Windows.Forms.TextBox RadiusText;
        private System.Windows.Forms.TrackBar RadiusTracker;
    }
}