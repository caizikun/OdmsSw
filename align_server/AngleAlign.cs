using System;
using System.Threading;

namespace Neon.Aligner
{
    public partial class AlignLogic
    {
        const double _AngleSpringDistance = 50;

        public void AngleTy(int stageNo, double springDistance = _AngleSpringDistance)
        {
            Angle(stageNo == mLeft.stageNo ? AppStageId.Left : AppStageId.Right, AngleAxis.Ty, springDistance);
        }
        public void AngleTx(int stageNo, double springDistance = _AngleSpringDistance)
        {
            Angle(stageNo == mLeft.stageNo ? AppStageId.Left : AppStageId.Right, AngleAxis.Tx, springDistance);
        }

        public void Angle(AppStageId stageId, AngleAxis axis, double springDistance = _AngleSpringDistance)
        {
            var msgHeader = $"Angle: {stageId} {axis}";
            mReporter?.Invoke($"{msgHeader} Starting...");
            var dz = Math.Abs(springDistance);

            var stage = stageId == AppStageId.Left ? mLeft : mRight;
            try
            {
                stage.RelMove(stage.AXIS_Z, dz);
                stage.WaitForIdle(stage.AXIS_Z);
                angle(stageId, axis);
            }
            finally
            {
                stage.RelMove(stage.AXIS_Z, -dz);
                stage.WaitForIdle(stage.AXIS_Z);
            }
        }


        void angle(AppStageId stageId, AngleAxis axis)
        {
            const double SEARCH_STEP = 0.032;
            const int HILLCOUNT = 2;

            //ID
            Istage stage = stageId == AppStageId.Left ? mLeft : mRight;
            CurStageNo = stageId == AppStageId.Left ? mLeft.stageNo : mRight.stageNo;
            CurAxisNo = axis == AngleAxis.Tx ? stage.AXIS_TX : stage.AXIS_TY;

            try
            {
                //status 초기화.
                AngleStatus status = _angleStatus[stageId][axis];
                status.pos = 0;
                status.sens = 0;
                status.posList.Clear();
                status.sensList.Clear();

                mWatch.Restart();
                mCompleted = mStopFlag = false;
                CurFuncNo = axis == AngleAxis.Tx ? ANGLE_TX_SINGLE : ANGLE_TY_SINGLE;//start

                //190308 
                if (IsTestMode)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        status.posList.Add(i);
                        status.sensList.Add(5 - Math.Abs((i - 10.0) / 10));
                        Thread.Sleep(100);
                    }
                    return;
                }

                //시작 포지션을 구한다.
                double dbOriginalPos = stage.GetAxisAbsPos(CurAxisNo);
                status.pos = Math.Round(dbOriginalPos, STGPOSUVWRES);
                status.posList.Add(dbOriginalPos);

                //시작 센서값을 얻는다.
                double dbOriginSensVal = Math.Round(readVoltage(CurStageNo), DISTSENSRES);
                status.sens = Math.Round(dbOriginSensVal, DISTSENSRES);
                status.sensList.Add(dbOriginSensVal);

                //--------------------- Searching -------------------------------
                double dbSearchStep = Math.Round(SEARCH_STEP, STGPOSUVWRES);
                double dbCurSensVal = Math.Round(dbOriginSensVal, DISTSENSRES);
                double dbCurPos = Math.Round(dbOriginalPos, STGPOSUVWRES);
                double dbMaxSensVal = Math.Round(dbOriginSensVal, DISTSENSRES);
                double dbMaxPos = Math.Round(dbOriginalPos, STGPOSUVWRES);
                double dbSbSensVal = Math.Round(dbOriginSensVal, DISTSENSRES);   //Searching base sensor value
                double dbSbPos = Math.Round(dbOriginalPos, STGPOSUVWRES);                  //Searching base position
                int nTargetDirection = stage.DIRECTION_MINUS;
                int nFindCount = 0;
                bool bSearchDirChanged = false;
                double dbMoveDist = 0;
                while (true)
                {
                    //한 스텝 이동~~
                    dbMoveDist = Math.Round(dbSearchStep, STGPOSUVWRES);
                    if (nTargetDirection != stage.DIRECTION_PLUS) dbMoveDist *= (-1);
                    stage.RelMove(CurAxisNo, dbMoveDist);
                    stage.WaitForIdle(CurAxisNo);

                    //현재 위치 계산
                    dbCurPos += dbMoveDist;
                    dbCurPos = Math.Round(dbCurPos, STGPOSUVWRES);
                    status.pos = Math.Round(dbCurPos, STGPOSUVWRES);
                    status.posList.Add(dbCurPos);

                    //현재 Sensor 값을 읽는다.
                    dbCurSensVal = Math.Round(readVoltage(CurStageNo), DISTSENSRES);
                    status.sens = Math.Round(dbCurSensVal, DISTSENSRES);
                    status.sensList.Add(dbCurSensVal);

                    //Max 값일때의 Postion을 구한다.
                    if (dbCurSensVal > dbMaxSensVal)
                    {
                        dbMaxSensVal = Math.Round(dbCurSensVal, DISTSENSRES);
                        dbMaxPos = Math.Round(dbCurPos, STGPOSUVWRES);
                    }

                    if (dbCurSensVal <= dbSbSensVal)
                    {
                        //--------------- 압력 값이 낮은 쪽으로 잘 못 찾아가는 경우 -----------//
                        if (nFindCount > HILLCOUNT)
                        {
                            //3번 연속 떨어진 상황 // --> Searching base로 이동한다.

                            //이동
                            dbMoveDist = Math.Round((dbSbPos - dbCurPos), STGPOSUVWRES);
                            stage.RelMove(CurAxisNo, dbMoveDist);
                            stage.WaitForIdle(CurAxisNo);

                            //현재 위치.
                            dbCurPos = Math.Round(dbSbPos, STGPOSUVWRES);
                            status.pos = dbCurPos;
                            status.posList.Add(dbSbPos);

                            //distance Sensor value 값 읽음.
                            dbCurSensVal = Math.Round(readVoltage(CurStageNo), DISTSENSRES);
                            status.sens = Math.Round(dbCurSensVal, DISTSENSRES);
                            status.sensList.Add(dbCurSensVal);

                            //stop?
                            if (mStopFlag == true)
                            {
                                stage.StopMove(CurAxisNo);
                                break;
                            }

                            //Searching base Update!! 
                            //이론상으로 서칭베이스로 이동했으므로 서칭베이스값을 변경 시켜줄 필요가 없지만
                            //실제로는 디스턴스 센서값은 바뀔 수 있다.
                            //그래서 업데이트 한다.!!
                            dbSbPos = dbCurPos;
                            dbSbSensVal = dbCurSensVal;

                            //이번 서칭 방향이 변경되었다면 끝낸다.
                            if (bSearchDirChanged == true) break;

                            //Searching 뱡향 변경
                            nTargetDirection = nTargetDirection == stage.DIRECTION_PLUS ? stage.DIRECTION_MINUS : stage.DIRECTION_PLUS;

                            bSearchDirChanged = true;

                            nFindCount = 0;
                        }
                        else
                        {
                            //Distance sensor 값이 감소는 했지만 3번 이하// 
                            //--> nFindCount를 1증가 시킴
                            nFindCount++;
                        }
                    }
                    else
                    {
                        //---------압력 값이 높아지는 쪽으로 잘 찾아가고 있는 경우--------//
                        dbSbPos = Math.Round(dbCurPos, STGPOSUVWRES);
                        dbSbSensVal = Math.Round(dbCurSensVal, DISTSENSRES);
                        nFindCount = 0;
                    }
                    if (mStopFlag) break;
                }//while
            }
            finally
            {
                mCompleted = true;
                mWatch.Stop();
            }
        }

    }//class
}
