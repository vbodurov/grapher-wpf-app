using System;
using System.Collections.Generic;

namespace GrapherApp.UI.Services
{
    public abstract class BaseFuncRunner
    {
        public abstract double Run(double x);

        protected BaseFuncRunner fn => this;
        protected const double PI = Math.PI;
        protected const double E = Math.E;
        protected const double pi = Math.PI;
        protected const double e = Math.E;
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
        protected double bezier(double x, double bx, double by, double cx, double cy) { return SpecFuncs.Bezier(x, bx, by, cx, cy); }
        protected double bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return SpecFuncs.Bezier(x, ax, ay, bx, by, cx, cy, dx, dy); }

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
        protected double Bezier(double x, double bx, double by, double cx, double cy) { return SpecFuncs.Bezier(x, bx, by, cx, cy); }
        protected double Bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return SpecFuncs.Bezier(x, ax, ay, bx, by, cx, cy, dx, dy); }

    }

    // to test functions: http://cubic-bezier.com/#.26,1.66,.77,.3
    internal static class SpecFuncs
    {
        const double epsilon = 0.000000001;
        private static void GetCubicCoefficients(double a, double b, double c, double d, out double c0, out double c1, out double c2, out double c3)
        {
            c0 = -a + 3 * b - 3 * c + d;
            c1 = 3 * a - 6 * b + 3 * c;
            c2 = -3 * a + 3 * b;
            c3 = a;
        }
        private static int GetQuadraticRoots(double a, double b, double c, out double r0, out double r1)
        {
            r0 = r1 = 0;

            if (Math.Abs(a - 0) < epsilon)
            {
                if (Math.Abs(b - 0) < epsilon) return 0;
                r0 = -c / b;
                return 1;
            }

            var q = b * b - 4 * a * c;
            var signQ =
                        (q > 0) ? 1
                        : q < 0 ? -1
                        : 0;

            if (signQ < 0)
            {
                return 0;
            }
            if (Math.Abs(signQ - 0) < epsilon)
            {
                r0 = -b / (2 * a);
                return 1;
            }
            var n = -b / (2 * a);
            r0 = n;
            r1 = n;
            var tmp = Math.Sqrt(q) / (2 * a);
            r0 -= tmp;
            r1 += tmp;
            return 2;
        }
        private static int GetCubicRoots(double a, double b, double c, double d, out double r0, out double r1, out double r2)
        {
            r1 = r2 = 0;
            if (Math.Abs(a - 0) < epsilon) return GetQuadraticRoots(b, c, d, out r0, out r1);

            b /= a;
            c /= a;
            d /= a;

            var q = (b * b - 3 * c) / 9.0;
            var qCubed = q * q * q;
            var r = (2 * b * b * b - 9 * b * c + 27 * d) / 54.0;

            var diff = qCubed - r * r;
            if (diff >= 0)
            {
                if (Math.Abs(q - 0) < epsilon)
                {
                    r0 = 0.0;
                    return 1;
                }

                var theta = Math.Acos(r / Math.Sqrt(qCubed));
                var qSqrt = Math.Sqrt(q); // won't change

                r0 = -2 * qSqrt * Math.Cos(theta / 3.0) - b / 3.0;
                r1 = -2 * qSqrt * Math.Cos((theta + 2 * Math.PI) / 3.0) - b / 3.0;
                r2 = -2 * qSqrt * Math.Cos((theta + 4 * Math.PI) / 3.0) - b / 3.0;

                return 3;
            }
            var tmp = Math.Pow(Math.Sqrt(-diff) + Math.Abs(r), 1 / 3.0);
            var rSign = (r > 0) ? 1 : r < 0 ? -1 : 0;
            r0 = -rSign * (tmp + q / tmp) - b / 3.0;
            return 1;
        }
        public static double Bezier(double x, double bx, double by, double cx, double cy)
        {
            return Bezier(x, 0, 0, bx, by, cx, cy, 1, 1);
        }
        private static double GetSingleValue(double t, double a, double b, double c, double d)
        {
            return (t * t * (d - a) + 3 * (1 - t) * (t * (c - a) + (1 - t) * (b - a))) * t + a;
        }
        public static double Bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            if (ax < dx)
            {
                if (x <= ax + epsilon) return ay;
                if (x >= dx - epsilon) return dy;
            }
            else
            {
                if (x >= ax + epsilon) return ay;
                if (x <= dx - epsilon) return dy;
            }
            double c0, c1, c2, c3;
            GetCubicCoefficients(ax, bx, cx, dx, out c0, out c1, out c2, out c3);

            double r0, r1, r2;
            // x(t) = a*t^3 + b*t^2 + c*t + d
            var rootsLength = GetCubicRoots(c0, c1, c2, c3 - x, out r0, out r1, out r2);
            var time = Double.NaN;
            if (rootsLength == 0)
                time = 0;
            else if (rootsLength == 1)
                time = r0;
            else
            {


                for (var i = 0; i < rootsLength; ++i)
                {
                    var root = i == 0 ? r0 : i == 1 ? r1 : r2;
                    if (0 <= root && root <= 1)
                    {
                        time = root;
                        break;
                    }
                }
            }

            return Double.IsNaN(time) ? Double.NaN : GetSingleValue(time, ay, by, cy, dy);
        }

        public static readonly Dictionary<string, double> state =
            new Dictionary<string, double>(StringComparer.InvariantCultureIgnoreCase);

        public static void InitState(string name, double val)
        {
            if (!state.ContainsKey(name)) state[name] = val;
            state[name] = val;
        }

    }

}