using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Neon.Aligner;
using Free302.TnM.Device;
using Free302.MyLibrary.Utility;
using System.Reflection;

public partial class InitForm : Form, IInitForm
{

    #region ==== Constructor / Destructor ====

    public InitForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        try
        {
            base.OnLoad(e);
            loadConfig();
            displayConfig(false);

            chkTlsPmMode.Visible = AppLogic.License.ShowDevUI;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{ex.Message}\n\n{ex.StackTrace}", "InitForm.OnLoad()");
        }
    }


    #endregion



    #region ==== Config ====

    XConfig mConfig;
    public SweepLogic SweepLogic { get { return CGlobal.sweepLogic; } }

    void display(string msg)
    {
        rtxtStatus.AppendText($"{msg}\n");
    }
    private void display(string item, bool bSuccessOK)
    {
        var res = bSuccessOK ? "PASS" : "FAIL";
        var msg = $"{item.PadRight(50, '.')} {res}";
        rtxtStatus.AppendText($"{msg}\n");

        if (bSuccessOK == false)
        {
            int nStartPos = rtxtStatus.Find(msg);
            rtxtStatus.Select(nStartPos, msg.Length);
            rtxtStatus.SelectionColor = Color.OrangeRed;
        }
        rtxtStatus.Refresh();
    }

    private bool loadConfig()
    {
        AppLogic.GetProductAndTime(Assembly.GetExecutingAssembly());

        mConfig = new XConfig(@"config\conf_init.xml");

        //---------- TLS & PM ----------
        CGlobal.Address.GpibBoard = mConfig.GetValue("GPIB_BOARD").To<int>();

        CGlobal.Address.GpibTLS = mConfig.GetValue("GPIB_TLS").To<int>();
        CGlobal.TlsType = (CGlobal._TlsType)mConfig.GetValue("TLS_TYPE", "1").To<int>();
        CGlobal.UsingTcpServer = CGlobal.TlsType == GlobalBase._TlsType.Server;
        CGlobal.TlsParam = ConfigTlsParam.Create(mConfig.GetValue("TLS_PARAM", "0;1260;1360;0.050;50"));

        CGlobal.Address.GpibOpm = mConfig.GetValue("GPIB_PM").To<int>();
        CGlobal.OpmType = mConfig.GetValue("OPM_TYPE", "1").To<int>();
        CGlobal.IsTlsPmDual = (CGlobal.TlsType == GlobalBase._TlsType.Agilent) && mConfig.GetValue("IsTlsFrame", "0").Contains("1");
        CGlobal.PmGain = mConfig.GetValue("POWERMETER_GAIN", "0;-20").Unpack<int>().ToArray();
        CGlobal.PmAlignGain = mConfig.GetValue("PM_ALIGN_GAIN", "0").To<int>();
        CGlobal.OpmAvgTimeUs = mConfig.GetValue("OPM_AVG_TIME_US", "100").To<int>();
        if (CGlobal.OpmType == 2) CGlobal.HsbOpmParam = mConfig.GetValue("OPM_HSB", "2;2;1.10;0.8;0.00005;3;1.34;0.8;0.00005").Unpack<decimal>().ToArray();


        //---------- OSW ----------
        CGlobal.Address.COM_Osw = mConfig.GetValue("COMPORT_OSW1").To<int>();
        CGlobal.OswAlignPort = mConfig.GetValue("OSW_ALIGN_PORT").To<int>();
        CGlobal.OswTlsPort = mConfig.GetValue("OSW_TLS_PORT").To<int>();
        CGlobal.UsingOsw = mConfig.GetValue("OSW_Using").Contains("1");
        CGlobal.OswIsNeon = mConfig.GetValue("OSW_IS_NEON").Contains("1");

        //---------- Stage ----------
        CGlobal.McType = mConfig.GetValue("MC_Type").To<CGlobal.MC>();

        if (CGlobal.McType == CGlobal.MC.Suruga)
        {
            var suruga = mConfig.GetValue("Suruga_Left").Unpack<int>().ToArray();
            CGlobal.Address.COM_LeftMc1 = suruga[0];
            CGlobal.Address.COM_LeftMc2 = suruga[1];
            CGlobal.Address.COM_LeftMc3 = suruga[2];
            suruga = mConfig.GetValue("Suruga_Right").Unpack<int>().ToArray();
            CGlobal.Address.COM_RightMc1 = suruga[0];
            CGlobal.Address.COM_RightMc2 = suruga[1];
            CGlobal.Address.COM_RightMc3 = suruga[2];
            CGlobal.Address.COM_OtherMc = mConfig.GetValue("Suruga_Other").Unpack<int>().First();
        }
        CGlobal.UsingSurugaOther = mConfig.GetValue("Using_Suruga_Other").Contains("1");
        if (CGlobal.UsingSurugaOther) CGlobal.Address.COM_OtherMc = mConfig.GetValue("Suruga_Other").Unpack<int>().First();

        var axisInfo = mConfig.GetValue("Center_Info").Unpack<string>().ToArray();
        CGlobal.UsingCenterAxis = axisInfo[0].Contains("1");
        CGlobal.CenterAxis = axisInfo[1].Contains("X") ? (int)AlignAxis.X : (int)AlignAxis.Y;
        CGlobal.CenterDirection = axisInfo[2].Contains("F") ? 1 : -1;
        axisInfo = mConfig.GetValue("Camera_Info").Unpack<string>().ToArray();
        CGlobal.UsingCameraAxis = axisInfo[0].Contains("1");
        CGlobal.CameraAxis = axisInfo[1].Contains("X") ? (int)AlignAxis.X : (int)AlignAxis.Y;
        CGlobal.CameraDirection = axisInfo[2].Contains("F") ? 1 : -1;

        //DAQ BASE PARAM
        _daqBaseParam = mConfig.GetValue("DAQ_BASE_PARAM", "10000;100").Unpack<int>().ToArray();


        //---------- DAQ for DistanceSensor [Analog in/out] ---------
        CGlobal.DaqAiParam.Chs = mConfig.GetValue("DAQ_AICH_ADDRESS").Unpack<int>().ToArray();
        CGlobal.DaqAiParam.IsRSE = mConfig.GetValue("DAQ_AICH_RSE").Contains("1");
        CGlobal.DaqAiParam.VoltRange = mConfig.GetValue("DAQ_AICH_VOLT").Unpack<double>().ToArray();
        CGlobal.IsSfac = mConfig.GetValue("DAQ_IS_SFAC").Contains("1");

        //---------- DAQ for Air valva [Digital in/out] ----------
        CGlobal.UsingAir = mConfig.GetValue("DAQ_AIR_Using").Contains("1");
        CGlobal.DaqforAirParam.av1 = mConfig.GetValue("DAQ_AIR_LEFT").To<int>();
        CGlobal.DaqforAirParam.av2 = mConfig.GetValue("DAQ_AIR_RIGHT").To<int>();

        //---------- DAQ for UV [Digital out] ----------
        var uv = mConfig.GetValue("DAQ_UV").Unpack<int>().ToArray();
        CGlobal.UsingUv = uv[0] == 1;
        CGlobal.DaqParamForUV.dev = uv[1];
        CGlobal.DaqParamForUV.port = uv[2];
        CGlobal.DaqParamForUV.line = uv[3];

        return true;
    }

    private void displayConfig(bool throwEx = true)
    {
        const int width = -15;
        //display config
        display($"{"App Version:",width}\t{AppLogic.License.Version}");

        using (var w = new StreamReader(@".\config\ServerConfig.txt"))
        {
            display($"{"TLS Server:",width}\t{w.ReadLine()}:{w.ReadLine().Trim()}");
            display($"{"TLS Client:",width}\t{w.ReadLine().Trim()}");
            w.Close();
        }

        display($"{"TLS Sweep:",width}\t{CGlobal.TlsParam.ToString()}");
        display($"{"OPM Gain:",width}\t{CGlobal.PmGain[0]} {CGlobal.PmGain[1]}");
    }

    #endregion



    #region ==== Initialization ====

    int[] _daqBaseParam = new int[] { 10000, 100 };

    private bool InitDevices()
    {

        //Variables..
        bool bRet = true;

        int gpibBoard = CGlobal.Address.GpibBoard;

        #region ------------------- TLS ------------------- 

        if (!CGlobal.UsingTcpServer)
        {
            try
            {
                if (CGlobal.TlsType == GlobalBase._TlsType.Agilent)
                {
                    var tls = new C8164();
                    if (tls.Connect(gpibBoard, CGlobal.Address.GpibTLS) == true)
                    {
                        tls.Init(true, CGlobal.IsTlsPmDual, CGlobal.PmGain[0], CGlobal.TlsParam, CGlobal.OpmAvgTimeUs);
                        CGlobal.Tls = tls;
                        display("TLS:8164 ", true);
                    }
                }
                else if (CGlobal.TlsType == GlobalBase._TlsType.Santec)
                {
                    var tls = new SantecTls();
                    if (tls.ConnectByGpib(CGlobal.Address.GpibTLS) == true)
                    {
                        tls.Init(CGlobal.TlsParam);
                        CGlobal.Tls = tls;
                        display("TLS:Santec ", true);
                    }
                }
                else
                {
                    CGlobal.Tls = new TestTls();
                    display("TLS:TEST ", true);
                }
            }
            catch
            {
                display("TLS ", false);
            }
        }

        #endregion


        #region ------------------- DAQ ------------------------

        var daq = new DrBae.TnM.Device.DaqBase("HsbDaq", true, _daqBaseParam);
        daq.IsTestMode = _isTestMode;
        try
        {
            daq.Open();
            CGlobal.DaqBase = daq;
            CGlobal.DAQ = new Daq();
            display("Creating DAQ ", true);
        }
        catch
        {
            CGlobal.DAQ = null;
            display("Creating DAQ ", false);
            Thread.Sleep(1000);
            bRet = false;
        }


        #endregion


        #region ------------------- PM ------------------- 

        try
        {
            if (_isTestMode)
            {
                var opm = new TestOpm();
                opm.Init();
                CGlobal.Opm = opm;
            }
            else
            {
                if (CGlobal.OpmType == 1)
                {
                    var pm = new C8164();
                    if (pm.Connect(gpibBoard, CGlobal.Address.GpibOpm) == true)
                    {
                        pm.Init(false, CGlobal.IsTlsPmDual, CGlobal.PmGain[0], CGlobal.TlsParam, CGlobal.OpmAvgTimeUs);
                        if (CGlobal.PmGain[0] != CGlobal.PmAlignGain) pm.SetGainLevel(CGlobal.PmAlignGain);

                        CGlobal.Opm = pm;
                        display("OPM: 8164 ", true);
                    }
                }
                else if (CGlobal.OpmType == 2)
                {
                    var pm = new HsbOpm();
                    pm.Init(daq, CGlobal.HsbOpmParam);
                    CGlobal.Opm = pm;
                    display("OPM: HsbOpm ", true);
                }
                else if (CGlobal.OpmType == 3)
                {
                    //Santec PowerMeter
                    var pm = new SantecPm();
                    if (pm.Connect(gpibBoard, CGlobal.Address.GpibOpm) == true)
                    {
                        pm.Init(false, CGlobal.IsTlsPmDual, CGlobal.PmGain[0], CGlobal.TlsParam);
                        if (CGlobal.PmGain[0] != CGlobal.PmAlignGain) pm.SetGainLevel(CGlobal.PmAlignGain);

                        CGlobal.Opm = pm;
                        display("OPM: SantecPm ", true);
                    }
                }
                else throw new Exception($"Unknown power meter: OpmType={CGlobal.OpmType}");
            }
        }
        catch
        {
            display("OPM ", false);
        }

        #endregion



        if (CGlobal.IsTlsPmMode) return bRet;//[TLS & PM만 사용 모드 적용시 - 기타 장비 초기화 생략]



        #region ------------------- S-fac Serial ------------------------

        var sfac = new SfacSerial();
        if (CGlobal.IsSfac)
        {
            sfac.Config();
            sfac.Open();

            //Air Valve
            CGlobal.AirValve = sfac;

            display("SFAC Serial ", true);
        }

        #endregion



        #region ------------------- Distance sensor ---------------------

        if (CGlobal.IsSfac)
        {
            if (sfac.InitDistanceSensor(CGlobal.DAQ, CGlobal.DaqAiParam.Chs, CGlobal.DaqAiParam.VoltRange, CGlobal.DaqAiParam.IsRSE))
            {
                display("Distance Sensor ", true);
                CGlobal.Ds2000 = sfac;
            }
            else
            {
                CGlobal.Ds2000 = null;
                display("Distance Sensor ", false);
                Thread.Sleep(1000);
                bRet = false;
            }
        }
        else
        {
            //var ds = new DS2000();
            var ds = new DsNew(); //new DS2000();
            //var ds = new TestDs(); //new DS2000();
            //if (ds.Init(CGlobal.DAQ, CGlobal.DaqAiParam.Chs, CGlobal.DaqAiParam.VoltRange, CGlobal.DaqAiParam.IsRSE))
            if (ds.Init(daq, CGlobal.DaqAiParam.IsRSE, CGlobal.DaqAiParam.Chs, CGlobal.DaqAiParam.VoltRange))
            {
                display("Distance Sensor ", true);
                CGlobal.Ds2000 = ds;
            }
            else
            {
                CGlobal.Ds2000 = null;
                display("Distance Sensor ", false);
                Thread.Sleep(1000);
                bRet = false;
            }
        }

        #endregion



        #region ------------------- Air valve controller ---------------------

        if (CGlobal.UsingAir)
        {
            if (CGlobal.IsSfac) CGlobal.AirValve = sfac;
            else
            {
                var avc = new CairValveCont();

                if (avc.Init(CGlobal.DAQ, CGlobal.DaqforAirParam.av1, CGlobal.DaqforAirParam.av2))
                {
                    display("Air Valve Contoller ", true);
                    CGlobal.AirValve = avc;
                }
                else
                {
                    CGlobal.AirValve = null;
                    display("Air Valve Contoller ", false);
                    Thread.Sleep(1000);
                    CGlobal.AirValve = null;
                    bRet = false;
                }
            }
        }

        #endregion



        #region ------------------- Ushio UV Cure ---------------------

        if (CGlobal.UsingUv)
        {
            bool uvResult = false;
            try
            {
                var uv = new UshioUvNew();//UshioUvCure();
                uvResult = uv.Init(daq, CGlobal.DaqParamForUV.dev, CGlobal.DaqParamForUV.port, CGlobal.DaqParamForUV.line);
                //uvResult = uv.Init(CGlobal.DAQ, CGlobal.DaqParamForUV.dev, CGlobal.DaqParamForUV.port, CGlobal.DaqParamForUV.line);
                CGlobal.UshioUvCure = uv;
            }
            catch (Exception ex)
            {
                CGlobal.UshioUvCure = null;
                Log.Write($"InitForm.InitDevices().UV:\n{ex.Message}");
            }

            display("Ushio UV ", uvResult);
        }

        #endregion



        #region  ------------------- Optical Swith 1 ------------------- 

        if (CGlobal.UsingOsw)
        {
            try
            {
                Osw osw1 = CGlobal.osw;

                if (osw1 == null)
                {
                    osw1 = new Osw(CGlobal.OswAlignPort, CGlobal.OswTlsPort, CGlobal.OswIsNeon);
                    CGlobal.osw = osw1;
                }
                if (osw1.Connect(CGlobal.Address.COM_Osw) == true)
                {
                    display("OSW ", true);
                }
                else
                {
                    CGlobal.osw = null;
                    display("OSW ", false);
                    bRet = false;
                }
            }
            catch
            {
                display("OSW ", false);
                bRet = false;
            }
        }


        #endregion



        #region ------------------- Stage ------------------- 

        if (CGlobal.McType == CGlobal.MC.Suruga)
        {
            //**********[Suruga M.C.]**********
            bool resAligner = false;

            //*Left*
            var leftAligner = new SurugaAligner(6);
            try
            {
                resAligner = leftAligner.InitMc(mConfig.GetValue("Suruga_Left").Unpack<int>().ToArray(), @"config\conf_MC_Suruga_Left.xml");
            }
            catch (Exception ex)
            {
                if (leftAligner != null) display($"{leftAligner} : {ex.Message}");
                else display($"LEFT: {ex.Message}");
                leftAligner = null;
            }
            CGlobal.LeftAligner = leftAligner;

            if (resAligner)
            {
                display("Left-Stage ", true);
            }
            else
            {
                CGlobal.LeftAligner = null;
                display("Left-Stage ", false);
                bRet = false;
            }

            //*Right*
            resAligner = false;
            var rightAligner = new SurugaAligner(6);
            try
            {
                resAligner = rightAligner.InitMc(mConfig.GetValue("Suruga_Right").Unpack<int>().ToArray(), @"config\conf_MC_Suruga_Right.xml");
            }
            catch (Exception ex)
            {
                if (rightAligner != null) display($"{rightAligner} : {ex.Message}");
                else display($"RIGHT: {ex.Message}");
                rightAligner = null;
            }
            CGlobal.RightAligner = rightAligner;

            if (resAligner)
            {
                display("Right-Stage ", true);
            }
            else
            {
                CGlobal.RightAligner = null;
                display("Right-Stage ", false);
                bRet = false;
            }

            //*Other*
            resAligner = false;
            if (CGlobal.UsingCenterAxis || CGlobal.UsingCameraAxis)
            {
                var otherAligner = new SurugaAligner(2);
                try
                {
                    resAligner = otherAligner.InitMc(mConfig.GetValue("Suruga_Other").Unpack<int>().ToArray(), @"config\conf_MC_Suruga_Other.xml");
                }
                catch (Exception ex)
                {
                    if (otherAligner != null) display($"{otherAligner} : {ex.Message}");
                    else display($"Other: {ex.Message}");
                    otherAligner = null;
                }
                CGlobal.OtherAligner = otherAligner;

                if (resAligner)
                {
                    display("Other-Stage ", true);
                }
                else
                {
                    CGlobal.RightAligner = null;
                    display("Other-Stage ", false);
                    bRet = false;
                }
            }
        }
        else
        {
            //**********[Autonics M.C. || Nova M.C.]**********
            //*Left*
            var leftAligner = new LeftAligner();
            try
            {
                leftAligner.Open();
            }
            catch (Exception ex)
            {
                if (leftAligner != null) display($"{leftAligner} : {ex.Message}");
                else display($"LEFT: {ex.Message}");
                leftAligner = null;
            }
            if (leftAligner?.IsConnectedOK() ?? false)
            {
                CGlobal.LeftAligner = leftAligner;
                display("LeftAligner ", true);
            }
            else
            {
                CGlobal.LeftAligner = null;
                display("LeftAligner ", false);
                bRet = false;
            }

            //*Right*
            var rightAligner = new RightAligner();
            try
            {
                rightAligner.Open();
            }
            catch (Exception ex)
            {
                if (rightAligner != null) display($"{rightAligner} : {ex.Message}");
                else display($"RIGHT: {ex.Message}");
                rightAligner = null;
            }
            if (rightAligner?.IsConnectedOK() ?? false)
            {
                CGlobal.RightAligner = rightAligner;
                display("RightAligner ", true);
            }
            else
            {
                CGlobal.RightAligner = null;
                display("RightAligner ", false);
                bRet = false;
            }

            //*Other*
            if (!CGlobal.UsingSurugaOther && (CGlobal.UsingCenterAxis || CGlobal.UsingCameraAxis))
            {
                var otherAligner = new CenterAligner();
                try
                {
                    otherAligner.Open();
                }
                catch (Exception ex)
                {
                    if (otherAligner != null) display($"{otherAligner}: {ex.Message}");
                    else display($"{ex.Message}");
                    otherAligner = null;
                }
                if (otherAligner?.IsConnectedOK() ?? false)
                {
                    CGlobal.OtherAligner = otherAligner;
                    display("CenterAligner ", true);
                }
                else
                {
                    CGlobal.OtherAligner = null;
                    display("CenterAligner ", false);
                    bRet = false;
                }
            }
        }

        //************* [Suruga Other Aligner] *************
        if (CGlobal.UsingSurugaOther)
        {
            var surugaConfig = @"config\conf_MC_Suruga_Other.xml";

            if (!CGlobal.IsSfac)
            {
                var aligner = new SurugaAligner(2);
                aligner.stageNo = 9;//
                aligner.InitMc(new int[] { CGlobal.Address.COM_OtherMc }, surugaConfig);
                if (aligner?.IsConnectedOK() ?? false)
                {
                    CGlobal.OtherAligner = aligner;
                    display("Other Aligner (Suruga) ", true);
                }
                else
                {
                    CGlobal.OtherAligner = null;
                    display("OtherAligner (Suruga) ", false);
                    bRet = false;
                }
            }
            else
            {
                var aligner = new SfacAligner(CGlobal.Address.COM_OtherMc, surugaConfig);
                if (aligner?.IsConnectedOK() ?? false)
                {
                    CGlobal.OtherAligner = aligner;
                    CGlobal.CenterAxis = aligner.AXIS_X;          //Center : X축, Suruga
                    CGlobal.CameraAxis = aligner.AXIS_Y;          //Camera : Y축, Nova
                    display("OtherAligner (SFAC) ", true);
                }
                else
                {
                    CGlobal.OtherAligner = null;
                    display("OtherAligner (SFAC) ", false);
                    bRet = false;
                }
            }
        }

        #endregion


        return bRet;

    }



    private bool InitSystem()
    {
        bool bRet = true;

        //--------------- Sweep system  --------------
        try
        {
            SweepLogic swpSys = CGlobal.sweepLogic;
            if (swpSys == null)
            {
                swpSys = new SweepLogic(CGlobal.Tls, CGlobal.Opm);
                CGlobal.sweepLogic = swpSys;
            }

            if (swpSys.TcpServer_Init(CGlobal.UsingTcpServer))
            {
                display("Sweep Logic ", true);
            }
            else
            {
                display("Sweep Logic ", false);
                bRet = false;
            }

        }
        catch
        {
            display("Sweep Logic ", false);
            Thread.Sleep(1000);
            bRet = false;
        }

        if (CGlobal.IsTlsPmMode) return bRet;//***************************************************************


        //-------------- Alignment ---------------------
        Istage leftStg = CGlobal.LeftAligner;
        Istage rightStg = CGlobal.RightAligner;
        IoptMultimeter mpm = CGlobal.Opm;
        var ds = CGlobal.Ds2000;

        if ((leftStg != null) && (rightStg != null) && (ds != null))
        {
            CGlobal.alignLogic = new AlignLogic(leftStg, rightStg, ds, mpm);
            ScanParam.FinePeakStep = 0.25;
            AlignLogic.IsTestMode = _isTestMode;
            display("Align Logic ", true);
        }
        else
        {
            bRet = false;
            display("Align Logic ", false);
        }

        return bRet;
    }

    #endregion



    #region ==== Ui Button ====

    private void btnInit_Click(object sender, EventArgs e)
    {
        try
        {
            rtxtStatus.Clear();

            Cursor = Cursors.WaitCursor;
            CGlobal.InitOK = true;

            loadConfig();
            displayConfig();
            display("");

            CGlobal.IsTlsPmMode = chkTlsPmMode.Checked;         //TLS PM mode

            CGlobal.InitOK &= InitDevices();
            CGlobal.InitOK &= InitSystem();

            Log.Write($"\n{rtxtStatus.Text}", "init", true);

            if (CGlobal.InitOK) Close();
        }        
        catch (Exception ex)
        {
            display($"InitForm:\n{ex.Message}");
            if (ex.InnerException != null) display($"\n{ex.InnerException.Message}");
#if DEBUG
            if (ex.InnerException != null) display($"\n\n{ex.InnerException.StackTrace}");
            display($"\n{ex.StackTrace}");
#endif
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        Program.ExitApp = true;
        this.Close();
        Application.Exit();
    }

    private void btn계속_Click(object sender, EventArgs e)
    {
        try
        {
            Close();
        }
        catch (Exception)
        {
            //display(ex.Message, false);
            Program.ExitApp = true;
            Close();
            Application.Exit();
        }
    }

    #endregion

    private void InitForm_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.F12)
        {
            _isTestMode = true;
            CGlobal.IsTestMode = _isTestMode;

            btnInit.ForeColor = Color.OrangeRed;
            btnInit.PerformClick();
        }
    }
    bool _isTestMode = false;
}
