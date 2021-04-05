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
using Free302.TnM.DataAnalysis;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;
using al = Neon.Aligner.AlignLogic;

public partial class frmAlignStatus : Form
{

    
    #region definition

    private const int RESMW = 9;        // 10^(-9) mW
    private const int RESDBM = 3;       // 10^(-3) dBm

    #endregion



    
    #region Private member Variables

    private bool m_bStop;
    private Thread m_pThread;
    private AutoResetEvent m_pAutoEvent;

    private Istage m_leftStage;
    private Istage m_rightStage;
    private AlignLogic mAlign;

    #endregion




    #region structure/innor class

    private struct labelParam
    {
        public string CmdName;
        public string Stage;
        public string Unit;
    }

    #endregion




    #region consturctor/destructor


    /// <summary>
    /// 생성자
    /// </summary>
    public frmAlignStatus()
    {
        InitializeComponent();

        initGraph(mGraph);
    }



    /// <summary>
    /// 폼을 초기화한다.
    /// </summary>
    private void frmAlignStatus_Load(object sender, EventArgs e)
    {

        m_leftStage = (Istage)(CGlobal.LeftAligner);
        m_rightStage = (Istage)(CGlobal.RightAligner);
        mAlign = CGlobal.alignLogic;
        if (mAlign != null) mAlign.AlignStarted += startAlign;

        string confFilepath = Application.StartupPath + @"\config\config_AlignStatus.xml";
        this.Location = LoadWndStartPos(confFilepath);


        m_pAutoEvent = new AutoResetEvent(false);
        m_pThread = new Thread(ThreadFunc);
        m_pThread.IsBackground = true;
        m_pThread.Name = "AlignStatus";
        m_pThread.Start();

    }


    
    /// <summary>
    /// 폼을 마무리한다.
    /// </summary>
    private void frmAlignStatus_FormClosing(object sender, FormClosingEventArgs e)
    {

        if (mAlign != null) mAlign.AlignStarted -= startAlign;

        string confFilepath = Application.StartupPath + @"\config\config_AlignStatus.xml";
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


    #endregion


    

    #region Thread Function


    /// <summary>
    /// Thread Function
    /// </summary>
    private void ThreadFunc()
    {
        while (true)
        {
            m_pAutoEvent.WaitOne();

            if (mAlign.IsCompleted() == false) displayAlignStatus(mAlign.CurFuncNo);

            Thread.Sleep(100);
        }//while
    }



    private void startAlign(int alingId)
    {
        m_pAutoEvent.Set();
    }



    private void displayAlignStatus(int alingId)
    {
        m_bStop = false;

        if (alingId == al.ZAPPROACH_SINGLE) ZApproach();
        else if (alingId == al.ZAPPROACH_DUAL) ZApproachDual();
        else if (alingId == al.ANGLE_TY_SINGLE) FaArrangeTy();
        else if (alingId == al.ANGLE_TX_SINGLE) FaArrangeTx();
        else if (alingId == al.XY_SEARCH) XySearch();
        else if (alingId == al.XYBLINDSEARCH) XyBlindSearch();
        else if (alingId == al.XYFULLBLINDSEARCH) XyFullBlindSearch();
        else if (alingId == al.AXISSEARCH) AxisSearch();
        else if (alingId == al.ROLLOUT) RollOut();
        else if (alingId == al.SYNCXYSEARCH) SyncXySearch();
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
    private void SetLabelMsg(Label _lb, string _msg)
    {
        _lb.Text = _msg;
        _lb.Refresh();
    }
    


    private void SetInitLabel(labelParam _label)
    {
        SetLabelMsg(lbLeadTime, "...");
        SetLabelMsg(lbCmdName, _label.CmdName);
        SetLabelMsg(lbStage, _label.Stage);
        SetLabelMsg(lbCurUnit, _label.Unit);
        SetLabelMsg(lbMaxUnit, _label.Unit);
    }
    


    private void PlotDatas(double[] data1)
    {
        if ((data1?.Length ?? 0) < 1) return;
        try
        {
            var x1 = new double[] { 0, data1.Length };
            DataPlot.Plot(mGraph, data1, x1, 0, _chs[0]);
        }
        catch { }
    }

    Ch[] _chs = new Ch[] { Ch.CH1, Ch.CH4 };
    double[][] _data = new double[2][];

    private void Plot2Datas(double[] data1, double[] data2)
    {
        if ((data1?.Length ?? 0) < 1) return;
        if ((data2?.Length ?? 0) < 1) return;
        try
        {
            var x1 = new double[] { 0, data1.Length };
            _data[0] = data1;
            _data[1] = data2;
            DataPlot.Plot(mGraph, _data, x1, 0, _chs);
        }
        catch { }
    }
    WdmGraph initGraph(WdmGraph g)
    {
        g.ShowLegends = false;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.ChartType = SeriesChartType.FastLine;
        g.LineThickness = 2;

        g.ScaleFactorX = 1;
        g.Cwl = new List<int> { 0, 1000 };
        g.ReCalcIntervalX = false;
        //g.IntervalY = 0.05;
        //g.MinY = -45;

        return g;
    }


    const int _updateDelay_ms = 100;

    /// <summary>
    /// ZApproach 상태 출력
    /// </summary>
    private void ZApproach()
    {

        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "Z-Approach";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[V]";
        Invoke(lp, new object[] {initLabel});


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        var state = (stageNo == al.STAGE_L) ? AlignStatusPool.zappIn : AlignStatusPool.zappOut;
        while (true)
        {
            try
            {
                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });    //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //ZApproach에서는 max value가 필요 없음.

            }
            catch { /*do nothing*/ } 
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }


    
    /// <summary>
    /// ZApproach 상태 출력
    /// </summary>
    private void ZApproachDual()
    {

        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();

        var initLabel = new labelParam();
        initLabel.CmdName = "Z-Approach Dual";
        initLabel.Stage = "L & R";
        initLabel.Unit = "[V]";
        Invoke(lp, new object[] { initLabel });


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

                Invoke(pd2, new object[] { datas1, datas2 });      //plot
                Invoke(slm, new object[] { lbCurVal, "..." });     //current value.
                Invoke(slm, new object[] { lbMaxValue, "..." });   //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);



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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds,2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }

    

    /// <summary>
    ///  FA Arrangement   θY
    /// </summary>
    private void FaArrangeTy()
    {
        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();

        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "Angle θY";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[V]";
        Invoke(lp, new object[] { initLabel });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        var state = (stageNo == al.LEFT_STAGE) ? AlignStatusPool.faTyIn : AlignStatusPool.faTyOut;
        while (true)
        {
            try
            {
                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //FA-Arrange θY는 max value가 필요 없음.
            }
            catch { } 
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }


        
    /// <summary>
    ///  FA Arrangement   θX
    /// </summary>
    private void FaArrangeTx()
    {
        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "Angle θX";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[V]";
        Invoke(lp, new object[] { initLabel });


        //plot & display
        double dbCurVal = 0;
        double[] dbValArr = null;
        var state = (stageNo == al.STAGE_L) ? AlignStatusPool.faTxIn : AlignStatusPool.faTxOut;
        while (true)
        {
            try
            {
                dbCurVal = state.sens;
                dbValArr = state.sensList.ToArray();

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) }); //current value.
                Invoke(slm, new object[] { lbMaxValue, "___" }); //FA-Arrange θY는 max value가 필요 없음.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });

    }


    const double _min_mW = 1e-9;
    const double _max_mW = 1e+1;
    static double mw2dBm(double mW)
    {
        if (mW < _min_mW) mW = _min_mW;
        else if (mW > _max_mW) mW = _max_mW;
        return Math.Round(Unit.MillWatt2Dbm(mW), RESDBM);
    }
    static double[] mw2dBm(IList<double> mWs) => mWs.Select(m => mw2dBm(m)).ToArray();

    private void XySearch()
    {
        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        
        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var lp = new labelParam();
        lp.CmdName = "XY Search";
        lp.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        lp.Unit = "[dBm]";
        Invoke((Action<labelParam>)SetInitLabel, new object[] { lp });

        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = -100;
        var state = (stageNo == al.LEFT_STAGE) ? AlignStatusPool.xySearchIn : AlignStatusPool.xySearchOut;

        while (true)
        {
            try
            {
                if ((state.pwrList?.Count ?? 0) <= 0) continue;
                dbCurVal = mw2dBm(state.pwr);
                dbValArr = mw2dBm(state.pwrList);
                dbMaxVal = dbValArr.Max();

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });     //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) });   //max value.
            }
            catch { }
            Thread.Sleep(_updateDelay_ms);

            //completed?
            if ((mAlign.IsCompleted() == true) || (mAlign.CurFuncNo != al.XY_SEARCH)) 
            {
                Invoke(slm, new object[] { lbCmdName, "XySearch...Complete!!" });
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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }


        
    /// <summary>
    /// XYFineSearch ( Digital )
    /// </summary>
    private void SyncXySearch()
    {

        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();

        var initLabel = new labelParam();
        initLabel.CmdName = "XY-SyncXySearch";
        initLabel.Stage = "L/R";
        initLabel.Unit = "[dBm]";
        Invoke(lp, new object[] { initLabel });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = -100;

        while (true)
        {
            try
            {
                CsearchStatus state = AlignStatusPool.syncXySearchIn;

                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });   //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) }); //max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds,2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }


    
    /// <summary>
    /// BlindSearch 
    /// </summary>
    private void XyBlindSearch()
    {

        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "XyBlindSearch";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[dBm]";
        Invoke(lp, new object[] { initLabel });


        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = 0;
        var state = (stageNo == al.LEFT_STAGE) ? AlignStatusPool.xyBlindSearchIn : AlignStatusPool.xyBlindSearchOut;
        while (true)
        {
            try
            {
                dbCurVal = mw2dBm(state.pwr);
                dbValArr = mw2dBm(state.pwrList);
                dbMaxVal = dbValArr.Max();

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });   //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) }); //max value.
            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }

    

    /// <summary>
    /// BlindSearch 
    /// </summary>
    private void XyFullBlindSearch()
    {
        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "XyFullBlindSearch";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[dBm]";
        Invoke(lp, new object[] { initLabel });

        //plot & value
        double dbCurVal = 0;
        double[] dbValArr = null;
        double dbMaxVal = 0;
        var state = AlignStatusPool.xyBlindSearchIn;
        while (true)
        {
            try
            {
                dbCurVal = Math.Round(JeffOptics.mW2dBm(state.pwr), RESDBM);
                dbValArr = JeffOptics.mW2dBm(state.pwrList.ToArray());
                dbMaxVal = Math.Round(JeffOptics.mW2dBm(state.pwrList.Max()), RESDBM);

                Invoke(pd, new object[] { dbValArr });   //plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });     //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) });   //max value.
            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);

            //completed?
            if (mAlign.IsCompleted() == true)
            {
                Invoke(slm, new object[] { lbCmdName, "XyFullBlindSearch...Completed!!" });
                break;
            }

            //stop?
            if (m_bStop == true)
            {
                Invoke(slm, new object[] { lbCmdName, "XyFullBlindSearch...Stoped!!" });
                break;
            }

        } //while

        //leadTime. (complete/stop)
        jTimer.StopTimer();
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }
    


    /// <summary>
    /// AxisSearch ( Digital )
    /// </summary>
    private void AxisSearch()
    {
        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "AxisSearch";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[dBm]";
        Invoke(lp, new object[] { initLabel });

        Istage stage = (stageNo == al.LEFT_STAGE) ? m_leftStage : m_rightStage;

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

                dbCurVal = mw2dBm(state.pwr);
                dbValArr = mw2dBm(state.pwrList);
                dbMaxVal = dbValArr.Max();

                Invoke(pd, new object[] { dbValArr });//plot
                Invoke(slm, new object[] { lbCurVal, Convert.ToString(dbCurVal) });  //current value.
                Invoke(slm, new object[] { lbMaxValue, Convert.ToString(dbMaxVal) });//max value.

            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);


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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds, 2);
        Invoke(slm, new object[] { lbLeadTime, leadTime.ToString() });
    }

    
    
    /// <summary>
    /// Roll alignment. 
    /// </summary>
    private void RollOut()
    {

        Action<Label, string> slm = SetLabelMsg;
        Action<double[]> pd = PlotDatas;
        Action<double[], double[]> pd2 = Plot2Datas;
        Action<labelParam> lp = SetInitLabel;

        //leadTime. (init)
        JeffTimer jTimer = new JeffTimer();
        jTimer.StartTimer();
        int stageNo = mAlign.CurStageNo;

        var initLabel = new labelParam();
        initLabel.CmdName = "RollOut";
        initLabel.Stage = (stageNo == al.STAGE_L) ? "LEFT" : "RIGHT";
        initLabel.Unit = "[dBm]";
        Invoke(lp, new object[] { initLabel });


        //plot & value
        double[] datas1 = null;
        double[] datas2 = null;
        var state = (stageNo == al.LEFT_STAGE) ? AlignStatusPool.rollIn : AlignStatusPool.rollOut;
        while (true)
        {
            try
            {
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

                Invoke(pd2, new object[] { datas1, datas2 });       //plot
                Invoke(slm, new object[] { lbCurVal, "..." });      //current value.
                Invoke(slm, new object[] { lbMaxValue, "..." });    //max value.
            }
            catch { /*do nothing*/ }
            Thread.Sleep(_updateDelay_ms);

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
        double leadTime = Math.Round(jTimer.GetLeadTime().TotalSeconds,2);
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



}

