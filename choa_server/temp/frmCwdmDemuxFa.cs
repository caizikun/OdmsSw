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
using Free302.TnMLibrary;
using Free302.TnMLibrary.DataAnalysis;
using Neon.Aligner;
using System.IO;

public partial class frmCwdmDemuxFa : Form
{


    #region definition


    private const int PWRRESMW = 9;                 // 10^(-9) mW
    private const int PWRRESDBM = 3;                // 10^(-3) dBm
    private const int PWRRESDB = 3;                 // 10^(-3) dB

    private const int SWEEPRNG_START = 1260;        //start sweep wavelength [nm]
    private const int SWEEPRNG_STOP = 1360;         //stop sweep wavelength [nm]
    private const int SWEEPRNG_STEP = 50;           //step sweep wavelength [pm]
    private const int SWEEPSPEED = 40;              //sweep swpeed[nm/s]

    private const int WAVELEN_CH1 = 1271;           //[nm/s]
    private const int WAVELEN_CH2 = 1291;           //[nm/s]
    private const int WAVELEN_CH3 = 1311;           //[nm/s]
    private const int WAVELEN_CH4 = 1331;           //[nm/s]

    private int GAINLEVEL1 = CGlobal.PmGain[0];        //[dBm]
    private int GAINLEVEL2 = CGlobal.PmGain[1];        //[dBm]
    private double TLS_OUTPWR = CGlobal.TlsParam.Power;  //[dBm]

    //private const int ALIGN_THRESPOW = -30;       //[dBm] Align 성공여부 Threshold power.

    private const int FA_SMF = 0;
    private const int FA_MMF = 1;
    private const int DIRECTION_FORWARD = 0;        //ex)1271->1291->1311->1331
    private const int DIRECTION_REVERSE = 1;        //ex)1331->1311->1291->1271
    private const int AUTOSAVE_FULL = 0;
    private const int AUTOSAVE_RANGE = 1;


    #endregion
	



    #region structure/innor class


    private struct threadParam
    {
        public List<List<string>> chipBarList;

        public int gains;                                //number of gains. 
        public List<int> gainList;                       //
        public int chipWidth;                            //칩 간 간격
        public int outPitch;                             //output FA corepitch [um]     
        public int fa;                                   //SMF or MMF
        public int chDirect;                             //channel direction

        public double thresPwr;                          //alignment 성공 판단할 광파워.[dBm]
        //public int nextBarDistY;                       //다음 바의 y축으로 떨어진 거리.
		//public List<CalignPoint2d> barDistList;
        public List<CalignPoint2d> barChipDistList;
        public List<CalignPoint2d> barChipDistOutList;

        public bool autoSave;
        public int autoSaveType;                        //full or range.
        public int saveRngStart;                        //save range start wavelengh.
        public int saveRngStop;                         //save range stop wavelengh.

        public bool elimiateCladPwr;                    //clading mode power 제거??
        public bool alignment;                          //alignment. <-- uncheck하면 1칩만 측정됨.
        public bool measurement;                        //measurement.?
        public bool faArrangement;                      //FA arrangement?
        public bool roll;

		public double xfbInRng;	                        //parameters for 'XyFullBlind'
		public double xfbInStep;
		public double xfbOutRng;
		public double xfbOutStep;

		public string saveFolderPath;
    }




    private class CalignPos
    {
        public int chipIdx;
        public CStageAbsPos posIn;
        public CStageAbsPos posOut;
        public CStageAbsPos posCtr;
    }




    private class Cmeasure
    {
        public string chipNo;
        public DateTime msrTime;                        //측정 시간.
        public CalignPos pos;                           //aligned position.
        public SweepLogic.CswpNonpol sd;
        //public analy 
    }
	

    #endregion

	
	

    #region member variables


    private SweepLogic.CswpRefNonpol m_ref;

    private Itls m_tls;
    private IoptMultimeter m_mpm;
    private Istage m_leftStg;
    private Istage m_rightStg;
    private Istage m_ctrStg;	                //center stage.

    private SweepLogic m_swSys;
    private IAlignment m_align;

    bool m_stopFlag;
    bool m_isRuning;                            //running:true , stop :false
    private threadParam m_tp;
    private AutoResetEvent m_autoEvent;
    private Thread m_thread;
    private LogItem log;

    private CprogRes m_procState;
    private List<List<Cmeasure>> m_chipBarMsrList;


    private double m_stgClosedPosZin;           //stage가 closed되었을때 input pos.
    private double m_stgClosedPosZout;          //stage가 closed되었을때 input pos.



	//testmode
	bool mTestMode = false;
	int mCurrentTest = 1;
	bool mAutoReturn;
	int mWlstep = SWEEPRNG_STEP;
	int mCladDeltaX = 100;                      //μm
    
    #endregion

		


    #region thread function


    /// <summary>
    /// thread function.
    /// </summary>
    private void ThreadFunc()
    {
        const int APPROACHBUFFDIST = 40;                 //[um]    
        const int CHIP2FADIST = 6;                       //[um]


        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<string> dsi = DisplayShortInfor;
        Action<Cmeasure> pca = Plot;
        Action ew = EnableWnd;
        Action spw = ShowProgressWnd;


        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;


        CStageAbsPos beginPosIn = null;
        CStageAbsPos beginPosOut = null;
        CStageAbsPos beginPosCtr = null;

        CStageAbsPos retPosIn = null;
        CStageAbsPos retPosOut = null;
        CStageAbsPos retPosCtr = null;  //돌아갈 center stage position

        CStageAbsPos curPosIn = null;
        CStageAbsPos curPosOut = null;
        CStageAbsPos curPosCtr = null;  //현재 center stage position

        List<List<CalignPos>> chipBarPosList = new List<List<CalignPos>>();


        JeffTimer jTimer = new JeffTimer();

        while (true)
        {

            //신호 대기.
            m_isRuning = false;
            m_autoEvent.WaitOne();
            m_isRuning = true;
            m_stopFlag = false;


            //form instance 
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



            //저장공간 초기화.
            if (m_chipBarMsrList == null)
                m_chipBarMsrList = new List<List<Cmeasure>>();
            m_chipBarMsrList.Clear();


            //전체 측정 할 칩수.(모든 칩바) 와 칩바 갯수.
            int totalBarCnt = 0;
            int totalChipCnt = 0;
            for (int i = 0; i < m_tp.chipBarList.Count(); i++)
            {
                try
                {

                    if (m_tp.chipBarList[i].Count() != 0)
                        totalBarCnt++;
                
                    totalChipCnt += m_tp.chipBarList[i].Count();
                    
                }
                catch
                {
                    //do nothing.
                }
            }



            //마지막 칩바가 있는 layer.
            int lastLayer = m_tp.chipBarList.Count();
            for (int i = (m_tp.chipBarList.Count()-1); i > 0 ; i--)
            {
                try
                {
                    if (m_tp.chipBarList[i].Count() != 0)
                        break;
                }
                catch
                {
                    //do nothing.
                }

                lastLayer--;
            }



            //process state 초기화.
            m_procState.Clear();
            m_procState.compeleted = false;
            m_procState.totalItemCnt = totalChipCnt;
            m_procState.startTime = DateTime.Now;
            Invoke(spw);



            //chip 방향(칩의채널 방향)에 따른 파장 설정.
            int[] wavelens = new int[4];
            if (m_tp.chDirect == DIRECTION_FORWARD)
            {
                //-- 정방향 --//
                wavelens[0] = WAVELEN_CH1;
                wavelens[1] = WAVELEN_CH2;
                wavelens[2] = WAVELEN_CH3;
                wavelens[3] = WAVELEN_CH4;
            }
            else
            {
                //-- 역방향 --//
                wavelens[0] = WAVELEN_CH4;
                wavelens[1] = WAVELEN_CH3;
                wavelens[2] = WAVELEN_CH2;
                wavelens[3] = WAVELEN_CH1;
            }



            //FA 종류에 따른 Detector port 및 wavelength 설정 
            int[] ports = new int[4];
            if (m_tp.fa == FA_SMF)
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



            //시작 위치 저장.
            //left
            try { beginPosIn = m_leftStg.GetAbsPositions(); }
            catch { beginPosIn = new CStageAbsPos(); }
            //right
            try { beginPosOut = m_rightStg.GetAbsPositions(); }
            catch { beginPosOut = new CStageAbsPos(); }
            //center
            try { beginPosCtr = m_ctrStg.GetAbsPositions(); }
            catch { beginPosCtr = new CStageAbsPos(); }

            retPosIn = (CStageAbsPos)beginPosIn.Clone();
            retPosOut = (CStageAbsPos)beginPosOut.Clone();
            retPosCtr = (CStageAbsPos)beginPosCtr.Clone();

            chipBarPosList.Clear();



            //Disable optical source controller
            if (frmSourCont != null)
                frmSourCont.DisableForm();


            int curLayer = 0; //현재 layer
            bool isEmptyLayer = false;
            List<string> chipNoList = null;
            List<CalignPos> posList = null;
            List<Cmeasure> msrList = null;
            for (int i = 0; i < m_tp.chipBarList.Count(); i++)
            {

				if (m_tp.chipBarList[i].Count == 0)
					break;


				curLayer = i + 1;


                //check.... 칩바 작업 진행 or not?
                chipNoList = m_tp.chipBarList[i];
                try
                {
                    if ((chipNoList == null) || (chipNoList.Count() == 0))
                        isEmptyLayer = true;
                    else
                        isEmptyLayer = false;
                }
                catch
                {
                    isEmptyLayer = true;
                }

                if (isEmptyLayer == true)
                {
                    m_chipBarMsrList.Add(null);
                    chipBarPosList.Add(null);
                    continue;
                }

                
                //칩바 측정.
                string chipNo = "";
                posList = new List<CalignPos>();
                msrList = new List<Cmeasure>();
                for (int j = 0; j < chipNoList.Count(); j++)
                {
                    
                    AlignTimer.StartChip(chipNo); //time record by DrBae 2015-10-26


                    //칩측정 시간을 알아내기 위해~~ 타이머 시작!!
                    jTimer.ResetTimer();
                    jTimer.StartTimer();


                    //chip no.
                    chipNo = chipNoList[j];
                    m_procState.curItemNo = chipNo;

                    log.RecordLogItem(chipNo, "측정시작");                 //LogItem

                    //FA Arrangement.
                    if ((j == 0) && (m_tp.faArrangement == true))
                        FaArrangement();


                    //Approach
                    AlignTimer.RecordTime(TimingAction.Approach); //time record by DrBae 2015-10-26
                    if (m_tp.alignment == true)
                    {
                        if (frmDistSens != null)
                            frmDistSens.StopSensing();

                        Invoke(dsi, "Approach In,out stage ");
                        ApproachInOut(APPROACHBUFFDIST, CHIP2FADIST);

                        if (frmDistSens != null)
                            frmDistSens.StopSensing();

                        if (m_stopFlag == true)
                            break;
                    }

                    //LogItem(Align전 Position)
                    log.mPosIn.x = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_X);
                    log.mPosIn.y = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Y);
                    log.mPosIn.z = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
                    log.mPosIn.tx = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TX);
                    log.mPosIn.ty = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TY);
                    log.mPosIn.tz = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TZ);

                    log.mPosOut.x = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_X);
                    log.mPosOut.y = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Y);
                    log.mPosOut.z = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);
                    log.mPosOut.tx = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TX);
                    log.mPosOut.ty = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TY);
                    log.mPosOut.tz = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TZ);

                    log.mPosCenter.x = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_X);

                    log.RecordLogItem(AlignState.ChipMove);                 //LogItem



                    //Alignment 
                    bool alignSuccess = false;
                    if (m_tp.alignment == true)
                    {
                        Invoke(dsi, "Alignment");

                        if (frmDigitalPwr != null)
                            frmDigitalPwr.DisplayOff();


                        //첫번째 칩은 무조건 out Roll을 하고
                        //나머지는 out Roll을 하거나 XySearch한다.
                        bool outfine = false;
                        bool roll = false;
                        if (j == 0)
                        {
							//outfine = false;
							//roll = true;
							outfine = true;
							roll = false;
						}
						else
                        {
                            if (m_tp.roll == true)
                            {
                                outfine = false;
                                roll = true;
                            }
                            else
                            {
                                outfine = true;
                                roll = false;
                            }
                        }


						alignSuccess = Alignment(ports[0], ports[3],
												 wavelens[0], wavelens[3],
												 (int)m_tp.thresPwr,
												 true,
												 outfine,
												 roll,
												 (m_tp.outPitch * (ports.Length - 1)));


						if (frmDigitalPwr != null)
                            frmDigitalPwr.DisplayOn();

                        if (m_stopFlag == true)
                            break;

                    }
                    else
                    {
                        //alignment를 uncheck하면
                        //algnment는 success된걸로 한다.!!
                        alignSuccess = true;
                    }

                    if (alignSuccess == false)
                        Invoke(dsi, "Alignment 실패!! 다음칩으로 ...");




                    //포지션 획득.
                    if (alignSuccess == true)
                    {
                        
                        //-- input --//
                        if (curPosIn == null)
                            curPosIn = new CStageAbsPos();
                        try
                        {
                            curPosIn.x = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_X);
                            curPosIn.y = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Y);
                            curPosIn.z = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
                            curPosIn.tx = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TX);
                            curPosIn.ty = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TY);
                            curPosIn.tz = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TZ);
                        }
                        catch
                        {
                            curPosIn.x = 0;
                            curPosIn.y = 0;
                            curPosIn.z = 0;
                            curPosIn.tx = 0;
                            curPosIn.ty = 0;
                            curPosIn.tz = 0;
                        }

                        if (curPosOut == null)  //-- ouput --//
                            curPosOut = new CStageAbsPos();
                        try
                        {
                            curPosOut.x = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_X);
                            curPosOut.y = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Y);
                            curPosOut.z = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);
                            curPosOut.tx = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TX);
                            curPosOut.ty = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TY);
                            curPosOut.tz = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TZ);
                        }
                        catch
                        {
                            curPosOut.x = 0;
                            curPosOut.y = 0;
                            curPosOut.z = 0;
                            curPosOut.tx = 0;
                            curPosOut.ty = 0;
                            curPosOut.tz = 0;
                        }


                        //-- center --//
                        if (curPosCtr == null)
                            curPosCtr = new CStageAbsPos();
                        try
                        {
                            curPosCtr.x = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_X);
                        }
                        catch
                        {
                            curPosOut.x = 0;
                        }

                        //stage closed position 저장.				
                        m_stgClosedPosZin = curPosIn.z;
                        m_stgClosedPosZout = curPosOut.z;

                        log.mPosIn = curPosIn;
                        log.mPosOut = curPosOut;
                        log.mPosCenter = curPosCtr;
                        log.RecordLogItem(AlignState.AlignPass);                 //LogItem
                    }
                    else
                    {
                        log.mPosIn.x = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_X);
                        log.mPosIn.y = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Y);
                        log.mPosIn.z = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
                        log.mPosIn.tx = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TX);
                        log.mPosIn.ty = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TY);
                        log.mPosIn.tz = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TZ);

                        log.mPosOut.x = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_X);
                        log.mPosOut.y = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Y);
                        log.mPosOut.z = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);
                        log.mPosOut.tx = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TX);
                        log.mPosOut.ty = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TY);
                        log.mPosOut.tz = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TZ);

                        log.mPosCenter.x = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_X);

                        log.RecordLogItem(AlignState.AlignFail);                 //LogItem

                    }


                    //완료 후 복귀 포지션
                    if ((alignSuccess == true) && (i == 0) && (j == 0))
                    {
                        retPosIn = (CStageAbsPos)curPosIn.Clone();
                        retPosOut = (CStageAbsPos)curPosOut.Clone();
                        retPosCtr = (CStageAbsPos)curPosCtr.Clone();
                    }

                    if ((alignSuccess == true) || (posList.Count == 0))
					{
						CalignPos alignPos = new CalignPos();
						alignPos.chipIdx = 0;
						alignPos.posIn = (CStageAbsPos)curPosIn.Clone();
						alignPos.posOut = (CStageAbsPos)curPosOut.Clone();
						alignPos.posCtr = (CStageAbsPos)curPosCtr.Clone();
						posList.Add(alignPos);
					}
                    
                    
                    //measurement.
                    AlignTimer.RecordTime(TimingAction.SweepCore); //time recode by DrBae 2015-10-26
                    Cmeasure meas = null;
                    if (alignSuccess == true)
                    {

                        meas = new Cmeasure();
                        meas.chipNo = chipNo;
                        meas.msrTime = DateTime.Now;
                        meas.pos = posList.Last();

                        if (m_tp.measurement == true)
                        {

                            Invoke(dsi, "measurment");

                            //display off.
                            if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

                            //measurement
                            meas.sd = Measurement(ports,
                                                  m_tp.gainList.ToArray(),
                                                  SWEEPRNG_START,
                                                  SWEEPRNG_STOP,
                                                  Math.Round(mWlstep / 1000.0, 3),
                                                  m_tp.fa,
                                                  m_tp.chDirect,
                                                  m_ref,
                                                  m_tp.elimiateCladPwr);

                            //display off.
                            if (frmDigitalPwr != null)
                                frmDigitalPwr.DisplayOn();


                            //save
                            if (m_tp.autoSave == true)
                            {
                                Invoke(dsi, "save data.");

                                //edit by DrBae 2015-10-22
                                string filePath = RawTextFile.BuildFileName(m_tp.saveFolderPath, 
                                                                            meas.chipNo);
                                if (m_tp.autoSaveType == AUTOSAVE_FULL)
                                    meas.sd.SaveToTxt(filePath);
                                else
                                    meas.sd.SaveToTxt(filePath, m_tp.saveRngStart, m_tp.saveRngStop);

                                //cladding mode
                                RawDataNP.Last.SavePower(filePath);
                                RawDataNP.Last.SaveTotalLoss(filePath);
                            }
                        }


						msrList.Add(meas);
					}
                    

               

                    //plot
                    Invoke(dsi, "plot data.");
                    Invoke(pca, meas);


                    AlignTimer.EndChip(); //time record by DrBae 2015-10-26


					if (m_tp.alignment == false)
						break;

					//receive stop command from user??
					if (m_stopFlag == true)
                        break;


					//move to next chip
					if ((alignSuccess == false) && (j == 0))
						break;


					if (j != (chipNoList.Count() - 1))
                    {
                        Invoke(dsi, "move next chip.");
                        MoveNextChip(posList, m_tp.chipWidth, j);
                    }


                    //time 측정 끝!!
                    jTimer.StopTimer();
                    m_procState.SetItemProcTime(jTimer.GetLeadTime().TotalSeconds);


                }//one of chip bar.


                m_chipBarMsrList.Add(msrList);
                chipBarPosList.Add(posList);


				//chipBar의 포지션을 저장한다.
				try
				{
					string posFilePath = m_tp.saveFolderPath + "\\";
					posFilePath += "POS_";
					posFilePath += msrList[0].chipNo;
					posFilePath += ".txt";
					SaveChipBarPosToTxt(posFilePath, msrList);
				}
				catch
				{
					//do nothing.
				}
				


				//receive stop command from user??
				if (m_stopFlag == true)
                    break;

                //case no-alignment: measure only one chip and finish the work.
                if (m_tp.alignment == false)
                    break;


                //Move to next chipBar.
                if (curLayer == m_tp.chipBarList.Count())
                    break;

                if (curLayer == lastLayer)
                    break;

                CalignPos chipPos1st = null;
                try { chipPos1st = posList.First(); }
                catch { chipPos1st = null; }
                if (chipPos1st == null)
                {
                    chipPos1st = new CalignPos();
                    chipPos1st.posIn = new CStageAbsPos();
                    chipPos1st.posOut = new CStageAbsPos();
                    chipPos1st.posIn = m_leftStg.GetAbsPositions();
                    chipPos1st.posOut = m_rightStg.GetAbsPositions();
                }

                //MoveNextLayerChipBar(chipPos1st, m_tp.barChipDistList[i],CHIP2FADIST);
                MoveNextLayerChipBar(chipPos1st,
                                        m_tp.barChipDistList[i],
                                        m_tp.barChipDistOutList[i],
                                        CHIP2FADIST);




            }//chip bar list.

            




            //time record by DrBae 2015-10-26
            AlignTimer.EndBar();


            //완료 처리.
            if (m_stopFlag == true)
            {

                //stop stage.
                m_leftStg.StopMove();
                m_rightStg.StopMove();


                m_procState.msg = "Process has stopped!!";
                m_procState.endTime = DateTime.Now;
                m_procState.compeleted = true;

                string msg = "작업이 취소되었습니다. \n";
                msg += "초기 위치로 이동(Yes), 멈춤(No)";
                DialogResult dialRes = DialogResult.No;
                if (mTestMode || mAutoReturn)
                {
                    MoveToInitPos(retPosIn, retPosOut, retPosCtr);
                }
                else
                {
                    dialRes = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialRes == DialogResult.Yes)
						MoveToInitPos(retPosIn, retPosOut, retPosCtr);
                }

            }
            else
            {

                m_procState.msg = "Process has completed!!";
                m_procState.endTime = DateTime.Now;
                m_procState.compeleted = true;

                Invoke(dsi, "측정 완료!!");

                string msg = "작업이 완료되었습니다. \n";
                msg += "초기 위치로 이동(Yes), 멈춤(No)";
                DialogResult dialRes = DialogResult.No;
                if (mTestMode || mAutoReturn) MoveToInitPos(retPosIn, retPosOut, retPosCtr);
                else
                {
                    dialRes = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialRes == DialogResult.Yes)
						MoveToInitPos(retPosIn, retPosOut, retPosCtr);
                }
            }


            //tls wavelength 1번 
            m_tls.SetTlsWavelen(wavelens[0]);

            //화면 활성화!!
            Invoke(ew);

            //Enable optical source controller
            if (frmSourCont != null)
                frmSourCont.EnableForm();

            log.RecordLogItem("Measurement", "Bar 측정 종료");                 //LogItem
            log = null;

            //TESTmode by DrBae 2015-10-02
            if (mTestMode && !m_stopFlag)
            {
                mCurrentTest--;
                Invoke(new Action(() =>
                {
                    textNumMeasure.Text = mCurrentTest.ToString();
                    textNumMeasure.Update();
                }));

                if (mCurrentTest > 0)
                {
                    //this.Invoke(new Action(performMeasureClick));
                    Invoke(new Action(async () =>
                    {
                        await Task.Delay(5000 + totalChipCnt * 500);
                        btnMeasure.PerformClick();
                    }));
                }
            }//testmode



        }//while (true)
    }


	#endregion




	#region private method



	/// <summary>
	/// 칩바의 포지션을 저장한다.
	/// </summary>
	/// <param name="_filePath">file path.</param>
	/// <param name="_msrList">measrement list.</param>
	/// <returns></returns>
	private bool SaveChipBarPosToTxt(string _filePath, List<Cmeasure> _msrList )
	{

		bool ret = false;

		string strLineBuf = "";
		FileStream fs = null;
		StreamWriter sw = null;

		try
		{


			//File Open
			string filepath = _filePath;
			if (filepath == "")
				throw new ApplicationException("");

			fs = new FileStream(filepath, FileMode.Create);
			sw = new StreamWriter(fs);


			//header.
			//chip no. , in x,y,z,tx,ty,tz , out x,y,z,tx,ty,tz
			strLineBuf = "";
			strLineBuf = "chNo, in-x, y, z, tx, ty, tz, out-x, y, z, tx, ty, tz, ctr-x";
			sw.WriteLine(strLineBuf);


			//data
			strLineBuf = "";
			
			for (int i = 0; i < _msrList.Count ; i++)
			{
				strLineBuf = _msrList[i].chipNo + ", ";

				strLineBuf += _msrList[i].pos.posIn.x.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posIn.y.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posIn.z.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posIn.tx.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posIn.ty.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posIn.tz.ToString() + ", ";

				strLineBuf += _msrList[i].pos.posOut.x.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posOut.y.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posOut.z.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posOut.tx.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posOut.ty.ToString() + ", ";
				strLineBuf += _msrList[i].pos.posOut.tz.ToString() + ", ";

				strLineBuf += _msrList[i].pos.posCtr.x.ToString() ;

				sw.WriteLine(strLineBuf);

			}


			ret = true;

		}
		catch
		{
			ret = false;
		}
		finally
		{
			//File close
			if (sw != null)
				sw.Close();

			if (fs != null)
				fs.Close();
		}

		return ret;
	}




	/// <summary>
	/// 다음층 칩바로 이동한다.
	/// </summary>
	/// <param name="_curBarPos">현재 bar의 첫번째 칩 위치.</param>
	/// <param name="_barDist">다음 바까지 Y축 distance. </param>
	private void MoveNextLayerChipBar(CalignPos _curBarPos, CalignPoint2d _alignPt )
    {

        const int OPENDIST = 2000;
        const int CLOSEDIST = 2000;
        const int ALIGNDIST = 6;

        try
        {



			//stage open.
			m_leftStg.RelMove(m_leftStg.AXIS_Z, OPENDIST * (-1));
			m_rightStg.RelMove(m_rightStg.AXIS_Z, OPENDIST * (-1));
			m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


			//----- 현재 chipbar의 첫번째 칩 좌표로 이동.
			//center pos. 이동
			m_ctrStg.AbsMove(m_ctrStg.AXIS_X, _curBarPos.posCtr.x);
			m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);


			//tz 이동.
			m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _curBarPos.posOut.tz);

			//X축 이동
			m_leftStg.AbsMove(m_leftStg.AXIS_X, _curBarPos.posIn.x);
			m_rightStg.AbsMove(m_rightStg.AXIS_X, _curBarPos.posOut.x);


			//Y축 이동
			m_leftStg.AbsMove(m_leftStg.AXIS_Y, _curBarPos.posIn.y);
			m_rightStg.AbsMove(m_rightStg.AXIS_Y, _curBarPos.posOut.y);
			m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);



			//----- 다음 chipbar로 이동.
			//Y축 이동
			m_leftStg.RelMove(m_leftStg.AXIS_Y, _alignPt.y);
            m_rightStg.RelMove(m_rightStg.AXIS_Y, _alignPt.y);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);


			//X축 이동
			m_leftStg.RelMove(m_leftStg.AXIS_X, _alignPt.x );
			m_rightStg.RelMove(m_rightStg.AXIS_X, _alignPt.x);
			m_rightStg.WaitForIdle(m_rightStg.AXIS_X);




			//stage close
			m_leftStg.RelMove(m_leftStg.AXIS_Z, CLOSEDIST);
            m_rightStg.RelMove(m_rightStg.AXIS_Z, CLOSEDIST);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);



            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();

            //input approach.
            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //output approach.
            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.WaitForIdle();

        }
        catch
        {
            //do nothing.
        }


    }




    /// <summary>
	/// 다음층 칩바로 이동한다.
	/// </summary>
	/// <param name="_c1Pos">현재 bar의 첫번째 칩 위치.</param>
	/// <param name="_alignPt">현재 위치에서 다음 칩바까지의 이동거리(x,y)  ex) (80,-6030) </param>
	private void MoveNextLayerChipBar(CalignPos _curBarPos, CalignPoint2d _alignPt, int _alignDist)
    {

        const int OPENDIST = 2000;
        const int CLOSEDIST = 2000;

        try
        {

            //stage open.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, OPENDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, OPENDIST * (-1));
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


            //----- 현재 chipbar의 첫번째 칩 좌표로 이동.
            //center pos. 이동
            m_ctrStg.AbsMove(m_ctrStg.AXIS_X, _curBarPos.posCtr.x);
            m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);


            //tz 이동.
            m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _curBarPos.posOut.tz);

            //X축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_X, _curBarPos.posIn.x);
            m_rightStg.AbsMove(m_rightStg.AXIS_X, _curBarPos.posOut.x);


            //Y축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_Y, _curBarPos.posIn.y);
            m_rightStg.AbsMove(m_rightStg.AXIS_Y, _curBarPos.posOut.y);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);


            //다음 chipbar로 이동.
            m_leftStg.RelMove(m_leftStg.AXIS_Y, _alignPt.y);    //Y축 이동
            m_rightStg.RelMove(m_rightStg.AXIS_Y, _alignPt.y);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);

            m_leftStg.RelMove(m_leftStg.AXIS_X, _alignPt.x);    //X축 이동
            m_rightStg.RelMove(m_rightStg.AXIS_X, _alignPt.x);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_X);


            //stage close
            m_leftStg.RelMove(m_leftStg.AXIS_Z, CLOSEDIST);
            m_rightStg.RelMove(m_rightStg.AXIS_Z, CLOSEDIST);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


            //approach.
            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();

            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, _alignDist * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, _alignDist * (-1));
            m_rightStg.WaitForIdle();

        }
        catch
        {
            //do nothing.
        }


    }




    /// <summary>
	/// 다음층 칩바로 이동한다.
	/// </summary>
	/// <param name="_c1Pos">현재 bar의 첫번째 칩 위치.</param>
	/// <param name="_alignPt">현재 위치에서 다음 칩바까지의 이동거리(x,y) 
    ///                        input side ex) (80,-6030) </param>
    /// <param name="_alignPt">현재 위치에서 다음 칩바까지의 이동거리(x,y)
    ///                          output side ex) (80,-6030) </param>
	private void MoveNextLayerChipBar(CalignPos _curBarPos,
                                      CalignPoint2d _alignPtIn,
                                      CalignPoint2d _alignPtOut,
                                      int _alignDist)
    {

        const int OPENDIST = 2000;
        const int CLOSEDIST = 2000;

        try
        {

            //stage open.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, OPENDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, OPENDIST * (-1));
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


            //----- 현재 chipbar의 첫번째 칩 좌표로 이동.
            //center pos. 이동
            m_ctrStg.AbsMove(m_ctrStg.AXIS_X, _curBarPos.posCtr.x);
            m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);


            //tz 이동.
            m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _curBarPos.posOut.tz);

            //X축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_X, _curBarPos.posIn.x);
            m_rightStg.AbsMove(m_rightStg.AXIS_X, _curBarPos.posOut.x);


            //Y축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_Y, _curBarPos.posIn.y);
            m_rightStg.AbsMove(m_rightStg.AXIS_Y, _curBarPos.posOut.y);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);


            //다음 chipbar로 이동.
            m_leftStg.RelMove(m_leftStg.AXIS_Y, _alignPtIn.y);    //Y축 이동
            m_rightStg.RelMove(m_rightStg.AXIS_Y, _alignPtOut.y);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);

            m_leftStg.RelMove(m_leftStg.AXIS_X, _alignPtIn.x);    //X축 이동
            m_rightStg.RelMove(m_rightStg.AXIS_X, _alignPtOut.x);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_X);


            //stage close
            m_leftStg.RelMove(m_leftStg.AXIS_Z, CLOSEDIST);
            m_rightStg.RelMove(m_rightStg.AXIS_Z, CLOSEDIST);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


            //approach.
            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();

            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, _alignDist * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, _alignDist * (-1));
            m_rightStg.WaitForIdle();

        }
        catch
        {
            //do nothing.
        }


    }



    
    /// <summary>
    /// chip no. list를 만든다.
    /// </summary>
    /// <param name="_chipNo1st">첫번째 칩넘버.</param>
    /// <param name="_cnt">칩 갯수.</param>
    /// <returns>chip no. array</returns>
    private string[] MakeChipNos(string _chipNo1st, int _cnt)
    {

        string[] strChipNos = null;

        try
        {

            string[] strTempArr = null;
            string strWfNo = "";    //wafer no.
            string strDate = "";    //date
            int startChipNo = 0;    //start chip no.
            strTempArr = _chipNo1st.Split('-');
            if (strTempArr.Length < 5)
                throw new ApplicationException();


            for (int i = 0; i < strTempArr.Length - 2; i++)
            {
                strWfNo += strTempArr[i] + "-";
            }

            strWfNo += strTempArr[strTempArr.Length - 2].Substring(0, 1);
            startChipNo = Convert.ToInt32(strTempArr[strTempArr.Length - 2].Substring(1));
            strDate = strTempArr[strTempArr.Length - 1];

            strChipNos = new string[_cnt];
            for (int i = 0; i < _cnt; i++)
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
        }


        return strChipNos;

    }



    
    /// <summary>
    /// process resulst form을 띄운다.!!
    /// </summary>
    private void ShowProgressWnd()
    {
        try
        {

            if (Application.OpenForms.OfType<frmProcessRes>().Count() > 0)
                return;


            frmProcessRes frm = new frmProcessRes();
            frm.MdiParent = CGlobal.g_frmMain;
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




    /// <summary>
    /// stage가 closed 되었는지?
    /// </summary>
    /// <returns>true: close , false:open</returns>
    private bool IsStgClosed()
    {

        const int MARGIN = 1000;


        //처음엔 무조건 open 상태로 여긴다.
        if ((m_stgClosedPosZin == 0.0) && (m_stgClosedPosZout == 0.0))
            return false;


        //current pos.
        double posIn = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
        double posOut = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);


        //판단.
        if ((posIn > (m_stgClosedPosZin + MARGIN)) ||
            (posOut > (m_stgClosedPosZout + MARGIN)))
            return false;


        return true;

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
    private void MoveToInitPos(CStageAbsPos _posIn, 
							   CStageAbsPos _posOut,
							   CStageAbsPos _posCtr)

    {

        const int STAGEOPENDIST = 10000;


        if (m_chipBarMsrList.Count() == 0)
            return;

        try
        {

			//stage open.
			m_leftStg.RelMove(m_leftStg.AXIS_Z, STAGEOPENDIST * (-1));
			m_rightStg.RelMove(m_rightStg.AXIS_Z, STAGEOPENDIST * (-1));
			m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


			//center pos. 이동
			m_ctrStg.AbsMove(m_ctrStg.AXIS_X, _posCtr.x);
			m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);


			//tz 이동.
			m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _posOut.tz);

			//X축 이동
			m_leftStg.AbsMove(m_leftStg.AXIS_X, _posIn.x);
			m_rightStg.AbsMove(m_rightStg.AXIS_X, _posOut.x);


			//Y축 이동
			m_leftStg.AbsMove(m_leftStg.AXIS_Y, _posIn.y);
			m_rightStg.AbsMove(m_rightStg.AXIS_Y, _posOut.y);
			m_rightStg.WaitForIdle(m_rightStg.AXIS_Y);


			//Z축 이동
			m_leftStg.AbsMove(m_leftStg.AXIS_Z, (_posIn.z - STAGEOPENDIST));
			m_rightStg.AbsMove(m_rightStg.AXIS_Z, (_posOut.z - STAGEOPENDIST));
			m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


		}
        catch
        {
            //do nothing
        }

    }




    /// <summary>
    /// 칩의 한 채널 데이터를 출력한다.
    /// </summary>
    /// <param name="chipNo">chip no.</param>
    /// <param name="_chnlNo">channel no.</param>
    private void Plot(string _chipNo, int _chnlNo)
    {

        try
        {
            wfgTrans.ClearData();


            JeffColor jColor = new JeffColor();


            //find chip data.
            Cmeasure meas = null;
            for (int i = 0; i < m_chipBarMsrList.Count(); i++)
            {
                try
                {
                    meas = m_chipBarMsrList[i].Find(p => p.chipNo == _chipNo);
                }
                catch
                {
                    //do nothing
                }

                if (meas != null)
                    break;
            }

            if (meas == null)
                return;
            

            //find channel data.
            SweepLogic.CswpPortIlNonpol chnlData = null;
            chnlData = meas.sd.portDataList.Find(p => p.port == _chnlNo);



            //plot..
            NationalInstruments.UI.WaveformPlot wfpPwr = null;
            wfpPwr = new NationalInstruments.UI.WaveformPlot();
            wfpPwr.XAxis = wfgTrans.Plots[0].XAxis;
            wfpPwr.YAxis = wfgTrans.Plots[0].YAxis;
            wfpPwr.LineColor = System.Drawing.Color.White;
            wfpPwr.DefaultStart = meas.sd.startWavelen;
            wfpPwr.DefaultIncrement = Math.Round(meas.sd.stepWavelen, 3);
            wfpPwr.PlotY(chnlData.ilList.ToArray());
            wfgTrans.Plots.Add(wfpPwr);
            wfgTrans.Refresh();

        }
        catch
        {
            //do nothing.
        }
    }



    
    /// <summary>
    /// 칩의 모든 데이터(4channels)를 출력한다
    /// </summary>
    /// <param name="msr">measurement instance</param>
    private void Plot(Cmeasure _meas)
    {

        try
        {

            lbChipNo.Text = _meas.chipNo;

            wfgTrans.ClearData();

            //color
            Color[] colors = new Color[4];
            colors[0] = System.Drawing.Color.White;
            colors[1] = System.Drawing.Color.Gold;
            colors[2] = System.Drawing.Color.Cyan;
            colors[3] = System.Drawing.Color.Tomato;



            //plot..
            JeffColor color = new JeffColor();
            int chnlCnt = _meas.sd.portDataList.Count();
            for (int i = 0; i < chnlCnt; i++)
            {
                //channel data.
                SweepLogic.CswpPortIlNonpol chnlData = null;
                chnlData = _meas.sd.portDataList[i];

                //plot
                NationalInstruments.UI.WaveformPlot wfpPwr = null;
                wfpPwr = new NationalInstruments.UI.WaveformPlot();
                wfpPwr.LineColor = colors[i];
                wfpPwr.DefaultStart = _meas.sd.startWavelen;
                wfpPwr.DefaultIncrement = Math.Round(_meas.sd.stepWavelen, 3);
                wfpPwr.XAxis = wfgTrans.Plots[0].XAxis;
                wfpPwr.YAxis = wfgTrans.Plots[0].YAxis;
                wfpPwr.PlotY(chnlData.ilList.ToArray());
                wfgTrans.Plots.Add(wfpPwr);

            }

            wfgTrans.Refresh();

        }
        catch
        {
            //do nothing.
        }


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

            wfgTrans.ClearData();

            //color
            Color[] colors = new Color[4];
            colors[0] = System.Drawing.Color.White;
            colors[1] = System.Drawing.Color.Gold;
            colors[2] = System.Drawing.Color.Cyan;
            colors[3] = System.Drawing.Color.Tomato;



            //find chip data.
            Cmeasure meas = null;
            for (int i = 0; i < m_chipBarMsrList.Count(); i++)
            {
                try
                {
                    meas = m_chipBarMsrList[i].Find(p => p.chipNo == _chipNo);
                }
                catch
                {
                    //do nothing
                }
                
                if (meas != null)
                    break;
            }

            if (meas == null)
                return;


            //plot..
            JeffColor color = new JeffColor();
            int chnlCnt = meas.sd.portDataList.Count();
            for (int i = 0; i < chnlCnt; i++)
            {
                //channel data.
                SweepLogic.CswpPortIlNonpol chnlData = null;
                chnlData = meas.sd.portDataList[i];

                //plot
                NationalInstruments.UI.WaveformPlot wfpPwr = null;
                wfpPwr = new NationalInstruments.UI.WaveformPlot();
                wfpPwr.LineColor = colors[i];
                wfpPwr.DefaultStart = meas.sd.startWavelen;
                wfpPwr.DefaultIncrement = Math.Round(meas.sd.stepWavelen, 3);
                wfpPwr.XAxis = wfgTrans.Plots[0].XAxis;
                wfpPwr.YAxis = wfgTrans.Plots[0].YAxis;
                wfpPwr.PlotY(chnlData.ilList.ToArray());
                wfgTrans.Plots.Add(wfpPwr);

            }

            wfgTrans.Refresh();

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// measurment
    /// </summary>
    /// <param name="_gainLvls">port no. array</param>
    /// <param name="_gainLvls">gain level array </param>
    /// <param name="_chDir">channel direction</param>
    /// <param name="_eliCladPwr">eliminate clading power or not.</param>
    /// <returns></returns>
    private SweepLogic.CswpNonpol Measurement(int[] _ports,
                                             int[] _gainLvls,
                                             int _startWave,
                                             int _stopWave,
                                             double _stepWave,
                                             int _fa,
                                             int _chDir,
                                             SweepLogic.CswpRefNonpol _swpRef,
                                             bool _eliCladPwr)
    {


        SweepLogic.CswpNonpol ret = null;


        try
        {


            //---------- signal data ----------------


            //sweep 
            m_swSys.SetSweepMode(_ports, _startWave, _stopWave, _stepWave);
            log.RecordLogItem("Measurement", "SetSweepMode 완료 ");                 //LogItem

            m_swSys.ExecSweepNonpol(_ports, _gainLvls, log);
            log.RecordLogItem("Measurement", "ExecSweepNonPol 완료 ");              //LogItem

            m_swSys.StopSweepMode(_ports);
            log.RecordLogItem("Measurement", "StopSweepMode 완료 ");                //LogItem


            //aquire powerdata.
            List<SweepLogic.CswpPortPwrNonpol> signalPwrList = null;
            signalPwrList = m_swSys.GetSwpPwrDataNonpol(_ports);


            //---- clading mode power.
            List<SweepLogic.CswpPortPwrNonpol> cladPwrList = null;
            if (_eliCladPwr == true)
            {
                //x축으로 50um 이동.
                m_rightStg.RelMove(m_rightStg.AXIS_X, -mCladDeltaX);
                m_rightStg.WaitForIdle();

                //sweep
                int[] cladGainLvls = new int[1];
				if(_gainLvls.Length >= 2) cladGainLvls[0] = _gainLvls[1] + 10;//-30; //[dBm]
				else cladGainLvls[0] = _gainLvls[0];

				m_swSys.SetSweepMode(_ports, _startWave, _stopWave, _stepWave);
                log.RecordLogItem("clading mode", "SetSweepMode 완료 ");                 //LogItem
                m_swSys.ExecSweepNonpol(_ports, cladGainLvls, log);
                log.RecordLogItem("clading mode", "ExecSweepNonPol 완료 ");              //LogItem
                m_swSys.StopSweepMode(_ports);
                log.RecordLogItem("clading mode", "StopSweepMode 완료 ");                //LogItem

				//aquire powerdata.
				cladPwrList = m_swSys.GetSwpPwrDataNonpol(_ports);


				//reset gain level
				m_mpm.SetGainLevel(_ports, _gainLvls[0]);

				//return to core position
				m_rightStg.RelMove(m_rightStg.AXIS_X, mCladDeltaX);
				m_rightStg.WaitForIdle();
				
            }


           
            //------ power -> il
            //power -> insertion loss
            int dataPoint = signalPwrList[0].powList.Count();
            ret = new SweepLogic.CswpNonpol();
            ret.portDataList = new List<SweepLogic.CswpPortIlNonpol>();
            ret.startWavelen = _startWave;
            ret.stopWavelen = _stopWave;
            ret.stepWavelen = Math.Round(_stepWave, 3);
            for (int i = 0; i < signalPwrList.Count(); i++)
            {
                SweepLogic.CswpPortIlNonpol portIl = new SweepLogic.CswpPortIlNonpol();
                portIl.port = _ports[i];    //여기서에서는 channel no.
                portIl.ilList = new List<double>();
                double wavelen = (int)_startWave;
                double il = 0.0;
                double inPwr = 0.0;
                double outPwr = 0.0;
                for (int j = 0; j < dataPoint; j++)
                {

                    //input power. == ref. power.
                    try
                    {
                        inPwr = _swpRef.RefPow(_ports[i], wavelen);
                    }
                    catch
                    {
                        inPwr = 0;
                    }


                    //output power. == singal power - clading mode power.
                    try
                    {

                        if (_eliCladPwr == true)
                            outPwr = signalPwrList[i].powList[j] - cladPwrList[i].powList[j];
                        else
                            outPwr = signalPwrList[i].powList[j];
                    }
                    catch
                    {
                        outPwr = signalPwrList[i].powList[j];
                    }


                    try
                    {

                        if (outPwr > 0 )
                            il = JeffOptics.CalcTransmittance_dB(inPwr, outPwr);
                        else
                            il = -60;

                        il = Math.Round(il, 3);


                        if (il <= -60)
                            il = -60;

                    }
                    catch
                    {
                        il = -60;
                    }


                  
                    portIl.ilList.Add(il);
                    wavelen += _stepWave;
                }

                ret.portDataList.Add(portIl);
            }//for






            //----chip 채널 번호 수정 1,2,3,4...
            int chnlCnt = ret.portDataList.Count();
            if (_fa == FA_SMF)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = ret.portDataList[i].port - chnlCnt;
                }
            }


            //---칩 방향이 역방향이면 channel no.를 변경시킨다.
            //( ex 1,2,3,4 -> 4,3,2,1 )
            if (_chDir == DIRECTION_REVERSE)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = chnlCnt - i;
                }
            }


			#region === cladding mode test ===

			try
			{
				//channel list
				int[] chList = new int[ret.portDataList.Count];
				for(int i = 0; i < chList.Length; i++) chList[i] = ret.portDataList[i].port;

				//column list
				RawDataColumn cols = RawDataColumn.Ref | RawDataColumn.Total;
				if(_eliCladPwr) cols |= RawDataColumn.Clad;
				RawDataNP rawData = new RawDataNP(chList, cols);

				//add wavelength
				rawData.AddWl(_startWave, _stopWave, _stepWave);

				//add power
				for(int i = 0; i < chList.Length; i++)
				{
					rawData.Add(chList[i], RawDataColumn.Ref, _swpRef.RefPow(_ports[i]));//ref
					rawData.Add(chList[i], RawDataColumn.Total, signalPwrList[i].powList);//total
					if(_eliCladPwr) rawData.Add(chList[i], RawDataColumn.Clad, cladPwrList[i].powList);//clad
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
			}
			#endregion //~clad


		}//try
		catch(Exception ex)
        {
			MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");

			if (ret != null) ret = null;
        }


        return ret;

    }



    
    /// <summary>
    /// 다음칩으로 이동한다.
    /// lsm을 이용 1차 함수 parameter를 구하고
    /// 이를 이용하여 다음칩 위치를 추정하고 스테이지를 그 위치로 이동시킨다.
    /// </summary>
    /// <param name="_posList">aligned postion array</param>
    /// <param name="_chipWdith">chip width</param>
    /// <param name="_curIdx">현재 칩 index</param>
    private void MoveNextChip(List<CalignPos> _posList, int _chipWdith, int _curIdx)
    {

        //const int STAGEOPENDIST = 100;
        const int STAGECLOSEMARGIN = 100;
        const int ALIGNDIST = 10;       //[um]

        try
        {

			////lsm를 이용하여 1차함수 parameter를 구한다.
			////y = ax+b
			////input 쪽 좌표가 기준.
			//double ay1 = 0.0;//input y축 기울기 
			//double ay2 = 0.0;//output y축 기울기 
			//double by1 = 0.0;//input y축 절편.
			//double by2 = 0.0;//output의 y축 절편   


			//int posCnt = _posList.Count();
			//if (posCnt < 2)
			//{
			//    //--default--//
			//    ay1 = 0.0;
			//    ay2 = 0.0;
			//    by1 = _posList.Last().posIn.y;
			//    by2 = _posList.Last().posOut.y;

			//}
			//else
			//{
			//    //--lsm--//

			//    //y축
			//    double[] xPoss = new double[posCnt];
			//    double[] yPoss = new double[posCnt];
			//    for (int i = 0; i < _posList.Count(); i++)  //input.
			//    {
			//        xPoss[i] = _posList[i].posCtr.x;
			//        yPoss[i] = _posList[i].posIn.y;
			//    }
			//    JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref ay1, ref by1);


			//    for (int i = 0; i < _posList.Count(); i++)  //output.
			//    {
			//        xPoss[i] = _posList[i].posCtr.x;
			//        yPoss[i] = _posList[i].posOut.y;
			//    }
			//    JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref ay2, ref by2);
			//}



			////next chip 위치 계산. 
			//double nextPosCtrX = 0.0;
			//double nextPosInY = 0.0;
			//double nextPosOutY = 0.0;

			//double nextPosInZ = 0.0;
			//double nextPosOutZ = 0.0;

			//if (posCnt < 2)
			//{
			//    //x _ center
			//    nextPosCtrX = (int)(_posList[0].posCtr.x - (_chipWdith * (_curIdx + 1)));

			//    //y
			//    nextPosInY = (int)(ay1 * nextPosCtrX + by1);
			//    nextPosOutY = (int)(ay2 * nextPosCtrX + by2);

			//    //z
			//    nextPosInZ = _posList.Last().posIn.z - STAGECLOSEMARGIN;
			//    nextPosOutZ = _posList.Last().posOut.z - STAGECLOSEMARGIN;

			//}
			//else
			//{
			//    int preChipIdx = _posList[posCnt - 2].chipIdx;
			//    int lastChipIdx = _posList[posCnt - 1].chipIdx;
			//    int dx = (int)(_posList[posCnt - 1].posCtr.x - _posList[posCnt - 2].posCtr.x);
			//    dx = (int)(dx / (lastChipIdx - preChipIdx));
			//    dx = Math.Abs(dx);

			//    //x _ ctr
			//    nextPosCtrX = _posList[posCnt - 1].posCtr.x - (dx * (_curIdx - lastChipIdx + 1));

			//    //y
			//    nextPosInY = (int)(ay1 * nextPosCtrX + by1);
			//    nextPosOutY = (int)(ay2 * nextPosCtrX + by2);

			//    //z
			//    nextPosInZ = _posList.Last().posIn.z - STAGECLOSEMARGIN;
			//    nextPosOutZ = _posList.Last().posOut.z - STAGECLOSEMARGIN;

			//}


			//next chip 위치 계산. 
			double nextPosCtrX = 0.0;
			double nextPosInY = 0.0;
			double nextPosOutY = 0.0;
			double nextPosInZ = 0.0;
			double nextPosOutZ = 0.0;

			nextPosCtrX = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_X) - _chipWdith;
			nextPosInY = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Y);
			nextPosOutY = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Y);
			nextPosInZ = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z) - STAGECLOSEMARGIN;
			nextPosOutZ = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z) - STAGECLOSEMARGIN;


			//stage open.
			m_leftStg.AbsMove(m_leftStg.AXIS_Z, nextPosInZ);
            m_rightStg.AbsMove(m_rightStg.AXIS_Z, nextPosOutZ);


            //X축 이동
            m_ctrStg.AbsMove(m_ctrStg.AXIS_X, nextPosCtrX);


            //Y축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_Y, nextPosInY);
            m_rightStg.AbsMove(m_rightStg.AXIS_Y, nextPosOutY);


            //완료 대기.
            m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);





            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();

            //input approach.
            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //output approach.
            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.WaitForIdle();


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
            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();


            //stage open.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, STAGEOPENDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, STAGEOPENDIST * (-1));
            m_rightStg.WaitForIdle();
            if (m_stopFlag == true)
                throw new ApplicationException();


            //input approach.
            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //output approach.
            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //input ty
            align.FaArrTySingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //output ty
            align.FaArrTySingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //stage open.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, STAGEOPENDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, STAGEOPENDIST * (-1));
            m_rightStg.WaitForIdle();
            if (m_stopFlag == true)
                throw new ApplicationException();


            //input approach.
            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();

            //output approach.
            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //move to align-distance.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, ALIGNDIST * (-1));
            m_rightStg.WaitForIdle();

        }
        catch
        {
            //do nothing.
        }



    }




    /// <summary>
    /// alignment 실행.
    /// 1.input 
    /// 2.output
    /// 3.roll
    /// </summary>
    /// <param name="port1">port for channel 1</param>
    /// <param name="port2">port for channel last</param>
    /// <param name="_wavelen1">Channel1의 wavelength</param>
    /// <param name="_wavelen2">channellast의 waveleneth</param>
    /// <param name="_thresPowr">Alignment됬다고 보는 광파워. [dBm]</param>
    /// <param name="_inAlign">in align할지 말지?</param>
    /// <param name="_outAlign">out align할지 말지?</param>
    /// <param name="_outRoll">outroll 할지 말지?</param>
    /// <param name="_outRollDist">out roll distance [um]</param>
    /// <returns>광을 못찾거나 취소하면 false.</returns>
    private bool Alignment(int _port1, int _port2,
                           int _wavelen1, int _wavelen2,
                           int _thresPowr,
                           bool _inAlign, bool _outAlign, bool _outRoll,
                           int _outRollDist)
    {


		const double XYSEARCHSTEP = 1;          //[um]
		//const int SYNCSEARCHRNG = 500;		//[um]
		//const double SYNCSEARCHSTEP = 6;		//[um]
		const int ROLLRNG = 80;                 //[um]
		const int ROLLSTEP = 5;                 //[um]
		const double ROLLPOSTCOND = 2;          //[um]
		const int MOVESTAGESTEP = 50;
		const int APPROACHBUFFDIST = 40;
		const int CHIP2FADIST = 10;

		bool ret = false;

        try
        {
			//time record by DrBae 2015-10-26
			AlignTimer.RecordTime(TimingAction.AlignIn);


			double temp = -100;
            bool alignSuccess = false;


            IAlignmentDigital align = null;
            if (m_align is IAlignmentDigital)
                align = (IAlignmentDigital)m_align;
            else
                throw new ApplicationException();


			//align할 port와 wavelength 선택			[2016-11-04 ko]
			//짧은 파장으로 align한다.
			int alignPort = 0;
			double alignWavelen = 0.0;
			if (_wavelen1 < _wavelen2)
			{
				alignPort = _port1;
				alignWavelen = _wavelen1;
			}
			else
			{
				alignPort = _port2;
				alignWavelen = _wavelen2;
			}


			//Semi-Align 상태인가? (= Align가능한가?)
			m_tls.SetTlsWavelen(_wavelen1);
            temp = m_mpm.ReadPwr(alignPort);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, PWRRESDBM);
            if (temp >= _thresPowr)
                alignSuccess = true;
            else
                alignSuccess = false;



            //SyncXySearch 시도.(광을 찾은 상태가 아니면 )
            if (alignSuccess == false)
            {
                log.RecordLogItem("Alignment", "광 Power 잃음 " + temp.ToString() + "dBm");                 //LogItem

				//Stage 50㎛씩 각각 상하좌우 이동후 원위치						[2016-11-04 ko]
				m_leftStg.RelMove(m_leftStg.AXIS_Z, -1 * MOVESTAGESTEP);
				m_rightStg.RelMove(m_rightStg.AXIS_Z, -1 * MOVESTAGESTEP);

				m_leftStg.RelMove(m_leftStg.AXIS_X, -1 * MOVESTAGESTEP);
				m_leftStg.RelMove(m_leftStg.AXIS_Y, -1 * MOVESTAGESTEP);
				m_rightStg.RelMove(m_rightStg.AXIS_X, -1 * MOVESTAGESTEP);
				m_rightStg.RelMove(m_rightStg.AXIS_Y, -1 * MOVESTAGESTEP);

				m_leftStg.RelMove(m_leftStg.AXIS_X, MOVESTAGESTEP);
				m_leftStg.RelMove(m_leftStg.AXIS_Y, MOVESTAGESTEP);
				m_rightStg.RelMove(m_rightStg.AXIS_X, MOVESTAGESTEP);
				m_rightStg.RelMove(m_rightStg.AXIS_Y, MOVESTAGESTEP);

				ApproachInOut(APPROACHBUFFDIST, CHIP2FADIST);

				if (m_stopFlag == true)
					throw new ApplicationException();

				temp = m_mpm.ReadPwr(alignPort);
				temp = JeffOptics.mW2dBm(temp);
				temp = Math.Round(temp, 3);
				if (temp < _thresPowr)
				{
					ret = false;
					return ret;
				}
                else
                {
                    log.RecordLogItem("Alignment", "광 Power 다시 찾음 " + temp.ToString() + "dBm");                 //LogItem
                }

                //align.XyFullBlindSearch(alignPort, 
                //                        BLINDSEARCHINRNG,
                //                        BLINDSEARCHINSTEP,
                //                        BLINDSEARCHOUTRNG,
                //                        BLINDSEARCHOUTSTEP,
                //                        _thresPowr);

                //if (m_stopFlag == true)
                //    throw new ApplicationException();


                //temp = m_mpm.ReadPwr(alignPort);
                //temp = JeffOptics.mW2dBm(temp);
                //temp = Math.Round(temp, PWRRESDBM);
                //if (temp < _thresPowr)
                //    throw new ApplicationException();


                ////fine search out 
                //align.XySearch(m_align.STAGE_R, alignPort, XYSEARCHSTEP);

            }



            //fine search input 
            if (_inAlign == true)
            {
                m_tls.SetTlsWavelen(_wavelen1);
                align.XySearch(m_align.STAGE_L, alignPort, XYSEARCHSTEP);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }


            //fine search out 
            if (_outAlign == true)
            {
                m_tls.SetTlsWavelen(_wavelen1);
                align.XySearch(m_align.STAGE_R, alignPort, XYSEARCHSTEP);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }

            temp = m_mpm.ReadPwr(alignPort);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, 3);
            log.RecordLogItem("Alignment", "XY-FineSearch 완료 " + temp.ToString() + "dBm");                 //LogItem

            //time record by DrBae 2015-10-26
            AlignTimer.RecordTime(TimingAction.Roll);


            //roll alignment out 
            if (_outRoll == true)
            {
                m_tls.SetTlsWavelen(_wavelen1);
                align.RollOut(_port1, _port2,
                                _outRollDist,
                                m_tls,
                                _wavelen1, _wavelen2,
                                ROLLRNG, ROLLSTEP, ROLLPOSTCOND);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }

            temp = m_mpm.ReadPwr(_port2);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, 3);
            log.RecordLogItem("Alignment", "Roll 완료 " + temp.ToString() + "dBm");                          //LogItem

            ret = true;

        }
        catch
        {
            ret = false;
        }

        return ret;
    }



    

	/// <summary>
	/// alignment 실행.
	/// 1.input 
	/// 2.output
	/// 3.roll
	/// </summary>
	/// <param name="port1">port for channel 1</param>
	/// <param name="port2">port for channel last</param>
	/// <param name="_wavelen1">Channel1의 wavelength</param>
	/// <param name="_wavelen2">channellast의 waveleneth</param>
	/// <param name="_thresPowr">Alignment됬다고 보는 광파워. [dBm]</param>
	/// <param name="_inAlign">in align할지 말지?</param>
	/// <param name="_outAlign">out align할지 말지?</param>
	/// <param name="_outRoll">outroll 할지 말지?</param>
	/// <param name="_outRollDist">out roll distance [um]</param>
	/// <returns>광을 못찾거나 취소하면 false.</returns>
	private bool Alignment(int _port1, int _port2,
						   int _wavelen1, int _wavelen2,
						   int _thresPowr,
						   bool _inAlign,
						   bool _outAlign,
						   bool _outRoll,
						   int _outRollDist,
						   double _xfbInRng,
						   double _xfbInStep,
						   double _xfbOutRng,
						   double _xfbOutStep)
	{


		const double XYSEARCHSTEP = 1;    //[um]
		const int ROLLRNG = 80;           //[um]
		const int ROLLSTEP = 5;           //[um]
		const double ROLLPOSTCOND = 2;    //[um]

		bool ret = false;


		CStageAbsPos startInpos = null;
		CStageAbsPos startOutpos = null;


		try
		{
			//time record by DrBae 2015-10-26
			AlignTimer.RecordTime(TimingAction.AlignIn);

			double temp = -100;
			bool alignSuccess = false;

			IAlignmentDigital align = null;
			if (m_align is IAlignmentDigital)
				align = (IAlignmentDigital)m_align;
			else
				throw new ApplicationException();


			//start postion.
			startInpos = m_leftStg.GetAbsPositions();
			startOutpos = m_rightStg.GetAbsPositions();


			//Semi-Align 상태인가? (= Align가능한가?)
			m_tls.SetTlsWavelen(_wavelen1);
			temp = m_mpm.ReadPwr(_port1);
			temp = JeffOptics.mW2dBm(temp);
			temp = Math.Round(temp, PWRRESDBM);
			if (temp >= _thresPowr)
				alignSuccess = true;
			else
				alignSuccess = false;



			//SyncXySearch 시도.(광을 찾은 상태가 아니면 )
			if (alignSuccess == false)
			{

				align.XyFullBlindSearch(_port1,
										_xfbInRng,
										_xfbInStep,
										_xfbOutRng,
										_xfbOutStep,
										_thresPowr);

				if (m_stopFlag == true)
					throw new ApplicationException();


				temp = m_mpm.ReadPwr(_port1);
				temp = JeffOptics.mW2dBm(temp);
				temp = Math.Round(temp, PWRRESDBM);
				if (temp < _thresPowr)
					throw new ApplicationException();


				//fine search out 
				align.XySearch(m_align.STAGE_R, _port1, XYSEARCHSTEP);

			}



			//fine search input 
			if (_inAlign == true)
			{
				m_tls.SetTlsWavelen(_wavelen1);
				align.XySearch(m_align.STAGE_L, _port1, XYSEARCHSTEP);

				if (m_stopFlag == true)
					throw new ApplicationException();
			}


			//fine search out 
			if (_outAlign == true)
			{
				m_tls.SetTlsWavelen(_wavelen1);
				align.XySearch(m_align.STAGE_R, _port1, XYSEARCHSTEP);

				if (m_stopFlag == true)
					throw new ApplicationException();
			}


			//time record by DrBae 2015-10-26
			AlignTimer.RecordTime(TimingAction.Roll);


			//roll alignment out 
			if (_outRoll == true)
			{
				m_tls.SetTlsWavelen(_wavelen1);
				align.RollOut(_port1, _port2,
								_outRollDist,
								m_tls,
								_wavelen1, _wavelen2,
								ROLLRNG, ROLLSTEP, ROLLPOSTCOND);

				if (m_stopFlag == true)
					throw new ApplicationException();
			}


			ret = true;

		}
		catch
		{
			ret = false;
		}




		//if alignment fail, it comes back to start position.
		if (ret == false)
		{
			try
			{
				m_leftStg.AbsMove(m_leftStg.AXIS_X, startInpos.x);
				m_leftStg.AbsMove(m_leftStg.AXIS_Y, startInpos.y);
				m_rightStg.AbsMove(m_rightStg.AXIS_X, startOutpos.x);
				m_rightStg.AbsMove(m_rightStg.AXIS_Y, startOutpos.y);
				m_rightStg.WaitForIdle();
			}
			catch { }
		}



		return ret;
	}



    
	/// <summary>
	/// Approach in,out state.
	/// 1.open stage 
	/// 2.approach
	/// 3.open state
	/// </summary>
	/// <param name="beforeDist">approach 전</param>
	/// <param name="afterDist">approach 완료 후 (FA와 칩간의 거리) </param>
	/// <returns></returns>
	private bool ApproachInOut(int _beforeDist, int _afterDist)
    {

        bool ret = false;

        _beforeDist = Math.Abs(_beforeDist) * (-1);
        _afterDist = Math.Abs(_afterDist) * (-1);


        try
        {

            //input,out 후진 (안정상 후진후 approach 실시한다.)
            m_leftStg.RelMove(m_leftStg.AXIS_Z, _beforeDist);
            m_rightStg.RelMove(m_rightStg.AXIS_Z, _beforeDist);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);

            if (m_stopFlag == true)
                throw new ApplicationException();



            //Left stage ZApproach
            IAlignmentFa align = null;
            if (m_align is IAlignmentFa)
                align = (IAlignmentFa)m_align;
            else
                throw new ApplicationException();

            align.ZappSingleStage(m_leftStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //right stage ZApproach
            align.ZappSingleStage(m_rightStg.stageNo);
            if (m_stopFlag == true)
                throw new ApplicationException();


            //input,out 후진 (광파워 맥스될 FA와 칩간의 거리 )
            m_leftStg.RelMove(m_leftStg.AXIS_Z, _afterDist);
            m_rightStg.RelMove(m_rightStg.AXIS_Z, _afterDist);
            m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);


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
    /// Label 객체에 메세지 출력!!
    /// </summary>
    /// <param name="_lb"> label 객체 포인터</param>
    /// <param name="_msg"> 출력할 메세지</param>
    private void SetLabelMsg(System.Windows.Forms.Label _lb, string _msg)
    {
        _lb.Text = _msg;
        _lb.Refresh();
    }




    /// <summary>
    /// ToolStrip Status 에 Message 출력!!
    /// </summary>
    /// <param name="_msg">Message.</param>
    private void DisplayTssMsg(string _msg)
    {
        tsslbStatus.Text = _msg;
    }




	/// <summary>
	/// Config 설정을 불러온다.
	/// </summary>
	/// <param name="confFilepath"></param>
	private void LoadConfig(string confFilepath)
	{
		Cconfig conf = null;
		try
		{
			string strTemp = "";
			conf = new Cconfig(confFilepath);


			strTemp = conf.GetValue("GAINS");   //Gains
			if (strTemp == "1")
				rbtnGain1.Checked = true;
			else
				rbtnGain2.Checked = true;

			txtChipWidth.Text = conf.GetValue("CHIPWIDTH");
			cbCorepitch.Text = conf.GetValue("COREPITCH");

			strTemp = conf.GetValue("FA");      //FA
			if (Convert.ToInt32(strTemp) == FA_SMF)
				rbtnFA_SMF.Checked = true;
			else
				rbtnFA_MMF.Checked = true;

			strTemp = conf.GetValue("CHDIRECTION"); //channel direction
			if (Convert.ToInt32(strTemp) == DIRECTION_FORWARD)
				rbtnChDirForward.Checked = true;
			else
				rbtnChDirReverse.Checked = true;

			strTemp = conf.GetValue("SAVEFOLDERPATH");
			lbSaveFolderPath.Text = strTemp;

			strTemp = conf.GetValue("SAVERNGSTART");
			txtSaveRangeStart.Text = strTemp;

			strTemp = conf.GetValue("SAVERNGSTOP");
			txtSaveRangeStop.Text = strTemp;

			strTemp = conf.GetValue("AUTOSAVE"); //auto save
			if (strTemp == "0")
				chkAutoSave.Checked = false;
			else
				chkAutoSave.Checked = true;
			grpAutosave.Enabled = chkAutoSave.Checked;


			strTemp = conf.GetValue("XFBINRNG");    //XyFullBlind
			txtXfbInRng.Text = strTemp;

			strTemp = conf.GetValue("XFBINSTEP");
			txtXfbInStep.Text = strTemp;

			strTemp = conf.GetValue("XFBOUTRNG");
			txtXfbOutRng.Text = strTemp;

			strTemp = conf.GetValue("XFBOUTSTEP");
			txtXfbOutStep.Text = strTemp;


			strTemp = conf.GetValue("AUTOSAVEFULL");
			if (Convert.ToInt32(strTemp) == AUTOSAVE_FULL)
				rbtnAutoSaveFull.Checked = true;
			else
				rbtnAutoSaveRng.Checked = false;


			strTemp = conf.GetValue("ELIMINATECLADPWR");
			if (strTemp == "1")
				chkEliCladPwr.Checked = true;
			else
				chkEliCladPwr.Checked = false;


			strTemp = conf.GetValue("ALIGNMENT");
			if (strTemp == "1")
				chkAlignment.Checked = true;
			else
				chkAlignment.Checked = false;


			strTemp = conf.GetValue("MEASUREMENT");
			if (strTemp == "1")
				chkMeasurement.Checked = true;
			else
				chkMeasurement.Checked = false;

			strTemp = conf.GetValue("FAARRANGEMENT");
			if (strTemp == "1")
				chkFaArrangement.Checked = true;
			else
				chkFaArrangement.Checked = false;


			strTemp = conf.GetValue("ROLL");
			if (strTemp == "1")
				checkRoll.Checked = true;
			else
				checkRoll.Checked = false;


			//Bar chip distance.
			txtBarDist12xIn.Text = conf.GetValue("BARDIST1_2X"); //in
			txtBarDist12yIn.Text = conf.GetValue("BARDIST1_2Y");
			txtBarDist23xIn.Text = conf.GetValue("BARDIST2_3X");
			txtBarDist23yIn.Text = conf.GetValue("BARDIST2_3Y");
			txtBarDist34xIn.Text = conf.GetValue("BARDIST3_4X");
			txtBarDist34yIn.Text = conf.GetValue("BARDIST3_4Y");
			txtBarDist12xOut.Text = conf.GetValue("BARDIST1_2X_OUT"); //out
			txtBarDist12yOut.Text = conf.GetValue("BARDIST1_2Y_OUT");
			txtBarDist23xOut.Text = conf.GetValue("BARDIST2_3X_OUT");
			txtBarDist23yOut.Text = conf.GetValue("BARDIST2_3Y_OUT");
			txtBarDist34xOut.Text = conf.GetValue("BARDIST3_4X_OUT");
			txtBarDist34yOut.Text = conf.GetValue("BARDIST3_4Y_OUT");



			strTemp = conf.GetValue("THRESPWR"); //Thres. optical power.
			txtThresPwr.Text = strTemp;


			strTemp = conf.GetValue("CIB1"); //Chips in bar1
			txtcib1.Text = strTemp;

			strTemp = conf.GetValue("CIB2"); //Chips in bar2
			txtcib2.Text = strTemp;

			strTemp = conf.GetValue("CIB3"); //Chips in bar3
			txtcib3.Text = strTemp;

			strTemp = conf.GetValue("CIB4"); //Chips in bar4
			txtcib4.Text = strTemp;



			strTemp = conf.GetValue("CHKCHIPNO1");
			if (strTemp == "1")
				chkChip1.Checked = true;
			else
				chkChip1.Checked = false;

			strTemp = conf.GetValue("CHKCHIPNO2");
			if (strTemp == "1")
				chkChip2.Checked = true;
			else
				chkChip2.Checked = false;

			strTemp = conf.GetValue("CHKCHIPNO3");
			if (strTemp == "1")
				chkChip3.Checked = true;
			else
				chkChip3.Checked = false;

			strTemp = conf.GetValue("CHKCHIPNO4");
			if (strTemp == "1")
				chkChip4.Checked = true;
			else
				chkChip4.Checked = false;



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




	/// <summary>
	/// Config 설정을 저장한다.
	/// </summary>
	/// <param name="confFilepath"></param>
	private void SaveConfig(string confFilepath)
	{
		Cconfig conf = null;
		try
		{
			string strTemp = "";
			conf = new Cconfig(confFilepath);

			if (rbtnGain1.Checked == true)  //Gains
				strTemp = "1";
			else
				strTemp = "2";
			conf.SetValue("GAINS", strTemp);

			conf.SetValue("CHIPWIDTH", txtChipWidth.Text);
			conf.SetValue("COREPITCH", cbCorepitch.Text);

			if (rbtnFA_SMF.Checked == true)  //FA
				strTemp = FA_SMF.ToString();  //SMF
			else
				strTemp = FA_MMF.ToString();  //MMF
			conf.SetValue("FA", strTemp);

			if (rbtnChDirForward.Checked == true)  //channel direction
				strTemp = DIRECTION_FORWARD.ToString();
			else
				strTemp = DIRECTION_REVERSE.ToString();
			conf.SetValue("CHDIRECTION", strTemp);

			conf.SetValue("SAVEFOLDERPATH", lbSaveFolderPath.Text);
			conf.SetValue("SAVERNGSTART", txtSaveRangeStart.Text);
			conf.SetValue("SAVERNGSTOP", txtSaveRangeStop.Text);

			if (chkAutoSave.Checked == true)    //AutoSave
				strTemp = "1";
			else
				strTemp = "0";
			conf.SetValue("AUTOSAVE", strTemp);

			if (rbtnAutoSaveFull.Checked == true) //autoSave full or range.
				strTemp = AUTOSAVE_FULL.ToString();
			else
				strTemp = AUTOSAVE_RANGE.ToString();
			conf.SetValue("AUTOSAVEFULL", strTemp);


			conf.SetValue("BARDIST1_2X", txtBarDist12xIn.Text); //in
			conf.SetValue("BARDIST1_2Y", txtBarDist12yIn.Text);
			conf.SetValue("BARDIST2_3X", txtBarDist23xIn.Text);
			conf.SetValue("BARDIST2_3Y", txtBarDist23yIn.Text);
			conf.SetValue("BARDIST3_4X", txtBarDist34xIn.Text);
			conf.SetValue("BARDIST3_4Y", txtBarDist34yIn.Text);
			conf.SetValue("BARDIST1_2X_OUT", txtBarDist12xOut.Text); //out
			conf.SetValue("BARDIST1_2Y_OUT", txtBarDist12yOut.Text);
			conf.SetValue("BARDIST2_3X_OUT", txtBarDist23xOut.Text);
			conf.SetValue("BARDIST2_3Y_OUT", txtBarDist23yOut.Text);
			conf.SetValue("BARDIST3_4X_OUT", txtBarDist34xOut.Text);
			conf.SetValue("BARDIST3_4Y_OUT", txtBarDist34yOut.Text);


			conf.SetValue("XFBINRNG", txtXfbInRng.Text);    //XyFullBlind
			conf.SetValue("XFBINSTEP", txtXfbInStep.Text);
			conf.SetValue("XFBOUTRNG", txtXfbOutRng.Text);
			conf.SetValue("XFBOUTSTEP", txtXfbOutStep.Text);


			if (chkEliCladPwr.Checked == true)
				conf.SetValue("ELIMINATECLADPWR", "1");
			else
				conf.SetValue("ELIMINATECLADPWR", "0");

			if (chkAlignment.Checked == true)
				conf.SetValue("ALIGNMENT", "1");
			else
				conf.SetValue("ALIGNMENT", "0");

			if (chkMeasurement.Checked == true)
				conf.SetValue("MEASUREMENT", "1");
			else
				conf.SetValue("MEASUREMENT", "0");

			if (chkFaArrangement.Checked == true)
				conf.SetValue("FAARRANGEMENT", "1");
			else
				conf.SetValue("FAARRANGEMENT", "0");

			if (checkRoll.Checked == true)
				conf.SetValue("ROLL", "1");
			else
				conf.SetValue("ROLL", "0");



			conf.SetValue("THRESPWR", txtThresPwr.Text); //Thres. optical power.


			conf.SetValue("CIB1", txtcib1.Text); //Chips in bar1
			conf.SetValue("CIB2", txtcib2.Text); //Chips in bar2
			conf.SetValue("CIB3", txtcib3.Text); //Chips in bar3
			conf.SetValue("CIB4", txtcib4.Text); //Chips in bar4


			if (chkChip1.Checked == true)
				conf.SetValue("CHKCHIPNO1", "1");
			else
				conf.SetValue("CHKCHIPNO1", "0");

			if (chkChip2.Checked == true)
				conf.SetValue("CHKCHIPNO2", "1");
			else
				conf.SetValue("CHKCHIPNO2", "0");

			if (chkChip3.Checked == true)
				conf.SetValue("CHKCHIPNO3", "1");
			else
				conf.SetValue("CHKCHIPNO3", "0");

			if (chkChip4.Checked == true)
				conf.SetValue("CHKCHIPNO4", "1");
			else
				conf.SetValue("CHKCHIPNO4", "0");




			strTemp = Convert.ToString(m_procState.GetAvgProcTime()); //평균 item 처리 
			conf.SetValue("ITEMPROCESS_TIME", strTemp);


		}
		catch
		{
			MessageBox.Show("설정값을 저장하는데 실패!!",
							"에러",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
		}
		finally
		{
			if (conf != null)
				conf.Dispose();
			conf = null;
		}
	}


	#endregion



	
	#region constructor/destructor

	public frmCwdmDemuxFa()
    {
        InitializeComponent();
    }


    #endregion





    /// <summary>
    /// init form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmCwdmDemuxFa_Load(object sender, EventArgs e)
	{

		m_tls = CGlobal.g_tls;
		m_mpm = CGlobal.g_mpm;
		m_leftStg = CGlobal.g_leftStage;
		m_rightStg = CGlobal.g_rightStage;
		m_ctrStg = CGlobal.g_othStage;
		m_swSys = CGlobal.g_swSys;
		m_align = CGlobal.g_align;

		m_chipBarMsrList = new List<List<Cmeasure>>();
		m_procState = new CprogRes();


		//ref.
		m_ref = new SweepLogic.CswpRefNonpol();
		//if (!m_ref.LoadFromTxt(Application.StartupPath + "\\refNonpol.txt"))
		if (!m_ref.LoadFromTxt(CGlobal.g_refPath))
		{
			MessageBox.Show("레퍼런스 값을 불러오는데 실패!!",
							"에러",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
			m_ref = null;
		}
		this.Text += "          " + CGlobal.g_refPath;




		//option & configs.
		string confFilepath = Application.StartupPath + "\\conf_cwdmDemuxFa.xml";
		this.Location = LoadWndStartPos(confFilepath);

		LoadConfig(confFilepath);

		//edit by DrBae
		m_tls.SetTlsOutPwr(TLS_OUTPWR);
		Thread.Sleep(200);
		m_tls.SetTlsSweepSpeed(SWEEPSPEED);

		//쓰레드 가동.
		m_autoEvent = new AutoResetEvent(false);
		m_thread = new Thread(ThreadFunc);
		m_thread.Start();


	}




	/// <summary>
	/// terminate form.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void frmCwdmDemuxFa_FormClosing(object sender, FormClosingEventArgs e)
	{

		//save options and options.
		string confFilepath = Application.StartupPath + "\\conf_cwdmDemuxFa.xml";
		SaveWndStartPos(confFilepath);

		SaveConfig(confFilepath);



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




	/// <summary>
	/// chip no. array를 만들어 chip list에 추가!!
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnChipNoOk_Click(object sender, EventArgs e)
    {

        //check chip no. & cib.
        try
        {

            if ((chkChip1.Checked == false) && (chkChip2.Checked == false) &&
                (chkChip3.Checked == false) && (chkChip4.Checked == false))
            {
                throw new ApplicationException("최소 한개의 칩바는 선택되야함.");
            }


            if (chkChip1.Checked == true)
            {
                if (txtChipNo1.Text == "")
                    throw new ApplicationException("칩바1의 첫번째 칩넘버가 빠짐.");
                if (txtcib1.Text == "")
                    throw new ApplicationException("칩바1의 칩갯수가 빠짐.");
            }

            if (chkChip2.Checked == true)
            {
                if (txtChipNo2.Text == "")
                    throw new ApplicationException("칩바2의 첫번째 칩넘버가 빠짐.");
                if (txtcib2.Text == "")
                    throw new ApplicationException("칩바2의 칩갯수가 빠짐.");
            }

            if (chkChip3.Checked == true)
            {
                if (txtChipNo3.Text == "")
                    throw new ApplicationException("칩바3의 첫번째 칩넘버가 빠짐.");
                if (txtcib3.Text == "")
                    throw new ApplicationException("칩바3의 칩갯수가 빠짐.");
            }

            if (chkChip4.Checked == true)
            {
                if (txtChipNo4.Text == "")
                    throw new ApplicationException("칩바4의 첫번째 칩넘버가 빠짐.");
                if (txtcib4.Text == "")
                    throw new ApplicationException("칩바4의 칩갯수가 빠짐.");
            }


        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.ToString(),
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            throw new ApplicationException();
        }



            
        //chip list 만듬.
        string[] chipNos1 = null;   //첫번째 칩바 == 1층 == 젤 위
        int cib1 =0;
        if (chkChip1.Checked == true)
        {
            cib1 = Convert.ToInt32(txtcib1.Text);
            chipNos1 = MakeChipNos(txtChipNo1.Text, cib1);
        }

        string[] chipNos2 = null; //두번째 칩바 == 2층
        int cib2 = 0;
        if (chkChip2.Checked == true)
        {
            cib2 = Convert.ToInt32(txtcib2.Text);
            chipNos2 = MakeChipNos(txtChipNo2.Text, cib2);
        }

        string[] chipNos3 = null;   //세번째 칩바 == 3층
        int cib3 = 0;
        if (chkChip3.Checked == true)
        {
            cib3 = Convert.ToInt32(txtcib3.Text);
            chipNos3 = MakeChipNos(txtChipNo3.Text, cib3);
        }

        string[] chipNos4 = null;   //네번째 칩바 == 4층
        int cib4 = 0;
        if (chkChip4.Checked == true)
        {
            cib4 = Convert.ToInt32(txtcib4.Text);
            chipNos4 = MakeChipNos(txtChipNo4.Text, cib4);
        }



        //grid setting.
        string[] strColumArr = "no|chipNo|layer".Split('|');
        string[] strValueArr = new string[strColumArr.Length];
        hdgvChipNos.HanDefaultSetting();
        hdgvChipNos.DeleteAllRows();
        hdgvChipNos.Font = new System.Drawing.Font("Source Code Pro", 7, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        hdgvChipNos.MultiSelect = false;
        hdgvChipNos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        hdgvChipNos.ReadOnly = true;
        hdgvChipNos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        hdgvChipNos.AllowUserToOrderColumns = false;
        hdgvChipNos.AllowUserToResizeRows = false;
        hdgvChipNos.SetColumns(ref strColumArr);



        //그리드에 칩넘버 입력!!
        int listNo = 1;
        if (chipNos1 != null)
        {
            for (int i = 0; i < cib1; i++)
            {
                strValueArr[0] = Convert.ToString(listNo);   //no.
                strValueArr[1] = chipNos1[i];               //chip no.
                strValueArr[2] = "1";                         //layer
                hdgvChipNos.Insert(ref strColumArr, ref strValueArr);

                listNo++;
            }
        }

        if (chipNos2 != null)
        {
            for (int i = 0; i < cib2; i++)
            {
                strValueArr[0] = Convert.ToString(listNo);   //no.
                strValueArr[1] = chipNos2[i];               //chip no.
                strValueArr[2] = "2";                       //layer
                hdgvChipNos.Insert(ref strColumArr, ref strValueArr);

                listNo++;
            }
        }


        if (chipNos3 != null)
        {
            for (int i = 0; i < cib3; i++)
            {
                strValueArr[0] = Convert.ToString(listNo);   //no.
                strValueArr[1] = chipNos3[i];               //chip no.
                strValueArr[2] = "3";                       //layer
                hdgvChipNos.Insert(ref strColumArr, ref strValueArr);

                listNo++;
            }
        }


        if (chipNos4 != null)
        {
            for (int i = 0; i < cib4; i++)
            {
                strValueArr[0] = Convert.ToString(listNo);   //no.
                strValueArr[1] = chipNos4[i];               //chip no.
                strValueArr[2] = "4";                       //layer
                hdgvChipNos.Insert(ref strColumArr, ref strValueArr);

                listNo++;
            }
        }

        hdgvChipNos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        //time record by DrBae 2015-10-26
        AlignTimer.StartBar(txtChipNo1.Text.Trim());
    }




    /// <summary>
    /// Stop...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        if (!m_isRuning)
            return;


        //confirm?
        DialogResult dialRes;
        dialRes = MessageBox.Show("작업이 진행중입니다. 중지하시겠습니까?",
                                  "확인",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question);
        if (dialRes == DialogResult.No)
            return;


        //stop
        m_stopFlag = true;
    }




    /// <summary>
    /// stage open한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenStages_Click(object sender, EventArgs e)
    {
        const int OPENDIST = 10000;


        //opened or closed?
        if (IsStgClosed() == false)
        {
            MessageBox.Show("스테이지가 이미 열려있습니다.",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            return;
        }



        //confirm?
        DialogResult dialRes;
        dialRes = MessageBox.Show("스테이지가 열립니다. 진행하시겠습니까?",
                                    "확인",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);
        if (dialRes == DialogResult.No)
            return;



        //open stage.
        DisableWnd();

        m_leftStg.RelMove(m_leftStg.AXIS_Z, ((-1) * OPENDIST));
        m_rightStg.RelMove(m_leftStg.AXIS_Z, ((-1) * OPENDIST));
        m_rightStg.WaitForIdle();

        EnableWnd();
    }




    /// <summary>
    /// chip no.를 모두 삭제한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnDelAllChipNos_Click(object sender, EventArgs e)
    {
        hdgvChipNos.DeleteAllRows();
    }




    /// <summary>
    /// 측정 시작!!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnMeasure_Click(object sender, EventArgs e)
    {

        //측정중이면 걍 나간다.!!
        if (m_isRuning == true)
            return;


        //측정 할 칩 갯수 확인
        int chipCnt = hdgvChipNos.RowCount;
        if (chipCnt < 1)
        {
            MessageBox.Show("측정 할 칩 갯수가 0입니다.",
                                    "확인",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            return;
        }

		//TESTmode
		mTestMode = this.checkTestMode.Checked;
		mCurrentTest = int.Parse(this.textNumMeasure.Text.Trim());
		this.textNumMeasure.Tag = mCurrentTest;
		mAutoReturn = this.checkAutoReturn.Checked;
		mWlstep = int.Parse(textWlStep.Text.Trim());
		mCladDeltaX = int.Parse(this.textCladDeltaX.Text.Trim());

		//time record by DrBae 2015-10-26
		AlignTimer.RecordTime(TimingAction.Measure);

        //LogItem [2016-11-21 Ko] 
        string[] strTempArr;
        strTempArr = hdgvChipNos.Rows[0].Cells[1].Value.ToString().Split('-');
        string strWfNo = "";
        for (int i = 0; i < strTempArr.Length - 2; i++)
        {
            strWfNo += strTempArr[i] + "-";
        }
        string startChipNo = strTempArr[strTempArr.Length - 2];
        strTempArr = hdgvChipNos.Rows[hdgvChipNos.Rows.Count - 1].Cells[1].Value.ToString().Split('-');
        string endChipNo = strTempArr[strTempArr.Length - 2];
        string BarChipNo = strWfNo + startChipNo + "~" + endChipNo;

        if (log == null)
        {
            log = new LogItem(BarChipNo, this.Text);
        }
        else
        {
            log = null;
            log = new LogItem(BarChipNo, this.Text);
        }


        try
        {

            DisableWnd();


            //gain level
            m_tp.gainList = null;
            m_tp.gainList = new List<int>();
            if (rbtnGain1.Checked == true)
            {
                m_tp.gains = 1;
                m_tp.gainList.Add(GAINLEVEL1);
            }
            else
            {
                m_tp.gains = 2;
                m_tp.gainList.Add(GAINLEVEL1);
                m_tp.gainList.Add(GAINLEVEL2);
            }


            //칩 간격 , FA종류 ,  방향 , coreptich
            m_tp.chipWidth = Convert.ToInt32(txtChipWidth.Text);    //chip 간격
            m_tp.outPitch = Convert.ToInt32(cbCorepitch.Text);      //out channel coreptich.

            if (rbtnFA_SMF.Checked == true)
                m_tp.fa = FA_SMF;
            else
                m_tp.fa = FA_MMF;

            if (rbtnChDirForward.Checked == true)
                m_tp.chDirect = DIRECTION_FORWARD;
            else
                m_tp.chDirect = DIRECTION_REVERSE;



            //scan& save  range
            m_tp.saveRngStart = Convert.ToInt32(txtSaveRangeStart.Text);
            m_tp.saveRngStop = Convert.ToInt32(txtSaveRangeStop.Text);
            m_tp.saveFolderPath = lbSaveFolderPath.Text;
            m_tp.autoSave = chkAutoSave.Checked;

            if (rbtnAutoSaveFull.Checked == true)
                m_tp.autoSaveType = AUTOSAVE_FULL;
            else
                m_tp.autoSaveType = AUTOSAVE_RANGE;


            //fa Arrangement, alignment , measurment
            m_tp.elimiateCladPwr = chkEliCladPwr.Checked;
            m_tp.faArrangement = chkFaArrangement.Checked;
            m_tp.alignment = chkAlignment.Checked;
            m_tp.measurement = chkMeasurement.Checked;
            m_tp.roll = checkRoll.Checked;



            //about Multi Bar..
            List<CalignPoint2d> barChipDistList = new List<CalignPoint2d>();
            CalignPoint2d pt = new CalignPoint2d(); //1->2
            double pos = 0;
            if (double.TryParse(txtBarDist12xIn.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist12yIn.Text, out pos))
                pt.y = pos;
            barChipDistList.Add(pt);

            pt = new CalignPoint2d();   //2->3
            pos = 0;
            if (double.TryParse(txtBarDist23xIn.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist23yIn.Text, out pos))
                pt.y = pos;
            barChipDistList.Add(pt);

            pt = new CalignPoint2d();   //3->4
            pos = 0;
            if (double.TryParse(txtBarDist34xIn.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist34yIn.Text, out pos))
                pt.y = pos;
            barChipDistList.Add(pt);

            if (m_tp.barChipDistList != null)
                m_tp.barChipDistList = null;
            m_tp.barChipDistList = barChipDistList;


            List<CalignPoint2d> barChipDistOutList = new List<CalignPoint2d>();
            pt = new CalignPoint2d(); //1->2
            pos = 0;
            if (double.TryParse(txtBarDist12xOut.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist12yOut.Text, out pos))
                pt.y = pos;
            barChipDistOutList.Add(pt);

            pt = new CalignPoint2d();   //2->3
            pos = 0;
            if (double.TryParse(txtBarDist23xOut.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist23yOut.Text, out pos))
                pt.y = pos;
            barChipDistOutList.Add(pt);

            pt = new CalignPoint2d();   //3->4
            pos = 0;
            if (double.TryParse(txtBarDist34xOut.Text, out pos))
                pt.x = pos;

            if (double.TryParse(txtBarDist34yOut.Text, out pos))
                pt.y = pos;
            barChipDistOutList.Add(pt);

            if (m_tp.barChipDistOutList != null)
                m_tp.barChipDistOutList = null;
            m_tp.barChipDistOutList = barChipDistOutList;


            m_tp.thresPwr = Convert.ToDouble(txtThresPwr.Text);

			double.TryParse(txtXfbInRng.Text,out m_tp.xfbInRng);
			double.TryParse(txtXfbInStep.Text, out m_tp.xfbInStep);
			double.TryParse(txtXfbOutRng.Text, out m_tp.xfbOutRng);
			double.TryParse(txtXfbOutStep.Text, out m_tp.xfbOutStep);


			//chip numbers
			if (m_tp.chipBarList == null)
                m_tp.chipBarList = new List<List<string>>();
            else
                m_tp.chipBarList.Clear();

            List<string> chipNoList1 = new List<string>();
            List<string> chipNoList2 = new List<string>();
            List<string> chipNoList3 = new List<string>();
            List<string> chipNoList4 = new List<string>();
            int layer = 0;
            string chipNo = "";
            for (int i = 0; i < hdgvChipNos.RowCount; i++)
            {
                chipNo = (string)(hdgvChipNos.Rows[i].Cells[1].Value);
                layer = Convert.ToInt32(hdgvChipNos.Rows[i].Cells[2].Value);

                switch (layer)
                {
                    case 1:
                        chipNoList1.Add(chipNo);
                        break;
                    case 2:
                        chipNoList2.Add(chipNo);
                        break;
                    case 3:
                        chipNoList3.Add(chipNo);
                        break;
                    case 4:
                        chipNoList4.Add(chipNo);
                        break;
                }
            }

            m_tp.chipBarList.Add(chipNoList1);
            m_tp.chipBarList.Add(chipNoList2);
            m_tp.chipBarList.Add(chipNoList3);
            m_tp.chipBarList.Add(chipNoList4);


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




    private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
    {
        System.Windows.Forms.CheckBox chk = (System.Windows.Forms.CheckBox)sender;
        if (chk.Checked == true)
            grpAutosave.Enabled = true;
        else
            grpAutosave.Enabled = false;
    }
	  


	
    /// <summary>
    /// save folder path를 선택한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveFolder_Click(object sender, EventArgs e)
    {
        System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
        if (fd.ShowDialog() == DialogResult.OK)
            lbSaveFolderPath.Text = fd.SelectedPath;
    }



    
    private void chkChip1_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox cb = ((CheckBox)sender);

        if (cb.Checked == true)
        {
            txtChipNo1.Enabled = true;
            txtcib1.Enabled = true;
        }
        else
        {
            txtChipNo1.Enabled = false;
            txtcib1.Enabled = false;
        }

    }




    private void chkChip2_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox cb = ((CheckBox)sender);

        if (cb.Checked == true)
        {
            txtChipNo2.Enabled = true;
            txtcib2.Enabled = true;
        }
        else
        {
            txtChipNo2.Enabled = false;
            txtcib2.Enabled = false;
        }

    }



    
    private void chkChip3_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox cb = ((CheckBox)sender);

        if (cb.Checked == true)
        {
            txtChipNo3.Enabled = true;
            txtcib3.Enabled = true;
        }
        else
        {
            txtChipNo3.Enabled = false;
            txtcib3.Enabled = false;
        }

    }



    
    private void chkChip4_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox cb = ((CheckBox)sender);

        if (cb.Checked == true)
        {
            txtChipNo4.Enabled = true;
            txtcib4.Enabled = true;
        }
        else
        {
            txtChipNo4.Enabled = false;
            txtcib4.Enabled = false;
        }
    }




	/// <summary>
	/// 칩 데이터 출력!!
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void hdgvChipNos_CellClick(object sender, DataGridViewCellEventArgs e)
	{
		string chipNo = (string)(hdgvChipNos.SelectedRows[0].Cells[1].Value);
		Plot(chipNo);
	}




	private void hdgvChipNos_SelectionChanged(object sender, EventArgs e)
	{
        if (hdgvChipNos.CurrentRow == null) return;
        if (hdgvChipNos.CurrentRow.Index < 0) return;
        if (hdgvChipNos.SelectedRows == null) return;
        if (hdgvChipNos.SelectedRows.Count < 1) return;

        string chipNo = (string)(hdgvChipNos.SelectedRows[0].Cells[1].Value);
		Plot(chipNo);
	}


}
