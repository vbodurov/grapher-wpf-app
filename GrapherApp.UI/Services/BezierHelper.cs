using System;

namespace GrapherApp.UI.Services
{
    // to test functions: http://cubic-bezier.com/#.26,1.66,.77,.3
    public static class BezierHelper
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
    }
}