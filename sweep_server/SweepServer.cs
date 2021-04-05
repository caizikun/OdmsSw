using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Jeffsoft;
using Free302.TnM.DataAnalysis;

namespace Neon.Aligner
{
    public partial class SweepLogic
    {


        #region ==== prepare to Sweep ====


        int mStartWave, mStopWave, mNumDataPoint;
        double mStepWave;


        public void SetSweepMode(int[] _portNos, int _startWave, int _stopWave, double _stepWave)
        {
            initSweepParam(_startWave, _stopWave, _stepWave);

            if (mUsingLocalTls) mTls.SetTlsSweepRange(_startWave, _stopWave, _stepWave);

            mPm.SetPdSweepMode(_portNos, _startWave, _stopWave, _stepWave);
        }


        void initSweepParam(int start_nm, int stop_nm, double step_nm)
        {
            mStartWave = start_nm;
            mStopWave = stop_nm;
            mStepWave = step_nm;
            mNumDataPoint = 1 + (int)Math.Floor((stop_nm - start_nm) / step_nm);

            mWaves = Enumerable.Range(0, mNumDataPoint).Select(x => Math.Round(mStartWave + mStepWave * x, 3)).ToList();
        }


        #endregion




        #region ==== Sweep ====


        private async Task execSweepNonPol(int[] ports, int[] gains, int wlStart, int wlStop, double wlStep)
        {
            try
            {
                List<double> logWave = null;

                //Sweep하고 데이터를 획득한다.
                var rawData = new List<List<PortPowers>>();//gain, port
                for (int i = 0; i < gains.Length; i++)
                {
                    mReporter?.Invoke($"\tExecSweepNonpol(): Gain={i + 1}/{gains.Length} ({gains[i]}dBm)");

                    mPm.SetGainLevel(ports, gains[i]);

                    try
                    {
                        //register - 리턴 후 스윕 시작
                        if (!mUsingLocalTls) TcpServer_Register(true);

                        //Powermeter의 설정
                        SetSweepMode(ports, wlStart, wlStop, wlStep);
                        mPm.SetPdLogMode(ports);

                        //start sweep
                        if (mUsingLocalTls)
                        {
                            mTls.TlsLogOn();
                            mTls.ExecTlsSweepCont();
                            while (mTls.IsTlsSweepOperating()) Thread.Sleep(100);
                            logWave = mTls.GetTlsWavelenLog();
                        }
                        else
                        {
                            //TLS sweep ready
                            mTcp.Ready();

                            //Get Wave data
                            logWave = (mTcp.GetData()).ToList();
                        }
                    }
                    finally
                    {
                        if (!mUsingLocalTls) TcpServer_Register(false);//서버 탈퇴
                    }

                    //Optical Power data를 얻는다.
                    mReporter?.Invoke("\tExecSweepNonpol(): GetPowLogData()");
                    var power = readPmLogData(ports);
                    rawData.Add(power);

                    if (mUsingLocalTls) mTls.TlsLogOff();
                    mPm.StopPdLogMode(ports);
                    mPm.StopPdSweepMode(ports);
                }

                
                mPm.SetGainLevel(ports, gains[0]);

                //logged power data를 등간격으로 만들어 준다.
                mReporter?.Invoke($"\tExecSweepNonpol(): starting Normalization");
                for (int i = 0; i < gains.Length; i++)
                {
                    for (int j = 0; j < ports.Length; j++)
                    {
                        rawData[i][j].NonPol = applyLinearInterpolation(mStartWave, mStepWave, logWave, rawData[i][j].NonPol);
                    }
                }

                //data stiching...
                //delete data.
                if (gains.Length > 1)
                {
                    mReporter?.Invoke($"\tExecSweepNonpol(): starting stiching");
                    mPortPowerList = new List<PortPowers>();//port

                    var merging = new DataMerging(gains[1], 2, DataMerging.DataUnit.MilliWatt);
                    for (int p = 0; p < ports.Length; p++)
                    {
                        var power = merging.Merge(rawData[0][p].NonPol.ToArray(), rawData[1][p].NonPol.ToArray());
                        mPortPowerList.Add(new PortPowers(rawData[0][p].Port, power.ToList()));
                    }
                }
                else mPortPowerList = rawData[0];
            }
            catch (Exception ex)
            {
                mPortPowerList?.Clear();
                var msg = $"ExecSweepNonpol():\n{ex.Message}\n\n{ex.StackTrace}";
                mReporter?.Invoke(msg);
            }
        }



        private List<double> applyLinearInterpolation(double waveStart, double waveStep, List<double> waveLog, List<double> powerLog)
        {
            List<double> retList = null;

            try
            {

                double[] ptXarr = new double[2];
                double[] ptYarr = new double[2];
                double wavelen = waveStart;
                int dataPoint = waveLog.Count();
                int index = 0;
                double interpolRes = 0;
                retList = new List<double>();
                for (int i = 0; i < dataPoint; i++)
                {

                    //find index.
                    index = waveLog.BinarySearch(wavelen);
                    if (index < 0)
                    {
                        index = index ^ (-1);

                        if (index >= dataPoint)
                            index = dataPoint - 1;
                    }


                    //Interpolation
                    if (index <= 1)
                    {
                        ptXarr[0] = waveLog[0];
                        ptXarr[1] = waveLog[1];
                        ptYarr[0] = powerLog[0];
                        ptYarr[1] = powerLog[1];
                    }
                    else if (index == (dataPoint - 1))
                    {
                        ptXarr[0] = waveLog[dataPoint - 2];
                        ptXarr[1] = waveLog[dataPoint - 1];
                        ptYarr[0] = powerLog[dataPoint - 2];
                        ptYarr[1] = powerLog[dataPoint - 1];
                    }
                    else
                    {
                        ptXarr[0] = waveLog[index - 1];
                        ptXarr[1] = waveLog[index];
                        ptYarr[0] = powerLog[index - 1];
                        ptYarr[1] = powerLog[index];
                    }



                    if (ptYarr[0] > 100)
                        interpolRes = ptYarr[0];
                    else
                        interpolRes = JeffMath.LinearInterpolation(ptXarr[0], ptYarr[0],
                                                                   ptXarr[1], ptYarr[1],
                                                                   wavelen);

                    interpolRes = Math.Round(interpolRes, 9);

                    if (interpolRes <= 0.0)
                        interpolRes = 0.000000001;


                    //list에 추가
                    retList.Add(interpolRes);

                    //next wavelength.
                    wavelen += waveStep;
                    wavelen = Math.Round(wavelen, 3);
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



        private List<PortPowers> readPmLogData(int[] _portNos)
        {
            List<PortPowers> swpPortPwrList = null;

            swpPortPwrList = new List<PortPowers>();
            for (int k = 0; k < _portNos.Length; k++)
            {
                PortPowers portPwr = new PortPowers(_portNos[k], mPm.GetPwrLog(_portNos[k]));
                swpPortPwrList.Add(portPwr);
            }

            return swpPortPwrList;
        }


        #endregion




        #region ==== measurement ====

        public enum FaType { SMF = 0, MMF = 1 }
        public enum ChipDirection { FORWARD = 0, REVERSE = 1 } //ex)forward : 1271->1291->1311->1331  Reverse : 1331->1311->1291->1271

        /// <summary>
        /// DUT ch 순서(==파장순서)로 pm 포트를 정렬해서 리턴
        /// </summary>
        /// <param name="_fa"></param>
        /// <param name="_ChDir"></param>
        /// <returns></returns>
        public int[] PmChMapping(int _fa, int _ChDir)
        {
            var fa = (_fa == 0) ? FaType.SMF : FaType.MMF;
            var chDir = (_ChDir == 0) ? ChipDirection.FORWARD : ChipDirection.REVERSE;

            int[] pmCh = (fa == FaType.MMF) ? new int[] { 1, 2, 3, 4 } : new int[] { 5, 6, 7, 8 };

            if (chDir == ChipDirection.REVERSE) Array.Reverse(pmCh);

            return pmCh;
        }

		public int[] MonitorMapping(int _fa, int _ChDir, in int[] wave)
		{
			var fa = (_fa == 0) ? FaType.SMF : FaType.MMF;
			var chDir = (_ChDir == 0) ? ChipDirection.FORWARD : ChipDirection.REVERSE;

			int[] pmCh = (fa == FaType.MMF) ? new int[] { 1, 2, 3, 4 } : new int[] { 5, 6, 7, 8 };

			if (chDir == ChipDirection.REVERSE) Array.Reverse(wave);

			return pmCh;
		}


        public async Task<DutData> MeasureSpecturmNp_Dut(int[] ports, int[] gains, int waveStart, int waveStop, double waveStep)
        {
            var power = await MeasureSpecturmNp(ports, gains, waveStart, waveStop, waveStep); ;
            var transData = new DutData(waveStart, waveStop, waveStep, mNumDataPoint);
            transData.mTrans = power;
            return transData;
        }

        public async Task<List<PortPowers>> MeasureSpecturmNp(int[] ports, int[] gains, int wlStart, int wlStop, double wlStep)
        {
            //sweep 
            mReporter?.Invoke("MeasureSpecturmNp()");
            await execSweepNonPol(ports, gains, wlStart, wlStop, wlStep);

            return mPortPowerList;
        }


        /// <summary>
        /// 지정 포트의 파워를 <paramref name="numAvg"/>만큼 읽어서 그 평균을 리턴
        /// </summary>
        /// <param name="port"></param>
        /// <param name="numAvg"></param>
        /// <param name="convertToDbm"></param>
        /// <returns></returns>
        public double MeasurePower(int port, int numAvg, bool convertToDbm)
        {
            const double mWmin = 1.000230285020825E-10;//-99.999dBm
            const double mWmax = 10.0;

            if (numAvg < 1) numAvg = 1;
            var power_mw = 0.0;
            for (int i = 0; i < numAvg; i++)
            {
                var mw = mPm.ReadPower(port);
                if (mw > mWmax) mw = mWmax;
                else if (mw < mWmin) mw = mWmin;
                else if (double.IsNaN(mw)) mw = mWmin;
                power_mw += mw;
            }
            power_mw /= numAvg;
            return convertToDbm ? Unit.MillWatt2Dbm(power_mw) : power_mw;
        }

        /// <summary>
        /// 각 포트 대응 파장에서 파워 측정
        /// </summary>
        /// <param name="dutCwl_nm"></param>
        /// <param name="pmPorts"></param>
        /// <param name="numAvg"></param>
        /// <param name="convertToDbm"></param>
        /// <returns></returns>
        public async Task<double[][]> MeasurePower(int[] dutCwl_nm, int[] pmPorts, int numAvg, bool convertToDbm, 
												   Action<int> moveToPort, Action<bool> moveAs, bool measureClading = false)
        {
            try
            {
                //register
                if (!mUsingLocalTls)
                {
                    TcpServer_Register(true);
                    mTcp.BeginAlign();
                }
				double[][] power = new double[2][];
				for (int i = 0; i < power.Length; i++) power[i] = new double[pmPorts.Length];

                for (int p = 0; p < pmPorts.Length; p ++)
                {
                    //set TLS wave 
                    if (mUsingLocalTls) mTls.SetTlsWavelen(dutCwl_nm[p]);
                    else mTcp.Align(dutCwl_nm[p]);

                    //move to port
                    moveToPort?.Invoke(p);

                    //read power
                    power[0][p] = MeasurePower(pmPorts[p], numAvg, convertToDbm);

					//measure clading
					if (measureClading)
					{
						moveAs?.Invoke(true);
						power[1][p] = MeasurePower(pmPorts[p], numAvg, convertToDbm);
						moveAs?.Invoke(false); 
					}
				}
                return power;
            }
            finally
            {
                if (!mUsingLocalTls)
                {
                    //await mTcp.EndAlign();
                    TcpServer_Register(false);//서버 탈퇴
                }
            }
        }

        public async Task<double> MeasurePower(int wave_nm, int port, int numAvg, bool convertToDbm)
        {
			//ref form에서 호출
            return (await MeasurePower(new int[] { wave_nm }, new int[] { port }, numAvg, convertToDbm, null, null))[0][0];
        }



        public void CalcTrans(DutData dut, ReferenceDataNp refData, bool loop = false)
        {

            mReporter?.Invoke("Measurement(): calcualting transmittance");
			try
			{
				for (int p = 0; p < dut.mTrans.Count(); p++)
				{
					var port = dut.mTrans[p].Port;
					var dutPowerList = dut.GetNonPolLossOf(port);
					for (int j = 0; j < mNumDataPoint; ++j)
					{
						if (loop) port = 1;

						double refPower = refData.RefPow(port, mWaves[j]);
						double dutPower = dutPowerList[j];

						var t = dutPower / refPower;
						if (t > 10.0) t = 10.0;//+10dB
						else if (t < 1e-5) t = 1e-5;//-50dB
						else if (double.IsNaN(t)) t = 1e-5;
						var il = Unit.Linear2Db(t);

						dut.mTrans[p].NonPol[j] = il;
					}
				}
			}
			catch (Exception)
			{
			}

        }



        public DutData SumDutData(List<DutData> chData, int waveStart, int waveStop, double waveStep, int _ChDir)
        {
            var chDir = (_ChDir == 0) ? ChipDirection.FORWARD : ChipDirection.REVERSE;
            var allPort = new List<PortPowers>();

            switch (chDir)
            {
                case ChipDirection.FORWARD:
                    for (int i = 0; i < chData[0].NumCh; i++)
                    {
                        allPort.Add(chData[i].mTrans[0]);                           //Data Sum
                        allPort[i].Port = i + 1;                                    //Port 번호 지정
                    }
                    break;

                case ChipDirection.REVERSE:
                    for (int i = 0; i < chData[0].NumCh; i++)
                    {
                        allPort.Add(chData[i].mTrans[chData[0].NumCh - 1]);         //Data Sum
                        allPort[i].Port = chData[0].NumCh - i;                      //Port 번호 지정
                    }
                    break;
            }

            var transData = new DutData(waveStart, waveStop, waveStep, mNumDataPoint);
            transData.mTrans = allPort;

            return transData;
        }


        #endregion


    } 
}