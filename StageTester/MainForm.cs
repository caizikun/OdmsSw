using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using Free302.TnM.Device;
using Free302.MyLibrary.Utility;
using Free302.MyLibrary.Config;
using Free302.MyLibrary.SwInfo;
using Neon.Aligner;

namespace Free302.TnM.Device.StageTester
{
    public partial class MainForm : Form
    {

        #region === Member Variables ===

        Properties.Settings set = Properties.Settings.Default;

        DeviceLogic deviceLogic;

        #endregion



        #region === Class Framwork ===

        public MainForm()
        {
            InitializeComponent();

            //init Business Logic Member
            this.deviceLogic = new DeviceLogic();

			//axis grid map
			initAxisGridMap();

            //ui event handler
            setupUiEventHandler();
            
        }
        void MainForm_Load(object sender, EventArgs e)
        {
			try
			{
				//font
				loadFont();

				//init ui
				initUi();
			}
			catch(Exception ex)
			{
				MyDebug.ShowErrorMessage("MainForm_Load()", ex);
			}
        }

		PrivateFontCollection mFontColl;
		Font mNumericFont;

		void loadFont()
		{
			if (mFontColl == null) mFontColl = new PrivateFontCollection();
			string fontFile;

			try
			{
				//#0
				fontFile = System.IO.Path.Combine(Application.StartupPath, "SourceCodePro-Regular.otf");
				mFontColl.AddFontFile(fontFile);
				fontFile = System.IO.Path.Combine(Application.StartupPath, "SourceCodePro-Bold.otf");
				mFontColl.AddFontFile(fontFile);
				mNumericFont = new Font(mFontColl.Families[0], 12, FontStyle.Bold);
			}
			catch(System.IO.FileNotFoundException)
			{
				mNumericFont = new Font("Consolas", 12, FontStyle.Bold);
				//mNumericFont = new Font("Source Code Pro", 12, FontStyle.Bold);
			}
		}

        void toolQuit_Click(object sender, EventArgs e)
        {
			try
			{
				this.Close();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolQuit_Click()", ex);
			}
        }
        void toolSwInfo_Click(object sender, EventArgs e)
        {
			try
			{
				Type[] objs = { typeof(Aligner), typeof(AlignerBase), typeof(MyCast), typeof(Neon.Aligner.Istage)};
				FSwInfoGrid fSw = new FSwInfoGrid(objs);
				fSw.Show();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolSwInfo_Click()", ex);
			}
        }

        #endregion 



        #region === UI Setup ===


        private void setupUiEventHandler()
        {
            //UI Event Handler
            this.Load += MainForm_Load;
			this.FormClosing += MainForm_FormClosing;
            this.toolQuit.Click += toolQuit_Click;
            this.toolOpen.Click += toolOpen_Click;
            this.toolTest.Click += toolTest_Click;
            this.toolClose.Click += toolClose_Click;
            this.toolSwInfo.Click += toolSwInfo_Click;
			this.toolStop.Click += toolStop_Click;
			this.toolGoZero.Click += toolGoZero_Click;
			this.toolHome.Click += toolHome_Click;
			this.toolReport.Click += toolReport_Click;
			this.toolTestForm.Click += ToolTestForm_Click;

			//aligner select
			this.toolAligner.SelectedIndexChanged += toolAligner_SelectedIndexChanged;


			//test
			this.Resize += MainForm_Resize;
            this.splitMain.SplitterMoved += split_SplitterMoved;
			this.splitGrid.SplitterMoved += split_SplitterMoved;
			this.splitGridTop.SplitterMoved += split_SplitterMoved;
        }

		void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				set.MainFormLocation = this.Location;
				if (this.Size.Height > 30 && this.Size.Width > 50) set.MainFormSize = this.Size;

				set.SplitMainDistance = this.splitMain.SplitterDistance;
				set.SplitGridDistance = this.splitGrid.SplitterDistance;
				set.SplitGridTopDistance = this.splitGridTop.SplitterDistance;

				set.Save();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("MainForm_FormClosing()", ex);
			}
		}

        private void initUi()
        {
			//
			Type[] types = { typeof(Aligner), typeof(AlignerBase), typeof(Neon.Aligner.Istage), typeof(MyCast) };

			FSwInfoGrid f = new FSwInfoGrid(types);
			this.Text = set.MainFormText + " - " + f.buildVersionString();
			f.Dispose();

			this.Location = set.MainFormLocation;
			//this.Size = set.MainFormSize;
			this.Font = set.MainFont;

			//Main Tool strip
			this.toolMain.Font = set.MainFont;
			this.toolAligner.DropDownStyle = ComboBoxStyle.DropDownList;
			this.toolAligner.Font = set.PositionFont;
			this.toolAligner.Items.AddRange(new string[] { "Test", "Center", "Left", "Right" });
			this.toolAligner.Tag = new [] { IFA_AlignerId.Test, IFA_AlignerId.Center, IFA_AlignerId.Left, IFA_AlignerId.Right};
			this.toolAligner.SelectedIndex = 0;

			//log text box
			this.textLog.Dock = DockStyle.Fill;

			//splitMain
            this.splitMain.Dock = DockStyle.Fill;
            //this.splitMain.FixedPanel = FixedPanel.Panel1;
			if(splitMain.Size.Height <= set.SplitMainDistance+100)//
			{
				splitMain.Size = new System.Drawing.Size(set.SplitMainDistance + 100, 399);
			}
            this.splitMain.SplitterDistance = set.SplitMainDistance;

			//splitGrid
			this.splitGrid.SplitterDistance = set.SplitGridDistance;
			this.splitGrid.Panel2Collapsed = true;

			//splitGridTop
			this.splitGridTop.SplitterDistance = set.SplitGridTopDistance;

            //grid Status
            initGridStatus();
            initGridConfig();

			//main form size - reset after child ui resized
			this.Size = set.MainFormSize;

        }

		#region UI size test...

		void split_SplitterMoved(object sender, SplitterEventArgs e)
		{
			displaySplitDistance();
		}
		void MainForm_Resize(object sender, EventArgs e)
		{
			textLog.AppendText(string.Format("FormSize = {0} : ", this.Size.ToString()));
			displaySplitDistance();
		}
		private void displaySplitDistance()
		{
			textLog.AppendText(string.Format("split = {0}, {1}, {2}\r\n",
				splitMain.SplitterDistance, splitGrid.SplitterDistance, splitGridTop.SplitterDistance));
		}

		#endregion


        private void initGridStatus()
        {
            DataGridView g = this.gridStatus;
            g.Dock = DockStyle.Fill;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeColumns = false;
            g.AllowUserToResizeRows = false;
            g.AllowUserToOrderColumns = false;
            g.RowHeadersVisible = false;
            g.ReadOnly = true;

			g.RowTemplate.Height = set.GridRowHieght;

            #region Columns
            
            string[] colHeader = deviceLogic.getStatusTitle();
            //DataGridViewCheckBoxColumn col;
            DataGridViewColumn col;
            foreach (string name in colHeader)
            {
				//첫 열, 마지막 열
                if (colHeader[0].Equals(name) || colHeader[colHeader.Length - 1].Equals(name))
                {
                    col = new DataGridViewTextBoxColumn();
					//1st column = axis name
                    if (colHeader[0].Equals(name))
                    {
                        col.ReadOnly = true;
                        col.Frozen = true;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else//textbox.. position
                    {
						col.MinimumWidth = 100;
						col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        col.DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                        //col.DefaultCellStyle.Font = set.PositionFont;
						col.DefaultCellStyle.Font = this.mNumericFont;
                        col.DefaultCellStyle.ForeColor = Color.ForestGreen;

						//col.DefaultCellStyle.Format = "0.00";

                    }
                }
                else//checkbox
                {
                    col = new DataGridViewCheckBoxColumn();                    
                    col.Width = 40;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                col.Name = name;
                col.HeaderText = name;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                g.Columns.Add(col);
            }

            #endregion

            string[] rowHeader = deviceLogic.getAxisTitle();
            foreach (string name in rowHeader)
            {
                g.Rows.Add(name, false, false, false, false, false, double.NaN);
            }
        }

		//enum ColumnIndexOfGridConfig { }
		//Axis;Speed;Move-;Len(μm);Move+;Stop;Zero;Home
		const int colAxis=0, colSpeed=1, colMoveN=2, colMoveP=4, colLen=3, colStop=5,colZero=6,colHome=7;
		private void initGridConfig()
        {
            DataGridView g = this.gridConfig;
			g.Tag = false;

            #region gridview properties

            g.Dock = DockStyle.Fill;
            g.AllowUserToAddRows = false;
            g.AllowUserToDeleteRows = false;
            g.AllowUserToResizeColumns = false;
            g.AllowUserToResizeRows = false;
            g.AllowUserToOrderColumns = false;
            g.RowHeadersVisible = false;
            //g.ReadOnly = true;
            g.EditMode = DataGridViewEditMode.EditOnEnter;

            g.RowTemplate.Height = set.GridRowHieght;

            g.DataError += g_DataError;
            g.CellFormatting += g_CellFormatting;
            g.CellParsing += g_CellParsing;

			g.CellContentClick += g_CellContentClick;

            #endregion


            #region Columns

            string[] colHeader = set.GridConfigColumnTitle.Unpack<string>().ToArray();

			DataGridViewColumn col;
            for (int i = 0; i < colHeader.Length; i++)
            {
                if (i== colAxis || i== colLen)//textbox
                {
                    col = new DataGridViewTextBoxColumn();
                    if (i== colAxis)//config name
                    {
                        col.ReadOnly = true;
                        col.Frozen = true;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }
                    else//position
                    {
						col.MinimumWidth = 100;
						col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        col.DefaultCellStyle.Padding = new Padding(0, 0, 5, 0);
                        //col.DefaultCellStyle.Font = set.PositionFont;
						col.DefaultCellStyle.Font = this.mNumericFont;
                        col.DefaultCellStyle.ForeColor = Color.ForestGreen;
                    }
                }
                //Axis;NORG;ORG;Speed;Move-;Len(μm);Move+
                else if(i== colSpeed)
                {
                    Array speedValues = Enum.GetValues(typeof(McSpeed));
                    //DataTable dt = new DataTable();
                    //dt.Columns.Add("Name", typeof(string));
                    //dt.Columns.Add("Value", typeof(PmcSpeed));
                    //foreach (PmcSpeed s in speedValues) dt.Rows.Add(s, s);

                    DataGridViewComboBoxColumn colCb = new DataGridViewComboBoxColumn();
                    col = colCb;
                    col.Width = 60;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    colCb.DataSource = speedValues;// dt;
                    //colCb.DisplayMember = "Name";
                    //colCb.ValueMember = "Value";
                    colCb.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else
                {
                    col = new DataGridViewButtonColumn();
                    col.Width = 80;
                }
                //common
                col.Name = colHeader[i];
                col.HeaderText = colHeader[i];
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                g.Columns.Add(col);
            }

            #endregion

            //for each Axis add a row
            string[] rowHeader = set.AxisTitle.Unpack<string>().ToArray();
			foreach (string name in rowHeader)
            {
				//Axis;Speed;Move-;Len(μm);Move+;Stop;Zero;Home
				g.Rows.Add(name, McSpeed.Fast, "◀", 1000, "▶", "Stop", "Zero", "Home");
            }

			//
			g.Tag = true;
        }

		#region grid UI event handler

		void g_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex != colSpeed) return;
            e.Value = Enum.Parse(typeof(McSpeed), e.Value.ToString());
            e.ParsingApplied = true;
        }

        void g_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != colSpeed) return;
            //e.Value = Enum.Parse(typeof(PmcSpeed),e.Value.ToString());
            //e.FormattingApplied = true;
        }
        
        void g_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
			MyDebug.ShowErrorMessage("grid_DataError()", e.Exception);
            //MessageBox.Show(e.Exception.Message);
            e.Cancel = true;            
        }
		#endregion //grid event handler


		#endregion //=== UI setup



		void toolTest_Click(object sender, EventArgs e)
        {
            this.textLog.AppendText("\r\ntoolTest_Click() @ " 
				+ DateTime.Now.ToLongTimeString() + "\r\n");
            try
            {
				//deviceLogic.testMoveAsync();
				textLog.AppendText(deviceLogic.testIsMoving() + "\r\n");
            }
            catch(Exception pex)
            {
				MyDebug.ShowErrorMessage("toolTest_Click():", pex);
            }
            finally
            {
                this.textLog.AppendText("...done @ " 
					+ DateTime.Now.ToLongTimeString() + "\r\n");
            }
        }
		private void ToolTestForm_Click(object sender, EventArgs e)
		{
			deviceLogic.testTesterForm();
		}




		#region === Grid Operations ===

		//AlignAxis - Grid Row Index
		Dictionary<AlignAxis, int> mAxisGridMap;
		Dictionary<int, AlignAxis> mGridAxisMap;
		void initAxisGridMap()
		{
			if (this.mAxisGridMap == null)
				mAxisGridMap = new Dictionary<AlignAxis, int>();
			this.mAxisGridMap.Clear();

			if (this.mGridAxisMap == null)
				mGridAxisMap = new Dictionary<int, AlignAxis>();
			this.mGridAxisMap.Clear();

			int i = 0;
			foreach (AlignAxis axis in MyEnum<AlignAxis>.ValueArray)
			{
				if (axis == AlignAxis.None) continue;
				if (axis == AlignAxis.All) continue;

				mAxisGridMap.Add(axis, i);
				mGridAxisMap.Add(i, axis);

				this.textLog.AppendText(axis.ToString() + " = " + i.ToString() + "\r\n");
				i++;
			}
		}


        //int colAxis=0, colSpeed=1, colMoveN=2, colMoveP=4, colLen=3, colStop=5,colZero=6,colHome=7;
        int[] actionColIndex = { colMoveN, colMoveP, colStop, colZero, colHome };

		void g_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			//this.textLog.AppendText(string.Format("({0},{1})\r\n", e.RowIndex, e.ColumnIndex));

			if (e.RowIndex < 0) return;
			DataGridView g = (DataGridView)sender;
			if (!(bool)g.Tag) return;
			if (!actionColIndex.Contains(e.ColumnIndex)) return;
			if (!deviceLogic.IsOpen) return;

			double dx = parseGridDx(e.RowIndex);
			if(dx==0) return ;

			if (!mGridAxisMap.ContainsKey(e.RowIndex)) return;
			AlignAxis axis = mGridAxisMap[e.RowIndex];

			switch(e.ColumnIndex)
			{
				case colStop:
					deviceLogic.stop(axis);
                    break;

				case colZero:
					deviceLogic.zeroing(axis);
					break;

				case colHome:
					deviceLogic.homing(axis);
					break;

				case colMoveN: //move -
					this.deviceLogic.moveAxis(axis, -1 * dx);
					break;

				case colMoveP: //move +
					this.deviceLogic.moveAxis(axis, +1 * dx);
					break;

				default: break;
			}
		}
		double parseGridDx(int rowIndex)
		{
			string strValue = this.gridConfig.Rows[rowIndex].Cells[colLen].Value.ToString();
			strValue = strValue.Trim();

			return Math.Abs(Double.Parse(strValue));
		}


		#endregion 




		#region === Reporting Async ===

		public void uiUpdater(AlignerMotionParam param)
        {
            if (this.Disposing) return;
            if (this.IsDisposed) return;

			try
			{
				//this.umPosition.Text = string.Format("{0}", param.x);
				//Application.DoEvents();
				
				string[] colHeader = deviceLogic.getStatusTitle();
				//Axis;EMG;LMT+;LMT-;NORG;ORG;POS
				foreach (AlignAxis axis in param.mParamMap.Keys)
				{
					if (axis == AlignAxis.None) continue;
					if (axis == AlignAxis.All) continue;

					if (!mAxisGridMap.ContainsKey(axis))
					{
						this.textLog.AppendText("error : " + axis.ToString() + "\r\n");
						//this.textLog.AppendText(mAxisGridRowMap.Keys.Count.ToString());
						return;
					}

					int rowIndex = mAxisGridMap[axis];

					DataGridViewCellCollection cells = this.gridStatus.Rows[rowIndex].Cells;
					McMotionParam mcParam = param.mParamMap[axis];

					cells[colHeader[1]].Value = false;
					cells[colHeader[2]].Value = mcParam.isLmtP;
					cells[colHeader[3]].Value = mcParam.isLmtN;
					cells[colHeader[4]].Value = mcParam.isNorg;
					cells[colHeader[5]].Value = mcParam.isOrg;

					cells[colHeader[6]].Value = mcParam.x;
				}
			}
			catch(Exception ex)
			{
				MyDebug.ShowErrorMessage("uiUpdater()", ex);
			}

        }

		static int counter = 0;
		void progress_ProgressChanged(object sender, AlignerMotionParam e)
		{
			if (this.Disposing) return;
			if (this.IsDisposed) return;

			counter++;
			if (counter % 10 != 0) return;

			this.textLog.AppendText(string.Format("[{0}]\r\n{1}\r\n", 
				DateTime.Now.ToLongTimeString(), e.ToString())); 
		}

		//
		bool reportSet = false;
		Progress<AlignerMotionParam> mReporter;		

		void toolReport_Click(object sender, EventArgs e)
		{
			setReporter(true);
        }
		void setReporter(bool start)
		{
			try
			{
				if(mReporter == null)
				{
					mReporter = new Progress<AlignerMotionParam>(uiUpdater);
					mReporter.ProgressChanged += progress_ProgressChanged;
				}

				if(!deviceLogic.IsOpen) return;

				if(reportSet || !start)
				{
					deviceLogic.setReporter(null);
					reportSet = false;
				}
				else
				{
					deviceLogic.setReporter(mReporter);
					reportSet = true;
				}
			}
			catch(Exception ex)
			{
				MyDebug.ShowErrorMessage("setReporter()", ex);
			}
			finally
			{
				this.toolReport.ForeColor = reportSet ? Color.DeepSkyBlue : this.ForeColor;
			}
		}

        #endregion ===




		#region === Simple Operations ===



		void toolAligner_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.deviceLogic.IsOpen)
			{
				this.deviceLogic.stop();
				this.deviceLogic.close();
			}

			IFA_AlignerId id = IFA_AlignerId.Test;
			id = ((IFA_AlignerId[])toolAligner.Tag)[toolAligner.SelectedIndex];
			this.deviceLogic.selectAligner(id);
		}
		
		void toolOpen_Click(object sender, EventArgs e)
		{
			this.textLog.AppendText("Starting Open @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			//Application.DoEvents();

			try
			{
				this.textLog.AppendText(deviceLogic.open());
			}
			catch (Exception ex)
			{
				this.textLog.AppendText(ex.Message + "\r\n" + ex.StackTrace);
			}
			finally
			{
				this.textLog.AppendText("\r\n...done @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			}
		}

		void toolClose_Click(object sender, EventArgs e)
		{
			this.textLog.AppendText("\r\ntoolClose_Click() @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			try
			{
				setReporter(false);
				this.textLog.AppendText( deviceLogic.close());				
            }
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolClose_Click():", ex);
			}
			finally
			{
				this.textLog.AppendText("\r\n...done @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			}
		}


        void toolStop_Click(object sender, EventArgs e)
		{
			this.textLog.AppendText("\r\ntoolStop_Click() @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			try
			{
				this.deviceLogic.stop();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolStop_Click():", ex);
			}
			finally
			{
				this.textLog.AppendText("...done @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			}
		}

		void toolHome_Click(object sender, EventArgs e)
		{
			this.textLog.AppendText("\r\ntoolHome_Click() @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			try
			{
				deviceLogic.homing();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolHome_Click():", ex);
			}
			finally
			{
				this.textLog.AppendText("...done @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			}
		}

		void toolGoZero_Click(object sender, EventArgs e)
		{
			this.textLog.AppendText("\r\ntoolGoZero_Click() @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			try
			{
				deviceLogic.zeroing();
			}
			catch (Exception ex)
			{
				MyDebug.ShowErrorMessage("toolGoZero_Click():", ex);
			}
			finally
			{
				this.textLog.AppendText("...done @ " + DateTime.Now.ToLongTimeString() + "\r\n");
			}
		}

		#endregion === simple


	}
}

