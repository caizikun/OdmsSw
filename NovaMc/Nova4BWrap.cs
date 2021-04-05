using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Management;


namespace Free302.TnM.Device
{
	public class Nova4BWrap
	{

		//레지스터 번호
		private const int WR0 = 1;
		private const int WR1 = 2;
		private const int WR2 = 3;
		private const int WR3 = 4;
		private const int WR4 = 5;
		private const int WR5 = 6;
		private const int WR6 = 7;
		private const int WR7 = 8;

		private const int RR0 = 9;
		private const int RR1 = 10;
		private const int RR2 = 11;
		private const int RR3 = 12;
		private const int RR4 = 13;
		private const int RR5 = 14;
		private const int RR6 = 15;
		private const int RR7 = 16;

		public enum AXIS
		{
			NONE = 0,   // 축 지정 없음
			X = 1,      // X 축
			Y = 2,      // Y 축
			Z = 4,      // Z 축
			U = 8,      // U 축
			ALL = 15    // 전체 축
		}

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

		public static class MC8041P
		{

			public static IntPtr Nmc_Open(int No, bool IntrptFlg)
			{
				//보드 오픈
				return Nova8041P.OpenCard_N(No, IntPtr.Zero);
			}

			public static int Nmc_Close(int No)
			{
				//보드 Close
				return (Nova8041P.CloseCard_N(No)) ? 1 : 0;
			}

			public static void Nmc_Command(int No, int IcNo, AXIS axis, CMD cmd)
			{
				//Command
				var ax = (int)axis;
				ax &= 0x0f;
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)cmd);
			}

			public static void Nmc_Range(int No, int IcNo, AXIS axis, int wdata)
			{
				//레인지 (R) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Range);
			}

			public static void Nmc_Jerk(int No, int IcNo, AXIS axis, int wdata)
			{
				//가속도 증가율 (K)설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 65535) wdata = 65535;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Jerk);
			}

			public static void Nmc_Acc(int No, int IcNo, AXIS axis, int wdata)
			{
				//가속도 (A) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 8000) wdata = 8000;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Acc);
			}

			public static void Nmc_Dec(int No, int IcNo, AXIS axis, int wdata)
			{
				//감속도 (D) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 8000) wdata = 8000;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Dec);
			}

			public static void Nmc_StartSpd(int No, int IcNo, AXIS axis, int wdata)
			{
				//초속도 (SV) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 8000) wdata = 8000;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_StartSpd);
			}

			public static void Nmc_Speed(int No, int IcNo, AXIS axis, int wdata)
			{
				//드라이브 속도 (V) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 8000) wdata = 8000;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Speed);
			}

			public static void Nmc_Pulse(int No, int IcNo, AXIS axis, int wdata)
			{
				//출력 펄스 수/종점 (P) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				//if (wdata < -8388608) wdata = -8388608;
				//if (wdata > 268435455) wdata = 268435455;

				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Pulse);
			}

			public static void Nmc_Center(int No, int IcNo, AXIS axis, int wdata)
			{
				//원호 중심점 (C) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata < -8388608) wdata = -8388608;
				if (wdata > 8388608) wdata = 8388608;

				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + 0x08);
			}

			public static void Nmc_Lp(int No, int IcNo, AXIS axis, int wdata)
			{
				//논리 위치 카운터 (LP) 설정
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Lp);
			}

			public static void Nmc_Ep(int No, int IcNo, AXIS axis, int wdata)
			{
				//실위치 카운터 (EP) 설정
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_Ep);
			}

			public static void Nmc_AccOfst(int No, int IcNo, AXIS axis, int wdata)
			{
				//가속 카운터 오프셋 (AO) 설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 65535) wdata = 65535;
				if (wdata < 0) wdata = 0;

				Nova8041P.OutW_N(No, WR7, (wdata >> 16) & 0xffff);
				Nova8041P.OutW_N(No, WR6, wdata & 0xffff);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_AccOfst);
			}

			public static void Nmc_DJerk(int No, int IcNo, AXIS axis, int wdata)
			{
				//감속도 증가율 (L)설정
				var ax = (int)axis;
				ax &= 0x0f;
				if (wdata > 65535) wdata = 65535;
				if (wdata < 1) wdata = 1;

				Nova8041P.OutW_N(No, WR6, wdata);
				Nova8041P.OutW_N(No, WR0, (ax << 8) + 0x0e);
			}


			public static int Nmc_ReadLp(int No, int IcNo, AXIS axis)
			{
				//논리위치 카운터 값 (LP) 읽기
				Int32 a, d6, d7;
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_ReadLp);
				d6 = Nova8041P.InW_N(No, RR6);
				d7 = Nova8041P.InW_N(No, RR7);
				a = d6 + (d7 << 16);
				return a;
			}


			public static int Nmc_GetDriveStatus(int No, int IcNo, AXIS axis)
			{
				//지정한 모든 축의 드라이브가 종료하는 경우는 0을 돌려 준다.
				//지정한 축 중 하나 이상이 드라이브 중의 경우는 0 이외를 돌려 준다
				var ax = (int)axis;
				ax &= 0x0f;

				return (Nova8041P.InW_N(No, RR0) & ax);
			}


			public static void Nmc_WriteReg1(int No, int IcNo, AXIS axis, int wdata)
			{
				//라이트 레지스터 1 설정
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_NOP);
				Nova8041P.OutW_N(No, WR1, wdata);
			}

			public static void Nmc_WriteReg2(int No, int IcNo, AXIS axis, int wdata)
			{
				//라이트 레지스터 2 설정
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_NOP);
				Nova8041P.OutW_N(No, WR2, wdata);
			}

			public static void Nmc_WriteReg3(int No, int IcNo, AXIS axis, int wdata)
			{
				//라이트 레지스터 3 설정
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_NOP);
				Nova8041P.OutW_N(No, WR3, wdata);
			}

			public static void Nmc_WriteReg4(int No, int IcNo, int wdata)
			{
				//라이트 레지스터 4 설정
				Nova8041P.OutW_N(No, WR4, wdata);
			}

			public static void Nmc_WriteReg5(int No, int IcNo, int wdata)
			{
				//라이트 레지스터 5 설정
				Nova8041P.OutW_N(No, WR5, wdata);
			}

			public static long Nmc_ReadReg0(int No, int IcNo)
			{
				//리드 레지스터 0 읽기
				return Nova8041P.InW_N(No, RR0);
			}

			public static long Nmc_ReadReg2(int No, int IcNo, AXIS axis)
			{
				//리드 레지스터 2 읽기
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_NOP);
				return Nova8041P.InW_N(No, RR2);
			}

			public static long Nmc_ReadReg3(int No, int IcNo, AXIS axis)
			{
				//리드 레지스터 3 읽기
				var ax = (int)axis;
				ax &= 0x0f;

				Nova8041P.OutW_N(No, WR0, (ax << 8) + (int)CMD.CMD_NOP);
				return Nova8041P.InW_N(No, RR3);
			}

			public static long Nmc_ReadReg4(int No, int IcNo)
			{
				//리드 레지스터 4 읽기
				return Nova8041P.InW_N(No, RR4);
			}

			public static long Nmc_ReadReg5(int No, int IcNo)
			{
				//리드 레지스터 5 읽기
				return Nova8041P.InW_N(No, RR5);
			}

			public static void Nmc_Reset(int No, int IcNo)
			{
				//소프트 리셋
				Nova8041P.OutW_N(No, WR0, 0x8000);
			}

		}


		public static class Nova8041P
		{
			private const string DllFileName = "MC8041P.DLL";
			const CallingConvention DllCalling = CallingConvention.StdCall;

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern IntPtr OpenCard(IntPtr o);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern IntPtr OpenCard_N(int No, IntPtr o);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern void OutW(ushort iocc, int data);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern void OutW_N(int No, ushort iocc, int data);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern UInt16 InW(UInt16 iocc);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern UInt16 InW_N(int No, UInt16 iocc);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern bool CloseCard(object o);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern bool CloseCard_N(int No);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern bool CloseCard_all(object o);


			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern void ReadRR3(ref ushort Rr3X, ref ushort Rr3Y, ref ushort Rr3Z, ref ushort Rr3U);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern void ReadRR3_N(int No, ref ushort Rr3X, ref ushort Rr3Y, ref ushort Rr3Z, ref ushort Rr3U);


			#region == 신드라이버용 정의 ==

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool OpenMC8041P(int No);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			[return: MarshalAs(UnmanagedType.I2)]
			public static extern long OpenMC8041P(int No, long Adr);


			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern bool CloseMC8041P(int No);


			[DllImport(DllFileName)]
			public static extern void CloseAllMC8041P();


			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern void OutpMC8041P(int No, long Adr, long Dat);

			[DllImport(DllFileName, CharSet = CharSet.Ansi, CallingConvention = DllCalling)]
			public static extern long InpMC8041P(int No, long Adr); 

			#endregion


		}
	}
}
