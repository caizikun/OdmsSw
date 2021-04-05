using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;

namespace Neon.Aligner
{
    public class Osw : IoptSwitch, IDisposable
    {
        private int mAlignPort = 0;
        private int mTlsPort = 1;

        private SerialPort mSerial;
        private bool m_connected; //연결 상태 !!
        private int m_closedPort; //out closed port

        public bool IsNeonOsw = true;
        private string mNewLine;


        public Osw(int alingPort, int tlsPort, bool isNeonOsw)
        {            
            m_connected = false;
            m_closedPort = 0;

            mAlignPort = alingPort;
            mTlsPort = tlsPort;

            IsNeonOsw = isNeonOsw;
            mNewLine = isNeonOsw ? "\r" : "\n";

            //SerialPort 객체 생성
            if (mSerial == null) mSerial = new SerialPort();

            // Setup parameters
            //mSerial.PortName = $"COM8";
            if (isNeonOsw)
            {
                mSerial.BaudRate = 9600;
            }
            else
            {
                mSerial.BaudRate = 115200;
                mSerial.Handshake = Handshake.RequestToSend;
            }
            mSerial.DataBits = 8;
            mSerial.StopBits = StopBits.One;
            mSerial.Parity = Parity.None;
            mSerial.ReadTimeout = 2000;
            mSerial.NewLine = mNewLine;

            mEofEvent = new AutoResetEvent(false);
            mSerial.DataReceived += serial_DataReceived;
        }

        public void SetToTls()
        {
            log($"SetToTls()");
            CloseOutPort(mTlsPort);
        }
        public void SetToAlign()
        {
            log($"SetToAlign()");
            CloseOutPort(mAlignPort);
        }

        public event Action<int> PortChanged;
        public SerialPort Serial => mSerial;


        #region private method
        
        private void write(string msg)
        {
            //Send~
            mSerial.DiscardInBuffer();
            var byteBuffer = Encoding.ASCII.GetBytes($"{msg}{mNewLine}");
            mSerial.Write(byteBuffer, 0, byteBuffer.Length);

            Thread.Sleep(100);
        }
        
        private string readString()
        {
            //------------- receive ------------------//
            var byteBuffer = new byte[mSerial.BytesToRead];
            mSerial.Read(byteBuffer, 0, mSerial.BytesToRead);
            var strResponse = Encoding.ASCII.GetString(byteBuffer).Trim();
            mSerial.DiscardInBuffer();
            return strResponse;
        }


        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            mEofEvent.Set();
            //if (e.EventType == SerialData.Eof) mEofEvent.Set();
        }

        AutoResetEvent mEofEvent;

        private void beginWaitingData()
        {
            mEofEvent.Reset();
        }
        private bool endWaitingData()
        {
            //while (mSerial.BytesToRead == 0) Thread.Sleep(100);
            mEofEvent.WaitOne(5000);
            if (mSerial.BytesToRead == 0)
            {
                log($"<{mSerial.PortName}>가 5초동안 반응이 없습니다.");
                return false;
            }
            return true;
        }
        void log(string msg)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            writer.WriteLine($"{time}\t{msg}");
            writer.Flush();
        }
        static Osw()
        {
            dir = Application.StartupPath + @"\log";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            filePath = dir + $@"\osw.txt";
            writer = new StreamWriter(filePath, true);
        }
        static string dir;// = Application.StartupPath + @"\dir";
        static string filePath;// = dir + $@"\osw\osw.txt";
        static StreamWriter writer;// = new StreamWriter(filePath, true);

        /// <summary>
        /// 명령을 보내고 응답을 받는다.
        /// </summary>
        /// <param name="cmd"> 쿼리 </param>
        /// <returns> Response </returns>
        private string query(string cmd)
        {
            try
            {
                //send and receive.
                Monitor.Enter(mSerial);

                beginWaitingData();
                write(cmd);
                if(endWaitingData()) LastReadString = readString();
                return LastReadString;
            }
            finally {  Monitor.Exit(mSerial); }
        }

        static string LastReadString = "0";

        #endregion


        #region public method


        /// <summary>
        /// Controller에  RS232로 연결한다.!!
        /// </summary>
        /// <param name="_comport"> COM Port Number </param>
        /// <returns> True : Connection is completed ,  false:Connection is fail. </returns>
        public bool Connect(int _comport)
        {
            //Check Connection
            if (true == m_connected) return true;

            mSerial.PortName = $"COM{_comport}";

            //port open
            if (mSerial.IsOpen == true) mSerial.Close();
            Thread.Sleep(200);
            mSerial.Open();

            //연결 확인.
            if (IsNeonOsw)
            {
                var strResponse = query("S?");
                m_connected = strResponse.Contains("0") || strResponse.Contains("1");
            }
            else
            {
                var strResponse = query("I?");
                m_connected = strResponse.ToUpper().Contains("OSW");
            }

            if (m_connected) m_closedPort = GetOutClosedPort();
            else mSerial.Close();

            return m_connected;
        }



        /// <summary>
        /// Controller와 접속을 끊는다.
        /// </summary>
        public void Disconnect()
        {
            mSerial?.Close();
            m_connected = false;
        }


        /// <summary>
        /// get close input no.
        /// 무존건 1을 리턴.
        /// </summary>
        /// <returns></returns>
        public int GetInClosedPort()
        {
            return 1;
        }

        /// <summary>
        /// close outport.
        /// </summary>
        /// <param name="port"></param>
        private void CloseOutPort(int port)
        {
            //---- TEST ----
            if (!mSerial.IsOpen)
            {
                var errorMsg = $"OSW error: serial port <{mSerial.PortName}> not opened";
                //throw new Exception(errorMsg);
                log(errorMsg);
                return;
            }

            int actual = -1;
            for (int i = 0; i < 10; i++)
            {
                log($"CloseOutPort()\ti={i}\tport={port}");
                write($"S {port}");
                actual = int.Parse(query("S?"));
                if (actual != port)
                {
                    log($"CloseOutPort()\ti={i}\tport={port}\tactual={actual}");
                    Thread.Sleep(200);
                    continue;
                }

                m_closedPort = port;
                PortChanged?.Invoke(m_closedPort);
                return;
            }
            //
            {
                var errorMsg = $"OSW port error: expetected=<{port}>, actual=<{actual}>";
                //throw new Exception(errorMsg);
                log(errorMsg);
            }
        }



        /// <summary>
        /// get closed out port.
        /// </summary>
        /// <returns></returns>
        public int GetOutClosedPort()
        {
            if (m_closedPort != 0) return m_closedPort;
            return int.Parse(query("S?"));
        }

        public void Dispose()
        {
            mSerial?.Dispose();
            writer?.Close();
            writer?.Dispose();
        }




        #endregion

    }


}
