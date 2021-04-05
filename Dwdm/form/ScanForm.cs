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
    public partial class ScanForm : Form
    {

        #region ---- Class & UI 초기화 ---

        Neon.Dwdm.Properties.Settings mSet = Neon.Dwdm.Properties.Settings.Default;

        public ScanForm()
        {
            InitializeComponent();
        }

        private void ScanForm_Load(object sender, EventArgs e)
        {
            mLeft = CGlobal.LeftAligner;
            mRight = CGlobal.RightAligner;

            initUi();
		}

		Istage mLeft;
		Istage mRight;
		IPm mPD;
		C8164 mOpm;

		Dictionary<ScanAxisId, ComboBox> mAlignerBox;
        Dictionary<ScanAxisId, ComboBox> mAxisBox;
        Dictionary<Button, TextBox> mUiMoveTextBox;

        Dictionary<RangeMode, string> mRanageModeString;
		string[] mAxisString = { "X", "Y", "Z", "Tx", "Ty", "Tz" };
        string[] mAlignerString = { "Left", "Right" };

        void initUi()
        {
			//PM 관련 UI
			cboDaqPrimary.DataSource = Enum.GetValues(typeof(DaqPrimary));
			cboDaqPrimary.SelectedIndex = 0;
			cboAiCh.DataSource = Enum.GetValues(typeof(DaqAiCh));
			cboAiCh.SelectedIndex = 0;
			cboRse.DataSource = Enum.GetValues(typeof(NationalInstruments.DAQmx.CIEncoderAInputTerminalConfiguration));
			cboRse.SelectedIndex = 1;
			cboAvgTime.DataSource = Enum.GetValues(typeof(PmAvgTime));
			cboAvgTime.SelectedItem = PmAvgTime.T100ms;

			//Scan Axis 관련 UI
			mAlignerBox = new Dictionary<ScanAxisId, ComboBox> {
				{ ScanAxisId.X1, uiAligner1 }, { ScanAxisId.X2, uiAligner2 } ,{ ScanAxisId.X3, uiAligner3 } };
			mAxisBox = new Dictionary<ScanAxisId, ComboBox> {
				{ ScanAxisId.X1, cbAxis1 }, { ScanAxisId.X2, cbAxis2 } ,{ ScanAxisId.X3, cbAxis3 } };
            mRanageModeString = new Dictionary<RangeMode, string> {
                { RangeMode.Full, "-R → +R" }, { RangeMode.PosDir, "0 → +R" }, { RangeMode.NegDir, "0 → -R" } };
            mUiMoveTextBox = new Dictionary<Button, TextBox> {
                { uiMoveOrigin1, uiOrigin1 }, {uiMoveOrigin2, uiOrigin2 },
				{ uiMoveCenter1, uiCenter1 }, {uiMoveCenter2, uiCenter2 },
                { uiMovePeak1, uiPeak1 } , { uiMovePeak2, uiPeak2 } };

            btnScan.Enabled = false;

            cboMoveOption1.DataSource = mRanageModeString.ToList();
            cboMoveOption1.DisplayMember = "Value";
            cboMoveOption1.ValueMember = "Key";
            cboMoveOption1.SelectedIndex = 0;
            cboMoveOption2.DataSource = mRanageModeString.ToList();
            cboMoveOption2.DisplayMember = "Value";
            cboMoveOption2.ValueMember = "Key";
            cboMoveOption2.SelectedIndex = 0;
			cboMoveOption3.DataSource = mRanageModeString.ToList();
			cboMoveOption3.DisplayMember = "Value";
			cboMoveOption3.ValueMember = "Key";
			cboMoveOption3.SelectedIndex = 2;
			cboMoveOption3.Enabled = false;

			uiAligner1.Items.AddRange(mAlignerString);
			uiAligner1.SelectedIndex = 0;
			uiAligner2.Items.AddRange(mAlignerString);
            uiAligner2.SelectedIndex = 0;
			uiAligner3.Items.AddRange(mAlignerString);
			uiAligner3.SelectedIndex = 0;

			cbAxis1.Items.AddRange(mAxisString);
			cbAxis1.SelectedIndex = 0;
			cbAxis2.Items.AddRange(mAxisString);
            cbAxis2.SelectedIndex = 1;
			cbAxis3.Items.AddRange(mAxisString);
			cbAxis3.SelectedIndex = 2;

            //move
            uiMoveOrigin1.Click += uiMove_Click;
            uiMoveOrigin2.Click += uiMove_Click;
            uiMovePeak1.Click += uiMove_Click;
            uiMovePeak2.Click += uiMove_Click;
            uiMoveCenter1.Click += uiMove_Click;
            uiMoveCenter2.Click += uiMove_Click;

			uiAligner1.SelectedIndexChanged += uiAligner1_SelectedIndexChanged;

		}

        Istage selectedAligner(ScanAxisId axisId)
        {
            return mAlignerBox[axisId].SelectedItem.Equals("Left") ? mLeft : mRight;
        }

        int selectedAxis(ScanAxisId axisId)
        {
            var aligner = selectedAligner(axisId);
            var axisString = mAxisBox[axisId].SelectedItem.ToString();

            if (axisString == "X") return aligner.AXIS_X;
            if (axisString == "Y") return aligner.AXIS_Y;
            if (axisString == "Z") return aligner.AXIS_Z;
            if (axisString == "Tx") return aligner.AXIS_TX;
            if (axisString == "Ty") return aligner.AXIS_TY;

            throw new Exception($"Unknown axis: {axisString}");
        }


		private void uiAligner1_SelectedIndexChanged(object sender, EventArgs e)
		{
			uiAligner2.SelectedIndex = uiAligner1.SelectedIndex;
			uiAligner3.SelectedIndex = uiAligner1.SelectedIndex;
		}
		        
        #endregion



        #region ==== Log 표시 ====

        private void updateCoord(ScanStatus status)
        {
			//scan후 각 위치[시작위치, center, peak] 화면 표시
            updateCoord(uiOrigin1, status.Origin[0]);
			updateCoord(uiCenter1, status.Center[0]);
			updateCoord(uiPeak1, status.Peak[0]);

			if (status.Origin.Count() > 1)
			{
				updateCoord(uiOrigin2, status.Origin[1]);
				updateCoord(uiCenter2, status.Center[1]);
				updateCoord(uiPeak2, status.Peak[1]);
			}
			this.Refresh();
        }

		private void updateReporter(ScanStatus status)
		{
			//scan중 현재 pos 화면 표시
			statusPos1.Text = $"{status.RelativePos[0]}";
			var currPos = $"{status.CurrentPos[0]:F2} ";
			if (status.RelativePos.Count() >= 2)
			{
				statusPos2.Text = $"{status.RelativePos[1]}";
				currPos += $"\t{status.CurrentPos[1]:F2} ";
			}
			if (status.RelativePos.Count() >= 3)
			{
				statusPos3.Text = $"{status.RelativePos[2]}";
				currPos += $"\t{status.CurrentPos[2]:F2}";
			}
			AddLog(currPos);
		}

        public void AddLog(string msg ,bool clear = false)
        {
            if (this.InvokeRequired) this.Invoke((Action)(() => writeLog(msg, clear)));
            else writeLog(msg, clear);
        }

        void writeLog(string msg, bool clear)
        {
			if (clear) txtLog.Clear();
            txtLog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {msg}\n");
            txtLog.Focus();
        }

        #endregion



        #region ==== Init Device ====

        public bool InitState = false;

        private void btnPDInit_Click(object sender, EventArgs e)
        {
            AddLog("초기화_Click");

            if (InitState) return;

            try
            {
                initDevice();		//PD 초기화

                InitState = true;
                btnScan.Enabled = true;
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                AddLog(ex.StackTrace);
            }
        }

        void initDevice()
        {
            var avgTime = (PmAvgTime)cboAvgTime.SelectedItem;//unit=100us

            //PD 초기화
            if (uiDaqOpm.Checked)
            {
                //DAQ PM
                var pd = new DaqPm();

                var daqprimary = (DaqPrimary)cboDaqPrimary.SelectedItem;
                var dicChToAi = new Dictionary<PmCh, DaqAiCh>() { { PmCh.CH1, (DaqAiCh)cboAiCh.SelectedItem } };
                var dicAiToRange = new Dictionary<DaqAiCh, double[]>() { { (DaqAiCh)cboAiCh.SelectedItem, new double[] { 0, 10 } } };
                var res = cboRse.SelectedIndex == 0 ? true : false;

                var responsivity = txtDaqResponsivity.Text.Unpack<double>().ToArray();
                var dicResistance = new Dictionary<DaqAiCh, double>() { { (DaqAiCh)cboAiCh.SelectedItem, txtDaqResistance.Text.To<double>() } };

                pd.SetAddressInfo(daqprimary, dicChToAi, dicAiToRange, res);
                pd.SetDaqSystemInfo(responsivity, dicResistance);
                mPD = pd;
                mPD.WriteUnit(PmCh.CH1, PmPowerUnit.dBm);
                mPD.WriteAvgTime(PmCh.CH1, avgTime);
            }
            else if (uiEsm.Checked)
            {
                //Source Meter PM
                var pd = new Keithley2401();
                var dic = new Dictionary<PmCh, PmSlotPort>() { { PmCh.CH1, new PmSlotPort((DeviceSlot)1, (DevicePort)1) } };

                pd.SetAddressInfo(GpibPrimary.G0, GpibTimeout.T1s, (GpibPrimary)txtPd2Address.Text.To<int>());
                pd.SetChMap(dic);
                pd.Open();

                mPD = pd;
				if (chkPDmA.Checked)  mPD.WriteUnit(PmCh.CH1, PmPowerUnit.mA);
				else mPD.WriteUnit(PmCh.CH1, PmPowerUnit.dBm);

				mPD.WriteAvgTime(PmCh.CH1, avgTime);
            }
			else if (uiAppOpm.Checked)
            {
                mOpm = CGlobal.Pm8164;
                mOpm.SetAvgTime_ms(1, (int)(0.01 * (int)avgTime));
            }
			else if (uiTestMode.Checked)
			{
				mLeft = new TestStage(6);
				mRight = new TestStage(6);
			}
        }

        #endregion



        #region ==== Scan ====
        
		volatile bool mStopUpdate = true;

		private async void btnScan_ClickAsync(object sender, EventArgs e)
		{
			await doScan();
		}

		public async Task doScan()
		{
			var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
			var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
			
			//Scan 시작
			try
			{
				AddLog("Scan_Click", true);
				Cursor = Cursors.WaitCursor;
				if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
				statusAxis1.Text = $"{cbAxis1.SelectedItem}:";
				statusAxis2.Text = $"{cbAxis2.SelectedItem}";
				statusAxis3.Text = $"{cbAxis3.SelectedItem}";

				//scan option
				Func<double> powerReader;
				int digit = (chkPDmA.Checked && uiEsm.Checked) ? 12 : 3;            //소수점 자리수 설정
				if (!uiAppOpm.Checked) powerReader = () => Math.Round(mPD.ReadPower(PmCh.CH1), digit);
				else powerReader = () => Math.Round(mOpm.ReadPowerTrig_dBm(1), 3);

				if (uiTestMode.Checked) powerReader = null;

				var scanParams = new List<ScanParam>();

				//1번째 Scan 설정
				var param1 = new ScanParam
				{
					Aligner = selectedAligner(ScanAxisId.X1),
					Axis = selectedAxis(ScanAxisId.X1),
					Range = txtScanRange1.Text.To<double>(),
					Step = txtScanStep1.Text.To<double>(),
					RangeMode = (RangeMode)cboMoveOption1.SelectedValue,

					DoReturnOgigin = chkReturnOrigin.Checked,
					SaveName = mSaveFileName,
					SaveFolder = btnSaveFolder.Text,
					SaveTime = uiSaveTime.Checked
				};
				
				scanParams.Add(param1);

				//2번째 Scan 설정
				if (chkAxis2.Checked)
				{
					var param2 = new ScanParam
					{
						Aligner = selectedAligner(ScanAxisId.X2),
						Axis = selectedAxis(ScanAxisId.X2),
						Range = txtScanRange2.Text.To<double>(),
						Step = txtScanStep2.Text.To<double>(),
						RangeMode = (RangeMode)cboMoveOption2.SelectedValue
					};
					//2축 scan시 Origin Return 항상 true
					param1.DoReturnOgigin = true;
					chkReturnOrigin.Checked = true;
					chkReturnOrigin.Refresh();

					scanParams.Add(param2);
				}

				//3번째 Scan 설정 [Z축]
				if (chkAxis3.Checked)
				{
					var param3 = new ScanParam
					{
						Aligner = selectedAligner(ScanAxisId.X3),
						Axis = selectedAxis(ScanAxisId.X3),
						Range = txtScanRange3.Text.To<double>(),
						Step = txtScanStep3.Text.To<double>(),
						RangeMode = (RangeMode)cboMoveOption3.SelectedValue
					};

					scanParams.Add(param3);
				}
				
				//시작설정
				ScanLogic.mStop = false;
				mStopUpdate = false;

				var scanAxis = (from a in scanParams select a.AxisName).ToArray();
				var status = new ScanStatus(scanAxis);
				status.Reporter = updateReporter;

				//Run scan******************
				await ScanLogic.runScan(scanParams, powerReader, status);

				//각 좌표(origin, center, peak) 출력
				updateCoord(status);

				//종료 설정
				AddLog("측정 종료.");

				//stage controller폼 각 축의 현재위치 Update
				if (formStageControl != null)
					for (int i = 0; i < scanParams.Count; i++)
						formStageControl.UpdateAxisPos(scanParams[i].Aligner.stageNo, scanParams[i].Axis);

			}
			catch (Exception ex)
			{
				AddLog(ex.Message);
				AddLog(ex.StackTrace);
			}
			finally
			{
				mStopUpdate = true;
				Cursor = Cursors.Default;
				if (frmDigitalPwr != null && !uiTestMode.Checked) frmDigitalPwr.DisplayOn();
			}
		}


		private void btnStop_Click(object sender, EventArgs e)
        {
            //Stop 버튼
            AddLog("Stop_Click");
            ScanLogic.mStop = true;
        }


        private async void uiMove_Click(object sender, EventArgs e)
        {
			//scan 후 position 이동 버튼(Origin, peak, center)
            try
            {
                Cursor = Cursors.WaitCursor;

                var button = (Button)sender;
                AddLog($"{button.Name}_Click");
                var aligner = selectedAligner((sender == uiMoveOrigin1 || sender == uiMovePeak1 || sender == uiMoveCenter1) ? ScanAxisId.X1 : ScanAxisId.X2);
                if (aligner == null) return;

                var axis = selectedAxis((sender == uiMoveOrigin1 || sender == uiMovePeak1 || sender == uiMoveCenter1) ? ScanAxisId.X1 : ScanAxisId.X2);

                var ui = mUiMoveTextBox[(Button)sender];
                var coord = ui.Text.To<double>();

                var dx = (double)((decimal)coord - (decimal)aligner.GetAxisAbsPos(axis));
                coord = await ScanLogic.MoveAs(aligner, axis, dx);
                AddLog($"Current = {coord}");
            }
            catch (Exception ex)
            {
                AddLog(ex.Message);
                AddLog(ex.StackTrace);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        void updateCoord(TextBox ui, double coord)
        {
            ui.Text = coord.ToString("F2");
        }
		
		public void ReturnOrigin(bool check)
		{
			chkReturnOrigin.Checked = check;
		}


		public void UpdateStagePosition()
		{
			var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
			formStageControl?.UpdateAxisPos();
		}

		#endregion



		#region ==== Save ====

		private void btnSaveFolder_Click(object sender, EventArgs e)
        {
            //저장 폴더 설정
            var fd = new FolderBrowserDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                btnSaveFolder.Text = fd.SelectedPath;
                mSet.ScanForm_SaveFolder = fd.SelectedPath;
                mSet.Save();
            }
        }


		public string mSaveFileName = "";

		private void txtSaveName_TextChanged(object sender, EventArgs e)
		{
			mSaveFileName = txtSaveName.Text;
		}
		
		#endregion


	}

}