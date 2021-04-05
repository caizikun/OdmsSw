using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Jeffsoft;
using Neon.Aligner;
using Free302.TnMLibrary.DataAnalysis;
using System.IO;


public partial class frmWdmRef : Form
{



	#region definition

	private const int VALVE1 = 1;
	private const int VALVE2 = 2;
    
    private const int XYSEARCHSTEP = 1;

    private double TLS_OUTPWR = CGlobal.TlsParam.Power;      //[dBm]
    private int GAIN_LEVEL = CGlobal.PmGain[0];            //[dBm]
    
    private const int SWEEPWAVE_START = 1260;           //[nm]
	private const int SWEEPWAVE_STOP = 1360;            //[nm]
	private const int SWEEPSPEED = 40;
	private const int SWEEPWAVE_STEP = 50;              //[9m]
    

	private const int DEFAULT_FACOREPITCH = 127;
	private const int DEFAULT_OPTPWRTHRES = 0;
	private const int DEFAULT_ALIGNWAVELEN = 1260;

	private const int CMD_SINGLEPORT = 0;
	private const int CMD_MULTIPORTS = 1;
	private const int CMD_GOPORT = 2;
	private const int CMD_MSR = 3;
	private const int CMD_GOINITPOS = 4;

	#endregion





	#region structure/innor class


	private struct option
	{
		public int optPwrThre;                      //[dBm] 
		public int corePitch;                       //[um] Output FA Corepicth
		public int alignWavelen;                    //[nm]Wavelength for Alignment;
	}




	private struct threadParam
	{
		public int cmd;
		public int startPort;
		//public int lastPort;
		public bool align;
	}




	private struct initPos
	{
		public CStageAbsPos inpos;
		public CStageAbsPos outpos;
	}

    
	#endregion





	#region member variables

   
	private SweepLogic.CswpRefNonpol m_ref;
	private SweepLogic.CswpRefNonpol m_newRef;

	private Itls m_tls;
	private IoptMultimeter m_mpm;
	private Istage m_leftStg;
	private Istage m_rightStg;
	private IairValvController m_avc;


	private SweepLogic m_swSys;
	private IAlignment m_align;


	private option m_opt;

	bool m_stopFlag;
	bool m_isRuning;                                 //running:true , stop :false
	private threadParam m_tp;
	private AutoResetEvent m_autoEvent;
	private Thread m_thread;

	private initPos m_initPos;

    int mWlStep = SWEEPWAVE_STEP;

    #endregion





    #region ==== TEST: Reference File, GainLevel File 2016-02-23 DrBae ====

    [STAThread]
	public static void Main()
	{
		frmWdmRef f = new frmWdmRef();
		f.ShowDialog();
	}

	public frmWdmRef()
	{
		InitializeComponent();

        this.Load += Form_Load;
        this.btnLoadReference.Click += BtnLoadReference_Click;
        this.btnExportReference.Click += BtnExportReference_Click;
    }


	private void Form_Load(object sender, EventArgs e)
	{
		loadReference(false);
	}

	private void BtnLoadReference_Click(object sender, EventArgs e)
	{
		loadReference(true);
	}

	private void BtnExportReference_Click(object sender, EventArgs e)
	{
		exportReference();
	}

	#region ---- Referecen File Handler ----

	//string mDefaultReferenceFile = "reference_np.txt";
	string mLastReferenceFile;
	string mReferenceButtonText = "Reference 불러오기: {0} ({1} {2})";
	
	private void loadReference(bool showDialog)
	{
		//string fileName = Path.Combine(Application.StartupPath, mDefaultReferenceFile);
		string fileName = CGlobal.g_refPath;
		if (showDialog || !File.Exists(fileName))
		{
			OpenFileDialog fd = new OpenFileDialog();
			fd.Title = "REFERENCE 파일??";
			fd.InitialDirectory = Application.StartupPath;
			fd.Filter = "Reference File(*.txt)|*.txt";
			if (fd.ShowDialog() != DialogResult.OK) return;
			fileName = fd.FileName;
		}

		m_ref = new SweepLogic.CswpRefNonpol();
		if (!m_ref.LoadFromTxt(fileName))
		{
			MessageBox.Show("레퍼런스 값을 불러오는데 실패!!");
			m_ref = null;
			return;
		}

		mLastReferenceFile = fileName;
		CGlobal.g_refPath = fileName;    //[2016-10-31 ko]

		DateTime time = File.GetLastWriteTime(fileName);
		this.btnLoadReference.Text = string.Format(mReferenceButtonText,
			Path.GetFileName(fileName), time.ToShortDateString(), time.ToString("HH:mm:ss"));

		//plot:
		DisplayList();
		PlotCurRef(1);
	}

	void exportReference()
	{
		SaveFileDialog fd = new SaveFileDialog();
		fd.Title = "REFERENCE 파일??";
		fd.FileName = mLastReferenceFile;
		fd.Filter = "Reference File(*.txt)|*.txt";
		if (fd.ShowDialog() != DialogResult.OK) return;

		//save
		//save to file.
		if (!m_ref.SaveToTxt(fd.FileName))
		{
			string msg = "레퍼런스 데이터를 저장하는데 실패!!";
			MessageBox.Show(msg, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	#endregion


	#endregion





	#region ThreadFunction

    
    /// <summary>
    /// Thread
    /// </summary>
	private void ThreadFunc()
	{

		Action<Label, string> slt = SetLabelText;
		Action<string> stssl = SetTssLabelText;
		Action<int> pnr = PlotNewRef;
		Action ew = EnableWnd;


		frmSourceController frmSourCont = null;
		if (Application.OpenForms.OfType<frmSourceController>().Count() > 0)
			frmSourCont = Application.OpenForms.OfType<frmSourceController>().FirstOrDefault();


		while (true)
		{

			//시작 신호 대기
			m_isRuning = false;
			m_autoEvent.WaitOne();
			m_isRuning = true;
			m_stopFlag = false;


			switch (m_tp.cmd)
			{
				case CMD_GOPORT:
					{
						//position 
						int targPort = m_tp.startPort;
						int curPort = FindCurFaPortPos();
						if (curPort < 0)
						{
							MessageBox.Show("In FA의 위치를 찾는데 실패!!",
											"에러",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error);
							break;
						}


						//Target Port로 이동
						int dist = 0;
						dist = m_opt.corePitch * (targPort - curPort);
						m_leftStg.RelMove(m_leftStg.AXIS_Z, -100);
						m_leftStg.RelMove(m_leftStg.AXIS_X, dist);
						m_leftStg.WaitForIdle(m_leftStg.AXIS_X);
						m_leftStg.RelMove(m_leftStg.AXIS_Z, 85);
						m_leftStg.WaitForIdle(m_leftStg.AXIS_X);



						//확인..
						curPort = FindCurFaPortPos();
						if (curPort != targPort)
						{
							MessageBox.Show("In FA 이동 후 위치 이상. ",
											"에러",
											MessageBoxButtons.OK,
											MessageBoxIcon.Error);
							Invoke(slt, new object[] { lbCurPort, "???" });
							break;
						}

						break;
					}


				case CMD_GOINITPOS:


					
					m_leftStg.AbsMove(m_leftStg.AXIS_X, m_initPos.inpos.x);
					m_rightStg.AbsMove(m_rightStg.AXIS_X, m_initPos.outpos.x);

					m_leftStg.AbsMove(m_leftStg.AXIS_Y, m_initPos.inpos.y);
					m_rightStg.AbsMove(m_rightStg.AXIS_Y, m_initPos.outpos.y);

					m_leftStg.AbsMove(m_leftStg.AXIS_TX, m_initPos.inpos.tx);
					m_rightStg.AbsMove(m_rightStg.AXIS_TX, m_initPos.outpos.tx);

					m_leftStg.AbsMove(m_leftStg.AXIS_TY, m_initPos.inpos.ty);
					m_rightStg.AbsMove(m_rightStg.AXIS_TY, m_initPos.outpos.ty);

					m_leftStg.AbsMove(m_leftStg.AXIS_TZ, m_initPos.inpos.tz);
					m_rightStg.AbsMove(m_rightStg.AXIS_TZ, m_initPos.outpos.tz);

					m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_initPos.inpos.z);
					m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_initPos.outpos.z);

					while (m_rightStg.IsMovingOK())
					{

						if ( m_stopFlag == true)
						{
							m_leftStg.StopMove();
							m_rightStg.StopMove();
							break;
						}

					}


					break;

				case CMD_SINGLEPORT:
					{

                        //---------- Port 검사 ---------//
                        int curPort = FindCurFaPortPos();
                        //.
                        //.
                        //.



                        #region -------  Averaging -----------------

                        int numAverage = 5, numMovingAverage = 10;
                        textAverage.Invoke(new Action(delegate ()
                        {
                            if (!int.TryParse(textAverage.Text, out numAverage)) numAverage = 5;
                        }));
                        textMA.Invoke(new Action(delegate ()
                        {
                            if (!int.TryParse(textMA.Text, out numMovingAverage)) numMovingAverage = 10;
                        }));
                        mAveraging = new Averaging(numAverage, numMovingAverage);

                        bool boolCancel = false;
                        for (int i = 0; i < mAveraging.NumAveraging; i++)
                        {
                            //test mode by DrBae
                            btnNewStart_single.Invoke(new Action(delegate ()
                            {
                                textAverage.Text = $"{mAveraging.NumAveraging - i}";
                                textAverage.Update();
                            }));


                            //--------- alignment --------//
                            if (m_tp.align == true)
                            {

                                IAlignmentDigital alignDigi = null;
                                IAlignmentFa alignFa = null;
                                try
                                {
                                    alignDigi = (IAlignmentDigital)(m_align);
                                    alignFa = (IAlignmentFa)(m_align);
                                }
                                catch
                                {
                                    MessageBox.Show("이 시스템은 Alignment를 지원하지 않습니다.",
                                                    "에러",
                                                    MessageBoxButtons.OK,
                                                    MessageBoxIcon.Error);
                                    return;
                                }



                                //Disable optical source controller
                                if (frmSourCont != null)
                                    frmSourCont.DisableForm();



                                //TLS wavelength 변경.
                                Invoke(stssl, "TLS wavlength 설정");
                                m_tls.SetTlsWavelen(m_opt.alignWavelen);
                                if (m_stopFlag == true)
                                    return;


                                //100um 후진
                                Invoke(stssl, "100um back (Left Stage)... ");
                                m_leftStg.RelMove(m_leftStg.AXIS_Z, -100);
                                m_leftStg.WaitForIdle(m_leftStg.AXIS_Z);
                                if (m_stopFlag == true) return;


                                //Left-stage ZApproach
                                Invoke(stssl, "ZApproach(Left Stage)... ");
                                alignFa.ZappSingleStage(m_align.LEFT_STAGE);
                                if (m_stopFlag == true)
                                    return;


                                //10um 후진
                                Invoke(stssl, "10um back (input Stage)... ");
                                m_leftStg.RelMove(m_leftStg.AXIS_Z, -10);
                                m_leftStg.WaitForIdle(m_leftStg.AXIS_Z);
                                if (m_stopFlag == true)
                                    return;


                                //input Stage  XY Fine Searching
                                Invoke(stssl, "XySearching (Left Stage)... ");
                                alignDigi.XySearch(m_align.LEFT_STAGE, m_tp.startPort, XYSEARCHSTEP);
                                if (m_stopFlag == true)
                                    return;

                            }


                            //--------- measurement --------//

                            //진행 여부?? 
                            double pwr = 0;
                            string strCont = "";
                            pwr = JeffOptics.mW2dBm(m_mpm.ReadPwr(m_tp.startPort));
                            pwr = Math.Round(pwr, 3);
                            strCont = "Power : " + pwr.ToString() + " [dBm]" + "\nContinue?";
                            DialogResult dialRes;
                            if (i == 0)
                            {
                                dialRes = MessageBox.Show(strCont, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (dialRes == DialogResult.No)
                                {
                                    boolCancel = true;
                                    break;//break averaging loop
                                }
                            }

                            //display Digital powermeter off
                            if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
                            {
                                frmDigitalOptPowermeter frm = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();
                                frm.DisplayOff();
                            }


                            //sweep and acquire data
                            Invoke(stssl, "Measurement... ");
                            int startWave = SWEEPWAVE_START;
                            int stopWave = SWEEPWAVE_STOP;
                            double stepWave = Math.Round(mWlStep / 1000.0, 3);

                            m_swSys.SetSweepMode(m_tp.startPort, startWave, stopWave, stepWave);
                            Thread.Sleep(100);
                            m_swSys.ExecSweepNonpol(m_tp.startPort, GAIN_LEVEL);
                            Thread.Sleep(100);
                            m_swSys.StopSweepMode(m_tp.startPort);

                            SweepLogic.PortPowerPair swPwr = null;
                            swPwr = m_swSys.GetSwpPwrDataNonpol(m_tp.startPort);

                            //edit by DrBae 2015-10-26
                            mAveraging.AddData(swPwr.Power);

                            //insert data to ref.
                            if (m_newRef == null)
                            {
                                m_newRef = new SweepLogic.CswpRefNonpol();
                                m_newRef.Clear(startWave, stopWave, stepWave);
                            }
                            m_newRef.SetPortData(swPwr);

                            //plot..
                            Invoke(pnr, m_tp.startPort);

                            //inter measurement delay
                            Thread.Sleep(100);

                        }//averaging

                        //apply moving average //edit by DrBae 2015-10-26
                        if (!boolCancel) applyMovingAverage();

                        //display numAverag
                        btnNewStart_single.Invoke(new Action(delegate ()
                        {
                            textAverage.Text = $"{mAveraging.NumAveraging}";
                            textAverage.Update();
                        }));

                        #endregion ----- ~ averaging



                        //display Digital powermeter on
                        if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
                        {
                            frmDigitalOptPowermeter frm = Application.OpenForms.OfType<frmDigitalOptPowermeter>().FirstOrDefault();
                            frm.DisplayOn();
                        }


                        //Enable optical source controller
                        if (frmSourCont != null)
                            frmSourCont.EnableForm();


                        break;
                    }


                case CMD_MULTIPORTS:

					break;
			}


			//완료 메세지.//
			Invoke(ew);
			if (m_stopFlag == true)
			{
				MessageBox.Show("명령이 중지되었습니다",
								"확인",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning);
			}
			else
			{
				MessageBox.Show("완료!!",
								"확인",
								MessageBoxButtons.OK,
								MessageBoxIcon.Information);
			}


		}//while (true)


	}


    

	#region ==== Moving Averaging ====

	Averaging mAveraging;
	
	private void btnMA_Click(object sender, EventArgs e)
	{
		int numAvg;
		if (!int.TryParse(textMA.Text, out numAvg)) numAvg=1;
		mAveraging.NumMovingAveraging = numAvg;
		applyMovingAverage();
	}
	void applyMovingAverage()
	{
		if(mAveraging == null) return;
		mAveraging.ApplyMovingAverage(m_swSys.GetSwpPwrDataNonpol(m_tp.startPort).Power);

		//plot
		Invoke(new Action<int>(PlotNewRef), m_tp.startPort);

	}
	#endregion


	#endregion





	#region private method


	/// <summary>
	/// widow start postion을 불러온다.
	/// </summary>
	/// <param name="_filePath">config file path.</param>
	/// <returns></returns>
	private Point LoadWndStartPos(string _filePath)
	{

		Point ret = new Point();

		string temp = "";
		try
		{

			Cconfig conf = new Cconfig(_filePath);


			try
			{
				temp = conf.GetValue("WNDPOSX");
				ret.X = Convert.ToInt32(temp);
			}
			catch
			{
				ret.X = 0;
			}


			try
			{
				temp = conf.GetValue("WNDPOSY");
				ret.Y = Convert.ToInt32(temp);
			}
			catch
			{
				ret.Y = 0;
			}

		}
		catch
		{
			ret.X = 0;
			ret.Y = 0;
		}

		return ret;
	}




	/// <summary>
	/// widow start postion을 저장한다.
	/// </summary>
	/// <param name="_filePath">config file path.</param>
	/// <returns></returns>
	private Point SaveWndStartPos(string _filePath)
	{

		Point ret = new Point();
		Cconfig conf = null;

		string temp = "";
		try
		{
			conf = new Cconfig(_filePath);


			temp = this.Location.X.ToString();
			conf.SetValue("WNDPOSX", temp);


			temp = this.Location.Y.ToString();
			conf.SetValue("WNDPOSY", temp);


		}
		catch
		{
			//do nothing
		}
		finally
		{
			if (conf != null)
				conf.Dispose();

			conf = null;
		}


		return ret;
	}




	/// <summary>
	/// set ToolStripStatus Label text.
	/// </summary>
	/// <param name="_msg">message </param>
	private void SetTssLabelText(string _msg)
	{
		ToolStripStatusLabel1.Text = _msg;
	}



	
	/// <summary>
	/// Set Label text 
	/// </summary>
	private void SetLabelText(Label _label,string _msg)
	{
		_label.Text = _msg;
	}




	/// <summary>
	/// Init. Pos.으로 스테이지 이동.
	/// </summary>
	private void MoveToInitPos()
	{

		//포지션이 설정되지 않았으면 이동하지 않는다.
		if ((m_initPos.inpos.x == 0) && (m_initPos.inpos.y == 0) &&
			(m_initPos.inpos.z == 0) && (m_initPos.inpos.tx == 0) &&
			(m_initPos.inpos.ty == 0) && (m_initPos.inpos.tz == 0) &&
			(m_initPos.outpos.x == 0) && (m_initPos.outpos.y == 0) &&
			(m_initPos.outpos.z == 0) && (m_initPos.outpos.tx == 0) &&
			(m_initPos.outpos.ty == 0) && (m_initPos.outpos.tz == 0))
			return;


		//이동
		m_leftStg.AbsMove(m_leftStg.AXIS_X, m_initPos.inpos.x);
		m_rightStg.AbsMove(m_rightStg.AXIS_X, m_initPos.outpos.x);

		m_leftStg.AbsMove(m_leftStg.AXIS_Y, m_initPos.inpos.y);
		m_rightStg.AbsMove(m_rightStg.AXIS_Y, m_initPos.outpos.y);

		m_leftStg.AbsMove(m_leftStg.AXIS_TX, m_initPos.inpos.tx);
		m_rightStg.AbsMove(m_rightStg.AXIS_TX, m_initPos.outpos.tx);

		m_leftStg.AbsMove(m_leftStg.AXIS_TY, m_initPos.inpos.ty);
		m_rightStg.AbsMove(m_rightStg.AXIS_TY, m_initPos.outpos.ty);

		m_leftStg.AbsMove(m_leftStg.AXIS_TZ, m_initPos.inpos.tz);
		m_rightStg.AbsMove(m_rightStg.AXIS_TZ, m_initPos.outpos.tz);

		m_leftStg.AbsMove(m_leftStg.AXIS_Z, m_initPos.inpos.z);
		m_rightStg.AbsMove(m_rightStg.AXIS_Z, m_initPos.outpos.z);

		//완료 대기.
		while (m_rightStg.IsMovingOK())
		{

			if (m_stopFlag == true)
			{
				m_leftStg.StopMove();
				m_rightStg.StopMove();
				return;
			}

		}

	}




	/// <summary>
	/// Disable window except stop button.
	/// </summary>
	private void DisWndButStop()
	{
		grpOption.Enabled = false;
		grpList.Enabled = false;
		grpExecution.Enabled = false;
	}




	/// <summary>
	/// Enable window.
	/// </summary>
	private void EnableWnd()
	{
		grpOption.Enabled = true;
		grpList.Enabled = true;
		grpExecution.Enabled = true;
	}




	/// <summary>
	/// find current Fa position.
	/// </summary>
	/// <returns>port pos. if can't find, it returns -1</returns>
	private int FindCurFaPortPos()
	{

		int curPort = -1;
		double pwr = 0;
		try
		{
			for (int i = 0; i < m_mpm.portCnt; i++)
			{
				pwr = JeffOptics.mW2dBm(m_mpm.ReadPwr(i + 1));

				if (pwr >= m_opt.optPwrThre)
				{
					curPort = i + 1;
					break;
				}
			}
		}
		catch
		{
			curPort = -1;
		}


		return curPort;
	}




	/// <summary>
	/// List 출력!!
	/// </summary>
	public void DisplayList()
	{
        
		try
		{
			string[] cols = {"Port No.", "ref. exist"};
			string[] vals = new string[ cols.Length ];
			hgdvList.DeleteAllRows();
			hgdvList.HanDefaultSetting();
			hgdvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			hgdvList.ReadOnly = true;
			hgdvList.Font = new System.Drawing.Font("Source Code Pro", 8, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			hgdvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			hgdvList.AllowUserToOrderColumns = false;
			hgdvList.AllowUserToResizeRows = false;
			hgdvList.RowHeadersWidth = 25;
			hgdvList.SetColumns(ref cols);

			for( int i = 1 ; i <= m_mpm.portCnt ; i++)
			{
				vals[0] = Convert.ToString(i);

				string strExist = "";
				try
				{
					if( m_ref.RefPow(i) != null )
						strExist = "exist";
					else
						strExist = "empty";
				}
				catch
				{
					strExist = "empty";
				}
				vals[1] = strExist;

				hgdvList.Insert(ref cols, ref vals);
			}
				
			hgdvList.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
		}
		catch
		{
			//do nothing.
		}


	}




	/// <summary>
	/// 현재 ref.를 plot한다.
	/// polt current ref. data.
	/// </summary>
	/// <param name="_port">port no.</param>
	private void PlotCurRef(int _port)
	{

		try
		{

			lbPlotPortNo.Text = _port.ToString();

			//clear graph
			wfgCurRef.ClearData();
			

			//dBm 값으로 plot~~
			List<double>  refPowList = m_ref.RefPow(_port);
			double[] refPows = JeffOptics.mW2dBm(refPowList.ToArray());


			NationalInstruments.UI.WaveformPlot wfpPwr = null;
			wfpPwr = new NationalInstruments.UI.WaveformPlot();
			wfpPwr.LineColor = System.Drawing.Color.White;
			wfpPwr.DefaultStart = m_ref.startWave;
			wfpPwr.DefaultIncrement = m_ref.stepWave;
			wfpPwr.XAxis = wfgCurRef.Plots[0].XAxis;
			wfpPwr.YAxis = wfgCurRef.Plots[0].YAxis;
			wfpPwr.PlotY(JeffOptics.mW2dBm(refPowList.ToArray()));
			wfgCurRef.Plots.Add(wfpPwr);
			wfgCurRef.Refresh();

		}
		catch
		{
			//do nothing.
		}


	}




	/// <summary>
	/// New ref.를 plot한다.
	/// polt current ref. data.
	/// </summary>
	/// <param name="_port">port no.</param>
	private void PlotNewRef(int _port)
	{
		try
		{
			//clear graph
			wfgNewRef.ClearData();


			//dBm 값으로 plot~~
			List<double> refPowList = m_newRef.RefPow(_port);
			double[] refPows = JeffOptics.mW2dBm(refPowList.ToArray());

			NationalInstruments.UI.WaveformPlot wfpPwr = null;
			wfpPwr = new NationalInstruments.UI.WaveformPlot();
			wfpPwr.LineColor = System.Drawing.Color.Lime;
			wfpPwr.DefaultStart = m_newRef.startWave;
			wfpPwr.DefaultIncrement = m_newRef.stepWave;
			wfpPwr.XAxis = wfgNewRef.Plots[0].XAxis;
			wfpPwr.YAxis = wfgNewRef.Plots[0].YAxis;
			wfpPwr.PlotY(JeffOptics.mW2dBm(refPowList.ToArray()));
			wfgNewRef.Plots.Add(wfpPwr);
			wfgNewRef.Refresh();

		}
		catch
		{
			//do nothing.
		}


	}




	/// <summary>
	/// load init pos. from xml file.
	/// </summary>
	/// <param name="_filepath">config file path</param>
	/// <returns></returns>
	private initPos LoadInitPos(string _filepath)
	{

		initPos ret = new initPos();
		ret.inpos = new CStageAbsPos();
		ret.outpos = new CStageAbsPos();

		Cconfig conf = null; 

		try
		{

			conf = new Cconfig(_filepath);

			//in
			string strTemp = "";
			strTemp = conf.GetValue("INITPOS_IN_X");
			ret.inpos.x = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_IN_Y");
			ret.inpos.y = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_IN_Z");
			ret.inpos.z = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_IN_TX");
			ret.inpos.x = Convert.ToDouble(strTemp);
			ret.inpos.x = Math.Round(ret.inpos.tx, 4);

			strTemp = conf.GetValue("INITPOS_IN_TY");
			ret.inpos.y = Convert.ToDouble(strTemp);
			ret.inpos.y = Math.Round(ret.inpos.ty, 4);

			strTemp = conf.GetValue("INITPOS_IN_TZ");
			ret.inpos.z = Convert.ToDouble(strTemp);
			ret.inpos.z = Math.Round(ret.inpos.tz, 4);


			//out
			strTemp = conf.GetValue("INITPOS_OUT_X");
			ret.outpos.x = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_OUT_Y");
			ret.outpos.y = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_OUT_Z");
			ret.outpos.z = Convert.ToInt32(strTemp);

			strTemp = conf.GetValue("INITPOS_OUT_TX");
			ret.outpos.x = Convert.ToDouble(strTemp);
			ret.outpos.x = Math.Round(ret.outpos.tx, 4);

			strTemp = conf.GetValue("INITPOS_OUT_TY");
			ret.outpos.y = Convert.ToDouble(strTemp);
			ret.outpos.y = Math.Round(ret.outpos.ty, 4);

			strTemp = conf.GetValue("INITPOS_OUT_TZ");
			ret.outpos.z = Convert.ToDouble(strTemp);
			ret.outpos.z = Math.Round(ret.outpos.tz, 4);

		}
		catch
		{
			ret.inpos.x = 0;
			ret.inpos.y = 0;
			ret.inpos.z = 0;
			ret.inpos.tx = 0;
			ret.inpos.ty = 0;
			ret.inpos.tz = 0;

			ret.outpos.x = 0;
			ret.outpos.y = 0;
			ret.outpos.z = 0;
			ret.outpos.tx = 0;
			ret.outpos.ty = 0;
			ret.outpos.tz = 0;
		}


		if (conf != null)
			conf.Dispose();
		conf = null;

		return ret;

	}



    
	/// <summary>
	/// save options to xml file.
	/// </summary>
	/// <param name="_opt"></param>
	/// <param name="_filepath"></param>
	/// <returns></returns>
	private bool SaveInitPos(string _filepath)
	{
		bool ret = false;

		Cconfig conf = null;
		try

		{
			conf = new Cconfig(_filepath);

			//in
			conf.SetValue("INITPOS_IN_X", m_initPos.inpos.x.ToString());
			conf.SetValue("INITPOS_IN_Y", m_initPos.inpos.y.ToString());
			conf.SetValue("INITPOS_IN_Z", m_initPos.inpos.z.ToString());
			conf.SetValue("INITPOS_IN_TX", m_initPos.inpos.tx.ToString());
			conf.SetValue("INITPOS_IN_TY", m_initPos.inpos.ty.ToString());
			conf.SetValue("INITPOS_IN_TZ", m_initPos.inpos.tz.ToString());

			//out
			conf.SetValue("INITPOS_OUT_X", m_initPos.outpos.x.ToString());
			conf.SetValue("INITPOS_OUT_Y", m_initPos.outpos.y.ToString());
			conf.SetValue("INITPOS_OUT_Z", m_initPos.outpos.z.ToString());
			conf.SetValue("INITPOS_OUT_TX", m_initPos.outpos.tx.ToString());
			conf.SetValue("INITPOS_OUT_TY", m_initPos.outpos.ty.ToString());
			conf.SetValue("INITPOS_OUT_TZ", m_initPos.outpos.tz.ToString());

		   
			ret = true;
		}
		catch
		{
			ret = false;
		}


		if (conf != null)
			conf.Dispose();
		conf = null;



		return ret;
	}




	/// <summary>
	/// Init position을 출력한다.
	/// </summary>
	private void DisplayInitPos()
	{

		//in
		lbInitPosInX.Text = m_initPos.inpos.x.ToString();
		lbInitPosInY.Text = m_initPos.inpos.y.ToString();
		lbInitPosInZ.Text = m_initPos.inpos.z.ToString();
		lbInitPosInTx.Text = m_initPos.inpos.tx.ToString();
		lbInitPosInTy.Text = m_initPos.inpos.ty.ToString();
		lbInitPosInTz.Text = m_initPos.inpos.tz.ToString();

		//out
		lbInitPosOutX.Text = m_initPos.outpos.x.ToString();
		lbInitPosOutY.Text = m_initPos.outpos.y.ToString();
		lbInitPosOutZ.Text = m_initPos.outpos.z.ToString();
		lbInitPosOutTx.Text = m_initPos.outpos.tx.ToString();
		lbInitPosOutTy.Text = m_initPos.outpos.ty.ToString();
		lbInitPosOutTz.Text = m_initPos.outpos.tz.ToString();


	}



    
	/// <summary>
	/// load options from xml file.
	/// </summary>
	/// <param name="_filepath"></param>
	/// <returns></returns>
	private option LoadOptionFromXml(string _filepath)
	{

		option ret = new option();

		try
		{
			Cconfig conf = new Cconfig(_filepath);

			string strTemp = "";
			strTemp = conf.GetValue("OPTPWR_THRES");
			ret.optPwrThre = Convert.ToInt32(strTemp);
			strTemp = conf.GetValue("FA_COREPITCH");
			ret.corePitch = Convert.ToInt32(strTemp);
			strTemp = conf.GetValue("ALIGNWAVELENGTH");
			ret.alignWavelen = Convert.ToInt32(strTemp);

			conf.Dispose();
			conf= null;
		}
		catch
		{
			ret.optPwrThre = 0;
			ret.corePitch = 0;
		}

		return ret;

	}




	/// <summary>
	/// save options to xml file.
	/// </summary>
	/// <param name="_opt"></param>
	/// <param name="_filepath"></param>
	/// <returns></returns>
	private bool SaveOptionToXml(option _opt, string _filepath)
	{

		bool ret = false;

		try
		{
			Cconfig conf = new Cconfig(_filepath);
			conf.SetValue("OPTPWR_THRES", _opt.optPwrThre.ToString());
			conf.SetValue("FA_COREPITCH", _opt.corePitch.ToString());
			conf.SetValue("ALIGNWAVELENGTH", _opt.alignWavelen.ToString());
			conf.Dispose();
			conf = null;

			ret = true;
		}
		catch
		{
			ret = false;
		}

		return ret;

	}


	#endregion





	/// <summary>
	/// init 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void frmWdmRef_Load(object sender, EventArgs e)
	{

		//init member variables
		m_swSys = CGlobal.g_swSys;
		m_align = CGlobal.g_align;

		m_avc = CGlobal.g_avc;
		m_tls = CGlobal.g_tls;
		m_mpm = CGlobal.g_mpm;
		m_leftStg = (Istage)(CGlobal.g_leftStage);
		m_rightStg = (Istage)(CGlobal.g_rightStage);


		//ref.
		//m_ref = new CsweepSys.CswpRefNonpol();
		//m_ref.LoadFromTxt(Application.StartupPath + "\\refNonpol.txt");
		//DisplayList();
		//PlotCurRef(1);


		//load option && configs.
		string confFilepath = Application.StartupPath + "\\conf_ref.xml";
		this.Location = LoadWndStartPos(confFilepath);

		m_opt = LoadOptionFromXml(confFilepath);
		if ((m_opt.corePitch == 0) && (m_opt.optPwrThre == 0))
		{
			m_opt.optPwrThre = DEFAULT_OPTPWRTHRES;
			m_opt.corePitch = DEFAULT_FACOREPITCH;
			m_opt.alignWavelen = DEFAULT_ALIGNWAVELEN;
		}
		txtOptPwrThres.Text = Convert.ToString(m_opt.optPwrThre);
		txtFAcorepitch.Text = Convert.ToString(m_opt.corePitch);
		txtAlignWavelen.Text = Convert.ToString(m_opt.alignWavelen);


		m_initPos = LoadInitPos(confFilepath);
		DisplayInitPos();



		//excution
		cbStartPort.Items.Clear();
		cbStoptPort.Items.Clear();
		try
		{
			for (int i = 1; i <= m_mpm.portCnt; i++)
			{
				cbStartPort.Items.Insert(i - 1, Convert.ToString(i));
				cbStoptPort.Items.Insert(i - 1, Convert.ToString(i));
			}
		}
		catch
		{
			//do nothing.
		}


		int curFaPos = FindCurFaPortPos();
		if (curFaPos != -1)
			lbCurPort.Text = curFaPos.ToString();
		else
			lbCurPort.Text = "???";


		if (m_tls != null)
		{
			m_tls.SetTlsOutPwr(TLS_OUTPWR);
			Thread.Sleep(200);
			m_tls.SetTlsSweepSpeed(SWEEPSPEED);//added by Bae 2015-10-05
		}


		//fix output
		if(m_avc != null) m_avc.OpenValve(VALVE2);


		//start 가동.
		m_autoEvent = new AutoResetEvent(false);
		m_thread = new Thread(ThreadFunc);
		m_thread.Start();

		
	}



    
	/// <summary>
	/// terminates form.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void frmWdmRef_FormClosing(object sender, FormClosingEventArgs e)
	{

		//free output
		if(m_avc != null) m_avc.CloseValve(VALVE2);


		//optical powermeter setting.
		try
		{
			int alignPort = 5;
			if (Application.OpenForms.OfType<AlignForm>().Count() > 0)
			{
				AlignForm alignForm = (AlignForm)Application.OpenForms["frmAlignment"];
				alignPort = alignForm.alignPort;
			}

			
			if (Application.OpenForms.OfType<frmDigitalOptPowermeter>().Count() > 0)
			{
				frmDigitalOptPowermeter frm = null;
				frm = (frmDigitalOptPowermeter)Application.OpenForms["frmDigitalOptPowermeter"];
				frm.SetFirstCh(alignPort);
			}

		}
		catch
		{
			//do nothing.
		}

        

		//save config.
		string confFilepath = Application.StartupPath + "\\conf_ref.xml";
		SaveWndStartPos(confFilepath);

		if (!SaveOptionToXml(m_opt, confFilepath))
		{
			MessageBox.Show("설정값을 저장하는데 실패!!",
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
		}


		if (!SaveInitPos(confFilepath))
		{
			MessageBox.Show("Init position을 저장하는데 실패!!",
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
		}



		//thread 종료 및 마무리.
			if (m_thread != null)
		{
			m_thread.Abort();
			m_thread.Join();
			m_thread = null;
		}


		if (m_autoEvent != null)
			m_autoEvent.Dispose();
		m_autoEvent = null;


		m_tls = null;
		m_mpm = null;
		m_leftStg = null;
		m_rightStg = null;

		m_swSys = null;
		m_align = null;

	}




    /// <summary>
    /// 옵션을 적용한다
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void btnOptApply_Click(object sender, EventArgs e)
	{
		try
		{

			m_opt.optPwrThre = Convert.ToInt32(txtOptPwrThres.Text);
			m_opt.corePitch = Convert.ToInt32(txtFAcorepitch.Text);
			m_opt.alignWavelen = Convert.ToInt32(txtAlignWavelen.Text);


			if (false == SaveOptionToXml(m_opt, Application.StartupPath + "\\conf_ref.xml"))
				throw new ApplicationException();
			
 
			MessageBox.Show("옵션이 설정되었습니다.",
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
		}
		catch
		{
			MessageBox.Show("옵션을 설정하는데 실패하였습니다.",
							"에러",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
		}

		
	}




    /// <summary>
    /// 옵션 취소
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void btnOptCancel_Click(object sender, EventArgs e)
	{
		txtOptPwrThres.Text = Convert.ToString(m_opt.optPwrThre);
		txtFAcorepitch.Text = Convert.ToString(m_opt.corePitch);
	}




    /// <summary>
    /// list 선택
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void hgdvList_CellClick(object sender, DataGridViewCellEventArgs e)
	{

		try
		{
			int portNo  = Convert.ToInt32(hgdvList.CurrentRow.Cells[0].Value);
			PlotCurRef(portNo);
			PlotNewRef(portNo);

			cbStartPort.Text = portNo.ToString();

		}
		catch
		{
			//do nothing.
		}

	 
	}




	/// <summary>
	/// move in-FA to out-FA port. position
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void GoPort_Click(object sender, EventArgs e)
	{

		if (m_isRuning == true )
			return;


		string msg = "port" + cbStartPort.Text + "로 이동하시겠습니까?";
		DialogResult res;
		res = MessageBox.Show(msg,
						      "확인",
						      MessageBoxButtons.OKCancel,
						      MessageBoxIcon.Question);
		if (res == DialogResult.Cancel)
			return;

		
		try
		{
			//optical powermeter setting.
			try
			{
				frmDigitalOptPowermeter frm = null;
				frm = (frmDigitalOptPowermeter)Application.OpenForms["frmDigitalOptPowermeter"];
				frm.SetFirstCh(Convert.ToInt32(cbStartPort.Text));
			}
			catch
			{
				//do nothing.
			}
			

			DisWndButStop();


			//start..
			m_tp.cmd = CMD_GOPORT;
			m_tp.startPort = Convert.ToInt32(cbStartPort.Text);

			m_autoEvent.Set();
			Thread.Sleep(10);

		}
		catch(Exception ex)
		{
			EnableWnd();
			MessageBox.Show(ex.ToString());
		}

	}




	/// <summary>
	/// start single-Ref.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnNewStart_single_Click(object sender, EventArgs e)
	{

		if (m_isRuning == true)
			return;

		//confirm?
		string msg = "port" + cbStartPort.Text + " 레퍼런스 작업을 진행하시겠습니까?";
		DialogResult res;
		res = MessageBox.Show(msg,
							  "확인",
							  MessageBoxButtons.OKCancel,
							  MessageBoxIcon.Question);
		if (res == DialogResult.Cancel)
			return;


		try
		{
			//optical powermeter setting.
			try
			{
				frmDigitalOptPowermeter frm = null;
				frm = (frmDigitalOptPowermeter)Application.OpenForms["frmDigitalOptPowermeter"];
				frm.SetFirstCh(Convert.ToInt32(cbStartPort.Text));
			}
			catch
			{
				//do nothing.
			}


			DisWndButStop();

			//parameter
			m_tp.cmd = CMD_SINGLEPORT;
			m_tp.startPort = Convert.ToInt32(cbStartPort.Text);
			m_tp.align = chkAlign.Checked;

			//exec.
			m_autoEvent.Set();
			Thread.Sleep(10);

		}
		catch (Exception ex)
		{
			EnableWnd();
			MessageBox.Show(ex.ToString());
		}



	}




    /// <summary>
    /// 1개 적용
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
	private void btnNewApply_single_Click(object sender, EventArgs e)
	{
		//port no.
		int port = 0;
		try
		{
			port = Convert.ToInt32(cbStartPort.Text);
		}
		catch
		{
			string msg = "port no.가 이상합니다.";
			DialogResult res;
			res = MessageBox.Show(msg,
								  "에러",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning);
			return;
		}



		//data 존재 유무 판단.
		try
		{
			if ((m_newRef == null) || (m_newRef.portCnt == 0))
				throw new ApplicationException();
		}
		catch
		{
			string msg = "레퍼런스데이터가 없습니다.";
			DialogResult res;
			res = MessageBox.Show(msg,
								  "확인",
								  MessageBoxButtons.OK,
								  MessageBoxIcon.Warning);
			return;
		}



		
		//new ref. data를 구한다. 
		int startWaveNew = m_newRef.startWave;
		int stopWaveNew = m_newRef.stopWave;
		double stepWaveNew = Math.Round(m_newRef.stepWave, 3);
		SweepLogic.PortPowerPair pwrRefData = null;
		pwrRefData = m_newRef.GetPortData(port);
		if (pwrRefData == null)
		{
			string msg ="port" + port.ToString() + " 레퍼런스데이터가 없습니다.";
			MessageBox.Show(msg,
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);
			return;
		}



		//apply
		int startWaveCur = m_ref.startWave;
		int stopWaveCur = m_ref.stopWave;
		double stepWaveCur = Math.Round(m_ref.stepWave, 3);

		if ((startWaveNew != startWaveCur) ||
			(stopWaveNew != stopWaveCur) ||
			(stepWaveNew != stepWaveCur))
		{
			string msg = "기존 데이터와 Sweep parameter가 다릅니다.\n";
			msg += "기존 데이터를 모두 지우고 계속 진행할까요?";
			DialogResult res;
			res = MessageBox.Show(msg, "확인",  MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (res == DialogResult.No) return;

			m_ref.Clear();
		}



		if (m_ref.portCnt == 0)
			m_ref.SetWavelength(startWaveNew, stopWaveNew, stepWaveNew);

		m_ref.SetPortData(pwrRefData);
		m_newRef.DeletePortData(port);




		//save to file.
		//if( !m_ref.SaveToTxt(Application.StartupPath + "\\refNonpol.txt" ))
		if ( !m_ref.SaveToTxt(CGlobal.g_refPath))
		{
			string msg = "레퍼런스 데이터를 저장하는데 실패!!";
			MessageBox.Show(msg, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			return;
		}



		//완료처리.
		PlotCurRef(port);
		wfgNewRef.ClearData();
		DisplayList();

		MessageBox.Show("데이터 적용 완료!!", "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);

	}




	/// <summary>
	/// 초기 위치로 이동
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnInitPosGo_Click(object sender, EventArgs e)
	{

		if (m_isRuning == true)
			return;


		//confirm.
		string msg = "스테이지를 레퍼런스 초기 위치로 이동하시겠습니까?";
		DialogResult res;
		res = MessageBox.Show(msg,
							"확인",
							MessageBoxButtons.OKCancel,
							MessageBoxIcon.Question);
		if (res == DialogResult.Cancel)
			return;



		//포지션이 설정되지 않았으면 이동하지 않는다.
		if ((m_initPos.inpos.x == 0) && (m_initPos.inpos.y == 0) &&
			(m_initPos.inpos.z == 0) && (m_initPos.inpos.tx == 0) &&
			(m_initPos.inpos.ty == 0) && (m_initPos.inpos.tz == 0) &&
			(m_initPos.outpos.x == 0) && (m_initPos.outpos.y == 0) &&
			(m_initPos.outpos.z == 0) && (m_initPos.outpos.tx == 0) &&
			(m_initPos.outpos.ty == 0) && (m_initPos.outpos.tz == 0))
		{

			MessageBox.Show("위치값이 설정되지 않았습니다",
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);

			return;
		}




		//execution.
		try
		{

			DisWndButStop();

			//Go
			m_tp.cmd = CMD_GOINITPOS;
			m_autoEvent.Set();
			Thread.Sleep(10);


		}
		catch (Exception ex)
		{
			EnableWnd();
			MessageBox.Show(ex.ToString());
		}




	}




	/// <summary>
	/// 현재 위치를 초기위치로 설정.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void btnInitPosApply_Click(object sender, EventArgs e)
	{

		string msg = "현재 스테이지 포지션을 초기위치로 설정하시겠습니까?";
		DialogResult res = MessageBox.Show(msg,
										   "확인",
										   MessageBoxButtons.YesNo,
										   MessageBoxIcon.Question);
		if (res == DialogResult.No)
			return;


		//설정.
		m_initPos.inpos = m_leftStg.GetAbsPositions();
		m_initPos.outpos = m_rightStg.GetAbsPositions();


		//저장.
		string confFilepath = Application.StartupPath + "\\conf_ref.xml";
		if (!SaveInitPos(confFilepath))
		{
			MessageBox.Show("Init position을 저장하는데 실패!!",
							"확인",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
			return;
		}

		//출력 
		DisplayInitPos();
		MessageBox.Show("설정 완료!!", "확인");

	}

  
}


