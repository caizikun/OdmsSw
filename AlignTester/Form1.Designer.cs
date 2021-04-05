namespace AlignTester
{
    partial class Form1
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
            this.uiRun = new System.Windows.Forms.Button();
            this.uiLog = new System.Windows.Forms.TextBox();
            this.uiSplit1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.uiInit = new System.Windows.Forms.Button();
            this.uiSplit2 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplit1)).BeginInit();
            this.uiSplit1.Panel1.SuspendLayout();
            this.uiSplit1.Panel2.SuspendLayout();
            this.uiSplit1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplit2)).BeginInit();
            this.uiSplit2.Panel2.SuspendLayout();
            this.uiSplit2.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiRun
            // 
            this.uiRun.Location = new System.Drawing.Point(13, 61);
            this.uiRun.Name = "uiRun";
            this.uiRun.Size = new System.Drawing.Size(111, 42);
            this.uiRun.TabIndex = 0;
            this.uiRun.Text = "Run";
            this.uiRun.UseVisualStyleBackColor = true;
            this.uiRun.Click += new System.EventHandler(this.uiRun_Click);
            // 
            // uiLog
            // 
            this.uiLog.Location = new System.Drawing.Point(23, 20);
            this.uiLog.Multiline = true;
            this.uiLog.Name = "uiLog";
            this.uiLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.uiLog.Size = new System.Drawing.Size(119, 65);
            this.uiLog.TabIndex = 1;
            // 
            // uiSplit1
            // 
            this.uiSplit1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.uiSplit1.Location = new System.Drawing.Point(35, 41);
            this.uiSplit1.Name = "uiSplit1";
            // 
            // uiSplit1.Panel1
            // 
            this.uiSplit1.Panel1.Controls.Add(this.flowLayoutPanel1);
            // 
            // uiSplit1.Panel2
            // 
            this.uiSplit1.Panel2.Controls.Add(this.uiSplit2);
            this.uiSplit1.Size = new System.Drawing.Size(450, 301);
            this.uiSplit1.SplitterDistance = 150;
            this.uiSplit1.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.uiInit);
            this.flowLayoutPanel1.Controls.Add(this.uiRun);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(150, 301);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // uiInit
            // 
            this.uiInit.Location = new System.Drawing.Point(13, 13);
            this.uiInit.Name = "uiInit";
            this.uiInit.Size = new System.Drawing.Size(111, 42);
            this.uiInit.TabIndex = 1;
            this.uiInit.Text = "Init";
            this.uiInit.UseVisualStyleBackColor = true;
            this.uiInit.Click += new System.EventHandler(this.uiInit_Click);
            // 
            // uiSplit2
            // 
            this.uiSplit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiSplit2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.uiSplit2.Location = new System.Drawing.Point(0, 0);
            this.uiSplit2.Name = "uiSplit2";
            this.uiSplit2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // uiSplit2.Panel2
            // 
            this.uiSplit2.Panel2.Controls.Add(this.uiLog);
            this.uiSplit2.Size = new System.Drawing.Size(296, 301);
            this.uiSplit2.SplitterDistance = 98;
            this.uiSplit2.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 645);
            this.Controls.Add(this.uiSplit1);
            this.Font = new System.Drawing.Font("Noto Sans KR Regular", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.uiSplit1.Panel1.ResumeLayout(false);
            this.uiSplit1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiSplit1)).EndInit();
            this.uiSplit1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.uiSplit2.Panel2.ResumeLayout(false);
            this.uiSplit2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiSplit2)).EndInit();
            this.uiSplit2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button uiRun;
        private System.Windows.Forms.TextBox uiLog;
        private System.Windows.Forms.SplitContainer uiSplit1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button uiInit;
        private System.Windows.Forms.SplitContainer uiSplit2;
    }
}

