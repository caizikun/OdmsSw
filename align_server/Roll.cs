using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Neon.Aligner
{
    public class RollParam
    {
        public int Range, Step;
        public double Threshold;

        public bool UsingLocalTls;
        public double Wave1, Wave2;

        public int Port1, Port2, DistanceX;

        private RollParam(int r, int s, double t)
        {
            Range = r; Step = s; Threshold = t;
            UsingLocalTls = false;
            Wave1 = 1550;
            Wave2 = 1550;
        }
        public static RollParam Create(int range, int step, double threshold)
        {
            return new RollParam(range, step, threshold);
        }
        public static RollParam Create(int range, int step, double threshold, int port1, int port2, int distX)
        {
            return new RollParam(range, step, threshold)
            {
                Port1 = port1, Port2 = port2, DistanceX = distX
            };
        }
    }



    public partial class AlignLogic
    {
        public void SetRollParam(int range, int step, double threshold)
        {
            RollParam = RollParam.Create(range, step, threshold);
        }
        public void SetRollParam   (int range, int step, double threshold, int port1, int port2, int distX)
        {
            RollParam = RollParam.Create(range, step, threshold, port1, port2, distX);
        }
        public RollParam RollParam { get; private set; }

        public void RollOut()
        {
            int port1 = RollParam.Port1;
            int port2 = RollParam.Port2;
            int dist = RollParam.DistanceX;
            int rng = RollParam.Range;
            double step = RollParam.Step;
            double thres = RollParam.Threshold;

            RollOut(port1, port2, dist, rng, step, thres);
        }

        /// <summary>
        /// output side Roll alignment를 한다.
        /// </summary>
        /// <param name="_port1">1st port of powermeter for searching</param>
        /// <param name="_port2">2nd port of powermeter for searching</param>
        /// <param name="_dist">a distance between 1st ch. to 2st ch. of DUT [um]</param>
        /// <param name="_rng">searching range [um]</param>
        /// <param name="_step">searching step [um]</param>
        /// <param name="_thres">정렬이 완료되었다고 판단할 값. </param>
        public void RollOut(int _port1, int _port2, int _dist, int _rng, double _step, double _thres)
        {
            const double MINANGSTEP = 0.0032; //TZ 회전 할때의 최소 회전각. degree
            const int HILLCOUNT = 0;
            const double LPFCONST = 0.5;  //Low pass filter 상수.

            //시작 ...
            mCompleted = false;
            mStopFlag = false;
            CurFuncNo = ROLLOUT;
            CurStageNo = RIGHT_STAGE;

            try
            {
                //state
                CrollStatus state = AlignStatusPool.rollOut;
                state.Clear();

                //stage 
                Istage stage = mRight;
                int stgNo = mRight.stageNo;

                //190308 
                if (IsTestMode)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        state.tzPosList.Add(i);
                        state.pwrList1.Add(5 - Math.Abs((i - 10.0) / 10));
                        state.pwrList2.Add(5 - Math.Abs((i - 10.0) / 9));
                        Thread.Sleep(200);
                    }
                    return;
                }

                //X축을 1um 스탭으로 정렬한다.
                //first channel에 얼라인 된 상태에서 시작한다.
                PosPwr1d orgPosPwr;
                orgPosPwr = AxisSearchByHillclimb(stgNo, stage.AXIS_X, _port1, 1, HILLCOUNT);
                if (mStopFlag == true) return;

                //roll alignment 
                double moveDist = 0.0;
                double angStep = 0.0;
                double curTzPos = stage.GetAxisAbsPos(stage.AXIS_TZ);
                double curDy = 0.0;
                int curSearchDir = 0;
                List<double> ch1PwrList = new List<double>();
                List<double> ch2PwrList = new List<double>();
                int scanLoopTime = (int)(_rng / _step);
                double ctrPos1 = 0.0;   //center pos of port1
                double ctrPos2 = 0.0;   //center pos of port2
                double p1f = 0.0;
                double p2f = 0.0;
                double p1l = 0.0;
                double p2l = 0.0;

                while (true)
                {
                    //---- prepare ---
                    double pwr1 = 0.0;
                    double pwr2 = 0.0;
                    ch1PwrList.Clear();
                    ch2PwrList.Clear();

                    //state.
                    state.pwrList1.Clear();
                    state.pwrList2.Clear();

                    //move to start-scan pos.
                    moveDist = (-1) * Math.Round((double)(_rng / 2), STGPOSXYZRES);
                    stage.RelMove(stage.AXIS_Y, moveDist);
                    stage.WaitForIdle(stage.AXIS_Y);


                    //---- scan first and last port. ---
                    for (int i = 0; i < scanLoopTime; i++)
                    {
                        //한스텝 이동.
                        stage.RelMove(stage.AXIS_Y, _step);
                        stage.WaitForIdle(stage.AXIS_Y);

                        //optical power.
                        pwr1 = Math.Round(mPm.ReadPower(_port1), OPTPWRRES);
                        pwr2 = Math.Round(mPm.ReadPower(_port2), OPTPWRRES);
                        ch1PwrList.Add(pwr1);
                        ch2PwrList.Add(pwr2);

                        //state.
                        state.pwrList1.Add(pwr1);
                        state.pwrList2.Add(pwr2);
                    }
                    if (mStopFlag == true) return;

                    //광파워를 확인하고 원래 값에 1/4 이하로 떨어지면
                    //XY searching을 한다.
                    //광을 잃어버리지 않기 위해서...
                    if (ch1PwrList.Max() <= (orgPosPwr.pwr / 4))
                    {
                        doXySearch_HillClimb(stgNo, _port1, 1, HILLCOUNT);
                        continue;
                    }

                    //low pass filtering
                    //ch1PwrList = JeffMath.LowPassFilter(ch1PwrList.ToArray(), LPFCONST).Select((x) => Math.Round(x, OPTPWRRES)).ToList();
                    //ch2PwrList = JeffMath.LowPassFilter(ch2PwrList.ToArray(), LPFCONST).Select((x) => Math.Round(x, OPTPWRRES)).ToList();
                    DataProcessingLogic.ApplyMovingAverage(ch1PwrList, 2);
                    DataProcessingLogic.ApplyMovingAverage(ch2PwrList, 2);

                    //center pos. ,delta y , power at center pos.
                    ctrPos1 = CalcCtrPos(ch1PwrList, _step);
                    ctrPos2 = CalcCtrPos(ch2PwrList, _step);
                    p1f = CalcPwrByPos(ch1PwrList, ctrPos1, _step);
                    p2f = CalcPwrByPos(ch2PwrList, ctrPos1, _step);
                    p1l = CalcPwrByPos(ch1PwrList, ctrPos2, _step);
                    p2l = CalcPwrByPos(ch2PwrList, ctrPos2, _step);
                    curDy = Math.Round(ctrPos2 - ctrPos1, STGPOSXYZRES);

                    //angle
                    angStep = Math.Abs((180 / Math.PI) * (curDy / _dist)) / 4;
                    angStep = angStep - (angStep % MINANGSTEP);
                    angStep = Math.Round(angStep, STGPOSUVWRES);
                    if (angStep <= MINANGSTEP) angStep = MINANGSTEP;

                    //direction.
                    if (curDy > 0) curSearchDir = stage.DIRECTION_PLUS;
                    else if (curDy < 0) curSearchDir = stage.DIRECTION_MINUS;

                    //----move to Y-axis max. position
                    moveDist = (-1) * (int)(_rng - ctrPos1);
                    stage.RelMove(stage.AXIS_Y, moveDist);
                    stage.WaitForIdle(stage.AXIS_Y);

                    //------ Align 완료 판별 --------
                    if (Math.Abs(curDy) <= _thres) break;

                    //------ tz 회전 --------
                    //tz axis  step 회전
                    if (curSearchDir == stage.DIRECTION_PLUS) moveDist = Math.Round(angStep, STGPOSUVWRES);
                    else moveDist = (-1) * Math.Round(angStep, STGPOSUVWRES);
                    stage.RelMove(stage.AXIS_TZ, moveDist);
                    stage.WaitForIdle(stage.AXIS_TZ);

                    //Tz position 계산.
                    curTzPos += moveDist;
                    curTzPos = Math.Round(curTzPos, STGPOSUVWRES);

                    //state
                    state.tzPos = curTzPos;
                    state.tzPosList.Add(curTzPos);
                }

                //마무리 alignment.
                //AxisSearchByHillclimb(stgNo, stage.AXIS_X, _port1, 1, HILLCOUNT);
                //AxisSearchByHillclimb(stgNo, stage.AXIS_Y, _port1, 1, HILLCOUNT);
                //XySearch(stgNo, 60, 4, _port1);
            }
            finally
            {
                //complete!!
                //curAlignNo = NOOPERATION;
                mCompleted = true;
            }
        }



        /// <summary>
        /// output side Roll alignment를 한다. 
        /// (TLS 이용)
        /// </summary>
        /// <param name="port1">1st port of powermeter for searching</param>
        /// <param name="port2">2nd port of powermeter for searching</param>
        /// <param name="portDistance">a distance between 1st ch. to 2st ch. of DUT [um]</param>
        /// <param name="tls">a reference to  TLS(Tunalbe Laser Source) instacne</param>
        /// <param name="wave1">wavelength to port1</param>
        /// <param name="wave2">wavelength to port2</param>
        /// <param name="searchRange">searching range [um]</param>
        /// <param name="scanStep"> y-axis scan할 때의 step [um]</param>
        /// <param name="maxYDiff">정렬 됬다고 판단할 값 : 첫번째 포토 정렬될을때 와 두번째 포트 정렬됬을 때의 y축 차이값 [um] </param>
        public void RollOut(int port1, int port2, int portDistance, Itls tls, double wave1, double wave2, int searchRange, double scanStep, double maxYDiff)
        {
            const double MINANGSTEP = 0.0032; //TZ 회전 할때의 최소 회전각. degree
            const int HILLCOUNT = 0;
            const double LPFCONST = 0.5;  //Low pass filter 상수.

            //시작 ...
            mCompleted = false;
            mStopFlag = false;
            CurFuncNo = ROLLOUT;
            CurStageNo = RIGHT_STAGE;

            //int wl1 = (int)wave1;
            //int wl2 = (int)wave2;

            try
            {
                //---- TCP 서버 등록, 단독 정렬 모드 시작
                //if (doRegister) await mTcp?.Register();
                //await mTcp?.BeginAlign();

                //state
                CrollStatus state = AlignStatusPool.rollOut;
                state.Clear();

                //stage 
                Istage stage = mRight;
                int stgNo = mRight.stageNo;

                //190308 
                if (IsTestMode)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        state.tzPosList.Add(i);
                        state.pwrList1.Add(5 - Math.Abs((i - 10.0) / 10));
                        state.pwrList2.Add(5 - Math.Abs((i - 10.0) / 9));
                        Thread.Sleep(200);
                    }
                    return;
                }

                //X축을 1um 스탭으로 정렬한다.
                //first channel에 얼라인 된 상태에서 시작한다.
                tls.SetTlsWavelen(wave1);
                //await mTcp.Align(wl1);
                double basePower = AxisSearchByHillclimb(stgNo, stage.AXIS_X, port1, 1, HILLCOUNT).pwr;

                //stop?
                if (mStopFlag == true) return;

                //roll alignment 
                double moveDist = 0.0;
                double angStep = 0.0;
                double curTzPos = stage.GetAxisAbsPos(stage.AXIS_TZ);
                double curDy = 0.0;
                int curSearchDir = 0;
                List<double> ch1PwrList = new List<double>();
                List<double> ch2PwrList = new List<double>();
                int scanLoopTime = (int)(searchRange / scanStep);
                double ctrPos1 = 0.0;   //center pos of port1
                double ctrPos2 = 0.0;   //center pos of port2
                double p1f = 0.0;
                double p2f = 0.0;
                double p1l = 0.0;
                double p2l = 0.0;

                while (true)
                {
                    //---- prepare ---
                    double pwr1 = 0.0;
                    double pwr2 = 0.0;
                    ch1PwrList.Clear();
                    ch2PwrList.Clear();

                    //state.
                    state.pwrList1.Clear();
                    state.pwrList2.Clear();

                    //move to start-scan pos. & set tls wavelength for first channel.
                    moveDist = (-1) * Math.Round((double)(searchRange / 2), STGPOSXYZRES);
                    moveDist = Math.Round(moveDist, STGPOSXYZRES);
                    stage.RelMove(stage.AXIS_Y, moveDist);
                    stage.WaitForIdle(stage.AXIS_Y);

                    //---- scan first  port. ---
                    tls.SetTlsWavelen(wave1);
                    for (int i = 0; i < scanLoopTime; i++)
                    {
                        //한스텝 이동.
                        stage.RelMove(stage.AXIS_Y, scanStep);
                        stage.WaitForIdle(stage.AXIS_Y);

                        //optical power.
                        pwr1 = Math.Round(mPm.ReadPower(port1), OPTPWRRES); //[mW]
                        ch1PwrList.Add(pwr1);

                        //state.
                        state.pwrList1.Add(pwr1);
                    }

                    if (mStopFlag == true) return;

                    //광파워를 확인하고 원래 값에 1/4 이하로 떨어지면
                    //XY searching을 한다.
                    //광을 잃어버리지 않기 위해서...
                    if (ch1PwrList.Max() <= (basePower / 4))
                    {
                        doXySearch_HillClimb(stgNo, port1, 1, HILLCOUNT);
                        continue;
                    }

                    //---- scan last port. ---
                    tls.SetTlsWavelen(wave2);
                    //await mTcp.Align(wl2);
                    for (int i = 0; i < scanLoopTime; i++)
                    {
                        //한스텝 이동.
                        stage.RelMove(stage.AXIS_Y, (-1) * scanStep);
                        stage.WaitForIdle(stage.AXIS_Y);

                        //optical power.
                        pwr2 = Math.Round(mPm.ReadPower(port2), OPTPWRRES);
                        ch2PwrList.Add(pwr2);

                        //state.
                        state.pwrList2.Add(pwr2);
                    }
                    ch2PwrList.Reverse();
                    state.pwrList2.Reverse();

                    if (mStopFlag == true) return;

                    //low pass filtering
                    //ch1PwrList = JeffMath.LowPassFilter(ch1PwrList.ToArray(), LPFCONST).Select((x) => Math.Round(x, OPTPWRRES)).ToList();
                    //ch2PwrList = JeffMath.LowPassFilter(ch2PwrList.ToArray(), LPFCONST).Select((x) => Math.Round(x, OPTPWRRES)).ToList();
                    DataProcessingLogic.ApplyMovingAverage(ch1PwrList, 2);
                    DataProcessingLogic.ApplyMovingAverage(ch2PwrList, 2);

                    //center pos. ,delta y , power at center pos.
                    ctrPos1 = CalcCtrPos(ch1PwrList, scanStep);
                    ctrPos2 = CalcCtrPos(ch2PwrList, scanStep);
                    p1f = CalcPwrByPos(ch1PwrList, ctrPos1, scanStep);
                    p2f = CalcPwrByPos(ch2PwrList, ctrPos1, scanStep);
                    p1l = CalcPwrByPos(ch1PwrList, ctrPos2, scanStep);
                    p2l = CalcPwrByPos(ch2PwrList, ctrPos2, scanStep);
                    curDy = Math.Round(ctrPos2 - ctrPos1, 2);

                    //회전할 angle 계산.
                    angStep = Math.Abs((180 / Math.PI) * (curDy / portDistance)) / 4;
                    angStep = angStep - (angStep % MINANGSTEP);
                    angStep = Math.Round(angStep, STGPOSUVWRES);
                    if (angStep <= MINANGSTEP) angStep = MINANGSTEP;

                    //회전 방향 결정.
                    if (curDy > 0) curSearchDir = stage.DIRECTION_PLUS;
                    else if (curDy < 0) curSearchDir = stage.DIRECTION_MINUS;

                    //move to Y-axis max. position
                    moveDist = (int)(ctrPos1);
                    stage.RelMove(stage.AXIS_Y, moveDist);
                    //_tls.SetTlsWavelen(_lambda1);
                    //await mTcp.Align(wl1);
                    stage.WaitForIdle(stage.AXIS_Y);

                    //Alignment 완료 판별  
                    if (Math.Abs(curDy) <= maxYDiff) break;


                    //------ tz 회전 --------
                    //이동.
                    if (curSearchDir == stage.DIRECTION_PLUS) moveDist = Math.Round(angStep, STGPOSUVWRES);
                    else moveDist = (-1) * Math.Round(angStep, STGPOSUVWRES);
                    stage.RelMove(stage.AXIS_TZ, moveDist);
                    stage.WaitForIdle(stage.AXIS_TZ);

                    //Tz position 계산.
                    curTzPos += moveDist;
                    curTzPos = Math.Round(curTzPos, STGPOSUVWRES);

                    //state
                    state.tzPos = curTzPos;
                    state.tzPosList.Add(curTzPos);

                }//while ~ align

                //마무리 alignment
                tls.SetTlsWavelen(wave1);
                //await mTcp.Align(wl1);
                //AxisSearchByHillclimb(stgNo, stage.AXIS_X, port1, 1, HILLCOUNT);
                //AxisSearchByHillclimb(stgNo, stage.AXIS_Y, port1, 1, HILLCOUNT);

            }
            finally
            {
                //await mTcp?.EndAlign();
                //if (doRegister) await mTcp?.UnRegister();
                //complete!!
                //curAlignNo = NOOPERATION;
                mCompleted = true;
            }
        }

    }//class

}
