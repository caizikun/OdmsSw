
partial class frmAirValveCont
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAirValveCont));
            this.btnOpenIn = new System.Windows.Forms.Button();
            this.btnCloseIn = new System.Windows.Forms.Button();
            this.btnCloseOut = new System.Windows.Forms.Button();
            this.btnOpenOut = new System.Windows.Forms.Button();
            this.tss = new System.Windows.Forms.StatusStrip();
            this.tsslbIn = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslbOut = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tss.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenIn
            // 
            this.btnOpenIn.Font = new System.Drawing.Font("휴먼둥근헤드라인", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOpenIn.Location = new System.Drawing.Point(8, 8);
            this.btnOpenIn.Name = "btnOpenIn";
            this.btnOpenIn.Size = new System.Drawing.Size(142, 39);
            this.btnOpenIn.TabIndex = 93;
            this.btnOpenIn.Text = "IN OPEN";
            this.btnOpenIn.UseVisualStyleBackColor = true;
            this.btnOpenIn.Click += new System.EventHandler(this.btnOpenIn_Click);
            // 
            // btnCloseIn
            // 
            this.btnCloseIn.Font = new System.Drawing.Font("휴먼둥근헤드라인", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCloseIn.Location = new System.Drawing.Point(8, 49);
            this.btnCloseIn.Name = "btnCloseIn";
            this.btnCloseIn.Size = new System.Drawing.Size(142, 39);
            this.btnCloseIn.TabIndex = 94;
            this.btnCloseIn.Text = "IN CLOSE";
            this.btnCloseIn.UseVisualStyleBackColor = true;
            this.btnCloseIn.Click += new System.EventHandler(this.btnCloseIn_Click);
            // 
            // btnCloseOut
            // 
            this.btnCloseOut.Font = new System.Drawing.Font("휴먼둥근헤드라인", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCloseOut.Location = new System.Drawing.Point(156, 49);
            this.btnCloseOut.Name = "btnCloseOut";
            this.btnCloseOut.Size = new System.Drawing.Size(142, 39);
            this.btnCloseOut.TabIndex = 96;
            this.btnCloseOut.Text = "OUT CLOSE";
            this.btnCloseOut.UseVisualStyleBackColor = true;
            this.btnCloseOut.Click += new System.EventHandler(this.btnCloseOut_Click);
            // 
            // btnOpenOut
            // 
            this.btnOpenOut.Font = new System.Drawing.Font("휴먼둥근헤드라인", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnOpenOut.Location = new System.Drawing.Point(156, 8);
            this.btnOpenOut.Name = "btnOpenOut";
            this.btnOpenOut.Size = new System.Drawing.Size(142, 39);
            this.btnOpenOut.TabIndex = 95;
            this.btnOpenOut.Text = "OUT OPEN";
            this.btnOpenOut.UseVisualStyleBackColor = true;
            this.btnOpenOut.Click += new System.EventHandler(this.btnOpenOut_Click);
            // 
            // tss
            // 
            this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbIn,
            this.tsslbOut});
            this.tss.Location = new System.Drawing.Point(0, 93);
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(302, 24);
            this.tss.TabIndex = 288;
            this.tss.Text = "StatusStrip1";
            // 
            // tsslbIn
            // 
            this.tsslbIn.AutoSize = false;
            this.tsslbIn.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbIn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tsslbIn.Name = "tsslbIn";
            this.tsslbIn.Size = new System.Drawing.Size(100, 19);
            this.tsslbIn.Text = "...";
            // 
            // tsslbOut
            // 
            this.tsslbOut.AutoSize = false;
            this.tsslbOut.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbOut.Name = "tsslbOut";
            this.tsslbOut.Size = new System.Drawing.Size(100, 19);
            this.tsslbOut.Text = "...";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmAirValveCont
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 117);
            this.Controls.Add(this.tss);
            this.Controls.Add(this.btnCloseOut);
            this.Controls.Add(this.btnOpenOut);
            this.Controls.Add(this.btnCloseIn);
            this.Controls.Add(this.btnOpenIn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAirValveCont";
            this.Text = "Air Valve Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAirValveCont_FormClosing);
            this.Load += new System.EventHandler(this.frmAirValveCont_Load);
            this.tss.ResumeLayout(false);
            this.tss.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.Button btnOpenIn;
    internal System.Windows.Forms.Button btnCloseIn;
    internal System.Windows.Forms.Button btnCloseOut;
    internal System.Windows.Forms.Button btnOpenOut;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbIn;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbOut;
    private System.Windows.Forms.Timer timer1;
}
