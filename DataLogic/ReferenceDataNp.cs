using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Jeffsoft;
using Neon.Aligner;



namespace Neon.Aligner
{
    public class ReferenceDataNp
    {
        private int m_startWave;                    //start wavelength.
        private int m_stopWave;                     //stop wavelength.
        private double m_stepWave;                  //wavelength step
        private List<PortPowers> m_prtPwrList;
        private string m_filepath;                  //ref. filepath.


        //-------- property ------------

        public int startWave { get { return m_startWave; } }
        public int stopWave { get { return m_stopWave; } }
        public double stepWave { get { return m_stepWave; } }
        public int dataPoint { get { return (int)((m_stopWave - m_startWave) / m_stepWave) + 1; } }
        public int portCnt
        {
            get
            {
                int ret = 0;
                try
                {
                    ret = m_prtPwrList.Count();
                }
                catch
                {
                    ret = 0;
                }
                return ret;
            }
        }

        public ReferenceDataNp()
        {
            mMonitorPower = new Dictionary<int, double>();
        }


        #region ==== Monitor Power ====

        private Dictionary<int, double> mMonitorPower;

        public void ClearMonitorPower(int port)
        {
            mMonitorPower[port] = 0;
        }

        public void SetMonitorPower(int port, double power)
        {
            mMonitorPower[port] = Math.Round(power, 3);
            //mMonitorPower.OrderBy(x => x.Key);
        }
        public double MonitorPower(int port)
        {
            if (!mMonitorPower.ContainsKey(port)) mMonitorPower.Add(port, 0);
            return mMonitorPower[port];
        }

        public double[] MonitorPower()
        {
            mMonitorPower.OrderBy(x => x.Key);
            return mMonitorPower.Values.ToArray();
        }

        #endregion



        #region ==== File IO ====

        public bool LoadReferenceFromTxt(string _filepath)
        {

            bool ret = false;

            string strLineBuf = "";
            string[] strTempArr = null;
            FileStream fs = null;
            StreamReader sr = null;

            try
            {

                //initailize member variables.
                m_startWave = 0;
                m_stopWave = 0;
                m_stepWave = 0.0;
                if (m_prtPwrList != null) m_prtPwrList.Clear();
                m_prtPwrList = null;
                m_filepath = null;


                //File Open
                fs = new FileStream(_filepath, FileMode.Open);
                sr = new StreamReader(fs);


                //[START WAVELENGTH]
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                m_startWave = Convert.ToInt32(strTempArr[1]);


                //[STOP WAVELENGTH]
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                m_stopWave = Convert.ToInt32(strTempArr[1]);


                //[STEP] 
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                m_stepWave = Convert.ToDouble(strTempArr[1]);
                m_stepWave = Math.Round(m_stepWave, 3);


                //[REFERENCE PORTS]
                int chnlCnt = 0;
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                chnlCnt = Convert.ToInt32(strTempArr[1]);


                //[DATA COUNT]
                int dataCnt = 0;
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                dataCnt = Convert.ToInt32(strTempArr[1]);


                //[MAX POLARIZATION FILTER ANGLE]
                double polAng = 0.0;
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                polAng = Convert.ToDouble(strTempArr[1]);
                polAng = Math.Round(polAng, 2);


                //[SCAN_MODE]
                strLineBuf = sr.ReadLine();


                //[PORT]
                int nPort = 0;
                List<int> portNoList = new List<int>();
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                strTempArr = strTempArr[1].Split('\t');
                for (int j = 0; j < strTempArr.Length; j++)
                {
                    try
                    {
                        nPort = Convert.ToInt32(strTempArr[j]);
                    }
                    catch
                    {
                        nPort = 0;
                    }

                    if (nPort != 0) portNoList.Add(nPort);
                }


                //[Monitor Port]
                strLineBuf = sr.ReadLine();
                strTempArr = strLineBuf.Split(']');
                strTempArr = strTempArr[1].Split('\t');
                for (int i = 0; i < portNoList.Count; i++)
                    SetMonitorPower(portNoList[i], Convert.ToDouble(strTempArr[i]));


                //[DATA] 
                m_prtPwrList = new List<PortPowers>();
                for (int i = 0; i < chnlCnt; i++)
                {
                    PortPowers swpPortPwr = new PortPowers(portNoList[i], 1);
                    m_prtPwrList.Add(swpPortPwr);
                }

                strLineBuf = sr.ReadLine();
                int nIndex = 0;
                double refPow = 0.0;
                while (true)
                {

                    strLineBuf = sr.ReadLine();
                    if (strLineBuf == "[END_OF_FILE]")
                        break;


                    strTempArr = strLineBuf.Split('\t');
                    for (int i = 0; i < chnlCnt; i++)
                    {
                        refPow = Convert.ToDouble(strTempArr[i + 1]);
                        m_prtPwrList[i].NonPol.Add(refPow);
                    }

                    nIndex++;

                }


                m_filepath = _filepath;
                ret = true;

            }
            catch
            {
                m_startWave = 0;
                m_stopWave = 0;
                m_stepWave = 0.0;
                if (m_prtPwrList != null) m_prtPwrList.Clear();
                m_prtPwrList = null;
                m_filepath = null;

                ret = false;
            }
            finally
            {
                //File close
                if (sr != null) sr.Close();
                if (fs != null) fs.Close();
            }

            return ret;
        }



        public bool SaveToTxt(string _filePath)
        {
            bool ret = false;

            string strLineBuf = "";
            FileStream fs = null;
            StreamWriter sw = null;

            try
            {

                int dataPoint = 0;  //ref. data 갯수.
                int portCnt = 0;    //port 갯수.

                //File Open
                string filepath = _filePath;
                if (filepath == "") filepath = m_filepath;
                if (filepath == "") throw new ApplicationException("");

                fs = new FileStream(filepath, FileMode.Create);
                sw = new StreamWriter(fs);


                //[START WAVELENGTH]
                strLineBuf = "[START WAVELENGTH]" + m_startWave.ToString();
                sw.WriteLine(strLineBuf);

                //[STOP WAVELENGTH]
                strLineBuf = "[STOP WAVELENGTH]" + m_stopWave.ToString();
                sw.WriteLine(strLineBuf);

                //[STEP] 
                strLineBuf = "[STEP WAVELENGTH]";
                strLineBuf += Math.Round(m_stepWave, 3).ToString();
                sw.WriteLine(strLineBuf);

                //[REFERENCE PORTS]
                portCnt = m_prtPwrList.Count;
                strLineBuf = "[REFERENCE PORTS]";
                strLineBuf += Convert.ToInt32(portCnt);
                sw.WriteLine(strLineBuf);


                //[DATA COUNT]
                dataPoint = m_prtPwrList[0].NonPol.Count;
                strLineBuf = "[DATA COUNT]";
                strLineBuf += Convert.ToInt32(dataPoint);
                sw.WriteLine(strLineBuf);

                //[MAX POLARIZATION FILTER ANGLE]
                strLineBuf = "[MAX POLARIZATION FILTER ANGLE]0";
                sw.WriteLine(strLineBuf);

                //[SCAN_MODE]
                strLineBuf = "[SCAN_MODE]NON-POLARIZATION";
                sw.WriteLine(strLineBuf);

                //[PORT]
                strLineBuf = "[PORT]";
                for (int i = 0; i < m_prtPwrList.Count; i++)
                    strLineBuf += Convert.ToString(m_prtPwrList[i].Port) + '\t';
                sw.WriteLine(strLineBuf);

                //[MONITOR Ref]
                strLineBuf = "[MONITOR]";
                double monitorRef = 0;
                for (int i = 0; i < m_prtPwrList.Count; i++)
                {
                    monitorRef = MonitorPower(m_prtPwrList[i].Port);
                    strLineBuf += Convert.ToString(monitorRef) + '\t';
                }
                sw.WriteLine(strLineBuf);


                //[DATA] 
                strLineBuf = "[DATA]";
                sw.WriteLine(strLineBuf);


                //reference
                strLineBuf = "";
                double wavelen = m_startWave;   //wavelength [nm]
                double optPwr = 0;              //optical powrer [mw]
                for (int i = 0; i < dataPoint; i++)
                {
                    //wavelength
                    strLineBuf = Convert.ToString(wavelen) + '\t';

                    //power
                    for (int j = 0; j < portCnt; j++)
                    {
                        optPwr = Math.Round(m_prtPwrList[j].NonPol[i], 9);
                        strLineBuf += Convert.ToString(optPwr) + '\t';
                    }
                    sw.WriteLine(strLineBuf);

                    wavelen += m_stepWave;
                    wavelen = Math.Round(wavelen, 3);
                }


                //[END_OF_FILE]
                strLineBuf = "[END_OF_FILE]";
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
                if (sw != null) sw.Close();
                if (fs != null) fs.Close();
            }

            return ret;
        }


        #endregion




        /// <summary>
        /// clear... 
        /// </summary>
        public void Clear()
        {
            m_startWave = 0;
            m_stopWave = 0;     //stop wavelength.
            m_stepWave = 0.0;   //wavelength step

            if (m_prtPwrList != null)
                m_prtPwrList.Clear();
            m_prtPwrList = null;

            m_filepath = "";
        }



        /// <summary>
        /// clear and set wavelength.
        /// </summary>
        /// <param name="_startWave">start wavelength</param>
        /// <param name="_stopWave">stop wavelength</param>
        /// <param name="_stepWave">wavelength step</param>
        public void Clear(double _startWave, double _stopWave, double _stepWave)
        {
            m_startWave = (int)_startWave;          //start wavelength.
            m_stopWave = (int)_stopWave;            //stop wavelength.
            m_stepWave = Math.Round(_stepWave, 3);  //wavelength step

            if (m_prtPwrList != null)
                m_prtPwrList.Clear();
            m_prtPwrList = null;

            m_filepath = "";
        }



        /// <summary>
        /// wavelength 설정.
        /// </summary>
        /// <param name="_wavelenList"></param>
        public void SetWavelength(List<double> _wavelenList)
        {
            if (_wavelenList == null)
                return;

            m_startWave = (int)(_wavelenList[0]);
            m_stopWave = (int)(_wavelenList.Last());
            m_stepWave = (_wavelenList[1] - _wavelenList[0]);
            m_stepWave = Math.Round(m_stepWave, 3);

        }



        /// <summary>
        /// wavelength 설정.
        /// </summary>
        /// <param name="_startWave">start wavelength</param>
        /// <param name="_stopWave">stop wavelength</param>
        /// <param name="_stepWave">wavelength step</param>
        public void SetWavelength(double _startWave, double _stopWave, double _stepWave)
        {
            m_startWave = (int)(_startWave);
            m_stopWave = (int)(_stopWave);
            m_stepWave = Math.Round(_stepWave, 3);
        }




        /// <summary>
        /// Set ref. data for port.
        /// </summary>
        /// <param name="_swpPortPwr"></param>
        public void SetPortData(PortPowers _swpPortPwr)
        {

            if (_swpPortPwr == null)
                return;


            //find index.
            int idx = -1;
            try
            {
                idx = m_prtPwrList.FindIndex(p => p.Port == _swpPortPwr.Port);
            }
            catch
            {
                m_prtPwrList = new List<PortPowers>();
            }


            //add
            if (idx >= 0)
                m_prtPwrList[idx] = _swpPortPwr;
            else
                m_prtPwrList.Add(_swpPortPwr);

        }




        /// <summary>
        /// get all of ref.data for a port.
        /// </summary>
        /// <param name="_port">port no.</param>
        /// <returns></returns>
        public PortPowers GetPortData(int _port)
        {

            PortPowers ret = null;

            try
            {
                ret = m_prtPwrList.Find(p => p.Port == _port);
            }
            catch
            {
                ret = null;
            }

            return ret;
        }



        /// <summary>
        /// remove port data from list.
        /// </summary>
        /// <param name="_port"></param>
        public void DeletePortData(int _port)
        {

            PortPowers pd = null;
            try
            {
                pd = m_prtPwrList.Find(p => p.Port == _port);
            }
            catch
            {
                pd = null;
            }


            if (pd != null)
                m_prtPwrList.Remove(pd);

        }



        /// <summary>
        /// get the ref. power 
        /// 보간법을 이용하여 wavlength에 대한 ref. power를 구한다.
        /// </summary>
        /// <param name="_port">port no.</param>
        /// <param name="_wavelen">wavlength</param>
        /// <returns></returns>
        public double RefPow(int _port, double _wavelen)
        {

            double ret = 0.0;

            try
            {
                PortPowers swPortPwr = m_prtPwrList.Find(p => p.Port == _port);
                if (swPortPwr == null)
                    throw new ApplicationException();


                int dataPoint = swPortPwr.NonPol.Count();
                int idx = 0;
                idx = Convert.ToInt32((_wavelen - m_startWave) / m_stepWave);
                if (idx >= (dataPoint - 1))
                    idx = dataPoint - 2;


                double[] xArr = new double[2];
                double[] yArr = new double[2];
                xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
                xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
                yArr[0] = swPortPwr.NonPol[idx];
                yArr[1] = swPortPwr.NonPol[idx + 1];
                ret = JeffMath.LinearInterpolation(xArr[0], yArr[0], xArr[1], yArr[1], _wavelen);
                ret = Math.Round(ret, 9);

            }
            catch
            {
                ret = 0.0;
            }

            return ret;

        }


        /// <summary>
        /// get the ref. power list
        /// </summary>
        /// <param name="_port">port no.</param>
        /// <returns></returns>
        public List<double> RefPow(int _port)
        {

            List<double> retList = null;

            try
            {
                PortPowers swPortPwr = m_prtPwrList.Find(p => p.Port == _port);
                if (swPortPwr == null)
                    throw new ApplicationException();

                retList = swPortPwr.NonPol;

            }
            catch
            {
                if (retList != null)
                    retList.Clear();
                retList.Clear();
            }

            return retList;
        }


    }//class 


}
    