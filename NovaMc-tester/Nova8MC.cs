using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Free302.TnM.Device;
using static Free302.TnM.Device.Nova8BWrap;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace serverapp_tester
{
	public class Nova8MC
	{
		// 보드번호
		public static int mBoardNo { get; set; } = -1;

		/// <summary>
		/// 환경 설정값
		/// </summary>
		
		// IC 칩별 축 매칭 (AX-X, AY-Y, ..., BX-X, ...)
		public static Dictionary<McAxis, NmcAxis> mAxisMap = new Dictionary<McAxis, NmcAxis> { };
		public static int[] mUnitConversionFactor { get; set; } = { 8000000, 8000000, 8000000, 8000000, 8000000, 8000000, 8000000, 8000000 };
		// 스피트 설정 값, 8축 축별로 지정
		public static int[] mSpeed { get; set; } = { 1900, 1900, 1900, 1900, 1900, 1900, 1900, 1900 };
		// 홈 위치 값, 8축 축별로 지정
		public static int[] mHomePos { get; set; } = { 0, 0, 0, 0, 0, 0, 0, 0 };
		// 펄스 모드 값, 8축 축별로 지정, 1펄스/2펄스
		public static PulseMode[] mPulseMode { get; set; } = { PulseMode.OnePulse, PulseMode.OnePulse, PulseMode.OnePulse, PulseMode.OnePulse
			, PulseMode.OnePulse, PulseMode.OnePulse, PulseMode.OnePulse, PulseMode.OnePulse};
		// 축의 이동 가능 거리, 8축 축별로 지정
		public static int[] mAxislength { get; set; } = { 0, 0, 0, 0, 0, 0, 0, 0 };
		// 배율
		public static int[] mRatio { get; set; } = { 20, 20, 20, 20, 20, 20, 20, 20 };

		// 스테이지의 모터 해상도 (거리/Pulse)
		// Full/Half Step
		//public static double[] mUnitToPolar = { 1, 0.5, 2, 0.00268, 0.0012, 0.0012, 1, 0.5 };   // ex) 선형축 => 1, 원형축 => 0.0032
		//public static double[] mUnitToPolar = { 2, 0.00268, 0.0012, 0.0012, 20, 1, 1, 1 };
		public static double[] mUnitToPolar = { 1, 1, 1, 1, 1, 1, 1, 1 };



		/// <summary>
		/// 실시간 상태값
		/// </summary>

		// M/C 보드 열기 상태 값, M/C 보드 기준
		public static bool mIsOpen { get; set; } = false;
		// 축 현재 논리적 위치 상태값, 8축 축별로
		public static int[] mIsPosition { get; set; } = { -1, -1, -1, -1, -1, -1, -1, -1 };
		// 정지시 상태 값, 8축 축별로 지정
		public static bool[] mIsStop { get; set; } = { false, false, false, false, false, false, false, false };
		// CW 방향 리미트 상태 값, 8축 축별로 지정
		public static bool[] mIsLimitCW { get; set; } = { false, false, false, false, false, false, false, false };
		// CCW 방향 리미트 상태 값, 8축 축별로 지정
		public static bool[] mIsLimitCCW { get; set; } = { false, false, false, false, false, false, false, false };
		// 리미트센서가 리미트 일때 : 하드웨어 리미트 Active High=>true인지 Low=>false인지 값
		//public static bool[] mIsLimitHighLow { get; set; } = { false, true, true, true, true, true, true, true };
		public static bool[] mIsLimitLevel { get; set; } = { false, false, false, false, false, false, false, false };


		// 현재 M/C 보드의 동작 상태 정보
		public enum Status
		{
			MoveCW, MoveCCW, Zero, Home, Stop, MoveStop, ZeroStop, HomeStop
		}
		// 모터 제어용 드라이브 펄스 모드
		public enum PulseMode : UInt16
		{
			OnePulse = 0x0040, TwoPulse = 0
		}
		// 모터 제어용 시그널 펄스의 TTL
		public enum PulseLevel : UInt16
		{
			PulseLow = 0x0080, PulseHigh = 0
		}
		// 모터 제어용 1 Puls 용 방향 지정
		public enum Direction : UInt16
		{
			CWHigh = 0x0100, CCWLow = 0
		}
		// 모터 제어용 H/W 리미트 모드
		public enum LimitHW : UInt16
		{
			CWLMT = 0x0008, CCWLMT = 0x0010, None = 0
		}

		// 배열 변수에서 축번호 위치 첨자
		public enum AxisArrayNo
		{
			AX = 0, AY = 1, AZ = 2, AU = 3, BX = 4, BY = 5, BZ = 6, BU = 7
		}

		// 축의 물리적 위치 및 이름 8축
		// UI 개발 시 UI 쪽과 맞출 필요 있음
		public enum McAxis
		{
			None = 0x0, AX = 0x01, AY = 0x02, AZ = 0x04, AU = 0x08, BX = 0x10, BY = 0x20, BZ = 0x40, BU = 0x80,
			All = AX | AY | AU | AZ | BX | BY | BZ | BU
		}

		// 클래스 생성자
		static Nova8MC()
		{
			Nova8MC.mAxisMap.Clear();

			// Dictionary 목록 추가
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.AX, NmcAxis.X);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.AY, NmcAxis.Y);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.AZ, NmcAxis.Z);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.AU, NmcAxis.U);

			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.BX, NmcAxis.X);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.BY, NmcAxis.Y);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.BZ, NmcAxis.Z);
			Nova8MC.mAxisMap.Add(Nova8MC.McAxis.BU, NmcAxis.U);
		}

		/// <summary>
		/// 지정된 축의 정보를 딕셔너리에서 가져옴
		/// </summary>
		/// <param name="pAxis"></param>
		/// <returns></returns>
		//public static NovaMcAxis getAxis(McAxis pAxis)
		public static NmcAxis getAxis(McAxis pAxis = McAxis.All)
		{
			return mAxisMap[pAxis];
		}


		/// <summary>
		/// M/C 보드 1개를 열기
		/// </summary>
		/// <param name="pNo">보드번호, 전역 보드번호 설정</param>
		/// <returns></returns>
		public static bool Open(int pNo)
		{
			try
			{
				// M/C 보드 열기
				mIsOpen = Convert.ToBoolean(MC8000P.Nmc_Open(pNo, false));

				// 열기 오류이면
				if (!mIsOpen) return false;

				MC8000P.Nmc_Reset(pNo, 0);
				MC8000P.Nmc_Reset(pNo, 1);

				MC8000P.Nmc_WriteReg1(pNo, 0, AXIS.ALL, 0x0000);
				MC8000P.Nmc_WriteReg1(pNo, 1, AXIS.ALL, 0x0000);

				// HLMT+, HLMT- 설정
				MC8000P.Nmc_WriteReg2(pNo, 0, AXIS.ALL, 0x0018);
				MC8000P.Nmc_WriteReg2(pNo, 1, AXIS.ALL, 0x0018);


				// WR3 입력 신호의 전 필터 설정 및 입력 신호 지연 010 => 512μs -> 0.5msec
				MC8000P.Nmc_WriteReg3(pNo, 0, AXIS.ALL, 0x4F00);
				MC8000P.Nmc_WriteReg3(pNo, 1, AXIS.ALL, 0x4F00);

				// 일반 출력신호 값 설정
				MC8000P.Nmc_WriteReg4(pNo, 0, 0x0000);
				MC8000P.Nmc_WriteReg4(pNo, 1, 0x0000);

				// 범용 출력신호 활성여부 설정
				//MC8000P.Nmc_WriteReg5(mBoardNo, 0, 0x0000);
				//MC8000P.Nmc_WriteReg5(mBoardNo, 1, 0x0000);
				MC8000P.Nmc_WriteReg5(pNo, 0, 0xffff);
				MC8000P.Nmc_WriteReg5(pNo, 1, 0xffff);

				// 
				MC8000P.Nmc_AccOfst(pNo, 0, AXIS.ALL, 0);
				MC8000P.Nmc_AccOfst(pNo, 1, AXIS.ALL, 0);

				// 보드번호
				mBoardNo = pNo;

				return true;
			}
			catch
			{
				mBoardNo = -1;
				return false;
			}
		}

		/// <summary>
		/// M/C 보드 1개의 인스턴스를 닫음
		/// </summary>
		/// <param name="pNo"></param>
		/// <returns></returns>
		public static bool Close(int pNo)
		{
			try
			{
				MC8000P.Nmc_Close(pNo);
				mBoardNo = -1;
				mIsOpen = false;
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// 리미트센서 액티브레벨 설정
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		public static void SetLimitLevel(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);


			//public const UInt16 WR2_LMT_HLMTP = 0x0008;// +방향 limit입력신호(nLMTP)의 논리 level설정(0:low,1:high에서 active)
			//public const UInt16 WR2_LMT_HLMTM = 0x0010;// -방향 limit입력신호(nLMTM)의 논리 level설정(0:low,1:high에서 active)
			// 1 펄스, +방향 H/W 리미트, -방향 H/W 리미트, +방향 일때 High : -방향 일때 Low
			//var vConfig = (UInt16)(WR2_DRV_PLSMD | WR2_LMT_HLMTP | WR2_LMT_HLMTM | WR2_LMT_LMTMD | WR2_DRV_DIR_L);
			//var vConfig = (UInt16)(WR2_DRV_PLSMD | WR2_LMT_HLMTP | WR2_LMT_HLMTM | WR2_DRV_DIR_L);

			UInt16 vConfig = 0;
			vConfig |= (UInt16)PulseMode.OnePulse;
			if (mIsLimitLevel[vArrayNo])
			{
				vConfig |= (UInt16)LimitHW.CWLMT;
				vConfig |= (UInt16)LimitHW.CCWLMT;
			}
			vConfig |= (UInt16)Direction.CWHigh;

			Nova8BWrap.MC8000P.Nmc_WriteReg2(pNo, icNo, AXIS.ALL, vConfig);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		public static void MoveInit(int pNo, McAxis pAXIS)
		{
			
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);


			// 배율 범위 설정
			//int R = PmcHwInfo.R(stage.PulseDivider);//400000
													//Pmc4BWrap.pmc4bpci_set_range(mPmcAddress, (ushort)pmcAxis, R);
													// 2017.06.22
			MC8000P.Nmc_Range(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper())
				, (int)(mUnitConversionFactor[vArrayNo] / mRatio[vArrayNo]));    // 배율 계산식 참조


			// 리미트센서 액티브 레벨 설정
			SetLimitLevel(pNo, pAXIS);


			MC8000P.Nmc_Jerk(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), NovaMmcAccel.aac);  // 가속도 증가율
			MC8000P.Nmc_DJerk(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), NovaMmcAccel.ddc); // 감속도 증가율

			MC8000P.Nmc_Acc(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), NovaMmcAccel.ac);    // 가속도
			MC8000P.Nmc_Dec(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), NovaMmcAccel.dc);    // 감속도


			//MC8000P.Nmc_StartSpd(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), 100);   // 구동 속도
			MC8000P.Nmc_Speed(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), mSpeed[vArrayNo]);     // 이동 속도
		}
		/// <summary>
		/// 축 이동
		/// </summary>
		/// <param name="pNo">보드 번호</param>
		/// <param name="pAXIS">축 번호</param>
		/// <param name="pDistance">이동거리(0보다 크면 +방향, 0보다 작으면 -방향</param>
		public static void Move(int pNo, McAxis pAXIS, int pDistance)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());
			int vDistance = pDistance; // * mRatio[vArrayNo];

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			// 이동하기 전에 M/C 보드 환경설정
			MoveInit(pNo, pAXIS);

			//내부 최소단위로 반올림
			// Full/Half Step 적용
			vDistance = (int)Math.Round( (double)vDistance * UnitToInternal(mRatio[vArrayNo], mUnitToPolar[vArrayNo]) );
		

			if (pDistance > 0)
			{
				// 이동할 거리 설정 == Pulse (양의 정수)
				MC8000P.Nmc_Pulse(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), vDistance);
				// 명령:33 X 축 '+'방향 정량 드라이브
				MC8000P.Nmc_Command(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), CMD.CMD_F_DRV_P);
			}
			else
			{
				// 이동할 거리 설정 == Pulse (양의 정수)
				MC8000P.Nmc_Pulse(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), - vDistance);
				// 명령:34 X 축 '-'방향 정량 드라이브
				MC8000P.Nmc_Command(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), CMD.CMD_F_DRV_M);
			}
			while (!mIsStop[0])
			{
				mIsStop[0] = IsStop(pNo, pAXIS);
				Application.DoEvents();
			}
		}

		/// <summary>
		/// 모터를 정지
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool Stop(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			MC8000P.Nmc_Command(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), CMD.CMD_STOP_SUDDEN);
			while (!mIsStop[vArrayNo])
			{
				mIsStop[vArrayNo] = IsStop(pNo, pAXIS);
				Application.DoEvents();
			}
			return true;
		}

		/// <summary>
		/// 모터가 정지되었는지 확인
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool IsStop(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			mIsStop[vArrayNo] = !Convert.ToBoolean(MC8000P.Nmc_GetDriveStatus(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper())));
			return mIsStop[vArrayNo];
		}

		/// <summary>
		/// 영점 정렬
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool Zero(int pNo, McAxis pAXIS)
		{
			int vPostion = 0;

			try
			{
				int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

				int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
				NmcAxis vAXIS = getAxis(pAXIS);

				// 이동하기 전에 M/C 보드 환경설정
				MoveInit(pNo, pAXIS);

				// 이동할 거리 설정 == Pulse (양의 정수)
				MC8000P.Nmc_Pulse(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), (int)10000000 * 20 * -1);
				// 명령:34 X 축 '-'방향 정량 드라이브
				MC8000P.Nmc_Command(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), CMD.CMD_F_DRV_M);

				mIsStop[vArrayNo] = false;
				while (!mIsStop[vArrayNo])
				{
					mIsStop[vArrayNo] = IsStop(pNo, pAXIS);
					Application.DoEvents();
				}
				SetPosition(pNo, pAXIS, vPostion);
			}
			catch
			{
				return false;
			}

			return true;
		}


		/// <summary>
		/// +방향 최대 리미트로 이동
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool Max(int pNo, McAxis pAXIS)
		{
			int vPostion = 0;

			try
			{
				int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

				int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
				NmcAxis vAXIS = getAxis(pAXIS);

				// 이동하기 전에 M/C 보드 환경설정
				MoveInit(pNo, pAXIS);

				// 이동할 거리 설정 == Pulse (양의 정수)
				MC8000P.Nmc_Pulse(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), (int)10000000 * 20);
				// 명령:34 X 축 '-'방향 정량 드라이브
				MC8000P.Nmc_Command(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), CMD.CMD_F_DRV_P);

				mIsStop[vArrayNo] = false;
				while (!mIsStop[vArrayNo])
				{
					mIsStop[vArrayNo] = IsStop(pNo, pAXIS);
					Application.DoEvents();
				}
				

				if (IsLimitCCW(pNo, pAXIS)) SetPosition(pNo, pAXIS, vPostion);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 중앙 정렬
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool Home(int pNo, McAxis pAXIS)
		{
			double vMaxStep, vMinStep, vCenterStep, vHomeStep = 0;
			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			// 1. 영점으로 이동
			Zero(pNo, pAXIS);
			vMinStep = GetPosition(pNo, pAXIS); // 1-1. 최소점의 위치값 획득
			System.Threading.Thread.Sleep(1000);
			// 2. 최대점으로 이동
			Max(pNo, pAXIS);
			vMaxStep = GetPosition(pNo, pAXIS); // 2-1. 최대점의 위치값 획득
			System.Threading.Thread.Sleep(1000);
			// 3. Move Orign
			vCenterStep = (vMaxStep / 2); // 홈 스텝 계산
			// 4. Move Orign
			Move(pNo, pAXIS, -(int)vCenterStep);
			vHomeStep = GetPosition(pNo, pAXIS); // 4-1. 홈의 위치값 획득
			System.Threading.Thread.Sleep(1000);
			return true;
		}


		/// <summary>
		/// 메소트를 한번 호출하여 자동으로 스테이지의 실제 거리를 측정
		/// </summary>
		/// <param name="pNo"></param>
		/// <param name="pAXIS"></param>
		/// <returns></returns>
		public static bool Distance(int pNo, McAxis pAXIS)
		{
			double vMaxStep, vMinStep, vCenterStep, vHomeStep = 0;
			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			// 1. 영점으로 이동
			Zero(pNo, pAXIS);
			vMinStep = GetPosition(pNo, pAXIS); // 1-1. 최소점의 위치값 획득
			System.Threading.Thread.Sleep(1000);
			// 2. 최대점으로 이동
			Max(pNo, pAXIS);
			vMaxStep = GetPosition(pNo, pAXIS); // 2-1. 최대점의 위치값 획득
			System.Threading.Thread.Sleep(1000);
			// 3. Move Orign
			//vCenterStep = (int)(vMaxStep / 2 / 20); // 홈 스텝 계산
													// 4. Move Orign
			//Move(pNo, pAXIS, -vCenterStep);
			//vHomeStep = GetPosition(pNo, pAXIS); // 4-1. 홈의 위치값 획득
			//System.Threading.Thread.Sleep(1000);
			return true;
		}

		/// <summary>
		/// 현재 논리적 위치 보드에 설정
		/// </summary>
		/// <param name="pNo">보드번호</param>
		/// <param name="pAXIS">축번호</param>
		/// <param name="pPosition">설정할 위치 값, 실수 사용가능</param>
		/// <returns>설정된 위치 값</returns>
		public static int SetPosition(int pNo, McAxis pAXIS, Double pPosition)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());
			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());

			NmcAxis vAXIS = getAxis(pAXIS);

			int vPosition = (int)(pPosition * UnitToInternal(mRatio[vArrayNo], mUnitToPolar[vArrayNo]));

			MC8000P.Nmc_Lp(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()), vPosition);

			return vPosition;
		}

		/// <summary>
		/// 현재 논리적 위치 보드에서 가져옴
		/// </summary>
		/// <param name="pNo">보드번호</param>
		/// <param name="pAXIS">가져올 위치의 축번호</param>
		/// <returns>현재 위치 값</returns>
		public static double GetPosition(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			double vPosition = MC8000P.Nmc_ReadLp(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper()));

			vPosition = vPosition * (1 / UnitToInternal(mRatio[vArrayNo], mUnitToPolar[vArrayNo]));

			return vPosition;
		}

		/// <summary>
		/// 축의 CW 정방향 리미트 여부 가져옴
		/// </summary>
		/// <param name="pNo">보드번호</param>
		/// <param name="pAXIS">축번호</param>
		/// <returns>CW 정방향 리미트 여부, 리미트 이면 true </returns>
		public static bool IsLimitCW(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			// 리미트 액티브 레벨 설정
			SetLimitLevel(pNo, pAXIS);

			bool vIsLimitCW = false;
			try
			{
				vIsLimitCW = (MC8000P.Nmc_ReadReg2(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper())) & 0x0004) >= 1;
			}
			catch
			{
				vIsLimitCW = false;
			}

			return vIsLimitCW;
		}

		/// <summary>
		/// 축의 CCW 역방향 리미트 여부 가져옴
		/// </summary>
		/// <param name="pNo">보드번호</param>
		/// <param name="pAXIS">축번호</param>
		/// <returns>CCW 역방향 리미트 여부, 리미트 이면 true </returns>
		public static bool IsLimitCCW(int pNo, McAxis pAXIS)
		{
			int vArrayNo = (int)Enum.Parse(typeof(AxisArrayNo), pAXIS.ToString().ToUpper());

			int icNo = (int)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), pAXIS.ToString());
			NmcAxis vAXIS = getAxis(pAXIS);

			// 리미트 액티브 레벨 설정
			SetLimitLevel(pNo, pAXIS);

			bool vIsLimitCCW = false;
			try
			{
				vIsLimitCCW = (MC8000P.Nmc_ReadReg2(pNo, icNo, (AXIS)Enum.Parse(typeof(AXIS), vAXIS.ToString().ToUpper())) & 0x0008) >= 1;
			}
			catch
			{
				vIsLimitCCW = false;
			}

			return vIsLimitCCW;
		}

		
		/// <summary>
		/// Pulse를 배율을 적용하여 실제 거리로 환산 (μM)
		/// </summary>
		/// <param name="pDistance"></param>
		/// <param name="pUnitToPolar"></param>
		/// <returns></returns>
		// Full/Half Step
		// utility: display unit ~ MC internal unit
		public static double UnitToInternal(double pRatio, double pUnitToPolar)
		{
			return (pRatio / pUnitToPolar);
		}


		//-------------------------------------------------------------------------------------

		public static void XMLConfigCreate(string pFilePath)
		{
			// 문서를 만들고 지정된 값의 노드를 만든다..
			XmlDocument vNewXmlDoc = new XmlDocument();
			vNewXmlDoc.AppendChild(vNewXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

			// 최상위 노드를 만든다.
			XmlNode root = vNewXmlDoc.CreateElement("", "Root", "");
			vNewXmlDoc.AppendChild(root);

			// 지정된 XML문서로 만들고 저장한다.
			vNewXmlDoc.Save("ConfigNova.xml");
		}

		/// <summary>
		/// 자식노드를 생성해서 CreateNode의 도움을 받아 값 넣기
		/// </summary>
		public static void XMLNodeSetting()
		{
			string vConifgFileName = "ConfigNova.xml";

			XmlDocument vXmlDoc = new XmlDocument();

			// 환경설정 파일이 없으면 만든다.
			if (!File.Exists(vConifgFileName))
			{
				vXmlDoc.AppendChild(vXmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));
				// 최상위 노드를 만든다.
				XmlNode root = vXmlDoc.CreateElement("", "Root", "");
				vXmlDoc.AppendChild(root);
				// 지정된 XML문서로 만들고 저장한다.
				vXmlDoc.Save(vConifgFileName);
			}

			// 환경파일을 로딩한다.
			vXmlDoc.Load("ConfigNova.xml");
			XmlNode vFristNode = vXmlDoc.DocumentElement;
			XmlElement vRoot = vXmlDoc.CreateElement("BOOK");

			vRoot.SetAttribute("NUMBER", "1");

			vRoot.AppendChild(XMLCreateNode(vXmlDoc, "NAME", "열공하자!"));
			vRoot.AppendChild(XMLCreateNode(vXmlDoc, "MADE", "ㅁㅁ출판"));

			vFristNode.AppendChild(vRoot);
			vXmlDoc.Save("bookconfig.xml");
		}

		/// <summary>
		/// 자식노드 생성하고 값넣기
		/// </summary>
		/// <param name="pXMLDoc">
		/// <param name="pName">
		/// <param name="pInnerXml">
		/// <returns></returns>
		public static XmlNode XMLCreateNode(XmlDocument pXMLDoc, string pName, string pInnerXml)
		{
			XmlNode vNode = pXMLDoc.CreateElement(string.Empty, pName, string.Empty);
			vNode.InnerXml = pInnerXml;

			return vNode;
		}




	}
}
