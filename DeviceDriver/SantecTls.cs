using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Neon.Aligner
{
    public class SantecTls : Itls
    {

        #region definition

        private const int DEFAULT_SWEEPSTART = 1260;
        private const int DEFAULT_SWEEPSTOP = 1360;
        private const double DEFAULT_SWEEPSTEP = 0.05;
        private const int DEFAULT_SWEEPSPEED = 40; //[nm/s]
        private const int SWEEP_CYCLE = 1;

        #endregion



        #region private member variables

        private SantecTlsDriver mTls;

        private double m_wavelen;       //TLS의 현재 wavelength. [nm]
        private double m_pow;           //TLS의 현재 output power [dBm]

        private int m_swpWaveStart;
        private int m_swpwaveStop;
        private double m_swpWaveStep;

        #endregion



        #region constructor/desconsrtor

        /// <summary>
        /// default constructor
        /// </summary>
        public SantecTls()
        {
            mTls = new SantecTlsDriver();

            m_wavelen = 0;
            m_pow = -100;

            m_swpWaveStart = 0;
            m_swpwaveStop = 0;
            m_swpWaveStep = 0;
        }


        #endregion




        #region public method


        /// <summary>
        /// connect by gpib.
        /// </summary>
        /// <param name="_gpibAddr">gpib address</param>
        /// <returns></returns>
        public bool ConnectByGpib(int _gpib)
        {
            bool ret = false;

            try
            {
                ret = mTls.ConnectByGpib(_gpib);
            }
            catch
            {
                ret = false;
            }

            return ret;
        }




        /// <summary>
        /// Initialize device.
        /// </summary>
        /// <returns></returns>
        public bool Init(ConfigTlsParam param)
        {
            bool ret = false;

            try
            {
                mTls.SetTrigOutModeStep();
                
                
                mTls.SetSweepCycle(SWEEP_CYCLE);
                //mTls.SetSweepRange(param.WaveStart, param.WaveStop, param.WaveStep);
                SetTlsSweepRange(param.WaveStart, param.WaveStop, param.WaveStep);
                //mTls.SetSweepSpeed(param.Speed);
                SetTlsSweepSpeed(param.Speed);
                //mTls.SetOutPwr(param.Power);
                SetTlsOutPwr(param.Power);

                mTls.SetPwrUnitDBm();
                mTls.LaserOn();

                ret = true;
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
            //not supported!!
        }


        /// <summary>
        /// set Tls Logging off.
        /// </summary>
        public void TlsLogOff()
        {
            //not supported!!
        }



        /// <summary>
        /// set output optical power.
        /// </summary>
        /// <param name="_pwr">optical power [dBm]</param>
        public void SetTlsOutPwr(double _pwr)
        {

            if (Math.Round(m_pow, 2) == Math.Round(_pwr, 2))
                return;

            try
            {
                mTls.SetOutPwr(_pwr);
                m_pow = Math.Round(_pwr, 2);
            }
            catch
            {
                m_pow = -100;
            }

        }




        /// <summary>
        /// get output optical power;
        /// </summary>
        /// <returns>function fail : -100 [dBm]</returns>
        public double GetTlsOutPwr()
        {


            if ((int)m_pow != -100)
                return Math.Round(m_pow, 2);


            double ret = 0;

            try
            {
                ret = mTls.GetOutPwr();
            }
            catch
            {
                ret = -100;
            }

            m_pow = Math.Round(ret, 2);


            return ret;

        }


        /// <summary>
        /// set wavelength
        /// </summary>
        /// <param name="_wl">wavelength [nm]</param>
        public void SetTlsWavelen(double _wl)
        {
            if (Math.Round(m_wavelen, 4) == Math.Round(_wl, 4))
                return;

            try
            {

                if (_wl > 1360)
                    _wl = 1360;

                if (_wl < 1260)
                    _wl = 1260;


                mTls.SetWavelen(_wl);
                m_wavelen = Math.Round(_wl, 4);
                mTls.GetWavelen();
            }
            catch
            {
                m_wavelen = 0;
            }


        }





        /// <summary>
        /// get wavelength
        /// </summary>
        /// <returns>wavelength [nm]</returns>
        public double GetTlsWavelen()
        {

            if (m_wavelen != 0)
                return Math.Round(m_wavelen, 4);


            double ret = 0;

            try
            {
                ret = mTls.GetWavelen();
                ret = Math.Round(ret, 4);

                m_wavelen = ret;

            }
            catch
            {
                ret = 0;
            }


            return ret;
        }



        /// <summary>
        /// get wavelength logging data
        /// type a,b는 wavelength logging을 지원하지 않는다.
        /// 그래서 그냥 logging data를 만들어준다.
        /// </summary>
        /// <returns> if function fails, it returns null. [nm]</returns>
        public List<double> GetTlsWavelenLog()
        {

            List<double> retList = null;

            if (!mTls.HasWaveLogging)
            {
                retList = new List<double>();
                int dataPoint = (int)((m_swpwaveStop - m_swpWaveStart) / m_swpWaveStep) + 1;
                double wavelen = m_swpWaveStart;
                for (int i = 0; i < dataPoint; i++)
                {
                    retList.Add(wavelen);
                    wavelen = wavelen + m_swpWaveStep;
                    wavelen = Math.Round(wavelen, 3);
                }
            }
            else return mTls.GetWavelenLog();            
            
            return retList;
        }



        /// <summary>
        /// Sets the sweep speed for continuous sweep.
        /// </summary>
        /// <param name="_speed">sweep speed [n/m]
        ///                      range - 1.0 ~ 100[nm/s],
        ///                      step 0.1 [nm/s] </param>
        public void SetTlsSweepSpeed(double _speed)
        {
            try
            {
                mTls.SetSweepSpeed(_speed);
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
        public double GetTlsSweepSpeed()
        {
            double ret = 0;

            try
            {
                ret = mTls.GetSweepSpeed();
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }



        /// <summary>
        /// set wavelength range for sweep.
        /// </summary>
        /// <param name="_start">start wavelength</param>
        /// <param name="_stop">stop wavelength</param>
        /// <param name="_step">wavelength step</param>
        public void SetTlsSweepRange(int _start, int _stop, double _step)
        {

            if ((m_swpWaveStart == _start) &&
                (m_swpwaveStop == _stop) &&
                ((Math.Round(m_swpWaveStep, 3)) == (Math.Round(_step, 3))))
                return;


            try
            {
                mTls.SetSweepRange(_start, _stop, _step);
                mTls.SetTrigOutModeStep();

                m_swpWaveStart = _start;
                m_swpwaveStop = _stop;
                m_swpWaveStep = Math.Round(_step, 3);

            }
            catch
            {
                //do nothing.
            }

        }



        /// <summary>
        /// get wavelength range for sweep.
        /// </summary>
        /// <param name="_start"></param>
        /// <param name="_stop"></param>
        /// <param name="_step"></param>
        public void GetTlsSweepRange(ref int _start, ref int _stop, ref double _step)
        {


            _start = m_swpWaveStart;
            _stop = m_swpwaveStop;
            _step = m_swpWaveStep;


            //기존에 저장된 데이터 있으면 바로
            if ((_start != 0) && (_stop != 0) && (_step != 0))
                return;


            //기존에 저장된 데이터 없으면 장비에게 물어본다.
            try
            {
                mTls.GetSweepRange(ref _start, ref _stop, ref _step);
            }
            catch
            {
                _start = 0;
                _stop = 0;
                _step = 0;
            }

        }





        /// <summary>
        /// set on the LD current.
        /// </summary>
        public void TlsOn()
        {
            try
            {
                mTls.LaserOn();
            }
            catch
            {
                //do nothing.
            }
        }



        /// <summary>
        /// set off the LD current.
        /// </summary>
        public void TlsOff()
        {
            try
            {
                mTls.LaserOff();
            }
            catch
            {
                //do nothing.
            }
        }



        /// <summary>
        /// start sweeping as continuous mode.
        /// </summary>
        public void ExecTlsSweepCont()
        {
            try
            {


                //tls 파장을 sweep range 처음으로 변경. 
                //powermeter와의 sink문제가 발생한다.
                //그래서 sleep도 주고 이처름 파장도 range처음으로 바꿔준다.
                //santec tls 진짜 거지 같다.
                SetTlsWavelen(m_swpWaveStart);


                //sweep
                mTls.SweepCont();


                //Sweep이 완료되면 wavelength는 sweep range의 마지막으로 간다.
                m_wavelen = m_swpwaveStop;
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
                ret = mTls.IsSweeping();
            }
            catch
            {
                ret = false;
            }

            return ret;

        }


        #endregion

    }

}