using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Neon.Aligner;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;

public partial class frmDistSensViewer : Form
{


    #region Delegate

    private delegate void fpSetMessage(ref System.Windows.Forms.TextBox tb,string strMessage);
    private delegate void pPlotDatas(double Val1, double Val2);

    #endregion




    #region private member variables.

    private IDispSensor m_distSens;
    private AutoResetEvent m_pAutoEvent;

    private Thread m_thread;
    private bool m_bIsOperating;
    private bool m_bStop;

    #endregion




    #region Thread Function


    private void ThreadFunc()
    {

        pPlotDatas pd = new pPlotDatas(DisplaySensorsValue);
        Action<Label, string> addMsg = LabelSetMsg;
        

        //Sensor value를 구해서 출력한다.
        double leftSens = 0;
        double rightSens = 0;


        while (true)
        {

            m_pAutoEvent.WaitOne();
            m_bIsOperating = true;
            
            while (true)
            {

                if (m_bStop == true)
                {
                    m_bStop = false;
                    break;
                }
                
                //read
                try
                {
                    leftSens = Math.Round(m_distSens.ReadDist(SensorID.Left), 3);
                    rightSens = Math.Round(m_distSens.ReadDist(SensorID.Right), 3);
                }
                catch
                {
                    leftSens = 0.0;
                    rightSens = 0.0;
                }


                //출력
                Invoke(pd, new object[] { leftSens, rightSens });
                Invoke(addMsg, new object[] { lbLeftSens, Convert.ToString(leftSens) });
                Invoke(addMsg, new object[] { lbRightSens, Convert.ToString(rightSens) });

                Thread.Sleep(100);

            }
            
            m_bIsOperating = false;

        } //while

    }


    #endregion




    #region Constructor / distructor


    public frmDistSensViewer()
    {
        InitializeComponent();

        initGraph(wg1);
        initGraph(wg2);
    }



    /// <summary>
    /// Init form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmDistSensViewer_Load(object sender, EventArgs e)
    {
        m_distSens = CGlobal.Ds2000;
        m_pAutoEvent = new AutoResetEvent(false);


        //configs.
        string confFilepath = Application.StartupPath + @"config\conf_distSens.xml";
        this.Location = LoadWndStartPos(confFilepath);


        //Thread를 동작 시킨다.!!
        m_thread = new Thread(ThreadFunc);
        m_thread.Start();
    }



    /// <summary>
    /// terminate form.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmDistSensViewer_FormClosing(object sender, FormClosingEventArgs e)
    {

        //configs.
        string confFilepath = Application.StartupPath + "\\config\\conf_distSens.xml";
        SaveWndStartPos(confFilepath);


        //thread && kernel object
        if (m_thread != null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        if (m_pAutoEvent != null)
            m_pAutoEvent.Dispose();
        m_pAutoEvent = null;

    }


    #endregion




    #region Private member Function



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
    /// label에 message를 입력한다.
    /// </summary>
    /// <param name="lb"></param>
    /// <param name="strMessage"></param>
    private void LabelSetMsg(System.Windows.Forms.Label lb, string strMessage)
    {
        lb.Text = strMessage;
    }



    
    //////////////////////////////////////////////////////////////
    //DisplaySensorsValue ///////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - 그래프를 그린다.
    //
    //Param - [IN] dbLeftSensorValue : Left Sensor Value
    //           [OUT] dbRightSensorValue : Left Sensor Value
    //
    private void DisplaySensorsValue(double dbLeftSensorValue, double dbRightSensorValue)
    {
        _counter++;
        if (_counter > 100)
        {
            _counter = 0;
            _graphData[0].Clear();
            _graphData[1].Clear();
        }
        if (_counter == 0) return;
        ////Left Sensor Value 출력!!
        //wfgLeft.Plots[0].PlotYAppend(dbLeftSensorValue);
        ////Right Sensor Value 출력!!
        //wfgRight.Plots[0].PlotYAppend(dbRightSensorValue);

        _graphData[0].Add(dbLeftSensorValue);
        _graphData[1].Add(dbRightSensorValue);
        DataPlot.Plot(wg1, _graphData[0].ToArray(), new double[] { 0, _counter }, 0, Ch.CH1);
        DataPlot.Plot(wg2, _graphData[1].ToArray(), new double[] { 0, _counter }, 0, Ch.CH4);
    }
    List<double>[] _graphData;
    volatile int _counter = 0;
    WdmGraph initGraph(WdmGraph g)
    {
        _graphData = new List<double>[2];
        _graphData[0] = new List<double>();
        _graphData[1] = new List<double>();

        g.ShowLegends = false;
        g.BorderStyle = BorderStyle.FixedSingle;
        g.ChartType = SeriesChartType.FastLine;
        g.LineThickness = 1;

        g.ScaleFactorX = 1;
        g.Cwl = new List<int> { 0, 1000 };
        g.ReCalcIntervalX = false;

        return g;
    }

    #endregion




    #region Public method


    //////////////////////////////////////////////////////////////
    //DisplayOn ////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc -  센싱 시작!!
    //
    public void StartSensing()
    {

        if (m_bIsOperating == false)
        {
            m_pAutoEvent?.Set();
            Thread.Sleep(10);
        }

    }




    ////////////////////////////////////////////////////////////////
    ////StopSensing /////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    ////desc -  sensing을 멈춘다.
    ////

    public void StopSensing()
    {

        if (m_bIsOperating == true)
        {
            m_bStop = true;
            Thread.Sleep(10);
        }

    }


    #endregion




    #region Button event


    //////////////////////////////////////////////////////////////
    //btnStartReading_Click /////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - 읽기를 시작한다.
    //
    private void btnStartReading_Click(object sender, EventArgs e)
    {


        try
        {


            if (m_bIsOperating == false)
            {
                //화면 정리.
                btnStartReading.Enabled = false;

                //Display~~
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }



        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    //////////////////////////////////////////////////////////////
    //btnStopReading_Click /////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - 멈춘다.
    //
    private void btnStopReading_Click(object sender, EventArgs e)
    {

        if (m_bIsOperating == true)
        {

            m_bStop = true;
            Thread.Sleep(10);

            btnStartReading.Enabled = true;

        }

    } 


    #endregion



}
