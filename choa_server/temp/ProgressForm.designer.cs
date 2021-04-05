
partial class ProgressForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
			this.lbPercentSymbol = new System.Windows.Forms.Label();
			this.lbRemainTime = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.lbProcessTime = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lbEndTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lbStartTime = new System.Windows.Forms.Label();
			this.Label24 = new System.Windows.Forms.Label();
			this.lbEstiEndTime = new System.Windows.Forms.Label();
			this.Label1 = new System.Windows.Forms.Label();
			this.lbCurItemNo = new System.Windows.Forms.Label();
			this.Label20 = new System.Windows.Forms.Label();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.lbPercent = new System.Windows.Forms.Label();
			this.statusProgressBar = new System.Windows.Forms.ProgressBar();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lbPercentSymbol
			// 
			this.lbPercentSymbol.AutoSize = true;
			this.lbPercentSymbol.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbPercentSymbol.Location = new System.Drawing.Point(155, 162);
			this.lbPercentSymbol.Name = "lbPercentSymbol";
			this.lbPercentSymbol.Size = new System.Drawing.Size(17, 15);
			this.lbPercentSymbol.TabIndex = 279;
			this.lbPercentSymbol.Text = "%";
			// 
			// lbRemainTime
			// 
			this.lbRemainTime.AutoSize = true;
			this.lbRemainTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbRemainTime.Location = new System.Drawing.Point(123, 61);
			this.lbRemainTime.Name = "lbRemainTime";
			this.lbRemainTime.Size = new System.Drawing.Size(16, 15);
			this.lbRemainTime.TabIndex = 278;
			this.lbRemainTime.Text = "...";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(5, 61);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(83, 15);
			this.label6.TabIndex = 277;
			this.label6.Text = "Remain Time :";
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnOK.Location = new System.Drawing.Point(95, 176);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(114, 34);
			this.btnOK.TabIndex = 276;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lbProcessTime
			// 
			this.lbProcessTime.AutoSize = true;
			this.lbProcessTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbProcessTime.Location = new System.Drawing.Point(123, 99);
			this.lbProcessTime.Name = "lbProcessTime";
			this.lbProcessTime.Size = new System.Drawing.Size(16, 15);
			this.lbProcessTime.TabIndex = 275;
			this.lbProcessTime.Text = "...";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(24, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(64, 15);
			this.label4.TabIndex = 274;
			this.label4.Text = "걸린 시간 :";
			// 
			// lbEndTime
			// 
			this.lbEndTime.AutoSize = true;
			this.lbEndTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbEndTime.Location = new System.Drawing.Point(123, 80);
			this.lbEndTime.Name = "lbEndTime";
			this.lbEndTime.Size = new System.Drawing.Size(16, 15);
			this.lbEndTime.TabIndex = 273;
			this.lbEndTime.Text = "...";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(22, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 15);
			this.label2.TabIndex = 272;
			this.label2.Text = "End Time  :";
			// 
			// lbStartTime
			// 
			this.lbStartTime.AutoSize = true;
			this.lbStartTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbStartTime.Location = new System.Drawing.Point(123, 23);
			this.lbStartTime.Name = "lbStartTime";
			this.lbStartTime.Size = new System.Drawing.Size(16, 15);
			this.lbStartTime.TabIndex = 271;
			this.lbStartTime.Text = "...";
			// 
			// Label24
			// 
			this.Label24.AutoSize = true;
			this.Label24.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label24.Location = new System.Drawing.Point(21, 23);
			this.Label24.Name = "Label24";
			this.Label24.Size = new System.Drawing.Size(67, 15);
			this.Label24.TabIndex = 270;
			this.Label24.Text = "Start Time :";
			// 
			// lbEstiEndTime
			// 
			this.lbEstiEndTime.AutoSize = true;
			this.lbEstiEndTime.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbEstiEndTime.Location = new System.Drawing.Point(123, 42);
			this.lbEstiEndTime.Name = "lbEstiEndTime";
			this.lbEstiEndTime.Size = new System.Drawing.Size(16, 15);
			this.lbEstiEndTime.TabIndex = 269;
			this.lbEstiEndTime.Text = "...";
			// 
			// Label1
			// 
			this.Label1.AutoSize = true;
			this.Label1.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label1.Location = new System.Drawing.Point(4, 42);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(84, 15);
			this.Label1.TabIndex = 268;
			this.Label1.Text = "Est. End Time :";
			// 
			// lbCurItemNo
			// 
			this.lbCurItemNo.AutoSize = true;
			this.lbCurItemNo.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbCurItemNo.Location = new System.Drawing.Point(123, 4);
			this.lbCurItemNo.Name = "lbCurItemNo";
			this.lbCurItemNo.Size = new System.Drawing.Size(16, 15);
			this.lbCurItemNo.TabIndex = 267;
			this.lbCurItemNo.Text = "...";
			// 
			// Label20
			// 
			this.Label20.AutoSize = true;
			this.Label20.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Label20.Location = new System.Drawing.Point(51, 4);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(37, 15);
			this.Label20.TabIndex = 266;
			this.Label20.Text = "Item :";
			// 
			// txtMessage
			// 
			this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtMessage.Location = new System.Drawing.Point(7, 122);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.ReadOnly = true;
			this.txtMessage.Size = new System.Drawing.Size(280, 40);
			this.txtMessage.TabIndex = 265;
			this.txtMessage.TabStop = false;
			this.txtMessage.Text = "message!!";
			// 
			// lbPercent
			// 
			this.lbPercent.Font = new System.Drawing.Font("Segoe UI Symbol", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lbPercent.Location = new System.Drawing.Point(120, 162);
			this.lbPercent.Name = "lbPercent";
			this.lbPercent.Size = new System.Drawing.Size(29, 15);
			this.lbPercent.TabIndex = 264;
			this.lbPercent.Text = "???";
			this.lbPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// statusProgressBar
			// 
			this.statusProgressBar.Location = new System.Drawing.Point(7, 177);
			this.statusProgressBar.Name = "statusProgressBar";
			this.statusProgressBar.Size = new System.Drawing.Size(280, 33);
			this.statusProgressBar.TabIndex = 263;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// frmProcessRes
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(292, 218);
			this.ControlBox = false;
			this.Controls.Add(this.lbPercentSymbol);
			this.Controls.Add(this.lbRemainTime);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.lbProcessTime);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.lbEndTime);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbStartTime);
			this.Controls.Add(this.Label24);
			this.Controls.Add(this.lbEstiEndTime);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.lbCurItemNo);
			this.Controls.Add(this.Label20);
			this.Controls.Add(this.txtMessage);
			this.Controls.Add(this.lbPercent);
			this.Controls.Add(this.statusProgressBar);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmProcessRes";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Processing";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProcessRes_FormClosing);
			this.Load += new System.EventHandler(this.frmProcessRes_Load);
			this.Shown += new System.EventHandler(this.frmProcessRes_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lbPercentSymbol;
    internal System.Windows.Forms.Label lbRemainTime;
    internal System.Windows.Forms.Label label6;
    private System.Windows.Forms.Button btnOK;
    internal System.Windows.Forms.Label lbProcessTime;
    internal System.Windows.Forms.Label label4;
    internal System.Windows.Forms.Label lbEndTime;
    internal System.Windows.Forms.Label label2;
    internal System.Windows.Forms.Label lbStartTime;
    internal System.Windows.Forms.Label Label24;
    internal System.Windows.Forms.Label lbEstiEndTime;
    internal System.Windows.Forms.Label Label1;
    internal System.Windows.Forms.Label lbCurItemNo;
    internal System.Windows.Forms.Label Label20;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Label lbPercent;
    private System.Windows.Forms.ProgressBar statusProgressBar;
    private System.Windows.Forms.Timer timer1;
}
