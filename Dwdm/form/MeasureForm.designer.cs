
partial class MeasureForm
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
			this.components = new System.ComponentModel.Container();
			this.btnMeasure = new System.Windows.Forms.Button();
			this.btnClearChipSn = new System.Windows.Forms.Button();
			this.txtNumChips = new System.Windows.Forms.TextBox();
			this.Label16 = new System.Windows.Forms.Label();
			this.btnApplyChipSn = new System.Windows.Forms.Button();
			this.txtFisrtChipNo = new System.Windows.Forms.TextBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.panelMeasure = new System.Windows.Forms.GroupBox();
			this.chkSerial = new System.Windows.Forms.CheckBox();
			this.Label15 = new System.Windows.Forms.Label();
			this.grpGraphAnalysis = new System.Windows.Forms.GroupBox();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.lbChipNo = new System.Windows.Forms.Label();
			this.mGraph = new NationalInstruments.UI.WindowsForms.WaveformGraph();
			this.waveformPlot2 = new NationalInstruments.UI.WaveformPlot();
			this.xAxis2 = new NationalInstruments.UI.XAxis();
			this.yAxis2 = new NationalInstruments.UI.YAxis();
			this.statusBar = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.btnOpenStages = new System.Windows.Forms.Button();
			this.groupChipList = new System.Windows.Forms.GroupBox();
			this.uiGridDut = new System.Windows.Forms.DataGridView();
			this.grpEtc = new System.Windows.Forms.GroupBox();
			this.btnCloseStages = new System.Windows.Forms.Button();
			this.panelParam = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupAlignCheck = new System.Windows.Forms.GroupBox();
			this.uiCladDistance = new System.Windows.Forms.TextBox();
			this.chkCenterStage = new System.Windows.Forms.CheckBox();
			this.chkRoll = new System.Windows.Forms.CheckBox();
			this.chkAlignment = new System.Windows.Forms.CheckBox();
			this.uiMeasureCladding = new System.Windows.Forms.CheckBox();
			this.chkFaArrangement = new System.Windows.Forms.CheckBox();
			this.chkRetChip1Pos = new System.Windows.Forms.CheckBox();
			this.chkMeasurement = new System.Windows.Forms.CheckBox();
			this.groupPmChannels = new System.Windows.Forms.GroupBox();
			this.txtLoopStop = new System.Windows.Forms.TextBox();
			this.txtLoopStart = new System.Windows.Forms.TextBox();
			this.label42 = new System.Windows.Forms.Label();
			this.chkLoopScan = new System.Windows.Forms.CheckBox();
			this.txtPmPortStart = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtPmPortStop = new System.Windows.Forms.ComboBox();
			this.chkChOrderReverse = new System.Windows.Forms.CheckBox();
			this.Label33 = new System.Windows.Forms.Label();
			this.groupAlignParams = new System.Windows.Forms.GroupBox();
			this.txtChipWidth = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.Label39 = new System.Windows.Forms.Label();
			this.txtAlignTimes = new System.Windows.Forms.TextBox();
			this.txtChPitch = new System.Windows.Forms.ComboBox();
			this.label34 = new System.Windows.Forms.Label();
			this.Label36 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.txtYBuffer = new System.Windows.Forms.TextBox();
			this.txtZBuffer = new System.Windows.Forms.TextBox();
			this.label32 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.txtChipsPerRightAlign = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.groupSave = new System.Windows.Forms.GroupBox();
			this.btnSaveFolder = new System.Windows.Forms.Button();
			this.txtSaveWaveStop = new System.Windows.Forms.TextBox();
			this.txtSaveWaveStart = new System.Windows.Forms.TextBox();
			this.rbtnAutoSaveRng = new System.Windows.Forms.RadioButton();
			this.label44 = new System.Windows.Forms.Label();
			this.rbtnAutoSaveFull = new System.Windows.Forms.RadioButton();
			this.chkAutoSave = new System.Windows.Forms.CheckBox();
			this.chkWaferFolder = new System.Windows.Forms.CheckBox();
			this.groupTls = new System.Windows.Forms.GroupBox();
			this.txtTlsNoiseShift = new System.Windows.Forms.TextBox();
			this.label40 = new System.Windows.Forms.Label();
			this.label41 = new System.Windows.Forms.Label();
			this.txtTlsSNR = new System.Windows.Forms.TextBox();
			this.label35 = new System.Windows.Forms.Label();
			this.label38 = new System.Windows.Forms.Label();
			this.txtWaveStep = new System.Windows.Forms.TextBox();
			this.txtTlsPower = new System.Windows.Forms.TextBox();
			this.txtWaveStop = new System.Windows.Forms.TextBox();
			this.txtWaveStart = new System.Windows.Forms.TextBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbOptRng = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.GroupPmGain = new System.Windows.Forms.GroupBox();
			this.rbtnGain2 = new System.Windows.Forms.RadioButton();
			this.rbtnGain1 = new System.Windows.Forms.RadioButton();
			this.txtPmGainLevel2 = new System.Windows.Forms.TextBox();
			this.txtPmGainLevel1 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chkBackupPower = new System.Windows.Forms.CheckBox();
			this.chkBackupWave = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.btnClosedPosGo = new System.Windows.Forms.Button();
			this.btnSaveAsClosePosition = new System.Windows.Forms.Button();
			this.label21 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.lbClosePosOutZ = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.lbClosePosInZ = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lbl_Init_Center_Y = new System.Windows.Forms.Label();
			this.label27 = new System.Windows.Forms.Label();
			this.label43 = new System.Windows.Forms.Label();
			this.lbl_Init_Center_X = new System.Windows.Forms.Label();
			this.label45 = new System.Windows.Forms.Label();
			this.btnInitPosGo = new System.Windows.Forms.Button();
			this.lbInitPosOutTz = new System.Windows.Forms.Label();
			this.btnSaveAsInitPosition = new System.Windows.Forms.Button();
			this.label28 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.lbInitPosOutTy = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lbInitPosInX = new System.Windows.Forms.Label();
			this.lbInitPosOutTx = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.lbInitPosInY = new System.Windows.Forms.Label();
			this.lbInitPosOutZ = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lbInitPosInZ = new System.Windows.Forms.Label();
			this.lbInitPosOutY = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lbInitPosInTx = new System.Windows.Forms.Label();
			this.label37 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.lbInitPosOutX = new System.Windows.Forms.Label();
			this.lbInitPosInTy = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.lbInitPosInTz = new System.Windows.Forms.Label();
			this.WaveformPlot1 = new NationalInstruments.UI.WaveformPlot();
			this.XAxis1 = new NationalInstruments.UI.XAxis();
			this.YAxis1 = new NationalInstruments.UI.YAxis();
			this.uiLoadReference = new System.Windows.Forms.Button();
			this.uiPol_Minus45Diagonal = new System.Windows.Forms.CheckBox();
			this.uiPol_LeftCircular = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.btnSave = new System.Windows.Forms.Button();
			this.panelMeasure.SuspendLayout();
			this.grpGraphAnalysis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mGraph)).BeginInit();
			this.statusBar.SuspendLayout();
			this.groupChipList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.uiGridDut)).BeginInit();
			this.grpEtc.SuspendLayout();
			this.panelParam.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupAlignCheck.SuspendLayout();
			this.groupPmChannels.SuspendLayout();
			this.groupAlignParams.SuspendLayout();
			this.groupSave.SuspendLayout();
			this.groupTls.SuspendLayout();
			this.GroupPmGain.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnMeasure
			// 
			this.btnMeasure.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnMeasure.Location = new System.Drawing.Point(9, 106);
			this.btnMeasure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnMeasure.Name = "btnMeasure";
			this.btnMeasure.Size = new System.Drawing.Size(234, 49);
			this.btnMeasure.TabIndex = 24;
			this.btnMeasure.Text = "MEASURE";
			this.btnMeasure.UseVisualStyleBackColor = true;
			this.btnMeasure.Click += new System.EventHandler(this.btnMeasure_Click);
			// 
			// btnClearChipSn
			// 
			this.btnClearChipSn.Location = new System.Drawing.Point(190, 74);
			this.btnClearChipSn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnClearChipSn.Name = "btnClearChipSn";
			this.btnClearChipSn.Size = new System.Drawing.Size(53, 25);
			this.btnClearChipSn.TabIndex = 23;
			this.btnClearChipSn.Text = "Clear";
			this.btnClearChipSn.UseVisualStyleBackColor = true;
			this.btnClearChipSn.Click += new System.EventHandler(this.btnClearChipSn_Click);
			// 
			// txtNumChips
			// 
			this.txtNumChips.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtNumChips.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Neon.Dwdm.Properties.Settings.Default, "NumChips", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtNumChips.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtNumChips.ForeColor = System.Drawing.Color.White;
			this.txtNumChips.Location = new System.Drawing.Point(82, 75);
			this.txtNumChips.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtNumChips.Name = "txtNumChips";
			this.txtNumChips.Size = new System.Drawing.Size(26, 22);
			this.txtNumChips.TabIndex = 21;
			this.txtNumChips.Text = global::Neon.Dwdm.Properties.Settings.Default.NumChips;
			this.txtNumChips.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Label16
			// 
			this.Label16.AutoSize = true;
			this.Label16.Location = new System.Drawing.Point(6, 79);
			this.Label16.Name = "Label16";
			this.Label16.Size = new System.Drawing.Size(68, 15);
			this.Label16.TabIndex = 275;
			this.Label16.Text = "Num Chips";
			// 
			// btnApplyChipSn
			// 
			this.btnApplyChipSn.Location = new System.Drawing.Point(114, 74);
			this.btnApplyChipSn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnApplyChipSn.Name = "btnApplyChipSn";
			this.btnApplyChipSn.Size = new System.Drawing.Size(70, 25);
			this.btnApplyChipSn.TabIndex = 22;
			this.btnApplyChipSn.Text = "Apply";
			this.btnApplyChipSn.UseVisualStyleBackColor = true;
			this.btnApplyChipSn.Click += new System.EventHandler(this.btnApplyChipSn_Click);
			// 
			// txtFisrtChipNo
			// 
			this.txtFisrtChipNo.BackColor = System.Drawing.SystemColors.Window;
			this.txtFisrtChipNo.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Neon.Dwdm.Properties.Settings.Default, "lastChipSn", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.txtFisrtChipNo.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtFisrtChipNo.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtFisrtChipNo.Location = new System.Drawing.Point(9, 43);
			this.txtFisrtChipNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtFisrtChipNo.Name = "txtFisrtChipNo";
			this.txtFisrtChipNo.Size = new System.Drawing.Size(234, 25);
			this.txtFisrtChipNo.TabIndex = 20;
			this.txtFisrtChipNo.Text = global::Neon.Dwdm.Properties.Settings.Default.lastChipSn;
			// 
			// btnStop
			// 
			this.btnStop.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnStop.ForeColor = System.Drawing.Color.OrangeRed;
			this.btnStop.Location = new System.Drawing.Point(9, 20);
			this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(234, 44);
			this.btnStop.TabIndex = 280;
			this.btnStop.Text = "STOP";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// panelMeasure
			// 
			this.panelMeasure.Controls.Add(this.chkSerial);
			this.panelMeasure.Controls.Add(this.Label15);
			this.panelMeasure.Controls.Add(this.txtFisrtChipNo);
			this.panelMeasure.Controls.Add(this.btnMeasure);
			this.panelMeasure.Controls.Add(this.btnApplyChipSn);
			this.panelMeasure.Controls.Add(this.Label16);
			this.panelMeasure.Controls.Add(this.btnClearChipSn);
			this.panelMeasure.Controls.Add(this.txtNumChips);
			this.panelMeasure.Location = new System.Drawing.Point(199, 13);
			this.panelMeasure.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelMeasure.Name = "panelMeasure";
			this.panelMeasure.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelMeasure.Size = new System.Drawing.Size(249, 166);
			this.panelMeasure.TabIndex = 281;
			this.panelMeasure.TabStop = false;
			this.panelMeasure.Text = "Measurement";
			// 
			// chkSerial
			// 
			this.chkSerial.AutoSize = true;
			this.chkSerial.Location = new System.Drawing.Point(64, 23);
			this.chkSerial.Name = "chkSerial";
			this.chkSerial.Size = new System.Drawing.Size(15, 14);
			this.chkSerial.TabIndex = 317;
			this.chkSerial.UseVisualStyleBackColor = true;
			// 
			// Label15
			// 
			this.Label15.AutoSize = true;
			this.Label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label15.Location = new System.Drawing.Point(6, 21);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(52, 15);
			this.Label15.TabIndex = 280;
			this.Label15.Text = "Chip SN";
			// 
			// grpGraphAnalysis
			// 
			this.grpGraphAnalysis.Controls.Add(this.splitContainer1);
			this.grpGraphAnalysis.Location = new System.Drawing.Point(199, 312);
			this.grpGraphAnalysis.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.grpGraphAnalysis.Name = "grpGraphAnalysis";
			this.grpGraphAnalysis.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.grpGraphAnalysis.Size = new System.Drawing.Size(913, 516);
			this.grpGraphAnalysis.TabIndex = 284;
			this.grpGraphAnalysis.TabStop = false;
			this.grpGraphAnalysis.Text = "Graph";
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(3, 20);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.lbChipNo);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.mGraph);
			this.splitContainer1.Size = new System.Drawing.Size(907, 492);
			this.splitContainer1.SplitterDistance = 34;
			this.splitContainer1.TabIndex = 286;
			// 
			// lbChipNo
			// 
			this.lbChipNo.AutoSize = true;
			this.lbChipNo.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbChipNo.Location = new System.Drawing.Point(6, 9);
			this.lbChipNo.Name = "lbChipNo";
			this.lbChipNo.Size = new System.Drawing.Size(184, 18);
			this.lbChipNo.TabIndex = 284;
			this.lbChipNo.Text = "TVD12280044-2-4-160706";
			this.lbChipNo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// mGraph
			// 
			this.mGraph.CaptionFont = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mGraph.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mGraph.Location = new System.Drawing.Point(0, 0);
			this.mGraph.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.mGraph.Name = "mGraph";
			this.mGraph.Plots.AddRange(new NationalInstruments.UI.WaveformPlot[] {
            this.waveformPlot2});
			this.mGraph.Size = new System.Drawing.Size(907, 454);
			this.mGraph.TabIndex = 286;
			this.mGraph.UseColorGenerator = true;
			this.mGraph.XAxes.AddRange(new NationalInstruments.UI.XAxis[] {
            this.xAxis2});
			this.mGraph.YAxes.AddRange(new NationalInstruments.UI.YAxis[] {
            this.yAxis2});
			// 
			// waveformPlot2
			// 
			this.waveformPlot2.HistoryCapacity = 20000;
			this.waveformPlot2.LineColor = System.Drawing.Color.LightGoldenrodYellow;
			this.waveformPlot2.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
			this.waveformPlot2.XAxis = this.xAxis2;
			this.waveformPlot2.YAxis = this.yAxis2;
			// 
			// xAxis2
			// 
			this.xAxis2.AutoMinorDivisionFrequency = 5;
			this.xAxis2.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.xAxis2.MajorDivisions.GridVisible = true;
			this.xAxis2.MajorDivisions.LabelFormat = new NationalInstruments.UI.FormatString(NationalInstruments.UI.FormatStringMode.Numeric, "F0");
			this.xAxis2.MinorDivisions.GridVisible = true;
			this.xAxis2.Mode = NationalInstruments.UI.AxisMode.AutoScaleExact;
			this.xAxis2.Range = new NationalInstruments.UI.Range(1500D, 1570D);
			// 
			// yAxis2
			// 
			this.yAxis2.AutoMinorDivisionFrequency = 5;
			this.yAxis2.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.yAxis2.MajorDivisions.GridVisible = true;
			this.yAxis2.MajorDivisions.LabelFormat = new NationalInstruments.UI.FormatString(NationalInstruments.UI.FormatStringMode.Numeric, "F1");
			this.yAxis2.MinorDivisions.GridVisible = true;
			this.yAxis2.Mode = NationalInstruments.UI.AxisMode.Fixed;
			this.yAxis2.Range = new NationalInstruments.UI.Range(-60D, 0D);
			// 
			// statusBar
			// 
			this.statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
			this.statusBar.Location = new System.Drawing.Point(0, 832);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(1119, 29);
			this.statusBar.TabIndex = 285;
			this.statusBar.Text = "StatusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.statusLabel.Font = new System.Drawing.Font("맑은 고딕", 11F);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(150, 24);
			this.statusLabel.Text = "status message here";
			// 
			// btnOpenStages
			// 
			this.btnOpenStages.Location = new System.Drawing.Point(9, 72);
			this.btnOpenStages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnOpenStages.Name = "btnOpenStages";
			this.btnOpenStages.Size = new System.Drawing.Size(114, 35);
			this.btnOpenStages.TabIndex = 286;
			this.btnOpenStages.Text = "OPEN STAGES";
			this.btnOpenStages.UseVisualStyleBackColor = true;
			this.btnOpenStages.Click += new System.EventHandler(this.btnOpenStages_Click);
			// 
			// groupChipList
			// 
			this.groupChipList.Controls.Add(this.uiGridDut);
			this.groupChipList.Location = new System.Drawing.Point(454, 13);
			this.groupChipList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupChipList.Name = "groupChipList";
			this.groupChipList.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupChipList.Size = new System.Drawing.Size(658, 234);
			this.groupChipList.TabIndex = 287;
			this.groupChipList.TabStop = false;
			this.groupChipList.Text = "Chip List";
			// 
			// uiGridDut
			// 
			this.uiGridDut.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.uiGridDut.Location = new System.Drawing.Point(21, 43);
			this.uiGridDut.Name = "uiGridDut";
			this.uiGridDut.RowTemplate.Height = 23;
			this.uiGridDut.Size = new System.Drawing.Size(293, 110);
			this.uiGridDut.TabIndex = 272;
			// 
			// grpEtc
			// 
			this.grpEtc.Controls.Add(this.btnCloseStages);
			this.grpEtc.Controls.Add(this.btnOpenStages);
			this.grpEtc.Controls.Add(this.btnStop);
			this.grpEtc.Location = new System.Drawing.Point(199, 183);
			this.grpEtc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.grpEtc.Name = "grpEtc";
			this.grpEtc.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.grpEtc.Size = new System.Drawing.Size(249, 128);
			this.grpEtc.TabIndex = 288;
			this.grpEtc.TabStop = false;
			// 
			// btnCloseStages
			// 
			this.btnCloseStages.Location = new System.Drawing.Point(132, 72);
			this.btnCloseStages.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnCloseStages.Name = "btnCloseStages";
			this.btnCloseStages.Size = new System.Drawing.Size(111, 35);
			this.btnCloseStages.TabIndex = 288;
			this.btnCloseStages.Text = "CLOSE STAGES";
			this.btnCloseStages.UseVisualStyleBackColor = true;
			this.btnCloseStages.Click += new System.EventHandler(this.btnCloseStages_Click);
			// 
			// panelParam
			// 
			this.panelParam.Controls.Add(this.tabPage1);
			this.panelParam.Controls.Add(this.tabPage2);
			this.panelParam.Location = new System.Drawing.Point(5, 13);
			this.panelParam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.panelParam.Name = "panelParam";
			this.panelParam.SelectedIndex = 0;
			this.panelParam.Size = new System.Drawing.Size(188, 815);
			this.panelParam.TabIndex = 290;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage1.Controls.Add(this.groupAlignCheck);
			this.tabPage1.Controls.Add(this.chkMeasurement);
			this.tabPage1.Controls.Add(this.groupPmChannels);
			this.tabPage1.Controls.Add(this.groupAlignParams);
			this.tabPage1.Controls.Add(this.groupSave);
			this.tabPage1.Controls.Add(this.groupTls);
			this.tabPage1.Controls.Add(this.GroupPmGain);
			this.tabPage1.Location = new System.Drawing.Point(4, 24);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tabPage1.Size = new System.Drawing.Size(180, 787);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Configuration";
			// 
			// groupAlignCheck
			// 
			this.groupAlignCheck.Controls.Add(this.uiCladDistance);
			this.groupAlignCheck.Controls.Add(this.chkCenterStage);
			this.groupAlignCheck.Controls.Add(this.chkRoll);
			this.groupAlignCheck.Controls.Add(this.chkAlignment);
			this.groupAlignCheck.Controls.Add(this.uiMeasureCladding);
			this.groupAlignCheck.Controls.Add(this.chkFaArrangement);
			this.groupAlignCheck.Controls.Add(this.chkRetChip1Pos);
			this.groupAlignCheck.Location = new System.Drawing.Point(6, 617);
			this.groupAlignCheck.Name = "groupAlignCheck";
			this.groupAlignCheck.Size = new System.Drawing.Size(165, 135);
			this.groupAlignCheck.TabIndex = 314;
			this.groupAlignCheck.TabStop = false;
			this.groupAlignCheck.Text = "Aligner";
			// 
			// uiCladDistance
			// 
			this.uiCladDistance.BackColor = System.Drawing.Color.Black;
			this.uiCladDistance.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Neon.Dwdm.Properties.Settings.Default, "doClad_Distance", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.uiCladDistance.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiCladDistance.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiCladDistance.Location = new System.Drawing.Point(58, 83);
			this.uiCladDistance.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiCladDistance.Name = "uiCladDistance";
			this.uiCladDistance.Size = new System.Drawing.Size(45, 25);
			this.uiCladDistance.TabIndex = 329;
			this.uiCladDistance.Text = global::Neon.Dwdm.Properties.Settings.Default.doClad_Distance;
			this.uiCladDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.toolTip1.SetToolTip(this.uiCladDistance, "Clad Distance [㎛]");
			// 
			// chkCenterStage
			// 
			this.chkCenterStage.AutoSize = true;
			this.chkCenterStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkCenterStage.ForeColor = System.Drawing.Color.DarkRed;
			this.chkCenterStage.Location = new System.Drawing.Point(6, 23);
			this.chkCenterStage.Name = "chkCenterStage";
			this.chkCenterStage.Size = new System.Drawing.Size(150, 19);
			this.chkCenterStage.TabIndex = 315;
			this.chkCenterStage.Text = "Using Center Stage";
			this.chkCenterStage.UseVisualStyleBackColor = true;
			// 
			// chkRoll
			// 
			this.chkRoll.AutoSize = true;
			this.chkRoll.Checked = global::Neon.Dwdm.Properties.Settings.Default.doRollAlign;
			this.chkRoll.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doRollAlign", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkRoll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkRoll.Location = new System.Drawing.Point(106, 65);
			this.chkRoll.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkRoll.Name = "chkRoll";
			this.chkRoll.Size = new System.Drawing.Size(48, 19);
			this.chkRoll.TabIndex = 17;
			this.chkRoll.Text = "Roll";
			this.chkRoll.UseVisualStyleBackColor = true;
			// 
			// chkAlignment
			// 
			this.chkAlignment.AutoSize = true;
			this.chkAlignment.Checked = global::Neon.Dwdm.Properties.Settings.Default.doFineAlign;
			this.chkAlignment.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doFineAlign", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkAlignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAlignment.Location = new System.Drawing.Point(6, 65);
			this.chkAlignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkAlignment.Name = "chkAlignment";
			this.chkAlignment.Size = new System.Drawing.Size(80, 19);
			this.chkAlignment.TabIndex = 16;
			this.chkAlignment.Text = "Fine Align";
			this.chkAlignment.UseVisualStyleBackColor = true;
			// 
			// uiMeasureCladding
			// 
			this.uiMeasureCladding.AutoSize = true;
			this.uiMeasureCladding.Checked = global::Neon.Dwdm.Properties.Settings.Default.doClad;
			this.uiMeasureCladding.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doClad", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.uiMeasureCladding.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMeasureCladding.Location = new System.Drawing.Point(6, 86);
			this.uiMeasureCladding.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiMeasureCladding.Name = "uiMeasureCladding";
			this.uiMeasureCladding.Size = new System.Drawing.Size(51, 19);
			this.uiMeasureCladding.TabIndex = 328;
			this.uiMeasureCladding.Text = "Clad";
			this.uiMeasureCladding.UseVisualStyleBackColor = true;
			// 
			// chkFaArrangement
			// 
			this.chkFaArrangement.AutoSize = true;
			this.chkFaArrangement.Checked = global::Neon.Dwdm.Properties.Settings.Default.doFabAngleAlign;
			this.chkFaArrangement.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doFabAngleAlign", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkFaArrangement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkFaArrangement.Location = new System.Drawing.Point(6, 44);
			this.chkFaArrangement.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkFaArrangement.Name = "chkFaArrangement";
			this.chkFaArrangement.Size = new System.Drawing.Size(112, 19);
			this.chkFaArrangement.TabIndex = 15;
			this.chkFaArrangement.Text = "FAB Angle Align";
			this.chkFaArrangement.UseVisualStyleBackColor = true;
			// 
			// chkRetChip1Pos
			// 
			this.chkRetChip1Pos.AutoSize = true;
			this.chkRetChip1Pos.Checked = global::Neon.Dwdm.Properties.Settings.Default.doAutoReturn;
			this.chkRetChip1Pos.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doAutoReturn", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkRetChip1Pos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkRetChip1Pos.Location = new System.Drawing.Point(6, 107);
			this.chkRetChip1Pos.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkRetChip1Pos.Name = "chkRetChip1Pos";
			this.chkRetChip1Pos.Size = new System.Drawing.Size(90, 19);
			this.chkRetChip1Pos.TabIndex = 19;
			this.chkRetChip1Pos.Text = "Auto Return";
			this.chkRetChip1Pos.UseVisualStyleBackColor = true;
			// 
			// chkMeasurement
			// 
			this.chkMeasurement.AutoSize = true;
			this.chkMeasurement.Checked = global::Neon.Dwdm.Properties.Settings.Default.doMeasurement;
			this.chkMeasurement.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkMeasurement.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doMeasurement", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkMeasurement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkMeasurement.Location = new System.Drawing.Point(12, 756);
			this.chkMeasurement.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkMeasurement.Name = "chkMeasurement";
			this.chkMeasurement.Size = new System.Drawing.Size(103, 19);
			this.chkMeasurement.TabIndex = 18;
			this.chkMeasurement.Text = "Measurement";
			this.chkMeasurement.UseVisualStyleBackColor = true;
			// 
			// groupPmChannels
			// 
			this.groupPmChannels.Controls.Add(this.txtLoopStop);
			this.groupPmChannels.Controls.Add(this.txtLoopStart);
			this.groupPmChannels.Controls.Add(this.label42);
			this.groupPmChannels.Controls.Add(this.chkLoopScan);
			this.groupPmChannels.Controls.Add(this.txtPmPortStart);
			this.groupPmChannels.Controls.Add(this.label11);
			this.groupPmChannels.Controls.Add(this.txtPmPortStop);
			this.groupPmChannels.Controls.Add(this.chkChOrderReverse);
			this.groupPmChannels.Controls.Add(this.Label33);
			this.groupPmChannels.Location = new System.Drawing.Point(6, 190);
			this.groupPmChannels.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupPmChannels.Name = "groupPmChannels";
			this.groupPmChannels.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupPmChannels.Size = new System.Drawing.Size(165, 87);
			this.groupPmChannels.TabIndex = 313;
			this.groupPmChannels.TabStop = false;
			this.groupPmChannels.Text = "Channels";
			// 
			// txtLoopStop
			// 
			this.txtLoopStop.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLoopStop.ForeColor = System.Drawing.Color.DarkRed;
			this.txtLoopStop.Location = new System.Drawing.Point(117, 58);
			this.txtLoopStop.Name = "txtLoopStop";
			this.txtLoopStop.Size = new System.Drawing.Size(40, 23);
			this.txtLoopStop.TabIndex = 324;
			this.txtLoopStop.Text = "12";
			this.txtLoopStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtLoopStart
			// 
			this.txtLoopStart.Enabled = false;
			this.txtLoopStart.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLoopStart.ForeColor = System.Drawing.Color.DarkRed;
			this.txtLoopStart.Location = new System.Drawing.Point(63, 58);
			this.txtLoopStart.Name = "txtLoopStart";
			this.txtLoopStart.Size = new System.Drawing.Size(40, 23);
			this.txtLoopStart.TabIndex = 323;
			this.txtLoopStart.Text = "1";
			this.txtLoopStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label42
			// 
			this.label42.AutoSize = true;
			this.label42.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label42.Location = new System.Drawing.Point(103, 64);
			this.label42.Name = "label42";
			this.label42.Size = new System.Drawing.Size(14, 15);
			this.label42.TabIndex = 319;
			this.label42.Text = "~";
			// 
			// chkLoopScan
			// 
			this.chkLoopScan.AutoSize = true;
			this.chkLoopScan.Location = new System.Drawing.Point(11, 60);
			this.chkLoopScan.Name = "chkLoopScan";
			this.chkLoopScan.Size = new System.Drawing.Size(46, 19);
			this.chkLoopScan.TabIndex = 273;
			this.chkLoopScan.Text = "Dut";
			this.toolTip1.SetToolTip(this.chkLoopScan, "output FAB가 이동하면서 측정 DUT 측정 (PM 부족시)");
			this.chkLoopScan.UseVisualStyleBackColor = true;
			// 
			// txtPmPortStart
			// 
			this.txtPmPortStart.BackColor = System.Drawing.Color.Black;
			this.txtPmPortStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.txtPmPortStart.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPmPortStart.ForeColor = System.Drawing.Color.White;
			this.txtPmPortStart.FormattingEnabled = true;
			this.txtPmPortStart.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
			this.txtPmPortStart.Location = new System.Drawing.Point(63, 15);
			this.txtPmPortStart.Name = "txtPmPortStart";
			this.txtPmPortStart.Size = new System.Drawing.Size(40, 22);
			this.txtPmPortStart.TabIndex = 9;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(103, 22);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(14, 15);
			this.label11.TabIndex = 312;
			this.label11.Text = "~";
			// 
			// txtPmPortStop
			// 
			this.txtPmPortStop.BackColor = System.Drawing.Color.Black;
			this.txtPmPortStop.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.txtPmPortStop.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPmPortStop.ForeColor = System.Drawing.Color.White;
			this.txtPmPortStop.FormattingEnabled = true;
			this.txtPmPortStop.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
			this.txtPmPortStop.Location = new System.Drawing.Point(117, 15);
			this.txtPmPortStop.Name = "txtPmPortStop";
			this.txtPmPortStop.Size = new System.Drawing.Size(40, 22);
			this.txtPmPortStop.TabIndex = 10;
			// 
			// chkChOrderReverse
			// 
			this.chkChOrderReverse.AutoSize = true;
			this.chkChOrderReverse.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkChOrderReverse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkChOrderReverse.Location = new System.Drawing.Point(68, 39);
			this.chkChOrderReverse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkChOrderReverse.Name = "chkChOrderReverse";
			this.chkChOrderReverse.Size = new System.Drawing.Size(89, 19);
			this.chkChOrderReverse.TabIndex = 316;
			this.chkChOrderReverse.Text = "Reverse Ch";
			this.chkChOrderReverse.UseVisualStyleBackColor = true;
			// 
			// Label33
			// 
			this.Label33.AutoSize = true;
			this.Label33.Location = new System.Drawing.Point(9, 20);
			this.Label33.Name = "Label33";
			this.Label33.Size = new System.Drawing.Size(28, 15);
			this.Label33.TabIndex = 278;
			this.Label33.Text = "P.M";
			// 
			// groupAlignParams
			// 
			this.groupAlignParams.Controls.Add(this.txtChipWidth);
			this.groupAlignParams.Controls.Add(this.label10);
			this.groupAlignParams.Controls.Add(this.label25);
			this.groupAlignParams.Controls.Add(this.Label39);
			this.groupAlignParams.Controls.Add(this.txtAlignTimes);
			this.groupAlignParams.Controls.Add(this.txtChPitch);
			this.groupAlignParams.Controls.Add(this.label34);
			this.groupAlignParams.Controls.Add(this.Label36);
			this.groupAlignParams.Controls.Add(this.label31);
			this.groupAlignParams.Controls.Add(this.label29);
			this.groupAlignParams.Controls.Add(this.txtYBuffer);
			this.groupAlignParams.Controls.Add(this.txtZBuffer);
			this.groupAlignParams.Controls.Add(this.label32);
			this.groupAlignParams.Controls.Add(this.label8);
			this.groupAlignParams.Controls.Add(this.txtChipsPerRightAlign);
			this.groupAlignParams.Controls.Add(this.label30);
			this.groupAlignParams.Location = new System.Drawing.Point(6, 278);
			this.groupAlignParams.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupAlignParams.Name = "groupAlignParams";
			this.groupAlignParams.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupAlignParams.Size = new System.Drawing.Size(165, 167);
			this.groupAlignParams.TabIndex = 313;
			this.groupAlignParams.TabStop = false;
			this.groupAlignParams.Text = "Align Parameters";
			// 
			// txtChipWidth
			// 
			this.txtChipWidth.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtChipWidth.Location = new System.Drawing.Point(80, 18);
			this.txtChipWidth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtChipWidth.Name = "txtChipWidth";
			this.txtChipWidth.Size = new System.Drawing.Size(49, 22);
			this.txtChipWidth.TabIndex = 11;
			this.txtChipWidth.Text = "15500";
			this.txtChipWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(134, 22);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(25, 15);
			this.label10.TabIndex = 311;
			this.label10.Text = "μm";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label25
			// 
			this.label25.AutoSize = true;
			this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label25.Location = new System.Drawing.Point(134, 45);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(25, 15);
			this.label25.TabIndex = 311;
			this.label25.Text = "μm";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Label39
			// 
			this.Label39.AutoSize = true;
			this.Label39.Location = new System.Drawing.Point(9, 22);
			this.Label39.Name = "Label39";
			this.Label39.Size = new System.Drawing.Size(68, 15);
			this.Label39.TabIndex = 283;
			this.Label39.Text = "Chip Width";
			// 
			// txtAlignTimes
			// 
			this.txtAlignTimes.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtAlignTimes.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtAlignTimes.Location = new System.Drawing.Point(80, 136);
			this.txtAlignTimes.Name = "txtAlignTimes";
			this.txtAlignTimes.Size = new System.Drawing.Size(49, 23);
			this.txtAlignTimes.TabIndex = 327;
			this.txtAlignTimes.Text = "1";
			this.txtAlignTimes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtChPitch
			// 
			this.txtChPitch.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtChPitch.FormattingEnabled = true;
			this.txtChPitch.Items.AddRange(new object[] {
            "127",
            "250",
            "750"});
			this.txtChPitch.Location = new System.Drawing.Point(80, 41);
			this.txtChPitch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtChPitch.Name = "txtChPitch";
			this.txtChPitch.Size = new System.Drawing.Size(49, 22);
			this.txtChPitch.TabIndex = 12;
			this.txtChPitch.Text = "127";
			// 
			// label34
			// 
			this.label34.AutoSize = true;
			this.label34.Location = new System.Drawing.Point(9, 140);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(70, 15);
			this.label34.TabIndex = 326;
			this.label34.Text = "Align Times";
			// 
			// Label36
			// 
			this.Label36.AutoSize = true;
			this.Label36.Location = new System.Drawing.Point(9, 45);
			this.Label36.Name = "Label36";
			this.Label36.Size = new System.Drawing.Size(55, 15);
			this.Label36.TabIndex = 279;
			this.Label36.Text = "CH Pitch";
			// 
			// label31
			// 
			this.label31.AutoSize = true;
			this.label31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label31.Location = new System.Drawing.Point(134, 93);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(25, 15);
			this.label31.TabIndex = 325;
			this.label31.Text = "μm";
			this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label29
			// 
			this.label29.AutoSize = true;
			this.label29.Location = new System.Drawing.Point(9, 69);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(50, 15);
			this.label29.TabIndex = 318;
			this.label29.Text = "Z buffer";
			// 
			// txtYBuffer
			// 
			this.txtYBuffer.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtYBuffer.Location = new System.Drawing.Point(80, 89);
			this.txtYBuffer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtYBuffer.Name = "txtYBuffer";
			this.txtYBuffer.Size = new System.Drawing.Size(49, 22);
			this.txtYBuffer.TabIndex = 324;
			this.txtYBuffer.Text = "0";
			this.txtYBuffer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtZBuffer
			// 
			this.txtZBuffer.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtZBuffer.Location = new System.Drawing.Point(80, 65);
			this.txtZBuffer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtZBuffer.Name = "txtZBuffer";
			this.txtZBuffer.Size = new System.Drawing.Size(49, 22);
			this.txtZBuffer.TabIndex = 319;
			this.txtZBuffer.Text = "60";
			this.txtZBuffer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label32
			// 
			this.label32.AutoSize = true;
			this.label32.Location = new System.Drawing.Point(9, 93);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(38, 15);
			this.label32.TabIndex = 323;
			this.label32.Text = "Y axis";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(134, 69);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(25, 15);
			this.label8.TabIndex = 320;
			this.label8.Text = "μm";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtChipsPerRightAlign
			// 
			this.txtChipsPerRightAlign.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtChipsPerRightAlign.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtChipsPerRightAlign.Location = new System.Drawing.Point(80, 112);
			this.txtChipsPerRightAlign.Name = "txtChipsPerRightAlign";
			this.txtChipsPerRightAlign.Size = new System.Drawing.Size(49, 23);
			this.txtChipsPerRightAlign.TabIndex = 322;
			this.txtChipsPerRightAlign.Text = "1";
			this.txtChipsPerRightAlign.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label30
			// 
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(9, 116);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(62, 15);
			this.label30.TabIndex = 321;
			this.label30.Text = "OutAlign#";
			// 
			// groupSave
			// 
			this.groupSave.Controls.Add(this.btnSaveFolder);
			this.groupSave.Controls.Add(this.txtSaveWaveStop);
			this.groupSave.Controls.Add(this.txtSaveWaveStart);
			this.groupSave.Controls.Add(this.rbtnAutoSaveRng);
			this.groupSave.Controls.Add(this.label44);
			this.groupSave.Controls.Add(this.rbtnAutoSaveFull);
			this.groupSave.Controls.Add(this.chkAutoSave);
			this.groupSave.Controls.Add(this.chkWaferFolder);
			this.groupSave.Location = new System.Drawing.Point(6, 446);
			this.groupSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupSave.Name = "groupSave";
			this.groupSave.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupSave.Size = new System.Drawing.Size(165, 169);
			this.groupSave.TabIndex = 312;
			this.groupSave.TabStop = false;
			this.groupSave.Text = "Save";
			// 
			// btnSaveFolder
			// 
			this.btnSaveFolder.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
			this.btnSaveFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSaveFolder.ForeColor = System.Drawing.Color.DodgerBlue;
			this.btnSaveFolder.Location = new System.Drawing.Point(6, 107);
			this.btnSaveFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSaveFolder.Name = "btnSaveFolder";
			this.btnSaveFolder.Size = new System.Drawing.Size(153, 53);
			this.btnSaveFolder.TabIndex = 293;
			this.btnSaveFolder.Text = "Save folder";
			this.btnSaveFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSaveFolder.UseVisualStyleBackColor = true;
			this.btnSaveFolder.Click += new System.EventHandler(this.btnSaveFolder_Click);
			// 
			// txtSaveWaveStop
			// 
			this.txtSaveWaveStop.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSaveWaveStop.ForeColor = System.Drawing.Color.Tomato;
			this.txtSaveWaveStop.Location = new System.Drawing.Point(119, 80);
			this.txtSaveWaveStop.Name = "txtSaveWaveStop";
			this.txtSaveWaveStop.Size = new System.Drawing.Size(40, 23);
			this.txtSaveWaveStop.TabIndex = 351;
			this.txtSaveWaveStop.Text = "1570";
			this.txtSaveWaveStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtSaveWaveStart
			// 
			this.txtSaveWaveStart.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSaveWaveStart.ForeColor = System.Drawing.Color.Tomato;
			this.txtSaveWaveStart.Location = new System.Drawing.Point(57, 80);
			this.txtSaveWaveStart.Name = "txtSaveWaveStart";
			this.txtSaveWaveStart.Size = new System.Drawing.Size(40, 23);
			this.txtSaveWaveStart.TabIndex = 350;
			this.txtSaveWaveStart.Text = "1520";
			this.txtSaveWaveStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// rbtnAutoSaveRng
			// 
			this.rbtnAutoSaveRng.AutoSize = true;
			this.rbtnAutoSaveRng.Location = new System.Drawing.Point(57, 60);
			this.rbtnAutoSaveRng.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.rbtnAutoSaveRng.Name = "rbtnAutoSaveRng";
			this.rbtnAutoSaveRng.Size = new System.Drawing.Size(58, 19);
			this.rbtnAutoSaveRng.TabIndex = 316;
			this.rbtnAutoSaveRng.Text = "Range";
			this.rbtnAutoSaveRng.UseVisualStyleBackColor = true;
			// 
			// label44
			// 
			this.label44.AutoSize = true;
			this.label44.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label44.Location = new System.Drawing.Point(101, 86);
			this.label44.Name = "label44";
			this.label44.Size = new System.Drawing.Size(14, 15);
			this.label44.TabIndex = 352;
			this.label44.Text = "~";
			// 
			// rbtnAutoSaveFull
			// 
			this.rbtnAutoSaveFull.AutoSize = true;
			this.rbtnAutoSaveFull.Checked = true;
			this.rbtnAutoSaveFull.Location = new System.Drawing.Point(12, 60);
			this.rbtnAutoSaveFull.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.rbtnAutoSaveFull.Name = "rbtnAutoSaveFull";
			this.rbtnAutoSaveFull.Size = new System.Drawing.Size(44, 19);
			this.rbtnAutoSaveFull.TabIndex = 315;
			this.rbtnAutoSaveFull.TabStop = true;
			this.rbtnAutoSaveFull.Text = "Full";
			this.rbtnAutoSaveFull.UseVisualStyleBackColor = true;
			// 
			// chkAutoSave
			// 
			this.chkAutoSave.AutoSize = true;
			this.chkAutoSave.Checked = global::Neon.Dwdm.Properties.Settings.Default.doAutoSave;
			this.chkAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkAutoSave.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doAutoSave", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkAutoSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAutoSave.Location = new System.Drawing.Point(12, 20);
			this.chkAutoSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkAutoSave.Name = "chkAutoSave";
			this.chkAutoSave.Size = new System.Drawing.Size(77, 19);
			this.chkAutoSave.TabIndex = 13;
			this.chkAutoSave.Text = "AutoSave";
			this.chkAutoSave.UseVisualStyleBackColor = true;
			// 
			// chkWaferFolder
			// 
			this.chkWaferFolder.AutoSize = true;
			this.chkWaferFolder.Checked = true;
			this.chkWaferFolder.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkWaferFolder.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.chkWaferFolder.Location = new System.Drawing.Point(12, 40);
			this.chkWaferFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkWaferFolder.Name = "chkWaferFolder";
			this.chkWaferFolder.Size = new System.Drawing.Size(132, 19);
			this.chkWaferFolder.TabIndex = 314;
			this.chkWaferFolder.Text = "Create Wafer Folder";
			this.chkWaferFolder.UseVisualStyleBackColor = true;
			// 
			// groupTls
			// 
			this.groupTls.Controls.Add(this.txtTlsNoiseShift);
			this.groupTls.Controls.Add(this.label40);
			this.groupTls.Controls.Add(this.label41);
			this.groupTls.Controls.Add(this.txtTlsSNR);
			this.groupTls.Controls.Add(this.label35);
			this.groupTls.Controls.Add(this.label38);
			this.groupTls.Controls.Add(this.txtWaveStep);
			this.groupTls.Controls.Add(this.txtTlsPower);
			this.groupTls.Controls.Add(this.txtWaveStop);
			this.groupTls.Controls.Add(this.txtWaveStart);
			this.groupTls.Controls.Add(this.label18);
			this.groupTls.Controls.Add(this.label4);
			this.groupTls.Controls.Add(this.lbOptRng);
			this.groupTls.Controls.Add(this.label1);
			this.groupTls.Location = new System.Drawing.Point(6, 69);
			this.groupTls.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupTls.Name = "groupTls";
			this.groupTls.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupTls.Size = new System.Drawing.Size(165, 119);
			this.groupTls.TabIndex = 281;
			this.groupTls.TabStop = false;
			this.groupTls.Text = "TLS";
			// 
			// txtTlsNoiseShift
			// 
			this.txtTlsNoiseShift.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsNoiseShift.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtTlsNoiseShift.Location = new System.Drawing.Point(83, 92);
			this.txtTlsNoiseShift.Name = "txtTlsNoiseShift";
			this.txtTlsNoiseShift.Size = new System.Drawing.Size(40, 23);
			this.txtTlsNoiseShift.TabIndex = 347;
			this.txtTlsNoiseShift.Text = "-3";
			this.txtTlsNoiseShift.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label40
			// 
			this.label40.AutoSize = true;
			this.label40.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label40.Location = new System.Drawing.Point(12, 96);
			this.label40.Name = "label40";
			this.label40.Size = new System.Drawing.Size(66, 15);
			this.label40.TabIndex = 348;
			this.label40.Text = "Noise Shift";
			this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label41
			// 
			this.label41.AutoSize = true;
			this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label41.Location = new System.Drawing.Point(128, 96);
			this.label41.Name = "label41";
			this.label41.Size = new System.Drawing.Size(22, 15);
			this.label41.TabIndex = 349;
			this.label41.Text = "dB";
			this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtTlsSNR
			// 
			this.txtTlsSNR.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsSNR.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtTlsSNR.Location = new System.Drawing.Point(83, 68);
			this.txtTlsSNR.Name = "txtTlsSNR";
			this.txtTlsSNR.Size = new System.Drawing.Size(40, 23);
			this.txtTlsSNR.TabIndex = 344;
			this.txtTlsSNR.Text = "-40";
			this.txtTlsSNR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label35
			// 
			this.label35.AutoSize = true;
			this.label35.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label35.Location = new System.Drawing.Point(12, 72);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(30, 15);
			this.label35.TabIndex = 345;
			this.label35.Text = "SNR";
			this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label38
			// 
			this.label38.AutoSize = true;
			this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label38.Location = new System.Drawing.Point(128, 72);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(22, 15);
			this.label38.TabIndex = 346;
			this.label38.Text = "dB";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtWaveStep
			// 
			this.txtWaveStep.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtWaveStep.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtWaveStep.Location = new System.Drawing.Point(116, 19);
			this.txtWaveStep.Name = "txtWaveStep";
			this.txtWaveStep.Size = new System.Drawing.Size(45, 23);
			this.txtWaveStep.TabIndex = 343;
			this.txtWaveStep.Text = "20";
			this.txtWaveStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtTlsPower
			// 
			this.txtTlsPower.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsPower.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtTlsPower.Location = new System.Drawing.Point(83, 44);
			this.txtTlsPower.Name = "txtTlsPower";
			this.txtTlsPower.ReadOnly = true;
			this.txtTlsPower.Size = new System.Drawing.Size(40, 23);
			this.txtTlsPower.TabIndex = 338;
			this.txtTlsPower.Text = "-15";
			this.txtTlsPower.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtWaveStop
			// 
			this.txtWaveStop.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtWaveStop.ForeColor = System.Drawing.Color.Tomato;
			this.txtWaveStop.Location = new System.Drawing.Point(70, 19);
			this.txtWaveStop.Name = "txtWaveStop";
			this.txtWaveStop.Size = new System.Drawing.Size(40, 23);
			this.txtWaveStop.TabIndex = 337;
			this.txtWaveStop.Text = "1570";
			this.txtWaveStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtWaveStart
			// 
			this.txtWaveStart.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtWaveStart.ForeColor = System.Drawing.Color.Tomato;
			this.txtWaveStart.Location = new System.Drawing.Point(21, 19);
			this.txtWaveStart.Name = "txtWaveStart";
			this.txtWaveStart.Size = new System.Drawing.Size(40, 23);
			this.txtWaveStart.TabIndex = 336;
			this.txtWaveStart.Text = "1520";
			this.txtWaveStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label18.Location = new System.Drawing.Point(59, 23);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(14, 15);
			this.label18.TabIndex = 340;
			this.label18.Text = "~";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(12, 48);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(40, 15);
			this.label4.TabIndex = 341;
			this.label4.Text = "Power";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lbOptRng
			// 
			this.lbOptRng.AutoSize = true;
			this.lbOptRng.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbOptRng.Location = new System.Drawing.Point(4, 23);
			this.lbOptRng.Name = "lbOptRng";
			this.lbOptRng.Size = new System.Drawing.Size(24, 15);
			this.lbOptRng.TabIndex = 339;
			this.lbOptRng.Text = "λ : ";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(128, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 15);
			this.label1.TabIndex = 342;
			this.label1.Text = "dBm";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// GroupPmGain
			// 
			this.GroupPmGain.Controls.Add(this.rbtnGain2);
			this.GroupPmGain.Controls.Add(this.rbtnGain1);
			this.GroupPmGain.Controls.Add(this.txtPmGainLevel2);
			this.GroupPmGain.Controls.Add(this.txtPmGainLevel1);
			this.GroupPmGain.Controls.Add(this.label9);
			this.GroupPmGain.Location = new System.Drawing.Point(6, 5);
			this.GroupPmGain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.GroupPmGain.Name = "GroupPmGain";
			this.GroupPmGain.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.GroupPmGain.Size = new System.Drawing.Size(165, 63);
			this.GroupPmGain.TabIndex = 280;
			this.GroupPmGain.TabStop = false;
			this.GroupPmGain.Text = "PM Gain Levels";
			// 
			// rbtnGain2
			// 
			this.rbtnGain2.AutoSize = true;
			this.rbtnGain2.Location = new System.Drawing.Point(72, 15);
			this.rbtnGain2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.rbtnGain2.Name = "rbtnGain2";
			this.rbtnGain2.Size = new System.Drawing.Size(32, 19);
			this.rbtnGain2.TabIndex = 2;
			this.rbtnGain2.TabStop = true;
			this.rbtnGain2.Text = "2";
			this.rbtnGain2.UseVisualStyleBackColor = true;
			// 
			// rbtnGain1
			// 
			this.rbtnGain1.AutoSize = true;
			this.rbtnGain1.Location = new System.Drawing.Point(19, 15);
			this.rbtnGain1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.rbtnGain1.Name = "rbtnGain1";
			this.rbtnGain1.Size = new System.Drawing.Size(32, 19);
			this.rbtnGain1.TabIndex = 1;
			this.rbtnGain1.TabStop = true;
			this.rbtnGain1.Text = "1";
			this.rbtnGain1.UseVisualStyleBackColor = true;
			// 
			// txtPmGainLevel2
			// 
			this.txtPmGainLevel2.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPmGainLevel2.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtPmGainLevel2.Location = new System.Drawing.Point(72, 34);
			this.txtPmGainLevel2.Name = "txtPmGainLevel2";
			this.txtPmGainLevel2.ReadOnly = true;
			this.txtPmGainLevel2.Size = new System.Drawing.Size(40, 23);
			this.txtPmGainLevel2.TabIndex = 4;
			this.txtPmGainLevel2.Text = "-10";
			this.txtPmGainLevel2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtPmGainLevel1
			// 
			this.txtPmGainLevel1.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPmGainLevel1.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtPmGainLevel1.Location = new System.Drawing.Point(19, 34);
			this.txtPmGainLevel1.Name = "txtPmGainLevel1";
			this.txtPmGainLevel1.ReadOnly = true;
			this.txtPmGainLevel1.Size = new System.Drawing.Size(40, 23);
			this.txtPmGainLevel1.TabIndex = 3;
			this.txtPmGainLevel1.Text = "-10";
			this.txtPmGainLevel1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(120, 37);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(33, 15);
			this.label9.TabIndex = 311;
			this.label9.Text = "dBm";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Controls.Add(this.groupBox5);
			this.tabPage2.Controls.Add(this.groupBox3);
			this.tabPage2.Location = new System.Drawing.Point(4, 24);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.tabPage2.Size = new System.Drawing.Size(180, 787);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Position";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.chkBackupPower);
			this.groupBox1.Controls.Add(this.chkBackupWave);
			this.groupBox1.Location = new System.Drawing.Point(3, 703);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(171, 77);
			this.groupBox1.TabIndex = 220;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Backup Options";
			// 
			// chkBackupPower
			// 
			this.chkBackupPower.AutoSize = true;
			this.chkBackupPower.Checked = global::Neon.Dwdm.Properties.Settings.Default.backup_Power;
			this.chkBackupPower.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "backup_Power", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkBackupPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBackupPower.Location = new System.Drawing.Point(11, 50);
			this.chkBackupPower.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkBackupPower.Name = "chkBackupPower";
			this.chkBackupPower.Size = new System.Drawing.Size(61, 19);
			this.chkBackupPower.TabIndex = 20;
			this.chkBackupPower.Text = "Power";
			this.chkBackupPower.UseVisualStyleBackColor = true;
			// 
			// chkBackupWave
			// 
			this.chkBackupWave.AutoSize = true;
			this.chkBackupWave.Checked = global::Neon.Dwdm.Properties.Settings.Default.backup_Wave;
			this.chkBackupWave.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "backup_Wave", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.chkBackupWave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBackupWave.Location = new System.Drawing.Point(11, 23);
			this.chkBackupWave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.chkBackupWave.Name = "chkBackupWave";
			this.chkBackupWave.Size = new System.Drawing.Size(56, 19);
			this.chkBackupWave.TabIndex = 19;
			this.chkBackupWave.Text = "Wave";
			this.chkBackupWave.UseVisualStyleBackColor = true;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.btnClosedPosGo);
			this.groupBox5.Controls.Add(this.btnSaveAsClosePosition);
			this.groupBox5.Controls.Add(this.label21);
			this.groupBox5.Controls.Add(this.label23);
			this.groupBox5.Controls.Add(this.lbClosePosOutZ);
			this.groupBox5.Controls.Add(this.label14);
			this.groupBox5.Controls.Add(this.label17);
			this.groupBox5.Controls.Add(this.lbClosePosInZ);
			this.groupBox5.Location = new System.Drawing.Point(3, 410);
			this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox5.Size = new System.Drawing.Size(171, 195);
			this.groupBox5.TabIndex = 219;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Close-position";
			// 
			// btnClosedPosGo
			// 
			this.btnClosedPosGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnClosedPosGo.Location = new System.Drawing.Point(6, 136);
			this.btnClosedPosGo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnClosedPosGo.Name = "btnClosedPosGo";
			this.btnClosedPosGo.Size = new System.Drawing.Size(152, 51);
			this.btnClosedPosGo.TabIndex = 202;
			this.btnClosedPosGo.Text = "GO";
			this.btnClosedPosGo.UseVisualStyleBackColor = true;
			this.btnClosedPosGo.Click += new System.EventHandler(this.btnClosedPosGo_Click);
			// 
			// btnSaveAsClosePosition
			// 
			this.btnSaveAsClosePosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSaveAsClosePosition.Location = new System.Drawing.Point(6, 96);
			this.btnSaveAsClosePosition.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSaveAsClosePosition.Name = "btnSaveAsClosePosition";
			this.btnSaveAsClosePosition.Size = new System.Drawing.Size(152, 32);
			this.btnSaveAsClosePosition.TabIndex = 201;
			this.btnSaveAsClosePosition.Text = "Read && Save";
			this.btnSaveAsClosePosition.UseVisualStyleBackColor = true;
			this.btnSaveAsClosePosition.Click += new System.EventHandler(this.btnClosedPosApply_Click);
			// 
			// label21
			// 
			this.label21.AutoSize = true;
			this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label21.Location = new System.Drawing.Point(8, 51);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(53, 15);
			this.label21.TabIndex = 198;
			this.label21.Text = "Output ";
			// 
			// label23
			// 
			this.label23.AutoSize = true;
			this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label23.Location = new System.Drawing.Point(67, 51);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(20, 15);
			this.label23.TabIndex = 199;
			this.label23.Text = "Z :";
			// 
			// lbClosePosOutZ
			// 
			this.lbClosePosOutZ.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbClosePosOutZ.Location = new System.Drawing.Point(105, 51);
			this.lbClosePosOutZ.Name = "lbClosePosOutZ";
			this.lbClosePosOutZ.Size = new System.Drawing.Size(47, 21);
			this.lbClosePosOutZ.TabIndex = 200;
			this.lbClosePosOutZ.Text = "00000";
			this.lbClosePosOutZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label14.Location = new System.Drawing.Point(8, 25);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(43, 15);
			this.label14.TabIndex = 195;
			this.label14.Text = "Input ";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.Location = new System.Drawing.Point(67, 25);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(20, 15);
			this.label17.TabIndex = 196;
			this.label17.Text = "Z :";
			// 
			// lbClosePosInZ
			// 
			this.lbClosePosInZ.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbClosePosInZ.Location = new System.Drawing.Point(105, 25);
			this.lbClosePosInZ.Name = "lbClosePosInZ";
			this.lbClosePosInZ.Size = new System.Drawing.Size(47, 21);
			this.lbClosePosInZ.TabIndex = 197;
			this.lbClosePosInZ.Text = "00000";
			this.lbClosePosInZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.lbl_Init_Center_Y);
			this.groupBox3.Controls.Add(this.label27);
			this.groupBox3.Controls.Add(this.label43);
			this.groupBox3.Controls.Add(this.lbl_Init_Center_X);
			this.groupBox3.Controls.Add(this.label45);
			this.groupBox3.Controls.Add(this.btnInitPosGo);
			this.groupBox3.Controls.Add(this.lbInitPosOutTz);
			this.groupBox3.Controls.Add(this.btnSaveAsInitPosition);
			this.groupBox3.Controls.Add(this.label28);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.lbInitPosOutTy);
			this.groupBox3.Controls.Add(this.label20);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.lbInitPosInX);
			this.groupBox3.Controls.Add(this.lbInitPosOutTx);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.lbInitPosInY);
			this.groupBox3.Controls.Add(this.lbInitPosOutZ);
			this.groupBox3.Controls.Add(this.label19);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.lbInitPosInZ);
			this.groupBox3.Controls.Add(this.lbInitPosOutY);
			this.groupBox3.Controls.Add(this.label26);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.lbInitPosInTx);
			this.groupBox3.Controls.Add(this.label37);
			this.groupBox3.Controls.Add(this.label24);
			this.groupBox3.Controls.Add(this.lbInitPosOutX);
			this.groupBox3.Controls.Add(this.lbInitPosInTy);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.label22);
			this.groupBox3.Controls.Add(this.lbInitPosInTz);
			this.groupBox3.Location = new System.Drawing.Point(2, 8);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox3.Size = new System.Drawing.Size(172, 392);
			this.groupBox3.TabIndex = 218;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Initial Postion";
			// 
			// lbl_Init_Center_Y
			// 
			this.lbl_Init_Center_Y.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Init_Center_Y.Location = new System.Drawing.Point(40, 246);
			this.lbl_Init_Center_Y.Name = "lbl_Init_Center_Y";
			this.lbl_Init_Center_Y.Size = new System.Drawing.Size(47, 21);
			this.lbl_Init_Center_Y.TabIndex = 220;
			this.lbl_Init_Center_Y.Text = "00000";
			this.lbl_Init_Center_Y.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label27
			// 
			this.label27.AutoSize = true;
			this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label27.Location = new System.Drawing.Point(18, 249);
			this.label27.Name = "label27";
			this.label27.Size = new System.Drawing.Size(20, 15);
			this.label27.TabIndex = 219;
			this.label27.Text = "Y :";
			// 
			// label43
			// 
			this.label43.AutoSize = true;
			this.label43.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label43.Location = new System.Drawing.Point(7, 204);
			this.label43.Name = "label43";
			this.label43.Size = new System.Drawing.Size(49, 15);
			this.label43.TabIndex = 218;
			this.label43.Text = "Center";
			// 
			// lbl_Init_Center_X
			// 
			this.lbl_Init_Center_X.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbl_Init_Center_X.Location = new System.Drawing.Point(40, 225);
			this.lbl_Init_Center_X.Name = "lbl_Init_Center_X";
			this.lbl_Init_Center_X.Size = new System.Drawing.Size(47, 21);
			this.lbl_Init_Center_X.TabIndex = 217;
			this.lbl_Init_Center_X.Text = "00000";
			this.lbl_Init_Center_X.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label45
			// 
			this.label45.AutoSize = true;
			this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label45.Location = new System.Drawing.Point(18, 228);
			this.label45.Name = "label45";
			this.label45.Size = new System.Drawing.Size(21, 15);
			this.label45.TabIndex = 216;
			this.label45.Text = "X :";
			// 
			// btnInitPosGo
			// 
			this.btnInitPosGo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnInitPosGo.Location = new System.Drawing.Point(6, 326);
			this.btnInitPosGo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnInitPosGo.Name = "btnInitPosGo";
			this.btnInitPosGo.Size = new System.Drawing.Size(152, 43);
			this.btnInitPosGo.TabIndex = 215;
			this.btnInitPosGo.Text = "GO";
			this.btnInitPosGo.UseVisualStyleBackColor = true;
			this.btnInitPosGo.Click += new System.EventHandler(this.btnInitPosGo_Click);
			// 
			// lbInitPosOutTz
			// 
			this.lbInitPosOutTz.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutTz.Location = new System.Drawing.Point(117, 173);
			this.lbInitPosOutTz.Name = "lbInitPosOutTz";
			this.lbInitPosOutTz.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutTz.TabIndex = 214;
			this.lbInitPosOutTz.Text = "00000";
			this.lbInitPosOutTz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnSaveAsInitPosition
			// 
			this.btnSaveAsInitPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSaveAsInitPosition.Location = new System.Drawing.Point(6, 278);
			this.btnSaveAsInitPosition.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSaveAsInitPosition.Name = "btnSaveAsInitPosition";
			this.btnSaveAsInitPosition.Size = new System.Drawing.Size(152, 36);
			this.btnSaveAsInitPosition.TabIndex = 200;
			this.btnSaveAsInitPosition.Text = "Read && Save";
			this.btnSaveAsInitPosition.UseVisualStyleBackColor = true;
			this.btnSaveAsInitPosition.Click += new System.EventHandler(this.btnInitPosApply_Click);
			// 
			// label28
			// 
			this.label28.AutoSize = true;
			this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label28.Location = new System.Drawing.Point(96, 175);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(27, 15);
			this.label28.TabIndex = 213;
			this.label28.Text = "TZ :";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.Location = new System.Drawing.Point(9, 18);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(43, 15);
			this.label13.TabIndex = 190;
			this.label13.Text = "Input ";
			// 
			// lbInitPosOutTy
			// 
			this.lbInitPosOutTy.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutTy.Location = new System.Drawing.Point(117, 150);
			this.lbInitPosOutTy.Name = "lbInitPosOutTy";
			this.lbInitPosOutTy.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutTy.TabIndex = 212;
			this.lbInitPosOutTy.Text = "00000";
			this.lbInitPosOutTy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.AutoSize = true;
			this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label20.Location = new System.Drawing.Point(18, 42);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(21, 15);
			this.label20.TabIndex = 188;
			this.label20.Text = "X :";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(96, 152);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(27, 15);
			this.label2.TabIndex = 211;
			this.label2.Text = "TY :";
			// 
			// lbInitPosInX
			// 
			this.lbInitPosInX.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInX.Location = new System.Drawing.Point(40, 39);
			this.lbInitPosInX.Name = "lbInitPosInX";
			this.lbInitPosInX.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInX.TabIndex = 189;
			this.lbInitPosInX.Text = "00000";
			this.lbInitPosInX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbInitPosOutTx
			// 
			this.lbInitPosOutTx.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutTx.Location = new System.Drawing.Point(117, 128);
			this.lbInitPosOutTx.Name = "lbInitPosOutTx";
			this.lbInitPosOutTx.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutTx.TabIndex = 210;
			this.lbInitPosOutTx.Text = "00000";
			this.lbInitPosOutTx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(18, 65);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(20, 15);
			this.label12.TabIndex = 191;
			this.label12.Text = "Y :";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(96, 130);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(28, 15);
			this.label3.TabIndex = 209;
			this.label3.Text = "TX :";
			// 
			// lbInitPosInY
			// 
			this.lbInitPosInY.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInY.Location = new System.Drawing.Point(40, 62);
			this.lbInitPosInY.Name = "lbInitPosInY";
			this.lbInitPosInY.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInY.TabIndex = 192;
			this.lbInitPosInY.Text = "00000";
			this.lbInitPosInY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbInitPosOutZ
			// 
			this.lbInitPosOutZ.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutZ.Location = new System.Drawing.Point(40, 175);
			this.lbInitPosOutZ.Name = "lbInitPosOutZ";
			this.lbInitPosOutZ.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutZ.TabIndex = 208;
			this.lbInitPosOutZ.Text = "00000";
			this.lbInitPosOutZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label19.Location = new System.Drawing.Point(18, 87);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(20, 15);
			this.label19.TabIndex = 193;
			this.label19.Text = "Z :";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(18, 175);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(20, 15);
			this.label5.TabIndex = 207;
			this.label5.Text = "Z :";
			// 
			// lbInitPosInZ
			// 
			this.lbInitPosInZ.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInZ.Location = new System.Drawing.Point(40, 84);
			this.lbInitPosInZ.Name = "lbInitPosInZ";
			this.lbInitPosInZ.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInZ.TabIndex = 194;
			this.lbInitPosInZ.Text = "00000";
			this.lbInitPosInZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbInitPosOutY
			// 
			this.lbInitPosOutY.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutY.Location = new System.Drawing.Point(40, 152);
			this.lbInitPosOutY.Name = "lbInitPosOutY";
			this.lbInitPosOutY.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutY.TabIndex = 206;
			this.lbInitPosOutY.Text = "00000";
			this.lbInitPosOutY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label26
			// 
			this.label26.AutoSize = true;
			this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label26.Location = new System.Drawing.Point(95, 42);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(28, 15);
			this.label26.TabIndex = 195;
			this.label26.Text = "TX :";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(18, 152);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(20, 15);
			this.label6.TabIndex = 205;
			this.label6.Text = "Y :";
			// 
			// lbInitPosInTx
			// 
			this.lbInitPosInTx.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInTx.Location = new System.Drawing.Point(117, 40);
			this.lbInitPosInTx.Name = "lbInitPosInTx";
			this.lbInitPosInTx.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInTx.TabIndex = 196;
			this.lbInitPosInTx.Text = "00000";
			this.lbInitPosInTx.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label37
			// 
			this.label37.AutoSize = true;
			this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label37.Location = new System.Drawing.Point(7, 106);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(53, 15);
			this.label37.TabIndex = 204;
			this.label37.Text = "Output ";
			// 
			// label24
			// 
			this.label24.AutoSize = true;
			this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label24.Location = new System.Drawing.Point(95, 65);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(27, 15);
			this.label24.TabIndex = 197;
			this.label24.Text = "TY :";
			// 
			// lbInitPosOutX
			// 
			this.lbInitPosOutX.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosOutX.Location = new System.Drawing.Point(40, 130);
			this.lbInitPosOutX.Name = "lbInitPosOutX";
			this.lbInitPosOutX.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosOutX.TabIndex = 203;
			this.lbInitPosOutX.Text = "00000";
			this.lbInitPosOutX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// lbInitPosInTy
			// 
			this.lbInitPosInTy.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInTy.Location = new System.Drawing.Point(117, 63);
			this.lbInitPosInTy.Name = "lbInitPosInTy";
			this.lbInitPosInTy.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInTy.TabIndex = 198;
			this.lbInitPosInTy.Text = "00000";
			this.lbInitPosInTy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(18, 130);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(21, 15);
			this.label7.TabIndex = 202;
			this.label7.Text = "X :";
			// 
			// label22
			// 
			this.label22.AutoSize = true;
			this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label22.Location = new System.Drawing.Point(95, 87);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(27, 15);
			this.label22.TabIndex = 199;
			this.label22.Text = "TZ :";
			// 
			// lbInitPosInTz
			// 
			this.lbInitPosInTz.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbInitPosInTz.Location = new System.Drawing.Point(117, 85);
			this.lbInitPosInTz.Name = "lbInitPosInTz";
			this.lbInitPosInTz.Size = new System.Drawing.Size(47, 21);
			this.lbInitPosInTz.TabIndex = 201;
			this.lbInitPosInTz.Text = "00000";
			this.lbInitPosInTz.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// WaveformPlot1
			// 
			this.WaveformPlot1.HistoryCapacity = 20000;
			this.WaveformPlot1.LineColor = System.Drawing.Color.LightGoldenrodYellow;
			this.WaveformPlot1.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
			this.WaveformPlot1.XAxis = this.XAxis1;
			this.WaveformPlot1.YAxis = this.YAxis1;
			// 
			// XAxis1
			// 
			this.XAxis1.AutoMinorDivisionFrequency = 5;
			this.XAxis1.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.XAxis1.MajorDivisions.GridVisible = true;
			this.XAxis1.MinorDivisions.GridVisible = true;
			this.XAxis1.Mode = NationalInstruments.UI.AxisMode.AutoScaleExact;
			this.XAxis1.Range = new NationalInstruments.UI.Range(1260D, 1360D);
			// 
			// YAxis1
			// 
			this.YAxis1.AutoMinorDivisionFrequency = 5;
			this.YAxis1.Caption = "[dB]";
			this.YAxis1.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.YAxis1.MajorDivisions.GridVisible = true;
			this.YAxis1.MinorDivisions.GridVisible = true;
			this.YAxis1.Mode = NationalInstruments.UI.AxisMode.Fixed;
			this.YAxis1.Range = new NationalInstruments.UI.Range(-60D, 0D);
			// 
			// uiLoadReference
			// 
			this.uiLoadReference.AutoSize = true;
			this.uiLoadReference.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
			this.uiLoadReference.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.uiLoadReference.ForeColor = System.Drawing.Color.DodgerBlue;
			this.uiLoadReference.Location = new System.Drawing.Point(454, 256);
			this.uiLoadReference.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiLoadReference.Name = "uiLoadReference";
			this.uiLoadReference.Size = new System.Drawing.Size(138, 32);
			this.uiLoadReference.TabIndex = 14;
			this.uiLoadReference.Text = "Reference File:";
			this.uiLoadReference.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.uiLoadReference.UseVisualStyleBackColor = true;
			this.uiLoadReference.Click += new System.EventHandler(this.uiLoadReference_Click);
			// 
			// uiPol_Minus45Diagonal
			// 
			this.uiPol_Minus45Diagonal.AutoSize = true;
			this.uiPol_Minus45Diagonal.Location = new System.Drawing.Point(578, 295);
			this.uiPol_Minus45Diagonal.Name = "uiPol_Minus45Diagonal";
			this.uiPol_Minus45Diagonal.Size = new System.Drawing.Size(114, 19);
			this.uiPol_Minus45Diagonal.TabIndex = 291;
			this.uiPol_Minus45Diagonal.Text = "Diagonal = -45º";
			this.uiPol_Minus45Diagonal.UseVisualStyleBackColor = true;
			this.uiPol_Minus45Diagonal.CheckedChanged += new System.EventHandler(this.uiPolLeft_CheckedChanged);
			// 
			// uiPol_LeftCircular
			// 
			this.uiPol_LeftCircular.AutoSize = true;
			this.uiPol_LeftCircular.Location = new System.Drawing.Point(454, 295);
			this.uiPol_LeftCircular.Name = "uiPol_LeftCircular";
			this.uiPol_LeftCircular.Size = new System.Drawing.Size(107, 19);
			this.uiPol_LeftCircular.TabIndex = 292;
			this.uiPol_LeftCircular.Text = "Circular =  Left";
			this.uiPol_LeftCircular.UseVisualStyleBackColor = true;
			this.uiPol_LeftCircular.CheckedChanged += new System.EventHandler(this.uiPolLeft_CheckedChanged);
			// 
			// btnSave
			// 
			this.btnSave.Location = new System.Drawing.Point(1029, 256);
			this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(80, 32);
			this.btnSave.TabIndex = 293;
			this.btnSave.Text = "Save";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// MeasureForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.ClientSize = new System.Drawing.Size(1119, 861);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.uiPol_Minus45Diagonal);
			this.Controls.Add(this.uiPol_LeftCircular);
			this.Controls.Add(this.panelParam);
			this.Controls.Add(this.grpEtc);
			this.Controls.Add(this.groupChipList);
			this.Controls.Add(this.grpGraphAnalysis);
			this.Controls.Add(this.statusBar);
			this.Controls.Add(this.panelMeasure);
			this.Controls.Add(this.uiLoadReference);
			this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Neon.Dwdm.Properties.Settings.Default, "MeasureFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Location = global::Neon.Dwdm.Properties.Settings.Default.MeasureFormLocation;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "MeasureForm";
			this.Text = "DWDM 측정";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.Load += new System.EventHandler(this.Form_Load);
			this.panelMeasure.ResumeLayout(false);
			this.panelMeasure.PerformLayout();
			this.grpGraphAnalysis.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mGraph)).EndInit();
			this.statusBar.ResumeLayout(false);
			this.statusBar.PerformLayout();
			this.groupChipList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.uiGridDut)).EndInit();
			this.grpEtc.ResumeLayout(false);
			this.panelParam.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.groupAlignCheck.ResumeLayout(false);
			this.groupAlignCheck.PerformLayout();
			this.groupPmChannels.ResumeLayout(false);
			this.groupPmChannels.PerformLayout();
			this.groupAlignParams.ResumeLayout(false);
			this.groupAlignParams.PerformLayout();
			this.groupSave.ResumeLayout(false);
			this.groupSave.PerformLayout();
			this.groupTls.ResumeLayout(false);
			this.groupTls.PerformLayout();
			this.GroupPmGain.ResumeLayout(false);
			this.GroupPmGain.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion
    internal System.Windows.Forms.Button btnMeasure;
    internal System.Windows.Forms.Button btnClearChipSn;
    internal System.Windows.Forms.TextBox txtNumChips;
    internal System.Windows.Forms.Label Label16;
    internal System.Windows.Forms.Button btnApplyChipSn;
    internal System.Windows.Forms.TextBox txtFisrtChipNo;
    internal System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.GroupBox panelMeasure;
    private System.Windows.Forms.GroupBox grpGraphAnalysis;
    internal System.Windows.Forms.Label lbChipNo;
    internal System.Windows.Forms.StatusStrip statusBar;
    internal System.Windows.Forms.ToolStripStatusLabel statusLabel;
    internal System.Windows.Forms.Button btnOpenStages;
    private System.Windows.Forms.GroupBox groupChipList;
    private System.Windows.Forms.GroupBox grpEtc;
    private System.Windows.Forms.TabControl panelParam;
    private System.Windows.Forms.TabPage tabPage1;
    internal System.Windows.Forms.CheckBox chkFaArrangement;
    internal System.Windows.Forms.CheckBox chkAlignment;
    internal System.Windows.Forms.CheckBox chkMeasurement;
    internal System.Windows.Forms.CheckBox chkAutoSave;
    internal System.Windows.Forms.GroupBox GroupPmGain;
    internal System.Windows.Forms.RadioButton rbtnGain2;
    internal System.Windows.Forms.RadioButton rbtnGain1;
    internal System.Windows.Forms.Label Label33;
    internal System.Windows.Forms.Label Label36;
    internal System.Windows.Forms.ComboBox txtChPitch;
    internal System.Windows.Forms.Label Label39;
    internal System.Windows.Forms.TextBox txtChipWidth;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.GroupBox groupBox5;
    internal System.Windows.Forms.Button btnClosedPosGo;
    internal System.Windows.Forms.Button btnSaveAsClosePosition;
    internal System.Windows.Forms.Label label21;
    internal System.Windows.Forms.Label label23;
    internal System.Windows.Forms.Label lbClosePosOutZ;
    internal System.Windows.Forms.Label label14;
    internal System.Windows.Forms.Label label17;
    internal System.Windows.Forms.Label lbClosePosInZ;
    private System.Windows.Forms.GroupBox groupBox3;
    internal System.Windows.Forms.Button btnInitPosGo;
    internal System.Windows.Forms.Label lbInitPosOutTz;
    internal System.Windows.Forms.Button btnSaveAsInitPosition;
    internal System.Windows.Forms.Label label28;
    internal System.Windows.Forms.Label label13;
    internal System.Windows.Forms.Label lbInitPosOutTy;
    internal System.Windows.Forms.Label label20;
    internal System.Windows.Forms.Label label2;
    internal System.Windows.Forms.Label lbInitPosInX;
    internal System.Windows.Forms.Label lbInitPosOutTx;
    internal System.Windows.Forms.Label label12;
    internal System.Windows.Forms.Label label3;
    internal System.Windows.Forms.Label lbInitPosInY;
    internal System.Windows.Forms.Label lbInitPosOutZ;
    internal System.Windows.Forms.Label label19;
    internal System.Windows.Forms.Label label5;
    internal System.Windows.Forms.Label lbInitPosInZ;
    internal System.Windows.Forms.Label lbInitPosOutY;
    internal System.Windows.Forms.Label label26;
    internal System.Windows.Forms.Label label6;
    internal System.Windows.Forms.Label lbInitPosInTx;
    internal System.Windows.Forms.Label label37;
    internal System.Windows.Forms.Label label24;
    internal System.Windows.Forms.Label lbInitPosOutX;
    internal System.Windows.Forms.Label lbInitPosInTy;
    internal System.Windows.Forms.Label label7;
    internal System.Windows.Forms.Label label22;
    internal System.Windows.Forms.Label lbInitPosInTz;
    internal System.Windows.Forms.Button btnCloseStages;
    internal System.Windows.Forms.CheckBox chkRetChip1Pos;
    internal System.Windows.Forms.CheckBox chkRoll;
    internal System.Windows.Forms.Label Label15;
    internal System.Windows.Forms.Label label43;
    internal System.Windows.Forms.Label lbl_Init_Center_X;
    internal System.Windows.Forms.Label label45;
    internal System.Windows.Forms.TextBox txtPmGainLevel2;
    internal System.Windows.Forms.TextBox txtPmGainLevel1;
    internal System.Windows.Forms.Label label9;
    internal System.Windows.Forms.Label label25;
    internal System.Windows.Forms.Label label10;
    private System.Windows.Forms.SplitContainer splitContainer1;
    internal NationalInstruments.UI.WaveformPlot WaveformPlot1;
    internal NationalInstruments.UI.XAxis XAxis1;
    internal NationalInstruments.UI.YAxis YAxis1;
    internal NationalInstruments.UI.WindowsForms.WaveformGraph mGraph;
    internal NationalInstruments.UI.WaveformPlot waveformPlot2;
    internal NationalInstruments.UI.XAxis xAxis2;
    internal NationalInstruments.UI.YAxis yAxis2;
    internal System.Windows.Forms.ComboBox txtPmPortStop;
    internal System.Windows.Forms.ComboBox txtPmPortStart;
    internal System.Windows.Forms.Label label11;
    private System.Windows.Forms.DataGridView uiGridDut;
    internal System.Windows.Forms.Button uiLoadReference;
    private System.Windows.Forms.CheckBox uiPol_Minus45Diagonal;
    private System.Windows.Forms.CheckBox uiPol_LeftCircular;
    internal System.Windows.Forms.CheckBox chkWaferFolder;
    internal System.Windows.Forms.CheckBox chkCenterStage;
    internal System.Windows.Forms.CheckBox chkChOrderReverse;
    internal System.Windows.Forms.Label lbl_Init_Center_Y;
    internal System.Windows.Forms.Label label27;
    internal System.Windows.Forms.Label label8;
    internal System.Windows.Forms.TextBox txtZBuffer;
    internal System.Windows.Forms.Label label29;
    private System.Windows.Forms.CheckBox chkSerial;
    internal System.Windows.Forms.Label label30;
    internal System.Windows.Forms.TextBox txtChipsPerRightAlign;
    internal System.Windows.Forms.Label label31;
    internal System.Windows.Forms.TextBox txtYBuffer;
    internal System.Windows.Forms.Label label32;
    internal System.Windows.Forms.TextBox txtAlignTimes;
    internal System.Windows.Forms.Label label34;
	internal System.Windows.Forms.CheckBox uiMeasureCladding;
	private System.Windows.Forms.TextBox uiCladDistance;
	internal System.Windows.Forms.GroupBox groupTls;
	internal System.Windows.Forms.TextBox txtTlsNoiseShift;
	internal System.Windows.Forms.Label label40;
	internal System.Windows.Forms.Label label41;
	internal System.Windows.Forms.TextBox txtTlsSNR;
	internal System.Windows.Forms.Label label35;
	internal System.Windows.Forms.Label label38;
	internal System.Windows.Forms.TextBox txtWaveStep;
	internal System.Windows.Forms.TextBox txtTlsPower;
	internal System.Windows.Forms.TextBox txtWaveStop;
	internal System.Windows.Forms.TextBox txtWaveStart;
	internal System.Windows.Forms.Label label18;
	internal System.Windows.Forms.Label label4;
	internal System.Windows.Forms.Label lbOptRng;
	internal System.Windows.Forms.Label label1;
	internal System.Windows.Forms.GroupBox groupPmChannels;
	internal System.Windows.Forms.Label label42;
	private System.Windows.Forms.CheckBox chkLoopScan;
	internal System.Windows.Forms.GroupBox groupAlignParams;
	internal System.Windows.Forms.GroupBox groupSave;
	internal System.Windows.Forms.TextBox txtLoopStart;
	private System.Windows.Forms.ToolTip toolTip1;
	internal System.Windows.Forms.TextBox txtLoopStop;
	private System.Windows.Forms.GroupBox groupAlignCheck;
	internal System.Windows.Forms.RadioButton rbtnAutoSaveRng;
	internal System.Windows.Forms.RadioButton rbtnAutoSaveFull;
	internal System.Windows.Forms.TextBox txtSaveWaveStop;
	internal System.Windows.Forms.TextBox txtSaveWaveStart;
	internal System.Windows.Forms.Label label44;
	internal System.Windows.Forms.Button btnSaveFolder;
	internal System.Windows.Forms.Button btnSave;
	private System.Windows.Forms.GroupBox groupBox1;
	internal System.Windows.Forms.CheckBox chkBackupPower;
	internal System.Windows.Forms.CheckBox chkBackupWave;
}
