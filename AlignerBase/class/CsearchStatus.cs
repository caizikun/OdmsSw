using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Neon.Aligner
{



    /// <summary>
    /// 2D Searching status.
    /// </summary>
    public class CsearchStatus
    {
        const double minMw = 1e-7;
        public CalignPoint2d pos { get; set; }

        double _power = minMw;
        public double pwr
        {
            get => _power;
            set => _power = value < minMw ? minMw : value;
        }

        public List<CalignPoint2d> posList { get; set; }
        public List<double> pwrList { get; set; }

        public void Add(double x, double y, double power)
        {
            //if (power < 1e-9) return;
            //if (double.IsNaN(power)) return;
            //if (double.IsNegativeInfinity(power)) return;

            pos.x = x;
            pos.y = y;
            pwr = power;

            posList.Add(new CalignPoint2d(x, y));
            pwrList.Add(power);
        }



        public CsearchStatus()
        {
            pos = new CalignPoint2d();
            posList = new List<CalignPoint2d>();
            pwrList = new List<double>();
        }


        /// <summary>
        /// clean up.
        /// </summary>
        public void Clear()
        {
            pos.x = 0;
            pos.y = 0;
            pwr = 0;
            posList.Clear();
            pwrList.Clear();
        }


    }


}