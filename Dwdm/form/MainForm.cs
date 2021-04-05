using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner.UI;


public partial class MainForm : Form//, IFormCanClosed
{
    MyLogic mLogic;

    public MainForm()
    {
        InitializeComponent();

        mLogic = MyLogic.Instance;

        this.Text = Neon.Aligner.AppLogic.GetProductAndTime(Assembly.GetExecutingAssembly());
    }

        
    private void Form_Load(object sender, EventArgs e)
    {
        try
        {
            InitForm frmInit = new InitForm();
            frmInit.ShowDialog(this);
            tsslbSource.Text = (CGlobal.mIsLocalTls) ? $"Local TLS <{GlobalAddress.GPIB_AgilentTls}>" : $"TCP Server";
			tsslbPC.Text = $"PC <{CGlobal.mPcType}>";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"MainForm.Form_Load():\n{ex.Message}");
        }
    }


    private void Form_Shown(object sender, EventArgs e)
    {
        System.Threading.Thread.Sleep(100);
        //this.WindowState = FormWindowState.Maximized;
        Location = new Point(0, 0);
        Size = new Size(1920, 1030);
        WindowState = FormWindowState.Maximized;
        try
        {
            ShowBasicChildWnds();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"MainForm.Form_Shown():\n{ex.Message}");
        }
    }


    /// <summary>
    /// 기본적인 창을 띄운다.!!
    /// </summary>
    private void ShowBasicChildWnds()
    {
		if (CGlobal.LeftAligner != null && CGlobal.RightAligner != null)
		{
			MyLogic.CreateAndShow<AlignForm>(true, true);
			MyLogic.CreateAndShow<frmAlignStatus>(true, true);
			MyLogic.CreateAndShow<uiStageControl>(true, true);
		}

        var f = MyLogic.CreateAndShow<OpmDisplayForm>(false, true);
        //f.Pm = CGlobal.Pm8164;
        f.Show();

        MyLogic.CreateAndShow<frmSourceController>(true, true);

        menuFile_initDoNotAsk.Checked = Neon.Dwdm.Properties.Settings.Default.doInitNotAsk;

    }


    /// <summary>
    /// Software적으로 emergency stop한다.!!!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            var f = MyLogic.CreateAndShow<uiStageControl>(true, false);
            f?.StopStages();
            f?.UpdateAxisPos();

            MyLogic.CreateAndShow<AlignForm>(true, false)?.StopOperation();
        }

        //MyLogic.log($"Form_KeyDown(): {e.KeyCode}");
    }

    
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (CanIBeClosed(e))
        {
            if (CGlobal.Pm8164 != null) CGlobal.Pm8164.Dispose();
            if (CGlobal.Tls8164 != null) CGlobal.Tls8164.Dispose();
            foreach (var child in MdiChildren) child.Close();

            mLogic.SaveConfig();

            Neon.Dwdm.Properties.Settings.Default.doInitNotAsk = menuFile_initDoNotAsk.Checked;
            Neon.Dwdm.Properties.Settings.Default.Save();
        }        
    }

    public bool CanIBeClosed(object param)
    {
        var result = DialogResult.OK == MessageBox.Show("프로그램을 종료합니다.", "종료", MessageBoxButtons.OKCancel);
        Program.AppicationBeingQuit = result;
        ((FormClosingEventArgs)param).Cancel = !result;
        return result;
    }


    #region ---- Menu Handler ----

    private void MNU_TOOL_DISTSENSVIEWER_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<frmDistSensViewer>(true);
    }
    private void MNU_TOOL_STAGECONTROL_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<uiStageControl>(true);
    }
    private void MNU_TOOL_DIGIOPTPWR_Click(object sender, EventArgs e)
    {
        var f = MyLogic.CreateAndShow<OpmDisplayForm>(false, true);
        //f.Pm = CGlobal.Pm8164;
        f.Show();
    }
    private void MNU_TOOL_ALIGNMENT_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<AlignForm>(true, true);
    }
    private void MNU_TOOL_SOURCECONTROL_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<frmSourceController>(true);
    }
    private void referenceNonPolarizationToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<ReferenceForm>(true);
    }
    private void MNU_MSR_CWDMMUXFAPOL_Click(object sender, EventArgs e)
    {
        MyLogic.CreateAndShow<MeasureForm>(true);
    }

    private void MNU_FILE_EXIT_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void MNU_HELP_ABOUT_Click(object sender, EventArgs e)
    {
        Free302.MyLibrary.SwInfo.FSwInfoGrid f = new Free302.MyLibrary.SwInfo.FSwInfoGrid();
        f.ShowDialog();
    }




    #endregion


	private void menuTlsStateTest_Click(object sender, EventArgs e)
	{
		MyLogic.CreateAndShow<TlsStateTestForm>(true);
	}

    private void uiMenuScan_Click(object sender, EventArgs e)
    {
		//Scan Form
        var f = new ScanTest.ScanForm();
        f.MdiParent = Program.MainForm;
        f.Show();
    }

	private void uiMenuScanMonitorPort_Click(object sender, EventArgs e)
	{
		//Scan Monitor Port
		var f = new ScanTest.ScanMonitorPort();
		f.MdiParent = Program.MainForm;
		f.Show();
	}
	
	private void menuTestForm_Click(object sender, EventArgs e)
	{
		var f = new Neon.Dwdm.TestForm();
		f.MdiParent = Program.MainForm;
		f.Show();
	}

	private void uiMenuChamber_Click(object sender, EventArgs e)
	{
		var f = new Neon.Dwdm.ChamberControl();
		f.MdiParent = Program.MainForm;
		f.Show();
	}

	private void uiMenuUvCure_Click(object sender, EventArgs e)
	{
		var f = new UvCureForm();
        f.AssignDevice(CGlobal.LeftAligner, CGlobal.RightAligner, CGlobal.OtherAligner,
            CGlobal.UshioUvCure, CGlobal.AirValve, CGlobal.CameraAxis);
		f.MdiParent = Program.MainForm;
		f.Show();
	}

    private void menuMcuTest_Click(object sender, EventArgs e)
    {
        var f = new Neon.Dwdm.form.McuTestForm();
        f.MdiParent = Program.MainForm;
        f.Show();
    }
}
