using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Neon.Aligner;
using Jeffsoft;



public partial class frmStageControl : Form//, IFormCanClosed
{


    #region definition


    //command
    public enum CMD { NOOPERATION, RELMOVE, APPROACH, HOMING, ZEROING, ZEROING_ALL };


    //speed
    public const int SPEED_LOW = 0;
    public const int SPEED_MID = 1;
    public const int SPEED_HIGH = 2;
    public const int SPEED_MIDHIGH = 3;


    #endregion




    #region structure


    private struct StageContorlParam
    {
        public bool bMoveStop;
        public bool bIsMoving;

        public CMD nCommand;                //명령
        public int nStage;                  //Stage, ( Left Stage or Right Stage)
        public int nAxis;                   //Axis
        public double dbDistance;           //Distance       
    }



    private struct ChipProtectSys
    {
        public bool enable;                 //enable or disable
        public double thres;                //threshold distance.  [V]
    }

    #endregion

    


    #region Delegate

    delegate void fpWindow();
    delegate void fpWindowStage(int nAxis);
    delegate void fpDisplayStagePositions(int nStage);
    delegate void fpDispalyAxisPosition(int nStageNo, int nAxis);

    #endregion

    

    
    #region private member variables.

    private Istage m_leftStage;
    private Istage m_rightStage;
    private Istage m_othStage;

    private Dictionary<int, Istage> mStage;

    private IDispSensor m_distSens;

    private StageContorlParam m_scp;
    private ChipProtectSys m_cps;

    private AutoResetEvent m_pAutoEvent;
    private Thread m_thread;

    #endregion




    #region constructor/desconstructor


    public frmStageControl()
    {
        InitializeComponent();
    }



    /// <summary>
    /// 폼을 초기화 한다.!!
    /// </summary>
    private void FrmStageControl_Load(object sender, EventArgs e)
    {

        try
        {

            //멤버 변수 설정
            m_leftStage = CGlobal.LeftAligner;
            //CGlobal.LeftAligner.CoordUpdate += leftAligner_CoordUpdate;
            m_rightStage = CGlobal.RightAligner;
            //CGlobal.RightAligner.CoordUpdate += rightAligner_CoordUpdate;
            m_othStage = CGlobal.OtherAligner;
            m_distSens = CGlobal.Ds2000;

            mStage = new Dictionary<int, Istage>();
            if(m_leftStage!=null) mStage.Add(m_leftStage.stageNo, m_leftStage);
			if (m_rightStage != null) mStage.Add(m_rightStage.stageNo, m_rightStage);
			if (m_othStage != null) mStage.Add(m_othStage.stageNo, m_othStage);

            m_pAutoEvent = new AutoResetEvent(false);

            //load setting & display
            string confFilepath = Application.StartupPath + "\\conf_StageControl3stg.xml";
            this.Location = LoadWndStartPos(confFilepath);
            loadDistance(confFilepath);                     //이동 거리 설정값 불러오기.
            

            //Current position 출력
            DisplayCurPositions();


            //Thread를 동작 시킨다.!!
            m_scp.nCommand = CMD.NOOPERATION;
            m_thread = new Thread(ThreadFunc);
            m_thread.Start();


            this.Location = LoadWndStartPos(confFilepath);

        }
        catch
        {
            //do nothing.
        }


    }
   


    /// <summary>
    /// 폼을 마무리한다.
    /// </summary>
    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!Program.CanIBeClosed(e)) return;

        //CGlobal.LeftAligner.CoordUpdate -= leftAligner_CoordUpdate;
        //CGlobal.RightAligner.CoordUpdate -= rightAligner_CoordUpdate;

        //param 저장.
        string confFilepath = Application.StartupPath + "\\conf_StageControl3stg.xml";
        SaveWndStartPos(confFilepath);
        saveDistance(confFilepath);


        //쓰레드 종료!!
        if (m_thread != null)
        {
            m_thread.Abort();
            m_thread.Join();
            m_thread = null;
        }

        if (m_pAutoEvent != null)
            m_pAutoEvent.Dispose();
        m_pAutoEvent = null;


        m_leftStage = null;
        m_rightStage = null;
        m_distSens = null;



    }

    

    public bool CanIBeClosed(object param)
    {
        //if (!CanIBeClosed(e)) return;
        ((FormClosingEventArgs)param).Cancel = !Program.AppicationBeingQuit;
        return Program.AppicationBeingQuit;
    }
    

    #endregion




    #region Thread Function



    private void ThreadFunc()
    {
        while (true)
        {
            //명령을 기다린다.
            m_scp.bIsMoving = false;
            m_scp.bMoveStop = false;
            m_pAutoEvent.WaitOne();
            m_scp.bIsMoving = true;

            Invoke((Action)DisableWindowButStop);

            switch (m_scp.nCommand)
            {
                case CMD.RELMOVE:

                    //이동
                    AsyncRelMove(m_scp.nStage, m_scp.nAxis, m_scp.dbDistance);

                    while (true)
                    {
                        //정지 버튼이 눌려지면.. 동작을 멈춘다.
                        if (m_scp.bMoveStop == true)
                        {
                            StopMove(m_scp.nStage, m_scp.nAxis);
                            Invoke((Action)EnableWindowAll);
                            break;
                        }

                        //이동이 완료될때까지 기다린다.
                        if (InMotion(m_scp.nStage, m_scp.nAxis) == false) break;

                        //혹시 컨택하게되면 바로 멈추고 뒤로 500um 이동!!
                        //if (m_cps.enable == true)
                        //{
                        //    if ((m_scp.nAxis == m_leftStage.AXIS_Z) && (m_scp.dbDistance > 0))
                        //    {
                        //        double sensVal = 0;
                        //        if (m_scp.nStage == m_leftStage.stageNo) sensVal = m_distSens.ReadDist(SensorID.Left);
                        //        else sensVal = m_distSens.ReadDist(SensorID.Right);

                        //        if (sensVal <= m_cps.thres)
                        //        {
                        //            StopMove(m_scp.nStage, m_scp.nAxis);
                        //            AsyncRelMove(m_scp.nStage, m_scp.nAxis, -500);
                        //            Invoke((Action)EnableWindowAll);
                        //        }
                        //    }
                        //}//if enable                        
                    }//while

                    //Position을 출력하고 폼을 활성화 시킨다.
                    Invoke((Action)EnableWindowAll);
                    Invoke((Action)(() => DisplayAxisPosition(m_scp.nStage, m_scp.nAxis)));

                    break;

                case CMD.HOMING:

                    //Homing
                    HommingStage(m_scp.nStage);

                    //완료될때까지 기다림
                    while (true)
                    {
                        //정지 버튼이 눌려지면.. 동작을 멈춘다.
                        if (m_scp.bMoveStop == true)
                        {
                            StopMove(m_scp.nStage);
                            Invoke((Action)EnableWindowAll);
                            break;
                        }

                        //이동이 완료될때까지 기다린다.
                        if (InMotion(m_scp.nStage) == false) break;

                    }

                    //Position을 출력하고 폼을 활성화 시킨다.
                    Invoke((Action)(() => DisplayStageCurPositions(m_scp.nStage)));
                    Invoke((Action)EnableWindowAll);

                    break;
            }



        } //while (true) 



    }


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

            XConfig conf = new XConfig(_filePath);


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
        XConfig conf = null;

        string temp = "";
        try
        {
            conf = new XConfig(_filePath);


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
    /// 각 축의 이동거리 기존 설정값을 불러온다.
    /// </summary>
    /// <param name="confFilepath"></param>
    private void loadDistance(string confFilepath)
    {
        try
        {
            XConfig config = null;
            config = new XConfig(confFilepath);
            txtLeftXCurDist.Text = config.GetValue("[DIST_X_LEFT]");
            txtLeftYCurDist.Text = config.GetValue("[DIST_Y_LEFT]");
            txtLeftZCurDist.Text = config.GetValue("[DIST_Z_LEFT]");
            txtLeftThetaXCurDist.Text = config.GetValue("[DIST_TX_LEFT]");
            txtLeftThetaYCurDist.Text = config.GetValue("[DIST_TY_LEFT]");
            txtLeftThetaZCurDist.Text = config.GetValue("[DIST_TZ_LEFT]");
            txtRightXCurDist.Text = config.GetValue("[DIST_X_RIGHT]");
            txtRightYCurDist.Text = config.GetValue("[DIST_Y_RIGHT]");
            txtRightZCurDist.Text = config.GetValue("[DIST_Z_RIGHT]");
            txtRightThetaXCurDist.Text = config.GetValue("[DIST_TX_RIGHT]");
            txtRightThetaYCurDist.Text = config.GetValue("[DIST_TY_RIGHT]");
            txtRightThetaZCurDist.Text = config.GetValue("[DIST_TZ_RIGHT]");
            txtCenterDistanceX.Text = config.GetValue("[DIST_X_OTHER]");
            txtCenterDistanceY.Text = config.GetValue("[DIST_Y_OTHER]");

            if (config.GetValue("[CPS_ENABLE]") == "1")
                m_cps.enable = true;
            else
                m_cps.enable = false;
            m_cps.thres = Convert.ToDouble(config.GetValue("[CPS_THRES]"));
            m_cps.thres = Math.Round(m_cps.thres, 3);

            chkEnableCps.Checked = m_cps.enable;
            txtCpsThres.Text = Convert.ToString(m_cps.thres);

            config.Dispose();
            config = null;
        }
        catch
        {
            MessageBox.Show("설정값을 불러오는데 실패...",
                            "에러",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            txtLeftXCurDist.Text = "0.0";
            txtLeftYCurDist.Text = "0.0";
            txtLeftZCurDist.Text = "0.0";
            txtLeftThetaXCurDist.Text = "0.0";
            txtLeftThetaYCurDist.Text = "0.0";
            txtLeftThetaZCurDist.Text = "0.0";
            txtRightXCurDist.Text = "0.0";
            txtRightYCurDist.Text = "0.0";
            txtRightZCurDist.Text = "0.0";
            txtRightThetaXCurDist.Text = "0.0";
            txtRightThetaYCurDist.Text = "0.0";
            txtRightThetaZCurDist.Text = "0.0";

            m_cps.enable = true;
            m_cps.thres = 3.0;

            chkEnableCps.Checked = m_cps.enable;
            txtCpsThres.Text = Convert.ToString(m_cps.thres);
        }
    }



    /// <summary>
    /// 각 축의 이동거리 설정값을 저장한다.
    /// </summary>
    /// <param name="confFilepath"></param>
    private void saveDistance(string confFilepath)
    {
        XConfig config = null;
        try
        {
            config = new XConfig(confFilepath);

            config.SetValue("DIST_X_LEFT", txtLeftXCurDist.Text);
            config.SetValue("DIST_Y_LEFT", txtLeftYCurDist.Text);
            config.SetValue("DIST_Z_LEFT", txtLeftZCurDist.Text);
            config.SetValue("DIST_TX_LEFT", txtLeftThetaXCurDist.Text);
            config.SetValue("DIST_TY_LEFT", txtLeftThetaYCurDist.Text);
            config.SetValue("DIST_TZ_LEFT", txtLeftThetaZCurDist.Text);

            config.SetValue("DIST_X_RIGHT", txtRightXCurDist.Text);
            config.SetValue("DIST_Y_RIGHT", txtRightYCurDist.Text);
            config.SetValue("DIST_Z_RIGHT", txtRightZCurDist.Text);
            config.SetValue("DIST_TX_RIGHT", txtRightThetaXCurDist.Text);
            config.SetValue("DIST_TY_RIGHT", txtRightThetaYCurDist.Text);
            config.SetValue("DIST_TZ_RIGHT", txtRightThetaZCurDist.Text);

            config.SetValue("DIST_X_OTHER", txtCenterDistanceX.Text);
            config.SetValue("DIST_Y_OTHER", txtCenterDistanceY.Text);



            if (m_cps.enable == true)
                config.SetValue("CPS_ENABLE", "1");
            else
                config.SetValue("CPS_ENABLE", "0");

            config.SetValue("CPS_THRES", m_cps.thres.ToString());




            config.Dispose();
            config = null;
        }
        catch
        {
            if (config != null)
                config.Dispose();
            config = null;
        }
    }



    private void AsyncRelMove(int _stageNo, int _axis, double _dist)
    {
        try { mStage[_stageNo].RelMove(_axis, _dist); } catch { /*do nothing*/ }
    }
    private void HommingStage(int _stageNo)
    {
        try { mStage[_stageNo].Homing(); } catch { /*do nothing*/ }
    }
    private void StopMove(int _stageNo)
    {
        try { mStage[_stageNo].StopMove(); } catch { /*do nothing*/ }
    }
    private void StopMove(int _stageNo, int _axis)
    {
        try { mStage[_stageNo].StopMove(_axis); } catch { /*do nothing*/ }
    }

    private bool InMotion(int _stageNo, int _axis)
    {
        try { return mStage[_stageNo].IsMovingOK(_axis); } catch { return false; }
    }

    private bool InMotion(int _stageNo)
    {
        try { return mStage[_stageNo].IsMovingOK(); } catch { return false; }
    }

    

    /// <summary>
    /// 모든 화면을  Disable시킨다.
    /// </summary>
    private void DisableWindowAll()
    {
        this.Enabled = false;
    }


    
    /// <summary>
    /// Stop만 빼고 화면을 Disable시킨다.
    /// </summary>
    private void DisableWindowButStop()
    {

        grpLeftStage.Enabled = false;
        grpRightStage.Enabled = false;
        grpOtherStage.Enabled = false;
        grpCps.Enabled = false;
        btnStop.Enabled = true;

        grpLeftStage.Refresh();
        grpRightStage.Refresh();
        grpOtherStage.Refresh();
        grpCps.Refresh();

        btnStop.Focus();

    }
    


    /// <summary>
    /// 모든 화면을  Enable시킨다.
    /// </summary>
    private void EnableWindowAll()
    {

        this.Enabled = true;

        grpLeftStage.Enabled = true;
        grpRightStage.Enabled = true;
        grpOtherStage.Enabled = true;
        grpCps.Enabled = true;

        btnStop.Enabled = true;

        grpLeftStage.Refresh();
        grpRightStage.Refresh();
        grpOtherStage.Refresh();
        grpCps.Refresh();

        btnStop.Refresh();

    }

    

    /// <summary>
    /// Stage를 group을 Disable 한다.
    /// </summary>
    /// <param name="_stageNo"></param>
    private void DisableWindowStage(int _stageNo)
    {

        GroupBox gb = null;

        if (_stageNo == m_leftStage.stageNo)
            gb = grpLeftStage;
        else if (_stageNo == m_rightStage.stageNo)
            gb = grpRightStage;


        gb.Enabled = false;
        gb.Update();

    }
    


    /// <summary>
    /// 한 축의 Position을 출력한다.
    /// </summary>
    /// <param name="_stage"> left or right</param>
    /// <param name="_axis"> axis </param>
    private void DisplayAxisPosition(int _stageNo, int _axis)
    {
        try
        {
            //Stage 선택!!
            Istage stage = mStage[_stageNo];

            
            //Display...
            TextBox tb = null;
            if (_stageNo == m_leftStage.stageNo)
            {
                if (_axis == stage.AXIS_X) tb = txtLeftXCurPos;
                else if (_axis == stage.AXIS_Y) tb = txtLeftYCurPos;
                else if (_axis == stage.AXIS_Z) tb = txtLeftZCurPos;
                else if (_axis == stage.AXIS_TX) tb = txtLeftThetaXCurPos;
                else if (_axis == stage.AXIS_TY) tb = txtLeftThetaYCurPos;
                else if (_axis == stage.AXIS_TZ) tb = txtLeftThetaZCurPos;
            }
            else if (_stageNo == m_rightStage.stageNo)
            {
                //----- Right Stage ----//
                if (_axis == stage.AXIS_X) tb = txtRightXCurPos;
                else if (_axis == stage.AXIS_Y) tb = txtRightYCurPos;
                else if (_axis == stage.AXIS_Z) tb = txtRightZCurPos;
                else if (_axis == stage.AXIS_TX) tb = txtRightThetaXCurPos;
                else if (_axis == stage.AXIS_TY) tb = txtRightThetaYCurPos;
                else if (_axis == stage.AXIS_TZ) tb = txtRightThetaZCurPos;
            }
            else if (_stageNo == m_othStage.stageNo)
            {
                if (_axis == CGlobal.CameraAxis) tb = txtOtherXCurPos;
                else if (_axis == CGlobal.CenterAxis) tb = txtOtherYCurPos;
            }


            if (tb != null) tb.Text = Convert.ToString(mStage[_stageNo].GetAxisAbsPos(_axis));
        }
        catch { /*do nothing*/ }
    }

    

    /// <summary>
    /// Left , Right 모든 축의 position을 출력한다.
    /// </summary>
    private void DisplayCurPositions()
    {

        try
        {

            CStageAbsPos aps = null;

            if (m_leftStage != null)
            {
                //-------------Left stage---------------------//
                aps = m_leftStage.GetAbsPositions();

                Invoke((Action)(() =>
                {
                    txtLeftXCurPos.Text = Convert.ToString(aps.x);
                    txtLeftYCurPos.Text = Convert.ToString(aps.y);
                    txtLeftYCurPos.Text = Convert.ToString(aps.y);
                    txtLeftZCurPos.Text = Convert.ToString(aps.z);
                    txtLeftThetaXCurPos.Text = Convert.ToString(aps.tx);
                    txtLeftThetaYCurPos.Text = Convert.ToString(aps.ty);
                    txtLeftThetaZCurPos.Text = Convert.ToString(aps.tz);
                }));
            }

            if (m_rightStage != null)
            {
                //-------------Right stage---------------------//
                aps = m_rightStage.GetAbsPositions();

                Invoke((Action)(() =>
                {
                    txtRightXCurPos.Text = Convert.ToString(aps.x);
                    txtRightYCurPos.Text = Convert.ToString(aps.y);
                    txtRightZCurPos.Text = Convert.ToString(aps.z);
                    txtRightThetaXCurPos.Text = Convert.ToString(aps.tx);
                    txtRightThetaYCurPos.Text = Convert.ToString(aps.ty);
                    txtRightThetaZCurPos.Text = Convert.ToString(aps.tz);
                }));
            }

            if (m_othStage != null)
            {
                //-------------Other stage---------------------//
                aps = m_othStage.GetAbsPositions();

                Invoke((Action)(() =>
                {
                    txtOtherXCurPos.Text = Convert.ToString((CGlobal.CameraAxis == CGlobal.OtherAligner.AXIS_X) ? aps.x : aps.y);
                    txtOtherYCurPos.Text = Convert.ToString((CGlobal.CenterAxis == CGlobal.OtherAligner.AXIS_X) ? aps.x : aps.y);
                }));
            }

        }
        catch { /*do nothing*/ }
    }



    //private void leftAligner_CoordUpdate(double[] coord)
    //{
    //    this.Invoke((Action)(() =>
    //    {
    //        txtLeftXCurPos.Text = coord[0].ToString();
    //        txtLeftYCurPos.Text = coord[1].ToString();
    //        txtLeftZCurPos.Text = coord[2].ToString();
    //        txtLeftThetaXCurPos.Text = coord[3].ToString();
    //        txtLeftThetaYCurPos.Text = coord[4].ToString();
    //        txtLeftThetaZCurPos.Text = coord[5].ToString();
    //    }));
    //}

    //private void rightAligner_CoordUpdate(double[] coord)
    //{
    //    this.Invoke((Action)(() =>
    //    {
    //        txtRightXCurPos.Text = coord[0].ToString();
    //        txtRightYCurPos.Text = coord[1].ToString();
    //        txtRightZCurPos.Text = coord[2].ToString();
    //        txtRightThetaXCurPos.Text = coord[3].ToString();
    //        txtRightThetaYCurPos.Text = coord[4].ToString();
    //        txtRightThetaZCurPos.Text = coord[5].ToString();
    //    }));
    //}


    #endregion
    

    

    #region public method
    

    /// <summary>
    ///  Stage의 모든축 Position을 출력한다.
    /// </summary>
    /// <param name="_stageNo"> left or right</param>
    public void DisplayStageCurPositions(int _stageNo)
    {
        try
        {
            //Stage 선택!!
            Istage stage = null;
            if (_stageNo == m_leftStage.stageNo) stage = m_leftStage;
            else if (_stageNo == m_rightStage.stageNo) stage = m_rightStage;
            else stage = m_othStage;


            //Display...
            CStageAbsPos ap = stage.GetAbsPositions();
            if (_stageNo == m_leftStage.stageNo)
            {
                //----- Left Stage ----//
                txtLeftXCurPos.Text = Convert.ToString(ap.x);
                txtLeftYCurPos.Text = Convert.ToString(ap.y);
                txtLeftZCurPos.Text = Convert.ToString(ap.z);
                txtLeftThetaXCurPos.Text = Convert.ToString(ap.tx);
                txtLeftThetaYCurPos.Text = Convert.ToString(ap.ty);
                txtLeftThetaZCurPos.Text = Convert.ToString(ap.tz);

            }
            else if (_stageNo == m_rightStage.stageNo)
            {
                //----- Right Stage ----//
                txtRightXCurPos.Text = Convert.ToString(ap.x);
                txtRightYCurPos.Text = Convert.ToString(ap.y);
                txtRightZCurPos.Text = Convert.ToString(ap.z);
                txtRightThetaXCurPos.Text = Convert.ToString(ap.tx);
                txtRightThetaYCurPos.Text = Convert.ToString(ap.ty);
                txtRightThetaZCurPos.Text = Convert.ToString(ap.tz);
            }
            else if (_stageNo == m_othStage.stageNo)
            {
                txtOtherXCurPos.Text = Convert.ToString((CGlobal.CameraAxis == CGlobal.OtherAligner.AXIS_X)? ap.x : ap.y);
                txtOtherYCurPos.Text = Convert.ToString((CGlobal.CenterAxis == CGlobal.OtherAligner.AXIS_X)? ap.x : ap.y);
            }

        }
        catch { /*do nothing*/ }


    }



    /// <summary>
    /// Stage들을 멈춘다.
    /// </summary>
    public void StopStages()
    {
        m_scp.bMoveStop = true;
    }



    /// <summary>
    /// Axis의 Position을 업데이트 한다.
    /// </summary>
    /// <param name="_stageNo"></param>
    /// <param name="_axis"></param>
    public void UpdateAxisPos(int _stageNo, int _axis)
    {
        DisplayAxisPosition(_stageNo, _axis);
    }



    /// <summary>
    /// postion을 update한다.
    /// </summary>
    public void UpdateAxisPos()
    {
        DisplayCurPositions();
    }


    #endregion


    

    #region UI_[Left & Right Stage]


    /// <summary>
    /// Z 축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftZMovePlus_Click(object sender, EventArgs e)
    {

        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_Z;
            m_scp.dbDistance = Convert.ToDouble(txtLeftZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Z 축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftZMoveMinus_Click(object sender, EventArgs e)
    {

        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_Z;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Leftstage zeroing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZero_Left_Click(object sender, EventArgs e)
    {

        if (DialogResult.No == MessageBox.Show("LEFT 스테이지를 Zeroing하시겠습니까?", "확인", MessageBoxButtons.YesNo))
            return;

        DisableWindowAll();

        try
        {
            m_leftStage.Zeroing();
            m_leftStage.Homing();
        }
        catch
        {
            //do nothing
        }

        DisplayCurPositions();


        MessageBox.Show("Left Stage의 Zeroing이 완료되었습니다.",
                        "확인",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

        EnableWindowAll();

    }



    /// <summary>
    /// right Stage Zeroing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZero_Right_Click(object sender, EventArgs e)
    {

        if (DialogResult.No == MessageBox.Show("RIGHT 스테이지를 Zeroing하시겠습니까?", "확인", MessageBoxButtons.YesNo))
            return;


        DisableWindowAll();

        try
        {
            m_rightStage.Zeroing();
            m_rightStage.Homing();
        }
        catch (Exception)
        {

        }


        DisplayCurPositions();

        EnableWindowAll();

        MessageBox.Show("Right Stage의 Zeroing이 완료되었습니다.",
                        "확인",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
    }



    /// <summary>
    /// 모든 스테이지를 Zeroing한다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZeroingAll_Click(object sender, EventArgs e)
    {

        if (DialogResult.No == MessageBox.Show("모든 스테이지를 Zeroing하시겠습니까?", "확인", MessageBoxButtons.YesNo))
            return;

        DisableWindowAll();


        //left stage.
        try
        {
            m_leftStage.Zeroing();
            m_leftStage.Homing();
        }
        catch { /* do nothing. */}


        //right stage.
        try
        {
            m_rightStage.Zeroing();
            m_rightStage.Homing();
        }
        catch { /* do nothing. */ }


        //other stage
        try
        {
            m_othStage.Zeroing();
            m_othStage.Homing();
        }
        catch { /* do nothing. */ }



        DisplayCurPositions();

        EnableWindowAll();

    }


    /// <summary>
    /// X축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftXMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_X;
            m_scp.dbDistance = Convert.ToDouble(txtLeftXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// X축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftXMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_X;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Y축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftYMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_Y;
            m_scp.dbDistance = Convert.ToDouble(txtLeftYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Y축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftYMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_Y;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta X축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaXMovePlus_Click(object sender, EventArgs e)
    {

        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaX;
            m_scp.dbDistance = Convert.ToDouble(txtLeftThetaXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Theta X축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaXMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaX;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftThetaXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Theta Y축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaYMovePlus_Click(object sender, EventArgs e)
    {

        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaY;
            m_scp.dbDistance = Convert.ToDouble(txtLeftThetaYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Theta Y축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaYMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaY;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftThetaYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Theta Z축을 상대값만큼 이동(+)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaZMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaZ;
            m_scp.dbDistance = Convert.ToDouble(txtLeftThetaZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Theta Z축을 상대값만큼 이동(-)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnLeftThetaZMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_leftStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_leftStage.AXIS_ThetaZ;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtLeftThetaZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(100);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// X축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightXMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_X;
            m_scp.dbDistance = Convert.ToDouble(txtRightXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    ///  X축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightXMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_X;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }

    }



    /// <summary>
    /// Y축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightYMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_Y;
            m_scp.dbDistance = Convert.ToDouble(txtRightYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Y축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightYMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_Y;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Z축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightZMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_Z;
            m_scp.dbDistance = Convert.ToDouble(txtRightZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Z축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightZMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_Z;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta X축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaXMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaX;
            m_scp.dbDistance = Convert.ToDouble(txtRightThetaXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta X축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaXMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaX;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightThetaXCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta Y축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaYMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaY;
            m_scp.dbDistance = Convert.ToDouble(txtRightThetaYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta Y축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaYMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaY;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightThetaYCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta Z축을 상대값만큼 이동(+) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaZMovePlus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaZ;
            m_scp.dbDistance = Convert.ToDouble(txtRightThetaZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// Theta Z축을 상대값만큼 이동(-) , Right Stage
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnRightThetaZMoveMinus_Click(object sender, EventArgs e)
    {
        try
        {
            m_scp.nStage = m_rightStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = m_rightStage.AXIS_ThetaZ;
            m_scp.dbDistance = (-1) * Convert.ToDouble(txtRightThetaZCurDist.Text);

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }



    /// <summary>
    /// 모든 스테이지의 이동을 멈춘다.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnStop_Click(object sender, EventArgs e)
    {
        m_scp.bMoveStop = true;
    }



    /// <summary>
    /// Chip protection system 설정을 취소함.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnCancelCps_Click(object sender, EventArgs e)
    {
        chkEnableCps.Checked = m_cps.enable;
        txtCpsThres.Text = Convert.ToString(m_cps.thres);
    }



    /// <summary>
    /// Chip protection system 설정 적용.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnApplyCps_Click(object sender, EventArgs e)
    {

        XConfig config = null;

        try
        {

            config = new XConfig(Application.StartupPath + "\\conf_StageControl3stg.xml");

            //enable.
            if (chkEnableCps.Checked == true)
            {
                config.SetValue("[CPS_ENABLE]", "1");
                m_cps.enable = true;
            }
            else
            {
                config.SetValue("[CPS_ENABLE]", "0");
                m_cps.enable = false;
            }


            //threshold.
            config.SetValue("[CPS_THRES]", txtCpsThres.Text);
            m_cps.thres = Convert.ToDouble(txtCpsThres.Text);
            m_cps.thres = Math.Round(m_cps.thres, 3);


            config.Dispose();
            config = null;

            MessageBox.Show("설정 완료!!",
                            "확인",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
        catch
        {
            if (config != null)
                config.Dispose();
            config = null;

            MessageBox.Show("설정하는데 실패하였습니다.",
                            "에러",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

            chkEnableCps.Checked = m_cps.enable;
            txtCpsThres.Text = Convert.ToString(m_cps.thres);
        }

        

    }


    #endregion




    #region UI_[Other Stage]


    /// <summary>
    /// other stage zeroing.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void btnZero_Other_Click(object sender, EventArgs e)
    {
        if (DialogResult.No == MessageBox.Show("Ohter Stage를 Zeroing하시겠습니까?", "확인", MessageBoxButtons.YesNo))
            return;

        DisableWindowAll();

        try
        {
            m_othStage.Zeroing();
            m_othStage.Homing();
        }
        catch (Exception)
        {

        }

        DisplayCurPositions();


        MessageBox.Show("Other Stage의 Zeroing이 완료되었습니다.",
                        "확인",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

        EnableWindowAll();
    }



    private void btnOtherXMovePlus_Click(object sender, EventArgs e)
    {
        moveCenter(CGlobal.CameraAxis, double.Parse(txtCenterDistanceX.Text) * CGlobal.CameraDirection);
    }

    private void btnOtherXMoveMinus_Click(object sender, EventArgs e)
    {
        moveCenter(CGlobal.CameraAxis, -double.Parse(txtCenterDistanceX.Text) * CGlobal.CameraDirection);
    }

    private void btnOtherYMovePlus_Click(object sender, EventArgs e)
    {
        moveCenter(CGlobal.CenterAxis, double.Parse(txtCenterDistanceY.Text) * CGlobal.CenterDirection);
    }

    private void btnOtherYMoveMinus_Click(object sender, EventArgs e)
    {
        moveCenter(CGlobal.CenterAxis, -double.Parse(txtCenterDistanceY.Text) * CGlobal.CenterDirection);
    }

    void moveCenter(int axis, double delta)
    {
        try
        {
            m_scp.nStage = m_othStage.stageNo;
            m_scp.nCommand = CMD.RELMOVE;
            m_scp.nAxis = axis;
            m_scp.dbDistance = delta;

            if (m_scp.bIsMoving == false)
            {
                m_pAutoEvent.Set();
                Thread.Sleep(10);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"StageController.moveCenter()\n{ex.Message}\n{ex.StackTrace}");
        }
    } 


    #endregion


}
