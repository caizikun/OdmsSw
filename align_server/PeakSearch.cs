using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Free302.TnM.DataAnalysis;

namespace Neon.Aligner
{
    public partial class AlignLogic
    {
        #region ==== Axis Scan ====

        ScanParam mScanParam;
        const int mStepDelay_ms = 10;

        void setupScanParam(ScanParam param, Func<int, double> powerFunc = null)
        {
            var stage = param.StageNo == mLeft.stageNo ? mLeft : mRight;
            param.MoveFunc = (s, axis, distance) =>
            {
                stage.RelMove(axis, distance);
                stage.WaitForIdle(axis);
                Thread.Sleep(mStepDelay_ms);
            };
            param.PositionFunc = (s, axis) => stage.GetAxisAbsPos(axis);
            param.PowerFunc = powerFunc ?? mPm.ReadPower;

            param.ReportFunc = mReporter;
            param.StopCheckFunc = doStopChecking;
        }

        static void doScan(ScanParam param)
        {
            if (param.MoveFunc == null) throw new ArgumentException($"AlignLogic.scan():\nScanParam.MoveFunc == null");
            if (param.PositionFunc == null) throw new ArgumentException($"AlignLogic.scan():\nScanParam.PositionFunc == null");
            if (param.PowerFunc == null) throw new ArgumentException($"AlignLogic.scan():\nScanParam.PowerFunc == null");
            
            var msgHeader = $"doScan(): {param.StageNo}.{param.AxisNo} @ PmCh{param.Port} : ";
            param.ReportFunc?.Invoke($"{msgHeader}Starting {param.Range}/{param.Step}");

            //190308 
            if (IsTestMode)
            {
                for (int i = 0; i < 20; i++)
                {
                    param.UpdateFunc(i, 5 - Math.Abs((i - 10.0) / 10));
                    Thread.Sleep(100);
                }
                return;
            }

            //scan
            ScanData data = scan(param, msgHeader);
            for (int i = 0; i < 5; i++)
            {
                if (data.Power.Count < 2) break;

                param.Dir = nextDir(param, data);//check complete condition
                if (param.Dir == ScanDirection.BiDir) break;

                moveToPeak(param, data);
                var backDistance = Math.Ceiling(param.Range * 0.2);
                var dir = param.Dir == ScanDirection.Negative ? +1 : -1;
                param.Move(dir * backDistance);
                data = scan(param, msgHeader);
            }//for
            
            //move to target position
            if (!param.MoveToCenter) moveToPeak(param, data);
            else moveToCenter(param, data);
        }
        static ScanDirection nextDir(ScanParam param, ScanData scanData)
        {
            var peakIndex = scanData.FindPeakIndex();
            var length = scanData.Coord.Count;

            if (peakIndex < length * 0.1) return ScanDirection.Negative;
            if (peakIndex > length * 0.9) return ScanDirection.Positive;
            else return ScanDirection.BiDir;
        }
        static ScanData scan(ScanParam param, string msgHeader)
        {
            var scanData = new ScanData();

            //backup starting point
            var startCoord = param.Position();
            param.ReportFunc?.Invoke($"{msgHeader}current coord = {startCoord}");

            //scan 방향 결정
            var dir = -1;
            if (param.Dir == ScanDirection.BiDir) param.Move(-dir * param.Range / 2);//move to starting point = -range/2
            else if (param.Dir == ScanDirection.Positive) dir = +1;
            else if (param.Dir == ScanDirection.Negative) dir = -1;

            if (param.StopCheckFunc()) return scanData;

            //read coord, power & step
            var numStep = 1 + (int)Math.Ceiling(param.Range / param.Step);
            for (int i = 0; i < numStep; i++)
            {
                var coord = param.Position();
                scanData.Coord.Add(coord);

                var power = param.Power();
                scanData.Power.Add(power);

#if DEBUG
                param.ReportFunc?.Invoke($"{msgHeader}({coord}, {power:F03})");
#endif
                param.UpdateFunc?.Invoke(coord, power);
                if (param.StopCheckFunc()) return scanData;//user stop

                if (i < numStep - 1) param.Move(dir * param.Step);
            }
            return scanData;
        }

        static void moveToPeak(ScanParam param, ScanData scanData)
        {
            var peakCoord = scanData.FindPeakCoord();
            if (double.IsNaN(peakCoord)) return;

            var peakPower = param.DoFinePeak ? scanData.Power.Max() : double.NaN;

            var numLoop = 3;
            var maxDiff = 0.01;//0.02dB
            var dx = ScanParam.FinePeakStep;

            param.ReportFunc?.Invoke($"MoveToPeak ({peakCoord}, {peakPower:F03})");
            param.MoveTo(peakCoord);

            if (double.IsNaN(peakPower)) return;
            if (isPeakPower(param, peakPower, peakCoord, maxDiff)) return;

            var msgHeader = "moveToPeak()";
            File.WriteAllText(@"log\error.log", $"{msgHeader}: Starting fine peak search ({peakCoord}, {peakPower})");

            for (int i = 0; i < numLoop; i++)
            {
                var range = i + 1;//um
                var fullRange = (int)Math.Ceiling(2 * range / dx);

                //step1 : move to + range
                param.Move(range);
                if (isPeakPower(param, peakPower, peakCoord, maxDiff)) return;

                //step2 : - scan
                for (int j = 0; j < fullRange; j++)
                {
                    param.Move(-dx);
                    if (isPeakPower(param, peakPower, peakCoord, maxDiff)) return;
                }

                //step3: + scan
                for (int j = 0; j < fullRange; j++)
                {
                    param.Move(+dx);
                    if (isPeakPower(param, peakPower, peakCoord, maxDiff)) return;
                }

                //step4: move to - range ~ original position
                param.Move(-range);
                if (isPeakPower(param, peakPower, peakCoord, maxDiff)) return;

                if (param.StopCheckFunc()) return;
            }

            var msg = $"{msgHeader}: <실패> peak power = {peakPower:F03}인 위치를 찾을 수없습니다.";
            param.ReportFunc?.Invoke(msg);
            File.WriteAllText(@"log\error.log", msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="peakPower"></param>
        /// <param name="peakCoord"></param>
        /// <param name="maxDiff"></param>
        /// <returns></returns>
        static bool isPeakPower(ScanParam param, double peakPower, double peakCoord, double maxDiff)
        {
            var coord = param.Position();
            var power = param.Power();
            if (peakPower - power <= maxDiff)
            {
                var msg = $"Peak found:({peakCoord}, {peakPower:F03})->({coord}, {power:F03})";
                param.ReportFunc?.Invoke(msg);
                File.WriteAllText(@"log\error.log", msg);

                return true;
            }
            return false;
        }

        static void moveToCenter(ScanParam param, ScanData scanData)
        {
            var centerCoord = scanData.FindCenterCoord();
            if (double.IsNaN(centerCoord)) return;

            param.MoveTo(centerCoord);

            var powerList = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                var centerPower = param.Power();
                powerList.Add(centerPower);
            }            
            param.ReportFunc?.Invoke($"Center: ({centerCoord}, min={powerList.Min():F03}, max={powerList.Max():F03})");
        }


        #region ==== TEST ====

        static double mCurrentCoord = 0;
        static void test_Move(int s, int a, double distance)
        {
            mCurrentCoord += distance;
            //Task.Delay(100).Wait();
        }
        static double test_Position(int s, int a)
        {
            return mCurrentCoord;
        }
        static Random mRandom = new Random(DateTime.Now.Millisecond);
        static int mRange = 40;
        static double test_Power(int port)
        {
            var x = mCurrentCoord;

            var w = mRange / 7;
            var h = 30;
            var exp = -x * x / 2 / w / w;
            var y = h * Math.Exp(exp);

            var r = 2 * mRandom.NextDouble();
            return y + r;
        }

        public static void TestScan(Action<string> reporter, Action<double, double> updater)
        {
            var range = 80;
            var step = 3;
            var param = new ScanParam(1, 1, range, step, 1);
            param.MoveFunc = test_Move;
            param.PositionFunc = test_Position;
            param.PowerFunc = test_Power;
            param.ReportFunc = reporter;
            param.UpdateFunc = updater;

            mRange = range;
            mCurrentCoord = 0;
            doScan(param);
        }

#endregion



        public void AxisSearch(int stageNo, int axisNo, int port, double range, double step, bool moveToCenter)
        {
            try
            {
                //시작 ...
                mCompleted = mStopFlag = false;

                CurFuncNo = AXISSEARCH;
                CurStageNo = stageNo;
                CurAxisNo = axisNo;

                //Stage
                Istage stage = (stageNo == LEFT_STAGE) ? mLeft : mRight;
                mScanParam = new ScanParam(stageNo, axisNo, range, step, port);
                mScanParam.MoveToCenter = moveToCenter;
                setupScanParam(mScanParam);

                if (axisNo == stage.AXIS_X)
                {
                    CsearchStatus status = (stageNo == LEFT_STAGE) ? AlignStatusPool.axisSearchInX : AlignStatusPool.axisSearchOutX;
                    status.Clear();

                    //scan
                    var y0 = mScanParam.PositionFunc(stageNo, stage.AXIS_Y);//current y
                    mScanParam.UpdateFunc = (x, p) => status.Add(x, y0, Unit.Dbm2MilliWatt(p));
                    doScan(mScanParam);
                }
                else if (axisNo == stage.AXIS_Y)
                {
                    CsearchStatus status = (stageNo == LEFT_STAGE) ? AlignStatusPool.axisSearchInY : AlignStatusPool.axisSearchOutY;
                    status.Clear();

                    //scan
                    var x0 = mScanParam.PositionFunc(stageNo, stage.AXIS_X);//current x
                    mScanParam.UpdateFunc = (y, p) => status.Add(x0, y, Unit.Dbm2MilliWatt(p));
                    doScan(mScanParam);
                }
            }
            finally
            {
                mCompleted = true;
            }
        }

        #endregion



        #region ==== XY Search - Axis Scan ====

        /// <summary>
        /// Peak search by scan
        /// </summary>
        public void XySearch(XYSearchParam param, Func<int, double> powerFunc = null)
        {
            try
            {
                //시작 ...
                mCompleted = mStopFlag = false;
                CurFuncNo = XY_SEARCH;
                CurStageNo = param.StageNo;

                if (param.SearchByScan) doXySearch_Scan(param, powerFunc);
                else doXySearch_HillClimb(param.StageNo, param.Port, param.StepY, 0);
            }
            finally
            {
                mCompleted = true;
                mWatch.Stop();
            }
        }

        private void doXySearch_Scan(XYSearchParam param, Func<int, double> powerFunc)
        {
            var whichStage = (param.StageNo == LEFT_STAGE) ? "LEFT" : "RIGHT";
            mReporter?.Invoke($"doXySearch_Scan(): {whichStage} @ P{param.Port}");

            //Stage
            Istage stage = (param.StageNo == LEFT_STAGE) ? mLeft : mRight;
            var moveToCenter = param.ScanToCenter;//param.StageNo == RIGHT_STAGE;

            //status 초기화.
            CsearchStatus status = (param.StageNo == LEFT_STAGE)? AlignStatusPool.xySearchIn: AlignStatusPool.xySearchOut;
            status.Clear();

            //y scan
            status.Clear();
            mScanParam = new ScanParam(param.StageNo, stage.AXIS_Y, param.RangeY * param.RangeScaleFactor, param.StepY, param.Port);
            mScanParam.MoveToCenter = moveToCenter;
            setupScanParam(mScanParam, powerFunc);
            var x0 = mScanParam.PositionFunc(param.StageNo, stage.AXIS_X);//current x
            mScanParam.UpdateFunc = (y, p) => status.Add(x0, y, Unit.Dbm2MilliWatt(p));
            mScanParam.DoFinePeak = false;
            doScan(mScanParam);
            if (doStopChecking()) return;

            //x scan
            status.Clear();
            mScanParam = new ScanParam(param.StageNo, stage.AXIS_X, param.RangeX * param.RangeScaleFactor, param.StepX, param.Port);
            mScanParam.MoveToCenter = moveToCenter;
            setupScanParam(mScanParam, powerFunc);
            var y0 = mScanParam.PositionFunc(param.StageNo, stage.AXIS_Y);//current y
            mScanParam.UpdateFunc = (x, p) => status.Add(x, y0, Unit.Dbm2MilliWatt(p));
            mScanParam.DoFinePeak = false;
            doScan(mScanParam);
            if (doStopChecking()) return;

            //y scan
            status.Clear();
            mScanParam = new ScanParam(param.StageNo, stage.AXIS_Y, param.RangeY * param.RangeScaleFactor, param.StepY, param.Port);
            mScanParam.MoveToCenter = moveToCenter;
            setupScanParam(mScanParam, powerFunc);
            x0 = mScanParam.PositionFunc(param.StageNo, stage.AXIS_X);//current x
            mScanParam.UpdateFunc = (y, p) => status.Add(x0, y, Unit.Dbm2MilliWatt(p));
            mScanParam.DoFinePeak = true;
            doScan(mScanParam);
        }

        #endregion



        #region ==== XY Search - Hill Climb ====

        /// <summary>
        /// peak search by hill climb
        /// </summary>
        public void XySearch(int stageNo, int port, double step)
        {
            const int _hillcount = 0;

            try
            {
                //시작 ...
                mCompleted = false;
                mStopFlag = false;

                CurFuncNo = XY_SEARCH;
                CurStageNo = stageNo;

                //alignment.
                doXySearch_HillClimb(stageNo, port, step, _hillcount);
            }
            finally
            {
                //완료..
                mCompleted = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="stageNo"></param>
        /// <param name="port"></param>
        /// <param name="step"></param>
        /// <param name="_hillcount"></param>
        private void doXySearch_HillClimb(int stageNo, int port, double step, int _hillcount)
        {

            xPos = new List<double>();
            yPos = new List<double>();
            xPower = new List<double>();
            yPower = new List<double>();

            var whichStage = (stageNo == LEFT_STAGE) ? "LEFT" : "RIGHT";
            mReporter?.Invoke($"doXySearch_HillClimb(): {whichStage} @ P{port}");

            //Stage
            Istage stage = (stageNo == LEFT_STAGE) ? mLeft : mRight;
            if (stage == null)
            {
                mReporter?.Invoke("doXySearch_HillClimb():stage==null");
                return;
            }

            //status 초기화.
            CsearchStatus state = (stageNo == LEFT_STAGE) ? AlignStatusPool.xySearchIn : AlignStatusPool.xySearchOut;
            state.pos.x = 0;
            state.pos.y = 0;
            state.pwr = 0;
            state.posList.Clear();
            state.pwrList.Clear();
            
            AxisSearchByHillclimb(stageNo, stage.AXIS_Y, port, step, _hillcount);
            if (mStopFlag == true) return;

            yPos.Add(99999);
            yPower.Add(99999);

            AxisSearchByHillclimb(stageNo, stage.AXIS_X, port, step, _hillcount);
            if (mStopFlag == true) return;

            xPos.Add(99999);
            xPower.Add(99999);

            AxisSearchByHillclimb(stageNo, stage.AXIS_Y, port, step, _hillcount);

            yPos.Add(99999);
            yPower.Add(99999);

            //var startPos = stage.GetAxisAbsPos(stage.AXIS_X);
            //var startPower = Math.Round(Unit.MillWatt2Dbm(mPm.ReadPower(port)), 3);
            AxisSearchByHillclimb(stageNo, stage.AXIS_X, port, ScanParam.FinePeakStep, _hillcount);

            //var endPos = stage.GetAxisAbsPos(stage.AXIS_X);
            //var endPower = Math.Round(Unit.MillWatt2Dbm(mPm.ReadPower(port)), 3);
            //var msg = $"AxisSearchByHillclimb(): {whichStage}.X, ({startPos},{startPower}) -> ({endPos},{endPower})";
            //Log.Write(msg);

            //startPos = stage.GetAxisAbsPos(stage.AXIS_Y);
            //startPower = Math.Round(Unit.MillWatt2Dbm(mPm.ReadPower(port)), 3);
            AxisSearchByHillclimb(stageNo, stage.AXIS_Y, port, ScanParam.FinePeakStep, _hillcount);
            //endPos = stage.GetAxisAbsPos(stage.AXIS_Y);
            //endPower = Math.Round(Unit.MillWatt2Dbm(mPm.ReadPower(port)), 3);
            //msg = $"AxisSearchByHillclimb(): {whichStage}.Y, ({startPos},{startPower}) -> ({endPos},{endPower})";
            //Log.Write(msg);

            //string xx = "xPos :\t" +  string.Join("\t", xPos);
            //string xp = "xPower :\t" + string.Join("\t", xPower);
            //string yy = "yPos :\t" + string.Join("\t", yPos);
            //string yp = "yPower :\t" + string.Join("\t", yPower);
            //Log.Write(whichStage, "HillClimb_" + DateTime.Now.ToString("yy-MM-dd"));
            //Log.Write(xx, "HillClimb_" + DateTime.Now.ToString("yy-MM-dd"));
            //Log.Write(xp, "HillClimb_" + DateTime.Now.ToString("yy-MM-dd"));
            //Log.Write(yy, "HillClimb_" + DateTime.Now.ToString("yy-MM-dd"));
            //Log.Write(yp, "HillClimb_" + DateTime.Now.ToString("yy-MM-dd"));
        }

        private static PosPwr1d doTestMode(CsearchStatus state)
        {
            PosPwr1d ret;
            ret.pos = 10;
            ret.pwr = 5;

            for (int i = 0; i < 20; i++)
            {
                state.posList.Add(new CalignPoint2d(i, i));
                state.pwrList.Add(5 - Math.Abs((i - 10.0) / 10));
                Thread.Sleep(100);
            }
            return ret;
        }

        #endregion

    }//class
}
