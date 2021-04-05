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



public partial class frmSourceController : Form
{



	#region ==== Class Framework ====


	const int sOswTls = 1;
	const int sOsw635 = 2;
	const string sConfigFileName = "conf_optSourCont.xml";

	private Itls mTls;
	private IoptSwitch mOsw;
	private Form mPmForm;

	private static frmSourceController sSelf;
	//public static frmSourceController Self { get; private set; }




	public frmSourceController(Itls tls, IoptSwitch osw, Form pmForm)
	{

		mTls = tls;
		mOsw = osw;
		mPmForm = pmForm;

		if(sSelf != null) return;
		InitializeComponent();
		sSelf = this;

	}




	public frmSourceController(Itls tls, Form pmForm)
	{

		mTls = tls;
		mPmForm = pmForm;

		if (sSelf != null) return;
		InitializeComponent();
		sSelf = this;

	}




	private void frmSourceController_Load(object sender, EventArgs e)
	{

		try
		{
			//configs.
			string confFilepath = System.IO.Path.Combine(Application.StartupPath, sConfigFileName);
			this.Location = LoadWndStartPos(confFilepath);			

			readAndDisplayDeviceSettings();

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

		sSelf = null;
	}




	/// <summary>
	/// widow start postion을 불러온다.
	/// </summary>
	/// <param name="_filePath">config file path.</param>
	/// <returns></returns>
    /// 
	private Point LoadWndStartPos(string _filePath)
	{

		Point ret = new Point();

		string temp = "";
		try
		{

			Cconfig conf = new Cconfig(_filePath);


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
		Cconfig conf = null;

		string temp = "";
		try
		{
			conf = new Cconfig(_filePath);


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





	/// <summary>
	/// read OSW & TSL settings and Update UI
	/// </summary>
	private void readAndDisplayDeviceSettings()
	{

		#region Powermeter Display

		bool pmFormStatus = true;
		try
		{
			if(mPmForm != null)
			{
				pmFormStatus = mPmForm.Enabled;
				mPmForm.Enabled = false;
				Thread.Sleep(100);
			}
		}
		catch{	//무시
		}

		#endregion



		#region Source Switch

		//try
		//{
		//	//optical switch.
		//	int closePort = mOsw.GetOutClosedPort();
		//	if (closePort == sOsw635)
		//	{
		//		rbtn635.Checked = true;
		//		//grpTls.Enabled = false;
		//	}
		//	else
		//	{
		//		rbtnTLS.Checked = true;
		//		//grpTls.Enabled = true;
		//	}
		//}
		//catch{}

		#endregion



		#region TLS Settings

		try
		{
			double waveLen = Math.Round(mTls.GetTlsWavelen(), 3);
			txtTlsWavelen.Text = $"{waveLen}";
			updateUiWlState(waveLen);

			//optical power.
			double pwr = Math.Round(mTls.GetTlsOutPwr(), 3);
			txtTlsPwr.Text = pwr.ToString();
		}
		catch (Exception ex)
		{
			txtTlsWavelen.Text = "???";
			txtTlsPwr.Text = "???";
			rbtn1271.Checked = false;
			rbtn1291.Checked = false;
			rbtn1311.Checked = false;
			rbtn1331.Checked = false;
			throw ex;
		}

		#endregion



		#region Powermeter Display

		try
		{
			if(mPmForm != null)
			{
				mPmForm.Enabled = pmFormStatus;
				Thread.Sleep(100);
			}
		}
		catch { }

		#endregion
	

	}




	private void updateUiWlState(double wl)
	{

		bInternalChange = true;
		if(wl == 1271.000) rbtn1271.Checked = true; else rbtn1271.Checked = false;
		if(wl == 1291.000) rbtn1291.Checked = true; else rbtn1291.Checked = false;
		if(wl == 1311.000) rbtn1311.Checked = true; else rbtn1311.Checked = false;
		if(wl == 1331.000) rbtn1331.Checked = true; else rbtn1331.Checked = false;
		bInternalChange = false;

	}




	/// <summary>
	/// 간단한 정보를 ToolStripLabel에 출력한다.!!
	/// </summary>
	/// <param name="_msg"></param>
	private void displayStatusMessage(string _msg)
	{

		tsslbStatus.Text = _msg;
		tss.Refresh();

	}




	#region ==== Public API ====

	public void EnableForm()
	{
		Action a = ()=> 
		{
			readAndDisplayDeviceSettings();
			this.Enabled = true;
		};
		Invoke(a);
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

		bool pmFormStatus = true;
		try
		{
			if(mPmForm != null)
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
			readAndDisplayDeviceSettings();
			MessageBox.Show(ex.Message);
		}
		finally
		{
			if(mPmForm != null)
			{
				mPmForm.Enabled = pmFormStatus;
				Thread.Sleep(100);
			}
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

		double wl = 1301;
		if(rbtn1271.Checked == true) wl = 1271;
		if(rbtn1291.Checked == true) wl = 1291;
		if(rbtn1311.Checked == true) wl = 1311;
		if(rbtn1331.Checked == true) wl = 1331;
		txtTlsWavelen.Text = wl.ToString();
		setTlsWl(wl);
	}
	

	#endregion





	#region ==== Source Switch ====


	private void Osw_CheckedChanged(object sender, EventArgs e)
	{
		//if((sender as RadioButton).Checked) setupOsw();
	}




	void setupOsw()
	{

		//635 laser.
		try
		{
			if (rbtn635.Checked) mOsw.CloseOutPort(sOsw635);
		}
		catch
		{

		}

		//tls
		try
		{
			if (rbtnTLS.Checked) mOsw.CloseOutPort(sOswTls);
		}
		catch
		{

		}
		
	}



	#endregion



}
