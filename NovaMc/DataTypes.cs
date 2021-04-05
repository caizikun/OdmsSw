using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{
	public class PmcHwInfo
	{
		public const int MaxNumAxes = Nova8BWrap.AXIS_NUM;
		public const int MaxNumPmc = Nova8BWrap.MAX_CARD;

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

	public enum NovaMcErrorStatus : ushort
	{
		//return value
		FALSE = Nova8BWrap.MMC_FALSE,//Pmc4BWrap.MMC_FALSE,
		TRUE = Nova8BWrap.MMC_TRUE,//Pmc4BWrap.MMC_TRUE,		//not error

		//error status
		//OPEN_ERR			= Pmc4BWrap.MMC_OPEN_ERR, 
		//IOADDRESS_ERR		= Pmc4BWrap.MMC_IOADDRESS_ERR, 
		//TIMEOUT_ERR			= Pmc4BWrap.MMC_TIMEOUT_ERR, 
		//      INVALID_AXIS		= Pmc4BWrap.MMC_INVALID_AXIS,   
		//ILLEGAL_PARAMETER	= Pmc4BWrap.MMC_ILLEGAL_PARAMETER,  
		//ZERO_PARAMETER		= Pmc4BWrap.MMC_ZERO_PARAMETER, 
		//      ERROR				= Pmc4BWrap.MMC_ERROR,         
		//QUIT				= Pmc4BWrap.MMC_QUIT,
		//INVALID_CARD		= Pmc4BWrap.MMC_INVALID_CARD

		//error status
		// 2017.06.21 Jeon
		OPEN_ERR = Nova8BWrap.MMC_OPEN_ERR,
		IOADDRESS_ERR = Nova8BWrap.MMC_IOADDRESS_ERR,
		TIMEOUT_ERR = Nova8BWrap.MMC_TIMEOUT_ERR,
		INVALID_AXIS = Nova8BWrap.MMC_INVALID_AXIS,
		ILLEGAL_PARAMETER = Nova8BWrap.MMC_ILLEGAL_PARAMETER,
		ZERO_PARAMETER = Nova8BWrap.MMC_ZERO_PARAMETER,
		ERROR = Nova8BWrap.MMC_ERROR,
		QUIT = Nova8BWrap.MMC_QUIT,
		INVALID_CARD = Nova8BWrap.MMC_INVALID_CARD
	}

	public enum NovaMcAddress : ushort  // 0 ~ 15 <   MaxNumPmc
	{
		PMC00 = 0, PMC01, PMC02, PMC03, PMC04, PMC05, PMC06, PMC07,
		PMC08, PMC09, PMC10, PMC11, PMC12, PMC13, PMC14, PMC15
	}


	[Flags]
	public enum NmcAxis : ushort
	{
		//2017.06.21 Jeon
		None = 0,
		X = Nova8BWrap.NOVA8BPCI_AXIS1,
		Y = Nova8BWrap.NOVA8BPCI_AXIS2,
		Z = Nova8BWrap.NOVA8BPCI_AXIS3,
		U = Nova8BWrap.NOVA8BPCI_AXIS4,

		/// <summary>
		/// 나머지 4축 추가
		/// </summary>
		/// 2017.06.21 Jeon
		/// MCX304 칩번호(0 or 1) 로 지정되므로 일단 보류

		All = X | Y | Z | U
	}

	[Flags]
	public enum NovaMmcModeConfig : ushort
	{
		/// <summary>
		/// ushort.MaxValue 수정하세요.
		/// </summary>
		//None = 0x00,
		//LMT_SoftStop = ushort.MaxValue,// Pmc4BWrap.WR2_LMT_LMTMD,     //stop slowly on LMT signal
		//LMTP_ActiveHigh = ushort.MaxValue,//  //LMT+ active level high
		//LMTN_ActiveHigh = ushort.MaxValue,//  //LMT- active level high
		//Mode_PulseAndDir = ushort.MaxValue,// //drive pulse mode - Pulse And Dir
		//Pulse_ActiveDown = ushort.MaxValue,// //dirve pusle level - low
		//Dir_PositiveHigh = ushort.MaxValue//  //direction - positive at high


		// 2017.06.21 Jeon
		None = 0x00,
		LMT_SoftStop = Nova8BWrap.WR2_LMT_LMTMD,     //stop slowly on LMT signal
		LMTP_ActiveHigh = Nova8BWrap.WR2_LMT_HLMTP,  //LMT+ active level high
		LMTN_ActiveHigh = Nova8BWrap.WR2_LMT_HLMTM,  //LMT- active level high
		Mode_PulseAndDir = Nova8BWrap.WR2_DRV_PLSMD, //drive pulse mode - Pulse And Dir
		Pulse_ActiveDown = Nova8BWrap.WR2_DRV_PLS_L, //dirve pusle level - low
		Dir_PositiveHigh = Nova8BWrap.WR2_DRV_DIR_L  //direction - positive at high

	}


	public class NovaMmcAccel
	{
		/// <summary>
		/// 필요하면 맞게 수정하고
		/// 불필요하면 삭제
		/// </summary>
		public const Int16 ac = 1000; //acceleration
		public const Int16 dc = 1000; //deceleration
		public const Int16 aac = 101; //accRate= 
		public const Int16 ddc = 101; //decRate = 
	}

	// 2017.06.22 Jeon
	// IC Chip No, 1~4 => 0, 5~8 => 1
	public enum NovaMcIcNo : short
	{
		X1 = Nova8BWrap.IC.A,
		X2 = Nova8BWrap.IC.A,
		X3 = Nova8BWrap.IC.A,
		X4 = Nova8BWrap.IC.A,
		X5 = Nova8BWrap.IC.B,
		X6 = Nova8BWrap.IC.B,
		X7 = Nova8BWrap.IC.B,
		X8 = Nova8BWrap.IC.B
	}



}
