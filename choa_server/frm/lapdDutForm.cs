using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Free302.MyLibrary.Utility;
using Free302.TnM.Neon.Pigtail;
using Neon.Aligner;
using System.IO;

public partial class LapdDutForm : Form
{


	#region ==== class Frameworks ====

	Neon.Aligner.choa_server.Properties.Settings mSet = Neon.Aligner.choa_server.Properties.Settings.Default;
	PdMeasureLogic mPdLogic;
		

	public LapdDutForm()
	{
		InitializeComponent();

		initUi();
		initMembers();
	}
	

	private void initUi()
	{
		initWaveGrid(); //wave Grid 설정
		initRefGrid();  //Ref Grid 설정
		initDutGrid();	//Dut Grid 설정
		initDaqUi();
	}


	private void initMembers()
	{
		mPdLogic = new PdMeasureLogic();

	}


	private void lapdDutForm_Load(object sender, EventArgs e)
	{
		OpmDisplayForm pmDisplayForm = FormLogic<frmMain>.CreateAndShow<OpmDisplayForm>(true, false);
		pmDisplayForm?.DisplayOff();
		if (lbSaveFolderPath.Text.Trim() == "Save Folder :") lbSaveFolderPath.Text = Application.StartupPath;
	}


	private void lapdDutForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		//설정값 저장
		mSet.lapdChipWidth = txtChipWidth.Text;
		mSet.lapdWaveStart = txtWaveStart.Text;
		mSet.lapdOffsetZ = txtLapdOffset.Text;
		mSet.lapdOffsetY = txtLapdOffsetY.Text;
		mSet.lapdOffsetBack = txtOffsetBack.Text;
		mSet.lapdOffsetReturn = txtOffsetReturn.Text;

		mSet.Save();
		mPdLogic.Dispose();
	}

	#endregion



	#region ==== Log ====

	private void updateStatus(PdMeasureStatus status)
	{
		uiStatus.Text = $"[{status.CurrentSerial}측정 완료]     DUT 측정중...";

		//측정 데이터 출력
		if (mDtDut.Columns.Count == 0) return;
		DataRow ILRow = mDtDut.Select($"{mDtColumn1Name} = '{status.CurrentSerial}'").FirstOrDefault();
		foreach (var item in status.dutIL) ILRow[item.Key.ToString()] = item.Value;

	}

	public void AddLog(string msg)
	{
		if (this.InvokeRequired) this.Invoke((Action)(() => writeLog(msg)));
		else writeLog(msg);
	}

	void writeLog(string msg)
	{
		uiStatus.Text = ($"[{DateTime.Now.ToString("yyMMdd HH:mm:ss.fff")}] {msg}");
	}

	#endregion



	#region ==== DAQ <PD> ====

	private void initDaqUi()
	{
		cboDaqPrimary.DataSource = Enum.GetValues(typeof(DaqPrimary));
		cboDaqPrimary.SelectedIndex = 0;
		cboAiCh.DataSource = Enum.GetValues(typeof(DaqAiCh));
		cboAiCh.SelectedIndex = 0;
		cboRse.DataSource = Enum.GetValues(typeof(NationalInstruments.DAQmx.CIEncoderAInputTerminalConfiguration));
		cboRse.SelectedIndex = 1;
		cboAvgTime.DataSource = Enum.GetValues(typeof(PmAvgTime));
		cboAvgTime.SelectedItem = PmAvgTime.T100ms;
	}


	private void btnPDInit_Click(object sender, EventArgs e)
	{
		//PD 초기화 버튼
		var avgTime = (PmAvgTime)cboAvgTime.SelectedItem;//unit=100us
		
		//PD 초기화
		if (!uiEsm.Checked)
		{
			//DAQ PM
			var pd = new DaqPm();

			var daqprimary = (DaqPrimary)cboDaqPrimary.SelectedItem;
			var dicChToAi = new Dictionary<PmCh, DaqAiCh>() { { PmCh.CH1, (DaqAiCh)cboAiCh.SelectedItem } };
			var dicAiToRange = new Dictionary<DaqAiCh, double[]>() { { (DaqAiCh)cboAiCh.SelectedItem, new double[] { 0, 10 } } };
			var res = cboRse.SelectedIndex == 0 ? true : false;

			var responsivity = txtDaqResponsivity.Text.Unpack<double>().ToArray();
			var dicResistance = new Dictionary<DaqAiCh, double>() { { (DaqAiCh)cboAiCh.SelectedItem, txtDaqResistance.Text.To<double>() } };

			mPdLogic.InitPD(avgTime, daqprimary, dicChToAi, dicAiToRange, res, responsivity, dicResistance);
		}
		else
		{
			//Source Meter PM
			var pd = new Keithley2401();
			var dic = new Dictionary<PmCh, PmSlotPort>() { { PmCh.CH1, new PmSlotPort((DeviceSlot)1, (DevicePort)1) } };
			var address = txtEsmAddress.Text.Unpack<string>().ToArray();

			mPdLogic.InitPD(avgTime, pd, dic, address);
		}

		//Power Display
		mPdLogic.PowerReporter = UpdatePower;

	}

	#endregion



	#region ==== Power Display ====

	public void UpdatePower(double power)
	{
		if (!this.Created) return;
		if (this.IsDisposed) return;

		var text = $"{power:f03}";

		if (this.InvokeRequired) this.Invoke((Action)(() => { txtPdPower.Text = text; }));
		else txtPdPower.Text = text;
	}

	#endregion



	#region ====  WaveList ====


	private void initWaveGrid()
	{
		//wave list grid 초기화
		DataGridView grid = this.gridWaveList;
		string[] columnNames = new string[] { "Num", "λ [nm]" };

		grid.ReadOnly = false;
		grid.ScrollBars = ScrollBars.Vertical;
		grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
		grid.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;

		//row
		grid.RowHeadersVisible = false;
		grid.AllowUserToAddRows = false;
		grid.AllowUserToResizeRows = false;
		grid.RowTemplate.Height = 20;

		//col
		var gridCol = new DataGridViewTextBoxColumn();
		gridCol.Name = columnNames[0];
		gridCol.HeaderText = columnNames[0];
		gridCol.DataPropertyName = gridCol.Name;
		gridCol.Width = 44;
		gridCol.ReadOnly = true;
		gridCol.Frozen = true;
		gridCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridCol.SortMode = DataGridViewColumnSortMode.NotSortable;
		gridCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		grid.Columns.Add(gridCol);

		gridCol = new DataGridViewTextBoxColumn();
		gridCol.Name = columnNames[1];
		gridCol.HeaderText = columnNames[1];
		gridCol.DataPropertyName = gridCol.Name;
		gridCol.Width = 75;
		gridCol.ReadOnly = false;
		gridCol.Frozen = true;
		gridCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridCol.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridCol.SortMode = DataGridViewColumnSortMode.NotSortable;
		gridCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		grid.Columns.Add(gridCol);

	}

	private void displayWaveList(List<double> waveList)
	{
		//wavelist - grid 표시
		DataGridView grid = this.gridWaveList;
		grid.Rows.Clear();
		for (int i = 0; i < waveList.Count; i++) grid.Rows.Add((i + 1).ToString(), waveList[i]);

	}

	private List<double> makeWaveList(double waveStart, double waveStep, int waveCount)
	{
		//wavelist - list 생성
		List<double> list = new List<double>();
		double wave = waveStart;
		for (int i = 0; i < waveCount; i++)
		{
			list.Add(wave);
			wave = (double)((decimal)wave + (decimal)waveStep);
		}

		return list;
	}

	private double[] getWaveList()
	{
		//list grid값 읽어오기
		List<double> list = new List<double>();
		for (int i = 0; i < gridWaveList.Rows.Count; i++)
			list.Add(Convert.ToDouble(gridWaveList.Rows[i].Cells[1].Value));

		return list.ToArray();
	}

	private void btnWaveApply_Click(object sender, EventArgs e)
	{
		//measureWave - Apply 버튼 클릭
		var list = makeWaveList(txtWaveStart.Text.To<double>(), txtWaveStep.Text.To<double>(), txtWaveCount.Text.To<int>());
		displayWaveList(list);
		mSet.lapdWaveStart = txtWaveStart.Text;
		mSet.Save();
	}


	#endregion



	#region ==== Reference ====

	DataTable mDtRef;

	void initRefGrid()
	{

		mDtRef = new DataTable();
		gridRef.DataSource = mDtRef;
		gridRef.EditMode = DataGridViewEditMode.EditProgrammatically;
		gridRef.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		gridRef.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridRef.RowHeadersVisible = false;
		gridRef.AllowUserToAddRows = false;

	}


	private async void btnMeasureRef_Click(object sender, EventArgs e)
	{
		//[Ref 측정 버튼]
		if (!mPdLogic.ConnectPD)
		{
			MessageBox.Show("PD 초기화!");
			return;
		}
		if (gridWaveList.Rows.Count == 0) btnWaveApply.PerformClick();

		AddLog("Ref. 측정");
		this.Cursor = Cursors.WaitCursor;

		var wavelist = getWaveList();
		DataRow powerRow = mDtRef.NewRow();
		List<double> powers;
		try
		{
			mPdLogic.PowerDisplayOn = false;
			powers = await mPdLogic?.ReadPower(wavelist);

			//grid 표시
			mDtRef.Clear();
			mDtRef.Columns.Clear();
			if (wavelist.Length > 50) gridRef.DataSource = null;
			else gridRef.DataSource = mDtRef;

			for (int i = 0; i < wavelist.Length; i++)
			{
				mDtRef.Columns.Add(wavelist[i].ToString(), typeof(double));
				powerRow[wavelist[i].ToString()] = powers[i];
			}
			mDtRef.Rows.Add(powerRow);

		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
		}
		finally
		{
			mPdLogic.PowerDisplayOn = true;
			this.Cursor = Cursors.Default;
			AddLog("Ref. 측정완료");
		}
		
	}


	private void btnApplyRef_Click(object sender, EventArgs e)
	{
		//[Ref 적용 버튼]
		double[] refLambda = mDtRef.Columns.Cast<DataColumn>().Select(x => x.ColumnName.To<double>()).ToArray();

		if (refLambda.Length == 0) { AddLog("Ref. 측정값이 없습니다."); return; }
		AddLog("Ref. 적용");

		//ref 적용
		mPdLogic.SetRefLambda(refLambda);
		for (int i = 0; i < refLambda.Length; i++)
			mPdLogic.RecoredRef(refLambda[i], Convert.ToDouble(mDtRef.Rows[0][i]));
		mPdLogic.SaveRefFile();
		btnLoadReference.Text = mPdLogic.RefFileName;
		AddLog("Ref. 적용완료");

	}


	private void btnRefSaveAs_Click(object sender, EventArgs e)
	{
		//[Ref 따로저장 버튼]
		if (mDtRef.Columns.Count == 0) { AddLog("Ref. 측정값이 없습니다."); return; }
		AddLog("Ref. 다른 이름으로 저장");

		SaveFileDialog fd = new SaveFileDialog();
		fd.Title = "Pd_Reference 파일";
		fd.InitialDirectory = Path.GetDirectoryName(mPdLogic.RefFileName);
		fd.Filter = $"Reference File(*.txt)|*.txt";
		if (fd.ShowDialog() != DialogResult.OK) return;

		mPdLogic.SaveRefFile(fd.FileName);

		btnLoadReference.Text = fd.FileName;        //file path
		AddLog("Ref. 저장완료");

	}


	private void btnLoadReference_Click(object sender, EventArgs e)
	{
		//[Ref 불러오기]
		OpenFileDialog fd = new OpenFileDialog();
		fd.Title = "Pd_Reference 파일";
		fd.InitialDirectory = Path.GetDirectoryName(mPdLogic.RefFileName);
		fd.Filter = $"Reference File(*.txt)|*.txt";
		if (fd.ShowDialog() != DialogResult.OK) return;

		var refDic = mPdLogic.loadRefFile(fd.FileName);

		//grid 표시
		mDtRef.Clear();
		mDtRef.Columns.Clear();
		if (refDic.Keys.Count < 50) gridRef.DataSource = mDtRef;			//파장이 50개 미만인 경우만 화면 표시
		else gridRef.DataSource = null;

		DataRow powerRow = mDtRef.NewRow();
		foreach (var item in refDic.Keys)
		{
			mDtRef.Columns.Add(item.ToString(), typeof(double));
			powerRow[item.ToString()] = refDic[item];
		}
		mDtRef.Rows.Add(powerRow);

		//waveList update
		displayWaveList(mDtRef.Columns.Cast<DataColumn>().Select(x => x.ColumnName.To<double>()).ToList());

		btnLoadReference.Text = fd.FileName;        //file path
		AddLog("Ref. 적용완료 [Ref 불러오기]");

	}

	#endregion



	#region ==== Dut ====

	/// <summary>
	/// Serial 개수 번호 설정 위치값 (hyphen으로 구분)
	/// </summary>
	private int SerialCountIndex { get; set; } = 4;

	DataTable mDtDut;

	void initDutGrid()
	{
		//Dut Grid 초기화
		mDtDut = new DataTable();
		gridDut.DataSource = mDtDut;
		gridDut.EditMode = DataGridViewEditMode.EditProgrammatically;
		gridDut.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
		gridDut.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		gridDut.RowHeadersVisible = false;
		gridDut.AllowUserToAddRows = false;

	}
	

	private async void btnMeasureDut_Click(object sender, EventArgs e)
	{
		//[Dut 측정 버튼]
		if (gridWaveList.Rows.Count == 0) btnWaveApply.PerformClick();

		string saveName, savePath;
		var chipSerials = setChipSerial(out saveName);
		savePath = $"{lbSaveFolderPath.Text}\\LargeAreaPd_DutData";
		Directory.CreateDirectory(savePath);

		//측정 조건
		if (!mPdLogic.ConnectPD)
			{ MessageBox.Show("PD 초기화!"); return; }         //DAQ 초기화 확인
		if (chipSerials == null)
			{ MessageBox.Show("PD 초기화!"); return; }         //chip serial list 확인

		AddLog("Dut 측정 Click");
		
		//grid 표시 설정
		dutGridDisplayChipSerial(chipSerials);

		//측정 옵션 설정
		var param = new PdMeasureParam
		{
			ChipSerials = chipSerials,
			ChipWidth = txtChipWidth.Text.To<int>(),
			Alignment = chkAlignment.Checked,
			OutAlign = txtOutAlign.Text.To<int>(),
			Approach = chkApproach.Checked,
			ApproachDistance = txtApproach.Text.To<int>(),
			LapdOffset = txtLapdOffset.Text.To<int>(),
			lapdOffsetYaxis = txtLapdOffsetY.Text.To<double>(),
			LapdOffsetBack = txtOffsetBack.Text.To<int>(),
			LapdOffsetReturn = txtOffsetReturn.Text.To<int>(),
			Measurement = chkMeasurement.Checked,
			CenterStage = chkCenterStage.Checked,
			endZDistance = txtZDistance.Text.To<int>(),
			saveFolderPath = savePath,
			saveName = saveName
		};

		//시작설정
		PdMeasureLogic.mStop = false;
		var status = new PdMeasureStatus();
		status.Reporter = updateStatus;

		this.Cursor = Cursors.WaitCursor;			//mouse 커서
		mPdLogic.PowerDisplayOn = false;			//power display 비 활성화
		enableUi(false);							//컨트롤 비활성화

		try
		{
			//측정
			await mPdLogic.DoMeasure(param, status);

		}
		catch (Exception ex)
		{
			MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
		}
		finally
		{
			this.Cursor = Cursors.Default;			//mouse 커서
			mPdLogic.PowerDisplayOn = true;			//power display 활성화
			enableUi(true);                         //컨트롤 활성화
			AddLog("Dut. 측정완료");
		}

	}

	const string mDtColumn1Name = "Serial";

	private void dutGridDisplayChipSerial(string[] chipSerials)
	{
		//[Dut Grid - Chip serial 입력]
		DataRow ILRow;
		var wavelist = getWaveList();
		mDtDut.Clear();
		mDtDut.Columns.Clear();
		if (wavelist.Length > 50) return;

		mDtDut.Columns.Add(mDtColumn1Name, typeof(string));
		for (int i = 0; i < wavelist.Length; i++) mDtDut.Columns.Add(wavelist[i].ToString(), typeof(double));
		for (int i = 0; i < chipSerials.Length; i++)
		{
			ILRow = mDtDut.NewRow();
			ILRow[mDtColumn1Name] = chipSerials[i];
			for (int j = 0; j < wavelist.Length; j++) ILRow[wavelist[j].ToString()] = 0;
			mDtDut.Rows.Add(ILRow);
		}

		for (int i = 0; i < gridDut.Columns.Count; i++)
			gridDut.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

	}


	private void enableUi(bool enable)
	{
		//[Ui 활성화 || 비활성화]
		groupInit.Enabled = enable;
		groupPowerDisplay.Enabled = enable;
		groupAlignment.Enabled = enable;
		groupMeasureWave.Enabled = enable;
		groupSave.Enabled = enable;
		groupReference.Enabled = enable;
		groupDutOption.Enabled = enable;

		btnMeasureDut.Enabled = enable;

	}

	string[] setChipSerial(out string saveName)
	{
		//[시리얼 및 칩 개수 입력 처리]
		int numChips;
		saveName = "";
		//오류 확인
		if (txtDutChipCount.Text == "") numChips = 0;
		else if (!int.TryParse(txtDutChipCount.Text, out numChips))
		{
			MessageBox.Show(this, "칩개수를 정확히 입력해 주세요.", "Dut Chip 개수 입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return null;
		}

		string[] inString = txtDutSerial.Text.ToUpper().Split('-');
		if (inString.Length < SerialCountIndex)
		{
			string comment = $"\nChip Count 설정위치가 {SerialCountIndex}입니다.\nHyphen(-) 구분개수가 부족.  {SerialCountIndex - 1}개 필요!";
			MessageBox.Show(this, comment, "Dut Serial 입력 오류!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return null;
		}

		//Serial 입력값 처리
		string[] serialList = new string[numChips];
		string strFront = "";
		string strRear = "";
		if (SerialCountIndex > 1) for (int i = 0; i < SerialCountIndex - 1; i++) strFront += inString[i] + "-";

		var strStartChipNo = Regex.Replace(inString[SerialCountIndex - 1], @"\D", "");
		int startChipNo = 0;
		if (strStartChipNo != "")
		{
			strFront += Regex.Replace(inString[SerialCountIndex - 1], strStartChipNo, "");
			startChipNo = Convert.ToInt32(strStartChipNo);
		}
		else strFront += inString[SerialCountIndex - 1];

		if (inString.Length > SerialCountIndex) for (int i = SerialCountIndex; i < inString.Length; i++) strRear += "-" + inString[i];

		//List 추가
		for (int i = 0; i < numChips; i++)
		{
			serialList[i] = strFront;                       //chip 번호 앞 string
			serialList[i] += $"{(startChipNo + i):D2}";     //chip 번호
			serialList[i] += strRear;                       //chip 번호 뒤 string
			serialList[i] = serialList[i].ToUpper();        //대문자 변환
		}
		
		if (numChips <= 1) saveName = serialList[0];
		else saveName = $"{strFront}{startChipNo:D2}~{startChipNo + numChips - 1:D2}";

		return serialList;
	}



	private void btnStop_Click(object sender, EventArgs e)
	{
		//[Dut 측정 Stop 버튼]
		AddLog("Stop_Click");
		PdMeasureLogic.mStop = true;
	}

	#endregion



	#region ==== Save ====

	private void btnSaveFolder_Click(object sender, EventArgs e)
	{
		//[save folder 지정]
		var fd = new FolderBrowserDialog();
		if (fd.ShowDialog() == DialogResult.OK)
		{
			lbSaveFolderPath.Text = fd.SelectedPath;
			mSet.lapdDutForm_Savefolder = fd.SelectedPath;
			mSet.Save();
		}
	}

	
	#endregion



	#region ==== Alignment ====

	private async void btnAxisScan_Click(object sender, EventArgs e)
	{
		//[Input 정렬]
		if (!mPdLogic.ConnectPD)
		{
			MessageBox.Show("PD 초기화!");
			return;
		}
		AddLog("btnAxisScan_Click");

		try
		{
			this.Cursor = Cursors.WaitCursor;
			mPdLogic.PowerDisplayOn = false;
			await mPdLogic?.doXySearchByLAPD();

		}
		finally
		{
			mPdLogic.PowerDisplayOn = true;
			this.Cursor = Cursors.Default;
			AddLog("정렬완료");
		}
	}



	#endregion


}

