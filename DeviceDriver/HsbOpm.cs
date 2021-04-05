using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
//using DaqTask = NationalInstruments.DAQmx.Task;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
    public class HsbOpm : IoptMultimeter
    {
        public int NumPorts => _param.Count;

        public object[] ChList => _param.Keys.Select(c => (object)c).ToArray();

        DaqBase _daq;
        Dictionary<int, Param> _param = new Dictionary<int, Param>();//<ch, param>

        public bool Init(DaqBase daq, decimal[] param)
        {
            _daq = daq ?? throw new ArgumentNullException("daq");
            var numCh = (int)param[0];
            _param.Clear();
            for (int i = 0; i < numCh; i++)
            {
                var ch = i + 1;
                var i0 = 1 + i * 4;
                var p = new Param(ch, param[i0], param[i0 + 1], param[i0 + 2], param[i0 + 3]);
                _param.Add(ch, p);
                _daq.AddCh_AI(1, p.ai, DaqBase.AiTC.DIFF, p.Vmin); 
            }

            return true;
        }


        #region ==== OPM ====

        public double ReadPower(int ch)
        {
            if (!_param.ContainsKey(ch)) throw new ArgumentOutOfRangeException("port");
            var p = _param[ch];

            var v = double.NaN;
            do
            {
                try { v = _daq.Fetch(p.ai); }
                catch (DaqException ex)
                {
                    Thread.Sleep(50);
                    Free302.MyLibrary.Utility.MyDebug.WriteLine($"HsbOpm.ReadPower({ch}):\n{ex.Message}");
                }
            }
            while (double.IsNaN(v));

            return v <= p.Vmin ? p.Pmin : (v / p.R / p.eta);
        }
        public List<double> GetPwrLog(int ch)
        {
            if (!_param.ContainsKey(ch)) throw new ArgumentOutOfRangeException("ch");
            var p = _param[ch];
            var numDp = p.numDp;

            double[] volt;
            volt = new double[numDp];
            for (int i = 0; i < numDp; i++) volt[i] = _daq.Fetch(p.ai);

            var data = volt.Select(v => v <= p.Vmin ? p.Pmin : (v / p.R / p.eta)).ToList();

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
            public Param(int ch, decimal ai, decimal R, decimal eta, decimal Vmin)
            {
                this.ch = ch;
                this.ai = (int)ai;
                this.R = (double)R;
                this.eta = (double)eta;
                this.Vmin = (double)Vmin;
            }
            public int ch, ai, gain, start, stop, step;
            public double wave, R, eta, Vmin;

            public int numDp => 1 + ((stop - start) / step);
            public double Pmin => Vmin / R / eta;//mW
        }


    }//class
}
