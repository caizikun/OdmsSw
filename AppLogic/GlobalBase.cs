using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public class GlobalBase
    {

		public struct DaqAnalogParam
        {
            public int[] Chs;
            public bool IsRSE;
            public double[] VoltRange;
        }

		public struct DaqDigitalOut
		{
			//Ushio UV 
			public int dev;
			public int port;
			public int line;
		}

		public struct DeviceAddress
        {
            public int COM_LeftMc1;           //left stage 1
            public int COM_LeftMc2;           //left stage 2
            public int COM_LeftMc3;           //left stage 3
            public int COM_RightMc1;          //right stage 1
            public int COM_RightMc2;          //right stage 2
            public int COM_RightMc3;          //right stage 3
            public int COM_OtherMc;           //other stage 1

            public int COM_Osw;

            public int GpibTLS;
            public int GpibOpm;
            public int GpibBoard;
        }

		//Sfac 장비
		public static bool IsSfac = false;

        //TLS & PM 구성 전용
        public static bool IsTlsPmMode = false;
        public static bool IsTestMode = false;

        //TLS, PM
        public enum _TlsType { Server, Agilent, Santec, Test }//<!--server=0; Agilent TLS : 1 || Santec TLS : 2 || HsbLs = 3-->
        public static Itls Tls { get; set; }// { get { return TlsType == 1 ? (Itls)Tls8164 : (TlsType == 2 ? (Itls)TlsSantec : HsbLs); } }
        public static IoptMultimeter Opm { get; set; }//{ get { return OpmType == 1 ? (IoptMultimeter)Pm8164 : HsbOpm; } }

        //TLS, PM Param
        public static _TlsType TlsType;
        public static bool IsTlsPmDual = true;
        public static bool UsingTcpServer = false;
        public static ConfigTlsParam TlsParam;
        public static int[] PmGain = { -10, -30 };
        public static int OpmType = 1;
        public static int PmAlignGain;
        public static int OpmAvgTimeUs = 200;
        public static decimal[] HsbOpmParam;

        //OSW
        public static Osw osw;
        public static int OswAlignPort;
        public static int OswTlsPort;
        public static bool UsingOsw;
        public static bool OswIsNeon = false;

        //Logics
        public static AlignLogic alignLogic;
        public static SweepLogic sweepLogic;

		//ALIGN
		//public static bool UsingAlignSource;
		public static int AlignThresholdPower;
        public static XYSearchParam XySearchParamLeft;
        public static XYSearchParam XySearchParamRight;

		//Stage
		public static bool UsingCenterAxis;
		public static int CenterAxis;
        public static int CenterDirection = 1;
        public static bool UsingCameraAxis;
		public static int CameraAxis;
        public static int CameraDirection = 1;

		//UV
		public static IUvCure UshioUvCure;
		public static bool UsingUv;
        public static DaqDigitalOut DaqParamForUV;

        public static bool UsingAir;
		public static IairValvController AirValve;

    }
}
