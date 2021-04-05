using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Neon.Aligner
{
    public partial class AlignLogic
    {
        public enum ApproachPhase { A1, A2, A3 }//
        public class ApproachParam
        {
            public string time;
            public AppStageId stage;
            public ApproachPhase phase;
            public double baseV;//open voltage of the phase
            public double baseZ;//open z coord
            public List<double> volt;//current voltage
            public List<double> z;//current z coord
            public bool complete = false;
            public int elapsed_ms;
            public ApproachParam(AppStageId st, ApproachPhase ph)
            {
                time = DateTime.Now.ToString("yyMMdd_HHmmss");
                stage = st;
                phase = ph;
                baseV = 0;
                baseZ = 0;
                volt = new List<double>();
                z = new List<double>();
            }
            public override string ToString()
            {
                return $"{stage},{phase},{baseV:F03},{baseZ:F01}";
            }
            public void Write(string fileName)
            {
                var writer = new StreamWriter(fileName, true);
                var sb = new StringBuilder();
                sb.Append($"[{time}]{stage}\t{phase}\t{baseV:F03}\t{baseZ:F01}\n");
                for (int i = 0; i < volt.Count; i++) sb.Append($"{z[i]:F01}\t{volt[i]:F03}\t{baseV - volt[i]:F03}\n");
                writer.WriteLine(sb.ToString());
                writer.Close();
            }
            public static void WriteError(string fileName, string msg)
            {
                var writer = new StreamWriter(fileName, true);
                var t = DateTime.Now.ToString("yyMMdd_HHmmss");
                writer.WriteLine($"[{t}]\t<ERROR>\t{msg}");
                writer.Close();
            }
        }

        static AlignLogic()
        {
            var t = DateTime.Now.ToString("yyMMdd_HHmmss");
            mApproachLogFile = $@"log\{t}_approach.txt";
        }
        public static string mApproachLogFile;
        static Stopwatch mWatch = new Stopwatch();
        

        /// <summary>
        /// 양 스테이지 후퇴 후 순차 접촉/후퇴
        /// </summary>
        /// <param name="bufferDistance">접근 전 후퇴 거리</param>
        /// <param name="distance">접촉 후 후퇴거리</param>
        /// <returns></returns>
        public bool ApproachInOut(int bufferDistance, int distance, bool simultaneous = false)
        {
            mReporter?.Invoke($"AlignLogic.ApproachInOut(): {bufferDistance}:{distance}");

            bufferDistance = Math.Abs(bufferDistance);
            distance = Math.Abs(distance);

            mLeft.RelMove(mLeft.AXIS_Z, -bufferDistance);
            mRight.RelMove(mRight.AXIS_Z, -bufferDistance);
            mRight.WaitForIdle(mRight.AXIS_Z);
            if (mStopFlag) return false;

            //left stage approach
            Approach(AppStageId.Left, distance, bufferDistance);
            if (mStopFlag) return false;

            //right stage ZApproach
            Approach(AppStageId.Right, distance, bufferDistance);
            if (mStopFlag) return false;

            return true;
        }
        

        /// <summary>
        /// Z축 Approach (LEFT OR RIGHT / Input or Output) 
        /// </summary>
        /// <param name="_stageNo"> stage no. (LEFT OR RIGHT / Input or Output)</param>
        public void ZappSingleStage(int _stageNo)
        {
            var id = (LEFT_STAGE == _stageNo) ? AppStageId.Left : AppStageId.Right;
            mCompleted = mStopFlag = false;
            Approach(id, 0, 0);
        }
        
        //sensor parameters
        const double APPROACH_MOVE_STEP1 = 10;  //[um]
        const double APPROACH_MOVE_STEP2 = 1;  //[um]
        const int APPROACH_NUM_AVG = 1;
        const double CONT_VOLT_DIFF = 0.015; // [v] 접촉했음을 판단할 센서 변화량.
        const double MIN_START_VOLT = 2.0;//minimum valid voltage at starting position

        public void Approach(AppStageId stageId, int distance, int bufferDistance, bool monitor = true)
        {
            try
            {
                double moveStep;
                var msgHeader = $"AlignLogic.Approach().{stageId} : ";
                mReporter?.Invoke($"{msgHeader}Starting...");
                if (distance < 0) ApproachParam.WriteError(mApproachLogFile, $"{msgHeader} distance={distance}");

                //시작 ...
                mCompleted = mStopFlag = false;
                mWatch.Restart();

                Istage stage = (stageId == AppStageId.Left) ? mLeft : mRight;
                int stageNo = stage.stageNo;
                if(monitor) CurFuncNo = ZAPPROACH_SINGLE;
                CurStageNo = stageNo;

                //status 초기화.
                CzappStatus zappStatus = (stageId == AppStageId.Left) ? AlignStatusPool.zappIn : AlignStatusPool.zappOut;
                zappStatus.Clear();

                //190308 
                if (IsTestMode)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        stage.RelMove(stage.AXIS_Z, -10);
                        zappStatus.posList.Add(stage.GetAxisAbsPos(stage.AXIS_Z));

                        if(i==18) zappStatus.sensList.Add(4.0);
                        else if(i==19) zappStatus.sensList.Add(4.2);
                        else zappStatus.sensList.Add(4.5);
                        Thread.Sleep(100);
                        if (mStopFlag) break;
                    }

                    //이격 후 종료
                    stage.RelMove(stage.AXIS_Z, -distance, 1000);
                    stage.WaitForIdle(stage.AXIS_Z);

                    mCompleted = true;
                    return;
                }


                double minOpenVolt = 100;

                #region ---- STEP 0  ----

                //이동
                if (bufferDistance != 0)
                {
                    mReporter?.Invoke($"{msgHeader}Moving {-bufferDistance}");
                    stage.RelMove(stage.AXIS_Z, -bufferDistance);
                    stage.WaitForIdle(stage.AXIS_Z);
                }
                #endregion


                #region ------ STEP1 ------

                var param = new ApproachParam(stageId, ApproachPhase.A1);

                //시작 위치를 구한다.
                param.baseZ = Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), STGPOSXYZRES);
                zappStatus.pos = param.baseZ;
                zappStatus.posList.Add(param.baseZ);

                //Contact하지 않은 상태의  센서값을 구한다.
                double volt;
                while ((volt = readVoltage(stageNo, APPROACH_NUM_AVG)) < MIN_START_VOLT)
                {
                    var msg = $"Invalid sensor voltage :\nstage={stageNo}, phase={param.phase}, voltage={volt} < {MIN_START_VOLT}";
                    writeError(msg);
                }

                param.baseV = volt;
                zappStatus.noContSens = param.baseV;                
                if (minOpenVolt > param.baseV) minOpenVolt = param.baseV;

                var openVoltage1 = volt;

                mReporter?.Invoke($"{msgHeader}Step1 starting : open volt = {param.baseV:F03}");

                moveStep = 0;
                while (true)
                {
                    //이동
                    stage.RelMove(stage.AXIS_Z, APPROACH_MOVE_STEP1);
                    stage.WaitForIdle(stage.AXIS_Z);

                    //현재위치
                    moveStep += APPROACH_MOVE_STEP1;
                    zappStatus.pos = param.baseZ + moveStep;//Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), STGPOSXYZRES);
                    zappStatus.posList.Add(zappStatus.pos);
                    param.z.Add(zappStatus.pos);

                    //sensor voltage
                    volt = readVoltage(stageNo, APPROACH_NUM_AVG);
                    zappStatus.sens = volt;
                    zappStatus.sensList.Add(volt);
                    param.volt.Add(volt);
                    
                    //check contact 
                    var diff = Math.Round((double)(param.baseV - volt), 3);
                    if (diff >= CONT_VOLT_DIFF)
                    {
                        mReporter?.Invoke($"{msgHeader}Step1 ending : cantact volt = {volt:F03}");
                        break;
                    }

                    //정지?
                    if (mStopFlag)
                    {
                        mCompleted = true;
                        param.Write(mApproachLogFile);
                        return;
                    }
                }//step1

                param.Write(mApproachLogFile);
                mReporter?.Invoke($"Step1 end:{param.volt.Last():F03}");

                #endregion


                #region ------ STEP2 ------

                var openVolt1 = param.baseV;
                param = new ApproachParam(stageId, ApproachPhase.A2);

                //이동
                stage.RelMove(stage.AXIS_Z, -APPROACH_MOVE_STEP1);
                stage.WaitForIdle(stage.AXIS_Z);

                //현재위치
                param.baseZ = Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), STGPOSXYZRES);
                zappStatus.posList.Add(param.baseZ);
                zappStatus.pos = param.baseZ;

                //Contact하지 않은 상태의  센서값을 다시 구한다.
                while ((volt = readVoltage(stageNo, APPROACH_NUM_AVG)) < MIN_START_VOLT)
                {
                    var msg = $"Invalid sensor voltage :\nstage={stageNo}, phase={param.phase}, voltage={volt} < {MIN_START_VOLT}";
                    writeError(msg);
                }
                var openVolt2 = volt;
                param.baseV = openVolt2;                
                if (minOpenVolt > param.baseV) minOpenVolt = param.baseV;
                mReporter?.Invoke($"{msgHeader}Step2 starting : open volt = {param.baseV:F03}");

                //check open voltage 
                var openVoltDiff = openVolt2 - openVolt1;
                if (Math.Abs(openVoltDiff) > CONT_VOLT_DIFF)
                {
                    mReporter?.Invoke($"{msgHeader}Step2 : step12 open volt diff = {openVoltDiff:F03} > {CONT_VOLT_DIFF}");

                    minOpenVolt = openVoltage1;
                    writeError($"voltage1={openVolt1}, voltage2={openVolt2}, Δ={openVoltDiff:F03}, Zpos={param.baseZ:F01}");

                    //접촉 상태일 경우 - 후진
                    if (openVoltDiff < 0)
                    {
                        stage.RelMove(stage.AXIS_Z, -APPROACH_MOVE_STEP1 / 2);
                        stage.WaitForIdle(stage.AXIS_Z);
                    }

                    var z = Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), STGPOSXYZRES);
                    zappStatus.pos = z;
                    zappStatus.posList.Add(z);
                    param.baseZ = z;

                    volt = readVoltage(stageNo, APPROACH_NUM_AVG);
                    param.baseV = volt;
                    if (minOpenVolt > param.baseV) minOpenVolt = param.baseV;
                    mReporter?.Invoke($"{msgHeader}Step2 re-starting : open volt = {param.baseV:F03}");

                    //status - sensor
                    zappStatus.noContSens = volt;
                    zappStatus.sensList.Add(volt);
                }

                //------------- MOVE_STEP2 씩 이동
                moveStep = 0;
                while (true)
                {
                    //이동
                    stage.RelMove(stage.AXIS_Z, APPROACH_MOVE_STEP2);
                    stage.WaitForIdle(stage.AXIS_Z);

                    //현재위치
                    moveStep += APPROACH_MOVE_STEP2;
                    var z = param.baseZ + moveStep;//Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), STGPOSXYZRES);
                    zappStatus.pos = z;
                    zappStatus.posList.Add(z);
                    param.z.Add(z);

                    //sensor voltage
                    volt = readVoltage(stageNo, APPROACH_NUM_AVG);
                    zappStatus.sens = volt;
                    zappStatus.sensList.Add(volt);
                    param.volt.Add(volt);

                    //check contact 
                    var diff = Math.Round((double)(openVolt2 - volt), 3);//openVolt > v
                    if (diff >= CONT_VOLT_DIFF)
                    {
                        param.complete = true;
                        param.elapsed_ms = (int)mWatch.ElapsedMilliseconds;
                        mReporter?.Invoke($"{msgHeader}Step2 ending : cantact volt = {volt:F03}");
                        break;
                    }

                    //정지?
                    if (mStopFlag)
                    {
                        mCompleted = true;
                        param.Write(mApproachLogFile);
                        return;
                    }
                }
                param.Write(mApproachLogFile);

                #endregion


                //이격
                if (distance != 0)
                {
                    param = moveContactDistance(stageId, minOpenVolt, distance);
                    param.Write(mApproachLogFile);
                }
            }
            finally
            {
                //완료..
                mCompleted = true;
                //curAlignNo = NOOPERATION;
                mWatch.Stop();
            }
        }
        void writeError(string msg)
        {
            try
            {
                var w = new StreamWriter(mApproachLogFile, true);
                var t = DateTime.Now.ToString("yyMMdd-HHmmss.fff");
                w.WriteLine($"[{t}] {msg}");
                w.Close();
            }
            catch { }
        }

        private ApproachParam moveContactDistance(AppStageId stageId, double openVoltage, int distance_um)
        {
            var msgHeader = $"AlignLogic.Approach.{stageId} : ";
            mReporter?.Invoke($"{msgHeader}Step3 starting <{distance_um}>um");
            if (distance_um < 0) ApproachParam.WriteError(mApproachLogFile, $"{msgHeader} distance={distance_um}");

            Istage stage = (stageId == AppStageId.Left) ? mLeft : mRight;
            int stageNo = stage.stageNo;
            var param = new ApproachParam(stageId, ApproachPhase.A3);

            //시작 위치, 전압 기록
            var volt = readVoltage(stageNo, APPROACH_NUM_AVG);
            param.baseV = volt;
            param.baseZ = Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), 1);

            openVoltage -= 0.002;//1um = 10mV
            var numLoop = (int)Math.Abs(distance_um * 1.5);
            for (int j = 0; j <= numLoop; j++)//최대 1.5 distance_um 이동
            {
                //record coord
                param.z.Add(Math.Round(param.baseZ - j, 1));//stage.GetAxisAbsPos(stage.AXIS_Z), 1));

                //read voltage
                volt = readVoltage(stageNo, APPROACH_NUM_AVG);
                param.volt.Add(volt);

                //compare voltage
                if (volt >= openVoltage)
                {
                    mReporter?.Invoke($"{msgHeader}Step3 - open : z={param.z.Last():F0}, volt={volt:F03}");
                    break;
                }
                if (j < numLoop)
                {
                    stage.RelMove(stage.AXIS_Z, -1, 1000);
                    stage.WaitForIdle(stage.AXIS_Z);
                    Thread.Sleep(100);
                }
            }

            //CHECK
            var startingZ = param.z.Last();

            //이동
            stage.RelMove(stage.AXIS_Z, -distance_um, 1000);
            stage.WaitForIdle(stage.AXIS_Z);
            Thread.Sleep(100);

            //종료 위치, 전압 기록
            param.z.Add(Math.Round(stage.GetAxisAbsPos(stage.AXIS_Z), 1));
            volt = readVoltage(stageNo, APPROACH_NUM_AVG);
            param.volt.Add(volt);

            //error check
            var finalZ = param.z.Last();
            if(finalZ > startingZ)
                ApproachParam.WriteError(mApproachLogFile, $"{msgHeader} startingZ={startingZ}, finalZ={finalZ}");

            return param;
        }

        double readVoltage(int stageNo, int numAvg = 1)
        {
            var sensorId = (stageNo == mLeft.stageNo) ? SensorID.Left : SensorID.Right;
            var volt = 0.0;
            for (int i = 0; i < numAvg; i++) volt += mSensor.ReadDist(sensorId);
            return Math.Round((volt / numAvg), 3);
        }
        
    }//class

}
