using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TnM.Utility;

namespace Neon.Aligner
{
    public enum ScanDirection { BiDir, Positive, Negative }

    public class ScanParam
    {
        public int StageNo;
        public int AxisNo;
        public double Range;
        public double Step;
        public int Port;

        public bool MoveToCenter = false;//false=move to peak, true=move to center
        public bool DoFinePeak = true;
        public static double FinePeakStep = 1.0;
        public ScanDirection Dir = ScanDirection.BiDir;


        public ScanParam(int stage, int axis, double range, double step, int port)
        {
            StageNo = stage;
            AxisNo = axis;
            Range = range;
            Step = step;
            Port = port;
        }

        public Action<int, int, double> MoveFunc { get; set; }
        public Func<int, int, double> PositionFunc { get; set; }//read coord 
        public Func<int, double> PowerFunc { get; set; }

        public void Move(double distance)
        {
            MoveFunc(StageNo, AxisNo, distance);
        }
        public double Position()
        {
            return PositionFunc(StageNo, AxisNo);
        }
        public double Power()
        {
            //return Math.Round(Unit.MillWatt2Dbm(PowerFunc(Port)), 3);
            return Math.Round(10 * Math.Log10(PowerFunc(Port)), 3);
        }

        public Action<string> ReportFunc { get; set; }//report message
        public Action<double, double> UpdateFunc { get; set; }//report current coordinates

        public void MoveTo(double peakCoord)
        {
            var currentCoord = Position();
            var distance = peakCoord - currentCoord;

            Move(distance);
        }

        public Func<bool> StopCheckFunc { get; set; }

    }//class

    public class ScanData
    {
        public List<double> Coord;
        public List<double> Power;

        public ScanData()
        {
            Coord = new List<double>();
            Power = new List<double>();
        }

        public void Add(double coord, double power)
        {
            Coord.Add(coord);
            Power.Add(power);
        }
        public int FindPeakIndex()
        {
            if (Power.Count < 1) return 0;
            return Power.IndexOf(Power.Max());
        }

        public double FindPeakCoord()
        {
            if (Power.Count < 1) return double.NaN;
            var max = Power.Max();
            var maxIndex = Power.IndexOf(max);
            var maxCoord = Coord[maxIndex];

            return maxCoord;
        }

        public double FindCenterCoord()
        {
            //const double lpf = 0.5;  //Low pass filter 상수.
            //var data = Jeffsoft.JeffMath.LowPassFilter(Power.ToArray(), lpf).Select((x) => Math.Round(x, 3)).ToList();
            //var centerIndex = (int)Math.Round(AlignLogic.CalcCtrPos(data, 1));

            if (Coord == null || Coord.Count == 0) return double.NaN;

            //var centerIndex = (int)Math.Round(AlignLogic.CalcCtrPos(Power, 1));
            var centerIndex = (int)Math.Round(DataProcessingLogic.CalcCenter(Power, 2 * Power.Count / 3)[0]);

            return (Coord.Count < centerIndex + 1) ? double.NaN : Coord[centerIndex];
        }
    }

    public class XYSearchParam
    {
        public bool Run = true; //서치 실행 여부
        public bool SearchByScan = true;//정렬방식: scan=true or hill=false
        public bool ScanToCenter = false;//scan 방식: peak=false or center=true

        public int StageNo;
        public int Port;

        public double RangeX;
        public double StepX;
        public double RangeY;
        public double StepY;

        public double RangeScaleFactor = 1.0;
        public static double LastPeakPower = -200;

        private XYSearchParam()
        {
            StageNo = 1;
            Port = 1;
        }
        public void Set(double[] values)
        {
            RangeX = values[0];
            StepX = values[1];
            RangeY = values[2];
            StepY = values[3];

            if (values.Length == 7)
            {
                SearchByScan = values[4] != 0;
                ScanToCenter = values[5] != 0;
                Port = (int)Math.Round(values[6]);
            }
        }
        public static XYSearchParam Create(double[] values)
        {
            var instance = new XYSearchParam();
            instance.Set(values);
            return instance;
        }
        public static XYSearchParam Create(string packed)
        {
            var instance = new XYSearchParam();
            instance.Unpack(packed);
            return instance;
        }

        public string Pack()
        {
            return new double[] { RangeX, StepX, RangeY, StepY, SearchByScan ? 1 : 0, ScanToCenter ? 1 : 0, Port }.Pack();
        }
        public void Unpack(string packed)
        {
            Set(packed.Unpack<double>().ToArray());
        }

        public XYSearchParam Clone()
        {
            var clone = new XYSearchParam();
            clone.Run = Run;
            clone.SearchByScan = SearchByScan;
            clone.ScanToCenter = ScanToCenter;
            clone.StageNo = StageNo;
            clone.Port = Port;
            clone.RangeX = RangeX;
            clone.StepX = StepX;
            clone.RangeY = RangeY;
            clone.StepY = StepY;
            clone.RangeScaleFactor = RangeScaleFactor;
            return clone;
        }
    }

}
