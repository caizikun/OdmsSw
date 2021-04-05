using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
    public class UshioUvNew : IUvCure
    {
        DaqBase _daq;
        int _port, _line;

        public bool Init(DaqBase daq, int dev, int port, int line)
        {
            if (daq == null) return false;

            _daq = daq;
            _port = port;
            _line = line;

            try
            {
                _daq.AddCh_DO(dev, port, line);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void OpenShutter()
        {
            if (_daq == null) return;
            _daq.WriteDo(_line, true);
            Thread.Sleep(100);
            _daq.WriteDo(_line, false);
        }

        public void CloseShutter()
        {
            if (_daq == null) return;
            _daq.WriteDo(_line, false);
        }

    }//class
}
