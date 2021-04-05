using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{
    public class PmcHwInfo
    {
		public const int MaxNumAxes = Pmc4BWrap.AXIS_NUM;
		public const int MaxNumPmc = Pmc4BWrap.MAX_CARD;

		public const int MaxUnitMultiplier = 500;
		private const int UnitConversionFactor = 8000000;

		public static int R(int multiplier)
		{
			if (multiplier < 1) return UnitConversionFactor;
			if (multiplier > MaxUnitMultiplier) 
				multiplier = UnitConversionFactor;

			return UnitConversionFactor / multiplier;
		}
    }

    public enum PmcErrorStatus : ushort
    {
        //return value
		FALSE = Pmc4BWrap.MMC_FALSE,
		TRUE = Pmc4BWrap.MMC_TRUE,		//not error

        //error status
		OPEN_ERR			= Pmc4BWrap.MMC_OPEN_ERR, 
		IOADDRESS_ERR		= Pmc4BWrap.MMC_IOADDRESS_ERR, 
		TIMEOUT_ERR			= Pmc4BWrap.MMC_TIMEOUT_ERR, 
        INVALID_AXIS		= Pmc4BWrap.MMC_INVALID_AXIS,   
		ILLEGAL_PARAMETER	= Pmc4BWrap.MMC_ILLEGAL_PARAMETER,  
		ZERO_PARAMETER		= Pmc4BWrap.MMC_ZERO_PARAMETER, 
        ERROR				= Pmc4BWrap.MMC_ERROR,         
		QUIT				= Pmc4BWrap.MMC_QUIT,
		INVALID_CARD		= Pmc4BWrap.MMC_INVALID_CARD
    }

	public enum PmcAddress : ushort	// 0 ~ 15 <   MaxNumPmc
    {
        PMC00 = 0, PMC01, PMC02, PMC03, PMC04, PMC05, PMC06, PMC07, 
        PMC08, PMC09, PMC10, PMC11, PMC12, PMC13, PMC14, PMC15
    }

    [Flags]
    public enum PmcAxis : ushort
    {
		None = 0,
        X = Pmc4BWrap.PMC4BPCI_AXIS1, 
		Y = Pmc4BWrap.PMC4BPCI_AXIS2, 
		Z = Pmc4BWrap.PMC4BPCI_AXIS3,
        U = Pmc4BWrap.PMC4BPCI_AXIS4, 
		All = X | Y | Z | U
    }

	[Flags]
	public enum PmcModeConfig : ushort
	{
		None = 0x00,
		LMT_SoftStop = Pmc4BWrap.WR2_LMT_LMTMD,		//stop slowly on LMT signal
		LMTP_ActiveHigh = Pmc4BWrap.WR2_LMT_HLMTP,	//LMT+ active level high
		LMTN_ActiveHigh = Pmc4BWrap.WR2_LMT_HLMTM,	//LMT- active level high
		Mode_PulseAndDir = Pmc4BWrap.WR2_DRV_PLSMD,	//drive pulse mode - Pulse And Dir
		Pulse_ActiveDown = Pmc4BWrap.WR2_DRV_PLS_L,	//dirve pusle level - low
		Dir_PositiveHigh = Pmc4BWrap.WR2_DRV_DIR_L	//direction - positive at high
	} 


    public class PmcAccel
    {
        public const Int16 ac = 1000; //acceleration
        public const Int16 dc = 1000; //deceleration
        public const Int16 aac = 101; //accRate= 
        public const Int16 ddc = 101; //decRate = 
    }



}
