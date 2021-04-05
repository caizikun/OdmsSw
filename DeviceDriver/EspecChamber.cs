using System;
using System.Linq;
using System.Threading;
using TnM.Device;
using TnM.Utility;

namespace Neon.Aligner
{

	#region ==== param Class ====

	/// <summary>
	/// Chamber의 동작 Mode
	/// </summary>
	public enum ChamberMode { HOLD = 0, OFF, STANDBY, CONSTANT, RUN }


	/// <summary>
	/// 챔버 프로그램 모드 설정 파라미터
	/// </summary>
	public class ChamberRemoteParams
	{
		public double StartTemp;			//시작 온도
		public double AttainmentTemp;		//도달 온도
		public bool DoHumiControl;			//습도 On||Off
		public double StartHumi;			//시작 습도
		public double AttainmentHumi;		//도달 습도

		public int ExposeTime;				//지속 시간
	}


	/// <summary>
	/// 챔버 상태 수신 값
	/// </summary>
	public class ChamberConditions
	{
		public double Temp;					//현재 온도
		public double Humi;					//현재 습도
		public ChamberMode Mode;
		public int AlaramCount;
	}

	#endregion



	public class EspecChamber : GpibBase
	{

		static int mCounter = 0;
		int id;
		public EspecChamber()
		{
			mCounter += 1;
			id = mCounter;
		}
		protected override string log(string message)
		{
			return base.log($"[{id}]{message}");
		}


		#region ==== Chamber 통신 ====


		/// <summary>
		/// Chamber의 통신 Clear 실행
		/// </summary>
		/// <param name="count">실행 횟수</param>
		public void ChamberClear(int count)
		{
			for (int i = 0; i < count; i++)
			{
				Clear();
				Thread.Sleep(1000);
			}

		}


		/// <summary>
		/// Chamber의 상태값 가져오기
		/// </summary>
		/// <returns></returns>
		public ChamberConditions GetConditions()
		{
			var conditions = new ChamberConditions();

			var response = query("MON?").Split(',');
			if (response.Length == 3)
			{
				conditions.Temp = response[0].To<double>();
				conditions.Humi = -1;
				conditions.Mode = (ChamberMode)Enum.Parse(typeof(ChamberMode), response[1].Trim());
				conditions.AlaramCount = response[2].To<int>();
			}
			else if (response.Length == 4)
			{
				conditions.Temp = response[0].To<double>();
				conditions.Humi = response[1].To<double>();
				conditions.Mode = (ChamberMode)Enum.Parse(typeof(ChamberMode), response[2].Trim());
				conditions.AlaramCount = response[3].To<int>();
			}

			return conditions;

		}


		/// <summary>
		/// 챔버 동작 programming
		/// </summary>
		/// <param name="chamberParams"></param>
		/// <returns></returns>
		public void RunRemoteProgram(ChamberRemoteParams chamberParams)
		{

			setmaskBit(3, 1);
			resetSRQ();

			var cmd = "RUN PRGM,";
			cmd += $"TEMP{chamberParams.StartTemp} GOTEMP{chamberParams.AttainmentTemp} ";
			if (chamberParams.DoHumiControl)
				cmd += $"HUMI{chamberParams.StartTemp} GOHUMI{chamberParams.AttainmentTemp} ";
			else
				cmd += "HUMIOFF ";
			cmd += $"TIME{makeTimeString(chamberParams.ExposeTime)} ";

			query(cmd);
		}


		/// <summary>
		/// Chamber를 StandBy 상태로 설정한다.
		/// </summary>
		public void StopRemoteProgram()
		{
			SetOperatingMode(ChamberMode.STANDBY);
			setmaskBit(3, 0);
			resetSRQ();
		}


		/// <summary>
		/// Chamber의 Operating Mode를 설정한다.
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="programNo"></param>
		public void SetOperatingMode(ChamberMode mode, int programNo = 0)
		{
			var cmd = $"MODE, {mode} ";
			if (mode == ChamberMode.RUN) cmd += programNo.ToString();

			query(cmd);
		}


		public ChamberRemoteParams GetRemotePrgSetup()
		{
			var param = new ChamberRemoteParams();
			var response = query("RUN PRGM?").Split(',');

			param.StartTemp = response[0].Trim().Replace("TEMP", "").To<double>();
			param.AttainmentTemp = response[1].Trim().Replace("GOTEMP", "").To<double>();
			if (response[2].Trim().Replace("HUMI", "").To<double>() > 0)
			{
				param.StartHumi = response[2].Trim().Replace("HUMI", "").To<double>();
				param.AttainmentHumi = response[3].Trim().Replace("GOHUMI", "").To<double>();
			}
			else
			{
				param.StartHumi = -1;
				param.AttainmentHumi = -1;
			}
			param.ExposeTime = transTimeStringToMinutes(response[4].Trim().Replace("TIME", ""));

			return param;
		}


		#endregion



		#region ==== private ====

		private void setmaskBit(int srqNo, int bit)
		{
			var response = query("MASK?");
			if (response == null) return;

			var bitChar = (char)('0' + bit);
			var newCharArr = response.Select((c, i) => i == (srqNo - 1) ? bitChar : c).ToArray();
			var newMask = new string(newCharArr);

			var cmd = $"MASK,{newMask}";
			query(cmd);
		}


		private void resetSRQ()
		{
			var cmd = "SRQ, RESET";
			query(cmd);
		}


		private static string makeTimeString(int timeMinutes)
		{
			string strTime = "";
			int hour, min;
			hour = (int)(timeMinutes / 60);
			min = timeMinutes % 60;

			strTime = $"{hour}:{min:00}";

			return strTime;
		}


		private int transTimeStringToMinutes(string strTime)
		{
			int ret = -1;
			var tempArr = strTime.Split(':');
			if (tempArr.Length == 1) ret = tempArr[0].To<int>();
			else if (tempArr.Length == 2) ret = tempArr[1].To<int>();

			return ret;
		}


		private int getSRQbit(int srqNo)
		{
			var cmd = "SRQ?";
			var response = query(cmd);
			if (response == null) return 0;

			return response.Substring(srqNo - 1, 1).To<int>();
		}


		private int getMaskBit(int srqNo)
		{
			var cmd = "MASK?";
			var response = query(cmd);
			if (response == null) return 0;

			return response.Substring(srqNo - 1, 1).To<int>();
		} 

		#endregion


	}
}
