
partial class OpmDisplayForm
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
            this.btnUnit = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.uiGroupBox = new System.Windows.Forms.GroupBox();
            this.uiSetRef2 = new System.Windows.Forms.Label();
            this.uiSetRef1 = new System.Windows.Forms.Label();
            this.uiRef2 = new System.Windows.Forms.TextBox();
            this.uiRef1 = new System.Windows.Forms.TextBox();
            this.uiPort2 = new System.Windows.Forms.ComboBox();
            this.uiPort1 = new System.Windows.Forms.ComboBox();
            this.uiValue1 = new System.Windows.Forms.TextBox();
            this.uiValue2 = new System.Windows.Forms.TextBox();
            this.uiGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUnit
            // 
            this.btnUnit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUnit.Font = new System.Drawing.Font("Myriad Pro", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnit.Location = new System.Drawing.Point(204, 99);
            this.btnUnit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnUnit.Name = "btnUnit";
            this.btnUnit.Size = new System.Drawing.Size(73, 38);
            this.btnUnit.TabIndex = 113;
            this.btnUnit.Text = "dBm";
            this.btnUnit.UseVisualStyleBackColor = true;
            this.btnUnit.Click += new System.EventHandler(this.btnUnit_Click);
            // 
            // btnStart
            // 
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStart.Font = new System.Drawing.Font("Myriad Pro", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.Location = new System.Drawing.Point(70, 99);
            this.btnStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(103, 38);
            this.btnStart.TabIndex = 111;
            this.btnStart.Text = "OFF";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // uiGroupBox
            // 
            this.uiGroupBox.Controls.Add(this.uiSetRef2);
            this.uiGroupBox.Controls.Add(this.uiSetRef1);
            this.uiGroupBox.Controls.Add(this.uiRef2);
            this.uiGroupBox.Controls.Add(this.uiRef1);
            this.uiGroupBox.Controls.Add(this.uiPort2);
            this.uiGroupBox.Controls.Add(this.uiPort1);
            this.uiGroupBox.Controls.Add(this.uiValue1);
            this.uiGroupBox.Controls.Add(this.uiValue2);
            this.uiGroupBox.Location = new System.Drawing.Point(5, 0);
            this.uiGroupBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiGroupBox.Name = "uiGroupBox";
            this.uiGroupBox.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiGroupBox.Size = new System.Drawing.Size(279, 92);
            this.uiGroupBox.TabIndex = 110;
            this.uiGroupBox.TabStop = false;
            // 
            // uiSetRef2
            // 
            this.uiSetRef2.AutoSize = true;
            this.uiSetRef2.Location = new System.Drawing.Point(174, 56);
            this.uiSetRef2.Name = "uiSetRef2";
            this.uiSetRef2.Size = new System.Drawing.Size(19, 15);
            this.uiSetRef2.TabIndex = 125;
            this.uiSetRef2.Text = "▶";
            this.uiSetRef2.Click += new System.EventHandler(this.uiSetRef2_Click);
            // 
            // uiSetRef1
            // 
            this.uiSetRef1.AutoSize = true;
            this.uiSetRef1.Location = new System.Drawing.Point(174, 20);
            this.uiSetRef1.Name = "uiSetRef1";
            this.uiSetRef1.Size = new System.Drawing.Size(19, 15);
            this.uiSetRef1.TabIndex = 124;
            this.uiSetRef1.Text = "▶";
            this.uiSetRef1.Click += new System.EventHandler(this.uiSetRef1_Click);
            // 
            // uiRef2
            // 
            this.uiRef2.BackColor = System.Drawing.Color.Black;
            this.uiRef2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiRef2.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiRef2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiRef2.Location = new System.Drawing.Point(199, 51);
            this.uiRef2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiRef2.Name = "uiRef2";
            this.uiRef2.Size = new System.Drawing.Size(73, 22);
            this.uiRef2.TabIndex = 123;
            this.uiRef2.Text = "-0.314";
            this.uiRef2.TextChanged += new System.EventHandler(this.uiRef2_TextChanged);
            // 
            // uiRef1
            // 
            this.uiRef1.BackColor = System.Drawing.Color.Black;
            this.uiRef1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiRef1.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiRef1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiRef1.Location = new System.Drawing.Point(199, 16);
            this.uiRef1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiRef1.Name = "uiRef1";
            this.uiRef1.Size = new System.Drawing.Size(73, 22);
            this.uiRef1.TabIndex = 114;
            this.uiRef1.Text = "-0.314";
            this.uiRef1.TextChanged += new System.EventHandler(this.uiRef1_TextChanged);
            // 
            // uiPort2
            // 
            this.uiPort2.BackColor = System.Drawing.Color.Black;
            this.uiPort2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiPort2.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPort2.ForeColor = System.Drawing.Color.White;
            this.uiPort2.FormattingEnabled = true;
            this.uiPort2.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.uiPort2.Location = new System.Drawing.Point(6, 50);
            this.uiPort2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiPort2.Name = "uiPort2";
            this.uiPort2.Size = new System.Drawing.Size(53, 27);
            this.uiPort2.TabIndex = 122;
            this.uiPort2.SelectedIndexChanged += new System.EventHandler(this.cbChSecond_SelectedIndexChanged);
            // 
            // uiPort1
            // 
            this.uiPort1.BackColor = System.Drawing.Color.Black;
            this.uiPort1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uiPort1.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiPort1.ForeColor = System.Drawing.Color.White;
            this.uiPort1.FormattingEnabled = true;
            this.uiPort1.Items.AddRange(new object[] {
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
            "27",
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
            this.uiPort1.Location = new System.Drawing.Point(6, 15);
            this.uiPort1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiPort1.Name = "uiPort1";
            this.uiPort1.Size = new System.Drawing.Size(53, 27);
            this.uiPort1.TabIndex = 121;
            this.uiPort1.SelectedIndexChanged += new System.EventHandler(this.cbChFirst_SelectedIndexChanged);
            // 
            // uiValue1
            // 
            this.uiValue1.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiValue1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiValue1.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiValue1.ForeColor = System.Drawing.Color.Lime;
            this.uiValue1.Location = new System.Drawing.Point(65, 11);
            this.uiValue1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiValue1.Name = "uiValue1";
            this.uiValue1.ReadOnly = true;
            this.uiValue1.Size = new System.Drawing.Size(103, 29);
            this.uiValue1.TabIndex = 96;
            this.uiValue1.Text = "-12.123";
            // 
            // uiValue2
            // 
            this.uiValue2.BackColor = System.Drawing.SystemColors.MenuText;
            this.uiValue2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.uiValue2.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiValue2.ForeColor = System.Drawing.Color.Lime;
            this.uiValue2.Location = new System.Drawing.Point(65, 46);
            this.uiValue2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiValue2.Name = "uiValue2";
            this.uiValue2.ReadOnly = true;
            this.uiValue2.Size = new System.Drawing.Size(103, 29);
            this.uiValue2.TabIndex = 100;
            this.uiValue2.Text = "-12.123";
            // 
            // OpmDisplayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 144);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.uiGroupBox);
            this.Controls.Add(this.btnUnit);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OpmDisplayForm";
            this.Text = "Optical Powermeter";
            this.uiGroupBox.ResumeLayout(false);
            this.uiGroupBox.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    internal System.Windows.Forms.Button btnUnit;
    internal System.Windows.Forms.Button btnStart;
    internal System.Windows.Forms.GroupBox uiGroupBox;
    internal System.Windows.Forms.ComboBox uiPort2;
    internal System.Windows.Forms.ComboBox uiPort1;
    internal System.Windows.Forms.TextBox uiValue1;
    internal System.Windows.Forms.TextBox uiValue2;
    internal System.Windows.Forms.TextBox uiRef1;
    internal System.Windows.Forms.TextBox uiRef2;
    private System.Windows.Forms.Label uiSetRef2;
    private System.Windows.Forms.Label uiSetRef1;
}
