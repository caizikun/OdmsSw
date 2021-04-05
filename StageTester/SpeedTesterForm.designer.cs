namespace Free302.TnM.Device.StageTester
{
	partial class SpeedTesterForm
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
			if(disposing && (components != null))
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
			this.menuMain = new System.Windows.Forms.MenuStrip();
			this.menuReload = new System.Windows.Forms.ToolStripMenuItem();
			this.menuApply = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSaveData = new System.Windows.Forms.ToolStripMenuItem();
			this.gridConfig = new System.Windows.Forms.DataGridView();
			this.menuMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridConfig)).BeginInit();
			this.SuspendLayout();
			// 
			// menuMain
			// 
			this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuReload,
            this.menuApply,
            this.menuSaveData});
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(787, 28);
			this.menuMain.TabIndex = 0;
			this.menuMain.Text = "menuStrip1";
			// 
			// menuReload
			// 
			this.menuReload.Name = "menuReload";
			this.menuReload.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.menuReload.Size = new System.Drawing.Size(88, 24);
			this.menuReload.Text = "Reload F&1";
			// 
			// menuApply
			// 
			this.menuApply.Name = "menuApply";
			this.menuApply.ShortcutKeys = System.Windows.Forms.Keys.F2;
			this.menuApply.Size = new System.Drawing.Size(80, 24);
			this.menuApply.Text = "Apply F&2";
			// 
			// menuSaveData
			// 
			this.menuSaveData.Name = "menuSaveData";
			this.menuSaveData.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.menuSaveData.Size = new System.Drawing.Size(132, 24);
			this.menuSaveData.Text = "Save To File (F&3)";
			// 
			// gridConfig
			// 
			this.gridConfig.Location = new System.Drawing.Point(32, 71);
			this.gridConfig.Name = "gridConfig";
			this.gridConfig.RowTemplate.Height = 23;
			this.gridConfig.Size = new System.Drawing.Size(151, 75);
			this.gridConfig.TabIndex = 1;
			// 
			// StageTesterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(787, 419);
			this.Controls.Add(this.gridConfig);
			this.Controls.Add(this.menuMain);
			this.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
			this.MainMenuStrip = this.menuMain;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "StageTesterForm";
			this.Text = "StageTester";
			this.menuMain.ResumeLayout(false);
			this.menuMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridConfig)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuMain;
		private System.Windows.Forms.ToolStripMenuItem menuReload;
		private System.Windows.Forms.ToolStripMenuItem menuApply;
		private System.Windows.Forms.DataGridView gridConfig;
		private System.Windows.Forms.ToolStripMenuItem menuSaveData;
	}
}