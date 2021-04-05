using System;
using System.Runtime.InteropServices;
using System.Management;

namespace Free302.TnM.Device
{
    // ERROR: Not supported in C#: OptionDeclaration
    public class Nova8BWrap
	{
		////////////////////////////////////////////////// ///////////////////////////
		////
		//// 정의
		////
		////////////////////////////////////////////////// ///////////////////////////

		#region ==== common ====

		public const int MAX_CARD = 16;
		public const int AXIS_NUM = 4;

		//return value
		public const UInt16 MMC_FALSE = 0;
		public const UInt16 MMC_TRUE = 1;
		public const UInt16 MMC_OK = MMC_TRUE;
		public const UInt16 MMC_HIGH_LEVEL = MMC_TRUE;
		public const UInt16 MMC_LOW_LEVEL = MMC_FALSE;

		//status
		public const UInt16 MMC_OPEN_ERR = 5;
		public const UInt16 MMC_IOADDRESS_ERR = 6;
		public const UInt16 MMC_TIMEOUT_ERR = 7;
		public const UInt16 MMC_INVALID_AXIS = 8;
		public const UInt16 MMC_ILLEGAL_PARAMETER = 9;
		public const UInt16 MMC_ZERO_PARAMETER = 10;
		public const UInt16 MMC_ERROR = 11;
		public const UInt16 MMC_QUIT = 12;
		public const UInt16 MMC_INVALID_CARD = 13;

		#endregion

		#region === Axies ===

		public const UInt16 NOVA8BPCI_AXIS1 = 0x0100;
		public const UInt16 NOVA8BPCI_AXIS2 = 0x0200;
		public const UInt16 NOVA8BPCI_AXIS3 = 0x0400;
		public const UInt16 NOVA8BPCI_AXIS4 = 0x0800;

		#endregion

		// 레지스터 번호
		public const int MCX_WR0 = 0x0; //WR0
		public const int MCX_WR1 = 0x1; //WR1
		public const int MCX_WR2 = 0x2; //WR2
		public const int MCX_WR3 = 0x3; //WR3
		public const int MCX_WR4 = 0x4; //WR4
		public const int MCX_WR5 = 0x5; //WR5
		public const int MCX_WR6 = 0x6; //WR6
		public const int MCX_WR7 = 0x7; //WR7


		public const int MCX_RR0 = 0x0; //RR0
		public const int MCX_RR1 = 0x1; //RR1
		public const int MCX_RR2 = 0x2; //RR2
		public const int MCX_RR3 = 0x3; //RR3
		public const int MCX_RR4 = 0x4; //RR4
		public const int MCX_RR5 = 0x5; //RR5
		public const int MCX_RR6 = 0x6; //RR6
		public const int MCX_RR7 = 0x7; //RR7

		public enum REG_MCX
		{
			WR0_A = 0,
			RR0_A = 0,
			WR0 = 0,
			RR0 = 0,
			WR1_A = 1,
			RR1_A = 1,
			WR1 = 1,
			RR1 = 1,
			WR2_A = 2,
			RR2_A = 2,
			WR2 = 2,
			RR2 = 2,
			WR3_A = 3,
			RR3_A = 3,
			WR3 = 3,
			RR3 = 3,
			WR4_A = 4,
			RR4_A = 4,
			WR4 = 4,
			RR4 = 4,
			WR5_A = 5,
			RR5_A = 5,
			WR5 = 5,
			RR5 = 5,
			WR6_A = 6,
			RR6_A = 6,
			WR6 = 6,
			RR6 = 6,
			WR7_A = 7,
			RR7_A = 7,
			WR7 = 7,
			RR7 = 7,
			WR0_B = 8,
			RR0_B = 8,
			WR1_B = 9,
			RR1_B = 9,
			WR2_B = 10,
			RR2_B = 10,
			WR3_B = 11,
			RR3_B = 11,
			WR4_B = 12,
			RR4_B = 12,
			WR5_B = 13,
			RR5_B = 13,
			WR6_B = 14,
			RR6_B = 14,
			WR7_B = 15,
			RR7_B = 15,
			WR10 = 16,
			RR10 = 16,
			WR11 = 17,
			RR11 = 17,
			WR12 = 18,
			RR12 = 18,
			WR14 = 20,
			RR14 = 20,
			WR15 = 21,
			RR15 = 21,
			WR16 = 22,
			RR16 = 22
		}


		// IC 정의
		public enum IC
		{
			A = 0,
			MCX304 = 0,
			B = 1,
			MCX314AS = 1
		}

		// 축 정의
		public enum AXIS
		{
			NONE = 0,  // 축 지정 없음
			X = 1,     // X 축
			Y = 2,     // Y 축
			Z = 4,     // Z 축
			U = 8,     // U 축
			ALL = 15   // 전체 축
		}

		// WR1 (Mode register 1)
		// drive정지 입력신호
		public const UInt16 WR1_IN0_L = 0x0001;// 입력신호 IN0에대한 논리레벨(0-low에서정지,1-high에서감속정지)
		public const UInt16 WR1_IN0_E = 0x0002;// 입력신호 IN0에대한 유효/무효 설정(0-무효,1-유효)
		public const UInt16 WR1_IN1_L = 0x0004;
		public const UInt16 WR1_IN1_E = 0x0008;
		public const UInt16 WR1_IN2_L = 0x0010;
		public const UInt16 WR1_IN2_E = 0x0020;
		public const UInt16 WR1_IN3_L = 0x0040;
		public const UInt16 WR1_IN3_E = 0x0080;
		// interrupt의 발생조건 설정
		public const UInt16 WR1_INT_PULSE = 0x0100;// drive pulse의 rising edge(펄스에서 올라가는 순간)에서 인터럽트 발생(drive pulse정논리 설정시)
		public const UInt16 WR1_INT_PGECM = 0x0200;// 논리/실제위치 pulse값 >= COMP-register값 일때
		public const UInt16 WR1_INT_PLCM = 0x0400;// 논리/실제위치 pulse값 < COMP-register값 일때
		public const UInt16 WR1_INT_PLCP = 0x0800;// 논리/실제위치 pulse값 < COMP+register값 일때
		public const UInt16 WR1_INT_PGEP = 0x1000;// 논리/실제위치 pulse값 >= COMP+register값 일때
		public const UInt16 WR1_INT_CEND = 0x2000;// 정속지역에서 pulse출력을 종료할때(가감속 drive일때)
		public const UInt16 WR1_INT_CSTA = 0x4000;// 정속지역에서 pulse출력을 개시하였을때(가감속 drive일때)
		public const UInt16 WR1_INT_DEND = 0x8000;// drive가 종료하였을때

		// WR2 (Mode register 2)
		public const UInt16 WR2_LMT_SLMTP = 0x0001;// COMP+register를 software limit로 설정
		public const UInt16 WR2_LMT_SLMTM = 0x0002;// COMP-register를 software limit로 설정
		public const UInt16 WR2_LMT_LMTMD = 0x0004;// hardware limit(nLMTP,nLMTM)가 active될 경우 정지방식 설정(0:즉시정지,1:감속정지)
		public const UInt16 WR2_LMT_HLMTP = 0x0008;// +방향 limit입력신호(nLMTP)의 논리 level설정(0:low,1:high에서 active)
		public const UInt16 WR2_LMT_HLMTM = 0x0010;// -방향 limit입력신호(nLMTM)의 논리 level설정(0:low,1:high에서 active)
		public const UInt16 WR2_LMT_CMPSL = 0x0020;// COMP+/- register의 비교대상(0:논리위치,1:실제위치와 비교)
		public const UInt16 WR2_DRV_PLSMD = 0x0040;// drive pulse의 출력방식(0:독립2 pulse방식,1:1 pulse방식)
		public const UInt16 WR2_DRV_PLS_L = 0x0080;// drive pulse의 논리 level설정(0:정논리,1:부논리)
		public const UInt16 WR2_DRV_DIR_L = 0x0100;// drive pulse의 방향출력신호의 논리 level 설정(0:+방향일때low,-방향일때hi,1:+방향일때hi,-방향일때low)
		public const UInt16 WR2_ENC_PINMD = 0x0200;// encoder입력신호(nECA/PPIN,nECB/PMIN)의 출력방식(0:2상pulse,1:up/dn pulse입력)
		public const UInt16 WR2_ENC_PIND0 = 0x0400;// encoder 2상 pulse입력의 분주비를 설정(D1D0 00=1/1, 01=1/2
		public const UInt16 WR2_ENC_PIND1 = 0x0800;// encoder 2상 pulse입력의 분주비를 설정      10=1/4, 11=무효)
		public const UInt16 WR2_SRV_ALM_L = 0x1000;// nALARM입력 신호의 논리 level을 설정(0:low, 1:high에서 active)
		public const UInt16 WR2_SRV_ALM_E = 0x2000;// servo motor alarm용 입력신호 nALARM의 유/무효 설정(0:무효,1:유효)
		public const UInt16 WR2_SRV_INP_L = 0x4000;// nINPOS입력 신호의 논리 lovel을 설정(0:low, 1:high에서 active)
		public const UInt16 WR2_SRV_INP_E = 0x8000;// servo motor 위치 결정 완료용 입력신호 nINPOS의 유효/무효 설정(0:무효,1:유효)

		//<- New Add 2017.06.20





		///*

		// 2017.08.28
		// 디바이스 ID
		// MC8043P
		public const Int32 ID_MC8043P = 0xa0a2;
		// MC8080P
		public const Int32 ID_MC8080P = 0xa07d;
		// MC8082P
		public const Int32 ID_MC8082P = 0xa0d0;

		//// -------------------------------------------
		//// 명령 정의
		//// -------------------------------------------
		// 드라이브 명령

		public enum CMD
		{
			CMD_Range = 0,
			CMD_Jerk = 1,
			CMD_Acc = 2,
			CMD_Dec = 3,
			CMD_StartSpd = 4,
			CMD_Speed = 5,
			CMD_Pulse = 6,
			CMD_DecP = 7,
			CMD_Lp = 9,
			CMD_Ep = 10,
			CMD_CompP = 11,
			CMD_CompM = 12,
			CMD_AccOfst = 13,
			CMD_NOP = 15,
			CMD_ReadLp = 16,
			CMD_ReadEp = 17,
			CMD_ReadSpeed = 18,
			CMD_ReadAccDec = 19,
			CMD_ReadSyncBuff = 20,
			CMD_F_DRV_P = 32,
			CMD_F_DRV_M = 33,
			CMD_C_DRV_P = 34,
			CMD_C_DRV_M = 35,
			CMD_START_HOLD = 36,
			CMD_START_FREE = 37,
			CMD_STP_STS_CLR = 37,
			CMD_STOP_DEC = 38,
			CMD_STOP_SUDDEN = 39,
			CMD_IP_2ST = 48,
			CMD_IP_3ST = 49,
			CMD_IP_CW = 50,
			CMD_IP_CCW = 51,
			CMD_IP_2BP = 52,
			CMD_IP_3BP = 53,
			CMD_BP_ENABLED = 54,
			CMD_BP_DISABLED = 55,
			CMD_BP_STACK = 56,
			CMD_BP_CLR = 57,
			CMD_IP_1STEP = 58,
			CMD_IP_DEC_VALID = 59,
			CMD_IP_DEC_INVALID = 60,
			CMD_IP_INTRPT_CLR = 61,
			CMD_HomeMode = 96,
			CMD_HomeVelocity = 97,
			CMD_HOME_EXEC = 98,
			CMD_DEVCTR_CLR = 99
		}

		public const short CMD_F_DRV_P = 0x20; // + 방향 정량 펄스 드라이브
		public const short CMD_F_DRV_M = 0x21; // - 방향 정량 펄스 드라이브

		// + 방향 연속 펄스 드라이브
		public const short CMD_C_DRV_P = 0x22;
		// - 방향 연속 펄스 드라이브
		public const short CMD_C_DRV_M = 0x23;
		// 드라이브 시작 보류
		public const short CMD_START_HOLD = 0x24;
		// 드라이브 시작 무료 / 종료 상태 지우기
		public const short CMD_START_FREE = 0x25;
		// 드라이브 시작 무료 / 종료 상태 지우기
		public const short CMD_STP_STS_CLR = 0x25;


		public const short CMD_STOP_DEC = 0x26; // 드라이브 감속 정지
		public const short CMD_STOP_SUDDEN = 0x27; // 드라이브 즉시 정지

		// 보간 명령 ※ MCX314As 전용
		// 2 축 직선 보간 드라이브
		public const short CMD_IP_2ST = 0x30;
		// 3 축 직선 보간 드라이브
		public const short CMD_IP_3ST = 0x31;
		// CW 원호 보간 드라이브
		public const short CMD_IP_CW = 0x32;
		// CCW 원호 보간 드라이브
		public const short CMD_IP_CCW = 0x33;
		// 2 축 비트 패턴 보간 드라이브
		public const short CMD_IP_2BP = 0x34;
		// 3 축 비트 패턴 보간 드라이브
		public const short CMD_IP_3BP = 0x35;
		// BP 레지스터 쓰기 가능
		public const short CMD_BP_ENABLED = 0x36;
		// BP 레지스터 쓰기 불가
		public const short CMD_BP_DISABLED = 0x37;
		// BP 데이터 스택
		public const short CMD_BP_STACK = 0x38;
		// BP 데이터 지우기
		public const short CMD_BP_CLR = 0x39;
		// 보간 단계별
		public const short CMD_IP_1STEP = 0x3a;
		// 감속 활성화
		public const short CMD_IP_DEC_VALID = 0x3b;
		// 감속 해제
		public const short CMD_IP_DEC_INVALID = 0x3c;
		// 보간 인터럽트 클리어
		public const short CMD_IP_INTRPT_CLR = 0x3d;

		// 기타 명령
		public const short CMD_HOME_EXEC = 0x62; // 자동 원점 출력 실행
		public const short CMD_DEVCTR_CLR = 0x63; // 카운터 클리어 펄스 출력
												  // 동기화 작업 시작 ※ MCX314As 전용
		public const short CMD_SYNC_ACTIVE = 0x65;
		// NOP (축 전환 용)
		public const short CMD_NOP = 0xf;

		// ------------------------------------------------ -----------
		// 보간 종료 메시지 종료 상태 ※ MCX314As 전용
		// ------------------------------------------------ -----------
		// 보간 종료 메시지
		// BP 보간 종료 메시지
		public const short WM_BP_END = 0x401;
		// 연속 보간 종료 메시지
		public const short WM_CIP_END = 0x402;

		// ***** BP 보간 종료 상태 *****
		// ■ 정상
		// 백그라운드에서 BP 보간을 시작했다
		public const short BP_START = 0x101;
		// BP 보간 성공
		public const short BP_END = 0x102;

		// ■ 보간 시작 전에 오류
		// 지정된 데이터 수가 범위를 벗어
		public const short BP_CNT_ERR = 0x111;
		// 이미 BP 보간 또는 연속 보간이 실행 ​​중
		public const short BP_ALREADY_EXEC = 0x112;
		// 스레드를 시작하지 못했습니다
		public const short BP_THREAD_ERR = 0x113;
		// 메모리를 확보 할 수 없었다
		public const short BP_MALLOC_ERR = 0x114;
		// 인수 값이 잘못
		public const short BP_PARAM_ERR = 0x116;

		public const short BP_NOT_OPEN_ERR = 0x117; // 지정한 보드가 오픈되어 있지 않은
													// 기타 오류
		public const short BP_OTHER_ERR = 0x118;


		// ■ 보간 실행 오류
		// BP 보간이 도중에 중지 (속도가 빠르고 다음 데이터 스택이 늦었 던 경우)
		public const short BP_STOP = 0x121;
		// BP 보간 실행 중에 사용자가 중단 한
		public const short BP_USER_STOP = 0x122;
		// BP 보간 중에 보드에서 오류 발생 (RR0 오류 정보가 설정된)
		public const short BP_DRIVE_ERR = 0x123;

		// ***** 연속 보간 종료 상태 *****
		// ■ 정상
		// 백그라운드에서 지속적 보간을 시작했다
		public const short CIP_START = 0x201;
		// 연속 보간 성공
		public const short CIP_END = 0x202;

		// ■ 보간 시작 전에 오류
		// 지정된 데이터 수가 범위를 벗어
		public const short CIP_CNT_ERR = 0x211;
		// 이미 BP 보간 또는 연속 보간이 실행 ​​중
		public const short CIP_ALREADY_EXEC = 0x212;
		// 스레드를 시작하지 못했습니다
		public const short CIP_THREAD_ERR = 0x213;
		// 메모리를 확보 할 수 없었다
		public const short CIP_MALLOC_ERR = 0x214;
		// 명령 오류 (사용자가 지정한 명령이 잘못)
		public const short CIP_CMD_ERR = 0x215;
		// 인수 값이 잘못
		public const short CIP_PARAM_ERR = 0x216;

		public const short CIP_NOT_OPEN_ERR = 0x217; // 지정한 보드가 오픈되어 있지 않은
													 // 기타 오류
		public const short CIP_OTHER_ERR = 0x218;

		// ■ 보간 실행 오류
		// 연속 보간이 도중에 중지 (속도가 빠르고 다음 데이터 세트를 놓친 경우)
		public const short CIP_STOP = 0x221;
		// 연속 보간 실행 중에 사용자가 중단 한
		public const short CIP_USER_STOP = 0x222;
		// 연속 보간 중에 보드에서 오류 발생 (RR0 오류 정보가 설정된)
		public const short CIP_DRIVE_ERR = 0x223;


		// ------------------------------------------------ -----------
		// 보간 데이터 구조 ※ MCX314As 전용
		// ------------------------------------------------ -----------
		// 2 축 BP 보간
		public struct DATA_2BP
		{
			public short Bp1p; // BP1P 데이터
			public short Bp1m; // BP1M 데이터
			public short Bp2p; // BP2P 데이터
			public short Bp2m; // BP2M 데이터
		}
		// 3 축 BP 보간
		public struct DATA_3BP
		{
			public short Bp1p;// BP1P 데이터
			public short Bp1m;// BP1M 데이터
			public short Bp2p;// BP2P 데이터
			public short Bp2m;// BP2M 데이터
			public short Bp3p;// BP3P 데이터
			public short Bp3m;// BP3M 데이터
		}

		// 2 축 연속 보간
		public struct DATA_2CIP
		{
			public short Cmd;// 명령 번호 (CMD_IP_2ST, CMD_IP_CW, CMD_IP_CCW 중 하나를 설정할)
			public short Speed;// 속도 (지정하려면 1 ~ 8000, 지정하지 않으면 0으로 설정하는)
			public int EndP1;  // 종점 (제 1 축)
			public int EndP2;  // 끝 (2 축)
			public int Center1;// 원호 중심점 (제 1 축)
			public int Center2;// 원호 중심점 (2 축)
		}
		// 참고 : 제 1 축, 2 축, WR5로 지정

		// 3 축 연속 보간
		public struct DATA_3CIP
		{
			public int EndP1;// 종점 (제 1 축)
			public int EndP2;// 끝 (2 축)
			public int EndP3;// 끝 (3 축)
			public short Speed;// 속도 (지정하려면 1 ~ 8000, 지정하지 않으면 0으로 설정하는)
		}

		public static string Ver = " ";
		//public const string DllFileName = " ";
		// DLL Import 2017.08.28
		public static class MC8000P
		{
			// 생성자
			static MC8000P()
			{
				Ver = GetVer();
			}

			public static UserThread[] callback;// 사용자 스레드 콜백
			public delegate void UserThread(int param);// 사용자 스레드 콜백

			// DLL 버전 정보 장치관리자에서 읽어와 저장
			public static string Ver { get; private set; }

			// 장치 버전정보 가져오기
			public static string GetVer()
			{
				string vVer = "V0";

                //return vVer;

				// 시스템 드라이버 읽어올 WMI
				ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("Select * from Win32_PnPSignedDriver");
				ManagementObjectCollection objCollection = objSearcher.Get();

				foreach (ManagementObject obj in objCollection)
				{
					string info = String.Format("Device='{0}',Manufacturer='{1}',DriverVersion='{2}' ", obj["DeviceName"], obj["Manufacturer"], obj["DriverVersion"]);
					//Console.Out.WriteLine(info);
					if (obj["DeviceName"].ToString().Contains("MC8000P"))   // 신형 드라이버
					{
						if (obj["DriverVersion"].ToString().Contains("12.1.0.0")) { vVer = "V8"; break; }   // 신형 드라이버
						else if (obj["DriverVersion"].ToString().Contains("1.0.0.0")) { vVer = "V1"; break; }   // 구형 드라이버
						else vVer = "V0";   // 드라이버가 없을 시
					}
					else if (obj["DeviceName"].ToString().Contains("MC8082"))   // 구형 드라이버
					{
						if (obj["DriverVersion"].ToString().Contains("12.1.0.0")) { vVer = "V8"; break; }   // 신형 드라이버
						else if (obj["DriverVersion"].ToString().Contains("1.0.0.1")) { vVer = "V1"; break; }   // 구형 드라이버
						else vVer = "V0";   // 드라이버가 없을 시
					}
					else vVer = "V0";
				}
				return vVer.ToUpper().Trim();
			}

			/*
			 * // 동적 로딩 예제
						[DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
						static extern int LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpLibFileName);
						[DllImport("kernel32.dll", EntryPoint = "GetProcAddress")]
						static extern IntPtr GetProcAddress(int hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
						[DllImport("kernel32.dll", EntryPoint = "FreeLibrary")]
						static extern bool FreeLibrary(int hModule);

						// 예제
						//int hModule = LoadLibrary(path_to_your_dll);  // you can build it dynamically
						//if (hModule == 0) return;
						//IntPtr intPtr = GetProcAddress(hModule, method_name);
						//CallMethod action = (CallMethod)Marshal.GetDelegateForFunctionPointer(intPtr, typeof(CallMethod));
						//action.Invoke();
			 */


			//public static void Nmc_Open(int No, int IntrptFlg)
			//{
			//	if (Ver == "V1") V1.Nmc_Open(No, IntrptFlg);
			//	else if (Ver == "V8") V8.Nmc_Open(No, IntrptFlg);
			//	else throw new Exception();
			//}
			//public static int Nmc_Open(int No, int IntrptFlg) { return (Ver == "V1") ? V1.Nmc_Open(No, IntrptFlg) : V8.Nmc_Open(No, IntrptFlg); }

			//public static Nmc_Status Nmc_2BPExec(int No, int IcNo, DATA_2BP[] pData2Bp, int DataCnt, int IpAxis, bool ContinueFlg);
			//public static Nmc_Status Nmc_2BPExec_BG(IntPtr User_hWnd, int No, int IcNo, DATA_2BP[] pData2Bp, int DataCnt, int IpAxis, bool ContinueFlg);
			//public static Nmc_Status Nmc_2CIPExec(int No, int IcNo, DATA_2CIP[] pData2Cip, int DataCnt, int IpAxis, bool SpdChgFlg, bool ContinueFlg);
			//public static Nmc_Status Nmc_2CIPExec_BG(IntPtr User_hWnd, int No, int IcNo, DATA_2CIP[] pData2Cip, int DataCnt, int IpAxis, bool SpdChgFlg, bool ContinueFlg);
			//public static Nmc_Status Nmc_3BPExec(int No, int IcNo, DATA_3BP[] pData3Bp, int DataCnt, int IpAxis, bool ContinueFlg);
			//public static Nmc_Status Nmc_3BPExec_BG(IntPtr User_hWnd, int No, int IcNo, DATA_3BP[] pData3Bp, int DataCnt, int IpAxis, bool ContinueFlg);
			//public static Nmc_Status Nmc_3CIPExec(int No, int IcNo, DATA_3CIP[] pData3Cip, int DataCnt, int IpAxis, bool SpdChgFlg, bool ContinueFlg);
			//public static Nmc_Status Nmc_3CIPExec_BG(IntPtr User_hWnd, int No, int IcNo, DATA_3CIP[] pData3Cip, int DataCnt, int IpAxis, bool SpdChgFlg, bool ContinueFlg);


			////////////////////////////////////////////////// //////////////////////////////////
			////
			//// 드라이버 함수
			////
			////////////////////////////////////////////////// //////////////////////////////////

			/// <summary>
			///  DLL 파일 버전이 다를때 사용할 Wrap Function
			/// </summary>

			public static void Nmc_Acc(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Acc(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Acc(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_AccOfst(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_AccOfst(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_AccOfst(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Center(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Center(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Center(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_CheckEvent(int No, int IcNo, out int Rr3X, out int Rr3Y, out int Rr3Z, out int Rr3U)
			//{
			//}
			//public static bool Nmc_Close(int No)
			public static int Nmc_Close(int No)
			{
				if (Ver == "V1") return V1.Nmc_Close(No);
				else if (Ver == "V8") return V8.Nmc_Close(No);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_CloseAll();
			public static int Nmc_CloseAll()
			{
				if (Ver == "V1") return V1.Nmc_CloseAll();
				else if (Ver == "V8") return V8.Nmc_CloseAll();
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Command(int No, int IcNo, AXIS axis, CMD cmd)
			{
				if (Ver == "V1") V1.Nmc_Command(No, IcNo, axis, cmd);
				else if (Ver == "V8") V8.Nmc_Command(No, IcNo, axis, cmd);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Command_IP(int No, int IcNo, CMD cmd)
			{
				if (Ver == "V1") V1.Nmc_Command_IP(No, IcNo, cmd);
				else if (Ver == "V8") V8.Nmc_Command_IP(No, IcNo, cmd);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_CompM(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_CompM(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_CompM(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_CompP(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_CompP(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_CompP(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Dec(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Dec(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Dec(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_DecP(int No, int IcNo, AXIS axis, uint wdata)
			{
				if (Ver == "V1") V1.Nmc_DecP(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_DecP(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_DJerk(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_DJerk(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_DJerk(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Ep(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Ep(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Ep(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_ExpMode(int No, int IcNo, AXIS axis, int EM6_data, int EM7_data)
			{
				if (Ver == "V1") V1.Nmc_ExpMode(No, IcNo, axis, EM6_data, EM7_data);
				else if (Ver == "V8") V8.Nmc_ExpMode(No, IcNo, axis, EM6_data, EM7_data);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_GetBoardInfo(int No, out ushort DeviceID);
			public static int Nmc_GetBoardInfo(int No, ref ushort DeviceID)
			{
				if (Ver == "V1") return V1.Nmc_GetBoardInfo(No, ref DeviceID);
				else if (Ver == "V8") return V8.Nmc_GetBoardInfo(No, ref DeviceID);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_GetBpSc(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_GetBpSc(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_GetBpSc(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_GetCNextStatus(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_GetCNextStatus(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_GetCNextStatus(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_GetDriveStatus(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_GetDriveStatus(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_GetDriveStatus(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_HomeMode(int No, int IcNo, AXIS axis, int SM6_data)
			{
				if (Ver == "V1") V1.Nmc_HomeMode(No, IcNo, axis, SM6_data);
				else if (Ver == "V8") V8.Nmc_HomeMode(No, IcNo, axis, SM6_data);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_HomeSpd(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_HomeSpd(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_HomeSpd(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_InPort(int No, REG_MCX reg)
			{
				if (Ver == "V1") return V1.Nmc_InPort(No, reg);
				else if (Ver == "V8") return V8.Nmc_InPort(No, reg);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_IPGetMsgNo(int wParam, ref int No, ref int IcNo)
			{
				if (Ver == "V1") V1.Nmc_IPGetMsgNo(wParam, ref No, ref IcNo);
				else if (Ver == "V8") V8.Nmc_IPGetMsgNo(wParam, ref No, ref IcNo);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_IPStop(int No, int IcNo)
			public static int Nmc_IPStop(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_IPStop(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_IPStop(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Jerk(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Jerk(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Jerk(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Lp(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Lp(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Lp(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_Open(int No, bool IntrptFlg);
			public static int Nmc_Open(int No, bool IntrptFlg)
			{
				if (Ver == "V1") return V1.Nmc_Open(No, IntrptFlg);
				else if (Ver == "V8") return V8.Nmc_Open(No, IntrptFlg);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_OutPort(int No, REG_MCX reg, int Dat)
			{
				if (Ver == "V1") V1.Nmc_OutPort(No, reg, Dat);
				else if (Ver == "V8") V8.Nmc_OutPort(No, reg, Dat);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Pulse(int No, int IcNo, AXIS axis, uint wdata)
			{
				if (Ver == "V1") V1.Nmc_Pulse(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Pulse(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Pulse(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Pulse(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Pulse(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Range(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Range(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Range(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadAccDec(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadAccDec(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadAccDec(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadData(int No, int IcNo, AXIS axis, CMD cmd)
			{
				if (Ver == "V1") return V1.Nmc_ReadData(No, IcNo, axis, cmd);
				else if (Ver == "V8") return V8.Nmc_ReadData(No, IcNo, axis, cmd);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadEp(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadEp(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadEp(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_ReadEvent(int No, int IcNo, ref int Rr3X, ref int Rr3Y, ref int Rr3Z, ref int Rr3U)
			//{
			//	if (Ver == "V1") return V1.Nmc_ReadEvent(No, IcNo, ref Rr3X, ref Rr3Y, ref Rr3Z, ref Rr3U);
			//	else if (Ver == "V8") return V8.Nmc_ReadEvent(No, IcNo, ref Rr3X, ref Rr3Y, ref Rr3Z, ref Rr3U);
			//	else throw new Exception("Function Not Found!!");
			//}
			public static int Nmc_ReadLp(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadLp(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadLp(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg(int No, int IcNo, int RegNum)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg(No, IcNo, RegNum);
				else if (Ver == "V8") return V8.Nmc_ReadReg(No, IcNo, RegNum);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg0(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg0(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_ReadReg0(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg1(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg1(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadReg1(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg2(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg2(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadReg2(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg3(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg3(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadReg3(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg4(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg4(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_ReadReg4(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg5(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg5(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_ReadReg5(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg6(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg6(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_ReadReg6(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadReg7(int No, int IcNo)
			{
				if (Ver == "V1") return V1.Nmc_ReadReg7(No, IcNo);
				else if (Ver == "V8") return V8.Nmc_ReadReg7(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadRegSetAxis(int No, int IcNo, AXIS axis, int RegNumber)
			{
				if (Ver == "V1") return V1.Nmc_ReadRegSetAxis(No, IcNo, axis, RegNumber);
				else if (Ver == "V8") return V8.Nmc_ReadRegSetAxis(No, IcNo, axis, RegNumber);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadSpeed(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadSpeed(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadSpeed(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static int Nmc_ReadSyncBuff(int No, int IcNo, AXIS axis)
			{
				if (Ver == "V1") return V1.Nmc_ReadSyncBuff(No, IcNo, axis);
				else if (Ver == "V8") return V8.Nmc_ReadSyncBuff(No, IcNo, axis);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_Reset(int No, int IcNo)
			{
				if (Ver == "V1") V1.Nmc_Reset(No, IcNo);
				else if (Ver == "V8") V8.Nmc_Reset(No, IcNo);
				else throw new Exception("Function Not Found!!");
			}
			//public static bool Nmc_ResetEvent(int No)
			//public static int Nmc_ResetEvent(int No)
			//{
			//	if (Ver == "V1") return V1.Nmc_ResetEvent(No);
			//	else if (Ver == "V8") return V8.Nmc_ResetEvent(No);
			//	else throw new Exception("Function Not Found!!");
			//}
			//public static bool Nmc_SetEvent(int No, UserThread Callback, int param);
			public static void Nmc_Speed(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_Speed(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_Speed(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_StartSpd(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_StartSpd(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_StartSpd(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_SyncMode(int No, int IcNo, AXIS axis, int SM6_data, int SM7_data)
			{
				if (Ver == "V1") V1.Nmc_SyncMode(No, IcNo, axis, SM6_data, SM7_data);
				else if (Ver == "V8") V8.Nmc_SyncMode(No, IcNo, axis, SM6_data, SM7_data);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteData(int No, int IcNo, AXIS axis, CMD cmd, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteData(No, IcNo, axis, cmd, wdata);
				else if (Ver == "V8") V8.Nmc_WriteData(No, IcNo, axis, cmd, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteData2(int No, int IcNo, AXIS axis, CMD cmd, int WR6_data, int WR7_data)
			{
				if (Ver == "V1") V1.Nmc_WriteData2(No, IcNo, axis, cmd, WR6_data, WR7_data);
				else if (Ver == "V8") V8.Nmc_WriteData2(No, IcNo, axis, cmd, WR6_data, WR7_data);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg(int No, int IcNo, int RegNum, int Dat)
			{
				if (Ver == "V1") V1.Nmc_WriteReg(No, IcNo, RegNum, Dat);
				else if (Ver == "V8") V8.Nmc_WriteReg(No, IcNo, RegNum, Dat);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg0(int No, int IcNo, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg0(No, IcNo, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg0(No, IcNo, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg1(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg1(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg1(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg2(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg2(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg2(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg3(int No, int IcNo, AXIS axis, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg3(No, IcNo, axis, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg3(No, IcNo, axis, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg4(int No, int IcNo, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg4(No, IcNo, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg4(No, IcNo, wdata);
				else throw new Exception("Function Not Found!!");
			}
			public static void Nmc_WriteReg5(int No, int IcNo, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg5(No, IcNo, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg5(No, IcNo, wdata);
				else throw new Exception("Function Not Found!!");
			}

			public static void Nmc_WriteReg6(int No, int IcNo, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg6(No, IcNo, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg6(No, IcNo, wdata);
				else throw new Exception("Function Not Found!!");
			}

			public static void Nmc_WriteReg7(int No, int IcNo, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteReg7(No, IcNo, wdata);
				else if (Ver == "V8") V8.Nmc_WriteReg7(No, IcNo, wdata);
				else throw new Exception("Function Not Found!!");
			}

			public static void Nmc_WriteRegSetAxis(int No, int IcNo, AXIS axis, int RegNumber, int wdata)
			{
				if (Ver == "V1") V1.Nmc_WriteRegSetAxis(No, IcNo, axis, RegNumber, wdata);
				else if (Ver == "V8") V8.Nmc_WriteRegSetAxis(No, IcNo, axis, RegNumber, wdata);
				else throw new Exception("Function Not Found!!");
			}

			// DLL 버전 1용 라이브러리 70Ea
			private static class V1
			{
				private const string DllFileName = "MC8000Pv0.DLL";
				// 오픈 입출력 닫힘
				// 참고 : 제 1 축, 2 축, 3 축, WR5로 지정
				[DllImport(DllFileName, EntryPoint = "Nmc_Open", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 오픈
				public static extern int Nmc_Open(int No, bool IntrptFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 폐쇄
				public static extern int Nmc_Close(int No);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 모두 폐쇄
				public static extern int Nmc_CloseAll();

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드 정보 취득
				public static extern int Nmc_GetBoardInfo(int No, ref ushort DeviceID);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드에서 포트 입력
				public static extern int Nmc_InPort(int No, REG_MCX adr);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드에 포트 출력
				public static extern void Nmc_OutPort(int No, REG_MCX adr, int Dat);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드의 레지스터 쓰기
				public static extern void Nmc_WriteReg(int No, int IcNo, int RegNum, int Dat);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드의 레지스터 읽기
				public static extern int Nmc_ReadReg(int No, int IcNo, int RegNum);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 리셋 명령
				public static extern void Nmc_Reset(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 재설정
				public static extern void Nmc_Command(int No, int IcNo, AXIS Axis, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 축 지정 명령 쓰기 (명령 실행) // 보간 명령 쓰기 (명령 실행) // 라이트 레지스터
				public static extern void Nmc_Command_IP(int No, int IcNo, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 0 설정
				public static extern void Nmc_WriteReg0(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 1 설정
				public static extern void Nmc_WriteReg1(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 2 설정
				public static extern void Nmc_WriteReg2(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 3 설정
				public static extern void Nmc_WriteReg3(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 4 설정
				public static extern void Nmc_WriteReg4(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 5 설정
				public static extern void Nmc_WriteReg5(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 6 설정
				public static extern void Nmc_WriteReg6(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 7 설정
				public static extern void Nmc_WriteReg7(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 // 리드 레지스터 0 읽기
				public static extern int Nmc_ReadReg0(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 1 읽기
				public static extern int Nmc_ReadReg1(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 2 읽기
				public static extern int Nmc_ReadReg2(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 3 읽기
				public static extern int Nmc_ReadReg3(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 4 읽기
				public static extern int Nmc_ReadReg4(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 5 읽기
				public static extern int Nmc_ReadReg5(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 6 읽기
				public static extern int Nmc_ReadReg6(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 7 읽기
				public static extern int Nmc_ReadReg7(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 파라미터 설정 // 범위 설정
				public static extern void Nmc_Range(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속도 증가율 (가가 속도) (K) 설정
				public static extern void Nmc_Jerk(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속도 (A) 설정
				public static extern void Nmc_Acc(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 감속도 (D) 설정
				public static extern void Nmc_Dec(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 초속 (SV) 설정
				public static extern void Nmc_StartSpd(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 드라이브 속도 (V) 설정
				public static extern void Nmc_Speed​​(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 원호 중심점 (C) 설정 ※ MCX314As 전용
				public static extern void Nmc_Center(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 논리 위치 카운터 (LP) 설정
				public static extern void Nmc_Lp(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 실제 위치 카운터 (EP) 설정
				public static extern void Nmc_Ep(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // COMP + (CP) 설정
				public static extern void Nmc_CompP(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // COMP- (CM) 설정
				public static extern void Nmc_CompM(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속 카운터 오프셋 (AO) 설정
				public static extern void Nmc_AccOfst(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 감속 증가율 (L) 설정 ※ MCX314As 전용
				public static extern void Nmc_DJerk(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 원점 검출 속도 (HV) 설정
				public static extern void Nmc_HomeSpd(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 출력 펄스 수 / 보간 종점 (P) 설정 (VB)
				public static extern void Nmc_Pulse(int No, int IcNo, AXIS Axis, uint wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 출력 펄스 수 / 보간 종점 (P) 설정 (VB)
				public static extern void Nmc_Pulse(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 매뉴얼 감속 점 (DP) 설정 (VB)
				public static extern void Nmc_DecP(int No, int IcNo, AXIS Axis, double wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 기타 모드 설정 // 확장 모드 (EM) 설정 ※ MCX314As 전용
				public static extern void Nmc_ExpMode(int No, int IcNo, AXIS Axis, int EM6_data, int EM7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 동기 동작 모드 (SM) 설정 ※ MCX314As 전용
				public static extern void Nmc_SyncMode(int No, int IcNo, AXIS Axis, int SM6_data, int SM7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 자동 원점 출력 모드 (HM) 설정 ※ MCX304 전용
				public static extern void Nmc_HomeMode(int No, int IcNo, AXIS Axis, int WR6_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 논리 위치 카운터 값 (LP) 읽기
				public static extern int Nmc_ReadLp(int No, int IcNo, AXIS Axis);

				// 데이터 읽기
				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 실제 위치 카운터 값 (EP) 읽기
				public static extern int Nmc_ReadEp(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 현재 드라이브 속도 (CV) 읽기
				public static extern int Nmc_ReadSpeed​(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 현재 가압 / 감속도 (CA) 읽기
				public static extern int Nmc_ReadAccDec(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 동기화 버퍼 레지스터 (BR) 읽기 ※ MCX314As 전용
				public static extern int Nmc_ReadSyncBuff(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 드라이브 상태 검색 0 : 드라이브 종료 0 이외 : 드라이브 중
				public static extern int Nmc_GetDriveStatus(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 연속 보간 다음 데이터 쓰기 가능 상태 검색 0 : 다음 데이터 쓰기 불가, 0 이외 : 다음 데이터 쓰기 가능 ※ MCX314As 전용
				public static extern int Nmc_GetCNextStatus(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // BP 보간 스택 카운터 취득 0-3 ※ MCX314As 전용
				public static extern int Nmc_GetBpSc(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 쓰기 · 읽기 // 라이트 레지스터 쓰기 축 설정 (WR1 ~ 3)
				public static extern void Nmc_WriteRegSetAxis(int No, int IcNo, AXIS Axis, int RegNumber, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 리드 레지스터 읽기 축 설정 (RR1 ~ 3)
				public static extern int Nmc_ReadRegSetAxis(int No, int IcNo, AXIS Axis, int RegNumber);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 쓰기 (매개 변수)
				public static extern void Nmc_WriteData(int No, int IcNo, AXIS Axis, CMD cmd, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 쓰기 (확장 모드, 동기화 모드) ※ MCX314As 전용
				public static extern void Nmc_WriteData2(int No, int IcNo, AXIS Axis, CMD cmd, int WR6_data, int WR7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 읽기
				public static extern int Nmc_ReadData(int No, int IcNo, AXIS Axis, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간 (BP 보간 / 연속 보간) ※ MCX314As 전용	// 2 축 BP 보간 실행
				public static extern int Nmc_2BPExec(int No, int IcNo, ref DATA_2BP Data2Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 BP 보간 실행
				public static extern int Nmc_3BPExec(int No, int IcNo, ref DATA_3BP Data3Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 BP 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_2BPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_2BP Data2Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 BP 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_3BPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_3BP Data3Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 연속 보간 실행
				public static extern int Nmc_2CIPExec(int No, int IcNo, ref DATA_2CIP Data2Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 연속 보간 실행
				public static extern int Nmc_3CIPExec(int No, int IcNo, ref DATA_3CIP Data3Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 연속 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_2CIPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_2CIP Data2Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 연속 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_3CIPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_3CIP Data3Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간을 중단 (보간 드라이브를 즉시 정지하고, 상기 보간 함수에서 실행중인 보간 처리를 종료한다)
				public static extern int Nmc_IPStop(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간 종료시받은 메시지 (wParam)에서 보드 번호와 IC 번호를 얻을
				public static extern void Nmc_IPGetMsgNo(int wParam, ref int No, ref int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 스레드 콜백
				public static extern bool Nmc_SetEvent(int No, UserThread Callback, int param);
			}

			// DLL 버전 8용 라이브러리 70Ea
			private static class V8
			{
				private const string DllFileName = "MC8000PV8.DLL";
				// 오픈 입출력 닫힘
				// 참고 : 제 1 축, 2 축, 3 축, WR5로 지정
				[DllImport(DllFileName, EntryPoint = "Nmc_Open", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 오픈
				public static extern int Nmc_Open(int No, bool IntrptFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 폐쇄
				public static extern int Nmc_Close(int No);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드를 모두 폐쇄
				public static extern int Nmc_CloseAll();

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드 정보 취득
				public static extern int Nmc_GetBoardInfo(int No, ref ushort DeviceID);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드에서 포트 입력
				public static extern int Nmc_InPort(int No, REG_MCX adr);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드에 포트 출력
				public static extern void Nmc_OutPort(int No, REG_MCX adr, int Dat);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드의 레지스터 쓰기
				public static extern void Nmc_WriteReg(int No, int IcNo, int RegNum, int Dat);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보드의 레지스터 읽기
				public static extern int Nmc_ReadReg(int No, int IcNo, int RegNum);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 리셋 명령
				public static extern void Nmc_Reset(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 재설정
				public static extern void Nmc_Command(int No, int IcNo, AXIS Axis, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 축 지정 명령 쓰기 (명령 실행) // 보간 명령 쓰기 (명령 실행) // 라이트 레지스터
				public static extern void Nmc_Command_IP(int No, int IcNo, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 0 설정
				public static extern void Nmc_WriteReg0(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 1 설정
				public static extern void Nmc_WriteReg1(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 2 설정
				public static extern void Nmc_WriteReg2(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 3 설정
				public static extern void Nmc_WriteReg3(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 4 설정
				public static extern void Nmc_WriteReg4(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 5 설정
				public static extern void Nmc_WriteReg5(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 6 설정
				public static extern void Nmc_WriteReg6(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 라이트 레지스터 7 설정
				public static extern void Nmc_WriteReg7(int No, int IcNo, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 // 리드 레지스터 0 읽기
				public static extern int Nmc_ReadReg0(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 1 읽기
				public static extern int Nmc_ReadReg1(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 2 읽기
				public static extern int Nmc_ReadReg2(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 3 읽기
				public static extern int Nmc_ReadReg3(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 4 읽기
				public static extern int Nmc_ReadReg4(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 5 읽기
				public static extern int Nmc_ReadReg5(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 6 읽기
				public static extern int Nmc_ReadReg6(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]    // 리드 레지스터 7 읽기
				public static extern int Nmc_ReadReg7(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 파라미터 설정 // 범위 설정
				public static extern void Nmc_Range(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속도 증가율 (가가 속도) (K) 설정
				public static extern void Nmc_Jerk(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속도 (A) 설정
				public static extern void Nmc_Acc(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 감속도 (D) 설정
				public static extern void Nmc_Dec(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 초속 (SV) 설정
				public static extern void Nmc_StartSpd(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 드라이브 속도 (V) 설정
				public static extern void Nmc_Speed​​(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 원호 중심점 (C) 설정 ※ MCX314As 전용
				public static extern void Nmc_Center(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 논리 위치 카운터 (LP) 설정
				public static extern void Nmc_Lp(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 실제 위치 카운터 (EP) 설정
				public static extern void Nmc_Ep(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // COMP + (CP) 설정
				public static extern void Nmc_CompP(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // COMP- (CM) 설정
				public static extern void Nmc_CompM(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 가속 카운터 오프셋 (AO) 설정
				public static extern void Nmc_AccOfst(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 감속 증가율 (L) 설정 ※ MCX314As 전용
				public static extern void Nmc_DJerk(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 원점 검출 속도 (HV) 설정
				public static extern void Nmc_HomeSpd(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 출력 펄스 수 / 보간 종점 (P) 설정 (VB)
				public static extern void Nmc_Pulse(int No, int IcNo, AXIS Axis, uint wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 출력 펄스 수 / 보간 종점 (P) 설정 (VB)
				public static extern void Nmc_Pulse(int No, int IcNo, AXIS Axis, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 매뉴얼 감속 점 (DP) 설정 (VB)
				public static extern void Nmc_DecP(int No, int IcNo, AXIS Axis, double wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 기타 모드 설정 // 확장 모드 (EM) 설정 ※ MCX314As 전용
				public static extern void Nmc_ExpMode(int No, int IcNo, AXIS Axis, int EM6_data, int EM7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 동기 동작 모드 (SM) 설정 ※ MCX314As 전용
				public static extern void Nmc_SyncMode(int No, int IcNo, AXIS Axis, int SM6_data, int SM7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 자동 원점 출력 모드 (HM) 설정 ※ MCX304 전용
				public static extern void Nmc_HomeMode(int No, int IcNo, AXIS Axis, int WR6_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 논리 위치 카운터 값 (LP) 읽기
				public static extern int Nmc_ReadLp(int No, int IcNo, AXIS Axis);

				// 데이터 읽기
				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 실제 위치 카운터 값 (EP) 읽기
				public static extern int Nmc_ReadEp(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 현재 드라이브 속도 (CV) 읽기
				public static extern int Nmc_ReadSpeed​(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 현재 가압 / 감속도 (CA) 읽기
				public static extern int Nmc_ReadAccDec(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 동기화 버퍼 레지스터 (BR) 읽기 ※ MCX314As 전용
				public static extern int Nmc_ReadSyncBuff(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 드라이브 상태 검색 0 : 드라이브 종료 0 이외 : 드라이브 중
				public static extern int Nmc_GetDriveStatus(int No, int IcNo, AXIS Axis);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 연속 보간 다음 데이터 쓰기 가능 상태 검색 0 : 다음 데이터 쓰기 불가, 0 이외 : 다음 데이터 쓰기 가능 ※ MCX314As 전용
				public static extern int Nmc_GetCNextStatus(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // BP 보간 스택 카운터 취득 0-3 ※ MCX314As 전용
				public static extern int Nmc_GetBpSc(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 쓰기 · 읽기 // 라이트 레지스터 쓰기 축 설정 (WR1 ~ 3)
				public static extern void Nmc_WriteRegSetAxis(int No, int IcNo, AXIS Axis, int RegNumber, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 리드 레지스터 읽기 축 설정 (RR1 ~ 3)
				public static extern int Nmc_ReadRegSetAxis(int No, int IcNo, AXIS Axis, int RegNumber);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 쓰기 (매개 변수)
				public static extern void Nmc_WriteData(int No, int IcNo, AXIS Axis, CMD cmd, int wdata);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 쓰기 (확장 모드, 동기화 모드) ※ MCX314As 전용
				public static extern void Nmc_WriteData2(int No, int IcNo, AXIS Axis, CMD cmd, int WR6_data, int WR7_data);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 데이터 읽기
				public static extern int Nmc_ReadData(int No, int IcNo, AXIS Axis, CMD cmd);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간 (BP 보간 / 연속 보간) ※ MCX314As 전용	// 2 축 BP 보간 실행
				public static extern int Nmc_2BPExec(int No, int IcNo, ref DATA_2BP Data2Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 BP 보간 실행
				public static extern int Nmc_3BPExec(int No, int IcNo, ref DATA_3BP Data3Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 BP 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_2BPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_2BP Data2Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 BP 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_3BPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_3BP Data3Bp, int DataCnt, int IpAxis, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 연속 보간 실행
				public static extern int Nmc_2CIPExec(int No, int IcNo, ref DATA_2CIP Data2Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 연속 보간 실행
				public static extern int Nmc_3CIPExec(int No, int IcNo, ref DATA_3CIP Data3Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 2 축 연속 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_2CIPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_2CIP Data2Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 3 축 연속 보간 실행 (백그라운드에서 실행)
				public static extern int Nmc_3CIPExec_BG(int User_hWnd, int No, int IcNo, ref DATA_3CIP Data3Cip, int DataCnt, int IpAxis, int SpdChgFlg, int ContinueFlg);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간을 중단 (보간 드라이브를 즉시 정지하고, 상기 보간 함수에서 실행중인 보간 처리를 종료한다)
				public static extern int Nmc_IPStop(int No, int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 보간 종료시받은 메시지 (wParam)에서 보드 번호와 IC 번호를 얻을
				public static extern void Nmc_IPGetMsgNo(int wParam, ref int No, ref int IcNo);

				[DllImport(DllFileName, CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]   // 스레드 콜백
				public static extern bool Nmc_SetEvent(int No, UserThread Callback, int param);
			}


		}

	}
}
//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
