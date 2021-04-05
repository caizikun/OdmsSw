using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Neon.Aligner;
using System.Windows.Forms;
using Free302.TnM.DataAnalysis;
using System.IO;
using System.Text;
using Universe.TcpServer;
using DrBAE.TnM.TcpLib;
using PS = Universe.TcpServer.PolState;

public class SweepLogicDwdm
{



    #region ==== Class Freamwork ====

    private Itls mTls;
    private IoptMultimeter mOpm;
    private IpolController mOpc;
	private bool mUsingOpc = true;

    private List<double> mWaves;                            //calibrated(equal interval)
    private List<PortPowers> mPortPowerList;                //calibrated(equal interval , stiching)

    public Action<string, int, int, int> mReporter;         //msg, gain, pol, port

    public SweepLogicDwdm(Itls tls, IoptMultimeter opm, IpolController opc) : this(tls, opm)
    {
        mOpc = opc;
		mUsingOpc = CGlobal.mUsingOpc && opc != null;
	}

    public SweepLogicDwdm(Itls tls, IoptMultimeter opm)
    {
        mTls = tls;
        mOpm = opm;
		mUsingOpc = false;
	}

    #endregion



    #region ==== TCP Server ====


    ITcpAgentClient mTcp;
    bool mUsingLocalTls = false;
    bool mUsingLocalPC = false;

    public bool InitTcpServer(bool usingLocalTls, bool usingLocalPC)//out TcpAgentClient tcp)
    {
        try
        {
            mUsingLocalTls = usingLocalTls;
            mUsingLocalPC = usingLocalPC;

            if (!usingLocalTls)
            {
                mTcp = new Client((x) => { }, (x) => { });
                mTcp.Connect();
            }
            //tcp = mTcp;
            return mOpm != null && (!usingLocalTls || mTls != null);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"SweepLogic.Init(): \n{ex.Message}\n\n설정파일 존재여부나 TLS서버 네트웍크 설정을 확인하세요");
            //tcp = null;
            return false;
        }
    }

    public async Task Register(bool register)
    {
        var msg = register ? "Registering" : "UnRegistering";
        mReporter?.Invoke($"SweepLogic.Register: {msg}", 0, 0, 0);
        if (register) mTcp.Register();
        else mTcp.UnRegister();
    }

    #endregion



    #region ==== Polarization Control ====

    //public bool mIsOBand;
    public bool mIsLeftCircular;
    public bool mIsNegitiveDiagonal;//-45도 편광

    /// <summary>
    /// Polarizaton States
    /// </summary>
    enum PolState { Unknown, H, V, Dp, R, L, Dn }//Dp=diagonal positive, Dn=diagonal negative
    PolState mCurrentPolState;// = PolState.Unknown;

	/// <summary>
	/// find setting pol state
	/// </summary>
	/// <param name="pol"></param>
	/// <param name="baseAngle"></param>
	Action<double> getLocalPolSetter(int pol)
    {
        if (mOpc == null) return null;
		Action<double> setter = null;

		switch (pol)
        {
            case 0:
				setter = mOpc.SetToLinearHorizontal;
				mCurrentPolState = PolState.H;
				break;

            case 1:
				setter = mOpc.SetToLinearVertical;
                mCurrentPolState = PolState.V;
                break;

            case 2:
                if (mIsNegitiveDiagonal)
                {
                    mCurrentPolState = PolState.Dn;
					if (CGlobal.mPcType != CGlobal.PcType.C_8169a) setter = mOpc.SetToNegaLinearDiagonal;
					else
					{
						var msg = "8169 C-band는 -45º Pol을 구현 안함.";
						MyLogic.log(msg);
						throw new Exception(msg);
					}
                }
                else
                {
					setter = mOpc.SetToLinearDiagonal;
                    mCurrentPolState = PolState.Dp;
                }
                break;

            case 3:
                if (mIsLeftCircular)
                {
                    mCurrentPolState = PolState.L;
                    if (CGlobal.mPcType != CGlobal.PcType.C_8169a) setter = mOpc.SetToLHcircular;
                    else
                    {
						var msg = "8169 C-band는 Left Circular Pol을 구현 안함.";
						MyLogic.log(msg);
						throw new Exception(msg);
                    }
                }
                else
                {
					setter = mOpc.SetToRHcircular;
					mCurrentPolState = PolState.R;
				}
                break;
        }
		return setter;
    }

    public void SetObandPol(int pol)
    {
        if (!mUsingLocalPC) return;

        if (mOpc == null) return;
        if (pol == 0) mOpc.SetToLinearHorizontal(0);
        if (pol == 1) mOpc.SetToLinearVertical(0);

        if (pol == 2) mOpc.SetToLinearDiagonal(0);
        if (pol == 3) mOpc.SetToNegaLinearDiagonal(0);

        if (pol == 4) mOpc.SetToRHcircular(0);
        if (pol == 5) mOpc.SetToLHcircular(0);
    }
    
    public Action<double, double> mFindPolReporter;

    public async Task<double[]> FindMaxPolPos(int port)
    {
		if (CGlobal.mPcType != CGlobal.PcType.C_8169a) return new double[] { 0.0, 0.0 };

		double pos = 0.0;
        double maxPos = 0.0;
        double step = 2.0;
        double maxPwr = double.MinValue;

        int delay = 100;

        if (!mUsingLocalPC)//TCP Server mode
		{
            try
            {
                mTcp.BeginAlign();

                while (pos < 180)
                {
                    //set lambda plate
                    mTcp.Pc((int)pos, PS.POLARIZER);

                    double pwr = mOpm.ReadPower(port);

                    mFindPolReporter?.Invoke(pos, Math.Round(Unit.MillWatt2Dbm(pwr), 2));

                    if (pwr > maxPwr)
                    {
                        maxPwr = pwr;
                        maxPos = pos;
                    }
                    pos = Math.Round(pos + step);
                }
                mTcp.Pc((int)maxPos, PS.LHP);
            }
            finally
            {
                mTcp.EndAlign();
            }
        }
		else//Local mode
		{
            while (pos < 180)
            {
                //set lambda plate
                mOpc.SetPolFilterPos(pos);
                Thread.Sleep(delay);///160729 by DrBae

                double pwr = mOpm.ReadPower(port);

                mFindPolReporter?.Invoke(pos, Math.Round(Unit.MillWatt2Dbm(pwr), 2));

                if (pwr > maxPwr)
                {
                    maxPwr = pwr;
                    maxPos = pos;
                }
                pos = Math.Round(pos + step);
            }

            mOpc.SetPolFilterPos(maxPos);
            Thread.Sleep(delay);
            mOpc.SetQuarRetarderPos(maxPos);
            Thread.Sleep(delay);
            mOpc.SetHalfRetarderPos(maxPos);
            Thread.Sleep(delay);

        }
        
        return new double[] { maxPos, Math.Round(Unit.MillWatt2Dbm(maxPwr), 2) };
    }

    #endregion



    #region ==== prepare to Sweep ====

    int mStartWave, mStopWave, mNumDataPoint;
    double mStepWave;

	public int TlsWaveShift_pm { get; set; } = -100;

    void initSweepParam(int start_nm, int stop_nm, double step_nm)
    {
        mStartWave = start_nm;
        mStopWave = stop_nm;
        mStepWave = step_nm;
        mNumDataPoint = 1 + (int)Math.Floor((stop_nm - start_nm) / step_nm);

        mWaves = Enumerable.Range(0, mNumDataPoint).Select(x => Math.Round(mStartWave + mStepWave * x, 3)).ToList();
    }

    void setSweepMode(int[] ports, int _startWave, int _stopWave, double _stepWave)
    {
        initSweepParam(_startWave, _stopWave, _stepWave);

        //set device.
        if (mUsingLocalTls) mTls.SetTlsSweepRange(_startWave, _stopWave, _stepWave);
        mOpm.SetPdSweepMode(ports, _startWave, _stopWave, _stepWave);
    }

    #endregion



    #region ==== Sweep ====


	public void SetPmGain(int[] ports,int gain)
	{
		mOpm.SetGainLevel(ports, gain);
	}

	public bool DoWaveBackup { get; set; } = false;
	public bool DoPowerBackup { get; set; } = false;

	public string DutSN { get; set; } = "_unknown_dut_";

	async Task doSweep(int[] ports, int[] pmGains, double polBaseAngle)
    {
        mReporter?.Invoke($"Starting ExecSweep()>", 0, 0, 0);

        #region ---- buffer ----

        //clear buffer
        if (mPortPowerList != null) mPortPowerList.Clear();
        else mPortPowerList = new List<PortPowers>();

        int numGains = pmGains.Length;
        int numPorts = ports.Length;// mPm.portCnt;

		//data buffer
		var numPols = mUsingOpc ? 4 : 1;
        PortPowerRaw[][][] dataGainPolPort = new PortPowerRaw[numGains][][];
        for (int g = 0; g < numGains; g++)
        {
            dataGainPolPort[g] = new PortPowerRaw[numPols][];
            for (int pol = 0; pol < numPols; pol++) dataGainPolPort[g][pol] = new PortPowerRaw[numPorts];
        }

        #endregion

        //polarization filter pos. 설정.
		if (mUsingOpc && mUsingLocalPC && CGlobal.mPcType != CGlobal.PcType.C_8169a)
		{
			mReporter?.Invoke($"Setting polarizaton base angle = <{polBaseAngle}>", 0, 0, 0);
			mOpc?.SetPolFilterPos(polBaseAngle);
		}

		//var _min = 1e-9;
		
        #region ---- Sweep ----

        List<double> logWave = null;		

        //Sweep하고 데이터를 획득한다.
        for (int g = 0; g < numGains; g++)//for each gain level
        {
            //gain level 설정.
            mReporter?.Invoke("Setting gain level", g + 1, 0, 0);
            mOpm.SetGainLevel(ports, pmGains[g]);

            try
            {
				//TCP server Begin 
				/// TODO: Non-Pol 일경우 필요??
				if (!mUsingLocalTls && mUsingOpc) mTcp.BeginAlign();

                for (int pol = 0; pol < numPols; pol++)//for each pol.
                {
					//polarization 설정.
					if (mUsingOpc)
					{
						if (mUsingLocalPC)
						{
							var mPolSetter = getLocalPolSetter(pol);
							mReporter?.Invoke($"Setting polarization state <{mCurrentPolState}>", g + 1, pol + 1, 0);
							mPolSetter?.Invoke(polBaseAngle);
						}
						else
						{
							mCurrentPolState = (PolState)(pol + 1);
							mReporter?.Invoke($"Setting polarization state <{mCurrentPolState}>", g + 1, pol + 1, 0);
							mTcp.Pc((int)polBaseAngle, (PS)pol);
						}
					}

					mOpm.SetPdLogMode(ports);//180504 Tcp Ready 순서 변경
					
					//TLS sweep ready
					if (!mUsingLocalTls) mTcp.Ready();

                    //start Sweep
                    if (mUsingLocalTls)
                    {
                        mReporter?.Invoke($"Setting tls log on", g + 1, pol + 1, 0);
                        mTls.TlsLogOn();

                        mReporter?.Invoke("Starting tls sweep", g + 1, pol + 1, 0);
                        mTls.ExecTlsSweepCont();
                        while (mTls.IsTlsSweepOperating()) Thread.Sleep(200);

                        //read tls
                        mReporter?.Invoke("Reading wavelength data from TLS", g + 1, pol + 1, 0);
                        logWave = mTls.GetTlsWavelenLog();
					}
                    else
                    {
                        //Sweep (TCP Server)
                        logWave = (mTcp.GetData()).ToList();//unit=meter
					}

					shiftWaves(logWave, TlsWaveShift_pm * 1e-3);//pm 2 nm
					if (DoWaveBackup) backupWave(DutSN, pmGains[g], mCurrentPolState, logWave);

					//read pm
					mReporter?.Invoke("Reading power data from OPM", g + 1, pol + 1, 0);
					//Trigger Check!
					

                    var rawData = new List<PortPowerRaw>();
                    for (int i = 0; i < ports.Length; i++)
                    {
                        mReporter?.Invoke($"Reading power data from OPM : port=<{ports[i]}>", g + 1, pol + 1, 0);
                        var p = new PortPowerRaw(ports[i], mOpm.GetPwrLog(ports[i]));
                        chopPower_mW(p, pmGains[g]);

                        rawData.Add(p);
                    }

                    //Logging Mode 해지...
                    mReporter?.Invoke("Setting tls, pm log off", g + 1, pol + 1, 0);
                    if (mUsingLocalTls) mTls.TlsLogOff();
                    mOpm.StopPdLogMode(ports);


					//backup Powers
					if (DoPowerBackup) backupPower(DutSN, pmGains[g], mCurrentPolState, rawData);

                    //logged power data를 등간격으로 만들어 준다.
                    mReporter?.Invoke("Normalizing wave-power data", g + 1, pol + 1, 0);
					foreach (var raw in rawData)
					{
						raw.power = applyLinearInterpolation(mStartWave, mStepWave, logWave, raw.power);
						chopPower_mW(raw, pmGains[0]);
					}
					//assign data
					dataGainPolPort[g][pol] = rawData.ToArray();
                }// for pol                
            }
            finally
            {
                //TCP Server End
                if (!mUsingLocalTls) mTcp.EndAlign();
			}
            

        }// for gain

        #endregion


        //reset PM gain level
        mReporter?.Invoke("Setting gain level to <1>", 0, 0, 0);
        if (numGains > 1) mOpm.SetGainLevel(pmGains[0]);


        #region //gain data merging & transform buffer structure

        mReporter?.Invoke("Merging gain data", 0, 0, 0);
        for (int p = 0; p < numPorts; p++)//for each port
        {
            PortPowers stitchedData = new PortPowers(ports[p], numPols);//one port, 4 pol

			for (int pol = 0; pol < numPols; pol++)
            {
                if (numGains < 2) stitchedData[pol] = dataGainPolPort[0][pol][p].power;
                else
                {
                    var merge = new DataMerging(pmGains[1], 2, DataMerging.DataUnit.MilliWatt);
                    var data = merge.Merge(dataGainPolPort[0][pol][p].power.ToArray(), dataGainPolPort[1][pol][p].power.ToArray());
                    stitchedData[pol] = data.ToList();
                }
            }
            mPortPowerList.Add(stitchedData);//each port

        }//for each port
        #endregion


        //메모리 해제...
        dataGainPolPort = null;
        mReporter?.Invoke("Finishing ExecSweep()", 0, 0, 0);
    }

	static void backupWave(string dutSn, int gain, PolState polState, List<double> waves)
	{		
		StreamWriter sw = null;
		try
		{
			var file = buildBackupFilePath(BackupFileNameWave, dutSn, gain, polState);
			sw = new StreamWriter(file);
			var sb = new StringBuilder(waves.Count * 20);
			for (int i = 0; i < waves.Count; i++) sb.AppendLine(waves[i].ToString());
			sw.Write(sb.ToString());
		}
		finally
		{
			sw?.Close();
		}
	}
	const string BackupFolder = "backup";
	const string BackupFileNameWave = "wave";
	private static string buildBackupFilePath(string fileName, string dutSn, int gain, PolState polState)
	{
		DateTime d = DateTime.Now;
		var time = $"{ d.ToString("yyyy-MM-dd") }_{ d.ToString("HH-mm-ss")}";

		//check folder
		string folder = Path.Combine(Application.StartupPath, BackupFolder);
		if (File.Exists(folder)) File.Delete(folder);
		if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

		//build file path
		string file = Path.Combine(Application.StartupPath, BackupFolder,
			$"{dutSn}_{fileName}_gain={gain}_pol={polState}_{time}.txt");
		return file;
	}

	const string BackupFileNamePower = "power";
	static void backupPower(string dutSn, int gain, PolState polState, List<PortPowerRaw> powers)
	{
		StreamWriter sw = null;
		try
		{
			if (powers == null || powers.Count == 0) throw new Exception($"backupPower() : null powers or Zero count");
			if(powers[0] == null || powers[0].power == null) throw new Exception($"backupPower() : null powers[0] or null power[0].power");

			var file = buildBackupFilePath(BackupFileNamePower, dutSn, gain, polState);
			sw = new StreamWriter(file);
			var sb = new StringBuilder(20 * powers.Count);

			var numDp = powers[0].power.Count;

			for (int ch = 0; ch < powers.Count; ch++) sb.Append($"{powers[ch].port}\t");
			sb.Remove(sb.Length - 1, 1);
			sw.WriteLine(sb.ToString());

			for (int w = 0; w < numDp; w++)
			{
				sb.Clear();
				for (int ch = 0; ch < powers.Count; ch++) sb.Append($"{powers[ch].power[w]}\t");
				sb.Remove(sb.Length - 1, 1);
				sw.WriteLine(sb.ToString());
			}
		}
		finally
		{
			sw?.Close();
		}
	}


	void shiftWaves(List<double> data, double delta_meter)
	{
		for (int i = 0; i < data.Count; i++) data[i] += delta_meter;
	}

    void chopPower_mW(PortPowerRaw rawData, double gainLevel_dBm)
    {
        double limitUp = Unit.Dbm2MilliWatt(gainLevel_dBm + 10);               // +10dB 보다 큰값을 자르기
        double limitLo = Unit.Dbm2MilliWatt(gainLevel_dBm - 70);               // -70dB 보다 작은값을 자르기

        for (int i = 0; i < rawData.power.Count; i++)
        {
            if (rawData.power[i] > limitUp) rawData.power[i] = limitUp;//0dBm
            else if (rawData.power[i] < limitLo) rawData.power[i] = limitLo;//-80dBm
			else if (double.IsNaN(rawData.power[i])) rawData.power[i] = limitLo;
		}
    }

    static List<double> applyLinearInterpolation(double waveStart, double waveStep, List<double> waveLog, List<double> powerLog)
    {
        const int numDigitWave = 8;     // 0.1pm
        const int numDigitPower = 11;    // 10^-11 mW ~ -110dBm

        List<double> data = new List<double>();

        int numDataPoint = waveLog.Count();
        double xp = waveStart;
        for (int i = 0; i < numDataPoint; i++)
        {
            //next wavelength.
            xp = Math.Round(waveStart + i * waveStep, numDigitWave);

            //find index.
            int index = waveLog.BinarySearch(xp);

            //positive ~ exact λ matched
            if (index >= 0)
            {
                data.Add(Math.Round(powerLog[index], numDigitPower));
                continue;
            }

            index = ~index;
            if (index == 0) index++;
            if (index == numDataPoint) index--;

            try
            {
                double x1 = Math.Round(waveLog[index - 1], numDigitWave);
                double x2 = Math.Round(waveLog[index], numDigitWave);
                if (x2 == x1) x2 = Math.Round(x1 + waveStep, numDigitWave);

                double y1 = Math.Round(powerLog[index - 1], numDigitPower);
                double y2 = Math.Round(powerLog[index], numDigitPower);

                //if (x1 == x2) //never raised case
                var yp = Math.Round(y1 + (xp - x1) * (y2 - y1) / (x2 - x1), numDigitPower);
                //if (yp < 0.00000001) yp = 0.00000001;//-80dBm
				//if (yp > 2) yp = 2.0;//+3dBm
													 ///
													 /// y2 이후 데이터 y3, y4 등이 y2와 같을 경우 == 부정확함.
													 ///
				data.Add(yp);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                var msg = ex.Message;
            }
        }

        return data;
    }    


    #endregion



    #region ==== Measure Power & DUT ====

    public async Task<DutData> MeasureDut(int[] ports, int[] gainLevels, 
										  int waveStart, int waveStop, double waveStep_nm, 
										  ReferenceData refData, 
										  int tlsPower_dBm, int tlsSNR_dB, int noiseShift_dB)
    {
        //sweep 
        var logPower = await MeasurePower(ports, gainLevels, waveStart, waveStop, waveStep_nm, refData.PolBaseAngle);


        //noise Power
        //var noisePower_mW = Unit.Dbm2MilliWatt(noisePower_dBm);
        //for (int i = 0; i < logPower.Count; i++)
        //    logPower[i].Subtract(noisePower_mW);

		//noise Shift
		var shifter = new NoiseShifter
		{
			TlsPower_dBm = tlsPower_dBm,
			TlsSNR_dB = tlsSNR_dB,
			Shift_dB = noiseShift_dB
		};
		for (int i = 0; i < logPower.Count; i++)
			shifter.Transform(logPower[i].Data);

		mReporter?.Invoke($"Measurement(): calcualting transmittance", 0, 0, 0);
		var transData = calcTrans(logPower, refData);

        return transData;
    }

    public async Task<List<PortPowers>> MeasurePower(
		int[] ports, int[] gainLevels, int waveStart, int waveStop, double waveStep_nm, double polBaseAngle)
    {
        //sweep 
        mReporter?.Invoke($"Running doMeasurement() - SetSweepMode()", 0, 0, 0);
        setSweepMode(ports, waveStart, waveStop, waveStep_nm);

        mReporter?.Invoke($"Running doMeasurement() - ExecSweepPol()", 0, 0, 0);
        await doSweep(ports, gainLevels, polBaseAngle);

        mReporter?.Invoke($"Running doMeasurement() - StopSweepMode()", 0, 0, 0);
        mOpm.StopPdSweepMode();

        return mPortPowerList;
    }


    DutData calcTrans(List<PortPowers> dataPower, ReferenceData refData)
    {
        mReporter?.Invoke($"Running calcTransmitance()", 0, 0, 0);

        if (dataPower == null) throw new ArgumentNullException($"calcTransmitance(): dataPower == null");
        if (dataPower.Count == 0) throw new ArgumentOutOfRangeException($"calcTransmitance(): 입력 데이터의 길이가 0입니다.");

        int numPorts = dataPower.Count;
        int numDataPoints = dataPower[0][0].Count();
		var numPols = mUsingOpc ? 4 : 1;

        DutData data = new DutData(mStartWave, mStopWave, Math.Round(mStepWave, 3));
        data.mPower = dataPower;

        foreach (var portPower in dataPower)
        {
            int port = portPower.Port;

            mReporter?.Invoke($"Running calcTransmitance() :", 0, 0, port);

			var portLoss = new PortPowers(port, numPols);

			double wavelen = mStartWave;
			for (int j = 0; j < numDataPoints; j++)
			{
				//get REF power at the wave
				var refPower = refData.GetPowerAtWave(port, wavelen);

				if (numPols == 4)
				{
					//DUT power
					//var outPwrs = new double[numPols];
					//portPower.Data[j].CopyTo(outPwrs, 0);
                    var outPwrs = new double[] { portPower[0][j], portPower[1][j], portPower[2][j], portPower[3][j] };

					//calculate.
					double Tmax, Tmin;//Math.Abs 제거
					Mueller.CalcMaxMin(refPower, outPwrs, out Tmax, out Tmin);
					if (Tmax <= 1e-7) Tmax = 1e-7;
					if (Tmax >= 2) Tmax = 2;
					if (double.IsNaN(Tmax)) Tmax = 1e-7;
					if (Tmin <= 1e-7) Tmin = 1e-7;
					if (Tmin >= 2) Tmin = 2;
					if (double.IsNaN(Tmin)) Tmin = 1e-7;

					var dbMax = Math.Round(10 * Math.Log10(Tmax), 3);
					var dbMin = Math.Round(10 * Math.Log10(Tmin), 3);
					portLoss.Max.Add(dbMax);
					portLoss.Min.Add(dbMin);
				}
				else
				{
					var T = portPower.NonPol[j] / refPower[0];
					if (T <= 1e-7) T = 1e-7;
					if (T >= 2) T = 2;
					var loss = Math.Round(10 * Math.Log10(T), 3);
					portLoss.NonPol.Add(loss);
				}

                wavelen += mStepWave;
            }//for each data points
            data.AddTrans(portLoss);
        }//for each PORT

        return data;
    }

    #endregion

}

