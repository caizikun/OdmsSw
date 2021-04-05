namespace Free302.TnM.Device.StageTester
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
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolMain = new System.Windows.Forms.ToolStrip();
            this.toolAligner = new System.Windows.Forms.ToolStripComboBox();
            this.toolOpen = new System.Windows.Forms.ToolStripButton();
            this.toolTest = new System.Windows.Forms.ToolStripButton();
            this.toolQuit = new System.Windows.Forms.ToolStripButton();
            this.toolClose = new System.Windows.Forms.ToolStripButton();
            this.toolSwInfo = new System.Windows.Forms.ToolStripButton();
            this.umPosition = new System.Windows.Forms.ToolStripTextBox();
            this.toolStop = new System.Windows.Forms.ToolStripButton();
            this.toolGoZero = new System.Windows.Forms.ToolStripButton();
            this.toolHome = new System.Windows.Forms.ToolStripButton();
            this.toolReport = new System.Windows.Forms.ToolStripButton();
            this.toolTestForm = new System.Windows.Forms.ToolStripButton();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.splitGrid = new System.Windows.Forms.SplitContainer();
            this.splitGridTop = new System.Windows.Forms.SplitContainer();
            this.gridStatus = new System.Windows.Forms.DataGridView();
            this.gridConfig = new System.Windows.Forms.DataGridView();
            this.textLog = new System.Windows.Forms.TextBox();
            this.toolMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid)).BeginInit();
            this.splitGrid.Panel1.SuspendLayout();
            this.splitGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGridTop)).BeginInit();
            this.splitGridTop.Panel1.SuspendLayout();
            this.splitGridTop.Panel2.SuspendLayout();
            this.splitGridTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).BeginInit();
            this.SuspendLayout();
            // 
            // toolMain
            // 
            this.toolMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAligner,
            this.toolOpen,
            this.toolTest,
            this.toolQuit,
            this.toolClose,
            this.toolSwInfo,
            this.umPosition,
            this.toolStop,
            this.toolGoZero,
            this.toolHome,
            this.toolReport,
            this.toolTestForm});
            this.toolMain.Location = new System.Drawing.Point(0, 0);
            this.toolMain.Name = "toolMain";
            this.toolMain.Size = new System.Drawing.Size(756, 39);
            this.toolMain.TabIndex = 0;
            this.toolMain.Text = "toolStrip1";
            // 
            // toolAligner
            // 
            this.toolAligner.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolAligner.Name = "toolAligner";
            this.toolAligner.Size = new System.Drawing.Size(80, 39);
            // 
            // toolOpen
            // 
            this.toolOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolOpen.Image")));
            this.toolOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolOpen.Name = "toolOpen";
            this.toolOpen.Size = new System.Drawing.Size(40, 36);
            this.toolOpen.Text = "Open";
            // 
            // toolTest
            // 
            this.toolTest.Image = ((System.Drawing.Image)(resources.GetObject("toolTest.Image")));
            this.toolTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTest.Name = "toolTest";
            this.toolTest.Size = new System.Drawing.Size(64, 36);
            this.toolTest.Text = "Test";
            // 
            // toolQuit
            // 
            this.toolQuit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolQuit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolQuit.Image = ((System.Drawing.Image)(resources.GetObject("toolQuit.Image")));
            this.toolQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolQuit.Name = "toolQuit";
            this.toolQuit.Size = new System.Drawing.Size(36, 36);
            this.toolQuit.Text = "Quit";
            // 
            // toolClose
            // 
            this.toolClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolClose.Image = ((System.Drawing.Image)(resources.GetObject("toolClose.Image")));
            this.toolClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolClose.Name = "toolClose";
            this.toolClose.Size = new System.Drawing.Size(40, 36);
            this.toolClose.Text = "Close";
            // 
            // toolSwInfo
            // 
            this.toolSwInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolSwInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSwInfo.Image = ((System.Drawing.Image)(resources.GetObject("toolSwInfo.Image")));
            this.toolSwInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSwInfo.Name = "toolSwInfo";
            this.toolSwInfo.Size = new System.Drawing.Size(36, 36);
            this.toolSwInfo.Text = "Info";
            // 
            // umPosition
            // 
            this.umPosition.AutoSize = false;
            this.umPosition.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.umPosition.ForeColor = System.Drawing.Color.ForestGreen;
            this.umPosition.Name = "umPosition";
            this.umPosition.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.umPosition.Size = new System.Drawing.Size(79, 26);
            this.umPosition.Text = "0";
            this.umPosition.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolStop
            // 
            this.toolStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStop.Image")));
            this.toolStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStop.Name = "toolStop";
            this.toolStop.Size = new System.Drawing.Size(36, 36);
            this.toolStop.Text = "Stop";
            // 
            // toolGoZero
            // 
            this.toolGoZero.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolGoZero.Image = ((System.Drawing.Image)(resources.GetObject("toolGoZero.Image")));
            this.toolGoZero.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolGoZero.Name = "toolGoZero";
            this.toolGoZero.Size = new System.Drawing.Size(35, 36);
            this.toolGoZero.Text = "Zero";
            // 
            // toolHome
            // 
            this.toolHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolHome.Image = ((System.Drawing.Image)(resources.GetObject("toolHome.Image")));
            this.toolHome.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolHome.Name = "toolHome";
            this.toolHome.Size = new System.Drawing.Size(44, 36);
            this.toolHome.Text = "Home";
            // 
            // toolReport
            // 
            this.toolReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolReport.Image = ((System.Drawing.Image)(resources.GetObject("toolReport.Image")));
            this.toolReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolReport.Name = "toolReport";
            this.toolReport.Size = new System.Drawing.Size(46, 36);
            this.toolReport.Text = "Report";
            // 
            // toolTestForm
            // 
            this.toolTestForm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolTestForm.Image = ((System.Drawing.Image)(resources.GetObject("toolTestForm.Image")));
            this.toolTestForm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTestForm.Name = "toolTestForm";
            this.toolTestForm.Size = new System.Drawing.Size(68, 36);
            this.toolTestForm.Text = "Speed설정";
            // 
            // splitMain
            // 
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(32, 81);
            this.splitMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.splitGrid);
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.textLog);
            this.splitMain.Size = new System.Drawing.Size(358, 442);
            this.splitMain.SplitterDistance = 318;
            this.splitMain.SplitterIncrement = 2;
            this.splitMain.SplitterWidth = 2;
            this.splitMain.TabIndex = 3;
            // 
            // splitGrid
            // 
            this.splitGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGrid.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitGrid.Location = new System.Drawing.Point(0, 0);
            this.splitGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitGrid.Name = "splitGrid";
            this.splitGrid.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitGrid.Panel1
            // 
            this.splitGrid.Panel1.Controls.Add(this.splitGridTop);
            this.splitGrid.Size = new System.Drawing.Size(358, 318);
            this.splitGrid.SplitterDistance = 167;
            this.splitGrid.SplitterIncrement = 2;
            this.splitGrid.SplitterWidth = 2;
            this.splitGrid.TabIndex = 0;
            // 
            // splitGridTop
            // 
            this.splitGridTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGridTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitGridTop.Location = new System.Drawing.Point(0, 0);
            this.splitGridTop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitGridTop.Name = "splitGridTop";
            // 
            // splitGridTop.Panel1
            // 
            this.splitGridTop.Panel1.Controls.Add(this.gridStatus);
            // 
            // splitGridTop.Panel2
            // 
            this.splitGridTop.Panel2.Controls.Add(this.gridConfig);
            this.splitGridTop.Size = new System.Drawing.Size(358, 167);
            this.splitGridTop.SplitterDistance = 104;
            this.splitGridTop.SplitterIncrement = 2;
            this.splitGridTop.SplitterWidth = 2;
            this.splitGridTop.TabIndex = 0;
            // 
            // gridStatus
            // 
            this.gridStatus.AllowUserToAddRows = false;
            this.gridStatus.AllowUserToDeleteRows = false;
            this.gridStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridStatus.Location = new System.Drawing.Point(10, 20);
            this.gridStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridStatus.Name = "gridStatus";
            this.gridStatus.RowTemplate.Height = 23;
            this.gridStatus.Size = new System.Drawing.Size(53, 56);
            this.gridStatus.TabIndex = 0;
            // 
            // gridConfig
            // 
            this.gridConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridConfig.Location = new System.Drawing.Point(42, 20);
            this.gridConfig.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.gridConfig.Name = "gridConfig";
            this.gridConfig.RowTemplate.Height = 23;
            this.gridConfig.Size = new System.Drawing.Size(52, 56);
            this.gridConfig.TabIndex = 1;
            // 
            // textLog
            // 
            this.textLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textLog.Location = new System.Drawing.Point(22, 26);
            this.textLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textLog.Multiline = true;
            this.textLog.Name = "textLog";
            this.textLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textLog.Size = new System.Drawing.Size(101, 28);
            this.textLog.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 554);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.toolMain);
            this.DoubleBuffered = true;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.toolMain.ResumeLayout(false);
            this.toolMain.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.splitGrid.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid)).EndInit();
            this.splitGrid.ResumeLayout(false);
            this.splitGridTop.Panel1.ResumeLayout(false);
            this.splitGridTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGridTop)).EndInit();
            this.splitGridTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridConfig)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolMain;
        private System.Windows.Forms.ToolStripButton toolOpen;
        private System.Windows.Forms.ToolStripButton toolTest;
        private System.Windows.Forms.ToolStripButton toolQuit;
        private System.Windows.Forms.ToolStripButton toolClose;
        private System.Windows.Forms.ToolStripButton toolSwInfo;
        private System.Windows.Forms.ToolStripTextBox umPosition;
        private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.TextBox textLog;
        private System.Windows.Forms.DataGridView gridStatus;
		private System.Windows.Forms.DataGridView gridConfig;
		private System.Windows.Forms.ToolStripButton toolStop;
		private System.Windows.Forms.ToolStripButton toolGoZero;
		private System.Windows.Forms.ToolStripButton toolHome;
		private System.Windows.Forms.ToolStripButton toolReport;
		private System.Windows.Forms.SplitContainer splitGrid;
		private System.Windows.Forms.SplitContainer splitGridTop;
		private System.Windows.Forms.ToolStripComboBox toolAligner;
		private System.Windows.Forms.ToolStripButton toolTestForm;
	}
}

