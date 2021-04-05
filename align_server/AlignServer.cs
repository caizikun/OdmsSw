using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Free302.TnM.DataAnalysis;

namespace Neon.Aligner
{
    public partial class AlignLogic
    {
        public event Action<string> mReporter;
        public event Action<int> AlignStarted;
        public event Action<string> AlignCompleted;
        public bool IsEventEnable = true;
        public static bool IsTestMode = false;


        /// <summary>
        /// Fa를 칩에 맞춘다. ty만 진행, tx는 진행하지 않는다.
        /// </summary>
        public void FaArrangement(ref bool doStop)
        {
            const int STAGEOPENDIST = 50;   //[um]
            const int ALIGNDIST = 10;       //[um]

            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (doStop == true) return;

            //input approach.
            ZappSingleStage(mLeft.stageNo);
            if (doStop == true) return;

            //output approach.
            ZappSingleStage(mRight.stageNo);
            if (doStop == true) return;

            //input ty
            AngleTy(mLeft.stageNo);
            if (doStop == true) return;

            //output ty
            AngleTy(mRight.stageNo);
            if (doStop == true) return;

            //stage open.
            mLeft.RelMove(mLeft.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, STAGEOPENDIST * (-1));
            mRight.WaitForIdle();
            if (doStop == true) return;

            //input approach.
            ZappSingleStage(mLeft.stageNo);
            if (doStop == true) return;

            //output approach.
            ZappSingleStage(mRight.stageNo);
            if (doStop == true) return;

            //move to align-distance.
            mLeft.RelMove(mLeft.AXIS_Z, ALIGNDIST * (-1));
            mRight.RelMove(mRight.AXIS_Z, ALIGNDIST * (-1));
            mRight.WaitForIdle();
        }



        public bool AlignDut(int port1, int port2, int minSuccessPower, bool doRoll, int portDistance_um, XYSearchParam paramLeft, XYSearchParam paramRight, Itls tls)
        {
            const int SYNC_SEARCH_RANGE = 50;       //[um]
            const double SYNC_SEARCH_STEP = 10;     //[um]

            double[] powerDiff = { 5, 10 };
            double[] rangeFactor = { 1.5, 2.0 };

            try
            {
                mReporter?.Invoke($"AlingLogic.AlignDut(): Roll={doRoll}");

                //align할 port선택 
                int alignPort = port1;
                var power = Unit.MillWatt2Dbm(mPm.ReadPower(alignPort));

                //Blind Search 여부
                if (power < minSuccessPower)
                {
                    mReporter?.Invoke($"AlignDut(): power {power}<{minSuccessPower} - starting BlindSearch...");

                    //Sync Search 시도.(광을 찾은 상태가 아니면 )
                    var leftX0 = mLeft.GetAxisAbsPos(mLeft.AXIS_X);
                    var leftY0 = mLeft.GetAxisAbsPos(mLeft.AXIS_Y);
                    //SyncXySearch(alignPort, SYNC_SEARCH_RANGE, SYNC_SEARCH_STEP, minSuccessPower);
                    XyBlindSearch(mLeft.stageNo, port1, paramLeft.RangeX, paramLeft.StepX, minSuccessPower);
                    if (mStopFlag == true) return false;

                    //
                    power = Unit.MillWatt2Dbm(mPm.ReadPower(alignPort));
                    if (power < minSuccessPower)
                    {
                        var msg = $"AlignDut(): blind search failed";
                        mReporter?.Invoke(msg);
                        File.WriteAllText(@"log\error.log", msg);

                        //원위치
                        mLeft.AbsMove(mLeft.AXIS_X, leftX0);
                        mLeft.AbsMove(mLeft.AXIS_Y, leftY0);

                        return false;
                    }
                }

                //backup current position
                double dX = 0, dY = 0;

                #region ---- input align ----
                AlignTimer.RecordTime(TimingAction.AlignIn);
                if (paramLeft.Run)
                {
                    paramLeft.RangeScaleFactor = 1;
                    if (power < XYSearchParam.LastPeakPower - powerDiff[0]) paramLeft.RangeScaleFactor = rangeFactor[0];
                    if (power < XYSearchParam.LastPeakPower - powerDiff[1]) paramLeft.RangeScaleFactor = rangeFactor[1];

                    var leftX0 = mLeft.GetAxisAbsPos(mLeft.AXIS_X);
                    var leftY0 = mLeft.GetAxisAbsPos(mLeft.AXIS_Y);

                    paramLeft.StageNo = mLeft.stageNo;
                    paramLeft.Port = alignPort;
                    XySearch(paramLeft);
                    //XySearch(mLeft.stageNo, alignPort, XY_STEP_IN);//old method

                    dX = mLeft.GetAxisAbsPos(mLeft.AXIS_X) - leftX0;
                    dY = mLeft.GetAxisAbsPos(mLeft.AXIS_Y) - leftY0;

                    if (mStopFlag == true) return false;
                }
                #endregion

                #region ---- output align ----
                AlignTimer.RecordTime(TimingAction.Roll);
                //기본 정렬 = in stage와 동기화
                {
                    mRight.RelMove(mRight.AXIS_X, dX);
                    mRight.RelMove(mRight.AXIS_Y, dY);
                }
                if (paramRight.Run && doRoll)
                {
                    System.Threading.Thread.Sleep(100);
                    //roll alignment out 
                    if (tls == null) RollOut(port1, port2, portDistance_um, RollParam.Range, RollParam.Step, RollParam.Threshold);                               //AlignSource Roll
                    else RollOut(port1, port2, portDistance_um, tls, RollParam.Wave1, RollParam.Wave2, RollParam.Range, RollParam.Step, RollParam.Threshold);  //TLS Roll
                    if (mStopFlag == true) return false;
                }

                AlignTimer.RecordTime(TimingAction.AlignOut);
                if (paramRight.Run)
                {
                    power = Unit.MillWatt2Dbm(mPm.ReadPower(alignPort));
                    paramRight.RangeScaleFactor = 1;
                    if (power < XYSearchParam.LastPeakPower - powerDiff[0]) paramRight.RangeScaleFactor = rangeFactor[0];
                    if (power < XYSearchParam.LastPeakPower - powerDiff[1]) paramRight.RangeScaleFactor = rangeFactor[1];

                    //fine search out 
                    paramRight.StageNo = mRight.stageNo;
                    paramRight.Port = alignPort;
                    System.Threading.Thread.Sleep(100);
                    XySearch(paramRight);
                    if (mStopFlag == true) return false;
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                mReporter?.Invoke($"AlingLogic.AlignDut(): {ex.Message}");
                return false;
            }
        }



        /// <summary>
        /// calc displacements(y,z) of next chip
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="chipPitch"></param>
        /// <param name="chipIndex"></param>
        /// <returns>distance[in~out][dY~dZ]</returns>
        public static double[][] CalcNextChip(List<AlignPosition> coords, int chipPitch, int chipIndex)
        {
            //calc p2 by p0, p1
            //m=dy/dx, dx=x1-x0, dy=y1-y0
            //(y2-y1)=dy2=m*dx2=m*(x2-x1)
            var distance = new double[][] { new double[2], new double[2] };

            int numDp = coords.Count();
            if (numDp < 2) return distance;

            var p0 = coords[numDp - 2];
            var p1 = coords[numDp - 1];
            if (p1.chipIndex == p0.chipIndex) return distance;

            double dx = chipPitch * (p1.chipIndex - p0.chipIndex);//dx=p1.In.x - p0.In.x ~ 편차무시
            double dx2 = chipPitch * (chipIndex - p1.chipIndex);
            
            distance[0][0] = Math.Round(dx2 * (p1.In.y - p0.In.y) / dx, 1);
            distance[0][1] = Math.Round(dx2 * (p1.In.z - p0.In.z) / dx, 1);
            distance[1][0] = Math.Round(dx2 * (p1.Out.y - p0.Out.y) / dx, 1);
            distance[1][1] = Math.Round(dx2 * (p1.Out.z - p0.Out.z) / dx, 1);

            return distance;            
        }

        public static void Test_NextChip()
        {
            var coords = new List<AlignPosition>();
            var p = new AlignPosition();
            p.chipIndex = 0;
            p.In.x = 0;
            p.In.y = 0;
            p.In.z = 0;
            p.Out.x = 0;
            p.Out.y = 0;
            p.Out.z = 0;
            coords.Add(p);

            p = new AlignPosition();
            p.chipIndex = 1;
            p.In.x = 1850;
            p.In.y = -2.5;
            p.In.z = 3.5;
            p.Out.x = 1870;
            p.Out.y = 3.4;
            p.Out.z = 2.7;
            coords.Add(p);

            var distance = CalcNextChip(coords, 1870, 2);


        }

    }//class
}