namespace Neon.Aligner.choa_server
{
    partial class TestForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.uiCh = new System.Windows.Forms.CheckBox();
            this.uiNumLoop = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.uiStart = new System.Windows.Forms.Button();
            this.uiOpen = new System.Windows.Forms.Button();
            this.uiLog = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiNumLoop)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.uiCh);
            this.splitContainer1.Panel1.Controls.Add(this.uiNumLoop);
            this.splitContainer1.Panel1.Controls.Add(this.button1);
            this.splitContainer1.Panel1.Controls.Add(this.uiStart);
            this.splitContainer1.Panel1.Controls.Add(this.uiOpen);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.uiLog);
            this.splitContainer1.Size = new System.Drawing.Size(738, 433);
            this.splitContainer1.SplitterDistance = 140;
            this.splitContainer1.TabIndex = 0;
            // 
            // uiCh
            // 
            this.uiCh.AutoSize = true;
            this.uiCh.Location = new System.Drawing.Point(12, 63);
            this.uiCh.Name = "uiCh";
            this.uiCh.Size = new System.Drawing.Size(51, 21);
            this.uiCh.TabIndex = 1;
            this.uiCh.Text = "CH2";
            this.uiCh.UseVisualStyleBackColor = true;
            this.uiCh.CheckedChanged += new System.EventHandler(this.uiCh_CheckedChanged);
            // 
            // uiNumLoop
            // 
            this.uiNumLoop.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiNumLoop.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiNumLoop.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.uiNumLoop.Location = new System.Drawing.Point(12, 90);
            this.uiNumLoop.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.uiNumLoop.Name = "uiNumLoop";
            this.uiNumLoop.Size = new System.Drawing.Size(107, 26);
            this.uiNumLoop.TabIndex = 1;
            this.uiNumLoop.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiNumLoop.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 163);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(107, 35);
            this.button1.TabIndex = 3;
            this.button1.Text = "Stop";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // uiStart
            // 
            this.uiStart.Location = new System.Drawing.Point(12, 122);
            this.uiStart.Name = "uiStart";
            this.uiStart.Size = new System.Drawing.Size(107, 35);
            this.uiStart.TabIndex = 2;
            this.uiStart.Text = "Start";
            this.uiStart.UseVisualStyleBackColor = true;
            this.uiStart.Click += new System.EventHandler(this.uiStart_Click);
            // 
            // uiOpen
            // 
            this.uiOpen.Location = new System.Drawing.Point(12, 12);
            this.uiOpen.Name = "uiOpen";
            this.uiOpen.Size = new System.Drawing.Size(107, 35);
            this.uiOpen.TabIndex = 1;
            this.uiOpen.Text = "Open";
            this.uiOpen.UseVisualStyleBackColor = true;
            this.uiOpen.Click += new System.EventHandler(this.uiOpen_Click);
            // 
            // uiLog
            // 
            this.uiLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLog.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLog.Location = new System.Drawing.Point(0, 0);
            this.uiLog.Name = "uiLog";
            this.uiLog.Size = new System.Drawing.Size(594, 433);
            this.uiLog.TabIndex = 0;
            this.uiLog.Text = "";
            // 
            // TestForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(738, 433);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiNumLoop)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox uiLog;
        private System.Windows.Forms.Button uiOpen;
        private System.Windows.Forms.NumericUpDown uiNumLoop;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button uiStart;
        private System.Windows.Forms.CheckBox uiCh;
    }
}