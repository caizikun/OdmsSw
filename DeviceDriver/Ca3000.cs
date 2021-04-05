using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.NI4882;

namespace Neon.Aligner
{
    public class Ca3000 : IoptMultimeter
    {
        public Ca3000()
        {
            mLock = new object();
            CanLogByPort = false;
            mPortMap = new Dictionary<int, string>();
            for (int p = 1; p <= 4; p++) mPortMap.Add(p, $"{((p - 1) / 2) + 1}{2 - (p % 2)}");
        }

        Device mDevice;
        object mLock;
        Dictionary<int, string> mPortMap;

        public bool Open(int address)
        {
            mDevice = new Device(0, (byte)(0xFF & address), 0, TimeoutValue.T10s);

            return query("*IDN?").Contains("FCA");
        }

        public void Init(int gain, double[] wave)
        {
            StopMeasure();

            write("FUNC POW");
            write("AUTO 0");
            //write($"LEV 0,{gain}");
            SetGainLevel(gain);
            write("AOUT 22");
            write("WMOD SING"); //write("WMOD CONT");
            write("WAV 1310");//single wave mode
            write("TINT 0.002");
            if (wave == null) wave = new double[] { 1260, 1360, 0.05, 40 };
            write($"WSET {wave[0]},{wave[1]},{wave[2]}");
            write($"SPE {wave[3]}");
            write("LOGM 0");

            write("DISA 0");
            write("ENAB 11;*OPC;ENAB 12;*OPC;ENAB 21;*OPC;ENAB 22");//write("ENAB 11;*WAI;ENAB 12;*WAI;ENAB 21;*WAI;ENAB 22");

            var en = query("ENAB?");
        }

        public static void Test()
        {
            var ca = new Ca3000();
            ca.Open(16);

            ca.Init(0, null);

            var power = ca.ReadPower(1);

            ca.SetGainLevel(0);
            var gain = ca.query("LEV? 0");
            var ports = new int[] { 1, 2, 3, 4 };
            ca.SetGainLevel(ports, -10);
            var g3 = ca.GetGainLevel(3);
            var gains = ca.GetGainLevel(ports);

            var wave = ca.GetPdWavelen(1);
            ca.SetPdWavelen(1511);
            wave = ca.GetPdWavelen(1);

            ca.SetPdLogMode(1);
            ca.SetPdSweepMode(1, 1520, 1570, 0.01);

            while (true)
            {
                var response = ca.query("STAT?");
                var state = response.Contains("1");
                if (state) break;
                Task.Delay(100).Wait();
            }

            ca.StopPdSweepMode();
            ca.StopPdLogMode();

            var powers = ca.GetPwrLog(1);

        }
        public List<List<double>> TestSweep(Itls mTls)
        {

            SetPdLogMode(1);
            SetPdSweepMode(1, 1520, 1570, 0.05);
            
            //StartMeasure();

            mTls.TlsLogOn();
            mTls.ExecTlsSweepCont();
            while (mTls.IsTlsSweepOperating()) Task.Delay(100).Wait();
            var logWave = mTls.GetTlsWavelenLog();

            while (true)
            {
                var response = query("STAT?");
                var state = response.Contains("1");
                if (state) break;
                Task.Delay(100).Wait();
            }

            var power = GetPwrLog();

            StopMeasure();
            StopPdLogMode();

            return power;
        }


        #region ---- Device Base ----

       


        #endregion


        #region ---- GPIB IO ----

        void write(string cmd)
        {
            mDevice.Write(cmd);
            Task.Delay(100).Wait();
        }
        string readString()
        {
            return mDevice.ReadString().Trim();
        }
        string query(string cmd)
        {
            lock(mLock)
            {
                write(cmd);
                return readString();
            }
        }

        byte[] read()
        {
            //Read number of digit
            byte[] byteResponse = null;
            byteResponse = mDevice.ReadByteArray(2);

            int nDigitNum = 0;
            nDigitNum = Convert.ToInt32(System.Text.Encoding.Default.GetString(byteResponse, 1, 1));

            //Read size of binary data block
            byteResponse = mDevice.ReadByteArray(nDigitNum);

            //string strTempBuf = "";
            int nBinaryDataBlockSize = int.Parse(Encoding.ASCII.GetString(byteResponse));
            //for (int i = 0; i <= nDigitNum - 1; i++)
            //{
            //    strTempBuf += System.Text.Encoding.Default.GetString(byteResponse, i, 1);
            //}
            //nBinaryDataBlockSize = Convert.ToInt32(strTempBuf);



            // ----- Logging data -------

            //Data를 읽는다.
            byteResponse = mDevice.ReadByteArray(nBinaryDataBlockSize);


            return byteResponse;
        }
        #endregion


        #region ---- Interface : PM ----

        public bool CanLogByPort { get; private set; }
        public int NumPorts { get { return 4; } }
        public object[] ChList { get { return new object[] { 1, 2, 3, 4 }; } }
                

        public void SetGainManual(int port)
        {
            write($"AUTO {mPortMap[port]}");
        }

        public void SetGainManual()
        {
            write("AUTO 0");
        }

        public void SetGainLevel(int port, int gain)
        {
            write($"LEV {mPortMap[port]},{gain}");
        }

        public void SetGainLevel(int[] ports, int gain)
        {
            foreach (var p in ports) write($"LEV {mPortMap[p]},{gain}");
        }

        public void SetGainLevel(int gain)
        {
            foreach (var p in mPortMap.Values) write($"LEV {p},{gain}");
        }

        public int GetGainLevel(int port)
        {
            return int.Parse(query($"LEV? {mPortMap[port]}"));
        }

        public List<int> GetGainLevel(int[] ports)
        {
            var res = query("LEV? 0").Split(',').Select(x => int.Parse(x)).ToArray();
            var list = new List<int>();
            for (int i = 0; i< ports.Length; i++) list.Add(res[ports[i] - 1]);
            return list;
        }

        public void SetPdWavelen(int portNo, double _wavelen)
        {
            SetPdWavelen(_wavelen);
        }

        public void SetPdWavelen(double _wavelen)
        {
            write($"WAV {_wavelen}");
        }

        public double GetPdWavelen(int portNo)
        {
            return double.Parse(query("WAV?"));
        }

        public void SetPdLogMode(int port)
        {
            //write("LOGM 0");
            write("WMOD CONT");
            var ok = query("SETT?").Contains("1");
            if (!ok) throw new Exception("CA3000 setting is invalid - check paramters");            
        }

        public void SetPdLogMode(int[] ports)
        {
            SetPdLogMode(ports[0]);
        }

        public void StopPdLogMode(int portNo)
        {
            StopPdLogMode();
        }

        public void StopPdLogMode(int[] ports)
        {
            StopPdLogMode();
        }

        public void StopPdLogMode()
        {
            write("WMOD SING");
        }


        int mDp;

        public void SetPdSweepMode(int port, int _startWave, int _stopWave, double _step)
        {
            mDp = (int)Math.Ceiling((_stopWave - _startWave) / _step) + 1;

            var step = Math.Round(100.0 / (mDp - 1), 3);
            write($"WSET 1520,1620,{step}");
            //write($"SPE {wave[3]}");

            StartMeasure();
        }
        public void StartMeasure()
        {
            write("MEAS");
            mLogRead = false;
        }
        public void StopMeasure()
        {
            write("STOP");
        }

        public void SetPdSweepMode(int[] portNos, int _startWave, int _stopWave, double _step)
        {
            SetPdSweepMode(1, _startWave, _stopWave, _step);
        }

        public void StopPdSweepMode(int[] portNos)
        {
            StopPdSweepMode();
        }

        public void StopPdSweepMode()
        {
            write("STOP");
        }


        bool mLogRead = false;
        List<List<double>> mData;

        public List<double> GetPwrLog(int port)
        {
            if (!mLogRead) GetPwrLog();
            return mData[port - 1];
        }

        public List<List<double>> GetPwrLog()
        {
            byte[] bytes;
            lock (mLock)
            {
                write("LOGG?");
                bytes = read();
            }

            var totalLength = bytes.Length / sizeof(float);
            var dataPoint = totalLength / 5;
            var buffer = new float[totalLength];
            Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);

            mData = new List<List<double>>();
            
            for (int i = 1; i < 5; i++)
            {
                var temp = new List<double>();
                for (int j = 0; j < dataPoint; j++)
                {
                    temp.Add(Math.Pow(10, buffer[j * 5 + i] / 10));
                }
                mData.Add(temp);

            }
            mLogRead = true;
            return mData;


        }


        public double ReadPower(int port)
        {
            var dbm = double.Parse(query($"PPOW? {mPortMap[port]}"));
            return Math.Pow(10, dbm / 10);
        }



        #endregion

    }//class
}
