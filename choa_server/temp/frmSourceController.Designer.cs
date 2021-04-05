
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSourceController));
			this.Label20 = new System.Windows.Forms.Label();
			this.Label21 = new System.Windows.Forms.Label();
			this.txtTlsPwr = new System.Windows.Forms.TextBox();
			this.txtTlsWavelen = new System.Windows.Forms.TextBox();
			this.btnTlsOK = new System.Windows.Forms.Button();
			this.grpTls = new System.Windows.Forms.GroupBox();
			this.rbtn1331 = new System.Windows.Forms.RadioButton();
			this.rbtn1311 = new System.Windows.Forms.RadioButton();
			this.rbtn1291 = new System.Windows.Forms.RadioButton();
			this.rbtn1271 = new System.Windows.Forms.RadioButton();
			this.tss = new System.Windows.Forms.StatusStrip();
			this.tsslbStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.rbtnTLS = new System.Windows.Forms.RadioButton();
			this.rbtn635 = new System.Windows.Forms.RadioButton();
			this.grpTls.SuspendLayout();
			this.tss.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// Label20
			// 
			this.Label20.AutoSize = true;
			this.Label20.Font = new System.Drawing.Font("맑은 고딕", 9F);
			this.Label20.Location = new System.Drawing.Point(6, 22);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(68, 15);
			this.Label20.TabIndex = 150;
			this.Label20.Text = "파워 [dBm]";
			// 
			// Label21
			// 
			this.Label21.AutoSize = true;
			this.Label21.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label21.Location = new System.Drawing.Point(6, 48);
			this.Label21.Name = "Label21";
			this.Label21.Size = new System.Drawing.Size(61, 15);
			this.Label21.TabIndex = 149;
			this.Label21.Text = "파장 [nm]";
			// 
			// txtTlsPwr
			// 
			this.txtTlsPwr.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtTlsPwr.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtTlsPwr.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsPwr.ForeColor = System.Drawing.Color.Lime;
			this.txtTlsPwr.Location = new System.Drawing.Point(73, 21);
			this.txtTlsPwr.Name = "txtTlsPwr";
			this.txtTlsPwr.Size = new System.Drawing.Size(71, 17);
			this.txtTlsPwr.TabIndex = 147;
			this.txtTlsPwr.Text = "-15.0";
			this.txtTlsPwr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// txtTlsWavelen
			// 
			this.txtTlsWavelen.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtTlsWavelen.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtTlsWavelen.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTlsWavelen.ForeColor = System.Drawing.Color.Lime;
			this.txtTlsWavelen.Location = new System.Drawing.Point(73, 47);
			this.txtTlsWavelen.Name = "txtTlsWavelen";
			this.txtTlsWavelen.Size = new System.Drawing.Size(71, 17);
			this.txtTlsWavelen.TabIndex = 145;
			this.txtTlsWavelen.Text = "1550.00";
			this.txtTlsWavelen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtTlsWavelen.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTlsWavelen_KeyDown);
			// 
			// btnTlsOK
			// 
			this.btnTlsOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnTlsOK.Location = new System.Drawing.Point(72, 70);
			this.btnTlsOK.Name = "btnTlsOK";
			this.btnTlsOK.Size = new System.Drawing.Size(71, 31);
			this.btnTlsOK.TabIndex = 144;
			this.btnTlsOK.Text = "Apply";
			this.btnTlsOK.UseVisualStyleBackColor = true;
			this.btnTlsOK.Click += new System.EventHandler(this.btnTlsOK_Click);
			// 
			// grpTls
			// 
			this.grpTls.Controls.Add(this.rbtn1331);
			this.grpTls.Controls.Add(this.btnTlsOK);
			this.grpTls.Controls.Add(this.rbtn1311);
			this.grpTls.Controls.Add(this.rbtn1291);
			this.grpTls.Controls.Add(this.txtTlsWavelen);
			this.grpTls.Controls.Add(this.rbtn1271);
			this.grpTls.Controls.Add(this.Label21);
			this.grpTls.Controls.Add(this.txtTlsPwr);
			this.grpTls.Controls.Add(this.Label20);
			this.grpTls.Location = new System.Drawing.Point(4, 50);
			this.grpTls.Name = "grpTls";
			this.grpTls.Size = new System.Drawing.Size(215, 125);
			this.grpTls.TabIndex = 151;
			this.grpTls.TabStop = false;
			this.grpTls.Text = "Tunable Laser Source";
			// 
			// rbtn1331
			// 
			this.rbtn1331.AutoSize = true;
			this.rbtn1331.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn1331.Location = new System.Drawing.Point(150, 97);
			this.rbtn1331.Name = "rbtn1331";
			this.rbtn1331.Size = new System.Drawing.Size(62, 21);
			this.rbtn1331.TabIndex = 290;
			this.rbtn1331.TabStop = true;
			this.rbtn1331.Text = "1331";
			this.rbtn1331.UseVisualStyleBackColor = true;
			this.rbtn1331.CheckedChanged += new System.EventHandler(this.TlsWL_CheckedChanged);
			// 
			// rbtn1311
			// 
			this.rbtn1311.AutoSize = true;
			this.rbtn1311.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn1311.Location = new System.Drawing.Point(150, 71);
			this.rbtn1311.Name = "rbtn1311";
			this.rbtn1311.Size = new System.Drawing.Size(62, 21);
			this.rbtn1311.TabIndex = 289;
			this.rbtn1311.TabStop = true;
			this.rbtn1311.Text = "1311";
			this.rbtn1311.UseVisualStyleBackColor = true;
			this.rbtn1311.CheckedChanged += new System.EventHandler(this.TlsWL_CheckedChanged);
			// 
			// rbtn1291
			// 
			this.rbtn1291.AutoSize = true;
			this.rbtn1291.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn1291.Location = new System.Drawing.Point(150, 45);
			this.rbtn1291.Name = "rbtn1291";
			this.rbtn1291.Size = new System.Drawing.Size(62, 21);
			this.rbtn1291.TabIndex = 288;
			this.rbtn1291.TabStop = true;
			this.rbtn1291.Text = "1291";
			this.rbtn1291.UseVisualStyleBackColor = true;
			this.rbtn1291.CheckedChanged += new System.EventHandler(this.TlsWL_CheckedChanged);
			// 
			// rbtn1271
			// 
			this.rbtn1271.AutoSize = true;
			this.rbtn1271.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn1271.Location = new System.Drawing.Point(150, 19);
			this.rbtn1271.Name = "rbtn1271";
			this.rbtn1271.Size = new System.Drawing.Size(62, 21);
			this.rbtn1271.TabIndex = 287;
			this.rbtn1271.TabStop = true;
			this.rbtn1271.Text = "1271";
			this.rbtn1271.UseVisualStyleBackColor = true;
			this.rbtn1271.CheckedChanged += new System.EventHandler(this.TlsWL_CheckedChanged);
			// 
			// tss
			// 
			this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbStatus});
			this.tss.Location = new System.Drawing.Point(0, 178);
			this.tss.Name = "tss";
			this.tss.Size = new System.Drawing.Size(224, 24);
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
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.rbtnTLS);
			this.groupBox2.Controls.Add(this.rbtn635);
			this.groupBox2.Enabled = false;
			this.groupBox2.Location = new System.Drawing.Point(4, -1);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(215, 45);
			this.groupBox2.TabIndex = 287;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Optical Switch";
			// 
			// rbtnTLS
			// 
			this.rbtnTLS.AutoSize = true;
			this.rbtnTLS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtnTLS.Location = new System.Drawing.Point(139, 18);
			this.rbtnTLS.Name = "rbtnTLS";
			this.rbtnTLS.Size = new System.Drawing.Size(55, 21);
			this.rbtnTLS.TabIndex = 290;
			this.rbtnTLS.TabStop = true;
			this.rbtnTLS.Text = "TLS";
			this.rbtnTLS.UseVisualStyleBackColor = true;
			this.rbtnTLS.CheckedChanged += new System.EventHandler(this.Osw_CheckedChanged);
			// 
			// rbtn635
			// 
			this.rbtn635.AutoSize = true;
			this.rbtn635.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rbtn635.Location = new System.Drawing.Point(38, 18);
			this.rbtn635.Name = "rbtn635";
			this.rbtn635.Size = new System.Drawing.Size(53, 21);
			this.rbtn635.TabIndex = 289;
			this.rbtn635.TabStop = true;
			this.rbtn635.Text = "635";
			this.rbtn635.UseVisualStyleBackColor = true;
			this.rbtn635.CheckedChanged += new System.EventHandler(this.Osw_CheckedChanged);
			// 
			// frmSourceController
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(224, 202);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.tss);
			this.Controls.Add(this.grpTls);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSourceController";
			this.Text = "Optical Source Controller";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSourceController_FormClosing);
			this.Load += new System.EventHandler(this.frmSourceController_Load);
			this.grpTls.ResumeLayout(false);
			this.grpTls.PerformLayout();
			this.tss.ResumeLayout(false);
			this.tss.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Label Label20;
    internal System.Windows.Forms.Label Label21;
    internal System.Windows.Forms.TextBox txtTlsPwr;
    internal System.Windows.Forms.TextBox txtTlsWavelen;
    internal System.Windows.Forms.Button btnTlsOK;
    private System.Windows.Forms.GroupBox grpTls;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbStatus;
    private System.Windows.Forms.RadioButton rbtn1271;
    private System.Windows.Forms.RadioButton rbtn1291;
    private System.Windows.Forms.RadioButton rbtn1331;
    private System.Windows.Forms.RadioButton rbtn1311;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.RadioButton rbtnTLS;
    private System.Windows.Forms.RadioButton rbtn635;
}
