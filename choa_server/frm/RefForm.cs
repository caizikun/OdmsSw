using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Jeffsoft;
using Free302.TnMLibrary.DataAnalysis;
using Free302.MyLibrary.Utility;
using Neon.Aligner;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;

public partial class RefForm : Form
{


    #region definition

    private int mTlsPower = CGlobal.TlsParam.Power;
    private int mWaveStart = CGlobal.TlsParam.WaveStart;
    private int mWaveStop = CGlobal.TlsParam.WaveStop;
    private double mWaveStep = CGlobal.TlsParam.WaveStep;
    private int mSweepSpeed = CGlobal.TlsParam.Speed;
    private int mPmGain = CGlobal.PmGain[0];

    private const int DEFAULT_FACOREPITCH = 127;
    private const int DEFAULT_OPTPWRTHRES = 0;
    private const int DEFAULT_ALIGNWAVELEN = 1260;

    private enum CMD { SinglePort, MultiPorts, GoPort, Msr, GoInitPos, End };

    #endregion




    #region structure/innor class


    private struct option
    {
        public int optPwrThre;              //[dBm] 
        public int corePitch;               //[um] Output FA Corepicth
        public int alignWavelen;            //[nm]Wavelength for Alignment;
    }


    private struct threadParam
    {
        public CMD cmd;
        public int startPort;
        public bool align;

        public int NumAvg;
        public int NumMovingAvg;

        public bool tlsAlignment;
        public List<int> waveList;
        public bool McMonitor;
    }


    private struct initPos
    {
        public CStageAbsPos inpos;
        public CStageAbsPos outpos;
    }


    #endregion




    #region member variables


    private ReferenceDataNp m_ref;
    private ReferenceDataNp m_newRef;

    private Itls m_tls;
    private IoptMultimeter m_mpm;
    private Istage m_leftStg;
    private Istage m_rightStg;
    private IairValvController m_avc;

    private SweepLogic mSweep;
    private AlignLogic mAlign;

    private option m_opt;

    bool m_stopFlag;
    bool m_isRuning; //running:true , stop :false
    private threadParam mParam;
    private AutoResetEvent m_autoEvent;
    private Thread m_thread;

    private initPos m_initPos;


    #endregion




    #region Constructor / Destructor


    public RefForm()
    {
        InitializeComponent();

        this.btnMA.Visible = AppLogic.License.ShowDevUI;
        this.textMA.Visible = AppLogic.License.ShowDevUI;
        btnNewStart_Multi.Visible = AppLogic.License.ShowDevUI;
        chkTLSAlignment.Visible = AppLogic.License.ShowDevUI;
        label1.Visible = AppLogic.License.ShowDevUI;
        this.txtWaveList.Visible = AppLogic.License.ShowDevUI;

        initGraph(_wg);
        initGraph(_wgNew);
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        //Ref.
        m_ref = new ReferenceDataNp();
        m_ref.LoadReferenceFromTxt(Application.StartupPath + "\\refNonpol.txt");
        PlotCurRef(1);

        //Init member variables
        mSweep = CGlobal.sweepLogic;
        mAlign = CGlobal.alignLogic;
        if (mSweep != null) mSweep.mReporter += writeStatus;
        if (mAlign != null) mAlign.mReporter += writeStatus;

        m_tls = CGlobal.Tls;
        m_mpm = CGlobal.Opm;
        m_leftStg = CGlobal.LeftAligner;
        m_rightStg = CGlobal.RightAligner;
        m_avc = (CGlobal.AirValve != null) ? CGlobal.AirValve : null;

        //Load option && configs.
        string confFilepath = Application.StartupPath + @"\config\conf_ref.xml";
        this.Location = LoadWndStartPos(confFilepath);

        m_opt = LoadOptionFromXml(confFilepath);
        if ((m_opt.corePitch == 0) && (m_opt.optPwrThre == 0))
        {
            m_opt.optPwrThre = DEFAULT_OPTPWRTHRES;
            m_opt.corePitch = DEFAULT_FACOREPITCH;
            m_opt.alignWavelen = DEFAULT_ALIGNWAVELEN;
        }
        txtOptPwrThres.Text = Convert.ToString(m_opt.optPwrThre);
        txtFAcorepitch.Text = Convert.ToString(m_opt.corePitch);
        txtAlignWavelen.Text = Convert.ToString(m_opt.alignWavelen);
        LabelSweep.Text = $"Range : {mWaveStart}-{mWaveStop} : {mWaveStep}";
        labelTLSPower.Text = $"TLS Power : {mTlsPower} [dBm]";

        //Excution
        cbStartPort.Items.AddRange(m_mpm.ChList);
        cbStoptPort.Items.AddRange(m_mpm.ChList);
        cbStartPort.SelectedIndex = 0;
        cbStoptPort.SelectedIndex = 0;

        m_initPos = LoadInitPos(confFilepath);
        DisplayInitPos();
        InitDisplayList();

        int curFaPos = FindCurFaPortPos();
        if (curFaPos != -1) lbCurPort.Text = curFaPos.ToString();
        else lbCurPort.Text = "???";

        if (m_tls != null)
        {
            m_tls.SetTlsOutPwr(mTlsPower);
            Thread.Sleep(100);
            //m_tls.SetTlsSweepSpeed(SWEEPSPEED);//added by Bae 2015-10-26
        }

        //fix output
        if (m_avc != null) m_avc.OpenValve((int)AirValveAligner.Output);

        //start 가동.
        m_autoEvent = new AutoResetEvent(false);
        m_thread = new Thread(ThreadFunc);
        m_thread.Name = "REF";
        m_thread.IsBackground = true;
        m_thread.Start();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        e.Cancel |= m_isRuning;
        if (e.Cancel) return;

        if (mSweep != null) mSweep.mReporter -= writeStatus;
        if (mAlign != null) mAlign.mReporter -= writeStatus;

        //free output
        if (m_avc != null) m_avc.CloseValve((int)AirValveAligner.Output);

        //save config.
        string confFilepath = Application.StartupPath + "\\config\\conf_ref.xml";
        SaveWndStartPos(confFilepath);

        if (!SaveOptionToXml(m_opt, confFilepath))
            MessageBox.Show("Failed to save settings!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


        if (!SaveInitPos(confFilepath))
            MessageBox.Show("Failed to save Init position!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


        //thread 종료 및 마무리.
        if (m_thread != null)
        {
            mParam.cmd = CMD.End;
            m_autoEvent.Set();
            Thread.Sleep(100);

            //m_thread.Abort();
            //m_thread.Join();
            //m_thread = null;
        }

        //if (m_autoEvent != null) m_autoEvent.Dispose();
        //m_autoEvent = null;

        m_tls = null;
        m_mpm = null;
        m_leftStg = null;
        m_rightStg = null;

        mSweep = null;
        mAlign = null;

        base.OnFormClosing(e);
    }


    #endregion




    #region ThreadFunction


    async void ThreadFunc()
    {
        var runThread = true;
        while (runThread)
        {
            try
            {
                //시작 신호 대기
                m_isRuning = false;
                //if (m_autoEvent == null) break;
                m_autoEvent.WaitOne();
                m_isRuning = true;
                m_stopFlag = false;

                switch (mParam.cmd)
                {
                    case CMD.GoPort:
                        {
                            //position 
                            int targPort = mParam.startPort;
                            int curPort = FindCurFaPortPos();
                            if (curPort < 0)
                            {
                                MessageBox.Show("Failed to find In FAB position", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }

                            //Target Port로 이동
                            int dist = 0;
                            dist = m_opt.corePitch * (targPort - curPort);
                            m_leftStg.RelMove(m_leftStg.AXIS_Z, -100);
                            m_leftStg.RelMove(m_leftStg.AXIS_X, dist);
                            m_leftStg.WaitForIdle(m_leftStg.AXIS_X);
                            m_leftStg.RelMove(m_leftStg.AXIS_Z, 85);
                            m_leftStg.WaitForIdle(m_leftStg.AXIS_X);

                            //확인..
                            curPort = FindCurFaPortPos();
                            if (curPort != targPort)
                            {
                                MessageBox.Show("Invalid position detected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                updateCurrentPortLabel("???");
                                break;
                            }
                            else updateCurrentPortLabel(curPort.ToString());

                            break;
                        }

                    case CMD.GoInitPos:
                        goInitPos();
                        break;

                    case CMD.SinglePort:
                        await runSingle();
                        m_isRuning = false;
                        break;

                    case CMD.MultiPorts:
                        break;

                    case CMD.End:
                        runThread = false;
                        break;
                }
            }
            finally
            {
                try
                {
                    //완료 메세지.//
                    Invoke((Action)EnableWnd);

                    if (mParam.cmd == CMD.SinglePort)
                    {
                        if (m_stopFlag == true)
                            Invoke((Action)(() => MessageBox.Show("Stopped", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                        else if (runThread)
                            Invoke((Action)(() => MessageBox.Show("Complete!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    }
                }
                catch { }
            }
        }//while (true)

    }


    void updateNumAvgUi(int loopIndex)
    {
        Invoke((Action)(() =>
        {
            textAverage.Text = $"{mAveraging.NumAveraging - loopIndex}";
            textAverage.Refresh();
        }));
    }
    void showMessage(string msg)
    {
        Invoke((Func<IWin32Window, string, DialogResult>)MessageBox.Show, this, msg);
    }

    async Task runSingle()
    {
        var frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();
        var frmOpm = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();

        try
        {
            //Disable optical source controller
            frmSourCont?.DisableForm();

            int curPort = FindCurFaPortPos();

            //init new ref
            if (m_newRef == null)
            {
                m_newRef = new ReferenceDataNp();
                m_newRef.Clear(mWaveStart, mWaveStop, mWaveStep);
            }

            mAveraging = new Averaging(mParam.NumAvg, mParam.NumMovingAvg);
            m_newRef.ClearMonitorPower(mParam.startPort);

            for (int i = 0; i < mAveraging.NumAveraging; i++)
            {
                //test mode by DrBae
                updateNumAvgUi(i);

                //display Digital powermeter off
                frmOpm?.DisplayOff();

                //align left
                if (mParam.align) await alignLeft();

                if (m_stopFlag) return;

                //monitor Port Reference
                await measureMonitor(i);

                //sweep and acquire data
                await measureSpectrum();

                //inter measurement delay
                Thread.Sleep(100);

            }//averaging

            //display numAverag
            updateNumAvgUi(0);
        }
        catch (Exception ex)
        {
            showMessage($"RefForm.runSingle()\n{ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            //display Digital powermeter on
            frmOpm?.DisplayOn();

            //Enable optical source controller
            frmSourCont?.EnableForm();
        }

    }

    async Task measureMonitor(int avgIndex)
    {
        writeStatus("Measure monitor... ");

        var measurePower = 0.0;
        if (mParam.McMonitor)//TLS 이용 - local or server
        {
            CGlobal.osw?.SetToTls();

            //파장 변경(Align & monitor 측정 파장)
            var wave = mParam.waveList[(mParam.startPort - 1) % mParam.waveList.Count];
            measurePower = await mSweep.MeasurePower(wave, mParam.startPort, 10, true);
        }
        else//align LS 이용
        {
            CGlobal.osw?.SetToAlign();
            measurePower = mSweep.MeasurePower(mParam.startPort, 10, true);
        }

        var monitorNewRef = (m_newRef.MonitorPower(mParam.startPort) * avgIndex + measurePower) / (avgIndex + 1);//averaging
        m_newRef.SetMonitorPower(mParam.startPort, monitorNewRef);
    }

    async Task measureSpectrum()
    {
        writeStatus("Measure spectrum... ");
        try
        {
            CGlobal.osw?.SetToTls();
            await mSweep.MeasureSpecturmNp(new int[] { mParam.startPort }, new int[] { mPmGain }, mWaveStart, mWaveStop, mWaveStep);
        }
        finally
        {
            if (mParam.tlsAlignment) CGlobal.osw?.SetToTls();
            else CGlobal.osw?.SetToAlign();
            m_mpm.SetGainLevel(CGlobal.PmAlignGain);                //Align Gain Level 변경
        }
        PortPowers swPwr = mSweep.GetPortPower(mParam.startPort);
		if (swPwr.NonPol != null)
		{
			mAveraging.AddData(swPwr.NonPol);
			m_newRef.SetPortData(swPwr);

			//plot..
			Invoke((Action<int>)PlotNewRef, mParam.startPort); 
		}
    }

    async Task alignLeft()
    {
        bool alignEventStatus = false;
        try
        {
            alignEventStatus = mAlign?.IsEventEnable ?? false;
            if (mAlign != null) mAlign.IsEventEnable = false;

            //Left-stage ZApproach
            writeStatus("Left : Approaching...");
            mAlign?.Approach(AppStageId.Left, 10, 30);
            if (m_stopFlag == true) return;

            //OSW 설정
            if (mParam.tlsAlignment)
            {
                CGlobal.osw?.SetToTls();

                //파장 변경(Align & monitor 측정 파장)
                var wave = mParam.waveList[(mParam.startPort - 1) % mParam.waveList.Count];

                if (!CGlobal.UsingTcpServer) m_tls?.SetTlsWavelen(wave);
                else
                {
                    mSweep.TcpServer_Register(true);
                    mSweep.TcpServer_BeginAlign();
                    mSweep.TcpServer_Align(wave);
                }
            }
            else
            {
                CGlobal.osw?.SetToAlign();
            }

            //input Stage  XY Fine Searching
            writeStatus("Right : XY Searching...");

            //mAlign.XySearch(mAlign.LEFT_STAGE, m_tp.startPort, XYSEARCHSTEP);
            CGlobal.XySearchParamRight.StageNo = AlignLogic.RIGHT_STAGE;// AlignLogic.RIGHT_STAGE;
            CGlobal.XySearchParamRight.Port = mParam.startPort;
            mAlign?.XySearch(CGlobal.XySearchParamRight);
        }
        finally
        {
            if (CGlobal.UsingTcpServer) mSweep.TcpServer_Register(false);
            if (mAlign != null) mAlign.IsEventEnable = alignEventStatus;
        }
    }




    #region ==== Moving Averaging ====

    Averaging mAveraging;

    private void btnMA_Click(object sender, EventArgs e)
    {
        int numAvg;
        if (!int.TryParse(textMA.Text, out numAvg)) numAvg = 1;
        mAveraging.NumMovingAveraging = numAvg;
        applyMovingAverage();
    }
    void applyMovingAverage()
    {
        if (mAveraging == null) return;
        mAveraging.ApplyMovingAverage(mSweep.GetPortPower(mParam.startPort).NonPol);

        //plot
        Invoke(new Action<int>(PlotNewRef), mParam.startPort);

    }
    #endregion



    private void goInitPos()
    {

        m_leftStg.AbsMove(m_leftStg.AXIS_X, m_initPos.inpos.x);
        m_rightStg.AbsMove(m_rightStg.AXIS_X, m_initPos.outpos.x);

        m_leftStg.AbsMove(m_leftStg.AXIS_Y, m_initPos.inpos.y);
        m_rightStg.AbsMove(m_rightStg.AXIS_Y, m_initPos.outpos.y);

        m_leftStg.AbsMove(m_leftStg.AXIS_TX, m_initPos.inpos.tx);
        m_rightStg.AbsMove(m_rightStg.AXIS_TX, m_initPos.outpos.tx);

        m_leftStg.AbsMove(m_leftStg.AXIS_TY, m_initPos.inpos.ty);
        m_rightStg.AbsMove(m_rightStg.AXIS_TY, m_initPos.outpos.ty);

        m_leftStg.AbsMove(m_leftStg.AXIS_TZ, m_initPos.inpos.tz);
        m_rightStg.AbsMove(m_rightStg.AXIS_TZ, m_initPos.outpos.tz);

        m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_initPos.inpos.z);
        m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_initPos.outpos.z);

        while (m_rightStg.IsMovingOK())
        {
            if (m_stopFlag == true)
            {
                m_leftStg.StopMove();
                m_rightStg.StopMove();
                break;
            }

        }
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
    /// set ToolStripStatus Label text.
    /// </summary>
    /// <param name="_msg">message </param>
    private void writeStatus(string _msg)
    {
        if (InvokeRequired) Invoke((Action)(() => uiStatus.Text = _msg));
        else uiStatus.Text = _msg;
    }



    /// <summary>
    /// Set Label text 
    /// </summary>
    private void updateCurrentPortLabel(string _msg)
    {
        if (InvokeRequired) Invoke((Action)(() => lbCurPort.Text = _msg));
        else lbCurPort.Text = _msg;
    }



    /// <summary>
    /// Init. Pos.으로 스테이지 이동.
    /// </summary>
    private void MoveToInitPos()
    {

        //포지션이 설정되지 않았으면 이동하지 않는다.
        if ((m_initPos.inpos.x == 0) && (m_initPos.inpos.y == 0) &&
            (m_initPos.inpos.z == 0) && (m_initPos.inpos.tx == 0) &&
            (m_initPos.inpos.ty == 0) && (m_initPos.inpos.tz == 0) &&
            (m_initPos.outpos.x == 0) && (m_initPos.outpos.y == 0) &&
            (m_initPos.outpos.z == 0) && (m_initPos.outpos.tx == 0) &&
            (m_initPos.outpos.ty == 0) && (m_initPos.outpos.tz == 0))
            return;


        //이동
        m_leftStg.AbsMove(m_leftStg.AXIS_X, m_initPos.inpos.x);
        m_rightStg.AbsMove(m_rightStg.AXIS_X, m_initPos.outpos.x);

        m_leftStg.AbsMove(m_leftStg.AXIS_Y, m_initPos.inpos.y);
        m_rightStg.AbsMove(m_rightStg.AXIS_Y, m_initPos.outpos.y);

        m_leftStg.AbsMove(m_leftStg.AXIS_TX, m_initPos.inpos.tx);
        m_rightStg.AbsMove(m_rightStg.AXIS_TX, m_initPos.outpos.tx);

        m_leftStg.AbsMove(m_leftStg.AXIS_TY, m_initPos.inpos.ty);
        m_rightStg.AbsMove(m_rightStg.AXIS_TY, m_initPos.outpos.ty);

        m_leftStg.AbsMove(m_leftStg.AXIS_TZ, m_initPos.inpos.tz);
        m_rightStg.AbsMove(m_rightStg.AXIS_TZ, m_initPos.outpos.tz);

        m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_initPos.inpos.z);
        m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_initPos.outpos.z);

        //완료 대기.
        while (m_rightStg.IsMovingOK())
        {

            if (m_stopFlag == true)
            {
                m_leftStg.StopMove();
                m_rightStg.StopMove();
                return;
            }

        }

    }



    /// <summary>
    /// Disable window except stop button.
    /// </summary>
    private void DisWndButStop()
    {
        grpOption.Enabled = false;
        //grpList.Enabled = false;
        grpExecution.Enabled = false;
    }



    /// <summary>
    /// Enable window.
    /// </summary>
    private void EnableWnd()
    {
        grpOption.Enabled = true;
        grpList.Enabled = true;
        grpExecution.Enabled = true;
    }



    /// <summary>
    /// find current Fa position.
    /// </summary>
    /// <returns>port pos. if can't find, it returns -1</returns>
    private int FindCurFaPortPos()
    {

        int curPort = -1;
        double pwr = 0;
        try
        {
            for (int i = 0; i < m_mpm.NumPorts; i++)
            {
                pwr = Unit.MillWattTodBm(m_mpm.ReadPower(i + 1));

                if (pwr >= m_opt.optPwrThre && pwr < 100)
                {
                    curPort = i + 1;
                    break;
                }
            }
        }
        catch
        {
            curPort = -1;
        }


        return curPort;
    }



    /// <summary>
    /// List 출력!!
    /// </summary>
    public void InitDisplayList()
    {

        string[] cols = { "Port No.", "ref. exist" };
        string[] vals = new string[cols.Length];
        hgdvList.DeleteAllRows();
        hgdvList.HanDefaultSetting();
        hgdvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        hgdvList.ReadOnly = true;
        hgdvList.Font = new Font("Source Code Pro", 8, FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        hgdvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        hgdvList.AllowUserToOrderColumns = false;
        hgdvList.AllowUserToResizeRows = false;
        hgdvList.RowHeadersWidth = 25;
        hgdvList.SetColumns(ref cols);

        DisplayList();

        hgdvList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        hgdvList.SelectionChanged += hgdvList_SelectionChanged;

    }



    public void DisplayList(int port = 1)
    {
        hgdvList.Rows.Clear();
        mChange = false;

        string[] cols = { "Port No.", "ref. exist" };
        string[] vals = new string[cols.Length];

        for (int i = 1; i <= m_mpm.NumPorts; i++)
        {
            vals[0] = Convert.ToString(i);

            string strExist = "";
            try
            {
                if (m_ref.RefPow(i) != null) strExist = "Exist";
                else strExist = "empty";
            }
            catch
            {
                strExist = "empty";
            }
            vals[1] = strExist;

            hgdvList.Insert(ref cols, ref vals);
        }

        mChange = true;
        //hgdvList.CurrentCell = hgdvList.Rows[0].Cells[0];
        hgdvList.CurrentCell = hgdvList.Rows[cbStartPort.SelectedIndex].Cells[0];
    }


    WdmGraph initGraph(WdmGraph g)
    {
        g.ShowLegends = false;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.ChartType = SeriesChartType.FastLine;
        g.LineThickness = 1;

        g.ScaleFactorX = 1000;
        g.Cwl = new List<int> { 1271000, 1291000, 1311000, 1331000 };
        //g.IntervalX = 200;//200~10nm
        //g.IntervalOffetX = 21;

        //g.IntervalY = 0.05;
        //g.MinY = -45;

        return g;
    }

    /// <summary>
    /// 현재 ref.를 plot한다.
    /// polt current ref. data.
    /// </summary>
    /// <param name="_port">port no.</param>
    private void PlotCurRef(int _port)
    {
        try
        {
            //clear graph
            lbPlotPortNo.Text = _port.ToString();

            //dBm 값으로 plot~~
            List<double> refPowList = m_ref.RefPow(_port);
            double[] refPows = JeffOptics.mW2dBm(refPowList.ToArray());

            var wave = new double[] { m_ref.startWave, m_ref.stepWave };
            //DataPlot.Plot(uiCurrentGraph, refPows, wave, InspectionGrid.ShiftPeak);
            DataPlot.Plot(_wg, refPows, wave, InspectionGrid.ShiftPeak);
            //if(_testing) DataPlot.Plot(_wgNew, refPows, wave, InspectionGrid.ShiftPeak);
        }
        catch
        {
            //do nothing.
        }

    }

    /// <summary>
    /// New ref.를 plot한다.
    /// polt current ref. data.
    /// </summary>
    /// <param name="_port">port no.</param>
    private void PlotNewRef(int _port)
    {
        try
        {
            //dBm 값으로 plot~~
            List<double> refPowList = m_newRef.RefPow(_port);
            double[] refPows = JeffOptics.mW2dBm(refPowList.ToArray());

            var wave = new double[] { m_newRef.startWave, m_newRef.stepWave };
            //DataPlot.Plot(uiNewGraph, refPows, wave, InspectionGrid.ShiftPeak);
            DataPlot.Plot(_wgNew, refPows, wave, InspectionGrid.ShiftPeak);

            lbCurPort.Text = _port.ToString();
        }
        catch
        {
            //do nothing.
        }
    }



    /// <summary>
    /// load init pos. from xml file.
    /// </summary>
    /// <param name="_filepath">config file path</param>
    /// <returns></returns>
    private initPos LoadInitPos(string _filepath)
    {

        initPos ret = new initPos();
        ret.inpos = new CStageAbsPos();
        ret.outpos = new CStageAbsPos();

        XConfig conf = null;

        try
        {

            conf = new XConfig(_filepath);

            //in
            string strTemp = "";
            strTemp = conf.GetValue("INITPOS_IN_X");
            ret.inpos.x = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_IN_Y");
            ret.inpos.y = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_IN_Z");
            ret.inpos.z = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_IN_TX");
            ret.inpos.x = Convert.ToDouble(strTemp);
            ret.inpos.x = Math.Round(ret.inpos.tx, 4);

            strTemp = conf.GetValue("INITPOS_IN_TY");
            ret.inpos.y = Convert.ToDouble(strTemp);
            ret.inpos.y = Math.Round(ret.inpos.ty, 4);

            strTemp = conf.GetValue("INITPOS_IN_TZ");
            ret.inpos.z = Convert.ToDouble(strTemp);
            ret.inpos.z = Math.Round(ret.inpos.tz, 4);


            //out
            strTemp = conf.GetValue("INITPOS_OUT_X");
            ret.outpos.x = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_OUT_Y");
            ret.outpos.y = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_OUT_Z");
            ret.outpos.z = Convert.ToInt32(strTemp);

            strTemp = conf.GetValue("INITPOS_OUT_TX");
            ret.outpos.x = Convert.ToDouble(strTemp);
            ret.outpos.x = Math.Round(ret.outpos.tx, 4);

            strTemp = conf.GetValue("INITPOS_OUT_TY");
            ret.outpos.y = Convert.ToDouble(strTemp);
            ret.outpos.y = Math.Round(ret.outpos.ty, 4);

            strTemp = conf.GetValue("INITPOS_OUT_TZ");
            ret.outpos.z = Convert.ToDouble(strTemp);
            ret.outpos.z = Math.Round(ret.outpos.tz, 4);

        }
        catch
        {
            ret.inpos.x = 0;
            ret.inpos.y = 0;
            ret.inpos.z = 0;
            ret.inpos.tx = 0;
            ret.inpos.ty = 0;
            ret.inpos.tz = 0;

            ret.outpos.x = 0;
            ret.outpos.y = 0;
            ret.outpos.z = 0;
            ret.outpos.tx = 0;
            ret.outpos.ty = 0;
            ret.outpos.tz = 0;
        }


        if (conf != null)
            conf.Dispose();
        conf = null;

        return ret;

    }



    /// <summary>
    /// save options to xml file.
    /// </summary>
    /// <param name="_opt"></param>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    private bool SaveInitPos(string _filepath)
    {

        bool ret = false;

        XConfig conf = null;
        try

        {
            conf = new XConfig(_filepath);

            //in
            conf.SetValue("INITPOS_IN_X", m_initPos.inpos.x.ToString());
            conf.SetValue("INITPOS_IN_Y", m_initPos.inpos.y.ToString());
            conf.SetValue("INITPOS_IN_Z", m_initPos.inpos.z.ToString());
            conf.SetValue("INITPOS_IN_TX", m_initPos.inpos.tx.ToString());
            conf.SetValue("INITPOS_IN_TY", m_initPos.inpos.ty.ToString());
            conf.SetValue("INITPOS_IN_TZ", m_initPos.inpos.tz.ToString());

            //out
            conf.SetValue("INITPOS_OUT_X", m_initPos.outpos.x.ToString());
            conf.SetValue("INITPOS_OUT_Y", m_initPos.outpos.y.ToString());
            conf.SetValue("INITPOS_OUT_Z", m_initPos.outpos.z.ToString());
            conf.SetValue("INITPOS_OUT_TX", m_initPos.outpos.tx.ToString());
            conf.SetValue("INITPOS_OUT_TY", m_initPos.outpos.ty.ToString());
            conf.SetValue("INITPOS_OUT_TZ", m_initPos.outpos.tz.ToString());

            ret = true;
        }
        catch
        {
            ret = false;
        }


        if (conf != null)
            conf.Dispose();
        conf = null;



        return ret;
    }



    /// <summary>
    /// Init position을 출력한다.
    /// </summary>
    private void DisplayInitPos()
    {
        //in
        lbInitPosInX.Text = m_initPos.inpos.x.ToString();
        lbInitPosInY.Text = m_initPos.inpos.y.ToString();
        lbInitPosInZ.Text = m_initPos.inpos.z.ToString();
        lbInitPosInTx.Text = m_initPos.inpos.tx.ToString();
        lbInitPosInTy.Text = m_initPos.inpos.ty.ToString();
        lbInitPosInTz.Text = m_initPos.inpos.tz.ToString();

        //out
        lbInitPosOutX.Text = m_initPos.outpos.x.ToString();
        lbInitPosOutY.Text = m_initPos.outpos.y.ToString();
        lbInitPosOutZ.Text = m_initPos.outpos.z.ToString();
        lbInitPosOutTx.Text = m_initPos.outpos.tx.ToString();
        lbInitPosOutTy.Text = m_initPos.outpos.ty.ToString();
        lbInitPosOutTz.Text = m_initPos.outpos.tz.ToString();
    }



    /// <summary>
    /// load options from xml file.
    /// </summary>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    private option LoadOptionFromXml(string _filepath)
    {

        option ret = new option();

        try
        {
            XConfig conf = new XConfig(_filepath);

            string strTemp = "";
            strTemp = conf.GetValue("OPTPWR_THRES");
            ret.optPwrThre = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("FA_COREPITCH");
            ret.corePitch = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("ALIGNWAVELENGTH");
            ret.alignWavelen = Convert.ToInt32(strTemp);

            conf.Dispose();
            conf = null;
        }
        catch
        {
            ret.optPwrThre = 0;
            ret.corePitch = 0;
        }

        return ret;
    }



    /// <summary>
    /// save options to xml file.
    /// </summary>
    /// <param name="_opt"></param>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    private bool SaveOptionToXml(option _opt, string _filepath)
    {
        bool ret = false;

        try
        {
            XConfig conf = new XConfig(_filepath);
            conf.SetValue("OPTPWR_THRES", _opt.optPwrThre.ToString());
            conf.SetValue("FA_COREPITCH", _opt.corePitch.ToString());
            conf.SetValue("ALIGNWAVELENGTH", _opt.alignWavelen.ToString());
            conf.Dispose();
            conf = null;

            ret = true;
        }
        catch
        {
            ret = false;
        }

        return ret;
    }


    #endregion




    #region UI Handler


    /// <summary>
    /// move in-FA to out-FA port. position 이동
    /// </summary>
    private void GoPort_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true) return;

        //string msg = "port" + cbStartPort.Text + ": Move?";
        //DialogResult res;
        //res = MessageBox.Show(msg, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        //if (res == DialogResult.Cancel) return;

        try
        {

            //optical powermeter setting.
            var frm = (OpmDisplayForm)Application.OpenForms["PmDisplayForm"];
            frm?.SetFirstCh(Convert.ToInt32(cbStartPort.Text));

            DisWndButStop();

            //start..
            mParam.cmd = CMD.GoPort;
            mParam.startPort = Convert.ToInt32(cbStartPort.Text);

            m_autoEvent.Set();
            Thread.Sleep(10);

        }
        catch (Exception ex)
        {
            EnableWnd();
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// start single-Ref.
    /// </summary>
    private void btnNewStart_single_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true)
            return;

        try
        {

            //optical powermeter setting.
            var frm = (OpmDisplayForm)Application.OpenForms["PmDisplayForm"];
            frm?.SetFirstCh(Convert.ToInt32(cbStartPort.Text));

            DisWndButStop();

            //parameter
            mParam.cmd = CMD.SinglePort;
            mParam.startPort = Convert.ToInt32(cbStartPort.Text);
            mParam.align = chkAlign.Checked;
            //if (m_leftStg == null || m_rightStg == null) mParam.align = false;

            //param averaging
            mParam.NumAvg = textAverage.Text.To<int>();
            mParam.NumMovingAvg = textMA.Text.To<int>();

            //param monitor 
            mParam.tlsAlignment = chkTLSAlignment.Checked;
            mParam.waveList = txtWaveList.Text.Unpack<int>().ToList();
            mParam.McMonitor = uiMcMonitor.Checked;

            //exec.
            m_autoEvent.Set();
            Thread.Sleep(10);

        }
        catch (Exception ex)
        {
            EnableWnd();
            MessageBox.Show(ex.Message);
        }



    }



    /// <summary>
    /// 레퍼런스 적용
    /// </summary>
    private void btnNewApply_single_Click(object sender, EventArgs e)
    {

        //port no.
        int port = 0;
        try
        {
            port = Convert.ToInt32(cbStartPort.Text);
        }
        catch
        {
            string msg = "Invalid Port Number";
            DialogResult res;
            res = MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }



        //data 존재 유무 판단.
        try
        {
            if ((m_newRef == null) || (m_newRef.portCnt == 0)) throw new ApplicationException();
        }
        catch
        {
            MessageBox.Show("No Reference Data.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }


        //new ref. data를 구한다. 
        int startWaveNew = m_newRef.startWave;
        int stopWaveNew = m_newRef.stopWave;
        double stepWaveNew = Math.Round(m_newRef.stepWave, 3);
        PortPowers pwrRefData = null;
        pwrRefData = m_newRef.GetPortData(port);
        if (pwrRefData == null)
        {
            string msg = "No reference data exits for port " + port.ToString();
            MessageBox.Show(msg, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        double monitorPower = m_newRef.MonitorPower(port);


        //apply
        int startWaveCur = m_ref.startWave;
        int stopWaveCur = m_ref.stopWave;
        double stepWaveCur = Math.Round(m_ref.stepWave, 3);

        if ((startWaveNew != startWaveCur) || (stopWaveNew != stopWaveCur) || (stepWaveNew != stepWaveCur))
        {
            string msg = "Differenct reference parameter!!\nOverwrite?";
            var res = MessageBox.Show(msg, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            if (res == DialogResult.No) return;

            m_ref.Clear();
        }


        if (m_ref.portCnt == 0) m_ref.SetWavelength(startWaveNew, stopWaveNew, stepWaveNew);

        m_ref.SetPortData(pwrRefData);
        m_newRef.DeletePortData(port);

        //monitor
        m_ref.SetMonitorPower(port, monitorPower);


        //save to file.
        if (!m_ref.SaveToTxt(Application.StartupPath + "\\refNonpol.txt"))
        {
            string msg = "Failed to save reference data!!";
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }


        //완료처리.
        PlotCurRef(port);
        _wgNew.Clear();

    }



    /// <summary>
    /// 옵션 설정값 적용
    /// </summary>
    private void btnOptApply_Click(object sender, EventArgs e)
    {
        try
        {
            m_opt.optPwrThre = Convert.ToInt32(txtOptPwrThres.Text);
            m_opt.corePitch = Convert.ToInt32(txtFAcorepitch.Text);
            m_opt.alignWavelen = Convert.ToInt32(txtAlignWavelen.Text);


            if (false == SaveOptionToXml(m_opt, Application.StartupPath + "\\config\\conf_ref.xml"))
                throw new ApplicationException();


            MessageBox.Show("Applied!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch
        {
            MessageBox.Show("Failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


    }



    bool mChange = true;

    /// <summary>
    /// grid - Port 선택
    /// </summary>
    private void hgdvList_SelectionChanged(object sender, EventArgs e)
    {

        if (!mChange) return;

        try
        {
            int portNo = Convert.ToInt32(hgdvList.CurrentRow.Cells[0].Value);
            PlotCurRef(portNo);
            PlotNewRef(portNo);

            cbStartPort.Text = portNo.ToString();

            //optical powermeter setting.
            var frm = (OpmDisplayForm)Application.OpenForms["PmDisplayForm"];
            frm?.SetFirstCh(Convert.ToInt32(cbStartPort.Text));

        }
        catch
        {
            //do nothing.
        }
    }



    private void btnOptCancel_Click(object sender, EventArgs e)
    {
        txtOptPwrThres.Text = Convert.ToString(m_opt.optPwrThre);
        txtFAcorepitch.Text = Convert.ToString(m_opt.corePitch);
    }



    private void btnInitPosGo_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true)
            return;


        //confirm.
        string msg = "Move to initial position??";
        DialogResult res;
        res = MessageBox.Show(msg, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (res == DialogResult.Cancel) return;



        //포지션이 설정되지 않았으면 이동하지 않는다.
        if ((m_initPos.inpos.x == 0) && (m_initPos.inpos.y == 0) &&
            (m_initPos.inpos.z == 0) && (m_initPos.inpos.tx == 0) &&
            (m_initPos.inpos.ty == 0) && (m_initPos.inpos.tz == 0) &&
            (m_initPos.outpos.x == 0) && (m_initPos.outpos.y == 0) &&
            (m_initPos.outpos.z == 0) && (m_initPos.outpos.tx == 0) &&
            (m_initPos.outpos.ty == 0) && (m_initPos.outpos.tz == 0))
        {

            MessageBox.Show("Initial position is not set", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }


        //execution.
        try
        {

            DisWndButStop();

            //Go
            mParam.cmd = CMD.GoInitPos;
            m_autoEvent.Set();
            Thread.Sleep(10);


        }
        catch (Exception ex)
        {
            EnableWnd();
            MessageBox.Show(ex.ToString());
        }


    }



    private void btnInitPosApply_Click(object sender, EventArgs e)
    {
        //설정.
        m_initPos.inpos = m_leftStg.GetAbsPositions();
        m_initPos.outpos = m_rightStg.GetAbsPositions();


        //저장.
        string confFilepath = Application.StartupPath + "\\config\\conf_ref.xml";
        if (!SaveInitPos(confFilepath))
        {
            MessageBox.Show("Failed to save Init position!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        //출력 
        DisplayInitPos();
        MessageBox.Show("Initial position saved!!");
    }



    private void btnNewStart_Multi_Click(object sender, EventArgs e)
    {
    }



    #endregion

    private void btnStop_Click(object sender, EventArgs e)
    {
        m_stopFlag = true;
        if (mAlign != null) mAlign.mStopFlag = true;
    }

    private void cbStoptPort_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
