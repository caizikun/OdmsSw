using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Jeffsoft;
using Neon.Aligner;
using WaveformPlot = NationalInstruments.UI.WaveformPlot;
using AxisMode = NationalInstruments.UI.AxisMode;
using Range = NationalInstruments.UI.Range;
using FormatString = NationalInstruments.UI.FormatString;
using FormatStringMode = NationalInstruments.UI.FormatStringMode;
using NationalInstruments.UI.WindowsForms;
using Free302.TnMLibrary.DataAnalysis;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;

public partial class ReferenceForm : Form//, IFormCanClosed
{


    #region definition

    private const int RESMW = 9;// 10^(-9) mW
    private const int RESDBM = 3;// 10^(-3) dBm]

    private const int DEFAULT_FACOREPITCH = 127;
    private const int DEFAULT_OPTPWRTHRES = 0;
    private const int DEFAULT_ALIGNWAVELEN = 1550;
    private const int DEFAULT_AVGREP = 5;

    enum CommandCode { MoveToPort, FindMaxPolAngle, MeasureSingle, MeasureAll, MoveToInitPosition }

    #endregion




    #region structure/innor class

    private struct DutParam
    {
        public int PowerThreshold;  //[dBm] 
        public int OutPitch;        //[um] Output FA Corepicth
        public int waveAlign;       //[nm]Wavelength for Alignment;
        public int NumAvg;          //반복 횟수.
        public int NumMovingAvg;

        public bool IsAligned;
        public bool FoundPolPos;

        public int waveStart;
        public int waveStop;
        public double waveStep;
        public int tlsPowerLevel;
        public int pmGainLevel;
    }


    private struct ThreadParam
    {
        public CommandCode cmd;
        public int portStart;
        public int portEnd;
        public int portCurrentAligned;
        public double polBaseAngle;   //polariation filter pos.
        public bool doAlign;
        

        //status message
        public int infoPort;
        public int infoAvg;

        //TEST - Pol State
        public bool polLeftCircular;
        public bool polMinus45Diagonal;
    }

    #endregion




    #region member variables

    private Osw mOsw;
    private Itls mTls;
    private IoptMultimeter mPm;
    private Istage mLeft;
    private Istage mRight;

    private SweepLogicDwdm mSweep;
    private AlignLogic mAlign;

    private DutParam mDutParam;

    bool m_stopFlag;
    bool m_isRuning; //running:true , stop :false
    private ThreadParam mParam;
    private AutoResetEvent mEvent;
    private Thread mThread;
    private AlignPosition mInitPosition;

    #endregion




    #region ==== Class Framework ====

    MyLogic mLogic;

    public ReferenceForm()
    {
        InitializeComponent();

        mLogic = MyLogic.Instance;

        //device
        mSweep = CGlobal.SweepSystem;
        mAlign = CGlobal.Alignment;
        mOsw = CGlobal.Switch;
        mTls = CGlobal.Tls8164;
        mPm = CGlobal.Pm8164;
        mLeft = CGlobal.LeftAligner;
        mRight = CGlobal.RightAligner;

        mDutParam = new DutParam();

        //config
        mConfigFile = Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
        mConfig = new XConfig(mConfigFile);

        btnLoadReference.Click += btnLoadReference_Click;
        btnExportReference.Click += btnExportReference_Click;
    }
    
    XConfig mConfig;
    string mConfigFile;
    const string CONFIG_FILE_NAME = "config_reference.xml";


    private void Form_Load(object sender, EventArgs e)
    {
        try
        {
            //uiPcGroup.Enabled = GlobalParam.InBandC;
            initPcUi();

            //config
            loadDutParam();
            initNewRef();

            //stage position
            mInitPosition = LoadInitPosition(mConfig);
            displayInitPosition();

            //graph
            initGraph();
            displayPortCombo();

            findAlignedPmPort();

            displayReference();

            //source 설정.
            frmSourceController frmSourCont = MyLogic.CreateAndShow<frmSourceController>();

            //Disable optical source controller
            frmSourCont.DeactiveUpdate();

            mOsw?.SetToAlign();
            if(CGlobal.mIsLocalTls) mTls?.SetTlsOutPwr(mDutParam.tlsPowerLevel);

            //Enable optical source controller
            frmSourCont.ActiveUpdate();

			if(mLeft == null && mRight == null)
			{
				groupInitPos.Enabled = false;
				chkDoAlignment.Checked = false;
				chkDoAlignment.Enabled = false;
				btnMoveToPort.Enabled = false;
			}

			if (CGlobal.mPcType != CGlobal.PcType.C_8169a) uiPcGroup.Enabled = false;
			if (CGlobal.mPcType != CGlobal.PcType.O_PSG100) groupPcPSG100.Enabled = false;

            //start 가동.
            mEvent = new AutoResetEvent(false);
            mThread = new Thread(ThreadFunc);
            if (mTls != null && mPm != null) mThread.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.Form_Load():\n{ex.Message}");
        }
    }


    void loadDutParam()
    {
        //Dut Parameters
        mDutParam.OutPitch = int.Parse(mConfig.GetValue("FA_COREPITCH"));

        //measure param
        mDutParam.NumAvg = int.Parse(mConfig.GetValue("AVGREP"));
        mDutParam.NumMovingAvg = int.Parse(mConfig.GetValue("NUM_MOVING_AVERAGING"));

        //string strRange = mConfig.GetValue(CGlobal.InBandC ? "WAVE_RANGE_C" : "WAVE_RANGE_O");
        string strRange = mConfig.GetValue((CGlobal.mBand == CGlobal.WlBand.CBand) ? "WAVE_RANGE_C" : "WAVE_RANGE_O");
        int[] waveRange = MyLogic.parseIntArray(strRange);
        mDutParam.waveStart = waveRange[0];
        mDutParam.waveStop = waveRange[1];
        mDutParam.waveStep = Math.Round(waveRange[2] / 1000.0, 3);//pm-->nm
        mDutParam.waveAlign = waveRange[3];

        //device param
        mDutParam.tlsPowerLevel = int.Parse(mConfig.GetValue("TLS_POWER_LEVEL"));
        mDutParam.pmGainLevel = int.Parse(mConfig.GetValue("PM_GAIN_LEVEL"));
        mDutParam.PowerThreshold = int.Parse(mConfig.GetValue("OPTPWR_THRES"));

        if ((mDutParam.OutPitch == 0) && (mDutParam.PowerThreshold == 0))
        {
            mDutParam.PowerThreshold = DEFAULT_OPTPWRTHRES;
            mDutParam.OutPitch = DEFAULT_FACOREPITCH;
            mDutParam.waveAlign = DEFAULT_ALIGNWAVELEN;
            mDutParam.NumAvg = DEFAULT_AVGREP;

            mDutParam.IsAligned = true;
            mDutParam.FoundPolPos = false;
        }

        //display
        displayDutParam();
    }


    void displayDutParam()
    {
        //display
        txtOptPwrThres.Text = mDutParam.PowerThreshold.ToString();
        txtFAcorepitch.Text = mDutParam.OutPitch.ToString();
        txtAlignWavelen.Text = mDutParam.waveAlign.ToString();
        txtAvgRep.Text = mDutParam.NumAvg.ToString();
        txtNumMovingAveraging.Text = mDutParam.NumMovingAvg.ToString();
        chkDoAlignment.Checked = mDutParam.IsAligned;

        txtWaveStart.Text = mDutParam.waveStart.ToString();
        txtWaveStop.Text = mDutParam.waveStop.ToString();
        txtWaveStep.Text = mDutParam.waveStep.ToString("0.000");
        txtTlsPower.Text = mDutParam.tlsPowerLevel.ToString();
        txtPmGainLevel.Text = mDutParam.pmGainLevel.ToString();
    }


    void applyDutParamFromUi()
    {
        mDutParam.PowerThreshold = Convert.ToInt32(txtOptPwrThres.Text);
        mDutParam.OutPitch = Convert.ToInt32(txtFAcorepitch.Text);
        mDutParam.waveAlign = Convert.ToInt32(txtAlignWavelen.Text);
        mDutParam.NumAvg = Convert.ToInt32(txtAvgRep.Text);
        mDutParam.NumMovingAvg = int.Parse(txtNumMovingAveraging.Text);

        mDutParam.waveStart = int.Parse(txtWaveStart.Text.Trim());
        mDutParam.waveStop = int.Parse(txtWaveStop.Text.Trim());
        mDutParam.waveStep = double.Parse(txtWaveStep.Text.Trim());
        mDutParam.tlsPowerLevel = int.Parse(txtTlsPower.Text.Trim());
        mDutParam.pmGainLevel = int.Parse(txtPmGainLevel.Text.Trim());

        CGlobal.TlsPower = mDutParam.tlsPowerLevel;
    }


    bool saveDutParam()
    {
        mConfig.SetValue("FA_COREPITCH", mDutParam.OutPitch.ToString());
        
        mConfig.SetValue("AVGREP", mDutParam.NumAvg.ToString());
        mConfig.SetValue("NUM_MOVING_AVERAGING", mDutParam.NumMovingAvg.ToString());

        int[] waveRange = { mDutParam.waveStart, mDutParam.waveStop, (int)(mDutParam.waveStep * 1000), mDutParam.waveAlign };
        string waveString = "";
        foreach (int x in waveRange) waveString += x.ToString() + ",";
        waveString = waveString.Remove(waveString.Length - 1, 1);
        //mConfig.SetValue(CGlobal.InBandC ? "WAVE_RANGE_C" : "WAVE_RANGE_O", waveString);
        mConfig.SetValue((CGlobal.mBand == CGlobal.WlBand.CBand) ? "WAVE_RANGE_C" : "WAVE_RANGE_O", waveString);

        mConfig.SetValue("TLS_POWER_LEVEL", mDutParam.tlsPowerLevel.ToString());
        mConfig.SetValue("PM_GAIN_LEVEL", mDutParam.pmGainLevel.ToString());
        mConfig.SetValue("OPTPWR_THRES", mDutParam.PowerThreshold.ToString());

        //mConfig.Dispose();
        return true;
    }


    void initNewRef()
    {
        mRefNew = new ReferenceData();
        mRefNew.SetWavelength(mDutParam.waveStart, mDutParam.waveStop, mDutParam.waveStep);
        mRefNew.PolBaseAngle = mParam.polBaseAngle;//mRef.PolBaseAngle;
		mRefNew.NumPols = CGlobal.mUsingOpc ? 4 : 1;
    }


    private void displayInitPosition()
    {
        //in
        lbInitPosInX.Text = mInitPosition.In.x.ToString();
        lbInitPosInY.Text = mInitPosition.In.y.ToString();
        lbInitPosInZ.Text = mInitPosition.In.z.ToString();
        lbInitPosInTx.Text = mInitPosition.In.tx.ToString();
        lbInitPosInTy.Text = mInitPosition.In.ty.ToString();
        lbInitPosInTz.Text = mInitPosition.In.tz.ToString();

        //out
        lbInitPosOutX.Text = mInitPosition.Out.x.ToString();
        lbInitPosOutY.Text = mInitPosition.Out.y.ToString();
        lbInitPosOutZ.Text = mInitPosition.Out.z.ToString();
        lbInitPosOutTx.Text = mInitPosition.Out.tx.ToString();
        lbInitPosOutTy.Text = mInitPosition.Out.ty.ToString();
        lbInitPosOutTz.Text = mInitPosition.Out.tz.ToString();
    }

	static object[] Null_Ch_List = new object[] { };
    void displayPortCombo()
    {
        //excution
        txtPmPortStart.Items.Clear();
        txtPmPortStop.Items.Clear();
		var chs = (mPm?.ChList) ?? Null_Ch_List;
		int numPmPort = chs.Length;

        txtPmPortStart.Items.AddRange(chs);
        txtPmPortStop.Items.AddRange(chs);

        if (numPmPort == 0) txtPmPortStart.SelectedIndex = txtPmPortStop.SelectedIndex = -1;
        else
        {
            txtPmPortStart.SelectedIndex = 0;
            txtPmPortStop.SelectedIndex = numPmPort - 1;
        }
    }


    AlignPosition LoadInitPosition(XConfig mConfig)
    {
        AlignPosition position = new AlignPosition();
        try
        {
            //in
            position.In.x = int.Parse(mConfig.GetValue("INITPOS_IN_X"));
            position.In.y = int.Parse(mConfig.GetValue("INITPOS_IN_Y"));
            position.In.z = int.Parse(mConfig.GetValue("INITPOS_IN_Z"));
            position.In.tx = double.Parse(mConfig.GetValue("INITPOS_IN_TX"));
            position.In.ty = double.Parse(mConfig.GetValue("INITPOS_IN_TY"));
            position.In.tz = double.Parse(mConfig.GetValue("INITPOS_IN_TZ"));

            //out
            position.Out.x = int.Parse(mConfig.GetValue("INITPOS_OUT_X"));
            position.Out.y = int.Parse(mConfig.GetValue("INITPOS_OUT_Y"));
            position.Out.z = int.Parse(mConfig.GetValue("INITPOS_OUT_Z"));
            position.Out.tx = double.Parse(mConfig.GetValue("INITPOS_OUT_TX"));
            position.Out.ty = double.Parse(mConfig.GetValue("INITPOS_OUT_TY"));
            position.Out.tz = double.Parse(mConfig.GetValue("INITPOS_OUT_TZ"));

            //other.
            position.Other.x = int.Parse(mConfig.GetValue("INITPOS_OTH_X"));

            ////in
            //m_closedPos.In.z = int.Parse(mConfig.GetValue("CLOSEPOS_IN"));
            ////out
            //m_closedPos.Out.z = int.Parse(mConfig.GetValue("CLOSEPOS_OUT"));
        }
        catch
        {
            position.In.x = 0;
            position.In.y = 0;
            position.In.z = 0;
            position.In.tx = 0;
            position.In.ty = 0;
            position.In.tz = 0;
            position.Out.x = 0;
            position.Out.y = 0;
            position.Out.z = 0;
            position.Out.tx = 0;
            position.Out.ty = 0;
            position.Out.tz = 0;
        }

        return position;
    }


    bool SaveInitPosition(XConfig mConfig, AlignPosition position)
    {
        //in
        mConfig.SetValue("INITPOS_IN_X", position.In.x.ToString());
        mConfig.SetValue("INITPOS_IN_Y", position.In.y.ToString());
        mConfig.SetValue("INITPOS_IN_Z", position.In.z.ToString());
        mConfig.SetValue("INITPOS_IN_TX", position.In.tx.ToString());
        mConfig.SetValue("INITPOS_IN_TY", position.In.ty.ToString());
        mConfig.SetValue("INITPOS_IN_TZ", position.In.tz.ToString());

        //out
        mConfig.SetValue("INITPOS_OUT_X", position.Out.x.ToString());
        mConfig.SetValue("INITPOS_OUT_Y", position.Out.y.ToString());
        mConfig.SetValue("INITPOS_OUT_Z", position.Out.z.ToString());
        mConfig.SetValue("INITPOS_OUT_TX", position.Out.tx.ToString());
        mConfig.SetValue("INITPOS_OUT_TY", position.Out.ty.ToString());
        mConfig.SetValue("INITPOS_OUT_TZ", position.Out.tz.ToString());

        //other.
        mConfig.SetValue("INITPOS_OTH_X", position.Other.x.ToString());

        return true;
    }

    
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;
        
        //optical powermeter setting.
        try
        {
            //int alignPort = MyLogic.CreateAndShow<AlignForm>().alignPort;
            //MyLogic.CreateAndShow<PmDisplayForm>().SetFirstCh(alignPort);

            //save config.
            saveDutParam();
            SaveInitPosition(mConfig, mInitPosition);
            mConfig.Dispose();//write to file

            Neon.Dwdm.Properties.Settings.Default.Save();

            //thread 종료 및 마무리.
            if (mThread != null)
            {
                mThread.Abort();
                if(!mThread.ThreadState.HasFlag(ThreadState.Unstarted)) mThread.Join();
                mThread = null;
            }
        }
        catch
        {
            //do nothing.
        }
    }


    #endregion




    #region ==== Referecen File Handler ====


    ReferenceData mRef;
    ReferenceData mRefNew;
    string mReferenceButtonText = "Reference File : {0} ({1} {2})";
    
    private void displayReference()
    {
        if (mLogic.Reference == null) return;

        mRef = mLogic.Reference;

        if (!mRef.Loaded) return;

        string fileName = mRef.TextFile;
        DateTime time = File.GetLastWriteTime(fileName);
        btnLoadReference.Text = string.Format(mReferenceButtonText, Path.GetFileName(fileName), time.ToShortDateString(), time.ToString("HH:mm:ss"));

        //plot:
        txtPolPos.Text = Convert.ToString(mRef.PolBaseAngle);
        mParam.polBaseAngle = mRef.PolBaseAngle;
        DisplayPortGrid();
        PlotCurRef(1);
    }


    private void btnExportReference_Click(object sender, EventArgs e)
    {
        try { mLogic.ExportReference(); }
        catch (Exception ex)
        { MessageBox.Show(ex.Message); }
    }


    private void btnLoadReference_Click(object sender, EventArgs e)
    {
        try
        {
            mLogic.LoadReference(true);
            displayReference();

            MyLogic.Instance.SaveConfig();
        }
        catch (Exception ex)
        { MessageBox.Show(ex.Message); }
    }


    #endregion




    #region ==== Measure : Alignment & Measurement ====

    
    private void btnMeasureSingle_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;
        if (mThread == null || mPm == null)
        {
            displayStatusAsync($"장치가 준비되지 않았으므로 작업<Single Measure>을 중단함.");
            return;
        }                

        try
        {
            unlockControlUi(false);

            //parameter
            mParam.portStart = Convert.ToInt32(txtPmPortStart.Text);
            mParam.cmd = CommandCode.MeasureSingle;
            mParam.doAlign = chkDoAlignment.Checked;
            mParam.polLeftCircular = uiPol_LeftCircular.Checked;
            mParam.polMinus45Diagonal = uiPol_Minus45Diagonal.Checked;

            mRefNew.PolBaseAngle = mParam.polBaseAngle;

            //optical powermeter setting.
            MyLogic.CreateAndShow<OpmDisplayForm>(true, false)?.SetFirstCh(mParam.portStart);

            //exec.
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }        
    }


    private void btnMeasureAll_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;
        if (mThread == null || mPm == null)
        {
            displayStatusAsync($"장치가 준비되지 않았으므로 작업<Single Measure>을 중단함.");
            return;
        }
        

        try
        {
            unlockControlUi(false);

            //parameter
            mParam.portStart = Convert.ToInt32(txtPmPortStart.Text);
            mParam.portEnd = Convert.ToInt32(txtPmPortStop.Text);
            mParam.cmd = CommandCode.MeasureAll;
            if (!chkDoAlignment.Checked)
            {
                if (MessageBox.Show($"Align이 체크되지 않았습니다. Align 없이 여러 포트를 측정할까요?",
                        "Align 확인", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
            }
            mParam.doAlign = chkDoAlignment.Checked;

            mRefNew.PolBaseAngle = mParam.polBaseAngle;
            mParam.polLeftCircular = uiPol_Minus45Diagonal.Checked;
            mParam.polMinus45Diagonal = uiPol_Minus45Diagonal.Checked;

            //optical powermeter setting.
            MyLogic.CreateAndShow<OpmDisplayForm>(true, false)?.SetFirstCh(mParam.portStart);

            //exec.
            mEvent.Set();
            Thread.Sleep(10);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }        
    }




    async void ThreadFunc()
    {
        while (true)
        {
            //시작 신호 대기
            m_isRuning = false;
            mEvent.WaitOne();			

            m_isRuning = true;
            m_stopFlag = false;

			try
            {
				switch (mParam.cmd)
                {
                    case CommandCode.FindMaxPolAngle:		await FindPolPos(); break;
                    case CommandCode.MoveToPort:			MoveToPort();		break;
                    case CommandCode.MeasureSingle:			await RunSingle();	break;
                    case CommandCode.MoveToInitPosition:	moveToInitPos();	break;
                    case CommandCode.MeasureAll:			RunMulti();			break;
                }
                if (m_stopFlag == true) MessageBox.Show("명령이 중지되었습니다");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ReferenceForm.ThreadFunc():\n{ex.Message}\n\n{ex.StackTrace}");
            }
			finally
			{
				//완료후 UI 처리
				Invoke((Action)(() => unlockControlUi(true)));
			}
		}//while (true)
    }




    private async Task RunSingle()
    {
		var pmForm = MyLogic.CreateAndShow<OpmDisplayForm>();
		var sourceForm = MyLogic.CreateAndShow<frmSourceController>();

		try
		{
			//disable other form 
			if (pmForm != null) Invoke((Action)(() => pmForm.DisplayOff()));
			if (sourceForm != null) Invoke((Action)(() => sourceForm.DeactiveUpdate()));

			//set source to BLS
			mOsw?.SetToAlign();

            int curPort = mParam.portStart;

            //Port 검사 
            if (mParam.doAlign)
            {
                updateInfoAsync("Finding aligned port");
                int alignedPort = findAlignedPmPort();
                if (alignedPort != curPort)
                {
                    var response = MessageBox.Show($"측정할 포트=<{mParam.portStart}>와 정렬된 포트=<{alignedPort}>가 다릅니다.\n" 
                        +$"Yes=<{mParam.portStart} || No=<{alignedPort}",
                        "포트확인", MessageBoxButtons.YesNoCancel);

                    if (DialogResult.Cancel == response) return;//취소
                    else if (response == DialogResult.No)
                    {
                        mParam.portStart = curPort = alignedPort;//No    
                        Invoke((Action)(() => gridSelectPort(curPort)));
                    }
                }
            }

            //update status
            mParam.infoPort = curPort;

            if (mParam.doAlign && !CGlobal.TestMode)
            {
                //Approach
                updateInfoAsync("Approaching Left FAB");
                ApproachIn();
                if (m_stopFlag == true) return;
            }

			var numPols = CGlobal.mUsingOpc ? 4 : 1;
            var numAvg = mDutParam.NumAvg;
            PortPowers avgPortData = null;
            Averaging[] avg = { new Averaging(numAvg, 1), new Averaging(numAvg, 1), new Averaging(numAvg, 1), new Averaging(numAvg, 1) };
            for (int j = 0; j < numAvg; j++)
            {
                //update status
                mParam.infoAvg = j + 1;

                //alignment
                bool alignOk = true;
                if (mParam.doAlign && !CGlobal.TestMode)
                {
                    updateInfoAsync("Aligning");
                    alignOk = doAlign(curPort, mDutParam.PowerThreshold);
                    if (!alignOk) updateInfoAsync("Aligning failed.");
                }

                //measurement
                if (alignOk)
                {
                    updateInfoAsync("Measuring");
                    PortPowers power = await doMeasure(curPort, mDutParam.waveStart, mDutParam.waveStop, mDutParam.waveStep, mDutParam.pmGainLevel, mParam.polBaseAngle);

					if (power != null)
                    {
                        var plotData = new List<List<double>>();
                        for (int pol = 0; pol < numPols; pol++)
                        {
                            avg[pol].AddData(power[pol]);//data
                            plotData.Add(power[pol]);//graph
                        }
                        avgPortData = power;						

						//update graph
						updateInfoAsync($"Plotting port=<{mParam.portStart}>, avg=<{mParam.infoAvg}>");
                        Invoke((Action)(() => updateGraph(uiNewGraph, plotData, mRefNew.WaveStart, mRefNew.WaveStep)));
                    }
                    else updateInfoAsync("Measurement failed.");
                }

                //for each avg
                if (m_stopFlag) return;                
            }

            if (avgPortData != null)
            {
                mRefNew.SetPortData(avgPortData);
                //auto backup current measured data
                backupPort(avgPortData, mDutParam.waveStart, mDutParam.waveStop, mDutParam.waveStep, mRefNew.PolBaseAngle);
                //plot..
                updateInfoAsync($"Plotting port=<{mParam.portStart}>");
                Invoke((Action)(() => PlotNewRef(mParam.portStart)));
            }
        }
        finally
        {
            //Enable other form
            if (pmForm != null) Invoke((Action)(() => pmForm.DisplayOn()));
            if (sourceForm != null) Invoke((Action)(() => sourceForm.ActiveUpdate()));

            mParam.infoPort = 0;
            mParam.infoAvg = 0;
        }
    }


    const string BackupFolder = "backup";
    const string BackupFileName = "ref";
    void backupPort(PortPowers data, int waveStart, int waveStop, double waveStep, double polAngle)
    {
        try
        {
            var refData = new ReferenceData();
			refData.NumPols = CGlobal.mUsingOpc ? 4 : 1;
			refData.PolBaseAngle = polAngle;
            refData.SetWavelength(waveStart, waveStop, waveStep);
            refData.SetPortData(data);

            DateTime d = DateTime.Now;

            string folder = Path.Combine(Application.StartupPath, BackupFolder);

            if (File.Exists(folder)) File.Delete(folder);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string file = Path.Combine(Application.StartupPath, BackupFolder, 
                $"{BackupFileName}_port={data.Port}__{d.ToString("yyyy-MM-dd")}_{d.ToString("HH-mm-ss")}.{MyLogic.REFERENCE_EXT}");
            refData.SaveToTxt(file);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.backupPort():\\nn{ex.Message}\n\n{ex.StackTrace}");
        }
    }


    void displayStatusAsync(string msg)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");
        Invoke((Action)(() => uiStatusLabel.Text = $"[{time}] : {msg}"));
    }


    private void updateInfoAsync(string msg, int gain = 0, int pol = 0, int port = 0)
    {
        string message = "";
        if (gain != 0) message += $"Gain=<{gain}>";
        if (pol != 0) message += $", Pol=<{pol}>";
        if (port != 0) message += $", Port=<{port}>";
        if (msg != null) message += $" : {msg}";
        displayStatusAsync(message);
    }


    private async void RunMulti()
    {
        var sourceForm = MyLogic.CreateAndShow<frmSourceController>();
        var pmForm = MyLogic.CreateAndShow<OpmDisplayForm>();

        try
        {

            #region 초기위치 저장
            CStageAbsPos  curPosIn = null;
            CStageAbsPos  curPosOut = null;
            CStageAbsPos  retPosIn = null;
            CStageAbsPos  retPosOut = null;

            if (!CGlobal.TestMode)
            {
                curPosIn = mLeft.GetAbsPositions();
                curPosOut = mRight.GetAbsPositions();
                retPosIn = (CStageAbsPos )curPosIn.Clone();
                retPosOut = (CStageAbsPos )curPosOut.Clone();
            }
            #endregion


            //source 변경 BLS로.
            mOsw?.SetToAlign();

            #region //Port 검사 -- start port
            if (mParam.doAlign)
            {
                updateInfoAsync("Finding aligned port");
                int alignedPort = findAlignedPmPort();
                if (alignedPort != mParam.portStart)
                {
                    updateInfoAsync($"시작위치 Port=<{mParam.portStart}>으로 이동");

                    int dist = mDutParam.OutPitch * (mParam.portStart - alignedPort);
                    mLeft.RelMove(mLeft.AXIS_Z, -100);
                    mLeft.RelMove(mLeft.AXIS_X, dist);
                    mLeft.WaitForIdle(mLeft.AXIS_X);
                    mLeft.RelMove(mLeft.AXIS_Z, 85);
                    mLeft.WaitForIdle(mLeft.AXIS_X);
                }
            }
            int curPort = mParam.portStart;

            #endregion


            #region //Approach
            if (mParam.doAlign && !CGlobal.TestMode)
            {
                //Approach
                updateInfoAsync($"Z-Approachign in-FAB");
                ApproachIn();
            }
            if (m_stopFlag == true) return;
            #endregion


            //buffer init
            int numPort = mParam.portEnd - mParam.portStart + 1;

            //loop each port
            for (int p = 0; p < numPort; p++)//for each port
            {
                //update status
                mParam.infoPort = curPort;

                //disable other form 
                if (pmForm != null) Invoke((Action)(() => pmForm.DisplayOff()));
                if (sourceForm != null) Invoke((Action)(() => sourceForm.DeactiveUpdate()));

                var numAvg = mDutParam.NumAvg;
                PortPowers avgPortData = null;
                Averaging[] avg = { new Averaging(numAvg, 1), new Averaging(numAvg, 1), new Averaging(numAvg, 1), new Averaging(numAvg, 1) };
                for (int j = 0; j < numAvg; j++)//averaging
                {
                    //update status
                    mParam.infoAvg = j + 1;

                    //alignment
                    bool alignOk = true;
                    if (mParam.doAlign && !CGlobal.TestMode)
                    {
                        updateInfoAsync("Aligning");
                        alignOk = doAlign(curPort, mDutParam.PowerThreshold);
                        if (!alignOk) updateInfoAsync("Aligning failed.");
                    }
                    
                    //measurement
                    if (alignOk)
                    {
                        updateInfoAsync("Measuring");
                        PortPowers portData = await doMeasure(curPort, mDutParam.waveStart, mDutParam.waveStop, mDutParam.waveStep, mDutParam.pmGainLevel, mParam.polBaseAngle);
                        if (portData != null)
                        {
                            var tempData = new List<List<double>>();
                            for (int pol = 0; pol < 4; pol++)
                            {
                                avg[pol].AddData(portData[pol]);//data
                                tempData.Add(portData[pol]);//graph
                            }
                            avgPortData = portData;

                            //update graph
                            updateInfoAsync($"Plotting port=<{curPort}>, avg=<{mParam.infoAvg}>");
                            Invoke((Action)(() => updateGraph(uiNewGraph, tempData, mRefNew.WaveStart, mRefNew.WaveStep)));
                        }
                        else updateInfoAsync("Measurement failed.");
                    }
                    if (m_stopFlag == true) break;
                }//each averaging times

                //averaging.
                if (avgPortData != null)
                {
                    mRefNew.SetPortData(avgPortData);
                    //auto backup current measured data
                    backupPort(avgPortData, mDutParam.waveStart, mDutParam.waveStop, mDutParam.waveStep, mRefNew.PolBaseAngle);
                    //plot..
                    updateInfoAsync($"Ploting port=<{curPort}>");
                    Invoke((Action)(() => PlotCurRef(curPort)));
                    Invoke((Action)(() => PlotNewRef(curPort)));
                }                

                //Enable other form
                if (pmForm != null) pmForm.DisplayOn();
                if (sourceForm != null) sourceForm.ActiveUpdate();

                //******************************
                //Move Next Port
                if (curPort != mParam.portEnd)
                {
                    updateInfoAsync($"Moving to next port <{curPort + 1}>");
                    if(!CGlobal.TestMode) moveNextPort(mDutParam.OutPitch);
                    curPort++;

                    if (pmForm != null) pmForm.SetFirstCh(curPort);
                    Invoke((Action)(() => uiCurrentAlignedPort.Text = curPort.ToString()));
                }
            }//for each port


            #region //완료 처리.

            if (m_stopFlag == true)
            {
                //초기위치로 이동?
                string msg = "작업이 취소되었습니다. \n초기 위치로 이동(Yes), 멈춤(No)";
                DialogResult dialRes = DialogResult.No;
                dialRes = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialRes == DialogResult.Yes)
                {
                    if(!CGlobal.TestMode) MoveTo(retPosIn, retPosOut);
                    curPort = mParam.portStart;
                    if (pmForm != null) pmForm.SetFirstCh(mParam.portStart);
                }
            }
            else
            {
                //초기 위치로 이동.
                if(!CGlobal.TestMode) MoveTo(retPosIn, retPosOut);
                curPort = mParam.portStart;
                if (pmForm != null) pmForm.SetFirstCh(mParam.portStart);
            }
            Invoke((Action)(() => uiCurrentAlignedPort.Text = curPort.ToString()));

            #endregion

        }
        finally
        {
            //Enable other form
            if (pmForm != null) Invoke((Action)(() => pmForm.DisplayOn()));
            if (sourceForm != null) Invoke((Action)(() => sourceForm.ActiveUpdate()));

            mParam.infoPort = 0;
            mParam.infoAvg = 0;
        }
    }

    
    private bool doAlign(int _port, int _thresPowr)
    {
        const int SYNCSEARCHRNG = 100; //[um]
        const double SYNCSEARCHSTEP = 4;//[um]

        try
        {
            Func<bool> checkPower = () => Unit.MillWattTodBm(mPm.ReadPower(_port)) >= _thresPowr;

            //Semi-Align 상태인가? (= Align가능한가?)
            if (!checkPower())
            {
                //Sync Search 시도.(광을 찾은 상태가 아니면 )
                mAlign.SyncXySearch(_port, SYNCSEARCHRNG, SYNCSEARCHSTEP, _thresPowr);
                if (!checkPower()) return false;
                if (m_stopFlag == true) return false;
            }

            //fine search out 
            //mAlign.XySearch(mRight.stageNo, _port, XYSEARCHSTEP);
            CGlobal.XySearchParamLeft.Port = _port;
            mAlign.XySearch(CGlobal.XySearchParamLeft);

            return !m_stopFlag;
        }
        catch {  }
        return false;
    }


    private async Task<PortPowers> doMeasure(
		int port, int waveStart, int waveStop, double waveStep, int pmGainLevel, double polBaseAngle)
    {
        mOsw?.SetToTls();

        //sweep
        try
        {
            mSweep.mReporter = updateInfoAsync;
            mSweep.mIsLeftCircular = mParam.polLeftCircular;
            mSweep.mIsNegitiveDiagonal = mParam.polMinus45Diagonal;
            PortPowers polPower;

            if(!CGlobal.mIsLocalTls) await mSweep.Register(true);
            var polPowerList = await mSweep.MeasurePower(new int[] { port }, new int[] { pmGainLevel }, waveStart, waveStop, waveStep, polBaseAngle);

			polPower = polPowerList.Find(p => p.Port == port);

            return polPower;
        }
        finally
        {
            mOsw?.SetToAlign();
            if (!CGlobal.mIsLocalTls) await mSweep.Register(false);
            mSweep.mReporter = null;
        }
    }


    private void btnApplySingle_Click(object sender, EventArgs e)
    {
        try
        {
            //port no.
            int port = int.Parse(uiCurrentSelectedPort.Text);

            //data 존재 유무 판단.
            if ((mRefNew == null) || (mRefNew.NumPorts == 0) || !mRefNew.PortList.Contains(port))
            {
                MessageBox.Show($"Port=<{port}에 대한 측정 데이터가 없습니다.>");
                return;
            }

            if (mRef == null) mRef = new ReferenceData();
            if (mRef.NumPorts == 0)
            {
                mRef.SetWavelength(mRefNew.WaveStart, mRefNew.WaveStop, mRefNew.WaveStep);
                mRef.PolBaseAngle = mRefNew.PolBaseAngle;
            }

            //apply
            PortPowers newData = mRefNew.GetPortData(port);
            if (!mRef.compareWave(mRefNew))
            {
                string msg = "기존 데이터와 Sweep parameter가 다릅니다.\n기존 데이터를 모두 지우고 계속 진행할까요?";
                DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res != DialogResult.OK) return;
                mRef.ClearData();
                mRef.SetWavelength(mRefNew.WaveStart, mRefNew.WaveStop, mRefNew.WaveStep);
            }
            mRef.PolBaseAngle = mRefNew.PolBaseAngle;
            mRef.SetPortData(newData);
            //mRefNew.RemovePortData(port);///160713TEST

            //save to file.
            if (mRef.TextFile == null || mRef.TextFile.Length == 0)
            {
                //현재 레퍼러스 파일이 지정되지 않았을 경우 - 새로 저장
                mLogic.ExportReference();//dialog showing
                mLogic.LoadReference(false);
                displayReference();
            }
            if (!mRef.SaveToTxt(mRef.TextFile))
            {
                MessageBox.Show($"레퍼런스 file=<{mRef.TextFile}> 저장 실패.");
                return;
            }

            //완료처리.
            PlotCurRef(port);
            //uiNewGraph.ClearData();
            DisplayPortGrid(port);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.btnApplySingle_Click():\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    
    private void btnApplyAll_Click(object sender, EventArgs e)
    {
        try
        {
            if ((mRefNew == null) || (mRefNew.NumPorts == 0)) return;

            //apply
            if (mRef == null) mRef = new ReferenceData();
            if (mRef.NumPorts == 0)
            {
                mRef.SetWavelength(mRefNew.WaveStart, mRefNew.WaveStop, mRefNew.WaveStep);
                mRef.PolBaseAngle = mRefNew.PolBaseAngle;
            }

            if (!mRef.compareWave(mRefNew))
            {
                string msg = "기존 데이터와 Sweep parameter가 다릅니다.\n기존 데이터를 모두 지우고 계속 진행할까요?";
                DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res != DialogResult.OK) return;
                mRef.ClearData();
                mRef.SetWavelength(mRefNew.WaveStart, mRefNew.WaveStop, mRefNew.WaveStep);
            }

            //apply
            mRef.PolBaseAngle = mRefNew.PolBaseAngle;
            foreach (var port in mRefNew.PortList)
            {
                var portData = mRefNew.GetPortData(port);
                if (portData != null) mRef.SetPortData(portData);
            }            

            //save to file.
            if (mRef.TextFile == null || mRef.TextFile.Length == 0)
            {
                //현재 레퍼러스 파일이 지정되지 않았을 경우 - 새로 저장
                mLogic.ExportReference();//dialog showing
                mLogic.LoadReference(false);
                displayReference();
            }
            if (!mRef.SaveToTxt(mRef.TextFile))
            {
                MessageBox.Show($"레퍼런스 file=<{mRef.TextFile}> 저장 실패.");
                return;
            }            

            //완료처리.
            PlotCurRef(mRefNew.PortList.Min());
            //uiNewGraph.ClearData();
            DisplayPortGrid();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.btnApplyAll_Click():\n{ex.Message}\n{ex.StackTrace}");
        }        
    }


    public void DisplayPortGrid(int displayPort = 1)
    {
        int numPmPort = 40;

        var refPortList = mRef.PortList;

        if (uiPortGrid.RowCount != numPmPort)
        {
            uiPortGrid.Rows.Clear();
            uiPortGrid.RowCount = numPmPort;
        }

        for (int p = 1; p <= numPmPort; p++)
        {
            //var row = new object[] { p, refPortList.Contains(p) ? "○" : "×" };
            //uiPortGrid.Rows.Add(row);
            uiPortGrid.Rows[p - 1].Cells[0].Value = p;
            uiPortGrid.Rows[p - 1].Cells[1].Value = refPortList.Contains(p) ? "○" : "×";
        }

        int rowIndex = displayPort - 1;
        uiPortGrid.Rows[rowIndex].Selected = true;
        if (!uiPortGrid.Rows[rowIndex].Displayed) uiPortGrid.FirstDisplayedScrollingRowIndex = rowIndex;
    }


    #endregion




    #region ---- 8169 Max Pol ----

    private void btnFindPolAngle_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;
        if (mThread == null || mTls == null || mPm == null)
        {
            displayStatusAsync($"장치가 준비되지 않았으므로 작업<Find Pol. Angle>을 중단함.");
            return;
        }

        //string msg = "Polarization filter Position을 찾으시겠습니까?";
        //DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        //if (res == DialogResult.Cancel) return;

        try
        {
            unlockControlUi(false);

            //start..
            mParam.cmd = CommandCode.FindMaxPolAngle;
            mParam.portStart = Convert.ToInt32(txtPmPortStart.Text);

            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            unlockControlUi(true);
        }
        finally
        {
        }
    }


    /// <summary>
    /// polarization filter position을 찾는다.
    /// </summary>
    private async Task FindPolPos()
    {
        OpmDisplayForm form = null;
        try
        {
            form = MyLogic.CreateAndShow<OpmDisplayForm>();

            mOsw?.SetToTls();
            form.DisplayOff();

            if (!CGlobal.mIsLocalPc) await mSweep.Register(true);

            mSweep.mFindPolReporter = updateFindPol;
            double[] max = await mSweep.FindMaxPolPos(mParam.portStart);

            updateFindPol(max[0], max[1]);
        }
        finally
        {
            if (!CGlobal.mIsLocalPc) await mSweep.Register(false);
            mOsw?.SetToAlign();
            mSweep.mFindPolReporter = null;
            if (form != null) form.DisplayOn();
        }
    }


    void updateFindPol(double pos, double power)
    {
        Invoke((Action)(() =>
        {
            txtPolPos.Text = pos.ToString();
            txtPolPower.Text = power.ToString();
        }));
    }


    private void btnApplyPolAngle_Click(object sender, EventArgs e)
    {
        string msg = "pol. filter pos.를 적용할까요?";
        DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (res != DialogResult.OK) return;

        mParam.polBaseAngle = Math.Round(double.Parse(txtPolPos.Text));///160713Fix
        mRefNew.PolBaseAngle = mParam.polBaseAngle;
    }


    #endregion




    #region private method
    

    /// <summary>
    /// input을 approach한다.
    /// </summary>
    private void ApproachIn()
    {
        const int BACKDIST = 50;
        const int ALIGNDIST = 10;

        mAlign.Approach(AppStageId.Left, ALIGNDIST, BACKDIST);
    }




    /// <summary>
    /// 초기 위치로 이동한다.
    /// </summary>
    private void MoveTo(CStageAbsPos  _posIn, CStageAbsPos  _posOut)
    {

        const int STAGEOPENDIST = 5000;

        try
        {

            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, STAGEOPENDIST * (-1));
            //m_rightStg.RelMove(m_rightStg.AXIS_Z, STAGEOPENDIST * (-1));
            mLeft.WaitForIdle();


            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, _posIn.y);
            mRight.AbsMove(mRight.AXIS_Y, _posOut.y);


            //tz 이동.
            mRight.AbsMove(mRight.AXIS_TZ, _posOut.tz);


            //X축 이동
            mLeft.AbsMove(mLeft.AXIS_X, _posIn.x);
            mRight.AbsMove(mRight.AXIS_X, _posOut.x);
            mRight.WaitForIdle(mRight.AXIS_Z);


            //Z축 이동
            mLeft.AbsMove(mLeft.AXIS_Z, (_posIn.z - 200));
            mRight.AbsMove(mRight.AXIS_Z, (_posOut.z - 200));
            mRight.WaitForIdle(mRight.AXIS_Z);


        }
        catch
        {
            //do nothing
        }

    }




    /// <summary>
    /// 다음 칩으로 이동.
    /// </summary>
    /// <param name="_pitch"></param>
    private void moveNextPort(int _pitch)
    {
        const int BACKDIST = 50;
        const int ALIGNDIST = 10;

        //z축 후퇴
        mLeft.RelMove(mLeft.AXIS_Z, BACKDIST * (-1));
        if (m_stopFlag == true) return;

        //x축 포트 피치 만큼 이동
        mLeft.RelMove(mLeft.AXIS_X, _pitch);
        mLeft.WaitForIdle(mLeft.AXIS_X);
        if (m_stopFlag == true) return;

        //Z축 접근
        //Left-stage ZApproach
        mAlign.ZappSingleStage(AlignLogic.LEFT_STAGE);
        if (m_stopFlag == true) return;

        //searching할 거리 이격.
        mLeft.RelMove(mLeft.AXIS_Z, ALIGNDIST * (-1));
        mLeft.WaitForIdle(mLeft.AXIS_Z);
        if (m_stopFlag == true) return;
    }
    



    private void unlockControlUi(bool enable)
    {
        grpOption.Enabled = enable;
        //grpList.Enabled = enable;
        uiMeasureGroup.Enabled = enable;
    }




    //명령을 중지한다.
    private void btnStop_Click(object sender, EventArgs e)
    {
        if (m_isRuning != true) return;
        m_stopFlag = true;
    }




    /// <summary>
    /// mInitPosition 으로 스테이지 이동.
    /// </summary>
    private void moveToInitPos()
    {

        //포지션이 설정되지 않았으면 이동하지 않는다.
        if ((mInitPosition.In.x == 0) && (mInitPosition.In.y == 0) &&
            (mInitPosition.In.z == 0) && (mInitPosition.In.tx == 0) &&
            (mInitPosition.In.ty == 0) && (mInitPosition.In.tz == 0) &&
            (mInitPosition.Out.x == 0) && (mInitPosition.Out.y == 0) &&
            (mInitPosition.Out.z == 0) && (mInitPosition.Out.tx == 0) &&
            (mInitPosition.Out.ty == 0) && (mInitPosition.Out.tz == 0))
            return;


        //이동
        mLeft.AbsMove(mLeft.AXIS_X, mInitPosition.In.x);
        mRight.AbsMove(mRight.AXIS_X, mInitPosition.Out.x);

        mLeft.AbsMove(mLeft.AXIS_Y, mInitPosition.In.y);
        mRight.AbsMove(mRight.AXIS_Y, mInitPosition.Out.y);

        mLeft.AbsMove(mLeft.AXIS_TX, mInitPosition.In.tx);
        mRight.AbsMove(mRight.AXIS_TX, mInitPosition.Out.tx);

        mLeft.AbsMove(mLeft.AXIS_TY, mInitPosition.In.ty);
        mRight.AbsMove(mRight.AXIS_TY, mInitPosition.Out.ty);

        mLeft.AbsMove(mLeft.AXIS_TZ, mInitPosition.In.tz);
        mRight.AbsMove(mRight.AXIS_TZ, mInitPosition.Out.tz);

        mLeft.AbsMove(mLeft.AXIS_Z, mInitPosition.In.z);
        mRight.AbsMove(mRight.AXIS_Z, mInitPosition.Out.z);

        //완료 대기.
        while (mRight.IsMovingOK())
        {
            if (m_stopFlag == true)
            {
                mLeft.StopMove();
                mRight.StopMove();
                return;
            }
        }
    }


    #endregion




    #region ==== Graph ====


    /// <summary>
    /// 현재 ref.를 plot한다.
    /// polt current ref. data.
    /// </summary>
    /// <param name="_port">port no.</param>
    private void PlotCurRef(int _port)
    {
        uiPlotPortCurrent.Text = _port.ToString();
        if (mRef == null) return;
        List<List<double>> data = mRef.GetPowerByPort(_port);
        updateGraph(uiCurrentGraph, data, mRef.WaveStart, mRef.WaveStep);
    }


    /// <summary>
    /// New ref.를 plot한다.
    /// polt current ref. data.
    /// </summary>
    /// <param name="_port">port no.</param>
    private void PlotNewRef(int _port)
    {
        uiPlotPortNew.Text = _port.ToString();
        if (mRefNew == null) return;

        var wave = new double[] { mRefNew.WaveStart, mRefNew.WaveStep };
        List<List<double>> data = mRefNew.GetPowerByPort(_port);
        foreach(var l in data)
        {
            var buffer = l.ToArray();
            //DataPlot.Plot()
        }

        updateGraph(uiNewGraph, data, mRefNew.WaveStart, mRefNew.WaveStep);
    }
    

    Dictionary<int, WaveformPlot> mPlotCurrent; //<pol, Plot> dic
    Dictionary<int, WaveformPlot> mPlotNew;     //<pol, Plot> dic
    Dictionary<int, Color> mColor;              //<pol, Color> dic

    void initGraph()
    {
        
        initColor();
        mPlotCurrent = new Dictionary<int, WaveformPlot>();
        mPlotNew = new Dictionary<int, WaveformPlot>();
        initPlots();
    }
    WdmGraph initGraph(WdmGraph g)
    {
        g.ShowLegends = false;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.ChartType = SeriesChartType.FastLine;
        g.LineThickness = 1;

        g.ScaleFactorX = 1000;
        g.Cwl = new List<int> { 1271000, 1291000, 1311000, 1331000 };
        //g.IntervalY = 0.05;
        //g.MinY = -45;

        return g;
    }

    void initColor()
    {
        mColor = new Dictionary<int, Color>();
        mColor.Add(0, Color.Tomato);
        mColor.Add(1, Color.Gold);
        mColor.Add(2, Color.LimeGreen);
        mColor.Add(3, Color.DeepSkyBlue);
        mColor.Add(4, Color.Red);
        mColor.Add(5, Color.Green);
        mColor.Add(6, Color.Orange);
        mColor.Add(7, Color.Violet);
    }
    void initPlots()
    {
        WaveformGraph graph = uiCurrentGraph;
        WaveformPlot plotOrigin = graph.Plots[0];
        plotOrigin.ClearData();
        for (int pol = 0; pol < 4; pol++)
        {
            WaveformPlot plot = (WaveformPlot)plotOrigin.Clone();

            plot.XAxis = plotOrigin.XAxis;
            plot.XAxis.Mode = AxisMode.AutoScaleLoose;
            plot.YAxis = plotOrigin.YAxis;
            plot.YAxis.Mode = AxisMode.AutoScaleLoose;

            plot.LineColor = mColor[pol];

            mPlotCurrent.Add(pol, plot);
            mPlotNew.Add(pol, (WaveformPlot)plot.Clone());
        }
        uiCurrentGraph.Plots.Clear();
        uiNewGraph.Plots.Clear();

        uiCurrentGraph.Plots.AddRange(mPlotCurrent.Values.ToArray());
        uiNewGraph.Plots.AddRange(mPlotNew.Values.ToArray());
    }
    void updateGraph(WaveformGraph graph, List<List<double>> data_mW, double waveStart, double waveStep)
    {
        if (mColor == null) return;

        if (data_mW == null)
        {
            foreach (WaveformPlot p in graph.Plots) p.ClearData();
            return;
        }

        double[][] data = toDbm(data_mW);

        double yMin = double.MaxValue;
        double yMax = double.MinValue;
        for (int i = 0; i < data.Length; i++)
        {
            yMin = Math.Min(yMin, data[i].Min());
            yMax = Math.Max(yMax, data[i].Max());
        }

        if (yMax.Equals(double.NaN)) return;
        if (yMin.Equals(double.NaN)) return;

        double dy = (yMax - yMin) * 0.1;
        if (dy == 0) dy = 1;

        Range yRange = new Range(yMin - dy, yMax + dy);
        int numDigit = (int)Math.Round(Math.Log10(dy));
        if (numDigit > 0) numDigit = 0;

        FormatString yLabelFormat = new FormatString(FormatStringMode.Numeric, $"F{-numDigit}");
        Dictionary<int, WaveformPlot> dic = graph.Equals(uiCurrentGraph) ? mPlotCurrent : mPlotNew;

		var numPols = CGlobal.mUsingOpc ? 4 : 1;

		for (int pol = 0; pol < numPols; pol++)
			//for (int pol = 0; pol < dic.Values.Count; pol++)
        {
            var plot = dic[pol];
            plot.DefaultStart = waveStart;
            plot.DefaultIncrement = waveStep;

            plot.YAxis.Range = yRange;
            plot.YAxis.MajorDivisions.LabelFormat = yLabelFormat;
            plot.PlotY(data[pol]);
        }
        graph.Refresh();
    }

    double[][] toDbm(List<List<double>> data)
    {
        double[][] dbm = new double[data.Count][];
        for (int i = 0; i < data.Count; i++)
        {
            dbm[i] = new double[data[i].Count];
            for (int j = 0; j < data[i].Count; j++)
                dbm[i][j] = Unit.MillWattTodBm(data[i][j]);
        }
        return dbm;
    }

    //void initGraph(WaveformGraph Graph)
    //{
    //    mPlot = new Dictionary<int, WaveformPlot>();
    //    foreach (PmCh ch in mRefLogic.ChList)
    //    {
    //        ScatterPlot plotOrigin = Graph.Plots[0];
    //        ScatterPlot plot = (ScatterPlot)plotOrigin.Clone();

    //        plot.LineColor = mColor[ch];

    //        plot.XAxis = plotOrigin.XAxis;
    //        plot.XAxis.Mode = AxisMode.AutoScaleLoose;

    //        plot.YAxis = plotOrigin.YAxis;
    //        plot.YAxis.Mode = AxisMode.AutoScaleLoose;

    //        mPlot.Add(ch, plot);
    //        plot.ClearData();
    //    };

    //    Graph.Plots.Clear();
    //    foreach (ScatterPlot plot in mPlot.Values) Graph.Plots.Add(plot);
    //}

    //void updateGraph(PmCh ch, double[] x, double[] y, bool fixX, bool showWl, AxisCustomDivision[] divX = null)
    //{
    //    ScatterGraph Graph = this.mGraphScatter;

    //    mPlot[ch].XAxis.MajorDivisions.TickVisible = true;
    //    mPlot[ch].XAxis.MajorDivisions.LabelVisible = true;
    //    mPlot[ch].XAxis.CustomDivisions.Clear();

    //    mPlot[ch].PointColor = mPlot[ch].LineColor;

    //    if (fixX)
    //    {
    //        mPlot[ch].XAxis.Mode = AxisMode.Fixed;
    //        mPlot[ch].XAxis.Range = new Range(x[0] - 1, x[x.Length - 1] + 1);

    //        mPlot[ch].YAxis.Mode = AxisMode.Fixed;

    //        if (showWl)
    //        {
    //            mPlot[ch].XAxis.MajorDivisions.TickVisible = false;
    //            mPlot[ch].XAxis.MajorDivisions.LabelVisible = false;
    //            mPlot[ch].XAxis.CustomDivisions.AddRange(divX);
    //        }
    //        else mPlot[ch].XAxis.CustomDivisions.Clear();
    //    }
    //    else
    //    {
    //        mPlot[ch].XAxis.Mode = AxisMode.AutoScaleLoose;
    //    }

    //    mPlot[ch].PlotXY(x, y);
    //    Graph.Update();
    //}

    //private void BtnRefreshGraph_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        Dictionary<PmCh, double[]> data = mRefLogic.GetGraphData_ChWl();

    //        double[] xWl = mRefLogic.GetGraphX(true);
    //        double[] xIndex = mRefLogic.GetGraphX(false);
    //        AxisCustomDivision[] divX = mRefLogic.GetDivX();

    //        //calc y range
    //        double[] minList = new double[data.Count];
    //        double[] maxList = new double[data.Count];
    //        int i = 0;
    //        foreach (PmCh ch in data.Keys)
    //        {
    //            minList[i] = data[ch].Min();
    //            maxList[i++] = data[ch].Max();
    //        }

    //        double yMin = minList.Min();
    //        double yMax = maxList.Max();
    //        double dy = (yMax - yMin) * 0.1;
    //        if (dy == 0) dy = 1;
    //        Range yRange = new Range(yMin - dy, yMax + dy);
    //        int numDigit = (int)Math.Round(Math.Log10(dy));
    //        if (numDigit > 0) numDigit = 0;
    //        FormatString yLabelFormat = new FormatString(FormatStringMode.Numeric, $"F{-numDigit}");
    //        foreach (PmCh ch in data.Keys)
    //        {
    //            mPlot[ch].YAxis.Range = yRange;
    //            mPlot[ch].YAxis.MajorDivisions.LabelFormat = yLabelFormat;
    //        }

    //        if (!chkFixRangeX.Checked)
    //        {
    //            if (!chkShowWl.Checked)
    //                foreach (PmCh ch in data.Keys) updateGraph(ch, xIndex, data[ch], false, false);
    //            else foreach (PmCh ch in data.Keys) updateGraph(ch, xWl, data[ch], false, true);
    //        }
    //        else
    //        {
    //            if (!chkShowWl.Checked)
    //                foreach (PmCh ch in data.Keys) updateGraph(ch, xIndex, data[ch], true, false);
    //            else foreach (PmCh ch in data.Keys) updateGraph(ch, xIndex, data[ch], true, true, divX);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MyDebug.ShowErrorMessage("BtnTest_Click()", ex);
    //    }
    //}


    #endregion




    #region ==== Port List Handler ====


    private void txtPmPortStart_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtPmPortStart.Tag != null && (bool)txtPmPortStart.Tag) return;//이벤트 진행중....

            txtPmPortStart.Tag = true;//이벤트 시작

            int port = (int)txtPmPortStart.SelectedItem;
            gridSelectPort(port);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.txtPmPortStart_SelectedIndexChanged()\n{ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            txtPmPortStart.Tag = false;//이벤트 종료
        }
    }


    private void uiPortGrid_SelectionChanged(object sender, EventArgs e)
    {
		var pmForm = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);

		try
        {
            if (uiPortGrid.Tag != null && (bool)uiPortGrid.Tag) return;//이벤트 진행중....

            uiPortGrid.Tag = true;//이벤트 시작
			
			if (uiPortGrid.CurrentRow != null && uiPortGrid.CurrentRow.Cells[0].Value != null)
			{
				var curPort = (int)uiPortGrid.CurrentRow.Cells[0].Value;
				//그래프 출력
				gridSelectPort(curPort);
				//PM display - port 변경
				if (pmForm != null && curPort <= mPm.ChList.Length) pmForm.SetFirstCh(curPort);
			}

		}
        catch (Exception ex)
        {
            MessageBox.Show($"ReferenceForm.uiPortGrid_SelectionChanged()\n{ex.Message}\n{ex.StackTrace}");
        }
        finally
        {
            uiPortGrid.Tag = false;//이벤트 종료
        }
    }


    void gridSelectPort(int port)
    {
        //graph
        PlotCurRef(port);
        PlotNewRef(port);

        //label
        uiCurrentSelectedPort.Text = port.ToString();

        //port combo
        if (txtPmPortStart.Tag == null || !(bool)txtPmPortStart.Tag)
        {
            try
            {
                txtPmPortStart.Tag = true;
                txtPmPortStart.SelectedIndex = txtPmPortStart.Items.IndexOf(port);
            }
            finally { txtPmPortStart.Tag = false; }
        }

        //grid port
        if (uiPortGrid.Tag == null || !(bool)uiPortGrid.Tag)
        {
            try
            {
                uiPortGrid.Tag = true;
                if (uiPortGrid.RowCount >= port)
                {
                    int rowIndex = port - 1;
                    uiPortGrid.Rows[rowIndex].Selected = true;
                    if (!uiPortGrid.Rows[rowIndex].Displayed) uiPortGrid.FirstDisplayedScrollingRowIndex = rowIndex;
                }
            }
            finally { uiPortGrid.Tag = false; }
        }
    }

    
    private void btnMoveToPort_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;
        if (mThread == null || mTls == null || mPm == null)
        {
            displayStatusAsync($"장치가 준비되지 않았으므로 작업<Move To Port>을 중단함.");
            return;
        }

        try
        {
            MyLogic.CreateAndShow<OpmDisplayForm>().SetFirstCh(int.Parse(txtPmPortStart.Text));

            unlockControlUi(false);//lock ui

            //start..
            mParam.cmd = CommandCode.MoveToPort;
            mParam.portStart = int.Parse(txtPmPortStart.Text);            

            mEvent.Set();
            Thread.Sleep(10);

        }
        catch (Exception ex)
        {
            unlockControlUi(true);
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            uiCurrentAlignedPort.Text = mParam.portCurrentAligned.ToString();
        }
    }


    void MoveToPort()
    {
        //position 
        int portTarget = mParam.portStart;
        if (portTarget <= 0) return;

        int portCurrent = mParam.portCurrentAligned = findAlignedPmPort();
        if (portCurrent <= 0)
        {
            MessageBox.Show($"현재 정렬된 포트가 불분명합니다.\n정렬 후에 다시 시도하십시오.", 
                "현재 포트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (portCurrent == portTarget) return;

        updateInfoAsync($"Movint to port <{portCurrent}> -> <{portTarget}>");

        //Target Port로 이동
        int dist = mDutParam.OutPitch * (portTarget - portCurrent);
        mLeft.RelMove(mLeft.AXIS_Z, -100);
        mLeft.RelMove(mLeft.AXIS_X, dist);
        mLeft.WaitForIdle(mLeft.AXIS_X);
        mLeft.RelMove(mLeft.AXIS_Z, 85);//
        mLeft.WaitForIdle(mLeft.AXIS_X);

        displayCurrentAlignedPort(portTarget);
        //updateStatusAsync($"MoveToPort: {portCurrent} -> {portTarget} 시작");
    }


    private int findAlignedPmPort()
    {
        if (mPm == null) return 0;

        int[] pmPorts = mPm.ChList.Cast<int>().ToArray();
        for (int i = 0; i < mPm.NumPorts; i++)
        {
            int port = pmPorts[i];
            if (Unit.MillWattTodBm(mPm.ReadPower(port)) >= mDutParam.PowerThreshold)
            {
                displayCurrentAlignedPort(port);
                return port;
            }
        }
        displayCurrentAlignedPort(0);
        return 0;
    }


    void displayCurrentAlignedPort(int port)
    {
        Invoke((Action)(() => uiCurrentAlignedPort.Text = port.ToString()));
    }

    #endregion




    private void btnApplyDutParam_Click(object sender, EventArgs e)
    {
        try
        {
            applyDutParamFromUi();

            initNewRef();

            MessageBox.Show("옵션이 설정되었습니다.");
        }
        catch
        {
            MessageBox.Show("옵션을 설정하는데 실패하였습니다.");
        }
    }




    private void btnCancelDutParam_Click(object sender, EventArgs e)
    {
        txtOptPwrThres.Text = Convert.ToString(mDutParam.PowerThreshold);
        txtFAcorepitch.Text = Convert.ToString(mDutParam.OutPitch);
    }

    


    /// <summary>
    /// 현재 위치를 초기위치로 설정.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnInitPosApply_Click(object sender, EventArgs e)
    {
        string msg = "현재 스테이지 포지션을 초기위치로 설정하시겠습니까?";
        DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.No) return;

        //설정.
        mInitPosition.In = mLeft.GetAbsPositions();
        mInitPosition.Out = mRight.GetAbsPositions();
        SaveInitPosition(mConfig, mInitPosition);

        //출력 
        displayInitPosition();
        MessageBox.Show("설정 완료!!", "확인");
    }



    /// <summary>
    /// 초기위치로 이동한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnInitPosGo_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;

        //confirm.
        string msg = "스테이지를 레퍼런스 초기 위치로 이동하시겠습니까?";
        DialogResult res;
        res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (res == DialogResult.Cancel) return;


        //포지션이 설정되지 않았으면 이동하지 않는다.
        if ((mInitPosition.In.x == 0) && (mInitPosition.In.y == 0) &&
            (mInitPosition.In.z == 0) && (mInitPosition.In.tx == 0) &&
            (mInitPosition.In.ty == 0) && (mInitPosition.In.tz == 0) &&
            (mInitPosition.Out.x == 0) && (mInitPosition.Out.y == 0) &&
            (mInitPosition.Out.z == 0) && (mInitPosition.Out.tx == 0) &&
            (mInitPosition.Out.ty == 0) && (mInitPosition.Out.tz == 0))
        {
            MessageBox.Show("위치값이 설정되지 않았습니다", "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        //execution.
        try
        {
            unlockControlUi(false);

            //Go
            mParam.cmd = CommandCode.MoveToInitPosition;
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            unlockControlUi(true);
            MessageBox.Show(ex.ToString());
        }
    }

    

    private void chkAlign_CheckedChanged(object sender, EventArgs e)
    {
        btnMeasureAll.Enabled = chkDoAlignment.Checked;
    }

        

    private void txt_ToggleReadOnly(object sender, EventArgs e)
    {
        if(Form.ModifierKeys == Keys.Control) ((TextBox)sender).ReadOnly = !((TextBox)sender).ReadOnly;
    }



    private void uiPcPolRight_CheckedChanged(object sender, EventArgs e)
    {
        var ui = (CheckBox)sender;
        if (ui.Tag != null && (bool)ui.Tag) return;

        uiPol_LeftCircular.Tag = true;
        //uiPcPolLeft.Tag = true;

        try
        {
            //if (ui == uiPcPolLeft) uiPcPolRight.Checked = !uiPcPolLeft.Checked;
            //if (ui == uiPcPolRight) uiPcPolLeft.Checked = !uiPcPolRight.Checked;
        }
        finally
        {
            uiPol_LeftCircular.Tag = false;
            //uiPcPolPlus45.Tag = false;
        }
    }



    RadioButton[] mPolUi = new RadioButton[6];
    void initPcUi()
    {
        uiPol_H.Tag = 0;
        uiPol_V.Tag = 1;
        uiPol_p45.Tag = 2;
        uiPol_n45.Tag = 3;
        uiPol_R.Tag = 4;
        uiPol_L.Tag = 5;

        mPolUi[0] = uiPol_H;
        mPolUi[1] = uiPol_V;
        mPolUi[2] = uiPol_p45;
        mPolUi[3] = uiPol_n45;
        mPolUi[4] = uiPol_R;
        mPolUi[5] = uiPol_L;
    }



    private void uiPol_Set_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < 6; i++)
			if (mPolUi[i].Checked) mSweep.SetObandPol(i);
    }


	private void btnObandPcOption_Click(object sender, EventArgs e)
	{
		var height = this.Height;
		if (height == 800)
		{
			this.Height = 730;
			btnObandPcOption.Text = "▼";
		}
		else
		{
			this.Height = 800;
			btnObandPcOption.Text = "▲";
		}
		
	}

    private void btnLoadReference_Click_1(object sender, EventArgs e)
    {

    }
}


