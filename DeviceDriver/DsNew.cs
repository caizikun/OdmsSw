using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
    public class DsNew : IDispSensor
    {
        public int PhysicalSensorCount { get; private set; }

        Dictionary<SensorID, int> _chs;
        DaqBase _daq;

        public bool Init(DaqBase daq, bool isRse, int[] ai, double[] range)
        {
            if (daq == null) return false;
            _daq = daq;

            bool result = true;
            var terminal = isRse ? DaqBase.AiTC.RSE : DaqBase.AiTC.DIFF;

            //ai ch
            int ai1 = ai[0];
            int ai2 = ai.Length >= 2 ? ai[1] : ai[0];

            //Vmax
            var v = new double[] { 0, 0 };
            if (range != null) range.CopyTo(v, 0);

            //ch 1
            PhysicalSensorCount = 1;
            _daq.AddCh_AI(1, ai1, terminal, Vmax: v[1]);

            //ch 2
            if (ai1 != ai2)
            {
                PhysicalSensorCount = 2;
                _daq.AddCh_AI(1, ai2, terminal, Vmax: v[1]);
            }

            _chs = new Dictionary<SensorID, int>();
            _chs.Add(SensorID.Left, ai1);
            _chs.Add(SensorID.Right, ai2);

            return result;
        }

        public double ReadDist(SensorID id) => _daq.Fetch(_chs[id]);
        //public double ReadDist(SensorID id) => _daq.Read(_chs[id]);
    }
}
