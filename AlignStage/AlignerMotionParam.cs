using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{
    /// <summary>
	/// 각 축에 대한 McMotionParam을 포함, 관리
	/// </summary>
	public class AlignerMotionParam
    {
        //AlignStage contains this AsMotionParam
        Aligner mAlignStage;

        //McMotionParam for each AlignAxis
        public Dictionary<AlignAxis, McMotionParam> mParamMap;

        public AlignerMotionParam(Aligner aligner)
        {
            mAlignStage = aligner;
            mParamMap = new Dictionary<AlignAxis, McMotionParam>();

            //AlignAxis[] axes = EnumToArray<AlignAxis>.ValueArray;
            foreach (AlignAxis alignAxis in mAlignStage.EffectiveAxis)
            //foreach(AlignAxis alignAxis in axes)
            {
                //if (alignAxis == AlignAxis.None) continue;
                //if (alignAxis == AlignAxis.All) continue;

                McAxis mcAxis = mAlignStage.getMcAxis(alignAxis);
                if (mcAxis == McAxis.None) continue;
                IMc mc = mAlignStage.getMc(alignAxis);

                McMotionParam param = new McMotionParam(mc, mcAxis);

                mParamMap.Add(alignAxis, param);
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (AlignAxis axis in mParamMap.Keys)
            {
                sb.AppendLine(string.Format("[{0}] : {1}", axis, mParamMap[axis].ToString()));
            }
            return sb.ToString();
        }

    }

}
