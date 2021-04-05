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


public partial class frmAirValveCont : Form
{


    #region Private member variables

    private IairValvController m_avc;

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
    /// display tls information.
    /// </summary>
    private void Display()
    {
        try
        {


            //valve1
            AirValveState vs1;
            vs1 = m_avc.ValveState((int)AirValveAligner.Input);

            if (vs1 == AirValveState.open)
            {
                if (tsslbIn.Text != "OPEN")
                {
                    tsslbIn.Text = "OPEN";
                    tsslbIn.ForeColor = Color.Red;
                    btnOpenIn.Enabled = false;
                    btnCloseIn.Enabled = true;
                }

            }
            else if (vs1 == AirValveState.close)
            {
                if (tsslbIn.Text != "CLOSE")
                {
                    tsslbIn.Text = "CLOSE";
                    tsslbIn.ForeColor = Color.Black;
                    btnOpenIn.Enabled = true;
                    btnCloseIn.Enabled = false;
                }
            }
            else
            {
                if (tsslbIn.Text != "CLOSE")
                {
                    tsslbIn.Text = "...";
                    tsslbIn.ForeColor = Color.Black;
                    btnOpenIn.Enabled = true;
                    btnCloseIn.Enabled = true;
                }

            }


            //valve2
            AirValveState vs2;
            vs2 = m_avc.ValveState((int)AirValveAligner.Output);

            if (vs2 == AirValveState.open)
            {
                if (tsslbOut.Text != "OPEN")
                {
                    tsslbOut.Text = "OPEN";
                    tsslbOut.ForeColor = Color.Red;
                    btnOpenOut.Enabled = false;
                    btnCloseOut.Enabled = true;
                }
            }
            else if (vs2 == AirValveState.close)
            {
                if (tsslbOut.Text != "CLOSE")
                {
                    tsslbOut.Text = "CLOSE";
                    tsslbOut.ForeColor = Color.Black;
                    btnOpenOut.Enabled = true;
                    btnCloseOut.Enabled = false;
                }
            }
            else
            {

                if (tsslbOut.Text != "...")
                {
                    tsslbOut.Text = "...";
                    tsslbOut.ForeColor = Color.Black;
                    btnOpenOut.Enabled = true;
                    btnCloseOut.Enabled = true;
                }

            }


        }
        catch
        {
            tsslbIn.Text = "...";
            tsslbOut.Text = "...";
            tsslbIn.ForeColor = Color.Black;
            tsslbOut.ForeColor = Color.Black;
            tsslbIn.Enabled = true;
            tsslbOut.Enabled = true;
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

    



    #region  constructor/destructor

    public frmAirValveCont()
    {
        InitializeComponent();
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
    /// form을 Disable시킨다.
    /// </summary>
    public void DeactiveUpdate()
    {
        Action du = DisableUpdate;
        Invoke(du);
    }
    

    #endregion





    /// <summary>
    /// form을 초기화 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAirValveCont_Load(object sender, EventArgs e)
    {

        m_avc = CGlobal.AirValve;


        //configs.
        string confFilepath = Application.StartupPath + @"\config\conf_airValveCont.xml";
        this.Location = LoadWndStartPos(confFilepath);


        //start timer.
        timer1.Interval = 1000;
        timer1.Start();

    }




    /// <summary>
    /// form을 마무리 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAirValveCont_FormClosing(object sender, FormClosingEventArgs e)
    {
        //save location options.
        string confFilepath = Application.StartupPath + @"\config\conf_airValveCont.xml";
        SaveWndStartPos(confFilepath);
    }




    private void timer1_Tick(object sender, EventArgs e)
    {
        Display();
    }




    private void btnOpenIn_Click(object sender, EventArgs e)
    {
        m_avc.OpenValve((int)AirValveAligner.Input);
    }




    private void btnOpenOut_Click(object sender, EventArgs e)
    {
        m_avc.OpenValve((int)AirValveAligner.Output);
    }




    private void btnCloseIn_Click(object sender, EventArgs e)
    {
        m_avc.CloseValve((int)AirValveAligner.Input);
    }




    private void btnCloseOut_Click(object sender, EventArgs e)
    {
        m_avc.CloseValve((int)AirValveAligner.Output);
    }



}
