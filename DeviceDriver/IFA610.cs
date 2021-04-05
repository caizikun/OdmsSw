using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Free302.TnM.Device;
using TnmAligner = Free302.TnM.Device.Aligner;

namespace Neon.Aligner
{
	//public enum IFA_AlignerId { Center, Left, Right, Test }

	public class AlignerBase : TnmAligner, Istage
    {
		IFA_AlignerId alignerId;

		public AlignerBase(IFA_AlignerId alignerId, string name) : base(AlignStageId.AS1, name)
		{
			this.alignerId = alignerId;

			DeviceConfig config = new DeviceConfig(this.ConfigName);
			config.LoadConfig();
            this.applyConfig(config);
        }

        #region === Istage Interface ===


        #region properties

        public int AXIS_X { get { return (int)AlignAxis.X; } }
		public int AXIS_Y { get { return (int)AlignAxis.Y; } }
		public int AXIS_Z { get { return (int)AlignAxis.Z; } }
		public int AXIS_ThetaX { get { return (int)AlignAxis.Tx; } }
		public int AXIS_ThetaY { get { return (int)AlignAxis.Ty; } }
		public int AXIS_ThetaZ { get { return (int)AlignAxis.Tz; } }
		public int AXIS_ALL { get { return (int)AlignAxis.All; } }
		public int AXIS_U { get { return this.AXIS_ThetaX; } }
		public int AXIS_V { get { return this.AXIS_ThetaY; } }
		public int AXIS_W { get { return this.AXIS_ThetaZ; } }
		public int AXIS_TX { get { return AXIS_U; } }
		public int AXIS_TY { get { return AXIS_V; } }
		public int AXIS_TZ { get { return AXIS_W; } }

		public int MOVESPEED_SLOW
		{
			get { throw new Exception("속성 <MOVESPEED_SLOW>는 더이상 사용할 수 없음."); }
		}
		public int MOVESPEED_MID
		{
			get { throw new Exception("속성 <MOVESPEED_MID>는 더이상 사용할 수 없음."); }
		}
		public int MOVESPEED_FAST
		{
			get { throw new Exception("속성 <MOVESPEED_FAST>는 더이상 사용할 수 없음."); }
		}

		public int DIRECTION_MINUS { get { return (int)McDriveDir.Neg; } }
		public int DIRECTION_PLUS { get { return (int)McDriveDir.Pos; } }

		public int stageNo { get { return (int)alignerId; } }


		#endregion


		#region methods

		public bool IsConnectedOK()
		{
			bool isOk = true;
			foreach(AlignerMcTransform trans in mAxisTransMap.Values)
			//foreach (IMc mc in mMcMap.Values)
			{
				isOk &= trans.mc.IsOpen;
			}
			return isOk;
		}

		public bool Zeroing(int _axis)
		{
			AlignerMcTransform ic = mAxisTransMap[(AlignAxis)_axis];
			ic.mc.moveToOrigin(ic.mcAxis);

            WaitForIdle();
            Thread.Sleep(100);
            ic.mc.resetPosition(ic.mcAxis);
            saveDynamicData();

			return true;
		}
		public bool Zeroing()
		{
			foreach (var t in mAxisTransMap.Values) t.mc.moveToOrigin(t.mcAxis);

			WaitForIdle();
			Thread.Sleep(100);
            foreach (var t in mAxisTransMap.Values) t.mc.resetPosition(t.mcAxis);
            saveDynamicData();
			Thread.Sleep(1000);
			return true;
		}

		public bool Homing(int _axis)
		{
			AlignerMcTransform ic = mAxisTransMap[(AlignAxis)_axis];
			ic.mc.moveToStrokeCenter(ic.mcAxis);

            WaitForIdle();
            Thread.Sleep(100);
            saveDynamicData();

            return true;
		}
		public bool Homing()
		{
            foreach(var t in mAxisTransMap.Values) t.mc.moveToStrokeCenter(t.mcAxis);

            WaitForIdle();
			Thread.Sleep(100);
            saveDynamicData();

            return true;
		}

		

		public double GetAxisAbsPos(int _axis)
		{
			var trans = mAxisTransMap[(AlignAxis)_axis];
			var param = new McMotionParam(trans.mc, trans.mcAxis);
			trans.mc.readMotionStatus(param);
			
			//x,y,z축은 좌표 소숫점 첫재짜리.
			//tx 소숫점 3째자리 ex.) 0.003
			//ty,tz 소숫점 4째자리 ex.) 0.0032
			double x = param.x;
			switch(_axis)
			{
				case (int)(AlignAxis.X):
				case (int)(AlignAxis.Y):
				case (int)(AlignAxis.Z):
					x = Math.Round(x, 2);
					break;

				case (int)(AlignAxis.Tx):
					x = Math.Round(x, 3);
					break;

				case (int)(AlignAxis.Ty):
				case (int)(AlignAxis.Tz):
					x = Math.Round(x, 4);
					break;
			}

			return x;
		}

		public CStageAbsPos GetAbsPositions()
		{
            CStageAbsPos pos = new Neon.Aligner.CStageAbsPos();

			foreach (AlignAxis axis in mAxisTransMap.Keys)
			{
				double x = GetAxisAbsPos((int)axis);
				switch (axis)
				{
					case AlignAxis.X: pos.x = x; break;
					case AlignAxis.Y: pos.y = x; break;
					case AlignAxis.Z: pos.z = x; break;
					case AlignAxis.Tx: pos.tx = x; break;
					case AlignAxis.Ty: pos.ty = x; break;
					case AlignAxis.Tz: pos.tz = x; break;
				}
			}
			return pos;
		}

		public bool RelMove(int _axis, double _dist, int _speed)
		{
            var axis = (AlignAxis)_axis;
            AlignerMcTransform trans = mAxisTransMap[axis];
			McMotionParam param = new McMotionParam(trans.mc, trans.mcAxis, _dist, _speed);
			trans.mc.move(param);

            //WaitForIdle(_axis);
            //saveDynamicData(axis);
            waitAndSaveDynamicData(axis);

            return true;
		}
		public bool RelMove(int _axis, double _dist)
		{
            var axis = (AlignAxis)_axis;
            var t = mAxisTransMap[(AlignAxis)_axis];
			var param = new McMotionParam(t.mc, t.mcAxis, _dist);
			t.mc.move(param);

            //WaitForIdle(_axis);
            //saveDynamicData(axis);
            waitAndSaveDynamicData(axis);

            return true;
		}

		public bool AbsMove(int _axis, double _pos, int _speed)
		{
            var axis = (AlignAxis)_axis;
            var t = mAxisTransMap[axis];
			var param = new McMotionParam(t.mc, t.mcAxis, _pos, _speed);
			param.moveType = McMoveType.MoveTo;
			t.mc.move(param);

            //WaitForIdle(_axis);
            //saveDynamicData(axis);
            waitAndSaveDynamicData(axis);

            return true;
		}
		public bool AbsMove(int _axis, double _pos)
		{
            var axis = (AlignAxis)_axis;
            AlignerMcTransform trans = mAxisTransMap[axis];
			McMotionParam param = new McMotionParam(trans.mc, trans.mcAxis, _pos);
			param.moveType = McMoveType.MoveTo;
			trans.mc.move(param);

            //WaitForIdle(_axis);
            //saveDynamicData(axis);
            waitAndSaveDynamicData(axis);

            return true;
		}

		public void StopMove(int _axis)
		{
			AlignerMcTransform ic = mAxisTransMap[(AlignAxis)_axis];
			ic.mc.stop(ic.mcAxis);
		}
		public void StopMove()
		{
			foreach (AlignAxis axis in mAxisTransMap.Keys) StopMove((int)axis);
        }

		public bool IsMovingOK(int _axis)
		{
			AlignerMcTransform ic = mAxisTransMap[(AlignAxis)_axis];
			return ic.mc.isMoving(ic.mcAxis);
		}
		public bool IsMovingOK()
		{
			bool isOk = false;
			foreach (var axis in mAxisTransMap.Keys) isOk |= IsMovingOK((int)axis);
			return isOk;
		}

		virtual public void WaitForIdle(int _axis)
		{
            while (IsMovingOK(_axis)) Thread.Sleep(10);		
        }
		public void WaitForIdle()
		{
            while (IsMovingOK()) Thread.Sleep(10);
        }


		#endregion


		#endregion === Istage

	}

	public class CenterAligner : AlignerBase
	{
		public CenterAligner() : base(IFA_AlignerId.Center, "CenterAligner") { }
	}
	public class LeftAligner : AlignerBase
	{
		public LeftAligner() : base(IFA_AlignerId.Left, "LeftAligner") { }
	}
	public class RightAligner : AlignerBase
	{
		public RightAligner() : base(IFA_AlignerId.Right, "RightAligner") { }
	}


}
