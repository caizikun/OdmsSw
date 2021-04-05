using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using Jeffsoft;


public partial class frmProcessRes : Form//,IFormCanClosed
{

    #region private variables

    private CprogRes m_pr;
    private SpeechSynthesizer m_syn;


    #endregion



    #region constructor/destructor

    public frmProcessRes()
    {
        InitializeComponent();
    }

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
    /// 화면을 클리어 한다.!!
    /// </summary>
    private void ClearWindow()
    {
        this.Text = "";
        lbCurItemNo.Text = "";
        lbStartTime.Text = "";
        lbEstiEndTime.Text = "";
        lbRemainTime.Text = "";
        lbEndTime.Text = "";
        lbProcessTime.Text = "";
        txtMessage.Text = "";
        lbPercent.Text = "";
        statusProgressBar.Value = statusProgressBar.Minimum;
        btnOK.Visible = false;

        this.Refresh();
    }




    #endregion


    #region public method


    /// <summary>
    /// Processing Data를 설정한다.
    /// </summary>
    /// <param name="pr"></param>
    public void SetProgress(CprogRes pr)
    {

        m_pr = null;
        m_pr = pr;


        if (m_pr != null) 
        {

            if (m_pr.compeleted == false)
            {
                timer1.Interval = 500;
                timer1.Start();

            }

        }


    }



    #endregion






    /// <summary>
    /// 폼을 초기화 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmProcessRes_Load(object sender, EventArgs e)
    {
        m_syn = new SpeechSynthesizer();
        m_syn.SelectVoiceByHints(VoiceGender.Female);

        //option & configs.
        string confFilepath = Application.StartupPath + "\\conf_processRes.xml";
        this.Location = LoadWndStartPos(confFilepath);
    }

   


    private void timer1_Tick(object sender, EventArgs e)
    {
        if (m_pr.compeleted == true)
        {
            //완료//

            timer1.Stop();

            TimeSpan durTime = m_pr.endTime - m_pr.startTime;

            this.Text = "완료!!";
            lbCurItemNo.Text = "...";
            lbStartTime.Text = String.Format("{0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}:{5:D2}",
                                                                m_pr.startTime.Year,
                                                                m_pr.startTime.Month,
                                                                m_pr.startTime.Day,
                                                                m_pr.startTime.Hour,
                                                                m_pr.startTime.Minute,
                                                                m_pr.startTime.Second);
            lbEstiEndTime.Text = "...";
            lbRemainTime.Text = "...";
            lbEndTime.Text = String.Format("{0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}:{5:D2}",
                                                                m_pr.endTime.Year,
                                                                m_pr.endTime.Month,
                                                                m_pr.endTime.Day,
                                                                m_pr.endTime.Hour,
                                                                m_pr.endTime.Minute,
                                                                m_pr.endTime.Second);


            lbProcessTime.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", durTime.Hours,
                                                                                                    durTime.Minutes,
                                                                                                    durTime.Seconds);
            txtMessage.Text = m_pr.msg;
            lbPercent.Text = "100";
            statusProgressBar.Value = statusProgressBar.Maximum;

            lbPercent.Visible = false;
            lbPercentSymbol.Visible = false;
            statusProgressBar.Visible = false;
            btnOK.Visible = true;

        }
        else
        {
            //---- processing----//


            m_pr.CalcEstimateEndTime();
            //DateTime estiEndTime = CProcessResult.CalcEstimateEndTime (m_pr);
            TimeSpan remainTime = m_pr.estiEndTime - DateTime.Now;
            TimeSpan durTime = DateTime.Now - m_pr.startTime;
            int totalTimes = (int)((m_pr.estiEndTime - m_pr.startTime).TotalSeconds);


            this.Text = "processing~~";
            lbCurItemNo.Text = m_pr.curItemNo;
            lbStartTime.Text = String.Format("{0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}:{5:D2}",
                                                                m_pr.startTime.Year,
                                                                m_pr.startTime.Month,
                                                                m_pr.startTime.Day,
                                                                m_pr.startTime.Hour,
                                                                m_pr.startTime.Minute,
                                                                m_pr.startTime.Second);

            lbEstiEndTime.Text = String.Format("{0:D4}-{1:D2}-{2:D2}  {3:D2}:{4:D2}:{5:D2}",
                                                                m_pr.estiEndTime.Year,
                                                                m_pr.estiEndTime.Month,
                                                                m_pr.estiEndTime.Day,
                                                                m_pr.estiEndTime.Hour,
                                                                m_pr.estiEndTime.Minute,
                                                                m_pr.estiEndTime.Second);


            lbRemainTime.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", remainTime.Hours,
                                                                                                        remainTime.Minutes,
                                                                                                        remainTime.Seconds);
            //lbEndTime.Text = String.Format("yyyy-MM-dd HH:mm:ss", m_pr.endTime);
            lbEndTime.Text = "...";

            lbProcessTime.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", durTime.Hours,
                                                                                                         durTime.Minutes,
                                                                                                         durTime.Seconds);
            if (txtMessage.Text != m_pr.msg)
            {
                txtMessage.Text = m_pr.msg;
            }



            //progress
            statusProgressBar.Minimum = 0;
            if (statusProgressBar.Maximum != totalTimes)
            {
                statusProgressBar.Maximum = totalTimes;
            }

            try
            {

                statusProgressBar.Value = (int)(totalTimes - remainTime.TotalSeconds);
                lbPercent.Text = Convert.ToString((int)((totalTimes - remainTime.TotalSeconds) / totalTimes * 100));


                //speak.
                int percent = statusProgressBar.Value;
                switch (percent)
                {
                    case 50:
                    m_syn.Speak("50%");
                    break;

                    case 80:
                    m_syn.Speak("80%");
                    break;

                    case 90:
                    m_syn.Speak("90%");
                    break;

                    case 95:
                    m_syn.Speak("95%");
                    break;

                }
                

            }
            catch
            {
                statusProgressBar.Value = statusProgressBar.Maximum;
                lbPercent.Text = "100";
            }

            lbPercent.Visible = true;
            lbPercentSymbol.Visible = true;
            statusProgressBar.Visible = true;

            btnOK.Visible = false;


        }
    }


    /// <summary>
    ///폼을 죽인다. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOK_Click(object sender, EventArgs e)
    {
        this.Close();
    }


    private void frmProcessRes_Shown(object sender, EventArgs e)
    {
        ClearWindow();
    }




    /// <summary>
    /// form을 마무리 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;

        //save options and options.
        string confFilepath = Application.StartupPath + "\\conf_processRes.xml";
        SaveWndStartPos(confFilepath);
        if (m_syn != null) m_syn.Dispose();
        m_syn = null;
    }
    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }

}
