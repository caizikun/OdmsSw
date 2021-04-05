
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
            this.Label20 = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.txtTlsPwr = new System.Windows.Forms.TextBox();
            this.txtTlsWavelen = new System.Windows.Forms.TextBox();
            this.btnTlsOK = new System.Windows.Forms.Button();
            this.groupTLS = new System.Windows.Forms.GroupBox();
            this.rbtn1331 = new System.Windows.Forms.RadioButton();
            this.rbtn1311 = new System.Windows.Forms.RadioButton();
            this.rbtn1291 = new System.Windows.Forms.RadioButton();
            this.rbtn1271 = new System.Windows.Forms.RadioButton();
            this.tss = new System.Windows.Forms.StatusStrip();
            this.tsslbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupOSW = new System.Windows.Forms.GroupBox();
            this.rbtnTLS = new System.Windows.Forms.RadioButton();
            this.rbtnAlign = new System.Windows.Forms.RadioButton();
            this.groupTLS.SuspendLayout();
            this.tss.SuspendLayout();
            this.groupOSW.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Font = new System.Drawing.Font("Malgun Gothic", 9F);
            this.Label20.Location = new System.Drawing.Point(5, 24);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(51, 15);
            this.Label20.TabIndex = 150;
            this.Label20.Text = "P [dBm]";
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(7, 54);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(43, 15);
            this.Label21.TabIndex = 149;
            this.Label21.Text = "λ [nm]";
            // 
            // txtTlsPwr
            // 
            this.txtTlsPwr.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtTlsPwr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTlsPwr.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTlsPwr.ForeColor = System.Drawing.Color.Lime;
            this.txtTlsPwr.Location = new System.Drawing.Point(77, 23);
            this.txtTlsPwr.Name = "txtTlsPwr";
            this.txtTlsPwr.Size = new System.Drawing.Size(76, 20);
            this.txtTlsPwr.TabIndex = 147;
            this.txtTlsPwr.Text = "-15.0";
            this.txtTlsPwr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTlsWavelen
            // 
            this.txtTlsWavelen.BackColor = System.Drawing.SystemColors.MenuText;
            this.txtTlsWavelen.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTlsWavelen.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTlsWavelen.ForeColor = System.Drawing.Color.Lime;
            this.txtTlsWavelen.Location = new System.Drawing.Point(77, 54);
            this.txtTlsWavelen.Name = "txtTlsWavelen";
            this.txtTlsWavelen.Size = new System.Drawing.Size(76, 20);
            this.txtTlsWavelen.TabIndex = 145;
            this.txtTlsWavelen.Text = "1550.00";
            this.txtTlsWavelen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTlsWavelen.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtTlsWavelen_KeyDown);
            // 
            // btnTlsOK
            // 
            this.btnTlsOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTlsOK.Location = new System.Drawing.Point(77, 85);
            this.btnTlsOK.Name = "btnTlsOK";
            this.btnTlsOK.Size = new System.Drawing.Size(76, 31);
            this.btnTlsOK.TabIndex = 144;
            this.btnTlsOK.Text = "Apply";
            this.btnTlsOK.UseVisualStyleBackColor = true;
            this.btnTlsOK.Click += new System.EventHandler(this.btnTlsOK_Click);
            // 
            // grpTls
            // 
            this.groupTLS.Controls.Add(this.rbtn1331);
            this.groupTLS.Controls.Add(this.btnTlsOK);
            this.groupTLS.Controls.Add(this.rbtn1311);
            this.groupTLS.Controls.Add(this.rbtn1291);
            this.groupTLS.Controls.Add(this.txtTlsWavelen);
            this.groupTLS.Controls.Add(this.rbtn1271);
            this.groupTLS.Controls.Add(this.Label21);
            this.groupTLS.Controls.Add(this.txtTlsPwr);
            this.groupTLS.Controls.Add(this.Label20);
            this.groupTLS.Location = new System.Drawing.Point(5, 54);
            this.groupTLS.Name = "grpTls";
            this.groupTLS.Size = new System.Drawing.Size(230, 125);
            this.groupTLS.TabIndex = 151;
            this.groupTLS.TabStop = false;
            this.groupTLS.Text = "Tunable Laser Source";
            // 
            // rbtn1331
            // 
            this.rbtn1331.AutoSize = true;
            this.rbtn1331.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtn1331.Location = new System.Drawing.Point(163, 82);
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
            this.rbtn1311.Location = new System.Drawing.Point(163, 61);
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
            this.rbtn1291.Location = new System.Drawing.Point(163, 40);
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
            this.rbtn1271.Location = new System.Drawing.Point(163, 19);
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
            this.tss.Location = new System.Drawing.Point(0, 182);
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(242, 24);
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
            // groupOSW
            // 
            this.groupOSW.Controls.Add(this.rbtnTLS);
            this.groupOSW.Controls.Add(this.rbtnAlign);
            this.groupOSW.Location = new System.Drawing.Point(5, 3);
            this.groupOSW.Name = "groupOSW";
            this.groupOSW.Size = new System.Drawing.Size(230, 50);
            this.groupOSW.TabIndex = 287;
            this.groupOSW.TabStop = false;
            this.groupOSW.Text = "Optical Switch";
            // 
            // rbtnTLS
            // 
            this.rbtnTLS.AutoSize = true;
            this.rbtnTLS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnTLS.Location = new System.Drawing.Point(133, 22);
            this.rbtnTLS.Name = "rbtnTLS";
            this.rbtnTLS.Size = new System.Drawing.Size(55, 21);
            this.rbtnTLS.TabIndex = 290;
            this.rbtnTLS.TabStop = true;
            this.rbtnTLS.Text = "TLS";
            this.rbtnTLS.UseVisualStyleBackColor = true;
            this.rbtnTLS.CheckedChanged += new System.EventHandler(this.Osw_CheckedChanged);
            // 
            // rbtnAlign
            // 
            this.rbtnAlign.AutoSize = true;
            this.rbtnAlign.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnAlign.Location = new System.Drawing.Point(30, 22);
            this.rbtnAlign.Name = "rbtnAlign";
            this.rbtnAlign.Size = new System.Drawing.Size(62, 21);
            this.rbtnAlign.TabIndex = 289;
            this.rbtnAlign.TabStop = true;
            this.rbtnAlign.Text = "Align";
            this.rbtnAlign.UseVisualStyleBackColor = true;
            this.rbtnAlign.CheckedChanged += new System.EventHandler(this.Osw_CheckedChanged);
            // 
            // frmSourceController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 206);
            this.Controls.Add(this.groupOSW);
            this.Controls.Add(this.tss);
            this.Controls.Add(this.groupTLS);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSourceController";
            this.Text = "Optical Source Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmSourceController_FormClosing);
            this.Load += new System.EventHandler(this.frmSourceController_Load);
            this.groupTLS.ResumeLayout(false);
            this.groupTLS.PerformLayout();
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
    internal System.Windows.Forms.TextBox txtTlsPwr;
    internal System.Windows.Forms.TextBox txtTlsWavelen;
    internal System.Windows.Forms.Button btnTlsOK;
    private System.Windows.Forms.GroupBox groupTLS;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbStatus;
    private System.Windows.Forms.RadioButton rbtn1271;
    private System.Windows.Forms.RadioButton rbtn1291;
    private System.Windows.Forms.RadioButton rbtn1331;
    private System.Windows.Forms.RadioButton rbtn1311;
    private System.Windows.Forms.GroupBox groupOSW;
    private System.Windows.Forms.RadioButton rbtnTLS;
    private System.Windows.Forms.RadioButton rbtnAlign;
}
