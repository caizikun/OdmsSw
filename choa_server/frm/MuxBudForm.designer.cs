
partial class MuxBudForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MuxBudForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnMeasure = new System.Windows.Forms.Button();
            this.btnDelAllChipNos = new System.Windows.Forms.Button();
            this.txtChipCnt = new System.Windows.Forms.TextBox();
            this.btnChipNoOk = new System.Windows.Forms.Button();
            this.txtFisrtChipNo = new System.Windows.Forms.TextBox();
            this.hdgvChipNos = new HanDataGridView.HanDataGridView();
            this.btnStop = new System.Windows.Forms.Button();
            this.grpMeasurement = new System.Windows.Forms.GroupBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.grpGraphAnalysis = new System.Windows.Forms.GroupBox();
            this._wg = new DrBae.TnM.UI.WdmGraph();
            this.lbChipNo = new System.Windows.Forms.Label();
            this.tss = new System.Windows.Forms.StatusStrip();
            this.tsslbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnOpenStages = new System.Windows.Forms.Button();
            this.grpEtc = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.chkWaferFolder = new System.Windows.Forms.CheckBox();
            this.txtScanRange = new System.Windows.Forms.TextBox();
            this.chkCenterStage = new System.Windows.Forms.CheckBox();
            this.txtChipNo = new System.Windows.Forms.TextBox();
            this.checkScan = new System.Windows.Forms.CheckBox();
            this.chkAutoReturn = new System.Windows.Forms.CheckBox();
            this.chkFaArrangement = new System.Windows.Forms.CheckBox();
            this.btnSaveFolder = new System.Windows.Forms.Button();
            this.lbSaveFolderPath = new System.Windows.Forms.Label();
            this.chkAlignment = new System.Windows.Forms.CheckBox();
            this.chkMeasurement = new System.Windows.Forms.CheckBox();
            this.grpAutosave = new System.Windows.Forms.GroupBox();
            this.rbtnAutoSaveRng = new System.Windows.Forms.RadioButton();
            this.rbtnAutoSaveFull = new System.Windows.Forms.RadioButton();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.txtSaveRangeStart = new System.Windows.Forms.TextBox();
            this.txtSaveRangeStop = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.rbtnGain2 = new System.Windows.Forms.RadioButton();
            this.rbtnGain1 = new System.Windows.Forms.RadioButton();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnFA_MMF = new System.Windows.Forms.RadioButton();
            this.rbtnFA_SMF = new System.Windows.Forms.RadioButton();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Label30 = new System.Windows.Forms.Label();
            this.Label29 = new System.Windows.Forms.Label();
            this.PictureBox2 = new System.Windows.Forms.PictureBox();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.rbtnChDirReverse = new System.Windows.Forms.RadioButton();
            this.rbtnChDirForward = new System.Windows.Forms.RadioButton();
            this.Label36 = new System.Windows.Forms.Label();
            this.cbCorepitch = new System.Windows.Forms.ComboBox();
            this.Label38 = new System.Windows.Forms.Label();
            this.Label39 = new System.Windows.Forms.Label();
            this.txtChipWidth = new System.Windows.Forms.TextBox();
            this.Label40 = new System.Windows.Forms.Label();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabAnalysis = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPassRangeApply = new System.Windows.Forms.Button();
            this.txtPassRangeIlUnif = new System.Windows.Forms.NumericUpDown();
            this.txtPassRangeIlMin = new System.Windows.Forms.NumericUpDown();
            this.inspectionGrid = new Neon.Aligner.InspectionGrid();
            ((System.ComponentModel.ISupportInitialize)(this.hdgvChipNos)).BeginInit();
            this.grpMeasurement.SuspendLayout();
            this.grpGraphAnalysis.SuspendLayout();
            this.tss.SuspendLayout();
            this.grpEtc.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grpAutosave.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassRangeIlUnif)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassRangeIlMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inspectionGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMeasure
            // 
            this.btnMeasure.Font = new System.Drawing.Font("맑은 고딕", 17.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMeasure.Location = new System.Drawing.Point(7, 16);
            this.btnMeasure.Name = "btnMeasure";
            this.btnMeasure.Size = new System.Drawing.Size(290, 45);
            this.btnMeasure.TabIndex = 278;
            this.btnMeasure.Text = "MEASURE";
            this.btnMeasure.UseVisualStyleBackColor = true;
            this.btnMeasure.Click += new System.EventHandler(this.btnMeasure_Click);
            // 
            // btnDelAllChipNos
            // 
            this.btnDelAllChipNos.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelAllChipNos.Location = new System.Drawing.Point(217, 53);
            this.btnDelAllChipNos.Name = "btnDelAllChipNos";
            this.btnDelAllChipNos.Size = new System.Drawing.Size(82, 32);
            this.btnDelAllChipNos.TabIndex = 277;
            this.btnDelAllChipNos.Text = "Clear";
            this.btnDelAllChipNos.UseVisualStyleBackColor = true;
            this.btnDelAllChipNos.Click += new System.EventHandler(this.btnDelAllChipNos_Click);
            // 
            // txtChipCnt
            // 
            this.txtChipCnt.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtChipCnt.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChipCnt.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.txtChipCnt.Location = new System.Drawing.Point(57, 57);
            this.txtChipCnt.Name = "txtChipCnt";
            this.txtChipCnt.Size = new System.Drawing.Size(43, 25);
            this.txtChipCnt.TabIndex = 276;
            this.txtChipCnt.Text = "1";
            this.txtChipCnt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnChipNoOk
            // 
            this.btnChipNoOk.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChipNoOk.Location = new System.Drawing.Point(115, 53);
            this.btnChipNoOk.Name = "btnChipNoOk";
            this.btnChipNoOk.Size = new System.Drawing.Size(96, 32);
            this.btnChipNoOk.TabIndex = 273;
            this.btnChipNoOk.Text = "Apply";
            this.btnChipNoOk.UseVisualStyleBackColor = true;
            this.btnChipNoOk.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtFisrtChipNo
            // 
            this.txtFisrtChipNo.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtFisrtChipNo.Font = new System.Drawing.Font("Consolas", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFisrtChipNo.ForeColor = System.Drawing.Color.Lime;
            this.txtFisrtChipNo.Location = new System.Drawing.Point(12, 23);
            this.txtFisrtChipNo.Name = "txtFisrtChipNo";
            this.txtFisrtChipNo.Size = new System.Drawing.Size(287, 27);
            this.txtFisrtChipNo.TabIndex = 272;
            this.txtFisrtChipNo.Text = "TVC07200123-8-3-H52-1006";
            // 
            // hdgvChipNos
            // 
            this.hdgvChipNos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.hdgvChipNos.Location = new System.Drawing.Point(308, 109);
            this.hdgvChipNos.Name = "hdgvChipNos";
            this.hdgvChipNos.RowHeadersVisible = false;
            this.hdgvChipNos.RowTemplate.Height = 23;
            this.hdgvChipNos.Size = new System.Drawing.Size(307, 418);
            this.hdgvChipNos.TabIndex = 271;
            this.hdgvChipNos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.hdgvChipNos_CellClick);
            this.hdgvChipNos.SelectionChanged += new System.EventHandler(this.hdgvChipNos_SelectionChanged);
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnStop.ForeColor = System.Drawing.Color.OrangeRed;
            this.btnStop.Location = new System.Drawing.Point(150, 67);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(150, 32);
            this.btnStop.TabIndex = 280;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // grpMeasurement
            // 
            this.grpMeasurement.Controls.Add(this.Label16);
            this.grpMeasurement.Controls.Add(this.txtFisrtChipNo);
            this.grpMeasurement.Controls.Add(this.btnChipNoOk);
            this.grpMeasurement.Controls.Add(this.btnDelAllChipNos);
            this.grpMeasurement.Controls.Add(this.txtChipCnt);
            this.grpMeasurement.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpMeasurement.Location = new System.Drawing.Point(308, 7);
            this.grpMeasurement.Name = "grpMeasurement";
            this.grpMeasurement.Size = new System.Drawing.Size(307, 91);
            this.grpMeasurement.TabIndex = 281;
            this.grpMeasurement.TabStop = false;
            this.grpMeasurement.Text = "Chip List";
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Label16.Location = new System.Drawing.Point(9, 62);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(43, 15);
            this.Label16.TabIndex = 278;
            this.Label16.Text = "# Chip";
            // 
            // grpGraphAnalysis
            // 
            this.grpGraphAnalysis.Controls.Add(this._wg);
            this.grpGraphAnalysis.Controls.Add(this.lbChipNo);
            this.grpGraphAnalysis.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpGraphAnalysis.Location = new System.Drawing.Point(621, 7);
            this.grpGraphAnalysis.Name = "grpGraphAnalysis";
            this.grpGraphAnalysis.Size = new System.Drawing.Size(494, 365);
            this.grpGraphAnalysis.TabIndex = 284;
            this.grpGraphAnalysis.TabStop = false;
            this.grpGraphAnalysis.Text = "Plot";
            // 
            // _wg
            // 
            this._wg.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            this._wg.Cwl = null;
            this._wg.DataSource = null;
            this._wg.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._wg.IntervalOffetX = 0;
            this._wg.IntervalX = 1000;
            this._wg.IntervalY = 1000D;
            this._wg.LineThickness = 1;
            this._wg.Location = new System.Drawing.Point(4, 38);
            this._wg.Margin = new System.Windows.Forms.Padding(0);
            this._wg.MarkerSize = 6;
            this._wg.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.None;
            this._wg.MaxY = double.NaN;
            this._wg.MinY = double.NaN;
            this._wg.Name = "_wg";
            this._wg.ScaleFactorX = 1000;
            this._wg.ScaleFactorY = 0D;
            this._wg.ShowLegends = false;
            this._wg.Size = new System.Drawing.Size(488, 324);
            this._wg.TabIndex = 285;
            this._wg.Wl = ((System.Collections.Generic.List<int>)(resources.GetObject("_wg.Wl")));
            // 
            // lbChipNo
            // 
            this.lbChipNo.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold);
            this.lbChipNo.Location = new System.Drawing.Point(11, 19);
            this.lbChipNo.Name = "lbChipNo";
            this.lbChipNo.Size = new System.Drawing.Size(477, 19);
            this.lbChipNo.TabIndex = 284;
            this.lbChipNo.Text = "TVC07200123-8-3-H52-1006";
            this.lbChipNo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tss
            // 
            this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbStatus});
            this.tss.Location = new System.Drawing.Point(0, 638);
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(1122, 24);
            this.tss.TabIndex = 285;
            this.tss.Text = "StatusStrip1";
            // 
            // tsslbStatus
            // 
            this.tsslbStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbStatus.Name = "tsslbStatus";
            this.tsslbStatus.Size = new System.Drawing.Size(20, 19);
            this.tsslbStatus.Text = "...";
            // 
            // btnOpenStages
            // 
            this.btnOpenStages.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOpenStages.Location = new System.Drawing.Point(7, 67);
            this.btnOpenStages.Name = "btnOpenStages";
            this.btnOpenStages.Size = new System.Drawing.Size(137, 32);
            this.btnOpenStages.TabIndex = 286;
            this.btnOpenStages.Text = "OPEN STAGES";
            this.btnOpenStages.UseVisualStyleBackColor = true;
            this.btnOpenStages.Click += new System.EventHandler(this.btnOpenStages_Click);
            // 
            // grpEtc
            // 
            this.grpEtc.Controls.Add(this.btnOpenStages);
            this.grpEtc.Controls.Add(this.btnMeasure);
            this.grpEtc.Controls.Add(this.btnStop);
            this.grpEtc.Location = new System.Drawing.Point(308, 528);
            this.grpEtc.Name = "grpEtc";
            this.grpEtc.Size = new System.Drawing.Size(307, 107);
            this.grpEtc.TabIndex = 288;
            this.grpEtc.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(8, 7);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(295, 628);
            this.tabControl1.TabIndex = 290;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.chkWaferFolder);
            this.tabPage1.Controls.Add(this.txtScanRange);
            this.tabPage1.Controls.Add(this.chkCenterStage);
            this.tabPage1.Controls.Add(this.txtChipNo);
            this.tabPage1.Controls.Add(this.checkScan);
            this.tabPage1.Controls.Add(this.chkAutoReturn);
            this.tabPage1.Controls.Add(this.chkFaArrangement);
            this.tabPage1.Controls.Add(this.btnSaveFolder);
            this.tabPage1.Controls.Add(this.lbSaveFolderPath);
            this.tabPage1.Controls.Add(this.chkAlignment);
            this.tabPage1.Controls.Add(this.chkMeasurement);
            this.tabPage1.Controls.Add(this.grpAutosave);
            this.tabPage1.Controls.Add(this.GroupBox4);
            this.tabPage1.Controls.Add(this.GroupBox1);
            this.tabPage1.Controls.Add(this.GroupBox6);
            this.tabPage1.Controls.Add(this.Label36);
            this.tabPage1.Controls.Add(this.cbCorepitch);
            this.tabPage1.Controls.Add(this.Label38);
            this.tabPage1.Controls.Add(this.Label39);
            this.tabPage1.Controls.Add(this.txtChipWidth);
            this.tabPage1.Controls.Add(this.Label40);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(287, 602);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Option & Configuration";
            // 
            // chkWaferFolder
            // 
            this.chkWaferFolder.AutoSize = true;
            this.chkWaferFolder.Checked = true;
            this.chkWaferFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWaferFolder.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chkWaferFolder.Location = new System.Drawing.Point(130, 496);
            this.chkWaferFolder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkWaferFolder.Name = "chkWaferFolder";
            this.chkWaferFolder.Size = new System.Drawing.Size(132, 19);
            this.chkWaferFolder.TabIndex = 307;
            this.chkWaferFolder.Text = "Create Wafer Folder";
            this.chkWaferFolder.UseVisualStyleBackColor = true;
            // 
            // txtScanRange
            // 
            this.txtScanRange.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtScanRange.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtScanRange.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.txtScanRange.Location = new System.Drawing.Point(241, 376);
            this.txtScanRange.Name = "txtScanRange";
            this.txtScanRange.Size = new System.Drawing.Size(41, 25);
            this.txtScanRange.TabIndex = 294;
            this.txtScanRange.Text = "20";
            this.txtScanRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // chkCenterStage
            // 
            this.chkCenterStage.AutoSize = true;
            this.chkCenterStage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCenterStage.ForeColor = System.Drawing.Color.DarkRed;
            this.chkCenterStage.Location = new System.Drawing.Point(10, 332);
            this.chkCenterStage.Name = "chkCenterStage";
            this.chkCenterStage.Size = new System.Drawing.Size(150, 19);
            this.chkCenterStage.TabIndex = 306;
            this.chkCenterStage.Text = "Using Center Stage";
            this.chkCenterStage.UseVisualStyleBackColor = true;
            // 
            // txtChipNo
            // 
            this.txtChipNo.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtChipNo.Enabled = false;
            this.txtChipNo.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChipNo.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.txtChipNo.Location = new System.Drawing.Point(60, 376);
            this.txtChipNo.Name = "txtChipNo";
            this.txtChipNo.Size = new System.Drawing.Size(170, 25);
            this.txtChipNo.TabIndex = 292;
            this.txtChipNo.Text = "8";
            this.txtChipNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkScan
            // 
            this.checkScan.AutoSize = true;
            this.checkScan.Enabled = false;
            this.checkScan.Location = new System.Drawing.Point(10, 382);
            this.checkScan.Name = "checkScan";
            this.checkScan.Size = new System.Drawing.Size(51, 16);
            this.checkScan.TabIndex = 293;
            this.checkScan.Text = "align";
            this.checkScan.UseVisualStyleBackColor = true;
            // 
            // chkAutoReturn
            // 
            this.chkAutoReturn.AutoSize = true;
            this.chkAutoReturn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoReturn.Location = new System.Drawing.Point(164, 304);
            this.chkAutoReturn.Name = "chkAutoReturn";
            this.chkAutoReturn.Size = new System.Drawing.Size(90, 19);
            this.chkAutoReturn.TabIndex = 305;
            this.chkAutoReturn.Text = "Auto Return";
            this.chkAutoReturn.UseVisualStyleBackColor = true;
            // 
            // chkFaArrangement
            // 
            this.chkFaArrangement.AutoSize = true;
            this.chkFaArrangement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkFaArrangement.Location = new System.Drawing.Point(10, 276);
            this.chkFaArrangement.Name = "chkFaArrangement";
            this.chkFaArrangement.Size = new System.Drawing.Size(114, 19);
            this.chkFaArrangement.TabIndex = 303;
            this.chkFaArrangement.Text = "FA Arrangement";
            this.chkFaArrangement.UseVisualStyleBackColor = true;
            // 
            // btnSaveFolder
            // 
            this.btnSaveFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveFolder.Location = new System.Drawing.Point(6, 491);
            this.btnSaveFolder.Name = "btnSaveFolder";
            this.btnSaveFolder.Size = new System.Drawing.Size(110, 25);
            this.btnSaveFolder.TabIndex = 292;
            this.btnSaveFolder.Text = "Save Folder";
            this.btnSaveFolder.UseVisualStyleBackColor = true;
            this.btnSaveFolder.Click += new System.EventHandler(this.btnSaveFolder_Click);
            // 
            // lbSaveFolderPath
            // 
            this.lbSaveFolderPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSaveFolderPath.Location = new System.Drawing.Point(7, 528);
            this.lbSaveFolderPath.Name = "lbSaveFolderPath";
            this.lbSaveFolderPath.Size = new System.Drawing.Size(267, 62);
            this.lbSaveFolderPath.TabIndex = 293;
            this.lbSaveFolderPath.Text = "c:\\";
            // 
            // chkAlignment
            // 
            this.chkAlignment.AutoSize = true;
            this.chkAlignment.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAlignment.Location = new System.Drawing.Point(10, 304);
            this.chkAlignment.Name = "chkAlignment";
            this.chkAlignment.Size = new System.Drawing.Size(81, 19);
            this.chkAlignment.TabIndex = 302;
            this.chkAlignment.Text = "Alignment";
            this.chkAlignment.UseVisualStyleBackColor = true;
            // 
            // chkMeasurement
            // 
            this.chkMeasurement.AutoSize = true;
            this.chkMeasurement.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMeasurement.Location = new System.Drawing.Point(164, 276);
            this.chkMeasurement.Name = "chkMeasurement";
            this.chkMeasurement.Size = new System.Drawing.Size(103, 19);
            this.chkMeasurement.TabIndex = 301;
            this.chkMeasurement.Text = "Measurement";
            this.chkMeasurement.UseVisualStyleBackColor = true;
            // 
            // grpAutosave
            // 
            this.grpAutosave.Controls.Add(this.rbtnAutoSaveRng);
            this.grpAutosave.Controls.Add(this.rbtnAutoSaveFull);
            this.grpAutosave.Controls.Add(this.chkAutoSave);
            this.grpAutosave.Controls.Add(this.Label11);
            this.grpAutosave.Controls.Add(this.txtSaveRangeStart);
            this.grpAutosave.Controls.Add(this.txtSaveRangeStop);
            this.grpAutosave.Controls.Add(this.Label9);
            this.grpAutosave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAutosave.Location = new System.Drawing.Point(6, 406);
            this.grpAutosave.Name = "grpAutosave";
            this.grpAutosave.Size = new System.Drawing.Size(276, 71);
            this.grpAutosave.TabIndex = 299;
            this.grpAutosave.TabStop = false;
            // 
            // rbtnAutoSaveRng
            // 
            this.rbtnAutoSaveRng.AutoSize = true;
            this.rbtnAutoSaveRng.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnAutoSaveRng.Location = new System.Drawing.Point(195, 16);
            this.rbtnAutoSaveRng.Name = "rbtnAutoSaveRng";
            this.rbtnAutoSaveRng.Size = new System.Drawing.Size(62, 19);
            this.rbtnAutoSaveRng.TabIndex = 6;
            this.rbtnAutoSaveRng.Text = "Range";
            this.rbtnAutoSaveRng.UseVisualStyleBackColor = true;
            // 
            // rbtnAutoSaveFull
            // 
            this.rbtnAutoSaveFull.AutoSize = true;
            this.rbtnAutoSaveFull.Checked = true;
            this.rbtnAutoSaveFull.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnAutoSaveFull.Location = new System.Drawing.Point(140, 16);
            this.rbtnAutoSaveFull.Name = "rbtnAutoSaveFull";
            this.rbtnAutoSaveFull.Size = new System.Drawing.Size(45, 19);
            this.rbtnAutoSaveFull.TabIndex = 5;
            this.rbtnAutoSaveFull.TabStop = true;
            this.rbtnAutoSaveFull.Text = "Full";
            this.rbtnAutoSaveFull.UseVisualStyleBackColor = true;
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Checked = true;
            this.chkAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoSave.Location = new System.Drawing.Point(13, 17);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(77, 19);
            this.chkAutoSave.TabIndex = 300;
            this.chkAutoSave.Text = "AutoSave";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            this.chkAutoSave.CheckedChanged += new System.EventHandler(this.chkAutoSave_CheckedChanged);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(58, 44);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(74, 15);
            this.Label11.TabIndex = 294;
            this.Label11.Text = "Save Range";
            // 
            // txtSaveRangeStart
            // 
            this.txtSaveRangeStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSaveRangeStart.Location = new System.Drawing.Point(140, 41);
            this.txtSaveRangeStart.Name = "txtSaveRangeStart";
            this.txtSaveRangeStart.Size = new System.Drawing.Size(49, 21);
            this.txtSaveRangeStart.TabIndex = 295;
            this.txtSaveRangeStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSaveRangeStop
            // 
            this.txtSaveRangeStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSaveRangeStop.Location = new System.Drawing.Point(208, 41);
            this.txtSaveRangeStop.Name = "txtSaveRangeStop";
            this.txtSaveRangeStop.Size = new System.Drawing.Size(49, 21);
            this.txtSaveRangeStop.TabIndex = 296;
            this.txtSaveRangeStop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(194, 44);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(11, 15);
            this.Label9.TabIndex = 298;
            this.Label9.Text = "-";
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.rbtnGain2);
            this.GroupBox4.Controls.Add(this.rbtnGain1);
            this.GroupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox4.Location = new System.Drawing.Point(6, 221);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(276, 45);
            this.GroupBox4.TabIndex = 285;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Gains";
            // 
            // rbtnGain2
            // 
            this.rbtnGain2.AutoSize = true;
            this.rbtnGain2.Font = new System.Drawing.Font("돋움체", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbtnGain2.Location = new System.Drawing.Point(173, 17);
            this.rbtnGain2.Name = "rbtnGain2";
            this.rbtnGain2.Size = new System.Drawing.Size(32, 17);
            this.rbtnGain2.TabIndex = 6;
            this.rbtnGain2.TabStop = true;
            this.rbtnGain2.Text = "2";
            this.rbtnGain2.UseVisualStyleBackColor = true;
            // 
            // rbtnGain1
            // 
            this.rbtnGain1.AutoSize = true;
            this.rbtnGain1.Font = new System.Drawing.Font("돋움체", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbtnGain1.Location = new System.Drawing.Point(82, 17);
            this.rbtnGain1.Name = "rbtnGain1";
            this.rbtnGain1.Size = new System.Drawing.Size(32, 17);
            this.rbtnGain1.TabIndex = 5;
            this.rbtnGain1.TabStop = true;
            this.rbtnGain1.Text = "1";
            this.rbtnGain1.UseVisualStyleBackColor = true;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.rbtnFA_MMF);
            this.GroupBox1.Controls.Add(this.rbtnFA_SMF);
            this.GroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(6, 73);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(276, 52);
            this.GroupBox1.TabIndex = 278;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Out FAB";
            // 
            // rbtnFA_MMF
            // 
            this.rbtnFA_MMF.AutoSize = true;
            this.rbtnFA_MMF.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnFA_MMF.Location = new System.Drawing.Point(172, 21);
            this.rbtnFA_MMF.Name = "rbtnFA_MMF";
            this.rbtnFA_MMF.Size = new System.Drawing.Size(92, 21);
            this.rbtnFA_MMF.TabIndex = 6;
            this.rbtnFA_MMF.TabStop = true;
            this.rbtnFA_MMF.Text = "MMF (1~4)";
            this.rbtnFA_MMF.UseVisualStyleBackColor = true;
            // 
            // rbtnFA_SMF
            // 
            this.rbtnFA_SMF.AutoSize = true;
            this.rbtnFA_SMF.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnFA_SMF.Location = new System.Drawing.Point(70, 21);
            this.rbtnFA_SMF.Name = "rbtnFA_SMF";
            this.rbtnFA_SMF.Size = new System.Drawing.Size(87, 21);
            this.rbtnFA_SMF.TabIndex = 5;
            this.rbtnFA_SMF.TabStop = true;
            this.rbtnFA_SMF.Text = "SMF (5~8)";
            this.rbtnFA_SMF.UseVisualStyleBackColor = true;
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.label1);
            this.GroupBox6.Controls.Add(this.label2);
            this.GroupBox6.Controls.Add(this.Label30);
            this.GroupBox6.Controls.Add(this.Label29);
            this.GroupBox6.Controls.Add(this.PictureBox2);
            this.GroupBox6.Controls.Add(this.PictureBox1);
            this.GroupBox6.Controls.Add(this.rbtnChDirReverse);
            this.GroupBox6.Controls.Add(this.rbtnChDirForward);
            this.GroupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox6.Location = new System.Drawing.Point(6, 136);
            this.GroupBox6.Name = "GroupBox6";
            this.GroupBox6.Size = new System.Drawing.Size(276, 81);
            this.GroupBox6.TabIndex = 279;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Channel direction";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(235, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 15);
            this.label1.TabIndex = 254;
            this.label1.Text = "1331";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(235, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 253;
            this.label2.Text = "1271";
            // 
            // Label30
            // 
            this.Label30.AutoSize = true;
            this.Label30.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label30.Location = new System.Drawing.Point(104, 50);
            this.Label30.Name = "Label30";
            this.Label30.Size = new System.Drawing.Size(35, 15);
            this.Label30.TabIndex = 252;
            this.Label30.Text = "1331";
            // 
            // Label29
            // 
            this.Label29.AutoSize = true;
            this.Label29.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label29.Location = new System.Drawing.Point(104, 22);
            this.Label29.Name = "Label29";
            this.Label29.Size = new System.Drawing.Size(35, 15);
            this.Label29.TabIndex = 251;
            this.Label29.Text = "1271";
            // 
            // PictureBox2
            // 
            this.PictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox2.Image")));
            this.PictureBox2.Location = new System.Drawing.Point(162, 22);
            this.PictureBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PictureBox2.Name = "PictureBox2";
            this.PictureBox2.Size = new System.Drawing.Size(70, 45);
            this.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox2.TabIndex = 250;
            this.PictureBox2.TabStop = false;
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox1.Image")));
            this.PictureBox1.Location = new System.Drawing.Point(31, 23);
            this.PictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(70, 42);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureBox1.TabIndex = 249;
            this.PictureBox1.TabStop = false;
            // 
            // rbtnChDirReverse
            // 
            this.rbtnChDirReverse.AutoSize = true;
            this.rbtnChDirReverse.Font = new System.Drawing.Font("돋움체", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbtnChDirReverse.Location = new System.Drawing.Point(143, 38);
            this.rbtnChDirReverse.Name = "rbtnChDirReverse";
            this.rbtnChDirReverse.Size = new System.Drawing.Size(14, 13);
            this.rbtnChDirReverse.TabIndex = 6;
            this.rbtnChDirReverse.TabStop = true;
            this.rbtnChDirReverse.UseVisualStyleBackColor = true;
            // 
            // rbtnChDirForward
            // 
            this.rbtnChDirForward.AutoSize = true;
            this.rbtnChDirForward.Font = new System.Drawing.Font("돋움체", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.rbtnChDirForward.Location = new System.Drawing.Point(10, 38);
            this.rbtnChDirForward.Name = "rbtnChDirForward";
            this.rbtnChDirForward.Size = new System.Drawing.Size(14, 13);
            this.rbtnChDirForward.TabIndex = 5;
            this.rbtnChDirForward.TabStop = true;
            this.rbtnChDirForward.UseVisualStyleBackColor = true;
            // 
            // Label36
            // 
            this.Label36.AutoSize = true;
            this.Label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label36.Location = new System.Drawing.Point(34, 47);
            this.Label36.Name = "Label36";
            this.Label36.Size = new System.Drawing.Size(131, 15);
            this.Label36.TabIndex = 284;
            this.Label36.Text = "Channel Space(ptich) :";
            // 
            // cbCorepitch
            // 
            this.cbCorepitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbCorepitch.FormattingEnabled = true;
            this.cbCorepitch.Items.AddRange(new object[] {
            "250",
            "750"});
            this.cbCorepitch.Location = new System.Drawing.Point(172, 43);
            this.cbCorepitch.Name = "cbCorepitch";
            this.cbCorepitch.Size = new System.Drawing.Size(72, 23);
            this.cbCorepitch.TabIndex = 286;
            // 
            // Label38
            // 
            this.Label38.AutoSize = true;
            this.Label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label38.Location = new System.Drawing.Point(247, 47);
            this.Label38.Name = "Label38";
            this.Label38.Size = new System.Drawing.Size(31, 15);
            this.Label38.TabIndex = 287;
            this.Label38.Text = "[μm]";
            // 
            // Label39
            // 
            this.Label39.AutoSize = true;
            this.Label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label39.Location = new System.Drawing.Point(93, 16);
            this.Label39.Name = "Label39";
            this.Label39.Size = new System.Drawing.Size(72, 15);
            this.Label39.TabIndex = 288;
            this.Label39.Text = "Chip Width :";
            // 
            // txtChipWidth
            // 
            this.txtChipWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChipWidth.Location = new System.Drawing.Point(172, 13);
            this.txtChipWidth.Name = "txtChipWidth";
            this.txtChipWidth.Size = new System.Drawing.Size(72, 21);
            this.txtChipWidth.TabIndex = 290;
            this.txtChipWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label40
            // 
            this.Label40.AutoSize = true;
            this.Label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label40.Location = new System.Drawing.Point(247, 16);
            this.Label40.Name = "Label40";
            this.Label40.Size = new System.Drawing.Size(31, 15);
            this.Label40.TabIndex = 289;
            this.Label40.Text = "[μm]";
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabAnalysis);
            this.tabControl2.Location = new System.Drawing.Point(621, 378);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(490, 257);
            this.tabControl2.TabIndex = 293;
            // 
            // tabAnalysis
            // 
            this.tabAnalysis.BackColor = System.Drawing.SystemColors.Control;
            this.tabAnalysis.Controls.Add(this.label5);
            this.tabAnalysis.Controls.Add(this.label4);
            this.tabAnalysis.Controls.Add(this.btnPassRangeApply);
            this.tabAnalysis.Controls.Add(this.txtPassRangeIlUnif);
            this.tabAnalysis.Controls.Add(this.txtPassRangeIlMin);
            this.tabAnalysis.Controls.Add(this.inspectionGrid);
            this.tabAnalysis.Location = new System.Drawing.Point(4, 22);
            this.tabAnalysis.Name = "tabAnalysis";
            this.tabAnalysis.Padding = new System.Windows.Forms.Padding(3);
            this.tabAnalysis.Size = new System.Drawing.Size(482, 231);
            this.tabAnalysis.TabIndex = 0;
            this.tabAnalysis.Text = "Analysis";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label5.Location = new System.Drawing.Point(100, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 15);
            this.label5.TabIndex = 279;
            this.label5.Text = "Unif";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label4.Location = new System.Drawing.Point(8, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 15);
            this.label4.TabIndex = 278;
            this.label4.Text = "IL";
            // 
            // btnPassRangeApply
            // 
            this.btnPassRangeApply.Location = new System.Drawing.Point(206, 5);
            this.btnPassRangeApply.Name = "btnPassRangeApply";
            this.btnPassRangeApply.Size = new System.Drawing.Size(69, 26);
            this.btnPassRangeApply.TabIndex = 4;
            this.btnPassRangeApply.Text = "Apply";
            this.btnPassRangeApply.UseVisualStyleBackColor = true;
            this.btnPassRangeApply.Click += new System.EventHandler(this.btnPassRangeApply_Click);
            // 
            // txtPassRangeIlUnif
            // 
            this.txtPassRangeIlUnif.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtPassRangeIlUnif.DecimalPlaces = 1;
            this.txtPassRangeIlUnif.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtPassRangeIlUnif.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtPassRangeIlUnif.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtPassRangeIlUnif.Location = new System.Drawing.Point(131, 6);
            this.txtPassRangeIlUnif.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtPassRangeIlUnif.Name = "txtPassRangeIlUnif";
            this.txtPassRangeIlUnif.Size = new System.Drawing.Size(69, 25);
            this.txtPassRangeIlUnif.TabIndex = 2;
            this.txtPassRangeIlUnif.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassRangeIlUnif.Value = new decimal(new int[] {
            6,
            0,
            0,
            65536});
            // 
            // txtPassRangeIlMin
            // 
            this.txtPassRangeIlMin.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtPassRangeIlMin.DecimalPlaces = 1;
            this.txtPassRangeIlMin.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtPassRangeIlMin.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtPassRangeIlMin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtPassRangeIlMin.Location = new System.Drawing.Point(26, 6);
            this.txtPassRangeIlMin.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtPassRangeIlMin.Name = "txtPassRangeIlMin";
            this.txtPassRangeIlMin.Size = new System.Drawing.Size(69, 25);
            this.txtPassRangeIlMin.TabIndex = 1;
            this.txtPassRangeIlMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPassRangeIlMin.Value = new decimal(new int[] {
            108,
            0,
            0,
            -2147418112});
            // 
            // inspectionGrid
            // 
            this.inspectionGrid.AllowUserToAddRows = false;
            this.inspectionGrid.AllowUserToResizeColumns = false;
            this.inspectionGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.inspectionGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.inspectionGrid.ColorFail = System.Drawing.Color.Tomato;
            this.inspectionGrid.ColorPass = System.Drawing.Color.Black;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.inspectionGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.inspectionGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.inspectionGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.inspectionGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.inspectionGrid.Location = new System.Drawing.Point(3, 38);
            this.inspectionGrid.Name = "inspectionGrid";
            this.inspectionGrid.ReadOnly = true;
            this.inspectionGrid.RowHeadersVisible = false;
            this.inspectionGrid.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.inspectionGrid.RowTemplate.DefaultCellStyle.Format = "N2";
            this.inspectionGrid.RowTemplate.Height = 23;
            this.inspectionGrid.Size = new System.Drawing.Size(476, 190);
            this.inspectionGrid.TabIndex = 0;
            // 
            // MuxBudForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 662);
            this.Controls.Add(this.hdgvChipNos);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.grpEtc);
            this.Controls.Add(this.tss);
            this.Controls.Add(this.grpGraphAnalysis);
            this.Controls.Add(this.grpMeasurement);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MuxBudForm";
            this.Text = "CWDM Mux FA  Code name Bud";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCwdmMuxFa_FormClosing);
            this.Load += new System.EventHandler(this.frmCwdmMuxFa_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hdgvChipNos)).EndInit();
            this.grpMeasurement.ResumeLayout(false);
            this.grpMeasurement.PerformLayout();
            this.grpGraphAnalysis.ResumeLayout(false);
            this.tss.ResumeLayout(false);
            this.tss.PerformLayout();
            this.grpEtc.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.grpAutosave.ResumeLayout(false);
            this.grpAutosave.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabAnalysis.ResumeLayout(false);
            this.tabAnalysis.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassRangeIlUnif)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassRangeIlMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inspectionGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    internal System.Windows.Forms.Button btnMeasure;
    internal System.Windows.Forms.Button btnDelAllChipNos;
    internal System.Windows.Forms.TextBox txtChipCnt;
    internal System.Windows.Forms.Button btnChipNoOk;
    internal System.Windows.Forms.TextBox txtFisrtChipNo;
    internal HanDataGridView.HanDataGridView hdgvChipNos;
    internal System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.GroupBox grpMeasurement;
    private System.Windows.Forms.GroupBox grpGraphAnalysis;
    internal System.Windows.Forms.Label lbChipNo;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbStatus;
    internal System.Windows.Forms.Button btnOpenStages;
    private System.Windows.Forms.GroupBox grpEtc;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    internal System.Windows.Forms.CheckBox chkAutoReturn;
    internal System.Windows.Forms.CheckBox chkFaArrangement;
    internal System.Windows.Forms.CheckBox chkAlignment;
    internal System.Windows.Forms.CheckBox chkMeasurement;
    internal System.Windows.Forms.CheckBox chkAutoSave;
    internal System.Windows.Forms.GroupBox grpAutosave;
    internal System.Windows.Forms.RadioButton rbtnAutoSaveRng;
    internal System.Windows.Forms.RadioButton rbtnAutoSaveFull;
    internal System.Windows.Forms.GroupBox GroupBox4;
    internal System.Windows.Forms.RadioButton rbtnGain2;
    internal System.Windows.Forms.RadioButton rbtnGain1;
    internal System.Windows.Forms.GroupBox GroupBox1;
    internal System.Windows.Forms.RadioButton rbtnFA_MMF;
    internal System.Windows.Forms.RadioButton rbtnFA_SMF;
    internal System.Windows.Forms.GroupBox GroupBox6;
    internal System.Windows.Forms.RadioButton rbtnChDirReverse;
    internal System.Windows.Forms.RadioButton rbtnChDirForward;
    internal System.Windows.Forms.Label Label9;
    internal System.Windows.Forms.TextBox txtSaveRangeStop;
    internal System.Windows.Forms.TextBox txtSaveRangeStart;
    internal System.Windows.Forms.Label Label11;
    internal System.Windows.Forms.Label Label36;
    internal System.Windows.Forms.Label lbSaveFolderPath;
    internal System.Windows.Forms.ComboBox cbCorepitch;
    internal System.Windows.Forms.Button btnSaveFolder;
    internal System.Windows.Forms.Label Label38;
    internal System.Windows.Forms.Label Label39;
    internal System.Windows.Forms.TextBox txtChipWidth;
    internal System.Windows.Forms.Label Label40;
    internal System.Windows.Forms.TextBox txtChipNo;
    private System.Windows.Forms.CheckBox checkScan;
    internal System.Windows.Forms.TextBox txtScanRange;
    internal System.Windows.Forms.CheckBox chkCenterStage;
    internal System.Windows.Forms.Label label1;
    internal System.Windows.Forms.Label label2;
    internal System.Windows.Forms.Label Label30;
    internal System.Windows.Forms.Label Label29;
    internal System.Windows.Forms.PictureBox PictureBox2;
    internal System.Windows.Forms.PictureBox PictureBox1;
    private System.Windows.Forms.TabControl tabControl2;
    private System.Windows.Forms.TabPage tabAnalysis;
    internal System.Windows.Forms.Label label5;
    internal System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnPassRangeApply;
    private System.Windows.Forms.NumericUpDown txtPassRangeIlUnif;
    private System.Windows.Forms.NumericUpDown txtPassRangeIlMin;
    private Neon.Aligner.InspectionGrid inspectionGrid;
    internal System.Windows.Forms.Label Label16;
    internal System.Windows.Forms.CheckBox chkWaferFolder;
    private DrBae.TnM.UI.WdmGraph _wg;
}
