using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    public interface Istage
    {

        #region property

        int AXIS_X { get; }
        int AXIS_Y { get; }
        int AXIS_Z { get; }
        int AXIS_ThetaX { get; }
        int AXIS_ThetaY { get; }
        int AXIS_ThetaZ { get; }
        int AXIS_ALL { get; }
        int AXIS_U { get; }
        int AXIS_V { get; }
        int AXIS_W { get; }

        int AXIS_TX { get; }
        int AXIS_TY { get; }
        int AXIS_TZ { get; }

        int MOVESPEED_SLOW { get; }
        int MOVESPEED_MID { get; }
        int MOVESPEED_FAST { get; }

        int DIRECTION_MINUS { get; }
        int DIRECTION_PLUS { get; }


        int stageNo { get; }

        #endregion




        #region method

        bool IsConnectedOK();
        bool Homing(int _axis);
        bool Homing();
        bool Zeroing(int _axis);
        bool Zeroing();
        double GetAxisAbsPos(int _axis);
        CStageAbsPos  GetAbsPositions();
        bool RelMove(int _axis, double _dist, int _speed);
        bool RelMove(int _axis, double _dist);
        bool AbsMove(int _axis, double _pos, int _speed);
        bool AbsMove(int _axis, double _pos);
        void WaitForIdle(int _axis);
        void WaitForIdle();
        void StopMove(int _axis);
        void StopMove();
        bool IsMovingOK();
        bool IsMovingOK(int _axis);

        #endregion

    }


}
