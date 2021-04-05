using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Free302.MyLibrary.Utility;

namespace Neon.Aligner
{
    public class SurugaAlignerBase : IDisposable
    {
        public enum DriveSpeed { Low, High }
        public enum DriveStep { None, Full, Micro }

        // string mConfigFileName = @"config\conf_MotionController.xml";

        #region ==== Aligner & Stage Parameters ====

        protected int mNumAxis = 6;
        protected int mNumMc = 3;

        public CsurugaseikiMc[] mMC;
        
        public int[] mMcUnit;
        public double[] mStroke;
        public double[] mOriginCoord;
        public double[] mHomeCoord;
        public int[] mOriginReturnType;
        public double[] mResolution;//default resolution
        public double[] mResolutionLast;
        public int[] mDataNo;
        public int[][] mDivisionCode;
        public double[][][][] mSpeedTable;//[axis][step full~micro][speed low~high][values]
        public Func<int, int, int, int> mSpeedTableNumFunc = (axis, dataIndex, speedIndex) => (axis % 2) * 4 + dataIndex * 2 + speedIndex;


        public static int[] mMcAxisList;
        public static Dictionary<int, int> mAxisIndex;
        
        static SurugaAlignerBase()
        {
            mMcAxisList = new int[] { CsurugaseikiMc.MOTOR_AXIS_X, CsurugaseikiMc.MOTOR_AXIS_Y };

            mAxisIndex = new Dictionary<int, int>();
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_X, 0);
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_Y, 1);
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_Z, 2);
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_U, 3);
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_V, 4);
            mAxisIndex.Add(CsurugaseikiMc.MOTOR_AXIS_W, 5);

            initLog();
        }
        #endregion



        #region ==== Class Framework  ====

        public event Action<double[]> CoordUpdate;

        protected AlignerStateVector mState = new AlignerStateVector();
        protected DriveStep[] mLastStep;
        protected int[] mLastSpeedTable;
        protected double[] mCoord;

        public SurugaAlignerBase(int numAxis)
        {
            mNumAxis = numAxis;
            mNumMc = (numAxis + 1) / 2;
            mMC = new CsurugaseikiMc[mNumMc];
            mCoord = new double[numAxis];//last read coord from device      
            mLastStep = Enumerable.Repeat(DriveStep.None, numAxis).ToArray();
            mLastSpeedTable = Enumerable.Repeat(-1, numAxis).ToArray();
        }

        public void Dispose()
        {
            mLogWriter?.Close();
        }

        #endregion


        #region ==== MC Config ====


        private void loadConfig(string configFilePath)
        {
            var mConfig = new XConfig(configFilePath);

            mMcUnit = mConfig.GetValue("Unit").Unpack<int>().ToArray();
            mStroke = mConfig.GetValue("Stroke").Unpack<double>().ToArray();
            mOriginCoord = mConfig.GetValue("OriginPos").Unpack<double>().ToArray();
            mHomeCoord = mConfig.GetValue("HomePos").Unpack<double>().ToArray();
            mOriginReturnType = mConfig.GetValue("OriginReturnType").Unpack<int>().ToArray();

            mDivisionCode = new int[2][];
            mResolution = mConfig.GetValue("Resolution").Unpack<double>().ToArray();
            mResolutionLast = new double[mNumAxis];
            mResolution.CopyTo(mResolutionLast, 0);
            mDivisionCode[0] = mConfig.GetValue("Division_Data1").Unpack<int>().ToArray();
            mDivisionCode[1] = mConfig.GetValue("Division_Data2").Unpack<int>().ToArray();
            mDataNo = mConfig.GetValue("Division_DataNo").Unpack<int>().ToArray();

            try
            {

                string[] axisName = { "X", "Y", "Z", "TX", "TY", "TZ" };
                mSpeedTable = new double[mNumAxis][][][];
                for (int axis = 0; axis < mSpeedTable.Length; axis++)
                {
                    mSpeedTable[axis] = new double[2][][];
                    var strTables = mConfig.GetValue($"SpeedTable_{axisName[axis]}").Split('|');

                    //Data1
                    mSpeedTable[axis][0] = new double[2][];
                    for (int s = 0; s < 2; s++)
                        mSpeedTable[axis][0][s] = strTables[s].Unpack<double>().ToArray();
                    //Data2
                    mSpeedTable[axis][1] = new double[2][];
                    for (int s = 0; s < 2; s++)
                    {
                        mSpeedTable[axis][1][s] = new double[3];
                        for (int v = 0; v < 2; v++)
                            mSpeedTable[axis][1][s][v] = mSpeedTable[axis][0][s][v] * CsurugaseikiMc.DivisionValue[mDivisionCode[1][axis]];
                        mSpeedTable[axis][1][s][2] = mSpeedTable[axis][0][s][2];
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }

        public bool InitMc(int[] comPort, string configFile = null)
        {
            if (configFile != null) loadConfig(configFile);

            var num = Math.Min(mNumMc, comPort.Length);
            for (int m = 0; m < num; m++)
            {
                if (mMC[m] == null || !mMC[m].IsConnectedOK())
                {
                    var mc = new CsurugaseikiMc();
                    var isConnected = mc.ConnectByUSB_RS232(comPort[m]);
                    if (!isConnected) throw new Exception($"MC port={comPort[m]} 연결실패");
                    mMC[m] = mc;
                }
            }

            //
            for (int x = 0; x < mNumAxis; x++)
            {
                var mc = mMC[x / 2];
                var axis = mMcAxisList[x % 2];

                //단위설정 
                var r = mc.SetUnit(axis, mMcUnit[x]);
                if (!r) throw new Exception($"{nameof(InitMc)}: CsurugaseikiMc.SetUnit({mMcUnit[x]}) failed");

                //Speed Table value
                for (int d = 0; d < 2; d++)//division: data1, data2
                {
                    for (int s = 0; s < 2; s++)//speed: low, high
                    {
                        var st = new CsurugaseikiMc.CSpeedTable();
                        st.tableNo = mSpeedTableNumFunc(x, d, s);//(axis % 2) * 4 + d * 2 + s;
                        st.startVelocity = mSpeedTable[x][d][s][0];
                        st.drivingSpeed = mSpeedTable[x][d][s][1];
                        st.accDeaccRate = mSpeedTable[x][d][s][2];
                        r = mc.SetSpeedTable(st);
                        if (!r) throw new Exception($"{nameof(InitMc)}: CsurugaseikiMc.SetSpeedTable() failed");
                        if (x%2 == 0 && d == 1 && s == 1)
                        {
                            st.tableNo = 9;
                            st.drivingSpeed = 999999;
                            r = mc.SetSpeedTable(st);
                            if (!r) throw new Exception($"{nameof(InitMc)}: CsurugaseikiMc.SetSpeedTable() failed");
                        }
                    }
                }

                

                //step
                var step = mDataNo[x] == CsurugaseikiMc.DATA1 ? DriveStep.Full : DriveStep.Micro;
                setDriveStep(x, step);

                //stage & sensor parameter
                var newMss = new CsurugaseikiMc.CMemorySwitchState();
                newMss.axis = axis;
                newMss.sw0 = mOriginReturnType[x];                  //Origin retrun type
                newMss.sw1 = 0;                                     //Limit sensor input logic setting 
                newMss.sw2 = 0;                                     //Oring sensor input logic setting 
                newMss.sw3 = 0;                                     //Near origin  input logic setting
                mc.SetMemSwitchState(newMss);

               

                //soft limit
                var sl = new CsurugaseikiMc.CSoftLimit();
                sl.axis = mMcAxisList[x % 2];
                sl.CCWPosition = 0;
                sl.CWPosition = mStroke[x];
                sl.CCWEnable = false;
                sl.CWEnable = false;
                mc.SetSoftLimit(sl);
            }
            ReadCoord();

            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisIndex">X,Y,Z... ~ 0,1,2...</param>
        /// <param name="step"></param>
        private void setDriveStep(int axisIndex, DriveStep step)
        {
            var mc = mMC[axisIndex / 2];
            var axis = mMcAxisList[axisIndex % 2];
            var dataIndex = step == DriveStep.Micro ? 1 : 0;
            var dataNo = step == DriveStep.Micro ? CsurugaseikiMc.DATA2 : CsurugaseikiMc.DATA1;

            if (mLastStep[axisIndex] == step) return;

            //data No. 변경.
            var r = mc.SetAxisDataNo(axis, dataNo);
            if (!r) throw new Exception($"{nameof(setDriveStep)}: CsurugaseikiMc.SetAxisDataNo({axis}, {dataNo}) failed");

            //Driver Division 
            var divisionCode = mDivisionCode[dataIndex][axisIndex];
            r = mc.SetDrverDivision(axis, divisionCode);
            if (!r) throw new Exception($"{nameof(setDriveStep)}: CsurugaseikiMc.SetDrverDivision({axis}, {divisionCode}) failed");

            mLastStep[axisIndex] = step;

            //Standard Resolution
            ReadCoord();
            var divisionValue = CsurugaseikiMc.DivisionValue[divisionCode];
            //var divisionValue = (mDataNo[axisIndex] == 1) ? 1 : 20;
            var resolution = Math.Round(mResolution[axisIndex] / divisionValue,5, MidpointRounding.AwayFromZero);
            r = mc.SetStandardResolution(axis, resolution);
            if (!r) throw new Exception($"{nameof(setDriveStep)}: CsurugaseikiMc.SetStandardResolution({axis}, {resolution}) failed");

            mResolutionLast[axisIndex] = resolution;

            //Home position 설정
            mc.SetHomePosition(axis, mHomeCoord[axisIndex]);//mHomePos[axis]);

            //clear Constant Step Pulse
            mc.SetConstantStepPulse(axis, 99999999);

            ///***********************************
            ///StandardResolution을 바꾸면 MC가 보고하는 현재좌표 값이 바뀐다.
            /// 현재좌표값 재조정 필요
            mc.SetCurrentPosition(axis, mCoord[axisIndex]);

            //reset speed table to high
            setSpeed(axisIndex, DriveSpeed.High, step);
        }

        void setSpeed(int axisIndex, DriveSpeed speed = DriveSpeed.High, DriveStep step = DriveStep.Full)
        {
            var mc = mMC[axisIndex / 2];
            var axis = mMcAxisList[axisIndex % 2];
            var dataIndex = step == DriveStep.Micro ? 1 : 0;
            var speedIndex = speed == DriveSpeed.High ? 1 : 0;
            var tableNo = mSpeedTableNumFunc(axisIndex, dataIndex, speedIndex);

            if (tableNo == mLastSpeedTable[axisIndex]) return;

            var r = mc.SetSpeedTableNo(axis, tableNo);
            if (!r) throw new Exception($"{nameof(setSpeed)}: CsurugaseikiMc.SetSpeedTableNo({axis}, {tableNo}) failed");

            mLastSpeedTable[axisIndex] = tableNo;
        }

        #endregion



        #region ==== MC API ====


        public void ReadCoord()
        {
            //read current coordinates
            for (int x = 0; x < mNumAxis; x++)
            {
                mCoord[x] = mMC[x / 2].GetCurrentPosition(mMcAxisList[x % 2]);
            }
            //CoordUpdate?.Invoke(mCoord);//***** async?? ui thread??
        }

        public double ReadCoord(int axisIndex)
        {
            mCoord[axisIndex] = mMC[axisIndex / 2].GetCurrentPosition(mMcAxisList[axisIndex % 2]);
            //CoordUpdate?.Invoke(mCoord);//***** async?? ui thread??
            return mCoord[axisIndex];
        }


        public void WriteCoord(double[] coord_um)
        {
            //read current coordinates
            for (int x = 0; x < mNumAxis; x++)
            {
                var axis = mMcAxisList[x % 2];
                var r = mMC[x / 2].SetCurrentPosition(axis, coord_um[x]);
                if (!r) throw new Exception($"{nameof(setSpeed)}: CsurugaseikiMc.WriteCoord({axis}, {coord_um[x]}) failed");
            }
            ReadCoord();
            CoordUpdate?.Invoke(mCoord);//***** async?? ui thread??
        }

        public void Origin()
        {
            if (mState.IsRunning) return;
            try
            {
                mState.Increse();
                var lastStep = new DriveStep[mLastStep.Length];
                mLastStep.CopyTo(lastStep, 0);
                //step 설정
                for (int x = 0; x < mNumAxis; x++) setDriveStep(x, DriveStep.Full);

                //모든 Axis를 Origin으로 이동시킨다.
                for (int m = 0; m < mNumMc; m++) mMC[m].Async_Go(CsurugaseikiMc.MOTOR_AXIS_ALL, CsurugaseikiMc.DIRECTION_ORIGIN);
                for (int m = 0; m < mNumMc; m++) while (mMC[m].IsInMotionOK()) ;
                Thread.Sleep(100);

                //step 설정
                for (int x = 0; x < mNumAxis; x++) setDriveStep(x, lastStep[x]);

                //현재 위치를 0으로 설정
                for (int x = 0; x < mNumAxis; x++) mMC[x / 2].SetCurrentPosition(mMcAxisList[x % 2], mOriginCoord[x]);
                ReadCoord();

            }
            finally
            {
                mState.Decrese();
            }
        }
        public void Origin(int axisIndex)
        {
            if (mState.IsRunning) return;
            try
            {
                mState.Increse();

                //step 설정
                setDriveStep(axisIndex, mLastStep[axisIndex]);

                var mc = mMC[axisIndex / 2];
                var axis = mMcAxisList[axisIndex % 2];
                mc.Async_Go(axis, CsurugaseikiMc.DIRECTION_ORIGIN);
                while (mc.IsInMotionOK()) ;
                Thread.Sleep(100);

                //현재 위치 설정
                mc.SetCurrentPosition(axis, mOriginCoord[axisIndex]);

                //좌표 확인
                ReadCoord();
            }
            finally
            {
                mState.Decrese();
            }
        }

        public void MoveToHome()
        {
            if (mState.IsRunning) return;
            try
            {
                mState.Increse();

                //모든 Axis를 Homming으로 이동시킨다.
                for (int m = 0; m < mNumMc; m++) mMC[m].Async_Go(CsurugaseikiMc.MOTOR_AXIS_ALL, CsurugaseikiMc.DIRECTION_HOME);
                for (int m = 0; m < mNumMc; m++) while (mMC[m].IsInMotionOK()) ;
                Thread.Sleep(100);

                //update position.
                ReadCoord();
            }
            finally
            {
                mState.Decrese();
            }
        }
        public void MoveToHome(int axisIndex)
        {
            if (mState.IsRunning) return;
            try
            {
                mState.Increse();

                var mc = mMC[axisIndex / 2];

                mc.Async_Go(CsurugaseikiMc.MOTOR_AXIS_ALL, CsurugaseikiMc.DIRECTION_HOME);
                while (mc.IsInMotionOK()) ;
                Thread.Sleep(100);

                //update position.
                ReadCoord();
            }
            finally
            {
                mState.Decrese();
            }
        }


        public void Abort()
        {
            mState.DoAbort = true;
        }

        bool doCanMoveAs(int axisIndex, double displacement)
        {
            //var newCoord = ReadCoord(axisIndex) + displacement;
            var newCoord = mCoord[axisIndex] + displacement;
            var can =  newCoord >= 0 && newCoord <= mStroke[axisIndex];

            if (!can) writeLog($"AxisIndex={axisIndex}, displacement={displacement}, current coord={mCoord[axisIndex]}: Can not move");

            return can;
        }


        Stopwatch mWatch = Stopwatch.StartNew();
        List<Tuple<string, long>> mTiming = new List<Tuple<string, long>>();
        void recordTiming(string actionName)
        {
            mTiming.Add(new Tuple<string, long>(actionName, mWatch.ElapsedMilliseconds));
            mWatch.Restart();
        }


        public void MoveAs(int axisIndex, double displacement, DriveSpeed speed = DriveSpeed.High)
        {
            if (mState.IsRunning) return;

            mTiming.Clear();
            var time = DateTime.Now.ToString("yyMMMdd_HHmmss");
            mWatch.Restart();
            recordTiming($"[{time}] MoveAs(): axisIndex={axisIndex}, dx={displacement}");

            try
            {
                mState.Increse();
                recordTiming("mState.Increse()");

                if (!doCanMoveAs(axisIndex, displacement)) return;
                recordTiming("doCanMoveAs()");

                var currentCoord = mCoord[axisIndex];
                var mc = mMC[axisIndex / 2];
                var mcAxis = mMcAxisList[axisIndex % 2];

                //division 
                setDriveStep(axisIndex, mLastStep[axisIndex]);
                recordTiming("setDriveStep()");

                //Speed 설정!!
                setSpeed(axisIndex, speed, mLastStep[axisIndex]);
                recordTiming("setSpeed()");

                //distance & Direction
                int nDir = (displacement > 0) ? CsurugaseikiMc.DIRECTION_CW : CsurugaseikiMc.DIRECTION_CCW;

                var multiple = Math.Truncate(Math.Abs(displacement) / mResolutionLast[axisIndex]);
                displacement = Math.Round(multiple * mResolutionLast[axisIndex], 5);

                mc.SetConstantStepPulse(mcAxis, displacement);
                recordTiming("mc.SetConstantStepPulse()");

                mc.Async_Go(mcAxis, nDir);
                recordTiming("mc.Async_Go()");

                ////position 계산..
                mCoord[axisIndex] += displacement;

                //while (true)
                //{
                //    ReadCoord();

                //    var dx = Math.Abs(mCoord[axisIndex] - currentCoord);
                //    var diff = (Math.Abs(dx - Math.Abs(displacement)));
                //    if (axisIndex < 3)
                //    {
                //        if (diff <= mResolutionLast[axisIndex] * 0.1) break;
                //        //if (dx >= Math.Abs(displacement)) break;
                //    }
                //    else
                //    {
                //        if (diff <= mResolutionLast[axisIndex] * 3) break;
                //    }
                //    if (mState.DoAbort) break;

                //    Task.Delay(50).Wait();

                //}
            }
            catch(Exception ex)
            {
                var a = ex;
            }
            finally
            {
                mState.Decrese();
                recordTiming("mState.Decrese()");

                var len = mTiming.Count;
                var sb = new StringBuilder();
                for (int i = 0; i < len; i++) sb.AppendLine($"{mTiming[i].Item1}:\t{mTiming[i].Item2:D03}");
                writeLog(sb.ToString());
            }
        }

        #endregion



        #region ==== Log ====

        static StreamWriter mLogWriter;// = new StreamWriter(@"log\log_SurugaAligner.txt", true);
        const string mLogDir = "log";
        const string mLogFileName = "log_SurugaAligner.txt";

        static void initLog()
        {
            try
            {
                if (!Directory.Exists(mLogDir)) Directory.CreateDirectory(mLogDir);
                mLogWriter = new StreamWriter(Path.Combine(mLogDir, mLogFileName), true);
            }
            catch(Exception ex)
            {
                Log.Write($"static SurugaAlignerBase():\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        static void writeLog(string msg, bool recordTime = true)
        {
            lock (mLogWriter)
            {
                if (recordTime)
                {
                    var now = DateTime.Now;
                    var time = now.ToString("yyMmdd-HHmmss");

                    mLogWriter?.WriteLine($"[{time}.{now.Millisecond:D03}] {msg}");
                }
                else
                {
                    mLogWriter?.WriteLine(msg);
                }
                mLogWriter?.Flush();
            }
        }

        #endregion



        

    }//class
}
