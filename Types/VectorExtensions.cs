using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using System.Diagnostics;

namespace Types
{
    public static class VectorExtensions
    {
        public static Vector<double> Add(this Vector<double> vector, double value)
        {
            return new Vector<double>(vector.Select(element => element + value));
        }

        public static Vector<double> AddElementWise(this Vector<double> vector, IEnumerable<double> collection)
        {
            return new Vector<double>(vector.Zip(collection, (a, b) => a + b));
        }

        public static Vector<double> Substract(this Vector<double> vector, double value)
        {
            return vector.Add(-value);
        }

        public static Vector<double> SubstractElementWise(this Vector<double> vector, IEnumerable<double> collection)
        {
            return vector.AddElementWise(new Vector<double>(collection).Negate());
        }

        public static Vector<double> Multiply(this Vector<double> vector, double value)
        {
            return new Vector<double>(vector.Select(element => element * value));
        }

        public static Vector<double> MultiplyElementWise(this Vector<double> vector, IEnumerable<double> collection)
        {
            return new Vector<double>(vector.Zip(collection, (a, b) => a * b));
        }

        public static Vector<double> Divide(this Vector<double> vector, double value)
        {
            return vector.Multiply(1.0/value);
        }

        public static Vector<double> DivideElementWise(this Vector<double> vector, IEnumerable<double> collection)
        {
            return vector.MultiplyElementWise(
                new Vector<double>(collection.Select(element => element = 1.0 / element)));
        }

        public static Vector<double> Negate(this Vector<double> vector)
        {
            return vector.Multiply(-1);
        }

        public static Vector<double> Pow(this Vector<double> vector, double power)
        {
            return new Vector<double>(vector.Select(element => Math.Pow(element, power)));
        }
    }
}