using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.NI4882;
using System.Threading;

namespace Neon.Aligner
{
    public class PcPsg100 : IpolController
    {
        Device mDevice;

        public PcPsg100()
        {
        }

        void write(string cmd)//, bool wait)
        {
            mDevice.Write(cmd);

            ///PSG100은 *OPC 명령을 지원 안함.
            //if (!wait) return;
            //for (int i = 0; i < 100; i++)
            //{
            //    m_gpibDev.Write("*OPC?");
            //    if (m_gpibDev.ReadString().Contains("1")) return;
            //}
            //throw new Exception($"PcPsg100.write(\"{cmd}\"): <OPC> failed 100 times.");
        }
        string read()
        {
            return mDevice.ReadString().Trim();
        }
        string query(string cmd)
        {
            try
            {
                Monitor.Enter(mDevice);
                write(cmd);
                return read();
            }
            finally
            {
                Monitor.Exit(mDevice);
            }
        }

        public bool Connect(int gpibPrimaryAddress)
        {
            //gpib 객체 생성 및 연결
            mDevice = new Device(0, Convert.ToByte(gpibPrimaryAddress));

            //Identification을 물어본다.
            return query("*IDN?").Contains("PSG100");
        }

        #region ---- Not Supported ----
        public void SetPolFilterPos(double _pos)
        {
            return;
            //throw new NotSupportedException("PcPsg100: SetPolFilterPos() is not supported.");
        }

        public double GetPolFilterPos()
        {
            return 0;
            //throw new NotSupportedException("PcPsg100: GetPolFilterPos() is not supported.");
        }

        public void SetHalfRetarderPos(double _pos)
        {
            return;
            //throw new NotSupportedException("PcPsg100: SetHalfRetarderPos() is not supported.");
        }

        public double GetHalfRetarderPos()
        {
            return 0;
            //throw new NotSupportedException("PcPsg100: GetHalfRetarderPos() is not supported.");
        }

        public void SetQuarRetarderPos(double _pos)
        {
            return;
            //throw new NotSupportedException("PcPsg100: SetQuarRetarderPos() is not supported.");
        }

        public double GetQuarRetarderPos()
        {
            return 0;
            //throw new NotSupportedException("PcPsg100: GetQuarRetarderPos() is not supported.");
        }
        #endregion

        static readonly string[] sCmdString = { "LHP", "LVP", "LP45P", "RCP", "LCP", "LM45P" };

        public void SetToLinearHorizontal(double _polPos = 0)
        {
            write(sCmdString[0]);
            checkPolState(sCmdString[0]);
        }

        public void SetToLinearVertical(double _polPos = 0)
        {
            write(sCmdString[1]);
            checkPolState(sCmdString[1]);
        }

        public void SetToLinearDiagonal(double _polPos = 0)
        {
            write(sCmdString[2]);
            checkPolState(sCmdString[2]);
        }

        public void SetToRHcircular(double _polPos = 0)
        {
            write(sCmdString[3]);
            checkPolState(sCmdString[3]);
        }


        public void SetToLHcircular(double polBaseAngle = 0)
        {
            write(sCmdString[4]);
            checkPolState(sCmdString[4]);
        }
        public void SetToNegaLinearDiagonal(double polBaseAngle = 0)
        {
            write(sCmdString[5]);
            checkPolState(sCmdString[5]);
        }


        void checkPolState(string state)
        {
            if (!query("PSG?").Contains(state)) throw new Exception($"PcPsg100.readPolState() failed: <{state}>");
        }

    }//class


}
