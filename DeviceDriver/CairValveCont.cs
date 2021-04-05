namespace Neon.Aligner
{
    public class CairValveCont : IairValvController
    {
        private const int VALVE1 = (int)AirValveAligner.Input;
        private const int VALVE2 = (int)AirValveAligner.Output;
        public int valveCnt { get { return 2; } }

        #region Member variables

        private Daq m_daq = null;

        private int m_valve1Dino;   //value 1 digital io no.
        private int m_valve2Dino;   //value 2 digital io no.

        private AirValveState m_valve1State;
        private AirValveState m_valve2State;

        #endregion



        #region Public mehthod

        /// <summary>
        /// initalization.
        /// </summary>
        /// <param name="_daq">NI-daq instance</param>
        /// <param name="_sensAiNo"></param>
        /// <returns></returns>
        public bool Init(Daq _daq, int _diNo1, int _diNo2)
        {

            bool bRet = false;

            if (_daq == null)
                return false;

            try
            {
                //채널 생성.
                _daq.CreateDoCh(_diNo1);
                _daq.CreateDoCh(_diNo2);


                //asign member value 
                m_daq = _daq;
                m_valve1Dino = _diNo1;
                m_valve2Dino = _diNo2;


                //valve close..
                CloseValve(VALVE1);
                CloseValve(VALVE2);

                bRet = true;

            }
            catch
            {
                m_daq = null;
                m_valve1Dino = -1;
                m_valve2Dino = -1;
                bRet = false;
            }


            return bRet;


        }



        /// <summary>
        /// valve를 연다.
        /// </summary>
        /// <param name="_valve"> valve1 or valve2 </param>
        public void OpenValve(int _valve)
        {

            try
            {

                //digital 
                int doNo = -1;
                if (_valve == VALVE1)
                    doNo = m_valve1Dino;
                else if (_valve == VALVE2)
                    doNo = m_valve2Dino;

                //exc.
                m_daq.WriteDo(doNo, true);


                //state
                if (_valve == VALVE1)
                    m_valve1State = AirValveState.open;
                else if (_valve == VALVE2)
                    m_valve2State = AirValveState.open;

            }
            catch
            {
                if (_valve == VALVE1)
                    m_valve1State = AirValveState.close;
                else if (_valve == VALVE2)
                    m_valve2State = AirValveState.close;
            }


        }




        /// <summary>
        /// 모든 valve를 연다.
        /// </summary>
        /// <param name="_valve"> valve1 or valve2 </param>
        public void OpenValve()
        {
            OpenValve(VALVE1);
            OpenValve(VALVE2);
        }



        /// <summary>
        /// valve를 닫는다.
        /// </summary>
        /// <param name="_valve"> valve1 or valve2 </param>
        public void CloseValve(int _valve)
        {
            try
            {

                //digital out line no.
                int doNo = -1;
                if (_valve == VALVE1)
                    doNo = m_valve1Dino;
                else if (_valve == VALVE2)
                    doNo = m_valve2Dino;

                //exec.
                m_daq.WriteDo(doNo, false);


                //state
                if (_valve == VALVE1)
                    m_valve1State = AirValveState.close;
                else if (_valve == VALVE2)
                    m_valve2State = AirValveState.close;


            }
            catch
            {
                if (_valve == VALVE1)
                    m_valve1State = AirValveState.close;
                else if (_valve == VALVE2)
                    m_valve2State = AirValveState.close;
            }


        }



        /// <summary>
        /// 모든 밸브를 닫는다.
        /// </summary>
        public void CloseValve()
        {
            CloseValve(VALVE1);
            CloseValve(VALVE2);
        }



        /// <summary>
        /// valve의 상태를 얻는다.!!
        /// </summary>
        /// <param name="_valve">valve no.</param>
        /// <returns></returns>
        public AirValveState ValveState(int _valve)
        {

            AirValveState ret = AirValveState.close;


            if (_valve == VALVE1)
                ret = m_valve1State;
            else if (_valve == VALVE2)
                ret = m_valve2State;

            return ret;

        }



        #endregion

    }
}