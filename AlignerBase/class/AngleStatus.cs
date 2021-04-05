using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neon.Aligner
{



    /// <summary>
    /// FA-Arrange 상태를 나타냄.
    /// </summary>
    public class AngleStatus
    {
        public double pos { get; set; }     //z position
        public double sens { get; set; }    //distance sens value.
        public List<double> posList { get; set; }
        public List<double> sensList { get; set; }

        AppStageId stage;
        AngleAxis axis;

        public override string ToString()
        {
            return $"{stage}.{axis}";
        }

        /// <summary>
        /// 생성자.
        /// </summary>
        public AngleStatus(AppStageId stageId, AngleAxis axisId)
        {
            stage = stageId;
            axis = axisId;
            pos = 0;
            sens = 0;
            posList = new List<double>();
            sensList = new List<double>();
        }


        /// <summary>
        /// Clear
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