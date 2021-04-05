using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Data;
using Free302.MyLibrary.Utility;
using Free302.MyLibrary.Config;

namespace Free302.TnM.Device
{

	#region === Type Definition ===

	public enum StageId { None = 0, S1 = 1, S2, S3, S4, S5, S6 }

	public enum StageDrivePulseMode
	{
		PulsePerDir, //independent pulse for each direction
		PulseAndDir	//pulse and direction
	}
	public enum StageDirectionPulseLevel
	{
		PositiveLow = 0,	//move positive at high level
		PositiveHigh = 1,	//move positive at low level
	}

	/// <summary>
	/// stage polarity - polar & cartesian direction mapping
	/// varied by MC & Driver connection
	/// </summary>
	//public enum StageDrivePolarity
	//{
	//	PosCCW, //MC Positive pulse output connected to Driver CCW input
	//	NegCCW  //MC Positive pulse output connected to Driver CW input
	//}


	/// <summary>
    /// mechanical position of the stage position sensor
    /// </summary>
    public enum StageOriginPolarity
    {
        PosLimit,   //the orging sensor is near the postive direction limit
		NegLimit   //the orging sensor is near the postive direction limit
    }


    /// <summary>
    /// logical active TTL level
    /// </summary>
    public enum StageSensorActiveLevel { ActiveLow = 0, ActiveHigh = 1 }


    [Flags]
    public enum StageSensorType : ushort
    {
        None = 0x00, LmtPos = 0x01, LmtNeg = 0x02, NORG = 0x04, ORG = 0x08, 
		All = LmtPos | LmtNeg | NORG | ORG
    }



	#endregion


	/// <summary>
    /// Encapsulate the Stage & Driver Information
    /// </summary>
    public class StageBase : IDeviceConfig, IDeviceDynamicConfig
	{

		#region === Data Member ===

		//dirve pulse mode
		public StageDrivePulseMode DrivePulseMode { get; set; }
		public StageDirectionPulseLevel DirPulseLevel { get; set; }

		/// <summary>
		/// μm -> pulse conversion factor : pulse/μm
		/// </summary>
		public int PulseDivider { get; set; }

		/// <summary>
		/// μm -> 외부단위 conversion factor = degree/μm
		/// ex) 1000μm * 0.003º/μm -> 3º
		/// </summary>
		public double UnitToPolar { get; set; }

		/// <summary>
		/// mechanical dimension [μm]
		/// </summary>
		public double StrokeLength { get; set; }
		//public int OrginRange { get; set; }			//the range of active NORG
		//public int LengthOrgNorgCCW { get; set; }		//distance between origin(center) to CCW NORG 
		//public int LengthOrgNorgCW { get; set; }		//distance between origin(center) to CW NORG 

		//sensor
		public StageSensorType SensorType { get; set; }
		public StageSensorActiveLevel SensorActiveLevel { get; set; }
		public StageOriginPolarity OriginPolarity { get; set; }


		//utility: display unit ~ MC internal unit
		public double UnitToInternal { get { return PulseDivider / UnitToPolar; } }


		/// <summary>
		/// speed map, unit= μm/s
		/// </summary>
		protected Dictionary<McSpeed, int> mSpeedMap;
		public Dictionary<McSpeed, int> SpeedMap
		{
			get { return this.mSpeedMap; }
		}

		/// <summary>
		/// time interval between drive complete and report 
		/// </summary>
		public int ReportDelayTime_ms { get; set; }


		#endregion



		#region === Class Framework ===

		public StageBase() : this(StageId.None) {}

		public StageBase(StageId id, string name = DefaultName)
		{
			this.Id = id;
			this.mName = name;
			//this.DynamicDataFile = this.mConfig.ConfigName;

			mSpeedMap = new Dictionary<McSpeed, int>();

			initDefault();

			initDynamicDefault();
		}

		/// <summary>
		/// stage 실제 속성을 적용하는 메소드
		/// </summary>
		virtual protected void initDefault()
		{
			//dirve pulse mode
			DrivePulseMode = StageDrivePulseMode.PulsePerDir;
			DirPulseLevel = StageDirectionPulseLevel.PositiveLow;
			PulseDivider = 1;
			UnitToPolar = 1;

			//mechanical dimension [μm] - K102M60
			StrokeLength = 71190.00;			
			//OrginRange = 5023;
			//LengthOrgNorgCCW = 2511;
			//LengthOrgNorgCW = 2512;

			//sensor
			SensorType = StageSensorType.All;
			SensorActiveLevel = StageSensorActiveLevel.ActiveHigh;
			OriginPolarity = StageOriginPolarity.PosLimit;

			//speem map
			mSpeedMap.Add(McSpeed.Slow, 10);
			mSpeedMap.Add(McSpeed.Mid, 100);
			mSpeedMap.Add(McSpeed.Fast, 1000);

			//
			ReportDelayTime_ms = 0;

		}

		override public string ToString()
		{
			return string.Format("{0} @ ID={1}, Config={2}", mName, Id, ConfigName);
		}

		#endregion



		#region === Config 관리 ===

		private const string DefaultName = "StageBase";

		protected string mName;

		virtual public string ConfigName 
		{ 
			get { return string.Format("{0}.{1}", mName, Id); } 
		}

		//unique id of this stage
		public StageId Id { get; set; }

		DeviceConfig mConfig;


		virtual public DeviceConfig buildConfig()
		{
			if (mConfig == null) mConfig = new DeviceConfig(this.ConfigName);
			else mConfig.Clear();

			//mConfig.Add("Id", Id);

			//dirve pulse mode
			mConfig.Add("DrivePulseMode", DrivePulseMode);
			mConfig.Add("DirPulseLevel", DirPulseLevel);
			mConfig.Add("PulseDivider", PulseDivider);
			mConfig.Add("UnitToPolar", UnitToPolar);

			//mechanical dimension [μm]
			mConfig.Add("StrokeLength", StrokeLength);
			//mConfig.Add("LastPositionμm", LastPositionμm);
			//mConfig.Add("OrginRange", OrginRange);
			//mConfig.Add("LengthOrgNorgCCW", LengthOrgNorgCCW);
			//mConfig.Add("LengthOrgNorgCW", LengthOrgNorgCW);

			//sensor
			//mConfig.Add("SensorType", SensorType);//motor stage - sensor & encoder
			mConfig.Add("SensorActiveLevel", SensorActiveLevel);
			mConfig.Add("OriginPolarity", OriginPolarity);

			//dynamic data
			mConfig.Add("DynamicDataFile", DynamicDataFile);

			//speed map
			mConfig.Add("SpeedMap", mSpeedMap.Pack());

			//report delay time
			mConfig.Add("ReportDelayTime_ms", ReportDelayTime_ms);

			return mConfig;
		}

		/// <summary>
		/// 새 config에서 현재 config 추출, 적용
		/// </summary>
		/// <param name="newConfig"></param>
		virtual public void applyConfig(DeviceConfig newConfig)
		{
			if (this.mConfig == null) mConfig = new DeviceConfig(ConfigName);
			mConfig.Load(newConfig);

			//this.Id = mConfig.Get<StageId>("Id");

			//dirve pulse mode
			DrivePulseMode = mConfig.Get<StageDrivePulseMode>("DrivePulseMode");
			DirPulseLevel = mConfig.Get<StageDirectionPulseLevel>("DirPulseLevel");
			PulseDivider = mConfig.Get<int>("PulseDivider");
			UnitToPolar = mConfig.Get<double>("UnitToPolar");

			//mechanical dimension [μm]
			StrokeLength = mConfig.Get<double>("StrokeLength");
			//LastPositionμm = mConfig.Get<double>("LastPositionμm");
			//this.OrginRange = mConfig.Get<int>("OrginRange");
			//this.LengthOrgNorgCCW = mConfig.Get<int>("LengthOrgNorgCCW");
			//this.LengthOrgNorgCW = mConfig.Get<int>("LengthOrgNorgCW");

			//sensor
			//SensorType = mConfig.Get<StageSensorType>("SensorType");
			SensorActiveLevel = mConfig.Get<StageSensorActiveLevel>("SensorActiveLevel");
			OriginPolarity = mConfig.Get<StageOriginPolarity>("OriginPolarity");

			//dynamic data
			DynamicDataFile = mConfig.Get<string>("DynamicDataFile");

			//speed map
            mSpeedMap = mConfig["SpeedMap"].Unpack<McSpeed, int>();

			//report delay time
			ReportDelayTime_ms = mConfig.Get<int>("ReportDelayTime_ms");
        }

		#endregion




		#region ==== Dynamic Config ====

		DeviceConfig mDynamicConfig;
        object _lockDynamicConfig = new object();

		public string DynamicDataFile { get; set; }

		public double LastPositionμm { get; set; }

		public DateTime LastOriginDate { get; set; }

		void initDynamicDefault()
		{
			LastPositionμm = 0;
			LastOriginDate = DateTime.Now;
        }

		public void loadDynamicData()
		{
            lock (_lockDynamicConfig)
            {
                if (mDynamicConfig == null) mDynamicConfig = new DeviceConfig(DynamicDataFile);

                try
                {
                    mDynamicConfig.LoadConfig();
                }
                catch (Exception)
                {
                    mDynamicConfig = new DeviceConfig(DynamicDataFile);
                    saveDynamicData();//default file
                    mDynamicConfig.LoadConfig();
                } 
            }

			//apply
			applyDynamicData(mDynamicConfig);
		}

		protected void applyDynamicData(DeviceConfig config)
		{
			LastPositionμm = config.Get<double>("LastPositionμm");
			LastOriginDate = DateTime.Parse(config["LastOriginDate"]);
        }

		public void saveDynamicData()
		{
            lock (_lockDynamicConfig)
            {
                if (mDynamicConfig == null) mDynamicConfig = new DeviceConfig(DynamicDataFile);

                buidlDynamicData(mDynamicConfig);

                mDynamicConfig.SaveConfig(); 
            }
		}

		protected void buidlDynamicData(DeviceConfig config)
		{
			config.Clear();

			//config.Set<double>("LastPositionμm", LastPositionμm);
			config.Add("LastPositionμm", LastPositionμm);
			config.Add("LastOriginDate", LastOriginDate.ToString("o"));
        }



		#endregion



	}//--- stage base


}
