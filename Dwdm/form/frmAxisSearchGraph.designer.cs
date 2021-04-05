
partial class frmAxisSearchGraph
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnPeak = new System.Windows.Forms.Button();
            this.btnUnit = new System.Windows.Forms.Button();
            this.btnCursor = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.wfgSearch = new NationalInstruments.UI.WindowsForms.WaveformGraph();
            this.waveformPlot1 = new NationalInstruments.UI.WaveformPlot();
            this.xAxis1 = new NationalInstruments.UI.XAxis();
            this.yAxis1 = new NationalInstruments.UI.YAxis();
            ((System.ComponentModel.ISupportInitialize)(this.wfgSearch)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Source Code Pro", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Location = new System.Drawing.Point(288, 311);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(94, 25);
            this.btnSave.TabIndex = 212;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnPeak
            // 
            this.btnPeak.Font = new System.Drawing.Font("Source Code Pro", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPeak.Location = new System.Drawing.Point(193, 311);
            this.btnPeak.Name = "btnPeak";
            this.btnPeak.Size = new System.Drawing.Size(94, 25);
            this.btnPeak.TabIndex = 211;
            this.btnPeak.Text = "PEAK";
            this.btnPeak.UseVisualStyleBackColor = true;
            this.btnPeak.Click += new System.EventHandler(this.btnPeak_Click);
            // 
            // btnUnit
            // 
            this.btnUnit.Font = new System.Drawing.Font("Source Code Pro", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnit.Location = new System.Drawing.Point(3, 311);
            this.btnUnit.Name = "btnUnit";
            this.btnUnit.Size = new System.Drawing.Size(94, 25);
            this.btnUnit.TabIndex = 210;
            this.btnUnit.Text = "UNIT  dBm ";
            this.btnUnit.UseVisualStyleBackColor = true;
            this.btnUnit.Click += new System.EventHandler(this.btnUnit_Click);
            // 
            // btnCursor
            // 
            this.btnCursor.Font = new System.Drawing.Font("Source Code Pro", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCursor.Location = new System.Drawing.Point(98, 311);
            this.btnCursor.Name = "btnCursor";
            this.btnCursor.Size = new System.Drawing.Size(94, 25);
            this.btnCursor.TabIndex = 209;
            this.btnCursor.Text = "CURSOR ON";
            this.btnCursor.UseVisualStyleBackColor = true;
            this.btnCursor.Click += new System.EventHandler(this.btnCursor_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Source Code Pro", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(434, 311);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 25);
            this.btnClose.TabIndex = 208;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // wfgSearch
            // 
            this.wfgSearch.Font = new System.Drawing.Font("Source Code Pro", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.wfgSearch.Location = new System.Drawing.Point(3, 3);
            this.wfgSearch.Name = "wfgSearch";
            this.wfgSearch.Plots.AddRange(new NationalInstruments.UI.WaveformPlot[] {
            this.waveformPlot1});
            this.wfgSearch.Size = new System.Drawing.Size(525, 302);
            this.wfgSearch.TabIndex = 213;
            this.wfgSearch.XAxes.AddRange(new NationalInstruments.UI.XAxis[] {
            this.xAxis1});
            this.wfgSearch.YAxes.AddRange(new NationalInstruments.UI.YAxis[] {
            this.yAxis1});
            // 
            // waveformPlot1
            // 
            this.waveformPlot1.HistoryCapacity = 20000;
            this.waveformPlot1.LineColor = System.Drawing.Color.White;
            this.waveformPlot1.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
            this.waveformPlot1.LineWidth = 2F;
            this.waveformPlot1.XAxis = this.xAxis1;
            this.waveformPlot1.YAxis = this.yAxis1;
            // 
            // xAxis1
            // 
            this.xAxis1.AutoMinorDivisionFrequency = 5;
            this.xAxis1.MajorDivisions.GridVisible = true;
            this.xAxis1.MinorDivisions.GridVisible = true;
            this.xAxis1.Mode = NationalInstruments.UI.AxisMode.AutoScaleExact;
            this.xAxis1.Range = new NationalInstruments.UI.Range(0D, 2000D);
            // 
            // yAxis1
            // 
            this.yAxis1.AutoMinorDivisionFrequency = 5;
            this.yAxis1.Caption = "[dB]";
            this.yAxis1.CaptionFont = new System.Drawing.Font("Source Code Pro", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.yAxis1.MajorDivisions.GridVisible = true;
            this.yAxis1.MinorDivisions.GridVisible = true;
            this.yAxis1.Mode = NationalInstruments.UI.AxisMode.AutoScaleVisibleLoose;
            this.yAxis1.Range = new NationalInstruments.UI.Range(-100D, 10D);
            // 
            // frmAxisSearchGraph
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(530, 336);
            this.Controls.Add(this.wfgSearch);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnPeak);
            this.Controls.Add(this.btnUnit);
            this.Controls.Add(this.btnCursor);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAxisSearchGraph";
            this.Text = "Axissearch Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Load += new System.EventHandler(this.frmAxisSearchGraph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.wfgSearch)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.Button btnSave;
    internal System.Windows.Forms.Button btnPeak;
    internal System.Windows.Forms.Button btnUnit;
    internal System.Windows.Forms.Button btnCursor;
    internal System.Windows.Forms.Button btnClose;
    internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
    private NationalInstruments.UI.WindowsForms.WaveformGraph wfgSearch;
    internal NationalInstruments.UI.WaveformPlot waveformPlot1;
    internal NationalInstruments.UI.XAxis xAxis1;
    private NationalInstruments.UI.YAxis yAxis1;

}
