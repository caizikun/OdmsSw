using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Jeffsoft;
using Neon.Aligner;


public class SweepLogic 
{



	#region structure/innor class

	
	/// <summary>
	/// raw data. from device.
	/// </summary>
	public class CswpPortPwr
	{
		public int port;
		public List<double> powList;
	}




	/// <summary>
	/// calibrated data. (equal interval, stiching)
	/// non-polarization
	/// </summary>
	public class CswpPortPwrNonpol
	{
		public int port;
		public List<double> powList;
	}




	/// <summary>
	/// insertion loss
	/// non-polarization
	/// </summary>
	public class CswpPortIlNonpol
	{
		public int port;
		public List<double> ilList; 
	}



	
	/// <summary>
	/// 
	/// </summary>
	public class CswpNonpol
	{
		public int startWavelen;
		public int stopWavelen;
		public double stepWavelen;
		public List<CswpPortIlNonpol> portDataList;

		public int dataPoint
		{
			get
			{
				int ret = 0;
				try
				{
					ret = (int)((stopWavelen - startWavelen) / stepWavelen) + 1;
				}
				catch
				{
					ret = 0;
				}
				return ret;
			}
		}


		public int chnlCnt  //channel 갯수.
		{
			get
			{
				int ret = 0;
				try
				{
					ret = portDataList.Count();
				}
				catch
				{
					ret = 0;
				}
				return ret;
			}

		}


		/// <summary>
		/// text file에 저장한다.(full range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <returns></returns>
		public bool SaveToTxt(string _filePath)
		{

			bool ret = false;

			StringBuilder sbLine = new StringBuilder();
			StreamWriter sw = null;
			int numCh = chnlCnt;
			int numDataPoint = dataPoint;

			try
			{
				//File Open
				sw = new StreamWriter(_filePath, false, Encoding.ASCII);

				List<double>[] data = new List<double>[numCh];
				for(int i = 0; i < numCh; i++)
				{
					data[i] = portDataList.Find(p => p.port == (i + 1)).ilList;
				}


				//data
				double wavelen = startWavelen;   //wavelength [nm]
				for(int i = 0; i < dataPoint; i++)
				{
					sbLine.Clear();
					//wavelength
					sbLine.Append($"{wavelen}, ");

					//insertion..
					for(int j = 0; j < numCh; j++)
					{
						sbLine.Append($"{data[j][i]}, ");
					}
					sbLine.Remove(sbLine.Length - 2, 2);
					sw.WriteLine(sbLine.ToString());

					wavelen += stepWavelen;
					wavelen = Math.Round(wavelen, 3);
				}


				////[END_OF_FILE]
				//strLineBuf = "[END_OF_FILE]";
				//sw.WriteLine(strLineBuf);


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
			}

			return ret;
		}




		/// <summary>
		/// text file에 저장한다.(custom range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <param name="_rngStartWave">range start</param>
		/// <param name="_rngStopWave">range stop</param>
		/// <returns></returns>
		public bool SaveToTxt(string _filePath, int _startWave, int _stopWave)
		{

			bool ret = false;

			string strLineBuf = "";
			FileStream fs = null;
			StreamWriter sw = null;

			try
			{


				//File Open
				string filepath = _filePath;
				if (filepath == "")
					throw new ApplicationException("");

				fs = new FileStream(filepath, FileMode.Create);
				sw = new StreamWriter(fs);


				//data
				strLineBuf = "";
				int startIdx =(int)((_stopWave - startWavelen) / stepWavelen);
				double wavelen = _startWave;      //wavelength [nm]
				double il = 0.0;                //insertion loss [dB]
				for (int i = startIdx; i < dataPoint; i++)
				{

					if (wavelen > _stopWave)
						break;

					//wavelength
					strLineBuf = Convert.ToString(wavelen) + ',';

					//insertion..
					for (int j = 0; j < chnlCnt; j++)
					{
						il = Math.Round(portDataList[j].ilList[i], 3);
						strLineBuf += Convert.ToString(il) + ',';
					}
					sw.WriteLine(strLineBuf);

					wavelen += stepWavelen;
					wavelen = Math.Round(wavelen, 3);
				}


				////[END_OF_FILE]
				//strLineBuf = "[END_OF_FILE]";
				//sw.WriteLine(strLineBuf);


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

	}




	/// <summary>
	/// calibrated data. (equal interval, stiching)
	/// </summary>
	public class CswpPortPwrPol
	{
		public int port;
		public List<double> powList1;   //polarization state1
		public List<double> powList2;   //polarization state2
		public List<double> powList3;   //polarization state3
		public List<double> powList4;   //polarization state4
	}




	/// <summary>
	/// insertion loss
	/// </summary>
	public class CswpPortIlPol
	{
		public int port;
		public List<double> ilMin;  //high transmission 
		public List<double> ilMax;  //low transmission 
	}




	public class CswpPol
	{
		public int startWavelen;
		public int stopWavelen;
		public double stepWavelen;
		public List<CswpPortIlPol> portDataList;

		public int dataPoint
		{
			get
			{
				int ret = 0;
				try
				{
					ret = (int)((stopWavelen - startWavelen) / stepWavelen) + 1;
				}
				catch
				{
					ret = 0;
				}
				return ret;
			}
		}


		public int chnlCnt  //channel 갯수.
		{
			get
			{
				int ret = 0;
				try
				{
					ret = portDataList.Count();
				}
				catch
				{
					ret = 0;
				}
				return ret;
			}

		}



		/// <summary>
		/// text file에 저장한다.(full range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <returns></returns>
		public bool SaveToTxt(string _filePath)
		{

			bool ret = false;

			string strLineBuf = "";
			FileStream fs = null;
			StreamWriter sw = null;

			try
			{


				//File Open
				string filepath = _filePath;
				if (filepath == "")
					throw new ApplicationException("");

				fs = new FileStream(filepath, FileMode.Create);
				sw = new StreamWriter(fs);


				//data
				strLineBuf = "";
				double wavelen = startWavelen;   //wavelength [nm]
				for (int i = 0; i < dataPoint; i++)
				{
					//wavelength
					strLineBuf = Convert.ToString(wavelen) + ',';

					//insertion..
					double ilMin = 0.0;//max trnas
					double ilMax = 0.0;//min trnas.
					double avg = 0.0;
					double pdl = 0.0;
					for (int j = 0; j < chnlCnt; j++)
					{

						ilMin = Math.Round(portDataList[j].ilMin[i], 3);
						ilMax = Math.Round(portDataList[j].ilMax[i], 3);
						avg = Math.Round((ilMin + ilMax) / 2, 3);
						pdl = Math.Round((ilMin - ilMax), 3);

						strLineBuf += Convert.ToString(ilMin) + ',';
						strLineBuf += Convert.ToString(ilMax) + ',';
						strLineBuf += Convert.ToString(avg) + ',';
						strLineBuf += Convert.ToString(pdl) + ',';
					}
					sw.WriteLine(strLineBuf);

					wavelen += stepWavelen;
					wavelen = Math.Round(wavelen, 3);
				}


				////[END_OF_FILE]
				//strLineBuf = "[END_OF_FILE]";
				//sw.WriteLine(strLineBuf);


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
		/// text file에 저장한다.(custom range)
		/// </summary>
		/// <param name="_filepath">file path</param>
		/// <param name="_rngStartWave">range start</param>
		/// <param name="_rngStopWave">range stop</param>
		/// <returns></returns>
		public bool SaveToTxt(string _filePath, int _startWave, int _stopWave)
		{

			bool ret = false;

			string strLineBuf = "";
			FileStream fs = null;
			StreamWriter sw = null;

			try
			{


				//File Open
				string filepath = _filePath;
				if (filepath == "")
					throw new ApplicationException("");

				fs = new FileStream(filepath, FileMode.Create);
				sw = new StreamWriter(fs);


				//data
				strLineBuf = "";
				int startIdx = (int)((_stopWave - startWavelen) / stepWavelen);
				double wavelen = _startWave;      //wavelength [nm]
				for (int i = startIdx; i < dataPoint; i++)
				{

					if (wavelen > _stopWave)
						break;

					//wavelength
					strLineBuf = Convert.ToString(wavelen) + ',';

					//insertion..
					double ilMin = 0.0;//max trnas
					double ilMax = 0.0;//min trnas.
					double avg = 0.0;
					double pdl = 0.0;
					for (int j = 0; j < chnlCnt; j++)
					{
  
						ilMin = Math.Round(portDataList[j].ilMin[0], 3);
						ilMax = Math.Round(portDataList[j].ilMax[0], 3);
						avg = Math.Round((ilMin + ilMax) / 2, 3);
						pdl = Math.Round((ilMin - ilMax), 3);

						strLineBuf += Convert.ToString(ilMin) + ',';
						strLineBuf += Convert.ToString(ilMax) + ',';
						strLineBuf += Convert.ToString(avg) + ',';
						strLineBuf += Convert.ToString(pdl) + ',';
					}
					sw.WriteLine(strLineBuf);

					wavelen += stepWavelen;
					wavelen = Math.Round(wavelen, 3);
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


	}




	/// <summary>
	/// nonpol reference.
	/// </summary>
	public class CswpRefNonpol
	{

		private int m_startWave;  //start wavelength.
		private int m_stopWave; //stop wavelength.
		private double m_stepWave;  //wavelength step
		private List<CswpPortPwrNonpol> m_prtPwrList;
		private string m_filepath;   //ref. filepath.
		

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



		//-------- method ------------


		/// <summary>
		/// clear... 
		/// </summary>
		public void Clear()
		{
			m_startWave = 0;  
			m_stopWave = 0;     //stop wavelength.
			m_stepWave = 0.0;  //wavelength step
			
			if ( m_prtPwrList != null )
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
		/// text 파일 형태로 저장된 Reference 파일을 불어들인다.
		/// </summary>
		/// <param name="_filepath"></param>
		/// <returns></returns>
		public bool LoadFromTxt(string _filepath)
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
				if (m_prtPwrList != null)
					m_prtPwrList.Clear();
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
					
					if (nPort != 0)
						portNoList.Add(nPort);
				}
				//여기서 구한 port 갯수하고 chnlCnt하고 다른지 확인할것!


				//[DATA] 
				m_prtPwrList = new List<CswpPortPwrNonpol>();
				for (int i = 0; i < chnlCnt; i++)
				{
					CswpPortPwrNonpol swpPortPwr = new CswpPortPwrNonpol();
					swpPortPwr.port = portNoList[i];
					swpPortPwr.powList = new List<double>();
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
						m_prtPwrList[i].powList.Add(refPow); 
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
				if (m_prtPwrList != null)
					m_prtPwrList.Clear();
				m_prtPwrList = null;
				m_filepath = null;

				ret = false;
			}
			finally
			{

				//File close
				if (sr != null)
					sr.Close();

				if (fs != null)
					fs.Close();
			}

			return ret;
		}




		/// <summary>
		/// save to text file.
		/// </summary>
		/// <param name="_filePath"></param>
		/// <returns></returns>
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
				if (filepath == "")
					filepath = m_filepath;

				if (filepath == "")
					throw new ApplicationException("");

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
				dataPoint = m_prtPwrList[0].powList.Count;
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
				for (int i = 0; i < m_prtPwrList.Count ; i++)
				{
					strLineBuf += Convert.ToString(m_prtPwrList[i].port) + '\t';
				}
				sw.WriteLine(strLineBuf);


				//[DATA] 
				strLineBuf = "[DATA]";
				sw.WriteLine(strLineBuf);


				//reference
				strLineBuf = "";
				double wavelen = m_startWave;   //wavelength [nm]
				double optPwr = 0;  //optical powrer [mw]
				for (int i = 0; i < dataPoint; i++)
				{
					//wavelength
					strLineBuf = Convert.ToString(wavelen) + '\t';


					//power
					for (int j = 0; j < portCnt ; j++)
					{
						optPwr = Math.Round(m_prtPwrList[j].powList[i], 9) ;
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
				if (sw != null)
					sw.Close();

				if (fs != null)
					fs.Close();
			}

			return ret;
		}




		/// <summary>
		/// Set ref. data for port.
		/// </summary>
		/// <param name="_swpPortPwr"></param>
		public void SetPortData(CswpPortPwrNonpol _swpPortPwr)
		{

			if (_swpPortPwr == null)
				return;

			
			//find index.
			int idx = -1;
			try
			{
				idx = m_prtPwrList.FindIndex(p => p.port == _swpPortPwr.port);
			}
			catch
			{
				m_prtPwrList = new List<CswpPortPwrNonpol>();
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
		public CswpPortPwrNonpol GetPortData(int _port)
		{

			CswpPortPwrNonpol ret = null;

			try
			{
				ret = m_prtPwrList.Find(p => p.port == _port);
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

			CswpPortPwrNonpol pd = null;
			try
			{
				pd = m_prtPwrList.Find(p => p.port == _port);
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
				CswpPortPwrNonpol swPortPwr = m_prtPwrList.Find(p => p.port == _port);
				if (swPortPwr == null)
					throw new ApplicationException();


				int dataPoint = swPortPwr.powList.Count();
				int idx = 0;
				idx = Convert.ToInt32((_wavelen - m_startWave) / m_stepWave);
				if ( idx >= (dataPoint - 1) )
					idx = dataPoint - 2;
				

				double[] xArr = new double[2];
				double[] yArr = new double[2];
				xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
				xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
				yArr[0] = swPortPwr.powList[idx]; 
				yArr[1] = swPortPwr.powList[idx+1]; 
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
				CswpPortPwrNonpol swPortPwr = m_prtPwrList.Find(p => p.port == _port);
				if (swPortPwr == null)
					throw new ApplicationException();

				retList = swPortPwr.powList;

			}
			catch
			{
				if (retList != null)
					retList.Clear();
				retList.Clear();
			}

			return retList;
		}




	}



	
	/// <summary>
	/// nonpol reference.
	/// </summary>
	public class CswpRefPol
	{
		private int m_startWave;                     //start wavelength.
		private int m_stopWave;                      //stop wavelength.
		private double m_stepWave;                   //wavelength step
		private List<CswpPortPwrPol> m_prtPwrList;   //power data.
		private string m_filepath;                   //ref. filepath.
		private double m_polAngle;                   //polarization filter pos. at max. power.


		//-------- property ------------

		public int startWave { get { return m_startWave; } }
		public int stopWave { get { return m_stopWave; } }
		public double stepWave { get { return m_stepWave; } }
		public int dataPoint { get { return (int)((m_stopWave - m_startWave) / m_stepWave) + 1; } }
		public double polFilterPos { set { m_polAngle = value; }  get { return m_polAngle; } }
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



		//-------- method ------------


		/// <summary>
		/// clear... 
		/// </summary>
		public void Clear()
		{
			m_startWave = 0;
			m_stopWave = 0;     //stop wavelength.
			m_stepWave = 0.0;  //wavelength step

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
		/// text 파일 형태로 저장된 Reference 파일을 불어들인다.
		/// </summary>
		/// <param name="_filepath"></param>
		/// <returns></returns>
		public bool LoadFromTxt(string _filepath)
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
				if (m_prtPwrList != null)
					m_prtPwrList.Clear();
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
				strLineBuf = sr.ReadLine();
				strTempArr = strLineBuf.Split(']');
				m_polAngle = Convert.ToDouble(strTempArr[1]);
				m_polAngle = Math.Round(m_polAngle, 2);


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

					if (nPort != 0)
						portNoList.Add(nPort);
				}



				//[DATA] 
				m_prtPwrList = new List<CswpPortPwrPol>();
				for (int i = 0; i < chnlCnt; i++)
				{
					CswpPortPwrPol swpPortPwr = new CswpPortPwrPol();
					swpPortPwr.port = portNoList[i];
					swpPortPwr.powList1 = new List<double>();
					swpPortPwr.powList2 = new List<double>();
					swpPortPwr.powList3 = new List<double>();
					swpPortPwr.powList4 = new List<double>();
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
						//polarization state 1
						refPow = Convert.ToDouble(strTempArr[ (4*i) + 1]);
						m_prtPwrList[i].powList1.Add(refPow);

						//polarization state 2
						refPow = Convert.ToDouble(strTempArr[(4 * i) + 2]);
						m_prtPwrList[i].powList2.Add(refPow);

						//polarization state 3
						refPow = Convert.ToDouble(strTempArr[(4 * i) + 3]);
						m_prtPwrList[i].powList3.Add(refPow);

						//polarization state 4
						refPow = Convert.ToDouble(strTempArr[(4 * i) + 4]);
						m_prtPwrList[i].powList4.Add(refPow);

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
				if (m_prtPwrList != null)
					m_prtPwrList.Clear();
				m_prtPwrList = null;
				m_filepath = null;

				ret = false;
			}
			finally
			{

				//File close
				if (sr != null)
					sr.Close();

				if (fs != null)
					fs.Close();
			}

			return ret;
		}




		/// <summary>
		/// save to text file.
		/// </summary>
		/// <param name="_filePath"></param>
		/// <returns></returns>
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
				if (filepath == "")
					filepath = m_filepath;

				if (filepath == "")
					throw new ApplicationException("");

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
				strLineBuf += Convert.ToString(portCnt);
				sw.WriteLine(strLineBuf);


				//[DATA COUNT]
				dataPoint = m_prtPwrList[0].powList1.Count;
				strLineBuf = "[DATA COUNT]";
				strLineBuf += Convert.ToInt32(dataPoint);
				sw.WriteLine(strLineBuf);

				//[MAX POLARIZATION FILTER ANGLE]
				strLineBuf = "[MAX POLARIZATION FILTER ANGLE]";
				strLineBuf += Convert.ToString(polFilterPos);
				sw.WriteLine(strLineBuf);

				//[SCAN_MODE]
				strLineBuf = "[SCAN_MODE]POLARIZATION";
				sw.WriteLine(strLineBuf);

				//[PORT]
				strLineBuf = "[PORT]";
				for (int i = 0; i < m_prtPwrList.Count; i++)
				{
					strLineBuf += Convert.ToString(m_prtPwrList[i].port) + '\t';
				}
				sw.WriteLine(strLineBuf);


				//[DATA] 
				strLineBuf = "[DATA]";
				sw.WriteLine(strLineBuf);


				//reference
				strLineBuf = "";
				double wavelen = m_startWave;   //wavelength [nm]
				double optPwr = 0;  //optical powrer [mw]
				for (int i = 0; i < dataPoint; i++)
				{
					//wavelength
					strLineBuf = Convert.ToString(wavelen) + '\t';


					//power
					for (int j = 0; j < portCnt; j++)
					{
						//polarization state1
						optPwr = Math.Round(m_prtPwrList[j].powList1[i], 7);
						strLineBuf += Convert.ToString(optPwr) + '\t';

						//polarization state2
						optPwr = Math.Round(m_prtPwrList[j].powList2[i], 7);
						strLineBuf += Convert.ToString(optPwr) + '\t';

						//polarization state3
						optPwr = Math.Round(m_prtPwrList[j].powList3[i], 7);
						strLineBuf += Convert.ToString(optPwr) + '\t';

						//polarization state4
						optPwr = Math.Round(m_prtPwrList[j].powList4[i], 7);
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
				if (sw != null)
					sw.Close();

				if (fs != null)
					fs.Close();
			}

			return ret;
		}




		/// <summary>
		/// Set ref. data for port.
		/// </summary>
		/// <param name="_swpPortPwr"></param>
		public void SetPortData(CswpPortPwrPol _swpPortPwr)
		{

			if (_swpPortPwr == null)
				return;


			//find index.
			int idx = -1;
			try
			{
				idx = m_prtPwrList.FindIndex(p => p.port == _swpPortPwr.port);
			}
			catch
			{
				m_prtPwrList = new List<CswpPortPwrPol>();
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
		public CswpPortPwrPol GetPortData(int _port)
		{

			CswpPortPwrPol ret = null;

			try
			{
				ret = m_prtPwrList.Find(p => p.port == _port);
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

			CswpPortPwrPol pd = null;
			try
			{
				pd = m_prtPwrList.Find(p => p.port == _port);
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
		public double[] RefPow(int _port, double _wavelen)
		{

			double[] rets = null;

			try
			{
				rets = new double[4];


				CswpPortPwrPol swPortPwr = m_prtPwrList.Find(p => p.port == _port);
				if (swPortPwr == null)
					throw new ApplicationException();


				//find index.
				int dataPoint = swPortPwr.powList1.Count();
				int idx = 0;
				idx = Convert.ToInt32((_wavelen - m_startWave) / m_stepWave);
				if (idx >= (dataPoint - 1))
					idx = dataPoint - 2;


				//polarization state1
				double[] xArr = new double[2];
				double[] yArr = new double[2];
				xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
				xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
				yArr[0] = swPortPwr.powList1[idx];
				yArr[1] = swPortPwr.powList1[idx + 1];
				rets[0] = JeffMath.LinearInterpolation(xArr[0], yArr[0], xArr[1], yArr[1], _wavelen);
				rets[0] = Math.Round(rets[0], 7);

				//polarization state2
				xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
				xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
				yArr[0] = swPortPwr.powList2[idx];
				yArr[1] = swPortPwr.powList2[idx + 1];
				rets[1] = JeffMath.LinearInterpolation(xArr[0], yArr[0], xArr[1], yArr[1], _wavelen);
				rets[1] = Math.Round(rets[1], 7);

				//polarization state3
				xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
				xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
				yArr[0] = swPortPwr.powList3[idx];
				yArr[1] = swPortPwr.powList3[idx + 1];
				rets[2] = JeffMath.LinearInterpolation(xArr[0], yArr[0], xArr[1], yArr[1], _wavelen);
				rets[2] = Math.Round(rets[2], 7);

				//polarization state4
				xArr[0] = Math.Round(m_startWave + m_stepWave * idx, 3);
				xArr[1] = Math.Round(xArr[0] + m_stepWave, 3);
				yArr[0] = swPortPwr.powList4[idx];
				yArr[1] = swPortPwr.powList4[idx + 1];
				rets[3] = JeffMath.LinearInterpolation(xArr[0], yArr[0], xArr[1], yArr[1], _wavelen);
				rets[3] = Math.Round(rets[3], 7);

			}
			catch
			{
				rets = null;
			}

			return rets;

		}


		/// <summary>
		/// get the ref. power list
		/// </summary>
		/// <param name="_port">port no.</param>
		/// <returns></returns>
		public List<double>[] RefPow(int _port)
		{

			List<double>[] retLists = null;

			try
			{

				retLists = new List<double>[4];

				//find.
				CswpPortPwrPol swPortPwr = m_prtPwrList.Find(p => p.port == _port);
				if (swPortPwr == null)
					throw new ApplicationException();


				retLists[0] = swPortPwr.powList1;
				retLists[1] = swPortPwr.powList2;
				retLists[2] = swPortPwr.powList3;
				retLists[3] = swPortPwr.powList4;


			}
			catch
			{
				retLists = null;
			}

			return retLists;
		}

	}
	
		

	#endregion





	#region private variables


	private Itls m_tls;
	private IoptMultimeter m_mpm;
	private IpolController m_pc;

	private double m_wlOffset;                              //wavelength offset.

	private List<double> m_swpWavlenList;                   //calibrated(equal interval)
	private List<CswpPortPwrNonpol> m_swpPortPwrListNonpol; //calibrated(equal interval , stiching)
	private List<CswpPortPwrPol> m_swpPortPwrListPol;       //calibrated(equal interval , stiching)


	#endregion





	#region constructor/desconstructor


	/// <summary>
	/// constructor.
	/// </summary>
	/// <param name="_tls"></param>
	/// <param name="_mpm"></param>
	public SweepLogic(Itls _tls, IoptMultimeter _mpm)
	{
		m_tls = _tls;
		m_mpm = _mpm;
	}




	/// <summary>
	/// constructor.
	/// </summary>
	/// <param name="_tls"></param>
	/// <param name="_mpm"></param>
	public SweepLogic(Itls _tls, IoptMultimeter _mpm, IpolController _pc)
	{
		m_tls = _tls;
		m_mpm = _mpm;
		m_pc = _pc;
	}


	#endregion





	#region private method



	/// <summary>
	/// get power logging data of ports 
	/// </summary>
	/// <param name="_port">port no.</param>
	/// <returns></returns>
	private CswpPortPwr GetPowLogData(int _port)
	{
		CswpPortPwr portPwr = null;

		try
		{
			portPwr = new CswpPortPwr();
			portPwr.port = _port;
			portPwr.powList = m_mpm.GetPwrLog(_port);
		}
		catch
		{
			portPwr = null;
		}

		return portPwr;
		
	}




	/// <summary>
	/// get power logging data of ports 
	/// </summary>
	/// <param name="_portNos"></param>
	/// <returns></returns>
	private List<CswpPortPwr> GetPowLogData(int[] _portNos)
	{

		List<CswpPortPwr> swpPortPwrList = null;

		try
		{
			swpPortPwrList = new List<CswpPortPwr>();
			for( int k = 0; k < _portNos.Length ; k++)
			{
				CswpPortPwr portPwr = new CswpPortPwr();
				portPwr.port = _portNos[k];
				portPwr.powList = m_mpm.GetPwrLog(_portNos[k]);
				swpPortPwrList.Add(portPwr);
			}

		}
		catch
		{
			if( swpPortPwrList != null)
				swpPortPwrList.Clear();
			swpPortPwrList = null;
		}

		return swpPortPwrList;
	}




	/// <summary>
	/// 데이터를 등간격으로 만든다. 
	/// </summary>
	/// <param name="_starWavelen">start wavelength</param>
	/// <param name="_wavelenStep">wavelegnth step</param>
	/// <param name="_sourWaveLogList"></param>
	/// <param name="_sourPowList"></param>
	/// <returns></returns>
	private List<double> CalcEqualInterData(double _starWavelen, double _wavelenStep,
											List<double> _sourWaveLogList, 
											List<double> _sourPowList )
	{

		List<double> retList = null;

		try
		{

			double[] ptXarr =  new double[2];
			double[] ptYarr = new double[2];
			double wavelen = _starWavelen;
			int dataPoint =  _sourWaveLogList.Count();
			int index = 0;
			double interpolRes = 0;
			retList = new List<double>();
			for(int i = 0 ; i < dataPoint ; i++)
			{

				//find index.
				index = _sourWaveLogList.BinarySearch(wavelen);
				if (index < 0 )
				{
					index = index^(-1);

					if (index >= dataPoint)
						index = dataPoint - 1;
				}
					

				//Interpolation
				if( index <= 1 )
				{
					ptXarr[0] = _sourWaveLogList[0];
					ptXarr[1] = _sourWaveLogList[1];
					ptYarr[0] = _sourPowList[0];
					ptYarr[1] = _sourPowList[1];
				}
				else if( index == (dataPoint -1) )
				{
					ptXarr[0] = _sourWaveLogList[dataPoint - 2];
					ptXarr[1] = _sourWaveLogList[dataPoint - 1];
					ptYarr[0] = _sourPowList[dataPoint - 2];
					ptYarr[1] = _sourPowList[dataPoint - 1];
				}
				else
				{
					ptXarr[0] = _sourWaveLogList[index - 1];
					ptXarr[1] = _sourWaveLogList[index];
					ptYarr[0] = _sourPowList[index - 1];
					ptYarr[1] = _sourPowList[index];
				}


				
				if (ptYarr[0] > 100)
					interpolRes = ptYarr[0];
				else
					interpolRes = JeffMath.LinearInterpolation(ptXarr[0], ptYarr[0], 
																ptXarr[1], ptYarr[1],
																wavelen);

				interpolRes = Math.Round(interpolRes, 9);

				if (interpolRes <= 0.0)
					interpolRes = 0.000000001;
				


				//list에 추가
					retList.Add(interpolRes);

				//next wavelength.
				wavelen += _wavelenStep;
				wavelen = Math.Round(wavelen , 3 );
			}

		}
		catch
		{
			if (retList != null)
				retList.Clear();
			retList = null;
		}

		return retList;


	}



	
	/// <summary>
	/// stich multi range datas.
	/// finally make full range data.
	/// </summary>
	/// <param name="_gainLvls">gain level array.</param>
	/// <param name="_sourPowGainList"> multi gain power data </param>
	/// <returns></returns>
	private List<double> StichData(int[] _gainLvls,
								   List<List<double>> _sourPowGainList)
	{

		List<double> retList = null;

		try
		{

			retList = new List<double>();



			//gain level이 하나면 바로... 나간다!!.
			if (_sourPowGainList.Count() == 1)
			{
				for (int i = 0; i < _sourPowGainList[0].Count(); i++)
				{
					retList.Add(_sourPowGainList[0][i]);
				}
				return retList;
			}



			//원래 여러개의 gain의 값들을 stiching하다록 해야하는데...
			//그냥 2gain 의 값들을 stiching한다.
			int dataPoint = (_sourPowGainList[0]).Count();
			double valGain0 = 0.0;
			double valGain1 = 0.0;
			double valRes =0.0;
			for (int i = 0; i <= dataPoint - 1; i++)
			{
				valGain0 = (_sourPowGainList[0])[i];
				valGain1 = (_sourPowGainList[1])[i];

				if (valGain0 <= 0)
					valGain0 = 0.000000001;

				if (valGain1 <= 0)
					valGain1 = 0.000000001;



				if (JeffOptics.mW2dBm(valGain1) < _gainLvls[1])
				{

					//if (JeffOptics.mW2dBm(valGain1) > -80)
					//	valRes = valGain1;	
					//else
					//	valRes = valGain0;


					valRes = valGain1;

				}
				else
					valRes = valGain0;
		
				retList.Add(valRes);
			}


		}
		catch
		{
			if (retList != null)
				retList.Clear();
			retList = null;
		}

		return retList;

	}


	
	#endregion




	#region public method


	/// <summary>
	/// initialize sweep system.
	/// </summary>
	/// <returns></returns>
	public bool Init()
	{
		bool ret = false;

		if ((m_mpm == null) || (m_tls == null))
			ret = false;
		else
			ret = true;

		return ret;
	}




	/// <summary>
	/// Sweep mode로 변환
	/// 1.change tls sweep range
	/// 2.set PD's sweep parameter ( input trigger response , ...)
	/// </summary>
	/// <param name="_startWavelen"></param>
	/// <param name="_stopWavelen"></param>
	/// <param name="_step"></param>
	public void SetSweepMode(int[] _portNos, int _startWave, int _stopWave, double _stepWave)
	{

		//set device.
		m_tls.SetTlsSweepRange(_startWave, _stopWave, _stepWave);
		m_mpm.SetPdSweepMode(_portNos, _startWave, _stopWave, _stepWave);


		//make wavelength data arry. 
		if ((m_swpWavlenList != null) &&
			(m_swpWavlenList[0] == _startWave) &&
			(m_swpWavlenList[m_swpWavlenList.Count() - 1] == _stopWave) &&
			Math.Round((m_swpWavlenList[1] - m_swpWavlenList[0]), 3) == Math.Round(_stepWave, 3))
			return;

		int dataPoint = (int)((_stopWave - _startWave) / _stepWave) + 1;
		double wavelen = _startWave;
		m_swpWavlenList = null;
		m_swpWavlenList = new List<double>();
		m_swpWavlenList.Add( Math.Round(wavelen));
		for (int i = 1; i < dataPoint; i++)
		{
			wavelen += _stepWave;
			wavelen = Math.Round(wavelen, 3);
			m_swpWavlenList.Add(wavelen);
		}
		

	}




	/// <summary>
	/// Sweep mode로 변환
	/// 1.change tls sweep range
	/// 2.set PD's sweep parameter ( input trigger response , ...)
	/// </summary>
	/// <param name="_port">port no.</param>
	/// <param name="_startWave">start of sweep range [nm] </param>
	/// <param name="_stopWave">stop of sweep range [nm]</param>
	/// <param name="_stepWave">sweep step</param>
	public void SetSweepMode(int _port,	int _startWave,	int _stopWave,double _stepWave)
	{

		//set device.
		m_tls.SetTlsSweepRange(_startWave, _stopWave, _stepWave);
		m_mpm.SetPdSweepMode(_port, _startWave, _stopWave, _stepWave);

		//make wavelength data arry. 
		if ((m_swpWavlenList != null) &&
			(m_swpWavlenList[0] == _startWave) &&
			(m_swpWavlenList[m_swpWavlenList.Count() - 1] == _stopWave) &&
			Math.Round((m_swpWavlenList[1] - m_swpWavlenList[0]), 3) == Math.Round(_stepWave, 3))
			return;

		int dataPoint = (int)((_stopWave - _startWave) / _stepWave) + 1;
		double wavelen = _startWave;
		m_swpWavlenList = null;
		m_swpWavlenList = new List<double>();
		m_swpWavlenList.Add(Math.Round(wavelen));
		for (int i = 1; i < dataPoint; i++)
		{
			wavelen += _stepWave;
			wavelen = Math.Round(wavelen, 3);
			m_swpWavlenList.Add(wavelen);
		}

	}




	/// <summary>
	/// stop sweep mode.
	/// </summary>
	/// <param name="_portNos"></param>
	public void StopSweepMode(int[] _portNos)
	{
		m_mpm.StopPdSweepMode(_portNos);
	}




	/// <summary>
	/// stop sweep mode.
	/// </summary>
	/// <param name="_port"></param>
	public void StopSweepMode(int _port)
	{
		m_mpm.StopPdSweepMode(_port);
	}


  

	/// <summary>
	/// set wavlength offset.
	/// </summary>
	/// <param name="_wavelen">wavelength offset</param>
	public void SetWavelenOffset(double _wavelen)
	{
		m_wlOffset = _wavelen;
	}




	/// <summary>
	/// Execute Sweep as non-polarization mode.
	/// </summary>
	/// <param name="_port"></param>
	/// <param name="_gainLvl"></param>
	public void ExecSweepNonpol(int _port, int _gainLvl)
	{
		try
		{

			//delete data.
			if (m_swpPortPwrListNonpol != null)
				m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListNonpol = null;



			//Sweep하고 데이터를 획득한다.
			int dataPoint = 0;
			List<double> wlLogList = null;
			List<List<CswpPortPwr>> swPortPwrGainList = new List<List<CswpPortPwr>>();

			//Powermeter의 설정
			m_mpm.SetGainLevel(_port, _gainLvl);
			m_mpm.SetPdLogMode(_port);


			//tls 설정.
			if (wlLogList == null)
			{
				m_tls.TlsLogOn();
				Thread.Sleep(500);
			}

			//Sweep			
			m_tls.ExecTlsSweepCont();
			Thread.Sleep(2000);
			while (m_tls.IsTlsSweepOperating() == true)
			{
				Thread.Sleep(100);
			}
			
			

			//Wavlength logging Data를 얻는다.
			if (wlLogList == null)
				wlLogList = m_tls.GetTlsWavelenLog();


			//Optical Power data를 얻는다.
			CswpPortPwr swPortPwr = GetPowLogData(_port);


			//Logging Mode 해지...
			Thread.Sleep(100);
			m_tls.TlsLogOff();
			m_mpm.StopPdLogMode(_port);



			//Wavelength calibration
			for (int i = 0; i < wlLogList.Count(); i++)
			{
				wlLogList[i] += m_wlOffset;
			}
			dataPoint = wlLogList.Count();



			//logged power data를 등간격으로 만들어 준다.
			int swpRngStart = 0;
			int swpRngStop = 0;
			double swpRngStep = 0;
			m_tls.GetTlsSweepRange(ref swpRngStart, ref swpRngStop, ref swpRngStep);

			List<double> sourPwrList = swPortPwr.powList;
			List<double> retPwrList = null;
			retPwrList = CalcEqualInterData(swpRngStart,
											swpRngStep,
											wlLogList,
											sourPwrList);
			swPortPwr.powList = null;
			swPortPwr.powList = retPwrList;




			//list에 등록!!
			m_swpPortPwrListNonpol = new List<CswpPortPwrNonpol>();
			CswpPortPwrNonpol swPortPwrNonpol  = new CswpPortPwrNonpol();
			swPortPwrNonpol.port = swPortPwr.port;
			swPortPwrNonpol.powList = swPortPwr.powList;
			m_swpPortPwrListNonpol.Add(swPortPwrNonpol);



			//메모리 해제...
			swPortPwrGainList = null;

		}
		catch
		{

			if (m_swpPortPwrListNonpol != null)
				m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListNonpol = null;

		}


	}



	
	/// <summary>
	/// Execute Sweep as polarization mode.
	/// </summary>
	/// <param name="_port"></param>
	/// <param name="_gainLvl">detector gain level [dBm]</param>
	/// <param name="_polAngle">polarzation filter position at max. transmission.</param>
	public void ExecSweepPol(int _port, int _gainLvl, double _polAngle)
	{

		try
		{
			//delete data.
			if (m_swpPortPwrListPol != null)
				m_swpPortPwrListPol.Clear();
			else
				m_swpPortPwrListPol = new List<CswpPortPwrPol>();


			m_pc.SetPolFilterPos(_polAngle);


			//Sweep하고 데이터를 획득한다.
			int dataPoint = 0;
			List<double> wlLogList = null;
			CswpPortPwrPol swPortPwrPol = new CswpPortPwrPol();


			for (int i = 0; i < 4; i++)
			{


				//polarization 설정.
				switch (i)
				{
					case 0:
						m_pc.SetToLinearHorizontal(_polAngle);
						break;
					case 1:
						m_pc.SetToLinearVertical(_polAngle);
						break;
					case 2:
						m_pc.SetToLinearDiagonal(_polAngle);
						break;
					case 3:
						m_pc.SetToRHcircular(_polAngle);
						break;
				}


				//Powermeter의 설정
				m_mpm.SetGainLevel(_port, _gainLvl);
				m_mpm.SetPdLogMode(_port);


				//tls 설정.
				if (wlLogList == null)
					m_tls.TlsLogOn();


				//Sweep
				m_tls.ExecTlsSweepCont();
				Thread.Sleep(100);
				while (m_tls.IsTlsSweepOperating() == true)
				{
					Thread.Sleep(10);
				}


				//Wavlength logging Data를 얻는다.
				if (wlLogList == null)
					wlLogList = m_tls.GetTlsWavelenLog();

				//Optical Power data를 얻는다.
				CswpPortPwr swPortPwr = GetPowLogData(_port);


				//Logging Mode 해지...
				m_tls.TlsLogOff();
				m_mpm.StopPdLogMode(_port);



				//Wavelength calibration
				for (int j = 0; j < wlLogList.Count(); j++)
				{
					wlLogList[j] += m_wlOffset;
				}
				dataPoint = wlLogList.Count();



				//logged power data를 등간격으로 만들어 준다.
				int swpRngStart = 0;
				int swpRngStop = 0;
				double swpRngStep = 0;
				m_tls.GetTlsSweepRange(ref swpRngStart, ref swpRngStop, ref swpRngStep);

				List<double> sourPwrList = swPortPwr.powList;
				List<double> retPwrList = null;
				retPwrList = CalcEqualInterData(swpRngStart,
												swpRngStep,
												wlLogList,
												sourPwrList);
				swPortPwr.powList = null;
				swPortPwr.powList = retPwrList;




				//assign data
				swPortPwrPol.port = swPortPwr.port;
				switch (i)
				{
					case 0:
						swPortPwrPol.powList1 = swPortPwr.powList;
						break;
					case 1:
						swPortPwrPol.powList2 = swPortPwr.powList;
						break;
					case 2:
						swPortPwrPol.powList3 = swPortPwr.powList;
						break;
					case 3:
						swPortPwrPol.powList4 = swPortPwr.powList;
						break;
				}
				


			} // for i



			//list에 등록!!
			m_swpPortPwrListPol.Add(swPortPwrPol);


		}
		catch
		{

			if (m_swpPortPwrListPol != null)
				m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListPol = null;

		}


	}



	
	/// <summary>
	/// Execute Sweep as non-polarization mode.
	/// </summary>
	/// <param name="_portNos">port no. array.</param>
	/// <param name="_gainLvls">gain level array.</param>
	public void ExecSweepNonpol(int[] _portNos, int[] _gainLvls)
	{

		try
		{
			//delete data.
			if (m_swpPortPwrListNonpol != null) m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListNonpol = null;
			m_swpPortPwrListNonpol = new List<CswpPortPwrNonpol>();


			//Sweep하고 데이터를 획득한다.
			//int dataPoint = 0;
			List<double> wlLogList = null;
			List<List<CswpPortPwr>> swPortPwrGainList = new List<List<CswpPortPwr>>();
			for( int i=0; i < _gainLvls.Length ; i++)
			{

				//Powermeter의 설정
				m_mpm.SetGainLevel(_portNos, _gainLvls[i] );
				m_mpm.SetPdLogMode(_portNos);

				//tls 설정.
				if ( wlLogList == null ) m_tls.TlsLogOn();

				
				//Sweep
				m_tls.ExecTlsSweepCont();
				Thread.Sleep(500);
				while(m_tls.IsTlsSweepOperating() == true)
				{
					Thread.Sleep(10);
				}


				//Wavlength logging Data를 얻는다.
				if (wlLogList == null) wlLogList = m_tls.GetTlsWavelenLog();

				//Optical Power data를 얻는다.
				List<CswpPortPwr> swPortPwrList = GetPowLogData(_portNos);
				swPortPwrGainList.Add(swPortPwrList);

				//Logging Mode 해지...
				m_tls.TlsLogOff();
				m_mpm.StopPdLogMode(_portNos);

			}//for( int i=0; i < _gainLvls.Length ; i++)

			m_mpm.SetGainLevel(_portNos, _gainLvls[0]);


			//Wavelength calibration
			//0Band TLS은 현재 Calibration 할 수 없으므로
			//wavelength Offset는 걍 pass한다.!!
			//for ( int i = 0 ; i < wlLogList.Count() ; i++)
			//{
			//    wlLogList[i] += m_wlOffset;
			//}
			//dataPoint = wlLogList.Count();



			//logged power data를 등간격으로 만들어 준다.
			int swpRngStart = 0;
			int swpRngStop = 0;
			double swpRngStep = 0;
			m_tls.GetTlsSweepRange(ref swpRngStart, ref swpRngStop, ref swpRngStep);
			swpRngStep = Math.Round(swpRngStep, 3);
			for (int i = 0; i < _gainLvls.Length; i++)
			{
				for (int j = 0; j <_portNos.Length; j++)
				{
					List<double> sourPwrList = swPortPwrGainList[i][j].powList;
					List<double> retPwrList = null;
					retPwrList = CalcEqualInterData(swpRngStart,
													swpRngStep,
													wlLogList,
													sourPwrList);
					swPortPwrGainList[i][j].powList = null;
					swPortPwrGainList[i][j].powList = retPwrList;
				}
			}



			//data stiching...
			CswpPortPwrNonpol sweepPortPwr = null;
			List<List<double>> pwrGainList = new List<List<double>>();
			for (int i = 0; i < _portNos.Length; i++)
			{

				pwrGainList.Clear();
				for (int j = 0; j < _gainLvls.Length ; j++)
				{
					pwrGainList.Add(swPortPwrGainList[j][i].powList);
				}

				sweepPortPwr = new CswpPortPwrNonpol();
				sweepPortPwr.port = _portNos[i];
				sweepPortPwr.powList = StichData(_gainLvls, pwrGainList);
				m_swpPortPwrListNonpol.Add(sweepPortPwr);

			}


			//메모리 해제...
			swPortPwrGainList = null;

		}
		catch
		{

			if (m_swpPortPwrListNonpol != null)
				m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListNonpol = null;

		}

	}




    /// <summary>
    /// Execute Sweep as non-polarization mode.
    /// </summary>
    /// <param name="_portNos">port no. array.</param>
    /// <param name="_gainLvls">gain level array.</param>
    public void ExecSweepNonpol(int[] _portNos, int[] _gainLvls, LogItem log)
    {

        try
        {
            //delete data.
            if (m_swpPortPwrListNonpol != null) m_swpPortPwrListNonpol.Clear();
            m_swpPortPwrListNonpol = null;
            m_swpPortPwrListNonpol = new List<CswpPortPwrNonpol>();


            //Sweep하고 데이터를 획득한다.
            //int dataPoint = 0;
            List<double> wlLogList = null;
            List<List<CswpPortPwr>> swPortPwrGainList = new List<List<CswpPortPwr>>();
            for (int i = 0; i < _gainLvls.Length; i++)
            {

                //Powermeter의 설정
                m_mpm.SetGainLevel(_portNos, _gainLvls[i]);
                log.RecordLogItem("ExecSweepNonpol", "P.M - Gain " + _gainLvls[i].ToString() + "dBm");   //LogItem
                m_mpm.SetPdLogMode(_portNos);
                log.RecordLogItem("ExecSweepNonpol", "P.M - LogMode");              //LogItem

                //tls 설정.
                if (wlLogList == null) m_tls.TlsLogOn();
                log.RecordLogItem("ExecSweepNonpol", "TLS - LogOn");                //LogItem

                //Sweep
                m_tls.ExecTlsSweepCont();
                Thread.Sleep(500);
                while (m_tls.IsTlsSweepOperating() == true)
                {
                    Thread.Sleep(10);
                }
                log.RecordLogItem("ExecSweepNonpol", "TLS - SWEEP");                //LogItem

                //Wavlength logging Data를 얻는다.
                if (wlLogList == null)
                {
                    wlLogList = m_tls.GetTlsWavelenLog();
                    log.RecordLogItem("ExecSweepNonpol", "TLS - Get WL log");       //LogItem
                }

                //Optical Power data를 얻는다.
                List<CswpPortPwr> swPortPwrList = GetPowLogData(_portNos);
                swPortPwrGainList.Add(swPortPwrList);
                log.RecordLogItem("ExecSweepNonpol", "P.M - Get PM Log");           //LogItem

                //Logging Mode 해지...
                m_tls.TlsLogOff();
                log.RecordLogItem("ExecSweepNonpol", "TLS - LogOff");               //LogItem
                m_mpm.StopPdLogMode(_portNos);
                log.RecordLogItem("ExecSweepNonpol", "P.M - LogOff");               //LogItem

            }//for( int i=0; i < _gainLvls.Length ; i++)

            m_mpm.SetGainLevel(_portNos, _gainLvls[0]);


            //Wavelength calibration
            //0Band TLS은 현재 Calibration 할 수 없으므로
            //wavelength Offset는 걍 pass한다.!!
            //for ( int i = 0 ; i < wlLogList.Count() ; i++)
            //{
            //    wlLogList[i] += m_wlOffset;
            //}
            //dataPoint = wlLogList.Count();



            //logged power data를 등간격으로 만들어 준다.
            int swpRngStart = 0;
            int swpRngStop = 0;
            double swpRngStep = 0;
            m_tls.GetTlsSweepRange(ref swpRngStart, ref swpRngStop, ref swpRngStep);
            swpRngStep = Math.Round(swpRngStep, 3);
            for (int i = 0; i < _gainLvls.Length; i++)
            {
                for (int j = 0; j < _portNos.Length; j++)
                {
                    List<double> sourPwrList = swPortPwrGainList[i][j].powList;
                    List<double> retPwrList = null;
                    retPwrList = CalcEqualInterData(swpRngStart,
                                                    swpRngStep,
                                                    wlLogList,
                                                    sourPwrList);
                    swPortPwrGainList[i][j].powList = null;
                    swPortPwrGainList[i][j].powList = retPwrList;
                }
            }



            //data stiching...
            CswpPortPwrNonpol sweepPortPwr = null;
            List<List<double>> pwrGainList = new List<List<double>>();
            for (int i = 0; i < _portNos.Length; i++)
            {

                pwrGainList.Clear();
                for (int j = 0; j < _gainLvls.Length; j++)
                {
                    pwrGainList.Add(swPortPwrGainList[j][i].powList);
                }

                sweepPortPwr = new CswpPortPwrNonpol();
                sweepPortPwr.port = _portNos[i];
                sweepPortPwr.powList = StichData(_gainLvls, pwrGainList);
                m_swpPortPwrListNonpol.Add(sweepPortPwr);

            }


            //메모리 해제...
            swPortPwrGainList = null;

        }
        catch
        {

            if (m_swpPortPwrListNonpol != null)
                m_swpPortPwrListNonpol.Clear();
            m_swpPortPwrListNonpol = null;

        }

    }




    /// <summary>
    /// Execute Sweep as non-polarization mode.
    /// </summary>
    /// <param name="_portNos">port no. array.</param>
    /// <param name="_gainLvls">gain level array.</param>
    /// <param name="_polAngle">polarization filter pos at max. transmission</param>
    public void ExecSweepPol(int[] _portNos, int[] _gainLvls, double _polAngle)
	{

		try
		{
			//delete data.
			if (m_swpPortPwrListPol != null)
				m_swpPortPwrListPol.Clear();
			else
				m_swpPortPwrListPol = new List<CswpPortPwrPol>();



			//polarization filter pos. 설정.
			m_pc.SetPolFilterPos(_polAngle);


			//Sweep하고 데이터를 획득한다.
			int dataPoint = 0;
			List<double> wlLogList = null;

			int gainCnt = _gainLvls.Length;
			int portCnt = _portNos.Length;
			CswpPortPwr[][][] swPortPwrGainPolPorts = new CswpPortPwr[_gainLvls.Length][][];
			for (int i=0; i < gainCnt; i++)
			{
				swPortPwrGainPolPorts[i] = new CswpPortPwr[4][];
				for (int j = 0; j < portCnt; j++)
				{
					swPortPwrGainPolPorts[i][j] = new CswpPortPwr[portCnt];
				}
			}


			for (int i = 0; i < _gainLvls.Length; i++)
			{

				//gain level 설정.
				m_mpm.SetGainLevel(_portNos, _gainLvls[i]);


				for (int j = 0; j < 4; j++)
				{


					//polarization 설정.
					switch (j)
					{
						case 0:
						m_pc.SetToLinearHorizontal(_polAngle);
						break;
						case 1:
						m_pc.SetToLinearVertical(_polAngle);
						break;
						case 2:
						m_pc.SetToLinearDiagonal(_polAngle);
						break;
						case 3:
						m_pc.SetToRHcircular(_polAngle);
						break;
					}


					//Powermeter logging mode.
					m_mpm.SetPdLogMode(_portNos);


					//tls 설정.
					if (wlLogList == null)
						m_tls.TlsLogOn();


					//Sweep
					m_tls.ExecTlsSweepCont();
					Thread.Sleep(100);
					while (m_tls.IsTlsSweepOperating() == true)
					{
						Thread.Sleep(10);
					}


					//Wavlength logging Data를 얻는다.
					if (wlLogList == null)
						wlLogList = m_tls.GetTlsWavelenLog();

					//Optical Power data를 얻는다.
					List<CswpPortPwr> swPortPwrList = GetPowLogData(_portNos);
					swPortPwrGainPolPorts[i][j] = swPortPwrList.ToArray();


					//Logging Mode 해지...
					m_tls.TlsLogOff();
					m_mpm.StopPdLogMode(_portNos);

				}// for j


			}// for i
			m_mpm.SetGainLevel(_portNos, _gainLvls[0]);




			//Wavelength calibration
			for (int i = 0; i < wlLogList.Count(); i++)
			{
				wlLogList[i] += m_wlOffset;
			}
			dataPoint = wlLogList.Count();




			//data stiching...
			CswpPortPwrPol sweepPortPwr = null;
			List<List<double>> pwrGainList = new List<List<double>>();
			for (int i = 0; i < _portNos.Length; i++)
			{


				sweepPortPwr = new CswpPortPwrPol();
				sweepPortPwr.port = _portNos[i];


				for (int j = 0; j < 4; j++)
				{

					pwrGainList.Clear();
					for (int k = 0; k < _gainLvls.Length; k++)
					{
						pwrGainList.Add(swPortPwrGainPolPorts[k][j][i].powList);
					}


					switch (j)
					{
						case 0:
							sweepPortPwr.powList1 = StichData(_gainLvls, pwrGainList);
							break;
						case 1:
							sweepPortPwr.powList2 = StichData(_gainLvls, pwrGainList);
							break;
						case 2:
							sweepPortPwr.powList3 = StichData(_gainLvls, pwrGainList);
							break;
						case 3:
							sweepPortPwr.powList4 = StichData(_gainLvls, pwrGainList);
							break;
					}

					
				}

				m_swpPortPwrListPol.Add(sweepPortPwr);

			}


			//메모리 해제...
			swPortPwrGainPolPorts = null;

		}
		catch
		{

			if (m_swpPortPwrListNonpol != null)
				m_swpPortPwrListNonpol.Clear();
			m_swpPortPwrListNonpol = null;

		}

	}



	
	/// <summary>
	/// after sweep(Non polarization), get the power datas of port.
	/// </summary>
	/// <param name="port">port no.</param>
	/// <returns>instance of CswpPortPwrNonpol</returns>
	public CswpPortPwrNonpol GetSwpPwrDataNonpol(int port)
	{
		CswpPortPwrNonpol ret = null;
		ret = m_swpPortPwrListNonpol.Find(p => p.port == port);
		return ret;
	}




	/// <summary>
	/// after sweep(Non polarization), get the power datas of ports.
	/// </summary>
	/// <param name="ports">port no. array</param>
	/// <returns> list including instance of CswpPortPwrNonpol</returns>
	public List<CswpPortPwrNonpol> GetSwpPwrDataNonpol(int[] _ports)
	{

		List<CswpPortPwrNonpol> retList = null;

		try
		{
			retList = new List<CswpPortPwrNonpol>();

			CswpPortPwrNonpol swpPortPwr = null;
			for (int i = 0; i < _ports.Length; i++)
			{
				swpPortPwr = m_swpPortPwrListNonpol.Find(p => p.port == _ports[i]);
				retList.Add(swpPortPwr);
			}

		}
		catch
		{
			if (retList != null)
			{
				retList.Clear();
				retList = null;
			}
		}

		return retList;
	}




	/// <summary>
	/// after sweep(polarization), get the power datas of port.
	/// not supported!!
	/// </summary>
	/// <param name="port">port no.</param>
	/// <returns>return null</returns>
	public CswpPortPwrPol GetSwpPwrDataPol(int port)
	{

		CswpPortPwrPol ret = null;
		ret = m_swpPortPwrListPol.Find(p => p.port == port);
		return ret;

	}




	/// <summary>
	/// after sweep(polarization), get the power datas of ports.
	/// not supported!!
	/// </summary>
	/// <param name="_ports">port no. array.</param>
	/// <returns>return null</returns>
	public List<CswpPortPwrPol> GetSwpPwrDataPol(int[] _ports)
	{

		List<CswpPortPwrPol> retList = null;

		try
		{
			retList = new List<CswpPortPwrPol>();

			CswpPortPwrPol swpPortPwr = null;
			for (int i = 0; i < _ports.Length; i++)
			{
				swpPortPwr = m_swpPortPwrListPol.Find(p => p.port == _ports[i]);
				retList.Add(swpPortPwr);
			}

		}
		catch
		{
			if (retList != null)
			{
				retList.Clear();
				retList = null;
			}
		}

		return retList;

	}



	
	/// <summary>
	/// Find Maximum polarization filter pos.
	/// </summary>
	/// <param name="_port"> port </param>
	/// <param name="_wavelen">Powermeter wavelength , TLS wavelength </param>
	/// <returns></returns>
	public double FindMaxPolPos(int _port)
	{

		double ret = 0.0;

		try
		{
			m_pc.SetHalfRetarderPos(0);
			m_pc.SetQuarRetarderPos(0);

			List<double> pwrList = new List<double>();
			double pwr = 0.0;
			double pos = 0.0;
			double step = 0.2;
			double maxPos = 0.0;
			double maxPwr = 0.0;
			while (true)
			{


				if (pos > 180)
					break; 

				m_pc.SetPolFilterPos(pos);
				pwr = m_mpm.ReadPwr(_port);

				if (pos == 0.0)
				{
					maxPos = pos;
					maxPwr = pwr;
				}


				if (pwr > maxPwr)
				{
					maxPos = pos;
					maxPwr = pwr;
				}

				pos += Math.Round(step,2);
			}


			m_pc.SetPolFilterPos(maxPos);

			ret = maxPos;

		}
		catch
		{
			ret = 0.0;
		}

		return ret;

	}



	
	/// <summary>
	/// calculates PDL using Muller-Method
	/// </summary>
	/// <param name="_startWL">start wavelength</param>
	/// <param name="_stepWL">step of wavelength</param>
	/// <param name="_refPwrLH">ref. data of polarization state1</param>
	/// <param name="_refPwrLDP45">ref. data of polarization state2</param>
	/// <param name="_refPwrLDN45">ref. data of polarization state3</param>
	/// <param name="_refPwrRHC">ref. data of polarization state4</param>
	/// <param name="_dutPwrLH">transmission of polarization state1</param>
	/// <param name="_dutPwrLDP45">transmission of polarization state2</param>
	/// <param name="_dutPwrLDN45">transmission of polarization state3</param>
	/// <param name="_dutPwrRHC">transmission of polarization state4</param>
	/// <param name="_ilMin">[out] il minimum (max. transmission) </param>
	/// <param name="_ilMax">[out] il minimum (min. transmission)</param>
	/// <param name="_ilAvg">[out] il minimum (min. transmission)</param>
	/// <param name="_pdl"></param>
	public void CalcPDLMuller(double _refPwrLH,
							  double _refPwrLDP45,
							  double _refPwrLDN45,
							  double _refPwrRHC,
							  double _dutPwrLH,
							  double _dutPwrLDP45,
							  double _dutPwrLDN45,
							  double _dutPwrRHC,
							  ref double _ilMin,
							  ref double _ilMax,
							  ref double _ilAvg,
							  ref double _pdl)
	{
		
		try
		{
			double m11 = 0;
			double m12 = 0;
			double m13 = 0;
			double m14 = 0;
			double Tmax = 0;
			double Tmin = 0;
			double dbTemp = 0;

			m11 = 0.5 * ((_dutPwrLH / _refPwrLH) + (_dutPwrLDP45 / _refPwrLDP45));
			m12 = 0.5 * ((_dutPwrLH / _refPwrLH) - (_dutPwrLDP45 / _refPwrLDP45));
			m13 = (_dutPwrLDN45 / _refPwrLDN45) - m11;
			m14 = (_dutPwrRHC / _refPwrRHC) - m11;

			dbTemp = Math.Sqrt(m12 * m12 + m13 * m13 + m14 * m14);
			Tmax = Math.Abs(m11 + dbTemp);
			Tmin = Math.Abs(m11 - dbTemp);

			_ilMin = 10 * Math.Log10(Tmax);       //Maximum Transmitance
			_ilMax = 10 * Math.Log10(Tmin);       //Minimum Transmitance
			_ilAvg = 10 * Math.Log10((Tmax + Tmin) / 2);
			_pdl = 10 * Math.Log10(Tmax / Tmin);

		}
		catch
		{
			_ilMin = 0;
			_ilMax = 0;
			_ilAvg = 0;
			_pdl = 0;
		}


	}



	
	/// <summary>
	/// calculates PDL using Muller-Method
	/// </summary>
	/// <param name="_startWL">start wavelength</param>
	/// <param name="_stepWL">step of wavelength</param>
	/// <param name="_refPwrLH">ref. data of polarization state1</param>
	/// <param name="_refPwrLDP45">ref. data of polarization state2</param>
	/// <param name="_refPwrLDN45">ref. data of polarization state3</param>
	/// <param name="_refPwrRHC">ref. data of polarization state4</param>
	/// <param name="_dutPwrLH">transmission of polarization state1</param>
	/// <param name="_dutPwrLDP45">transmission of polarization state2</param>
	/// <param name="_dutPwrLDN45">transmission of polarization state3</param>
	/// <param name="_dutPwrRHC">transmission of polarization state4</param>
	/// <param name="_ilMin">[out] il minimum (max. transmission) </param>
	/// <param name="_ilMax">[out] il minimum (min. transmission)</param>
	/// <param name="_ilAvg">[out] il minimum (min. transmission)</param>
	/// <param name="_pdl"></param>
	public void CalcPDLMuller(double _refPwrLH,
							  double _refPwrLDP45,
							  double _refPwrLDN45,
							  double _refPwrRHC,
							  double _dutPwrLH,
							  double _dutPwrLDP45,
							  double _dutPwrLDN45,
							  double _dutPwrRHC,
							  ref double _ilMin,
							  ref double _ilMax)
	{
		
		try
		{                        
			double m11 = 0;
			double m12 = 0;
			double m13 = 0;
			double m14 = 0;
			double Tmax = 0;
			double Tmin = 0;
			double dbTemp = 0;

			m11 = 0.5 * ((_dutPwrLH / _refPwrLH) + (_dutPwrLDP45 / _refPwrLDP45));
			m12 = 0.5 * ((_dutPwrLH / _refPwrLH) - (_dutPwrLDP45 / _refPwrLDP45));
			m13 = (_dutPwrLDN45 / _refPwrLDN45) - m11;
			m14 = (_dutPwrRHC / _refPwrRHC) - m11;

			dbTemp = Math.Sqrt(m12 * m12 + m13 * m13 + m14 * m14);
			Tmax = Math.Abs(m11 + dbTemp);
			Tmin = Math.Abs(m11 - dbTemp);

			_ilMin = 10 * Math.Log10(Tmax);       //Maximum Transmitance
			_ilMax = 10 * Math.Log10(Tmin);       //Minimum Transmitance

		}
		catch
		{
			_ilMin = 0;
			_ilMax = 0;
		}



	}

	
	#endregion

	

}

