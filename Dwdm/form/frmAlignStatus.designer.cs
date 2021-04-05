
partial class frmAlignStatus
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
            this.lbMaxUnit = new System.Windows.Forms.Label();
            this.lbCurVal = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.lbStage = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.lbCmdName = new System.Windows.Forms.Label();
            this.lbCurUnit = new System.Windows.Forms.Label();
            this.wfgGraph = new NationalInstruments.UI.WindowsForms.WaveformGraph();
            this.waveformPlot1 = new NationalInstruments.UI.WaveformPlot();
            this.xAxis1 = new NationalInstruments.UI.XAxis();
            this.yAxis1 = new NationalInstruments.UI.YAxis();
            this.waveformPlot2 = new NationalInstruments.UI.WaveformPlot();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbLeadTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.wfgGraph)).BeginInit();
            this.SuspendLayout();
            // 
            // lbMaxUnit
            // 
            this.lbMaxUnit.AutoSize = true;
            this.lbMaxUnit.Font = new System.Drawing.Font("Segoe UI Symbol", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxUnit.Location = new System.Drawing.Point(259, 157);
            this.lbMaxUnit.Name = "lbMaxUnit";
            this.lbMaxUnit.Size = new System.Drawing.Size(47, 11);
            this.lbMaxUnit.TabIndex = 134;
            this.lbMaxUnit.Text = "[dBm] or [V]";
            // 
            // lbCurVal
            // 
            this.lbCurVal.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurVal.Location = new System.Drawing.Point(43, 155);
            this.lbCurVal.Name = "lbCurVal";
            this.lbCurVal.Size = new System.Drawing.Size(39, 13);
            this.lbCurVal.TabIndex = 132;
            this.lbCurVal.Text = "0.01";
            this.lbCurVal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(218, 155);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(39, 13);
            this.lbMaxValue.TabIndex = 131;
            this.lbMaxValue.Text = "0.01";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(176, 155);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(34, 13);
            this.Label6.TabIndex = 130;
            this.Label6.Text = "Max .";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(9, 155);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(28, 13);
            this.Label3.TabIndex = 129;
            this.Label3.Text = "Cur.";
            // 
            // lbStage
            // 
            this.lbStage.AutoSize = true;
            this.lbStage.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStage.Location = new System.Drawing.Point(67, 19);
            this.lbStage.Name = "lbStage";
            this.lbStage.Size = new System.Drawing.Size(29, 13);
            this.lbStage.TabIndex = 128;
            this.lbStage.Text = "LEFT";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(16, 19);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(45, 13);
            this.Label2.TabIndex = 127;
            this.Label2.Text = "STAGE :";
            // 
            // lbCmdName
            // 
            this.lbCmdName.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCmdName.Location = new System.Drawing.Point(6, 4);
            this.lbCmdName.Name = "lbCmdName";
            this.lbCmdName.Size = new System.Drawing.Size(298, 15);
            this.lbCmdName.TabIndex = 126;
            this.lbCmdName.Text = "...";
            this.lbCmdName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbCurUnit
            // 
            this.lbCurUnit.AutoSize = true;
            this.lbCurUnit.Font = new System.Drawing.Font("Segoe UI Symbol", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurUnit.Location = new System.Drawing.Point(88, 157);
            this.lbCurUnit.Name = "lbCurUnit";
            this.lbCurUnit.Size = new System.Drawing.Size(47, 11);
            this.lbCurUnit.TabIndex = 135;
            this.lbCurUnit.Text = "[dBm] or [V]";
            // 
            // wfgGraph
            // 
            this.wfgGraph.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wfgGraph.Font = new System.Drawing.Font("Segoe UI Symbol", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wfgGraph.Location = new System.Drawing.Point(11, 33);
            this.wfgGraph.Name = "wfgGraph";
            this.wfgGraph.Plots.AddRange(new NationalInstruments.UI.WaveformPlot[] {
            this.waveformPlot1,
            this.waveformPlot2});
            this.wfgGraph.Size = new System.Drawing.Size(290, 114);
            this.wfgGraph.TabIndex = 136;
            this.wfgGraph.UseColorGenerator = true;
            this.wfgGraph.XAxes.AddRange(new NationalInstruments.UI.XAxis[] {
            this.xAxis1});
            this.wfgGraph.YAxes.AddRange(new NationalInstruments.UI.YAxis[] {
            this.yAxis1});
            // 
            // waveformPlot1
            // 
            this.waveformPlot1.HistoryCapacity = 200;
            this.waveformPlot1.LineColor = System.Drawing.Color.White;
            this.waveformPlot1.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
            this.waveformPlot1.LineWidth = 2F;
            this.waveformPlot1.PointSize = new System.Drawing.Size(10, 10);
            this.waveformPlot1.XAxis = this.xAxis1;
            this.waveformPlot1.YAxis = this.yAxis1;
            // 
            // xAxis1
            // 
            this.xAxis1.Visible = false;
            // 
            // yAxis1
            // 
            this.yAxis1.CaptionFont = new System.Drawing.Font("Source Code Pro", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yAxis1.Mode = NationalInstruments.UI.AxisMode.AutoScaleVisibleLoose;
            // 
            // waveformPlot2
            // 
            this.waveformPlot2.HistoryCapacity = 200;
            this.waveformPlot2.LineColor = System.Drawing.Color.Lime;
            this.waveformPlot2.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
            this.waveformPlot2.LineWidth = 2F;
            this.waveformPlot2.XAxis = this.xAxis1;
            this.waveformPlot2.YAxis = this.yAxis1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Source Code Pro", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(283, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 14);
            this.label1.TabIndex = 138;
            this.label1.Text = "[s]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Source Code Pro", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(178, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 14);
            this.label4.TabIndex = 137;
            this.label4.Text = "LeadTime: ";
            // 
            // lbLeadTime
            // 
            this.lbLeadTime.AutoSize = true;
            this.lbLeadTime.Font = new System.Drawing.Font("Source Code Pro", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLeadTime.Location = new System.Drawing.Point(247, 19);
            this.lbLeadTime.Name = "lbLeadTime";
            this.lbLeadTime.Size = new System.Drawing.Size(42, 14);
            this.lbLeadTime.TabIndex = 139;
            this.lbLeadTime.Text = "00.00";
            this.lbLeadTime.Click += new System.EventHandler(this.label5_Click);
            // 
            // frmAlignStatus
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(313, 176);
            this.Controls.Add(this.lbLeadTime);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.wfgGraph);
            this.Controls.Add(this.lbCurUnit);
            this.Controls.Add(this.lbMaxUnit);
            this.Controls.Add(this.lbCurVal);
            this.Controls.Add(this.lbMaxValue);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.lbStage);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.lbCmdName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAlignStatus";
            this.Text = "Alignment Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAlignStatus_FormClosing);
            this.Load += new System.EventHandler(this.frmAlignStatus_Load);
            ((System.ComponentModel.ISupportInitialize)(this.wfgGraph)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Label lbMaxUnit;
    internal System.Windows.Forms.Label lbCurVal;
    internal System.Windows.Forms.Label lbMaxValue;
    internal System.Windows.Forms.Label Label6;
    internal System.Windows.Forms.Label Label3;
    internal System.Windows.Forms.Label lbStage;
    internal System.Windows.Forms.Label Label2;
    internal System.Windows.Forms.Label lbCmdName;
    internal System.Windows.Forms.Label lbCurUnit;
    internal NationalInstruments.UI.WindowsForms.WaveformGraph wfgGraph;
    private NationalInstruments.UI.WaveformPlot waveformPlot1;
    private NationalInstruments.UI.XAxis xAxis1;
    private NationalInstruments.UI.YAxis yAxis1;
    private NationalInstruments.UI.WaveformPlot waveformPlot2;
    internal System.Windows.Forms.Label label1;
    internal System.Windows.Forms.Label label4;
    internal System.Windows.Forms.Label lbLeadTime;
}
