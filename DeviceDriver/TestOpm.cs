using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
    public class TestOpm : IoptMultimeter
    {
        public int NumPorts => _param.Count;

        public object[] ChList => _param.Keys.Select(c => (object)c).ToArray();

        Dictionary<int, Param> _param = new Dictionary<int, Param>();//<ch, param>

        public bool Init()
        {
            var numCh = 4;
            _param.Clear();
            for (int i = 0; i < numCh; i++)
            {
                var ch = i + 1;
                var p = new Param(ch);
                _param.Add(ch, p);
            }
            return true;
        }
        Random _random = new Random();


        #region ==== OPM ====

        Tuple<bool, double> _testValue = new Tuple<bool, double>(false, 0);
        public void SetTestValue(double v = double.NaN)
        {
            _testValue = new Tuple<bool, double>(!double.IsNaN(v), v);
        }

        public double ReadPower(int ch)
        {
            if (!_param.ContainsKey(ch)) throw new ArgumentOutOfRangeException("ch");

            if(_testValue.Item1)
            {
                return _testValue.Item2 - ch / 20.0;
            }
            else
            {
                var p = _param[ch];
                var v = p.Pmin + 1e-3 * (0.5 + _random.NextDouble() / 2);
                return v;
            }
        }
        public List<double> GetPwrLog(int ch)
        {
            if (!_param.ContainsKey(ch)) throw new ArgumentOutOfRangeException("ch");
            var p = _param[ch];
            var numDp = p.numDp;

            double[] volt;
            volt = new double[numDp];
            for (int i = 0; i < numDp; i++) volt[i] = p.Pmin + 1e-3 * (0.5 + _random.NextDouble() / 2);

            var data = volt.ToList();
            return data;
        }

        #endregion



        #region ==== Not Impl ====

        public void SetGainManual(int port) { }
        public void SetGainManual() { }
        public void SetGainLevel(int port, int _level) => _param[port].gain = _level;
        public void SetGainLevel(int[] ports, int _level)
        {
            for (int i = 0; i < ports.Length; i++) _param[ports[i]].gain = _level;
        }
        public void SetGainLevel(int _level)
        {
            foreach (Param p in _param.Values) p.gain = _level;
        }
        public int GetGainLevel(int portNo) => _param[portNo].gain;
        public List<int> GetGainLevel(int[] portNos) => _param.Values.Select(p => p.gain).ToList();
        public void SetPdWavelen(int portNo, double _wavelen) => _param[portNo].wave = _wavelen;
        public void SetPdWavelen(double _wavelen)
        {
            foreach (Param p in _param.Values) p.wave = _wavelen;
        }
        public double GetPdWavelen(int portNo) => _param[portNo].wave;
        public void SetPdLogMode(int port) { }
        public void SetPdLogMode(int[] ports) { }
        public void StopPdLogMode(int portNo) { }
        public void StopPdLogMode(int[] portNos) { }
        public void StopPdLogMode() { }
        public void SetPdSweepMode(int port, int _startWave, int _stopWave, double _step)
        {
            _param[port].start = _startWave * 1000;
            _param[port].stop = _stopWave * 1000;
            _param[port].step = (int)Math.Floor(_step * 1000);
        }
        public void SetPdSweepMode(int[] portNos, int _startWave, int _stopWave, double _step)
        {
            for (int i = 0; i < portNos.Length; i++) SetPdSweepMode(portNos[i], _startWave, _stopWave, _step);
        }
        public void StopPdSweepMode(int[] portNos) { }
        public void StopPdSweepMode() { }

        #endregion


        class Param
        {
            public Param(int ch)
            {
                this.ch = ch;
            }
            public int ch, gain, start, stop, step;
            public double wave;

            public int numDp => 1 + ((stop - start) / step);
            public double Pmin =1e-9;//mW
        }


    }//class
}
