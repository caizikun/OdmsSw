using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public class SurugaAligner: SurugaAlignerBase, Istage
    {
        static int mAlignerCounter = 0;
        public SurugaAligner(int numAxis) : base(numAxis)
        {
            stageNo = ++mAlignerCounter;
        }

        public int AXIS_X { get { return CsurugaseikiMc.MOTOR_AXIS_X; } }
        public int AXIS_Y { get { return CsurugaseikiMc.MOTOR_AXIS_Y; } }
        public int AXIS_Z { get { return CsurugaseikiMc.MOTOR_AXIS_Z; } }
        public int AXIS_ThetaX { get { return CsurugaseikiMc.MOTOR_AXIS_U; } }
        public int AXIS_ThetaY { get { return CsurugaseikiMc.MOTOR_AXIS_V; } }
        public int AXIS_ThetaZ { get { return CsurugaseikiMc.MOTOR_AXIS_W; } }
        public int AXIS_TX{ get { return CsurugaseikiMc.MOTOR_AXIS_U; } }
        public int AXIS_TY{ get { return CsurugaseikiMc.MOTOR_AXIS_V; } }
        public int AXIS_TZ{ get { return CsurugaseikiMc.MOTOR_AXIS_W; } }
        public int AXIS_ALL { get { return CsurugaseikiMc.MOTOR_AXIS_ALL; } }
        public int AXIS_U { get { return CsurugaseikiMc.MOTOR_AXIS_U; } }
        public int AXIS_V { get { return CsurugaseikiMc.MOTOR_AXIS_V; } }
        public int AXIS_W { get { return CsurugaseikiMc.MOTOR_AXIS_W; } }

        public int MOVESPEED_SLOW { get { return 0; } }
        public int MOVESPEED_MID { get { return 1; } }
        public int MOVESPEED_FAST { get { return 2; } }
        public int DIRECTION_MINUS { get  { return 0; } }
        public int DIRECTION_PLUS { get {  return 1; } }
        public int stageNo { get; set; }


        public bool IsConnectedOK()
        {
            for (int m = 0; m < mMC.Length; m++) if (!mMC[m].IsConnectedOK()) return false;
            return true;
        }

        public bool Homing(int _axis)
        {
            base.MoveToHome(mAxisIndex[_axis]);
            return true;
        }

        public bool Homing()
        {
            base.MoveToHome();
            return true;
        }

        public bool Zeroing(int _axis)
        {
            base.Origin(mAxisIndex[_axis]);
            return true;
        }

        public bool Zeroing()
        {
            base.Origin();
            return true;
        }


        public double GetAxisAbsPos(int _axis)
        {
            return base.ReadCoord(mAxisIndex[_axis]);
        }

        public CStageAbsPos GetAbsPositions()
        {
            base.ReadCoord();
            var pos = new CStageAbsPos();
            pos.SetValue(mCoord);
            return pos;
        }


        public bool RelMove(int _axis, double _dist, int _speed)
        {
            var speed = (_speed == MOVESPEED_FAST) ? DriveSpeed.High : DriveSpeed.Low;
            base.MoveAs(mAxisIndex[_axis], _dist, speed);
            //WaitForIdle(_axis);//*******

            //base.ReadCoord(mAxisIndex[_axis]);

            return true;
        }

        public bool RelMove(int _axis, double _dist)
        {
            return RelMove(_axis, _dist, MOVESPEED_FAST);
        }

        public bool AbsMove(int _axis, double _pos, int _speed)
        {
            var current = mCoord[mAxisIndex[_axis]];
            return RelMove(_axis, _pos - current, _speed);
        }

        public bool AbsMove(int _axis, double _pos)
        {
            return AbsMove(_axis, _pos, MOVESPEED_FAST);
        }

        public void WaitForIdle(int _axis)
        {
            var axisIndex = mAxisIndex[_axis];
            var mcAxis = mMcAxisList[axisIndex % 2];
            while (mMC[mAxisIndex[_axis] / 2].IsInMotionOK(mcAxis)) Thread.Sleep(10);
        }

        public void WaitForIdle()
        {
            for (int m = 0; m < mMC.Length; m++) while (mMC[m].IsInMotionOK()) ;
        }

        public void StopMove(int _axis)
        {
            var axisIndex = mAxisIndex[_axis];
            var mcAxis = mMcAxisList[axisIndex % 2];
            mMC[axisIndex / 2].Stop(mcAxis, CsurugaseikiMc.STOP_TYPE_SLOWDOWN);
            base.Abort();
        }

        public void StopMove()
        {
            for (int m = 0; m < mMC.Length; m++) mMC[m].Stop(CsurugaseikiMc.STOP_TYPE_SLOWDOWN);
            base.Abort();
        }

        public bool IsMovingOK()
        {
            for (int m = 0; m < mMC.Length; m++)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (mMC[m].IsInMotionOK(mMcAxisList[x])) return true;
                }
            }
            return false;
        }

        public bool IsMovingOK(int _axis)
        {
            var axisIndex = mAxisIndex[_axis];
            var mcAxis = mMcAxisList[axisIndex % 2];
            return mMC[axisIndex / 2].IsInMotionOK(mcAxis);
        }
    }//class
}
