using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Free302.MyLibrary.Utility;
using Free302.MyLibrary.Config;
using System.Reflection;
using System.IO;

namespace Free302.TnM.Device
{

    /// <summary>
    /// align stage with 6 axis
    /// Istage 를 버리고 새로운 인터페이스 구축할 것!
    /// </summary>
    public class Aligner : DeviceBase<AlignStageId>, IAligner
    {

        #region === Class Framework ===

        //생성자
        public Aligner() : this(AlignStageId.None) { }

        public Aligner(AlignStageId id, string name = DefaultName) : base(id, name)
        {
            //chirality
            this.Chirality = CoordinatesChirality.Right;

            //McMap
            //mMcMap = new Dictionary<McId, IMc>();


        }

        public override string ToString()
        {
            return string.Format("{0}, Chirality={1}", base.ToString(), this.Chirality);
        }

        #endregion



        #region === 장치 구성 - 사용자 설정 ===

        private const string DefaultName = "Aligner";


        protected CoordinatesChirality Chirality { get; set; }


        #region --- IDeviceConfig 구현 ---

        override public DeviceConfig buildConfig()
        {
            base.buildConfig();

            //CoordinatesChirality
            mConfig.Add("Chirality", this.Chirality);

            //AxisMcMap
            Dictionary<AlignAxis, McId> axisMcMap = new Dictionary<AlignAxis, McId>();
            Dictionary<AlignAxis, McAxis> axisMap = new Dictionary<AlignAxis, McAxis>();
            foreach (AlignAxis axis in mAxisTransMap.Keys)
            {
                axisMcMap.Add(axis, mAxisTransMap[axis].mc.Id);
                axisMap.Add(axis, mAxisTransMap[axis].mcAxis);
            }
            mConfig.Add("AlignAxisToMcMap", axisMcMap.Pack());

            //AxisMcAxisMap
            mConfig.Add("AlignAxisToMcAxisMap", axisMap.Pack());

            //McMap
            Dictionary<McId, string> dic = new Dictionary<McId, string>();
            foreach (AlignerMcTransform trans in mTransAxisMap.Keys)
            {
                McId id = trans.mc.Id;
                IMc mc = trans.mc;//mMcMap[id];
                Type type = mc.GetType();

                if (!dic.ContainsKey(id))
                {
                    //dic.Add(mid, type.AssemblyQualifiedName);
                    //dic.Add(mid, type.FullName);
                    if (type.Namespace != null) dic.Add(id, type.Namespace + "." + type.Name);
                    else dic.Add(id, type.Name);
                }
            }
            mConfig.Add("McMap", dic.Pack());

            //each MC
            foreach (AlignerMcTransform trans in mTransAxisMap.Keys) mConfig.CopyFrom(trans.mc.buildConfig());

            return mConfig;
        }

        public override void applyConfig(DeviceConfig newConfig)
        {
            base.applyConfig(newConfig);
            mIsConfigSet = false;

            //CoordinatesChirality
            this.Chirality = mConfig.Get<CoordinatesChirality>("Chirality");

            //AxisMap
            Dictionary<AlignAxis, McId> axisMcMap = mConfig["AlignAxisToMcMap"].Unpack<AlignAxis, McId>();
            Dictionary<AlignAxis, McAxis> axisMap = mConfig["AlignAxisToMcAxisMap"].Unpack<AlignAxis, McAxis>();

            //mcMap
            Dictionary<McId, string> dicMc = mConfig["McMap"].Unpack<McId, string>();
            Dictionary<McId, string> dicDll = mConfig["McMapDll"].Unpack<McId, string>();

            if (mMcMap == null) mMcMap = new Dictionary<McId, IMc>();
            mMcMap.Clear();

            //each MC instace, applyConfig()
            foreach (McId id in dicMc.Keys)
            {
                var mcTypeName = dicMc[id];
                var mcAssemblyName = dicDll[id];
                //Assembly.LoadFrom(Path.Combine(dir, $"{mcAssemblyName}.dll"));

                IMc mc = (IMc)Activator.CreateInstance(mcAssemblyName, mcTypeName).Unwrap();
                mc.Id = id;
                mc.applyConfig(newConfig);
                mMcMap.Add(id, mc);
            }

            //TransMap
            if (mAxisTransMap == null) mAxisTransMap = new Dictionary<AlignAxis, AlignerMcTransform>();
            if (mTransAxisMap == null) mTransAxisMap = new Dictionary<AlignerMcTransform, AlignAxis>();
            mAxisTransMap.Clear();
            mTransAxisMap.Clear();

            foreach (AlignAxis axis in axisMcMap.Keys)//현재 필요..mAxisMcMap -> 임시변수로 변환
            {
                McAxis mcAxis = axisMap[axis];
                McId mcId = axisMcMap[axis];
                IMc mc = mMcMap[mcId];

                AlignerMcTransform trans = new AlignerMcTransform(this, axis, mc, mcAxis);
                mAxisTransMap.Add(axis, trans);
                mTransAxisMap.Add(trans, axis);
            }
            mIsConfigSet = true;
        }

        #endregion --IConfig


        protected void saveDynamicData()
        {
            foreach (var mc in mMcMap.Values) mc.SaveDynamicData();
        }
        protected void saveDynamicData(AlignAxis axis)
        {
            var t = mAxisTransMap[axis];
            t.mc.SaveDynamicData(t.mcAxis);
        }
        protected void waitAndSaveDynamicData(AlignAxis axis)
        {
            ThreadStart ts = new ThreadStart(() =>
            {
                var p = mAxisTransMap[axis];
                while (p.mc.isMoving(p.mcAxis)) Thread.Sleep(50);
                p.mc.SaveDynamicData(p.mcAxis);
            });
            var t = new Thread(ts);
            t.Name = $"{Name}{axis}";
            t.Start();
        }

        #endregion ===



        #region === Open & Close ===

        //Open
        protected override void performOpen()
        {
            //foreach (AlignerMcTransform trans in mAxisTransMap.Values)
            foreach (var mc in mMcMap.Values)
            {
                if (!mc.IsOpen) mc.Open();
            }
        }

        //Close
        protected override void performClose()
        {
            //foreach (AlignerMcTransform trans in mAxisTransMap.Values)
            foreach (IMc mc in mMcMap.Values)
            {
                if (mc.IsOpen) mc.Close();
            }
        }

        #endregion ===




        #region === Aligner ~ MC Transform ===

        //map
        protected Dictionary<AlignAxis, AlignerMcTransform> mAxisTransMap;
        protected Dictionary<AlignerMcTransform, AlignAxis> mTransAxisMap;
        protected Dictionary<McId, IMc> mMcMap;// = new Dictionary<McId, IMc>();


        public List<AlignAxis> EffectiveAxis
        {
            get
            {
                return mAxisTransMap.Keys.ToList();
            }
        }

        public IMc getMc(AlignAxis axis)
        {
            if (mAxisTransMap.ContainsKey(axis)) return mAxisTransMap[axis].mc;
            //if (mAxisMap.ContainsKey(axis)) return mMcMap[mAxisMcMap[axis]];
            else return null;
            //throw new Exception(string.Format("getMc(): AlignAxis '{0}'에 할당된 MC가 없습니다.", axis));
        }
        public McAxis getMcAxis(AlignAxis axis)
        {
            if (mAxisTransMap.ContainsKey(axis)) return mAxisTransMap[axis].mcAxis;
            //if (mAxisMap.ContainsKey(axis)) return mAxisMap[axis];
            else return McAxis.None;
            //throw new Exception(string.Format("getMcAxis(): AlignAxis '{0}'에 할당된 MC가 없습니다.", axis));
        }
        public AlignAxis getAlignAxis(IMc mc, McAxis mcAxis)
        {
            AlignerMcTransform trans = new AlignerMcTransform(this, mc, mcAxis);
            if (mTransAxisMap.ContainsKey(trans)) return mTransAxisMap[trans];
            else return AlignAxis.None;
        }


        #endregion



        #region === Status & Reporting ===

        IProgress<AlignerMotionParam> mReporter;
        AlignerMotionParam mMotionParam;

        private void mcReport(McMotionParam param)
        { }

        public async void reportStatusAsync(AlignerMotionParam param, IProgress<AlignerMotionParam> reporter)
        {
            if (reporter == null) return;

            if (this.mMotionParam == null) this.mMotionParam = new AlignerMotionParam(this);
            this.mReporter = reporter;

            foreach (AlignAxis axis in mAxisTransMap.Keys)
            {
                mAxisTransMap[axis].mc.reportStatusAsync(param.mParamMap[axis], new Progress<McMotionParam>(mcReport));
            }

            do
            {
                await Task.Delay(101);

                //report progress
                if (mReporter != null) mReporter.Report(param);

                //} while (!param.isError && !param.isStop && !param.isComplete && mReporter != null);
            } while (mReporter != null);
        }


        #endregion



        #region === Drive Methods ===

        public bool MoveFast(AlignAxis axis, double dx)
        {
            if (!mAxisTransMap.ContainsKey(axis)) return false;

            AlignerMcTransform trans = mAxisTransMap[axis];
            McMotionParam param = new McMotionParam(trans.mc, trans.mcAxis, dx);
            trans.mc.move(param);
            trans.mc.SaveDynamicData(trans.mcAxis);

            return true;
        }

        //public bool Zeroing(AlignAxis axis)
        //{
        //    AlignerMcTransform ic = mAxisTransMap[axis];

        //    ic.mc.moveToOrigin(ic.mcAxis);
        //    ic.mc.resetPosition(0, ic.mcAxis);
        //    ic.mc.moveToStrokeCenter(ic.mcAxis);
        //    ic.mc.SaveDynamicData(ic.mcAxis);

        //    return true;
        //}
        //public bool Zeroing()
        //{
        //    foreach (AlignAxis axis in mAxisTransMap.Keys) Zeroing(axis);

        //    return true;
        //}

        #endregion







    }//class

}
