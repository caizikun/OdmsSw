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
using Neon.Aligner;



public partial class frmLwdmFa : Form
{


    #region definition

    private const int SWEEPRNG_START = 1260;        //start sweep wavelength [nm]
    private const int SWEEPRNG_STOP = 1360;         //stop sweep wavelength [nm]
    private const double SWEEPRNG_STEP = 0.05;      //step sweep wavelength [nm]

    private const int WAVELEN_CH1 = 1295;           //1295.56 [nm/s]
    private const int WAVELEN_CH2 = 1300;           //1300.05 [nm/s]
    private const int WAVELEN_CH3 = 1305;           //1304.58 [nm/s]
    private const int WAVELEN_CH4 = 1309;           //1309.14 [nm/s]

    private int GAINLEVEL1 = CGlobal.PmGain[0];		//[dBm]
    private int GAINLEVEL2 = CGlobal.PmGain[1];		//[dBm]
    private double TLS_OUTPWR = CGlobal.TlsParam.Power;  //[dBm]
   
    private const int FA_SMF = 0;
    private const int FA_MMF = 1;
    private const int DIRECTION_FORWARD = 0;        //ex)1271->1291->1311->1331
    private const int DIRECTION_REVERSE = 1;        //ex)1331->1311->1291->1271
    private const int AUTOSAVE_FULL = 0;
    private const int AUTOSAVE_RANGE = 1;

    private const int CMD_RUN = 0;
    private const int CMD_GOINITPOS = 1;
    private const int CMD_OPENSTAGES = 2;
    private const int CMD_CLOSESTAGES = 3;


    #endregion




    #region structure/innor class


    private struct measureParam
    {
        public int[] ports;
        public int[] gainLvls;
        public double startWave;
        public double stopWave;
        public double stepWave;
        public int faKind;
        public int chDir;
        public bool eliCladPwr;
        public double cladDist;
        public SweepLogic.CswpRefNonpol swpRef;
    }




    private struct alignParam
    {
        public int port1;
        public int port2;
        public double wavelen1;					//[nm]
        public double wavelen2;					//[nm]
        public double thresPwr;					//[dBm]
        public bool searchIn;
        public bool searchOut;
        public bool rollOut;
        public double searchStep;				//[um]
        public double rollPitch;				//[um]
        public double rollRng;					//[um]
        public double rollStep;					//[um]
        public double rollpostcond;				//[um]
        public double syncRng;					//[um]
        public double syncStep;					//[um]
    }




    private struct threadParam
    {
        public int cmd;
        public string[] chipNos;
        public int gains;						//number of gains. 
        public List<int> gainList;				//
        public int chipWidth;					//칩 간 간격
        public int outPitch;					//output FA corepitch [um]     
        public int fa;							//SMF or MMF
        public int chDirect;					//channel direction

        public bool autoSave;
        public int autoSaveType;				//full or range.
        public int saveRngStart;				//save range start wavelengh.
        public int saveRngStop;					//save range stop wavelengh.

        public bool elimiateCladPwr;			//clading mode power 제거??
        public double cladDist;
        public bool alignment;					//alignment. <-- uncheck하면 1칩만 측정됨.
        public bool measurement;				//measurement.?
        public bool faArrangement;				//FA arrangement?
        public bool roll;
        public bool retChip1Pos;				//자동으로 First chip으로 이동한다.


        public string saveFolderPath;
    }




    //private class CalignPos
    //{
    //    public int chipIdx;
    //    public CStageAbsPos posIn;
    //    public CStageAbsPos posOut;
    //    public CStageAbsPos posCtr;
    //}




    private class CalignPos
    {
        public int chipIdx;
        public CStgsPos stgsPos;
    }




    private class CStgsPos : ICloneable
    {
        public CStageAbsPos posIn;
        public CStageAbsPos posOut;
        public CStageAbsPos posCtr;

        public object Clone()
        {
            CStgsPos ret = null; 

            try
            {
                ret = new CStgsPos();
                ret.posIn = new CStageAbsPos();
                ret.posOut = new CStageAbsPos();
                ret.posCtr = new CStageAbsPos();

                ret.posIn.x = posIn.x;		//input 
                ret.posIn.y = posIn.y;
                ret.posIn.z = posIn.z;
                ret.posIn.tx = posIn.tx;
                ret.posIn.ty = posIn.ty;
                ret.posIn.tz = posIn.tz;
                ret.posOut.x = posOut.x;    //output
                ret.posOut.y = posOut.y;
                ret.posOut.z = posOut.z;
                ret.posOut.tx = posOut.tx;
                ret.posOut.ty = posOut.ty;
                ret.posOut.tz = posOut.tz;
                ret.posCtr.x = posCtr.x;    //center
                ret.posCtr.y = posCtr.y;
                ret.posCtr.z = posCtr.z;
                ret.posCtr.tx = posCtr.tx;
                ret.posCtr.ty = posCtr.ty;
                ret.posCtr.tz = posCtr.tz;

            }
            catch
            {
                ret = null;
            }
            
            return ret;
        }


    }




    private struct closedPos
    {
        public double inPos;				//input Z축 위치.
        public double outPos;				//output z축 위치.
    }




    private class Cmeasure
    {
        public string chipNo;
        public DateTime msrTime;			//측정 시간.
        public CalignPos pos;				//aligned position.
        public SweepLogic.CswpNonpol sd;
        //public analy 
    }



    #endregion

    


    #region member variables


    private SweepLogic.CswpRefNonpol m_ref;

    private IdistSensor m_distSens;
    private Itls m_tls;
    private IoptMultimeter m_mpm;
    private Istage m_leftStg;
    private Istage m_rightStg;
    private Istage m_ctrStg;	//center stage.

    private SweepLogic m_swSys;
    private IAlignment m_align;


    bool m_stopFlag;
    bool m_isRuning;			//running:true , stop :false
    private threadParam m_tp;
    private AutoResetEvent m_autoEvent;
    private Thread m_thread;
    private LogItem log;

    private CprogRes m_procState;
    private List<Cmeasure> m_msrList;

    private CStgsPos m_initPos;
    private closedPos m_closedPos;

    #endregion
	



    #region thread function

	
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


            //do work.
            switch (m_tp.cmd)
            {
                case CMD_RUN:
                    Run();
                    break;

                case CMD_GOINITPOS:
                    MoveToInitPos();
                    break;

                case CMD_OPENSTAGES:
                    OpenStages();
                    break;

                case CMD_CLOSESTAGES:
                    CloseStages();
                    break;
            }



            //화면 활성화!!
            Invoke(ew);


        }//while (true)


    }






	#endregion




	#region constructor/destructor

	public frmLwdmFa()
	{
		InitializeComponent();
	}


	#endregion




	#region private method



	/// <summary>
	/// 현재의 모든 스테이지의 position들을 얻는다.
	/// </summary>
	/// <returns></returns>
	private CStgsPos GetStgsPos()
    {
        CStgsPos stgsPos = new CStgsPos();

        CStageAbsPos pos = new CStageAbsPos();  //input.
        try
        {
            pos.x = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_X);
            pos.y = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Y);
            pos.z = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
            pos.tx = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TX);
            pos.ty = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TY);
            pos.tz = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_TZ);
        }
        catch { }
        stgsPos.posIn = pos;

        pos = new CStageAbsPos();  //output.
        try
        {
            pos.x = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_X);
            pos.y = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Y);
            pos.z = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);
            pos.tx = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_TX);
            pos.ty = m_rightStg.GetAxisAbsPos(m_leftStg.AXIS_TY);
            pos.tz = m_rightStg.GetAxisAbsPos(m_leftStg.AXIS_TZ);
        }
        catch { }
        stgsPos.posOut = pos;

        pos = new CStageAbsPos();  //center.
        try
        {
            pos.x = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_X);
            pos.y = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_Y);
            pos.z = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_Z);
            pos.tx = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_TX);
            pos.ty = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_TY);
            pos.tz = m_ctrStg.GetAxisAbsPos(m_ctrStg.AXIS_TZ);
        }
        catch { }
        stgsPos.posCtr = pos;

        return stgsPos;
    }




    /// <summary>
    /// 작업을 실행한다.
    /// </summary>
    private void Run()
    {

        const int APPROACHBUFFDIST = 40;    //[um]    
        const int CHIP2FADIST = 10;         //[um]

        const double ALINGNTHRESPWR = -25;

        const double XYFINESEARCHSTEP = 1;
        const double ROLLRNG_MMF = 90;
        const double ROLLRNG_SMF = 20;
        const double ROLLSTEP_MMF = 5;
        const double ROLLSTEP_SMF = 1;
        const double ROLLPOSTCOND_MMF = 3;
        const double ROLLPOSTCOND_SMF = 0.3;
        const double SYNCRNG = 60;
        const double SYNCSTEP = 4;


        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<string> dsi = DisplayShortInfor;
        Action<string> pca = Plot;
        Action spw = ShowProgressWnd;
        Action uclps = UpdateChipListProgState;

        frmDistSensViewer frmDistSens = null;
        frmDigitalOptPowermeter frmDigitalPwr = null;
        frmStageControl frmStageCont = null;
        frmAlignStatus frmStatus = null;
        frmSourceController frmSourCont = null;

        CStgsPos beginPos = null;
        CStgsPos retPos = null;
        List<CalignPos> alignPosList = new List<CalignPos>();

        JeffTimer jTimer = new JeffTimer();



        //저장공간 초기화.
        m_msrList.Clear();


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



        //process state 초기화.
        m_procState.Clear();
        m_procState.compeleted = false;
        m_procState.totalItemCnt = m_tp.chipNos.Length;
        m_procState.startTime = DateTime.Now;
        Invoke(spw);


        //update chip list process state
        Invoke(uclps);


        //chip 방향(칩의채널 방향)에 따른 파장 설정.
        int[] wavelens = new int[4];
        if (m_tp.chDirect == DIRECTION_FORWARD)
        {
            wavelens[0] = WAVELEN_CH1; //정방향.
            wavelens[1] = WAVELEN_CH2;
            wavelens[2] = WAVELEN_CH3;
            wavelens[3] = WAVELEN_CH4;
        }
        else
        {
            wavelens[0] = WAVELEN_CH4; //역방향.
            wavelens[1] = WAVELEN_CH3;
            wavelens[2] = WAVELEN_CH2;
            wavelens[3] = WAVELEN_CH1;
        }


        //FA 종류에 따른 Detector port 
        int[] ports = new int[4];
        if (m_tp.fa == FA_SMF)
        {
            ports[0] = 5;
            ports[1] = 6;
            ports[2] = 7;
            ports[3] = 8;
        }
        else
        {
            ports[0] = 1;
            ports[1] = 2;
            ports[2] = 3;
            ports[3] = 4;
        }


        //시작 및 복귀 위치 
        beginPos = GetStgsPos();
        retPos = (CStgsPos)beginPos.Clone();
        alignPosList.Clear();


        //Disable optical source controller
        if (frmSourCont != null)
            frmSourCont.DisableForm();

        
        //alignment + 측정 + 다음칩 이동
        string chipNo = "";
        for (int i = 0; i < m_tp.chipNos.Length; i++)
        {

            //칩측정 시간을 알아내기 위해~~ 타이머 시작!!
            jTimer.ResetTimer();
            jTimer.StartTimer();


            //chip no.
            chipNo = m_tp.chipNos[i];
            m_procState.curItemNo = chipNo;

            log.RecordLogItem(chipNo, "측정시작");                 //LogItem

            //FA Arrangement.
            if ((i == 0) && (m_tp.faArrangement == true))
                FaArrangement();


            //Approach
            if ((i == 0) && (m_tp.faArrangement != true) && (m_tp.alignment == true))
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


                alignParam ap = new alignParam();
                ap.port1 = ports[0];
                ap.port2 = ports[3];
                ap.wavelen1 = wavelens[0];
                ap.wavelen2 = wavelens[3];
                ap.thresPwr = ALINGNTHRESPWR;

                ap.searchStep = XYFINESEARCHSTEP;
                ap.searchIn = true;
                if ( (i == 0) || (m_tp.roll == true) )
                {
                    ap.searchOut = false;
                    ap.rollOut = true;
                }
                else
                {
                    ap.searchOut = true;
                    ap.rollOut = false;
                }
                
                ap.rollPitch = m_tp.outPitch * (ports.Length - 1);
                if (m_tp.fa == FA_SMF)
                {
                    ap.rollRng = ROLLRNG_SMF;
                    ap.rollStep = ROLLSTEP_SMF;
                    ap.rollpostcond = ROLLPOSTCOND_SMF;
                }
                else
                {
                    ap.rollRng = ROLLRNG_MMF;
                    ap.rollStep = ROLLSTEP_MMF;
                    ap.rollpostcond = ROLLPOSTCOND_MMF;
                }

                ap.syncRng = SYNCRNG;
                ap.syncStep = SYNCSTEP;

                alignSuccess = Alignment(ap);


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



            //포지션 저장.
            if (alignSuccess == true)
            {
                CalignPos alignPos = new CalignPos();
                alignPos.chipIdx = i;
                alignPos.stgsPos = GetStgsPos();
                alignPosList.Add(alignPos);

                log.mPosIn = alignPos.stgsPos.posIn;
                log.mPosOut = alignPos.stgsPos.posOut;
                log.mPosCenter = alignPos.stgsPos.posCtr;
                log.RecordLogItem(AlignState.AlignPass);                 //LogItem

                if ((alignSuccess == true) && (i == 0))
                    retPos = GetStgsPos();
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



            //measurement. 
            if (alignSuccess == true)
            {

                Cmeasure meas = new Cmeasure();
                meas.chipNo = m_tp.chipNos[i];
                meas.msrTime = DateTime.Now;
                meas.pos = alignPosList.Last();

                if (m_tp.measurement == true)
                {

                    Invoke(dsi, "measurment");

                    //display off.
                    if (frmDigitalPwr != null)
                        frmDigitalPwr.DisplayOff();

                    //measurement
                    measureParam mp = new measureParam();
                    mp.ports = ports;
                    mp.gainLvls = m_tp.gainList.ToArray();
                    mp.startWave = SWEEPRNG_START;
                    mp.stopWave = SWEEPRNG_STOP;
                    mp.stepWave = SWEEPRNG_STEP;
                    mp.faKind = m_tp.fa;
                    mp.chDir = m_tp.chDirect;
                    mp.eliCladPwr = m_tp.elimiateCladPwr;
                    mp.cladDist = m_tp.cladDist;
                    mp.swpRef = m_ref;
                    meas.sd = Measurement(mp);

                    //display off.
                    if (frmDigitalPwr != null)
                        frmDigitalPwr.DisplayOn();


                    //save
                    if (m_tp.autoSave == true)
                    {
                        Invoke(dsi, "save data.");

                        string filePath = m_tp.saveFolderPath + "\\";
                        filePath += meas.chipNo + ".txt";
                        while (System.IO.File.Exists(filePath))
                        {
                            filePath = filePath.Replace(".txt", "");
                            filePath += "_copied.txt";
                        }

                        if (m_tp.autoSaveType == AUTOSAVE_FULL)
                            meas.sd.SaveToTxt(filePath);
                        else
                            meas.sd.SaveToTxt(filePath,
                                                m_tp.saveRngStart,
                                                m_tp.saveRngStop);
                    }


                } //m_tp.measurement

                m_msrList.Add(meas);

            }


            //plot & update chip list progres state.
            Invoke(dsi, "plot data.");
            Invoke(pca, m_tp.chipNos[i]);
            Invoke(uclps);


            if (m_stopFlag == true)
                break;

            if (m_tp.alignment == false)
                break;



            //move to next chip
            if (i != (m_tp.chipNos.Length - 1))
            {
                Invoke(dsi, "move next chip.");
                MoveNextChip(alignPosList, m_tp.chipWidth, i);
            }

            if (m_stopFlag == true)
                break;


            //time 측정 끝!!
            jTimer.StopTimer();
            m_procState.SetItemProcTime(jTimer.GetLeadTime().TotalSeconds);

        }


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
            dialRes = MessageBox.Show(msg,
                                        "확인",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);
            if (dialRes == DialogResult.Yes)
                MoveTo(retPos);
        }
        else
        {

            m_procState.msg = "Process has completed!!";
            m_procState.endTime = DateTime.Now;
            m_procState.compeleted = true;

            Invoke(dsi, "측정 완료!!");


            //초기 위치로 이동(?)
            if (m_tp.retChip1Pos == true)
            {
                MoveTo(retPos);
            }
            else
            {
                string msg = "작업이 완료되었습니다. \n";
                msg += "초기 위치로 이동(Yes), 멈춤(No)";
                DialogResult dialRes = DialogResult.No;
                dialRes = MessageBox.Show(msg,
                                            "확인",
                                            MessageBoxButtons.YesNo,
                                            MessageBoxIcon.Question);
                if (dialRes == DialogResult.Yes)
                    MoveTo(retPos);
            }

        }


        //tls 파장 변경.
        m_tls.SetTlsWavelen(wavelens[0]);


        //Enable optical source controller
        if (frmSourCont != null)
            frmSourCont.EnableForm();

        log.RecordLogItem("Measurement", "Bar 측정 종료");                 //LogItem
        log = null;

    }




    /// <summary>
    /// Stage들을 연다.
    /// </summary>
    private void OpenStages()
    {

        const int OPENDIST = 20000;

        //이동.
        m_leftStg.AbsMove(m_leftStg.AXIS_Z, (m_closedPos.inPos - OPENDIST) );
        m_rightStg.AbsMove(m_rightStg.AXIS_Z, (m_closedPos.outPos - OPENDIST));
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




    /// <summary>
    /// Stage들을 닫는다.
    /// </summary>
    private void CloseStages()
    {

        const double DISTSENSTHRES = 3.0;


        //이동.
        m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_closedPos.inPos );
        m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_closedPos.outPos);

        double distSens1 = 0.0;
        double distSens2 = 0.0;
        while (m_rightStg.IsMovingOK())
        {

            distSens1 = m_distSens.ReadDist(1);
            distSens2 = m_distSens.ReadDist(2);
            if ((distSens1 <= DISTSENSTHRES) || 
                (distSens2 <= DISTSENSTHRES))
            {
                m_leftStg.StopMove();
                m_rightStg.StopMove();
                break;
            }


            if (m_stopFlag == true)
            {
                m_leftStg.StopMove();
                m_rightStg.StopMove();
                break;
            }; 

        }


    }




    /// <summary>
    /// Initial position으로 이동한다.
    /// </summary>
    private void MoveToInitPos()
    {

        const int OPENDIST = 10000;

        //open stage.
        m_leftStg.RelMove(OPENDIST, m_initPos.posIn.z);
        m_rightStg.RelMove(OPENDIST, m_initPos.posOut.z);
        m_rightStg.WaitForIdle();


        //move to init.pos.
        m_ctrStg.AbsMove(m_ctrStg.AXIS_X, m_initPos.posCtr.x); //ctr. x-axis
        m_leftStg.AbsMove(m_ctrStg.AXIS_X, m_initPos.posIn.x); //x-axis
        m_rightStg.AbsMove(m_ctrStg.AXIS_X, m_initPos.posOut.x);
        m_leftStg.AbsMove(m_leftStg.AXIS_Y, m_initPos.posIn.y); //y-axis
        m_rightStg.AbsMove(m_rightStg.AXIS_Y, m_initPos.posOut.y);
        m_leftStg.AbsMove(m_leftStg.AXIS_TX, m_initPos.posIn.tx); //tx-axis
        m_rightStg.AbsMove(m_rightStg.AXIS_TX, m_initPos.posOut.tx);
        m_leftStg.AbsMove(m_leftStg.AXIS_TY, m_initPos.posIn.ty); //ty-axis
        m_rightStg.AbsMove(m_rightStg.AXIS_TY, m_initPos.posOut.ty);
        m_leftStg.AbsMove(m_leftStg.AXIS_TZ, m_initPos.posIn.tz); //tz-axis   
        m_rightStg.AbsMove(m_rightStg.AXIS_TZ, m_initPos.posOut.tz);
        m_rightStg.WaitForIdle();

        //close stage
        //m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_initPos.posIn.z);
        //m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_initPos.posOut.z);
        //m_rightStg.WaitForIdle(m_rightStg.AXIS_Z);

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




    /// <summary>
    /// load init pos. from xml file.
    /// </summary>
    /// <param name="_filepath">config file path</param>
    /// <returns></returns>
    private CStgsPos LoadInitPos(string _filepath)
    {
        CStgsPos ret = null;
        Cconfig conf = null;

        try
        {

            ret = new CStgsPos();
            ret.posIn = new CStageAbsPos();
            ret.posOut = new CStageAbsPos();
            ret.posCtr = new CStageAbsPos();
            conf = new Cconfig(_filepath);

            //in
            string strTemp = "";
            strTemp = conf.GetValue("INITPOS_IN_X");
            ret.posIn.x = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_IN_Y");
            ret.posIn.y = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_IN_Z");
            ret.posIn.z = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_IN_TX");
            ret.posIn.tx = Convert.ToDouble(strTemp);
            ret.posIn.tx = Math.Round(ret.posIn.tx, 4);
            strTemp = conf.GetValue("INITPOS_IN_TY");
            ret.posIn.ty = Convert.ToDouble(strTemp);
            ret.posIn.ty = Math.Round(ret.posIn.ty, 4);
            strTemp = conf.GetValue("INITPOS_IN_TZ");
            ret.posIn.tz = Convert.ToDouble(strTemp);
            ret.posIn.tz = Math.Round(ret.posIn.tz, 4);

            //out
            strTemp = conf.GetValue("INITPOS_OUT_X");
            ret.posOut.x = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_OUT_Y");
            ret.posOut.y = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_OUT_Z");
            ret.posOut.z = Convert.ToInt32(strTemp);
            strTemp = conf.GetValue("INITPOS_OUT_TX");
            ret.posOut.tx = Convert.ToDouble(strTemp);
            ret.posOut.tx = Math.Round(ret.posOut.tx, 4);
            strTemp = conf.GetValue("INITPOS_OUT_TY");
            ret.posOut.ty = Convert.ToDouble(strTemp);
            ret.posOut.ty = Math.Round(ret.posOut.ty, 4);
            strTemp = conf.GetValue("INITPOS_OUT_TZ");
            ret.posOut.tz = Convert.ToDouble(strTemp);
            ret.posOut.tz = Math.Round(ret.posOut.tz, 4);

            //center
            strTemp = conf.GetValue("INITPOS_CTR_X");
            ret.posCtr.x = Convert.ToInt32(strTemp);

        }
        catch
        {
            
        }

        if (conf != null)
            conf.Dispose();
        conf = null;

        return ret;

    }

	


    /// <summary>
    /// load close-pos. from xml file.
    /// </summary>
    /// <param name="_filepath">config file path</param>
    /// <returns></returns>
    private closedPos LoadClosePos(string _filepath)
    {

        closedPos ret = new closedPos();
        Cconfig conf = null;

        try
        {

            conf = new Cconfig(_filepath);

            //in
            string strTemp = "";
            strTemp = conf.GetValue("CLOSEPOS_IN");
            ret.inPos = Convert.ToInt32(strTemp);


            //out
            strTemp = conf.GetValue("CLOSEPOS_OUT");
            ret.outPos = Convert.ToInt32(strTemp);

        }
        catch
        {
            ret.inPos = 0;
            ret.outPos = 0;
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
        
        lbInitPosInX.Text = m_initPos.posIn.x.ToString();   //in
        lbInitPosInY.Text = m_initPos.posIn.y.ToString();
        lbInitPosInZ.Text = m_initPos.posIn.z.ToString();
        lbInitPosInTx.Text = m_initPos.posIn.tx.ToString();
        lbInitPosInTy.Text = m_initPos.posIn.ty.ToString();
        lbInitPosInTz.Text = m_initPos.posIn.tz.ToString();

        lbInitPosOutX.Text = m_initPos.posOut.x.ToString(); //out
        lbInitPosOutY.Text = m_initPos.posOut.y.ToString();
        lbInitPosOutZ.Text = m_initPos.posOut.z.ToString();
        lbInitPosOutTx.Text = m_initPos.posOut.tx.ToString();
        lbInitPosOutTy.Text = m_initPos.posOut.ty.ToString();
        lbInitPosOutTz.Text = m_initPos.posOut.tz.ToString();
        
        lbInitPosCtrX.Text = m_initPos.posCtr.x.ToString(); //center

    }




    /// <summary>
    /// Close position을 출력한다.
    /// </summary>
    private void DisplayClosePos()
    {
        lbClosePosInZ.Text = m_closedPos.inPos.ToString();
        lbClosePosOutZ.Text = m_closedPos.outPos.ToString();
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

        Cconfig conf = null;
        try

        {
            conf = new Cconfig(_filepath);

            //in
            conf.SetValue("INITPOS_IN_X", m_initPos.posIn.x.ToString());
            conf.SetValue("INITPOS_IN_Y", m_initPos.posIn.y.ToString());
            conf.SetValue("INITPOS_IN_Z", m_initPos.posIn.z.ToString());
            conf.SetValue("INITPOS_IN_TX", m_initPos.posIn.tx.ToString());
            conf.SetValue("INITPOS_IN_TY", m_initPos.posIn.ty.ToString());
            conf.SetValue("INITPOS_IN_TZ", m_initPos.posIn.tz.ToString());

            //out
            conf.SetValue("INITPOS_OUT_X", m_initPos.posOut.x.ToString());
            conf.SetValue("INITPOS_OUT_Y", m_initPos.posOut.y.ToString());
            conf.SetValue("INITPOS_OUT_Z", m_initPos.posOut.z.ToString());
            conf.SetValue("INITPOS_OUT_TX", m_initPos.posOut.tx.ToString());
            conf.SetValue("INITPOS_OUT_TY", m_initPos.posOut.ty.ToString());
            conf.SetValue("INITPOS_OUT_TZ", m_initPos.posOut.tz.ToString());

            //out
            conf.SetValue("INITPOS_CTR_X", m_initPos.posCtr.x.ToString());

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
    /// close-position을 config파일에 저장.
    /// </summary>
    /// <param name="_filepath"></param>
    /// <returns></returns>
    private bool SaveClosePos(string _filepath)
    {
        bool ret = false;

        Cconfig conf = null;
        try

        {
            conf = new Cconfig(_filepath);

            conf.SetValue("CLOSEPOS_IN", m_closedPos.inPos.ToString());
            conf.SetValue("CLOSEPOS_OUT", m_closedPos.outPos.ToString());

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
                for (int i = 0; i < hdgvChipNos.RowCount; i++)
                {

                    chipNo = hdgvChipNos.Rows[i].Cells[1].Value.ToString();

                    if (null != m_msrList.Find(p => p.chipNo == chipNo))
                    {
                        for (int j = 0; j < hdgvChipNos.Rows[i].Cells.Count; j++)
                        {
                            cell = hdgvChipNos.Rows[i].Cells[j];
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
    private void MoveTo(CStgsPos _stgPos)
    {
        const int STAGEOPENDIST = 10000;

        if (m_msrList.Count() == 0)
            return;

        try
        {
            //stage open.
            m_leftStg.RelMove(m_leftStg.AXIS_Z, STAGEOPENDIST * (-1));
            m_rightStg.RelMove(m_rightStg.AXIS_Z, STAGEOPENDIST * (-1));

            //move
            m_ctrStg.AbsMove(m_ctrStg.AXIS_X, _stgPos.posCtr.x);   //center X축 이동
            m_leftStg.AbsMove(m_leftStg.AXIS_Y, _stgPos.posIn.y);   //Y축 이동
            m_rightStg.AbsMove(m_rightStg.AXIS_Y, _stgPos.posOut.y);
            m_rightStg.AbsMove(m_rightStg.AXIS_TZ, _stgPos.posOut.tz); //tz 
            m_leftStg.AbsMove(m_leftStg.AXIS_X, _stgPos.posIn.x);   //x-axis.
            m_rightStg.AbsMove(m_rightStg.AXIS_X, _stgPos.posOut.x);
            m_rightStg.WaitForIdle();

            //stage colse
            m_leftStg.AbsMove(m_leftStg.AXIS_Z, (_stgPos.posIn.z - STAGEOPENDIST));
            m_rightStg.AbsMove(m_rightStg.AXIS_Z, (_stgPos.posOut.z - STAGEOPENDIST));
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
            meas = m_msrList.Find(p => p.chipNo == _chipNo);

            //find channel data.
            SweepLogic.CswpPortIlNonpol chnlData = null;
            chnlData = meas.sd.portDataList.Find(p => p.port == _chnlNo);

            

            //plot..
            NationalInstruments.UI.WaveformPlot wfpPwr = null;
            wfpPwr = new NationalInstruments.UI.WaveformPlot();
            wfpPwr.XAxis = wfgTrans.Plots[0].XAxis;
            wfpPwr.YAxis = wfgTrans.Plots[0].YAxis;
            wfpPwr.LineColor = System.Drawing.Color.White;
            wfpPwr.DefaultStart =  meas.sd.startWavelen ;
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
            meas = m_msrList.Find(p => p.chipNo == _chipNo);

            //plot..
            JeffColor color = new JeffColor();
            int chnlCnt = meas.sd.portDataList.Count();
            for (int i = 0; i < chnlCnt; i++)
            {
                //channel data.
                SweepLogic.CswpPortIlNonpol chnlData = null;
                chnlData = meas.sd.portDataList.Find(p => p.port == (i+1));

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
    /// <returns></returns>
    private SweepLogic.CswpNonpol Measurement(int[] _ports, 
                                             int[] _gainLvls,
                                             int _startWave,
                                             int _stopWave,
                                             double _stepWave,
                                             int _fa,
                                             int _chDir,
                                             SweepLogic.CswpRefNonpol _swpRef)
    {


        SweepLogic.CswpNonpol ret = null;

 
        try
        {
            //sweep 
            m_swSys.SetSweepMode(_ports, _startWave, _stopWave, _stepWave);
            m_swSys.ExecSweepNonpol(_ports, _gainLvls);
            m_swSys.StopSweepMode(_ports);


            //aquire powerdata.
            List<SweepLogic.CswpPortPwrNonpol> pwrList = null;
            pwrList = m_swSys.GetSwpPwrDataNonpol(_ports);


            //power -> insertion loss
            int dataPoint = pwrList[0].powList.Count();
            ret = new SweepLogic.CswpNonpol();
            ret.portDataList = new List<SweepLogic.CswpPortIlNonpol>();
            ret.startWavelen = _startWave;
            ret.stopWavelen = _stopWave;
            ret.stepWavelen = Math.Round(_stepWave, 3);
            for (int i = 0; i < pwrList.Count(); i++)
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

                    try
                    {
                        inPwr = _swpRef.RefPow(_ports[i], wavelen);
                    }
                    catch
                    {
                        inPwr = 0;
                    }

                    outPwr = pwrList[i].powList[j];

                    try
                    {
                        il = JeffOptics.CalcTransmittance_dB(inPwr, outPwr);
                        il = Math.Round(il, 3);
                        if (il <= -80)
                            il = -80;
                    }
                    catch
                    {
                        il = -80;
                    }
                    portIl.ilList.Add(il);
                    wavelen += _stepWave;
                }

                ret.portDataList.Add(portIl);
            }


            //chip 채널 번호 수정 1,2,3,4...
            int chnlCnt = ret.portDataList.Count();
            if (_fa == FA_SMF)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = ret.portDataList[i].port - chnlCnt;
                }
            }


            //칩 방향이 역방향이면 channel no.를 변경시킨다.
            //( ex 1,2,3,4 -> 4,3,2,1 )
            if (_chDir == DIRECTION_REVERSE)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = chnlCnt -i;
                }
            }


        }
        catch
        {
            if (ret != null)
                ret = null;
        }


        return ret;

    }




    /// <summary>
    /// measurment
    /// </summary>
    /// <param name="_mp">measurement parameter</param>
    /// <returns></returns>
    private SweepLogic.CswpNonpol Measurement(measureParam _mp)
    {

        SweepLogic.CswpNonpol ret = null;

        try
        {
            //sweep 
            m_swSys.SetSweepMode(_mp.ports, (int)_mp.startWave, (int)_mp.stopWave, _mp.stepWave);
            log.RecordLogItem("Measurement", "SetSweepMode 완료 ");                 //LogItem

            m_swSys.ExecSweepNonpol(_mp.ports, _mp.gainLvls, log);
            log.RecordLogItem("Measurement", "ExecSweepNonPol 완료 ");              //LogItem

            m_swSys.StopSweepMode(_mp.ports);
            log.RecordLogItem("Measurement", "StopSweepMode 완료 ");                //LogItem


            //aquire powerdata.
            List<SweepLogic.CswpPortPwrNonpol> pwrList = null;
            pwrList = m_swSys.GetSwpPwrDataNonpol(_mp.ports);


            //---- clading mode power.
            List<SweepLogic.CswpPortPwrNonpol> cladPwrList = null;
            if (_mp.eliCladPwr == true)
            {
                //x축으로 50um 이동.
                m_rightStg.RelMove(m_rightStg.AXIS_X, _mp.cladDist);
                m_rightStg.WaitForIdle();

                //sweep
                int[] cladGainLvls = new int[1];
                if (_mp.gainLvls.Length >= 2) cladGainLvls[0] = _mp.gainLvls[1] + 10;//-30; //[dBm]
                else cladGainLvls[0] = _mp.gainLvls[0];

                m_swSys.SetSweepMode(_mp.ports, (int)_mp.startWave, (int)_mp.stopWave, _mp.stepWave);
                log.RecordLogItem("clading mode", "SetSweepMode 완료 ");                 //LogItem

                m_swSys.ExecSweepNonpol(_mp.ports, cladGainLvls, log);
                log.RecordLogItem("clading mode", "ExecSweepNonPol 완료 ");              //LogItem

                m_swSys.StopSweepMode(_mp.ports);
                log.RecordLogItem("clading mode", "StopSweepMode 완료 ");                //LogItem

                //aquire powerdata.
                cladPwrList = m_swSys.GetSwpPwrDataNonpol(_mp.ports);


                //reset gain level
                m_mpm.SetGainLevel(_mp.ports, _mp.gainLvls[0]);

                //return to core position
                m_rightStg.RelMove(m_rightStg.AXIS_X, (_mp.cladDist * (-1)));
                m_rightStg.WaitForIdle();
            }



            //power -> insertion loss
            int dataPoint = pwrList[0].powList.Count();
            ret = new SweepLogic.CswpNonpol();
            ret.portDataList = new List<SweepLogic.CswpPortIlNonpol>();
            ret.startWavelen = (int)_mp.startWave;
            ret.stopWavelen = (int)_mp.stopWave;
            ret.stepWavelen = Math.Round(_mp.stepWave, 3);
            for (int i = 0; i < pwrList.Count(); i++)
            {
                SweepLogic.CswpPortIlNonpol portIl = new SweepLogic.CswpPortIlNonpol();
                portIl.port = _mp.ports[i];    //여기서에서는 channel no.
                portIl.ilList = new List<double>();
                double wavelen = _mp.startWave;
                double il = 0.0;
                double inPwr = 0.0;
                double outPwr = 0.0;
                for (int j = 0; j < dataPoint; j++)
                {

                    try //input power
                    {
                        inPwr = _mp.swpRef.RefPow(_mp.ports[i], wavelen);
                    }
                    catch
                    {
                        inPwr = 0;
                    }

                    try //output power.
                    {
                        if (_mp.eliCladPwr == true)
                            outPwr = pwrList[i].powList[j] - cladPwrList[i].powList[j];
                        else
                            outPwr = pwrList[i].powList[j];
                    }
                    catch
                    {
                        outPwr = pwrList[i].powList[j];
                    }

                    try //insertion loss
                    {
                        if (outPwr > 0)
                            il = JeffOptics.CalcTransmittance_dB(inPwr, outPwr);
                        else
                            il = -80;
                        il = Math.Round(il, 3);

                        if (il <= -80)
                            il = -80;
                    }
                    catch
                    {
                        il = -80;
                    }

                    portIl.ilList.Add(il);
                    wavelen += _mp.stepWave;
                }

                ret.portDataList.Add(portIl);
            }


            //chip 채널 번호 수정 1,2,3,4...
            int chnlCnt = ret.portDataList.Count();
            if (_mp.faKind == FA_SMF)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = ret.portDataList[i].port - chnlCnt;
                }
            }


            //칩 방향이 역방향이면 channel no.를 변경시킨다.
            //( ex 1,2,3,4 -> 4,3,2,1 )
            if (_mp.chDir == DIRECTION_REVERSE)
            {
                for (int i = 0; i < chnlCnt; i++)
                {
                    ret.portDataList[i].port = chnlCnt - i;
                }
            }


        }
        catch
        {
            if (ret != null)
                ret = null;
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

        const int STAGECLOSEMARGIN = 100;
        const int ALIGNDIST = 6;      

        try
        {

            //lsm를 이용하여 1차함수 parameter를 구한다.
            //y = ax+b
            //input 쪽 좌표가 기준.
            double ay1 = 0.0;//input y축 기울기 
            double ay2 = 0.0;//output y축 기울기 
            double by1 = 0.0;//input y축 절편.
            double by2 = 0.0;//output의 y축 절편    

            int posCnt = _posList.Count();
            if (posCnt < 2)
            {
                //--default--//
                ay1 = 0.0;
                ay2 = 0.0;
                by1 = _posList.Last().stgsPos.posIn.y;
                by2 = _posList.Last().stgsPos.posOut.y;

            }
            else
            {
                //--lsm--//

                //y축
                double[] xPoss = new double[posCnt];
                double[] yPoss = new double[posCnt];
                for (int i = 0; i < _posList.Count(); i++)  //input.
                {
                    xPoss[i] = _posList[i].stgsPos.posCtr.x;
                    yPoss[i] = _posList[i].stgsPos.posIn.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref ay1, ref by1);


                for (int i = 0; i < _posList.Count(); i++)  //output.
                {
                    xPoss[i] = _posList[i].stgsPos.posCtr.x;
                    yPoss[i] = _posList[i].stgsPos.posOut.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref ay2, ref by2);

            }


            //next chip 위치 계산. 
            double nextPosCtrX = 0.0;
            double nextPosInY = 0.0;
            double nextPosOutY = 0.0;
            double nextPosInZ = 0.0;
            double nextPosOutZ = 0.0;
            if (posCnt < 2)
            {
                //x _ center
                nextPosCtrX = (int)(_posList[0].stgsPos.posCtr.x - (_chipWdith * (_curIdx + 1)));

                //y
                nextPosInY = (int)(ay1 * nextPosCtrX + by1);
                nextPosOutY = (int)(ay2 * nextPosCtrX + by2);

                //z
                nextPosInZ = _posList.Last().stgsPos.posIn.z - STAGECLOSEMARGIN;
                nextPosOutZ = _posList.Last().stgsPos.posOut.z - STAGECLOSEMARGIN;

            }
            else
            {

                int preChipIdx = _posList[posCnt - 2].chipIdx;
                int lastChipIdx = _posList[posCnt - 1].chipIdx;
                int dx = (int)(_posList[posCnt - 1].stgsPos.posCtr.x - _posList[posCnt - 2].stgsPos.posCtr.x);
                dx = (int)(dx / (lastChipIdx - preChipIdx));
                dx = Math.Abs(dx);

                //x _ ctr
                nextPosCtrX = _posList[posCnt - 1].stgsPos.posCtr.x - (dx * (_curIdx - lastChipIdx + 1));

                //y
                nextPosInY = (int)(ay1 * nextPosCtrX + by1);
                nextPosOutY = (int)(ay2 * nextPosCtrX + by2);

                //z
                nextPosInZ = _posList.Last().stgsPos.posIn.z - STAGECLOSEMARGIN;
                nextPosOutZ = _posList.Last().stgsPos.posOut.z - STAGECLOSEMARGIN;

            }


            //move to next-chip.
            m_leftStg.AbsMove(m_leftStg.AXIS_Z, nextPosInZ);    //z-axis
            m_rightStg.AbsMove(m_rightStg.AXIS_Z, nextPosOutZ);
            m_ctrStg.AbsMove(m_ctrStg.AXIS_X, nextPosCtrX); //center
            m_leftStg.AbsMove(m_leftStg.AXIS_Y, nextPosInY); //y-axis
            m_rightStg.AbsMove(m_rightStg.AXIS_Y, nextPosOutY);
            m_ctrStg.WaitForIdle(m_ctrStg.AXIS_X);


            //z-approach
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
        const int ALIGNDIST = 6;       //[um]

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
    /// <returns>광을 못찾거나 취소하면 false.</returns>
    private bool Alignment(alignParam ap)
    {

		const int MOVESTAGESTEP = 50;
		const int APPROACHBUFFDIST = 40;
		const int CHIP2FADIST = 10;
		bool ret = false;

        try
        {

            double temp = -100;
            bool alignSuccess = false;


            IAlignmentDigital align = null;
            if (m_align is IAlignmentDigital)
                align = (IAlignmentDigital)m_align;
            else
                throw new ApplicationException();


            //Semi-Align 상태인가? (= Align가능한가?)
            m_tls.SetTlsWavelen(ap.wavelen1);
            temp = m_mpm.ReadPwr(ap.port1);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, 3);
            if (temp >= ap.thresPwr)
                alignSuccess = true;
            else
                alignSuccess = false;


            //Sync Search 시도.(광을 찾은 상태가 아니면 )
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

				temp = m_mpm.ReadPwr(ap.port1);
				temp = JeffOptics.mW2dBm(temp);
				temp = Math.Round(temp, 3);
				if (temp < ap.thresPwr)
				{
					ret = false;
					return ret;
				}
                else
                {
                    log.RecordLogItem("Alignment", "광 Power 다시 찾음 " + temp.ToString() + "dBm");                 //LogItem
                }

                //m_tls.SetTlsWavelen(ap.wavelen1);
                //align.SyncXySearch(ap.port1,
                //                    ap.syncRng,
                //                    ap.syncStep,
                //                    ap.thresPwr);

                //if (m_stopFlag == true)
                //    throw new ApplicationException();


                //temp = m_mpm.ReadPwr(ap.port1);
                //temp = JeffOptics.mW2dBm(temp);
                //temp = Math.Round(temp, 3);
                //if (temp < ap.thresPwr)
                //    throw new ApplicationException();
            }


			//fineSearch input side.
			if (ap.searchIn == true)
            {
                m_tls.SetTlsWavelen(ap.wavelen1);
                align.XySearch(m_leftStg.stageNo,
                                ap.port1,
                                ap.searchStep);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }


            //fineSearch out side. 
            if (ap.searchOut == true)
            {
                m_tls.SetTlsWavelen(ap.wavelen1);
                align.XySearch(m_rightStg.stageNo,
                                ap.port1,
                                ap.searchStep);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }

            temp = m_mpm.ReadPwr(ap.port1);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, 3);
            log.RecordLogItem("Alignment", "XY-FineSearch 완료 " + temp.ToString() + "dBm");                 //LogItem

            //roll alignment out 
            if (ap.rollOut == true)
            {
                align.RollOut(ap.port1, ap.port2,
                                (int)(ap.rollPitch),
                                m_tls, 
                                ap.wavelen1, ap.wavelen2,
                                (int)(ap.rollRng), ap.rollStep,ap.rollpostcond);

                if (m_stopFlag == true)
                    throw new ApplicationException();
            }

            temp = m_mpm.ReadPwr(ap.port2);
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




    ///// <summary>
    ///// alignment 실행.
    ///// 1.input 
    ///// 2.output
    ///// 3.roll
    ///// </summary>
    ///// <param name="port1">port for channel 1</param>
    ///// <param name="port2">port for channel last</param>
    ///// <param name="_wavelen1">Channel1의 wavelength</param>
    ///// <param name="_wavelen2">channellast의 waveleneth</param>
    ///// <param name="_thresPowr">Alignment됬다고 보는 광파워. [dBm]</param>
    ///// <param name="_inAlign">in align할지 말지?</param>
    ///// <param name="_outAlign">out align할지 말지?</param>
    ///// <param name="_outRoll">outroll 할지 말지?</param>
    ///// <param name="_outRollDist">out roll distance [um]</param>
    ///// <returns>광을 못찾거나 취소하면 false.</returns>
    //private bool Alignment(int _port1, int _port2, 
    //                        int _wavelen1, int _wavelen2, 
    //                        int _thresPowr,
    //                        bool _inAlign,
    //                        bool _outAlign,
    //                        bool _outRoll, 
    //                        int _outRollDist)
    //{

    //    const double XYSEARCHSTEP = 1;    //[um]
    //    const int SYNCSEARCHRNG = 50;       //[um]
    //    const double SYNCSEARCHSTEP = 10;    //[um]
    //    const int ROLLSEARCHRNG = 16;         //[um]
    //    const int ROLLSEARCHSTEP = 1;         //[um]
    //    const double ROLLPOSTCOND = 0.3;  //[um]

    //    bool ret = false;

    //    try
    //    {
            

    //        double temp = -100;
    //        bool alignSuccess = false;


    //        IAlignmentDigital align = null;
    //        if (m_align is IAlignmentDigital)
    //            align = (IAlignmentDigital)m_align;
    //        else
    //            throw new ApplicationException();


    //        //align할 port와 wavelength 선택
    //        //짧은 파장으로 align한다.
    //        int alignPort = 0;
    //        double alignWavelen = 0.0;
    //        if (_wavelen1 < _wavelen2)
    //        {
    //            alignPort = _port1;
    //            alignWavelen = _wavelen1;
    //        }
    //        else
    //        {
    //            alignPort = _port2;
    //            alignWavelen = _wavelen2;
    //        }


    //        //Semi-Align 상태인가? (= Align가능한가?)
    //        m_tls.SetTlsWavelen(alignWavelen);
    //        temp = m_mpm.ReadPwr(alignPort);
    //        temp = JeffOptics.mW2dBm(temp);
    //        temp = Math.Round(temp, 3);
    //        if (temp >= _thresPowr)
    //            alignSuccess = true;
    //        else
    //            alignSuccess = false;



    //        //Sync Search 시도.(광을 찾은 상태가 아니면 )
    //        if (alignSuccess == false)
    //        {
    //            m_tls.SetTlsWavelen(alignWavelen);
    //            align.SyncXySearch(alignPort,
    //                                SYNCSEARCHRNG,
    //                                SYNCSEARCHSTEP,
    //                                _thresPowr);


    //            if (m_stopFlag == true)
    //                throw new ApplicationException();


    //            temp = m_mpm.ReadPwr(alignPort);
    //            temp = JeffOptics.mW2dBm(temp);
    //            temp = Math.Round(temp, 3);
    //            if (temp < _thresPowr)
    //                throw new ApplicationException();
    //        }



    //        //search input 
    //        if (_inAlign == true)
    //        {
    //            m_tls.SetTlsWavelen(alignWavelen);
    //            align.XySearch(m_leftStg.stageNo,
    //                            alignPort,
    //                            XYSEARCHSTEP);

    //            if (m_stopFlag == true)
    //                throw new ApplicationException();
    //        }


    //        //fine search out 
    //        if (_outAlign == true)
    //        {
    //            m_tls.SetTlsWavelen(alignWavelen);
    //            align.XySearch(m_rightStg.stageNo,
    //                            alignPort,
    //                            XYSEARCHSTEP);

    //            if (m_stopFlag == true)
    //                throw new ApplicationException();
    //        }


    //        //roll alignment out 
    //        if (_outRoll == true)
    //        {
    //            align.RollOut(_port1, _port2, _outRollDist,
    //                        m_tls, _wavelen1, _wavelen2,
    //                        ROLLSEARCHRNG, ROLLSEARCHSTEP, ROLLPOSTCOND);

    //            if (m_stopFlag == true)
    //                throw new ApplicationException();
    //        }


    //        ret = true;

    //    }
    //    catch
    //    {
    //        ret = false;
    //    }

    //    return ret;
    //}




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


			strTemp = conf.GetValue("AUTOSAVEFULL");
			if (Convert.ToInt32(strTemp) == AUTOSAVE_FULL)
				rbtnAutoSaveFull.Checked = true;
			else
				rbtnAutoSaveRng.Checked = true;

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
				chkRoll.Checked = true;
			else
				chkRoll.Checked = false;

			strTemp = conf.GetValue("RETCHIP1");
			if (strTemp == "1")
				chkRetChip1Pos.Checked = true;
			else
				chkRetChip1Pos.Checked = false;

			strTemp = conf.GetValue("ELIMINATECLADPWR");
			if (strTemp == "1")
				chkEliCladPwr.Checked = true;
			else
				chkEliCladPwr.Checked = false;

			strTemp = conf.GetValue("CLADDIST");
			txtCladDist.Text = strTemp;



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

			if (chkRoll.Checked == true)
				conf.SetValue("ROLL", "1");
			else
				conf.SetValue("ROLL", "0");

			if (chkRetChip1Pos.Checked == true)
				conf.SetValue("RETCHIP1", "1");
			else
				conf.SetValue("RETCHIP1", "0");

			if (chkEliCladPwr.Checked == true)
				conf.SetValue("ELIMINATECLADPWR", "1");
			else
				conf.SetValue("ELIMINATECLADPWR", "0");

			conf.SetValue("CLADDIST", txtCladDist.Text);

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




	/// <summary>
	/// init form
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void frmCwdmMuxFa_Load(object sender, EventArgs e)
	{

		m_tls = CGlobal.g_tls;
		m_mpm = CGlobal.g_mpm;
		m_leftStg = CGlobal.g_leftStage;
		m_rightStg = CGlobal.g_rightStage;
		m_ctrStg = CGlobal.g_othStage;
		m_swSys = CGlobal.g_swSys;
		m_align = CGlobal.g_align;
		m_distSens = CGlobal.g_distSensor;

		m_msrList = new List<Cmeasure>();
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
		string confFilepath = Application.StartupPath + "\\conf_lwdmFa.xml";
		this.Location = LoadWndStartPos(confFilepath);
		LoadConfig(confFilepath);

		m_initPos = LoadInitPos(confFilepath);
		DisplayInitPos();

		m_closedPos = LoadClosePos(confFilepath);
		DisplayClosePos();



		//tls 설정.
		if (m_tls != null)
			m_tls.SetTlsOutPwr(TLS_OUTPWR);


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
	private void frmCwdmMuxFa_FormClosing(object sender, FormClosingEventArgs e)
	{

		//save options and options.
		string confFilepath = Application.StartupPath + "\\conf_lwdmFa.xml";
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
            MessageBox.Show("측정 할 칩 갯수아 0입니다.",
                                    "확인",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
            return;
        }


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



            m_tp.cmd = CMD_RUN;

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
            m_tp.outPitch = Convert.ToInt32(cbCorepitch.Text); //out channel coreptich.

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


            //fa Arrangement, alignment , measurment, auto init pos.
            m_tp.faArrangement = chkFaArrangement.Checked;
            m_tp.alignment = chkAlignment.Checked;
            m_tp.measurement = chkMeasurement.Checked;
            m_tp.roll = chkRoll.Checked;
            m_tp.retChip1Pos = chkRetChip1Pos.Checked;
            m_tp.elimiateCladPwr = chkEliCladPwr.Checked;
            double.TryParse(txtCladDist.Text, out m_tp.cladDist);

            //chip numbers
            m_tp.chipNos = new string[hdgvChipNos.RowCount];
            for (int i = 0; i < hdgvChipNos.RowCount; i++)
            {
                m_tp.chipNos[i] = (string)(hdgvChipNos.Rows[i].Cells[1].Value);
            }

            //측정 시작
            m_autoEvent.Set();
            Thread.Sleep(100);

        }
        catch(Exception ex)
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

	
   

    /// <summary>
    /// chip no. array를 만들어 chip list에 추가!!
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOK_Click(object sender, EventArgs e)
    {

        //check chip no.
        if (txtFisrtChipNo.Text == "")
        {
            MessageBox.Show("칩넘버를 입력하세요",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            return;
        }


        //check chip count.
        try
        {
            if (Convert.ToInt32(txtChipCnt.Text) < 1)
            {
                MessageBox.Show("칩갯수는 최소한 1개 이상^^",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                return;
            }
        }
        catch
        {
            MessageBox.Show("칩갯수를 정확히 입력해주세요.",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
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
            MessageBox.Show("입력한 칩넘버 이상.",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
            return;
        }



        string[] strChipNos = null;
        try
        {
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
            MessageBox.Show("입력한 칩넘버 이상.",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
        }

        



        //grid setting.
        string[] strColumArr = "no | chip no. |  comments".Split('|');
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
        for (int i = 0; i < strChipNos.Length; i++)
        {
            strValueArr[0] = Convert.ToString(i + 1);         //no.
            strValueArr[1] = strChipNos[i];       //chip number.
            strValueArr[2] = "";
            hdgvChipNos.Insert(ref strColumArr, ref strValueArr);
        }
        hdgvChipNos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;


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
    /// stage open한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnOpenStages_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true)
            return;


        //execution.
        try
        {

            DisableWnd();

            m_tp.cmd = CMD_OPENSTAGES;
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
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        if (!m_isRuning)
            return;

        //stop
        m_stopFlag = true;

    }
	   



    /// <summary>
    /// 현재 위치를 초기위치로 설정.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnInitPosApply_Click(object sender, EventArgs e)
    {

        //string msg = "현재 스테이지 포지션을 초기위치로 설정하시겠습니까?";
        //DialogResult res = MessageBox.Show(msg,
        //                                    "확인",
        //                                    MessageBoxButtons.YesNo,
        //                                    MessageBoxIcon.Question);
        //if (res == DialogResult.No)
        //    return;

        ////설정.
        //if (m_initPos == null)
        //    m_initPos = new CStgsPos();
        //m_initPos.posIn = m_leftStg.GetAbsPositions();
        //m_initPos.posOut = m_rightStg.GetAbsPositions();
        //m_initPos.posCtr = m_ctrStg.GetAbsPositions();


        ////저장.
        //string confFilepath = Application.StartupPath + "\\conf_lwdmFa.xml";
        //if (!SaveInitPos(confFilepath))
        //{
        //    MessageBox.Show("Init position을 저장하는데 실패!!",
        //                    "확인",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Error);
        //    return;
        //}

        ////출력 
        //DisplayInitPos();
        //MessageBox.Show("설정 완료!!", "확인");
    }




    /// <summary>
    /// Init position으로 이동한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnInitPosGo_Click(object sender, EventArgs e)
    {

        //if (m_isRuning == true)
        //    return;


        ////confirm.
        //string msg = "스테이지를 초기 위치로 이동하시겠습니까?";
        //DialogResult res;
        //res = MessageBox.Show(msg,
        //                    "확인",
        //                    MessageBoxButtons.OKCancel,
        //                    MessageBoxIcon.Question);
        //if (res == DialogResult.Cancel)
        //    return;



        ////포지션이 설정되지 않았으면 이동하지 않는다.
        //if ((m_initPos.posIn.x == 0) && (m_initPos.posIn.y == 0) &&
        //    (m_initPos.posIn.z == 0) && (m_initPos.posIn.tx == 0) &&
        //    (m_initPos.posIn.ty == 0) && (m_initPos.posIn.tz == 0) &&
        //    (m_initPos.posOut.x == 0) && (m_initPos.posOut.y == 0) &&
        //    (m_initPos.posOut.z == 0) && (m_initPos.posOut.tx == 0) &&
        //    (m_initPos.posOut.ty == 0) && (m_initPos.posOut.tz == 0) &&
        //    (m_initPos.posCtr.x == 0) )
        //{

        //    MessageBox.Show("위치값이 설정되지 않았습니다",
        //                    "확인",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Information);

        //    return;
        //}


        ////execution.
        //try
        //{
        //    DisableWnd();

        //    //Go
        //    m_tp.cmd = CMD_GOINITPOS;
        //    m_autoEvent.Set();
        //    Thread.Sleep(10);

        //}
        //catch (Exception ex)
        //{
        //    EnableWnd();
        //    MessageBox.Show(ex.ToString());
        //}


    }




    /// <summary>
    /// closed-position을 지정한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClosedPosApply_Click(object sender, EventArgs e)
    {

        //string msg = "현재 스테이지 포지션을 Close위치로 설정하시겠습니까?";
        //DialogResult res = MessageBox.Show(msg,
        //                                   "확인",
        //                                   MessageBoxButtons.YesNo,
        //                                   MessageBoxIcon.Question);
        //if (res == DialogResult.No)
        //    return;


        ////설정.
        //m_closedPos.inPos = m_leftStg.GetAxisAbsPos(m_leftStg.AXIS_Z);
        //m_closedPos.outPos = m_rightStg.GetAxisAbsPos(m_rightStg.AXIS_Z);


        ////저장.
        //string confFilepath = Application.StartupPath + "\\conf_cwdmMuxFa.xml";
        //if (!SaveClosePos(confFilepath))
        //{
        //    MessageBox.Show("Close-position을 저장하는데 실패!!",
        //                    "확인",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Error);
        //    return;
        //}

        ////출력 
        //DisplayClosePos();
        //MessageBox.Show("설정 완료!!", "확인");

    }




    /// <summary>
    /// close위치로 이동.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClosedPosGo_Click(object sender, EventArgs e)
    {

        //if (m_isRuning == true)
        //    return;


        ////confirm.
        //string msg = "스테이지를 close 위치로 이동하시겠습니까?";
        //DialogResult res;
        //res = MessageBox.Show(msg,
        //                    "확인",
        //                    MessageBoxButtons.OKCancel,
        //                    MessageBoxIcon.Question);
        //if (res == DialogResult.Cancel)
        //    return;



        ////포지션이 설정되지 않았으면 이동하지 않는다.
        //if ((m_closedPos.inPos == 0) && (m_closedPos.outPos == 0))
        //{

        //    MessageBox.Show("위치값이 설정되지 않았습니다",
        //                    "확인",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Information);

        //    return;
        //}




        ////execution.
        //try
        //{

        //    DisableWnd();

        //    //Go
        //    m_tp.cmd = CMD_CLOSESTAGES;
        //    m_autoEvent.Set();
        //    Thread.Sleep(10);


        //}
        //catch (Exception ex)
        //{
        //    EnableWnd();
        //    MessageBox.Show(ex.ToString());
        //}

    }




    /// <summary>
    /// close stages
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCloseStages_Click(object sender, EventArgs e)
    {

        if (m_isRuning == true)
            return;


        //execution.
        try
        {

            DisableWnd();

            m_tp.cmd = CMD_CLOSESTAGES;
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
