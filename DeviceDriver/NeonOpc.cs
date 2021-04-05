using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO.Ports;
using System.Threading;

namespace Neon.Aligner
{
    public class NeonOpc : IpolController
    {

        #region === Class Frameworks ====

        public NeonOpc() /*: IpolController, IDisposable*/
        {

            mCmd = new Dictionary<PolState, string>();
            foreach (PolState pol in Enum.GetValues(typeof(PolState)))
                mCmd.Add(pol, $":S{(int)pol:D02}{mEof}");

            //SerialPort 객체 생성
            if (mSerial == null) mSerial = new SerialPort();
            mSerial.BaudRate = 115200;
            mSerial.ReadBufferSize = 1024;
            //mSerial.DataBits = 8;
            //mSerial.StopBits = StopBits.One;
            //mSerial.Parity = Parity.None;
            //mSerial.ReadTimeout = 2000;
            //mSerial.NewLine = mNewLine;

            mEofEvent = new AutoResetEvent(false);
            mSerial.DataReceived += serial_DataReceived;
        }

        int mState;
        double mTemp;
        public Action<int, double> mReporter;       //reporter [state, Temp]

        #endregion



        #region ==== Serial 통신 [Public] ====

        /// <summary>
        /// Controller에  RS232로 연결한다.!!
        /// </summary>
        /// <param name="_comport"> COM Port Number </param>
        public void Connect(int _comport)
        {

            mSerial.PortName = $"COM{_comport}";

            try
            {
                //port open
                if (mSerial.IsOpen == true)
                {
                    mSerial.Close();
                    Thread.Sleep(200);
                }
                mSerial.Open();

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion



        #region ==== Serial 통신 ====

        private SerialPort mSerial;
        private Queue<char> mQueue = new Queue<char>();
        private const char mEof = '\r';
        private AutoResetEvent mEofEvent;


        private void write(string msg)
        {
            mSerial.DiscardInBuffer();
            mQueue.Clear();
            mEofEvent.Reset();

            //Send~
            mSerial.Write($"{msg}{mEof}");

        }


        private string read()
        {
            mEofEvent.WaitOne();
            var response = new string(mQueue.ToArray());

            return response;
        }


        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            while (true)
            {
                var strResponse = mSerial.ReadExisting();

                for (int i = 0; i < strResponse.Length; i++)
                    mQueue.Enqueue(strResponse[i]);

                if (strResponse.Contains(mEof))
                {
                    mEofEvent.Set();
                    break;
                }
            }

        }


        static readonly object mLock = new object();

        private PolState query(PolState pol)
        {
            lock (mLock)
            {
                write(mCmd[pol]);
                var response = read().Split(':');
                if (response.Length == 2)
                {
                    mState = int.Parse(Regex.Replace(response[0], @"\D", ""));
                    mTemp = double.Parse(Regex.Replace(response[1], @"[^0-9.-]", ""));
                }
                else throw new Exception("[query()] NPC64 통신오류");
                return (PolState)mState;
            }

        }

        #endregion



        #region ==== 편광 설정 [IpolController] ====

        enum PolState { LVP = 1, LHP, Lp45, Ln45, RCP, LCP }
        Dictionary<PolState, string> mCmd;


        private void setPol(PolState pol)
        {
            if (query(pol) != pol)
                throw new Exception($"[setPol({pol})]\tNPC64 편광 설정 오류.");
        }


        /// <summary>
        /// 편광 State변경
        /// </summary>
        /// <param name="state">State No. [LVP = 1, LHP, Lp45, Ln45, RCP, LCP]</param>
        public void SetPol(int state)
        {
            setPol((PolState)state);
            mReporter?.Invoke(mState, mTemp);
        }


        /// <summary>
        /// 편광을 LH(Linear Horizontal)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        public void SetToLinearHorizontal(double _polPos)
        {
            setPol(PolState.LHP);
        }


        /// <summary>
        /// 편광을 LV(Linear Vertical)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        public void SetToLinearVertical(double _polPos)
        {
            setPol(PolState.LVP);
        }


        /// <summary>
        /// 편광을 LD(Linear Diagonal)로 설정한다. (plus 45 degree)
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        public void SetToLinearDiagonal(double _polPos)
        {
            setPol(PolState.Lp45);
        }


        /// <summary>
        /// 편광을 RHC(right hand circular)로 설정한다.
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        public void SetToRHcircular(double _polPos)
        {
            setPol(PolState.RCP);
        }


        /// <summary>
        /// 편광을 LHC(left hand circular)로 설정한다.
        /// </summary>
        /// <param name="_polPos"></param>
        public void SetToLHcircular(double _polPos)
        {
            setPol(PolState.LCP);
        }


        /// <summary>
        /// 편광을 LD(Linear Diagonal)로 설정한다. (minus 45 degree)
        /// </summary>
        /// <param name="_polPos">polarization filter의 위치[degree]</param>
        public void SetToNegaLinearDiagonal(double _polPos)
        {
            setPol(PolState.Ln45);
        }


        #endregion



        #region ==== IDisposable ====

        public void Dispose()
        {
            mSerial?.Dispose();
        }

        #endregion



        #region ==== Not Supported [IpolController] ====

        /// <summary>
        /// polarization filter의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">polarization filter의 위치[degree]</param>
        public void SetPolFilterPos(double _pos) { }


        /// <summary>
        /// 현재 polarization filter의 위치를 얻는다.
        /// </summary>
        /// <returns>polarization filter의 위치[degree]</returns>
        public double GetPolFilterPos() { return 0; }


        /// <summary>
        ///  λ/2 retarder의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">λ/2 retarder의 위치[degree]</param>
        public void SetHalfRetarderPos(double _pos) { }


        /// <summary>
        /// 현재 λ/2 retarder의 위치를 얻는다.
        /// </summary>
        /// <returns>λ/2 retarder의 위치[degree]</returns>
        public double GetHalfRetarderPos() { return 0; }


        /// <summary>
        ///  λ/4 retarder의 위치를 설정한다.
        /// </summary>
        /// <param name="_pos">λ/4 retarder의 위치[degree]</param>
        public void SetQuarRetarderPos(double _pos) { }


        /// <summary>
        /// 현재 λ/4 retarder의 위치를 얻는다.
        /// </summary>
        /// <returns>λ/4 retarder의 위치[degree]</returns>
        public double GetQuarRetarderPos() { return 0; }

        #endregion



    }//class

}
