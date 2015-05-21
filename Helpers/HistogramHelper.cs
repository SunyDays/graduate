using System;
using System.Linq;

namespace Helpers
{
    public static class HistogramHelper
    {
        public static double[] ComputeFrequency(double[] array, int min, int max, int n)
        {
            var step = ComputeStep(min, max, n);
            var frequency = new double[n];

            var j = 0;
            for (var i = min + step; (float)i <= max; i+= step, j++)
                frequency[j] = array.Count(e => ((i - step) <= e && e < i)) / (double)array.Length;

            return frequency;
        }

        public static double ComputeStep(int min, int max, int n)
        {
            return (max - min) / (double)n;
        }
    }
}