using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Free302.TnM.DataAnalysis;
//using Free302.TnMLibrary.DataAnalysis;

namespace Neon.Aligner
{

    public partial class AlignLogic
    {
        #region definition

        private const int OPTPWRRES = 9;    //광파워 표현 reolution (1.0*10^-9 mW)
        private const int STGPOSXYZRES = 2;    //스테이지 좌표 resolution (1.0*10^-2 um)
        private const int STGPOSUVWRES = 4;    //스테이지 좌표 resolution (1.0*10^-4 degree)
        private const int DISTSENSRES = 3;  //Distance sensor resolution (1.0*10^-3 volt)

        public const int LEFT_STAGE = (int)IFA_AlignerId.Left; //mLeft?.stageNo ?? 0;//(int)IFA_AlignerId.Left; 
        public const int RIGHT_STAGE = (int)IFA_AlignerId.Right;// mRight?.stageNo ?? 0;//
        public const int LEFTRIGHT_STAGE = LEFT_STAGE+ RIGHT_STAGE;
        public const int STAGE_L = LEFT_STAGE;
        public const int STAGE_R = RIGHT_STAGE;
        public const int STAGE_LR = LEFTRIGHT_STAGE;


        public const int NOOPERATION = 0;
        public const int ZAPPROACH_SINGLE = 100;
        public const int ZAPPROACH_DUAL = 101;
        public const int ANGLE_TX_SINGLE = 102;
        public const int ANGLE_TY_SINGLE = 104;

        public const int XY_SEARCH = 200;
        public const int XY_SEARCH_BOTH = 210;
        public const int XYBLINDSEARCH = 201;
        public const int XYFULLBLINDSEARCH = 208;
        public const int AXISSEARCH = 202;
        public const int SYNCXYSEARCH = 204;
        public const int SYNCAXISSEARCH = 205;
        public const int ROLLOUT = 207;

        public const int FabAuto_Left = 301;
        public const int FabAuto_Right = 302;
        public const int FabAuto_Both = 300;
        public const int FabAuto_AppAndBack = 303;
        public const int FabAuto_AppAndBack_Both = 304;
        public const int FabAuto_BondingAlign = 305;

        #endregion



        #region structure/innor class


        /// <summary>
        /// XyFullBlindSearch parameters.
        /// </summary>
        private struct XfbParam
        {
            public double rngIn;
            public double stepIn;
            public double rngOut;
            public double stepOut;
        }


        private struct XfbInfor
        {
            public DateTime startTime;
            public DateTime endTime;
            public XfbParam param;
            public CalignPoint2d startPosIn;
            public CalignPoint2d startPosOut;
            public CalignPoint2d endPosIn;
            public CalignPoint2d endPosOut;
            public int searchCntIn;
            public int searchCntOut;
        }



        private struct PosPwr1d
        {
            public double pos; //position[um]
            public double pwr; //optial power[mw]
        }

        //private struct PosPwr2d
        //{
        //    public double x;   //x-axis position[um]
        //    public double y;   //y-axis position[um]
        //    public double pwr;    //optial power[mw]
        //}


        private struct InOutPosPwr
        {
            public double posInX;
            public double posInY;
            public double posOutX;
            public double posOutY;
            public double pwr;
            public double time;
        }

        #endregion



        #region member Variables

        private Istage mLeft;
        private Istage mRight;
        //private Istage mOther;
        private IDispSensor mSensor;
        private IoptMultimeter mPm;


        #endregion



        #region constructor/desconstructor

        //constructor
        public AlignLogic(Istage _leftStage, Istage _rightStage, IDispSensor _distSens, IoptMultimeter _mpm)
        {
            mLeft = _leftStage;
            mRight = _rightStage;
            mSensor = _distSens;
            mPm = _mpm;

            mStopFlag = false;
            mCompleted = true;
            _currentInstance = this;

            initAngleStatus();

        }

        static AlignLogic _currentInstance;

        #endregion




        #region Private method



        /// <summary>
        /// XyFullBlindSearch 관련 정보를 파일에 저장한다.
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private bool SaveXfbInfor(XfbInfor _info)
        {

            bool ret = false;



            FileStream fs = null;
            StreamWriter sw = null;


            try
            {


                //File Open
                string folerPath = "";
                folerPath = "xyFullBlind";
                if (!Directory.Exists(folerPath)) Directory.CreateDirectory(folerPath);

                string filepath = "";
                filepath = folerPath + "\\xfbInfo.txt";

                bool newFile = false;
                if (System.IO.File.Exists(filepath))
                {
                    fs = new FileStream(filepath, FileMode.Append);
                    newFile = false;
                }
                else
                {
                    fs = new FileStream(filepath, FileMode.Create);
                    newFile = true;
                }
                sw = new StreamWriter(fs);


                //column
                string strLineBuf = "";
                if (newFile == true)
                {
                    strLineBuf = "startTime, endTime, processTime, ";
                    strLineBuf += "rngIn, stepIn, rngOut, stepOut,  ";
                    strLineBuf += "startPosInX, startPosInY, endPosOutX, endPosOutY, ";
                    strLineBuf += "countIn, countOut ";
                    sw.WriteLine(strLineBuf);
                }


                //data.
                string strTemp = "";  //startTime
                strTemp = string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                                        _info.startTime.Year,
                                        _info.startTime.Month,
                                        _info.startTime.Day,
                                        _info.startTime.Hour,
                                        _info.startTime.Minute,
                                        _info.startTime.Second);
                strLineBuf = strTemp + ",";

                strTemp = "";  //endTime
                strTemp = string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}",
                                        _info.endTime.Year,
                                        _info.endTime.Month,
                                        _info.endTime.Day,
                                        _info.endTime.Hour,
                                        _info.endTime.Minute,
                                        _info.endTime.Second);
                strLineBuf += strTemp + ",";

                strTemp = "";  //process time
                TimeSpan ts = _info.endTime - _info.startTime;
                strTemp = Math.Round(ts.TotalSeconds, 2).ToString();
                strLineBuf += strTemp + ",";


                strTemp = "";  //searching range input side.
                strTemp = Math.Round(_info.param.rngIn, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //searching step input side.
                strTemp = Math.Round(_info.param.stepIn, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //searching range output side.
                strTemp = Math.Round(_info.param.rngOut, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //searching step output side.
                strTemp = Math.Round(_info.param.stepOut, 2).ToString();
                strLineBuf += strTemp + ",";


                strTemp = "";  //start position input X
                strTemp = Math.Round(_info.startPosIn.x, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //start position input y
                strTemp = Math.Round(_info.startPosIn.y, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //start position output X
                strTemp = Math.Round(_info.startPosOut.x, 2).ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //start position output y
                strTemp = Math.Round(_info.startPosOut.y, 2).ToString();
                strLineBuf += strTemp + ",";


                strTemp = "";  //count input side.
                strTemp = _info.searchCntIn.ToString();
                strLineBuf += strTemp + ",";

                strTemp = "";  //count output side.
                strTemp = _info.searchCntOut.ToString();
                strLineBuf += strTemp;
                sw.WriteLine(strLineBuf);



                ret = true;

            }
            catch
            {
                ret = false;
            }
            finally
            {
                //File close
                if (sw != null)
                    sw.Close();

                if (fs != null)
                    fs.Close();
            }


            return ret;

        }




        private bool SaveXfbPosPwr(List<InOutPosPwr> _sd)
        {
            bool ret = false;

            string strLineBuf = "";
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {


                //File Open
                string folerPath = "";
                folerPath = "xyFullBlind";
                if (!Directory.Exists(folerPath)) Directory.CreateDirectory(folerPath);

                string filepath = "";
                DateTime curTime = DateTime.Now;
                filepath += "FullBlindSearch_";
                filepath += string.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}",
                                        curTime.Year,
                                        curTime.Month,
                                        curTime.Day,
                                        curTime.Hour,
                                        curTime.Minute,
                                        curTime.Second);
                filepath += ".txt";
                filepath = folerPath + "\\" + filepath;

                if (filepath == "")
                    throw new ApplicationException("");

                fs = new FileStream(filepath, FileMode.Create);
                sw = new StreamWriter(fs);


                //column
                strLineBuf = "inX,InY,OutX,OutY,power,time";
                sw.WriteLine(strLineBuf);


                //data
                strLineBuf = "";
                for (int i = 0; i < _sd.Count(); i++)
                {
                    strLineBuf = Convert.ToString(Math.Round(_sd[i].posInX, 2)) + ',';
                    strLineBuf += Convert.ToString(Math.Round(_sd[i].posInY, 2)) + ',';
                    strLineBuf += Convert.ToString(Math.Round(_sd[i].posOutX, 2)) + ',';
                    strLineBuf += Convert.ToString(Math.Round(_sd[i].posOutY, 2)) + ',';
                    strLineBuf += Convert.ToString(_sd[i].pwr) + ',';
                    strLineBuf += Convert.ToString(_sd[i].time);

                    sw.WriteLine(strLineBuf);
                }


                ret = true;

            }
            catch
            {
                ret = false;
            }
            finally
            {
                //File close
                if (sw != null)
                    sw.Close();

                if (fs != null)
                    fs.Close();
            }

            return ret;

        }



        /// <summary>
        /// 나선형으로 회전하면서 광을 찾는다.
        /// </summary>
        /// <param name="_stageNo">Left or Right</param>
        /// <param name="_detectPort"></param>
        /// <param name="_rng">검색 범위 [um]</param>
        /// <param name="_step">스캔 스탭 [um]</param>
        /// <param name="_thresh">dbThresholdPwr : 광을 찾을 찾았다고 보는 광파워 [dBm]</param>
        /// 
        private bool XyBlindSearchsBySpiral(int _stgNo, int _port, double _rng, double _step, double _thres)
        {

            bool ret = false;

            try
            {
                //status 초기화.
                CsearchStatus state = null;
                if (_stgNo == LEFT_STAGE)
                    state = AlignStatusPool.xyBlindSearchIn;
                else
                    state = AlignStatusPool.xyBlindSearchOut;
                state.pos.x = 0;
                state.pos.y = 0;
                state.pwr = 0;
                state.posList.Clear();
                state.pwrList.Clear();


                //Stage
                Istage stage = null;
                if (_stgNo == LEFT_STAGE)
                    stage = mLeft;
                else
                    stage = mRight;


                //Alignment Threshold power.
                double dbAlignThresPwr = Unit.Dbm2MilliWatt(_thres);//Unit.Dbm2MilliWatt(_thres); //[mW]
                dbAlignThresPwr = Math.Round(dbAlignThresPwr, OPTPWRRES);


                //시작 위치를 구한다.
                CalignPoint2d startPos = new CalignPoint2d();
                startPos.x = stage.GetAxisAbsPos(stage.AXIS_X);
                startPos.y = stage.GetAxisAbsPos(stage.AXIS_Y);

                state.pos = new CalignPoint2d(startPos.x, startPos.y);
                state.posList.Add(startPos);


                //현재 광파워를 구한다.[mW]
                double dbOrgPwr = mPm.ReadPower(_port);
                dbOrgPwr = Math.Round(dbOrgPwr, OPTPWRRES);
                state.pwr = dbOrgPwr;
                state.pwrList.Add(dbOrgPwr);

                //Searching~~~
                bool bBlindAlignOK = false;
                double dbCurPwr = dbOrgPwr;
                CalignPoint2d curPos = new CalignPoint2d(startPos.x, startPos.y);
                CalignPoint2d maxPos = new CalignPoint2d(startPos.x, startPos.y);
                int maxLoopCnt = Convert.ToInt32(_rng / _step);
                for (int i = 1; i <= maxLoopCnt; i++)
                {

                    for (int nAxis = stage.AXIS_X; nAxis <= stage.AXIS_Y; nAxis++)
                    {
                        for (int j = 1; j <= i; j++)
                        {

                            //현재 광파워를 읽는다. [mW]
                            dbCurPwr = mPm.ReadPower(_port);
                            dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                            state.pwr = dbCurPwr;
                            state.pwrList.Add(dbCurPwr);


                            //일정 광파워 이상이면 멈춤. 
                            if (dbCurPwr >= dbAlignThresPwr)
                            {
                                //Alignment 성공.!!//
                                stage.StopMove(nAxis);
                                bBlindAlignOK = true;
                                break;
                            }


                            //이동.
                            stage.RelMove(nAxis, (_step * (Math.Pow((-1), i))));
                            stage.WaitForIdle(nAxis);


                            //현재 위치 계산.
                            if (nAxis == stage.AXIS_X)
                                curPos.x += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            else
                                curPos.y += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            state.pos.x = curPos.x;
                            state.pos.y = curPos.y;
                            state.posList.Add(new CalignPoint2d(curPos.x, curPos.y));

                            //사용자의 정지명령?
                            if (mStopFlag == true)
                            {
                                stage.StopMove(nAxis);
                                mCompleted = true;
                                return false;
                            }

                        }


                        if (bBlindAlignOK == true)
                            break;

                    }


                    if (bBlindAlignOK == true)
                        break;

                }


                //Thres.power를 넘지 못했다면 시작위치로 돌아간다.
                if (bBlindAlignOK == false)
                {
                    stage.AbsMove(stage.AXIS_X, startPos.x);
                    stage.AbsMove(stage.AXIS_Y, startPos.y);
                    stage.WaitForIdle(stage.AXIS_Y);
                }

                ret = bBlindAlignOK;

            }
            catch
            {
                ret = false;
            }

            return ret;
        }






        /// <summary>
        /// 나선형으로 회전하면서 광을 찾는다.
        /// </summary>
        /// <param name="_stageNo">Left or Right</param>
        /// <param name="_detectPort"></param>
        /// <param name="_rng">검색 범위 [um]</param>
        /// <param name="_step">스캔 스탭 [um]</param>
        /// <param name="_thresh">dbThresholdPwr : 광을 찾을 찾았다고 보는 광파워 [dBm]</param>
        /// <param name="_searchCnt"> [out] searching count</param>
        private bool XyBlindSearchsBySpiral(int _stgNo,
                                            int _port,
                                            double _rng,
                                            double _step,
                                            double _thres,
                                            ref int _searchCnt)
        {

            bool ret = false;

            try
            {
                //status 초기화.
                CsearchStatus state = null;
                if (_stgNo == LEFT_STAGE)
                    state = AlignStatusPool.xyBlindSearchIn;
                else
                    state = AlignStatusPool.xyBlindSearchOut;
                state.pos.x = 0;
                state.pos.y = 0;
                state.pwr = 0;
                state.posList.Clear();
                state.pwrList.Clear();


                //Stage
                Istage stage = null;
                if (_stgNo == LEFT_STAGE)
                    stage = mLeft;
                else
                    stage = mRight;


                //Alignment Threshold power.
                double dbAlignThresPwr = Unit.Dbm2MilliWatt(_thres); //[mW]
                dbAlignThresPwr = Math.Round(dbAlignThresPwr, OPTPWRRES);


                //시작 위치를 구한다.
                CalignPoint2d startPos = new CalignPoint2d();
                startPos.x = stage.GetAxisAbsPos(stage.AXIS_X);
                startPos.y = stage.GetAxisAbsPos(stage.AXIS_Y);

                state.pos = new CalignPoint2d(startPos.x, startPos.y);
                state.posList.Add(startPos);


                //현재 광파워를 구한다.[mW]
                double dbOrgPwr = mPm.ReadPower(_port);
                dbOrgPwr = Math.Round(dbOrgPwr, OPTPWRRES);
                state.pwr = dbOrgPwr;
                state.pwrList.Add(dbOrgPwr);


                //Searching~~~
                _searchCnt = 0;
                bool bBlindAlignOK = false;
                double dbCurPwr = dbOrgPwr;
                CalignPoint2d curPos = new CalignPoint2d(startPos.x, startPos.y);
                CalignPoint2d maxPos = new CalignPoint2d(startPos.x, startPos.y);
                int maxLoopCnt = Convert.ToInt32(_rng / _step);
                for (int i = 1; i <= maxLoopCnt; i++)
                {

                    for (int nAxis = stage.AXIS_X; nAxis <= stage.AXIS_Y; nAxis++)
                    {
                        for (int j = 1; j <= i; j++)
                        {

                            //현재 광파워를 읽는다. [mW]
                            dbCurPwr = mPm.ReadPower(_port);
                            dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                            state.pwr = dbCurPwr;
                            state.pwrList.Add(dbCurPwr);


                            //일정 광파워 이상이면 멈춤. 
                            if (dbCurPwr >= dbAlignThresPwr)
                            {
                                //Alignment 성공.!!//
                                stage.StopMove(nAxis);
                                bBlindAlignOK = true;
                                break;
                            }


                            //이동.
                            stage.RelMove(nAxis, (_step * (Math.Pow((-1), i))));
                            stage.WaitForIdle(nAxis);
                            _searchCnt++;


                            //현재 위치 계산.
                            if (nAxis == stage.AXIS_X)
                                curPos.x += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            else
                                curPos.y += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            state.pos.x = curPos.x;
                            state.pos.y = curPos.y;
                            state.posList.Add(new CalignPoint2d(curPos.x, curPos.y));


                            //사용자의 정지명령?
                            if (mStopFlag == true)
                            {
                                stage.StopMove(nAxis);
                                mCompleted = true;
                                return false;
                            }

                        }


                        if (bBlindAlignOK == true)
                            break;

                    }


                    if (bBlindAlignOK == true)
                        break;

                }

                //Thres.power를 넘지 못했다면 시작위치로 돌아간다.
                if (bBlindAlignOK == false)
                {
                    stage.AbsMove(stage.AXIS_X, startPos.x);
                    stage.AbsMove(stage.AXIS_Y, startPos.y);
                    stage.WaitForIdle(stage.AXIS_Y);
                }

                ret = bBlindAlignOK;

            }
            catch
            {
                ret = false;
            }

            return ret;
        }













        /// <summary>
        /// center position를  계산한다.
        /// 광파워가 가우시안 분포를 띈다고 가정하고 계산.
        /// </summary>
        /// <param name="_pwrList"></param>
        /// <param name="_step">데이터의 간격 [um] == Searching step</param>
        /// <param name="_pwr">center에 position에 광파워.</param>
        /// <returns></returns>
        public static double CalcCtrPos(List<double> _pwrList, double _step)
        {
            if (_pwrList == null) return 0;
            if (_pwrList.Count == 0) return 0;

            double ret = 0;


            //Peak power and its index.
            double peakPwr = Math.Round(_pwrList.Max(), OPTPWRRES); //[mW]
            double PeakPwrDbm = Unit.Dbm2MilliWatt(peakPwr);         //[dBm]
            int peakIdx = _pwrList.IndexOf(peakPwr);


            try
            {

                double pos1 = 0.0;
                double pos2 = 0.0;
                double pwr1 = 0.0;
                double pwr2 = 0.0;


                //standard = 3dB 
                double stdPwrDbm = PeakPwrDbm - 3; //[dBm]
                double stdPwr = Unit.Dbm2MilliWatt(stdPwrDbm);   //[mW];
                stdPwr = Math.Round(stdPwr, OPTPWRRES);


                //--- short ----//
                double stdPos1 = 0.0;
                int shortIdx = peakIdx - 1;
                while (true)
                {
                    if (shortIdx <= 0)
                        throw new ApplicationException();

                    //List에 존재하는 경우//
                    if (Math.Round(_pwrList[shortIdx], OPTPWRRES) == Math.Round(stdPwr, OPTPWRRES))
                        break;

                    //배열에 존재하지 않는 경우//
                    if (Math.Round(_pwrList[shortIdx], OPTPWRRES) < Math.Round(stdPwr, OPTPWRRES))
                        break;

                    shortIdx--;
                }
                pos1 = Math.Round(shortIdx * _step, STGPOSXYZRES);
                pos2 = Math.Round(pos1 + _step, STGPOSXYZRES);
                pwr1 = _pwrList[shortIdx];
                pwr2 = _pwrList[shortIdx + 1];
                //stdPos1 = JeffMath.LinearInterpolation(pwr1, pos1, pwr2, pos2, stdPwr);
                stdPos1 = DataProcessingLogic.Interpolate_1st(pwr1, pwr2, pos1, pos2, stdPwr);
                stdPos1 = Math.Round(stdPos1, 1);


                //--- long ----//
                double stdPos2 = 0.0;
                int longIdx = peakIdx + 1;
                while (true)
                {
                    if (longIdx == (_pwrList.Count() - 1))
                        break;

                    //List에 존재하는 경우//
                    if (Math.Round(_pwrList[longIdx], OPTPWRRES) == Math.Round(stdPwr, OPTPWRRES))
                        break;

                    //배열에 존재하지 않는 경우//
                    if (Math.Round(_pwrList[longIdx], OPTPWRRES) < Math.Round(stdPwr, OPTPWRRES))
                        break;

                    longIdx++;
                }


                pos1 = Math.Round(longIdx * _step, STGPOSXYZRES);
                pos2 = Math.Round(pos1 + _step, STGPOSXYZRES);
                pwr1 = _pwrList[longIdx];
                pwr2 = _pwrList[longIdx + 1];
                //stdPos2 = JeffMath.LinearInterpolation(pwr1, pos1, pwr2, pos2, stdPwr);
                stdPos2 = DataProcessingLogic.Interpolate_1st(pwr1, pwr2, pos1, pos2, stdPwr);
                stdPos2 = Math.Round(stdPos2, STGPOSXYZRES);

                ret = ((stdPos1 + stdPos2) / 2);
                ret = Math.Round(ret, STGPOSXYZRES);

            }
            catch
            {
                ret = Math.Round(peakIdx * _step, STGPOSXYZRES);
            }

            return ret;
        }




        /// <summary>
        /// Pos에 맞는 파워를 Linear Interpolation를 이용하여 구한다.
        /// pos는 0부터 시작.
        /// </summary>
        /// <param name="powers">optical power data List</param>
        /// <param name="pos">position.</param>
        /// <param name="step">데이터의 간격 [um] == Searching step</param>
        /// <returns> </returns>
        private double CalcPwrByPos(List<double> powers, double pos, double step)
        {
            try
            {
                int idx = (int)(Math.Truncate(pos / step));
                double pos1 = Math.Round((idx * step), STGPOSXYZRES);
                double pos2 = Math.Round((pos1 + step), STGPOSXYZRES);
                double pwr1 = powers[idx];
                double pwr2 = powers[idx + 1];
                //ret = JeffMath.LinearInterpolation(pos1, pwr1, pos2, pwr2, _pos);
                return DataProcessingLogic.Interpolate_1st(pos1, pos2, pwr1, pwr2, pos);
            }
            catch
            {
                return 0.0;
            }
        }



        List<double> xPos;
        List<double> yPos;
        List<double> xPower;
        List<double> yPower;


        /// <summary>
        ///  Hillclimb을 이용하여 Axis Fine Searching ~~~
        ///  digital
        /// </summary>
        /// <param name="_stgNo"> left or right </param>
        /// <param name="_axis">x,y</param>
        /// <param name="_port">detect port of powermeter</param>
        /// <param name="_searchStep">searching step</param>
        /// <param name="_hillTimes">hilltimes == hillcount</param>
        private PosPwr1d AxisSearchByHillclimb(int _stgNo, int _axis, int _port, double _searchStep, int _hillTimes)
        {
            //variables..
            PosPwr1d ret;
            ret.pos = 0.0;
            ret.pwr = 0.0;

            double dbCurPos = 0;
            double dbCurPwr = 0;

            //status 초기화.
            CsearchStatus state = _stgNo == LEFT_STAGE ? AlignStatusPool.xySearchIn : AlignStatusPool.xySearchOut;
            state.Clear();

            //190308 
            if (IsTestMode) return doTestMode(state);

            //Stage
            Istage stage = LEFT_STAGE == _stgNo ? mLeft : mRight;

            (xPos = xPos ?? new List<double>()).Clear();
            (yPos = yPos ?? new List<double>()).Clear();
            (xPower = xPower ?? new List<double>()).Clear();
            (yPower = yPower ?? new List<double>()).Clear();

            //시작 포지션 
            CalignPoint2d startPos = new CalignPoint2d();
            startPos.x = stage.GetAxisAbsPos(stage.AXIS_X);
            startPos.y = stage.GetAxisAbsPos(stage.AXIS_Y);

            if (_axis == stage.AXIS_X) dbCurPos = startPos.x;
            else dbCurPos = startPos.y;

            if (_axis == stage.AXIS_X) state.pos.x = dbCurPos;
            else state.pos.y = dbCurPos;
            state.posList.Add(new CalignPoint2d(state.pos.x, state.pos.x));

            //starting opticl power.
            dbCurPwr = mPm.ReadPower(_port);
            dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);

            state.pwr = dbCurPwr;
            state.pwrList.Add(dbCurPwr);

            if (_axis == stage.AXIS_X)
            {
                xPos.Add(state.pos.x);
                xPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
            }
            else
            {
                yPos.Add(state.pos.y);
                yPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
            }

            //stop?
            if (mStopFlag == true) return ret;


            //---------- Hillclimb --------------------
            double dbSbPwr = dbCurPwr;  //Searching base optical power.
            double dbSbPos = dbCurPos;  //Searching base position
            int nTargetDirection = stage.DIRECTION_MINUS;
            int nFindCount = 0;
            bool bSearchDirChanged = false;
            double dbMoveDist = 0;
            while (true)
            {
                //한 스텝 이동~~
                dbMoveDist = _searchStep;
                if (nTargetDirection != stage.DIRECTION_PLUS) dbMoveDist *= -1;
                stage.RelMove(_axis, dbMoveDist);
                stage.WaitForIdle(_axis);

                //current position.
                dbCurPos = Math.Round(dbCurPos + dbMoveDist, STGPOSXYZRES);

                if (_axis == stage.AXIS_X)
                {
                    state.pos.x = dbCurPos;
                    state.pos.y = startPos.y;
                }
                else
                {
                    state.pos.x = startPos.x;
                    state.pos.y = dbCurPos;
                }
                state.posList.Add(new CalignPoint2d(state.pos));

                //current optical power. [mW]
                dbCurPwr = mPm.ReadPower(_port);
                state.pwr = dbCurPwr;
                state.pwrList.Add(dbCurPwr);

                if (_axis == stage.AXIS_X)
                {
                    xPos.Add(state.pos.x);
                    xPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
                }
                else
                {
                    yPos.Add(state.pos.y);
                    yPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
                }

                //stop?
                if (mStopFlag == true) return ret;

                if (dbCurPwr < dbSbPwr)
                {
                    //---- 광파워가 낮은 쪽으로 잘 못 찾아가는 경우 ----//
                    if (nFindCount > _hillTimes)
                    {
                        //연속 nHillTimes번 떨어진 상황 // 
                        //--> Searching base로 이동한다.
                        dbMoveDist = Math.Round((dbSbPos - dbCurPos), STGPOSXYZRES);
                        stage.RelMove(_axis, dbMoveDist);
                        stage.WaitForIdle(_axis);

                        //현재 위치.[um]
                        dbCurPos = dbSbPos;

                        if (_axis == stage.AXIS_X)
                        {
                            state.pos.x = dbCurPos;
                            state.pos.y = startPos.y;
                        }
                        else
                        {
                            state.pos.x = startPos.x;
                            state.pos.y = dbCurPos;
                        }
                        state.posList.Add(new CalignPoint2d(state.pos));

                        //current optical power. [mW]
                        dbCurPwr = mPm.ReadPower(_port);
                        dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);

                        state.pwr = dbCurPwr;
                        state.pwrList.Add(dbCurPwr);

                        if (_axis == stage.AXIS_X)
                        {
                            xPos.Add(state.pos.x);
                            xPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
                        }
                        else
                        {
                            yPos.Add(state.pos.y);
                            yPower.Add(Math.Round(Unit.MillWatt2Dbm(dbCurPwr), 3));
                        }

                        //Searching base Update!! 
                        //이론상으로 서칭베이스로 이동했으므로 서칭베이스값을 변경 시켜줄 
                        //필요가 없지만 실제로는 광파워의 센서값은 바뀔 수 있다.
                        //그래서 업데이트 한다.!!
                        dbSbPos = dbCurPos;
                        dbSbPwr = dbCurPwr;

                        //이번 서칭 방향이 변경되었다면 끝낸다.
                        if (bSearchDirChanged == true) break;

                        //Searching 뱡향 변경
                        if (nTargetDirection == stage.DIRECTION_PLUS) nTargetDirection = stage.DIRECTION_MINUS;
                        else nTargetDirection = stage.DIRECTION_PLUS;
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
                    //----- 광파워가 높은 쪽으로 잘 찾아가고 있는 경우---//
                    dbSbPos = dbCurPos;
                    dbSbPwr = dbCurPwr;
                    nFindCount = 0;
                }
            }


            ret.pwr = Math.Round(dbSbPwr, OPTPWRRES); //[mW]
            ret.pos = dbCurPos;

            return ret;
        }




        #endregion


        #region IAlignment implementation


        /// <summary>
        /// 명령이 완료가 됬는지 아닌지 query
        /// </summary>
        /// <returns>true:완료, false:진행중 </returns>
        public bool IsCompleted()
        {
            return mCompleted;
        }

        public void ResetComplete() => mCompleted = false;
        public void SetComplete() => mCompleted = false;


        /// <summary>
        /// 명령을 멈춘다.
        /// </summary>
        public void StopOperation()
        {
            mStopFlag = true;
            //AlignCompleted?.Invoke(mAlignId);

            mLeft?.StopMove();
            mRight?.StopMove();            
        }

        #endregion


        #region IAlignmentDigital method implementation





        /// <summary>
        /// Blind Search (Digital version)
        /// 나선형으로 회전하면서 광을 찾는다.
        /// </summary>
        /// <param name="_stageNo">Left or Right</param>
        /// <param name="_detectPort"></param>
        /// <param name="_rng">검색 범위 [um]</param>
        /// <param name="_step">스캔 스탭 [um]</param>
        /// <param name="_thresh">dbThresholdPwr : 광을 찾을 찾았다고 보는 광파워 [dBm]</param>
        public void XyBlindSearch(int _stgNo, int _port, double _rng, double _step, double _thres)
        {
            //시작 ...
            mCompleted = false;
            mStopFlag = false;

            CurFuncNo = XYBLINDSEARCH;
            CurStageNo = _stgNo;

            try
            {
                //status 초기화.
                CsearchStatus state = _stgNo == LEFT_STAGE ? AlignStatusPool.xyBlindSearchIn : AlignStatusPool.xyBlindSearchOut;
                state.pos.x = 0;
                state.pos.y = 0;
                state.pwr = 0;
                state.posList.Clear();
                state.pwrList.Clear();

                //Stage
                Istage stage = _stgNo == LEFT_STAGE ? mLeft : mRight;

                //Alignment Threshold power.
                double dbAlignThresPwr = Unit.Dbm2MilliWatt(_thres); //[mW]
                dbAlignThresPwr = Math.Round(dbAlignThresPwr, OPTPWRRES);

                //190308 
                if (IsTestMode)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        state.posList.Add(new CalignPoint2d(i, i));
                        state.pwrList.Add(5 - Math.Abs((i - 10.0) / 10));
                        Thread.Sleep(100);
                    }
                    return;
                }


                //시작 위치를 구한다.
                CalignPoint2d startPos = new CalignPoint2d();
                startPos.x = stage.GetAxisAbsPos(stage.AXIS_X);
                startPos.y = stage.GetAxisAbsPos(stage.AXIS_Y);
                state.pos = startPos;
                state.posList.Add(startPos);

                //현재 광파워를 구한다.[mW]
                double dbOrgPwr = mPm.ReadPower(_port);
                dbOrgPwr = Math.Round(dbOrgPwr, OPTPWRRES);
                state.pwr = dbOrgPwr;
                state.pwrList.Add(dbOrgPwr);

                //Searching~~~
                bool bBlindAlignOK = false;
                double dbCurPwr = dbOrgPwr;
                double dbMaxPwr = dbOrgPwr;
                CalignPoint2d curPos = new CalignPoint2d(startPos.x, startPos.y);
                CalignPoint2d maxPos = new CalignPoint2d(startPos.x, startPos.y);
                int nMaxLoopCnt = Convert.ToInt32(_rng / _step);
                for (int i = 1; i <= nMaxLoopCnt; i++)
                {
                    for (int nAxis = stage.AXIS_X; nAxis <= stage.AXIS_Y; nAxis++)
                    {
                        for (int j = 1; j <= i; j++)
                        {
                            //이동.
                            stage.RelMove(nAxis, (_step * (Math.Pow((-1), i))));
                            stage.WaitForIdle(nAxis);


                            //현재 위치 계산.
                            if (nAxis == stage.AXIS_X)
                                curPos.x += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            else
                                curPos.y += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            state.pos.x = curPos.x;
                            state.pos.y = curPos.y;
                            state.posList.Add(new CalignPoint2d(curPos.x, curPos.y));


                            //현재 광파워를 읽는다. [mW]
                            dbCurPwr = mPm.ReadPower(_port);
                            dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                            state.pwr = dbCurPwr;
                            state.pwrList.Add(dbCurPwr);


                            //최대 광파워
                            if (dbCurPwr > dbMaxPwr)
                            {
                                dbMaxPwr = dbCurPwr;
                                maxPos.x = curPos.x;
                                maxPos.y = curPos.y;
                            }

                            //일정 광파워 이상이면 멈춤. 
                            if (dbCurPwr >= dbAlignThresPwr)
                            {
                                //Alignment 성공.!!//
                                stage.StopMove(nAxis);
                                bBlindAlignOK = true;
                                break;
                            }

                            //사용자의 정지명령?
                            if (mStopFlag == true)
                            {
                                stage.StopMove(nAxis);
                                //curAlignNo = NOOPERATION;
                                mCompleted = true;
                                return;
                            }
                        }
                        if (bBlindAlignOK == true) break;
                        if (mStopFlag) break;
                    }
                    if (bBlindAlignOK == true) break;
                    if (mStopFlag) break;
                }

                //Thres.power를 넘지 못했다면 최대 광파워 위치로 이동한다.
                if (bBlindAlignOK == false)
                {
                    double dbDistX = Math.Round(maxPos.x - curPos.x, STGPOSXYZRES);
                    double dbDistY = Math.Round(maxPos.x - curPos.x, STGPOSXYZRES);
                    stage.RelMove(stage.AXIS_X, dbDistX);
                    stage.RelMove(stage.AXIS_Y, dbDistY);
                    stage.WaitForIdle(stage.AXIS_Y);
                }
            }
            catch(Exception ex)
            {
                File.WriteAllText(@"log\error.log", $"XyBlindSearch():\n{ex.Message}\n{ex.StackTrace}");
            }

            finally
            {
                //완료..
                mCompleted = true;
                //curAlignNo = NOOPERATION;
                mWatch.Stop();
            }
        }




        /// <summary>
        /// Input,Output 모두 나선형으로 회전하면서 광을 찾는다.
        /// Input 한 번 끝나면 output 1step 이동. 
        /// </summary>
        /// <param name="_port">detect's port</param>
        /// <param name="_rngIn">input side scan range</param>
        /// <param name="_stepIn">input side scan step</param>
        /// <param name="_rngOut">output side scan range</param>
        /// <param name="_stepOut">output side scan step</param>
        /// <param name="_thres">optical power that we can start fine-alignment. [dBm]</param>
        public void XyFullBlindSearch(int _port,
                                        double _rngIn, double _stepIn,
                                        double _rngOut, double _stepOut,
                                        double _thres)
        {

            //시작 ...
            mCompleted = false;
            mStopFlag = false;

            CurFuncNo = XYFULLBLINDSEARCH;


            XfbInfor info = new XfbInfor();

            try
            {

                // <-------- for debugging
                List<InOutPosPwr> sd = new List<InOutPosPwr>();



                //status 초기화.
                CsearchStatus stateIn = AlignStatusPool.xyBlindSearchIn;
                stateIn.pos.x = 0;
                stateIn.pos.y = 0;
                stateIn.pwr = 0;
                stateIn.posList.Clear();
                stateIn.pwrList.Clear();

                CsearchStatus stateOut = AlignStatusPool.xyBlindSearchOut;
                stateOut.pos.x = 0;
                stateOut.pos.y = 0;
                stateOut.pwr = 0;
                stateOut.posList.Clear();
                stateOut.pwrList.Clear();


                //Alignment Threshold power.
                double dbAlignThresPwr = Unit.Dbm2MilliWatt(_thres); //[mW]
                dbAlignThresPwr = Math.Round(dbAlignThresPwr, OPTPWRRES);


                //시작 위치를 구한다.
                CalignPoint2d startPosIn = new CalignPoint2d();
                startPosIn.x = mLeft.GetAxisAbsPos(mLeft.AXIS_X);
                startPosIn.y = mLeft.GetAxisAbsPos(mLeft.AXIS_Y);
                stateIn.pos = new CalignPoint2d(startPosIn.x, startPosIn.y);
                stateIn.posList.Add(startPosIn);

                CalignPoint2d startPosOut = new CalignPoint2d();
                startPosOut.x = mRight.GetAxisAbsPos(mRight.AXIS_X);
                startPosOut.y = mRight.GetAxisAbsPos(mRight.AXIS_Y);
                stateOut.pos = new CalignPoint2d(startPosOut.x, startPosOut.y);
                stateOut.posList.Add(startPosOut);


                //start information 
                info.startTime = DateTime.Now;
                info.startPosIn = new CalignPoint2d();  //ICloneable Interface 구현필요.
                info.param.rngIn = _rngIn;
                info.param.stepIn = _stepIn;
                info.param.rngOut = _rngOut;
                info.param.stepOut = _stepOut;
                info.startPosIn.x = startPosIn.x;
                info.startPosIn.y = startPosIn.y;
                info.startPosOut = new CalignPoint2d();
                info.startPosOut.x = startPosOut.x;
                info.startPosOut.y = startPosOut.y;


                //현재 광파워를 구한다.[mW]
                double dbOrgPwr = mPm.ReadPower(_port);
                dbOrgPwr = Math.Round(dbOrgPwr, OPTPWRRES);
                stateIn.pwr = dbOrgPwr;
                stateIn.pwrList.Add(dbOrgPwr);
                stateOut.pwr = dbOrgPwr;
                stateOut.pwrList.Add(dbOrgPwr);


                //Searching~~~
                bool bBlindAlignOK = false;
                double dbCurPwr = dbOrgPwr;
                double dbMaxPwr = dbOrgPwr;
                CalignPoint2d curPosIn = new CalignPoint2d(startPosIn.x, startPosIn.y);
                CalignPoint2d curPosOut = new CalignPoint2d(startPosOut.x, startPosOut.y);


                int maxLoopCnt = Convert.ToInt32(_rngOut / _stepOut);
                for (int i = 1; i <= maxLoopCnt; i++)
                {
                    for (int axis = mRight.AXIS_X; axis <= mRight.AXIS_Y; axis++)
                    {
                        for (int j = 1; j <= i; j++)
                        {

                            //input search.
                            int searchCntIn = 0;
                            bBlindAlignOK = XyBlindSearchsBySpiral(mLeft.stageNo,
                                                                    _port,
                                                                    _rngIn, _stepIn,
                                                                    _thres,
                                                                    ref searchCntIn);
                            info.searchCntIn += searchCntIn;



                            //save용 데이터를 만든다. <------------- for debugging
                            for (int k = 0; k < stateIn.posList.Count(); k++)
                            {
                                InOutPosPwr posPwr = new InOutPosPwr();
                                posPwr.posOutX = curPosOut.x;
                                posPwr.posOutY = curPosOut.y;
                                posPwr.posInX = stateIn.posList[k].x;
                                posPwr.posInY = stateIn.posList[k].y;
                                posPwr.pwr = stateIn.pwrList[k];
                                sd.Add(posPwr);
                            }


                            if (bBlindAlignOK == true)
                                break;



                            //move output side.
                            mRight.RelMove(axis, (_stepOut * (Math.Pow((-1), i))));
                            mRight.WaitForIdle(axis);
                            info.searchCntOut++;



                            //현재 위치 계산.
                            if (axis == mRight.AXIS_X)
                                curPosOut.x += Math.Round((_stepOut * (Math.Pow((-1), i))), STGPOSXYZRES);
                            else
                                curPosOut.y += Math.Round((_stepOut * (Math.Pow((-1), i))), STGPOSXYZRES);
                            stateOut.pos.x = curPosOut.x;
                            stateOut.pos.y = curPosOut.y;
                            stateOut.posList.Add(new CalignPoint2d(curPosOut.x, curPosOut.y));


                            //사용자의 정지명령?
                            if (mStopFlag == true)
                            {
                                mLeft.StopMove();
                                mRight.StopMove();
                                //curAlignNo = NOOPERATION;
                                mCompleted = true;
                                return;
                            }

                        }


                        if (bBlindAlignOK == true)
                            break;
                    }



                    if (bBlindAlignOK == true)
                        break;
                }



                //end information 
                info.endTime = DateTime.Now;
                info.endPosIn = new CalignPoint2d();  //ICloneable Interface 구현필요.
                info.endPosIn.x = curPosIn.x;
                info.endPosIn.y = curPosIn.y;
                info.endPosOut = new CalignPoint2d();
                info.endPosOut.x = curPosOut.x;
                info.endPosOut.y = curPosOut.y;


                //save data <------------- for debugging
                SaveXfbPosPwr(sd);
                SaveXfbInfor(info);


            }
            catch
            {
                //do nothing.
            }


            //완료..
            //curAlignNo = NOOPERATION;
            mCompleted = true;

        }






        /// <summary>
        /// input, output의 한 축(X축 혹은 Y축)으로 서칭하면서 Theshold 광파워가 넘는곳을 찾는다.
        /// </summary>
        /// <param name="_axis">x or y</param>
        /// <param name="_port">port of powermeter for searching</param>
        /// <param name="_rng">seraching range[um]</param>
        /// <param name="_step">seraching step[um]</param>
        /// <param name="_thresh">정렬에 성공했다고 판단 할 광파워[dBm]</param>
        public void AxisBlindSearch(int _axis, int _port, double _rng, double _step, double _thres)
        {

        }



        /// <summary>
        /// blindSearch하는데 in,out sync해서 동작함.
        /// </summary>
        /// <param name="_port">port of powermeter for searching</param>
        /// <param name="_rng">seraching range[um]</param>
        /// <param name="_step">seraching step[um]</param>
        /// <param name="_thres">정렬에 성공했다고 판단 할 광파워[dBm]</param>
        /// <returns></returns>
        public bool SyncXySearch(int _port, double _rng, double _step, double _thres)
        {

            bool bAlignOK = false;


            //시작 ...
            mCompleted = false;
            mStopFlag = false;

            CurFuncNo = SYNCXYSEARCH;
            CurStageNo = STAGE_LR;


            try
            {

                //status 초기화. <-input을 기준으로 한다.
                CsearchStatus stateIn = null;
                stateIn = AlignStatusPool.syncXySearchIn;

                stateIn.pos.x = 0;
                stateIn.pos.y = 0;
                stateIn.pwr = 0;
                stateIn.posList.Clear();
                stateIn.pwrList.Clear();

                CsearchStatus stateOut = null;
                stateOut = AlignStatusPool.syncXySearchOut;
                stateOut.pos.x = 0;
                stateOut.pos.y = 0;
                stateOut.pwr = 0;
                stateOut.posList.Clear();
                stateOut.pwrList.Clear();

                stateIn.Clear();
                stateOut.Clear();


                //Stage
                Istage leftStg = mLeft;
                Istage rightStg = mRight;


                //Alignment Threshold power.
                double dbAlignThresPwr = 0; //[mW]
                dbAlignThresPwr = Unit.Dbm2MilliWatt(_thres);
                dbAlignThresPwr = Math.Round(dbAlignThresPwr, OPTPWRRES);


                //시작 위치를 구한다.  left stage가 기준.
                CalignPoint2d startPos = new CalignPoint2d();
                startPos.x = leftStg.GetAxisAbsPos(leftStg.AXIS_X);
                startPos.y = leftStg.GetAxisAbsPos(leftStg.AXIS_Y);
                startPos.x = Math.Round(startPos.x, OPTPWRRES);
                startPos.y = Math.Round(startPos.y, OPTPWRRES);
                stateIn.pos = startPos;
                stateIn.posList.Add(startPos);


                //현재 광파워를 구한다.[mW]
                double dbOrgPwr = mPm.ReadPower(_port);
                dbOrgPwr = Math.Round(dbOrgPwr, OPTPWRRES);

                stateIn.pwr = dbOrgPwr;
                stateIn.pwrList.Add(dbOrgPwr);


                //Searching~~~
                double dbCurPwr = dbOrgPwr;
                double dbMaxPwr = dbOrgPwr;
                CalignPoint2d curPos = new CalignPoint2d(startPos.x, startPos.y);
                CalignPoint2d maxPos = new CalignPoint2d(startPos.x, startPos.y);
                int nMaxLoopCnt = Convert.ToInt32(_rng / _step);
                for (int i = 1; i <= nMaxLoopCnt; i++)
                {


                    for (int nAxis = leftStg.AXIS_X; nAxis <= leftStg.AXIS_Y; nAxis++)
                    {
                        for (int j = 1; j <= i; j++)
                        {

                            //이동.
                            leftStg.RelMove(nAxis, (_step * (Math.Pow((-1), i))));
                            rightStg.RelMove(nAxis, (_step * (Math.Pow((-1), i))));
                            rightStg.WaitForIdle(nAxis);

                            //현재 위치 계산.
                            if (nAxis == leftStg.AXIS_X)
                                curPos.x += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);
                            else
                                curPos.y += Math.Round((_step * (Math.Pow((-1), i))), STGPOSXYZRES);

                            stateIn.pos.x = curPos.x;
                            stateIn.pos.y = curPos.y;
                            stateIn.posList.Add(new CalignPoint2d(curPos.x, curPos.y));

                            //현재 광파워를 읽는다. [mW]
                            dbCurPwr = mPm.ReadPower(_port);
                            dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                            stateIn.pwr = dbCurPwr;
                            stateIn.pwrList.Add(dbCurPwr);

                            //최대 광파워
                            if (dbCurPwr > dbMaxPwr)
                            {
                                dbMaxPwr = dbCurPwr;
                                maxPos.x = curPos.x;
                                maxPos.y = curPos.y;
                            }

                            //일정 광파워 이상이면 멈춤
                            if (dbCurPwr >= dbAlignThresPwr)
                            {
                                //alignment 성공//
                                leftStg.StopMove(nAxis);
                                rightStg.StopMove(nAxis);
                                bAlignOK = true;
                                break;
                            }


                            //사용자 정지 명령?
                            if (mStopFlag == true)
                            {
                                leftStg.StopMove(nAxis);
                                rightStg.StopMove(nAxis);

                                //curAlignNo = NOOPERATION;
                                mCompleted = true;
                                return false;
                            }


                        }

                        if (bAlignOK == true)
                            break;

                    }


                    if (bAlignOK == true)
                        break;

                }


                //Thres.power를 넘지 못했다면 최대 광파워 위치로 이동한다.
                if (bAlignOK == false)
                {
                    double dbDistX = Math.Round(maxPos.x - curPos.x, STGPOSXYZRES);
                    double dbDistY = Math.Round(maxPos.x - curPos.x, STGPOSXYZRES);
                    leftStg.RelMove(leftStg.AXIS_X, dbDistX);
                    rightStg.RelMove(leftStg.AXIS_X, dbDistX);
                    leftStg.RelMove(leftStg.AXIS_Y, dbDistY);
                    rightStg.RelMove(leftStg.AXIS_Y, dbDistY);
                    rightStg.WaitForIdle();
                }


            }
            catch
            {
                //do nothing.
            }


            //완료..
            //curAlignNo = NOOPERATION;
            mCompleted = true;

            return bAlignOK;
        }



        /// <summary>
        /// input,ouput stage를 싱크시켜 한 축(x or y) 서칭하면서 최대 광파워가 나오는 곳을 찾는다.
        /// </summary>
        /// <param name="_port">port of powermeter for searching</param>
        /// <param name="_rng">seraching range[um]</param>
        /// <param name="_step">seraching step[um]</param>
        public void SyncAxisSearch(int _axis, int _port, double _rng, double _step)
        {

            //variables.
            double dbCurPos = 0;
            double dbCurPwr = 0;
            int nTargetDirection = 0;
            double dbMoveDist = 0.0;

            List<double> posList = new List<double>();
            List<double> pwrList = new List<double>();


            //시작 ...
            mCompleted = false;
            mStopFlag = false;

            CurFuncNo = SYNCAXISSEARCH;
            CurStageNo = STAGE_LR;

            try
            {

                //status 초기화.
                CsearchStatus stateIn = null;
                CsearchStatus stateOut = null;
                if (_axis == mLeft.AXIS_X)
                {
                    stateIn = AlignStatusPool.syncAxisSearchInX;
                    stateOut = AlignStatusPool.syncAxisSearchOutX;
                }
                else
                {
                    stateIn = AlignStatusPool.syncAxisSearchInY;
                    stateOut = AlignStatusPool.syncAxisSearchOutX;
                }
                stateIn.Clear();
                stateOut.Clear();




                //-----시작 포지션 , 광파워~~-----//

                //Searching 시작 점 으로이동 (  서칭 시작 점으로 이동 : 서칭영역의 절반 이동 )
                int nSearchDirection = nTargetDirection;
                if (nSearchDirection == mLeft.DIRECTION_PLUS)
                    dbMoveDist = Math.Round((double)(_rng / 2));
                else
                    dbMoveDist = (-1) * Math.Round((double)(_rng / 2));
                dbMoveDist = Math.Round(dbMoveDist, STGPOSXYZRES);

                mLeft.RelMove(_axis, dbMoveDist);
                mRight.RelMove(_axis, dbMoveDist);
                mRight.WaitForIdle(_axis);

                if (mStopFlag == true)
                {
                    mCompleted = true;
                    return;
                }


                //Start Postion. (Input이 기준.)
                CalignPoint2d startPosIn = new CalignPoint2d();
                CalignPoint2d startPosOut = new CalignPoint2d();
                startPosIn.x = mLeft.GetAxisAbsPos(mLeft.AXIS_X);
                startPosIn.y = mLeft.GetAxisAbsPos(mLeft.AXIS_Y);
                startPosOut.x = mRight.GetAxisAbsPos(mRight.AXIS_X);
                startPosOut.y = mRight.GetAxisAbsPos(mRight.AXIS_Y);
                startPosIn.x = Math.Round(startPosIn.x, STGPOSXYZRES);
                startPosIn.y = Math.Round(startPosIn.y, STGPOSXYZRES);
                startPosOut.x = Math.Round(startPosOut.x, STGPOSXYZRES);
                startPosOut.y = Math.Round(startPosOut.y, STGPOSXYZRES);

                if (nSearchDirection == mLeft.DIRECTION_PLUS)
                    dbCurPos = startPosIn.x;
                else
                    dbCurPos = startPosIn.y;
                posList.Add(dbCurPos);

                stateIn.pos.x = startPosIn.x;
                stateIn.pos.y = startPosIn.y;
                stateOut.pos.x = startPosOut.x;
                stateOut.pos.y = startPosOut.y;
                stateIn.posList.Add(new CalignPoint2d(startPosIn));
                stateOut.posList.Add(new CalignPoint2d(startPosOut));


                //read optical power.   [mW]
                dbCurPwr = mPm.ReadPower(_port);
                dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                pwrList.Add(dbCurPwr);



                //------- Searching -----//
                nTargetDirection = mLeft.DIRECTION_PLUS;
                while (true)
                {

                    //이동~~
                    if (nSearchDirection == mLeft.DIRECTION_PLUS)
                        dbMoveDist = Math.Round(_step, STGPOSXYZRES);
                    else
                        dbMoveDist = (-1) * Math.Round(_step, STGPOSXYZRES);

                    mLeft.RelMove(_axis, dbMoveDist);
                    mRight.RelMove(_axis, dbMoveDist);
                    mRight.WaitForIdle(_axis);


                    //position 계산.
                    dbCurPos += Math.Round(dbMoveDist, STGPOSXYZRES);
                    posList.Add(dbCurPos);

                    if (_axis == mLeft.AXIS_X)
                    {
                        stateIn.pos.x = dbCurPos;
                        stateOut.pos.x += dbMoveDist;
                    }
                    else
                    {
                        stateIn.pos.y = dbCurPos;
                        stateOut.pos.y += dbMoveDist;
                    }
                    stateIn.posList.Add(new CalignPoint2d(stateIn.pos.x, stateIn.pos.y));
                    stateOut.posList.Add(new CalignPoint2d(stateOut.pos.x, stateOut.pos.y));


                    //광파워 측정.[mW]
                    dbCurPwr = mPm.ReadPower(_port);
                    dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                    pwrList.Add(dbCurPwr);

                    stateIn.pwr = dbCurPos;
                    stateOut.pwr = dbCurPos;
                    stateIn.pwrList.Add(dbCurPos);
                    stateOut.pwrList.Add(dbCurPos);


                    //서칭 영역을 넘기면 서칭 끝!!
                    if (Math.Abs(dbCurPos - posList[0]) > (_rng))
                        break;

                    //정지?
                    if (mStopFlag == true)
                    {
                        mLeft.StopMove(_axis);
                        mRight.StopMove(_axis);
                        mCompleted = true;
                        return;
                    }


                }



                //-------move to Max. position -----//
                double dbMaxPwr = pwrList.Max();
                int nMaxPwrIdx = pwrList.IndexOf(dbMaxPwr);
                double dbMaxPwrPos = posList[nMaxPwrIdx];
                dbMoveDist = Math.Round((dbMaxPwrPos - dbCurPos), STGPOSXYZRES);
                mLeft.RelMove(_axis, (dbMaxPwrPos - dbCurPos));
                mRight.RelMove(_axis, (dbMaxPwrPos - dbCurPos));
                mRight.WaitForIdle(_axis);


                //position 계산.
                dbCurPos += Math.Round(dbMoveDist, STGPOSXYZRES);
                posList.Add(dbCurPos);

                if (_axis == mLeft.AXIS_X)
                {
                    stateIn.pos.x = dbCurPos;
                    stateOut.pos.x += dbMoveDist;
                }
                else
                {
                    stateIn.pos.y = dbCurPos;
                    stateOut.pos.y += dbMoveDist;
                }
                stateIn.posList.Add(new CalignPoint2d(stateIn.pos.x, stateIn.pos.y));
                stateOut.posList.Add(new CalignPoint2d(stateOut.pos.x, stateOut.pos.y));


                //광파워 측정.[mW]
                dbCurPwr = mPm.ReadPower(_port);
                dbCurPwr = Math.Round(dbCurPwr, OPTPWRRES);
                pwrList.Add(dbCurPwr);

                stateIn.pwr = dbCurPos;
                stateOut.pwr = dbCurPos;
                stateIn.pwrList.Add(dbCurPos);
                stateOut.pwrList.Add(dbCurPos);

            }
            catch
            {
                //do nothing.
            }


            //완료..
            //curAlignNo = NOOPERATION;
            mCompleted = true;

        }





        #endregion //IAlignmentDigital method implementation



    }


}
