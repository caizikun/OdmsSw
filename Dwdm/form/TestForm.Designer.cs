namespace Neon.Dwdm
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
			this.btnDaqInit = new System.Windows.Forms.Button();
			this.cboDaqPrimary = new System.Windows.Forms.ComboBox();
			this.btnHigh = new System.Windows.Forms.Button();
			this.btnLow = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnDaqInit
			// 
			this.btnDaqInit.Location = new System.Drawing.Point(281, 46);
			this.btnDaqInit.Name = "btnDaqInit";
			this.btnDaqInit.Size = new System.Drawing.Size(75, 23);
			this.btnDaqInit.TabIndex = 0;
			this.btnDaqInit.Text = "Init";
			this.btnDaqInit.UseVisualStyleBackColor = true;
			this.btnDaqInit.Click += new System.EventHandler(this.btnDaqInit_Click);
			// 
			// cboDaqPrimary
			// 
			this.cboDaqPrimary.BackColor = System.Drawing.Color.Black;
			this.cboDaqPrimary.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboDaqPrimary.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.cboDaqPrimary.ForeColor = System.Drawing.Color.White;
			this.cboDaqPrimary.FormattingEnabled = true;
			this.cboDaqPrimary.Items.AddRange(new object[] {
            "X",
            "Y",
            "Z",
            "Tx",
            "Ty"});
			this.cboDaqPrimary.Location = new System.Drawing.Point(72, 46);
			this.cboDaqPrimary.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.cboDaqPrimary.Name = "cboDaqPrimary";
			this.cboDaqPrimary.Size = new System.Drawing.Size(177, 23);
			this.cboDaqPrimary.TabIndex = 159;
			// 
			// btnHigh
			// 
			this.btnHigh.Location = new System.Drawing.Point(281, 109);
			this.btnHigh.Name = "btnHigh";
			this.btnHigh.Size = new System.Drawing.Size(75, 23);
			this.btnHigh.TabIndex = 160;
			this.btnHigh.Text = "High";
			this.btnHigh.UseVisualStyleBackColor = true;
			this.btnHigh.Click += new System.EventHandler(this.btnHigh_Click);
			// 
			// btnLow
			// 
			this.btnLow.Location = new System.Drawing.Point(395, 109);
			this.btnLow.Name = "btnLow";
			this.btnLow.Size = new System.Drawing.Size(75, 23);
			this.btnLow.TabIndex = 161;
			this.btnLow.Text = "Low";
			this.btnLow.UseVisualStyleBackColor = true;
			this.btnLow.Click += new System.EventHandler(this.btnLow_Click);
			// 
			// button3
			// 
			this.button3.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button3.Location = new System.Drawing.Point(444, 306);
			this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(120, 44);
			this.button3.TabIndex = 177;
			this.button3.Text = "Air Right close";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button4.Location = new System.Drawing.Point(444, 254);
			this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(120, 44);
			this.button4.TabIndex = 176;
			this.button4.Text = "Air Right Fix";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.Location = new System.Drawing.Point(281, 306);
			this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(120, 44);
			this.button2.TabIndex = 175;
			this.button2.Text = "Air Left close";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.Location = new System.Drawing.Point(281, 254);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 44);
			this.button1.TabIndex = 174;
			this.button1.Text = "Air Left Fix";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// TestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(892, 546);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnLow);
			this.Controls.Add(this.btnHigh);
			this.Controls.Add(this.cboDaqPrimary);
			this.Controls.Add(this.btnDaqInit);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "TestForm";
			this.Text = "TestForm";
			this.Load += new System.EventHandler(this.TestForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnDaqInit;
		internal System.Windows.Forms.ComboBox cboDaqPrimary;
		private System.Windows.Forms.Button btnHigh;
		private System.Windows.Forms.Button btnLow;
		internal System.Windows.Forms.Button button3;
		internal System.Windows.Forms.Button button4;
		internal System.Windows.Forms.Button button2;
		internal System.Windows.Forms.Button button1;
	}
}