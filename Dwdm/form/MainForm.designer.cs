
partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MNU_FILE = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_FILE_EXIT = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFile_initDoNotAsk = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_STAGECONTROL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_ALIGNMENT = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_SOURCECONTROL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_DIGIOPTPWR = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_DISTSENSVIEWER = new System.Windows.Forms.ToolStripMenuItem();
            this.uiMenuUvCure = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMeasure = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMeasureReference = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMeasureDut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.uiMenuScan = new System.Windows.Forms.ToolStripMenuItem();
            this.uiMenuScanMonitorPort = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.uiMenuChamber = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_HELP = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_HELP_ABOUT = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPcTest = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTlsStateTest = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTestForm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMcuTest = new System.Windows.Forms.ToolStripMenuItem();
            this.tss = new System.Windows.Forms.StatusStrip();
            this.tsslbWavelen = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslbSource = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslbPC = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.MenuStrip1.SuspendLayout();
            this.tss.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_FILE,
            this.MNU_TOOL,
            this.menuMeasure,
            this.MNU_HELP});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(686, 25);
            this.MenuStrip1.TabIndex = 5;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // MNU_FILE
            // 
            this.MNU_FILE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_FILE_EXIT,
            this.menuFile_initDoNotAsk});
            this.MNU_FILE.Name = "MNU_FILE";
            this.MNU_FILE.Size = new System.Drawing.Size(53, 21);
            this.MNU_FILE.Text = "File(&F)";
            // 
            // MNU_FILE_EXIT
            // 
            this.MNU_FILE_EXIT.Name = "MNU_FILE_EXIT";
            this.MNU_FILE_EXIT.Size = new System.Drawing.Size(216, 22);
            this.MNU_FILE_EXIT.Text = "Exit(&X)";
            this.MNU_FILE_EXIT.Click += new System.EventHandler(this.MNU_FILE_EXIT_Click);
            // 
            // menuFile_initDoNotAsk
            // 
            this.menuFile_initDoNotAsk.Checked = global::Neon.Dwdm.Properties.Settings.Default.doInitNotAsk;
            this.menuFile_initDoNotAsk.CheckOnClick = true;
            this.menuFile_initDoNotAsk.Name = "menuFile_initDoNotAsk";
            this.menuFile_initDoNotAsk.Size = new System.Drawing.Size(216, 22);
            this.menuFile_initDoNotAsk.Text = "시작시 자동초기화 수행";
            // 
            // MNU_TOOL
            // 
            this.MNU_TOOL.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_TOOL_STAGECONTROL,
            this.MNU_TOOL_ALIGNMENT,
            this.MNU_TOOL_SOURCECONTROL,
            this.MNU_TOOL_DIGIOPTPWR,
            this.MNU_TOOL_DISTSENSVIEWER,
            this.uiMenuUvCure});
            this.MNU_TOOL.Name = "MNU_TOOL";
            this.MNU_TOOL.Size = new System.Drawing.Size(67, 21);
            this.MNU_TOOL.Text = "Tools(&T)";
            // 
            // MNU_TOOL_STAGECONTROL
            // 
            this.MNU_TOOL_STAGECONTROL.Name = "MNU_TOOL_STAGECONTROL";
            this.MNU_TOOL_STAGECONTROL.Size = new System.Drawing.Size(217, 22);
            this.MNU_TOOL_STAGECONTROL.Text = "Stage Controller";
            this.MNU_TOOL_STAGECONTROL.Click += new System.EventHandler(this.MNU_TOOL_STAGECONTROL_Click);
            // 
            // MNU_TOOL_ALIGNMENT
            // 
            this.MNU_TOOL_ALIGNMENT.Name = "MNU_TOOL_ALIGNMENT";
            this.MNU_TOOL_ALIGNMENT.Size = new System.Drawing.Size(217, 22);
            this.MNU_TOOL_ALIGNMENT.Text = "Alignment";
            this.MNU_TOOL_ALIGNMENT.Click += new System.EventHandler(this.MNU_TOOL_ALIGNMENT_Click);
            // 
            // MNU_TOOL_SOURCECONTROL
            // 
            this.MNU_TOOL_SOURCECONTROL.Name = "MNU_TOOL_SOURCECONTROL";
            this.MNU_TOOL_SOURCECONTROL.Size = new System.Drawing.Size(217, 22);
            this.MNU_TOOL_SOURCECONTROL.Text = "Source Controller";
            this.MNU_TOOL_SOURCECONTROL.Click += new System.EventHandler(this.MNU_TOOL_SOURCECONTROL_Click);
            // 
            // MNU_TOOL_DIGIOPTPWR
            // 
            this.MNU_TOOL_DIGIOPTPWR.Name = "MNU_TOOL_DIGIOPTPWR";
            this.MNU_TOOL_DIGIOPTPWR.Size = new System.Drawing.Size(217, 22);
            this.MNU_TOOL_DIGIOPTPWR.Text = "Opt. Powermeter";
            this.MNU_TOOL_DIGIOPTPWR.Click += new System.EventHandler(this.MNU_TOOL_DIGIOPTPWR_Click);
            // 
            // MNU_TOOL_DISTSENSVIEWER
            // 
            this.MNU_TOOL_DISTSENSVIEWER.Name = "MNU_TOOL_DISTSENSVIEWER";
            this.MNU_TOOL_DISTSENSVIEWER.Size = new System.Drawing.Size(217, 22);
            this.MNU_TOOL_DISTSENSVIEWER.Text = "Distance Sensor Viewer";
            this.MNU_TOOL_DISTSENSVIEWER.Click += new System.EventHandler(this.MNU_TOOL_DISTSENSVIEWER_Click);
            // 
            // uiMenuUvCure
            // 
            this.uiMenuUvCure.Name = "uiMenuUvCure";
            this.uiMenuUvCure.Size = new System.Drawing.Size(217, 22);
            this.uiMenuUvCure.Text = "Uv Cure";
            this.uiMenuUvCure.Click += new System.EventHandler(this.uiMenuUvCure_Click);
            // 
            // menuMeasure
            // 
            this.menuMeasure.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMeasureReference,
            this.menuMeasureDut,
            this.toolStripSeparator2,
            this.uiMenuScan,
            this.uiMenuScanMonitorPort,
            this.toolStripSeparator1,
            this.uiMenuChamber});
            this.menuMeasure.Name = "menuMeasure";
            this.menuMeasure.Size = new System.Drawing.Size(92, 21);
            this.menuMeasure.Text = "Measure(&M)";
            // 
            // menuMeasureReference
            // 
            this.menuMeasureReference.Name = "menuMeasureReference";
            this.menuMeasureReference.Size = new System.Drawing.Size(186, 22);
            this.menuMeasureReference.Text = "Reference ";
            this.menuMeasureReference.Click += new System.EventHandler(this.referenceNonPolarizationToolStripMenuItem_Click);
            // 
            // menuMeasureDut
            // 
            this.menuMeasureDut.Name = "menuMeasureDut";
            this.menuMeasureDut.Size = new System.Drawing.Size(186, 22);
            this.menuMeasureDut.Text = "DUT";
            this.menuMeasureDut.Click += new System.EventHandler(this.MNU_MSR_CWDMMUXFAPOL_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
            // 
            // uiMenuScan
            // 
            this.uiMenuScan.Name = "uiMenuScan";
            this.uiMenuScan.Size = new System.Drawing.Size(186, 22);
            this.uiMenuScan.Text = "Scan";
            this.uiMenuScan.Click += new System.EventHandler(this.uiMenuScan_Click);
            // 
            // uiMenuScanMonitorPort
            // 
            this.uiMenuScanMonitorPort.Name = "uiMenuScanMonitorPort";
            this.uiMenuScanMonitorPort.Size = new System.Drawing.Size(186, 22);
            this.uiMenuScanMonitorPort.Text = "Scan Monitor Port";
            this.uiMenuScanMonitorPort.Click += new System.EventHandler(this.uiMenuScanMonitorPort_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // uiMenuChamber
            // 
            this.uiMenuChamber.Name = "uiMenuChamber";
            this.uiMenuChamber.Size = new System.Drawing.Size(186, 22);
            this.uiMenuChamber.Text = "Chamber";
            this.uiMenuChamber.Click += new System.EventHandler(this.uiMenuChamber_Click);
            // 
            // MNU_HELP
            // 
            this.MNU_HELP.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_HELP_ABOUT,
            this.menuPcTest,
            this.menuTlsStateTest,
            this.menuTestForm,
            this.menuMcuTest});
            this.MNU_HELP.Name = "MNU_HELP";
            this.MNU_HELP.Size = new System.Drawing.Size(64, 21);
            this.MNU_HELP.Text = "Help(&H)";
            // 
            // MNU_HELP_ABOUT
            // 
            this.MNU_HELP_ABOUT.Name = "MNU_HELP_ABOUT";
            this.MNU_HELP_ABOUT.Size = new System.Drawing.Size(158, 22);
            this.MNU_HELP_ABOUT.Text = "About";
            this.MNU_HELP_ABOUT.Click += new System.EventHandler(this.MNU_HELP_ABOUT_Click);
            // 
            // menuPcTest
            // 
            this.menuPcTest.Name = "menuPcTest";
            this.menuPcTest.Size = new System.Drawing.Size(158, 22);
            this.menuPcTest.Text = "PcStabilityTest";
            // 
            // menuTlsStateTest
            // 
            this.menuTlsStateTest.Name = "menuTlsStateTest";
            this.menuTlsStateTest.Size = new System.Drawing.Size(158, 22);
            this.menuTlsStateTest.Text = "TlsStateTest";
            this.menuTlsStateTest.Click += new System.EventHandler(this.menuTlsStateTest_Click);
            // 
            // menuTestForm
            // 
            this.menuTestForm.Name = "menuTestForm";
            this.menuTestForm.Size = new System.Drawing.Size(158, 22);
            this.menuTestForm.Text = "TestForm";
            this.menuTestForm.Click += new System.EventHandler(this.menuTestForm_Click);
            // 
            // menuMcuTest
            // 
            this.menuMcuTest.Name = "menuMcuTest";
            this.menuMcuTest.Size = new System.Drawing.Size(158, 22);
            this.menuMcuTest.Text = "MCU Test";
            this.menuMcuTest.Click += new System.EventHandler(this.menuMcuTest_Click);
            // 
            // tss
            // 
            this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbWavelen,
            this.tsslbSource,
            this.tsslbPC});
            this.tss.Location = new System.Drawing.Point(0, 243);
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(686, 34);
            this.tss.TabIndex = 287;
            this.tss.Text = "StatusStrip1";
            // 
            // tsslbWavelen
            // 
            this.tsslbWavelen.AutoSize = false;
            this.tsslbWavelen.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbWavelen.Name = "tsslbWavelen";
            this.tsslbWavelen.Size = new System.Drawing.Size(120, 29);
            this.tsslbWavelen.Text = "0.0";
            // 
            // tsslbSource
            // 
            this.tsslbSource.AutoSize = false;
            this.tsslbSource.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbSource.Name = "tsslbSource";
            this.tsslbSource.Size = new System.Drawing.Size(120, 29);
            this.tsslbSource.Text = "TLS";
            // 
            // tsslbPC
            // 
            this.tsslbPC.AutoSize = false;
            this.tsslbPC.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbPC.Name = "tsslbPC";
            this.tsslbPC.Size = new System.Drawing.Size(120, 29);
            this.tsslbPC.Text = "TLS";
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(686, 277);
            this.Controls.Add(this.tss);
            this.Controls.Add(this.MenuStrip1);
            this.Font = new System.Drawing.Font("Malgun Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.MenuStrip1.ResumeLayout(false);
            this.MenuStrip1.PerformLayout();
            this.tss.ResumeLayout(false);
            this.tss.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    internal System.Windows.Forms.MenuStrip MenuStrip1;
    internal System.Windows.Forms.ToolStripMenuItem MNU_FILE;
    internal System.Windows.Forms.ToolStripMenuItem MNU_FILE_EXIT;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_STAGECONTROL;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_ALIGNMENT;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_DIGIOPTPWR;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_DISTSENSVIEWER;
    internal System.Windows.Forms.ToolStripMenuItem menuMeasure;
    internal System.Windows.Forms.ToolStripMenuItem MNU_HELP;
    internal System.Windows.Forms.ToolStripMenuItem MNU_HELP_ABOUT;
    private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_SOURCECONTROL;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbWavelen;
    private System.Windows.Forms.Timer timer1;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbSource;
    private System.Windows.Forms.ToolStripMenuItem menuMeasureReference;
    private System.Windows.Forms.ToolStripMenuItem menuMeasureDut;
    private System.Windows.Forms.ToolStripMenuItem menuFile_initDoNotAsk;
    private System.Windows.Forms.ToolStripMenuItem menuPcTest;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbPC;
	private System.Windows.Forms.ToolStripMenuItem menuTlsStateTest;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem uiMenuScan;
	private System.Windows.Forms.ToolStripMenuItem menuTestForm;
	private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
	private System.Windows.Forms.ToolStripMenuItem uiMenuChamber;
	private System.Windows.Forms.ToolStripMenuItem uiMenuScanMonitorPort;
	private System.Windows.Forms.ToolStripMenuItem uiMenuUvCure;
    private System.Windows.Forms.ToolStripMenuItem menuMcuTest;
}

