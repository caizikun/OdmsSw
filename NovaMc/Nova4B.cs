using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.Utility;
using static Free302.TnM.Device.Nova4BWrap;

namespace Free302.TnM.Device
{
	/// <summary>
	/// Nova 4축 MC
	/// </summary>
	public class Nova4B : McBase<StageBase>
	{

		#region --- Error Function ---

		public UInt16 Nova8B_is_error(UInt16 id, McAxis mcAxis)
		{
			UInt16 errorCode = Nova8BWrap.MMC_OK;

			StageBase stage = getStage(mcAxis);
			NmcAxis axis = getNovaAxis(mcAxis);
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());

			// RR2 레지스터를 읽어서 에러코드 값을 리턴
			errorCode = (UInt16)MC8041P.Nmc_ReadReg2(id, icNo, (AXIS)Enum.Parse(typeof(AXIS), axis.ToString().ToUpper()));

			if ((errorCode & 0x0001) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;         // SLMT+ Error, 리턴값 결정 필요
			else if ((errorCode & 0x0002) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // SLMT- Error, 리턴값 결정 필요
			else if ((errorCode & 0x0004) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // HLMT+ Error, 리턴값 결정 필요
			else if ((errorCode & 0x0008) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // HLMT- Error, 리턴값 결정 필요
			else if ((errorCode & 0x0010) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // ALARM Error, 리턴값 결정 필요
			else if ((errorCode & 0x0020) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // EMG Error, 리턴값 결정 필요

			return Nova8BWrap.MMC_OK; // 에러코드
		}

		// RR0 D4~D7 보간드라이브의 에러 체크
		// id 번호가 범위를 벗어나면 MMC_INVALID_CARD 리턴
		// 보간 드라이브의 레벨이 High 이면 MMC_HIGH_LEVEL을 리턴
		// 보간 드라이브의 레벨이 Low 이면 MMC_LOW_LEVEL을 리턴
		// 에러가 없으면 MMC_OK를 리턴

		// 각칩의 각축에 이상이 있으면 해당하는 축의 Error 상태 리턴
		public UInt16 Nova8B_flag_error(UInt16 id, McAxis mcAxis)
		{
			UInt16 errorCode = Nova8BWrap.MMC_OK;

			StageBase stage = getStage(mcAxis);
			NmcAxis axis = getNovaAxis(mcAxis);
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());

			// M/C 보드 번호 오류 체크
			if ((id > 15) || (id < 0)) return Nova8BWrap.MMC_INVALID_CARD;

			// RR0 레지스터를 읽어서 에러코드 값을 리턴
			errorCode = (UInt16)MC8041P.Nmc_ReadReg0(id, icNo);

			if ((errorCode & 0x0010) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;         // X 축 Error, 리턴값 결정 필요
			else if ((errorCode & 0x0020) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // Y 축 Error, 리턴값 결정 필요
			else if ((errorCode & 0x0040) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // Z 축 Error, 리턴값 결정 필요
			else if ((errorCode & 0x0080) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;    // U 축 Error, 리턴값 결정 필요

			/*			
						// RR2 레지스터를 읽어서 에러코드 값을 리턴
						errorCode = (UInt16)MC8000P.Nmc_ReadReg2(id, icNo, (AXIS)Enum.Parse(typeof(AXIS), axis.ToString().ToUpper()));
						if (readIsDrive(mcAxis))
						{
							if (stage.StrokeLength > 0)
							{
								if ((errorCode & 0x0001) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;
								else return Nova8BWrap.MMC_LOW_LEVEL;
							}
							else
							{
								if ((errorCode & 0x0002) > 0) return Nova8BWrap.MMC_HIGH_LEVEL;
								else return Nova8BWrap.MMC_LOW_LEVEL;
							}
						}
			*/
			return Nova8BWrap.MMC_OK;
		}


		// 보드를 Open 후에 발생하는 Error에 대한 코드
		// Open 상태, Axis 상태 etc....
		public UInt16 Nova8B_error(UInt16 id, McAxis mcAxis)
		{
			StageBase stage = getStage(mcAxis);
			NmcAxis axis = getNovaAxis(mcAxis);
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());

			// RR2 레지스터를 읽어서 에러코드 값을 리턴
			UInt16 errorCode = (UInt16)MC8041P.Nmc_ReadReg2(id, icNo, (AXIS)Enum.Parse(typeof(AXIS), axis.ToString().ToUpper()));

			if (errorCode == 0) return Nova8BWrap.MMC_HIGH_LEVEL;
			else if (errorCode == 0) return Nova8BWrap.MMC_LOW_LEVEL;
			else if (errorCode == 0) return Nova8BWrap.MMC_HIGH_LEVEL;
			else if (errorCode == 0) return Nova8BWrap.MMC_LOW_LEVEL;

			return Nova8BWrap.MMC_OK; // 에러코드
		}

		#endregion ------

		#region === Error Handling ===


		/// <summary>
		/// checks if the PMC command return value
		///  - if not MMC_OK then throws PmcException
		/// </summary>
		/// <param name="exId"></param>
		/// <param name="status"></param>
		protected static void checkNovaStatus(PmcExceptionId exId, ushort status, ushort address)
		{
			checkNovaStatus(exId, status, address, NmcAxis.None);
		}
		private static void checkNovaStatus(PmcExceptionId exId, UInt16 status, ushort address, NmcAxis NovaAxis)
		{
			if (status != Nova8BWrap.MMC_OK)
			{
				PmcException pex = null;
				try
				{
					pex = new PmcException(exId);
					pex.Data["Nova Address"] = address;
					pex.Data["Nova Axis"] = NovaAxis.ToString();

					if (NovaAxis != NmcAxis.None)
					{
						UInt16 detailStatus = Nova8BWrap.MMC_FALSE;

						//detailStatus = Pmc4BWrap.pmc4bpci_flag_error(address, (UInt16)pmcAxis); 
						//2017.06.22 Nova용 구현 필요
						//Nova8B_flag_error(address, )
						pex.Data["Asix Error Flag"] = detailStatus;

						//detailStatus = Pmc4BWrap.pmc4bpci_error();
						//2017.06.22 Nova용 구현 필요
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
		private void checkAxis(NmcAxis axis)
		{
			NmcAxis[] axes = MyEnum<NmcAxis>.ValueArray;

			for (int i = 1; i < axes.Length; i++)//0==None
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
		private const NovaMcAddress DefaultNovaAddress = NovaMcAddress.PMC15;

		//생성자
		public Nova4B() : this(McId.None) { }

		public Nova4B(McId id, string name = DefaultName)
			: base(id, name)
		{
			//PmcAddress
			NovaAddress = DefaultNovaAddress;

			//AxisMap
			mAxisMap = new Dictionary<McAxis, NmcAxis>();

			//global cancel flag
			initGlobalCancelFlag();

		}

		#endregion ===



		#region ===== 장치 구성 - 사용자 설정 =====
		//instance data = 설정파일에 저장, 사용자 설정 UI에서 선택

		/// <summary>
		/// PMC-4B-PC 카드에 설정된 주소 0~15
		/// </summary>
		UInt16 mNovaAddress;
		public NovaMcAddress NovaAddress
		{
			get { return (NovaMcAddress)mNovaAddress; }
			set { mNovaAddress = (ushort)value; }
		}

		/// <summary>
		/// MC 의 McAxis와 PMC4B의 PmcAxis(실제값)간의 연결
		/// - {X1 ~ X4} <--> {X, Y, Z, U} 
		/// </summary>
		private Dictionary<McAxis, NmcAxis> mAxisMap;

		private NmcAxis getNovaAxis(McAxis mcAxis)
		{
			return mAxisMap[mcAxis];
		}


		#region ----- IConfig -----

		override public DeviceConfig buildConfig()
		{
			base.buildConfig();

			//PmcAddress
			mConfig.Add("NovaAddress", this.NovaAddress);

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
			mNovaAddress = (ushort)mConfig.Get<NovaMcAddress>("PmcAddress");

			//AxisMap
			mAxisMap = mConfig["AxisMap"].Unpack<McAxis, NmcAxis>();

			mIsConfigSet = true;
		}

		#endregion


		#endregion ===




		#region === Open & Close ===

		static Dictionary<ushort, int> mOpenCounter;
		static Nova4B()
		{
			mOpenCounter = new Dictionary<ushort, int>();
		}
		static void openAddress(ushort address)
		{
			if (!mOpenCounter.ContainsKey(address)) mOpenCounter.Add(address, 0);
			int counter = mOpenCounter[address];

			if (counter == 0)
			{
				var status = Nova4BWrap.MC8041P.Nmc_Open(address, false);
				MC8041P.Nmc_Reset(address, 0);

				MC8041P.Nmc_WriteReg1(address, 0, AXIS.ALL, 0x0000);

				// HLMT+, HLMT- 설정
				MC8041P.Nmc_WriteReg2(address, 0, AXIS.ALL, 0x0018);

				// WR3 입력 신호의 전 필터 설정 및 입력 신호 지연 010 => 512μs -> 0.5msec
				MC8041P.Nmc_WriteReg3(address, 0, AXIS.ALL, 0x4F00);

				// 일반 출력신호 값 설정
				MC8041P.Nmc_WriteReg4(address, 0, 0x0000);

				// 범용 출력신호 활성여부 설정
				//MC8000P.Nmc_WriteReg5(address, 0, 0x0000);
				//MC8000P.Nmc_WriteReg5(address, 1, 0x0000);
				//MC8041P.Nmc_WriteReg5(address, 0, 0xffff);

				// 
				MC8041P.Nmc_AccOfst(address, 0, AXIS.ALL, 0);

				//checkPmcStatus(PmcExceptionId.Open, status, address);
			}
			mOpenCounter[address] = counter + 1;

		}
		static void closeAddress(ushort address)
		{
			if (!mOpenCounter.ContainsKey(address))
				throw new Exception($"등록되지 않은 Nova8B({address})를 닫으려 했습니다.");

			int counter = mOpenCounter[address];
			if (counter == 0) return;
			//throw new Exception($"닫힌 PMC4B({address})를 닫으려 했습니다.");

			if (counter == 1)
			{
				//UInt16 status = Pmc4BWrap.pmc4bpci_close(address);
				// 2017.06.23
				MC8041P.Nmc_Close(address);
				//checkPmcStatus(PmcExceptionId.Open, status, address);
				mOpenCounter[address] = 0;
			}
		}



		//Open
		protected override void performOpen()
		{
			//check the address if opened
			openAddress(mNovaAddress);

			//specific data of PMC4B
			foreach (McAxis mcAxis in mAxisMap.Keys)
			{
				setDriveMode(mcAxis);

				//load last position value
				StageBase stage = this.getStage(mcAxis);

				stage.loadDynamicData();

				this.writePosition(mcAxis, stage.LastPositionμm);
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
			if (!mIsOpen) return;

			this.stop();
			//while(this.isMoving()) Thread.Sleep(100);

			//save current position to config file
			Dictionary<McAxis, double> pos = this.getPosition();
			foreach (McAxis mcAxis in pos.Keys)
			{
				StageBase stage = this.getStage(mcAxis);
				stage.LastPositionμm = pos[mcAxis];

				//save to where???
				stage.saveDynamicData();
				//stage.buildConfig().SaveConfig();
			}

			//call close
			closeAddress(mNovaAddress);

		}

		#endregion ===



		#region === Setup ===



		private const NovaMmcModeConfig DefaultMode = NovaMmcModeConfig.LMT_SoftStop;

		//WR2 mode register
		private void setDriveMode(McAxis mcAxis)
		{

			// 감감속
			//NovaMmcModeConfig wData = DefaultMode;
			NovaMmcModeConfig wData = 0;
			NmcAxis axis = getNovaAxis(mcAxis);
			StageBase stage = getStage(mcAxis);
			UInt16 icNo = 0;

			// D3, D4
			if (stage.SensorActiveLevel == StageSensorActiveLevel.ActiveHigh)
			{
				//wData &= ~(NovaMmcModeConfig.LMTP_ActiveHigh | NovaMmcModeConfig.LMTN_ActiveHigh);
				wData |= NovaMmcModeConfig.LMTP_ActiveHigh | NovaMmcModeConfig.LMTN_ActiveHigh;
			}

			// D6 // 2Pulse => +, - Direction, 1Pulse => Only + Direction ...
			if (stage.DrivePulseMode == StageDrivePulseMode.PulseAndDir)
				wData |= NovaMmcModeConfig.Mode_PulseAndDir;

			// D8
			if (stage.DirPulseLevel == StageDirectionPulseLevel.PositiveHigh)
				wData |= NovaMmcModeConfig.Dir_PositiveHigh;

			Nova4BWrap.MC8041P.Nmc_WriteReg2(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), axis.ToString().ToUpper()), (UInt16)wData);
		}

		/// <summary>
		/// Enables 'Hw Limit' signal for all Axes
		/// </summary>
		private void setHwLimit()
		{
			//Enable HW Limit signal
			ushort wData = Nova8BWrap.WR2_LMT_HLMTP | Nova8BWrap.WR2_LMT_HLMTM;
			// HLMT+, HLMT- 설정
			MC8041P.Nmc_WriteReg2(this.mNovaAddress, 0, AXIS.ALL, wData);
		}


		//WR1 : sensor config
		private void setSensorConfig(McAxis mcAxis, McSensorConfig sensor)
		{
			uint enableFlag = 0;

			NmcAxis axis = getNovaAxis(mcAxis);
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());

			checkAxis(axis);

			StageBase stage = getStage(mcAxis);
			uint flag = enableFlag;

			//if (stage.SensorActiveLevel == StageSensorActiveLevel.ActiveHigh)
			//	flag |= levelFlag;  // 0x0010 | 0x0100
			//else flag &= ~levelFlag;// 0x0010 & 0x1011 //ActiveLow


			flag = (uint)stage.DirPulseLevel | (uint)stage.DrivePulseMode | (uint)stage.SensorActiveLevel;
			if (stage.SensorActiveLevel == StageSensorActiveLevel.ActiveLow)
			{
				flag |= WR2_LMT_HLMTP | WR2_LMT_HLMTM;
			}

			//write to PMC
			//setAxis(axis);
			// ushort status = Pmc4BWrap.pmc4bpci_wwr1(this.mPmcAddress, (ushort)axis, (ushort)enableFlag);

			Nova4BWrap.MC8041P.Nmc_WriteReg2(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), axis.ToString().ToUpper()), (UInt16)flag);
			UInt16 status = Nova8BWrap.MMC_OK;

			checkNovaStatus(PmcExceptionId.setNORG, status, mNovaAddress, axis);
		}



		#endregion



		#region === Drive Info ===


		#region getPosition, resetPosition

		public override Dictionary<McAxis, double> getPosition(McAxis mcAxis = McAxis.All)
		{
			Dictionary<McAxis, double> dic = new Dictionary<McAxis, double>();

			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
			foreach (McAxis axis in effectiveAxes.Keys)
			{
				dic.Add(axis, readPosition(axis));
			}

			return dic;
		}

		private double readPosition(McAxis mcAxis)
		{
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);

			// 논리 위치 읽어 옴
			int pos = MC8041P.Nmc_ReadLp(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()));

			NovaMcErrorStatus isError = readError(mcAxis);
			//if (isError != NovaMcErrorStatus.TRUE) checkPmcStatus(PmcExceptionId.Read, (ushort)isError, mPmcAddress, axis);
			//this.mAxisMap;

			//************************************
			// 내부 -> 외부로 변환후 리턴
			//************************************
			StageBase stage = this.getStage(mcAxis);

			double factor = 1 / stage.UnitToInternal;
			var coord_um = pos * factor;

			//실제축 & 논리축간 좌표변환
			var logic_coord_um = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength - coord_um : coord_um;
			return logic_coord_um;
		}

		//McAxis mcAxis = McAxis.All

		public override void resetPosition(McAxis mcAxis = McAxis.All)
		{
			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

			foreach (McAxis axis in effectiveAxes.Keys)
			{
				mGlobalCancelFlag[axis] = false;

				StageBase stage = getStage(axis);
				stage.LastOriginDate = DateTime.Now;

				// 2019.01.25 by DrBAE
				// new coordnate to write
				var coord = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength : 0;
				writePosition(axis, coord);
				stage.LastPositionμm = coord;
			}
		}
		
		private void writePosition(McAxis mcAxis, double newPosition)
		{
			StageBase stage = this.getStage(mcAxis);

			//실제축 & 논리축간 좌표변환
			var coord_um = stage.OriginPolarity == StageOriginPolarity.PosLimit ? stage.StrokeLength - newPosition : newPosition;

			//************************************
			// 외부 - 내부로 변환후 리턴
			//************************************
			int internalPosition = (int)(coord_um * stage.UnitToInternal);

			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);
			MC8041P.Nmc_Lp(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), (UInt16)newPosition);
		}


		#endregion


		#region isMoving

		protected override bool performIsMoving(McAxis mcAxis = McAxis.All)
		{
			bool driving = false;

			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

			if (mcAxis == McAxis.All) foreach (McAxis a in effectiveAxes.Keys) driving |= readIsDrive(mcAxis);
			else if (!effectiveAxes.ContainsKey(mcAxis)) return false;
			else driving = readIsDrive(mcAxis);

			return driving;
		}

		/// <summary>
		/// return true if the axis is driving
		/// </summary>
		/// <param name="axis"></param>
		/// <returns></returns>
		private bool readIsDrive(McAxis axis)
		{
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), axis.ToString()); // X5
			NmcAxis novaAxis = getNovaAxis(axis);
			UInt16 status = (UInt16)MC8041P.Nmc_GetDriveStatus(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()));
			return (status >= Nova8BWrap.MMC_TRUE);
		}
		#endregion


		#region Drive Status

		//PCM4B _error()
		private NovaMcErrorStatus readError(McAxis mcAxis)
		{
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);

			//MC8000P.Nmc_ReadReg0(this.mPmcAddress, this.mAxisMap, int IcNo);
			return (NovaMcErrorStatus.TRUE);// Pmc4BWrap.pmc4bpci_error();
		}
		//pmc4b _is_error()
		private bool readIsError(McAxis mcAxis)
		{
			return (ushort)NovaMcErrorStatus.TRUE == ushort.MaxValue;// Pmc4BWrap.pmc4bpci_is_error(this.mPmcAddress, (ushort)axis);

		}
		//pmc4b _is_stop()
		private bool readIsStop(McAxis mcAxis)
		{
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);

			return !((MC8041P.Nmc_GetDriveStatus(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper())) >= (UInt16)NovaMcErrorStatus.TRUE));

			//return (ushort)NovaMcErrorStatus.TRUE == ushort.MaxValue;// Pmc4BWrap.pmc4bpci_is_stop(this.mPmcAddress, (ushort)axis);
		}
		//pmc4b _is_flag_in0 ~ connected to NORG signal of state
		private bool readFlagNORG()
		{
			return (ushort)NovaMcErrorStatus.TRUE == ushort.MaxValue;// Pmc4BWrap.pmc4bpci_flag_in0(this.mPmcAddress);
		}
		//pmc4b _is_flag_in1
		private bool readFlagORG()
		{
			return (ushort)NovaMcErrorStatus.TRUE == ushort.MaxValue;// Pmc4BWrap.pmc4bpci_flag_in1(this.mPmcAddress);
		}

		//pmc4bpci_flag_limitplus
		private bool readFlagLMTP(McAxis mcAxis)
		{
			//Pmc4BWrap.pmc4bpci_set_axis(this.mPmcAddress, (ushort)axis);
			//return (ushort)NovaMcErrorStatus.TRUE == ushort.MaxValue;//Pmc4BWrap.pmc4bpci_flag_limitplus(this.mPmcAddress);
			// 2017.06.22 Jeon
			//MC8000P.Nmc_Pulse(mPmcAddress, (int)IC.B, AXIS.ALL, (int)dx0 * -1);
			//MC8000P.Nmc_Command(mPmcAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), pmcAxis.ToString().ToUpper()), CMD.CMD_F_DRV_M); // 명령:34 X 축 '-'방향 정량 드라이브

			// Ic chip Number Check
			//StageBase stage = this.getStage(mcAxis);
			//mcAxis = McAxis.AY;

			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);

			return (MC8041P.Nmc_ReadReg2(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper())) & 0x0004) >= (UInt16)NovaMcErrorStatus.TRUE;
			// Jeon 2017.09.01
			//return (MC8000P.Nmc_ReadReg2(this.mNovaAddress, icNo, AXIS.Y) & 0x0004) >= (UInt16)NovaMcErrorStatus.TRUE;
		}
		//pmc4bpci_flag_limitplus
		private bool readFlagLMTN(McAxis mcAxis)
		{
			// 2017.06.22 Jeon
			//mcAxis = McAxis.AY;
			UInt16 icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), mcAxis.ToString());
			NmcAxis novaAxis = getNovaAxis(mcAxis);
			return (MC8041P.Nmc_ReadReg2(this.mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString())) & 0x0008) >= (UInt16)NovaMcErrorStatus.TRUE;
		}



		public override void readMotionStatus(McMotionParam param)
		{
			//position
			param.x = readPosition(param.axis);

			NmcAxis axis = mAxisMap[param.axis];
			param.isError = readIsError(param.axis);
			param.isStop = readIsStop(param.axis);

			param.isNorg = readFlagNORG();
			param.isOrg = readFlagORG();
			param.isLmtP = readFlagLMTP(param.axis);
			param.isLmtN = readFlagLMTN(param.axis);
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
			NmcAxis x = NmcAxis.None;
			UInt16 icNo = 0;


			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
			foreach (McAxis a in effectiveAxes.Keys)
			{
				//global cancel flag
				mGlobalCancelFlag[a] = true;

				x |= mAxisMap[a];

				// 즉지 정지
				//var mcAxis = effectiveAxes[x];
				icNo = (UInt16)(NovaMcIcNo)Enum.Parse(typeof(NovaMcIcNo), a.ToString());
			}

			// CMD_STOP_SUDDEN:축 드라이브 즉시 정지, CMD_STOP_DEC: 감속정지
			MC8041P.Nmc_Command(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), x.ToString().ToUpper().ToUpper()), CMD.CMD_STOP_SUDDEN);
		}
		public Enum NovaMcIcNo { get; private set; }

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

			//read status
			readMotionStatus(param);
		}

		private void writeMove(McMotionParam param)
		{
			var status = NovaMcErrorStatus.FALSE;

			//check if this MC open
			checkOpen(PmcExceptionId.Move);

			// 이미 Move 중이면, Move 실행하지 않음
			if (readIsDrive(param.axis)) return;

			//check if the axis connected
			NmcAxis novaAxis = getNovaAxis(param.axis);

			checkAxis(novaAxis);
			
			UInt16 icNo = 0;	// Ic chip 

			//Set Sensor
			//McSensorConfig sensor = McSensorConfig.None;
			//this.setSensorConfig(param.axis, sensor);

			//stage info
			StageBase stage = getStage(param.axis);

			//배율 조정
			int R = PmcHwInfo.R(stage.PulseDivider);//400000
			MC8041P.Nmc_Range(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), R);


			//이동방식에 따른 유효이동거리 계산
			double dx0 = param.dx0;
			if (param.moveType == McMoveType.MoveTo) dx0 -= readPosition(param.axis);

			//내부 최소단위로 반올림
			dx0 = Math.Round(dx0 * stage.UnitToInternal);

			//move
			//setDriveMode(param.axis);
			//MC8041P.Nmc_Jerk(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), NovaMmcAccel.aac);
			//MC8041P.Nmc_DJerk(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), NovaMmcAccel.ddc);
			//MC8041P.Nmc_Acc(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), NovaMmcAccel.ac);
			//MC8041P.Nmc_Dec(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), NovaMmcAccel.dc);
			//MC8041P.Nmc_StartSpd(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), 100);
			MC8041P.Nmc_Speed(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), (UInt16)param.speed);

			//실제축 & 논리축간 좌표변환
			var trans = stage.OriginPolarity == StageOriginPolarity.PosLimit;
			var positiveDir = (trans && dx0 < 0) || (!trans && dx0 > 0);
			dx0 = Math.Abs(dx0);

			if (positiveDir)
			{
				MC8041P.Nmc_Pulse(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), (int)dx0);
				MC8041P.Nmc_Command(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), CMD.CMD_F_DRV_P); // 명령:33 X 축 '+'방향 정량 드라이브
			}
			else
			{
				MC8041P.Nmc_Pulse(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), (int)dx0);
				MC8041P.Nmc_Command(mNovaAddress, icNo, (AXIS)Enum.Parse(typeof(AXIS), novaAxis.ToString().ToUpper()), CMD.CMD_F_DRV_M); // 명령:34 X 축 '-'방향 정량 드라이브
			}
			status = NovaMcErrorStatus.TRUE;
			checkNovaStatus(PmcExceptionId.Move, (UInt16)status, mNovaAddress, novaAxis);
		}


		#endregion ---


		#region --- Zero & Home ---
		//***
		public override void moveToLogicalOrigin(McAxis mcAxis = McAxis.All)
		{
			this.stop(mcAxis);

			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);
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
			// 2017.07.03 Jeon
			// 이동 중이면 중복 실행하지 않음
			if (readIsDrive(mcAxis)) return;

			this.stop(mcAxis);
			//this.stopAsync = false;

			Dictionary<McAxis, NmcAxis> effectiveAxes = this.getEffectiveAxis(mcAxis);

			foreach (McAxis ax in effectiveAxes.Keys)
			{
				StageBase stage = getStage(ax);

				McMotionParam param = new McMotionParam(this, ax, stage.StrokeLength);
				param.dx0 *= stage.UnitToPolar;//μm -> 외부 단위로 변환
				param.moveType = McMoveType.MoveAs;

				//calc home direction
				if (stage.OriginPolarity == StageOriginPolarity.NegLimit) param.dx0 *= -1;

				//param.senseORG = true;
				this.move(param);

			}//foreach

		}

		public override void moveToStrokeCenter(McAxis mcAxis = McAxis.All)
		{

			//move to origin
			this.moveToOrigin(mcAxis);

			Dictionary<McAxis, NmcAxis> dicEff = getEffectiveAxis(mcAxis);

			//move as half of stroke
			performAfterStopAsync(dicEff, (axis) =>
			{
				StageBase stage = getStage(axis);

				McMotionParam param = new McMotionParam(this, axis, stage.StrokeLength / 2);
				param.dx0 *= stage.UnitToPolar;//μm -> 외부 단위로 변환
				param.moveType = McMoveType.MoveAs;

				//find oppisite origin direction
				if (stage.OriginPolarity == StageOriginPolarity.PosLimit) param.dx0 *= -1;

				//move~
				this.move(param);
			});
		}




		/// <summary>
		/// 주어진 McAxis에서 유효한 - MC에 연결되어 있는 - 축의 목록을 구한다.
		/// </summary>
		/// <param name="mcAxis"></param>
		/// <returns></returns>
		private Dictionary<McAxis, NmcAxis> getEffectiveAxis(McAxis mcAxis = McAxis.All)
		{
			McAxis[] axes = MyEnum<McAxis>.ValueArray;
			Dictionary<McAxis, NmcAxis> dic = new Dictionary<McAxis, NmcAxis>();

			foreach (McAxis a in axes)
			{
				if (a == McAxis.None) continue;
				if (a == McAxis.All) continue;
				if (!mcAxis.HasFlag(a)) continue;

				if (mAxisMap.ContainsKey(a))
				{
					dic.Add(a, mAxisMap[a]);
				}
			}
			return dic;
		}

		private delegate void ActionAfterStop(McAxis axis);
		private Dictionary<McAxis, bool> mGlobalCancelFlag;
		private void initGlobalCancelFlag()
		{
			if (mGlobalCancelFlag == null) mGlobalCancelFlag = new Dictionary<McAxis, bool>();
			mGlobalCancelFlag.Clear();

			McAxis[] axisList = (McAxis[])Enum.GetValues(typeof(McAxis));
			foreach (McAxis axis in axisList)
			{
				if (axis == McAxis.None) continue;
				if (axis == McAxis.All) continue;
				mGlobalCancelFlag.Add(axis, false);
			}
		}
		//private void setGlobalCancelFlag(McAxis axis, bool )

		private async void performAfterStopAsync(Dictionary<McAxis, NmcAxis> axisDic, ActionAfterStop action)
		{
			List<McAxis> stopList = new List<McAxis>();

			while (axisDic.Count > 0)
			{
				await Task.Delay(1);

				//find stopped axis
				stopList.Clear();
				foreach (McAxis mcAxis in axisDic.Keys)
				{
					// 2017.06.26
					// 파라미터 물리적 위치로 바꿀것
					// readIsDrive(axisDic[mcAxis])
					//if (mGlobalCancelFlag[mcAxis] || !readIsDrive(axisDic[mcAxis])) stopList.Add(mcAxis);
					if (mGlobalCancelFlag[mcAxis] || !readIsDrive(mcAxis)) stopList.Add(mcAxis);
				}

				//perform action on the stopped axis
				foreach (McAxis mcAxis in stopList)
				{
					if (!mGlobalCancelFlag[mcAxis])
					{
						action(mcAxis);//perform the action
						axisDic.Remove(mcAxis);
					}
				}
			}
		}




		#endregion
		#endregion




	}//class

}
