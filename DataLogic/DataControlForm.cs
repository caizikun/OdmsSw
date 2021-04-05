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
using System.Diagnostics;
using System.Collections.Async;
using DrBae.TnM.UI;
using System.Windows.Forms.DataVisualization.Charting;

namespace Neon.Aligner
{
	public partial class DataControlForm : Form
	{
		Properties.Settings mSet = Properties.Settings.Default;

		public DataControlForm()
		{
			InitializeComponent();

			mDfp = new DataFileProcessor();
			mBs = new BindingSource();
			mBs.DataSource = mDfp.dt;
			uiFileGrid.DataSource = mBs;

			initUi();

			Load += dataControl_Load;
			FormClosed += dataControl_FormClosed;
			mBs.CurrentItemChanged += mBs_CurrentItemChanged;
		}

		private void dataControl_Load(object sender, EventArgs e)
		{
			Size = mSet.DataControl_FormSize;
		}
		private void dataControl_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (WindowState == FormWindowState.Normal) mSet.DataControl_FormSize = this.Size;

			mSet.Save();
		}


		void initUi()
		{
			uiTable.Dock = DockStyle.Fill;
            initGraph();

            initUi_FileGrid();
			initUi_Upload();
			initUi_Viewer();
			initInspection();
		}

		DataFileProcessor mDfp;
		BindingSource mBs;
        WdmGraph _wg;

        void initGraph()
        {
            _wg = new WdmGraph();
            uiTableInspection.Controls.Add(_wg, 0, 4);
            _wg.Dock = DockStyle.Fill;
            _wg.BorderStyle = BorderStyle.FixedSingle;
            _wg.ChartType = SeriesChartType.FastLine;
            _wg.LineThickness = 1;

            _wg.ScaleFactorX = 1000;
            _wg.Cwl = new List<int> { 1271000, 1291000, 1311000, 1331000 };
            //_wg.IntervalX = 1000;//10000?
            //_wg.IntervalOffetX = 101;
            _wg.MinY = -45;
        }

        #region ---- File Add/Clear ----


        void initUi_FileGrid()
		{
			uiTable.Controls.Add(uiFileGrid);
			uiTable.SetCellPosition(uiFileGrid, new TableLayoutPanelCellPosition(0, 2));
			uiTable.SetColumnSpan(uiFileGrid, 3);

			uiFileGrid.Dock = DockStyle.Fill;
			uiFileGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
			uiFileGrid.AllowUserToAddRows = false;
			uiFileGrid.ReadOnly = true;
			uiFileGrid.RowHeadersVisible = false;
			uiFileGrid.AllowUserToResizeRows = false;
			uiFileGrid.RowTemplate.Height = 25;

			uiFileGrid.Columns[mDfp.mColNames[2]].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			uiFileGrid.Columns[mDfp.mColNames[1]].Visible = false;
			uiFileGrid.Columns[mDfp.mColNames[3]].Visible = false;
			uiFileGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

			uiFileGrid.DataError += uiFileGrid_DataError;
			uiFileGrid.KeyDown += UiFileGrid_KeyDown;
			uiFileGrid.RowsRemoved += UiFileGrid_RowsRemoved;
			uiFileGrid.MouseClick += UiFileGrid_MouseClick;
		}

		private void UiFileGrid_MouseClick(object sender, MouseEventArgs e)
		{
			if (uiFileGrid.Rows.Count == 0) return;
			if (e.Button == MouseButtons.Right)
			{
				var m = new ContextMenuStrip();
				m.Items.Add("Erase upload data");
				m.ItemClicked += M_ItemClicked;
				m.Show(uiFileGrid, new Point(e.X, e.Y));
			}
		}


		private void M_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			mDfp.EraseUploadData();
		}



		private void UiFileGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
		{
			if (mUpload) return;
			writeLog((uiFileGrid.Rows.Count).ToString() + "개");
		}



		private void UiFileGrid_KeyDown(object sender, KeyEventArgs e)
		{
			if (uiFileGrid.Rows.Count == 0) return;
			if (mDfp.dtCount == 0) return;
			if (mUpload) return;

			var index = uiFileGrid.CurrentCell.RowIndex;
			var checkList = mDfp.dtCheckList;

			if (e.KeyCode == Keys.Back)
			{
				if (checkList[index])
				{
					mDfp.ErrorCheckChip(index, false);
					uiFileGrid.Rows[index].Cells[0].Style.BackColor = Color.White;
				}
				else
				{
					mDfp.ErrorCheckChip(index, true);
					uiFileGrid.Rows[index].Cells[0].Style.BackColor = Color.OrangeRed;
				}
			}

		}



		private void uiFileGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
		{
			if (mUpload) return;
			showError($"{e.RowIndex},{e.ColumnIndex}", e.Exception);
		}



		string mPath;

		private void uiOpen_Click(object sender, EventArgs e)
		{
			try
			{
				var of = new OpenFileDialog();
				of.Filter = "Text File(*.txt)|*.txt";
				of.Multiselect = true;
				var res = of.ShowDialog();
				if (res != DialogResult.OK) return;

				mPath = Path.GetDirectoryName(of.FileNames[0]);
				mDfp.AddFiles(of.FileNames);

				uiSantecFilter.BackColor = SystemColors.Control;
				writeLog(of.FileNames.Count().ToString() + "개");
			}
			catch (Exception ex)
			{
				showError(nameof(uiOpen_Click), ex);
			}
		}



		private void uiClear_Click(object sender, EventArgs e)
		{
			mDfp.ClearFiles();
			uiSantecFilter.BackColor = SystemColors.Control;
		}


		#endregion




		#region ---- Unzip ----


		private void uiUnzip_Click(object sender, EventArgs e)
		{

			try
			{
				var of = new OpenFileDialog();
				of.Filter = "7Z File(*.7z)|*.7z";
				of.Multiselect = true;
				var res = of.ShowDialog();
				if (res != DialogResult.OK) return;

				//directory 생성
				var saveFolder = Path.GetDirectoryName(of.FileNames[0]);
				var dir = saveFolder + @"\zipFiles";
				if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

				//압축 해제
				foreach (var file in of.FileNames)
				{
					ZipData.Extract(file, saveFolder);
					DataBackup.BackupFile(file, "zipFiles");
				}

			}
			catch (Exception ex)
			{
				showError(nameof(uiOpen_Click), ex);
			}

		}


		#endregion




		#region ---- Santec Filter ----


		private void uiSantecFilter_Click(object sender, EventArgs e)
		{
			if (mPath == null) return;
			SantecFilter.ApplyFilterFolder(mPath);
			uiSantecFilter.BackColor = Color.LightGreen;
		}

		#endregion




		#region ---- Upload ----

		void initUi_Upload()
		{
			uiCategory.SelectedIndex = 0;
			//uiComment.Text = DateTime.Now.ToString("대도A - ");
		}

		bool mUpload = false;
		int mTotal = 0;
		int mCount = 0;

		private async void uiUpload_Click(object sender, EventArgs e)
		{
			if (uiFileGrid.Rows.Count == 0) return;
			if (MessageBox.Show("Upload를 진행하시겠습니까?", "Neon Server", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
			if (uiFileGrid.Rows[0].Cells[2].Value.ToString() == "0")
				if (MessageBox.Show("Upload가 한번 진행되었습니다. 다시 하시겠습니까?", "Neon Server", MessageBoxButtons.YesNo) == DialogResult.No) return;
			mUpload = true;

			var sw = Stopwatch.StartNew();
			var lastTime = sw.ElapsedMilliseconds;

			try
			{
				uiSelectFile.Enabled = false;
				uiUpload.Enabled = false;
				uiFileGrid.Enabled = false;

				var worker = uiOperator.Text;
				var comment = uiComment.Text;
				//var category = uiCategory.SelectedIndex == 0 ? UploadCategory.CwdmMux : UploadCategory.CwdmDemux;
				UploadCategory category = UploadCategory.CwdmMux;
				switch (uiCategory.SelectedIndex)
				{
					case 0:
						category = UploadCategory.CwdmMux;
						break;
					case 1:
						category = UploadCategory.CwdmDemux;
						break;
					case 2:
						category = UploadCategory.McMux;
						break;
                    case 3:
                        category = UploadCategory.RichDemux;
                        break;
                }

				var rowsPath = new string[mDfp.dtCount];
				for (int i = 0; i < rowsPath.Length; i++)
				{
					rowsPath[i] = (string)mDfp.dt.Rows[i][mDfp.mColNames[1]];
				}

				var barList = getBarList(rowsPath);
				var snList = rowsPath.Select(x => new SerialNumber(x)).ToArray();
				mTotal = mDfp.dt.Rows.Count;
				mCount = 0;

				//LogIn
				await WebUploader.doLogin();
				//await Task.Delay(100);

				if (radioUploadParallel.Checked)
				{
					await barList.ParallelForEachAsync(async (bar) =>
							{
								string[] files;
								int[] filesIndex;
								files = getFilesOfBar(bar, snList);
								filesIndex = getIndex(files, rowsPath);
								await Task.Delay(100);
								await doFileUpload(filesIndex, files, category, worker, comment);
							}, maxDegreeOfParalellism: 5);
				}
				else
				{
					foreach (var bar in barList)
					{
						string[] files;
						int[] filesIndex;
						files = getFilesOfBar(bar, snList);
						filesIndex = getIndex(files, rowsPath);

						await doFileUpload(filesIndex, files, category, worker, comment);

						//uiFileGrid.CurrentCell = uiFileGrid[0, filesIndex.Last()];
						//uiFileGrid.FirstDisplayedScrollingRowIndex = uiFileGrid.Rows[filesIndex[(int)(filesIndex.Length * 0.7)]].Index;
					};
				}

				var time = DateTime.Now.ToString("yyMMdd HH:mm:ss"); ;
				writeLog($"{time}: numFiles={mDfp.dt.Rows.Count}, Δt={sw.ElapsedMilliseconds}ms");
				uiSantecFilter.BackColor = SystemColors.Control;
				MessageBox.Show($"Data Upload 완료\n소요 시간 : {sw.Elapsed.Minutes}분 {sw.Elapsed.Seconds}초\n총 :{mTotal.ToString()}개 중 {mCount.ToString()}개 upload 완료됨.", "Neon Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				showError(nameof(uiUpload_Click), ex);
			}
			finally
			{
				uiSelectFile.Enabled = true;
				uiUpload.Enabled = true;
				uiFileGrid.Enabled = true;
				mUpload = false;
			}
		}



		private string[] getFilesOfBar(string bar, SerialNumber[] snList)
		{
			//var b = Array.FindAll(snList, (x => x.WaferBar == bar));
			//var c = b.Select(x => x.FilePath).ToArray();
			var files = (from x in snList
						 where x.WaferBar == bar
						 select x.FilePath).ToArray();

			return files;
		}

		private int[] getIndex(string[] files, string[] rowsPath)
		{
			int[] index = new int[files.Length];
			for (int i = 0; i < files.Length; i++)
			{
				index[i] = Array.IndexOf(rowsPath, files[i]);
			}
			return index;
		}

		private string[] getBarList(string[] rowsFiles)
		{
			var sn = rowsFiles.Select(f => new SerialNumber(f));
			return sn.Distinct().Select(x => x.WaferBar).ToArray();

		}



		async Task doFileUpload(int[] rowIndex, string[] filePath, UploadCategory category, string worker, string comment)
		{
			try
			{
				//mBs.SuspendBinding();
				var res = await WebUploader.Upload(filePath, category, worker, comment);

				lock (mLock)
				{
					for (int i = rowIndex.Length - 1; i >= res; i--)
					{
						mDfp.uploadCheck(rowIndex[i]);
					}
					mCount += (rowIndex.Length - res);
					writeLog(mTotal.ToString() + "개" + "  : " + mCount.ToString() + "개 upload 완료");
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show($"doFileUpload() \n {ex.Message} \n {ex.StackTrace}");
			}
			finally
			{
				//mBs.ResumeBinding();
			}

		}


		#endregion




		#region ---- Shift ----

		static readonly object mLock = new object();
		private void uiRunShift_Click(object sender, EventArgs e)
		{
			try
			{
				var sw = Stopwatch.StartNew();

				var mShift = (double)uiShiftValue.Value;
				var doBackup = false;

				Parallel.For(0, mDfp.dt.Rows.Count, i =>
				{
					var filePath = (string)mDfp.dt.Rows[i][1];
					var ext = Path.GetExtension(filePath).Contains("7z");
					if (!ext)
					{
						DataShifter.runShiftFile(filePath, mShift, doBackup);

						lock (mLock)
						{
							//mDt.BeginLoadData();
							mDfp.uploadCheck(i, 11);
							//mDt.EndLoadData();
						}
					}
				});

				var time = DateTime.Now.ToString("yyMMdd HH:mm:ss"); ;
				writeLog($"{time}: numFiles={mDfp.dt.Rows.Count}, Δt={sw.ElapsedMilliseconds}ms");
			}
			catch (Exception ex)
			{
				showError(nameof(uiOpen_Click), ex);
			}
		}

		private void uiDoShift_CheckedChanged(object sender, EventArgs e)
		{
			uiRunShift.Visible = uiShiftValue.Visible = uiDoShift.Checked;
		}

		#endregion




		#region ---- Viewer/Inspection ----


		void initUi_Viewer()
		{
			uiDoShift.Checked = false;

			uiTable.Controls.Add(uiTableInspection);
			uiTable.SetCellPosition(uiTableInspection, new TableLayoutPanelCellPosition(3, 2));
			uiTable.SetColumnSpan(uiTableInspection, 2);
		}


		private void initInspection()
		{
			uiTableInspection.Dock = DockStyle.Fill;

			uiInspectionPeak.Dock = DockStyle.Fill;
			uiInspectionMcIL.Dock = DockStyle.Fill;
			uiInspectionMcAx.Dock = DockStyle.Fill;

			uiInspectionPeak.InitColumnRow();

			var showDwl = true;
			var item = showDwl ? new string[] { "IL", "DWL" } : new string[] { "IL" };
			uiInspectionPeak.AddRow(item);

			var range = new double[] { -3.8, 0.6 };
			txtPassRangeIlMin.Value = (decimal)range[0];
			txtPassRangeIlUnif.Value = (decimal)range[1];
			uiInspectionPeak.SetPassRange(item[0], range, false);
			uiInspectionPeak.SetPassTest(item[0], (r, v) => r < v);//IL test

			if (showDwl)
			{
				range = new double[] { 0.5, 0.5 };
				uiInspectionPeak.SetPassRange(item[1], range, true);
				uiInspectionPeak.SetPassTest(item[1], (r, v) => r > Math.Abs(v));//DWL test
			}

			//McDemux Inspection - 170503
			initInspectionMc();

			uiInspectionPeak.AutoResizeColumns();
			uiInspectionMcIL.AutoResizeColumns();
			uiInspectionMcAx.AutoResizeColumns();
		}


		string[] mRowIl = { "McIL" };
		string[] mColIl = { "IL1", "IL2", "IL3", "IL4" };
		string[] mRowAx = { "McAx" };
		string[] mColAx = { "Ax1", "Ax2", "Ax3", "Ax4", "Ax5", "Ax6" };


		private void initInspectionMc()
		{
			uiInspectionMcIL.InitColumnRow();
			uiInspectionMcIL.AddCol(mColIl);
			uiInspectionMcIL.AddRow(mRowIl);
			uiInspectionMcIL.SetPassRange(mRowIl[0], new double[] { -3.7, 0.5 }, false);
			uiInspectionMcIL.SetPassTest(mRowIl[0], (r, v) => r < v);

			uiInspectionMcAx.InitColumnRow();
			uiInspectionMcAx.AddCol(mColAx);
			uiInspectionMcAx.AddRow(mRowAx);
			uiInspectionMcAx.SetPassRange(mRowAx[0], new double[] { 20.0, 3.0 }, false);
			uiInspectionMcAx.SetPassTest(mRowAx[0], (r, v) => r > v);
		}


		private void btnPassRangeApply_Click(object sender, EventArgs e)
		{
			var item = uiInspectionPeak.InspectionItem;
			double[] passRange = new double[]
			{
			(double)txtPassRangeIlMin.Value, (double)txtPassRangeIlUnif.Value
			};
			uiInspectionPeak.SetPassRange(item[0], passRange, false);
			uiInspectionPeak.Refresh();

			uiInspectionMcIL.SetPassRange(uiInspectionMcIL.InspectionItem[0], new double[] { (double)uiMcIL.Value, 0.6 }, false);
			uiInspectionMcIL.Refresh();
			uiInspectionMcAx.SetPassRange(uiInspectionMcAx.InspectionItem[0], new double[] { (double)uiMcAx.Value, 3.0 }, false);
			uiInspectionMcAx.Refresh();
		}


		private void mBs_CurrentItemChanged(object sender, EventArgs e)
		{
			if (!uiFileGrid.Enabled) return;
			if (mBs.Current == null) return;
			if (mUpload) return;
			var filePath = (mBs.Current as DataRowView)[1] as string;
			if (!Path.GetExtension(filePath).Contains("txt")) return;
			if (!File.Exists(filePath)) return;

			//claear
			//uiGraph.ClearData();

			var dut = DutData.LoadFileNp(filePath);

			//calc peak
			double[][] peak;
			try
			{
				peak = WdmAnalyzer.AnalyzeNp(dut);
			}
			catch (Exception ex)
			{
				peak = new double[2][];
				peak[0] = Enumerable.Repeat(-99.99, 7).ToArray();
				peak[1] = Enumerable.Repeat(+99.99, 7).ToArray();

				var msg = ex.Message;
			}
			uiInspectionPeak.SetValue(peak);
           // DataPlot.Plot(uiGraph, dut, InspectionGrid.ShiftPeak);
            DataPlot.Plot(_wg, dut, InspectionGrid.ShiftPeak);

            //calc MC
            double[][] mc;
			try
			{
				mc = WdmAnalyzer.AnalyzeMcDemux(dut);
			}
			catch (Exception)
			{
				mc = new double[2][]; ;
				mc[0] = Enumerable.Repeat(-99.99, 7).ToArray();
				mc[1] = Enumerable.Repeat(+99.99, 9).ToArray();
			}
			uiInspectionMcIL.SetValue(mc, 0);
			uiInspectionMcAx.SetValue(mc, 1);
		}




		#endregion




		#region ---- Log ----

		void showError(string name, Exception ex)
		{
			MessageBox.Show($"{name}:\n{ex.Message}\n\n{ex.StackTrace}");
		}
		void writeLog(string msg)
		{
			Action action = () => uiStatus.Text = msg;
			if (InvokeRequired) Invoke(action);
			else action();
		}

		#endregion




		#region ---- Error Check ----



		private void uiOverlab_Click(object sender, EventArgs e)
		{
			//중복 측정 파일 검색
			if (uiFileGrid.Rows.Count == 0) return;

			var checkList = mDfp.OverlabCheck();

			if (checkList.Count(x => x == true) != 0)
			{
				uiEraseData.Enabled = true;
				for (int i = 0; i < checkList.Length; i++)
					if (checkList[i]) uiFileGrid.Rows[i].Cells[0].Style.BackColor = Color.OrangeRed;

				MessageBox.Show($"{checkList.Count(x => x == true).ToString()}개 항목이 검색되었습니다.");
			}
			else MessageBox.Show("검색된 항목이 없습니다.");
		}



		private void uiErrorCheck_Click(object sender, EventArgs e)
		{
			//Error 파일 검색            
			if (uiFileGrid.Rows.Count == 0) return;
			var errorIL = (double)uiErrorIL.Value;

			var checkList = mDfp.ErrorCheck(errorIL);

			if (checkList.Count(x => x == true) != 0)
			{
				uiEraseData.Enabled = true;
				for (int i = 0; i < checkList.Length; i++)
					if (checkList[i]) uiFileGrid.Rows[i].Cells[0].Style.BackColor = Color.OrangeRed;

				MessageBox.Show($"{checkList.Count(x => x == true).ToString()}개 항목이 검색되었습니다.");
			}
			else MessageBox.Show("검색된 항목이 없습니다.");
		}



		private void uiEraseData_Click(object sender, EventArgs e)
		{
			//검색파일 제거(이동)
			if (uiFileGrid.Rows.Count == 0) return;
			mDfp.EraseFile = true;

			int count = mDfp.EraseError();
			writeLog(mDfp.dtCount.ToString() + "개" + "  (" + count.ToString() + "개 삭제)");
			uiEraseData.Enabled = false;
		}



        #endregion

        private void uiTableInspection_Paint(object sender, PaintEventArgs e)
        {

        }
    }//class
}
