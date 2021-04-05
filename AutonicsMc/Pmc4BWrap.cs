using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace Free302.TnM.Device
{
    public static class Pmc4BWrap
    {
        const string ImportingDllName = "Free302.TnM.Device.Pmc4B.dll";

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

        #region === Axes ===

        public const UInt16 PMC4BPCI_AXIS1 = 0x0100;
        public const UInt16 PMC4BPCI_AXIS2 = 0x0200;
        public const UInt16 PMC4BPCI_AXIS3 = 0x0400;
        public const UInt16 PMC4BPCI_AXIS4 = 0x0800;
        public const UInt16 PMC4BPCI_AXIS_X = PMC4BPCI_AXIS1;
        public const UInt16 PMC4BPCI_AXIS_Y = PMC4BPCI_AXIS1;
        public const UInt16 PMC4BPCI_AXIS_Z = PMC4BPCI_AXIS1;
        public const UInt16 PMC4BPCI_AXIS_U = PMC4BPCI_AXIS1;

        #endregion

        //register address
        public const UInt16 WR0 = 0x00;
        public const UInt16 wr0 = 0x00;
        public const UInt16 WR1 = 0x02;
        public const UInt16 wr1 = 0x02;
        public const UInt16 WR2 = 0x04;
        public const UInt16 wr2 = 0x04;
        public const UInt16 WR3 = 0x06;
        public const UInt16 wr3 = 0x06;
        public const UInt16 WR4 = 0x08;
        public const UInt16 wr4 = 0x08;
        public const UInt16 WR5 = 0x0a;
        public const UInt16 wr5 = 0x0a;
        public const UInt16 WR6 = 0x0c;
        public const UInt16 wr6 = 0x0c;
        public const UInt16 WR7 = 0x0e;
        public const UInt16 wr7 = 0x0e;
                                 
        public const UInt16 RR0 = 0x00;
        public const UInt16 rr0 = 0x00;
        public const UInt16 RR1 = 0x02;
        public const UInt16 rr1 = 0x02;
        public const UInt16 RR2 = 0x04;
        public const UInt16 rr2 = 0x04;
        public const UInt16 RR3 = 0x06;
        public const UInt16 rr3 = 0x06;
        public const UInt16 RR4 = 0x08;
        public const UInt16 rr4 = 0x08;
        public const UInt16 RR5 = 0x0a;
        public const UInt16 rr5 = 0x0a;
        public const UInt16 RR6 = 0x0c;
        public const UInt16 rr6 = 0x0c;
        public const UInt16 RR7 = 0x0e;
        public const UInt16 rr7 = 0x0e;

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
        public const UInt16 WR1_INT_PULSE   = 0x0100;// drive pulse의 rising edge(펄스에서 올라가는 순간)에서 인터럽트 발생(drive pulse정논리 설정시)
        public const UInt16 WR1_INT_PGECM   = 0x0200;// 논리/실제위치 pulse값 >= COMP-register값 일때
        public const UInt16 WR1_INT_PLCM    = 0x0400;// 논리/실제위치 pulse값 < COMP-register값 일때
        public const UInt16 WR1_INT_PLCP    = 0x0800;// 논리/실제위치 pulse값 < COMP+register값 일때
        public const UInt16 WR1_INT_PGEP    = 0x1000;// 논리/실제위치 pulse값 >= COMP+register값 일때
        public const UInt16 WR1_INT_CEND    = 0x2000;// 정속지역에서 pulse출력을 종료할때(가감속 drive일때)
        public const UInt16 WR1_INT_CSTA    = 0x4000;// 정속지역에서 pulse출력을 개시하였을때(가감속 drive일때)
        public const UInt16 WR1_INT_DEND    = 0x8000;// drive가 종료하였을때

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


        public delegate void McIntHandler();
        [DllImport(ImportingDllName)] 
        extern public static UInt16 pmc4bpci_open(int id, McIntHandler handler);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_reset(UInt16 id, UInt16 axis);//axis가 왜 필요한지..??
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_close(UInt16 id);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_close_all();

        
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_pls_move(UInt16 id, UInt16 axis, Int32 pls, Int16 vel, Int16 acc);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_pls_move_wait(UInt16 id, UInt16 axis, Int32 pls, Int16 vel, Int16 acc);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_pls_smove(UInt16 id, UInt16 axis, Int32 pls,
            Int16 vel, Int16 acc, Int16 acac, Int16 dec, Int16 dcac);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_pls_smove_wait(UInt16 id, UInt16 axis, Int32 pls,
            Int16 vel, Int16 acc, Int16 acac, Int16 dec, Int16 dcac);

        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_stop(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_dstop(UInt16 id, UInt16 axis);

        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_set_axis(UInt16 id, UInt16 axis);

        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_outw(UInt16 id, UInt16 wOffset, UInt16 wData);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_inw(UInt16 id, UInt16 wOffset);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_wwr1(UInt16 id, UInt16 axis, UInt16 wdata);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_wwr2(UInt16 id, UInt16 axis, UInt16 wdata);

        [DllImport(ImportingDllName)]
        extern public static Int32 pmc4bpci_get_logicalposition(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]
		extern public static UInt16 pmc4bpci_set_lpcounter(UInt16 id, UInt16 axis, Int32 wdata);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_is_error(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_is_stop(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]        
        extern public static UInt16 pmc4bpci_is_drive(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]        
        extern public static UInt16 pmc4bpci_flag_in0(UInt16 id);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_flag_in1(UInt16 id);
		[DllImport(ImportingDllName)]
		extern public static UInt16 pmc4bpci_flag_limitplus(UInt16 id);
		[DllImport(ImportingDllName)]
		extern public static UInt16 pmc4bpci_flag_limitminus(UInt16 id);

        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_flag_error(UInt16 id, UInt16 axis);

        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_error();
        
        [DllImport(ImportingDllName, CharSet = CharSet.Ansi)]
        extern public static IntPtr ValidString();


        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_wait(UInt16 id, UInt16 axis);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_wait_timeout(UInt16 id, UInt16 axis, uint msec);





        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_set_range(UInt16 id, UInt16 axis, Int32 wdata);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_driver_id(byte[] str);
        [DllImport(ImportingDllName)]
        extern public static UInt16 pmc4bpci_check_valid_id(UInt16 id);

    }
}
