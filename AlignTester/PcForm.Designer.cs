namespace AlignTester
{
    partial class PcForm
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
            this.uiGpib = new System.Windows.Forms.TextBox();
            this.uiOpen = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.uiPol_Write = new System.Windows.Forms.Button();
            this.uiPol_V = new System.Windows.Forms.RadioButton();
            this.uiPol_L = new System.Windows.Forms.RadioButton();
            this.uiPol_n45 = new System.Windows.Forms.RadioButton();
            this.uiPol_R = new System.Windows.Forms.RadioButton();
            this.uiPol_p45 = new System.Windows.Forms.RadioButton();
            this.uiPol_H = new System.Windows.Forms.RadioButton();
            this.uiPol_Read = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // uiGpib
            // 
            this.uiGpib.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.uiGpib.ForeColor = System.Drawing.Color.DodgerBlue;
            this.uiGpib.Location = new System.Drawing.Point(12, 13);
            this.uiGpib.Name = "uiGpib";
            this.uiGpib.Size = new System.Drawing.Size(45, 23);
            this.uiGpib.TabIndex = 314;
            this.uiGpib.Text = "20";
            this.uiGpib.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // uiOpen
            // 
            this.uiOpen.Location = new System.Drawing.Point(137, 13);
            this.uiOpen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.uiOpen.Name = "uiOpen";
            this.uiOpen.Size = new System.Drawing.Size(100, 43);
            this.uiOpen.TabIndex = 315;
            this.uiOpen.Text = "Open";
            this.uiOpen.UseVisualStyleBackColor = true;
            this.uiOpen.Click += new System.EventHandler(this.uiOpen_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.uiPol_Read);
            this.groupBox1.Controls.Add(this.uiPol_Write);
            this.groupBox1.Controls.Add(this.uiPol_V);
            this.groupBox1.Controls.Add(this.uiPol_L);
            this.groupBox1.Controls.Add(this.uiPol_n45);
            this.groupBox1.Controls.Add(this.uiPol_R);
            this.groupBox1.Controls.Add(this.uiPol_p45);
            this.groupBox1.Controls.Add(this.uiPol_H);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(10);
            this.groupBox1.Size = new System.Drawing.Size(244, 243);
            this.groupBox1.TabIndex = 316;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "O-Band PC";
            // 
            // uiPol_Write
            // 
            this.uiPol_Write.Location = new System.Drawing.Point(125, 29);
            this.uiPol_Write.Name = "uiPol_Write";
            this.uiPol_Write.Size = new System.Drawing.Size(100, 52);
            this.uiPol_Write.TabIndex = 1;
            this.uiPol_Write.Text = "Write";
            this.uiPol_Write.UseVisualStyleBackColor = true;
            this.uiPol_Write.Click += new System.EventHandler(this.uiPol_Write_Click);
            // 
            // uiPol_V
            // 
            this.uiPol_V.AutoSize = true;
            this.uiPol_V.Location = new System.Drawing.Point(13, 64);
            this.uiPol_V.Name = "uiPol_V";
            this.uiPol_V.Size = new System.Drawing.Size(65, 19);
            this.uiPol_V.TabIndex = 0;
            this.uiPol_V.TabStop = true;
            this.uiPol_V.Text = "Vertical";
            this.uiPol_V.UseVisualStyleBackColor = true;
            // 
            // uiPol_L
            // 
            this.uiPol_L.AutoSize = true;
            this.uiPol_L.Location = new System.Drawing.Point(13, 204);
            this.uiPol_L.Name = "uiPol_L";
            this.uiPol_L.Size = new System.Drawing.Size(90, 19);
            this.uiPol_L.TabIndex = 0;
            this.uiPol_L.TabStop = true;
            this.uiPol_L.Text = "Left Circular";
            this.uiPol_L.UseVisualStyleBackColor = true;
            // 
            // uiPol_n45
            // 
            this.uiPol_n45.AutoSize = true;
            this.uiPol_n45.Location = new System.Drawing.Point(13, 134);
            this.uiPol_n45.Name = "uiPol_n45";
            this.uiPol_n45.Size = new System.Drawing.Size(49, 19);
            this.uiPol_n45.TabIndex = 0;
            this.uiPol_n45.TabStop = true;
            this.uiPol_n45.Text = "-45º";
            this.uiPol_n45.UseVisualStyleBackColor = true;
            // 
            // uiPol_R
            // 
            this.uiPol_R.AutoSize = true;
            this.uiPol_R.Location = new System.Drawing.Point(13, 169);
            this.uiPol_R.Name = "uiPol_R";
            this.uiPol_R.Size = new System.Drawing.Size(98, 19);
            this.uiPol_R.TabIndex = 0;
            this.uiPol_R.TabStop = true;
            this.uiPol_R.Text = "Rigth Circular";
            this.uiPol_R.UseVisualStyleBackColor = true;
            // 
            // uiPol_p45
            // 
            this.uiPol_p45.AutoSize = true;
            this.uiPol_p45.Location = new System.Drawing.Point(13, 99);
            this.uiPol_p45.Name = "uiPol_p45";
            this.uiPol_p45.Size = new System.Drawing.Size(52, 19);
            this.uiPol_p45.TabIndex = 0;
            this.uiPol_p45.TabStop = true;
            this.uiPol_p45.Text = "+45º";
            this.uiPol_p45.UseVisualStyleBackColor = true;
            // 
            // uiPol_H
            // 
            this.uiPol_H.AutoSize = true;
            this.uiPol_H.Location = new System.Drawing.Point(13, 29);
            this.uiPol_H.Name = "uiPol_H";
            this.uiPol_H.Size = new System.Drawing.Size(81, 19);
            this.uiPol_H.TabIndex = 0;
            this.uiPol_H.TabStop = true;
            this.uiPol_H.Text = "Horizontal";
            this.uiPol_H.UseVisualStyleBackColor = true;
            // 
            // uiPol_Read
            // 
            this.uiPol_Read.Location = new System.Drawing.Point(125, 99);
            this.uiPol_Read.Name = "uiPol_Read";
            this.uiPol_Read.Size = new System.Drawing.Size(100, 52);
            this.uiPol_Read.TabIndex = 2;
            this.uiPol_Read.Text = "Read";
            this.uiPol_Read.UseVisualStyleBackColor = true;
            this.uiPol_Read.Click += new System.EventHandler(this.uiPol_Read_Click);
            // 
            // PcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 336);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.uiOpen);
            this.Controls.Add(this.uiGpib);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PcForm";
            this.Text = "PcForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox uiGpib;
        internal System.Windows.Forms.Button uiOpen;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button uiPol_Write;
        private System.Windows.Forms.RadioButton uiPol_V;
        private System.Windows.Forms.RadioButton uiPol_L;
        private System.Windows.Forms.RadioButton uiPol_n45;
        private System.Windows.Forms.RadioButton uiPol_R;
        private System.Windows.Forms.RadioButton uiPol_p45;
        private System.Windows.Forms.RadioButton uiPol_H;
        private System.Windows.Forms.Button uiPol_Read;
    }
}