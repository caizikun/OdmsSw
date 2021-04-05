using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neon.Aligner;

namespace Neon.Aligner
{
    public static class AlignStatusPool
    {
        public static CzappStatus zappIn { get; } = new CzappStatus();

        public static CzappStatus zappOut { get; } = new CzappStatus();

        public static AngleStatus faTxIn { get; } = new AngleStatus(AppStageId.Left, AngleAxis.Tx);

        public static AngleStatus faTxOut { get; } = new AngleStatus(AppStageId.Right, AngleAxis.Tx);

        public static AngleStatus faTyIn { get; } = new AngleStatus(AppStageId.Left, AngleAxis.Ty);

        public static AngleStatus faTyOut { get; } = new AngleStatus(AppStageId.Right, AngleAxis.Ty);

        //xySearch
        public static CsearchStatus xySearchIn { get; } = new CsearchStatus();

        public static CsearchStatus xySearchOut { get; } = new CsearchStatus();

        //xyBlindSearch
        public static CsearchStatus xyBlindSearchIn { get; } = new CsearchStatus();

        public static CsearchStatus xyBlindSearchOut { get; } = new CsearchStatus();

        //axisSearch
        public static CsearchStatus axisSearchInX { get; } = new CsearchStatus();

        public static CsearchStatus axisSearchInY { get; } = new CsearchStatus();

        public static CsearchStatus axisSearchOutX { get; } = new CsearchStatus();

        public static CsearchStatus axisSearchOutY { get; } = new CsearchStatus();

        //syncXySearch
        public static CsearchStatus syncXySearchIn { get; } = new CsearchStatus();

        public static CsearchStatus syncXySearchOut { get; } = new CsearchStatus();

        //syncAxisSearch
        public static CsearchStatus syncAxisSearchInX { get; } = new CsearchStatus();

        public static CsearchStatus syncAxisSearchInY { get; } = new CsearchStatus();

        public static CsearchStatus syncAxisSearchOutX { get; } = new CsearchStatus();

        public static CsearchStatus syncAxisSearchOutY { get; } = new CsearchStatus();

        //roll in
        public static CrollStatus rollIn { get; } = new CrollStatus();

        //roll out
        public static CrollStatus rollOut { get; } = new CrollStatus();

    }
}