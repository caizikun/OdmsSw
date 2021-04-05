using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Jeffsoft;
using Neon.Aligner;
using al = Neon.Aligner.AlignLogic;



public partial class frmAlignStatus : Form//,IFormCanClosed
{

    #region definition

    private const int RESMW = 9;// 10^(-9) mW
    private const int RESDBM = 3;// 10^(-3) dBm

    #endregion


    #region "Private member Variables"

    private bool m_bStop;
    private Thread m_pThread;
    private AutoResetEvent m_pAutoEvent;

    private Istage m_leftStage;
    private Istage m_rightStage;
    private AlignLogic mAlign;

    #endregion




    #region consturctor/destructor

    /// <summary>
    /// 생성자
    /// </summary>
    public frmAlignStatus()
    {
        InitializeComponent();
    }

    #endregion





    #region Thread Function


    /// <summary>
    /// Thread Function
    /// </summary>
    private void ThreadFunc()
    {
        while (true)
        {
            if (mAlign == null)
            {
                Thread.Sleep(1000);
                continue;
            }

            if (mAlign.IsCompleted() == false)
            {
                int alignNo = mAlign.CurFuncNo;

                if (alignNo == al.ZAPPROACH_SINGLE) ZApproach();
                else if (alignNo == al.ZAPPROACH_DUAL) ZApproachDual();
                else if (alignNo == al.ANGLE_TY_SINGLE) FaArrangeTy();
                else if (alignNo == al.ANGLE_TX_SINGLE) FaArrangeTx();

                else if (alignNo == al.XY_SEARCH) XySearch();
                else if (alignNo == al.XYBLINDSEARCH) XyBlindSearch();
                else if (alignNo == al.AXISSEARCH) AxisSearch();
                else if (alignNo == al.ROLLOUT) RollOut();
                else if (alignNo == al.SYNCXYSEARCH) SyncXySearch();
            }
			Thread.Sleep(100);
        }//while
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
    /// Label 객체에 메세지 출력!!
    /// </summary>
    /// <param name="_lb"> label 객체 포인터</param>
    /// <param name="_msg"> 출력할 메세지</param>
    private void SetLabelMsg( System.Windows.Forms.Label _lb, string _msg)
    {
        _lb.Text = _msg;
        _lb.Refresh();
    }




    /// <summary>
    /// 데이터를 Plot.
    /// </summary>
    /// <param name="_datas"> 출력데이터.  </param>
    private void PlotDatas(double[] _datas)
    {
        waveformPlot1.Visible = true;
        waveformPlot2.Visible = false;

        if (_datas != null)
            waveformPlot1.PlotY(_datas);

        wfgGraph.Refresh();
    }



    /// <summary>
    /// 데이터1,2 Plot
    /// </summary>
    /// <param name="_datas1"> 출력데이터1 </param>
    /// <param name="_datas2"> 출력데이터2 </param>
    private void Plot2Datas(double[] _datas1, double[] _datas2)
    {
		try
		{
			waveformPlot1.Visible = true;
			waveformPlot2.Visible = true;

			if (_datas1 != null)
				waveformPlot1.PlotY(_datas1);

			if (_datas2 != null)
				waveformPlot2.PlotY(_datas2);

			wfgGraph.Refresh();
		}
		catch { }
    }




    /// <summary>
    /// ZApproach 상태 출력
    /// </summary>
    private void ZApproach()
    {

        Action<System.Windows.Forms.Label,string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;

        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });


        //process Name.
        Invoke(slm, new object[] { lbCmdName, "Z-Approach" });


        //Stage
        int stageNo = mAlign.CurStageNo;
        if (stageNo == al.STAGE_L)
            strTemp = "LEFT";
        else
            strTemp = "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });


        ////value unit
        //Invoke(slm, new object[] { lbCurUnit, "[V]" });
        //Invoke(slm, new object[] { lbMaxUnit, "[V]" });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        while (true)
        {

            //plot & display
            try
            {
                CzappStatus state = null;
                if (stageNo == al.STAGE_L)
                    state = AlignStatusPool.zappIn;
                else if (stageNo == al.STAGE_R)
                    state = AlignStatusPool.zappOut;

                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });    //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //ZApproach에서는 max value가 필요 없음.

            }
            catch { /*do nothing*/ } 
            Thread.Sleep(50);



            //완료?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "ZAPPROACH...Completed!!" });
                break;
            }

            //정지?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "ZAPPROACH...Stoped!!" });
                break;
            }

        }//while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }







    /// <summary>
    /// ZApproach 상태 출력
    /// </summary>
    private void ZApproachDual()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });


        //process Name.
        Invoke(slm, new object[] { lbCmdName, "Z-Approach Dual" });


        //Stage
        Invoke(slm, new object[] { lbStage, "L & R" });


        //value unit
        Invoke(slm, new object[] { lbCurUnit, "[V]" });
        Invoke(slm, new object[] { lbMaxUnit, "[V]" });


        //plot & value
        double[] datas1 = null;
        double[] datas2 = null;
        while (true)
        {

            //plot & display
            try
            {
                try
                {
                    datas1 = AlignStatusPool.zappIn.sensList.ToArray();
                    datas2 = AlignStatusPool.zappOut.sensList.ToArray();
                }
                catch
                {
                    datas1 = null;
                    datas2 = null;
                }

                Invoke(pd2, new object[] { datas1, datas2 });   //plot
                Invoke(slm, new object[] { lbCurVal, "..." });     //current value.
                Invoke(slm, new object[] { lbMaxValue, "..." });    //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);



            //완료?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "ZAPPROACH...Completed!!" });
                break;
            }

            //정지?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "ZAPPROACH...Stoped!!" });
                break;
            }

        }//while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }










    /// <summary>
    ///  FA Arrangement   θY
    /// </summary>
    private void FaArrangeTy()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;
            
        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });


        //process Name.
        Invoke(slm, new object[] { lbCmdName, "FA-Arrange θY" });


        //Stage
        int stageNo = mAlign.CurStageNo;
        if (stageNo == al.STAGE_L) strTemp = stageNo == al.STAGE_L ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });

        //value unit
        Invoke(slm, new object[] { lbCurUnit, "[V]" });
        Invoke(slm, new object[] { lbMaxUnit, "[V]" });



        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        while (true)
        {

            try
            {
                AngleStatus state = null;
                if (stageNo == al.LEFT_STAGE)
                    state = AlignStatusPool.faTyIn;
                else
                    state = AlignStatusPool.faTyOut;

                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //FA-Arrange θY는 max value가 필요 없음.
            }
            catch { /*do nothing*/ } 
            Thread.Sleep(50);


            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "FA-Arrange θY...Completed!!" });
                break;
            }


            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "FA-Arrange θY...Stoped!!" });
                break;
            }

        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });


    }






    /// <summary>
    ///  FA Arrangement   θX
    /// </summary>
    private void FaArrangeTx()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;

        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });

        //process Name.
        Invoke(slm, new object[] { lbCmdName, "FA-Arrange θX" });

        //Stage
        int stageNo = mAlign.CurStageNo;
        strTemp = stageNo == al.LEFT_STAGE ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });

        //plot & display
        double dbCurVal = 0;
        double[] dbValArr = null;
        while (true)
        {

            try
            {
                AngleStatus state = stageNo == al.STAGE_L ? AlignStatusPool.faTxIn : AlignStatusPool.faTxOut;

                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //FA-Arrange θY는 max value가 필요 없음.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);


            //completed?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "FA-Arrange θX...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "FA-Arrange θX...Stoped!!" });
                break;
            }

        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }





    /// <summary>
    /// XYFineSearch ( Digital )
    /// </summary>
    private void XySearch()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });

        //process Name.
        Invoke(slm, new object[] { lbCmdName, "XY-FineSearch" });

        //Stage
        int stageNo = mAlign.CurStageNo;
        var strTemp = stageNo == al.LEFT_STAGE ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = -100;

        while (true)
        {

            try
            {
                CsearchStatus state = stageNo == al.LEFT_STAGE ? AlignStatusPool.xySearchIn : AlignStatusPool.xySearchOut;

                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });     //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) }); //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);

            //completed?
            if ((mAlign.IsCompleted() == true) || (mAlign.CurFuncNo != al.XY_SEARCH)) 
            {
                Invoke(slm, new object[] { lbCmdName, "XySearch...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "XySearch...Stoped!!" });
                break;
            }

        } //while



        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
        

    }







    /// <summary>
    /// XYFineSearch ( Digital )
    /// </summary>
    private void SyncXySearch()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;

        string strTemp = "";

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });


        //process Name.
        Invoke(slm, new object[] { lbCmdName, "XY-SyncXySearch" });


        //Stage
        int stageNo = al.LEFTRIGHT_STAGE;
        strTemp = "L/R";
        Invoke(slm, new object[] { lbStage, strTemp });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = -100;

        while (true)
        {

            try
            {

                CsearchStatus state;
                state = AlignStatusPool.syncXySearchIn;

                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) }); //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);

            //completed?
            if ((mAlign.IsCompleted() == true) || (mAlign.CurFuncNo != al.SYNCXYSEARCH))
            {
                Invoke(slm, new object[] { lbCmdName, "SyncXySearch...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "SyncXySearch...Stoped!!" });
                break;
            }

        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }







    /// <summary>
    /// BlindSearch 
    /// </summary>
    private void XyBlindSearch()
    {

        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;


        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });

        //process Name.
        Invoke(slm, new object[] { lbCmdName, "XyBlindSearch" });

        //Stage
        int stageNo = mAlign.CurStageNo;
        strTemp = stageNo == al.LEFT_STAGE ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });

        ////unit 표시.
        //Invoke(slm, new object[] { lbCurUnit, "[dBm]" });
        //Invoke(slm, new object[] { lbMaxUnit, "[dBm]" });

        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = 0;
        while (true)
        {

            try
            {
                CsearchStatus state = stageNo == al.LEFT_STAGE ? AlignStatusPool.xyBlindSearchIn : AlignStatusPool.xyBlindSearchOut;

                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) }); //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);


            //completed?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "XyBlindSearch...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "XyBlindSearch...Stoped!!" });
                break;
            }

        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }




    /// <summary>
    /// AxisSearch ( Digital )
    /// </summary>
    private void AxisSearch()
    {


        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;



        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });

        //process Name.
        Invoke(slm, new object[] { lbCmdName, "AxisSearch" });

        //Stage
        int stageNo = mAlign.CurStageNo;
        strTemp = stageNo == al.LEFT_STAGE ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });

        Istage stage = stageNo == al.LEFT_STAGE ? m_leftStage : m_rightStage;            

        //Axis
        int axisNo = mAlign.CurAxisNo;


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = -100;
        while (true)
        {

            try
            {
                CsearchStatus state = null;
                if (stageNo == al.LEFT_STAGE)
                {
                    if (axisNo == stage.AXIS_X) state = AlignStatusPool.axisSearchInX;
                    else if (axisNo == stage.AXIS_Y) state = AlignStatusPool.axisSearchInY;
                }
                else if (stageNo == al.RIGHT_STAGE)
                {
                    if (axisNo == stage.AXIS_X) state = AlignStatusPool.axisSearchOutX;
                    else if (axisNo == stage.AXIS_Y) state = AlignStatusPool.axisSearchOutY;
                }

                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });//current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) });//max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);


            //completed?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "AxisSearch...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "AxisSearch...Stoped!!" });
                break;
            }


        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }





    /// <summary>
    /// Roll alignment. 
    /// </summary>
    private void RollOut()
    {


        Action<System.Windows.Forms.Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;



        string strTemp = "";


        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        Invoke(slm, new object[] { lbLeadTime, "..." });

        //process Name.
        Invoke(slm, new object[] { lbCmdName, "RollOut" });

        //Stage
        int stageNo = mAlign.CurStageNo;
        strTemp = stageNo == al.LEFT_STAGE ? "LEFT" : "RIGHT";
        Invoke(slm, new object[] { lbStage, strTemp });

        //plot & value
        double[] datas1 = null;
        double[] datas2 = null;
        while (true)
        {
            try
            {
                CrollStatus state = stageNo == al.LEFT_STAGE ? AlignStatusPool.rollIn : AlignStatusPool.rollOut;
                try
                {
                    datas1 = state.pwrList1.ToArray();
                    datas2 = state.pwrList2.ToArray();
                }
                catch
                {
                    datas1 = null;
                    datas2 = null;
                }


                Invoke(pd2, new object[] { datas1, datas2 });//plot
                Invoke(slm, new object[] { lbCurVal, "..." });//current value.
                Invoke(slm, new object[] { lbMaxValue, "..." });//max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(50);


            //completed?
            if ((mAlign.IsCompleted() == true))
            {
                Invoke(slm, new object[] { lbCmdName, "RollOut...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "RollOut...Stoped!!" });
                break;
            }


        } //while


        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = jTimer.GetLeadTime().TotalSeconds;
        leadTime = Math.Round(leadTime, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }





    #endregion


    #region public method


    /// <summary>
    /// Display를 멈춘다.!!
    /// </summary>
    public void Stop()
    {
        m_bStop = true;
    }



    #endregion






    /// <summary>
    /// 폼을 초기화한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAlignStatus_Load(object sender, EventArgs e)
    {
        m_leftStage = (Istage)(CGlobal.LeftAligner);
        m_rightStage = (Istage)(CGlobal.RightAligner);
        mAlign = CGlobal.Alignment;


        string confFilepath = Application.StartupPath + "\\config_AlignStatus.xml";
        this.Location = LoadWndStartPos(confFilepath);


        m_pAutoEvent = new AutoResetEvent(false);
        m_pThread = new Thread(ThreadFunc);
        m_pThread.Start();

    }


    /// <summary>
    /// 폼을 마무리한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAlignStatus_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;

        string confFilepath = Application.StartupPath + "\\config_AlignStatus.xml";
        SaveWndStartPos(confFilepath);

        if (m_pThread != null)
        {
            m_bStop = true;
            Thread.Sleep(50);

            m_pThread.Abort();
            m_pThread.Join();
            m_pThread = null;
        }
        m_leftStage = null;
        m_rightStage = null;
        mAlign = null;
    }
    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }

    private void label5_Click(object sender, EventArgs e)
    {

    }
}

