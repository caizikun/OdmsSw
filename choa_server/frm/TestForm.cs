using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Free302.TnM.DataAnalysis;
using System.Diagnostics;
using DrBae.TnM.Device;

namespace Neon.Aligner.choa_server
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        DaqBase _daq;
        HsbOpm _opm;
        CancellationTokenSource _cts;
        private async void uiOpen_Click(object sender, EventArgs e)
        {
            uiLog.Clear();
            int numLoop = 0;
            _stop = false;
            _cts = new CancellationTokenSource();
            try
            {
                log("_daq.Open()...");
                _daq = new DaqBase("TestDaq", true, new int[] { 10000, 100 });
                _daq.Open();

                _opm = new HsbOpm();
                log("_opm.Init()");
                _opm.Init(_daq, CGlobal.HsbOpmParam);

                var w = Stopwatch.StartNew();
                numLoop = (int)uiNumLoop.Value;

                log($"# \tPower \tΔt");
                for (int i = 0; i < numLoop; i++)
                {
                    uiNumLoop.Value = numLoop - i;

                    w.Restart();
                    var pw = 10 * Math.Log10(_opm.ReadPower(1));
                    var dt = w.ElapsedMilliseconds;
                    log($"{i + 1} \t{pw:F03} \t{dt}");

                    Refresh();
                    if (_stop) break;
                    await Task.Delay(100, _cts.Token);
                }
                log($"_stop={_stop}");
            }
            catch (TaskCanceledException ex)
            {
                log($"TaskCanceledException: IsCancellationRequested={ex.CancellationToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
            finally
            {
                _daq?.Dispose();
                uiNumLoop.Value = numLoop;
            }
        }

        void log(string msg)
        {
            if (InvokeRequired) Invoke((Action)(() => _log(msg)));
            else _log(msg);
        }
        void _log(string msg)
        {
            var time = DateTime.Now.ToString("[HH:mm:ss.fff]");
            uiLog.AppendText($"{time} {msg}\n");
            uiLog.Refresh();
        }

        volatile bool _stop = false;
        private async void uiStart_Click(object sender, EventArgs e)
        {
            int numLoop = 0;
            var f = Application.OpenForms.OfType<AlignForm>().FirstOrDefault();
            if (f == null)
            {
                log("Can not found AlignForm");
                return;
            }
            try
            {
                _stop = false;
                _cts = new CancellationTokenSource();

                var ch = uiCh.Checked ? 1 : 2;
                numLoop = (int)uiNumLoop.Value;
                var sb = new StringBuilder();
                for (int i = 0; i < numLoop; i++)
                {
                    uiNumLoop.Value = numLoop - i;
                    Refresh();
                    f.btnFINE_R_Digital.PerformClick();

                    //if (_cts.IsCancellationRequested) break;//
                    //Application.DoEvents();
                    //Thread.Sleep(5000);
                    await Task.Delay(5000, _cts.Token);
                    if (_stop) break;
                }
                if (_cts.IsCancellationRequested) log($"for: IsCancellationRequested={_cts.IsCancellationRequested}");
            }
            catch (TaskCanceledException ex)
            {
                log($"TaskCanceledException: IsCancellationRequested={ex.CancellationToken.IsCancellationRequested}");
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
            finally
            {
                uiNumLoop.Value = numLoop;
            }
        }

        private void uiCh_CheckedChanged(object sender, EventArgs e)
        {
            uiCh.Text = uiCh.Checked ? "CH1" : "CH2";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //_stop = true;
            _cts?.Cancel();
        }
    }
}
