
partial class InitForm
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
			this.uiStatusText = new System.Windows.Forms.RichTextBox();
			this.btnInit = new System.Windows.Forms.Button();
			this.btnPass = new System.Windows.Forms.Button();
			this.btnQuit = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.uiPcNpcO = new System.Windows.Forms.RadioButton();
			this.uiPcNpcC = new System.Windows.Forms.RadioButton();
			this.uiPcNone = new System.Windows.Forms.RadioButton();
			this.uiDoPCserver = new System.Windows.Forms.CheckBox();
			this.uiPcPsg100 = new System.Windows.Forms.RadioButton();
			this.uiPc8169 = new System.Windows.Forms.RadioButton();
			this.groupInitOption = new System.Windows.Forms.GroupBox();
			this.uiTestMode = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.uiTlsWaveShiftPm = new System.Windows.Forms.TextBox();
			this.uiDoTlsWaveShift = new System.Windows.Forms.CheckBox();
			this.uiTlsTcpServer = new System.Windows.Forms.RadioButton();
			this.uiTlsLocal = new System.Windows.Forms.RadioButton();
			this.uiDoNotAsk = new System.Windows.Forms.CheckBox();
			this.uiDoPassStage = new System.Windows.Forms.CheckBox();
			this.uiDoPassPm = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupInitOption.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// uiStatusText
			// 
			this.uiStatusText.BackColor = System.Drawing.SystemColors.MenuText;
			this.uiStatusText.Font = new System.Drawing.Font("D2Coding", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.uiStatusText.ForeColor = System.Drawing.Color.White;
			this.uiStatusText.Location = new System.Drawing.Point(12, 13);
			this.uiStatusText.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.uiStatusText.Name = "uiStatusText";
			this.uiStatusText.ReadOnly = true;
			this.uiStatusText.Size = new System.Drawing.Size(544, 304);
			this.uiStatusText.TabIndex = 84;
			this.uiStatusText.Text = "";
			// 
			// btnInit
			// 
			this.btnInit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnInit.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnInit.Location = new System.Drawing.Point(324, 565);
			this.btnInit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnInit.Name = "btnInit";
			this.btnInit.Size = new System.Drawing.Size(232, 61);
			this.btnInit.TabIndex = 83;
			this.btnInit.Text = "초기화 실행";
			this.btnInit.UseVisualStyleBackColor = true;
			this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
			// 
			// btnPass
			// 
			this.btnPass.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnPass.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnPass.Location = new System.Drawing.Point(87, 565);
			this.btnPass.Name = "btnPass";
			this.btnPass.Size = new System.Drawing.Size(105, 61);
			this.btnPass.TabIndex = 85;
			this.btnPass.Text = "나중에";
			this.btnPass.UseVisualStyleBackColor = true;
			this.btnPass.Click += new System.EventHandler(this.btnPass_Click);
			// 
			// btnQuit
			// 
			this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnQuit.Location = new System.Drawing.Point(12, 565);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(70, 61);
			this.btnQuit.TabIndex = 85;
			this.btnQuit.Text = "종료";
			this.btnQuit.UseVisualStyleBackColor = true;
			this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.uiPcNpcO);
			this.groupBox1.Controls.Add(this.uiPcNpcC);
			this.groupBox1.Controls.Add(this.uiPcNone);
			this.groupBox1.Controls.Add(this.uiDoPCserver);
			this.groupBox1.Controls.Add(this.uiPcPsg100);
			this.groupBox1.Controls.Add(this.uiPc8169);
			this.groupBox1.Location = new System.Drawing.Point(216, 324);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(340, 160);
			this.groupBox1.TabIndex = 86;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Polarization Controller";
			// 
			// uiPcNpcO
			// 
			this.uiPcNpcO.AutoSize = true;
			this.uiPcNpcO.Location = new System.Drawing.Point(167, 129);
			this.uiPcNpcO.Name = "uiPcNpcO";
			this.uiPcNpcO.Size = new System.Drawing.Size(130, 19);
			this.uiPcNpcO.TabIndex = 92;
			this.uiPcNpcO.Text = "NPC64-O (O-Band)";
			this.uiPcNpcO.UseVisualStyleBackColor = true;
			this.uiPcNpcO.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiPcNpcC
			// 
			this.uiPcNpcC.AutoSize = true;
			this.uiPcNpcC.Location = new System.Drawing.Point(167, 100);
			this.uiPcNpcC.Name = "uiPcNpcC";
			this.uiPcNpcC.Size = new System.Drawing.Size(128, 19);
			this.uiPcNpcC.TabIndex = 91;
			this.uiPcNpcC.Text = "NPC64-C (C-Band)";
			this.uiPcNpcC.UseVisualStyleBackColor = true;
			this.uiPcNpcC.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiPcNone
			// 
			this.uiPcNone.AutoSize = true;
			this.uiPcNone.Location = new System.Drawing.Point(27, 69);
			this.uiPcNone.Name = "uiPcNone";
			this.uiPcNone.Size = new System.Drawing.Size(142, 19);
			this.uiPcNone.TabIndex = 90;
			this.uiPcNone.Text = "None     (무편광측정)";
			this.uiPcNone.UseVisualStyleBackColor = true;
			this.uiPcNone.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiDoPCserver
			// 
			this.uiDoPCserver.AutoSize = true;
			this.uiDoPCserver.Location = new System.Drawing.Point(27, 29);
			this.uiDoPCserver.Name = "uiDoPCserver";
			this.uiDoPCserver.Size = new System.Drawing.Size(84, 19);
			this.uiDoPCserver.TabIndex = 89;
			this.uiDoPCserver.Text = "TCP Server";
			this.uiDoPCserver.UseVisualStyleBackColor = true;
			this.uiDoPCserver.CheckedChanged += new System.EventHandler(this.uiDoPCserver_CheckedChanged);
			// 
			// uiPcPsg100
			// 
			this.uiPcPsg100.AutoSize = true;
			this.uiPcPsg100.Location = new System.Drawing.Point(27, 129);
			this.uiPcPsg100.Name = "uiPcPsg100";
			this.uiPcPsg100.Size = new System.Drawing.Size(121, 19);
			this.uiPcPsg100.TabIndex = 0;
			this.uiPcPsg100.Text = "PSG100 (O-Band)";
			this.uiPcPsg100.UseVisualStyleBackColor = true;
			this.uiPcPsg100.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiPc8169
			// 
			this.uiPc8169.AutoSize = true;
			this.uiPc8169.Location = new System.Drawing.Point(27, 99);
			this.uiPc8169.Name = "uiPc8169";
			this.uiPc8169.Size = new System.Drawing.Size(121, 19);
			this.uiPc8169.TabIndex = 0;
			this.uiPc8169.Text = "8169A   (C-Band)";
			this.uiPc8169.UseVisualStyleBackColor = true;
			this.uiPc8169.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// groupInitOption
			// 
			this.groupInitOption.Controls.Add(this.uiTestMode);
			this.groupInitOption.Controls.Add(this.uiDoNotAsk);
			this.groupInitOption.Controls.Add(this.uiDoPassStage);
			this.groupInitOption.Controls.Add(this.uiDoPassPm);
			this.groupInitOption.Location = new System.Drawing.Point(12, 490);
			this.groupInitOption.Name = "groupInitOption";
			this.groupInitOption.Size = new System.Drawing.Size(544, 68);
			this.groupInitOption.TabIndex = 87;
			this.groupInitOption.TabStop = false;
			this.groupInitOption.Text = "Options";
			// 
			// uiTestMode
			// 
			this.uiTestMode.AutoSize = true;
			this.uiTestMode.Location = new System.Drawing.Point(439, 31);
			this.uiTestMode.Name = "uiTestMode";
			this.uiTestMode.Size = new System.Drawing.Size(80, 19);
			this.uiTestMode.TabIndex = 88;
			this.uiTestMode.Text = "test mode";
			this.uiTestMode.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.uiTlsWaveShiftPm);
			this.groupBox2.Controls.Add(this.uiDoTlsWaveShift);
			this.groupBox2.Controls.Add(this.uiTlsTcpServer);
			this.groupBox2.Controls.Add(this.uiTlsLocal);
			this.groupBox2.Location = new System.Drawing.Point(12, 324);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(198, 160);
			this.groupBox2.TabIndex = 87;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "TLS";
			// 
			// uiTlsWaveShiftPm
			// 
			this.uiTlsWaveShiftPm.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.uiTlsWaveShiftPm.ForeColor = System.Drawing.Color.DodgerBlue;
			this.uiTlsWaveShiftPm.Location = new System.Drawing.Point(47, 127);
			this.uiTlsWaveShiftPm.Name = "uiTlsWaveShiftPm";
			this.uiTlsWaveShiftPm.Size = new System.Drawing.Size(87, 23);
			this.uiTlsWaveShiftPm.TabIndex = 328;
			this.uiTlsWaveShiftPm.Text = "1";
			this.uiTlsWaveShiftPm.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// uiDoTlsWaveShift
			// 
			this.uiDoTlsWaveShift.AutoSize = true;
			this.uiDoTlsWaveShift.Checked = true;
			this.uiDoTlsWaveShift.CheckState = System.Windows.Forms.CheckState.Checked;
			this.uiDoTlsWaveShift.Location = new System.Drawing.Point(20, 101);
			this.uiDoTlsWaveShift.Name = "uiDoTlsWaveShift";
			this.uiDoTlsWaveShift.Size = new System.Drawing.Size(114, 19);
			this.uiDoTlsWaveShift.TabIndex = 1;
			this.uiDoTlsWaveShift.Text = "Wave Shift (pm)";
			this.uiDoTlsWaveShift.UseVisualStyleBackColor = true;
			this.uiDoTlsWaveShift.CheckedChanged += new System.EventHandler(this.uiDoTlsWaveShift_CheckedChanged);
			// 
			// uiTlsTcpServer
			// 
			this.uiTlsTcpServer.AutoSize = true;
			this.uiTlsTcpServer.Location = new System.Drawing.Point(20, 57);
			this.uiTlsTcpServer.Name = "uiTlsTcpServer";
			this.uiTlsTcpServer.Size = new System.Drawing.Size(97, 19);
			this.uiTlsTcpServer.TabIndex = 0;
			this.uiTlsTcpServer.Text = "2. TCP Server";
			this.uiTlsTcpServer.UseVisualStyleBackColor = true;
			this.uiTlsTcpServer.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiTlsLocal
			// 
			this.uiTlsLocal.AutoSize = true;
			this.uiTlsLocal.Location = new System.Drawing.Point(20, 28);
			this.uiTlsLocal.Name = "uiTlsLocal";
			this.uiTlsLocal.Size = new System.Drawing.Size(67, 19);
			this.uiTlsLocal.TabIndex = 0;
			this.uiTlsLocal.Text = "1. Local";
			this.uiTlsLocal.UseVisualStyleBackColor = true;
			this.uiTlsLocal.CheckedChanged += new System.EventHandler(this.uiPc_CheckedChanged);
			// 
			// uiDoNotAsk
			// 
			this.uiDoNotAsk.AutoSize = true;
			this.uiDoNotAsk.Checked = global::Neon.Dwdm.Properties.Settings.Default.doInitNotAsk;
			this.uiDoNotAsk.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doInitNotAsk", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.uiDoNotAsk.Location = new System.Drawing.Point(340, 31);
			this.uiDoNotAsk.Name = "uiDoNotAsk";
			this.uiDoNotAsk.Size = new System.Drawing.Size(50, 19);
			this.uiDoNotAsk.TabIndex = 87;
			this.uiDoNotAsk.Text = "不問";
			this.uiDoNotAsk.UseVisualStyleBackColor = true;
			// 
			// uiDoPassStage
			// 
			this.uiDoPassStage.AutoSize = true;
			this.uiDoPassStage.Checked = global::Neon.Dwdm.Properties.Settings.Default.doInitPassStage;
			this.uiDoPassStage.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doInitPassStage", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.uiDoPassStage.Location = new System.Drawing.Point(35, 31);
			this.uiDoPassStage.Name = "uiDoPassStage";
			this.uiDoPassStage.Size = new System.Drawing.Size(84, 19);
			this.uiDoPassStage.TabIndex = 87;
			this.uiDoPassStage.Text = "Stage 생략";
			this.uiDoPassStage.UseVisualStyleBackColor = true;
			// 
			// uiDoPassPm
			// 
			this.uiDoPassPm.AutoSize = true;
			this.uiDoPassPm.Checked = global::Neon.Dwdm.Properties.Settings.Default.doInitPassPm;
			this.uiDoPassPm.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Neon.Dwdm.Properties.Settings.Default, "doInitPassPm", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.uiDoPassPm.Location = new System.Drawing.Point(162, 31);
			this.uiDoPassPm.Name = "uiDoPassPm";
			this.uiDoPassPm.Size = new System.Drawing.Size(126, 19);
			this.uiDoPassPm.TabIndex = 87;
			this.uiDoPassPm.Text = "TLS, PM 셋업 생략";
			this.uiDoPassPm.UseVisualStyleBackColor = true;
			// 
			// InitForm
			// 
			this.AcceptButton = this.btnInit;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnPass;
			this.ClientSize = new System.Drawing.Size(568, 635);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupInitOption);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnQuit);
			this.Controls.Add(this.btnPass);
			this.Controls.Add(this.uiStatusText);
			this.Controls.Add(this.btnInit);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InitForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "초기화";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupInitOption.ResumeLayout(false);
			this.groupInitOption.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.RichTextBox uiStatusText;
    internal System.Windows.Forms.Button btnInit;
    private System.Windows.Forms.Button btnPass;
    private System.Windows.Forms.Button btnQuit;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.RadioButton uiPcPsg100;
    private System.Windows.Forms.RadioButton uiPc8169;
    private System.Windows.Forms.CheckBox uiDoPassStage;
    private System.Windows.Forms.CheckBox uiDoNotAsk;
    private System.Windows.Forms.CheckBox uiDoPassPm;
    private System.Windows.Forms.GroupBox groupInitOption;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.RadioButton uiTlsTcpServer;
    private System.Windows.Forms.RadioButton uiTlsLocal;
    private System.Windows.Forms.CheckBox uiTestMode;
    private System.Windows.Forms.CheckBox uiDoPCserver;
	private System.Windows.Forms.RadioButton uiPcNone;
	private System.Windows.Forms.CheckBox uiDoTlsWaveShift;
	internal System.Windows.Forms.TextBox uiTlsWaveShiftPm;
	private System.Windows.Forms.RadioButton uiPcNpcO;
	private System.Windows.Forms.RadioButton uiPcNpcC;
}
