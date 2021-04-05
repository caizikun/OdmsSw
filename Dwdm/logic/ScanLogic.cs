using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;
using Free302.TnM.Neon.Pigtail;
using Free302.TnM.DataAnalysis;
using Free302.TnM.Device;

namespace ScanTest
{
	#region ---- Data Types ----

	enum ScanAxisId { X1, X2, X3 }

	enum RangeMode { Full, PosDir, NegDir }

	class ScanParam
	{
		public Istage Aligner;
		public int Axis;
		public string AxisName { get { return axisNameDic[Axis]; } }
		public double Range;
		public double Step;
		public RangeMode RangeMode = RangeMode.Full;
		public int AvgTime_ms = 100;

		public string SaveFolder;
		public string SaveName;
		public bool DoSave = true;
		public bool SaveTime = true;

		public bool DoReturnOgigin = false;

		public int NumSteps => 1 + (int)Math.Floor((RangeMode == RangeMode.Full ? Range * 2 : Range) / Step);


		public ScanParam Clone()
		{
			var c = new ScanParam();
			c.Aligner = Aligner;
			c.Axis = Axis;
			c.Range = Range;
			c.Step = Step;
			c.RangeMode = RangeMode;
			c.AvgTime_ms = AvgTime_ms;

			c.SaveFolder = SaveFolder;
			c.SaveName = SaveName;
			c.DoSave = DoSave;
			c.SaveTime = SaveTime;

			c.DoReturnOgigin = DoReturnOgigin;

			return c;
		}


		Dictionary<int, string> axisNameDic;
		public ScanParam()
		{
			axisNameDic = Enum.GetValues(typeof(AlignAxis)).Cast<AlignAxis>()
									    .ToDictionary(t => (int)t, t => t.ToString());
		}


		public string buildFileName()
		{
			var fileName = SaveName.Length < 1 ? $"[{axisNameDic[Axis]}]" : SaveName;
			if (SaveTime) fileName = $"{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}";
			return fileName;
		}
		string buildFileName(Dictionary<string, double> nameDic, bool isTwoScanName)
		{
			var firstKey = nameDic.Keys.ToArray()[0];
			string fileName = "";
			if (SaveName.Length > 0) fileName = $"{SaveName}_";
			
			var index = 0;

			foreach (var item in nameDic)
			{
				if (item.Key == firstKey || (index == 1 && isTwoScanName))
					fileName += $"[{item.Key}]";
				else
					fileName += $"[{item.Key}{((item.Value >= 0) ? "  " : " ")}{item.Value}]";
				index += 1;
			}

			if (SaveTime) fileName = $"{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}";
			return fileName;
		}
		

		public string buildFilePath()
		{
			if (!Directory.Exists(SaveFolder)) Directory.CreateDirectory(SaveFolder);
			return FileNameBuilder.BuidlNewFileName(SaveFolder, buildFileName(), "txt");
		}
		public string buildFilePath(Dictionary<string, double> nameDic, bool isTwoScanName, string newFolder = null)
		{
			if (!Directory.Exists(newFolder ?? SaveFolder)) Directory.CreateDirectory(newFolder?? SaveFolder);
			return FileNameBuilder.BuidlNewFileName(newFolder, buildFileName(nameDic, isTwoScanName), "txt");
		}
	}

	
	class ScanStatus
	{
		public double[] Origin;         //scan 시작 위치
		public double[] Peak;           //scan 후 peak점
		public double[] Center;         //scna Center

		public string[] AxisName;
		public double[] RelativePos;    //scan 시작점(=0)과의 상대위치
		public double[] CurrentPos;     //현재위치

		public ScanStatus(string[] scanAxis)
		{
			var count = scanAxis.Length;
			AxisName = scanAxis;

			Origin = new double[count];
			Peak = new double[count];
			Center = new double[count];

			RelativePos = new double[count];
			CurrentPos = new double[count];

			for (int i = 0; i < count; i++)
			{
				Origin[i] = double.NaN; Peak[i] = double.NaN; Center[i] = double.NaN;
				RelativePos[i] = 0; CurrentPos[i] = double.NaN;
			}
		}

		public Action<ScanStatus> Reporter;

	}

	#endregion


	/// <summary>
	/// ScanForm 대응
	/// 2018-04-24 br DrBae
	/// </summary>
	class ScanLogic
	{
		public static volatile bool mStop = false;

		public static async Task<Tuple<double[], double[]>> runScan(List<ScanParam> scanParams, Func<double> powerReader, ScanStatus status)
		{
			if (scanParams.Count == 0) return null;
			//[scan info]
			var numDp = scanParams.Last().NumSteps;
			var aligner = scanParams.Last().Aligner;
			var axis = scanParams.Last().Axis;
			var step = scanParams.Last().Step * (scanParams.Last().RangeMode == RangeMode.NegDir ? -1 : 1);
			//[save file info]
			var fileNameDic = new Dictionary<string, double>();
			for (int i = 0; i < status.AxisName.Length; i++) fileNameDic.Add(status.AxisName[i], status.RelativePos[i]);
			var filePathOrigin = scanParams[0].SaveFolder;
			var filePathBackup = Path.Combine(scanParams.First().SaveFolder, "backup");
			var fileName1Axis = scanParams.First().buildFilePath(fileNameDic, false, (status.RelativePos.Length > 1) ? filePathBackup : filePathOrigin);
			var fileName2Axis = scanParams.First().buildFilePath(fileNameDic, true, filePathOrigin);
			//[2축 이상 Scan시]
			var scanParams2 = new List<ScanParam>(scanParams);
			scanParams2.RemoveAt(scanParams2.Count - 1);
			//[scan Data 저장공간]
			var coordBuffer = new double[numDp];
			var powerBuffer = new double[numDp];

			//원점 위치
			status.Origin[scanParams.Count - 1] = status.CurrentPos[scanParams.Count - 1] = aligner.GetAxisAbsPos(axis);
			status.RelativePos[scanParams.Count - 1] = 0;

			//scan 시작위치로 이동
			if (scanParams.Last().RangeMode == RangeMode.Full)
			{
				await MoveAs(aligner, axis, -scanParams.Last().Range);
				status.RelativePos[scanParams.Count - 1] = -scanParams.Last().Range;
			}

			//이동 방향 조정
			var preStep = 10 * (scanParams.Last().RangeMode == RangeMode.NegDir ? +1 : -1);
			await MoveAs(aligner, axis, preStep);
			await MoveAs(aligner, axis, -preStep);
			await Task.Delay(250);
			
			//Scan 시작
			for (int i = 0; i < numDp; i++)
			{
				//get data
				powerBuffer[i] = powerReader?.Invoke() ?? randomPower(scanParams[0].AvgTime_ms);            //파워
				coordBuffer[i] = status.CurrentPos[scanParams.Count - 1] = aligner.GetAxisAbsPos(axis);		//위치
				status.Reporter(status);

				//**** 1축 Scan [재귀 호출]****
				if (scanParams2.Count >= 1)
				{
					var data = await runScan(scanParams2, powerReader, status);

					//Save [2축 데이터 저장]
					if (scanParams2.Count == 1)
					{
						if (i == 0) Save(fileName2Axis, data.Item1);       //첫 행 = 좌표
						Save(fileName2Axis, data.Item2, coordBuffer[i]);   //X2, 파워 
					}
				}

				//move next [현재 축 다음 step 이동]
				await MoveAs(aligner, axis, step);

				//상대좌표 update
				if (i < numDp - 1) status.RelativePos[scanParams.Count - 1] = Math.Round(status.RelativePos[scanParams.Count - 1] + step, 3);

				//check stop signal
				if (mStop) break;
			}

			//Save [1축 저장]
			if (scanParams2.Count == 0) Save(fileName1Axis, coordBuffer, powerBuffer);

			//원점 이동
			if (scanParams.First().DoReturnOgigin)
			{
				var dist = (double)((decimal)status.Origin[scanParams.Count - 1] - (decimal)aligner.GetAxisAbsPos(axis));
				await MoveAs(aligner, axis, dist);
				status.CurrentPos[scanParams.Count - 1] = aligner.GetAxisAbsPos(axis);
				status.RelativePos[scanParams.Count - 1] = status.Origin[scanParams.Count - 1];
				status.Reporter(status);
			}

			//상태 갱신
			status.Peak[scanParams.Count - 1] = findPeak(coordBuffer, powerBuffer);
			status.Center[scanParams.Count - 1] = findCenter(coordBuffer, powerBuffer);

			return new Tuple<double[], double[]>(coordBuffer, powerBuffer);
		}
		
		

		#region ---- Util ----

		static double findPeak(double[] coord, double[] power)
		{
			//첫번째 최대값 위치
			var i = Array.IndexOf(power, power.Max());
			return coord[i];
		}
		static double findCenter(double[] coord, double[] power)
		{
			var pw = power.Max() - 1;
			var i1 = Array.FindIndex(power, x => x >= pw);
			var i2 = Array.FindLastIndex(power, x => x >= pw);
			var i = (int)Math.Round((i1 + i2) / 2.0);
			return coord[i];
		}

		static Random random = new Random(DateTime.Now.Millisecond);
		static double randomPower(int avgTime_ms)
		{
			var dBm = Math.Round(Unit.MillWatt2Dbm(0.01 + random.NextDouble()),3);
			Task.Delay(avgTime_ms).Wait();
			return dBm;
		}


		public static async Task<double> MoveAs(Istage aligner, int axis, double displacement_um)
		{
			if (aligner == null) return double.NaN;

			if (displacement_um > 3000 || displacement_um < -3000)
			{
				var ok = MessageBox.Show("이동거리가 1000um를 넘습니다. 이동할까요?", "레알?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
				if (ok != DialogResult.OK) return double.NaN;
			}

			aligner.RelMove(axis, displacement_um);
			for (int i = 0; i < 300; i++)
			{
				if (aligner.IsMovingOK(axis)) await Task.Delay(250);
				else break;
			}
			if (aligner.GetType().Name == "TestStage") await Task.Delay(100);

			return aligner.GetAxisAbsPos(axis);
		}

		#endregion



		#region ---- File Save ----

		/// <summary>
		/// 2축 스캔시 1회 스캔 결과 저장
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="dataArr">파워 또는 좌표</param>
		/// <param name="coordX2">디폴트 시 <paramref name="dataArr"/>는 X1 좌표로 해석</param>
		public static void Save(string filePath, double[] dataArr, double coordX2 = double.NaN)
		{
			var sb = new StringBuilder(dataArr.Length * 20);

			using (var sw = new StreamWriter(filePath, true))
			{
				sb.Append(double.IsNaN(coordX2) ? "X1X2\t" : $"{coordX2}\t");
				for (int i = 0; i < dataArr.Length; i++) sb.Append($"{dataArr[i]}\t");
				sb.Remove(sb.Length - 1, 1);
				sw.WriteLine(sb.ToString());
				sw.Close();
			}
		}

		/// <summary>
		/// 1축 스캔 결과 저장
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="coordX"></param>
		/// <param name="power"></param>
		public static void Save(string filePath, double[] coordX, double[] power)
		{
			var sb = new StringBuilder(50);

			using (var sw = new StreamWriter(filePath, false))
			{
				for (int i = 0; i < coordX.Length; i++) sb.AppendLine($"{coordX[i]:F04}\t{power[i]}");
				sw.Write(sb.ToString());
				sw.Close();
			}
		}

		#endregion

	}//class


	public class TestStage : Istage
	{
		public int AXIS_X => (int)AlignAxis.X;

		public int AXIS_Y => (int)AlignAxis.Y;

		public int AXIS_Z => (int)AlignAxis.Z;

		public int AXIS_ThetaX => (int)AlignAxis.Tx;

		public int AXIS_ThetaY => (int)AlignAxis.Ty;

		public int AXIS_ThetaZ => (int)AlignAxis.Tz;

		public int AXIS_ALL => (int)AlignAxis.All;

		public int AXIS_U => (int)AlignAxis.Tx;

		public int AXIS_V => (int)AlignAxis.Ty;

		public int AXIS_W => (int)AlignAxis.Tz;

		public int AXIS_TX => (int)AlignAxis.Tx;

		public int AXIS_TY => (int)AlignAxis.Ty;

		public int AXIS_TZ => (int)AlignAxis.Tz;

		public int MOVESPEED_SLOW => 0;

		public int MOVESPEED_MID => 1;

		public int MOVESPEED_FAST => 2;

		public int DIRECTION_MINUS => 0;

		public int DIRECTION_PLUS => 1;

		public int stageNo { get; protected set; }

		static int mAlignerCounter = 0;
		Dictionary<int, double> mStagePos;

		public TestStage(int numAxis = 6)
		{
			stageNo = ++mAlignerCounter;
			mStagePos = new Dictionary<int, double>()
			{ {AXIS_X, 0 }, { AXIS_Y, 0 }, { AXIS_Z, 0 }, { AXIS_TX, 0 }, { AXIS_TY, 0 }, { AXIS_TZ, 0 } };
		}


		public bool AbsMove(int _axis, double _pos, int _speed)
		{
			return AbsMove(_axis, _pos);
		}

		public bool AbsMove(int _axis, double _pos)
		{
			mStagePos[_axis] = _pos;
			return true;
		}

		public CStageAbsPos GetAbsPositions()
		{
			var pos = new CStageAbsPos();
			pos.SetValue(mStagePos.Values.ToArray());

			return pos;
		}

		public double GetAxisAbsPos(int _axis)
		{
			return mStagePos[_axis];
		}

		public bool Homing(int _axis)
		{
			return true;
		}

		public bool Homing()
		{
			return true;
		}

		public bool IsConnectedOK()
		{
			return true;
		}

		public bool IsMovingOK()
		{
			return false;
		}

		public bool IsMovingOK(int _axis)
		{
			return false;
		}

		public bool RelMove(int _axis, double _dist, int _speed)
		{
			return RelMove(_axis, _dist);
		}

		public bool RelMove(int _axis, double _dist)
		{
			mStagePos[_axis] += _dist;
			return true;
		}

		public void StopMove(int _axis)
		{
			
		}

		public void StopMove()
		{
			
		}

		public void WaitForIdle(int _axis)
		{
			
		}

		public void WaitForIdle()
		{
			
		}

		public bool Zeroing(int _axis)
		{
			throw new NotImplementedException();
		}

		public bool Zeroing()
		{
			throw new NotImplementedException();
		}
	}
	
}