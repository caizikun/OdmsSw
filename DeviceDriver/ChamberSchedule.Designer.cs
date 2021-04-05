namespace Neon.Aligner
{
	partial class ChamberSchedule
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

		#region 구성 요소 디자이너에서 생성한 코드

		/// <summary> 
		/// 디자이너 지원에 필요한 메서드입니다. 
		/// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.splitContainerMain = new System.Windows.Forms.SplitContainer();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.btnApplyCurrTime = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.btnLoadSchedule = new System.Windows.Forms.Button();
			this.txtTimeEnd = new System.Windows.Forms.TextBox();
			this.txtTimeStart = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.txtCount = new System.Windows.Forms.TextBox();
			this.txtTimeCurrent = new System.Windows.Forms.TextBox();
			this.gridScheduleTable = new System.Windows.Forms.DataGridView();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.Label39 = new System.Windows.Forms.Label();
			this.txtTimeRemain = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
			this.splitContainerMain.Panel1.SuspendLayout();
			this.splitContainerMain.Panel2.SuspendLayout();
			this.splitContainerMain.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridScheduleTable)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainerMain
			// 
			this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMain.IsSplitterFixed = true;
			this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.splitContainerMain.Name = "splitContainerMain";
			this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMain.Panel1
			// 
			this.splitContainerMain.Panel1.Controls.Add(this.groupBox1);
			// 
			// splitContainerMain.Panel2
			// 
			this.splitContainerMain.Panel2.Controls.Add(this.gridScheduleTable);
			this.splitContainerMain.Size = new System.Drawing.Size(800, 350);
			this.splitContainerMain.SplitterDistance = 91;
			this.splitContainerMain.SplitterWidth = 5;
			this.splitContainerMain.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.txtTimeRemain);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.txtTimeEnd);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.btnApplyCurrTime);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.btnLoadSchedule);
			this.groupBox1.Controls.Add(this.txtTimeStart);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.txtCount);
			this.groupBox1.Controls.Add(this.txtTimeCurrent);
			this.groupBox1.Controls.Add(this.Label39);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.groupBox1.Size = new System.Drawing.Size(800, 91);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Chamber Schedule";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label3.Location = new System.Drawing.Point(556, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 15);
			this.label3.TabIndex = 344;
			this.label3.Text = "Count :";
			// 
			// btnApplyCurrTime
			// 
			this.btnApplyCurrTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnApplyCurrTime.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnApplyCurrTime.Location = new System.Drawing.Point(662, 17);
			this.btnApplyCurrTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnApplyCurrTime.Name = "btnApplyCurrTime";
			this.btnApplyCurrTime.Size = new System.Drawing.Size(130, 36);
			this.btnApplyCurrTime.TabIndex = 176;
			this.btnApplyCurrTime.Text = "Reset Schedule";
			this.btnApplyCurrTime.UseVisualStyleBackColor = true;
			this.btnApplyCurrTime.Click += new System.EventHandler(this.btnApplyCurrTime_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label2.Location = new System.Drawing.Point(598, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 15);
			this.label2.TabIndex = 343;
			this.label2.Text = "종료 시간 :";
			// 
			// btnLoadSchedule
			// 
			this.btnLoadSchedule.AutoSize = true;
			this.btnLoadSchedule.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowFrame;
			this.btnLoadSchedule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnLoadSchedule.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.btnLoadSchedule.ForeColor = System.Drawing.Color.DodgerBlue;
			this.btnLoadSchedule.Location = new System.Drawing.Point(6, 17);
			this.btnLoadSchedule.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.btnLoadSchedule.Name = "btnLoadSchedule";
			this.btnLoadSchedule.Size = new System.Drawing.Size(535, 36);
			this.btnLoadSchedule.TabIndex = 175;
			this.btnLoadSchedule.Text = "Schedule File:";
			this.btnLoadSchedule.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnLoadSchedule.UseVisualStyleBackColor = true;
			this.btnLoadSchedule.Click += new System.EventHandler(this.btnLoadSchedule_Click);
			// 
			// txtTimeEnd
			// 
			this.txtTimeEnd.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTimeEnd.ForeColor = System.Drawing.Color.Tomato;
			this.txtTimeEnd.Location = new System.Drawing.Point(662, 57);
			this.txtTimeEnd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtTimeEnd.Name = "txtTimeEnd";
			this.txtTimeEnd.Size = new System.Drawing.Size(130, 29);
			this.txtTimeEnd.TabIndex = 342;
			this.txtTimeEnd.Text = "...";
			this.txtTimeEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtTimeStart
			// 
			this.txtTimeStart.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTimeStart.ForeColor = System.Drawing.Color.Tomato;
			this.txtTimeStart.Location = new System.Drawing.Point(467, 57);
			this.txtTimeStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtTimeStart.Name = "txtTimeStart";
			this.txtTimeStart.Size = new System.Drawing.Size(130, 29);
			this.txtTimeStart.TabIndex = 340;
			this.txtTimeStart.Text = "...";
			this.txtTimeStart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label1.Location = new System.Drawing.Point(403, 64);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 15);
			this.label1.TabIndex = 341;
			this.label1.Text = "시작 시간 :";
			// 
			// txtCount
			// 
			this.txtCount.BackColor = System.Drawing.SystemColors.MenuText;
			this.txtCount.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtCount.ForeColor = System.Drawing.Color.Lime;
			this.txtCount.Location = new System.Drawing.Point(609, 20);
			this.txtCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtCount.Name = "txtCount";
			this.txtCount.Size = new System.Drawing.Size(37, 29);
			this.txtCount.TabIndex = 177;
			this.txtCount.Text = "1";
			this.txtCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// txtTimeCurrent
			// 
			this.txtTimeCurrent.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTimeCurrent.ForeColor = System.Drawing.Color.Maroon;
			this.txtTimeCurrent.Location = new System.Drawing.Point(68, 57);
			this.txtTimeCurrent.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtTimeCurrent.Name = "txtTimeCurrent";
			this.txtTimeCurrent.Size = new System.Drawing.Size(130, 29);
			this.txtTimeCurrent.TabIndex = 338;
			this.txtTimeCurrent.Text = "...";
			this.txtTimeCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// gridScheduleTable
			// 
			this.gridScheduleTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridScheduleTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridScheduleTable.Location = new System.Drawing.Point(0, 0);
			this.gridScheduleTable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.gridScheduleTable.Name = "gridScheduleTable";
			this.gridScheduleTable.RowTemplate.Height = 23;
			this.gridScheduleTable.Size = new System.Drawing.Size(800, 254);
			this.gridScheduleTable.TabIndex = 0;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 1000;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Label39
			// 
			this.Label39.AutoSize = true;
			this.Label39.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Label39.Location = new System.Drawing.Point(4, 64);
			this.Label39.Name = "Label39";
			this.Label39.Size = new System.Drawing.Size(66, 15);
			this.Label39.TabIndex = 339;
			this.Label39.Text = "현재 시간 :";
			// 
			// txtTimeRemain
			// 
			this.txtTimeRemain.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtTimeRemain.ForeColor = System.Drawing.Color.Maroon;
			this.txtTimeRemain.Location = new System.Drawing.Point(264, 57);
			this.txtTimeRemain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.txtTimeRemain.Name = "txtTimeRemain";
			this.txtTimeRemain.Size = new System.Drawing.Size(130, 29);
			this.txtTimeRemain.TabIndex = 345;
			this.txtTimeRemain.Text = "...";
			this.txtTimeRemain.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.label4.Location = new System.Drawing.Point(200, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(66, 15);
			this.label4.TabIndex = 346;
			this.label4.Text = "남은 시간 :";
			// 
			// ChamberSchedule
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerMain);
			this.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "ChamberSchedule";
			this.Size = new System.Drawing.Size(800, 350);
			this.splitContainerMain.Panel1.ResumeLayout(false);
			this.splitContainerMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
			this.splitContainerMain.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridScheduleTable)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainerMain;
		private System.Windows.Forms.DataGridView gridScheduleTable;
		private System.Windows.Forms.Button btnLoadSchedule;
		private System.Windows.Forms.Button btnApplyCurrTime;
		internal System.Windows.Forms.TextBox txtCount;
		internal System.Windows.Forms.TextBox txtTimeCurrent;
		internal System.Windows.Forms.Label label2;
		internal System.Windows.Forms.TextBox txtTimeEnd;
		internal System.Windows.Forms.Label label1;
		internal System.Windows.Forms.TextBox txtTimeStart;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Timer timer1;
		internal System.Windows.Forms.Label label3;
		internal System.Windows.Forms.Label Label39;
		internal System.Windows.Forms.TextBox txtTimeRemain;
		internal System.Windows.Forms.Label label4;
	}
}
