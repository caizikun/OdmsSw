using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Neon.Aligner
{

    public class InspectionGrid : DataGridView
    {
        string[] mRowNames = { "Peak", "DWL" };
        string[] mColNames = { "CH1", "CH2", "CH3", "CH4" };
        static string[] mColNamesEx = { "Min", "Max", "Unif" };
        static double[] mShiftValue = { -7.0, 0.0 };
        static double[] mShiftValueZero = { 0.0, 0.0 };

        Dictionary<string, int> mRows;
        Dictionary<string, double[]> mPassRange;
        Dictionary<string, bool> mPassRangeSign;

        Dictionary<string, Func<double, double, bool>> mPassTest;

        static bool DoShift = SecurityControl.DoShift;

        public string[] InspectionItem { get { return mRows.Keys.ToArray(); } }

        public static double ShiftPeak { get { return (DoShift) ? mShiftValue[0] : 0; } }


        #region construtor

        public InspectionGrid()
        {
            mRows = new Dictionary<string, int>();
            mPassRange = new Dictionary<string, double[]>();
            mPassRangeSign = new Dictionary<string, bool>();
            mPassTest = new Dictionary<string, Func<double, double, bool>>();

            ColorFail = System.Drawing.Color.Tomato;
            ColorPass = System.Drawing.Color.Black;

            gridProperties();

            this.DataError += InspectionGrid_DataError;
            this.CellFormatting += InspectionGrid_CellFormatting;

        }

        private void gridProperties()
        {
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.RowHeadersVisible = false;
            this.AllowUserToResizeColumns = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //this.DefaultCellStyle.Format = "N2";
            this.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", this.Font.Size);
            this.RowTemplate.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.RowTemplate.DefaultCellStyle.Format = "N2";

            this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            this.RowTemplate.Height = 30;
            this.ColumnHeadersHeight = 30;
        }

        #endregion



        #region ---- Grid Event ----

        private void InspectionGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.Message);
        }

        private void InspectionGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;
            if (e.ColumnIndex == 0) return;

            checkPassRange(e);
            e.FormattingApplied = false;

        }

        public System.Drawing.Color ColorPass { get; set; }
        public System.Drawing.Color ColorFail { get; set; }

        private void checkPassRange(DataGridViewCellFormattingEventArgs e)
        {
            var value = Math.Round((double)e.Value, 2);
            var rowName = (string)this.Rows[e.RowIndex].Cells[0].Value;
            var colName = (string)this.Columns[e.ColumnIndex].HeaderText;

            if (!mPassRange.ContainsKey(rowName)) return;

            var range = mPassRange[rowName];
            var sign = mPassRangeSign[rowName];
            var test = mPassTest[rowName];

            //test ch value
            if (mColNames.Contains(colName)) e.CellStyle.ForeColor = test(range[0], value) ? ColorPass : ColorFail;

            //test uniformity
            else if (colName == mColNamesEx[2]) e.CellStyle.ForeColor = (value > range[1]) ? ColorFail : ColorPass;
        }

        #endregion



        #region ---- Init ----

        public void InitColumnRow()
        {
            AddCol(mColNames);
            AddRow(mRowNames);

            SetPassRange(mRowNames[0], new double[] { -10.8, 0.6 }, false);
            SetPassRange(mRowNames[1], new double[] { 0.5, 0.5 }, true);

            SetPassTest(mRowNames[0], (r, v) => r < v);
            SetPassTest(mRowNames[1], (r, v) => r > Math.Abs(v));
        }


        public void AddCol(string[] colName)
        {
            mColNames = colName;
            this.Columns.Clear();
            this.Columns.Add("", "");

            foreach (var col in colName)
                this.Columns.Add(col, col);

            for (int i = 0; i < mColNamesEx.Length; i++)
                this.Columns.Add(mColNamesEx[i], mColNamesEx[i]);

            for (int i = 0; i < this.Columns.Count; i++)
                this.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

        }

        public void AddRow(string[] rowName)
        {
            this.Rows.Clear();
            mRows.Clear();
            var value = new object[Columns.Count + 1];
            foreach (var row in rowName)
            {
                mRows.Add(row, mRows.Keys.Count);
                value[0] = row;
                for (int i = 1; i < Columns.Count; i++) value[i] = 0.0;
                this.Rows.Add(value);
            }
        }

        #endregion



        #region ---- Set Value ----

        public void SetValue(string rowName, string colName, double value)
        {
            if (mRows.Count == 0) return;
            this.Rows[mRows[rowName]].Cells[colName].Value = value;
        }

        public void SetValue(double[][] value, int rowIndex = 0)
        {
            var shift = DoShift ? mShiftValue : mShiftValueZero;
            var colLen = value[rowIndex].Length;
            var rowLen = Math.Min(value.Length, mRows.Count);
            for (int row = 0; row < rowLen; row++)
            {
                for (int col = 0; col < colLen; col++)
                    Rows[row].Cells[col + 1].Value = value[row + rowIndex][col] + shift[row];
                Rows[row].Cells[colLen].Value = value[row + rowIndex][colLen - 1];
            }
        }


        public void SetPassRange(string itemName, double[] value, bool inverseSign)
        {
            mPassRange[itemName] = value;
            mPassRangeSign[itemName] = inverseSign;

        }

        public void SetPassTest(string itemName, Func<double, double, bool> test)
        {
            mPassTest[itemName] = test;
        }


        public void SetShiftValue(double[] shift)
        {
            mShiftValue = shift;
            mShiftValueZero = new double[shift.Length];
        }

        #endregion



    }
}
