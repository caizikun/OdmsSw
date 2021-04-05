using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Neon.Aligner;


public enum AlignState
{
    ChipMove,
    AlignPass,
    AlignFail
}




public class LogItem
{

    private static int mBarChipCount;
    private StreamWriter mSW;


    public CStageAbsPos  mPosIn;
    public CStageAbsPos  mPosOut;
    public CStageAbsPos  mPosCenter;
    public double[] mPwr;




    public LogItem(string BarChipNum, string MsrForm)
    {
        mBarChipCount = 0;
        var msrDate = System.DateTime.Now;
        mPwr = new double[4];
        mPosIn = new CStageAbsPos ();
        mPosOut = new CStageAbsPos ();
        mPosCenter = new CStageAbsPos ();

        string folderPath = AppDomain.CurrentDomain.BaseDirectory + @"\LogItem";
        var directoryCheck = new DirectoryInfo(folderPath);
        if (directoryCheck.Exists == false)
        {
            directoryCheck.Create();
        }

        string fileName = folderPath + string.Format(@"\{0}_{1}.txt", msrDate.ToString("yyMMdd_HHmmss"), BarChipNum);
        mSW = new StreamWriter(fileName, true);

        mSW.Write(MsrForm);
        mSW.WriteLine();

        string ColHeaders = "ChipNum\tBarChipNum\tMsrTime\tAlignState\tLeft_X\tLeft_Y\tLeft_Z\tLeft_TX\tLeft_TY\tLeft_TZ\tRight_X\tRight_Y\tRight_Z\tRight_TX\tRight_TY\tRight_TZ\tCenter_X\tPwrPort1\tPwrPort2\tPwrPort3\tPwrPort4";

        mSW.Write(ColHeaders);
        mSW.WriteLine();
        mSW.Flush();

        mSW.AutoFlush = true;

    }




    public void RecordLogItem(AlignState alignState)
    {

        var NowTime = System.DateTime.Now.ToLongTimeString();
        if (alignState == AlignState.ChipMove)
        {
            mBarChipCount += 1;
        }

        string aa = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}"
                                  , alignState.ToString(), mBarChipCount.ToString(), NowTime, ""
                                  , mPosIn.x, mPosIn.y, mPosIn.z, mPosIn.tx, mPosIn.ty, mPosIn.tz
                                  , mPosOut.x, mPosOut.y, mPosOut.z, mPosOut.tx, mPosOut.ty, mPosOut.tz
                                  , mPosCenter.x);

        mSW.Write(aa);
        mSW.WriteLine();
        mSW.Flush();

    }




    public void RecordLogItem(string ChipNum, string Comment)
    {
        var NowTime = System.DateTime.Now.ToLongTimeString();

        string aa = string.Format("{0}\t{1}\t{2}\t{3}"
                                  , ChipNum, mBarChipCount.ToString(), NowTime, Comment);

        mSW.Write(aa);
        mSW.WriteLine();
        mSW.Flush();

    }



}

