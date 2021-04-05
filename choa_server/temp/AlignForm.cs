using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Neon.Aligner;
using Jeffsoft;


public partial class AlignForm : Form
{


    #region definition

    private const int ALIGNRES = 1;                   //alignment resolution [um]
    private const int STGPOSXYZRES = 2;               //XYZ position Stage resolution.


    private int DEFAULT_DIGIPORT1 = 1;                //digital port first.
    private int DEFAULT_DIGIPORT2 = 2;                //digital port last.
    private int DEFAULT_FASTRNG = 20;                 //fast search range. [um]
    private int DEFAULT_BLINDRNG = 100;               //blind range [um]
    private int DEFAULT_BLINDSTEP = 4;                //blind step [um]
    private int DEFAULT_BLINDTHRES = -25;             //blind searcg threshold power. [dBm]
    private int DEFAULT_ROLLDISTIN = 0;
    private int DEFAULT_ROLLDISTOUT = 889;
    private bool DEFAULT_TLSFORROLL = false;
    private int DEFAULT_TLSROLLWAVE1 = 1271;          //TLS for Roll wavlenegth first.
    private int DEFAULT_TLSROLLWAVE2 = 1331;          //TLS for Roll wavlenegth last.
    private int DEFAULT_ROLLRNG = 20;
    private int DEFAULT_ROLLSTEP = 1; 
    private double DEFAULT_ROLLPOSTCOND = 0.5; 


    //private string DEFAULT_AXISSCAN_STAGE = "INPUT"; //input stage
    private string DEFAULT_AXISSCAN_AXIS = "X";      //x axis
    private int DEFAULT_AXISSCAN_PORT = 1;
    private int DEFAULT_AXISSCAN_RNG = 20;           // [um]
    private int DEFAULT_AXISSCAN_STEP = 1;           // [um]

    #endregion





    #region Structure


    private struct ThreadParam
    {
        public int cmd;
        public int stageNo;
        public int axis;
        public int port1;   
        public int port2;
        public int range;               //Search Range.
        public int range2;
        public double step;
        public double step2;
        public double thres;            //thresshold power.[dBm] or post condition [um] for alignment
        public int rollDist;
        public bool tlsForRoll;         //tls for roll
        public Itls tls;
        public double wave1;            //TLS Roll 첫번째 파장.
        public double wave2;            //TLS 마지막 파장.
    }


    #endregion





    #region Private member variables

    private Istage m_leftStage;
    private Istage m_rightStage;
    //private IoptSwitch m_osw1;						// for input roll-alignment.
    private Itls m_tls;									// for tlsRoll 

    private IoptMultimeter m_mpm;
    private IAlignment m_align;

    private AutoResetEvent m_autoEvent;
    private Thread m_thread;
    private ThreadParam m_tp;

    #endregion





    #region property

    public int alignPort { get { return m_tp.port1; } }


    #endregion





    #region consturctor/destructor


    //생성자.
    public AlignForm()
    {
        InitializeComponent();
    }


    #endregion





    #region Thread function


    /// <summary>
    /// Thread Function.
    /// </summary>
    private void ThreadFunc()
    {

        int cmd = 0;

        while (true)
        {

            m_autoEvent.WaitOne();

            cmd = m_tp.cmd;
            if (m_align is IAlignmentFa)
            {
                // fa alignemnt //
                IAlignmentFa align = (IAlignmentFa)m_align;
                int sn = m_tp.stageNo; //stage no.

                if (cmd == align.ZAPPROACH_SINGLE )
                    align.ZappSingleStage(sn);
                else if (cmd == align.ZAPPROACH_DUAL)
                    align.ZappBothStage();
                else if (cmd == align.FAARRANAGE_TY_SINGLE )
                    align.FaArrTySingleStage(sn);
                else if (cmd == align.FAARRANAGE_TX_SINGLE )
                    align.FaArrTxSingleStage(sn);
            }


            if (m_align is IAlignmentDigital)
            {
                // alignemnt Digital //
                IAlignmentDigital align = (IAlignmentDigital)m_align;

                if (cmd == align.XYSEARCH)
                {
                    align.XySearch(m_tp.stageNo,
                                    m_tp.port1,
                                    m_tp.step);

                }

                else if (cmd == align.XYBLINDSEARCH)
                {
                    align.XyBlindSearch(m_tp.stageNo,
                                        m_tp.port2,
                                        m_tp.range,
                                        m_tp.step,
                                        m_tp.thres);
                }


                else if (cmd == align.XYFULLBLINDSEARCH)
                {
                    align.XyFullBlindSearch(m_tp.port1,
                                            m_tp.range,
                                            m_tp.step,
                                            m_tp.range2,
                                            m_tp.step2,
                                            m_tp.thres);
                }


                else if (cmd == align.ROLLOUT)
                {

                    if (m_tp.tlsForRoll == true)
                    {
						align.RollOut(m_tp.port1,
									m_tp.port2,
									m_tp.rollDist,
									m_tp.tls,
									m_tp.wave1,
									m_tp.wave2,
									m_tp.range,
									m_tp.step,
									m_tp.thres);

                    }
                    else
                    {
						align.RollOut(m_tp.port1,
									m_tp.port2,
									m_tp.rollDist,
									m_tp.range,
									m_tp.step,
									m_tp.thres);
					}
                    
                }


                else if (cmd == align.AXISSEARCH)
                {
                    align.AxisSearch(m_tp.stageNo,
                                        m_tp.axis,
                                        m_tp.port1,
                                        m_tp.range,
                                        m_tp.step);
                }

                else if (cmd == align.SYNCXYSEARCH)
                {
                    align.SyncXySearch(m_tp.port2,
                                        m_tp.range,
                                        m_tp.step,
                                        m_tp.thres);
                }

            }
                


        }//while


    }//private void ThreadFunc()


    #endregion




    
    #region Private method


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
            if (conf != null)
                conf.Dispose();

            conf = null;
        }




        return ret;
    }



    
    //////////////////////////////////////////////////////////////
    //EnableWnd ///////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - 모두다 살린다.
    //
    private void EnableWnd()
    {

        grpPassive.Enabled = true;
        TabControl1.Enabled = true;

        grpPassive.Refresh();
        TabControl1.Refresh();

    }




    //////////////////////////////////////////////////////////////
    //DisableWndButStop ////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - Stop 버튼만 살리고 다 죽인다.
    //
    private void DisableWndButStop()
    {

        grpPassive.Enabled = false;
        TabControl1.Enabled = false;

        grpPassive.Refresh();
        TabControl1.Refresh();

    }



    #endregion




    
    #region public method


    //////////////////////////////////////////////////////////////
    //StopOperation //////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - Stop Alignment 
    //
    public void StopOperation()
    {
        if (m_align == null)
            return;


        if (m_align.IsCompleted() == true)
            return;



        m_align.StopOperation();
        while (m_align.IsCompleted())
        {
            Thread.Sleep(10);
        }


        EnableWnd();
        ToolStripStatusLabel1.Text = "명령이 중지 됨";

    }


    #endregion



   

    /// <summary>
    /// initalize form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAlignment_Load(object sender, EventArgs e)
    {

        m_leftStage = (Istage)(CGlobal.g_leftStage);
        m_rightStage = (Istage)(CGlobal.g_rightStage);
        m_mpm = CGlobal.g_mpm;
        m_tls = CGlobal.g_tls;
        m_align = CGlobal.g_align;
        

        //combox 설정.
        cbDigitalPort_F.Items.Clear();
        cbDigitalPort_L.Items.Clear();
        try
        {
            for (int i = 1; i <= m_mpm.portCnt; i++)
            {
                cbDigitalPort_F.Items.Insert(i - 1, Convert.ToString(i));
                cbDigitalPort_L.Items.Insert(i - 1, Convert.ToString(i));
            }
        }
        catch
        {
            cbDigitalPort_F.Text = "???";
            cbDigitalPort_L.Text = "???";
        }

        cbAxisScanPort.Items.Clear();   //axis scan port
        try
        {
            for (int i = 1; i <= m_mpm.portCnt; i++)
            {
                cbAxisScanPort.Items.Insert(i - 1, Convert.ToString(i));
            }
        }
        catch
        {
            //do nothing.
        }



        //load config & dislay.
        string confFilepath = Application.StartupPath + "\\conf_alignment.xml";
        this.Location = LoadWndStartPos(confFilepath);

        Cconfig conf = null; 
        try
        {
            string temp = "";

            conf = new Cconfig(confFilepath);


            //digital
            cbDigitalPort_F.Text = conf.GetValue("DIGITAL_PORT_FIRST");
            cbDigitalPort_L.Text = conf.GetValue("DIGITAL_PORT_LAST");
            cbSyncSearchRngDigital.Text = conf.GetValue("DIGITAL_SYNCRANGE");
            cbSyncSearchStepDigital.Text = conf.GetValue("DIGITAL_SYNCSTEP");
            cbBlindRangeDigital.Text = conf.GetValue("DIGITAL_BLINDRANGE");
            cbBlindStepDigital.Text = conf.GetValue("DIGITAL_BLINDSTEP");
            cbFblindInRng.Text = conf.GetValue("FULLBLINDINRANGE");
            cbFblindInStep.Text = conf.GetValue("FULLBLINDINSTEP");
            cbFblindOutRng.Text = conf.GetValue("FULLBLINDOUTRANGE");
            cbFblindOutStep.Text = conf.GetValue("FULLBLINDOUTSTEP");
            cbBlindStepDigital.Text = conf.GetValue("DIGITAL_BLINDSTEP");

            txtThresDigital.Text = conf.GetValue("DIGITAL_THRES");
            txtDigitalRollDistIn.Text = conf.GetValue("DIGITAL_ROLLDISTIN");
            txtDigitalRollDistOut.Text = conf.GetValue("DIGITAL_ROLLDISTOUT");
            temp = conf.GetValue("DIGITAL_ROLLFORTLS");

            if (temp == "1")
                chkTlsForRoll.Checked = true;
            else
                chkTlsForRoll.Checked = false;

            txtDigitalRollTlsWaveFirst.Text = conf.GetValue("DIGITAL_ROLLTLSWAVE_FIRST");
            txtDigitalRollTlsWaveLast.Text = conf.GetValue("DIGITAL_ROLLTLSWAVE_LAST");
            txtRollRng.Text = conf.GetValue("ROLLRNG");
            textRollStep.Text = conf.GetValue("ROLLSTEP");
            txtRollPostcond.Text = conf.GetValue("ROLLPOSTCOND");


            //Axis
            cbAxisScanStage.Text = conf.GetValue("AXISSCAN_STAGE");
            cbAxisScanAxis.Text = conf.GetValue("AXISSCAN_AXIS");
            cbAxisScanPort.Text = conf.GetValue("AXISSCAN_PORT");
            cbAxisScanRange.Text = conf.GetValue("AXISSCAN_RANGE");
            cbAxisStep.Text = conf.GetValue("AXISSCAN_STEP");

        }
        catch
        {
            MessageBox.Show("설정값을 불러오든데 실패!! \n기본값 사용.",
                            "에러",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            cbDigitalPort_F.Text = DEFAULT_DIGIPORT1.ToString() ;
            cbDigitalPort_L.Text = DEFAULT_DIGIPORT2.ToString();

            cbSyncSearchRngDigital.Text = DEFAULT_FASTRNG.ToString();
            cbBlindRangeDigital.Text = DEFAULT_BLINDRNG.ToString();
            cbBlindStepDigital.Text = DEFAULT_BLINDSTEP.ToString();
            txtThresDigital.Text = DEFAULT_BLINDTHRES.ToString();
            txtDigitalRollDistIn.Text = DEFAULT_ROLLDISTIN.ToString();
            txtDigitalRollDistOut.Text = DEFAULT_ROLLDISTOUT.ToString();
            chkTlsForRoll.Checked = DEFAULT_TLSFORROLL;
            txtDigitalRollTlsWaveFirst.Text = DEFAULT_TLSROLLWAVE1.ToString();
            txtDigitalRollTlsWaveLast.Text = DEFAULT_TLSROLLWAVE2.ToString();
            txtRollRng.Text = DEFAULT_ROLLRNG.ToString();
            textRollStep.Text = DEFAULT_ROLLSTEP.ToString();
            txtRollPostcond.Text = DEFAULT_ROLLPOSTCOND.ToString();

            cbAxisScanStage.Text = "INPUT";
            cbAxisScanAxis.Text = DEFAULT_AXISSCAN_AXIS.ToString();
            cbAxisScanPort.Text = DEFAULT_AXISSCAN_PORT.ToString();
            cbAxisScanRange.Text = DEFAULT_AXISSCAN_RNG.ToString();
            cbAxisStep.Text = DEFAULT_AXISSCAN_STEP.ToString();


        }
        finally
        {
            if (conf != null)
            {
                conf.Dispose();
                conf = null;
            }
        }




        //쓰레드 가동.
        try
        {
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
        }
        catch
        {
            m_tp.port2 = 5;
        }

        m_tp.cmd = m_align.NOOPERATION;
        m_autoEvent = new AutoResetEvent(false);
        m_thread = new Thread(ThreadFunc);
        m_thread.Start();

        


        //Alignment Status 창을 띄움.
        if ( Application.OpenForms.OfType<frmAlignStatus>().Count() == 0 )
        {
            frmAlignStatus frm = new frmAlignStatus();
            frm.MdiParent = Application.OpenForms["frmMain"];
            frm.Show();
            frm.Refresh();
        }


    }



    
    /// <summary>
    /// 폼을 마무리 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAlignment_FormClosing(object sender, FormClosingEventArgs e)
    {

        StopOperation();


        //save configs.
        string confFilepath = Application.StartupPath + "\\conf_alignment.xml";
        SaveWndStartPos(confFilepath);

        Cconfig conf = null;

        try
        {
            conf = new Cconfig(confFilepath);
            conf.SetValue("DIGITAL_PORT_FIRST", cbDigitalPort_F.Text);
            conf.SetValue("DIGITAL_PORT_LAST", cbDigitalPort_L.Text);
            conf.SetValue("DIGITAL_SYNCRANGE", cbSyncSearchRngDigital.Text);
            conf.SetValue("DIGITAL_SYNCSTEP", cbSyncSearchStepDigital.Text);
            conf.SetValue("DIGITAL_BLINDRANGE", cbSyncSearchRngDigital.Text);
            conf.SetValue("DIGITAL_BLINDSTEP", cbBlindStepDigital.Text);

            conf.SetValue("FULLBLINDINRANGE", cbFblindInRng.Text);
            conf.SetValue("FULLBLINDINSTEP", cbFblindInStep.Text);
            conf.SetValue("FULLBLINDOUTRANGE", cbFblindOutRng.Text);
            conf.SetValue("FULLBLINDOUTSTEP", cbFblindOutStep.Text);

            conf.SetValue("DIGITAL_THRES", txtThresDigital.Text);
            conf.SetValue("DIGITAL_ROLLDISTIN", txtDigitalRollDistIn.Text);
            conf.SetValue("DIGITAL_ROLLDISTOUT", txtDigitalRollDistOut.Text);

            if (chkTlsForRoll.Checked == true)
                conf.SetValue("DIGITAL_ROLLFORTLS", "1");
            else
                conf.SetValue("DIGITAL_ROLLFORTLS", "0");

            conf.SetValue("DIGITAL_ROLLTLSWAVE_FIRST", txtDigitalRollTlsWaveFirst.Text);
            conf.SetValue("DIGITAL_ROLLTLSWAVE_LAST", txtDigitalRollTlsWaveLast.Text);

            conf.SetValue("ROLLRNG", txtRollRng.Text);
            conf.SetValue("ROLLSTEP", textRollStep.Text);
            conf.SetValue("ROLLPOSTCOND", txtRollPostcond.Text);

            conf.SetValue("AXISSCAN_STAGE", cbAxisScanStage.Text);
            conf.SetValue("AXISSCAN_AXIS", cbAxisScanAxis.Text);
            conf.SetValue("AXISSCAN_PORT", cbAxisScanPort.Text);
            conf.SetValue("AXISSCAN_RANGE", cbAxisScanRange.Text);
            conf.SetValue("AXISSCAN_STEP", cbAxisStep.Text);
        }
        catch
        {
            //do nothing.
        }
        finally
        {
            if (conf != null)
            {
                conf.Dispose();
                conf = null;
            }  
        }

        


        //thread 종료 및 마무리.
        if (m_thread != null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        if (m_autoEvent != null)
            m_autoEvent.Dispose();
        m_autoEvent = null;


        m_mpm = null;
        m_align = null;
        m_leftStage = null;
        m_rightStage = null;

    }



  
    /// <summary>
    /// ZApproach (Input)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZapp_L_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

            

        try
        {

            if (m_align.IsCompleted() == false)
                return;



            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "ZApproach _ LEFT ...";



            //ZApproach...
            m_tp.cmd = ((IAlignmentFa)m_align).ZAPPROACH_SINGLE;
            m_tp.stageNo = m_align.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //완료대기
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }

            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L,m_leftStage.AXIS_Z);
            }


            //resotre windows state.
            ToolStripStatusLabel1.Text = "ZApproach _ LEFT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }


    }



 
    /// <summary>
    /// ZApproach (Output)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZapp_R_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();


        try
        {
            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "ZApproach _ RIGHT ...";


            //Zapproach
            m_tp.cmd = ((IAlignmentFa)m_align).ZAPPROACH_SINGLE;
            m_tp.stageNo = m_align.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //상태 출력 및 완료대기 
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }



            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R,m_rightStage.AXIS_Z);
            }


            //restore winodw state.
            ToolStripStatusLabel1.Text = "ZApproach _ RIGHT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }


    }




    /// <summary>
    /// FA Arrange Y _ Left Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARY_L_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "FA Arrangement Ty_ LEFT...";


            //Operation
            m_tp.cmd = ((IAlignmentFa)m_align).FAARRANAGE_TY_SINGLE;
            m_tp.stageNo = m_align.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_TY);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Z);
            }
                
            //Dispaly sensing display
            ToolStripStatusLabel1.Text = "FARY _ LEFT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }




    /// <summary>
    /// FA Arrange... right 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARY_R_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();



        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "FA Arrangement Ty_ RIGHT...";


            //Operation
            m_tp.cmd = ((IAlignmentFa)m_align).FAARRANAGE_TY_SINGLE;
            m_tp.stageNo = m_align.RIGHT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_TY);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_Z);
            }

            //Dispaly sensing display
            ToolStripStatusLabel1.Text = "FARY _ RIGHT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }
    }




    /// <summary>
    /// FA Arrange X _ Left Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARX_L_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();



        try
        {

            if (m_align.IsCompleted() == false)
                return;



            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "FA Arrangement Tx_ LEFT...";


            //Operation
            m_tp.cmd = ((IAlignmentFa)m_align).FAARRANAGE_TX_SINGLE;
            m_tp.stageNo = m_align.STAGE_L;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_TX);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Z);
            }

            //Dispaly sensing display
            ToolStripStatusLabel1.Text = "FA Arrangement Tx_ LEFT... completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();



        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }



    
    /// <summary>
    /// FA Arrange X _ Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARX_R_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;



            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "FA Arrangement Tx_ RIGHT...";


            //Operation
            m_tp.cmd = ((IAlignmentFa)m_align).FAARRANAGE_TX_SINGLE;
            m_tp.stageNo = m_align.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_TX);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_Z);
            }

            //Dispaly sensing display
            ToolStripStatusLabel1.Text = "FA Arrangement Tx_ RIGHT... completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();



        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }


    }




    /// <summary>
    /// Left Stage Blind Search.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBlind_L_Digital_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();



        try
        {


            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "Blind Search_ LEFT...";



            //Blind Search
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).XYBLINDSEARCH;
            else
                throw new ApplicationException();

            m_tp.stageNo = m_align.STAGE_L;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text);      //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);       //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);         //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);     //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);



            //Display alignment status
            while (false == m_align.IsCompleted())
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if ( frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Y);
            }


            //restore winodws state.
            ToolStripStatusLabel1.Text = "Blind Search _LEFT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }


    }




    /// <summary>
    /// Right Stage Blind Search.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBlind_R_Digital_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "Blind Search_RIGHT...";


            //Blind Search
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).XYBLINDSEARCH;
            else
                throw new ApplicationException();

            m_tp.stageNo = m_align.STAGE_R;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text); //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);   //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);    //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text); //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //Display alignment status
            while (m_align.IsCompleted()== false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_leftStage.AXIS_Y);
            }


            //resotre windows state.
            ToolStripStatusLabel1.Text = "Blind Search_RIGHT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();



        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }




    /// <summary>
    /// Fine L (Digital)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFINE_L_Digital_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;

        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();


            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "XySearch _ LEFT ...";



            //fine xySearch.
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).XYSEARCH;
            else
                throw new ApplicationException();

            m_tp.stageNo = m_align.STAGE_L;
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //wait and display alignment status 
            while (false == m_align.IsCompleted())
            {
                Application.DoEvents();
            }



            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Y);
            }


            //restore windows state.
            ToolStripStatusLabel1.Text = "XySearch _ LEFT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }




    /// <summary>
    /// Fine R (Digital)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFINE_R_Digital_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        frmDigitalOptPowermeter frmDigitalPwr = null;
        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();
            
        frmStageControl frmStageCont = null;
        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        frmAlignStatus frmStatus = null;
        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();


        frmSourceController frmSourCont = null;
        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();


            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "XySearch _  RIGHT ...";


            //fine xySearch.
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).XYSEARCH;
            else
                throw new ApplicationException();
            m_tp.stageNo = m_align.STAGE_R;
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //wait and display alignment status 
            while (false == m_align.IsCompleted())
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_leftStage.AXIS_Y);
            }


            //Display On
            ToolStripStatusLabel1.Text = "XySearch _ RIGHT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();


        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }


    

    /// <summary>
    /// Roll alignment (right)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRoll_R_Digital_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();



        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "ROLL Out...";




            //roll alignment
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).ROLLOUT;
            else
                throw new ApplicationException();

            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.port2 = Convert.ToInt32(cbDigitalPort_L.Text);
            m_tp.rollDist = Convert.ToInt32(txtDigitalRollDistOut.Text);
            m_tp.tlsForRoll = chkTlsForRoll.Checked;
            m_tp.tls = m_tls;
            m_tp.wave1 = Convert.ToInt32(txtDigitalRollTlsWaveFirst.Text);
            m_tp.wave2 = Convert.ToInt32(txtDigitalRollTlsWaveLast.Text);
            m_tp.range = Convert.ToInt32(txtRollRng.Text);
            m_tp.step = Convert.ToDouble(textRollStep.Text);
            m_tp.thres = Convert.ToDouble(txtRollPostcond.Text);

            m_autoEvent.Set();
            Thread.Sleep(100);


            //wait and display alignment status 
            while (!m_align.IsCompleted())
            {
                Application.DoEvents();
            }



            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_Y);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_TZ);
            }

            //Display completed message an
            ToolStripStatusLabel1.Text = "ROLL Out ... completed!!";


        }
        catch (Exception)
        {
            ToolStripStatusLabel1.Text = "Error!!";
        }
        finally
        {

            //resotre other's windows state.
            this.Cursor = Cursors.Default;
            EnableWnd();

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();

        }



    }




    /// <summary>
    /// 명령을 중지한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {

        if (m_align.IsCompleted() == true)
            return;


        //stop
        m_align.StopOperation();
        while (m_align.IsCompleted() == false)
        {
            Thread.Sleep(10);
        }


        //완료!!
        this.Cursor = Cursors.Default;
        EnableWnd();
        ToolStripStatusLabel1.Text = "명령이 중지 됨";

    }




    /// <summary>
    /// Axis Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAxisScan_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "Axis Search ...";



            //operation
            m_tp.cmd = ((IAlignmentDigital)m_align).AXISSEARCH;
            if (cbAxisScanStage.Text == "INPUT")
                m_tp.stageNo = m_align.STAGE_L;
            else
                m_tp.stageNo = m_align.STAGE_R;

            m_tp.port1 = Convert.ToInt32(cbAxisScanPort.Text);

            if (cbAxisScanAxis.Text == "X")
                m_tp.axis = m_leftStage.AXIS_X;
            else
                m_tp.axis = m_leftStage.AXIS_Y;

            m_tp.range = Convert.ToInt32(cbAxisScanRange.Text);
            m_tp.step = Convert.ToInt32(cbAxisStep.Text);

            m_autoEvent.Set();
            Thread.Sleep(100);


            // 정렬정보 출력 및  완료 대기!!
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null) 
                frmStageCont.UpdateAxisPos(m_tp.stageNo, m_tp.axis);


            //restore window state.
            ToolStripStatusLabel1.Text = "Axis Search...  completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();



            //AxisSearch 그래프 Refresh.!!
            frmAxisSearchGraph frm = null;
            if (Application.OpenForms.OfType<frmAxisSearchGraph>().Count() ==0 )
                frm =  new frmAxisSearchGraph();
            else
                frm = Application.OpenForms.OfType<frmAxisSearchGraph>().FirstOrDefault();

            if (frm == null)
            {
                frm.MdiParent = Application.OpenForms["frmMain"];
                frm.Show();
            }


            CsearchStatus state = null;
            if (cbAxisScanStage.Text == "INPUT")
            {
                if (cbAxisScanAxis.Text == "X")
                    state = CalignStatRes.axisSearchInX;
                else
                    state = CalignStatRes.axisSearchInY;
            }
            else
            {
                if (cbAxisScanAxis.Text == "X")
                    state = CalignStatRes.axisSearchOutX;
                else
                    state = CalignStatRes.axisSearchOutY;
            }

            double startPos = 0;
            double stepPos = 0;
            if (cbAxisScanAxis.Text == "X")
            {
                startPos = Math.Round(state.posList[0].x, STGPOSXYZRES);
                stepPos = Math.Round(state.posList[1].x - state.posList[0].x);
            }
            else
            {
                startPos = Math.Round(state.posList[0].y, STGPOSXYZRES);
                stepPos = Math.Round(state.posList[1].y - state.posList[0].y);
            }
            frm.Plot(startPos, stepPos, state.pwrList.ToArray());



        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }


    }




    /// <summary>
    /// 스캔한 내용을 plot한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAxisPlot_Click(object sender, EventArgs e)
    {

        //open window
        frmAxisSearchGraph frm = null;
        if (Application.OpenForms.OfType<frmAxisSearchGraph>().Count() == 0)
            frm = new frmAxisSearchGraph();
        else
            frm = Application.OpenForms.OfType<frmAxisSearchGraph>().FirstOrDefault();


        if (frm != null)
        {
            frm.MdiParent = Application.OpenForms["frmMain"];
            frm.Show();
        }




        //plot
        try
        {

            CsearchStatus state = null;
            if (cbAxisScanStage.Text == "INPUT")
            {
                if (cbAxisScanAxis.Text == "X")
                    state = CalignStatRes.axisSearchInX;
                else
                    state = CalignStatRes.axisSearchInY;

            }
            else
            {
                if (cbAxisScanAxis.Text == "X")
                    state = CalignStatRes.axisSearchOutX;
                else
                    state = CalignStatRes.axisSearchOutY;
            }


            double startPos = 0;
            double stepPos = 0;
            if (cbAxisScanAxis.Text == "X")
            {
                startPos = Math.Round(state.posList[0].x, STGPOSXYZRES);
                stepPos = Math.Round(state.posList[1].x - state.posList[0].x, STGPOSXYZRES);
            }
            else
            {
                startPos = Math.Round(state.posList[0].y, STGPOSXYZRES);
                stepPos = Math.Round(state.posList[1].y - state.posList[0].y, STGPOSXYZRES);
            }
            frm.Plot(startPos, stepPos, state.pwrList.ToArray());

        }
        catch { /* do nothing */ }


    }




    /// <summary>
    /// SyncXySearch를 실행한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSyncSearch_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();


            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "SyncXySearch...";


            //Blind Search
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).SYNCXYSEARCH;
            else
                throw new ApplicationException();

            m_tp.stageNo = m_align.STAGE_LR;
            m_tp.range = Convert.ToInt32(cbSyncSearchRngDigital.Text);  //Search Range [um]
            m_tp.step = Convert.ToInt32(cbSyncSearchStepDigital.Text);   //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);         //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);         //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //Display alignment status
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Y);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_Y);
            }


            //resotre windows state.
            ToolStripStatusLabel1.Text = "SyncXySearch completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();

        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }




    /// <summary>
    /// fullblind search 실행.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFullBlind_Click(object sender, EventArgs e)
    {

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<frmStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (m_align.IsCompleted() == false)
                return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            if (frmSourCont != null)
                frmSourCont.DisableForm();


            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            ToolStripStatusLabel1.Text = "XyFullBlindSearch...";


            //XyFullBlind Search
            if (m_align is IAlignmentDigital)
                m_tp.cmd = ((IAlignmentDigital)m_align).XYFULLBLINDSEARCH;
            else
                throw new ApplicationException();

            m_tp.stageNo = m_align.STAGE_LR;
            m_tp.range = Convert.ToInt32(cbFblindInRng.Text);  //Search Range inputside [um]
            m_tp.step = Convert.ToDouble(cbFblindInStep.Text);   //Search Step inputside [um] 
            m_tp.range2 = Convert.ToInt32(cbFblindOutRng.Text);  //Search Range outputside [um]
            m_tp.step2 = Convert.ToDouble(cbFblindOutStep.Text);   //Search Step outputside [um] 
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);     //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);     //detect port!!

            m_autoEvent.Set();
            Thread.Sleep(100);


            //Display alignment status
            while (m_align.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_L, m_leftStage.AXIS_Y);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_X);
                frmStageCont.UpdateAxisPos(m_align.STAGE_R, m_rightStage.AXIS_Y);
            }


            //resotre windows state.
            ToolStripStatusLabel1.Text = "XyFullBlindSearch completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null)
                frmDistSens.StartSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOn();

            if (frmSourCont != null)
                frmSourCont.EnableForm();

        }
        catch (Exception ex)
        {
            ToolStripStatusLabel1.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }


}


