using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Neon.Aligner;
using Unit = Free302.TnM.DataAnalysis.Unit;



public class C8164 : IDisposable, IoptMultimeter, Itls 
{

    #region definition

    private int TLS_SLOT = 0;								//tls slot no.
    private const int TLS_CHANNEL = 1;						//tls channel no.

    private const int SWEEP_CYCLE = 1;
    private const int SWEEP_SPEED_C = 40;					//[nm/s];
    private const int SWEEP_SPEED_O = 40;					//[nm/s];

    private const int DEFAULT_GAINLEVEL = 0;				//[dBm]
    private const double SENS_AVGTIME = 0.1;				//100 [us]
    private const int DEFAULT_PM_WAVELENTH_C = 1550;
    private const int DEFAULT_PM_WAVELENTH_O = 1301;


    #endregion



    #region structure/inner class

    private struct ThreadParam
    {
        //public int cmd;
        //public double param;
        //public List<List<double>> pwrLogList;
    }

    private class CtlsState
    {
        public int swpWaveStart { get; set; }				//[nm]
        public int swpWaveStop { get; set; }				//[nm]
        public double swpWaveStep { get; set; }				//[nm]
        public double swpSpeed { get; set; }				//sweep speed[nm/s]
        public double outPwr { get; set; }					//output power[dBm]
        public double wavelen { get; set; }					//wavelength[nm]
        public bool ldOn { get; set; }						//ld on or off?
        public int swpDataPoint								//sweep data points.
        {
            get
            {
                int datapoint = 0;
                try
                {
                    datapoint = (int)((swpWaveStop - swpWaveStart) / swpWaveStep) + 1;
                }
                catch
                {
                    datapoint = 0;
                }
                return datapoint;    
            }
        }
    }


    private class PmPortAddress
    {
        public int port { get; set; }
        public int gpib { get; set; }
        public int slot { get; set; }
        public int ch { get; set; }
    }


    private class CportState
    {
        public int port;
        public bool gainLvlAuto;
        public int gainLvl;     //[dBm] detection Range
        public double avgTime;
        public double wavelen;  // wavelength.

        public bool logMode;
        public int logDataCount;
        public double logAvgTime;

        public int inTrgRes;
    }

    #endregion


    public bool CanLogByPort { get; private set; }
    public int NumPorts { get { return mNumPmPorts; } }
    public object[] ChList { get { return mPmPortAddress.Select(x => (object)x.port).ToArray(); }  }


    #region member variables

    private CtlsState m_tlsState;
    private AgilentFrameSystem mTls;

    private int mNumPmPorts;

    private AgilentFrameSystem mPm1;
    private AgilentFrameSystem mPm2;

    private List<PmPortAddress> mPmPortAddress;
    private List<CportState> mPmPortState;

    private bool m_disposed = false;

    #endregion



    #region ==== Class Framework ====

    /// <summary>
    /// constructor.
    /// </summary>
    public C8164()
    {
        m_tlsState = new CtlsState();
        m_tlsState.outPwr = -100;
        mTls = new AgilentFrameSystem();

        mNumPmPorts = 0;
        mPm1 = new AgilentFrameSystem();
        mPm2 = new AgilentFrameSystem();
        mPmPortAddress = new List<PmPortAddress>();
        mPmPortState = new List<CportState>();
    }    


    /// <summary>
    /// connect to the device.
    /// </summary>
    /// <param name="_gpib"></param>
    public bool Connect(int gpibBoard, int gpibTls, int gpibPm1, int gpibPm2, int tlsSlot)
    {
        TLS_SLOT = tlsSlot;
		if (CGlobal.mIsLocalTls)
		{
			if (!mTls.ConnectByGpib(gpibBoard, gpibTls)) return false;
		}
        if (!mPm1.ConnectByGpib(gpibBoard, gpibPm1)) return false;
        if (!mPm2.ConnectByGpib(gpibBoard, gpibPm2)) return false;
        return true;
    }


    public bool Init(bool passTls, bool passPm)
    {
        bool ret = false;
        try
        {
            int[][] devicePort = { CGlobal.PmPort1, CGlobal.PmPort2 };
            AgilentFrameSystem[] pm = { mPm1, mPm2 };
            int pmPortIndex = 0;

            for (int j = 0; j < 2; j++)//each frame
            {
                for (int i = 0; i < devicePort[j].Length; i++) //each slot,ch
                {
                    PmPortAddress address = new PmPortAddress();
                    address.port = ++pmPortIndex;

                    address.gpib = pm[j].gpibAddr;
                    address.slot = (devicePort[j][i] + 1) / 2;
                    address.ch = ((devicePort[j][i] + 1) % 2) + 1;
                    mPmPortAddress.Add(address);
                }
            }
            
            mNumPmPorts = mPmPortAddress.Count();

            //----------------------hardware Trigger Setting ------------
            //8164 frame의 TLS을 사용할시 Loopbak
            //외부 TLS를 사용시 PASSTHROUGH (ex. santec tsl510 )
            mTls.SetConfTrig(AgilentFrameSystem.TRIGGER_CONFIG_LOOPBACK);
            mPm1.SetConfTrig(mTls.gpibAddr == mPm1.gpibAddr ? AgilentFrameSystem.TRIGGER_CONFIG_LOOPBACK : AgilentFrameSystem.TRIGGER_CONFIG_PASSTHROUGH);
            mPm2.SetConfTrig(mTls.gpibAddr == mPm2.gpibAddr ? AgilentFrameSystem.TRIGGER_CONFIG_LOOPBACK : AgilentFrameSystem.TRIGGER_CONFIG_PASSTHROUGH);

            
            //-------- TLS setting-------------------------------------
            //TLS: sweep information
            int swpWaveStart = 0;
            int swpWaveStop = 0;
            double swpWaveStep = 0.0;
            mTls.GetSourSweepRng(TLS_SLOT, TLS_CHANNEL, ref swpWaveStart, ref swpWaveStop, ref swpWaveStep);
            m_tlsState.swpWaveStart = swpWaveStart;
            m_tlsState.swpWaveStop = swpWaveStop;
            m_tlsState.swpWaveStep = swpWaveStep;
            m_tlsState.swpWaveStep = Math.Round(m_tlsState.swpWaveStep, 3);

            if (!passTls)
            {
                //set
                mTls.SetSourLambdaLogOff(TLS_SLOT, TLS_CHANNEL);
                mTls.SetSourAmpModeOff(TLS_SLOT, TLS_CHANNEL);
                mTls.SetSourSweepCycle(TLS_SLOT, TLS_CHANNEL, SWEEP_CYCLE);
                mTls.SetSourSweepMode(TLS_SLOT, TLS_CHANNEL, AgilentFrameSystem.SWEEPMODE_CONT);
                mTls.SetSourOutTrig(TLS_SLOT, TLS_CHANNEL, AgilentFrameSystem.TRIGGEN_STFINISHED);

                SetTlsSweepSpeed((CGlobal.mBand == CGlobal.WlBand.CBand) ? SWEEP_SPEED_C : SWEEP_SPEED_O);
                mTls.SetSourOutPwr(TLS_SLOT, TLS_CHANNEL, CGlobal.TlsPower);
				mTls.Sync_SetTLSPowerUnit(TLS_SLOT, 1, AgilentFrameSystem.POWER_UNIT_DBM);

				TlsOn();
            }


            //--------Detectors setting-------------------------------------
            for (int i = 0; i < mNumPmPorts; i++)
            {
                CportState ps = new CportState();
                ps.port = mPmPortAddress[i].port;
                ps.gainLvl = GetGainLevel(ps.port);
                ps.logMode = IsLogMode(ps.port);
                ps.inTrgRes = GetInTrigResp(ps.port);
                ps.gainLvlAuto = IsGainAuto(ps.port);
                ps.avgTime = GetAvgTime(ps.port);
                ps.wavelen = GetPdWavelen(ps.port);
                GetPortLogParam(ps.port, ref ps.logDataCount, ref ps.logAvgTime);
                mPmPortState.Add(ps);
            }

            if (!passPm)
            {
                StopPdLogMode();
                SetPdUnitWatt();
                SetGainManual();
                SetInTrigResIgnore();
                SetGainLevel(DEFAULT_GAINLEVEL);

                //if (CGlobal.InBandC) SetPdWavelen(DEFAULT_PM_WAVELENTH_C);
                if (CGlobal.mBand == CGlobal.WlBand.CBand) SetPdWavelen(DEFAULT_PM_WAVELENTH_C);
                else SetPdWavelen(DEFAULT_PM_WAVELENTH_O);

                SetLogParam(mPmPortState[0].logDataCount, SENS_AVGTIME);
            }

            ret = true;
        }
        catch
        {
            ret = false;
        }
        return ret;
    }

    #endregion

    

    #region private/poretected method

    // Dispose(bool disposing) executes in two distinct scenarios. 
    // If disposing equals true, the method has been called directly 
    // or indirectly by a user's code. Managed and unmanaged resources 
    // can be disposed. 
    // If disposing equals false, the method has been called by the 
    // runtime from inside the finalizer and you should not reference 
    // other objects. Only unmanaged resources can be disposed. 
    protected virtual void Dispose(bool disposing)
    {

        // Check to see if Dispose has already been called. 
        if (!m_disposed)
        {
            // If disposing equals true, dispose all managed 
            // and unmanaged resources. 
            if (disposing)
            {
                m_tlsState = null;
                mTls = null;
                mPm1 = null;
                mPm2 = null;
                mPmPortAddress = null;
                mPmPortState = null;
            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here. 
            // If disposing is false, 
            // only the following code is executed.
            //CloseHandle(handle);
            //handle = IntPtr.Zero;

            // Note disposing has been done.
            m_disposed = true;
        }

    }




    /// <summary>
    /// set power unit of port to watt.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_unit">dbm, watt</param>
    private void SetPdUnitWatt(int _portNo)
    {

        try
        {
            PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
            mPm1.SetSensPwrUnit(portPos.slot, portPos.ch, AgilentFrameSystem.POWER_UNIT_WATT);
        }
        catch
        {
            //do nothing.
        }

    }

    

    /// <summary>
    /// set power unit of  all ports to watt.
    /// </summary>
    private void SetPdUnitWatt()
    {
        try
        {
            for (int i = 0; i < mPmPortAddress.Count(); i++)
            {
                SetPdUnitWatt(mPmPortAddress[i].port);
            }
        }
        catch
        {
            //do nothing
        }
    }


    

    #endregion




    #region public method



    /// <summary>
    /// Implement IDisposable. 
    /// Do not make this method virtual. 
    /// A derived class should not be able to override this method. 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        // This object will be cleaned up by the Dispose method. 
        // Therefore, you should call GC.SupressFinalize to 
        // take this object off the finalization queue 
        // and prevent finalization code for this object 
        // from executing a second time.
        GC.SuppressFinalize(this);
    }






    #region Itls implementation


    /// <summary>
    /// turn on tls.
    /// </summary>
    public void TlsOn()
    {
        mTls.SourPowerOn(TLS_SLOT, TLS_CHANNEL);
        m_tlsState.ldOn = true;
    }

    /// <summary>
    /// turn off tls.
    /// </summary>
    public void TlsOff()
    {
        mTls.SourPowerOff(TLS_SLOT, TLS_CHANNEL);
        m_tlsState.ldOn = false;
    }


    /// <summary>
    /// set output optical power.
    /// </summary>
    /// <param name="_pwr">optical power [dBm]</param>
    public void SetTlsOutPwr(double _pwr)
    {

        if (Math.Round(_pwr, 3) == Math.Round(m_tlsState.outPwr, 3))
            return;

        //set it as new value.
        try
        {
            //파워 범위에 맞게...
            if (_pwr > -10.0) _pwr = -10.0;//**************
            if (_pwr < -16.0) _pwr = -16.0;

            //설정...
            mTls.SetSourOutPwr(TLS_SLOT, TLS_CHANNEL, _pwr);
            m_tlsState.outPwr = Math.Round(_pwr,3) ;
 
        }
        catch
        {
            m_tlsState.outPwr = -100;
        }
       
    }


    /// <summary>
    /// get output optical power;
    /// </summary>
    /// <returns>function fail : -100 [dBm]</returns>
    public double GetTlsOutPwr()
    {

        if ((int)(m_tlsState.outPwr) != -100)
            return m_tlsState.outPwr;

        try
        {
            m_tlsState.outPwr = mTls.GetSourOutPwr(TLS_SLOT, TLS_CHANNEL);
            m_tlsState.outPwr = Math.Round(m_tlsState.outPwr, 3);
        }
        catch
        {
            m_tlsState.outPwr = -100;
        }

        return m_tlsState.outPwr;

    }


    /// <summary>
    /// set wavelength
    /// </summary>
    /// <param name="_wl">wavelength [nm]</param>
    public void SetTlsWavelen(double _wl)
    {

        if (Math.Round(_wl, 3) == Math.Round(m_tlsState.wavelen, 3)) return;

        //set it as new value.
        try
        {
            //OBand tls의 파장 범위에 맞게... ^^
            //if (_wl > 1575) _wl = 1575.0;///160713TEST
            //if(_wl < 1470.0) _wl = 1470.0;///160713TEST

            //설정.
            mTls.SetSourWavelen(TLS_SLOT, TLS_CHANNEL, Math.Round(_wl, 3));
            m_tlsState.wavelen = Math.Round(_wl, 3);
        }
        catch
        {
            m_tlsState.wavelen = 0.0;
        }

    }



    /// <summary>
    /// get wavelength
    /// </summary>
    /// <returns>wavelength [nm]</returns>
    public double GetTlsWavelen()
    {

       

        try
        {
            if (m_tlsState.wavelen == 0.0)
            {
                m_tlsState.wavelen = mTls.GetSourWavelen(TLS_SLOT);
                m_tlsState.wavelen = Math.Round(m_tlsState.wavelen, 3);
            }
                
        }
        catch
        {
            m_tlsState.wavelen = 0.0;
        }

        return m_tlsState.wavelen;
    }



    /// <summary>
    /// set wavelength range for sweep.
    /// </summary>
    /// <param name="_start">start wavelength</param>
    /// <param name="_stop">stop wavelength</param>
    /// <param name="_step">wavelength step</param>
    public void SetTlsSweepRange(int _start, int _stop, double _step)
    {

        if ((m_tlsState.swpWaveStart == _start) &&
            (m_tlsState.swpWaveStop == _stop) &&
            ((Math.Round(m_tlsState.swpWaveStep, 3)) == (Math.Round(_step, 3))))
            return;


        try
        {

            //if (_start < 1460) _start = 1460;///160713TEST
            //if (_stop > 1575) _stop = 1575;///160713TEST



            mTls.SetSourSweepWlRng(TLS_SLOT, TLS_CHANNEL,
                                    _start,
                                    _stop,
                                    _step);


            m_tlsState.swpWaveStart = _start;
            m_tlsState.swpWaveStop = _stop;
            m_tlsState.swpWaveStep = Math.Round(_step, 3);

        }
        catch
        {
            m_tlsState.swpWaveStart = 0;
            m_tlsState.swpWaveStop = 0;
            m_tlsState.swpWaveStep = 0.0;
        }

    }




    /// <summary>
    /// get wavelength range for sweep.
    /// </summary>
    /// <param name="_start">range start</param>
    /// <param name="_stop">range stop</param>
    /// <param name="_step">sweep step wavelength</param>
    public void GetTlsSweepRange(ref int _start, ref int _stop, ref double _step)
    {


        _start = m_tlsState.swpWaveStart;
        _stop = m_tlsState.swpWaveStop;
        _step = Math.Round(m_tlsState.swpWaveStep, 3);


        //기존에 저장된 데이터 있으면 바로
        if ((_start != 0) && (_stop != 0) && (_step != 0))
            return;



        //기존에 저장된 데이터 없으면 장비에게 물어본다.
        try
        {
            mTls.GetSourSweepRng(TLS_SLOT, TLS_CHANNEL,
                                    ref _start,
                                    ref _stop,
                                    ref _step);
        }
        catch
        {
            _start = 0;
            _stop = 0;
            _step = 0;
        }


        m_tlsState.swpWaveStart = _start;
        m_tlsState.swpWaveStop = _stop;
        m_tlsState.swpWaveStep = Math.Round(_step, 3) ;

    }



    /// <summary>
    /// Sets the sweep speed for continuous sweep.
    /// </summary>
    /// <param name="_speed">sweep speed [n/m]
    ///                      range - [nm/s],
    ///                      step  [nm/s] </param>
    public void SetTlsSweepSpeed(double _speed)
    {
        if (Math.Round(m_tlsState.swpSpeed, 1) == Math.Round(_speed, 1))
            return;

        try
        {
            mTls.SetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL, Math.Round(_speed, 1));

            m_tlsState.swpSpeed = mTls.GetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL);
            m_tlsState.swpSpeed = Math.Round(m_tlsState.swpSpeed, 1);
        }
        catch
        {
            m_tlsState.swpSpeed = 0.0;
        }

    }



    /// <summary>
    /// get the speed for sweeping.
    /// 일단 메모리에 저장된 값을 찾고
    /// 없으면 장비에 물어본다.
    /// </summary>
    /// <returns>sweep speed [n/m]</returns>
    public double GetTlsSweepSpeed()
    {

        if (Math.Round(m_tlsState.swpSpeed, 1) != 0.0)
            return Math.Round(m_tlsState.swpSpeed, 1);

        try
        {
            m_tlsState.swpSpeed = mTls.GetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL);
            m_tlsState.swpSpeed = Math.Round(m_tlsState.swpSpeed, 1);
        }
        catch
        {
            m_tlsState.swpSpeed = 0.0;
        }

        return Math.Round(m_tlsState.swpSpeed, 1);
    }



    /// <summary>
    /// get wavelength logging data
    /// </summary>
    /// <returns> if function fails, it returns null. [nm]</returns>
    public List<double> GetTlsWavelenLog()
    {
		var log = mTls.GetSourLambdaLog(TLS_SLOT, TLS_CHANNEL);
		readError(mTls);
		return log;
    }



    /// <summary>
    /// begin sweeping as continuous mode.
    /// </summary>
    public void ExecTlsSweepCont()
	{
		var state = -1;
		var watch = Stopwatch.StartNew();

		Log.Write("Sweep Starting...");
		readError(mTls);

		state = tlsState();
		if (state != 1)
		{
			Log.Write($"========== TLS State : {state} ==========");
			state = TryLdOn();
		}
		Log.Write($"State={state}\tΔT={watch.ElapsedMilliseconds}\tms");

		mTls.AsyncSourSweepStart(TLS_SLOT, TLS_CHANNEL);
		m_tlsState.wavelen = m_tlsState.swpWaveStop;
	}



	public int TryLdOn()
	{
		int state = 0;
		float[] temp = new float[2];
		for (int i = 0; i < 120; i++)
		{
			state = tlsState();
			temp = TlsTemp();
			Log.Write($"[Counter={i + 1}]\tState={state}\tTempLast={temp[0]}\tTempCurrent={temp[1]}");
			if (state == 1)
			{
				return state;
			}
			else
			{
				readError(mTls);
			}
			Task.Delay(10000).Wait();
			mTls.SourPowerOn(TLS_SLOT, TLS_CHANNEL);
		}
		Log.Write($"[Counter=120]\tState={state}\tTempLast={temp[0]}\tTempCurrent={temp[1]}");
		return state;
	}



	public int tlsState(bool recordLog = false)
	{
		var state = mTls.GetSourPowerState(TLS_SLOT, TLS_CHANNEL);
		var temp = TlsTemp();
		if (recordLog)
		{
			var time = DateTime.Now.ToString("MMdd_HHmmss.fff");
			Log.Write($"[{time}]\tTLS State : {state}\tTempLast={temp[0]}\tTempCurrent={temp[1]}");
		}
		return state;
	}



	public float[] TlsTemp()
	{
		var tempCurrent = mTls.GetSourZeroTemp(TLS_SLOT);
		var tempLast = mTls.GetSourZeroTempLast(TLS_SLOT);
		return new float[] { tempLast, tempCurrent };
	}



	public void readError(AgilentFrameSystem frame, [CallerMemberName] string callingMethod = "")
    {
		for (int j = 0; j < 30; j++)
        {
			var message = frame.ReadErrorMessage();
			Log.Write($"[{frame.gpibAddr}] [{callingMethod}().readError()]\t{message}");
            if (message.Contains("No error")) break;
        }
    }



    /// <summary>
    /// query whether sweeping is operating or not.
    /// </summary>
    /// <returns>true:operating, false:stop </returns>
    public bool IsTlsSweepOperating()
    {
        return mTls.IsSweepping(TLS_SLOT, TLS_CHANNEL);
    }



    /// <summary>
    /// set Tls Logging on.
    /// </summary>
    public void TlsLogOn()
    {
        mTls.SetSourLambdaLogOn(TLS_SLOT, TLS_CHANNEL);
    }


    /// <summary>
    /// set Tls Logging off.
    /// </summary>
    public void TlsLogOff()
    {
        try
        {
            mTls.SetSourLambdaLogOff(TLS_SLOT, TLS_CHANNEL);
        }
        catch
        {
            //do nothing.
        }
    }



    #endregion


    /// <summary>
    /// sets logging parameter of detector ports. (all port)
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <param name="_dataPoint">number of datas</param>
    /// <param name="_avgTime">averaging time</param>
    public void SetLogParam(int _dataPoint, double _avgTime)
    {
        foreach(var p in mPmPortAddress) SetLogParam(p.port, _dataPoint, _avgTime);
    }


    /// <summary>
    /// sets logging paramater of detector port.
    /// </summary>
    /// <param name="_portNo">detector port no.</param>
    /// <param name="_numOfData">data points.</param>
    /// <param name="_avgTime">average time.</param>
    public void SetLogParam(int _portNo, int _numOfData, double _avgTime)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == _portNo);
        CportState state = mPmPortState.Find(ps => ps.port == _portNo);
        
        //새값으로 세팅 한다..
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        pm.SetSensLogParam(address.slot, _numOfData, _avgTime);

        state.logDataCount = _numOfData;
        state.logAvgTime = _avgTime;

        //other port position & state.
        var address2 = mPmPortAddress.Find(a => a.gpib == address.gpib && a.slot == address.slot && a.port != address.port);
        if (address2 != null)
        {
            CportState othStat = mPmPortState.Find(ps => ps.port == address2.port);
            othStat.logDataCount = _numOfData;
            othStat.logAvgTime = _avgTime;
        }
    }

    /// <summary>
    /// Set pd to sweep mode.
    /// </summary>
    /// <param name="_port"></param>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_step"></param>
    public void SetPdSweepMode(int _port, int _startWave, int _stopWave, double _stepWave)
    {
        int dataPoint = Convert.ToInt32((_stopWave - _startWave) / _stepWave) + 1;
        SetGainManual(_port);
        SetInTrigResSingMsr(_port);
        SetLogParam(_port, dataPoint, SENS_AVGTIME);
    }
    /// <summary>
    /// Set pd to sweep mode.
    /// </summary>
    /// <param name="_port"></param>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_step"></param>
    public void SetPdSweepMode(int[] _portNos, int _startWave, int _stopWave, double _stepWave)
    {
        for (int i = 0; i < _portNos.Length; i++) SetPdSweepMode(_portNos[i], _startWave, _stopWave, _stepWave);
        //throw new NotImplementedException();
    }




    public void SetGainLevel(int gainlLevel)
    {
        foreach (var address in mPmPortAddress) SetGainLevel(address.port, gainlLevel);
    }

    public void SetGainLevel(int[] ports, int gainLevel)
    {
        for (int i = 0; i < ports.Length; i++) SetGainLevel(ports[i], gainLevel);
    }    
    public void SetGainLevel(int port, int gainLevel)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        CportState state = mPmPortState.Find(ps => ps.port == port);

        //기존에 셋팅 되어있는 값이면 걍 나감.
        if (state.gainLvl == gainLevel) return;

        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        pm.SetSensPwrRng(address.slot, address.ch, gainLevel);
        
        state.gainLvl = gainLevel;
    }


    
    /// <summary>
    /// get port's gain level.
    /// </summary>
    /// <param name="_portNo"> port no.</param>
    public int GetGainLevel(int _portNo)
    {
        int ret = 0;

        PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
        int slot = portPos.slot;
        int chnl = portPos.ch;
        var pm = portPos.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        ret = pm.GetSensPwrRng(slot, chnl);
        return ret;
    }
    public List<int> GetGainLevel()
    {
        List<int> retList = null;
        int[] ports = new int[mNumPmPorts];
        for (int i = 0; i < mNumPmPorts; i++) ports[i] = i + 1;
        retList = GetGainLevel(ports);
        return retList;
    }
    public List<int> GetGainLevel(int[] _portNos)
    {
        List<int> retList = new List<int>();
        for (int i = 0; i < _portNos.Count(); i++) retList.Add(GetGainLevel(_portNos[i]));
        return retList;
    }


    /// <summary>
    /// sets the powermeter's wavelength.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_wavelen">wavelength [nm]</param>
    public void SetPdWavelen(int _portNo, double _wavelen)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == _portNo);
        CportState state = mPmPortState.Find(ps => ps.port == _portNo);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;

        //기존에 셋팅 되어있는 값이면 걍 나감.
        if (Math.Round(state.wavelen, 3) == Math.Round(_wavelen, 3)) return;

        pm.SetSensWavelen(address.slot, address.ch, _wavelen);
        
        state.wavelen = _wavelen;
    }
    public void SetPdWavelen(double _wavelen)
    {
        for (int i = 0; i < mPmPortAddress.Count(); i++) SetPdWavelen(mPmPortAddress[i].port, _wavelen);
    }
    public void SetPdWavelen(int[] _portNos, double _wavelen)
    {
        for (int i = 0; i < _portNos.Length; i++) SetPdWavelen(_portNos[i], _wavelen);
    }


    public List<double> GetPdWavelen()
    {
        throw new NotImplementedException();
    }


    public List<double> GetPdWavelen(int[] _ports)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// gets the powermeter's wavelength.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns>pd's wavelength [nm]</returns>
    public double GetPdWavelen(int _portNo)
    {
        double ret = 0.0;
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == _portNo);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        pm.GetSensWavelen(address.slot, address.ch);
        return Math.Round(ret, 3);
    }
    

    /// <summary>
    /// set photodiodes to sweep mode (all ports)
    /// 1.gain maunual
    /// 2.trigger procedure를 single measure로 설정.
    /// 3.logging parameter 설정.
    /// 2015.10.13일 Neon.aliner의 IoptMultimeter interface 변경 필요.
    /// </summary>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_stepWave"></param>
    public void SetPdSweepMode(int _startWave, int _stopWave, double _stepWave)
    {
        throw new NotImplementedException();
    }
    public void SetPdSweepMode(int[] ports, int numDataPoints)
    {
        for (int i = 0; i < ports.Length; i++)
        {
            SetGainManual(ports[i]);
            SetInTrigResSingMsr(ports[i]);
            SetLogParam(ports[i], numDataPoints, SENS_AVGTIME);
        }
    }

    /// <summary>
    /// stop pd to sweep mode. all port;
    /// 1.trigger procedure를 제거한다.
    /// </summary>
    public void StopPdSweepMode()
    {
        SetInTrigResIgnore();
    }

    public void StopPdSweepMode(int _port)
    {
        SetInTrigResIgnore(_port);
    }

    public void StopPdSweepMode(int[] _portNos)
    {
        throw new NotImplementedException();
    }




    public void SetInTrigResIgnore()
    {
        foreach (var a in mPmPortAddress) SetInTrigResIgnore(a.port);
    }
    public void SetInTrigResIgnore(int[] _portNos)
    {
        for (int i = 0; i < _portNos.Length; i++) SetInTrigResIgnore(_portNos[i]);
    }
    /// <summary>
    /// set input trigger response of port to 'ignore'
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetInTrigResIgnore(int _portNo)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == _portNo);
        CportState state = mPmPortState.Find(ps => ps.port == _portNo);

        //other port position & state.
        PmPortAddress othPos = null;
        int nOtherPortNo = -1;
        for (int i = 0; i < mNumPmPorts; i++)
        {
            othPos = mPmPortAddress[i];
            if ((address.gpib == othPos.gpib) &&
                (address.slot == othPos.slot) &&
                (_portNo != othPos.port))
            {
                nOtherPortNo = othPos.port;
                break;
            }
            othPos = null;
        }

        CportState othStat = null;
        if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
            othStat = mPmPortState.Find(ps => ps.port == nOtherPortNo);



        //기존에 셋팅 되어있는 값이면 걍 나감.
        if (state.inTrgRes == AgilentFrameSystem.INTRIGRES_IGNORE)
        {
            if (othPos != null)
                othStat.inTrgRes = AgilentFrameSystem.INTRIGRES_IGNORE;
            return;
        }


        //새값으로 세팅 한다..
        if (address.gpib == mPm1.gpibAddr)
        {
            mPm1.SetSensInTrigResp(address.slot,
                                AgilentFrameSystem.INTRIGRES_IGNORE);
        }
        else if (address.gpib == mPm2.gpibAddr)
        {
            mPm2.SetSensInTrigResp(address.slot,
                                AgilentFrameSystem.INTRIGRES_IGNORE);
        }



        state.inTrgRes = AgilentFrameSystem.INTRIGRES_IGNORE;
        if (othPos != null)
            othStat.inTrgRes = AgilentFrameSystem.INTRIGRES_IGNORE;
    }

    
    public void SetInTrigResSingMsr()
    {
        foreach (var a in mPmPortAddress) SetInTrigResSingMsr(a.port);
    }
    public void SetInTrigResSingMsr(int[] _portNos)
    {
        for (int i = 0; i < _portNos.Length; i++) SetInTrigResSingMsr(_portNos[i]);
    }
    /// <summary>
    /// set input trigger response of port to 'single measure'
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetInTrigResSingMsr(int _portNo)
    {
        //port position & state.
        PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
        CportState portStat = null;
        portStat = mPmPortState.Find(ps => ps.port == _portNo);


        //other port position & state.
        PmPortAddress othPos = null;
        int nOtherPortNo = -1;
        for (int i = 0; i < mNumPmPorts; i++)
        {
            othPos = mPmPortAddress[i];
            if ((portPos.gpib == othPos.gpib) &&
                (portPos.slot == othPos.slot) &&
                (_portNo != othPos.port))
            {
                nOtherPortNo = othPos.port;
                break;
            }
            othPos = null;
        }

        CportState othStat = null;
        if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
            othStat = mPmPortState.Find(ps => ps.port == nOtherPortNo);



        //기존에 셋팅 되어있는 값이면 걍 나감.
        if (portStat.inTrgRes == AgilentFrameSystem.INTRIGRES_SINGLEMEASURE)
        {
            if (othPos != null) othStat.inTrgRes = AgilentFrameSystem.INTRIGRES_SINGLEMEASURE;
            return;
        }


        //새값으로 세팅 한다..
        if (portPos.gpib == mPm1.gpibAddr)
        {
            mPm1.SetSensInTrigResp(portPos.slot,
                                    AgilentFrameSystem.INTRIGRES_SINGLEMEASURE);
        }
        else if (portPos.gpib == mPm2.gpibAddr)
        {
            mPm2.SetSensInTrigResp(portPos.slot,
                                    AgilentFrameSystem.INTRIGRES_SINGLEMEASURE);
        }



        portStat.inTrgRes = AgilentFrameSystem.INTRIGRES_SINGLEMEASURE;
        if (othPos != null) othStat.inTrgRes = AgilentFrameSystem.INTRIGRES_SINGLEMEASURE;
    }


    
    public void SetGainManual()
    {
        for (int i = 0; i < mPmPortAddress.Count(); i++) SetGainManual(mPmPortAddress[i].port);
    }    
    public void SetGainManual(int[] _portNos)
    {
        for (int i = 0; i < _portNos.Length; i++) SetGainManual(_portNos[i]);
    }
    /// <summary>
    /// sets power-range of port to manual.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetGainManual(int _portNo)
    {
        //port position & state.
        PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
        CportState portStat = null;
        portStat = mPmPortState.Find(ps => ps.port == _portNo);


        //other port position & state.
        PmPortAddress othPos = null;
        int nOtherPortNo = -1;
        for (int i = 0; i < mNumPmPorts; i++)
        {
            othPos = mPmPortAddress[i];
            if ((portPos.gpib == othPos.gpib) &&
                (portPos.slot == othPos.slot) &&
                (_portNo != othPos.port))
            {
                nOtherPortNo = othPos.port;
                break;
            }
            othPos = null;
        }

        CportState othStat = null;
        if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
            othStat = mPmPortState.Find(ps => ps.port == nOtherPortNo);



        //기존에 셋팅 되어있는 값이면 걍 나감.
        if (portStat.gainLvlAuto == false)
        {
            if (othPos != null)
                othStat.gainLvlAuto = false;
            return;
        }


        //새값으로 세팅 한다..
        if (portPos.gpib == mPm1.gpibAddr)
            mPm1.SetSensPwrRngManual(portPos.slot);
        else if (portPos.gpib == mPm2.gpibAddr)
            mPm2.SetSensPwrRngManual(portPos.slot);

        portStat.gainLvlAuto = false;
        if (othPos != null)
            othStat.gainLvlAuto = false;
    }
       
    


    public void SetPdLogMode()
    {
        foreach (var p in mPmPortAddress) SetPdLogMode(p.port);
    }
    public void SetPdLogMode(int[] ports)
    {
        for (int i = 0; i < ports.Length; i++) SetPdLogMode(ports[i]);
    }

    /// <summary>
    /// set port to logging mode.
    /// </summary>
    /// <param name="port">port no.</param>
    /// Logging mode 2번 들어가게되면... 이상해짐.
    /// 절대로 Logging mode는 slot단위로 들어가고 나가야 됨.
    public void SetPdLogMode(int port)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        CportState state = mPmPortState.Find(ps => ps.port == port);
        var pm = (address.gpib == mPm1.gpibAddr) ? mPm1 : mPm2;

        if (pm.IsSensLogMode(address.slot)) return;

        pm.SetSensLogFunc(address.slot);

        return;

        #region --- old ---
        ////other port position & state.
        //PmPortAddress address2 = null;
        //int nOtherPortNo = -1;
        //for (int i = 0; i < mNumPmPorts; i++)
        //{
        //    address2 = mPmPortAddress[i];
        //    if ((address.gpib == address2.gpib) && (address.slot == address2.slot) && (_port != address2.port))
        //    {
        //        nOtherPortNo = address2.port;
        //        break;
        //    }
        //    address2 = null;
        //}

        //CportState state2 = null;
        //if (address2 != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
        //    state2 = mPmPortState.Find(ps => ps.port == nOtherPortNo);


        ////기존에 셋팅 되어있는 값과 동일하면 그냥 나감.
        //if (state.logMode == true)
        //{
        //    if (address2 != null) state2.logMode = true;
        //    return;
        //}


        ////새값으로 세팅 한다..
        //if (address.gpib == mPm1.gpibAddr) mPm1.SetSensLogFunc(address.slot);
        //else if (address.gpib == mPm2.gpibAddr) mPm2.SetSensLogFunc(address.slot);


        //state.logMode = true;
        //if (address2 != null) state2.logMode = true;
        #endregion
    }


    public void StopPdLogMode()
    {
        foreach (var p in mPmPortAddress) StopPdLogMode(p.port);

		if (mPmPortAddress[0].gpib == mPm1.gpibAddr) readError(mPm1);
		if (mPmPortAddress[mPmPortAddress.Count - 1].gpib == mPm2.gpibAddr && mPm1.gpibAddr != mPm2.gpibAddr) readError(mPm2);
	}
    public void StopPdLogMode(int[] ports)
    {
        for (int i = 0; i < ports.Length; i++) StopPdLogMode(ports[i]);

		if (mPmPortAddress[0].gpib == mPm1.gpibAddr) readError(mPm1);
		if (mPmPortAddress[mPmPortAddress.Count - 1].gpib == mPm2.gpibAddr && mPm1.gpibAddr != mPm2.gpibAddr) readError(mPm2);
	}
    public void StopPdLogMode(int port)
    {
        //port position & state.
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        CportState state = mPmPortState.Find(ps => ps.port == port);
        var pm = (address.gpib == mPm1.gpibAddr) ? mPm1 : mPm2;

        if (!pm.IsSensLogMode(address.slot)) return;

        pm.StopSensLogFunc(address.slot);

        #region ---- old ----
        ////other port position & state.
        //PmPortAddress othPos = null;
        //int nOtherPortNo = -1;
        //for (int i = 0; i < mNumPmPorts; i++)
        //{
        //    othPos = mPmPortAddress[i];
        //    if ((address.gpib == othPos.gpib) &&
        //        (address.slot == othPos.slot) &&
        //        (port != othPos.port))
        //    {
        //        nOtherPortNo = othPos.port;
        //        break;
        //    }
        //    othPos = null;
        //}

        //CportState othStat = null;
        //if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
        //    othStat = mPmPortState.Find(ps => ps.port == nOtherPortNo);


        ////기존에 셋팅 되어있는 값과 동일하면 그냥 나감.
        //if (state.logMode == false)
        //{
        //    if (othPos != null) othStat.logMode = false;
        //    return;
        //}


        ////새값으로 세팅 한다..
        //if (address.gpib == mPm1.gpibAddr) mPm1.StopSensLogFunc(address.slot);
        //else if (address.gpib == mPm2.gpibAddr) mPm2.StopSensLogFunc(address.slot);

        //state.logMode = false;
        //if (othPos != null) othStat.logMode = false;
        #endregion
    }




    /// <summary>
    /// get logging parameter of detect port.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_dataPoints">number of data points</param>
    /// <param name="_avgTime">sensing averaging time</param>
    public void GetPortLogParam(int _portNo, ref int _dataPoints, ref double _avgTime)
    {
        try
        {
            PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.ch;

            if (portPos.gpib == mPm1.gpibAddr)
                mPm1.GetSensLogParam(slot, ref _dataPoints, ref _avgTime);
            else if (portPos.gpib == mPm2.gpibAddr)
                mPm2.GetSensLogParam(slot, ref _dataPoints, ref _avgTime);

        }
        catch
        {
            _dataPoints = 0;
            _avgTime = 0;
        }
    }



    /// <summary>
    /// gets the power unit of detect port.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns></returns>
    public int GetPwrUnit(int _portNo)
    {
        int ret = -1;

        try
        {
            PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.ch;

            if (portPos.gpib == mPm1.gpibAddr)
                ret = mPm1.GetSensPwrUnit(slot, chnl);
            else if (portPos.gpib == mPm2.gpibAddr)
                ret = mPm2.GetSensPwrUnit(slot, chnl);

        }
        catch
        {
            ret = -1;
        }

        return ret;
    }



    /// <summary>
    /// gets the averaging time of detector port.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns></returns>
    public double GetAvgTime(int _portNo)
    {
        double ret = 0.0;

        try
        {
            PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.ch;

            if (portPos.gpib == mPm1.gpibAddr)
                ret = mPm1.GetSensAvgTime(slot);
            else if (portPos.gpib == mPm2.gpibAddr)
                ret = mPm2.GetSensAvgTime(slot);

        }
        catch
        {
            ret = 0.0;
        }

        return ret;
    }

   




    /// <summary>
    /// logging mode or not.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns></returns>
    public bool IsLogMode(int _portNo)
    {
        var address = mPmPortAddress.Find(_p => _p.port == _portNo);
        return ((address.gpib == mPm1.gpibAddr) ? mPm1 : mPm2).IsSensLogMode(address.slot);
    }



    /// <summary>
    /// gets input trigger response of detect port.
    /// </summary>
    /// <param name="_portNo"></param>
    /// <returns></returns>
    public int GetInTrigResp(int _portNo)
    {
        int ret = 0;
        PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
        int slot = portPos.slot;

        if (portPos.gpib == mPm1.gpibAddr)
            ret = mPm1.GetSensInTrigResp(slot);
        else if (portPos.gpib == mPm2.gpibAddr)
            ret = mPm2.GetSensInTrigResp(slot);
        return ret;
    }



    /// <summary>
    /// whether ranging auto or manual. 
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns>true:auto or false:manaul</returns>
    public bool IsGainAuto(int _portNo)
    {

        bool ret = false;
        PmPortAddress portPos = mPmPortAddress.Find(_p => _p.port == _portNo);
        int slot = portPos.slot;

        if (portPos.gpib == mPm1.gpibAddr)
            ret = mPm1.IsSensRangeAuto(slot);
        else if (portPos.gpib == mPm2.gpibAddr)
            ret = mPm2.IsSensRangeAuto(slot);
        return ret;
    }



    /// <summary>
    /// read optical power.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns>optical power [mw]</returns>
    public double ReadPower(int _port)
    {
        double ret = 0;
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == _port);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        ret = pm.ReadSensPwr(address.slot, address.ch);
        return ret;
    }
    public List<double> ReadPwr(int[] _ports)
    {
        List<double> retList = new List<double>();
        for (int i = 0; i < _ports.Length; i++) retList.Add(ReadPower(_ports[i]));
        return retList;
    }

    public List<double> ReadPwr()
    {
        //return ReadPwr(mPmPortAddress.Select((x) => x.port).ToArray());
        throw new NotImplementedException();
    }

    /// <summary>
    /// 8164 read == trigger & fetch
    /// </summary>
    /// <param name="port"></param>
    /// <returns></returns>
    public double ReadPowerTrig_dBm(int port)
    {
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        return Unit.MillWatt2Dbm(pm.ReadPowerTrig_mW(address.slot, address.ch));
    }
    public double ReadPower_dBm(int port)
    {
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        return Unit.MillWatt2Dbm(pm.ReadSensPwr(address.slot, address.ch));
    }
    public void WriteAtt(int slot, decimal att_dB)
    {
        //PmPortAddress address = mPmPortAddress.Find(_p => _p.slot == slot);
        //var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        mPm1.WriteAtt(slot, att_dB);
        var act = ReadAtt(slot);
        if (att_dB != act)
            throw new Exception($"Attenuator error: exp={att_dB} != act={act}");
    }
    public decimal ReadAtt(int slot)
    {
        //PmPortAddress address = mPmPortAddress.Find(_p => _p.slot == slot);
        //var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        return (decimal)Math.Round(mPm1.ReadAtt(slot), 3);
    }
    public void WriteAttOffset(int slot, decimal att_dB)
    {
        //PmPortAddress address = mPmPortAddress.Find(_p => _p.slot == slot);
        //var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        mPm1.WriteAttOffset(slot, att_dB);
        var act = ReadAttOffset(slot);
        if (att_dB != act)
            throw new Exception($"Attenuator offset error: exp={att_dB} != act={act}");
    }
    public decimal ReadAttOffset(int slot)
    {
        //PmPortAddress address = mPmPortAddress.Find(_p => _p.slot == slot);
        //var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        return (decimal)Math.Round(mPm1.ReadAttOffset(slot), 3);
    }


    public void SetAvgTime_ms(int port, int time_ms)
    {
        var address = mPmPortAddress.Find(p => p.port == port);
        var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        pm.SetSensAvgTime(address.slot, time_ms);//milli second
    }



    public int mGainNo;
    public int mPol;
    public Action<string, int, int, int> mDutReporter;//msg, gain, pol, port

    public List<double> GetPwrLog(int port)
    {
        PmPortAddress address = mPmPortAddress.Find(_p => _p.port == port);
        if (address == null) return new List<double>();
		var pm = address.gpib == mPm1.gpibAddr ? mPm1 : mPm2;
        var pmLog = pm.GetSensLog(address.slot, address.ch);

		readError(pm, $"<PM.{port}>GetPwrLog");
		return pmLog;
    }
    

    #endregion



}
