using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;
using Free302.MyLibrary.Utility;

public partial class uiStageControl : Form
{

	#region ==== constructor/destructor ====

	private Istage mLeftStage;
	private Istage mRightStage;
	private Istage mOthStage;
	private Dictionary<int, Istage> mStage;
	private IDispSensor mSensor;

	public uiStageControl()
	{
		InitializeComponent();
	}

	
	private void uiStageControl_Load(object sender, EventArgs e)
	{
		mLeftStage = CGlobal.LeftAligner;
		mRightStage = CGlobal.RightAligner;
		mOthStage = CGlobal.OtherAligner;

		mStage = new Dictionary<int, Istage>();
		if (mLeftStage != null) mStage.Add(mLeftStage.stageNo, mLeftStage);
		if (mRightStage != null) mStage.Add(mRightStage.stageNo, mRightStage);
		if (mOthStage != null) mStage.Add(mOthStage.stageNo, mOthStage);

		mSensor = CGlobal.Ds2000;

		//setting move button event
		initControlDic();
		initControlEvent();

		//load config
		loadConfig();
		//trackBar 설정
		setTrackBar();
		//현재 Pos 업데이트
		UpdateAxisPos();
	}


	Dictionary<int, Dictionary<int, TextBox>> mPosDic;              //현재 위치 textbox [key : stage No]
	Dictionary<int, Dictionary<int, TextBox>> mDistDic;             //이동거리 설정 textbox [key : stage No]

	private void initControlDic()
	{
		//이동거리 및 현재 위치 textBox 지정
		mPosDic = new Dictionary<int, Dictionary<int, TextBox>>();
		mDistDic = new Dictionary<int, Dictionary<int, TextBox>>();

		if (mLeftStage != null)
		{
			mPosDic.Add(mLeftStage.stageNo, new Dictionary<int, TextBox>() {
						{ mLeftStage.AXIS_X, txtPosLeftX }, { mLeftStage.AXIS_Y, txtPosLeftY }, { mLeftStage.AXIS_Z, txtPosLeftZ },
						{ mLeftStage.AXIS_TX, txtPosLeftTx }, { mLeftStage.AXIS_TY, txtPosLeftTy }, { mLeftStage.AXIS_TZ, txtPosLeftTz } } );
			mDistDic.Add(mLeftStage.stageNo, new Dictionary<int, TextBox>() {
						{ mLeftStage.AXIS_X, txtDistLeftX }, { mLeftStage.AXIS_Y, txtDistLeftY }, { mLeftStage.AXIS_Z, txtDistLeftZ },
						{ mLeftStage.AXIS_TX, txtDistLeftTx }, { mLeftStage.AXIS_TY, txtDistLeftTy }, { mLeftStage.AXIS_TZ, txtDistLeftTz } } );
		}
		if (mRightStage != null)
		{
			mPosDic.Add(mRightStage.stageNo, new Dictionary<int, TextBox>() {
						{ mRightStage.AXIS_X, txtPosRightX }, { mRightStage.AXIS_Y, txtPosRightY }, { mRightStage.AXIS_Z, txtPosRightZ },
						{ mRightStage.AXIS_TX, txtPosRightTx }, { mRightStage.AXIS_TY, txtPosRightTy }, { mRightStage.AXIS_TZ, txtPosRightTz } } );
			mDistDic.Add(mRightStage.stageNo, new Dictionary<int, TextBox>() {
						{ mRightStage.AXIS_X, txtDistRightX }, { mRightStage.AXIS_Y, txtDistRightY }, { mRightStage.AXIS_Z, txtDistRightZ },
						{ mRightStage.AXIS_TX, txtDistRightTx }, { mRightStage.AXIS_TY, txtDistRightTy }, { mRightStage.AXIS_TZ, txtDistRightTz } } );
		}
		if (mOthStage != null)
		{
			mPosDic.Add(mOthStage.stageNo, new Dictionary<int, TextBox>() {
						{ CGlobal.CameraAxis, txtPosCamera}, { CGlobal.CenterAxis, txtPosCenter } } );
			mDistDic.Add(mOthStage.stageNo, new Dictionary<int, TextBox>() {
						{ CGlobal.CameraAxis, txtDistCamera }, { CGlobal.CenterAxis, txtDistCenter } } );
		}

	}


	private void initControlEvent()
	{
		//각 이동버튼 이벤트 등록
		if (mLeftStage != null)
		{
			btnMoveLeftX_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_X, true, s, e);
			btnMoveLeftX_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_X, false, s, e);
			btnMoveLeftY_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_Y, true, s, e);
			btnMoveLeftY_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_Y, false, s, e);
			btnMoveLeftZ_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_Z, true, s, e);
			btnMoveLeftZ_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_Z, false, s, e);
			btnMoveLeftTx_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TX, true, s, e);
			btnMoveLeftTx_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TX, false, s, e);
			btnMoveLeftTy_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TY, true, s, e);
			btnMoveLeftTy_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TY, false, s, e);
			btnMoveLeftTz_p.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TZ, true, s, e);
			btnMoveLeftTz_n.Click += (s, e) => btnMove_Click(mLeftStage, mLeftStage.AXIS_TZ, false, s, e);
			btnZeroLeft.Click += (s, e) => btnZero_Click(mLeftStage, s, e);
			trackBarDistLeft.Scroll += (s, e) => trackBarDist_Scroll(mLeftStage, s, e);
		}

		if (mRightStage != null)
		{
			btnMoveRightX_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_X, true, s, e);
			btnMoveRightX_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_X, false, s, e);
			btnMoveRightY_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_Y, true, s, e);
			btnMoveRightY_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_Y, false, s, e);
			btnMoveRightZ_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_Z, true, s, e);
			btnMoveRightZ_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_Z, false, s, e);
			btnMoveRightTx_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TX, true, s, e);
			btnMoveRightTx_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TX, false, s, e);
			btnMoveRightTy_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TY, true, s, e);
			btnMoveRightTy_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TY, false, s, e);
			btnMoveRightTz_p.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TZ, true, s, e);
			btnMoveRightTz_n.Click += (s, e) => btnMove_Click(mRightStage, mRightStage.AXIS_TZ, false, s, e);
			btnZeroRight.Click += (s, e) => btnZero_Click(mRightStage, s, e);
			trackBarDistRight.Scroll += (s, e) => trackBarDist_Scroll(mRightStage, s, e);
		}

		if (mOthStage != null)
		{
			btnMoveCamera_p.Click += (s, e) => btnMove_Click(mOthStage, CGlobal.CameraAxis, (CGlobal.CameraDirection == 1) ? true : false, s, e);
			btnMoveCamera_n.Click += (s, e) => btnMove_Click(mOthStage, CGlobal.CameraAxis, (CGlobal.CameraDirection == 1) ? false : true, s, e);
			btnMoveCenter_p.Click += (s, e) => btnMove_Click(mOthStage, CGlobal.CenterAxis, (CGlobal.CenterDirection == 1) ? true : false, s, e);
			btnMoveCenter_n.Click += (s, e) => btnMove_Click(mOthStage, CGlobal.CenterAxis, (CGlobal.CenterDirection == 1) ? false : true, s, e);
			btnZeroOther.Click += (s, e) => btnZero_Click(mOthStage, s, e);
		}
		else
		{
			btnMoveCenter_p.Enabled = false;
			btnMoveCenter_n.Enabled = false;
			btnMoveCamera_p.Enabled = false;
			btnMoveCamera_n.Enabled = false;
		}

		btnStop.Click += btnStop_Click;

	}


	private void uiStageControl_FormClosed(object sender, FormClosedEventArgs e)
	{
		saveConfig();
	}


	string mConfigPath = Application.StartupPath + "\\config\\conf_StageControl3stg.xml";

	private void loadConfig()
	{
		//Config값 읽어오기
		try
		{
			var config = new XConfig(mConfigPath);
			//폼 위치
			this.Location = new Point(config.GetValue("WNDPOSX").To<int>(), config.GetValue("WNDPOSY").To<int>());

            //각 축별 이동거리값
            foreach (var aligner in mStage.Keys)
                foreach (var box in mDistDic[aligner].Values) box.Text = config.GetValue(box.Tag.ToString());

            //TrackBar
            txtDistanceOption.Text = config.GetValue("DistTrackBar");

			//Chip protection
			chkEnableCps.Checked = mChipProtect = (config.GetValue("CPS_ENABLE").Contains("1") || config.GetValue("CPS_ENABLE").ToLower().Contains("t")) ? true : false;
			mCpsThres = Math.Round(config.GetValue("CPS_THRES").To<double>(), 3);
			txtCpsThres.Text = mCpsThres.ToString();

            //CAMERA POSITION
            var cp = config.GetValue("CAMERA_POSITIONS", "0;0").Unpack<string>().ToArray();
            txtAbsPosCam1.Text = cp[0];
            txtAbsPosCam2.Text = cp[1];

            config.Save();
		}
		catch
		{
			MessageBox.Show("Reading Config value  Fail...", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);

			foreach (var aligner in mStage.Keys)
				foreach (var box in mDistDic[aligner].Values) box.Text = "0.0";

			chkEnableCps.Checked = mChipProtect = false;
			mCpsThres = 3.0;
			txtCpsThres.Text = mCpsThres.ToString();
		}
	}

	private void saveConfig()
	{
		//config 값 저장
		XConfig config = null;
		try
		{
			config = new XConfig(mConfigPath);
			//폼 위치
			config.SetValue("WNDPOSX", this.Location.X.ToString());
			config.SetValue("WNDPOSY", this.Location.Y.ToString());

			//각 축별 이동거리값
			foreach (var aligner in mStage.Keys)
				foreach (var box in mDistDic[aligner].Values) config.SetValue(box.Tag.ToString(), box.Text);

			//trackBar
			config.SetValue("DistTrackBar", txtDistanceOption.Text);
			
			//Chip protection
			config.SetValue("CPS_ENABLE", (mChipProtect) ? "true" : "false");
			config.SetValue("CPS_THRES", mCpsThres.ToString());

            //CAMERA POSITION
            var cp = new int[] { txtAbsPosCam1.Text.To<int>(), txtAbsPosCam2.Text.To<int>() };
            config.SetValue("CAMERA_POSITIONS", cp.Pack());

            config.Save();
		}
		catch
		{
			if (config != null) config.Dispose();
		}
	}

	#endregion



	#region ==== Move ====

	private async void btnMove_Click(Istage aligner, int axis, bool directionPlus, object sender, EventArgs e)
	{
		//이동버튼 Event
		if (aligner == null || mIsMoving) return;
		var uiDist = mDistDic[aligner.stageNo][axis];
		var uiPos = mPosDic[aligner.stageNo][axis];
		if (uiPos.Enabled == false) return;		//비활성화 축 미실행

        var isOther = aligner.stageNo == mOthStage.stageNo;

        try
		{
			var distance = (uiDist.Text == "") ? 0 : uiDist.Text.To<double>();            //이동거리
			if (!directionPlus) distance *= -1;

			await RunStage(aligner, axis, distance, (x) => displayPos(uiPos, axis, x, isOther));
		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
		}

	}


	bool mStop = false;
	bool mIsMoving = false;
	bool mChipProtect = false;
	double mCpsThres = 0.0;

	async Task RunStage(Istage aligner, int axis, double distance, Action<double> pos)
	{
		//stage 이동★
		var sensorValue = 0.0;
		mStop = false;
		aligner.RelMove(axis, distance);					//이동
		while (true)
		{
			if (!aligner.IsMovingOK(axis)) break;			//이동 중 check
			if (mStop)
			{
				aligner.StopMove(axis);						//Stop
				break;
			}
			if (mChipProtect && axis == aligner.AXIS_Z)		//Chip 보호(Z축 안전장치)
			{
				sensorValue = mSensor.ReadDist((aligner.stageNo == mLeftStage.stageNo) ? SensorID.Left : SensorID.Right);
				if (sensorValue <= mCpsThres)
				{
					aligner.StopMove(axis);                 //Stop
					break;
				}
			}
			mIsMoving = true;
			await Task.Delay(10);
		}

		pos(aligner.GetAxisAbsPos(axis));					//이동 후 좌표 출력
		mIsMoving = false;
	}


	private void btnStop_Click(object sender, EventArgs e)
	{
		//Stop 버튼
		mStop = true;
        mLeftStage?.StopMove();
        mRightStage?.StopMove();
        mOthStage?.StopMove();

        SendKeys.Send("{ESC}");
	}


	private void displayPos(TextBox txtPos, int axis, double pos, bool isOtherStage = false)
	{
		//현재 좌표 표시
		string dpPos = (axis <= mLeftStage.AXIS_Z) ? $"{pos:F01}" : $"{pos:F04}";
        if (isOtherStage) dpPos = $"{pos:F0}";
        if (this.InvokeRequired) this.Invoke((Action)(() => { txtPos.Text = dpPos; txtPos.Refresh(); }));
        else { txtPos.Text = dpPos; txtPos.Refresh(); }
	}

	#endregion

	

	#region === Zeroing ====

	private async void btnZero_Click(Istage aligner, object sender, EventArgs e)
	{
		//이동버튼 Event
		if (aligner == null || mIsMoving) return;
		if (DialogResult.No == MessageBox.Show($"{((Button)sender).Tag.ToString()}를 Zeroing 하시겠습니까?", "Zeroing", MessageBoxButtons.YesNo)) return;

		try
		{
			mIsMoving = true;

			aligner.Zeroing();
			while (true)
			{
				if (!aligner.IsMovingOK()) break;       //이동 중 check
				await Task.Delay(10);
			}
			UpdateAxisPos(aligner.stageNo);             //zero 후 좌표 출력

			await Task.Delay(100);
			aligner.Homing();
			while (true)
			{
				if (!aligner.IsMovingOK()) break;       //이동 중 check
				await Task.Delay(10);
			}
			UpdateAxisPos(aligner.stageNo);             //Home 후 좌표 출력
			MessageBox.Show("Zeroing 완료!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
		}
        finally
        {
            mIsMoving = false;
        }
    }
	
	#endregion



	#region ==== Public [위치 업데이트], [Stop] ===

	public void UpdateAxisPos()
	{
		//모든 Aligner 모든 Axis
		foreach (var aligner in mStage) UpdateAxisPos(aligner.Value.stageNo);
	}

	public void UpdateAxisPos(int stageNo)
	{
		//특정 Aligner 모든 Axis
		if (!mStage.ContainsKey(stageNo)) return;

        var aligner = mStage[stageNo];
        var dic = mPosDic[stageNo];

        foreach (var x in dic.Keys)
		{
			try
			{
                displayPos(dic[x], x, aligner.GetAxisAbsPos(x), stageNo == mOthStage.stageNo);
			}
			catch
			{
				dic[x].Enabled = false;
			}
		}
	}

	public void UpdateAxisPos(int stageNo, int axis)
	{
        if (!mStage.ContainsKey(stageNo)) return;
        var aligner = mStage[stageNo];
        var dic = mPosDic[stageNo];

        //특정 Aligner의 특정 Axis
		try
		{
			displayPos(dic[axis], axis, aligner.GetAxisAbsPos(axis), stageNo == mOthStage.stageNo);
		}
		catch
		{
			dic[axis].Enabled = false;
		}
	} 


	public void StopStages()
	{
		mStop = true;
	}

	#endregion



	#region ==== TrackBar ====

	double[] mTrackBarDist;

	private void btnSetTrackBar_Click(object sender, EventArgs e)
	{
		setTrackBar();
	}


	private void setTrackBar()
	{
		double[] distOption;
		try
		{
			distOption = txtDistanceOption.Text.Unpack<double>().ToArray();
			mTrackBarDist = distOption;
			trackBarDistLeft.Maximum = mTrackBarDist.Length - 1;
			trackBarDistRight.Maximum = mTrackBarDist.Length - 1;
		}
		catch (Exception)
		{
			MessageBox.Show("입력값 확인!");
		}
	}


	private void trackBarDist_Scroll(Istage aligner, object sender, EventArgs e)
	{
		if (mTrackBarDist == null) return;

		var setValue = mTrackBarDist[(aligner.stageNo == mLeftStage.stageNo) ? trackBarDistLeft.Value : trackBarDistRight.Value];
		mDistDic[aligner.stageNo][aligner.AXIS_Z].Text = setValue.ToString();
	}
	
	#endregion



	#region ==== Chip Protection System ====

	private void chkEnableCps_CheckedChanged(object sender, EventArgs e)
	{
		mChipProtect = ((CheckBox)sender).Checked;
	}


	private void txtCpsThres_TextChanged(object sender, EventArgs e)
	{
		mCpsThres = ((TextBox)sender).Text.To<double>();
	}
		
	#endregion



	#region ==== Camera Axis ABS Move ====

	private async void btnAbsMoveCam1_Click(object sender, EventArgs e)
	{
		if (mOthStage == null || mIsMoving || txtAbsPosCam1.Text == "" || txtPosCamera.Enabled == false) return;
		var moveDist = txtAbsPosCam1.Text.To<double>() - txtPosCamera.Text.To<double>();
        
		try
		{
			await RunStage(mOthStage, CGlobal.CameraAxis, moveDist, (x) => displayPos(txtPosCamera, CGlobal.CameraAxis, x, true));
		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
		}
	}
	
	private async void btnAbsMoveCam2_Click(object sender, EventArgs e)
	{
		if (mOthStage == null || mIsMoving || txtAbsPosCam2.Text == "" || txtPosCamera.Enabled == false) return;
		var moveDist = txtAbsPosCam2.Text.To<double>() - txtPosCamera.Text.To<double>();

		try
		{
			await RunStage(mOthStage, CGlobal.CameraAxis, moveDist, (x) => displayPos(txtPosCamera, CGlobal.CameraAxis, x, true));
		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
		}
	}

	
	private void btnSetAbsMove1_Click(object sender, EventArgs e)
	{
		if (mOthStage == null) return;
		txtAbsPosCam1.Text = txtPosCamera.Text;
		btnAbsMoveCam1.Enabled = true;
	}

	private void btnSetAbsMove2_Click(object sender, EventArgs e)
	{
		if (mOthStage == null) return;
		txtAbsPosCam2.Text = txtPosCamera.Text;
		btnAbsMoveCam2.Enabled = true;
	}




    #endregion

}
