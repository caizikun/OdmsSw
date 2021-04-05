using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neon.Aligner;



public class C8164 : IoptMultimeter, Itls
{

    #region definition

    public const int TLS_SLOT = 0;                   //tls slot no.
    public const int TLS_CHANNEL = 1;                //tls channel no.

    public const int SWEEP_CYCLE = 1;
    public const int SWEEP_SPEED = 40;               //[nm/s];
    //public const int DEFAULT_TLS_POWER = -10       //[dBm]

    public const int DEFAULT_GAINLEVEL = -10;        //[dBm]
    public const double SENS_AVGTIME = 0.1;          //100 [us]
    public const int DEFAULT_PDWAVELENTH = 1310;

    #endregion



    #region structure/inner class


    private class CtlsState
    {

        public int swpWaveStart { get; set; }   //[nm]
        public int swpWaveStop { get; set; }    //[nm]
        public double swpWaveStep { get; set; } //[nm]
        public double swpSpeed { get; set; }    //sweep speed[nm/s]
        public double outPwr { get; set; }      //output power[dBm]
        public double wavelen { get; set; }     //wavelength[nm]
        public bool ldOn { get; set; }          //ld on or off?
        public int swpDataPoint                 //sweep data points.
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



    private class CportPos
    {
        public int port { get; set; }
        public int gpib { get; set; }
        public int slot { get; set; }
        public int chnl { get; set; }
    }


    private class CportState
    {
        public int port;
        public int gainLvl;     //[dBm] detection Range
        public bool logMode;
        public int inTrgRes;
        public bool gainLvlAuto;
        public double avgTime;
        public int logDataCount;
        public double logAvgTime;
        public double wavelen;  // wavelength.
    }


    #endregion



    #region property

    public int portCnt { get { return m_portCnt; } }

    #endregion



    #region member variables

    private CtlsState m_tlsState;

    private int m_portCnt;
    private CagilLightwaveSystem m_lws;
    private List<CportPos> m_portPosList;
    private List<CportState> m_portStatList;

    #endregion



    #region constructor/desconstructor

    /// <summary>
    /// constructor.
    /// </summary>
    public C8164()
    {

        m_tlsState = new CtlsState();
        m_portCnt = 0;
        m_lws = new CagilLightwaveSystem();
        m_portPosList = new List<CportPos>();
        m_portStatList = new List<CportState>();
    }

    #endregion



    #region private method


    /// <summary>
    /// set power unit of port to watt.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_unit">dbm, watt</param>
    private void SetPdUnitWatt(int _portNo)
    {
        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            m_lws.SetSensPwrUnit(portPos.slot,
                                 portPos.chnl,
                                 CagilLightwaveSystem.POWER_UNIT_WATT);
        }
        catch
        {
            //do nothing.
        }

    }



    /// <summary>
    /// set power unit of ports to watt.
    /// </summary>
    /// <param name="_portNos">port no. array </param>
    private void SetPdUnitWatt(int[] _portNos)
    {

        try
        {
            for (int i = 0; i < _portNos.Length; i++)
            {
                SetPdUnitWatt(_portNos[i]);
            }

        }
        catch
        {
            //do nothing
        }

    }



    /// <summary>
    /// set power unit of  all ports to watt.
    /// </summary>
    private void SetPdUnitWatt()
    {
        try
        {
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                SetPdUnitWatt(m_portPosList[i].port);
            }
        }
        catch
        {
            //do nothing
        }
    }



    /// <summary>
    /// set power unit of port to dBm.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_unit">dbm, watt</param>
    private void SetPdUnitDBm(int _portNo)
    {

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            m_lws.SetSensPwrUnit(portPos.slot,
                                portPos.chnl,
                                CagilLightwaveSystem.POWER_UNIT_DBM);
        }
        catch
        {
            //do nothing.
        }

    }


    /// <summary>
    /// set power unit of ports to watt.
    /// </summary>
    /// <param name="_portNos">port no. array </param>
    private void SetPdUnitDBm(int[] _portNos)
    {

        try
        {
            for (int i = 0; i < _portNos.Length; i++)
            {
                SetPdUnitDBm(_portNos[i]);
            }

        }
        catch
        {
            //do nothing
        }

    }



    /// <summary>
    /// set power unit of  all ports to watt.
    /// </summary>
    private void SetPdUnitDBm()
    {
        try
        {
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                SetPdUnitDBm(m_portPosList[i].port);
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
    /// connect to the device.
    /// </summary>
    /// <param name="_gpib"></param>
    public bool Connect(int _gpib)
    {
        bool ret = false;

        try
        {
            ret = m_lws.ConnectByGpib(_gpib);
        }
        catch
        {
            ret = false;
        }

        return ret;
    }




    /// <summary>
    /// initailize device.
    /// </summary>
    /// <returns></returns>
    public bool Init()
    {

        bool ret = false;

        try
        {

            //set port postion.
            CportPos portPos = null;
            portPos = new CportPos();
            portPos.port = 1;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 1;
            portPos.chnl = 1;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 2;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 1;
            portPos.chnl = 2;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 3;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 2;
            portPos.chnl = 1;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 4;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 2;
            portPos.chnl = 2;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 5;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 3;
            portPos.chnl = 1;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 6;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 3;
            portPos.chnl = 2;
            m_portPosList.Add(portPos);


            portPos = new CportPos();
            portPos.port = 7;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 4;
            portPos.chnl = 1;
            m_portPosList.Add(portPos);

            portPos = new CportPos();
            portPos.port = 8;
            portPos.gpib = m_lws.gpibAddr;
            portPos.slot = 4;
            portPos.chnl = 2;
            m_portPosList.Add(portPos);
            m_portCnt = m_portPosList.Count();



            //----------------------hardware Trigger Setting ------------
            //8164 frame의 TLS을 사용할시 Loopbak
            //외부 TLS를 사용시 PASSTHROUGH (ex. santec tsl510 )
            m_lws.SetConfTrig(CagilLightwaveSystem.TRIGGER_CONFIG_LOOPBACK);



            //--------tls setting-------------------------------------


            //sweep information
            int swpWaveStart = 0;
            int swpWaveStop = 0;
            double swpWaveStep = 0.0;
            m_lws.GetSourSweepRng(TLS_SLOT, TLS_CHANNEL,
                                  ref swpWaveStart,
                                  ref swpWaveStop,
                                  ref swpWaveStep);

            m_tlsState.swpWaveStart = swpWaveStart;
            m_tlsState.swpWaveStop = swpWaveStop;
            m_tlsState.swpWaveStep = swpWaveStep;
            m_tlsState.swpWaveStep = Math.Round(m_tlsState.swpWaveStep, 3);


            //set
            m_lws.SetSourLambdaLogOff(TLS_SLOT, TLS_CHANNEL);
            m_lws.SetSourAmpModeOff(TLS_SLOT, TLS_CHANNEL);
            m_lws.SetSourSweepCycle(TLS_SLOT, TLS_CHANNEL, SWEEP_CYCLE);
            m_lws.SetSourSweepMode(TLS_SLOT, TLS_CHANNEL, CagilLightwaveSystem.SWEEPMODE_CONT);
            m_lws.SetSourOutTrig(TLS_SLOT, TLS_CHANNEL, CagilLightwaveSystem.TRIGGEN_STFINISHED);


            SetTlsSweepSpeed(SWEEP_SPEED);
            TlsOn();



            //--------Detectors setting-------------------------------------
            for (int i = 0; i < m_portCnt; i++)
            {
                CportState ps = new CportState();
                ps.port = m_portPosList[i].port;
                ps.gainLvl = GetGainLevel(ps.port);
                ps.logMode = IsLogMode(ps.port);
                ps.inTrgRes = GetInTrigResp(ps.port);
                ps.gainLvlAuto = IsGainAuto(ps.port);
                ps.avgTime = GetAvgTime(ps.port);
                ps.wavelen = GetPdWavelen(ps.port);
                GetPortLogParam(ps.port, ref ps.logDataCount, ref ps.logAvgTime);
                m_portStatList.Add(ps);
            }

            StopPdLogMode();
            SetPdUnitWatt();
            SetGainManual();
            SetInTrigResIgnore();
            SetGainLevel(DEFAULT_GAINLEVEL);
            SetPdWavelen(DEFAULT_PDWAVELENTH);
            SetLogParam(m_portStatList[0].logDataCount, SENS_AVGTIME);

            ret = true;

        }
        catch
        {
            ret = false;
        }

        return ret;

    }




    #region Itls implementation


    /// <summary>
    /// turn on tls.
    /// </summary>
    public void TlsOn()
    {
        m_lws.SourPowerOn(TLS_SLOT, TLS_CHANNEL);
        m_tlsState.ldOn = true;
    }

    /// <summary>
    /// turn off tls.
    /// </summary>
    public void TlsOff()
    {
        m_lws.SourPowerOff(TLS_SLOT, TLS_CHANNEL);
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
            if (_pwr > -12.0)   //상한
                _pwr = -12.0;

            if (_pwr < -16.0)   //하한
                _pwr = -16.0;

            //설정...
            m_lws.SetSourOutPwr(TLS_SLOT, TLS_CHANNEL, _pwr);
            m_tlsState.outPwr = Math.Round(_pwr, 3);

        }
        catch
        {
            //do nothing.
        }

    }


    /// <summary>
    /// get output optical power;
    /// </summary>
    /// <returns>function fail : -100 [dBm]</returns>
    public double GetTlsOutPwr()
    {

        try
        {
            m_tlsState.outPwr = m_lws.GetSourOutPwr(TLS_SLOT, TLS_CHANNEL);
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

        if (Math.Round(_wl, 3) == Math.Round(m_tlsState.wavelen, 3))
            return;

        //set it as new value.
        try
        {
            //OBand tls의 파장 범위에 맞게... ^^
            if (_wl > 1370.0)
                _wl = 1370.0;

            if (_wl < 1260.0)
                _wl = 1260.0;


            //설정.
            m_lws.SetSourWavelen(TLS_SLOT, TLS_CHANNEL, Math.Round(_wl, 3));
            m_tlsState.wavelen = Math.Round(_wl, 3);

        }
        catch
        {
            //do nothing.
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
                m_tlsState.wavelen = m_lws.GetSourWavelen(TLS_SLOT);
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

            if (_start < 1260)
                _start = 1260;

            if (_stop > 1370)
                _stop = 1370;



            m_lws.SetSourSweepWlRng(TLS_SLOT, TLS_CHANNEL,
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
            m_lws.GetSourSweepRng(TLS_SLOT, TLS_CHANNEL,
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
        m_tlsState.swpWaveStep = Math.Round(_step, 3);

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
            m_lws.SetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL, Math.Round(_speed, 1));

            m_tlsState.swpSpeed = m_lws.GetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL);
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
            m_tlsState.swpSpeed = m_lws.GetSourSweepSpeed(TLS_SLOT, TLS_CHANNEL);
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
        List<double> retList = null;

        try
        {

            retList = m_lws.GetSourLambdaLog(TLS_SLOT, TLS_CHANNEL);

        }
        catch
        {
            if (retList != null)
                retList.Clear();
            retList = null;
        }

        return retList;
    }


    /// <summary>
    /// begin sweeping as continuous mode.
    /// </summary>
    public void ExecTlsSweepCont()
    {
        try
        {
            m_lws.AsyncSourSweepStart(TLS_SLOT, TLS_CHANNEL);
            m_tlsState.wavelen = m_tlsState.swpWaveStop;
        }
        catch
        {
            //do nothing.
        }

    }



    /// <summary>
    /// query whether sweeping is operating or not.
    /// </summary>
    /// <returns>true:operating, false:stop </returns>
    public bool IsTlsSweepOperating()
    {
        bool ret = false;

        try
        {
            ret = m_lws.IsSweepping(TLS_SLOT, TLS_CHANNEL);
        }
        catch
        {
            ret = false;
        }

        return ret;
    }



    /// <summary>
    /// set Tls Logging on.
    /// </summary>
    public void TlsLogOn()
    {

        try
        {
            m_lws.SetSourLambdaLogOn(TLS_SLOT, TLS_CHANNEL);
        }
        catch
        {
            //do nothing.
        }

    }


    /// <summary>
    /// set Tls Logging off.
    /// </summary>
    public void TlsLogOff()
    {
        try
        {
            m_lws.SetSourLambdaLogOff(TLS_SLOT, TLS_CHANNEL);
        }
        catch
        {
            //do nothing.
        }
    }



    #endregion




    /// <summary>
    /// sets logging parameter of detector ports.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void SetLogParam(int[] _portNos)
    {
        for (int i = 0; i < _portNos.Length; i++)
        {
            SetLogParam(_portNos[i],
                        m_portStatList[0].logDataCount,
                        m_portStatList[0].avgTime);
        }
    }




    /// <summary>
    /// sets logging parameter of detector ports.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <param name="_dataPoint">number of datas</param>
    public void SetLogParam(int[] _portNos, int _dataPoint)
    {

        int portNo = 0;
        for (int i = 0; i < _portNos.Length; i++)
        {
            portNo = _portNos[i];
            SetLogParam(portNo, _dataPoint, m_portStatList[0].avgTime);
        }

    }




    /// <summary>
    /// sets logging parameter of detector ports. (all port)
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <param name="_dataPoint">number of datas</param>
    /// <param name="_avgTime">averaging time</param>
    public void SetLogParam(int _dataPoint, double _avgTime)
    {
        int portNo = 0;
        for (int i = 0; i < m_portPosList.Count(); i++)
        {
            portNo = m_portPosList[i].port;
            SetLogParam(portNo, _dataPoint, _avgTime);
        }
    }




    /// <summary>
    /// sets logging parameter of detector ports.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <param name="_dataPoint">number of datas</param>
    /// <param name="_avgTime">averaging time</param>
    public void SetLogParam(int[] _portNos, int _dataPoint, double _avgTime)
    {
        int portNo = 0;
        for (int i = 0; i < _portNos.Length; i++)
        {
            portNo = _portNos[i];
            SetLogParam(portNo, _dataPoint, _avgTime);
        }
    }




    /// <summary>
    /// sets logging paramater of detector port.
    /// </summary>
    /// <param name="_portNo">detector port no.</param>
    /// <param name="_numOfData">data points.</param>
    /// <param name="_avgTime">average time.</param>
    public void SetLogParam(int _portNo, int _numOfData, double _avgTime)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);



            //새값으로 세팅 한다..
            m_lws.SetSensLogParam(portPos.slot, _numOfData, _avgTime);
            portStat.logDataCount = _numOfData;
            portStat.logAvgTime = _avgTime;
            if (othPos != null)
            {
                othStat.logDataCount = _numOfData;
                othStat.logAvgTime = _avgTime;
            }


        }
        catch
        {
            //do nothing.!!
        }

    }




    /// <summary>
    /// sets the powermeter's detect range.
    /// </summary>
    /// <param name="_pwrRng">powermeter's detect range [dBm]</param>
    public void SetGainLevel(int _pwrRng)
    {

        try
        {
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                SetGainLevel(m_portPosList[i].port, _pwrRng);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets the powermeter's detect range.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <param name="_pwrRng">powermeter's detect range [dBm]</param>
    public void SetGainLevel(int[] _portNos, int _pwrRng)
    {

        try
        {
            for (int i = 0; i < _portNos.Length; i++)
            {
                SetGainLevel(_portNos[i], _pwrRng);
            }

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets the powermeter's detect range.
    /// </summary>
    /// <param name="_portNos">port no.</param>
    /// <param name="_pwrRng">powermeter's detect range [dBm]</param>
    public void SetGainLevel(int _portNo, int _pwrRng)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //기존에 셋팅 되어있는 값이면 걍 나감.
            if (portStat.gainLvl == _pwrRng)
                return;

            //새로운 값으로 설정.
            m_lws.SetSensPwrRng(portPos.slot, portPos.chnl, _pwrRng);
            portStat.gainLvl = _pwrRng;

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// gets the most postive signal entry expected for all ports.
    /// </summary>
    /// <returns></returns>
    public List<int> GetGainLevel()
    {
        List<int> retList = null;

        try
        {
            int[] ports = null;
            ports = new int[portCnt];

            for (int i = 0; i < portCnt; i++)
            {
                ports[i] = i + 1;
            }

            retList = GetGainLevel(ports);

        }
        catch
        {
            if (retList != null)
                retList.Clear();
            retList = null;
        }

        return retList;
    }




    /// <summary>
    /// get port's gain level.
    /// gets the most postive signal entry expected for a port.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    /// <returns></returns>
    public List<int> GetGainLevel(int[] _portNos)
    {

        List<int> retList = null;

        try
        {
            retList = new List<int>();

            int gainLvl = 0;
            for (int i = 0; i < _portNos.Count(); i++)
            {
                gainLvl = GetGainLevel(_portNos[i]);
                retList.Add(gainLvl);
            }

        }
        catch
        {
            if (retList != null)
                retList.Clear();
            retList = null;
        }

        return retList;

    }




    /// <summary>
    /// get port's gain level.
    /// </summary>
    /// <param name="_portNo"> port no.</param>
    public int GetGainLevel(int _portNo)
    {
        int ret = 0;

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.chnl;
            ret = m_lws.GetSensPwrRng(slot, chnl);
        }
        catch
        {
            ret = 0;
        }

        return ret;

    }




    /// <summary>
    /// sets the all powermeter's wavelength.
    /// </summary>
    /// <param name="_wavelen">wavelength [nm]</param>
    public void SetPdWavelen(double _wavelen)
    {
        try
        {
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                SetPdWavelen(m_portPosList[i].port, _wavelen);
            }
        }
        catch
        {
            //do nothing.
        }
    }




    /// <summary>
    /// sets the powermeter's wavelength.
    /// </summary>
    /// <param name="_portNos">port no.</param>
    /// <param name="_wavelen">wavelength [nm]</param>
    public void SetPdWavelen(int[] _portNos, double _wavelen)
    {
        try
        {
            for (int i = 0; i < _portNos.Length; i++)
            {
                SetPdWavelen(_portNos[i], _wavelen);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets the powermeter's wavelength.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <param name="_wavelen">wavelength [nm]</param>
    public void SetPdWavelen(int _portNo, double _wavelen)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //기존에 셋팅 되어있는 값이면 걍 나감.
            _wavelen = Math.Round(_wavelen, 3);
            if (Math.Round(portStat.wavelen, 3) == Math.Round(_wavelen, 3))
                return;

            //새로운 값으로 설정.
            m_lws.SetSensWavelen(portPos.slot, portPos.chnl, _wavelen);
            portStat.wavelen = _wavelen;

        }
        catch
        {
            //do nothing.
        }

    }




    public List<double> GetPdWavelen()
    {

        //not implemeted.
        return null;

    }




    public List<double> GetPdWavelen(int[] _ports)
    {

        //not implemeted.
        return null;

    }




    /// <summary>
    /// gets the powermeter's wavelength.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    /// <returns>pd's wavelength [nm]</returns>
    public double GetPdWavelen(int _portNo)
    {

        double ret = 0.0;


        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.chnl;
            ret = m_lws.GetSensWavelen(slot, chnl);
        }
        catch
        {
            ret = 0.0;
        }

        return Math.Round(ret, 3);
    }




    /// <summary>
    /// Set all pd to sweep mode
    /// </summary>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_step"></param>
    public void SetPdSweepMode(int _startWave, int _stopWave, double _stepWave)
    {

        int portNo = 0;
        for (int i = 0; i < m_portPosList.Count(); i++)
        {
            portNo = m_portPosList[i].port;
            SetPdSweepMode(portNo, _startWave, _stopWave, _stepWave);
        }

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

        for (int i = 0; i < _portNos.Length; i++)
        {
            SetPdSweepMode(_portNos[i], _startWave, _stopWave, _stepWave);
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

        try
        {
            int dataPoint = Convert.ToInt32((_stopWave - _startWave) / _stepWave) + 1;
            SetGainManual(_port);
            SetInTrigResSingMsr(_port);
            SetLogParam(_port, dataPoint, SENS_AVGTIME);

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// stop pd to sweep mode.
    /// </summary>
    /// <param name="_port"></param>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_step"></param>
    public void StopPdSweepMode(int _port)
    {
        SetInTrigResIgnore(_port);
    }




    /// <summary>
    /// stop pd to sweep mode.
    /// </summary>
    /// <param name="_port"></param>
    /// <param name="_startWave"></param>
    /// <param name="_stopWave"></param>
    /// <param name="_step"></param>
    public void StopPdSweepMode(int[] _portNos)
    {
        for (int i = 0; i < _portNos.Length; i++)
        {
            SetInTrigResIgnore(_portNos[i]);
        }
    }




    /// <summary>
    /// stop sweep mode ( all pd  )
    /// </summary>
    public void StopPdSweepMode()
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetInTrigResIgnore(portNo);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// 
    /// </summary>
    public void SetInTrigResIgnore()
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetInTrigResIgnore(portNo);
            }
        }
        catch
        {
            //do nothing.
        }
    }




    /// <summary>
    /// set input trigger response of port to 'ignore'
    /// </summary>
    /// <param name="_portNos">port no. array.</param>
    public void SetInTrigResIgnore(int[] _portNos)
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                SetInTrigResIgnore(portNo);
            }
        }
        catch
        {
            //do nothing.
        }
    }




    /// <summary>
    /// set input trigger response of port to 'ignore'
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetInTrigResIgnore(int _portNo)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);



            //기존에 셋팅 되어있는 값이면 걍 나감.
            if (portStat.inTrgRes == CagilLightwaveSystem.INTRIGRES_IGNORE)
            {
                if (othPos != null)
                    othStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_IGNORE;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.SetSensInTrigResp(portPos.slot,
                                    CagilLightwaveSystem.INTRIGRES_IGNORE);

            portStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_IGNORE;
            if (othPos != null)
                othStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_IGNORE;

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// set all input trigger response of port to 'single measure'
    /// </summary>
    public void SetInTrigResSingMsr()
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetInTrigResSingMsr(portNo);
            }
        }
        catch
        {
            //do nothing.
        }
    }




    /// <summary>
    /// set input trigger response of port to 'single measure'
    /// </summary>
    /// <param name="_portNos">port no. array.</param>
    public void SetInTrigResSingMsr(int[] _portNos)
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                SetInTrigResSingMsr(portNo);
            }
        }
        catch
        {
            //do nothing.
        }
    }




    /// <summary>
    /// set input trigger response of port to 'single measure'
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetInTrigResSingMsr(int _portNo)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);



            //기존에 셋팅 되어있는 값이면 걍 나감.
            if (portStat.inTrgRes == CagilLightwaveSystem.INTRIGRES_SINGLEMEASURE)
            {
                if (othPos != null)
                    othStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_SINGLEMEASURE;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.SetSensInTrigResp(portPos.slot,
                                    CagilLightwaveSystem.INTRIGRES_SINGLEMEASURE);

            portStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_SINGLEMEASURE;
            if (othPos != null)
                othStat.inTrgRes = CagilLightwaveSystem.INTRIGRES_SINGLEMEASURE;

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of ports to manual (all port)
    /// </summary>
    public void SetGainManual()
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetGainManual(portNo);
            }

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of ports to manual
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void SetGainManual(int[] _portNos)
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                SetGainManual(portNo);
            }

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of port to manual.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetGainManual(int _portNo)
    {

        try
        {
            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);



            //기존에 셋팅 되어있는 값이면 걍 나감.
            if (portStat.gainLvlAuto == false)
            {
                if (othPos != null)
                    othStat.gainLvlAuto = false;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.SetSensPwrRngManual(portPos.slot);

            portStat.gainLvlAuto = false;
            if (othPos != null)
                othStat.gainLvlAuto = false;


        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of ports to automatic (all port)
    /// </summary>
    public void SetGainAuto()
    {
        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetGainAuto(portNo);
            }

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of ports to manual
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void SetGainAuto(int[] _portNos)
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                SetGainAuto(portNo);
            }

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// sets power-range of port to automatic.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void SetGainAuto(int _portNo)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);



            //기존에 셋팅 되어있는 값이면 걍 나감.
            if (portStat.gainLvlAuto == true)
            {
                if (othPos != null)
                    othStat.gainLvlAuto = true;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.SetSensPwrRngAuto(portPos.slot);

            portStat.gainLvlAuto = true;
            if (othPos != null)
                othStat.gainLvlAuto = true;

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// set logging-mode of detect port. (all port)
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void SetPdLogMode()
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                SetPdLogMode(portNo);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// set logging-mode of detect port.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void SetPdLogMode(int[] _portNos)
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                SetPdLogMode(portNo);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// set port to logging mode.
    /// </summary>
    /// <param name="_port">port no.</param>
    /// Logging mode 2번 들어가게되면... 이상해짐.
    /// 절대로 Logging mode는 slot단위로 들어가고 나가야 됨.
    public void SetPdLogMode(int _port)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _port);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _port);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_port != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);


            //기존에 셋팅 되어있는 값과 동일하면 그냥 나감.
            if (portStat.logMode == true)
            {
                if (othPos != null)
                    othStat.logMode = true;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.SetSensLogFunc(portPos.slot);
            portStat.logMode = true;
            if (othPos != null)
                othStat.logMode = true;

        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// stop logging-mode of detect port. (all port)
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void StopPdLogMode()
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < m_portPosList.Count(); i++)
            {
                portNo = m_portPosList[i].port;
                StopPdLogMode(portNo);
            }
        }
        catch
        {
            //do nothing.
        }

    }




    /// <summary>
    /// stop logging-mode of detect port.
    /// </summary>
    /// <param name="_portNos">port no. array</param>
    public void StopPdLogMode(int[] _portNos)
    {

        try
        {
            int portNo = 0;
            for (int i = 0; i < _portNos.Length; i++)
            {
                portNo = _portNos[i];
                StopPdLogMode(portNo);
            }
        }
        catch
        {
            //do nothing.
        }

    }



    /// <summary>
    /// stop logging-mode of detect port.
    /// </summary>
    /// <param name="_portNo">port no.</param>
    public void StopPdLogMode(int _portNo)
    {

        try
        {

            //port position & state.
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            CportState portStat = null;
            portStat = m_portStatList.Find(ps => ps.port == _portNo);


            //other port position & state.
            CportPos othPos = null;
            int nOtherPortNo = -1;
            for (int i = 0; i < m_portCnt; i++)
            {
                othPos = m_portPosList[i];
                if ((portPos.slot == othPos.slot) &&
                    (_portNo != othPos.port))
                {
                    nOtherPortNo = othPos.port;
                    break;
                }
                othPos = null;
            }

            CportState othStat = null;
            if (othPos != null) //슬롯에 다른 포트가 존재함.(거의 대부분 그렇다.)
                othStat = m_portStatList.Find(ps => ps.port == nOtherPortNo);


            //기존에 셋팅 되어있는 값과 동일하면 그냥 나감.
            if (portStat.logMode == false)
            {
                if (othPos != null)
                    othStat.logMode = false;
                return;
            }


            //새값으로 세팅 한다..
            m_lws.StopSensLogFunc(portPos.slot);
            portStat.logMode = false;
            if (othPos != null)
                othStat.logMode = false;

        }
        catch
        {
            //do nothing.
        }

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
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.chnl;
            m_lws.GetSensLogParam(slot, ref _dataPoints, ref _avgTime);
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
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.chnl;
            ret = m_lws.GetSensPwrUnit(slot, chnl);
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
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            int chnl = portPos.chnl;
            ret = m_lws.GetSensAvgTime(slot);
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
        bool ret = false;

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            ret = m_lws.IsSensLogMode(slot);
        }
        catch
        {
            ret = false;
        }

        return ret;
    }




    /// <summary>
    /// gets input trigger response of detect port.
    /// </summary>
    /// <param name="_portNo"></param>
    /// <returns></returns>
    public int GetInTrigResp(int _portNo)
    {
        int ret = 0;

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            ret = m_lws.GetSensInTrigResp(slot);
        }
        catch
        {
            ret = 0;
        }

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

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _portNo);
            int slot = portPos.slot;
            ret = m_lws.IsSensRangeAuto(slot);
        }
        catch
        {
            ret = false;
        }

        return ret;

    }




    /// <summary>
    /// read optical power.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns>optical power [mw]</returns>
    public double ReadPwr(int _port)
    {
        double ret = 0;

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _port);
            ret = m_lws.ReadSensPwr(portPos.slot, portPos.chnl);
        }
        catch
        {
            ret = 0.0000000001;
        }

        return ret;
    }




    /// <summary>
    /// read optical power.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns>optical power [mw]</returns>
    public List<double> ReadPwr(int[] _ports)
    {
        List<double> retList = null;

        try
        {
            retList = new List<double>();
            for (int i = 0; i < _ports.Length; i++)
            {
                retList.Add(ReadPwr(_ports[i]));
            }
        }
        catch
        {
            if (retList != null)
                retList.Clear();
            retList = null;
        }

        return retList;
    }




    /// <summary>
    /// read optical power.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns>optical power [w]</returns>
    public List<double> ReadPwr()
    {
        List<double> retList = null;

        try
        {

            int[] ports = null;
            ports = new int[portCnt];

            for (int i = 0; i < portCnt; i++)
            {
                ports[i] = i + 1;
            }

            retList = ReadPwr(ports);

        }
        catch
        {
            if (retList != null)
                retList.Clear();
            retList = null;
        }

        return retList;
    }




    /// <summary>
    /// get logging data of all ports.
    /// </summary>
    /// <param name="_ports"></param>
    /// <returns></returns>
    public List<List<double>> GetPwrLog()
    {
        List<List<double>> ret = null;

        try
        {
            List<int> ports = new List<int>();
            for (int i = 1; i <= portCnt; i++)
            {
                ports.Add(i);
            }


            ret = GetPwrLog(ports.ToArray());
        }
        catch
        {
            ret = null;
        }

        return ret;

    }




    /// <summary>
    /// get logging data of port.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns></returns>
    public List<List<double>> GetPwrLog(int[] _ports)
    {
        List<List<double>> ret = null;

        try
        {
            ret = new List<List<double>>();

            for (int i = 0; i < _ports.Length; i++)
            {
                ret[i] = GetPwrLog(_ports[i]);
            }
        }
        catch
        {
            if (ret != null)
                ret.Clear();
            ret = null;
        }

        return ret;
    }




    /// <summary>
    /// get logging data of port.
    /// </summary>
    /// <param name="_port"></param>
    /// <returns></returns>
    public List<double> GetPwrLog(int _port)
    {
        List<double> retList = null;

        try
        {
            CportPos portPos = m_portPosList.Find(_p => _p.port == _port);
            retList = m_lws.GetSensLog(portPos.slot, portPos.chnl);
        }
        catch
        {
            retList = null;
        }

        return retList;
    }


    #endregion

}