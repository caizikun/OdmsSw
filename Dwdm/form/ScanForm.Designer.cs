
namespace ScanTest
{
    partial class ScanForm
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
			this.btnPDInit = new System.Windows.Forms.Button();
			this.Label28 = new System.Windows.Forms.Label();
			this.uiAligner1 = new System.Windows.Forms.ComboBox();
			this.Label7 = new System.Windows.Forms.Label();
			this.cbAxis1 = new System.Windows.Forms.ComboBox();
			this.groupBoxScanAxis = new System.Windows.Forms.GroupBox();
			this.chkAxis3 = new System.Windows.Forms.CheckBox();
			this.uiAligner3 = new System.Windows.Forms.ComboBox();
			this.cboMoveOption3 = new System.Windows.Forms.ComboBox();
			this.txtScanStep3 = new System.Windows.Forms.TextBox();
			this.txtScanRange3 = new System.Windows.Forms.TextBox();
			this.cbAxis3 = new System.Windows.Forms.ComboBox();
			this.uiMoveOrigin2 = new System.Windows.Forms.Button();
			this.uiMoveOrigin1 = new System.Windows.Forms.Button();
			this.uiAligner2 = new System.Windows.Forms.ComboBox();
			this.uiCenter2 = new System.Windows.Forms.TextBox();
			this.uiCenter1 = new System.Windows.Forms.TextBox();
			this.uiPeak2 = new System.Windows.Forms.TextBox();
			this.uiPeak1 = new System.Windows.Forms.TextBox();
			this.uiOrigin2 = new System.Windows.Forms.TextBox();
			this.uiOrigin1 = new System.Windows.Forms.TextBox();
			this.uiMoveCenter2 = new System.Windows.Forms.Button();
			this.uiMovePeak2 = new System.Windows.Forms.Button();
			this.uiMovePeak1 = new System.Windows.Forms.Button();
			this.uiMoveCenter1 = new System.Windows.Forms.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cboMoveOption2 = new System.Windows.Forms.ComboBox();
			this.cboMoveOption1 = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.txtScanStep2 = new System.Windows.Forms.TextBox();
			this.txtScanRange2 = new System.Windows.Forms.TextBox();
			this.txtScanStep1 = new System.Windows.Forms.TextBox();
			this.txtScanRange1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkAxis2 = new System.Windows.Forms.CheckBox();
			this.cbAxis2 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSaveName = new System.Windows.Forms.TextBox();
			this.cboAvgTime = new System.Windows.Forms.ComboBox();
			this.label19 = new System.Windows.Forms.Label();
			this.btnScan = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.groupBoxSave = new System.Windows.Forms.GroupBox();
			this.btnSaveFolder = new System.Windows.Forms.Button();
			this.groupBoxOpm = new System.Windows.Forms.GroupBox();
			this.uiTestMode = new System.Windows.Forms.RadioButton();
			this.uiAppOpm = new System.Windows.Forms.RadioButton();
			this.chkPDmA = new System.Windows.Forms.CheckBox();
			this.label17 = new System.Windows.Forms.Label();
			this.txtDaqResponsivity = new System.Windows.Forms.TextBox();
			this.txtDaqResistance = new System.Windows.Forms.TextBox();
			this.cboRse = new System.Windows.Forms.ComboBox();
			this.cboAiCh = new System.Windows.Forms.ComboBox();
			this.cboDaqPrimary = new System.Windows.Forms.ComboBox();
			this.txtPd2Address = new System.Windows.Forms.TextBox();
			this.uiEsm = new System.Windows.Forms.RadioButton();
			this.uiDaqOpm = new System.Windows.Forms.RadioButton();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusAxis1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusPos1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusAxis2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusPos2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusAxis3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusPos3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.chkReturnOrigin = new System.Windows.Forms.CheckBox();
			this.groupBoxScan = new System.Windows.Forms.GroupBox();
			this.uiSaveTime = new System.Windows.Forms.CheckBox();
			this.groupBoxScanAxis.SuspendLayout();
			this.groupBoxSave.SuspendLayout();
			this.groupBoxOpm.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.groupBoxScan.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPDInit
			// 
			this.btnPDInit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPDInit.ForeColor = System.Drawing.Color.DarkRed;
			this.btnPDInit.Location = new System.Drawing.Point(258, 144);
			this.btnPDInit.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnPDInit.Name = "btnPDInit";
			this.btnPDInit.Size = new System.Drawing.Size(106, 33);
			this.btnPDInit.TabIndex = 140;
			this.btnPDInit.Text = "OPM 초기화";
			this.btnPDInit.UseVisualStyleBackColor = true;
			this.btnPDInit.Click += new System.EventHandler(this.btnPDInit_Click);
			// 
			// Label28
			// 
			this.Label28.AutoSize = true;
			this.Label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.Label28.ForeColor = System.Drawing.Color.Black;
			this.Label28.Location = new System.Drawing.Point(65, 25);
			this.Label28.Name = "Label28";
			this.Label28.Size = new System.Drawing.Size(50, 16);
			this.Label28.TabIndex = 162;
			this.Label28.Text = "Aligner";
			this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// uiAligner1
			// 
			this.uiAligner1.BackColor = System.Drawing.Color.Black;
			this.uiAligner1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.uiAligner1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiAligner1.ForeColor = System.Drawing.Color.White;
			this.uiAligner1.FormattingEnabled = true;
			this.uiAligner1.Location = new System.Drawing.Point(58, 49);
			this.uiAligner1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiAligner1.Name = "uiAligner1";
			this.uiAligner1.Size = new System.Drawing.Size(64, 23);
			this.uiAligner1.TabIndex = 163;
			// 
			// Label7
			// 
			this.Label7.AutoSize = true;
			this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.Label7.ForeColor = System.Drawing.Color.Black;
			this.Label7.Location = new System.Drawing.Point(27, 52);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(25, 16);
			this.Label7.TabIndex = 154;
			this.Label7.Text = "1st";
			// 
			// cbAxis1
			// 
			this.cbAxis1.BackColor = System.Drawing.Color.Black;
			this.cbAxis1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAxis1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbAxis1.ForeColor = System.Drawing.Color.White;
			this.cbAxis1.FormattingEnabled = true;
			this.cbAxis1.Location = new System.Drawing.Point(125, 49);
			this.cbAxis1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cbAxis1.Name = "cbAxis1";
			this.cbAxis1.Size = new System.Drawing.Size(43, 23);
			this.cbAxis1.TabIndex = 155;
			// 
			// groupBoxScanAxis
			// 
			this.groupBoxScanAxis.Controls.Add(this.chkAxis3);
			this.groupBoxScanAxis.Controls.Add(this.uiAligner3);
			this.groupBoxScanAxis.Controls.Add(this.cboMoveOption3);
			this.groupBoxScanAxis.Controls.Add(this.txtScanStep3);
			this.groupBoxScanAxis.Controls.Add(this.txtScanRange3);
			this.groupBoxScanAxis.Controls.Add(this.cbAxis3);
			this.groupBoxScanAxis.Controls.Add(this.uiMoveOrigin2);
			this.groupBoxScanAxis.Controls.Add(this.uiMoveOrigin1);
			this.groupBoxScanAxis.Controls.Add(this.uiAligner2);
			this.groupBoxScanAxis.Controls.Add(this.uiCenter2);
			this.groupBoxScanAxis.Controls.Add(this.uiCenter1);
			this.groupBoxScanAxis.Controls.Add(this.uiPeak2);
			this.groupBoxScanAxis.Controls.Add(this.uiPeak1);
			this.groupBoxScanAxis.Controls.Add(this.uiOrigin2);
			this.groupBoxScanAxis.Controls.Add(this.uiOrigin1);
			this.groupBoxScanAxis.Controls.Add(this.uiMoveCenter2);
			this.groupBoxScanAxis.Controls.Add(this.uiMovePeak2);
			this.groupBoxScanAxis.Controls.Add(this.uiMovePeak1);
			this.groupBoxScanAxis.Controls.Add(this.uiMoveCenter1);
			this.groupBoxScanAxis.Controls.Add(this.label10);
			this.groupBoxScanAxis.Controls.Add(this.label9);
			this.groupBoxScanAxis.Controls.Add(this.label6);
			this.groupBoxScanAxis.Controls.Add(this.label2);
			this.groupBoxScanAxis.Controls.Add(this.cboMoveOption2);
			this.groupBoxScanAxis.Controls.Add(this.cboMoveOption1);
			this.groupBoxScanAxis.Controls.Add(this.label5);
			this.groupBoxScanAxis.Controls.Add(this.txtScanStep2);
			this.groupBoxScanAxis.Controls.Add(this.txtScanRange2);
			this.groupBoxScanAxis.Controls.Add(this.txtScanStep1);
			this.groupBoxScanAxis.Controls.Add(this.txtScanRange1);
			this.groupBoxScanAxis.Controls.Add(this.label1);
			this.groupBoxScanAxis.Controls.Add(this.chkAxis2);
			this.groupBoxScanAxis.Controls.Add(this.cbAxis2);
			this.groupBoxScanAxis.Controls.Add(this.uiAligner1);
			this.groupBoxScanAxis.Controls.Add(this.cbAxis1);
			this.groupBoxScanAxis.Controls.Add(this.Label7);
			this.groupBoxScanAxis.Controls.Add(this.Label28);
			this.groupBoxScanAxis.Location = new System.Drawing.Point(14, 433);
			this.groupBoxScanAxis.Name = "groupBoxScanAxis";
			this.groupBoxScanAxis.Size = new System.Drawing.Size(772, 172);
			this.groupBoxScanAxis.TabIndex = 166;
			this.groupBoxScanAxis.TabStop = false;
			this.groupBoxScanAxis.Text = "Scan Axis";
			// 
			// chkAxis3
			// 
			this.chkAxis3.AutoSize = true;
			this.chkAxis3.Location = new System.Drawing.Point(6, 124);
			this.chkAxis3.Name = "chkAxis3";
			this.chkAxis3.Size = new System.Drawing.Size(47, 21);
			this.chkAxis3.TabIndex = 294;
			this.chkAxis3.Text = "3rd";
			this.chkAxis3.UseVisualStyleBackColor = true;
			// 
			// uiAligner3
			// 
			this.uiAligner3.BackColor = System.Drawing.Color.Black;
			this.uiAligner3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.uiAligner3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiAligner3.ForeColor = System.Drawing.Color.White;
			this.uiAligner3.FormattingEnabled = true;
			this.uiAligner3.Location = new System.Drawing.Point(58, 122);
			this.uiAligner3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiAligner3.Name = "uiAligner3";
			this.uiAligner3.Size = new System.Drawing.Size(64, 23);
			this.uiAligner3.TabIndex = 292;
			// 
			// cboMoveOption3
			// 
			this.cboMoveOption3.BackColor = System.Drawing.Color.Black;
			this.cboMoveOption3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMoveOption3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboMoveOption3.ForeColor = System.Drawing.Color.White;
			this.cboMoveOption3.FormattingEnabled = true;
			this.cboMoveOption3.Items.AddRange(new object[] {
            "-R → +R",
            "0 → +R",
            "0 →  -R"});
			this.cboMoveOption3.Location = new System.Drawing.Point(275, 122);
			this.cboMoveOption3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboMoveOption3.Name = "cboMoveOption3";
			this.cboMoveOption3.Size = new System.Drawing.Size(86, 23);
			this.cboMoveOption3.TabIndex = 286;
			// 
			// txtScanStep3
			// 
			this.txtScanStep3.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanStep3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanStep3.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanStep3.Location = new System.Drawing.Point(224, 122);
			this.txtScanStep3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanStep3.Name = "txtScanStep3";
			this.txtScanStep3.Size = new System.Drawing.Size(47, 23);
			this.txtScanStep3.TabIndex = 285;
			this.txtScanStep3.Text = "1";
			this.txtScanStep3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtScanRange3
			// 
			this.txtScanRange3.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanRange3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanRange3.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanRange3.Location = new System.Drawing.Point(171, 122);
			this.txtScanRange3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanRange3.Name = "txtScanRange3";
			this.txtScanRange3.Size = new System.Drawing.Size(54, 23);
			this.txtScanRange3.TabIndex = 284;
			this.txtScanRange3.Text = "10";
			this.txtScanRange3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// cbAxis3
			// 
			this.cbAxis3.BackColor = System.Drawing.Color.Black;
			this.cbAxis3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAxis3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbAxis3.ForeColor = System.Drawing.Color.White;
			this.cbAxis3.FormattingEnabled = true;
			this.cbAxis3.Location = new System.Drawing.Point(125, 122);
			this.cbAxis3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cbAxis3.Name = "cbAxis3";
			this.cbAxis3.Size = new System.Drawing.Size(43, 23);
			this.cbAxis3.TabIndex = 283;
			// 
			// uiMoveOrigin2
			// 
			this.uiMoveOrigin2.Location = new System.Drawing.Point(460, 85);
			this.uiMoveOrigin2.Name = "uiMoveOrigin2";
			this.uiMoveOrigin2.Size = new System.Drawing.Size(46, 23);
			this.uiMoveOrigin2.TabIndex = 282;
			this.uiMoveOrigin2.Text = "0◀";
			this.uiMoveOrigin2.UseVisualStyleBackColor = true;
			// 
			// uiMoveOrigin1
			// 
			this.uiMoveOrigin1.Location = new System.Drawing.Point(460, 49);
			this.uiMoveOrigin1.Name = "uiMoveOrigin1";
			this.uiMoveOrigin1.Size = new System.Drawing.Size(46, 23);
			this.uiMoveOrigin1.TabIndex = 281;
			this.uiMoveOrigin1.Text = "0◀";
			this.uiMoveOrigin1.UseVisualStyleBackColor = true;
			// 
			// uiAligner2
			// 
			this.uiAligner2.BackColor = System.Drawing.Color.Black;
			this.uiAligner2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.uiAligner2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiAligner2.ForeColor = System.Drawing.Color.White;
			this.uiAligner2.FormattingEnabled = true;
			this.uiAligner2.Location = new System.Drawing.Point(58, 85);
			this.uiAligner2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiAligner2.Name = "uiAligner2";
			this.uiAligner2.Size = new System.Drawing.Size(64, 23);
			this.uiAligner2.TabIndex = 280;
			// 
			// uiCenter2
			// 
			this.uiCenter2.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiCenter2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiCenter2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiCenter2.Location = new System.Drawing.Point(640, 85);
			this.uiCenter2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiCenter2.Name = "uiCenter2";
			this.uiCenter2.Size = new System.Drawing.Size(80, 23);
			this.uiCenter2.TabIndex = 278;
			this.uiCenter2.Text = "NaN";
			this.uiCenter2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiCenter1
			// 
			this.uiCenter1.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiCenter1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiCenter1.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiCenter1.Location = new System.Drawing.Point(640, 49);
			this.uiCenter1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiCenter1.Name = "uiCenter1";
			this.uiCenter1.Size = new System.Drawing.Size(80, 23);
			this.uiCenter1.TabIndex = 277;
			this.uiCenter1.Text = "NaN";
			this.uiCenter1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiPeak2
			// 
			this.uiPeak2.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiPeak2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiPeak2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiPeak2.Location = new System.Drawing.Point(514, 85);
			this.uiPeak2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiPeak2.Name = "uiPeak2";
			this.uiPeak2.Size = new System.Drawing.Size(73, 23);
			this.uiPeak2.TabIndex = 276;
			this.uiPeak2.Text = "NaN";
			this.uiPeak2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiPeak1
			// 
			this.uiPeak1.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiPeak1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiPeak1.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiPeak1.Location = new System.Drawing.Point(514, 49);
			this.uiPeak1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiPeak1.Name = "uiPeak1";
			this.uiPeak1.Size = new System.Drawing.Size(73, 23);
			this.uiPeak1.TabIndex = 275;
			this.uiPeak1.Text = "NaN";
			this.uiPeak1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiOrigin2
			// 
			this.uiOrigin2.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiOrigin2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiOrigin2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiOrigin2.Location = new System.Drawing.Point(384, 85);
			this.uiOrigin2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiOrigin2.Name = "uiOrigin2";
			this.uiOrigin2.Size = new System.Drawing.Size(79, 23);
			this.uiOrigin2.TabIndex = 274;
			this.uiOrigin2.Text = "NaN";
			this.uiOrigin2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiOrigin1
			// 
			this.uiOrigin1.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiOrigin1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiOrigin1.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiOrigin1.Location = new System.Drawing.Point(384, 49);
			this.uiOrigin1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiOrigin1.Name = "uiOrigin1";
			this.uiOrigin1.Size = new System.Drawing.Size(79, 23);
			this.uiOrigin1.TabIndex = 273;
			this.uiOrigin1.Text = "NaN";
			this.uiOrigin1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiMoveCenter2
			// 
			this.uiMoveCenter2.Location = new System.Drawing.Point(719, 85);
			this.uiMoveCenter2.Name = "uiMoveCenter2";
			this.uiMoveCenter2.Size = new System.Drawing.Size(46, 23);
			this.uiMoveCenter2.TabIndex = 272;
			this.uiMoveCenter2.Text = "|◀";
			this.uiMoveCenter2.UseVisualStyleBackColor = true;
			// 
			// uiMovePeak2
			// 
			this.uiMovePeak2.Location = new System.Drawing.Point(586, 85);
			this.uiMovePeak2.Name = "uiMovePeak2";
			this.uiMovePeak2.Size = new System.Drawing.Size(46, 23);
			this.uiMovePeak2.TabIndex = 271;
			this.uiMovePeak2.Text = "◀|";
			this.uiMovePeak2.UseVisualStyleBackColor = true;
			// 
			// uiMovePeak1
			// 
			this.uiMovePeak1.Location = new System.Drawing.Point(586, 49);
			this.uiMovePeak1.Name = "uiMovePeak1";
			this.uiMovePeak1.Size = new System.Drawing.Size(46, 23);
			this.uiMovePeak1.TabIndex = 270;
			this.uiMovePeak1.Text = "◀|";
			this.uiMovePeak1.UseVisualStyleBackColor = true;
			// 
			// uiMoveCenter1
			// 
			this.uiMoveCenter1.Location = new System.Drawing.Point(719, 49);
			this.uiMoveCenter1.Name = "uiMoveCenter1";
			this.uiMoveCenter1.Size = new System.Drawing.Size(46, 23);
			this.uiMoveCenter1.TabIndex = 269;
			this.uiMoveCenter1.Text = "|◀";
			this.uiMoveCenter1.UseVisualStyleBackColor = true;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label10.ForeColor = System.Drawing.Color.Black;
			this.label10.Location = new System.Drawing.Point(657, 25);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(47, 16);
			this.label10.TabIndex = 266;
			this.label10.Text = "Center";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label9.ForeColor = System.Drawing.Color.Black;
			this.label9.Location = new System.Drawing.Point(530, 25);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(40, 16);
			this.label9.TabIndex = 263;
			this.label9.Text = "Peak";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label6.ForeColor = System.Drawing.Color.Black;
			this.label6.Location = new System.Drawing.Point(402, 25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(43, 16);
			this.label6.TabIndex = 260;
			this.label6.Text = "Origin";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Location = new System.Drawing.Point(130, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 16);
			this.label2.TabIndex = 259;
			this.label2.Text = "Axis";
			// 
			// cboMoveOption2
			// 
			this.cboMoveOption2.BackColor = System.Drawing.Color.Black;
			this.cboMoveOption2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMoveOption2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboMoveOption2.ForeColor = System.Drawing.Color.White;
			this.cboMoveOption2.FormattingEnabled = true;
			this.cboMoveOption2.Items.AddRange(new object[] {
            "-R → +R",
            "0 → +R",
            "0 →  -R"});
			this.cboMoveOption2.Location = new System.Drawing.Point(275, 85);
			this.cboMoveOption2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboMoveOption2.Name = "cboMoveOption2";
			this.cboMoveOption2.Size = new System.Drawing.Size(86, 23);
			this.cboMoveOption2.TabIndex = 247;
			// 
			// cboMoveOption1
			// 
			this.cboMoveOption1.BackColor = System.Drawing.Color.Black;
			this.cboMoveOption1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboMoveOption1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboMoveOption1.ForeColor = System.Drawing.Color.White;
			this.cboMoveOption1.FormattingEnabled = true;
			this.cboMoveOption1.Items.AddRange(new object[] {
            "-R → +R",
            "0 → +R",
            "0 →  -R"});
			this.cboMoveOption1.Location = new System.Drawing.Point(275, 49);
			this.cboMoveOption1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboMoveOption1.Name = "cboMoveOption1";
			this.cboMoveOption1.Size = new System.Drawing.Size(86, 23);
			this.cboMoveOption1.TabIndex = 246;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label5.ForeColor = System.Drawing.Color.Black;
			this.label5.Location = new System.Drawing.Point(288, 25);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(61, 16);
			this.label5.TabIndex = 245;
			this.label5.Text = "Direction";
			// 
			// txtScanStep2
			// 
			this.txtScanStep2.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanStep2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanStep2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanStep2.Location = new System.Drawing.Point(224, 85);
			this.txtScanStep2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanStep2.Name = "txtScanStep2";
			this.txtScanStep2.Size = new System.Drawing.Size(47, 23);
			this.txtScanStep2.TabIndex = 244;
			this.txtScanStep2.Text = "1";
			this.txtScanStep2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtScanRange2
			// 
			this.txtScanRange2.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanRange2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanRange2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanRange2.Location = new System.Drawing.Point(171, 85);
			this.txtScanRange2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanRange2.Name = "txtScanRange2";
			this.txtScanRange2.Size = new System.Drawing.Size(54, 23);
			this.txtScanRange2.TabIndex = 242;
			this.txtScanRange2.Text = "10";
			this.txtScanRange2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtScanStep1
			// 
			this.txtScanStep1.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanStep1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanStep1.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanStep1.Location = new System.Drawing.Point(224, 49);
			this.txtScanStep1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanStep1.Name = "txtScanStep1";
			this.txtScanStep1.Size = new System.Drawing.Size(47, 23);
			this.txtScanStep1.TabIndex = 235;
			this.txtScanStep1.Text = "1";
			this.txtScanStep1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtScanRange1
			// 
			this.txtScanRange1.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtScanRange1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtScanRange1.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtScanRange1.Location = new System.Drawing.Point(171, 49);
			this.txtScanRange1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtScanRange1.Name = "txtScanRange1";
			this.txtScanRange1.Size = new System.Drawing.Size(54, 23);
			this.txtScanRange1.TabIndex = 234;
			this.txtScanRange1.Text = "10";
			this.txtScanRange1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(175, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 16);
			this.label1.TabIndex = 169;
			this.label1.Text = "Range     Step";
			// 
			// chkAxis2
			// 
			this.chkAxis2.AutoSize = true;
			this.chkAxis2.Location = new System.Drawing.Point(6, 86);
			this.chkAxis2.Name = "chkAxis2";
			this.chkAxis2.Size = new System.Drawing.Size(50, 21);
			this.chkAxis2.TabIndex = 168;
			this.chkAxis2.Text = "2nd";
			this.chkAxis2.UseVisualStyleBackColor = true;
			// 
			// cbAxis2
			// 
			this.cbAxis2.BackColor = System.Drawing.Color.Black;
			this.cbAxis2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAxis2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cbAxis2.ForeColor = System.Drawing.Color.White;
			this.cbAxis2.FormattingEnabled = true;
			this.cbAxis2.Location = new System.Drawing.Point(125, 85);
			this.cbAxis2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cbAxis2.Name = "cbAxis2";
			this.cbAxis2.Size = new System.Drawing.Size(43, 23);
			this.cbAxis2.TabIndex = 167;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(17, 67);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 16);
			this.label3.TabIndex = 256;
			this.label3.Text = "File Name";
			// 
			// txtSaveName
			// 
			this.txtSaveName.BackColor = System.Drawing.Color.Black;
			this.txtSaveName.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSaveName.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtSaveName.Location = new System.Drawing.Point(93, 63);
			this.txtSaveName.Name = "txtSaveName";
			this.txtSaveName.Size = new System.Drawing.Size(271, 25);
			this.txtSaveName.TabIndex = 255;
			this.txtSaveName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.txtSaveName.TextChanged += new System.EventHandler(this.txtSaveName_TextChanged);
			// 
			// cboAvgTime
			// 
			this.cboAvgTime.BackColor = System.Drawing.Color.Black;
			this.cboAvgTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAvgTime.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboAvgTime.ForeColor = System.Drawing.Color.White;
			this.cboAvgTime.FormattingEnabled = true;
			this.cboAvgTime.Location = new System.Drawing.Point(258, 112);
			this.cboAvgTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboAvgTime.Name = "cboAvgTime";
			this.cboAvgTime.Size = new System.Drawing.Size(106, 23);
			this.cboAvgTime.TabIndex = 254;
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label19.ForeColor = System.Drawing.Color.Black;
			this.label19.Location = new System.Drawing.Point(298, 92);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(66, 16);
			this.label19.TabIndex = 253;
			this.label19.Text = "Avg Time";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnScan
			// 
			this.btnScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnScan.Location = new System.Drawing.Point(152, 21);
			this.btnScan.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnScan.Name = "btnScan";
			this.btnScan.Size = new System.Drawing.Size(100, 60);
			this.btnScan.TabIndex = 168;
			this.btnScan.Text = "Scan";
			this.btnScan.UseVisualStyleBackColor = true;
			this.btnScan.Click += new System.EventHandler(this.btnScan_ClickAsync);
			// 
			// btnStop
			// 
			this.btnStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStop.ForeColor = System.Drawing.Color.Red;
			this.btnStop.Location = new System.Drawing.Point(264, 21);
			this.btnStop.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(100, 60);
			this.btnStop.TabIndex = 169;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// groupBoxSave
			// 
			this.groupBoxSave.Controls.Add(this.uiSaveTime);
			this.groupBoxSave.Controls.Add(this.btnSaveFolder);
			this.groupBoxSave.Controls.Add(this.txtSaveName);
			this.groupBoxSave.Controls.Add(this.label3);
			this.groupBoxSave.Location = new System.Drawing.Point(14, 202);
			this.groupBoxSave.Name = "groupBoxSave";
			this.groupBoxSave.Size = new System.Drawing.Size(384, 125);
			this.groupBoxSave.TabIndex = 237;
			this.groupBoxSave.TabStop = false;
			this.groupBoxSave.Text = "Save Folder";
			// 
			// btnSaveFolder
			// 
			this.btnSaveFolder.AutoSize = true;
			this.btnSaveFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Neon.Dwdm.Properties.Settings.Default, "ScanForm_SaveFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.btnSaveFolder.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
			this.btnSaveFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnSaveFolder.ForeColor = System.Drawing.Color.DodgerBlue;
			this.btnSaveFolder.Location = new System.Drawing.Point(18, 23);
			this.btnSaveFolder.Name = "btnSaveFolder";
			this.btnSaveFolder.Size = new System.Drawing.Size(346, 32);
			this.btnSaveFolder.TabIndex = 239;
			this.btnSaveFolder.Text = global::Neon.Dwdm.Properties.Settings.Default.ScanForm_SaveFolder;
			this.btnSaveFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSaveFolder.UseVisualStyleBackColor = true;
			this.btnSaveFolder.Click += new System.EventHandler(this.btnSaveFolder_Click);
			// 
			// groupBoxOpm
			// 
			this.groupBoxOpm.Controls.Add(this.uiTestMode);
			this.groupBoxOpm.Controls.Add(this.uiAppOpm);
			this.groupBoxOpm.Controls.Add(this.chkPDmA);
			this.groupBoxOpm.Controls.Add(this.label17);
			this.groupBoxOpm.Controls.Add(this.cboAvgTime);
			this.groupBoxOpm.Controls.Add(this.txtDaqResponsivity);
			this.groupBoxOpm.Controls.Add(this.label19);
			this.groupBoxOpm.Controls.Add(this.txtDaqResistance);
			this.groupBoxOpm.Controls.Add(this.cboRse);
			this.groupBoxOpm.Controls.Add(this.cboAiCh);
			this.groupBoxOpm.Controls.Add(this.cboDaqPrimary);
			this.groupBoxOpm.Controls.Add(this.txtPd2Address);
			this.groupBoxOpm.Controls.Add(this.uiEsm);
			this.groupBoxOpm.Controls.Add(this.uiDaqOpm);
			this.groupBoxOpm.Controls.Add(this.btnPDInit);
			this.groupBoxOpm.Location = new System.Drawing.Point(14, 9);
			this.groupBoxOpm.Name = "groupBoxOpm";
			this.groupBoxOpm.Size = new System.Drawing.Size(384, 185);
			this.groupBoxOpm.TabIndex = 240;
			this.groupBoxOpm.TabStop = false;
			this.groupBoxOpm.Text = "Optical Powermeter";
			// 
			// uiTestMode
			// 
			this.uiTestMode.AutoSize = true;
			this.uiTestMode.Checked = true;
			this.uiTestMode.Location = new System.Drawing.Point(133, 150);
			this.uiTestMode.Name = "uiTestMode";
			this.uiTestMode.Size = new System.Drawing.Size(90, 21);
			this.uiTestMode.TabIndex = 256;
			this.uiTestMode.TabStop = true;
			this.uiTestMode.Text = "Test Mode";
			this.uiTestMode.UseVisualStyleBackColor = true;
			// 
			// uiAppOpm
			// 
			this.uiAppOpm.AutoSize = true;
			this.uiAppOpm.Checked = true;
			this.uiAppOpm.Location = new System.Drawing.Point(11, 150);
			this.uiAppOpm.Name = "uiAppOpm";
			this.uiAppOpm.Size = new System.Drawing.Size(85, 21);
			this.uiAppOpm.TabIndex = 255;
			this.uiAppOpm.TabStop = true;
			this.uiAppOpm.Text = "App OPM";
			this.uiAppOpm.UseVisualStyleBackColor = true;
			// 
			// chkPDmA
			// 
			this.chkPDmA.AutoSize = true;
			this.chkPDmA.Checked = true;
			this.chkPDmA.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkPDmA.Location = new System.Drawing.Point(184, 91);
			this.chkPDmA.Name = "chkPDmA";
			this.chkPDmA.Size = new System.Drawing.Size(47, 21);
			this.chkPDmA.TabIndex = 254;
			this.chkPDmA.Text = "mA";
			this.chkPDmA.UseVisualStyleBackColor = true;
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label17.ForeColor = System.Drawing.Color.Black;
			this.label17.Location = new System.Drawing.Point(258, 54);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(45, 13);
			this.label17.TabIndex = 251;
			this.label17.Text = "Ω, V/W";
			// 
			// txtDaqResponsivity
			// 
			this.txtDaqResponsivity.BackColor = System.Drawing.Color.Black;
			this.txtDaqResponsivity.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDaqResponsivity.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtDaqResponsivity.Location = new System.Drawing.Point(182, 50);
			this.txtDaqResponsivity.Name = "txtDaqResponsivity";
			this.txtDaqResponsivity.Size = new System.Drawing.Size(70, 23);
			this.txtDaqResponsivity.TabIndex = 160;
			this.txtDaqResponsivity.Text = "0.9";
			this.txtDaqResponsivity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtDaqResistance
			// 
			this.txtDaqResistance.BackColor = System.Drawing.Color.Black;
			this.txtDaqResistance.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDaqResistance.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtDaqResistance.Location = new System.Drawing.Point(106, 50);
			this.txtDaqResistance.Name = "txtDaqResistance";
			this.txtDaqResistance.Size = new System.Drawing.Size(70, 23);
			this.txtDaqResistance.TabIndex = 159;
			this.txtDaqResistance.Text = "10000";
			this.txtDaqResistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// cboRse
			// 
			this.cboRse.BackColor = System.Drawing.Color.Black;
			this.cboRse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboRse.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboRse.ForeColor = System.Drawing.Color.White;
			this.cboRse.FormattingEnabled = true;
			this.cboRse.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Tx",
            "Ty"});
			this.cboRse.Location = new System.Drawing.Point(258, 20);
			this.cboRse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboRse.Name = "cboRse";
			this.cboRse.Size = new System.Drawing.Size(106, 23);
			this.cboRse.TabIndex = 158;
			// 
			// cboAiCh
			// 
			this.cboAiCh.BackColor = System.Drawing.Color.Black;
			this.cboAiCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboAiCh.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboAiCh.ForeColor = System.Drawing.Color.White;
			this.cboAiCh.FormattingEnabled = true;
			this.cboAiCh.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Tx",
            "Ty"});
			this.cboAiCh.Location = new System.Drawing.Point(182, 20);
			this.cboAiCh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboAiCh.Name = "cboAiCh";
			this.cboAiCh.Size = new System.Drawing.Size(70, 23);
			this.cboAiCh.TabIndex = 157;
			// 
			// cboDaqPrimary
			// 
			this.cboDaqPrimary.BackColor = System.Drawing.Color.Black;
			this.cboDaqPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboDaqPrimary.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboDaqPrimary.ForeColor = System.Drawing.Color.White;
			this.cboDaqPrimary.FormattingEnabled = true;
			this.cboDaqPrimary.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Tx",
            "Ty"});
			this.cboDaqPrimary.Location = new System.Drawing.Point(106, 20);
			this.cboDaqPrimary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboDaqPrimary.Name = "cboDaqPrimary";
			this.cboDaqPrimary.Size = new System.Drawing.Size(70, 23);
			this.cboDaqPrimary.TabIndex = 156;
			// 
			// txtPd2Address
			// 
			this.txtPd2Address.BackColor = System.Drawing.Color.Black;
			this.txtPd2Address.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPd2Address.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtPd2Address.Location = new System.Drawing.Point(105, 89);
			this.txtPd2Address.Name = "txtPd2Address";
			this.txtPd2Address.Size = new System.Drawing.Size(73, 23);
			this.txtPd2Address.TabIndex = 143;
			this.txtPd2Address.Text = "28";
			this.txtPd2Address.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiEsm
			// 
			this.uiEsm.AutoSize = true;
			this.uiEsm.Location = new System.Drawing.Point(11, 91);
			this.uiEsm.Name = "uiEsm";
			this.uiEsm.Size = new System.Drawing.Size(52, 21);
			this.uiEsm.TabIndex = 142;
			this.uiEsm.Text = "ESM";
			this.uiEsm.UseVisualStyleBackColor = true;
			// 
			// uiDaqOpm
			// 
			this.uiDaqOpm.AutoSize = true;
			this.uiDaqOpm.Location = new System.Drawing.Point(10, 21);
			this.uiDaqOpm.Name = "uiDaqOpm";
			this.uiDaqOpm.Size = new System.Drawing.Size(72, 21);
			this.uiDaqOpm.TabIndex = 141;
			this.uiDaqOpm.Text = "DAQ AI";
			this.uiDaqOpm.UseVisualStyleBackColor = true;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.txtLog);
			this.groupBox6.Location = new System.Drawing.Point(404, 9);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(382, 418);
			this.groupBox6.TabIndex = 242;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(3, 21);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(376, 394);
			this.txtLog.TabIndex = 1;
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusAxis1,
            this.statusPos1,
            this.statusAxis2,
            this.statusPos2,
            this.statusAxis3,
            this.statusPos3});
			this.statusStrip.Location = new System.Drawing.Point(0, 617);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 11, 0);
			this.statusStrip.Size = new System.Drawing.Size(797, 22);
			this.statusStrip.TabIndex = 243;
			this.statusStrip.Text = "statusStrip1";
			// 
			// statusAxis1
			// 
			this.statusAxis1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusAxis1.ForeColor = System.Drawing.Color.DarkCyan;
			this.statusAxis1.Name = "statusAxis1";
			this.statusAxis1.Size = new System.Drawing.Size(23, 17);
			this.statusAxis1.Text = "1 :";
			// 
			// statusPos1
			// 
			this.statusPos1.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusPos1.ForeColor = System.Drawing.Color.DarkCyan;
			this.statusPos1.Name = "statusPos1";
			this.statusPos1.Size = new System.Drawing.Size(15, 17);
			this.statusPos1.Text = "0";
			// 
			// statusAxis2
			// 
			this.statusAxis2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusAxis2.ForeColor = System.Drawing.Color.Magenta;
			this.statusAxis2.Name = "statusAxis2";
			this.statusAxis2.Size = new System.Drawing.Size(23, 17);
			this.statusAxis2.Text = "2 :";
			// 
			// statusPos2
			// 
			this.statusPos2.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusPos2.ForeColor = System.Drawing.Color.Magenta;
			this.statusPos2.Name = "statusPos2";
			this.statusPos2.Size = new System.Drawing.Size(15, 17);
			this.statusPos2.Text = "0";
			// 
			// statusAxis3
			// 
			this.statusAxis3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusAxis3.ForeColor = System.Drawing.Color.Brown;
			this.statusAxis3.Name = "statusAxis3";
			this.statusAxis3.Size = new System.Drawing.Size(24, 17);
			this.statusAxis3.Text = "Z :";
			// 
			// statusPos3
			// 
			this.statusPos3.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.statusPos3.ForeColor = System.Drawing.Color.Brown;
			this.statusPos3.Name = "statusPos3";
			this.statusPos3.Size = new System.Drawing.Size(15, 17);
			this.statusPos3.Text = "0";
			// 
			// chkReturnOrigin
			// 
			this.chkReturnOrigin.AutoSize = true;
			this.chkReturnOrigin.Checked = true;
			this.chkReturnOrigin.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkReturnOrigin.Location = new System.Drawing.Point(18, 42);
			this.chkReturnOrigin.Name = "chkReturnOrigin";
			this.chkReturnOrigin.Size = new System.Drawing.Size(109, 21);
			this.chkReturnOrigin.TabIndex = 283;
			this.chkReturnOrigin.Text = "Return Origin";
			this.chkReturnOrigin.UseVisualStyleBackColor = true;
			// 
			// groupBoxScan
			// 
			this.groupBoxScan.Controls.Add(this.btnScan);
			this.groupBoxScan.Controls.Add(this.chkReturnOrigin);
			this.groupBoxScan.Controls.Add(this.btnStop);
			this.groupBoxScan.Location = new System.Drawing.Point(14, 333);
			this.groupBoxScan.Name = "groupBoxScan";
			this.groupBoxScan.Size = new System.Drawing.Size(384, 94);
			this.groupBoxScan.TabIndex = 257;
			this.groupBoxScan.TabStop = false;
			this.groupBoxScan.Text = "Scan";
			// 
			// uiSaveTime
			// 
			this.uiSaveTime.AutoSize = true;
			this.uiSaveTime.Checked = true;
			this.uiSaveTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.uiSaveTime.Location = new System.Drawing.Point(18, 98);
			this.uiSaveTime.Name = "uiSaveTime";
			this.uiSaveTime.Size = new System.Drawing.Size(112, 21);
			this.uiSaveTime.TabIndex = 295;
			this.uiSaveTime.Text = "Add SaveTime";
			this.uiSaveTime.UseVisualStyleBackColor = true;
			// 
			// ScanForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(797, 639);
			this.Controls.Add(this.groupBoxScan);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBoxOpm);
			this.Controls.Add(this.groupBoxSave);
			this.Controls.Add(this.groupBoxScanAxis);
			this.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "ScanForm";
			this.Text = "frmScanPD";
			this.Load += new System.EventHandler(this.ScanForm_Load);
			this.groupBoxScanAxis.ResumeLayout(false);
			this.groupBoxScanAxis.PerformLayout();
			this.groupBoxSave.ResumeLayout(false);
			this.groupBoxSave.PerformLayout();
			this.groupBoxOpm.ResumeLayout(false);
			this.groupBoxOpm.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.groupBoxScan.ResumeLayout(false);
			this.groupBoxScan.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button btnPDInit;
        internal System.Windows.Forms.Label Label28;
        internal System.Windows.Forms.ComboBox uiAligner1;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.ComboBox cbAxis1;
        private System.Windows.Forms.GroupBox groupBoxScanAxis;
        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkAxis2;
        internal System.Windows.Forms.ComboBox cbAxis2;
        internal System.Windows.Forms.TextBox txtScanStep1;
        internal System.Windows.Forms.TextBox txtScanRange1;
        internal System.Windows.Forms.Button btnScan;
        internal System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox groupBoxSave;
        private System.Windows.Forms.Button btnSaveFolder;
        private System.Windows.Forms.GroupBox groupBoxOpm;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusPos1;
        private System.Windows.Forms.ToolStripStatusLabel statusPos2;
        internal System.Windows.Forms.ComboBox cboMoveOption1;
        internal System.Windows.Forms.Label label5;
        internal System.Windows.Forms.TextBox txtScanStep2;
        internal System.Windows.Forms.TextBox txtScanRange2;
        internal System.Windows.Forms.ComboBox cboMoveOption2;
        private System.Windows.Forms.RadioButton uiEsm;
        private System.Windows.Forms.RadioButton uiDaqOpm;
        private System.Windows.Forms.TextBox txtPd2Address;
        internal System.Windows.Forms.ComboBox cboRse;
        internal System.Windows.Forms.ComboBox cboAiCh;
        internal System.Windows.Forms.ComboBox cboDaqPrimary;
        internal System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtDaqResponsivity;
        private System.Windows.Forms.TextBox txtDaqResistance;
        internal System.Windows.Forms.ComboBox cboAvgTime;
        internal System.Windows.Forms.Label label19;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSaveName;
        private System.Windows.Forms.CheckBox chkPDmA;
        private System.Windows.Forms.RadioButton uiAppOpm;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button uiMoveCenter2;
        private System.Windows.Forms.Button uiMovePeak2;
        private System.Windows.Forms.Button uiMovePeak1;
        private System.Windows.Forms.Button uiMoveCenter1;
        internal System.Windows.Forms.Label label10;
        internal System.Windows.Forms.Label label9;
        internal System.Windows.Forms.TextBox uiCenter2;
        internal System.Windows.Forms.TextBox uiCenter1;
        internal System.Windows.Forms.TextBox uiPeak2;
        internal System.Windows.Forms.TextBox uiPeak1;
        internal System.Windows.Forms.TextBox uiOrigin2;
        internal System.Windows.Forms.TextBox uiOrigin1;
        internal System.Windows.Forms.ComboBox uiAligner2;
		private System.Windows.Forms.Button uiMoveOrigin2;
		private System.Windows.Forms.Button uiMoveOrigin1;
		private System.Windows.Forms.ToolStripStatusLabel statusAxis1;
		private System.Windows.Forms.ToolStripStatusLabel statusAxis2;
		private System.Windows.Forms.CheckBox chkReturnOrigin;
		private System.Windows.Forms.GroupBox groupBoxScan;
		private System.Windows.Forms.CheckBox chkAxis3;
		internal System.Windows.Forms.ComboBox uiAligner3;
		internal System.Windows.Forms.ComboBox cboMoveOption3;
		internal System.Windows.Forms.TextBox txtScanStep3;
		internal System.Windows.Forms.TextBox txtScanRange3;
		internal System.Windows.Forms.ComboBox cbAxis3;
		private System.Windows.Forms.ToolStripStatusLabel statusPos3;
		private System.Windows.Forms.ToolStripStatusLabel statusAxis3;
		private System.Windows.Forms.RadioButton uiTestMode;
		private System.Windows.Forms.CheckBox uiSaveTime;
	}
}