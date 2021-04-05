using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Neon.Aligner;
using System.Collections.Generic;
using System.Linq;
using PwUnit = Free302.TnM.DataAnalysis.Unit;
using TnM.Utility;

public partial class OpmDisplayForm : Form
{
    #region definition

    private const int RESMW = 7;             // 10^(-4) mW
    private const int RESDBM = 3;            // 10^(-3) dBm

    string mConfFilepath = Application.StartupPath + @"\config\config_DigiOptPwr.xml";

    #endregion



    #region private member

    private IoptMultimeter m_mpm;
    private AutoResetEvent m_AutoEvent;
    private Thread m_thread;
    private bool m_bStop = false;
    private bool m_bIsOperating = false;
    double mShiftValueMW = Math.Pow(10, -0.7);
    bool mDoShift = false;

    public enum Unit { dBm, dB }
    public enum DisplayLine { L1, L2 }

    Unit _unit = Unit.dBm;
    Dictionary<DisplayLine, TextBox> _valueUi;  //각 display 라인별 power 출력 TextBox
    Dictionary<DisplayLine, ComboBox> _portUi;  //각 display 라인별 opm port 번호 출력 ComboBox
    Dictionary<DisplayLine, int> _ports;        //각 display 라인별 opm port 번호
    Dictionary<int, double> _refValue;          //각 opm port별 reference power
    Dictionary<DisplayLine, TextBox> _refUi;    //각 display 라인별 reference power 출력 TextBox

    #endregion




    #region constructor/destructor

    public OpmDisplayForm()
    {
        InitializeComponent();

        mDoShift = SecurityControl.DoShift;

        _ports = new Dictionary<DisplayLine, int>();
        _valueUi = new Dictionary<DisplayLine, TextBox> { { DisplayLine.L1, uiValue1 }, { DisplayLine.L2, uiValue2 } };
        _portUi = new Dictionary<DisplayLine, ComboBox> { { DisplayLine.L1, uiPort1 }, { DisplayLine.L2, uiPort2 } };

        _refValue = new Dictionary<int, double>();
        _refUi = new Dictionary<DisplayLine, TextBox> { { DisplayLine.L1, uiRef1 }, { DisplayLine.L2, uiRef2 } };
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        try
        {
            Location = LoadWndStartPos(mConfFilepath);

            setPM(m_mpm ?? GlobalBase.Opm);
            using (XConfig conf = new XConfig(mConfFilepath))
            {
                uiPort1.SelectedItem = (DisplayCh)int.Parse(conf.GetValue("CH_FIRST"));
                uiPort2.SelectedItem = (DisplayCh)int.Parse(conf.GetValue("CH_SECOND"));
                setUnit(conf.GetValue("UNIT", "dBm").To<Unit>());

                _refValue = conf.GetValue("REF_VALUES", "1~-12.345 2~-12.345").Unpack<int, double>();
                foreach (var line in _ports.Keys) _refUi[line].Text = _refValue[_ports[line]].ToString();
                foreach(var p in m_mpm.ChList)
                {
                    var port = (int)p;
                    if (!_refValue.ContainsKey(port)) _refValue[port] = 0;
                }
            }

            //member variables 초기화!!
            m_AutoEvent = new AutoResetEvent(false);

            //Thread를 동작 시킨다.!!
            m_thread = new Thread(ThreadFunc);
            m_thread.IsBackground = true;
            m_thread.Name = "OPM";
            m_thread.Start();
            setOnOff(false);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);

        SaveWndStartPos(mConfFilepath);
        using (XConfig conf = new XConfig(mConfFilepath))
        {
            conf.SetValue("CH_FIRST", _ports[DisplayLine.L1].ToString());
            conf.SetValue("CH_SECOND", _ports[DisplayLine.L2].ToString());
            conf.SetValue("UNIT", _unit.ToString());
            conf.SetValue("REF_VALUES", _refValue.Pack());
            conf.Save();
        }

        //쓰레드 종료!!
        if (m_thread != null)
        {
            m_bStop = true;
            Thread.Sleep(10);

            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        //Event 객체 제거..
        if (m_AutoEvent != null) m_AutoEvent.Dispose();
        m_AutoEvent = null;
    }


    #endregion




    #region Thread Function


    const double _MinMw = 1e-7;

    private void ThreadFunc()
    {
        double[] dbPwrArr = new double[2];
        double dbPwrChFirst = 0;
        double dbPwrChSecond = 0;

        while (true)
        {
            m_AutoEvent.WaitOne();
            m_bIsOperating = true;

            while (true)
            {
                if (m_bStop) break;

                try
                {
                    dbPwrChFirst = m_mpm.ReadPower(_ports[DisplayLine.L1]);      //[mW]
                    dbPwrChSecond = m_mpm.ReadPower(_ports[DisplayLine.L2]);    //[mW]
                    if (dbPwrChFirst < _MinMw) dbPwrChFirst = _MinMw;
                    if (dbPwrChSecond < _MinMw) dbPwrChSecond = _MinMw;
                }
                catch
                {
                    dbPwrChFirst = _MinMw;
                    dbPwrChSecond = _MinMw;
                }

                if (mDoShift)
                {
                    dbPwrChFirst *= mShiftValueMW;
                    dbPwrChSecond *= mShiftValueMW;
                }

                //mW -> dBm
                dbPwrChFirst = Math.Round(PwUnit.MillWatt2Dbm(dbPwrChFirst), RESDBM);
                dbPwrChSecond = Math.Round(PwUnit.MillWatt2Dbm(dbPwrChSecond), RESDBM);

                //단위 변환
                if (_unit == Unit.dB)//dB
                {
                    dbPwrChFirst -= _refValue[_ports[DisplayLine.L1]];
                    dbPwrChSecond -= _refValue[_ports[DisplayLine.L2]];
                }

                dbPwrChFirst = Math.Round(dbPwrChFirst, RESDBM);
                dbPwrChSecond = Math.Round(dbPwrChSecond, RESDBM);

                //출력!! 
                Invoke((Action)(() => _valueUi[DisplayLine.L1].Text = $"{dbPwrChFirst:F03}"));
                Invoke((Action)(() => _valueUi[DisplayLine.L2].Text = $"{dbPwrChSecond:F03}"));

                Thread.Sleep(250);
            }

            m_bIsOperating = false;
            m_bStop = false;
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

    #endregion



    #region ---- Ch ----

    enum DisplayCh { CH1 = 1, CH2, CH3, Ch4, CH5, CH6, CH7, CH8 }
    void setPM(IoptMultimeter pm)
    {
        if (pm == null) return;
        m_mpm = pm;

        var chs = pm.ChList.Select(x=>(DisplayCh)x).ToArray();//int array
        var chObjects = chs.Select(x => (object)x).ToArray();

        foreach (var cb in _portUi.Values)
        {
            cb.Items.Clear();
            cb.Items.AddRange(chObjects);
            if (chs.Length > 0) cb.SelectedIndex = 0;
        }

        foreach (var p in _portUi.Keys) _ports[p] = (int)_portUi[p].SelectedItem;
    }

    private void cbChFirst_SelectedIndexChanged(object sender, EventArgs e) => _ports[DisplayLine.L1] = (int)uiPort1.SelectedItem;
    private void cbChSecond_SelectedIndexChanged(object sender, EventArgs e) => _ports[DisplayLine.L2] = (int)uiPort2.SelectedItem;
    public void SetPort(DisplayLine line, int opmPort)
    {
        if (!InvokeRequired) _portUi[line].SelectedItem = (DisplayCh)opmPort;
        else Invoke((Action)(_portUi[line].SelectedItem = (DisplayCh)opmPort));
    }
	public void SetFirstCh(int nPort) => SetPort(DisplayLine.L1, nPort);
    public void SetSecCh(int nPort) => SetPort(DisplayLine.L2, nPort);
    
    #endregion



    #region ---- Unit ----

    private void btnUnit_Click(object sender, EventArgs e) => setUnit(_unit == Unit.dBm ? Unit.dB : Unit.dBm);

    public void SetUnit(Unit unit)
    {
        if (!InvokeRequired) setUnit(unit);
        else Invoke((Action)(() => setUnit(unit)));
    }
    void setUnit(Unit unit)
    {
        _unit = unit;
        btnUnit.Text = _unit.ToString();
        btnUnit.ForeColor = _unit == Unit.dB ? Color.DodgerBlue : Color.Black;
        uiRef1.Enabled = uiRef2.Enabled = _unit == Unit.dB;
    } 

    #endregion



    #region ---- On Off ----

    private void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            setOnOff(!m_bIsOperating);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private void setOnOff(bool on)
    {
        m_bIsOperating = on;
        m_bStop = !on;
        btnStart.ForeColor = m_bIsOperating ? Color.LightGreen : Color.Black;
        btnStart.BackColor = m_bIsOperating ? Color.White : SystemColors.Control;
        btnStart.Text = m_bIsOperating ? "ON" : "OFF";
        if (m_bIsOperating)
        {
            m_AutoEvent.Set();
            Thread.Sleep(10);
        }
        Refresh();
    }
    public void DisplayOn()
    {
        if (!InvokeRequired) setOnOff(true);
        else Invoke((Action)(() => setOnOff(true)));
    }
    public void DisplayOff()
    {
        if (!InvokeRequired) setOnOff(false);
        else Invoke((Action)(() => setOnOff(false)));
    }

    #endregion



    #region ---- REF ----

    private void uiSetRef1_Click(object sender, EventArgs e) => measureRefValueUi(sender, DisplayLine.L1);
    private void uiSetRef2_Click(object sender, EventArgs e) => measureRefValueUi(sender, DisplayLine.L2);
    private void measureRefValueUi(object sender, DisplayLine line)
    {
        var ui = (Label)sender;
        try
        {
            ui.Enabled = false;
            measureRefValue(line);
        }
        finally
        {
            ui.Enabled = true;
        }
    }
    private void measureRefValue(DisplayLine line)
    {
        if (m_mpm == null) return;

        var port = _ports[line];

        var running = m_bIsOperating;
        try
        {
            Cursor = Cursors.WaitCursor;
            if (running) DisplayOff();
            
            //(m_mpm as TestOpm)?.SetTestValue(0.7);//TEST: ref

            var mw = 0.0;
            for (int i = 0; i < 5; i++) mw += m_mpm.ReadPower(port);
            mw /= 5;
            if (mw < _MinMw) mw = _MinMw;
            var dbm = Math.Round(PwUnit.MillWatt2Dbm(mw), RESDBM);
            _refValue[port] = dbm;
            _refUi[line].Text = dbm.ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"OpmDisplayForm.setRefValue({line}): {ex.Message}");
        }
        finally
        {
            //(m_mpm as TestOpm)?.SetTestValue((0.7 - port / 20.0) / 2 + port / 20.0);
            //(m_mpm as TestOpm)?.SetTestValue();
            if (running) DisplayOn();
            Cursor = Cursors.Default;
        }
    }

    private void uiRef1_TextChanged(object sender, EventArgs e) => setUi2Ref(DisplayLine.L1);
    private void uiRef2_TextChanged(object sender, EventArgs e) => setUi2Ref(DisplayLine.L2);
    private void setUi2Ref(DisplayLine line)
    {
        var port = _ports[line];
        var ui = _refUi[line];
        var v = _refValue[port];
        if (double.TryParse(ui.Text, out v)) _refValue[port] = v;
    }

    #endregion



}//class


