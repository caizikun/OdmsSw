using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Free302.TnMLibrary.DataAnalysis;

namespace Neon.Aligner
{
    public class DutData
    {
        public int mWaveStart;
        public int mWaveStop;
        public double mWaveStep;
        public int NumDataPoints { get; private set; }

        public int NumCh { get { return mTrans == null ? 0 : mTrans.Count; } }
        public int[] Channels { get { return mTrans.Select(x => x.Port).OrderBy(x => x).ToArray(); } }
		//public int NumPols { get; set; }

        public List<PortPowers> mTrans;
        public List<PortPowers> mPower;


        public static double LastCladPower = 1e-8;//mW

        public DutData(int wlStart_nm, int wlStop_nm, double wlStep_nm, int numDp) : this(wlStart_nm, wlStop_nm, wlStep_nm)
        {
			if (numDp != NumDataPoints)
				throw new Exception($"DutData(): numDp({numDp}) != NumDataPoints({NumDataPoints})");
            NumDataPoints = numDp;
        }

        public DutData(int wlStart_nm, int wlStop_nm, double wlStep_nm)
        {
            mTrans = new List<PortPowers>();

            mWaveStart = wlStart_nm;
            mWaveStop = wlStop_nm;
            mWaveStep = wlStep_nm;
            NumDataPoints = (int)((mWaveStop - mWaveStart) / mWaveStep) + 1;
        }

        public void AddTrans(PortPowers portPowers)
        {
            if (mTrans.Find(x => x.Port == portPowers.Port) != null) throw new Exception($"{portPowers.Port} port번호가 이미 존재합니다.");
            mTrans.Add(portPowers);
        }

        public void ReverseChOrder()
        {
            var chs = Channels;
            mTrans.Sort((x, y) => -x.Port + y.Port);
            mPower.Sort((x, y) => -x.Port + y.Port);
            for (int i = 0; i < chs.Length; i++)
            {
                mTrans[i].Port = chs[i];
                mPower[i].Port = chs[i];
            }
        }

        public PortPowers GetPolLossOf(int port)
        {
            var value = mTrans.Find(x => x.Port == port);
            if (value == null)
                throw new IndexOutOfRangeException($"DutData.GerPortLoss():\nport=<{port} 데이터가 없습니다.>");
            return value;
        }

        public List<double> GetNonPolLossOf(int port)
        {
            var value = mTrans.Find(p => p.Port == port).NonPol;
            if (value == null)
                throw new IndexOutOfRangeException($"DutData.GerPortLoss():\nport=<{port} 데이터가 없습니다.>");
            return value;
        }


        public void Subtract(DutData sub)
        {
            for (int i = 0; i < mTrans.Count; i++) mTrans[i].Subtract(sub.mTrans[i]);
        }
        public void Subtract(double sub)
        {
            for (int i = 0; i < mTrans.Count; i++)
                for (int j = 0; j < mTrans[i].NonPol.Count; j++)
                {
                    mTrans[i].NonPol[j] -= sub;
                }
        }

		

		#region ==== writeTransmitance [Public] ====


		/// <summary>
		/// text file에 저장한다.																	(full range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <returns></returns>
		public void WriteTransmitance(string filePath)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);

			writeTrans(numPols, sb);									//sweep data 기록
			writeSaveFile(filePath, sb);								//save
		}


		/// <summary>
		/// text file에 저장한다.																	(full range + monitor)
		/// </summary>
		/// <param name="filePath">file path</param>
		/// <param name="monitorIL">monitor port 측정값</param>
		public void WriteTransmitance(string filePath, double[] monitorIL)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);
			
			writeTrans(numPols, sb);									//sweep data 기록
			writeMonitorPower(monitorIL, sb);							//monitor IL값 기록
			writeSaveFile(filePath, sb);								//save
		}


		/// <summary>
		/// text file에 저장한다.																	(full range + monitor + monitorClad)
		/// </summary>
		/// <param name="filePath">file path</param>
		/// <param name="monitorIL">monitor port 측정값</param>
		/// <param name="monitorClad">monitor Clading 측정값</param>
		public void WriteTransmitance(string filePath, double[] monitorIL, double[] monitorClad)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);
			
			writeTrans(numPols, sb);									//sweep data 기록
			writeMonitorPower(monitorIL, sb);							//monitor IL값 기록
			writeMonitorPower(monitorClad, sb, "##");					//monitor clading값 기록
			writeSaveFile(filePath, sb);								//save
		}

		
		/// <summary>
		/// text file에 저장한다.																	(Custom range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <returns></returns>
		public void WriteTransmitance(string filePath, double startWL, double stopWL)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);

			writeTrans(numPols, sb, new double[] { startWL, stopWL });  //sweep data 기록
			writeSaveFile(filePath, sb);								//save
		}


		/// <summary>
		/// text file에 저장한다.																	(Custom range + monitor)
		/// </summary>
		/// <param name="filePath">file path</param>
		/// <param name="startWL">기록 시작파장</param>
		/// <param name="stopWL">기록 끝파장</param>
		/// <param name="monitorIL">monitor port 측정값</param>
		public void WriteTransmitance(string filePath, double startWL, double stopWL, double[] monitorIL)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);

			writeTrans(numPols, sb, new double[] { startWL, stopWL });  //sweep data 기록
			writeMonitorPower(monitorIL, sb);							//monitor IL값 기록
			writeSaveFile(filePath, sb);								//save
		}


		/// <summary>
		/// text file에 저장한다.																	(Custom range + monitor + monitorClad)
		/// </summary>
		/// <param name="filePath">file path</param>
		/// <param name="startWL">기록 시작파장</param>
		/// <param name="stopWL">기록 끝파장</param>
		/// <param name="monitorIL">monitor port 측정값</param>
		/// <param name="monitorClad">monitor clading 측정값</param>
		public void WriteTransmitance(string filePath, double startWL, double stopWL, double[] monitorIL, double[] monitorClad)
		{
			if (mTrans == null || mTrans.Count == 0 || mTrans[0].NonPol == null) return;
			int numPols = mTrans[0].NumPols;
			var sb = new StringBuilder((NumCh * numPols * 8 + 8) * NumDataPoints);

			writeTrans(numPols, sb, new double[] { startWL, stopWL });  //sweep data 기록
			writeMonitorPower(monitorIL, sb);							//monitor IL값 기록
			writeMonitorPower(monitorClad, sb, "##");					//monitor clading값 기록
			writeSaveFile(filePath, sb);								//save
		}

		#endregion



		#region ==== write transmitance [private] ====


		private void writeTrans(int numPols, StringBuilder sb, double[] wlStartStop = null)
		{
			//측정 데이터 기록
			var line = new StringBuilder();
			double wl = mWaveStart;
			var startWL = (wlStartStop == null) ? mWaveStart : wlStartStop[0];
			var stopWL = (wlStartStop == null) ? mWaveStop : wlStartStop[1];

			for (int i = 0; i < NumDataPoints; i++)                                                     //i : Data Points
			{

				if (Math.Round(wl, 3) >= startWL)
				{
					line.Clear();

					//wavelength
					line.Append($"{wl:F3}, ");

					//data
					double min, max, avg, pdl;
					for (int ch = 0; ch < NumCh; ch++)                                                  //ch : dut Channels
					{
						if (numPols == 1) line.Append($"{mTrans[ch].NonPol[i]:F3}, ");
						else
						{
							max = mTrans[ch].Max[i];
							min = mTrans[ch].Min[i];
							avg = Unit.WattTodBm((Unit.dBmToWatt(min) + Unit.dBmToWatt(max)) / 2);
							pdl = max - min;

							line.Append($"{max:F3}, {min:F3}, {avg:F3}, {pdl:F3}, ");
						}
					}
					line.Remove(line.Length - 2, 2);
					sb.AppendLine(line.ToString());
				}
				wl += mWaveStep;

				if (Math.Round(wl, 3) > stopWL) break;

			}
		}


		private static void writeMonitorPower(double[] monitorIL, StringBuilder sb, string mark = "#")
		{
			//monitor port 측정값 기록
			var line = new StringBuilder();
			if (monitorIL != null)
			{
				line.Clear();
				line.Append($"{mark}");
				for (int i = 0; i < monitorIL.Length; i++) line.Append($"{monitorIL[i]}, ");
				line.Remove(line.Length - 2, 2);

				sb.AppendLine(line.ToString());
			}
		}


		private static void writeSaveFile(string filePath, StringBuilder sb)
		{
			//save : file에 쓰기
			if (ZipData.UsingZip) ZipData.Save(filePath, sb);
			else using (var sw = new StreamWriter(filePath, false, Encoding.ASCII))
				{
					sw.Write(sb.ToString());
					sw.Close();
				}
		} 


		#endregion



		public void WritePower(string filePath)
        {
            StringBuilder line = new StringBuilder();
            StreamWriter sw = null;
            List<Tuple<double, double>> error = new List<Tuple<double, double>>();

            try
            {
                //File Open
                sw = new StreamWriter(filePath, false, Encoding.ASCII);

                //data
                double wl = mWaveStart;   //wavelength [nm]
                for (int i = 0; i < NumDataPoints; i++)
                {
                    line.Clear();
                    //wavelength
                    line.Append($"{wl:F3}, ");

					//data
					var numPols = mPower[0].NumPols;
                    for (int c = 0; c < NumCh; c++)
                        for (int p = 0; p < numPols; p++)
                        {
                            if (mPower[c][p][i] < 1E-11 || double.IsNaN(mPower[c][p][i]))
                            {
                                error.Add(new Tuple<double, double>(wl, mPower[c][p][i]));
                            }
                            line.Append($"{mPower[c][p][i]:F9}, ");
                        }
                    line.Remove(line.Length - 2, 2);
                    sw.WriteLine(line);
                    wl += mWaveStep;
                }
                for (int i = 0; i < error.Count; i++)
                {
                    Log.Write($"[DutData.WritePower()]Wave = {error[i].Item1}\tPower = {error[i].Item2}");
                }
                
            }
            finally
            {
                if (sw != null) sw.Close();
            }
        }



        public static DutData LoadFileNp(string filePath)
        {
            var rawString = read(filePath);
            if (rawString.Count == 0) throw new Exception($"파일이 비었습니다.\n{filePath}");

            int numDataPoints = rawString.Count;
            int numChs = (rawString[0].Length - 1);//non pol

            var waveStart = double.Parse(rawString[0][0]);
            var last = rawString.Last();
            if (last[0].Contains("#"))
            {
                last = rawString[rawString.Count - 2];
                numDataPoints--;
            }
            var waveStop = double.Parse(last[0]);
            var waveStep = Math.Round((waveStop - waveStart) / numDataPoints, 3);

            var dutData = new DutData((int)waveStart, (int)waveStop, waveStep);            

            for (int c = 0; c < numChs; c++)//for each ch
            {
                var buffer = new double[numDataPoints];
                for (int i = 0; i < numDataPoints; i++) buffer[i] = double.Parse(rawString[i][c + 1]);
                dutData.AddTrans(new PortPowers(c + 1, buffer.ToList()));
            }
            return dutData;
        }



        public static List<string[]> read(string fileName)//wl, field
        {
            var rawString = new List<string[]>();

            //데이터 읽어오기(Txt문서)
            StreamReader reader = new StreamReader(fileName);
            string line;
            string[] array;
            var split = new char[] { ',', '\t' };
            while ((line = reader.ReadLine()) != null)
            {
                array = line.Split(split, StringSplitOptions.RemoveEmptyEntries);
                rawString.Add(array);
            }
            reader.Close();

            return rawString;
        }



        public static void Test(Action<double,double> plotter)
        {
            int start = 1260, stop = 1360;
            double step = 0.05;
            var dut = new DutData(start, stop, step);

            var random = new Random();

            for (int ch = 1; ch <= 4; ch++)
            {
                var power = new PortPowers(ch, 1);
                for (double w = start; w <= stop; w += step)
                {
                    var p = random.NextDouble();
                    power.NonPol.Add(p);
                    //plotter(w, p);
                }
                dut.AddTrans(power);
            }

            var chip = "WaferSn-1-1-A01-Test2.txt";
            var filePath = Application.StartupPath + @"\" + chip;

            dut.WriteTransmitance(filePath, null);

        }

    } 
}
