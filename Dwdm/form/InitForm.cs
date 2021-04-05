using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Neon.Aligner;
using Free302.TnM.Device;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;
using Calignment = Neon.Aligner.AlignLogic;
using Agilent8169A = Neon.Aligner.Agilent8169;


public partial class InitForm : Form
{

    const string CONFIG_FILE_NAME = "config_initialization.xml";
    string mConfigFile;
    XConfig mConfig;

    public InitForm()
    {
        try
        {
            InitializeComponent();

            XConfig.mConfigFolder = "config";
            mConfigFile = Path.Combine(Application.StartupPath, CONFIG_FILE_NAME);
            mConfig = new XConfig(mConfigFile);

            loadConfig();
            displayConfig();

			this.Shown += form_Shown;
			this.Load += form_Load;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"InitForm.InitForm():\n{ex.Message}");
        }
    }

	private void form_Load(object sender, EventArgs e)
	{
		uiTlsWaveShiftPm.Text = CGlobal.TlsWaveShift_pm.ToString();
		uiDoTlsWaveShift.Checked = CGlobal.TlsWaveShift_pm != 0;
	}

	private void form_Shown(object sender, EventArgs e)
    {
        doNotAsk = uiDoNotAsk.Checked;
        if (doNotAsk) btnInit.PerformClick();
    }



    private void loadConfig()
    {
        //-------------------- OSW -------------------- 
        GlobalAddress.COM_Osw = int.Parse(mConfig.GetValue("COMPORT_OSW1"));
        GlobalAddress.OswAlignPort = int.Parse(mConfig.GetValue("OSW_ALIGN_PORT"));
        GlobalAddress.OswTlsPort = int.Parse(mConfig.GetValue("OSW_TLS_PORT"));

        //--------------------  Analog Input -------------------- 
        GlobalAddress.AI_Left = int.Parse(mConfig.GetValue("DAQ_AINO_DS2000_LEFT"));
        GlobalAddress.AI_Right = int.Parse(mConfig.GetValue("DAQ_AINO_DS2000_RIGHT"));
        CGlobal.Ds2000_Terminal_RSE = mConfig.GetValue("DAQ_AICH_RES").Contains("1");
        CGlobal.Ds2000_VoltageRange =  mConfig.GetValue("DAQ_AICH_VOLT").Unpack<double>().ToArray();
        CGlobal.IsSfac = mConfig.GetValue("DAQ_IS_SFAC").To<bool>();

        //-------------------- Device Connection -------------------- 
        //Motion Controller
        var mcConfig = mConfig.GetValue("MC_Type").ToLower();
        if (mcConfig == CGlobal.MC.Suruga.ToString().ToLower()) CGlobal.McType = CGlobal.MC.Suruga;
        else if (mcConfig == CGlobal.MC.Autonics.ToString().ToLower()) CGlobal.McType = CGlobal.MC.Autonics;
        else CGlobal.McType = CGlobal.MC.Nova;

        GlobalAddress.COM_LeftAxis1 = int.Parse(mConfig.GetValue("COMPORT_L1STAGE"));
        GlobalAddress.COM_LeftAxis2 = int.Parse(mConfig.GetValue("COMPORT_L2STAGE"));
        GlobalAddress.COM_LeftAxis3 = int.Parse(mConfig.GetValue("COMPORT_L3STAGE"));
        GlobalAddress.COM_RightAxis1 = int.Parse(mConfig.GetValue("COMPORT_R1STAGE"));
        GlobalAddress.COM_RightAxis2 = int.Parse(mConfig.GetValue("COMPORT_R2STAGE"));
        GlobalAddress.COM_RightAxis3 = int.Parse(mConfig.GetValue("COMPORT_R3STAGE"));
        GlobalAddress.COM_OtherAxis1 = int.Parse(mConfig.GetValue("COMPORT_OSTAGE"));

        //-------------------- TLS -------------------
        GlobalAddress.GPIB_Board = int.Parse(mConfig.GetValue("GPIB_BOARD"));
        GlobalAddress.GPIB_AgilentTls = int.Parse(mConfig.GetValue("GPIBADDR_TLS"));
        CGlobal.TlsSlot = int.Parse(mConfig.GetValue("TLS_SLOT"));
		CGlobal.TlsWaveShift_pm = int.Parse(mConfig.GetValue("TLS_WAVE_SHIFT_PICOMETER"));

		switch (Neon.Dwdm.Properties.Settings.Default.init_TLS_Type)
		{
			case 0:
				uiTlsLocal.Checked = true;
				break;
			case 1:
				uiTlsTcpServer.Checked = true;
				break;
		}

		//-------------------- PM -------------------- 
		GlobalAddress.GPIB_AgilentPm1 = int.Parse(mConfig.GetValue("GPIBADDR_PM1"));
        GlobalAddress.GPIB_AgilentPm2 = int.Parse(mConfig.GetValue("GPIBADDR_PM2"));
        CGlobal.PmPort1 = MyLogic.parseIntArray(mConfig.GetValue("PM1_PORT"));
        CGlobal.PmPort2 = MyLogic.parseIntArray(mConfig.GetValue("PM2_PORT"));

        //-------------------- PC -------------------- 
        GlobalAddress.GPIB_Pc_Cband_8169A = int.Parse(mConfig.GetValue("GPIBADDR_8169A"));
        GlobalAddress.GPIB_Pc_Oband_PSG100 = int.Parse(mConfig.GetValue("GPIBADDR_PC_PSG100"));
		GlobalAddress.COM_Pc_Cband_NPC64C = int.Parse(mConfig.GetValue("COMPORT_NPC64_C"));
		GlobalAddress.COM_Pc_Oband_NPC64O = int.Parse(mConfig.GetValue("COMPORT_NPC64_O"));

		switch (Neon.Dwdm.Properties.Settings.Default.init_PC_Device)
		{
			case 0:
				uiPcNone.Checked = true;
				break;
			case 1:
				uiPc8169.Checked = true;
				break;
			case 2:
				uiPcNpcC.Checked = true;
				break;
			case 3:
				uiPcPsg100.Checked = true;
				break;
			case 4:
				uiPcNpcO.Checked = true;
				break;
		}

		//-------------------- Chamber ---------------
		GlobalAddress.GPIB_Chamber = int.Parse(mConfig.GetValue("GPIBADDR_CHAMBER"));
		CGlobal.mUsingChamber = mConfig.GetValue("CHAMBER_USING").To<bool>();

		const string sDefaultName = "DwdmMeasure";
        var mConfigInit = new ConfigBase(sDefaultName);
        mConfigInit.LoadConfig();
        CGlobal.TlsPower = mConfigInit.Get<int>("TLS_POWER_LEVEL");

		//-------------------- Ushio UV ---------------
		


	}


	private void displayConfig()
    {
        using (var w = new StreamReader(@".\config\ServerConfig.txt"))
        {
            string strText = "";
            strText += $"Local TLS      : {GlobalAddress.GPIB_AgilentTls}   <board : {GlobalAddress.GPIB_Board}>\n";
            strText += $"TCP server     : {w.ReadLine()}:{w.ReadLine()}\t{w.ReadLine()}\n";
			strText += $"TLS Power      : {CGlobal.TlsPower} [dBm]\n\n";

			strText += $"C_band 8169A   : {GlobalAddress.GPIB_Pc_Cband_8169A}\n";
            strText += $"O_band PSG100  : {GlobalAddress.GPIB_Pc_Oband_PSG100}\n";
			strText += $"C_band NPC64C  : {GlobalAddress.COM_Pc_Cband_NPC64C}\n";
			strText += $"O_band NPC64O  : {GlobalAddress.COM_Pc_Oband_NPC64O}\n\n";

			strText += $"Chamber        : {GlobalAddress.GPIB_Chamber}\t{(CGlobal.mUsingChamber ? "use" : "not use")}\n\n";

			strText += $"P.M1           : {GlobalAddress.GPIB_AgilentPm1}\n";
            strText += $"P.M1 Port      : ";
            foreach (var pm in CGlobal.PmPort1) strText += $"{pm}  ";
            strText += "\n";
            strText += $"P.M2           : {GlobalAddress.GPIB_AgilentPm2}\n";
            strText += $"P.M2 Port      : ";
            foreach (var pm in CGlobal.PmPort2) strText += $"{pm}  ";

            uiStatusText.AppendText(strText);
            w.Close();
        }
    }


    private bool initDevices()
    {
        //Variables..
        bool finalResult = true;
        bool result = false;


		#region ==== init Chamber ====

		//-------------------- Espec Chamber -------------------- 
		bool resChamber = false;
		try
		{
			if (CGlobal.mUsingChamber)
			{

				var chamber = new EspecChamber();
				var gpibConfig = new TnM.Device.GpibConfig("Chamber", GlobalAddress.GPIB_Board, GlobalAddress.GPIB_Chamber);
				gpibConfig.IsIeee4882Compatable = false;

				chamber.Config(gpibConfig);
				chamber.Open();
				CGlobal.EspecChamber = chamber;
				AppendSTextToStatus("Chamber");
			}
			else
			{
				CGlobal.EspecChamber = null;
			}
			resChamber = true;
		}
		catch { resChamber = false; }
		if (!resChamber) CGlobal.EspecChamber = null;
		finalResult &= resChamber;

		#endregion


		#region ==== S-fac Serial ===

		var sfac = new SfacSerial();
		if (CGlobal.IsSfac)
		{
			sfac.Config();
			sfac.Open();

			//Air Valve
			CGlobal.AirValve = sfac;
		}

		#endregion


		#region ===== init Distance Sensor =====

		//-------------------- DAQ -------------------- 
		CGlobal.Daq = new Daq();
		var resDaq = CGlobal.Daq != null;
        AppendSTextToStatus("DAQ", resDaq);
        finalResult &= resDaq;

		//-------------------- DS2000 -------------------- 
		bool resSensor = false;
		var chs = new int[] { GlobalAddress.AI_Left, GlobalAddress.AI_Right };

		if (CGlobal.IsSfac)
		{
			resSensor = sfac.InitDistanceSensor(CGlobal.Daq, chs, CGlobal.Ds2000_VoltageRange, CGlobal.Ds2000_Terminal_RSE);
			if (resSensor) CGlobal.Ds2000 = sfac;
		}
		else
		{
			var ds2000 = new DS2000();
			resSensor = ds2000.Init(CGlobal.Daq, chs, CGlobal.Ds2000_VoltageRange, CGlobal.Ds2000_Terminal_RSE);
			if (resSensor) CGlobal.Ds2000 = ds2000;
		}

		AppendSTextToStatus("Sensor", resSensor);
		finalResult &= resSensor;

		#endregion


		#region ===== init Stage =====
		bool resAlignerLeft = false;
		bool resAlignerRight = false;
		bool resAlignerOther = false;
		if (CGlobal.McType == CGlobal.MC.Suruga)
        {
            //-------------------- Left Stage -------------------- 
            var leftAligner = new SurugaAligner(6);
            if (true)
            {
                var ports = new int[] { GlobalAddress.COM_LeftAxis1, GlobalAddress.COM_LeftAxis2, GlobalAddress.COM_LeftAxis3 };
                if (doPassStage) resAlignerLeft = true;
                else			 resAlignerLeft = leftAligner.InitMc(ports, @"config\conf_MC_Suruga_Left.xml");
            }
            if (!resAlignerLeft) CGlobal.LeftAligner = null;
            AppendSTextToStatus("Left-Stage setup", resAlignerLeft);
            finalResult &= resAlignerLeft;
            CGlobal.LeftAligner = leftAligner;

            //-------------------- Right Stage -------------------- 
            var rightAligner = new SurugaAligner(6);
            if (true)
            {
                var ports = new int[] { GlobalAddress.COM_RightAxis1, GlobalAddress.COM_RightAxis2, GlobalAddress.COM_RightAxis3 };
                if (doPassStage) resAlignerRight = true;
                else			 resAlignerRight = rightAligner.InitMc(ports, @"config\conf_MC_Suruga_Right.xml");
            }
            if (!resAlignerRight) CGlobal.RightAligner = null;
            AppendSTextToStatus("Right-Stage setup", resAlignerRight);
            finalResult &= resAlignerRight;
            CGlobal.RightAligner = rightAligner;

            //-------------------- Other Stage -------------------- 
            var otherAligner = new SurugaAligner(2);
            if (true)
            {
                var ports = new int[] { GlobalAddress.COM_OtherAxis1 };
                if (doPassStage) resAlignerOther = true;
                else
                {
                    CGlobal.CameraAxis = mConfig.GetValue("CAMERA_AXIS").Contains("X") ? otherAligner.AXIS_X : otherAligner.AXIS_Y;
                    CGlobal.CenterAxis = mConfig.GetValue("CENTER_AXIS").Contains("Y") ? otherAligner.AXIS_Y : otherAligner.AXIS_X;
					resAlignerOther = otherAligner.InitMc(ports, @"config\conf_MC_Suruga_Center.xml");

                }
            }
            if (!resAlignerOther) CGlobal.OtherAligner = null;
            AppendSTextToStatus("Other-Stage setup", resAlignerOther);
            finalResult &= resAlignerOther;
            CGlobal.OtherAligner = otherAligner;
        }
        else
        {
            //-------------------- Left Stage -------------------- 
            var leftAligner = new LeftAligner();
            if (doPassStage) resAlignerLeft = true;
            else
            {
                try
                {
                    leftAligner.Open();
                }
                catch (Exception ex)
                {
                    if (leftAligner != null) AppendSTextToStatus($"{leftAligner} : {ex.Message}");
                    else AppendSTextToStatus($"LEFT: {ex.Message}");
                }
            }

            CGlobal.LeftAligner = leftAligner;

            if (leftAligner.IsConnectedOK())
            {
				//Connection success //
				resAlignerLeft = true;
            }
            else
            {
                //Connection fail //
                CGlobal.LeftAligner = null;
				resAlignerLeft = false;
            }
            AppendSTextToStatus("Left-Stage setup", resAlignerLeft);


            //-------------------- Right Stage -------------------- 
            var rightAligner = new RightAligner();
            if (doPassStage) resAlignerRight = true;
            else
            {
                try
                {
                    rightAligner.Open();
                }
                catch (Exception ex)
                {
                    if (rightAligner != null) AppendSTextToStatus($"{rightAligner} : {ex.Message}");
                    else AppendSTextToStatus($"RIGHT: {ex.Message}");
                }
            }
            CGlobal.RightAligner = rightAligner;

            if (rightAligner.IsConnectedOK())
            {
				//Connection success //
				resAlignerRight = true;
            }
            else
            {
                //Connection fail //
                CGlobal.RightAligner = null;
				resAlignerRight = false;
            }
            AppendSTextToStatus("Right-Stage setup", resAlignerRight);


            //-------------------- Other Stage -------------------- 
            var otherAligner = new CenterAligner();
            if (doPassStage) resAlignerOther = true;
            else
            {
                try
                {
                    otherAligner.Open();
                }
                catch (Exception ex)
                {
                    if (otherAligner != null) AppendSTextToStatus($"{otherAligner} : {ex.Message}");
                    else AppendSTextToStatus($"Other: {ex.Message}");
                }
            }
            CGlobal.OtherAligner = otherAligner;
            CGlobal.CameraAxis = mConfig.GetValue("CAMERA_AXIS").Contains("X") ? otherAligner.AXIS_X : otherAligner.AXIS_Y;
            CGlobal.CenterAxis = mConfig.GetValue("CENTER_AXIS").Contains("Y") ? otherAligner.AXIS_Y : otherAligner.AXIS_X;

            if (otherAligner.IsConnectedOK())
            {
				//Connection success //
				resAlignerOther = true;
            }
            else
            {
                //Connection fail //
                CGlobal.OtherAligner = null;
				resAlignerOther = false;
            }
            AppendSTextToStatus("Other-Stage setup", resAlignerOther);

        }

        CGlobal.CameraDirection = mConfig.GetValue("CAMERA_Direction").Contains("F") ? 1 : -1;
        CGlobal.CenterDirection = mConfig.GetValue("CENTER_Direction").Contains("F") ? 1 : -1;

		#endregion


		#region ===== init 8164 ====

		//-------------------- tls, pm -------------------- 
		bool resTlsPm = false;
        try
        {
            C8164 agilent = CGlobal.Pm8164 = CGlobal.Tls8164 = new C8164();
            result = agilent.Connect(GlobalAddress.GPIB_Board, GlobalAddress.GPIB_AgilentTls, GlobalAddress.GPIB_AgilentPm1, GlobalAddress.GPIB_AgilentPm2, CGlobal.TlsSlot);
            AppendSTextToStatus("TLS, PM connection", result);
            if (result)
            {
				resTlsPm = agilent.Init(!CGlobal.mIsLocalTls, doPassPmSetup);
                AppendSTextToStatus("TLS, PM setup", resTlsPm);
            }
        }
        catch
        {
			resTlsPm = false;
        }

        if (!resTlsPm)
        {
            CGlobal.Pm8164?.Dispose();
            CGlobal.Pm8164 = null;
            CGlobal.Tls8164?.Dispose();
            CGlobal.Tls8164 = null;
        }
        finalResult &= resTlsPm;

		#endregion


		#region ===== init PC =====

		bool resPC = false;
		//-------------------- polariztion controller -------------------- 
		if (CGlobal.mUsingOpc && CGlobal.mIsLocalPc)
		{
			switch (CGlobal.mPcType)
			{
				case CGlobal.PcType.None:
					resPC = true;
					break;
				case CGlobal.PcType.C_8169a:
					try
					{
						var agilent8169A = new Agilent8169A();
						resPC = agilent8169A.Connect(GlobalAddress.GPIB_Pc_Cband_8169A);
						CGlobal.mPc = agilent8169A;
					}
					catch { resPC = false; }
					break;
				case CGlobal.PcType.O_PSG100:
					try
					{
						var psg100 = new PcPsg100();
						resPC = psg100.Connect(GlobalAddress.GPIB_Pc_Oband_PSG100);
						CGlobal.mPc = psg100;
					}
					catch { resPC = false; }
					break;
				case CGlobal.PcType.C_NPC64C:
					try
					{
						var npc64C = new NeonOpc();
						npc64C.Connect(GlobalAddress.COM_Pc_Cband_NPC64C);
						resPC = true;
						CGlobal.mPc = npc64C;
					}
					catch { resPC = false; }
					break;
				case CGlobal.PcType.O_NPC64O:
					try
					{
						var npc64O = new NeonOpc();
						npc64O.Connect(GlobalAddress.COM_Pc_Oband_NPC64O);
						resPC = true;
						CGlobal.mPc = npc64O;
					}
					catch { resPC = false; }
					break;
			}
			AppendSTextToStatus($"{CGlobal.mPcType}", resPC);
		}
		else { resPC = true; }

		finalResult &= resPC;

		#endregion


		#region ===== init OSW =====
		bool resOSW = false;
        //-------------------- Optical Swith 1 -------------------- 
        try
        {
            Osw osw1 = CGlobal.Switch = new Osw(GlobalAddress.OswAlignPort, GlobalAddress.OswTlsPort, false);
			resOSW = osw1.Connect(GlobalAddress.COM_Osw);
            AppendSTextToStatus("Optical Switch 1", resOSW);
        }
        catch { resOSW = false; }
        if (!resOSW) CGlobal.Switch = null;
        finalResult &= resOSW;

		#endregion




		return finalResult;

    }



    private bool initSystem()
    {
        bool bRes = false;

        //--------------- Sweep system  --------------
        var sweepSystem = CGlobal.SweepSystem;
        if (sweepSystem == null)
        {
            if(CGlobal.mIsLocalPc) sweepSystem = new SweepLogicDwdm(CGlobal.Tls8164, CGlobal.Pm8164, CGlobal.mPc);
            else sweepSystem = new SweepLogicDwdm(CGlobal.Tls8164, CGlobal.Pm8164);
            CGlobal.SweepSystem = sweepSystem;

			sweepSystem.TlsWaveShift_pm = CGlobal.TlsWaveShift_pm;
		}

        bRes = sweepSystem.InitTcpServer(CGlobal.mIsLocalTls , CGlobal.mIsLocalPc);//out CGlobal.mTcp);
        if (bRes) AppendSTextToStatus("Sweep System ...OK", true);
        else	  AppendSTextToStatus("Sweep System ...FAIL", false);

        //-------------- Alignment -------------------
        ScanParam.FinePeakStep = 0.25;
        CGlobal.Alignment = new Calignment(CGlobal.LeftAligner, CGlobal.RightAligner, CGlobal.Ds2000, CGlobal.Pm8164);

        return CGlobal.Alignment != null && CGlobal.SweepSystem != null;
    }



    void init()
    {
        initEnd = false;

        string fileName = @"log\log_init.txt";
        StreamWriter sw = null;

        try
        {
            sw = new StreamWriter(fileName);
            sw.WriteLine(DateTime.Now.ToLongTimeString());

            CGlobal.initOK = true;
            CGlobal.initOK &= initDevices();
            CGlobal.initOK &= initSystem();
            
            Invoke((Action)(() => sw.Write(uiStatusText.Text)));
            
            sw.Close();
            //if (!GlobalParam.initOK) System.Diagnostics.Process.Start($"{fileName}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"InitForm.init():\n{ex.Message}");
        }
        finally
        {
            initEnd = true;
            if (sw != null)  sw.Close();
        }
    }


    private void AppendSTextToStatus(string strText, bool ok = true)
    {
        Invoke((Action)(() =>
        {
            if (!ok) uiStatusText.AppendText("\n");
            uiStatusText.AppendText(strText + ":\t" + (ok ? "OK" : "Failed") + "\n");
            if (!ok) uiStatusText.AppendText("\n");

            if (!ok)
            {
                int nStartPos = uiStatusText.Find(strText);
                uiStatusText.Select(nStartPos, strText.Length);
                uiStatusText.SelectionColor = Color.Yellow;
            }
            uiStatusText.SelectionStart = uiStatusText.Text.Length;
            uiStatusText.ScrollToCaret();
            uiStatusText.Refresh();
        }));
    }



    static Thread sInitThread;
    static bool initEnd = false;
    bool doNotAsk;
    bool doPassStage;
    bool doPassPmSetup;

    private void btnInit_Click(object sender, EventArgs e)
    {
		//초기화 실행 버튼
        try
        {
            Cursor = Cursors.WaitCursor;
            CGlobal.TestMode = uiTestMode.Checked;
            uiStatusText.Clear();

			//Local || Tcp
            CGlobal.mIsLocalPc = (!uiDoPCserver.Checked);

			MyLogic.Instance.Reference.NumPols = CGlobal.mUsingOpc ? 4 : 1;
			//Tls Shift
			CGlobal.TlsWaveShift_pm = uiTlsWaveShiftPm.Text.To<int>();

			//Init option [pm || stage init pass]
			doPassPmSetup = uiDoPassPm.Checked;
            doPassStage = uiDoPassStage.Checked;

            sInitThread = new Thread(init);
            sInitThread.Name = "Init";
            sInitThread.Start();

            while (!initEnd)
            {
                Application.DoEvents();
            }
            Close();

        }
        catch (Exception ex)
        {
            MessageBox.Show($"InitForm.btnInit_Click():\n{ex.Message}");
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }




    #region ---- Menu Handler ----


    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        mConfig.Dispose();

		Neon.Dwdm.Properties.Settings.Default.init_TLS_Type = (uiTlsLocal.Checked) ? 0 : 1;

		int pc = 0;
		if (uiPc8169.Checked) pc = 1;
		else if (uiPcNpcC.Checked) pc = 2;
		else if (uiPcPsg100.Checked) pc = 3;
		else if (uiPcNpcO.Checked) pc = 4;
		Neon.Dwdm.Properties.Settings.Default.init_PC_Device = pc;
		Neon.Dwdm.Properties.Settings.Default.Save();
    }



    private void btnPass_Click(object sender, EventArgs e)
    {
        if (sInitThread != null) sInitThread.Abort();

        CGlobal.mUsingOpc = !uiPcNone.Checked;
        CGlobal.mIsLocalTls = uiTlsLocal.Checked;

        Close();
    }
	

    private void btnQuit_Click(object sender, EventArgs e)
    {
        if (sInitThread != null) sInitThread.Abort();
        Application.Exit();
    }


	//bool mIgnorePcEvent = false;
    private void uiPc_CheckedChanged(object sender, EventArgs e)
    {
		//편광컨트롤러 radioButtion 
		//if (mIgnorePcEvent) return;

		if (uiPcNone.Checked)
		{
			CGlobal.mBand = CGlobal.WlBand.None;
			CGlobal.mPcType = CGlobal.PcType.None;
		}
		else if (uiPc8169.Checked)
		{
			CGlobal.mBand = CGlobal.WlBand.CBand;
			CGlobal.mPcType = CGlobal.PcType.C_8169a;
		}
		else if (uiPcPsg100.Checked)
		{
			CGlobal.mBand = CGlobal.WlBand.OBand;
			CGlobal.mPcType = CGlobal.PcType.O_PSG100;
		}
		else if (uiPcNpcC.Checked)
		{
			CGlobal.mBand = CGlobal.WlBand.CBand;
			CGlobal.mPcType = CGlobal.PcType.C_NPC64C;
		}
		else if (uiPcNpcO.Checked)
		{
			CGlobal.mBand = CGlobal.WlBand.OBand;
			CGlobal.mPcType = CGlobal.PcType.O_NPC64O;
		}

		CGlobal.mUsingOpc = !uiPcNone.Checked;
		CGlobal.mIsLocalTls = uiTlsLocal.Checked;
    }
	

    private void uiDoPCserver_CheckedChanged(object sender, EventArgs e)
    {
        if (uiDoPCserver.Checked && uiTlsLocal.Checked)
        {
            MessageBox.Show("Local TLS 상태입니다.");
            uiDoPCserver.Checked = false;
			return;
        }

		CGlobal.mIsLocalPc = (!uiDoPCserver.Checked);
	}


	private void uiDoTlsWaveShift_CheckedChanged(object sender, EventArgs e)
	{
		uiTlsWaveShiftPm.Enabled = uiDoTlsWaveShift.Checked;
	}


	#endregion


}

