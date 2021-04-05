using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;

namespace Free302.TnM.Device
{

	/// <summary>
	/// Motion Controller 추상화
	/// </summary>
	/// <typeparam name="TStage">type of Stage</typeparam>
    public abstract class McBase<TStage> : DeviceBase<McId>, IMc where TStage : StageBase, new()
    {

		#region === Class Framework ===


		//생성자		
		public McBase(McId id, string name = DefaultName) : base(id, name)
		{
			//AxisStageIdMap
			mAxisStageIdMap = new Dictionary<McAxis, StageId>();

			//StageMap
			mStageMap = new Dictionary<StageId, TStage>();

		}


		#endregion



		#region === 장치 구성 - 사용자 설정 ===

		private const string DefaultName = "McBase";

        /// <summary>
        /// MC에 현재 연결되어 있는 축
        /// </summary>
        public List<McAxis> EffectiveAxis
        {
			get { return this.mAxisStageIdMap.Keys.ToList(); }
        }
        public IEnumerable<McAxis> EffectiveAxes(McAxis axis) => mAxisStageIdMap.Keys.Where(x => axis.HasFlag(x));

        /// <summary>
        /// 각 축에 연결된 스테이지 사전
        /// </summary>
        private Dictionary<McAxis, StageId> mAxisStageIdMap;
		private Dictionary<StageId, TStage> mStageMap;
		protected StageId getStageId(McAxis mcAxis)
		{
			return mAxisStageIdMap[mcAxis];
		}
		public StageBase getStage(McAxis mcAxis)
		{
			StageId sid = mAxisStageIdMap[mcAxis];
			TStage stage = mStageMap[sid];
			return stage;
		}
		
		public void setReportDelay(McAxis mcAxis, int delayTime)
		{
			StageId sid = mAxisStageIdMap[mcAxis];
			TStage stage = mStageMap[sid];
			stage.ReportDelayTime_ms = delayTime;
		}

		
		public int getSpeedValue(McAxis mcAxis, McSpeed mcSpeed)
		{
			StageBase stage = this.getStage(mcAxis);
			int speed = stage.SpeedMap[mcSpeed];
			return speed;

			//return mSpeedMap[speed];
		}
		public void setSpeedValue(McAxis mcAxis, int speedValue)
		{
			StageBase stage = this.getStage(mcAxis);
			stage.SpeedMap[McSpeed.Fast] = speedValue;
		}


		#region --- IDeviceConfig 구현 ---

		override public DeviceConfig buildConfig()
		{
			base.buildConfig();//DeviceBase

            //AxisStageIdMap
            mConfig.Add("AxisStageIdMap", mAxisStageIdMap.Pack());

            //StageMap
            foreach (var stage in mStageMap.Values) mConfig.CopyFrom(stage.buildConfig());

			return mConfig;
		}

		override public void applyConfig(DeviceConfig newConfig)
		{
			base.applyConfig(newConfig);

			mIsConfigSet = false;

            //AxisStageIdMap
            mAxisStageIdMap = mConfig["AxisStageIdMap"].Unpack<McAxis, StageId>();

			//StageMap
			buildStageMap(newConfig);

			mIsConfigSet = true;
		}

		protected void buildStageMap(DeviceConfig newConfig)
        {
            //StageMap
            if (mStageMap == null) mStageMap = new Dictionary<StageId, TStage>();
            mStageMap.Clear();

            foreach (StageId sid in mAxisStageIdMap.Values)
            {
                TStage stage = new TStage();
                stage.Id = sid;
				stage.applyConfig(newConfig);//
                mStageMap.Add(sid, stage);
            }
        }

		#endregion --IConfig

		
		#endregion ===



		#region === Open & Close ===

		//Open
		public void OpenBase()
		{
			base.Open();
        }
		//Close
		public void CloseBase()
		{
			base.Close();
            SaveDynamicData();
        }

		#endregion ===



		#region === Setup ===

		//---------------------------------
		//reset...?  ~ close & open?
		//---------------------------------

		#endregion



		#region === Drive Info ===

		public abstract Dictionary<McAxis, double> getPosition(McAxis axis = McAxis.All);
		public abstract void resetPosition(McAxis axis = McAxis.All);
		public virtual bool isMoving(McAxis axis = McAxis.All)
		{
			McAxis[] axisList = MyEnum<McAxis>.ToFlagArray(axis);
			List<int> delay = new List<int>();
			foreach(McAxis ax in axisList)
			{
				if(!mAxisStageIdMap.ContainsKey(ax)) continue;
				StageBase stage = getStage(ax);
				delay.Add(stage.ReportDelayTime_ms);
			}

			bool result = performIsMoving(axis);
            Thread.Sleep(delay.Max());
			return result;
		}
		protected abstract bool performIsMoving(McAxis axis = McAxis.All);

        public void SaveDynamicData() => SaveDynamicData(McAxis.All);
        public void SaveDynamicData(McAxis axis = McAxis.All)
        {
            //save current position to config file
            Dictionary<McAxis, double> pos = this.getPosition();

            if (axis == McAxis.All)
            {
                foreach (var x in pos.Keys)
                {
                    StageBase stage = this.getStage(x);
                    stage.LastPositionμm = pos[x];
                    stage.saveDynamicData();
                }
            }
            else if (axis != McAxis.None && pos.ContainsKey(axis))
            {
                StageBase stage = this.getStage(axis);
                stage.LastPositionμm = pos[axis];
                stage.saveDynamicData();
            }
        }

		public abstract void readMotionStatus(McMotionParam param);

		//callback to report drive status
		protected IProgress<McMotionParam> mReporter;
		public abstract void reportStatusAsync(McMotionParam param, IProgress<McMotionParam> reporter);
		public abstract void reportStatusAsync(McMotionParam[] paramList, IProgress<McMotionParam> reporter);

        #endregion
		


        #region === Drive ===

        public abstract void stop(McAxis axis = McAxis.All);
        
        /// <summary>
        /// move sync without progress reporting
        /// </summary>
        /// <param name="param"></param>
		public abstract void move(McMotionParam param);


        /// <summary>
        /// go to the HW limit ~ ORG sensing?
        /// </summary>
        /// <param name="axis"></param>
		public abstract void moveToOrigin(McAxis axis = McAxis.All);

        /// <summary>
        /// logical(SW) origin
        /// </summary>
        /// <param name="axis"></param>
        public abstract void moveToLogicalOrigin(McAxis axis = McAxis.All);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="axis"></param>
		public abstract void moveToStrokeCenter(McAxis axis = McAxis.All);


        #endregion
		
		
	}
}
