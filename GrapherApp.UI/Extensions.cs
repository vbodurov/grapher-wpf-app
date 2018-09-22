using System;

namespace GrapherApp.UI
{
    public static class Extensions
    {
        public static float Float(this double n)
        {
            return (float)n;
        }
        public static double Double(this float n)
        {
            return n;
        }
        public static double Negate(this double n)
        {
            return n * -1;
        }
        public static double FromRangeTo01(this double xOfRange, double xMin, double xMax)
        {
            var diff = xMax - xMin;
            return (double)(Math.Abs(diff) < 0.0000001 ? 1.0 : (xOfRange - xMin) / diff);
        }
        public static double FromRangeToM11(this double xOfRange, double xMin, double xMax)
        {
            var diff = xMax - xMin;
            return (double)(2 * (Math.Abs(diff) < 0.0000001 ? 1.0 : (xOfRange - xMin) / diff) - 1);
        }
        public static double From01ToRange(this double x01, double xMin, double xMax)
        {
            return (double)((xMax - xMin) * x01 + xMin);
        }
        public static double FromM11ToRange(this double x01, double xMin, double xMax)
        {
            return (double)((xMax - xMin) * x01 * 0.5 + xMin + (xMax - xMin) * 0.5);
        }
        public static double From01To10(this double x)
        {
            return 1f - x;
        }
        public static double FromMin11To01(this double x)
        {
            return x * 0.5f + 0.5f;
        }
        public static double From01ToMin11(this double x)
        {
            return 2 * x - 1f;
        }
        public static int RoundAsInt(this double n, double multiplier)
        {
            return (int)Math.Round(n * multiplier, 0);
        }
        public static int RoundAsInt(this double n)
        {
            return (int)Math.Round(n, 0);
        }
        public static double Round(this double n)
        {
            return Math.Round(n, 0);
        }
        public static double Round(this double n, int digits)
        {
            return Math.Round(n, digits);
        }
        public static double WithFunc(this double n, Func<double, double> func)
        {
            return func(n);
        }
        public static double ClampMin11(this double value)
        {
            if (value < -1f)
            {
                return -1f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }
        public static double Clamp01(this double x)
        {
            if (x < 0f)
            {
                return 0f;
            }
            if (x > 1f)
            {
                return 1f;
            }
            return x;
        }
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min)
            {
                value = (float)min;
                return value;
            }
            if (value > max)
            {
                value = (float)max;
            }
            return value;
        }
        public static double NoMoreThan(this double a, double b) { return a > b ? (float)b : a; }
        public static double NoLessThan(this double a, double b) { return a < b ? (float)b : a; }

    }
}