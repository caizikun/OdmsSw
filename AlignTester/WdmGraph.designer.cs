namespace AlignTester
{
    partial class WdmGraph
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.mGraph = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cbSeriesList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnLoad = new System.Windows.Forms.ToolStripSplitButton();
            this.cbLoadOption = new System.Windows.Forms.ToolStripComboBox();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.btnCapture = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnLegends = new System.Windows.Forms.ToolStripDropDownButton();
            this.chkLegends = new System.Windows.Forms.ToolStripMenuItem();
            this.chkPeak = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPolsOption = new System.Windows.Forms.ToolStripDropDownButton();
            this.chkOptionPlotPol = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mGraph)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mGraph
            // 
            chartArea1.Name = "ChartArea1";
            this.mGraph.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.mGraph.Legends.Add(legend1);
            this.mGraph.Location = new System.Drawing.Point(78, 55);
            this.mGraph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.mGraph.Name = "mGraph";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.mGraph.Series.Add(series1);
            this.mGraph.Size = new System.Drawing.Size(437, 197);
            this.mGraph.TabIndex = 0;
            this.mGraph.Text = "chart1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 58);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 58);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 58);
            // 
            // cbSeriesList
            // 
            this.cbSeriesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSeriesList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbSeriesList.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold);
            this.cbSeriesList.Items.AddRange(new object[] {
            "*"});
            this.cbSeriesList.Name = "cbSeriesList";
            this.cbSeriesList.Size = new System.Drawing.Size(80, 58);
            this.cbSeriesList.Sorted = true;
            this.cbSeriesList.ToolTipText = "Single Plot";
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoad,
            this.btnReset,
            this.toolStripSeparator1,
            this.btnCapture,
            this.btnZoomOut,
            this.toolStripSeparator2,
            this.btnLegends,
            this.btnPolsOption,
            this.toolStripSeparator3,
            this.cbSeriesList});
            this.toolStrip.Location = new System.Drawing.Point(78, 272);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(462, 58);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnLoad
            // 
            this.btnLoad.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cbLoadOption});
            this.btnLoad.Image = global::AlignTester.Properties.Resources.openData;
            this.btnLoad.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(52, 55);
            this.btnLoad.Text = "Load";
            this.btnLoad.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // cbLoadOption
            // 
            this.cbLoadOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoadOption.Name = "cbLoadOption";
            this.cbLoadOption.Size = new System.Drawing.Size(121, 23);
            // 
            // btnReset
            // 
            this.btnReset.Image = global::AlignTester.Properties.Resources.replay;
            this.btnReset.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(47, 55);
            this.btnReset.Text = "초기화";
            this.btnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // btnCapture
            // 
            this.btnCapture.Image = global::AlignTester.Properties.Resources.camera;
            this.btnCapture.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(53, 55);
            this.btnCapture.Text = "Capture";
            this.btnCapture.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnCapture.ToolTipText = "Chart image capture to clipboard";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Image = global::AlignTester.Properties.Resources.zoom_out;
            this.btnZoomOut.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(67, 55);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnZoomOut.ToolTipText = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click_1);
            // 
            // btnLegends
            // 
            this.btnLegends.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkLegends,
            this.chkPeak});
            this.btnLegends.Image = global::AlignTester.Properties.Resources.assignment;
            this.btnLegends.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLegends.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLegends.Name = "btnLegends";
            this.btnLegends.Size = new System.Drawing.Size(49, 55);
            this.btnLegends.Text = "범례";
            this.btnLegends.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnLegends.ToolTipText = "Choose legends Type";
            // 
            // chkLegends
            // 
            this.chkLegends.Checked = true;
            this.chkLegends.CheckOnClick = true;
            this.chkLegends.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLegends.Name = "chkLegends";
            this.chkLegends.Size = new System.Drawing.Size(180, 22);
            this.chkLegends.Text = "범례표시";
            // 
            // chkPeak
            // 
            this.chkPeak.CheckOnClick = true;
            this.chkPeak.Name = "chkPeak";
            this.chkPeak.Size = new System.Drawing.Size(180, 22);
            this.chkPeak.Text = "Peak 표시";
            // 
            // btnPolsOption
            // 
            this.btnPolsOption.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkOptionPlotPol});
            this.btnPolsOption.Image = global::AlignTester.Properties.Resources.check;
            this.btnPolsOption.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPolsOption.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPolsOption.Name = "btnPolsOption";
            this.btnPolsOption.Size = new System.Drawing.Size(49, 55);
            this.btnPolsOption.Text = "편광";
            this.btnPolsOption.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPolsOption.ToolTipText = "Choose Plot Pols";
            // 
            // chkOptionPlotPol
            // 
            this.chkOptionPlotPol.CheckOnClick = true;
            this.chkOptionPlotPol.Name = "chkOptionPlotPol";
            this.chkOptionPlotPol.Size = new System.Drawing.Size(126, 22);
            this.chkOptionPlotPol.Text = "편광 표시";
            // 
            // WdmGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mGraph);
            this.Controls.Add(this.toolStrip);
            this.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "WdmGraph";
            this.Size = new System.Drawing.Size(751, 405);
            ((System.ComponentModel.ISupportInitialize)(this.mGraph)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataVisualization.Charting.Chart mGraph;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCapture;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton btnLegends;
        private System.Windows.Forms.ToolStripMenuItem chkPeak;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox cbSeriesList;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripMenuItem chkLegends;
        private System.Windows.Forms.ToolStripSplitButton btnLoad;
        private System.Windows.Forms.ToolStripDropDownButton btnPolsOption;
        private System.Windows.Forms.ToolStripComboBox cbLoadOption;
        private System.Windows.Forms.ToolStripMenuItem chkOptionPlotPol;
    }
}
