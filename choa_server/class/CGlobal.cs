using DrBae.TnM.Device;
using Free302.TnM.Device;
using Neon.Aligner;

class CGlobal : GlobalBase
{

	public struct DaqParamforAir
	{
		public int av1;                     //air valve 1
		public int av2;                     //air valve 2
	}

	public static DeviceAddress Address;
    public static DaqAnalogParam DaqAiParam;
    public static DaqParamforAir DaqforAirParam;

    public static Daq DAQ;
    public static DaqBase DaqBase;
    public static IDispSensor Ds2000;
    

	public enum MC { Suruga, Autonics, Nova }//TODO: stage config 에서 처리할 것
	public static MC McType;
    public static Istage LeftAligner;
    public static Istage RightAligner;
    public static Istage OtherAligner;
    public static bool UsingSurugaOther = false;

    public static bool InitOK;


    
}

