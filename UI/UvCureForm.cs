using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;
using Free302.MyLibrary.Utility;
using System.Diagnostics;
using System.Threading;
using al = Neon.Aligner.AlignLogic;

namespace Neon.Aligner.UI
{
    public partial class UvCureForm : Form
    {

        #region ==== Constructor ====

        public UvCureForm()
        {
            InitializeComponent();

            _chk = new Dictionary<object, CheckBox>()
            {
                { btnMoveToStarting, checkBox1 },
                { btnAlignAll, checkBox2 },
                { btnOpen, checkBox3 },
                { btnClose, checkBox4 },
                { btnMoveUv, checkBox5 },
                { btnFine, checkBox6 },
                { btnStartCuring, checkBox7 },
                { btnFinish, checkBox8 }
            };

            initCmd();
        }

        Istage mCamera;
        Istage mLeft;
        Istage mRight;
        IUvCure mUshioUvCure;
        IairValvController mAir;
        int mCamAxis;
        Settings _set = Settings.Default;

        public void AssignDevice(Istage left, Istage right, Istage camera, IUvCure uv, IairValvController air, int cameraAxis)
        {
            mLeft = left;
            mRight = right;
            mCamera = camera;
            mUshioUvCure = uv;
            mAir = air;
            mCamAxis = cameraAxis;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (mCamera != null && mLeft != null && mRight != null) WriteLog("Stage Init Ok");
            if (mUshioUvCure != null) WriteLog("UV Init Ok");

            txtStartingLeft.Enabled = _set.UvCheckLeft;
            txtStartingRight.Enabled = _set.UvCheckRight;
            changeDut(_set.Uv_DutType);

            cbAlignOpmPort1.SelectedItem = _set.UvAlignOpmPort;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            mStop = true;

            if (this.WindowState == FormWindowState.Normal)
            {
                _set.UvFormLocation = this.Location;
                _set.Save();
            }
            base.OnClosing(e);
        }

        Dictionary<object, CheckBox> _chk;
        void clearCheckBox() { foreach (var c in _chk.Values) c.Checked = false; }
        void setCheckBox(object sender) => _chk[sender].Checked = true;

        public void SetOpmChs(object[] chs)
        {
            cbAlignOpmPort1.Items.Clear();
            cbAlignOpmPort1.Items.AddRange(chs);
        }

        #endregion


        #region ---- Thread Comm ----

        public Action<int, int> CoordUpdater { get; set; }
        public Action CancelAlign { get; set; }
        public Action<UvCmd, int, MyCTS, bool, object> AlignerAction { get; set; }
        void waitAndUpdate(Istage stage, int axis = int.MinValue)
        {
            if (stage == null) return;
            if (axis == int.MinValue) axis = stage.AXIS_Z;
            stage.WaitForIdle(axis);
            CoordUpdater?.Invoke(stage.stageNo, axis);
        }

        MyCTS _cts;//AlignForm으로부터 정렬 종료 신호를 받기 위한 cts
        void waitAlignComplete(CancellationTokenSource cts)
        {
            using (cts) while (!cts.IsCancellationRequested)
                {
                    Application.DoEvents();
                    if (mStop)
                    {
                        cts?.Cancel();
                        break;
                    }
                }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                CancelAlign?.Invoke();
                _cts?.Cancel();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }


        private void UvCureForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) btnCancel.PerformClick();
        }


        #endregion


        #region ---- Starting Position ----

        bool _isStartingLeftSet = true;
        bool _isStartingRightSet = true;

        private void chkLeft_CheckedChanged(object sender, EventArgs e)
        {
            txtStartingLeft.Enabled = chkLeft.Checked;
        }

        private void chkRight_CheckedChanged(object sender, EventArgs e)
        {
            txtStartingRight.Enabled = chkRight.Checked;
        }

        private void btnSetLeft_Click(object sender, EventArgs e)
        {
            try
            {
                var pos = mLeft?.GetAxisAbsPos(mLeft.AXIS_Z) ?? double.NaN;
                _isStartingLeftSet = !double.IsNaN(pos);
                txtStartingLeft.Text = pos.ToString();
            }
            catch (Exception)
            {
                txtStartingLeft.Text = "NaN";
            }
        }
        private void btnSetRight_Click(object sender, EventArgs e)
        {
            try
            {
                var pos = mRight?.GetAxisAbsPos(mRight.AXIS_Z) ?? double.NaN;
                _isStartingRightSet = !double.IsNaN(pos);
                txtStartingRight.Text = pos.ToString();
            }
            catch (Exception)
            {
                txtStartingRight.Text = "NaN";
            }
        }

        private void btnMoveToStarting_Click(object sender, EventArgs e)
        {
            try
            {
                txtLog.Clear();
                uiEnable(false);

                var canceled = false;

                if (chkLeft.Checked)
                {
                    var pos = txtStartingLeft.Text.To<double>();
                    if (pos < 0)
                    {
                        var r = MessageBox.Show("LEFT starting position is invalid!\nContinue??", "Position", MessageBoxButtons.OKCancel);
                        if (r != DialogResult.OK) return;
                    }
                    canceled = !moveToStarting(mLeft);
                    //Apporach?
                    if (canceled) return;
                }
                if (chkRight.Checked)
                {
                    var pos = txtStartingRight.Text.To<double>();
                    if (pos < 0)
                    {
                        var r = MessageBox.Show("RIGHT starting position is invalid!\nContinue??", "Position", MessageBoxButtons.OKCancel);
                        if (r != DialogResult.OK) return;
                    }
                    moveToStarting(mRight);
                    //Apporach?
                }

                waitAndUpdate(mLeft);
                waitAndUpdate(mRight);

                clearCheckBox();
                checkBox1.Checked = true;
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }

        const double _afterCureBackDistance = 3000;
        bool moveToStarting(Istage stage)
        {
            var s = stage;
            var x = s.AXIS_Z;

            const double bufferDistance = 30;
            var finishDistance = txtFinalDistance.Text.To<double>();
            if (s == mRight) finishDistance += _afterCureBackDistance;

            var cur = s.GetAxisAbsPos(x);
            var pos = (s == mLeft ? txtStartingLeft : txtStartingRight).Text.To<double>();
            var distance = pos - cur;
            if (distance - bufferDistance > finishDistance)
            {
                var stageName = s == mLeft ? "LEFT" : "RIGHT";
                var msg = $"{stageName} moving distance = {distance}μm is longer than finish distance ({finishDistance})! \nAre you SURE??";
                var res = MessageBox.Show(msg, "Starting Position??", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (res != DialogResult.OK) return false;
            }
            s.RelMove(x, distance - bufferDistance);
            return true;
        }

        #endregion


        #region ==== Log 표시 ====

        void printStep(object sender)
        {
            var btn = (Button)sender;
            WriteLog($"<{btn.Tag}> {btn.Text}");
        }
        public void WriteLog(string msg, bool clear = false, bool lf = true)
        {
            var lfChar = lf ? "\r\n" : " ";
            if (this.InvokeRequired) this.Invoke((Action)(() => writeLog(msg, clear, lfChar)));
            else writeLog(msg, clear, lfChar);
        }

        void writeLog(string msg, bool clear, string lf)
        {
            if (clear) txtLog.Text = "";
            txtLog.AppendText($"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {msg}{lf}");
            txtLog.Focus();
        }

        void UpdateCureTime(double time_sec)
        {
            if (InvokeRequired) Invoke((Action<double>)writeCureTime, time_sec);
            else writeCureTime(time_sec);
        }
        void writeCureTime(double time_sec)
        {
            txtCureTime.Text = time_sec.ToString("F01");
            txtCureTime.Refresh();
        }

        #endregion


        #region ---- UV ----

        private void btnMoveUv_Click(object sender, EventArgs e)
        {
            try
            {
                printStep(sender);
                uiEnable(false);
                var uvPos = txtUvPos.Text.To<double>();
                mCamera.AbsMove(mCamAxis, uvPos);
                waitAndUpdate(mCamera, mCamAxis);
                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }

        private async void btnStartCuring_Click(object sender, EventArgs e)
        {
            try
            {
                printStep(sender);
                uiEnable(false);
                var uvPos = txtUvPos.Text.To<double>();
                var uvTime = txtCureTime.Text.To<double>();

                //UV position
                mCamera.AbsMove(mCamAxis, uvPos);
                waitAndUpdate(mCamera, mCamAxis);

                await RunUv(uvTime);

                //z back
                mRight.RelMove(mRight.AXIS_Z, -_afterCureBackDistance);
                waitAndUpdate(mRight, mRight.AXIS_Z);

                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }


        volatile bool mStop = false;

        public async Task RunUv(double cureTime)
        {
            mStop = false;

            mUshioUvCure?.OpenShutter();

            var watch = Stopwatch.StartNew();
            while (!mStop)
            {
                var dt = watch.ElapsedMilliseconds / 1000.0;
                UpdateCureTime(cureTime - dt);
                if (dt > cureTime) break;
                await Task.Delay(250);
            }
            UpdateCureTime(cureTime);
            WriteLog(" ... UV finished", false);
        }

        #endregion


        #region ---- Open Close Finish ----

        bool mIsStageOpen = false;

        private void btnOpenFABs_Click(object sender, EventArgs e)
        {
            try
            {
                uiEnable(false);
                var distance = -1 * txtOpenDistance.Text.To<double>();
                printStep(sender);

                if (chkLeft.Checked) mLeft?.RelMove(mLeft.AXIS_Z, distance);
                if (chkRight.Checked) mRight?.RelMove(mRight.AXIS_Z, distance);

                waitAndUpdate(mLeft);
                waitAndUpdate(mRight);

                mIsStageOpen = true;
                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }

        private void btnCloseFABs_Click(object sender, EventArgs e)
        {
            try
            {
                if (mIsStageOpen == false)
                    if (DialogResult.OK !=
                        MessageBox.Show("Check stage position!\nStage Close?", "Warning!",
                                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question)) return;

                if (txtCameraPos.Text == "")
                {
                    MessageBox.Show("Set Camera Position first!", "Warning!");
                    return;
                }

                printStep(sender);
                uiEnable(false);

                //AddLog("fixing FAB rail");
                //mAir?.OpenValve();

                var distance = txtCloseDistance.Text.To<double>();
                if (chkLeft.Checked) mLeft?.RelMove(mLeft.AXIS_Z, distance);
                if (chkRight.Checked) mRight?.RelMove(mRight.AXIS_Z, distance);
                waitAndUpdate(mLeft);
                waitAndUpdate(mRight);

                mIsStageOpen = false;

                distance = txtCloseDistanceSlow.Text.To<double>();
                var speed = txtCloseSlowSpeed.Text.To<int>();
                if (chkLeft.Checked) mLeft?.RelMove(mLeft.AXIS_Z, distance, speed);
                if (chkRight.Checked) mRight?.RelMove(mRight.AXIS_Z, distance, speed);
                waitAndUpdate(mLeft);
                waitAndUpdate(mRight);

                ////approach
                //_Data = (int)txtAppDistance.Text.To<double>();
                //sendAlignSignal(sender, false);

                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtCameraPos.Text == "")
                {
                    MessageBox.Show("Set Camera Position first!", "Warning!");
                    return;
                }

                uiEnable(false);

                //AddLog("release FAB rail");
                //mAir?.CloseValve();
                printStep(sender);

                var cameraPos = txtCameraPos.Text.To<double>();
                mCamera.AbsMove(mCamAxis, cameraPos);

                var distance = -1 * txtFinalDistance.Text.To<double>();
                if (chkLeft.Checked) mLeft?.RelMove(mLeft.AXIS_Z, distance);
                if (chkRight.Checked) mRight?.RelMove(mRight.AXIS_Z, distance);

                waitAndUpdate(mCamera, mCamAxis);
                waitAndUpdate(mLeft);
                waitAndUpdate(mRight);

                WriteLog("----- Finished -----");
                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                uiEnable(true);
            }
        }

        #endregion


        private void uiEnable(bool enable)
        {
            //      btnMoveToStarting.Enabled = enable;
            //btnOpen.Enabled = enable;
            //btnClose.Enabled = enable;
            //      btnStartCuring.Enabled = enable;
            //btnFinish.Enabled = enable;
            groupParam.Enabled = enable;
            groupAction.Enabled = enable;
        }


        #region ---- Set Position ----

        private void btnSetCamera_Click(object sender, EventArgs e)
        {
            try
            {
                var pos = mCamera?.GetAxisAbsPos(mCamAxis) ?? double.NaN;
                txtCameraPos.Text = pos.ToString();
            }
            catch (Exception)
            {
                txtCameraPos.Text = "NaN";
            }
        }

        private void btnSetUv_Click(object sender, EventArgs e)
        {
            try
            {
                var pos = mCamera?.GetAxisAbsPos(mCamAxis) ?? double.NaN;
                txtUvPos.Text = pos.ToString();
            }
            catch (Exception)
            {
                txtUvPos.Text = "NaN";
            }
        }
        #endregion


        #region ---- Aysnc Align ----

        void initCmd()
        {
            btnAlignAll.Tag = UvCmd.All;
            btnClose.Tag = UvCmd.Approach;
            btnFine.Tag = UvCmd.FineSearch;
        }

        object _Data;

        void sendAlignSignal(object sender, bool setUi = true)
        {
            try
            {
                uiEnable(false);

                var cmd = (UvCmd)((Button)sender).Tag;
                int sn = 0;
                if (sender == btnFine) sn = mLeft.stageNo + mRight.stageNo;
                else if (sender == btnAlignAll)
                {
                    sn += chkLeftAlign.Checked ? mLeft.stageNo : 0;
                    sn += chkRightAlign.Checked ? mRight.stageNo : 0;
                }
                else
                {
                    sn += chkLeft.Checked ? mLeft.stageNo: 0;
                    sn += chkRight.Checked ? mRight.stageNo: 0;
                }

                // send signal & wait for complete
                using (_cts = new MyCTS())
                {
                    AlignerAction?.Invoke(cmd, sn, _cts, _set.Uv_DutType, _Data);
                    waitAlignComplete(_cts);
                }
                setCheckBox(sender);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
            finally
            {
                if (setUi) uiEnable(true);
            }
        }

        private void btnFine_Click(object sender, EventArgs e)
        {
            printStep(sender);

            var ch = cbAlignOpmPort1.SelectedItem;
            if (ch == null)
            {
                MessageBox.Show($"Check CH number!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _set.UvAlignOpmPort = (int)ch;
            _Data = _set.UvAlignOpmPort;
            sendAlignSignal(sender);

            try { _set.Save(); } catch { }
        }

        private void btnAlignAll_Click(object sender, EventArgs e)
        {
            printStep(sender);
            _Data = (int)txtAppDistance.Text.To<double>();
            sendAlignSignal(sender);
        }

        #endregion


        #region ---- DUT Type ----

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var dr = MessageBox.Show("Changing DUT. Are you sure?",
                "DUT", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (dr != DialogResult.OK) return;

                changeDut(!_set.Uv_DutType);
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }

        private void changeDut(bool isDemux)
        {
            try
            {
                _set.Uv_DutType = isDemux;
                lblDut.Text = isDemux ? "DeMUX" : "MUX";
                lblDut.LinkColor = isDemux ? Color.Orange : Color.DodgerBlue;
                lblDut.BackColor = isDemux ? Color.AliceBlue : Color.MistyRose;
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }
        }


        #endregion


    }

}