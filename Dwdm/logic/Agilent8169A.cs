using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.NI4882;
using System.Threading;
using Neon.Aligner;


public class Agilent8169A : IpolController
{


    #region Member Variables

    private int m_gpibAddr;     //GPIB address
    private bool m_connected;   //연결 상태!!
    private Device m_gpibDev;   //gpib Device

    #endregion


    #region consturctor/destructor

    //소멸자.
    public Agilent8169A()
    {
        m_gpibAddr = 0;
        m_connected = false;
        m_gpibDev = null;
    }

    #endregion





    #region Private method



    /// <summary>
    /// query to device.
    /// </summary>
    /// <param name="_strQuery">query command </param>
    /// <returns></returns>
    private string Query(string _strQuery)
    {
        try
        {
            Monitor.Enter(m_gpibDev);

            m_gpibDev.Write(_strQuery);
            return m_gpibDev.ReadString();
        }
        finally
        {
            Monitor.Exit(m_gpibDev);
        }
    }



    /// <summary>
    /// 새로운 GPIB명령을 보낼 수 있을때 까지 기다린다.
    /// </summary>
    private void WaitForGpibIdle()
    {


        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();



            string strCommand = "";
            string strResponse = "";

            while (true)
            {

                //Send Command
                strCommand = "*OPC?";
                m_gpibDev.Write(strCommand);

                //Receive
                strResponse = m_gpibDev.ReadString();


                if (Convert.ToInt32(strResponse) == 1)
                    break;

            }

            m_gpibDev.Clear();


        }
        catch
        {
            //do nothing.
        }


    }



    #endregion





    #region public method


    /// <summary>
    /// 8169에 연결한다.
    /// </summary>
    /// <param name="_gpib"> GPIB Address </param>
    /// <returns> true : Connection is completed , false : Connection is fail.</returns>
    public bool Connect(int _gpib)
    {


        bool ret = true;


        try
        {

            //gpib 객체 생성 및 연결
            if (m_gpibDev == null)
                m_gpibDev = new Device(0, Convert.ToByte(_gpib));
            m_connected = true;


            //Identification을 물어본다.
            string strCommand = "*IDN?";
            string strResponse = "";
            strResponse = Query(strCommand);
            m_gpibDev.Write(strCommand);

            if (strResponse.IndexOf("HP8169A") < 0)
                throw new ApplicationException();

            m_gpibAddr = _gpib;
            m_connected = true;


        }
        catch
        {

            //메모리 해제
            if (m_gpibDev != null)
            {
                m_gpibDev.Dispose();
                m_gpibDev = null;
            }
            m_connected = false;
            ret = false;

        }

        return ret;

    }




    /// <summary>
    /// Polarizing Filter의 Position을 설정한다.!!
    /// </summary>
    /// <param name="_pos"></param>
    public void SetPolFilterPos(double _pos)
    {

        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();


            //Send Command
            _pos = Math.Round(_pos, 2);
            string strCommand = "";
            strCommand = "POS:POL " + Convert.ToString(_pos);
            m_gpibDev.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing.
        }


    }




    /// <summary>
    /// Get polarization filter position.
    /// </summary>
    /// <returns></returns>
    public double GetPolFilterPos()
    {

        double dbRet = 0;

        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();

            //query
            string strCommand = "POS:POL?";
            string strResponse = "";
            strResponse = Query(strCommand);
            dbRet = Convert.ToDouble(strResponse);
            dbRet = Math.Round(dbRet, 2);

        }
        catch
        {
            dbRet = 0;
        }

        return dbRet;

    }




    /// <summary>
    /// set position of half retarder plate.
    /// </summary>
    /// <param name="_pos">position[degree]</param>
    public void SetHalfRetarderPos(double _pos)
    {


        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();


            //Send Command
            _pos = Math.Round(_pos, 2);
            string strCommand = "";
            strCommand = "POS:HALF " + Convert.ToString(_pos);
            m_gpibDev.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing
        }


    }






    /// <summary>
    /// get postion of HalfRetarderPos.
    /// </summary>
    /// <returns></returns>
    public double GetHalfRetarderPos()
    {

        double dbRet = 0;

        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();

            //query
            string strCommand = "POS:HALF?";
            string strResponse = "";
            strResponse = Query(strCommand);
            dbRet = Convert.ToDouble(strResponse);
            dbRet = Math.Round(dbRet, 2);

        }
        catch
        {
            dbRet = 0;
        }

        return dbRet;


    }




    /// <summary>
    /// set postion of Quarter Retarder plate.
    /// </summary>
    /// <param name="_pos">position[degree]</param>
    public void SetQuarRetarderPos(double _pos)
    {


        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();


            //Send Command
            _pos = Math.Round(_pos, 2);
            string strCommand = "";
            strCommand = "POS:QUAR " + Convert.ToString(_pos);
            m_gpibDev.Write(strCommand);
            WaitForGpibIdle();


        }
        catch
        {
            //do nothing
        }



    }



    /// <summary>
    /// get position of quard retarder
    /// </summary>
    /// <returns>postion [degree]</returns>
    public double GetQuarRetarderPos()
    {


        double dbRet = 0;

        try
        {

            //Check Connection
            if (false == m_connected)
                throw new ApplicationException();

            //query
            string strCommand = "POS:QUAR?";
            string strResponse = "";
            strResponse = Query(strCommand);
            dbRet = Convert.ToDouble(strResponse);
            dbRet = Math.Round(dbRet, 2);

        }
        catch
        {
            dbRet = 0;
        }

        return dbRet;


    }





    /// <summary>
    ///  set Polarization Controller as Linear Horizental
    /// </summary>
    /// <param name="dbPolPos"></param>
    public void SetToLinearHorizontal(double _polPos)
    {
        SetPolFilterPos(_polPos);
        SetQuarRetarderPos(_polPos);
        SetHalfRetarderPos(_polPos);
    }


    /// <summary>
    /// set Polarization Controller as Linear Vertical
    /// </summary>
    /// <param name="dbPolPos"></param>             
    public void SetToLinearVertical(double _polPos)
    {
        SetPolFilterPos(_polPos);
        SetQuarRetarderPos(_polPos);
        SetHalfRetarderPos(_polPos + 45);
    }



    /// <summary>
    /// set Polarization Controller as Linear diagonal
    /// </summary>
    /// <param name="dbPolPos"></param>  
    public void SetToLinearDiagonal(double _polPos)
    {
        SetPolFilterPos(_polPos);
        SetQuarRetarderPos(_polPos);
        SetHalfRetarderPos(_polPos + 22.5);
    }



    /// <summary>
    /// set Polarization Controller as RH circular
    /// </summary>
    /// <param name="dbPolPos"></param>                
    public void SetToRHcircular(double dbPolPos)
    {
        SetPolFilterPos(dbPolPos);
        SetQuarRetarderPos(dbPolPos + 45);
        SetHalfRetarderPos(dbPolPos);
    }









    #endregion




}