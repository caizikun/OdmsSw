namespace ScanTest
{
	partial class ScanMonitorPort
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
			this.groupBoxMonitorAuto = new System.Windows.Forms.GroupBox();
			this.chkScanNextChip = new System.Windows.Forms.CheckBox();
			this.btnMonitorScanStart = new System.Windows.Forms.Button();
			this.groupBoxMonitorMove = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.uiDistanceNextChip = new System.Windows.Forms.TextBox();
			this.uiMoveNextChip = new System.Windows.Forms.Button();
			this.uiSaveCh1ComCoord = new System.Windows.Forms.Button();
			this.uiMoveCom = new System.Windows.Forms.Button();
			this.uiMoveM4 = new System.Windows.Forms.Button();
			this.uiMoveM3 = new System.Windows.Forms.Button();
			this.uiMoveM2 = new System.Windows.Forms.Button();
			this.uiMoveM1 = new System.Windows.Forms.Button();
			this.uiMoveCH4 = new System.Windows.Forms.Button();
			this.uiMoveCH3 = new System.Windows.Forms.Button();
			this.uiMoveCH2 = new System.Windows.Forms.Button();
			this.uiMoveCH1 = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.uiAlignRight = new System.Windows.Forms.Button();
			this.uiAlignLeft = new System.Windows.Forms.Button();
			this.txtSaveName2 = new System.Windows.Forms.TextBox();
			this.txtSaveName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxMonitorAuto.SuspendLayout();
			this.groupBoxMonitorMove.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxMonitorAuto
			// 
			this.groupBoxMonitorAuto.Controls.Add(this.btnMonitorScanStart);
			this.groupBoxMonitorAuto.Controls.Add(this.txtSaveName);
			this.groupBoxMonitorAuto.Controls.Add(this.label3);
			this.groupBoxMonitorAuto.Location = new System.Drawing.Point(12, 12);
			this.groupBoxMonitorAuto.Name = "groupBoxMonitorAuto";
			this.groupBoxMonitorAuto.Size = new System.Drawing.Size(466, 85);
			this.groupBoxMonitorAuto.TabIndex = 285;
			this.groupBoxMonitorAuto.TabStop = false;
			this.groupBoxMonitorAuto.Text = "Monitor Ports - Auto";
			// 
			// chkScanNextChip
			// 
			this.chkScanNextChip.AutoSize = true;
			this.chkScanNextChip.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.chkScanNextChip.Location = new System.Drawing.Point(354, 20);
			this.chkScanNextChip.Name = "chkScanNextChip";
			this.chkScanNextChip.Size = new System.Drawing.Size(106, 16);
			this.chkScanNextChip.TabIndex = 284;
			this.chkScanNextChip.Text = "next Chip 측정";
			this.chkScanNextChip.UseVisualStyleBackColor = true;
			// 
			// btnMonitorScanStart
			// 
			this.btnMonitorScanStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnMonitorScanStart.Location = new System.Drawing.Point(335, 12);
			this.btnMonitorScanStart.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnMonitorScanStart.Name = "btnMonitorScanStart";
			this.btnMonitorScanStart.Size = new System.Drawing.Size(125, 65);
			this.btnMonitorScanStart.TabIndex = 284;
			this.btnMonitorScanStart.Text = "Start";
			this.btnMonitorScanStart.UseVisualStyleBackColor = true;
			// 
			// groupBoxMonitorMove
			// 
			this.groupBoxMonitorMove.Controls.Add(this.uiSaveCh1ComCoord);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveCom);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveM4);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveM3);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveM2);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveM1);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveCH4);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveCH3);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveCH2);
			this.groupBoxMonitorMove.Controls.Add(this.uiMoveCH1);
			this.groupBoxMonitorMove.Controls.Add(this.label8);
			this.groupBoxMonitorMove.Controls.Add(this.label4);
			this.groupBoxMonitorMove.Controls.Add(this.uiAlignRight);
			this.groupBoxMonitorMove.Controls.Add(this.uiAlignLeft);
			this.groupBoxMonitorMove.Location = new System.Drawing.Point(12, 216);
			this.groupBoxMonitorMove.Name = "groupBoxMonitorMove";
			this.groupBoxMonitorMove.Size = new System.Drawing.Size(466, 328);
			this.groupBoxMonitorMove.TabIndex = 286;
			this.groupBoxMonitorMove.TabStop = false;
			this.groupBoxMonitorMove.Text = "Monitor Ports - Stage Move";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label11.ForeColor = System.Drawing.Color.Black;
			this.label11.Location = new System.Drawing.Point(316, 45);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(65, 16);
			this.label11.TabIndex = 300;
			this.label11.Text = "Next Chip";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// uiDistanceNextChip
			// 
			this.uiDistanceNextChip.BackColor = System.Drawing.Color.Black;
			this.uiDistanceNextChip.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiDistanceNextChip.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.uiDistanceNextChip.Location = new System.Drawing.Point(387, 42);
			this.uiDistanceNextChip.Name = "uiDistanceNextChip";
			this.uiDistanceNextChip.Size = new System.Drawing.Size(73, 23);
			this.uiDistanceNextChip.TabIndex = 299;
			this.uiDistanceNextChip.Text = "3500";
			this.uiDistanceNextChip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiMoveNextChip
			// 
			this.uiMoveNextChip.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveNextChip.Location = new System.Drawing.Point(335, 70);
			this.uiMoveNextChip.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveNextChip.Name = "uiMoveNextChip";
			this.uiMoveNextChip.Size = new System.Drawing.Size(125, 37);
			this.uiMoveNextChip.TabIndex = 298;
			this.uiMoveNextChip.Text = "▶▶  Next Chip";
			this.uiMoveNextChip.UseVisualStyleBackColor = true;
			this.uiMoveNextChip.Click += new System.EventHandler(this.uiMoveNextChip_Click);
			// 
			// uiSaveCh1ComCoord
			// 
			this.uiSaveCh1ComCoord.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiSaveCh1ComCoord.Location = new System.Drawing.Point(301, 44);
			this.uiSaveCh1ComCoord.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiSaveCh1ComCoord.Name = "uiSaveCh1ComCoord";
			this.uiSaveCh1ComCoord.Size = new System.Drawing.Size(159, 53);
			this.uiSaveCh1ComCoord.TabIndex = 297;
			this.uiSaveCh1ComCoord.Text = "현재위치를 CH1↔COM   으로 기억";
			this.uiSaveCh1ComCoord.UseVisualStyleBackColor = true;
			this.uiSaveCh1ComCoord.Click += new System.EventHandler(this.uiSaveCh1ComCoord_Click);
			// 
			// uiMoveCom
			// 
			this.uiMoveCom.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveCom.Location = new System.Drawing.Point(144, 109);
			this.uiMoveCom.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveCom.Name = "uiMoveCom";
			this.uiMoveCom.Size = new System.Drawing.Size(89, 37);
			this.uiMoveCom.TabIndex = 296;
			this.uiMoveCom.Tag = "COM";
			this.uiMoveCom.Text = "▶▶  COM";
			this.uiMoveCom.UseVisualStyleBackColor = true;
			// 
			// uiMoveM4
			// 
			this.uiMoveM4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveM4.Location = new System.Drawing.Point(144, 277);
			this.uiMoveM4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveM4.Name = "uiMoveM4";
			this.uiMoveM4.Size = new System.Drawing.Size(89, 37);
			this.uiMoveM4.TabIndex = 295;
			this.uiMoveM4.Tag = "M4";
			this.uiMoveM4.Text = "▶▶  M4";
			this.uiMoveM4.UseVisualStyleBackColor = true;
			// 
			// uiMoveM3
			// 
			this.uiMoveM3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveM3.Location = new System.Drawing.Point(144, 235);
			this.uiMoveM3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveM3.Name = "uiMoveM3";
			this.uiMoveM3.Size = new System.Drawing.Size(89, 37);
			this.uiMoveM3.TabIndex = 294;
			this.uiMoveM3.Tag = "M3";
			this.uiMoveM3.Text = "▶▶  M3";
			this.uiMoveM3.UseVisualStyleBackColor = true;
			// 
			// uiMoveM2
			// 
			this.uiMoveM2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveM2.Location = new System.Drawing.Point(144, 193);
			this.uiMoveM2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveM2.Name = "uiMoveM2";
			this.uiMoveM2.Size = new System.Drawing.Size(89, 37);
			this.uiMoveM2.TabIndex = 293;
			this.uiMoveM2.Tag = "M2";
			this.uiMoveM2.Text = "▶▶  M2";
			this.uiMoveM2.UseVisualStyleBackColor = true;
			// 
			// uiMoveM1
			// 
			this.uiMoveM1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveM1.Location = new System.Drawing.Point(144, 151);
			this.uiMoveM1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveM1.Name = "uiMoveM1";
			this.uiMoveM1.Size = new System.Drawing.Size(89, 37);
			this.uiMoveM1.TabIndex = 292;
			this.uiMoveM1.Tag = "M1";
			this.uiMoveM1.Text = "▶▶  M1";
			this.uiMoveM1.UseVisualStyleBackColor = true;
			// 
			// uiMoveCH4
			// 
			this.uiMoveCH4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveCH4.Location = new System.Drawing.Point(17, 277);
			this.uiMoveCH4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveCH4.Name = "uiMoveCH4";
			this.uiMoveCH4.Size = new System.Drawing.Size(89, 37);
			this.uiMoveCH4.TabIndex = 291;
			this.uiMoveCH4.Tag = "CH4";
			this.uiMoveCH4.Text = "▶▶  CH4";
			this.uiMoveCH4.UseVisualStyleBackColor = true;
			// 
			// uiMoveCH3
			// 
			this.uiMoveCH3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveCH3.Location = new System.Drawing.Point(17, 235);
			this.uiMoveCH3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveCH3.Name = "uiMoveCH3";
			this.uiMoveCH3.Size = new System.Drawing.Size(89, 37);
			this.uiMoveCH3.TabIndex = 290;
			this.uiMoveCH3.Tag = "CH3";
			this.uiMoveCH3.Text = "▶▶  CH3";
			this.uiMoveCH3.UseVisualStyleBackColor = true;
			// 
			// uiMoveCH2
			// 
			this.uiMoveCH2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveCH2.Location = new System.Drawing.Point(17, 193);
			this.uiMoveCH2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveCH2.Name = "uiMoveCH2";
			this.uiMoveCH2.Size = new System.Drawing.Size(89, 37);
			this.uiMoveCH2.TabIndex = 289;
			this.uiMoveCH2.Tag = "CH2";
			this.uiMoveCH2.Text = "▶▶  CH2";
			this.uiMoveCH2.UseVisualStyleBackColor = true;
			// 
			// uiMoveCH1
			// 
			this.uiMoveCH1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiMoveCH1.Location = new System.Drawing.Point(17, 151);
			this.uiMoveCH1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiMoveCH1.Name = "uiMoveCH1";
			this.uiMoveCH1.Size = new System.Drawing.Size(89, 37);
			this.uiMoveCH1.TabIndex = 288;
			this.uiMoveCH1.Tag = "CH1";
			this.uiMoveCH1.Text = "▶▶  CH1";
			this.uiMoveCH1.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label8.ForeColor = System.Drawing.Color.Black;
			this.label8.Location = new System.Drawing.Point(169, 21);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(39, 16);
			this.label8.TabIndex = 287;
			this.label8.Text = "Right";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label4.ForeColor = System.Drawing.Color.Black;
			this.label4.Location = new System.Drawing.Point(47, 21);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(29, 16);
			this.label4.TabIndex = 286;
			this.label4.Text = "Left";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// uiAlignRight
			// 
			this.uiAlignRight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiAlignRight.Location = new System.Drawing.Point(144, 44);
			this.uiAlignRight.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiAlignRight.Name = "uiAlignRight";
			this.uiAlignRight.Size = new System.Drawing.Size(89, 53);
			this.uiAlignRight.TabIndex = 285;
			this.uiAlignRight.Text = "XY Align ↕";
			this.uiAlignRight.UseVisualStyleBackColor = true;
			this.uiAlignRight.Click += new System.EventHandler(this.uiAlign_Click);
			// 
			// uiAlignLeft
			// 
			this.uiAlignLeft.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiAlignLeft.Location = new System.Drawing.Point(17, 44);
			this.uiAlignLeft.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.uiAlignLeft.Name = "uiAlignLeft";
			this.uiAlignLeft.Size = new System.Drawing.Size(89, 53);
			this.uiAlignLeft.TabIndex = 284;
			this.uiAlignLeft.Text = "↕ XY  Align";
			this.uiAlignLeft.UseVisualStyleBackColor = true;
			this.uiAlignLeft.Click += new System.EventHandler(this.uiAlign_Click);
			// 
			// txtSaveName2
			// 
			this.txtSaveName2.BackColor = System.Drawing.Color.Black;
			this.txtSaveName2.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSaveName2.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtSaveName2.Location = new System.Drawing.Point(19, 50);
			this.txtSaveName2.Name = "txtSaveName2";
			this.txtSaveName2.Size = new System.Drawing.Size(279, 25);
			this.txtSaveName2.TabIndex = 289;
			this.txtSaveName2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtSaveName
			// 
			this.txtSaveName.BackColor = System.Drawing.Color.Black;
			this.txtSaveName.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtSaveName.ForeColor = System.Drawing.Color.DeepSkyBlue;
			this.txtSaveName.Location = new System.Drawing.Point(19, 41);
			this.txtSaveName.Name = "txtSaveName";
			this.txtSaveName.Size = new System.Drawing.Size(279, 25);
			this.txtSaveName.TabIndex = 287;
			this.txtSaveName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Location = new System.Drawing.Point(19, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 16);
			this.label3.TabIndex = 288;
			this.label3.Text = "File Name";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.chkScanNextChip);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.txtSaveName2);
			this.groupBox1.Controls.Add(this.uiDistanceNextChip);
			this.groupBox1.Controls.Add(this.uiMoveNextChip);
			this.groupBox1.Location = new System.Drawing.Point(12, 99);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(466, 111);
			this.groupBox1.TabIndex = 289;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Next Chip";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Location = new System.Drawing.Point(19, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 16);
			this.label1.TabIndex = 289;
			this.label1.Text = "File Name [next Chip]";
			// 
			// ScanMonitorPort
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(492, 561);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBoxMonitorMove);
			this.Controls.Add(this.groupBoxMonitorAuto);
			this.Name = "ScanMonitorPort";
			this.Text = "ScanMonitorPort";
			this.groupBoxMonitorAuto.ResumeLayout(false);
			this.groupBoxMonitorAuto.PerformLayout();
			this.groupBoxMonitorMove.ResumeLayout(false);
			this.groupBoxMonitorMove.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxMonitorAuto;
		private System.Windows.Forms.CheckBox chkScanNextChip;
		internal System.Windows.Forms.Button btnMonitorScanStart;
		private System.Windows.Forms.GroupBox groupBoxMonitorMove;
		internal System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox uiDistanceNextChip;
		internal System.Windows.Forms.Button uiMoveNextChip;
		internal System.Windows.Forms.Button uiSaveCh1ComCoord;
		internal System.Windows.Forms.Button uiMoveCom;
		internal System.Windows.Forms.Button uiMoveM4;
		internal System.Windows.Forms.Button uiMoveM3;
		internal System.Windows.Forms.Button uiMoveM2;
		internal System.Windows.Forms.Button uiMoveM1;
		internal System.Windows.Forms.Button uiMoveCH4;
		internal System.Windows.Forms.Button uiMoveCH3;
		internal System.Windows.Forms.Button uiMoveCH2;
		internal System.Windows.Forms.Button uiMoveCH1;
		internal System.Windows.Forms.Label label8;
		internal System.Windows.Forms.Label label4;
		internal System.Windows.Forms.Button uiAlignRight;
		internal System.Windows.Forms.Button uiAlignLeft;
		private System.Windows.Forms.TextBox txtSaveName2;
		private System.Windows.Forms.TextBox txtSaveName;
		internal System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		internal System.Windows.Forms.Label label1;
	}
}