using System;

namespace GrapherApp.UI
{
    public static class Extensions
    {
        public static double FromRangeTo01(this double xOfRange, double xMin, double xMax)
        {
            var diff = xMax - xMin;
            return (Math.Abs(diff) < 0.0000001 ? 1.0 : (xOfRange - xMin) / diff);
        }
        public static double From01ToRange(this double x01, double xMin, double xMax)
        {
            return ((xMax - xMin) * x01 + xMin);
        }

        public static double From01To(this double x, double yForX0, double yForX1)
        {
            return (x * (yForX1 - yForX0) + yForX0);
        }
        public static double FromMin11To01(this double x)
        {
            return (x) * 0.5f + 0.5f;
        }
        public static double From01ToMin11(this double x)
        {
            return 2 * x - 1f;
        }
    }
}