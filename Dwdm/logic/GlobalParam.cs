using Neon.Aligner;
using Calignment = Neon.Aligner.AlignLogic;
using Universe.TcpServer;

class GlobalAddress
{
	//Stage [Suruga]
    public static int COM_LeftAxis1;
    public static int COM_LeftAxis2;
    public static int COM_LeftAxis3;
    public static int COM_RightAxis1;
    public static int COM_RightAxis2;
    public static int COM_RightAxis3;
    public static int COM_OtherAxis1;

	//OSW
    public static int COM_Osw;
    public static int OswAlignPort;
    public static int OswTlsPort;

	//TLS & PM
	public static int GPIB_Board;
	public static int GPIB_AgilentTls;
    public static int GPIB_AgilentPm1;
    public static int GPIB_AgilentPm2;

	//polarization controller.
	public static int GPIB_Pc_Cband_8169A;
	public static int GPIB_Pc_Oband_PSG100;
	public static int COM_Pc_Cband_NPC64C;
	public static int COM_Pc_Oband_NPC64O;

	//DAQ
    public static int AI_Left;
    public static int AI_Right;

	//Espec Chamber
	public static int GPIB_Chamber;

	//UV
	public static int DaqDO_dev;
	public static int DaqDO_port;
	public static int DaqDO_line;

}



class CGlobal
{
	
    #region global variables

    public static bool initOK;

    public static Daq Daq;
    public static IDispSensor Ds2000;
    public static bool Ds2000_Terminal_RSE;
    public static double[] Ds2000_VoltageRange;

    public static bool IsSfac;
	public static IairValvController AirValve;

	public enum MC { Suruga, Autonics, Nova }
    public static MC McType;
    public static Istage LeftAligner;
    public static Istage RightAligner;
    public static Istage OtherAligner;
    public static int CenterAxis;
    public static int CameraAxis;
    public static int CenterDirection;
    public static int CameraDirection;

    public static Osw Switch;

	public static C8164 Tls8164;
    public static ITcpAgentClient mTcp;
    public static int TlsSlot;

    public static C8164 Pm8164;
    public static int[] PmPort1;
    public static int[] PmPort2;

	public enum WlBand { None, OBand, CBand }
	public static WlBand mBand = WlBand.CBand;
	public enum PcType { None, C_8169a, C_NPC64C, O_PSG100, O_NPC64O }
	public static PcType mPcType = PcType.None;
	public static IpolController mPc;

    public static bool mIsLocalTls = true;
    public static bool mIsLocalPc = true;
	public static bool mUsingOpc = true;

    public static Calignment Alignment;
    public static SweepLogicDwdm SweepSystem;

    public static int AlignThresholdPower;
    public static XYSearchParam XySearchParamLeft;
    public static XYSearchParam XySearchParamRight;

    public static bool TestMode = false;
    public static int TlsPower;
	public static int TlsWaveShift_pm = -100;

	public static EspecChamber EspecChamber;
	public static bool mUsingChamber;

	public static UshioUvCure UshioUvCure;
	public static bool mUsingUv;

	#endregion


	static CGlobal()
    {
        XySearchParamLeft = XYSearchParam.Create(new double[] { 10, 1, 10, 1 });
        XySearchParamRight = XYSearchParam.Create(new double[] { 10, 1, 10, 1 });
    }

}

