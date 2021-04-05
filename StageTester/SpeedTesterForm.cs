using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Free302.MyLibrary.Utility;
using Free302.MyLibrary.Config;
using Free302.TnM.Device;
using Neon.Aligner;

namespace Free302.TnM.Device.StageTester
{
	public partial class SpeedTesterForm : Form
	{

		#region ==== Data Types ====

		enum ConfigItem { Speed_μs, Delay_ms }

		#endregion



		#region ==== Class Framewrok ====

		static SpeedTesterForm sSelf = null;

		public static SpeedTesterForm Self
		{
			get
			{
				if(sSelf == null || sSelf.IsDisposed || sSelf.Disposing)
				{
					sSelf = new SpeedTesterForm();
				}
				return sSelf;
			}
		}

		private SpeedTesterForm()
		{		
			InitializeComponent();
			DoubleBuffered = true;

			//제어할 Aligner 목록
			mAligners = new Dictionary<IFA_AlignerId, AlignerBase>();

			//menu handler
			this.menuReload.Click += MenuReload_Click;
			this.menuApply.Click += MenuApply_Click;
			this.menuSaveData.Click += MenuSaveData_Click;

			//init DataTable
			initDt();

			//init ui - Grid
			initUi();
		}


		private void MenuSaveData_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				saveData();
				MessageBox.Show("Saving complete!");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void MenuApply_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				applyData(this.gridConfig);
				MessageBox.Show("Applying complete!");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}

		private void MenuReload_Click(object sender, EventArgs e)
		{
			try
			{
				this.Cursor = Cursors.WaitCursor;
				loadConfigData();
				MessageBox.Show("Loading complete!");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
			}
			finally
			{
				this.Cursor = Cursors.Default;
			}
		}


		#endregion




		#region ==== Aligner 관리 ====

		/// <summary>
		/// 제어하고자 하는 Aligner를 지정하는 메소드
		///  - 특정 Aligner를 개별적으로 지정
		/// </summary>
		/// <param name="id">AlignerId - Left, Right, Center 중 하나</param>
		/// <param name="aligner"></param>
		public void registerAligner(IFA_AlignerId id, AlignerBase aligner)
		{
			if(aligner == null) return;

			if(mAligners.ContainsKey(id)) mAligners[id] = aligner;
			else mAligners.Add(id, aligner);

			//reload config data
			loadConfigData();
		}

		/// <summary>
		/// 제어하고자 하는 Aligner를 지정하는 메소드
		///  - Left, Right, Center를 한꺼번에 지정
		///  - null을 넣으면 생략 가능
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <param name="center"></param>
		/// <param name="test"></param>
		public void registerAligner(AlignerBase left, AlignerBase right, AlignerBase center, AlignerBase test = null)
		{
			//mAligners.Clear();

			if(left != null)
			{
				if(mAligners.ContainsKey(IFA_AlignerId.Left)) mAligners[IFA_AlignerId.Left] = left;
				else mAligners.Add(IFA_AlignerId.Left, left);
			}
			if(right != null)
			{
				if(mAligners.ContainsKey(IFA_AlignerId.Right)) mAligners[IFA_AlignerId.Right] = right;
				else mAligners.Add(IFA_AlignerId.Right, right);
			}
			if(center != null)
			{
				if(mAligners.ContainsKey(IFA_AlignerId.Center)) mAligners[IFA_AlignerId.Center] = center;
				else mAligners.Add(IFA_AlignerId.Center, center);
			}
			if(test != null)
			{
				if(mAligners.ContainsKey(IFA_AlignerId.Test)) mAligners[IFA_AlignerId.Test] = test;
				else mAligners.Add(IFA_AlignerId.Test, test);
			}

			//reload config data
			loadConfigData();

		}


		#endregion




		#region ===== Config Data 관리 =====

		Dictionary<IFA_AlignerId, AlignerBase> mAligners;
		DataTable mDt;
		BindingSource mBs;
		List<string> mColumnNames;

		void initDt()
		{
			try
			{
				mColumnNames = new List<string>();
				if(mDt == null) mDt = new DataTable("ConfigData");
				mDt.BeginInit();

				mDt.Rows.Clear();
				mDt.Columns.Clear();

				//aligner
				mColumnNames.Add("Aligner");
				mDt.Columns.Add(mColumnNames.Last(), typeof(string)); //AlignerId

				//config item
				mColumnNames.Add("Config");
				mDt.Columns.Add(mColumnNames.Last(), typeof(string));

				//axis
				AlignAxis[] axisList = MyEnum<AlignAxis>.ValueArray;
				foreach(AlignAxis axis in axisList)
				{
					if(axis == AlignAxis.None) continue;
					if(axis == AlignAxis.All) continue;

					mColumnNames.Add(axis.ToString());
					mDt.Columns.Add(mColumnNames.Last(), typeof(int));
				}

				if(mBs == null) mBs = new BindingSource();
				mBs.DataSource = mDt;
			}
			catch(Exception ex)
			{
				MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
			}
			finally
			{
				mDt.EndInit();
			}

		}

		void loadConfigData()
		{
			try
			{
				mDt.BeginInit();
				mDt.Rows.Clear();

				foreach(IFA_AlignerId id in mAligners.Keys)
				//foreach(AlignerId id in EnumToArray<AlignerId>.ValueArray)
				{
					AlignerBase aligner = mAligners[id];

					DataRow rowSpeed = mDt.NewRow();
					DataRow rowDelay = mDt.NewRow();

					//speed data
					rowSpeed["Aligner"] = id;
					rowSpeed["Config"] = ConfigItem.Speed_μs;

					//delay data
					rowDelay["Aligner"] = id;
					rowDelay["Config"] = ConfigItem.Delay_ms;

					//gather data of each axis
					foreach(AlignAxis axis in MyEnum<AlignAxis>.ValueArray)
					{
						if(axis == AlignAxis.None) continue;
						if(axis == AlignAxis.All) continue;

						McAxis mcAxis = aligner.getMcAxis(axis);
						if(mcAxis == McAxis.None) continue;
						if(mcAxis == McAxis.All) continue;

						IMc mc = aligner.getMc(axis);
						StageBase stage = mc.getStage(mcAxis);

						//speed
						rowSpeed[axis.ToString()] = stage.SpeedMap[McSpeed.Fast];

						//delay
						rowDelay[axis.ToString()] = stage.ReportDelayTime_ms;

					}//axis

					mDt.Rows.Add(rowSpeed);
					mDt.Rows.Add(rowDelay);

				}//for each id
			}
			catch(Exception ex)
			{
				MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
			}
			finally
			{
				mDt.EndInit();
			}
		}

		void applyData(DataGridView g)
		{
			foreach(DataRow row in mDt.Rows)
			{
				IFA_AlignerId id = (IFA_AlignerId)Enum.Parse(typeof(IFA_AlignerId), (string)row["Aligner"]);
				AlignerBase aligner = mAligners[id];
				ConfigItem item = (ConfigItem )Enum.Parse(typeof(ConfigItem), (string)row["Config"]);
				
				for(int i = 2; i < row.ItemArray.Length; i++)
				{
					AlignAxis axis = (AlignAxis)Enum.Parse(typeof(AlignAxis), row.Table.Columns[i].ColumnName);

					object value = row[i];
					if(DBNull.Value == value) continue;

					int intValue = (int)value;
					if(item == ConfigItem.Speed_μs) this.setSpeed(id, axis, intValue);
					else this.setDelay(id, axis, intValue);
				}
			}
		}
		
		void setSpeed(IFA_AlignerId aId, AlignAxis axis, int speed)
		{
			McAxis mcAxis = mAligners[aId].getMcAxis(axis);
			if(mcAxis == McAxis.None) return;

			IMc mc = mAligners[aId].getMc(axis);
			mc.setSpeedValue(mcAxis, speed);
		}
		void setDelay(IFA_AlignerId aId, AlignAxis axis, int delay)
		{
			McAxis mcAxis = mAligners[aId].getMcAxis(axis);
			if(mcAxis == McAxis.None) return;

			IMc mc = mAligners[aId].getMc(axis);
			mc.setReportDelay(mcAxis, delay);
		}


		void saveData()
		{
			SaveFileDialog ofd = new SaveFileDialog();
			ofd.InitialDirectory = Application.ExecutablePath;
			ofd.Filter = "XML Data File(*.xml)|*.xml";

			if(ofd.ShowDialog() == DialogResult.OK)
			{
				this.mDt.WriteXml(ofd.FileName);
            }
			
		}

		#endregion




		#region ===== UI =====

		void initUi()
		{
			DataGridView g = this.gridConfig;

			g.DataSource = null;

			//grid
			initGrid(g);
			initGridColumn(g);

			//bind
			g.DataSource = this.mBs;
			this.mBs.DataSource = this.mDt;
		}

		void initGrid(DataGridView g)
		{
			//grid properties
			g.AllowUserToAddRows = false;
			g.AllowUserToDeleteRows = false;
			g.AllowUserToOrderColumns = false;
			g.AllowUserToResizeColumns = false;
			g.AllowUserToResizeRows = false;
			g.EditMode = DataGridViewEditMode.EditOnEnter;

			g.RowHeadersVisible = false;
			g.RowTemplate.Height = 36;
			g.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
			g.ColumnHeadersHeight = 36;
			g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			//g.RowTemplate.DefaultCellStyle.BackColor = Color.FromArgb(255, 128, 128 + 64, 255);
			g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(255, 221, 235, 247);

			g.Font = new Font("Source Code Pro", 12);
			g.Dock = DockStyle.Fill;
		}
		void initGridColumn(DataGridView g)
		{
			g.Rows.Clear();
			g.Columns.Clear();

			AlignAxis[] axisList = MyEnum<AlignAxis>.ValueArray;

			DataGridViewTextBoxColumn col;
			foreach(DataColumn dc in mDt.Columns)
			{
				col = new DataGridViewTextBoxColumn();
				col.Name = dc.ColumnName;
				col.HeaderText = col.Name;
				col.Width = 60;
				col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
				col.SortMode = DataGridViewColumnSortMode.NotSortable;
				col.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
				col.DataPropertyName = col.Name;

				g.Columns.Add(col);
			}

			for(int i = 0; i < 2; i++)
			{
				DataGridViewColumn c = g.Columns[mDt.Columns[i].ColumnName];
				c.ReadOnly = true;
				//c.Frozen = true;
				c.DefaultCellStyle.BackColor = SystemColors.ButtonFace;
				c.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
			}

		}
		


		#endregion



	}
}
