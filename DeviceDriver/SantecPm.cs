using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.Aligner
{
	public class SantecPm : IoptMultimeter
	{


		#region structure/inner class


		protected class CportPos
		{
			public int port { get; set; }
			public int gpib { get; set; }
			public int slot { get; set; }
			public int chnl { get; set; }

			public CportPos() { }
			public CportPos(int port, int gpib, int slot)
			{
				this.port = port;
				this.gpib = gpib;
				this.slot = slot;
				this.chnl = 1;
			}
		}


		#endregion



		#region property

		public int NumPorts { get { return m_portCnt; } }

		public object[] ChList { get { return m_portPosList.Select(x => (object)x.port).ToArray(); } }

		#endregion



		#region member variables

		private int m_portCnt;
		private SantecPmDriver _Santec;
		protected List<CportPos> m_portPosList;

		#endregion



		#region constructor

		public SantecPm()
		{

			m_portCnt = 0;
			_Santec = new SantecPmDriver();
			m_portPosList = new List<CportPos>();

			//set port postion.
			initPortAddress();

		}

		protected virtual void initPortAddress()
		{
			m_portPosList.Add(new CportPos() { port = 1, gpib = _Santec.gpibAddress, slot = 0, chnl = 1 });
			m_portPosList.Add(new CportPos() { port = 2, gpib = _Santec.gpibAddress, slot = 0, chnl = 2 });
			m_portPosList.Add(new CportPos() { port = 3, gpib = _Santec.gpibAddress, slot = 0, chnl = 3 });
			m_portPosList.Add(new CportPos() { port = 4, gpib = _Santec.gpibAddress, slot = 0, chnl = 4 });

			m_portCnt = m_portPosList.Count();
		}

		/// <summary>
		/// connect to the device.
		/// </summary>
		/// <param name="_gpibNo"></param>
		public bool Connect(int _gpibBoard, int _gpibNo)
		{
			bool ret = false;

			try
			{
				ret = _Santec.ConnectByGpib(_gpibBoard, _gpibNo);
			}
			catch
			{
				ret = false;
			}

			return ret;
		}

		//fixed parameter
		const double _AvgTime = 0.1;

		public bool Init(bool isTls, bool isTlsPmDual, int pmGain, ConfigTlsParam tlsParam)
		{

			_Santec.SetUnit(SantecPmDriver.Unit.mW);
			_Santec.SetMeasurementMode(SantecPmDriver.MeasurementMode.SWEEP1);
			_Santec.SetGainMode(SantecPmDriver.GainMode.Manual);
			_Santec.SetTrigger(SantecPmDriver.Trigger.External);
			_Santec.SetSweepSpeed(tlsParam.Speed);
			_Santec.SetAvgTime(_AvgTime);
			_Santec.StopSweep();
			
			while (true)
			{
				if (_Santec.ReadErrorCord() == 0) break;        //error Clear
			}

			return true;

		}


		#endregion



		public int GetGainLevel(int portNo)
		{
			//GetGainLevel
			return _Santec.GetGainLevel();
		}

		public List<int> GetGainLevel(int[] portNos)
		{
			//GetGainLevel
			var gain = _Santec.GetGainLevel();
			var port = new List<int>();
			for (int i = 0; i < portNos.Length; i++) port.Add(gain);

			return port;
		}

		public double GetPdWavelen(int portNo)
		{
			//GetWave
			return _Santec.GetWave();
		}

		public List<double> GetPwrLog(int port)
		{
			//Get Sweep Data
			return _Santec.GetPmLog(port);
		}

		public double ReadPower(int port)
		{
			//ReadPower
			return _Santec.ReadPower(port);
		}

		public void SetGainLevel(int port, int _level)
		{
			//SetGainLevel
			_Santec.SetGainLevel(_level);
		}

		public void SetGainLevel(int[] ports, int _level)
		{
			//SetGainLevel
			_Santec.SetGainLevel(_level);
		}

		public void SetGainLevel(int _level)
		{
			//SetGainLevel
			_Santec.SetGainLevel(_level);
		}

		public void SetGainManual(int port)
		{
			//SetGainMode
			_Santec.SetGainMode(SantecPmDriver.GainMode.Manual);
		}

		public void SetGainManual()
		{
			//SetGainMode
			_Santec.SetGainMode(SantecPmDriver.GainMode.Manual);
		}

		public void SetPdLogMode(int port)
		{
			//ReadySweep
			_Santec.ReadySweep();
		}

		public void SetPdLogMode(int[] ports)
		{
			//ReadySweep
			_Santec.ReadySweep();
		}

		public void SetPdSweepMode(int port, int _startWave, int _stopWave, double _step)
		{
			//SetSweepWave
			_Santec.SetSweepWave(_startWave, _stopWave, _step);
		}

		public void SetPdSweepMode(int[] portNos, int _startWave, int _stopWave, double _step)
		{
			//SetSweepWave
			_Santec.SetSweepWave(_startWave, _stopWave, _step);
		}

		public void SetPdWavelen(int portNo, double _wavelen)
		{
			//SetWave
			_Santec.SetWave(_wavelen);
		}

		public void SetPdWavelen(double _wavelen)
		{
			//SetWave
			_Santec.SetWave(_wavelen);
		}

		public void StopPdLogMode(int portNo)
		{
		}

		public void StopPdLogMode(int[] portNos)
		{
		}

		public void StopPdLogMode()
		{
		}

		public void StopPdSweepMode(int[] portNos)
		{
			_Santec.StopSweep();
		}

		public void StopPdSweepMode()
		{
			_Santec.StopSweep();
		}
	}
}
