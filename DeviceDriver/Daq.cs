﻿using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.DAQmx;

namespace Neon.Aligner
{
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
                // Do not re-create Dispose clean-up code here. 
                // Calling Dispose(false) is optimal in terms of 
                // readability and maintainability.
                try
                {
                    Dispose(false);
                }
                catch { }
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
            /// terminal configuration : Differential
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
                    //Create a task
                    m_task = new Task();

                    //Create channel & reader.
                    string strPhyChName = "Dev1/ai" + _chnlNo.ToString(); //채널의 물리적 이름
                    string strChName = "ch" + _chnlNo.ToString(); //채널 이름
                    m_task.AIChannels.CreateVoltageChannel(strPhyChName, strChName, AITerminalConfiguration.Differential, 
                        _min, _max, AIVoltageUnits.Volts);

                    //m_task.Timing.ConfigureSampleClock("", 1000, SampleClockActiveEdge.Rising, SampleQuantityMode.FiniteSamples, 100);

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
            /// 1.task 생성.
            /// 2.channel 생성. terminal configrulation : Referenced single-end
            /// 3.reader생성.
            /// 4.task 시작!!
            /// </summary>
            /// <param name="_chnlNo">channel no.</param>
            /// <param name="_min">minimum signal value.</param>
            /// <param name="_max">maximum signal value.</param>
            /// <returns></returns>
            public bool CreateRse(int _chnlNo, double _min, double _max)
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

                    m_task = new Task();

                    //Create channel & reader.
                    string strPhyChName = "Dev1/ai" + _chnlNo.ToString(); //채널의 물리적 이름
                    string strChName = "ch" + _chnlNo.ToString(); //채널 이름
                    m_task.AIChannels.CreateVoltageChannel(strPhyChName, strChName, AITerminalConfiguration.Rse,
                                                            _min, _max, AIVoltageUnits.Volts);
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
                    lock (_lock)
                    {
                        //ret = m_reader.ReadSingleSample();
                        ret = m_reader.ReadMultiSample(20).Average();
                    }
                }
                catch
                {
                    ret = 0;
                }

                return ret;
            }
            object _lock = new object();


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
            //private int m_lineCnt;
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
        /// single line DO(digital Out) channel 
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



			public bool Create(int dev, int port, int line)
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
					string strPhyChName = $"Dev{dev}/port{port}/line{line}";    //채널의 물리적 이름
					string strChName = $"port{port}_{line}";					//채널 이름
					m_task.DOChannels.CreateChannel(strPhyChName,strChName,
												    ChannelLineGrouping.OneChannelForEachLine);

					m_writer = new DigitalSingleChannelWriter(m_task.Stream);
					m_line = line;
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



        #region private/protected method



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

            if (!m_disposed)
            {

                if (disposing)
                {

                    //analog input single
                    if (m_aiSingList != null)
                    {
                        try
                        {
                            for (int i = 0; i < m_aiSingList.Count(); i++)
                            {
                                m_aiSingList[i].Dispose();
                            }
                        }
                        catch
                        {
                            //do nothing.
                        }
                    }


                    //digital input single
                    if (m_diSingChList != null)
                    {
                        try
                        {
                            for (int i = 0; i < m_diSingChList.Count(); i++)
                            {
                                m_diSingChList[i].Dispose();
                            }
                        }
                        catch
                        {
                            //do nothing.
                        }
                    }


                    //digital input multi
                    if (m_diMultiChList != null)
                    {
                        try
                        {
                            for (int i = 0; i < m_diMultiChList.Count(); i++)
                            {
                                m_diMultiChList[i].Dispose();
                            }
                        }
                        catch
                        {
                            //do nothing.
                        }
                    }


                    //digital out single.
                    if (m_doSingChList != null)
                    {
                        try
                        {
                            for (int i = 0; i < m_doSingChList.Count(); i++)
                            {
                                m_doSingChList[i].Dispose();
                            }
                        }
                        catch
                        {
                            //do nothing.
                        }
                    }


                    //digital out multi.
                    if (m_doMultiChList != null)
                    {
                        try
                        {
                            for (int i = 0; i < m_doMultiChList.Count(); i++)
                            {
                                m_doMultiChList[i].Dispose();
                            }
                        }
                        catch
                        {
                            //do nothing.
                        }
                    }


                }


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
        /// digital INPUT 생성
        /// </summary>
        /// <param name="_aiNo">analog input no.</param>
        public bool CreateAiCh(int _aiNo, double _min, double _max)
        {
            bool ret = false;
            try
            {
                //create digital input channel instance.
                CaiChannel aiCh = new CaiChannel();
                if (aiCh.Create(_aiNo, _min, _max) == false) throw new ApplicationException();

                //add it to list.
                if (m_aiSingList == null) m_aiSingList = new List<CaiChannel>();
                m_aiSingList.Add(aiCh);

                ret = true;
            }
            catch(Exception ex)
            {
                ex.Message.ToString();
                ret = false;
            }


            return ret;

        }



        /// <summary>
        /// digital INPUT 생성 - RSE
        /// </summary>
        /// <param name="_aiNo">analog input no.</param>
        public bool CreateAiChRse(int _aiNo, double _min, double _max)
        {
            bool ret = false;
            try
            {
                //create digital input channel instance.
                CaiChannel aiCh = new CaiChannel();
                if (aiCh.CreateRse(_aiNo, _min, _max) == false) throw new ApplicationException();

                //add it to list.
                if (m_aiSingList == null) m_aiSingList = new List<CaiChannel>();
                m_aiSingList.Add(aiCh);

                ret = true;
            }
            catch
            {
                ret = false;
            }


            return ret;

        }



        /// <summary>
        /// digital INPUT 생성 [single line]
        /// </summary>
        /// <param name="_lineNo">line no.</param>
        /// <returns></returns>
        public bool CreateDiCh(int _lineNo)
        {

            bool ret = false;

            try
            {
                //create digital input channel instance.
                CdiSingLineChannel diSingChnl = new CdiSingLineChannel();
                if (diSingChnl.Create(_lineNo) == false) throw new ApplicationException();

                //add it to list.
                if (m_diSingChList == null)
                    m_diSingChList = new List<CdiSingLineChannel>();
                m_diSingChList.Add(diSingChnl);

                ret = true;
            }
            catch
            {
                ret = false;
            }


            return ret;
        }
		


        /// <summary>
        /// Read a data from Digitial IO single line.
        /// </summary>
        /// <param name="_lineNo">line no.</param>
        /// <returns> true:1 , false:0 </returns>
        public bool ReadDi(int _lineNo)
        {
            bool ret = false;

            try
            {

                CdiSingLineChannel di = null;
                di = m_diSingChList.Find(p => p.line == _lineNo);
                if (di == null) throw new ApplicationException();

                ret = di.Read();

            }
            catch
            {
                ret = false;
            }


            return ret;
        }
		


        /// <summary>
        /// digital INPUT 생성 [multi lines]
        /// </summary>
        /// <param name="_lines">line no. array.</param>
        /// <returns></returns>
        public bool CreateDiCh(int[] _lines)
        {

            bool ret = false;

            try
            {
                //create digital input channel instance.
                CdiMultiLineChannel diMultiChnl = new CdiMultiLineChannel();
                if (diMultiChnl.Create(_lines) == false)throw new ApplicationException();

                //add it to list.
                if (m_diMultiChList == null) m_diMultiChList = new List<CdiMultiLineChannel>();
                m_diMultiChList.Add(diMultiChnl);

                ret = true;
            }
            catch
            {
                ret = false;
            }


            return ret;
        }



        /// <summary>
        /// Read		[multi line]
        /// </summary>
        /// <param name="_strLines">line no. string  ex)0:7 </param>
        /// <returns> integer type value. </returns>
        public int ReadDi(string _strLines)
        {
            int ret = 0;

            try
            {

                CdiMultiLineChannel di = null;
                di = m_diMultiChList.Find(p => p.lineStr == _strLines);
                if (di == null) throw new ApplicationException();

                ret = di.Read();

            }
            catch
            {
                ret = 0;
            }


            return ret;
        }



        /// <summary>
        /// Read		[Digitial IO]
        /// </summary>
        /// <param name="_lines">line no. array</param>
        /// <returns> integer type value. </returns>
        public int ReadDi(int[] _lines)
        {
            int ret = 0;

            try
            {

                //완벽하지 않음. 
                // Array의 값을 비교하여서 판단해야 하는데 시간관계상
                //나중에 하기로 한다.

                //CdiMultiLineChannel di = null;
                //di = m_diMultiChList.Find(p => p.lineStr == _strLines);
                //if (di == null)
                //    throw new ApplicationException();

                //ret = di.Read();

            }
            catch
            {
                ret = 0;
            }


            return ret;
        }



        /// <summary>
        /// digital OUTPUT 생성 [single lines]
        /// </summary>
        /// <param name="line">line no.</param>
        public bool CreateDoCh(int _line)
        {

            bool ret = false;

            try
            {

                //create digital input channel instance.
                CdoSingLineChannel doSingChnl = new CdoSingLineChannel();
                if (doSingChnl.Create(_line) == false) throw new ApplicationException();

                //false 출력!!! <- 처음 채널 생성하면 0V
                doSingChnl.Write(false);

                //add it to list.
                if (m_doSingChList == null)
                    m_doSingChList = new List<CdoSingLineChannel>();
                m_doSingChList.Add(doSingChnl);

                ret = true;
            }
            catch
            {
                ret = false;
            }


            return ret;
        }


		public bool CreateDoCh(int dev, int port, int line)
		{

			bool ret = false;

			try
			{

				//create digital input channel instance.
				CdoSingLineChannel doSingChnl = new CdoSingLineChannel();
				if (doSingChnl.Create(dev, port, line) == false) throw new ApplicationException();

				//false 출력!!! <- 처음 채널 생성하면 0V
				doSingChnl.Write(false);

				//add it to list.
				if (m_doSingChList == null)
					m_doSingChList = new List<CdoSingLineChannel>();
				m_doSingChList.Add(doSingChnl);

				ret = true;
			}
			catch
			{
				ret = false;
			}


			return ret;
		}



		/// <summary>
		/// Write		[Digitia OUTPUT, single line]
		/// </summary>
		/// <param name="_strLines">line no. string  ex)0:7 </param>
		/// <returns> integer type value. </returns>
		public int WriteDo(int _line, bool _data)
        {
            int ret = 0;

            try
            {

                CdoSingLineChannel doCh = null;
                doCh = m_doSingChList.Find(p => p.line == _line);
                if (doCh == null) throw new ApplicationException();

                doCh.Write(_data);

            }
            catch
            {
                ret = 0;
            }


            return ret;
        }



        /// <summary>
        /// digital input 생성 [multi lines]
        /// </summary>
        /// <param name="_lineStr">line no. string</param>
        /// <returns></returns>
        public bool CreateDoCh(string _lineStr)
        {

            bool ret = false;

            try
            {

                //create digital input channel instance.
                CdoMultiLineChannel doMultiChnl = new CdoMultiLineChannel();
                if (doMultiChnl.Create(_lineStr) == false) throw new ApplicationException();

                //false 출력!!
                //doMultiChnl.Write(0);
                WriteDo(_lineStr, 0);

                //add it to list.
                if (m_doMultiChList == null)
                    m_doMultiChList = new List<CdoMultiLineChannel>();
                m_doMultiChList.Add(doMultiChnl);


                ret = true;
            }
            catch
            {
                ret = false;
            }


            return ret;
        }



        /// <summary>
        /// Write		[Digitial IO single line]
        /// </summary>
        /// <param name="_strLines">line no. string  ex)0:7 </param>
        /// <returns> integer type value. </returns>
        public int WriteDo(string _strLines, UInt32 _data)
        {
            int ret = 0;

            try
            {

                CdoMultiLineChannel doCh = null;
                doCh = m_doMultiChList.Find(p => p.lineStr == _strLines);
                if (doCh == null) throw new ApplicationException();

                doCh.Write(_data);

            }
            catch
            {
                ret = 0;
            }


            return ret;
        }



        /// <summary>
        /// Read		[analog input]
        /// </summary>
        /// <param name="_aiNo">analog input no.</param>
        /// <returns></returns>
        public double ReadAi(int _aiNo)
        {
            try
            {

                CaiChannel ai = null;
                ai = m_aiSingList.Find(p => p.chnlNo == _aiNo);
                if (ai == null) throw new ApplicationException($"_aiNo={_aiNo} not exist");

                return ai.Read();
            }
            catch(Exception ex)
            {
                try { Log.Write($"Daq.ReadAi():\n{ex.Message}"); }
                catch { }
                return  double.PositiveInfinity;
            }
        }
		



        #endregion

    }
}