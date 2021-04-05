using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NationalInstruments.NI4882;
using System.Runtime.InteropServices;

public class AgilentFrameSystem
{


    #region definition

    public const int TLS_OUTPUT_PATH_HIGH = 0;              //Highpower
    public const int TLS_OUTPUT_PATH_LOWS = 1;              //LowSSE
    public const int TLS_OUTPUT_PATH_BHR = 2;               //Both outputs are active but only the High Power output is Regulated.
    public const int TLS_OUTPUT_PATH_BLR =3;                //Both outputs are active but only the Low SSE output is Regulated.
														    
    public const int MODULEKIND_EMPTY = 0;                  //Empty
    public const int MODULEKIND_SOURCE = 1;                 //Source
    public const int MODULEKIND_SENSOR = 2;                 //Sensor
    public const int MODULEKIND_ATTENUATOR = 3;             //Attenuator
    
    public const int SWEEPMODE_STEP = 0;					//sweep mode step.
    public const int SWEEPMODE_MANUAL = 1;					//sweep mode manual.
    public const int SWEEPMODE_CONT = 2;					//sweep mode continuos.

    public const int INTRIGRES_IGNORE = 0;                  //Ignore incomming trigger
    public const int INTRIGRES_SINGLEMEASURE = 1;           //Start Single measurement
    public const int INTRIGRES_COLMELTEMEASURE = 2;         //Start Complete measurement
    public const int INTRIGRES_NEXTSTEP = 3;                //Perform next step of a stepped sweep
    public const int INTRIGRES_SWSTART = 4;                 //Start a sweep cycle

    public const int TRIGGEN_DISABLE = 0;					//Never
    public const int TRIGGEN_AVGOVER = 1;					//averaging time period finishes.
    public const int TRIGGEN_MEASURE = 2;					//When averaging time period begins.
    public const int TRIGGEN_MODULATION = 3;				//for every leading edge of a digitally-modulated(TTL) signal.
    public const int TRIGGEN_STFINISHED = 4;				//When a sweep step finishes.
    public const int TRIGGEN_SWFINISHED = 5;				//When a sweep cycle finishes.
    public const int TRIGGEN_SWSTARTED = 6;					//When a sweep cycle starts.

    public const int TRIGGER_CONFIG_DISABLE = 0;            //Triger connectors are disable.
    public const int TRIGGER_CONFIG_DEFAULT = 1;            //default...
    public const int TRIGGER_CONFIG_PASSTHROUGH = 2;		//Passthrough
    public const int TRIGGER_CONFIG_LOOPBACK = 3;           //Loopback

    public const int POWER_UNIT_DBM = 0;                    //dBm
    public const int POWER_UNIT_WATT = 1;                   //Watt

    #endregion




    #region Structure

    public struct Slot
    {
        public int slotNo;									//Axis
        public int nModuleKind;								//Module Kind...
    }

    //private struct Command
    //{
    //    public bool bStartSweep;
    //    public bool bStopSweep;
    //}

    public struct SweepParam
    {
        public double nStartWL;                             //Start Wavelength
        public double nStopWL;                              //Stop Wavelength
        public double dbStep;                               //Step
        public double dbSpeed;                              //Sweep Speed [nm/s]
        public double avgTime;								//Average Time [ms]
        public double[] dbSensorChannelNoArr;				//Power를 Logging할 Sensor Slot Number들  
        public int nSensorPowRange;							//Dector의 Power Range
    }

    #endregion




    #region Member Variables

    private int m_gpibAddr;         //GPIB address
    private bool m_bConnectedOK;    //연결 상태!!
    private Device mDevice;
    private Slot[] m_arrSlot;

    #endregion




    #region consturctor/destructor

    //생성자.
    public AgilentFrameSystem()
    {
        m_gpibAddr = -1;
        m_bConnectedOK = false;

        mDevice = null;
        m_arrSlot = null; 
    }   

    #endregion




    #region property

    public int gpibAddr { get { return m_gpibAddr; } }

	public bool ActiveDevice { get { return mDevice != null ? true : false; }  }

	#endregion




	#region Private method

	static object mLock = new object();

	private string Query(string _cmd)
	{

		lock (mLock)
		{
			//Neon.Aligner.Log.Write($"[Query] cmd = {_cmd} ");
			mDevice.Write(_cmd);
			var response = mDevice.ReadString().Trim();
			//Neon.Aligner.Log.Write($"[Query] response = {response} ");
			return response;
		}

	}

	

	/// <summary>
	/// binaray data block -> data array( double type).
	/// </summary>
	/// <param name="_sourBuf">binary data block</param>
	/// <param name="_valSize"> byte size of data.</param>
	/// <param name="_dataPoints">number of datas.</param>
	/// <returns></returns>
	private List<double> BinDataBlockToList(byte[] _sourBuf, int _valSize)
    {
        List<double> retList = null;

        try
        {

            int dataPoint = _sourBuf.Length / _valSize;
            retList = new List<double>();


            //Byte 단위의 데이터를 Double 타입으로 변환해준다.!!
            IntPtr ptrTarget = Marshal.AllocHGlobal(_valSize);
            int nSourceBufIndex = 0;
            float[] tempFloatArr = new float[1];
            double[] tempDblArr = new double[1];
            double data = 0; //마샬링 된 데이터
            for (int i = 0; i < dataPoint; i++)
            {
                Marshal.Copy(_sourBuf, nSourceBufIndex, ptrTarget, _valSize);

                try
                {

                    if (_valSize == 4)
                    {
                        //4Byte//
                        Marshal.Copy(ptrTarget, tempFloatArr, 0, 1);
                        data = tempFloatArr[0];
                    }
                    else if (_valSize == 8)
                    {
                        //8Byte//
                        Marshal.Copy(ptrTarget, tempDblArr, 0, 1);
                        data = tempDblArr[0];
                    }


                    //황당한 데이터를 고쳐준다.
                    if (data <= 0)
                        data = 0.000000000001;
                }
                catch
                {
                    data = 0.000000000001;
                }



                


                retList.Add(data);

                nSourceBufIndex += _valSize;

            }

            Marshal.FreeHGlobal(ptrTarget);


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
    /// wait for idle state
    /// </summary>
    private void WaitForGpibIdle()
    {

        while (true)
        {
            string strResponse = Query("*OPC?");
            if (strResponse.Contains("1")) break;
        }

    }


	#endregion




	#region Public method

	const int mDefaultBufferSize = 161000;

    /// <summary>
    ///  816x에 연결한다.
    /// </summary>
    /// <param name="_gpibAddr"> GPIB Address </param>
    /// <returns> true : Connection is completed , false : Connection is fail.</returns>
    public bool ConnectByGpib(int _gpibBoard, int _gpibAddr)
    {

        //Variables..
        bool bRet = true;
        try
        {
			//gpib 객체 생성 및 연결
			if (mDevice == null)
			{
				mDevice = new Device(_gpibBoard, Convert.ToByte(_gpibAddr));
				mDevice.DefaultBufferSize = mDefaultBufferSize;
			}
			m_bConnectedOK = true;

            //Identification을 물어본다.
            string strResponse = "";
            strResponse = Query("*IDN?");
			if (strResponse.IndexOf("816") < 0) throw new ApplicationException();

            m_gpibAddr = _gpibAddr;
            m_bConnectedOK = true;
        }
        catch
        {
            //메모리 해제
            if (mDevice != null)
            {
                mDevice.Dispose();
                mDevice = null;
            }
            m_bConnectedOK = false;
            bRet = false;
        }
        return bRet;
    }



    //////////////////////////////////////////////////////////////
    //SetSlot ///////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - slot를 설정한다.
    //
    //Param - [IN] slotNo : Slot Number
    //            [IN] nModuleKind : Kind of Module
    //
    public void SetSlot(int slotNo, int nModuleKind)
    {

        //Find position of Array
        bool bFindOK = false;
        int i = 0;
        if (m_arrSlot == null)
        {
            m_arrSlot = new Slot[1];
            i = 0;
        }
        else
        {

            for (i = 0; i <= m_arrSlot.GetLength(0) - 1; i++)
            {
                if (m_arrSlot[i].slotNo == slotNo)
                {
                    bFindOK = true;
                    break; 
                }
            }

            if (false == bFindOK)
            {
                Array.Resize<Slot>(ref m_arrSlot, m_arrSlot.GetLength(0) + 1);
                i = m_arrSlot.GetLength(0) - 1;
            }

        }


        //Module Kind을 설정
        m_arrSlot[i].nModuleKind = nModuleKind;

    }



    /// <summary>
    /// sets wavelength of source.
    /// </summary>
    /// <param name="_slotNo">slot no. of source.</param>
    /// <param name="_chnlNo">channel no. of source</param>
    /// <param name="_wavelen">wavelength [nm]</param>
    public void SetSourWavelen(int _slotNo, int _chnlNo, double _wavelen)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Send Command
            _wavelen = Math.Round(_wavelen, 3);
            string strCommand = "";
            strCommand = "SOURCE" + _slotNo.ToString();
            strCommand += ":CHAN" + _chnlNo.ToString();
            strCommand += ":WAVE " + _wavelen.ToString() + "NM";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }

    }



    /// <summary>
    /// gets wavelength of source.
    /// </summary>
    /// <param name="_slotNo">slot no. of source</param>
    /// <returns>wavelength[nm]</returns>
    public double GetSourWavelen(int _slotNo)
    {

        double dbRet = 0;
        try
        {
            //query
            string strCommand = "";
            string strResponse = "";
            strCommand = "SOURCE" + _slotNo.ToString();
            strCommand += ":WAVE?";
            strResponse = Query(strCommand);

            dbRet = Convert.ToDouble(strResponse) * 1000000000;
            dbRet = Math.Round(dbRet, 3);

        }
        catch
        {
            dbRet = 0;
        }


        return dbRet;

    }



    /// <summary>
    /// sets output power of source.
    /// </summary>
    /// <param name="_slotNo">slot no. of source.</param>
    /// <param name="_chnlNo">channel no of source.</param>
    /// <param name="_pwr">optical power of source</param>
    public void SetSourOutPwr(int _slotNo, int _chnlNo, double _pwr)
    {
        //Send Command
        _pwr = Math.Round(_pwr, 3);
        string strCommand = "";
        strCommand = "SOUR" + _slotNo.ToString();
        strCommand += ":CHAN" + _chnlNo.ToString();
        strCommand += ":POW " + Convert.ToString(_pwr) + "dBm";
        mDevice.Write(strCommand);
        WaitForGpibIdle();

    }



    /// <summary>
    /// gets output power of source.
    /// </summary>
    /// <param name="_slotNo">slot no. of source</param>
    /// <param name="_chnlNo">cahnnal no. of source</param>
    /// <returns> [dBm]</returns>
    public double GetSourOutPwr(int _slotNo, int _chnlNo)
    {

        double dbRet = -100;

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException("gpib connection is fail..");

            string strCommand = "";
            string strResponse = "";

            ////unit을 dBm으로한다.
            //strCommand = "SOUR" + Convert.ToString(_slotNo);
            //strCommand = strCommand + ":UNIT DBM";
            //m_gpibDev.Write(strCommand);
            //WaitForGpibIdle();

            //power를 얻는다.
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":POW?";
            strResponse =  Query(strCommand);

            dbRet = Math.Round( Convert.ToDouble(strResponse), 3);

        }
        catch 
        {
            dbRet = -100;
        }


        return dbRet;

    }


	
    /// <summary>
    /// set output-path of source.
    /// </summary>
    /// <param name="_slotNo">number of slot.</param>
    /// <param name="_chnlNo">number of channel no.</param>
    /// <param name="_path">high,lows,bhr,blr</param>
    public void SetSourOutPath(int _slotNo, int _chnlNo, int _path)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Check parameter...
            if ((_path < TLS_OUTPUT_PATH_HIGH) || (_path > TLS_OUTPUT_PATH_BLR))
                throw new ApplicationException();

            //Send Command
            string strCommand = "";
            strCommand = "OUTP" + _slotNo.ToString() + ":PATH ";
            switch (_path)
            {
                case TLS_OUTPUT_PATH_HIGH:
                    strCommand += "HIGH";
                    break;
                case TLS_OUTPUT_PATH_LOWS:
                    strCommand += "LOWS";
                    break;
                case TLS_OUTPUT_PATH_BHR:
                    strCommand += "BHR";
                    break;
                case TLS_OUTPUT_PATH_BLR:
                    strCommand += "BLR";
                    break;
            }
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing
        }

    }


	    
    /// <summary>
    /// source power on
    /// </summary>
    /// <param name="_slotNo">slot no of source.</param>
    /// <param name="_chnlNo">channel no of souece.</param>
    public void SourPowerOn(int _slotNo, int _chnlNo)
    {
		
        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();
			
            //Send Command
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo) + ":";
            strCommand += "POW:STAT 1";
            mDevice.Write(strCommand);
            WaitForGpibIdle();
			
        }
        catch
        {
            //do nothing.
        }


    }


    /// <summary>
    /// source power off
    /// </summary>
    /// <param name="_slotNo">slot no. of source.</param>
    /// <param name="_chnlNo">channel no of source</param>
    public void SourPowerOff(int _slotNo, int _chnlNo)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Send Command
            string strCommand = "";
            strCommand = "SOUR" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            strCommand += "POW:STAT 0";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }

    }



    public int GetSourPowerState(int _slotNo, int _chnlNo)
    {
        try
        {
            //Send Command
            string strCommand = $"SOUR{_slotNo}:CHAN{_chnlNo}:POW:STAT?";
            return Query(strCommand).Contains("1")? 1 : 0;

        }
		catch (NationalInstruments.NI4882.GpibException ex)
		{
			Neon.Aligner.Log.Write($"GPIB exception error : {ex.Message}");
			if (ex.InnerException != null) Neon.Aligner.Log.Write($"{ex.InnerException.Message}");
			return -1;
		}
		catch (Exception ex)
		{
			Neon.Aligner.Log.Write($"{ex.Message}\r\n{ex.StackTrace}");
			if (ex.InnerException != null) Neon.Aligner.Log.Write($"{ex.InnerException.Message}");
			return -1;
		}


    }


	public float GetSourZeroTemp(int _slotNo)
	{
		try
		{
			//Send Command
			string strCommand = $"SOUR{_slotNo}:wav:corr:zero:temp:act?";
			return float.Parse(Query(strCommand));

		}
		catch (Exception)
		{
			return -1;
		}


	}


	public float GetSourZeroTempLast(int _slotNo)
	{
		try
		{
			//Send Command
			string strCommand = $"SOUR{_slotNo}:wav:corr:zero:temp:last?";
			return float.Parse(Query(strCommand));

		}
		catch (Exception)
		{
			return -1;
		}


	}



	public string ReadErrorMessage()
    {
        try
        {
            //Send Command
            string strCommand = "SYSTem:ERRor?";
            return Query(strCommand);

        }
        catch (Exception ex)
        {
            return $"GPIB ERROR : {ex.Message}";
        }
		
    }



    /// <summary>
    /// set sweep mode
    /// </summary>
    /// <param name="_slotNo">slot no of source.</param>
    /// <param name="_chnlNo">channel no of source.</param>
    /// <param name="_mode">sweep mode (STEP,MANUAL,CONTINUE)</param>
    public void SetSourSweepMode(int _slotNo, int _chnlNo, int _mode)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK)
                throw new ApplicationException();


            //Send Command
            string strCommand = "";
            strCommand = "SOUR" + _slotNo.ToString();
            strCommand += ":WAV:SWE:MODE ";

            switch (_mode)
            {
                case SWEEPMODE_STEP:
                    strCommand += "STEP";
                    break;
                case SWEEPMODE_MANUAL:
                    strCommand += "MAN";
                    break;
                case SWEEPMODE_CONT:
                    strCommand += "CONT";
                    break;
            }

            mDevice.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing
        }

    }
	


    /// <summary>
    /// sets the sweep speed.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <param name="_speed">sweep speed [nm/s]</param>
    public void SetSourSweepSpeed(int _slotNo, int _chnlNo, double _speed)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK)
                throw new ApplicationException("gpib connection is fail..");


            //Send Command
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":WAV:SWE:SPE " + Convert.ToString(_speed) + "nm/s";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing
        }


    }
	


    /// <summary>
    /// get the speed for sweeping.
    /// </summary>
    /// <param name="_slotNo">slot no. of tls.</param>
    /// <param name="_chnlNo">channel no. of tls.</param>
    /// <returns> sweep speed [nm]</returns>
    public double GetSourSweepSpeed(int _slotNo, int _chnlNo)
    {

        double ret = 0.0;

        try
        {
            //Check Connection
            if (false == m_bConnectedOK)
                throw new ApplicationException("gpib connection is fail..");

            
            //query
            string strCommand = "";
            string strResponse = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":WAV:SWE:SPE?";
            strResponse = Query(strCommand);
            try
            {
                //[m/s] -> [n/s]
                ret = Convert.ToDouble(strResponse) * 1000000000;
                ret = Math.Round(ret ,1);
            }
            catch
            {
                ret = 0.0;
            }


        }
        catch
        {
            ret = 0.0;
        }

        return ret;

    }



    /// <summary>
    /// sets the wavelnegth range for sweep.
    /// </summary>
    /// <param name="_slotNo">slot no. of source</param>
    /// <param name="_chnlNo">channel no. of source</param>
    /// <param name="_startWl">start wavelength</param>
    /// <param name="_stopWl">stop wavelength</param>
    /// <param name="_step">wavelength step</param>
    public void SetSourSweepWlRng(int _slotNo, int _chnlNo, 
                                  double _startWl, double _stopWl, double _step)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK)
                throw new ApplicationException("gpib connection is fail..");


            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SOUR" + _slotNo.ToString();
            strCommand += ":CHAN" + _chnlNo.ToString();
            strCommand += ":WAV:SWE:START " + _startWl.ToString() + "nm";
            mDevice.Write(strCommand);
            WaitForGpibIdle();


            //Sets the stop point of the sweep
            strCommand = "";
            strCommand = "SOUR" + _slotNo.ToString();
            strCommand += ":CHAN" + _chnlNo.ToString();
            strCommand += ":WAV:SWE:STOP " + _stopWl.ToString() + "nm";
            mDevice.Write(strCommand);
            WaitForGpibIdle();


            //Sets the stop point of the sweep
            strCommand = "";
            strCommand = "SOUR" + _slotNo.ToString();
            strCommand += ":CHAN" + _chnlNo.ToString() ;
            strCommand += ":WAV:SWE:STEP " + _step.ToString() + "nm";
            mDevice.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing
        }


    }

  

    /// <summary>
    /// gets the sweep range of optical source.
    /// </summary>
    /// <param name="_slotNo"></param>
    /// <param name="_chnlNo"></param>
    /// <param name="_startWl"></param>
    /// <param name="_stopWl"></param>
    /// <param name="_step"></param>
    public void GetSourSweepRng(int _slotNo, int _chnlNo, 
                                ref int _startWl, ref int _stopWl, ref double _step)
    {

        try
        {

            double temp = 0;

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Get the starting point of the sweep
            string strCommand = "";
            string strResponse = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand = strCommand + ":WAV:SWE:START?";
            strResponse =  Query(strCommand);
            try
            {
                temp = Convert.ToDouble(strResponse) * 1000000000;
                _startWl = Convert.ToInt32(temp);
            }
            catch
            {
                _startWl = 0;
            }

            //Gets the stop point of the sweep
            strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand = strCommand + ":WAV:SWE:STOP?";
            strResponse = Query(strCommand);
            try
            {
                temp = Convert.ToDouble(strResponse) * 1000000000;
                _stopWl = Convert.ToInt32(temp);
            }
            catch
            {
                _stopWl = 0;
            }

            //gets the step of the sweep
            strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand = strCommand + ":WAV:SWE:STEP?";
            strResponse = Query(strCommand);
            try
            {
                _step = Math.Round(Convert.ToDouble(strResponse) * 1000000000, 3);
            }
            catch
            {
                _step = 0;
            }

        }
        catch 
        {
            _startWl = 0;
            _stopWl = 0;
            _step = 0;
        }


    }


	
    /// <summary>
    /// sets input trigger response of sensor.
    /// Can only be sent to master channel, slave channel is also affected
    /// </summary>
    /// <param name="_slotNo">slot no of sensor.</param>
    /// <param name="_resp">response</param>
    public void SetSensInTrigResp(int _slotNo, int _resp)
    {
		
        try
        {
            //Check Connection
            if (false == m_bConnectedOK)throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand += "TRIG" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            switch (_resp)
            {
                case INTRIGRES_IGNORE:
                    strCommand += "INP IGN";
                    break;
                case INTRIGRES_SINGLEMEASURE:
                    strCommand += "INP SME";
                    break;
                case INTRIGRES_COLMELTEMEASURE:
                    strCommand += "INP CME";
                    break;
                case INTRIGRES_NEXTSTEP:
                    strCommand += "INP NEXT";
                    break;
                case INTRIGRES_SWSTART:
                    strCommand += "INP SW";
                    break;
            }

            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }


    }
	


    /// <summary>
    /// gets input trigger response of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no of sensor</param>
    /// <returns>response</returns>
    public int GetSensInTrigResp(int _slotNo)
    {

        int nRet = -1;

        try
        {

            if (false == m_bConnectedOK) throw new ApplicationException();

            string strCommand = "";
            string strResponse = "";
            strCommand = "TRIG" + _slotNo + ":";
            strCommand += "CHAN1:INP?";
            strResponse = Query(strCommand);

            if (strResponse.ToUpper() == "IGN")
                nRet = INTRIGRES_IGNORE;
            else if (strResponse.ToUpper()  == "SME")
                nRet = INTRIGRES_SINGLEMEASURE;
            else if (strResponse.ToUpper() == "CME")
                nRet = INTRIGRES_COLMELTEMEASURE;
            else if (strResponse.ToUpper() == "NEXT")
                nRet = INTRIGRES_NEXTSTEP;
            else if (strResponse.ToUpper() == "SW")
                nRet = INTRIGRES_SWSTART;

        }
        catch
        {
            nRet = -1;
        }


        return nRet;

    }



    /// <summary>
    /// sets input trigger response of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no of sensor</param>
    /// <param name="_resp">response</param>
    public void SetSensInTrigRes(int _slotNo, int _resp)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();


            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "TRIG" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            switch (_resp)
            {
                case INTRIGRES_IGNORE:
                    strCommand += "INP IGN";
                    break;
                case INTRIGRES_SINGLEMEASURE:
                    strCommand += "INP SME";
                    break;
                case INTRIGRES_COLMELTEMEASURE:
                    strCommand += "INP CME";
                    break;
                case INTRIGRES_NEXTSTEP:
                    strCommand += "INP NEXT";
                    break;
                case INTRIGRES_SWSTART:
                    strCommand += "INP SW";
                    break;
            }

            mDevice.Write(strCommand);
            WaitForGpibIdle();


        }
        catch 
        {
            //do nothing
        }


    }



    /// <summary>
    /// sets the wavelength of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <param name="_wl">wavelength</param>
    public void SetSensWavelen(int _slotNo, int _chnlNo, double _wl)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //send command
            string strCommand = "";
            strCommand = "SENS" + Convert.ToString(_slotNo) + ":";
            strCommand += "CHAN" + Convert.ToString(_chnlNo) + ":";
            strCommand += "POW:WAV " + Convert.ToString(_wl) + "NM";

            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }


    }


	
    /// <summary>
    /// gets the wavelength of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <returns></returns>
    public double GetSensWavelen(int _slotNo, int _chnlNo)
    {

        double dbWL = 0;

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //query
            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + Convert.ToString(_slotNo) + ":";
            strCommand = strCommand + "CHAN" + Convert.ToString(_chnlNo) + ":";
            strCommand = strCommand + "POW:WAV?";
            strResponse = Query(strCommand);

            dbWL = Convert.ToDouble(strResponse) * 1000000000;
            dbWL = Math.Round(dbWL, 4);

        }
        catch
        {
            //do nothing
        }


        return dbWL;

    }



    /// <summary>
    /// sets the output trigger.
    /// </summary>
    /// <param name="_slotNo">slot no. of detector</param>
    /// <param name="_chnlNo">channel no. of detector</param>
    /// <param name="nWhen">when an output trigger is generated</param>
    public void SetSensOutTrig(int _slotNo, int _chnlNo, int nWhen)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "TRIG" + Convert.ToString(_slotNo) + ":";
            strCommand = strCommand + "CHAN" + Convert.ToString(_chnlNo) + ":";
            strCommand = strCommand + "OUTP ";
            switch (nWhen)
            {
                case TRIGGEN_DISABLE:
                    strCommand += "DIS";
                    break;
                case TRIGGEN_AVGOVER:
                    strCommand += "AVG";
                    break;
                case TRIGGEN_MEASURE:
                    strCommand += "MEAS";
                    break;
                case TRIGGEN_MODULATION:
                    strCommand += "MOD";
                    break;
                case TRIGGEN_STFINISHED:
                    strCommand += "STF";
                    break;
                case TRIGGEN_SWFINISHED:
                    strCommand += "SWF";
                    break;
                case TRIGGEN_SWSTARTED:
                    strCommand += "SWS";
                    break;
            }
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //nothing
        }

    }



    /// <summary>
    /// Set the most postive signal entry expected for a sensor
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor.</param>
    /// <param name="_chnlNo">channel no. of sensor.</param>
    /// <param name="_pwr">optical power [dBm]</param>
    public void SetSensPwrRng(int _slotNo, int _chnlNo, int _pwr)
    {
        
        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException("gpib connection is fail..");

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            strCommand += "POW:RANGE " + _pwr.ToString() + "dBm";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //nothing
        }


    }
	


    /// <summary>
    /// Get the most postive signal entry expected for a sensor
    /// </summary>
    /// <param name="_slotNo">slot no. of detector</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <returns></returns>
    public int GetSensPwrRng(int _slotNo, int _chnlNo)
    {
		
        int nRet = 100;

        try
        {

            if (false == m_bConnectedOK) return nRet;

            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            strCommand += "POW:RANGE?";
            strResponse = Query(strCommand);
           
            nRet = (int)(Convert.ToDouble(strResponse));

        }
        catch 
        {
            nRet = 100;
        }

        return nRet;

    }



    /// <summary>
    /// sets the range of sensor to prouce the most dynamic range
    /// without overloading
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor</param>
    public void SetSensPwrRngAuto(int _slotNo)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            strCommand += "POW:RANGE:AUTO 1";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }

    }


	    
    /// <summary>
    /// disable automatic power ranging for the module.
    /// </summary>
    /// <param name="_slotNo">slot no of moudle.</param>
    public void SetSensPwrRngManual(int _slotNo)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            strCommand += "POW:RANGE:AUTO 0";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }

    }


	
    //////////////////////////////////////////////////////////////                           
    //Sync_SetSensorPowerUnit ///////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - TLS의 Power Unit을 설정한다.
    //
    //Param - [IN] slotNo : slot number
    //           [IN] chnlNo : Channel number
    //           [IN] nUnit : Power Unit
    //
    public void Sync_SetTLSPowerUnit(int slotNo, int chnlNo, int nUnit)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SOURCE" + Convert.ToString(slotNo) + ":";
            strCommand = strCommand + "CHAN" + Convert.ToString(chnlNo) + ":";
            switch (nUnit)
            {
                case POWER_UNIT_DBM:
                    strCommand = strCommand + "POW:UNIT 0";
                    break;
                case POWER_UNIT_WATT:
                    strCommand = strCommand + "POW:UNIT 1";
                    break;
            }
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }


    }


	
    /// <summary>
    /// sets the power unit of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor.</param>
    /// <param name="_chnlNo">channel no. of sensor.</param>
    /// <param name="_unit">dBm, watt</param>
    public void  SetSensPwrUnit(int _slotNo, int _chnlNo, int _unit)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SENSE" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            switch (_unit)
            {
                case POWER_UNIT_DBM:
                    strCommand += "POW:UNIT 0";
                    break;
                case POWER_UNIT_WATT:
                    strCommand += "POW:UNIT 1";
                    break;
            }
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing.
        }


    }


	
    /// <summary>
    /// gets the power unit of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no. of senosr</param>
    /// <param name="_chnlNo">channel no. of sensor</param>
    /// <returns></returns>
    public int GetSensPwrUnit(int _slotNo, int _chnlNo)
    {

        int nRet = -1;

        try
        {

            if (false == m_bConnectedOK) return nRet;

            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            strCommand += "POW:UNIT?";
            strResponse = Query(strCommand);
            nRet = Convert.ToInt32(strResponse);     // 0: dBm , 1:Watt
            
        }
        catch 
        {
            nRet = -1;
        }


        return nRet;

    }
	


    /// <summary>
    /// sets the averaging time for the module.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_avgTime">sensing average time</param>
    public void SetSensAvgTime(int _slotNo, double _avgTime)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException("gpib connection is fail..");

            //Sets the starting point of the sweep
            string strCommand = "";
            strCommand = "SENS" + _slotNo.ToString();
            strCommand += ":POW:ATIME " + _avgTime.ToString() + "MS";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing.
        }


    }



    /// <summary>
    /// gets the averaging time of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor</param>
    /// <param name="_chnlNo">channel no. of sensor</param>
    /// <returns></returns>
    public double GetSensAvgTime(int _slotNo)
    {

        double dbRet = -1;

        try
        {

            if (false == m_bConnectedOK) return dbRet;

            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            strCommand += "POW:ATIME?";
            strResponse =  Query(strCommand);
            dbRet = Math.Round(Convert.ToDouble(strResponse)*1000, 1);     // [ms]
            
        }
        catch 
        {
            dbRet = -1;
        }


        return dbRet;

    }
	


    /// <summary>
    /// set sensor  to logging mode and enable logging data acqusition func
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor</param>
    public void SetSensLogFunc(int _slotNo)
    {
        string strCommand = $"SENS{_slotNo}:FUNC:STAT LOGG,STAR";
        mDevice.Write(strCommand);
        WaitForGpibIdle();
    }

	

    /// <summary>
    /// whether logging mode or not. 
    /// </summary>
    /// <param name="_slotNo"></param>
    /// <returns>true:logging mode , false:not </returns>
    public bool IsSensLogMode(int _slotNo)
    {
        string strResponse = Query($"SENS{_slotNo}:FUNC:STAT?");
        return !strResponse.Contains("NONE");
    }
	


    /// <summary>
    /// whether ranging auto or manual. 
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor.</param>
    /// <returns>true:auto or false:manaul</returns>
    public bool IsSensRangeAuto(int _slotNo)
    {

        bool bRet = false;

        try
        {

            if (false == m_bConnectedOK) bRet = false;

            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "CHAN1:";
            strCommand += "POW:RANGE:AUTO?";
            strResponse = Query(strCommand);
            if ( Convert.ToInt32(strResponse) == 0)
                bRet = false;   //manual
            else
                bRet = true;    //auto

        }
        catch
        {
            bRet = false;
        }


        return bRet;


    }



    /// <summary>
    /// set logging mode parameter of senser.
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor</param>
    /// <param name="_dataPoints">number of data points</param>
    /// <param name="_avgTime">sensing averaging time</param>
    public void SetSensLogParam(int _slotNo, int _dataPoints, double _avgTime)
    {
        
        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //parameter setting
            string strCommand = "";
            strCommand = "SENS" + _slotNo.ToString() + ":";
            strCommand += "FUNC:PAR:LOGG ";
            strCommand += _dataPoints.ToString() + ",";
            strCommand += _avgTime.ToString() + "ms";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing.
        }

    }



    /// <summary>
    /// get logging parameter of sensor.
    /// </summary>
    /// <param name="_slotNo">slot no. of sensor.</param>
    /// <param name="_dataPoints">number of data points</param>
    /// <param name="_avgTime">sensing averaging time</param>
    /// <returns></returns>
    public bool GetSensLogParam(int _slotNo, ref int _dataPoints, ref double _avgTime)
    {

        bool ret = false;

        try
        {

            if (false == m_bConnectedOK) return false;

            string strCommand = "";
            string strResponse = "";
            strCommand = "SENS" + Convert.ToString(_slotNo) + ":";
            strCommand += "FUNC:PAR:LOGG?";
            strResponse =  Query(strCommand);

            string[] strTempArr = strResponse.Split(',');
            _dataPoints = Convert.ToInt32(strTempArr[0]);
            _avgTime = Math.Round(Convert.ToDouble(strTempArr[1]) * 1000, 1);  //[ms]

            ret = true;

        }
        catch 
        {
            _dataPoints = -1;
            _avgTime = -1;
            ret =  false;
        }

        return ret;
    }

	

    /// <summary>
    /// Disable logging data acquistion function mode
    /// </summary>
    /// <param name="_slotNo"></param>
    public void StopSensLogFunc(int _slotNo)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Stop logging data acquistion function mode
            string strCommand = "";
            strCommand = "SENSE" + Convert.ToString(_slotNo) + ":";
            strCommand += "CHAN1:";
            strCommand += "FUNC:STAT LOGG,STOP ";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing.
        }


    }

	

    /// <summary>
    /// sets the number of sweep cycle.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <param name="_cycle">number of sweep cycle</param>
    public void SetSourSweepCycle(int _slotNo, int _chnlNo, int _cycle)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //parameter setting
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":WAV:SWE:CYCL " + Convert.ToString(_cycle);
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing.
        }


    }


	
    /// <summary>
    /// Sets the hardware trigger configuration with 
    /// regard to Output and Input Trigger Connectors
    /// </summary>
    /// <param name="_conf"></param>
    public void SetConfTrig(int _conf)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Send... ^^
            string strCommand = "";
            strCommand = "TRIG:CONF " + Convert.ToString(_conf);
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing
        }


    }


	
    /// <summary>
    /// sets the output trigger
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <param name="_when">when an output trigger is generated</param>
    public void SetSourOutTrig(int _slotNo, int _chnlNo, int _when)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Send... ^^
            string strCommand = "";
            strCommand = "TRIG" + Convert.ToString(_slotNo) + ":OUTP ";
            switch (_when)
            {
                case TRIGGEN_DISABLE:
                    strCommand += "DIS";
                    break;
                case TRIGGEN_AVGOVER:
                    strCommand += "AVG";
                    break;
                case TRIGGEN_MEASURE:
                    strCommand += "MEA";
                    break;
                case TRIGGEN_MODULATION:
                    strCommand += "MOD";
                    break;
                case TRIGGEN_STFINISHED:
                    //When a sweep step finishes.
                    strCommand += "STF";
                    break;
                case TRIGGEN_SWFINISHED:
                    //When a sweep cycle finishes
                    strCommand += "STF";
                    break;
                case TRIGGEN_SWSTARTED:
                    //When a sweep cycle start
                    strCommand += "SWS";
                    break;
            }

            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch 
        {
            //do nothing
        }


    }

	

    /// <summary>
    /// Disable amplitude modulation of the laser output.
    /// </summary>
    /// <param name="_slotNo"></param>
    /// <param name="_chnlNo"></param>
    public void SetSourAmpModeOff(int _slotNo, int _chnlNo)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK)
                throw new ApplicationException();


            //Send... ^^
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":AM:STAT 0";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing
        }

    }



    /// <summary>
    /// lambda logging on
    /// </summary>
    /// <param name="_slotNo">slot no. of optical source</param>
    /// <param name="_chnlNo">channel no. of optical source</param>
    public void SetSourLambdaLogOn(int _slotNo, int _chnlNo)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Switches Lamda logging on
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":CHAN" + Convert.ToString(_chnlNo);
            strCommand += ":WAV:SWE:LLOG 1";
            mDevice.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing.
        }


    }
	
   

    /// <summary>
    /// lambda logging off
    /// </summary>
    /// <param name="_slotNo">slot no. of optical source</param>
    /// <param name="_chnlNo">channel no. of optical source</param>
    public void SetSourLambdaLogOff(int _slotNo, int _chnlNo)
    {

        try
        {

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Switches Lamda logging on
            string strCommand = "";
            strCommand = "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":WAV:SWE:LLOG 0";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing
        }
		
    }

	

    //////////////////////////////////////////////////////////////                                       
    //SetAttenuatorAlpha ////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc -  Attenuator의 Alpha값 설정
    //
    public void Sync_SetAttenuatorAlpha(int slotNo, double dbValue)
    {


        try
        {

            //Check Connection
            if (false == m_bConnectedOK)
            {
                throw new ApplicationException();
            }


            //Switches Lamda logging on
            string strCommand = "";
            strCommand = "INP" + Convert.ToString(slotNo) + ":ATT " + Convert.ToString(dbValue);
            mDevice.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing
        }



    }


	
    //////////////////////////////////////////////////////////////                                      
    //GetAttenuatorAlpha ////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc - Attenuator의 Alpha값을 얻는다.
    //
    public double Sync_GetAttenuatorAlpha(int slotNo)
    {
        //Variables..
        double dbRet = -100;

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Switches Lamda logging on
            string strCommand = "";
            string strResponse = "";
            strCommand = "INP" + Convert.ToString(slotNo) + ":ATT? ";
            //mDevice.Write(strCommand);
            //strResponse = mDevice.ReadString();
			strResponse = Query(strCommand);
            dbRet = Convert.ToDouble(strResponse);

        }
        catch
        {
            dbRet = -100;
        }

        return dbRet;

    }
	


    //////////////////////////////////////////////////////////////                                       
    //SetAttenuatorAlpha ////////////////////////////////////
    //////////////////////////////////////////////////////////////
    //desc -  Attenuator의  wavelength값 설정
    //
    public void Sync_SetAttenuatorLambda(int slotNo, double dbValue)
    {

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Switches Lamda logging on
            string strCommand = "";
            strCommand = "INP" + Convert.ToString(slotNo) + ":CHAN1:WAV " + Convert.ToString(dbValue) + "NM";
            mDevice.Write(strCommand);
            WaitForGpibIdle();

        }
        catch
        {
            //do nothing.
        }


    }


	
    ////////////////////////////////////////////////////////////////
    ////GetAttenuatorAlpha ///////////////////////////////////////
    ////////////////////////////////////////////////////////////////
    ////desc - Attenuator의 Wavelength값을 얻는다.
    ////
    public double Sync_GetAttenuatorLambda(int slotNo)
    {
        //Variables..
        double dbWavelength = 0;

        try
        {

            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //Switches Lamda logging on
            string strCommand = "";
            string strResponse = "";
            strCommand = "INP" + Convert.ToString(slotNo) + ":CHAN1:WAV?";
            //mDevice.Write(strCommand);
            //strResponse = mDevice.ReadString();
			strResponse = Query(strCommand);

            dbWavelength = Convert.ToDouble(strResponse) * Math.Pow(10, 9);
            dbWavelength = Math.Round(dbWavelength, 4);

        }
        catch 
        {
            dbWavelength = 0;
        }

        return dbWavelength;

    }

		

    /// <summary>
    /// performs the sweep
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    public void AsyncSourSweepStart(int _slotNo, int _chnlNo)
    {
        //Start Sweep!!//(state 1 : run sweep)
        string strCommand = "SOURCE" + Convert.ToString(_slotNo);
        strCommand += ":WAV:SWE:STATE 1";
        mDevice.Write(strCommand);
        WaitForGpibIdle();
		Thread.Sleep(1000);
    }



    /// <summary>
    /// stop the sweep
    /// </summary>
    public void SweepStop()
    {
        //Stop Sweep!!
        string strCommand = "";
        strCommand += "SOURCE0:WAV:SWE:STATE 0";
        mDevice.Write(strCommand);
        WaitForGpibIdle();
    }


	
    /// <summary>
    /// query sweeping state.
    /// </summary>
    /// <param name="_slotNo">slot no. of optical source.</param>
    /// <param name="_chnl">channel no. of optical source.</param>
    /// <returns></returns>
    public bool IsSweepping(int _slotNo, int _chnl)
    {

        bool bRet = false;
		
        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException("gpib connection is fail..");

            //send query!!
            string strCommand = "";
            string strResponse = "";
            strCommand += "SOUR" + Convert.ToString(_slotNo);
            strCommand += ":CHAN" + Convert.ToString(_chnl);
            strCommand += ":WAV:SWE:STATE?";
            strResponse = Query(strCommand);
            if (1 == Convert.ToInt32(strResponse) )
                bRet = true;    //Sweep is running//
            else
                bRet = false; //completed!!//

        }
        catch
        {
            bRet = false;
        }

        return bRet;

    }
	

 
    /// <summary>
    /// get the lambda logging data of optical source.
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <param name="_dataPoints">data points</param>
    /// <returns>lambda logging data.</returns>
    public List<double> GetSourLambdaLog(int _slotNo, int _chnlNo)
    {
        try
        {
            Monitor.Enter(mDevice);

			#region old
			//Data points를 얻는다!!
			//mDevice.Write($"SOUR{_slotNo}:READ:POIN? LLOG");
			//int numDataPoint = int.Parse(mDevice.ReadString());
			//int numDataPoint = int.Parse(Query($"SOUR{_slotNo}:READ:POIN? LLOG"));

			//mDevice.Write($"SOUR{_slotNo}:READ:DATA? LLOG");

			////Read number of digit
			//byte[] byteResponse = mDevice.ReadByteArray(2);
			//int numDigitOfDataLength = int.Parse(Encoding.ASCII.GetString(byteResponse, 1, 1));

			////Read size of binary data block
			//int blockSize = int.Parse(Encoding.ASCII.GetString(mDevice.ReadByteArray(numDigitOfDataLength)));
			//if (blockSize != numDataPoint * 8)
			//    MyLogic.log($"AgilentFrameSystem.GetSourlambdaLog(): BlickSize 이상: {blockSize}{numDataPoint * 8}");

			////----------------- Logging data ------------------------------------
			//byteResponse = mDevice.ReadByteArray(blockSize);
			//List<double> data = BinDataBlockToList(byteResponse, 8);

			//var bytes = mDevice.ReadByteArray(calcBinarySize(numDataPoint, 8)); 
			#endregion
			
			mDevice.Write($"SOUR{_slotNo}:READ:DATA? LLOG");
			var bytes = mDevice.ReadByteArray();
			var data = parseBinaryAs<double>(bytes);

			//nm단위로 바꾸고 소수점 4째자리 수로 바꾼다.!!            
			return data.Select((x) => Math.Round(x * 1000000000, 4)).ToList();//0.1pm
        }

        finally { Monitor.Exit(mDevice); }
    }


    
    /// <summary>
    /// Get the double type list of the last data acquisition function of sensor
    /// </summary>
    /// <param name="_slotNo">slot no.</param>
    /// <param name="_chnlNo">channel no.</param>
    /// <returns> optical power data array [mw] </returns>
    public List<double> GetSensLog(int _slotNo, int _chnlNo)
    {

		string cmd = "";
		var w = new System.Diagnostics.Stopwatch();

		try
        {
            Monitor.Enter(mDevice);
			
			if (_chnlNo % 2 == 1)									//PowerMeter "Master channel"만 동작
			{
				//complete 될때까지... 대기..
				cmd = $"SENS{_slotNo}:CHAN{_chnlNo}:FUNC:STATE?";
				int miliSecond = 3000;
				w = System.Diagnostics.Stopwatch.StartNew();
				while (true)
				{
					if (Query(cmd).Contains("COMPLETE")) break;

					//20초 경과후 종료
					if (w.ElapsedMilliseconds > miliSecond)
					{
						string msg = $"[{gpibAddr:00}][{DateTime.Now.ToString("")}] AgilentFrameSystem.GetSensLog({_slotNo}, {_chnlNo:00}): "
								   + $"Log Function이 {miliSecond / 1000}초 동안 완료되지 않음!";
						MyLogic.log(msg);
						break;
					}
					Thread.Sleep(250);
				}
				MyLogic.log($"[{gpibAddr:00}][{DateTime.Now.ToString("")}] AgilentFrameSystem.GetSensLog({_slotNo}, {_chnlNo:00}): wait= {w.ElapsedMilliseconds}ms");
				w.Restart(); 
			}

			//int dataPoint = 0;
			//double avgTime = 0;
			//GetSensLogParam(_slotNo, ref dataPoint, ref avgTime);

			// -----   Logging data Header -----
			cmd = $"SENS{_slotNo}:CHAN{_chnlNo}:FUNC:RES?";
            mDevice.Write(cmd);

			#region old
			////Read number of digit
			//byte[] byteResponse = mDevice.ReadByteArray(2);
			//int nDigitNum = int.Parse(Encoding.ASCII.GetString(byteResponse, 1, 1));
			//int blockSize = int.Parse(Encoding.ASCII.GetString(mDevice.ReadByteArray(nDigitNum)));

			////Data를 읽는다.
			//byteResponse = mDevice.ReadByteArray(blockSize);
			//var data = BinDataBlockToList(byteResponse, 4);
			//MyLogic.log($"[{gpibAddr}]AgilentFrameSystem.GetSensLog({_slotNo}, {_chnlNo}): read={w.ElapsedMilliseconds}ms");
			//return data.Select((x) => x * 1000).ToList();//watt-> mW

			//var bytes = mDevice.ReadByteArray(calcBinarySize(dataPoint, 4)); 
			#endregion

			var bytes = mDevice.ReadByteArray();
			var data = parseBinaryAs<float>(bytes);
			return data.Select(x => (double)x * 1000).ToList();//watt-> mW

		}

        finally { Monitor.Exit(mDevice); }
    }

		

	static T[] parseBinaryAs<T>(byte[] byteBuffer)
	{

		//length of binary block
		int numDigits = int.Parse(Encoding.ASCII.GetString(byteBuffer, 1, 1));
		int byteLengthOfData = int.Parse(Encoding.ASCII.GetString(byteBuffer, 2, numDigits));
		int indexData = 2 + numDigits;

		int byteLengthOfT = Marshal.SizeOf(default(T));
		var numValue = byteLengthOfData / byteLengthOfT;
		T[] data = new T[numValue];
		if (numValue > 0) Buffer.BlockCopy(byteBuffer, indexData, data, 0, byteLengthOfData);

		return data;
	}

	//static int calcBinarySize(int numDataPoint, int elementSize)
	//{
	// 	return (int)Math.Floor(Math.Log10(numDataPoint * elementSize)) + 3 + numDataPoint * elementSize + 1;
	//}



	/// <summary>
	/// read optical power from sensor.
	/// </summary>
	/// <param name="_slotNo">slot no. of sensor</param>
	/// <param name="_chnlNo">channel no. of sensor</param>
	/// <returns>optical power [mw]</returns>
	public double ReadSensPwr(int _slotNo, int _chnlNo)
    {
		
        double dbPwr = 0;

        try
        {
            //Check Connection
            if (false == m_bConnectedOK) throw new ApplicationException();

            //send query!!
            string strCommand = "";
            string strResponse = "";
            strCommand = "FETCH" + _slotNo.ToString() + ":";
            strCommand += "CHAN" + _chnlNo.ToString() + ":";
            strCommand +=  "POW?";
            strResponse = Query(strCommand);
            dbPwr = Convert.ToDouble(strResponse) * 1000;
            dbPwr = Math.Abs(dbPwr);

        }
        catch 
        {
            dbPwr = 0.0000000001;
        }

        //return
        return dbPwr;


    }

    public double ReadPowerTrig_mW(int slot, int ch)
    {
        double mw;
        var ok = double.TryParse(Query($"READ{slot}:CHAN{ch}:POW?"), out mw);//watt
        return ok ? mw * 1000 : 1e-6;
    }


    public void WriteAtt(int slot, decimal att_dB)
    {
        mDevice.Write($"INP{slot}:ATT {att_dB}dB");
    }
    public double ReadAtt(int slot)
    {
        return double.Parse(Query($"INP{slot}:ATT?"));
    }
    public void WriteAttOffset(int slot, decimal att_dB)
    {
        mDevice.Write($"INP{slot}:OFFS {att_dB}dB");
    }
    public double ReadAttOffset(int slot)
    {
        return double.Parse(Query($"INP{slot}:OFFS?"));
    }



    #endregion




}
