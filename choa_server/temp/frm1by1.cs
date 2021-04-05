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



public partial class frm1by1 : Form
{


    #region structure/innor class

    //private struct threadParam
    //{
    //    public string[] chipNos;
    //    public int gains;       //number of gains. 
    //    public List<int> gainList;    
    //    public int inPitch;     //input pitch
    //    public int outPitch;    //output FA corepitch [um]     
    //    public int detectPort;  //SMF or MMF
    //    public int direction;   //measurement direction

    //    public bool autoSave;
    //    public int autoSaveType;    //full or range.
    //    public int saveRngStart;    //save range start wavelengh.
    //    public int saveRngStop;     //save range stop wavelengh.

    //    public bool alignment;  //alignment. <-- uncheck하면 1칩만 측정됨.
    //    public bool measurement;  //measurement.?
    //    public bool faArrangement;  //FA arrangement?

    //    public string saveFolderPath;
    //}



    #endregion





    #region member variables

    //private CsweepSys.CswpRefNonpol m_ref;

    //private Itls m_tls;
    //private IoptMultimeter m_mpm;
    //private Istage m_leftStg;
    //private Istage m_rightStg;

    //private CsweepSys m_swSys;
    //private IAlignment m_align;


    //bool m_stopFlag;
    //bool m_isRuning; //running:true , stop :false
    //private threadParam m_tp;
    //private AutoResetEvent m_autoEvent;
    //private Thread m_thread;

    //private CprogRes m_procState;
    //private List<Cmeasure> m_msrList;

    //private JeffSoundPlayer m_jPlayer;


    #endregion





    #region constructor/destructor

    public frm1by1()
    {
        InitializeComponent();
    }

    #endregion





    private void frm1by1_Load(object sender, EventArgs e)
    {


    }




    /// <summary>
    /// save folder path를 선택한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnSaveFolder_Click(object sender, EventArgs e)
    {
        System.Windows.Forms.FolderBrowserDialog fd = new System.Windows.Forms.FolderBrowserDialog();
        if (fd.ShowDialog() == DialogResult.OK)
            lbSaveFolderPath.Text = fd.SelectedPath;
    }


}
