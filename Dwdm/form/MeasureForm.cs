using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Jeffsoft;
using Neon.Aligner;
using Free302.MyLibrary.Config;
using Free302.TnMLibrary.DataAnalysis;
using Free302.TnM.DataAnalysis;
using Free302.MyLibrary.Utility;

public partial class MeasureForm : Form, IConfigUser//, IFormCanClosed
{

    #region definition

    private const int RESMW = 9;                        // 10^(-9) mW
    private const int RESDBM = 3;                       // 10^(-3) dBm]

    private const int ALIGN_THRESPOW = -35;             //[dBm] Align 성공여부 Threshold power.

    enum CommandCode { Measure, MeasureSchedule, MoveToInitPosition, MoveToOpenPosition, MoveToClosePosition, Test }

	enum AutoSave { Full, Range }

	private class ThreadParam
    {
        //from ui
        public CommandCode cmd;
        public string[] ChipSNs;

        //from config file
        public int TlsPowerLevel;
        public int TlsSNR;
		public int TlsNoiseShift;
        public int[] PmGainLevels;
        public int[] WaveRange;

        public int ChipWidth;                           //칩 간 간격
        public int ChPitch;                             //output FA corepitch [um]
        public int ZApproachBuffer;                     //Z축 Approach Buffer
        public int mRightAlignInterval;                 //Output FAB Align
        public int YBuffer;                             //Bar 2번 칩 Y축 보정값
        public int AlignTimes;

        public int[] MappedPmPort;
        public bool DutChOrderReverse;
		public bool LoopScan;
		public int LoopStopPort;

        public int NumPmGainLevels;                     //number of gains. 

        public bool doAutoSave;
		public AutoSave SaveType = AutoSave.Full;
		public double[] SaveRange;

		public bool doBackupWave;
		public bool doBackupPower;

        public bool doFineAlign;                        //alignment. <-- uncheck하면 1칩만 측정됨.
        public bool doMeasurement;
		public bool doCladdingMode;
		public int CladDistance = 100;//um
        public bool doFabAngleAlign;
        public bool doRoll;
        public bool doAutoReturn;                       //자동으로 First chip으로 이동한다.
        public string saveFolderPath;

        public bool polLeftCircular;
        public bool polMinus45Diagonal;

        //status info
        public string infoDutSn;
    }



    /// <summary>
    /// 측정된 데이터와 위치정보, 시간정보
    /// </summary>
    private class MeasureInfo
    {
        public DateTime date;                           //측정 시간.
        public AlignPosition alignPosition;             //aligned position.

        public string dutSn;
        public DutData dutData;

        public System.Diagnostics.Stopwatch watch;
        
        public MeasureInfo()
        {
            alignPosition = new AlignPosition();
            watch = new System.Diagnostics.Stopwatch();
        }
    }

    #endregion



    #region member variables

    //private IdistSensor mSensor;
    private Osw mSwitch;
    private Itls mTls;
    private IoptMultimeter mPm;
    private Istage mLeft;
    private Istage mRight;
    private Istage mCenter;
    private IDispSensor mSensor;
    private SweepLogicDwdm mSweep;
    private AlignLogic mAlign;

    bool m_stopFlag;								//정지 신호
    bool m_isRuning;                                //running:true , stop :false//뭐가 러닝?
	public bool m_ScheduleFlag;                     //스케쥴모드 신호

    private ThreadParam mParam;
    private AutoResetEvent mEvent;
    private Thread mThread;

    private AlignPosition mInitPosition;
    private AlignPosition mClosePosition;

    private List<MeasureInfo> mMeasureInfo;
    private CprogRes mTimingStats;

    #endregion




    #region ==== Class Framework ====


    MyLogic mLogic;

    public MeasureForm()
    {
        try
        {
            InitializeComponent();

            //info
            mParam = new ThreadParam();
            mInitPosition = new AlignPosition();
            mClosePosition = new AlignPosition();
            mMeasureInfo = new List<MeasureInfo>();
            mTimingStats = new CprogRes();

            //config
            mConfig = new SelfConfig(sDefaultName, this);
            //mConfig.SaveConfig();
            mConfig.LoadConfig();

            //
            mLogic = MyLogic.Instance;

            //device
            mSwitch = CGlobal.Switch;
            mTls = CGlobal.Tls8164;
            mPm = CGlobal.Pm8164;
            mLeft = CGlobal.LeftAligner;
            mRight = CGlobal.RightAligner;
            mCenter = CGlobal.OtherAligner;
            mSweep = CGlobal.SweepSystem;
            mAlign = CGlobal.Alignment;
            mSensor = CGlobal.Ds2000;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"MeasureForm.MeasureForm():\n{ex.Message}");
        }
    }



    private void Form_Load(object sender, EventArgs e)
    {
        try
        {
			LoadConfig();
			ApplyConfig(mConfig);

			displayParam();

			initGridDut();

            initGraph();

            //check ref loaded
            displayReference();

            mSwitch?.SetToAlign();
            mTls?.SetTlsOutPwr(mParam.TlsPowerLevel);

            //쓰레드 가동.
            mEvent = new AutoResetEvent(false);
            mThread = new Thread(ThreadFunc);
            if (mTls != null && mPm != null) mThread.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Form_Load():\n{ex.Message}");
        }
    }



    void displayParam()
    {
        txtTlsPower.Text = mParam.TlsPowerLevel.ToString();
        txtTlsSNR.Text = mParam.TlsSNR.ToString();

        txtPmGainLevel1.Text = mParam.PmGainLevels[0].ToString();
        txtPmGainLevel2.Text = mParam.PmGainLevels[1].ToString();

        txtWaveStart.Text = mParam.WaveRange[0].ToString();
        txtWaveStop.Text = mParam.WaveRange[1].ToString();
        txtWaveStep.Text = mParam.WaveRange[2].ToString();

		txtSaveWaveStart.Text = txtWaveStart.Text;
		txtSaveWaveStop.Text = txtWaveStop.Text;

        txtChipWidth.Text = mParam.ChipWidth.ToString();
        txtChPitch.Text = mParam.ChPitch.ToString();

        rbtnGain1.Checked = mParam.NumPmGainLevels == 1;
        rbtnGain2.Checked = mParam.NumPmGainLevels == 2;

        displayParam_PmPort();
        displayParam_Positions();

		if (CGlobal.LeftAligner == null && CGlobal.RightAligner == null)
		{
			groupAlignCheck.Enabled = false;
			groupAlignParams.Enabled = false;
			btnOpenStages.Enabled = false;
			btnCloseStages.Enabled = false;
		}

    }



    void displayParam_PmPort()
    {
        //excution
        txtPmPortStart.Items.Clear();
        txtPmPortStop.Items.Clear();

        int numPmPort = mPm.ChList.Length;

        txtPmPortStart.Items.AddRange(mPm.ChList);
        txtPmPortStop.Items.AddRange(mPm.ChList);

        if (numPmPort == 0) txtPmPortStart.SelectedIndex = txtPmPortStop.SelectedIndex = -1;
        else
        {
            if (mPm.ChList.Contains(mParam.MappedPmPort[0])) txtPmPortStart.SelectedItem = mParam.MappedPmPort[0];
            else txtPmPortStart.SelectedIndex = 0;
            if (mPm.ChList.Contains(mParam.MappedPmPort[1])) txtPmPortStop.SelectedItem = mParam.MappedPmPort[1];
            else txtPmPortStop.SelectedIndex = numPmPort - 1;
        }
    }



    void displayParam_Positions()
    {
        //in
        lbInitPosInX.Text = mInitPosition.In.x.ToString();
        lbInitPosInY.Text = mInitPosition.In.y.ToString();
        lbInitPosInZ.Text = mInitPosition.In.z.ToString();
        lbInitPosInTx.Text = mInitPosition.In.tx.ToString();
        lbInitPosInTy.Text = mInitPosition.In.ty.ToString();
        lbInitPosInTz.Text = mInitPosition.In.tz.ToString();

        //out
        lbInitPosOutX.Text = mInitPosition.Out.x.ToString();
        lbInitPosOutY.Text = mInitPosition.Out.y.ToString();
        lbInitPosOutZ.Text = mInitPosition.Out.z.ToString();
        lbInitPosOutTx.Text = mInitPosition.Out.tx.ToString();
        lbInitPosOutTy.Text = mInitPosition.Out.ty.ToString();
        lbInitPosOutTz.Text = mInitPosition.Out.tz.ToString();

        //other.
        lbl_Init_Center_X.Text = mInitPosition.Other.x.ToString();
        lbl_Init_Center_Y.Text = mInitPosition.Other.y.ToString();

        //close
        lbClosePosInZ.Text = mClosePosition.In.z.ToString();
        lbClosePosOutZ.Text = mClosePosition.Out.z.ToString();
    }



    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;

        BuildConfig();
        mConfig.SaveConfig();
        mLogic.SaveConfig();

        Neon.Dwdm.Properties.Settings.Default.Save();

        try
        {
            //thread 종료 및 마무리.
            if (mThread != null)
            {
                mThread.Abort();                
                if (!mThread.ThreadState.HasFlag(ThreadState.Unstarted)) mThread.Join(5000);
                mThread = null;
            }
            if (mEvent != null) mEvent.Dispose();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Form_FormClosing():\n{ex.Message}");
        }
    }



    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }


    #endregion



    #region ==== Config ====


    SelfConfig mConfig;
    const string sDefaultName = "DwdmMeasure";

    #region ---- Key Value Strings ----

    /// <summary>
    /// 클래스 내에서 사용되는 상수 문자열, 키 이름, 키 기본값
    /// </summary>
    static class K
    {
        public const string TlsPowerLevel = "TLS_POWER_LEVEL";
        public const string TlsSNR = "TLS_SNR";
		public const string TlsNoiseShift = "TLS_NOISE_SHIFT";
        public const string PmGainLevels = "PM_GAIN_LEVEL";
        public const string WaveRange_O = "WAVE_RANGE_O";
        public const string WaveRange_C = "WAVE_RANGE_C";

        public const string ChipWidth = "CHIP_WIDTH";
        public const string ChPitch = "CHANNEL_PITCH";

        public const string DutChOrderReverse = "DUT_CH_ORDER_REVERSE";
        public const string MappedPmPort_C = "MAPPED_PM_PORT_C";
        public const string MappedPmPort_O = "MAPPED_PM_PORT_O";
        public const string NumPmGainLevels = "NUM_PM_GAIN_LEVELS";

        public const string AvgProcessTime = "AVERAGE_PROCESS_TIME";

        public const string InitPositionIn = "INIT_POSITION_IN";
        public const string InitPositionOut = "INIT_POSITION_OUT";
        public const string InitPositionOther = "INIT_POSITION_OTHER";

        public const string ClosePosition = "CLOSE_POSITION";
    }

    static class V
    {
        public const int TlsPowerLevel = -15;
        public static int TlsNoiseLevel = -100;
		public static int TlsNoiseW = -3;
        public static int[] PmGainLevels = { -10, -40 };
        public static int[] WaveRange_O = { 1260, 1360, 20 };//{nm, nm, pm}
        public static int[] WaveRange_C = { 1520, 1570, 20 };//{nm, nm, pm}

        public const int ChipWidth = 15500;
        public const int ChPitch = 127;

        public const bool DutChOrderReverse = false;//Pm Ch ~ Dut Ch order
        public static int[] MappedPmPort_C = { 1, 40 };
        public static int[] MappedPmPort_O = { 1, 16 };
        public const int NumPmGainLevels = 2;
        public const double AvgProcessTime = 229.65;

        public static double[] InitPositionIn = { 23442, 3584, 8688, 1.13, 7.52, 8.00 };
        public static double[] InitPositionOut = { 21418, 3230, 6371, 1.13, 8.86, 8.10 };
        public static double[] InitPositionOther = { 32968, 0, 0, 0, 0, 0 };

        public static double[] ClosePosition = { 7596, 4649 };
    }
    #endregion



    public void BuildDefaultConfig(ConfigBase config)
    {
        //TLS & PM
        config.Add(K.TlsPowerLevel, V.TlsPowerLevel);
        config.Add(K.TlsSNR, V.TlsNoiseLevel);
		config.Add(K.TlsNoiseShift, V.TlsNoiseW);
        config.AddList(K.PmGainLevels, V.PmGainLevels);
        config.AddList(K.WaveRange_O, V.WaveRange_O);
        config.AddList(K.WaveRange_C, V.WaveRange_C);

        //DUT
        config.Add(K.ChipWidth, V.ChipWidth);
        config.Add(K.ChPitch, V.ChPitch);

        //Measurement
        config.Add(K.DutChOrderReverse, V.DutChOrderReverse);
        config.AddList(K.MappedPmPort_C, V.MappedPmPort_C);
        config.AddList(K.MappedPmPort_O, V.MappedPmPort_O);
        config.Add(K.NumPmGainLevels, V.NumPmGainLevels);
        config.Add(K.AvgProcessTime, V.AvgProcessTime);

        //Aligner
        config.AddList(K.InitPositionIn, V.InitPositionIn);
        config.AddList(K.InitPositionOut, V.InitPositionOut);
        config.AddList(K.InitPositionOther, V.InitPositionOther);
        config.AddList(K.ClosePosition, V.ClosePosition);
    }



    public ConfigBase BuildConfig()
    {
        //TLS & PM
        mConfig.Set(K.TlsPowerLevel, mParam.TlsPowerLevel);
        mConfig.Set(K.TlsSNR, mParam.TlsSNR);
		mConfig.Set(K.TlsNoiseShift, mParam.TlsNoiseShift);
        mConfig.SetList(K.PmGainLevels, mParam.PmGainLevels);
        //mConfig.SetList(CGlobal.InBandC ? K.WaveRange_C : K.WaveRange_O, mParam.WaveRange);
        mConfig.SetList((CGlobal.mBand == CGlobal.WlBand.CBand) ? K.WaveRange_C : K.WaveRange_O, mParam.WaveRange);
        //DUT
        mConfig.Set(K.ChipWidth, mParam.ChipWidth);
        mConfig.Set(K.ChPitch, mParam.ChPitch);

        //Measurement
        mConfig.Set(K.DutChOrderReverse, mParam.DutChOrderReverse);
        //mConfig.SetList(CGlobal.InBandC ? K.MappedPmPort_C : K.MappedPmPort_O, mParam.MappedPmPort);
        mConfig.SetList((CGlobal.mBand == CGlobal.WlBand.CBand) ? K.MappedPmPort_C : K.MappedPmPort_O, mParam.MappedPmPort);
        mConfig.Set(K.NumPmGainLevels, mParam.NumPmGainLevels);
        mConfig.Set(K.AvgProcessTime, mTimingStats.GetAvgProcTime());

        //Aligner
        mConfig.SetList(K.InitPositionIn, mInitPosition.InValues);
        mConfig.SetList(K.InitPositionOut, mInitPosition.OutValues);
        mConfig.SetList(K.InitPositionOther, mInitPosition.OtherValues);
        mConfig.SetList(K.ClosePosition, mClosePosition.CloseValues);

        return mConfig;
    }



    public void ApplyConfig(ConfigBase config)
    {
        //TLS & PM
        mParam.TlsPowerLevel = mConfig.Get<int>(K.TlsPowerLevel);
        mParam.TlsSNR = mConfig.Get<int>(K.TlsSNR);
		mParam.TlsNoiseShift = mConfig.Get<int>(K.TlsNoiseShift);
        mParam.PmGainLevels = mConfig.GetList<int>(K.PmGainLevels).ToArray();
        //mParam.WaveRange = mConfig.GetList<int>(CGlobal.InBandC ? K.WaveRange_C : K.WaveRange_O).ToArray();
        mParam.WaveRange = mConfig.GetList<int>((CGlobal.mBand == CGlobal.WlBand.CBand) ? K.WaveRange_C : K.WaveRange_O).ToArray();

        //DUT
        mParam.ChipWidth = mConfig.Get<int>(K.ChipWidth);
        mParam.ChPitch = mConfig.Get<int>(K.ChPitch);

        //Measurement
        mParam.DutChOrderReverse = mConfig.Get<bool>(K.DutChOrderReverse);
        //mParam.MappedPmPort = mConfig.GetList<int>(CGlobal.InBandC ? K.MappedPmPort_C : K.MappedPmPort_O).ToArray();
        mParam.MappedPmPort = mConfig.GetList<int>((CGlobal.mBand == CGlobal.WlBand.CBand) ? K.MappedPmPort_C : K.MappedPmPort_O).ToArray();
        mParam.NumPmGainLevels = mConfig.Get<int>(K.NumPmGainLevels);
        mTimingStats.SetAvgProcTime(mConfig.Get<double>(K.AvgProcessTime));

        //Aligner
        mInitPosition.In.SetValue(mConfig.GetList<double>(K.InitPositionIn).ToArray());
        mInitPosition.Out.SetValue(mConfig.GetList<double>(K.InitPositionOut).ToArray());
        mInitPosition.Other.SetValue(mConfig.GetList<double>(K.InitPositionOther).ToArray());
        mClosePosition.SetCloseValue(mConfig.GetList<double>(K.ClosePosition).ToArray());

    }



    public void SaveConfig()
    {
        mConfig.SaveConfig();
    }

    public void LoadConfig()
    {
        mConfig.LoadConfig();
    }


    #endregion




    #region ==== Dut SN & Data Folder ====


    private void btnSaveFolder_Click(object sender, EventArgs e)
    {
        var browser = new FolderBrowserDialog();
		//browser.RootFolder = Environment.SpecialFolder.MyDocuments;
		if (browser.ShowDialog() == DialogResult.OK) btnSaveFolder.Text = browser.SelectedPath;
    }


	private void btnSave_Click(object sender, EventArgs e)
	{
		//측정 데이터 저장 (수동 저장)
		if (mMeasureInfo.Count == 0) return;

		var savePath = btnSaveFolder.Text == "Save folder" ? Application.StartupPath : btnSaveFolder.Text;
		var dir = savePath + @"\" + mMeasureInfo[0].dutSn.Split('-')[0];
		if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

		for (int i = 0; i < mMeasureInfo.Count; i++)
		{
			string filePath = RawTextFile.BuildFileName(dir, mMeasureInfo[i].dutSn);
			if (rbtnAutoSaveFull.Checked) mMeasureInfo[i].dutData.WriteTransmitance(filePath);
			else mMeasureInfo[i].dutData.WriteTransmitance(filePath, txtSaveWaveStart.Text.To<double>(), txtSaveWaveStop.Text.To<double>());
		}

	}
	

	void initGridDut()
    {
        //grid setting.
        uiGridDut.Dock = DockStyle.Fill;
        uiGridDut.ReadOnly = true;
        uiGridDut.MultiSelect = false;
        uiGridDut.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        uiGridDut.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        //uiGridDut.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        uiGridDut.RowHeadersVisible = false;
        uiGridDut.AllowUserToAddRows = false;
        uiGridDut.AllowUserToDeleteRows = false;
        uiGridDut.AllowUserToOrderColumns = false;
        uiGridDut.AllowUserToResizeRows = false;
        uiGridDut.AllowUserToOrderColumns = false;

        //columns
        string[] colNames = { "#", "SN", "Comments", "Status" };
        foreach (string cn in colNames)
        {
            uiGridDut.Columns.Add(cn, cn);
        }
    }


    /// <summary>
    /// align Power 값 출력
    /// </summary>
    private void displayAlignPower(int index, string AlignPower)
    {
        uiGridDut.Rows[index].Cells[3].Value = AlignPower;
    }


    private void btnApplyChipSn_Click(object sender, EventArgs e)
    {
        int numChips;
        if (!int.TryParse(txtNumChips.Text, out numChips))
        {
            MessageBox.Show("칩개수를 정확히 입력해주세요.");
            return;
        }

        //grid clear
        uiGridDut.Rows.Clear();

        //칩 넘버 array를 만든다.!!
        string[] inString = txtFisrtChipNo.Text.ToUpper().Split('-');

		if (numChips == 1)
		{
			//칩개수 1개일경우
			uiGridDut.Rows.Add(new object[] { 1, txtFisrtChipNo.Text, "" });
			return;
		}
		if (inString.Length < 4)
        {
            MessageBox.Show("입력한 SN 이상");
            return;
        }
        string comment = "";

        if (!chkSerial.Checked)
        {
            for (int i = 4; i < inString.Length; i++) comment += inString[i];
            int startChipId = Convert.ToInt32(inString[2]);
            for (int i = 0; i < numChips; i++)
            {
                string sn = $"{inString[0]}-{inString[1]}-{startChipId + i:D1}-{inString[3]}";
                uiGridDut.Rows.Add(new object[] { i + 1, sn, comment });
            } 
        }
        else
        {
            if (inString.Length < 5)
            {
                MessageBox.Show("입력한 SN 이상");
                return;
            }

            string strWfNo = "";
            string strDate = "";
            for (int i = 0; i < inString.Length - 2; i++) strWfNo += inString[i] + "-";
            strWfNo += inString[inString.Length - 2].Substring(0, 1);
            int startchipNo = Convert.ToInt32(inString[inString.Length - 2].Substring(1));
            strDate = inString[inString.Length - 1];

            string strChipNos;
            for (int i = 0; i < numChips; i++)
            {
                strChipNos = strWfNo;
                strChipNos += $"{(startchipNo + i):D2}";
                strChipNos += "-" + strDate;
                strChipNos = strChipNos.ToUpper();
                uiGridDut.Rows.Add(new object[] { i + 1, strChipNos, comment });
            }


        }
    }


	public void SetChipNos(List<string> ChipList)
	{
		if (ChipList.Count == 0) return;

		//grid clear
		uiGridDut.Rows.Clear();
		for (int i = 0; i < ChipList.Count; i++)
		{
			uiGridDut.Rows.Add(new object[] { i + 1, ChipList[i], ""});
		}
		uiGridDut.Refresh();
	}


	private void btnClearChipSn_Click(object sender, EventArgs e)
    {
        uiGridDut.Rows.Clear();
    }



    /// <summary>
    /// 진행 상황에 따라 chip List에서 progress 상태를
    /// 업데이트한다.
    /// </summary>
    private void updateDutGrid()
    {
        try
        {
            if (!m_isRuning)
            {
                foreach (DataGridViewRow r in uiGridDut.Rows) r.DefaultCellStyle.BackColor = SystemColors.Window;
                //원래대로 복원 -> 바탕을 하얀색으로 변경.
                //DataGridViewCell cell = null;
                //for (int i = 0; i < uiGridDut.RowCount; i++)
                //{
                //    for (int j = 0; j < uiGridDut.Rows[i].Cells.Count; j++)
                //    {
                //        cell = uiGridDut.Rows[i].Cells[j];
                //        if (cell.Style.BackColor != Color.White) cell.Style.BackColor = Color.White;
                //    }
                //}
            }
            else
            {
                //----- 작업중 -----//
                //DataGridViewCell cell = null;
                for (int i = 0; i < uiGridDut.RowCount; i++)
                {
                    string sn = uiGridDut.Rows[i].Cells[1].Value.ToString();
                    if (null != mMeasureInfo.Find(p => p.dutSn == sn))
                    {
                        uiGridDut.Rows[i].DefaultCellStyle.BackColor = Color.Gray;
                        //for (int j = 0; j < uiGridDut.Rows[i].Cells.Count; j++)
                        //{
                        //    cell = uiGridDut.Rows[i].Cells[j];
                        //    if (cell.Style.BackColor != Color.Yellow) cell.Style.BackColor = Color.Yellow;
                        //}
                    }
                }
            }//else
        }
        catch
        {
            //do nothing.
        }
    }



    private void txt_ToggleReadOnly(object sender, EventArgs e)
    {
        if (Form.ModifierKeys == Keys.Control) ((TextBox)sender).ReadOnly = !((TextBox)sender).ReadOnly;
    }


    #endregion




    #region ==== Measure : Alignment & Measurement ====


    private void btnMeasure_Click(object sender, EventArgs e)
    {
		
		#region 측정 조건 
		
		//측정 중 확인
		if (m_isRuning == true) return;

		if (mThread == null || mTls == null || mPm == null) return;

		//측정 할 칩 개수 확인
		if (uiGridDut.RowCount < 1)
		{
			MessageBox.Show("측정 할 칩 개수가 0입니다.");
			return;
		}

		//폴더 확인
		if (!checkFolder()) return;

		//Loop Scan 확인
		if (chkLoopScan.Checked && txtPmPortStart.SelectedIndex != 0)
		{
			MessageBox.Show("Loop Scan 측정시, P.M 시작port를 1로 설정!");
			return;
		} 

		#endregion

		try
        {
			//reference 확인
			if (!mLogic.Reference.Loaded) mLogic.LoadReference(false);

			lockControlUi(true);
            mParam.cmd = (CGlobal.TestMode)? CommandCode.Test : CommandCode.Measure;
			if (m_ScheduleFlag) mParam.cmd = CommandCode.MeasureSchedule;

            //TLS
            mParam.TlsPowerLevel = int.Parse(txtTlsPower.Text);
            mParam.TlsSNR = int.Parse(txtTlsSNR.Text);
			mParam.TlsNoiseShift = int.Parse(txtTlsNoiseShift.Text);
            mParam.WaveRange[0] = int.Parse(txtWaveStart.Text);
            mParam.WaveRange[1] = int.Parse(txtWaveStop.Text);
            mParam.WaveRange[2] = int.Parse(txtWaveStep.Text);

            //PM
            mParam.PmGainLevels[0] = int.Parse(txtPmGainLevel1.Text);
            mParam.PmGainLevels[1] = int.Parse(txtPmGainLevel2.Text);
			mParam.LoopScan = chkLoopScan.Checked;
			mParam.LoopStopPort = int.Parse(txtLoopStop.Text);
            mParam.NumPmGainLevels = rbtnGain1.Checked ? 1 : 2;            

            mParam.MappedPmPort[0] = int.Parse(txtPmPortStart.Text);
            mParam.MappedPmPort[1] = int.Parse(txtPmPortStop.Text);
            mParam.DutChOrderReverse = chkChOrderReverse.Checked;

            //DUT 정보
            mParam.ChipWidth = int.Parse(txtChipWidth.Text);        //chip 간격
            mParam.ChPitch = int.Parse(txtChPitch.Text);            //out channel coreptich.

			//scan& save  range
			mParam.saveFolderPath = btnSaveFolder.Text == "Save folder" ? Application.StartupPath : btnSaveFolder.Text;
            mParam.doAutoSave = chkAutoSave.Checked;
			mParam.SaveType = rbtnAutoSaveFull.Checked ? AutoSave.Full : AutoSave.Range;
			mParam.SaveRange = new double[] { txtSaveWaveStart.Text.To<double>(), txtSaveWaveStop.Text.To<double>() };

            //fa Arrangement, alignment , measurment
            mParam.doFabAngleAlign = chkFaArrangement.Checked;
            mParam.doFineAlign = chkAlignment.Checked;
            mParam.doMeasurement = chkMeasurement.Checked;
			mParam.doCladdingMode = uiMeasureCladding.Checked;
			mParam.CladDistance = uiCladDistance.Text.To<int>();
			mParam.doAutoReturn = chkRetChip1Pos.Checked;

            mParam.ZApproachBuffer = int.Parse(txtZBuffer.Text);
            mParam.YBuffer = int.Parse(txtYBuffer.Text);
            mParam.AlignTimes = int.Parse(txtAlignTimes.Text);

            mParam.polLeftCircular = uiPol_Minus45Diagonal.Checked;
            mParam.polMinus45Diagonal = uiPol_Minus45Diagonal.Checked;

            mMoveNextByCenter = chkCenterStage.Checked;

            mParam.mRightAlignInterval = int.Parse(txtChipsPerRightAlign.Text);

            //roll
            mParam.doRoll = chkRoll.Checked;

			//backup
			mParam.doBackupWave = chkBackupWave.Checked;
			mParam.doBackupPower = chkBackupPower.Checked;

            //chip numbers
            mParam.ChipSNs = new string[uiGridDut.RowCount];
            for (int i = 0; i < uiGridDut.RowCount; i++)
            {
                mParam.ChipSNs[i] = (string)(uiGridDut.Rows[i].Cells[1].Value);
            }


            //측정 시작
            mEvent.Set();
            Thread.Sleep(100);

        }
        catch (Exception ex)
        {
            lockControlUi(false);
            MessageBox.Show(ex.ToString());
        }
    }



    void loadRollParam()
    {
        string mConfigFile = Path.Combine(Application.StartupPath, AlignForm.CONFIG_FILE_NAME);
        XConfig mConfig = new XConfig(mConfigFile);
        mAlign.SetRollParam(mConfig.GetValue("ROLLRNG").To<int>(), mConfig.GetValue("ROLLSTEP").To<int>(), mConfig.GetValue("ROLLPOSTCOND").To<double>());
    }



    private void lockControlUi(bool doLock)
    {
        panelParam.Enabled = !doLock;
        panelMeasure.Enabled = !doLock;
    }



    bool checkFolder()
    {
        bool mWaferFolder = chkWaferFolder.Checked;

        var path = btnSaveFolder.Text.Trim();
		if (path == "Save folder") path = Application.StartupPath;
        if (!Directory.Exists(path))
        {
            MessageBox.Show($"저장 폴더 <{path}>가 없습니다.");
            return false;
        }

        mParam.saveFolderPath = path;
        if (mWaferFolder)
        {
            var folder = path;
            try
            {
                var wafer = txtFisrtChipNo.Text.Split('-')[0];
                folder = Path.Combine(path, wafer);
                Directory.CreateDirectory(folder);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 폴더 <{folder}>를 만드는 중 에러가 발생했습니다.\n{ex.Message}");
                return false;
            }
            mParam.saveFolderPath = folder;
        }
        return true;
    }



    private async void ThreadFunc()
    {
        while (true)
        {
            //신호 대기.
            m_isRuning = false;
            mEvent.WaitOne();
            m_isRuning = true;
            m_stopFlag = false;
			m_ScheduleFlag = false;

            try
            {
                //do work.
                switch (mParam.cmd)
                {
                    case CommandCode.Measure: await Run(); break;
					case CommandCode.MeasureSchedule: await Run(true); break;
                    case CommandCode.MoveToInitPosition: moveToInitPos(); break;
                    case CommandCode.MoveToOpenPosition: OpenZStages(); break;
                    case CommandCode.MoveToClosePosition: CloseZStages(); break;
                    case CommandCode.Test: RunTest(); break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"MeasureForm.ThreadFunc():\n{ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                //화면 활성화!!
                Invoke((Action)(() => lockControlUi(false)));
            }
        }//while (true)
    }


    private async void RunTest()
    {
		//무한 반복 측정
        while (true)
        {
            if (m_stopFlag) break;
            await Run();
            await Task.Delay(6000);
        }

    }

		
    private async Task Run(bool scheduleMode = false)
    {
        //Alignment Parameters
        const int bufferDistance = 40;          //[um]    
        const int measureDistance = 10;         //[um]

        frmSourceController frmSourCont = null;
        try
        {

            #region ---- BAR Init ----

            List<double> alignPower = new List<double>();
            int pmPortStart = mParam.MappedPmPort[0];
            int pmPortStop = mParam.MappedPmPort[1];
            int[] pmPorts = new int[pmPortStop - pmPortStart + 1];
            for (int i = 0; i < pmPorts.Length; i++) pmPorts[i] = pmPortStart + i;

            int rollDistance = mParam.ChPitch * ((pmPortStop - pmPortStart) - 1);//*** -2 ???

            //form instance
            var frmDistSens = MyLogic.CreateAndShow<frmDistSensViewer>(true, false);
            var frmDigitalPwr = MyLogic.CreateAndShow<OpmDisplayForm>(true, false);
            var formStageControl = MyLogic.CreateAndShow<uiStageControl>(true, false);
            var frmStatus = MyLogic.CreateAndShow<frmAlignStatus>(true, false);
            frmSourCont = MyLogic.CreateAndShow<frmSourceController>(true, false);

            bool pmDisplayRun = true;
            double alignPowerFirst = 0;
            double alignPowerLast = 0;
            Invoke((Action)(() => Log.Write(mParam.ChipSNs.First(), "DutMeasure_" + DateTime.Now.ToString("yyyy-MM-dd"))));

            List<AlignPosition> alignedPositions = new List<AlignPosition>();
            JeffTimer jTimer = new JeffTimer();

            //저장공간 초기화.
            mMeasureInfo.Clear();

            //progress 초기화.
            mTimingStats.Clear();
            mTimingStats.compeleted = false;
            mTimingStats.totalItemCnt = mParam.ChipSNs.Length;
            mTimingStats.startTime = DateTime.Now;

            //update chip list process state
            Invoke((Action)updateDutGrid);

            //Disable optical source controller
            if (frmSourCont != null) frmSourCont.DeactiveUpdate();

            #endregion


            #region ---- 시작위치=리턴위치, 첫 칩 위치? ----

            alignedPositions.Clear();

            AlignPosition returnPosition = new AlignPosition();
            returnPosition.In = mLeft?.GetAbsPositions();
            returnPosition.Out = mRight?.GetAbsPositions();
            returnPosition.Other = mCenter?.GetAbsPositions();

            #endregion


            //alignment + 측정 + 다음칩 이동
            for (int chipIndex = 0; chipIndex < mParam.ChipSNs.Length; chipIndex++)//for each chip
            {
                #region CHIP 전처리
                //status info
                mParam.infoDutSn = mParam.ChipSNs[chipIndex];

                //statistics
                mTimingStats.curItemNo = mParam.ChipSNs[chipIndex];

                //FA Arrangement.
                if (chipIndex == 0 && mParam.doFabAngleAlign)
                {
                    updateInfoAsync($"Aligning FAB");
                    doFabAngleAlign();
                }

                //칩측정 시간을 알아내기 위해~~ 타이머 시작!!
                jTimer.ResetTimer();
                jTimer.StartTimer();

				#endregion


				#region //Approach
				if (!mParam.doFabAngleAlign && mParam.doFineAlign)
				{
					updateInfoAsync("Z-Approaching In,Out stage");
					if (frmDistSens != null) frmDistSens.StopSensing();
					approachInOut(chipIndex == 0 ? bufferDistance : 0, measureDistance);

					if (m_stopFlag == true) break;
				}
				#endregion


				#region//Alignment 

				bool alignSuccess = true;
                if (mParam.doFineAlign == true)
                {
                    updateInfoAsync($"Aligning X-Y axes");

                    if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();
                     
                    //out stage alignment
                    CGlobal.XySearchParamRight.Run = (chipIndex % mParam.mRightAlignInterval == 0);

					//DUT Align**********
					if (CGlobal.mIsLocalTls) mTls.SetTlsWavelen(mAlign.RollParam.Wave1);
					alignSuccess = mAlign.AlignDut(pmPortStart, pmPortStop, CGlobal.AlignThresholdPower, mParam.doRoll, rollDistance, CGlobal.XySearchParamLeft, CGlobal.XySearchParamRight, null);
                    //Align 반복 실행
                    if (mParam.AlignTimes > 1)
                    {
                        for (int i = 0; i < mParam.AlignTimes; i++)
                        {
                            mAlign.AlignDut(pmPortStart, pmPortStop, CGlobal.AlignThresholdPower, false, rollDistance, CGlobal.XySearchParamLeft, CGlobal.XySearchParamRight, null);
                        }
                    }
                    alignPower.Add(JeffOptics.mW2dBm(mPm.ReadPower(pmPortStart)));

                    alignPowerFirst = Math.Round(JeffOptics.mW2dBm(mPm.ReadPower(pmPortStart)), 3);
                    alignPowerLast = Math.Round(JeffOptics.mW2dBm(mPm.ReadPower(pmPortStop)), 3);

                    string alignSuccessPower = $"{alignPowerFirst},   {alignPowerLast}";
                    //Align Power 화면 표시
                    Invoke((Action)(() => displayAlignPower(chipIndex, alignSuccessPower)));


					if (frmDigitalPwr != null && pmDisplayRun) frmDigitalPwr.DisplayOn();

					if (m_stopFlag == true) break;
                }

                if (alignSuccess == true)
                {
                    updateInfoAsync("Saving aligned position");

                    //position 저장.
                    AlignPosition alignPos = new AlignPosition();
                    alignPos.chipIndex = chipIndex;
                    alignPos.In = mLeft?.GetAbsPositions();
                    alignPos.Out = mRight?.GetAbsPositions();
                    alignPos.Other = mCenter?.GetAbsPositions();
                    alignedPositions.Add(alignPos);

                    //완료 후 복귀 포지션
                    if (chipIndex == 0)
                    {
                        returnPosition.In = (CStageAbsPos )alignPos.In?.Clone();  //clone 필요??
                        returnPosition.Out = (CStageAbsPos )alignPos.Out?.Clone();
                        returnPosition.Other = (CStageAbsPos )alignPos.Other?.Clone();
                    }
                }
                else updateInfoAsync($"Aligning X-Y axes failed");

                formStageControl?.UpdateAxisPos();

				if (alignSuccess)
				{
					try
					{
						//Log 기록 (좌표 & AlignPower)
						string logStr = $"{chipIndex}\t";
						logStr += $"{alignPowerFirst}\t{alignPowerLast}\t";
						logStr += $"{alignedPositions.Last().In.x}\t{alignedPositions.Last().In.y}\t{alignedPositions.Last().In.z}\t";
						logStr += $"{alignedPositions.Last().Out.x}\t{alignedPositions.Last().Out.y}\t{alignedPositions.Last().Out.z}\t";
						logStr += $"{alignedPositions.Last().Other.x}";
						Invoke((Action)(() => Log.Write(logStr, "DutMeasure_" + DateTime.Now.ToString("yyyy-MM-dd"))));
					}
					catch (Exception)
					{
					}
				}

				#endregion


				#region //ScheduleMode - Action Waiting

				if (scheduleMode)
				{
					updateInfoAsync("Waiting Schedule Time...");
					while (true)
					{
						if (m_ScheduleFlag) break;
						if (m_stopFlag) break;
						await Task.Delay(1000);
					}
					m_ScheduleFlag = false;
				}
				if (m_stopFlag) break;

				#endregion


				#region //measurement & saving

				if (alignSuccess)
                {
                    MeasureInfo mInfo = new MeasureInfo();
                    mInfo.dutSn = mParam.ChipSNs[chipIndex];
                    mInfo.date = DateTime.Now;
                    mInfo.alignPosition = alignedPositions.Last();

                    if (mParam.doMeasurement)
					{
						updateInfoAsync("Measuring");

						//display off.
						if (frmDigitalPwr != null) frmDigitalPwr.DisplayOff();

						//measurement
						await doMeasure(pmPorts, mInfo);

						//save power
						updateInfoAsync("Backing-up power data");
						backupPower(mInfo.dutSn, mInfo.dutData);

						//save Loss
						if (mParam.doAutoSave)
						{
							updateInfoAsync("Saving transmitance data");
							var dir = mParam.saveFolderPath + @"\" + mInfo.dutSn.Split('-')[0];
							if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
							string filePath = RawTextFile.BuildFileName(dir, mInfo.dutSn);
							if (mParam.SaveType == AutoSave.Full) mInfo.dutData.WriteTransmitance(filePath);
							else mInfo.dutData.WriteTransmitance(filePath, mParam.SaveRange[0], mParam.SaveRange[1]);
						}

						//display ON
						if (frmDigitalPwr != null && pmDisplayRun) frmDigitalPwr.DisplayOn();

					} //m_tp.measurement
					mMeasureInfo.Add(mInfo);
                }//measure
                #endregion


                #region CHIP 후처리

                //update chip list progres state.
                if (mParam.doMeasurement)
                {
                    updateInfoAsync("Plotting");
                    Invoke((Action)(() => Plot(mParam.ChipSNs[chipIndex])));
                    Invoke((Action)updateDutGrid);
                }
                if (m_stopFlag == true) break;

                //timing stats
                jTimer.StopTimer();
                mTimingStats.SetItemProcTime(jTimer.GetLeadTime().TotalSeconds);

				if (!scheduleMode)	//스케쥴 모드에서는 skip
				{
					//alignment가 uncheck되면 칩 하나만 측정하고 나간다.
					if (!mParam.doFineAlign) break;

					//move to next chip
					if (chipIndex != (mParam.ChipSNs.Length - 1))
					{
						updateInfoAsync("Moving next chip");
						MoveNextChip(alignedPositions, mParam.ChipWidth, chipIndex);
						//frmStageCont?.UpdateAxisPos();
						if (m_stopFlag == true) break;
					} 
				}

                #endregion

            }// for each chip


            #region 완료 후 처리.

            mTimingStats.endTime = DateTime.Now;
            mTimingStats.compeleted = true;

            if (m_stopFlag)
            {
                //stop stage.
                mLeft?.StopMove();
                mRight?.StopMove();

                mTimingStats.msg = "작업이 취소되었습니다";

                if (!CGlobal.TestMode && mLeft != null && mRight != null)
                {
                    string msg = "작업이 취소되었습니다. \n초기 위치로 이동(Yes), 멈춤(No)";
                    DialogResult dialRes = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialRes == DialogResult.Yes) moveTo(returnPosition);
                }
            }
            else
            {
                mTimingStats.msg = "측정 완료!!";
                displayStatusAsync("측정 완료!!");

				//초기 위치로 이동
				if (mLeft != null && mRight != null)
				{
					if (!CGlobal.TestMode && mParam.doAutoReturn) moveTo(returnPosition);
					else if (!CGlobal.TestMode && !mParam.doAutoReturn && mRight != null)
					{
						string msg = "작업이 완료되었습니다. \n초기 위치로 이동(Yes), 멈춤(No)";
						DialogResult dialRes = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
						if (dialRes == DialogResult.Yes) moveTo(returnPosition);
					}
					formStageControl?.UpdateAxisPos(); 
				}

            }
            #endregion

        }
        finally
        {
            //Enable optical source controller
            if (frmSourCont != null) frmSourCont.ActiveUpdate();

            mParam.infoDutSn = null;
        }
    }

	private async Task doMeasure(int[] pmPorts, MeasureInfo mInfo)
	{
		var gains = new int[mParam.NumPmGainLevels];
		var loopCount = mParam.LoopStopPort / mParam.MappedPmPort[1];
		if (mParam.LoopStopPort % mParam.MappedPmPort[1] != 0) loopCount += 1;
		int loopScanDistance = mParam.ChPitch * mParam.MappedPmPort[1];

		Array.Copy(mParam.PmGainLevels, gains, mParam.NumPmGainLevels);

		try
		{
			//TLS로 소스 변경.
			mSwitch?.SetToTls();

			//TLS 서버 등록
			if (!CGlobal.mIsLocalTls) await mSweep.Register(true);

			//core power
			mSweep.DutSN = mInfo.dutSn;
			mSweep.DoWaveBackup = mParam.doBackupWave;
			mSweep.DoPowerBackup = mParam.doBackupPower;

			DutData corePower;
			if (mParam.LoopScan)
			{
				var chPower = new List<DutData>();
				for (int i = 0; i < loopCount; i++)
				{
					chPower.Add(await doSweep(pmPorts, gains, mParam.WaveRange[0], mParam.WaveRange[1], mParam.WaveRange[2], mLogic.Reference));
					if (i < loopCount - 1) mRight?.RelMove(mRight.AXIS_X, loopScanDistance);//이동
				}
				mRight?.RelMove(mRight.AXIS_X, -1 * loopScanDistance * loopCount);//원위치
				corePower = sumSweepData(chPower, mParam.WaveRange[0], mParam.WaveRange[1], mParam.WaveRange[2], mParam.LoopStopPort);
			}
			else corePower = await doSweep(pmPorts, gains, mParam.WaveRange[0], mParam.WaveRange[1], mParam.WaveRange[2], mLogic.Reference);		//normal Sweep

			mSweep.DoWaveBackup = false;
			mSweep.DoPowerBackup = false;

			//Clad 측정
			if (mParam.doCladdingMode)
			{
				DutData cladPower;
				if (mParam.LoopScan)
				{
					var chPower = new List<DutData>();
					for (int i = 0; i < loopCount; i++)
					{
						chPower.Add(await measureClad(pmPorts, gains));
						if (i < loopCount - 1) mRight?.RelMove(mRight.AXIS_X, loopScanDistance);//이동
					}
					mRight?.RelMove(mRight.AXIS_X, -1 * loopScanDistance * loopCount);//원위치
					cladPower = sumSweepData(chPower, mParam.WaveRange[0], mParam.WaveRange[1], mParam.WaveRange[2], mParam.LoopStopPort);

				}else cladPower = await measureClad(pmPorts, gains);
				corePower.Subtract(cladPower);//core -clad
			}

			//reset gain level for align
			mSweep.SetPmGain(pmPorts, mParam.PmGainLevels[0]);

			mInfo.dutData = corePower;
		}
		finally
		{
			if (!CGlobal.mIsLocalTls) await mSweep.Register(false); //TLS 서버 탈퇴
			mSwitch?.SetToAlign();//BLS로 소스 변경.
		}

		//채널 순서 변경
		if (mParam.DutChOrderReverse)
		{
			updateInfoAsync("Reversing ch order");
			mInfo.dutData.ReverseChOrder();
		}
	}

	DutData sumSweepData(List<DutData>chData, int waveStart, int waveStop, double waveStep_nm, int dutLength)
	{
		var allPortPowers = new List<PortPowers>();
		var allPortTrans = new List<PortPowers>();

		int portIndex = 1;
		for (int i = 0; i < chData.Count; i++)
		{
			for (int j = 0; j < chData[i].NumCh; j++)
			{
				allPortPowers.Add(chData[i].mPower[j]);
				allPortPowers[i].Port = portIndex;
				allPortTrans.Add(chData[i].mTrans[j]);
				allPortTrans[i].Port = portIndex;
				portIndex += 1;
				if (portIndex > dutLength) break;
			}
		}
		
		var dutData = new DutData(waveStart, waveStop, waveStep_nm / 1000);
		dutData.mPower = allPortPowers;
		dutData.mTrans = allPortTrans;

		return dutData;

	}


	async Task<DutData> measureClad(int[] pmPorts, int[] pmGains)
	{
		AlignTimer.RecordTime(TimingAction.SweepClad);

		int[] pmGainsClad = new int[1];

		//move X to clad position
		mRight?.RelMove(mRight.AXIS_X, -mParam.CladDistance);
		mRight?.WaitForIdle();

		//set gain level
		if (pmGains.Length >= 2) pmGainsClad[0] = pmGains[1] + 10;//-30; //[dBm]
		else pmGainsClad[0] = pmGains[0];

		var cladPower = await doSweep(pmPorts, pmGainsClad, mParam.WaveRange[0], mParam.WaveRange[1], mParam.WaveRange[2], mLogic.Reference);

		mRight?.RelMove(mRight.AXIS_X, mParam.CladDistance);
		mRight?.WaitForIdle();

		return cladPower;
	}
	
	async Task<DutData> doSweep(int[] ports, int[] gains, int waveStart, int waveStop, double waveStep_pm, ReferenceData refData)
	{
		updateInfoAsync("Running doMeasurement()");

		double waveStep_nm = waveStep_pm / 1000;

		DutData data;
		try
		{
			mSweep.mReporter = updateInfoAsync;
			mSweep.mIsLeftCircular = mParam.polLeftCircular;
			mSweep.mIsNegitiveDiagonal = mParam.polMinus45Diagonal;

            //sweep 
			data = await mSweep.MeasureDut(ports, gains, 
										   waveStart, waveStop, waveStep_nm, 
										   refData, 
										   mParam.TlsPowerLevel, mParam.TlsSNR, mParam.TlsNoiseShift);
		}
		finally { mSweep.mReporter = null; }
		return data;
	}




	const string BackupFolder = "backup";
    const string BackupFileNamePower = "power";
	const string BackupFileNameWave = "wave";
	internal static void backupPower(string dutSn, DutData data)
    {
        try
		{
			string file = buildBackupFilePath(dutSn, BackupFileNamePower);

			data.WritePower(file);
		}
		catch (Exception ex)
        {
            MessageBox.Show($"MeasureForm.backupPower():\\nn{ex.Message}\n\n{ex.StackTrace}");
        }
    }

	private static string buildBackupFilePath(string dutSn, string fileName)
	{
		DateTime d = DateTime.Now;

		string folder = Path.Combine(Application.StartupPath, BackupFolder);

		if (File.Exists(folder)) File.Delete(folder);
		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

		string file = Path.Combine(Application.StartupPath, BackupFolder,
			$"{dutSn}_{fileName}_{d.ToString("yyyy-MM-dd")}_{d.ToString("HH-mm-ss")}.txt");
		return file;
	}

	void displayStatusAsync(string msg)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");
        Invoke((Action)(() => statusLabel.Text = $"[{time}] {msg}"));
    }

    private void updateInfoAsync(string msg, int gain = 0, int pol = 0, int port = 0)
    {
        string message = "";
        if (mParam.infoDutSn != null) message += $"Dut=<{mParam.infoDutSn}>";
        if (gain != 0) message += $", Gain=<{gain}>";
        if (pol != 0) message += $", Pol=<{pol}>";
        if (port != 0) message += $", Port=<{port}>";
        if (msg != null) message += $" : {msg}";
        displayStatusAsync(message);
    }


    /// <summary>
    /// Approach in,out state.
    /// 1.open stage 
    /// 2.approach
    /// 3.open state
    /// </summary>
    /// <param name="beforeDist">approach 전</param>
    /// <param name="afterDist">approach 완료 후 (FA와 칩간의 거리) </param>
    /// <returns></returns>
    private bool approachInOut(int bufferDistance, int distance)
    {
        bool ret = false;

        bufferDistance = Math.Abs(bufferDistance) * (-1);
        distance = Math.Abs(distance) * (-1);

        try
        {
            return mAlign.ApproachInOut(bufferDistance, distance);
        }
        catch
        {
            ret = false;
        }

        return ret;
    }



    /// <summary>
    /// Fa를 칩에 맞춘다. ty만 진행, tx는 진행하지 않는다.
    /// </summary>
    private void doFabAngleAlign()
    {
        const int STAGEOPENDIST = 50;   //[um]
        const int ALIGNDIST = 10;       //[um]

        try
        {
            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (m_stopFlag == true) throw new ApplicationException();

            //input approach.
            mAlign.ZappSingleStage(mLeft.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //output approach.
            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //input ty
            mAlign.AngleTy(mLeft.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //output ty
            mAlign.AngleTy(mRight.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (m_stopFlag == true) throw new ApplicationException();

            //input approach.
            mAlign.ZappSingleStage(mLeft.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //output approach.
            mAlign.ZappSingleStage(mRight.stageNo);
            if (m_stopFlag == true) throw new ApplicationException();

            //move to align-distance.
            mLeft.RelMove(mLeft.AXIS_Z, ALIGNDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, ALIGNDIST * (-1));
            mRight.WaitForIdle();
        }
        catch
        {
            //do nothing.
        }


    }



    /// <summary>
    /// alignment 실행.
    /// 1.input 
    /// 2.output
    /// 3.roll
    /// </summary>
    /// <param name="port1">port for channel 1</param>
    /// <param name="port2">port for channel last</param>
    /// <param name="_thresPowr">Alignment됬다고 보는 광파워. [dBm]</param>
    /// <param name="_inAlign">in align할지 말지?</param>
    /// <param name="_outAlign">out align할지 말지?</param>
    /// <param name="_outRoll">outroll 할지 말지?</param>
    /// <param name="_outRollDist">out roll distance [um]</param>
    /// <returns>광을 못찾거나 취소하면 false.</returns>
    private bool doAlignDut(int _port1, int _port2, int _thresPowr, bool _inAlign, bool _outAlign, bool _outRoll, int _outRollDist)
    {
        const double XYSEARCHSTEP = 1;//[um]
        const int SYNCSEARCHRNG = 100; //[um]
        const double SYNCSEARCHSTEP = 4;//[um]
        bool ret = false;

        try
        {
            double temp = -100;
            bool alignSuccess = false;

            //BLS로 소스 변경.
            mSwitch?.SetToAlign();

            //Semi-Align 상태인가? (= Align가능한가?)
            temp = mPm.ReadPower(_port1);
            temp = JeffOptics.mW2dBm(temp);
            temp = Math.Round(temp, RESDBM);
            if (temp >= _thresPowr) alignSuccess = true;
            else alignSuccess = false;

            //Sync Search 시도.(광을 찾은 상태가 아니면 )
            if (alignSuccess == false)
            {
                mAlign.SyncXySearch(_port1, SYNCSEARCHRNG, SYNCSEARCHSTEP, _thresPowr); 
                if (m_stopFlag == true) throw new ApplicationException();

                temp = mPm.ReadPower(_port1);
                temp = JeffOptics.mW2dBm(temp);
                temp = Math.Round(temp, RESDBM);
                if (temp < _thresPowr) throw new ApplicationException();

                //fine search out 
                mAlign.XySearch(mRight.stageNo, _port1, XYSEARCHSTEP);
                if (m_stopFlag == true) throw new ApplicationException();
            }

            //search input 
            if (_inAlign == true)
            {
                mAlign.XySearch(mLeft.stageNo, _port1, XYSEARCHSTEP);
                if (m_stopFlag == true) throw new ApplicationException();
            }

            //fine search out 
            if (_outAlign == true)
            {
                mAlign.XySearch(mRight.stageNo, _port1, XYSEARCHSTEP);
                if (m_stopFlag == true) throw new ApplicationException();
            }

            //roll alignment out 
            if (_outRoll == true)
            {
                int range = this.mAlign.RollParam.Range;
                int step = this.mAlign.RollParam.Step;
                double threshold = this.mAlign.RollParam.Threshold;

                if (!mAlign.RollParam.UsingLocalTls)
                {
                    //Align Source
                    mAlign.RollOut(_port1, _port2, _outRollDist, range, step, threshold);
                }
                else
                {
                    double wave1 = mAlign.RollParam.Wave1;
                    double wave2 = mAlign.RollParam.Wave2;
                    //TLS
                    mAlign.RollOut(_port1, _port2, _outRollDist, mTls, wave1, wave2, range, step, threshold);
                }

                if (m_stopFlag == true) throw new ApplicationException();

                //roll 이후 fine search out 
                if (_outAlign == true)
                {
                    mAlign.XySearch(mRight.stageNo, _port1, XYSEARCHSTEP);
                    if (m_stopFlag == true) throw new ApplicationException();
                }

                if (m_stopFlag == true) throw new ApplicationException();
            }
            ret = true;
        }
        catch
        {
            ret = false;
        }

        return ret;
    }




    bool mMoveNextByCenter = false;

    /// <summary>
    /// 다음칩으로 이동한다.
    /// lsm을 이용 1차 함수 parameter를 구하고
    /// 이를 이용하여 다음칩 위치를 추정하고 스테이지를 그 위치로 이동시킨다.
    /// </summary>
    /// <param name="_posList">aligned postion array</param>
    /// <param name="_chipWdith">chip width</param>
    /// <param name="_curIdx">현재 칩 index</param>
    void MoveNextChip(List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {
        AlignTimer.RecordTime(TimingAction.MoveNext);

        int bufferDistance = mParam.ZApproachBuffer;            //Buffer 거리 설정(ko 170816)
        
        if (mMoveNextByCenter) moveNextChip_Center(bufferDistance, _posList, _chipWdith, _curIdx);
        else moveNextChip_LeftRight(bufferDistance, _posList, _chipWdith, _curIdx);

    }



    /// <summary>
    /// 다음칩으로 이동한다.
    /// lsm을 이용 1차 함수 parameter를 구하고
    /// 이를 이용하여 다음칩 위치를 추정하고 스테이지를 그 위치로 이동시킨다.
    /// </summary>
    /// <param name="_posList">aligned postion array</param>
    /// <param name="_chipWdith">chip width</param>
    /// <param name="_curIdx">현재 칩 index</param>
    private void moveNextChip_Center(int bufferDistance, List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {

        Func<CStageAbsPos , double> centerFunc = (pos) => CGlobal.CenterAxis == mCenter.AXIS_X ? pos.x : pos.y;

        try
        {
            //lsm를 이용하여 1차함수 parameter를 구한다.
            //y = ax+b
            //input 쪽 좌표가 기준.
            double a1 = 0.0;//input y축 기울기 
            double a2 = 0.0;//output y축 기울기 
            double b1 = 0.0;//input y축 절편.
            double b2 = 0.0;//output의 y축 절편    

            int posCnt = _posList.Count();
            if (posCnt < 2)
            {
                //--default--//
                a1 = 0.0;
                a2 = 0.0;
                b1 = _posList.Last().In.y + mParam.YBuffer;
                b2 = _posList.Last().Out.y + mParam.YBuffer;
            }
            else
            {
                //y축
                double[] xPoss = new double[posCnt];
                double[] yPoss = new double[posCnt];
                for (int i = 0; i < _posList.Count(); i++)  //input.
                {
                    xPoss[i] = centerFunc(_posList[i].Other);
                    yPoss[i] = _posList[i].In.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a1, ref b1);

                for (int i = 0; i < _posList.Count(); i++)  //output.
                {
                    xPoss[i] = centerFunc(_posList[i].Other);
                    yPoss[i] = _posList[i].Out.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a2, ref b2);
            }

            //next chip 위치 계산. 
            double nextCenter = 0.0;
            double nextInY = 0.0;
            double nextOutY = 0.0;

            double nextInZ = 0.0;
            double nextOutZ = 0.0;


            if (posCnt < 2)
            {
                //x _ center
                nextCenter = (int)(centerFunc(_posList[0].Other) - (_chipWdith * (_curIdx + 1)));

                //y
                nextInY = (int)(a1 * nextCenter + b1);
                nextOutY = (int)(a2 * nextCenter + b2);

                //z
                nextInZ = _posList.Last().In.z - bufferDistance;
                nextOutZ = _posList.Last().Out.z - bufferDistance;
            }
            else
            {

                int preChipIdx = _posList[posCnt - 2].chipIndex;
                int lastChipIdx = _posList[posCnt - 1].chipIndex;
                int dx = (int)(centerFunc(_posList[posCnt - 1].Other) - centerFunc(_posList[posCnt - 2].Other));
                dx = (int)(dx / (lastChipIdx - preChipIdx));
                dx = Math.Abs(dx);

                //x _ ctr
                nextCenter = centerFunc(_posList[posCnt - 1].Other) - (dx * (_curIdx - lastChipIdx + 1));

                //y
                nextInY = (int)(a1 * nextCenter + b1);
                nextOutY = (int)(a2 * nextCenter + b2);

                //z (170307 ko)
                double[] diffInZ = new double[posCnt - 1];
                double[] diffOutZ = new double[posCnt - 1];
                int marginInZ, marginOutZ;
                for (int i = 0; i < posCnt - 1; i++)
                {
                    diffInZ[i] = _posList[i].In.z - _posList[i + 1].In.z;
                    diffOutZ[i] = _posList[i].Out.z - _posList[i + 1].Out.z;
                }
                marginInZ = bufferDistance + (int)diffInZ.Average();
                marginOutZ = bufferDistance + (int)diffOutZ.Average();

                nextInZ = _posList.Last().In.z - marginInZ;
                nextOutZ = _posList.Last().Out.z - marginOutZ;
            }

            //z축
            mLeft.AbsMove(mLeft.AXIS_Z, nextInZ);
            mRight.AbsMove(mRight.AXIS_Z, nextOutZ);

            //X축 이동 - center
            mCenter.AbsMove(CGlobal.CenterAxis, nextCenter);            

            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, nextInY);
            mRight.AbsMove(mRight.AXIS_Y, nextOutY);

            //완료 대기.
            mCenter.WaitForIdle(CGlobal.CenterAxis);            
        }
        catch
        {
            //do nothing
        }
    }



    private void moveNextChip_LeftRight(int bufferDistance, List<AlignPosition> _posList, int _chipWdith, int _curIdx)
    {
        try
        {
            //lsm를 이용하여 1차함수 parameter를 구한다.
            //y = ax+b
            //input 쪽 좌표가 기준.
            double a1 = 0.0;//input y축 기울기 
            double a2 = 0.0;//output y축 기울기 
            double b1 = 0.0;//input y축 절편.
            double b2 = 0.0;//output의 y축 절편    

            int posCnt = _posList.Count();
            if (posCnt < 2)
            {
                //--default--//
                a1 = 0.0;
                a2 = 0.0;
                b1 = _posList.Last().In.y;
                b2 = _posList.Last().Out.y;
            }
            else
            {
                //y축
                double[] xPoss = new double[posCnt];
                double[] yPoss = new double[posCnt];
                for (int i = 0; i < _posList.Count(); i++)  //input.
                {
                    xPoss[i] = _posList[i].In.x;
                    yPoss[i] = _posList[i].In.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a1, ref b1);

                for (int i = 0; i < _posList.Count(); i++)  //output.
                {
                    xPoss[i] = _posList[i].Out.x;
                    yPoss[i] = _posList[i].Out.y;
                }
                JeffMath.lsm_LinearFunc(xPoss, yPoss, posCnt, 0, ref a2, ref b2);
            }

            //next chip 위치 계산. 
            double nextPosInX = 0.0;
            double nextPosInY = 0.0;
            double nextPosOutX = 0.0;
            double nextPosOutY = 0.0;
            double nextPosInZ = 0.0;
            double nextPosOutZ = 0.0;

            if (posCnt < 2)
            {
                //x
                nextPosInX = (int)(_posList[0].In.x + (_chipWdith * (_curIdx + 1)));
                nextPosOutX = (int)(_posList[0].Out.x + (_chipWdith * (_curIdx + 1)));

                //y
                nextPosInY = (int)(a1 * nextPosInX + b1);
                nextPosOutY = (int)(a2 * nextPosOutX + b2);

                //z
                nextPosInZ = _posList.Last().In.z - bufferDistance;
                nextPosOutZ = _posList.Last().Out.z - bufferDistance;
            }
            else
            {
                int preChipIdx = _posList[posCnt - 2].chipIndex;
                int lastChipIdx = _posList[posCnt - 1].chipIndex;
                int dx = (int)(_posList[posCnt - 1].In.x - _posList[posCnt - 2].In.x);
                dx = (int)(dx / (lastChipIdx - preChipIdx));

                //x
                nextPosInX = _posList[posCnt - 1].In.x + (dx * (_curIdx - lastChipIdx + 1));
                nextPosOutX = (int)(_posList[posCnt - 1].Out.x + (dx * (_curIdx - lastChipIdx + 1)));

                //y
                nextPosInY = (int)(a1 * nextPosInX + b1);
                nextPosOutY = (int)(a2 * nextPosOutX + b2);

                //z (170307 ko)
                double[] diffInZ = new double[posCnt - 1];
                double[] diffOutZ = new double[posCnt - 1];
                int marginInZ, marginOutZ;
                for (int i = 0; i < posCnt - 1; i++)
                {
                    diffInZ[i] = _posList[i].In.z - _posList[i + 1].In.z;
                    diffOutZ[i] = _posList[i].Out.z - _posList[i + 1].Out.z;
                }
                marginInZ = bufferDistance + (int)diffInZ.Average();
                marginOutZ = bufferDistance + (int)diffOutZ.Average();

                nextPosInZ = _posList.Last().In.z - marginInZ;
                nextPosOutZ = _posList.Last().Out.z - marginOutZ;
            }

            //Z축 이동
            mLeft.AbsMove(mLeft.AXIS_Z, nextPosInZ);
            mRight.AbsMove(mRight.AXIS_Z, nextPosOutZ);

            //X축 이동
            mLeft.AbsMove(mLeft.AXIS_X, nextPosInX);
            mRight.AbsMove(mRight.AXIS_X, nextPosOutX);

            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, nextPosInY);
            mRight.AbsMove(mRight.AXIS_Y, nextPosOutY);

            mRight.WaitForIdle(mRight.AXIS_Z);
        }
        catch
        {
            //do nothing
        }
    }




    private void btnStop_Click(object sender, EventArgs e)
    {
        m_stopFlag = true;
    }

    #endregion



    #region ==== Graph ====

    Dictionary<int, Color> mColor;
    NationalInstruments.UI.WaveformPlot mBasePlot;//base plot

    void initColor()
    {
        mColor = new Dictionary<int, Color>();
        mColor.Add(0, Color.Tomato);
        mColor.Add(1, Color.Gold);
        mColor.Add(2, Color.LimeGreen);
        mColor.Add(3, Color.DeepSkyBlue);

        mColor.Add(4, Color.Red);
        mColor.Add(5, Color.Orange);
        mColor.Add(6, Color.Green);
        mColor.Add(7, Color.Blue);
        //mColor.Add(7, Color.Violet);
    }

    void initGraph()
    {
        initColor();
        mBasePlot = new NationalInstruments.UI.WaveformPlot();
        mBasePlot.XAxis = mGraph.Plots[0].XAxis;
        mBasePlot.YAxis = mGraph.Plots[0].YAxis;

        uiGridDut.SelectionChanged += uiGridDut_SelectionChanged;
    }

    private void uiGridDut_SelectionChanged(object sender, EventArgs e)
    {
        if (uiGridDut.CurrentRow == null) return;
        if (uiGridDut.CurrentRow.Index < 0) return;

        Plot(uiGridDut.CurrentRow.Cells[1].Value.ToString());
    }

    void updateGraph(MeasureInfo info)
    {
        //color
        Color[] colors = mColor.Values.ToArray();

        //plot..
        mBasePlot.DefaultStart = info.dutData.mWaveStart;
        mBasePlot.DefaultIncrement = info.dutData.mWaveStep;
        int numCh = info.dutData.NumCh;
        int[] ports = info.dutData.Channels;
        for (int i = 0; i < numCh; i++)
        {
            //channel data.
            PortPowers portData = info.dutData.GetPolLossOf(ports[i]);

            int numColorIndex = colors.Length / 2;

            //plot max trans.
            var plot = (NationalInstruments.UI.WaveformPlot)mBasePlot.Clone();
            plot.XAxis = mBasePlot.XAxis;
            plot.YAxis = mBasePlot.YAxis;
            plot.LineColor = colors[i % numColorIndex];

            plot.PlotY(portData.Min.ToArray());
            mGraph.Plots.Add(plot);

            //plot min trans
            plot = (NationalInstruments.UI.WaveformPlot)mBasePlot.Clone();
            plot.LineColor = colors[i % numColorIndex + numColorIndex];
            plot.XAxis = mBasePlot.XAxis;
            plot.YAxis = mBasePlot.YAxis;


			if(CGlobal.mUsingOpc) plot.PlotY(portData.Max.ToArray());
			else plot.PlotY(portData.NonPol.ToArray());
			mGraph.Plots.Add(plot);
        }
        mGraph.Refresh();
    }

    
    private void Plot(string dutSn)
    {
        try
        {
            lbChipNo.Text = dutSn;
            mGraph.Plots.Clear();

            //find chip data.
            MeasureInfo info = mMeasureInfo.Find(p => p.dutSn == dutSn);
            if (info == null)
            {
                lbChipNo.Text = $"{dutSn}: 데이터가 없습니다.";
                return;
            }
            if (info.dutData == null) return;
            updateGraph(info);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"MeasureForm.Plot():\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    #endregion




    #region ==== Stage Position & Init, In, Out ====
    

    /// <summary>
    /// 주어진 위치로 모든 스테이지를 이동한다.
    /// </summary>
    private void moveTo(AlignPosition position)
    {
        const int zOpenDistance = 10000;
        if (mMeasureInfo.Count() == 0) return;

        CStageAbsPos  pIn = position.In;
        CStageAbsPos  pOut = position.Out;
        CStageAbsPos  pOther = position.Other;

        try
        {
            //Z축 이동
            mLeft.AbsMove(mLeft.AXIS_Z, pIn.z - zOpenDistance);
            mRight.AbsMove(mRight.AXIS_Z, pOut.z - zOpenDistance);
            mRight.WaitForIdle(mRight.AXIS_Z);

            //Y축 이동
            mLeft.AbsMove(mLeft.AXIS_Y, pIn.y);
            mRight.AbsMove(mRight.AXIS_Y, pOut.y);

            //tz 이동.
            mRight.AbsMove(mRight.AXIS_TZ, pOut.tz);

            //X축 이동
            mLeft.AbsMove(mLeft.AXIS_X, pIn.x);
            mRight.AbsMove(mRight.AXIS_X, pOut.x);
            mRight.WaitForIdle(mRight.AXIS_Z);

            //center stage.
            if (mMoveNextByCenter)
            {
                var pos = CGlobal.CenterAxis == mCenter.AXIS_X ? pOther.x : pOther.y;
                mCenter.AbsMove(CGlobal.CenterAxis, pos);
                mCenter.WaitForIdle(CGlobal.CenterAxis);
            }
        }
        catch
        {
            //do nothing
        }
    }


    /// <summary>
    /// 저장된 초기위치로 이동한다.
    /// </summary>
    private void moveToInitPos()
    {
        //buffer
        mLeft.RelMove(mLeft.AXIS_Z, -1000);
        mLeft.RelMove(mRight.AXIS_Z, -1000);

        if (mInitPosition.In.IsValid)
        {
            mLeft.AbsMove(mLeft.AXIS_X, mInitPosition.In.x);
            mLeft.AbsMove(mLeft.AXIS_Y, mInitPosition.In.y);
            mLeft.AbsMove(mLeft.AXIS_TX, mInitPosition.In.tx);
            mLeft.AbsMove(mLeft.AXIS_TY, mInitPosition.In.ty);
            mLeft.AbsMove(mLeft.AXIS_TZ, mInitPosition.In.tz);
            mLeft.AbsMove(mLeft.AXIS_Z, mInitPosition.In.z);
        }

        if (mInitPosition.Out.IsValid)
        {
            mRight.AbsMove(mRight.AXIS_X, mInitPosition.Out.x);
            mRight.AbsMove(mRight.AXIS_Y, mInitPosition.Out.y);
            mRight.AbsMove(mRight.AXIS_TX, mInitPosition.Out.tx);
            mRight.AbsMove(mRight.AXIS_TY, mInitPosition.Out.ty);
            mRight.AbsMove(mRight.AXIS_TZ, mInitPosition.Out.tz);
            mRight.AbsMove(mRight.AXIS_Z, mInitPosition.Out.z);
        }
        if (mInitPosition.Other.IsValid)
        {
            mCenter.AbsMove(CGlobal.CenterAxis, CGlobal.CenterAxis);// mInitPosition.Other.x);
        }

        while (mCenter.IsMovingOK(CGlobal.CenterAxis))
        {
            if (m_stopFlag == true)
            {
                mLeft.StopMove();
                mRight.StopMove();
                break;
            }
        }
    }


    /// <summary>
    /// Stage들을 연다.
    /// </summary>
    private void OpenZStages()
    {
        const int OPENDIST = 15000;
        try
        {
            //이동.
            mLeft.AbsMove(mLeft.AXIS_Z, (mClosePosition.In.z - OPENDIST));
            mRight.AbsMove(mRight.AXIS_Z, (mClosePosition.Out.z - OPENDIST));
            while (mRight.IsMovingOK())
            {
                if (m_stopFlag == true)
                {
                    mLeft.StopMove();
                    mRight.StopMove();
                    break;
                }
            }//while
        }
        catch
        {
            //do nothing.
        }
    }


    /// <summary>
    /// Stage들을 닫는다.
    /// </summary>
    private void CloseZStages()
    {
        const double DISTSENSTHRES = 3.0;

        try
        {
            //이동.
            mLeft.AbsMove(mLeft.AXIS_Z, mClosePosition.In.z);
            mRight.AbsMove(mRight.AXIS_Z, mClosePosition.Out.z);

            while (mRight.IsMovingOK())
            {
                double distSens1 = mSensor.ReadDist(SensorID.Left);
                double distSens2 = mSensor.ReadDist(SensorID.Right);
                if ((distSens1 <= DISTSENSTHRES) || (distSens2 <= DISTSENSTHRES))
                {
                    mLeft.StopMove();
                    mRight.StopMove();
                    break;
                }

                if (m_stopFlag == true)
                {
                    mLeft.StopMove();
                    mRight.StopMove();
                    break;
                }
            }
        }
        catch
        {
        }
    }



    /// <summary>
    /// 현재 위치를 초기위치로 설정.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnInitPosApply_Click(object sender, EventArgs e)
    {
        string msg = "현재 스테이지 포지션을 초기위치로 설정하시겠습니까?";
        DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (res == DialogResult.No) return;

        //설정.
        if (mLeft != null) mInitPosition.In = mLeft.GetAbsPositions();
        if (mRight != null) mInitPosition.Out = mRight.GetAbsPositions();
        if (mCenter != null) mInitPosition.Other = mCenter.GetAbsPositions();

        //저장.
        BuildConfig();

        //출력 
        displayParam_Positions();
        MessageBox.Show("설정 완료!!", "확인");
    }
    
    private void btnInitPosGo_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;

        if (mLeft == null || mRight == null) return;


        //confirm.
        string msg = "스테이지를 초기 위치로 이동하시겠습니까?";
        DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
        if (res == DialogResult.Cancel) return;
        
        //execution.
        try
        {
            lockControlUi(true);
            //Go
            mParam.cmd = CommandCode.MoveToInitPosition;
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            lockControlUi(false);
        }


    }
    
    private void btnClosedPosApply_Click(object sender, EventArgs e)
    {
        string msg = "현재 스테이지 포지션을 Close위치로 설정하시겠습니까?";
        DialogResult res = MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo,  MessageBoxIcon.Question);
        if (res == DialogResult.No) return;

        //설정.
        mClosePosition.In.z = mLeft.GetAxisAbsPos(mLeft.AXIS_Z);
        mClosePosition.Out.z = mRight.GetAxisAbsPos(mRight.AXIS_Z);

        //저장.
        BuildConfig();

        //출력 
        displayParam_Positions();
        MessageBox.Show("설정 완료!!", "확인");
    }

    private void btnClosedPosGo_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;

        //confirm.
        string msg = "스테이지를 close 위치로 이동하시겠습니까?";
        if (DialogResult.Cancel == MessageBox.Show(msg, "확인", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) return;

        //포지션이 설정되지 않았으면 이동하지 않는다.
        if ((mClosePosition.In.z == 0) && (mClosePosition.Out.z == 0))
        {
            MessageBox.Show("위치값이 설정되지 않았습니다");
            return;
        }
        //execution.
        try
        {
            lockControlUi(true);
            //Go
            mParam.cmd = CommandCode.MoveToClosePosition;
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            lockControlUi(false);
        }
    }
    
    private void btnCloseStages_Click(object sender, EventArgs e)
    {
        if (m_isRuning == true) return;

        //execution.
        try
        {
            lockControlUi(true);
            mParam.cmd = CommandCode.MoveToClosePosition;
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            lockControlUi(false);
        }
    }

    private void btnOpenStages_Click(object sender, EventArgs e)
    {
        if (m_isRuning) return;

        //execution.
        try
        {
            lockControlUi(true);
            mParam.cmd = CommandCode.MoveToOpenPosition;
            mEvent.Set();
            Thread.Sleep(10);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            lockControlUi(false);
        }
    }


    #endregion



    private void uiLoadReference_Click(object sender, EventArgs e)
    {
        try
        {
            mLogic.LoadReference(true);
            displayReference();

            MyLogic.Instance.SaveConfig();
        }
        catch (Exception ex)
        { MessageBox.Show(ex.Message); }
    }

    string mReferenceButtonText = "Reference File : {0} ({1} {2})";

    void displayReference()
    {
        if (mLogic.Reference == null) return;

        if (!mLogic.Reference.Loaded) return;

        string fileName = mLogic.Reference.TextFile;
        DateTime time = File.GetLastWriteTime(fileName);
        uiLoadReference.Text = string.Format(mReferenceButtonText, Path.GetFileName(fileName), time.ToShortDateString(), time.ToString("HH:mm:ss"));

    }

    private void uiPolLeft_CheckedChanged(object sender, EventArgs e)
    {
        var ui = (CheckBox)sender;
        if (ui.Tag != null && (bool)ui.Tag) return;

        uiPol_LeftCircular.Tag = true;
        //uiPol_Minus45Diagonal.Tag = true;

        try
        {
            //if (ui == uiPol_Minus45Diagonal) uiPol_LeftCircular.Checked = !uiPol_Minus45Diagonal.Checked;
            //if (ui == uiPol_LeftCircular) uiPol_Minus45Diagonal.Checked = !uiPol_LeftCircular.Checked;
        }
        finally
        {
            uiPol_LeftCircular.Tag = false;
            //uiPol_Minus45Diagonal.Tag = false;
        }
    }

	
}
