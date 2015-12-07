using System;
using System.Linq;

/*
http://cubic-bezier.com/#.97,.37,.24,.85
var y =
  beziers(b => b.from(-1,-1)
    .to(0,0).curve(.26,-0.63,.24,.85)
    .to(1,1).curve(.55,.18,.79,1.5))
.run(x);
*/
namespace GrapherApp.UI.Services
{
    public interface IBezierGroup
    {
        double run(double x);
    }
    public interface IBezierGroupBuilder
    {
        PointDouble From { get; }
        double LastLimit { get; }
        IBezierGroup setup(Action<IBezierGroupBuilder> setup);
        IBezierGroupBuilder from(double x, double y);
        IBezierFragmentBuilder to(double x, double y);
    }
    public interface IBezierFragment
    {
        double run(double x);
        IBezierGroupBuilder GroupBuilder { get; }
        PointDouble From { get; }
        PointDouble To { get; }
        PointDouble B { get; }
        PointDouble C { get; }
    }
    public interface IBezierFragmentBuilder
    {
        IBezierFragmentBuilder to(double x, double y);
        IBezierFragmentBuilder curve(double bx, double by, double cx, double cy);
    }
    public sealed class BezierGroup : IBezierGroup, IBezierGroupBuilder
    {
        private readonly IBezierGroupBuilder _builder;
        private RangeSet<IBezierFragment> _fragments;
        private bool _isSetUpFunctionInvoked = false;
        private PointDouble _from;

        public BezierGroup()
        {
            _builder = this;
            _from = new PointDouble(0,0);
            _fragments = new RangeSet<IBezierFragment>(0);
        }

        double IBezierGroup.run(double x)
        {
            IBezierFragment fragment;
            if (_fragments.TryFind(x, out fragment))
            {
                return fragment.run(x);
            }
            return 0.0;
        }
        PointDouble IBezierGroupBuilder.From { get { return _from; } }
        double IBezierGroupBuilder.LastLimit { get { return _fragments.LastLimit; } }
        IBezierGroup IBezierGroupBuilder.setup(Action<IBezierGroupBuilder> setup)
        {
            if (!_isSetUpFunctionInvoked)
            {
                setup(this);
                _isSetUpFunctionInvoked = true;
            }
            return this;
        }
        IBezierGroupBuilder IBezierGroupBuilder.from(double x, double y)
        {
            _from = new PointDouble(x,y);
            _fragments = new RangeSet<IBezierFragment>(x);
            return _builder;
        }
        IBezierFragmentBuilder IBezierGroupBuilder.to(double x, double y)
        {
            var lastPoint = _fragments.Count == 0 ? _from : _fragments.LastValue.To;
            var fragment = new BezierFragment(this, lastPoint, new PointDouble(x, y));
            _fragments.Add(x, fragment);
            return fragment;
        }
    }
    public sealed class BezierFragment : IBezierFragment, IBezierFragmentBuilder
    {
        private readonly IBezierGroupBuilder _groupBuilder;
        private readonly PointDouble _from;
        private readonly PointDouble _to;
        private readonly double _xRange;
        private readonly double _yRange;
        private PointDouble _b;
        private PointDouble _c;


        public BezierFragment(IBezierGroupBuilder groupBuilder, PointDouble from, PointDouble to)
        {
            _groupBuilder = groupBuilder;
            _from = from;
            _to = to;
            _b = new PointDouble(0.1, 0.5);
            _c = new PointDouble(0.5, 0.1);

            _xRange = _to.X - _from.X;
            _yRange = _to.Y - _from.Y;
        }

        IBezierGroupBuilder IBezierFragment.GroupBuilder { get { return _groupBuilder; } }
        PointDouble IBezierFragment.From { get { return _from; } }
        PointDouble IBezierFragment.To { get { return _to; } }
        PointDouble IBezierFragment.B { get { return _b; } }
        PointDouble IBezierFragment.C { get { return _c; } }

        double IBezierFragment.run(double x)
        {
            var relativeX = (x - _from.X)/_xRange;
            var relativeY = BezierHelper.Bezier(relativeX, _b.X, _b.Y, _c.X, _c.Y);
            return relativeY*_yRange + _from.Y;
        }
        IBezierFragmentBuilder IBezierFragmentBuilder.to(double x, double y)
        {
            return _groupBuilder.to(x, y);
        }
        IBezierFragmentBuilder IBezierFragmentBuilder.curve(double bx, double by, double cx, double cy)
        {
            _b = new PointDouble(bx, by);
            _c = new PointDouble(cx, cy);
            return this;
        }
    }
}