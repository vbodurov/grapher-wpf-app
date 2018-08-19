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
        protected float bezier2parts(double x,
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
        protected float bezier3parts(double x,
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
        protected float bezier4parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1,
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2,
            double ax3, double ay3, double bx3, double by3, double cx3, double cy3,
            double ax4, double ay4, double bx4, double by4, double cx4, double cy4, 
            double dx4, double dy4)
        {

            if (x <= ax2)
            {
                return (float)BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, ax2, ay2);
            }
            if (x <= ax3)
            {
                return (float)BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, ax3, ay3);
            }
            if (x <= ax4)
            {
                return (float)BezierHelper.GetY(x, ax3, ay3, bx3, by3, cx3, cy3, ax4, ay4);
            }
            return (float)BezierHelper.GetY(x, ax4, ay4, bx4, by4, cx4, cy4, dx4, dy4);
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
        protected float Bezier2Parts(double x,
            double ax1, double ay1, double bx1, double by1, double cx1, double cy1, double dx1, double dy1,
            double ax2, double ay2, double bx2, double by2, double cx2, double cy2, double dx2, double dy2)
        {
            if (abs(dx1 - ax2) > 0.001 || abs(dy1 - ay2) > 0.001)
                throw new ArgumentException($"Not matching bezier curves {dx1},{dy1} != {ax2},{ay2}");

            if (x <= dx1)
            {
                return (float)BezierHelper.GetY(x, ax1, ay1, bx1, by1, cx1, cy1, dx1, dy1);
            }
            return (float)BezierHelper.GetY(x, ax2, ay2, bx2, by2, cx2, cy2, dx2, dy2);
        }
    }

}