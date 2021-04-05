using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TnM.DeviceFx;

namespace Neon.Aligner
{
	public class SfacSerial : SerialBase, IDispSensor, IairValvController
	{

		#region ==== Sfac serial 통신  common ====

		bool mIsSerialOpen = false;

		public override void Open()
		{
			try
			{
				base.Open();
				CloseValve();
				mIsSerialOpen = true;
			}
			catch 
			{
				mIsSerialOpen = false;
			}
		}


		public void Config()
		{
			var config = new SerialConfig("COM1", 115200)
			{
				EndOfMessage = '\r'
			};
			base.Config(config);
			base.EnableEndOfMessage = true;
		} 

		#endregion



		#region ==== IdistSensor ====

		/// 시리얼 통신 [SFAC 장비의 명령]
		const string CMD_SfacComOpen = "REM ";              // 시리얼 포트를 열고 초기화
		const string CMD_MotorOpen = "MSTART 0";			// Motor Signal 핀을 열고 초기화
		Dictionary<SensorID, string> CMD_Seosor = 
			new Dictionary<SensorID, string> { {SensorID.Left, "SELS 1"}, {SensorID.Right, "SELS 2"} };

		DS2000 mDs200;

		public int PhysicalSensorCount
		{
			get { return mDs200.PhysicalSensorCount; }
		}


		bool mInitDistSensor = false;
		SensorID mCurrentID;

		public bool InitDistanceSensor(Daq daq, int[] aiCh, double[] voltRange, bool daqRSE)
		{
			if (!mIsSerialOpen) return false;

			if (mDs200 == null) mDs200 = new DS2000();
			var result = mDs200.Init(daq, aiCh, voltRange, daqRSE);

			//Sfac Serial Init설정
			write(CMD_SfacComOpen);
			write(CMD_MotorOpen);
			write(CMD_Seosor[SensorID.Left]);

			mInitDistSensor = result;
			return result;
		}

		
		/// <summary>
		/// distance 값을 읽어들인다.
		/// </summary>
		public double ReadDist(SensorID sensorID)
		{
			if (!mIsSerialOpen || !mInitDistSensor) return 0;

			if (mCurrentID != sensorID)
			{
				write(CMD_Seosor[sensorID]);
				mCurrentID = sensorID;
				Thread.Sleep(500);
			}
			return mDs200.ReadDist(sensorID);
		}

		#endregion



		#region ==== iairValvController ====

		Dictionary<int, AirValveState> mValveState = new Dictionary<int, AirValveState>
												{ { (int)AirValveAligner.Input, AirValveState.close },
												  { (int)AirValveAligner.Output, AirValveState.close } };

		public int valveCnt { get { return 2; } }

		public void CloseValve()
		{
			if (!mIsSerialOpen) return;
			CloseValve((int)AirValveAligner.Input);
			CloseValve((int)AirValveAligner.Output);
		}


		public void CloseValve(int _valve)
		{
			if (!mIsSerialOpen) return;
			if (mValveState[_valve] ==  AirValveState.open)
			{
				write($"SETSOL {_valve},{(int)AirValveState.close}");
				mValveState[_valve] = AirValveState.close;
				Thread.Sleep(500);
			}
		}


		public void OpenValve()
		{
			if (!mIsSerialOpen) return;
			OpenValve((int)AirValveAligner.Input);
			OpenValve((int)AirValveAligner.Output);
		}


		public void OpenValve(int _valve)
		{
			if (!mIsSerialOpen) return;
			if (mValveState[_valve] == AirValveState.close)
			{
				write($"SETSOL {_valve},{(int)AirValveState.open}");
				mValveState[_valve] = AirValveState.open;
				Thread.Sleep(500);
			}
		}


		public AirValveState ValveState(int _valve)
		{
			return mValveState[_valve];
		} 

		#endregion
	}


}
