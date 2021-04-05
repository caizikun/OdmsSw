using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{

	#region === Motion Control Data Definition ===

	/// <summary>
	/// MC간 구별하는 ID - 설정파일에 저장
	/// </summary>
	public enum McId { None = 0, MC1 = 1, MC2, MC3, MC4, MC5, MC6, MC7, MC8, MC9 }

	/// <summary>
	/// MC의 축 
	/// </summary>
	[Flags]
	public enum McAxis
	{
		None = 0,
		X1 = 1, X2 = 2, X3 = 4, X4 = 8, X5 = 16, X6 = 32, X7 = 64, X8 = 128,
		X9 = 256, X10 = 512, X11 = 1024, X12 = 2048, X13 = 4096, X14 = 8192, X15 = 16384, X16 = 32768,
		All = X1 | X2 | X3 | X4 | X5 | X6 | X7 | X8 | X9 | X10 | X11 | X12 | X13 | X14 | X15 | X16


		// ******************************************************************************
		// ******************************************************************************		
		//
		//    수 정 하 지    말 것!!!
		//
		//    수정시 현재 Autonics MC 사용중인 모든 장비의 설정파일을 바꿔야 함
		//
		// ******************************************************************************
		// ******************************************************************************

		//None = 0x0, AX = 0x01, AY = 0x02, AZ = 0x04, AU = 0x08, BX = 0x10, BY = 0x20, BZ = 0x40, BU = 0x80,
		//All = AX | AY | AU | AZ | BX | BY | BZ | BU
	}

	public enum McSpeed { Fast = 0, Mid = 1, Slow = 2 }

	/// <summary>
	/// 이동 방향 : + -
	/// </summary>
	public enum McDriveDir
	{
		Neg = -1, Pos = +1
	}

	/// <summary>
	/// 스테이지 모터의 회전 방향
	/// </summary>
	public enum McPolarDir
	{
		CW = -1, CCW = +1
	}


	/// <summary>
	/// 이동명령시 지정하는 수치의 의미 - 상대거리 이동, 절대좌표 이동
	/// </summary>
	public enum McMoveType
	{
		/// <summary>
		/// 지정한 수치 == 이동거리
		/// </summary>
		MoveAs,

		/// <summary>
		/// 지정한 수치 == 좌표
		/// </summary>
		MoveTo
	}


	#endregion


	/// <summary>
	/// 이동 결과 상태
	/// </summary>
	[Flags]
	public enum McMoveStatus : ushort
	{
		None = 0x00, 
		Complete = 0x01, Stop = 0x02, Error = 0x04, 
		NORG = 0x08, ORG = 0x10, LMTP = 0x20, LMTN = 0x40
	}

	/// <summary>
	/// 감지할 센서를 지정
	/// </summary>
	[Flags]
	public enum McSensorConfig : ushort
	{
		None = 0x00, LMTP = 0x01, LMTN = 0x02, NORG = 0x04, ORG = 0x08
	} 


    public class McMotionParam : IMcMotionParam
    {

        #region === Class Framework ===

		public IMc mMc;

		public McMotionParam(IMc mc, McAxis axis) : this(mc, axis, 0) {}

		public McMotionParam(IMc mc, McAxis axis, double umLength)
		{
			this.mMc = mc;
			this.axis = axis;
			this.speed = mMc.getSpeedValue(axis, McSpeed.Fast);
			this._dx0 = umLength;
			this._sensor = McSensorConfig.LMTN | McSensorConfig.LMTP;
		}
		public McMotionParam(IMc mc, McAxis axis, double umLength, int speed) 
			: this(mc, axis, umLength)
		{
			this.speed = speed;
			//update();
		}	


		public override string ToString()
		{
			return string.Format("({6}.{5}) {0,6} → {1,6} : {2,6} / {3,6}μm @ {4}", x0, x, dx, dx0,
				_status.ToString(), axis, mMc.Id);
		}

        #endregion

        

        #region === Coordinates ===

        public McAxis axis { get; set; }

        public int speed { get; set; }//

		public McMoveType moveType { get; set; }

        double _x0, _x, _dx0, _dx;

        //initial position
		public double x0
        {
            get { return _x0; }
            set 
            { 
                _x0 = value;
                update();
            }
        }
        //final position
		public double x 
        { 
            get { return _x; }
            set 
            { 
                _x = value;
                update();
            }
        }

		/// <summary>
		/// required move length
		/// </summary>
		public double dx0 
        { 
            get { return _dx0; }
            set
            { 
                _dx0 = value;
                update();
            }
        }
        
        //effective displacement
		public double dx { get { return _dx; } }

        
        #endregion



        #region === Sensor Settings ===

        McSensorConfig _sensor;
        public McSensorConfig sensor
        {
            get { return this._sensor; }
            set{ this._sensor = value; }
        }

        public bool senseNORG
        {
            get { return this._sensor.HasFlag(McSensorConfig.NORG); }
            set 
            {
                if (value)  this._sensor |= McSensorConfig.NORG;
                else        this._sensor &= ~McSensorConfig.NORG;
            }
        }

        public bool senseORG
        {
            get { return this._sensor.HasFlag(McSensorConfig.ORG); }
            set
            {
                if (value) this._sensor |= McSensorConfig.ORG;
                else this._sensor &= ~McSensorConfig.ORG;
            }
        }
        

        #endregion



        #region === Result Stauts ===

		bool _error,	//pcm4b_is_error
			_stop,		//pcm4b_is_stop
			_complete,	//dx = dx0 지정한 거리만큼 이동 완료
			_norg,		//NORG 시그널 감지
			_org,		//ORG 신호 감지
			_lmtp,		//LMT+
			_lmtn;		//LMT-

        McMoveStatus _status = McMoveStatus.None;
        public McMoveStatus status { get { return _status; } }

        //error
        public bool isError 
        { 
            get { return _error; }
            set { _error = value; update(); }
        }

        //stop
        public bool isStop
        { 
            get { return _stop; }
            set { _stop = value; update(); }
        }

        //complete
        public bool isComplete { get { return _complete; } }

        //NORG
        public bool isNorg 
        { 
            get { return _norg; }
            set { _norg = value; update(); }
        }

        //Org
        public bool isOrg 
        { 
            get { return _org; }
            set { _org = value; update(); }
        }

		//Lmt+
		public bool isLmtP
		{
			get { return _lmtp; }
			set { _lmtp = value; update(); }
		}
		//Lmt-
		public bool isLmtN
		{
			get { return _lmtn; }
			set { _lmtn = value; update(); }
		}

        #endregion

        

        #region === Common Methods ===

		public void resetStatus()
		{
			this._status = McMoveStatus.None;
		}

        public void resetAll()
        {
			axis = McAxis.None;

            _x = _x0 = _dx = _dx0 = 0;
			speed = 0;// mMc.getSpeedValue(McSpeed.Fast);

            _error = _stop = _complete = _norg = _org = _lmtp = _lmtn = false;
            _status = McMoveStatus.None;
        }
		public void initCoord(double x0, double x)
        {
            _x0 = x0;
            _x = x;
        }

        void update()
        {
            //dx
            _dx = _x - _x0;

            //complete
            _complete = Math.Abs(_dx) >= Math.Abs(_dx0);
            
            //status
            _status |= _complete ? McMoveStatus.Complete : 0;
            _status |= _error ? McMoveStatus.Error : 0;
            _status |= _stop ? McMoveStatus.Stop : 0;
            _status |= _norg ? McMoveStatus.NORG : 0;
            _status |= _org ? McMoveStatus.ORG : 0;
			_status |= _lmtp ? McMoveStatus.LMTP : 0;
			_status |= _lmtn ? McMoveStatus.LMTN : 0;

        }



        #endregion



    }


}
