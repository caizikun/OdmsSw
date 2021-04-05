using System;
using System.Linq;
using System.Drawing;
using DrBae.TnM.UI;
using System.Collections.Generic;

namespace Neon.Aligner
{
    public class DataPlot
    {
        //public static void Plot(object graph, DutData sd, double shift, int startPort = 1)
        //{}
        //public static void Plot(object graph, double[] data, double[] wave, double shift)
        //{ }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="graph"></param>
        ///// <param name="sd"></param>
        ///// <param name="shift"></param>
        ///// <param name="startPort">첫 채널의 번호: 1 or 5 </param>
        //public static void Plot(WaveformGraph graph, DutData sd, double shift, int startPort = 1)
        //{
        //    int numCh = sd.mTrans.Count();
        //    var wave = new double[] { sd.mWaveStart, Math.Round(sd.mWaveStep, 3) };

        //    for (int i = 0; i < numCh; i++)
        //    {
        //        //channel data.
        //        PortPowers data = sd.mTrans.Find(p => p.Port == (i + startPort));
        //        Plot(graph, data.NonPol.ToArray(), wave, shift);
        //    }
        //}
        ///// <summary>
        ///// 주어진 Graph에 주어진 데이터의 Plot을 추가한다.
        ///// RefForm 에서 2건 사용중
        ///// </summary>
        ///// <param name="graph"></param>
        ///// <param name="data"></param>
        ///// <param name="wave"></param>
        ///// <param name="shift"></param>
        //public static void Plot(WaveformGraph graph, double[] data, double[] wave, double shift)
        //{
        //    //plot
        //    var plot = new NationalInstruments.UI.WaveformPlot();
        //    plot.LineColor = mColors[_colorINdex];

        //    plot.DefaultStart = wave[0];
        //    plot.DefaultIncrement = wave[1];

        //    plot.XAxis = graph.Plots[0].XAxis;
        //    plot.YAxis = graph.Plots[0].YAxis;

        //    if (shift != 0) plot.PlotY(data.Select(x => x + shift).ToArray());
        //    else plot.PlotY(data);

        //    _colorINdex = (_colorINdex + 1) % mColors.Length;

        //    graph.Plots.Add(plot);
        //}


        public static Color[] mColors = WdmGraph.Colors;
            //new Color[] { Color.White, Color.Tomato, Color.Gold, Color.LimeGreen, Color.DeepSkyBlue, Color.Red, Color.Green, Color.Orange, Color.Violet };

        static Dictionary<Ch, double[]> dut2ds(DutData dut, double shift)
        {
            return dut.mTrans.ToDictionary(pp => (Ch)pp.Port, pp => pp.NonPol.Select(t => t + shift).ToArray());
        }
        public static void Plot(IWdmGraph wg, DutData dut, double shift, int startPort = 1)
        {
            wg.DataSource = () => dut2ds(dut, shift);
            wg.ScaleFactorY = 1;

            var start = dut.mWaveStart * 1000;
            var step = (int)(dut.mWaveStep * 1000);
            wg.ScaleFactorX = 1000;
            wg.Wl = Enumerable.Range(0, dut.NumDataPoints).Select(i => start + i * step).ToList();
            wg.Refresh();
        }

        public static void Plot(IWdmGraph wg, double[] data, double[] wave, double shift)
        {
            Plot(wg, data, wave, shift, Ch.CH0);
        }
        public static void Plot(IWdmGraph wg, double[] data, double[] wave, double shift, Ch ch)
        {
            var chs = new Ch[] { ch };
            var dd = new double[][] { data };
            Plot(wg, dd, wave, shift, chs);
        }

        public static void Plot(IWdmGraph wg, IList<double[]> data, double[] wave, double shift, Ch[] chs)
        {
            wg.ScaleFactorX = 1000;
            var start = (int)(wave[0] * 1000);
            var step = (int)(wave[1] * 1000);
            wg.Wl = Enumerable.Range(0, data[0].Length).Select(i => start + i * step).ToList();

            var numCh = data.Count;
            var dic = new Dictionary<Ch, double[]>();
            for (int i = 0; i < numCh; i++) dic.Add(chs[i], data[i].Select(v => v + shift).ToArray());
            wg.DataSource = () => dic;

            wg.Refresh();
        }


    }//class
}
