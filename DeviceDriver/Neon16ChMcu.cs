using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TnM.DeviceFx;

namespace Neon.Aligner
{
    public class Neon16ChMcu : SerialBase
    {        
        public void Config(int portNumber)
        {
            var config = new SerialConfig($"COM{portNumber}", 115200)
            {
                EndOfMessage = '\n'
            };
            Config(config);
            EnableEndOfMessage = true;
        }
        bool _opened = false;
        public override void Open()
        {
            base.Open();
            _opened = true;
        }


        //public decimal Read(int port) => Math.Round(getData()[port - 1], 0);
		public decimal Read(int port)
		{
			var data = getData()[port - 1];
			var digits = (ValueType == _ValueType.dB) ? 3 : 0;
			return Math.Round(data, digits);
		}


		public enum _ValueType { CH, ADC, nA, dB }
        public _ValueType ValueType { get; set; } = _ValueType.ADC;

        public object[] ChList => Enumerable.Range(1, 16).Select(i => (object)i).ToArray();

        static char[] _split = { ',', ':' };
        decimal[] getData()
		{
            try
            {
                _isCritical = true;
                var chs = query($":FETCH? 0").Split(_split[0]);
                return chs.Select(s => decimal.Parse(s.Split(_split[1])[(int)ValueType])).ToArray();
            }
            finally
            {
                _isCritical = false;
            }
		}

        volatile bool _isRunning = false;
        volatile bool _isCritical = false;
        volatile int _port = 1;
        Action<decimal> _monitor;
        CancellationTokenSource _cts = new CancellationTokenSource();
        public void SetMonitor(Action<decimal> monitor, int port)
        {
            _port = port;
            _monitor = monitor;
            if(!_isRunning) runMonitor();
        }
        void runMonitor()
        {
            Task.Run(() => 
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    if (_opened) lock (_monitor) if (!_isCritical) _monitor(Read(_port));
                    Thread.Sleep(250);
                }
            });
        }
        public new void Close()
        {
            if (_isRunning)
            {
                _cts.Cancel();
                Thread.Sleep(300);
                _monitor = null;
                _isRunning = false;
            }
            base.Close();
            _opened = false;
        }

    }//class
}
