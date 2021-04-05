using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using Neon.Aligner;
using System.Threading;

namespace Neon.Dwdm.form
{
    public partial class McuTestForm : Form
    {
        public McuTestForm()
        {
            InitializeComponent();

            initCombo();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            uiMcuInit_Click(this, e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                _mcu.Close();
            }
            catch(Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
        }

        C8164 _8164 = CGlobal.Pm8164;
        Neon16ChMcu _mcu;

        void initCombo()
        {
            //OPM chs
            var chs = (_8164?.ChList) ?? new object[0];
            uiOpmCh.Items.Clear();
            uiOpmCh.Items.AddRange(chs);
            uiOpmCh.SelectedIndex = chs.Length == 0 ? -1 : 0;

            //MCU chs
            uiMcuCh.Items.Clear();
            uiMcuCh.Items.AddRange(Enumerable.Range(1, 16).Select(i => (object)i).ToArray());
            uiMcuCh.SelectedIndex = 0;

            //MCU value type
            uiValueType.Items.Clear();
            var types = Enum.GetValues(typeof(Neon16ChMcu._ValueType)).Cast<object>().ToArray();
            uiValueType.Items.AddRange(types);
            uiValueType.SelectedIndex = 0;
        }


        #region ---- MCU ----

        private void uiMcuInit_Click(object sender, EventArgs e)
        {
            try
            {
                openMcu(int.Parse(uiMcuComPort.Text));
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
        }

        private void uiMcuCh_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _mcu?.SetMonitor(displayMcu, (int)uiMcuCh.SelectedItem);
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
        }
        private void uiValueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (_mcu == null) return;
                _mcu.ValueType = (Neon16ChMcu._ValueType)uiValueType.SelectedItem;
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
        }
        void displayMcu(decimal value)
        {
            Invoke((Action)(() => uiMcuMonitor.Text = value.ToString()));
        }

        private void openMcu(int comNumber)
        {
            try
            {
                if (_mcu != null) return;

                _mcu = new Neon16ChMcu();
                _mcu.Config(comNumber);
                _mcu.ValueType = Neon16ChMcu._ValueType.ADC;
                _mcu.Open();

                uiMcuInit.ForeColor = Color.DodgerBlue;
                uiMcuInit.Refresh();

                _mcu?.SetMonitor(displayMcu, (int)uiMcuCh.SelectedItem);

                log($"MCU opened @ COM{_mcuCom}");
            }
            catch (Exception)
            {
                _mcu = null;
                throw;
            }
        }
        private void closeMcu()
        {
            _mcu?.Close();
            _mcu = null;
            log($"MCU closed");
            uiMcuInit.ForeColor = Color.Black;
        }

        #endregion


        private async void uiRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (uiRun.Tag != null && (bool)uiRun.Tag)
                {
                    _cts?.Cancel();
                    return;
                }

                uiRun.Tag = true;
                uiRun.ForeColor = Color.OrangeRed;

                _attSlot = int.Parse(uiAttSlot.Text);
                _scan = uiScanRange.Text.Split(' ').Select(s => decimal.Parse(s)).ToArray();
                _mcuCom = int.Parse(uiMcuComPort.Text);
                _mcuCh = (int)uiMcuCh.SelectedItem;
                _opmCh = int.Parse(uiOpmCh.Text);
                _numDp = int.Parse(uiNumDp.Text);

                if (!_isUserFolder)
                {
                    var time = DateTime.Now.ToString("yyMMdd_HHmmss");
                    _path = uiSaveFolder.Text = Path.Combine(Application.StartupPath, $"Data_{time}");
                    uiSaveFolder.Refresh();
                }

                uiLog.Focus();


                _cts = new CancellationTokenSource();
                await run();
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
            finally
            {
                uiRun.Tag = false;
                uiRun.ForeColor = Color.Black;
            }
        }

        int _attSlot;
        int _mcuCom;
        int _opmCh;
        int _mcuCh;
        string _path;
        decimal[] _scan;
        int _numDp = 1;

        CancellationTokenSource _cts;

        async Task run()
        {
            var start = Math.Max(_scan[0], _scan[1]);
            var stop = Math.Min(_scan[0], _scan[1]);
            var step = Math.Abs(_scan[2]) * (stop <= start ? -1 : +1);
			var valueType = _mcu.ValueType;

			if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
			var time = DateTime.Now.ToString("HHmmss");
			var filePath = Path.Combine(_path, $"Ch{_mcuCh:D02}_{time}.txt");

            try
            {
                log($"Starting CH{_mcuCh}: {_scan.Select(i => i.ToString()).Aggregate((s1, s2) => $"{s1}:{s2}")}");
                openMcu(_mcuCom);

                _8164.WriteAttOffset(_attSlot, 0);
                _8164.WriteAtt(_attSlot, start);

                if (_numDp == 1)
                {
                    using (var sw = new StreamWriter(filePath))
                    {
                        for (decimal att = start; att >= stop; att += step)
                        {
                            if (_cts.Token.IsCancellationRequested) return;

                            _8164.WriteAtt(_attSlot, Math.Abs(att));
                            Thread.Sleep(300);

                            var pw = double.NaN;
                            try { pw = Math.Round(_8164.ReadPower_dBm(_opmCh), 3); } catch { }

							_mcu.ValueType = Neon16ChMcu._ValueType.ADC;
							var mcu = _mcu.Read(_mcuCh);
							_mcu.ValueType = Neon16ChMcu._ValueType.dB;
							var mcuDB = _mcu.Read(_mcuCh);

							var data = $"{att:F02}\t{pw:F03}\t{mcu}\t{mcuDB}";
                            sw.WriteLine(data);
                            log($"CH{_mcuCh}\t{data}");
                        }
                    }
				}
                else
                {
                    var pathBack = Path.Combine(_path, "_");
                    if (!Directory.Exists(pathBack)) Directory.CreateDirectory(pathBack);

                    var chData = new List<List<decimal>>();

                    for (decimal att = start; att >= stop; att += step)
                    {
                        var filePathBackup = Path.Combine(pathBack, $"Ch{_mcuCh:D02}_{att:F02}dB.txt");
                        using (var sw = new StreamWriter(filePathBackup))
                        {
                            _8164.WriteAtt(_attSlot, Math.Abs(att));
                            Thread.Sleep(300);

                            var atts = new List<decimal>();
                            var powers = new List<decimal>();
                            var mcus = new List<decimal>();

                            chData.Add(atts);
                            chData.Add(powers);
                            chData.Add(mcus);

                            for (int i = 0; i < _numDp; i++)
                            {
                                if (_cts.Token.IsCancellationRequested) return;

                                atts.Add(att);
                                var pw = 0m;
                                try { pw = (decimal)Math.Round(_8164.ReadPower_dBm(_opmCh), 3); } catch { }

								_mcu.ValueType = Neon16ChMcu._ValueType.ADC;
								var mcu = _mcu.Read(_mcuCh);
								_mcu.ValueType = Neon16ChMcu._ValueType.dB;
								var mcuDb = _mcu.Read(_mcuCh);

								powers.Add(pw);
                                mcus.Add(mcu);

                                var data = $"{att:F02}\t{pw:F03}\t{mcu}\t{mcuDb}";
                                sw.WriteLine(data);
                            }
                            log($"CH{_mcuCh} AVG\t{att:F02}\t{powers.Average()}\t{mcus.Average()}");
                        }//using sw
                    }//for Att

                    save(chData, filePath);

                }//else - 
            }
            finally
            {
				//closeMcu();
				_mcu.ValueType = valueType;
				_8164.WriteAtt(_attSlot, start);
				log($"Finished CH{_mcuCh}");
            }
        }



        #region ---- Save & Log ----

        bool _isUserFolder = false;
        private void uiSaveFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                var fd = new FolderBrowserDialog();
                if (fd.ShowDialog() != DialogResult.OK) return;
                _path = uiSaveFolder.Text = fd.SelectedPath;
                _isUserFolder = true;
            }
            catch (Exception ex)
            {
                log(ex.Message);
                log(ex.StackTrace);
            }
        }

        void save(List<List<decimal>> data, string filePath)
        {
            using (var sw = new StreamWriter(filePath))
            {
                var sb = new StringBuilder();
                for (int r = 0; r < data[0].Count; r++)
                {
                    sb.Clear();
                    for (int c = 0; c < data.Count; c++) sb.Append($"{data[c][r]}\t");
                    sw.WriteLine(sb.ToString());
                }
            }
        }


        void log(string msg)
        {
            if (!InvokeRequired) _log(msg);
            else Invoke((Action<string>)_log, msg);
        }
        void _log(string msg)
        {
            uiLog.AppendText($"{msg}\r\n");
            uiLog.Refresh();
        }

        #endregion

    }//class   

}
