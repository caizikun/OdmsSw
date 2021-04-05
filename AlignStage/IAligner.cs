using System;
using System.Collections.Generic;

namespace Free302.TnM.Device
{
	public interface IAligner
	{
		/// <summary>
		/// list of AlignAxis currently connected
		/// </summary>
		List<AlignAxis> EffectiveAxis { get; }

		AlignAxis getAlignAxis(IMc mc, McAxis mcAxis);

		IMc getMc(AlignAxis axis);

		McAxis getMcAxis(AlignAxis axis);

		bool MoveFast(AlignAxis axis, double dx);

		void reportStatusAsync(AlignerMotionParam param, IProgress<AlignerMotionParam> reporter);

        //void SaveDynamicData();
	}
}