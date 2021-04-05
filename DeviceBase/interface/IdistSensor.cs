using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neon.Aligner
{
    public enum SensorID { Left, Right }

    /// <summary>
    /// distance sensor에 대한 interface.
    /// </summary>
    public interface IDispSensor
    {
        /// <summary>
        /// 총 센서 갯수.
        /// </summary>
        int PhysicalSensorCount { get; }    

        /// <summary>
        /// distance 값을 읽어들인다.
        /// </summary>
        /// <param name="_sensorNo">sensor no.</param>
        /// <returns>distance</returns>
        double ReadDist(SensorID sensorID);

    }


}
