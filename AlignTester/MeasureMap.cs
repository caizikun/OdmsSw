using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Free302.MyLibrary.Utility;
using Free302.MyLibrary.Config;
using System.IO;

namespace AlignTester
{
    public class MeasureMap : ConfigBase
    {

        #region ==== Class Framework ====

        protected MeasureMap(string name) : base(name)
        {
            //base.LoadConfig();
            //initMeasureMap();
        }

        const string MapFileExt = "XmlMap";
        protected override void initStorage(string configName)
        {
            base.initStorage(configName);
            mConfigFilePath = $@"{mConfigDirectory}\{configName}.{MapFileExt}";
        }
        void applyMap()
        {
            mChMap = this["DUT_CH_PM_CH_MAP"].Unpack<DutCh, PmCh>();

            PmPols = new PmPol[NumPol];
            Array.Copy(sPmPols, PmPols, NumPol);

            DutPols = new DutDatum[NumPol];
            Array.Copy(sDutPols, DutPols, NumPol);

            var data = new MeasureData<PmCh, PmPol>(NumCh, NumPol, LsSweepWave);
            NumDataPoint = data.NumDataPoint;
        }

        //read from map file & create instance
        public static MeasureMap CreateFromFile(string mapFilePath)
        {
            var name = Path.GetFileNameWithoutExtension(mapFilePath);
            var map = new MeasureMap(name);

            var file = mapFilePath;
            if (!File.Exists(file))
            {
                file = map.mConfigFilePath;
                if (!File.Exists(file)) throw new FileNotFoundException($"MeasureMap.LoadMapFile(): 맵 파일 <{mapFilePath}>가 없습니다.");
            }
            else
            {
                map.mConfigFilePath = file;
                map.mConfigDirectory = Path.GetDirectoryName(file);
            }

            map.LoadConfig();
            map.applyMap();         
            return map;
        }


        public string ToText()
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                mDs.Tables[0].WriteXml(sw);
                sw.Close();
            }
            return sb.ToString();
        }


        public static MeasureMap CreateFromString(string mapString)
        {
            var ds = new System.Data.DataSet();
            using (var sr = new StringReader(mapString))
            {
                ds.ReadXml(sr);
                sr.Close();
            }
            var map = new MeasureMap(ds.DataSetName);
            map.mDs = ds;
            map.applyMap();
            return map;
        }


        #endregion


        static PmPol[] sPmPols = MyEnum<PmPol>.ValueArray;
        static DutDatum[] sDutPols = MyEnum<DutDatum>.ValueArray;

        //pdl?
        public MeasurePDL MeasurePDL { get { return Get<MeasurePDL>("MEASURE_PDL"); } }
        public int NumPol { get { return MeasurePDL == MeasurePDL.Yes ? 4 : 1; } }
        public PmPol[] PmPols { get; private set; }
        public DutDatum[] DutPols { get; private set; }

        //ch
        Dictionary<DutCh, PmCh> mChMap;
        public Dictionary<DutCh, PmCh> ChMap { get { return mChMap; } }
        public int NumCh { get { return mChMap.Count; } }
        public DutCh[] DutChs { get { return mChMap.Keys.ToArray(); } }
        public PmCh[] PmChs { get { return mChMap.Values.Distinct().ToArray(); } }

        //tls
        public double LsPowerDbm { get { return Get<double>("LS_LASER_POWER_DBM"); } }
        public double[] LsSweepWave { get { return GetList<double>("LS_LASER_SWEEP_NM").ToArray(); } }
        public int NumDataPoint { get; private set; }

        //PC
        public double PcPolAngle
        {
            get { return Get<double>("PC_POL_ANGLE"); }
            set { Set("PC_POL_ANGLE", value); SaveConfig(); }
        }

        //pm
        public int[] PmGainLelvelsDbm { get { return GetList<int>("PM_GAIN_LEVELS_DBM").ToArray(); } }
        public PmAvgTime PmAvgTime { get { return Get<PmAvgTime>("PM_AVG_TIME"); } }
        public double PmWaveNm { get { return Get<double>("PM_WAVE_NM"); } }



        public static void Test()
        {
            var map = MeasureMap.CreateFromFile("CwdmMux_REF.XmlMap");
            var avg = map.PmAvgTime;
            var mapString = map.ToText();
            var map2 = MeasureMap.CreateFromString(mapString);
        }

    }//class
}
