
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAlignStatus));
            this.lbMaxUnit = new System.Windows.Forms.Label();
            this.lbCurVal = new System.Windows.Forms.Label();
            this.lbMaxValue = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.lbStage = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.lbCmdName = new System.Windows.Forms.Label();
            this.lbCurUnit = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbLeadTime = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mGraph = new DrBae.TnM.UI.WdmGraph();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbMaxUnit
            // 
            this.lbMaxUnit.AutoSize = true;
            this.lbMaxUnit.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxUnit.Location = new System.Drawing.Point(364, 142);
            this.lbMaxUnit.Name = "lbMaxUnit";
            this.lbMaxUnit.Size = new System.Drawing.Size(79, 17);
            this.lbMaxUnit.TabIndex = 134;
            this.lbMaxUnit.Text = "[dBm] or [V]";
            this.lbMaxUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCurVal
            // 
            this.lbCurVal.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurVal.Location = new System.Drawing.Point(312, 100);
            this.lbCurVal.Name = "lbCurVal";
            this.lbCurVal.Size = new System.Drawing.Size(53, 19);
            this.lbCurVal.TabIndex = 132;
            this.lbCurVal.Text = "0.01";
            this.lbCurVal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMaxValue
            // 
            this.lbMaxValue.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMaxValue.Location = new System.Drawing.Point(312, 141);
            this.lbMaxValue.Name = "lbMaxValue";
            this.lbMaxValue.Size = new System.Drawing.Size(53, 18);
            this.lbMaxValue.TabIndex = 131;
            this.lbMaxValue.Text = "0.01";
            this.lbMaxValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.ForeColor = System.Drawing.Color.DarkRed;
            this.Label6.Location = new System.Drawing.Point(312, 123);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(40, 17);
            this.Label6.TabIndex = 130;
            this.Label6.Text = "Max .";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.Color.DarkRed;
            this.Label3.Location = new System.Drawing.Point(312, 83);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(31, 17);
            this.Label3.TabIndex = 129;
            this.Label3.Text = "Cur.";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbStage
            // 
            this.lbStage.AutoSize = true;
            this.lbStage.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStage.ForeColor = System.Drawing.Color.Blue;
            this.lbStage.Location = new System.Drawing.Point(400, 25);
            this.lbStage.Name = "lbStage";
            this.lbStage.Size = new System.Drawing.Size(38, 17);
            this.lbStage.TabIndex = 128;
            this.lbStage.Text = "LEFT";
            this.lbStage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.Color.DarkRed;
            this.Label2.Location = new System.Drawing.Point(312, 25);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(53, 17);
            this.Label2.TabIndex = 127;
            this.Label2.Text = "STAGE :";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCmdName
            // 
            this.lbCmdName.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCmdName.Location = new System.Drawing.Point(1, 11);
            this.lbCmdName.Name = "lbCmdName";
            this.lbCmdName.Size = new System.Drawing.Size(305, 17);
            this.lbCmdName.TabIndex = 126;
            this.lbCmdName.Text = "...";
            this.lbCmdName.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lbCurUnit
            // 
            this.lbCurUnit.AutoSize = true;
            this.lbCurUnit.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurUnit.Location = new System.Drawing.Point(364, 101);
            this.lbCurUnit.Name = "lbCurUnit";
            this.lbCurUnit.Size = new System.Drawing.Size(79, 17);
            this.lbCurUnit.TabIndex = 135;
            this.lbCurUnit.Text = "[dBm] or [V]";
            this.lbCurUnit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(421, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 17);
            this.label1.TabIndex = 138;
            this.label1.Text = "[s]";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkRed;
            this.label4.Location = new System.Drawing.Point(312, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 17);
            this.label4.TabIndex = 137;
            this.label4.Text = "LeadTime: ";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbLeadTime
            // 
            this.lbLeadTime.AutoSize = true;
            this.lbLeadTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLeadTime.Location = new System.Drawing.Point(381, 49);
            this.lbLeadTime.Name = "lbLeadTime";
            this.lbLeadTime.Size = new System.Drawing.Size(44, 17);
            this.lbLeadTime.TabIndex = 139;
            this.lbLeadTime.Text = "00.00";
            this.lbLeadTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mGraph);
            this.groupBox1.Controls.Add(this.lbLeadTime);
            this.groupBox1.Controls.Add(this.lbCmdName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.Label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lbStage);
            this.groupBox1.Controls.Add(this.Label3);
            this.groupBox1.Controls.Add(this.Label6);
            this.groupBox1.Controls.Add(this.lbMaxValue);
            this.groupBox1.Controls.Add(this.lbCurVal);
            this.groupBox1.Controls.Add(this.lbCurUnit);
            this.groupBox1.Controls.Add(this.lbMaxUnit);
            this.groupBox1.Location = new System.Drawing.Point(4, -5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(451, 169);
            this.groupBox1.TabIndex = 140;
            this.groupBox1.TabStop = false;
            // 
            // mGraph
            // 
            this.mGraph.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            this.mGraph.Cwl = null;
            this.mGraph.DataSource = null;
            this.mGraph.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mGraph.IntervalOffetX = 0;
            this.mGraph.IntervalX = 1000;
            this.mGraph.IntervalY = 1000D;
            this.mGraph.LineThickness = 1;
            this.mGraph.Location = new System.Drawing.Point(0, 31);
            this.mGraph.MarkerSize = 6;
            this.mGraph.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
            this.mGraph.MaxY = double.NaN;
            this.mGraph.MinY = double.NaN;
            this.mGraph.Name = "mGraph";
            this.mGraph.ScaleFactorX = 1000;
            this.mGraph.ScaleFactorY = 0D;
            this.mGraph.ShowLegends = false;
            this.mGraph.Size = new System.Drawing.Size(300, 147);
            this.mGraph.TabIndex = 141;
            this.mGraph.Wl = ((System.Collections.Generic.List<int>)(resources.GetObject("mGraph.Wl")));
            // 
            // frmAlignStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 172);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAlignStatus";
            this.Text = "Alignment Status";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAlignStatus_FormClosing);
            this.Load += new System.EventHandler(this.frmAlignStatus_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

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
    internal System.Windows.Forms.Label label1;
    internal System.Windows.Forms.Label label4;
    internal System.Windows.Forms.Label lbLeadTime;
    private System.Windows.Forms.GroupBox groupBox1;
    private DrBae.TnM.UI.WdmGraph mGraph;
}
