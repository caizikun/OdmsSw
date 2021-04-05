using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Jeffsoft;
using Free302.TnMLibrary.DataAnalysis;
using Free302.MyLibrary.Utility;
using Neon.Aligner;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;

public partial class MuxBudForm : Form
{


    #region definition

	private const int STGPOSXYZRES = 2;                     //XYZ position Stage resolution.
	private const int STGPOSUVWRES = 4;                     //UVW position Stage resolution.

    private int mTlsPower = CGlobal.TlsParam.Power;         //[dBm]
    private int mWaveStart = CGlobal.TlsParam.WaveStart;    //start sweep wavelength [nm]
    private int mWaveStop = CGlobal.TlsParam.WaveStop;      //stop sweep wavelength [nm]
    private double mWaveStep = CGlobal.TlsParam.WaveStep;   //step sweep wavelength [nm]
	private int mSweepSpeed = CGlobal.TlsParam.Speed;
    int mGain1 = CGlobal.PmGain[0];                         //[dBm]
    int mGain2 = CGlobal.PmGain[1];                         //[dBm]

    private enum WAVELEN { CH1 = 1271, CH2 = 1291, CH3 = 1311, CH4 = 1331 }

    private enum FaType { SMF, MMF }
    private enum ChipDirection { Forward, Reverse }

    private enum AutoSave { Full, Range }
    private enum CMD { RUN, OpenStages }

    
    //private const int CMD_RUN = 0;
    //private const int CMD_OPENSTAGES = 1;

    #endregion



    
    #region structure/innor class


    private struct threadParam
    {
        public CMD cmd;
        public string[] chipList;
        public int numGains;					            //number of gains. 
        public List<int> gainList;				            //
        public int chipWidth;					            //칩 간 간격
        public int outPitch;					            //output FA corepitch [um]     
        public FaType fa;						            //SMF or MMF
        public ChipDirection chDirect;					    //channel direction

        public bool autoSave;
        public AutoSave autoSaveType;				        //full or range.
        public int saveRngStart;				            //save range start wavelengh.
        public int saveRngStop;					            //save range stop wavelengh.

        public bool alignment;					            //alignment. <-- uncheck하면 1칩만 측정됨.
        public bool measurement;				            //measurement.?
        public bool faArrangement;				            //FA arrangement?

        public bool AutoReturn;					            //자동으로 First chip으로 이동한다.

        public string saveFolderPath;
    }

    

    private struct closedPos
    {
        public double outPos;					            //output z축 위치.
    }


    
    private class Cmeasure
    {
        public string chipNo;
        public DateTime msrTime;				            //측정 시간.
        public DutData sd;
    }

    
    #endregion



	
    #region member variables


    private ReferenceDataNp m_ref;

    private IDispSensor m_distSens;
    private IoptMultimeter mPm;
    private Istage mRight;
    private Istage mLeft;
    private Istage mCenter;					                //center stage.

	private SweepLogic mSweep;
    private AlignLogic mAlign;

    bool m_stopFlag;
    bool m_isRuning;							            //running:true , stop :false
    private threadParam m_tp;
    private AutoResetEvent m_autoEvent;
    private Thread m_thread;

    private CprogRes m_procState;
    private List<Cmeasure> m_msrList;

    private enum moveStage {Center, Right }
    private moveStage m_MoveNextChip;


    #endregion




    #region Thread function


    /// <summary>
    /// thread function.
    /// </summary>
    private void ThreadFunc()
    {

        Action ew = EnableWnd;

        while (true)
        {

            //신호 대기.
            m_isRuning = false;
            m_autoEvent.WaitOne();
            m_isRuning = true;
            m_stopFlag = false;

            //do work!!
            switch (m_tp.cmd)
            {
                case CMD.RUN:
                    Run();
                    break;

                case CMD.OpenStages:
                    OpenStages();
                    break;

            }

            //화면 활성화!!
            Invoke(ew);

        }//while (true)

    }


    #endregion




    #region Private method


    frmDistSensViewer frmDistSens = null;
    OpmDisplayForm frmDigitalPwr = null;
	uiStageControl frmStageCont = null;
    frmAlignStatus frmStatus = null;
    frmSourceController frmSourCont = null;

    void findForms()
    {
        //form instance 
        frmDistSens = Application.OpenForms.OfType<frmDistSensViewer>().FirstOrDefault();
        frmDigitalPwr = Application.OpenForms.OfType<OpmDisplayForm>().FirstOrDefault();
        frmStageCont = Application.OpenForms.OfType<uiStageControl>().FirstOrDefault();
        frmStatus = Application.OpenForms.OfType<frmAlignStatus>().FirstOrDefault();
        frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();
    }


    static int[] setupWave(ChipDirection chDirection)
    {
        var waves = new int[4];
        if (chDirection == ChipDirection.Forward)
        {
            //-- 정방향 --//
            waves[0] = (int)WAVELEN.CH1;
            waves[1] = (int)WAVELEN.CH2;
            waves[2] = (int)WAVELEN.CH3;
            waves[3] = (int)WAVELEN.CH4;
        }
        else
        {
            //-- 역방향 --//
            waves[0] = (int)WAVELEN.CH4;
            waves[1] = (int)WAVELEN.CH3;
            waves[2] = (int)WAVELEN.CH2;
            waves[3] = (int)WAVELEN.CH1;
        }
        return waves;
    }


    static int[] setupPorts(int faDirection)
    {
        //FA 종류에 따른 Detector port 및 wavelength 설정 
        int[] ports = new int[4];
        if (faDirection == (int)FaType.SMF)
        {
            //--- SMF ---//
            //detector 5~8번 사용.
            ports[0] = 5;
            ports[1] = 6;
            ports[2] = 7;
            ports[3] = 8;
        }
        else
        {
            //--- MMF ---//
            //detector 1~4번 사용.
            ports[0] = 1;
            ports[1] = 2;
            ports[2] = 3;
            ports[3] = 4;
        }

        return ports;
    }


    AlignPosition readCoord(Istage leftStage, Istage rightStage, Istage centerStage)
    {
        var coord = new AlignPosition();
        coord.In = leftStage?.GetAbsPositions();
        coord.Out = rightStage?.GetAbsPositions();
        coord.Other = centerStage?.GetAbsPositions();

        return coord;
    }



    /// <summary>
    /// 작업을 실행한다.
    /// </summary>
    private async void Run()
    {
        const int APPROACHBUFFDIST = 40;            //[um]    
        const int CHIP2FADIST = 10;                 //[um]

        Action<Label, string> slm = SetLabelMsg;
        Action<string> dsi = DisplayShortInfor;
        Action<string> pca = Plot;
        Action spw = ShowProgressWnd;
        Action uclps = UpdateChipListProgState;
        
        CStageAbsPos retPosIn = null;               //돌아갈 left   stage position
        CStageAbsPos retPosOut = null;              //돌아갈 right  stage position
        CStageAbsPos retPosCtr = null;              //돌아갈 center stage position
        AlignPosition currentPos = null;

        List<AlignPosition> alignPosList = new List<AlignPosition>();
        JeffTimer jTimer = new JeffTimer();

        AlignTimer.StartBar(m_tp.chipList[0]);

        //find Tools Form 
        findForms();

        //저장공간 초기화.
        m_msrList.Clear();

        //process state 초기화.
        m_procState.Clear();
        m_procState.compeleted = false;
        m_procState.totalItemCnt = m_tp.chipList.Length;
        m_procState.startTime = DateTime.Now;
        Invoke(spw);

        //update chip list process state
        Invoke(uclps);


        //chip 방향(칩의채널 방향)에 따른 파장 설정.
        int[] chCenterWaves = setupWave(m_tp.chDirect);


        //FA 종류에 따른 Detector port 및 wavelength 설정 
        //int[] pmPorts = setupPorts(m_tp.fa);
        var pmPorts = mSweep.PmChMapping((int)m_tp.fa, (int)m_tp.chDirect);

        //시작 위치 저장.
        alignPosList.Clear();
        if (m_tp.alignment)
        {
            retPosIn = mLeft?.GetAbsPositions();
            retPosOut = mRight?.GetAbsPositions();
            retPosCtr = mCenter?.GetAbsPositions(); 
        }

        
        //Disable optical source controller
        if (frmSourCont != null) frmSourCont.DisableForm();
                

        //alignment + 측정 + 다음칩 이동
        string chipNo = "";
        for (int chipIndex = 0; chipIndex < m_tp.chipList.Length; chipIndex++)
        {

            //FA Arrangement.
            if ((chipIndex == 0) && (m_tp.faArrangement == true)) FaArrangement();


            //칩측정 시간을 알아내기 위해~~ 타이머 시작!!
            AlignTimer.StartChip(m_tp.chipList[chipIndex]);
            jTimer.ResetTimer();
            jTimer.StartTimer();


            //chip no.
            chipNo = m_tp.chipList[chipIndex];
            m_procState.curItemNo = chipNo;


            //Approach
            if ((chipIndex == 0) && (m_tp.faArrangement != true) && (m_tp.alignment == true))
            {
                AlignTimer.RecordTime(TimingAction.Approach);

                if (frmDistSens != null) frmDistSens.StopSensing();

                Invoke(dsi, "Approach out stage ");
                ApproachOut(APPROACHBUFFDIST, CHIP2FADIST);

                frmDistSens?.StopSensing();

                if (m_stopFlag == true) break;
            }


            //Alignment 
            bool alignSuccess = false;
            if (m_tp.alignment)
            {
                Invoke(dsi, "Alignment");
                if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
                //set align source
                CGlobal.osw?.SetToAlign();

                var portDistance = m_tp.outPitch * (pmPorts.Length - 1);

                //run aligning (※ Output FA만 fineSearch)
                bool alignChange = mChangeChipList.Contains(chipIndex + 1);

                alignSuccess = Alignment(pmPorts[0], pmPorts[3], chCenterWaves[0], chCenterWaves[3], CGlobal.AlignThresholdPower, alignChange);

                if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();
                if (m_stopFlag == true) break;
            }
            else
            {
                //alignment를 uncheck하면
                //algnment는 success된걸로 한다.!!
                alignSuccess = true;
            }


            //포지션 획득.
            if (alignSuccess)
            {
                if (m_tp.alignment)
                {
                    currentPos = readCoord(mLeft, mRight, mCenter);
                    currentPos.chipIndex = chipIndex;

                    if (alignSuccess) alignPosList.Add(currentPos);
                    else Invoke(dsi, "Alignment 실패!!");

                    if ((alignSuccess) && (chipIndex == 0))
                    {
                        //완료 후 복귀 포지션
                        retPosOut = currentPos.Out;
                        retPosCtr = currentPos.Other;
                    }
                } 
            }
            else
            {
                Invoke(dsi, "Alignment 실패!! 다음칩으로 ...");
            }



            //measurement. 
            if (alignSuccess)
            {

                Cmeasure meas = new Cmeasure();
                meas.chipNo = m_tp.chipList[chipIndex];
                meas.msrTime = DateTime.Now;
                //meas.pos = alignPosList.Last();

                if (m_tp.measurement)
                {

                    Invoke(dsi, "measurment");
                    //display off.
                    if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

                    //measurement
                    try
                    {
                        CGlobal.osw?.SetToTls();
                        meas.sd = await doMeasure(chipIndex, pmPorts);      //sweep
                    }
                    finally
                    {
                        CGlobal.osw?.SetToAlign();
                    }

                    //display off.
                    if (frmDigitalPwr != null) frmDigitalPwr.DisplayOn();

                    //save
                    if (m_tp.autoSave == true)
                    {
                        Invoke(dsi, "save data.");
                        string filePath = RawTextFile.BuildFileName(m_tp.saveFolderPath, meas.chipNo);
                        meas.sd.WriteTransmitance(filePath);

                    }

                } //m_tp.measurement

                m_msrList.Add(meas);

            }//measurement. 


            //plot
            Invoke(dsi, "plot data.");
            Invoke(pca, m_tp.chipList[chipIndex]);
            Invoke((Action)(() => Inspection(m_tp.chipList[chipIndex])));

            if (m_stopFlag == true) break;


            //alignment가 uncheck되면 칩 하나만 측정하고 나간다.
            if (m_tp.alignment == false)
            {
                AlignTimer.EndChip();
                break;
            }

            //move to next chip
            if (chipIndex != (m_tp.chipList.Length - 1))
            {
                Invoke(dsi, "move next chip.");
                MoveNextChip(alignPosList, m_tp.chipWidth, chipIndex);
            }

            if (m_stopFlag == true) break;


            //time 측정 끝!!
            jTimer.StopTimer();
            m_procState.SetItemProcTime(jTimer.GetLeadTime().TotalSeconds);

            //update chip list process state
            Invoke(uclps);

            AlignTimer.EndChip();

        }// for (int i = 0; i < m_tp.chipNos.Length; i++)


        //완료 처리.
        AlignTimer.RecordTime(TimingAction.SaveAndPlot);

        if (m_stopFlag == true)
        {
			//stop stage.
			try
			{
				mRight.StopMove();
				mCenter.StopMove();
			}
			catch
			{
				//do nothing.
			}

			m_procState.msg = "Process has stopped!!";
            m_procState.endTime = DateTime.Now;
            m_procState.compeleted = true;


            string msg = "작업이 취소되었습니다. \n";
            msg += "초기 위치로 이동(Yes), 멈춤(No)";
            DialogResult dialRes = DialogResult.No;
            dialRes = MessageBox.Show(msg, "확인",  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialRes == DialogResult.Yes) MoveTo(retPosOut, retPosCtr);

        }
        else
        {
            m_procState.msg = "Process has completed!!";
            m_procState.endTime = DateTime.Now;
            m_procState.compeleted = true;
            Invoke(dsi, "측정 완료!!");

            //초기 위치로 이동
            if (m_tp.AutoReturn) MoveTo(retPosOut, retPosCtr);
        }
       
        //Enable optical source controller
        if (frmSourCont != null) frmSourCont.EnableForm();


        //칩바 측정시간에 원점 복귀 시간 포함
        AlignTimer.EndBar();

    }

    

    /// <summary>
    /// sweep!
    /// </summary>
    /// <param name="chipIndex"></param>
    /// <param name="pmPorts"></param>
    /// <returns></returns>
    private async Task<DutData> doMeasure(int chipIndex, int[] pmPorts)
    {
        var pmGains = m_tp.gainList.ToArray();

        AlignTimer.RecordTime(TimingAction.SweepCore);
        
        //core power
        var corePower = await mSweep.MeasureSpecturmNp_Dut(pmPorts, pmGains, mWaveStart, mWaveStop, mWaveStep);
        //calc trans
        mSweep.CalcTrans(corePower, m_ref);

        return corePower;
                
    }

    

    /// <summary>
    /// Stage들을 연다.
    /// </summary>
    private void OpenStages()
    {
        const int OPENDIST = 20000;

        //이동.
        mRight.RelMove(mRight.AXIS_Z, (-1) * OPENDIST);
        mRight.WaitForIdle();

    }


        
    /// <summary>
    /// load close-pos. from xml file.
    /// </summary>
    /// <param name="_filepath">config file path</param>
    /// <returns></returns>
    private closedPos LoadClosePos(string _filepath)
    {

        closedPos ret = new closedPos();
        XConfig conf = null;

        try
        {
            conf = new XConfig(_filepath);

			//out
			string strTemp = "";
			strTemp = conf.GetValue("CLOSEPOS_OUT");
            ret.outPos = Convert.ToDouble(strTemp);
			ret.outPos = Math.Round(ret.outPos, STGPOSXYZRES);
		}
        catch
        {
            //do nothing.
        }


        if (conf != null) conf.Dispose(); 

        return ret;
    }
    

    
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
                for (int i = 0; i < hdgvChipNos.RowCount; i++)
                {
                    for (int j = 0; j < hdgvChipNos.Rows[i].Cells.Count; j++)
                    {
                        cell = hdgvChipNos.Rows[i].Cells[j];
                        if (cell.Style.BackColor != Color.White) cell.Style.BackColor = Color.White;

                    }
                }


            }
            else
            {
                //----- 작업중 -----//

                //측정된 것까지 노란색으로 칠한다.
                string chipNo = "";
                DataGridViewCell cell = null;
                for (int i = 0; i < hdgvChipNos.RowCount; i++)
                {

                    chipNo = hdgvChipNos.Rows[i].Cells[1].Value.ToString();

                    if (null != m_msrList.Find(p => p.chipNo == chipNo))
                    {
                        for (int j = 0; j < hdgvChipNos.Rows[i].Cells.Count; j++)
                        {
                            cell = hdgvChipNos.Rows[i].Cells[j];
                            if (cell.Style.BackColor != Color.Yellow) cell.Style.BackColor = Color.Yellow;
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
            if (Application.OpenForms.OfType<ProgressForm>().Count() > 0) return;

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
    private Point SaveWndStartPos()
    {

        Point ret = new Point();
        XConfig conf = null;

        string temp = "";
        try
        {
            conf = new XConfig(mConfFilePath);


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
        tabControl1.Enabled = false;
        grpMeasurement.Enabled = false;
    }


    
    /// <summary>
    /// Disable window.
    /// </summary>
    private void EnableWnd()
    {
        tabControl1.Enabled = true;
        grpMeasurement.Enabled = true;
    }


    
    /// <summary>
    /// 초기 위치로 이동한다.
    /// </summary>
    private void MoveTo(CStageAbsPos  _posOut, CStageAbsPos  _posCtr)
    {

        const int stageGapOpen = 10000;

        if (m_msrList.Count() == 0) return;

        try
        {
            //stage gap to move
            mRight.RelMove(mRight.AXIS_Z, stageGapOpen * (-1));

            //-------- output ----------

            //X축 이동
            mRight.AbsMove(mRight.AXIS_X, _posOut.x);
            mRight.WaitForIdle(mRight.AXIS_Z);

            //Y축 이동
            mRight.AbsMove(mRight.AXIS_Y, _posOut.y);

            //tz 이동.
            mRight.AbsMove(mRight.AXIS_TZ, _posOut.tz);

      
            //-------- center ----------
            if (m_MoveNextChip == moveStage.Center)
            {
                var x = CGlobal.CenterAxis == mCenter.AXIS_X ? _posCtr.x : _posCtr.y;
                mCenter.AbsMove(CGlobal.CenterAxis, x);
                mCenter.WaitForIdle(CGlobal.CenterAxis);
            }

			//Z축 이동
			//mRight.AbsMove(mRight.AXIS_Z, (_posOut.z - stageGapOpen));
			//mRight.WaitForIdle(mRight.AXIS_Z);
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

        //g.IntervalY = 0.05;
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
                        
            //find chip data.
            Cmeasure meas = null;
            meas = m_msrList.Find(p => p.chipNo == _chipNo);
            int startPort = (m_tp.fa == FaType.MMF) ? 1 : 5;
            DataPlot.Plot(_wg, meas.sd, InspectionGrid.ShiftPeak, startPort);
        }
        catch
        {
            //do nothing.
        }
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
            inspectionGrid.SetValue(analyze);

        }
        catch
        {
            //do nothing.
        }
    }



    private void MoveNextChip(List<AlignPosition> positions, int chipWdith, int chipIndex)
    {
        const int bufferDistance = 60;
        const int ALIGNDIST = 10;            //[um]

        var distance = AlignLogic.CalcNextChip(positions, chipWdith, chipIndex);//[in~out][y~z]
        Action<double> moveCenter = dx => mCenter.RelMove(CGlobal.CenterAxis, dx);

        try
        {
            //stage open.
            mRight.RelMove(mRight.AXIS_Z, bufferDistance * (-1));

            //Y축 이동
            mRight.RelMove(mRight.AXIS_Y, distance[1][0]);

            //X축 이동 (Center Or Right)
            if (m_MoveNextChip == moveStage.Center) mCenter.RelMove(CGlobal.CenterAxis, -chipWdith);
            else mRight.RelMove(mRight.AXIS_X, chipWdith);

            //완료 대기.
            if (m_MoveNextChip == moveStage.Center) mCenter.WaitForIdle(CGlobal.CenterAxis);
            else mRight.WaitForIdle(mRight.AXIS_X);

            //output approach.
            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //move to align-distance.
            mRight.RelMove(mRight.AXIS_Z, ALIGNDIST * (-1));
            mRight.WaitForIdle(mRight.AXIS_Z);
        }
        catch (Exception ex)
        {
            Log.Write($"MuxBudForm.MoveNextChip():\n{ex.Message}\n{ex.StackTrace}");
        }
    }



    /// <summary>
    /// 다음칩으로 이동한다.
    /// lsm을 이용 1차 함수 parameter를 구하고
    /// 이를 이용하여 다음칩 위치를 추정하고 스테이지를 그 위치로 이동시킨다.
    /// </summary>
    /// <param name="_posList">aligned postion array</param>
    /// <param name="_chipWdith">chip width</param>
    /// <param name="_curIdx">현재 칩 index</param>
    private void MoveNextChip_OLD(List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {                
        const int STAGECLOSEMARGIN = 100;
        const int ALIGNDIST = 6;            //[um]

        Func<AlignPosition, double> getX = (pos) =>
         {
             return m_MoveNextChip == moveStage.Center ? 
             (CGlobal.CenterAxis == mCenter.AXIS_X ? pos.Other.x : pos.Other.y) : pos.Out.x;
         };

        try
		{
			//lsm를 이용하여 1차함수 parameter를 구한다.
			//y = ax+b
			//input 쪽 좌표가 기준.
			double ay = 0.0;//output y축 기울기 
			double by = 0.0;//output의 y축 절편    

			int posCnt = _posList.Count();
			if (posCnt < 2)
			{
				//--default--//
				ay = 0.0;
				by = _posList.Last().Out.y;
			}
			else
			{
				//--lsm--//

				//y축
				double[] xPoss = new double[posCnt];
				double[] yPoss = new double[posCnt];
				for (int i = 0; i < _posList.Count(); i++)  //output.
				{
                    //xPoss[i] = (m_MoveNextChip == moveStage.Center) ? _posList[i].Other.x : _posList[i].Out.x; ;
                    xPoss[i] = getX(_posList[i]);
                    yPoss[i] = _posList[i].Out.y;
				}
				JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref ay, ref by);

			}


			//next chip 위치 계산. 
			double nextPosOutY = 0.0;
			double nextPosOutX = 0.0;
			double nextPosOutZ = 0.0;

			if (posCnt < 2)
			{

                //x (Center Or Output)
                //if (m_MoveNextChip == moveStage.Center)
                //    nextPosOutX = (int)(_posList[0].Other.x - (_chipWdith * (_curIdx + 1)));
                //else nextPosOutX = (int)(_posList[0].Out.x + (_chipWdith * (_curIdx + 1)));
                int direction = (m_MoveNextChip == moveStage.Center) ? -1 : 1;
                nextPosOutX = (int)(getX(_posList[0]) + direction * (_chipWdith * (_curIdx + 1)));

                //y
                nextPosOutY = (int)(ay * nextPosOutX + by);
				//z
				nextPosOutZ = _posList.Last().Out.z - STAGECLOSEMARGIN;
			}
			else
			{

				int preChipIdx = _posList[posCnt - 2].chipIndex;
				int lastChipIdx = _posList[posCnt - 1].chipIndex;

                int dx;
                //if (m_MoveNextChip == moveStage.Center)
                //{
                //    dx = (int)(_posList[posCnt - 1].Other.x - _posList[posCnt - 2].Other.x);
                //    dx = (int)(dx / (lastChipIdx - preChipIdx));
                //    //x _ Center Stage
                //    nextPosOutX = (int)(_posList[posCnt - 1].Other.x + (dx * (_curIdx - lastChipIdx + 1)));
                //}
                //else
                //{
                //    dx = (int)(_posList[posCnt - 1].Out.x - _posList[posCnt - 2].Out.x);
                //    dx = (int)(dx / (lastChipIdx - preChipIdx));
                //    //x _ Right Stage
                //    nextPosOutX = (int)(_posList[posCnt - 1].Out.x + (dx * (_curIdx - lastChipIdx + 1)));
                //}
                dx = (int)(getX(_posList[posCnt - 1]) - getX(_posList[posCnt - 2]));
                dx = (int)(dx / (lastChipIdx - preChipIdx));
                nextPosOutX = (int)(getX(_posList[posCnt - 1]) + (dx * (_curIdx - lastChipIdx + 1)));


                //y
                nextPosOutY = (int)(ay * nextPosOutX + by);
				//z
				nextPosOutZ = _posList.Last().Out.z - STAGECLOSEMARGIN;
			}


			//stage open.
			mRight.AbsMove(mRight.AXIS_Z, nextPosOutZ);

            //X축 이동 (Center Or Right)
            if (m_MoveNextChip == moveStage.Center)
                mCenter.AbsMove(mCenter.AXIS_X, nextPosOutX);
            else mRight.AbsMove(mRight.AXIS_X, nextPosOutX);

            //Y축 이동
            mRight.AbsMove(mRight.AXIS_Y, nextPosOutY);

            //완료 대기.
            if (m_MoveNextChip == moveStage.Center) mCenter.WaitForIdle(mCenter.AXIS_X);
            else mRight.WaitForIdle(mRight.AXIS_X);

			//output approach.
			mAlign.ZappSingleStage(mRight.stageNo);
			if (m_stopFlag == true)
				throw new ApplicationException();


			//move to align-distance.
			mRight.RelMove(mRight.AXIS_Z, ALIGNDIST * (-1));
			mRight.WaitForIdle();


		}
		catch
		{
			//do nothing
		}


	}


    
    /// <summary>
    /// Fa를 칩에 맞춘다. ty만 진행, tx는 진행하지 않는다.
    /// </summary>
    private void FaArrangement()
    {

        const int STAGEOPENDIST = 50;   //[um]
        const int ALIGNDIST = 10;       //[um]

        try
        {
            //stage open.
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (m_stopFlag == true)
                throw new ApplicationException();


            //output approach.
            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //output ty
            mAlign.AngleTy(mRight.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //stage open.
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (m_stopFlag == true)
                throw new ApplicationException();


            //output approach.
            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            mRight.RelMove(mRight.AXIS_Z, ALIGNDIST * (-1));
            mRight.WaitForIdle();

        }
        catch
        {
            //do nothing.
        }

        

    }


    
    /// <summary>
    /// alignment 실행.
    /// </summary>
    /// <param name="port1">port for channel 1</param>
    /// <param name="port2">port for channel last</param>
    /// <param name="_wavelen1">Channel1의 wavelength</param>
    /// <param name="_wavelen2">channellast의 waveleneth</param>
    /// <param name="_thresPowr">Alignment됬다고 보는 광파워. [dBm]</param>
    /// <returns>광을 못찾거나 취소하면 false.</returns>
    private bool Alignment(int _port1, int _port2, int _wavelen1, int _wavelen2, int _thresPowr, bool _changeAlign)
    {
        const double XYSEARCHSTEP = 1;//[um]
        const int SYNCSEARCHRNG = 100; //[um]
        const double SYNCSEARCHSTEP = 5;//[um]
        bool ret = false;

        double[] powerDiff = { 10, 18 };
        double[] rangeFactor = { 1.5, 2.0 };
        XYSearchParam paramRight = CGlobal.XySearchParamRight.Clone();

        try
        {
            if (_changeAlign)
            {
                paramRight.RangeX = mChangedScanRange;
                paramRight.RangeY = mChangedScanRange;
            }

            //align할 port선택
            //짧은 파장으로 align한다.
            int alignPort = 0;
            if (_wavelen1 < _wavelen2) alignPort = _port1;
            else alignPort = _port2;

            paramRight.Port = alignPort;

            //Align 가능한가?
            var power = Unit.MillWattTodBm(mPm.ReadPower(alignPort));
            if (power < _thresPowr)
            {
                //Sync Search 시도.(광을 찾은 상태가 아니면 )
                mAlign.SyncXySearch(alignPort, SYNCSEARCHRNG, SYNCSEARCHSTEP, _thresPowr);

                if (m_stopFlag == true) throw new ApplicationException();
                
                power = Unit.MillWattTodBm(mPm.ReadPower(alignPort));
                if (power < _thresPowr) throw new ApplicationException();
            }

            //fine search out
            mAlign.XySearch(mRight.stageNo, alignPort, XYSEARCHSTEP);

            //paramRight.RangeScaleFactor = 1;
            //if (power < XYSearchParam.LastPeakPower - powerDiff[0]) paramRight.RangeScaleFactor = rangeFactor[0];
            //if (power < XYSearchParam.LastPeakPower - powerDiff[1]) paramRight.RangeScaleFactor = rangeFactor[1];
            //paramRight.StageNo = m_rightStg.stageNo;
            //mAlign.XySearch(paramRight);

            if (m_stopFlag == true) throw new ApplicationException();
            
            //power = Unit.MillWattTodBm(m_mpm.ReadPwr(alignPort));

            ret = true;

        }
        catch
        {
            ret = false;
        }

        return ret;
    }


    
    /// <summary>
    /// Approach out state.
    /// 1.open stage 
    /// 2.approach
    /// 3.open state
    /// </summary>
    /// <param name="beforeDist">approach 전</param>
    /// <param name="afterDist">approach 완료 후 (FA와 칩간의 거리) </param>
    /// <returns></returns>
    private bool ApproachOut(int _beforeDist, int _afterDist)
    {

        bool ret = false;

        _beforeDist = Math.Abs(_beforeDist) * (-1);
        _afterDist = Math.Abs(_afterDist) * (-1);

        try
        {
            //out 후진 (안정상 후진후 approach 실시한다.)
            mRight.RelMove(mRight.AXIS_Z, _beforeDist);
            mRight.WaitForIdle(mRight.AXIS_Z);

            if (m_stopFlag == true) throw new ApplicationException();

            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //out 후진 (광파워 맥스될 FA와 칩간의 거리 )
            mRight.RelMove(mRight.AXIS_Z, _afterDist);
            mRight.WaitForIdle(mRight.AXIS_Z);

            ret = true;
        }
        catch
        {
            ret = false;
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
    /// 간단한 정보를 ToolStripLabel에 출력한다.!!
    /// </summary>
    /// <param name="_msg"></param>
    private void writeStatus(string _msg)
    {
        if (InvokeRequired) Invoke((Action)(() => tsslbStatus.Text = _msg));
        tsslbStatus.Text = _msg;
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


    
	/// <summary>
	/// Config 설정을 불러온다
	/// </summary>
	/// <param name="confFilepath"></param>
	private void LoadConfig(string confFilepath)
	{
		XConfig conf = null;
		try
		{
			string strTemp = "";
			conf = new XConfig(confFilepath);

            //-------------Part 1
            txtChipWidth.Text = conf.GetValue("CHIPWIDTH");
            cbCorepitch.Text = conf.GetValue("COREPITCH");

            strTemp = conf.GetValue("FA");                                                              //FA Type (SMF 5~8) or (MMF 1~4)
            if (Convert.ToInt32(strTemp) == (int)FaType.SMF) rbtnFA_SMF.Checked = true;
            else                                             rbtnFA_MMF.Checked = true;

            strTemp = conf.GetValue("CHDIRECTION");                                                     //channel direction (Forward or Reverse)
            if (Convert.ToInt32(strTemp) == (int)ChipDirection.Forward) rbtnChDirForward.Checked = true;
            else                                                        rbtnChDirReverse.Checked = true;

            strTemp = conf.GetValue("GAINS");                                                           //Gains
            if (strTemp == "1") rbtnGain1.Checked = true;
            else rbtnGain2.Checked = true;

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
                        
            strTemp = conf.GetValue("MOVE_NEXT_BY_CENTER");                                             //Center Stage
            if (strTemp == "1") chkCenterStage.Checked = true;
            else                chkCenterStage.Checked = false;


            //-------------Part 3
            strTemp = conf.GetValue("AUTOSAVE");                                                        //auto save
            if (strTemp == "0") chkAutoSave.Checked = false;
            else                chkAutoSave.Checked = true;
            grpAutosave.Enabled = chkAutoSave.Checked;

            strTemp = conf.GetValue("AUTOSAVEFULL");                                                    //Save Ragne Full : Custom
            if (Convert.ToInt32(strTemp) == (int)AutoSave.Full) rbtnAutoSaveFull.Checked = true;
            else                                                rbtnAutoSaveRng.Checked = false;

            strTemp = conf.GetValue("SAVEFOLDERPATH");
			lbSaveFolderPath.Text = strTemp;

			strTemp = conf.GetValue("SAVERNGSTART");
			txtSaveRangeStart.Text = strTemp;

			strTemp = conf.GetValue("SAVERNGSTOP");
			txtSaveRangeStop.Text = strTemp;

            //-------------Part 4     
            strTemp = conf.GetValue("RETCHIP1");
			if (strTemp == "1") chkAutoReturn.Checked = true;
			else                chkAutoReturn.Checked = false;

			strTemp = conf.GetValue("ITEMPROCESS_TIME");
			m_procState.SetAvgProcTime(Convert.ToDouble(strTemp));

		}
		catch
		{
			MessageBox.Show("설정값을 불러오든데 실패!! \n기본값 사용.",
							"에러",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
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

        m_tp.saveFolderPath = path;
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
            m_tp.saveFolderPath = folder;
        }
        return true;
    }



    /// <summary>
    /// Config 설정을 저장한다.
    /// </summary>
    /// <param name="confFilepath"></param>
    private void SaveConfig()
	{
		XConfig conf = null;
		try
		{
			conf = new XConfig(mConfFilePath);

            //-------------Part 1
            conf.SetValue("CHIPWIDTH", txtChipWidth.Text);
            conf.SetValue("COREPITCH", cbCorepitch.Text);
            conf.SetValue("GAINS", (rbtnGain1.Checked) ? "1" : "2");                                                //Gains
            conf.SetValue("FA", (rbtnFA_SMF.Checked) ? 
                          ((int)FaType.SMF).ToString() : ((int)FaType.MMF).ToString());                             //FA Type (SMF 5~8) or (MMF 1~4)
            conf.SetValue("CHDIRECTION", (rbtnChDirForward.Checked) ?
                          ((int)ChipDirection.Forward).ToString() : ((int)ChipDirection.Reverse).ToString());       //Channel Direction (Forward or Reverse)


            //-------------Part 2
            conf.SetValue("FAARRANGEMENT", (chkFaArrangement.Checked) ? "1" : "0");                                 //FA Arrangement
            conf.SetValue("MEASUREMENT", (chkMeasurement.Checked) ? "1" : "0");                                     //Measurement
            conf.SetValue("ALIGNMENT", (chkAlignment.Checked) ? "1" : "0");                                         //Alignment
            conf.SetValue("AUTO_RETURN", (chkAutoReturn.Checked) ? "1" : "0");                                      //Auto Return
            conf.SetValue("MOVE_NEXT_BY_CENTER", (chkCenterStage.Checked) ? "1" : "0");                             //Center Stage
            

            //-------------Part 3
            conf.SetValue("AUTOSAVE", (chkAutoSave.Checked) ? "1" : "0");                                           //Auto Save
            conf.SetValue("AUTOSAVEFULL", (rbtnAutoSaveFull.Checked) ? "1" : "0");                                  //Save Range Full : Custom
			conf.SetValue("SAVERNGSTART", txtSaveRangeStart.Text);                                                  //Save Range Custom - Start WL
            conf.SetValue("SAVERNGSTOP", txtSaveRangeStop.Text);                                                    //Save Range Custom - Stop WL
            conf.SetValue("SAVEFOLDERPATH", lbSaveFolderPath.Text);                                                 //Save Folder


            //-------------Part 4   
            conf.SetValue("ITEMPROCESS_TIME", m_procState.GetAvgProcTime().ToString());                             //평균 item 처리 

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


    #endregion




    #region Constructor / Destructor


    public MuxBudForm()
    {
        InitializeComponent();

        initGraph(_wg);
    }



    string mConfFilePath = Application.StartupPath + @"\config\conf_cwdmMuxFaBud.xml";

    /// <summary>
    /// init form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmCwdmMuxFa_Load(object sender, EventArgs e)
    {

        mPm = CGlobal.Opm;
        mRight = CGlobal.RightAligner;
        mLeft = CGlobal.LeftAligner;
        mCenter = CGlobal.OtherAligner;
        mSweep = CGlobal.sweepLogic;
        mAlign = CGlobal.alignLogic;
        m_distSens = CGlobal.Ds2000;
        m_MoveNextChip = (chkCenterStage.Checked) ? moveStage.Center : moveStage.Right;

        mSweep.mReporter += writeStatus;
        if (mAlign != null) mAlign.mReporter += writeStatus;

        m_msrList = new List<Cmeasure>();
        m_procState = new CprogRes();


        //ref.
        m_ref = new ReferenceDataNp();

        //if (!m_ref.LoadFromTxt(CGlobal.g_refPath))
        if (!m_ref.LoadReferenceFromTxt(Application.StartupPath + "\\refNonpol.txt"))
        {
            MessageBox.Show("레퍼런스 값을 불러오는데 실패!!", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            m_ref = null;
        }

        //option & configs.
        this.Location = LoadWndStartPos(mConfFilePath);
        LoadConfig(mConfFilePath);

        initInspection();

        //center Stage 사용 유무
        chkCenterStage.Enabled = mCenter != null;

        //쓰레드 가동.
        m_autoEvent = new AutoResetEvent(false);
        m_thread = new Thread(ThreadFunc);
        m_thread.Start();

    }



    private void initInspection()
    {
        inspectionGrid.InitColumnRow();

        var conf = new XConfig(mConfFilePath);
        var showDwl = conf.GetValue("INSPECTIN_SHOW_DWL", "0").Contains("1");
        var item = showDwl ? new string[] { "IL", "DWL" } : new string[] { "IL" };
        inspectionGrid.AddRow(item);

        var range = conf.GetValue("INSPECTION_IL_RANGE", "-10.8;0.6").Unpack<double>().ToArray();
        txtPassRangeIlMin.Value = (decimal)range[0];
        txtPassRangeIlUnif.Value = (decimal)range[1];
        inspectionGrid.SetPassRange(item[0], range, false);
        inspectionGrid.SetPassTest(item[0], (r, v) => r < v);//IL test

        if (showDwl)
        {
            range = conf.GetValue("INSPECTION_DWL_RANGE", "0.5;0.5").Unpack<double>().ToArray();
            inspectionGrid.SetPassRange(item[1], range, true);
            inspectionGrid.SetPassTest(item[1], (r, v) => r > Math.Abs(v));//DWL test
        }
    }



    private void btnPassRangeApply_Click(object sender, EventArgs e)
    {
        var item = inspectionGrid.InspectionItem;
        double[] passRange = new double[]
        {
            (double)txtPassRangeIlMin.Value, (double)txtPassRangeIlUnif.Value
        };
        inspectionGrid.SetPassRange(item[0], passRange, false);

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



    /// <summary>
    /// terminate form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmCwdmMuxFa_FormClosing(object sender, FormClosingEventArgs e)
    {
        //save options 
        SaveWndStartPos();
        SaveConfig();


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

    }


    #endregion




    #region UI Event


    bool mChangeScanRange = false;
    List<int> mChangeChipList = new List<int>();
    double mChangedScanRange = 60;



    /// <summary>
    /// 측정 시작!!
    /// </summary>
    private void btnMeasure_Click(object sender, EventArgs e)
    {

        //측정중이면 나간다.!!
        if (m_isRuning == true) return;


        //측정 할 칩 갯수 확인
        int chipCnt = hdgvChipNos.RowCount;
        if (chipCnt < 1)
        {
            MessageBox.Show("측정할 칩 개수가 0개 입니다.");
            return;
        }

        if (!checkFolder()) return;

        try
        {
            DisableWnd();

            //gain level
            m_tp.gainList = new List<int>();
            if (rbtnGain1.Checked == true)
            {
                m_tp.numGains = 1;
                m_tp.gainList.Add(mGain1);
            }
            else
            {
                m_tp.numGains = 2;
                m_tp.gainList.Add(mGain1);
                m_tp.gainList.Add(mGain2);
            }


            //칩 간격, FA종류, 방향, coreptich
            m_tp.chipWidth = Convert.ToInt32(txtChipWidth.Text);    //chip 간격
            m_tp.outPitch = Convert.ToInt32(cbCorepitch.Text);      //out channel coreptich.

            if (rbtnFA_SMF.Checked == true) m_tp.fa = FaType.SMF;
            else m_tp.fa = FaType.MMF;

            if (rbtnChDirForward.Checked == true) m_tp.chDirect = ChipDirection.Forward;
            else m_tp.chDirect = ChipDirection.Reverse;


            //scan & save  range
            m_tp.saveRngStart = Convert.ToInt32(txtSaveRangeStart.Text);
            m_tp.saveRngStop = Convert.ToInt32(txtSaveRangeStop.Text);
            m_tp.autoSave = chkAutoSave.Checked;

            if (rbtnAutoSaveFull.Checked == true) m_tp.autoSaveType = AutoSave.Full;
            else m_tp.autoSaveType = AutoSave.Range;


            //fa Arrangement, alignment , measurment
            m_tp.faArrangement = chkFaArrangement.Checked;
            m_tp.alignment = chkAlignment.Checked;
            if (mRight == null) m_tp.alignment = false;
            m_tp.measurement = chkMeasurement.Checked;


            //chip numbers
            m_tp.chipList = new string[hdgvChipNos.RowCount];
            for (int i = 0; i < hdgvChipNos.RowCount; i++)
            {
                m_tp.chipList[i] = (string)(hdgvChipNos.Rows[i].Cells[1].Value);
            }

            mChangeScanRange = checkScan.Checked;
            var a = txtChipNo.Text.Split(' ');
            foreach (var item in a)
                mChangeChipList.Add(int.Parse(item));
            mChangedScanRange = double.Parse(txtScanRange.Text);

            //자동귀환
            m_tp.AutoReturn = this.chkAutoReturn.Checked;

            //Center Or Right
            m_MoveNextChip = (chkCenterStage.Checked) ? moveStage.Center : moveStage.Right;

            m_tp.cmd = CMD.RUN;

            //측정 시작
            m_autoEvent.Set();
            Thread.Sleep(100);

        }
        catch (Exception ex)
        {
            EnableWnd();
            MessageBox.Show(ex.ToString());
        }


    }



    /// <summary>
    /// 자동 측정 선택
    /// </summary>
    private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox chk = (CheckBox)sender;
        if (chk.Checked == true) grpAutosave.Enabled = true;
        else                     grpAutosave.Enabled = false;

    }



    /// <summary>
    /// save folder path를 선택한다.
    /// </summary>
    private void btnSaveFolder_Click(object sender, EventArgs e)
    {

        FolderBrowserDialog fd = new FolderBrowserDialog();
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
                MessageBox.Show("칩 개수는 최소한 1개 이상", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        catch
        {
            MessageBox.Show("칩갯수를 정확히 입력해주세요.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }



        //칩 넘버 array를 만든다.!!
        string[] strTempArr = null;
        string strWfNo = "";    //wafer no.
        string strDate = "";    //date
        int startChipNo = 0;    //start chip no.
        string[] strChipNos = null;
        try
        {
            strTempArr = txtFisrtChipNo.Text.Split('-');
            if (strTempArr.Length < 5)
            {
                MessageBox.Show("입력한 칩넘버 이상.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < strTempArr.Length - 2; i++)
            {
                strWfNo += strTempArr[i] + "-";
            }

            strWfNo += strTempArr[strTempArr.Length - 2].Substring(0, 1);
            startChipNo = Convert.ToInt32(strTempArr[strTempArr.Length - 2].Substring(1));
            strDate = strTempArr[strTempArr.Length - 1];

            strChipNos = new string[Convert.ToInt32(txtChipCnt.Text)];
            for (int i = 0; i < Convert.ToInt32(txtChipCnt.Text); i++)
            {
                strChipNos[i] = strWfNo;
                strChipNos[i] += String.Format("{0:D2}", (startChipNo + i));
                strChipNos[i] += "-" + strDate;
                strChipNos[i] = strChipNos[i].ToUpper();
            }
        }
        catch
        {
            strChipNos = null;
            MessageBox.Show("칩넘버를 만드는데 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }



        //grid setting.
        string[] strColumArr = "No | Chip No. | note ".Split('|');
        string[] strValueArr = new string[strColumArr.Length];
        hdgvChipNos.HanDefaultSetting();
        hdgvChipNos.DeleteAllRows();
        hdgvChipNos.Font = new Font("Source Code Pro", 7, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
        hdgvChipNos.MultiSelect = false;
        hdgvChipNos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        hdgvChipNos.ReadOnly = true;
        hdgvChipNos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        hdgvChipNos.AllowUserToOrderColumns = false;
        hdgvChipNos.AllowUserToResizeRows = false;
        hdgvChipNos.SetColumns(ref strColumArr);


        //그리드에 칩넘버 입력!!
        for (int i = 0; i < strChipNos.Length; i++)
        {
            strValueArr[0] = Convert.ToString(i + 1);       //no.
            strValueArr[1] = strChipNos[i];                 //chip number.
            strValueArr[2] = "";
            hdgvChipNos.Insert(ref strColumArr, ref strValueArr);
        }
        hdgvChipNos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

    }



    /// <summary>
    /// chip no.를 모두 삭제한다.
    /// </summary>
    private void btnDelAllChipNos_Click(object sender, EventArgs e)
    {
        hdgvChipNos.DeleteAllRows();
    }



    /// <summary>
    /// stage open한다.
    /// </summary>
    private void btnOpenStages_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true) return;

        //execution.
        try
        {
            DisableWnd();

            m_tp.cmd = CMD.OpenStages;
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
    /// Stop...
    /// </summary>
    private void btnStop_Click(object sender, EventArgs e)
    {

        if (!m_isRuning) return;

        //confirm?
        DialogResult dialRes;
        dialRes = MessageBox.Show("작업이 진행중입니다. 중지하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (dialRes == DialogResult.No) return;

        //stop
        m_stopFlag = true;

    }



    /// <summary>
    /// 칩 데이터 출력!!
    /// </summary>
    private void hdgvChipNos_CellClick(object sender, DataGridViewCellEventArgs e)
    {

        string chipNo = (string)(hdgvChipNos.SelectedRows[0].Cells[1].Value);
        Plot(chipNo);
        Inspection(chipNo);

    }



    /// <summary>
    /// 칩 데이터 출력!!
    /// </summary>
    private void hdgvChipNos_SelectionChanged(object sender, EventArgs e)
    {
        if (hdgvChipNos.CurrentRow == null) return;
        if (hdgvChipNos.CurrentRow.Index < 0) return;
        if (hdgvChipNos.SelectedRows == null) return;
        if (hdgvChipNos.SelectedRows.Count < 1) return;

        string chipNo = (string)(hdgvChipNos.SelectedRows[0].Cells[1].Value);
        Plot(chipNo);
        Inspection(chipNo);
    }


    #endregion

}
