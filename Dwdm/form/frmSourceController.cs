using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;
using Jeffsoft;


public partial class frmSourceController : Form//, IFormCanClosed
{
    #region Private member variables

    private Itls m_tls;
    private IoptMultimeter m_pm;
    private Osw m_osw;


    #endregion



    #region private method


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
            if (conf != null)
                conf.Dispose();

            conf = null;
        }

        return ret;
    }





    /// <summary>
    /// 간단한 정보를 ToolStripLabel에 출력한다.!!
    /// </summary>
    /// <param name="_msg"></param>
    private void DisplayShortInfor(string _msg)
    {
        tsslbStatus.Text = _msg;
        tss.Refresh();
    }



    /// <summary>
    /// display tls information.
    /// </summary>
    private void Display()
    {
        try
        {
            if (m_osw == null) return;

            //optical switch.
            int closePort = m_osw.GetOutClosedPort();
            if (closePort == GlobalAddress.OswAlignPort)
            {
                lbSource.Text = "635 + BLS";
                grpTls.Enabled = false;
            }
            else
            {
                lbSource.Text = "TLS";
                grpTls.Enabled = true;
            }


            //tls
            double wavelen = m_tls.GetTlsWavelen();
            double pwr = m_tls.GetTlsOutPwr();
            wavelen = Math.Round(wavelen, 3);   //[nm]
            pwr = Math.Round(pwr, 3);   //[dBm]

            lbTls.Text = wavelen.ToString() + ", " + pwr.ToString();

        }
        catch
        {
            lbTls.Text = "???, ???";
        }

    }



    private void EnableUpdate()
    {

        timer1.Start();
        this.Enabled = true;

    }


    private void DisableUpdate()
    {

        timer1.Stop();
        this.Enabled = false;

    }


    #endregion



    #region public method


    /// <summary>
    /// form을 Enable시킨다.
    /// </summary>
    public void ActiveUpdate()
    {

        Action eu = EnableUpdate;
        Invoke(eu);

    }


    /// <summary>
    /// form을 Enable시킨다.
    /// </summary>
    public void EnableForm()
    {
        Action eu = EnableUpdate;
        Invoke(eu);
    }


    /// <summary>
    /// form을 Disable시킨다.
    /// </summary>
    public void DeactiveUpdate()
    {
        Action du = DisableUpdate;
        Invoke(du);
    }


    /// <summary>
    /// form을 Disable시킨다.
    /// </summary>
    public void DisableForm()
    {
        Action du = DisableUpdate;
        Invoke(du);
    }


    #endregion






    public frmSourceController()
    {
        InitializeComponent();
    }





    /// <summary>
    /// initialize form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmSourceController_Load(object sender, EventArgs e)
    {
        try
        {
            m_tls = CGlobal.Tls8164;
			txtTlsWavelen.Text = m_tls.GetTlsWavelen().ToString();
			txtTlsPwr.Text = m_tls.GetTlsOutPwr().ToString();

            m_pm = CGlobal.Pm8164;

			m_osw = CGlobal.Switch;
			if (m_osw == null) groupOSW.Enabled = false;

			//configs.
			string confFilepath = Application.StartupPath + "\\conf_optSourCont.xml";
            this.Location = LoadWndStartPos(confFilepath);

            //start timer 
            timer1.Interval = 1000;
            if (m_tls != null && m_osw != null) timer1.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }


    /// <summary>
    /// Tls wavelength설정.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTlsOK_Click(object sender, EventArgs e)
    {

        try
        {
            //set wavelength.
            double targetWavelen = Convert.ToDouble(txtTlsWavelen.Text);
            targetWavelen = Math.Round(targetWavelen, 3);
            m_tls.SetTlsWavelen(targetWavelen);

            //설정이 완벽히 되었는지 검사.!!
            double appliedWavelen = m_tls.GetTlsWavelen();
            if (Math.Round(targetWavelen, 3) != Math.Round(appliedWavelen, 3))
            {
                MessageBox.Show("파장 설정 안됨.");
                throw new ApplicationException();
            }

            //완료 처리.
            DisplayShortInfor("TLS 설정 완료");
        }
        catch
        {
            MessageBox.Show("TLS 설정 실패");
            Display();
        }
    }



    private void timer1_Tick(object sender, EventArgs e)
    {
        Display();
    }




    /// <summary>
    /// form closing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;
        //save location options.
        string confFilepath = Application.StartupPath + "\\conf_optSourCont.xml";
        SaveWndStartPos(confFilepath);
    }
    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }


    /// <summary>
    /// 스위치 : source 변경. TLS
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void rbtnTLS_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (rbtnTLS.Checked == true) m_osw?.SetToTls();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"frmSourceController.tls_CheckedChanged():\n{ex.Message}\n{ex.StackTrace}");
        }
    }


    /// <summary>
    /// 스위치 : source 변경. 635+BLS
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void rbtn635_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (rbtn635bls.Checked == true) m_osw?.SetToAlign();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"frmSourceController.tls_CheckedChanged():\n{ex.Message}\n{ex.StackTrace}");
        }
    }
    
    /// <summary>
    /// Tls wavelength설정.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnTlsPwr_Click(object sender, EventArgs e)
    {
        try
        {
            //set optical power.
            double targetPwr = Convert.ToDouble(txtTlsPwr.Text);
            targetPwr = Math.Round(targetPwr, 3);
            m_tls.SetTlsOutPwr(targetPwr);


            //설정이 완벽히 되었는지 검사.!!
            double appliedPwr = m_tls.GetTlsOutPwr();
            if (Math.Round(targetPwr, 3) != Math.Round(appliedPwr, 3))
            {
                MessageBox.Show("광파워 설정 안됨.");
                throw new ApplicationException();
            }
            

            //완료 처리.
            DisplayShortInfor("TLS 설정 완료");

        }
        catch
        {
            MessageBox.Show("TLS 설정 실패");
            Display();
        }
    }


}
