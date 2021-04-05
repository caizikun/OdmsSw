using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Neon.Aligner;
using Free302.MyLibrary.Utility;
using Free302.TnM.Neon.Pigtail;
using Free302.TnMLibrary.DataAnalysis;


public class PdMeasureParam
{
	//Dut 측정 parameters
	public string[] ChipSerials;
	public int ChipWidth;                               //칩 간 간격

	public bool Alignment;                              //alignment. 
	public int OutAlign;
	public bool Approach;								//approach
	public int ApproachDistance;						//approach후 후진 거리
	public bool Measurement;                            //measurement.
	public int LapdOffset;
	public double lapdOffsetYaxis;
	public int LapdOffsetBack;							//Offset 이동 시 이동전 Z축 후진
	public int LapdOffsetReturn;                        //Offset 이동 시 이동후 Z축 전진

	public int endZDistance;							//측정 종료 후 Z축 후진 거리
	public bool CenterStage;

	public string saveFolderPath;
	public string saveName;
}



public class PdMeasureStatus
{
	//Dut 측정 정보 전달
	public Action<PdMeasureStatus> Reporter;

	public string CurrentSerial;
	internal Dictionary<double, double> dutIL;
}



public class PdMeasureLogic : IDisposable
{

	#region ==== members ====

	IPm mPD;
	public bool ConnectPD { get { if (mPD == null) return false; else return true; } }

	private Itls m_tls;
	private Istage mLeft;
	private Istage mRight;
	private Istage mOther;
	private IDispSensor mDistanceSensor;

	private AlignLogic mAlign;

	#endregion



	#region ==== Class Frameworks ====

	public PdMeasureLogic()
	{
		mLeft = CGlobal.LeftAligner;
		mRight = CGlobal.RightAligner;
		mOther = CGlobal.OtherAligner;
		mDistanceSensor = CGlobal.Ds2000;
		m_tls = CGlobal.Tls;
		mAlign = CGlobal.alignLogic;
		mRef = new Dictionary<double, double>();

		mThread = new Thread(run);

	}

	public void Dispose()
	{
		PowerDisplayOn = false;
		mThread = null;
	}

	#endregion



	#region ==== DAQ & ESM <PD> ====

	public void InitPD(PmAvgTime avgTime,
					   DaqPrimary daqprimary, Dictionary<PmCh, DaqAiCh> dicChToAi, Dictionary<DaqAiCh, double[]> dicAiToRange,
					   bool res, double[] responsivity, Dictionary<DaqAiCh, double> dicResistance)
	{
		///DAQ 초기화
		var pd = new DaqPm();

		pd.SetAddressInfo(daqprimary, dicChToAi, dicAiToRange, res);
		pd.SetDaqSystemInfo(responsivity, dicResistance);
		initPD_member(avgTime, pd);
	}


	public void InitPD(PmAvgTime avgTime, Keithley2401 pd, Dictionary<PmCh, PmSlotPort> dic, string[] address)
	{
		//Source Meter 초기화
		pd.SetAddressInfo((GpibPrimary)(address[0].To<int>()), GpibTimeout.T1s, (GpibPrimary)address[1].To<int>());
		pd.SetChMap(dic);
		pd.Open();
		initPD_member(avgTime, pd);
	}


	void initPD_member(PmAvgTime avgTime, IPm pd)
	{
		mPD = pd;
		mPD.WriteUnit(PmCh.CH1, PmPowerUnit.dBm);
		mPD.WriteAvgTime(PmCh.CH1, avgTime);

		mThread.Start();	//Power Display
	}
	

	double readPower(bool isdBm = true)
	{
		//파워 측정
		if (mPD == null) return double.NaN;

		var power = Math.Round(mPD.ReadPower(PmCh.CH1), 3);
		if (double.IsNaN(power)) power = -80;
		else if (power < -80) power = -80;

		if (!isdBm) power = Unit.dBmToMilliWatt(power);

		return power;
	}


	async Task<double> readPower(double wave)
	{
		//파장 변경 후 파워 측정
		if (mPD == null) return double.NaN;
		m_tls?.SetTlsWavelen(wave);
		await Task.Delay(100);
		return readPower();
	}


	public async Task<List<double>> ReadPower(double[] waveList)
	{
		//파장 Array 파워 측정
		List<double> power = new List<double>();

		for (int i = 0; i < waveList.Length; i++)
		{
			m_tls?.SetTlsWavelen(waveList[i]);
			await Task.Delay(100);
			power.Add(readPower());
		}

		return power;
	}

	#endregion



	#region ==== Power Display ====

	Thread mThread;
	public bool PowerDisplayOn { get; set; } = true;

	public Action<double> PowerReporter { get; set; }


	private async void run()
	{
		double power = double.NaN;

		while (true)
		{
			if (PowerDisplayOn)
			{
				power = readPower();
				PowerReporter?.Invoke(power);

				await Task.Delay(250); 
			}

		}
	}
	
	#endregion



	#region ==== Alignment ====

	public async Task doXySearchByLAPD()
	{
		//PD 광파워 값 사용 - 정렬
		var param = CGlobal.XySearchParamLeft;
		param.SearchByScan = true;
		Func<int, double> powerFunc = (x) => readPower(false);

		mAlign?.XySearch(param, powerFunc);
		await Task.Delay(100);
	}

	public async Task doXySearchOpm(bool left)
	{
		if (left) mAlign?.XySearch(CGlobal.XySearchParamLeft);
		else mAlign?.XySearch(CGlobal.XySearchParamRight);

		await Task.Delay(100);
	}

	#endregion



	#region ==== Reference ====

	private Dictionary<double, double> mRef;        //key : λ[nm], value : power[dBm]

	/// <summary>
	/// Ref파일 파일 경로
	/// </summary>
	public string RefFileName { get; set; } = $"{Directory.GetCurrentDirectory()}\\referencePD.txt";


	public void SetRefLambda(double[] wave)
	{
		//Ref 파장 설정
		mRef.Clear();
		for (int i = 0; i < wave.Length; i++) mRef.Add(wave[i], -60);
	}


	public void RecoredRef(double wave, double power)
	{
		//Ref 값 기록
		if (mRef.ContainsKey(wave)) mRef[wave] = power;
		else return;
				
	}


	public void SaveRefFile(string fileName = null)
	{
		//Ref 파일 저장
		var sb = new StringBuilder();
		var saveFileName = (fileName != null) ? fileName : RefFileName;
		if (File.Exists(saveFileName)) File.Delete(saveFileName);
		
		var lambda = mRef.Keys.ToArray();
		sb.Append("Wavelength");
		for (int i = 0; i < lambda.Length; i++) sb.Append($"\t{lambda[i]}");
		sb.AppendLine();
		sb.Append("RefPower");
		for (int i = 0; i < lambda.Length; i++) sb.Append($"\t{mRef[lambda[i]]}");
		
		using (var sw = new StreamWriter(saveFileName, false, Encoding.ASCII))
		{
			sw.Write(sb.ToString());
			sw.Close();
		}

	}

	public Dictionary<double,double> loadRefFile(string fileName)
	{
		//Ref 파일 데이터 Load
		mRef.Clear();
		var rawString = DutData.read(fileName);
		if (rawString.Count == 0) throw new Exception($"파일이 비었습니다.\n{fileName}");

		int waveIndex = 0, powerIndex = 0;

		for (int i = 0; i < rawString.Count; i++)
		{
			if (rawString[i][0].Trim() == "Wavelength") waveIndex = i;
			if (rawString[i][0].Trim() == "RefPower") powerIndex = i;
		}

		for (int i = 1; i < rawString[0].Length; i++)
			mRef.Add(rawString[waveIndex][i].To<double>(), rawString[powerIndex][i].To<double>());

		RefFileName = fileName;
		return mRef;
	}


	#endregion



	#region ==== DUT ====

	async Task<Dictionary<double, double>> measureDutIL()
	{
		//Dut 측정 -> IL계산
		var dutIL = new Dictionary<double, double>(mRef);
		var keys = new List<double>(dutIL.Keys);
		double power;

		foreach (var wave in keys)
		{
			power = await readPower(wave);
			dutIL[wave] += -power;
			dutIL[wave] *= -1;
			dutIL[wave] = Math.Round(dutIL[wave], 3);
		}

		return dutIL;
	}


	private async Task moveNextChip(int chipWidth, bool centerStage)
	{
		//[다음칩 이동]
		const int bufferDistance = 60;

		//stage open.
		mLeft?.RelMove(mLeft.AXIS_Z, bufferDistance * (-1));

		//x next chip
		if (centerStage)
		{
			mOther?.RelMove(CGlobal.CenterAxis, chipWidth);
			mOther?.WaitForIdle(CGlobal.CenterAxis);
		}
		else
		{
			mLeft?.RelMove(mLeft.AXIS_X, chipWidth);
			mLeft?.WaitForIdle(mLeft.AXIS_X);
			mRight?.RelMove(mRight.AXIS_X, chipWidth);
			mRight?.WaitForIdle(mRight.AXIS_X);
		}
		await Task.Delay(100);
	}


	private AlignPosition recordPos()
	{
		//stage 좌표값 가져오기(초기위치 파악)
		var coord = new AlignPosition();
		coord.In = mLeft?.GetAbsPositions();
		coord.Out = mRight?.GetAbsPositions();
		coord.Other = mOther?.GetAbsPositions();

		return coord;
	}


	private async Task moveInitPosition(AlignPosition initPos, bool isCenter, int bufferDistance = 10000)
	{
		//초기위치 이동
		if (mAlign == null) return;

		//Z축 이동
		mLeft?.RelMove(mLeft.AXIS_Z, -bufferDistance);
		mRight?.RelMove(mRight.AXIS_Z, -bufferDistance);
		mRight?.WaitForIdle(mRight.AXIS_Z);
		await Task.Delay(100);

		//Y축 이동
		mLeft?.AbsMove(mLeft.AXIS_Y, initPos.In.y);
		mRight?.AbsMove(mRight.AXIS_Y, initPos.Out.y);
		mRight?.WaitForIdle(mRight.AXIS_X);
		await Task.Delay(100);

		//X축 이동
		mLeft?.AbsMove(mLeft.AXIS_X, initPos.In.x);
		mRight?.AbsMove(mRight.AXIS_X, initPos.Out.x);
		mRight?.WaitForIdle(mRight.AXIS_X);
		await Task.Delay(100);

		//Center축 이동
		if (isCenter)
		{
			var pos = CGlobal.CenterAxis == mOther.AXIS_X ? initPos.Other.x : initPos.Other.y;
			mOther?.AbsMove(CGlobal.CenterAxis, pos);//center pos. 이동
			mOther?.WaitForIdle(CGlobal.CenterAxis);
		}

	}


	private void saveDutFile(Dictionary<string, Dictionary<double, double>> iLDic, string saveFileName)
	{
		//[Dut 측정 데이터 저장]
		var sb = new StringBuilder();

		var lambda = mRef.Keys.ToArray();
		sb.Append("Wavelength[nm]");
		for (int i = 0; i < lambda.Length; i++) sb.Append($"\t{lambda[i]}");
		sb.AppendLine();
		sb.Append("Ref.Power[dBm]");
		for (int i = 0; i < lambda.Length; i++) sb.Append($"\t{mRef[lambda[i]]}");
		sb.AppendLine();
		sb.AppendLine();

		//IL Data 기록
		foreach (var item in iLDic)
		{
			sb.Append($"{item.Key}");										//Chip Serial
			foreach (var IL in item.Value) sb.Append($"\t{IL.Value}");		//IL
			sb.AppendLine();
		}

		//save
		using (var sw = new StreamWriter(saveFileName, false, Encoding.ASCII))
		{
			sw.Write(sb.ToString());
			sw.Close();
		}

	}

	public static volatile bool mStop = false;


	public async Task DoMeasure(PdMeasureParam param, PdMeasureStatus status)
	{
		//[측정***]
		var ILDic = new Dictionary<string, Dictionary<double, double>>();							//Key : wavelength		value : IL
		var initPos = recordPos();                                                                  //초기 위치 기억

		for (int i = 0; i < param.ChipSerials.Length; i++)
		{
			status.CurrentSerial = param.ChipSerials[i];
			m_tls?.SetTlsWavelen(mRef.Keys.ToArray().Min());                                        //TLS 파장 변경

			if (param.Approach)                                                                     //approach
			{
				mAlign?.Approach(AppStageId.Left, 40, param.ApproachDistance);
				mAlign?.Approach(AppStageId.Right, 40, param.ApproachDistance); 
			}
			if (mStop) return;                                                                      //STOP Check
			if (param.Alignment)                                                                    //정렬
			{
				await doXySearchOpm(true);
				if (i % param.OutAlign == 0) await doXySearchOpm(false);
			}
			if (mStop) return;                                                                      //STOP Check
			if (param.LapdOffset != 0)                                                              //**LAPD로 이동  (output X축 이동)
			{
				mRight?.RelMove(mRight.AXIS_Z, -param.LapdOffsetBack);
				mRight?.WaitForIdle(mRight.AXIS_Z);
				mRight?.RelMove(mRight.AXIS_X, param.LapdOffset);
				mRight?.WaitForIdle(mRight.AXIS_X);
				mRight?.RelMove(mRight.AXIS_Y, param.lapdOffsetYaxis);
				mRight?.WaitForIdle(mRight.AXIS_Y);
				mRight?.RelMove(mRight.AXIS_Z, param.LapdOffsetReturn);
				mRight?.WaitForIdle(mRight.AXIS_Z);
			}
			if (mStop) return;                                                                      //STOP Check
			if (param.Measurement)                                                                  //측정!!!!!!!!
			{
				var dutIL = await measureDutIL();
				ILDic[param.ChipSerials[i]] = dutIL;

				status.dutIL = dutIL;
				status.Reporter(status);
			}
			if (mStop) return;                                                                      //STOP Check
			if (param.LapdOffset != 0)                                                              //**LAPD로 이동 - return FAB (output X축 이동)
			{
				mRight?.RelMove(mRight.AXIS_Z, -param.LapdOffsetBack);
				mRight?.WaitForIdle(mRight.AXIS_Z);
				mRight?.RelMove(mRight.AXIS_X, -param.LapdOffset);
				mRight?.WaitForIdle(mRight.AXIS_X);
				mRight?.RelMove(mRight.AXIS_Y, -param.lapdOffsetYaxis);
				mRight?.WaitForIdle(mRight.AXIS_Y);
			}
			if (param.ChipSerials.Length > 1 && i < param.ChipSerials.Length - 1)
				await moveNextChip(param.ChipWidth, param.CenterStage);                             //다음 칩 이동
		}

		if (mStop) return;

		var saveFileName = RawTextFile.BuildFileName(param.saveFolderPath, param.saveName);
		saveDutFile(ILDic, saveFileName);                                                           //Dut 측정 데이터 저장

		if(param.ChipSerials.Length > 1)
			await moveInitPosition(initPos, param.CenterStage, param.endZDistance);                 //초기위치로 이동

		m_tls?.SetTlsWavelen(mRef.Keys.ToArray().Min());											//TLS 파장 변경

	}

	#endregion


}

