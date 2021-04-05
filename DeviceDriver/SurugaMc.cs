using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.IO.Ports;
using NationalInstruments.NI4882;

namespace Neon.Aligner
{

    public class CsurugaseikiMc
    {


        #region definition


        //connection method
        public const int CONNECTION_METHOD_NONE = -1;              //None
        public const int CONNECTION_METHOD_USB_RS232 = 0;          //USB Or RS232
        public const int CONNECTION_METHOD_GPIB = 1;               //GPIB


        //Motor Axis
        public const int MOTOR_AXIS_X = 1;
        public const int MOTOR_AXIS_Y = 2;
        public const int MOTOR_AXIS_Z = 3;
        public const int MOTOR_AXIS_U = 4;
        public const int MOTOR_AXIS_V = 5;
        public const int MOTOR_AXIS_W = 6;
        public const int MOTOR_AXIS_ALL = 7;


        //Coordinate
        public const int MOTOR_AXIS_COORDINATE_ABS = 0;
        public const int MOTOR_AXIS_COORDINATE_REL = 1;

        
        //Unit
        public const int UNIT_PULSE = 0;                           //pulse
        public const int UNIT_UM = 1;                              //um
        public const int UNIT_MM = 2;                              //mm
        public const int UNIT_DEG = 3;                             //degree
        public const int UNIT_MRAD = 4;                            //mrad


        //Move direction
        public const int DIRECTION_CW = 0;                        //CW direction in Constant Step Pulse mode
        public const int DIRECTION_CCW = 1;                       //CCW direction in Constant Step Pulse mode
        public const int DIRECTION_ORIGIN = 2;                    //Origin 
        public const int DIRECTION_HOME = 3;                      //Home 
        public const int DIRECTION_ABS = 4;                       //ABS Position
        public const int DIRECTION_CWJ = 5;                       //CW direction in Continuous Driving mode
        public const int DIRECTION_CCWJ = 6;                      //CCW direction in Continuous Driving mode
        

        //Dividing number of drivers of each axis
        public const int DRIVER_DIVISION_1PER1 = 0;               // <Number of Division / Step>       1/1
        public const int DRIVER_DIVISION_1PER2 = 1;               // <Number of Division / Step>       1/2
        public const int DRIVER_DIVISION_1PER2_5 = 2;             // <Number of Division / Step>       1/2.5
        public const int DRIVER_DIVISION_1PER4 = 3;               // <Number of Division / Step>       1/4
        public const int DRIVER_DIVISION_1PER5 = 4;               // <Number of Division / Step>       1/5
        public const int DRIVER_DIVISION_1PER8 = 5;               // <Number of Division / Step>       1/8
        public const int DRIVER_DIVISION_1PER10 = 6;              // <Number of Division / Step>       1/10
        public const int DRIVER_DIVISION_1PER20 = 7;              // <Number of Division / Step>       1/20
        public const int DRIVER_DIVISION_1PER25 = 8;              // <Number of Division / Step>       1/25
        public const int DRIVER_DIVISION_1PER40 = 9;              // <Number of Division / Step>       1/40
        public const int DRIVER_DIVISION_1PER50 = 10;             // <Number of Division / Step>       1/40
        public const int DRIVER_DIVISION_1PER80 = 11;             // <Number of Division / Step>       1/80
        public const int DRIVER_DIVISION_1PER100 = 12;            // <Number of Division / Step>       1/100
        public const int DRIVER_DIVISION_1PER125 = 13;            // <Number of Division / Step>       1/125
        public const int DRIVER_DIVISION_1PER200 = 14;            // <Number of Division / Step>       1/200
        public const int DRIVER_DIVISION_1PER250 = 15;            // <Number of Division / Step>       1/250

        public static double[] DivisionValue = { 1, 2, 2.5, 4, 5, 8, 10, 20, 25, 40, 50, 80, 100, 125, 200, 250 };


        //Origin Return Type
        public const int ORIGIN_RETRUNTYPE_0 = 0;
        public const int ORIGIN_RETRUNTYPE_1 = 1;
        public const int ORIGIN_RETRUNTYPE_2 = 2;
        public const int ORIGIN_RETRUNTYPE_3 = 3;
        public const int ORIGIN_RETRUNTYPE_4 = 4;
        public const int ORIGIN_RETRUNTYPE_5 = 5;
        public const int ORIGIN_RETRUNTYPE_6 = 6;
        public const int ORIGIN_RETRUNTYPE_7 = 7;
        public const int ORIGIN_RETRUNTYPE_8 = 8;
        public const int ORIGIN_RETRUNTYPE_9 = 9;
        public const int ORIGIN_RETRUNTYPE_10 = 10;
        public const int ORIGIN_RETRUNTYPE_11 = 11;
        public const int ORIGIN_RETRUNTYPE_12 = 12;


        //Speed Table
        public const int SPEED_TABLE_0 = 0;
        public const int SPEED_TABLE_1 = 1;
        public const int SPEED_TABLE_2 = 2;
        public const int SPEED_TABLE_3 = 3;
        public const int SPEED_TABLE_4 = 4;
        public const int SPEED_TABLE_5 = 5;
        public const int SPEED_TABLE_6 = 6;
        public const int SPEED_TABLE_7 = 7;
        public const int SPEED_TABLE_8 = 8;
        public const int SPEED_TABLE_9 = 9;

        
        //Memory Switch
        public const int MEMORY_SWITCH_0 = 0;                     //Origin Return Type
        public const int MEMORY_SWITCH_1 = 1;                     //Mechanical Limit Sensor
        public const int MEMORY_SWITCH_2 = 2;                     //Origin Sensor Input Logic
        public const int MEMORY_SWITCH_3 = 3;                     //Near Origin Sensor Input
        public const int MEMORY_SWITCH_4 = 4;                     //Current Down
        public const int MEMORY_SWITCH_5 = 5;                     //Motion Direction Switching
        public const int MEMORY_SWITCH_6 = 6;                     //Stop Processing
        public const int MEMORY_SWITCH_7 = 7;                     //Origin Return 0 Reset

        
        //Speed KIND
        public const int SPEED_KIND_STARTUPVELOCITY = 0;          //Start Up Velocity (L)
        public const int SPEED_KIND_DRVSPEED = 1;                 //Drving Speed (F)
        public const int SPEED_KIND_ACCDEACCRATE = 2;             //Accelation & Deaccelaration rate (R)
        public const int SPEED_KIND_SACCDEACCRATE = 3;            //S Accelation & Deaccelaration rate (S)

        
        //Stop type
        public const int STOP_TYPE_EMERGENCY = 0;                 //Emergency Stop
        public const int STOP_TYPE_SLOWDOWN = 1;                  //Slow down

        
        //Mechanical Limit detection
        public const int MECHANICAL_LIMIT_DETECTION_NOTYET = 0;
        public const int MECHANICAL_LIMIT_DETECTION_CW = 1;
        public const int MECHANICAL_LIMIT_DETECTION_CCW = 2;
        public const int MECHANICAL_LIMIT_DETECTION_CW_CCW = 3;


        //Software Limit detection
        public const int SOFT_LIMIT_DETECTION_NOTYET = 0;
        public const int SOFT_LIMIT_DETECTION_CW = 1;
        public const int SOFT_LIMIT_DETECTION_CCW = 2;


        //Origin detection
        public const int ORIGIN_DETECTION_NOTYET = 0;
        public const int ORIGIN_DETECTION_OK = 1;


        //Origin detection
        public const int HOME_DETECTION_NOTYET = 0;
        public const int HOME_DETECTION_OK = 1;


        //Origin detection
        public const int DATA1 = 1;
        public const int DATA2 = 2;
        

        #endregion



        
        #region Structures
        

        public class CSoftLimit
        {
            public int axis { get; set; }                          //Axis
            public bool CWEnable { get; set; }                     //CW Effective or No Effective
            public double CWPosition { get; set; }                 //CW SoftLimit Position
            public bool CCWEnable { get; set; }                    //CCW Effective or No Effective
            public double CCWPosition { get; set; }                //CCW SoftLimit Position
        }



        public class CMemorySwitchState
        {
            public int axis { get; set; }                          //Axis
            public int sw0 { get; set; }                           //Memory Switch 0
            public int sw1 { get; set; }                           //Memory Switch 1
            public int sw2 { get; set; }                           //Memory Switch 2
            public int sw3 { get; set; }                           //Memory Switch 3
            public int sw4 { get; set; }                           //Memory Switch 4
            public int sw5 { get; set; }                           //Memory Switch 5
            public int sw6 { get; set; }                           //Memory Switch 6
            public int sw7 { get; set; }                           //Memory Switch 7
        }

        

        public class CSpeedTable
        {
            public int tableNo { get; set; }                       //Table Number
            public double startVelocity { get; set; }              //Start-up Velocity
            public double drivingSpeed { get; set; }               //Driving Speed
            public double accDeaccRate { get; set; }               //Accelation & Deaccelaration Rate
            public double sAccDeaccRate { get; set; }              //S Accelation & Deaccelaration Rate
        }


        #endregion



        
        #region Private Member variables

        private SerialPort m_sp;
        private bool m_bUSBorRS232dataReceived;


        private bool m_bConnectedOK;                //연결 상태!!
        private int m_nConnctionMethod;             //연결 방법

        private int m_nGPIBaddr;                    //GPIB address
        private Device m_gpibDev;                   //gpib Device

        private Hashtable m_softLimits;
        private Hashtable m_memSwStates;            //memory switch state, key : axis no , value : memory switch state
        private Hashtable m_speedTables;
        private Hashtable m_stdRess;                // standard resolution  key : axis no , value : standard resolution
        private Hashtable m_constStepPulses;
        private Hashtable m_spdTblNos;              // key : axis no , value : speed table no.
        private int m_progNo;                       //program no.

        #endregion



        
        #region consturctor/destructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CsurugaseikiMc()
        {

            m_bConnectedOK = false;
            m_nConnctionMethod = CONNECTION_METHOD_NONE;

            m_nGPIBaddr = -1;
            m_gpibDev = null;

            m_softLimits = new Hashtable();
            m_memSwStates = new Hashtable();
            m_speedTables = new Hashtable();
            m_stdRess = new Hashtable();
            m_constStepPulses = new Hashtable();
            m_spdTblNos = new Hashtable();
            m_progNo = -1;
        }


        
        /// <summary>
        /// 소멸자.
        /// </summary>
        ~CsurugaseikiMc()
        {

            if (m_gpibDev != null)
            {
                m_gpibDev.Dispose();
                m_gpibDev = null;
            }

        }
        

        #endregion



        
        #region Event Handler


        ////////////////////////////////////////////////////////////////
        ////SerialPort_DataReceived ///////////////////////////////
        ////////////////////////////////////////////////////////////////
        ////desc - SerialPort에서 Data를 받을때...
        //
        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            if (m_bUSBorRS232dataReceived == false) m_bUSBorRS232dataReceived = true;

        }

        #endregion



        
        #region private method
        

        /// <summary>
        /// program No를 설정한다.
        /// </summary>
        /// <param name="_progNo"></param>
        /// <returns></returns>
        private bool Sync_SetProgramNo(int _progNo)
        {
            //Variables.. 
            bool bRet = true;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    return false;


                //Check parameter
                if ((_progNo < 0) || (_progNo > 7))
                    return false;


                //Command를 날린다.
                string strCommand = "SELPRG " + _progNo.ToString();
                if (Sync_SendCommand(strCommand) == false)
                    throw new ApplicationException();

                bRet = true;

            }
            catch
            {
                bRet = false;
            }


            return bRet;
        }

        

        /// <summary>
        /// RS232을 이용하여 명령을 보낸다.!!
        /// </summary>
        /// <param name="strCommand"> Motion contoller에 보낼 명령 </param>
        /// <returns></returns>
        private bool Sync_SendCommandByRs232usb(string strCommand)
        {
            //Variables.
            bool bRet = true;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }

                //Send~
                m_sp.DiscardInBuffer();

                byte[] byteBuffer = null;
                string strMsg = "";
                strMsg = strCommand + "\r";
                byteBuffer = System.Text.Encoding.Default.GetBytes(strMsg);
                m_sp.Write(byteBuffer, 0, byteBuffer.Length);

                Thread.Sleep(80);

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }
        


        /// <summary>
        /// USB or RS232 data received signal을 받을때까지 기다린다.
        /// </summary>
        private void Sync_WaitForUSBorRS232RecvedSignal()
        {

            //while (true)
            //{
            //    if (m_bUSBorRS232dataReceived == true)
            //    {
            //        m_bUSBorRS232dataReceived = false;
            //        break;

            //    }
            //}


            while (m_sp.BytesToRead == 0)
            {
                Thread.Sleep(10);
            }

        }

        

        /// <summary>
        /// RS232 , usb를 이용하여 응답을 받는다.
        /// </summary>
        /// <returns> 응답 </returns>
        private string Sync_ReceiveResponseByRs232usb()
        {


            string strResponse = "";
            byte[] byteBuffer = null;


            try
            {


                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //------------- receive ------------------//
                byteBuffer = new byte[m_sp.BytesToRead];
                m_sp.Read(byteBuffer, 0, m_sp.BytesToRead);
                strResponse = System.Text.Encoding.Default.GetString(byteBuffer);
                strResponse = strResponse.Replace("\r", ""); ;


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
        /// 새로운 GPIB명령을 보낼 수 있을때 까지 기다린다.
        /// 엥간하면 사용하지 말기를... ㅋㅋ
        /// </summary>
        /// <returns></returns>
        public bool WaitForGpibIdle()
        {

            //Variables..
            bool bRet = true;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //operation이 completed 됬는지 물어보고
                //대답이 1이면 완료된거임.
                string strCommand = "";
                string strResponse = "";
                strCommand = "*OPC?";

                Monitor.Enter(m_gpibDev);

                while (true)
                {

                    Sync_SendCommandByGPIB(strCommand);
                    Thread.Sleep(10);
                    strResponse = Sync_ReceiveResponseByGPIB();
                    if ((Convert.ToInt32(strResponse) == 1))
                    {
                        break;
                    }

                }

                Monitor.Exit(m_gpibDev);

                m_gpibDev.Clear();

            }
            catch
            {
                bRet = false;
            }


            return bRet;


        }



        /// <summary>
        /// GPIB를 이용하여 명령을 보낸다.!!
        /// </summary>
        /// <param name="strCommand">Motion contoller에 보낼 명령</param>
        /// <returns></returns>
        private bool Sync_SendCommandByGPIB(string strCommand)
        {

            //Variables.
            bool bRet = true;

            try
            {
                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }

                //Send~
                m_gpibDev.Write(strCommand);

            }
            catch
            {
                bRet = false;
            }

            return bRet;

        }



        /// <summary>
        /// GPIB를 이용하여 응답을 받는다.
        /// </summary>
        /// <param name="nReaded"></param>
        /// <returns> response </returns>
        private string Sync_ReceiveResponseByGPIB(ref int nReaded)
        {

            string strResponse = "";

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }

                //receive
                strResponse = m_gpibDev.ReadString();
                strResponse = strResponse.Replace("\r\n", "");
                nReaded = strResponse.Length;

            }
            catch
            {
                strResponse = "";
            }


            return strResponse;

        }


                
        /// <summary>
        /// GPIB를 이용하여 응답을 받는다.
        /// </summary>
        /// <returns> response </returns>
        private string Sync_ReceiveResponseByGPIB()
        {

            string strResponse = "";

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }

                //receive
                strResponse = m_gpibDev.ReadString();
                strResponse = strResponse.Replace("\r\n", "");

            }
            catch
            {
                strResponse = "";
            }

            return strResponse;

        }

        

        /// <summary>
        /// Command를 보낸다.
        /// </summary>
        /// <param name="strCommand"> Motion contoller에 보낼 명령 </param>
        /// <returns></returns>
        private bool Sync_SendCommand(string strCommand)
        {

            //variables
            bool bRet = true;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //send
                switch ((m_nConnctionMethod))
                {
                    case CONNECTION_METHOD_USB_RS232:
                        //RS232//
                        bRet = Sync_SendCommandByRs232usb(strCommand);
                        break;

                    case CONNECTION_METHOD_GPIB:
                        //GPIB//
                        bRet = Sync_SendCommandByGPIB(strCommand);
                        WaitForGpibIdle();
                        break;

                }



            }
            catch
            {
                bRet = false;
            }



            return bRet;

        }



        /// <summary>
        /// 응답을 받는다.
        /// </summary>
        /// <param name="strResponse"> 데이터를 받을 버퍼 </param>
        /// <param name="nReadedCount"> Buffer의 Size </param>
        /// <returns></returns>
        private string Sync_ReceiveResponse()
        {

            string strResponse = "";


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //receive
                switch (m_nConnctionMethod)
                {
                    case CONNECTION_METHOD_USB_RS232:
                        //------------ RS232 ------------------//
                        strResponse = Sync_ReceiveResponseByRs232usb();
                        break;

                    case CONNECTION_METHOD_GPIB:
                        //------------ GPIB ------------------//
                        strResponse = Sync_ReceiveResponseByGPIB();
                        break;
                }

            }
            catch
            {
                strResponse = "";
            }


            return strResponse;

        }


        
        /// <summary>
        /// 명령을 보내고 응답을 받는다.
        /// </summary>
        /// <param name="strQuery"> 쿼리 </param>
        /// <returns> Response </returns>
        private string Query(string strQuery)
        {

            string retResponse = "";

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();


                switch (m_nConnctionMethod)
                {
                    case CONNECTION_METHOD_USB_RS232:
                        //---- USB OR RS232 ------//

                        Monitor.Enter(m_sp);
                        Sync_SendCommandByRs232usb(strQuery);

                        //Thread.Sleep(120);//?
                        Sync_WaitForUSBorRS232RecvedSignal();

                        retResponse = Sync_ReceiveResponseByRs232usb();
                        Monitor.Exit(m_sp);
                        break;

                    case CONNECTION_METHOD_GPIB:
                        //------ GPIB -----//

                        Monitor.Enter(m_gpibDev);
                        Sync_SendCommandByGPIB(strQuery);
                        retResponse = Sync_ReceiveResponseByGPIB();
                        Monitor.Exit(m_gpibDev);
                        break;

                }



            }
            catch
            {
                retResponse = "";
            }


            return retResponse;

        }
        


        /// <summary>
        /// Command to request setting of Speed Table
        /// </summary>
        /// <param name="nTableNo"> table Number</param>
        /// <returns></returns>
        private CSpeedTable Sync_GetSpeedTable(int nTableNo)
        {

            //Variables.. 
            CSpeedTable retSpdTbl = null;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return null;
                }


                retSpdTbl = new CSpeedTable();
                retSpdTbl.tableNo = nTableNo;

                string strCommand = "";
                string strResponse = "";


                //get StartVelocity
                strCommand = "";
                strResponse = "";
                strCommand = "Lspeed" + Convert.ToString(nTableNo) + "?";
                strResponse = Query(strCommand);
                retSpdTbl.startVelocity = Convert.ToDouble(strResponse);


                //get driving speed
                strCommand = "";
                strResponse = "";
                strCommand = "Fspeed" + Convert.ToString(nTableNo) + "?";
                strResponse = Query(strCommand);
                retSpdTbl.drivingSpeed = Convert.ToDouble(strResponse);


                //get Acceleration-and-Deacceleration Rate
                strCommand = "";
                strResponse = "";
                strCommand = "Rate" + Convert.ToString(nTableNo) + "?";
                strResponse = Query(strCommand);
                retSpdTbl.accDeaccRate = Convert.ToDouble(strResponse);



                if ((retSpdTbl.startVelocity == 0) &&
                    (retSpdTbl.drivingSpeed == 0) &&
                    (retSpdTbl.accDeaccRate == 0))
                {
                    return null;
                }


            }
            catch
            {
                retSpdTbl = null;
            }



            return retSpdTbl;

        }


        
        /// <summary>
        /// Command to request for Memory Switch State
        /// </summary>
        /// <param name="_axis"></param>
        /// <returns>memory Switch state</returns>
        private CMemorySwitchState Sync_GetMemSwitchState(int _axis)
        {

            CMemorySwitchState retMms = null;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Check Param
                if ((_axis < MOTOR_AXIS_X) || (_axis > MOTOR_AXIS_W))
                {
                    throw new ApplicationException();
                }


                retMms = new CMemorySwitchState();
                retMms.axis = _axis;


                //Memory Swith0
                string strCommand = "";
                string strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch0?";
                strResponse = Query(strCommand);
                retMms.sw0 = Convert.ToInt32(strResponse);

                //Memory Swith1
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch1?";
                strResponse = Query(strCommand);
                retMms.sw1 = Convert.ToInt32(strResponse);

                //Memory Swith2
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch2?";
                strResponse = Query(strCommand);
                retMms.sw2 = Convert.ToInt32(strResponse);

                //Memory Swith3
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch3?";
                strResponse = Query(strCommand);
                retMms.sw3 = Convert.ToInt32(strResponse);

                //Memory Swith4
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch4?";
                strResponse = Query(strCommand);
                retMms.sw4 = Convert.ToInt32(strResponse);

                //Memory Swith5
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch5?";
                strResponse = Query(strCommand);
                retMms.sw5 = Convert.ToInt32(strResponse);

                //Memory Swith6
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch6?";
                strResponse = Query(strCommand);
                retMms.sw6 = Convert.ToInt32(strResponse);

                //Memory Swith7
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":MEMorySWitch7?";
                strResponse = Query(strCommand);
                retMms.sw7 = Convert.ToInt32(strResponse);


            }
            catch
            {
                retMms = null;
            }

            return retMms;

        }

        

        /// <summary>
        /// Command to set Memory SW state for axis
        /// </summary>
        /// <param name="_mss"></param>
        /// <returns></returns>
        private bool Sync_SetMemSwitchState(CMemorySwitchState _mss)
        {

            bool bRet = false;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check parameter
                if (_mss == null)
                {
                    return true;
                }



                string strCommand = "";
                //Set Memory Switch State 0
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW0 ";
                strCommand = strCommand + Convert.ToString(_mss.sw0);
                Sync_SendCommand(strCommand);

                //Set Memory Switch State 1
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW1 ";
                strCommand = strCommand + Convert.ToString(_mss.sw1);
                Sync_SendCommand(strCommand);


                //Set Memory Switch State 2
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW2 ";
                strCommand = strCommand + Convert.ToString(_mss.sw2);
                Sync_SendCommand(strCommand);


                //Set Memory Switch State 3
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW3 ";
                strCommand = strCommand + Convert.ToString(_mss.sw3);
                Sync_SendCommand(strCommand);


                //Set Memory Switch State 4
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW4 ";
                strCommand = strCommand + Convert.ToString(_mss.sw4);
                Sync_SendCommand(strCommand);

                //Set Memory Switch State 5
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW5 ";
                strCommand = strCommand + Convert.ToString(_mss.sw5);
                Sync_SendCommand(strCommand);

                //Set Memory Switch State 6
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW6 ";
                strCommand = strCommand + Convert.ToString(_mss.sw6);
                Sync_SendCommand(strCommand);

                //Set Memory Switch State 7
                strCommand = "Axis" + Convert.ToString(_mss.axis);
                strCommand = strCommand + ":MEMSW7 ";
                strCommand = strCommand + Convert.ToString(_mss.sw7);
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }


            return bRet;


        }


        
        /// <summary>
        /// SoftLimit를 장비를 얻는다.
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <returns> soft limit </returns>
        private CSoftLimit Sync_GetSoftLimit(int _axis)
        {

            CSoftLimit retSoftLmt = null;


            try
            {

                string strCommand = "";
                string strResponse = "";


                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Check Param
                if ((_axis < MOTOR_AXIS_X) | (_axis > MOTOR_AXIS_W))
                {
                    throw new ApplicationException();
                }



                retSoftLmt = new CSoftLimit();
                retSoftLmt.axis = _axis;


                //CW softlimit Enable??
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":CWSLE?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                if (Convert.ToInt32(strResponse) > 0)
                {
                    retSoftLmt.CWEnable = true;
                }
                else
                {
                    retSoftLmt.CWEnable = false;
                }


                //CW SoftLimit Point
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":CWSLP?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retSoftLmt.CWPosition = Convert.ToDouble(strResponse);



                //CCW SoftLimit Enable?
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":CCWSLE?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                if (Convert.ToInt32(strResponse) > 0)
                {
                    retSoftLmt.CCWEnable = true;
                }
                else
                {
                    retSoftLmt.CCWEnable = false;
                }



                //CCW SoftLimit Point
                strCommand = "";
                strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);
                strCommand = strCommand + ":CCWSLP?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retSoftLmt.CCWPosition = Convert.ToDouble(strResponse);



            }
            catch
            {
                retSoftLmt = null;
            }

            return retSoftLmt;

        }

        

        /// <summary>
        /// SoftLimit를 설정한다.
        /// </summary>
        /// <param name="_sl"> soft limit</param>
        /// <returns></returns>
        private bool Sync_SetSoftLimit(CSoftLimit _sl)
        {

            bool bRet = false;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check Param
                if ((_sl.axis < MOTOR_AXIS_X) || (_sl.axis > MOTOR_AXIS_W))
                {
                    return false;
                }


                string strCommand = "";
                //set CW SoftLimit Enable ?
                strCommand = "Axis" + Convert.ToString(_sl.axis);       //Axis
                if (true == _sl.CWEnable)
                {
                    strCommand = strCommand + ":CWSoftLimitEnable 1";   //Enable
                }
                else
                {
                    strCommand = strCommand + ":CWSoftLimitEnable 0";   //Disable
                }
                Sync_SendCommand(strCommand);



                //set CW SoftLimit Position
                strCommand = "Axis" + Convert.ToString(_sl.axis);   //Axis
                strCommand = strCommand + ":CWSoftLimitPoint ";
                strCommand = strCommand + Convert.ToString(_sl.CWPosition);
                Sync_SendCommand(strCommand);


                //Set CCW SoftLimit Enable?
                strCommand = "Axis" + Convert.ToString(_sl.axis);
                if (true == _sl.CCWEnable)
                {
                    strCommand = strCommand + ":CCWSoftLimitEnable 1";  //Enable
                }
                else
                {
                    strCommand = strCommand + ":CCWSoftLimitEnable 0";  //Disable
                }


                //set CCW SoftLimit Position
                strCommand = "Axis" + Convert.ToString(_sl.axis);   //Axis
                strCommand = strCommand + ":CCWSoftLimitPoint ";
                strCommand = strCommand + Convert.ToString(_sl.CCWPosition);
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }


        
        /// <summary>
        ///  Command to set Constant Step Pulse of axis
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <param name="_pulse"> const step pulse</param>
        /// <returns></returns>
        private bool Sync_SetConstantStepPulse(int _axis, double _pulse)
        {

            bool bRet = false;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException("gpib connection is fail..");
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":";
                strCommand = strCommand + "PULS ";
                strCommand = strCommand + Convert.ToString(Math.Round(_pulse, 4));
                bRet = Sync_SendCommand(strCommand);
                if (bRet == false)
                {
                    return bRet;
                }



                bRet = true;

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }

        

        /// <summary>
        /// Command to request for setting of Constant Step Pulse of each axis
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <returns> constant step pulse</returns>
        private int Sync_GetConstantStepPulse(int _axis)
        {

            int retVal = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);  //Axis
                strCommand = strCommand + ":PULSe?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retVal = Convert.ToInt32(strResponse);


            }
            catch
            {
                retVal = 0;
            }


            return retVal;


        }



        /// <summary>
        /// Command to request for standrad resoultion of motorized stage
        /// </summary>
        /// <param name="_axis"></param>
        /// <returns> standard resolution </returns>
        private double Sync_GetStandardResolution(int _axis)
        {

            double retStdRes = 0;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);  //Axis
                strCommand = strCommand + ":STANDARDresolution?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                retStdRes = Math.Round(Convert.ToDouble(strResponse), 4);


            }
            catch
            {
                retStdRes = 0;
            }


            return retStdRes;

        }
        


        /// <summary>
        /// Command to set standrad resoultion of motorized stage
        /// </summary>
        /// <param name="_axis"> axis .( ex. X,Y,Z,U,V,W) </param>
        /// <param name="_resolution">standard resolution</param>
        /// <returns></returns>
        private bool Sync_SetStandardResolution(int _axis, double _resolution)
        {

            bool bRet = true;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check parameter
                if ((_resolution < 1E-07) | (_resolution > 99999999))
                {
                    return false;
                }



                //설정값이 기존값과 동일하면 그냥 나간다. 
                double prevStdRes = 0;
                try
                {
                    prevStdRes = Math.Round((double)m_stdRess[_axis], 4);
                }
                catch
                {
                    prevStdRes = 0;
                }

                if (Math.Round(prevStdRes, 4) == Math.Round(_resolution, 4))
                    return true;



                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":STANDARD ";
                strCommand = strCommand + Convert.ToString(_resolution);
                if (false == Sync_SendCommand(strCommand))
                {
                    throw new ApplicationException();
                }


                //save
                m_stdRess[_axis] = Math.Round(_resolution, 4);

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }



        /// <summary>
        /// Command to request for setting of Speed Table Number of axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W)  </param>
        /// <returns> speed table no.  함수 실패시 -1</returns>
        private int Sync_GetSpeedTableNo(int _axis)
        {

            int retSpdTblNo = -1;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException("gpib connection is fail..");
                }



                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":SELectSPeed?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                retSpdTblNo = Convert.ToInt32(strResponse);


            }
            catch
            {
                retSpdTblNo = -1;
            }


            return retSpdTblNo;

        }



        /// <summary>
        /// Command to set Speed Table of axis
        /// </summary>
        /// <param name="_axis"></param>
        /// <param name="_speedTableNo"></param>
        /// <returns></returns>
        private bool Sync_SetSpeedTableNo(int _axis, int _speedTableNo)
        {
            
            //Variables.. 
            bool bRet = true;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check parameter
                if ((_speedTableNo < 0) || (_speedTableNo > 9))
                {
                    return false;
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":SELectSPeed ";
                strCommand = strCommand + Convert.ToString(_speedTableNo);
                if (Sync_SendCommand(strCommand) == false)
                {
                    throw new ApplicationException();
                }


                bRet = true;

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }



        /// <summary>
        /// Command to set Speed table
        /// </summary>
        /// <param name="_st"></param>
        /// <returns></returns>
        private bool Sync_SetSpeedTable(CSpeedTable _st)
        {

            //Variables.. 
            bool bRet = true;



            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check parameter ----(Speed Table Number)---
                if ((_st.tableNo < SPEED_TABLE_0) || (_st.tableNo > SPEED_TABLE_9))
                {
                    return false;
                }




                string strCommand = "";

                //set StartVelocity
                strCommand = "";
                strCommand = "Lspeed" + Convert.ToString(_st.tableNo) + " " + Convert.ToString(_st.startVelocity);
                if (false == Sync_SendCommand(strCommand))
                {
                    throw new ApplicationException();
                }


                //set driving speed
                strCommand = "";
                strCommand = "Fspeed" + Convert.ToString(_st.tableNo) + " " + Convert.ToString(_st.drivingSpeed);
                if (false == Sync_SendCommand(strCommand))
                {
                    throw new ApplicationException();
                }



                //set Acceleration-and-Deacceleration Rate
                strCommand = "";
                strCommand = "Rate" + Convert.ToString(_st.tableNo) + " " + Convert.ToString(_st.accDeaccRate);
                if (false == Sync_SendCommand(strCommand))
                {
                    throw new ApplicationException();
                }



                //set S-Acceleration-and-Deacceleration Rate
                strCommand = "";
                strCommand = "SRate" + Convert.ToString(_st.tableNo) + " " + Convert.ToString(_st.sAccDeaccRate);
                if (false == Sync_SendCommand(strCommand))
                {
                    throw new ApplicationException();
                }


                bRet = true;

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }


        #endregion

        


        #region Public method

        
        /// <summary>
        /// progarm을 설정한다.
        /// </summary>
        /// <param name="_progNo"></param>
        /// <returns></returns>
        public bool SetProgramNo(int _progNo)
        {

            if (_progNo == m_progNo)
                return true;

            bool ret = false;

            try
            {
                ret = Sync_SetProgramNo(_progNo);
                if (ret == true)
                    m_progNo = _progNo;

            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        

        /// <summary>
        /// program을 입력한다.
        /// ex).
        /// DELPRG 0
        /// SETPRG 0,0,AXI1:PLUS 0.5
        /// SETPRG 0,1,AXI1:GO CW
        /// SETPRG 0,2,END
        /// </summary>
        /// <param name="prgStrList">program을 m.c에 입력한다.</param>
        /// <returns></returns>
        public bool SetProgram(List<string> prgStrList)
        {
            bool ret = false;

            try
            {

                for (int i = 0; i < prgStrList.Count(); i++)
                {
                    if (!Sync_SendCommand(prgStrList[i]))
                        throw new ApplicationException("");
                }


                ret = true;
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        

        /// <summary>
        /// SoftLimit 데이터를 얻는다.
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <returns> soft limit </returns>
        public CSoftLimit GetSoftLimit(int _axis)
        {

            CSoftLimit retSoftLmt = null;
            retSoftLmt = (CSoftLimit)m_softLimits[_axis];

            if (retSoftLmt == null)
            {
                retSoftLmt = Sync_GetSoftLimit(_axis);
                if (retSoftLmt == null)
                {
                    return null;
                }
                m_softLimits[_axis] = retSoftLmt;
            }

            return retSoftLmt;

        }

        

        /// <summary>
        /// SoftLimit를 설정한다.
        /// </summary>
        /// <param name="_sl">softlimit</param>
        /// <returns></returns>
        public bool SetSoftLimit(CSoftLimit _sl)
        {

            CSoftLimit prevSoftLmt = null;
            prevSoftLmt = (CSoftLimit)m_softLimits[_sl.axis];

            if ((prevSoftLmt == null) ||
                (prevSoftLmt.CWEnable != _sl.CWEnable) ||
                (prevSoftLmt.CWPosition != _sl.CWPosition) ||
                (prevSoftLmt.CCWEnable != _sl.CCWEnable) ||
                (prevSoftLmt.CCWPosition != _sl.CCWPosition))
            {

                if (false == Sync_SetSoftLimit(_sl))
                {
                    return false;
                }

                m_softLimits[_sl.axis] = _sl;
            }

            return true;

        }

        

        /// <summary>
        /// GetConstantStepPulse
        /// </summary>
        /// <param name="_axis">axis</param>
        /// <returns> constant step pulse</returns>
        public int GetConstantStepPulse(int _axis)
        {

            int retVal = 0;
            retVal = (int)m_constStepPulses[_axis];
            if (retVal == 0)
            {
                retVal = Sync_GetConstantStepPulse(_axis);
                m_constStepPulses[_axis] = retVal;
            }

            return retVal;

        }


                
        /// <summary>
        ///  Command to set Constant Step Pulse of axis
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <param name="_pulse"> const step pulse</param>
        /// <returns></returns>
        /// 
        public bool SetConstantStepPulse(int _axis, double _pulse)
        {

            //설정값이 기존값과 동일하면 그냥 나간다.
            double prevVal = 0;
            try
            {
                prevVal = (double)m_constStepPulses[_axis];
            }
            catch
            {
                prevVal = 0;
            }


            if (prevVal == _pulse)
            {
                return true;
            }


            //설정
            if (false == Sync_SetConstantStepPulse(_axis, _pulse))
            {
                return false;
            }


            //save
            m_constStepPulses[_axis] = _pulse;

            return true;
        }


        
        /// <summary>
        /// Command to request for Memory Switch State
        /// </summary>
        /// <param name="_axis"></param>
        /// <returns></returns>
        public CMemorySwitchState GetMemSwitchState(int _axis)
        {

            CMemorySwitchState retMss = null;
            retMss = (CMemorySwitchState)m_memSwStates[_axis];
            if (retMss == null)
            {
                retMss = Sync_GetMemSwitchState(_axis);
                m_memSwStates[_axis] = retMss;
            }

            return retMss;
        }

        

        /// <summary>
        /// Command to set Memory SW state for axis
        /// </summary>
        /// <param name="_mss"> memory switch state </param>
        /// <returns></returns>
        public bool SetMemSwitchState(CMemorySwitchState _mss)
        {

            CMemorySwitchState prvMss = null;
            prvMss = (CMemorySwitchState)m_memSwStates[_mss.axis];

            if ((prvMss == null) ||
                (prvMss.sw0 != _mss.sw0) || (prvMss.sw1 != _mss.sw1) ||
                (prvMss.sw2 != _mss.sw2) || (prvMss.sw3 != _mss.sw3) ||
                (prvMss.sw4 != _mss.sw4) || (prvMss.sw5 != _mss.sw5) ||
                (prvMss.sw6 != _mss.sw6) || (prvMss.sw7 != _mss.sw7))
            {

                if (false == Sync_SetMemSwitchState(_mss))
                {
                    return false;
                }

                m_memSwStates[_mss.axis] = _mss;
            }

            return true;

        }


        
        /// <summary>
        /// ommand to request for setting of display unit of axis
        /// </summary>
        /// <param name="nAxis">모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <returns> unit </returns>
        public int GetUnit(int _axis)
        {

            int retUnit = -1;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //get
                string strCommand = "";
                string strResponse = "";
                strCommand = "AXI" + Convert.ToString(_axis);   //Axis
                strCommand = strCommand + ":UNIT?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retUnit = Convert.ToInt32(strResponse);


            }
            catch
            {
                retUnit = -1;
            }

            return retUnit;

        }


        
        /// <summary>
        /// Command to set  Display Unit of axis
        /// </summary>
        /// <param name="_axis">모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <param name="_unit">unit number </param>
        /// <returns></returns>
        public bool SetUnit(int _axis, int _unit)
        {

            bool bRet = false;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Check parameter
                if ((_unit < UNIT_PULSE) | (_unit > UNIT_MRAD))
                {
                    return false;
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "AXI" + Convert.ToString(_axis);   //Axis
                strCommand += ":UNIT ";
                strCommand += Convert.ToString(_unit);
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }



            return bRet;


        }


        
        /// <summary>
        /// Command to get a dividing number of driver of axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <returns> a dividing number of drivers for axis </returns>
        public int GetDriverDivision(int _axis)
        {

            int retDivNo = 0;


            try
            {
                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Query
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":";        //Axis
                strCommand = strCommand + "DRiverDIVision?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                retDivNo = Convert.ToInt32(strResponse);

            }
            catch
            {
                retDivNo = 0;
            }


            return retDivNo;

        }
        


        /// <summary>
        /// Command to set a dividing number of driver of axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <param name="_divNo">  a dividing number of drivers for axis </param>
        /// <returns></returns>
        public bool SetDrverDivision(int _axis, int _divNo)
        {
            bool bRet = true;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":"; //Axis
                strCommand = strCommand + "DRiverDIVision " + Convert.ToString(_divNo);
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }

        

        /// <summary>
        /// Command to request for setting of home position of axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <returns> home position of each axis </returns>
        public double GetHomePosition(int _axis)
        {
            double retPos = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Query
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":HOMEPosition?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                retPos = Math.Round(Convert.ToDouble(strResponse), 4);

            }
            catch
            {
                retPos = 0;
            }


            return retPos;

        }


        
        /// <summary>
        /// Command to set for setting of home position of axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W)  </param>
        /// <param name="_position"> home position of each axis </param>
        /// <returns></returns>
        public bool SetHomePosition(int _axis, double _position)
        {

            bool bRet = true;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":";    //Axis
                strCommand += "HOMEPosition ";
                strCommand += Convert.ToString(Math.Round(_position, 1));
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }

            return bRet;

        }


        
        /// <summary>
        /// 축의 Data No를 설정한다.
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W)  </param>
        /// <param name="_dataNo"> DATA1 or DATA2 </param>
        /// <returns></returns>
        public bool SetAxisDataNo(int _axis, int _dataNo)
        {

            bool bRet = true;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return false;
                }


                //Query
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":DATA ";
                strCommand = strCommand + Convert.ToString(_dataNo);
                bRet = Sync_SendCommand(strCommand);

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }

        

        /// <summary>
        /// 현재 축의 Data No를 얻는다.
        /// </summary>
        /// <param name="_axis"></param>
        /// <returns></returns>
        public int GetAxisDataNo(int _axis)
        {

            int retDataNo = 0;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Query
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":DATA?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                retDataNo = Convert.ToInt32(strResponse);

            }
            catch
            {
                retDataNo = 0;
            }


            return retDataNo;

        }


        
        /// <summary>
        ///  Command to request for setting of travel distance per pulse of axis
        ///  (= resolution of motorized stage/dividing number of driver )
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W)  </param>
        /// <returns> resolution </returns>
        public double GetCurrentResolution(int _axis)
        {

            double retResolution = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Query
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":RESOLUTion?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retResolution = Convert.ToDouble(strResponse);


            }
            catch
            {
                retResolution = 0;
            }

            return retResolution;

        }


        
        /// <summary>
        ///  Command to set standrad resoultion of motorized stage
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <returns> standard resolution</returns>
        public double GetStandardResoultion(int _axis)
        {

            double dbRetVal = 0;
            try
            {
                dbRetVal = (double)m_stdRess[_axis];
            }
            catch
            {
                dbRetVal = 0;
            }


            if (dbRetVal == 0)
            {
                dbRetVal = Sync_GetStandardResolution(_axis);
                m_stdRess[_axis] = dbRetVal;
            }

            return dbRetVal;

        }

        

        /// <summary>
        /// standard resolution을 설정한다.
        /// </summary>
        /// <param name="_axis"></param>
        /// <param name="_resolut"></param>
        /// <returns></returns>
        public bool SetStandardResolution(int _axis, double _resolut)
        {

            double prevStdRes = 0;
            try
            {
                prevStdRes = (double)m_stdRess[_axis];
            }
            catch
            {
                prevStdRes = 0;
            }


            if (Math.Round(prevStdRes, 4) != Math.Round(_resolut, 4))
            {

                if (false == Sync_SetStandardResolution(_axis, _resolut))
                {
                    return false;
                }

                m_stdRess[_axis] = _resolut;
            }

            return true;

        }
        


        /// <summary>
        /// 축의 speed table no.를 얻는다.
        /// </summary>
        /// <param name="nAxis"></param>
        /// <returns> table number ,  default -1</returns>
        public int GetSpeedTableNo(int _axis)
        {

            int nRetVal = -1;

            //hashtable에서 먼저 찾는다.
            try
            {
                nRetVal = (int)m_spdTblNos[_axis];
            }
            catch
            {
                nRetVal = -1;
            }

            //hashTable에 없으면 장비에게 물어본다.
            if (nRetVal == -1)
            {
                nRetVal = Sync_GetSpeedTableNo(_axis);
                m_spdTblNos[_axis] = nRetVal;
            }

            return nRetVal;
        }
        


        /// <summary>
        /// 축의 speed table no.를 설정한다.
        /// </summary>
        /// <param name="_axis"></param>
        /// <param name="_speedTableNo"></param>
        /// <returns></returns>
        public bool SetSpeedTableNo(int _axis, int _speedTableNo)
        {

            int prevSpdTblNo = -1;
            try
            {
                prevSpdTblNo = (int)m_spdTblNos[_axis];
            }
            catch {/* do nothing*/}


            if ((prevSpdTblNo == -1) || (prevSpdTblNo != _speedTableNo))
            {
                if (false == Sync_SetSpeedTableNo(_axis, _speedTableNo))
                {
                    return false;
                }

                m_spdTblNos[_axis] = _speedTableNo;
            }

            return true;

        }

        

        /// <summary>
        /// Command to request setting of Speed Table
        /// hashtable에 존재하면 바로 아니면 장비에서 데이터를 얻는다.
        /// </summary>
        /// <param name="nTableNo"> table Number</param>
        /// <returns> speed table no.</returns>
        public CSpeedTable GetSpeedTable(int _tableNo)
        {

            CSpeedTable retSpdTbl = null;
            retSpdTbl = (CSpeedTable)m_speedTables[_tableNo];
            if (retSpdTbl == null)
            {
                retSpdTbl = Sync_GetSpeedTable(_tableNo);

                if (retSpdTbl != null)
                {
                    m_speedTables[_tableNo] = retSpdTbl;
                }
            }

            return retSpdTbl;
        }

        

        /// <summary>
        /// Command to set Speed table
        /// </summary>
        /// <param name="_st"> speed table</param>
        /// <returns></returns>
        public bool SetSpeedTable(CSpeedTable _st)
        {

            CSpeedTable prevSpdTbl = null;
            prevSpdTbl = (CSpeedTable)m_speedTables[_st.tableNo];

            if ((prevSpdTbl == null) ||
                (prevSpdTbl.startVelocity != _st.startVelocity) ||
                (prevSpdTbl.drivingSpeed != _st.drivingSpeed) ||
                (prevSpdTbl.accDeaccRate != _st.accDeaccRate) ||
                (prevSpdTbl.sAccDeaccRate != _st.sAccDeaccRate))
            {

                if (false == Sync_SetSpeedTable(_st))
                {
                    return false;
                }

                m_speedTables[_st.tableNo] = _st;

            }

            return true;

        }
        


        /// <summary>
        /// Command to request for setting of Current position of axis
        /// </summary>
        /// <param name="_axis">모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <param name="_position"></param>
        /// <returns> current position of each axis   -99999999 ~ 99999999 , -9.9999999 ~ 9.9999999 </returns>
        public bool SetCurrentPosition(int _axis, double _position)
        {

            bool bRet = false;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis);  //Axis
                strCommand = strCommand + ":Position ";
                strCommand = strCommand + Convert.ToString(Math.Round(_position, 4));
                bRet = Sync_SendCommand(strCommand);


            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }

        

        /// <summary>
        /// Command for Driver type
        /// </summary>
        /// <returns> driver type</returns>
        public int GetDriverType()
        {

            int retDrvType = 0;


            try
            {
                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "DriverTYpe?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }
                retDrvType = Convert.ToInt32(strResponse);


            }
            catch
            {
                retDrvType = 0;
            }

            return retDrvType;

        }
        


        /// <summary>
        /// 축이 현재 Motion controller에 연결되어있는지 확인한다.!!
        /// </summary>
        /// <param name="_axis"></param>
        /// <returns> true : loaded , false : not loaded</returns>
        public bool IsAxisLoadedOK(int _axis)
        {

            bool bRet = false;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }

                //Check parameter
                if ((_axis < MOTOR_AXIS_X) || (_axis > MOTOR_AXIS_W))
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":READY?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                {
                    throw new ApplicationException();
                }

                if (Convert.ToInt32(strResponse) > 0)
                {
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }


            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }

        

        //////////////////////////////////////////////////////////////
        //Async_Go ////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////
        //desc -  Driving commands for each axis
        //
        //Param - [IN] nAxis : 모터 축.( ex. X,Y,Z,U,V,W) 
        //            [IN] nDirection : 
        //
        public bool Async_Go(int nAxis, int nDirection)
        {

            //Variables.. 
            bool bRet = true;



            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }



                //Command를 날린다.
                string strCommand = "";
                if (nAxis != MOTOR_AXIS_ALL)
                {
                    strCommand = "AXI" + Convert.ToString(nAxis) + ":";
                }
                else
                {
                    strCommand = "AXIALL:";
                }
                strCommand = strCommand + "GO ";
                strCommand = strCommand + Convert.ToString(nDirection);
                bRet = Sync_SendCommand(strCommand);


                //usb나 rs232로 통신하는 경우는 Z,U,V,W 축에 이동명령 후에는
                //약 190ms 기다려줘야 한다. 
                //190ms 이전에 이동중인지 물어보면 멈춤으로 답을 줌.
                //if (m_nConnctionMethod == CONNECTION_METHOD_USB_RS232)
                //{

                //    if (nAxis > MOTOR_AXIS_Y)
                //    {
                //        Thread.Sleep(190);
                //    }

                //}



            }
            catch
            {
                bRet = false;
            }


            //return
            return bRet;

        }



        /// <summary>
        /// 연결상태 여부를 알아본다.
        /// </summary>
        /// <returns> Connected:True , Disconnected: False</returns>
        public bool IsConnectedOK()
        {
            return m_bConnectedOK;
        }

        

        /// <summary>
        /// 연결 방법을 얻는다.!!
        /// </summary>
        /// <returns></returns>
        public int GetConnectionMethod()
        {
            return m_nConnctionMethod;
        }


        
        /// <summary>
        /// Controller에 USB나 RS232로 연결한다.!!
        /// </summary>
        /// <param name="nComportNo"> COM Port Number </param>
        /// <returns> True : Connection is completed ,  false:Connection is fail. </returns>
        public bool ConnectByUSB_RS232(int nComportNo)
        {

            bool bRet = true;

            try
            {

                //Check Connection
                if (true == m_bConnectedOK)
                {
                    return true;
                }


                //SerialPort 객체 생성
                if ((m_sp == null))
                {
                    m_sp = new SerialPort();
                }


                // Setup parameters
                m_sp.PortName = "COM" + Convert.ToString(nComportNo);
                m_sp.BaudRate = 38400;
                m_sp.DataBits = 8;
                m_sp.StopBits = StopBits.One;
                m_sp.Parity = Parity.None;
                m_sp.ReadTimeout = 200;
                m_sp.NewLine = "\r";




                //port open
                if (m_sp.IsOpen == true)
                {
                    m_sp.Close();
                }
                m_sp.Open();


                m_nConnctionMethod = CONNECTION_METHOD_USB_RS232;
                m_bConnectedOK = true;



                //연결 확인.
                string strCommand = "";
                string strResponse = "";
                strCommand = "*IDN?";
                strResponse = Query(strCommand);
                if (strResponse.IndexOf("SURUGA") < 0)
                {
                    throw new ApplicationException();
                }



                bRet = true;

            }
            catch
            {

                //메모리 해제
                if (m_sp != null)
                {
                    m_sp.Dispose();
                    m_sp = null;
                }


                m_nConnctionMethod = CONNECTION_METHOD_NONE;
                m_bConnectedOK = false;
                bRet = false;

            }


            return bRet;

        }


        
        /// <summary>
        /// GPIB로 연결한다.
        /// </summary>
        /// <param name="nGpibAddr"> gpib address</param>
        /// <returns> True : Connection is completed ,  false:Connection is fail.  </returns>
        public bool ConnectByGPIB(int _gpibAddr)
        {

            bool bRet = true;

            try
            {


                //gpib 객체 생성 및 연결
                if (m_gpibDev == null)
                {
                    m_gpibDev = new Device(0, Convert.ToByte(_gpibAddr));
                }


                m_nGPIBaddr = _gpibAddr;
                m_bConnectedOK = true;
                m_nConnctionMethod = CONNECTION_METHOD_GPIB;



                //연결 확인.
                string strCommand = "";
                string strResponse = "";
                strCommand = "*IDN?";
                strResponse = Query(strCommand);
                if (strResponse.IndexOf("SURUGA") < 0)
                {
                    throw new ApplicationException();
                }


            }
            catch
            {

                //메모리 해제
                if (m_gpibDev != null)
                {
                    m_gpibDev.Dispose();
                    m_gpibDev = null;
                }

                m_nGPIBaddr = -1;
                m_bConnectedOK = false;
                m_nConnctionMethod = CONNECTION_METHOD_NONE;

                bRet = false;

            }


            return bRet;


        }

        

        /// <summary>
        /// Controller와 접속을 끊는다.
        /// </summary>
        public void Disconnect()
        {

            //Connection을 끊는다.!!
            switch (m_nConnctionMethod)
            {

                case CONNECTION_METHOD_USB_RS232:
                    //USB or RS232  Connection을 끊는다.

                    if (m_sp != null)
                    {
                        m_sp.Close();
                        m_sp = null;
                    }

                    break;

                case CONNECTION_METHOD_GPIB:
                    //GPIB Connection을 끊는다.


                    if (m_gpibDev != null)
                    {
                        m_gpibDev.Dispose();
                        m_gpibDev = null;

                    }

                    break;
            }



            //멤버변수를 초기화 한다.!!
            m_bConnectedOK = false;
            m_nConnctionMethod = CONNECTION_METHOD_NONE;
            m_nGPIBaddr = -1;

            m_softLimits.Clear();
            m_memSwStates.Clear();
            m_speedTables.Clear();
            m_spdTblNos.Clear();
            m_stdRess.Clear();
            m_constStepPulses.Clear();

        }



        /// <summary>
        /// Command to request for settg of Current position of axis
        /// </summary>
        /// <param name="nAxis">모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <returns> current postion</returns>
        public double GetCurrentPosition(int _axis)
        {

            double retPos = 0;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();


                //Command를 날린다.
                string strCommand = "";
                string strResponse = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":";    //Axis
                strCommand += "Position?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                    throw new ApplicationException();

                retPos = Convert.ToDouble(strResponse);

            }
            catch
            {
                retPos = 0;
            }

            return retPos;

        }

        

        /// <summary>
        /// Stage(모든 축) 가 현재 이동중인지 알아본다. 
        /// </summary>
        /// <param name="bMotionState"></param>
        /// <returns></returns>
        public bool IsInMotionOK()
        {

            bool bRet = false;


            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();


                //Query~
                string strCommand = "";
                string strResponse = "";
                strCommand = "MOTIONALL?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                    throw new ApplicationException();

                if (Convert.ToInt32(strResponse) == 0)
                    bRet = false;   //멈춤
                else
                    bRet = true;    //이동중

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }
        


        /// <summary>
        /// 특정 축의 Motor가 현재 이동중인지 알아본다.
        /// </summary>
        /// <param name="_axis"> axis </param>
        /// <returns>true : in motion , false :At halt</returns>
        public bool IsInMotionOK(int _axis)
        {

            bool bRet = true;



            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    throw new ApplicationException();


                //Query~
                string strCommand = "";
                string strResponse = "";
                strCommand = "AXIS" + Convert.ToString(_axis) + ":MOTION?";
                strResponse = Query(strCommand);
                if (strResponse == "")
                    throw new ApplicationException();


                if (Convert.ToInt32(strResponse) == 0)
                    bRet = false;//멈춤//
                else
                    bRet = true; //이동중//

            }
            catch
            {
                bRet = false;
            }


            return bRet;


        }


        
        /// <summary>
        /// Driving commands for each axis
        /// </summary>
        /// <param name="_axis"> 모터 축.( ex. X,Y,Z,U,V,W)  </param>
        /// <param name="_absPos"> 절대 좌표값. </param>
        /// <returns></returns>
        public bool Async_AbsGo(int _axis, double _absPos)
        {

            bool bRet = true;

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis) + ":";    //Axis
                strCommand = strCommand + "GOABSolute ";
                strCommand = strCommand + Convert.ToString(_absPos);
                bRet = Sync_SendCommand(strCommand);


                bRet = true;

            }
            catch
            {
                bRet = false;
            }


            return bRet;

        }


        
        /// <summary>
        /// This command conducts emergency-stop or slowdown-stop
        /// </summary>
        /// <param name="_axis">nAxis : 모터 축.( ex. X,Y,Z,U,V,W) </param>
        /// <param name="_stopType"></param>
        public void Stop(int _axis, int _stopType)
        {

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return;
                }


                //Check parameter ( Motor Axis )
                if ((_axis < MOTOR_AXIS_X) || (_axis > MOTOR_AXIS_W))
                {
                    throw new ApplicationException();
                }


                //Check parameter ( Absolute position )
                if ((_stopType < STOP_TYPE_EMERGENCY) || (_stopType > STOP_TYPE_SLOWDOWN))
                {
                    throw new ApplicationException();
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "Axis" + Convert.ToString(_axis);      //Axis
                strCommand = strCommand + ":STOP ";
                strCommand = strCommand + Convert.ToString(_stopType);
                Sync_SendCommand(strCommand);

                Thread.Sleep(10);

            }
            catch
            {
                //do nothing
            }

        }


        
        /// <summary>
        /// This command conducts emergency-stop or slowdown-stop
        /// </summary>
        /// <param name="nStopType">STOP_TYPE_EMERGENCY , STOP_TYPE_SLOWDOWN</param>
        public void Stop(int _stopType)
        {

            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                {
                    return;
                }


                //Check parameter ( Absolute position )
                if ((_stopType < STOP_TYPE_EMERGENCY) | (_stopType > STOP_TYPE_SLOWDOWN))
                {
                    return;
                }


                //Command를 날린다.
                string strCommand = "";
                strCommand = "AXIALL:STOP ";
                strCommand = strCommand + Convert.ToString(_stopType);
                Sync_SendCommand(strCommand);
                Thread.Sleep(50);


            }
            catch
            {
                // do nothing
            }

        }

        

        /// <summary>
        /// progarm을 실행한다.
        /// </summary>
        public void StartProgram()
        {
            try
            {

                //Check Connection
                if (false == m_bConnectedOK)
                    return;

                //Command를 날린다.
                string strCommand = "PRG 0";
                Sync_SendCommand(strCommand);


            }
            catch
            {
                // do nothing
            }
        }



        #endregion

    }


}
