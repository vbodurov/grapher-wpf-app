using System;
using System.Collections.Generic;
using System.Windows.Media;
// ReSharper disable InconsistentNaming
namespace GrapherApp.UI.Services
{
    public abstract class BaseFuncRunner
    {

        private IBezierGroupBuilder _bezierGroupBuilder;
        public static readonly Dictionary<string, double> state =
            new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase);

        public abstract double Run(double x);
        public virtual void GraphDrawingStarts(Color color, string source)
        {
            _bezierGroupBuilder = new BezierGroup();
            state.Clear();
        }
        public virtual void GraphDrawingEnds(Color color, string source) { }

        protected IBezierGroup beziers(Action<IBezierGroupBuilder> setup)
        {
            return _bezierGroupBuilder.setup(setup);
        }

        public IBezierGroup BezierGroup => (IBezierGroup)_bezierGroupBuilder;

        protected BaseFuncRunner fn => this;
        protected const double PI = Math.PI;
        protected const double E = Math.E;
        protected double time => Time.time;
        protected double noise(double x) { return NoiseGenerator.Generate(x); }
        protected double clamp(double n, double min, double max) { return n < min ? min : n > max ? max : n; }
        protected double clamp01(double n) { return clamp(n,0,1); }
        protected double clampM11(double n) { return clamp(n, -1, 1); }
        protected double lerp(double a, double b, double c) { return (b - a) * c + a; }
        protected double avg(double a, double b) { return (b - a) * 0.5 + a; }
        protected double pow(double x, double n) { return Math.Pow(x, n); }
        protected double abs(double x) { return Math.Abs(x); }
        protected double acos(double x) { return Math.Acos(x); }
        protected double asin(double x) { return Math.Asin(x); }
        protected double atan(double x) { return Math.Atan(x); }
        protected double atan2(double x, double y) { return Math.Atan2(x, y); }
        protected double sin(double x) { return Math.Sin(x); }
        protected double sinh(double x) { return Math.Sinh(x); }
        protected double cos(double x) { return Math.Cos(x); }
        protected double cosh(double x) { return Math.Cosh(x); }
        protected double tan(double x) { return Math.Tan(x); }
        protected double tanh(double x) { return Math.Tanh(x); }
        protected double sqrt(double x) { return Math.Sqrt(x); }
        protected double sign(double x) { return Math.Sign(x); }
        protected double max(double x, double y) { return Math.Max(x, y); }
        protected double max(double a, double b, double c) { return Math.Max(Math.Max(a, b), c); }
        protected double max(double a, double b, double c, double d) { return Math.Max(Math.Max(Math.Max(a, b), c), d); }
        protected double max(double a, double b, double c, double d, double e) { return Math.Max(Math.Max(Math.Max(Math.Max(a, b), c), d), e); }
        protected double max(double a, double b, double c, double d, double e, double f) { return Math.Max(Math.Max(Math.Max(Math.Max(Math.Max(a, b), c), d), e), f); }
        protected double min(double x, double y) { return Math.Min(x, y); }
        protected double min(double a, double b, double c) { return Math.Min(Math.Min(a, b), c); }
        protected double min(double a, double b, double c, double d) { return Math.Min(Math.Min(Math.Min(a, b), c), d); }
        protected double min(double a, double b, double c, double d, double e) { return Math.Min(Math.Min(Math.Min(Math.Min(a, b), c), d), e); }
        protected double min(double a, double b, double c, double d, double e, double f) { return Math.Min(Math.Min(Math.Min(Math.Min(Math.Min(a, b), c), d), e), f); }
        protected double exp(double x) { return Math.Exp(x); }
        protected double floor(double x) { return Math.Floor(x); }
        protected double ceiling(double x) { return Math.Ceiling(x); }
        protected double round(double x) { return Math.Round(x); }
        protected double log(double x) { return Math.Log(x); }
        protected double log(double x, double y) { return Math.Log(x, y); }
        protected double log10(double x) { return Math.Log10(x); }
        protected double bezier(double x, double bx, double by, double cx, double cy) { return BezierHelper.GetY(x, 0, 0, bx, by, cx, cy, 1 ,1); }
        protected double bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return BezierHelper.GetY(x, ax, ay, bx, by, cx, cy, dx, dy); }
        protected double bezier2parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1, 
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2, 
            double dx2, double dy2)
        {
            if (x <= ax2)
            {
                return (float)BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, ax2, ay2);
            }
            return (float)BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, dx2, dy2);
        }
        protected double smoothstep(double x)
        {
            return smoothstep(0, 1, x);
        }
        protected double smoothstep(double edge0, double edge1, double x)
        {
            // Scale, bias and saturate x to 0..1 range
            x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            // Evaluate polynomial
            return x * x * (3 - 2 * x);
        }
        protected double smootherstep(double x)
        {
            return smootherstep(0, 1, x);
        }
        protected double smootherstep(double edge0, double edge1, double x)
        {
            // Scale, and clamp x to 0..1 range
            x = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            // Evaluate polynomial
            return x * x * x * (x * (x * 6 - 15) + 10);
        }
        protected double bezier3parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1, 
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2, 
            double ax3, double ay3, double bx3, double by3, double cx3, double cy3, 
            double dx3, double dy3)
        {
            if (x <= ax2)
            {
                return (float)BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, ax2, ay2);
            }
            if (x <= ax3)
            {
                return (float)BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, ax3, ay3);
            }
            return (float)BezierHelper.GetY(x, ax3, ay3, bx3, by3, cx3, cy3, dx3, dy3);
        }
        protected double bezier4parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1,
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2,
            double ax3, double ay3, double bx3, double by3, double cx3, double cy3,
            double ax4, double ay4, double bx4, double by4, double cx4, double cy4, 
            double dx4, double dy4)
        {

            if (x <= ax2)
            {
                return BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, ax2, ay2);
            }
            if (x <= ax3)
            {
                return BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, ax3, ay3);
            }
            if (x <= ax4)
            {
                return BezierHelper.GetY(x, ax3, ay3, bx3, by3, cx3, cy3, ax4, ay4);
            }
            return BezierHelper.GetY(x, ax4, ay4, bx4, by4, cx4, cy4, dx4, dy4);
        }
        protected static double uniFun(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1)
        {
            double y;
            if (x < f1StartX) y = f0(x);
            else y = f1(x);
            return y;
        }
        protected static double uniFun(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1,
            double f2StartX, Func<double, double> f2)
        {
            double y;
            if (x < f1StartX) y = f0(x);
            else if (x < f2StartX) y = f1(x);
            else y = f2(x);
            return y;
        }
        protected static double uniFun(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1,
            double f2StartX, Func<double, double> f2,
            double f3StartX, Func<double, double> f3)
        {
            double y;
            if (x < f1StartX) y = f0(x);
            else if (x < f2StartX) y = f1(x);
            else if (x < f3StartX) y = f2(x);
            else y = f3(x);
            return y;
        }
        protected static double uniFun(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1,
            double f2StartX, Func<double, double> f2,
            double f3StartX, Func<double, double> f3,
            double f4StartX, Func<double, double> f4)
        {
            double y;
            if (x < f1StartX) y = f0(x);
            else if (x < f2StartX) y = f1(x);
            else if (x < f3StartX) y = f2(x);
            else if (x < f4StartX) y = f3(x);
            else y = f4(x);
            return y;
        }
        protected static double uniFun(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1,
            double f2StartX, Func<double, double> f2,
            double f3StartX, Func<double, double> f3,
            double f4StartX, Func<double, double> f4,
            double f5StartX, Func<double, double> f5)
        {
            double y;
            if (x < f1StartX) y = f0(x);
            else if (x < f2StartX) y = f1(x);
            else if (x < f3StartX) y = f2(x);
            else if (x < f4StartX) y = f3(x);
            else if (x < f5StartX) y = f4(x);
            else y = f5(x);
            return y;
        }
        protected static double uniFun01(double x, Func<double, double> f0,
            double f1StartX, Func<double, double> f1)
        {
            double y;
            if (x < f1StartX) y = f0(x.FromRangeTo01(0, f1StartX));
            else y = f1(x.FromRangeTo01(f1StartX, 1));
            return y;
        }
        protected static double uniFun01(double x, Func<double, double> f0,
           double f1StartX, Func<double, double> f1,
           double f2StartX, Func<double, double> f2)
        {
            double y;
            if (x < f1StartX) y = f0(x.FromRangeTo01(0, f1StartX));
            else if (x < f2StartX) y = f1(x.FromRangeTo01(f1StartX, f2StartX));
            else y = f2(x.FromRangeTo01(f2StartX, 1));
            return y;
        }
        protected static double uniFun01(double x, Func<double, double> f0,
           double f1StartX, Func<double, double> f1,
           double f2StartX, Func<double, double> f2,
           double f3StartX, Func<double, double> f3)
        {
            double y;
            if (x < f1StartX) y = f0(x.FromRangeTo01(0, f1StartX));
            else if (x < f2StartX) y = f1(x.FromRangeTo01(f1StartX, f2StartX));
            else if (x < f3StartX) y = f2(x.FromRangeTo01(f2StartX, f3StartX));
            else y = f3(x.FromRangeTo01(f3StartX, 1));
            return y;
        }
        protected static double uniFun01(double x, Func<double, double> f0,
           double f1StartX, Func<double, double> f1,
           double f2StartX, Func<double, double> f2,
           double f3StartX, Func<double, double> f3,
           double f4StartX, Func<double, double> f4)
        {
            double y;
            if (x < f1StartX) y = f0(x.FromRangeTo01(0, f1StartX));
            else if (x < f2StartX) y = f1(x.FromRangeTo01(f1StartX, f2StartX));
            else if (x < f3StartX) y = f2(x.FromRangeTo01(f2StartX, f3StartX));
            else if (x < f4StartX) y = f3(x.FromRangeTo01(f3StartX, f4StartX));
            else y = f4(x.FromRangeTo01(f4StartX, 1));
            return y;
        }
        protected static double uniFun01(double x, Func<double, double> f0,
           double f1StartX, Func<double, double> f1,
           double f2StartX, Func<double, double> f2,
           double f3StartX, Func<double, double> f3,
           double f4StartX, Func<double, double> f4,
           double f5StartX, Func<double, double> f5)
        {
            double y;
            if (x < f1StartX) y = f0(x.FromRangeTo01(0, f1StartX));
            else if (x < f2StartX) y = f1(x.FromRangeTo01(f1StartX, f2StartX));
            else if (x < f3StartX) y = f2(x.FromRangeTo01(f2StartX, f3StartX));
            else if (x < f4StartX) y = f3(x.FromRangeTo01(f3StartX, f4StartX));
            else if (x < f5StartX) y = f4(x.FromRangeTo01(f4StartX, f5StartX));
            else y = f5(x.FromRangeTo01(f5StartX, 1));
            return y;
        }
        protected double Noise(double x) { return NoiseGenerator.Generate(x); }
        protected double Clamp(double n, double min, double max) { return n < min ? min : n > max ? max : n; }
        protected double Lerp(double a, double b, double c) { return lerp(a, b, c); }
        protected double Avg(double a, double b) { return avg(a, b); }
        protected double Pow(double x, double n) { return pow(x, n); }
        protected double Abs(double x) { return abs(x); }
        protected double Acos(double x) { return acos(x); }
        protected double Asin(double x) { return asin(x); }
        protected double Atan(double x) { return atan(x); }
        protected double Atan2(double x, double y) { return atan2(x, y); }
        protected double Sin(double x) { return sin(x); }
        protected double Sinh(double x) { return sinh(x); }
        protected double Cos(double x) { return cos(x); }
        protected double Cosh(double x) { return cosh(x); }
        protected double Tan(double x) { return tan(x); }
        protected double Tanh(double x) { return tanh(x); }
        protected double Sqrt(double x) { return sqrt(x); }
        protected double Sign(double x) { return sign(x); }
        protected double Max(double x, double y) { return max(x, y); }
        protected double Min(double x, double y) { return min(x, y); }
        protected double Exp(double x) { return exp(x); }
        protected double Floor(double x) { return floor(x); }
        protected double Ceiling(double x) { return ceiling(x); }
        protected double Round(double x) { return round(x); }
        protected double Log(double x) { return log(x); }
        protected double Log(double x, double y) { return log(x, y); }
        protected double Log10(double x) { return log10(x); }
        protected double Bezier(double x, double bx, double by, double cx, double cy) { return BezierHelper.GetY(x, 0, 0, bx, by, cx, cy, 1 ,1); }
        protected double Bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return BezierHelper.GetY(x, ax, ay, bx, by, cx, cy, dx, dy); }
        protected double Bezier2Parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1, double dx1, double dy1,
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2, double dx2, double dy2)
        {
            if (abs(dx1 - ax2) > 0.001 || abs(dy1 - ay2) > 0.001)
                throw new ArgumentException($"Not matching bezier curves {dx1},{dy1} != {ax2},{ay2}");

            if (x <= dx1)
            {
                return BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, dx1, dy1);
            }
            return BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, dx2, dy2);
        }

        protected double dotProductByDegree(double degree)
        {
            var iDeg = (int)abs(Math.Round(degree));
            if (iDeg > 180) iDeg = iDeg % 180;
            return DotProductByDegree[iDeg];
        }
        static readonly float[] DotProductByDegree = { 1f, 0.9998477f, 0.9993908f, 0.9986295f, 0.9975641f, 0.9961947f, 0.9945219f, 0.9925461f, 0.9902681f, 0.9876884f, 0.9848077f, 0.9816272f, 0.9781476f, 0.9743701f, 0.9702957f, 0.9659258f, 0.9612617f, 0.9563048f, 0.9510565f, 0.9455186f, 0.9396926f, 0.9335804f, 0.9271839f, 0.9205049f, 0.9135454f, 0.9063078f, 0.8987941f, 0.8910065f, 0.8829476f, 0.8746197f, 0.8660254f, 0.8571673f, 0.8480481f, 0.8386706f, 0.8290376f, 0.8191521f, 0.809017f, 0.7986355f, 0.7880108f, 0.7771459f, 0.7660445f, 0.7547096f, 0.7431449f, 0.7313538f, 0.7193398f, 0.7071067f, 0.6946584f, 0.6819984f, 0.6691306f, 0.656059f, 0.6427876f, 0.6293204f, 0.6156615f, 0.601815f, 0.5877852f, 0.5735765f, 0.5591929f, 0.5446391f, 0.5299193f, 0.5150381f, 0.5f, 0.4848096f, 0.4694716f, 0.4539905f, 0.4383711f, 0.4226182f, 0.4067366f, 0.3907312f, 0.3746066f, 0.3583679f, 0.3420201f, 0.3255681f, 0.309017f, 0.2923717f, 0.2756374f, 0.258819f, 0.2419218f, 0.224951f, 0.2079117f, 0.1908091f, 0.1736482f, 0.1564345f, 0.1391731f, 0.1218693f, 0.1045284f, 0.08715588f, 0.06975645f, 0.05233604f, 0.03489941f, 0.01745242f, 0f, -0.01745248f, -0.03489947f, -0.05233586f, -0.06975651f, -0.0871557f, -0.1045285f, -0.1218693f, -0.139173f, -0.1564344f, -0.1736481f, -0.1908089f, -0.2079116f, -0.224951f, -0.2419218f, -0.258819f, -0.2756375f, -0.2923716f, -0.3090171f, -0.3255682f, -0.3420202f, -0.3583679f, -0.3746065f, -0.3907312f, -0.4067366f, -0.4226184f, -0.4383712f, -0.4539905f, -0.4694716f, -0.4848095f, -0.5000001f, -0.515038f, -0.5299193f, -0.5446391f, -0.5591928f, -0.5735766f, -0.5877852f, -0.601815f, -0.6156615f, -0.6293204f, -0.6427877f, -0.656059f, -0.6691307f, -0.6819984f, -0.6946582f, -0.7071067f, -0.7193398f, -0.7313538f, -0.7431448f, -0.7547096f, -0.7660444f, -0.777146f, -0.7880107f, -0.7986356f, -0.8090171f, -0.819152f, -0.8290374f, -0.8386706f, -0.8480481f, -0.8571672f, -0.8660253f, -0.8746197f, -0.8829476f, -0.8910065f, -0.8987941f, -0.9063078f, -0.9135456f, -0.9205048f, -0.9271837f, -0.9335804f, -0.9396925f, -0.9455187f, -0.9510566f, -0.9563048f, -0.9612616f, -0.9659257f, -0.9702957f, -0.9743701f, -0.9781476f, -0.9816272f, -0.9848078f, -0.9876882f, -0.9902682f, -0.9925461f, -0.9945219f, -0.9961947f, -0.9975641f, -0.9986296f, -0.9993908f, -0.9998477f, -1f };
    }

}