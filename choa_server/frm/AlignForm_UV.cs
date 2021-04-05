using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Neon.Aligner;
using Free302.MyLibrary.Utility;
using Neon.Aligner.UI;
using al = Neon.Aligner.AlignLogic;

partial class AlignForm
{
    #region ---- Align API ----

    public object[] OpmChs => mPm.ChList;

    public void CancelAlign()
    {
        _stopping = true;
        mAlign?.StopOperation();
        mLeft?.StopMove();
        mRight?.StopMove();
    }

    MyCTS _ctsUv;//signal auto align complete
    volatile bool _isDeMux = false;
    volatile object _UvFormData;
    class _CancelException : Exception
    { }

    public void RunAction(UvCmd cmdCode, int stageNo, MyCTS cts, bool isDemux, object data)
    {
        try
        {
            _ctsUv = cts;
            _UvFormData = data;
            _stopping = false;
            switch (cmdCode)
            {
                case UvCmd.Approach: //approach
                    setUiStart("Approach...");

                    m_tp.cmd = al.FabAuto_AppAndBack;
                    if (stageNo == mLeft.stageNo) m_tp.stageNo = mLeft.stageNo;
                    else if (stageNo == mRight.stageNo) m_tp.stageNo = mRight.stageNo;
                    else if (stageNo != 0) m_tp.cmd = al.FabAuto_AppAndBack_Both;
                    else break;//unknown

                    m_autoEvent.Set();
                    Thread.Sleep(50);
                    break;

                case UvCmd.FineSearch: //fine search
                    setUiStart("XY Fine Search");
                    ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;

                    //fine xySearch.
                    m_tp.cmd = al.XY_SEARCH;
                    if (stageNo == mLeft.stageNo) m_tp.stageNo = mLeft.stageNo;
                    else if (stageNo == mRight.stageNo) m_tp.stageNo = mRight.stageNo;
                    else if (stageNo != 0) m_tp.cmd = al.XY_SEARCH_BOTH;
                    else break;//unknown

                    //cbAlignOpmPort1.SelectedItem = data;//UV 폼과 Align폼 동기화
                    m_tp.port1 = (int)data;//cbAlignOpmPort1.Text.To<int>();
                    m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);         //Searh Threshold [dBm]

                    m_autoEvent.Set();
                    Thread.Sleep(50);
                    break;

                case UvCmd.All: //app~angle~blind~fine
                    setUiStart("Auto Align for Bonding");
                    ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;

                    _isDeMux = isDemux;

                    //fine xySearch.
                    m_tp.cmd = al.FabAuto_BondingAlign;
                    m_tp.stageNo = stageNo;

                    //blind xySearch.
                    ScanParam.FinePeakStep = (double)uiPeakSearchStep.Value;
                    m_tp.range = Convert.ToInt32(cbBlindRangeDigital.Text);      //Search Range [um]
                    m_tp.step = Convert.ToInt32(cbBlindStepDigital.Text);       //Search Step [um]
                    m_tp.port1 = Convert.ToInt32(cbAlignOpmPort1.Text);     //detect port!!
                    m_tp.thres = Convert.ToInt32(txt_search_threshold.Text);         //Searh Threshold [dBm]

                    //roll
                    setRollParam();//range, step, threshold
                    if (chkTlsForRoll.Checked)
                    {
                        m_tp.tlsForRoll = chkTlsForRoll.Checked;
                        m_tp.tls = mTls;
                        var waves = txtRollWave1.Text.Unpack<double>().ToArray();
                        m_tp.wave1 = waves[0];
                        m_tp.wave2 = waves[1];
                    }

                    m_autoEvent.Set();
                    Thread.Sleep(50);
                    break;
            }

            //waitAlignComplete(cts);
        }
        catch (Exception ex)
        {
            uiStatus.Text = $"Error: cmd = {cmdCode}";
            MessageBox.Show(ex.Message);
            _ctsUv?.Cancel();
        }
    }

    #endregion



    #region ---- FAB Angle Auto ----

    volatile bool _stopping = false;
    int _fab_auto_function = 0;
    static string[] _fab_auot_text = new string[] { "App+θY+θX", "App -10" };
    static int[] _fab_auto_aligner_code = new int[] { al.FabAuto_Right, al.FabAuto_AppAndBack };

    Button[,] _buttons;
    void initFabButton()
    {
        _buttons = new Button[,] {
            { btnApp_L, btnTY_L, btnTX_L, btnApp_L },
            { btnZapp_R, btnFARY_R, btnFARX_R, btnZapp_R }
        };
    }

    private void btnFabAll_Click(object sender, EventArgs e)
    {
        try
        {
            if (!(mAlign?.IsCompleted() ?? true)) return;

            setUiStart("FAB Auto...");
            _stopping = false;
            _UvFormData = 10;

            //left
            if (sender.Equals(btnFab_All_L)) m_tp.cmd = chkFab_All_Joint.Checked ? al.FabAuto_Both : al.FabAuto_Left;

            //right only
            else
            {
                m_tp.cmd = _fab_auto_aligner_code[_fab_auto_function];
                m_tp.stageNo = al.STAGE_L;//mRight.stageNo;
            }

            m_autoEvent.Set();
            Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            uiStatus.Text = "Error!!";
            this.Cursor = Cursors.Default;
            MessageBox.Show(ex.ToString());
        }
    }

    void sleepStopAction(Action act)
    {
        Thread.Sleep(100);
        if (_stopping) throw new _CancelException();
        act?.Invoke();
    }

    /// <summary>
    /// 주어진 aligner z축 Approach with (_UvFOrmData, 0)
    /// complete signal 없음
    /// </summary>
    /// <param name="sn"></param>
    void autoApproach(int sn, bool doSignalComplete = false)
    {
        try
        {
            var id = (mLeft.stageNo == sn) ? AppStageId.Left : AppStageId.Right;
            sleepStopAction(() => mAlign.Approach(id, (int)_UvFormData, 0));
        }
        catch (_CancelException) { }
        finally { if (doSignalComplete) _ctsUv?.Cancel(); }
    }
    /// <summary>
    /// 좌우 aligner z축 apprach: 완료후 complete signal 발생
    /// </summary>
    void autoApproach(bool doSignalComplete = false)
    {
        var cts = _ctsUv;
        //_ctsUv = null;
        try
        {
            sleepStopAction(() => autoApproach(mLeft.stageNo));
            sleepStopAction(() => autoApproach(mRight.stageNo));
        }
        catch (_CancelException) { }
        finally { if (doSignalComplete) _ctsUv?.Cancel(); }
    }


    /// <summary>
    /// 주어진 aligner : approach(0,10) Ty4회 Tx1회 approach(10,50)
    /// complete signal 불필요
    /// </summary>
    /// <param name="sn"></param>
    void autoAngle(int sn, bool doSignalComplete = false)// autoAngle = -10 App Ty Tx -30 App -10
    {
        try
        {
            var id = (mLeft.stageNo == sn) ? AppStageId.Left : AppStageId.Right;
            var stage = (mLeft.stageNo == sn) ? mLeft : mRight;

            sleepStopAction( ()=> mAlign.Approach(id, 0, 10));// approach -10+0
            sleepStopAction(()=> mAlign.AngleTy(sn));// Ty
            sleepStopAction(()=> mAlign.AngleTx(sn));// Tx
            sleepStopAction(() => mAlign.Approach(id, (int)_UvFormData, 30));// Approach -30-10
        }
        catch(_CancelException) { }
        finally { if(doSignalComplete) _ctsUv?.Cancel(); }
    }
    /// <summary>
    /// 좌우 각도 정렬, AlignForm에서만 사용 -> signal 불필요
    /// </summary>
    void autoAngle_AlignFormOnly()// Left=autoAngle, Right={autoAngle | autoApproach}
    {
        autoAngle(mLeft.stageNo);
        if (_stopping) return;

        if (_fab_auto_function == 0) autoAngle(mRight.stageNo);
        else autoApproach(mRight.stageNo);
    }

    void autoAlignBonding_Signal()
    {
        var cts = _ctsUv;
        //_ctsUv = null;

        var mux = !_isDeMux;
        var demux = _isDeMux;

        var both = (m_tp.stageNo == al.LEFT_STAGE + al.RIGHT_STAGE);
        var left = m_tp.stageNo == al.LEFT_STAGE || both;
        var right = m_tp.stageNo == al.RIGHT_STAGE || both;

        try
        {
            //approach & angle align
            if (left) autoAngle(mLeft.stageNo);
            if (right)
            {
                if (mux) sleepStopAction(() => autoAngle(mRight.stageNo));
                if (demux) sleepStopAction(() => autoApproach(mRight.stageNo));
            }

            if (left)
            {
                sleepStopAction(() => mAlign.XyBlindSearch(mLeft.stageNo, m_tp.port1, m_tp.range, m_tp.step, m_tp.thres));
                sleepStopAction(() => xySearch(mLeft.stageNo));
                sleepStopAction(() => xySearch(mRight.stageNo));
            }

            //right roll
            if (right && mux)
            {
                sleepStopAction(() => roll());
                sleepStopAction(() => xySearch(mLeft.stageNo));
                sleepStopAction(() => xySearch(mRight.stageNo));
            }
        }
        catch (_CancelException) { }
        finally { _ctsUv?.Cancel(); }
    }

    #endregion


}//class