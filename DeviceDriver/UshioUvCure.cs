using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DrBae.TnM.Device;

namespace Neon.Aligner
{
	public class UshioUvCure : IUvCure
    {

		Daq mDaq = null;
		int mLine;

		public bool Init(Daq daq, int dev, int port, int line)
		{
			if (daq == null) return false;
			bool result = true;

			mDaq = daq;
			mLine = line;

			try
			{
				mDaq.CreateDoCh(dev, port, line);
			}
			catch
			{
				result = false;
			}
			
			return result;
		}


		public void OpenShutter()
		{
			if (mDaq == null) return;
			mDaq.WriteDo(mLine, true);
			Thread.Sleep(500);
			mDaq.WriteDo(mLine, false);
		}

		public void CloseShutter()
		{
			if (mDaq == null) return;
			mDaq.WriteDo(mLine, false);
		}

	}
}
