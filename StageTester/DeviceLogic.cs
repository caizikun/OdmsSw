using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

//using free302;
using Free302.TnM.Device;
using Free302.MyLibrary.Utility;
using Neon.Aligner;


namespace Free302.TnM.Device.StageTester
{

    class DeviceLogic
	{

		#region === Data Members ===

		Properties.Settings set = Properties.Settings.Default;

		//align stage
		AlignerBase mAligner;
		Dictionary<IFA_AlignerId, AlignerBase> mAligners;

		//version 
		Assembly mLibraryAssembly;
		Dictionary<Type, Assembly> mAssembly;
		public Assembly libraryAssembly { get { return mLibraryAssembly; } }

		#endregion 

		

		#region === constructor open close ===

		public DeviceLogic()
        {
			mAligners = new Dictionary<IFA_AlignerId, AlignerBase>(4);
			mAligners.Add(IFA_AlignerId.Test, new AlignerBase(IFA_AlignerId.Test, "TestAligner"));
			mAligners.Add(IFA_AlignerId.Center, new CenterAligner());
			mAligners.Add(IFA_AlignerId.Left, new LeftAligner());
			mAligners.Add(IFA_AlignerId.Right, new RightAligner());

			mAligner = mAligners[IFA_AlignerId.Test];

			//SW info
			mAssembly = new Dictionary<Type, Assembly>();
			Type type = typeof(Aligner);
			mAssembly.Add(type, Assembly.GetAssembly(type));
			type = typeof(AlignerBase);
			mAssembly.Add(type, Assembly.GetAssembly(type));
			this.mLibraryAssembly = Assembly.GetAssembly(type);
        }

		public AlignerBase selectAligner(IFA_AlignerId id)
		{
			this.mAligner = this.mAligners[id];
			return mAligner;
		}

		
		public string open()
		{
			StringBuilder sb = new StringBuilder();

			if (!mAligner.IsOpen) mAligner.Open();

			sb.Append(mAligner.ToString());

			return sb.ToString();
		}

        public string close()
        {
            StringBuilder sb = new StringBuilder();

			if (!mAligner.IsOpen)
			{
				sb.AppendFormat("{0} is not opened.", mAligner.ToString());
				return sb.ToString();
			}

			mAligner.Close();
			
            return sb.ToString();
        }

		#endregion



		#region === Drive Status Methods ===

		public void setReporter(IProgress<AlignerMotionParam> reporter)
		{
			if (!mAligner.IsOpen) return;

			AlignerMotionParam param = new AlignerMotionParam(this.mAligner);

			mAligner.reportStatusAsync(param, reporter);

			//if (reporter != null) reporter.Report(param);
		}
		public void testMoveAsync()
        {
			mAligner.RelMove((int)AlignAxis.Z, -3000, 1000);
        }

		public string testIsMoving()
		{
			bool isMoving = mAligner.IsMovingOK();
			return $"is moving : {isMoving}";
		}

		#endregion




		#region === Drive Methods ===

		public bool IsOpen
		{
			get { return mAligner.IsOpen; }
		}

		public void moveAxis(AlignAxis axis, double dx)
		{
			if (!mAligner.IsOpen) return;

			mAligner.MoveFast(axis, dx);
		}


		#endregion




		#region === Simple Operations ===

		public void zeroing()
		{
			if (!mAligner.IsOpen) return;

			Stopwatch watch = Stopwatch.StartNew();

			mAligner.Zeroing();
            mAligner.Homing();

            watch.Stop();
			MessageBox.Show($"Zeroing complete @ {watch.ElapsedMilliseconds / 1000} sec.");

		}
		public void zeroing(AlignAxis axis)
		{
			if(!mAligner.IsOpen) return;

            while (mAligner.IsMovingOK()) ;

            Stopwatch watch = Stopwatch.StartNew();
			mAligner.Zeroing((int)axis);

			MessageBox.Show($"Zeroing <{axis}> complete @ {watch.ElapsedMilliseconds / 1000} sec.");
		}
		public void homing()
		{
			if (!mAligner.IsOpen) return;

			mAligner.Homing();
		}
		public void homing(AlignAxis axis)
		{
			if(!mAligner.IsOpen) return;

			mAligner.Homing((int)axis);
		}

		public void stop()
        {
			if (!mAligner.IsOpen) return;

			mAligner.StopMove();			
        }
		public void stop(AlignAxis axis)
		{
			if(!mAligner.IsOpen) return;

			mAligner.StopMove((int)axis);
		}

		#endregion



		#region === UI Configuration ==

		public string[] getAxisTitle()
        {
            return set.AxisTitle.Unpack<string>().ToArray();
            //return config.getStringArray("AxisTitle");
        }
        public string[] getStatusTitle()
        {
            return set.GridStatusColumnTitle.Unpack<string>().ToArray();
            //return config.getStringArray("StatusTitle");
        }

		#endregion




		#region === TesterForm ===

		public void testTesterForm()
		{
			SpeedTesterForm testerForm = SpeedTesterForm.Self;

			testerForm.registerAligner(IFA_AlignerId.Test, mAligners[IFA_AlignerId.Test]);

			testerForm.registerAligner(mAligners[IFA_AlignerId.Left], mAligners[IFA_AlignerId.Right], 
				mAligners[IFA_AlignerId.Center]);

			testerForm.Show();

			//mAligners[IFA_AlignerId.Left].Open();
			//mAligners[IFA_AlignerId.Right].Open();


		}

		#endregion

	}

    

}
