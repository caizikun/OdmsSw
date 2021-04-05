using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Free302.MyLibrary.Utility;

namespace Neon.Aligner
{

	public class ScheduleParam
	{
		public DateTime StartTime;				//시작 시간

		public double StartTemp;				//시작 온도
		public double AttainmentTemp;			//도달 온도
		public double StartHumi;				//시작 습도
		public double AttainmentHumi;			//도달 습도
		public bool DoHumi;						//습도 On||Off

		public int ExposeTime;					//지속 시간[m]

		public bool DoMeasure;					//측정 유무
		public int MeasureDelayMinute;			//측정 delay 시간[m]

		public ScheduleParam Clone()
		{
			var c = new ScheduleParam();
			c.StartTime = StartTime;

			c.StartTemp = StartTemp;
			c.AttainmentTemp = AttainmentTemp;
			c.StartHumi = StartHumi;
			c.AttainmentHumi = AttainmentHumi;
			c.DoHumi = DoHumi;

			c.ExposeTime = ExposeTime;

			c.DoMeasure = DoMeasure;
			c.MeasureDelayMinute = MeasureDelayMinute;

			return c;
		}

	}


	
	public class EspecSchedule
	{

		/// <summary>
		/// 스케쥴 실행 Action값
		/// </summary>
		public Action<string> RunAction { get; set; }
		

		/// <summary>
		/// 동작 챔버 class
		/// </summary>
		public EspecChamber Chamber { get; set; }


		/// <summary>
		/// 스케쥴 파일 불러오기
		/// </summary>
		/// <param name="fileName">파일 경로</param>
		/// <returns></returns>
		public List<ScheduleParam> LoadSchedule (string fileName)
		{
			List<ScheduleParam> scheduleUnits = new List<ScheduleParam>();
			ScheduleParam unit;

			DateTime currTime = new DateTime(2000, 1, 1, 00, 00, 00);

			var obj = new StreamReader(fileName);
			string line = "";

			while (true)
			{
				line = obj.ReadLine();
				if (line == null || line == "") break;

				if (line.Contains("[INITTIME]")) 
				{
					line = line.Replace("[INITTIME]", "").Trim();
					currTime = DateTime.Parse(line);
				}
				else if (line.Contains("[START]") || line.Contains("[END ]") || (!line.Contains("[") && !line.Contains("#")))
				{
					unit = new ScheduleParam();
					if (line.Contains("]"))
					{
						var temp = line.Split(']');
						line = temp[1].Trim();
					}
					var tempUnit = line.Split(':');
					
					//start 온도 & Attain 온도
					if (tempUnit[0].Contains("_"))
					{
						var tempSet = tempUnit[0].Split('_');
						unit.StartTemp = tempSet[0].Trim().To<double>();
						unit.AttainmentTemp = tempSet[1].Trim().To<double>();
					}
					else
					{
						unit.StartTemp = tempUnit[0].Trim().To<double>();
						unit.AttainmentTemp = tempUnit[0].Trim().To<double>();
					}

					//시간
					unit.StartTime = currTime;
					unit.ExposeTime = tempUnit[1].To<int>();
					unit.MeasureDelayMinute = tempUnit[3].To<int>();

					//측정
					unit.DoMeasure = tempUnit[2].Contains("MEAS") ? true : false;

					//습도
					if (tempUnit.Length == 5)
					{
						if (tempUnit[4].Contains("OFF")) unit.DoHumi = false;
						else
						{
							unit.DoHumi = true;
							if (tempUnit[4].Contains("_"))
							{
								var tempHumiSet = tempUnit[4].Split('_');
								unit.StartHumi = tempHumiSet[0].Trim().To<double>();
								unit.AttainmentHumi = tempHumiSet[1].Trim().To<double>();
							}
							else
							{
								unit.StartHumi = tempUnit[4].Trim().To<double>();
								unit.AttainmentHumi = tempUnit[4].Trim().To<double>();
							}
						}
					}

					scheduleUnits.Add(unit);
					currTime = currTime.AddMinutes(unit.ExposeTime);
				}

			}// while
			obj.Close();


			return scheduleUnits;
		}



		public volatile bool mStop = false;

		/// <summary>
		/// 스케쥴 실행
		/// </summary>
		/// <param name="scheduleList">스케쥴 List</param>
		/// <param name="actionRunIndex">현재 진행 스케쥴 Index</param>
		/// <returns>스케쥴 종료 sign [true : 정상종료 || false : Stop버튼]</returns>
		public async Task<bool> RunSchedule(List<ScheduleParam> scheduleList, Action<int> actionRunIndex, Action<double, double> actionChamberStatus)
		{
			var currTime = DateTime.Now;
			mStop = false;

			//Clear 실행 [10회]
			Chamber?.ChamberClear(10);

			for (int i = 0; i < scheduleList.Count; i++)
			{
				var param = new ChamberRemoteParams
				{
					StartTemp = scheduleList[i].StartTemp,
					AttainmentTemp = scheduleList[i].AttainmentTemp,
					StartHumi = scheduleList[i].StartHumi,
					AttainmentHumi = scheduleList[i].AttainmentHumi,
					DoHumiControl = scheduleList[i].DoHumi,
					ExposeTime = scheduleList[i].ExposeTime
				};

				//챔버 스케쥴 실행
				actionRunIndex(i);
				Chamber?.RunRemoteProgram(param);
				await Task.Delay(1000);

				while (true)
				{
					currTime = DateTime.Now;

					//Action(측정) 실행
					var diffTime = (currTime - scheduleList[i].StartTime.AddMinutes(scheduleList[i].MeasureDelayMinute)).TotalSeconds;
					if (diffTime > 0 && diffTime <= 1 && scheduleList[i].DoMeasure)
					{
						RunAction($"{i+1}_{scheduleList[i].StartTemp}" );
						await Task.Delay(1000);
					}

					//스케쥴 [1 time] 종료 조건
					if (currTime > scheduleList[i].StartTime.AddMinutes(scheduleList[i].ExposeTime)) break;

					//Stop
					if (mStop)
					{
						Chamber?.StopRemoteProgram();
						return false;
					}
					await Task.Delay(1000);

					//챔버 온도, 습도 읽기 (10초에 1번)
					if (currTime.Second % 10 == 0)
					{
						var conditions = new ChamberConditions() { Temp = 0, Humi = 0 };
						if(Chamber != null) conditions = Chamber.GetConditions();
						actionChamberStatus(conditions.Temp, conditions.Humi);
					}

				}

			}

			//챔버 프로그램 모드 종료
			Chamber?.StopRemoteProgram();
			return true;

		}


	}
}
