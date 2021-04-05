using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
    public class TestDs : ITestDs
    {
        public int PhysicalSensorCount => 2;

        public double avg { get; set; } = 4.5;
        public double noise { get; set; } = 0.005;
        public double dv { get; set; } = 0.01;
        public double rx { get; set; } = 60000;
        public double ry { get; set; } = 60000;
        public double rz { get; set; } = 150000;
        public double dx { get; set; } = 20000;
        public double dy { get; set; } = 2000;

        public TestDs()
        {
            _appState = new Dictionary<SensorID, bool> { { SensorID.Left, false }, { SensorID.Right, false } };
            _angState = new Dictionary<SensorID, bool> { { SensorID.Left, false }, { SensorID.Right, false } };
        }
        public bool Init(DaqBase daq, int[] ai) { return true; }

        Dictionary<SensorID, bool> _appState = new Dictionary<SensorID, bool>();
        Dictionary<SensorID, bool> _angState = new Dictionary<SensorID, bool>();
        Random _r = new Random();

        public void Angle(SensorID sensorID, double distance_deg, double dx)
        {
            _appState[sensorID] = true;
            _count = (int)(distance_deg / dx);
            _dx = dx;
        }

        int _count = 0;
        double _dx = 0;
        public void Approach(SensorID sensorID, double distance_um, double dx)
        {
            _appState[sensorID] = true;
            _count = (int)(distance_um / dx);
            _dx = dx;
        }

        public double ReadDist(SensorID sensorID)
        {
            _count--;
            var volt = (_r.NextDouble() - 0.5) * noise;
            if (_appState[sensorID] && _count <= 0) volt += dv * _dx * _count;
            if (_angState[sensorID]) volt -= dv * _dx * Math.Abs(_count);
            return volt;
        }

        public void Reset()
        {
            _appState[SensorID.Left] = false;
            _appState[SensorID.Right] = false;
            _angState[SensorID.Left] = false;
            _angState[SensorID.Right] = false;

        }
    }//class
}
