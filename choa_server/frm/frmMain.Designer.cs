
partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.MenuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MNU_FILE = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_FILE_EXIT = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_INIT = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_INITRES = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.MNU_TOOL_STAGECONTROL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_STAGEPOSITIONER = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_ALIGNMENT = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_SOURCECONTROL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_DIGIOPTPWR = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_DISTSENSVIEWER = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_AIRVALVECONTROL = new System.Windows.Forms.ToolStripMenuItem();
            this.uiMenuToolsUv = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.MNU_TOOL_LAMBDACAL = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_STAGETESTER = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_TOOL_PdScan = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_MSR = new System.Windows.Forms.ToolStripMenuItem();
            this.menurReference = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDutBud = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.MNU_MSR_LAPD = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDataViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.MNU_HELP = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTest = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSystemManual = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOperationManal = new System.Windows.Forms.ToolStripMenuItem();
            this.tss = new System.Windows.Forms.StatusStrip();
            this.tsslbWavelen = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslbSource = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.MenuStrip1.SuspendLayout();
            this.tss.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip1
            // 
            this.MenuStrip1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_FILE,
            this.MNU_TOOL,
            this.MNU_MSR,
            this.MNU_HELP});
            this.MenuStrip1.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip1.Name = "MenuStrip1";
            this.MenuStrip1.Size = new System.Drawing.Size(544, 24);
            this.MenuStrip1.TabIndex = 5;
            this.MenuStrip1.Text = "MenuStrip1";
            // 
            // MNU_FILE
            // 
            this.MNU_FILE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_FILE_EXIT});
            this.MNU_FILE.Name = "MNU_FILE";
            this.MNU_FILE.Size = new System.Drawing.Size(53, 20);
            this.MNU_FILE.Text = "File(&F)";
            // 
            // MNU_FILE_EXIT
            // 
            this.MNU_FILE_EXIT.Name = "MNU_FILE_EXIT";
            this.MNU_FILE_EXIT.Size = new System.Drawing.Size(109, 22);
            this.MNU_FILE_EXIT.Text = "Exit(&X)";
            this.MNU_FILE_EXIT.Click += new System.EventHandler(this.MNU_FILE_EXIT_Click);
            // 
            // MNU_TOOL
            // 
            this.MNU_TOOL.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MNU_TOOL_INIT,
            this.MNU_TOOL_INITRES,
            this.ToolStripSeparator3,
            this.MNU_TOOL_STAGECONTROL,
            this.MNU_TOOL_STAGEPOSITIONER,
            this.MNU_TOOL_ALIGNMENT,
            this.MNU_TOOL_SOURCECONTROL,
            this.MNU_TOOL_DIGIOPTPWR,
            this.MNU_TOOL_DISTSENSVIEWER,
            this.MNU_TOOL_AIRVALVECONTROL,
            this.uiMenuToolsUv,
            this.ToolStripSeparator2,
            this.MNU_TOOL_LAMBDACAL,
            this.MNU_TOOL_STAGETESTER,
            this.MNU_TOOL_PdScan});
            this.MNU_TOOL.Name = "MNU_TOOL";
            this.MNU_TOOL.Size = new System.Drawing.Size(62, 20);
            this.MNU_TOOL.Text = "Tools(&T)";
            // 
            // MNU_TOOL_INIT
            // 
            this.MNU_TOOL_INIT.Name = "MNU_TOOL_INIT";
            this.MNU_TOOL_INIT.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_INIT.Text = "Initalization";
            this.MNU_TOOL_INIT.Visible = false;
            // 
            // MNU_TOOL_INITRES
            // 
            this.MNU_TOOL_INITRES.Enabled = false;
            this.MNU_TOOL_INITRES.Name = "MNU_TOOL_INITRES";
            this.MNU_TOOL_INITRES.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_INITRES.Text = "Initalization Result";
            this.MNU_TOOL_INITRES.Visible = false;
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(199, 6);
            // 
            // MNU_TOOL_STAGECONTROL
            // 
            this.MNU_TOOL_STAGECONTROL.Name = "MNU_TOOL_STAGECONTROL";
            this.MNU_TOOL_STAGECONTROL.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_STAGECONTROL.Text = "Stage Controller";
            this.MNU_TOOL_STAGECONTROL.Click += new System.EventHandler(this.MNU_TOOL_STAGECONTROL_Click);
            // 
            // MNU_TOOL_STAGEPOSITIONER
            // 
            this.MNU_TOOL_STAGEPOSITIONER.Name = "MNU_TOOL_STAGEPOSITIONER";
            this.MNU_TOOL_STAGEPOSITIONER.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_STAGEPOSITIONER.Text = "Stage Positioner";
            this.MNU_TOOL_STAGEPOSITIONER.Visible = false;
            this.MNU_TOOL_STAGEPOSITIONER.Click += new System.EventHandler(this.MNU_TOOL_STAGEPOSITIONER_Click);
            // 
            // MNU_TOOL_ALIGNMENT
            // 
            this.MNU_TOOL_ALIGNMENT.Name = "MNU_TOOL_ALIGNMENT";
            this.MNU_TOOL_ALIGNMENT.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_ALIGNMENT.Text = "Alignment";
            this.MNU_TOOL_ALIGNMENT.Click += new System.EventHandler(this.MNU_TOOL_ALIGNMENT_Click);
            // 
            // MNU_TOOL_SOURCECONTROL
            // 
            this.MNU_TOOL_SOURCECONTROL.Name = "MNU_TOOL_SOURCECONTROL";
            this.MNU_TOOL_SOURCECONTROL.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_SOURCECONTROL.Text = "Source Controller";
            this.MNU_TOOL_SOURCECONTROL.Click += new System.EventHandler(this.MNU_TOOL_SOURCECONTROL_Click);
            // 
            // MNU_TOOL_DIGIOPTPWR
            // 
            this.MNU_TOOL_DIGIOPTPWR.Name = "MNU_TOOL_DIGIOPTPWR";
            this.MNU_TOOL_DIGIOPTPWR.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_DIGIOPTPWR.Text = "Opt. Powermeter";
            this.MNU_TOOL_DIGIOPTPWR.Click += new System.EventHandler(this.MNU_TOOL_DIGIOPTPWR_Click);
            // 
            // MNU_TOOL_DISTSENSVIEWER
            // 
            this.MNU_TOOL_DISTSENSVIEWER.Name = "MNU_TOOL_DISTSENSVIEWER";
            this.MNU_TOOL_DISTSENSVIEWER.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_DISTSENSVIEWER.Text = "Distance Sensor Viewer";
            this.MNU_TOOL_DISTSENSVIEWER.Click += new System.EventHandler(this.MNU_TOOL_DISTSENSVIEWER_Click);
            // 
            // MNU_TOOL_AIRVALVECONTROL
            // 
            this.MNU_TOOL_AIRVALVECONTROL.Name = "MNU_TOOL_AIRVALVECONTROL";
            this.MNU_TOOL_AIRVALVECONTROL.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_AIRVALVECONTROL.Text = "AirValve Controller";
            this.MNU_TOOL_AIRVALVECONTROL.Visible = false;
            this.MNU_TOOL_AIRVALVECONTROL.Click += new System.EventHandler(this.MNU_TOOL_AIRVALVECONTROL_Click);
            // 
            // uiMenuToolsUv
            // 
            this.uiMenuToolsUv.Name = "uiMenuToolsUv";
            this.uiMenuToolsUv.Size = new System.Drawing.Size(202, 22);
            this.uiMenuToolsUv.Text = "UV Curing";
            this.uiMenuToolsUv.Click += new System.EventHandler(this.uiMenuToolsUv_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(199, 6);
            // 
            // MNU_TOOL_LAMBDACAL
            // 
            this.MNU_TOOL_LAMBDACAL.Enabled = false;
            this.MNU_TOOL_LAMBDACAL.Name = "MNU_TOOL_LAMBDACAL";
            this.MNU_TOOL_LAMBDACAL.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_LAMBDACAL.Text = "TLS Calibration";
            this.MNU_TOOL_LAMBDACAL.Visible = false;
            this.MNU_TOOL_LAMBDACAL.Click += new System.EventHandler(this.MNU_TOOL_LAMBDACAL_Click);
            // 
            // MNU_TOOL_STAGETESTER
            // 
            this.MNU_TOOL_STAGETESTER.Name = "MNU_TOOL_STAGETESTER";
            this.MNU_TOOL_STAGETESTER.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_STAGETESTER.Text = "Stage Tester";
            this.MNU_TOOL_STAGETESTER.Visible = false;
            this.MNU_TOOL_STAGETESTER.Click += new System.EventHandler(this.MNU_TOOL_STAGETESTER_Click);
            // 
            // MNU_TOOL_PdScan
            // 
            this.MNU_TOOL_PdScan.Name = "MNU_TOOL_PdScan";
            this.MNU_TOOL_PdScan.Size = new System.Drawing.Size(202, 22);
            this.MNU_TOOL_PdScan.Text = "PD Scan Tester";
            this.MNU_TOOL_PdScan.Visible = false;
            this.MNU_TOOL_PdScan.Click += new System.EventHandler(this.MNU_TOOL_PdScan_Click);
            // 
            // MNU_MSR
            // 
            this.MNU_MSR.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menurReference,
            this.menuDut,
            this.menuDutBud,
            this.toolStripSeparator1,
            this.MNU_MSR_LAPD,
            this.toolStripSeparator4,
            this.menuDataViewer});
            this.MNU_MSR.Name = "MNU_MSR";
            this.MNU_MSR.Size = new System.Drawing.Size(113, 20);
            this.MNU_MSR.Text = "Measurement(&M)";
            // 
            // menurReference
            // 
            this.menurReference.Name = "menurReference";
            this.menurReference.Size = new System.Drawing.Size(146, 22);
            this.menurReference.Text = "Reference";
            this.menurReference.Click += new System.EventHandler(this.MNU_MSR_REF_Click);
            // 
            // menuDut
            // 
            this.menuDut.Name = "menuDut";
            this.menuDut.Size = new System.Drawing.Size(146, 22);
            this.menuDut.Text = "CWDM";
            this.menuDut.Click += new System.EventHandler(this.MNU_MSR_CWDMMUXFA_Click);
            // 
            // menuDutBud
            // 
            this.menuDutBud.Name = "menuDutBud";
            this.menuDutBud.Size = new System.Drawing.Size(146, 22);
            this.menuDutBud.Text = "CWDM BUD";
            this.menuDutBud.Visible = false;
            this.menuDutBud.Click += new System.EventHandler(this.MNU_MSR_CWDMMUXFABUD_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
            // 
            // MNU_MSR_LAPD
            // 
            this.MNU_MSR_LAPD.Name = "MNU_MSR_LAPD";
            this.MNU_MSR_LAPD.Size = new System.Drawing.Size(146, 22);
            this.MNU_MSR_LAPD.Text = "LargeArea PD";
            this.MNU_MSR_LAPD.Visible = false;
            this.MNU_MSR_LAPD.Click += new System.EventHandler(this.MNU_MSR_LAPD_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(143, 6);
            // 
            // menuDataViewer
            // 
            this.menuDataViewer.Name = "menuDataViewer";
            this.menuDataViewer.Size = new System.Drawing.Size(146, 22);
            this.menuDataViewer.Text = "Data Viewer";
            this.menuDataViewer.Visible = false;
            this.menuDataViewer.Click += new System.EventHandler(this.menuDataViewer_Click);
            // 
            // MNU_HELP
            // 
            this.MNU_HELP.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTest,
            this.menuSystemManual,
            this.menuOperationManal});
            this.MNU_HELP.Name = "MNU_HELP";
            this.MNU_HELP.Size = new System.Drawing.Size(60, 20);
            this.MNU_HELP.Text = "Help(&H)";
            // 
            // menuTest
            // 
            this.menuTest.Name = "menuTest";
            this.menuTest.Size = new System.Drawing.Size(251, 22);
            this.menuTest.Text = "==TEST==";
            this.menuTest.Click += new System.EventHandler(this.MNU_HELP_ABOUT_Click);
            // 
            // menuSystemManual
            // 
            this.menuSystemManual.Name = "menuSystemManual";
            this.menuSystemManual.Size = new System.Drawing.Size(251, 22);
            this.menuSystemManual.Text = "System Manual";
            this.menuSystemManual.Click += new System.EventHandler(this.menuManual_Click);
            // 
            // menuOperationManal
            // 
            this.menuOperationManal.Name = "menuOperationManal";
            this.menuOperationManal.Size = new System.Drawing.Size(251, 22);
            this.menuOperationManal.Text = "Operation Manual (测试作业说明书)";
            this.menuOperationManal.Click += new System.EventHandler(this.manualToolStripMenuItem_Click);
            // 
            // tss
            // 
            this.tss.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tss.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslbWavelen,
            this.tsslbSource});
            this.tss.Location = new System.Drawing.Point(0, 345);
            this.tss.Name = "tss";
            this.tss.Size = new System.Drawing.Size(544, 30);
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
            this.tsslbWavelen.Size = new System.Drawing.Size(100, 25);
            this.tsslbWavelen.Text = "0.0";
            // 
            // tsslbSource
            // 
            this.tsslbSource.AutoSize = false;
            this.tsslbSource.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslbSource.Name = "tsslbSource";
            this.tsslbSource.Size = new System.Drawing.Size(100, 25);
            this.tsslbSource.Text = "TLS";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(544, 375);
            this.Controls.Add(this.tss);
            this.Controls.Add(this.MenuStrip1);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmMain";
            this.Text = "Neon C_Agilent";
            this.Shown += new System.EventHandler(this.frmMain_Shown);
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
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_INIT;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_INITRES;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_STAGECONTROL;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_STAGEPOSITIONER;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_ALIGNMENT;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_DIGIOPTPWR;
    internal System.Windows.Forms.ToolStripMenuItem MNU_TOOL_DISTSENSVIEWER;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_LAMBDACAL;
    internal System.Windows.Forms.ToolStripMenuItem MNU_MSR;
    internal System.Windows.Forms.ToolStripMenuItem menurReference;
    internal System.Windows.Forms.ToolStripMenuItem MNU_HELP;
    internal System.Windows.Forms.ToolStripMenuItem menuTest;
    private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_SOURCECONTROL;
    private System.Windows.Forms.ToolStripMenuItem menuDut;
    internal System.Windows.Forms.StatusStrip tss;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbWavelen;
    private System.Windows.Forms.Timer timer1;
    internal System.Windows.Forms.ToolStripStatusLabel tsslbSource;
    private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_AIRVALVECONTROL;
	private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_STAGETESTER;
	private System.Windows.Forms.ToolStripMenuItem menuDutBud;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menuDataViewer;
	private System.Windows.Forms.ToolStripMenuItem MNU_TOOL_PdScan;
	private System.Windows.Forms.ToolStripMenuItem MNU_MSR_LAPD;
	private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripMenuItem uiMenuToolsUv;
    private System.Windows.Forms.ToolStripMenuItem menuSystemManual;
    private System.Windows.Forms.ToolStripMenuItem menuOperationManal;
}

