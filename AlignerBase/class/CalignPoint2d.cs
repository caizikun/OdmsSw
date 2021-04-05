using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;


namespace Neon.Aligner
{



    /// <summary>
    /// 2D point for alignment.
    /// </summary>
    public class CalignPoint2d
    {
        public double x { get; set; }
        public double y { get; set; }

        public CalignPoint2d()
        {
            x = 0;
            y = 0;
        }

        public CalignPoint2d(double _x, double _y)
        {
            x = _x;
            y = _y;
        }


        public CalignPoint2d(CalignPoint2d pt)
        {
            try
            {
                x = pt.x;
                y = pt.y;
            }
            catch
            {
                //do nothing.
            }
        }


    }


}