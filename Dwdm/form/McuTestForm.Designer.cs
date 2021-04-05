namespace Neon.Dwdm.form
{
    partial class McuTestForm
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
            this.uiAttInit = new System.Windows.Forms.Label();
            this.uiAttSlot = new System.Windows.Forms.TextBox();
            this.uiMcuComPort = new System.Windows.Forms.TextBox();
            this.uiMcuInit = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.uiOpmCh = new System.Windows.Forms.ComboBox();
            this.uiScanRange = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.uiSaveFolder = new System.Windows.Forms.LinkLabel();
            this.uiLog = new System.Windows.Forms.RichTextBox();
            this.uiRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.uiMcuMonitor = new System.Windows.Forms.TextBox();
            this.uiMcuCh = new System.Windows.Forms.ComboBox();
            this.uiValueType = new System.Windows.Forms.ComboBox();
            this.uiNumDp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uiAttInit
            // 
            this.uiAttInit.AutoSize = true;
            this.uiAttInit.Location = new System.Drawing.Point(126, 46);
            this.uiAttInit.Name = "uiAttInit";
            this.uiAttInit.Size = new System.Drawing.Size(114, 19);
            this.uiAttInit.TabIndex = 0;
            this.uiAttInit.Text = "Attenuator SLOT";
            // 
            // uiAttSlot
            // 
            this.uiAttSlot.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiAttSlot.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiAttSlot.ForeColor = System.Drawing.Color.White;
            this.uiAttSlot.Location = new System.Drawing.Point(12, 42);
            this.uiAttSlot.Name = "uiAttSlot";
            this.uiAttSlot.Size = new System.Drawing.Size(94, 25);
            this.uiAttSlot.TabIndex = 2;
            this.uiAttSlot.Text = "3";
            this.uiAttSlot.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // uiMcuComPort
            // 
            this.uiMcuComPort.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiMcuComPort.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiMcuComPort.ForeColor = System.Drawing.Color.White;
            this.uiMcuComPort.Location = new System.Drawing.Point(12, 10);
            this.uiMcuComPort.Name = "uiMcuComPort";
            this.uiMcuComPort.Size = new System.Drawing.Size(94, 25);
            this.uiMcuComPort.TabIndex = 1;
            this.uiMcuComPort.Text = "14";
            this.uiMcuComPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // uiMcuInit
            // 
            this.uiMcuInit.AutoSize = true;
            this.uiMcuInit.Location = new System.Drawing.Point(126, 14);
            this.uiMcuInit.Name = "uiMcuInit";
            this.uiMcuInit.Size = new System.Drawing.Size(92, 19);
            this.uiMcuInit.TabIndex = 0;
            this.uiMcuInit.Text = "MCU COM #";
            this.uiMcuInit.Click += new System.EventHandler(this.uiMcuInit_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(126, 105);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 19);
            this.label3.TabIndex = 0;
            this.label3.Text = "Monitor OPM CH";
            // 
            // uiOpmCh
            // 
            this.uiOpmCh.BackColor = System.Drawing.Color.Black;
            this.uiOpmCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiOpmCh.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiOpmCh.ForeColor = System.Drawing.Color.White;
            this.uiOpmCh.FormattingEnabled = true;
            this.uiOpmCh.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
            this.uiOpmCh.Location = new System.Drawing.Point(12, 104);
            this.uiOpmCh.Name = "uiOpmCh";
            this.uiOpmCh.Size = new System.Drawing.Size(94, 26);
            this.uiOpmCh.TabIndex = 4;
            // 
            // uiScanRange
            // 
            this.uiScanRange.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiScanRange.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiScanRange.ForeColor = System.Drawing.Color.White;
            this.uiScanRange.Location = new System.Drawing.Point(12, 73);
            this.uiScanRange.Name = "uiScanRange";
            this.uiScanRange.Size = new System.Drawing.Size(94, 25);
            this.uiScanRange.TabIndex = 3;
            this.uiScanRange.Text = "0 -30 1.0";
            this.uiScanRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 19);
            this.label4.TabIndex = 0;
            this.label4.Text = "Att Scan Range (dBm)";
            // 
            // uiSaveFolder
            // 
            this.uiSaveFolder.AutoSize = true;
            this.uiSaveFolder.Location = new System.Drawing.Point(12, 179);
            this.uiSaveFolder.Name = "uiSaveFolder";
            this.uiSaveFolder.Size = new System.Drawing.Size(81, 19);
            this.uiSaveFolder.TabIndex = 14;
            this.uiSaveFolder.TabStop = true;
            this.uiSaveFolder.Text = "Save Folder";
            this.uiSaveFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.uiSaveFolder_LinkClicked);
            // 
            // uiLog
            // 
            this.uiLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiLog.Location = new System.Drawing.Point(12, 217);
            this.uiLog.Name = "uiLog";
            this.uiLog.Size = new System.Drawing.Size(460, 447);
            this.uiLog.TabIndex = 15;
            this.uiLog.Text = "";
            // 
            // uiRun
            // 
            this.uiRun.Location = new System.Drawing.Point(343, 147);
            this.uiRun.Name = "uiRun";
            this.uiRun.Size = new System.Drawing.Size(129, 28);
            this.uiRun.TabIndex = 6;
            this.uiRun.Text = "Run";
            this.uiRun.UseVisualStyleBackColor = true;
            this.uiRun.Click += new System.EventHandler(this.uiRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "MCU CH";
            // 
            // uiMcuMonitor
            // 
            this.uiMcuMonitor.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiMcuMonitor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiMcuMonitor.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiMcuMonitor.ForeColor = System.Drawing.Color.OrangeRed;
            this.uiMcuMonitor.Location = new System.Drawing.Point(212, 154);
            this.uiMcuMonitor.Name = "uiMcuMonitor";
            this.uiMcuMonitor.Size = new System.Drawing.Size(104, 18);
            this.uiMcuMonitor.TabIndex = 16;
            this.uiMcuMonitor.Text = "NaN";
            this.uiMcuMonitor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // uiMcuCh
            // 
            this.uiMcuCh.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiMcuCh.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiMcuCh.FormattingEnabled = true;
            this.uiMcuCh.Location = new System.Drawing.Point(12, 149);
            this.uiMcuCh.Name = "uiMcuCh";
            this.uiMcuCh.Size = new System.Drawing.Size(94, 26);
            this.uiMcuCh.TabIndex = 17;
            this.uiMcuCh.SelectedIndexChanged += new System.EventHandler(this.uiMcuCh_SelectedIndexChanged);
            // 
            // uiValueType
            // 
            this.uiValueType.BackColor = System.Drawing.Color.Black;
            this.uiValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiValueType.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiValueType.ForeColor = System.Drawing.Color.White;
            this.uiValueType.FormattingEnabled = true;
            this.uiValueType.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40"});
            this.uiValueType.Location = new System.Drawing.Point(378, 73);
            this.uiValueType.Name = "uiValueType";
            this.uiValueType.Size = new System.Drawing.Size(94, 26);
            this.uiValueType.TabIndex = 18;
            this.uiValueType.SelectedIndexChanged += new System.EventHandler(this.uiValueType_SelectedIndexChanged);
            // 
            // uiNumDp
            // 
            this.uiNumDp.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiNumDp.Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiNumDp.ForeColor = System.Drawing.Color.White;
            this.uiNumDp.Location = new System.Drawing.Point(378, 105);
            this.uiNumDp.Name = "uiNumDp";
            this.uiNumDp.Size = new System.Drawing.Size(94, 25);
            this.uiNumDp.TabIndex = 19;
            this.uiNumDp.Text = "1";
            this.uiNumDp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(341, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(131, 19);
            this.label2.TabIndex = 20;
            this.label2.Text = "Data Type && Points";
            // 
            // McuTestForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(525, 676);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.uiNumDp);
            this.Controls.Add(this.uiValueType);
            this.Controls.Add(this.uiMcuCh);
            this.Controls.Add(this.uiMcuMonitor);
            this.Controls.Add(this.uiRun);
            this.Controls.Add(this.uiLog);
            this.Controls.Add(this.uiSaveFolder);
            this.Controls.Add(this.uiScanRange);
            this.Controls.Add(this.uiOpmCh);
            this.Controls.Add(this.uiMcuComPort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uiAttSlot);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.uiMcuInit);
            this.Controls.Add(this.uiAttInit);
            this.Font = new System.Drawing.Font("Malgun Gothic", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "McuTestForm";
            this.Text = "McuTestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label uiAttInit;
        private System.Windows.Forms.TextBox uiAttSlot;
        private System.Windows.Forms.TextBox uiMcuComPort;
        private System.Windows.Forms.Label uiMcuInit;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.ComboBox uiOpmCh;
        private System.Windows.Forms.TextBox uiScanRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel uiSaveFolder;
        private System.Windows.Forms.RichTextBox uiLog;
        private System.Windows.Forms.Button uiRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox uiMcuMonitor;
        private System.Windows.Forms.ComboBox uiMcuCh;
        internal System.Windows.Forms.ComboBox uiValueType;
        private System.Windows.Forms.TextBox uiNumDp;
        private System.Windows.Forms.Label label2;
    }
}