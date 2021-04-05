
/*
///////////////////////////////////////////////////////////////////////////////
//
//	File Name	:	pmc4bpci.h
//
//                  www.autonics.co.kr
//
//  Version     :   v2.0
//  date        :   2012/3/22
//
///////////////////////////////////////////////////////////////////////////////
*/

#if !defined(__PMC4BPCI_H__)
#define __PMC4BPCI_H__

#define __PMC4BPCI__

#ifdef __cplusplus
extern "C" {
#endif


#define	AXIS_NUM	4
#define MAX_CARD	16


#define WR0			0x00
#define wr0			0x00
#define WR1			0x02
#define wr1			0x02
#define WR2			0x04
#define wr2			0x04
#define WR3			0x06
#define wr3			0x06
#define WR4			0x08
#define wr4			0x08
#define WR5			0x0a
#define wr5			0x0a
#define WR6			0x0c
#define wr6			0x0c
#define WR7			0x0e
#define wr7			0x0e

#define RR0			0x00
#define rr0			0x00
#define RR1			0x02
#define rr1			0x02
#define RR2			0x04
#define rr2			0x04
#define RR3			0x06
#define rr3			0x06
#define RR4			0x08
#define rr4			0x08
#define RR5			0x0a
#define rr5			0x0a
#define RR6			0x0c
#define rr6			0x0c
#define RR7			0x0e
#define rr7			0x0e

#define	bp1p		0x04
#define	bp1m		0x06
#define	bp2p 		0x08
#define	bp2m		0x0a
#define	bp3p		0x0c
#define	bp3m		0x0e


#ifdef __PMC4BPCI__
	#define RID			0x10	// card id address (for PMC-4B-PCI)
#endif

#ifdef __PMC2ISA__
	#define RDD			0x10	// user data address (for PMC-2ISA)
#endif


//=============================================================================
// EX-FUNCTION
//=============================================================================

#ifndef MMC_VOID
	typedef void MMC_VOID;
#endif

#ifndef MMC_INT16
	typedef signed short int MMC_INT16;
#endif

#ifndef MMC_INT32
	typedef signed long MMC_INT32;
#endif

#ifndef MMC_INT8
	typedef signed char MMC_INT8;
#endif

#ifndef MMC_INT16U
	typedef unsigned short int MMC_INT16U;
#endif
	
#ifndef MMC_INT32U
	typedef unsigned long MMC_INT32U;
#endif

#ifndef MMC_INT8U
	typedef unsigned char MMC_INT8U;
#endif

#ifndef MMC_FLOAT
	typedef float MMC_FLOAT;
#endif

#ifndef MMC_DOUBLE
	typedef double MMC_DOUBLE;
#endif

#ifndef MMC_LONGDOUBLE
	typedef long double MMC_LONGDOUBLE;
#endif

#ifndef MMC_VERTEX
	typedef struct {
		double x;
		double y;
		union {
			double z;
			double k;
		};
	} MMC_VERTEX;
#endif

// common
#define MMC_FALSE				0
#define MMC_TRUE				1
#define MMC_OK					MMC_TRUE	//2
#define MMC_HIGH_LEVEL			1
#define MMC_LOW_LEVEL			0
#define MMC_OPEN_ERR			5
#define MMC_IOADDRESS_ERR		6
#define MMC_TIMEOUT_ERR			7
#define MMC_INVALID_AXIS		8
#define MMC_ILLEGAL_PARAMETER	9
#define MMC_ZERO_PARAMETER		10
#define MMC_ERROR				11
#define MMC_QUIT				12
#define MMC_INVALID_CARD		13


#define AXIS_X					0x01
#define AXIS_Y					0x02
#define	AXIS_Z					0x04
#define AXIS_U					0x08

// WR1 (Mode register 1)
// INn_SLOWSTOP �Է½�ȣ IN0������ ������(0-low��������,1-high������������)
// INn_ENABL �Է½�ȣ IN0������ ��ȿ/��ȿ ����(0-��ȿ,1-��ȿ)
#define IN0_SLOWSTOP			WR1_IN0_L
#define IN0_ENABLE				WR1_IN0_E
#define IN1_SLOWSTOP			WR1_IN1_L
#define IN1_ENABLE				WR1_IN1_E
#define IN2_SLOWSTOP			WR1_IN2_L
#define IN2_ENABLE				WR1_IN2_E
#define IN3_SLOWSTOP			WR1_IN3_L
#define IN3_ENABLE				WR1_IN3_E
// interrupt�� �߻����� ����
#define INT_PULSE				WR1_INT_PULSE	// drive pulse�� rising edge(�޽����� �ö󰡴� ����)���� ���ͷ�Ʈ �߻�(drive pulse���� ������)
#define INT_PGECM				WR1_INT_PGECM	// ��/������ġ pulse�� >= COMP-register�� �϶�
#define INT_PLCM				WR1_INT_PLCM	// ��/������ġ pulse�� < COMP-register�� �϶�
#define INT_PLCP				WR1_INT_PLCP	// ��/������ġ pulse�� < COMP+register�� �϶�
#define INT_PGEP				WR1_INT_PGEP	// ��/������ġ pulse�� >= COMP+register�� �϶�
#define INT_CEND				WR1_INT_CEND	// ������������ pulse����� �����Ҷ�(������ drive�϶�)
#define INT_CSTA				WR1_INT_CSTA	// ������������ pulse����� �����Ͽ�����(������ drive�϶�)
#define INT_DEND				WR1_INT_DEND	// drive�� �����Ͽ�����

// WR2 (Mode register 2)
#define LMT_SLMTP				WR2_LMT_SLMTP	// COMP+register�� software limit�� ����
#define LMT_SLMTM				WR2_LMT_SLMTM	// COMP-register�� software limit�� ����
#define LMT_LMTMD				WR2_LMT_LMTMD	// hardware limit(nLMTP,nLMTM)�� active�� ��� ������� ����(0:�������,1:��������)
#define LMT_HLMTP				WR2_LMT_HLMTP	// +���� limit�Է½�ȣ(nLMTP)�� �� level����(0:low,1:high���� active)
#define LMT_HLMTM				WR2_LMT_HLMTM	// -���� limit�Է½�ȣ(nLMTM)�� �� level����(0:low,1:high���� active)
#define LMT_CMPSL				WR2_LMT_CMPSL	// COMP+/- register�� �񱳴��(0:����ġ,1:������ġ�� ��)
#define DRV_PLSMD				WR2_DRV_PLSMD	// drive pulse�� ��¹��(0:����2 pulse���,1:1 pulse���)
#define DRV_PLS_L				WR2_DRV_PLS_L	// drive pulse�� �� level����(0:����,1:�γ�)
#define DRV_DIR_L				WR2_DRV_DIR_L	// drive pulse�� ������½�ȣ�� �� level ����(0:+�����϶�low,-�����϶�hi,1:+�����϶�hi,-�����϶�low)
#define ENC_PINMD				WR2_ENC_PINMD	// encoder�Է½�ȣ(nECA/PPIN,nECB/PMIN)�� ��¹��(0:2��pulse,1:up/dn pulse�Է�)
#define ENC_PIND0				WR2_ENC_PIND0	// encoder 2�� pulse�Է��� ���ֺ� ����(D1D0 00=1/1, 01=1/2
#define ENC_PIND1				WR2_ENC_PIND1	// encoder 2�� pulse�Է��� ���ֺ� ����      10=1/4, 11=��ȿ)
#define SRV_ALM_L				WR2_SRV_ALM_L	// nALARM�Է� ��ȣ�� �� level�� ����(0:low, 1:high���� active)
#define SRV_ALM_E				WR2_SRV_ALM_E	// servo motor alarm�� �Է½�ȣ nALARM�� ��/��ȿ ����(0:��ȿ,1:��ȿ)
#define SRV_INP_L				WR2_SRV_INP_L	// nINPOS�Է� ��ȣ�� �� level�� ����(0:low, 1:high���� active)
#define SRV_INP_E				WR2_SRV_INP_E	// servo motor ��ġ ���� �Ϸ�� �Է½�ȣ nINPOS�� ��ȿ/��ȿ ����(0:��ȿ,1:��ȿ)

// WR3 (Mode register 3)
#define DRV_MANLD				WR3_DRV_MANLD	// ������ ���� drive�� ���ӹ��(0:�ڵ�����,1:manual����)
#define DRV_DSNSE				WR3_DRV_DSNSE	// ������ drive���ӽ� ������ �޴� ����(0:���ӵ��� ���� ����,1:���ӵ��� ���� ����-manual����϶��� ���-)
#define DRV_SACC				WR3_DRV_SACC	// ����������/S�� ������ ����(0:����������, 1:S�ڰ�����)
#define DRV_EXOP0				WR3_DRV_EXOP0	// �ܺ��Է½�ȣ (D4D3 00=�ܺ��Է½�ȣ������drive���۹�ȿ, 01=����drive mode
#define DRV_EXOP1				WR3_DRV_EXOP1	// �ܺ��Է½�ȣ       10=����drive mode, 11=�ܺ��Է½�ȣ������drive���۹�ȿ
#define OUT_OUTSL				WR3_OUT_OUTSL	// ��½�ȣ nOUT4~7�� ����/drive������ ���ΰ���(0:�������,1:drive����ǥ��)
#define OUT_OUT4				WR3_OUT_OUT4	// ��½�ȣ�� ����Ҷ� ���(0:low level���,1:high level���)
#define OUT_OUT5				WR3_OUT_OUT5	//
#define OUT_OUT6				WR3_OUT_OUT6	//
#define OUT_OUT7				WR3_OUT_OUT7	//

// WR5 (interpolation)
#define INP_EXPLS				WR5_INP_EXPLS	// �ܺ����ۿ���(0:���۾���,1:����drive�� �ܺν�ȣ-EXPLSN-�� step����)
#define	INP_CMPLS				WR5_INP_CMPLS	// (0:���۾���,1:����drive�� command�� step����)
#define INP_CIINT				WR5_INP_CIINT	// ���Ӻ������� interrupt�߻� �㰡/���� ����(0:����,1:�㰡)
#define INP_BPINT				WR5_INP_BPINT	// bit pattern������ interrupt�߻� �㰡/���� ����(0:����,1:�㰡)

#define INP_AXIS1_X				WR5_INP_AXIS1_X				// ������ ��1�࿡ �����ϰ��� �ϴ��� ����
#define INP_AXIS1_Y				WR5_INP_AXIS1_Y				//
#define INP_AXIS1_Z				WR5_INP_AXIS1_Z				//
#define INP_AXIS1_U				WR5_INP_AXIS1_U				//
#define INP_AXIS2_X				WR5_INP_AXIS2_X				// ������ ��2�࿡ �����ϰ��� �ϴ��� ����
#define INP_AXIS2_Y				WR5_INP_AXIS2_Y				//
#define INP_AXIS2_Z				WR5_INP_AXIS2_Z				//
#define INP_AXIS2_U				WR5_INP_AXIS2_U				//
#define INP_AXIS3_X				WR5_INP_AXIS3_X				// ������ ��3�࿡ �����ϰ��� �ϴ��� ����
#define INP_AXIS3_Y				WR5_INP_AXIS3_Y				//
#define INP_AXIS3_Z				WR5_INP_AXIS3_Z				//
#define INP_AXIS3_U				WR5_INP_AXIS3_U				//
#define INP_LCMODE_INVALIDATE	WR5_INP_LCMODE_INVALIDATE	// ����drive�� ��ȿ mode
#define INP_LCMODE_2AXIS		WR5_INP_LCMODE_2AXIS		// 2�� ���� ����
#define INP_LCMODE_3AXIS		WR5_INP_LCMODE_3AXIS		// 3�� ���� ����


#define PMC4BPCI_AXIS1	0x0100
#define PMC4BPCI_AXIS2	0x0200
#define	PMC4BPCI_AXIS3	0x0400
#define PMC4BPCI_AXIS4	0x0800
#define PMC4BPCI_AXIS_X	PMC4BPCI_AXIS1
#define PMC4BPCI_AXIS_Y	PMC4BPCI_AXIS2
#define	PMC4BPCI_AXIS_Z	PMC4BPCI_AXIS3
#define PMC4BPCI_AXIS_U	PMC4BPCI_AXIS4

#define PMC4BPCI_RANGE	8000000

// command code
#define PMC4BPCI_CMD_R	0x00	// Range ����(8,000,000:����=1 ~ 16,000:����:500),4
#define PMC4BPCI_CMD_K	0x01	// �����ӵ� ���� (1~65535),2
#define PMC4BPCI_CMD_A	0x02	// ���ӵ� ����(1~8000),2
#define PMC4BPCI_CMD_D	0x03	// ���ӵ� ����(1~8000),2
#define PMC4BPCI_CMD_SV	0x04	// ó���ӵ� ����(1~8000),2
#define PMC4BPCI_CMD_V	0x05	// drive �ӵ� ����(1~8000),2
#define PMC4BPCI_CMD_P	0x06	// ��� pulse��(0~268,435,455),4
								// ��������(-8,388,608~8,388,607),4
#define PMC4BPCI_CMD_DP	0x07	// manual ������ ����(0~2g),4
#define PMC4BPCI_CMD_C	0x08	// ��ȣ�߽��� ����(-8m~8m),4
#define PMC4BPCI_CMD_LP	0x09	// ����ġ counter����(-2g~2g),4
#define PMC4BPCI_CMD_EP	0x0a	// ����ġ counter����(-2g~2g),4
#define PMC4BPCI_CMD_CP	0x0b	// COMP +register����(-2g~2g),4
#define PMC4BPCI_CMD_CM	0x0c	// COMP -register����(-2g~2g),4
#define PMC4BPCI_CMD_AO	0x0d	// ���� counter offset����(0~65535),2
#define PMC4BPCI_CMD_AD	0x0e	// �����ӵ� ����(1~8000),2
#define PMC4BPCI_CMD_NOP	0x0f	// NOP(�౳ȯ��)
#define PMC4BPCI_CMD_HS_SET	0x60	// Home search(set)
#define PMC4BPCI_CMD_HS_V	0x61	// Home search(speed)
#define PMC4BPCI_CMD_HS_RUN	0x62	// Home search(run)
#define PMC4BPCI_CMD_RST	0x8000	// Reset

// data �о�� ���
#define PMC4BPCI_READ_LP	0x10	// ����ġ counter�о�� (-2g~2g),4
#define PMC4BPCI_READ_EP	0x11	// ������ġ counter�о�� (-2g~2g),4
#define PMC4BPCI_READ_CV	0x12	// ���� drive�ӵ� �о��(1~8000),2
#define PMC4BPCI_READ_CA	0x13	// ���� ���мӵ� �о��(1~8000),2

// drive ���
#define PMC4BPCI_DRV_PF	0x20	// +���� ���� drive
#define PMC4BPCI_DRV_MF	0x21	// -���� ���� drive
#define PMC4BPCI_DRV_PC	0x22	// +���� ���� drive
#define PMC4BPCI_DRV_MC	0x23	// -���� ���� drive
#define PMC4BPCI_DRV_DH	0x24	// drive���� hold
#define PMC4BPCI_DRV_DF	0x25	// drive���� free/�Ϸ� status clear
#define PMC4BPCI_DRV_DDS	0x26	// drive���� ����
#define PMC4BPCI_DRV_DS	0x27	// drive��� ����

// ���� ���
#define PMC4BPCI_INP_2LD	0x30	// 2�� �������� drive
#define PMC4BPCI_INP_3LD	0x31	// 3�� �������� drive
#define PMC4BPCI_INP_CW	0x32	// CW��ȣ ���� drive
#define PMC4BPCI_INP_CCW	0x33	// CCW��ȣ ���� drive
#define PMC4BPCI_INP_2BP	0x34	// 2�� bit pattern ���� drive
#define PMC4BPCI_INP_3BP	0x35	// 3�� bit pattern ���� drive
#define PMC4BPCI_INP_BPE	0x36	// BP register ���� ����
#define PMC4BPCI_INP_BPD	0x37	// BP register ���� �Ұ���
#define PMC4BPCI_INP_BPS	0x38	// BP data stack
#define PMC4BPCI_INP_BPC	0x39	// BP data clear
#define PMC4BPCI_INP_SS	0x3a	// ���� single step
#define PMC4BPCI_INP_DV1	0x3b	// ���� ��ȿ
#define PMC4BPCI_INP_DV2	0x3c	// ���� ��ȿ
#define PMC4BPCI_INP_IC	0x3d	// ���� interrupt clear

// WR1 (Mode register 1)
// drive���� �Է½�ȣ
#define WR1_IN0_L		0x0001	// �Է½�ȣ IN0������ ������(0-low��������,1-high������������)
#define WR1_IN0_E		0x0002	// �Է½�ȣ IN0������ ��ȿ/��ȿ ����(0-��ȿ,1-��ȿ)
#define WR1_IN1_L		0x0004
#define WR1_IN1_E		0x0008
#define WR1_IN2_L		0x0010
#define WR1_IN2_E		0x0020
#define WR1_IN3_L		0x0040
#define WR1_IN3_E		0x0080
// interrupt�� �߻����� ����
#define WR1_INT_PULSE	0x0100	// drive pulse�� rising edge(�޽����� �ö󰡴� ����)���� ���ͷ�Ʈ �߻�(drive pulse���� ������)
#define WR1_INT_PGECM	0x0200	// ��/������ġ pulse�� >= COMP-register�� �϶�
#define WR1_INT_PLCM	0x0400	// ��/������ġ pulse�� < COMP-register�� �϶�
#define WR1_INT_PLCP	0x0800	// ��/������ġ pulse�� < COMP+register�� �϶�
#define WR1_INT_PGEP	0x1000	// ��/������ġ pulse�� >= COMP+register�� �϶�
#define WR1_INT_CEND	0x2000	// ������������ pulse����� �����Ҷ�(������ drive�϶�)
#define WR1_INT_CSTA	0x4000	// ������������ pulse����� �����Ͽ�����(������ drive�϶�)
#define WR1_INT_DEND	0x8000	// drive�� �����Ͽ�����

// WR2 (Mode register 2)
#define WR2_LMT_SLMTP	0x0001	// COMP+register�� software limit�� ����
#define WR2_LMT_SLMTM	0x0002	// COMP-register�� software limit�� ����
#define WR2_LMT_LMTMD	0x0004	// hardware limit(nLMTP,nLMTM)�� active�� ��� ������� ����(0:�������,1:��������)
#define WR2_LMT_HLMTP	0x0008	// +���� limit�Է½�ȣ(nLMTP)�� �� level����(0:low,1:high���� active)
#define WR2_LMT_HLMTM	0x0010	// -���� limit�Է½�ȣ(nLMTM)�� �� level����(0:low,1:high���� active)
#define WR2_LMT_CMPSL	0x0020	// COMP+/- register�� �񱳴��(0:����ġ,1:������ġ�� ��)
#define WR2_DRV_PLSMD	0x0040	// drive pulse�� ��¹��(0:����2 pulse���,1:1 pulse���)
#define WR2_DRV_PLS_L	0x0080	// drive pulse�� �� level����(0:����,1:�γ�)
#define WR2_DRV_DIR_L	0x0100	// drive pulse�� ������½�ȣ�� �� level ����(0:+�����϶�low,-�����϶�hi,1:+�����϶�hi,-�����϶�low)
#define WR2_ENC_PINMD	0x0200	// encoder�Է½�ȣ(nECA/PPIN,nECB/PMIN)�� ��¹��(0:2��pulse,1:up/dn pulse�Է�)
#define WR2_ENC_PIND0	0x0400	// encoder 2�� pulse�Է��� ���ֺ� ����(D1D0 00=1/1, 01=1/2
#define WR2_ENC_PIND1	0x0800	// encoder 2�� pulse�Է��� ���ֺ� ����      10=1/4, 11=��ȿ)
#define WR2_SRV_ALM_L	0x1000	// nALARM�Է� ��ȣ�� �� level�� ����(0:low, 1:high���� active)
#define WR2_SRV_ALM_E	0x2000	// servo motor alarm�� �Է½�ȣ nALARM�� ��/��ȿ ����(0:��ȿ,1:��ȿ)
#define WR2_SRV_INP_L	0x4000	// nINPOS�Է� ��ȣ�� �� lovel�� ����(0:low, 1:high���� active)
#define WR2_SRV_INP_E	0x8000	// servo motor ��ġ ���� �Ϸ�� �Է½�ȣ nINPOS�� ��ȿ/��ȿ ����(0:��ȿ,1:��ȿ)


// WR3 (Mode register 3)
#define WR3_DRV_MANLD	0x0001	// ������ ���� drive�� ���ӹ��(0:�ڵ�����,1:manual����)
#define WR3_DRV_DSNSE	0x0002	// ������ drive���ӽ� ������ �޴� ����(0:���ӵ��� ���� ����,1:���ӵ��� ���� ����-manual����϶��� ���-)
#define WR3_DRV_SACC	0x0004	// ����������/S�� ������ ����(0:����������, 1:S�ڰ�����)
#define WR3_DRV_EXOP0	0x0008	// �ܺ��Է½�ȣ (D4D3 00=�ܺ��Է½�ȣ������drive���۹�ȿ, 01=����drive mode
#define WR3_DRV_EXOP1	0x0010	// �ܺ��Է½�ȣ       10=����drive mode, 11=�ܺ��Է½�ȣ������drive���۹�ȿ
#define WR3_OUT_OUTSL	0x0080	// ��½�ȣ nOUT4~7�� ����/drive������ ���ΰ���(0:�������,1:drive����ǥ��)
#define WR3_OUT_OUT4	0x0100	// ��½�ȣ�� ����Ҷ� ���(0:low level���,1:high level���)
#define WR3_OUT_OUT5	0x0200	//
#define WR3_OUT_OUT6	0x0400	//
#define WR3_OUT_OUT7	0x0800	//

// WR4 (output register)
#define WR4_OUT_XOUT0	0x0001	// ������½�ȣnOUT0~3�� ����� ����(0:low level,1:high level)
#define WR4_OUT_XOUT1	0x0002	//
#define WR4_OUT_XOUT2	0x0004	//
#define WR4_OUT_XOUT3	0x0008	//
#define WR4_OUT_YOUT0	0x0010	//
#define	WR4_OUT_YOUT1	0x0020	//
#define WR4_OUT_YOUT2	0x0040	//
#define WR4_OUT_YOUT3	0x0080	//
#define WR4_OUT_ZOUT0	0x0100	//
#define WR4_OUT_ZOUT1	0x0200	//
#define WR4_OUT_ZOUT2	0x0400	//
#define WR4_OUT_ZOUT3	0x0800	//
#define WR4_OUT_UOUT0	0x1000	//
#define WR4_OUT_UOUT1	0x2000	//
#define WR4_OUT_UOUT2	0x4000	//
#define WR4_OUT_UOUT3	0x8000	//

// WR5 (���� mode register)
#define WR5_INP_AX10	0x0001	// ����drive�� �����ϴ� ��1�� ����
#define WR5_INP_AX11	0x0002	// AX10AX11 00:X, 01:Y, 10:Z, 11:U
#define WR5_INP_AX20	0x0004	// ����drive�� �����ϴ� ��2�� ����
#define WR5_INP_AX21	0x0008	//
#define WR5_INP_AX30	0x0010	// ����drive�� �����ϴ� ��3�� ����(2�ຸ���� ����Ұ�� ������ ���� ���)
#define WR5_INP_AX31	0x0020	//
#define WR5_INP_LSPD0	0x0100	// ����drive�� ��������mode�� ����
#define WR5_INP_LSPD1	0x0200	// D1D0	00:����������ȿ,01:2�༱������,10:�����Ұ�,11:3�༱������
#define WR5_INP_EXPLS	0x0800	// �ܺ����ۿ���(0:���۾���,1:����drive�� �ܺν�ȣ-EXPLSN-�� step����)
#define	WR5_INP_CMPLS	0x1000	// (0:���۾���,1:����drive�� command�� step����)
#define WR5_INP_CIINT	0x4000	// ���Ӻ������� interrupt�߻� �㰡/���� ����(0:����,1:�㰡)
#define WR5_INP_BPINT	0x8000	// bit pattern������ interrupt�߻� �㰡/���� ����(0:����,1:�㰡)

#define WR5_INP_AXIS1_X				0x0000						// ������ ��1�࿡ �����ϰ��� �ϴ��� ����
#define WR5_INP_AXIS1_Y				WR5_INP_AX10				//
#define WR5_INP_AXIS1_Z				WR5_INP_AX11				//
#define WR5_INP_AXIS1_U				(WR5_INP_AX10|WR5_INP_AX11)	//
#define WR5_INP_AXIS2_X				0x0000						// ������ ��2�࿡ �����ϰ��� �ϴ��� ����
#define WR5_INP_AXIS2_Y				WR5_INP_AX20				//
#define WR5_INP_AXIS2_Z				WR5_INP_AX21				//
#define WR5_INP_AXIS2_U				(WR5_INP_AX20|WR5_INP_AX21)	//
#define WR5_INP_AXIS3_X				0x0000						// ������ ��3�࿡ �����ϰ��� �ϴ��� ����
#define WR5_INP_AXIS3_Y				WR5_INP_AX30				//
#define WR5_INP_AXIS3_Z				WR5_INP_AX31				//
#define WR5_INP_AXIS3_U				(WR5_INP_AX30|WR5_INP_AX31)	//
#define WR5_INP_LCMODE_INVALIDATE	0x0000						// ����drive�� ��ȿ mode
#define WR5_INP_LCMODE_2AXIS		WR5_INP_LSPD0				// 2�� ���� ����
#define WR5_INP_LCMODE_3AXIS		(WR5_INP_LSPD0|WR5_INP_LSPD1)// 3�� ���� ����

// WR6 (write data register 1)
#define WR6_OUT_WD0		0x0001			// ���� ��� register
#define WR6_OUT_WD1		0x0002			//
#define WR6_OUT_WD2		0x0004			//
#define WR6_OUT_WD3		0x0008			//
#define WR6_OUT_WD4		0x0010			//
#define WR6_OUT_WD5		0x0020			//
#define WR6_OUT_WD6		0x0040			//
#define WR6_OUT_WD7		0x0080			//
#define WR6_OUT_WD8		0x0100			//
#define WR6_OUT_WD9		0x0200			//
#define WR6_OUT_WD10	0x0400			//
#define WR6_OUT_WD11	0x0800			//
#define WR6_OUT_WD12	0x1000			//
#define WR6_OUT_WD13	0x2000			//
#define WR6_OUT_WD14	0x4000			//
#define WR6_OUT_WD15	0x8000			//

// WR7 (write data register 2)
#define WR7_OUT_WD0		0x0001			// ���� ��� register
#define WR7_OUT_WD1		0x0002			//
#define WR7_OUT_WD2		0x0004			//
#define WR7_OUT_WD3		0x0008			//
#define WR7_OUT_WD4		0x0010			//
#define WR7_OUT_WD5		0x0020			//
#define WR7_OUT_WD6		0x0040			//
#define WR7_OUT_WD7		0x0080			//
#define WR7_OUT_WD8		0x0100			//
#define WR7_OUT_WD9		0x0200			//
#define WR7_OUT_WD10	0x0400			//
#define WR7_OUT_WD11	0x0800			//
#define WR7_OUT_WD12	0x1000			//
#define WR7_OUT_WD13	0x2000			//
#define WR7_OUT_WD14	0x4000			//
#define WR7_OUT_WD15	0x8000			//
#define WR6_OUT_WD16	WR7_OUT_WD0		// ���� ��� register
#define WR6_OUT_WD17	WR7_OUT_WD1		//
#define WR6_OUT_WD18	WR7_OUT_WD2		//
#define WR6_OUT_WD19	WR7_OUT_WD3		//
#define WR6_OUT_WD20	WR7_OUT_WD4		//
#define WR6_OUT_WD21	WR7_OUT_WD5		//
#define WR6_OUT_WD22	WR7_OUT_WD6		//
#define WR6_OUT_WD23	WR7_OUT_WD7		//
#define WR6_OUT_WD24	WR7_OUT_WD8		//
#define WR6_OUT_WD25	WR7_OUT_WD9		//
#define WR6_OUT_WD26	WR7_OUT_WD10	//
#define WR6_OUT_WD27	WR7_OUT_WD11	//
#define WR6_OUT_WD28	WR7_OUT_WD12	//
#define WR6_OUT_WD29	WR7_OUT_WD13	//
#define WR6_OUT_WD30	WR7_OUT_WD14	//
#define WR6_OUT_WD31	WR7_OUT_WD15	//

// RR0 (�� status register)
#define RR0_X_DRV		0x0001	// X�� drive�� ���� ǥ��(1:drive pulse�����,0:drive������)
#define RR0_Y_DRV		0x0002	// Y
#define RR0_Z_DRV		0x0004	// Z
#define RR0_U_DRV		0x0008	// U
#define RR0_X_ERR		0x0010	// X�� error�߻����� ǥ��(1:RR1,RR2 register�� error bit���� 1���� �����ǵ� ����)
#define RR0_Y_ERR		0x0020	// Y
#define RR0_Z_ERR		0x0040	// Z
#define RR0_U_ERR		0x0080	// U
#define RR0_I_DRV		0x0100	// ����drive���� ǥ��(1:����drive pulse�����)
#define RR0_CNEXT		0x0200	// (1:���Ӻ��� drive���� ���� node�� ���� parameter data/���� ����� ���԰���)
#define RR0_ZONE0		0x0400	// ��ȣ���� drive�� �־ ���� drive�� ������ ����Ų��.
#define RR0_ZONE1		0x0800	// E2E1E0 000:0, 001:1, 010:2, 011:3, 100:4, 101:5, 110:6, 111:7
#define RR0_ZONE2		0x1000	//
#define RR0_BPSC0		0x2000	// bit pattern���� drive���� stack counter(SC)�� ���� ����Ų��
#define RR0_BPSC1		0x4000	// C1C0 00:0, 01:1, 10:2, 11:3

// RR1 (status register 1)
#define RR1_CMPP		0x0001	// (1:��/������ġ counter>=COMP+ register, 0:else)
#define RR1_CMPM		0x0002	// (1:��/������ġ counter<COMP- register, 0:else)
#define RR1_ASND		0x0004	// (1:������ drive���� ���Ӷ�)
#define RR1_CNST		0x0008	// (1:������ drive���� ���Ӷ�)
#define RR1_DSND		0x0010	// (1:������ drive���� ���Ӷ�)
#define RR1_AASND		0x0020	// (1:S�� ������drive���� ���ӵ�/���ӵ��� �����Ҷ�)
#define RR1_ACNST		0x0040	// (1:S�� ������drive���� ���ӵ�/���ӵ��� �����Ҷ�)
#define RR1_ADSND		0x0080	// (1:S�� ������drive���� ���ӵ�/���ӵ��� �����Ҷ�)
#define RR1_IN0			0x0100	// (1:drive�� �ܺΰ���������ȣnIN0�����ؼ� �����Ͽ�����)
#define RR1_IN1			0x0200	//                            nIN1
#define RR1_IN2			0x0400	//                            nIN2
#define RR1_IN3			0x0800	//                            nIN3
#define RR1_LMTP		0x1000	// (1:drive�� +���� limit��ȣ(nLMTP)�� ���ؼ� �����Ҷ�)
#define RR1_LMTM		0x2000	// (1:drive�� -���� limit��ȣ(nLMTM)�� ���ؼ� �����Ҷ�)
#define RR1_ALARM		0x4000	// (1:servo motor��alarm��ȣ (nALARM)�� ���� �����Ҷ�)
#define RR1_EMG			0x8000	// (1:���������ȣ(EMGN)�� ���ؼ� �����Ҷ�)

// RR2 (status register 2)
#define RR2_SLMTP		0x0001	// (1:COMP+ register�� ������ Ŀ������)
#define RR2_SLMTM		0x0002	// (1:COMP- register�� ������ Ŀ������)
#define RR2_HLMTP		0x0004	// (1:+���� limit��ȣ(nLMTP)�� active level�϶�)
#define RR2_HLMTM		0x0008	// (1:-���� limit��ȣ(nLMTM)�� active level�϶�)
#define RR2_ALARM		0x0010	// (1:servo motor�� alarm��ȣ(nALARM)�� ��ȿ�������� active level�� �Ǿ�����)
#define RR2_EMG			0x0020	// (1:��� ������ȣ(EMGN)�� low level�� �Ǿ�������)
// RR2 (status home search)
#define RR2_HOME		0x0080	// (1:IN2 Signal error at Automatic home-search)
// bit D8-D12 = home search execution status
//		 0 = Waits for an automatic home-search execution command
//		 3 = Step1, Waits for activation of the IN0 signal
//		 8 = Step2, Waits for activation of the IN1 signal
//		12 = Step2, Waits for deactivation of the IN1 signal
//		15 = Step2, Waits for activation of the IN1 signal
//		20 = Step3, Waits for activation of the IN2 signal
//		25 = Step4, Offset driving

// RR3 (status register 3)
#define RR3_PULSE		0x0001	// (1:drive pulse�� rising edge(�������� pulse�� �ö󰡴� ����)�϶�)
#define RR3_PGECM		0x0002	// (1:��/����ġ>=COMP- register)
#define RR3_PLCM		0x0004	// (1:��/����ġ<COMP- register)
#define RR3_PLCP		0x0008	// (1:��/����ġ<COMP+ register)
#define RR3_PGECP		0x0010	// (1:��/����ġ>=COMP+ register)
#define RR3_CEND		0x0020	// (1:������ drive�϶� ������������ pulse����� ����)
#define RR3_CSTA		0x0040	// (1:������ drive�϶� ������������ pulse����� ����)
#define RR3_DEND		0x0080	// (1:drive�� ����)

// RR4,5 (input register 1,2)
#define X_IN0			0x0001	// X���� �Է»��� ǥ��
#define X_IN1			0x0002	//
#define X_IN2			0x0004	//
#define X_IN3			0x0008	//
#define X_EXP			0x0010	//
#define X_EXM			0x0020	//
#define X_XINP			0x0040	//
#define X_XALM			0x0080	//
#define Y_IN0			0x0100	// Y���� �Է»��� ǥ��
#define Y_IN1			0x0200	//
#define Y_IN2			0x0400	//
#define Y_IN3			0x0800	//
#define Y_EXP			0x1000	//
#define Y_EXM			0x2000	//
#define Y_XINP			0x4000	//
#define Y_XALM			0x8000	//
#define Z_IN0			0x0001	// Z���� �Է»��� ǥ��
#define Z_IN1			0x0002	//
#define Z_IN2			0x0004	//
#define Z_IN3			0x0008	//
#define Z_EXP			0x0010	//
#define Z_EXM			0x0020	//
#define Z_XINP			0x0040	//
#define Z_XALM			0x0080	//
#define U_IN0			0x0100	// U���� �Է»��� ǥ��
#define U_IN1			0x0200	//
#define U_IN2			0x0400	//
#define U_IN3			0x0800	//
#define U_EXP			0x1000	//
#define U_EXM			0x2000	//
#define U_XINP			0x4000	//
#define U_XALM			0x8000	//

#define n_IN0			X_IN0	// ���� �Է»��� ǥ��
#define n_IN1			X_IN1	//
#define n_IN2			X_IN2	//
#define n_IN3			X_IN3	//
#define n_EXPP			X_EXP	//
#define n_EXPM			X_EXM	//
#define n_INPOS			X_XINP	//
#define n_ALARM			X_XALM	//

// Message dispatch�� ����� ȣ���Լ� ����
typedef MMC_INT16U (*PMC4BPCI_MSG_DISPATCH)(MMC_INT32U wParam);

typedef struct _stHomeSearch_ {
	union {
		MMC_INT16U	_WR6;
		struct {
			MMC_INT16U	EPCLR			: 1;
			MMC_INT16U	EPINV			: 1;
			MMC_INT16U	POINV			: 1;
			MMC_INT16U	AVTRI			: 1;
			MMC_INT16U	VRING			: 1;
			MMC_INT16U	HMINT			: 1;	// home search interrupt
			MMC_INT16U	none			: 1;
			MMC_INT16U	SMODE			: 1;
			MMC_INT16U	FE0				: 1;
			MMC_INT16U	FE1				: 1;
			MMC_INT16U	FE2				: 1;
			MMC_INT16U	FE3				: 1;
			MMC_INT16U	FE4				: 1;
			MMC_INT16U	filter_delay	: 3;
		};
	};
	union {
		MMC_INT16U	_WR7;
		struct {
			MMC_INT16U	step1_enable	: 1;
			MMC_INT16U	step1_dir		: 1;
			MMC_INT16U	step2_enable	: 1;
			MMC_INT16U	step2_dir		: 1;
			MMC_INT16U	step3_enable	: 1;
			MMC_INT16U	step3_dir		: 1;
			MMC_INT16U	step4_enable	: 1;
			MMC_INT16U	step4_dir		: 1;
			MMC_INT16U	pulse_clear		: 1;
			MMC_INT16U	step3_stop		: 1;
			MMC_INT16U	limit			: 1;
			MMC_INT16U	DCC_E			: 1;
			MMC_INT16U	DCC_L			: 1;
			MMC_INT16U	DCC_pulse_width	: 3;
		};
	};

	MMC_INT32	step4_offset;
	MMC_INT16	hs_speed;	// home search speed
	MMC_INT16	move_speed;	// move speed
} stHomeSearch;



#ifndef __PMC4BPCI_DEFINE_DLL__

#define CALLAPI	__stdcall


//=========================================================================
// SIMPLE FUNCTION
//=========================================================================
HANDLE CALLAPI OpenCard_N( int id, void (WINAPI *isr)(void), int base_addr, int size, int irq);
BOOL CALLAPI CloseCard_N( int id );
BOOL CALLAPI CloseCard_all( void );
WORD CALLAPI InW_N( int id, WORD iocc );
void CALLAPI OutW_N( int id, WORD iocc, int data );
int CALLAPI SetInterrupt_N( int id, void (WINAPI *isr)(void));
char* CALLAPI ValidString( void );
int CALLAPI GetAxes( void );

//=========================================================================
// EX-FUNCTION
//=========================================================================

#define pmc4bpci_set_mod1			pmc4bpci_wwr1
#define pmc4bpci_set_mod2			pmc4bpci_wwr2
#define pmc4bpci_set_mod3			pmc4bpci_wwr3
#define pmc4bpci_set_outdata		pmc4bpci_wwr4
#define pmc4bpci_set_interpolation	pmc4bpci_wwr5
#define pmc4bpci_set_endpoint		pmc4bpci_set_pulse
#define	pmc4bpci_enable_interrupt	pmc4bpci_wwr1


MMC_INT16U CALLAPI pmc4bpci_driver_id(MMC_INT8U* str);
MMC_INT16U CALLAPI pmc4bpci_check_valid_id (MMC_INT16 id);
MMC_INT16U CALLAPI pmc4bpci_open( int id, void (WINAPI *funcIntHandler)(void) );
MMC_INT16U CALLAPI pmc4bpci_close(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_close_all();
MMC_INT16U CALLAPI pmc4bpci_outw(MMC_INT16U id, MMC_INT16U wOffset, MMC_INT16U wData);
MMC_INT16U CALLAPI pmc4bpci_inw(MMC_INT16U id, MMC_INT16U wOffset);
MMC_INT16U CALLAPI pmc4bpci_outb(MMC_INT16U id, MMC_INT16U wOffset, MMC_INT8U wData);
MMC_INT8U  CALLAPI pmc4bpci_inb(MMC_INT16U id, MMC_INT16U wOffset);
MMC_INT16U CALLAPI pmc4bpci_wwr1(MMC_INT16U id, MMC_INT16U axis, MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_wwr2(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_wwr3(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_wwr4(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_wwr5(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_write_data(MMC_INT16U id, MMC_INT16U axis,MMC_INT32U wdata);
MMC_INT16U CALLAPI pmc4bpci_command(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U cmd);
MMC_INT16U CALLAPI pmc4bpci_rr0(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_rr1(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_rr2(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_rr3(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_rr4(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_rr5(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_set_range(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_acac(MMC_INT16U id, MMC_INT16U axis,MMC_INT16U wdata);
MMC_INT16U CALLAPI pmc4bpci_set_acc(MMC_INT16U id, MMC_INT16U axis,MMC_INT16 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_dec(MMC_INT16U id, MMC_INT16U axis,MMC_INT16 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_startv(MMC_INT16U id, MMC_INT16U axis,MMC_INT16 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_speed(MMC_INT16U id, MMC_INT16U axis,MMC_INT16 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_pulse(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_decpoint(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_center(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_lpcounter(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_epcounter(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_compplus(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_compminus(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_accoffset(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 wdata);
MMC_INT16U CALLAPI pmc4bpci_set_axis(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_reset(MMC_INT16U id, MMC_INT16U axis);
MMC_INT32  CALLAPI pmc4bpci_get_logicalposition(MMC_INT16U id, MMC_INT16U axis);
MMC_INT32  CALLAPI pmc4bpci_get_encoderposition(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_get_currentvelocity(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_get_currentacc(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_wait(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_wait_timeout(MMC_INT16U id, MMC_INT16U axis,MMC_INT32U msec);
MMC_INT32U CALLAPI pmc4bpci_get_timeout();
MMC_INT16U CALLAPI pmc4bpci_nextwait(MMC_INT16U id, MMC_INT16U msec);
MMC_INT16U CALLAPI pmc4bpci_bpwait(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_homesearch_sw(MMC_INT16U id, MMC_INT16U axis, MMC_INT16 vel);
MMC_INT16U CALLAPI pmc4bpci_homesearch(MMC_INT16U id, MMC_INT16U axis, stHomeSearch* param );
MMC_INT16U CALLAPI pmc4bpci_pls_move(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pls,MMC_INT16 vel,MMC_INT16 acc);
MMC_INT16U CALLAPI pmc4bpci_pls_move_wait(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pls,MMC_INT16 vel,MMC_INT16 acc);
MMC_INT16U CALLAPI pmc4bpci_pos_move(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pos,MMC_INT16 vel,MMC_INT16 acc);
MMC_INT16U CALLAPI pmc4bpci_pos_move_wait(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pos,MMC_INT16 vel,MMC_INT16 acc);
MMC_INT16U CALLAPI pmc4bpci_cmove(MMC_INT16U id, MMC_INT16U axis,MMC_INT16 vel);
MMC_INT16U CALLAPI pmc4bpci_pls_smove(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pls,MMC_INT16 vel,MMC_INT16 acc, MMC_INT16 acac, MMC_INT16 dec, MMC_INT16 dcac);
MMC_INT16U CALLAPI pmc4bpci_pls_smove_wait(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pls,MMC_INT16 vel,MMC_INT16 acc, MMC_INT16 acac, MMC_INT16 dec, MMC_INT16 dcac);
MMC_INT16U CALLAPI pmc4bpci_pos_smove(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pos,MMC_INT16 vel,MMC_INT16 acc, MMC_INT16 acac, MMC_INT16 dec, MMC_INT16 dcac);
MMC_INT16U CALLAPI pmc4bpci_pos_imove3(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *p);
MMC_INT16U CALLAPI pmc4bpci_pls_imove3(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *p);
MMC_INT16U CALLAPI pmc4bpci_pos_imove2(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *p);
MMC_INT16U CALLAPI pmc4bpci_pls_imove2(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *p);
MMC_INT16U CALLAPI pmc4bpci_pos_iarc(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *cp,MMC_VERTEX *ep,MMC_INT16 dir);
MMC_INT16U CALLAPI pmc4bpci_pos_iarca(MMC_INT16U id, MMC_INT16U *axis,MMC_VERTEX *cp,MMC_DOUBLE ang);
MMC_INT16U CALLAPI pmc4bpci_pos_smove_wait(MMC_INT16U id, MMC_INT16U axis,MMC_INT32 pos,MMC_INT16 vel,MMC_INT16 acc, MMC_INT16 acac, MMC_INT16 dec, MMC_INT16 dcac);
MMC_INT16U CALLAPI pmc4bpci_smove_stop(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_stop(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_dstop(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_set_msgdispatch(PMC4BPCI_MSG_DISPATCH funcMsgHandler,MMC_INT32U wParam);
MMC_INT16U CALLAPI pmc4bpci_set_timeout(MMC_INT32U msec);
MMC_INT16U CALLAPI pmc4bpci_is_drive(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_is_idrive(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_error(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_nextcommand(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_is_zone(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_is_bpstackcounter(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_is_bpdrive(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_compp(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_compm(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_aacc(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_cons(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_dacc(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_aacac(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_acons(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_dacac(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_is_stop(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_flag_in0(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_in1(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_in2(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_in3(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_limitplus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_limitminus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_servoalarm(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_flag_emergency(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_is_error(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_error_slimitplus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_error_slimitminus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_error_hlimitplus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_error_hlimitminus(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_error_servoalarm(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_error_emergency(MMC_INT16U id);
MMC_INT16U CALLAPI pmc4bpci_is_interrupt(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_get_interrupt(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_pulse(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_pulseGEcompm(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_pulseLcompm(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_pulseLcompp(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_pulseGEcompp(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_cend(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_cstart(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_interrupt_enddrive(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_input_status(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_in0(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_in1(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_in2(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_in3(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_exp(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_exm(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_inpos(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_inputstatus_alarm(MMC_INT16U id, MMC_INT16U axis);
MMC_INT16U CALLAPI pmc4bpci_error();
MMC_INT16U CALLAPI pmc4bpci_calc_scale(MMC_INT32U range);
MMC_INT32U CALLAPI pmc4bpci_calc_acceleration(MMC_INT16U acc,MMC_INT32U range);
MMC_INT16U CALLAPI pmc4bpci_calc_velocity(MMC_INT16U vel,MMC_INT32U range);
MMC_INT32U CALLAPI pmc4bpci_calc_acac(MMC_INT32 acac,MMC_INT32U range);
MMC_INT32U CALLAPI pmc4bpci_rev_scale(MMC_INT16U scale);
MMC_INT16U CALLAPI pmc4bpci_rev_acceleration(MMC_INT16U acc,MMC_INT32U range);
MMC_INT16U CALLAPI pmc4bpci_rev_velocity(MMC_INT16U vel,MMC_INT32U range);
MMC_INT16U CALLAPI pmc4bpci_rev_acac(MMC_INT16U acac,MMC_INT32U range);
MMC_DOUBLE CALLAPI util_math_dist(MMC_VERTEX* p1, MMC_VERTEX* p2);
MMC_DOUBLE CALLAPI util_math_angle(MMC_VERTEX* p1, MMC_VERTEX* p2);
MMC_VOID   CALLAPI util_math_polar(MMC_VERTEX* p1, MMC_DOUBLE ang, MMC_DOUBLE len, MMC_VERTEX* p);
MMC_VOID   CALLAPI util_math_circle_3p(MMC_VERTEX* p1,MMC_VERTEX* p2,MMC_VERTEX* p3,MMC_VERTEX* cp,MMC_DOUBLE* r);
MMC_INT16U CALLAPI pmc4bpci_con_imove2abs(MMC_INT16U id, MMC_INT16U *axis, MMC_VERTEX *p, MMC_INT16 n, MMC_DOUBLE scale);


#endif

#ifdef __cplusplus
}
#endif


#endif // __PMC4BPCI_H__
