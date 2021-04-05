using Free302.TnM.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
	public class SfacAligner : Istage
	{
		SurugaAligner mCenter;      //Suruga : 센터		[X축]
		CenterAligner mCamera;		//Nova   : 카메라	[Y축]

		public SfacAligner(int surugaComport, string configFile)
		{
			mCamera = new CenterAligner();
			mCamera.Open();

			mCenter = new SurugaAligner(2);
			mCenter.InitMc(new int[] { surugaComport }, configFile);
		}



		public int AXIS_X => mCamera.AXIS_X;

		public int AXIS_Y => mCamera.AXIS_Y;

		public int AXIS_Z => mCamera.AXIS_Z;

		public int AXIS_ThetaX => mCamera.AXIS_ThetaX;

		public int AXIS_ThetaY => mCamera.AXIS_ThetaY;

		public int AXIS_ThetaZ => mCamera.AXIS_ThetaZ;

		public int AXIS_ALL => mCamera.AXIS_ALL;

		public int AXIS_U => mCamera.AXIS_U;

		public int AXIS_V => mCamera.AXIS_V;

		public int AXIS_W => mCamera.AXIS_W;

		public int AXIS_TX => mCamera.AXIS_TX;

		public int AXIS_TY => mCamera.AXIS_TY;

		public int AXIS_TZ => mCamera.AXIS_TZ;

		public int MOVESPEED_SLOW => mCamera.MOVESPEED_SLOW;

		public int MOVESPEED_MID => mCamera.MOVESPEED_MID;

		public int MOVESPEED_FAST => mCamera.MOVESPEED_FAST;

		public int DIRECTION_MINUS => mCamera.DIRECTION_MINUS;

		public int DIRECTION_PLUS => mCamera.DIRECTION_PLUS;

		public int stageNo => mCamera.stageNo;

		public bool AbsMove(int _axis, double _pos, int _speed)
		{
			if (_axis == AXIS_X) return mCenter.AbsMove(_axis, _pos, _speed);
			else return mCamera.AbsMove(_axis, _pos, _speed);
		}

		public bool AbsMove(int _axis, double _pos)
		{
			if (_axis == AXIS_X) return mCenter.AbsMove(_axis, _pos);
			else return mCamera.AbsMove(_axis, _pos);
		}

		public CStageAbsPos GetAbsPositions()
		{
			var pos = new CStageAbsPos();
			var centerPos = mCenter.GetAxisAbsPos(AXIS_X);
			var cameraPos = mCamera.GetAxisAbsPos(AXIS_Y);

			pos.SetValue(new double[] { centerPos, cameraPos });
			return pos;
		}

		public double GetAxisAbsPos(int _axis)
		{
			if (_axis == AXIS_X) return mCenter.GetAxisAbsPos(_axis);
			else return mCamera.GetAxisAbsPos(_axis);

		}

		public bool Homing(int _axis)
		{
			if (_axis == AXIS_X) return mCenter.Homing(_axis);
			else return mCamera.Homing(_axis);
		}

		public bool Homing()
		{
			Homing(AXIS_X);
			Homing(AXIS_Y);
			return true;
		}

		public bool IsConnectedOK()
		{
			return true;
		}

		public bool IsMovingOK()
		{
			var center = IsMovingOK(AXIS_X);
			var camera = IsMovingOK(AXIS_Y);

			if (center && camera) return true;
			else return false;
		}

		public bool IsMovingOK(int _axis)
		{
			if (_axis == AXIS_X) return mCenter.IsMovingOK(_axis);
			else return mCamera.IsMovingOK(_axis);
		}

		public bool RelMove(int _axis, double _dist, int _speed)
		{
			if (_axis == AXIS_X) return mCenter.RelMove(_axis, _dist, _speed);
			else return mCamera.RelMove(_axis, _dist, _speed);
		}

		public bool RelMove(int _axis, double _dist)
		{
			if (_axis == AXIS_X) return mCenter.RelMove(_axis, _dist);
			else return mCamera.RelMove(_axis, _dist);
		}

		public void StopMove(int _axis)
		{
			if (_axis == AXIS_X) mCenter.StopMove(_axis);
			else mCamera.StopMove(_axis);
		}

		public void StopMove()
		{
			mCenter.StopMove();
			mCamera.StopMove();
		}

		public void WaitForIdle(int _axis)
		{
			if (_axis == AXIS_X) mCenter.WaitForIdle(_axis);
			else mCamera.WaitForIdle(_axis);
		}

		public void WaitForIdle()
		{
			mCenter.WaitForIdle();
			mCamera.WaitForIdle();
		}

		public bool Zeroing(int _axis)
		{
			if (_axis == AXIS_X) return mCenter.Zeroing(_axis);
			else return mCamera.Zeroing(_axis);
		}

		public bool Zeroing()
		{
			Zeroing(AXIS_X);
			Zeroing(AXIS_Y);
			return true;
		}
	}
}
