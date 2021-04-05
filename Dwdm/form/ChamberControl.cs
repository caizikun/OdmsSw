using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Neon.Aligner;

namespace Neon.Dwdm
{
	public partial class ChamberControl : Form
	{

		#region ==== Constructor ====

		public ChamberControl()
		{
			InitializeComponent();
		}

		EspecChamber mChamber;

		private void ChamberControl_Load(object sender, EventArgs e)
		{
			mChamber = CGlobal.EspecChamber;
			UiChamberSchedule.RunChamber = mChamber;				//chamber class
			UiChamberSchedule.RunAction = scheduleAction;			//스케쥴 측정실행 Action 
			UiChamberSchedule.ActionChamberStatus = statusDisplay;	//챔버 온도 습도 표시 Action
		}

		private void ChamberControl_FormClosed(object sender, FormClosedEventArgs e)
		{
			UiChamberSchedule.StopSchedule();
		}


		private void scheduleAction(string chipComment)
		{
			//스케쥴 실행 Action  *측정!!
			var form = MyLogic.CreateAndShow<MeasureForm>(true, false);
			form.m_ScheduleFlag = true;

			AddLog("측정 실행!");

		}

		private void setActionSerial(string serial, List<string> tempList, int repeatCount)
		{
			//측정 시리얼 입력 - 측정form : Serial List
			var form = MyLogic.CreateAndShow<MeasureForm>(true, false);
			var actionListTemp = new List<string>();

			int index = 1;
			for (int i = 0; i < repeatCount; i++)
				for (int j = 0; j < tempList.Count; j++)
				{
					actionListTemp.Add($"{serial}-{index}-{tempList[j]}");
					index += 1;
				}

			form.SetChipNos(actionListTemp);

		}
			
		private void startMeasure()
		{
			//Measure 실행
			var form = MyLogic.CreateAndShow<MeasureForm>(true, false);
			form.m_ScheduleFlag = true;
			form.btnMeasure.PerformClick();
		}

		#endregion



		#region ==== Log 표시 ====

		public void AddLog(string msg, bool clear = false)
		{
			if (this.InvokeRequired) this.Invoke((Action)(() => writeLog(msg, clear)));
			else writeLog(msg, clear);
		}

		void writeLog(string msg ,bool clear)
		{
			if (clear) txtLog.Text = "";
			txtLog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss")}] {msg}\n");
			txtLog.Focus();
		}

		#endregion

		

		#region ==== 예약 ====

		private async Task<bool> bookMeasure(DateTime bookingTime)
		{
			bool checkIn = false;
			var span = bookingTime - DateTime.Now;

			if ((span).Ticks < 0)
			{
				MessageBox.Show("설정한 예약 시간이 이미 지났습니다.", "예약 실패");
				return checkIn;
			}

			string bookMessage = "";
			if (span.Days > 0) bookMessage = $"{span.Days}일 ";
			bookMessage += $"{span.Hours}시간 {span.Minutes}분 후에 스케쥴 측정이 시작됩니다.";
			bookMessage += "\n예약 측정을 하시겠습니까?";
			if (DialogResult.No == MessageBox.Show(bookMessage, "스케쥴 예약 실행", MessageBoxButtons.YesNo)) return checkIn;

			while (true)
			{
				if ((bookingTime - DateTime.Now).Ticks < 0)
				{
					checkIn = true;
					break;
				}
				if (mStop) break;
				await Task.Delay(1000);
			}

			return checkIn;			//true : 예약 정상 종료.
		}

		#endregion



		#region ==== Control [UI] ====

		private async void btnStart_Click(object sender, EventArgs e)
		{
			//스케쥴 시작 버튼
			AddLog("Start Click!", true);
			mStop = false;

			//Dut창 && ChipNo 확인
			var form = MyLogic.CreateAndShow<MeasureForm>(true, false);
			if (form == null)
			{
				MessageBox.Show("Dut 측정창이 열려있지 않습니다.", "Warning");
				return;
			}
			
			if (chkBookSchedule.Checked)
			{
				//예약 설정
				if (!await bookMeasure(pickerBookSchedule.Value))
				{
					MessageBox.Show("예약 취소.", "Warning");
					return;
				} 
			}
			else
			{
				//스케쥴 측정 - 측정 시작 확인
				if (DialogResult.No == MessageBox.Show("스케쥴을 시작하시겠습니까?", "스케쥴 실행", MessageBoxButtons.YesNo)) return;
			}

			var actionSchedule = UiChamberSchedule.GetActionSchedule();
			var scheduleRepeatCount = UiChamberSchedule.ScheduleRepeatCount;
			var originChipNo = form.txtFisrtChipNo.Text;
			if (actionSchedule.Count > 0)
			{
				//측정 List를 입력.
				setActionSerial(originChipNo, actionSchedule, scheduleRepeatCount);

				//Measure 실행
				startMeasure(); 
			}

			try
			{
				wfgChamber.ClearData();
				btnStart.Enabled = false;
				//스케쥴 실행
				await UiChamberSchedule.RunSchedule();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
			}
			finally
			{
				//Dut창 ChipNo 원래대로 설정
				form.txtFisrtChipNo.Text = originChipNo;

				btnStart.Enabled = true;
				AddLog("스케쥴 종료");
			}
		}

		bool mStop = false;

		private void btnStop_Click(object sender, EventArgs e)
		{
			//스케쥴 강제 종료 버튼
			AddLog("Stop Click!");
			try
			{
				UiChamberSchedule.StopSchedule();
				var form = MyLogic.CreateAndShow<MeasureForm>(true, false);
				form?.btnStop.PerformClick();
				AddLog("스케쥴 강제 종료");
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
			}
		}

		#endregion



		#region ==== Chamber Status [챔버 정보창] ====

		private void statusDisplay(double temp, double humi)
		{
			//챔버 정보 출력
			addStatus(txtStatusTemp, temp.ToString());
			addStatus(txtStatusHumi, humi.ToString());

			addTempGraph(temp);
		}


		private void addStatus(TextBox textBox, string msg)
		{
			//온도 습도 표시
			if (this.InvokeRequired) this.Invoke((Action)(() => textBox.Text = msg));
			else textBox.Text = msg;
		}

		private void addTempGraph(double temp)
		{
			//그래프 출력
			if (this.InvokeRequired) this.Invoke((Action)(() => wfgChamber.PlotYAppend(temp)));
			else wfgChamber.PlotYAppend(temp);
		}
		
		#endregion
		
		
	}
}
