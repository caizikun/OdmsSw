using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Neon.Aligner;


namespace Neon.Aligner
{
    public class ReferenceData
    {
        private List<PortPowers> mPortData;  //power data.

        private string mFilePath;      //ref. filepath.

        //-------- property ------------

        public int WaveStart { get; private set; }
        public int WaveStop { get; private set; }
        public double WaveStep { get; private set; }

        public int NumDataPoints { get { return (int)((WaveStop - WaveStart) / WaveStep) + 1; } }
        public double PolBaseAngle { get; set; }
		public int NumPols { get; set; }
        public int NumPorts { get { return mPortData.Count(); } }
        public List<int> PortList
        {
            get
            {
                return mPortData.Select(x => x.Port).ToList();
            }
        }

        public string TextFile { get { return mFilePath; } set { mFilePath = value; } }
        public bool Loaded { get; private set; }

        public ReferenceData()
        {
            mPortData = new List<PortPowers>();			
        }


        public bool compareWave(ReferenceData other)
        {
            return WaveStart == other.WaveStart && WaveStop == other.WaveStop && WaveStep == other.WaveStep;
        }

        //-------- method ------------


        /// <summary>
        /// clear... 
        /// </summary>
        public void ClearData()
        {
            WaveStart = 0;
            WaveStop = 0;     //stop wavelength.
            WaveStep = 0.0;  //wavelength step

            mPortData.Clear();
            //mFilePath = "";
        }

        public void SetWavelength(double startWl, double stopWl, double stepWl)
        {
            ClearData();
            WaveStart = (int)(startWl);
            WaveStop = (int)(stopWl);
            WaveStep = Math.Round(stepWl, 3);
        }


        public bool LoadText(string _filepath)
        {
            bool ret = false;

            string line;
            string[] strTempArr = null;
            StreamReader sr = null;

            try
            {
                //initailize member variables.
                WaveStart = 0;
                WaveStop = 0;
                WaveStep = 0.0;
                if (mPortData != null) mPortData.Clear();
                else mPortData = new List<PortPowers>();

                //File Open
                sr = new StreamReader(_filepath);

                //[START WAVELENGTH]
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                WaveStart = Convert.ToInt32(strTempArr[1]);

                //[STOP WAVELENGTH]
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                WaveStop = Convert.ToInt32(strTempArr[1]);

                //[STEP] 
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                WaveStep = Convert.ToDouble(strTempArr[1]);
                WaveStep = Math.Round(WaveStep, 3);

                //[REFERENCE PORTS]
                int numPorts = 0;
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                numPorts = Convert.ToInt32(strTempArr[1]);

                //[DATA COUNT]
                int dataCnt = 0;
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                dataCnt = Convert.ToInt32(strTempArr[1]);

                //[MAX POLARIZATION FILTER ANGLE]
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                PolBaseAngle = Convert.ToDouble(strTempArr[1]);
                PolBaseAngle = Math.Round(PolBaseAngle, 2);

                //[SCAN_MODE]
                line = sr.ReadLine();
				strTempArr = line.Split(']');
				var numLoadPols = strTempArr[1].Contains("POLARIZATION") ? 4 : 1;
                if (NumPols != numLoadPols)
                {
                    NumPols = numLoadPols;
                    //throw new Exception($"Error: Inconsistent number of pols");
                }

				//[PORT]
                List<int> ports = new List<int>();
                line = sr.ReadLine();
                strTempArr = line.Split(']');
                strTempArr = strTempArr[1].Split(new char[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < strTempArr.Length; j++) ports.Add(int.Parse(strTempArr[j]));

                //[DATA] 
                mPortData = new List<PortPowers>();
                for (int i = 0; i < numPorts; i++)
                {
                    PortPowers portPower = new PortPowers(ports[i], NumPols);
                    mPortData.Add(portPower);
                }

                line = sr.ReadLine();//[DATA]
                int nIndex = 0;
                //double refPow = 0.0;
                while (true)
                {
                    line = sr.ReadLine();
                    if (line.Contains("[END_OF_FILE]")) break;

                    strTempArr = line.Split('\t');
                    for (int i = 0; i < numPorts; i++)
                    {
                        var polPowers = new string[NumPols];
                        Array.Copy(strTempArr, (NumPols * i) + 1, polPowers, 0, NumPols);
                        mPortData[i].AddPowerFromString(polPowers);
                    }
                    nIndex++;
                }

                mFilePath = _filepath;
                ret = true;
                Loaded = true;
            }
            //catch (Exception ex)
            //{
            //    ex.Message.ToString();
            //    WaveStart = 0;
            //    WaveStop = 0;
            //    WaveStep = 0.0;
            //    if (mPortData != null) mPortData.Clear();
            //    mPortData = null;
            //    mFilePath = null;

            //    ret = false;
            //    Loaded = false;
            //}
            finally
            {
                if (sr != null) sr.Close();
            }

            return ret;
        }

        public bool SaveToTxt(string _filePath = "")
        {
            bool ret = false;

            StringBuilder line = new StringBuilder();
            StreamWriter sw = null;

            try
            {
                //File Open
                if (_filePath == "") _filePath = mFilePath;
                sw = new StreamWriter(_filePath, false, Encoding.ASCII);

                sw.WriteLine($"[START WAVELENGTH] {WaveStart}");
                sw.WriteLine($"[STOP WAVELENGTH] {WaveStop}");
                sw.WriteLine($"[STEP WAVELENGTH] {WaveStep}");
                sw.WriteLine($"[REFERENCE PORTS] {NumPorts}");
                sw.WriteLine($"[DATA COUNT] {NumDataPoints}");
                sw.WriteLine($"[MAX POLARIZATION FILTER ANGLE] {PolBaseAngle}");
				sw.WriteLine($"[SCAN_MODE] " + (NumPols == 4 ? "POLARIZATION" : "NONPOL"));

                //[PORT]
                line.Clear();
                line.Append("[PORT]\t");
                for (int i = 0; i < mPortData.Count; i++) line.Append($"{mPortData[i].Port}\t");
                sw.WriteLine(line);

                sw.WriteLine($"[DATA]");
                double wl = WaveStart;
                for (int waveIndex = 0; waveIndex < NumDataPoints; waveIndex++)
                {
                    line.Clear();
                    //wavelength
                    line.Append($"{wl}\t");

                    //power
                    for (int ch = 0; ch < NumPorts; ch++)
                    {
                        for (int pol = 0; pol < NumPols; pol++)
                            line.Append($"{Math.Round(mPortData[ch][pol][waveIndex], 8)}\t");
                    }
                    sw.WriteLine(line);
                    wl = Math.Round(wl + WaveStep, 3);
                }

                //[END_OF_FILE]
                sw.WriteLine("[END_OF_FILE]");
                ret = true;
            }
            //catch
            //{
            //    ret = false;
            //}
            finally
            {
                if (sw != null) sw.Close();
            }

            return ret;
        }

        public void SetPortData(PortPowers newData)
        {
            if (newData == null) return;
            int idx = mPortData.FindIndex(p => p.Port == newData.Port);
            if (idx == -1) mPortData.Add(newData);
            else mPortData[idx] = newData;
        }

        public PortPowers GetPortData(int _port)
        {
            return mPortData.Find(p => p.Port == _port); ;
        }

        public void RemovePortData(int _port)
        {
            int index = mPortData.FindIndex(p => p.Port == _port);
            if (index != -1) mPortData.RemoveAt(index);
        }

        public double[] GetPowerAtWave(PortPowers portPower, double wave)
        {
            double[] polPower = new double[NumPols];

            int index = (int)Math.Floor((wave - WaveStart) / WaveStep);//데이터의 파장이 오름차순으로 정렬되어있음을 가정
            if (index < 0) return null;
            if (index >= NumDataPoints - 1) return null;

            double[] x = new double[2];
            x[0] = Math.Round(WaveStart + WaveStep * index, 3);
            x[1] = Math.Round(x[0] + WaveStep, 3);

            //polarization state1
            double[] y = new double[2];

            for (int pol = 0; pol < NumPols; pol++)
            {
                y[0] = portPower[pol][index];
                y[1] = portPower[pol][index + 1];
                polPower[pol] = Math.Round(y[0] + (y[1] - y[0]) * (wave - x[0]) / (x[1] - x[0]), 7);
            }

            return polPower;
        }

        public double[] GetPowerAtWave(int port, double wave)
        {
            PortPowers portPower = mPortData.Find(p => p.Port == port);
            if (portPower == null)
				throw new IndexOutOfRangeException($"ReferenceData.GetPowerAtWave(): 레퍼런스에 포트=<{port}> 데이터가 없습니다.");
            return GetPowerAtWave(portPower, wave);
        }


        public List<List<double>> GetPowerByPort(int port)
        {
            int index = mPortData.FindIndex(p => p.Port == port);
            //if (index == -1) throw new IndexOutOfRangeException($"ReferenceData.GetPowerAtWave(): 레퍼런스에 포트=<{port}> 데이터가 없습니다.");
            if (index == -1) return null;

            return mPortData[index].Data;
        }


    } 
}
