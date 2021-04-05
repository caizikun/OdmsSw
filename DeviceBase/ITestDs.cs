using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public interface ITestDs : IDispSensor
    {
        double avg { get; set; }
        double noise { get; set; }
        double dv { get; set; }//1num당 전압 차

        double rx { get; set; }//angle()시 반경
        double ry { get; set; }//angle()시 반경
        double rz { get; set; }//angle()시 반경
        double dx { get; set; }//angle(): chip width
        double dy { get; set; }//angle(): chip thickness 

        //
        void Reset();
        void Approach(SensorID sensorID, double distance_um, double dx);
        void Angle(SensorID sensorID, double angle_deg, double dx);// -angle ~ +angle

    }
}
