using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace AlignTester
{

    public class MeasureData<TCh, TPol>
    {
        TCh[] mChs;
        public int NumCh { get { return mChs.Length; } }
        public TCh[] Chs { get { return mChs; } }

        TPol[] mPols;
        public int NumPol { get { return mPols.Length; } }
        public TPol[] Pols { get { return mPols; } }


        double[] mWave;
        //double[][][] mPower;//ch, pol, wl
        Dictionary<TCh, Dictionary<TPol,double[]>> mPowerDic;

        public double StartWave { get; private set; }
        public double StopWave { get; private set; }
        public double StepWave { get; private set; }
        public int NumDataPoint { get; private set; }

        public MeasureData(int numCh, int numPol, double[] waveRange)
        {
            mChs = new TCh[numCh];
            var chs = (TCh[])Enum.GetValues(typeof(TCh));
            Array.Copy(chs, mChs, numCh);

            mPols = new TPol[numPol];
            var pols = (TPol[])Enum.GetValues(typeof(TPol));
            Array.Copy(pols, mPols, numPol);

            StartWave = waveRange[0];
            StopWave = waveRange[1];
            StepWave = waveRange[2];

            NumDataPoint = 1 + (int)Math.Floor((StopWave - StartWave) / StepWave);
            mWave = Enumerable.Range(0, NumDataPoint).Select((x, i) => StartWave + i * StepWave).ToArray();

            //mPower = new double[numCh][][];
            mPowerDic = new Dictionary<TCh, Dictionary<TPol, double[]>>();
            foreach (var ch in mChs)
            {
                mPowerDic[ch] = new Dictionary<TPol, double[]>();
                foreach (var p in mPols) mPowerDic[ch][p] = new double[NumDataPoint];
            }            
        }

        public override string ToString()
        {
            return $"{NumCh}CH:{NumPol}POL:{{{StartWave}~{StopWave}/{StepWave}}}";
        }


        public double[] Waves { get { return mWave; } }

        public Dictionary<TPol, double[]> Get(TCh ch)
        {
            return mPowerDic[ch];
        }
        public double[] Get(TCh ch, TPol pol)
        {
            return mPowerDic[ch][pol];
        }

        public void Add(TCh ch, Dictionary<TPol, double[]> data)
        {
            if (mPowerDic.ContainsKey(ch)) mPowerDic.Remove(ch);
            mPowerDic.Add(ch, data);
        }





        public string ToText(char mDelimeter)
        {
            var sb = new StringBuilder();

            for (int w = 0; w < NumDataPoint; w++)
            {
                sb.Append($"{mWave[w]:F03}{mDelimeter}");
                foreach(var c in mChs)
                {
                    foreach(var p in mPols) sb.Append($"{mPowerDic[c][p][w]:F07}{mDelimeter}");
                }
                sb.Remove(sb.Length - 1, 1);//remove last delimeter
                sb.Append('\n');
            }
            return sb.ToString();
        }
        

        public static MeasureData<TCh, TPol> Parse(string readString, int numPol)
        {
            var lines = readString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //meta data
            var dataStartIndex = skipMeta(lines);

            //data
            var del = ',';
            if (lines[dataStartIndex].Contains('\t')) del = '\t';
            else if (lines[dataStartIndex].Contains(',')) del = ',';
            else throw new Exception($"데이터 형식 오류 : 열 구분자가 <,> 또는 <TAB>이 아님");

            var items = lines[dataStartIndex].Split(new char[] { del }, StringSplitOptions.RemoveEmptyEntries);
            int numCh = (items.Length - 1) / numPol;

            //wave
            var startWave = double.Parse(items[0]);
            var stepWave = Math.Round(double.Parse(lines[dataStartIndex + 1].Split(del)[0]) - startWave, 3);

            var numTotalLines = lines.Length;
            if (lines[numTotalLines - 1].Split(del).Length == 0) numTotalLines--;
            if (lines[numTotalLines - 1][0] == '[') numTotalLines--;

            var stopWave = Math.Round(startWave + stepWave * (numTotalLines - dataStartIndex - 1), 3);

            //data parsing
            var data = new MeasureData<TCh, TPol>(numCh, numPol, new double[] { startWave, stopWave, stepWave });
            data.parseData(lines, dataStartIndex, del);
            return data;
        }
        void parseData(string[] lines, int startLineIndex, char del)
        {
            for (int w = 0; w < NumDataPoint; w++)
            {
                var colIndex = 1;
                var cols = lines[startLineIndex + w].Split(del);

                for (int c = 0; c < NumCh; c++)
                {
                    for (int p = 0; p < NumPol; p++)
                    {
                        mPowerDic[mChs[c]][mPols[p]][w] = double.Parse(cols[colIndex++]);
                    }
                }
            }
        }

        static int skipMeta(string[] lines)
        {
            for (int i = 0; i < lines.Length; i++) if (lines[i][0] != '[') return i;
            return 0;
        }



		#region ==== For Test ====

		static MeasureData<PmCh, PmPol> mDefaultRef;
		static MeasureData<DutCh, DutDatum> mDefaultDut;
		public static MeasureData<PmCh, PmPol> DefaultRef { get { return mDefaultRef; } }
		public static MeasureData<DutCh, DutDatum> DefaultDut { get { return mDefaultDut; } }
		static MeasureData()
		{
			mDefaultRef = new MeasureData<PmCh, PmPol>(4, 1, new double[] { 1260, 1360, 0.05 });
			mDefaultDut = new MeasureData<DutCh, DutDatum>(4, 1, new double[] { 1260, 1360, 0.05 });
			var random = new Random(DateTime.Now.Millisecond);

			for (int w = 0; w < mDefaultRef.NumDataPoint; w++)
			{
				for (int c = 0; c < mDefaultRef.NumCh; c++)
				{
					for (int p = 0; p < mDefaultRef.NumPol; p++)
					{
						mDefaultRef.mPowerDic[mDefaultRef.mChs[c]][mDefaultRef.mPols[p]][w] = -15 - random.NextDouble();
						mDefaultDut.mPowerDic[mDefaultDut.mChs[c]][mDefaultDut.mPols[p]][w] = -15 - 3 * random.NextDouble();
					}
				}
			}
		}

		#endregion


	}//class

}
