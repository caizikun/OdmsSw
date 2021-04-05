using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Data;


namespace Free302.TnM.Device
{

    /// <summary>
    /// 관리할 모든 장치의 공통 기본 기능 정의
    ///  - GPIB, PCI, USB, RS232 ...
    /// </summary>
    /// <typeparam name="TDeviceId">장치의 아이디로 사용할 데이터 타입</typeparam>
    public abstract class DeviceBase<TDeviceId> : IDevice<TDeviceId>
    {

		#region === Class Framework ===

		public DeviceBase() : this(default(TDeviceId), DefaultName) { }

		public DeviceBase(TDeviceId id, string name = DefaultName)
		{
			mName = name;
			mIsOpen = false;

			mId = id;
			IsIdSet = !id.Equals(default(TDeviceId));//*************

			mConfig = new DeviceConfig(ConfigName);
			mIsConfigSet = false;

			//--- 작업중 ---
			mSetup = new DeviceSetup();//**************************************
		}

        /// <summary>
        /// 장치의 이름, 상태 등을 출력할 때 사용
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} @ ID={1}, Config={2}", mName, mId, this.ConfigName);
		}

		#endregion ===



		#region === 장치 구성 - 사용자 제공 설정 - Before Open ===

		//class name
        private const string DefaultName = "DeviceBase";
        protected string mName;
        /// <summary>
        /// 현 인스턴스의 이름
        /// </summary>
        public virtual string Name
        {
            get { return mName; }
			set 
			{ 
				mName = value;
				mConfig.ConfigName = this.ConfigName;				
			}
        }


        protected TDeviceId mId;
        /// <summary>
        /// 장치 인스턴스를 여러개 생성해서 사용할 경우
        /// 각 인스턴스를 구별할 아이디
        /// </summary>
        public virtual TDeviceId Id
        {
            get 
            {
                if (!IsIdSet)
                {
                    throw new Exception(string.Format("{0} : ID is not setup.", mName));
                }
                return mId; 
            }
            set 
			{ 
				mId = value; 
				IsIdSet = true;
				mConfig.ConfigName = this.ConfigName;
			}
        }

        /// <summary>
        /// 장치의 아이디가 설정되어 있는지 여부
        /// </summary>
		public virtual bool IsIdSet { get; protected set; }


		#region --- IConfig & Config ---


		virtual public DeviceConfig buildConfig()
		{
			if (mConfig == null) mConfig = new DeviceConfig(ConfigName);
			mConfig.Clear();
            //todo: build ...
			return mConfig;
		}

		virtual public void applyConfig(DeviceConfig newConfig)
		{
			mIsConfigSet = false;
            mConfig.Load(newConfig);
			mIsConfigSet = true;
		}

		virtual public string ConfigName { get { return $"{mName}.{mId}"; } }

		//address info - GPBI address or etc
		protected DeviceConfig mConfig;		
		protected bool mIsConfigSet;
        /// <summary>
        /// 설정이 적용 되어 있는지 여부
        /// </summary>
		public virtual bool IsConfigSet { get { return mIsConfigSet; } }

		//error checking - 
		protected void checkConfig(string errorMessage) { if (!mIsConfigSet) { throw new Exception(errorMessage); } }
		protected void checkConfig()
		{
            checkConfig($"{this} :: checkConfig()");
		}


		#endregion

		

        #endregion




		#region === Open & Close ===

		#region Open

		//open
        protected bool mIsOpen;
        /// <summary>
        /// 장치가 열려있는지 여부
        /// </summary>
        public virtual bool IsOpen
        {
            get { return mIsOpen; }
        }

        /// <summary>
        /// 장치를 연결 및 셋업 준비
        /// </summary>
        public virtual void Open()
        {
			checkConfig();

			performOpen();

            mIsOpen = true;
        }

		//Open 작업을 수행하는 메소드
		abstract protected void performOpen();


        //error checking - 
        protected void checkOpen(string errorMessage)
        {
            if (!mIsOpen)
            {
                throw new Exception(errorMessage);
            }
        }
		protected void checkOpen()
        {
			checkOpen(this.ToString());
        }

        #endregion --open


        #region Close

        /// <summary>
        /// 연결 종료
        /// </summary>
        public virtual void Close()
        {
            checkOpen();

			performClose();

            mIsOpen = false;
        }

		//Close 작업을 수행하는 메소드
		abstract protected void performClose();

        #endregion

		#endregion ===




		#region === Setup - Real Device ===


		//config file...?
		//DB table key
		protected DeviceSetup mSetup;
        /// <summary>
        /// 장치의 셋업상태
        /// </summary>
		public DeviceSetup Setup
		{
			get { return this.mSetup; }
		}




		#endregion ===






	}
}
