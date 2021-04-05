using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Neon.Aligner;


public class DS2000 : IdistSensor
{
    public int sensorCnt { get { return 2; } }

    private Cdaq mDaq;
    private int mInputAddress1;
    private int mInputAddress2;

    public DS2000(Cdaq daq, int s1, int s2)
    {
        mDaq = daq;
        mInputAddress1 = s1;
        mInputAddress2 = s2;
        
    }

    public bool Init()
    {
        //채널 생성.
        bool result = true;
        result &= mDaq.CreateAiCh(mInputAddress1, 0.0, 10.0);
        result &= mDaq.CreateAiCh(mInputAddress2, 0.0, 10.0);
        return result;
    }


    public double ReadDistance(SensorNumber sensorId)
    {
        if (sensorId != SensorNumber.S1 && sensorId != SensorNumber.S2) return 0;
        int address = sensorId == SensorNumber.S1 ? mInputAddress1 : mInputAddress2;
        return mDaq.ReadAi(address);
    }

    public double ReadDist(int sensorNo)
    {
        if (sensorNo == 0) return ReadDistance(SensorNumber.S1);
        else if (sensorNo == 1) return ReadDistance(SensorNumber.S2);
        else throw new InvalidOperationException($"DS2000.ReadDist(int):\n무효한 Sensor No <{sensorNo}>");
    }
    
}
