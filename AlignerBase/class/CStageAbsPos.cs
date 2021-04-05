using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{

    public class CStageAbsPos  : ICloneable
    {
        public double x = double.NaN;
        public double y = double.NaN;
        public double z = double.NaN;
        public double tx = double.NaN;
        public double ty = double.NaN;
        public double tz = double.NaN;

        public object Clone()
        {
            CStageAbsPos  ret = new CStageAbsPos ();
            ret.x = this.x;
            ret.y = this.y;
            ret.z = this.z;
            ret.tx = this.tx;
            ret.ty = this.ty;
            ret.tz = this.tz;

            return ret;
        }

        public void Clear()
        {
            x = y = z = tx = ty = tz = 0;
        }

        public void SetValue(double[] coord)
        {
            var len = coord.Length;
            if (len >= 1) x = coord[0];
            if (len >= 2) y = coord[1];
            if (len >= 3) z = coord[2];
            if (len >= 4) tx = coord[3];
            if (len >= 5) ty = coord[4];
            if (len >= 6) tz = coord[5];

        }

        public void CopyTo(CStageAbsPos target)
        {
            target.x = x;
            target.y = y;
            target.z = z;
            target.tx = tx;
            target.ty = ty;
            target.tz = tz;
        }

        public bool IsValid
        {
            get
            {
                return !double.IsNaN(x) && !double.IsNaN(y) && !double.IsNaN(z) && !double.IsNaN(tx) && !double.IsNaN(ty) && !double.IsNaN(tz);
            }
        }

    }


    /// <summary>
    /// 모든 Aligner 의 포지션 저장
    /// </summary>
    public class AlignPosition
    {
        public int chipIndex;
        public CStageAbsPos  In;
        public CStageAbsPos  Out;
        public CStageAbsPos  Other;

        public AlignPosition()
        {
            In = new CStageAbsPos ();
            Out = new CStageAbsPos ();
            Other = new CStageAbsPos ();
        }

        public double[] InValues { get { return new double[] { In.x, In.y, In.z, In.tx, In.ty, In.tz }; } }
        public double[] OutValues { get { return new double[] { In.x, In.y, In.z, In.tx, In.ty, In.tz }; } }
        public double[] OtherValues { get { return new double[] { Other.x, Other.y, Other.z, Other.tx, Other.ty, Other.tz }; } }

        public double[] CloseValues { get { return new double[] { In.z, Out.z }; } }
        public void SetCloseValue(double[] values)
        {
            In.z = values[0];
            Out.z = values[1];
        }
    }

}
