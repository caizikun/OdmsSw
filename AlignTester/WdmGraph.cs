using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using Free302.MyLibrary.Utility;

namespace AlignTester
{
    public partial class WdmGraph : UserControl
    {

        #region ==== Class Framework ====

        public WdmGraph()
        {
            InitializeComponent();

            //그래프 설정
            initColor();
            initGraph();

            ClearData();

            //ToolStrip 설정
            initToolStrip();
        }

        public DockStyle ToolStripDock { get { return toolStrip.Dock; } set { toolStrip.Dock = value; } }
        public bool ToolStripVisible { get { return toolStrip.Visible; } set { toolStrip.Visible = value; } }

        Legend[] mLegends;

        void initGraph(bool usingComponent = true)
        {
            //그래프 초기화
            try
            {
                var g = mGraph;
                g.BeginInit();

                mGraph.Dock = DockStyle.Fill;

                //graph
                g.BackColor = Color.FromArgb(211, 223, 240);
                g.BackSecondaryColor = Color.White;
                g.BackGradientStyle = GradientStyle.HorizontalCenter;
                g.BorderlineDashStyle = ChartDashStyle.Solid;
                g.BorderlineWidth = 1;
                g.BorderlineColor = Color.Gray;

                //chart area
                var ca = g.ChartAreas[0];
                ca.BackColor = Color.Transparent;

                if (usingComponent)
                {
                    //axes
                    ca.AxisX.MajorGrid.Enabled = true;            // x축 눈금선 표시여부
                    ca.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
                    ca.AxisX.Title = "Wavelength [nm]";
                    //ca.AxisX.TitleForeColor = Color.Blue;
                    ca.AxisY.Title = "Loss [dB]";
                    //ca.AxisY.TitleForeColor = Color.Blue;
                    ca.AxisX.TitleFont = this.Font;
                    ca.AxisY.TitleFont = this.Font;

                    //title
                    var title = new Title();
                    title.ForeColor = Color.Maroon;
                    title.Alignment = ContentAlignment.MiddleCenter;
                    title.BackColor = Color.Transparent;
                    g.Titles.Add(title);

                    //legends
                    mLegends = new Legend[] { mGraph.Legends[0], new Legend() };
                    initGraph_Legends(mLegends[0], false);
                    initGraph_Legends(mLegends[1], true);

                    //scrollbar
                    ca.AxisX.ScrollBar.ButtonColor = Color.LightBlue;
                    ca.AxisX.ScrollBar.LineColor = Color.Black;
                    ca.AxisY.ScrollBar.ButtonColor = Color.LightBlue;
                    ca.AxisY.ScrollBar.LineColor = Color.Black;

                    //cursor
                    ca.CursorX.IsUserEnabled = true;
                    ca.CursorX.IsUserSelectionEnabled = true;
                    ca.CursorX.SelectionColor = System.Drawing.Color.Gray;
                    ca.CursorX.Interval = 0.01;
                    ca.CursorY.IsUserEnabled = true;
                    ca.CursorY.IsUserSelectionEnabled = true;
                    ca.CursorY.SelectionColor = System.Drawing.Color.Gray;
                    ca.CursorY.Interval = 0.01;
                }
                else
                {
                    ca.CursorX.IsUserEnabled = false;
                    ca.CursorY.IsUserEnabled = false;
                }
                
            }
            finally
            {
                mGraph.EndInit();
            }
        }

        void initGraph_Legends(Legend legend, bool addPeak)
        {
            //범례 초기화
            legend.IsTextAutoFit = false;
            legend.Docking = Docking.Bottom;
            legend.LegendStyle = LegendStyle.Table;
            legend.BackHatchStyle = ChartHatchStyle.LightDownwardDiagonal;
            legend.BackSecondaryColor = Color.LightYellow;
            legend.BorderColor = Color.Gray;
            legend.BorderDashStyle = ChartDashStyle.Solid;
            legend.BorderWidth = 1;
            legend.ShadowOffset = 3;
            legend.Font = new Font("Consolas", 10F, FontStyle.Bold);
            legend.Alignment = StringAlignment.Center;

            //columns
            LegendCellColumn firstColumn = new LegendCellColumn();
            firstColumn.ColumnType = LegendCellColumnType.SeriesSymbol;
            legend.CellColumns.Add(firstColumn);

            LegendCellColumn secondColumn = new LegendCellColumn();
            secondColumn.ColumnType = LegendCellColumnType.Text;
            legend.CellColumns.Add(secondColumn);

            if (addPeak)
            {
                LegendCellColumn maxColumn = new LegendCellColumn();
                maxColumn.Text = "#MAX{N3}";
                maxColumn.ForeColor = Color.Coral;
                legend.CellColumns.Add(maxColumn);
            }
        }

        static Color[] mColorTable =
        {
            Color.Tomato, Color.DarkOrange, Color.LimeGreen, Color.DeepSkyBlue,
            Color.LightCoral, Color.LightSalmon, Color.DarkViolet, Color.LightSlateGray
        };
        static Dictionary<AlignerAxis, Color> mAxisColor;
        static Dictionary<PmCh, Color> mPmChColor;
        static Dictionary<DutCh, Color> mDutChColor;

        void initColor()
        {
            //그래프 초기화
            mAxisColor = buildColorMap<AlignerAxis>(mColorTable);
            mPmChColor = buildColorMap<PmCh>(mColorTable);
            mDutChColor = buildColorMap<DutCh>(mColorTable);
        }

        Dictionary<T, Color> buildColorMap<T>(Color[] colors)
        {
            var map = new Dictionary<T, Color>();
            var Ts = (T[])Enum.GetValues(typeof(T));
            var len = Math.Min(Ts.Length, colors.Length);
            for (int i = 0; i < len; i++) map.Add(Ts[i], colors[i]);
            return map;
        }

        void setChartTitle(string fileName)
        {
            //차트 타이틀 (출력 데이터 파일 경로 표시)
            mGraph.Titles[0].Text = fileName.Replace(@"\n", @"\ n");
        }

        public void ClearData()
        {
            //그래프 데이터 초기화
            try
            {
                mGraph.BeginInit();
                mGraph.Series.Clear();
                mGraph.Legends[0].Enabled = false;

                setChartTitle("");

                mGraph.ChartAreas[0].AxisY.Minimum = double.MaxValue;
                mGraph.ChartAreas[0].AxisY.Maximum = double.MinValue;

                cbSeriesList.Items.Clear();
                cbSeriesList.Items.Add("*");
                cbSeriesList.SelectedIndex = 0;
            }
            finally
            {
                mGraph.EndInit();
            }
        }

        private void initToolStrip()
        {
            toolStrip.Dock = DockStyle.Right;
            btnLoad.ButtonClick += btnLoadData_Click;
            initLoadOption();
            initPolNames();
            initPlotPols();
            //기본 Tools
            btnReset.Click += btnReset_Click;
            btnZoomOut.Click += btnZoomOut_Click;
            btnCapture.Click += btnCapture_Click;
            //범례
            chkPeak.CheckedChanged += chkLegends_CheckedChanged;
            chkLegends.CheckedChanged += chkLegends_CheckedChanged;
            //편광 옵션
            chkOptionPlotPol.CheckedChanged += chkOptionPlotPol_CheckedChanged;
            foreach (var item in mPolsItem) if (PlotPols) item.Checked = true;
            
            foreach (var item in mPolsItem) item.CheckedChanged += polsItem_CheckedChanged;
            //Series 1개 표시
            cbSeriesList.SelectedIndexChanged += cbSeriesList_SelectedIndexChanged;
        }


        #endregion



        #region =====Tool Button=====

        /// <summary>
        /// 그래프 출력용 데이터 파일 확장자 설정
        /// </summary>
        public string LoadData_EXT { get; set; }
        /// <summary>
        /// 출력데이터 타입 선택
        ///     Dut : True |
        ///     Reference : False
        /// </summary>
        public bool IsDutData { get; set; }
        
        public Func<string, NumPolsPerCh, Tuple<MeasureData<PmCh, PmPol>, MeasureMap>> RefTextReader { get; set; }
        public Func<string, NumPolsPerCh, Tuple<MeasureData<DutCh, DutDatum>, MeasureMap>> DutTextReader { get; set; }

        string mLastFileName;

        private void btnLoadData_Click(object sender, EventArgs e)
        {

            //Ref 불러오기
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = $"Data File(*.{LoadData_EXT})|*.{LoadData_EXT}";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    mLastFileName = ofd.FileName;
                    loadDataFile(mLastFileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.btnLoadData_Click(): {ex.Message}");
                setChartTitle("");
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private void polsItem_CheckedChanged(object sender, EventArgs e)
        {
            //'편광 선택 보기' 옵션 설정시 그래프 재로드
            if (mLastFileName == null) return;
            try
            {
                loadDataFile(mLastFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"polsItem_CheckedChanged(): {ex.Message}");
            }

        }

        private void loadDataFile(string fileName)
        {
            //데이터 불러오기(그래프 출력) 
            ClearData();

            NumPolsPerCh polCount = (NumPolsPerCh)cbLoadOption.SelectedItem;

            if (IsDutData)
            {
                var data = DutTextReader(fileName, polCount).Item1;
                foreach (var ch in data.Chs)
                    foreach (var pol in data.Pols) UpdateSeries(ch, pol, data.Waves, data.Get(ch, pol));
            }
            else
            {
                var data = RefTextReader(fileName, polCount).Item1;
                foreach (var ch in data.Chs)
                    foreach (var pol in data.Pols) UpdateSeries(ch, pol, data.Waves, data.Get(ch, pol));
            }
            setChartTitle(fileName);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            //초기화 버튼
            try
            {
                ClearData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.btnReset_Click(): {ex.Message}");
                setChartTitle("");
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            //줌아웃 버튼
            try
            {
                if (mGraph.Series.Count == 0) return;

                // Reset zoom on X and Y axis
                mGraph.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                mGraph.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.btnZoomOut_Click(): {ex.Message}");
            }
        }

        private void cbSeriesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //1개 채널만 보기설정
            try
            {
                if (mGraph.Series.Count == 0) return;
                if (cbSeriesList.SelectedIndex == 0) foreach (var item in mGraph.Series) item.Enabled = true;
                else foreach (var item in mGraph.Series) item.Enabled = item.Name == cbSeriesList.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.cbSeriesList_SelectedIndexChanged(): {ex.Message}");
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            //그래프 캡처
            try
            {
                // create a memory stream to save the chart image    
                using (var stream = new MemoryStream())
                {
                    // save the chart image to the stream    
                    mGraph.SaveImage(stream, System.Drawing.Imaging.ImageFormat.Png);

                    // create a bitmap using the stream    
                    var bmp = new Bitmap(stream);
                    // save the bitmap to the clipboard    
                    Clipboard.SetDataObject(bmp);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.btnCapture_Click(): {ex.Message}");
            }
        }

        private void chkLegends_CheckedChanged(object sender, EventArgs e)
        {
            //범례 Max값 표시 선택
            try
            {
                displayLegend(chkLegends.Checked, chkPeak.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.chkLegends_CheckedChanged(): {ex.Message}");
            }
        }

        private void displayLegend(bool show, bool showPeak)
        {
            //범례 Max값 표시
            chkLegends.Checked = show;
            chkPeak.Checked = showPeak;
            mGraph.Legends[0] = mLegends[showPeak ? 1 : 0];
            mGraph.Legends[0].Enabled = show;
        }

        private void initLoadOption()
        {
            //Load Data Option (Auto, 1, 4)
            cbLoadOption.Items.AddRange(MyEnum<NumPolsPerCh>.ValueObjectArray);
            cbLoadOption.SelectedIndex = 0;
        }

        static string[][] PolNames;
        private void setOptionPolsText()
        {
            //Load Data 선택별 Option 표시
            int index = IsDutData ? 1 : 0;
            for (int i = 0; i < mPolsItem.Length; i++)
            {
                mPolsItem[i].Text = PolNames[index][i];
                if (IsDutData && PlotPols) mPolsItem[i].Checked = true;
            }
        }

        private void initPolNames()
        {
            //편광 이름 설정
            PolNames = new string[2][];
            PolNames[0] = Enum.GetNames(typeof(PmPol));
            PolNames[1] = Enum.GetNames(typeof(DutDatum));
        }

        ToolStripMenuItem[] mPolsItem;
        private void initPlotPols()
        {
            //편광 표시 옵션 메뉴 구성
            mPolsItem = new ToolStripMenuItem[4];
            for (int i = 0; i < mPolsItem.Length; i++)
            {
                mPolsItem[i] = new ToolStripMenuItem();
                mPolsItem[i].CheckOnClick = true;
                mPolsItem[i].Text = i.ToString();
            }
            btnPolsOption.DropDownItems.AddRange(mPolsItem);
            chkOptionPlotPol.Checked = true;
        }

        private void chkOptionPlotPol_CheckedChanged(object sender, EventArgs e)
        {
            //편광별 보기 옵션 설정
            for (int i = 0; i < mPolsItem.Length; i++) mPolsItem[i].Visible = chkOptionPlotPol.Checked;
            PlotPols = chkOptionPlotPol.Checked;

            //'편광 선택 보기' 옵션 설정시 그래프 재로드
            if (mLastFileName != null)
            {
                try
                {
                    loadDataFile(mLastFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"chkOptionPlotPol_CheckedChanged(): {ex.Message}");
                }
            }

        }

        bool isPmPolVisible(PmPol pol)
        {
            //P1,P2,P3,P4 - 선택된 편광 설정
            return mPolsItem[(int)pol].Checked;
        }

        bool isDutPolVisible(DutDatum pol)
        {
            //Min, Max, Avg, PDL - 선택된 편광 설정
            return mPolsItem[(int)pol].Checked;
        }

        #endregion



        #region ==== Graph Setup API ====

        /// <summary>
        /// 범례 표시 여부
        /// </summary>
        public bool LegendVisible { get { return chkLegends.Checked; } set { chkLegends.Checked = value; } }
        /// <summary>
        /// 범례 Peak값 표시 여부
        /// </summary>
        public bool PeakVisible { get { return chkPeak.Checked; } set { chkPeak.Checked = value; } }
        /// <summary>
        /// 편광 표시 여부
        ///     True : 4개 편광 표시,
        ///     False : 첫번째 편광 표시
        /// </summary>
        public bool PlotPols { get { return chkOptionPlotPol.Checked; } set { chkOptionPlotPol.Checked = value; } }

        public void SetAlignGraphTools()
        {
            //Align용 설정
            initGraph(false);
            btnLoad.Visible = false;
            btnZoomOut.Visible = false;
            btnLegends.Visible = false;
            cbSeriesList.Visible = false;
            btnPolsOption.Visible = false;
            toolStripSeparator2.Visible = false;
            toolStripSeparator3.Visible = false;
        }

        public void SetRefInfoGraphTools()
        {
            //참조 Ref용 설정
            btnReset.Visible = false;
            btnLegends.Visible = true;
            toolStripSeparator3.Visible = false;

            IsDutData = false;
            setOptionPolsText();
        }

        public void SetNewRefGraphTools()
        {
            //Ref용 설정
            btnLoad.Visible = false;
            btnLegends.Visible = true;
            btnPolsOption.Visible = false;
            toolStripSeparator3.Visible = false;

            IsDutData = false;
            setOptionPolsText();
        }

        public void SetDutGraphTools()
        {
            //Dut용 설정
            btnLoad.Visible = true;
            //cbSeriesList.Size = new Size(100, 60);

            IsDutData = true;
            setOptionPolsText();
        }


        #endregion



        #region ==== Plot API ====

        public void AddAxis(AlignerAxis axis, bool legends = true)
        {
            //series추가 (Align 그래프용)
            try
            {
                mGraph.BeginInit();

                var name = axis.ToString();
                var s = mGraph.Series.FindByName(name);
                if (s == null)
                {
                    s = new Series(name);
                    s.ChartType = SeriesChartType.FastLine;
                    s.BorderWidth = 2;
                    if (mAxisColor.ContainsKey(axis)) s.Color = mAxisColor[axis];
                    mGraph.Series.Add(s);
                }
                displayLegend(legends, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.AddAxis():\n{ex.Message}");
            }
            finally
            {
                mGraph.EndInit();
            }
        }
        public void ClearAxis(AlignerAxis axis)
        {
            try
            {
                mGraph.BeginInit();

                var name = axis.ToString();
                var s = mGraph.Series.FindByName(name);
                if (s != null) s.Points.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.AddAxis():\n{ex.Message}");
            }
            finally
            {
                mGraph.EndInit();
            }
        }

        public void AddPoint(AlignerAxis axis, double coord, double power)
        {
            try
            {
                //데이터 추가 (Align 그래프용)
                if (mGraph.Series.Count == 0) return;

                var points = mGraph.Series[axis.ToString()].Points;
                points.AddXY(coord, power);

                var ca = mGraph.ChartAreas[0];
                var min = coord - 1;
                var max = coord + 1;
                if (ca.AxisX.Minimum > min) ca.AxisX.Minimum = min;
                if (ca.AxisX.Maximum < max) ca.AxisX.Maximum = max;

                min = power - 1;
                max = power + 1;
                if (ca.AxisY.Minimum > min) ca.AxisY.Minimum = min;
                if (ca.AxisY.Maximum < max) ca.AxisY.Maximum = max;
                //ca.AxisX.Interval = 1;

                mGraph.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WdmGraph.AddPoint():\n{ex.Message}");
            }
        }

        public void UpdateSeries<TCh, TPol>(TCh ch, TPol pol, double[] wl, double[] pwr)
        {
            try
            {
                mGraph.BeginInit();

                var pols = (TPol[])Enum.GetValues(typeof(TPol));
                var chs = (TCh[])Enum.GetValues(typeof(TCh));
                int iCh = Array.IndexOf(chs, ch, 0);

                var color = Color.Transparent;
                if (iCh < mColorTable.Length && pol.Equals(pols[0])) color = mColorTable[iCh];

                var name = $"{ch}";
                Series s = null;

                if (!updatePolsVisible(ch, pol, out s, out name)) return;


                //series build
                if (s == null)
                {
                    s = new Series(name);
                    s.ChartType = SeriesChartType.Line;
                    s.BorderWidth = 1;

                    if (color != Color.Transparent) s.Color = color;
                    mGraph.Series.Add(s);
                    cbSeriesList.Items.Add(name);
                }

                //data
                s.Points.DataBindXY(wl, pwr);

                var ca = mGraph.ChartAreas[0];
                ca.AxisX.Minimum = wl[0];
                ca.AxisX.Maximum = wl[wl.Length - 1];
                ca.AxisY.LabelStyle.Format = "{0.#}";   //Y축 눈금

                double min, max;//, curMin, curMax;
                findMinMax(pwr, out min, out max);

                if (ca.AxisY.Minimum > min) ca.AxisY.Minimum = min;
                if (ca.AxisY.Maximum < max) ca.AxisY.Maximum = max;

                displayLegend(chkLegends.Checked, false);

            }
            finally
            {
                mGraph.EndInit();
            }
        }

        bool updatePolsVisible<TCh, TPol>(TCh ch, TPol pol, out Series s, out string name)
        {
            name = $"{ch}";
            s = null;
            var pols = (TPol[])Enum.GetValues(typeof(TPol));

            if (!PlotPols)
            {
                if (!pol.Equals(pols[0])) return false;
                s = mGraph.Series.FindByName(name);
            }
            else
            {
                name = $"{ch}.{pol}";
                s = mGraph.Series.FindByName(name);

                bool polVisible = true;
                if (typeof(TPol).Equals(typeof(PmPol))) polVisible = isPmPolVisible(pol.ToString().To<PmPol>());
                else polVisible = isDutPolVisible(pol.ToString().To<DutDatum>());
                if (!polVisible)
                {
                    if (s != null) mGraph.Series.Remove(s);
                    return false;
                }
            }
            return true;
        }

        int findMinMax(double[] data, out double min, out double max)
        {
            //Y축 경계값 설정
            var scale = 1.1;

            min = data.Min();
            max = data.Max();

            var avg = (max + min) / 2;
            var diff = (max - min);
            if (diff == 0) diff = 0.4;
            var height = diff * scale;

            int numDigit = 1 - (int)Math.Round(Math.Log10(height * 0.1));
            if (numDigit <= 0) numDigit = 1;
            var div = Math.Round(height / 10, numDigit);

            max = Math.Round(div * Math.Ceiling((avg + diff * scale / 2) / div), numDigit);
            min = Math.Round(div * Math.Floor((avg - diff * scale / 2) / div), numDigit);

            if (min == max)
            {
                max = avg + 0.5;
                min = avg - 0.5;
            }
            //if (min < -50) min = -50;

            return numDigit;
        }



        #endregion

        private void btnZoomOut_Click_1(object sender, EventArgs e)
        {

        }
    }



}
