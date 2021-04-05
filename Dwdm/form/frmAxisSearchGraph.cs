using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Jeffsoft;



public partial class frmAxisSearchGraph : Form//,IFormCanClosed
{


    #region definition

    private const  int UNIT_MW = 0;
	private const  int UNIT_DBM = 1;

    #endregion




    #region Private member variables

    private int m_nUnit;

    private double[] m_dbPowrArr_mW;
    private double[] m_dbPowrArr_dBm;
    private double[] m_dbPosArr;

    private NationalInstruments.UI.XYCursor m_pCurosor;


    #endregion




    #region constructor/destructor

    /// <summary>
    /// 생성자.
    /// </summary>
    public frmAxisSearchGraph()
    {
        InitializeComponent();
    }

    #endregion




    #region private method




    /// <summary>
    /// mW단위로 데이터 plot.
    /// </summary>
    /// <param name="_startPos">start wavelength </param>
    /// <param name="_step">wavelength step</param>
    /// <param name="_datas"> [mW]</param>
    private void PlotMW(double _startPos, double _step, double[] _datas)
    {

        if (_datas == null)
            return;


        //plot
        wfgSearch.Plots[0].DefaultStart = Math.Round(_startPos, 3);
        wfgSearch.Plots[0].DefaultIncrement = Math.Round(Math.Abs(_step), 3);
        wfgSearch.Plots[0].PlotY(_datas);
        wfgSearch.Refresh();
        wfgSearch.YAxes[0].Caption = "[mW]";


        //Peak 위치를 찾는다.
        int nPeakIndex = 0;
        double dbMaxPow = _datas.Max();
        nPeakIndex = Array.IndexOf(_datas, dbMaxPow);


        //커서를 만들어서 붙인다.
        if (m_pCurosor != null)
        {
            m_pCurosor.Dispose();
            m_pCurosor = null;
        }

        NationalInstruments.UI.WaveformPlot pPlot = wfgSearch.Plots[0];
        m_pCurosor = new NationalInstruments.UI.XYCursor(pPlot);
        m_pCurosor.XPosition = m_dbPosArr[nPeakIndex];              //위치는 peak!!
        m_pCurosor.Color = Color.Aqua;
        m_pCurosor.LabelForeColor = Color.Aqua;
        m_pCurosor.LabelVisible = true;
        m_pCurosor.Visible = true;

        wfgSearch.Cursors.Add(m_pCurosor);
        wfgSearch.Refresh();

    }


    /// <summary>
    /// dBm단위로 데이터 plot.
    /// </summary>
    /// <param name="_startPos"></param>
    /// <param name="_step"></param>
    /// <param name="_datas"></param>
    private void PlotDBm(double _startPos, double _step, double[] _datas)
    {

        if (_datas == null)
            return;


        //plot
        wfgSearch.Plots[0].DefaultStart = Math.Round(_startPos, 3);
        wfgSearch.Plots[0].DefaultIncrement = Math.Round(Math.Abs(_step), 3);
        wfgSearch.Plots[0].PlotY(_datas);
        wfgSearch.Refresh();

        wfgSearch.YAxes[0].Caption = "[dBm]";


        //Peak 위치를 찾는다.
        int nPeakIndex = 0;
        double dbMaxPow = _datas.Max();
        nPeakIndex = Array.IndexOf(_datas, dbMaxPow);


        //커서를 만들어서 붙인다.
        if (m_pCurosor != null)
        {
            m_pCurosor.Dispose();
            m_pCurosor = null;
        }

        NationalInstruments.UI.WaveformPlot pPlot = wfgSearch.Plots[0];
        m_pCurosor = new NationalInstruments.UI.XYCursor(pPlot);
        m_pCurosor.XPosition = m_dbPosArr[nPeakIndex];              //위치는 peak!!
        m_pCurosor.Color = Color.Aqua;
        m_pCurosor.LabelForeColor = Color.Aqua;
        m_pCurosor.LabelVisible = true;
        m_pCurosor.Visible = true;

        wfgSearch.Cursors.Add(m_pCurosor);
        wfgSearch.Refresh();


    }







    /// <summary>
    /// data를 파일에 저장.
    /// </summary>
    /// <param name="strFilePath"> 파일경로 </param>
    /// <param name="dbPosArr">position data array.</param>
    /// <param name="dbPowArr_mW">optical power data array [mW]</param>
    /// <returns></returns>
    private bool SaveData(string strFilePath, double[] dbPosArr, double[] dbPowArr_mW)
    {


        //Variables..
        bool bRet = true;
        string strLineBuf = "";


        FileStream fs = null;
        StreamWriter sw = null;


        try
        {

            //File Open
            fs = new FileStream(strFilePath, FileMode.Open);
            sw = new StreamWriter(fs);

            //colum
            strLineBuf = "Position  Power[mW]";
            sw.WriteLine(strLineBuf);


            //data
            for (int i = 0; i < dbPosArr.Length ; i++)
            {
                strLineBuf = Convert.ToString(dbPosArr[i]) + '\t' + Convert.ToString(dbPowArr_mW[i]);
                sw.WriteLine(strLineBuf);
            }


            //[END_OF_FILE]
            strLineBuf = "[END_OF_FILE]";
            sw.WriteLine(strLineBuf);


            bRet = true;

        }
        catch 
        {
            bRet = false;
        }
        finally
        {
            //File close
            if (sw != null)
                sw.Close();

            if (fs != null)
                fs.Close();
        }


        return bRet;

    }



        
    /// <summary>
    /// plot data.
    /// </summary>
    /// <param name="_startPos">start position</param>
    /// <param name="_step"> moving step</param>
    /// <param name="_pwrArr">power data array [mW]</param>
    public void Plot(double _startPos, double _step, double[] _pwrArr)
    {

        if ( _pwrArr != null )
        {

            m_dbPowrArr_mW = _pwrArr;
            m_dbPowrArr_dBm = JeffOptics.mW2dBm(m_dbPowrArr_mW);

            m_dbPosArr = new double[m_dbPowrArr_mW.Length];
            for (int i = 0; i < m_dbPowrArr_mW.Length; i++)
            {
                if( i == 0 )
                {
                    m_dbPosArr[i] = Math.Round(_startPos, 3);
                }
                else
                {
                    m_dbPosArr[i] += _step;
                    m_dbPosArr[i] = Math.Round(m_dbPosArr[i], 3);
                }
            }
                
        }
        else
        {
            return;
        }


        PlotMW(_startPos, _step, _pwrArr);

    }




        
    #endregion





    /// <summary>
    /// 폼을 초기화 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void frmAxisSearchGraph_Load(object sender, EventArgs e)
    {
        //멤버변수 초기화.
        m_nUnit = UNIT_MW;
        wfgSearch.YAxes[0].Caption = "[mW]";
    }



    /// <summary>
    /// 폼을 마무리 한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;
        
        m_dbPowrArr_mW = null;
        m_dbPowrArr_dBm = null;
        m_dbPosArr = null;
    }
    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }



    /// <summary>
    /// 폼을 닫는다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }



    /// <summary>
    /// Unit 변경
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnUnit_Click(object sender, EventArgs e)
    {

        double startWave =  Math.Round( m_dbPosArr[0] , 3);
        double stepWavelen =  Math.Round( m_dbPosArr[1]-  m_dbPosArr[0], 3);

        if (m_nUnit == UNIT_MW)
        {
            //mW->dBm
            PlotDBm(startWave, stepWavelen, m_dbPowrArr_mW );
            btnUnit.Text = "UNIT dBm";
            m_nUnit = UNIT_DBM;
        }
        else
        {
            //dBm->mW
            PlotMW(startWave, stepWavelen, m_dbPowrArr_dBm);
            btnUnit.Text = "UNIT mW";
            m_nUnit = UNIT_MW;
        }

    }


    /// <summary>
    /// cursor on,off
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCursor_Click(object sender, EventArgs e)
    {


        if (m_pCurosor.Visible == true)
        {
            //on->off//
            try
            {
                m_pCurosor.Visible = false;
                btnCursor.Text = "CURSOR OFF";
            }
            catch { /* do nothing */ }
                
        }
        else
        {
            //off->on//
            try
            {
                m_pCurosor.Visible = true;
                btnCursor.Text = "CURSOR ON";
            }
            catch { /* do nothing */ }
        }
      

    }



    /// <summary>
    /// move cursor to peak.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnPeak_Click(object sender, EventArgs e)
    {

        //peak 검색.
        double maxVal = m_dbPowrArr_mW.Max();
        int maxIdx = Array.IndexOf(m_dbPowrArr_mW, maxVal);

      
        //이동.
        if (m_pCurosor != null)
        {
            m_pCurosor.XPosition = m_dbPosArr[maxIdx];
            wfgSearch.Refresh();
        }


    }


    /// <summary>
    /// 데이터 저장.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSave_Click(object sender, EventArgs e)
    {

        if (m_dbPowrArr_mW == null)
        {
            MessageBox.Show("저장할 데이터가 없음", "헉",MessageBoxButtons.OK,MessageBoxIcon.Error);
            return;
        }



        // file 선택!!
        string strFilePath = "";
        System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
        sfd.Filter = "Text files (*.txt)|*.txt";
        sfd.ShowDialog();
        strFilePath = sfd.FileName;
        sfd = null;

        if (string.IsNullOrEmpty(strFilePath))
            return;

            
        //save data.
        if (SaveData( strFilePath, m_dbPosArr, m_dbPowrArr_mW) == true)
            MessageBox.Show("파일 저장 성공", "확인", MessageBoxButtons.OK);
        else
            MessageBox.Show("파일 저장 실패!!", "헉", MessageBoxButtons.OK, MessageBoxIcon.Error);
           
    }


}


