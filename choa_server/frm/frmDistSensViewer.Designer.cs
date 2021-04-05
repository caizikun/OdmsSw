


partial class frmDistSensViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDistSensViewer));
            this.btnStartReading = new System.Windows.Forms.Button();
            this.lbUnit = new System.Windows.Forms.Label();
            this.btnStopReading = new System.Windows.Forms.Button();
            this.Label11 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbLeftSens = new System.Windows.Forms.Label();
            this.lbRightSens = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.wg1 = new DrBae.TnM.UI.WdmGraph();
            this.wg2 = new DrBae.TnM.UI.WdmGraph();
            this.SuspendLayout();
            // 
            // btnStartReading
            // 
            this.btnStartReading.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartReading.Location = new System.Drawing.Point(476, 31);
            this.btnStartReading.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStartReading.Name = "btnStartReading";
            this.btnStartReading.Size = new System.Drawing.Size(80, 59);
            this.btnStartReading.TabIndex = 143;
            this.btnStartReading.Text = "START";
            this.btnStartReading.UseVisualStyleBackColor = true;
            this.btnStartReading.Click += new System.EventHandler(this.btnStartReading_Click);
            // 
            // lbUnit
            // 
            this.lbUnit.AutoSize = true;
            this.lbUnit.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUnit.Location = new System.Drawing.Point(173, 175);
            this.lbUnit.Name = "lbUnit";
            this.lbUnit.Size = new System.Drawing.Size(26, 19);
            this.lbUnit.TabIndex = 84;
            this.lbUnit.Text = "[V]";
            // 
            // btnStopReading
            // 
            this.btnStopReading.BackColor = System.Drawing.Color.Crimson;
            this.btnStopReading.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopReading.ForeColor = System.Drawing.Color.White;
            this.btnStopReading.Location = new System.Drawing.Point(476, 98);
            this.btnStopReading.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStopReading.Name = "btnStopReading";
            this.btnStopReading.Size = new System.Drawing.Size(80, 59);
            this.btnStopReading.TabIndex = 144;
            this.btnStopReading.Text = "STOP";
            this.btnStopReading.UseVisualStyleBackColor = false;
            this.btnStopReading.Click += new System.EventHandler(this.btnStopReading_Click);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(56, 9);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(134, 18);
            this.Label11.TabIndex = 264;
            this.Label11.Text = "SENSOR1(INPUT)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(281, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 18);
            this.label1.TabIndex = 265;
            this.label1.Text = "SENSOR2(OUTPUT)";
            // 
            // lbLeftSens
            // 
            this.lbLeftSens.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLeftSens.Location = new System.Drawing.Point(70, 168);
            this.lbLeftSens.Name = "lbLeftSens";
            this.lbLeftSens.Size = new System.Drawing.Size(96, 31);
            this.lbLeftSens.TabIndex = 266;
            this.lbLeftSens.Text = "???";
            this.lbLeftSens.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbRightSens
            // 
            this.lbRightSens.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRightSens.ForeColor = System.Drawing.Color.Tomato;
            this.lbRightSens.Location = new System.Drawing.Point(303, 168);
            this.lbRightSens.Name = "lbRightSens";
            this.lbRightSens.Size = new System.Drawing.Size(96, 31);
            this.lbRightSens.TabIndex = 268;
            this.lbRightSens.Text = "???";
            this.lbRightSens.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(406, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 19);
            this.label3.TabIndex = 267;
            this.label3.Text = "[V]";
            // 
            // wg1
            // 
            this.wg1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            this.wg1.Cwl = null;
            this.wg1.DataSource = null;
            this.wg1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wg1.IntervalOffetX = 0;
            this.wg1.IntervalX = 1000;
            this.wg1.IntervalY = 1000D;
            this.wg1.LineThickness = 1;
            this.wg1.Location = new System.Drawing.Point(2, 31);
            this.wg1.Margin = new System.Windows.Forms.Padding(0);
            this.wg1.MarkerSize = 6;
            this.wg1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
            this.wg1.MaxY = double.NaN;
            this.wg1.MinY = double.NaN;
            this.wg1.Name = "wg1";
            this.wg1.ReCalcIntervalX = true;
            this.wg1.ScaleFactorX = 1000;
            this.wg1.ScaleFactorY = 0D;
            this.wg1.ShowLegends = false;
            this.wg1.Size = new System.Drawing.Size(235, 135);
            this.wg1.TabIndex = 269;
            this.wg1.Wl = ((System.Collections.Generic.List<int>)(resources.GetObject("wg1.Wl")));
            // 
            // wg2
            // 
            this.wg2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            this.wg2.Cwl = null;
            this.wg2.DataSource = null;
            this.wg2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wg2.IntervalOffetX = 0;
            this.wg2.IntervalX = 1000;
            this.wg2.IntervalY = 1000D;
            this.wg2.LineThickness = 1;
            this.wg2.Location = new System.Drawing.Point(238, 31);
            this.wg2.Margin = new System.Windows.Forms.Padding(0);
            this.wg2.MarkerSize = 6;
            this.wg2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
            this.wg2.MaxY = double.NaN;
            this.wg2.MinY = double.NaN;
            this.wg2.Name = "wg2";
            this.wg2.ReCalcIntervalX = true;
            this.wg2.ScaleFactorX = 1000;
            this.wg2.ScaleFactorY = 0D;
            this.wg2.ShowLegends = false;
            this.wg2.Size = new System.Drawing.Size(235, 135);
            this.wg2.TabIndex = 270;
            this.wg2.Wl = ((System.Collections.Generic.List<int>)(resources.GetObject("wg2.Wl")));
            // 
            // frmDistSensViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 204);
            this.Controls.Add(this.wg2);
            this.Controls.Add(this.wg1);
            this.Controls.Add(this.lbRightSens);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbLeftSens);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Label11);
            this.Controls.Add(this.lbUnit);
            this.Controls.Add(this.btnStartReading);
            this.Controls.Add(this.btnStopReading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDistSensViewer";
            this.Text = "Distance sensor viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDistSensViewer_FormClosing);
            this.Load += new System.EventHandler(this.frmDistSensViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button btnStartReading;
    internal System.Windows.Forms.Label lbUnit;
    internal System.Windows.Forms.Button btnStopReading;
    internal System.Windows.Forms.Label Label11;
    internal System.Windows.Forms.Label label1;
    internal System.Windows.Forms.Label lbLeftSens;
    internal System.Windows.Forms.Label lbRightSens;
    internal System.Windows.Forms.Label label3;
    private DrBae.TnM.UI.WdmGraph wg1;
    private DrBae.TnM.UI.WdmGraph wg2;
}
