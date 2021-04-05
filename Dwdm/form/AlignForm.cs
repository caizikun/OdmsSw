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
using Free302.MyLibrary.Utility;
using al = Neon.Aligner.AlignLogic;

public partial class AlignForm : Form           //,IFormCanClosed
{

    #region definition

    private const int ALIGNRES = 1;             //alignment resolution [um]
    private const int STGPOSXYZRES = 2;         //XYZ position Stage resolution.


    private int DEFAULT_DIGIPORT1 = 1;          //digital port first.
    private int DEFAULT_DIGIPORT2 = 2;          //digital port last.
    private int DEFAULT_FASTRNG = 20;           //fast search range. [um]
    private int DEFAULT_BLINDRNG = 100;         //blind range [um]
    private int DEFAULT_BLINDSTEP = 4;          //blind step [um]
    private int DEFAULT_BLINDTHRES = -25;       //blind searcg threshold power. [dBm]
    private int DEFAULT_ROLLDISTIN = 0;
    private int DEFAULT_ROLLDISTOUT = 889;
    private bool DEFAULT_TLSFORROLL = false;
    private int DEFAULT_TLSROLLWAVE1 = 1271;    //TLS for Roll wavlenegth first.
    private int DEFAULT_TLSROLLWAVE2 = 1331;    //TLS for Roll wavlenegth last.
    private int DEFAULT_ROLLRNG = 20;
    private int DEFAULT_ROLLSTEP = 1;
    private double DEFAULT_ROLLPOSTCOND = 0.5;


    private string DEFAULT_AXISSCAN_AXIS = "X"; //x axis
    private int DEFAULT_AXISSCAN_PORT = 1;
    private int DEFAULT_AXISSCAN_RNG = 20;      // [um]
    private int DEFAULT_AXISSCAN_STEP = 1;      // [um]

    #endregion




    #region Structure

    private struct ThreadParam
    {
        public int cmd;
        public int stageNo;
        public int axis;
        public int port1;
        public int port2;
        public int range;                       //Search Range.
        public double step;
        public double thres;                    //thresshold power.[dBm] or post condition [um] for alignment
        public int rollDist;
        public bool tlsForRoll;                 //tls for roll
        public Itls tls;
        public double wave1;                    //TLS Roll 첫번째 파장.
        public double wave2;                    //TLS 마지막 파장.

        public XYSearchParam XyParam;
    }

    #endregion




    #region Private member variables

    private Istage m_leftStage;
    private Istage m_rightStage;
    private Itls m_tls;                         // for tlsRoll 

    private IoptMultimeter m_mpm;
    private AlignLogic mAlign;

    private AutoResetEvent m_autoEvent;
    private Thread m_thread;
    private ThreadParam m_tp;

    #endregion




    #region Property

    public int alignPort { get { return m_tp.port1; } }

    #endregion




    #region consturctor/destructor

    //생성자.
    public AlignForm()
    {
        InitializeComponent();

        this.Load += form_Load;
    }


    #endregion




    #region Thread function


    bool mMoveToCenter = false;

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

            // fa alignemnt //
            int sn = m_tp.stageNo; //stage no.

            if (cmd == al.ZAPPROACH_SINGLE) mAlign.ZappSingleStage(sn);
            else if (cmd == al.ZAPPROACH_DUAL) mAlign.ApproachInOut(0, 0);
            else if (cmd == al.ANGLE_TY_SINGLE) mAlign.AngleTy(sn);
            else if (cmd == al.ANGLE_TX_SINGLE) mAlign.AngleTx(sn);

            if (cmd == al.XY_SEARCH + 10000) mAlign.XySearch(m_tp.XyParam);

            else if (cmd == al.XY_SEARCH) mAlign.XySearch(m_tp.stageNo, m_tp.port1, m_tp.step);

            else if (cmd == al.XYBLINDSEARCH) mAlign.XyBlindSearch(m_tp.stageNo, m_tp.port2, m_tp.range, m_tp.step, m_tp.thres);

            else if (cmd == al.ROLLOUT)
            {
                if (m_tp.tlsForRoll == true)
                    mAlign.RollOut(m_tp.port1, m_tp.port2, m_tp.rollDist, m_tp.tls, m_tp.wave1, m_tp.wave2, m_tp.range, m_tp.step, m_tp.thres);
                else
                    mAlign.RollOut(m_tp.port1, m_tp.port2, m_tp.rollDist, m_tp.range, m_tp.step, m_tp.thres);
            }

            else if (cmd == al.AXISSEARCH) mAlign.AxisSearch(m_tp.stageNo, m_tp.axis, m_tp.port1, m_tp.range, m_tp.step, mMoveToCenter);
            else if (cmd == al.SYNCXYSEARCH) mAlign.SyncXySearch(m_tp.port2, m_tp.range, m_tp.step, m_tp.thres);

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




    #region Public method


    //////////////////////////////////////////////////////////////
    //StopOperation //////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - Stop Alignment 
    //
    public void StopOperation()
    {
        if (mAlign == null)
            return;


        if (mAlign.IsCompleted() == true)
            return;



        mAlign.StopOperation();
        while (mAlign.IsCompleted())
        {
            Thread.Sleep(10);
        }


        EnableWnd();
        uiStatus.Text = "명령이 중지 됨";

    }


    #endregion




    public const string CONFIG_FILE_NAME = @"config\conf_alignment.xml";
    static string mConfigFile;
    XConfig mConfig;

    /// <summary>
    /// initalize form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAlignment_Load(object sender, EventArgs e)
    {
        mConfigFile = System.IO.Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
        mConfig = new XConfig(mConfigFile);

        m_leftStage = CGlobal.LeftAligner;
        m_rightStage = CGlobal.RightAligner;
        m_mpm = CGlobal.Pm8164;
        m_tls = CGlobal.Tls8164;
        mAlign = CGlobal.Alignment;

        if (mAlign != null) mAlign.mReporter += writeStatus;
        if (m_leftStage != null) CGlobal.XySearchParamLeft.StageNo = m_leftStage.stageNo;
        if (m_rightStage != null) CGlobal.XySearchParamRight.StageNo = m_rightStage.stageNo;

        //combox 설정.
        cbDigitalPort_F.Items.Clear();
        cbDigitalPort_L.Items.Clear();
		cbAxisScanPort.Items.Clear();   //axis scan port
		cbDigitalPort_F.Items.AddRange((m_mpm?.ChList) ?? Null_Ch_List);
		cbDigitalPort_L.Items.AddRange((m_mpm?.ChList) ?? Null_Ch_List);
		cbAxisScanPort.Items.AddRange((m_mpm?.ChList) ?? Null_Ch_List);

        //load config & dislay.
        Location = LoadWndStartPos(mConfigFile);
        loadConfig();


        //쓰레드 가동.
        try
        {
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
        }
        catch
        {
            m_tp.port2 = 5;
        }

        m_tp.cmd = al.NOOPERATION;
        m_autoEvent = new AutoResetEvent(false);
        m_thread = new Thread(ThreadFunc);
        m_thread.Start();

        //Alignment Status 창을 띄움.
        //MyLogic.ShowOrNew<frmAlignStatus>();
    }



    private void loadConfig()
    {

        try
        {
            string temp = "";

            //digital
            cbDigitalPort_F.Text = mConfig.GetValue("DIGITAL_PORT_FIRST");
            cbDigitalPort_L.Text = mConfig.GetValue("DIGITAL_PORT_LAST");
            cbSyncSearchRngDigital.Text = mConfig.GetValue("DIGITAL_SYNCRANGE");
            cbSyncSearchStepDigital.Text = mConfig.GetValue("DIGITAL_SYNCSTEP");
            cbBlindRangeDigital.Text = mConfig.GetValue("DIGITAL_BLINDRANGE");
            cbBlindStepDigital.Text = mConfig.GetValue("DIGITAL_BLINDSTEP");
            txtThresDigital.Text = mConfig.GetValue("DIGITAL_THRES");
            txtDigitalRollDistIn.Text = mConfig.GetValue("DIGITAL_ROLLDISTIN");
            txtDigitalRollDistOut.Text = mConfig.GetValue("DIGITAL_ROLLDISTOUT");
            temp = mConfig.GetValue("DIGITAL_ROLLFORTLS");

            if (temp == "1")
                chkTlsForRoll.Checked = true;
            else
                chkTlsForRoll.Checked = false;

            txtRollWave1.Text = mConfig.GetValue("DIGITAL_ROLLTLSWAVE_FIRST");
            txtRollWave2.Text = mConfig.GetValue("DIGITAL_ROLLTLSWAVE_LAST");

            txtRollRange.Text = mConfig.GetValue("ROLLRNG");
            txtRollStep.Text = mConfig.GetValue("ROLLSTEP");
            txtRollThreshold.Text = mConfig.GetValue("ROLLPOSTCOND");


            //Axis
            cbAxisScanStage.Text = mConfig.GetValue("AXISSCAN_STAGE");
            cbAxisScanAxis.Text = mConfig.GetValue("AXISSCAN_AXIS");
            cbAxisScanPort.Text = mConfig.GetValue("AXISSCAN_PORT");
            cbAxisScanRange.Text = mConfig.GetValue("AXISSCAN_RANGE");
            cbAxisStep.Text = mConfig.GetValue("AXISSCAN_STEP");

            uiPeakSearchStep.Value = mConfig.GetValue("PEAK_SEARCH_STEP", 1.0).To<decimal>();
            ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;
        }
        catch
        {
            MessageBox.Show("설정값을 불러오든데 실패!! \n기본값 사용.",
                            "에러",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            cbDigitalPort_F.Text = DEFAULT_DIGIPORT1.ToString();
            cbDigitalPort_L.Text = DEFAULT_DIGIPORT2.ToString();

            cbSyncSearchRngDigital.Text = DEFAULT_FASTRNG.ToString();
            cbBlindRangeDigital.Text = DEFAULT_BLINDRNG.ToString();
            cbBlindStepDigital.Text = DEFAULT_BLINDSTEP.ToString();
            txtThresDigital.Text = DEFAULT_BLINDTHRES.ToString();
            CGlobal.AlignThresholdPower = (int)double.Parse(txtThresDigital.Text);

            txtDigitalRollDistIn.Text = DEFAULT_ROLLDISTIN.ToString();
            txtDigitalRollDistOut.Text = DEFAULT_ROLLDISTOUT.ToString();
            chkTlsForRoll.Checked = DEFAULT_TLSFORROLL;
            txtRollWave1.Text = DEFAULT_TLSROLLWAVE1.ToString();
            txtRollWave2.Text = DEFAULT_TLSROLLWAVE2.ToString();
            txtRollRange.Text = DEFAULT_ROLLRNG.ToString();
            txtRollStep.Text = DEFAULT_ROLLSTEP.ToString();
            txtRollThreshold.Text = DEFAULT_ROLLPOSTCOND.ToString();

            cbAxisScanStage.Text = "INPUT";
            cbAxisScanAxis.Text = DEFAULT_AXISSCAN_AXIS.ToString();
            cbAxisScanPort.Text = DEFAULT_AXISSCAN_PORT.ToString();
            cbAxisScanRange.Text = DEFAULT_AXISSCAN_RNG.ToString();
            cbAxisStep.Text = DEFAULT_AXISSCAN_STEP.ToString();

            mConfig.Save();
        }


    }
    


    /// <summary>
    /// 폼을 마무리 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;

        StopOperation();
        //save configs.
        SaveWndStartPos(mConfigFile);
        try
        {
            mConfig.SetValue("DIGITAL_PORT_FIRST", cbDigitalPort_F.Text);
            mConfig.SetValue("DIGITAL_PORT_LAST", cbDigitalPort_L.Text);
            mConfig.SetValue("DIGITAL_SYNCRANGE", cbSyncSearchRngDigital.Text);
            mConfig.SetValue("DIGITAL_SYNCSTEP", cbSyncSearchStepDigital.Text);
            mConfig.SetValue("DIGITAL_BLINDRANGE", cbSyncSearchRngDigital.Text);
            mConfig.SetValue("DIGITAL_BLINDSTEP", cbBlindStepDigital.Text);
            mConfig.SetValue("DIGITAL_THRES", txtThresDigital.Text);
            mConfig.SetValue("DIGITAL_ROLLDISTIN", txtDigitalRollDistIn.Text);
            mConfig.SetValue("DIGITAL_ROLLDISTOUT", txtDigitalRollDistOut.Text);

            if (chkTlsForRoll.Checked == true)
                mConfig.SetValue("DIGITAL_ROLLFORTLS", "1");
            else
                mConfig.SetValue("DIGITAL_ROLLFORTLS", "0");

            mConfig.SetValue("DIGITAL_ROLLTLSWAVE_FIRST", txtRollWave1.Text);
            mConfig.SetValue("DIGITAL_ROLLTLSWAVE_LAST", txtRollWave2.Text);

            mConfig.SetValue("ROLLRNG", txtRollRange.Text);
            mConfig.SetValue("ROLLSTEP", txtRollStep.Text);
            mConfig.SetValue("ROLLPOSTCOND", txtRollThreshold.Text);

            mConfig.SetValue("AXISSCAN_STAGE", cbAxisScanStage.Text);
            mConfig.SetValue("AXISSCAN_AXIS", cbAxisScanAxis.Text);
            mConfig.SetValue("AXISSCAN_PORT", cbAxisScanPort.Text);
            mConfig.SetValue("AXISSCAN_RANGE", cbAxisScanRange.Text);
            mConfig.SetValue("AXISSCAN_STEP", cbAxisStep.Text);
        }
        catch
        {
            //do nothing.
        }

        //thread 종료 및 마무리.
        if (m_thread != null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        if (m_autoEvent != null) m_autoEvent.Dispose();
        m_autoEvent = null;


        m_mpm = null;
        mAlign = null;
        m_leftStage = null;
        m_rightStage = null;

    }
    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }




    #region Button UI Handler


    /// <summary>
    /// ZApproach (Input)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZapp_L_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "ZApproach _ LEFT ...";


            //Operation...
            m_tp.cmd = al.ZAPPROACH_SINGLE;
            m_tp.stageNo = al.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //완료대기
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null) formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Z);

            //resotre windows state.
            uiStatus.Text = "ZApproach _ LEFT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show($"frmAlign.btnZapp_L_Click():\n{ex.Message}");
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }
    }




    /// <summary>
    /// ZApproach (Output)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZapp_R_Click(object sender, EventArgs e)
    {

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "ZApproach _ RIGHT ...";


            //Operation
            m_tp.cmd = al.ZAPPROACH_SINGLE;
            m_tp.stageNo = al.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //상태 출력 및 완료대기 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null) formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_Z);

            //restore winodw state.
            uiStatus.Text = "ZApproach _ RIGHT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.Message);
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }


    }




    /// <summary>
    /// FA Arrange Y _ Left Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARY_L_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "FA Arrangement Ty_ LEFT...";


            //Operation
            m_tp.cmd = al.ANGLE_TY_SINGLE;
            m_tp.stageNo = al.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_TY);
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Z);
            }

            //Dispaly sensing display
            uiStatus.Text = "FARY _ LEFT completed!!";
            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.Message);
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }
    }




    /// <summary>
    /// FA Arrange... right 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARY_R_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "FA Arrangement Ty_ RIGHT...";


            //Operation
            m_tp.cmd = al.ANGLE_TY_SINGLE;
            m_tp.stageNo = al.RIGHT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_TY);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_Z);
            }

            //Dispaly sensing display
            uiStatus.Text = "FARY _ RIGHT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }
    }




    /// <summary>
    /// FA Arrange X _ Left Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARX_L_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "FA Arrangement Tx_ LEFT...";


            //Operation
            m_tp.cmd = al.ANGLE_TX_SINGLE;
            m_tp.stageNo = al.STAGE_L;
            m_autoEvent.Set();
            Thread.Sleep(100);


            // 상태 출력 및 완료대기 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_TX);
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Z);
            }

            //Dispaly sensing display
            uiStatus.Text = "FA Arrangement Tx_ LEFT... completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }

    }




    /// <summary>
    /// FA Arrange X _ Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFARX_R_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var uiStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {

            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "FA Arrangement Tx_ RIGHT...";

            //Operation
            m_tp.cmd = al.ANGLE_TX_SINGLE;
            m_tp.stageNo = al.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(100);

            // 상태 출력 및 완료대기 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (uiStageControl != null)
            {
				uiStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_TX);
				uiStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_Z);
            }

            //Dispaly sensing display
            uiStatus.Text = "FA Arrangement Tx_ RIGHT... completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }


    }




    /// <summary>
    /// Left Stage Blind Search.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBlind_L_Digital_Click(object sender, EventArgs e)
    {

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "Blind Search_ LEFT...";


            //Blind Search
            m_tp.cmd = al.XYBLINDSEARCH;

            m_tp.stageNo = al.STAGE_L;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text);     //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);       //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);         //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);         //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //Display alignment status
            while (false == mAlign.IsCompleted()) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Y);
            }

            //restore winodws state.
            uiStatus.Text = "Blind Search _LEFT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            this.Cursor = Cursors.Default;
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

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "Blind Search_RIGHT...";

            //Blind Search 
            m_tp.cmd = al.XYBLINDSEARCH;
            m_tp.stageNo = al.STAGE_R;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text); //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);   //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);     //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);     //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);

            //Display alignment status
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_R, m_leftStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_leftStage.AXIS_Y);
            }

            //resotre windows state.
            uiStatus.Text = "Blind Search_RIGHT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }

    }




    /// <summary>
    /// Fine L (Digital)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFINE_L_Digital_Click(object sender, EventArgs e)
    {

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
            if (frmSourCont != null) frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "XySearch _ LEFT ...";


            //fine xySearch. 
            m_tp.cmd = al.XY_SEARCH;
            m_tp.stageNo = al.STAGE_L;
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);
            

            //wait and display alignment status 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Y);
            }

            //restore windows state.
            uiStatus.Text = "XySearch _ LEFT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
            if (frmSourCont != null) frmSourCont.EnableForm();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }

    }




    /// <summary>
    /// Fine R (Digital)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnFINE_R_Digital_Click(object sender, EventArgs e)
    {

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
            if (frmSourCont != null) frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "XySearch _  RIGHT ...";


            //fine xySearch.
            m_tp.cmd = al.XY_SEARCH;
            m_tp.stageNo = al.STAGE_R;
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //wait and display alignment status 
            while (false == mAlign.IsCompleted()) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_R, m_leftStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_leftStage.AXIS_Y);
            }

            //Display On
            uiStatus.Text = "XySearch _ RIGHT completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
            if (frmSourCont != null) frmSourCont.EnableForm();
            
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }

    }




    /// <summary>
    /// Roll alignment (right)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRoll_R_Digital_Click(object sender, EventArgs e)
    {

        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

        try
        {
            if (mAlign.IsCompleted() == false) return;
            SetRollParam();

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
            if (frmSourCont != null) frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "ROLL Out...";


            //roll alignment
            m_tp.cmd = al.ROLLOUT;

            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);
            m_tp.port2 = Convert.ToInt32(cbDigitalPort_L.Text);
            m_tp.rollDist = Convert.ToInt32(txtDigitalRollDistOut.Text);
            m_tp.tlsForRoll = chkTlsForRoll.Checked;
            m_tp.tls = m_tls;
            m_tp.wave1 = Convert.ToInt32(txtRollWave1.Text);
            m_tp.wave2 = Convert.ToInt32(txtRollWave2.Text);
            m_tp.range = Convert.ToInt32(txtRollRange.Text);
            m_tp.step = Convert.ToDouble(txtRollStep.Text);
            m_tp.thres = Convert.ToDouble(txtRollThreshold.Text);

            m_autoEvent.Set();
            Thread.Sleep(100);


            //wait and display alignment status 
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_Y);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_TZ);
            }

            //Display completed message an
            uiStatus.Text = "ROLL Out ... completed!!";

        }
        catch
        {
            uiStatus.Text = "Error!!";
        }
        finally
        {
            //resotre other's windows state.
            this.Cursor = Cursors.Default;
            EnableWnd();

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
            if (frmSourCont != null) frmSourCont.EnableForm();

        }


    }




    /// <summary>
    /// 명령을 중지한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        if (mAlign == null) return;
        if (mAlign.IsCompleted()) return;

        //stop
        mAlign.StopOperation();
        while (mAlign.IsCompleted() == false)
        {
            Thread.Sleep(10);
        }


        //완료!!
        this.Cursor = Cursors.Default;
        EnableWnd();
        uiStatus.Text = "명령이 중지 됨";
    }




    /// <summary>
    /// Axis Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAxisScan_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

        try
        {

            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
            if (frmSourCont != null) frmSourCont.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "Axis Search ...";


            //operation
            m_tp.cmd = al.AXISSEARCH;
            if (cbAxisScanStage.Text == "INPUT") m_tp.stageNo = al.STAGE_L;
            else m_tp.stageNo = al.STAGE_R;

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
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null) formStageControl.UpdateAxisPos(m_tp.stageNo, m_tp.axis);

            //restore window state.
            uiStatus.Text = "Axis Search...  completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
            if (frmSourCont != null) frmSourCont.EnableForm();


            //AxisSearch 그래프 Refresh.!!
            frmAxisSearchGraph frm = null;
            if (Application.OpenForms.OfType<frmAxisSearchGraph>().Count() == 0)
                frm = new frmAxisSearchGraph();
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
                    state = AlignStatusPool.axisSearchInX;
                else
                    state = AlignStatusPool.axisSearchInY;
            }
            else
            {
                if (cbAxisScanAxis.Text == "X")
                    state = AlignStatusPool.axisSearchOutX;
                else
                    state = AlignStatusPool.axisSearchOutY;
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
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
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
                    state = AlignStatusPool.axisSearchInX;
                else
                    state = AlignStatusPool.axisSearchInY;

            }
            else
            {
                if (cbAxisScanAxis.Text == "X")
                    state = AlignStatusPool.axisSearchOutX;
                else
                    state = AlignStatusPool.axisSearchOutY;
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
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            if (frmDistSens != null) frmDistSens.StopSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
            if (frmSourCont != null) frmSourCont.DisableForm();


            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "SyncXySearch...";


            //Blind Search 
            m_tp.cmd = al.SYNCXYSEARCH;
            m_tp.stageNo = al.STAGE_LR;
            m_tp.range = Convert.ToInt32(cbSyncSearchRngDigital.Text);   //Search Range [um]
            m_tp.step = Convert.ToInt32(cbSyncSearchStepDigital.Text);   //Search Step [um]
            m_tp.thres = Convert.ToInt32(txtThresDigital.Text);          //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbDigitalPort_F.Text);          //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(100);


            //Display alignment status
            while (mAlign.IsCompleted() == false) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_L, m_leftStage.AXIS_Y);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_X);
                formStageControl.UpdateAxisPos(al.STAGE_R, m_rightStage.AXIS_Y);
            }


            //resotre windows state.
            uiStatus.Text = "SyncXySearch completed!!";

            if (frmDistSens != null) frmDistSens.StartSensing();
            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
            if (frmSourCont != null) frmSourCont.EnableForm();

        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }

    } 

    #endregion

    
    

    #region ==== XySearch Param ====

    private void applySearchParam()
    {
        try
        {
            CGlobal.AlignThresholdPower = (int)double.Parse(txtThresDigital.Text);

            CGlobal.XySearchParamLeft.Port = CGlobal.XySearchParamRight.Port = uiAlignPmPort.Text.To<int>();


            CGlobal.XySearchParamLeft.RangeX = txt_range_left_x.Text.To<double>();
            CGlobal.XySearchParamLeft.StepX = txt_step_left_x.Text.To<double>();
            CGlobal.XySearchParamLeft.RangeY = txt_range_left_y.Text.To<double>();
            CGlobal.XySearchParamLeft.StepY = txt_step_left_y.Text.To<double>();
            CGlobal.XySearchParamLeft.SearchByScan = uiCheckScanLeft.Checked;
            CGlobal.XySearchParamLeft.ScanToCenter = uiCheckCenterLeft.Checked;

            CGlobal.XySearchParamRight.RangeX = txt_range_right_x.Text.To<double>();
            CGlobal.XySearchParamRight.StepX = txt_step_right_x.Text.To<double>();
            CGlobal.XySearchParamRight.RangeY = txt_range_right_y.Text.To<double>();
            CGlobal.XySearchParamRight.StepY = txt_step_right_y.Text.To<double>();
            CGlobal.XySearchParamRight.SearchByScan = uiCheckScanRight.Checked;
            CGlobal.XySearchParamRight.ScanToCenter = uiCheckCenterRight.Checked;

            ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;

            writeStatus("Param applied!");
        }
        catch { }
    }


    public void SetRollParam()
    {
        Action action = () =>
        {
			if (mAlign != null)
			{
				mAlign.SetRollParam(txtRollRange.Text.To<int>(), txtRollStep.Text.To<int>(), txtRollThreshold.Text.To<double>());
				mAlign.RollParam.UsingLocalTls = chkTlsForRoll.Checked;
				mAlign.RollParam.Wave1 = txtRollWave1.Text.To<double>();
				mAlign.RollParam.Wave2 = txtRollWave2.Text.To<double>();
				if (!CGlobal.mIsLocalTls && mAlign.RollParam.UsingLocalTls)
					throw new Exception("AlignForm.SetRollParam()\nTLS 서버 이용중에 TLS를 이용한 ROLL을 시도했습니다.");
			}
        };

        if (InvokeRequired) Invoke(action);
        else action();
    }


    private void refreshParamUi()
    {
        uiAlignPmPort.Text = CGlobal.XySearchParamLeft.Port.ToString();

        txt_range_left_x.Text = CGlobal.XySearchParamLeft.RangeX.ToString();
        txt_step_left_x.Text = CGlobal.XySearchParamLeft.StepX.ToString();
        txt_range_left_y.Text = CGlobal.XySearchParamLeft.RangeY.ToString();
        txt_step_left_y.Text = CGlobal.XySearchParamLeft.StepY.ToString();
        uiCheckScanLeft.Checked = CGlobal.XySearchParamLeft.SearchByScan;
        uiCheckCenterLeft.Checked = CGlobal.XySearchParamLeft.ScanToCenter;

        txt_range_right_x.Text = CGlobal.XySearchParamRight.RangeX.ToString();
        txt_step_right_x.Text = CGlobal.XySearchParamRight.StepX.ToString();
        txt_range_right_y.Text = CGlobal.XySearchParamRight.RangeY.ToString();
        txt_step_right_y.Text = CGlobal.XySearchParamRight.StepY.ToString();
        uiCheckScanRight.Checked = CGlobal.XySearchParamRight.SearchByScan;
        uiCheckCenterRight.Checked = CGlobal.XySearchParamRight.ScanToCenter;

        uiPeakSearchStep.Value = (decimal)ScanParam.FinePeakStep;
    }



    const string paramFileName = @"config\XySearchParam.txt";
    private void saveSearchParamToFile()
    {
        using (var w = new System.IO.StreamWriter(paramFileName, false))
        {
            w.WriteLine(CGlobal.XySearchParamLeft.Pack());
            w.WriteLine(CGlobal.XySearchParamRight.Pack());
            w.WriteLine(ScanParam.FinePeakStep.ToString());
            w.WriteLine($"{txtRollRange.Text};{txtRollStep.Text};{txtRollThreshold.Text}");
            w.Close();
        }
    }


    private void loadSearchParamFromFile()
    {
        if (!System.IO.File.Exists(paramFileName)) saveSearchParamToFile();
        using (var w = new System.IO.StreamReader(paramFileName))
        {
            CGlobal.XySearchParamLeft.Unpack(w.ReadLine());
            CGlobal.XySearchParamRight.Unpack(w.ReadLine());
            ScanParam.FinePeakStep = w.ReadLine().To<double>();

            var rollParam = w.ReadLine().Unpack<double>().ToArray();
            txtRollRange.Text = rollParam[0].ToString();
            txtRollStep.Text = rollParam[1].ToString();
            txtRollThreshold.Text = rollParam[2].ToString();

            w.Close();
        }
    }

    private void btnSaveParam_Click(object sender, EventArgs e)
    {
        try
        {
            applySearchParam();
            SetRollParam();
            saveSearchParamToFile();
            writeStatus($"Saved: {paramFileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{nameof(AlignForm)}.{nameof(btnSaveParam)}():\n{ex.Message}");
        }
    }


	static object[] Null_Ch_List = new object[] { };
    Dictionary<object, Control[]> mParamUi;
    private void form_Load(object sender, EventArgs e)
    {
        try
        {
            loadSearchParamFromFile();

            mParamUi = new Dictionary<object, Control[]>();
            mParamUi[uiCheckScanLeft] = new Control[] { txt_range_left_x, txt_range_left_y, txt_step_left_x, txt_step_left_y, uiCheckCenterLeft };
            mParamUi[uiCheckScanRight] = new Control[] { txt_range_right_x, txt_range_right_y, txt_step_right_x, txt_step_right_y, uiCheckCenterRight };

			uiAlignPmPort.Items.AddRange((m_mpm?.ChList) ?? Null_Ch_List);
            uiCheckScanLeft.CheckStateChanged += toggleParamUi;
            uiCheckScanRight.CheckStateChanged += toggleParamUi;
            //btnSaveParam.Click += btnSaveParam_Click;
            //btnLeftXy.Click += btnXy_Click;
            //btnRightXy.Click += btnXy_Click;

            refreshParamUi();
            toggleParamUi(uiCheckScanLeft, null);
            toggleParamUi(uiCheckScanRight, null);

            applySearchParam();
            SetRollParam();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{nameof(AlignForm)}.{nameof(form_Load)}():\n{ex.Message}");
        }
    }

    private void toggleParamUi(object sender, EventArgs e)
    {
        var alive = ((CheckBox)sender).Checked;
        if (mParamUi.ContainsKey(sender)) foreach (var c in mParamUi[sender]) c.Enabled = alive;
    }



    private void btnXy_Click(object sender, EventArgs e)
    {
        var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(false, false);
        var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(false, false);
        var formStageControl = MyLogic.CreateAndShow<uiStageControl>(false, false);
        var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = MyLogic.CreateAndShow<frmSourceController>(false, false);

        applySearchParam();
        XYSearchParam xyParam = (btnLeftXy.Equals(sender)) ? CGlobal.XySearchParamLeft : CGlobal.XySearchParamRight;

        var msg = "XY Search - ";
        msg += xyParam.Equals(CGlobal.XySearchParamLeft) ? "Left" : "Right";
        msg += xyParam.SearchByScan ? " - Scan" : " - Hill";

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            frmDistSens?.StopSensing();
            frmDigitalPwr?.DisplayOff();
            frmSourCont?.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            writeStatus($"Starting: {msg}");

            //param
            m_tp.cmd = al.XY_SEARCH + 10000;
            m_tp.XyParam = xyParam;
            //m_tp.stageNo = xyParam.StageNo;
            //m_tp.port1 = xyParam.Port;
            //m_tp.step = xyParam.StepY;
            m_autoEvent.Set();
            Thread.Sleep(100);

            //wait and display alignment status 
            while (false == mAlign.IsCompleted()) Application.DoEvents();

            //Update Stage Postion
            if (formStageControl != null)
            {
                formStageControl.UpdateAxisPos();
            }

            //restore windows state.
            writeStatus($"Complete: {msg}");

            frmDistSens?.StartSensing();
            frmDigitalPwr?.DisplayOn();
            frmSourCont?.EnableForm();
        }
        catch (Exception ex)
        {
            writeStatus($"Error: {msg}");
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
            this.Cursor = Cursors.Default;
        }
    }


    #endregion




    private void writeStatus(string msg)
    {
        Action func = () =>
        {
            uiStatus.Text = msg;
            uiStatusStrip.Refresh();
        };

        if (!uiStatusStrip.InvokeRequired) func();
        else uiStatusStrip.Invoke(func);
    }


    private void cbDigitalPort_F_SelectedIndexChanged(object sender, EventArgs e)
    {
        uiAlignPmPort.Text = cbDigitalPort_F.Text;
    }


    private void uiApplyPeakStep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;
    }


}


