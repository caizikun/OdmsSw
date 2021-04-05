using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Free302.TnM.WdmAnalyzer;
using System.Windows.Forms;

namespace Neon.Aligner
{
    public class WdmAnalyzer
    {
        #region ---- TEST ----

        public static double[][] Test(string filePath)
        {
            var dut = DutData.LoadFileNp(filePath);
            //return AnalyzeNp(dut);
            return AnalyzeMcDemux(dut);
        }


        #endregion



        #region ---- MC DeMUX ----

        public static double[][] AnalyzeMcDemux(DutData dutData)
        {
            var data = new List<List<double>>();
            foreach (var ch in dutData.Channels) data.Add(dutData.GetNonPolLossOf(ch));

            var analysis = analyzeMcDemux(data, dutData.mWaveStart, dutData.mWaveStep);

            //2x
            var result = new double[analysis.Length][];//[il~ax][w1~w6]

            for (int i = 0; i < result.Length; i++)
            {
                var numValue = analysis[i].Length;
                result[i] = new double[numValue + 3];// + min, max, uni

                for (int c = 0; c < numValue; c++) result[i][c] = analysis[i][c];

                double[] chValue = new double[numValue];
                Array.Copy(result[i], chValue, numValue);
                result[i][numValue + 0] = chValue.Min();
                result[i][numValue + 1] = chValue.Max();
                result[i][numValue + 2] = Math.Abs(Math.Round(result[i][numValue + 1] - result[i][numValue + 0], 3));
            }

            return result;
        }


        public static double[] MC_IL_WAVES = { 1264.0, 1277.8, 1284.0, 1297.8, 1304.0, 1317.8, 1324.0, 1337.8 };
        public static double[] MC_AX_WAVES = { 1264.5, 1277.5, 1284.5, 1297.5, 1304.5, 1317.5, 1324.5, 1337.5 };
        //static int[] mMcChIndex = { 1, 0, 2, 1, 3, 2 };
        static int[] mMcChIndex = { 0, 1, 1, 2, 2, 3 };
        static int[] mMcWaveIndex = { 2, 1, 4, 3, 6, 5 };

        /// <summary>
        /// calc Mc Demux IL, Ax
        /// </summary>
        /// <param name="data"></param>
        /// <param name="wlStart"></param>
        /// <param name="wlStep"></param>
        /// <returns>[il~ax][w1~w8]</returns>
        static double[][] analyzeMcDemux(List<List<double>> data, double wlStart, double wlStep)
        {
            var numCh = data.Count;
            var numAx = MC_IL_WAVES.Length - 2;

            var analysis = new double[][] { new double[numCh], new double[MC_IL_WAVES.Length - 2] };
            var trans = new double[numCh][];//[numCh,wl]

            //IL
            for (int c = 0; c < numCh; c++) trans[c] = Calculator.CalcTransOfWaves(DataPoint.Create(data[c], wlStart, wlStep), MC_IL_WAVES);
            for (int c = 0; c < numCh; c++) analysis[0][c] = Math.Min(trans[c][c * 2], trans[c][c * 2 + 1]);

            //Ax
            for (int c = 0; c < numCh; c++) trans[c] = Calculator.CalcTransOfWaves(DataPoint.Create(data[c], wlStart, wlStep), MC_AX_WAVES);
            for (int w = 0; w < numAx; w++) analysis[1][w] = trans[mMcChIndex[w]][mMcWaveIndex[w]];

            return analysis;
        }

        #endregion



        #region ---- CWDM ----

        const double PEAK_TO_BOUNDARY_TRANS_FOR_CWL = 6.0;

        public static double[][] AnalyzeNp(DutData dutData)
        {
            var data = new List<List<double>>();
            foreach (var ch in dutData.Channels)
            {
                data.Add(dutData.GetNonPolLossOf(ch));
            }

            double cwlRefLevel = 6.0;

            var analysis = AnalyzeNp(data, dutData.mWaveStart, dutData.mWaveStep, cwlRefLevel);

            //2x7
            var result = new double[2][];//ch1 ~ 4
            var keys = new string[] { Calculator.SK.PeakT, Calculator.SK.Dwl };
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[7];//peakLoss, Dwl
                double[] chValue = new double[4];
                for (int c = 0; c < 4; c++) chValue[c] = analysis[c][keys[i]];
                Array.Copy(chValue, result[i], 4);
                result[i][4] = chValue.Min();
                result[i][5] = chValue.Max();
                result[i][6] = Math.Abs(result[i][4] - result[i][5]);
            }

            return result;
        }

        public static Dictionary<string, double>[] AnalyzeNp(List<List<double>> data, double wlStart, double wlStep, double cwlRefLevel)
        {
            var plan = WavePlan.Cwdm;
            var chs = (DutCh[])Enum.GetValues(typeof(DutCh));
            var pol = Pol.Avg;

            double passBand = 10.0;

            var anlysis = new Dictionary<string, double>[data.Count];

            for (int i = 0; i < data.Count; i++)
            {
                var rawData = DataPoint.Create(data[i], wlStart, wlStep);
                anlysis[i] = Calculator.Analyze(chs[i], pol, rawData, cwlRefLevel, passBand, plan);
                anlysis[i][Calculator.SK.Pol] = (double)SpecScope.Ch;
            }
            return anlysis;
        }

        #endregion

    }//class
}
