using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{

    /// <summary>
    /// AlignAxis ~ (Mc, McAxis) 사이 변환
    /// </summary>
    public class AlignerMcTransform
    {

        #region === Instance Data ===

        //AlignStage contains this IMc
        private Aligner mAligner;

        //Axis mapped
        public AlignAxis alignAxis;
        public IMc mc;
        public McAxis mcAxis;

        #endregion



        #region === Instance Methods ===

        public AlignerMcTransform(Aligner aligner, AlignAxis axis, IMc mc, McAxis mcAxis)
        {
            this.mAligner = aligner;
            this.alignAxis = axis;
            this.mc = mc;
            this.mcAxis = mcAxis;
        }
        public AlignerMcTransform(Aligner aligner, IMc mc, McAxis mcAxis)
        {
            this.mAligner = aligner;
            this.alignAxis = aligner.getAlignAxis(mc, mcAxis);
            this.mc = mc;
            this.mcAxis = mcAxis;
        }

        public override int GetHashCode()
        {
            return ((int)mc.Id) << 16 + (int)mcAxis;
        }

        public override bool Equals(object obj)
        {
            AlignerMcTransform trans = obj as AlignerMcTransform;
            return alignAxis == trans.alignAxis && mc.Id == trans.mc.Id && mcAxis == trans.mcAxis;
        }


        public override string ToString()
        {
            return string.Format("{0}.{1}", mc.Id, mcAxis);
        }


        #endregion


    }
}
