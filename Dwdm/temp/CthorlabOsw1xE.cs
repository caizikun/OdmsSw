using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using Neon.Aligner;


public class Osw
{

    #region definition

    private const int OUT_PORT_1 = 1;
    private const int OUT_PORT_2 = 2;

    #endregion



    #region private member variables

    private SerialPort m_sp;
    private bool m_connected; //연결 상태 !!

    private int m_closedPort; //out closed port

    #endregion



    #region consturctor/destructor


    /// <summary>
    /// default constructor
    /// </summary>
    public Osw()
    {
        m_sp = null;
        m_connected = false;
        m_closedPort = 0;
    }


    #endregion



    #region private method


    /// <summary>
    /// RS232을 이용하여 명령을 보낸다.!!
    /// </summary>
    /// <param name="strCommand"> Motion contoller에 보낼 명령 </param>
    /// <returns></returns>
    private bool SendCommand(string _cmd)
    {

        //Variables.
        bool bRet = true;



        try
        {

            //Check Connection
            if (false == m_connected)
                return false;


            //Send~
            m_sp.DiscardInBuffer();

            byte[] byteBuffer = null;
            string strMsg = "";
            strMsg = _cmd + "\n";
            byteBuffer = System.Text.Encoding.Default.GetBytes(strMsg);
            m_sp.Write(byteBuffer, 0, byteBuffer.Length);

            Thread.Sleep(5);

        }
        catch
        {
            bRet = false;
        }


        return bRet;

    }




    /// <summary>
    /// RS232 이용하여 응답을 받는다.
    /// </summary>
    /// <returns> 응답 </returns>
    private string ReceiveResp()
    {


        string strResponse = "";
        byte[] byteBuffer = null;


        try
        {


            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();


            //------------- receive ------------------//
            byteBuffer = new byte[m_sp.BytesToRead];
            m_sp.Read(byteBuffer, 0, m_sp.BytesToRead);
            strResponse = System.Text.Encoding.Default.GetString(byteBuffer);
            strResponse = strResponse.Replace("\r", "");
            strResponse = strResponse.Replace("\n", "");

            m_sp.DiscardInBuffer();

        }
        catch
        {
            strResponse = "";
        }
        finally
        {
            byteBuffer = null;
        }

        return strResponse;

    }


    /// <summary>
    /// 응답이 올때까지 기다린다.
    /// </summary>
    private void WaitForReceive()
    {
        while (m_sp.BytesToRead == 0)
        {
            Thread.Sleep(10);
        }
    }



    /// <summary>
    /// 명령을 보내고 응답을 받는다.
    /// </summary>
    /// <param name="strQuery"> 쿼리 </param>
    /// <returns> Response </returns>
    private string Query(string strQuery)
    {
        try
        {
            Monitor.Enter(m_sp);

            SendCommand(strQuery);
            WaitForReceive();
            return ReceiveResp();
        }
        finally { Monitor.Exit(m_sp); }
    }





    #endregion







    #region public method



    /// <summary>
    /// Controller에  RS232로 연결한다.!!
    /// </summary>
    /// <param name="_comport"> COM Port Number </param>
    /// <returns> True : Connection is completed ,  false:Connection is fail. </returns>
    public bool Connect(int _comport)
    {

        bool bRet = true;

        try
        {

            //Check Connection
            if (true == m_connected)
                return true;


            //SerialPort 객체 생성
            if (m_sp == null)
                m_sp = new SerialPort();


            // Setup parameters
            m_sp.PortName = "COM" + Convert.ToString(_comport);
            m_sp.BaudRate = 115200;
            m_sp.DataBits = 8;
            m_sp.StopBits = StopBits.One;
            m_sp.Parity = Parity.None;
            m_sp.Handshake = Handshake.RequestToSend;
            m_sp.ReadTimeout = 2000;
            m_sp.NewLine = "\r";



            //port open
            if (m_sp.IsOpen == true)
                m_sp.Close();
            m_sp.Open();
            m_connected = true;



            //연결 확인.
            string strCommand = "";
            string strResponse = "";
            strCommand = "I?";
            strResponse = Query(strCommand);
            if (strResponse.IndexOf("OSW") < 0)
                throw new ApplicationException();

            bRet = true;


            m_closedPort = GetOutClosedPort();


        }
        catch
        {

            //메모리 해제
            if (m_sp != null)
            {
                m_sp.Dispose();
                m_sp = null;
            }

            m_connected = false;
            bRet = false;

        }


        return bRet;

    }



    /// <summary>
    /// Controller와 접속을 끊는다.
    /// </summary>
    public void Disconnect()
    {

        if (m_sp != null)
        {
            m_sp.Close();
            m_sp = null;
        }

        m_connected = false;

    }




    /// <summary>
    /// close input 
    /// </summary>
    /// <param name="_portNo"></param>
    public void CloseInPort(int _portNo)
    {
        //not implemented!! ^^
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
    /// <param name="_portNo"></param>
    public void CloseOutPort(int _portNo)
    {

        if ( m_closedPort == _portNo)
            return;


        try
        {

            //Check Connection
            if (true != m_connected)
                return;

            //send...
            string cmd = "";
            cmd = "S " + Convert.ToString(_portNo);
            SendCommand(cmd);
            Thread.Sleep(100);

            m_closedPort = _portNo;

        }
        catch
        {
            m_closedPort = 0;
        }


    }



    /// <summary>
    /// get closed out port.
    /// </summary>
    /// <returns></returns>
    public int GetOutClosedPort()
    {

        if (m_closedPort != 0)
            return m_closedPort;

        int ret = 0;

        try
        {
            string strCmd = "S?";
            string strResp = "";
            strResp = Query(strCmd);
            ret = Convert.ToInt32(strResp);

        }
        catch
        {
            ret = 0;
        }

        return ret;
    }




    #endregion







}
