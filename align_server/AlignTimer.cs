using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;


namespace Neon.Aligner
{
	public enum TimingAction
    {
        Measure,    //starting measure
        Start,      //starting chip
        Approach,   //starting approach
        AlignIn,  //starting aling
        AlignOut,
        Roll,       //staring roll
        SweepCore,      //starting measure
        SweepClad,      //starting measure
        SaveAndPlot, 
        MoveNext,
        Return,
        End
    };

	public class AlignTimer
	{
		static string mLogDirectory;
		const string mLogFileName = "timing";
        const string mTimeFormatForLogFileName = "yyMMdd-HHmmss";
        const string mTimeFormat = "yyyy-MM-dd HH:mm:ss";//for excel importing
        const string mTimeSpanFormat = "HH:mm:ss";
        static string mFileName;
		static StreamWriter mWriter;

		static AlignTimer()
		{
            mLogDirectory = @"log\";
            if (!Directory.Exists(mLogDirectory)) Directory.CreateDirectory(mLogDirectory);

            mWatch = new Stopwatch();
            var time = DateTime.Now.ToString(mTimeFormatForLogFileName);
			mFileName = Path.Combine(mLogDirectory, $"{mLogFileName}_{time}.txt");
			mWriter = new StreamWriter(Path.Combine(mLogDirectory, mFileName), true);
			mWriter.AutoFlush = true;
		}
		
		public static string Resolution
		{
			get { return $"{Stopwatch.Frequency}ticks/sec, Δt={1000L * 1000L * 1000L / Stopwatch.Frequency}ns"; }
		}

        static bool sInBar;
        static bool sInChip;
        static DateTime mStartTime;
        static Stopwatch mWatch;
        static string mLastSerial;

        /// <summary>
        /// load, 
        /// </summary>
        public static void StartBar(string serialNumber)
		{
			//end last bar if any
			if(sInBar) EndBar();

            mLastSerial = serialNumber;

            //init flag
            sInBar = true;

            //init watch
            mWatch.Restart();
			mStartTime = DateTime.Now;

            //record
            mWriter.WriteLine($"{serialNumber}\tBAR-START\t{mStartTime.ToString(mTimeFormat)}");

        }
		public static void EndBar()
		{
			mWatch.Stop();
            var endTime = DateTime.Now;
            var dt = (endTime - mStartTime).ToString();
            mWriter.WriteLine($"{mLastSerial}\tBAR-END\t{endTime.ToString(mTimeFormat)}\tΔt\t{dt}");
            mWriter.Flush();
			sInBar = false;
		}
		//Timing { None, Load, Measure, Approach, RollChip, Alignment, Sweep};

		//
		public static void StartChip(string chipSn)
		{
			if(sInChip) EndChip();
			sInChip = true;
			RecordTime(TimingAction.Start, chipSn);
		}
		public static void EndChip()
		{
			sInChip = false;
			RecordTime(TimingAction.End);
        }

        //{ None, Load, Measure, Approach, RollChip, Alignment, Sweep };
        public static void RecordTime(TimingAction startingAction, string prolog = "")
		{
            mWatch.Stop();
            if(prolog != "") mWriter.Write(prolog);
            mWriter.Write($"\t{mWatch.ElapsedMilliseconds}\t{startingAction}");
            if(startingAction == TimingAction.End) mWriter.WriteLine();
            mWriter.Flush();
            mWatch.Restart();
		}


	}
}