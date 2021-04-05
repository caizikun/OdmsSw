using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    public class DS2000 : IDispSensor
    {
        public int PhysicalSensorCount { get; private set; }

        private Daq mDaq = null;
        private Dictionary<SensorID, int> mChs;


        /// <summary>
        /// 초기화
        /// </summary>
        /// <param name="_daq">daq instance</param>
        /// <param name="aiCh1">sensor1 analog input no.</param>
        /// <param name="aiCh2">sensor2 analog input no.</param>
        /// <returns></returns>
        public bool Init(Daq _daq, int[] aiCh, double[] voltRange, bool rse)
        {
            if (_daq == null) return false;
            mDaq = _daq;

            bool result = true;

            //ai ch
            int aiCh1 = aiCh[0];
            int aiCh2 = aiCh.Length >= 2 ? aiCh[1] : aiCh[0];
            Action<int> create = (ch) 
                => result &= rse ? _daq.CreateAiChRse(ch, voltRange[0], voltRange[1]) : _daq.CreateAiCh(ch, voltRange[0], voltRange[1]);

            //ch 1
            PhysicalSensorCount = 1;
            create(aiCh1);

            //ch 2
            if (aiCh1 != aiCh2)
            {
                PhysicalSensorCount = 2;
                create(aiCh2);
            }            

            mChs = new Dictionary<SensorID, int>();
            mChs.Add(SensorID.Left, aiCh1);
            mChs.Add(SensorID.Right, aiCh2);

            return result;
        }


		/// <summary>
		/// distance 값을 읽어들인다.
		/// </summary>
		/// <param name="sensorID">sensor ID
		public double ReadDist(SensorID sensorID)
        {

			if (mChs.ContainsKey(sensorID)) return mDaq.ReadAi(mChs[sensorID]);
			else return mDaq.ReadAi(mChs[SensorID.Left]);

		}        
    }

}
