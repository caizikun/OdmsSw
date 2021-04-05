using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public class AlignerStateVector
    {
        bool mDoAbort = false;
        int mRunCounter = 0;

        public bool DoAbort
        {
            get
            {
                var value = mDoAbort;
                if (mDoAbort && mRunCounter <= 0) mDoAbort = false;
                return value;
            }
            set { mDoAbort = value; }
        }

        public void Increse()
        {
            mRunCounter++;
        }
        public void Decrese()
        {
            mRunCounter--;
            if (mRunCounter <= 0)
            {
                mRunCounter = 0;
                mDoAbort = false;
            }
        }
        public bool IsRunning
        {
            get { return mRunCounter > 0; }
        }
    }//class
}
