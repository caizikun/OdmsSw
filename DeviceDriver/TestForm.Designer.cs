namespace Neon.Aligner.Test
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
            this.uiParamGrid = new System.Windows.Forms.DataGridView();
            this.uiInitMc = new System.Windows.Forms.Button();
            this.uiCoord = new System.Windows.Forms.NumericUpDown();
            this.uiTimer = new System.Windows.Forms.Label();
            this.uiDistance = new System.Windows.Forms.NumericUpDown();
            this.uiMovePositive = new System.Windows.Forms.Button();
            this.uiMoveNegative = new System.Windows.Forms.Button();
            this.uiZeroing = new System.Windows.Forms.Button();
            this.uiMoveToHome = new System.Windows.Forms.Button();
            this.uiAbort = new System.Windows.Forms.Button();
            this.uiReadCoord = new System.Windows.Forms.Button();
            this.uiWriteCoord = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.uiNumLoop = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.uiMoveRepeat = new System.Windows.Forms.Button();
            this.uiReadConfigFile = new System.Windows.Forms.Button();
            this.uiAxis = new System.Windows.Forms.ComboBox();
            this.uiConfigFilePath = new System.Windows.Forms.LinkLabel();
            this.uiMcAxis_X = new System.Windows.Forms.CheckBox();
            this.uiMcAxis_Y = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.uiParamGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiCoord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiDistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiNumLoop)).BeginInit();
            this.SuspendLayout();
            // 
            // uiParamGrid
            // 
            this.uiParamGrid.AllowUserToAddRows = false;
            this.uiParamGrid.AllowUserToDeleteRows = false;
            this.uiParamGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.uiParamGrid.Dock = System.Windows.Forms.DockStyle.Left;
            this.uiParamGrid.Location = new System.Drawing.Point(0, 0);
            this.uiParamGrid.Name = "uiParamGrid";
            this.uiParamGrid.RowTemplate.Height = 35;
            this.uiParamGrid.Size = new System.Drawing.Size(374, 731);
            this.uiParamGrid.TabIndex = 0;
            // 
            // uiInitMc
            // 
            this.uiInitMc.Location = new System.Drawing.Point(397, 147);
            this.uiInitMc.Name = "uiInitMc";
            this.uiInitMc.Size = new System.Drawing.Size(120, 42);
            this.uiInitMc.TabIndex = 1;
            this.uiInitMc.Text = "Init MC";
            this.uiInitMc.UseVisualStyleBackColor = true;
            this.uiInitMc.Click += new System.EventHandler(this.uiInitMc_Click);
            // 
            // uiCoord
            // 
            this.uiCoord.BackColor = System.Drawing.Color.Black;
            this.uiCoord.DecimalPlaces = 4;
            this.uiCoord.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiCoord.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiCoord.Location = new System.Drawing.Point(539, 316);
            this.uiCoord.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.uiCoord.Minimum = new decimal(new int[] {
            99999999,
            0,
            0,
            -2147483648});
            this.uiCoord.Name = "uiCoord";
            this.uiCoord.Size = new System.Drawing.Size(120, 26);
            this.uiCoord.TabIndex = 2;
            this.uiCoord.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // uiTimer
            // 
            this.uiTimer.AutoSize = true;
            this.uiTimer.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiTimer.Location = new System.Drawing.Point(393, 703);
            this.uiTimer.Name = "uiTimer";
            this.uiTimer.Size = new System.Drawing.Size(63, 19);
            this.uiTimer.TabIndex = 3;
            this.uiTimer.Text = "label1";
            // 
            // uiDistance
            // 
            this.uiDistance.BackColor = System.Drawing.Color.Black;
            this.uiDistance.DecimalPlaces = 4;
            this.uiDistance.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiDistance.ForeColor = System.Drawing.Color.Tomato;
            this.uiDistance.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.uiDistance.Location = new System.Drawing.Point(693, 316);
            this.uiDistance.Maximum = new decimal(new int[] {
            99999999,
            0,
            0,
            0});
            this.uiDistance.Name = "uiDistance";
            this.uiDistance.Size = new System.Drawing.Size(120, 26);
            this.uiDistance.TabIndex = 4;
            this.uiDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.uiDistance.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // uiMovePositive
            // 
            this.uiMovePositive.Location = new System.Drawing.Point(693, 268);
            this.uiMovePositive.Name = "uiMovePositive";
            this.uiMovePositive.Size = new System.Drawing.Size(120, 42);
            this.uiMovePositive.TabIndex = 5;
            this.uiMovePositive.Text = "Move +";
            this.uiMovePositive.UseVisualStyleBackColor = true;
            this.uiMovePositive.Click += new System.EventHandler(this.uiMovePositive_Click);
            // 
            // uiMoveNegative
            // 
            this.uiMoveNegative.Location = new System.Drawing.Point(693, 348);
            this.uiMoveNegative.Name = "uiMoveNegative";
            this.uiMoveNegative.Size = new System.Drawing.Size(120, 42);
            this.uiMoveNegative.TabIndex = 6;
            this.uiMoveNegative.Text = "Move -";
            this.uiMoveNegative.UseVisualStyleBackColor = true;
            this.uiMoveNegative.Click += new System.EventHandler(this.uiMoveNegative_Click);
            // 
            // uiZeroing
            // 
            this.uiZeroing.Location = new System.Drawing.Point(397, 268);
            this.uiZeroing.Name = "uiZeroing";
            this.uiZeroing.Size = new System.Drawing.Size(120, 42);
            this.uiZeroing.TabIndex = 7;
            this.uiZeroing.Text = "Zeroing";
            this.uiZeroing.UseVisualStyleBackColor = true;
            this.uiZeroing.Click += new System.EventHandler(this.uiZeroing_Click);
            // 
            // uiMoveToHome
            // 
            this.uiMoveToHome.Location = new System.Drawing.Point(397, 316);
            this.uiMoveToHome.Name = "uiMoveToHome";
            this.uiMoveToHome.Size = new System.Drawing.Size(120, 42);
            this.uiMoveToHome.TabIndex = 7;
            this.uiMoveToHome.Text = "Move To Home";
            this.uiMoveToHome.UseVisualStyleBackColor = true;
            this.uiMoveToHome.Click += new System.EventHandler(this.uiMoveToHome_Click);
            // 
            // uiAbort
            // 
            this.uiAbort.Location = new System.Drawing.Point(819, 306);
            this.uiAbort.Name = "uiAbort";
            this.uiAbort.Size = new System.Drawing.Size(120, 42);
            this.uiAbort.TabIndex = 8;
            this.uiAbort.Text = "Abort";
            this.uiAbort.UseVisualStyleBackColor = true;
            this.uiAbort.Click += new System.EventHandler(this.uiAbort_Click);
            // 
            // uiReadCoord
            // 
            this.uiReadCoord.Location = new System.Drawing.Point(539, 268);
            this.uiReadCoord.Name = "uiReadCoord";
            this.uiReadCoord.Size = new System.Drawing.Size(120, 42);
            this.uiReadCoord.TabIndex = 9;
            this.uiReadCoord.Text = "Read Coord";
            this.uiReadCoord.UseVisualStyleBackColor = true;
            this.uiReadCoord.Click += new System.EventHandler(this.uiReadCoord_Click);
            // 
            // uiWriteCoord
            // 
            this.uiWriteCoord.Location = new System.Drawing.Point(539, 348);
            this.uiWriteCoord.Name = "uiWriteCoord";
            this.uiWriteCoord.Size = new System.Drawing.Size(120, 42);
            this.uiWriteCoord.TabIndex = 10;
            this.uiWriteCoord.Text = "Write Coord";
            this.uiWriteCoord.UseVisualStyleBackColor = true;
            this.uiWriteCoord.Click += new System.EventHandler(this.uiWriteCoord_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(435, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "초기화";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(583, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 16);
            this.label2.TabIndex = 12;
            this.label2.Text = "좌표";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(737, 243);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "변위";
            // 
            // uiNumLoop
            // 
            this.uiNumLoop.BackColor = System.Drawing.Color.Black;
            this.uiNumLoop.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiNumLoop.ForeColor = System.Drawing.Color.Tomato;
            this.uiNumLoop.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.uiNumLoop.Location = new System.Drawing.Point(693, 412);
            this.uiNumLoop.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.uiNumLoop.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.uiNumLoop.Name = "uiNumLoop";
            this.uiNumLoop.Size = new System.Drawing.Size(120, 26);
            this.uiNumLoop.TabIndex = 15;
            this.uiNumLoop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.uiNumLoop.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(654, 414);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 16);
            this.label4.TabIndex = 16;
            this.label4.Text = "반복";
            // 
            // uiMoveRepeat
            // 
            this.uiMoveRepeat.Location = new System.Drawing.Point(819, 402);
            this.uiMoveRepeat.Name = "uiMoveRepeat";
            this.uiMoveRepeat.Size = new System.Drawing.Size(120, 42);
            this.uiMoveRepeat.TabIndex = 17;
            this.uiMoveRepeat.Text = "Move Repeat";
            this.uiMoveRepeat.UseVisualStyleBackColor = true;
            this.uiMoveRepeat.Click += new System.EventHandler(this.uiMoveRepeat_Click);
            // 
            // uiReadConfigFile
            // 
            this.uiReadConfigFile.Location = new System.Drawing.Point(462, 46);
            this.uiReadConfigFile.Name = "uiReadConfigFile";
            this.uiReadConfigFile.Size = new System.Drawing.Size(136, 42);
            this.uiReadConfigFile.TabIndex = 18;
            this.uiReadConfigFile.Text = "Read Config File";
            this.uiReadConfigFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.uiReadConfigFile.UseVisualStyleBackColor = true;
            this.uiReadConfigFile.Click += new System.EventHandler(this.uiReadConfigFile_Click);
            // 
            // uiAxis
            // 
            this.uiAxis.BackColor = System.Drawing.Color.Black;
            this.uiAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiAxis.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiAxis.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiAxis.FormattingEnabled = true;
            this.uiAxis.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Tx",
            "Ty",
            "Tz"});
            this.uiAxis.Location = new System.Drawing.Point(395, 56);
            this.uiAxis.Name = "uiAxis";
            this.uiAxis.Size = new System.Drawing.Size(57, 26);
            this.uiAxis.TabIndex = 19;
            this.uiAxis.SelectedIndexChanged += new System.EventHandler(this.uiAxis_SelectedIndexChanged);
            // 
            // uiConfigFilePath
            // 
            this.uiConfigFilePath.AutoSize = true;
            this.uiConfigFilePath.Location = new System.Drawing.Point(604, 56);
            this.uiConfigFilePath.Name = "uiConfigFilePath";
            this.uiConfigFilePath.Size = new System.Drawing.Size(69, 16);
            this.uiConfigFilePath.TabIndex = 20;
            this.uiConfigFilePath.TabStop = true;
            this.uiConfigFilePath.Text = "linkLabel1";
            // 
            // uiMcAxis_X
            // 
            this.uiMcAxis_X.AutoSize = true;
            this.uiMcAxis_X.Checked = true;
            this.uiMcAxis_X.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uiMcAxis_X.Enabled = false;
            this.uiMcAxis_X.Location = new System.Drawing.Point(397, 106);
            this.uiMcAxis_X.Name = "uiMcAxis_X";
            this.uiMcAxis_X.Size = new System.Drawing.Size(62, 20);
            this.uiMcAxis_X.TabIndex = 21;
            this.uiMcAxis_X.Text = "MC_X";
            this.uiMcAxis_X.UseVisualStyleBackColor = true;
            this.uiMcAxis_X.CheckedChanged += new System.EventHandler(this.uiMcAxis_X_CheckedChanged);
            // 
            // uiMcAxis_Y
            // 
            this.uiMcAxis_Y.AutoSize = true;
            this.uiMcAxis_Y.Enabled = false;
            this.uiMcAxis_Y.Location = new System.Drawing.Point(464, 106);
            this.uiMcAxis_Y.Name = "uiMcAxis_Y";
            this.uiMcAxis_Y.Size = new System.Drawing.Size(63, 20);
            this.uiMcAxis_Y.TabIndex = 22;
            this.uiMcAxis_Y.Text = "MC_Y";
            this.uiMcAxis_Y.UseVisualStyleBackColor = true;
            this.uiMcAxis_Y.CheckedChanged += new System.EventHandler(this.uiMcAxis_Y_CheckedChanged);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1073, 731);
            this.Controls.Add(this.uiMcAxis_Y);
            this.Controls.Add(this.uiMcAxis_X);
            this.Controls.Add(this.uiConfigFilePath);
            this.Controls.Add(this.uiAxis);
            this.Controls.Add(this.uiReadConfigFile);
            this.Controls.Add(this.uiMoveRepeat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.uiNumLoop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.uiWriteCoord);
            this.Controls.Add(this.uiReadCoord);
            this.Controls.Add(this.uiAbort);
            this.Controls.Add(this.uiMoveToHome);
            this.Controls.Add(this.uiZeroing);
            this.Controls.Add(this.uiMoveNegative);
            this.Controls.Add(this.uiMovePositive);
            this.Controls.Add(this.uiDistance);
            this.Controls.Add(this.uiTimer);
            this.Controls.Add(this.uiCoord);
            this.Controls.Add(this.uiInitMc);
            this.Controls.Add(this.uiParamGrid);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "TestForm";
            this.Text = "TestForm";
            ((System.ComponentModel.ISupportInitialize)(this.uiParamGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiCoord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiDistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiNumLoop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView uiParamGrid;
        private System.Windows.Forms.Button uiInitMc;
        private System.Windows.Forms.NumericUpDown uiCoord;
        private System.Windows.Forms.Label uiTimer;
        private System.Windows.Forms.NumericUpDown uiDistance;
        private System.Windows.Forms.Button uiMovePositive;
        private System.Windows.Forms.Button uiMoveNegative;
        private System.Windows.Forms.Button uiZeroing;
        private System.Windows.Forms.Button uiMoveToHome;
        private System.Windows.Forms.Button uiAbort;
        private System.Windows.Forms.Button uiReadCoord;
        private System.Windows.Forms.Button uiWriteCoord;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown uiNumLoop;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button uiMoveRepeat;
        private System.Windows.Forms.Button uiReadConfigFile;
        private System.Windows.Forms.ComboBox uiAxis;
        private System.Windows.Forms.LinkLabel uiConfigFilePath;
        private System.Windows.Forms.CheckBox uiMcAxis_X;
        private System.Windows.Forms.CheckBox uiMcAxis_Y;
    }
}