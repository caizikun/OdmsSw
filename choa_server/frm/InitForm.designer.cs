
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitForm));
            this.rtxtStatus = new System.Windows.Forms.RichTextBox();
            this.btnInit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btn계속 = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.chkTlsPmMode = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtxtStatus
            // 
            this.rtxtStatus.BackColor = System.Drawing.SystemColors.MenuText;
            this.rtxtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtStatus.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxtStatus.ForeColor = System.Drawing.Color.White;
            this.rtxtStatus.Location = new System.Drawing.Point(0, 0);
            this.rtxtStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rtxtStatus.Name = "rtxtStatus";
            this.rtxtStatus.ReadOnly = true;
            this.rtxtStatus.Size = new System.Drawing.Size(644, 495);
            this.rtxtStatus.TabIndex = 84;
            this.rtxtStatus.TabStop = false;
            this.rtxtStatus.Text = "";
            // 
            // btnInit
            // 
            this.btnInit.Location = new System.Drawing.Point(148, 5);
            this.btnInit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnInit.Name = "btnInit";
            this.btnInit.Size = new System.Drawing.Size(140, 50);
            this.btnInit.TabIndex = 83;
            this.btnInit.Text = "Initialize";
            this.btnInit.UseVisualStyleBackColor = true;
            this.btnInit.Click += new System.EventHandler(this.btnInit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(548, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 50);
            this.btnCancel.TabIndex = 85;
            this.btnCancel.Text = "Quit";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btn계속
            // 
            this.btn계속.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn계속.Location = new System.Drawing.Point(445, 5);
            this.btn계속.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn계속.Name = "btn계속";
            this.btn계속.Size = new System.Drawing.Size(97, 50);
            this.btn계속.TabIndex = 87;
            this.btn계속.Text = "OK";
            this.btn계속.UseVisualStyleBackColor = true;
            this.btn계속.Click += new System.EventHandler(this.btn계속_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtxtStatus);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.chkTlsPmMode);
            this.splitContainer1.Panel2.Controls.Add(this.btnInit);
            this.splitContainer1.Panel2.Controls.Add(this.btn계속);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Size = new System.Drawing.Size(644, 561);
            this.splitContainer1.SplitterDistance = 495;
            this.splitContainer1.SplitterWidth = 3;
            this.splitContainer1.TabIndex = 88;
            // 
            // chkTlsPmMode
            // 
            this.chkTlsPmMode.AutoSize = true;
            this.chkTlsPmMode.Location = new System.Drawing.Point(21, 20);
            this.chkTlsPmMode.Margin = new System.Windows.Forms.Padding(2);
            this.chkTlsPmMode.Name = "chkTlsPmMode";
            this.chkTlsPmMode.Size = new System.Drawing.Size(115, 23);
            this.chkTlsPmMode.TabIndex = 90;
            this.chkTlsPmMode.Text = "TLS PM mode";
            this.chkTlsPmMode.UseVisualStyleBackColor = true;
            // 
            // InitForm
            // 
            this.AcceptButton = this.btnInit;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(644, 561);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "System Initalization";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InitForm_KeyDown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.RichTextBox rtxtStatus;
    internal System.Windows.Forms.Button btnInit;
	private System.Windows.Forms.Button btnCancel;
    internal System.Windows.Forms.Button btn계속;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.CheckBox chkTlsPmMode;
}
