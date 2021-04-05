using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Free302.TnM.Device;
using static Free302.TnM.Device.Nova8BWrap;
using System.Threading;

namespace serverapp_tester
{
	public partial class Form_Main : Form
	{
		// 좌표 상태
		double mPosAX = 0; double mPosAY = 0; double mPosAZ = 0; double mPosAU = 0;
		double mPosBX = 0; double mPosBY = 0; double mPosBU = 0; double mPosBZ = 0;

		// 리미트 상태
		bool mLmtCWAX = false; bool mLmtCCWAX = false; bool mLmtCWAY = false; bool mLmtCCWAY = false;
		bool mLmtCWAZ = false; bool mLmtCCWAZ = false; bool mLmtCWAU = false; bool mLmtCCWAU = false;
		bool mLmtCWBX = false; bool mLmtCCWBX = false; bool mLmtCWBY = false; bool mLmtCCWBY = false;
		bool mLmtCWBZ = false; bool mLmtCCWBZ = false; bool mLmtCWBU = false; bool mLmtCCWBU = false;

		// 열기 상태
		//bool mOpen = false;
		


		public Form_Main()
		{
			InitializeComponent();
		}

		private void Form_Main_Load(object sender, EventArgs e)
		{
			Nova8MC.XMLNodeSetting();
		}


		// M/C 보드 열기
		private void button_Open_Click(object sender, EventArgs e)
		{
			int vMcBoardNo = (int)this.numericUpDown_BoardNo.Value;

			// M/C 보드 열기
			if (Nova8MC.mBoardNo < 0)
			{
				// 보드번호 파라미터로 전달 후 정상 열리면 보드넘버 전역변수에 저장
				if (Nova8MC.Open(vMcBoardNo))
				{
					//Thread.Sleep(200);

					//this.panel_Move.Enabled = true;

					// 쓰기 명령 시작
					this.timer_Write.Start();
					// 읽기 명령 시작
					this.timer_Read.Start();
					// 출력 시작
					this.timer_Display.Start();

					this.button_Close.Enabled = true;
					this.button_Open.Enabled = false;

					// 리미트 액티브 설정
					Nova8MC.mIsLimitLevel[0] = !this.checkBox_LmtHLAX.Checked; 
					Nova8MC.mIsLimitLevel[1] = !this.checkBox_LmtHLAY.Checked; 
					Nova8MC.mIsLimitLevel[2] = !this.checkBox_LmtHLAZ.Checked; 
					Nova8MC.mIsLimitLevel[3] = !this.checkBox_LmtHLAU.Checked; 

					Nova8MC.mIsLimitLevel[4] = !this.checkBox_LmtHLBX.Checked; 
					Nova8MC.mIsLimitLevel[5] = !this.checkBox_LmtHLBY.Checked; 
					Nova8MC.mIsLimitLevel[6] = !this.checkBox_LmtHLBZ.Checked; 
					Nova8MC.mIsLimitLevel[7] = !this.checkBox_LmtHLBU.Checked; 
					//this.mOpen = true;
				}
			}
		}

		// M/C 보드 닫기
		private void button_Close_Click(object sender, EventArgs e)
		{
			// M/C 보드 닫기
			if (!Nova8MC.Close(Nova8MC.mBoardNo)) return;

			//this.panel_Move.Enabled = false;

			// 쓰기 명령 중지
			this.timer_Write.Stop();
			// 읽기 명령 중지
			this.timer_Read.Stop();
			// 출력 중지
			//this.timer_Display.Stop();

			this.button_Open.Enabled = true;
			this.button_Close.Enabled = false;
			Nova8MC.mBoardNo = -1;
			//this.mOpen = false;
		}

		// 콘솔 출력
		private void timer_Consol_Tick(object sender, EventArgs e)
		{
			// 현재 논리 위치 출력
			Console.WriteLine("★Position: {8}\n\r	AX={0},AY={1},AZ={2},AU={3},BX={4},BY={5},BZ={6},BU={7}"
				, mPosAX, mPosAY, mPosAZ, mPosAU, mPosBX, mPosBY, mPosBZ, mPosBU, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

			// 현재 리미트 상태 출력
			Console.WriteLine("＠Limit: {8}\n\r	AXCW={0},AXCCW={1},AYCW={2},AYCCW={3},AZCW={4},AZCCW={5},AUCW={6},AUCCW={7}\n\r"
			   , Convert.ToInt16(mLmtCWAX), Convert.ToInt16(mLmtCCWAX)
			   , Convert.ToInt16(mLmtCWAY), Convert.ToInt16(mLmtCCWAY)
			   , Convert.ToInt16(mLmtCWAZ), Convert.ToInt16(mLmtCCWAZ)
			  , Convert.ToInt16(mLmtCWAU), Convert.ToInt16(mLmtCCWAU), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

		}

		// 쓰기 명령
		private void timer_Write_Tick(object sender, EventArgs e)
		{
			/*
			// 연속 왕복 테스트
			// 실험결과: 왕복지정된 거리만큼 움직이면 원래 있던곳으로 돌아와야 하나
			// 한쪽 방향으로 미세하게 Shift 되는 현상이 있음.
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AX, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AY, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AU, 1000);

			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BX, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BY, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ, 1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BU, 1000);

			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AX);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AY);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AU);

			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BX);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BY);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BU);

			//Thread.Sleep(100);

			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AX, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AY, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.AU, -1000);

			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BX, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BY, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ, -1000);
			Nova8MC.Move(Nova8MC.mBoardNo, Nova8MC.McAxis.BU, -1000);

			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AX);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AY);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.AU);

			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BX);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BY);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ);
			Nova8MC.Stop(Nova8MC.mBoardNo, Nova8MC.McAxis.BU);

			*/
		}

		// 읽기 명령
		private void timer_Read_Tick(object sender, EventArgs e)
		{
			// 현재 논리 위치 읽어옴
			mPosAX = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.AX);
			mPosAY = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.AY);
			mPosAZ = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ);
			mPosAU = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.AU);
			mPosBX = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.BX);
			mPosBY = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.BY);
			mPosBZ = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ);
			mPosBU = Nova8MC.GetPosition(Nova8MC.mBoardNo, Nova8MC.McAxis.BU);

			// 현재 리미트 상태 읽어옴
			mLmtCWAX = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AX);
			mLmtCCWAX = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AX);
			mLmtCWAY = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AY);
			mLmtCCWAY = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AY);
			mLmtCWAZ = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ);
			mLmtCCWAZ = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AZ);
			mLmtCWAU = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AU);
			mLmtCCWAU = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.AU);


			mLmtCWBX = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BX);
			mLmtCCWBX = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BX);
			mLmtCWBY = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BY);
			mLmtCCWBY = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BY);
			mLmtCWBZ = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ);
			mLmtCCWBZ = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BZ);
			mLmtCWBU = Nova8MC.IsLimitCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BU);
			mLmtCCWBU = Nova8MC.IsLimitCCW(Nova8MC.mBoardNo, Nova8MC.McAxis.BU);
		}

		// 출력
		private void timer_Display_Tick(object sender, EventArgs e)
		{
			// 제어 패널 활성 여부
			this.panel_Move.Enabled = Nova8MC.mIsOpen;

			// 현재 논리 위치 출력
			this.label_PosAX.Text = mPosAX.ToString("#,0.00");
			this.label_PosAY.Text = mPosAY.ToString("#,0.00");
			this.label_PosAZ.Text = mPosAZ.ToString("#,0.00");
			this.label_PosAU.Text = mPosAU.ToString("#,0.00");

			this.label_PosBX.Text = mPosBX.ToString("#,0.00");
			this.label_PosBY.Text = mPosBY.ToString("#,0.00");
			this.label_PosBZ.Text = mPosBZ.ToString("#,0.00");
			this.label_PosBU.Text = mPosBU.ToString("#,0.00");

			// 현재 리미트 상태 출력
			this.checkBox_LmtCWAX.Checked = mLmtCWAX; this.checkBox_LmtCCWAX.Checked = mLmtCCWAX;
			this.checkBox_LmtCWAY.Checked = mLmtCWAY; this.checkBox_LmtCCWAY.Checked = mLmtCCWAY;
			this.checkBox_LmtCWAZ.Checked = mLmtCWAZ; this.checkBox_LmtCCWAZ.Checked = mLmtCCWAZ;
			this.checkBox_LmtCWAU.Checked = mLmtCWAU; this.checkBox_LmtCCWAU.Checked = mLmtCCWAU;

			this.checkBox_LmtCWBX.Checked = mLmtCWBX; this.checkBox_LmtCCWBX.Checked = mLmtCCWBX;
			this.checkBox_LmtCWBY.Checked = mLmtCWBY; this.checkBox_LmtCCWBY.Checked = mLmtCCWBY;
			this.checkBox_LmtCWBZ.Checked = mLmtCWBZ; this.checkBox_LmtCCWBZ.Checked = mLmtCCWBZ;
			this.checkBox_LmtCWBU.Checked = mLmtCWBU; this.checkBox_LmtCCWBU.Checked = mLmtCCWBU;

			// 축별 리미트 연결상태
		 	this.button_DistanceAX.Enabled = this.numericUpDown_SpeedAX.Enabled = this.button_HomeAX.Enabled = this.button_ZeroAX.Enabled 
				= this.numericUpDown_PulseAX.Enabled = this.button_StopAX.Enabled
				= this.numericUpDown_RatioAX.Enabled = this.numericUpDown_ResolutionAX.Enabled
				= this.button_CWAX.Enabled = this.button_CCWAX.Enabled = this.label_PosAX.Enabled = (!mLmtCWAX | !mLmtCCWAX);
			this.button_DistanceAY.Enabled = this.numericUpDown_SpeedAY.Enabled = this.button_HomeAY.Enabled = this.button_ZeroAY.Enabled 
				= this.numericUpDown_PulseAY.Enabled = this.button_StopAY.Enabled
				= this.numericUpDown_RatioAY.Enabled = this.numericUpDown_ResolutionAY.Enabled
				= this.button_CWAY.Enabled = this.button_CCWAY.Enabled = this.label_PosAY.Enabled = (!mLmtCWAY | !mLmtCCWAY);
			this.button_DistanceAZ.Enabled = this.numericUpDown_SpeedAZ.Enabled = this.button_HomeAZ.Enabled = this.button_ZeroAZ.Enabled 
				= this.numericUpDown_PulseAZ.Enabled = this.button_StopAZ.Enabled
				= this.numericUpDown_RatioAZ.Enabled = this.numericUpDown_ResolutionAZ.Enabled
				= this.button_CWAZ.Enabled = this.button_CCWAZ.Enabled = this.label_PosAZ.Enabled = (!mLmtCWAZ | !mLmtCCWAZ);
			this.button_DistanceAU.Enabled = this.numericUpDown_SpeedAU.Enabled = this.button_HomeAU.Enabled = this.button_ZeroAU.Enabled 
				= this.numericUpDown_PulseAU.Enabled = this.button_StopAU.Enabled
				= this.numericUpDown_RatioAU.Enabled = this.numericUpDown_ResolutionAU.Enabled
				= this.button_CWAU.Enabled = this.button_CCWAU.Enabled = this.label_PosAU.Enabled = (!mLmtCWAU | !mLmtCCWAU);

			this.button_DistanceBX.Enabled = this.numericUpDown_SpeedBX.Enabled = this.button_HomeBX.Enabled = this.button_ZeroBX.Enabled
				= this.numericUpDown_PulseBX.Enabled = this.button_StopBX.Enabled
				= this.numericUpDown_RatioBX.Enabled = this.numericUpDown_ResolutionBX.Enabled
				= this.button_CWBX.Enabled = this.button_CCWBX.Enabled = this.label_PosBX.Enabled = (!mLmtCWBX | !mLmtCCWBX);
			this.button_DistanceBY.Enabled = this.numericUpDown_SpeedBY.Enabled = this.button_HomeBY.Enabled = this.button_ZeroBY.Enabled 
				= this.numericUpDown_PulseBY.Enabled = this.button_StopBY.Enabled
				= this.numericUpDown_RatioBY.Enabled = this.numericUpDown_ResolutionBY.Enabled
				= this.button_CWBY.Enabled = this.button_CCWBY.Enabled = this.label_PosBY.Enabled = (!mLmtCWBY | !mLmtCCWBY);
			this.button_DistanceBZ.Enabled = this.numericUpDown_SpeedBZ.Enabled = this.button_HomeBZ.Enabled = this.button_ZeroBZ.Enabled
				= this.numericUpDown_PulseBZ.Enabled = this.button_StopBZ.Enabled
				= this.numericUpDown_RatioBZ.Enabled = this.numericUpDown_ResolutionBZ.Enabled
				= this.button_CWBZ.Enabled = this.button_CCWBZ.Enabled = this.label_PosBZ.Enabled = (!mLmtCWBZ | !mLmtCCWBZ);
			this.button_DistanceBU.Enabled = this.numericUpDown_SpeedBU.Enabled = this.button_HomeBU.Enabled = this.button_ZeroBU.Enabled 
				= this.numericUpDown_PulseBU.Enabled = this.button_StopBU.Enabled
				= this.numericUpDown_RatioBU.Enabled = this.numericUpDown_ResolutionBU.Enabled
				= this.button_CWBU.Enabled = this.button_CCWBU.Enabled = this.label_PosBU.Enabled = (!mLmtCWBU | !mLmtCCWBU);
		}

		/// <summary>
		/// Move CW
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_CWAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());
			int vPulse = 0;

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAX.Value; vPulse = (int)this.numericUpDown_PulseAX.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAX.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAX.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAY.Value; vPulse = (int)this.numericUpDown_PulseAY.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAY.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAY.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAZ.Value; vPulse = (int)this.numericUpDown_PulseAZ.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAZ.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAZ.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAU.Value; vPulse = (int)this.numericUpDown_PulseAU.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAU.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAU.Value;
			}

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBX.Value; vPulse = (int)this.numericUpDown_PulseBX.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBX.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBX.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBY.Value; vPulse = (int)this.numericUpDown_PulseBY.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBY.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBY.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBZ.Value; vPulse = (int)this.numericUpDown_PulseBZ.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBZ.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBZ.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBU.Value; vPulse = (int)this.numericUpDown_PulseBU.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBU.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBU.Value;
			}

			Nova8MC.Move(Nova8MC.mBoardNo, vMcAxis, vPulse);
		}

		/// <summary>
		/// Move CCW
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_CCWAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());

			int vPulse = 0;

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAX.Value; vPulse = (int)this.numericUpDown_PulseAX.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAX.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAX.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAY.Value; vPulse = (int)this.numericUpDown_PulseAY.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAY.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAY.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAZ.Value; vPulse = (int)this.numericUpDown_PulseAZ.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAZ.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAZ.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAU.Value; vPulse = (int)this.numericUpDown_PulseAU.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioAU.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionAU.Value;
			}

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBX.Value; vPulse = (int)this.numericUpDown_PulseBX.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBX.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBX.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBY.Value; vPulse = (int)this.numericUpDown_PulseBY.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBY.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBY.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBZ.Value; vPulse = (int)this.numericUpDown_PulseBZ.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBZ.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBZ.Value;
			}
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBU.Value; vPulse = (int)this.numericUpDown_PulseBU.Value;
				Nova8MC.mRatio[vArrayNo] = (int)this.numericUpDown_RatioBU.Value; Nova8MC.mUnitToPolar[vArrayNo] = (int)this.numericUpDown_ResolutionBU.Value;
			}

			Nova8MC.Move(Nova8MC.mBoardNo, vMcAxis, -(int)vPulse);
		}
		
		/// <summary>
		/// Stop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_StopAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			Nova8MC.Stop(Nova8MC.mBoardNo, vMcAxis);
		}
		
		/// <summary>
		/// Zero
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_ZeroAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());

			int vPulse = 0;

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAX.Value; vPulse = (int)this.numericUpDown_PulseAX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAY.Value; vPulse = (int)this.numericUpDown_PulseAY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAZ.Value; vPulse = (int)this.numericUpDown_PulseAZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAU.Value; vPulse = (int)this.numericUpDown_PulseAU.Value; }

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBX.Value; vPulse = (int)this.numericUpDown_PulseBX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBY.Value; vPulse = (int)this.numericUpDown_PulseBY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBZ.Value; vPulse = (int)this.numericUpDown_PulseBZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBU.Value; vPulse = (int)this.numericUpDown_PulseBU.Value; }

			Nova8MC.Zero(Nova8MC.mBoardNo, vMcAxis);
		}
		
		/// <summary>
		/// Home
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_HomeAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());

			int vPulse = 0;

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAX.Value; vPulse = (int)this.numericUpDown_PulseAX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAY.Value; vPulse = (int)this.numericUpDown_PulseAY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAZ.Value; vPulse = (int)this.numericUpDown_PulseAZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAU.Value; vPulse = (int)this.numericUpDown_PulseAU.Value; }

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBX.Value; vPulse = (int)this.numericUpDown_PulseBX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBY.Value; vPulse = (int)this.numericUpDown_PulseBY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBZ.Value; vPulse = (int)this.numericUpDown_PulseBZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBU.Value; vPulse = (int)this.numericUpDown_PulseBU.Value; }

			Nova8MC.Home(Nova8MC.mBoardNo, vMcAxis);
		}

		/// <summary>
		/// 이동거리 설정
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button_DistanceAX_Click(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as Button).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());

			int vPulse = 0;

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAX.Value; vPulse = (int)this.numericUpDown_PulseAX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAY.Value; vPulse = (int)this.numericUpDown_PulseAY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAZ.Value; vPulse = (int)this.numericUpDown_PulseAZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedAU.Value; vPulse = (int)this.numericUpDown_PulseAU.Value; }

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBX.Value; vPulse = (int)this.numericUpDown_PulseBX.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBY.Value; vPulse = (int)this.numericUpDown_PulseBY.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBZ.Value; vPulse = (int)this.numericUpDown_PulseBZ.Value; }
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mSpeed[vArrayNo] = (int)this.numericUpDown_SpeedBU.Value; vPulse = (int)this.numericUpDown_PulseBU.Value; }

			Nova8MC.Distance(Nova8MC.mBoardNo, vMcAxis);
		}

		/// <summary>
		/// 리미트 High, Low 설정
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBox_LmtHLAX_CheckedChanged(object sender, EventArgs e)
		{
			Nova8MC.McAxis vMcAxis = (Nova8MC.McAxis)Enum.Parse(typeof(Nova8MC.McAxis), (sender as CheckBox).Tag.ToString().ToUpper());
			int vArrayNo = (int)Enum.Parse(typeof(Nova8MC.AxisArrayNo), vMcAxis.ToString().ToUpper());

			if (vMcAxis == Nova8MC.McAxis.AX) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLAX.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.AY) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLAY.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.AZ) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLAZ.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.AU) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLAU.Checked; }

			else if (vMcAxis == Nova8MC.McAxis.BX) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLBX.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.BY) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLBY.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.BZ) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLBZ.Checked; }
			else if (vMcAxis == Nova8MC.McAxis.BU) { Nova8MC.mIsLimitLevel[vArrayNo] = !this.checkBox_LmtHLBU.Checked; }
		}

	}
}
