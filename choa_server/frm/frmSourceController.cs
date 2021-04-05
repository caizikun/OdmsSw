using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Neon.Aligner;
using Jeffsoft;


public partial class frmSourceController : Form
{

	#region ==== Class Framework ====

	const string sConfigFileName = @"config\conf_optSourCont.xml";

	private Itls mTls;
    private IoptMultimeter mPm;
	private IoptSwitch mOsw;
	private Form mPmForm;

	//public static frmSourceController Self { get; private set; }
	public frmSourceController(Itls tls, IoptMultimeter pm, IoptSwitch osw, Form pmForm)
	{
		mTls = tls;
        mPm = pm;
        if(osw != null)
        {
            mOsw = osw;
            mOsw.PortChanged += osw_PortChanged;
        }
        mPmForm = pmForm;

		InitializeComponent();
	}



	private void frmSourceController_Load(object sender, EventArgs e)
	{

        if (!CGlobal.UsingOsw) groupOSW.Enabled = false;
        if (CGlobal.UsingTcpServer) groupTLS.Enabled = false;

		try
		{
			//configs.
			string confFilepath = System.IO.Path.Combine(Application.StartupPath, sConfigFileName);
			this.Location = LoadWndStartPos(confFilepath);			

			//readAndDisplayDeviceSettings();

		}
		catch(Exception ex)
		{
			MessageBox.Show(ex.Message);
		}

	}



	private void frmSourceController_FormClosing(object sender, FormClosingEventArgs e)
	{
		//save location options.
		string confFilepath = System.IO.Path.Combine(Application.StartupPath, sConfigFileName);
		SaveWndStartPos(confFilepath);
	}



	/// <summary>
	/// widow start postion을 불러온다.
	/// </summary>
	/// <param name="_filePath">config file path.</param>
	/// <returns></returns>
	private Point LoadWndStartPos(string _filePath)
	{
		Point ret = new Point();

		string temp = "";
		try
		{

			XConfig conf = new XConfig(_filePath);


			try
			{
				temp = conf.GetValue("WNDPOSX");
				ret.X = Convert.ToInt32(temp);
			}
			catch
			{
				ret.X = 0;
			}


			try
			{
				temp = conf.GetValue("WNDPOSY");
				ret.Y = Convert.ToInt32(temp);
			}
			catch
			{
				ret.Y = 0;
			}

		}
		catch
		{
			ret.X = 0;
			ret.Y = 0;
		}

		return ret;
	}



	/// <summary>
	/// widow start postion을 저장한다.
	/// </summary>
	/// <param name="_filePath">config file path.</param>
	/// <returns></returns>
	private Point SaveWndStartPos(string _filePath)
	{

		Point ret = new Point();
		XConfig conf = null;

		string temp = "";
		try
		{
			conf = new XConfig(_filePath);


			temp = this.Location.X.ToString();
			conf.SetValue("WNDPOSX", temp);


			temp = this.Location.Y.ToString();
			conf.SetValue("WNDPOSY", temp);


		}
		catch
		{
			//do nothing
		}
		finally
		{
			if(conf != null)
				conf.Dispose();

			conf = null;
		}

		return ret;
	}


    #endregion




    #region  ==== private method ====


    private void updateUiWlState(double wl)
    {
        bInternalChange = true;
        if (wl == 1271.000) rbtn1271.Checked = true; else rbtn1271.Checked = false;
        if (wl == 1291.000) rbtn1291.Checked = true; else rbtn1291.Checked = false;
        if (wl == 1311.000) rbtn1311.Checked = true; else rbtn1311.Checked = false;
        if (wl == 1331.000) rbtn1331.Checked = true; else rbtn1331.Checked = false;
        bInternalChange = false;
    }



    /// <summary>
    /// 간단한 정보를 ToolStripLabel에 출력한다.!!
    /// </summary>
    /// <param name="_msg"></param>
    private void displayStatusMessage(string _msg)
    {
        tsslbStatus.Text = (mOsw != null) ? _msg : "OSW not Conneted";
        tss.Refresh();
    }


    #endregion




    #region ==== Public API ====


    public void EnableForm()
	{
        Invoke((Action)(() => this.Enabled = true));
    }
	public void DisableForm()
	{
		Invoke((Action)(() => this.Enabled = false));
	}


	#endregion




	#region ==== TLS 설정 ====


	private void txtTlsWavelen_KeyDown(object sender, KeyEventArgs e)
	{
		if(e.KeyCode == Keys.Enter) this.btnTlsOK.PerformClick();
	}



	private void btnTlsOK_Click(object sender, EventArgs e)
	{
        var frmDigitalPwr = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
        
        bool pmFormStatus = true;
		try
		{
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            if (mPmForm != null)
			{
				pmFormStatus = mPmForm.Enabled;
				mPmForm.Enabled = false;
				Thread.Sleep(100);
			}

			//set wavelength.
			double target = Convert.ToDouble(txtTlsWavelen.Text);
			setTlsWl(Math.Round(target, 3));

			//set power
			target = Convert.ToDouble(txtTlsPwr.Text);
			setTlsPower(Math.Round(target, 3));

			//완료 처리.
			displayStatusMessage("TLS 설정 완료");

		}
		catch(Exception ex)
		{
			//readAndDisplayDeviceSettings();
			MessageBox.Show(ex.Message);
		}
		finally
		{
			if(mPmForm != null)
			{
				mPmForm.Enabled = pmFormStatus;
				Thread.Sleep(100);
			}
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
        }
	}



	void setTlsWl(double target)
	{
		mTls.SetTlsWavelen(target);
		Thread.Sleep(200);

		double actual = Math.Round(mTls.GetTlsWavelen(), 3);
		if(target != actual)
		{
			throw new Exception($"TLS 파장 설정이 제대로 되지 않았습니다.\r\n{target} != {actual}");
		}
		updateUiWlState(target);
	}



	void setTlsPower(double target)
	{
		mTls.SetTlsOutPwr(target);
		Thread.Sleep(200);

		double actual = Math.Round(mTls.GetTlsOutPwr(), 3);
		if(target != actual)
		{
			throw new Exception($"TLS 파워 설정이 제대로 되지 않았습니다.\r\n{target} != {actual}");
		}
	}


	bool bInternalChange = false;

	private void TlsWL_CheckedChanged(object sender, EventArgs e)
	{
		if(!(sender as RadioButton).Checked) return;
		if(bInternalChange) return;

        var frmDigitalPwr = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
        if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

        double wl = 1301;
		if(rbtn1271.Checked == true) wl = 1271;
		if(rbtn1291.Checked == true) wl = 1291;
		if(rbtn1311.Checked == true) wl = 1311;
		if(rbtn1331.Checked == true) wl = 1331;
		txtTlsWavelen.Text = wl.ToString();
		setTlsWl(wl);

        if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
    }


    #endregion




    #region ==== Source Switch ====

    bool beginUpdate = false;

    private void Osw_CheckedChanged(object sender, EventArgs e)
	{
        try
        {
            beginUpdate = true;
            var r = (sender as RadioButton);
            if (r.Checked) setupOsw();
            displayStatusMessage($"OSW : {r.Text} - OK");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Osw_CheckedChanged():\n{ex.Message}");
        }
    }



    void setupOsw()
    {
        if (rbtnAlign.Checked)
        {
            mOsw?.SetToAlign();
            mPm.SetGainLevel(CGlobal.PmAlignGain);        //Align Gain Level 변경
        }
        else if (rbtnTLS.Checked)
        {
            mOsw?.SetToTls();
            mPm.SetGainLevel(CGlobal.PmGain[0]);          //Sweep Gain 1 Level 변경
        }
    }



    private void osw_PortChanged(int port)
    {
        if (beginUpdate) return;
        Invoke((Action) delegate ()
        {
            rbtnTLS.Checked = CGlobal.OswTlsPort == port;
            rbtnAlign.Checked = !rbtnTLS.Checked;
        });
    }

    #endregion


}
