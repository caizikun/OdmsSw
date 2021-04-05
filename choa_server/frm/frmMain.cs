using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Neon.Aligner;
using Neon.Aligner.UI;

public partial class frmMain : Form
{
    #region constructor/destructor


    /// <summary>
    /// constructor
    /// </summary>
    public frmMain()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        var initForm = new InitForm();
        initForm.ShowDialog(this);
        if (Program.ExitApp) return;

        base.OnLoad(e);
        //base.Width = 1400;
        //base.Height = 1100;

        this.Text = $"{AppLogic.License.Version}";
        initUIbyLicense();
        checkTlsPmMode();
    }
    void initUIbyLicense()
    {
        // HSB vs HSC
        var hsb = AppLogic.License.IsHSB;
        var dev = AppLogic.License.ShowDevUI || CGlobal.IsTestMode;

        // Dev vs Prod
        menuTest.Visible = dev;
        uiMenuToolsUv.Visible = hsb || dev;
        menuDut.Visible = !hsb || dev;
        menuDutBud.Visible = AppLogic.License.ShowBudMenu || dev;
    }
    /// <summary>
    /// TLS & PM 단독 사용 모드에서 기능창 Open menu 비활성화
    /// </summary>
    private void checkTlsPmMode()
    {
        if (CGlobal.IsTlsPmMode)
        {
            MNU_TOOL_STAGECONTROL.Enabled = false;
            MNU_TOOL_STAGEPOSITIONER.Enabled = false;
            MNU_TOOL_ALIGNMENT.Enabled = false;
            MNU_TOOL_DISTSENSVIEWER.Enabled = false;
            MNU_TOOL_AIRVALVECONTROL.Enabled = false;
            MNU_TOOL_STAGETESTER.Enabled = false;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (Program.ExitApp) e.Cancel = false;
        else
        {
            var dr = MessageBox.Show("Quit app?", "Quit", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            e.Cancel = dr != DialogResult.Yes;
            if (e.Cancel) return;
        }
        base.OnFormClosing(e);

        Program.ExitApp = true;
        //Application.Exit();
    }

    /// <summary>
    /// 기본 창을 띄운다.!!
    /// </summary>
    private void frmMain_Shown(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(100);
        WindowState = FormWindowState.Maximized;
        Refresh();

        try
        {
            ShowBasicChildWnds();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            if (Application.OpenForms.OfType<uiStageControl>().Count() > 0)
            {
                uiStageControl frm = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();
                frm.StopStages();
                frm.UpdateAxisPos();
            }

            if (Application.OpenForms.OfType<AlignForm>().Count() > 0)
            {
                AlignForm frm = Application.OpenForms.OfType<AlignForm>().FirstOrDefault();
                frm.StopOperation();
            }
        }
        base.OnKeyDown(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F12 && e.Control && e.Shift && e.Alt)
        {
            AppLogic.License.ShowDevUI = !AppLogic.License.ShowDevUI;
            initUIbyLicense();

            e.SuppressKeyPress = true;
        }
        base.OnKeyUp(e);
    }


    #endregion




    #region private method


    /// <summary>
    /// 기본적인 창을 띄운다.!!
    /// </summary>
    private void ShowBasicChildWnds()
	{

		Form frm = null;
        
		if (CGlobal.Opm != null)
		{
			//optical powermeter
			frm = new OpmDisplayForm();
			frm.MdiParent = this;
			frm.Show();
		}
		
		if (CGlobal.TlsType != GlobalBase._TlsType.Test && (CGlobal.Tls != null || CGlobal.osw != null))
		{
			//Source controller
			frm = new frmSourceController(CGlobal.Tls, CGlobal.Opm, CGlobal.osw, frm);
			frm.MdiParent = this;
			frm.Show();
			frm.Refresh(); 
		}

        if (CGlobal.IsTlsPmMode) return;

		if (CGlobal.LeftAligner != null)
		{
			//alignment
			frm = new AlignForm();
			frm.MdiParent = this;
			frm.Show();
			frm.Refresh();

			//Stage Controller
			frm = new uiStageControl();
			frm.MdiParent = this;
			frm.Show();
			frm.Refresh(); 
		}

        uiMenuToolsUv.PerformClick();

    }


    private void MNU_FILE_EXIT_Click(object sender, EventArgs e)
    {
        this.Close();
    }



    private void timer1_Tick(object sender, EventArgs e)
    {

        //tls wavelength
        try
        {
            Itls tls = CGlobal.Tls;
            double wavelen = tls.GetTlsWavelen();
            wavelen = Math.Round(wavelen, 3);
            tsslbWavelen.Text = Convert.ToString(wavelen);
        }
        catch
        {
            tsslbWavelen.Text = "???";
        }


        //optical source
        //try
        //{
        //    IoptSwitch osw = CGlobal.g_osw1;
        //    int outPort = osw.GetOutClosedPort();
        //    if (outPort == CGlobal.OSW1_635)
        //        tsslbSource.Text = "635";
        //    else
        //        tsslbSource.Text = "TLS";
        //}
        //catch
        //{
        //    tsslbSource.Text = "???";
        //}


    }


    #endregion
    



    #region ==== Menu Tools ====


    private void MNU_TOOL_DISTSENSVIEWER_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0) return;

        frmDistSensViewer frm = new frmDistSensViewer();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_STAGECONTROL_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<uiStageControl>().Count() > 0) return;

		uiStageControl frm = new uiStageControl();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_DIGIOPTPWR_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<OpmDisplayForm>().Count() > 0) return;

        OpmDisplayForm frm = new OpmDisplayForm();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_ALIGNMENT_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<AlignForm>().Count() > 0) return;

        AlignForm frm = new AlignForm();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_SOURCECONTROL_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0) return;

        Form fPm = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();
        frmSourceController frm = new frmSourceController(CGlobal.Tls, CGlobal.Opm, CGlobal.osw, fPm);
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_STAGEPOSITIONER_Click(object sender, EventArgs e)
    {

        if (Application.OpenForms.OfType<frmStagePos>().Count() > 0) return;

        frmStagePos frm = new frmStagePos();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_TOOL_AIRVALVECONTROL_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<frmAirValveCont>().Count() > 0)return;

        frmAirValveCont frm = new frmAirValveCont();
        frm.MdiParent = this;
        frm.Show();

    }


    private void MNU_TOOL_STAGETESTER_Click(object sender, EventArgs e)
    {
        Free302.TnM.Device.StageTester.SpeedTesterForm.Self.Show();
    }
    

    private void menuDataViewer_Click(object sender, EventArgs e)
    {
        var f = new DataControlForm();
		f.Show();
    }

	private void MNU_TOOL_PdScan_Click(object sender, EventArgs e)
	{
	}

	#endregion




	#region ==== Menu <Not use> ====


	private void cWDMDemuxFiberArrayToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //if (Application.OpenForms.OfType<frmCwdmDemuxFa>().Count() > 0)
        //	return;

        //frmCwdmDemuxFa frm = new frmCwdmDemuxFa();
        //frm.MdiParent = this;
        //frm.Show();
    }


    private void MNU_MSR_1X1_Click(object sender, EventArgs e)
    {
        //if (Application.OpenForms.OfType<frm1by1>().Count() > 0)
        //	return;

        //frm1by1 frm = new frm1by1();
        //frm.MdiParent = this;
        //frm.Show();
    }


    private void MNU_MSR_CWDMMUX1F_Click(object sender, EventArgs e)
    {
        //if (Application.OpenForms.OfType<frmCwdmMux1f>().Count() > 0)
        //	return;

        //frmCwdmMux1f frm = new frmCwdmMux1f();
        //frm.MdiParent = this;
        //frm.Show();
    }


    private void MNU_MSR_LWDM1F_Click(object sender, EventArgs e)
    {
        //if (Application.OpenForms.OfType<frmLwdm1f>().Count() > 0)
        //	return;

        //frmLwdm1f frm = new frmLwdm1f();
        //frm.MdiParent = this;
        //frm.Show();
    }


    private void MNU_MSR_LWDMFA_Click(object sender, EventArgs e)
    {
        //if (Application.OpenForms.OfType<frmLwdmFa>().Count() > 0)
        //	return;

        //frmLwdmFa frm = new frmLwdmFa();
        //frm.MdiParent = this;
        //frm.Show();
    }


    #endregion




    #region ==== Menu Measurements ====


    private void MNU_MSR_REF_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<RefForm>().Count() > 0) return;

        RefForm frm = new RefForm();
        frm.MdiParent = this;
        frm.Show();
    }

    
    private void MNU_MSR_CWDMMUXFA_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<MuxFaForm>().Count() > 0) return;

        MuxFaForm frm = new MuxFaForm();
        frm.MdiParent = this;
        frm.Show();
    }


    private void MNU_MSR_CWDMMUXFABUD_Click(object sender, EventArgs e)
    {
        if (Application.OpenForms.OfType<MuxBudForm>().Count() > 0) return;

        MuxBudForm frm = new MuxBudForm();
        frm.MdiParent = this;
        frm.Show();
    }

	
	private void MNU_MSR_LAPD_Click(object sender, EventArgs e)
	{
		if (Application.OpenForms.OfType<LapdDutForm>().Count() > 0) return;

		LapdDutForm frm = new LapdDutForm();
		frm.MdiParent = this;
		frm.Show();
	}

    private void MNU_TOOL_LAMBDACAL_Click(object sender, EventArgs e)
    {

    }

    private void uiMenuToolsUv_Click(object sender, EventArgs e)
    {
        AlignForm af = Application.OpenForms.OfType<AlignForm>().FirstOrDefault();
        if (af == null)
        {
            af = new AlignForm();
            af.MdiParent = this;
            af.Show();
        }

        if (Application.OpenForms.OfType<UvCureForm>().Count() > 0) return;

        var uf = new UvCureForm();
        uf.AssignDevice(CGlobal.LeftAligner, CGlobal.RightAligner, CGlobal.OtherAligner,
            CGlobal.UshioUvCure, CGlobal.AirValve, CGlobal.CameraAxis);

        uf.CancelAlign = af.CancelAlign;
        uf.AlignerAction = af.RunAction;
        uf.SetOpmChs(af.OpmChs);

        var cf = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();
        if (cf != null) uf.CoordUpdater = cf.UpdateAxisPos;

        uf.MdiParent = this;
        uf.Show();
    }



    #endregion


    #region ==== Menu Help ====

    private void MNU_HELP_ABOUT_Click(object sender, EventArgs e)
    {
        var f = new Neon.Aligner.choa_server.TestForm();
        //f.MdiParent = this;
        f.Show();

        //dialog test
        //MessageBox.Show("test...");
    }

    private void menuManual_Click(object sender, EventArgs e)
    {
        try
        {
            AppLogic.ShowManual(1);
        }
        catch { }
    }

    private void manualToolStripMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            AppLogic.ShowManual(2);
        }
        catch { }
    }

    #endregion


}
