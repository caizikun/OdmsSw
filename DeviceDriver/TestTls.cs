using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public class TestTls : Itls
    {
        public TestTls()
        {
            _p = new Param();
        }
        Param _p;

        public void ExecTlsSweepCont()
        {
            _p.ing = true;
            Task.Delay(1000).Wait();
            _p.ing = false;
        }
        public double GetTlsOutPwr() => _p.power;
        public void GetTlsSweepRange(ref int _start, ref int _stop, ref double _step)
        {
            _start = _p.start;
            _stop = _p.stop;
            _step = _p.step;
        }
        public double GetTlsSweepSpeed() => _p.speed;
        public double GetTlsWavelen() => _p.wave;
        public List<double> GetTlsWavelenLog() => _p.llog;
        public bool IsTlsSweepOperating() => _p.ing;
        public void SetTlsOutPwr(double _pwr) => _p.power = _pwr;
        public void SetTlsSweepRange(int _start, int _stop, double _step)
        {
            _p.start = _start;
            _p.stop = _stop;
            _p.step = _step;
        }
        public void SetTlsSweepSpeed(double _speed) => _p.speed = _speed;
        public void SetTlsWavelen(double _wl) => _p.wave = _wl;
        public void TlsLogOff() { }
        public void TlsLogOn() { }
        public void TlsOff() { }
        public void TlsOn() { }

        class Param
        {
            public bool ing = false;
            public double power, speed, wave, step;
            public int start, stop;
            public List<double> llog
            {
                get
                {
                    var wls = new List<double>();
                    for (double wl = start; wl <= stop; wl += step) wls.Add(wl);
                    return wls;
                }
            }
        }

    }//class
}
