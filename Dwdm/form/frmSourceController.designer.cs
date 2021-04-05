
partial class frmSourceController
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
			this.Label20 = new System.Windows.Forms.Label();
			this.Label21 = new System.Windows.Forms.Label();
			this.Label19 = new System.Windows.Forms.Label();
			this.txtTlsPwr = new System.Windows.Forms.TextBox();
			this.Label18 = new System.Windows.Forms.Label();
			this.txtTlsWavelen = new System.Windows.Forms.TextBox();
			this.btnTlsLambda = new System.Windows.Forms.Button();
			this.grpTls = new System.Windows.Forms.GroupBox();
			this.lbTls = new System.Windows.Forms.Label();
			this.btnTlsPwr = new System.Windows.Forms.Button();
			this.tss = new System.Windows.Forms.StatusStrip();
			this.tsslbStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.groupOSW = new System.Windows.Forms.GroupBox();
			this.lbSource = new System.Windows.Forms.Label();
			this.rbtnTLS = new System.Windows.Forms.RadioButton();
			this.rbtn635bls = new System.Windows.Forms.RadioButton();
			this.grpTls.SuspendLayout();
			this.tss.SuspendLayout();
			this.groupOSW.SuspendLayout();
			this.SuspendLayout();
			// 
			// Label20
			// 
			this.Label20.AutoSize = true;
			this.Label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label20.Location = new System.Drawing.Point(13, 87);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(42, 15);
			this.Label20.TabIndex = 150;
			this.Label20.Text = "Power";
			// 
			// Label21
			// 
			this.Label21.AutoSize = true;
			this.Label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label21.Location = new System.Drawing.Point(6, 57);
			this.Label21.Name = "Label21";
			this.Label21.Size = new System.Drawing.Size(53, 15);
			this.Label21.TabIndex = 149;
			this.Label21.Text = "Lambda";
			// 
			// Label19
			// 
			this.Label19.AutoSize = true;
			this.Label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label19.Location = new System.Drawing.Point(221, 88);
			this.Label19.Name = "Label19";
			this.Label19.Size = new System.Drawing.Size(34, 13);
			this.Label19.TabIndex = 148;
			this.Label19.Text = "[dBm]";
			// 
			// txtTlsPwr
			// 
			this.txtTlsPwr.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtTlsPwr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsPwr.ForeColor = System.Drawing.Color.White;
			this.txtTlsPwr.Location = new System.Drawing.Point(61, 81);
			this.txtTlsPwr.Name = "txtTlsPwr";
			this.txtTlsPwr.Size = new System.Drawing.Size(85, 26);
			this.txtTlsPwr.TabIndex = 147;
			this.txtTlsPwr.Text = "-15";
			this.txtTlsPwr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// Label18
			// 
			this.Label18.AutoSize = true;
			this.Label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label18.Location = new System.Drawing.Point(221, 58);
			this.Label18.Name = "Label18";
			this.Label18.Size = new System.Drawing.Size(27, 13);
			this.Label18.TabIndex = 146;
			this.Label18.Text = "[nm]";
			// 
			// txtTlsWavelen
			// 
			this.txtTlsWavelen.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtTlsWavelen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsWavelen.ForeColor = System.Drawing.Color.White;
			this.txtTlsWavelen.Location = new System.Drawing.Point(61, 51);
			this.txtTlsWavelen.Name = "txtTlsWavelen";
			this.txtTlsWavelen.Size = new System.Drawing.Size(85, 26);
			this.txtTlsWavelen.TabIndex = 145;
			this.txtTlsWavelen.Text = "1550";
			this.txtTlsWavelen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// btnTlsLambda
			// 
			this.btnTlsLambda.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnTlsLambda.Location = new System.Drawing.Point(152, 51);
			this.btnTlsLambda.Name = "btnTlsLambda";
			this.btnTlsLambda.Size = new System.Drawing.Size(70, 26);
			this.btnTlsLambda.TabIndex = 144;
			this.btnTlsLambda.Text = "set";
			this.btnTlsLambda.UseVisualStyleBackColor = true;
			this.btnTlsLambda.Click += new System.EventHandler(this.btnTlsOK_Click);
			// 
			// grpTls
			// 
			this.grpTls.Controls.Add(this.lbTls);
			this.grpTls.Controls.Add(this.btnTlsPwr);
			this.grpTls.Controls.Add(this.btnTlsLambda);
			this.grpTls.Controls.Add(this.Label20);
			this.grpTls.Controls.Add(this.txtTlsWavelen);
			this.grpTls.Controls.Add(this.Label21);
			this.grpTls.Controls.Add(this.Label18);
			this.grpTls.Controls.Add(this.Label19);
			this.grpTls.Controls.Add(this.txtTlsPwr);
			this.grpTls.Location = new System.Drawing.Point(5, 79);
			this.grpTls.Name = "grpTls";
			this.grpTls.Size = new System.Drawing.Size(266, 118);
			this.grpTls.TabIndex = 151;
			this.grpTls.TabStop = false;
			this.grpTls.Text = "Tunable Laser Source";
			// 
			// lbTls
			// 
			this.lbTls.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbTls.Location = new System.Drawing.Point(20, 20);
			this.lbTls.Name = "lbTls";
			this.lbTls.Size = new System.Drawing.Size(233, 18);
			this.lbTls.TabIndex = 292;
			this.lbTls.Text = "...";
			this.lbTls.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// btnTlsPwr
			// 
			this.btnTlsPwr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnTlsPwr.Location = new System.Drawing.Point(152, 81);
			this.btnTlsPwr.Name = "btnTlsPwr";
			this.btnTlsPwr.Size = new System.Drawing.Size(70, 26);
			this.btnTlsPwr.TabIndex = 151;
			this.btnTlsPwr.Text = "set";
			this.btnTlsPwr.UseVisualStyleBackColor = true;
			this.btnTlsPwr.Click += new System.EventHandler(this.btnTlsPwr_Click);
			// 
			// tss
			// 
			this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbStatus});
			this.tss.Location = new System.Drawing.Point(0, 202);
			this.tss.Name = "tss";
			this.tss.Size = new System.Drawing.Size(276, 24);
			this.tss.TabIndex = 286;
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
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// groupOSW
			// 
			this.groupOSW.Controls.Add(this.lbSource);
			this.groupOSW.Controls.Add(this.rbtnTLS);
			this.groupOSW.Controls.Add(this.rbtn635bls);
			this.groupOSW.Location = new System.Drawing.Point(5, 0);
			this.groupOSW.Name = "groupOSW";
			this.groupOSW.Size = new System.Drawing.Size(266, 77);
			this.groupOSW.TabIndex = 287;
			this.groupOSW.TabStop = false;
			this.groupOSW.Text = "Optical Switch";
			// 
			// lbSource
			// 
			this.lbSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbSource.Location = new System.Drawing.Point(20, 20);
			this.lbSource.Name = "lbSource";
			this.lbSource.Size = new System.Drawing.Size(233, 18);
			this.lbSource.TabIndex = 291;
			this.lbSource.Text = "...";
			this.lbSource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// rbtnTLS
			// 
			this.rbtnTLS.AutoSize = true;
			this.rbtnTLS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtnTLS.Location = new System.Drawing.Point(165, 48);
			this.rbtnTLS.Name = "rbtnTLS";
			this.rbtnTLS.Size = new System.Drawing.Size(54, 20);
			this.rbtnTLS.TabIndex = 290;
			this.rbtnTLS.TabStop = true;
			this.rbtnTLS.Text = "TLS";
			this.rbtnTLS.UseVisualStyleBackColor = true;
			this.rbtnTLS.CheckedChanged += new System.EventHandler(this.rbtnTLS_CheckedChanged);
			// 
			// rbtn635bls
			// 
			this.rbtn635bls.AutoSize = true;
			this.rbtn635bls.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.749999F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn635bls.Location = new System.Drawing.Point(29, 48);
			this.rbtn635bls.Name = "rbtn635bls";
			this.rbtn635bls.Size = new System.Drawing.Size(94, 20);
			this.rbtn635bls.TabIndex = 289;
			this.rbtn635bls.TabStop = true;
			this.rbtn635bls.Text = "635 + BLS";
			this.rbtn635bls.UseVisualStyleBackColor = true;
			this.rbtn635bls.CheckedChanged += new System.EventHandler(this.rbtn635_CheckedChanged);
			// 
			// frmSourceController
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(276, 226);
			this.Controls.Add(this.groupOSW);
			this.Controls.Add(this.tss);
			this.Controls.Add(this.grpTls);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSourceController";
			this.Text = "Optical Source Controller";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
			this.Load += new System.EventHandler(this.frmSourceController_Load);
			this.grpTls.ResumeLayout(false);
			this.grpTls.PerformLayout();
			this.tss.ResumeLayout(false);
			this.tss.PerformLayout();
			this.groupOSW.ResumeLayout(false);
			this.groupOSW.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Label Label20;
    internal System.Windows.Forms.Label Label21;
    internal System.Windows.Forms.Label Label19;
    internal System.Windows.Forms.TextBox txtTlsPwr;
    internal System.Windows.Forms.Label Label18;
    internal System.Windows.Forms.TextBox txtTlsWavelen;
    internal System.Windows.Forms.Button btnTlsLambda;
    private System.Windows.Forms.GroupBox grpTls;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbStatus;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.GroupBox groupOSW;
    private System.Windows.Forms.RadioButton rbtnTLS;
    private System.Windows.Forms.RadioButton rbtn635bls;
    internal System.Windows.Forms.Label lbSource;
    internal System.Windows.Forms.Label lbTls;
    internal System.Windows.Forms.Button btnTlsPwr;
}
