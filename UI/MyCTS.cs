using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.Aligner.UI
{
    public class MyCTS : CancellationTokenSource
    {
        public bool IsDisposed { get; private set; } = false;
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            IsDisposed = true;
        }
        public new void Cancel()
        {
            if(!IsDisposed) base.Cancel();
        }

    }//class
}
