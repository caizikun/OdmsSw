using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NationalInstruments.NI4882;
using System.Runtime.InteropServices;
using System.IO;
using TnM.Utility;

namespace Neon.Aligner
{
	public class SantecPmDriver
	{



		#region member variable

		private int m_gpibAddr;
		private bool m_bConnectedOK;
		private Device m_gpibDev;

		Dictionary<int, int> _Slot = new Dictionary<int, int>();        //Key : Port, Value : Slot
		Dictionary<int, int> _Channel = new Dictionary<int, int>();     //Key : Port, Value : Channel

		#endregion


		#region property

		public int gpibAddress { get { return m_gpibAddr; } }

		#endregion



		#region consturctor/destructor

		public SantecPmDriver()
		{

		}

		~SantecPmDriver()
		{

		}

		#endregion



		#region private method

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
						if (data <= 0) data = 0.000000000001;

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
				if (retList != null) retList.Clear();
				retList = null;
			}

			return retList;

		}



		private void Write(string cmd)
		{
			m_gpibDev.Write(cmd);
			Thread.Sleep(10);
		}


		/// <summary>
		/// send command & receive response.
		/// </summary>
		/// <param name="_cmd"></param>
		/// <returns></returns>
		private string Query(string _cmd)
		{

			string strRet = "";

			//Check Connection
			if (false == m_bConnectedOK) return strRet;

			//query.
			try
			{
				Monitor.Enter(m_gpibDev);

				//send
				strRet = "";
				Write(_cmd);

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
		}




		#endregion



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
				if (m_gpibDev == null)
				{
					m_gpibDev = new Device(_gpibBoard, Convert.ToByte(_gpibAddr));
					m_gpibDev.DefaultBufferSize = mDefaultBufferSize;
				}

				m_bConnectedOK = true;
				m_gpibAddr = _gpibAddr;
				m_bConnectedOK = true;


				initPm();
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

		private void initPm()
		{
			//port check 
			int startPort = 1;
			var slot = Query("IDIS?").Split(',');
			for (int i = 0; i < slot.Length; i++)
			{
				if (slot[i].Contains("1"))
				{
					var channel = Query($"READ? {i}").Split(',');
					for (int j = 0; j < channel.Length; j++)
					{
						_Slot.Add(startPort, i);
						_Channel.Add(startPort, j + 1);
						startPort += 1;
					}
				}
			}
		}



		#region ==== Read 측정파워값 읽기 ====

		/// <summary>
		/// 광파워값 읽기
		/// </summary>
		/// <param name="port">포트번호</param>
		/// <returns>광파워값</returns>
		public double ReadPower(int port)
		{
			var slotPower = Array.ConvertAll(Query($"READ? {_Slot[port]}").Split(','), double.Parse);
			return slotPower[_Channel[port] - 1];
		}
		#endregion

		#region ==== Trigger 트리거 설정 ====

		public enum Trigger { Internal, External }
		public void SetTrigger(Trigger trigger) => Write($"TRIG {(int)trigger}");
		public Trigger GetTrigger() => (Trigger)(Query("TRIG?").Trim().To<int>());

		#endregion



		#region ==== Unit 유닛설정 ====



		public enum Unit { dBm, mW }
		public void SetUnit(Unit unit)
		{
			Write($"UNIT {(int)unit}");
		}
		public Unit GetUnit() => (Unit)Query("UNIT?").Trim().To<int>();

		#endregion



		#region ==== Average time 설정 ====

		/// <summary>
		/// average Time 설정
		/// </summary>
		/// <param name="avgTime">0.01㎲ ~ 10000㎲</param>
		public void SetAvgTime(double avgTime) => Write($"AVG {avgTime}");

		public double GetAvgTime() => Query("AVG?").Trim().To<double>();

		#endregion



		#region ==== GainMode 설정 ====

		public enum GainMode { Manual, Auto }
		public void SetGainMode(GainMode gain) => Write($"AUTO {(int)gain}");
		public GainMode GetGainMode() => (GainMode)Query("AUTO?").Trim().To<int>();

		#endregion



		#region ==== Gain Level 설정 ====

		public void SetGainLevel(int gain)
		{
			int lev = 1;

			if (gain >= 0) lev = 1;
			else if (gain >= -10) lev = 2;
			else if (gain >= -20) lev = 3;
			else if (gain >= -30) lev = 4;
			else lev = 5;

			Write($"LEV {lev}");
		}
		public int GetGainLevel()
		{
			var lev = Query("LEV?").Trim().To<int>();

			if (lev == 1) return 0;
			else if (lev == 2) return -10;
			else if (lev == 3) return -20;
			else if (lev == 4) return -30;
			else return -40;
		}

		#endregion



		#region ==== Wave 파장설정 ====

		/// <summary>
		/// 파장설정
		/// </summary>
		/// <param name="wave">1250nm ~ 1630nm</param>
		public void SetWave(double wave)
		{
			if (wave < 1250) wave = 1250;
			else if (wave > 1630) wave = 1630;

			Write($"WAV {wave}");
		}

		public double GetWave() => Query("WAV?").Trim().To<double>();

		#endregion



		#region ==== Error ====

		public int ReadErrorCord()
		{
			return Query("ERR?").Split(',')[0].To<int>();
		}

		public string ReadErrorString()
		{
			return Query("ERR?");
		}

		#endregion


		#region ==== Sweep 항목 설정 ====

		public string GetStatus()
		{
			var a = Query("STAT?").Split(',')[0].To<int>();
			if (a == 0) return $"{a} :\tMeasuring is still in process.";
			else if (a == 1) return $"{a} :\tMeasurement completed";
			else return $"{a} :\tSetting up for measuring, Force to stop the measurement ";
		}

		/// <summary>
		/// Sweep 파장 설정 [nm]
		/// </summary>
		/// <param name="startWl">Start 1250 ~ 1630</param>
		/// <param name="stopWl">Stop 1250 ~ 1630</param>
		/// <param name="stepWl">Step 0.002 ~ 10</param>
		public void SetSweepWave(double startWl, double stopWl, double stepWl)
		{
			Write($"WSET {startWl},{stopWl},{stepWl}");
		}

		double[] getSweepWave()
		{
			return Query("WSET?").Split(',').Select(Double.Parse).ToArray();
		}

		public double GetSweepStartWave()
		{
			return getSweepWave()[0];
		}

		public double GetSweepStopWave()
		{
			return getSweepWave()[1];
		}

		public double GetSweepStepWave()
		{
			return getSweepWave()[2];
		}


		/// <summary>
		/// sweep speed 설정
		/// </summary>
		/// <param name="speed">0.001nm/sec ~ 100nm/sec</param>
		public void SetSweepSpeed(double speed) => Write($"SPE {speed}");

		public double GetSweepSpeed() => Query("SPE?").Trim().To<double>();


		/// <summary>
		/// DataPoint 설정
		/// </summary>
		/// <param name="dataPoint">1 ~ 1,000,000</param>
		public void SetDataPoint(int dataPoint) => Write($"LOGN {dataPoint}");

		public int GetDataPoint() => Query("LOGN?").Trim().To<int>();

		public enum MeasurementMode { CONST1, SWEEP1, CONST2, SWEEP2, FREERUN }

		/// <summary>
		/// 측정모드 설정
		/// </summary>
		/// <param name="mode">CONST1 : 기본 상태, SWEEP1 : sweep 상태</param>
		public void SetMeasurementMode(MeasurementMode mode) => Write($"WMOD {mode}");

		public MeasurementMode GetMeasurementMode()
		{
			return (MeasurementMode)Enum.Parse(typeof(MeasurementMode), Query("WMOD?").Trim());
		}


		public void ReadySweep() => Write("MEAS");
		public void StopSweep() => Write("STOP");

		public bool IsSweepping()
		{
			var a = Query("STAT?").Split(',')[0].To<int>();
			if (a == 0) return true;
			else return false;
		}

		public List<double> GetPmLog(int port)
		{
			var logPower = new List<double>();
			var ascii = new ASCIIEncoding();

			try
			{
				Monitor.Enter(m_gpibDev);

				Write($"LOGG? {_Slot[port]},{_Channel[port]}");

				var hash = ascii.GetString(m_gpibDev.ReadByteArray(1));                         // tag '#'
				var digit = Convert.ToInt32(ascii.GetString(m_gpibDev.ReadByteArray(1)));       // Number of digits
				var numData = Convert.ToInt32(ascii.GetString(m_gpibDev.ReadByteArray(digit)));

				var byteResponse = m_gpibDev.ReadByteArray(numData);                            // Measureing Data

				Monitor.Exit(m_gpibDev);

				logPower = BinDataBlockToList(byteResponse, 4);                                 // mW

			}
			catch (Exception)
			{
				logPower = null;
			}

			return logPower;
		}

		public int GetPmLogCount(int port)
		{
			var ascii = new ASCIIEncoding();
			int numData = new int();
			try
			{
				Monitor.Enter(m_gpibDev);

				Write($"LOGG? {_Slot[port]},{_Channel[port]}");

				var hash = ascii.GetString(m_gpibDev.ReadByteArray(1));                         // tag '#'
				var digit = Convert.ToInt32(ascii.GetString(m_gpibDev.ReadByteArray(1)));       // Number of digits
				numData = Convert.ToInt32(ascii.GetString(m_gpibDev.ReadByteArray(digit)));

				var byteResponse = m_gpibDev.ReadByteArray(numData);                            // Measureing Data

				Monitor.Exit(m_gpibDev);
			}
			catch (Exception)
			{
			}
			return numData / 4;
		}

		#endregion

	}
}
