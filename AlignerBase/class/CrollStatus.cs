using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Neon.Aligner
{



    /// <summary>
    /// roll alignment status.
    /// </summary>
    public class CrollStatus
    {

        public List<double> pwrList1;
        public List<double> pwrList2;


        public double tzPos;//thetaZ axis position
        public List<double> tzPosList;//thetaZ axis position list


        /// <summary>
        /// default constructor.
        /// </summary>
        public CrollStatus()
        {
            pwrList1 = new List<double>();
            pwrList2 = new List<double>();

            tzPos = 0;
            tzPosList = new List<double>();
        }


        /// <summary>
        /// clear.
        /// </summary>
        public void Clear()
        {

            pwrList1.Clear();
            pwrList2.Clear();

            tzPos = 0;
            tzPosList.Clear();
        }

    }



}

