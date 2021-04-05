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
using Jeffsoft;
using Free302.TnMLibrary.DataAnalysis;
using Free302.MyLibrary.Utility;
using Neon.Aligner;
using System.IO;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;

public partial class MuxFaForm : Form
{

    #region definition

    private const int RESMW = 9;							// 10^(-9) mW
    private const int RESDBM = 3;							// 10^(-3) dBm

    private int mTlsPower = CGlobal.TlsParam.Power;
    private int mWaveStart = CGlobal.TlsParam.WaveStart;
    private int mWaveStop = CGlobal.TlsParam.WaveStop;
    private double mWaveStep = CGlobal.TlsParam.WaveStep;
    private int mSweepSpeed = CGlobal.TlsParam.Speed;
    private int mGain1 = CGlobal.PmGain[0];			        //[dBm]
    private int mGain2 = CGlobal.PmGain[1];			        //[dBm]

    const int APPROACHBUFFDIST = 40;                        //[um]    
    int mApproachDistance = 20;                             //[um]
    
    private enum WAVELEN { CH1 = 1271, CH2 = 1291, CH3 = 1311, CH4 = 1331 }
    private enum FaType { SMF, MMF }
    private enum ChipDirection { Forward, Reverse}
    private enum AutoSave { Full, Range }
    private enum monitorType { NCMMC, NCMETRI }

    #endregion




    #region structure/innor class


    private struct threadParam
    {
        public string[] chipList;

        public int chipWidth;							    //칩 간 간격
        public int outPitch;							    //output FA corepitch [um]     
        public bool doClad;					                //clading mode power 제거
        public int numGains;							    //number of gains. 
        public List<int> gainList;						    //Gains

        public FaType fa;									//SMF or MMF
        public ChipDirection chDirect;						//channel direction
        public int[] DutChCwl_nm;							//Align Wavelength
        public int[] PmPorts;

        public bool autoSave;
        public AutoSave autoSaveType;						//full or range.
        public int saveRngStart;						    //save range start wavelengh.
        public int saveRngStop;							    //save range stop wavelengh.

        public bool alignment;							    //alignment. <-- uncheck하면 1칩만 측정됨.
        public bool faArrangement;						    //FA arrangement
        public bool measurement;						    //measurement.
        public bool roll;                                   //Roll

        public bool doMonitor;                              //monitor Port 측정
		public bool doMonitorClading;                       //monitor Port Clading 측정
		public double monitorCladingDist;					//monitor Port Clading 거리
        public bool MonitorMcTls;							//TLS로 MC monitor 측정
        public monitorType MonitorType;
        public double[] MonitorRefPower_dBm;
		public int[] MonitorCwl_nm;
		public int[] MonitorPmPorts;

		public bool outFabLoop;
        public bool singleSweep;

        public string saveFolderPath;
    }



    private class Cmeasure
    {
        public string chipNo;
        public DateTime msrTime;						    //측정 시간.
        public AlignPosition pos;							//aligned position.
        public DutData sd;
    }
    

    #endregion



	
    #region member variables


    private ReferenceDataNp m_ref;

    private Itls m_tls;
    private IoptMultimeter m_mpm;
    private Istage mLeft;
    private Istage mRight;
    private Istage mCenter;

    public SweepLogic mSweep;
    private AlignLogic mAlign;

    bool mTlsAlignment;                                 //TLS사용 Alignment (Local TLS 사용시)
    bool m_stopFlag;
    bool m_isRuning;									//running : true , stop : false

    private threadParam mParam;
    private AutoResetEvent m_autoEvent;
    private Thread m_thread;

    private CprogRes m_procState;
    private List<Cmeasure> m_msrList;
    
    private Dictionary<monitorType, int> m_MonitorPort1Dist;

    bool mMoveNextByCenter = false;                     //Next Chip 이동 : Center or Left Right
    int mRightAlignInterval = 8;                        //Output FAB Align

    //test mode by DrBae
    bool mTestMode = false;
	int mCurrentTest = 1;
	bool mAutoReturn = true;
    int mCladDeltaX = 100;//μm

    
    #endregion




    #region Thread function
    
    frmDistSensViewer frmDistSens = null;
    OpmDisplayForm frmDigitalPwr = null;
	uiStageControl frmStageCont = null;
    frmAlignStatus frmStatus = null;
	frmSourceController frmSourCont = null;

	void findForms()
    {
        //form instance 
        frmDistSens = FormLogic<frmMain>.CreateAndShow<frmDistSensViewer>(true, false);
        frmDigitalPwr = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
        frmStageCont = FormLogic<frmMain>.CreateAndShow<uiStageControl>(true, false);
        frmStatus = FormLogic<frmMain>.CreateAndShow<frmAlignStatus>(true, true);
		try
		{
			frmSourCont = Application.OpenForms.OfType<frmSourceController>().First();
		}
		catch
		{
			frmSourCont = null;
		}
	}
	

    AlignPosition readCoord(Istage leftStage, Istage rightStage, Istage centerStage)
    {
        var coord = new AlignPosition();
        coord.In = leftStage?.GetAbsPositions();
        coord.Out = rightStage?.GetAbsPositions();
        if(mMoveNextByCenter) coord.Other = centerStage?.GetAbsPositions();

        return coord;
    }

	
    bool doStopChecking()
    {
        if(m_stopFlag) AlignTimer.EndChip();
        return m_stopFlag;
    }

    void showMessage(string msg)
    {
        Invoke((Func<IWin32Window, string, DialogResult>)MessageBox.Show, this, msg);
    }


    /// <summary>
    /// thread function.
    /// </summary>
    async void ThreadFunc()
    {
        #region ---- INIT  ----

        AlignPosition currentPos = null;				
        List<AlignPosition> alignPosList = new List<AlignPosition>();

        JeffTimer jTimer = new JeffTimer();
        
        #endregion

        while (true)
        {
            //신호 대기.
            m_isRuning = false;
            if (m_autoEvent == null) break;
            m_autoEvent.WaitOne();

            #region ---- SETUP : BAR ----

            AlignTimer.StartBar(mParam.chipList[0]);

            findForms();
            m_isRuning = true;
            m_stopFlag = false;

            //저장공간 초기화.
            m_msrList.Clear();

            //process state 초기화.
            m_procState.Clear();
            m_procState.compeleted = false;
            m_procState.totalItemCnt = mParam.chipList.Length;
            m_procState.startTime = DateTime.Now;
            Invoke((Action)ShowProgressWnd);

            //update chip list process state
            Invoke((Action)UpdateChipListProgState);

            //시작 위치 저장.
            alignPosList.Clear();
            if (mParam.alignment)
            {
                currentPos = readCoord(mLeft, mRight, mCenter);
                alignPosList.Add(currentPos);
            }

            //Disable optical source controller
            frmSourCont?.DisableForm();

            //정렬상태 확인
            if (mParam.alignment)
            {
                var power = mSweep.MeasurePower(mParam.PmPorts[0], 1, true);
                if (CGlobal.AlignThresholdPower <= power) XYSearchParam.LastPeakPower = power;
                else
                {
                    if (!mTestMode)
                    {
                        var msg = $"OPM port{mParam.PmPorts[0]} power=<{power}> is lower than threshold <{CGlobal.AlignThresholdPower}>.\n\nContinue?";
                        var dr = MessageBox.Show(msg, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (dr != DialogResult.OK)
                        {
                            Invoke((Action)EnableWnd);
                            frmSourCont?.EnableForm();
                            if (mAutoReturn) AlignTimer.EndBar();
                            continue;//쓰레드 대기 모드로 
                        }
                    }
                    else
                    {
                        AlignTimer.EndBar();
                        continue;//쓰레드 대기 모드로 
                    }
                }
            }
            #endregion


            #region ---- RUN ----
            try
            {
                //alignment + 측정 + 다음칩 이동
                for (int chipIndex = 0; chipIndex < mParam.chipList.Length; chipIndex++)
                {
                    #region ---- SETUP : CHIP ----

                    AlignTimer.StartChip(mParam.chipList[chipIndex]);
                    jTimer.ResetTimer();
                    jTimer.StartTimer();

                    if (doStopChecking()) break;
                    //status
                    m_procState.curItemNo = mParam.chipList[chipIndex];

                    #endregion


                    #region ---- APPROACH ----

                    if (mParam.alignment)
                    {
                        //FA Arrangement.
                        if (mParam.faArrangement && chipIndex == 0) mAlign.FaArrangement(ref m_stopFlag);//edit 161124 DrBae

                        //Approach
                        if (!mParam.faArrangement)
                        {
                            AlignTimer.RecordTime(TimingAction.Approach);
                            writeStatus("Approach In + Out z-stage ");
                            mAlign.ApproachInOut((chipIndex == 0) ? APPROACHBUFFDIST : 0, mApproachDistance);
                            frmDistSens?.StopSensing();
                            if (doStopChecking()) break;
                        }
                    }
                    #endregion


                    #region ---- Alignment ----

                    bool alignSuccess = false;
                    if (mParam.alignment) alignSuccess = await doAlign(chipIndex);
                    else alignSuccess = true;//alignment를 uncheck하면 //algnment는 success된걸로 한다.!!
                    if (doStopChecking()) break;

                    //포지션 획득.
                    if (mParam.alignment)
                    {
                        //Align Power 화면 표시
                        var alignPowerFirst = Math.Round(mSweep.MeasurePower(mParam.PmPorts[0], 1, true), 3);
                        var alignPowerLast = Math.Round(mSweep.MeasurePower(mParam.PmPorts.Last(), 1, true), 3);
                        string alignPowers = $"{alignPowerFirst},  {alignPowerLast}";
                        Invoke((Action)(() => displayNote(chipIndex, alignPowers)));

                        if (alignSuccess)
                        {
                            currentPos = readCoord(mLeft, mRight, mCenter);
                            currentPos.chipIndex = chipIndex;
                            alignPosList.Add(currentPos);

                            //Align Power 화면 표시
                            //var alignPowerFirst = Math.Round(mSweep.MeasurePower(mParam.PmPorts[0], 1, true), 3);
                            //var alignPowerLast = Math.Round(mSweep.MeasurePower(mParam.PmPorts.Last(), 1, true), 3);
                            //string alignPowers = $"{alignPowerFirst},  {alignPowerLast}";
                            //Invoke((Action)(() => displayNote(chipIndex, alignPowers)));
                        }
                        else
                        {
                            //edit 161124 DrBae
                            writeStatus("Alignment failed!!");
                            if (chipIndex != (mParam.chipList.Length - 1))
                            {
                                writeStatus("moving next chip.");
                                MoveNextChip(alignPosList, mParam.chipWidth, chipIndex);

                                AlignTimer.EndChip();
                                continue;
                            }
                            else break;
                        }
                    }
                    

                    #endregion


                    #region ---- Measurement ----

                    Cmeasure meas = new Cmeasure();
                    meas.chipNo = mParam.chipList[chipIndex];
                    meas.msrTime = DateTime.Now;
                    meas.pos = alignPosList.LastOrDefault();
                    m_msrList.Add(meas);
                    if (mParam.measurement) meas.sd = await doMeasure(chipIndex, mParam.PmPorts, mParam.DutChCwl_nm);

                    #endregion


                    #region ---- Monitor Port Power ----

                    //monitor Port 측정
                    var monitorIL = new double[mParam.PmPorts.Length];
					var monitorClading = new double[mParam.PmPorts.Length];
					if (mParam.doMonitor && !CGlobal.IsTlsPmMode) await doMonitor(mParam.MonitorRefPower_dBm, monitorIL, monitorClading);
                    #endregion


                    #region ---- 칩측정 완료 처리 ----

                    AlignTimer.RecordTime(TimingAction.SaveAndPlot);

                    if (mParam.measurement)
                    {
                        //save
                        if (mParam.autoSave == true)
                        {
                            writeStatus("save data.");
                            string filePath = RawTextFile.BuildFileName(mParam.saveFolderPath, meas.chipNo);
                            if (mParam.autoSaveType == AutoSave.Full)
                            {
                                //Full Range Save
								if (mParam.doMonitor && mParam.doMonitorClading) meas.sd.WriteTransmitance(filePath, monitorIL, monitorClading);
								else if (mParam.doMonitor) meas.sd.WriteTransmitance(filePath, monitorIL);
                                else meas.sd.WriteTransmitance(filePath); 
                            }
                            else
                            {
								//Custom Range Save
								if (mParam.doMonitor && mParam.doMonitorClading) meas.sd.WriteTransmitance(filePath, mParam.saveRngStart, mParam.saveRngStop, monitorIL, monitorClading);
								else if (mParam.doMonitor) meas.sd.WriteTransmitance(filePath, mParam.saveRngStart, mParam.saveRngStop, monitorIL);
                                else meas.sd.WriteTransmitance(filePath, mParam.saveRngStart, mParam.saveRngStop);
                            }
                        }

                        writeStatus("plot data.");
                        Invoke((Action)(() => Plot(mParam.chipList[chipIndex])));//plot
                        Invoke((Action)(() => Inspection(mParam.chipList[chipIndex])));//Inspection
                    }

                    //alignment가 uncheck되면 칩 하나만 측정하고 나간다.
                    if (!mParam.alignment && !mTestMode)
                    {
                        AlignTimer.EndChip();
                        break;
                    }

                    //move to next chip
                    if (doStopChecking()) break;
                    if (chipIndex != (mParam.chipList.Length - 1) && !CGlobal.IsTlsPmMode)
                    {
                        writeStatus("move next chip.");
                        MoveNextChip(alignPosList, mParam.chipWidth, chipIndex);
                    }
                   

                    //time 측정 끝!!
                    jTimer.StopTimer();
                    m_procState.SetItemProcTime(jTimer.GetLeadTime().TotalSeconds);

                    //update chip list progres state. 색표시
                    Invoke((Action)UpdateChipListProgState);


                    AlignTimer.EndChip();

                    #endregion

                }// for (int i = 0; i < m_tp.chipNos.Length; i++)
            }
            catch(Exception ex)
            {
                showMessage($"{ex.Message}\r\n\r\n{ex.StackTrace}");
                //continue;
                m_stopFlag = true;
            }
            finally
            {
                if (!mTlsAlignment) CGlobal.osw?.SetToAlign();

                //자동귀환이 아닐 경우 칩바 타이머 종료 - 원점귀환 시간 미포함
                if (!mTestMode && !mAutoReturn) AlignTimer.EndBar();

                frmDigitalPwr?.DisplayOn();
            }
            #endregion


            #region ---- BAR 완료 처리 ----

            try
            {
                if (m_stopFlag == true)
                {
                    //stop stage.
                    mLeft?.StopMove();
                    mRight?.StopMove();
                    m_procState.msg = "Process has stopped!!";
                    m_procState.endTime = DateTime.Now;
                    m_procState.compeleted = true;

                    //testmode
                    mCurrentTest = 0;

                    if (mParam.chipList.Length > 1 && mParam.alignment)
                    {
                        if (mTestMode) MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, false);
                        else if (mAutoReturn) MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, true);
                        else
                        {
                            string msg = "Operation canceled.\n";
                            msg += "Move back to #1 position?";

                            if (DialogResult.Yes == MessageBox.Show(msg, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, true);
                        }
                    }
                }//stop
                else
                {
                    m_procState.msg = "Operation has been completed!!";
                    m_procState.endTime = DateTime.Now;
                    m_procState.compeleted = true;
                    writeStatus("Chip Bar Finished!!");

                    if (mParam.chipList.Length > 1 && mParam.alignment)
                    {
                        if (mTestMode) MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, false);
                        else if (mAutoReturn) MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, true);
                        else
                        {
                            string msg = "Operation complete!\n";
                            msg += "Move back to #1 position?";

                            if (DialogResult.Yes == MessageBox.Show(msg, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                MoveToInitPosition(alignPosList[0].In, alignPosList[0].Out, alignPosList[0].Other, true);
                        }
                    }
                }

                //화면 활성화!!
                Invoke((Action)EnableWnd);                

                //Enable optical source controller
                frmSourCont?.EnableForm();
            }
            finally
            {
                //칩바 측정시간에 원점 복귀 시간 포함
                if (mTestMode || mAutoReturn) AlignTimer.EndBar();
                //Stage Controller - Pos update
                Invoke((Action)(() => frmStageCont?.UpdateAxisPos()));
            }

            #endregion


            #region ---- AUTO TEST MODE ----

            //TESTmode by DrBae 2015-10-02
            if (mTestMode && !m_stopFlag)
            {
                mCurrentTest--;
                Invoke(new Action(() =>
                {
                    testNumberTextBox.Text = mCurrentTest.ToString();
                    testNumberTextBox.Update();
                }));

                if (mCurrentTest > 0)
                {
                    //this.Invoke(new Action(performMeasureClick));
                    Invoke(new Action(async () =>
                    {
                        await Task.Delay(mParam.chipList.Length * 100);
                        btnMeasure.PerformClick();
                    }));
                }
            }//testmode
            #endregion


        }//while (true) - thread loop

    }

    private async Task doMonitor(double[] monitorRef, double[] monitorIL, double[] monitorClading)
    {

        writeStatus("Measuring Monitor Port");

		Action<int> moveCh = (p) => moveMonitorCh(mLeft, mParam.MonitorType, m_MonitorPort1Dist[mParam.MonitorType], p + 1);
		Action<bool> movePortToClading = new Action<bool>(async (f) => await MoveAs(mRight, mRight.AXIS_X, mParam.monitorCladingDist, f));

		var currentPos = readCoord(mLeft, mRight, mCenter);

		//파장설정
        if (mParam.MonitorMcTls)//TLS 이용
        {
            CGlobal.osw?.SetToTls();
            m_mpm.SetGainLevel(CGlobal.PmGain[0]);
		}
        else//ALS 이용
        {
            CGlobal.osw?.SetToAlign();
            m_mpm.SetGainLevel(CGlobal.PmAlignGain);//Align Gain Level 변경
        }
		
		//measure order == pm order == always forward direction		◆
		var power = await mSweep.MeasurePower(mParam.MonitorCwl_nm, mParam.MonitorPmPorts, 10, true, moveCh, movePortToClading, mParam.doMonitorClading);

		//monitorRef order == pm forward direction
		for (int i = 0; i < power[0].Length; i++)
		{
			monitorIL[i] = Math.Round(power[0][i] - monitorRef[i], 3);
			monitorClading[i] = Math.Round(power[1][i] - monitorRef[i], 3);
		}

		if (mParam.chDirect == ChipDirection.Reverse)
		{
			Array.Reverse(monitorIL);
			Array.Reverse(monitorClading);
		}

        Invoke((Action)(() => displayMonitorIL(monitorIL)));

        goBackInputPort(currentPos);
    }

    private async Task<bool> doAlign(int chipIndex)
    {
        //, mParam.PmPorts, mParam.DutChCwl_nm
        writeStatus("Alignment");
        if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
        var alignEvent = mAlign?.IsEventEnable ?? false;
        try
        {
            if(mAlign != null) mAlign.IsEventEnable = false;
            if (mTlsAlignment)
            {
                CGlobal.osw?.SetToTls();
                m_mpm.SetGainLevel(CGlobal.PmGain[0]);

                if (CGlobal.UsingTcpServer)
                {
                    mSweep.TcpServer_Register(true);
                    mSweep.TcpServer_BeginAlign();
                    mSweep.TcpServer_Align(mParam.DutChCwl_nm[0]);
                }
                else m_tls?.SetTlsWavelen(mParam.DutChCwl_nm[0]);
            }
            else
            {
                CGlobal.osw?.SetToAlign();//set align sources
                m_mpm.SetGainLevel(CGlobal.PmAlignGain);//Align Gain Level 변경
            }

            //roll param
            var portDistance = mParam.outPitch * (mParam.PmPorts.Length - 1);
            bool doRoll = mParam.roll;
            if (chipIndex == 0) doRoll = true;
            if (mParam.outFabLoop || mParam.singleSweep) doRoll = false;
            mAlign.RollParam.Wave1 = mParam.DutChCwl_nm.First();
            mAlign.RollParam.Wave2 = mParam.DutChCwl_nm.Last();

            //out stage alignment
            CGlobal.XySearchParamRight.Run = (chipIndex % mRightAlignInterval == 0);

			//run aligning
			var firstPort = mParam.PmPorts.Min();
			var lastPort = mParam.PmPorts.Max();
            return mAlign.AlignDut(firstPort, lastPort,
                                    CGlobal.AlignThresholdPower, doRoll, portDistance,
                                    CGlobal.XySearchParamLeft, CGlobal.XySearchParamRight,
                                    (mTlsAlignment) ? m_tls : null);
        }
        finally
        {
            if (CGlobal.UsingTcpServer && mTlsAlignment) mSweep.TcpServer_Register(false);
            if (mAlign != null) mAlign.IsEventEnable = alignEvent;
        }
    }

    async Task<DutData> doMeasure(int chipIndex, int[] pmPorts, int[] chCenterWaves)
    {
        try
        {
            writeStatus("measurment");
            frmDigitalPwr?.DisplayOff();

            var pmGains = mParam.gainList.ToArray();
            bool doClad = mParam.doClad;
            bool outFabLoopSweep = mParam.outFabLoop;
            ChipDirection direction = mParam.chDirect;
            int pitch = mParam.outPitch;
            bool monitor = mParam.doMonitor;

            AlignTimer.RecordTime(TimingAction.SweepCore);

            //core power
            DutData corePower = new DutData(mWaveStart, mWaveStop, mWaveStep);
			CGlobal.osw?.SetToTls();

			if (outFabLoopSweep)
			{
				var chPower = new List<DutData>();
				bool alignSuccess = true;
				var xySearchLeft = CGlobal.XySearchParamLeft;
				var xySearchRight = CGlobal.XySearchParamRight;
				xySearchLeft.Run = false;
				xySearchRight.Run = true;
				bool roll = false;

				for (int i = 0; i < pmPorts.Length; i++)
				{
					if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

					//각 ch sweep
					chPower.Add(await mSweep.MeasureSpecturmNp_Dut(pmPorts, pmGains, mWaveStart, mWaveStop, mWaveStep));       //Sweep

					if (i < pmPorts.Length - 1)
					{
						//TLS 파장 변환
						m_tls?.SetTlsWavelen(chCenterWaves[i + 1]);
						//Next Channel 이동
						mRight.RelMove(mRight.AXIS_Z, -100);                              //Z축 이동(후진)
						mRight.WaitForIdle(mRight.AXIS_Z);
						mRight.RelMove(mRight.AXIS_X, pitch);                             //X축 이동
						mRight.WaitForIdle(mRight.AXIS_X);
						mAlign.ZappSingleStage(mRight.stageNo);                           //Approach
						if (m_stopFlag == true) throw new ApplicationException();
						mRight.RelMove(mRight.AXIS_Z, -mApproachDistance);                //Approach buffer
																						  //Alignment (right Stage)
						alignSuccess = mAlign.AlignDut(1, 1,
													   CGlobal.AlignThresholdPower,
													   roll, pitch,
													   xySearchLeft, xySearchRight, null);
					}
					else
					{
						//Ch.1 위치로 복귀
						mRight.RelMove(mRight.AXIS_Z, -100);                             //Z축 이동(후진)
						mRight.WaitForIdle(mRight.AXIS_Z);
						mRight.RelMove(mRight.AXIS_X, -1 * pitch * i);                   //X축 이동
						mRight.WaitForIdle(mRight.AXIS_X);
						mAlign.ZappSingleStage(mRight.stageNo);                          //Approach
						mRight.RelMove(mRight.AXIS_Z, -mApproachDistance);               //Approach buffer
					}

				}

				//data sum
				corePower = mSweep.SumDutData(chPower, mWaveStart, mWaveStop, mWaveStep, (int)direction);

			}
			else corePower = await mSweep.MeasureSpecturmNp_Dut(pmPorts, pmGains, mWaveStart, mWaveStop, mWaveStep);//Sweep (normal)

			//clading power
			if (doClad)
			{
				var cladPower = await measureClad(pmPorts, pmGains);
				corePower?.Subtract(cladPower);
			}

			//calc trans
			mSweep.CalcTrans(corePower, m_ref, outFabLoopSweep);

			return corePower;
        }
        finally
        {
            if (mTlsAlignment)
            {
                m_tls?.SetTlsWavelen(chCenterWaves[0]);
            }
            else
            {
                CGlobal.osw?.SetToAlign();
                m_mpm.SetGainLevel(CGlobal.PmAlignGain);                //Align Gain Level 변경
            }
        }
    }

	async Task<DutData> measureClad(int[] pmPorts, int[] pmGains)
	{
		AlignTimer.RecordTime(TimingAction.SweepClad);

		int[] pmGainsClad = new int[1];

		//move X to clad position
		mRight.RelMove(mRight.AXIS_X, -mCladDeltaX);
		mRight.WaitForIdle();

		//set gain level
		if (pmGains.Length >= 2) pmGainsClad[0] = pmGains[1] + 10;//-30; //[dBm]
		else pmGainsClad[0] = pmGains[0];

		var cladPower = await mSweep.MeasureSpecturmNp_Dut(pmPorts, pmGainsClad, mWaveStart, mWaveStop, mWaveStep);

		mRight.RelMove(mRight.AXIS_X, mCladDeltaX);
		mRight.WaitForIdle();

		return cladPower;
	}
	

	private void moveMonitorCh(Istage aligner, monitorType type, int port1Dist, int pmPort)
	{
		const int stageOpenDist = 20;           //[um]
		const int pitch = 250;                  //[um]

		try
		{
			//move monitor Port
			int pitchPort1 = -1 * port1Dist;
			int pitchPort3 = port1Dist * 2 - pitch * 2;

			//stage open.
			aligner.RelMove(aligner.AXIS_Z, -stageOpenDist);
			aligner.WaitForIdle(aligner.AXIS_Z);

			//x축 이동.
			if (pmPort == 1) aligner.RelMove(aligner.AXIS_X, pitchPort1);
			else if (pmPort == 3 && type == monitorType.NCMETRI) aligner.RelMove(aligner.AXIS_X, pitchPort3);
			else aligner.RelMove(aligner.AXIS_X, pitch);
			aligner.WaitForIdle(aligner.AXIS_X);

			//stage close.
			if (pmPort == 1) mAlign?.ZappSingleStage(aligner.stageNo);
			else if (pmPort == 3 && type == monitorType.NCMETRI) mAlign?.ZappSingleStage(aligner.stageNo);
			else aligner.RelMove(aligner.AXIS_Z, stageOpenDist);
			aligner.WaitForIdle(aligner.AXIS_X);

			//Input 정렬
			CGlobal.XySearchParamLeft.Port = pmPort;
			mAlign?.XySearch(CGlobal.XySearchParamLeft);

		}
		catch (Exception)
		{
			//do nothing.
		}

	}

	static async Task MoveAs(Istage alinger, int axis, double displacement_um, bool forward)
	{
		if (!forward) displacement_um *= -1;
		alinger.RelMove(axis, displacement_um);
		for (int i = 0; i < 300; i++)
		{
			if (alinger.IsMovingOK(axis)) await Task.Delay(250);
			else break;
		}
	}



	/// <summary>
	/// monitor port 모두 측정후 Input Port로 Fab 복귀
	/// </summary>
	/// <param name="currentPos"></param>
	private void goBackInputPort(AlignPosition currentPos)
    {
        const int stageOpenDist = 40;           //[um]

        try
        {
            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, -stageOpenDist);

            //move.
            mLeft.AbsMove(mLeft.AXIS_X, currentPos.In.x);
            mLeft.WaitForIdle(mLeft.AXIS_X);
            mLeft.AbsMove(mLeft.AXIS_Y, currentPos.In.y);
            mLeft.WaitForIdle(mLeft.AXIS_Y);
            mLeft.AbsMove(mLeft.AXIS_Z, currentPos.In.z);

            mLeft.WaitForIdle(mLeft.AXIS_Z);
        }
        catch (Exception)
        {
            //do nothing.
        }
    }


    /// <summary>
    /// monitor IL 값 출력
    /// </summary>
    private void displayMonitorIL(double[] _monitorIL)
    {

        string chipNo = "";

        for (int i = 0; i < gridMonitor.RowCount; i++)
        {
            chipNo = gridMonitor.Rows[i].Cells[0].Value.ToString();

            if (chipNo == m_msrList.Last().chipNo)
            {
				for (int j = 0; j < 4; j++) gridMonitor.Rows[i].Cells[j + 1].Value = Math.Round(_monitorIL[j], 3);
                gridMonitor.CurrentCell = gridMonitor.Rows[i].Cells[0];
            }

        }
    }


	#endregion




	#region Private method


	/// <summary>
	/// 진행 상황에 따라 chip List에서 progress 상태를
	/// 업데이트한다.
	/// </summary>
	private void UpdateChipListProgState()
	{

		try
		{

			if (m_isRuning == false)
			{
				//----- 작업 완료 ,  Idle 상태 -----//


				//원래대로 복원 -> 바탕을 하얀색으로 변경.
				DataGridViewCell cell = null;
				for (int i = 0; i < uiGridChips.RowCount; i++)
				{
					for (int j = 0; j < uiGridChips.Rows[i].Cells.Count; j++)
					{
						cell = uiGridChips.Rows[i].Cells[j];
						if (cell.Style.BackColor != Color.White)
							cell.Style.BackColor = Color.White;

					}
				}


			}
			else
			{
				//----- 작업중 -----//

				//측정된 것까지 노란색으로 칠한다.
				string chipNo = "";
				DataGridViewCell cell = null;
				for (int i = 0; i < uiGridChips.RowCount; i++)
				{

					chipNo = uiGridChips.Rows[i].Cells[1].Value.ToString();

					if (null != m_msrList.Find(p => p.chipNo == chipNo))
					{
						for (int j = 0; j < uiGridChips.Rows[i].Cells.Count; j++)
						{
							cell = uiGridChips.Rows[i].Cells[j];
							if (cell.Style.BackColor != Color.Yellow)
								cell.Style.BackColor = Color.Yellow;
						}
					}

				}

			}//else


		}
		catch
		{
			//do nothing.
		}



	}


    
	/// <summary>
	/// process resulst form을 띄운다.!!
	/// </summary>
	private void ShowProgressWnd()
    {
        try
        {
            ProgressForm frm = new ProgressForm();
			frm.MdiParent = this.MdiParent;
            frm.SetProcData(m_procState);
            frm.Show();
        }
        catch
        {
            //do nothing.
        }
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
            if (conf != null)
                conf.Dispose();

            conf = null;
        }

        return ret;
    }

    

    /// <summary>
    /// Disable window.
    /// </summary>
    private void DisableWnd()
    {
        grpOptConf.Enabled = false;
        grpMeasurement.Enabled = false;
    }
    


    /// <summary>
    /// Disable window.
    /// </summary>
    private void EnableWnd()
    {
        grpOptConf.Enabled = true;
        grpMeasurement.Enabled = true;
    }

    

    /// <summary>
    /// 초기 위치로 이동한다.
    /// </summary>
    private void MoveToInitPosition(CStageAbsPos  _posIn, CStageAbsPos  _posOut, CStageAbsPos  _posCenter, bool doOpen = true)
    {
        const int OpenDistanceForReturn = 10000;
        const int BufferDistance = 100;

        if (m_msrList.Count() == 0) return;

        try
        {
            //stage open.
            if (doOpen)
            {
                _posIn.z -= OpenDistanceForReturn;
                _posOut.z -= OpenDistanceForReturn;
            }

            //Z축 이동
            mLeft.AbsMove(mLeft.AXIS_Z, _posIn.z - BufferDistance);
            mRight.AbsMove(mRight.AXIS_Z, _posOut.z - BufferDistance);
            mRight.WaitForIdle(mRight.AXIS_Z);


            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, _posIn.y);
            mRight.AbsMove(mRight.AXIS_Y, _posOut.y);
            mRight.WaitForIdle(mRight.AXIS_X);

            //tz 이동. : MMF 사용 - 불필요
            //m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _posOut.tz);


            //X축 이동
            mLeft.AbsMove(mLeft.AXIS_X, _posIn.x);
            mRight.AbsMove(mRight.AXIS_X, _posOut.x);
            mRight.WaitForIdle(mRight.AXIS_X);

            //Center축 이동
            if (mMoveNextByCenter)
            {
                var pos = CGlobal.CenterAxis == mCenter.AXIS_X ? _posCenter.x : _posCenter.y;
                mCenter.AbsMove(CGlobal.CenterAxis, pos);//center pos. 이동
                mCenter.WaitForIdle(CGlobal.CenterAxis);
            }
        }
        catch
        {
            //do nothing
        }
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

        g.IntervalY = 5;
        g.MinY = -45;

        return g;
    }
    /// <summary>
    /// 칩의 모든 데이터(4channels)를 출력한다
    /// </summary>
    /// <param name="_chipNo">chip no.</param>
    private void Plot(string _chipNo)
    {
        try
        {
            lbChipNo.Text = _chipNo;
            //wfgTrans.ClearData();

            //find chip data.
            Cmeasure meas = m_msrList.Find(p => p.chipNo == _chipNo);
            int startPort = (mParam.fa == FaType.MMF) ? 1 : 5;
            if (mParam.singleSweep && mParam.chDirect == ChipDirection.Reverse) startPort += 3;
            //DataPlot.Plot(this.wfgTrans, meas.sd, InspectionGrid.ShiftPeak, startPort);
            DataPlot.Plot(_wg, meas.sd, InspectionGrid.ShiftPeak, startPort);

            //범례
            var color = DataPlot.mColors;
            if (mParam.chDirect == ChipDirection.Reverse) Array.Reverse(color);

            lblPlotCh1.ForeColor = color[1]; lblPlotCh1.BackColor = SystemColors.ControlDark;
            lblPlotCh2.ForeColor = color[2]; lblPlotCh2.BackColor = SystemColors.ControlDark;
            lblPlotCh3.ForeColor = color[3]; lblPlotCh3.BackColor = SystemColors.ControlDark;
            lblPlotCh4.ForeColor = color[4]; lblPlotCh4.BackColor = SystemColors.ControlDark;

        }
        catch
        {
            //do nothing.
        }
    }



    /// <summary>
    /// Align Power 화면 표시
    /// </summary>
    /// <param name="index"></param>
    /// <param name="AlignPower"></param>
    private void displayNote(int index, string AlignPower)
    {
        uiGridChips.Rows[index].Cells[2].Value = AlignPower;
    }



    /// <summary>
    /// 분석데이터를 출력한다(peak Loss, DWL)
    /// </summary>
    /// <param name="chipNo"></param>
    private void Inspection(string chipNo)
    {
        try
        {
            //find chip data.
            Cmeasure meas = m_msrList.Find(p => p.chipNo == chipNo);
            if (meas == null) return;

            var analyze = WdmAnalyzer.AnalyzeNp(meas.sd);
            uiInspectionPeak.SetValue(analyze);

            var mc = WdmAnalyzer.AnalyzeMcDemux(meas.sd);
            uiInspectionMcIL.SetValue(mc, 0);
            uiInspectionMcAx.SetValue(mc, 1);
        }
        catch 
        {
            //do nothing.
        }
    }
    


    /// <summary>
    /// 다음칩으로 이동한다.
    /// lsm을 이용 1차 함수 parameter를 구하고
    /// 이를 이용하여 다음칩 위치를 추정하고 스테이지를 그 위치로 이동시킨다.
    /// </summary>
    /// <param name="positions">aligned postion array</param>
    /// <param name="chipWdith">chip width</param>
    /// <param name="chipIndex">현재 칩 index</param>
    void MoveNextChip(List<AlignPosition> positions, int chipWdith, int chipIndex)
    {
        AlignTimer.RecordTime(TimingAction.MoveNext);

        const int bufferDistance = 60;

        var distance = AlignLogic.CalcNextChip(positions, chipWdith, chipIndex);//[in~out][y~z]

        //if (mMoveNextByCenter) moveNextChip_Center(bufferDistance, positions, chipWdith, chipIndex);
        //else moveNextChip_LeftRight(bufferDistance, positions, chipWdith, chipIndex);

        try
        {
            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, bufferDistance * (-1));
            mRight.RelMove(mRight.AXIS_Z, bufferDistance * (-1));

            //x next chip
            if (mMoveNextByCenter)
            {
                //var msg = $"DutForm.MoveNextChip():\nCenterDir={CGlobal.CenterDirection}, chipW={chipWdith}";
                //MessageBox.Show(msg);
                mCenter.RelMove(CGlobal.CenterAxis, -CGlobal.CenterDirection * chipWdith);
            }
            else
            {
                mLeft.RelMove(mLeft.AXIS_X, chipWdith);
                mRight.RelMove(mRight.AXIS_X, chipWdith);
            }

            //Y,Z축 보정
            mLeft.RelMove(mLeft.AXIS_Y, distance[0][0]);
            mLeft.RelMove(mLeft.AXIS_Z, distance[0][1]);
            mRight.RelMove(mRight.AXIS_Y, distance[1][0]);
            mRight.RelMove(mRight.AXIS_Z, distance[1][1]);

            //완료 대기.
            if (mMoveNextByCenter) mCenter.WaitForIdle(CGlobal.CenterAxis);
            else mRight.WaitForIdle(mRight.AXIS_X);
        }
        catch (Exception ex)
        {
            Log.Write($"MuxFaForm.MoveNextChip():\n{ex.Message}\n{ex.StackTrace}");
        }
    }



    private void moveNextChip_LeftRight(int bufferDistance, List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {
        try
        {
            //lsm를 이용하여 1차함수 parameter를 구한다.
            //y = ax+b
            //input 쪽 좌표가 기준.
            double a1 = 0.0;//input y축 기울기 
            double a2 = 0.0;//output y축 기울기 
            double b1 = 0.0;//input y축 절편.
            double b2 = 0.0;//output의 y축 절편    

            int posCnt = _posList.Count();
            if (posCnt < 2)
            {
                //--default--//
                a1 = 0.0;
                a2 = 0.0;
                b1 = _posList.Last().In.y;
                b2 = _posList.Last().Out.y;
            }
            else
            {
                //y축
                double[] xPoss = new double[posCnt];
                double[] yPoss = new double[posCnt];
                for (int i = 0; i < _posList.Count(); i++)  //input.
                {
                    xPoss[i] = _posList[i].In.x;
                    yPoss[i] = _posList[i].In.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a1, ref b1);

                for (int i = 0; i < _posList.Count(); i++)  //output.
                {
                    xPoss[i] = _posList[i].Out.x;
                    yPoss[i] = _posList[i].Out.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a2, ref b2);
            }

            //next chip 위치 계산. 
            double nextPosInX = 0.0;
            double nextPosInY = 0.0;
            double nextPosOutX = 0.0;
            double nextPosOutY = 0.0;
            double nextPosInZ = 0.0;
            double nextPosOutZ = 0.0;

            if (posCnt < 2)
            {
                //x
                nextPosInX = (int)(_posList[0].In.x + (_chipWdith * (_curIdx + 1)));
                nextPosOutX = (int)(_posList[0].Out.x + (_chipWdith * (_curIdx + 1)));

                //y
                nextPosInY = (int)(a1 * nextPosInX + b1);
                nextPosOutY = (int)(a2 * nextPosOutX + b2);

                //z
                nextPosInZ = _posList.Last().In.z - bufferDistance;
                nextPosOutZ = _posList.Last().Out.z - bufferDistance;
            }
            else
            {
                int preChipIdx = _posList[posCnt - 2].chipIndex;
                int lastChipIdx = _posList[posCnt - 1].chipIndex;
                int dx = (int)(_posList[posCnt - 1].In.x - _posList[posCnt - 2].In.x);
                dx = (int)(dx / (lastChipIdx - preChipIdx));

                //x
                nextPosInX = _posList[posCnt - 1].In.x + (dx * (_curIdx - lastChipIdx + 1));
                nextPosOutX = (int)(_posList[posCnt - 1].Out.x + (dx * (_curIdx - lastChipIdx + 1)));

                //y
                nextPosInY = (int)(a1 * nextPosInX + b1);
                nextPosOutY = (int)(a2 * nextPosOutX + b2);

                //z (170307 ko)
                double[] diffInZ = new double[posCnt - 1];
                double[] diffOutZ = new double[posCnt - 1];
                int marginInZ, marginOutZ;
                for (int i = 0; i < posCnt - 1; i++)
                {
                    diffInZ[i] = _posList[i].In.z - _posList[i + 1].In.z;
                    diffOutZ[i] = _posList[i].Out.z - _posList[i + 1].Out.z;
                }
                marginInZ = bufferDistance + (int)diffInZ.Average();
                marginOutZ = bufferDistance + (int)diffOutZ.Average();

                nextPosInZ = _posList.Last().In.z - marginInZ;
                nextPosOutZ = _posList.Last().Out.z - marginOutZ;
            }

            //Z축 이동
            mLeft.AbsMove(mLeft.AXIS_Z, nextPosInZ);
            mRight.AbsMove(mRight.AXIS_Z, nextPosOutZ);

            //X축 이동
            mLeft.AbsMove(mLeft.AXIS_X, nextPosInX);
            mRight.AbsMove(mRight.AXIS_X, nextPosOutX);

            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, nextPosInY);
            mRight.AbsMove(mRight.AXIS_Y, nextPosOutY);

            mRight.WaitForIdle(mRight.AXIS_Z);
        }
        catch
        {
            //do nothing
        }
    }
    


    private void moveNextChip_Center(int bufferDistance, List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {
        Func<AlignPosition, double> getCenter = (pos) => CGlobal.CenterAxis == mCenter.AXIS_X ? pos.Other.x : pos.Other.y;

        try
        {
            //lsm를 이용하여 1차함수 parameter를 구한다.
            //y = ax+b
            //input 쪽 좌표가 기준.
            double a1 = 0.0;//input y축 기울기 
            double a2 = 0.0;//output y축 기울기 
            double b1 = 0.0;//input y축 절편.
            double b2 = 0.0;//output의 y축 절편    

            int posCnt = _posList.Count();
            if (posCnt < 2)
            {
                //--default--//
                a1 = 0.0;//in
                a2 = 0.0;//out
                b1 = _posList.Last().In.y;
                b2 = _posList.Last().Out.y;
            }
            else
            {
                //y축
                double[] xPoss = new double[posCnt];
                double[] yPoss = new double[posCnt];
                for (int i = 0; i < _posList.Count(); i++)  //input.
                {
                    xPoss[i] = getCenter(_posList[i]);
                    yPoss[i] = _posList[i].In.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a1, ref b1);

                for (int i = 0; i < _posList.Count(); i++)  //output.
                {
                    xPoss[i] = getCenter(_posList[i]);
                    yPoss[i] = _posList[i].Out.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a2, ref b2);
            }

            //next chip 위치 계산. 
            double nextCenter = 0.0;
            double nextInY = 0.0;
            double nextOutY = 0.0;

            double nextInZ = 0.0;
            double nextOutZ = 0.0;

            if (posCnt < 2)
            {
                //x _ center
                nextCenter = (int)(getCenter(_posList[0]) - (_chipWdith * (_curIdx + 1)));

                //y
                nextInY = (int)(a1 * nextCenter + b1);
                nextOutY = (int)(a2 * nextCenter + b2);

                //z
                nextInZ = _posList.Last().In.z - bufferDistance;
                nextOutZ = _posList.Last().Out.z - bufferDistance;
            }
            else
            {
                int preChipIdx = _posList[posCnt - 2].chipIndex;
                int lastChipIdx = _posList[posCnt - 1].chipIndex;
                int dx = (int)(getCenter(_posList[posCnt - 1]) - getCenter(_posList[posCnt - 2]));
                dx = (int)(dx / (lastChipIdx - preChipIdx));
                dx = Math.Abs(dx);

                //x _ ctr
                nextCenter = getCenter(_posList[posCnt - 1]) - (dx * (_curIdx - lastChipIdx + 1));

                //y
                nextInY = (int)(a1 * nextCenter + b1);
                nextOutY = (int)(a2 * nextCenter + b2);

                //z (170307 ko)
                double[] diffInZ = new double[posCnt - 1];
                double[] diffOutZ = new double[posCnt - 1];
                int marginInZ, marginOutZ;
                for (int i = 0; i < posCnt - 1; i++)
                {
                    diffInZ[i] = _posList[i].In.z - _posList[i + 1].In.z;
                    diffOutZ[i] = _posList[i].Out.z - _posList[i + 1].Out.z;
                }
                marginInZ = bufferDistance + (int)diffInZ.Average();
                marginOutZ = bufferDistance + (int)diffOutZ.Average();

                nextInZ = _posList.Last().In.z - marginInZ;
                nextOutZ = _posList.Last().Out.z - marginOutZ;
            }

            //z축
            mLeft.AbsMove(mLeft.AXIS_Z, nextInZ);
            mRight.AbsMove(mRight.AXIS_Z, nextOutZ);

            //X축 이동 - cneter
            mCenter.AbsMove(CGlobal.CenterAxis, nextCenter);

            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, nextInY);
            mRight.AbsMove(mRight.AXIS_Y, nextOutY);

            //완료 대기.
            mCenter.WaitForIdle(CGlobal.CenterAxis);
        }
        catch
        {
            //do nothing
        }
    }



    /// <summary>
    /// 간단한 정보를 ToolStripLabel에 출력한다.!!
    /// </summary>
    /// <param name="_msg"></param>
    private void writeStatus(string _msg)
    {
        if (InvokeRequired) Invoke((Action)(() =>
        {
            uiStatus.Text = _msg;
            this.Refresh();
        }));
        else
        {
            uiStatus.Text = _msg;
            this.Refresh();
        }
    }



    /// <summary>
    /// Label 객체에 메세지 출력!!
    /// </summary>
    /// <param name="_lb"> label 객체 포인터</param>
    /// <param name="_msg"> 출력할 메세지</param>
    private void SetLabelMsg(Label _lb, string _msg)
    {
        _lb.Text = _msg;
        _lb.Refresh();
    }
    


    private void loadConfig()
    {
        XConfig conf = null;
        try
        {
            string strTemp = "";
            conf = new XConfig(mConfFilePath);
            

            //-------------Part 1
            txtChipWidth.Text = conf.GetValue("CHIPWIDTH");
            txtCorepitch.Text = conf.GetValue("COREPITCH");

            //strTemp = conf.GetValue("FA");                                                              //FA Type (SMF 5~8) or (MMF 1~4)
            //if (Convert.ToInt32(strTemp) == (int)FaType.SMF) rbtnFA_SMF.Checked = true;
            //else                                             rbtnFA_MMF.Checked = true;

            strTemp = conf.GetValue("CHDIRECTION");                                                     //channel direction (Forward or Reverse)
            if (Convert.ToInt32(strTemp) == (int)ChipDirection.Forward) rbtnChDirForward.Checked = true;
            else                                                        rbtnChDirReverse.Checked = true;

            strTemp = conf.GetValue("GAINS");                                                           //Gains
            if (strTemp == "1") rbtnGain1.Checked = true;
            else                rbtnGain2.Checked = true;

            //-------------Part 2
            strTemp = conf.GetValue("FAARRANGEMENT");                                                   //FA Arrangement
            if (strTemp == "1") chkFaArrangement.Checked = true;
            else                chkFaArrangement.Checked = false;

            strTemp = conf.GetValue("MEASUREMENT");                                                     //Measurement
            if (strTemp == "1") chkMeasurement.Checked = true;
            else                chkMeasurement.Checked = false;

            strTemp = conf.GetValue("ALIGNMENT");                                                       //Alignment
            if (strTemp == "1") chkAlignment.Checked = true;
            else                chkAlignment.Checked = false;

            chkClad.Checked = conf.GetValue("DO_CLADDING_MEASURE").Contains("1");                       //Clading mode Measurement

            chkRoll.Checked = conf.GetValue("ROLL").Contains("1");                                      //Roll

            txtApproach.Text = conf.GetValue("APPROACH");                                                        //Approach,
            chkCenterStage.Checked = conf.GetValue("chkCenterStage").Contains("1");                                                    //Approach,

            mRightAlignInterval = conf.GetValue("RIGHT_ALIGN_INTERVAL").To<int>();                      //Output Align #
            txtChipsPerRightAlign.Text = mRightAlignInterval.ToString();

            strTemp = conf.GetValue("AUTO_RETURN");                                                     //Auto Return,
            if (strTemp == "1") chkAutoReturn.Checked = true;
            else                chkAutoReturn.Checked = false;

            mMoveNextByCenter = conf.GetValue("MOVE_NEXT_BY_CENTER").Contains("1");                     //Center Stage
            if (mMoveNextByCenter) chkCenterStage.Checked = true;

            strTemp = conf.GetValue("CH_WAVELIST");                                                     //CH WaveList
            txtWaveList.Text = strTemp;
            
            //-------------Part 3
            strTemp = conf.GetValue("AUTOSAVE");                                                        //auto Save
            if (strTemp == "1") chkAutoSave.Checked = true;
            else                chkAutoSave.Checked = false;

            strTemp = conf.GetValue("AUTOSAVEFULL");                                                    //Save Range Full : Custom
            if (Convert.ToInt32(strTemp) == (int)AutoSave.Full) rbtnAutoSaveFull.Checked = true;
            else                                                rbtnAutoSaveRng.Checked = false;

            strTemp = conf.GetValue("SAVERNGSTART");                                                    //Save Range Custom - Start WL
            txtSaveRangeStart.Text = strTemp;

            strTemp = conf.GetValue("SAVERNGSTOP");                                                     //Save Range Custom - Stop WL
            txtSaveRangeStop.Text = strTemp;

            strTemp = conf.GetValue("SAVEFOLDERPATH");                                                  //Save Folder
            lbSaveFolderPath.Text = strTemp;

            //-------------Part 4        
            strTemp = conf.GetValue("ITEMPROCESS_TIME");                                                //평균 item 처리 
            m_procState.SetAvgProcTime(Convert.ToDouble(strTemp));

            
            strTemp = conf.GetValue("MONITOR_PORT1_NCMMC");                                             //monitor Port Port1 Distance (NCMMC)
            txtMonitor_NCMMC.Text = strTemp;

            strTemp = conf.GetValue("MONITOR_PORT1_NCMETRI");                                           //monitor Port Port1 Distance (NCMETRI)
            txtMonitor_NCMETRI.Text = strTemp;

            
        }
        catch
        {
            MessageBox.Show("설정값을 불러오든데 실패!! \n기본값 사용.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (conf != null)
            {
                conf.Dispose();
                conf = null;
            }
        }
    }


    
    private void saveConfig()
    {
        XConfig conf = null;
        try
        {
            conf = new XConfig(mConfFilePath);

            //-------------Part 1
            conf.SetValue("CHIPWIDTH", txtChipWidth.Text);
            conf.SetValue("COREPITCH", txtCorepitch.Text);
            //conf.SetValue("FA", (rbtnFA_SMF.Checked) ? 
            //              ((int)FaType.SMF).ToString() : ((int)FaType.MMF).ToString());                         //FA Type (SMF 5~8) or (MMF 1~4)
            conf.SetValue("CHDIRECTION", (rbtnChDirForward.Checked)? 
                          ((int)ChipDirection.Forward).ToString() : ((int)ChipDirection.Reverse).ToString());   //Channel Direction (Forward or Reverse)
            conf.SetValue("GAINS", (rbtnGain1.Checked) ? "1": "2");                                             //Gains


            //-------------Part 2
            conf.SetValue("FAARRANGEMENT", (chkFaArrangement.Checked)? "1" : "0");                              //FA Arrangement
            conf.SetValue("MEASUREMENT", (chkMeasurement.Checked) ? "1" : "0");                                 //Measurement
            conf.SetValue("ALIGNMENT", (chkAlignment.Checked)? "1" : "0");                                      //Alignment
            conf.SetValue("DO_CLADDING_MEASURE", (chkClad.Checked) ? "1" : "0");                                //Clading mode Measurement
            conf.SetValue("ROLL", (chkRoll.Checked) ? "1" : "0");                                               //Roll
            conf.SetValue("APPROACH", txtApproach.Text);//Approach
            conf.SetValue("chkCenterStage", chkCenterStage.Checked ? "1" : "0");//chkCenterStage
            conf.SetValue("RIGHT_ALIGN_INTERVAL", txtChipsPerRightAlign.Text);                                  //Output Align #
            conf.SetValue("AUTO_RETURN", (chkAutoReturn.Checked) ? "1" : "0");                                  //Auto Return
            conf.SetValue("MOVE_NEXT_BY_CENTER", (chkCenterStage.Checked) ? "1" : "0");                         //Center Stage
            conf.SetValue("CH_WAVELIST", txtWaveList.Text);                                                     //CH WaveList


            //-------------Part 3
            conf.SetValue("AUTOSAVE", (chkAutoSave.Checked) ? "1" : "0");                                       //Auto Save
            conf.SetValue("AUTOSAVEFULL", (rbtnAutoSaveFull.Checked) ? "1" : "0");                              //Save Range Full : Custom
            conf.SetValue("SAVERNGSTART", txtSaveRangeStart.Text);                                              //Save Range Custom - Start WL
            conf.SetValue("SAVERNGSTOP", txtSaveRangeStop.Text);                                                //Save Range Custom - Stop WL
            conf.SetValue("SAVEFOLDERPATH", lbSaveFolderPath.Text);                                             //Save Folder


            //-------------Part 4     
            conf.SetValue("ITEMPROCESS_TIME", m_procState.GetAvgProcTime().ToString());                         //평균 item 처리 
            conf.SetValue("MONITOR_PORT1_NCMMC", txtMonitor_NCMMC.Text);                                        //monitor Port Port1 Distance (NCMMC)
            conf.SetValue("MONITOR_PORT1_NCMETRI", txtMonitor_NCMETRI.Text);                                    //monitor Port Port1 Distance (NCMETRI)

        }
        catch
        {
            MessageBox.Show("설정값을 저장하는데 실패!!", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (conf != null)
            {
                conf.Dispose();
                conf = null;
            }
        }
    }



    bool checkFolder()
    {
        bool mWaferFolder = chkWaferFolder.Checked;

        var path = lbSaveFolderPath.Text.Trim();
        if (!Directory.Exists(lbSaveFolderPath.Text.Trim()))
        {
            MessageBox.Show($"저장 폴더 <{path}>가 없습니다.");
            return false;
        }

        mParam.saveFolderPath = path;
        if (mWaferFolder)
        {
            var folder = path;
            try
            {
                var wafer = txtFisrtChipNo.Text.Split('-')[0];
                folder = Path.Combine(path, wafer);
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 폴더 <{folder}>를 만드는 중 에러가 발생했습니다.\n{ex.Message}");
                return false;
            }
            mParam.saveFolderPath = folder;
        }
        return true;
    }


    #endregion



    #region Constructor / Destructor


    public MuxFaForm()
    {
        InitializeComponent();

        this.chkLoopSweep.Visible = AppLogic.License.ShowDevUI;
        this.chkSingleSweep.Visible = AppLogic.License.ShowDevUI;
        this.chkTLSAlignment.Visible = AppLogic.License.ShowDevUI;
        this.label1.Visible = AppLogic.License.ShowDevUI;
        this.txtWaveList.Visible = AppLogic.License.ShowDevUI;

        initGraph(_wg);

    }


    string mConfFilePath = Application.StartupPath + @"\config\conf_cwdmMuxFa.xml";
    string mRefFilePath = Application.StartupPath + @"\refNonpol.txt";


    bool _testing = true;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);


        m_tls = CGlobal.Tls;
        m_mpm = CGlobal.Opm;
        mTlsAlignment = false;
        mLeft = CGlobal.LeftAligner;
        mRight = CGlobal.RightAligner;
        mCenter = CGlobal.OtherAligner;
        mSweep = CGlobal.sweepLogic;
        mAlign = CGlobal.alignLogic;
		if (mAlign != null) mSweep.mReporter += writeStatus;
		if (mAlign != null) mAlign.mReporter += writeStatus;

		m_msrList = new List<Cmeasure>();
        m_procState = new CprogRes();

        //ref.
        m_ref = new ReferenceDataNp();
        if (!m_ref.LoadReferenceFromTxt(mRefFilePath))
        {
            MessageBox.Show("레퍼런스 값을 불러오는데 실패!!", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            m_ref = null;
        }
		
        //option & configs.
        this.Location = LoadWndStartPos(mConfFilePath);
        loadConfig();
		
        //Local TLS 측정시 TLS 설정
        if (!CGlobal.UsingTcpServer)
        {
            m_tls?.SetTlsOutPwr(mTlsPower);
            Thread.Sleep(100);
            m_tls?.SetTlsSweepSpeed(mSweepSpeed); 
        }

        initInspection();
		
        //쓰레드 가동.
        m_autoEvent = new AutoResetEvent(false);
        m_thread = new Thread(ThreadFunc);
        m_thread.IsBackground = true;
        m_thread.Name = "DUT";
        m_thread.Start();
		
    }

    
    private void initInspection()
    {
        uiInspectionPeak.InitColumnRow();

        var conf = new XConfig(mConfFilePath);
        var showDwl = conf.GetValue("INSPECTIN_SHOW_DWL", "0").Contains("1");
        var item = showDwl ? new string[] { "Peak", "DWL" } : new string[] { "Peak"};
        uiInspectionPeak.AddRow(item);

        var range = conf.GetValue("INSPECTION_IL_RANGE", "-10.8;0.6").Unpack<double>().ToArray();
        txtPassRangeIlMin.Value = (decimal)range[0];
        txtPassRangeIlUnif.Value = (decimal)range[1];
        uiInspectionPeak.SetPassRange(item[0], range, false);
        uiInspectionPeak.SetPassTest(item[0], (r, v) => r < v);//IL test

        if (showDwl)
        {
            range = conf.GetValue("INSPECTION_DWL_RANGE", "0.5;0.5").Unpack<double>().ToArray();
            uiInspectionPeak.SetPassRange(item[1], range, true);
            uiInspectionPeak.SetPassTest(item[1], (r, v) => r > Math.Abs(v));//DWL test
        }

        //McDemux Inspection - 170503
        initInspectionMc();
    }
    string[] mRowIl = { "McIL" };
    string[] mColIl = { "IL1", "IL2", "IL3", "IL4" };
    string[] mRowAx = { "McAx" };
    string[] mColAx = { "Ax1", "Ax2", "Ax3", "Ax4", "Ax5", "Ax6" };
    private void initInspectionMc()
    {
        uiInspectionMcIL.InitColumnRow();
        uiInspectionMcIL.AddCol(mColIl);
        uiInspectionMcIL.AddRow(mRowIl);
        uiInspectionMcIL.SetPassRange(mRowIl[0], new double[] { -3.7, 0.5 }, false);
        uiInspectionMcIL.SetPassTest(mRowIl[0], (r, v) => r < v);

        uiInspectionMcAx.InitColumnRow();
        uiInspectionMcAx.AddCol(mColAx);
        uiInspectionMcAx.AddRow(mRowAx);
        uiInspectionMcAx.SetPassRange(mRowAx[0], new double[] { 20.0, 3.0 }, false);
        uiInspectionMcAx.SetPassTest(mRowAx[0], (r, v) => r > v);
    }
	

    private void btnPassRangeApply_Click(object sender, EventArgs e)
    {
        var item = uiInspectionPeak.InspectionItem;
        double[] passRange = new double[]
        {
            (double)txtPassRangeIlMin.Value, (double)txtPassRangeIlUnif.Value
        };
        uiInspectionPeak.SetPassRange(item[0], passRange, false);
        uiInspectionPeak.Refresh();

        uiInspectionMcIL.SetPassRange(uiInspectionMcIL.InspectionItem[0], new double[] { (double)uiMcIL.Value, 0.6}, false);
        uiInspectionMcIL.Refresh();
        uiInspectionMcAx.SetPassRange(uiInspectionMcAx.InspectionItem[0], new double[] { (double)uiMcAx.Value, 3.0 }, false);
        uiInspectionMcAx.Refresh();

        try
        {
            var conf = new XConfig(mConfFilePath);
            conf.SetValue("INSPECTION_IL_RANGE", $"{passRange[0]};{passRange[1]}");
            conf.Save();
            conf.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }
    

    private void frmCwdmMuxFa_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (mSweep != null) mSweep.mReporter -= writeStatus;
        if (mAlign != null) mAlign.mReporter -= writeStatus;


        //save options and options.
        SaveWndStartPos(mConfFilePath);

        saveConfig();
        

        //thread 종료 및 마무리.
        if (m_thread != null)
        {
            //m_thread.Abort();
            //m_thread.Join();
            m_thread = null;
        }


        if (m_autoEvent != null) m_autoEvent.Dispose();
        m_autoEvent = null;

    }


    #endregion



    #region UI Event


    /// <summary>
    /// 측정 시작!!
    /// </summary>
    private void btnMeasure_Click(object sender, EventArgs e)
    {
        //측정진행 확인
        if (m_isRuning == true) return;

        //측정할 칩 갯수 확인
        int chipCnt = uiGridChips.RowCount;
        if (chipCnt < 1)
        {
            MessageBox.Show("측정할 칩 개수가 0개 입니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        //측정옵션 중복 확인
        if (chkLoopSweep.Checked && chkSingleSweep.Checked)
        {
            MessageBox.Show("Sweep 옵션이 중복 설정되었습니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
		
        //TEST mode 2015-10-02
        mTestMode = this.checkTestMode.Checked;
        mCurrentTest = int.Parse(this.testNumberTextBox.Text);
        this.testNumberTextBox.Tag = mCurrentTest;
        
        if (!checkFolder()) return;

        try
        {
            DisableWnd();

            mTlsAlignment = chkTLSAlignment.Checked;										//Alignment 방식 (TLS or Align Source)

            mParam.outFabLoop = chkLoopSweep.Checked;										//Output FAB Loop Sweep
            mParam.singleSweep = chkSingleSweep.Checked;									//1x1 도파로 Sweep

            //Gain level
            mParam.gainList = null;
            mParam.gainList = new List<int>();
            if (rbtnGain1.Checked)
            {
                mParam.numGains = 1;
                mParam.gainList.Add(mGain1);
            }
            else
            {
                mParam.numGains = 2;
                mParam.gainList.Add(mGain1);
                mParam.gainList.Add(mGain2);
            }

            //측정 옵션
            mParam.chipWidth = Convert.ToInt32(txtChipWidth.Text);							//Chip 간격
            mParam.outPitch = Convert.ToInt32(txtCorepitch.Text);							//Out channel Coreptich.

            mParam.fa = FaType.MMF;// (rbtnFA_SMF.Checked) ? FaType.SMF : FaType.MMF;						//SMF(5~8) MMF(1~4)
            mParam.chDirect = (rbtnChDirForward.Checked) ? ChipDirection.Forward : ChipDirection.Reverse;
            var pmPorts = mSweep.PmChMapping((int)mParam.fa, (int)mParam.chDirect);
            mParam.PmPorts = mParam.singleSweep ? new int[] { pmPorts[0] } : pmPorts;		// 1x1 Single Sweep

            mParam.DutChCwl_nm = txtWaveList.Text.Unpack<int>().ToArray();					//TLS Align 파장 설정
            if (mParam.DutChCwl_nm.Length == 0) mParam.DutChCwl_nm = new int[] { 1271, 1291, 1311, 1331 };

			mParam.MonitorCwl_nm = new int[mParam.DutChCwl_nm.Length];
			mParam.DutChCwl_nm.CopyTo(mParam.MonitorCwl_nm, 0);
			mParam.MonitorPmPorts = mSweep.MonitorMapping((int)mParam.fa, (int)mParam.chDirect, in mParam.MonitorCwl_nm);

            mParam.doClad = chkClad.Checked;                                                //Clading mode 측정
			mCladDeltaX = textCladDeltaX.Text.To<int>();									//Clad Delta X값 설정
			mRightAlignInterval = txtChipsPerRightAlign.Text.To<int>();						//Output FAB Align간격
            mAutoReturn = this.chkAutoReturn.Checked;										//자동귀환
            mApproachDistance = txtApproach.Text.To<int>();									//Approach후 거리 설정

            //Save
            mParam.autoSave = chkAutoSave.Checked;
            mParam.autoSaveType = (rbtnAutoSaveFull.Checked) ? AutoSave.Full : AutoSave.Range;
            mParam.saveRngStart = Convert.ToInt32(txtSaveRangeStart.Text);
            mParam.saveRngStop = Convert.ToInt32(txtSaveRangeStop.Text);

            //Fa Arrangement, Alignment , Measurment
            mParam.faArrangement = chkFaArrangement.Checked;
            mParam.alignment = chkAlignment.Checked;
            if (mLeft == null || mRight == null) mParam.alignment = false;
            mParam.measurement = chkMeasurement.Checked;
            mParam.roll = chkRoll.Checked;
            
            //Center Stage
            mMoveNextByCenter = chkCenterStage.Checked;

            //monitor Port
            mParam.doMonitor = chkMonitor.Checked;
            mParam.MonitorMcTls = uiMcMonitor.Checked;
			mParam.doMonitorClading = chkMonitorClading.Checked;
			mParam.monitorCladingDist = txtMonitor_Clading.Text.To<double>();
            m_MonitorPort1Dist = new Dictionary<monitorType, int>();
            m_MonitorPort1Dist.Add(monitorType.NCMMC, txtMonitor_NCMMC.Text.To<int>());
            m_MonitorPort1Dist.Add(monitorType.NCMETRI, txtMonitor_NCMETRI.Text.To<int>());
            mParam.MonitorType = (radioNCMMC.Checked) ? monitorType.NCMMC : monitorType.NCMETRI;
            mParam.MonitorRefPower_dBm = m_ref.MonitorPower();

            //Chip Numbers
            mParam.chipList = new string[uiGridChips.RowCount];
            for (int i = 0; i < uiGridChips.RowCount; i++) mParam.chipList[i] = (string)(uiGridChips.Rows[i].Cells[1].Value);

			//monitor 측정창 활성화 확인
			if (chkMonitor.Checked && gridMonitor.Rows.Count == 0)
			{
				initMonitorPortGrid(mParam.chipList);
				gridMonitor.Refresh();
			}

			//측정 시작
			m_autoEvent.Set();
            Thread.Sleep(100);

        }
        catch (Exception ex)
        {
            EnableWnd();
            MessageBox.Show(ex.Message);
        }

    }



    /// <summary>
    /// save folder path를 선택한다.
    /// </summary>
    private void btnSaveFolder_Click(object sender, EventArgs e)
    {
        var fd = new FolderBrowserDialog();
        if (fd.ShowDialog() == DialogResult.OK) lbSaveFolderPath.Text = fd.SelectedPath;
    }



    /// <summary>
    /// chip no. array를 만들어 chip list에 추가!!
    /// </summary>
    private void btnOK_Click(object sender, EventArgs e)
    {

        //check chip no.
        if (txtFisrtChipNo.Text == "")
        {
            MessageBox.Show("칩넘버를 입력하세요", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        //check chip count.
        try
        {
            if (Convert.ToInt32(txtChipCnt.Text) < 1)
            {
                MessageBox.Show("칩 개수는 최소한 1개 이상!", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        catch
        {
            MessageBox.Show("칩 개수를 정확히 입력해주세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }


        //칩 넘버 array를 만든다.!!
        string[] strTempArr = null;
        string strWfNo = "";    //wafer no.
        string strDate = "";    //date
        int startChipNo = 0;    //start chip no.
        strTempArr = txtFisrtChipNo.Text.Split('-');
        if (strTempArr.Length < 5)
        {
            MessageBox.Show("입력한 칩넘버 이상.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        for (int i = 0; i < strTempArr.Length - 2; i++) strWfNo += strTempArr[i] + "-";

        strWfNo += strTempArr[strTempArr.Length - 2].Substring(0, 1);
        startChipNo = Convert.ToInt32(strTempArr[strTempArr.Length - 2].Substring(1));
        strDate = strTempArr[strTempArr.Length - 1];

        string[] strChipNos = new string[Convert.ToInt32(txtChipCnt.Text)];
        for (int i = 0; i < Convert.ToInt32(txtChipCnt.Text); i++)
        {
            strChipNos[i] = strWfNo;
            strChipNos[i] += String.Format("{0:D2}", (startChipNo + i));
            strChipNos[i] += "-" + strDate;
            strChipNos[i] = strChipNos[i].ToUpper();
        }

        
        //grid setting.
        string[] strColumArr = " No | Chip No. | note ".Split('|');
        string[] strValueArr = new string[strColumArr.Length];
        uiGridChips.HanDefaultSetting();
        uiGridChips.DeleteAllRows();
        uiGridChips.MultiSelect = false;
        uiGridChips.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        uiGridChips.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        uiGridChips.ReadOnly = true;
        uiGridChips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        uiGridChips.AllowUserToOrderColumns = false;
        uiGridChips.AllowUserToResizeRows = false;
        uiGridChips.SetColumns(ref strColumArr);

        
        //그리드에 칩넘버 입력!!
        for (int i = 0; i < strChipNos.Length; i++)
        {
            strValueArr[0] = Convert.ToString(i + 1);         //no.
            strValueArr[1] = strChipNos[i];                   //chip number.
            strValueArr[2] = "";
            uiGridChips.Insert(ref strColumArr, ref strValueArr);
        }
        uiGridChips.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        //monitor Port 측정 그리드 초기화
        initMonitorPortGrid(strChipNos);

    }



    /// <summary>
    /// monitor Port 측정 그리드 초기화
    /// </summary>
    /// <param name="strChipNos"></param>
    private void initMonitorPortGrid(string[] strChipNos)
    {
        if (chkMonitor.Checked)
        {
            string[] strColumArr = "  chip no. |  Ch.1  |  Ch.2  |  Ch.3  |  Ch.4  ".Split('|');
            if (rbtnChDirReverse.Checked) strColumArr = "  chip no. |  Ch.4  |  Ch.3  |  Ch.2  |  Ch.1  ".Split('|');
            string[] strValueArr1 = new string[strColumArr.Length];
            gridMonitor.HanDefaultSetting();
            gridMonitor.DeleteAllRows();
            gridMonitor.Font = new System.Drawing.Font("Source Code Pro", 7, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            gridMonitor.MultiSelect = false;
            gridMonitor.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridMonitor.ReadOnly = true;
            gridMonitor.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            gridMonitor.AllowUserToOrderColumns = false;
            gridMonitor.AllowUserToResizeRows = false;
            gridMonitor.SetColumns(ref strColumArr);

            //그리드에 칩넘버 입력!!
            for (int i = 0; i < strChipNos.Length; i++)
            {
                strValueArr1[0] = strChipNos[i];       //chip number.
                strValueArr1[1] = "";
                strValueArr1[2] = "";
                strValueArr1[3] = "";
                strValueArr1[4] = "";
                gridMonitor.Insert(ref strColumArr, ref strValueArr1);
            }
            gridMonitor.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            gridMonitor.CurrentCell = gridMonitor.Rows[0].Cells[0];
        }
    }



    /// <summary>
    /// chip no.를 모두 삭제한다.
    /// </summary>
    private void btnDelAllChipNos_Click(object sender, EventArgs e)
    {
        uiGridChips.DeleteAllRows();
        gridMonitor.DeleteAllRows();
    }



    /// <summary>
    /// Stop...
    /// </summary>
    private void btnStop_Click(object sender, EventArgs e)
    {
        if (!m_isRuning) return;

        //confirm?
        DialogResult dialRes;
        dialRes = MessageBox.Show("작업이 진행중입니다. 중지하시겠습니까?",
                                  "확인",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question);
        if (dialRes == DialogResult.No) return;

        //stop
        m_stopFlag = true;
        if(mAlign != null) mAlign.mStopFlag = true;


        //frmAlignment
        if (Application.OpenForms.OfType<AlignForm>().Count() > 0)
        {
            AlignForm frm = Application.OpenForms.OfType<AlignForm>().FirstOrDefault();
            frm?.StopOperation();
        }

        if (Application.OpenForms.OfType<frmAlignStatus>().Count() > 0)
        {
            var frm = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();
            frm?.Stop();
        }

    }



    /// <summary>
    /// 칩 데이터 출력!!
    /// </summary>
    private void hdgvChipNos_CellClick(object sender, DataGridViewCellEventArgs e)
    {

        string chipNo = (string)(uiGridChips.SelectedRows[0].Cells[1].Value);
        Plot(chipNo);
        Inspection(chipNo);

    }



    /// <summary>
    /// RAW Data 파일을 불러와서 그래프에 출력
    /// </summary>
    private void btnReadRawData_Click(object sender, EventArgs e)
    {
        try
        {
            var fd = _ofd;
            //fd.InitialDirectory = Program.CD;
            //var fd = new OpenFileDialog();
            //fd.Filter = "Raw Text Data (*.txt)|*.txt";
            var ok = fd.ShowDialog();
            if (ok == DialogResult.OK)
            {
                var dut = DutData.LoadFileNp(fd.FileName);
                var peak = WdmAnalyzer.AnalyzeNp(dut);
                uiInspectionPeak.SetValue(peak);

                var mc = WdmAnalyzer.AnalyzeMcDemux(dut);
                uiInspectionMcIL.SetValue(mc, 0);
                uiInspectionMcAx.SetValue(mc, 1);

                //wfgTrans.ClearData();
                //DataPlot.Plot(wfgTrans, dut, InspectionGrid.ShiftPeak);
                DataPlot.Plot(_wg, dut, InspectionGrid.ShiftPeak);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"menuGrid_ReadRawData_Click():\n{ex.Message}\n{ex.StackTrace}");
        }

    }



    /// <summary>
    /// Chip List 선택(그래프 출력)
    /// </summary>
    private void hdgvChipNos_SelectionChanged(object sender, EventArgs e)
    {
        if (uiGridChips.CurrentRow == null) return;
        if (uiGridChips.CurrentRow.Index < 0) return;
        if (uiGridChips.SelectedRows == null) return;
        if (uiGridChips.SelectedRows.Count < 1) return;

        string chipNo = (string)(uiGridChips.SelectedRows[0].Cells[1].Value);
        Plot(chipNo);
        Inspection(chipNo);
    }



    private void chkAlignment_CheckedChanged(object sender, EventArgs e)
    {
        if (!chkAlignment.Checked) chkRoll.Checked = false;
    }



    private void chkTLSAlignment_CheckedChanged(object sender, EventArgs e)
    {
        if (chkTLSAlignment.Checked && CGlobal.UsingTcpServer)
        {
            MessageBox.Show("TLS Server Mode 상태에서는 사용하실 수 없습니다.", "Warning", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            chkTLSAlignment.Checked = false;
        }

        if (chkTLSAlignment.Checked) txtWaveList.Enabled = true;
        else
        {
            txtWaveList.Enabled = false;
            chkLoopSweep.Checked = false;
        }

    }



    private void chkLoopSweep_CheckedChanged(object sender, EventArgs e)
    {
        if (chkLoopSweep.Checked && CGlobal.UsingTcpServer)
        {
            MessageBox.Show("TLS Server Mode 상태에서는 사용하실 수 없습니다.", "Warning",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
            chkLoopSweep.Checked = false;
        }
        else if (chkLoopSweep.Checked) chkTLSAlignment.Checked = true;

    }



    #endregion

    private void chkMeasurement_CheckedChanged(object sender, EventArgs e)
    {

    }
}
