using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Aligner
{
    public partial class AlignLogic
    {
        public volatile bool mStopFlag;           //작업중지 시키려면 True 시킨다. 

        private int mFuncId;
        public int CurFuncNo
        {
            get { return mFuncId; }
            private set
            {
                mFuncId = value;
                if (mFuncId != NOOPERATION) AlignStarted?.Invoke(mFuncId);
            }
        }

        public int CurStageNo { get; private set; }
        public int CurAxisNo { get; private set; }

        private bool mAlignCompleted;
        private bool mCompleted
        {
            get { return mAlignCompleted; }
            set
            {
                mAlignCompleted = value;
                if (value && IsEventEnable) AlignCompleted?.Invoke(mFuncId.ToString());
                mFuncId = NOOPERATION;
            }
        }


        private bool doStopChecking()
        {
            if (mStopFlag) return true;
            else return false;            
        }


        Dictionary<AppStageId, Dictionary<AngleAxis, AngleStatus>> _angleStatus;
        void initAngleStatus()
        {
            _angleStatus = new Dictionary<AppStageId, Dictionary<AngleAxis, AngleStatus>>();
            var d = new Dictionary<AngleAxis, AngleStatus>();
            d.Add(AngleAxis.Tx, AlignStatusPool.faTxIn);
            d.Add(AngleAxis.Ty, AlignStatusPool.faTyIn);
            _angleStatus.Add(AppStageId.Left, d);

            d = new Dictionary<AngleAxis, AngleStatus>();
            d.Add(AngleAxis.Tx, AlignStatusPool.faTxOut);
            d.Add(AngleAxis.Ty, AlignStatusPool.faTyOut);
            _angleStatus.Add(AppStageId.Right, d);
        }



    }//class
}
