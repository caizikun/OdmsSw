using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Free302.TnMLibrary.DataAnalysis;


public class DutData
{
    public int mWaveSart;
    public int mWaveStop;
    public double mWaveStep;
    public int NumDataPoints { get; private set; }
    public int NumCh { get { return mTrans == null ? 0 : mTrans.Count; } }
    public int[] Channels { get { return mTrans.Select(x => x.Port).ToArray(); } }

    public List<PortPowers> mTrans;
    public List<PortPowers> mPower;


    public static double LastCladPower = 1e-8;//mW

    public DutData(int wlStart_nm, int wlStop_nm, double wlStep_nm, int numDp) : this(wlStart_nm, wlStop_nm,wlStep_nm)
    {
        NumDataPoints = numDp;
    }

    public DutData(int wlStart_nm, int wlStop_nm, double wlStep_nm)
    {
        mTrans = new List<PortPowers>();

        mWaveSart = wlStart_nm;
        mWaveStop = wlStop_nm;
        mWaveStep = wlStep_nm;
        NumDataPoints = (int)((mWaveStop - mWaveSart) / mWaveStep) + 1;
    }

    public void AddTrans(PortPowers portPowers)
    {
        if (mTrans.Find(x => x.Port == portPowers.Port) != null) throw new Exception($"{portPowers.Port} port번호가 이미 존재합니다.");
        mTrans.Add(portPowers);
    }
    

    public PortPowers GetPolLossOf(int port)
    {
        var value = mTrans.Find(x => x.Port == port);
        if (value == null)
            throw new IndexOutOfRangeException($"DutData.GerPortLoss():\nport=<{port} 데이터가 없습니다.>");
        return value;
    }

    public List<double> GetNonPolLossOf(int port)
    {
        var value = mTrans.Find(p => p.Port == port).NonPol;
        if (value == null)
            throw new IndexOutOfRangeException($"DutData.GerPortLoss():\nport=<{port} 데이터가 없습니다.>");
        return value;
    }

    public void Subtract(DutData sub)
    {
        for (int i = 0; i < mTrans.Count; i++) mTrans[i].Subtract(sub.mTrans[i]);
    }
    public void Subtract(double sub)
    {
        for (int i = 0; i < mTrans.Count; i++)
            for (int j = 0; j < mTrans[i].NonPol.Count; j++)
            {
                mTrans[i].NonPol[j] -= sub;
            }
    }


    /// <summary>
    /// text file에 저장한다.(full range)
    /// </summary>
    /// <param name="_filepath">file path</param>
    /// <returns></returns>
    public void WriteTransmitance(string _filePath, double[] _monitorIL = null)
    {
        StringBuilder line = new StringBuilder();
        StreamWriter sw = null;

        try
        {
            //File Open
            sw = new StreamWriter(_filePath, false, Encoding.ASCII);
            int polNums = mTrans.Count();

            //data
            double wl = mWaveSart;   //wavelength [nm]
            for (int i = 0; i < NumDataPoints; i++)
            {
                line.Clear();
                //wavelength
                line.Append($"{wl:F3}, ");

                //data
                double min, max, avg, pdl;
                for (int ch = 0; ch < NumCh; ch++)
                {
                    if (polNums == 1) line.Append($"{mTrans[ch].NonPol[i]:F3}");
                    else
                    {
                        max = mTrans[ch].Max[i];
                        min = mTrans[ch].Min[i];
                        avg = Unit.WattTodBm((Unit.dBmToWatt(min) + Unit.dBmToWatt(max)) / 2);
                        pdl = max - min;

                        line.Append($"{max:F3}, {min:F3}, {avg:F3}, {pdl:F3}, ");
                    }
                }
                line.Remove(line.Length - 2, 2);
                sw.WriteLine(line);
                wl += mWaveStep;
            }
            //monitor IL값 저장****************************
            if (_monitorIL != null)
            {
                var strLineBuf = "#";
                for (int i = 0; i < _monitorIL.Length; i++)
                    strLineBuf += Convert.ToString(_monitorIL[i]) + ',';

                sw.WriteLine(strLineBuf);
            }
        }        
        finally
        {
            if (sw != null) sw.Close();
        }
    }


    public void WritePower(string filePath)
    {
        StringBuilder line = new StringBuilder();
        StreamWriter sw = null;

        try
        {
            //File Open
            sw = new StreamWriter(filePath, false, Encoding.ASCII);

            //data
            double wl = mWaveSart;   //wavelength [nm]
            for (int i = 0; i < NumDataPoints; i++)
            {
                line.Clear();
                //wavelength
                line.Append($"{wl:F3}, ");

                //data
                for (int ch = 0; ch < NumCh; ch++)
                    line.Append($"{mPower[ch][0][i]:F9}, {mPower[ch][1][i]:F9}, {mPower[ch][2][i]:F9}, {mPower[ch][3][i]:F9}, ");
                line.Remove(line.Length - 2, 2);
                sw.WriteLine(line);
                wl += mWaveStep;
            }
        }
        finally
        {
            if (sw != null) sw.Close();
        }
    }


}



