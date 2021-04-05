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


namespace AlignTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            initUi();
        }

        void initUi()
        {
            uiSplit1.Dock = DockStyle.Fill;
            uiSplit2.Dock = DockStyle.Fill;
            uiLog.Dock = DockStyle.Fill;
            uiSplit1.SplitterDistance = 150;
            uiSplit2.SplitterDistance = 350;

            //initUi_Graph();

            initUi_Grid();
        }

        DataGridView mGrid;
        DataTable mDt;

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        void log(string msg)
        {
            if (InvokeRequired) Invoke((Action)(() => uiLog.AppendText($"{msg}\r\n")));
            uiLog.AppendText($"{msg}\r\n");
        }
        void initUi_Grid()
        {
            mGrid = new DataGridView();
            mDt = new DataTable("Data");
            mGrid.DataSource = mDt;
            uiSplit2.Panel1.Controls.Add(mGrid);
            mGrid.Dock = DockStyle.Fill;

            mDt.Columns.Add("wave", typeof(double));
            mDt.Columns.Add("power", typeof(double));
            //mDt.Rows.Add(new double[] { 0.0 });
        }


        WdmGraph uiGraph;
        void initUi_Graph()
        {
            uiGraph = new WdmGraph();
            uiSplit2.Panel1.Controls.Add(uiGraph);
            uiGraph.Dock = DockStyle.Fill;
            uiGraph.SetAlignGraphTools();

            uiGraph.AddAxis(AlignerAxis.X, true);
        }
        void update(double coord, double power)
        {
            if (InvokeRequired) Invoke((Action)(() => uiGraph.AddPoint(AlignerAxis.X, coord, power)));
            else uiGraph.AddPoint(AlignerAxis.X, coord, power);
        }


        private void uiRun_Click(object sender, EventArgs e)
        {
            try
            {
                uiLog.Clear();
                //uiGraph.ClearAxis(AlignerAxis.X);

                //DutData.Test(update);
                //AlignLogic.TestScan(log, update);
                //WdmAnalyzer.Test();

                var data = mPm.TestSweep(mTls);
                mDt.BeginLoadData();
                mDt.Rows.Clear();
                for (int i = 0; i < data[0].Count; i++) mDt.Rows.Add(data[0][i], data[1][i]);
                mDt.EndLoadData();
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }


        SantecTls mTls;
        Ca3000 mPm;
        ConfigTlsParam mParam;

        private void uiInit_Click(object sender, EventArgs e)
        {
            try
            {
                log("Init SantecTls...");
                mTls = new SantecTls();
                mTls.ConnectByGpib(1);
                mParam = new ConfigTlsParam();
                mParam.WaveStart = 1520;
                mParam.WaveStop = 1570;
                mParam.WaveStep = 0.05;
                mParam.Speed = 40;
                mParam.Power = 0;
                mTls.Init(mParam);

                log("Init Ca3000...");
                mPm = new Ca3000();
                mPm.Open(16);
                mPm.Init(0, mParam.Values);

                log("Init complete!");
            }
            catch (Exception ex)
            {
                log(ex.Message);
            }
        }
    }
}
