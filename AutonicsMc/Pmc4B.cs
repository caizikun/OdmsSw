using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;

namespace Free302.TnM.Device
{
    /// <summary>
    /// Autonics 4축 MC
    /// </summary>
    public class Pmc4B : McBase<StageBase>
    {

        #region === Class Specific Service ===

		/// <summary>
		/// 감지된 PMC4B 카드의 주소목록을 준다.
		/// </summary>
		/// <returns></returns>
        public static List<PmcAddress> ScanPmc4B()
        {
            List<PmcAddress> scanResult = new List<PmcAddress>();
            UInt16 state = 0;

            for (ushort address = 0; address < PmcHwInfo.MaxNumPmc; address++)
            {
                state = Pmc4BWrap.pmc4bpci_open(address, null);

                if (state == Pmc4BWrap.MMC_OK) scanResult.Add((PmcAddress)address);
            }

            return scanResult;
        }

        #endregion


        
        #region === Error Handling ===

        /// <summary>
        /// checks if the PMC command return value
        ///  - if not MMC_OK then throws PmcException
        /// </summary>
        /// <param name="exId"></param>
        /// <param name="status"></param>
        protected static void checkPmcStatus(PmcExceptionId exId, ushort status, ushort address)
        {
            checkPmcStatus(exId, status, address, PmcAxis.None);
        }
        private static void checkPmcStatus(PmcExceptionId exId, UInt16 status, ushort address, PmcAxis pmcAxis)
        {
            if (status != Pmc4BWrap.MMC_OK)
            {
				PmcException pex = null;
                try
                {
					pex = new PmcException(exId);
					pex.Data["Pmc Address"] = address;
					pex.Data["Pmc Axis"] = pmcAxis.ToString();

					if (pmcAxis != PmcAxis.None)
					{
						UInt16 detailStatus = Pmc4BWrap.MMC_FALSE;
						
						detailStatus = Pmc4BWrap.pmc4bpci_flag_error(address, (UInt16)pmcAxis);
						pex.Data["Asix Error Flag"] = detailStatus;

						detailStatus = Pmc4BWrap.pmc4bpci_error();
						pex.Data["Error"] = detailStatus;
					}

                }
                catch (Exception ex)
                {
                    pex.Data["Detail"] = ex.Message;
					throw pex;
                }
                throw pex;
            }
        }

       

        /// <summary>
        /// check the axis of this PMC is connected to a real driver/stage
        /// </summary>
        /// <param name="axis"></param>
        private void checkAxis(PmcAxis axis)
        {
			PmcAxis[] axes = MyEnum<PmcAxis>.ValueArray;
			
			for (int i = 1; i < axes.Length; i++ )//0==None
			{
				if (!axis.HasFlag(axes[i])) continue;

				if (!this.mAxisMap.Values.Contains(axes[i]))
				{
					PmcException pex = new PmcException(PmcExceptionId.NotConnectedAxis);
					throw pex;
				}
			}
        }


        #endregion 


				
		#region === Class Framework ===

		private const string DefaultName = "PMC4B";
		private const PmcAddress DefaultPmcAddress = PmcAddress.PMC15;

		//생성자
		public Pmc4B() : this(McId.None){}

		public Pmc4B(McId id, string name = DefaultName) : base(id, name)
		{
			//PmcAddress
			PmcAddress = DefaultPmcAddress;

			//AxisMap
			mAxisMap = new Dictionary<McAxis, PmcAxis>();

			//global cancel flag
			initGlobalCancelFlag();

		}		

		#endregion ===



        #region ===== 장치 구성 - 사용자 설정 =====
        //instance data = 설정파일에 저장, 사용자 설정 UI에서 선택

        /// <summary>
        /// PMC-4B-PC 카드에 설정된 주소 0~15
        /// </summary>
        protected UInt16 mPmcAddress;
		public PmcAddress PmcAddress
		{
			get { return (PmcAddress)mPmcAddress; }
			set { mPmcAddress = (ushort)value; }
		}

        /// <summary>
        /// MC 의 McAxis와 PMC4B의 PmcAxis(실제값)간의 연결
        /// - {X1 ~ X4} <--> {X, Y, Z, U} 
        /// </summary>
        protected Dictionary<McAxis, PmcAxis> mAxisMap;
		
        private PmcAxis getPmcAxis(McAxis mcAxis)
        {
            return mAxisMap[mcAxis];
        }


		#region ----- IConfig -----

		override public DeviceConfig buildConfig()
		{
			base.buildConfig();

			//PmcAddress
			mConfig.Add("PmcAddress", this.PmcAddress);

			//AxisMap
			mConfig.Add("AxisMap", mAxisMap.Pack());

			return mConfig;
		}


		override public void applyConfig(DeviceConfig newConfig)
		{
			mIsConfigSet = false;

			//base
			base.applyConfig(newConfig);

			//PmcAddress
			mPmcAddress = (ushort)mConfig.Get<PmcAddress>("PmcAddress");

			//AxisMap
			mAxisMap = mConfig["AxisMap"].Unpack<McAxis, PmcAxis>();

			mIsConfigSet = true;
		}

		#endregion


		#endregion ===




		#region === Open & Close ===

		protected static Dictionary<ushort, int> mOpenCounter;
        static Pmc4B()
		{
			mOpenCounter = new Dictionary<ushort, int>();
        }
        protected static void openAddress(ushort address)
		{
			if(!mOpenCounter.ContainsKey(address)) mOpenCounter.Add(address, 0);
			int counter = mOpenCounter[address];

			if(counter == 0)
			{
				UInt16 status = Pmc4BWrap.pmc4bpci_open(address, null);
				checkPmcStatus(PmcExceptionId.Open, status, address);
            }
			mOpenCounter[address] = counter + 1;

		}
        protected static void closeAddress(ushort address)
		{
			if(!mOpenCounter.ContainsKey(address))
				throw new Exception($"등록되지 않은 PMC4B({address})를 닫으려 했습니다.");

			int counter = mOpenCounter[address];
			if (counter == 0) return;
				//throw new Exception($"닫힌 PMC4B({address})를 닫으려 했습니다.");

            if(counter == 1)
			{
				UInt16 status = Pmc4BWrap.pmc4bpci_close(address);
				checkPmcStatus(PmcExceptionId.Open, status, address);
				mOpenCounter[address] = 0;
			}
		}

		//Open
		protected override void performOpen()
		{
			//check the address if opened
			openAddress(mPmcAddress);

			//specific data of PMC4B
			foreach (McAxis mcAxis in mAxisMap.Keys)
			{
				setDriveMode(mcAxis);

				//load last position value
				StageBase stage = this.getStage(mcAxis);
				stage.loadDynamicData();				
				writePosition(mcAxis, stage.LastPositionμm);
			}
		}

		/// <summary>
		/// 열렸는지 검사
		/// 안열렸을시 PmcException(exId) throw
		/// </summary>
		/// <param name="exId"></param>
		private void checkOpen(PmcExceptionId exId)
		{
			if (mIsOpen) return;

			PmcException pex = new PmcException(exId);
			throw pex;
		}

		//Close
		protected override void performClose() 
		{
			if(!mIsOpen) return;

			this.stop();
            //while(this.isMoving()) Thread.Sleep(100);

            //save current position to config file
            //saveDanamicData();

            //call close
            closeAddress(mPmcAddress);

		}

		#endregion ===



		#region === Setup ===

		//select axis to operate on
		private void setAxis(PmcAxis axis)
		{
			ushort status = Pmc4BWrap.pmc4bpci_set_axis(this.mPmcAddress, (ushort)axis);
			checkPmcStatus(PmcExceptionId.setMode, status, mPmcAddress, axis);
		}

        /// <summary>
        ///  - reset the controller & all axes
		///  - perform initialization - setHwLimit();
        /// </summary>
        private void reset()
        {
            ushort status = Pmc4BWrap.pmc4bpci_reset(this.mPmcAddress, (UInt16)PmcAxis.All);
            checkPmcStatus(PmcExceptionId.Reset, status, mPmcAddress);

            setHwLimit();
        }

		private const PmcModeConfig DefaultMode = PmcModeConfig.LMT_SoftStop;

        //WR2 mode register
        protected void setDriveMode(McAxis mcAxis)
		{
			PmcModeConfig wData = DefaultMode;
			PmcAxis axis = getPmcAxis(mcAxis);
			StageBase stage = getStage(mcAxis);

			if (stage.SensorActiveLevel == StageSensorActiveLevel.ActiveHigh)
				wData |= PmcModeConfig.LMTP_ActiveHigh | PmcModeConfig.LMTN_ActiveHigh;
			//else wData &= (ushort)~(PmcModeConfig.LMTP_ActiveHigh | PmcModeConfig.LMTN_ActiveHigh);

			if (stage.DrivePulseMode == StageDrivePulseMode.PulseAndDir)
				wData |= PmcModeConfig.Mode_PulseAndDir;

			if (stage.DirPulseLevel == StageDirectionPulseLevel.PositiveHigh)
				wData |= PmcModeConfig.Dir_PositiveHigh;

			
			ushort status = Pmc4BWrap.pmc4bpci_wwr2(this.mPmcAddress, (UInt16)axis, (ushort)wData);
			checkPmcStatus(PmcExceptionId.setMode, status, mPmcAddress, axis);
		}

        /// <summary>
        /// Enables 'Hw Limit' signal for all Axes
        /// </summary>
        private void setHwLimit()
        {
            //Enable HW Limit signal
            ushort wData = Pmc4BWrap.WR2_LMT_HLMTP | Pmc4BWrap.WR2_LMT_HLMTM;
            ushort status = Pmc4BWrap.pmc4bpci_wwr2(this.mPmcAddress, (UInt16)PmcAxis.All, wData);
            checkPmcStatus(PmcExceptionId.setMode, status, mPmcAddress);
        }


		//WR1 : sensor config
        private void setSensorConfig(McAxis mcAxis, McSensorConfig sensor)
        {
			uint enableFlag = 0;
			uint levelFlag = 0;

			if(sensor.HasFlag(McSensorConfig.NORG))
			{
				enableFlag |= Pmc4BWrap.WR1_IN0_E;
				levelFlag |= Pmc4BWrap.WR1_IN0_L;//ActiveHigh
			}
			if (sensor.HasFlag(McSensorConfig.ORG))
			{
				enableFlag |= Pmc4BWrap.WR1_IN1_E;
				levelFlag |= Pmc4BWrap.WR1_IN1_L;//ActiveHigh
			}
			PmcAxis axis = getPmcAxis(mcAxis);
			checkAxis(axis);

			StageBase stage = getStage(mcAxis);
			uint flag = enableFlag;

			if (stage.SensorActiveLevel == StageSensorActiveLevel.ActiveHigh)
				flag |= levelFlag;	// 0x0010 | 0x0100
			else flag &= ~levelFlag;// 0x0010 & 0x1011 //ActiveLow

			//write to PMC
			setAxis(axis);
			ushort status = Pmc4BWrap.pmc4bpci_wwr1(this.mPmcAddress, (ushort)axis, (ushort)enableFlag);
			checkPmcStatus(PmcExceptionId.setNORG, status, mPmcAddress, axis);
		}



        #endregion

        

        #region === Drive Info ===


        #region getPosition, resetPosition

		public override Dictionary<McAxis, double> getPosition(McAxis mcAxis = McAxis.All)
        {
			Dictionary<McAxis, double> dic = new Dictionary<McAxis, double>();

			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
			foreach (McAxis axis in effectiveAxes.Keys)
			{
				dic.Add(axis, readPosition(axis));
			}

			return dic;
        }

		private double readPosition(McAxis mcAxis)
		{
			//dont check isOpen in private methods
			//checkOpen(PmcExceptionId.Read);

			PmcAxis axis = this.getPmcAxis(mcAxis);
			int internalPosition = Pmc4BWrap.pmc4bpci_get_logicalposition(this.mPmcAddress, (ushort)axis);

			PmcErrorStatus isError = readError();
			if (isError != PmcErrorStatus.TRUE) checkPmcStatus(PmcExceptionId.Read, (ushort)isError, mPmcAddress, axis);

			//************************************
			// 내부 -> 외부로 변환후 리턴
			//************************************
			StageBase stage = this.getStage(mcAxis);
			double factor = 1 / stage.UnitToInternal;
			var coord_um = internalPosition * factor;

            // 2019.01.25 by DrBAE ~ 좌표계 기본원칙에 따라 불필요
            //실제축 & 논리축간 좌표변환
            //var logic_coord_um = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength - coord_um : coord_um;
            return coord_um;
        }


		public override void resetPosition(McAxis mcAxis = McAxis.All)
		{
			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

			//cancel flag 초기화
			//initGlobalCancelFlag();
			foreach(McAxis axis in effectiveAxes.Keys)
			{
				mGlobalCancelFlag[axis] = false;
			}

			performAfterStopAsync(effectiveAxes, (x) => 
			{
				StageBase stage = getStage(x);
				stage.LastOriginDate = DateTime.Now;

                // 2019.01.25 by DrBAE
                // new coordnate to write
                var coord = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength : 0;
                writePosition(x, coord);
                stage.LastPositionμm = coord;
            });
		}
        protected void writePosition(McAxis mcAxis, double newPosition)
        {
            StageBase stage = this.getStage(mcAxis);

            // 2019.01.25 by DrBAE ~ 좌표계 기본원칙에 따라 불필요
            //실제축 & 논리축간 좌표변환
            //var coord_um = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength - newPosition : newPosition;

            //************************************
            // 외부 - 내부로 변환후 리턴
            //************************************
			int internalPosition = (int)(newPosition * stage.UnitToInternal);

			PmcAxis axis = this.mAxisMap[mcAxis];
			ushort status = Pmc4BWrap.pmc4bpci_set_lpcounter(this.mPmcAddress, (ushort)axis, internalPosition);
			checkPmcStatus(PmcExceptionId.SetPosition, status, mPmcAddress, axis);
        }

        
        #endregion


        #region isMoving

		protected override bool performIsMoving(McAxis mcAxis = McAxis.All)
		{
			bool driving = false;

			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

            if (mcAxis == McAxis.All) foreach (McAxis a in effectiveAxes.Keys) driving |= readIsDrive(mAxisMap[a]);
            else if (!effectiveAxes.ContainsKey(mcAxis)) return false;
            else driving = readIsDrive(mAxisMap[mcAxis]);

            return driving;
		}

		/// <summary>
		/// return true if the axis is driving
		/// </summary>
		/// <param name="axis"></param>
		/// <returns></returns>
		private bool readIsDrive(PmcAxis axis)
        {
			ushort status = Pmc4BWrap.pmc4bpci_is_drive(this.mPmcAddress, (ushort)axis);
			return status == Pmc4BWrap.MMC_TRUE;
        }
        #endregion


		#region Drive Status

		//PCM4B _error()
		private PmcErrorStatus readError()
		{
			return (PmcErrorStatus)Pmc4BWrap.pmc4bpci_error();
		}
		//pmc4b _is_error()
		private bool readIsError(PmcAxis axis)
		{
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_is_error(this.mPmcAddress, (ushort)axis);
		}
		//pmc4b _is_stop()
		private bool readIsStop(PmcAxis axis)
		{
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_is_stop(this.mPmcAddress, (ushort)axis);
		}
		//pmc4b _is_flag_in0 ~ connected to NORG signal of state
		private bool readFlagNORG()
		{
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_flag_in0(this.mPmcAddress);
		}
		//pmc4b _is_flag_in1
		private bool readFlagORG()
		{
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_flag_in1(this.mPmcAddress);
		}

		//pmc4bpci_flag_limitplus
		private bool readFlagLMTP()
		{
			//Pmc4BWrap.pmc4bpci_set_axis(this.mPmcAddress, (ushort)axis);
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_flag_limitplus(this.mPmcAddress);
		}
		//pmc4bpci_flag_limitplus
		private bool readFlagLMTN()
		{
			return (ushort)PmcErrorStatus.TRUE == Pmc4BWrap.pmc4bpci_flag_limitminus(this.mPmcAddress);
		}



		public override void readMotionStatus(McMotionParam param)
		{
			//position
			param.x = readPosition(param.axis);

			PmcAxis axis = mAxisMap[param.axis];
			param.isError = readIsError(axis);
			param.isStop = readIsStop(axis);

			param.isNorg = readFlagNORG();
			param.isOrg = readFlagORG();
			param.isLmtP = readFlagLMTP();
			param.isLmtN = readFlagLMTN();
		}

		#endregion 



		#region Reporting

		/// <summary>
		/// param에 지정한 축의 운동상태를 계속 읽어서 reporter로 보고한다.
		/// </summary>
		/// <param name="param"></param>
		/// <param name="reporter"></param>
		public override async void reportStatusAsync(McMotionParam param, IProgress<McMotionParam> reporter)
        {
			this.mReporter = reporter;
			if (mReporter == null) return;

			do
			{
				await Task.Delay(100);

				//read status
				param.resetStatus();
				readMotionStatus(param);

				//report progress
				if (mReporter != null) mReporter.Report(param);

			//} while (!param.isError && !param.isStop && !param.isComplete && mReporter != null);
			} while (mReporter != null);
        }

        //
        public override async void reportStatusAsync(McMotionParam[] paramList, IProgress<McMotionParam> reporter)
        {
			this.mReporter = reporter;
			if (mReporter == null) return;

            bool continueCondition;
            do
            {
                //sleep 1000ms 
                //Thread.Sleep(250);
                await Task.Delay(250);

                continueCondition = false;
                foreach (McMotionParam param in paramList)
                {
                    //read status
					readMotionStatus(param);

					//report
					if (mReporter != null) mReporter.Report(param);

                    //continueCondition |= (!param.isError && !param.isStop && !param.isComplete);
					continueCondition |= (!param.isError && !param.isStop && !param.isComplete);
                }
            //} while (continueCondition);
			} while (mReporter != null);
        }		
		
		#endregion 


		#endregion //===




		#region === Drive Methods ===


		public override void stop(McAxis mcAxis = McAxis.All)
        {
			PmcAxis x = PmcAxis.None;

			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
			foreach (McAxis a in effectiveAxes.Keys)
			{
				//global cancel flag
				mGlobalCancelFlag[a] = true;

				x |= mAxisMap[a];
			}
			Pmc4BWrap.pmc4bpci_stop(mPmcAddress, (ushort)x);
        }
		

        #region --- Move ---

		public override void move(McMotionParam param)
		{
			writeMove(param);
			mGlobalCancelFlag[param.axis] = false;
		}

		private void moveAndWait(McMotionParam param)
		{
			writeMove(param);
			mGlobalCancelFlag[param.axis] = false;

			//wait
			Pmc4BWrap.pmc4bpci_wait(this.mPmcAddress, (UInt16)this.getPmcAxis(param.axis));

			//read status
			readMotionStatus(param);
		}

		private void writeMove(McMotionParam param)
		{
			//check if this MC open
			checkOpen(PmcExceptionId.Move);

			//check if the axis connected
			PmcAxis pmcAxis = getPmcAxis(param.axis);
			checkAxis(pmcAxis);
			
			//Set Sensor
			McSensorConfig sensor = McSensorConfig.None;
			if (param.senseNORG) sensor |= McSensorConfig.NORG;
			if (param.senseORG) sensor |= McSensorConfig.ORG;
			this.setSensorConfig(param.axis, sensor);

			//stage info
			StageBase stage = getStage(param.axis);

			//배율 조정
			int R = PmcHwInfo.R(stage.PulseDivider);//400000
			Pmc4BWrap.pmc4bpci_set_range(mPmcAddress, (ushort)pmcAxis, R);

			//이동방식에 따른 유효이동거리 계산
			double dx0 = param.dx0;
            if (param.moveType == McMoveType.MoveTo) dx0 -= readPosition(param.axis);

            //내부 최소단위로 반올림
            dx0 = Math.Round(dx0 * stage.UnitToInternal);

            // 2019.01.25 by DrBAE ~ 좌표계 기본원칙에 따라 불필요
            //실제축 & 논리축간 좌표변환
            //dx0 = stage.OriginPolarity == StageOriginPolarity.PosLimit ? -dx0 : dx0;

            //move
            ushort status = Pmc4BWrap.pmc4bpci_pls_smove(mPmcAddress, (ushort)pmcAxis, (int)dx0, (short)param.speed,
				PmcAccel.ac, PmcAccel.aac, PmcAccel.dc, PmcAccel.ddc);
			checkPmcStatus(PmcExceptionId.Move, status, mPmcAddress, pmcAxis);
		}


		#endregion ---


		#region --- Zero & Home ---
		//***
        public override void moveToLogicalOrigin(McAxis mcAxis = McAxis.All)
        {
			this.stop(mcAxis);

			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
			foreach (McAxis ax in effectiveAxes.Keys)
			{
				McMotionParam param = new McMotionParam(this, ax);
				param.dx0 = 0;
				param.moveType = McMoveType.MoveTo;
				this.move(param);
			}
        }

        public override void moveToOrigin(McAxis mcAxis = McAxis.All)
        {
			this.stop(mcAxis);
			//this.stopAsync = false;

			Dictionary<McAxis, PmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

			foreach (McAxis ax in effectiveAxes.Keys)
			{
				StageBase stage = getStage(ax);

				McMotionParam param = new McMotionParam(this, ax, stage.StrokeLength);
				param.dx0 *= stage.UnitToPolar;//μm -> 외부 단위로 변환
				param.moveType = McMoveType.MoveAs;
				
				//calc home direction
				if (stage.OriginPolarity == StageOriginPolarity.NegLimit) param.dx0 *= -1;
				
				//param.senseNORG = true;
				this.move(param);
				
			}//foreach

		}        
        
		public override void moveToStrokeCenter(McAxis mcAxis = McAxis.All)
		{
			//move to origin
			//this.moveToOrigin(mcAxis);

			Dictionary<McAxis, PmcAxis> dicEff = getEffectiveAxis(mcAxis);

			//move as half of stroke
			//performAfterStopAsync(dicEff, (axis) => 
			//{
			//	StageBase stage = getStage(axis);

			//	McMotionParam param = new McMotionParam(this, axis, stage.StrokeLength / 2);
			//	param.dx0 *= stage.UnitToPolar;//μm -> 외부 단위로 변환
			//	param.moveType = McMoveType.MoveAs;

			//	//find oppisite origin direction
			//	if (stage.OriginPolarity == StageOriginPolarity.PosLimit) param.dx0 *= -1;

			//	//move~
			//	this.move(param);
			//});
			foreach (McAxis ax in dicEff.Keys)
			{
				StageBase stage = getStage(ax);

				McMotionParam param = new McMotionParam(this, ax, stage.StrokeLength / 2);
				param.dx0 *= stage.UnitToPolar;//μm -> 외부 단위로 변환
				param.moveType = McMoveType.MoveAs;

				//find oppisite origin direction
				if (stage.OriginPolarity == StageOriginPolarity.PosLimit) param.dx0 *= -1;

				//move~
				this.move(param);
			}




		}




		/// <summary>
		/// 주어진 McAxis에서 유효한 - MC에 연결되어 있는 - 축의 목록을 구한다.
		/// </summary>
		/// <param name="mcAxis"></param>
		/// <returns></returns>
		private Dictionary<McAxis, PmcAxis> getEffectiveAxis(McAxis mcAxis = McAxis.All)
		{
			McAxis[] axes = MyEnum<McAxis>.ValueArray;
			Dictionary<McAxis, PmcAxis> dic = new Dictionary<McAxis, PmcAxis>();

			foreach (var a in axes)
			{
				if (a == McAxis.None) continue;
				if (a == McAxis.All) continue;
				if (!mcAxis.HasFlag(a)) continue;
				if (mAxisMap.ContainsKey(a)) dic.Add(a, mAxisMap[a]);
			}
			return dic;
		}

		private delegate void ActionAfterStop(McAxis axis);
		private Dictionary<McAxis, bool> mGlobalCancelFlag;
		private void initGlobalCancelFlag()
		{
			if(mGlobalCancelFlag == null) mGlobalCancelFlag = new Dictionary<McAxis, bool>();
			mGlobalCancelFlag.Clear();

			McAxis[] axisList = (McAxis[])Enum.GetValues(typeof(McAxis));
			foreach(McAxis axis in axisList)
			{
				if(axis == McAxis.None) continue;
				if(axis == McAxis.All) continue;
				mGlobalCancelFlag.Add(axis, false);
			}
		}
		//private void setGlobalCancelFlag(McAxis axis, bool )

		private async void performAfterStopAsync(Dictionary<McAxis, PmcAxis> axisDic, ActionAfterStop action)
		{
			List<McAxis> stopList = new List<McAxis>();

			while (axisDic.Count > 0)
			{
				await Task.Delay(1);

				//find stopped axis
				stopList.Clear();
				foreach (McAxis mcAxis in axisDic.Keys)
				{
					if(mGlobalCancelFlag[mcAxis] || !readIsDrive(axisDic[mcAxis])) stopList.Add(mcAxis);
				}

				//perform action on the stopped axis
				foreach(McAxis mcAxis in stopList)
				{
					if(!mGlobalCancelFlag[mcAxis])
					{
						action(mcAxis);//perform the action
						axisDic.Remove(mcAxis);
					}
				}
			}
		}


		#region ...test : BackgroundWorker
		void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.DoWork += worker_DoWork;
			//worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
			worker.RunWorkerCompleted += worker_RunWorkerCompleted;
		}

		void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			throw new NotImplementedException();
		}

		#endregion ...test

		#endregion ---


		#endregion ===




	}
}
