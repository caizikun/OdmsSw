using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Threading.Tasks;
using Free302.MyLibrary.Utility;

namespace Neon.Aligner
{

    public class ConfigTlsParam
    {
        public int Power = -15;
        public int WaveStart = 1520;
        public int WaveStop = 1620;
        public double WaveStep = 0.050;
        public int Speed = 40;

        //public static ConfigTlsParam Create(string str)
        //{
        //    var list = str.Unpack<double>().ToArray();
        //    var param = new ConfigTlsParam();
        //    param.Power = (int)list[0];
        //    param.WaveStart = (int)list[1];
        //    param.WaveStop = (int)list[2];
        //    param.WaveStep = list[3];
        //    param.Speed = (int)list[4];
        //    return param;
        //}

        public override string ToString()
        {
            return $"{Power}dBm {WaveStart}~{WaveStop}/{WaveStep:F03}nm {Speed}nm/s";
        }
    }
    
}
