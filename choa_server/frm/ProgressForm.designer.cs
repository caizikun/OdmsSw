
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
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lbPercent = new System.Windows.Forms.Label();
            this.statusProgressBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbPercentSymbol
            // 
            this.lbPercentSymbol.AutoSize = true;
            this.lbPercentSymbol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbPercentSymbol.Location = new System.Drawing.Point(397, 88);
            this.lbPercentSymbol.Name = "lbPercentSymbol";
            this.lbPercentSymbol.Size = new System.Drawing.Size(21, 16);
            this.lbPercentSymbol.TabIndex = 279;
            this.lbPercentSymbol.Text = "%";
            // 
            // lbRemainTime
            // 
            this.lbRemainTime.AutoSize = true;
            this.lbRemainTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemainTime.Location = new System.Drawing.Point(124, 96);
            this.lbRemainTime.Name = "lbRemainTime";
            this.lbRemainTime.Size = new System.Drawing.Size(16, 15);
            this.lbRemainTime.TabIndex = 278;
            this.lbRemainTime.Text = "...";
            this.lbRemainTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 15);
            this.label6.TabIndex = 277;
            this.label6.Text = "Remain Time :";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(342, 113);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 42);
            this.btnOK.TabIndex = 276;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lbProcessTime
            // 
            this.lbProcessTime.AutoSize = true;
            this.lbProcessTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProcessTime.Location = new System.Drawing.Point(124, 142);
            this.lbProcessTime.Name = "lbProcessTime";
            this.lbProcessTime.Size = new System.Drawing.Size(16, 15);
            this.lbProcessTime.TabIndex = 275;
            this.lbProcessTime.Text = "...";
            this.lbProcessTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(41, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 15);
            this.label4.TabIndex = 274;
            this.label4.Text = "Elapsed :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbEndTime
            // 
            this.lbEndTime.AutoSize = true;
            this.lbEndTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEndTime.Location = new System.Drawing.Point(124, 119);
            this.lbEndTime.Name = "lbEndTime";
            this.lbEndTime.Size = new System.Drawing.Size(16, 15);
            this.lbEndTime.TabIndex = 273;
            this.lbEndTime.Text = "...";
            this.lbEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(33, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 15);
            this.label2.TabIndex = 272;
            this.label2.Text = "End Time  :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbStartTime
            // 
            this.lbStartTime.AutoSize = true;
            this.lbStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStartTime.Location = new System.Drawing.Point(124, 50);
            this.lbStartTime.Name = "lbStartTime";
            this.lbStartTime.Size = new System.Drawing.Size(16, 15);
            this.lbStartTime.TabIndex = 271;
            this.lbStartTime.Text = "...";
            this.lbStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label24
            // 
            this.Label24.AutoSize = true;
            this.Label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label24.Location = new System.Drawing.Point(31, 50);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(69, 15);
            this.Label24.TabIndex = 270;
            this.Label24.Text = "Start Time :";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbEstiEndTime
            // 
            this.lbEstiEndTime.AutoSize = true;
            this.lbEstiEndTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbEstiEndTime.Location = new System.Drawing.Point(124, 73);
            this.lbEstiEndTime.Name = "lbEstiEndTime";
            this.lbEstiEndTime.Size = new System.Drawing.Size(16, 15);
            this.lbEstiEndTime.TabIndex = 269;
            this.lbEstiEndTime.Text = "...";
            this.lbEstiEndTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(13, 73);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(89, 15);
            this.Label1.TabIndex = 268;
            this.Label1.Text = "Est. End Time :";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCurItemNo
            // 
            this.lbCurItemNo.AutoSize = true;
            this.lbCurItemNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbCurItemNo.ForeColor = System.Drawing.Color.MidnightBlue;
            this.lbCurItemNo.Location = new System.Drawing.Point(34, 13);
            this.lbCurItemNo.Name = "lbCurItemNo";
            this.lbCurItemNo.Size = new System.Drawing.Size(17, 16);
            this.lbCurItemNo.TabIndex = 267;
            this.lbCurItemNo.Text = "...";
            // 
            // txtMessage
            // 
            this.txtMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessage.Location = new System.Drawing.Point(272, 13);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(226, 66);
            this.txtMessage.TabIndex = 265;
            this.txtMessage.Text = "message!!";
            this.txtMessage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbPercent
            // 
            this.lbPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbPercent.Location = new System.Drawing.Point(367, 87);
            this.lbPercent.Name = "lbPercent";
            this.lbPercent.Size = new System.Drawing.Size(25, 16);
            this.lbPercent.TabIndex = 264;
            this.lbPercent.Text = "???";
            this.lbPercent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusProgressBar
            // 
            this.statusProgressBar.Location = new System.Drawing.Point(272, 113);
            this.statusProgressBar.Name = "statusProgressBar";
            this.statusProgressBar.Size = new System.Drawing.Size(226, 43);
            this.statusProgressBar.TabIndex = 263;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 166);
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
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.lbPercent);
            this.Controls.Add(this.statusProgressBar);
            this.Font = new System.Drawing.Font("Malgun Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.Label lbPercent;
    private System.Windows.Forms.ProgressBar statusProgressBar;
    private System.Windows.Forms.Timer timer1;
}
