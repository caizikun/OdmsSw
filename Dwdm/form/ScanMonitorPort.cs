using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Neon.Aligner;
using Free302.TnM.Neon.Pigtail;
using Free302.MyLibrary.Utility;


namespace ScanTest
{
	public partial class ScanMonitorPort : Form
	{

		ScanForm mScanForm;
		Istage mLeft;
		Istage mRight;

		#region ---- Class & UI 초기화 ---

		public ScanMonitorPort()
		{
			InitializeComponent();

			mScanForm = MyLogic.CreateAndShow<ScanForm>(true, false);
			mLeft = CGlobal.LeftAligner;
			mRight = CGlobal.RightAligner;

			initAlignUi();
		}


		private void initAlignUi()
		{
			uiMoveCH1.Click += uiMoveCh_Click;
			uiMoveCH2.Click += uiMoveCh_Click;
			uiMoveCH3.Click += uiMoveCh_Click;
			uiMoveCH4.Click += uiMoveCh_Click;

			uiMoveCom.Click += uiMoveCh_Click;
			uiMoveM1.Click += uiMoveCh_Click;
			uiMoveM2.Click += uiMoveCh_Click;
			uiMoveM3.Click += uiMoveCh_Click;
			uiMoveM4.Click += uiMoveCh_Click;

			uiMoveCH1.Tag = mLeft;
			uiMoveCH2.Tag = mLeft;
			uiMoveCH3.Tag = mLeft;
			uiMoveCH4.Tag = mLeft;

			uiMoveCom.Tag = mRight;
			uiMoveM1.Tag = mRight;
			uiMoveM2.Tag = mRight;
			uiMoveM3.Tag = mRight;
			uiMoveM4.Tag = mRight;

			mCurrentButton = new Dictionary<Istage, Button> { { mLeft, uiMoveCH1 }, { mRight, uiMoveCom } };
			mButtonCoord = new Dictionary<Button, int>
			{
				{ uiMoveCH1, 0},
				{ uiMoveCH2, 1*DistanceChPitch},
				{ uiMoveCH3, 2*DistanceChPitch},
				{ uiMoveCH4, 3*DistanceChPitch},
				{ uiMoveCom, 0},
				{ uiMoveM1, DistanceCom2M1},
				{ uiMoveM2, 1*DistanceMonitorPitch+DistanceCom2M1},
				{ uiMoveM3, 2*DistanceMonitorPitch+DistanceCom2M1},
				{ uiMoveM4, 3*DistanceMonitorPitch+DistanceCom2M1}
			};

			mChMoveButton = new Dictionary<int, Button>
			{
				{ 0, uiMoveCH1 }, {1, uiMoveCH2 }, {2, uiMoveCH3 }, {3, uiMoveCH4 }
			};
			mMonitorMoveButton = new Dictionary<int, Button>
			{
				{ 0, uiMoveM1 }, {1, uiMoveM2 }, {2, uiMoveM3 }, {3, uiMoveM4 }
			};

		}

		#endregion



		#region ==== Aligner ====
		
		double[] mCoordCh1Com = new double[2];
		const int DistanceCom2M4 = 3000;
		const int DistanceCom2M1 = 2250;
		const int DistanceChPitch = 750;
		const int DistanceMonitorPitch = 250;
		Dictionary<Istage, Button> mCurrentButton;
		Dictionary<Button, int> mButtonCoord;

		public async void uiMoveCh_Click(object sender, EventArgs e)
		{
			//이동 버튼
			try
			{
				((Control)sender).Enabled = false;
				await moveCh(sender);
			}
			finally
			{
				((Control)sender).Enabled = true;
			}
		}


		async Task moveCh(object sender)
		{
			var button = (Button)sender;
			mScanForm?.AddLog($"{button.Text}_Click");

			var aligner = (Istage)button.Tag;
			var lastBtn = mCurrentButton[aligner];
			lastBtn.ForeColor = Color.Black;
			var dx = mButtonCoord[button] - mButtonCoord[lastBtn];
			await ScanLogic.MoveAs(aligner, aligner.AXIS_X, dx);

			mCurrentButton[aligner] = button;
			button.ForeColor = Color.OrangeRed;
		}


		private async void uiAlign_Click(object sender, EventArgs e)
		{
			//XY Align 정렬버튼
			try
			{
				((Control)sender).Enabled = false;
				await doAlign(sender);
			}
			finally
			{
				((Control)sender).Enabled = true;
			}
		}


		async Task doAlign(object sender)
		{
			mScanForm?.AddLog($"{((Button)sender).Text}_Click");
			var form = MyLogic.CreateAndShow<AlignForm>(true, false);
			(sender == uiAlignLeft ? form.btnFINE_L_Digital : form.btnFINE_L_Digital).PerformClick();
			await Task.Delay(1000);
		}


		async Task doApproach()
		{
			var form = MyLogic.CreateAndShow<AlignForm>(true, false);
			form.btnZapp_L.PerformClick();
			await Task.Delay(1000);
			form.btnZapp_R.PerformClick();
			await Task.Delay(1000);
		}


		private async void uiSaveCh1ComCoord_Click(object sender, EventArgs e)
		{
			//현재위치를 CH1↔COM 으로 기억
			await saveCh1ComCoord(sender);
		}


		async Task saveCh1ComCoord(object sender)
		{
			mScanForm?.AddLog($"{((Button)sender).Text}_Click");
			mCoordCh1Com[0] = mLeft.GetAxisAbsPos(mLeft.AXIS_X);
			mCoordCh1Com[1] = mRight.GetAxisAbsPos(mRight.AXIS_X);

			mCurrentButton[mLeft] = uiMoveCH1;
			uiMoveCH1.ForeColor = Color.OrangeRed;
			mCurrentButton[mRight] = uiMoveCom;
			uiMoveCom.ForeColor = Color.OrangeRed;
			await Task.Delay(500);
		}


		private async void uiMoveNextChip_Click(object sender, EventArgs e)
		{
			//다음 칩으로 이동
			try
			{
				((Control)sender).Enabled = false;
				await moveNextChip(sender);
			}
			finally
			{
				((Control)sender).Enabled = true;
			}
		}


		async Task moveNextChip(object sender)
		{
			mScanForm?.AddLog($"{((Button)sender).Text}_Click");
			var dx = uiDistanceNextChip.Text.To<double>();

			dx -= mButtonCoord[mCurrentButton[mLeft]];
			await ScanLogic.MoveAs(mLeft, mLeft.AXIS_X, dx);

			dx -= mButtonCoord[mCurrentButton[mRight]];
			await ScanLogic.MoveAs(mRight, mRight.AXIS_X, dx);

			await saveCh1ComCoord(uiMoveNextChip);
		}


		#endregion



		#region ==== Monitor Port 자동 측정

		Dictionary<int, Button> mChMoveButton;
		Dictionary<int, Button> mMonitorMoveButton;

		double mZbuffer = 100;
		int[] mChWave = { 1271, 1291, 1311, 1331 };             //CH별 설정 파장

		private async void btnMonitorScanStart_Click(object sender, EventArgs e)
		{
			if (mScanForm == null) return;

			if (!mScanForm.InitState)
			{
				MessageBox.Show("PD 초기화 확인!");
				return;
			}
			mScanForm.AddLog("", true);

			//모니터 포트 자동 측정
			mScanForm.AddLog($"{((Button)sender).Text}_Click");
			mScanForm.ReturnOrigin(true);						//Return Origin Check!

			try
			{
				mScanForm.AddLog($"1번칩 측정");
				await monitorAuto(txtSaveName.Text);            //1번칩 측정

				if (chkScanNextChip.Checked)
				{
					//Z축 후진
					mScanForm.AddLog($"Z축 후진");
					await ScanLogic.MoveAs(mLeft, mLeft.AXIS_Z, -1 * mZbuffer);
					await ScanLogic.MoveAs(mRight, mRight.AXIS_Z, -1 * mZbuffer);

					//다음칩 이동
					await moveNextChip(uiMoveNextChip);

					//Approach
					mScanForm.AddLog($"Z축 Approach");
					if (mZbuffer != 0) await doApproach();

					//**측정**
					mScanForm.AddLog($"2번칩 측정");
					await monitorAuto(txtSaveName2.Text);       //2번칩 측정
				}

			}
			finally
			{
				//stage position update
				mScanForm.UpdateStagePosition();
				mScanForm.AddLog($"자동 측정 종료!");
			}

		}


		async Task monitorAuto(string saveFileName)
		{

			//현재위치를 CH1↔COM 으로 기억
			await saveCh1ComCoord(uiMoveNextChip);

			for (int i = 0; i < 4; i++)
			{
				//Ch port || Comport 이동
				await moveCh(mChMoveButton[i]);
				await moveCh(uiMoveCom);

				//파장 변경
				await setTlsWave(mChWave[i]);

				//정렬
				await doAlign(uiAlignLeft);
				await doAlign(uiAlignRight);

				//측정 [Comport]
				mScanForm.mSaveFileName = $"{saveFileName}-CH{i + 1}-COM";
				await mScanForm.doScan();

				//monitor 이동
				//await moveCh(mChMoveButton[i]);
				await moveCh(mMonitorMoveButton[i]);

				//정렬
				//await doAlign(uiAlignLeft);
				await doAlign(uiAlignRight);

				//측정 [monitor port]
				mScanForm.mSaveFileName = $"{saveFileName}-CH{i + 1}-M{i + 1}";
				await mScanForm.doScan();

			}

			//시작위치 Return : CH1 - Comport
			await moveCh(mChMoveButton[0]);
			await moveCh(uiMoveCom);

			//파장 변경	CH1	
			await setTlsWave(mChWave[0]);

		}


		private async Task setTlsWave(int wave)
		{
			var form = MyLogic.CreateAndShow<frmSourceController>(true, false);
			form.txtTlsWavelen.Text = wave.ToString();
			form.btnTlsLambda.PerformClick();
			await Task.Delay(1000);
		}

	
		#endregion


	}
}
