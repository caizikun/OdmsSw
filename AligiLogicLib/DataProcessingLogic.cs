using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public class DataProcessingLogic
    {
        public static void ApplyMovingAverage(IList<double> listMA, int numAvg)
        {
            if (numAvg < 2) return;
            var numDp = listMA.Count;

            for (int i = 0; i < numDp; ++i)
            {
                int j0 = i - (numAvg / 2) - (numAvg % 2) + 1;
                if (j0 < 0) j0 = 0;

                int j1 = i + numAvg / 2;
                if (j1 > numDp - 1) j1 = numDp - 1;

                var value = 0.0;
                for (int j = j0; j <= j1; ++j) value += listMA[j];
                value = value / (j1 - j0 + 1);

                listMA[i] = value;
            }
        }

        public static double Interpolate_1st(double x1, double x2, double y1, double y2, double xp)
            => (x1 == x2) ? y1 : y1 + (y2 - y1) / (x2 - x1) * (xp - x1);
        public static double Interpolate_1st(double[] x, double[] y, double xp) => Interpolate_1st(x[0], x[1], y[0], y[1], xp);

        class DataPoint
        {
            public int x = 0;
            public double y = double.NaN;
            public DataPoint(int x, double y)
            {
                this.x = x;
                this.y = y;
            }
        }

        /// <summary>
        /// 주어진 데이터의 FWHM 영역을 이용하여 센터 좌표를 구한다.
        /// </summary>
        /// <param name="yData"></param>
        /// <param name="FWHM"></param>
        /// <returns>[center X, width=xR-xL]</returns>
        public static double[] CalcCenter(IList<double> yData, double FWHM)
        {
            var result = new double[2] { double.NaN, 0.0 };

            ApplyMovingAverage(yData, 5);//apply 5 points moving average

            var _dps = yData.Select((y, x) => new DataPoint(x, yData[x])).ToArray();

            var peakY = yData.Max();
            var minY = yData.Min();
            var HM = (peakY + minY) / 2.0;
            if (peakY < 3.0 * minY) return result;

            //HM range
            int peakX = _dps.First(p => p.y == peakY).x;//peakIndex
            var dpL = _dps.LastOrDefault(p => p.y < HM && p.x < peakX);//left dp
            var dpR = _dps.FirstOrDefault(p => p.y < HM && p.x > peakX);//right dp

            if (dpL == null || dpR == null) return result;

            double xR;
            double xL;
            if (dpL == null)
            {
                xR = performInter(dpR, -1, HM);
                xL = xR - FWHM;
            }
            else
            {
                if (dpR == null)
                {
                    xL = performInter(dpL, 1, HM);
                    xR = xL + FWHM;
                }
                else
                {
                    xL = performInter(dpL, 1, HM);
                    xR = performInter(dpR, -1, HM);
                }
            }
            return new double[] { (xL + xR) / 2.0, xR - xL };

            double performInter(DataPoint dp, int dp2Index, double yp)
            {
                if (dp.x + dp2Index < 0 || dp.x + dp2Index >= _dps.Length) return dp.x;
                return Interpolate_1st(dp.y, _dps[dp.x + dp2Index].y, dp.x, dp.x + dp2Index, yp);

            }
        }//CalcCenter

    }//class
}
