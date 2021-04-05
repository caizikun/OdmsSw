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
using Free302.MyLibrary.Utility;
using al = Neon.Aligner.AlignLogic;

public partial class AlignForm : Form
{
    #region definition

    private const int ALIGNRES = 1;                 //alignment resolution [um]
    private const int STGPOSXYZRES = 2;             //XYZ position Stage resolution.

    private int DEFAULT_FASTRNG = 20;               //fast search range. [um]
    private int DEFAULT_BLINDRNG = 100;             //blind range [um]
    private int DEFAULT_BLINDSTEP = 4;              //blind step [um]
    private int DEFAULT_ROLLDISTOUT = 889;

    private int DEFAULT_ROLLRNG = 20;
    private int DEFAULT_ROLLSTEP = 1;
    private double DEFAULT_ROLLPOSTCOND = 0.5;

    #endregion


    #region Structure

    private struct ThreadParam
    {
        public int cmd;
        public string cmdName;
        public int stageNo;
        public int axis;
        public int port1;
        public int port2;
        public int range;				//Search Range.
        public int range2;
        public double step;
        public double step2;
        public double thres;			//thresshold power.[dBm] or post condition [um] for alignment
        public int rollDist;
        public bool tlsForRoll;         //tls for roll
        public Itls tls;
        public double wave1;            //TLS Roll 첫번째 파장.
        public double wave2;            //TLS 마지막 파장.
    }

    #endregion


    #region Private member variables

    private Istage mLeft;
    private Istage mRight;
    private Istage mOther;
    private Itls mTls; // for tlsRoll 
    private IoptMultimeter mPm;

    al mAlign;
    private AutoResetEvent m_autoEvent;

    private Thread m_thread;
    private ThreadParam m_tp;

    #endregion


    #region consturctor/destructor/load


    //생성자.
    public AlignForm()
    {
        InitializeComponent();

        this.chkTlsForRoll.Visible = AppLogic.License.ShowDevUI;
        this.txtRollWave1.Visible = AppLogic.License.ShowDevUI;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        TabControl1.TabPages.RemoveAt(TabControl1.TabPages.Count - 1);
        TabControl1.TabPages.RemoveAt(TabControl1.TabPages.Count - 1);
        TabControl1.TabPages.RemoveAt(TabControl1.TabPages.Count - 1);

        btnBlind_L_Digital.Enabled = true;

        initFabButton();
        init();
    }

    private void init()
    {
        try
        {
            mLeft = CGlobal.LeftAligner;
            mRight = CGlobal.RightAligner;
            mOther = CGlobal.OtherAligner;
            mPm = CGlobal.Opm;
            mTls = CGlobal.Tls;
            mAlign = CGlobal.alignLogic;
            if (mAlign != null)
            {
                mAlign.mReporter += writeStaus;
                //mAlign.AlignCompleted += setUiComplete;
            }

            initUiParamHandler();

            //combox 설정.
            cbAlignOpmPort1.Items.Clear();
            cbDigitalPort_L.Items.Clear();
            cbAxisScanPort.Items.Clear();   //axis scan port

            if (mPm != null)
            {
                var chs = mPm.ChList;
                cbAlignOpmPort1.Items.AddRange(chs);
                cbDigitalPort_L.Items.AddRange(chs);
                cbAxisScanPort.Items.AddRange(chs);
            }

            //config 
            mConfig = new XConfig(Application.StartupPath + mConfFilepath);
            loadConfig();

            uiPeakSearchStep.Value = (decimal)ScanParam.FinePeakStep;

            //쓰레드 가동.            
            if (!int.TryParse(cbAlignOpmPort1.Text, out m_tp.port1)) m_tp.port1 = 1;
            if (!int.TryParse(cbAlignOpmPort1.Text, out m_tp.port2)) m_tp.port1 = 2;
            m_tp.cmd = 0;                                   // mAlign.NOOPERATION;
            m_autoEvent = new AutoResetEvent(false);
            m_thread = new Thread(ThreadFunc);
            m_thread.IsBackground = true;
            m_thread.Name = "AlignForm";
            m_thread.Start();


            //Alignment Status 창을 띄움.
            if (Application.OpenForms.OfType<frmAlignStatus>().Count() == 0)
            {
                frmAlignStatus frm = new frmAlignStatus();
                frm.MdiParent = Application.OpenForms.OfType<frmMain>().First();
                frm.Show();
                frm.Refresh();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"frmAlignment_Load():\n{ex.Message}");
        }
    }

    const string mConfFilepath = @"\config\conf_alignment.xml";
    XConfig mConfig;

    private void loadConfig()
    {
        //load config & dislay.
        Location = new Point(mConfig.GetValue("WNDPOSX").To<int>(), mConfig.GetValue("WNDPOSY").To<int>());

        cbAlignOpmPort1.SelectedIndex = 0;
        cbDigitalPort_L.SelectedIndex = cbDigitalPort_L.Items.Count - 1;

        //
        loadSearchParam(mConfig);

        //BLIND
        cbSyncSearchRngDigital.Text = mConfig.GetValue("DIGITAL_SYNCRANGE", DEFAULT_FASTRNG);
        cbSyncSearchStepDigital.Text = mConfig.GetValue("DIGITAL_SYNCSTEP", 1);
        cbBlindRangeDigital.Text = mConfig.GetValue("DIGITAL_BLINDRANGE", DEFAULT_BLINDRNG);
        cbBlindStepDigital.Text = mConfig.GetValue("DIGITAL_BLINDSTEP", DEFAULT_BLINDSTEP);
        cbFblindInRng.Text = mConfig.GetValue("FULLBLINDINRANGE", 100);
        cbFblindInStep.Text = mConfig.GetValue("FULLBLINDINSTEP", 1);
        cbFblindOutRng.Text = mConfig.GetValue("FULLBLINDOUTRANGE", 200);
        cbFblindOutStep.Text = mConfig.GetValue("FULLBLINDOUTSTEP", 5);

        //roll
        txtDigitalRollDistOut.Text = mConfig.GetValue("DIGITAL_ROLLDISTOUT", DEFAULT_ROLLDISTOUT);
        txtRollRange.Text = mConfig.GetValue("ROLLRNG", DEFAULT_ROLLRNG);
        txtRollStep.Text = mConfig.GetValue("ROLLSTEP", DEFAULT_ROLLSTEP);
        txtRollThreshold.Text = mConfig.GetValue("ROLLPOSTCOND", DEFAULT_ROLLPOSTCOND);

        //scan
        cbAxisScanStage.Text = mConfig.GetValue("AXISSCAN_STAGE", "INPUT");
        cbAxisScanAxis.Text = mConfig.GetValue("AXISSCAN_AXIS", "X");
        cbAxisScanPort.Text = mConfig.GetValue("AXISSCAN_PORT", 1);
        cbAxisScanRange.Text = mConfig.GetValue("AXISSCAN_RANGE", 100);
        cbAxisStep.Text = mConfig.GetValue("AXISSCAN_STEP", 1);

        //fab auto function
        _fab_auto_function = mConfig.GetValue("FAB_AUTO_FUNCTION_RIGHT", "0").To<int>();
        btnFab_All_R.Text = _fab_auot_text[_fab_auto_function];

        mConfig.Save();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (mAlign != null) mAlign.AlignCompleted -= setUiComplete;
        if (mAlign != null) mAlign.mReporter -= writeStaus;

        StopOperation();

        //save configs.
        if (WindowState == FormWindowState.Normal)
        {
            mConfig?.SetValue("WNDPOSX", Location.X.ToString());
            mConfig?.SetValue("WNDPOSY", Location.Y.ToString());
            mConfig?.Save();
        }
        saveConfig();

        //thread 종료 및 마무리.
        if (m_thread != null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        if (m_autoEvent != null) m_autoEvent.Dispose();
        m_autoEvent = null;

        mPm = null;
        mAlign = null;
        mLeft = null;
        mRight = null;

        try
        {
            //Alignment Status 창을 띄움.
            var f = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();
            f?.Close();
        }
        catch
        {
        }

        base.OnFormClosed(e);
    }

    private void saveConfig()
    {
        if (mConfig == null) return;
        mConfig.SetValue("DIGITAL_PORT_FIRST", cbAlignOpmPort1.Text);
        mConfig.SetValue("DIGITAL_PORT_LAST", cbDigitalPort_L.Text);
        mConfig.SetValue("DIGITAL_SYNCRANGE", cbSyncSearchRngDigital.Text);
        mConfig.SetValue("DIGITAL_SYNCSTEP", cbSyncSearchStepDigital.Text);
        mConfig.SetValue("DIGITAL_BLINDRANGE", cbSyncSearchRngDigital.Text);
        mConfig.SetValue("DIGITAL_BLINDSTEP", cbBlindStepDigital.Text);

        mConfig.SetValue("FULLBLINDINRANGE", cbFblindInRng.Text);
        mConfig.SetValue("FULLBLINDINSTEP", cbFblindInStep.Text);
        mConfig.SetValue("FULLBLINDOUTRANGE", cbFblindOutRng.Text);
        mConfig.SetValue("FULLBLINDOUTSTEP", cbFblindOutStep.Text);

        mConfig.SetValue("DIGITAL_THRES", txt_search_threshold.Text);

        //roll
        mConfig.SetValue("DIGITAL_ROLLDISTOUT", txtDigitalRollDistOut.Text);
        mConfig.SetValue("ROLLRNG", txtRollRange.Text);
        mConfig.SetValue("ROLLSTEP", txtRollStep.Text);
        mConfig.SetValue("ROLLPOSTCOND", txtRollThreshold.Text);

        //xy search param
        if (CGlobal.XySearchParamLeft != null)
        {
            mConfig.SetValue("ALIGN_LEFT_XY_RANGE_STEP", CGlobal.XySearchParamLeft.Pack());
            mConfig.SetValue("XY_SEARCH_BY_SCAN_LEFT", CGlobal.XySearchParamLeft.SearchByScan ? "1" : "0");
            mConfig.SetValue("SCAN_TO_CENTER_LEFT", CGlobal.XySearchParamLeft.ScanToCenter ? "1" : "0");
        }
        if (CGlobal.XySearchParamRight != null)
        {
            mConfig.SetValue("ALIGN_RIGHT_XY_RANGE_STEP", CGlobal.XySearchParamRight.Pack());
            mConfig.SetValue("XY_SEARCH_BY_SCAN_RIGHT", CGlobal.XySearchParamRight.SearchByScan ? "1" : "0");
            mConfig.SetValue("SCAN_TO_CENTER_RIGHT", CGlobal.XySearchParamRight.ScanToCenter ? "1" : "0");
        }
        mConfig.SetValue("PEAK_SEARCH_STEP", ScanParam.FinePeakStep.ToString());

        mConfig.SetValue("AXISSCAN_STAGE", cbAxisScanStage.Text);
        mConfig.SetValue("AXISSCAN_AXIS", cbAxisScanAxis.Text);
        mConfig.SetValue("AXISSCAN_PORT", cbAxisScanPort.Text);
        mConfig.SetValue("AXISSCAN_RANGE", cbAxisScanRange.Text);
        mConfig.SetValue("AXISSCAN_STEP", cbAxisStep.Text);

        //fab auto function
        mConfig.SetValue("FAB_AUTO_FUNCTION_RIGHT", _fab_auto_function.ToString());

        mConfig.Save();
    }

    #endregion


    #region Thread function

    /// <summary>
    /// Thread Function.
    /// </summary>
    void ThreadFunc()
    {
        while (true)
        {
            m_autoEvent.WaitOne();

            var align = mAlign;
            var cmd = m_tp.cmd;

            // fa alignemnt //
            int sn = m_tp.stageNo; //stage no.

            if (cmd == al.ZAPPROACH_SINGLE) align.ZappSingleStage(sn);
            else if (cmd == al.ZAPPROACH_DUAL) align.ApproachInOut(0, 0);
            else if (cmd == al.ANGLE_TY_SINGLE) align.AngleTy(sn);
            else if (cmd == al.ANGLE_TX_SINGLE)
            {
                var stage = mLeft.stageNo == sn ? mLeft : mRight;
                //stage.RelMove(stage.AXIS_Z, 50);
                align.AngleTx(sn);
                //stage.RelMove(stage.AXIS_Z, -50);
            }

            else if (cmd == al.XY_SEARCH) xySearch(m_tp.stageNo);
            else if (cmd == al.XY_SEARCH_BOTH) xySearch();

            else if (cmd == al.AXISSEARCH)
                align.AxisSearch(m_tp.stageNo, m_tp.axis, m_tp.port1, m_tp.range, m_tp.step, mMoveToCenter);
            else if (cmd == al.ROLLOUT) roll();

            else if (cmd == al.SYNCXYSEARCH)
                align.SyncXySearch(m_tp.port1, m_tp.range, m_tp.step, m_tp.thres);
            else if (cmd == al.XYBLINDSEARCH)
                align.XyBlindSearch(m_tp.stageNo, m_tp.port1, m_tp.range, m_tp.step, m_tp.thres);
            else if (cmd == al.XYFULLBLINDSEARCH)
                align.XyFullBlindSearch(m_tp.port1, m_tp.range, m_tp.step, m_tp.range2, m_tp.step2, m_tp.thres);


            else if (cmd == al.FabAuto_AppAndBack) autoApproach(sn, true);
            else if (cmd == al.FabAuto_AppAndBack_Both) autoApproach(true);

            else if (cmd == al.FabAuto_Left) autoAngle(mLeft.stageNo, true);
            else if (cmd == al.FabAuto_Right) autoAngle(mRight.stageNo, true);
            else if (cmd == al.FabAuto_Both) autoAngle_AlignFormOnly();

            else if (cmd == al.FabAuto_BondingAlign) autoAlignBonding_Signal();

            setUiComplete(m_tp.cmdName);

        }//while
    }//private void ThreadFunc()



    frmDistSensViewer frmDistSens = null;
    OpmDisplayForm frmDigitalPwr = null;
    frmSourceController frmSourCont = null;
    uiStageControl frmStageCont = null;

    private void setUiStart(string msg)
    {
        m_tp.cmdName = msg;
        uiStatus.Text = $"Starting {msg}";
        uiStatusStrip.Refresh();
        DisableWndButStop();
        this.Cursor = Cursors.WaitCursor;

        frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();
        frmDigitalPwr = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();
        frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();
        frmStageCont = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();

        //change windows state.
        frmDistSens?.StopSensing();
        frmDigitalPwr?.DisplayOff();
        frmSourCont?.DisableForm();
    }

    private void setUiComplete(string msg)
    {
        try
        {
            if (InvokeRequired) Invoke((Action)(() => _setUiComplete(msg)));
            else _setUiComplete(msg);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"AlignForm.setUiComplete()\n{ex.Message}\n\n{ex.StackTrace}");
        }
    }

    private void _setUiComplete(string msg)
    {
        //Update Stage Postion
        if (frmStageCont != null)
        {
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_X);
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_Y);
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_Z);
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_TX);
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_TY);
            frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_TZ);

            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_X);
            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_Y);
            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_Z);
            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_TX);
            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_TY);
            frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_TZ);
        }

        //restore windows state.
        //uiStatus.Text += " Completed!";
        EnableWnd();
        uiStatus.Text = $"Finished {msg}";
        uiStatusStrip.Refresh();
        this.Cursor = Cursors.Default;

        frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();
        frmDigitalPwr = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();
        frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();
        frmStageCont = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();

        frmDistSens?.StartSensing();
        frmDigitalPwr?.DisplayOn();
        frmSourCont?.EnableForm();


    }

    void writeStaus(string msg)
    {
        if (InvokeRequired) Invoke((Action)(() =>
        {
            uiStatus.Text = msg;
            //uiStatusStrip.Refresh();
            Refresh();
        }));
        else
        {
            uiStatus.Text = msg;
            //uiStatusStrip.Refresh();
            Refresh();
        }
    }


    #endregion


    #region Private method

    private void EnableWnd()
    {

        grpPassive.Enabled = true;
        TabControl1.Enabled = true;

        grpPassive.Refresh();
        TabControl1.Refresh();
    }

    private void DisableWndButStop()
    {
        grpPassive.Enabled = false;
        TabControl1.Enabled = false;

        grpPassive.Refresh();
        TabControl1.Refresh();
    }


    #endregion




    #region ---- Stop ----

    public void StopOperation()
    {
        _stopping = true;
        mAlign?.StopOperation();
    }

    /// <summary>
    /// 명령을 중지한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        StopOperation();
    }


    #endregion




    #region ---- XY Search Param ----

    private void initUiParamHandler()
    {
        txt_search_threshold.TextChanged += param_Changed;
        txt_range_left_x.TextChanged += param_Changed;
        txt_step_left_x.TextChanged += param_Changed;
        txt_range_left_y.TextChanged += param_Changed;
        txt_step_left_y.TextChanged += param_Changed;

        txt_range_right_x.TextChanged += param_Changed;
        txt_step_right_x.TextChanged += param_Changed;
        txt_range_right_y.TextChanged += param_Changed;
        txt_step_right_y.TextChanged += param_Changed;

        uiCheckScanLeft.CheckedChanged += param_Changed;
        uiCheckCenterLeft.CheckedChanged += param_Changed;
        uiCheckScanRight.CheckedChanged += param_Changed;
        uiCheckCenterRight.CheckedChanged += param_Changed;

        txtRollRange.TextChanged += rollParam_Changed;
        txtRollStep.TextChanged += rollParam_Changed;
        txtRollThreshold.TextChanged += rollParam_Changed;
    }
    private void param_Changed(object sender, EventArgs e)
    {
        applySearchParam();

        var v = uiCheckCenterLeft.Enabled = uiCheckScanLeft.Checked;
        txt_range_left_x.Enabled = txt_range_left_y.Enabled = v;
        txt_step_left_x.Enabled = txt_step_left_y.Enabled = v;

        v = uiCheckCenterRight.Enabled = uiCheckScanRight.Checked;
        txt_range_right_x.Enabled = txt_range_right_y.Enabled = v;
        txt_step_right_x.Enabled = txt_step_right_y.Enabled = v;
    }
    private void uiMoveToCenter_CheckedChanged(object sender, EventArgs e)
    {
        uiMoveToCenter.Text = uiMoveToCenter.Checked ? "CENTER" : "PEAK";
    }


    private void applySearchParam()
    {
        if (mLoadingParam) return;
        try
        {
            CGlobal.AlignThresholdPower = (int)double.Parse(txt_search_threshold.Text);

            if (CGlobal.XySearchParamLeft == null) CGlobal.XySearchParamLeft = XYSearchParam.Create(new double[] { 10, 1, 10, 1 });
            if (CGlobal.XySearchParamRight == null) CGlobal.XySearchParamRight = XYSearchParam.Create(new double[] { 10, 1, 10, 1 });

            //save to global
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
        }
        catch(Exception ex)
        {
            Log.Write(ex.Message);
        }
    }

    bool mLoadingParam = false;
    private void loadSearchParam(XConfig config)
    {
        try
        {
            mLoadingParam = true;
            txt_search_threshold.Text = config.GetValue("DIGITAL_THRES");
            CGlobal.AlignThresholdPower = (int)double.Parse(txt_search_threshold.Text);

            ScanParam.FinePeakStep = config.GetValue("PEAK_SEARCH_STEP", "0.25").To<double>();
            uiPeakSearchStep.Value = (decimal)ScanParam.FinePeakStep;

            //search param - Left
            CGlobal.XySearchParamLeft = XYSearchParam.Create(config.GetValue("ALIGN_LEFT_XY_RANGE_STEP").Unpack<double>().ToArray());
            CGlobal.XySearchParamLeft.SearchByScan = config.GetValue("XY_SEARCH_BY_SCAN_LEFT").Contains("1");
            CGlobal.XySearchParamLeft.ScanToCenter = config.GetValue("SCAN_TO_CENTER_LEFT").Contains("1");
            txt_range_left_x.Enabled = txt_range_left_y.Enabled = txt_step_left_x.Enabled = txt_step_left_y.Enabled = CGlobal.XySearchParamLeft.SearchByScan;

            //search param - Right
            CGlobal.XySearchParamRight = XYSearchParam.Create(config.GetValue("ALIGN_RIGHT_XY_RANGE_STEP").Unpack<double>().ToArray());
            CGlobal.XySearchParamRight.SearchByScan = config.GetValue("XY_SEARCH_BY_SCAN_RIGHT").Contains("1");
            CGlobal.XySearchParamRight.ScanToCenter = config.GetValue("SCAN_TO_CENTER_RIGHT").Contains("1");
            txt_range_right_x.Enabled = txt_range_right_y.Enabled = txt_step_right_x.Enabled = txt_step_right_y.Enabled = CGlobal.XySearchParamRight.SearchByScan;

            //update UI
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
        }
        finally
        {
            mLoadingParam = false;
        }
    }

    #endregion




    #region ---- Roll ----


    void setRollParam()
    {
        mAlign?.SetRollParam(txtRollRange.Text.To<int>(), txtRollStep.Text.To<int>(), txtRollThreshold.Text.To<double>(),
            cbAlignOpmPort1.Text.To<int>(), cbDigitalPort_L.Text.To<int>(), txtDigitalRollDistOut.Text.To<int>());
    }

    void rollParam_Changed(object sender, EventArgs e)
    {
        try
        {
            var range = txtRollRange.Text.To<int>();
            var step = txtRollStep.Text.To<int>();
            var threshold = txtRollThreshold.Text.To<double>();
            mAlign?.SetRollParam(range, step, threshold);
        }
        catch { }
    }

    private void btnRoll_R_Digital_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Right - Roll");

            m_tp.cmd = al.ROLLOUT;
            m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text);
            m_tp.port2 = Convert.ToInt32(cbDigitalPort_L.Text);
            m_tp.rollDist = Convert.ToInt32(txtDigitalRollDistOut.Text);
            m_tp.tlsForRoll = chkTlsForRoll.Checked;
            m_tp.tls = mTls;
            var waves = txtRollWave1.Text.Unpack<double>().ToArray();
            m_tp.wave1 = waves[0];
            m_tp.wave2 = waves[1];
            m_tp.range = Convert.ToInt32(txtRollRange.Text);
            m_tp.step = Convert.ToDouble(txtRollStep.Text);
            m_tp.thres = Convert.ToDouble(txtRollThreshold.Text);

            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception)
        {
            uiStatus.Text = "Error!!";
        }
    }

    void roll()
    {
        Invoke((Action)setRollParam);

        Thread.Sleep(100);//await Task.Delay(100);
        if (m_tp.tlsForRoll)
            mAlign.RollOut(m_tp.port1, m_tp.port2, m_tp.rollDist, m_tp.tls, m_tp.wave1, m_tp.wave2, m_tp.range, m_tp.step, m_tp.thres);
        else mAlign.RollOut();
    }


    #endregion



    #region ---- Approach ----

    private void btnZapp_L_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Left - Approach");

            //ZApproach...
            m_tp.cmd = al.ZAPPROACH_SINGLE;
            m_tp.stageNo = al.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    private void btnZapp_R_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Right - Approach");

            //Zapproach
            m_tp.cmd = al.ZAPPROACH_SINGLE;
            m_tp.stageNo = al.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }

    #endregion



    #region ---- Angle X Y ----

    private void btnFARY_L_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Left - θY");

            //Operation
            m_tp.cmd = al.ANGLE_TY_SINGLE;
            m_tp.stageNo = al.LEFT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    private void btnFARY_R_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Right - θY");

            //Operation
            m_tp.cmd = al.ANGLE_TY_SINGLE;
            m_tp.stageNo = al.RIGHT_STAGE;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    private void btnFARX_L_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Left - θX");

            //Operation
            m_tp.cmd = al.ANGLE_TX_SINGLE;
            m_tp.stageNo = al.STAGE_L;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    private void btnFARX_R_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Right - θX");

            //Operation
            m_tp.cmd = al.ANGLE_TX_SINGLE;
            m_tp.stageNo = al.STAGE_R;
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }

    #endregion



    #region ---- Blind ----

    /// <summary>
    /// Left Stage Blind Search.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBlind_L_Digital_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Left - Blind");

            //blind xySearch.
            ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;
            m_tp.cmd = al.XYBLINDSEARCH;
            m_tp.stageNo = al.STAGE_L;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text);      //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);       //Search Step [um]
            m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text);     //detect port!!
            m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);         //Searh Threshold [dBm]

            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    /// <summary>
    /// Right Stage Blind Search.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnBlind_R_Digital_Click(object sender, EventArgs e)
    {
        var frmDistSens = FormLogic<frmMain>.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
        var frmStageCont = FormLogic<frmMain>.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = FormLogic<frmMain>.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = Application.OpenForms.OfType<frmSourceController>().First();

        try
        {

            //if (mAlign.IsCompleted() == false) return;


            //change windows state.
            if (frmDistSens != null)
                frmDistSens.StopSensing();

            if (frmDigitalPwr != null)
                frmDigitalPwr.DisplayOff();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "Blind Search_RIGHT...";


            //Blind Search
            m_tp.cmd = al.XYBLINDSEARCH;

            m_tp.stageNo = al.STAGE_R;
            m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text); //Search Range [um]
            m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);   //Search Step [um]
            m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);    //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text); //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(50);

            //Display alignment status
            while (mAlign.IsCompleted() == false)
            {
                Application.DoEvents();
            }

            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(al.STAGE_R, mLeft.AXIS_X);
                frmStageCont.UpdateAxisPos(al.STAGE_R, mLeft.AXIS_Y);
            }

            //resotre windows state.
            uiStatus.Text = "Blind Search_RIGHT completed!!";
            EnableWnd();
            this.Cursor = Cursors.Default;

            if (frmDistSens != null) frmDistSens.StartSensing();

            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }

    #endregion



    #region ---- XY Search ----

    private void btnFINE_L_Digital_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Left - XYSearch");
            //saveSearchParam();

            ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;

            //fine xySearch.
            m_tp.cmd = al.XY_SEARCH;
            m_tp.stageNo = al.STAGE_L;
            //m_tp.range = txt_range_left_x.Text.To<int>();
            //m_tp.step = txt_step_left_x.Text.To<double>();
            m_tp.port1 = cbAlignOpmPort1.Text.To<int>();
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.Message);
        }
    }


    public void btnFINE_R_Digital_Click(object sender, EventArgs e)
    {
        try
        {
            //if (mAlign.IsCompleted() == false) return;
            setUiStart("Right - XYSearch");
            //saveSearchParam();

            ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;

            //fine xySearch.
            m_tp.cmd = al.XY_SEARCH;
            m_tp.stageNo = al.STAGE_R;
            //m_tp.range = txt_range_right_x.Text.To<int>();
            //m_tp.step = txt_step_right_x.Text.To<double>();
            m_tp.port1 = cbAlignOpmPort1.Text.To<int>();
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }

    void xySearch(int sn)
    {
        Thread.Sleep(100);//await Task.Delay(100);
        var param = (sn == al.LEFT_STAGE) ? CGlobal.XySearchParamLeft : CGlobal.XySearchParamRight;
        param.StageNo = sn;
        param.Port = m_tp.port1;
        mAlign?.XySearch(param);
    }
    void xySearch()
    {
        var cts = _ctsUv;
        _ctsUv = null;

        xySearch(mLeft.stageNo);
        xySearch(mRight.stageNo);
        cts?.Cancel();
    }

    #endregion



    #region ---- Axis Scan ----

    bool mMoveToCenter = false;

    /// <summary>
    /// Axis Search
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnAxisScan_Click(object sender, EventArgs e)
    {
        try
        {
            if (mAlign.IsCompleted() == false) return;

            mMoveToCenter = uiMoveToCenter.Checked;

            //fine xySearch.
            m_tp.cmd = al.AXISSEARCH;

            if (cbAxisScanStage.Text == "INPUT") m_tp.stageNo = al.STAGE_L;
            else m_tp.stageNo = al.STAGE_R;

            var stage = (m_tp.stageNo == mLeft.stageNo) ? mLeft : mRight;

            if (cbAxisScanAxis.Text == "X") m_tp.axis = stage.AXIS_X;
            else m_tp.axis = stage.AXIS_Y;

            setUiStart($"{cbAxisScanStage.Text}.{cbAxisScanAxis.Text} - Scan");

            m_tp.range = cbAxisScanRange.Text.To<int>();
            m_tp.step = cbAxisStep.Text.To<double>();
            m_tp.port1 = cbAxisScanPort.Text.To<int>();
            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }


    #endregion



    #region ---- Sync & FullBlind ----

    /// <summary>
    /// SyncXySearch를 실행한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSyncSearch_Click(object sender, EventArgs e)
    {

        var frmDistSens = FormLogic<frmMain>.CreateAndShow<frmDistSensViewer>(true, false);
        var frmDigitalPwr = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
        var frmStageCont = FormLogic<frmMain>.CreateAndShow<uiStageControl>(true, false);
        var frmStatus = FormLogic<frmMain>.CreateAndShow<frmAlignStatus>(true, true);
        var frmSourCont = Application.OpenForms.OfType<frmSourceController>().First();

        try
        {
            if (mAlign.IsCompleted() == false) return;

            //change windows state.
            frmDistSens?.StopSensing();
            frmDigitalPwr?.DisplayOff();
            frmSourCont?.DisableForm();

            DisableWndButStop();
            this.Cursor = Cursors.WaitCursor;
            uiStatus.Text = "SyncXySearch...";

            //Blind Search
            m_tp.cmd = al.SYNCXYSEARCH;

            m_tp.stageNo = al.STAGE_LR;
            m_tp.range = Convert.ToInt32(cbSyncSearchRngDigital.Text);  //Search Range [um]
            m_tp.step = Convert.ToInt32(cbSyncSearchStepDigital.Text);  //Search Step [um]
            m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);    //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text);         //detect port!!
            m_tp.step = ALIGNRES;
            m_autoEvent.Set();
            Thread.Sleep(50);

            //Display alignment status
            while (mAlign.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_X);
                frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_Y);
                frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_X);
                frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_Y);
            }


            //resotre windows state.
            uiStatus.Text = "SyncXySearch completed!!";
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
            uiStatus.Text = "Error!!";
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
        OpmDisplayForm frmDigitalPwr = null;
        uiStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        if (Application.OpenForms.OfType<frmDistSensViewer>().Count() > 0)
            frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();

        if (Application.OpenForms.OfType<OpmDisplayForm>().Count() > 0)
            frmDigitalPwr = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();

        if (Application.OpenForms.OfType<uiStageControl>().Count() > 0)
            frmStageCont = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
            frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();

        if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
            frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


        try
        {

            if (mAlign.IsCompleted() == false)
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
            uiStatus.Text = "XyFullBlindSearch...";


            //XyFullBlind Search 
            m_tp.cmd = al.XYFULLBLINDSEARCH;

            m_tp.stageNo = al.STAGE_LR;
            m_tp.range = Convert.ToInt32(cbFblindInRng.Text);  //Search Range inputside [um]
            m_tp.step = Convert.ToDouble(cbFblindInStep.Text);   //Search Step inputside [um] 
            m_tp.range2 = Convert.ToInt32(cbFblindOutRng.Text);  //Search Range outputside [um]
            m_tp.step2 = Convert.ToDouble(cbFblindOutStep.Text);   //Search Step outputside [um] 
            m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);     //Searh Threshold [dBm]
            m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text);     //detect port!!

            m_autoEvent.Set();
            Thread.Sleep(50);


            //Display alignment status
            while (mAlign.IsCompleted() == false)
            {
                Application.DoEvents();
            }


            //Update Stage Postion
            if (frmStageCont != null)
            {
                frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_X);
                frmStageCont.UpdateAxisPos(al.STAGE_L, mLeft.AXIS_Y);
                frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_X);
                frmStageCont.UpdateAxisPos(al.STAGE_R, mRight.AXIS_Y);
            }


            //resotre windows state.
            uiStatus.Text = "XyFullBlindSearch completed!!";
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
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            EnableWnd();
        }

    }


    #endregion




    private void uiApplyPeakStep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;
    }

    private void chkFab_All_Joint_CheckedChanged(object sender, EventArgs e)
    {
        btnFab_All_R.Enabled = !chkFab_All_Joint.Checked;
    }

    private void chkTlsForRoll_CheckedChanged(object sender, EventArgs e)
    {
        txtRollWave1.Enabled = chkTlsForRoll.Checked;
    }

}


