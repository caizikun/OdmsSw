namespace Neon.Dwdm
{
	partial class ChamberControl
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.txtStatusHumi = new System.Windows.Forms.TextBox();
			this.txtStatusTemp = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.Label39 = new System.Windows.Forms.Label();
			this.wfgChamber = new NationalInstruments.UI.WindowsForms.WaveformGraph();
			this.WaveformPlot2 = new NationalInstruments.UI.WaveformPlot();
			this.XAxis2 = new NationalInstruments.UI.XAxis();
			this.YAxis2 = new NationalInstruments.UI.YAxis();
			this.Label15 = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnStop = new System.Windows.Forms.Button();
			this.btnStart = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.UiChamberSchedule = new Neon.Aligner.ChamberSchedule();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.chkBookSchedule = new System.Windows.Forms.CheckBox();
			this.pickerBookSchedule = new System.Windows.Forms.DateTimePicker();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.wfgChamber)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtStatusHumi);
			this.groupBox1.Controls.Add(this.txtStatusTemp);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.Label39);
			this.groupBox1.Controls.Add(this.wfgChamber);
			this.groupBox1.Controls.Add(this.Label15);
			this.groupBox1.Controls.Add(this.Label12);
			this.groupBox1.Location = new System.Drawing.Point(4, 360);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(400, 175);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Chamber Status";
			// 
			// txtStatusHumi
			// 
			this.txtStatusHumi.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtStatusHumi.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStatusHumi.ForeColor = System.Drawing.Color.Lime;
			this.txtStatusHumi.Location = new System.Drawing.Point(299, 125);
			this.txtStatusHumi.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.txtStatusHumi.Name = "txtStatusHumi";
			this.txtStatusHumi.Size = new System.Drawing.Size(72, 29);
			this.txtStatusHumi.TabIndex = 342;
			this.txtStatusHumi.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtStatusTemp
			// 
			this.txtStatusTemp.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtStatusTemp.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtStatusTemp.ForeColor = System.Drawing.Color.Lime;
			this.txtStatusTemp.Location = new System.Drawing.Point(299, 57);
			this.txtStatusTemp.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.txtStatusTemp.Name = "txtStatusTemp";
			this.txtStatusTemp.Size = new System.Drawing.Size(72, 29);
			this.txtStatusTemp.TabIndex = 178;
			this.txtStatusTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(302, 108);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(59, 15);
			this.label1.TabIndex = 341;
			this.label1.Text = "현재 습도";
			// 
			// Label39
			// 
			this.Label39.AutoSize = true;
			this.Label39.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Label39.Location = new System.Drawing.Point(301, 39);
			this.Label39.Name = "Label39";
			this.Label39.Size = new System.Drawing.Size(59, 15);
			this.Label39.TabIndex = 340;
			this.Label39.Text = "현재 온도";
			// 
			// wfgChamber
			// 
			this.wfgChamber.Font = new System.Drawing.Font("Segoe UI Symbol", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wfgChamber.Location = new System.Drawing.Point(5, 20);
			this.wfgChamber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.wfgChamber.Name = "wfgChamber";
			this.wfgChamber.Plots.AddRange(new NationalInstruments.UI.WaveformPlot[] {
            this.WaveformPlot2});
			this.wfgChamber.Size = new System.Drawing.Size(290, 151);
			this.wfgChamber.TabIndex = 166;
			this.wfgChamber.UseColorGenerator = true;
			this.wfgChamber.XAxes.AddRange(new NationalInstruments.UI.XAxis[] {
            this.XAxis2});
			this.wfgChamber.YAxes.AddRange(new NationalInstruments.UI.YAxis[] {
            this.YAxis2});
			// 
			// WaveformPlot2
			// 
			this.WaveformPlot2.HistoryCapacity = 500000;
			this.WaveformPlot2.LineColor = System.Drawing.Color.Magenta;
			this.WaveformPlot2.LineColorPrecedence = NationalInstruments.UI.ColorPrecedence.UserDefinedColor;
			this.WaveformPlot2.LineWidth = 3F;
			this.WaveformPlot2.XAxis = this.XAxis2;
			this.WaveformPlot2.YAxis = this.YAxis2;
			// 
			// XAxis2
			// 
			this.XAxis2.AutoMinorDivisionFrequency = 5;
			this.XAxis2.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.XAxis2.CaptionVisible = false;
			this.XAxis2.EndLabelsAlwaysVisible = false;
			this.XAxis2.MajorDivisions.GridVisible = true;
			this.XAxis2.MinorDivisions.GridVisible = true;
			this.XAxis2.Mode = NationalInstruments.UI.AxisMode.AutoScaleVisibleLoose;
			this.XAxis2.Range = new NationalInstruments.UI.Range(0D, 2000D);
			this.XAxis2.Visible = false;
			// 
			// YAxis2
			// 
			this.YAxis2.AutoMinorDivisionFrequency = 5;
			this.YAxis2.CaptionFont = new System.Drawing.Font("Segoe UI Symbol", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.YAxis2.MajorDivisions.GridVisible = true;
			this.YAxis2.MinorDivisions.GridVisible = true;
			this.YAxis2.Range = new NationalInstruments.UI.Range(-80D, 80D);
			// 
			// Label15
			// 
			this.Label15.AutoSize = true;
			this.Label15.Font = new System.Drawing.Font("HY견고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Label15.Location = new System.Drawing.Point(372, 132);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(24, 16);
			this.Label15.TabIndex = 172;
			this.Label15.Text = "%";
			// 
			// Label12
			// 
			this.Label12.AutoSize = true;
			this.Label12.Font = new System.Drawing.Font("HY견고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Label12.Location = new System.Drawing.Point(371, 64);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(25, 16);
			this.Label12.TabIndex = 169;
			this.Label12.Text = "℃";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnStop);
			this.groupBox2.Controls.Add(this.btnStart);
			this.groupBox2.Location = new System.Drawing.Point(658, 360);
			this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox2.Size = new System.Drawing.Size(145, 175);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Control";
			// 
			// btnStop
			// 
			this.btnStop.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStop.ForeColor = System.Drawing.Color.Red;
			this.btnStop.Location = new System.Drawing.Point(12, 111);
			this.btnStop.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(120, 50);
			this.btnStop.TabIndex = 167;
			this.btnStop.Text = "Stop";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// btnStart
			// 
			this.btnStart.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnStart.Location = new System.Drawing.Point(12, 20);
			this.btnStart.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnStart.Name = "btnStart";
			this.btnStart.Size = new System.Drawing.Size(120, 70);
			this.btnStart.TabIndex = 166;
			this.btnStart.Text = "START";
			this.btnStart.UseVisualStyleBackColor = true;
			this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.txtLog);
			this.groupBox3.Location = new System.Drawing.Point(410, 417);
			this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox3.Size = new System.Drawing.Size(242, 118);
			this.groupBox3.TabIndex = 168;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(3, 20);
			this.txtLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(236, 94);
			this.txtLog.TabIndex = 169;
			// 
			// UiChamberSchedule
			// 
			this.UiChamberSchedule.ActionChamberStatus = null;
			this.UiChamberSchedule.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.UiChamberSchedule.Location = new System.Drawing.Point(4, 2);
			this.UiChamberSchedule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.UiChamberSchedule.Name = "UiChamberSchedule";
			this.UiChamberSchedule.RunAction = null;
			this.UiChamberSchedule.RunChamber = null;
			this.UiChamberSchedule.Size = new System.Drawing.Size(800, 350);
			this.UiChamberSchedule.TabIndex = 169;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.pickerBookSchedule);
			this.groupBox4.Controls.Add(this.chkBookSchedule);
			this.groupBox4.Location = new System.Drawing.Point(410, 360);
			this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox4.Size = new System.Drawing.Size(242, 60);
			this.groupBox4.TabIndex = 170;
			this.groupBox4.TabStop = false;
			// 
			// chkBookSchedule
			// 
			this.chkBookSchedule.AutoSize = true;
			this.chkBookSchedule.Location = new System.Drawing.Point(9, 25);
			this.chkBookSchedule.Name = "chkBookSchedule";
			this.chkBookSchedule.Size = new System.Drawing.Size(50, 19);
			this.chkBookSchedule.TabIndex = 0;
			this.chkBookSchedule.Text = "예약";
			this.chkBookSchedule.UseVisualStyleBackColor = true;
			// 
			// pickerBookSchedule
			// 
			this.pickerBookSchedule.CustomFormat = "dd일 HH시 mm분";
			this.pickerBookSchedule.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.pickerBookSchedule.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.pickerBookSchedule.Location = new System.Drawing.Point(66, 21);
			this.pickerBookSchedule.Name = "pickerBookSchedule";
			this.pickerBookSchedule.Size = new System.Drawing.Size(166, 25);
			this.pickerBookSchedule.TabIndex = 2;
			// 
			// ChamberControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(813, 542);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.UiChamberSchedule);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "ChamberControl";
			this.Text = "Chamber";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChamberControl_FormClosed);
			this.Load += new System.EventHandler(this.ChamberControl_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.wfgChamber)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		internal NationalInstruments.UI.WindowsForms.WaveformGraph wfgChamber;
		internal NationalInstruments.UI.WaveformPlot WaveformPlot2;
		internal NationalInstruments.UI.XAxis XAxis2;
		internal NationalInstruments.UI.YAxis YAxis2;
		internal System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Label Label12;
		internal System.Windows.Forms.Label label1;
		internal System.Windows.Forms.Label Label39;
		internal System.Windows.Forms.Button btnStop;
		internal System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox txtLog;
		internal System.Windows.Forms.TextBox txtStatusHumi;
		internal System.Windows.Forms.TextBox txtStatusTemp;
		private Aligner.ChamberSchedule UiChamberSchedule;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.DateTimePicker pickerBookSchedule;
		private System.Windows.Forms.CheckBox chkBookSchedule;
	}
}