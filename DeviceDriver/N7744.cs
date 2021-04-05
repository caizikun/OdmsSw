using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GpibDev = NationalInstruments.NI4882.Device;
using TcpSession = NationalInstruments.Visa.TcpipSession;

namespace Neon.Aligner
{
    public class N7744 : IoptMultimeter
    {
        public N7744()
        {
        }

        object mLock = new object();
        TcpSession mDevice;

        public bool Connect()
        {
            const string resource = "TCPIP0::192.168.0.160::inst0::INSTR";
            try
            {
                mDevice = new TcpSession(resource, Ivi.Visa.AccessModes.None, 3000);
                return true;
            }
            catch(Exception ex)
            {
                Log.Write($"N7744.Connect() <{resource}>: {ex.Message}", true);
                return false;
            }
        }
        public bool Init(int pmGain, ConfigTlsParam tlsParam)
        {

            SetGainLevel(pmGain);

            return true;
        }

        string query(string cmd, bool readResponse = true)
        {
            lock (mLock)
            {
                mDevice.RawIO.Write(cmd);
                return readResponse ? mDevice.RawIO.ReadString().Trim() : string.Empty;
            }
        }



        public int NumPorts => 4;

        int[] mPorts = new int[] { 1, 2, 3, 4 };
        public object[] ChList => mPorts.Select(x => (object)x).ToArray();

        public int GetGainLevel(int port)
        {
            return int.Parse(query($"SENS{port}:POW:RANG?"));
        }

        public List<int> GetGainLevel(int[] portNos)
        {
            var list = new List<int>();
            foreach (var p in portNos) list.Add(GetGainLevel(p));
            return list;
        }

        public double GetPdWavelen(int portNo)
        {
            throw new NotImplementedException();
        }

        public List<double> GetPwrLog(int port)
        {
            var numDp = 1 + (mWaveParam_pm[1] - mWaveParam_pm[0]) / mWaveParam_pm[2];
            var data = new List<double>(numDp);
            var r = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < numDp; i++) data.Add(0.1 * port + 0.5 * r.NextDouble());
            return data;
        }

        public double ReadPower(int port)
        {
            double power;
            if (double.TryParse(query($"READ{port}:POW?"), out power)) return power * 1000;
            else return double.NaN;
        }

        public void SetGainLevel(int port, int level_dBm)
        {
            SetGainManual(port);
            query($"SENS{port}:POW:RANG {level_dBm}DBM", false);
        }
        public void SetGainLevel(int[] ports, int level_dBm)
        {
            foreach (var p in ports) SetGainLevel(p, level_dBm);
        }

        public void SetGainLevel(int level_dBm)
        {
            SetGainLevel(mPorts, level_dBm);
        }

        public void SetGainManual(int port)
        {
            query($"SENS{port}:POW:RANG:AUTO 0", false);
        }

        public void SetGainManual()
        {
            foreach (var p in mPorts) SetGainManual(p);
        }

        public void SetPdLogMode(int port)
        {
        }

        public void SetPdLogMode(int[] ports)
        {
        }

        int[] mWaveParam_pm = new int[] { 1520000, 1570000, 50 };
        public void SetPdSweepMode(int port, int _startWave, int _stopWave, double _step)
        {
            mWaveParam_pm[0] = 1000 * _startWave;
            mWaveParam_pm[1] = 1000 * _stopWave;
            mWaveParam_pm[2] = (int)(_step * 1000);
        }

        public void SetPdSweepMode(int[] portNos, int _startWave, int _stopWave, double _step)
        {
            SetPdSweepMode(1, _startWave, _stopWave, _step);
        }

        public void SetPdWavelen(int portNo, double _wavelen)
        {
        }

        public void SetPdWavelen(double _wavelen)
        {
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
        }

        public void StopPdSweepMode()
        {
        }

    }//class
}
