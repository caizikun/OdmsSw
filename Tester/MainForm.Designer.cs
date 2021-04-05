namespace Tester
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.uiCmd = new System.Windows.Forms.TextBox();
            this.uiRunSerial = new System.Windows.Forms.Button();
            this.uiNewLine = new System.Windows.Forms.ComboBox();
            this.uiBps = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.uiRun = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.uiNeon = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.uiTls = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.uiAlign = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.uiCom = new System.Windows.Forms.NumericUpDown();
            this.uiLog = new System.Windows.Forms.RichTextBox();
            this.uiRemoveNL = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.uiBps)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiTls)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiAlign)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiCom)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 605);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(983, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(983, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.uiRemoveNL);
            this.splitContainer1.Panel1.Controls.Add(this.uiCmd);
            this.splitContainer1.Panel1.Controls.Add(this.uiRunSerial);
            this.splitContainer1.Panel1.Controls.Add(this.uiNewLine);
            this.splitContainer1.Panel1.Controls.Add(this.uiBps);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.uiRun);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.uiNeon);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.uiTls);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.uiAlign);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.uiCom);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.uiLog);
            this.splitContainer1.Size = new System.Drawing.Size(983, 581);
            this.splitContainer1.SplitterDistance = 259;
            this.splitContainer1.TabIndex = 2;
            // 
            // uiCmd
            // 
            this.uiCmd.ForeColor = System.Drawing.Color.Coral;
            this.uiCmd.Location = new System.Drawing.Point(112, 479);
            this.uiCmd.Name = "uiCmd";
            this.uiCmd.Size = new System.Drawing.Size(100, 27);
            this.uiCmd.TabIndex = 13;
            this.uiCmd.Text = "I?";
            this.uiCmd.TextChanged += new System.EventHandler(this.uiCmd_TextChanged);
            // 
            // uiRunSerial
            // 
            this.uiRunSerial.Location = new System.Drawing.Point(112, 396);
            this.uiRunSerial.Name = "uiRunSerial";
            this.uiRunSerial.Size = new System.Drawing.Size(100, 49);
            this.uiRunSerial.TabIndex = 12;
            this.uiRunSerial.Text = "Run Serial";
            this.uiRunSerial.UseVisualStyleBackColor = true;
            this.uiRunSerial.Click += new System.EventHandler(this.uiRunSerial_Click);
            // 
            // uiNewLine
            // 
            this.uiNewLine.ForeColor = System.Drawing.Color.DarkOrange;
            this.uiNewLine.FormattingEnabled = true;
            this.uiNewLine.Items.AddRange(new object[] {
            "\\r",
            "\\n"});
            this.uiNewLine.Location = new System.Drawing.Point(119, 352);
            this.uiNewLine.Name = "uiNewLine";
            this.uiNewLine.Size = new System.Drawing.Size(93, 28);
            this.uiNewLine.TabIndex = 11;
            // 
            // uiBps
            // 
            this.uiBps.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiBps.ForeColor = System.Drawing.Color.DarkOrange;
            this.uiBps.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.uiBps.Location = new System.Drawing.Point(119, 306);
            this.uiBps.Maximum = new decimal(new int[] {
            115200,
            0,
            0,
            0});
            this.uiBps.Minimum = new decimal(new int[] {
            9600,
            0,
            0,
            0});
            this.uiBps.Name = "uiBps";
            this.uiBps.Size = new System.Drawing.Size(93, 25);
            this.uiBps.TabIndex = 10;
            this.uiBps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiBps.Value = new decimal(new int[] {
            9600,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 352);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 20);
            this.label6.TabIndex = 8;
            this.label6.Text = "NewLine";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 307);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "BaudRate";
            // 
            // uiRun
            // 
            this.uiRun.Location = new System.Drawing.Point(70, 204);
            this.uiRun.Name = "uiRun";
            this.uiRun.Size = new System.Drawing.Size(142, 49);
            this.uiRun.TabIndex = 9;
            this.uiRun.Text = "Run OSW";
            this.uiRun.UseVisualStyleBackColor = true;
            this.uiRun.Click += new System.EventHandler(this.uiRun_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "NEON OSW";
            // 
            // uiNeon
            // 
            this.uiNeon.AutoSize = true;
            this.uiNeon.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uiNeon.Location = new System.Drawing.Point(197, 173);
            this.uiNeon.Name = "uiNeon";
            this.uiNeon.Size = new System.Drawing.Size(15, 14);
            this.uiNeon.TabIndex = 7;
            this.uiNeon.UseVisualStyleBackColor = true;
            this.uiNeon.CheckedChanged += new System.EventHandler(this.uiNeon_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "TLS PORT";
            // 
            // uiTls
            // 
            this.uiTls.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTls.ForeColor = System.Drawing.Color.DarkOrange;
            this.uiTls.Location = new System.Drawing.Point(151, 119);
            this.uiTls.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.uiTls.Name = "uiTls";
            this.uiTls.Size = new System.Drawing.Size(61, 25);
            this.uiTls.TabIndex = 4;
            this.uiTls.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiTls.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "ALS PORT";
            // 
            // uiAlign
            // 
            this.uiAlign.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiAlign.ForeColor = System.Drawing.Color.DarkOrange;
            this.uiAlign.Location = new System.Drawing.Point(151, 75);
            this.uiAlign.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.uiAlign.Name = "uiAlign";
            this.uiAlign.Size = new System.Drawing.Size(61, 25);
            this.uiAlign.TabIndex = 2;
            this.uiAlign.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiAlign.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "COM";
            // 
            // uiCom
            // 
            this.uiCom.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiCom.ForeColor = System.Drawing.Color.DarkOrange;
            this.uiCom.Location = new System.Drawing.Point(151, 31);
            this.uiCom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.uiCom.Name = "uiCom";
            this.uiCom.Size = new System.Drawing.Size(61, 25);
            this.uiCom.TabIndex = 0;
            this.uiCom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.uiCom.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // uiLog
            // 
            this.uiLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uiLog.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiLog.Location = new System.Drawing.Point(0, 0);
            this.uiLog.Name = "uiLog";
            this.uiLog.Size = new System.Drawing.Size(720, 581);
            this.uiLog.TabIndex = 0;
            this.uiLog.Text = "";
            // 
            // uiRemoveNL
            // 
            this.uiRemoveNL.AutoSize = true;
            this.uiRemoveNL.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.uiRemoveNL.Location = new System.Drawing.Point(64, 486);
            this.uiRemoveNL.Name = "uiRemoveNL";
            this.uiRemoveNL.Size = new System.Drawing.Size(15, 14);
            this.uiRemoveNL.TabIndex = 14;
            this.uiRemoveNL.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(983, 627);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "Tester";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.uiBps)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiTls)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiAlign)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiCom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox uiLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown uiCom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown uiTls;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown uiAlign;
        private System.Windows.Forms.CheckBox uiNeon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button uiRun;
        private System.Windows.Forms.NumericUpDown uiBps;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox uiNewLine;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button uiRunSerial;
        private System.Windows.Forms.TextBox uiCmd;
        private System.Windows.Forms.CheckBox uiRemoveNL;
    }
}

