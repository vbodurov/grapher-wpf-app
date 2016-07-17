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
    }

}