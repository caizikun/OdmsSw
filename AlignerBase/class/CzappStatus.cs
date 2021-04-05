using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Neon.Aligner
{



    /// <summary>
    /// Zapproach 상태와 결과를 나타냄.
    /// </summary>
    public class CzappStatus
    {
        public double pos { get; set; }     //current postion.
        public double sens { get; set; }        //current sensor value.
        public double noContSens { get; set; }  //contact 되지 않았을때 sensor value.
        public List<double> posList { get; set; }
        public List<double> sensList { get; set; }


        /// <summary>
        /// 생성자.
        /// </summary>
        public CzappStatus()
        {

            pos = 0;
            sens = 0;
            noContSens = 0;
            posList = new List<double>();
            sensList = new List<double>();
        }


        /// <summary>
        /// clear.
        /// </summary>
        public void Clear()
        {
            pos = 0;
            sens = 0;
            posList.Clear();
            sensList.Clear();
        }


    }


}