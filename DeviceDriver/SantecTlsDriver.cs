using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.NI4882;
using System.Threading;
using System.Runtime.InteropServices;

namespace Neon.Aligner
{
    public class SantecTlsDriver
    {



        #region definition

        private const int UNIT_DBM = 0;
        private const int UNIT_MW = 1;

        private const int TRIGOUT_NONE = 0;
        private const int TRIGOUT_STOP = 1;
        private const int TRIGOUT_START = 2;
        private const int TRIGOUT_STEP = 3;

        private const int SWEEPMODE_STEP1 = 0;  //step operiation, one-way
        private const int SWEEPMODE_CONT1 = 1;  //contiuous operation, one-way
        private const int SWEEPMODE_STEP2 = 2;  //step operiation, two-way
        private const int SWEEPMODE_CONT2 = 3;  //contiuous operiation, two-way

        private const int SWEEPSTATE_STOP = 0;
        private const int SWEEPSTATE_OPERATION = 1;
        private const int SWEEPSTATE_PAUSE = 2;
        private const int SWEEPSTATE_TRIGSTANDBY = 3;

        #endregion




        #region member variable

        private int m_gpibAddr;
        private bool m_bConnectedOK;
        private Device m_gpibDev;

        #endregion


        #region property

        public int gpibAddress { get { return m_gpibAddr; } }

        #endregion



        #region private method

        /// <summary>
        /// send command & receive response.
        /// </summary>
        /// <param name="_cmd"></param>
        /// <returns></returns>
        private string Query(string _cmd)
        {

            string strRet = "";


            //Check Connection
            if (false == m_bConnectedOK)
                return strRet;


            //query.
            try
            {

                Monitor.Enter(m_gpibDev);

                //send
                strRet = "";
                m_gpibDev.Write(_cmd);

                //recv
                strRet = m_gpibDev.ReadString();
                strRet = strRet.Replace("\n", "");

            }
            catch
            {
                strRet = "";
            }
            finally
            {
                Monitor.Exit(m_gpibDev);
            }


            return strRet;

        }



        /// <summary>
        /// wait for idle state
        /// </summary>
        private void WaitForGpibIdle()
        {


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException("gpib connection is fail..");


                //query.
                string strCommand = "";
                string strResponse = "";
                strCommand = "*OPC?";
                while (true)
                {
                    strResponse = Query(strCommand);
                    if (Convert.ToInt32(strResponse) == 1)
                        break;
                }



            }
            catch
            {
                //do nothing
            }


        }








        /// <summary>
        /// sets the sweep start wavelength
        /// </summary>
        /// <param name="_wavelen">wavelength</param>
        private void SetSweepStart(double _wavelen)
        {

            try
            {
                string strCmd = ":WAV:SWE:STAR ";
                strCmd += Math.Round(_wavelen, 3);
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }

        }


        /// <summary>
        /// gets the sweep start wavelength
        /// </summary>
        /// <returns></returns>
        private double GetSweepStart()
        {
            double ret = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV:SWE:STAR?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(ret), 3);


            }
            catch
            {
                ret = -100;
            }

            return ret;
        }



        /// <summary>
        /// sets the sweep stop wavelength
        /// </summary>
        /// <param name="_wavelen">wavelength</param>
        private void SetSweepStop(double _wavelen)
        {

            try
            {
                string strCmd = ":WAV:SWE:STOP ";
                strCmd += Math.Round(_wavelen, 3);
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }

        }


        /// <summary>
        /// gets the sweep stop wavelength
        /// </summary>
        /// <returns></returns>
        private double GetSweepStop()
        {
            double ret = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV:SWE:STOP?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(ret), 3);


            }
            catch
            {
                ret = -100;
            }

            return ret;
        }




        /// <summary>
        /// sets the sweep wavelength step
        /// </summary>
        /// <param name="_wavelen">wavelength</param>
        private void SetSweepStep(double _step)
        {

            try
            {
                string strCmd = ":WAV:SWE:STEP ";
                strCmd += Math.Round(_step, 3);
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }

        }


        /// <summary>
        /// gets the sweep wavelength step
        /// </summary>
        /// <returns></returns>
        private double GetSweepStep()
        {
            double ret = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV:SWE:STEP?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(ret), 3);


            }
            catch
            {
                ret = -100;
            }

            return ret;
        }




        /// <summary>
        /// sweep
        /// </summary>
        /// <param name="_mode"></param>
        private void Sweep(int _mode)
        {
            try
            {
                string strCmd = ":WAV:SWE " + _mode.ToString();
                m_gpibDev.Write(strCmd);
                Thread.Sleep(700);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }
        }


        /// <summary>
        /// Reads out the current sweep mode state.
        /// </summary>
        private int GetSweepState()
        {

            int ret = -1;

            try
            {
                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV:SWE?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Convert.ToInt32(strResponse);

            }
            catch
            {
                ret = -1;
            }

            return ret;
        }



        /// <summary>
        /// set power unit of tls.
        /// </summary>
        /// <param name="_unit"></param>
        private void SetPwrUnit(int _unit)
        {
            try
            {
                string strCommand = ":POW:UNIT ";
                strCommand += _unit.ToString();
                m_gpibDev.Write(strCommand);
            }
            catch
            {
                //do nothing.
            }
        }




        /// <summary>
        /// Sets the timing of the trigger signal output.
        /// </summary>
        /// <param name="_mode">out-trigger mode</param>
        private void SetTrigOutMode(int _mode)
        {
            try
            {
                string strCommand = ":TRIG:OUTP ";
                strCommand += _mode.ToString();
                m_gpibDev.Write(strCommand);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }
        }

        #endregion




        #region public method



        public bool HasWaveLogging = false;

        /// <summary>
        ///  connect to device.
        /// </summary>
        /// <param name="_gpibAddr"> GPIB Address </param>
        /// <returns> true : Connection is completed , false : Connection is fail.</returns>
        public bool ConnectByGpib(int _gpibAddr)
        {

            //Variables..
            bool bRet = true;


            try
            {

                //gpib 객체 생성 및 연결
                if (m_gpibDev == null)
                    m_gpibDev = new Device(0, Convert.ToByte(_gpibAddr));
                m_bConnectedOK = true;


                //Identification을 물어본다.
                string strResponse = "";
                strResponse = Query("*IDN?");
                if (strResponse.IndexOf("SANTEC") < 0) throw new ApplicationException();
                HasWaveLogging = strResponse.Contains("550");

                m_gpibAddr = _gpibAddr;
                m_bConnectedOK = true;


            }
            catch
            {

                //메모리 해제
                if (m_gpibDev != null)
                {
                    m_gpibDev.Dispose();
                    m_gpibDev = null;
                }

                m_bConnectedOK = false;
                bRet = false;

            }


            return bRet;


        }







        /// <summary>
        /// set power unit of tls to watt
        /// </summary>
        public void SetPwrUnitMwatt()
        {
            SetPwrUnit(UNIT_MW);
        }


        /// <summary>
        /// set power unit of tls to watt
        /// </summary>
        public void SetPwrUnitDBm()
        {
            SetPwrUnit(UNIT_DBM);
        }


        /// <summary>
        /// set trigger output mode to NONE.
        /// </summary>
        public void SetTrigOutModeNone()
        {
            SetTrigOutMode(TRIGOUT_NONE);
        }


        /// <summary>
        /// set trigger output mode to START.
        /// </summary>
        public void SetTrigOutModeStart()
        {
            SetTrigOutMode(TRIGOUT_START);
        }


        /// <summary>
        /// set trigger output mode to STOP.
        /// </summary>
        public void SetTrigOutModeStop()
        {
            SetTrigOutMode(TRIGOUT_STOP);
        }


        /// <summary>
        /// set trigger output mode to STEP.
        /// </summary>
        public void SetTrigOutModeStep()
        {
            SetTrigOutMode(TRIGOUT_STEP);
        }





        /// <summary>
        /// Sets the interval of the trigger signal output.
        /// </summary>
        /// <param name="_step">range  0.005 – 120 [nm] (type A/B)</param>
        public void SetTriggerOutStep(double _step)
        {
            try
            {
                string strCommand = ":TRIG:OUTP:STEP ";
                strCommand += _step.ToString();
                m_gpibDev.Write(strCommand);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }
        }



        /// <summary>
        /// Sets enable of the external trigger signal input.
        /// </summary>
        public void SetTriggerInExtEnable()
        {
            try
            {
                string strCommand = ":TRIG:INP:EXT 1";
                m_gpibDev.Write(strCommand);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }

        }



        /// <summary>
        /// Sets disable of the external trigger signal input.
        /// </summary>
        public void SetTriggerInExtDisable()
        {
            try
            {
                string strCommand = ":TRIG:INP:EXT 0";
                m_gpibDev.Write(strCommand);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing
            }
        }




        /// <summary>
        /// set output optical power.
        /// </summary>
        /// <param name="_pwr">optical power [dBm]</param>
        public void SetOutPwr(double _pwr)
        {
            try
            {
                string strCmd = ":POW ";
                strCmd += Math.Round(_pwr, 3).ToString();
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
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
        public double GetOutPwr()
        {

            double ret = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":POW?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(strResponse), 3);


            }
            catch
            {
                ret = -100;
            }

            return ret;

        }


        /// <summary>
        /// set wavelength
        /// </summary>
        /// <param name="_wl">wavelength [nm]</param>
        public void SetWavelen(double _wl)
        {
            try
            {
                //var wl = $"{_wl:F3}";
                //string strCmd = $":WAV {wl};*OPC?";
                //         m_gpibDev.Write(strCmd);
                //m_gpibDev.ReadString();

                string strCmd = $":WAV {_wl}";
                m_gpibDev.Write(strCmd);
                Thread.Sleep(1500);
                WaitForGpibIdle();
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
        public double GetWavelen()
        {
            double ret = 0.0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(strResponse), 3);

            }
            catch
            {
                ret = 0.0;
            }

            return ret;
        }



        /// <summary>
        /// get wavelength logging data
        /// </summary>
        /// <returns> if function fails, it returns null. [nm]</returns>
        public List<double> GetWavelenLog()
        {
            List<double> retList = null;
            try
            {
                Monitor.Enter(m_gpibDev);

                //send
                m_gpibDev.Write(":READ:DATA?");

                ////Read number of digit
                //byte[] byteResponse = byteResponse = m_gpibDev.ReadByteArray(2);
                //int nDigitNum = int.Parse(Encoding.ASCII.GetString(byteResponse, 1, 1));

                ////Size of binary data block
                //byteResponse = m_gpibDev.ReadByteArray(nDigitNum);
                //int nBinaryDataBlockSize = int.Parse(Encoding.ASCII.GetString(byteResponse));

                //Data를 읽는다.
                var byteResponse = m_gpibDev.ReadByteArray(999999);
                Monitor.Exit(m_gpibDev);

                var waves = BinaryParser.To<int>(byteResponse);
                retList = waves.Select(x => Math.Round(x / 10000.0, 3)).ToList();
            }
            catch
            {
                if (retList != null) retList.Clear();
                retList = null;
            }
            finally
            {
                if(Monitor.IsEntered(m_gpibDev)) Monitor.Exit(m_gpibDev);
            }
            return retList;
        }



        /// <summary>
        /// Sets the sweep speed for continuous sweep.
        /// </summary>
        /// <param name="_speed">sweep speed [n/m]
        ///                      range - 1.0 ~ 100[nm/s],
        ///                      step 0.1 [nm/s] </param>
        public void SetSweepSpeed(double _speed)
        {
            try
            {
                string strCmd = ":WAV:SWE:SPE ";
                strCmd += Math.Round(_speed, 1).ToString();
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing.
            }
        }



        /// <summary>
        /// Gets the sweep speed 
        /// </summary>
        /// <returns>sweep speed [n/m]</returns>
        public double GetSweepSpeed()
        {
            double ret = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();

                //query
                string strCommand = ":WAV:SWE:SPE?";
                string strResponse = "";
                strResponse = Query(strCommand);

                ret = Math.Round(Convert.ToDouble(strResponse), 1);


            }
            catch
            {
                ret = 0;
            }

            return ret;
        }



        /// <summary>
        ///  Sets the amount of time spent during each step in step sweep operation.
        ///  The wait time between each sweep is also set to the same value.
        ///  This wait time does not include the time required for wavelength conversion.
        /// </summary>
        /// <param name="_time">0~999.9 [s], step - 0.1 [s]</param>
        public void SetSweepDwell(double _time)
        {
            try
            {
                string strCmd = ":WAV:SWE:SPE ";
                strCmd += Math.Round(_time, 1).ToString();
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing.
            }
        }




        /// <summary>
        /// set wavelength range for sweep.
        /// </summary>
        /// <param name="_start">start wavelength</param>
        /// <param name="_stop">stop wavelength</param>
        /// <param name="_step">wavelength step</param>
        public void SetSweepRange(int _start, int _stop, double _step)
        {
            SetSweepStart(_start);
            SetSweepStop(_stop);
            SetSweepStep(_step);
            SetTriggerOutStep(_step);
        }


        /// <summary>
        /// get wavelength range for sweep.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_stop"></param>
        /// <param name="_step"></param>
        public void GetSweepRange(ref int _start, ref int _stop, ref double _step)
        {
            _start = (int)GetSweepStart();
            _stop = (int)GetSweepStop();
            _step = GetSweepStep();
        }


        /// <summary>
        /// Sets the number of wavelength sweeps.
        /// </summary>
        public void SetSweepCycle(int _cycle)
        {
            try
            {
                string strCmd = ":WAV:SWE:CYCL " + _cycle.ToString();
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing.
            }
        }


        /// <summary>
        /// set on the LD current.
        /// </summary>
        public void LaserOn()
        {
            try
            {
                string strCmd = ":POW:STAT 1";
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing.
            }
        }



        /// <summary>
        /// set off the LD current.
        /// </summary>
        public void LaserOff()
        {
            try
            {
                string strCmd = ":POW:STAT 0";
                m_gpibDev.Write(strCmd);
                WaitForGpibIdle();
            }
            catch
            {
                //do nothing.
            }
        }



        /// <summary>
        /// start sweeping as continuous mode.
        /// </summary>
        public void SweepCont()
        {
            Sweep(SWEEPMODE_CONT1);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// start sweeping as step mode.
        /// </summary>
        public void SweepStep()
        {
            Sweep(SWEEPMODE_STEP1);
        }


        /// <summary>
        /// query whether sweeping is operating or not.
        /// </summary>
        /// <returns>true:operating, false:stop </returns>
        public bool IsSweeping()
        {
            if (GetSweepState() == SWEEPSTATE_OPERATION)
                return true;
            else
                return false;
        }


        #endregion







    }

}