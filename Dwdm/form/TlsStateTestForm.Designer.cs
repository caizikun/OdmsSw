
	partial class TlsStateTestForm
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
			this.btnRun = new System.Windows.Forms.Button();
			this.btnStop = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.txtDelay = new System.Windows.Forms.TextBox();
			this.label30 = new System.Windows.Forms.Label();
			this.txtLog = new System.Windows.Forms.RichTextBox();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnRun
			// 
			this.btnRun.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnRun.Location = new System.Drawing.Point(129, 8);
			this.btnRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnRun.Name = "btnRun";
			this.btnRun.Size = new System.Drawing.Size(200, 50);
			this.btnRun.TabIndex = 25;
			this.btnRun.Text = "Run";
			this.btnRun.UseVisualStyleBackColor = true;
			this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
			// 
			// btnStop
			// 
			this.btnStop.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnStop.ForeColor = System.Drawing.Color.OrangeRed;
			this.btnStop.Location = new System.Drawing.Point(343, 8);
			this.btnStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnStop.Name = "btnStop";
			this.btnStop.Size = new System.Drawing.Size(200, 50);
			this.btnStop.TabIndex = 281;
			this.btnStop.Text = "STOP";
			this.btnStop.UseVisualStyleBackColor = true;
			this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.txtDelay);
			this.splitContainer1.Panel1.Controls.Add(this.label30);
			this.splitContainer1.Panel1.Controls.Add(this.btnRun);
			this.splitContainer1.Panel1.Controls.Add(this.btnStop);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.txtLog);
			this.splitContainer1.Size = new System.Drawing.Size(736, 510);
			this.splitContainer1.SplitterDistance = 90;
			this.splitContainer1.TabIndex = 282;
			// 
			// txtDelay
			// 
			this.txtDelay.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDelay.ForeColor = System.Drawing.Color.DodgerBlue;
			this.txtDelay.Location = new System.Drawing.Point(20, 35);
			this.txtDelay.Name = "txtDelay";
			this.txtDelay.Size = new System.Drawing.Size(49, 23);
			this.txtDelay.TabIndex = 324;
			this.txtDelay.Text = "60";
			this.txtDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label30
			// 
			this.label30.AutoSize = true;
			this.label30.Location = new System.Drawing.Point(18, 18);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(105, 12);
			this.label30.TabIndex = 323;
			this.label30.Text = "Delay Time (sec)";
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(0, 0);
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(736, 416);
			this.txtLog.TabIndex = 3;
			this.txtLog.Text = "";
			// 
			// TlsStateTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(736, 510);
			this.Controls.Add(this.splitContainer1);
			this.Name = "TlsStateTestForm";
			this.Text = "TlsStateTestForm";
			this.Load += new System.EventHandler(this.TlsStateTestForm_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		internal System.Windows.Forms.Button btnRun;
		internal System.Windows.Forms.Button btnStop;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.RichTextBox txtLog;
		internal System.Windows.Forms.TextBox txtDelay;
		internal System.Windows.Forms.Label label30;
	}
