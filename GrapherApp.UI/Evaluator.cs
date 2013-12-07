using System;
using System.Collections.Generic;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;


namespace GrapherApp.UI
{
    /// <summary>
    /// to test functions: http://cubic-bezier.com/#.26,1.66,.77,.3
    /// </summary>
    public static class f
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
                r0 = -c/b;
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
                r0 = -b/(2*a);
                return 1;
            }
            var n = -b/(2*a);
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
            var rootsLength = GetCubicRoots(c0, c1, c2, c3-x, out r0, out r1, out r2); 
            var time = Double.NaN;
            if (rootsLength == 0)
                    time = 0;
            else if (rootsLength == 1)
                    time = r0;
            else  
            {


                for (var i = 0; i < rootsLength; ++i )
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
        }
               
    }

    public class Evaluator
    {
        private const string Functions = @"
const double PI = Math.PI;
const double E = Math.E;
const double pi = Math.PI;
const double e = Math.E;
double lerp(double a, double b, double c) { return (b - a) * c + a; }
double avg(double a, double b) { return (b - a) * 0.5 + a; }
double pow(double x, double n) { return Math.Pow(x, n); }
double abs(double x) { return Math.Abs(x); }
double acos(double x) { return Math.Acos(x); }
double asin(double x) { return Math.Asin(x); }
double atan(double x) { return Math.Atan(x); }
double atan2(double x, double y) { return Math.Atan2(x, y); }
double sin(double x) { return Math.Sin(x); }
double sinh(double x) { return Math.Sinh(x); }
double cos(double x) { return Math.Cos(x); }
double cosh(double x) { return Math.Cosh(x); }
double tan(double x) { return Math.Tan(x); }
double tanh(double x) { return Math.Tanh(x); }
double sqrt(double x) { return Math.Sqrt(x); }
double sign(double x) { return Math.Sign(x); }
double max(double x, double y) { return Math.Max(x, y); }
double min(double x, double y) { return Math.Min(x, y); }
double exp(double x) { return Math.Exp(x); }
double floor(double x) { return Math.Floor(x); }
double ceiling(double x) { return Math.Ceiling(x); }
double round(double x) { return Math.Round(x); }
double log(double x) { return Math.Log(x); }
double log(double x, double y) { return Math.Log(x, y); }
double log10(double x) { return Math.Log10(x); }
double bezier(double x, double bx, double by, double cx, double cy) { return f.Bezier(x, bx, by, cx, cy); }
double bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return f.Bezier(x, ax, ay, bx, by, cx, cy, dx, dy); }

double Lerp(double a, double b, double c) { return lerp(a, b, c); }
double Avg(double a, double b) { return avg(a,b); }
double Pow(double x, double n) { return pow(x, n); }
double Abs(double x) { return abs(x); }
double Acos(double x) { return acos(x); }
double Asin(double x) { return asin(x); }
double Atan(double x) { return atan(x); }
double Atan2(double x, double y) { return atan2(x, y); }
double Sin(double x) { return sin(x); }
double Sinh(double x) { return sinh(x); }
double Cos(double x) { return cos(x); }
double Cosh(double x) { return cosh(x); }
double Tan(double x) { return tan(x); }
double Tanh(double x) { return tanh(x); }
double Sqrt(double x) { return sqrt(x); }
double Sign(double x) { return sign(x); }
double Max(double x, double y) { return max(x, y); }
double Min(double x, double y) { return min(x, y); }
double Exp(double x) { return exp(x); }
double Floor(double x) { return floor(x); }
double Ceiling(double x) { return ceiling(x); }
double Round(double x) { return round(x); }
double Log(double x) { return log(x); }
double Log(double x, double y) { return log(x, y); }
double Log10(double x) { return log10(x); }
double Bezier(double x, double bx, double by, double cx, double cy) { return f.Bezier(x, bx, by, cx, cy); }
double Bezier(double x, double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy) { return f.Bezier(x, ax, ay, bx, by, cx, cy, dx, dy); }
                ";



        public Evaluator()
        {
            SetUp();
        }

        private Session _session;
        private void SetUp()
        {
            var scriptEngine = new ScriptEngine();
            _session = scriptEngine.CreateSession();
            _session.AddReference("System");
            _session.AddReference("System.Core");
            _session.AddReference(this.GetType().Assembly);

            _session.ImportNamespace("System");
            _session.ImportNamespace("GrapherApp.UI");

            _session.Execute(Functions);
        }

        public Func<double,double> GetFunction(string source)
        {
            source = source.Trim();
            var lines = source.Split('\n');
            var lastLine = lines[lines.Length - 1].Trim();
            if (source.IndexOf("return", StringComparison.InvariantCulture) < 0)
                lines[lines.Length - 1] = "return " + lines[lines.Length - 1];
            if (!lastLine.EndsWith(";"))
                lines[lines.Length - 1] = lines[lines.Length - 1] + ";";
            source = String.Join("\n", lines);
            return _session.Execute<Func<double, double>>(@"
                Func<double, double> factory() { 
                    return x => { "+source+@" }; 
                 } 
                 factory();");
        }
    }
}