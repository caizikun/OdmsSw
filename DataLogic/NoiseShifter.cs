using System;
using System.Collections.Generic;

namespace Neon.Aligner
{
    /// <summary>
    /// 특정 레벨 근처의 값을 지정한 최대 변화 크기 내에서 연속적으로 변화시킴
    /// </summary>
	public class NoiseShifter
	{
        /// <summary>
        /// NoiseShifter 사용법
        /// </summary>
        public static void Test()
        {
            var shifter = new NoiseShifter();
            shifter.TlsPower_dBm = -10;
            shifter.TlsSNR_dB = -40;
            shifter.Shift_dB = -3;

            var value = new List<double>();
            for (int i = 0; i < 21; i++)
            {
                var transValue = shifter.transform(dBm2mW(-60 + i));
                value.Add(mW2dBm(transValue));
            }
        }

        public override string ToString() => $"{TlsPower_dBm} {TlsSNR_dB} {mShift}";
        static double dBm2mW(double dbm) => Math.Pow(10, dbm / 10);
        static double mW2dBm(double mW) => Math.Log10(mW) * 10;


        #region ---- Input Parameters ----

        /// <summary>
        /// 최대 변화 크기
        /// </summary>
        public double Shift_dB
        {
            get { return mShift; }
            set
            {
                mShift = value;
                mWeightAmp = calcWeightAmp(value);
            }
        }
        double mShift;// shift dB
        double mWeightAmp;//weight
        static double calcWeightAmp(double shift_dB) => (1 - Math.Pow(10, shift_dB / 10)) / 0.104755;

        /// <summary>
        /// TLS 파워 레벨 ~ 파워 레벨이 변하면 노이즈 레벨도 변함
        /// </summary>
        public int TlsPower_dBm { get; set; }

        /// <summary>
        /// TLS의 SNR ~ TLS power = 출력파워 레벨과 노이즈 레벨간의 차이 ~ 0 dBm 일때의 노이즈 레벨
        /// </summary>
        public int TlsSNR_dB { get; set; }

        #endregion



        #region ---- Transform ----
        
        /// <summary>
        /// 노이즈 레벨 (dBm) = TLS Power(dBm)  + SNR(dB)
        /// </summary>
        double NoisePower_mW => dBm2mW(TlsPower_dBm + TlsSNR_dB);

        static double calcWeight(double weightAmp, double power_mW, double noisePower_mW)
        {
            var x = 10 * Math.Log10(power_mW / noisePower_mW);
            var w = weightAmp * Math.Exp(-1 * (x - 2.0723) * (x - 2.0723) / 18) / 3 / Math.Sqrt(2 * Math.PI);
            return w;
        }

        double transform(double power_mW)
        {
            var w = calcWeight(mWeightAmp, power_mW, NoisePower_mW);
            var p = power_mW - NoisePower_mW * w;
            return p;
        }

        public void Transform(List<double> power)
        {
            for (int i = 0; i < power.Count; i++) power[i] = transform(power[i]);
        }
        public void Transform(List<List<double>> power)
        {
            for (int i = 0; i < power.Count; i++) Transform(power[i]);
        }

        #endregion


    }
}
