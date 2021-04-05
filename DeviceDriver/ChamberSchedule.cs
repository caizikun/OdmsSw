using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Free302.MyLibrary.Utility;

namespace Neon.Aligner
{
	public partial class ChamberSchedule : UserControl
	{

		#region ==== Constructor / Destructor ====

		EspecSchedule mChamberSchedule;
		Action<string> mRunAction;
		public Action<string> RunAction
		{
			get { return mRunAction; }
			set { mChamberSchedule.RunAction = value; mRunAction = value; }
		}

		EspecChamber mChamber;
		public EspecChamber RunChamber
		{
			get { return mChamber; }
			set { mChamberSchedule.Chamber = value; mChamber = value; }
		}


		public ChamberSchedule()
		{
			InitializeComponent();

			mChamberSchedule = new EspecSchedule();
			initScheduleGrid();
		}


		private void timer1_Tick(object sender, EventArgs e)
		{
			//현재시간 표시
			txtTimeCurrent.Text = DateTime.Now.ToString(@"HH\:mm\:ss");
			//남은시간 표시
			if (isRun)
			{
				var remainingTime = mEndTime - DateTime.Now;
				txtTimeRemain.Text = (remainingTime.Days >= 1 ? $"{remainingTime.Days}d. " : "") + remainingTime.ToString(@"hh\:mm\:ss");
			}
		}

		#endregion



		#region ==== Schedule load & display ====

		int mScheduleRepeatCount = 1;
		public int ScheduleRepeatCount { get { return mScheduleRepeatCount; } }
		List<ScheduleParam> mScheduleUnits;

		public List<string> GetActionSchedule()
		{
			if (mScheduleUnits == null) return null;
			List<string> actionSchedule = new List<string>();

			for (int i = 0; i < mScheduleUnits.Count; i++)
			{
				if (mScheduleUnits[i].DoMeasure)
					actionSchedule.Add(mScheduleUnits[i].StartTemp.ToString());
					
			}

			return actionSchedule;

		}

		
		private void btnLoadSchedule_Click(object sender, EventArgs e)
		{
			//스케쥴 불러오기
			OpenFileDialog ofd = new OpenFileDialog();
			try
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					mScheduleUnits = mChamberSchedule.LoadSchedule(ofd.FileName);
					mScheduleRepeatCount = txtCount.Text.To<int>();   //Count값
					setSchedule();								//스케쥴 설정 및 grid표시
					btnLoadSchedule.Text = ofd.FileName;		//스케쥴 파일 경로표시
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
			}
			
		}


		DataTable mScheduleTable;
		string[] mColumnsName = {"Cycle", "Start Time (min)", "Start Temp     (ºC)", "Attain. Temp  (ºC)", "Exp. Time (min)", "Msr Delay Time (min)", "Msr. ", "Start Humi(%)", "Attain Humi(%)" };

		void initScheduleGrid()
		{
			//grid 초기화
			mScheduleTable = new DataTable();
			gridScheduleTable.DataSource = mScheduleTable;
			gridScheduleTable.EditMode = DataGridViewEditMode.EditProgrammatically;
			gridScheduleTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			gridScheduleTable.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			gridScheduleTable.RowHeadersVisible = false;
			gridScheduleTable.AllowUserToAddRows = false;

			mScheduleTable.Clear();
			mScheduleTable.Columns.Clear();

			for (int i = 0; i < mColumnsName.Length; i++)
				mScheduleTable.Columns.Add(mColumnsName[i]);
			
		}


		DateTime mEndTime;	//측정 종료 시간

		private List<ScheduleParam> setSchedule()
		{
			//Full 스케쥴 화면 표시
			if (mScheduleUnits == null || mScheduleUnits.Count == 0) return null;
			var confirmSchedule = new List<ScheduleParam>();			//최종 스케쥴 return값
			mScheduleTable.Clear();
			var cycleMinute = 0;
			var currTime = DateTime.Now;
			var timeGap = currTime - mScheduleUnits[0].StartTime;

			for (int i = 0; i < mScheduleRepeatCount; i++)
			{
				for (int j = 0; j < mScheduleUnits.Count; j++)
				{
					var dataRow = mScheduleTable.NewRow();

					dataRow[mColumnsName[0]] = $"{i + 1}";
					var startTime = (i == 0 && j == 0) ? currTime : mScheduleUnits[j].StartTime.AddMinutes(cycleMinute) + timeGap;
					dataRow[mColumnsName[1]] = startTime.ToString("HH:mm");

					dataRow[mColumnsName[2]] = mScheduleUnits[j].StartTemp;
					dataRow[mColumnsName[3]] = mScheduleUnits[j].AttainmentTemp;
					dataRow[mColumnsName[4]] = mScheduleUnits[j].ExposeTime;
					dataRow[mColumnsName[5]] = mScheduleUnits[j].MeasureDelayMinute;
					dataRow[mColumnsName[6]] = (mScheduleUnits[j].DoMeasure == true) ? "O" : "x";
					if (mScheduleUnits[j].DoHumi)
					{
						dataRow[mColumnsName[7]] = mScheduleUnits[j].StartHumi;
						dataRow[mColumnsName[8]] = mScheduleUnits[j].AttainmentHumi;
					}
					else
					{
						dataRow[mColumnsName[7]] = "OFF";
						dataRow[mColumnsName[8]] = "OFF";
					}
					mScheduleTable.Rows.Add(dataRow);

					var unit = mScheduleUnits[j].Clone();
					unit.StartTime = startTime;
					confirmSchedule.Add(unit);
					
				}
				cycleMinute += mScheduleUnits.AsEnumerable().Sum(x => x.ExposeTime);
			}

			//습도 표시
			gridScheduleTable.Columns[7].Visible = mScheduleUnits[0].DoHumi;
			gridScheduleTable.Columns[8].Visible = mScheduleUnits[0].DoHumi;

			//시간표시
			txtTimeStart.Text = currTime.ToString("HH시 mm분");
			var endTime = mScheduleUnits[0].StartTime.AddMinutes(cycleMinute) + timeGap;
			var totalDays = (endTime.Date - currTime.Date).Days; 
			txtTimeEnd.Text = (totalDays > 0)? $"[+{(int)totalDays}일] {endTime:HH시 mm분}" : endTime.ToString("HH시 mm분");
			var remainingTime = endTime - currTime;
			txtTimeRemain.Text = (remainingTime.Days >= 1 ? $"{remainingTime.Days}d. " : "") + remainingTime.ToString(@"hh\:mm\:ss");
			mEndTime = endTime;

			//grid 색 초기화
			for (int i = 0; i < gridScheduleTable.Rows.Count; i++)
				gridScheduleTable.Rows[i].DefaultCellStyle.BackColor = SystemColors.Window;

			return confirmSchedule;

		}


		private void btnApplyCurrTime_Click(object sender, EventArgs e)
		{
			//스케쥴 재 설정 버튼(현재 시간 & count 적용)
			if (mScheduleUnits == null || mScheduleUnits.Count == 0) return;
			mScheduleRepeatCount = txtCount.Text.To<int>();   //Count값

			setSchedule();

		}
		
		#endregion



		#region ==== Schedule 실행 ====

		bool isRun = false;

		/// <summary>
		/// 스케쥴 실행
		/// </summary>
		public async Task<bool> RunSchedule()
		{
			if (mScheduleUnits == null || mScheduleUnits.Count == 0) return true;
			if (isRun) return true;
			mScheduleRepeatCount = txtCount.Text.To<int>();   //Count값

			//count & 현재 시간 적용
			var finalSchedule = setSchedule();

			//스케쥴 시작 ★
			runModeUi(true);
			isRun = true;

			var ret = await mChamberSchedule.RunSchedule(finalSchedule, displayCurrScheduleUnit, ActionChamberStatus);
			isRun = false;
			runModeUi(false);
			if (ret) MessageBox.Show("모든 스케쥴이 종료되었습니다.", "측정완료!");
			else MessageBox.Show("스케쥴 진행이 중지되었습니다.","Stop!");

			return true;
		}

		public Action<double, double> ActionChamberStatus { get; set; }

		private void displayCurrScheduleUnit(int index)
		{
			//현재 진행 스케쥴 표시 [grid 색]
			bool measure = gridScheduleTable.Rows[index].Cells[6].Value.ToString() == "O" ? true : false;
			if (measure) gridScheduleTable.Rows[index].DefaultCellStyle.BackColor = Color.LightGreen;
			else gridScheduleTable.Rows[index].DefaultCellStyle.BackColor = Color.LightBlue;
		}


		private void runModeUi(bool run)
		{
			if (run)
			{
				//스케쥴 실행 시 UI 설정
				txtTimeCurrent.Font = new Font(txtTimeCurrent.Font, FontStyle.Bold);
				txtTimeCurrent.BackColor = Color.Black;
				txtTimeCurrent.ForeColor = Color.Lime;
				txtTimeRemain.Font = new Font(txtTimeCurrent.Font, FontStyle.Bold);
				txtTimeRemain.BackColor = Color.Black;
				txtTimeRemain.ForeColor = Color.Lime;
			}
			else
			{
				//스케쥴 종료 시 UI 설정
				txtTimeCurrent.Font = new Font(txtTimeCurrent.Font, FontStyle.Regular);
				txtTimeCurrent.BackColor = SystemColors.Window;
				txtTimeCurrent.ForeColor = Color.Maroon;
				txtTimeRemain.Font = new Font(txtTimeCurrent.Font, FontStyle.Regular);
				txtTimeRemain.BackColor = SystemColors.Window;
				txtTimeRemain.ForeColor = Color.Maroon;
			}
			btnApplyCurrTime.Enabled = !run;
			btnLoadSchedule.Enabled = !run;
		}


		/// <summary>
		/// 스케쥴 강제 종료
		/// </summary>
		public void StopSchedule()
		{
			mChamberSchedule.mStop = true;
			runModeUi(false);
			isRun = false;
		} 

		#endregion


	}
}
