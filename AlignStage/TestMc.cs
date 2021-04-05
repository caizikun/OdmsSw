using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Free302.TnM.Device
{
    public class TestMc : McBase<StageBase>
    {
        public TestMc(): this(McId.MC1, "PMC4B")
        {
        }
        public TestMc(McId id, string name) : base(id, name)
        {
            _coords = new Dictionary<McAxis, double>();
            _moving = new Dictionary<McAxis, int>();
        }

        Dictionary<McAxis, double> _coords;
        Dictionary<McAxis, int> _moving;

        public override void applyConfig(DeviceConfig newConfig)
        {
            base.applyConfig(newConfig);
            _coords.Clear();
            EffectiveAxis.ForEach(x => { _coords.Add(x, 0); _moving.Add(x, 0); });
        }

        public override Dictionary<McAxis, double> getPosition(McAxis axis = McAxis.All)
        {
            return EffectiveAxes(axis).ToDictionary(x => x, x => _coords[x]);
        }
        public override void move(McMotionParam param)
        {
            var dx = param.dx0;
            if (param.moveType == McMoveType.MoveTo) dx = param.dx - _coords[param.axis];
            _coords[param.axis] += dx;

            var dt = (int)(1000 * Math.Abs(dx) / param.speed); // getStage(param.axis).SpeedMap[McSpeed.Fast]);
            _moving[param.axis] = 1;

            Task.Delay(dt).ContinueWith(t => _moving[param.axis] = 0);//.Wait();//.Start();
        }
        public override void moveToLogicalOrigin(McAxis axis = McAxis.All) => moveToStrokeCenter(axis);
        public override void moveToOrigin(McAxis axis = McAxis.All)
        {
            var axes = EffectiveAxes(axis);
            axes.Sum(x => (_moving[x] = 1) + (_coords[x] = 0));
            Task.Delay(3000).ContinueWith(t => axes.Sum(x => _moving[x] = 0));//.Start();
        }
        public override void moveToStrokeCenter(McAxis axis = McAxis.All)
        {
            var axes = EffectiveAxes(axis);
            axes.Sum(x => (_moving[x] = 1) + (_coords[x] = getStage(x).StrokeLength / 2));
            Task.Delay(3000).ContinueWith(t => axes.Sum(x => _moving[x] = 0));//.Start();
        }
        public override void readMotionStatus(McMotionParam param)
        {
            //position
            param.x = _coords[param.axis];

            param.isError = false;
            param.isStop = _moving[param.axis] == 0;

            param.isNorg = false;
            param.isOrg = false;
            param.isLmtP = _coords[param.axis] > getStage(param.axis).StrokeLength;
            param.isLmtN = _coords[param.axis] < 0;
        }
        public override void reportStatusAsync(McMotionParam param, IProgress<McMotionParam> reporter)
        {
            readMotionStatus(param);
            reporter?.Report(param);
        }
        public override void reportStatusAsync(McMotionParam[] paramList, IProgress<McMotionParam> reporter)
        {
            foreach(var p in paramList)
            {
                readMotionStatus(p);
                reporter?.Report(p);
            }
        }
        public override void resetPosition(McAxis axis = McAxis.All) => EffectiveAxes(axis).Sum(x => _coords[x] = 0);
        public override void stop(McAxis axis = McAxis.All) => EffectiveAxes(axis).Sum(x => _moving[x] = 0);
        protected override void performClose()
        {
        }
        protected override bool performIsMoving(McAxis axis = McAxis.All) => EffectiveAxes(axis).Sum(x => _moving[x]) != 0;
        protected override void performOpen()
        {
        }

    }//class
}
