using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Free302.MyLibrary.Utility;

namespace Neon.Aligner
{
    public enum PmPol { P1 = 0, P2 = 1, P3 = 2, P4 = 3, Min = 0, Max = 1, NonPol = 0 }

    public class PortPowers
    {
        public int Port;
        List<List<double>> mPowers;

        public PortPowers(int port, List<double> power) : this(port, 1)
        {
            NonPol = power;
        }
        public PortPowers(int port, int numPols = 4)
        {
            this.Port = port;
            mPowers = new List<List<double>>();
            NumPols = numPols;
        }

        const double MinPower_mW = 1e-8;//-80dBm

        public int NumPols
        {
            get { return mPowers.Count; }
            set
            {
                mPowers.Clear();
                for (int i = 0; i < value; i++) mPowers.Add(new List<double>());
            }
        }

        public List<double> Min { get { return mPowers[0]; } }    //high transmission 
        public List<double> Max { get { return mPowers[1]; } }    //low transmission 
        public List<double> NonPol
        {
            get { return mPowers[0]; }
            set { mPowers[0] = value; }
        }

        public void AddPowers(List<double> portPowers, PmPol pmPol)
        {
            //mPowers.Add(portPowers);
            mPowers[(int)pmPol] = portPowers;
        }


        public List<double> this[int index]
        {
            get { return mPowers[index]; }
            set { mPowers[index] = value; }
        }

        public List<List<double>> Data
        {
            get { return mPowers; }
        }

        public void AddPowerFromString(string[] polPowers)
        {
            for (int i = 0; i < polPowers.Length; i++) mPowers[i].Add(polPowers[i].To<double>());
        }


        public void Subtract(PortPowers sub)
        {
            if (Port != sub.Port) throw new Exception($"PortPowerPair.Subtract(): 두 port가 서로 다릅니다. {Port} != {sub.Port}");
            NonPol = NonPol.Zip(sub.NonPol, (x, y) => (x - y) < MinPower_mW ? MinPower_mW : (x - y)).ToList();
        }

        public void Subtract(double power_mW)
        {
            for (int i = 0; i < mPowers.Count; i++)
                mPowers[i] = mPowers[i].Select(x => (x - power_mW) < MinPower_mW ? MinPower_mW : (x - power_mW)).ToList();
        }

    }
}
