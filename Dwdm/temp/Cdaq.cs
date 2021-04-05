using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NationalInstruments.DAQmx;



public class Daq : IDisposable
{


    #region definition

    private const int SAMPLING_RATE = 1000;
    private const int SAMPLESPERCHANNEL = 2;    //2는 최소값임 

    #endregion



    #region structure/innor class


    /// <summary>
    /// AI channel
    /// </summary>
    private class CaiChannel
    {

        #region member variables

        private int m_chnlNo;    //0,1,...
        private Task m_task;   //di task;
        private AnalogSingleChannelReader m_reader;

        private bool m_disposed = false;

        #endregion



        #region property

        public int chnlNo { get { return m_chnlNo; } }

        #endregion



        #region constructor/desconstructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CaiChannel()
        {
            m_chnlNo = -1;
            m_task = null;
            m_reader = null;
        }


        /// <summary>
        /// desconstctor
        /// This destructor will run only if the Dispose method 
        /// does not get called. 
        /// </summary>
        ~CaiChannel()
        {
            try
            {
                // Do not re-create Dispose clean-up code here. 
                // Calling Dispose(false) is optimal in terms of 
                // readability and maintainability.
                Dispose(false);
            }
            catch
            {
                //log here
            }
        }


        #endregion


        #region private/poretected method


        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {

            // Check to see if Dispose has already been called. 
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_task != null)
                    {
                        m_task.Stop();
                        m_task.Dispose();
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_disposed = true;
            }

        }


        #endregion



        #region public method


        /// <summary>
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 1.task 생성.
        /// 2.channel 생성.
        /// 3.reader생성.
        /// 4.task 시작!!
        /// </summary>
        /// <param name="_chnlNo">channel no.</param>
        /// <param name="_min">minimum signal value.</param>
        /// <param name="_max">maximum signal value.</param>
        /// <returns></returns>
        public bool Create(int _chnlNo, double _min, double _max)
        {

            bool ret = false;


            try
            {

                //clean task.
                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }
                m_task = null;


                //Create a task
                m_task = new Task();


                //Create channel & reader.
                string strPhyChName = "Dev1/ai" + _chnlNo.ToString(); //채널의 물리적 이름
                string strChName = "ch" + _chnlNo.ToString(); //채널 이름
                m_task.AIChannels.CreateVoltageChannel(strPhyChName,
                                                        strChName,
                                                        AITerminalConfiguration.Differential,
                                                        _min,
                                                        _max,
                                                        AIVoltageUnits.Volts);


                m_reader = new AnalogSingleChannelReader(m_task.Stream);

                m_chnlNo = _chnlNo;

                ret = true;

            }
            catch
            {

                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }

                m_task = null;
                m_reader = null;
                m_chnlNo = -1;

                ret = false;
            }

            return ret;
        }




        /// <summary>
        /// read a data
        /// </summary>
        /// <returns> analog voltage value </returns>
        public double Read()
        {
            double ret = 0;

            try
            {
                ret = m_reader.ReadSingleSample();
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }


        #endregion



    }




    /// <summary>
    /// single line DI channel 
    /// line no를 매개변수로 생성하거나
    /// create method하면 바로 task시작된다.
    /// </summary>
    private class CdiSingLineChannel : IDisposable
    {

        #region member variables

        private int m_line;    //0,1,...
        private Task m_task;   //di task;
        private DigitalSingleChannelReader m_reader;

        private bool m_disposed = false;

        #endregion


        #region property

        public int line { get { return m_line; } }

        #endregion



        #region constructor/desconstructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CdiSingLineChannel()
        {
            m_line = -1;
            m_task = null;
            m_reader = null;
        }



        /// <summary>
        /// desconstctor
        /// This destructor will run only if the Dispose method 
        /// does not get called. 
        /// It gives your base class the opportunity to finalize. 
        /// Do not provide destructors in types derived from this class.
        /// 
        /// </summary>
        ~CdiSingLineChannel()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }


        #endregion



        #region private/poretected method


        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {

            // Check to see if Dispose has already been called. 
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_task != null)
                    {
                        m_task.Stop();
                        m_task.Dispose();
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_disposed = true;
            }

        }


        #endregion



        #region public method


        /// <summary>
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 1.task 생성.
        /// 2.channel 생성.
        /// 3.reader생성.
        /// 4.task 시작!!
        /// </summary>
        /// <param name="_line">line no.</param>
        /// <returns></returns>
        public bool Create(int _line)
        {

            bool ret = false;


            try
            {

                //clean task.
                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }
                m_task = null;


                //Create a task
                m_task = new Task();


                //Create channel & reader.
                string strPhyChName = "Dev1/port0/line" + _line.ToString(); //채널의 물리적 이름
                string strChName = "ch_port0_" + _line.ToString(); //채널 이름
                m_task.DIChannels.CreateChannel(strPhyChName,
                                            strChName,
                                            ChannelLineGrouping.OneChannelForEachLine);


                m_reader = new DigitalSingleChannelReader(m_task.Stream);

                m_line = _line;

                ret = true;

            }
            catch
            {

                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }

                m_task = null;
                m_reader = null;
                m_line = -1;

                ret = false;
            }

            return ret;
        }




        /// <summary>
        /// read a data
        /// </summary>
        /// <returns> true : 1 , false : 0 </returns>
        public bool Read()
        {
            bool ret = false;

            try
            {

                ret = m_reader.ReadSingleSampleSingleLine();
            }
            catch
            {
                ret = false;
            }

            return ret;
        }


        #endregion

    }


    /// <summary>
    /// multi line DI channel 
    /// </summary>
    private class CdiMultiLineChannel : IDisposable
    {

        #region member variables

        private string m_strlines;    // ex)line0:7 
        private int[] m_lines;
        private Task m_task;   //di task;
        private DigitalSingleChannelReader m_reader;

        private bool m_disposed = false;

        #endregion


        #region property


        public string lineStr { get { return m_strlines; } }

        public int lineCnt
        {
            get
            {
                int cnt = 0;
                try
                {
                    cnt = m_lines.Length;
                }
                catch
                {
                    cnt = 0;
                }
                return cnt;
            }
        }

        #endregion


        #region constructor/desconstructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CdiMultiLineChannel()
        {
            m_strlines = "";
            m_lines = null;
            m_task = null;
            m_reader = null;
        }


        /// <summary>
        /// desconstructor
        /// </summary>
        ~CdiMultiLineChannel()
        {
            Dispose(false);
        }

        #endregion


        #region private/poretected method


        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {

            // Check to see if Dispose has already been called. 
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_task != null)
                    {
                        m_task.Stop();
                        m_task.Dispose();
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_disposed = true;
            }

        }


        #endregion


        #region method


        /// <summary>
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 1.task 생성.
        /// 2.channel 생성.
        /// 3.reader생성.
        /// </summary>
        /// <param name="_lines">line no. array</param>
        /// <returns></returns>
        public bool Create(int[] _lines)
        {

            bool ret = false;


            try
            {

                //clean task.
                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }
                m_task = null;


                //Create a task
                m_task = new Task();


                //Create channel & reader.
                string strlineNo = _lines[0].ToString() + ":" + _lines[m_lines.Length - 1].ToString();
                string strPhyChName = "Dev1/port0/line" + strlineNo; //채널의 물리적 이름
                string strChName = "ch_port0_" + strlineNo; //채널 이름
                m_task.DIChannels.CreateChannel(strPhyChName,
                                            strChName,
                                            ChannelLineGrouping.OneChannelForAllLines);


                m_reader = new DigitalSingleChannelReader(m_task.Stream);

                m_lines = _lines;
                m_strlines = strlineNo;

                ret = true;

            }
            catch
            {

                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }

                m_task = null;
                m_reader = null;
                m_lines = null;
                m_strlines = "";

                ret = false;
            }

            return ret;
        }




        /// <summary>
        /// read a data
        /// </summary>
        /// <returns> </returns>
        public int Read()
        {
            int ret = 0;

            try
            {

                ret = (int)(m_reader.ReadSingleSamplePortUInt32());
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }


        #endregion

    }


    /// <summary>
    /// single line DO(digital out) channel 
    /// </summary>
    private class CdoSingLineChannel : IDisposable
    {

        #region member variables

        private int m_line;    //0,1,...
        private Task m_task;   //do task;
        private DigitalSingleChannelWriter m_writer;

        private bool m_disposed;

        #endregion


        #region property

        public int line { get { return m_line; } }

        #endregion

        #region constructor/desconstructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CdoSingLineChannel()
        {
            m_line = -1;
            m_task = null;
            m_writer = null;
            m_disposed = false;
        }



        /// <summary>
        /// desconstctor
        /// This destructor will run only if the Dispose method 
        /// does not get called. 
        /// It gives your base class the opportunity to finalize. 
        /// Do not provide destructors in types derived from this class.
        /// 
        /// </summary>
        ~CdoSingLineChannel()
        {
            // Do not re-create Dispose clean-up code here. 
            // Calling Dispose(false) is optimal in terms of 
            // readability and maintainability.
            Dispose(false);
        }


        #endregion


        #region private/poretected method


        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {

            // Check to see if Dispose has already been called. 
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_task != null)
                    {
                        m_task.Stop();
                        m_task.Dispose();
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_disposed = true;
            }

        }


        #endregion


        #region public method


        /// <summary>
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 1.task 생성.
        /// 2.channel 생성.
        /// 3.reader생성.
        /// 4.task 시작!!
        /// </summary>
        /// <param name="_line">line no.</param>
        /// <returns></returns>
        public bool Create(int _line)
        {

            bool ret = false;


            try
            {

                //clean task.
                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }
                m_task = null;


                //Create a task
                m_task = new Task();


                //Create channel & reader.
                string strPhyChName = "Dev1/port0/line" + _line.ToString(); //채널의 물리적 이름
                string strChName = "ch_port0_" + _line.ToString(); //채널 이름
                m_task.DOChannels.CreateChannel(strPhyChName,
                                            strChName,
                                            ChannelLineGrouping.OneChannelForEachLine);


                m_writer = new DigitalSingleChannelWriter(m_task.Stream);

                m_line = _line;

                ret = true;

            }
            catch
            {

                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }

                m_task = null;
                m_writer = null;
                m_line = -1;

                ret = false;
            }

            return ret;
        }



        /// <summary>
        /// write datas to DI multi lines.
        /// </summary>
        /// <param name="datas"> array of Boolean samples to write to the task. </param>
        /// <returns> </returns>
        public void Write(bool _data)
        {
            try
            {
                m_writer.WriteSingleSampleSingleLine(true, _data);
            }
            catch
            {
                //do nothing.
            }
        }




        #endregion

    }



    /// <summary>
    /// multi line DO(digital out) channel 
    /// </summary>
    private class CdoMultiLineChannel : IDisposable
    {

        #region member variables

        private string m_strlines;    // ex)line0:7 
        private int[] m_lines;
        private int m_lineCnt;
        private Task m_task;   //di task;
        private DigitalSingleChannelWriter m_writer;

        private bool m_disposed = false;

        #endregion


        #region property


        public string lineStr { get { return m_strlines; } }

        public int lineCnt
        {
            get
            {
                int cnt = 0;
                try
                {
                    cnt = m_lines.Length;
                }
                catch
                {
                    cnt = 0;
                }
                return cnt;
            }
        }

        #endregion


        #region constructor/desconstructor


        /// <summary>
        /// default constructor
        /// </summary>
        public CdoMultiLineChannel()
        {
            m_strlines = "";
            m_lines = null;
            m_task = null;
            m_writer = null;
        }



        /// <summary>
        /// desconstructor
        /// </summary>
        ~CdoMultiLineChannel()
        {
            Dispose(false);
        }

        #endregion


        #region private/poretected method


        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {

            // Check to see if Dispose has already been called. 
            if (!m_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    if (m_task != null)
                    {
                        m_task.Stop();
                        m_task.Dispose();
                    }
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                //CloseHandle(handle);
                //handle = IntPtr.Zero;

                // Note disposing has been done.
                m_disposed = true;
            }

        }


        #endregion


        #region method


        /// <summary>
        /// Implement IDisposable. 
        /// Do not make this method virtual. 
        /// A derived class should not be able to override this method. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// 1.task 생성.
        /// 2.channel 생성.
        /// 3.reader생성.
        /// </summary>
        /// <param name="_lineStr">line no. string  ex)0:7 </param>
        /// <returns></returns>
        public bool Create(string _lineStr)
        {

            bool ret = false;


            try
            {

                //clean task.
                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }
                m_task = null;


                //Create a task
                m_task = new Task();


                //Create channel & reader.
                string strPhyChName = "Dev1/Port0/line" + _lineStr; //채널의 물리적 이름
                string strChName = "ch_port0_" + _lineStr; //채널 이름
                m_task.DOChannels.CreateChannel(strPhyChName,
                                            strChName,
                                            ChannelLineGrouping.OneChannelForAllLines);

                m_writer = new DigitalSingleChannelWriter(m_task.Stream);



                //init other member variable.
                string[] temps = null;
                temps = _lineStr.Split(':');
                int startLine = Convert.ToInt32(temps[0]);
                int endLine = Convert.ToInt32(temps[temps.Length - 1]);
                m_lines = new int[endLine - startLine + 1];
                for (int i = 0; i < m_lines.Length; i++)
                {
                    if (i == 0)
                        m_lines[i] = startLine;
                    else
                        m_lines[i] = m_lines[i - 1] + 1;
                }

                m_lineCnt = m_lines.Length;
                m_strlines = _lineStr;


                ret = true;

            }
            catch
            {

                if (m_task != null)
                {
                    m_task.Stop();
                    m_task.Dispose();
                }

                m_task = null;
                m_writer = null;
                m_lines = null;
                m_strlines = "";

                ret = false;
            }

            return ret;
        }




        /// <summary>
        /// write datas to DI multi lines.
        /// </summary>
        /// <param name="datas"> array of Boolean samples to write to the task. </param>
        /// <returns> </returns>
        public void Write(bool[] datas)
        {

            try
            {
                m_writer.WriteSingleSampleMultiLine(true, datas);
            }
            catch
            {
                //do nothing.
            }

        }


        /// <summary>
        /// write datas to DI multi lines.
        /// </summary>
        /// <param name="data"></param>
        public void Write(UInt32 data)
        {

            //int -> bool array.
            bool[] sendDatas = null;
            sendDatas = new bool[m_lineCnt];

            for (int i = 0; i < m_lineCnt; i++)
            {
                if ((data & ((uint)Math.Pow(2, i))) > 0)
                    sendDatas[i] = true;
                else
                    sendDatas[i] = false;
            }



            //write
            try
            {
                m_writer.WriteSingleSampleMultiLine(true, sendDatas);
            }
            catch
            {
                //do nothing.
            }

        }

        #endregion

    }



    #endregion






    #region Private Member variables


    private List<CaiChannel> m_aiSingList;

    private List<CdiSingLineChannel> m_diSingChList;
    private List<CdiMultiLineChannel> m_diMultiChList;

    private List<CdoSingLineChannel> m_doSingChList;
    private List<CdoMultiLineChannel> m_doMultiChList;


    private bool m_disposed;

    #endregion




    #region constructor/desconstructor


    /// <summary>
    /// constructor
    /// </summary>
    public Daq()
    {
        m_aiSingList = null;
        m_diSingChList = null;
        m_diMultiChList = null;
        m_doSingChList = null;
        m_doMultiChList = null;

        m_disposed = false;
    }



    /// <summary>
    /// desconstructor
    /// </summary>
    ~Daq()
    {
        Dispose(false);
    }


    #endregion

    


    #region public method


    /// <summary>
    /// digital input 채널을 만들고 (single )
    /// task를 Start시킨다.
    /// </summary>
    /// <param name="_aiNo">analog input no.</param>
    /// <returns></returns>
    public bool CreateAiCh(int _aiNo, double _min, double _max)
    {
        bool ret = false;
        try
        {
            CaiChannel aiCh = new CaiChannel();
            if (aiCh.Create(_aiNo, _min, _max) == false) throw new ApplicationException();

            //add it to list.
            if (m_aiSingList == null) m_aiSingList = new List<CaiChannel>();
            m_aiSingList.Add(aiCh);

            ret = true;
        }
        catch(Exception ex)
        {
            ret = false;
            ex.Message.ToString();
        }


        return ret;


    }
    
    

    /// <summary>
    /// analog input으로 데이터를 하나 읽어들인다. 
    /// </summary>
    /// <param name="_aiNo">analog input no.</param>
    /// <returns></returns>
    public double ReadAi(int _aiNo)
    {
        double ret = 0;

        try
        {
            CaiChannel ai = m_aiSingList.Find(p => p.chnlNo == _aiNo);
            if (ai == null) throw new ApplicationException();

            ret = ai.Read();
        }
        catch
        {
            ret = 0;
        }
        return ret;
    }



    #endregion



    #region ==== IDispose ====


    /// <summary>
    /// Dispose managed resources.
    /// Dispose(bool disposing) executes in two distinct scenarios. 
    /// If disposing equals true, the method has been called directly 
    /// or indirectly by a user's code. Managed and unmanaged resources 
    /// can be disposed. 
    /// If disposing equals false, the method has been called by the 
    /// runtime from inside the finalizer and you should not reference 
    /// other objects. Only unmanaged resources can be disposed. 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (m_disposed) return;

        if (disposing)
        {
            //analog input single
            if (m_aiSingList != null) foreach (var ch in m_aiSingList) ch.Dispose();

            //digital input single
            if (m_diSingChList != null) foreach (var ch in m_diSingChList) ch.Dispose();

            //digital input multi
            if (m_diMultiChList != null) foreach (var ch in m_diMultiChList) ch.Dispose();
            
            //digital out single.
            if (m_doSingChList != null) foreach (var ch in m_doSingChList) ch.Dispose();
            
            //digital out multi.
            if (m_doMultiChList != null) foreach (var ch in m_doMultiChList) ch.Dispose();
        }

        // Note disposing has been done.
        m_disposed = true;

    }
    

    /// <summary>
    /// Implement IDisposable. 
    /// Do not make this method virtual. 
    /// A derived class should not be able to override this method. 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion



}
